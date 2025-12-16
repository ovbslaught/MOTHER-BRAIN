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
		public ActionResult samlcallback()
		{
			try
			{
				dynamic loginPageObject = null, provider = null, samlResponce = null;
				samlResponce = XVar.Clone(MVCFunctions.postvalue(new XVar("SAMLResponse")));
				if(XVar.Pack(!(XVar)(samlResponce)))
				{
					MVCFunctions.Echo("wrong request");
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				samlResponce = XVar.Clone(MVCFunctions.base64_decode((XVar)(samlResponce)));
				provider = XVar.Clone(Security.getAuthPlugin(new XVar("sa")));
				loginPageObject = XVar.Clone(Security.createLoginPageObject());
				if(XVar.Pack(loginPageObject.LoginWithSP((XVar)(provider), (XVar)(samlResponce))))
				{
					loginPageObject.redirectAfterSuccessfulLogin();
				}
				MVCFunctions.Echo("unable to login.");
				MVCFunctions.Echo(loginPageObject.message);
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
