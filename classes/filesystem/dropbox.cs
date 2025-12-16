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
	public partial class DropboxFileSystem : OAuthFileSystem
	{
		protected dynamic path;
		protected dynamic chunkSize = XVar.Pack(16777216);
		protected static bool skipDropboxFileSystemCtor = false;
		public DropboxFileSystem(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipDropboxFileSystemCtor)
			{
				skipDropboxFileSystemCtor = false;
				return;
			}
			this.sessionKey = new XVar("dropboxOauthTokenStatus");
			this.privateStorageKey = new XVar("dropbox");
			this.path = XVar.Clone(var_params["path"]);
		}
		protected override XVar _getConnection()
		{
			return CommonFunctions.getDropboxConnection();
		}
		protected virtual XVar getEndpoint(dynamic _param_entity)
		{
			#region pass-by-value parameters
			dynamic entity = XVar.Clone(_param_entity);
			#endregion

			return MVCFunctions.Concat("https://", entity, ".dropboxapi.com/2/files/");
		}
		public override XVar saveUploadedFile(dynamic _param_uploadFile, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic uploadFile = XVar.Clone(_param_uploadFile);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic fileData = null, path = null, uploadData = XVar.Array();
			fileData = XVar.Clone(RunnerFileSystem.uploadedFileContent((XVar)(uploadFile)));
			path = XVar.Clone(MVCFunctions.Concat(DropboxFileSystem.normalizePath((XVar)(this.path)), userFilename));
			if(MVCFunctions.strlen_bin((XVar)(fileData)) <= Constants.DROPBOX_UPLOAD_BYTES_LIMIT)
			{
				uploadData = XVar.Clone(this.saveShortData((XVar)(fileData), (XVar)(path)));
			}
			else
			{
				uploadData = XVar.Clone(this.saveLongData((XVar)(fileData), (XVar)(path)));
			}
			return uploadData["path_lower"];
		}
		protected virtual XVar getDefaultUploadStrategy()
		{
			dynamic strategy = null;
			strategy = XVar.Clone(new XVar("mode", "add", "autorename", true, "mute", true, "strict_conflict", true));
			return strategy;
		}
		public virtual XVar saveShortData(dynamic _param_fileData, dynamic _param_path, dynamic _param_uploadStrategy = null)
		{
			#region default values
			if(_param_uploadStrategy as Object == null) _param_uploadStrategy = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			dynamic path = XVar.Clone(_param_path);
			dynamic uploadStrategy = XVar.Clone(_param_uploadStrategy);
			#endregion

			dynamic result = XVar.Array(), strategy = XVar.Array(), url = null, var_request = null;
			strategy = XVar.Clone((XVar.Pack(uploadStrategy) ? XVar.Pack(uploadStrategy) : XVar.Pack(this.getDefaultUploadStrategy())));
			strategy.InitAndSetArrayItem(path, "path");
			url = XVar.Clone(MVCFunctions.Concat(this.getEndpoint(new XVar("content")), "upload"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url)));
			var_request.headers.InitAndSetArrayItem("application/octet-stream", "content-type");
			var_request.headers.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(strategy)), "dropbox-api-arg");
			var_request.body = XVar.Clone(fileData);
			result = XVar.Clone(var_request.run());
			return MVCFunctions.runner_json_decode((XVar)(result["content"]));
		}
		protected virtual XVar initUploadSession()
		{
			dynamic result = XVar.Array(), uploadData = XVar.Array(), url = null, var_request = null;
			url = XVar.Clone(MVCFunctions.Concat(this.getEndpoint(new XVar("content")), "upload_session/start"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url)));
			var_request.headers.InitAndSetArrayItem("application/octet-stream", "content-type");
			var_request.headers.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(new XVar("close", false))), "dropbox-api-arg");
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(result["error"]))
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return false;
			}
			uploadData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
			if(XVar.Pack(!(XVar)(uploadData["session_id"])))
			{
				this.setLastError((XVar)(result["content"]));
				return false;
			}
			return uploadData["session_id"];
		}
		protected virtual XVar trySendByteRanges(dynamic _param_data, dynamic _param_sessionId)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic sessionId = XVar.Clone(_param_sessionId);
			#endregion

			dynamic chunkEnd = null, chunkLength = null, chunkStart = null, contentLength = null, part = null, rangesCount = null, result = XVar.Array(), url = null, var_request = null;
			contentLength = XVar.Clone(MVCFunctions.strlen_bin((XVar)(data)));
			rangesCount = XVar.Clone((XVar)Math.Ceiling((double)(contentLength / this.chunkSize)));
			url = XVar.Clone(MVCFunctions.Concat(this.getEndpoint(new XVar("content")), "upload_session/append_v2"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url)));
			var_request.headers.InitAndSetArrayItem("application/octet-stream", "content-type");
			result = new XVar(null);
			part = new XVar(0);
			for(;part < rangesCount; part++)
			{
				chunkStart = XVar.Clone(this.chunkSize * part);
				chunkEnd = XVar.Clone(chunkStart + this.chunkSize);
				chunkEnd -= (chunkEnd  %  contentLength)  %  this.chunkSize + 1;
				chunkLength = XVar.Clone((chunkEnd - chunkStart) + 1);
				var_request.headers.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(new XVar("cursor", new XVar("session_id", sessionId, "offset", chunkStart), "close", false))), "dropbox-api-arg");
				var_request.body = XVar.Clone(MVCFunctions.substr((XVar)(data), (XVar)(chunkStart), (XVar)(chunkLength)));
				result = XVar.Clone(var_request.run());
				data = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
				if(XVar.Pack(data))
				{
					return false;
				}
			}
			return true;
		}
		protected virtual XVar saveLongData(dynamic _param_fileData, dynamic _param_path, dynamic _param_uploadStrategy = null)
		{
			#region default values
			if(_param_uploadStrategy as Object == null) _param_uploadStrategy = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			dynamic path = XVar.Clone(_param_path);
			dynamic uploadStrategy = XVar.Clone(_param_uploadStrategy);
			#endregion

			dynamic result = XVar.Array(), sessionId = null, strategy = XVar.Array(), url = null, var_params = XVar.Array(), var_request = null;
			sessionId = XVar.Clone(this.initUploadSession());
			if(XVar.Pack(!(XVar)(this.trySendByteRanges((XVar)(fileData), (XVar)(sessionId)))))
			{
				return null;
			}
			url = XVar.Clone(MVCFunctions.Concat(this.getEndpoint(new XVar("content")), "upload_session/finish"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url)));
			var_request.headers.InitAndSetArrayItem("application/octet-stream", "content-type");
			strategy = XVar.Clone((XVar.Pack(uploadStrategy) ? XVar.Pack(uploadStrategy) : XVar.Pack(this.getDefaultUploadStrategy())));
			strategy.InitAndSetArrayItem(path, "path");
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(XVar.Array(), "cursor");
			var_params.InitAndSetArrayItem(sessionId, "cursor", "session_id");
			var_params.InitAndSetArrayItem(MVCFunctions.strlen_bin((XVar)(fileData)), "cursor", "offset");
			var_params.InitAndSetArrayItem(strategy, "commit");
			var_request.headers.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(var_params)), "dropbox-api-arg");
			result = XVar.Clone(var_request.run());
			return MVCFunctions.runner_json_decode((XVar)(result["content"]));
		}
		public override XVar delete(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic result = XVar.Array(), url = null, var_request = null;
			url = XVar.Clone(MVCFunctions.Concat(this.getEndpoint(new XVar("api")), "delete_v2"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url)));
			var_request.headers.InitAndSetArrayItem("application/json", "content-type");
			var_request.body = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(new XVar("path", path))));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(result["error"]))
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return false;
			}
			return MVCFunctions.runner_json_decode((XVar)(result["content"]));
		}
		public virtual XVar initUpload(dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic data = XVar.Array(), normalizedPath = null, result = XVar.Array(), strategy = XVar.Array(), uniqueFilename = null, url = null, var_params = null, var_request = null;
			url = XVar.Clone(MVCFunctions.Concat(this.getEndpoint(new XVar("api")), "get_temporary_upload_link"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url)));
			var_request.headers.InitAndSetArrayItem("application/json", "content-type");
			normalizedPath = XVar.Clone(DropboxFileSystem.normalizePath((XVar)(this.path)));
			uniqueFilename = XVar.Clone(this.tryCreateUniqueFile((XVar)(userFilename), (XVar)(normalizedPath)));
			if(XVar.Pack(!(XVar)(uniqueFilename)))
			{
				this.setLastError((XVar)(MVCFunctions.Concat("Unable to get unique file name for ", userFilename, " at ", normalizedPath)));
				return false;
			}
			strategy = XVar.Clone(new XVar("mode", "overwrite", "autorename", false, "mute", true, "strict_conflict", true));
			strategy.InitAndSetArrayItem(MVCFunctions.Concat(normalizedPath, uniqueFilename), "path");
			var_params = XVar.Clone(new XVar("commit_info", strategy, "duration", Constants.DROPBOX_EXPIRATION_TIME_SECONDS));
			var_request.body = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(var_params)));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(result["error"]))
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return false;
			}
			data = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
			if(XVar.Pack(!(XVar)(data["link"])))
			{
				this.setLastError((XVar)(result["content"]));
				return false;
			}
			return new XVar("uploadParams", new XVar("url", data["link"]), "fileId", strategy["path"]);
		}
		public override XVar redirectToFile(dynamic _param_path, dynamic _param_thumbnail)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			dynamic thumbnail = XVar.Clone(_param_thumbnail);
			#endregion

			dynamic data = XVar.Array(), result = XVar.Array(), url = null, var_params = null, var_request = null;
			url = XVar.Clone(MVCFunctions.Concat(this.getEndpoint(new XVar("api")), "get_temporary_link"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url)));
			var_request.headers.InitAndSetArrayItem("application/json", "content-type");
			var_params = XVar.Clone(new XVar("path", path));
			var_request.body = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(var_params)));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(result["error"]))
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return false;
			}
			data = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
			if(XVar.Pack(!(XVar)(data["link"])))
			{
				this.setLastError((XVar)(result["content"]));
				return false;
			}
			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", data["link"])));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public override XVar getFileInfo(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic data = XVar.Array(), result = XVar.Array(), ret = XVar.Array(), url = null, var_params = null, var_request = null;
			url = XVar.Clone(MVCFunctions.Concat(this.getEndpoint(new XVar("api")), "get_metadata"));
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(url)));
			var_request.headers.InitAndSetArrayItem("application/json", "content-type");
			var_params = XVar.Clone(new XVar("path", path, "include_media_info", false, "include_deleted", false, "include_has_explicit_shared_members", false));
			var_request.body = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(var_params)));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(result["error"]))
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return null;
			}
			data = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
			if(XVar.Pack(!(XVar)(data["name"])))
			{
				this.setLastError((XVar)(result["content"]));
				return null;
			}
			ret = XVar.Clone(new XVar("raw", data, "returnContent", false));
			ret.InitAndSetArrayItem(data["path_lower"], "fullPath");
			ret.InitAndSetArrayItem(data["size"], "size");
			return ret;
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
		public override XVar directUpload()
		{
			return true;
		}
		protected override XVar tryCreateFile(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic strategy = null, uploadData = XVar.Array();
			strategy = XVar.Clone(new XVar("mode", "add", "autorename", false, "mute", true, "strict_conflict", true));
			uploadData = XVar.Clone(this.saveShortData((XVar)(this.stubFileData), (XVar)(path), (XVar)(strategy)));
			if(XVar.Pack(uploadData["error"]))
			{
				return false;
			}
			return true;
		}
		public override XVar createAuthRequest()
		{
			dynamic rest = null;
			rest = XVar.Clone(this.getConnection());
			return rest.createUserAuthRequest((XVar)(new XVar("token_access_type", "offline")));
		}
	}
}
