using System;
using System.Globalization;
using System.IO;

namespace idee5.Common;
/// <summary>
/// Used to create a hash code from multiple objects.
/// </summary>
public class HashCodeCombiner {
    private long _combinedHash = 5381L;

    /// <summary>
    /// Add an integer value to the hash code.
    /// </summary>
    /// <param name="i">Value to add.</param>
    public void AddInt(int i) {
        _combinedHash = ((_combinedHash << 5) + _combinedHash) ^ i;
    }

    /// <summary>
    /// Add an <see cref="object"/> to the hash code.
    /// </summary>
    /// <param name="o">Object to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="o"/> is <c>null</c>.</exception>
    public void AddObject(object o) {
#if NETSTANDARD2_0_OR_GREATER
        if (o == null) throw new ArgumentNullException(nameof(o));
#else
        ArgumentNullException.ThrowIfNull(o);
#endif

        AddInt(o.GetHashCode());
    }

    /// <summary>
    /// Add a <see cref="DateTime"/> to the hash code.
    /// </summary>
    /// <param name="d"><see cref="DateTime"/> to add.</param>
    public void AddDateTime(DateTime d) => AddInt(d.GetHashCode());

    /// <summary>
    /// Add a case insensitive <see cref="string"/> to the hash.
    /// </summary>
    /// <param name="s"><see cref="string"/> to add.</param>
    public void AddCaseInsensitiveString(string s) {
        if (s != null)
            AddInt(StringComparer.CurrentCultureIgnoreCase.GetHashCode(s));
    }

    /// <summary>
    /// Add a file system item to the hash code.
    /// </summary>
    /// <param name="f">File system Item to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="f"/> is <c>null</c>.</exception>
    public void AddFileSystemItem(FileSystemInfo f) {
#if NETSTANDARD2_0_OR_GREATER
        if (f == null) throw new ArgumentNullException(nameof(f));
#else
        ArgumentNullException.ThrowIfNull(f);
#endif
        //if it doesn't exist, don't proceed.
        if (!f.Exists) return;
        AddCaseInsensitiveString(f.FullName);
        AddDateTime(f.CreationTimeUtc);
        AddDateTime(f.LastWriteTimeUtc);
        //check if it is a file or folder
        if (f is FileInfo fileInfo) { AddInt(fileInfo.Length.GetHashCode()); }
        if (f is DirectoryInfo dirInfo) {
            foreach (FileInfo d in dirInfo.GetFiles()) { AddFile(d); }
            foreach (DirectoryInfo s in dirInfo.GetDirectories()) { AddFolder(s); }
        }
    }

    /// <summary>
    /// Add a file to the hash code.
    /// </summary>
    /// <param name="f"></param>
    public void AddFile(FileInfo f) { AddFileSystemItem(f); }

    /// <summary>
    /// Add a folder/directory to the hash code.
    /// </summary>
    /// <param name="d"></param>
    public void AddFolder(DirectoryInfo d) { AddFileSystemItem(d); }

    /// <summary>
    /// Returns the hex code of the combined hash code
    /// </summary>
    /// <returns>The hash code as string.</returns>
    public string GetCombinedHashCode() { return _combinedHash.ToString(format: "x", provider: CultureInfo.InvariantCulture); }
}