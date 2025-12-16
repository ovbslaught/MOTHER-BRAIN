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
	public partial class EditCalendarPage : EditPage
	{
		protected static bool skipEditCalendarPageCtor = false;
		public EditCalendarPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipEditCalendarPageCtor)
			{
				skipEditCalendarPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

		}
		public override XVar process()
		{
			dynamic ret = null;
			if(this.action == "deleteEvent")
			{
				ret = XVar.Clone(this.processDeleteEvent());
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(ret)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			if(this.action == "updateEvent")
			{
				ret = XVar.Clone(this.processUpdateEvent());
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(ret)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			base.process();

			return null;
		}
		protected virtual XVar processUpdateEvent()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(new XVar("success", true));
			if(XVar.Pack(!(XVar)(this.updateEvent((XVar)(MVCFunctions.postvalue(new XVar("start"))), (XVar)(MVCFunctions.postvalue(new XVar("end"))), (XVar)(MVCFunctions.postvalue(new XVar("allDay")))))))
			{
				ret.InitAndSetArrayItem(this.dataSource.lastError(), "error");
				ret.InitAndSetArrayItem(false, "success");
			}
			return ret;
		}
		protected virtual XVar updateEvent(dynamic _param_startStr, dynamic _param_endStr, dynamic _param_fullDay)
		{
			#region pass-by-value parameters
			dynamic startStr = XVar.Clone(_param_startStr);
			dynamic endStr = XVar.Clone(_param_endStr);
			dynamic fullDay = XVar.Clone(_param_fullDay);
			#endregion

			dynamic dateField = null, dc = null, endDateField = null, endTimeField = null, endTimeFieldType = null, fullDayField = null, start = null, timeField = null, timeFieldType = null, var_end = null;
			dateField = XVar.Clone(this.pSet.getCalendarValue(new XVar("dateField")));
			timeField = XVar.Clone(this.pSet.getCalendarValue(new XVar("timeField")));
			endDateField = XVar.Clone(this.pSet.getCalendarValue(new XVar("endDateField")));
			endTimeField = XVar.Clone(this.pSet.getCalendarValue(new XVar("endTimeField")));
			fullDayField = XVar.Clone(this.pSet.getCalendarValue(new XVar("fullDayField")));
			timeFieldType = XVar.Clone(this.pSet.getFieldType((XVar)(timeField)));
			endTimeFieldType = XVar.Clone(this.pSet.getFieldType((XVar)(endTimeField)));
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getSecurityCondition());
			dc.keys = XVar.Clone(this.keys);
			start = XVar.Clone(MVCFunctions.db2time((XVar)(startStr)));
			var_end = XVar.Clone(MVCFunctions.db2time((XVar)(endStr)));
			if(XVar.Pack(start))
			{
				dc.values.InitAndSetArrayItem(CommonFunctions.dbFormatDate((XVar)(start)), dateField);
				if(XVar.Pack(timeField))
				{
					dc.values.InitAndSetArrayItem((XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(timeFieldType))) ? XVar.Pack(CommonFunctions.dbFormatDateTime((XVar)(start))) : XVar.Pack(CommonFunctions.dbFormatTime((XVar)(start)))), timeField);
				}
			}
			if(XVar.Pack(var_end))
			{
				if(XVar.Pack(endDateField))
				{
					dc.values.InitAndSetArrayItem(CommonFunctions.dbFormatDate((XVar)(var_end)), endDateField);
				}
				if(XVar.Pack(endTimeField))
				{
					dc.values.InitAndSetArrayItem((XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(endTimeFieldType))) ? XVar.Pack(CommonFunctions.dbFormatDateTime((XVar)(var_end))) : XVar.Pack(CommonFunctions.dbFormatTime((XVar)(var_end)))), endTimeField);
				}
			}
			if(XVar.Pack(fullDayField))
			{
				dc.values.InitAndSetArrayItem(fullDay, fullDayField);
			}
			if(XVar.Pack(!(XVar)(dc.values)))
			{
				return true;
			}
			if(XVar.Pack(!(XVar)(this.dataSource.updateSingle((XVar)(dc)))))
			{
				return false;
			}
			return true;
		}
		protected virtual XVar processDeleteEvent()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(new XVar("success", true));
			if(XVar.Pack(!(XVar)(this.deleteAvailable())))
			{
				ret.InitAndSetArrayItem("No delete permissions", "error");
				ret.InitAndSetArrayItem(false, "success");
				return ret;
			}
			if(XVar.Pack(!(XVar)(this.deleteEvent())))
			{
				ret.InitAndSetArrayItem(this.dataSource.lastError(), "error");
				ret.InitAndSetArrayItem(false, "success");
			}
			return ret;
		}
		protected virtual XVar deleteEvent()
		{
			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getSecurityCondition());
			dc.keys = XVar.Clone(this.keys);
			if(XVar.Pack(!(XVar)(this.dataSource.deleteSingle((XVar)(dc)))))
			{
				return false;
			}
			return true;
		}
		protected override XVar getSaveStatusJSON()
		{
			dynamic keyFields = XVar.Array(), keyValues = XVar.Array(), newData = null, statusJson = XVar.Array();
			statusJson = XVar.Clone(base.getSaveStatusJSON());
			if(XVar.Pack(!(XVar)(this.updatedSuccessfully)))
			{
				return statusJson;
			}
			newData = XVar.Clone(this.getCurrentRecordInternal());
			if(XVar.Pack(!(XVar)(newData)))
			{
				return statusJson;
			}
			statusJson.InitAndSetArrayItem(CalendarPage.makeEvent((XVar)(newData), this), "event");
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			keyValues = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> k in keyFields.GetEnumerator())
			{
				keyValues.InitAndSetArrayItem(this.oldKeys[k.Value], null);
			}
			statusJson.InitAndSetArrayItem(keyValues, "oldKeys");
			return statusJson;
		}
		protected override XVar prepareButtons()
		{
			base.prepareButtons();
			if(XVar.Pack(this.deleteAvailable()))
			{
				this.xt.assign(new XVar("calendar_delete_attrs"), (XVar)(MVCFunctions.Concat("id=\"deleteButton", this.id, "\"")));
				this.xt.assign(new XVar("calendar_delete"), new XVar(true));
			}
			this.xt.assign(new XVar("reset_button"), new XVar(false));
			this.xt.assign(new XVar("view_page_button"), new XVar(false));

			return null;
		}
		public override XVar deleteAvailable()
		{
			return this.permis[this.tName]["delete"];
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
			settings.InitAndSetArrayItem(pSet.getTableValue(new XVar("calendarSettings")), "calendarSettings");
			return settings;
		}
	}
}
