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
	public partial class DataSourceSQL : DataSource
	{
		protected dynamic cipherer;
		protected static bool skipDataSourceSQLCtor = false;
		public DataSourceSQL(dynamic _param_name, dynamic _param_pSet_packed, dynamic _param_connection)
			:base((XVar)_param_name)
		{
			if(skipDataSourceSQLCtor)
			{
				skipDataSourceSQLCtor = false;
				return;
			}
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.pSet = XVar.UnPackProjectSettings(pSet);
			this.connection = XVar.Clone(connection);
			this.opDescriptors = XVar.Clone(this.pSet.getDataSourceOps());
		}
		protected override XVar getListImpl(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic result = null;
			result = XVar.Clone(this.getListData((XVar)(dc), new XVar(true)));
			if(XVar.Pack(!(XVar)(result)))
			{
				return result;
			}
			if(XVar.Pack(!(XVar)(this.codeOp(new XVar("selectList")))))
			{
				result.seekRecord((XVar)(dc.startRecord));
			}
			return result;
		}
		protected override XVar getSingleImpl(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic op = null, result = null, sql = null;
			op = new XVar("selectOne");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				return this.callCodeOp((XVar)(op), (XVar)(dc));
			}
			RunnerContext.pushDataCommandContext((XVar)(dc));
			sql = XVar.Clone(DB.PrepareSQL((XVar)(this.getSQL((XVar)(op)))));
			RunnerContext.pop();
			if(XVar.Pack(sql))
			{
				result = XVar.Clone(this.connection.limitedQuery((XVar)(sql), new XVar(0), new XVar(1), new XVar(true)));
				result.setFieldSubstitutions((XVar)(this.getFieldSubs(new XVar(false))));
				if(XVar.Pack(!(XVar)(this.opDescriptors[op]["skipFilter"])))
				{
					result = XVar.Clone(this.filterResult((XVar)(result), (XVar)(dc.filter)));
				}
			}
			else
			{
				result = XVar.Clone(this.getListData((XVar)(dc), new XVar(false)));
			}
			return result;
		}
		protected override XVar getListData(dynamic _param_dc, dynamic _param_listRequest = null)
		{
			#region default values
			if(_param_listRequest as Object == null) _param_listRequest = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic listRequest = XVar.Clone(_param_listRequest);
			#endregion

			dynamic op = null, res = null;
			if(XVar.Pack(dc._cache["listData"]))
			{
				dc._cache["listData"].seekRecord((XVar)(dc._cache["listDataPos"]));
				return dc._cache["listData"];
			}
			if(XVar.Pack(this.falseCondition((XVar)(dc.filter))))
			{
				dc._cache.InitAndSetArrayItem(new ArrayResult((XVar)(XVar.Array())), "listData");
				dc._cache.InitAndSetArrayItem(0, "listDataPos");
				return dc._cache["listData"];
			}
			op = new XVar("selectList");
			dc.filter = XVar.Clone(this.addKeysToFilter((XVar)(dc)));
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				res = XVar.Clone(this.callCodeOp((XVar)(op), (XVar)(dc)));
				if(XVar.Pack(!(XVar)(res)))
				{
					return res;
				}
			}
			else
			{
				dynamic sql = null;
				RunnerContext.pushDataCommandContext((XVar)(dc));
				sql = XVar.Clone(DB.PrepareSQL((XVar)(this.getSQL(new XVar("selectList")))));
				RunnerContext.pop();
				res = XVar.Clone(this.connection.query((XVar)(sql)));
				if(XVar.Pack(!(XVar)(res)))
				{
					return res;
				}
				res.setFieldSubstitutions((XVar)(this.getFieldSubs((XVar)(listRequest))));
				res = XVar.Clone(this.addExtraColumns((XVar)(res), (XVar)(dc)));
				if(XVar.Pack(!(XVar)(this.opDescriptors[op]["skipFilter"])))
				{
					res = XVar.Clone(this.filterResult((XVar)(res), (XVar)(dc.filter)));
				}
				if(XVar.Pack(!(XVar)(this.opDescriptors[op]["skipOrder"])))
				{
					res = XVar.Clone(this.reorderResult((XVar)(dc), (XVar)(res)));
				}
			}
			if(XVar.Pack(res.randomAccess()))
			{
				dc._cache.InitAndSetArrayItem(res, "listData");
				dc._cache.InitAndSetArrayItem(res.position(), "listDataPos");
			}
			return res;
		}
		public override XVar getCount(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic op = null, sql = null;
			op = new XVar("count");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				return this.callCodeOp((XVar)(op), (XVar)(dc));
			}
			sql = XVar.Clone(DB.PrepareSQL((XVar)(this.getSQL((XVar)(op)))));
			if(XVar.Pack(sql))
			{
				dynamic data = XVar.Array(), res = null;
				res = XVar.Clone(this.connection.query((XVar)(sql)));
				if(XVar.Pack(!(XVar)(res)))
				{
					return 0;
				}
				res = XVar.Clone(this.filterResult((XVar)(res), (XVar)(dc.filter)));
				data = XVar.Clone(res.fetchNumeric());
				if(XVar.Pack(!(XVar)(data)))
				{
					return 0;
				}
				return data[0];
			}
			else
			{
				dynamic ret = null;
				ret = XVar.Clone(this.getListData((XVar)(dc), new XVar(true)));
				if(XVar.Pack(ret))
				{
					if(XVar.Pack(!(XVar)(ret.randomAccess())))
					{
						ret = XVar.Clone(ArrayResult.createFromResult((XVar)(ret)));
						dc._cache.InitAndSetArrayItem(ret, "listData");
						dc._cache.InitAndSetArrayItem(0, "listDataPos");
					}
					return ret.count();
				}
				return 0;
			}

			return null;
		}
		protected virtual XVar getSQL(dynamic _param_op)
		{
			#region pass-by-value parameters
			dynamic op = XVar.Clone(_param_op);
			#endregion

			return this.opDescriptors[op]["sql"];
		}
		public override XVar updateSingle(dynamic _param_dc, dynamic _param_requireKeys = null)
		{
			#region default values
			if(_param_requireKeys as Object == null) _param_requireKeys = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic requireKeys = XVar.Clone(_param_requireKeys);
			#endregion

			dynamic op = null, sql = null;
			op = new XVar("update");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				return this.callCodeOp((XVar)(op), (XVar)(dc));
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.count(dc.values))))
			{
				return true;
			}
			RunnerContext.pushDataCommandContext((XVar)(dc));
			sql = XVar.Clone(DB.PrepareSQL((XVar)(this.getSQL((XVar)(op)))));
			RunnerContext.pop();
			if(XVar.Pack(this.connection.exec((XVar)(sql))))
			{
				return true;
			}
			this.setError((XVar)(this.connection.lastError()));
			return false;
		}
		public override XVar deleteSingle(dynamic _param_dc, dynamic _param_requireKeys = null)
		{
			#region default values
			if(_param_requireKeys as Object == null) _param_requireKeys = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic requireKeys = XVar.Clone(_param_requireKeys);
			#endregion

			dynamic op = null, sql = null;
			op = new XVar("delete");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				return this.callCodeOp(new XVar("deleteRecord"), (XVar)(dc));
			}
			if((XVar)(!(XVar)(MVCFunctions.count(dc.keys)))  && (XVar)(requireKeys))
			{
				return true;
			}
			RunnerContext.pushDataCommandContext((XVar)(dc));
			sql = XVar.Clone(DB.PrepareSQL((XVar)(this.getSQL((XVar)(op)))));
			RunnerContext.pop();
			if(XVar.Pack(this.connection.exec((XVar)(sql))))
			{
				return true;
			}
			this.setError((XVar)(this.connection.lastError()));
			return false;
		}
		public override XVar insertSingle(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic op = null, sql = null;
			op = new XVar("insert");
			if(XVar.Pack(this.codeOp((XVar)(op))))
			{
				dynamic ret = null;
				ret = XVar.Clone(this.callCodeOp((XVar)(op), (XVar)(dc)));
				if((XVar)(!XVar.Equals(XVar.Pack(ret), XVar.Pack(false)))  && (XVar)(!(XVar)(MVCFunctions.is_array((XVar)(ret)))))
				{
					ret = XVar.Clone(XVar.Array());
				}
				return ret;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.count(dc.values))))
			{
				return XVar.Array();
			}
			RunnerContext.pushDataCommandContext((XVar)(dc));
			sql = XVar.Clone(DB.PrepareSQL((XVar)(this.getSQL((XVar)(op)))));
			RunnerContext.pop();
			if(XVar.Pack(this.connection.exec((XVar)(sql))))
			{
				return dc.values;
			}
			this.setError((XVar)(this.connection.lastError()));
			return false;
		}
	}
}
