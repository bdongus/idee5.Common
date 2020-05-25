using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace idee5.Common.Data {
    public static class ExcelExportExtensions
    {
        /// <summary>
        /// Create excel XML from an <see cref="System.Collections.Generic.IEnumerable"/>.
        /// </summary>
        /// <param name="rows">The <see cref="System.Collections.Generic.IEnumerable"/> object to create the Excel document from.</param>
        /// <param name="sheetName">Name of the sheet. The name will NOT be validated!</param>
        /// <returns>Excel 2003 document as <see cref="XDocument"/>.</returns>
        public static XDocument ToExcelXml(this IEnumerable<object> rows, string sheetName = "data") {
            if (sheetName == null)
                throw new ArgumentNullException(nameof(sheetName));

            sheetName = sheetName.Replace("/", "-").Replace("\\", "-");

            XNamespace mainNamespace = "urn:schemas-microsoft-com:office:spreadsheet";
            XNamespace o = "urn:schemas-microsoft-com:office:office";
            XNamespace x = "urn:schemas-microsoft-com:office:excel";
            XNamespace ss = "urn:schemas-microsoft-com:office:spreadsheet";
            XNamespace html = "http://www.w3.org/TR/REC-html40";

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            //Generate header using reflection
            IEnumerable<XElement> headerRow = from p in rows.First().GetType().GetProperties()
                                              select new XElement(mainNamespace + "Cell",
                                                  new XElement(mainNamespace + "Data",
                                                      new XAttribute(ss + "Type", "String"), p.Name));
            var cultureToUse = CultureInfo.CurrentCulture;
            var workbook = new XElement(mainNamespace + "Workbook",
                new XAttribute(XNamespace.Xmlns + "html", html),
                new XAttribute(XName.Get("ss", "http://www.w3.org/2000/xmlns/"), ss),
                new XAttribute(XName.Get("o", "http://www.w3.org/2000/xmlns/"), o),
                new XAttribute(XName.Get("x", "http://www.w3.org/2000/xmlns/"), x),
                new XAttribute(XName.Get("xmlns", ""), mainNamespace),
                new XElement(o + "DocumentProperties",
                    new XAttribute(XName.Get("xmlns", ""), o),
                    new XElement(o + "Author", "idee5"),
                    new XElement(o + "LastAuthor", "idee5"),
                    new XElement(o + "Created", DateTime.Now.ToString(cultureToUse))), //end document properties
                new XElement(x + "ExcelWorkbook",
                    new XAttribute(XName.Get("xmlns", ""), x),
                    new XElement(x + "WindowHeight", "12750"),
                    new XElement(x + "WindowWidth", "24855"),
                    new XElement(x + "WindowTopX", "240"),
                    new XElement(x + "WindowTopY", "75"),
                    new XElement(x + "ProtectStructure", "False"),
                    new XElement(x + "ProtectWindows", "False")), //end ExcelWorkbook
                new XElement(mainNamespace + "Styles",
                    new XElement(mainNamespace + "Style",
                        new XAttribute(ss + "ID", "Default"),
                        new XAttribute(ss + "Name", "Normal"),
                        new XElement(mainNamespace + "Alignment",
                            new XAttribute(ss + "Vertical", "Bottom")),
                        new XElement(mainNamespace + "Borders"),
                        new XElement(mainNamespace + "Font",
                            new XAttribute(ss + "FontName", "Calibri"),
                            new XAttribute(x + "Family", "Swiss"),
                            new XAttribute(ss + "Size", "11"),
                            new XAttribute(ss + "Color", "#000000")),
                        new XElement(mainNamespace + "Interior"),
                        new XElement(mainNamespace + "NumberFormat"),
                        new XElement(mainNamespace + "Protection")),
                    new XElement(mainNamespace + "Style",
                        new XAttribute(ss + "ID", "Header"),
                        new XElement(mainNamespace + "Font",
                            new XAttribute(ss + "FontName", "Calibri"),
                            new XAttribute(x + "Family", "Swiss"),
                            new XAttribute(ss + "Size", "11"),
                            new XAttribute(ss + "Color", "#000000"),
                            new XAttribute(ss + "Bold", "1")))), // close styles
                new XElement(mainNamespace + "Worksheet",
                    new XAttribute(ss + "Name", sheetName /* Sheet name */),
                    new XElement(mainNamespace + "Table",
                        new XAttribute(ss + "ExpandedColumnCount", headerRow.Count().ToString(cultureToUse)),
                        new XAttribute(ss + "ExpandedRowCount", (rows.Count() + 1).ToString(cultureToUse)),
                        new XAttribute(x + "FullColumns", "1"),
                        new XAttribute(x + "FullRows", "1"),
                        new XAttribute(ss + "DefaultRowHeight", "15"),
                        new XElement(mainNamespace + "Column",
                            new XAttribute(ss + "Width", "81")),
                        new XElement(mainNamespace + "Row", new XAttribute(ss + "StyleID", "Header"), headerRow),
                        from contentRow in rows
                        select new XElement(mainNamespace + "Row",
                            new XAttribute(ss + "StyleID", "Default"),
                            from p in contentRow.GetType().GetProperties()
                            select new XElement(mainNamespace + "Cell",
                                new XElement(mainNamespace + "Data", new XAttribute(ss + "Type", "String"), p.GetValue(contentRow, null)))/* Build cells using reflection */)), //close table
                    new XElement(x + "WorksheetOptions",
                        new XAttribute(XName.Get("xmlns", ""), x),
                        new XElement(x + "PageSetup",
                            new XElement(x + "Header",
                                new XAttribute(x + "Margin", "0.3")),
                            new XElement(x + "Footer",
                                new XAttribute(x + "Margin", "0.3")),
                            new XElement(x + "PageMargins",
                                new XAttribute(x + "Bottom", "0.75"),
                                new XAttribute(x + "Left", "0.7"),
                                new XAttribute(x + "Right", "0.7"),
                                new XAttribute(x + "Top", "0.75"))),
                        new XElement(x + "Print",
                            new XElement(x + "ValidPrinterInfo"),
                            new XElement(x + "HorizontalResolution", "600"),
                            new XElement(x + "VerticalResolution", "600")),
                        new XElement(x + "Selected"),
                        new XElement(x + "Panes",
                            new XElement(x + "Pane",
                                new XElement(x + "Number", "3"),
                                new XElement(x + "ActiveRow", "1"),
                                new XElement(x + "ActiveCol", "0"))),
                        new XElement(x + "ProtectObjects", "False"),
                        new XElement(x + "ProtectScenarios", "False"))));

            xdoc.Add(workbook);

            return xdoc;
        }
    }
}