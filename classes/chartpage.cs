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
	public partial class ChartPage : RunnerPage
	{
		public dynamic show_message_block = XVar.Pack(false);
		protected static bool skipChartPageCtor = false;
		public ChartPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipChartPageCtor)
			{
				skipChartPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

			this.bodyForms = XVar.Clone(new XVar(0, "grid"));
			if(this.mode == Constants.CHART_DASHBOARD)
			{
				this.pageData.InitAndSetArrayItem(this.getStartMasterKeys(), "detailsMasterKeys");
			}
			this.pageData.InitAndSetArrayItem(this.pSet.getChartCount() == 1, "singleChartPage");
		}
		protected override XVar assignSessionPrefix()
		{
			if(this.mode == Constants.CHART_DASHBOARD)
			{
				this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.dashTName, "_", this.tName));
			}
			else
			{
				this.sessionPrefix = XVar.Clone(this.tName);
			}

			return null;
		}
		public virtual XVar process()
		{
			if((XVar)(this.mode == Constants.CHART_DASHDETAILS)  || (XVar)((XVar)(this.mode == Constants.CHART_DETAILS)  && (XVar)((XVar)(this.masterPageType == Constants.PAGE_LIST)  || (XVar)(this.masterPageType == Constants.PAGE_REPORT))))
			{
				this.updateDetailsTabTitles();
			}
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessChart"))))
			{
				this.eventsObject.BeforeProcessChart(this);
			}
			this.processGridTabs();
			this.doCommonAssignments();
			this.addButtonHandlers();
			this.addCommonJs();
			this.commonAssign();
			if(this.mode != Constants.CHART_DASHBOARD)
			{
				this.buildSearchPanel();
				this.assignSimpleSearch();
			}
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_advsearch")] = MVCFunctions.serialize((XVar)(this.searchClauseObj));
			this.displayMasterTableInfo();
			this.showPage();

			return null;
		}
		public override XVar callBeforeQueryEvent(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic order = null, prep = XVar.Array(), sql = null, where = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("BeforeQueryChart")))))
			{
				return null;
			}
			prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
			where = XVar.Clone(prep["where"]);
			order = XVar.Clone(prep["order"]);
			sql = XVar.Clone(prep["sql"]);
			this.eventsObject.BeforeQueryChart((XVar)(sql), ref where, ref order);
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
				if(order != prep["order"])
				{
					this.dataSource.overrideOrder((XVar)(dc), (XVar)(order));
				}
			}

			return null;
		}
		public override XVar getMasterCondition()
		{
			if(this.mode == Constants.CHART_DASHBOARD)
			{
				return null;
			}
			return base.getMasterCondition();
		}
		public virtual XVar getStartMasterKeys()
		{
			dynamic data = XVar.Array(), dc = null, detailsTables = XVar.Array(), masterKeysArr = XVar.Array(), rs = null;
			detailsTables = XVar.Clone(this.pSet.getDetailsTables());
			if(MVCFunctions.count(detailsTables) == 0)
			{
				return XVar.Array();
			}
			dc = XVar.Clone(this.getSubsetDataCommand());
			dc.reccount = new XVar(1);
			rs = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				MVCFunctions.showError((XVar)(this.dataSource.lastError()));
			}
			data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc())));
			masterKeysArr = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> details in detailsTables.GetEnumerator())
			{
				dynamic detailsKeys = XVar.Array();
				detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(details.Value)));
				foreach (KeyValuePair<XVar, dynamic> mk in detailsKeys["masterKeys"].GetEnumerator())
				{
					masterKeysArr.InitAndSetArrayItem(new XVar(MVCFunctions.Concat("masterkey", mk.Key + 1), data[mk.Value]), details.Value);
				}
			}
			return masterKeysArr;
		}
		public virtual XVar doCommonAssignments()
		{
			this.xt.assign(new XVar("searchPanel"), new XVar(true));
			if(XVar.Pack(this.isShowMenu()))
			{
				this.xt.assign(new XVar("menu_block"), new XVar(true));
			}
			this.setLangParams();
			this.xt.assign(new XVar("chart_block"), new XVar(true));
			this.xt.assign(new XVar("asearch_link"), new XVar(true));
			this.xt.assign(new XVar("exportpdflink_attrs"), new XVar("onclick='chart.saveAsPDF();'"));
			this.xt.assign(new XVar("advsearchlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"advButton", this.id, "\"")));
			this.xt.assign(new XVar("message_block"), new XVar(true));
			if((XVar)((XVar)((XVar)(this.mode == Constants.CHART_SIMPLE)  || (XVar)(this.mode == Constants.CHART_DASHBOARD))  && (XVar)(this.pSet.noRecordsOnFirstPage()))  && (XVar)(!(XVar)(this.searchClauseObj.isSearchFunctionalityActivated())))
			{
				this.show_message_block = new XVar(true);
				this.hideElement(new XVar("chart"));
				this.xt.assign(new XVar("chart_block"), new XVar(false));
				this.xt.assign(new XVar("message"), (XVar)(this.noRecordsMessage()));
				this.xt.assign(new XVar("message_class"), new XVar("alert-warning"));
			}
			if(XVar.Pack(!(XVar)(this.show_message_block)))
			{
				this.hideElement(new XVar("message"));
			}
			this.assignChartElement();
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage((XVar)(this.isDisplayLoading)));
			if(XVar.Pack(!(XVar)(this.isDashboardElement())))
			{
				this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<div id=\"search_suggest\" class=\"search_suggest\"></div>");
			}
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
			this.xt.assignbyref(new XVar("body"), (XVar)(this.body));

			return null;
		}
		public virtual XVar assignChartElement()
		{

			return null;
		}
		public virtual XVar prepareDetailsForEditViewPage()
		{
			this.addButtonHandlers();
			this.AddJSFile(new XVar(MVCFunctions.Concat("usercode/pageevents_", Constants.GLOBAL_PAGES_SHORT, ".js")));
			this.xt.assign(new XVar("body"), (XVar)(this.body));
			this.xt.assign(new XVar("chart_block"), new XVar(true));
			this.xt.assign(new XVar("message_block"), new XVar(true));

			return null;
		}
		protected override XVar getExtraAjaxPageParams()
		{
			dynamic returnJSON = XVar.Array();
			returnJSON = XVar.Clone(XVar.Array());
			if(this.mode == Constants.REPORT_DETAILS)
			{
				returnJSON.InitAndSetArrayItem(returnJSON["headerCont"], "headerCont");
			}
			return returnJSON;
		}
		public virtual XVar beforeShowEvent()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowChart"))))
			{
				this.eventsObject.BeforeShowChart((XVar)(this.xt), ref this.templatefile, this);
			}

			return null;
		}
		public virtual XVar showPage()
		{
			this.beforeShowEvent();
			if((XVar)((XVar)(this.mode == Constants.CHART_DETAILS)  || (XVar)(this.mode == Constants.CHART_DASHBOARD))  || (XVar)(this.mode == Constants.CHART_DASHDETAILS))
			{
				this.addControlsJSAndCSS();
				this.fillSetCntrlMaps();
				this.xt.assign(new XVar("header"), new XVar(false));
				this.xt.assign(new XVar("footer"), new XVar(false));
				this.body.InitAndSetArrayItem("", "begin");
				this.body.InitAndSetArrayItem("", "end");
				this.xt.assign(new XVar("body"), (XVar)(this.body));
				this.displayAJAX((XVar)(this.templatefile), (XVar)(this.id + 1));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			if(this.mode == Constants.CHART_POPUPDETAILS)
			{
				dynamic respArr = XVar.Array();
				this.xt.assign(new XVar("header"), new XVar(false));
				this.xt.assign(new XVar("footer"), new XVar(false));
				this.body.InitAndSetArrayItem("", "begin");
				this.body.InitAndSetArrayItem("", "end");
				this.xt.prepare_template((XVar)(this.templatefile));
				respArr = XVar.Clone(XVar.Array());
				respArr.InitAndSetArrayItem(true, "success");
				respArr.InitAndSetArrayItem(this.xt.fetch_loaded(new XVar("body")), "body");
				respArr.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("counter")), "counter");
				this.xt.assign(new XVar("container_master"), new XVar(false));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(respArr)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			this.display((XVar)(this.templatefile));

			return null;
		}
		public override XVar processGridTabs()
		{
			dynamic ctChanged = null;
			ctChanged = XVar.Clone(base.processGridTabs());
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_chartTabWhere")] = this.getCurrentTabWhere();
			return ctChanged;
		}
		public override XVar gridTabsAvailable()
		{
			return true;
		}
		public override XVar displayTabsInPage()
		{
			return (XVar)((XVar)(this.simpleMode())  || (XVar)((XVar)(this.mode == Constants.CHART_DETAILS)  && (XVar)((XVar)(this.masterPageType == Constants.PAGE_VIEW)  || (XVar)(this.masterPageType == Constants.PAGE_EDIT))))  || (XVar)((XVar)(this.mode == Constants.CHART_DASHBOARD)  && (XVar)(this.dashElementData["tabLocation"] == "body"));
		}
		protected override XVar getBodyMarkup(dynamic _param_templatefile)
		{
			#region pass-by-value parameters
			dynamic templatefile = XVar.Clone(_param_templatefile);
			#endregion

			if((XVar)(this.mode == Constants.CHART_DASHBOARD)  && (XVar)(this.dashElementData["tabLocation"] == "body"))
			{
				return this.fetchBlocksList((XVar)(new XVar(0, "above-grid_block", 1, "grid_tabs", 2, "grid_block")));
			}
			return base.getBodyMarkup((XVar)(templatefile));
		}
		public override XVar element2Item(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(name == "message")
			{
				return new XVar(0, "grid_message");
			}
			if(name == "chart")
			{
				return new XVar(0, "chart");
			}
			return base.element2Item((XVar)(name));
		}
		public override XVar prepareDisplayDetails()
		{
			dynamic bodyContents = null, chartXtParams = XVar.Array(), forms = null, resizeChart = null;
			resizeChart = new XVar(true);
			if((XVar)((XVar)(this.mode == Constants.CHART_SIMPLE)  || (XVar)(this.mode == Constants.CHART_DASHBOARD))  || (XVar)((XVar)(this.mode == Constants.CHART_DETAILS)  && (XVar)((XVar)(this.masterPageType == Constants.PAGE_VIEW)  || (XVar)(this.masterPageType == Constants.PAGE_EDIT))))
			{
				resizeChart = new XVar(false);
			}
			chartXtParams = XVar.Clone(new XVar("id", this.id, "table", this.tName, "ctype", this.pSet.getChartType(), "resize", resizeChart, "chartName", this.shortTableName, "chartPreview", (XVar)(!XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.CHART_SIMPLE)))  && (XVar)(this.mode != Constants.CHART_DASHBOARD)));
			if((XVar)(this.mode == Constants.CHART_DASHBOARD)  || (XVar)(this.mode == Constants.CHART_DASHDETAILS))
			{
				chartXtParams.InitAndSetArrayItem(this.dashElementData["reload"], "refreshTime");
			}
			this.prepareCharts();
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
		public override XVar prepareCharts()
		{
			dynamic chartXtParams = XVar.Array();
			chartXtParams = XVar.Clone(new XVar("id", this.id, "chartPreview", (XVar)(!XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.CHART_SIMPLE)))  && (XVar)(this.mode != Constants.CHART_DASHBOARD), "stateLink", this.getStateUrlParams(), "stateParams", this.getStateParams()));
			if((XVar)(this.dashTName)  && (XVar)(this.mode == Constants.CHART_DASHBOARD))
			{
				chartXtParams.InitAndSetArrayItem(true, "dash");
				chartXtParams.InitAndSetArrayItem(this.dashTName, "dashTName");
				chartXtParams.InitAndSetArrayItem(this.dashElementName, "dashElementName");
				chartXtParams.InitAndSetArrayItem(this.dashPage, "dashPage");
			}
			this.xt.assign_function(new XVar("chart"), new XVar("xt_showpdchart"), (XVar)(chartXtParams));

			return null;
		}
		public static XVar readChartModeFromRequest()
		{
			dynamic mode = null;
			mode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(mode == "listdetails")
			{
				return Constants.CHART_DETAILS;
			}
			else
			{
				if(mode == "listdetailspopup")
				{
					return Constants.CHART_POPUPDETAILS;
				}
				else
				{
					if(mode == "dashchart")
					{
						return Constants.CHART_DASHBOARD;
					}
					else
					{
						if(mode == "dashdetails")
						{
							return Constants.CHART_DASHDETAILS;
						}
						else
						{
							return Constants.CHART_SIMPLE;
						}
					}
				}
			}

			return null;
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
			settings.InitAndSetArrayItem(this.searchClauseObj.simpleSearchActive, "simpleSearchActive");
			return settings;
		}
	}
}
