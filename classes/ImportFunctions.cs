using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using LumenWorks.Framework.IO.Csv;
using OfficeOpenXml;

namespace runnerDotNet
{
    public class ImportFunctions
    {
        public static XVar openImportExcelFile( XVar uploadedFile )
        {
            if (uploadedFile == null)
            {
                return null;
            }

			try
			{
				ExcelPackage xlsPack = new ExcelPackage();

				// preview mode - file is in the _FILES list
				for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
				{
					if (HttpContext.Current.Request.Files[i].FileName == uploadedFile)
					{
						xlsPack.Load(HttpContext.Current.Request.Files[i].InputStream);
						return XVar.Pack(xlsPack);
					}
				}

				// import mode - file already saved on disc
				if (MVCFunctions.file_exists(uploadedFile))
				{
					using (FileStream rd = new FileStream(uploadedFile, FileMode.Open))
						xlsPack.Load(rd);
					return XVar.Pack(xlsPack);
				}

				return null;
			}
			catch(Exception ex)
			{
				MVCFunctions.import_error_handler(ex);
				return null;
			}
        }

        public static XVar getImportExcelFields(XVar data)
        {
			XVar fields = XVar.Array();
			
			if(data == null)
				return fields;
			
			try
			{
				ExcelPackage xlsPack = data.Value as ExcelPackage;
				var worksheet = xlsPack.Workbook.Worksheets[1];
				int cellIndex = 1;
				while (worksheet.Cells[1, cellIndex].Value != null)
				{
					fields.Add(XVar.Pack(worksheet.Cells[1, cellIndex].Value));
					cellIndex++;
				}
			}
			catch(Exception ex)
			{
				MVCFunctions.import_error_handler(ex);
			}
            return fields;
        }

		public static XVar ImportDataFromExcel( XVar fileHandle, XVar fieldsData, ImportPage importPageObject, XVar autoinc, XVar headersLineOption, XVar skipLinesOption)    
        {
			if (fileHandle == null)
				return null;

			XVar metaData = XVar.Array();
			metaData["totalRecords"] = 0;

			dynamic addedRecords = 0;
			dynamic updatedRecords = 0;
			dynamic errorMessages = XVar.Array();
			dynamic unprocessedData = XVar.Array();

			try
			{
				ExcelPackage xlsPack = fileHandle.Value as ExcelPackage;
				
				foreach(var record in IterateSheet(xlsPack, fieldsData, headersLineOption, skipLinesOption))
				{
					importPageObject.importRecord(record, autoinc, ref addedRecords, ref updatedRecords, errorMessages, ref unprocessedData);
					metaData["totalRecords"] = metaData["totalRecords"] + 1;
				}
			}
			catch(Exception ex)
			{
				MVCFunctions.import_error_handler(ex);
			}
			
			metaData["addedRecords"] = addedRecords;
			metaData["updatedRecords"] = updatedRecords;
			metaData["errorMessages"] = errorMessages;
			metaData["unprocessedData"] = unprocessedData;

			return metaData;
        }
		
