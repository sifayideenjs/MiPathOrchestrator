using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiPathOrchestrator.Utilities
{
    public static class ExcelUtility
    {
        public static DataTable ReadAsDataTable(string fileName)
        {
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                foreach (Cell cell in rows.ElementAt(0))
                {
                    dataTable.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                }

                foreach (Row row in rows)
                {
                    DataRow dataRow = dataTable.NewRow();

                    //for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                    //{
                    //    dataRow[i] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                    //}

                    //Handled to consider empty cell items too
                    for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                    {
                        Cell cell = row.Descendants<Cell>().ElementAt(i);
                        int actualCellIndex = CellReferenceToIndex(cell);
                        dataRow[actualCellIndex] = GetCellValue(spreadSheetDocument, cell);
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            dataTable.Rows.RemoveAt(0);

            return dataTable;
        }

        public static List<T> ConvertToDataTable<T>(DataTable dataTable)
        {
            List<T> dataList = new List<T>();
            if(dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    T item = GetItem<T>(row);
                    dataList.Add(item);
                }
            }
            return dataList;
        }

        public static List<T> ConvertToList<T>(string fileName)
        {
            var dataTable = ReadAsDataTable(fileName);
            var dataList = ConvertToDataTable<T>(dataTable);
            return dataList;
        }

        private static T GetItem<T>(DataRow dataRow)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                var xx = dataRow[column.ColumnName];
                if (dataRow[column.ColumnName] == DBNull.Value) continue;
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dataRow[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        private static int CellReferenceToIndex(Cell cell)
        {
            int index = 0;
            string reference = cell.CellReference.ToString().ToUpper();
            foreach (char ch in reference)
            {
                if (Char.IsLetter(ch))
                {
                    int value = (int)ch - (int)'A';
                    index = (index == 0) ? value : ((index + 1) * 26) + value;
                }
                else
                    return index;
            }
            return index;
        }

        private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        public static string DataTableToHtml(DataTable dataTable)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<style type='text/css'>.TFtable{width:100%;border-collapse:collapse;}.TFtable td{padding:7px;border:#4e95f4 1px solid;}.TFtable tr{background: #b8d1f3;}.TFtable tr:nth - child(odd){background:#b8d1f3;}.TFtable tr:nth - child(even){background: #dae5f4;}</style>");
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' class='TFtable' style='font-family:Arial; font-size:small;'>");
            strHTMLBuilder.Append("<tr valign='top'>");

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                strHTMLBuilder.Append("<td><b>");
                strHTMLBuilder.Append(dataColumn.ColumnName);
                strHTMLBuilder.Append("</b></td>");
            }

            strHTMLBuilder.Append("</tr>");

            int currRow = 1;
            foreach (DataRow dataRow in dataTable.Rows)
            {

                strHTMLBuilder.Append("<tr style='" + (currRow % 2 == 0 ? "background-color:white" : "background-color:lightgreen") + "' valign='top'>");
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    strHTMLBuilder.Append("<td>");
                    strHTMLBuilder.Append(dataRow[dataColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");
                currRow++;
            }

            strHTMLBuilder.Append("</table>");
            string Htmltext = strHTMLBuilder.ToString();

            return Htmltext;
        }

        public static List<Dictionary<string, string>> DataTableToDictionaryList(DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(
                row => dataTable.Columns.Cast<DataColumn>().ToDictionary(
                    column => column.ColumnName,
                    column => row[column].ToString()
                )).ToList();
        }
    }
}
