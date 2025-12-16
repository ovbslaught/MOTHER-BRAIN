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
	public partial class SecurityPluginFB : SecurityPlugin
	{
		public dynamic fbObj;
		protected static bool skipSecurityPluginFBCtor = false;
		public SecurityPluginFB(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipSecurityPluginFBCtor)
			{
				skipSecurityPluginFBCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.appId = XVar.Clone(var_params["appId"]);
			this.appSecret = XVar.Clone(var_params["secretId"]);
			this.fbObj = XVar.Clone(MVCFunctions.fbCreateObject((XVar)(this.appId), (XVar)(this.appSecret)));
		}
		public override XVar getUserInfo(dynamic _param_token)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			#endregion

			dynamic fbme = XVar.Array(), infoData = XVar.Array(), ret = XVar.Array();
			infoData = XVar.Clone(MVCFunctions.fbGetUserInfo((XVar)(this.fbObj), (XVar)(token)));
			fbme = XVar.Clone(infoData["info"]);
			if(XVar.Pack(!(XVar)(fbme)))
			{
				this.var_error = XVar.Clone(infoData["error"]);
				return XVar.Array();
			}
			CommonFunctions.setProjectCookie(new XVar("fb_sr_token"), (XVar)(token), (XVar)(MVCFunctions.time() + (30 * 1440) * 60), new XVar(true));
			ret = XVar.Clone(new XVar("id", MVCFunctions.Concat("fb", XVar.Pack(fbme["id"]).ToString()), "name", MVCFunctions.runner_convert_encoding((XVar)(XVar.Pack(fbme["name"]).ToString()), (XVar)(GlobalVars.cCharset), new XVar("UTF-8")), "email", XVar.Pack(fbme["email"]).ToString(), "raw", fbme));
			if((XVar)(fbme["picture"])  && (XVar)(MVCFunctions.is_array((XVar)(fbme["picture"]))))
			{
				dynamic picResult = XVar.Array();
				picResult = XVar.Clone(MVCFunctions.runner_http_request((XVar)(fbme["picture"]["data"]["url"]), new XVar(""), new XVar("GET"), (XVar)(XVar.Array()), new XVar(false)));
				if(XVar.Pack(picResult["content"]))
				{
					ret.InitAndSetArrayItem(picResult["content"], "picture");
				}
			}
			return ret;
		}
		public override XVar getJSSettings()
		{
			return new XVar("isFB", true, "FBappId", this.appId);
		}
		public override XVar onLogout()
		{
			MVCFunctions.fbDestroySession((XVar)(this.fbObj));
			CommonFunctions.setProjectCookie(new XVar("fb_sr_token"), new XVar(""), (XVar)(MVCFunctions.time() - 1), new XVar(true));

			return null;
		}
		public override XVar savedToken()
		{
			return MVCFunctions.GetCookie("fb_sr_token");
		}
	}
}
