using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XLS2Config
{
    class NPOIExcelHelper
    {
        public static void DataTable2Excel(DataTable dt, string file, string sheetName)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                row.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
            }
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                IRow row2 = sheet.CreateRow(j + 1);
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    row2.CreateCell(k).SetCellValue(dt.Rows[j][k].ToString());
                }
            }

            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            byte[] buffer = stream.ToArray();

            using (FileStream stream2 = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                stream2.Write(buffer, 0, buffer.Length);
                stream2.Flush();
            }
        }

        public static DataTable Excel2DataTable(string file)
        {
            DataTable table = new DataTable();
            IWorkbook workbook = null;
            DataTable dataTable = null;
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    workbook = new XSSFWorkbook(stream);
                    if (workbook == null)
                    {
                        Debug.LogError("workbook === null");
                    }

                    dataTable = Export2DataTable(workbook.GetSheetAt(0), -1, true);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message +" stack="+ ex.StackTrace);
                }
                finally
                {
                    stream.Close();
                }
            }

            return dataTable;
        }

        private static DataTable Export2DataTable(ISheet sheet, int HeaderRowIndex, bool needHeader)
        {
            DataTable table = new DataTable();
            try
            {
                int lastCellNum;
                if ((HeaderRowIndex < 0) || !needHeader)
                {
                    XSSFRow row = sheet.GetRow(0) as XSSFRow;
                    if (row == null)
                    {
                        return null;
                    }

                    lastCellNum = row.LastCellNum;
                    for (int j = row.FirstCellNum; j <= lastCellNum; j++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(j));
                        table.Columns.Add(column);
                    }
                }
                else
                {
                    XSSFRow row = sheet.GetRow(HeaderRowIndex) as XSSFRow;
                    lastCellNum = row.LastCellNum;
                    for (int j = row.FirstCellNum; j <= lastCellNum; j++)
                    {
                        if (row.GetCell(j) == null)
                        {
                            break;
                        }
                        DataColumn column = new DataColumn(row.GetCell(j).StringCellValue);
                        table.Columns.Add(column);
                    }
                }
                int lastRowNum = sheet.LastRowNum;
                for (int i = HeaderRowIndex + 1; i <= sheet.LastRowNum; i++)
                {
                    XSSFRow row3 = null;
                    if (sheet.GetRow(i) == null)
                    {
                        row3 = sheet.CreateRow(i) as XSSFRow;
                    }
                    else
                    {
                        row3 = sheet.GetRow(i) as XSSFRow;
                    }
                    DataRow row = null;
                    if (row3.FirstCellNum >= 0)
                    {
                        row = table.NewRow();
                        for (int j = row3.FirstCellNum; j <= lastCellNum; j++)
                        {
                            string str2;
                            if (row3.GetCell(j) == null)
                            {
                                continue;
                            }
                            switch (row3.GetCell(j).CellType)
                            {
                                case CellType.Numeric:
                                    {
                                        if (!DateUtil.IsCellDateFormatted(row3.GetCell(j)))
                                        {
                                            break;
                                        }
                                        row[j] = DateTime.FromOADate(row3.GetCell(j).NumericCellValue);
                                        continue;
                                    }
                                case CellType.String:
                                    {
                                        string stringCellValue = row3.GetCell(j).StringCellValue;
                                        if (string.IsNullOrEmpty(stringCellValue))
                                        {
                                            goto Label_026E;
                                        }
                                        try
                                        {
                                            row[j] = stringCellValue;
                                        }
                                        catch (Exception)
                                        {
                                            row[j] = null;
                                        }
                                        continue;
                                    }
                                case CellType.Formula:
                                    switch (row3.GetCell(j).CachedFormulaResultType)
                                    {
                                        case CellType.Numeric:
                                            goto Label_02B4;

                                        case CellType.String:
                                            goto Label_02D6;

                                        case CellType.Boolean:
                                            goto Label_0328;

                                        case CellType.Error:
                                            goto Label_0347;
                                    }
                                    goto Label_036A;

                                case CellType.Boolean:
                                    {
                                        row[j] = Convert.ToString(row3.GetCell(j).BooleanCellValue);
                                        continue;
                                    }
                                case CellType.Error:
                                    {
                                        row[j] = ErrorEval.GetText(row3.GetCell(j).ErrorCellValue);
                                        continue;
                                    }
                                default:
                                    goto Label_03C0;
                            }
                            row[j] = Convert.ToDouble(row3.GetCell(j).NumericCellValue);
                            continue;
                        Label_026E:
                            row[j] = null;
                            continue;
                        Label_02B4:
                            row[j] = Convert.ToString(row3.GetCell(j).NumericCellValue);
                            continue;
                        Label_02D6:
                            str2 = row3.GetCell(j).StringCellValue;
                            if ((str2 != null) && (str2.Length > 0))
                            {
                                row[j] = str2.ToString();
                            }
                            else
                            {
                                row[j] = null;
                            }
                            continue;
                        Label_0328:
                            row[j] = Convert.ToString(row3.GetCell(j).BooleanCellValue);
                            continue;
                        Label_0347:
                            row[j] = ErrorEval.GetText(row3.GetCell(j).ErrorCellValue);
                            continue;
                        Label_036A:
                            row[j] = "";
                            continue;
                        Label_03C0:
                            row[j] = "";
                        }
                    }
                    if (row != null)
                    {
                        table.Rows.Add(row);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
            return table;
        }

        private static object GetValueType(XSSFCell cell)
        {
            if (cell == null)
            {
                return null;
            }
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue;

                case CellType.String:
                    return cell.StringCellValue;

                case CellType.Blank:
                    return null;

                case CellType.Boolean:
                    return cell.BooleanCellValue;

                case CellType.Error:
                    return cell.ErrorCellValue;
            }
            return ("=" + cell.StringCellValue);
        }
    }
}
