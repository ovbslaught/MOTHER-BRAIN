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
	public partial class GlobalController : BaseController
	{
		public XVar autocomplete()
		{
			try
			{
				dynamic contextParams = XVar.Array(), control = null, editControls = null, field = null, isExistParent = null, masterTable = null, mode = null, pageName = null, pageType = null, parentCtrlsData = null, respObj = null, shortTableName = null;
				ProjectSettings pSet;
				MVCFunctions.Header("Expires", "Thu, 01 Jan 1970 00:00:01 GMT");
				shortTableName = XVar.Clone(MVCFunctions.postvalue(new XVar("shortTName")));
				GlobalVars.table = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(shortTableName)));
				if(XVar.Pack(!(XVar)(GlobalVars.table)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				field = XVar.Clone(MVCFunctions.postvalue(new XVar("field")));
				pageType = XVar.Clone(MVCFunctions.postvalue(new XVar("pageType")));
				pageName = XVar.Clone(MVCFunctions.postvalue(new XVar("page")));
				if(XVar.Pack(!(XVar)(Security.userHasFieldPermissions((XVar)(GlobalVars.table), (XVar)(field), (XVar)(pageType), (XVar)(pageName), new XVar(true)))))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				GlobalVars.cipherer = XVar.Clone(new RunnerCipherer((XVar)(GlobalVars.table)));
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), (XVar)(pageType), (XVar)(pageName)));
				editControls = XVar.Clone(new EditControlsContainer(new XVar(null), (XVar)(pSet), (XVar)(pageType), (XVar)(GlobalVars.cipherer)));
				control = XVar.Clone(editControls.getControl((XVar)(field)));
				contextParams = XVar.Clone(XVar.Array());
				contextParams.InitAndSetArrayItem(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("data")))), "data");
				masterTable = XVar.Clone(MVCFunctions.postvalue(new XVar("masterTable")));
				if((XVar)((XVar)(masterTable != XVar.Pack(""))  && (XVar)(XSession.Session.KeyExists(MVCFunctions.Concat(masterTable, "_masterRecordData"))))  || (XVar)(MVCFunctions.postvalue(new XVar("masterData"))))
				{
					dynamic masterControlsData = XVar.Array(), masterData = XVar.Array();
					masterData = XVar.Clone(XSession.Session[MVCFunctions.Concat(masterTable, "_masterRecordData")]);
					if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(masterData)))))
					{
						masterData = XVar.Clone(XVar.Array());
					}
					masterControlsData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("masterData")))));
					foreach (KeyValuePair<XVar, dynamic> mValue in masterControlsData.GetEnumerator())
					{
						masterData.InitAndSetArrayItem(mValue.Value, mValue.Key);
					}
					contextParams.InitAndSetArrayItem(masterData, "masterData");
				}
				RunnerContext.push((XVar)(new RunnerContextItem(new XVar(Constants.CONTEXT_ROW), (XVar)(contextParams))));
				parentCtrlsData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("parentCtrlsData")))));
				isExistParent = XVar.Clone(MVCFunctions.postvalue(new XVar("isExistParent")));
				mode = XVar.Clone(MVCFunctions.intval((XVar)(MVCFunctions.postvalue(new XVar("mode")))));
				respObj = XVar.Clone(new XVar("success", true, "data", control.getLookupContentToReload((XVar)(XVar.Equals(XVar.Pack(isExistParent), XVar.Pack("1"))), (XVar)(mode), (XVar)(parentCtrlsData))));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(respObj)));
				RunnerContext.pop();
				MVCFunctions.Echo(new XVar(""));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
