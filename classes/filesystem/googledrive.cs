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
	public partial class GoogleDriveFileSystem : OAuthFileSystem
	{
		public dynamic publicUnlisted = XVar.Pack(false);
		public dynamic folder = XVar.Pack(false);
		protected dynamic _fileInfoCache = XVar.Array();
		protected static bool skipGoogleDriveFileSystemCtor = false;
		public GoogleDriveFileSystem(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipGoogleDriveFileSystemCtor)
			{
				skipGoogleDriveFileSystemCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.sessionKey = new XVar("googleDriveOauthTokenStatus");
			this.privateStorageKey = new XVar("googledrive");
			this.folder = XVar.Clone(var_params["folder"]);
		}
		protected override XVar _getConnection()
		{
			return CommonFunctions.getGoogleDriveConnection();
		}
		public override XVar saveUploadedFile(dynamic _param_uploadFile, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic uploadFile = XVar.Clone(_param_uploadFile);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic fileContents = null;
			fileContents = XVar.Clone(RunnerFileSystem.uploadedFileContent((XVar)(uploadFile)));
			return this.saveData((XVar)(fileContents), (XVar)(userFilename));
		}
		public override XVar saveData(dynamic _param_fileContents, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic fileContents = XVar.Clone(_param_fileContents);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic headers = XVar.Array(), rest = null, result = null, url = null, var_request = null;
			rest = XVar.Clone(this.getConnection());
			var_request = XVar.Clone(new HttpRequest(new XVar("https://www.googleapis.com/upload/drive/v3/files"), new XVar("POST")));
			var_request.urlParams.InitAndSetArrayItem("resumable", "uploadType");
			var_request.postPayload.InitAndSetArrayItem(userFilename, "name");
			if(XVar.Pack(this.folder))
			{
				var_request.postPayload.InitAndSetArrayItem(new XVar(0, this.folder), "parents");
			}
			var_request.headers.InitAndSetArrayItem("application/json; charset=UTF-8", "Content-Type");
			rest.requestAddAuth((XVar)(var_request));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(!(XVar)(result)))
			{
			}
			headers = XVar.Clone(HttpRequest.parseHeaders((XVar)(result)));
			url = XVar.Clone(headers["location"]);
			if(XVar.Pack(url))
			{
				dynamic uploadRequest = null, uploadResult = XVar.Array();
				uploadRequest = XVar.Clone(new HttpRequest((XVar)(url), new XVar("PUT")));
				uploadRequest.headers.InitAndSetArrayItem(MVCFunctions.Concat("*/", MVCFunctions.strlen((XVar)(fileContents))), "Range");
				uploadRequest.headers.InitAndSetArrayItem("application/octet-stream", "Content-Type");
				uploadRequest.body = XVar.Clone(fileContents);
				uploadResult = XVar.Clone(uploadRequest.run());
				if((XVar)(uploadResult)  && (XVar)(!(XVar)(uploadResult["error"])))
				{
					dynamic uploadData = XVar.Array();
					uploadData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(uploadResult["content"])));
					this.setFilePermissions((XVar)(uploadData["id"]));
					return uploadData["id"];
				}
			}
			return null;
		}
		protected virtual XVar setFilePermissions(dynamic _param_fileId)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			#endregion

			dynamic rest = null, var_request = null;
			var_request = XVar.Clone(new HttpRequest((XVar)(MVCFunctions.Concat("https://www.googleapis.com/drive/v3/files/", fileId, "/permissions")), new XVar("POST")));
			var_request.postPayload.InitAndSetArrayItem("reader", "role");
			var_request.postPayload.InitAndSetArrayItem("anyone", "type");
			var_request.headers.InitAndSetArrayItem("application/json; charset=UTF-8", "Content-Type");
			rest = XVar.Clone(this.getConnection());
			rest.requestAddAuth((XVar)(var_request));
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
		public override XVar redirectToFile(dynamic _param_fileId, dynamic _param_thumbnail)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			dynamic thumbnail = XVar.Clone(_param_thumbnail);
			#endregion

			dynamic fInfo = XVar.Array(), url = null;
			fInfo = XVar.Clone(this.getFileInfo((XVar)(fileId)));
			if(XVar.Pack(!(XVar)(fInfo)))
			{
				return false;
			}
			url = XVar.Clone(fInfo["raw"]["webContentLink"]);
			if((XVar)(thumbnail)  && (XVar)(fInfo["raw"]["thumbnailLink"]))
			{
				url = XVar.Clone(fInfo["raw"]["thumbnailLink"]);
			}
			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", url)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public override XVar getFileInfo(dynamic _param_fileId)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			#endregion

			dynamic data = XVar.Array(), fields = XVar.Array(), result = XVar.Array(), ret = null, var_request = null;
			if(XVar.Pack(this._fileInfoCache[fileId]))
			{
				return this._fileInfoCache[fileId];
			}
			fields = XVar.Clone(XVar.Array());
			fields.InitAndSetArrayItem("id", null);
			fields.InitAndSetArrayItem("name", null);
			fields.InitAndSetArrayItem("mimeType", null);
			fields.InitAndSetArrayItem("webContentLink", null);
			fields.InitAndSetArrayItem("iconLink", null);
			fields.InitAndSetArrayItem("thumbnailLink", null);
			fields.InitAndSetArrayItem("permissions", null);
			fields.InitAndSetArrayItem("size", null);
			var_request = XVar.Clone(this.getAuthorizedRequest((XVar)(MVCFunctions.Concat("https://www.googleapis.com/drive/v3/files/", fileId)), new XVar("GET")));
			var_request.urlParams.InitAndSetArrayItem(MVCFunctions.implode(new XVar(","), (XVar)(fields)), "fields");
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

			dynamic rest = null, var_request = null;
			var_request = XVar.Clone(new HttpRequest((XVar)(MVCFunctions.Concat("https://www.googleapis.com/drive/v3/files/", fileId)), new XVar("DELETE")));
			rest = XVar.Clone(this.getConnection());
			rest.requestAddAuth((XVar)(var_request));
			var_request.run();

			return null;
		}
		public virtual XVar initUpload(dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic fileId = null, fileInfo = XVar.Array(), headers = XVar.Array(), rest = null, result = XVar.Array(), var_request = null;
			rest = XVar.Clone(this.getConnection());
			fileInfo = XVar.Clone(this.createFile((XVar)(userFilename)));
			fileId = XVar.Clone(fileInfo["id"]);
			this.setFilePermissions((XVar)(fileId));
			var_request = XVar.Clone(new HttpRequest((XVar)(MVCFunctions.Concat("https://www.googleapis.com/upload/drive/v3/files/", fileId)), new XVar("PATCH")));
			var_request.urlParams.InitAndSetArrayItem("resumable", "uploadType");
			var_request.headers.InitAndSetArrayItem(CommonFunctions.projectOrigin(), "Origin");
			rest = XVar.Clone(this.getConnection());
			rest.requestAddAuth((XVar)(var_request));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(result["error"]))
			{
				this.setLastError((XVar)(result["errorMessage"]));
				return false;
			}
			headers = XVar.Clone(HttpRequest.parseHeaders((XVar)(result)));
			if(XVar.Pack(!(XVar)(headers["location"])))
			{
				this.setLastError((XVar)(MVCFunctions.Concat("Error initaiting upload.\r\n", MVCFunctions.runner_json_encode((XVar)(result)))));
				return false;
			}
			return new XVar("uploadParams", new XVar("url", headers["location"]), "fileId", fileId);
		}
		protected virtual XVar createFile(dynamic _param_filename)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			#endregion

			dynamic fileInfo = null, rest = null, result = XVar.Array(), var_request = null;
			rest = XVar.Clone(this.getConnection());
			var_request = XVar.Clone(new HttpRequest(new XVar("https://www.googleapis.com/drive/v3/files"), new XVar("POST")));
			var_request.postPayload.InitAndSetArrayItem(filename, "name");
			if(XVar.Pack(this.folder))
			{
				var_request.postPayload.InitAndSetArrayItem(new XVar(0, this.folder), "parents");
			}
			var_request.headers.InitAndSetArrayItem("application/json; charset=UTF-8", "Content-Type");
			rest.requestAddAuth((XVar)(var_request));
			result = XVar.Clone(var_request.run());
			if(XVar.Pack(!(XVar)(result)))
			{
				return false;
			}
			fileInfo = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(result["content"])));
			return fileInfo;
		}
		public override XVar directUpload()
		{
			return true;
		}
		public override XVar createAuthRequest()
		{
			dynamic rest = null;
			rest = XVar.Clone(this.getConnection());
			return rest.createUserAuthRequest((XVar)(new XVar("access_type", "offline")));
		}
	}
}
