using System;
using System.Data;
using System.Text;

namespace idee5.Common.Data {
    public static class DataTableExtensions {
        /// <summary>
        /// Export this <see cref="DataTable"/> to a CSV string.
        /// </summary>
        /// <param name="dt">This instance.</param>
        /// <param name="separator">The separator character.</param>
        /// <param name="quotationMark">The quotation character.</param>
        /// <param name="withHeader">Wether to add a header row with column names or not</param>
        /// <returns>T <see cref="DataTable"/> as CSV string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dt"/> or <paramref name="separator"/> is <c>null</c>.</exception>
        public static string ExportToCsv(this DataTable dt, string separator = ",", string quotationMark = "\"", bool withHeader = false) {
            if (dt == null)
                throw new ArgumentNullException(nameof(dt));

            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

            var sb = new StringBuilder();
            var quotationEscape = quotationMark + quotationMark;

            if (withHeader) {
                foreach (DataColumn col in dt.Columns)
                    sb.Append(quotationMark).Append(col.ColumnName).Append(quotationMark).Append(separator);
                sb.Remove(sb.Length - separator.Length, separator.Length);
                sb.AppendLine();
            }

            foreach (DataRow row in dt.Rows) {
                foreach (var col in row.ItemArray)
                    sb.Append(quotationMark).Append(col.ToString().Replace(quotationMark, quotationEscape)).Append(quotationMark).Append(separator);
                sb.Remove(sb.Length - separator.Length, separator.Length);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}