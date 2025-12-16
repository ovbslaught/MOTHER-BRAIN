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
	public partial class GanttPage : ChartPage
	{
		public dynamic action = XVar.Pack("");
		protected static bool skipGanttPageCtor = false;
		public GanttPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipGanttPageCtor)
			{
				skipGanttPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

		}
		public override XVar process()
		{
			if((XVar)(this.mode == Constants.GANTT_DASHDETAILS)  || (XVar)((XVar)(this.mode == Constants.GANTT_DETAILS)  && (XVar)((XVar)(this.masterPageType == Constants.PAGE_LIST)  || (XVar)(this.masterPageType == Constants.PAGE_REPORT))))
			{
				this.updateDetailsTabTitles();
			}
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessGantt"))))
			{
				this.eventsObject.BeforeProcessGantt(this);
			}
			this.processGridTabs();
			this.doCommonAssignments();
			this.addButtonHandlers();
			this.addCommonJs();
			this.commonAssign();
			if((XVar)(this.pSet.noRecordsOnFirstPage())  && (XVar)(!(XVar)(this.isSearchFunctionalityActivated())))
			{
				this.showNoRecordsMessage();
			}
			else
			{
				this.prepareQueryCommand();
				this.prepareGanttData();
			}
			if(this.mode != Constants.GANTT_DASHBOARD)
			{
				this.buildSearchPanel();
				this.assignSimpleSearch();
			}
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_advsearch")] = MVCFunctions.serialize((XVar)(this.searchClauseObj));
			this.displayMasterTableInfo();
			this.showPage();

			return null;
		}
		public override XVar commonAssign()
		{
			base.commonAssign();
			if((XVar)(this.addAvailable())  && (XVar)((XVar)(!(XVar)(this.pSet.noRecordsOnFirstPage()))  || (XVar)(this.isSearchFunctionalityActivated())))
			{
				dynamic add_attrs = null;
				this.xt.assign(new XVar("add_link"), new XVar(true));
				add_attrs = XVar.Clone(MVCFunctions.Concat("id=\"addButton", this.id, "\""));
				this.xt.assign(new XVar("addlink_attrs"), (XVar)(add_attrs));
				if(XVar.Pack(this.dashTName))
				{
					this.xt.assign(new XVar("addlink_getparams"), (XVar)(MVCFunctions.Concat("?fromDashboard=", this.dashTName)));
				}
			}

			return null;
		}
		protected virtual XVar prepareGanttData()
		{
			dynamic ganttData = XVar.Array(), langData = XVar.Array();
			ganttData = XVar.Clone(new XVar("tasks", this.prepareTaskData()));
			ganttData.InitAndSetArrayItem(this.viewAvailable(), "canView");
			ganttData.InitAndSetArrayItem(this.addChildAvailable(), "canAddChild");
			ganttData.InitAndSetArrayItem(this.editAvailable(), "canEdit");
			ganttData.InitAndSetArrayItem((XVar)(this.editAvailable())  && (XVar)(this.pSet.getGanttValue(new XVar("progressField"))), "canEditProgress");
			langData = XVar.Clone(GlobalVars.languages_data[CommonFunctions.mlang_getcurrentlang()]);
			if(XVar.Pack(langData))
			{
				ganttData.InitAndSetArrayItem(langData["languageCode"], "language");
			}
			this.pageData.InitAndSetArrayItem(ganttData, "ganttData");

			return null;
		}
		protected virtual XVar prepareTaskData()
		{
			dynamic data = null, recordLimit = null, rs = null, task = null, tasks = XVar.Array();
			rs = XVar.Clone(this.dataSource.getList((XVar)(this.queryCommand)));
			tasks = XVar.Clone(XVar.Array());
			recordLimit = XVar.Clone(this.pSet.getRecordsLimit());
			while((XVar)(!(XVar)(recordLimit))  || (XVar)(MVCFunctions.count(tasks) < recordLimit))
			{
				data = XVar.Clone(rs.fetchAssoc());
				if(XVar.Pack(!(XVar)(data)))
				{
					break;
				}
				task = XVar.Clone(GanttPage.makeTask((XVar)(data), this));
				tasks.InitAndSetArrayItem(task, null);
			}
			return tasks;
		}
		public static XVar makeTask(dynamic _param_data, dynamic _param_pageObject)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			dynamic categoryColors = XVar.Array(), categoryField = null, container = null, endDateField = null, endDateParts = null, keyFields = XVar.Array(), keyParams = XVar.Array(), keyValues = XVar.Array(), keylink = null, nameControl = null, nameField = null, parentField = null, progressField = null, startDateField = null, startDateParts = null, task = XVar.Array();
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(pageObject.tName), new XVar(Constants.PAGE_GANTT), (XVar)(pageObject.pageName)));
			startDateField = XVar.Clone(pSet.getGanttValue(new XVar("startDateField")));
			endDateField = XVar.Clone(pSet.getGanttValue(new XVar("endDateField")));
			nameField = XVar.Clone(pSet.getGanttValue(new XVar("nameField")));
			parentField = XVar.Clone(pSet.getGanttValue(new XVar("parentField")));
			progressField = XVar.Clone(pSet.getGanttValue(new XVar("progressField")));
			categoryField = XVar.Clone(pSet.getGanttValue(new XVar("categoryField")));
			categoryColors = XVar.Clone(pSet.getGanttValue(new XVar("categoryColors")));
			container = XVar.Clone(new ViewControlsContainer((XVar)(pSet), new XVar(Constants.PAGE_GANTT), new XVar(null)));
			nameControl = XVar.Clone(container.getControl((XVar)(nameField)));
			keyFields = XVar.Clone(pSet.getTableKeys());
			if(MVCFunctions.count(keyFields) != 1)
			{
				parentField = new XVar("");
			}
			keyValues = XVar.Clone(XVar.Array());
			keyParams = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> k in keyFields.GetEnumerator())
			{
				keyValues.InitAndSetArrayItem(data[k.Value], null);
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("key", k.Key + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(data[k.Value]))))), null);
			}
			keylink = XVar.Clone(MVCFunctions.implode(new XVar("&"), (XVar)(keyParams)));
			task = XVar.Clone(XVar.Array());
			task.InitAndSetArrayItem(MVCFunctions.implode(new XVar("~"), (XVar)(keyValues)), "id");
			task.InitAndSetArrayItem(keyValues, "keys");
			startDateParts = XVar.Clone(MVCFunctions.db2time((XVar)(data[startDateField])));
			endDateParts = XVar.Clone(MVCFunctions.db2time((XVar)(data[endDateField])));
			if((XVar)(!(XVar)(data[endDateField]))  || (XVar)(0 < CommonFunctions.comparedates((XVar)(startDateParts), (XVar)(endDateParts))))
			{
				endDateParts = XVar.Clone(startDateParts);
			}
			task.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(startDateParts), new XVar("yyyy-MM-dd")), "start");
			task.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(endDateParts), new XVar("yyyy-MM-dd 23:59")), "end");
			task.InitAndSetArrayItem(nameControl.getTextValue((XVar)(data)), "name");
			if((XVar)(parentField)  && (XVar)(data[parentField]))
			{
				task.InitAndSetArrayItem(new XVar(0, XVar.Pack(data[parentField]).ToString()), "dependencies");
			}
			if(XVar.Pack(progressField))
			{
				task.InitAndSetArrayItem(data[progressField], "progress");
			}
			if(XVar.Pack(categoryField))
			{
				dynamic category = null;
				category = XVar.Clone(data[categoryField]);
				foreach (KeyValuePair<XVar, dynamic> cc in categoryColors.GetEnumerator())
				{
					if(cc.Value["value"] == category)
					{
						task.InitAndSetArrayItem(MVCFunctions.Concat("#", cc.Value["color"]), "color");
						task.InitAndSetArrayItem((XVar.Pack(CommonFunctions.fgColorBlack((XVar)(cc.Value["color"]))) ? XVar.Pack("#000000") : XVar.Pack("#ffffff")), "color_text");
					}
				}
			}
			task.InitAndSetArrayItem((XVar)(pageObject.editAvailable())  && (XVar)(pageObject.recordEditable((XVar)(data), new XVar("E"))), "canEdit");
			return task;
		}
		public virtual XVar prepareQueryCommand()
		{
			dynamic prep = XVar.Array();
			this.queryCommand = XVar.Clone(this.getSubsetDataCommand());
			prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(this.queryCommand)));
			this.querySQL = XVar.Clone(prep["sql"]);
			this.callBeforeQueryEvent((XVar)(this.queryCommand));
			if(XVar.Pack(!(XVar)(this.pSet.hideNumberOfRecords())))
			{
				dynamic recordLimit = null;
				this.numRowsFromSQL = XVar.Clone(this.dataSource.getCount((XVar)(this.queryCommand)));
				recordLimit = XVar.Clone(this.pSet.getRecordsLimit());
				if((XVar)(recordLimit)  && (XVar)(recordLimit < this.numRowsFromSQL))
				{
					this.numRowsFromSQL = XVar.Clone(recordLimit);
				}
			}

			return null;
		}
		public override XVar callBeforeQueryEvent(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic order = null, prep = XVar.Array(), sql = null, where = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("BeforeQueryGantt")))))
			{
				return null;
			}
			prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
			where = XVar.Clone(prep["where"]);
			order = XVar.Clone(prep["order"]);
			sql = XVar.Clone(prep["sql"]);
			this.eventsObject.BeforeQueryGantt((XVar)(sql), (XVar)(where), (XVar)(order), this);
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
		public override XVar beforeShowEvent()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowGantt"))))
			{
				this.eventsObject.BeforeShowGantt((XVar)(this.xt), (XVar)(this.templatefile), this);
			}

			return null;
		}
		public static XVar readGanttModeFromRequest()
		{
			dynamic mode = null;
			mode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(mode == "listdetails")
			{
				return Constants.GANTT_DETAILS;
			}
			else
			{
				if(mode == "dashgantt")
				{
					return Constants.GANTT_DASHBOARD;
				}
				else
				{
					return Constants.GANTT_SIMPLE;
				}
			}

			return null;
		}
		public override XVar addCommonJs()
		{
			base.addCommonJs();
			this.AddJSFile(new XVar("include/gantt/frappe-gantt.umd.js"));
			this.AddCSSFile(new XVar("include/gantt/frappe-gantt.css"));

			return null;
		}
		protected virtual XVar getRecordset()
		{
			dynamic rs = null;
			rs = XVar.Clone(this.dataSource.getList((XVar)(this.queryCommand)));

			return null;
		}
		public override XVar deleteAvailable()
		{
			return (XVar)(MVCFunctions.count(this.pSet.getTableKeys()))  && (XVar)(this.permis[this.tName]["delete"]);
		}
		protected virtual XVar addChildAvailable()
		{
			return (XVar)((XVar)(this.addAvailable())  && (XVar)(MVCFunctions.count(this.pSet.getTableKeys()) == 1))  && (XVar)(this.pSet.getGanttValue(new XVar("parentField")));
		}
	}
}
