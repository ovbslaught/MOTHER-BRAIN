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
		public XVar mfhandler()
		{
			try
			{
				dynamic field = null, fileHandler = null, filename = null, keys = XVar.Array(), outputAsAttachment = null, pageName = null, pageType = null, requestAction = null, success = null, thumb = null, useHttpRange = null, userpicMode = null;
				ProjectSettings pSet;
				GlobalVars.gReadPermissions = new XVar(false);
				GlobalVars.table = XVar.Clone(MVCFunctions.postvalue(new XVar("table")));
				field = XVar.Clone(MVCFunctions.postvalue(new XVar("field")));
				thumb = XVar.Clone(MVCFunctions.postvalue(new XVar("thumb")));
				pageType = XVar.Clone(MVCFunctions.postvalue(new XVar("pageType")));
				outputAsAttachment = XVar.Clone(MVCFunctions.postvalue(new XVar("nodisp")) != 1);
				pageName = XVar.Clone(MVCFunctions.postvalue(new XVar("page")));
				userpicMode = new XVar(false);
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
				if(XVar.Pack(!(XVar)(userpicMode)))
				{
					dynamic showField = null;
					if(XVar.Pack(!(XVar)(CommonFunctions.GetTableURL((XVar)(GlobalVars.table)))))
					{
						MVCFunctions.Echo(new XVar(0));
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
					requestAction = XVar.Clone(MVCFunctions.postvalue("_action"));
					showField = XVar.Clone((XVar)((XVar)((XVar)(requestAction == "POST")  || (XVar)(MVCFunctions.postvalue(new XVar("fkey"))))  || (XVar)(requestAction == "DELETE"))  || (XVar)(requestAction == "RESET"));
					if(XVar.Pack(!(XVar)(Security.userHasFieldPermissions((XVar)(GlobalVars.table), (XVar)(field), (XVar)(pageType), (XVar)(pageName), (XVar)(showField)))))
					{
						MVCFunctions.Echo(new XVar(0));
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
				}
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), (XVar)(pageType)));
				CommonFunctions.add_nocache_headers();
				if((XVar)(requestAction == "DELETE")  || (XVar)(requestAction == "POST"))
				{
					MVCFunctions.Header("Content-Disposition", "inline; filename=\"files.json\"");
					MVCFunctions.Header("X-Content-Type-Options", "nosniff");
					MVCFunctions.Header("Access-Control-Allow-Origin", "*");
					MVCFunctions.Header("Access-Control-Allow-Methods", "OPTIONS, HEAD, GET, POST");
					MVCFunctions.Header("Access-Control-Allow-Headers", "X-File-Name, X-File-Type, X-File-Size");
				}
				switch(((XVar)requestAction).ToString())
				{
					case "RESET":
						fileHandler = XVar.Clone(new RunnerFileHandler((XVar)(field), (XVar)(pSet), (XVar)(MVCFunctions.postvalue(new XVar("formStamp")))));
						fileHandler.resetTo((XVar)(MVCFunctions.postvalue(new XVar("defaultValue"))));
						MVCFunctions.Echo(MVCFunctions.runner_json_encode(new XVar(true)));
						break;
					case "DELETE":
						fileHandler = XVar.Clone(new RunnerFileHandler((XVar)(field), (XVar)(pSet), (XVar)(MVCFunctions.postvalue(new XVar("formStamp")))));
						success = XVar.Clone(fileHandler.delete((XVar)(MVCFunctions.postvalue(new XVar("fileName")))));
						MVCFunctions.Header("Content-type", "application/json");
						MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(success)));
						break;
					case "POST":
						fileHandler = XVar.Clone(new RunnerFileHandler((XVar)(field), (XVar)(pSet), (XVar)(MVCFunctions.postvalue(new XVar("formStamp")))));
						if(XVar.Pack(fileHandler.directUpload()))
						{
							fileHandler.prepareDirectUpload((XVar)(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("file"))))));
						}
						else
						{
							fileHandler.acceptUpload((XVar)(MVCFunctions.uploadFiles(new XVar("files"))), new XVar(true));
						}
						break;
					case "GET":
					default:
						fileHandler = XVar.Clone(new RunnerFileHandler((XVar)(field), (XVar)(pSet), (XVar)(MVCFunctions.postvalue(new XVar("fkey")))));
						filename = XVar.Clone((XVar.Pack(MVCFunctions.postvalue(new XVar("file")) != "") ? XVar.Pack(MVCFunctions.postvalue(new XVar("file"))) : XVar.Pack(MVCFunctions.postvalue(new XVar("filename")))));
						useHttpRange = XVar.Clone(!(XVar)(MVCFunctions.postvalue(new XVar("norange"))));
						keys = XVar.Clone(XVar.Array());
						foreach (KeyValuePair<XVar, dynamic> k in pSet.getTableKeys().GetEnumerator())
						{
							keys.InitAndSetArrayItem(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("key", k.Key + 1))), k.Value);
						}
						fileHandler.showFile((XVar)(filename), (XVar)(!(XVar)(!(XVar)(MVCFunctions.postvalue(new XVar("thumbnail"))))), (XVar)(!(XVar)(!(XVar)(MVCFunctions.postvalue(new XVar("icon"))))), (XVar)(outputAsAttachment), (XVar)(useHttpRange), (XVar)(keys));
						break;
				}
				MVCFunctions.Echo(new XVar(""));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
