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
	public partial class S3FileSystem : RunnerFileSystem
	{
		public dynamic accessKey;
		public dynamic secretKey;
		public dynamic bucket;
		public dynamic region;
		public dynamic path;
		protected dynamic _fileInfoCache = XVar.Array();
		protected static bool skipS3FileSystemCtor = false;
		public S3FileSystem(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipS3FileSystemCtor)
			{
				skipS3FileSystemCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.accessKey = XVar.Clone(var_params["accessKey"]);
			this.secretKey = XVar.Clone(var_params["secretKey"]);
			this.bucket = XVar.Clone(var_params["bucket"]);
			this.region = XVar.Clone(var_params["region"]);
			this.path = XVar.Clone(this.prepareFilePath((XVar)(var_params["path"])));
		}
		protected virtual XVar prepareFilePath(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			return S3FileSystem.normalizePath((XVar)(path));
		}
		protected virtual XVar urlPostfix()
		{
			return ".s3.amazonaws.com";
		}
		protected virtual XVar host()
		{
			return MVCFunctions.Concat(this.bucket, this.urlPostfix());
		}
		protected virtual XVar endpoint()
		{
			return MVCFunctions.Concat("https://", this.host());
		}
		protected virtual XVar setRequiredHeaders(dynamic _param_request, dynamic _param_reqTimestamp)
		{
			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			dynamic reqTimestamp = XVar.Clone(_param_reqTimestamp);
			#endregion

			var_request.headers.InitAndSetArrayItem(this.host(), "host");
			var_request.headers.InitAndSetArrayItem(MVCFunctions.iso8601date_timestamp((XVar)(reqTimestamp)), "x-amz-date");
			var_request.headers.InitAndSetArrayItem(MVCFunctions.hash_sha256((XVar)(var_request.body)), "x-amz-content-sha256");
			var_request.headers.InitAndSetArrayItem("text/plain", "accept");

			return null;
		}
		protected virtual XVar saveFile(dynamic _param_data, dynamic _param_resourceKey)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic resourceKey = XVar.Clone(_param_resourceKey);
			#endregion

			dynamic reqTimestamp = null, url = null, var_request = null, var_response = XVar.Array();
			reqTimestamp = XVar.Clone(MVCFunctions.time());
			url = XVar.Clone(MVCFunctions.Concat(this.endpoint(), "/", AuthorizationAWS4.pathEncode((XVar)(resourceKey))));
			var_request = XVar.Clone(new HttpRequest((XVar)(url), new XVar("PUT")));
			var_request.body = XVar.Clone(data);
			this.setRequiredHeaders((XVar)(var_request), (XVar)(reqTimestamp));
			var_request.headers.InitAndSetArrayItem(MVCFunctions.base64_encode((XVar)(MVCFunctions.md5((XVar)(var_request.body), new XVar(true)))), "content-md5");
			var_request.headers.InitAndSetArrayItem(MVCFunctions.strlen_bin((XVar)(var_request.body)), "content-length");
			this.addHeaderAuthentication((XVar)(var_request), (XVar)(resourceKey), (XVar)(reqTimestamp));
			var_response = XVar.Clone(var_request.run());
			if(var_response["responseCode"] == 200)
			{
				return resourceKey;
			}
			return null;
		}
		public override XVar saveUploadedFile(dynamic _param_uploadFile, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic uploadFile = XVar.Clone(_param_uploadFile);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic data = null, resourceKey = null, uniqueFilename = null;
			uniqueFilename = XVar.Clone(this.tryCreateUniqueFile((XVar)(userFilename), (XVar)(this.path)));
			if(XVar.Pack(!(XVar)(uniqueFilename)))
			{
				this.setLastError((XVar)(MVCFunctions.Concat("Unable to get unique file name for ", userFilename, " at ", this.path)));
				return null;
			}
			data = XVar.Clone(RunnerFileSystem.uploadedFileContent((XVar)(uploadFile)));
			resourceKey = XVar.Clone(MVCFunctions.Concat(this.path, uniqueFilename));
			return this.saveFile((XVar)(data), (XVar)(resourceKey));
		}
		public override XVar delete(dynamic _param_resourceKey)
		{
			#region pass-by-value parameters
			dynamic resourceKey = XVar.Clone(_param_resourceKey);
			#endregion

			dynamic reqTimestamp = null, url = null, var_request = null;
			reqTimestamp = XVar.Clone(MVCFunctions.time());
			url = XVar.Clone(MVCFunctions.Concat(this.endpoint(), "/", AuthorizationAWS4.pathEncode((XVar)(resourceKey))));
			var_request = XVar.Clone(new HttpRequest((XVar)(url), new XVar("DELETE")));
			this.setRequiredHeaders((XVar)(var_request), (XVar)(reqTimestamp));
			this.addHeaderAuthentication((XVar)(var_request), (XVar)(resourceKey), (XVar)(reqTimestamp));
			var_request.run();

			return null;
		}
		public override XVar redirectToFile(dynamic _param_resourceKey, dynamic _param_thumbnail)
		{
			#region pass-by-value parameters
			dynamic resourceKey = XVar.Clone(_param_resourceKey);
			dynamic thumbnail = XVar.Clone(_param_thumbnail);
			#endregion

			dynamic downloadUrl = null, parts = XVar.Array(), reqTimestamp = null, url = null, var_request = null;
			reqTimestamp = XVar.Clone(MVCFunctions.time());
			url = XVar.Clone(MVCFunctions.Concat(this.endpoint(), "/", AuthorizationAWS4.pathEncode((XVar)(resourceKey))));
			var_request = XVar.Clone(new HttpRequest((XVar)(url), new XVar("GET")));
			var_request.headers.InitAndSetArrayItem(this.host(), "host");
			var_request.urlParams.InitAndSetArrayItem(MVCFunctions.iso8601date_timestamp((XVar)(reqTimestamp)), "X-Amz-Date");
			var_request.urlParams.InitAndSetArrayItem(Constants.S3_EXPIRATION_TIME_SECONDS, "X-Amz-Expires");
			this.addUrlAuthentication((XVar)(var_request), (XVar)(resourceKey), (XVar)(reqTimestamp));
			downloadUrl = XVar.Clone(MVCFunctions.Concat(this.endpoint(), "/", AuthorizationAWS4.pathEncode((XVar)(resourceKey)), "?"));
			parts = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in var_request.urlParams.GetEnumerator())
			{
				parts.InitAndSetArrayItem(MVCFunctions.Concat(value.Key, "=", value.Value), null);
			}
			downloadUrl = MVCFunctions.Concat(downloadUrl, MVCFunctions.implode(new XVar("&"), (XVar)(parts)));
			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", downloadUrl)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public override XVar getFileInfo(dynamic _param_resourceKey)
		{
			#region pass-by-value parameters
			dynamic resourceKey = XVar.Clone(_param_resourceKey);
			#endregion

			dynamic headers = XVar.Array(), reqTimestamp = null, result = XVar.Array(), ret = null, url = null, var_request = null;
			if(XVar.Pack(this._fileInfoCache[resourceKey]))
			{
				return this._fileInfoCache[resourceKey];
			}
			reqTimestamp = XVar.Clone(MVCFunctions.time());
			url = XVar.Clone(MVCFunctions.Concat(this.endpoint(), "/", AuthorizationAWS4.pathEncode((XVar)(resourceKey))));
			var_request = XVar.Clone(new HttpRequest((XVar)(url), new XVar("HEAD")));
			this.setRequiredHeaders((XVar)(var_request), (XVar)(reqTimestamp));
			this.addHeaderAuthentication((XVar)(var_request), (XVar)(resourceKey), (XVar)(reqTimestamp));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(result["error"]))
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return null;
			}
			if(result["responseCode"] != 200)
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return null;
			}
			headers = XVar.Clone(HttpRequest.parseHeaders((XVar)(result)));
			ret = XVar.Clone(new XVar("fullPath", resourceKey, "size", headers["content-length"], "raw", result, "returnContent", false));
			this._fileInfoCache.InitAndSetArrayItem(ret, resourceKey);
			return ret;
		}
		public override XVar moveFile(dynamic _param_file, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion


			return null;
		}
		protected virtual XVar getAuthObject(dynamic _param_request, dynamic _param_resource, dynamic _param_reqTimestamp)
		{
			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			dynamic resource = XVar.Clone(_param_resource);
			dynamic reqTimestamp = XVar.Clone(_param_reqTimestamp);
			#endregion

			dynamic var_params = XVar.Array();
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(var_request.method, "httpMethod");
			var_params.InitAndSetArrayItem(var_request.headers, "headersMap");
			var_params.InitAndSetArrayItem(var_request.urlParams, "queryParameterMap");
			var_params.InitAndSetArrayItem(this.accessKey, "accessKey");
			var_params.InitAndSetArrayItem(this.secretKey, "secretKey");
			var_params.InitAndSetArrayItem(Constants.S3_AWS_SERVICE, "awsService");
			var_params.InitAndSetArrayItem(this.region, "region");
			var_params.InitAndSetArrayItem(var_request.body, "payload");
			var_params.InitAndSetArrayItem(reqTimestamp, "reqTimestamp");
			var_params.InitAndSetArrayItem(this.bucket, "bucket");
			var_params.InitAndSetArrayItem(resource, "resource");
			return new AuthorizationAWS4((XVar)(var_params));
		}
		protected virtual XVar addHeaderAuthentication(dynamic _param_request, dynamic _param_resource, dynamic _param_reqTimestamp)
		{
			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			dynamic resource = XVar.Clone(_param_resource);
			dynamic reqTimestamp = XVar.Clone(_param_reqTimestamp);
			#endregion

			dynamic auth = null, components = XVar.Array();
			auth = XVar.Clone(this.getAuthObject((XVar)(var_request), (XVar)(resource), (XVar)(reqTimestamp)));
			components = XVar.Clone(XVar.Array());
			components.InitAndSetArrayItem(MVCFunctions.Concat("Credential=", auth.getCredential()), null);
			components.InitAndSetArrayItem(MVCFunctions.Concat("SignedHeaders=", auth.getSignedHeaders()), null);
			components.InitAndSetArrayItem(MVCFunctions.Concat("Signature=", auth.getSignature()), null);
			var_request.headers.InitAndSetArrayItem(MVCFunctions.Concat(Constants.AWS4_SIGN_ALGO, " ", MVCFunctions.implode(new XVar(","), (XVar)(components))), "Authorization");

			return null;
		}
		protected virtual XVar addUrlAuthentication(dynamic _param_request, dynamic _param_resource, dynamic _param_reqTimestamp)
		{
			#region pass-by-value parameters
			dynamic var_request = XVar.Clone(_param_request);
			dynamic resource = XVar.Clone(_param_resource);
			dynamic reqTimestamp = XVar.Clone(_param_reqTimestamp);
			#endregion

			dynamic auth = null;
			auth = XVar.Clone(this.getAuthObject((XVar)(var_request), (XVar)(resource), (XVar)(reqTimestamp)));
			auth.setForUrl(new XVar(true));
			var_request.urlParams.InitAndSetArrayItem(Constants.AWS4_SIGN_ALGO, "X-Amz-Algorithm");
			var_request.urlParams.InitAndSetArrayItem(auth.getCredential(), "X-Amz-Credential");
			var_request.urlParams.InitAndSetArrayItem(auth.getSignedHeaders(), "X-Amz-SignedHeaders");
			auth.setQueryParameterMap((XVar)(var_request.urlParams));
			var_request.urlParams.InitAndSetArrayItem(auth.getSignature(), "X-Amz-Signature");

			return null;
		}
		public static XVar normalizePath(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic lastSymbol = null;
			if(XVar.Pack(!(XVar)(path)))
			{
				return "";
			}
			lastSymbol = XVar.Clone(MVCFunctions.substr((XVar)(path), (XVar)(MVCFunctions.strlen((XVar)(path)) - 1), new XVar(1)));
			if(lastSymbol != "/")
			{
				path = MVCFunctions.Concat(path, "/");
			}
			if(MVCFunctions.substr((XVar)(path), new XVar(0), new XVar(1)) == "/")
			{
				path = XVar.Clone(MVCFunctions.substr((XVar)(path), new XVar(1)));
			}
			return path;
		}
		public virtual XVar initUpload(dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic auth = null, policy = null, postPolicy = XVar.Array(), reqTimestamp = null, resourceKey = null, uniqueFilename = null, uploadParams = null;
			uniqueFilename = XVar.Clone(this.tryCreateUniqueFile((XVar)(userFilename), (XVar)(this.path)));
			if(XVar.Pack(!(XVar)(uniqueFilename)))
			{
				this.setLastError((XVar)(MVCFunctions.Concat("Unable to get unique file name for ", userFilename, " at ", this.path)));
				return null;
			}
			resourceKey = XVar.Clone(MVCFunctions.Concat(this.path, uniqueFilename));
			reqTimestamp = XVar.Clone(MVCFunctions.time());
			auth = XVar.Clone(this.getAuthObject((XVar)(new HttpRequest(new XVar(""))), new XVar(""), (XVar)(reqTimestamp)));
			postPolicy = XVar.Clone(XVar.Array());
			postPolicy.InitAndSetArrayItem(MVCFunctions.iso8601date((XVar)(reqTimestamp + Constants.S3_EXPIRATION_TIME_SECONDS)), "expiration");
			postPolicy.InitAndSetArrayItem(new XVar(0, new XVar("bucket", this.bucket), 1, new XVar(0, "eq", 1, "$key", 2, resourceKey), 2, new XVar("x-amz-credential", auth.getCredential()), 3, new XVar("x-amz-algorithm", "AWS4-HMAC-SHA256"), 4, new XVar("x-amz-date", MVCFunctions.iso8601date_timestamp((XVar)(reqTimestamp)))), "conditions");
			policy = XVar.Clone(MVCFunctions.base64_encode((XVar)(MVCFunctions.runner_json_encode((XVar)(postPolicy)))));
			uploadParams = XVar.Clone(new XVar("key", resourceKey, "X-Amz-Credential", auth.getCredential(), "X-Amz-Algorithm", "AWS4-HMAC-SHA256", "X-Amz-Date", MVCFunctions.iso8601date_timestamp((XVar)(reqTimestamp)), "Policy", policy, "X-Amz-Signature", auth.signString((XVar)(policy))));
			return new XVar("uploadParams", new XVar("url", this.endpoint(), "data", uploadParams), "fileId", resourceKey);
		}
		public override XVar directUpload()
		{
			return true;
		}
		protected override XVar tryCreateFile(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			if(XVar.Pack(!(XVar)(this.getFileInfo((XVar)(path)))))
			{
				this.saveFile((XVar)(this.stubFileData), (XVar)(path));
				return true;
			}
			return false;
		}
	}
}
