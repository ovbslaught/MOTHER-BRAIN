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
	public partial class ViewCurrencyField : ViewControl
	{
		protected static bool skipViewCurrencyFieldCtor = false;
		public ViewCurrencyField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject) // proxy constructor
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
			result = XVar.Clone(CommonFunctions.str_format_currency((XVar)(data[this.field])));
			if((XVar)(!(XVar)(this.container.forExport))  || (XVar)((XVar)(this.container.forExport != "excel")  && (XVar)(this.container.forExport != "csv")))
			{
				result = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(result)));
			}
			if(XVar.Pack(this.searchHighlight))
			{
				result = XVar.Clone(this.highlightSearchWord((XVar)(result), new XVar(false), (XVar)(data[this.field])));
			}
			return result;
		}
		public override XVar highlightSearchWord(dynamic _param_value, dynamic _param_encoded, dynamic _param_dbValue = null)
		{
			#region default values
			if(_param_dbValue as Object == null) _param_dbValue = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic encoded = XVar.Clone(_param_encoded);
			dynamic dbValue = XVar.Clone(_param_dbValue);
			#endregion

			return this.highlightSearchWordForNumber((XVar)(value), (XVar)(encoded), (XVar)(dbValue));
		}
		public override XVar getTextValue(dynamic data)
		{
			return CommonFunctions.str_format_currency((XVar)(data[this.field]));
		}
		public override XVar getValueHighlighted(dynamic _param_value, dynamic _param_highlightData)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic highlightData = XVar.Clone(_param_highlightData);
			#endregion

			dynamic pattern = null, searchOpt = null, searchWordArr = XVar.Array();
			searchWordArr = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> searchWord in highlightData["searchWords"].GetEnumerator())
			{
				dynamic word = null, wordArr = null;
				word = XVar.Clone(MVCFunctions.preg_replace(new XVar("/[\\.,]/"), new XVar(""), (XVar)(searchWord.Value)));
				wordArr = XVar.Clone(MVCFunctions.str_split((XVar)(word)));
				word = XVar.Clone(MVCFunctions.implode(new XVar("[^\\d]{0,2}"), (XVar)(wordArr)));
				word = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(word)));
				if(searchOpt == "Starts with")
				{
					word = XVar.Clone(MVCFunctions.Concat("^", word));
				}
				searchWordArr.InitAndSetArrayItem(word, null);
			}
			pattern = XVar.Clone(MVCFunctions.Concat("/(", MVCFunctions.implode(new XVar("|"), (XVar)(searchWordArr)), ")/"));
			searchOpt = XVar.Clone(highlightData["searchOpt"]);
			switch(((XVar)searchOpt).ToString())
			{
				case "Equals":
					return this.addHighlightingSpan((XVar)(value));
				case "Starts with":
					return MVCFunctions.preg_replace((XVar)(pattern), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(value));
				case "Contains":
					return MVCFunctions.preg_replace((XVar)(pattern), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(value));
				default:
					return value;
			}

			return null;
		}
	}
}
