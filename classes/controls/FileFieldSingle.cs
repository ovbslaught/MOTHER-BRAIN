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
	public partial class FileFieldSingle : EditControl
	{
		protected static bool skipFileFieldSingleCtor = false;
		public FileFieldSingle(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection)
		{
			if(skipFileFieldSingleCtor)
			{
				skipFileFieldSingleCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.format = new XVar(Constants.EDIT_FORMAT_FILE);
		}
		public override XVar addJSFiles()
		{

			return null;
		}
		public override XVar addCSSFiles()
		{

			return null;
		}
		public override XVar buildControl(dynamic _param_value, dynamic _param_mode, dynamic _param_fieldNum, dynamic _param_validate, dynamic _param_additionalCtrlParams, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic validate = XVar.Clone(_param_validate);
			dynamic additionalCtrlParams = XVar.Clone(_param_additionalCtrlParams);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic disp = null, filename_size = null, keyParams = XVar.Array(), keylink = null, strfilename = null, strtype = null;
			base.buildControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			if(mode == Constants.MODE_SEARCH)
			{
				dynamic classString = null;
				this.format = new XVar("");
				classString = new XVar("");
				classString = new XVar(" class=\"form-control\"");
				MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "\" ", classString, this.inputStyle, " type=\"text\" ", "autocomplete=\"off\" ", (XVar.Pack(this.is508 == true) ? XVar.Pack(MVCFunctions.Concat("alt=\"", this.strLabel, "\" ")) : XVar.Pack("")), "name=\"", this.cfield, "\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\">"));
				this.buildControlEnd((XVar)(validate), (XVar)(mode));
				return null;
			}
			keyParams = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> kf in this.pageObject.pSetEdit.getTableKeys().GetEnumerator())
			{
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("key", kf.Key + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(data[kf.Value]))))), null);
			}
			keylink = XVar.Clone(MVCFunctions.Concat("&", MVCFunctions.implode(new XVar("&"), (XVar)(keyParams))));
			disp = new XVar("");
			strfilename = new XVar("");
			filename_size = XVar.Clone((XVar.Pack(this.pageObject.pSetEdit.isUseTimestamp((XVar)(this.field))) ? XVar.Pack(50) : XVar.Pack(30)));
			if((XVar)(mode == Constants.MODE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_EDIT))
			{
				dynamic fileName = null, filesArray = XVar.Array();
				filesArray = XVar.Clone(this.getFileData((XVar)(value)));
				fileName = new XVar("");
				if(XVar.Pack(filesArray))
				{
					dynamic fileData = XVar.Array(), viewFormat = null;
					fileData = XVar.Clone(filesArray[0]);
					fileName = XVar.Clone(fileData["usrName"]);
					viewFormat = XVar.Clone(this.pageObject.pSetEdit.getViewFormat((XVar)(this.field)));
					if((XVar)(viewFormat == Constants.FORMAT_FILE)  || (XVar)(viewFormat == Constants.FORMAT_FILE_IMAGE))
					{
						disp = XVar.Clone(MVCFunctions.Concat(this.getFileOrImageMarkup((XVar)(fileData), (XVar)(keylink)), "<br />"));
					}
				}
				strfilename = XVar.Clone(MVCFunctions.Concat("<input type=hidden name=\"filenameHidden_", this.cfieldname, "\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(fileName)), "\"><br>", CommonFunctions.mlang_message(new XVar("FILENAME")), "&nbsp;&nbsp;<input type=\"text\" style=\"background-color:gainsboro\" disabled id=\"filename_", this.cfieldname, "\" name=\"filename_", this.cfieldname, "\" size=\"", filename_size, "\" maxlength=\"100\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(fileName)), "\">"));
				strtype = XVar.Clone(MVCFunctions.Concat("<br><input id=\"", this.ctype, "_keep\" type=\"Radio\" name=\"", this.ctype, "\" value=\"upload0\" checked class=\"rnr-uploadtype\">", CommonFunctions.mlang_message(new XVar("KEEP"))));
				if((XVar)((XVar)(MVCFunctions.strlen((XVar)(value)))  || (XVar)(mode == Constants.MODE_INLINE_EDIT))  && (XVar)(!(XVar)(this.pageObject.pSetEdit.isRequired((XVar)(this.field)))))
				{
					strtype = MVCFunctions.Concat(strtype, "<input id=\"", this.ctype, "_delete\" type=\"Radio\" name=\"", this.ctype, "\" value=\"upload1\" class=\"rnr-uploadtype\">", CommonFunctions.mlang_message(new XVar("DELETE")));
				}
				strtype = MVCFunctions.Concat(strtype, "<input id=\"", this.ctype, "_update\" type=\"Radio\" name=\"", this.ctype, "\" value=\"upload2\" class=\"rnr-uploadtype\">", CommonFunctions.mlang_message(new XVar("UPDATE")));
			}
			else
			{
				strtype = XVar.Clone(MVCFunctions.Concat("<input id=\"", this.ctype, "\" type=\"hidden\" name=\"", this.ctype, "\" value=\"upload2\">"));
				strfilename = XVar.Clone(MVCFunctions.Concat("<br>", CommonFunctions.mlang_message(new XVar("FILENAME")), "&nbsp;&nbsp;<input type=\"text\" id=\"filename_", this.cfieldname, "\" name=\"filename_", this.cfieldname, "\" size=\"", filename_size, "\" maxlength=\"100\">"));
			}
			MVCFunctions.Echo(MVCFunctions.Concat(disp, strtype));
			if((XVar)(mode == Constants.MODE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_EDIT))
			{
				MVCFunctions.Echo("<br>");
			}
			MVCFunctions.Echo(MVCFunctions.Concat("<input type=\"File\" id=\"", this.cfield, "\" ", "accept=\"", this.pageObject.pSetEdit.getAcceptFileTypesHtml((XVar)(this.field)), "\" ", (XVar.Pack((XVar)((XVar)(mode == Constants.MODE_INLINE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  && (XVar)(this.is508 == true)) ? XVar.Pack(MVCFunctions.Concat("alt=\"", this.strLabel, "\" ")) : XVar.Pack("")), " name=\"", this.cfield, "\" >", strfilename));
			MVCFunctions.Echo(MVCFunctions.Concat("<input type=\"Hidden\" id=\"notempty_", this.cfieldname, "\" value=\"", (XVar.Pack(MVCFunctions.strlen((XVar)(value))) ? XVar.Pack(1) : XVar.Pack(0)), "\">"));
			this.buildControlEnd((XVar)(validate), (XVar)(mode));

			return null;
		}
		public virtual XVar getFileOrImageMarkup(dynamic _param_fileData, dynamic _param_keylink)
		{
			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic altAttr = null, fileName = null, urls = XVar.Array();
			fileName = XVar.Clone(fileData["usrName"]);
			urls = XVar.Clone(this.getFileUrls((XVar)(fileData), (XVar)(keylink)));
			if(XVar.Pack(!(XVar)(urls["url"])))
			{
				return "";
			}
			if(XVar.Pack(!(XVar)(CommonFunctions.CheckImageExtension((XVar)(fileName)))))
			{
				return MVCFunctions.Concat("<a target=\"_blank\" href=\"", MVCFunctions.runner_htmlspecialchars((XVar)(urls["url"])), "\">", MVCFunctions.runner_htmlspecialchars((XVar)(fileName)), "</a>");
			}
			if(XVar.Pack(!(XVar)(urls["thumbnail"])))
			{
				urls.InitAndSetArrayItem(urls["url"], "thumbnail");
			}
			altAttr = XVar.Clone(MVCFunctions.Concat(" alt=\"", MVCFunctions.runner_htmlspecialchars((XVar)(fileName)), "\""));
			return MVCFunctions.Concat("<a target=\"_blank\" href=\"", MVCFunctions.runner_htmlspecialchars((XVar)(urls["url"])), "\" >", "<img class=\"r-editfile-img\" ", altAttr, " border=0 src=\"", MVCFunctions.runner_htmlspecialchars((XVar)(urls["thumbnail"])), "\"></a>");
		}
		public override XVar readWebValue(dynamic avalues, dynamic blobfields, dynamic _param_legacy1, dynamic _param_legacy2, dynamic filename_values)
		{
			#region pass-by-value parameters
			dynamic legacy1 = XVar.Clone(_param_legacy1);
			dynamic legacy2 = XVar.Clone(_param_legacy2);
			#endregion

			this.getPostValueAndType();
			if(XVar.Pack(MVCFunctions.FieldSubmitted((XVar)(MVCFunctions.Concat(this.goodFieldName, "_", this.id)))))
			{
				dynamic fileNameForPrepareFunc = null;
				fileNameForPrepareFunc = XVar.Clone(CommonFunctions.securityCheckFileName((XVar)(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("filename_", this.goodFieldName, "_", this.id))))));
				if(this.pageObject.pageType != Constants.PAGE_EDIT)
				{
					this.webValue = XVar.Clone(MVCFunctions.prepare_upload((XVar)(this.field), new XVar("upload2"), (XVar)(fileNameForPrepareFunc), (XVar)(fileNameForPrepareFunc), new XVar(""), (XVar)(this.id), (XVar)(this.pageObject)));
				}
				else
				{
					if(MVCFunctions.substr((XVar)(this.webType), new XVar(0), new XVar(4)) == "file")
					{
						dynamic prepearedFile = XVar.Array();
						prepearedFile = XVar.Clone(MVCFunctions.prepare_file((XVar)(this.webValue), (XVar)(this.field), (XVar)(this.webType), (XVar)(fileNameForPrepareFunc), (XVar)(this.id)));
						if(!XVar.Equals(XVar.Pack(prepearedFile), XVar.Pack(false)))
						{
							dynamic filename = null;
							this.webValue = XVar.Clone(prepearedFile["value"]);
							filename = XVar.Clone(prepearedFile["filename"]);
						}
						else
						{
							this.webValue = new XVar(false);
						}
					}
					else
					{
						if(MVCFunctions.substr((XVar)(this.webType), new XVar(0), new XVar(6)) == "upload")
						{
							if(XVar.Pack(fileNameForPrepareFunc))
							{
								this.webValue = XVar.Clone(fileNameForPrepareFunc);
							}
							if(this.webType == "upload1")
							{
								dynamic oldValues = XVar.Array();
								oldValues = XVar.Clone(this.pageObject.getOldRecordData());
								fileNameForPrepareFunc = XVar.Clone(oldValues[this.field]);
							}
							this.webValue = XVar.Clone(MVCFunctions.prepare_upload((XVar)(this.field), (XVar)(this.webType), (XVar)(fileNameForPrepareFunc), (XVar)(this.webValue), new XVar(""), (XVar)(this.id), (XVar)(this.pageObject)));
						}
					}
				}
			}
			else
			{
				this.webValue = new XVar(false);
			}
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(this.webValue), XVar.Pack(false)))))
			{
				if((XVar)(this.webValue)  && (XVar)(this.pageObject.pSetEdit.getCreateThumbnail((XVar)(this.field))))
				{
					dynamic contents = null, ext = null;
					contents = XVar.Clone(MVCFunctions.GetUploadedFileContents((XVar)(MVCFunctions.Concat("value_", this.goodFieldName, "_", this.id))));
					ext = XVar.Clone(CommonFunctions.CheckImageExtension((XVar)(MVCFunctions.GetUploadedFileName((XVar)(MVCFunctions.Concat("value_", this.goodFieldName, "_", this.id))))));
					if(XVar.Pack(ext))
					{
						dynamic thumb = null;
						thumb = XVar.Clone(MVCFunctions.CreateThumbnail((XVar)(contents), (XVar)(this.pageObject.pSetEdit.getThumbnailSize((XVar)(this.field))), (XVar)(ext)));
						this.pageObject.filesToSave.InitAndSetArrayItem(new SaveFile((XVar)(thumb), (XVar)(MVCFunctions.Concat(this.pageObject.pSetEdit.getStrThumbnail((XVar)(this.field)), this.webValue)), (XVar)(this.pageObject.pSetEdit.getUploadFolder((XVar)(this.field))), (XVar)(this.pageObject.pSetEdit.isAbsolute((XVar)(this.field)))), null);
					}
				}
				avalues.InitAndSetArrayItem(this.webValue, this.field);
			}

			return null;
		}
		public override XVar makeWidthStyle(dynamic _param_widthPx)
		{
			#region pass-by-value parameters
			dynamic widthPx = XVar.Clone(_param_widthPx);
			#endregion

			if(XVar.Pack(0) == widthPx)
			{
				return "";
			}
			return MVCFunctions.Concat("min-width: ", widthPx, "px");
		}
		protected virtual XVar getFileData(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return RunnerFileHandler.getFileArray((XVar)(value), (XVar)(this.field), (XVar)(this.pageObject.pSet));
		}
		protected virtual XVar getFileUrls(dynamic _param_fileData, dynamic _param_keylink)
		{
			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic additionalParams = XVar.Array(), file = XVar.Array(), fs = null, fsInfo = XVar.Array(), lastModified = null, ret = XVar.Array(), var_params = XVar.Array();
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pageObject.pSet);
			fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(pSet), (XVar)(this.field)));
			fsInfo = XVar.Clone(fs.getFileInfo((XVar)(fileData["name"])));
			if(XVar.Pack(!(XVar)(fsInfo)))
			{
				return XVar.Array();
			}
			lastModified = XVar.Clone(MVCFunctions.time());
			if(XVar.Pack(fsInfo["lastModified"]))
			{
				lastModified = XVar.Clone(fsInfo["lastModified"]);
			}
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(fileData["usrName"], "file");
			var_params.InitAndSetArrayItem(pSet.table(), "table");
			var_params.InitAndSetArrayItem(this.field, "field");
			var_params.InitAndSetArrayItem(CommonFunctions.fileAttrHash((XVar)(keylink), (XVar)(file["size"]), (XVar)(lastModified)), "hash");
			foreach (KeyValuePair<XVar, dynamic> val in additionalParams.GetEnumerator())
			{
				var_params.InitAndSetArrayItem(val.Value, val.Key);
			}
			ret = XVar.Clone(XVar.Array());
			ret.InitAndSetArrayItem(MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat(CommonFunctions.prepareUrlQuery((XVar)(var_params)), keylink))), "url");
			if((XVar)(fileData["thumbnail"])  && (XVar)(fs.getFileInfo((XVar)(fileData["thumbnail"]))))
			{
				var_params.InitAndSetArrayItem(1, "thumbnail");
				ret.InitAndSetArrayItem(MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat(CommonFunctions.prepareUrlQuery((XVar)(var_params)), keylink))), "thumbnail");
			}
			if((XVar)(!(XVar)(ret["thumbnail"]))  && (XVar)(512000 < fsInfo["size"]))
			{
				ret.InitAndSetArrayItem("images/icons/jpg.png", "thumbnail");
			}
			return ret;
		}
	}
}
