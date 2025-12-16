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
	public partial class ViewVideoFileField : ViewFileField
	{
		protected static bool skipViewVideoFileFieldCtor = false;
		public ViewVideoFileField(dynamic field, dynamic container, dynamic pageobject) // proxy constructor
			:base((XVar)field, (XVar)container, (XVar)pageobject) {}

		public override XVar addJSFiles()
		{
			this.AddJSFile(new XVar("include/videojs/video.min.js"));
			this.getJSControl();

			return null;
		}
		public override XVar addCSSFiles()
		{
			this.AddCSSFile(new XVar("include/videojs/video-js.min.css"));
			this.AddCSSFile(new XVar("include/videojs/rnr-videojs-theme.css"));

			return null;
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

			dynamic pageType = null, urls = XVar.Array(), values = XVar.Array();
			pageType = XVar.Clone(this.container.pageType);
			if((XVar)((XVar)((XVar)(!(XVar)(html))  || (XVar)(pageType == Constants.PAGE_EXPORT))  || (XVar)(pageType == Constants.PAGE_PRINT))  || (XVar)(this.container.forExport != ""))
			{
				dynamic ret = null;
				ret = XVar.Clone(this.getTextValue((XVar)(data)));
				return (XVar.Pack(html) ? XVar.Pack(MVCFunctions.runner_htmlspecialchars((XVar)(ret))) : XVar.Pack(ret));
			}
			urls = XVar.Clone(this.getFileURLs((XVar)(data), (XVar)(keylink)));
			values = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> u in urls.GetEnumerator())
			{
				values.InitAndSetArrayItem(this.makeVideoControl((XVar)(u.Value)), null);
			}
			return MVCFunctions.implode(new XVar(""), (XVar)(values));
		}
		protected virtual XVar makeVideoControl(dynamic _param_urlData)
		{
			#region pass-by-value parameters
			dynamic urlData = XVar.Clone(_param_urlData);
			#endregion

			dynamic sources = null, srcURL = null, types = XVar.Array(), vHeight = null, vWidth = null;
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			vWidth = XVar.Clone(pSet.getVideoWidth((XVar)(this.field)));
			vHeight = XVar.Clone(pSet.getVideoHeight((XVar)(this.field)));
			vWidth = XVar.Clone((XVar.Pack(vWidth) ? XVar.Pack(vWidth) : XVar.Pack(300)));
			vHeight = XVar.Clone((XVar.Pack(vHeight) ? XVar.Pack(vHeight) : XVar.Pack(200)));
			srcURL = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(urlData["url"])));
			types = XVar.Clone(new XVar(0, urlData["type"], 1, "video/mp4", 2, "video/webm", 3, "video/ogg"));
			types = XVar.Clone(MVCFunctions.array_unique((XVar)(types)));
			sources = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> var_type in types.GetEnumerator())
			{
				sources = MVCFunctions.Concat(sources, "<source type=\"", MVCFunctions.runner_htmlspecialchars((XVar)(var_type.Value)), "\" src=\"", srcURL, "\" />");
			}
			return MVCFunctions.Concat("<div style=\"width:", vWidth, "px; height:", vHeight, "px;\">", "<video class=\"video-js rnr-videojs-theme\" width=\"", vWidth, "\" height=\"", vHeight, "\">", sources, "<p class=\"vjs-no-js\">", "To view this video please enable JavaScript, and consider upgrading to a web browser that supports HTML5 video.", "</p>", "</video>", "</div>");
		}
		public override XVar getPdfValue(dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			return "''";
		}
		protected virtual XVar getFileURLs(dynamic data, dynamic _param_keylink)
		{
			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic fieldIsUrl = null, fileData = XVar.Array(), ret = XVar.Array();
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			fileData = XVar.Clone(this.getFilesData((XVar)(data[this.field])));
			fieldIsUrl = XVar.Clone(pSet.isVideoUrlField((XVar)(this.field)));
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> file in fileData.GetEnumerator())
			{
				dynamic ext = null, url = null, var_type = null;
				if(XVar.Pack(!(XVar)(file.Value["name"])))
				{
					continue;
				}
				if(XVar.Pack(!(XVar)(fieldIsUrl)))
				{
					if(XVar.Pack(!(XVar)(this.fastFileExists((XVar)(file.Value["name"])))))
					{
						continue;
					}
				}
				if(XVar.Pack(fieldIsUrl))
				{
					url = XVar.Clone(file.Value["name"]);
					ext = XVar.Clone(CommonFunctions.getFileExtension((XVar)(url)));
				}
				else
				{
					dynamic addParams = XVar.Array();
					addParams = XVar.Clone(XVar.Array());
					if(XVar.Pack(!(XVar)(pSet.isRewindEnabled((XVar)(this.field)))))
					{
						addParams.InitAndSetArrayItem("1", "norange");
					}
					url = XVar.Clone(MVCFunctions.Concat(CommonFunctions.projectURL(), this.getFileUrl((XVar)(file.Value), (XVar)(keylink), new XVar(false), (XVar)(addParams))));
					ext = XVar.Clone(CommonFunctions.getFileExtension((XVar)(file.Value["usrName"])));
				}
				var_type = XVar.Clone(file.Value["type"]);
				if(XVar.Pack(!(XVar)(var_type)))
				{
					var_type = XVar.Clone(CommonFunctions.mimeTypeByExt((XVar)(ext)));
				}
				if(var_type == "application/octet-stream")
				{
					var_type = new XVar("video/flv");
				}
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(var_type), new XVar("video"))), XVar.Pack(0)))
				{
					continue;
				}
				ret.InitAndSetArrayItem(new XVar("url", url, "type", var_type), null);
			}
			return ret;
		}
		protected override XVar isUrl()
		{
			return this.pSettings().isVideoUrlField((XVar)(this.field));
		}
	}
}
