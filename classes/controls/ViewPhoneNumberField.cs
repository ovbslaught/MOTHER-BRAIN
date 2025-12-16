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
	public partial class ViewPhoneNumberField : ViewControl
	{
		protected static bool skipViewPhoneNumberFieldCtor = false;
		public ViewPhoneNumberField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject) // proxy constructor
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

			dynamic result = null;
			result = XVar.Clone(this.getTextValue((XVar)(data)));
			if((XVar)(!(XVar)(this.container.forExport))  || (XVar)((XVar)(this.container.forExport != "excel")  && (XVar)(this.container.forExport != "csv")))
			{
				result = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(result)));
			}
			if(XVar.Pack(this.searchHighlight))
			{
				result = XVar.Clone(this.highlightSearchWord((XVar)(result), new XVar(true), (XVar)(data[this.field])));
			}
			return result;
		}
		public override XVar getTextValue(dynamic data)
		{
			dynamic result = null;
			result = XVar.Clone(data[this.field]);
			if(MVCFunctions.strlen((XVar)(result)) == 7)
			{
				return MVCFunctions.Concat(MVCFunctions.substr((XVar)(result), new XVar(0), new XVar(3)), "-", MVCFunctions.substr((XVar)(result), new XVar(3)));
			}
			if(MVCFunctions.strlen((XVar)(result)) == 10)
			{
				return MVCFunctions.Concat("(", MVCFunctions.substr((XVar)(result), new XVar(0), new XVar(3)), ") ", MVCFunctions.substr((XVar)(result), new XVar(3), new XVar(3)), "-", MVCFunctions.substr((XVar)(result), new XVar(6)));
			}
			return result;
		}
		public override XVar getValueHighlighted(dynamic _param_value, dynamic _param_highlightData)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic highlightData = XVar.Clone(_param_highlightData);
			#endregion

			dynamic searchOpt = null, searchWord = null, searchWordArr = XVar.Array();
			searchWordArr = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> _searchWord in highlightData["searchWords"].GetEnumerator())
			{
				dynamic word = null, wordArr = null;
				word = XVar.Clone(_searchWord.Value);
				wordArr = XVar.Clone(MVCFunctions.str_split((XVar)(word)));
				word = XVar.Clone(MVCFunctions.implode(new XVar("([\\-\\(]|\\) )?"), (XVar)(wordArr)));
				searchWordArr.InitAndSetArrayItem(MVCFunctions.Concat("[(]?", MVCFunctions.runner_htmlspecialchars((XVar)(word))), null);
			}
			searchWord = XVar.Clone(MVCFunctions.implode(new XVar("|"), (XVar)(searchWordArr)));
			searchOpt = XVar.Clone(highlightData["searchOpt"]);
			switch(((XVar)searchOpt).ToString())
			{
				case "Equals":
					return this.addHighlightingSpan((XVar)(value));
				case "Starts with":
					return MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/^(", searchWord, ")/")), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(value));
				case "Contains":
					return MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/(", searchWord, ")/")), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(value));
				default:
					return value;
			}

			return null;
		}
	}
}
