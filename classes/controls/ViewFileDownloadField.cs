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
	public partial class ViewFileDownloadField : ViewFileField
	{
		public dynamic sizeUnits = XVar.Array();
		protected static bool skipViewFileDownloadFieldCtor = false;
		public ViewFileDownloadField(dynamic _param_field, dynamic _param_container, dynamic _param_pageobject)
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageobject)
		{
			if(skipViewFileDownloadFieldCtor)
			{
				skipViewFileDownloadFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic container = XVar.Clone(_param_container);
			dynamic pageobject = XVar.Clone(_param_pageobject);
			#endregion

			dynamic pageObject = null;
			this.sizeUnits = XVar.Clone(new XVar(0, "KB", 1, "MB", 2, "GB", 3, "TB"));
		}
		public override XVar addJSFiles()
		{
			if(XVar.Pack(this.container.pSet.showThumbnail((XVar)(this.field))))
			{
				this.getJSControl();
			}

			return null;
		}
		public override XVar addCSSFiles()
		{

			return null;
		}
		protected override XVar getElementTextValue(dynamic _param_fileData, dynamic data)
		{
			#region pass-by-value parameters
			dynamic fileData = XVar.Clone(_param_fileData);
			#endregion

			dynamic displayFileName = null;
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			if(XVar.Pack(pSet.showCustomExpr((XVar)(this.field))))
			{
				return pSet.getFileText((XVar)(this.field), (XVar)(fileData), (XVar)(data));
			}
			displayFileName = XVar.Clone(fileData["usrName"]);
			if(XVar.Pack(pSet.showFileSize((XVar)(this.field))))
			{
				displayFileName = MVCFunctions.Concat(displayFileName, " ", this.fileSizeString((XVar)(fileData["size"])));
			}
			return displayFileName;
		}
		protected virtual XVar makeFileElement(dynamic _param_urlData)
		{
			#region pass-by-value parameters
			dynamic urlData = XVar.Clone(_param_urlData);
			#endregion

			dynamic displayFilename = null, image = null, target = null;
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			target = XVar.Clone((XVar.Pack(urlData["newTab"]) ? XVar.Pack("target=\"_blank\"") : XVar.Pack("")));
			image = new XVar("");
			if(XVar.Pack(urlData["thumbnailMode"]))
			{
				image = XVar.Clone(MVCFunctions.Concat("<img  border=\"0\"", " alt=\"", MVCFunctions.runner_htmlspecialchars((XVar)(urlData["filename"])), "\"", " src=\"", MVCFunctions.runner_htmlspecialchars((XVar)(urlData["imageUrl"])), "\" />"));
			}
			else
			{
				if(XVar.Pack(urlData["imageUrl"]))
				{
					image = XVar.Clone(MVCFunctions.Concat("<img class=\"r-fileicon \" src=\"", MVCFunctions.runner_htmlspecialchars((XVar)(urlData["imageUrl"])), "\" />"));
				}
			}
			if(XVar.Pack(image))
			{
				image = XVar.Clone(MVCFunctions.Concat("<a target=\"_blank\" href=\"", MVCFunctions.runner_htmlspecialchars((XVar)(urlData["url"])), "\">", image, "</a>"));
			}
			if(XVar.Pack(urlData.KeyExists("htmlLabel")))
			{
				displayFilename = XVar.Clone(urlData["htmlLabel"]);
			}
			else
			{
				dynamic htmlLabel = null;
				htmlLabel = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(urlData["filename"])));
				if(XVar.Pack(this.searchHighlight))
				{
					htmlLabel = XVar.Clone(this.highlightSearchWord((XVar)(htmlLabel), new XVar(true), new XVar("")));
				}
				displayFilename = XVar.Clone(MVCFunctions.Concat("<a ", target, " dir=\"LTR\" href=\"", MVCFunctions.runner_htmlspecialchars((XVar)(urlData["url"])), "\">", htmlLabel, "</a>"));
			}
			if(XVar.Pack(urlData.KeyExists("size")))
			{
				displayFilename = MVCFunctions.Concat(displayFilename, " ", this.fileSizeString((XVar)(urlData["size"])));
			}
			if(XVar.Pack(urlData["thumbnailMode"]))
			{
				return MVCFunctions.Concat(image, "<br>", displayFilename);
			}
			return MVCFunctions.Concat(image, displayFilename);
		}
		protected virtual XVar fileSizeString(dynamic _param_filesize)
		{
			#region pass-by-value parameters
			dynamic filesize = XVar.Clone(_param_filesize);
			#endregion

			dynamic fileSizeAndUnit = XVar.Array();
			fileSizeAndUnit = XVar.Clone(this.getFileSizeAndUnits((XVar)(filesize)));
			return MVCFunctions.Concat(CommonFunctions.str_format_number((XVar)((XVar)Math.Round((double)(fileSizeAndUnit["size"]), 2))), " ", this.sizeUnits[fileSizeAndUnit["unitIndex"]]);
		}
		public override XVar showDBValue(dynamic data, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic controls = XVar.Array(), isExport = null, urls = XVar.Array();
			isExport = XVar.Clone((XVar)(this.container.pageType == Constants.PAGE_EXPORT)  || (XVar)(this.container.forExport != ""));
			if((XVar)(!(XVar)(html))  || (XVar)(isExport))
			{
				return this.getTextValue((XVar)(data));
			}
			urls = XVar.Clone(this.getFileURLs((XVar)(data), (XVar)(keylink)));
			controls = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> u in urls.GetEnumerator())
			{
				controls.InitAndSetArrayItem(this.makeFileElement((XVar)(u.Value)), null);
			}
			return MVCFunctions.implode(new XVar("<br>"), (XVar)(controls));
		}
		public virtual XVar getFileSizeAndUnits(dynamic _param_size, dynamic _param_deepLevel = null)
		{
			#region default values
			if(_param_deepLevel as Object == null) _param_deepLevel = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic size = XVar.Clone(_param_size);
			dynamic deepLevel = XVar.Clone(_param_deepLevel);
			#endregion

			dynamic shrinkedSize = null;
			shrinkedSize = XVar.Clone(size / 1024);
			if((XVar)(1024 < shrinkedSize)  && (XVar)(deepLevel < MVCFunctions.count(this.sizeUnits) - 1))
			{
				return this.getFileSizeAndUnits((XVar)(shrinkedSize), (XVar)(deepLevel + 1));
			}
			return new XVar("size", shrinkedSize, "unitIndex", deepLevel);
		}
		public static XVar getFileIconByType(dynamic _param_file_name, dynamic _param_fileType)
		{
			#region pass-by-value parameters
			dynamic file_name = XVar.Clone(_param_file_name);
			dynamic fileType = XVar.Clone(_param_fileType);
			#endregion

			if(XVar.Pack(!(XVar)(fileType)))
			{
				fileType = XVar.Clone(CommonFunctions.mimeTypeByExt((XVar)(CommonFunctions.getFileExtension((XVar)(file_name)))));
			}
			return CommonFunctions.getIconByFileType((XVar)(fileType), (XVar)(file_name));
		}
		protected virtual XVar getFileURLs(dynamic data, dynamic _param_keylink)
		{
			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic fileURLs = XVar.Array(), files = XVar.Array(), fs = null, showThumbnails = null;
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			showThumbnails = XVar.Clone(pSet.showThumbnail((XVar)(this.field)));
			fileURLs = XVar.Clone(XVar.Array());
			fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(pSet), (XVar)(this.field)));
			files = XVar.Clone(this.getFilesData((XVar)(data[this.field])));
			foreach (KeyValuePair<XVar, dynamic> f in files.GetEnumerator())
			{
				dynamic finfo = XVar.Array(), imageUrl = null, thumbnailMode = null, url = null, urlData = XVar.Array();
				finfo = XVar.Clone(this.fastFileInfo((XVar)(f.Value["name"])));
				if(XVar.Pack(!(XVar)(finfo)))
				{
					continue;
				}
				url = XVar.Clone(this.getFileUrl((XVar)(f.Value), (XVar)(keylink), new XVar(false), (XVar)(new XVar("nodisp", 1))));
				thumbnailMode = new XVar(false);
				if((XVar)((XVar)(f.Value["thumbnail"])  && (XVar)(pSet.showThumbnail((XVar)(this.field))))  && (XVar)(CommonFunctions.CheckImageExtension((XVar)(f.Value["usrName"]))))
				{
					if(XVar.Pack(this.fastFileExists((XVar)(f.Value["thumbnail"]))))
					{
						thumbnailMode = new XVar(true);
					}
				}
				if(XVar.Pack(thumbnailMode))
				{
					imageUrl = XVar.Clone(this.getFileUrl((XVar)(f.Value), (XVar)(keylink), new XVar(true)));
				}
				else
				{
					if(XVar.Pack(pSet.showIcon((XVar)(this.field))))
					{
						imageUrl = XVar.Clone(MVCFunctions.Concat("images/icons/", ViewFileDownloadField.getFileIconByType((XVar)(f.Value["usrName"]), (XVar)(f.Value["type"]))));
					}
				}
				urlData = XVar.Clone(new XVar("url", url, "imageUrl", imageUrl, "filename", f.Value["usrName"], "thumbnailMode", thumbnailMode));
				if((XVar)(this.displayPDF())  && (XVar)(MVCFunctions.strtolower((XVar)(CommonFunctions.getFileExtension((XVar)(f.Value["usrName"])))) == "pdf"))
				{
					urlData.InitAndSetArrayItem(true, "newTab");
				}
				if(XVar.Pack(pSet.showCustomExpr((XVar)(this.field))))
				{
					urlData.InitAndSetArrayItem(pSet.getFileText((XVar)(this.field), (XVar)(f.Value), (XVar)(data)), "htmlLabel");
				}
				if(XVar.Pack(pSet.showFileSize((XVar)(this.field))))
				{
					if(XVar.Pack(finfo.KeyExists("size")))
					{
						urlData.InitAndSetArrayItem(finfo["size"], "size");
					}
					else
					{
						if(XVar.Pack(f.Value.KeyExists("size")))
						{
							urlData.InitAndSetArrayItem(f.Value["size"], "size");
						}
					}
				}
				fileURLs.InitAndSetArrayItem(urlData, null);
			}
			return fileURLs;
		}
	}
}
