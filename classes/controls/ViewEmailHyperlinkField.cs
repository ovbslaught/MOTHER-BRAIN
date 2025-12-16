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
	public partial class ViewEmailHyperlinkField : ViewControl
	{
		protected static bool skipViewEmailHyperlinkFieldCtor = false;
		public ViewEmailHyperlinkField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject) // proxy constructor
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageObject) {}

		public override XVar getPdfValue(dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic containsMailTo = null, link = null, result = null, title = null;
			result = XVar.Clone(data[this.field]);
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(result)))))
			{
				return "''";
			}
			containsMailTo = XVar.Clone(MVCFunctions.substr((XVar)(result), new XVar(0), new XVar(7)) == "mailto:");
			title = XVar.Clone((XVar.Pack(containsMailTo) ? XVar.Pack(MVCFunctions.substr((XVar)(result), new XVar(8))) : XVar.Pack(result)));
			link = XVar.Clone((XVar.Pack(containsMailTo) ? XVar.Pack(result) : XVar.Pack(MVCFunctions.Concat("mailto:", result))));
			return MVCFunctions.runner_json_encode((XVar)(new XVar("text", title, "link", link)));
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

			dynamic containsMailTo = null, link = null, result = null, title = null;
			result = XVar.Clone(data[this.field]);
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(result)))))
			{
				return "";
			}
			containsMailTo = XVar.Clone(MVCFunctions.substr((XVar)(result), new XVar(0), new XVar(7)) == "mailto:");
			title = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)((XVar.Pack(containsMailTo) ? XVar.Pack(MVCFunctions.substr((XVar)(result), new XVar(8))) : XVar.Pack(result)))));
			link = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)((XVar.Pack(containsMailTo) ? XVar.Pack(result) : XVar.Pack(MVCFunctions.Concat("mailto:", result))))));
			if(XVar.Pack(this.searchHighlight))
			{
				title = XVar.Clone(this.highlightSearchWord((XVar)(title), new XVar(false), new XVar("")));
			}
			return MVCFunctions.Concat("<a href=\"", link, "\">", title, "</a>");
		}
		public override XVar getTextValue(dynamic data)
		{
			dynamic result = null;
			result = XVar.Clone(data[this.field]);
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(result)))))
			{
				return "";
			}
			if(MVCFunctions.substr((XVar)(result), new XVar(0), new XVar(7)) == "mailto:")
			{
				return MVCFunctions.substr((XVar)(result), new XVar(8));
			}
			return result;
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
	}
}
