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
	public partial class paramsLogger : XClass
	{
		protected dynamic var_type;
		protected dynamic userID = XVar.Pack("");
		protected dynamic cookie = XVar.Pack("");
		protected dynamic tableNameId;
		protected dynamic dataSource;
		public paramsLogger(dynamic _param_tableNameId, dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic tableNameId = XVar.Clone(_param_tableNameId);
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			this.var_type = XVar.Clone(var_type);
			this.tableNameId = XVar.Clone(tableNameId);
			this.assignUserId();
			this.assignCookieParams();
			this.dataSource = XVar.Clone(CommonFunctions.getTableDataSource((XVar)(ProjectSettings.getProjectValue(new XVar("settingsTable"), new XVar("table"))), (XVar)(GlobalVars.cman.getSavedSearchesConnId())));
		}
		protected virtual XVar assignUserId()
		{
			if(XVar.Pack(Security.isLoggedIn()))
			{
				this.userID = XVar.Clone(Security.getUserName());
			}

			return null;
		}
		protected virtual XVar assignCookieParams()
		{
			if((XVar)(!(XVar)(MVCFunctions.strlen((XVar)(MVCFunctions.GetCookie("paramsLogger")))))  && (XVar)(!(XVar)(this.userID)))
			{
				CommonFunctions.runner_setcookie(new XVar("paramsLogger"), (XVar)(CommonFunctions.generatePassword(new XVar(24))), (XVar)(MVCFunctions.time() + (5 * 365) * 86400), new XVar(""), new XVar(""), new XVar(false), new XVar(false));
			}
			this.cookie = XVar.Clone(MVCFunctions.GetCookie("paramsLogger"));

			return null;
		}
		protected virtual XVar getCommonFilter()
		{
			dynamic conditions = XVar.Array(), userConditions = XVar.Array();
			userConditions = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.userID))
			{
				userConditions.InitAndSetArrayItem(DataCondition.FieldEquals(new XVar("USERNAME"), (XVar)(this.userID)), null);
			}
			if(XVar.Pack(this.cookie))
			{
				userConditions.InitAndSetArrayItem(DataCondition.FieldEquals(new XVar("COOKIE"), (XVar)(this.cookie)), null);
			}
			if(XVar.Pack(!(XVar)(userConditions)))
			{
				return DataCondition._False();
			}
			conditions = XVar.Clone(new XVar(0, DataCondition._Or((XVar)(userConditions)), 1, DataCondition.FieldEquals(new XVar("TABLENAME"), (XVar)(this.tableNameId))));
			if(!XVar.Equals(XVar.Pack(this.var_type), XVar.Pack(Constants.SSEARCH_PARAMS_TYPE)))
			{
				conditions.InitAndSetArrayItem(DataCondition.FieldEquals(new XVar("TYPE"), (XVar)(this.var_type)), null);
			}
			return DataCondition._And((XVar)(conditions));
		}
		public virtual XVar save(dynamic _param_data, dynamic _param__values = null)
		{
			#region default values
			if(_param__values as Object == null) _param__values = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic _values = XVar.Clone(_param__values);
			#endregion

			dynamic dc = null, issetData = null, values = XVar.Array();
			issetData = XVar.Clone(MVCFunctions.strlen((XVar)(this.readData())) != 0);
			if((XVar)(issetData)  && (XVar)(this.var_type != Constants.SSEARCH_PARAMS_TYPE))
			{
				dc = XVar.Clone(this.getUpdateCommand((XVar)(data)));
				this.dataSource.updateSingle((XVar)(dc), new XVar(false));
				return null;
			}
			dc = XVar.Clone(new DsCommand());
			values = XVar.Clone(_values);
			values.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(data)), "SEARCH");
			values.InitAndSetArrayItem(this.tableNameId, "TABLENAME");
			if(XVar.Pack(this.userID))
			{
				values.InitAndSetArrayItem(this.userID, "USERNAME");
			}
			else
			{
				if(XVar.Pack(this.cookie))
				{
					values.InitAndSetArrayItem(this.cookie, "COOKIE");
				}
			}
			if(this.var_type != Constants.SSEARCH_PARAMS_TYPE)
			{
				values.InitAndSetArrayItem(this.var_type, "TYPE");
			}
			dc.values = XVar.Clone(values);
			this.dataSource.insertSingle((XVar)(dc));

			return null;
		}
		public virtual XVar saveShowHideData(dynamic _param_deviceClass, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic deviceClass = XVar.Clone(_param_deviceClass);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic dc = null, values = XVar.Array();
			if(XVar.Pack(this.getShowHideData((XVar)(deviceClass))))
			{
				dc = XVar.Clone(this.getUpdateCommand((XVar)(data)));
				dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, DataCondition.FieldEquals(new XVar("NAME"), (XVar)(deviceClass))))));
				this.dataSource.updateSingle((XVar)(dc), new XVar(false));
				return null;
			}
			dc = XVar.Clone(new DsCommand());
			values = XVar.Clone(XVar.Array());
			values.InitAndSetArrayItem(deviceClass, "NAME");
			values.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(data)), "SEARCH");
			values.InitAndSetArrayItem(this.tableNameId, "TABLENAME");
			values.InitAndSetArrayItem(this.var_type, "TYPE");
			if(XVar.Pack(this.userID))
			{
				values.InitAndSetArrayItem(this.userID, "USERNAME");
			}
			else
			{
				if(XVar.Pack(this.cookie))
				{
					values.InitAndSetArrayItem(this.cookie, "COOKIE");
				}
			}
			dc.values = XVar.Clone(values);
			this.dataSource.insertSingle((XVar)(dc));

			return null;
		}
		protected virtual XVar update(dynamic _param_data, dynamic _param_addWhere = null)
		{
			#region default values
			if(_param_addWhere as Object == null) _param_addWhere = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic addWhere = XVar.Clone(_param_addWhere);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(this.getUpdateCommand((XVar)(data)));
			this.dataSource.updateSingle((XVar)(dc));

			return null;
		}
		protected virtual XVar getUpdateCommand(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(this.getDataCommand());
			dc.values = XVar.Clone(XVar.Array());
			dc.values.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(data)), "SEARCH");
			return dc;
		}
		protected virtual XVar delete()
		{
			dynamic dataSource = null, dc = null;
			dc = XVar.Clone(this.getDataCommand());
			dataSource.deleteSingle((XVar)(dc), new XVar(false));

			return null;
		}
		public virtual XVar getData()
		{
			return this.decode((XVar)(this.readData()));
		}
		public virtual XVar decode(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic parsed = null;
			parsed = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(data)));
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(parsed)))))
			{
				return MVCFunctions.runner_unserialize_array((XVar)(data));
			}
			return parsed;
		}
		protected virtual XVar getDataCommand()
		{
			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getCommonFilter());
			return dc;
		}
		protected virtual XVar readData()
		{
			dynamic data = XVar.Array(), dc = null, qResult = null;
			dc = XVar.Clone(this.getDataCommand());
			qResult = XVar.Clone(this.dataSource.getSingle((XVar)(dc)));
			if(XVar.Pack(!(XVar)(qResult)))
			{
				return "";
			}
			data = XVar.Clone(qResult.fetchAssoc());
			if(XVar.Pack(!(XVar)(data.KeyExists("SEARCH"))))
			{
				return false;
			}
			return data["SEARCH"];
		}
		public virtual XVar getShowHideData(dynamic _param_deviceClass = null)
		{
			#region default values
			if(_param_deviceClass as Object == null) _param_deviceClass = new XVar(-1);
			#endregion

			#region pass-by-value parameters
			dynamic deviceClass = XVar.Clone(_param_deviceClass);
			#endregion

			dynamic data = XVar.Array(), dc = null, qResult = null, ret = XVar.Array();
			dc = XVar.Clone(this.getDataCommand());
			if(XVar.Pack(0) <= deviceClass)
			{
				dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, DataCondition.FieldEquals(new XVar("NAME"), (XVar)(deviceClass))))));
			}
			qResult = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			if(XVar.Pack(!(XVar)(qResult)))
			{
				return XVar.Array();
			}
			ret = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				ret.InitAndSetArrayItem(this.decode((XVar)(data["SEARCH"])), data["NAME"]);
			}
			return ret;
		}
	}
}
