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
		public XVar sessioncontrol()
		{
			try
			{
				dynamic sessionControl = XVar.Array();
				CommonFunctions.add_nocache_headers();
				if(XVar.Pack(!(XVar)(Security.hasLogin())))
				{
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				sessionControl = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("sessionControl")));
				if((XVar)(sessionControl["keepAlive"])  && (XVar)(MVCFunctions.postvalue(new XVar("sessionControl")) == "keepAlive"))
				{
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(new XVar("success", true))));
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if((XVar)(sessionControl["forceExpire"])  && (XVar)(MVCFunctions.postvalue(new XVar("sessionControl")) == "forceExpire"))
				{
					dynamic lastUserActivity = null, lifeTimeSeconds = null, secondsOfInvactivity = null, var_response = XVar.Array(), warnBeforeSeconds = null;
					var_response = XVar.Clone(new XVar("success", true));
					lastUserActivity = XVar.Clone(MVCFunctions.postvalue(new XVar("lastUserActivity")));
					if((XVar)(XSession.Session.KeyExists("lastUserActivity"))  && (XVar)(lastUserActivity < XSession.Session["lastUserActivity"]))
					{
						lastUserActivity = XVar.Clone(XSession.Session["lastUserActivity"]);
					}
					XSession.Session["lastUserActivity"] = lastUserActivity;
					lifeTimeSeconds = XVar.Clone(sessionControl["lifeTime"] * 60);
					warnBeforeSeconds = XVar.Clone(MVCFunctions.postvalue(new XVar("warnBeforeSeconds")));
					secondsOfInvactivity = XVar.Clone(MVCFunctions.time() - lastUserActivity);
					if(lifeTimeSeconds <= secondsOfInvactivity)
					{
						var_response.InitAndSetArrayItem(true, "logout");
						XSession.Session.Remove("lastUserActivity");
					}
					else
					{
						if((XVar)(sessionControl["warnExpire"])  && (XVar)(lifeTimeSeconds - warnBeforeSeconds <= secondsOfInvactivity))
						{
							var_response.InitAndSetArrayItem(true, "showExpirePopup");
							XSession.Session["lastUserActivity"] = (MVCFunctions.time() - lifeTimeSeconds * 60) + warnBeforeSeconds;
							var_response.InitAndSetArrayItem(warnBeforeSeconds, "secondsLeft");
						}
					}
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(var_response)));
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
