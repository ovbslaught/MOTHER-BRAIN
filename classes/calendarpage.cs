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
	public partial class CalendarPage : ChartPage
	{
		public dynamic action = XVar.Pack("");
		public dynamic availableViews = XVar.Array();
		protected static bool skipCalendarPageCtor = false;
		public CalendarPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipCalendarPageCtor)
			{
				skipCalendarPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

		}
		public override XVar process()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessCalendar"))))
			{
				this.eventsObject.BeforeProcessCalendar(this);
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
				this.prepareCalendarData();
			}
			if(this.mode != Constants.CALENDAR_DASHBOARD)
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
			if(XVar.Pack(this.exportAvailable()))
			{
				this.xt.assign(new XVar("export_link"), new XVar(true));
				this.xt.assign(new XVar("exportlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"export_", this.id, "\"")));
			}
			if(XVar.Pack(this.importAvailable()))
			{
				this.xt.assign(new XVar("import_link"), new XVar(true));
				this.xt.assign(new XVar("importlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"import_", this.id, "\" name=\"import_", this.id, "\"")));
			}

			return null;
		}
		protected virtual XVar prepareEventData()
		{
			dynamic calendarEvents = XVar.Array(), data = null, recordLimit = null, rs = null, var_event = null;
			rs = XVar.Clone(this.dataSource.getList((XVar)(this.queryCommand)));
			calendarEvents = XVar.Clone(XVar.Array());
			recordLimit = XVar.Clone(this.pSet.getRecordsLimit());
			while((XVar)(!(XVar)(recordLimit))  || (XVar)(MVCFunctions.count(calendarEvents) < recordLimit))
			{
				data = XVar.Clone(rs.fetchAssoc());
				if(XVar.Pack(!(XVar)(data)))
				{
					break;
				}
				var_event = XVar.Clone(CalendarPage.makeEvent((XVar)(data), this));
				calendarEvents.InitAndSetArrayItem(var_event, null);
			}
			return calendarEvents;
		}
		public static XVar makeEvent(dynamic _param_data, dynamic _param_pageObject)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			dynamic categoryColors = XVar.Array(), categoryField = null, container = null, dateField = null, descriptionField = null, editable = null, endDate = null, endDateField = null, endTime = null, endTimeField = null, endTimeFieldType = null, fullDayField = null, keyFields = XVar.Array(), keyValues = XVar.Array(), startDate = null, startTime = null, subjectControl = null, subjectField = null, timeField = null, timeFieldType = null, var_event = XVar.Array();
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(pageObject.tName), new XVar(Constants.PAGE_CALENDAR), (XVar)(pageObject.pageName)));
			dateField = XVar.Clone(pSet.getCalendarValue(new XVar("dateField")));
			subjectField = XVar.Clone(pSet.getCalendarValue(new XVar("subjectField")));
			descriptionField = XVar.Clone(pSet.getCalendarValue(new XVar("descriptionField")));
			categoryField = XVar.Clone(pSet.getCalendarValue(new XVar("categoryField")));
			fullDayField = XVar.Clone(pSet.getCalendarValue(new XVar("fullDayField")));
			timeField = XVar.Clone(pSet.getCalendarValue(new XVar("timeField")));
			endDateField = XVar.Clone(pSet.getCalendarValue(new XVar("endDateField")));
			endTimeField = XVar.Clone(pSet.getCalendarValue(new XVar("endTimeField")));
			categoryColors = XVar.Clone(pSet.getCalendarValue(new XVar("categoryColors")));
			container = XVar.Clone(new ViewControlsContainer((XVar)(pSet), new XVar(Constants.PAGE_CALENDAR), new XVar(null)));
			subjectControl = XVar.Clone(container.getControl((XVar)(subjectField)));
			timeFieldType = XVar.Clone(pSet.getFieldType((XVar)(timeField)));
			endTimeFieldType = XVar.Clone(pSet.getFieldType((XVar)(endTimeField)));
			keyFields = XVar.Clone(pSet.getTableKeys());
			keyValues = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> k in keyFields.GetEnumerator())
			{
				keyValues.InitAndSetArrayItem(data[k.Value], null);
			}
			var_event = XVar.Clone(XVar.Array());
			var_event.InitAndSetArrayItem(keyValues, "keys");
			var_event.InitAndSetArrayItem(MVCFunctions.implode(new XVar("~"), (XVar)(keyValues)), "id");
			if(XVar.Pack(fullDayField))
			{
				var_event.InitAndSetArrayItem(CheckboxField.checkedValue((XVar)(data[fullDayField])), "allDay");
			}
			if((XVar)(!(XVar)(timeField))  || (XVar)(!(XVar)(data[timeField])))
			{
				var_event.InitAndSetArrayItem(true, "allDay");
			}
			startDate = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(data[dateField]))), new XVar("yyyy-MM-dd")));
			startTime = XVar.Clone((XVar.Pack(timeField) ? XVar.Pack(CalendarPage.formatTime((XVar)(timeFieldType), (XVar)(data[timeField]))) : XVar.Pack("")));
			endDate = XVar.Clone((XVar.Pack((XVar)(endDateField)  && (XVar)(data[endDateField])) ? XVar.Pack(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(data[endDateField]))), new XVar("yyyy-MM-dd"))) : XVar.Pack(startDate)));
			endTime = XVar.Clone((XVar.Pack(endTimeField) ? XVar.Pack(CalendarPage.formatTime((XVar)(endTimeFieldType), (XVar)(data[endTimeField]))) : XVar.Pack(startTime)));
			if((XVar)(var_event["allDay"])  || (XVar)(!(XVar)(startTime)))
			{
				var_event.InitAndSetArrayItem(startDate, "start");
			}
			else
			{
				var_event.InitAndSetArrayItem(MVCFunctions.Concat(startDate, "T", startTime), "start");
			}
			if((XVar)(var_event["allDay"])  || (XVar)(!(XVar)(endTime)))
			{
				var_event.InitAndSetArrayItem(endDate, "end");
			}
			else
			{
				var_event.InitAndSetArrayItem(MVCFunctions.Concat(endDate, "T", endTime), "end");
			}
			var_event.InitAndSetArrayItem(subjectControl.getTextValue((XVar)(data)), "title");
			if(XVar.Pack(categoryField))
			{
				dynamic category = null;
				category = XVar.Clone(data[categoryField]);
				foreach (KeyValuePair<XVar, dynamic> cc in categoryColors.GetEnumerator())
				{
					if(cc.Value["value"] == category)
					{
						var_event.InitAndSetArrayItem(MVCFunctions.Concat("#", cc.Value["color"]), "backgroundColor");
						var_event.InitAndSetArrayItem((XVar.Pack(CommonFunctions.fgColorBlack((XVar)(cc.Value["color"]))) ? XVar.Pack("#000000") : XVar.Pack("#ffffff")), "textColor");
					}
				}
			}
			editable = XVar.Clone((XVar)(pageObject.editAvailable())  && (XVar)(pageObject.recordEditable((XVar)(data), new XVar("E"))));
			var_event.InitAndSetArrayItem(editable, "startEditable");
			var_event.InitAndSetArrayItem(editable, "durationEditable");
			return var_event;
		}
		public static XVar formatTime(dynamic _param_fieldType, dynamic _param_fieldValue)
		{
			#region pass-by-value parameters
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic fieldValue = XVar.Clone(_param_fieldValue);
			#endregion

			dynamic arr = XVar.Array();
			arr = XVar.Clone(ViewTimeField.splitStringValue((XVar)(fieldType), (XVar)(fieldValue)));
			if(XVar.Pack(!(XVar)(arr)))
			{
				return "";
			}
			return CommonFunctions.format_datetime_custom((XVar)(new XVar(0, 0, 1, 0, 2, 0, 3, arr[0], 4, arr[1], 5, arr[2])), new XVar("HH:mm:ss"));
		}
		protected virtual XVar prepareCalendarData()
		{
			dynamic calendarData = XVar.Array(), initialView = null;
			calendarData = XVar.Clone(new XVar("events", this.prepareEventData()));
			initialView = XVar.Clone(MVCFunctions.postvalue(new XVar("view")));
			if(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(initialView), (XVar)(this.availableViews))), XVar.Pack(false)))
			{
				initialView = new XVar("");
			}
			if(XVar.Pack(!(XVar)(initialView)))
			{
				initialView = XVar.Clone((XVar.Pack(this.searchClauseObj.searchStarted()) ? XVar.Pack("list") : XVar.Pack("month")));
			}
			calendarData.InitAndSetArrayItem(initialView, "initialView");
			calendarData.InitAndSetArrayItem(this.editAvailable(), "canEdit");
			calendarData.InitAndSetArrayItem(this.addAvailable(), "canAdd");
			calendarData.InitAndSetArrayItem(!(XVar)(!(XVar)(this.pSet.getCalendarValue(new XVar("endDateField")))), "hasEndDate");
			calendarData.InitAndSetArrayItem((GlobalVars.locale_info["LOCALE_IFIRSTDAYOFWEEK"] + 1)  %  7, "firstWeekDay");
			calendarData.InitAndSetArrayItem(CommonFunctions.currentLocale(), "locale");
			this.pageData.InitAndSetArrayItem(calendarData, "calendarData");

			return null;
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
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("BeforeQueryCalendar")))))
			{
				return null;
			}
			prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
			where = XVar.Clone(prep["where"]);
			order = XVar.Clone(prep["order"]);
			sql = XVar.Clone(prep["sql"]);
			this.eventsObject.BeforeQueryCalendar((XVar)(sql), (XVar)(where), (XVar)(order), this);
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
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowCalendar"))))
			{
				this.eventsObject.BeforeShowCalendar((XVar)(this.xt), (XVar)(this.templatefile), this);
			}

			return null;
		}
		public static XVar readCalendarModeFromRequest()
		{
			dynamic mode = null;
			mode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(mode == "dashcalendar")
			{
				return Constants.CALENDAR_DASHBOARD;
			}
			else
			{
				return Constants.CALENDAR_SIMPLE;
			}

			return null;
		}
		public override XVar addCommonJs()
		{
			base.addCommonJs();
			this.AddJSFile(new XVar("include/calendar/index.global.min.js"));
			this.AddJSFile(new XVar("include/calendar/locales-all.global.min.js"), new XVar("include/calendar/index.global.min.js"));

			return null;
		}
		protected virtual XVar getRecordset()
		{
			dynamic rs = null;
			rs = XVar.Clone(this.dataSource.getList((XVar)(this.queryCommand)));

			return null;
		}
	}
}
