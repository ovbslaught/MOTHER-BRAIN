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
	public partial class ViewAudioFileField : ViewFileField
	{
		protected static bool skipViewAudioFileFieldCtor = false;
		public ViewAudioFileField(dynamic field, dynamic container, dynamic pageobject) // proxy constructor
			:base((XVar)field, (XVar)container, (XVar)pageobject) {}

		public override XVar showDBValue(dynamic data, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic controls = XVar.Array(), fileUrls = XVar.Array(), pageType = null;
			pageType = XVar.Clone(this.container.pageType);
			if((XVar)((XVar)((XVar)(!(XVar)(html))  || (XVar)(pageType == Constants.PAGE_EXPORT))  || (XVar)(pageType == Constants.PAGE_PRINT))  || (XVar)(this.container.forExport != ""))
			{
				dynamic ret = null;
				ret = XVar.Clone(this.getTextValue((XVar)(data)));
				return (XVar.Pack(html) ? XVar.Pack(MVCFunctions.runner_htmlspecialchars((XVar)(ret))) : XVar.Pack(ret));
			}
			fileUrls = XVar.Clone(this.getFileURLs((XVar)(data), (XVar)(keylink)));
			controls = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> u in fileUrls.GetEnumerator())
			{
				controls.InitAndSetArrayItem(ViewAudioFileField.makeAudioControl((XVar)(u.Value)), null);
			}
			return MVCFunctions.implode(new XVar(""), (XVar)(controls));
		}
		public static XVar makeAudioControl(dynamic _param_urlData)
		{
			#region pass-by-value parameters
			dynamic urlData = XVar.Clone(_param_urlData);
			#endregion

			dynamic htmlAltTitle = null, htmlTitle = null, htmlUrl = null;
			htmlTitle = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(urlData["title"])));
			htmlAltTitle = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(urlData["altTitle"])));
			htmlUrl = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(urlData["url"])));
			return MVCFunctions.Concat("<figure>", "<figcaption>", htmlTitle, "<figcaption>", "<audio controls preload=\"none\" src=\"", htmlUrl, "\">", "<a title=\"", htmlAltTitle, "\" href=\"", htmlUrl, "\">", htmlAltTitle, "</a>", "</audio></figure>");
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
				dynamic altTitle = null, title = null, titleField = null, url = null;
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
				}
				else
				{
					url = XVar.Clone(MVCFunctions.Concat(CommonFunctions.projectURL(), this.getFileUrl((XVar)(file.Value), (XVar)(keylink), new XVar(false))));
				}
				titleField = XVar.Clone(pSet.getAudioTitleField((XVar)(this.field)));
				if(XVar.Pack(titleField))
				{
					title = XVar.Clone(data[titleField]);
					altTitle = XVar.Clone(title);
				}
				else
				{
					altTitle = XVar.Clone(file.Value["usrName"]);
					title = new XVar("");
				}
				ret.InitAndSetArrayItem(new XVar("url", url, "title", title, "altTitle", altTitle), null);
			}
			return ret;
		}
		protected override XVar isUrl()
		{
			return this.pSettings().isVideoUrlField((XVar)(this.field));
		}
	}
}
