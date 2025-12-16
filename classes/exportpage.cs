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
	public partial class ExportPage : RunnerPage
	{
		public dynamic exportType = XVar.Pack("");
		public dynamic allPagesMode = XVar.Pack(false);
		public dynamic currentPageMode = XVar.Pack(false);
		protected dynamic textFormattingType;
		public dynamic useRawValues = XVar.Pack(false);
		public dynamic csvDelimiter = XVar.Pack(",");
		public dynamic selectedFields = XVar.Array();
		protected static bool skipExportPageCtor = false;
		public ExportPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipExportPageCtor)
			{
				skipExportPageCtor = false;
				return;
			}
			this.headerForms = XVar.Clone(new XVar(0, "top"));
			this.footerForms = XVar.Clone(new XVar(0, "footer"));
			this.bodyForms = XVar.Clone(new XVar(0, "grid"));
			this.textFormattingType = XVar.Clone(this.pSet.getExportTxtFormattingType());
			this.useRawValues = XVar.Clone((XVar)(this.useRawValues)  || (XVar)(this.textFormattingType == Constants.EXPORT_RAW));
			if((XVar)((XVar)(this.exportType)  && (XVar)(this.useRawValues))  && (XVar)(this.textFormattingType == Constants.EXPORT_FORMATTED))
			{
				this.useRawValues = new XVar(false);
			}
			if(XVar.Pack(!(XVar)(this.selectedFields)))
			{
				this.selectedFields = XVar.Clone(this.pSet.getExportFields());
			}
			if(XVar.Pack(!(XVar)(this.searchClauseObj)))
			{
				this.searchClauseObj = XVar.Clone(this.getSearchObject());
			}
		}
		public virtual XVar process()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessExport"))))
			{
				this.eventsObject.BeforeProcessExport(this);
			}
			if(XVar.Pack(this.exportType))
			{
				RunnerContext.pushSearchContext((XVar)(this.searchClauseObj));
				this.exportByType();
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
				return null;
			}
			this.fillSettings();
			this.doCommonAssignments();
			this.addButtonHandlers();
			this.addCommonJs();
			this.displayExportPage();

			return null;
		}
		public override XVar addCommonJs()
		{
			base.addCommonJs();
			if(XVar.Pack(this.pSet.checkExportFieldsSelection()))
			{
				this.AddCSSFile(new XVar("include/chosen/bootstrap-chosen.css"));
				this.AddJSFile(new XVar("include/chosen/chosen.jquery.js"));
			}

			return null;
		}
		protected virtual XVar doCommonAssignments()
		{
			if(this.mode == Constants.EXPORT_SIMPLE)
			{
				this.body.InitAndSetArrayItem(CommonFunctions.GetBaseScriptsForPage(new XVar(false)), "begin");
				this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
				this.xt.assignbyref(new XVar("body"), (XVar)(this.body));
			}
			else
			{
				this.xt.assign(new XVar("cancel_button"), new XVar(true));
			}
			this.xt.assign(new XVar("groupExcel"), new XVar(true));
			this.xt.assign(new XVar("exportlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"saveButton", this.id, "\"")));
			if(XVar.Pack(this.pSet.checkExportFieldsSelection()))
			{
				this.xt.assign(new XVar("choosefields"), new XVar(true));
				this.xt.assign(new XVar("exportFieldsCtrl"), (XVar)(this.getChooseFieldsCtrlMarkup()));
			}
			if(XVar.Pack(!(XVar)(this.selection)))
			{
				this.xt.assign(new XVar("rangeheader_block"), new XVar(true));
				this.xt.assign(new XVar("range_block"), new XVar(true));
			}
			if(this.textFormattingType == Constants.EXPORT_BOTH)
			{
				this.xt.assign(new XVar("exportformat"), new XVar(true));
			}

			return null;
		}
		protected virtual XVar getChooseFieldsCtrlMarkup()
		{
			dynamic options = XVar.Array();
			options = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in this.pSet.getExportFields().GetEnumerator())
			{
				options.InitAndSetArrayItem(MVCFunctions.Concat("<option value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(field.Value)), "\" selected=\"selected\">", MVCFunctions.runner_htmlspecialchars((XVar)(this.pSet.label((XVar)(field.Value)))), "</option>"), null);
			}
			return MVCFunctions.Concat("<select name=\"exportFields\" multiple style=\"width: 100%;\" data-placeholder=\"", CommonFunctions.mlang_message(new XVar("PLEASE_SELECT")), "\" id=\"exportFields", this.id, "\">", MVCFunctions.implode(new XVar(""), (XVar)(options)), "</select>");
		}
		protected virtual XVar exportByType()
		{
			dynamic dc = null, myPage = null, pageSize = null, rs = null;
			myPage = new XVar(1);
			pageSize = new XVar(0);
			if(XVar.Pack(this.currentPageMode))
			{
				myPage = XVar.Clone((int)XSession.Session[MVCFunctions.Concat(this.tName, "_pagenumber")]);
				if(XVar.Pack(!(XVar)(myPage)))
				{
					myPage = new XVar(1);
				}
				pageSize = XVar.Clone((int)XSession.Session[MVCFunctions.Concat(this.tName, "_pagesize")]);
				if(XVar.Pack(!(XVar)(pageSize)))
				{
					pageSize = XVar.Clone(this.pSet.getInitialPageSize());
				}
				if(XVar.Pack(this.pSet.getRecordsLimit()))
				{
					pageSize = XVar.Clone(this.pSet.getRecordsLimit() - (myPage - 1) * pageSize);
				}
				if(pageSize < XVar.Pack(0))
				{
					pageSize = new XVar(0);
				}
			}
			else
			{
				if((XVar)(this.allPagesMode)  && (XVar)(this.pSet.getRecordsLimit()))
				{
					pageSize = XVar.Clone(this.pSet.getRecordsLimit());
				}
			}
			dc = XVar.Clone(this.getSubsetDataCommand());
			if((XVar)(this.currentPageMode)  || (XVar)((XVar)(this.allPagesMode)  && (XVar)(this.pSet.getRecordsLimit())))
			{
				dc.startRecord = XVar.Clone(pageSize * (myPage - 1));
				dc.reccount = XVar.Clone(pageSize);
			}
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeQueryExport"))))
			{
				dynamic order = null, prep = XVar.Array(), sql = null, where = null;
				prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
				sql = XVar.Clone(prep["sql"]);
				where = XVar.Clone(prep["where"]);
				order = XVar.Clone(prep["order"]);
				this.eventsObject.BeforeQueryExport((XVar)(sql), ref where, ref order, this);
				if(sql != prep["sql"])
				{
					this.dataSource.overrideSQL((XVar)(dc), (XVar)(sql));
				}
				else
				{
					if(where != prep["where"])
					{
						this.dataSource.overrideWhere((XVar)(dc), (XVar)(where));
					}
				}
				if(order != prep["order"])
				{
					this.dataSource.overrideOrder((XVar)(dc), (XVar)(order));
				}
			}
			rs = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				MVCFunctions.showError((XVar)(this.dataSource.lastError()));
			}
			this.exportTo((XVar)(this.exportType), (XVar)(rs), (XVar)(pageSize));

			return null;
		}
		public override XVar getSubsetDataCommand(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(base.getSubsetDataCommand((XVar)(ignoreFilterField)));
			this.reoderCommandForReoderedRows((XVar)(this.getListPSet()), (XVar)(dc));
			return dc;
		}
		public virtual XVar exportTo(dynamic _param_type, dynamic _param_rs, dynamic _param_pageSize)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic rs = XVar.Clone(_param_rs);
			dynamic pageSize = XVar.Clone(_param_pageSize);
			#endregion

			if(MVCFunctions.substr((XVar)(var_type), new XVar(0), new XVar(5)) == "excel")
			{
				var_type = new XVar("excel");
			}
			if(var_type == "excel")
			{
				GlobalVars.locale_info.InitAndSetArrayItem("0", "LOCALE_SGROUPING");
				GlobalVars.locale_info.InitAndSetArrayItem("0", "LOCALE_SMONGROUPING");
				CommonFunctions.ExportToExcel((XVar)(rs), (XVar)(pageSize), this);
			}
			else
			{
				if(var_type == "word")
				{
					this.ExportToWord((XVar)(rs), (XVar)(pageSize));
				}
				else
				{
					if(var_type == "xml")
					{
						this.ExportToXML((XVar)(rs), (XVar)(pageSize));
					}
					else
					{
						if(var_type == "csv")
						{
							GlobalVars.locale_info.InitAndSetArrayItem("0", "LOCALE_SGROUPING");
							GlobalVars.locale_info.InitAndSetArrayItem(".", "LOCALE_SDECIMAL");
							GlobalVars.locale_info.InitAndSetArrayItem("0", "LOCALE_SMONGROUPING");
							GlobalVars.locale_info.InitAndSetArrayItem(".", "LOCALE_SMONDECIMALSEP");
							this.ExportToCSV((XVar)(rs), (XVar)(pageSize));
						}
					}
				}
			}

			return null;
		}
		public virtual XVar ExportToWord(dynamic _param_rs, dynamic _param_pageSize)
		{
			#region pass-by-value parameters
			dynamic rs = XVar.Clone(_param_rs);
			dynamic pageSize = XVar.Clone(_param_pageSize);
			#endregion

			MVCFunctions.Header("Content-Type", "application/vnd.ms-word");
			MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Disposition: attachment;Filename=", CommonFunctions.GetTableURL((XVar)(this.tName)), ".doc")));
			MVCFunctions.Echo("<html>");
			MVCFunctions.Echo(MVCFunctions.Concat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=", GlobalVars.cCharset, "\">"));
			MVCFunctions.Echo("<body>");
			MVCFunctions.Echo("<table border=1>");
			this.WriteTableData((XVar)(rs), (XVar)(pageSize));
			MVCFunctions.Echo("</table>");
			MVCFunctions.Echo("</body>");
			MVCFunctions.Echo("</html>");

			return null;
		}
		public virtual XVar ExportToXML(dynamic _param_rs, dynamic _param_pageSize)
		{
			#region pass-by-value parameters
			dynamic rs = XVar.Clone(_param_rs);
			dynamic pageSize = XVar.Clone(_param_pageSize);
			#endregion

			dynamic eventRes = null, numberOfRows = null, row = null, values = XVar.Array();
			MVCFunctions.Header("Content-Type", "text/xml");
			MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Disposition: attachment;Filename=", CommonFunctions.GetTableURL((XVar)(this.tName)), ".xml")));
			MVCFunctions.Echo(MVCFunctions.Concat("<?xml version=\"1.0\" encoding=\"", GlobalVars.cCharset, "\" standalone=\"yes\"?>\r\n"));
			MVCFunctions.Echo("<table>\r\n");
			this.viewControls.setForExportVar(new XVar("xml"));
			row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc())));
			numberOfRows = new XVar(0);
			while((XVar)((XVar)(!(XVar)(pageSize))  || (XVar)(numberOfRows < pageSize))  && (XVar)(row))
			{
				RunnerContext.pushRecordContext((XVar)(row), this);
				values = XVar.Clone(this.getValuesFromRow((XVar)(row)));
				eventRes = new XVar(true);
				if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeOut"))))
				{
					eventRes = XVar.Clone(this.eventsObject.BeforeOut((XVar)(row), (XVar)(values), this));
				}
				if(XVar.Pack(eventRes))
				{
					numberOfRows++;
					MVCFunctions.Echo("<row>\r\n");
					foreach (KeyValuePair<XVar, dynamic> val in values.GetEnumerator())
					{
						dynamic field = null;
						field = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(CommonFunctions.XMLNameEncode((XVar)(val.Key)))));
						MVCFunctions.Echo(MVCFunctions.Concat("<", field, ">", CommonFunctions.xmlencode((XVar)(values[val.Key])), "</", field, ">\r\n"));
					}
					MVCFunctions.Echo("</row>\r\n");
				}
				RunnerContext.pop();
				row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc())));
			}
			MVCFunctions.Echo("</table>\r\n");

			return null;
		}
		public virtual XVar ExportToCSV(dynamic _param_rs, dynamic _param_pageSize)
		{
			#region pass-by-value parameters
			dynamic rs = XVar.Clone(_param_rs);
			dynamic pageSize = XVar.Clone(_param_pageSize);
			#endregion

			dynamic delimiter = null, eventRes = null, headerParts = XVar.Array(), numberOfRows = null, row = null, values = XVar.Array();
			if((XVar)(this.pSet.chekcExportDelimiterSelection())  && (XVar)(MVCFunctions.strlen((XVar)(this.csvDelimiter))))
			{
				delimiter = XVar.Clone(this.csvDelimiter);
			}
			else
			{
				delimiter = XVar.Clone(this.pSet.getExportDelimiter());
			}
			MVCFunctions.Header("Content-Type", "application/csv");
			MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Disposition: attachment;Filename=", CommonFunctions.GetTableURL((XVar)(this.tName)), ".csv")));
			if(XVar.Equals(XVar.Pack(ProjectSettings.getProjectValue(new XVar("charset"))), XVar.Pack("utf-8")))
			{
				MVCFunctions.printBOM();
			}
			this.viewControls.setForExportVar(new XVar("csv"));
			row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc())));
			headerParts = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in this.selectedFields.GetEnumerator())
			{
				headerParts.InitAndSetArrayItem(MVCFunctions.Concat("\"", MVCFunctions.str_replace(new XVar("\""), new XVar("\"\""), (XVar)(field.Value)), "\""), null);
			}
			MVCFunctions.Echo(MVCFunctions.implode((XVar)(delimiter), (XVar)(headerParts)));
			MVCFunctions.Echo("\r\n");
			numberOfRows = new XVar(0);
			while((XVar)((XVar)(!(XVar)(pageSize))  || (XVar)(numberOfRows < pageSize))  && (XVar)(row))
			{
				RunnerContext.pushRecordContext((XVar)(row), this);
				values = XVar.Clone(this.getValuesFromRow((XVar)(row)));
				eventRes = new XVar(true);
				if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeOut"))))
				{
					eventRes = XVar.Clone(this.eventsObject.BeforeOut((XVar)(row), (XVar)(values), this));
				}
				if(XVar.Pack(eventRes))
				{
					dynamic dataRowParts = XVar.Array();
					dataRowParts = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> field in this.selectedFields.GetEnumerator())
					{
						dataRowParts.InitAndSetArrayItem(MVCFunctions.Concat("\"", MVCFunctions.str_replace(new XVar("\""), new XVar("\"\""), (XVar)(values[field.Value])), "\""), null);
					}
					MVCFunctions.Echo(MVCFunctions.implode((XVar)(delimiter), (XVar)(dataRowParts)));
				}
				RunnerContext.pop();
				numberOfRows++;
				row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc())));
				if((XVar)((XVar)((XVar)(!(XVar)(pageSize))  || (XVar)(numberOfRows < pageSize))  && (XVar)(row))  && (XVar)(eventRes))
				{
					MVCFunctions.Echo("\r\n");
				}
			}

			return null;
		}
		public virtual XVar getFormattedFieldValue(dynamic _param_fName, dynamic _param_row)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic row = XVar.Clone(_param_row);
			#endregion

			if(XVar.Pack(this.useRawValues))
			{
				return row[fName];
			}
			return this.getExportValue((XVar)(fName), (XVar)(row), new XVar(""), (XVar)(this.exportType == "word"));
		}
		protected virtual XVar WriteTableData(dynamic _param_rs, dynamic _param_pageSize)
		{
			#region pass-by-value parameters
			dynamic rs = XVar.Clone(_param_rs);
			dynamic pageSize = XVar.Clone(_param_pageSize);
			#endregion

			dynamic eventRes = null, numberOfRows = null, row = null, totalFieldsData = XVar.Array(), totals = XVar.Array(), totalsFields = XVar.Array(), values = XVar.Array();
			totalFieldsData = XVar.Clone(this.pSet.getTotalsFields());
			row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc())));
			MVCFunctions.Echo("<tr>");
			foreach (KeyValuePair<XVar, dynamic> field in this.selectedFields.GetEnumerator())
			{
				if(this.exportType == "excel")
				{
					MVCFunctions.Echo(MVCFunctions.Concat("<td style=\"width: 100\" x:str>", CommonFunctions.PrepareForExcel((XVar)(this.pSet.label((XVar)(field.Value)))), "</td>"));
				}
				else
				{
					MVCFunctions.Echo(MVCFunctions.Concat("<td>", this.pSet.label((XVar)(field.Value)), "</td>"));
				}
			}
			MVCFunctions.Echo("</tr>");
			totals = XVar.Clone(XVar.Array());
			totalsFields = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> data in totalFieldsData.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(data.Value["fName"]), (XVar)(this.selectedFields)))))
				{
					continue;
				}
				totals.InitAndSetArrayItem(new XVar("value", 0, "numRows", 0), data.Value["fName"]);
				totalsFields.InitAndSetArrayItem(new XVar("fName", data.Value["fName"], "totalsType", data.Value["totalsType"], "viewFormat", this.pSet.getViewFormat((XVar)(data.Value["fName"]))), null);
			}
			numberOfRows = new XVar(0);
			this.viewControls.setForExportVar(new XVar("export"));
			while((XVar)((XVar)(!(XVar)(pageSize))  || (XVar)(numberOfRows < pageSize))  && (XVar)(row))
			{
				RunnerContext.pushRecordContext((XVar)(row), this);
				CommonFunctions.countTotals((XVar)(totals), (XVar)(totalsFields), (XVar)(row));
				values = XVar.Clone(this.getValuesFromRow((XVar)(row)));
				eventRes = new XVar(true);
				if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeOut"))))
				{
					eventRes = XVar.Clone(this.eventsObject.BeforeOut((XVar)(row), (XVar)(values), this));
				}
				if(XVar.Pack(eventRes))
				{
					numberOfRows++;
					MVCFunctions.Echo("<tr>");
					foreach (KeyValuePair<XVar, dynamic> field in this.selectedFields.GetEnumerator())
					{
						dynamic editFormat = null, fType = null;
						fType = XVar.Clone(this.pSet.getFieldType((XVar)(field.Value)));
						if((XVar)(CommonFunctions.IsCharType((XVar)(fType)))  && (XVar)(this.exportType == "excel"))
						{
							MVCFunctions.Echo("<td x:str>");
						}
						else
						{
							MVCFunctions.Echo("<td>");
						}
						editFormat = XVar.Clone(this.pSet.getEditFormat((XVar)(field.Value)));
						if(editFormat == Constants.EDIT_FORMAT_LOOKUP_WIZARD)
						{
							if((XVar)(this.pSet.NeedEncode((XVar)(field.Value)))  && (XVar)(this.exportType == "excel"))
							{
								MVCFunctions.Echo(CommonFunctions.PrepareForExcel((XVar)(values[field.Value])));
							}
							else
							{
								MVCFunctions.Echo(values[field.Value]);
							}
						}
						else
						{
							if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(fType))))
							{
								MVCFunctions.Echo(values[field.Value]);
							}
							else
							{
								if((XVar)((XVar)((XVar)(CommonFunctions.NeedQuotes((XVar)(field.Value)))  && (XVar)(this.exportType == "excel"))  && (XVar)(editFormat != Constants.FORMAT_CUSTOM))  && (XVar)(!(XVar)(this.pSet.isUseRTE((XVar)(field.Value)))))
								{
									MVCFunctions.Echo(CommonFunctions.PrepareForExcel((XVar)(values[field.Value])));
								}
								else
								{
									MVCFunctions.Echo(values[field.Value]);
								}
							}
						}
						MVCFunctions.Echo("</td>");
					}
					MVCFunctions.Echo("</tr>");
				}
				RunnerContext.pop();
				row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc())));
			}
			if(XVar.Pack(MVCFunctions.count(totalFieldsData)))
			{
				MVCFunctions.Echo("<tr>");
				foreach (KeyValuePair<XVar, dynamic> data in totalFieldsData.GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(data.Value["fName"]), (XVar)(this.selectedFields)))))
					{
						continue;
					}
					MVCFunctions.Echo("<td>");
					if(XVar.Pack(MVCFunctions.strlen((XVar)(data.Value["totalsType"]))))
					{
						if(data.Value["totalsType"] == "COUNT")
						{
							MVCFunctions.Echo(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("COUNT")), ": "));
						}
						else
						{
							if(data.Value["totalsType"] == "TOTAL")
							{
								MVCFunctions.Echo(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("TOTAL")), ": "));
							}
							else
							{
								if(data.Value["totalsType"] == "AVERAGE")
								{
									MVCFunctions.Echo(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("AVERAGE")), ": "));
								}
							}
						}
						MVCFunctions.Echo(MVCFunctions.runner_htmlspecialchars((XVar)(CommonFunctions.GetTotals((XVar)(data.Value["fName"]), (XVar)(totals[data.Value["fName"]]["value"]), (XVar)(data.Value["totalsType"]), (XVar)(totals[data.Value["fName"]]["numRows"]), (XVar)(this.pSet.getViewFormat((XVar)(data.Value["fName"]))), new XVar(Constants.PAGE_EXPORT), (XVar)(this.pSet), (XVar)(this.useRawValues), this))));
					}
					MVCFunctions.Echo("</td>");
				}
				MVCFunctions.Echo("</tr>");
			}

			return null;
		}
		public virtual XVar getValuesFromRow(dynamic row)
		{
			dynamic values = XVar.Array();
			values = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in this.selectedFields.GetEnumerator())
			{
				dynamic fType = null;
				fType = XVar.Clone(this.pSet.getFieldType((XVar)(field.Value)));
				if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(fType))))
				{
					values.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("LONG_BINARY")), field.Value);
				}
				else
				{
					values.InitAndSetArrayItem(this.getFormattedFieldValue((XVar)(field.Value), (XVar)(row)), field.Value);
				}
			}
			return values;
		}
		public virtual XVar ExportToExcel_old(dynamic _param_rs, dynamic _param_nPageSize)
		{
			#region pass-by-value parameters
			dynamic rs = XVar.Clone(_param_rs);
			dynamic nPageSize = XVar.Clone(_param_nPageSize);
			#endregion

			MVCFunctions.Header("Content-Type", "application/vnd.ms-excel");
			MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Disposition: attachment;Filename=", CommonFunctions.GetTableURL((XVar)(this.tName)), ".xls")));
			MVCFunctions.Echo("<html>");
			MVCFunctions.Echo("<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns=\"http://www.w3.org/TR/REC-html40\">");
			MVCFunctions.Echo(MVCFunctions.Concat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=", GlobalVars.cCharset, "\">"));
			MVCFunctions.Echo("<body>");
			MVCFunctions.Echo("<table border=1>");
			this.WriteTableData((XVar)(rs), (XVar)(nPageSize));
			MVCFunctions.Echo("</table>");
			MVCFunctions.Echo("</body>");
			MVCFunctions.Echo("</html>");

			return null;
		}
		protected virtual XVar displayExportPage()
		{
			dynamic templatefile = null;
			templatefile = XVar.Clone(this.templatefile);
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowExport"))))
			{
				this.eventsObject.BeforeShowExport((XVar)(this.xt), ref templatefile, this);
			}
			if(this.mode == Constants.EXPORT_POPUP)
			{
				this.xt.assign(new XVar("footer"), new XVar(false));
				this.xt.assign(new XVar("header"), new XVar(false));
				this.xt.assign(new XVar("body"), (XVar)(this.body));
				this.displayAJAX((XVar)(templatefile), (XVar)(this.id + 1));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			this.display((XVar)(templatefile));

			return null;
		}
		public static XVar readModeFromRequest()
		{
			if(XVar.Pack(MVCFunctions.postvalue(new XVar("onFly"))))
			{
				return Constants.EXPORT_POPUP;
			}
			return Constants.EXPORT_SIMPLE;
		}
		public override XVar getDataSourceFilterCriteria(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic filter = null, selectedRecords = XVar.Array();
			filter = XVar.Clone(base.getDataSourceFilterCriteria());
			selectedRecords = XVar.Clone(this.getSelectedRecords());
			if(!XVar.Equals(XVar.Pack(selectedRecords), XVar.Pack(null)))
			{
				dynamic keyFields = null, recConditions = XVar.Array();
				keyFields = XVar.Clone(this.pSet.getTableKeys());
				recConditions = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> keys in selectedRecords.GetEnumerator())
				{
					recConditions.InitAndSetArrayItem(DataCondition.FieldsEqual((XVar)(keyFields), (XVar)(keys.Value)), null);
				}
				filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, filter, 1, DataCondition._Or((XVar)(recConditions))))));
			}
			return filter;
		}
		public override XVar getSecurityCondition()
		{
			return Security.SelectCondition(new XVar("P"), (XVar)(this.pSet));
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
			if(XVar.Pack(this.pSet.chekcExportDelimiterSelection()))
			{
				settings.InitAndSetArrayItem(this.pSet.getExportDelimiter(), "csvDelimiter");
			}
			if(XVar.Pack(this.selection))
			{
				settings.InitAndSetArrayItem(this.getSelection(), "selection");
			}
			return settings;
		}
	}
}
