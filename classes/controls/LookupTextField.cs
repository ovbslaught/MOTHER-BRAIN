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
	public partial class LookupTextField : LookupField
	{
		public dynamic localPSet;
		public dynamic ciphererLink = XVar.Pack(null);
		public dynamic ciphererDisplay = XVar.Pack(null);
		protected static bool skipLookupTextFieldCtor = false;
		private bool skipLookupFieldCtorSurrogate = new Func<bool>(() => skipLookupFieldCtor = true).Invoke();
		public LookupTextField(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection)
		{
			if(skipLookupTextFieldCtor)
			{
				skipLookupTextFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.format = new XVar(Constants.EDIT_FORMAT_LOOKUP_WIZARD);
			this.lookupPageType = XVar.Clone(this.pageObject.pSetEdit.getPageTypeByFieldEditFormat((XVar)(this.field), new XVar(Constants.EDIT_FORMAT_LOOKUP_WIZARD)));
			this.localPSet = XVar.Clone(new ProjectSettings((XVar)(this.pageObject.tName), (XVar)(this.lookupPageType)));
			this.lookupDataSource = XVar.Clone(CommonFunctions.getLookupDataSource((XVar)(this.field), (XVar)(this.localPSet)));
			this.lookupTable = XVar.Clone(this.localPSet.getLookupTable((XVar)(this.field)));
			this.lookupType = XVar.Clone(this.localPSet.getLookupType((XVar)(this.field)));
			if(this.lookupType == Constants.LT_QUERY)
			{
				this.lookupPSet = XVar.Clone(new ProjectSettings((XVar)(this.lookupTable)));
			}
			this.displayFieldName = XVar.Clone(this.localPSet.getDisplayField((XVar)(this.field)));
			this.linkFieldName = XVar.Clone(this.localPSet.getLinkField((XVar)(this.field)));
			this.linkAndDisplaySame = XVar.Clone(this.displayFieldName == this.linkFieldName);
			this.ciphererLink = XVar.Clone(new RunnerCipherer((XVar)(this.pageObject.tName)));
			if(this.lookupType == Constants.LT_QUERY)
			{
				this.ciphererDisplay = XVar.Clone(new RunnerCipherer((XVar)(this.lookupTable)));
			}
			else
			{
				this.ciphererDisplay = XVar.Clone(this.ciphererLink);
			}
			this.LCType = XVar.Clone(this.localPSet.lookupControlType((XVar)(this.field)));
			this.multiselect = XVar.Clone(this.localPSet.multiSelect((XVar)(this.field)));
			this.lwLinkField = XVar.Clone(connection.addFieldWrappers((XVar)(this.localPSet.getLinkField((XVar)(this.field)))));
			this.lwDisplayFieldWrapped = XVar.Clone(RunnerPage.sqlFormattedDisplayField((XVar)(this.field), (XVar)(connection), (XVar)(this.localPSet)));
			this.customDisplay = XVar.Clone(this.localPSet.getCustomDisplay((XVar)(this.field)));
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

			base.parentBuildControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "\" ", this.inputStyle, " type=\"text\" ", (XVar.Pack(mode == Constants.MODE_SEARCH) ? XVar.Pack("autocomplete=\"off\" ") : XVar.Pack("")), (XVar.Pack((XVar)((XVar)(mode == Constants.MODE_INLINE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  && (XVar)(this.is508 == true)) ? XVar.Pack(MVCFunctions.Concat("alt=\"", this.strLabel, "\" ")) : XVar.Pack("")), "name=\"", this.cfield, "\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\">"));
			this.buildControlEnd((XVar)(validate), (XVar)(mode));

			return null;
		}
		public override XVar getSearchOptions(dynamic _param_selOpt, dynamic _param_not, dynamic _param_both)
		{
			#region pass-by-value parameters
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			dynamic isPHPEncripted = null, optionsArray = XVar.Array();
			optionsArray = XVar.Clone(XVar.Array());
			isPHPEncripted = XVar.Clone(this.pageObject.cipherer.isFieldPHPEncrypted((XVar)(this.field)));
			if(XVar.Pack(!(XVar)(isPHPEncripted)))
			{
				optionsArray.InitAndSetArrayItem(Constants.CONTAINS, null);
			}
			optionsArray.InitAndSetArrayItem(Constants.EQUALS, null);
			if(XVar.Pack(!(XVar)(isPHPEncripted)))
			{
				optionsArray.InitAndSetArrayItem(Constants.STARTS_WITH, null);
				optionsArray.InitAndSetArrayItem(Constants.MORE_THAN, null);
				optionsArray.InitAndSetArrayItem(Constants.LESS_THAN, null);
				optionsArray.InitAndSetArrayItem(Constants.BETWEEN, null);
			}
			optionsArray.InitAndSetArrayItem(Constants.EMPTY_SEARCH, null);
			if(XVar.Pack(both))
			{
				if(XVar.Pack(!(XVar)(isPHPEncripted)))
				{
					optionsArray.InitAndSetArrayItem(Constants.NOT_CONTAINS, null);
				}
				optionsArray.InitAndSetArrayItem(Constants.NOT_EQUALS, null);
				if(XVar.Pack(!(XVar)(isPHPEncripted)))
				{
					optionsArray.InitAndSetArrayItem(Constants.NOT_STARTS_WITH, null);
					optionsArray.InitAndSetArrayItem(Constants.NOT_MORE_THAN, null);
					optionsArray.InitAndSetArrayItem(Constants.NOT_LESS_THAN, null);
					optionsArray.InitAndSetArrayItem(Constants.NOT_BETWEEN, null);
				}
				optionsArray.InitAndSetArrayItem(Constants.NOT_EMPTY, null);
			}
			return this.buildSearchOptions((XVar)(optionsArray), (XVar)(selOpt), (XVar)(var_not), (XVar)(both));
		}
	}
}
