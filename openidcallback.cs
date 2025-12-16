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
		public ActionResult openidcallback()
		{
			try
			{
				dynamic loginPageObject = null, openIdStateMap = XVar.Array(), plugin = null, state = null, token = null;
				state = XVar.Clone(MVCFunctions.postvalue("state"));
				openIdStateMap = CommonFunctions.storageGet(new XVar("openIdStateMap"));
				if((XVar)((XVar)(state == XVar.Pack(""))  || (XVar)(!(XVar)(MVCFunctions.is_array((XVar)(openIdStateMap)))))  || (XVar)(!(XVar)(openIdStateMap[state])))
				{
					MVCFunctions.Echo("wrong request");
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				plugin = XVar.Clone(Security.getAuthPlugin((XVar)(openIdStateMap[state])));
				openIdStateMap.Remove(state);
				if(XVar.Pack(!(XVar)(plugin)))
				{
					MVCFunctions.Echo("Unknown security provider");
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if(XVar.Pack(!(XVar)(MVCFunctions.postvalue("code"))))
				{
					MVCFunctions.Echo("Unable to authenticate. ");
					MVCFunctions.Echo(MVCFunctions.postvalue("error"));
					MVCFunctions.Echo(" ");
					MVCFunctions.Echo(MVCFunctions.postvalue("error_description"));
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				token = XVar.Clone(plugin.getIdToken((XVar)(MVCFunctions.postvalue("code"))));
				if(XVar.Pack(!(XVar)(token)))
				{
					MVCFunctions.Echo("invalid token");
					if(XVar.Pack(ProjectSettings.getProjectValue(new XVar("detailedError"))))
					{
						MVCFunctions.Echo("<pre>");
						MVCFunctions.Echo(plugin.getError());
						MVCFunctions.Echo("</pre>");
					}
				}
				loginPageObject = XVar.Clone(Security.createLoginPageObject());
				if(XVar.Pack(loginPageObject.LoginWithSP((XVar)(plugin), (XVar)(token))))
				{
					loginPageObject.redirectAfterSuccessfulLogin();
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				else
				{
					MVCFunctions.Echo("unable to login.");
					MVCFunctions.Echo(loginPageObject.message);
				}
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
