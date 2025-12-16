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
	public partial class HttpRequest : XClass
	{
		public dynamic url = XVar.Pack("");
		public dynamic method = XVar.Pack("GET");
		public dynamic headers = XVar.Array();
		public dynamic urlParams = XVar.Array();
		public dynamic postPayload = XVar.Array();
		public dynamic body = XVar.Pack(null);
		public HttpRequest(dynamic _param_url, dynamic _param_method = null, dynamic _param_urlParams = null, dynamic _param_postPayload = null, dynamic _param_headers = null)
		{
			#region default values
			if(_param_method as Object == null) _param_method = new XVar("GET");
			if(_param_urlParams as Object == null) _param_urlParams = new XVar(XVar.Array());
			if(_param_postPayload as Object == null) _param_postPayload = new XVar(XVar.Array());
			if(_param_headers as Object == null) _param_headers = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic url = XVar.Clone(_param_url);
			dynamic method = XVar.Clone(_param_method);
			dynamic urlParams = XVar.Clone(_param_urlParams);
			dynamic postPayload = XVar.Clone(_param_postPayload);
			dynamic headers = XVar.Clone(_param_headers);
			#endregion

			this.url = XVar.Clone(url);
			this.urlParams = XVar.Clone(urlParams);
			this.postPayload = XVar.Clone(postPayload);
			this.method = XVar.Clone(method);
			this.headers = XVar.Clone(headers);
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(this.postPayload)))))
			{
				this.postPayload = XVar.Clone(XVar.Array());
				this.body = XVar.Clone(postPayload);
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(this.urlParams)))))
			{
				this.urlParams = XVar.Clone(XVar.Array());
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(this.headers)))))
			{
				this.headers = XVar.Clone(XVar.Array());
			}
		}
		public virtual XVar getUrl()
		{
			return CommonFunctions.prepareUrl((XVar)(this.url), (XVar)(this.urlParams));
		}
		public virtual XVar addBasicAuthorization(dynamic _param_username, dynamic _param_password)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			#endregion

			this.headers.InitAndSetArrayItem(MVCFunctions.Concat("Basic ", MVCFunctions.base64_encode((XVar)(MVCFunctions.Concat(username, ":", password)))), "Authorization");

			return null;
		}
		public virtual XVar getRequestKey()
		{
			if((XVar)((XVar)(this.postPayload)  || (XVar)(this.body))  || (XVar)(this.method != "GET"))
			{
				return null;
			}
			return this.getUrl();
		}
		public virtual XVar prepareRequestBody()
		{
			dynamic cTypeParts = XVar.Array(), contentType = null, var_type = null;
			if(!XVar.Equals(XVar.Pack(this.body), XVar.Pack(null)))
			{
				return this.body;
			}
			if(XVar.Pack(!(XVar)(this.postPayload)))
			{
				return "";
			}
			contentType = XVar.Clone(this.getHeader(new XVar("Content-Type")));
			cTypeParts = XVar.Clone(MVCFunctions.explode(new XVar(";"), (XVar)(contentType)));
			var_type = new XVar("");
			if(0 < MVCFunctions.count(cTypeParts))
			{
				var_type = XVar.Clone(MVCFunctions.strtolower((XVar)(MVCFunctions.trim((XVar)(cTypeParts[0])))));
			}
			if((XVar)(!(XVar)(var_type))  || (XVar)(var_type == "application/x-www-form-urlencoded"))
			{
				if(XVar.Pack(!(XVar)(var_type)))
				{
					this.setHeader(new XVar("Content-Type"), new XVar("application/x-www-form-urlencoded"));
				}
				return CommonFunctions.prepareUrlQuery((XVar)(this.postPayload));
			}
			if((XVar)(var_type == "application/json")  || (XVar)(var_type == "application/json-patch+json"))
			{
				return MVCFunctions.runner_json_encode((XVar)(this.postPayload));
			}
			if(var_type == "multipart/form-data")
			{
				dynamic bodyParts = XVar.Array(), boundary = null;
				boundary = XVar.Clone(CommonFunctions.generatePassword(new XVar(40)));
				this.setHeader(new XVar("Content-Type"), (XVar)(MVCFunctions.Concat(var_type, ";boundary=", boundary)));
				bodyParts = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> value in this.postPayload.GetEnumerator())
				{
					bodyParts.InitAndSetArrayItem(MVCFunctions.Concat("--", boundary, "\r\nContent-disposition: form-data; name=\"", value.Key, "\"\r\n\r\n", value.Value, "\r\n"), null);
				}
				bodyParts.InitAndSetArrayItem(MVCFunctions.Concat("--", boundary, "--\r\n"), null);
				return MVCFunctions.implode(new XVar(""), (XVar)(bodyParts));
			}
			return "";
		}
		public virtual XVar run()
		{
			dynamic body = null, bodyLength = null, ret = XVar.Array(), url = null;
			url = XVar.Clone(CommonFunctions.prepareUrl((XVar)(this.url), (XVar)(this.urlParams)));
			body = XVar.Clone(this.prepareRequestBody());
			bodyLength = XVar.Clone(MVCFunctions.strlen_bin((XVar)(body)));
			if(XVar.Pack(bodyLength))
			{
				this.headers.InitAndSetArrayItem(bodyLength, "Content-Length");
			}
			if(XVar.Pack(GlobalVars.dDebug))
			{
				MVCFunctions.Echo("<pre>");
				MVCFunctions.Echo("HTTP request:");
				MVCFunctions.Echo("\nurl:");
				MVCFunctions.print_r((XVar)(this.url));
				MVCFunctions.Echo("\nmethod:");
				MVCFunctions.print_r((XVar)(this.method));
				MVCFunctions.Echo("\nheaders:");
				MVCFunctions.print_r((XVar)(this.headers));
				MVCFunctions.Echo("\nurl params:");
				MVCFunctions.print_r((XVar)(this.urlParams));
				MVCFunctions.Echo("\nbody:");
				MVCFunctions.print_r((XVar)(this.body));
				MVCFunctions.Echo("</pre>");
			}
			ret = XVar.Clone(MVCFunctions.runner_http_request((XVar)(url), (XVar)(body), (XVar)(this.method), (XVar)(this.headers)));
			if(XVar.Pack(GlobalVars.dDebug))
			{
				MVCFunctions.Echo("HTTP response:");
				MVCFunctions.print_r((XVar)(ret));
			}
			if(XVar.Pack(ret["error"]))
			{
				ret.InitAndSetArrayItem(ret["error"], "errorMessage");
				ret.InitAndSetArrayItem(true, "error");
			}
			if((XVar)(ret["responseCode"])  && (XVar)((XVar)(ret["responseCode"] < 200)  || (XVar)(300 <= ret["responseCode"])))
			{
				ret.InitAndSetArrayItem(true, "error");
				ret.InitAndSetArrayItem(MVCFunctions.Concat("HTTP error ", ret["responseCode"], "\r\n.", ret["content"]), "errorMessage");
			}
			return ret;
		}
		public static XVar parseResponseArray(dynamic _param_response)
		{
			#region pass-by-value parameters
			dynamic var_response = XVar.Clone(_param_response);
			#endregion

			dynamic content = null, headers = XVar.Array(), obj = null, urlencoded = null;
			headers = XVar.Clone(MVCFunctions.explode(new XVar("\r\n"), (XVar)(var_response["header"])));
			urlencoded = new XVar(false);
			foreach (KeyValuePair<XVar, dynamic> h in headers.GetEnumerator())
			{
				if((XVar)(XVar.Equals(XVar.Pack(MVCFunctions.stripos((XVar)(h.Value), new XVar("Content-Type:"))), XVar.Pack(0)))  && (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.stripos((XVar)(h.Value), new XVar("urlencoded"))), XVar.Pack(false))))
				{
					urlencoded = new XVar(true);
					break;
				}
			}
			content = XVar.Clone(var_response["content"]);
			if(XVar.Pack(urlencoded))
			{
				dynamic result = null;
				result = XVar.Clone(XVar.Array());
				result = XVar.Clone(MVCFunctions.parseQueryString((XVar)(content)));
				return result;
			}
			obj = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(content)));
			if((XVar)((XVar)(MVCFunctions.is_array((XVar)(obj)))  && (XVar)(MVCFunctions.count(obj) == 0))  && (XVar)(MVCFunctions.trim((XVar)(content)) != "[]"))
			{
				return null;
			}
			return obj;
		}
		public static XVar parseHeaders(dynamic var_response)
		{
			dynamic headers = XVar.Array(), ret = XVar.Array();
			headers = XVar.Clone(MVCFunctions.explode(new XVar("\r\n"), (XVar)(var_response["header"])));
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> h in headers.GetEnumerator())
			{
				dynamic pos = null;
				pos = XVar.Clone(MVCFunctions.strpos((XVar)(h.Value), new XVar(":")));
				if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
				{
					continue;
				}
				ret.InitAndSetArrayItem(MVCFunctions.trim((XVar)(MVCFunctions.substr((XVar)(h.Value), (XVar)(pos + 1)))), MVCFunctions.strtolower((XVar)(MVCFunctions.substr((XVar)(h.Value), new XVar(0), (XVar)(pos)))));
			}
			return ret;
		}
		public virtual XVar getHeader(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return CommonFunctions.getArrayElementNC((XVar)(this.headers), (XVar)(name));
		}
		public virtual XVar setHeader(dynamic _param_name, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic header = null;
			header = XVar.Clone(CommonFunctions.getArrayKeyNC((XVar)(this.headers), (XVar)(name)));
			if(XVar.Pack(!(XVar)(header)))
			{
				header = XVar.Clone(name);
			}
			this.headers.InitAndSetArrayItem(value, header);

			return null;
		}
	}
}
