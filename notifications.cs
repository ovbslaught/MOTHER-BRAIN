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
	public partial class SessionStaticGlobalController : BaseController
	{
		public XVar notifications()
		{
			try
			{
				dynamic lastId = null, lastRead = null, messages = null, noti = null;
				GlobalVars.gReadPermissions = new XVar(false);
				lastId = XVar.Clone(CommonFunctions.postvalue_number(new XVar("lastId")));
				lastRead = XVar.Clone(CommonFunctions.postvalue_number(new XVar("lastRead")));
				noti = XVar.Clone(new RunnerNotifications((XVar)(CommonFunctions.getNotificationSettings())));
				if(XVar.Pack(!(XVar)(noti.enabled)))
				{
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if(XVar.Pack(lastRead))
				{
					noti.updateLastRead((XVar)(lastRead));
				}
				messages = XVar.Clone(noti.getUpdates((XVar)(lastId)));
				MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(new XVar("messages", messages, "time", MVCFunctions.now()))));
				MVCFunctions.Echo(new XVar(""));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
