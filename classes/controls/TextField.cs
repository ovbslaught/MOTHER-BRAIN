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
	public partial class TextField : TextControl
	{
		protected static bool skipTextFieldCtor = false;
		private bool skipTextControlCtorSurrogate = new Func<bool>(() => skipTextControlCtor = true).Invoke();
		public TextField(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection)
		{
			if(skipTextFieldCtor)
			{
				skipTextFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.format = new XVar(Constants.EDIT_FORMAT_TEXT_FIELD);
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

			dynamic altAttr = null, autocompleteAttr = null, classString = null, inputType = null;
			base.buildControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			inputType = XVar.Clone(this.pageObject.pSetEdit.getHTML5InputType((XVar)(this.field)));
			altAttr = XVar.Clone((XVar.Pack((XVar)((XVar)(mode == Constants.MODE_INLINE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  && (XVar)(this.is508 == true)) ? XVar.Pack(MVCFunctions.Concat(" alt=\"", this.strLabel, "\" ")) : XVar.Pack("")));
			classString = new XVar("");
			classString = new XVar(" class=\"form-control\"");
			autocompleteAttr = new XVar("");
			if(mode == Constants.MODE_SEARCH)
			{
				autocompleteAttr = new XVar("autocomplete=\"off\"");
			}
			if((XVar)(this.pageObject.pageType == "register")  && (XVar)(this.field == Security.usernameField()))
			{
				autocompleteAttr = new XVar("autocomplete=\"username\"");
			}
			MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "\" ", classString, this.inputStyle, " type=\"", inputType, "\" ", autocompleteAttr, " ", altAttr, "name=\"", this.cfield, "\" ", this.maxLengthAttr(), this.getPlaceholderAttr(), " value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\">"));
			this.buildControlEnd((XVar)(validate), (XVar)(mode));

			return null;
		}
		public override XVar getFirstElementId()
		{
			return this.cfield;
		}
	}
}
