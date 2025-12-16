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
	public partial class ViewNumberField : ViewControl
	{
		protected static bool skipViewNumberFieldCtor = false;
		public ViewNumberField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject) // proxy constructor
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
			return CommonFunctions.str_format_number((XVar)(data[this.field]), (XVar)(this.container.pSet.isDecimalDigits((XVar)(this.field))));
		}
		protected override XVar formatSearchWord(dynamic _param_searchWord)
		{
			#region pass-by-value parameters
			dynamic searchWord = XVar.Clone(_param_searchWord);
			#endregion

			dynamic decimalPlaces = null;
			decimalPlaces = XVar.Clone(this.container.pSet.isDecimalDigits((XVar)(this.field)));
			return CommonFunctions.str_format_number((XVar)(searchWord), (XVar)(decimalPlaces));
		}
		public override XVar getValueHighlighted(dynamic _param_value, dynamic _param_highlightData)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic highlightData = XVar.Clone(_param_highlightData);
			#endregion

			return this.getNumberValueHighlighted((XVar)(value), (XVar)(highlightData));
		}
	}
}
