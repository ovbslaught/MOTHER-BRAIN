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
	public partial class UserControl : EditControl
	{
		protected static bool skipUserControlCtor = false;
		public UserControl(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection) // proxy constructor
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection) {}

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
			this.buildUserControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			this.buildControlEnd((XVar)(validate), (XVar)(mode));

			return null;
		}
		public virtual XVar buildUserControl(dynamic _param_value, dynamic _param_mode, dynamic _param_fieldNum, dynamic _param_validate, dynamic _param_additionalCtrlParams, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic validate = XVar.Clone(_param_validate);
			dynamic additionalCtrlParams = XVar.Clone(_param_additionalCtrlParams);
			dynamic data = XVar.Clone(_param_data);
			#endregion


			return null;
		}
		public virtual XVar initUserControl()
		{

			return null;
		}
		public virtual XVar getUserSearchOptions()
		{
			return XVar.Array();
		}
		public override XVar getSearchOptions(dynamic _param_selOpt, dynamic _param_not, dynamic _param_both)
		{
			#region pass-by-value parameters
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			return this.buildSearchOptions((XVar)(this.getUserSearchOptions()), (XVar)(selOpt), (XVar)(var_not), (XVar)(both));
		}
		public override XVar init()
		{
			dynamic eventsObject = null, field = null, method = null, pageType = null, tName = null;
			ProjectSettings pSet;
			tName = XVar.Clone(this.pageObject.tName);
			field = XVar.Clone(this.field);
			pSet = XVar.UnPackProjectSettings(this.pageObject.pSetEdit);
			if((XVar)(this.pageObject.pageType == Constants.PAGE_SEARCH)  && (XVar)(pSet.getDefaultPageType() == Constants.PAGE_DASHBOARD))
			{
				dynamic dashFields = XVar.Array();
				dashFields = XVar.Clone(pSet.getDashboardSearchFields());
				tName = XVar.Clone(dashFields[field][0]["table"]);
				field = XVar.Clone(dashFields[field][0]["field"]);
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(tName), new XVar(Constants.PAGE_SEARCH)));
			}
			pageType = XVar.Clone(pSet.getEffectiveEditFormat((XVar)(field)));
			method = XVar.Clone(MVCFunctions.Concat("plugin_", MVCFunctions.GoodFieldName((XVar)(field)), "_ef", pageType));
			eventsObject = XVar.Clone(CommonFunctions.getEventObject((XVar)(pSet)));
			if(XVar.Pack(eventsObject.fieldValues["editPluginInit"][field][pageType]))
			{
				dynamic settings = XVar.Array();
				settings = XVar.Clone(eventsObject.Invoke(method, (XVar)(this.pageObject)));
				foreach (KeyValuePair<XVar, dynamic> value in settings.GetEnumerator())
				{
					this.settings.InitAndSetArrayItem(value.Value, value.Key);
				}
			}

			return null;
		}
	}
}
