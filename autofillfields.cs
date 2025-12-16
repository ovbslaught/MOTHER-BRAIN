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
		public XVar autofillfields()
		{
			try
			{
				dynamic contextParams = XVar.Array(), control = null, editControls = null, linkFieldVal = null, mainField = null, masterTable = null, pageName = null, pageType = null, shortTableName = null;
				ProjectSettings pSet;
				MVCFunctions.Header("Expires", "Thu, 01 Jan 1970 00:00:01 GMT");
				shortTableName = XVar.Clone(MVCFunctions.postvalue(new XVar("shortTName")));
				GlobalVars.table = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(shortTableName)));
				if(XVar.Pack(!(XVar)(GlobalVars.table)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				mainField = XVar.Clone(MVCFunctions.postvalue(new XVar("mainField")));
				linkFieldVal = XVar.Clone(MVCFunctions.postvalue(new XVar("linkFieldVal")));
				pageName = XVar.Clone(MVCFunctions.postvalue(new XVar("page")));
				pageType = XVar.Clone(MVCFunctions.postvalue(new XVar("pageType")));
				if(XVar.Pack(!(XVar)(Security.userHasFieldPermissions((XVar)(GlobalVars.table), (XVar)(mainField), (XVar)(pageType), (XVar)(pageName), new XVar(true)))))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				GlobalVars.cipherer = XVar.Clone(new RunnerCipherer((XVar)(GlobalVars.table)));
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), (XVar)(pageType), (XVar)(pageName)));
				editControls = XVar.Clone(new EditControlsContainer(new XVar(null), (XVar)(pSet), (XVar)(pageType), (XVar)(GlobalVars.cipherer)));
				control = XVar.Clone(editControls.getControl((XVar)(mainField)));
				contextParams = XVar.Clone(XVar.Array());
				contextParams.InitAndSetArrayItem(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("data")))), "data");
				masterTable = XVar.Clone(MVCFunctions.postvalue(new XVar("masterTable")));
				if((XVar)(masterTable != XVar.Pack(""))  && (XVar)(XSession.Session.KeyExists(MVCFunctions.Concat(masterTable, "_masterRecordData"))))
				{
					contextParams.InitAndSetArrayItem(XSession.Session[MVCFunctions.Concat(masterTable, "_masterRecordData")], "masterData");
				}
				RunnerContext.push((XVar)(new RunnerContextItem(new XVar(Constants.CONTEXT_ROW), (XVar)(contextParams))));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(new XVar("success", true, "data", control.getAutoFillData((XVar)(linkFieldVal))))));
				RunnerContext.pop();
				MVCFunctions.Echo(new XVar(""));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