		public static IEnumerable<XVar> IterateSheet(ExcelPackage xlsPack, XVar fieldsData, XVar headersLineOption, XVar skipLinesOption)
		{
			XVar previewData = null;
			
			int skipLinesAmount = (int) skipLinesOption["amount"];
			int startRow = skipLinesOption != null ? skipLinesAmount + 1 : 1;
				
			foreach(var worksheet in xlsPack.Workbook.Worksheets) 
			{
				for( int row = startRow; row <= worksheet.Dimension.End.Row; ++row)
				{
					XVar arr = XVar.Array();
					
					if (headersLineOption != null && headersLineOption["number"] == row) {
						continue;
					}
					
					for (int i = 0; i < worksheet.Dimension.End.Column; i++)
					{
						if( !fieldsData.KeyExists(i) )
							continue;
							
						var cell = worksheet.Cells[ row, i + 1 ];
						XVar val = null;

						var pic = worksheet.Drawings["Image" + row + "_" + i] as OfficeOpenXml.Drawing.ExcelPicture;
						if (pic != null)
						{
							val = new XVar(MVCFunctions.BytesFromImage(pic.Image));
						}
						else if (cell != null && cell.Value != null)
						{
							bool dateCorrectlyExtracted = false;
							if (cell.Value is DateTime) // sometimes date stored in a DateTime
							{
								val = ((DateTime)cell.Value).ToString("yyyy-MM-dd H:mm:ss");
								dateCorrectlyExtracted = true;
							}
							else if (Regex.IsMatch(cell.Style.Numberformat.Format, ("([ymdHis])"))) // sometimes date stored in a weird number
							{
								cell.Style.Numberformat.Format = "yyyy-MM-dd H:mm:ss";
								val = cell.Text;
								dateCorrectlyExtracted = true;
							}
							else if (cell.IsRichText)
							{
								val = cell.RichText.Text;
							}
							else
							{
								val = new XVar(cell.Value.ToString());
							}

							// the following code block looks suspicious (!)
							if (fieldsData.KeyExists(i) && fieldsData[i]["dateTimeType"] // column should have date value, 
								&& !dateCorrectlyExtracted								// but in excel it looks awfull
								&& (row != 0))				// also we still dont know actual format and it is not header row
							{
								previewData = XVar.Array();
								// so try to guess format from the value
								previewData["dateFormat"] = ImportPage.extractDateFormat(val);
							}
						}

						if( previewData != null )
						{
							arr.Add(MVCFunctions.runner_htmlspecialchars(val));
						}
						else 
						{
							arr[ fieldsData[i]["fName"] ] = val;
						}
					}

					yield return arr;
				}
			}
		}

		public static IEnumerable<XVar> IterateSheetPreview( ExcelPackage xlsPack )
		{
			foreach(var worksheet in xlsPack.Workbook.Worksheets)
			{
				for( int row = 1; row <= worksheet.Dimension.End.Row; ++row)
				{
					XVar arr = XVar.Array();
					for (int i = 0; i < worksheet.Dimension.End.Column; i++)
					{
						var cell = worksheet.Cells[ row, i + 1 ];
						XVar val = null;
						if (cell != null && cell.Value != null)
						{					
							if (cell.Value is DateTime) // sometimes date stored in a DateTime
							{
								val = ((DateTime)cell.Value).ToString("yyyy-MM-dd H:mm:ss");	
							}
							else if (Regex.IsMatch(cell.Style.Numberformat.Format, ("([ymdHis])"))) // sometimes date stored in a weird number
							{
								cell.Style.Numberformat.Format = "yyyy-MM-dd H:mm:ss";
								val = cell.Text;
							}
							else if (cell.IsRichText)
							{
								val = cell.RichText.Text;
							}
							else
							{
								val = new XVar(cell.Value.ToString());
							}
						}

						arr.Add(MVCFunctions.runner_htmlspecialchars(val));
					}

					yield return arr;
				}
			}
		}
		
		public static XVar getPreviewDataFromExcel(XVar fileHandle, XVar fieldsData) 
		{
			XVar previewData = XVar.Array();
			XVar tableData = XVar.Array();
			var remainNumOfPreviewRows = 100;
			
			try
			{
				ExcelPackage xlsPack = fileHandle.Value as ExcelPackage;
				
				foreach(var record in IterateSheetPreview(xlsPack))
				{
				
					if (remainNumOfPreviewRows<0)
						break;
					
					XVar row = XVar.Array();
					foreach (KeyValuePair<XVar, dynamic> field in record.GetEnumerator())
					{
						row.Add(field.Value);
					}
					tableData.Add(row);
										
					remainNumOfPreviewRows--;
				}
			}
			catch(Exception ex)
			{
				MVCFunctions.import_error_handler(ex);
			}

			previewData["tableData"]=tableData;
			return previewData;	
		}		
		

    }
}