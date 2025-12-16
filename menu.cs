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
		public ActionResult menu()
		{
			try
			{
				dynamic pageObject = null, var_params = XVar.Array();
				XTempl xt;
				if(XVar.Pack(Security.hasLogin()))
				{
					Security.processLogoutRequest();
					if((XVar)(!(XVar)(CommonFunctions.isLogged()))  || (XVar)(Security.isGuest()))
					{
						Security.tryRelogin();
					}
					if(XVar.Pack(!(XVar)(CommonFunctions.isLogged())))
					{
						CommonFunctions.redirectToLogin();
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
				}
				if((XVar)(XSession.Session["MyURL"] == "")  || (XVar)(!(XVar)(Security.isGuest())))
				{
					Security.saveRedirectURL();
				}
				xt = XVar.UnPackXTempl(new XTempl());
				var_params = XVar.Clone(XVar.Array());
				var_params.InitAndSetArrayItem(CommonFunctions.postvalue_number(new XVar("id")), "id");
				var_params.InitAndSetArrayItem(xt, "xt");
				var_params.InitAndSetArrayItem(Constants.GLOBAL_PAGES, "tName");
				var_params.InitAndSetArrayItem(Constants.PAGE_MENU, "pageType");
				var_params.InitAndSetArrayItem(false, "needSearchClauseObj");
				var_params.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("page")), "pageName");
				GlobalVars.pageObject = XVar.Clone(new MenuPage((XVar)(var_params)));
				GlobalVars.pageObject.init();
				GlobalVars.pageObject.process();
				return null;
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
