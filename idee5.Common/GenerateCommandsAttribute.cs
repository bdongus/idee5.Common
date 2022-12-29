using System;

namespace idee5.Common {
    /// <summary>
    /// Marker attribute for the source generator to create commands and handler for every public method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class GenerateCommandsAttribute : Attribute {
        /// <summary>
        /// NativeName of the "Analyzer additional file" with the template for commands.
        /// </summary>
        public string CommandTemplate { get; set; }
        /// <summary>
        /// NativeName of the "Analyzer additional file" with the template for command properties.
        /// </summary>
        public string PropertyTemplate { get; set; }
        /// <summary>
        /// NativeName of the "Analyzer additional file" with the source template for command handlers. E.g. "CommandHandlerTemplate.txt"
        /// </summary>
        public string HandlerTemplate { get; }

        /// <summary>
        /// Create a marker attribute for the command source generator.
        /// </summary>
        /// <param name="handlerTemplate">NativeName of the "Analyzer additional file" with the source template for command handlers. E.g. "CommandHandlerTemplate.txt"</param>
        public GenerateCommandsAttribute(string handlerTemplate) {
            HandlerTemplate = handlerTemplate;
        }
    }
}
