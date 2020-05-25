namespace idee5.Common {
    /// <summary>
    /// Ermöglicht das Binden von Zeichenketten an WPF-Elemente
    /// </summary>
    public class StringObject
    {
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="StringObject"/>-Klasse.
        /// </summary>
        public StringObject() : base() { }

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="StringObject"/>-Klasse.
        /// </summary>
        /// <param name="value">Die Zeichenkette</param>
        public StringObject(string value) { Value = value; }

        public string Value { get; set; }
    }
}