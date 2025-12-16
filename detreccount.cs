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
		public XVar detreccount()
		{
			try
			{
				dynamic dSTable = null, dTable = null, mKeys = null, mSTable = null, mTable = null, output = null, pageObject = null, pageType = null, respObj = null, var_params = XVar.Array();
				XTempl xt;
				MVCFunctions.Header("Expires", "Thu, 01 Jan 1970 00:00:01 GMT");
				mSTable = XVar.Clone(MVCFunctions.postvalue(new XVar("mSTable")));
				mTable = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(mSTable)));
				if(XVar.Pack(!(XVar)(mTable)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				dSTable = XVar.Clone(MVCFunctions.postvalue(new XVar("dSTable")));
				dTable = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(dSTable)));
				if(XVar.Pack(!(XVar)(dTable)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if(XVar.Pack(!(XVar)(Security.userCan(new XVar("S"), (XVar)(dTable)))))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				mKeys = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("mKeys")))));
				pageType = XVar.Clone(MVCFunctions.postvalue(new XVar("pageType")));
				xt = XVar.UnPackXTempl(new XTempl());
				var_params = XVar.Clone(new XVar("pageType", pageType));
				var_params.InitAndSetArrayItem(xt, "xt");
				var_params.InitAndSetArrayItem(mTable, "tName");
				var_params.InitAndSetArrayItem(false, "needSearchClauseObj");
				pageObject = XVar.Clone(new RunnerPage((XVar)(var_params)));
				output = XVar.Clone(pageObject.countDetailsRecsNoSubQ((XVar)(dTable), (XVar)(mKeys)));
				respObj = XVar.Clone(new XVar("success", true, "recsCount", output));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(respObj)));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
