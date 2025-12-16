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
		public XVar imager()
		{
			try
			{
				dynamic field = null, keys = XVar.Array(), pageName = null, pageType = null, shortTable = null, var_params = XVar.Array();
				ProjectSettings pSet;
				shortTable = XVar.Clone(MVCFunctions.postvalue(new XVar("table")));
				GlobalVars.table = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(shortTable)));
				if(XVar.Pack(!(XVar)(GlobalVars.table)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				field = XVar.Clone(MVCFunctions.postvalue(new XVar("field")));
				pageName = XVar.Clone(MVCFunctions.postvalue(new XVar("page")));
				if(XVar.Pack(!(XVar)(Security.userHasFieldPermissions((XVar)(GlobalVars.table), (XVar)(field), new XVar(Constants.PAGE_EDIT), (XVar)(pageName), new XVar(false)))))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), (XVar)(pageType), (XVar)(pageName)));
				var_params = XVar.Clone(XVar.Array());
				var_params.InitAndSetArrayItem(GlobalVars.table, "table");
				var_params.InitAndSetArrayItem(field, "field");
				var_params.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("src")) == 1, "src");
				var_params.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("alt")), "alt");
				var_params.InitAndSetArrayItem(pageType, "pageType");
				var_params.InitAndSetArrayItem(pageName, "page");
				keys = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> k in pSet.getTableKeys().GetEnumerator())
				{
					keys.InitAndSetArrayItem(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("key", k.Key + 1))), k.Value);
				}
				var_params.InitAndSetArrayItem(keys, "keys");
				CommonFunctions.GetImageFromDB((XVar)(var_params));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
