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
	public partial class AuthorizationAWS4 : XClass
	{
		protected dynamic httpMethod;
		protected dynamic headersMap;
		protected dynamic reqTimestamp;
		protected dynamic bucket;
		protected dynamic accessKey;
		protected dynamic secretKey;
		protected dynamic region;
		protected dynamic awsService;
		protected dynamic queryParameterMap;
		protected dynamic resource;
		protected dynamic payload;
		private dynamic forUrl;
		public AuthorizationAWS4(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.httpMethod = XVar.Clone(var_params["httpMethod"]);
			this.headersMap = XVar.Clone(var_params["headersMap"]);
			this.reqTimestamp = XVar.Clone(var_params["reqTimestamp"]);
			this.bucket = XVar.Clone(var_params["bucket"]);
			this.accessKey = XVar.Clone(var_params["accessKey"]);
			this.secretKey = XVar.Clone(var_params["secretKey"]);
			this.region = XVar.Clone(var_params["region"]);
			this.awsService = XVar.Clone(var_params["awsService"]);
			this.queryParameterMap = XVar.Clone(var_params["queryParameterMap"]);
			this.resource = XVar.Clone(var_params["resource"]);
			this.payload = XVar.Clone(var_params["payload"]);
			this.forUrl = new XVar(false);
		}
		public virtual XVar setQueryParameterMap(dynamic _param_queryParameterMap)
		{
			#region pass-by-value parameters
			dynamic queryParameterMap = XVar.Clone(_param_queryParameterMap);
			#endregion

			this.queryParameterMap = XVar.Clone(queryParameterMap);

			return null;
		}
		public virtual XVar setForUrl(dynamic _param_enabled)
		{
			#region pass-by-value parameters
			dynamic enabled = XVar.Clone(_param_enabled);
			#endregion

			this.forUrl = XVar.Clone(enabled);

			return null;
		}
		public static XVar pathEncode(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic encParts = XVar.Array();
			encParts = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> part in MVCFunctions.explode(new XVar("/"), (XVar)(path)).GetEnumerator())
			{
				encParts.InitAndSetArrayItem(MVCFunctions.RawUrlEncode((XVar)(part.Value)), null);
			}
			return MVCFunctions.implode(new XVar("/"), (XVar)(encParts));
		}
		protected virtual XVar getCanonicalURI()
		{
			return AuthorizationAWS4.pathEncode((XVar)(MVCFunctions.Concat("/", this.resource)));
		}
		protected virtual XVar getCanonicalQueryString()
		{
			dynamic queryParameterList = XVar.Array(), result = XVar.Array();
			queryParameterList = XVar.Clone(MVCFunctions.array_keys((XVar)(this.queryParameterMap)));
			MVCFunctions.sort(ref queryParameterList);
			result = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> key in queryParameterList.GetEnumerator())
			{
				dynamic elem = null, value = null;
				value = XVar.Clone(this.queryParameterMap[key.Value]);
				elem = XVar.Clone(MVCFunctions.Concat(MVCFunctions.RawUrlEncode((XVar)(key.Value)), "=", MVCFunctions.RawUrlEncode((XVar)(value))));
				result.InitAndSetArrayItem(elem, null);
			}
			return MVCFunctions.implode(new XVar("&"), (XVar)(result));
		}
		protected virtual XVar getCanonicalHeaders()
		{
			dynamic canonicalHeaders = XVar.Array(), headersList = XVar.Array();
			headersList = XVar.Clone(MVCFunctions.array_keys((XVar)(this.headersMap)));
			MVCFunctions.sort(ref headersList);
			canonicalHeaders = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> key in headersList.GetEnumerator())
			{
				dynamic value = null;
				value = XVar.Clone(this.headersMap[key.Value]);
				canonicalHeaders.InitAndSetArrayItem(MVCFunctions.Concat(MVCFunctions.strtolower((XVar)(key.Value)), ":", MVCFunctions.trim((XVar)(value))), null);
			}
			return MVCFunctions.Concat(MVCFunctions.implode(new XVar("\n"), (XVar)(canonicalHeaders)), "\n");
		}
		public virtual XVar getSignedHeaders()
		{
			dynamic headersList = XVar.Array();
			headersList = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in this.headersMap.GetEnumerator())
			{
				headersList.InitAndSetArrayItem(MVCFunctions.strtolower((XVar)(value.Key)), null);
			}
			MVCFunctions.sort(ref headersList);
			return MVCFunctions.implode(new XVar(";"), (XVar)(headersList));
		}
		protected virtual XVar getHashedPayload()
		{
			if(XVar.Pack(this.forUrl))
			{
				return "UNSIGNED-PAYLOAD";
			}
			return MVCFunctions.hash_sha256((XVar)(this.payload));
		}
		protected virtual XVar getCanonicalRequest()
		{
			dynamic components = null;
			components = XVar.Clone(new XVar(0, this.httpMethod, 1, this.getCanonicalURI(), 2, this.getCanonicalQueryString(), 3, this.getCanonicalHeaders(), 4, this.getSignedHeaders(), 5, this.getHashedPayload()));
			return MVCFunctions.implode(new XVar("\n"), (XVar)(components));
		}
		protected virtual XVar getDate(dynamic _param_intTimestamp)
		{
			#region pass-by-value parameters
			dynamic intTimestamp = XVar.Clone(_param_intTimestamp);
			#endregion

			return MVCFunctions.substr((XVar)(MVCFunctions.iso8601date_timestamp((XVar)(intTimestamp))), new XVar(0), new XVar(8));
		}
		protected virtual XVar getHashedCanonicalRequest()
		{
			return MVCFunctions.hash_sha256((XVar)(this.getCanonicalRequest()));
		}
		protected virtual XVar getScope()
		{
			dynamic components = null;
			components = XVar.Clone(new XVar(0, this.getDate((XVar)(this.reqTimestamp)), 1, this.region, 2, this.awsService, 3, Constants.AWS4_REQ_POSTFIX));
			return MVCFunctions.implode(new XVar("/"), (XVar)(components));
		}
		protected virtual XVar getStringToSign()
		{
			dynamic components = null;
			components = XVar.Clone(new XVar(0, Constants.AWS4_SIGN_ALGO, 1, MVCFunctions.iso8601date_timestamp((XVar)(this.reqTimestamp)), 2, this.getScope(), 3, this.getHashedCanonicalRequest()));
			return MVCFunctions.implode(new XVar("\n"), (XVar)(components));
		}
		protected virtual XVar getSigningKey()
		{
			dynamic dateKey = null, dateRegionKey = null, dateRegionServiceKey = null, signingKey = null;
			dateKey = XVar.Clone(MVCFunctions.hash_hmac_sha256((XVar)(this.getDate((XVar)(this.reqTimestamp))), (XVar)(MVCFunctions.Concat("AWS4", this.secretKey)), new XVar(true)));
			dateRegionKey = XVar.Clone(MVCFunctions.hash_hmac_sha256((XVar)(this.region), (XVar)(dateKey), new XVar(true)));
			dateRegionServiceKey = XVar.Clone(MVCFunctions.hash_hmac_sha256((XVar)(this.awsService), (XVar)(dateRegionKey), new XVar(true)));
			signingKey = XVar.Clone(MVCFunctions.hash_hmac_sha256(new XVar(Constants.AWS4_REQ_POSTFIX), (XVar)(dateRegionServiceKey), new XVar(true)));
			return signingKey;
		}
		public virtual XVar signString(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.hash_hmac_sha256((XVar)(str), (XVar)(this.getSigningKey()), new XVar(false));
		}
		public virtual XVar getSignature()
		{
			return this.signString((XVar)(this.getStringToSign()));
		}
		public virtual XVar getCredential()
		{
			return MVCFunctions.Concat(this.accessKey, "/", this.getScope());
		}
	}
}
