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
	public partial class OneDriveFileSystem : OAuthFileSystem
	{
		public dynamic path = XVar.Pack(null);
		public dynamic driveId = XVar.Pack(null);
		protected dynamic driveEndpoint = XVar.Pack(null);
		private dynamic chunkSize = XVar.Pack(5242880);
		protected dynamic _fileInfoCache = XVar.Array();
		protected static bool skipOneDriveFileSystemCtor = false;
		public OneDriveFileSystem(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipOneDriveFileSystemCtor)
			{
				skipOneDriveFileSystemCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.sessionKey = new XVar("onedriveOauthTokenStatus");
			this.privateStorageKey = new XVar("onedrive");
			this.path = XVar.Clone(var_params["path"]);
			this.driveId = XVar.Clone(var_params["driveId"]);
			this.driveEndpoint = XVar.Clone(MVCFunctions.Concat(Constants.oneDriveApiEndpoint, "/drives/", this.driveId));
		}
		protected override XVar _getConnection()
		{
			return CommonFunctions.getOneDriveConnection();
		}
		public override XVar saveUploadedFile(dynamic _param_uploadFile, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic uploadFile = XVar.Clone(_param_uploadFile);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic fileData = null, uploadData = XVar.Array();
			fileData = XVar.Clone(RunnerFileSystem.uploadedFileContent((XVar)(uploadFile)));
			if(MVCFunctions.strlen_bin((XVar)(fileData)) <= Constants.oneDrive4MB_Bytes)
			{
				uploadData = XVar.Clone(this.saveShortData((XVar)(fileData), (XVar)(userFilename)));
			}
			else
			{
				uploadData = XVar.Clone(this.saveLongData((XVar)(fileData), (XVar)(userFilename)));
			}
			return uploadData["id"];
		}
		private XVar normalizedFilePath(dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic encodedPath = null, resourcePath = null;
			resourcePath = XVar.Clone(MVCFunctions.Concat(OneDriveFileSystem.normalizePath((XVar)(this.path)), userFilename));
			encodedPath = XVar.Clone(OneDriveFileSystem.pathEncode((XVar)(resourcePath)));
			return MVCFunctions.Concat("/root:", encodedPath, ":");
		}
		protected virtual XVar saveShortData(dynamic _param_fileData, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic rest = null, result = XVar.Array(), uploadData = null, url = null, var_request = null;
			url = XVar.Clone(MVCFunctions.Concat(this.driveEndpoint, this.normalizedFilePath((XVar)(userFilename)), "/content"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url), new XVar("PUT")));
			var_request.urlParams.InitAndSetArrayItem("rename", "@microsoft.graph.conflictBehavior");
			var_request.body = XVar.Clone(fileData);
			rest = XVar.Clone(getConnection());
			result = XVar.Clone(var_request.run());
			uploadData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
			return uploadData;
		}
		private XVar trySendByteRanges(dynamic _param_url, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic url = XVar.Clone(_param_url);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic chunkEnd = null, chunkLength = null, chunkStart = null, contentLength = null, part = null, rangesCount = null, responseCode = null, result = XVar.Array(), uploadData = null, var_request = null;
			contentLength = XVar.Clone(MVCFunctions.strlen_bin((XVar)(data)));
			rangesCount = XVar.Clone((XVar)Math.Ceiling((double)(contentLength / this.chunkSize)));
			var_request = XVar.Clone(new HttpRequest((XVar)(url), new XVar("PUT")));
			result = new XVar(null);
			part = new XVar(0);
			for(;part < rangesCount; part++)
			{
				chunkStart = XVar.Clone(this.chunkSize * part);
				chunkEnd = XVar.Clone(chunkStart + this.chunkSize);
				chunkEnd -= (chunkEnd  %  contentLength)  %  this.chunkSize + 1;
				chunkLength = XVar.Clone((chunkEnd - chunkStart) + 1);
				var_request.headers.InitAndSetArrayItem(chunkLength, "content-length");
				var_request.headers.InitAndSetArrayItem(MVCFunctions.Concat("bytes ", chunkStart, "-", chunkEnd, "/", contentLength), "content-range");
				var_request.body = XVar.Clone(MVCFunctions.substr((XVar)(data), (XVar)(chunkStart), (XVar)(chunkLength)));
				result = XVar.Clone(var_request.run());
				responseCode = XVar.Clone(result["responseCode"]);
				if((XVar)((XVar)(responseCode != 201)  && (XVar)(responseCode != 202))  && (XVar)(responseCode != 200))
				{
					return null;
				}
			}
			uploadData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
			return uploadData;
		}
		public override XVar directUpload()
		{
			return true;
		}
		protected virtual XVar initUploadSession(dynamic _param_objectPath, dynamic _param_replaceExisting = null)
		{
			#region default values
			if(_param_replaceExisting as Object == null) _param_replaceExisting = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic objectPath = XVar.Clone(_param_objectPath);
			dynamic replaceExisting = XVar.Clone(_param_replaceExisting);
			#endregion

			dynamic data = XVar.Array(), result = XVar.Array(), uploadParams = XVar.Array(), url = null, var_request = null;
			url = XVar.Clone(MVCFunctions.Concat(this.driveEndpoint, objectPath, "/createUploadSession"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url), new XVar("POST")));
			uploadParams = XVar.Clone(XVar.Array());
			uploadParams.InitAndSetArrayItem(XVar.Array(), "item");
			if(XVar.Pack(replaceExisting))
			{
				uploadParams.InitAndSetArrayItem("replace", "item", "@microsoft.graph.conflictBehavior");
			}
			else
			{
				uploadParams.InitAndSetArrayItem("rename", "item", "@microsoft.graph.conflictBehavior");
			}
			var_request.headers.InitAndSetArrayItem("application/json", "content-type");
			var_request.body = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(uploadParams)));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(result["error"]))
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return false;
			}
			data = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
			if(XVar.Pack(!(XVar)(data["uploadUrl"])))
			{
				this.setLastError((XVar)(result["content"]));
				return false;
			}
			return data["uploadUrl"];
		}
		protected virtual XVar saveLongData(dynamic _param_fileData, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic objectPath = null, uploadUrl = null;
			objectPath = XVar.Clone(this.normalizedFilePath((XVar)(userFilename)));
			uploadUrl = XVar.Clone(this.initUploadSession((XVar)(objectPath)));
			if(XVar.Pack(!(XVar)(uploadUrl)))
			{
				return null;
			}
			return this.trySendByteRanges((XVar)(uploadUrl), (XVar)(fileData));
		}
		public override XVar redirectToFile(dynamic _param_fileId, dynamic _param_thumbnail)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			dynamic thumbnail = XVar.Clone(_param_thumbnail);
			#endregion

			dynamic fInfo = XVar.Array();
			fInfo = XVar.Clone(this.getFileInfo((XVar)(fileId)));
			if(XVar.Pack(!(XVar)(fInfo)))
			{
				return false;
			}
			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", fInfo["raw"]["@microsoft.graph.downloadUrl"])));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public virtual XVar initUpload(dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic fileId = null, objectPath = null, uploadData = XVar.Array(), uploadUrl = null;
			uploadData = XVar.Clone(this.saveShortData(new XVar(Constants.oneDriveStubData), (XVar)(userFilename)));
			fileId = XVar.Clone(uploadData["id"]);
			if(XVar.Pack(!(XVar)(fileId)))
			{
				return null;
			}
			objectPath = XVar.Clone(MVCFunctions.Concat("/items/", fileId));
			uploadUrl = XVar.Clone(this.initUploadSession((XVar)(objectPath), new XVar(true)));
			return new XVar("uploadParams", new XVar("url", uploadUrl), "fileId", fileId);
		}
		public override XVar getFileInfo(dynamic _param_fileId)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			#endregion

			dynamic data = XVar.Array(), result = XVar.Array(), ret = null, url = null, var_request = null;
			if(XVar.Pack(this._fileInfoCache[fileId]))
			{
				return this._fileInfoCache[fileId];
			}
			url = XVar.Clone(MVCFunctions.Concat(this.driveEndpoint, "/items/", fileId));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url), new XVar("GET")));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(result["error"]))
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return null;
			}
			data = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
			if(XVar.Pack(!(XVar)(data["id"])))
			{
				this.setLastError((XVar)(result["content"]));
				return null;
			}
			ret = XVar.Clone(new XVar("fullPath", data["id"], "size", data["size"], "raw", data, "returnContent", false));
			this._fileInfoCache.InitAndSetArrayItem(ret, fileId);
			return ret;
		}
		public override XVar delete(dynamic _param_fileId)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			#endregion

			dynamic url = null, var_request = null;
			url = XVar.Clone(MVCFunctions.Concat(this.driveEndpoint, "/items/", fileId));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url), new XVar("DELETE")));
			var_request.run();

			return null;
		}
		protected static XVar normalizePath(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic lastSymbol = null;
			if(XVar.Pack(!(XVar)(path)))
			{
				return "/";
			}
			lastSymbol = XVar.Clone(MVCFunctions.substr((XVar)(path), (XVar)(MVCFunctions.strlen((XVar)(path)) - 1), new XVar(1)));
			if(lastSymbol != "/")
			{
				path = MVCFunctions.Concat(path, "/");
			}
			if(MVCFunctions.substr((XVar)(path), new XVar(0), new XVar(1)) != "/")
			{
				path = XVar.Clone(MVCFunctions.Concat("/", path));
			}
			return path;
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
	}
}
