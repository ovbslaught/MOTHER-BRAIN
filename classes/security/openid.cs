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
	public partial class SecurityPluginOpenId : SecurityPlugin
	{
		public dynamic clientId = XVar.Pack("");
		public dynamic clientSecret = XVar.Pack("");
		public dynamic scope = XVar.Pack("");
		public dynamic configUrl = XVar.Pack("");
		public dynamic nameClaim = XVar.Pack("");
		public dynamic emailClaim = XVar.Pack("");
		public dynamic pictureUrlClaim = XVar.Pack("");
		public dynamic logOut = XVar.Pack(false);
		public dynamic access_token = XVar.Pack("");
		protected static bool skipSecurityPluginOpenIdCtor = false;
		public SecurityPluginOpenId(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipSecurityPluginOpenIdCtor)
			{
				skipSecurityPluginOpenIdCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.clientId = XVar.Clone(var_params["clientId"]);
			this.clientSecret = XVar.Clone(var_params["clientSecret"]);
			this.scope = XVar.Clone(var_params["scope"]);
			this.configUrl = XVar.Clone(var_params["configUrl"]);
			this.nameClaim = XVar.Clone(var_params["nameClaim"]);
			this.emailClaim = XVar.Clone(var_params["emailClaim"]);
			this.pictureUrlClaim = XVar.Clone(var_params["pictureUrlClaim"]);
			this.logOut = XVar.Clone(var_params["logOut"]);
		}
		public override XVar getUserInfo(dynamic _param_id_token)
		{
			#region pass-by-value parameters
			dynamic id_token = XVar.Clone(_param_id_token);
			#endregion

			dynamic payload = XVar.Array(), ret = XVar.Array();
			payload = XVar.Clone(this.verifyIdToken((XVar)(id_token)));
			if(XVar.Pack(payload["error"]))
			{
				this.var_error = XVar.Clone(MVCFunctions.Concat("OpenID security plugin: ", payload["error"], " ", payload["error_description"]));
			}
			if((XVar)(!(XVar)(payload))  || (XVar)(payload["error"]))
			{
				return XVar.Array();
			}
			ret = XVar.Clone(new XVar("id", MVCFunctions.Concat(this.code, payload["sub"])));
			ret.InitAndSetArrayItem(payload, "raw");
			if(XVar.Pack(this.nameClaim))
			{
				ret.InitAndSetArrayItem(MVCFunctions.runner_convert_encoding((XVar)(payload[this.nameClaim]), (XVar)(GlobalVars.cCharset), new XVar("UTF-8")), "name");
			}
			if(XVar.Pack(this.emailClaim))
			{
				ret.InitAndSetArrayItem(payload[this.emailClaim], "email");
			}
			if((XVar)(this.pictureUrlClaim)  && (XVar)(payload[this.pictureUrlClaim]))
			{
				dynamic picResult = XVar.Array();
				picResult = XVar.Clone(MVCFunctions.runner_http_request((XVar)(payload[this.pictureUrlClaim]), new XVar(""), new XVar("GET"), (XVar)(XVar.Array()), new XVar(false)));
				if(XVar.Pack(picResult["content"]))
				{
					ret.InitAndSetArrayItem(picResult["content"], "picture");
				}
			}
			if((XVar)((XVar)(this.nameClaim)  && (XVar)(!(XVar)(ret["name"])))  || (XVar)((XVar)(this.emailClaim)  && (XVar)(!(XVar)(ret["email"]))))
			{
				dynamic claimData = XVar.Array();
				claimData = XVar.Clone(this.getClaimData());
				if((XVar)(claimData)  && (XVar)(!(XVar)(claimData["error"])))
				{
					ret.InitAndSetArrayItem(MVCFunctions.array_merge((XVar)(ret["raw"]), (XVar)(claimData)), "raw");
					if((XVar)(this.nameClaim)  && (XVar)(!(XVar)(ret["name"])))
					{
						ret.InitAndSetArrayItem(MVCFunctions.runner_convert_encoding((XVar)(claimData[this.nameClaim]), (XVar)(GlobalVars.cCharset), new XVar("UTF-8")), "name");
					}
					if((XVar)(this.emailClaim)  && (XVar)(!(XVar)(ret["email"])))
					{
						ret.InitAndSetArrayItem(claimData[this.emailClaim], "email");
					}
				}
			}
			return ret;
		}
		public virtual XVar getClaimData()
		{
			dynamic config = XVar.Array(), headers = null, var_response = XVar.Array();
			config = XVar.Clone(this.getConfig());
			headers = XVar.Clone(new XVar("Authorization", MVCFunctions.Concat("Bearer ", this.access_token)));
			var_response = XVar.Clone(MVCFunctions.runner_http_request((XVar)(config["userinfo_endpoint"]), new XVar(""), new XVar("GET"), (XVar)(headers), new XVar(false)));
			if(XVar.Pack(var_response["error"]))
			{
				return new XVar("error", MVCFunctions.Concat("OpenID security plugin: ", var_response["header"], var_response["content"]));
			}
			return HttpRequest.parseResponseArray((XVar)(var_response));
		}
		public virtual XVar verifyIdToken(dynamic _param_token)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			#endregion

			dynamic config = null, jwk = null, verifiedTokenData = XVar.Array();
			config = XVar.Clone(this.getConfig());
			jwk = XVar.Clone(Security.getOpenIdJWK((XVar)(token), (XVar)(config)));
			verifiedTokenData = XVar.Clone(Security.openIdVerifyToken((XVar)(token), (XVar)(jwk)));
			if(XVar.Pack(!(XVar)(verifiedTokenData)))
			{
				return false;
			}
			return verifiedTokenData["payload"];
		}
		public virtual XVar getConfig()
		{
			dynamic wellKnown = XVar.Array();
			wellKnown = CommonFunctions.storageFindOrCreate(new XVar("openIdConfig"));
			if(XVar.Pack(!(XVar)(wellKnown[this.code])))
			{
				wellKnown.InitAndSetArrayItem(Security.getOpenIdConfiguration((XVar)(this.configUrl)), this.code);
			}
			return wellKnown[this.code];
		}
		public override XVar redirectToLogin()
		{
			dynamic config = XVar.Array(), openIdStateMap = XVar.Array(), state = null, var_request = null;
			openIdStateMap = CommonFunctions.storageFindOrCreate(new XVar("openIdStateMap"));
			state = XVar.Clone(CommonFunctions.generatePassword(new XVar(10)));
			openIdStateMap.InitAndSetArrayItem(this.code, state);
			config = XVar.Clone(this.getConfig());
			var_request = XVar.Clone(new HttpRequest((XVar)(config["authorization_endpoint"]), new XVar("GET"), (XVar)(new XVar("response_type", "code", "scope", this.scope, "client_id", this.clientId, "state", state, "redirect_uri", MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("openidcallback")))))));
			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", var_request.getUrl())));

			return null;
		}
		public virtual XVar getIdToken(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			dynamic config = XVar.Array(), result = XVar.Array(), var_request = null, var_response = XVar.Array();
			config = XVar.Clone(this.getConfig());
			var_request = XVar.Clone(new HttpRequest((XVar)(config["token_endpoint"]), new XVar("POST")));
			var_request.postPayload = XVar.Clone(new XVar("grant_type", "authorization_code", "code", code, "redirect_uri", MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("openidcallback")))));
			var_request.addBasicAuthorization((XVar)(this.clientId), (XVar)(this.clientSecret));
			var_request.headers.InitAndSetArrayItem("application/x-www-form-urlencoded", "Content-Type");
			var_response = XVar.Clone(var_request.run());
			if(XVar.Pack(var_response["error"]))
			{
				this.var_error = new XVar("Unable to obtain authorization token.");
				this.var_error = MVCFunctions.Concat(this.var_error, var_response["header"], var_response["content"]);
				return false;
			}
			result = XVar.Clone(HttpRequest.parseResponseArray((XVar)(var_response)));
			if((XVar)(!(XVar)(result))  || (XVar)(!(XVar)(result["id_token"])))
			{
				this.var_error = new XVar("Unable to parse autorization token.");
				this.var_error = MVCFunctions.Concat(this.var_error, var_response["header"], var_response["content"]);
				return false;
			}
			this.access_token = XVar.Clone(result["access_token"]);
			return result["id_token"];
		}
		public override XVar hasExternalLogout()
		{
			dynamic config = XVar.Array();
			config = XVar.Clone(this.getConfig());
			return (XVar)(config["end_session_endpoint"] != null)  && (XVar)(this.logOut);
		}
		public override XVar redirectToLogout(dynamic _param_token, dynamic _param_redirectUri)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			dynamic redirectUri = XVar.Clone(_param_redirectUri);
			#endregion

			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", this.externalLogoutUrl((XVar)(token), (XVar)(redirectUri)))));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar externalLogoutUrl(dynamic _param_token, dynamic _param_redirectUri)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			dynamic redirectUri = XVar.Clone(_param_redirectUri);
			#endregion

			dynamic config = XVar.Array(), endpoint = null, var_request = null;
			config = XVar.Clone(this.getConfig());
			endpoint = XVar.Clone(config["end_session_endpoint"]);
			var_request = XVar.Clone(new HttpRequest((XVar)(endpoint), new XVar("GET"), (XVar)(new XVar("id_token_hint", token, "post_logout_redirect_uri", redirectUri))));
			return var_request.getUrl();
		}
	}
}
