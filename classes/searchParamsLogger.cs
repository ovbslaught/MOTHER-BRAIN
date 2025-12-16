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
	public partial class searchParamsLogger : paramsLogger
	{
		protected dynamic savedSearchesParams = XVar.Array();
		protected static bool skipsearchParamsLoggerCtor = false;
		public searchParamsLogger(dynamic _param_tableNameId, dynamic _param_type = null)
			:base((XVar)_param_tableNameId, (XVar)Constants.SSEARCH_PARAMS_TYPE)
		{
			if(skipsearchParamsLoggerCtor)
			{
				skipsearchParamsLoggerCtor = false;
				return;
			}
			#region default values
			if(_param_type as Object == null) _param_type = new XVar(Constants.SSEARCH_PARAMS_TYPE);
			#endregion

			#region pass-by-value parameters
			dynamic tableNameId = XVar.Clone(_param_tableNameId);
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			this.var_type = new XVar(Constants.SSEARCH_PARAMS_TYPE);
		}
		public virtual XVar saveSearch(dynamic _param_searchName, dynamic _param_searchParams)
		{
			#region pass-by-value parameters
			dynamic searchName = XVar.Clone(_param_searchName);
			dynamic searchParams = XVar.Clone(_param_searchParams);
			#endregion

			dynamic savedSearchNames = null, values = XVar.Array();
			savedSearchNames = XVar.Clone(this.getSavedSeachesParams());
			if(XVar.Pack(savedSearchNames.KeyExists(searchName)))
			{
				this.updateSearch((XVar)(searchName), (XVar)(searchParams));
				return null;
			}
			values = XVar.Clone(XVar.Array());
			values.InitAndSetArrayItem(searchName, "NAME");
			this.save((XVar)(searchParams), (XVar)(values));

			return null;
		}
		public virtual XVar updateSearch(dynamic _param_searchName, dynamic _param_searchParams)
		{
			#region pass-by-value parameters
			dynamic searchName = XVar.Clone(_param_searchName);
			dynamic searchParams = XVar.Clone(_param_searchParams);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(this.getUpdateCommand((XVar)(searchParams)));
			dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, DataCondition.FieldEquals(new XVar("NAME"), (XVar)(searchName))))));
			this.dataSource.updateSingle((XVar)(dc), new XVar(false));

			return null;
		}
		public virtual XVar deleteSearch(dynamic _param_searchName)
		{
			#region pass-by-value parameters
			dynamic searchName = XVar.Clone(_param_searchName);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(this.getDataCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, DataCondition.FieldEquals(new XVar("NAME"), (XVar)(searchName))))));
			this.dataSource.deleteSingle((XVar)(dc), new XVar(false));

			return null;
		}
		public virtual XVar getSavedSeachesParams()
		{
			dynamic data = XVar.Array(), dc = null, names = XVar.Array(), qResult = null;
			if(XVar.Pack(MVCFunctions.count(this.savedSearchesParams)))
			{
				return this.savedSearchesParams;
			}
			dc = XVar.Clone(this.getDataCommand());
			dc.order = XVar.Clone(XVar.Array());
			dc.order.InitAndSetArrayItem(new XVar("column", "NAME"), null);
			qResult = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			if(XVar.Pack(!(XVar)(qResult)))
			{
				return XVar.Array();
			}
			names = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				if((XVar)(!(XVar)(data["TYPE"]))  || (XVar)(data["TYPE"] == 1))
				{
					if(MVCFunctions.substr((XVar)(data["SEARCH"]), new XVar(0), new XVar(2)) != "{\"")
					{
						names.InitAndSetArrayItem(MVCFunctions.runner_unserialize_array((XVar)(data["SEARCH"])), data["NAME"]);
					}
					else
					{
						names.InitAndSetArrayItem(this.decode((XVar)(data["SEARCH"])), data["NAME"]);
					}
				}
			}
			this.savedSearchesParams = XVar.Clone(names);
			return names;
		}
	}
}
