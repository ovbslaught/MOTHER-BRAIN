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
	public partial class DataSource : XClass
	{
		protected dynamic _name;
		protected dynamic connection;
		protected dynamic opDescriptors;
		protected dynamic _error = XVar.Pack("");
		protected ProjectSettings pSet;
		public DataSource(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			this._name = XVar.Clone(name);
		}
		public virtual XVar tableBased()
		{
			return false;
		}
		public virtual XVar setContext()
		{

			return null;
		}
		public virtual XVar updateContext(dynamic _param_name, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic value = XVar.Clone(_param_value);
			#endregion


			return null;
		}
		public virtual XVar getCount(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion


			return null;
		}
		public virtual XVar getList(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic result = null;
			result = XVar.Clone(this.getListImpl((XVar)(dc)));
			return this.substituteDataResult((XVar)(result));
		}
		protected virtual XVar getListImpl(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion


			return null;
		}
		protected virtual XVar getListData(dynamic _param_dc, dynamic _param_listRequest = null)
		{
			#region default values
			if(_param_listRequest as Object == null) _param_listRequest = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic listRequest = XVar.Clone(_param_listRequest);
			#endregion


			return null;
		}
		public virtual XVar deleteSingle(dynamic _param_dc, dynamic _param_requireKeys = null)
		{
			#region default values
			if(_param_requireKeys as Object == null) _param_requireKeys = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic requireKeys = XVar.Clone(_param_requireKeys);
			#endregion


			return null;
		}
		public virtual XVar getSingle(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic result = null;
			result = XVar.Clone(this.getSingleImpl((XVar)(dc)));
			return this.substituteDataResult((XVar)(result));
		}
		protected virtual XVar getSingleImpl(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion


			return null;
		}
		public virtual XVar updateSingle(dynamic _param_dc, dynamic _param_requireKeys = null)
		{
			#region default values
			if(_param_requireKeys as Object == null) _param_requireKeys = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic requireKeys = XVar.Clone(_param_requireKeys);
			#endregion


			return null;
		}
		public virtual XVar insertSingle(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion


			return null;
		}
		public virtual XVar updateMany(dynamic _param_keys, dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			dynamic values = XVar.Clone(_param_values);
			#endregion


			return null;
		}
		public virtual XVar add(dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			#endregion


			return null;
		}
		public virtual XVar getNextPrevKeys(dynamic _param_dc, dynamic data, dynamic _param_what = null)
		{
			#region default values
			if(_param_what as Object == null) _param_what = new XVar(Constants.BOTH_RECORDS);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic what = XVar.Clone(_param_what);
			#endregion


			return null;
		}
		protected virtual XVar basicFieldCondition(dynamic _param_op)
		{
			#region pass-by-value parameters
			dynamic op = XVar.Clone(_param_op);
			#endregion

			return (XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(op == Constants.dsopMORE)  || (XVar)(op == Constants.dsopLESS))  || (XVar)(op == Constants.dsopEQUAL))  || (XVar)(op == Constants.dsopEMPTY))  || (XVar)(op == Constants.dsopNULL))  || (XVar)(op == Constants.dsopIN))  || (XVar)(op == Constants.dsopCONTAIN))  || (XVar)(op == Constants.dsopSTART))  || (XVar)(op == Constants.dsopALL_IN_LIST))  || (XVar)(op == Constants.dsopSOME_IN_LIST))  || (XVar)(op == Constants.dsopSTART))  || (XVar)(op == Constants.dsopBETWEEN);
		}
		public virtual XVar lastAutoincValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion


			return null;
		}
		protected virtual XVar falseCondition(dynamic _param_condition)
		{
			#region pass-by-value parameters
			dynamic condition = XVar.Clone(_param_condition);
			#endregion

			this.flattenANDs((XVar)(condition));
			if(XVar.Pack(!(XVar)(condition)))
			{
				return false;
			}
			if(XVar.Equals(XVar.Pack(condition.operation), XVar.Pack(Constants.dsopFALSE)))
			{
				return true;
			}
			if(XVar.Equals(XVar.Pack(condition.operation), XVar.Pack(Constants.dsopAND)))
			{
				foreach (KeyValuePair<XVar, dynamic> op in condition.operands.GetEnumerator())
				{
					if(XVar.Equals(XVar.Pack(op.Value.value.operation), XVar.Pack(Constants.dsopFALSE)))
					{
						return true;
					}
				}
			}
			return false;
		}
		public virtual XVar addExtraColumns(dynamic _param_rs, dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic rs = XVar.Clone(_param_rs);
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic data = XVar.Array(), ret = XVar.Array();
			if(XVar.Pack(!(XVar)(dc.extraColumns)))
			{
				return rs;
			}
			ret = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(rs.fetchAssoc())))
			{
				foreach (KeyValuePair<XVar, dynamic> ec in dc.extraColumns.GetEnumerator())
				{
					if(XVar.Pack(ec.Value.field))
					{
						data.InitAndSetArrayItem(DataSource.groupValue((XVar)(data[ec.Value.field]), (XVar)(this.getFieldType((XVar)(ec.Value.field))), (XVar)(ec.Value.modifier)), ec.Value.alias);
					}
				}
				ret.InitAndSetArrayItem(data, null);
			}
			return new ArrayResult((XVar)(ret));
		}
		public virtual XVar filterResult(dynamic _param_result, dynamic _param_filter)
		{
			#region pass-by-value parameters
			dynamic result = XVar.Clone(_param_result);
			dynamic filter = XVar.Clone(_param_filter);
			#endregion

			dynamic data = null, ret = XVar.Array();
			if(XVar.Pack(this.falseCondition((XVar)(filter))))
			{
				return new ArrayResult((XVar)(XVar.Array()));
			}
			ret = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(result.fetchAssoc())))
			{
				if(XVar.Pack(this.filterRecord((XVar)(data), (XVar)(filter))))
				{
					ret.InitAndSetArrayItem(data, null);
				}
			}
			return new ArrayResult((XVar)(ret));
		}
		protected virtual XVar filterRecord(dynamic _param_data, dynamic _param_filter)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic filter = XVar.Clone(_param_filter);
			#endregion

			dynamic op = null;
			if(XVar.Pack(!(XVar)(filter)))
			{
				return true;
			}
			op = XVar.Clone(filter.operation);
			if((XVar)(op == Constants.dsopAND)  || (XVar)(op == Constants.dsopOR))
			{
				if(XVar.Pack(!(XVar)(filter.operands)))
				{
					return true;
				}
				foreach (KeyValuePair<XVar, dynamic> o in filter.operands.GetEnumerator())
				{
					dynamic result = null;
					result = XVar.Clone(this.filterRecord((XVar)(data), (XVar)(o.Value.value)));
					if((XVar)(!(XVar)(result))  && (XVar)(op == Constants.dsopAND))
					{
						return false;
					}
					if((XVar)(result)  && (XVar)(op == Constants.dsopOR))
					{
						return true;
					}
				}
				return (XVar.Pack(op == Constants.dsopOR) ? XVar.Pack(false) : XVar.Pack(true));
			}
			else
			{
				if(op == Constants.dsopNOT)
				{
					return !(XVar)(this.filterRecord((XVar)(data), (XVar)(filter.operands[0].value)));
				}
				else
				{
					if(op == Constants.dsopFALSE)
					{
						return false;
					}
					else
					{
						if(XVar.Pack(basicFieldCondition((XVar)(op))))
						{
							return this.checkBasicFieldCondition((XVar)(data), (XVar)(filter));
						}
					}
				}
			}
			return true;
		}
		protected virtual XVar checkBasicFieldCondition(dynamic _param_data, dynamic _param_condition)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic condition = XVar.Clone(_param_condition);
			#endregion

			dynamic fieldName = null, fieldValue = null, modifier = null, op = null, value = null, value1 = null;
			op = XVar.Clone(condition.operation);
			fieldName = XVar.Clone(condition.operands[0].value);
			modifier = XVar.Clone(condition.operands[0].modifier);
			fieldValue = XVar.Clone(data[fieldName]);
			if(XVar.Pack(modifier))
			{
				fieldValue = XVar.Clone(DataSource.groupValue((XVar)(fieldValue), (XVar)(this.getFieldType((XVar)(fieldName))), (XVar)(modifier)));
			}
			if(op == Constants.dsopEMPTY)
			{
				return (XVar)(XVar.Equals(XVar.Pack(fieldValue), XVar.Pack("")))  || (XVar)(XVar.Equals(XVar.Pack(fieldValue), XVar.Pack(null)));
			}
			if(op == Constants.dsopNULL)
			{
				return XVar.Equals(XVar.Pack(fieldValue), XVar.Pack(null));
			}
			value = XVar.Clone(condition.operands[1].value);
			value1 = new XVar("");
			if(2 < MVCFunctions.count(condition.operands))
			{
				value1 = XVar.Clone(condition.operands[2].value);
			}
			if(XVar.Equals(XVar.Pack(condition.caseInsensitive), XVar.Pack(Constants.dsCASE_INSENSITIVE)))
			{
				fieldValue = XVar.Clone(MVCFunctions.strtoupper((XVar)(fieldValue)));
				value = XVar.Clone(MVCFunctions.strtoupper((XVar)(value)));
				value1 = XVar.Clone(MVCFunctions.strtoupper((XVar)(value1)));
			}
			if(op == Constants.dsopMORE)
			{
				return value < fieldValue;
			}
			else
			{
				if(op == Constants.dsopLESS)
				{
					return fieldValue < value;
				}
				else
				{
					if(op == Constants.dsopEQUAL)
					{
						return fieldValue == value;
					}
					else
					{
						if(op == Constants.dsopCONTAIN)
						{
							return !XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(fieldValue), (XVar)(value))), XVar.Pack(false));
						}
						else
						{
							if(op == Constants.dsopSTART)
							{
								return XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(fieldValue), (XVar)(value))), XVar.Pack(0));
							}
							else
							{
								if(op == Constants.dsopBETWEEN)
								{
									return (XVar)(value <= fieldValue)  && (XVar)(fieldValue <= value1);
								}
								else
								{
									if(op == Constants.dsopIN)
									{
										if(XVar.Equals(XVar.Pack(condition.caseInsensitive), XVar.Pack(Constants.dsCASE_INSENSITIVE)))
										{
											return !XVar.Equals(XVar.Pack(CommonFunctions.getArrayElementNC((XVar)(value), (XVar)(fieldValue))), XVar.Pack(null));
										}
										else
										{
											return !XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(fieldValue), (XVar)(value))), XVar.Pack(false));
										}
									}
								}
							}
						}
					}
				}
			}
			return false;
		}
		protected virtual XVar flattenANDs(dynamic _param_condition)
		{
			#region pass-by-value parameters
			dynamic condition = XVar.Clone(_param_condition);
			#endregion

			dynamic newOperands = XVar.Array();
			if(XVar.Pack(!(XVar)(condition)))
			{
				return null;
			}
			if(condition.operation != Constants.dsopAND)
			{
				return null;
			}
			newOperands = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> cop in condition.operands.GetEnumerator())
			{
				dynamic cond = null;
				if(XVar.Pack(!(XVar)(cop.Value.value)))
				{
					continue;
				}
				cond = XVar.Clone(cop.Value.value);
				this.flattenANDs((XVar)(cond));
				if(XVar.Equals(XVar.Pack(cond.operation), XVar.Pack(Constants.dsopAND)))
				{
					foreach (KeyValuePair<XVar, dynamic> op in cond.operands.GetEnumerator())
					{
						newOperands.InitAndSetArrayItem(op.Value, null);
					}
				}
				else
				{
					newOperands.InitAndSetArrayItem(cop.Value, null);
				}
			}
			condition.operands = XVar.Clone(newOperands);

			return null;
		}
		public virtual XVar reorderResult(dynamic _param_dc, dynamic _param_res)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic res = XVar.Clone(_param_res);
			#endregion

			if(XVar.Pack(!(XVar)(dc.order)))
			{
				return res;
			}
			return res.reorder((XVar)(new XVar(0, dc, 1, "compareRecords")));
		}
		public virtual XVar getOpSubtype(dynamic _param_op)
		{
			#region pass-by-value parameters
			dynamic op = XVar.Clone(_param_op);
			#endregion

			if(XVar.Pack(!(XVar)(this.opDescriptors[op])))
			{
				return "";
			}
			return this.opDescriptors[op]["subtype"];
		}
		public virtual XVar codeOp(dynamic _param_op)
		{
			#region pass-by-value parameters
			dynamic op = XVar.Clone(_param_op);
			#endregion

			return this.getOpSubtype((XVar)(op)) == "code";
		}
		public virtual XVar callCodeOp(dynamic _param_op, dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic op = XVar.Clone(_param_op);
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic events = null, ret = null;
			events = XVar.Clone(CommonFunctions.getEventObject((XVar)(this.pSet)));
			if(XVar.Pack(!(XVar)(events)))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(events.exists((XVar)(op)))))
			{
				return false;
			}
			RunnerContext.pushDataCommandContext((XVar)(dc));
			ret = XVar.Clone(events.Invoke(op, this, (XVar)(dc)));
			RunnerContext.pop();
			return ret;
		}
		public virtual XVar lastError()
		{
			if(XVar.Pack(this._error))
			{
				return this._error;
			}
			return this.connection.lastError();
		}
		public virtual XVar setError(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			this._error = XVar.Clone(str);

			return null;
		}
		public virtual XVar getTotalCount(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic totals = null;
			totals = XVar.Clone(this.getTotals((XVar)(dc)));
			if(XVar.Pack(totals))
			{
				return totals.count();
			}
			return totals;
		}
		public virtual XVar getTotals(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic arrPtr = XVar.Array(), groupFields = XVar.Array(), groupModifiers = XVar.Array(), groupRs = null, groupTypes = XVar.Array(), orderCommand = null, record = XVar.Array(), records = null, ret = null, rs = null, skipEmptyGroups = XVar.Array(), skipRecord = null;
			rs = XVar.Clone(this.getListData((XVar)(dc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				return false;
			}
			groupFields = XVar.Clone(XVar.Array());
			groupTypes = XVar.Clone(XVar.Array());
			groupModifiers = XVar.Clone(XVar.Array());
			skipEmptyGroups = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> t in dc.totals.GetEnumerator())
			{
				if((XVar)(!(XVar)(t.Value["total"]))  || (XVar)(t.Value["total"] == "distinct"))
				{
					groupFields.InitAndSetArrayItem(t.Value["field"], null);
					groupTypes.InitAndSetArrayItem(this.getFieldType((XVar)(t.Value["field"])), t.Value["field"]);
					groupModifiers.InitAndSetArrayItem(t.Value["modifier"], t.Value["field"]);
					if(XVar.Pack(t.Value["skipEmpty"]))
					{
						skipEmptyGroups.InitAndSetArrayItem(true, t.Value["field"]);
					}
				}
			}
			records = XVar.Clone(XVar.Array());
			while(XVar.Pack(record = rs.fetchAssoc()))
			{
				arrPtr = records;
				skipRecord = new XVar(false);
				foreach (KeyValuePair<XVar, dynamic> gf in groupFields.GetEnumerator())
				{
					dynamic gValue = null;
					gValue = XVar.Clone(DataSource.groupValue((XVar)(record[gf.Value]), (XVar)(groupTypes[gf.Value]), (XVar)(groupModifiers[gf.Value])));
					record.InitAndSetArrayItem(gValue, gf.Value);
					if((XVar)(gValue == XVar.Pack(""))  && (XVar)(skipEmptyGroups[gf.Value]))
					{
						skipRecord = new XVar(true);
						break;
					}
					if(XVar.Pack(!(XVar)(arrPtr.KeyExists(gValue))))
					{
						arrPtr.InitAndSetArrayItem(XVar.Array(), gValue);
					}
					arrPtr = arrPtr[gValue];
				}
				if(XVar.Pack(!(XVar)(skipRecord)))
				{
					arrPtr.InitAndSetArrayItem(record, null);
				}
			}
			ret = XVar.Clone(XVar.Array());
			this.calculateTotals((XVar)(dc), (XVar)(records), (XVar)(MVCFunctions.count(groupFields)), (XVar)(ret));
			groupRs = XVar.Clone(new ArrayResult((XVar)(ret)));
			orderCommand = XVar.Clone(new DsCommand());
			foreach (KeyValuePair<XVar, dynamic> t in dc.totals.GetEnumerator())
			{
				dynamic totalField = null;
				if(XVar.Pack(!(XVar)(t.Value["direction"])))
				{
					continue;
				}
				totalField = XVar.Clone((XVar.Pack(t.Value["alias"]) ? XVar.Pack(t.Value["alias"]) : XVar.Pack(t.Value["field"])));
				orderCommand.order.InitAndSetArrayItem(new XVar("column", totalField, "dir", t.Value["direction"]), null);
			}
			this.reorderResult((XVar)(orderCommand), (XVar)(groupRs));
			groupRs.seekRecord((XVar)(dc.startRecord));
			return groupRs;
		}
		protected virtual XVar getTotalOperantValue(dynamic _param_op, dynamic record)
		{
			#region pass-by-value parameters
			dynamic op = XVar.Clone(_param_op);
			#endregion

			if(XVar.Pack(op))
			{
				if(op.var_type == Constants.dsotFIELD)
				{
					return record[op.value];
				}
				if(op.var_type == Constants.dsotCONST)
				{
					return op.value;
				}
			}
			return null;
		}
		protected virtual XVar getCaseStatementResult(dynamic _param_caseExpr, dynamic record)
		{
			#region pass-by-value parameters
			dynamic caseExpr = XVar.Clone(_param_caseExpr);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> condition in caseExpr.conditions.GetEnumerator())
			{
				if(XVar.Pack(this.filterRecord((XVar)(record), (XVar)(condition.Value))))
				{
					return new XVar("value", this.getTotalOperantValue((XVar)(caseExpr.values[condition.Key]), (XVar)(record)), "skipRecordTotal", caseExpr.values[condition.Key].var_type == Constants.dsotNULL);
				}
			}
			return new XVar("value", this.getTotalOperantValue((XVar)(caseExpr.defValue), (XVar)(record)), "skipRecordTotal", caseExpr.defValue.var_type == Constants.dsotNULL);
		}
		protected virtual XVar getGroupTotals(dynamic _param_dc, dynamic records)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic recordCounts = XVar.Array(), ret = XVar.Array(), totalField = null;
			ret = XVar.Clone(XVar.Array());
			recordCounts = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> r in records.GetEnumerator())
			{
				dynamic needContinue = null;
				needContinue = new XVar(false);
				foreach (KeyValuePair<XVar, dynamic> t in dc.totals.GetEnumerator())
				{
					dynamic skipRecordTotal = null, sourceField = null, sourceFieldValue = null;
					sourceField = XVar.Clone(t.Value["field"]);
					totalField = XVar.Clone((XVar.Pack(t.Value["alias"]) ? XVar.Pack(t.Value["alias"]) : XVar.Pack(t.Value["field"])));
					if((XVar)(t.Value["total"])  && (XVar)(t.Value["total"] != "distinct"))
					{
						needContinue = new XVar(true);
					}
					if(XVar.Pack(t.Value["caseStatement"]))
					{
						dynamic caseResult = XVar.Array();
						caseResult = XVar.Clone(this.getCaseStatementResult((XVar)(t.Value["caseStatement"]), (XVar)(r.Value)));
						sourceFieldValue = XVar.Clone(caseResult["value"]);
						skipRecordTotal = XVar.Clone(caseResult["skipRecordTotal"]);
					}
					else
					{
						sourceFieldValue = XVar.Clone(r.Value[sourceField]);
						skipRecordTotal = new XVar(false);
					}
					if(XVar.Pack(skipRecordTotal))
					{
						continue;
					}
					if((XVar)(!(XVar)(t.Value["total"]))  || (XVar)(t.Value["total"] == "distinct"))
					{
						if(XVar.Pack(!(XVar)(ret.KeyExists(totalField))))
						{
							ret.InitAndSetArrayItem(sourceFieldValue, totalField);
						}
					}
					else
					{
						if(t.Value["total"] != "count")
						{
							if((XVar)(t.Value["total"] == "sum")  || (XVar)(t.Value["total"] == "avg"))
							{
								if(XVar.Pack(!(XVar)(ret.KeyExists(totalField))))
								{
									ret.InitAndSetArrayItem(0, totalField);
								}
								ret[totalField] += sourceFieldValue;
								if(XVar.Pack(!(XVar)(recordCounts[totalField])))
								{
									recordCounts.InitAndSetArrayItem(0, totalField);
								}
								++(recordCounts[totalField]);
							}
							if((XVar)(t.Value["total"] == "min")  || (XVar)(t.Value["total"] == "max"))
							{
								if(XVar.Pack(!(XVar)(ret.KeyExists(totalField))))
								{
									ret.InitAndSetArrayItem(sourceFieldValue, totalField);
								}
								else
								{
									if((XVar)((XVar)(t.Value["total"] == "min")  && (XVar)(sourceFieldValue < ret[totalField]))  || (XVar)((XVar)(t.Value["total"] == "max")  && (XVar)(ret[totalField] < sourceFieldValue)))
									{
										ret.InitAndSetArrayItem(sourceFieldValue, totalField);
									}
								}
							}
						}
						else
						{
							if(XVar.Pack(!(XVar)(ret.KeyExists(totalField))))
							{
								ret.InitAndSetArrayItem(0, totalField);
							}
							ret[totalField] += 1;
						}
					}
				}
				if(XVar.Pack(!(XVar)(needContinue)))
				{
					break;
				}
			}
			foreach (KeyValuePair<XVar, dynamic> t in dc.totals.GetEnumerator())
			{
				totalField = XVar.Clone((XVar.Pack(t.Value["alias"]) ? XVar.Pack(t.Value["alias"]) : XVar.Pack(t.Value["field"])));
				if((XVar)(t.Value["total"] == "avg")  && (XVar)(recordCounts[totalField]))
				{
					ret[totalField] /= recordCounts[totalField];
				}
			}
			return ret;
		}
		protected virtual XVar calculateTotals(dynamic _param_dc, dynamic records, dynamic _param_levelsLeft, dynamic ret)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic levelsLeft = XVar.Clone(_param_levelsLeft);
			#endregion

			if(XVar.Pack(!(XVar)(levelsLeft)))
			{
				ret.InitAndSetArrayItem(this.getGroupTotals((XVar)(dc), (XVar)(records)), null);
			}
			else
			{
				--(levelsLeft);
				foreach (KeyValuePair<XVar, dynamic> ptr in records.GetEnumerator())
				{
					this.calculateTotals((XVar)(dc), (XVar)(ptr.Value), (XVar)(levelsLeft), (XVar)(ret));
				}
			}

			return null;
		}
		public static XVar groupValue(dynamic _param_value, dynamic _param_ftype, dynamic _param_modifier)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic ftype = XVar.Clone(_param_ftype);
			dynamic modifier = XVar.Clone(_param_modifier);
			#endregion

			if(XVar.Pack(!(XVar)(modifier)))
			{
				return value;
			}
			if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(ftype))))
			{
				return DataSource.groupValueNumber((XVar)(value), (XVar)(modifier));
			}
			else
			{
				if(XVar.Pack(CommonFunctions.IsCharType((XVar)(ftype))))
				{
					return DataSource.groupValueChar((XVar)(value), (XVar)(modifier));
				}
				else
				{
					if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(ftype))))
					{
						return DataSource.groupValueDate((XVar)(value), (XVar)(modifier));
					}
				}
			}
			return value;
		}
		public static XVar groupValueNumber(dynamic _param_value, dynamic _param_modifier)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic modifier = XVar.Clone(_param_modifier);
			#endregion

			if(XVar.Pack(!(XVar)(modifier)))
			{
				return value;
			}
			if(XVar.Pack(0) <= value)
			{
				return (XVar)Math.Floor((double)(value / modifier)) * modifier;
			}
			else
			{
				return (XVar)Math.Ceiling((double)(value / modifier)) * modifier;
			}

			return null;
		}
		protected static XVar groupValueChar(dynamic _param_value, dynamic _param_modifier)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic modifier = XVar.Clone(_param_modifier);
			#endregion

			if(XVar.Pack(!(XVar)(modifier)))
			{
				return value;
			}
			return MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(modifier));
		}
		public static XVar groupValueDate(dynamic _param_value, dynamic _param_modifier)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic modifier = XVar.Clone(_param_modifier);
			#endregion

			dynamic quarter = null, time = XVar.Array(), week = null;
			time = XVar.Clone(MVCFunctions.db2time((XVar)(value)));
			if(XVar.Pack(!(XVar)(time)))
			{
				return value;
			}
			switch(((XVar)modifier).ToInt())
			{
				case 1:
					return MVCFunctions.Concat(MVCFunctions.str_pad((XVar)(time[0]), new XVar(4), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), "0101");
				case 2:
					quarter = XVar.Clone((XVar)Math.Floor((double)((time[1] - 1) / 3)) + 1);
					return MVCFunctions.Concat(MVCFunctions.str_pad((XVar)(time[0]), new XVar(4), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(quarter), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), "01");
				case 3:
					return MVCFunctions.Concat(MVCFunctions.str_pad((XVar)(time[0]), new XVar(4), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[1]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), "01");
				case 4:
					week = XVar.Clone(CommonFunctions.getweeknumber((XVar)(time)));
					return MVCFunctions.Concat(MVCFunctions.str_pad((XVar)(time[0]), new XVar(4), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(week), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), "01");
				case 5:
					return MVCFunctions.Concat(MVCFunctions.str_pad((XVar)(time[0]), new XVar(4), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[1]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[2]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)));
				case 6:
					return MVCFunctions.Concat(MVCFunctions.str_pad((XVar)(time[0]), new XVar(4), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[1]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[2]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[3]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)));
				case 7:
					return MVCFunctions.Concat(MVCFunctions.str_pad((XVar)(time[0]), new XVar(4), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[1]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[2]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[3]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)), MVCFunctions.str_pad((XVar)(time[4]), new XVar(2), new XVar("0"), new XVar(Constants.STR_PAD_LEFT)));
			}
			return value;
		}
		public virtual XVar overrideSQL(dynamic _param_dc, dynamic _param_sql)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic sql = XVar.Clone(_param_sql);
			#endregion


			return null;
		}
		public virtual XVar overrideWhere(dynamic _param_dc, dynamic _param_where, dynamic _param_having = null)
		{
			#region default values
			if(_param_having as Object == null) _param_having = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic where = XVar.Clone(_param_where);
			dynamic having = XVar.Clone(_param_having);
			#endregion


			return null;
		}
		public virtual XVar overrideOrder(dynamic _param_dc, dynamic _param_orderby)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic orderby = XVar.Clone(_param_orderby);
			#endregion


			return null;
		}
		public virtual XVar getConnection()
		{
			return this.connection;
		}
		public virtual XVar getConnectionId()
		{
			return this.connection.connId;
		}
		public virtual XVar prepareSQL(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			return new XVar("sql", "", "where", "", "order", "");
		}
		public virtual XVar checkAuthorization()
		{
			return true;
		}
		public virtual XVar getAuthorizationInfo()
		{
			return null;
		}
		public virtual XVar decryptRecord(dynamic data)
		{
			return data;
		}
		public virtual XVar wrap(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return str;
		}
		protected virtual XVar makeAdvancedValues(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> value in dc.values.GetEnumerator())
			{
				if(XVar.Pack(dc.advValues.KeyExists(value.Key)))
				{
					continue;
				}
				dc.advValues.InitAndSetArrayItem(new DsOperand(new XVar(Constants.dsotCONST), (XVar)(value.Value)), value.Key);
			}

			return null;
		}
		public virtual XVar updateRowNumber(dynamic _param_dc, dynamic _param_startNumber = null)
		{
			#region default values
			if(_param_startNumber as Object == null) _param_startNumber = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic startNumber = XVar.Clone(_param_startNumber);
			#endregion

			return null;
		}
		public virtual XVar updateRowNumberAvailable(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			return false;
		}
		public virtual XVar silentMode(dynamic _param_mode)
		{
			#region pass-by-value parameters
			dynamic mode = XVar.Clone(_param_mode);
			#endregion


			return null;
		}
		public virtual XVar getColumnList()
		{
			return XVar.Array();
		}
		public virtual XVar getFieldSubs(dynamic _param_listRequest)
		{
			#region pass-by-value parameters
			dynamic listRequest = XVar.Clone(_param_listRequest);
			#endregion

			dynamic ret = XVar.Array();
			if(XVar.Pack(!(XVar)(this.pSet)))
			{
				return XVar.Array();
			}
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getFieldsList().GetEnumerator())
			{
				dynamic source = null;
				source = XVar.Clone(this.pSet.getFieldSource((XVar)(f.Value), (XVar)(listRequest)));
				if((XVar)(!(XVar)(source))  && (XVar)(!(XVar)(listRequest)))
				{
					source = XVar.Clone(this.pSet.getFieldSource((XVar)(f.Value), new XVar(true)));
				}
				if(XVar.Pack(!(XVar)(source)))
				{
					continue;
				}
				ret.InitAndSetArrayItem(source, f.Value);
			}
			return ret;
		}
		private XVar substituteDataResult(dynamic _param_result)
		{
			#region pass-by-value parameters
			dynamic result = XVar.Clone(_param_result);
			#endregion

			dynamic eventsObject = null;
			if(XVar.Pack(!(XVar)(this.pSet)))
			{
				return result;
			}
			eventsObject = XVar.Clone(CommonFunctions.getEventObject((XVar)(this.pSet)));
			if((XVar)(eventsObject)  && (XVar)(eventsObject.exists(new XVar("ProcessRecord"))))
			{
				return new EventDataResult((XVar)(result), (XVar)(eventsObject));
			}
			return result;
		}
		protected virtual XVar addKeysToFilter(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic filter = null;
			filter = XVar.Clone(dc.filter);
			if(XVar.Pack(dc.keys))
			{
				filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, DataCondition.FieldsEqual((XVar)(this.getKeyFields()), (XVar)(dc.keys))))));
			}
			return filter;
		}
		protected virtual XVar getKeyFields()
		{
			if(XVar.Pack(this.pSet))
			{
				return this.pSet.getTableKeys();
			}
			return XVar.Array();
		}
		public virtual XVar getFieldType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(this.pSet))
			{
				return this.pSet.getFieldType((XVar)(field));
			}
			MVCFunctions.trigger_error(new XVar("Unsupported datasource"), new XVar(Constants.E_USER_ERROR));
			return 200;
		}
	}
}
