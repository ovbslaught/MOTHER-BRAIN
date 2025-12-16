using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using runnerDotNet;
namespace runnerDotNet
{
	public partial class ImportPage : RunnerPage
	{
		public dynamic audit = XVar.Pack(null);
		public dynamic action;
		public dynamic importType;
		public dynamic importText;
		public dynamic useXHR = XVar.Pack(false);
		public dynamic importData;
		public dynamic currentDateFormat;
		protected static bool skipImportPageCtor = false;
		public ImportPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipImportPageCtor)
			{
				skipImportPageCtor = false;
				return;
			}
			this.audit = XVar.Clone(CommonFunctions.GetAuditObject((XVar)(this.tName)));
		}
		protected override XVar buildJsTableSettings(dynamic _param_table, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic settings = XVar.Array();
			settings = XVar.Clone(base.buildJsTableSettings((XVar)(table), (XVar)(pSet)));
			settings.InitAndSetArrayItem(this.getImportfieldsLabels(), "importFieldsLabels");
			return settings;
		}
		protected virtual XVar getImportfieldsLabels()
		{
			dynamic importFieldsLabels = XVar.Array();
			importFieldsLabels = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> importField in this.pSet.getImportFields().GetEnumerator())
			{
				importFieldsLabels.InitAndSetArrayItem(CommonFunctions.GetFieldLabel((XVar)(MVCFunctions.GoodFieldName((XVar)(this.tName))), (XVar)(MVCFunctions.GoodFieldName((XVar)(importField.Value)))), importField.Value);
			}
			return importFieldsLabels;
		}
		public virtual XVar process()
		{
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(this.action)))))
			{
				this.removeOldTemporaryFiles();
			}
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessImport"))))
			{
				this.eventsObject.BeforeProcessImport(this);
			}
			if(this.action == "importPreview")
			{
				this.prepareAndSentPreviewData();
				return null;
			}
			if(this.action == "importData")
			{
				if(XVar.Pack(!(XVar)(CommonFunctions.isPostRequest())))
				{
					return false;
				}
				this.runImportAndSendResultReport();
				return null;
			}
			if(this.action == "downloadReport")
			{
				this.downloadReport();
				return null;
			}
			if(this.action == "downloadUnprocessed")
			{
				this.downloadUnprocessed();
				return null;
			}
			this.doCommonAssignments();
			this.addButtonHandlers();
			this.addCommonJs();
			this.displayImportPage();

			return null;
		}
		protected virtual XVar prepareAndSentPreviewData()
		{
			dynamic returnJSON = null, rnrTempFileName = null, rnrTempImportFilePath = null, var_response = XVar.Array();
			var_response = XVar.Clone(XVar.Array());
			rnrTempFileName = XVar.Clone(this.getImportTempFileName());
			rnrTempImportFilePath = XVar.Clone(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat("templates_c/", rnrTempFileName, ".csv"))));
			if(this.importType == "text")
			{
				MVCFunctions.runner_save_textfile((XVar)(rnrTempImportFilePath), (XVar)(this.importText));
				var_response.InitAndSetArrayItem(this.getPreviewDataFromText((XVar)(this.importText)), "previewData");
			}
			else
			{
				dynamic ext = null, importFileData = null, isExcel = null;
				ext = XVar.Clone(MVCFunctions.getImportFileExtension((XVar)(MVCFunctions.Concat("importFile", this.id))));
				isExcel = XVar.Clone((XVar)(MVCFunctions.strtoupper((XVar)(ext)) == "XLS")  || (XVar)(MVCFunctions.strtoupper((XVar)(ext)) == "XLSX"));
				if(XVar.Pack(isExcel))
				{
					rnrTempImportFilePath = XVar.Clone(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat("templates_c/", rnrTempFileName, ".", ext))));
				}
				importFileData = XVar.Clone(MVCFunctions.getImportFileData((XVar)(MVCFunctions.Concat("importFile", this.id))));
				MVCFunctions.upload_File((XVar)(importFileData), (XVar)(rnrTempImportFilePath));
				if(XVar.Pack(isExcel))
				{
					var_response.InitAndSetArrayItem(this.getPreviewDataFromExcel((XVar)(rnrTempImportFilePath)), "previewData");
				}
				else
				{
					dynamic importText = null;
					importText = XVar.Clone(MVCFunctions.CSVFileToText((XVar)(rnrTempImportFilePath), new XVar(true)));
					var_response.InitAndSetArrayItem(this.getPreviewDataFromText((XVar)(importText)), "previewData");
				}
			}
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_tempImportFilePath")] = rnrTempImportFilePath;
			returnJSON = XVar.Clone(CommonFunctions.printJSON((XVar)(var_response), (XVar)(this.useXHR)));
			if(returnJSON != false)
			{
				MVCFunctions.Echo(returnJSON);
			}
			else
			{
				MVCFunctions.Echo(CommonFunctions.mlang_message(new XVar("IMPORT_FILE_PARSING_FAILED")));
			}
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar runImportAndSendResultReport()
		{
			dynamic resultData = XVar.Array(), rnrTempImportFilePath = null;
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeImport"))))
			{
				dynamic message = null;
				message = new XVar("");
				if(XVar.Equals(XVar.Pack(this.eventsObject.BeforeImport(this, ref message)), XVar.Pack(false)))
				{
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(new XVar("failed", true, "message", message))));
					MVCFunctions.ob_flush();
					HttpContext.Current.Response.End();
					throw new RunnerInlineOutputException();
				}
			}
			rnrTempImportFilePath = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_tempImportFilePath")]);
			resultData = XVar.Clone(this.ImportFromFile((XVar)(rnrTempImportFilePath), (XVar)(this.importData)));
			MVCFunctions.runner_delete_file((XVar)(rnrTempImportFilePath));
			if(XVar.Pack(this.eventsObject.exists(new XVar("AfterImport"))))
			{
				this.eventsObject.AfterImport((XVar)(resultData["totalRecordsNumber"]), (XVar)(resultData["unprocessedRecordsNumber"]), this);
			}
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_tempImportLogFilePath")] = resultData["logFilePath"];
			if(XVar.Pack(resultData["unprocessedRecordsNumber"]))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_tempDataFilePath")] = resultData["unprocessedFilePath"];
			}
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(resultData)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar downloadReport()
		{
			dynamic logFilePath = null;
			logFilePath = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_tempImportLogFilePath")]);
			if(XVar.Pack(!(XVar)(MVCFunctions.myfile_exists((XVar)(logFilePath)))))
			{
				dynamic data = null;
				data = XVar.Clone(new XVar("success", false));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(data)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			MVCFunctions.Header("Content-Type", "text/plain");
			MVCFunctions.Header("Content-Disposition", "attachment;Filename=importLog.txt");
			MVCFunctions.Header("Cache-Control", "private");
			MVCFunctions.printfile((XVar)(logFilePath));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar downloadUnprocessed()
		{
			dynamic dataFilePath = null;
			dataFilePath = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_tempDataFilePath")]);
			if(XVar.Pack(!(XVar)(MVCFunctions.myfile_exists((XVar)(dataFilePath)))))
			{
				dynamic data = null;
				data = XVar.Clone(new XVar("success", false));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(data)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			MVCFunctions.Header("Content-Type", "application/csv");
			MVCFunctions.Header("Content-Disposition", "attachment;Filename=unpocessedData.csv");
			MVCFunctions.printfile((XVar)(dataFilePath));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public virtual XVar doCommonAssignments()
		{
			this.body.InitAndSetArrayItem(CommonFunctions.GetBaseScriptsForPage(new XVar(false)), "begin");
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
			this.xt.assignbyref(new XVar("body"), (XVar)(this.body));

			return null;
		}
		public override XVar clearSessionKeys()
		{
			base.clearSessionKeys();
			if((XVar)(!(XVar)(MVCFunctions.POSTSize()))  && (XVar)(!(XVar)(MVCFunctions.GETSize())))
			{
				XSession.Session.Remove(MVCFunctions.Concat(this.sessionPrefix, "_tempImportFilePath"));
				XSession.Session.Remove(MVCFunctions.Concat(this.sessionPrefix, "_tempImportLogFilePath"));
				XSession.Session.Remove(MVCFunctions.Concat(this.sessionPrefix, "_tempDataFilePath"));
			}

			return null;
		}
		protected virtual XVar getPreviewDataFromExcel(dynamic _param_filePath)
		{
			#region pass-by-value parameters
			dynamic filePath = XVar.Clone(_param_filePath);
			#endregion

			dynamic fieldsData = null, fileHandle = null, headerFieldsFromExcel = null, previewData = XVar.Array();
			fileHandle = XVar.Clone(ImportFunctions.openImportExcelFile((XVar)(filePath)));
			headerFieldsFromExcel = XVar.Clone(ImportFunctions.getImportExcelFields((XVar)(fileHandle)));
			fieldsData = XVar.Clone(this.getCorrespondingImportFieldsData((XVar)(headerFieldsFromExcel)));
			previewData = XVar.Clone(ImportFunctions.getPreviewDataFromExcel((XVar)(fileHandle), (XVar)(fieldsData)));
			previewData.InitAndSetArrayItem(fieldsData, "fieldsData");
			return previewData;
		}
		public virtual XVar getPreviewDataFromText(dynamic _param_importText)
		{
			#region pass-by-value parameters
			dynamic importText = XVar.Clone(_param_importText);
			#endregion

			dynamic dateFormat = null, delimiter = null, fieldsData = null, headerFieldsFromCSV = null, lines = XVar.Array(), previewData = XVar.Array();
			lines = XVar.Clone(this.removeEmptyLines((XVar)(ImportPage.CSVTextToLines((XVar)(importText)))));
			if(XVar.Pack(!(XVar)(lines)))
			{
				return XVar.Array();
			}
			delimiter = XVar.Clone(this.getCSVDelimiter((XVar)(MVCFunctions.array_slice((XVar)(lines), new XVar(0), new XVar(2)))));
			headerFieldsFromCSV = XVar.Clone(MVCFunctions.parseCSVLineNew((XVar)(lines[0]), (XVar)(delimiter)));
			fieldsData = XVar.Clone(this.getCorrespondingImportFieldsData((XVar)(headerFieldsFromCSV)));
			previewData = XVar.Clone(XVar.Array());
			previewData.InitAndSetArrayItem(true, "CSVPreview");
			previewData.InitAndSetArrayItem(delimiter, "delimiter");
			previewData.InitAndSetArrayItem(fieldsData, "fieldsData");
			previewData.InitAndSetArrayItem(MVCFunctions.array_slice((XVar)(lines), new XVar(0), new XVar(100)), "CSVlinesData");
			dateFormat = XVar.Clone(this.geDateFormat((XVar)(lines), (XVar)(delimiter), (XVar)(fieldsData)));
			previewData.InitAndSetArrayItem(this.getImportDateFormat((XVar)(dateFormat)), "dateFormat");
			return previewData;
		}
		protected virtual XVar geDateFormat(dynamic _param_lines, dynamic _param_delimiter, dynamic _param_fieldsData)
		{
			#region pass-by-value parameters
			dynamic lines = XVar.Clone(_param_lines);
			dynamic delimiter = XVar.Clone(_param_delimiter);
			dynamic fieldsData = XVar.Clone(_param_fieldsData);
			#endregion

			dynamic dateFormat = null;
			dateFormat = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> line in lines.GetEnumerator())
			{
				dynamic elems = XVar.Array();
				elems = XVar.Clone(MVCFunctions.parseCSVLineNew((XVar)(line.Value), (XVar)(delimiter)));
				foreach (KeyValuePair<XVar, dynamic> elem in elems.GetEnumerator())
				{
					if((XVar)(fieldsData.KeyExists(elem.Key))  && (XVar)(fieldsData[elem.Key]["dateTimeType"]))
					{
						dateFormat = XVar.Clone(ImportPage.extractDateFormat((XVar)(elem.Value)));
						if(XVar.Pack(MVCFunctions.strlen((XVar)(dateFormat))))
						{
							return dateFormat;
						}
					}
				}
			}
			return dateFormat;
		}
		protected virtual XVar removeEmptyLines(dynamic _param_lines)
		{
			#region pass-by-value parameters
			dynamic lines = XVar.Clone(_param_lines);
			#endregion

			dynamic resultLines = XVar.Array();
			resultLines = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> line in lines.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(MVCFunctions.trim((XVar)(line.Value))))))
				{
					resultLines.InitAndSetArrayItem(line.Value, null);
				}
			}
			return resultLines;
		}
		public static XVar extractDateFormat(dynamic _param_dateString)
		{
			#region pass-by-value parameters
			dynamic dateString = XVar.Clone(_param_dateString);
			#endregion

			dynamic dateComponents = XVar.Array(), dateSeparator = null, format = null, year = null;
			dateComponents = XVar.Clone(CommonFunctions.parsenumbers((XVar)(dateString)));
			if(MVCFunctions.count(dateComponents) < 3)
			{
				return "";
			}
			dateSeparator = XVar.Clone(GlobalVars.locale_info["LOCALE_SDATE"]);
			format = new XVar("");
			if((XVar)((XVar)(31 < dateComponents[0])  && (XVar)(ImportPage.testMonth((XVar)(dateComponents[1]))))  && (XVar)(12 <= dateComponents[2]))
			{
				year = XVar.Clone(dateComponents[0]);
				format = XVar.Clone(MVCFunctions.Concat("Y", dateSeparator, "M", dateSeparator, "D"));
			}
			if((XVar)((XVar)(12 <= dateComponents[0])  && (XVar)(ImportPage.testMonth((XVar)(dateComponents[1]))))  && (XVar)(31 < dateComponents[2]))
			{
				year = XVar.Clone(dateComponents[3]);
				format = XVar.Clone(MVCFunctions.Concat("D", dateSeparator, "M", dateSeparator, "Y"));
			}
			if((XVar)((XVar)(ImportPage.testMonth((XVar)(dateComponents[0])))  && (XVar)(12 <= dateComponents[1]))  && (XVar)(31 < dateComponents[2]))
			{
				year = XVar.Clone(dateComponents[3]);
				format = XVar.Clone(MVCFunctions.Concat("M", dateSeparator, "D", dateSeparator, "Y"));
			}
			if(XVar.Pack(format))
			{
				format = XVar.Clone(MVCFunctions.str_replace(new XVar("Y"), (XVar)((XVar.Pack(year < 100) ? XVar.Pack("YY") : XVar.Pack("YYYY"))), (XVar)(format)));
			}
			return format;
		}
		public static XVar testMonth(dynamic _param_number)
		{
			#region pass-by-value parameters
			dynamic number = XVar.Clone(_param_number);
			#endregion

			dynamic match = null, matched = null;
			match = XVar.Clone(XVar.Array());
			matched = XVar.Clone(MVCFunctions.preg_match(new XVar("/0[1-9]|1[0-2]/"), (XVar)(number), (XVar)(match)));
			if((XVar)((XVar)(matched)  && (XVar)(MVCFunctions.count(match)))  || (XVar)((XVar)(1 <= number)  && (XVar)(number <= 12)))
			{
				return true;
			}
			return false;
		}
		public static XVar getRefinedDateFormat(dynamic _param_dateFormat)
		{
			#region pass-by-value parameters
			dynamic dateFormat = XVar.Clone(_param_dateFormat);
			#endregion

			dynamic i = null, letter = null, refinedFormat = null;
			refinedFormat = new XVar("");
			dateFormat = XVar.Clone(MVCFunctions.strtolower((XVar)(dateFormat)));
			i = new XVar(0);
			for(;i < MVCFunctions.strlen((XVar)(dateFormat)); i++)
			{
				letter = XVar.Clone(dateFormat[i]);
				if((XVar)((XVar)((XVar)(letter == "d")  || (XVar)(letter == "m"))  || (XVar)(letter == "y"))  && (XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(refinedFormat), (XVar)(letter))), XVar.Pack(false))))
				{
					refinedFormat = MVCFunctions.Concat(refinedFormat, letter);
				}
			}
			return refinedFormat;
		}
		protected virtual XVar getCSVDelimiter(dynamic _param_firstTwoLines)
		{
			#region pass-by-value parameters
			dynamic firstTwoLines = XVar.Clone(_param_firstTwoLines);
			#endregion

			dynamic delimiter = null, delimiters = XVar.Array(), delimitersData = XVar.Array(), maxNumOfElems = null;
			delimiters = XVar.Clone(new XVar(0, ",", 1, ";", 2, "\t", 3, " "));
			delimitersData = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> delim in delimiters.GetEnumerator())
			{
				delimitersData.InitAndSetArrayItem(XVar.Array(), delim.Value);
				foreach (KeyValuePair<XVar, dynamic> line in firstTwoLines.GetEnumerator())
				{
					dynamic elemsNumber = null;
					elemsNumber = XVar.Clone(MVCFunctions.count(MVCFunctions.parseCSVLineNew((XVar)(line.Value), (XVar)(delim.Value))));
					if(elemsNumber <= 1)
					{
						break;
					}
					delimitersData.InitAndSetArrayItem(elemsNumber, delim.Value, line.Key);
				}
			}
			delimiter = new XVar(",");
			maxNumOfElems = new XVar(1);
			foreach (KeyValuePair<XVar, dynamic> data in delimitersData.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(data.Value)))
				{
					continue;
				}
				if((XVar)((XVar)(MVCFunctions.count(firstTwoLines) == 1)  || (XVar)((XVar)(MVCFunctions.count(firstTwoLines) == 2)  && (XVar)(data.Value[0] == data.Value[1])))  && (XVar)(maxNumOfElems < data.Value[0]))
				{
					maxNumOfElems = XVar.Clone(data.Value[0]);
					delimiter = XVar.Clone(data.Key);
				}
			}
			return delimiter;
		}
		public static XVar hasDateTimeFields(dynamic _param_fieldsData)
		{
			#region pass-by-value parameters
			dynamic fieldsData = XVar.Clone(_param_fieldsData);
			#endregion

			return true;
		}
		public virtual XVar getCorrespondingImportFieldsData(dynamic _param_headerFields)
		{
			#region pass-by-value parameters
			dynamic headerFields = XVar.Clone(_param_headerFields);
			#endregion

			dynamic importFields = XVar.Array(), tempFieldArray = XVar.Array(), tempGNamesArray = XVar.Array(), tempLabelArray = XVar.Array();
			importFields = XVar.Clone(this.pSet.getImportFields());
			tempFieldArray = XVar.Clone(XVar.Array());
			tempLabelArray = XVar.Clone(XVar.Array());
			tempGNamesArray = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> headerField in headerFields.GetEnumerator())
			{
				dynamic lowerHeaderField = null;
				lowerHeaderField = XVar.Clone(MVCFunctions.strtolower((XVar)(headerField.Value)));
				foreach (KeyValuePair<XVar, dynamic> importField in importFields.GetEnumerator())
				{
					dynamic dateTimeType = null, gName = null, label = null, trimHeaderField = null;
					dateTimeType = XVar.Clone(CommonFunctions.IsDateFieldType((XVar)(this.pSet.getFieldType((XVar)(importField.Value)))));
					if(lowerHeaderField == MVCFunctions.strtolower((XVar)(importField.Value)))
					{
						tempFieldArray.InitAndSetArrayItem(importField.Value, headerField.Key, "fName");
						tempFieldArray.InitAndSetArrayItem(dateTimeType, headerField.Key, "dateTimeType");
					}
					trimHeaderField = XVar.Clone(MVCFunctions.trim((XVar)(lowerHeaderField)));
					gName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(importField.Value)));
					if(trimHeaderField == MVCFunctions.strtolower((XVar)(MVCFunctions.trim((XVar)(gName)))))
					{
						tempGNamesArray.InitAndSetArrayItem(importField.Value, headerField.Key, "fName");
						tempGNamesArray.InitAndSetArrayItem(dateTimeType, headerField.Key, "dateTimeType");
					}
					label = XVar.Clone(CommonFunctions.GetFieldLabel((XVar)(MVCFunctions.GoodFieldName((XVar)(this.tName))), (XVar)(MVCFunctions.GoodFieldName((XVar)(importField.Value)))));
					if(trimHeaderField == MVCFunctions.strtolower((XVar)(MVCFunctions.trim((XVar)(label)))))
					{
						tempLabelArray.InitAndSetArrayItem(importField.Value, headerField.Key, "fName");
						tempLabelArray.InitAndSetArrayItem(dateTimeType, headerField.Key, "dateTimeType");
					}
				}
			}
			if((XVar)((XVar)(!(XVar)(tempFieldArray))  && (XVar)(!(XVar)(tempGNamesArray)))  && (XVar)(!(XVar)(tempLabelArray)))
			{
				return XVar.Array();
			}
			if((XVar)(MVCFunctions.count(tempLabelArray) <= MVCFunctions.count(tempFieldArray))  && (XVar)(MVCFunctions.count(tempGNamesArray) <= MVCFunctions.count(tempFieldArray)))
			{
				return tempFieldArray;
			}
			if((XVar)(MVCFunctions.count(tempFieldArray) <= MVCFunctions.count(tempLabelArray))  && (XVar)(MVCFunctions.count(tempGNamesArray) <= MVCFunctions.count(tempLabelArray)))
			{
				return tempLabelArray;
			}
			return tempGNamesArray;
		}
		public virtual XVar ImportFromFile(dynamic _param_filePath, dynamic importData)
		{
			#region pass-by-value parameters
			dynamic filePath = XVar.Clone(_param_filePath);
			#endregion

			dynamic dateFormat = null, fieldsData = null, headersLineOption = XVar.Array(), logFilePath = null, metaData = XVar.Array(), reportFileText = null, resultData = XVar.Array(), skipLinesOption = XVar.Array();
			fieldsData = XVar.Clone(this.refineImportFielsData((XVar)(importData["importFieldsData"])));
			dateFormat = XVar.Clone(ImportPage.getRefinedDateFormat((XVar)(this.getImportDateFormat((XVar)(importData["dateFormat"])))));
			this.currentDateFormat = XVar.Clone(dateFormat);
			headersLineOption = new XVar(null);
			skipLinesOption = new XVar(null);
			if(XVar.Pack(importData["useHeadersLineOption"]))
			{
				headersLineOption = XVar.Clone(XVar.Array());
				headersLineOption.InitAndSetArrayItem(importData["headersLineNumber"], "number");
			}
			if(XVar.Pack(importData["useSkipLinesOption"]))
			{
				skipLinesOption = XVar.Clone(XVar.Array());
				skipLinesOption.InitAndSetArrayItem(importData["skipLinesAmount"], "amount");
			}
			if(XVar.Pack(importData["CSV"]))
			{
				metaData = XVar.Clone(this.importFromCSV((XVar)(filePath), (XVar)(fieldsData), (XVar)(importData["delimiter"]), (XVar)(headersLineOption), (XVar)(skipLinesOption)));
			}
			else
			{
				metaData = XVar.Clone(this.importFromExcel((XVar)(filePath), (XVar)(fieldsData), (XVar)(headersLineOption), (XVar)(skipLinesOption)));
			}
			resultData = XVar.Clone(XVar.Array());
			resultData.InitAndSetArrayItem(this.getBasicReportText((XVar)(metaData["totalRecords"]), (XVar)(metaData["addedRecords"]), (XVar)(metaData["updatedRecords"])), "reportText");
			resultData.InitAndSetArrayItem(MVCFunctions.count(metaData["errorMessages"]), "unprocessedRecordsNumber");
			resultData.InitAndSetArrayItem(metaData["totalRecords"] - resultData["unprocessedRecordsNumber"], "totalRecordsNumber");
			reportFileText = XVar.Clone(this.getBasicReportText((XVar)(metaData["totalRecords"]), (XVar)(metaData["addedRecords"]), (XVar)(metaData["updatedRecords"]), new XVar(false), new XVar("\r\n"), (XVar)(metaData["errorMessages"]), (XVar)(metaData["unprocessedData"])));
			logFilePath = XVar.Clone(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat("templates_c/", this.getImportLogFileName(), ".txt"))));
			MVCFunctions.runner_save_file((XVar)(logFilePath), (XVar)(reportFileText));
			resultData.InitAndSetArrayItem(logFilePath, "logFilePath");
			if(XVar.Pack(MVCFunctions.count(metaData["unprocessedData"])))
			{
				dynamic unprocContent = null, unprocFilePath = null;
				unprocFilePath = XVar.Clone(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat("templates_c/", this.getUnprocessedDataFileName(), ".csv"))));
				unprocContent = XVar.Clone(this.getUnprocessedDataContent((XVar)(metaData["unprocessedData"])));
				MVCFunctions.runner_save_file((XVar)(unprocFilePath), (XVar)(unprocContent));
				resultData.InitAndSetArrayItem(unprocFilePath, "unprocessedFilePath");
			}
			return resultData;
		}
		protected virtual XVar getImportDateFormat(dynamic _param_dateFormat)
		{
			#region pass-by-value parameters
			dynamic dateFormat = XVar.Clone(_param_dateFormat);
			#endregion

			return (XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(dateFormat)))) ? XVar.Pack(GlobalVars.locale_info["LOCALE_SSHORTDATE"]) : XVar.Pack(dateFormat));
		}
		protected virtual XVar refineImportFielsData(dynamic _param_importFiledsData)
		{
			#region pass-by-value parameters
			dynamic importFiledsData = XVar.Clone(_param_importFiledsData);
			#endregion

			dynamic fieldsData = XVar.Array();
			fieldsData = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> fData in importFiledsData.GetEnumerator())
			{
				dynamic fName = null;
				fName = XVar.Clone(fData.Value["fName"]);
				if(XVar.Pack(fName))
				{
					fieldsData.InitAndSetArrayItem(new XVar("fName", fName, "type", this.pSet.getFieldType((XVar)(fName))), fData.Key);
				}
			}
			return fieldsData;
		}
		protected virtual XVar importFromCSV(dynamic _param_filePath, dynamic _param_fieldsData, dynamic _param_delimiter, dynamic _param_headersLineOption, dynamic _param_skipLinesOption)
		{
			#region pass-by-value parameters
			dynamic filePath = XVar.Clone(_param_filePath);
			dynamic fieldsData = XVar.Clone(_param_fieldsData);
			dynamic delimiter = XVar.Clone(_param_delimiter);
			dynamic headersLineOption = XVar.Clone(_param_headersLineOption);
			dynamic skipLinesOption = XVar.Clone(_param_skipLinesOption);
			#endregion

			dynamic addedRecords = null, autoinc = null, errorMessages = null, lines = XVar.Array(), metaData = XVar.Array(), text = null, totalRecords = null, unprocessedData = null, updatedRecords = null;
			text = XVar.Clone(MVCFunctions.CSVFileToText((XVar)(filePath), new XVar(false)));
			lines = XVar.Clone(ImportPage.CSVTextToLines((XVar)(text)));
			autoinc = XVar.Clone(this.hasAutoincImportFields((XVar)(fieldsData)));
			errorMessages = XVar.Clone(XVar.Array());
			unprocessedData = XVar.Clone(XVar.Array());
			addedRecords = new XVar(0);
			updatedRecords = new XVar(0);
			totalRecords = new XVar(0);
			if(headersLineOption != null)
			{
				dynamic idx = null;
				idx = XVar.Clone(headersLineOption["number"] - 1);
				lines.Remove(idx);
			}
			if(skipLinesOption != null)
			{
				dynamic i = null, linesCount = null;
				linesCount = XVar.Clone(skipLinesOption["amount"]);
				i = new XVar(0);
				for(;i < linesCount; i++)
				{
					lines.Remove(i);
				}
			}
			foreach (KeyValuePair<XVar, dynamic> line in lines.GetEnumerator())
			{
				dynamic elems = XVar.Array(), fieldsValuesData = XVar.Array();
				elems = XVar.Clone(MVCFunctions.parseCSVLineNew((XVar)(line.Value), (XVar)(delimiter)));
				fieldsValuesData = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> elem in elems.GetEnumerator())
				{
					dynamic fType = null, importFieldName = null;
					if(XVar.Pack(!(XVar)(fieldsData.KeyExists(elem.Key))))
					{
						continue;
					}
					importFieldName = XVar.Clone(fieldsData[elem.Key]["fName"]);
					fType = XVar.Clone(fieldsData[elem.Key]["type"]);
					fieldsValuesData.InitAndSetArrayItem(elem.Value, importFieldName);
				}
				this.importRecord((XVar)(fieldsValuesData), (XVar)(autoinc), ref addedRecords, ref updatedRecords, (XVar)(errorMessages), ref unprocessedData);
				totalRecords = XVar.Clone(totalRecords + 1);
			}
			metaData = XVar.Clone(XVar.Array());
			metaData.InitAndSetArrayItem(totalRecords, "totalRecords");
			metaData.InitAndSetArrayItem(addedRecords, "addedRecords");
			metaData.InitAndSetArrayItem(updatedRecords, "updatedRecords");
			metaData.InitAndSetArrayItem(errorMessages, "errorMessages");
			metaData.InitAndSetArrayItem(unprocessedData, "unprocessedData");
			return metaData;
		}
		protected virtual XVar importFromExcel(dynamic _param_filePath, dynamic _param_fieldsData, dynamic _param_headersLineOption, dynamic _param_skipLinesOption)
		{
			#region pass-by-value parameters
			dynamic filePath = XVar.Clone(_param_filePath);
			dynamic fieldsData = XVar.Clone(_param_fieldsData);
			dynamic headersLineOption = XVar.Clone(_param_headersLineOption);
			dynamic skipLinesOption = XVar.Clone(_param_skipLinesOption);
			#endregion

			dynamic autoinc = null, fileHandle = null;
			fileHandle = XVar.Clone(ImportFunctions.openImportExcelFile((XVar)(filePath)));
			autoinc = XVar.Clone(this.hasAutoincImportFields((XVar)(fieldsData)));
			return ImportFunctions.ImportDataFromExcel((XVar)(fileHandle), (XVar)(fieldsData), this, (XVar)(autoinc), (XVar)(headersLineOption), (XVar)(skipLinesOption));
		}
		protected virtual XVar hasAutoincImportFields(dynamic _param_fieldsData)
		{
			#region pass-by-value parameters
			dynamic fieldsData = XVar.Clone(_param_fieldsData);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> f in fieldsData.GetEnumerator())
			{
				if(XVar.Pack(this.pSet.isAutoincField((XVar)(f.Value["fName"]))))
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar prepareFiledsValuesData(dynamic _param_fieldsValuesData)
		{
			#region pass-by-value parameters
			dynamic fieldsValuesData = XVar.Clone(_param_fieldsValuesData);
			#endregion

			dynamic refinedFieldsValuesData = XVar.Array();
			refinedFieldsValuesData = XVar.Clone(XVar.Array());
			this.setUpdatedLatLng((XVar)(fieldsValuesData));
			foreach (KeyValuePair<XVar, dynamic> val in fieldsValuesData.GetEnumerator())
			{
				dynamic value = null, var_type = null;
				var_type = XVar.Clone(this.pSet.getFieldType((XVar)(val.Key)));
				if((XVar)(CommonFunctions.IsTimeType((XVar)(var_type)))  || (XVar)(this.pSet.getEditFormat((XVar)(val.Key)) == Constants.EDIT_FORMAT_TIME))
				{
					value = XVar.Clone(CommonFunctions.prepare_for_db((XVar)(val.Key), (XVar)(val.Value), new XVar("time"), new XVar(""), (XVar)(this.tName)));
					if(0 < MVCFunctions.strlen((XVar)(value)))
					{
						refinedFieldsValuesData.InitAndSetArrayItem(value, val.Key);
					}
					else
					{
						refinedFieldsValuesData.InitAndSetArrayItem(null, val.Key);
					}
					continue;
				}
				if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(var_type))))
				{
					if(XVar.Pack(!(XVar)(CommonFunctions.dateInDbFormat((XVar)(val.Value)))))
					{
						value = XVar.Clone(CommonFunctions.localdatetime2db((XVar)(val.Value), (XVar)(this.currentDateFormat)));
					}
					else
					{
						value = XVar.Clone(val.Value);
					}
					if(0 < MVCFunctions.strlen((XVar)(value)))
					{
						refinedFieldsValuesData.InitAndSetArrayItem(value, val.Key);
					}
					else
					{
						refinedFieldsValuesData.InitAndSetArrayItem(null, val.Key);
					}
					continue;
				}
				if((XVar)(!(XVar)(CommonFunctions.IsNumberType((XVar)(var_type))))  || (XVar)(MVCFunctions.IsNumeric(val.Value)))
				{
					refinedFieldsValuesData.InitAndSetArrayItem(val.Value, val.Key);
					continue;
				}
				value = XVar.Clone(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)(XVar.Pack(val.Value).ToString())));
				if(0 < MVCFunctions.strlen((XVar)(value)))
				{
					if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(value), (XVar)(GlobalVars.locale_info["LOCALE_SCURRENCY"]))), XVar.Pack(false)))
					{
						dynamic matches = XVar.Array();
						value = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, " ")), (XVar)(new XVar(0, "", 1, "")), (XVar)(value)));
						matches = XVar.Clone(XVar.Array());
						if(XVar.Pack(MVCFunctions.preg_match(new XVar("/^\\((.*)\\)$/"), (XVar)(value), (XVar)(matches))))
						{
							value = XVar.Clone((-1) * matches[1]);
						}
					}
					if(XVar.Pack(MVCFunctions.IsNumeric(value)))
					{
						refinedFieldsValuesData.InitAndSetArrayItem((double)value, val.Key);
					}
					else
					{
						refinedFieldsValuesData.InitAndSetArrayItem(0, val.Key);
					}
				}
				else
				{
					refinedFieldsValuesData.InitAndSetArrayItem(null, val.Key);
				}
			}
			return refinedFieldsValuesData;
		}
		protected virtual XVar callBeforeInsert(dynamic rawvalues, dynamic fieldsValuesData, dynamic errorMessage)
		{
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("BeforeInsert")))))
			{
				return true;
			}
			if(XVar.Equals(XVar.Pack(this.eventsObject.BeforeInsert((XVar)(rawvalues), (XVar)(fieldsValuesData), this, ref errorMessage)), XVar.Pack(false)))
			{
				return false;
			}
			return true;
		}
		public virtual XVar importRecord(dynamic _param_values, dynamic _param_identiyInsertOff, ref dynamic addedRecords, ref dynamic updatedRecords, dynamic errorMessages, ref dynamic unprocessedData)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			dynamic identiyInsertOff = XVar.Clone(_param_identiyInsertOff);
			#endregion

			dynamic errorMessage = null, failed = null, rawValues = null;
			rawValues = XVar.Clone(values);
			values = XVar.Clone(this.prepareFiledsValuesData((XVar)(values)));
			errorMessage = new XVar("");
			if(XVar.Pack(this.callBeforeInsert((XVar)(rawValues), (XVar)(values), (XVar)(errorMessage))))
			{
				failed = XVar.Clone(!(XVar)(this._importRecord((XVar)(values), (XVar)(identiyInsertOff), ref addedRecords, ref updatedRecords, ref errorMessage)));
			}
			else
			{
				failed = new XVar(true);
			}
			if(XVar.Pack(failed))
			{
				if(XVar.Pack(!(XVar)(unprocessedData)))
				{
					dynamic fieldNames = null;
					fieldNames = XVar.Clone(MVCFunctions.array_keys((XVar)(values)));
					unprocessedData.InitAndSetArrayItem(this.getImportFieldsLogCSVLine((XVar)(fieldNames)), null);
				}
				unprocessedData.InitAndSetArrayItem(this.parseValuesDataInLogCSVLine((XVar)(rawValues)), null);
				errorMessages.InitAndSetArrayItem(errorMessage, null);
			}

			return null;
		}
		protected virtual XVar _importRecord(dynamic _param_values, dynamic _param_identiyInsertOff, ref dynamic addedRecords, ref dynamic updatedRecords, ref dynamic errorMessage)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			dynamic identiyInsertOff = XVar.Clone(_param_identiyInsertOff);
			#endregion

			dynamic _keys = XVar.Array(), dc = null, insertResult = null, recordData = null, rs = null, updateResult = null, updateValues = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			dc.identiyInsertOff = XVar.Clone(identiyInsertOff);
			dc.values = values;
			insertResult = XVar.Clone(this.dataSource.insertSingle((XVar)(dc)));
			if(!XVar.Equals(XVar.Pack(insertResult), XVar.Pack(false)))
			{
				addedRecords = XVar.Clone(addedRecords + 1);
				if(XVar.Pack(this.audit))
				{
					this.audit.LogAdd((XVar)(this.tName), (XVar)(values), (XVar)(this.getRecordKeys((XVar)(insertResult))));
				}
				return true;
			}
			errorMessage = XVar.Clone(this.dataSource.lastError());
			_keys = XVar.Clone(this.getRecordKeys((XVar)(values)));
			if(XVar.Pack(!(XVar)(_keys)))
			{
				return false;
			}
			dc = XVar.Clone(new DsCommand());
			dc.keys = XVar.Clone(_keys);
			rs = XVar.Clone(this.dataSource.getSingle((XVar)(dc)));
			recordData = new XVar(null);
			if(XVar.Pack(rs))
			{
				dynamic fetchedArray = null;
				fetchedArray = XVar.Clone(rs.fetchAssoc());
				recordData = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
			}
			if(XVar.Pack(!(XVar)(recordData)))
			{
				return false;
			}
			dc = XVar.Clone(new DsCommand());
			dc.identiyInsertOff = XVar.Clone(identiyInsertOff);
			dc.keys = XVar.Clone(_keys);
			dc.filter = XVar.Clone(Security.SelectCondition(new XVar("E"), (XVar)(this.pSet)));
			updateValues = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(_keys.KeyExists(v.Key))))
				{
					updateValues.InitAndSetArrayItem(v.Value, v.Key);
				}
			}
			dc.values = XVar.Clone(updateValues);
			updateResult = XVar.Clone(this.dataSource.updateSingle((XVar)(dc)));
			if(XVar.Pack(updateResult))
			{
				updatedRecords = XVar.Clone(updatedRecords + 1);
				if(XVar.Pack(this.audit))
				{
					this.audit.LogEdit((XVar)(this.tName), (XVar)(values), (XVar)(recordData), (XVar)(_keys));
				}
				return true;
			}
			return false;
		}
		protected virtual XVar getRecordKeys(dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			#endregion

			dynamic keyFields = XVar.Array(), keys = XVar.Array();
			keys = XVar.Clone(XVar.Array());
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			foreach (KeyValuePair<XVar, dynamic> kf in keyFields.GetEnumerator())
			{
				if(XVar.Pack(values.KeyExists(kf.Value)))
				{
					keys.InitAndSetArrayItem(values[kf.Value], kf.Value);
				}
			}
			if(MVCFunctions.count(keys) != MVCFunctions.count(keyFields))
			{
				return XVar.Array();
			}
			return keys;
		}
		protected virtual XVar parseValuesDataInLogCSVLine(dynamic _param_fieldsValuesData)
		{
			#region pass-by-value parameters
			dynamic fieldsValuesData = XVar.Clone(_param_fieldsValuesData);
			#endregion

			dynamic values = XVar.Array();
			values = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in fieldsValuesData.GetEnumerator())
			{
				dynamic fType = null;
				fType = XVar.Clone(this.pSet.getFieldType((XVar)(value.Key)));
				if(XVar.Pack(!(XVar)(CommonFunctions.IsBinaryType((XVar)(fType)))))
				{
					values.InitAndSetArrayItem(MVCFunctions.Concat("\"", MVCFunctions.str_replace(new XVar("\""), new XVar("\"\""), (XVar)(value.Value)), "\""), null);
				}
			}
			return MVCFunctions.implode(new XVar(","), (XVar)(values));
		}
		protected virtual XVar getImportFieldsLogCSVLine(dynamic _param_importFields)
		{
			#region pass-by-value parameters
			dynamic importFields = XVar.Clone(_param_importFields);
			#endregion

			dynamic headerFields = XVar.Array();
			headerFields = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> fName in importFields.GetEnumerator())
			{
				dynamic fType = null;
				fType = XVar.Clone(this.pSet.getFieldType((XVar)(fName.Value)));
				if(XVar.Pack(!(XVar)(CommonFunctions.IsBinaryType((XVar)(fType)))))
				{
					headerFields.InitAndSetArrayItem(MVCFunctions.Concat("\"", MVCFunctions.str_replace(new XVar("\""), new XVar("\"\""), (XVar)(fName.Value)), "\""), null);
				}
			}
			return MVCFunctions.implode(new XVar(","), (XVar)(headerFields));
		}
		protected virtual XVar getUnprocessedDataContent(dynamic _param_unprocessedData)
		{
			#region pass-by-value parameters
			dynamic unprocessedData = XVar.Clone(_param_unprocessedData);
			#endregion

			dynamic content = null, headerLine = null;
			content = XVar.Clone(MVCFunctions.Concat(headerLine, MVCFunctions.implode(new XVar("\r\n"), (XVar)(unprocessedData))));
			return (XVar.Pack(GlobalVars.useUTF8) ? XVar.Pack(MVCFunctions.Concat("﻿", content)) : XVar.Pack(content));
		}
		protected virtual XVar getBasicReportText(dynamic _param_totalRecords, dynamic _param_addedRecords, dynamic _param_updatedRecords, dynamic _param_isNotLogFile = null, dynamic _param_lineBreak = null, dynamic _param_errorMessages = null, dynamic _param_unprocessedData = null)
		{
			#region default values
			if(_param_isNotLogFile as Object == null) _param_isNotLogFile = new XVar(true);
			if(_param_lineBreak as Object == null) _param_lineBreak = new XVar("<br>");
			if(_param_errorMessages as Object == null) _param_errorMessages = new XVar(XVar.Array());
			if(_param_unprocessedData as Object == null) _param_unprocessedData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic totalRecords = XVar.Clone(_param_totalRecords);
			dynamic addedRecords = XVar.Clone(_param_addedRecords);
			dynamic updatedRecords = XVar.Clone(_param_updatedRecords);
			dynamic isNotLogFile = XVar.Clone(_param_isNotLogFile);
			dynamic lineBreak = XVar.Clone(_param_lineBreak);
			dynamic errorMessages = XVar.Clone(_param_errorMessages);
			dynamic unprocessedData = XVar.Clone(_param_unprocessedData);
			#endregion

			dynamic boldBegin = null, boldEnd = null, importedReords = null, notImportedRecords = null, reportText = null;
			importedReords = XVar.Clone(addedRecords + updatedRecords);
			notImportedRecords = XVar.Clone(totalRecords - importedReords);
			boldBegin = new XVar("");
			boldEnd = new XVar("");
			reportText = new XVar("");
			if(XVar.Pack(isNotLogFile))
			{
				boldBegin = new XVar("<b>");
				boldEnd = new XVar("</b>");
			}
			else
			{
				reportText = MVCFunctions.Concat(reportText, CommonFunctions.mlang_message(new XVar("IMPORT_INTO")), " ", this.strOriginalTableName, lineBreak, CommonFunctions.str_format_datetime((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now())))), lineBreak, lineBreak);
			}
			reportText = MVCFunctions.Concat(reportText, MVCFunctions.mysprintf((XVar)(CommonFunctions.mlang_message(new XVar("IMPORT_RECORD_PROCESSED_SUCCESSFULLY"))), (XVar)(new XVar(0, MVCFunctions.Concat(boldBegin, importedReords, boldEnd), 1, MVCFunctions.Concat(boldBegin, totalRecords, boldEnd)))), lineBreak, MVCFunctions.mysprintf((XVar)(CommonFunctions.mlang_message(new XVar("IMPORT_RECORDS_ADDED"))), (XVar)(new XVar(0, MVCFunctions.Concat(boldBegin, addedRecords, boldEnd)))), lineBreak, MVCFunctions.mysprintf((XVar)(CommonFunctions.mlang_message(new XVar("IMPORT_RECORDS_UPDATED"))), (XVar)(new XVar(0, MVCFunctions.Concat(boldBegin, updatedRecords, boldEnd)))), lineBreak);
			if(XVar.Pack(notImportedRecords))
			{
				reportText = MVCFunctions.Concat(reportText, MVCFunctions.mysprintf((XVar)(CommonFunctions.mlang_message(new XVar("IMPORT_RECORDS_PROC_WITH_ERRORS"))), (XVar)(new XVar(0, MVCFunctions.Concat(boldBegin, notImportedRecords, boldEnd)))));
			}
			if((XVar)(notImportedRecords)  && (XVar)(MVCFunctions.count(errorMessages)))
			{
				dynamic i = null;
				reportText = MVCFunctions.Concat(reportText, ":");
				i = new XVar(0);
				for(;i < MVCFunctions.count(errorMessages); i++)
				{
					if(XVar.Pack(isNotLogFile))
					{
						reportText = MVCFunctions.Concat(reportText, lineBreak, errorMessages[i]);
					}
					else
					{
						reportText = MVCFunctions.Concat(reportText, lineBreak, lineBreak, errorMessages[i], lineBreak, unprocessedData[i + 1]);
					}
				}
			}
			return reportText;
		}
		public virtual XVar getImportTempFileName()
		{
			return MVCFunctions.Concat("import", this.getUniqueFileNameSuffix());
		}
		public virtual XVar getImportLogFileName()
		{
			return MVCFunctions.Concat("importLog", this.getUniqueFileNameSuffix());
		}
		public virtual XVar getUnprocessedDataFileName()
		{
			return MVCFunctions.Concat("importUnprocessed", this.getUniqueFileNameSuffix());
		}
		protected virtual XVar getUniqueFileNameSuffix()
		{
			dynamic dateMarker = null;
			dateMarker = XVar.Clone(MVCFunctions.getYMDdate((XVar)(MVCFunctions.time())));
			return MVCFunctions.Concat(dateMarker, "_", this.tName, "_", CommonFunctions.generatePassword(new XVar(5)));
		}
		public virtual XVar removeOldTemporaryFiles()
		{
			this.deleteTemporaryFilesFromDir(new XVar("templates_c/"));

			return null;
		}
		public virtual XVar deleteTemporaryFilesFromDir(dynamic _param_dir)
		{
			#region pass-by-value parameters
			dynamic dir = XVar.Clone(_param_dir);
			#endregion

			dynamic currentTime = null, fileNamesList = XVar.Array(), tempFilesDirectory = null, tempNamePattern = null;
			tempFilesDirectory = XVar.Clone(MVCFunctions.getabspath((XVar)(dir)));
			fileNamesList = XVar.Clone(MVCFunctions.getFileNamesFromDir((XVar)(tempFilesDirectory)));
			currentTime = XVar.Clone(MVCFunctions.strtotime((XVar)(MVCFunctions.now())));
			tempNamePattern = XVar.Clone(MVCFunctions.Concat("/^import.*([\\d]{4}-(0[1-9]|1[0-2])-([0-2][1-9]|3[0-1])).*_", this.tName, "_.{5}\\.\\w+/"));
			foreach (KeyValuePair<XVar, dynamic> fileName in fileNamesList.GetEnumerator())
			{
				dynamic matches = XVar.Array();
				matches = XVar.Clone(XVar.Array());
				if(XVar.Pack(MVCFunctions.preg_match((XVar)(tempNamePattern), (XVar)(fileName.Value), (XVar)(matches))))
				{
					dynamic timeFromFileName = null;
					timeFromFileName = XVar.Clone(MVCFunctions.strtotime((XVar)(matches[1])));
					if((XVar)(!XVar.Equals(XVar.Pack(timeFromFileName), XVar.Pack(false)))  && (XVar)(259200 < currentTime - timeFromFileName))
					{
						MVCFunctions.deleteImportTempFile((XVar)(MVCFunctions.Concat(tempFilesDirectory, fileName.Value)));
					}
				}
			}

			return null;
		}
		public static XVar CSVTextToLines(dynamic _param_text)
		{
			#region pass-by-value parameters
			dynamic text = XVar.Clone(_param_text);
			#endregion

			dynamic charNext = null, eol = null, i = null, inQuotes = null, j = null, lines = XVar.Array(), var_char = null;
			inQuotes = new XVar(false);
			j = new XVar(0);
			lines = XVar.Clone(XVar.Array());
			eol = new XVar("");
			i = new XVar(0);
			for(;i < MVCFunctions.strlen((XVar)(text)); i++)
			{
				var_char = XVar.Clone(MVCFunctions.substr((XVar)(text), (XVar)(i), new XVar(1)));
				charNext = XVar.Clone(MVCFunctions.substr((XVar)(text), (XVar)(i + 1), new XVar(1)));
				if(var_char == "\"")
				{
					if(XVar.Pack(!(XVar)(inQuotes)))
					{
						inQuotes = new XVar(true);
					}
					else
					{
						if(charNext == "\"")
						{
							i++;
							lines[j] = MVCFunctions.Concat(lines[j], var_char);
						}
						else
						{
							inQuotes = new XVar(false);
						}
					}
				}
				if((XVar)(!(XVar)(inQuotes))  && (XVar)(!(XVar)(eol)))
				{
					if(MVCFunctions.ord((XVar)(var_char)) == 10)
					{
						eol = XVar.Clone(var_char);
					}
					else
					{
						if((XVar)(MVCFunctions.ord((XVar)(var_char)) == 13)  && (XVar)(MVCFunctions.ord((XVar)(charNext)) == 10))
						{
							eol = XVar.Clone(MVCFunctions.Concat(var_char, charNext));
						}
					}
				}
				if((XVar)((XVar)(!(XVar)(inQuotes))  && (XVar)(eol))  && (XVar)(MVCFunctions.substr((XVar)(text), (XVar)(i), (XVar)(MVCFunctions.strlen((XVar)(eol)))) == eol))
				{
					j++;
					i += MVCFunctions.strlen((XVar)(eol)) - 1;
				}
				else
				{
					lines[j] = MVCFunctions.Concat(lines[j], var_char);
				}
			}
			return lines;
		}
		protected virtual XVar displayImportPage()
		{
			dynamic templatefile = null;
			templatefile = XVar.Clone(this.templatefile);
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowImport"))))
			{
				this.eventsObject.BeforeShowImport((XVar)(this.xt), ref templatefile, this);
			}
			this.display((XVar)(templatefile));

			return null;
		}
	}
}
