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
	public partial class FileField : EditControl
	{
		public dynamic formStamp = XVar.Pack("");
		protected static bool skipFileFieldCtor = false;
		public FileField(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection)
		{
			if(skipFileFieldCtor)
			{
				skipFileFieldCtor = false;
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
			if(XVar.Pack(this.format = new XVar(Constants.EDIT_FORMAT_FILE)))
			{
				this.pageObject.AddJSFile(new XVar("include/mupload.js"));
			}

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

			dynamic fh = null, filesArray = null, jsonValue = null, keylink = null, multiple = null, userFilesArray = null;
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
			this.formStamp = XVar.Clone(CommonFunctions.generatePassword(new XVar(15)));
			filesArray = XVar.Clone(this.getFileData((XVar)(value)));
			keylink = new XVar("");
			if(this.pageObject.pageType == Constants.PAGE_EDIT)
			{
				if(XVar.Pack(this.pageObject.keys))
				{
					dynamic i = null;
					i = new XVar(1);
					foreach (KeyValuePair<XVar, dynamic> keyValue in this.pageObject.keys.GetEnumerator())
					{
						keylink = MVCFunctions.Concat(keylink, "&key", i, "=", MVCFunctions.RawUrlEncode((XVar)(keyValue.Value)));
						i++;
					}
				}
			}
			fh = XVar.Clone(new RunnerFileHandler((XVar)(this.field), (XVar)(this.pageObject.pSet), (XVar)(this.formStamp)));
			userFilesArray = XVar.Clone(fh.loadFiles((XVar)(filesArray)));
			jsonValue = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(userFilesArray)));
			multiple = new XVar("");
			if(this.pageObject.pSetEdit.getMaxNumberOfFiles((XVar)(this.field)) != 1)
			{
				multiple = new XVar("multiple ");
			}
			MVCFunctions.Echo(MVCFunctions.Concat("\r\n <!-- The file upload form used as target for the file upload widget -->\r\n    <form id=\"fileupload_", this.cfieldname, "\" action=\"", MVCFunctions.GetTableLink(new XVar("mfhandler")), "\" method=\"POST\" enctype=\"multipart/form-data\">\r\n\r\n    <input type=\"hidden\" name=\"formStamp_", this.cfieldname, "\" id=\"formStamp_", this.cfieldname, "\" value=\"", this.formStamp, "\" />\r\n    <input type=\"hidden\" name=\"_action\" value=\"POST\" />\r\n    <input type=\"hidden\" id=\"value_", this.cfieldname, "\" name=\"value_", this.cfieldname, "\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(jsonValue)), "\" />\r\n\r\n    <!-- The fileupload-buttonbar contains buttons to add/delete files and start/cancel the upload -->\r\n        <div class=\"fileupload-buttonbar\">\r\n            <div class=\"span7\">\r\n                <!-- The fileinput-button span is used to style the file input field as button -->\r\n \t\t\t\t<SPAN class=\"btn btn-primary btn-sm fileinput-button filesUpload\">\r\n\t\t\t\t\t<!--<A class=\"rnr-button filesUpload button\" href=\"#\" >-->\r\n\t\t\t\t\t<input class=\"fileinput-button-input\" type=\"file\" accept=\"", this.pageObject.pSetEdit.getAcceptFileTypesHtml((XVar)(this.field)), "\" name=\"files[]\" value=\"", CommonFunctions.mlang_message(new XVar("ADD_FILES")), "\" ", multiple, " />", CommonFunctions.mlang_message(new XVar("ADD_FILES")), "<!--</A>-->\r\n\t\t\t\t</SPAN>", "\r\n\r\n            </div>\r\n            <!-- The global progress information -->\r\n            <div class=\"fileupload-progress fade\">\r\n                <!-- The global progress bar -->\r\n                <div class=\"progress\" role=\"progressbar\" aria-valuemin=\"0\" aria-valuenow=\"0\" aria-valuemax=\"100\">\r\n                    <div style=\"width:0;\" class=\"bar progress-bar progress-bar-info progress-bar-striped active\"  ></div>\r\n                </div>\r\n                <!-- The extended global progress information -->\r\n                <div class=\"progress-extended\">&nbsp;</div>\r\n            </div>\r\n        </div>\r\n        <!-- The loading indicator is shown during file processing -->\r\n        <div class=\"fileupload-loading\"></div>\r\n        <!-- The table listing the files available for upload/download -->\r\n        <table class=\"mupload-files\"><tbody class=\"files\"></tbody></table>\r\n    </form>\r\n    "));
			if(XVar.Pack(!(XVar)(this.container.globalVals.KeyExists("muploadTemplateIncluded"))))
			{
				MVCFunctions.Echo(MVCFunctions.Concat("<script type=\"text/x-tmpl\" id=\"template-download\">{% for (var i=0, file; file=o.files[i]; i++) { %}\r\n    <tr class=\"template-download fade\">\r\n        {% if (file.error) { %}\r\n            <td></td>\r\n            <td class=\"name\"><span class=\"text-muted\">{%=file.name%}</span></td>\r\n            <td class=\"size\"><span class=\"text-muted\" dir=\"LTR\">{%=o.formatFileSize(file.size)%}</span></td>\r\n            <td colspan=2 class=\"error\"><span class=\"text-danger rnr-error\">", CommonFunctions.mlang_message(new XVar("ERROR")), " {%=window.locale && window.locale.fileupload && window.locale.fileupload.errors[file.error] || file.error%}</span></td>\r\n        {% } else { %}\r\n            <td class=\"preview\">{% if (file.thumbnail_url) { %}\r\n                <a href=\"{%=file.url%}\" title=\"{%=file.name%}\" rel=\"gallery\" download=\"{%=file.name%}\"\r\n                \t{% if (!file.isIco) { %} {% } %}\r\n                \t><img class=\"mupload-preview-img\" src=\"{%=file.thumbnail_url%}&src=1\"></a>\r\n            {% } else { %}\r\n            \t{% if (file.isImg) { %}\r\n            \t\t<a href=\"{%=file.url%}&nodisp=1\" title=\"{%=file.name%}\" rel=\"gallery\" download=\"{%=file.name%}\" ><img class=\"mupload-preview-img\" src=\"{%=file.url%}&src=1\"></a>\r\n            \t{% } %}\r\n            {% } %}</td>\r\n            <td class=\"name\">\r\n                <a href=\"{%=file.url%}\" title=\"{%=file.name%}\" rel=\"{%=file.thumbnail_url&&'gallery'%}\" download=\"{%=file.name%}\">{%=file.name%}</a>\r\n            </td>\r\n            <td class=\"size\"><span dir=\"LTR\">{%=o.formatFileSize(file.size)%}</span></td>\r\n\t\t\t<td></td>\r\n\t\t\t<td class=\"delete\">\r\n\t\t\t\t{% if (!file.error) { %}\r\n\t\t\t\t<SPAN class=\"btn btn-xs btn-default delete\" data-type=\"{%=file.delete_type%}\" data-url=\"{%=file.delete_url%}\" data-name=\"{%=file.name%}\">\r\n\t\t\t\t<!--<A href=\"#\" >-->", CommonFunctions.mlang_message(new XVar("DELETE")), "<!--</A>-->\r\n\t\t\t\t\t</SPAN>\r\n\t\t\t\t{% } %}\r\n\t\t\t</td>\r\n        {% } %}\r\n    </tr>\r\n{% } %}\r\n</script>\r\n<script type=\"text/x-tmpl\" id=\"template-upload\">{% for (var i=0, file; file=o.files[i]; i++) { %}\r\n    <tr class=\"template-upload fade\">\r\n        <td class=\"preview\"><span class=\"fade\"></span></td>\r\n        {% if (file.error) { %}\r\n\t\t\t<td class=\"name\"><span class=\"text-muted\">{%=file.name%}</span></td>\r\n\t\t\t<td class=\"size\"><span class=\"text-muted\">{%=o.formatFileSize(file.size)%}</span></td>\r\n            <td class=\"error\" colspan=\"2\"><span class=\"text-danger rnr-error\">", CommonFunctions.mlang_message(new XVar("ERROR")), " {%=window.locale && window.locale.fileupload && window.locale.fileupload.errors[file.error] || file.error%}</span></td>\r\n        {% } else if (!i) { %}\r\n\t\t\t<td class=\"name\"><span>{%=file.name%}</span></td>\r\n\t\t\t<td class=\"size\"><span>{%=o.formatFileSize(file.size)%}</span></td>\r\n            <td>\r\n                <div class=\"progress progress-success progress-striped active\" role=\"progressbar\" aria-valuemin=\"0\"\r\n                \taria-valuemax=\"100\" aria-valuenow=\"0\"><div class=\"progress-bar bar\" style=\"width:0;\"></div></div>\r\n            </td>\r\n        {% } else { %}\r\n            <td></td>\r\n        {% } %}\r\n        <td class=\"cancel\">{% if (!i) { %}\r\n        \t{% if (!file.error) { %}\r\n        \t<SPAN class=\"btn btn-default btn-xs\">\r\n\t\t\t<!--<A href=\"#\" >-->", CommonFunctions.mlang_message(new XVar("CANCEL")), "<!--</A>-->\r\n\t\t\t\t</SPAN>\r\n\t\t\t{% } %}\r\n        {% } %}</td>\r\n    </tr>\r\n{% } %}</script>"));
				this.container.globalVals.InitAndSetArrayItem(true, "muploadTemplateIncluded");
			}
			this.buildControlEnd((XVar)(validate), (XVar)(mode));

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
		public override XVar readWebValue(dynamic avalues, dynamic blobfields, dynamic _param_legacy1, dynamic _param_legacy2, dynamic filename_values)
		{
			#region pass-by-value parameters
			dynamic legacy1 = XVar.Clone(_param_legacy1);
			dynamic legacy2 = XVar.Clone(_param_legacy2);
			#endregion

			this.getPostValueAndType();
			this.formStamp = XVar.Clone(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("formStamp_", this.goodFieldName, "_", this.id))));
			if((XVar)(MVCFunctions.FieldSubmitted((XVar)(MVCFunctions.Concat(this.goodFieldName, "_", this.id))))  && (XVar)(this.formStamp != ""))
			{
				dynamic filesArray = XVar.Array();
				filesArray = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(this.webValue)));
				if((XVar)(!(XVar)(MVCFunctions.is_array((XVar)(filesArray))))  || (XVar)(MVCFunctions.count(filesArray) == 0))
				{
					this.webValue = new XVar("");
				}
				else
				{
					dynamic result = XVar.Array(), searchStr = null, uploadDir = null;
					if(0 < MVCFunctions.count(XSession.Session[MVCFunctions.Concat("mupload_", this.formStamp)]))
					{
						foreach (KeyValuePair<XVar, dynamic> fileArray in XSession.Session[MVCFunctions.Concat("mupload_", this.formStamp)].GetEnumerator())
						{
							fileArray.Value.InitAndSetArrayItem(true, "deleted");
						}
					}
					result = XVar.Clone(XVar.Array());
					uploadDir = XVar.Clone(this.pageObject.pSetEdit.getLinkPrefix((XVar)(this.field)));
					searchStr = new XVar("");
					foreach (KeyValuePair<XVar, dynamic> file in filesArray.GetEnumerator())
					{
						if(XVar.Pack(XSession.Session[MVCFunctions.Concat("mupload_", this.formStamp)].KeyExists(file.Value["name"])))
						{
							dynamic sessionFile = XVar.Array();
							sessionFile = XVar.Clone(XSession.Session[MVCFunctions.Concat("mupload_", this.formStamp)][file.Value["name"]]["file"]);
							searchStr = MVCFunctions.Concat(searchStr, file.Value["name"], ",!");
							result.InitAndSetArrayItem(new XVar("name", sessionFile["name"], "usrName", file.Value["name"], "size", sessionFile["size"], "type", sessionFile["type"]), null);
							if((XVar)(this.pageObject.pSetEdit.getCreateThumbnail((XVar)(this.field)))  && (XVar)(sessionFile["thumbnail"] != ""))
							{
								dynamic lastIndex = null;
								lastIndex = XVar.Clone(MVCFunctions.count(result) - 1);
								result.InitAndSetArrayItem(sessionFile["thumbnail"], lastIndex, "thumbnail");
								result.InitAndSetArrayItem(sessionFile["thumbnail_type"], lastIndex, "thumbnail_type");
								result.InitAndSetArrayItem(sessionFile["thumbnail_size"], lastIndex, "thumbnail_size");
							}
							XSession.Session.InitAndSetArrayItem(false, MVCFunctions.Concat("mupload_", this.formStamp), file.Value["name"], "deleted");
						}
					}
					if(0 < MVCFunctions.count(result))
					{
						result.InitAndSetArrayItem(MVCFunctions.Concat(searchStr, ":sStrEnd"), 0, "searchStr");
						this.webValue = XVar.Clone(MVCFunctions.runner_json_encode_unescaped_unicode((XVar)(result)));
					}
					else
					{
						this.webValue = new XVar("");
					}
				}
			}
			else
			{
				this.webValue = new XVar(false);
			}
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(this.webValue), XVar.Pack(false)))))
			{
				if(this.connection.dbType == Constants.nDATABASE_Informix)
				{
					if(XVar.Pack(CommonFunctions.IsTextType((XVar)(this.pageObject.pSetEdit.getFieldType((XVar)(this.field))))))
					{
						blobfields.InitAndSetArrayItem(this.field, null);
					}
				}
				avalues.InitAndSetArrayItem(this.webValue, this.field);
			}

			return null;
		}
		public override XVar getSearchOptions(dynamic _param_selOpt, dynamic _param_not, dynamic _param_both)
		{
			#region pass-by-value parameters
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			dynamic isPHPEncripted = null, optionsArray = XVar.Array();
			optionsArray = XVar.Clone(XVar.Array());
			isPHPEncripted = XVar.Clone(this.pageObject.cipherer.isFieldPHPEncrypted((XVar)(this.field)));
			if(XVar.Pack(!(XVar)(isPHPEncripted)))
			{
				optionsArray.InitAndSetArrayItem(Constants.CONTAINS, null);
				optionsArray.InitAndSetArrayItem(Constants.EQUALS, null);
			}
			optionsArray.InitAndSetArrayItem(Constants.EMPTY_SEARCH, null);
			if(XVar.Pack(both))
			{
				if(XVar.Pack(!(XVar)(isPHPEncripted)))
				{
					optionsArray.InitAndSetArrayItem(Constants.NOT_CONTAINS, null);
					optionsArray.InitAndSetArrayItem(Constants.NOT_EQUALS, null);
				}
				optionsArray.InitAndSetArrayItem(Constants.NOT_EMPTY, null);
			}
			return this.buildSearchOptions((XVar)(optionsArray), (XVar)(selOpt), (XVar)(var_not), (XVar)(both));
		}
		public override XVar suggestValue(dynamic _param_value, dynamic _param_searchFor, dynamic var_response, dynamic row)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic searchFor = XVar.Clone(_param_searchFor);
			#endregion

			dynamic filesArray = XVar.Array();
			if(XVar.Pack(!(XVar)(value)))
			{
				return null;
			}
			value = XVar.Clone(MVCFunctions.substr((XVar)(value), new XVar(1)));
			filesArray = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(value)));
			if((XVar)(!(XVar)(MVCFunctions.is_array((XVar)(filesArray))))  || (XVar)(MVCFunctions.count(filesArray) == 0))
			{
				var_response.InitAndSetArrayItem(MVCFunctions.Concat("_", value), MVCFunctions.Concat("_", value));
			}
			else
			{
				dynamic i = null, pos = null;
				i = new XVar(0);
				for(;(XVar)(i < MVCFunctions.count(filesArray))  && (XVar)(MVCFunctions.count(var_response) < 10); i++)
				{
					if(XVar.Pack(this.pageObject.pSetEdit.getNCSearch()))
					{
						pos = XVar.Clone(MVCFunctions.stripos((XVar)(filesArray[i]["usrName"]), (XVar)(searchFor)));
					}
					else
					{
						pos = XVar.Clone(MVCFunctions.strpos((XVar)(filesArray[i]["usrName"]), (XVar)(searchFor)));
					}
					if(!XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
					{
						var_response.InitAndSetArrayItem(MVCFunctions.Concat("_", filesArray[i]["usrName"]), MVCFunctions.Concat("_", filesArray[i]["usrName"]));
					}
				}
			}

			return null;
		}
		public override XVar afterSuccessfulSave()
		{
			dynamic fs = null;
			if(XVar.Pack(!(XVar)(XSession.Session[MVCFunctions.Concat("mupload_", this.formStamp)])))
			{
				return null;
			}
			fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(this.pageObject.pSet), (XVar)(this.field)));
			foreach (KeyValuePair<XVar, dynamic> fileArray in XSession.Session[MVCFunctions.Concat("mupload_", this.formStamp)].GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(fileArray.Value["deleted"])))
				{
					continue;
				}
				fs.delete((XVar)(fileArray.Value["file"]["name"]));
				if(XVar.Pack(fileArray.Value["file"]["thumbnail"]))
				{
					fs.delete((XVar)(fileArray.Value["file"]["thumbnail"]));
				}
			}
			XSession.Session.Remove(MVCFunctions.Concat("mupload_", this.formStamp));

			return null;
		}
		public override XVar getFieldValueCopy(dynamic _param_fieldValue)
		{
			#region pass-by-value parameters
			dynamic fieldValue = XVar.Clone(_param_fieldValue);
			#endregion

			dynamic filesData = XVar.Array(), fs = null, uploadFolder = null;
			fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(this.pageObject.pSet), (XVar)(this.field)));
			if(XVar.Pack(!(XVar)(fs.fast())))
			{
				return "[]";
			}
			uploadFolder = XVar.Clone(this.pageObject.pSetEdit.getUploadFolder((XVar)(this.field)));
			filesData = XVar.Clone(this.getFileData((XVar)(fieldValue)));
			foreach (KeyValuePair<XVar, dynamic> idx in MVCFunctions.array_keys((XVar)(filesData)).GetEnumerator())
			{
				dynamic file = XVar.Array(), newName = null;
				file = filesData[idx.Value];
				newName = XVar.Clone(fs.copyFile((XVar)(file["name"]), (XVar)(file["usrName"])));
				if(XVar.Pack(!(XVar)(newName)))
				{
					continue;
				}
				file.InitAndSetArrayItem(newName, "name");
				if((XVar)(this.pageObject.pSetEdit.getCreateThumbnail((XVar)(this.field)))  && (XVar)(file["thumbnail"]))
				{
					dynamic newThumbnail = null, thumbnailPrefix = null;
					thumbnailPrefix = XVar.Clone(this.pageObject.pSetEdit.getStrThumbnail((XVar)(this.field)));
					newThumbnail = XVar.Clone(fs.copyFile((XVar)(file["thumbnail"]), (XVar)(MVCFunctions.Concat(thumbnailPrefix, file["usrName"]))));
					if(XVar.Pack(newThumbnail))
					{
						file.InitAndSetArrayItem(newThumbnail, "thumbnail");
					}
					else
					{
						file.Remove("thumbnail");
					}
				}
			}
			return MVCFunctions.runner_json_encode((XVar)(filesData));
		}
		public override XVar getBasicFieldCondition(dynamic _param_searchFor, dynamic _param_strSearchOption, dynamic _param_searchFor2 = null, dynamic _param_etype = null)
		{
			#region default values
			if(_param_searchFor2 as Object == null) _param_searchFor2 = new XVar("");
			if(_param_etype as Object == null) _param_etype = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic strSearchOption = XVar.Clone(_param_strSearchOption);
			dynamic searchFor2 = XVar.Clone(_param_searchFor2);
			dynamic etype = XVar.Clone(_param_etype);
			#endregion

			if(strSearchOption == Constants.EQUALS)
			{
				return this.getFilenameCondition(new XVar(Constants.dsopEQUAL), (XVar)(searchFor));
			}
			else
			{
				if(strSearchOption == Constants.STARTS_WITH)
				{
					return this.getFilenameCondition(new XVar(Constants.dsopSTART), (XVar)(searchFor));
				}
				else
				{
					if(strSearchOption == Constants.CONTAINS)
					{
						return this.getFilenameCondition(new XVar(Constants.dsopCONTAIN), (XVar)(searchFor));
					}
					else
					{
						if(strSearchOption == Constants.EMPTY_SEARCH)
						{
							return DataCondition.FieldIs((XVar)(this.field), new XVar(Constants.dsopEMPTY), (XVar)(searchFor));
						}
					}
				}
			}
			return null;
		}
		protected virtual XVar getFilenameCondition(dynamic _param_operation, dynamic _param_searchFor)
		{
			#region pass-by-value parameters
			dynamic operation = XVar.Clone(_param_operation);
			dynamic searchFor = XVar.Clone(_param_searchFor);
			#endregion

			dynamic after = null, before = null, caseInsensitive = null, fileSearchFor = null, likeWrapper = null, startCondition = null;
			caseInsensitive = XVar.Clone((XVar.Pack(this.pageObject.pSetEdit.getNCSearch()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_DEFAULT)));
			startCondition = XVar.Clone(DataCondition.FieldIs((XVar)(this.field), new XVar(Constants.dsopSTART), new XVar("[{"), (XVar)(caseInsensitive)));
			likeWrapper = new XVar(null);
			before = new XVar("searchStr\":\"");
			after = new XVar(":sStrEnd\"");
			if(operation == Constants.dsopEQUAL)
			{
				fileSearchFor = XVar.Clone(MVCFunctions.Concat(before, searchFor, ",!", after));
			}
			else
			{
				if(operation == Constants.dsopSTART)
				{
					fileSearchFor = XVar.Clone(MVCFunctions.Concat(before, searchFor));
					likeWrapper = XVar.Clone(new XVar("after", after));
				}
				else
				{
					fileSearchFor = XVar.Clone(searchFor);
					likeWrapper = XVar.Clone(new XVar("before", before, "after", after));
				}
			}
			return new DsCondition((XVar)(new XVar(0, new DsOperand(new XVar(Constants.dsotCONDITION), (XVar)(DataCondition._And((XVar)(new XVar(0, startCondition, 1, DataCondition.FieldIs((XVar)(this.field), new XVar(Constants.dsopCONTAIN), (XVar)(fileSearchFor), (XVar)(caseInsensitive), new XVar(0), (XVar)(likeWrapper))))))), 1, new DsOperand(new XVar(Constants.dsotCONDITION), (XVar)(DataCondition._And((XVar)(new XVar(0, DataCondition._Not((XVar)(startCondition)), 1, DataCondition.FieldIs((XVar)(this.field), (XVar)(operation), (XVar)(searchFor), (XVar)(caseInsensitive))))))))), new XVar(Constants.dsopOR), (XVar)(caseInsensitive));
		}
		protected virtual XVar getFileData(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return RunnerFileHandler.getFileArray((XVar)(value), (XVar)(this.field), (XVar)(this.pageObject.pSet));
		}
	}
}
