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
	public partial class ViewFileField : ViewControl
	{
		protected static bool skipViewFileFieldCtor = false;
		public ViewFileField(dynamic field, dynamic container, dynamic pageobject) // proxy constructor
			:base((XVar)field, (XVar)container, (XVar)pageobject) {}

		public override XVar getPdfValue(dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic filesList = null, textVal = null;
			textVal = XVar.Clone(this.getTextValue((XVar)(data)));
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(textVal)))))
			{
				return "\"\"";
			}
			filesList = XVar.Clone(MVCFunctions.explode(new XVar(", "), (XVar)(textVal)));
			return MVCFunctions.runner_json_encode((XVar)(new XVar("stack", filesList)));
		}
		public override XVar getTextValue(dynamic data)
		{
			dynamic fileNames = XVar.Array(), filesData = XVar.Array();
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(data[this.field])))))
			{
				return "";
			}
			if(XVar.Pack(this.container.pSet.isVideoUrlField((XVar)(this.field))))
			{
				return data[this.field];
			}
			fileNames = XVar.Clone(XVar.Array());
			filesData = XVar.Clone(this.getFilesData((XVar)(data[this.field])));
			foreach (KeyValuePair<XVar, dynamic> file in filesData.GetEnumerator())
			{
				fileNames.InitAndSetArrayItem(this.getElementTextValue((XVar)(file.Value), (XVar)(data)), null);
			}
			return MVCFunctions.implode(new XVar(", "), (XVar)(fileNames));
		}
		protected virtual XVar getElementTextValue(dynamic _param_fileData, dynamic data)
		{
			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			#endregion

			return fileData["usrName"];
		}
		public virtual XVar getFilesData(dynamic _param_fieldValue)
		{
			#region pass-by-value parameters
			dynamic fieldValue = XVar.Clone(_param_fieldValue);
			#endregion

			dynamic fileData = XVar.Array(), files = null;
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(fieldValue)))
			{
				return XVar.Array();
			}
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			if(XVar.Pack(this.isUrl()))
			{
				fileData = XVar.Clone(XVar.Array());
				fileData.InitAndSetArrayItem(CommonFunctions.runner_basename((XVar)(fieldValue)), "usrName");
				fileData.InitAndSetArrayItem(fieldValue, "name");
				return new XVar(0, fileData);
			}
			files = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(fieldValue)));
			if((XVar)(!(XVar)(MVCFunctions.is_array((XVar)(files))))  || (XVar)((XVar)(!(XVar)(files))  && (XVar)(fieldValue != "[]")))
			{
				fileData = XVar.Clone(XVar.Array());
				fileData.InitAndSetArrayItem(CommonFunctions.runner_basename((XVar)(fieldValue)), "usrName");
				fileData.InitAndSetArrayItem(MVCFunctions.Concat(DiskFileSystem.normalizePath((XVar)(pSet.getUploadFolder((XVar)(this.field)))), fieldValue), "name");
				if((XVar)((XVar)(pSet.getCreateThumbnail((XVar)(this.field)))  || (XVar)(pSet.showThumbnail((XVar)(this.field))))  && (XVar)(pSet.getStrThumbnail((XVar)(this.field))))
				{
					fileData.InitAndSetArrayItem(MVCFunctions.Concat(DiskFileSystem.normalizePath((XVar)(pSet.getUploadFolder((XVar)(this.field)))), pSet.getStrThumbnail((XVar)(this.field)), fieldValue), "thumbnail");
				}
				return new XVar(0, fileData);
			}
			return files;
		}
		protected virtual XVar fastFileInfo(dynamic _param_filename, dynamic _param_fs = null)
		{
			#region default values
			if(_param_fs as Object == null) _param_fs = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			dynamic fs = XVar.Clone(_param_fs);
			#endregion

			if(XVar.Pack(!(XVar)(filename)))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(fs)))
			{
				fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(this.pSettings()), (XVar)(this.field)));
			}
			if(XVar.Pack(!(XVar)(fs.fast())))
			{
				return new XVar("fullPath", filename);
			}
			return fs.getFileInfo((XVar)(filename));
		}
		protected virtual XVar fastFileExists(dynamic _param_filename, dynamic _param_fs = null)
		{
			#region default values
			if(_param_fs as Object == null) _param_fs = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			dynamic fs = XVar.Clone(_param_fs);
			#endregion

			return !(XVar)(!(XVar)(this.fastFileInfo((XVar)(filename), (XVar)(fs))));
		}
		protected virtual XVar getFileUrl(dynamic _param_fileData, dynamic _param_keylink, dynamic _param_thumbnail = null, dynamic _param_additionalParams = null)
		{
			#region default values
			if(_param_thumbnail as Object == null) _param_thumbnail = new XVar(false);
			if(_param_additionalParams as Object == null) _param_additionalParams = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic thumbnail = XVar.Clone(_param_thumbnail);
			dynamic additionalParams = XVar.Clone(_param_additionalParams);
			#endregion

			dynamic file = XVar.Array(), fs = null, lastModified = null, url = null, var_params = XVar.Array();
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(pSet), (XVar)(this.field)));
			lastModified = XVar.Clone(MVCFunctions.time());
			if(XVar.Pack(fs.fast()))
			{
				dynamic fsInfo = XVar.Array();
				fsInfo = XVar.Clone(fs.getFileInfo((XVar)(fileData["name"])));
				if((XVar)(fsInfo)  && (XVar)(fsInfo["lastModified"]))
				{
					lastModified = XVar.Clone(fsInfo["lastModified"]);
				}
			}
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(fileData["usrName"], "file");
			var_params.InitAndSetArrayItem(pSet.table(), "table");
			var_params.InitAndSetArrayItem(this.field, "field");
			var_params.InitAndSetArrayItem(1, "nodisp");
			var_params.InitAndSetArrayItem(CommonFunctions.fileAttrHash((XVar)(keylink), (XVar)(file["size"]), (XVar)(lastModified)), "hash");
			if(XVar.Pack(thumbnail))
			{
				var_params.InitAndSetArrayItem(1, "thumbnail");
			}
			foreach (KeyValuePair<XVar, dynamic> val in additionalParams.GetEnumerator())
			{
				var_params.InitAndSetArrayItem(val.Value, val.Key);
			}
			url = XVar.Clone(MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat(CommonFunctions.prepareUrlQuery((XVar)(var_params)), keylink))));
			if((XVar)(this.displayPDF())  && (XVar)(MVCFunctions.strtolower((XVar)(CommonFunctions.getFileExtension((XVar)(var_params["file"])))) == "pdf"))
			{
				url = XVar.Clone(MVCFunctions.Concat("include/pdfjs/web/viewer.html?file=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.Concat(MVCFunctions.projectPath(), url)))));
			}
			return url;
		}
		protected virtual XVar displayPDF()
		{
			dynamic viewFormat = null;
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			viewFormat = XVar.Clone(pSet.getViewFormat((XVar)(this.field)));
			return (XVar)(pSet.displayPDF((XVar)(this.field)))  && (XVar)((XVar)(viewFormat == Constants.FORMAT_FILE)  || (XVar)(viewFormat == Constants.FORMAT_DATABASE_FILE));
		}
		protected virtual XVar isUrl()
		{
			return false;
		}
	}
}
