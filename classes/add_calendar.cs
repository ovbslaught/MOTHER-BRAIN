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
	public partial class AddCalendarPage : AddPage
	{
		protected static bool skipAddCalendarPageCtor = false;
		public AddCalendarPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipAddCalendarPageCtor)
			{
				skipAddCalendarPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

		}
		protected override XVar prepareDefvalues()
		{
			dynamic dateField = null, endDate = null, endDateField = null, endTimeField = null, newDate = XVar.Array(), subjectField = null, timeField = null;
			base.prepareDefvalues();
			subjectField = XVar.Clone(this.pSet.getCalendarValue(new XVar("subjectField")));
			dateField = XVar.Clone(this.pSet.getCalendarValue(new XVar("dateField")));
			timeField = XVar.Clone(this.pSet.getCalendarValue(new XVar("timeField")));
			endDateField = XVar.Clone(this.pSet.getCalendarValue(new XVar("endDateField")));
			endTimeField = XVar.Clone(this.pSet.getCalendarValue(new XVar("endTimeField")));
			if(XVar.Pack(!(XVar)(this.defvalues[subjectField])))
			{
				this.defvalues.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("CALENDAR_NEW_EVENT")), subjectField);
			}
			newDate = XVar.Clone(MVCFunctions.db2time((XVar)(MVCFunctions.postvalue(new XVar("date")))));
			if(XVar.Pack(!(XVar)(newDate)))
			{
				newDate = XVar.Clone(MVCFunctions.db2time((XVar)(MVCFunctions.now())));
				newDate.InitAndSetArrayItem(0, 4);
				newDate.InitAndSetArrayItem(0, 5);
				newDate = XVar.Clone(CommonFunctions.addHours((XVar)(newDate), new XVar(1)));
			}
			if(XVar.Pack(!(XVar)(timeField)))
			{
				this.defvalues.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(newDate), new XVar("yyyy-MM-dd")), dateField);
			}
			if((XVar)(timeField)  && (XVar)(timeField != dateField))
			{
				this.defvalues.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(newDate), new XVar("yyyy-MM-dd")), dateField);
				this.defvalues.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(newDate), new XVar("HH:mm:ss")), timeField);
			}
			else
			{
				this.defvalues.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(newDate), new XVar("yyyy-MM-dd HH:mm:ss")), dateField);
			}
			endDate = XVar.Clone(CommonFunctions.addHours((XVar)(newDate), new XVar(1)));
			if(XVar.Pack(endDateField))
			{
				if(XVar.Pack(!(XVar)(endTimeField)))
				{
					this.defvalues.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(endDate), new XVar("yyyy-MM-dd")), endDateField);
				}
				if((XVar)(endTimeField)  && (XVar)(endTimeField != endDateField))
				{
					this.defvalues.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(endDate), new XVar("yyyy-MM-dd")), endDateField);
					this.defvalues.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(endDate), new XVar("HH:mm:ss")), endTimeField);
				}
				else
				{
					this.defvalues.InitAndSetArrayItem(CommonFunctions.format_datetime_custom((XVar)(endDate), new XVar("yyyy-MM-dd HH:mm:ss")), endDateField);
				}
			}

			return null;
		}
		protected override XVar getSaveStatusJSON()
		{
			dynamic statusJson = XVar.Array();
			statusJson = XVar.Clone(base.getSaveStatusJSON());
			if((XVar)(!(XVar)(this.insertedSuccessfully))  || (XVar)(!(XVar)(this.newRecordData)))
			{
				return statusJson;
			}
			statusJson.InitAndSetArrayItem(CalendarPage.makeEvent((XVar)(this.newRecordData), this), "event");
			return statusJson;
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
