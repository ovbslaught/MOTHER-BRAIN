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
	public partial class ViewHyperlinkField : ViewControl
	{
		protected static bool skipViewHyperlinkFieldCtor = false;
		public ViewHyperlinkField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject) // proxy constructor
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageObject) {}

		public override XVar showDBValue(dynamic data, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic linkData = XVar.Array();
			linkData = XVar.Clone(this.getLinkData((XVar)(data), (XVar)(keylink), (XVar)(html)));
			if(XVar.Pack(!(XVar)(linkData)))
			{
				return "";
			}
			return MVCFunctions.Concat("<a href=\"", MVCFunctions.runner_htmlspecialchars((XVar)(linkData["link"])), "\"", linkData["target"], ">", linkData["title"], "</a>");
		}
		public virtual XVar getLinkData(dynamic data, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic isExactMatched = null, lData = XVar.Array(), link = null, prefix = null, ret = null, target = null, title = null, var_type = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(data[this.field])))))
			{
				return false;
			}
			ret = XVar.Clone(data[this.field]);
			title = XVar.Clone(ret);
			link = XVar.Clone(ret);
			if(MVCFunctions.substr((XVar)(ret), (XVar)(MVCFunctions.strlen((XVar)(ret)) - 1)) == "#")
			{
				dynamic i = null;
				i = XVar.Clone(MVCFunctions.strpos((XVar)(ret), new XVar("#")));
				title = XVar.Clone(MVCFunctions.substr((XVar)(ret), new XVar(0), (XVar)(i)));
				link = XVar.Clone(MVCFunctions.substr((XVar)(ret), (XVar)(i + 1), (XVar)((MVCFunctions.strlen((XVar)(ret)) - i) - 2)));
				if(XVar.Pack(!(XVar)(title)))
				{
					title = XVar.Clone(link);
				}
			}
			prefix = XVar.Clone(this.getContainer().pSet.getLinkPrefix((XVar)(this.field)));
			target = XVar.Clone((XVar.Pack(this.getContainer().pSet.openLinkInNewWindow((XVar)(this.field))) ? XVar.Pack(" target=_blank") : XVar.Pack("")));
			var_type = XVar.Clone(this.getContainer().pSet.getLinkType((XVar)(this.field)));
			isExactMatched = new XVar(false);
			if(XVar.Pack(this.searchHighlight))
			{
				dynamic searchData = XVar.Array();
				searchData = XVar.Clone(this.pageObject.searchClauseObj.getSearchToHighlight((XVar)(this.field)));
				if(XVar.Pack(searchData))
				{
					isExactMatched = new XVar(true);
					foreach (KeyValuePair<XVar, dynamic> searchWord in searchData["searchWords"].GetEnumerator())
					{
						if(searchWord.Value != data[this.field])
						{
							isExactMatched = new XVar(false);
							break;
						}
					}
				}
			}
			if(var_type == 1)
			{
				title = XVar.Clone(CommonFunctions.GetMLString((XVar)(this.getContainer().pSet.getLinkDisplayText((XVar)(this.field)))));
				if(XVar.Pack(isExactMatched))
				{
					title = XVar.Clone(this.addHighlightingSpan((XVar)(title)));
				}
			}
			else
			{
				if(var_type == 2)
				{
					title = XVar.Clone(data[this.getContainer().pSet.getLinkDisplayField((XVar)(this.field))]);
					if(XVar.Pack(isExactMatched))
					{
						title = XVar.Clone(this.addHighlightingSpan((XVar)(title)));
					}
					else
					{
						if((XVar)(this.field == this.getContainer().pSet.getLinkDisplayField((XVar)(this.field)))  && (XVar)(this.searchHighlight))
						{
							title = XVar.Clone(this.highlightSearchWord((XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(title))), new XVar(true), new XVar("")));
						}
					}
				}
				else
				{
					if(XVar.Pack(this.searchHighlight))
					{
						if(XVar.Pack(isExactMatched))
						{
							title = XVar.Clone(this.addHighlightingSpan((XVar)(MVCFunctions.Concat(prefix, title))));
						}
						else
						{
							title = XVar.Clone(MVCFunctions.Concat(prefix, this.highlightSearchWord((XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(title))), new XVar(true), new XVar(""))));
						}
					}
					else
					{
						title = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.Concat(prefix, title))));
					}
				}
			}
			if((XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(link), new XVar("://"))), XVar.Pack(false)))  && (XVar)(MVCFunctions.substr((XVar)(link), new XVar(0), new XVar(7)) != "mailto:"))
			{
				link = XVar.Clone(MVCFunctions.Concat(prefix, link));
			}
			lData = XVar.Clone(XVar.Array());
			lData.InitAndSetArrayItem(link, "link");
			lData.InitAndSetArrayItem(target, "target");
			lData.InitAndSetArrayItem(title, "title");
			return lData;
		}
		public override XVar getTextValue(dynamic data)
		{
			dynamic link = null, prefix = null, ret = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(data[this.field])))))
			{
				return "";
			}
			link = XVar.Clone(data[this.field]);
			if(MVCFunctions.substr((XVar)(ret), (XVar)(MVCFunctions.strlen((XVar)(ret)) - 1)) == "#")
			{
				dynamic i = null;
				i = XVar.Clone(MVCFunctions.strpos((XVar)(ret), new XVar("#")));
				link = XVar.Clone(MVCFunctions.substr((XVar)(ret), (XVar)(i + 1), (XVar)((MVCFunctions.strlen((XVar)(ret)) - i) - 2)));
			}
			prefix = XVar.Clone(this.getContainer().pSet.getLinkPrefix((XVar)(this.field)));
			if((XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(link), new XVar("://"))), XVar.Pack(false)))  && (XVar)(MVCFunctions.substr((XVar)(link), new XVar(0), new XVar(7)) != "mailto:"))
			{
				link = XVar.Clone(MVCFunctions.Concat(prefix, link));
			}
			return link;
		}
		public override XVar getExportValue(dynamic data, dynamic _param_keylink = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			if(_param_html as Object == null) _param_html = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			return MVCFunctions.nl2br((XVar)(data[this.field]));
		}
		public override XVar getPdfValue(dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic html = null, linkData = XVar.Array();
			linkData = XVar.Clone(this.getLinkData((XVar)(data), (XVar)(keylink), (XVar)(html)));
			if(XVar.Pack(!(XVar)(linkData)))
			{
				return "''";
			}
			return MVCFunctions.runner_json_encode((XVar)(new XVar("text", linkData["title"], "link", linkData["link"])));
		}
	}
}
