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
	public partial class ViewImageDownloadField : ViewFileField
	{
		protected dynamic isImageURL = XVar.Pack(false);
		protected dynamic showThumbnails = XVar.Pack(false);
		protected dynamic useAbsolutePath = XVar.Pack(false);
		protected dynamic imageWidth;
		protected dynamic imageHeight;
		protected dynamic thumbWidth;
		protected dynamic thumbHeight;
		protected static bool skipViewImageDownloadFieldCtor = false;
		public ViewImageDownloadField(dynamic _param_field, dynamic _param_container, dynamic _param_pageobject)
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageobject)
		{
			if(skipViewImageDownloadFieldCtor)
			{
				skipViewImageDownloadFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic container = XVar.Clone(_param_container);
			dynamic pageobject = XVar.Clone(_param_pageobject);
			#endregion

			dynamic pageObject = null;
			this.showThumbnails = XVar.Clone((XVar)(container.pSet.showThumbnail((XVar)(this.field)))  && (XVar)(!(XVar)(this.isUrl())));
			this.useAbsolutePath = XVar.Clone(container.pSet.isAbsolute((XVar)(this.field)));
			this.imageWidth = XVar.Clone(container.pSet.getImageWidth((XVar)(this.field)));
			this.imageHeight = XVar.Clone(container.pSet.getImageHeight((XVar)(this.field)));
			if(XVar.Pack(this.showThumbnails))
			{
				this.thumbWidth = XVar.Clone(container.pSet.getThumbnailWidth((XVar)(this.field)));
				this.thumbHeight = XVar.Clone(container.pSet.getThumbnailHeight((XVar)(this.field)));
			}
		}
		public override XVar addJSFiles()
		{
			ProjectSettings pSet;
			this.getJSControl();
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			if(XVar.Pack(pSet.isGalleryEnabled((XVar)(this.field))))
			{
				this.AddJSFile(new XVar("include/photoswipe/photoswipe.min.js"));
				this.AddJSFile(new XVar("include/photoswipe/photoswipe-ui-default.min.js"));
			}

			return null;
		}
		public override XVar addCSSFiles()
		{
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			if(XVar.Pack(pSet.isGalleryEnabled((XVar)(this.field))))
			{
				this.AddCSSFile(new XVar("include/photoswipe/photoswipe.css"));
				this.AddCSSFile(new XVar("include/photoswipe/default-skin/default-skin.css"));
			}

			return null;
		}
		public override XVar getPdfValue(dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic filesArray = XVar.Array(), fs = null, imgCount = null, maxImages = null, resultValues = XVar.Array();
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(data[this.field])))
			{
				return "\"\"";
			}
			if(XVar.Pack(this.isUrl()))
			{
				return MVCFunctions.Concat("{\r\n\t\t\t\timage: \"", data[this.field], "\",\r\n\t\t\t\twidth: ", this.imageWidth, ",\r\n\t\t\t\theight: ", this.imageHeight, "\r\n\t\t\t}");
			}
			resultValues = XVar.Clone(XVar.Array());
			filesArray = XVar.Clone(this.getFilesData((XVar)(data[this.field])));
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			maxImages = XVar.Clone(pSet.getMaxImages((XVar)(this.field)));
			imgCount = new XVar(0);
			fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(pSet), (XVar)(this.field)));
			foreach (KeyValuePair<XVar, dynamic> imageFile in filesArray.GetEnumerator())
			{
				dynamic _image = null, _thumbPath = null, imagePath = null, thumbPath = null;
				if((XVar)(XVar.Pack(0) < maxImages)  && (XVar)(maxImages < imgCount++))
				{
					break;
				}
				if(XVar.Pack(!(XVar)(CommonFunctions.CheckImageExtension((XVar)(imageFile.Value["name"])))))
				{
					resultValues.InitAndSetArrayItem("\"\"", null);
					continue;
				}
				imagePath = XVar.Clone(this.getImagePath((XVar)(imageFile.Value["name"])));
				if(XVar.Pack(!(XVar)(MVCFunctions.myfile_exists((XVar)(imagePath)))))
				{
					continue;
				}
				if(XVar.Pack(!(XVar)(this.showThumbnails)))
				{
					_image = XVar.Clone(this.getImagePdfString((XVar)(imagePath), (XVar)(data), (XVar)(keylink)));
					if(XVar.Pack(_image))
					{
						resultValues.InitAndSetArrayItem(MVCFunctions.Concat("{\r\n\t\t\t\t\t\timage: \"", CommonFunctions.jsreplace((XVar)(_image)), "\",\r\n\t\t\t\t\t\twidth: ", this.imageWidth, ",\r\n\t\t\t\t\t\theight: ", this.imageHeight, "\r\n\t\t\t\t\t}"), null);
					}
					continue;
				}
				thumbPath = XVar.Clone(this.getImagePath((XVar)(imageFile.Value["thumbnail"])));
				_thumbPath = XVar.Clone((XVar.Pack(MVCFunctions.myfile_exists((XVar)(thumbPath))) ? XVar.Pack(thumbPath) : XVar.Pack(imagePath)));
				_image = XVar.Clone(this.getImagePdfString((XVar)(_thumbPath), (XVar)(data), (XVar)(keylink)));
				if(XVar.Pack(_image))
				{
					resultValues.InitAndSetArrayItem(MVCFunctions.Concat("{\r\n\t\t\t\t\timage: \"", CommonFunctions.jsreplace((XVar)(_image)), "\",\r\n\t\t\t\t\twidth: ", this.thumbWidth, ",\r\n\t\t\t\t\theight: ", this.thumbHeight, "\r\n\t\t\t\t}"), null);
				}
			}
			if(0 < MVCFunctions.count(resultValues))
			{
				return MVCFunctions.Concat("[", MVCFunctions.implode(new XVar(","), (XVar)(resultValues)), "]");
			}
			return "\"\"";
		}
		protected virtual XVar getImagePdfString(dynamic _param_imgPath, dynamic data, dynamic _param_keylink)
		{
			#region pass-by-value parameters
			dynamic imgPath = XVar.Clone(_param_imgPath);
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic content = null, imageType = null, urls = XVar.Array();
			content = XVar.Clone(MVCFunctions.myfile_get_contents_binary((XVar)(imgPath)));
			imageType = XVar.Clone(MVCFunctions.SupposeImageType((XVar)(content)));
			if((XVar)(imageType == "image/jpeg")  || (XVar)(imageType == "image/png"))
			{
				return MVCFunctions.Concat("data:", imageType, ";base64,", MVCFunctions.base64_bin2str((XVar)(content)));
			}
			urls = XVar.Clone(this.getFileURLs((XVar)(data), (XVar)(keylink)));
			if(XVar.Pack(urls))
			{
				return urls[0]["image"];
			}
			return "";
		}
		protected virtual XVar getFileURLs(dynamic data, dynamic _param_keylink)
		{
			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic fileURLs = XVar.Array(), showThumbnails = null;
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			showThumbnails = XVar.Clone(pSet.showThumbnail((XVar)(this.field)));
			fileURLs = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.isUrl()))
			{
				dynamic path = XVar.Array();
				path = XVar.Clone(MVCFunctions.pathinfo((XVar)(data[this.field])));
				fileURLs = XVar.Clone(new XVar(0, new XVar("image", data[this.field], "filename", path["filename"])));
			}
			else
			{
				dynamic files = XVar.Array(), fs = null;
				fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(pSet), (XVar)(this.field)));
				files = XVar.Clone(this.getFilesData((XVar)(data[this.field])));
				foreach (KeyValuePair<XVar, dynamic> f in files.GetEnumerator())
				{
					dynamic url = XVar.Array();
					if(XVar.Pack(!(XVar)(this.fastFileExists((XVar)(f.Value["name"]), (XVar)(fs)))))
					{
						continue;
					}
					url = XVar.Clone(new XVar("image", this.getFileUrl((XVar)(f.Value), (XVar)(keylink)), "filename", f.Value["usrName"]));
					if((XVar)(showThumbnails)  && (XVar)((XVar)(f.Value["thumbnail"])  || (XVar)(fs.autoThumbnails())))
					{
						if((XVar)(fs.autoThumbnails())  || (XVar)(this.fastFileExists((XVar)(f.Value["thumbnail"]), (XVar)(fs))))
						{
							url.InitAndSetArrayItem(MVCFunctions.Concat(url["image"], "&thumbnail=1"), "thumbnail");
						}
					}
					fileURLs.InitAndSetArrayItem(url, null);
				}
			}
			return fileURLs;
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

			dynamic attrs = XVar.Array(), fileURLs = null, htmlAttrs = XVar.Array();
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(data[this.field])))
			{
				return "";
			}
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			fileURLs = XVar.Clone(this.getFileURLs((XVar)(data), (XVar)(keylink)));
			attrs = XVar.Clone(XVar.Array());
			attrs.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(fileURLs)), "images");
			attrs.InitAndSetArrayItem(pSet.showThumbnail((XVar)(this.field)), "thumbnails");
			attrs.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(fileURLs)), "images");
			attrs.InitAndSetArrayItem(pSet.getMultipleImgMode((XVar)(this.field)), "multiple");
			attrs.InitAndSetArrayItem(pSet.getMaxImages((XVar)(this.field)), "max-images");
			attrs.InitAndSetArrayItem(pSet.isGalleryEnabled((XVar)(this.field)), "gallery");
			attrs.InitAndSetArrayItem(pSet.getGalleryMode(), "gallery-mode");
			attrs.InitAndSetArrayItem(pSet.getCaptionMode((XVar)(this.field)), "caption-mode");
			if(attrs["caption-mode"] == 3)
			{
				attrs.InitAndSetArrayItem(data[MVCFunctions.Concat("", pSet.getCaptionField((XVar)(this.field)))], "caption");
			}
			attrs.InitAndSetArrayItem(pSet.getImageWidth((XVar)(this.field)), "width");
			attrs.InitAndSetArrayItem(pSet.getImageHeight((XVar)(this.field)), "height");
			if(XVar.Pack(attrs["thumbnails"]))
			{
				attrs.InitAndSetArrayItem(pSet.getThumbnailWidth((XVar)(this.field)), "th-width");
				attrs.InitAndSetArrayItem(pSet.getThumbnailHeight((XVar)(this.field)), "th-height");
			}
			if(XVar.Pack(pSet.getImageBorder((XVar)(this.field))))
			{
				attrs.InitAndSetArrayItem("true", "border");
			}
			if(XVar.Pack(pSet.getImageFullWidth((XVar)(this.field))))
			{
				attrs.InitAndSetArrayItem("true", "fullwidth");
			}
			htmlAttrs = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in attrs.GetEnumerator())
			{
				htmlAttrs.InitAndSetArrayItem(MVCFunctions.Concat("data-", value.Key, "=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value.Value)), "\""), null);
			}
			return MVCFunctions.Concat("<div class=\"r-images\" ", MVCFunctions.join(new XVar(" "), (XVar)(htmlAttrs)), "></div>");
		}
		public override XVar getTextValue(dynamic data)
		{
			dynamic fileNames = XVar.Array(), filesData = XVar.Array();
			filesData = XVar.Clone(this.getFilesData((XVar)(data[this.field])));
			fileNames = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in filesData.GetEnumerator())
			{
				fileNames.InitAndSetArrayItem(f.Value["usrName"], null);
			}
			return MVCFunctions.implode(new XVar(", "), (XVar)(fileNames));
		}
		protected virtual XVar getImagePath(dynamic _param_imageFile)
		{
			#region pass-by-value parameters
			dynamic imageFile = XVar.Clone(_param_imageFile);
			#endregion

			if((XVar)(this.useAbsolutePath)  || (XVar)(MVCFunctions.isAbsolutePath((XVar)(imageFile))))
			{
				return imageFile;
			}
			return MVCFunctions.getabspath((XVar)(imageFile));
		}
		public override XVar neededLoadJSFiles()
		{
			return true;
		}
		protected override XVar isUrl()
		{
			return this.pSettings().isImageURL((XVar)(this.field));
		}
	}
}
