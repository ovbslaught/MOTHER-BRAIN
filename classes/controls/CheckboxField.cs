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
	public partial class CheckboxField : EditControl
	{
		protected static bool skipCheckboxFieldCtor = false;
		public CheckboxField(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection)
		{
			if(skipCheckboxFieldCtor)
			{
				skipCheckboxFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.format = new XVar(Constants.EDIT_FORMAT_CHECKBOX);
		}
		public static XVar checkedValue(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return (XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack("t")))  || (XVar)((XVar)(value != XVar.Pack(""))  && (XVar)(value != XVar.Pack(0)));
		}
		public override XVar buildControl(dynamic _param_value, dynamic _param_mode, dynamic _param_fieldNum, dynamic _param_validate, dynamic _param_additionalCtrlParams, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic validate = XVar.Clone(_param_validate);
			dynamic additionalCtrlParams = XVar.Clone(_param_additionalCtrlParams);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			base.buildControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			if((XVar)((XVar)((XVar)(mode == Constants.MODE_ADD)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  || (XVar)(mode == Constants.MODE_EDIT))  || (XVar)(mode == Constants.MODE_INLINE_EDIT))
			{
				dynamic reservedBoolean = null, var_checked = null;
				var_checked = new XVar("");
				if((XVar)((XVar)(this.connection.dbType == Constants.nDATABASE_PostgreSQL)  && (XVar)((XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack("t")))  || (XVar)((XVar)((XVar)(value != "f")  && (XVar)(value != XVar.Pack("")))  && (XVar)(value != XVar.Pack(0)))))  || (XVar)((XVar)(this.connection.dbType != Constants.nDATABASE_PostgreSQL)  && (XVar)((XVar)(value != XVar.Pack(""))  && (XVar)(value != XVar.Pack(0)))))
				{
					var_checked = new XVar(" checked");
				}
				if(this.connection.dbType == Constants.nDATABASE_PostgreSQL)
				{
					reservedBoolean = new XVar("data-true=\"t\" data-false=\"f\"");
				}
				MVCFunctions.Echo((XVar.Pack(additionalCtrlParams["inLabel"]) ? XVar.Pack("<span class=\"r-checkbox-inlabel r-checkbox-control\">") : XVar.Pack("<span class=\"checkbox r-checkbox-control\"><label>")));
				MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.ctype, "\" type=\"hidden\" name=\"", this.ctype, "\" value=\"checkbox\">"));
				MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "\" type=\"Checkbox\" ", (XVar.Pack((XVar)((XVar)(mode == Constants.MODE_INLINE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  && (XVar)(this.is508 == true)) ? XVar.Pack(MVCFunctions.Concat("alt=\"", this.strLabel, "\" ")) : XVar.Pack("")), "name=\"", this.cfield, "\" ", var_checked, " ", reservedBoolean, ">"));
				MVCFunctions.Echo((XVar.Pack(additionalCtrlParams["inLabel"]) ? XVar.Pack("</span>") : XVar.Pack("</label></span>")));
			}
			else
			{
				dynamic labels = XVar.Array(), options = XVar.Array(), possibleOptions = XVar.Array();
				MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.ctype, "\" type=\"hidden\" name=\"", this.ctype, "\" value=\"checkbox\">"));
				MVCFunctions.Echo(MVCFunctions.Concat("<select id=\"", this.cfield, "\" ", (XVar.Pack((XVar)((XVar)(mode == Constants.MODE_INLINE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  && (XVar)(this.is508 == true)) ? XVar.Pack(MVCFunctions.Concat("alt=\"", this.strLabel, "\" ")) : XVar.Pack("")), "name=\"", this.cfield, "\" class=\"form-control\">"));
				options = XVar.Clone(new XVar(0, "", 1, "on", 2, "off"));
				possibleOptions = XVar.Clone(new XVar("", XVar.Array(), "on", new XVar(0, "on", 1, "1"), "off", new XVar(0, "off", 1, "0")));
				labels = XVar.Clone(new XVar(0, "", 1, CommonFunctions.mlang_message(new XVar("TRUE")), 2, CommonFunctions.mlang_message(new XVar("FALSE"))));
				foreach (KeyValuePair<XVar, dynamic> optValue in options.GetEnumerator())
				{
					dynamic selected = null;
					selected = XVar.Clone((XVar.Pack(MVCFunctions.in_array((XVar)(value), (XVar)(possibleOptions[optValue.Value]))) ? XVar.Pack(" selected") : XVar.Pack("")));
					MVCFunctions.Echo(MVCFunctions.Concat("<option value=\"", optValue.Value, "\"", selected, ">", labels[optValue.Key], "</option>"));
				}
				MVCFunctions.Echo("</select>");
			}
			this.buildControlEnd((XVar)(validate), (XVar)(mode));

			return null;
		}
		public override XVar getFirstElementId()
		{
			return this.cfield;
		}
		public static XVar getFieldCondition(dynamic _param_field, dynamic _param_searchFor)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic searchFor = XVar.Clone(_param_searchFor);
			#endregion

			dynamic offCondition = null;
			if((XVar)(searchFor == "none")  || (XVar)((XVar)(searchFor != "on")  && (XVar)(searchFor != "off")))
			{
				return null;
			}
			offCondition = XVar.Clone(DataCondition._Or((XVar)(new XVar(0, DataCondition.FieldIs((XVar)(field), new XVar(Constants.dsopEQUAL), new XVar("0")), 1, DataCondition.FieldIs((XVar)(field), new XVar(Constants.dsopEMPTY), new XVar(""))))));
			if(searchFor == "off")
			{
				return offCondition;
			}
			return DataCondition._Not((XVar)(offCondition));
		}
		public override XVar getBasicFieldCondition(dynamic _param_searchFor, dynamic _param_strSearchOption, dynamic _param_searchFor2 = null, dynamic _param_etype = null)
		{
			#region default values
			if(_param_searchFor2 as Object == null) _param_searchFor2 = new XVar("");
			if(_param_etype as Object == null) _param_etype = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic strSearchOption = XVar.Clone(_param_strSearchOption);
			dynamic searchFor2 = XVar.Clone(_param_searchFor2);
			dynamic etype = XVar.Clone(_param_etype);
			#endregion

			if(strSearchOption != Constants.EQUALS)
			{
				return null;
			}
			return CheckboxField.getFieldCondition((XVar)(this.field), (XVar)(searchFor));
		}
	}
}
