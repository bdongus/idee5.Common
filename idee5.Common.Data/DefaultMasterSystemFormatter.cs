using System;

namespace idee5.Common.Data {
    /// <summary>
    /// Default implementation foramtting a <see cref="IMasterSystemReference"/> as trimmed '<see cref="IMasterSystemReference.MasterSystemHierarchy"/> <see cref="IMasterSystemReference.MasterSystemId"/>
    /// </summary>
    public class DefaultMasterSystemFormatter : IMasterSystemFormatter
    {
        /// <inheritdoc />
        public string FormatMasterSystemId(IMasterSystemReference masterSystemId) {
            if (masterSystemId == null) {
                throw new ArgumentNullException(nameof(masterSystemId));
            }

            return $"{masterSystemId.MasterSystemHierarchy.Trim()} {masterSystemId.MasterSystemId.Trim()}";
        }
    }
}