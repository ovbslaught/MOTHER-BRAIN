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
	public partial class AddGanttPage : AddPage
	{
		public dynamic parentValue = XVar.Pack(null);
		protected static bool skipAddGanttPageCtor = false;
		public AddGanttPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipAddGanttPageCtor)
			{
				skipAddGanttPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

		}
		protected override XVar prepareDefvalues()
		{
			dynamic endDateField = null, nameField = null, parentField = null, startDate = null, startDateField = null;
			base.prepareDefvalues();
			parentField = XVar.Clone(this.pSet.getGanttValue(new XVar("parentField")));
			nameField = XVar.Clone(this.pSet.getGanttValue(new XVar("nameField")));
			startDateField = XVar.Clone(this.pSet.getGanttValue(new XVar("startDateField")));
			endDateField = XVar.Clone(this.pSet.getGanttValue(new XVar("endDateField")));
			if(XVar.Pack(!(XVar)(this.defvalues[nameField])))
			{
				this.defvalues.InitAndSetArrayItem((XVar.Pack((XVar)(this.parentValue)  && (XVar)(parentField)) ? XVar.Pack(CommonFunctions.mlang_message(new XVar("GANTT_CHILD_TASK"))) : XVar.Pack(CommonFunctions.mlang_message(new XVar("GANTT_NEW_TASK")))), nameField);
			}
			if((XVar)(this.parentValue)  && (XVar)(parentField))
			{
				this.defvalues.InitAndSetArrayItem(this.parentValue, parentField);
			}
			if(XVar.Pack(!(XVar)(this.defvalues[startDateField])))
			{
				startDate = XVar.Clone(MVCFunctions.db2time((XVar)(MVCFunctions.postvalue(new XVar("start")))));
				if(XVar.Pack(!(XVar)(startDate)))
				{
					startDate = XVar.Clone(CommonFunctions.adddays((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar(1)));
				}
				this.defvalues.InitAndSetArrayItem(CommonFunctions.dbFormatDate((XVar)(startDate)), startDateField);
			}
			if(XVar.Pack(!(XVar)(this.defvalues[endDateField])))
			{
				dynamic endDate = null;
				startDate = XVar.Clone(MVCFunctions.db2time((XVar)(this.defvalues[startDateField])));
				endDate = XVar.Clone(CommonFunctions.adddays((XVar)(startDate), new XVar(1)));
				this.defvalues.InitAndSetArrayItem(CommonFunctions.dbFormatDate((XVar)(endDate)), endDateField);
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
			statusJson.InitAndSetArrayItem(GanttPage.makeTask((XVar)(this.newRecordData), this), "task");
			return statusJson;
		}
		protected override XVar buildNewRecordData()
		{
			dynamic parentField = null;
			base.buildNewRecordData();
			parentField = XVar.Clone(this.pSet.getGanttValue(new XVar("parentField")));
			if((XVar)((XVar)(parentField)  && (XVar)(this.parentValue))  && (XVar)(MVCFunctions.count(this.pSet.getTableKeys()) == 1))
			{
				if(XVar.Pack(!(XVar)(this.newRecordData[parentField])))
				{
					this.newRecordData.InitAndSetArrayItem(this.parentValue, parentField);
				}
			}

			return null;
		}
	}
}
