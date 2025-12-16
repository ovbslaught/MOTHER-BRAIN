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
	public partial class SecurityPluginGoogle : SecurityPluginOpenId
	{
		public dynamic domain = XVar.Pack("");
		protected static bool skipSecurityPluginGoogleCtor = false;
		public SecurityPluginGoogle(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipSecurityPluginGoogleCtor)
			{
				skipSecurityPluginGoogleCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.domain = XVar.Clone(var_params["domain"]);
			this.configUrl = new XVar("https://accounts.google.com/.well-known/openid-configuration");
		}
		private XVar getDomainList()
		{
			dynamic domainList = XVar.Array(), result = XVar.Array();
			domainList = XVar.Clone(MVCFunctions.explode(new XVar(","), (XVar)(this.domain)));
			result = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> domain in domainList.GetEnumerator())
			{
				dynamic trimDomain = null;
				trimDomain = XVar.Clone(MVCFunctions.trim((XVar)(domain.Value)));
				if(XVar.Pack(trimDomain))
				{
					result.InitAndSetArrayItem(trimDomain, null);
				}
			}
			return result;
		}
		public override XVar verifyIdToken(dynamic _param_token)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			#endregion

			dynamic allowedDomains = null, payload = XVar.Array();
			payload = XVar.Clone(base.verifyIdToken((XVar)(token)));
			if((XVar)(!(XVar)(payload))  || (XVar)(payload["error"]))
			{
				return payload;
			}
			if((XVar)(payload["iss"] != "https://accounts.google.com")  && (XVar)(payload["iss"] != "accounts.google.com"))
			{
				return false;
			}
			if(payload["aud"] != this.clientId)
			{
				return false;
			}
			allowedDomains = XVar.Clone(this.getDomainList());
			if((XVar)(0 < MVCFunctions.count(allowedDomains))  && (XVar)(!(XVar)(MVCFunctions.in_array((XVar)(payload["hd"]), (XVar)(allowedDomains)))))
			{
				dynamic domains = null;
				domains = XVar.Clone(MVCFunctions.implode(new XVar(", "), (XVar)(allowedDomains)));
				this.var_error = XVar.Clone(MVCFunctions.str_replace(new XVar("%s"), (XVar)(domains), (XVar)(CommonFunctions.mlang_message(new XVar("GOOGLE_DOMAIN")))));
				return false;
			}
			return payload;
		}
		public override XVar hasExternalLogout()
		{
			dynamic config = XVar.Array();
			config = XVar.Clone(this.getConfig());
			return (XVar)(config["revocation_endpoint"] != null)  && (XVar)(this.logOut);
		}
		public override XVar redirectToLogout(dynamic _param_token, dynamic _param_redirectUri)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			dynamic redirectUri = XVar.Clone(_param_redirectUri);
			#endregion

			dynamic config = XVar.Array(), endpoint = null, var_request = null;
			config = XVar.Clone(this.getConfig());
			endpoint = XVar.Clone(config["revocation_endpoint"]);
			var_request = XVar.Clone(new HttpRequest((XVar)(endpoint), new XVar("POST"), (XVar)(XVar.Array()), (XVar)(new XVar("token", token)), (XVar)(new XVar("Cache-Control", "no-store", "Content-Type", "application/x-www-form-urlencoded"))));
			var_request.run();
			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", redirectUri)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public override XVar saveStorageData()
		{
			CommonFunctions.storageSet(new XVar("logout_token_hint"), (XVar)(this.access_token));

			return null;
		}
	}
}
