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
	public partial class EditGanttPage : EditPage
	{
		protected static bool skipEditGanttPageCtor = false;
		public EditGanttPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipEditGanttPageCtor)
			{
				skipEditGanttPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

		}
		public override XVar process()
		{
			dynamic ret = null;
			if(this.action == "deleteTask")
			{
				ret = XVar.Clone(this.processDeleteTask());
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(ret)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			if(this.action == "updateTask")
			{
				ret = XVar.Clone(this.processUpdateTask());
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(ret)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			base.process();

			return null;
		}
		protected virtual XVar processDeleteTask()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(new XVar("success", true));
			if(XVar.Pack(!(XVar)(this.deleteAvailable())))
			{
				ret.InitAndSetArrayItem("No delete permissions", "error");
				ret.InitAndSetArrayItem(false, "success");
				return ret;
			}
			if(XVar.Pack(!(XVar)(this.deleteTask())))
			{
				ret.InitAndSetArrayItem(this.dataSource.lastError(), "error");
				ret.InitAndSetArrayItem(false, "success");
			}
			return ret;
		}
		protected virtual XVar processUpdateTask()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(new XVar("success", true));
			ret.InitAndSetArrayItem(this.updateTask((XVar)(new XVar("startDate", MVCFunctions.postvalue(new XVar("startDate")), "endDate", MVCFunctions.postvalue(new XVar("endDate")), "progress", MVCFunctions.postvalue(new XVar("progress"))))), "task");
			if(XVar.Pack(!(XVar)(ret["task"])))
			{
				ret.InitAndSetArrayItem(this.dataSource.lastError(), "error");
				ret.InitAndSetArrayItem(false, "success");
			}
			return ret;
		}
		protected virtual XVar deleteTask()
		{
			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getSecurityCondition());
			dc.keys = XVar.Clone(this.keys);
			return !(XVar)(!(XVar)(this.dataSource.deleteSingle((XVar)(dc))));
		}
		protected virtual XVar updateTask(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic dc = null, endDateField = null, newData = null, progressField = null, startDateField = null;
			startDateField = XVar.Clone(this.pSet.getGanttValue(new XVar("startDateField")));
			endDateField = XVar.Clone(this.pSet.getGanttValue(new XVar("endDateField")));
			progressField = XVar.Clone(this.pSet.getGanttValue(new XVar("progressField")));
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getSecurityCondition());
			dc.keys = XVar.Clone(this.keys);
			if(XVar.Pack(data["startDate"]))
			{
				dc.values.InitAndSetArrayItem(data["startDate"], startDateField);
			}
			if(XVar.Pack(data["endDate"]))
			{
				dc.values.InitAndSetArrayItem(data["endDate"], endDateField);
			}
			if(XVar.Pack(data["progress"]))
			{
				dc.values.InitAndSetArrayItem(data["progress"], progressField);
			}
			if(XVar.Pack(!(XVar)(this.dataSource.updateSingle((XVar)(dc)))))
			{
				return false;
			}
			newData = XVar.Clone(this.getCurrentRecordInternal());
			if(XVar.Pack(!(XVar)(newData)))
			{
				return XVar.Array();
			}
			return GanttPage.makeTask((XVar)(newData), this);
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
			statusJson.InitAndSetArrayItem(GanttPage.makeTask((XVar)(newData), this), "task");
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
				this.xt.assign(new XVar("gantt_delete_attrs"), (XVar)(MVCFunctions.Concat("id=\"deleteButton", this.id, "\"")));
				this.xt.assign(new XVar("gantt_delete"), new XVar(true));
			}
			if(XVar.Pack(this.addChildAvailable()))
			{
				this.xt.assign(new XVar("gantt_add_child_attrs"), (XVar)(MVCFunctions.Concat("id=\"addChildButton", this.id, "\"")));
				this.xt.assign(new XVar("gantt_add_child"), new XVar(true));
			}
			this.xt.assign(new XVar("reset_button"), new XVar(false));
			this.xt.assign(new XVar("view_page_button"), new XVar(false));

			return null;
		}
		public override XVar deleteAvailable()
		{
			return this.permis[this.tName]["delete"];
		}
		protected virtual XVar addChildAvailable()
		{
			return (XVar)((XVar)(this.addAvailable())  && (XVar)(MVCFunctions.count(this.pSet.getTableKeys()) == 1))  && (XVar)(this.pSet.getGanttValue(new XVar("parentField")));
		}
	}
}
