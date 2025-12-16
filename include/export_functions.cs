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
	public partial class CommonFunctions
	{
		public static XVar ExportToExcel(dynamic _param_rs, dynamic _param_pageSize, dynamic _param_pageObj)
		{
			#region pass-by-value parameters
			dynamic rs = XVar.Clone(_param_rs);
			dynamic pageSize = XVar.Clone(_param_pageSize);
			dynamic pageObj = XVar.Clone(_param_pageObj);
			#endregion

			dynamic arrColumnWidth = XVar.Array(), arrFields = XVar.Array(), arrLabel = XVar.Array(), arrTmpTotal = XVar.Array(), arrTotal = XVar.Array(), arrTotalMessage = XVar.Array(), eventRes = null, iNumberOfRows = null, objPHPExcel = null, row = XVar.Array(), totals = XVar.Array(), totalsFields = XVar.Array(), values = XVar.Array(), var_type = null;
			row = XVar.Clone(pageObj.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc())));
			totals = XVar.Clone(XVar.Array());
			arrLabel = XVar.Clone(XVar.Array());
			arrTotal = XVar.Clone(XVar.Array());
			arrFields = XVar.Clone(XVar.Array());
			arrColumnWidth = XVar.Clone(XVar.Array());
			arrTotalMessage = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in pageObj.selectedFields.GetEnumerator())
			{
				if(XVar.Pack(pageObj.pSet.appearOnExportPage((XVar)(field.Value))))
				{
					arrFields.InitAndSetArrayItem(field.Value, null);
				}
			}
			arrTmpTotal = XVar.Clone(pageObj.pSet.getTotalsFields());
			pageObj.viewControls.setForExportVar(new XVar("excel"));
			foreach (KeyValuePair<XVar, dynamic> field in arrFields.GetEnumerator())
			{
				arrLabel.InitAndSetArrayItem(CommonFunctions.GetFieldLabel((XVar)(MVCFunctions.GoodFieldName((XVar)(pageObj.tName))), (XVar)(MVCFunctions.GoodFieldName((XVar)(field.Value)))), field.Value);
				arrColumnWidth.InitAndSetArrayItem(10, field.Value);
				totals.InitAndSetArrayItem(new XVar("value", 0, "numRows", 0), field.Value);
				foreach (KeyValuePair<XVar, dynamic> tvalue in arrTmpTotal.GetEnumerator())
				{
					if(tvalue.Value["fName"] == field.Value)
					{
						totalsFields.InitAndSetArrayItem(new XVar("fName", field.Value, "totalsType", tvalue.Value["totalsType"], "viewFormat", pageObj.pSet.getViewFormat((XVar)(field.Value))), null);
					}
				}
			}
			iNumberOfRows = new XVar(0);
			objPHPExcel = XVar.Clone(ExportFunctions.ExportExcelInit((XVar)(arrLabel), (XVar)(arrColumnWidth)));
			while((XVar)((XVar)(!(XVar)(pageSize))  || (XVar)(iNumberOfRows < pageSize))  && (XVar)(row))
			{
				RunnerContext.pushRecordContext((XVar)(row), (XVar)(pageObj));
				CommonFunctions.countTotals((XVar)(totals), (XVar)(totalsFields), (XVar)(row));
				values = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> field in arrFields.GetEnumerator())
				{
					var_type = XVar.Clone(pageObj.pSet.getFieldType((XVar)(field.Value)));
					if((XVar)(CommonFunctions.IsBinaryType((XVar)(var_type)))  || (XVar)((XVar)(CommonFunctions.IsNumberType((XVar)(var_type)))  && (XVar)((XVar)(pageObj.pSet.getViewFormat((XVar)(field.Value)) == Constants.FORMAT_NUMBER)  || (XVar)(pageObj.pSet.getViewFormat((XVar)(field.Value)) == Constants.FORMAT_CURRENCY))))
					{
						values.InitAndSetArrayItem(row[field.Value], field.Value);
					}
					else
					{
						values.InitAndSetArrayItem(pageObj.getFormattedFieldValue((XVar)(field.Value), (XVar)(row)), field.Value);
					}
				}
				eventRes = new XVar(true);
				if(XVar.Pack(pageObj.eventsObject.exists(new XVar("BeforeOut"))))
				{
					eventRes = XVar.Clone(pageObj.eventsObject.BeforeOut((XVar)(row), (XVar)(values), (XVar)(pageObj)));
				}
				if(XVar.Pack(eventRes))
				{
					dynamic arrData = XVar.Array(), arrDataType = XVar.Array(), i = null;
					arrData = XVar.Clone(XVar.Array());
					arrDataType = XVar.Clone(XVar.Array());
					iNumberOfRows++;
					i = new XVar(0);
					foreach (KeyValuePair<XVar, dynamic> field in arrFields.GetEnumerator())
					{
						dynamic vFormat = null;
						arrData.InitAndSetArrayItem(values[field.Value], field.Value);
						vFormat = XVar.Clone(pageObj.pSet.getViewFormat((XVar)(field.Value)));
						var_type = XVar.Clone(pageObj.pSet.getFieldType((XVar)(field.Value)));
						if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
						{
							arrDataType.InitAndSetArrayItem("binary", field.Value);
						}
						else
						{
							if((XVar)((XVar)(vFormat == Constants.FORMAT_DATE_SHORT)  || (XVar)(vFormat == Constants.FORMAT_DATE_LONG))  || (XVar)(vFormat == Constants.FORMAT_DATE_TIME))
							{
								arrDataType.InitAndSetArrayItem("date", field.Value);
							}
							else
							{
								if(vFormat == Constants.FORMAT_FILE_IMAGE)
								{
									arrDataType.InitAndSetArrayItem("file", field.Value);
								}
								else
								{
									if((XVar)(Constants.FORMAT_NUMBER == vFormat)  && (XVar)(CommonFunctions.IsNumberType((XVar)(var_type))))
									{
										arrDataType.InitAndSetArrayItem("number", field.Value);
									}
									else
									{
										if((XVar)(Constants.FORMAT_CURRENCY == vFormat)  && (XVar)(CommonFunctions.IsNumberType((XVar)(var_type))))
										{
											arrDataType.InitAndSetArrayItem("currency", field.Value);
										}
										else
										{
											arrDataType.InitAndSetArrayItem("", field.Value);
										}
									}
								}
							}
						}
					}
					ExportFunctions.ExportExcelRecord((XVar)(arrData), (XVar)(arrDataType), (XVar)(iNumberOfRows), (XVar)(objPHPExcel), (XVar)(pageObj));
				}
				RunnerContext.pop();
				row = XVar.Clone(pageObj.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc())));
			}
			if(XVar.Pack(MVCFunctions.count(arrTmpTotal)))
			{
				foreach (KeyValuePair<XVar, dynamic> fName in arrFields.GetEnumerator())
				{
					dynamic total = null, totalMess = null, value = XVar.Array();
					value = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> tvalue in arrTmpTotal.GetEnumerator())
					{
						if(tvalue.Value["fName"] == fName.Value)
						{
							value = XVar.Clone(tvalue.Value);
						}
					}
					total = new XVar("");
					totalMess = new XVar("");
					if(XVar.Pack(value["totalsType"]))
					{
						if(value["totalsType"] == "COUNT")
						{
							totalMess = XVar.Clone(MVCFunctions.Concat("Count", ": "));
						}
						else
						{
							if(value["totalsType"] == "TOTAL")
							{
								totalMess = XVar.Clone(MVCFunctions.Concat("Total", ": "));
							}
							else
							{
								if(value["totalsType"] == "AVERAGE")
								{
									totalMess = XVar.Clone(MVCFunctions.Concat("Average", ": "));
								}
							}
						}
						total = XVar.Clone(CommonFunctions.GetTotals((XVar)(fName.Value), (XVar)(totals[fName.Value]["value"]), (XVar)(value["totalsType"]), (XVar)(totals[fName.Value]["numRows"]), (XVar)(value["viewFormat"]), new XVar("export"), (XVar)(pageObj.pSet), (XVar)(pageObj.useRawValues), (XVar)(pageObj)));
					}
					arrTotal.InitAndSetArrayItem(total, fName.Value);
					arrTotalMessage.InitAndSetArrayItem(totalMess, fName.Value);
				}
			}
			ExportFunctions.ExportExcelTotals((XVar)(arrTotal), (XVar)(arrTotalMessage), (XVar)(++(iNumberOfRows)), (XVar)(objPHPExcel));
			ExportFunctions.ExportExcelSave((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(pageObj.tName)), ".xlsx")), new XVar("Excel2007"), (XVar)(objPHPExcel));

			return null;
		}
		public static XVar excelNumberFormat(dynamic _param_nDigits)
		{
			#region pass-by-value parameters
			dynamic nDigits = XVar.Clone(_param_nDigits);
			#endregion

			dynamic nFormat = null, negative = null, negatives = null, positive = null;
			if(XVar.Equals(XVar.Pack(nDigits), XVar.Pack(false)))
			{
				nDigits = XVar.Clone(GlobalVars.locale_info["LOCALE_IDIGITS"]);
			}
			if(XVar.Pack(0) < nDigits)
			{
				nFormat = XVar.Clone(MVCFunctions.Concat("#,##0.", MVCFunctions.str_pad(new XVar(""), (XVar)(nDigits), new XVar("0"))));
			}
			else
			{
				nFormat = XVar.Clone(MVCFunctions.Concat("#,#", "#"));
			}
			positive = XVar.Clone(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_SPOSITIVESIGN"], nFormat));
			negatives = new XVar("");
			switch(((XVar)GlobalVars.locale_info["LOCALE_INEGNUMBER"]).ToInt())
			{
				case 0:
					negative = XVar.Clone(MVCFunctions.Concat("(", nFormat, ")"));
					break;
				case 1:
					negative = XVar.Clone(MVCFunctions.Concat("-", nFormat));
					break;
				case 2:
					negative = XVar.Clone(MVCFunctions.Concat("- ", nFormat));
					break;
				case 3:
					negative = XVar.Clone(MVCFunctions.Concat(nFormat, "-"));
					break;
				case 4:
					negative = XVar.Clone(MVCFunctions.Concat(nFormat, " -"));
					break;
			}
			return MVCFunctions.Concat(positive, ";[Red]", negative);
		}
		public static XVar excelCurrencyFormat()
		{
			dynamic curr = null, nFormat = null, negative = null, positive = null;
			if(0 < GlobalVars.locale_info["LOCALE_ICURRDIGITS"])
			{
				nFormat = XVar.Clone(MVCFunctions.Concat("#,#", "#0.", MVCFunctions.str_pad(new XVar(""), (XVar)(GlobalVars.locale_info["LOCALE_ICURRDIGITS"]), new XVar("0"))));
			}
			else
			{
				nFormat = XVar.Clone(MVCFunctions.Concat("# #", "#0"));
			}
			curr = XVar.Clone(MVCFunctions.Concat("\"", GlobalVars.locale_info["LOCALE_SCURRENCY"], "\""));
			positive = new XVar("");
			negative = new XVar("");
			switch(((XVar)GlobalVars.locale_info["LOCALE_ICURRENCY"]).ToInt())
			{
				case 0:
					positive = XVar.Clone(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_SCURRENCY"], nFormat));
					break;
				case 1:
					positive = XVar.Clone(MVCFunctions.Concat(nFormat, GlobalVars.locale_info["LOCALE_SCURRENCY"]));
					break;
				case 2:
					positive = XVar.Clone(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_SCURRENCY"], " ", nFormat));
					break;
				case 3:
					positive = XVar.Clone(MVCFunctions.Concat(nFormat, " ", GlobalVars.locale_info["LOCALE_SCURRENCY"]));
					break;
			}
			switch(((XVar)GlobalVars.locale_info["LOCALE_INEGCURR"]).ToInt())
			{
				case 0:
					negative = XVar.Clone(MVCFunctions.Concat("(", GlobalVars.locale_info["LOCALE_SCURRENCY"], nFormat, ")"));
					break;
				case 1:
					negative = XVar.Clone(MVCFunctions.Concat("-", GlobalVars.locale_info["LOCALE_SCURRENCY"], nFormat));
					break;
				case 2:
					negative = XVar.Clone(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_SCURRENCY"], "-", nFormat));
					break;
				case 3:
					negative = XVar.Clone(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_SCURRENCY"], nFormat, "-"));
					break;
				case 4:
					negative = XVar.Clone(MVCFunctions.Concat("(", nFormat, GlobalVars.locale_info["LOCALE_SCURRENCY"], ")"));
					break;
				case 5:
					negative = XVar.Clone(MVCFunctions.Concat("-", nFormat, GlobalVars.locale_info["LOCALE_SCURRENCY"]));
					break;
				case 6:
					negative = XVar.Clone(MVCFunctions.Concat(nFormat, "-", GlobalVars.locale_info["LOCALE_SCURRENCY"]));
					break;
				case 7:
					negative = XVar.Clone(MVCFunctions.Concat(nFormat, GlobalVars.locale_info["LOCALE_SCURRENCY"], "-"));
					break;
				case 8:
					negative = XVar.Clone(MVCFunctions.Concat("-", nFormat, " ", GlobalVars.locale_info["LOCALE_SCURRENCY"]));
					break;
				case 9:
					negative = XVar.Clone(MVCFunctions.Concat("-", GlobalVars.locale_info["LOCALE_SCURRENCY"], " ", nFormat));
					break;
				case 10:
					negative = XVar.Clone(MVCFunctions.Concat(nFormat, " ", GlobalVars.locale_info["LOCALE_SCURRENCY"], "-"));
					break;
				case 11:
					negative = XVar.Clone(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_SCURRENCY"], " ", nFormat, "-"));
					break;
				case 12:
					negative = XVar.Clone(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_SCURRENCY"], " -", nFormat));
					break;
				case 13:
					negative = XVar.Clone(MVCFunctions.Concat(nFormat, "- ", GlobalVars.locale_info["LOCALE_SCURRENCY"]));
					break;
				case 14:
					negative = XVar.Clone(MVCFunctions.Concat("(", GlobalVars.locale_info["LOCALE_SCURRENCY"], " ", nFormat, ")"));
					break;
				case 15:
					negative = XVar.Clone(MVCFunctions.Concat("(", nFormat, " ", GlobalVars.locale_info["LOCALE_SCURRENCY"], ")"));
					break;
			}
			return MVCFunctions.Concat(positive, ";[Red]", negative);
		}
	}
}
