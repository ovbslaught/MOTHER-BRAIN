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
	public partial class eventsBase : XClass
	{
		public dynamic events = XVar.Array();
		public dynamic maps = XVar.Array();
		public eventsBase()
		{
			this.init();
		}
		public virtual XVar init()
		{

			return null;
		}
		public virtual XVar exists(dynamic _param_event, dynamic _param_table = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_event = XVar.Clone(_param_event);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			if(table == XVar.Pack(""))
			{
				return !XVar.Equals(XVar.Pack(this.events.KeyExists(var_event)), XVar.Pack(false));
			}
			else
			{
				return (XVar)(this.events.KeyExists(var_event))  && (XVar)(this.events[var_event].KeyExists(table));
			}

			return null;
		}
		public virtual XVar existsMap(dynamic _param_page)
		{
			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			#endregion

			return !XVar.Equals(XVar.Pack(this.maps.KeyExists(page)), XVar.Pack(false));
		}
	}
	public partial class GlobalEventsBase : eventsBase
	{
		protected static bool skipGlobalEventsBaseCtor = false;
		public virtual XVar snippetExists(dynamic _param_event)
		{
			#region pass-by-value parameters
			dynamic var_event = XVar.Clone(_param_event);
			#endregion

			return this.events["onScreenEvents"].KeyExists(var_event);
		}
		public virtual XVar dashSnippetExists(dynamic _param_event)
		{
			#region pass-by-value parameters
			dynamic var_event = XVar.Clone(_param_event);
			#endregion

			return this.events["dashboardEvents"].KeyExists(var_event);
		}
		public virtual XVar buttonExists(dynamic _param_event)
		{
			#region pass-by-value parameters
			dynamic var_event = XVar.Clone(_param_event);
			#endregion

			return this.events["buttons"].KeyExists(var_event);
		}
		public override XVar exists(dynamic _param_event, dynamic _param_table = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_event = XVar.Clone(_param_event);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			if(XVar.Equals(XVar.Pack(var_event), XVar.Pack("GetTablePermissions")))
			{
				return this.events["tablePermissions"].KeyExists(table);
			}
			if(XVar.Equals(XVar.Pack(var_event), XVar.Pack("IsRecordEditable")))
			{
				return this.events["recordEditable"].KeyExists(table);
			}
			return this.events["pageEvents"].KeyExists(var_event);
		}
		public virtual XVar GetTablePermissions(dynamic _param_permissions, dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic permissions = XVar.Clone(_param_permissions);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic events = null;
			events = XVar.Clone(CommonFunctions.getEventObject((XVar)(new ProjectSettings((XVar)(table)))));
			return events.GetTablePermissions((XVar)(permissions));
		}
		public virtual XVar IsRecordEditable(dynamic _param_values, dynamic _param_isEditable, dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			dynamic isEditable = XVar.Clone(_param_isEditable);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic events = null;
			events = XVar.Clone(CommonFunctions.getEventObject((XVar)(new ProjectSettings((XVar)(table)))));
			return events.IsRecordEditable((XVar)(values), (XVar)(isEditable));
		}
		public virtual XVar prepareButtonContext(dynamic var_params)
		{
			dynamic button = null, contextParams = XVar.Array(), masterData = null;
			var_params.InitAndSetArrayItem(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("keys")))), "keys");
			var_params.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("isManyKeys")), "isManyKeys");
			if(XVar.Pack(!(XVar)(var_params["location"])))
			{
				var_params.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("location")), "location");
			}
			button = XVar.Clone(new Button((XVar)(var_params)));
			masterData = new XVar(false);
			if((XVar)(var_params.KeyExists("masterData"))  && (XVar)(0 < MVCFunctions.count(var_params["masterData"])))
			{
				masterData = XVar.Clone(var_params["masterData"]);
			}
			else
			{
				if(XVar.Pack(var_params.KeyExists("masterTable")))
				{
					masterData = XVar.Clone(button.getMasterData((XVar)(var_params["masterTable"])));
				}
			}
			contextParams = XVar.Clone(XVar.Array());
			if(var_params["location"] == Constants.PAGE_VIEW)
			{
				contextParams.InitAndSetArrayItem(button.getRecordData(), "data");
				contextParams.InitAndSetArrayItem(masterData, "masterData");
			}
			else
			{
				if(var_params["location"] == Constants.PAGE_EDIT)
				{
					contextParams.InitAndSetArrayItem(button.getRecordData(), "data");
					contextParams.InitAndSetArrayItem(var_params["fieldsData"], "newData");
					contextParams.InitAndSetArrayItem(masterData, "masterData");
				}
				else
				{
					if(var_params["location"] == "grid")
					{
						var_params.InitAndSetArrayItem("list", "location");
						contextParams.InitAndSetArrayItem(button.getRecordData(), "data");
						contextParams.InitAndSetArrayItem(var_params["fieldsData"], "newData");
						contextParams.InitAndSetArrayItem(masterData, "masterData");
					}
					else
					{
						contextParams.InitAndSetArrayItem(masterData, "masterData");
					}
				}
			}
			RunnerContext.push((XVar)(new RunnerContextItem((XVar)(var_params["location"]), (XVar)(contextParams))));
			return button;
		}
	}
	public partial class TableEventsBase : eventsBase
	{
		public dynamic fieldValues = XVar.Array();
		protected dynamic settings = XVar.Array();
		protected static bool skipTableEventsBaseCtor = false;
		public virtual XVar hasDefaultValue(dynamic _param_field, dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			return this.fieldValues["defaultValue"][field].KeyExists(pageType);
		}
		public virtual XVar hasAutoUpdateValue(dynamic _param_field, dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			return this.fieldValues["autoUpdateValue"][field].KeyExists(pageType);
		}
		public virtual XVar hasLookupWhere(dynamic _param_field, dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			return this.fieldValues["lookupWhere"][field].KeyExists(pageType);
		}
		public virtual XVar hasUploadFolder(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.fieldValues["uploadFolder"].KeyExists(field);
		}
		public virtual XVar defaultValue(dynamic _param_field, dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic method = null;
			method = XVar.Clone(MVCFunctions.Concat("default_", MVCFunctions.GoodFieldName((XVar)(field)), "_ef", pageType));
			return this.Invoke(method);
		}
		public virtual XVar autoUpdateValue(dynamic _param_field, dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic method = null;
			method = XVar.Clone(MVCFunctions.Concat("autoupdate_", MVCFunctions.GoodFieldName((XVar)(field)), "_ef", pageType));
			return this.Invoke(method);
		}
		public virtual XVar filterIntervalValue(dynamic _param_field, dynamic _param_idx, dynamic _param_lower)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic idx = XVar.Clone(_param_idx);
			dynamic lower = XVar.Clone(_param_lower);
			#endregion

			dynamic method = null;
			method = XVar.Clone(MVCFunctions.Concat("filter_", MVCFunctions.GoodFieldName((XVar)(field)), "_idx", idx, "_", (XVar.Pack(lower) ? XVar.Pack("lower") : XVar.Pack("upper"))));
			return this.Invoke(method);
		}
		public virtual XVar customExpression(dynamic _param_field, dynamic _param_pageType, dynamic _param_value, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic value = XVar.Clone(_param_value);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic method = null;
			method = XVar.Clone(MVCFunctions.Concat("custom_", MVCFunctions.GoodFieldName((XVar)(field)), "_vf", pageType));
			return this.Invoke(method, (XVar)(value), (XVar)(data));
		}
		public virtual XVar lookupWhere(dynamic _param_field, dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic method = null;
			method = XVar.Clone(MVCFunctions.Concat("lookupwhere_", MVCFunctions.GoodFieldName((XVar)(field)), "_ef", pageType));
			return this.Invoke(method);
		}
		public virtual XVar mapMarker(dynamic _param_field, dynamic _param_pageType, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic method = null;
			method = XVar.Clone(MVCFunctions.Concat("marker_", MVCFunctions.GoodFieldName((XVar)(field)), "_vf", pageType));
			return this.Invoke(method, (XVar)(data));
		}
		public virtual XVar fileText(dynamic _param_field, dynamic _param_pageType, dynamic _param_file, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic file = XVar.Clone(_param_file);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic method = null;
			method = XVar.Clone(MVCFunctions.Concat("filetext_", MVCFunctions.GoodFieldName((XVar)(field)), "_vf", pageType));
			return this.Invoke(method, (XVar)(file), (XVar)(data));
		}
		public virtual XVar uploadFolder(dynamic _param_field, dynamic _param_file)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic file = XVar.Clone(_param_file);
			#endregion

			dynamic method = null;
			method = XVar.Clone(MVCFunctions.Concat("folder_", MVCFunctions.GoodFieldName((XVar)(field))));
			return this.Invoke(method, (XVar)(file));
		}
	}
}
