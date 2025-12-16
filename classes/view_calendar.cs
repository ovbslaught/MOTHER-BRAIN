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
	public partial class ViewCalendarPage : ViewPage
	{
		public dynamic action = XVar.Pack("");
		protected static bool skipViewCalendarPageCtor = false;
		public ViewCalendarPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipViewCalendarPageCtor)
			{
				skipViewCalendarPageCtor = false;
				return;
			}
		}
		public override XVar process()
		{
			if(this.action == "deleteEvent")
			{
				dynamic ret = null;
				ret = XVar.Clone(this.processDeleteEvent());
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(ret)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			base.process();

			return null;
		}
		protected virtual XVar processDeleteEvent()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(new XVar("success", true));
			if((XVar)(!(XVar)(this.pSet.hasEditPage()))  || (XVar)(!(XVar)(this.deleteAvailable())))
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
		protected override XVar prepareButtons()
		{
			dynamic data = null;
			base.prepareButtons();
			this.xt.assign(new XVar("edit_page_button"), new XVar(false));
			data = XVar.Clone(this.getCurrentRecordInternal());
			if((XVar)((XVar)(this.deleteAvailable())  && (XVar)(this.recordEditable((XVar)(data), new XVar("D"))))  && (XVar)(this.pSet.hasEditPage()))
			{
				this.xt.assign(new XVar("calendar_delete_attrs"), (XVar)(MVCFunctions.Concat("id=\"deleteButton", this.id, "\"")));
				this.xt.assign(new XVar("calendar_delete"), new XVar(true));
			}
			if((XVar)(this.editAvailable())  && (XVar)(this.recordEditable((XVar)(data))))
			{
				this.xt.assign(new XVar("calendar_edit"), new XVar(true));
				this.xt.assign(new XVar("calendar_edit_attrs"), (XVar)(MVCFunctions.Concat("id=\"editPageButton", this.id, "\"")));
			}

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
		protected override XVar displayViewPage()
		{
			dynamic endTimeField = null, fullDayField = null, timeField = null;
			fullDayField = XVar.Clone(this.pSet.getCalendarValue(new XVar("fullDayField")));
			timeField = XVar.Clone(this.pSet.getCalendarValue(new XVar("timeField")));
			endTimeField = XVar.Clone(this.pSet.getCalendarValue(new XVar("endTimeField")));
			if(XVar.Pack(fullDayField))
			{
				dynamic data = XVar.Array();
				data = XVar.Clone(this.getCurrentRecordInternal());
				if(XVar.Pack(CheckboxField.checkedValue((XVar)(data[fullDayField]))))
				{
					this.hideField((XVar)(timeField));
					this.hideField((XVar)(endTimeField));
				}
			}
			return base.displayViewPage();
		}
	}
}
