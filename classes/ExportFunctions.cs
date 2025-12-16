using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;

namespace runnerDotNet
{
    public class ExportFunctions
    {
        public static XVar ExportExcelInit(XVar arrData, XVar arrWidth)
        {
            ExcelPackage pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            int cellIndex = 1;
            foreach (var cell in arrData.GetEnumerator())
            {
                ws.Cells[1, cellIndex].Value = cell.Value;
                cellIndex++;
            }
            return new XVar("excelObj", pck, "rowIndex", 1);
        }

        public static void ExportExcelTotals(XVar arrTotal, XVar arrTotalMessage, XVar iNumberOfRows, XVar objPHPExcel)
        {
            ExcelWorksheet ws = ( (ExcelPackage)objPHPExcel["excelObj"].Value ).Workbook.Worksheets["Sheet1"];
			objPHPExcel["rowIndex"]++;
			
            int rowIndex = objPHPExcel["rowIndex"];
			int cellIndex = 1;
			foreach( var cell in arrTotal.GetEnumerator() ) {
				ws.Cells[ rowIndex, cellIndex ].Value = ( ( XVar)arrTotalMessage[ cell.Key ] ).Value + (( XVar)cell.Value).Value.ToString();
                cellIndex++;
            }			
        }

        public static void ExportExcelSave(XVar filename, XVar format, XVar objPHPExcel)
        {
            ((ExcelPackage)objPHPExcel["excelObj"].Value).SaveAs(HttpContext.Current.Response.OutputStream);
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=" + filename.ToString());
            HttpContext.Current.Response.End();
            throw new RunnerInlineOutputException();
        }

        public static void ExportExcelRecord(XVar arrdata, XVar datatype, XVar row, XVar objPHPExcel, dynamic pageObj)
        {
            XVar rowObj = new XVar("data", arrdata, "datatype", datatype);
            objPHPExcel.Add(rowObj);

            int cellIndex = 1;
            ExcelWorksheet ws = ((ExcelPackage)objPHPExcel["excelObj"].Value).Workbook.Worksheets["Sheet1"];
            objPHPExcel["rowIndex"]++;
            int rowIndex = objPHPExcel["rowIndex"];
            foreach (var cell in arrdata.GetEnumerator())
            {
                if (datatype[cell.Key] == "binary")
                {
                    if (cell.Value == null)
                    {
                        continue;
                    }
                    Image img = MVCFunctions.ImageFromBytes(cell.Value.Value as byte[]);
                    InsertImage(img, ws, cellIndex, rowIndex);
                }
                else if (datatype[cell.Key] == "file")
                {
                    if (!cell.Value || !File.Exists(pageObj.pSet.getUploadFolder(cell.Key) + cell.Value.ToString()))
                    {
                        continue;
                    }
                    Image img = MVCFunctions.ImageFromFile(pageObj.pSet.getUploadFolder(cell.Key) + cell.Value.ToString());
                    InsertImage(img, ws, cellIndex, rowIndex);
                }
                else
                {
                    // try to get correct cell type 

                    var obj = ((XVar)cell.Value).Value;
					var type = pageObj.pSet.getFieldType(cell.Key);

					if (CommonFunctions.IsDateFieldType(type))
					{
						ws.Cells[objPHPExcel["rowIndex"], cellIndex].Style.Numberformat.Format = GlobalVars.locale_info["LOCALE_SSHORTDATE"].ToString()
							+ (CommonFunctions.IsDateTimeFieldType(type) ? " hh:mm:ss" : "");
					}
					else if ( datatype[ cell.Key ] == "number" ) {
						ws.Cells[objPHPExcel["rowIndex"], cellIndex].Style.Numberformat.Format = CommonFunctions.excelNumberFormat( pageObj.pSet.isDecimalDigits(cell.Key) );
					} 
					else if ( datatype[ cell.Key ] == "currency" ) {
						ws.Cells[objPHPExcel["rowIndex"], cellIndex].Style.Numberformat.Format = CommonFunctions.excelCurrencyFormat();
					}
					else if (CommonFunctions.IsNumberType(type) && obj is string)
					{
						double objDouble = 0;
						int objInt = 0;
						if (CommonFunctions.IsFloatType(type) && double.TryParse(obj as string, out objDouble))
							obj = objDouble;
						else if (int.TryParse(obj as string, out objInt))
							obj = objInt;
					}

					ws.Cells[objPHPExcel["rowIndex"], cellIndex].Value = obj;
                }
                cellIndex++;
            }
        }

        private static void InsertImage(Image img, ExcelWorksheet ws, int cellIndex, int rowIndex)
        {
            if( img == null ) {
                return;
            }
            double width = img.Width * 0.143;
            double height = img.Height * 0.75;
            if (ws.Row(rowIndex).Height < height)
            {
                ws.Row(rowIndex).Height = height;
            }
            if (ws.Column(cellIndex).Width < width)
            {
                ws.Column(cellIndex).Width = width;
            }
            ws.Column(cellIndex).BestFit = false;
            var excelImg = ws.Drawings.AddPicture("Image" + rowIndex.ToString() + "_" + cellIndex.ToString(), img);
            excelImg.SetPosition(rowIndex - 1, 0, cellIndex - 1, 0);
            excelImg.SetSize(img.Width, img.Height);
        }
    }
}