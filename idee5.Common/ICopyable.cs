namespace idee5.Common;
/// <summary>
/// Generic interface for deep copying reference types
/// </summary>
public interface ICopyable<T> {
    /// <summary>
    /// Creates a deep copy of this instance.
    /// </summary>
    /// <returns>Deep copy of <typeparamref name="T"/></returns>
    T Copy();
}