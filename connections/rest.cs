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
	public partial class RestConnection : XClass
	{
		public dynamic connId;
		public dynamic url;
		public dynamic authType;
		public dynamic dbType;
		public dynamic clientCredentials;
		public dynamic username;
		public dynamic password;
		public dynamic apiKey;
		public dynamic keyLocation;
		public dynamic keyParameter;
		public dynamic authUrl;
		public dynamic tokenUrl;
		public dynamic clientId;
		public dynamic clientSecret;
		public dynamic scope;
		public dynamic sendOauthClientId;
		public dynamic _encryptInfo;
		protected dynamic var_error = XVar.Pack("");
		protected dynamic responseCode = XVar.Pack("");
		protected dynamic var_response;
		protected dynamic authorizationRequest = XVar.Pack(null);
		public RestConnection(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.dbType = new XVar(Constants.nDATABASE_REST);
			this.connId = XVar.Clone(var_params["id"]);
			this.url = XVar.Clone(var_params["url"]);
			this.authType = XVar.Clone(var_params["authType"]);
			this.clientCredentials = XVar.Clone(var_params["clientCredentials"]);
			this.username = XVar.Clone(var_params["username"]);
			this.password = XVar.Clone(var_params["password"]);
			this.apiKey = XVar.Clone(var_params["APIkey"]);
			this.keyLocation = XVar.Clone(var_params["keyLocation"]);
			this.keyParameter = XVar.Clone(var_params["keyParameter"]);
			this.authUrl = XVar.Clone(var_params["authUrl"]);
			this.tokenUrl = XVar.Clone(var_params["tokenUrl"]);
			this.clientId = XVar.Clone(var_params["clientId"]);
			this.clientSecret = XVar.Clone(var_params["clientSecret"]);
			this.scope = XVar.Clone(var_params["scope"]);
			this.sendOauthClientId = XVar.Clone((XVar.Pack(var_params.KeyExists("sendOauthClientId")) ? XVar.Pack(var_params["sendOauthClientId"]) : XVar.Pack(true)));
		}
		public virtual XVar requestJson(dynamic _param_urlRequest, dynamic _param_method, dynamic _param_payload = null, dynamic _param_headers = null, dynamic _param_urlParams = null)
		{
			#region default values
			if(_param_payload as Object == null) _param_payload = new XVar(XVar.Array());
			if(_param_headers as Object == null) _param_headers = new XVar();
			if(_param_urlParams as Object == null) _param_urlParams = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic urlRequest = XVar.Clone(_param_urlRequest);
			dynamic method = XVar.Clone(_param_method);
			dynamic payload = XVar.Clone(_param_payload);
			dynamic headers = XVar.Clone(_param_headers);
			dynamic urlParams = XVar.Clone(_param_urlParams);
			#endregion

			dynamic var_request = null;
			var_request = XVar.Clone(new HttpRequest((XVar)(MVCFunctions.Concat(this.url, urlRequest)), (XVar)(method), (XVar)(urlParams), (XVar)(payload), (XVar)(headers)));
			return this.sendJsonRequest((XVar)(var_request));
		}
		public virtual XVar createRequest(dynamic _param_urlRequest, dynamic _param_method = null)
		{
			#region default values
			if(_param_method as Object == null) _param_method = new XVar("GET");
			#endregion

			#region pass-by-value parameters
			dynamic urlRequest = XVar.Clone(_param_urlRequest);
			dynamic method = XVar.Clone(_param_method);
			#endregion

			return new HttpRequest((XVar)(MVCFunctions.Concat(this.url, urlRequest)), (XVar)(method));
		}
		public virtual XVar sendJsonRequest(dynamic _param_request)
		{
			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			#endregion

			dynamic content = null, obj = null, res = XVar.Array();
			res = this.requestWithAuth((XVar)(var_request));
			if(XVar.Pack(res["error"]))
			{
				return false;
			}
			content = XVar.Clone(res["content"]);
			if(content == XVar.Pack(""))
			{
				return "";
			}
			obj = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(content)));
			if((XVar)((XVar)(MVCFunctions.is_array((XVar)(obj)))  && (XVar)(MVCFunctions.count(obj) == 0))  && (XVar)(MVCFunctions.trim((XVar)(content)) != "[]"))
			{
				this.var_error = XVar.Clone(MVCFunctions.Concat("Unable to parse JSON result\n\n", content));
				return false;
			}
			return obj;
		}
		public virtual XVar oauthTokenRequestParams()
		{
			if(XVar.Pack(this.clientCredentials))
			{
				return new XVar("grant_type", "client_credentials", "scope", this.scope, "client_secret", this.clientSecret, "client_id", this.clientId);
			}
			return new XVar("grant_type", "password", "username", this.username, "password", this.password, "scope", this.scope);
		}
		public virtual XVar getOauthAccessToken()
		{
			dynamic oauthToken = XVar.Array();
			oauthToken = XVar.Clone(this.getOauthToken());
			if(XVar.Pack(oauthToken))
			{
				oauthToken = XVar.Clone(this.checkRefreshOauthToken((XVar)(oauthToken)));
			}
			if(XVar.Pack(!(XVar)(oauthToken)))
			{
				if(XVar.Equals(XVar.Pack(this.authType), XVar.Pack("oauth")))
				{
					this.setAuthorizationRequest((XVar)(this.createUserAuthRequest()));
					return false;
				}
				else
				{
					if(XVar.Equals(XVar.Pack(this.authType), XVar.Pack("oauthserver")))
					{
						oauthToken = XVar.Clone(this.requestOauthToken((XVar)(this.createOauthTokenRequest((XVar)(this.oauthTokenRequestParams())))));
						this.setOauthToken((XVar)(oauthToken));
					}
				}
			}
			return oauthToken["access_token"];
		}
		public virtual XVar setOauthToken(dynamic _param_token)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			#endregion

			dynamic sessionKey = null;
			sessionKey = XVar.Clone(MVCFunctions.Concat("oauthToken_", this.connId));
			if(XVar.Pack(!(XVar)(token)))
			{
				XSession.Session.Remove(sessionKey);
			}
			else
			{
				XSession.Session[sessionKey] = MVCFunctions.runner_json_encode((XVar)(token));
			}

			return null;
		}
		public virtual XVar getOauthToken()
		{
			dynamic sessionKey = null;
			sessionKey = XVar.Clone(MVCFunctions.Concat("oauthToken_", this.connId));
			if(XVar.Pack(!(XVar)(XSession.Session[sessionKey])))
			{
				return false;
			}
			return MVCFunctions.runner_json_decode((XVar)(XSession.Session[sessionKey]));
		}
		public virtual XVar requestAddAuth(dynamic _param_request)
		{
			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			#endregion

			if((XVar)(XVar.Equals(XVar.Pack(this.authType), XVar.Pack("oauth")))  || (XVar)(XVar.Equals(XVar.Pack(this.authType), XVar.Pack("oauthserver"))))
			{
				dynamic accessToken = null;
				accessToken = XVar.Clone(this.getOauthAccessToken());
				if(XVar.Pack(!(XVar)(accessToken)))
				{
					this.var_error = XVar.Clone((XVar.Pack(this.var_error) ? XVar.Pack(this.var_error) : XVar.Pack("Not autorized yet")));
					return false;
				}
				var_request.headers.InitAndSetArrayItem(MVCFunctions.Concat("Bearer ", accessToken), "Authorization");
			}
			else
			{
				if(XVar.Equals(XVar.Pack(this.authType), XVar.Pack("basic")))
				{
					var_request.addBasicAuthorization((XVar)(this.username), (XVar)(this.password));
				}
				else
				{
					if(XVar.Equals(XVar.Pack(this.authType), XVar.Pack("api")))
					{
						if(this.keyLocation == Constants.rklHEADER)
						{
							var_request.headers.InitAndSetArrayItem(this.apiKey, this.keyParameter);
						}
						else
						{
							if(this.keyLocation == Constants.rklURL)
							{
								var_request.urlParams.InitAndSetArrayItem(this.apiKey, this.keyParameter);
							}
							else
							{
								var_request.postPayload.InitAndSetArrayItem(this.apiKey, this.keyParameter);
							}
						}
					}
					else
					{
						if(XVar.Equals(XVar.Pack(this.authType), XVar.Pack("custom")))
						{
							dynamic handler = null;
							handler = XVar.Clone(MVCFunctions.Concat("rest_authenticate", this.connId));
							GlobalVars.globalEvents.Invoke(handler, this, new XVar("data"), (XVar)(var_request), new XVar(null));
						}
					}
				}
			}
			if(this.getAuthorizationRequest() != null)
			{
				this.var_error = new XVar("Authorization required");
				return false;
			}
			return true;
		}
		public virtual XVar requestWithAuth(dynamic _param_request, dynamic _param_authenticateOnly = null)
		{
			#region default values
			if(_param_authenticateOnly as Object == null) _param_authenticateOnly = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			dynamic authenticateOnly = XVar.Clone(_param_authenticateOnly);
			#endregion

			dynamic authResult = null, requestKey = null, ret = XVar.Array();
			authResult = XVar.Clone(this.requestAddAuth((XVar)(var_request)));
			if(XVar.Pack(!(XVar)(authResult)))
			{
				return false;
			}
			if(XVar.Pack(authenticateOnly))
			{
				return "ok";
			}
			requestKey = XVar.Clone(var_request.getRequestKey());
			if((XVar)(!XVar.Equals(XVar.Pack(requestKey), XVar.Pack(null)))  && (XVar)(GlobalVars.restResultCache.KeyExists(requestKey)))
			{
				return GlobalVars.restResultCache[requestKey];
			}
			ret = XVar.Clone(this.doRequest((XVar)(var_request)));
			if(XVar.Pack(ret["error"]))
			{
				return ret;
			}
			if(!XVar.Equals(XVar.Pack(requestKey), XVar.Pack(null)))
			{
				GlobalVars.restResultCache.InitAndSetArrayItem(ret, requestKey);
			}
			return ret;
		}
		public virtual XVar doRequest(dynamic _param_request)
		{
			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(var_request.run());
			if(XVar.Pack(ret["error"]))
			{
				this.var_error = XVar.Clone(ret["errorMessage"]);
			}
			return ret;
		}
		public virtual XVar lastError()
		{
			return this.var_error;
		}
		public virtual XVar createUserAuthRequest(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic state = null, var_request = null;
			var_request = XVar.Clone(new HttpRequest((XVar)(this.authUrl), new XVar("GET")));
			state = XVar.Clone(CommonFunctions.generatePassword(new XVar(30)));
			var_request.urlParams = XVar.Clone(new XVar("response_type", "code", "approval_prompt", "auto", "redirect_uri", MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("oauthcallback"))), "client_id", this.clientId, "state", state));
			if(XVar.Pack(var_params))
			{
				foreach (KeyValuePair<XVar, dynamic> p in var_params.GetEnumerator())
				{
					var_request.urlParams.InitAndSetArrayItem(p.Value, p.Key);
				}
			}
			if(XVar.Pack(this.scope))
			{
				var_request.urlParams.InitAndSetArrayItem(this.scope, "scope");
			}
			return var_request;
		}
		public virtual XVar requestOauthToken(dynamic _param_request)
		{
			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			#endregion

			dynamic accessToken = XVar.Array(), result = XVar.Array(), var_response = XVar.Array();
			var_response = XVar.Clone(this.doRequest((XVar)(var_request)));
			if(XVar.Pack(var_response["error"]))
			{
				return false;
			}
			result = XVar.Clone(HttpRequest.parseResponseArray((XVar)(var_response)));
			if(XVar.Pack(!(XVar)(result["access_token"])))
			{
				this.var_error = XVar.Clone(MVCFunctions.Concat(var_response["header"], var_response["content"]));
				return false;
			}
			accessToken = XVar.Clone(new XVar("access_token", result["access_token"], "refresh_token", result["refresh_token"]));
			if((XVar)(!(XVar)(result["refresh_token"]))  && (XVar)(var_request.postPayload["refresh_token"]))
			{
				accessToken.InitAndSetArrayItem(var_request.postPayload["refresh_token"], "refresh_token");
			}
			if(XVar.Pack(result["expires_in"]))
			{
				accessToken.InitAndSetArrayItem(MVCFunctions.time() + result["expires_in"], "expires");
			}
			else
			{
				if(XVar.Pack(result["expires"]))
				{
					dynamic oauth2InceptionDate = null;
					accessToken.InitAndSetArrayItem((int)result["expires"], "expires");
					oauth2InceptionDate = new XVar(1349067600);
					if(accessToken["expires"] <= oauth2InceptionDate)
					{
						accessToken["expires"] += MVCFunctions.time();
					}
				}
			}
			return accessToken;
		}
		protected virtual XVar createOauthTokenRequest(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic var_request = null;
			var_request = XVar.Clone(new HttpRequest((XVar)(this.tokenUrl), new XVar("POST")));
			var_request.postPayload = XVar.Clone(var_params);
			var_request.addBasicAuthorization((XVar)(this.clientId), (XVar)(this.clientSecret));
			var_request.headers.InitAndSetArrayItem("application/x-www-form-urlencoded", "Content-Type");
			return var_request;
		}
		public virtual XVar validateCode(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			if(XVar.Pack(!(XVar)(code)))
			{
				this.var_error = new XVar("Provider returned no authorization code");
				return false;
			}
			if(XVar.Equals(XVar.Pack(this.authType), XVar.Pack("oauth")))
			{
				return this.validateOauthCode((XVar)(code));
			}
			else
			{
				if(XVar.Equals(XVar.Pack(this.authType), XVar.Pack("custom")))
				{
					dynamic handler = null;
					handler = XVar.Clone(MVCFunctions.Concat("rest_authenticate", this.connId));
					return GlobalVars.globalEvents.Invoke(handler, this, new XVar("validate"), new XVar(null), (XVar)(code));
				}
			}

			return null;
		}
		public virtual XVar validateOauthCode(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			dynamic accessToken = null, var_params = XVar.Array();
			var_params = XVar.Clone(new XVar("grant_type", "authorization_code", "code", code, "redirect_uri", MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("oauthcallback")))));
			if(XVar.Pack(this.sendOauthClientId))
			{
				var_params.InitAndSetArrayItem(this.clientId, "client_id");
			}
			accessToken = XVar.Clone(this.requestOauthToken((XVar)(this.createOauthTokenRequest((XVar)(var_params)))));
			if(XVar.Pack(!(XVar)(accessToken)))
			{
				return false;
			}
			this.setOauthToken((XVar)(accessToken));
			return true;
		}
		protected virtual XVar checkRefreshOauthToken(dynamic _param_accessToken)
		{
			#region pass-by-value parameters
			dynamic accessToken = XVar.Clone(_param_accessToken);
			#endregion

			if(XVar.Pack(this.accessTokenExpired((XVar)(accessToken))))
			{
				dynamic var_params = null;
				if(XVar.Pack(!(XVar)(accessToken["refresh_token"])))
				{
					return null;
				}
				var_params = XVar.Clone(new XVar("grant_type", "refresh_token", "refresh_token", accessToken["refresh_token"]));
				accessToken = XVar.Clone(this.requestOauthToken((XVar)(this.createOauthTokenRequest((XVar)(var_params)))));
				this.setOauthToken((XVar)(accessToken));
			}
			return accessToken;
		}
		public virtual XVar accessTokenExpired(dynamic _param_accessToken)
		{
			#region pass-by-value parameters
			dynamic accessToken = XVar.Clone(_param_accessToken);
			#endregion

			dynamic timeAllowance = null;
			timeAllowance = new XVar(2);
			return (XVar)(accessToken["expires"])  && (XVar)(accessToken["expires"] < MVCFunctions.time() - timeAllowance);
		}
		public virtual XVar isEncryptionByPHPEnabled()
		{
			return false;
		}
		public virtual XVar dbBased()
		{
			return false;
		}
		public virtual XVar close()
		{

			return null;
		}
		public virtual XVar checkIfJoinSubqueriesOptimized()
		{
			return false;
		}
		public virtual XVar checkDBSubqueriesSupport()
		{
			return false;
		}
		public virtual XVar setAuthorizationRequest(dynamic _param_request)
		{
			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			#endregion

			this.authorizationRequest = XVar.Clone(var_request);
			if(XVar.Pack(var_request.urlParams["state"]))
			{
				if(XVar.Pack(!(XVar)(XSession.Session["oauth2statemap"])))
				{
					XSession.Session["oauth2statemap"] = XVar.Array();
				}
				XSession.Session.InitAndSetArrayItem(this.connId, "oauth2statemap", var_request.urlParams["state"]);
			}

			return null;
		}
		public virtual XVar getAuthorizationRequest()
		{
			return this.authorizationRequest;
		}
		public virtual XVar checkAuthorization()
		{
			dynamic result = null;
			if(XVar.Pack(this.authorizationRequest))
			{
				return false;
			}
			result = XVar.Clone(this.requestWithAuth((XVar)(new HttpRequest(new XVar(""))), new XVar(true)));
			return !(XVar)(!(XVar)(result));
		}
	}
}
