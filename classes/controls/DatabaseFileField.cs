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
	public partial class DatabaseFileField : EditControl
	{
		protected static bool skipDatabaseFileFieldCtor = false;
		public DatabaseFileField(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection)
		{
			if(skipDatabaseFileFieldCtor)
			{
				skipDatabaseFileFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.format = XVar.Clone(pageObject.pSetEdit.getEditFormat((XVar)(field)));
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

			dynamic disp = null, strfilename = null, strtype = null;
			base.buildControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			disp = new XVar("");
			strfilename = new XVar("");
			if((XVar)(mode == Constants.MODE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_EDIT))
			{
				dynamic filename = null, itype = null, labelTag = null;
				value = XVar.Clone(this.connection.stripSlashesBinary((XVar)(value)));
				itype = XVar.Clone(MVCFunctions.SupposeImageType((XVar)(value)));
				if(XVar.Pack(itype))
				{
					if((XVar)(this.format == Constants.EDIT_FORMAT_DATABASE_IMAGE)  && (XVar)(!(XVar)(this.pageObject.pSetEdit.showThumbnail((XVar)(this.field)))))
					{
						dynamic imageId = null, imgHeight = null, imgWidth = null, src = null, style = null;
						src = XVar.Clone(MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat("filename=file.jpg&table=", MVCFunctions.RawUrlEncode((XVar)(this.container.tName)), "&field=", MVCFunctions.RawUrlEncode((XVar)(this.field)), "&nodisp=1", this.keylink, "&fileHash=", CommonFunctions.fileAttrHash((XVar)(this.keylink), (XVar)(MVCFunctions.strlen_bin((XVar)(value))))))));
						imgWidth = XVar.Clone(this.container.pSetEdit.getImageWidth((XVar)(this.field)));
						imgHeight = XVar.Clone(this.container.pSetEdit.getImageHeight((XVar)(this.field)));
						style = new XVar("");
						if(XVar.Pack(imgWidth))
						{
							style = MVCFunctions.Concat(style, "max-width:", imgWidth, "px;");
						}
						if(XVar.Pack(imgHeight))
						{
							style = MVCFunctions.Concat(style, "max-height:", imgHeight, "px;");
						}
						imageId = XVar.Clone(CommonFunctions.generatePassword(new XVar(10)));
						if(style != XVar.Pack(""))
						{
							style = XVar.Clone(MVCFunctions.Concat("<style> @media screen and (min-width: 768px) { [data-imageid=\"", imageId, "\"] { ", style, "} } </style>"));
						}
						disp = XVar.Clone(MVCFunctions.Concat(style, "<img class=\"\" data-imageid=\"", imageId, "\" id=\"image_", MVCFunctions.GoodFieldName((XVar)(this.field)), "_", this.id, "\" name=\"", this.cfield, "\""));
						if(XVar.Pack(this.is508))
						{
							disp = MVCFunctions.Concat(disp, " alt=\"Image from DB\"");
						}
						disp = MVCFunctions.Concat(disp, " border=0 src=\"", src, "\">");
					}
					else
					{
						if(XVar.Pack(this.pageObject.pSetEdit.showThumbnail((XVar)(this.field))))
						{
							disp = new XVar("<a target=_blank");
							disp = MVCFunctions.Concat(disp, " href=\"", MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat("page=", this.pageObject.pageName, "&table=", CommonFunctions.GetTableURL((XVar)(this.pageObject.tName)), "&", this.iquery, "&rndVal=", MVCFunctions.rand(new XVar(0), new XVar(32768))))), "\" >");
							disp = MVCFunctions.Concat(disp, "<img class=\"mupload-preview-img\" id=\"image_", MVCFunctions.GoodFieldName((XVar)(this.field)), "_", this.id, "\" name=\"", this.cfield, "\" border=0");
							if(XVar.Pack(this.is508))
							{
								disp = MVCFunctions.Concat(disp, " alt=\"Image from DB\"");
							}
							disp = MVCFunctions.Concat(disp, " src=\"", MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat("page=", this.pageObject.pageName, "&table=", CommonFunctions.GetTableURL((XVar)(this.pageObject.tName)), "&thumbnail=1", "&", this.iquery, "&rndVal=", MVCFunctions.rand(new XVar(0), new XVar(32768))))), "\">");
							disp = MVCFunctions.Concat(disp, "</a>");
						}
						else
						{
							disp = XVar.Clone(MVCFunctions.Concat("<img class=\"mupload-preview-img\" id=\"image_", MVCFunctions.GoodFieldName((XVar)(this.field)), "_", this.id, "\" name=\"", this.cfield, "\""));
							if(XVar.Pack(this.is508))
							{
								disp = MVCFunctions.Concat(disp, " alt=\"Image from DB\"");
							}
							disp = MVCFunctions.Concat(disp, " border=0 src=\"", MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat("table=", CommonFunctions.GetTableURL((XVar)(this.pageObject.tName)), "&page=", this.pageObject.pageName, "&", this.iquery, "&src=1&rndVal=", MVCFunctions.rand(new XVar(0), new XVar(32768))))), "\">");
						}
					}
				}
				else
				{
					if(XVar.Pack(MVCFunctions.strlen((XVar)(value))))
					{
						disp = XVar.Clone(MVCFunctions.Concat("<img class=\"mupload-preview-img\" id=\"image_", MVCFunctions.GoodFieldName((XVar)(this.field)), "_", this.id, "\" name=\"", this.cfield, "\" border=0 "));
						if(XVar.Pack(this.is508))
						{
							disp = MVCFunctions.Concat(disp, " alt=\"file\"");
						}
						disp = MVCFunctions.Concat(disp, " src=\"images/file.gif\">");
					}
				}
				if((XVar)((XVar)(this.format == Constants.EDIT_FORMAT_DATABASE_FILE)  && (XVar)(!(XVar)(itype)))  && (XVar)(MVCFunctions.strlen((XVar)(value))))
				{
					if(XVar.Pack(!(XVar)(filename = XVar.Clone(data[this.pageObject.pSetEdit.getFilenameField((XVar)(this.field))]))))
					{
						filename = new XVar("file.bin");
					}
					disp = XVar.Clone(MVCFunctions.Concat("<a href=\"", MVCFunctions.GetTableLink(new XVar("getfile"), new XVar(""), (XVar)(MVCFunctions.Concat("table=", CommonFunctions.GetTableURL((XVar)(this.pageObject.tName)), "&filename=", MVCFunctions.runner_htmlspecialchars((XVar)(filename)), "&pagename=", MVCFunctions.runner_htmlspecialchars((XVar)(this.pageObject.pSetEdit.pageName())), "&", this.iquery))), "\".>", disp, "</a>"));
				}
				if((XVar)(this.format == Constants.EDIT_FORMAT_DATABASE_FILE)  && (XVar)(this.pageObject.pSetEdit.getFilenameField((XVar)(this.field))))
				{
					if(XVar.Pack(!(XVar)(filename = XVar.Clone(data[this.pageObject.pSetEdit.getFilenameField((XVar)(this.field))]))))
					{
						filename = new XVar("");
					}
					if(mode == Constants.MODE_INLINE_EDIT)
					{
						strfilename = XVar.Clone(MVCFunctions.Concat("<br><label for=\"filename_", this.cfieldname, "\">", CommonFunctions.mlang_message(new XVar("FILENAME")), "</label>&nbsp;&nbsp;<input type=\"text\" ", this.inputStyle, " id=\"filename_", this.cfieldname, "\" name=\"filename_", this.cfieldname, "\" size=\"20\" maxlength=\"50\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(filename)), "\">"));
					}
					else
					{
						strfilename = XVar.Clone(MVCFunctions.Concat("<br><label for=\"filename_", this.cfieldname, "\">", CommonFunctions.mlang_message(new XVar("FILENAME")), "</label>&nbsp;&nbsp;<input type=\"text\" ", this.inputStyle, " id=\"filename_", this.cfieldname, "\" name=\"filename_", this.cfieldname, "\" size=\"20\" maxlength=\"50\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(filename)), "\">"));
					}
				}
				labelTag = new XVar("<label class=\"r-img-control\">");
				if(XVar.Pack(MVCFunctions.strlen((XVar)(value))))
				{
					strtype = XVar.Clone(MVCFunctions.Concat("<br>", labelTag, "<input id=\"", this.ctype, "_keep\" type=\"Radio\" name=\"", this.ctype, "\" value=\"file0\" checked class=\"rnr-uploadtype\">", CommonFunctions.mlang_message(new XVar("KEEP")), "</label>"));
					if((XVar)(MVCFunctions.strlen((XVar)(value)))  && (XVar)(!(XVar)(this.pageObject.pSetEdit.isRequired((XVar)(this.field)))))
					{
						strtype = MVCFunctions.Concat(strtype, labelTag, "<input id=\"", this.ctype, "_delete\" type=\"Radio\" name=\"", this.ctype, "\" value=\"file1\" class=\"rnr-uploadtype\">", CommonFunctions.mlang_message(new XVar("DELETE")), "</label>");
					}
					strtype = MVCFunctions.Concat(strtype, labelTag, "<input id=\"", this.ctype, "_update\" type=\"Radio\" name=\"", this.ctype, "\" value=\"file2\" class=\"rnr-uploadtype\">", CommonFunctions.mlang_message(new XVar("UPDATE")), "</label>");
				}
				else
				{
					strtype = XVar.Clone(MVCFunctions.Concat("<input id=\"", this.ctype, "_update\" type=\"hidden\" name=\"", this.ctype, "\" value=\"file2\" class=\"rnr-uploadtype\">"));
				}
			}
			else
			{
				strtype = XVar.Clone(MVCFunctions.Concat("<input id=\"", this.ctype, "\" type=\"hidden\" name=\"", this.ctype, "\" value=\"file2\">"));
				if((XVar)(this.format == Constants.EDIT_FORMAT_DATABASE_FILE)  && (XVar)(this.pageObject.pSetEdit.getFilenameField((XVar)(this.field))))
				{
					strfilename = XVar.Clone(MVCFunctions.Concat("<br><label for=\"filename_", this.cfieldname, "\">", CommonFunctions.mlang_message(new XVar("FILENAME")), "</label>&nbsp;&nbsp;<input type=\"text\" ", this.inputStyle, " id=\"filename_", this.cfieldname, "\" name=\"filename_", this.cfieldname, "\" size=\"20\" maxlength=\"50\">"));
				}
			}
			if((XVar)(mode == Constants.MODE_INLINE_EDIT)  && (XVar)(this.format == Constants.EDIT_FORMAT_DATABASE_FILE))
			{
				disp = new XVar("");
			}
			MVCFunctions.Echo(MVCFunctions.Concat(disp, strtype));
			if((XVar)((XVar)(mode == Constants.MODE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_EDIT))  && (XVar)(MVCFunctions.strlen((XVar)(value))))
			{
				MVCFunctions.Echo("<br>");
			}
			MVCFunctions.Echo(MVCFunctions.Concat("<input type=\"File\" ", this.inputStyle, " id=\"", this.cfield, "\" ", "accept=\"", this.pageObject.pSetEdit.getAcceptFileTypesHtml((XVar)(this.field)), "\" ", (XVar.Pack((XVar)((XVar)(mode == Constants.MODE_INLINE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  && (XVar)(this.is508)) ? XVar.Pack(MVCFunctions.Concat("alt=\"", this.strLabel, "\" ")) : XVar.Pack("")), " name=\"", this.cfield, "\" >", strfilename));
			MVCFunctions.Echo(MVCFunctions.Concat("<input type=\"Hidden\" id=\"notempty_", this.cfieldname, "\" value=\"", (XVar.Pack(MVCFunctions.strlen((XVar)(value))) ? XVar.Pack(1) : XVar.Pack(0)), "\">"));
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

			dynamic filename = null;
			filename = new XVar("");
			this.getPostValueAndType();
			if(XVar.Pack(MVCFunctions.FieldSubmitted((XVar)(MVCFunctions.Concat(this.goodFieldName, "_", this.id)))))
			{
				dynamic fileNameForPrepareFunc = null, prepearedFile = XVar.Array();
				fileNameForPrepareFunc = XVar.Clone(CommonFunctions.securityCheckFileName((XVar)(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("filename_", this.goodFieldName, "_", this.id))))));
				if((XVar)(this.pageObject.pageType != Constants.PAGE_EDIT)  && (XVar)(this.pageObject.pageType != Constants.PAGE_USERINFO))
				{
					prepearedFile = XVar.Clone(MVCFunctions.prepare_file((XVar)(this.webValue), (XVar)(this.field), new XVar("file2"), (XVar)(fileNameForPrepareFunc), (XVar)(this.id)));
					if(!XVar.Equals(XVar.Pack(prepearedFile), XVar.Pack(false)))
					{
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
					if(MVCFunctions.substr((XVar)(this.webType), new XVar(0), new XVar(4)) == "file")
					{
						prepearedFile = XVar.Clone(MVCFunctions.prepare_file((XVar)(this.webValue), (XVar)(this.field), (XVar)(this.webType), (XVar)(fileNameForPrepareFunc), (XVar)(this.id)));
						if(!XVar.Equals(XVar.Pack(prepearedFile), XVar.Pack(false)))
						{
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
							if(this.webType == "upload1")
							{
								dynamic oldValues = XVar.Array();
								oldValues = XVar.Clone(this.pageObject.getOldRecordData());
								fileNameForPrepareFunc = XVar.Clone(oldValues[this.field]);
							}
							this.webValue = XVar.Clone(MVCFunctions.prepare_upload((XVar)(this.field), (XVar)(this.webType), (XVar)(fileNameForPrepareFunc), (XVar)(this.webValue), new XVar(""), (XVar)(this.id), (XVar)(this.pageObject)));
						}
						else
						{
							this.webValue = new XVar(false);
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
				dynamic thumbField = null;
				if(XVar.Pack(this.webValue))
				{
					dynamic ext = null, resizeImageSize = null;
					if(XVar.Pack(this.pageObject.pSetEdit.getCreateThumbnail((XVar)(this.field))))
					{
						ext = XVar.Clone(CommonFunctions.CheckImageExtension((XVar)(MVCFunctions.GetUploadedFileName((XVar)(MVCFunctions.Concat("value_", this.goodFieldName, "_", this.id))))));
						if(XVar.Pack(ext))
						{
							dynamic thumb = null;
							thumb = XVar.Clone(MVCFunctions.CreateThumbnail((XVar)(this.webValue), (XVar)(this.pageObject.pSetEdit.getThumbnailSize((XVar)(this.field))), (XVar)(ext)));
							thumbField = XVar.Clone(this.pageObject.pSetEdit.getThumbnailField((XVar)(this.field)));
							blobfields.InitAndSetArrayItem(thumbField, null);
							avalues.InitAndSetArrayItem(thumb, thumbField);
						}
					}
					resizeImageSize = new XVar(0);
					if(XVar.Pack(this.pageObject.pSetEdit.getResizeOnUpload((XVar)(this.field))))
					{
						resizeImageSize = XVar.Clone(this.pageObject.pSetEdit.getNewImageSize((XVar)(this.field)));
					}
					else
					{
						if(XVar.Pack(this.fieldIsUserpic()))
						{
							resizeImageSize = new XVar(400);
						}
					}
					if(XVar.Pack(resizeImageSize))
					{
						ext = XVar.Clone(CommonFunctions.CheckImageExtension((XVar)(MVCFunctions.GetUploadedFileName((XVar)(MVCFunctions.Concat("value_", this.goodFieldName, "_", this.id))))));
						this.webValue = XVar.Clone(MVCFunctions.CreateThumbnail((XVar)(this.webValue), (XVar)(resizeImageSize), (XVar)(ext)));
					}
				}
				else
				{
					if((XVar)(this.pageObject.pageType == Constants.PAGE_EDIT)  && (XVar)(this.pageObject.pSetEdit.getCreateThumbnail((XVar)(this.field))))
					{
						thumbField = XVar.Clone(this.pageObject.pSetEdit.getThumbnailField((XVar)(this.field)));
						blobfields.InitAndSetArrayItem(thumbField, null);
						avalues.InitAndSetArrayItem("", thumbField);
					}
				}
				blobfields.InitAndSetArrayItem(this.field, null);
				avalues.InitAndSetArrayItem(this.webValue, this.field);
			}
			if((XVar)(filename)  && (XVar)(this.pageObject.pSetEdit.getFilenameField((XVar)(this.field))))
			{
				filename_values.InitAndSetArrayItem(filename, this.pageObject.pSetEdit.getFilenameField((XVar)(this.field)));
			}

			return null;
		}
		protected virtual XVar fieldIsUserpic()
		{
			return (XVar)(XVar.Equals(XVar.Pack(this.field), XVar.Pack(Security.userpicField())))  && (XVar)(XVar.Equals(XVar.Pack(this.container.tName), XVar.Pack(Security.loginTable())));
		}
	}
}
