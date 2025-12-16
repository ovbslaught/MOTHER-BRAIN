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
		public ActionResult oauthcallback()
		{
			try
			{
				dynamic connId = null, rconn = null, state = null;
				state = XVar.Clone(MVCFunctions.postvalue("state"));
				if((XVar)(state == XVar.Pack(""))  || (XVar)(!(XVar)(XSession.Session["oauth2statemap"])))
				{
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				connId = XVar.Clone(XSession.Session["oauth2statemap"][state]);
				XSession.Session["oauth2statemap"].Remove(state);
				if(XVar.Pack(!(XVar)(connId)))
				{
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				rconn = XVar.Clone(GlobalVars.restApis.getConnection((XVar)(connId)));
				if(XVar.Pack(!(XVar)(rconn)))
				{
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if(XVar.Pack(rconn.validateCode((XVar)(MVCFunctions.postvalue("code")))))
				{
					if(XVar.Pack(XSession.Session["MyURL"]))
					{
						dynamic url = null;
						url = XVar.Clone(XSession.Session["MyURL"]);
						XSession.Session.Remove("MyURL");
						MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", url)));
						MVCFunctions.Echo(new XVar(""));
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
				}
				MVCFunctions.Echo("invalid token");
				if(XVar.Pack(ProjectSettings.getProjectValue(new XVar("detailedError"))))
				{
					MVCFunctions.Echo("<pre>");
					MVCFunctions.Echo(rconn.lastError());
					MVCFunctions.Echo("</pre>");
				}
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
