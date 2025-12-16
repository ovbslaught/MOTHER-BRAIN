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
		public ActionResult external()
		{
			try
			{
				dynamic duration = null, host = null, jwt = null, payload = null, url = null;
				if((XVar)(!(XVar)(CommonFunctions.isLogged()))  || (XVar)(Security.isGuest()))
				{
					Security.tryRelogin();
				}
				if((XVar)(!(XVar)(CommonFunctions.isLogged()))  || (XVar)(Security.isGuest()))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				duration = new XVar(30);
				url = XVar.Clone(MVCFunctions.postvalue(new XVar("url")));
				if(XVar.Pack(!(XVar)(url)))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(url), new XVar("://"))), XVar.Pack(false)))
				{
					host = XVar.Clone(CommonFunctions.hostFromUrl((XVar)(url)));
				}
				else
				{
					host = XVar.Clone(CommonFunctions.projectHost());
				}
				payload = XVar.Clone(new XVar("username", Security.getUserName(), "host", host, "external", true, "origin", CommonFunctions.projectURL()));
				jwt = XVar.Clone(CommonFunctions.jwt_encode((XVar)(payload), (XVar)(duration)));
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(url), new XVar("?"))), XVar.Pack(false)))
				{
					url = MVCFunctions.Concat(url, "&token=", jwt);
				}
				else
				{
					url = MVCFunctions.Concat(url, "?token=", jwt);
				}
				MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", url)));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
