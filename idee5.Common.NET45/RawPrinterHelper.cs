// ================================================================================================
// Project : idee5.Common
// Author : Bernd Date
// created : 22.08.2014
// ================================================================================================
// Change history(changed on/by: )
// 1.0.0 : 22.08.2014 XX xxx
// ================================================================================================

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace idee5.Common.Net46 {
    /// <summary>
    /// Helper class to send raw data to a printer. Taken from http://support.microsoft.com/kb/322091/en-us
    /// </summary>
    public static class RawPrinterHelper {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
#pragma warning disable JustCode_NamingConventions // Naming conventions inconsistency
        public class DOCINFOA {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;

            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

#pragma warning restore JustCode_NamingConventions // Naming conventions inconsistency

        // Import non managed assemblies
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        /// <summary>
        /// Sends the given bytes to a printer.
        /// </summary>
        /// <param name="printerName">Name of the printer.</param>
        /// <param name="documentName">Name of the document.</param>
        /// <param name="buffer">
        /// A pointer to an array of bytes (unmanaged) that contains the data that should be written
        /// to the printer.
        /// </param>
        /// <param name="dwCount">The size, in bytes, of the array.</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool SendBytesToPrinter(string printerName, string documentName, IntPtr buffer, Int32 dwCount)
        {
            int dwError = 0, dwWritten = 0;
            var hPrinter = new IntPtr(value: 0);
            var di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = documentName;
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero)) {
                // Start a document.
                if (StartDocPrinter(hPrinter, level: 1, di: di)) {
                    // Start a page.
                    if (StartPagePrinter(hPrinter)) {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, buffer, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information about why not.
            if (!bSuccess) { dwError = Marshal.GetLastWin32Error(); }
            return bSuccess;
        }

        /// <summary>
        /// Sends the file to printer.
        /// </summary>
        /// <param name="printerName">Name of the printer.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static bool SendFileToPrinter(string printerName, string fileName)
        {
            // Open the file.
            var fs = new FileStream(fileName, FileMode.Open);
            // Create a BinaryReader on the file.
            var br = new BinaryReader(fs);
            return SendStreamToPrinter(printerName, fs, fileName);
        }

        public static bool SendStreamToPrinter(string printerName, Stream stream, string documentName)
        {
            // Create a BinaryReader on the stream.
            var br = new BinaryReader(stream);
            // Dim an array of bytes big enough to hold the file's contents.
            byte[] bytes = new byte[stream.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            var pUnmanagedBytes = new IntPtr(value: 0);
            int nLength = Convert.ToInt32(stream.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, startIndex: 0, destination: pUnmanagedBytes, length: nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(printerName, documentName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        public static bool SendStringToPrinter(string printerName, string documentName, string rawString)
        {
            IntPtr buffer;
              
            // How many characters are in the string?
            int size = rawString.Length;
            // Assume that the printer is expecting ANSI text, and then convert the string to ANSI text.
            buffer = Marshal.StringToCoTaskMemAnsi(rawString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(printerName, documentName, buffer, size);
            Marshal.FreeCoTaskMem(buffer);
            return true;
        }
    }
}