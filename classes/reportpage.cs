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
	public partial class ReportPage : RunnerPage
	{
		public dynamic pagestart = XVar.Pack(0);
		public dynamic arrReport = XVar.Array();
		public dynamic arrGroupsPerPage = XVar.Array();
		public dynamic crossTable = XVar.Pack(false);
		public dynamic crosstableRefresh = XVar.Pack(false);
		protected dynamic noRecordsFound = XVar.Pack(false);
		protected dynamic crossTableObj = XVar.Pack(null);
		public dynamic pdfJson = XVar.Pack(false);
		public dynamic x;
		public dynamic y;
		public dynamic dataField;
		public dynamic operation;
		public dynamic xType;
		public dynamic yType;
		public dynamic selectedAxis;
		public dynamic requestGoto;
		protected static bool skipReportPageCtor = false;
		public ReportPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipReportPageCtor)
			{
				skipReportPageCtor = false;
				return;
			}
			GlobalVars.init_runnerpage();
			this.crossTable = XVar.Clone(this.pSet.isCrossTabReport());
			if((XVar)((XVar)(this.mode == Constants.REPORT_DASHBOARD)  || (XVar)(this.mode == Constants.REPORT_DETAILS))  || (XVar)(this.mode == Constants.REPORT_DASHDETAILS))
			{
				if(this.mode != Constants.REPORT_DETAILS)
				{
					this.formBricks.InitAndSetArrayItem(new XVar(0, new XVar("name", "details_found", "align", "right")), "header");
				}
				this.formBricks.InitAndSetArrayItem(new XVar(0, "pagination_block"), "footer");
			}
			this.controlsMap.InitAndSetArrayItem(XVar.Array(), "pdfSettings");
			this.controlsMap.InitAndSetArrayItem(0, "pdfSettings", "allPagesMode");
		}
		protected override XVar assignSessionPrefix()
		{
			if(this.mode == Constants.REPORT_DASHBOARD)
			{
				this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.dashTName, "_", this.tName));
			}
			else
			{
				this.sessionPrefix = XVar.Clone(this.tName);
			}

			return null;
		}
		public override XVar setSessionVariables()
		{
			base.setSessionVariables();
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_advsearch")] = MVCFunctions.serialize((XVar)(this.searchClauseObj));
			if(XVar.Pack(!(XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")])))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")] = this.pSet.getInitialPageSize();
			}
			this.pageSize = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")]);
			if(XVar.Pack(!(XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")])))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")] = 1;
			}
			if(XVar.Pack(this.requestGoto))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")] = this.requestGoto;
			}
			this.myPage = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")]);
			this.pagestart = XVar.Clone((this.myPage - 1) * this.pageSize);
			if(this.pageSize < 0)
			{
				this.pagestart = new XVar(0);
			}

			return null;
		}
		public virtual XVar process()
		{
			dynamic extraParams = XVar.Array();
			if((XVar)(this.mode == Constants.REPORT_DASHDETAILS)  || (XVar)((XVar)(this.mode == Constants.REPORT_DETAILS)  && (XVar)((XVar)(this.masterPageType == Constants.PAGE_LIST)  || (XVar)(this.masterPageType == Constants.PAGE_REPORT))))
			{
				this.updateDetailsTabTitles();
			}
			if((XVar)(this.crossTable)  && (XVar)(!(XVar)(this.checkCrossParams())))
			{
				if(this.mode == Constants.REPORT_SIMPLE)
				{
					this.crossTableBaseRedirect();
					return null;
				}
				this.setDefaultParams();
			}
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessReport"))))
			{
				this.eventsObject.BeforeProcessReport(this);
			}
			this.setDetailsBadgeStyles();
			extraParams = XVar.Clone(this.getExtraReportParams());
			this.setGoogleMapsParams((XVar)(extraParams["fieldsArr"]));
			this.fillAdvancedMapData();
			if(XVar.Pack(this.googleMapCfg["isUseGoogleMap"]))
			{
				this.initGmaps();
			}
			if(this.mode != Constants.REPORT_DASHBOARD)
			{
				this.buildSearchPanel();
				this.assignSimpleSearch();
			}
			this.processGridTabs();
			RunnerContext.pushSearchContext((XVar)(this.searchClauseObj));
			this.setReportData((XVar)(extraParams));
			this.addCommonJs();
			this.addButtonHandlers();
			this.commonAssign();
			this.doCommonAssignments();
			this.addCustomCss();
			if(this.mode == Constants.REPORT_SIMPLE)
			{
				this.displayMasterTableInfo();
			}
			this.showPage();

			return null;
		}
		protected virtual XVar checkCrossParams()
		{
			if(XVar.Pack(!(XVar)(this.crossTable)))
			{
				return true;
			}
			return (XVar)((XVar)(MVCFunctions.strlen((XVar)(this.x)))  && (XVar)(MVCFunctions.strlen((XVar)(this.y))))  && (XVar)(MVCFunctions.strlen((XVar)(this.dataField)));
		}
		protected virtual XVar setDefaultParams()
		{
			dynamic prms = XVar.Array();
			prms = XVar.Clone(this.getDefaultCrossParams());
			this.x = XVar.Clone(prms["x"]);
			this.y = XVar.Clone(prms["y"]);
			this.dataField = XVar.Clone(prms["data"]);
			this.operation = XVar.Clone(prms["op"]);

			return null;
		}
		protected virtual XVar getDefaultCrossParams()
		{
			dynamic allNames = XVar.Array(), dataField = null, operation = null, reportFields = XVar.Array(), x = null, xNames = XVar.Array(), y = null, yNames = XVar.Array();
			if(XVar.Pack(!(XVar)(this.crossTable)))
			{
				return XVar.Array();
			}
			xNames = XVar.Clone(XVar.Array());
			yNames = XVar.Clone(XVar.Array());
			allNames = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in this.pSet.getReportGroupFieldsData().GetEnumerator())
			{
				dynamic axisName = null;
				axisName = XVar.Clone(value.Value["strGroupField"]);
				if(value.Value["crossTabAxis"] == 0)
				{
					xNames.InitAndSetArrayItem(axisName, null);
				}
				else
				{
					if(value.Value["crossTabAxis"] == 1)
					{
						yNames.InitAndSetArrayItem(axisName, null);
					}
					else
					{
						allNames.InitAndSetArrayItem(axisName, null);
					}
				}
			}
			if(XVar.Pack(xNames))
			{
				x = XVar.Clone(xNames[0]);
			}
			else
			{
				x = XVar.Clone(allNames[0]);
			}
			if(XVar.Pack(yNames))
			{
				y = XVar.Clone(yNames[0]);
			}
			else
			{
				if(XVar.Pack(!(XVar)(xNames)))
				{
					y = XVar.Clone(allNames[1]);
				}
				else
				{
					y = XVar.Clone(allNames[0]);
				}
			}
			dataField = new XVar("");
			operation = new XVar("");
			reportFields = XVar.Clone(this.pSet.getFieldsList());
			foreach (KeyValuePair<XVar, dynamic> field in reportFields.GetEnumerator())
			{
				dynamic fieldInfo = XVar.Array();
				operation = new XVar("");
				fieldInfo = XVar.Clone(this.pSet.reportFieldInfo((XVar)(field.Value)));
				if(XVar.Pack(fieldInfo))
				{
					if(XVar.Pack(fieldInfo["max"]))
					{
						operation = new XVar("max");
					}
					else
					{
						if(XVar.Pack(fieldInfo["min"]))
						{
							operation = new XVar("min");
						}
						else
						{
							if(XVar.Pack(fieldInfo["avg"]))
							{
								operation = new XVar("avg");
							}
							else
							{
								if(XVar.Pack(fieldInfo["sum"]))
								{
									operation = new XVar("sum");
								}
							}
						}
					}
				}
				if(XVar.Pack(operation))
				{
					dataField = XVar.Clone(field.Value);
					break;
				}
			}
			return new XVar("x", x, "y", y, "data", dataField, "op", operation);
		}
		protected virtual XVar getCurrentCrossParams()
		{
			dynamic prms = XVar.Array();
			prms = XVar.Clone(new XVar("x", this.x, "y", this.y, "data", this.dataField, "op", this.operation));
			if(XVar.Pack(this.xType))
			{
				prms.InitAndSetArrayItem(this.xType, "xtype");
			}
			if(XVar.Pack(this.yType))
			{
				prms.InitAndSetArrayItem(this.yType, "ytype");
			}
			return prms;
		}
		protected virtual XVar getDefaultCrossParamsString()
		{
			dynamic prms = XVar.Array();
			prms = XVar.Clone(this.getDefaultCrossParams());
			return MVCFunctions.Concat("x=", prms["x"], "&y=", prms["y"], "&data=", prms["data"], "&op=", prms["op"]);
		}
		protected virtual XVar crossTableBaseRedirect()
		{
			MVCFunctions.HeaderRedirect((XVar)(this.pSet.getShortTableName()), (XVar)(this.getPageType()), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(new XVar(0, this.getDefaultCrossParamsString(), 1, this.getStateUrlParams())))));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public virtual XVar setReportData(dynamic _param_options)
		{
			#region pass-by-value parameters
			dynamic options = XVar.Clone(_param_options);
			#endregion

			if(XVar.Pack(this.crossTable))
			{
				this.setCrosstabData((XVar)(options));
			}
			else
			{
				this.setStandartData((XVar)(options));
			}

			return null;
		}
		protected virtual XVar getCrossGroupFieldsSettings(dynamic _param_repGroupFields)
		{
			#region pass-by-value parameters
			dynamic repGroupFields = XVar.Clone(_param_repGroupFields);
			#endregion

			dynamic groupFields = XVar.Array(), xFieldsCount = XVar.Array(), xNames = XVar.Array(), yFieldsCount = XVar.Array(), yNames = XVar.Array();
			groupFields = XVar.Clone(XVar.Array());
			xNames = XVar.Clone(XVar.Array());
			yNames = XVar.Clone(XVar.Array());
			xFieldsCount = XVar.Clone(XVar.Array());
			yFieldsCount = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in repGroupFields.GetEnumerator())
			{
				if((XVar)(value.Value["crossTabAxis"] == 0)  || (XVar)(value.Value["crossTabAxis"] == 2))
				{
					++(xFieldsCount[value.Value["strGroupField"]]);
				}
				if((XVar)(value.Value["crossTabAxis"] == 1)  || (XVar)(value.Value["crossTabAxis"] == 2))
				{
					++(yFieldsCount[value.Value["strGroupField"]]);
				}
			}
			foreach (KeyValuePair<XVar, dynamic> value in repGroupFields.GetEnumerator())
			{
				dynamic xAxis = null, yAxis = null;
				groupFields.InitAndSetArrayItem(value.Value["strGroupField"], value.Key, "name");
				xAxis = XVar.Clone((XVar)(value.Value["crossTabAxis"] == 0)  || (XVar)(value.Value["crossTabAxis"] == 2));
				yAxis = XVar.Clone((XVar)(value.Value["crossTabAxis"] == 1)  || (XVar)(value.Value["crossTabAxis"] == 2));
				if((XVar)((XVar)(0 == value.Value["groupInterval"])  || (XVar)((XVar)(xAxis)  && (XVar)(xFieldsCount[value.Value["strGroupField"]] < 2)))  || (XVar)((XVar)(yAxis)  && (XVar)(yFieldsCount[value.Value["strGroupField"]] < 2)))
				{
					groupFields.InitAndSetArrayItem(this.pSet.label((XVar)(value.Value["strGroupField"])), value.Key, "label");
				}
				else
				{
					groupFields.InitAndSetArrayItem(MVCFunctions.Concat(this.pSet.label((XVar)(value.Value["strGroupField"])), " - ", CrossTableReport.getCrossIntervalName((XVar)(this.pSet.getFieldType((XVar)(value.Value["strGroupField"]))), (XVar)(value.Value["groupInterval"]))), value.Key, "label");
				}
				groupFields.InitAndSetArrayItem(true, value.Key, "uniqueName");
				groupFields.InitAndSetArrayItem(value.Value["groupInterval"], value.Key, "int_type");
				if(XVar.Pack(!(XVar)(xNames[value.Value["strGroupField"]])))
				{
					xNames.InitAndSetArrayItem(XVar.Array(), value.Value["strGroupField"]);
				}
				if(XVar.Pack(!(XVar)(yNames[value.Value["strGroupField"]])))
				{
					yNames.InitAndSetArrayItem(XVar.Array(), value.Value["strGroupField"]);
				}
				if(value.Value["crossTabAxis"] == 0)
				{
					groupFields.InitAndSetArrayItem("x", value.Key, "group_type");
					xNames.InitAndSetArrayItem(value.Key, value.Value["strGroupField"], null);
				}
				else
				{
					if(value.Value["crossTabAxis"] == 1)
					{
						groupFields.InitAndSetArrayItem("y", value.Key, "group_type");
						yNames.InitAndSetArrayItem(value.Key, value.Value["strGroupField"], null);
					}
					else
					{
						groupFields.InitAndSetArrayItem("all", value.Key, "group_type");
						xNames.InitAndSetArrayItem(value.Key, value.Value["strGroupField"], null);
						yNames.InitAndSetArrayItem(value.Key, value.Value["strGroupField"], null);
					}
				}
			}
			foreach (KeyValuePair<XVar, dynamic> indices in xNames.GetEnumerator())
			{
				if(1 < MVCFunctions.count(indices.Value))
				{
					foreach (KeyValuePair<XVar, dynamic> ind in indices.Value.GetEnumerator())
					{
						groupFields.InitAndSetArrayItem(false, ind.Value, "uniqueName");
					}
				}
			}
			foreach (KeyValuePair<XVar, dynamic> indices in yNames.GetEnumerator())
			{
				if(1 < MVCFunctions.count(indices.Value))
				{
					foreach (KeyValuePair<XVar, dynamic> ind in indices.Value.GetEnumerator())
					{
						groupFields.InitAndSetArrayItem(false, ind.Value, "uniqueName");
					}
				}
			}
			return groupFields;
		}
		public virtual XVar setCrosstabData(dynamic _options)
		{
			dynamic var_params = XVar.Array();
			if((XVar)(this.pSetSearch.noRecordsOnFirstPage())  && (XVar)(!(XVar)(this.isSearchFunctionalityActivated())))
			{
				this.xt.assign(new XVar("container_grid"), new XVar(false));
				this.showNoRecordsMessage();
			}
			GlobalVars.init_crosstable_report();
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(this.selectedAxis, "selectedAxis");
			var_params.InitAndSetArrayItem(this.x, "x");
			var_params.InitAndSetArrayItem(this.y, "y");
			var_params.InitAndSetArrayItem(this.dataField, "data");
			var_params.InitAndSetArrayItem(this.xType, "xType");
			var_params.InitAndSetArrayItem(this.yType, "yType");
			var_params.InitAndSetArrayItem(this.operation, "operation");
			var_params.InitAndSetArrayItem(this.getFieldClass((XVar)(this.x)), "headerClass");
			var_params.InitAndSetArrayItem(this.getFieldClass((XVar)(this.dataField)), "dataClass");
			var_params.InitAndSetArrayItem(this.tName, "tableName");
			var_params.InitAndSetArrayItem(this.pageType, "pageType");
			var_params.InitAndSetArrayItem(this.getCrossGroupFieldsSettings((XVar)(_options["repGroupFields"])), "groupFields");
			var_params.InitAndSetArrayItem(this.pSet.reportHasHorizontalSummary(), "xSummary");
			var_params.InitAndSetArrayItem(this.pSet.reportHasVerticalSummary(), "ySummary");
			var_params.InitAndSetArrayItem((XVar)(var_params["xSummary"])  || (XVar)(var_params["ySummary"]), "totalSummary");
			foreach (KeyValuePair<XVar, dynamic> value in _options["fieldsArr"].GetEnumerator())
			{
				var_params.InitAndSetArrayItem(value.Value["name"], "totals", value.Value["name"], "name");
				var_params.InitAndSetArrayItem(value.Value["label"], "totals", value.Value["name"], "label");
				var_params.InitAndSetArrayItem(value.Value["totalMax"], "totals", value.Value["name"], "max");
				var_params.InitAndSetArrayItem(value.Value["totalMin"], "totals", value.Value["name"], "min");
				var_params.InitAndSetArrayItem(value.Value["totalSum"], "totals", value.Value["name"], "sum");
				var_params.InitAndSetArrayItem(value.Value["totalAvg"], "totals", value.Value["name"], "avg");
			}
			if(XVar.Pack(this.pdfJsonMode()))
			{
				var_params.InitAndSetArrayItem(true, "pdfJSON");
			}
			this.crossTableObj = XVar.Clone(new CrossTableReport((XVar)(var_params), this));
			if(XVar.Pack(this.crosstableRefresh))
			{
				this.refreshCrossTable();
				return null;
			}
			if(XVar.Pack(this.crossTableObj.isEmpty()))
			{
				this.noRecordsFound = new XVar(true);
			}
			this.crossTableCommonAssign((XVar)(var_params["totalSummary"]));

			return null;
		}
		protected virtual XVar refreshCrossTable()
		{
			dynamic reportData = XVar.Array();
			reportData = XVar.Clone(XVar.Array());
			reportData.InitAndSetArrayItem(this.crossTableObj.getCrossTableData(), "rowsInfo");
			reportData.InitAndSetArrayItem(this.crossTableObj.getTotalsName(), "totalsName");
			reportData.InitAndSetArrayItem(this.crossTableObj.getCrossTableSummary(), "columnSummary");
			reportData.InitAndSetArrayItem(this.crossTableObj.getTotalSummary(), "totalSummary");
			reportData.InitAndSetArrayItem(this.getOperationCtrlMarkup(), "groupFuncCtrl");
			reportData.InitAndSetArrayItem(this.getFieldClass((XVar)(this.dataField)), "dataClass");
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(reportData)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar getCrossFieldCtrlMarkup(dynamic _param_axis)
		{
			#region pass-by-value parameters
			dynamic axis = XVar.Clone(_param_axis);
			#endregion

			dynamic classAttr = null, hiddenAttr = null, labelTag = null, lastLabel = null, options = XVar.Array();
			classAttr = XVar.Clone(MVCFunctions.Concat("form-control bs-cross-dd", axis));
			options = XVar.Clone(XVar.Array());
			lastLabel = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> fData in this.crossTableObj.getCrossFieldsData((XVar)(axis)).GetEnumerator())
			{
				dynamic intervalDataAttr = null;
				intervalDataAttr = XVar.Clone((XVar.Pack(fData.Value["intervalType"]) ? XVar.Pack(MVCFunctions.Concat(" data-", axis, "type=\"", fData.Value["intervalType"], "\" ")) : XVar.Pack("")));
				lastLabel = XVar.Clone(fData.Value["label"]);
				options.InitAndSetArrayItem(MVCFunctions.Concat("<option ", intervalDataAttr, " value=\"", fData.Value["value"], "\" ", fData.Value["selected"], ">", lastLabel, "</option>"), null);
			}
			hiddenAttr = new XVar("");
			labelTag = new XVar("");
			if(MVCFunctions.count(options) < 2)
			{
				labelTag = XVar.Clone(MVCFunctions.Concat("<span class=\"", classAttr, " like-text\">", lastLabel, "</span>"));
				hiddenAttr = new XVar("style=\"display: none;\"");
			}
			return MVCFunctions.Concat("<select ", hiddenAttr, " id=\"select_group_", axis, this.id, "\" class=\"", classAttr, "\">", MVCFunctions.implode(new XVar(""), (XVar)(options)), "</select>", labelTag);
		}
		protected virtual XVar getDataFieldCtrlMarkup()
		{
			dynamic classAttr = null, hiddenAttr = null, labelTag = null, lastLabel = null, options = XVar.Array();
			classAttr = new XVar("form-control bs-cross-ddvalue");
			options = XVar.Clone(XVar.Array());
			lastLabel = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> fData in this.crossTableObj.getDataFieldsList().GetEnumerator())
			{
				lastLabel = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(fData.Value["label"])));
				options.InitAndSetArrayItem(MVCFunctions.Concat("<option value=\"", fData.Value["value"], "\" ", fData.Value["selected"], ">", lastLabel, "</option>"), null);
			}
			hiddenAttr = new XVar("");
			labelTag = new XVar("");
			if(MVCFunctions.count(options) < 2)
			{
				labelTag = XVar.Clone(MVCFunctions.Concat("<span class=\"", classAttr, " like-text\">", lastLabel, "</span>"));
				hiddenAttr = new XVar("style=\"display: none;\"");
			}
			return MVCFunctions.Concat("<select ", hiddenAttr, " id=\"select_data", this.id, "\" class=\"", classAttr, "\">", MVCFunctions.implode(new XVar(""), (XVar)(options)), "</select>", labelTag);
		}
		protected virtual XVar getOldOperationCtrlMarkup()
		{
			dynamic inputCtrls = XVar.Array();
			inputCtrls = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> opData in this.crossTableObj.getCurrentOperationList().GetEnumerator())
			{
				dynamic cheked = null;
				cheked = XVar.Clone((XVar.Pack(opData.Value["selected"] == "selected") ? XVar.Pack("checked") : XVar.Pack("")));
				inputCtrls.InitAndSetArrayItem(MVCFunctions.Concat("<input type=radio value='", opData.Value["value"], "' name=\"group_func", this.id, "\" ", cheked, "> ", opData.Value["label"]), null);
			}
			return MVCFunctions.implode(new XVar("&nbsp;&nbsp;"), (XVar)(inputCtrls));
		}
		protected virtual XVar getOperationCtrlMarkup()
		{
			dynamic lastLabel = null, options = XVar.Array();
			options = XVar.Clone(XVar.Array());
			lastLabel = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> opData in this.crossTableObj.getCurrentOperationList().GetEnumerator())
			{
				lastLabel = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(opData.Value["label"])));
				options.InitAndSetArrayItem(MVCFunctions.Concat("<option value=\"", opData.Value["value"], "\" ", opData.Value["selected"], ">", lastLabel, "</option>"), null);
			}
			if(MVCFunctions.count(options) < 2)
			{
				return MVCFunctions.Concat("<span id=\"group_func", this.id, "\" class=\"bs-cross-ddgroup form-control like-text\">", lastLabel, "</span>");
			}
			return MVCFunctions.Concat("<select id=\"group_func", this.id, "\" class=\"bs-cross-ddgroup form-control\">", MVCFunctions.implode(new XVar(""), (XVar)(options)), "</select>");
		}
		protected virtual XVar crossTableCommonAssign(dynamic _param_showSummary)
		{
			#region pass-by-value parameters
			dynamic showSummary = XVar.Clone(_param_showSummary);
			#endregion

			dynamic allow_export = null, grid_row = XVar.Array();
			this.xt.assign(new XVar("cross_controls"), new XVar(true));
			this.xt.assign(new XVar("select_group_x"), (XVar)(this.getCrossFieldCtrlMarkup(new XVar("x"))));
			this.xt.assign(new XVar("select_group_y"), (XVar)(this.getCrossFieldCtrlMarkup(new XVar("y"))));
			this.xt.assign(new XVar("select_data"), (XVar)(this.getDataFieldCtrlMarkup()));
			this.xt.assign(new XVar("select_group"), (XVar)(this.getOperationCtrlMarkup()));
			this.hideItemType(new XVar("details_found"));
			this.hideItemType(new XVar("page_size"));
			this.xt.assign(new XVar("totals"), (XVar)(this.crossTableObj.getTotalsName()));
			grid_row.InitAndSetArrayItem(this.crossTableObj.getCrossTableData(), "data");
			if(XVar.Pack(!(XVar)(Security.permissionsAvailable())))
			{
				allow_export = new XVar(true);
			}
			else
			{
				allow_export = XVar.Clone(this.permis[this.tName]["export"]);
			}
			this.xt.assign(new XVar("export_link"), (XVar)((XVar)(allow_export)  && (XVar)(!(XVar)(this.noRecordsFound))));
			this.xt.assign(new XVar("prints_block"), (XVar)((XVar)(this.printAvailable())  && (XVar)(!(XVar)(this.noRecordsFound))));
			if(XVar.Pack(!(XVar)(this.isDashboardElement())))
			{
				this.xt.assign(new XVar("print_friendly"), (XVar)((XVar)(this.printAvailable())  && (XVar)(!(XVar)(this.noRecordsFound))));
			}
			if(XVar.Pack(!(XVar)(this.noRecordsFound)))
			{
				dynamic headerColspan = null, headers = null;
				this.xt.assign(new XVar("grid_row"), (XVar)(grid_row));
				headers = XVar.Clone(this.crossTableObj.getCrossTableHeader());
				this.xt.assignbyref(new XVar("group_header"), (XVar)(headers));
				this.xt.assignbyref(new XVar("col_summary"), (XVar)(this.crossTableObj.getCrossTableSummary()));
				this.xt.assignbyref(new XVar("total_summary"), (XVar)(this.crossTableObj.getTotalSummary()));
				this.xt.assignbyref(new XVar("summary_class"), (XVar)(this.getFieldClass((XVar)(this.dataField))));
				this.xt.assign(new XVar("cross_totals"), (XVar)(showSummary));
				headerColspan = XVar.Clone(MVCFunctions.count(grid_row["data"][0]["row_record"]["data"]));
				if(XVar.Pack(grid_row["data"][0]["row_record"]["cross_totals"]))
				{
					++(headerColspan);
				}
				if(1 < headerColspan)
				{
					this.xt.assign(new XVar("xselcell_attrs"), (XVar)(MVCFunctions.Concat("colspan=", headerColspan)));
				}
			}
			this.xt.assign(new XVar("grid_header"), (XVar)(!(XVar)(this.noRecordsFound)));

			return null;
		}
		public virtual XVar setStandartData(dynamic _options)
		{
			dynamic pageSize = null, rb = null;
			GlobalVars.init_reportlib();
			pageSize = XVar.Clone(this.pageSize);
			if(this.pageSize == -1)
			{
				pageSize = new XVar(0);
			}
			rb = XVar.Clone(new Report((XVar)(this.pSet.getOrderIndexes()), (XVar)(this.connection), (XVar)(pageSize), new XVar(0), (XVar)(_options), this));
			this.arrReport = XVar.Clone(rb.getReport((XVar)(this.pagestart)));
			this.setRecordsId();
			this.setDetailLinks();
			this.buildPagination();
			this.standardReportCommonAssign();
			this.assignColumnHeaderClasses();

			return null;
		}
		public override XVar getMasterTableSQLClause()
		{
			if((XVar)(this.mode == Constants.REPORT_DASHBOARD)  && (XVar)(!(XVar)(this.dashElementData.KeyExists("masterTable"))))
			{
				return "";
			}
			return base.getMasterTableSQLClause();
		}
		protected virtual XVar standardReportCommonAssign()
		{
			dynamic allow_export = null, allow_search = null;
			this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(this.tName)), "_dtable_column")), new XVar(true));
			this.xt.assign(new XVar("dtables_link"), new XVar(true));
			foreach (KeyValuePair<XVar, dynamic> value in this.arrReport["page"].GetEnumerator())
			{
				this.xt.assign((XVar)(value.Key), (XVar)(value.Value));
			}
			foreach (KeyValuePair<XVar, dynamic> value in this.arrReport["global"].GetEnumerator())
			{
				this.xt.assign((XVar)(value.Key), (XVar)(value.Value));
			}
			if(XVar.Pack(!(XVar)(!(XVar)(this.arrReport["list"]))))
			{
				this.xt.assign(new XVar("grid_row"), (XVar)(new XVar("data", this.arrReport["list"])));
			}
			else
			{
				this.noRecordsFound = new XVar(true);
			}
			if(XVar.Pack(!(XVar)(Security.permissionsAvailable())))
			{
				allow_export = new XVar(true);
				allow_search = new XVar(true);
			}
			else
			{
				allow_export = XVar.Clone(this.permis[this.tName]["export"]);
				allow_search = XVar.Clone(this.permis[this.tName]["search"]);
			}
			this.xt.assign(new XVar("export_link"), (XVar)((XVar)(allow_export)  && (XVar)(0 < this.arrReport["countRows"])));
			this.xt.assign(new XVar("prints_block"), (XVar)((XVar)(allow_export)  && (XVar)(0 < this.arrReport["countRows"])));
			this.xt.assign(new XVar("printall_link"), (XVar)((XVar)((XVar)(allow_export)  && (XVar)(this.pageSize < this.arrReport["countRows"]))  && (XVar)(0 < this.pageSize)));
			this.xt.assign(new XVar("print_recspp"), (XVar)(this.pSet.getReportPrintGroupsPerPage()));
			if(XVar.Pack(!(XVar)(this.isDashboardElement())))
			{
				this.xt.assign(new XVar("print_friendly"), (XVar)((XVar)(this.printAvailable())  && (XVar)(0 < this.arrReport["countRows"])));
				this.xt.assign(new XVar("print_friendly_all"), (XVar)((XVar)(this.printAvailable())  && (XVar)(0 < this.arrReport["countRows"])));
			}
			if((XVar)((XVar)(this.mode == Constants.REPORT_SIMPLE)  && (XVar)(allow_search))  && (XVar)(!(XVar)(!(XVar)(this.arrGroupsPerPage))))
			{
				this.xt.assign(new XVar("recordspp_block"), new XVar(true));
				this.createPerPage();
			}
			this.xt.assign(new XVar("details_found"), (XVar)(this.arrReport["countRows"] != 0));
			if(XVar.Pack(this.noRecordsFound))
			{
				this.hideItemType(new XVar("details_found"));
				this.hideItemType(new XVar("page_size"));
			}
			this.xt.assign(new XVar("details_block"), (XVar)(this.arrReport["countRows"] != 0));
			this.xt.assign(new XVar("records_found"), (XVar)(this.arrReport["countRows"]));
			this.xt.assign(new XVar("pages_block"), (XVar)(this.arrReport["countRows"] != 0));
			this.xt.assign(new XVar("page"), (XVar)(this.myPage));
			this.xt.assign(new XVar("maxpages"), (XVar)(this.maxPages));
			this.xt.assign(new XVar("global_summary"), (XVar)(!(XVar)(this.noRecordsFound)));
			this.xt.assign(new XVar("page_summary"), (XVar)(!(XVar)(this.noRecordsFound)));
			this.xt.assign(new XVar("summary_header"), (XVar)(!(XVar)(this.noRecordsFound)));
			this.xt.assign(new XVar("grid_header"), (XVar)(!(XVar)(this.noRecordsFound)));

			return null;
		}
		public override XVar buildPagination()
		{
			dynamic advSeparator = null, lastrecord = null, separator = null;
			this.maxPages = XVar.Clone(this.arrReport["maxpages"]);
			lastrecord = XVar.Clone(this.myPage * this.pageSize);
			if((XVar)(this.pageSize < 0)  || (XVar)(this.arrReport["countRows"] < lastrecord))
			{
				lastrecord = XVar.Clone(this.arrReport["countRows"]);
			}
			this.prepareRecordsIndicator((XVar)((this.myPage - 1) * this.pageSize + 1), (XVar)(lastrecord), (XVar)(this.arrReport["countRows"]));
			separator = new XVar("");
			advSeparator = new XVar("");
			if(1 < this.maxPages)
			{
				dynamic counter = null, counterend = null, counterstart = null, limit = null, pageLinks = null, pagination = null;
				this.xt.assign(new XVar("pagination_block"), new XVar(true));
				pagination = new XVar("");
				limit = new XVar(10);
				counterstart = XVar.Clone(this.myPage - (limit - 1));
				if(this.myPage  %  limit != 0)
				{
					counterstart = XVar.Clone((this.myPage - this.myPage  %  limit) + 1);
				}
				counterend = XVar.Clone(counterstart + (limit - 1));
				if(this.maxPages < counterend)
				{
					counterend = XVar.Clone(this.maxPages);
				}
				if(counterstart != 1)
				{
					pagination = MVCFunctions.Concat(pagination, this.getPaginationLink(new XVar(1), (XVar)(CommonFunctions.mlang_message(new XVar("FIRST")))), advSeparator);
					pagination = MVCFunctions.Concat(pagination, this.getPaginationLink((XVar)(counterstart - 1), (XVar)(CommonFunctions.mlang_message(new XVar("PREVIOUS")))), separator);
				}
				pageLinks = new XVar("");
				counter = XVar.Clone(counterstart);
				for(;counter <= counterend; counter++)
				{
					pageLinks = MVCFunctions.Concat(pageLinks, separator, this.getPaginationLink((XVar)(counter), (XVar)(counter), (XVar)(counter == this.myPage)));
				}
				pagination = MVCFunctions.Concat(pagination, pageLinks);
				if(counterend != this.maxPages)
				{
					pagination = MVCFunctions.Concat(pagination, separator, this.getPaginationLink((XVar)(counterend + 1), (XVar)(CommonFunctions.mlang_message(new XVar("NEXT")))), advSeparator);
					pagination = MVCFunctions.Concat(pagination, separator, this.getPaginationLink((XVar)(this.maxPages), (XVar)(CommonFunctions.mlang_message(new XVar("LAST")))));
				}
				pagination = XVar.Clone(MVCFunctions.Concat("<nav><ul class=\"pagination\" data-function=\"pagination", this.id, "\">", pagination, "</ul></nav>"));
				this.xt.assign(new XVar("pagination"), (XVar)(pagination));
			}
			else
			{
				if(XVar.Pack(!(XVar)(this.myPage)))
				{
					this.myPage = new XVar(1);
				}
			}

			return null;
		}
		protected virtual XVar setRecordsId()
		{
			dynamic i = null, recCount = null;
			recCount = XVar.Clone(MVCFunctions.count(this.arrReport["list"]));
			i = new XVar(0);
			for(;i < recCount; ++(i))
			{
				this.genId();
				this.arrReport.InitAndSetArrayItem(this.recId, "list", i, "recId");
			}

			return null;
		}
		protected virtual XVar setDetailLinks()
		{
			dynamic arrReportList = XVar.Array();
			if(this.mode == Constants.REPORT_DASHBOARD)
			{
				return null;
			}
			return null;
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				this.permis.InitAndSetArrayItem(this.getPermissions((XVar)(details.Value)), details.Value);
			}
			this.controlsMap.InitAndSetArrayItem(XVar.Array(), "gridRows");
			arrReportList = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> data in this.arrReport["list"].GetEnumerator())
			{
				dynamic gridRowInd = null, i = null, recId = null, record = XVar.Array(), tKeys = XVar.Array();
				if(XVar.Pack(!(XVar)(data.Value.KeyExists("row_data"))))
				{
					continue;
				}
				record = XVar.Clone(XVar.Array());
				recId = XVar.Clone(data.Value["recId"]);
				gridRowInd = XVar.Clone(MVCFunctions.count(this.controlsMap["gridRows"]));
				this.controlsMap.InitAndSetArrayItem(XVar.Array(), "gridRows", gridRowInd);
				this.controlsMap.InitAndSetArrayItem(recId, "gridRows", gridRowInd, "id");
				this.controlsMap.InitAndSetArrayItem(gridRowInd, "gridRows", gridRowInd, "rowInd");
				this.controlsMap.InitAndSetArrayItem(XVar.Array(), "gridRows", gridRowInd, "keyFields");
				this.controlsMap.InitAndSetArrayItem(XVar.Array(), "gridRows", gridRowInd, "keys");
				i = new XVar(0);
				for(;i < MVCFunctions.count(tKeys); i++)
				{
					this.controlsMap.InitAndSetArrayItem(tKeys[i], "gridRows", gridRowInd, "keyFields", i);
					this.controlsMap.InitAndSetArrayItem(data.Value[MVCFunctions.Concat(tKeys[i], "_value")], "gridRows", gridRowInd, "keys", i);
				}
				this.proccessDetailGridInfo((XVar)(record), (XVar)(data.Value), (XVar)(gridRowInd));
				record.InitAndSetArrayItem(MVCFunctions.Concat("data-record-id=\"", recId, "\""), "recordattrs");
				record.InitAndSetArrayItem(MVCFunctions.Concat(" id=\"gridRow", recId, "\""), "rowattrs");
				arrReportList.InitAndSetArrayItem(MVCFunctions.array_merge_assoc((XVar)(data.Value), (XVar)(record)), data.Key);
				this.recIds.InitAndSetArrayItem(recId, null);
			}
			foreach (KeyValuePair<XVar, dynamic> data in arrReportList.GetEnumerator())
			{
				this.arrReport.InitAndSetArrayItem(data.Value, "list", data.Key);
			}

			return null;
		}
		public virtual XVar prepareDetailsForEditViewPage()
		{
			this.addButtonHandlers();
			this.commonAssign();
			this.setReportData((XVar)(this.getExtraReportParams()));
			this.xt.assign(new XVar("cross_controls"), new XVar(false));
			this.xt.assign(new XVar("grid_block"), new XVar(true));
			this.xt.assign(new XVar("recordspp_block"), new XVar(true));
			this.doCommonAssignments();
			this.createPerPage();
			this.body.InitAndSetArrayItem("", "begin");
			this.body.InitAndSetArrayItem("", "end");
			this.xt.assign(new XVar("body"), (XVar)(this.body));

			return null;
		}
		public virtual XVar getExtraReportParams()
		{
			dynamic extraParams = XVar.Array(), paramfieldArr = XVar.Array(), reportFields = XVar.Array();
			extraParams = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(this.crossTable)))
			{
				extraParams.InitAndSetArrayItem(this.tName, "tName");
				extraParams.InitAndSetArrayItem(this.shortTableName, "shortTName");
				extraParams.InitAndSetArrayItem(this.sessionPrefix, "sessionPrefix");
				extraParams.InitAndSetArrayItem(this.shortTableName, "shortTableName");
				extraParams.InitAndSetArrayItem(this.pSet.getTableKeys(), "tKeyFields");
				extraParams.InitAndSetArrayItem(this.pSet.reportHasPageSummary(), "repPageSummary");
				extraParams.InitAndSetArrayItem(this.pSet.reportHasGlobalSummary(), "repGlobalSummary");
				extraParams.InitAndSetArrayItem(this.pSet.getReportLayout(), "repLayout");
				extraParams.InitAndSetArrayItem(this.pSet.isGroupSummaryCountShown(), "showGroupSummaryCount");
				extraParams.InitAndSetArrayItem(this.pSet.reportDetailsShown(), "repShowDet");
				extraParams.InitAndSetArrayItem(this.pSet.reportTotalFieldsExist(), "isExistTotalFields");
			}
			extraParams.InitAndSetArrayItem(this.pSet.getReportGroupFieldsData(), "repGroupFields");
			extraParams.InitAndSetArrayItem(MVCFunctions.count(extraParams["repGroupFields"]), "repGroupFieldsCount");
			paramfieldArr = XVar.Clone(XVar.Array());
			reportFields = XVar.Clone(this.pSet.getFieldsList());
			foreach (KeyValuePair<XVar, dynamic> field in reportFields.GetEnumerator())
			{
				dynamic fieldArr = XVar.Array(), fieldInfo = XVar.Array();
				fieldArr = XVar.Clone(XVar.Array());
				fieldArr.InitAndSetArrayItem(field.Value, "name");
				fieldArr.InitAndSetArrayItem(field.Value, "fName");
				fieldArr.InitAndSetArrayItem(this.pSet.label((XVar)(field.Value)), "label");
				fieldArr.InitAndSetArrayItem(MVCFunctions.GoodFieldName((XVar)(field.Value)), "goodName");
				fieldArr.InitAndSetArrayItem(true, "repPage");
				fieldArr.InitAndSetArrayItem(this.pSet.getViewFormat((XVar)(field.Value)), "viewFormat");
				fieldArr.InitAndSetArrayItem(this.pSet.getEditFormat((XVar)(field.Value)), "editFormat");
				fieldInfo = XVar.Clone(this.pSet.reportFieldInfo((XVar)(field.Value)));
				if(XVar.Pack(fieldInfo))
				{
					fieldArr.InitAndSetArrayItem(!(XVar)(!(XVar)(fieldInfo["max"])), "totalMax");
					fieldArr.InitAndSetArrayItem(!(XVar)(!(XVar)(fieldInfo["min"])), "totalMin");
					fieldArr.InitAndSetArrayItem(!(XVar)(!(XVar)(fieldInfo["avg"])), "totalAvg");
					fieldArr.InitAndSetArrayItem(!(XVar)(!(XVar)(fieldInfo["sum"])), "totalSum");
				}
				paramfieldArr.InitAndSetArrayItem(fieldArr, null);
			}
			extraParams.InitAndSetArrayItem(paramfieldArr, "fieldsArr");
			return extraParams;
		}
		public override XVar createPerPage()
		{
			dynamic allMessage = null, classString = null, i = null, reportGroupFields = null, rpp = null;
			classString = new XVar("class=\"form-control\"");
			allMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("ALL")));
			rpp = XVar.Clone(MVCFunctions.Concat("<select ", classString, " id=\"recordspp", this.id, "\">"));
			reportGroupFields = XVar.Clone(this.pSet.isReportWithGroups());
			if(XVar.Pack(reportGroupFields))
			{
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.arrGroupsPerPage); i++)
				{
					if(this.arrGroupsPerPage[i] != -1)
					{
						rpp = MVCFunctions.Concat(rpp, "<option value=\"", this.arrGroupsPerPage[i], "\" ", (XVar.Pack(this.pageSize == this.arrGroupsPerPage[i]) ? XVar.Pack("selected") : XVar.Pack("")), ">", this.arrGroupsPerPage[i], "</option>");
					}
					else
					{
						rpp = MVCFunctions.Concat(rpp, "<option value=\"-1\" ", (XVar.Pack(this.pageSize == this.arrGroupsPerPage[i]) ? XVar.Pack("selected") : XVar.Pack("")), ">", allMessage, "</option>");
					}
				}
			}
			else
			{
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.arrRecsPerPage); i++)
				{
					if(this.arrRecsPerPage[i] != -1)
					{
						rpp = MVCFunctions.Concat(rpp, "<option value=\"", this.arrRecsPerPage[i], "\" ", (XVar.Pack(this.pageSize == this.arrRecsPerPage[i]) ? XVar.Pack("selected") : XVar.Pack("")), ">", this.arrRecsPerPage[i], "</option>");
					}
					else
					{
						rpp = MVCFunctions.Concat(rpp, "<option value=\"-1\" ", (XVar.Pack(this.pageSize == this.arrRecsPerPage[i]) ? XVar.Pack("selected") : XVar.Pack("")), ">", allMessage, "</option>");
					}
				}
			}
			rpp = MVCFunctions.Concat(rpp, "</select>");
			this.xt.assign(new XVar("grpsPerPage"), (XVar)(rpp));

			return null;
		}
		public virtual XVar doCommonAssignments()
		{
			dynamic allow_export = null, allow_search = null;
			this.xt.assign(new XVar("left_block"), new XVar(true));
			this.assignBody();
			this.setLangParams();
			this.xt.assign(new XVar("searchPanel"), new XVar(true));
			if(XVar.Pack(this.isShowMenu()))
			{
				this.xt.assign(new XVar("menu_block"), new XVar(true));
			}
			if(XVar.Pack(!(XVar)(Security.permissionsAvailable())))
			{
				allow_search = new XVar(true);
				allow_export = new XVar(true);
			}
			else
			{
				allow_search = XVar.Clone(this.permis[this.tName]["search"]);
				allow_export = XVar.Clone(this.permis[this.tName]["export"]);
			}
			this.xt.assign(new XVar("grid_block"), (XVar)(allow_search));
			this.xt.assign(new XVar("toplinks_block"), (XVar)(allow_search));
			this.xt.assign(new XVar("asearch_link"), (XVar)(allow_search));
			this.xt.assign(new XVar("print_link"), (XVar)(allow_export));
			this.xt.assign(new XVar("printlink_attrs"), (XVar)(MVCFunctions.Concat("id=print_", this.id, " href='#'")));
			this.xt.assign(new XVar("printalllink_attrs"), (XVar)(MVCFunctions.Concat("id=printAll_", this.id, " href='#'")));
			this.xt.assign(new XVar("excellink_attrs"), (XVar)(MVCFunctions.Concat("id=export_to_excel", this.id, " href='#'")));
			this.xt.assign(new XVar("wordlink_attrs"), (XVar)(MVCFunctions.Concat("id=export_to_word", this.id, " href='#'")));
			this.xt.assign(new XVar("pdflink_attrs"), (XVar)(MVCFunctions.Concat("id=export_to_pdf", this.id, " href='#'")));
			this.xt.assign(new XVar("advsearchlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"advButton", this.id, "\"")));
			if(XVar.Pack(this.noRecordsFound))
			{
				this.showNoRecordsMessage();
			}
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getPageFields().GetEnumerator())
			{
				dynamic gf = null;
				gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(f.Value)));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_class")), (XVar)(this.fieldClass((XVar)(f.Value))));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_align")), (XVar)(this.fieldAlign((XVar)(f.Value))));
			}
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.xt.assign(new XVar("pdfFonts"), (XVar)(MVCFunctions.runner_json_encode((XVar)(CommonFunctions.getPdfFonts()))));
			}

			return null;
		}
		public override XVar addCommonJs()
		{
			base.addCommonJs();

			return null;
		}
		protected override XVar assignBody()
		{
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage((XVar)(this.isDisplayLoading)));
			if(this.mode == Constants.REPORT_SIMPLE)
			{
				this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<div id=\"search_suggest\" class=\"search_suggest\"></div>");
			}
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
			this.xt.assignbyref(new XVar("body"), (XVar)(this.body));

			return null;
		}
		public virtual XVar beforeShowReport()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowReport"))))
			{
				this.eventsObject.BeforeShowReport((XVar)(this.xt), ref this.templatefile, this);
			}

			return null;
		}
		public virtual XVar showPage()
		{
			this.beforeShowReport();
			if(this.mode == Constants.REPORT_SIMPLE)
			{
				this.display((XVar)(this.templatefile));
				return null;
			}
			this.body.InitAndSetArrayItem("", "begin");
			this.body.InitAndSetArrayItem("", "end");
			this.xt.assign(new XVar("body"), (XVar)(this.body));
			this.xt.assign(new XVar("header"), new XVar(false));
			this.xt.assign(new XVar("footer"), new XVar(false));
			if(this.mode == Constants.REPORT_DASHBOARD)
			{
				dynamic icon = null, returnJSON = XVar.Array();
				this.xt.prepare_template((XVar)(this.templatefile));
				this.addControlsJSAndCSS();
				this.fillSetCntrlMaps();
				returnJSON = XVar.Clone(XVar.Array());
				returnJSON.InitAndSetArrayItem(GlobalVars.pagesData, "pagesData");
				returnJSON.InitAndSetArrayItem(this.jsSettings, "settings");
				returnJSON.InitAndSetArrayItem(this.controlsHTMLMap, "controlsMap");
				returnJSON.InitAndSetArrayItem(this.viewControlsHTMLMap, "viewControlsMap");
				if(XVar.Pack(this.formBricks["footer"]))
				{
					returnJSON.InitAndSetArrayItem(this.fetchBlocksList((XVar)(this.formBricks["footer"]), new XVar(true)), "footerCont");
				}
				if(XVar.Pack(this.formBricks["header"]))
				{
					returnJSON.InitAndSetArrayItem(this.fetchBlocksList((XVar)(this.formBricks["header"]), new XVar(true)), "headerCont");
				}
				returnJSON.InitAndSetArrayItem(MVCFunctions.Concat("<span class=\"rnr-dbebrick\">", this.getPageTitle((XVar)(this.pageName), (XVar)(this.tName)), "</span>", returnJSON["headerCont"]), "headerCont");
				icon = XVar.Clone(CommonFunctions.getIconHTML((XVar)(this.dashElementData["item"]["icon"])));
				if(XVar.Pack(icon))
				{
					returnJSON.InitAndSetArrayItem(icon, "iconHtml");
				}
				this.assignFormFooterAndHeaderBricks(new XVar(false));
				this.xt.prepareContainers();
				returnJSON.InitAndSetArrayItem(this.fetchBlocksList((XVar)(new XVar(0, "above-grid_block", 1, "grid_tabs", 2, "grid_block"))), "html");
				returnJSON.InitAndSetArrayItem(this.flyId, "idStartFrom");
				returnJSON.InitAndSetArrayItem(true, "success");
				returnJSON.InitAndSetArrayItem(this.grabAllJsFiles(), "additionalJS");
				returnJSON.InitAndSetArrayItem(this.grabAllCSSFiles(), "CSSFiles");
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
				return null;
			}
			if((XVar)(this.mode == Constants.REPORT_DETAILS)  || (XVar)(this.mode == Constants.REPORT_DASHDETAILS))
			{
				this.showDpAjax();
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
				return null;
			}
			if((XVar)(this.mode)  && (XVar)(this.mode == "listdetailspopup"))
			{
				dynamic respArr = XVar.Array();
				this.xt.assign(new XVar("container_master"), new XVar(false));
				this.xt.assign(new XVar("cross_controls"), new XVar(false));
				this.xt.prepare_template((XVar)(this.templatefile));
				respArr = XVar.Clone(XVar.Array());
				respArr.InitAndSetArrayItem(true, "success");
				respArr.InitAndSetArrayItem(this.xt.fetch_loaded(new XVar("body")), "body");
				respArr.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("counter")), "counter");
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(respArr)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
				return null;
			}

			return null;
		}
		protected virtual XVar showDpAjax()
		{
			dynamic icon = null, returnJSON = XVar.Array();
			returnJSON = XVar.Clone(XVar.Array());
			if((XVar)((XVar)((XVar)((XVar)(this.mode == Constants.REPORT_DETAILS)  && (XVar)(this.dashTName))  && (XVar)(this.dashElementName))  && (XVar)(!(XVar)(this.shouldDisplayDetailsPage())))  && (XVar)((XVar)(this.masterPageType == Constants.PAGE_EDIT)  || (XVar)(this.masterPageType == Constants.PAGE_VIEW)))
			{
				returnJSON.InitAndSetArrayItem(true, "noData");
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				return null;
			}
			if((XVar)((XVar)(this.mode == Constants.REPORT_DETAILS)  && (XVar)(this.masterPageType == Constants.PAGE_LIST))  && (XVar)(!(XVar)(this.shouldDisplayDetailsPage())))
			{
				returnJSON.InitAndSetArrayItem(false, "success");
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				return null;
			}
			this.xt.unassign(new XVar("cross_controls"));
			this.hideElement(new XVar("printpanel"));
			this.xt.prepare_template((XVar)(this.templatefile));
			this.addControlsJSAndCSS();
			this.fillSetCntrlMaps();
			returnJSON.InitAndSetArrayItem(GlobalVars.pagesData, "pagesData");
			returnJSON.InitAndSetArrayItem(this.jsSettings, "settings");
			returnJSON.InitAndSetArrayItem(this.controlsHTMLMap, "controlsMap");
			returnJSON.InitAndSetArrayItem(this.viewControlsHTMLMap, "viewControlsMap");
			if(XVar.Pack(this.formBricks["footer"]))
			{
				returnJSON.InitAndSetArrayItem(this.fetchBlocksList((XVar)(this.formBricks["footer"]), new XVar(true)), "footerCont");
			}
			if(XVar.Pack(this.formBricks["header"]))
			{
				returnJSON.InitAndSetArrayItem(this.fetchBlocksList((XVar)(this.formBricks["header"]), new XVar(true)), "headerCont");
			}
			if((XVar)(this.mode == Constants.REPORT_DETAILS)  && (XVar)((XVar)((XVar)(this.masterPageType == Constants.PAGE_LIST)  || (XVar)(this.masterPageType == Constants.PAGE_REPORT))  || (XVar)(this.masterPageType == Constants.PAGE_CHART)))
			{
				returnJSON.InitAndSetArrayItem(returnJSON["headerCont"], "headerCont");
			}
			else
			{
				if(this.mode == Constants.REPORT_DASHDETAILS)
				{
					returnJSON.InitAndSetArrayItem(MVCFunctions.Concat("<span class=\"rnr-dbebrick\">", this.getPageTitle((XVar)(this.pageName), (XVar)(this.tName)), "</span>", returnJSON["headerCont"]), "headerCont");
				}
			}
			icon = XVar.Clone(CommonFunctions.getIconHTML((XVar)(this.dashElementData["item"]["icon"])));
			if(XVar.Pack(icon))
			{
				returnJSON.InitAndSetArrayItem(icon, "iconHtml");
			}
			this.assignFormFooterAndHeaderBricks(new XVar(false));
			this.xt.prepareContainers();
			returnJSON.InitAndSetArrayItem(MVCFunctions.Concat(this.xt.fetch_loaded(new XVar("grid_tabs")), this.xt.fetch_loaded(new XVar("grid_block")), this.xt.fetch_loaded(new XVar("pagination_block"))), "html");
			returnJSON.InitAndSetArrayItem(true, "success");
			returnJSON.InitAndSetArrayItem(this.id, "id");
			returnJSON.InitAndSetArrayItem(this.flyId, "idStartFrom");
			returnJSON.InitAndSetArrayItem(this.grabAllJsFiles(), "additionalJS");
			returnJSON.InitAndSetArrayItem(this.grabAllCSSFiles(), "additionalCSS");
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));

			return null;
		}
		public override XVar printAvailable()
		{
			if((XVar)((XVar)(this.mode == Constants.REPORT_DASHBOARD)  || (XVar)(this.mode == Constants.REPORT_DETAILS))  || (XVar)(this.mode == Constants.REPORT_DASHDETAILS))
			{
				return false;
			}
			return base.printAvailable();
		}
		public override XVar showPageDp(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			if(XVar.Pack(this.crossTable))
			{
				this.xt.unassign(new XVar("cross_controls"));
			}
			base.showPageDp((XVar)(var_params));

			return null;
		}
		public override XVar prepareDisplayDetails()
		{
			this.prepareDisplayDetailsPD();

			return null;
		}
		public virtual XVar prepareDisplayDetailsPD()
		{
			dynamic bodyContents = null, forms = null;
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.xt.assign(new XVar("embedded_grid"), new XVar(true));
				this.xt.assign(new XVar("embedded_page_title"), new XVar(true));
				this.xt.load_templateJSON((XVar)(this.templatefile));
				this.renderedBody = XVar.Clone(this.xt.fetch_loadedJSON(new XVar("body")));
				return null;
			}
			forms = XVar.Clone(new XVar(0, "grid"));
			bodyContents = XVar.Clone(this.fetchForms((XVar)(forms)));
			this.renderedBody = XVar.Clone(MVCFunctions.Concat("<div id=\"detailPreview", this.id, "\">", bodyContents, "</div>"));

			return null;
		}
		public override XVar showGridOnly()
		{
			MVCFunctions.Echo(this.renderedBody);

			return null;
		}
		public override XVar shouldDisplayDetailsPage()
		{
			if(XVar.Pack(!(XVar)(this.permis[this.tName]["search"])))
			{
				return false;
			}
			if((XVar)(this.noRecordsFound)  && (XVar)(0 == this.getGridTabsCount()))
			{
				return false;
			}
			return true;
		}
		protected virtual XVar getFieldClass(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic var_class = null;
			var_class = XVar.Clone(base.fieldClass((XVar)(field)));
			if(XVar.Pack(this.crossTable))
			{
				if(XVar.Pack(MVCFunctions.in_array((XVar)(this.pSet.getViewFormat((XVar)(field))), (XVar)(new XVar(0, Constants.FORMAT_DATE_SHORT, 1, Constants.FORMAT_DATE_LONG, 2, Constants.FORMAT_DATE_TIME)))))
				{
					return MVCFunctions.Concat(var_class, " rnr-field-crossdate");
				}
			}
			return var_class;
		}
		public static XVar readReportModeFromRequest()
		{
			dynamic mode = null, pageMode = null;
			mode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(mode == "listdetails")
			{
				pageMode = new XVar(Constants.REPORT_DETAILS);
			}
			else
			{
				if(mode == "listdetailspopup")
				{
					pageMode = new XVar(Constants.REPORT_POPUPDETAILS);
				}
				else
				{
					if(mode == "dashreport")
					{
						pageMode = new XVar(Constants.REPORT_DASHBOARD);
					}
					else
					{
						if(mode == "dashdetails")
						{
							pageMode = new XVar(Constants.REPORT_DASHDETAILS);
						}
						else
						{
							pageMode = new XVar(Constants.REPORT_SIMPLE);
						}
					}
				}
			}
			return pageMode;
		}
		public override XVar gridTabsAvailable()
		{
			return true;
		}
		public override XVar displayTabsInPage()
		{
			return (XVar)((XVar)(this.simpleMode())  || (XVar)((XVar)(this.mode == Constants.REPORT_DETAILS)  && (XVar)((XVar)(this.masterPageType == Constants.PAGE_VIEW)  || (XVar)(this.masterPageType == Constants.PAGE_EDIT))))  || (XVar)((XVar)(this.mode == Constants.REPORT_DASHBOARD)  && (XVar)(this.dashElementData["tabLocation"] == "body"));
		}
		public override XVar renderPageBody()
		{
			dynamic blocks = null;
			blocks = XVar.Clone(new XVar(0, "grid_tabs", 1, "message", 2, "grid_block", 3, "pagination_block"));
			return this.fetchBlocksList((XVar)(blocks), new XVar(false));
		}
		protected virtual XVar assignColumnHeaderClasses()
		{
			dynamic reportFields = XVar.Array();
			reportFields = XVar.Clone(this.pSet.getFieldsList());
			foreach (KeyValuePair<XVar, dynamic> field in reportFields.GetEnumerator())
			{
				dynamic goodName = null;
				goodName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(field.Value)));
				this.xt.assign((XVar)(MVCFunctions.Concat("fieldclass_", goodName)), (XVar)(this.getFieldClass((XVar)(field.Value))));
				this.xt.assign((XVar)(MVCFunctions.Concat(goodName, "_align")), (XVar)(this.fieldAlign((XVar)(field.Value))));
			}

			return null;
		}
		public override XVar pdfJsonMode()
		{
			return this.pdfJson;
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
			dc = XVar.Clone(base.getSubsetDataCommand());
			if((XVar)((XVar)(this.mode != Constants.REPORT_DETAILS)  && (XVar)(this.pSet.noRecordsOnFirstPage()))  && (XVar)(!(XVar)(this.isSearchFunctionalityActivated())))
			{
				dc.filter = XVar.Clone(DataCondition._False());
			}
			return dc;
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
			settings.InitAndSetArrayItem(this.crossTable, "crossTable");
			settings.InitAndSetArrayItem(this.searchClauseObj.simpleSearchActive, "simpleSearchActive");
			if((XVar)(this.crossTableObj)  && (XVar)(this.crossTableObj.isEmpty()))
			{
				settings.InitAndSetArrayItem(this.getCurrentCrossParams(), "crossParams");
			}
			return settings;
		}
	}
}
