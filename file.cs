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
	[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
	public partial class SessionStaticGlobalController : BaseController
	{
		public ActionResult file()
		{
			try
			{
				dynamic field = null, fileHandler = null, filename = null, formStamp = null, keys = XVar.Array(), outputAsAttachment = null, thumb = null, useHttpRange = null;
				ProjectSettings pSet;
				GlobalVars.gReadPermissions = new XVar(false);
				GlobalVars.table = XVar.Clone(MVCFunctions.postvalue(new XVar("table")));
				field = XVar.Clone(MVCFunctions.postvalue(new XVar("field")));
				thumb = XVar.Clone(MVCFunctions.postvalue(new XVar("thumb")));
				outputAsAttachment = XVar.Clone(MVCFunctions.postvalue(new XVar("nodisp")) == 1);
				if(XVar.Pack(MVCFunctions.postvalue(new XVar("userpic"))))
				{
					dynamic ftype = null, userData = XVar.Array(), userpicField = null, value = null;
					if(XVar.Pack(!(XVar)(Security.showUserPic())))
					{
						MVCFunctions.Echo(new XVar(""));
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
					userData = Security.currentUserData();
					userpicField = XVar.Clone((XVar.Pack(Security.userpicField()) ? XVar.Pack(Security.userpicField()) : XVar.Pack("picture")));
					value = userData[userpicField];
					ftype = XVar.Clone(MVCFunctions.SupposeImageType((XVar)(value)));
					if(XVar.Pack(!(XVar)(ftype)))
					{
						ftype = new XVar("image/png");
					}
					MVCFunctions.Header("Cache-Control", "max-age=0");
					MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Type: ", ftype)));
					MVCFunctions.echoBinary((XVar)(value));
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if(XVar.Pack(!(XVar)(CommonFunctions.GetTableURL((XVar)(GlobalVars.table)))))
				{
					MVCFunctions.Echo("unknown table");
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				formStamp = XVar.Clone(MVCFunctions.postvalue(new XVar("fkey")));
				if(XVar.Pack(!(XVar)(formStamp)))
				{
					if(XVar.Pack(!(XVar)(Security.checkFieldAccess((XVar)(GlobalVars.table), (XVar)(field), new XVar(false)))))
					{
						MVCFunctions.Echo("access denied");
						MVCFunctions.Echo(new XVar(""));
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
				}
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table)));
				fileHandler = XVar.Clone(new RunnerFileHandler((XVar)(field), (XVar)(pSet), (XVar)(formStamp)));
				filename = XVar.Clone((XVar.Pack(MVCFunctions.postvalue(new XVar("file")) != "") ? XVar.Pack(MVCFunctions.postvalue(new XVar("file"))) : XVar.Pack(MVCFunctions.postvalue(new XVar("filename")))));
				useHttpRange = XVar.Clone(!(XVar)(MVCFunctions.postvalue(new XVar("norange"))));
				keys = XVar.Clone(XVar.Array());
				if(XVar.Pack(!(XVar)(formStamp)))
				{
					foreach (KeyValuePair<XVar, dynamic> k in pSet.getTableKeys().GetEnumerator())
					{
						keys.InitAndSetArrayItem(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("key", k.Key + 1))), k.Value);
					}
				}
				fileHandler.showFile((XVar)(filename), (XVar)(!(XVar)(!(XVar)(MVCFunctions.postvalue(new XVar("thumbnail"))))), (XVar)(!(XVar)(!(XVar)(MVCFunctions.postvalue(new XVar("icon"))))), (XVar)(outputAsAttachment), (XVar)(useHttpRange), (XVar)(keys));
				MVCFunctions.Echo(new XVar(""));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
