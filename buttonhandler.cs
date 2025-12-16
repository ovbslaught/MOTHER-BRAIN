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
		public XVar buttonhandler()
		{
			try
			{
				dynamic buttId = null, eventId = null, field = null, method = null, page = null, pageTable = null, var_params = XVar.Array();
				ProjectSettings pSet;
				if(XVar.Pack(!(XVar)(CommonFunctions.isPostRequest())))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				var_params = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("params")))));
				if(XVar.Pack(var_params["_base64fields"]))
				{
					foreach (KeyValuePair<XVar, dynamic> f in var_params["_base64fields"].GetEnumerator())
					{
						var_params.InitAndSetArrayItem(MVCFunctions.base64_str2bin((XVar)(var_params[f.Value])), f.Value);
					}
				}
				buttId = XVar.Clone(var_params["buttId"]);
				eventId = XVar.Clone(MVCFunctions.postvalue(new XVar("event")));
				GlobalVars.table = XVar.Clone(var_params["table"]);
				pageTable = XVar.Clone(var_params["pageTable"]);
				if(XVar.Pack(!(XVar)(pageTable)))
				{
					pageTable = XVar.Clone(GlobalVars.table);
				}
				if((XVar)(!(XVar)(CommonFunctions.GetTableURL((XVar)(GlobalVars.table))))  || (XVar)(!(XVar)(CommonFunctions.GetTableURL((XVar)(pageTable)))))
				{
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				page = XVar.Clone(var_params["page"]);
				if(XVar.Pack(!(XVar)(Security.userCanSeePage((XVar)(pageTable), (XVar)(page)))))
				{
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				field = XVar.Clone(var_params["field"]);
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), new XVar(""), (XVar)(page), (XVar)(pageTable)));
				GlobalVars.cipherer = XVar.Clone(new RunnerCipherer((XVar)(GlobalVars.table)));
				if(XVar.Pack(buttId))
				{
					dynamic pageButtons = null;
					pageButtons = XVar.Clone(pSet.customButtons());
					if(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(buttId), (XVar)(pageButtons))), XVar.Pack(false)))
					{
						MVCFunctions.Echo(new XVar(""));
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
				}
				if(XVar.Pack(eventId))
				{
					if(XVar.Pack(!(XVar)(CommonFunctions.verifyAjaxSnippet((XVar)(eventId), (XVar)(field), (XVar)(pSet)))))
					{
						MVCFunctions.Echo(new XVar(""));
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
				}
				var_params.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("masterTable")), "masterTable");
				var_params.InitAndSetArrayItem(XVar.Array(), "masterKeys");
				var_params.InitAndSetArrayItem(RunnerPage.readMasterKeysFromRequest(), "masterKeys");
				if(XVar.Pack(buttId))
				{
					method = XVar.Clone(MVCFunctions.Concat("buttonHandler_", buttId));
					GlobalVars.globalEvents.Invoke(method, (XVar)(var_params));
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if((XVar)(eventId)  && (XVar)(!(XVar)(field)))
				{
					method = XVar.Clone(MVCFunctions.Concat("ajaxHandler_", eventId));
					var_params.InitAndSetArrayItem("grid", "location");
					GlobalVars.globalEvents.Invoke(method, (XVar)(var_params));
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if((XVar)(eventId)  && (XVar)(field))
				{
					method = XVar.Clone(MVCFunctions.Concat("fieldEvent_", eventId));
					var_params.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("pageType")), "location");
					GlobalVars.globalEvents.Invoke(method, (XVar)(var_params));
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
