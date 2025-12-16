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
	public partial class ViewCheckboxField : ViewControl
	{
		protected static bool skipViewCheckboxFieldCtor = false;
		public ViewCheckboxField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject) // proxy constructor
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageObject) {}

		public virtual XVar getTrueCondition(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if(this.getHostPageDbType() == Constants.nDATABASE_PostgreSQL)
			{
				return (XVar)(XVar.Equals(XVar.Pack(data[this.field]), XVar.Pack("t")))  || (XVar)((XVar)((XVar)(!XVar.Equals(XVar.Pack(data[this.field]), XVar.Pack("f")))  && (XVar)(data[this.field] != 0))  && (XVar)(data[this.field] != ""));
			}
			else
			{
				return (XVar)(data[this.field] != 0)  && (XVar)(data[this.field] != "");
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

			dynamic boolVal = null;
			boolVal = XVar.Clone((XVar.Pack(this.getTrueCondition((XVar)(data))) ? XVar.Pack("true") : XVar.Pack("false")));
			return MVCFunctions.Concat("{text: '', checkbox: ", boolVal, "}");

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

			dynamic imgSrc = null, result = null;
			result = new XVar("<img class=\"r-checkbox-image\" src=\"");
			imgSrc = new XVar("images/check_");
			imgSrc = MVCFunctions.Concat(imgSrc, (XVar.Pack(this.getTrueCondition((XVar)(data))) ? XVar.Pack("yes") : XVar.Pack("no")));
			result = MVCFunctions.Concat(result, imgSrc, ".gif\" border=0");
			if(XVar.Pack(CommonFunctions.isEnableSection508()))
			{
				result = MVCFunctions.Concat(result, " alt=\" \"");
			}
			result = MVCFunctions.Concat(result, ">");
			return result;
		}
		protected virtual XVar getHostPageDbType()
		{
			return GlobalVars.cman.byTable((XVar)(this.container.tName)).dbType;
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
