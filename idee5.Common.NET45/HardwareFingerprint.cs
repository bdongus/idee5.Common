// ==========================================================================
// Projekt: idee5.Common
// Autor: idee5 Erstellt am: 08.08.2013
// Beschreibung:
// ==========================================================================
// ==========================================================================
// Änderungshistorie (Geändert am/von: )
// 1.0.1 : 09.11.2015 BD new naming convention, C# 6.
// ==========================================================================

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Management;

namespace idee5.Common.Net46 {
    public class HardwareFingerprint {
        /// <summary>
        /// Erzeugt einen 32 Byte Fingerabdruck des Computers
        /// </summary>
        private static string _fingerprint;

        /// <summary>
        /// Erzeugt eine Instanz des Hardware Fingerabdrucks <see cref="HardwareFingerprint"/> class.
        /// </summary>
        /// <param name="cpu">Erste CPU berücksichtigen</param>
        /// <param name="bios">BIOS berücksichtigen</param>
        /// <param name="board">Hauptplatine berücksichtigen</param>
        /// <param name="disk">Erste Festplatte berücksichtigen</param>
        /// <param name="video">Erste Grafikkarte berücksichtigen</param>
        /// <param name="mac">MAC-Adresse der ersten Netzwerkkarte berücksichtigen</param>
        public HardwareFingerprint(bool cpu = true, bool bios = true, bool board = true, bool disk = false, bool video = false, bool mac = false) {
            string fp = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}{3}{4}{5}",
                cpu ? string.Format(CultureInfo.InvariantCulture, "CPU >> {0}", CpuId()) : "",
                bios ? string.Format(CultureInfo.InvariantCulture, "\nBIOS >> {0}", BiosId()) : "",
                board ? string.Format(CultureInfo.InvariantCulture, "\nBASE >> {0}", BaseId()) : "",
                disk ? string.Format(CultureInfo.InvariantCulture, "\nDISK >> {0}", DiskId()) : "",
                video ? string.Format(CultureInfo.InvariantCulture, "\nVIDEO >> {0}", VideoId()) : "",
                mac ? string.Format(CultureInfo.InvariantCulture, "\nMAC >> {0}", MacId()) : "");
            _fingerprint = GetHash(fp);
            _fingerprint = Regex.Replace(_fingerprint, pattern: "(.{4})", replacement: "$1-").TrimEnd(trimChars: new char[] { '-' });
        }

        /// <summary>
        /// Gets or sets the finger print.
        /// </summary>
        /// <value>The finger print.</value>
        public string GetFingerprint() { return _fingerprint; }

        private static string GetHash(string hashme) {
            // something about hashing: http://www.codinghorror.com/blog/2012/04/speed-hashing.html
            // Da es um Sicherheit geht und .NET 4+ nutzen, ignorieren wir Einschränkungen durch alte
            // Betriebssysteme http://windows.microsoft.com/de-CH/windows/products/lifecycle http://support.microsoft.com/lifecycle/default.aspx?LN=de&p1=3198&x=10&y=7
            var sec = SHA256.Create();
            byte[] bt = Encoding.UTF8.GetBytes(hashme);
            // http://regextester.net/live-javascript-regex-tester.php
            return BitConverter.ToString(sec.ComputeHash(bt)).Replace(oldValue: "-", newValue: "");
        }

        #region Original Device ID Getting Code

        /// <summary>
        /// Determine a hardware identifier
        /// </summary>
        /// <param name="wmiClass"></param>
        /// <param name="wmiProperty"></param>
        /// <param name="wmiMustBeTrue"></param>
        /// <returns>Hardware identifier </returns>
        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue = "") {
            string result = "";
            var mc = new ManagementClass(wmiClass);
            foreach (ManagementObject mo in mc.GetInstances()) {
                if (wmiMustBeTrue?.Length == 0 || mo[wmiMustBeTrue].ToString() == "True") {
                    //Only get the first one
                    if (result?.Length == 0) {
                        try {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch {
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Determine the CPU ID
        /// </summary>
        /// <returns>CPU ID</returns>
        private static string CpuId() {
            const string ProcessorClass = "Win32_Processor";
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = Identifier(wmiClass: ProcessorClass, wmiProperty: "UniqueId");
            if (retVal?.Length == 0) {
                retVal = Identifier(wmiClass: ProcessorClass, wmiProperty: "ProcessorId");
                if (retVal?.Length == 0) {
                    retVal = Identifier(wmiClass: ProcessorClass, wmiProperty: "Name");
                    if (retVal?.Length == 0)
                        retVal = Identifier(wmiClass: ProcessorClass, wmiProperty: "Manufacturer");
                }
            }
            return retVal;
        }


        /// <summary>
        /// Determine the BIOS identifier
        /// </summary>
        /// <returns>BIOS identifier</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "<Ausstehend>")]
        private static string BiosId() => String.Format(CultureInfo.InvariantCulture,
                "{0}{1}{2}{3}{4}{5}",
                Identifier(wmiClass: "Win32_BIOS", wmiProperty: "Manufacturer"),
                Identifier(wmiClass: "Win32_BIOS", wmiProperty: "SMBIOSBIOSVersion"),
                Identifier(wmiClass: "Win32_BIOS", wmiProperty: "IdentificationCode"),
                Identifier(wmiClass: "Win32_BIOS", wmiProperty: "SerialNumber"),
                Identifier(wmiClass: "Win32_BIOS", wmiProperty: "ReleaseDate"),
                Identifier(wmiClass: "Win32_BIOS", wmiProperty: "Version"));


        /// <summary>
        ///  Determine the main physical hard drive ID
        /// </summary>
        /// <returns>Main physical hard drive ID</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "<Ausstehend>")]
        private static string DiskId() => String.Format(
            CultureInfo.InvariantCulture,
            "{0}{1}{2}{3}",
            Identifier(wmiClass: "Win32_DiskDrive", wmiProperty: "Model"),
            Identifier(wmiClass: "Win32_DiskDrive", wmiProperty: "Manufacturer"),
            Identifier(wmiClass: "Win32_DiskDrive", wmiProperty: "Signature"),
            Identifier(wmiClass: "Win32_DiskDrive", wmiProperty: "TotalHeads"));

        /// <summary>
        /// Determine the motherboard ID
        /// </summary>
        /// <returns>Motherboard ID</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "<Ausstehend>")]
        private static string BaseId() => String.Format(
            CultureInfo.InvariantCulture,
            "{0}{1}{2}{3}",
            Identifier(wmiClass: "Win32_BaseBoard", wmiProperty: "Model"),
            Identifier(wmiClass: "Win32_BaseBoard", wmiProperty: "Manufacturer"),
            Identifier(wmiClass: "Win32_BaseBoard", wmiProperty: "Name"),
            Identifier(wmiClass: "Win32_BaseBoard", wmiProperty: "SerialNumber"));

        /// <summary>
        /// Determine the primary video controller ID
        /// </summary>
        /// <returns>Primary video controller ID</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "<Ausstehend>")]
        private static string VideoId() => String.Format(
            CultureInfo.InvariantCulture,
            "{0}{1}",
            Identifier(wmiClass: "Win32_VideoController", wmiProperty: "DriverVersion"),
            Identifier(wmiClass: "Win32_VideoController", wmiProperty: "Name"));

        /// <summary>
        /// Determine the first enabled network card ID
        /// </summary>
        /// <returns>First enabled network card ID</returns>
        private static string MacId() => Identifier(wmiClass: "Win32_NetworkAdapterConfiguration", wmiProperty: "MACAddress", wmiMustBeTrue: "IPEnabled");

        #endregion Original Device ID Getting Code
    }
}