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
	public partial class OrderClause : XClass
	{
		private ProjectSettings pSet = null;
		private dynamic cipherer = XVar.Pack(null);
		private dynamic sessionPrefix = XVar.Pack("");
		private dynamic connection = XVar.Pack("");
		private dynamic _cachedFields = XVar.Pack(null);
		private dynamic _cachedSortBySettings = XVar.Pack(null);
		public OrderClause(dynamic _param__pSet, dynamic _param__cipherer, dynamic _param__sessionPrefix, dynamic _param__connection, dynamic _param_needReadRequest = null)
		{
			#region default values
			if(_param_needReadRequest as Object == null) _param_needReadRequest = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic _pSet = XVar.Clone(_param__pSet);
			dynamic _cipherer = XVar.Clone(_param__cipherer);
			dynamic _sessionPrefix = XVar.Clone(_param__sessionPrefix);
			dynamic _connection = XVar.Clone(_param__connection);
			dynamic needReadRequest = XVar.Clone(_param_needReadRequest);
			#endregion

			this.pSet = XVar.UnPackProjectSettings(_pSet);
			this.cipherer = XVar.Clone(_cipherer);
			this.sessionPrefix = XVar.Clone(_sessionPrefix);
			this.connection = XVar.Clone(_connection);
			if(XVar.Pack(needReadRequest))
			{
				this.readRequest();
			}
		}
		public static XVar createFromPage(dynamic _param_pageObject, dynamic _param_needReadRequest = null)
		{
			#region default values
			if(_param_needReadRequest as Object == null) _param_needReadRequest = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic needReadRequest = XVar.Clone(_param_needReadRequest);
			#endregion

			return new OrderClause((XVar)(pageObject.pSet), (XVar)(pageObject.cipherer), (XVar)(pageObject.sessionPrefix), (XVar)(pageObject.connection), (XVar)(needReadRequest));
		}
		public virtual XVar getOrderFields()
		{
			dynamic columns = XVar.Array(), groupByRet = XVar.Array(), ret = XVar.Array(), saved = XVar.Array();
			ProjectSettings pSet;
			if(!XVar.Equals(XVar.Pack(this._cachedFields), XVar.Pack(null)))
			{
				return this._cachedFields;
			}
			ret = XVar.Clone(XVar.Array());
			columns = XVar.Clone(XVar.Array());
			pSet = XVar.UnPackProjectSettings(this.pSet);
			saved = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_orderby")]);
			if(0 != MVCFunctions.strlen((XVar)(saved["orderby"])))
			{
				dynamic fields = XVar.Array();
				fields = XVar.Clone(MVCFunctions.explode(new XVar(";"), (XVar)(saved["orderby"])));
				foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
				{
					dynamic dir = null, fieldName = null, goodField = null, index = null;
					dir = XVar.Clone(MVCFunctions.substr((XVar)(f.Value), new XVar(0), new XVar(1)));
					if((XVar)(dir != "a")  && (XVar)(dir != "d"))
					{
						continue;
					}
					goodField = XVar.Clone(MVCFunctions.substr((XVar)(f.Value), new XVar(1)));
					fieldName = XVar.Clone(pSet.getFieldByGoodFieldName((XVar)(goodField)));
					index = XVar.Clone(pSet.getFieldIndex((XVar)(fieldName)));
					if(XVar.Pack(!(XVar)(index)))
					{
						continue;
					}
					ret.InitAndSetArrayItem(new XVar("column", fieldName, "index", index, "expr", RunnerPage._getFieldSQLDecrypt((XVar)(fieldName), (XVar)(this.connection), (XVar)(this.pSet), (XVar)(this.cipherer)), "dir", (XVar.Pack(dir == "a") ? XVar.Pack("ASC") : XVar.Pack("DESC"))), null);
					columns.InitAndSetArrayItem(true, fieldName);
				}
			}
			else
			{
				if(0 != MVCFunctions.strlen((XVar)(saved["sortby"])))
				{
					dynamic sortbySettings = XVar.Array(), var_option = XVar.Array();
					sortbySettings = this.getSortBySettings();
					var_option = XVar.Clone(sortbySettings[saved["sortby"] - 1]);
					if(XVar.Pack(var_option))
					{
						foreach (KeyValuePair<XVar, dynamic> f in var_option["fields"].GetEnumerator())
						{
							ret.InitAndSetArrayItem(new XVar("column", f.Value["field"], "index", pSet.getFieldIndex((XVar)(f.Value["field"])), "expr", RunnerPage._getFieldSQLDecrypt((XVar)(f.Value["field"]), (XVar)(this.connection), (XVar)(this.pSet), (XVar)(this.cipherer)), "dir", (XVar.Pack(f.Value["desc"]) ? XVar.Pack("DESC") : XVar.Pack("ASC"))), null);
							columns.InitAndSetArrayItem(true, f.Value["field"]);
						}
					}
				}
				else
				{
					dynamic orderInfo = null;
					ret = XVar.Clone(OrderClause.originalOrderFields((XVar)(pSet)));
					foreach (KeyValuePair<XVar, dynamic> of in ret.GetEnumerator())
					{
						columns.InitAndSetArrayItem(true, of.Value["column"]);
					}
					orderInfo = XVar.Clone(pSet.getOrderIndexes());
				}
			}
			if(XVar.Pack(this.orderParsed()))
			{
				foreach (KeyValuePair<XVar, dynamic> k in pSet.getTableKeys().GetEnumerator())
				{
					if(XVar.Pack(columns.KeyExists(k.Value)))
					{
						continue;
					}
					ret.InitAndSetArrayItem(new XVar("column", k.Value, "index", pSet.getFieldIndex((XVar)(k.Value)), "expr", RunnerPage._getFieldSQLDecrypt((XVar)(k.Value), (XVar)(this.connection), (XVar)(this.pSet), (XVar)(this.cipherer)), "dir", "ASC", "hidden", true), null);
				}
			}
			groupByRet = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> grField in pSet.getGroupFields().GetEnumerator())
			{
				dynamic grFieldPos = null;
				grFieldPos = XVar.Clone(-1);
				foreach (KeyValuePair<XVar, dynamic> of in ret.GetEnumerator())
				{
					if(of.Value["column"] == grField.Value)
					{
						grFieldPos = XVar.Clone(of.Key);
						break;
					}
				}
				if(grFieldPos != -1)
				{
					groupByRet.InitAndSetArrayItem(ret[grFieldPos], null);
					ret.Remove(grFieldPos);
				}
				else
				{
					groupByRet.InitAndSetArrayItem(new XVar("column", grField.Value, "index", pSet.getFieldIndex((XVar)(grField.Value)), "expr", RunnerPage._getFieldSQLDecrypt((XVar)(grField.Value), (XVar)(this.connection), (XVar)(this.pSet), (XVar)(this.cipherer)), "dir", "ASC", "hidden", true), null);
				}
			}
			foreach (KeyValuePair<XVar, dynamic> o in groupByRet.GetEnumerator())
			{
				this._cachedFields.InitAndSetArrayItem(o.Value, null);
			}
			foreach (KeyValuePair<XVar, dynamic> o in ret.GetEnumerator())
			{
				this._cachedFields.InitAndSetArrayItem(o.Value, null);
			}
			return this._cachedFields;
		}
		public static XVar originalOrderFields(dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic orderInfo = XVar.Array(), ret = XVar.Array();
			orderInfo = XVar.Clone(pSet.getOrderIndexes());
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> o in orderInfo.GetEnumerator())
			{
				dynamic field = null;
				if(o.Value[0] == 0)
				{
					field = XVar.Clone(o.Value[2]);
				}
				else
				{
					field = XVar.Clone(pSet.GetFieldByIndex((XVar)(o.Value[0])));
				}
				ret.InitAndSetArrayItem(new XVar("column", field, "index", o.Value[0], "expr", o.Value[2], "dir", o.Value[1]), null);
			}
			return ret;
		}
		public virtual XVar getOrderUrlParams()
		{
			dynamic arrParams = XVar.Array(), orderFields = XVar.Array();
			arrParams = XVar.Clone(XVar.Array());
			orderFields = XVar.Clone(this.getOrderFields());
			foreach (KeyValuePair<XVar, dynamic> field in orderFields.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(field.Value["hidden"])))
				{
					dynamic dirChar = null;
					dirChar = XVar.Clone((XVar.Pack(field.Value["dir"] == "ASC") ? XVar.Pack("a") : XVar.Pack("d")));
					arrParams.InitAndSetArrayItem(MVCFunctions.Concat(dirChar, MVCFunctions.GoodFieldName((XVar)(field.Value["column"]))), null);
				}
			}
			return MVCFunctions.implode(new XVar(";"), (XVar)(arrParams));
		}
		public virtual XVar getOrderByExpression()
		{
			dynamic orderby = XVar.Array();
			orderby = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> of in this.getOrderFields().GetEnumerator())
			{
				if(XVar.Pack(of.Value["index"]))
				{
					orderby.InitAndSetArrayItem(MVCFunctions.Concat(of.Value["index"], " ", of.Value["dir"]), null);
				}
				else
				{
					orderby.InitAndSetArrayItem(MVCFunctions.Concat(of.Value["expr"], " ", of.Value["dir"]), null);
				}
			}
			if(XVar.Pack(orderby))
			{
				return MVCFunctions.Concat(" order by ", MVCFunctions.implode(new XVar(", "), (XVar)(orderby)));
			}
			return this.pSet.getStrOrderBy();
		}
		public virtual XVar getSortBySettings()
		{
			dynamic sortSettings = XVar.Array();
			if(!XVar.Equals(XVar.Pack(this._cachedSortBySettings), XVar.Pack(null)))
			{
				return this._cachedSortBySettings;
			}
			sortSettings = XVar.Clone(this.pSet.getSortControlSettings());
			if(XVar.Pack(!(XVar)(sortSettings)))
			{
				sortSettings = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> fName in this.pSet.getListFields().GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(this.isFieldSortable((XVar)(fName.Value)))))
					{
						continue;
					}
					sortSettings.InitAndSetArrayItem(new XVar("label", "", "fields", new XVar(0, new XVar("field", fName.Value, "desc", false, "labelOnly", true))), null);
				}
			}
			this._cachedSortBySettings = XVar.Clone(sortSettings);
			return sortSettings;
		}
		protected virtual XVar isFieldSortable(dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic var_type = null;
			var_type = XVar.Clone(this.pSet.getFieldType((XVar)(fName)));
			return (XVar)(!(XVar)(CommonFunctions.IsBinaryType((XVar)(var_type))))  && (XVar)((XVar)((XVar)(this.connection.dbType == Constants.nDATABASE_MySQL)  || (XVar)(this.connection.dbType == Constants.nDATABASE_PostgreSQL))  || (XVar)(!(XVar)(CommonFunctions.IsTextType((XVar)(var_type)))));
		}
		public virtual XVar getSortByControlIdx()
		{
			dynamic normOrder = XVar.Array(), orderFields = XVar.Array(), saved = XVar.Array(), sortString = null, sortbySettings = XVar.Array();
			sortbySettings = this.getSortBySettings();
			saved = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_orderby")]);
			if(MVCFunctions.strlen((XVar)(saved["sortby"])) != 0)
			{
				dynamic idx = null;
				idx = XVar.Clone((int)saved["sortby"] - 1);
				if(XVar.Pack(sortbySettings.KeyExists(idx)))
				{
					return idx;
				}
			}
			orderFields = this.getOrderFields();
			foreach (KeyValuePair<XVar, dynamic> o in orderFields.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(o.Value["hidden"])))
				{
					normOrder.InitAndSetArrayItem(new XVar(0, o.Value["column"], 1, o.Value["dir"]), null);
				}
			}
			sortString = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(normOrder)));
			foreach (KeyValuePair<XVar, dynamic> s in sortbySettings.GetEnumerator())
			{
				normOrder = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> f in s.Value["fields"].GetEnumerator())
				{
					normOrder.InitAndSetArrayItem(new XVar(0, f.Value["field"], 1, (XVar.Pack(f.Value["desc"]) ? XVar.Pack("DESC") : XVar.Pack("ASC"))), null);
				}
				if(MVCFunctions.runner_json_encode((XVar)(normOrder)) == sortString)
				{
					return s.Key;
				}
			}
			return -1;
		}
		public virtual XVar getListQueryData()
		{
			dynamic arrFieldForSort = XVar.Array(), arrHowFieldSort = XVar.Array();
			arrFieldForSort = XVar.Clone(XVar.Array());
			arrHowFieldSort = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> of in this.getOrderFields().GetEnumerator())
			{
				arrFieldForSort.InitAndSetArrayItem(of.Value["index"], null);
				arrHowFieldSort.InitAndSetArrayItem(of.Value["dir"], null);
			}
			return new XVar("fieldsForSort", arrFieldForSort, "howToSortData", arrHowFieldSort);
		}
		protected virtual XVar readRequest()
		{
			if((XVar)(MVCFunctions.strlen((XVar)(MVCFunctions.postvalue(new XVar("orderby")))))  || (XVar)(MVCFunctions.strlen((XVar)(MVCFunctions.postvalue(new XVar("sortby"))))))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_orderby")] = new XVar("orderby", MVCFunctions.postvalue(new XVar("orderby")), "sortby", MVCFunctions.postvalue(new XVar("sortby")));
			}

			return null;
		}
		protected virtual XVar orderParsed()
		{
			return (XVar)(!(XVar)(!(XVar)(this.pSet.getOrderIndexes())))  || (XVar)(this.pSet.getStrOrderBy() == "");
		}
	}
}
