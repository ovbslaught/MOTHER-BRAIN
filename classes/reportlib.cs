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
	public partial class ReportField : XClass
	{
		public dynamic _interval = XVar.Pack(0);
		public dynamic _tName = XVar.Pack("");
		public dynamic _name = XVar.Pack("");
		public dynamic _alias = XVar.Pack("");
		public dynamic _sqlname = XVar.Pack("");
		public dynamic _start = XVar.Pack(0);
		public dynamic _caseSensitive = XVar.Pack(false);
		public dynamic _recordBasedRequest = XVar.Pack(false);
		public dynamic _rowsInSummary = XVar.Pack(0);
		public dynamic _rowsInHeader = XVar.Pack(0);
		public dynamic _viewFormat = XVar.Pack("");
		public dynamic _oldAlgorithm = XVar.Pack(false);
		public ProjectSettings pSet = null;
		public dynamic cipherer = XVar.Pack(null);
		public dynamic _connection;
		public ReportField(dynamic _param_name, dynamic _param_interval, dynamic _param_alias, dynamic _param_table, dynamic _param_connection, dynamic _param_cipherer)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic interval = XVar.Clone(_param_interval);
			dynamic alias = XVar.Clone(_param_alias);
			dynamic table = XVar.Clone(_param_table);
			dynamic connection = XVar.Clone(_param_connection);
			dynamic cipherer = XVar.Clone(_param_cipherer);
			#endregion

			this._name = XVar.Clone(name);
			this._interval = XVar.Clone(interval);
			this._alias = XVar.Clone(alias);
			this._sqlname = XVar.Clone(alias);
			this._tName = XVar.Clone(table);
			this._connection = XVar.Clone(connection);
			this.cipherer = XVar.Clone(cipherer);
			if(table != XVar.Pack(""))
			{
				this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			}
		}
		public virtual XVar getFieldName(dynamic _param_fieldValue, dynamic _param_data = null, dynamic _param_pageObject = null)
		{
			#region default values
			if(_param_data as Object == null) _param_data = new XVar();
			if(_param_pageObject as Object == null) _param_pageObject = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic fieldValue = XVar.Clone(_param_fieldValue);
			dynamic data = XVar.Clone(_param_data);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public virtual XVar getGroup(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			return data[this.alias()];
		}
		public virtual XVar getKey(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			return data[this.alias()];
		}
		public virtual XVar setStart(dynamic _param_start)
		{
			#region pass-by-value parameters
			dynamic start = XVar.Clone(_param_start);
			#endregion

			this._start = XVar.Clone(start);
			this._sqlname = XVar.Clone(this.alias());
			return start + 1;
		}
		public virtual XVar name()
		{
			return this._name;
		}
		public virtual XVar alias()
		{
			return MVCFunctions.Concat(this._alias, this._start);
		}
		public virtual XVar overrideFormat()
		{
			return false;
		}
		public virtual XVar setCaseSensitive(dynamic _param_cs)
		{
			#region pass-by-value parameters
			dynamic cs = XVar.Clone(_param_cs);
			#endregion

			this._caseSensitive = XVar.Clone(cs);

			return null;
		}
	}
	public partial class ReportNumericField : ReportField
	{
		protected static bool skipReportNumericFieldCtor = false;
		public ReportNumericField(dynamic _param_name, dynamic _param_interval, dynamic _param_alias, dynamic _param_table, dynamic _param_connection, dynamic _param_cipherer)
			:base((XVar)_param_name, (XVar)_param_interval, (XVar)_param_alias, (XVar)_param_table, (XVar)_param_connection, (XVar)_param_cipherer)
		{
			if(skipReportNumericFieldCtor)
			{
				skipReportNumericFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic interval = XVar.Clone(_param_interval);
			dynamic alias = XVar.Clone(_param_alias);
			dynamic table = XVar.Clone(_param_table);
			dynamic connection = XVar.Clone(_param_connection);
			dynamic cipherer = XVar.Clone(_param_cipherer);
			#endregion

		}
		public override XVar getFieldName(dynamic _param_fieldValue, dynamic _param_data = null, dynamic _param_pageObject = null)
		{
			#region default values
			if(_param_data as Object == null) _param_data = new XVar();
			if(_param_pageObject as Object == null) _param_pageObject = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic fieldValue = XVar.Clone(_param_fieldValue);
			dynamic data = XVar.Clone(_param_data);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			dynamic value = null;
			value = XVar.Clone(data[(XVar.Pack(this._recordBasedRequest) ? XVar.Pack(this._name) : XVar.Pack(this._sqlname))]);
			if(XVar.Equals(XVar.Pack(value), XVar.Pack(null)))
			{
				return "NULL";
			}
			if(0 < this._interval)
			{
				dynamic start = null;
				start = XVar.Clone(DataSource.groupValueNumber((XVar)(value), (XVar)(this._interval)));
				return MVCFunctions.Concat(start, " - ", (start + this._interval) - 1);
			}
			else
			{
				return value;
			}

			return null;
		}
		public override XVar getKey(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if(XVar.Pack(this._recordBasedRequest))
			{
				if(0 < this._interval)
				{
					return DataSource.groupValueNumber((XVar)(data[this._name]), (XVar)(this._interval));
				}
				else
				{
					return data[this._name];
				}
			}
			else
			{
				return base.getKey((XVar)(data));
			}

			return null;
		}
	}
	public partial class ReportCharField : ReportField
	{
		protected static bool skipReportCharFieldCtor = false;
		public ReportCharField(dynamic _param_name, dynamic _param_interval, dynamic _param_alias, dynamic _param_table, dynamic _param_connection, dynamic _param_cipherer)
			:base((XVar)_param_name, (XVar)_param_interval, (XVar)_param_alias, (XVar)_param_table, (XVar)_param_connection, (XVar)_param_cipherer)
		{
			if(skipReportCharFieldCtor)
			{
				skipReportCharFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic interval = XVar.Clone(_param_interval);
			dynamic alias = XVar.Clone(_param_alias);
			dynamic table = XVar.Clone(_param_table);
			dynamic connection = XVar.Clone(_param_connection);
			dynamic cipherer = XVar.Clone(_param_cipherer);
			#endregion

		}
		public override XVar getFieldName(dynamic _param_fieldValue, dynamic _param_data = null, dynamic _param_pageObject = null)
		{
			#region default values
			if(_param_data as Object == null) _param_data = new XVar();
			if(_param_pageObject as Object == null) _param_pageObject = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic fieldValue = XVar.Clone(_param_fieldValue);
			dynamic data = XVar.Clone(_param_data);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			dynamic value = null;
			value = XVar.Clone(data[(XVar.Pack(this._recordBasedRequest) ? XVar.Pack(this._name) : XVar.Pack(this._sqlname))]);
			if(XVar.Equals(XVar.Pack(value), XVar.Pack(null)))
			{
				return "NULL";
			}
			if(0 < this._interval)
			{
				return MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(this._interval));
			}
			else
			{
				return value;
			}

			return null;
		}
		public override XVar getKey(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if(XVar.Pack(this._recordBasedRequest))
			{
				if(0 < this._interval)
				{
					if(XVar.Pack(this._caseSensitive))
					{
						return MVCFunctions.substr((XVar)(data[this._name]), new XVar(0), (XVar)(this._interval));
					}
					else
					{
						return MVCFunctions.strtolower((XVar)(MVCFunctions.substr((XVar)(data[this._name]), new XVar(0), (XVar)(this._interval))));
					}
				}
				else
				{
					if(XVar.Pack(this._caseSensitive))
					{
						return data[this._name];
					}
					else
					{
						return MVCFunctions.strtolower((XVar)(data[this._name]));
					}
				}
			}
			else
			{
				if(XVar.Pack(this._caseSensitive))
				{
					return data[this.alias()];
				}
				else
				{
					return MVCFunctions.strtolower((XVar)(data[this.alias()]));
				}
			}

			return null;
		}
	}
	public partial class ReportDateField : ReportField
	{
		protected static bool skipReportDateFieldCtor = false;
		public ReportDateField(dynamic _param_name, dynamic _param_interval, dynamic _param_alias, dynamic _param_table, dynamic _param_connection, dynamic _param_cipherer)
			:base((XVar)_param_name, (XVar)_param_interval, (XVar)_param_alias, (XVar)_param_table, (XVar)_param_connection, (XVar)_param_cipherer)
		{
			if(skipReportDateFieldCtor)
			{
				skipReportDateFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic interval = XVar.Clone(_param_interval);
			dynamic alias = XVar.Clone(_param_alias);
			dynamic table = XVar.Clone(_param_table);
			dynamic connection = XVar.Clone(_param_connection);
			dynamic cipherer = XVar.Clone(_param_cipherer);
			#endregion

		}
		public override XVar getFieldName(dynamic _param_fieldValue, dynamic _param_data = null, dynamic _param_pageObject = null)
		{
			#region default values
			if(_param_data as Object == null) _param_data = new XVar();
			if(_param_pageObject as Object == null) _param_pageObject = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic fieldValue = XVar.Clone(_param_fieldValue);
			dynamic data = XVar.Clone(_param_data);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			dynamic value = null;
			value = XVar.Clone(data[(XVar.Pack(this._recordBasedRequest) ? XVar.Pack(this._name) : XVar.Pack(this._sqlname))]);
			if((XVar)((XVar)(value == null)  || (XVar)(!(XVar)(value)))  || (XVar)(MVCFunctions.strcasecmp((XVar)(value), new XVar("null")) == 0))
			{
				return "NULL";
			}
			if(this._interval == 0)
			{
				if(XVar.Pack(this._viewFormat))
				{
					if(XVar.Pack(!(XVar)(this._recordBasedRequest)))
					{
						data.InitAndSetArrayItem(value, this._name);
					}
					return pageObject.formatReportFieldValue((XVar)(this._name), (XVar)(data));
				}
				else
				{
					dynamic date = null;
					date = XVar.Clone(MVCFunctions.db2time((XVar)(value)));
					return CommonFunctions.str_format_datetime((XVar)(date));
				}
			}
			return CommonFunctions.formatDateIntervalValue((XVar)(fieldValue), (XVar)(this._interval));
		}
		public override XVar getGroup(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if(this._interval == 0)
			{
				return data[this.alias()];
			}
			else
			{
				return DataSource.groupValueDate((XVar)(data[this.alias()]), (XVar)(this._interval));
			}

			return null;
		}
		public override XVar getKey(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if(XVar.Pack(!(XVar)(this._recordBasedRequest)))
			{
				return data[this.alias()];
			}
			else
			{
				return DataSource.groupValueDate((XVar)(data[this._name]), (XVar)(this._interval));
			}

			return null;
		}
		public override XVar overrideFormat()
		{
			return true;
		}
	}
	public partial class SQLStatement : XClass
	{
		public dynamic _fields = XVar.Array();
		public dynamic _hasDetails = XVar.Pack(true);
		public dynamic _skipCount = XVar.Pack(0);
		public dynamic _reportGlobalSummary = XVar.Pack(true);
		public dynamic _reportSummary = XVar.Pack(true);
		public dynamic _details = XVar.Pack(true);
		public dynamic _from = XVar.Pack(0);
		public dynamic _groupsTotal;
		public dynamic _limitLevel = XVar.Pack(0);
		public dynamic _hasGroups = XVar.Pack(true);
		public dynamic _recordBasedRequest = XVar.Pack(false);
		public dynamic _oldAlgorithm = XVar.Pack(false);
		public dynamic tName = XVar.Pack("");
		public dynamic shortTName = XVar.Pack("");
		public dynamic repGroupFieldsCount = XVar.Pack(0);
		public dynamic repPageSummary = XVar.Pack(0);
		public dynamic repGlobalSummary = XVar.Pack(0);
		public dynamic repLayout = XVar.Pack(0);
		public dynamic showGroupSummaryCount = XVar.Pack(0);
		public dynamic repShowDet = XVar.Pack(0);
		public dynamic repGroupFields = XVar.Array();
		public dynamic tKeyFields = XVar.Array();
		public dynamic isExistTotalFields = XVar.Pack(false);
		public dynamic fieldsArr = XVar.Array();
		public dynamic orderIndexes;
		public ProjectSettings pSet = null;
		public dynamic _connection;
		public dynamic _cipherer;
		public dynamic pageObject;
		public SQLStatement(dynamic _param_sql, dynamic _param_order, dynamic _param_groupsTotal, dynamic _param_connection, dynamic var_params, dynamic _param_cipherer, dynamic _param_pageObject)
		{
			#region pass-by-value parameters
			dynamic sql = XVar.Clone(_param_sql);
			dynamic order = XVar.Clone(_param_order);
			dynamic groupsTotal = XVar.Clone(_param_groupsTotal);
			dynamic connection = XVar.Clone(_param_connection);
			dynamic cipherer = XVar.Clone(_param_cipherer);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			dynamic fields = XVar.Array(), i = null, j = null, start = null;
			CommonFunctions.RunnerApply(this, (XVar)(var_params));
			this._connection = XVar.Clone(connection);
			this._cipherer = XVar.Clone(cipherer);
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_REPORT)));
			this.pageObject = XVar.Clone(pageObject);
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(sql)))))
			{
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			start = new XVar(0);
			fields = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.repGroupFields); i++)
			{
				j = new XVar(0);
				for(;j < MVCFunctions.count(this.fieldsArr); j++)
				{
					if(this.repGroupFields[i]["strGroupField"] == this.fieldsArr[j]["name"])
					{
						dynamic add = XVar.Array();
						add = XVar.Clone(XVar.Array());
						add.InitAndSetArrayItem(this.fieldsArr[j]["name"], "name");
						if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(this.pSet.getFieldType((XVar)(this.fieldsArr[j]["name"]))))))
						{
							add.InitAndSetArrayItem("numeric", "type");
						}
						else
						{
							if(XVar.Pack(CommonFunctions.IsCharType((XVar)(this.pSet.getFieldType((XVar)(this.fieldsArr[j]["name"]))))))
							{
								add.InitAndSetArrayItem("char", "type");
								add.InitAndSetArrayItem(GlobalVars.reportCaseSensitiveGroupFields, "case_sensitive");
							}
							else
							{
								if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(this.pSet.getFieldType((XVar)(this.fieldsArr[j]["name"]))))))
								{
									add.InitAndSetArrayItem("date", "type");
								}
								else
								{
									add.InitAndSetArrayItem("char", "type");
								}
							}
						}
						add.InitAndSetArrayItem(this.repGroupFields[i]["groupInterval"], "interval");
						add.InitAndSetArrayItem(this.fieldsArr[j]["viewFormat"], "viewformat");
						add.InitAndSetArrayItem(1, "rowsinsummary");
						if((XVar)((XVar)((XVar)(this.fieldsArr[j]["totalMax"])  || (XVar)(this.fieldsArr[j]["totalMin"]))  || (XVar)(this.fieldsArr[j]["totalAvg"]))  || (XVar)(this.fieldsArr[j]["totalSum"]))
						{
							add["rowsinsummary"]++;
						}
						if(this.repLayout == Constants.REPORT_STEPPED)
						{
							add.InitAndSetArrayItem(1, "rowsinheader");
						}
						else
						{
							if(this.repLayout == Constants.REPORT_BLOCK)
							{
								add.InitAndSetArrayItem(0, "rowsinheader");
							}
							else
							{
								if((XVar)(this.repLayout == Constants.REPORT_OUTLINE)  || (XVar)(this.repLayout == Constants.REPORT_ALIGN))
								{
									if(j == MVCFunctions.count(this.fieldsArr) - 1)
									{
										add.InitAndSetArrayItem(2, "rowsinheader");
									}
									else
									{
										add.InitAndSetArrayItem(1, "rowsinheader");
									}
								}
								else
								{
									if(this.repLayout == Constants.REPORT_TABULAR)
									{
										add.InitAndSetArrayItem(0, "rowsinheader");
									}
								}
							}
						}
						fields.InitAndSetArrayItem(add, null);
					}
				}
			}
			this._hasGroups = XVar.Clone(0 < MVCFunctions.count(fields));
			foreach (KeyValuePair<XVar, dynamic> field in fields.GetEnumerator())
			{
				dynamic f = null;
				f = XVar.Clone(CommonFunctions.create_reportfield((XVar)(field.Value["name"]), (XVar)(field.Value["type"]), (XVar)(field.Value["interval"]), new XVar("grp"), (XVar)(this.tName), (XVar)(this._connection), (XVar)(this._cipherer)));
				start = XVar.Clone(f.setStart((XVar)(start)));
				if(XVar.Pack(field.Value.KeyExists("case_sensitive")))
				{
					f.setCaseSensitive((XVar)(field.Value["case_sensitive"]));
				}
				if(XVar.Pack(field.Value.KeyExists("rowsinsummary")))
				{
					f._rowsInSummary = XVar.Clone(field.Value["rowsinsummary"]);
				}
				if(XVar.Pack(field.Value.KeyExists("rowsinheader")))
				{
					f._rowsInHeader = XVar.Clone(field.Value["rowsinheader"]);
				}
				f._viewFormat = XVar.Clone(field.Value["viewformat"]);
				this._fields.InitAndSetArrayItem(f, null);
			}
			this._reportSummary = XVar.Clone((XVar)(this.repPageSummary)  || (XVar)(this.repGlobalSummary));
			this._groupsTotal = XVar.Clone(groupsTotal);
		}
		public virtual XVar setRecordBasedRequest(dynamic _param_recordBasedRequest)
		{
			#region pass-by-value parameters
			dynamic recordBasedRequest = XVar.Clone(_param_recordBasedRequest);
			#endregion

			dynamic nCnt = null;
			this._recordBasedRequest = XVar.Clone(recordBasedRequest);
			nCnt = new XVar(0);
			for(;nCnt < MVCFunctions.count(this._fields); nCnt++)
			{
				this._fields[nCnt]._recordBasedRequest = XVar.Clone(recordBasedRequest);
			}

			return null;
		}
		public virtual XVar getGroup(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			return this._fields[0].getGroup((XVar)(data));
		}
		public virtual XVar field(dynamic _param_num)
		{
			#region pass-by-value parameters
			dynamic num = XVar.Clone(_param_num);
			#endregion

			return this._fields[num];
		}
		public virtual XVar setOldAlgorithm(dynamic _param_useOldAlgorithm = null)
		{
			#region default values
			if(_param_useOldAlgorithm as Object == null) _param_useOldAlgorithm = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic useOldAlgorithm = XVar.Clone(_param_useOldAlgorithm);
			#endregion

			dynamic nCnt = null;
			nCnt = new XVar(0);
			for(;nCnt < MVCFunctions.count(this._fields); nCnt++)
			{
				this._fields[nCnt]._oldAlgorithm = XVar.Clone(useOldAlgorithm);
			}
			this._oldAlgorithm = XVar.Clone(useOldAlgorithm);

			return null;
		}
	}
	public partial class Summarable : XClass
	{
		public dynamic _summary = XVar.Array();
		public dynamic tName = XVar.Pack("");
		public dynamic shortTName = XVar.Pack("");
		public dynamic repGroupFieldsCount = XVar.Pack(0);
		public dynamic repPageSummary = XVar.Pack(0);
		public dynamic repGlobalSummary = XVar.Pack(0);
		public dynamic repLayout = XVar.Pack(0);
		public dynamic showGroupSummaryCount = XVar.Pack(0);
		public dynamic repShowDet = XVar.Pack(0);
		public dynamic repGroupFields = XVar.Array();
		public dynamic tKeyFields = XVar.Array();
		public dynamic isExistTotalFields = XVar.Pack(false);
		public dynamic fieldsArr = XVar.Array();
		public dynamic cipherer = XVar.Pack(null);
		public dynamic _from;
		public Summarable(dynamic var_params)
		{
			CommonFunctions.RunnerApply(this, (XVar)(var_params));
			__init();
		}
		public virtual XVar init(dynamic _param_from = null)
			{return __init(_param_from);}

		private XVar __init(dynamic _param_from = null)
		{
			#region default values
			if(_param_from as Object == null) _param_from = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic from = XVar.Clone(_param_from);
			#endregion

			this._from = XVar.Clone(from);
			this.cipherer = XVar.Clone(new RunnerCipherer((XVar)(this.tName)));

			return null;
		}
		public virtual XVar writeGroup(dynamic begin, dynamic var_end, dynamic _param_gkey, dynamic _param_grp, dynamic _param_nField, dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic gkey = XVar.Clone(_param_gkey);
			dynamic grp = XVar.Clone(_param_grp);
			dynamic nField = XVar.Clone(_param_nField);
			dynamic values = XVar.Clone(_param_values);
			#endregion


			return null;
		}
		public virtual XVar addSummary(dynamic _param_recordsMode, dynamic summary, dynamic _param_data, ref dynamic nTotalRecords)
		{
			#region pass-by-value parameters
			dynamic recordsMode = XVar.Clone(_param_recordsMode);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic avg_value = null, countInGroup = null, field = XVar.Array(), fieldName = null, i = null, s = XVar.Array();
			countInGroup = XVar.Clone((XVar.Pack(summary.KeyExists("count")) ? XVar.Pack(summary["count"]) : XVar.Pack(0)));
			if(XVar.Pack(this.isExistTotalFields))
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(summary["summary"])))))
				{
					summary.InitAndSetArrayItem(XVar.Array(), "summary");
				}
				s = summary["summary"];
			}
			if(XVar.Pack(recordsMode))
			{
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.fieldsArr); i++)
				{
					field = this.fieldsArr[i];
					fieldName = XVar.Clone(field["name"]);
					if((XVar)((XVar)((XVar)(!(XVar)(field["totalMax"]))  && (XVar)(!(XVar)(field["totalMin"])))  && (XVar)(!(XVar)(field["totalAvg"])))  && (XVar)(!(XVar)(field["totalSum"])))
					{
						continue;
					}
					if(XVar.Equals(XVar.Pack(data[fieldName]), XVar.Pack(null)))
					{
						continue;
					}
					if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(s[fieldName])))))
					{
						s.InitAndSetArrayItem(XVar.Array(), fieldName);
					}
					if(XVar.Pack(!(XVar)(s[fieldName].KeyExists("count"))))
					{
						s.InitAndSetArrayItem(0, fieldName, "count");
					}
					if(XVar.Pack(field["totalMax"]))
					{
						if((XVar)(!(XVar)(s[fieldName].KeyExists("MAX")))  || (XVar)(s[fieldName]["MAX"] < data[fieldName]))
						{
							s.InitAndSetArrayItem(data[fieldName], fieldName, "MAX");
						}
					}
					if(XVar.Pack(field["totalMin"]))
					{
						if((XVar)(!(XVar)(s[fieldName].KeyExists("MIN")))  || (XVar)(data[fieldName] < s[fieldName]["MIN"]))
						{
							s.InitAndSetArrayItem(data[fieldName], fieldName, "MIN");
						}
					}
					if(XVar.Pack(field["totalAvg"]))
					{
						if(field["viewFormat"] == "Time")
						{
							avg_value = XVar.Clone(this.value2time((XVar)(data[fieldName])));
						}
						else
						{
							avg_value = XVar.Clone(data[fieldName]);
						}
						s.InitAndSetArrayItem(s[fieldName]["AVG"] * s[fieldName]["count"] + avg_value, fieldName, "AVG");
						s[fieldName]["count"]++;
						if(s[fieldName]["count"] != 0)
						{
							s.InitAndSetArrayItem(s[fieldName]["AVG"] / s[fieldName]["count"], fieldName, "AVG");
						}
					}
					if(XVar.Pack(field["totalSum"]))
					{
						if(field["viewFormat"] == "Time")
						{
							s[fieldName]["SUM"] += this.value2time((XVar)(data[fieldName]));
						}
						else
						{
							s[fieldName]["SUM"] += data[fieldName];
						}
					}
				}
				nTotalRecords++;
				countInGroup++;
			}
			else
			{
				dynamic summaryField = XVar.Array();
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.fieldsArr); i++)
				{
					field = this.fieldsArr[i];
					if((XVar)((XVar)((XVar)(!(XVar)(field["totalMax"]))  && (XVar)(!(XVar)(field["totalMin"])))  && (XVar)(!(XVar)(field["totalAvg"])))  && (XVar)(!(XVar)(field["totalSum"])))
					{
						continue;
					}
					fieldName = XVar.Clone(field["name"]);
					if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(s[fieldName])))))
					{
						s.InitAndSetArrayItem(XVar.Array(), fieldName);
					}
					summaryField = s[fieldName];
					if(XVar.Pack(field["totalMax"]))
					{
						if(!XVar.Equals(XVar.Pack(data[MVCFunctions.Concat(fieldName, "MAX")]), XVar.Pack(null)))
						{
							if((XVar)(!(XVar)(summaryField.KeyExists("MAX")))  || (XVar)(summaryField["MAX"] < data[MVCFunctions.Concat(fieldName, "MAX")]))
							{
								summaryField.InitAndSetArrayItem(data[MVCFunctions.Concat(fieldName, "MAX")], "MAX");
							}
						}
					}
					if(XVar.Pack(field["totalMin"]))
					{
						if(!XVar.Equals(XVar.Pack(data[MVCFunctions.Concat(fieldName, "MIN")]), XVar.Pack(null)))
						{
							if((XVar)(!(XVar)(summaryField.KeyExists("MIN")))  || (XVar)(data[MVCFunctions.Concat(fieldName, "MIN")] < summaryField["MIN"]))
							{
								summaryField.InitAndSetArrayItem(data[MVCFunctions.Concat(fieldName, "MIN")], "MIN");
							}
						}
					}
					if(XVar.Pack(field["totalAvg"]))
					{
						if(!XVar.Equals(XVar.Pack(data[MVCFunctions.Concat(fieldName, "AVG")]), XVar.Pack(null)))
						{
							if(field["viewFormat"] == "Time")
							{
								avg_value = XVar.Clone(this.value2time((XVar)(data[MVCFunctions.Concat(fieldName, "AVG")])));
							}
							else
							{
								avg_value = XVar.Clone(data[MVCFunctions.Concat(fieldName, "AVG")]);
							}
							summaryField.InitAndSetArrayItem(summaryField["AVG"] * summaryField["count"] + avg_value * data[MVCFunctions.Concat(fieldName, "NAVG")], "AVG");
							summaryField["count"] += data[MVCFunctions.Concat(fieldName, "NAVG")];
							if(summaryField["count"] != 0)
							{
								summaryField.InitAndSetArrayItem(summaryField["AVG"] / summaryField["count"], "AVG");
							}
						}
					}
					if(XVar.Pack(field["totalSum"]))
					{
						if(!XVar.Equals(XVar.Pack(data[MVCFunctions.Concat(fieldName, "SUM")]), XVar.Pack(null)))
						{
							if(field["viewFormat"] == "Time")
							{
								summaryField["SUM"] += this.value2time((XVar)(data[MVCFunctions.Concat(fieldName, "SUM")]));
							}
							else
							{
								summaryField["SUM"] += data[MVCFunctions.Concat(fieldName, "SUM")];
							}
						}
					}
				}
				nTotalRecords += data["countField"];
				countInGroup += data["countField"];
			}
			summary.InitAndSetArrayItem(countInGroup, "count");

			return null;
		}
		public virtual XVar _makeSummary(dynamic summary, dynamic _param_deep)
		{
			#region pass-by-value parameters
			dynamic deep = XVar.Clone(_param_deep);
			#endregion

			if(XVar.Pack(!(XVar)(summary["values"])))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> group in summary["values"].GetEnumerator())
			{
				dynamic grp = XVar.Array(), i = null;
				grp = summary["values"][group.Key];
				if(XVar.Pack(grp.KeyExists("values")))
				{
					this._makeSummary((XVar)(grp), (XVar)(deep + 1));
				}
				if((XVar)(grp.KeyExists("_begin"))  && (XVar)(grp.KeyExists("_end")))
				{
					this.writeGroup((XVar)(grp["_begin"]), (XVar)(grp["_end"]), (XVar)(group.Key), (XVar)(grp), (XVar)(deep), (XVar)(grp["_first"]));
				}
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(summary["summary"])))))
				{
					summary.InitAndSetArrayItem(XVar.Array(), "summary");
				}
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.fieldsArr); i++)
				{
					if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(summary["summary"][this.fieldsArr[i]["name"]])))))
					{
						summary.InitAndSetArrayItem(XVar.Array(), "summary", this.fieldsArr[i]["name"]);
					}
					if(XVar.Pack(MVCFunctions.is_array((XVar)(grp["summary"]))))
					{
						if(XVar.Pack(MVCFunctions.is_array((XVar)(grp["summary"][this.fieldsArr[i]["name"]]))))
						{
							if(XVar.Pack(this.fieldsArr[i]["totalMax"]))
							{
								if(XVar.Pack(grp["summary"][this.fieldsArr[i]["name"]].KeyExists("MAX")))
								{
									if((XVar)(!(XVar)(summary["summary"][this.fieldsArr[i]["name"]].KeyExists("MAX")))  || (XVar)(summary["summary"][this.fieldsArr[i]["name"]]["MAX"] < grp["summary"][this.fieldsArr[i]["name"]]["MAX"]))
									{
										summary.InitAndSetArrayItem(grp["summary"][this.fieldsArr[i]["name"]]["MAX"], "summary", this.fieldsArr[i]["name"], "MAX");
									}
								}
							}
							if(XVar.Pack(this.fieldsArr[i]["totalMin"]))
							{
								if(XVar.Pack(grp["summary"][this.fieldsArr[i]["name"]].KeyExists("MIN")))
								{
									if((XVar)(!(XVar)(summary["summary"][this.fieldsArr[i]["name"]].KeyExists("MIN")))  || (XVar)(grp["summary"][this.fieldsArr[i]["name"]]["MIN"] < summary["summary"][this.fieldsArr[i]["name"]]["MIN"]))
									{
										summary.InitAndSetArrayItem(grp["summary"][this.fieldsArr[i]["name"]]["MIN"], "summary", this.fieldsArr[i]["name"], "MIN");
									}
								}
							}
							if(XVar.Pack(this.fieldsArr[i]["totalAvg"]))
							{
								if(XVar.Pack(grp["summary"][this.fieldsArr[i]["name"]].KeyExists("AVG")))
								{
									summary.InitAndSetArrayItem(summary["summary"][this.fieldsArr[i]["name"]]["AVG"] * summary["summary"][this.fieldsArr[i]["name"]]["count"] + grp["summary"][this.fieldsArr[i]["name"]]["AVG"] * grp["summary"][this.fieldsArr[i]["name"]]["count"], "summary", this.fieldsArr[i]["name"], "AVG");
									summary["summary"][this.fieldsArr[i]["name"]]["count"] += grp["summary"][this.fieldsArr[i]["name"]]["count"];
									if(summary["summary"][this.fieldsArr[i]["name"]]["count"] != 0)
									{
										summary.InitAndSetArrayItem(summary["summary"][this.fieldsArr[i]["name"]]["AVG"] / summary["summary"][this.fieldsArr[i]["name"]]["count"], "summary", this.fieldsArr[i]["name"], "AVG");
									}
								}
							}
							if(XVar.Pack(this.fieldsArr[i]["totalSum"]))
							{
								if(XVar.Pack(grp["summary"][this.fieldsArr[i]["name"]]["SUM"]))
								{
									summary["summary"][this.fieldsArr[i]["name"]]["SUM"] += grp["summary"][this.fieldsArr[i]["name"]]["SUM"];
								}
							}
						}
					}
				}
				summary["count"] += grp["count"];
			}

			return null;
		}
		public virtual XVar value2time(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic arr = XVar.Array(), res = null;
			res = new XVar(0);
			arr = XVar.Clone(CommonFunctions.parsenumbers((XVar)(value)));
			if(XVar.Pack(arr.KeyExists(0)))
			{
				res += (arr[0] * 60) * 60;
			}
			if(XVar.Pack(arr.KeyExists(1)))
			{
				res += arr[1] * 60;
			}
			if(XVar.Pack(arr.KeyExists(2)))
			{
				res += arr[2];
			}
			return res;
		}
		public virtual XVar time2printable(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			return new XVar(0, MVCFunctions.intval((XVar)(time / (60 * 60))), 1, MVCFunctions.intval((XVar)(time / 60)), 2, time  %  60);
		}
	}
	public partial class ReportGroups : Summarable
	{
		public dynamic _report;
		public dynamic _global;
		public dynamic _totalRecords;
		public dynamic _maxpages;
		public dynamic _nGroup;
		public dynamic _oldFirst;
		public dynamic _sql;
		public dynamic _groupsTotal;
		public dynamic _connection;
		public dynamic _allGroupsUsed;
		public dynamic _countGroups;
		protected static bool skipReportGroupsCtor = false;
		public ReportGroups(dynamic sql, dynamic _param_connection, dynamic _param_groupsTotal, dynamic var_params, dynamic _param_report)
			:base((XVar)var_params)
		{
			if(skipReportGroupsCtor)
			{
				skipReportGroupsCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic connection = XVar.Clone(_param_connection);
			dynamic groupsTotal = XVar.Clone(_param_groupsTotal);
			dynamic report = XVar.Clone(_param_report);
			#endregion

			this.init();
			this._groupsTotal = XVar.Clone(groupsTotal);
			this._sql = sql;
			this._connection = XVar.Clone(connection);
			this._report = XVar.Clone(report);
		}
		public override XVar init(dynamic _param_from = null)
		{
			#region default values
			if(_param_from as Object == null) _param_from = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic from = XVar.Clone(_param_from);
			#endregion

			base.init((XVar)(from));
			this._global = XVar.Clone(XVar.Array());
			this._totalRecords = new XVar(0);
			this._maxpages = XVar.Clone(-1);
			this._from = XVar.Clone(from);
			this._nGroup = XVar.Clone(-1);
			this._oldFirst = new XVar("");
			this._allGroupsUsed = new XVar(false);
			this._countGroups = new XVar(0);

			return null;
		}
		public virtual XVar setGlobalSummary(dynamic _param_recordsMode, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic recordsMode = XVar.Clone(_param_recordsMode);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			this.addSummary((XVar)(recordsMode), (XVar)(this._global), (XVar)(data), ref this._totalRecords);

			return null;
		}
		public virtual XVar setGroup(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic field = null, firstKey = null;
			field = XVar.Clone(this._sql.field(new XVar(0)));
			firstKey = XVar.Clone(field.getKey((XVar)(data)));
			if(!XVar.Equals(XVar.Pack(firstKey), XVar.Pack(this._oldFirst)))
			{
				this._nGroup++;
				this._oldFirst = XVar.Clone(firstKey);
			}

			return null;
		}
		public virtual XVar isVisibleGroup()
		{
			return (XVar)(this._from <= this._nGroup)  && (XVar)(this._nGroup < this._from + this._groupsTotal);
		}
		public virtual XVar getDisplayGroups(dynamic _param_from)
		{
			#region pass-by-value parameters
			dynamic from = XVar.Clone(_param_from);
			#endregion

			this.init((XVar)(from));
			if(XVar.Pack(!(XVar)(this._groupsTotal)))
			{
				return XVar.Array();
			}
			else
			{
				dynamic groups = XVar.Array();
				groups = XVar.Clone(XVar.Array());
				this._allGroupsUsed = new XVar(false);
				if(XVar.Pack(this.repGroupFieldsCount))
				{
					dynamic data = XVar.Array(), dc = null, qResult = null;
					dc = XVar.Clone(this._report.pageObject.getSubsetDataCommand());
					dc.totals = XVar.Clone(new XVar(0, new XVar("alias", "grp0", "field", this.repGroupFields[0]["strGroupField"], "modifier", this.repGroupFields[0]["groupInterval"], "direction", this._report.groupOrderDirection(new XVar(0)))));
					dc.startRecord = XVar.Clone(this._from);
					dc.reccount = XVar.Clone(this._groupsTotal);
					qResult = XVar.Clone(this._report.pageObject.getDataSource().getTotals((XVar)(dc)));
					while((XVar)(data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(qResult.fetchAssoc()))))  && (XVar)(MVCFunctions.count(groups) < this._groupsTotal))
					{
						groups.InitAndSetArrayItem(data["grp0"], null);
					}
					if(MVCFunctions.count(groups) < this._groupsTotal)
					{
						this._allGroupsUsed = new XVar(true);
					}
				}
				if(XVar.Pack(0) < from)
				{
					this._allGroupsUsed = new XVar(false);
				}
				this._countGroups = XVar.Clone(MVCFunctions.count(groups));
				return groups;
			}

			return null;
		}
		public virtual XVar getCountGroups(dynamic _param_fullRequest = null)
		{
			#region default values
			if(_param_fullRequest as Object == null) _param_fullRequest = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic fullRequest = XVar.Clone(_param_fullRequest);
			#endregion

			if(XVar.Pack(this.repGroupFieldsCount))
			{
				if((XVar)(0 <= this._nGroup)  && (XVar)(fullRequest))
				{
					return this._nGroup + 1;
				}
				else
				{
					if(XVar.Pack(this._allGroupsUsed))
					{
						return this._countGroups;
					}
					else
					{
						dynamic dc = null;
						dc = XVar.Clone(this._report.pageObject.getSubsetDataCommand());
						dc.totals = XVar.Clone(new XVar(0, new XVar("alias", "grp0", "field", this.repGroupFields[0]["strGroupField"], "modifier", this.repGroupFields[0]["groupInterval"])));
						return this._report.pageObject.getDataSource().getTotalCount((XVar)(dc));
					}
				}
			}
			else
			{
				return 0;
			}

			return null;
		}
		public virtual XVar getSummary()
		{
			return this._global;
		}
		public virtual XVar allGroupsUsed()
		{
			return this._allGroupsUsed;
		}
	}
	public partial class ReportLogic : Summarable
	{
		public dynamic _list;
		public dynamic _totalRecords;
		public dynamic _pages;
		public dynamic _groupsTotal;
		public dynamic _groupsPerPage;
		public dynamic _groupCounter = XVar.Pack(0);
		public dynamic _connection;
		public dynamic _sql;
		public dynamic _groups;
		public dynamic _groupKeys;
		public dynamic _fullRequest = XVar.Pack(false);
		public dynamic _recordBasedRequest = XVar.Pack(false);
		public dynamic _doPaging = XVar.Pack(false);
		public dynamic _lastPageNumber = XVar.Pack(0);
		public dynamic _pageSummary;
		public dynamic _printRecordCount = XVar.Pack(0);
		public dynamic _listedRows = XVar.Pack(0);
		public dynamic _oldLevels;
		public dynamic pageObject = XVar.Pack(null);
		public ProjectSettings pSet = null;
		protected static bool skipReportLogicCtor = false;
		public ReportLogic(dynamic _param_order, dynamic _param_connection, dynamic _param_groupsTotal, dynamic _param_groupsPerPage, dynamic var_params, dynamic _param_pageObject = null)
			:base((XVar)var_params)
		{
			if(skipReportLogicCtor)
			{
				skipReportLogicCtor = false;
				return;
			}
			#region default values
			if(_param_pageObject as Object == null) _param_pageObject = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic order = XVar.Clone(_param_order);
			dynamic connection = XVar.Clone(_param_connection);
			dynamic groupsTotal = XVar.Clone(_param_groupsTotal);
			dynamic groupsPerPage = XVar.Clone(_param_groupsPerPage);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			this._connection = XVar.Clone(connection);
			this.cipherer = XVar.Clone(new RunnerCipherer((XVar)(this.tName)));
			this._sql = XVar.Clone(new SQLStatement((XVar)(XVar.Array()), (XVar)(order), (XVar)(groupsTotal), (XVar)(connection), (XVar)(var_params), (XVar)(this.cipherer), (XVar)(pageObject)));
			this._groups = XVar.Clone(new ReportGroups((XVar)(this._sql), (XVar)(connection), (XVar)(groupsTotal), (XVar)(var_params), this));
			this._groupsTotal = XVar.Clone(groupsTotal);
			this._groupsPerPage = XVar.Clone(groupsPerPage);
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_REPORT)));
			if(XVar.Pack(XVar.Equals(XVar.Pack(pageObject), XVar.Pack(null))))
			{
				this.pageObject = XVar.Clone(new ViewControlsContainer((XVar)(this.pSet), new XVar(Constants.PAGE_REPORT)));
			}
			else
			{
				this.pageObject = XVar.Clone(pageObject);
			}
			this.init();
		}
		public override XVar init(dynamic _param_from = null)
		{
			#region default values
			if(_param_from as Object == null) _param_from = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic from = XVar.Clone(_param_from);
			#endregion

			base.init((XVar)(from));
			this._sql._from = XVar.Clone(from);
			this._list = XVar.Clone(XVar.Array());
			this._totalRecords = new XVar(0);
			this._pages = XVar.Clone(XVar.Array());
			this._groupKeys = XVar.Clone(XVar.Array());
			this._lastPageNumber = new XVar(0);
			this._pageSummary = XVar.Clone(XVar.Array());
			this._listedRows = new XVar(0);
			this._oldLevels = XVar.Clone(XVar.Array());
			this.cipherer = XVar.Clone(new RunnerCipherer((XVar)(this.tName)));

			return null;
		}
		public virtual XVar getPages()
		{
			return this._pages;
		}
		public virtual XVar getFormattedRow(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion


			return null;
		}
		public override XVar writeGroup(dynamic begin, dynamic var_end, dynamic _param_gkey, dynamic _param_grp, dynamic _param_nField, dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic gkey = XVar.Clone(_param_gkey);
			dynamic grp = XVar.Clone(_param_grp);
			dynamic nField = XVar.Clone(_param_nField);
			dynamic values = XVar.Clone(_param_values);
			#endregion


			return null;
		}
		public virtual XVar _writePage(dynamic page, dynamic _param_src, dynamic _param_count)
		{
			#region pass-by-value parameters
			dynamic src = XVar.Clone(_param_src);
			dynamic count = XVar.Clone(_param_count);
			#endregion


			return null;
		}
		public virtual XVar writeGlobalSummary(dynamic _param_source)
		{
			#region pass-by-value parameters
			dynamic source = XVar.Clone(_param_source);
			#endregion


			return null;
		}
		public virtual XVar writePageSummary()
		{
			dynamic page = XVar.Array(), result = null;
			if(XVar.Pack(this._doPaging))
			{
				dynamic nCnt = null;
				nCnt = new XVar(0);
				for(;nCnt < MVCFunctions.count(this._list); nCnt++)
				{
					if(XVar.Pack(!(XVar)(this._pages.KeyExists(nCnt))))
					{
						this._pages.InitAndSetArrayItem(XVar.Array(), nCnt);
					}
					result = this._pages[nCnt];
					if(XVar.Pack(this._pageSummary.KeyExists(nCnt)))
					{
						page = XVar.Clone(this._pageSummary[nCnt]);
						this._writePage((XVar)(result), (XVar)((XVar.Pack(page.KeyExists("summary")) ? XVar.Pack(page["summary"]) : XVar.Pack(XVar.Array()))), (XVar)((XVar.Pack(page.KeyExists("count")) ? XVar.Pack(page["count"]) : XVar.Pack(0))));
					}
					else
					{
						this._writePage((XVar)(result), (XVar)(XVar.Array()), new XVar(0));
					}
				}
			}
			else
			{
				result = XVar.Clone(XVar.Array());
				page = XVar.Clone(this._summary);
				this._writePage((XVar)(result), (XVar)((XVar.Pack(page.KeyExists("summary")) ? XVar.Pack(page["summary"]) : XVar.Pack(XVar.Array()))), (XVar)((XVar.Pack(page.KeyExists("count")) ? XVar.Pack(page["count"]) : XVar.Pack(0))));
				this._summary = XVar.Clone(result);
			}
			if((XVar)(0 == MVCFunctions.count(this._pages))  && (XVar)(0 < MVCFunctions.count(this._list)))
			{
				this._pages.InitAndSetArrayItem(this._summary, null);
			}

			return null;
		}
		public virtual XVar makeSummary()
		{
			this._makeSummary((XVar)(this._summary), new XVar(0));

			return null;
		}
		public virtual XVar setSummary(dynamic _param_recordsMode, dynamic _param_data, dynamic _param_rowToAppend = null)
		{
			#region default values
			if(_param_rowToAppend as Object == null) _param_rowToAppend = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic recordsMode = XVar.Clone(_param_recordsMode);
			dynamic data = XVar.Clone(_param_data);
			dynamic rowToAppend = XVar.Clone(_param_rowToAppend);
			#endregion

			dynamic level = XVar.Array(), levels = XVar.Array(), setBegin = null;
			level = this._summary;
			setBegin = new XVar(false);
			if(XVar.Pack(this.repGroupFieldsCount))
			{
				dynamic field = null, groupIndex = null, groupKey = null, i = null, recordkeys = XVar.Array();
				recordkeys = XVar.Clone(XVar.Array());
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.repGroupFields); i++)
				{
					groupIndex = XVar.Clone(this.repGroupFields[i]["groupOrder"] - 1);
					field = XVar.Clone(this._sql.field((XVar)(groupIndex)));
					recordkeys.InitAndSetArrayItem(field.getKey((XVar)(data)), groupIndex);
				}
				if(0 < MVCFunctions.count(this._groupKeys))
				{
					dynamic changed = null, nKey = null;
					changed = new XVar(false);
					nKey = new XVar(0);
					for(;nKey < MVCFunctions.count(recordkeys); nKey++)
					{
						if(recordkeys[nKey] != this._groupKeys[nKey])
						{
							changed = new XVar(true);
							break;
						}
					}
					if(XVar.Pack(changed))
					{
						dynamic emptyRow = null, nKey2 = null;
						nKey2 = XVar.Clone(MVCFunctions.count(recordkeys) - 1);
						for(;nKey <= nKey2; nKey2--)
						{
							emptyRow = this.appendRow((XVar)(XVar.Array()));
							field = XVar.Clone(this._sql.field((XVar)(nKey2)));
							this._printRecordCount += field._rowsInSummary;
							this._listedRows++;
							this._oldLevels.InitAndSetArrayItem(emptyRow, nKey2, "_end");
						}
					}
					if(nKey == XVar.Pack(0))
					{
						++(this._groupCounter);
					}
				}
				this._groupKeys = XVar.Clone(recordkeys);
				levels = XVar.Clone(XVar.Array());
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.repGroupFields); i++)
				{
					groupIndex = XVar.Clone(this.repGroupFields[i]["groupOrder"] - 1);
					groupKey = XVar.Clone(recordkeys[groupIndex]);
					if(XVar.Pack(!(XVar)(level.KeyExists("values"))))
					{
						level.InitAndSetArrayItem(XVar.Array(), "values");
					}
					if(XVar.Pack(!(XVar)(level["values"].KeyExists(groupKey))))
					{
						dynamic newArray = null;
						newArray = XVar.Clone(XVar.Array());
						level.InitAndSetArrayItem(newArray, "values", groupKey);
						level = level["values"][groupKey];
						field = XVar.Clone(this._sql.field((XVar)(groupIndex)));
						this._printRecordCount += field._rowsInHeader;
						setBegin = new XVar(true);
						level.InitAndSetArrayItem(data, "_first");
					}
					else
					{
						level = level["values"][groupKey];
					}
					levels.InitAndSetArrayItem(level, null);
				}
				this.addSummary((XVar)(recordsMode), (XVar)(level), (XVar)(data), ref this._totalRecords);
				this._oldLevels = levels;
			}
			else
			{
				this.addSummary((XVar)(recordsMode), (XVar)(level), (XVar)(data), ref this._totalRecords);
				++(this._groupCounter);
			}
			if(XVar.Pack(rowToAppend))
			{
				dynamic added = null;
				added = this.appendRow((XVar)(rowToAppend));
				this._printRecordCount++;
				this._listedRows++;
				if((XVar)(setBegin)  && (XVar)(this.repGroupFieldsCount))
				{
					dynamic nCnt = null;
					nCnt = new XVar(0);
					for(;nCnt < MVCFunctions.count(levels); nCnt++)
					{
						if(XVar.Pack(!(XVar)(levels[nCnt].KeyExists("_begin"))))
						{
							levels.InitAndSetArrayItem(added, nCnt, "_begin");
						}
					}
				}
			}
			if(XVar.Pack(this.repPageSummary))
			{
				if((XVar)(this._doPaging)  && (XVar)(rowToAppend))
				{
					dynamic nPage = null, summaryCount = null;
					nPage = XVar.Clone(MVCFunctions.count(this._list) - 1);
					if(XVar.Pack(!(XVar)(this._pageSummary.KeyExists(nPage))))
					{
						this._pageSummary.InitAndSetArrayItem(0, nPage, "count");
					}
					summaryCount = XVar.Clone(this._pageSummary[nPage]["count"]);
					this.addSummary((XVar)(recordsMode), (XVar)(this._pageSummary[nPage]), (XVar)(data), ref summaryCount);
					this._pageSummary.InitAndSetArrayItem(summaryCount, nPage, "count");
				}
			}

			return null;
		}
		public virtual XVar setFinish()
		{
			if(0 < MVCFunctions.count(this._groupKeys))
			{
				dynamic emptyRow = null, field = null, nKey = null;
				nKey = XVar.Clone(MVCFunctions.count(this._groupKeys) - 1);
				for(;XVar.Pack(0) <= nKey; nKey--)
				{
					field = XVar.Clone(this._sql.field((XVar)(nKey)));
					this._printRecordCount += field._rowsInSummary;
					emptyRow = this.appendRow((XVar)(XVar.Array()));
					this._listedRows++;
					this._oldLevels.InitAndSetArrayItem(emptyRow, nKey, "_end");
				}
			}

			return null;
		}
		public virtual XVar appendRow(dynamic _param_row)
		{
			#region pass-by-value parameters
			dynamic row = XVar.Clone(_param_row);
			#endregion

			if(XVar.Pack(this._groupsPerPage))
			{
				dynamic page = null;
				if(XVar.Pack(!(XVar)(this.repGroupFieldsCount)))
				{
					page = XVar.Clone(MVCFunctions.intval((XVar)((this._groupCounter - 1) / this._groupsPerPage)));
				}
				else
				{
					page = XVar.Clone(MVCFunctions.intval((XVar)(this._groupCounter / this._groupsPerPage)));
				}
				if((XVar)(XVar.Pack(0) < page)  && (XVar)(!(XVar)(this._list.KeyExists(page - 1))))
				{
					MVCFunctions.ob_flush();
					HttpContext.Current.Response.End();
					throw new RunnerInlineOutputException();
				}
				this._list.InitAndSetArrayItem(row, page, null);
				return this._list[page][MVCFunctions.count(this._list[page]) - 1];
			}
			else
			{
				this._list.InitAndSetArrayItem(row, null);
				return this._list[MVCFunctions.count(this._list) - 1];
			}

			return null;
		}
		public virtual XVar recordVisible(dynamic _param_nRecord)
		{
			#region pass-by-value parameters
			dynamic nRecord = XVar.Clone(_param_nRecord);
			#endregion

			return (XVar)((XVar)((XVar)(this._sql._limitLevel == 1)  || (XVar)(this._groupsTotal == 0))  || (XVar)((XVar)(this._sql._limitLevel == 2)  && (XVar)((XVar)(0 <= nRecord - this._sql._skipCount)  && (XVar)(nRecord - this._sql._skipCount < this._groupsTotal))))  || (XVar)((XVar)(this._sql._limitLevel == 0)  && (XVar)((XVar)(0 <= nRecord - this._from)  && (XVar)(nRecord - this._sql._skipCount < this._from + this._groupsTotal)));
		}
		public virtual XVar getTotals()
		{
			if(XVar.Pack(this._fullRequest))
			{
				return this._groups.getSummary();
			}
			else
			{
				if(XVar.Pack(this._groups.allGroupsUsed()))
				{
					return this._summary;
				}
				else
				{
					dynamic data = XVar.Array(), dc = null, fetchedArray = null, rs = null, totalRecords = null, totals = null;
					totals = XVar.Clone(XVar.Array());
					dc = XVar.Clone(this.getTotalCommand(new XVar(false)));
					rs = XVar.Clone(this.pageObject.getDataSource().getTotals((XVar)(dc)));
					fetchedArray = XVar.Clone(rs.fetchAssoc());
					data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
					totalRecords = new XVar(0);
					data.InitAndSetArrayItem(this.pageObject.limitRowCount((XVar)(data["countField"])), "countField");
					this.addSummary(new XVar(false), (XVar)(totals), (XVar)(data), ref totalRecords);
					return totals;
				}
			}

			return null;
		}
		public virtual XVar getDataCommand(dynamic _param_from)
		{
			#region pass-by-value parameters
			dynamic from = XVar.Clone(_param_from);
			#endregion

			dynamic dc = null, orderFieldIndices = XVar.Array(), orderIndices = XVar.Array();
			orderIndices = XVar.Clone(this.pSet.getOrderIndexes());
			dc = XVar.Clone(this.pageObject.getSubsetDataCommand());
			orderFieldIndices = XVar.Clone(XVar.Array());
			dc.order = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> grp in this.repGroupFields.GetEnumerator())
			{
				dynamic alias = null, gField = null;
				gField = XVar.Clone(grp.Value["strGroupField"]);
				if(XVar.Pack(!(XVar)(grp.Value["groupInterval"])))
				{
					orderFieldIndices.InitAndSetArrayItem(true, this.pSet.getFieldIndex((XVar)(gField)));
				}
				alias = XVar.Clone(MVCFunctions.Concat("grp", grp.Key));
				dc.extraColumns.InitAndSetArrayItem(new DsFieldData(new XVar(""), (XVar)(alias), (XVar)(gField), (XVar)(grp.Value["groupInterval"])), null);
				dc.order.InitAndSetArrayItem(new XVar("column", alias, "dir", this.Invoke("groupOrderDirection", (XVar)(grp.Key))), null);
			}
			foreach (KeyValuePair<XVar, dynamic> o in orderIndices.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(orderFieldIndices[o.Value[0]])))
				{
					dc.order.InitAndSetArrayItem(new XVar("index", o.Value[0], "dir", o.Value[1]), null);
				}
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.count(this.repGroupFields))))
			{
				dc.startRecord = XVar.Clone(from);
				dc.reccount = XVar.Clone((XVar.Pack(this._groupsTotal) ? XVar.Pack(this._groupsTotal) : XVar.Pack(-1)));
			}
			return dc;
		}
		public virtual XVar getTotalCommand(dynamic _param_addGroups)
		{
			#region pass-by-value parameters
			dynamic addGroups = XVar.Clone(_param_addGroups);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(this.pageObject.getSubsetDataCommand());
			if(XVar.Pack(addGroups))
			{
				foreach (KeyValuePair<XVar, dynamic> grp in this.repGroupFields.GetEnumerator())
				{
					dynamic alias = null, gField = null;
					gField = XVar.Clone(grp.Value["strGroupField"]);
					alias = XVar.Clone(MVCFunctions.Concat("grp", grp.Key));
					dc.totals.InitAndSetArrayItem(new XVar("alias", alias, "field", gField, "modifier", grp.Value["groupInterval"], "direction", this.Invoke("groupOrderDirection", (XVar)(grp.Key))), null);
				}
			}
			dc.totals.InitAndSetArrayItem(new XVar("alias", "countField", "total", "count"), null);
			foreach (KeyValuePair<XVar, dynamic> f in this.fieldsArr.GetEnumerator())
			{
				dynamic field = null;
				field = f.Value["name"];
				if(XVar.Pack(f.Value["totalMax"]))
				{
					dc.totals.InitAndSetArrayItem(new XVar("alias", MVCFunctions.Concat(field, "MAX"), "field", field, "total", "max"), null);
				}
				if(XVar.Pack(f.Value["totalMin"]))
				{
					dc.totals.InitAndSetArrayItem(new XVar("alias", MVCFunctions.Concat(field, "MIN"), "field", field, "total", "min"), null);
				}
				if((XVar)(f.Value["totalAvg"])  && (XVar)(!(XVar)(CommonFunctions.IsDateFieldType((XVar)(this.pSet.getFieldType((XVar)(field)))))))
				{
					dc.totals.InitAndSetArrayItem(new XVar("alias", MVCFunctions.Concat(field, "AVG"), "field", field, "total", "avg"), null);
					dc.totals.InitAndSetArrayItem(new XVar("alias", MVCFunctions.Concat(field, "NAVG"), "field", field, "total", "count"), null);
				}
				if((XVar)(f.Value["totalSum"])  && (XVar)(!(XVar)(CommonFunctions.IsDateFieldType((XVar)(this.pSet.getFieldType((XVar)(field)))))))
				{
					dc.totals.InitAndSetArrayItem(new XVar("alias", MVCFunctions.Concat(field, "SUM"), "field", field, "total", "sum"), null);
				}
			}
			return dc;
		}
		public virtual XVar getGroupFilter(dynamic groups)
		{
			dynamic condition1 = null, condition2 = null, firstGroup = null, gField = null, lastGroup = null, modifier = null;
			gField = XVar.Clone(this.repGroupFields[0]["strGroupField"]);
			modifier = XVar.Clone(this.repGroupFields[0]["groupInterval"]);
			firstGroup = XVar.Clone(groups[0]);
			lastGroup = XVar.Clone(groups[MVCFunctions.count(groups) - 1]);
			if(this.Invoke("groupOrderDirection", new XVar(0)) == "DESC")
			{
				dynamic t = null;
				t = XVar.Clone(firstGroup);
				firstGroup = XVar.Clone(lastGroup);
				lastGroup = XVar.Clone(t);
			}
			condition1 = XVar.Clone(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(gField), new XVar(Constants.dsopLESS), (XVar)(firstGroup), new XVar(Constants.dsCASE_DEFAULT), (XVar)(modifier)))));
			condition2 = XVar.Clone(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(gField), new XVar(Constants.dsopMORE), (XVar)(lastGroup), new XVar(Constants.dsCASE_DEFAULT), (XVar)(modifier)))));
			return DataCondition._And((XVar)(new XVar(0, condition1, 1, condition2)));
		}
		public virtual XVar getReport(dynamic _param_from = null)
		{
			#region default values
			if(_param_from as Object == null) _param_from = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic from = XVar.Clone(_param_from);
			#endregion

			dynamic countGroups = null, countrows = null, data = null, dc = null, entType = null, global_totals = XVar.Array(), globals = null, i = null, isExistTimeFormatField = null, maxpages = null, nRow = null, nRowVisible = null, page = null, qResult = null, returnthis = null;
			this.init((XVar)(from));
			this._doPaging = XVar.Clone(this._groupsPerPage != 0);
			if(XVar.Pack(this.pageObject))
			{
				if(XVar.Pack(this.pageObject.pdfJsonMode()))
				{
					this._doPaging = new XVar(false);
				}
			}
			isExistTimeFormatField = new XVar(false);
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.fieldsArr); i++)
			{
				if(this.fieldsArr[i]["viewFormat"] == "Time")
				{
					isExistTimeFormatField = new XVar(true);
					break;
				}
			}
			this._fullRequest = XVar.Clone((XVar)(this.repGlobalSummary)  && (XVar)(isExistTimeFormatField));
			if((XVar)((XVar)((XVar)((XVar)(this._connection.dbType != Constants.nDATABASE_MySQL)  && (XVar)(this._connection.dbType != Constants.nDATABASE_PostgreSQL))  && (XVar)(this._connection.dbType != Constants.nDATABASE_MSSQLServer))  && (XVar)(this._connection.dbType != Constants.nDATABASE_Oracle))  && (XVar)(this._connection.dbType != Constants.nDATABASE_Access))
			{
				this._fullRequest = new XVar(true);
			}
			entType = XVar.Clone(this.pSet.getEntityType());
			if((XVar)(XVar.Equals(XVar.Pack(entType), XVar.Pack(Constants.titREST_REPORT)))  || (XVar)(XVar.Equals(XVar.Pack(entType), XVar.Pack(Constants.titSQL_REPORT))))
			{
				this._fullRequest = new XVar(true);
			}
			this._recordBasedRequest = XVar.Clone(this._fullRequest);
			if(XVar.Pack(!(XVar)(this.repGroupFieldsCount)))
			{
				this._recordBasedRequest = new XVar(true);
			}
			this._sql.setRecordBasedRequest((XVar)(this._recordBasedRequest));
			if(XVar.Pack(this._fullRequest))
			{
				this._sql._limitLevel = new XVar(0);
			}
			else
			{
				if(XVar.Pack(!(XVar)(this.repGroupFieldsCount)))
				{
					this._sql._limitLevel = new XVar(2);
				}
				else
				{
					this._sql._limitLevel = new XVar(1);
				}
			}
			page = XVar.Clone(-1);
			nRow = new XVar(0);
			nRowVisible = new XVar(0);
			if(XVar.Pack(!(XVar)(this._recordBasedRequest)))
			{
				dynamic groups = null;
				groups = XVar.Clone(this._groups.getDisplayGroups((XVar)(from)));
				dc = XVar.Clone((XVar.Pack(this.repShowDet) ? XVar.Pack(this.getDataCommand((XVar)(from))) : XVar.Pack(this.getTotalCommand(new XVar(true)))));
				if(XVar.Pack(MVCFunctions.count(groups)))
				{
					dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, this.getGroupFilter((XVar)(groups))))));
				}
				qResult = XVar.Clone((XVar.Pack(this.repShowDet) ? XVar.Pack(this.pageObject.getDataSource().getList((XVar)(dc))) : XVar.Pack(this.pageObject.getDataSource().getTotals((XVar)(dc)))));
				if(XVar.Pack(!(XVar)(qResult)))
				{
					MVCFunctions.showError((XVar)(this.pageObject.getDataSource().lastError()));
				}
				while(XVar.Pack(data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(qResult.fetchAssoc())))))
				{
					this.pageObject.recId = XVar.Clone(nRow);
					this.setSummary((XVar)(this.repShowDet), (XVar)(data), (XVar)((XVar.Pack(this.recordVisible((XVar)(nRow))) ? XVar.Pack(this.getFormattedRow((XVar)(data))) : XVar.Pack(null))));
					nRow++;
				}
			}
			else
			{
				dynamic visible = null;
				this._groups.init((XVar)(from));
				dc = XVar.Clone(this.getDataCommand((XVar)(from)));
				qResult = XVar.Clone(this.pageObject.getDataSource().getList((XVar)(dc)));
				if(XVar.Pack(!(XVar)(qResult)))
				{
					MVCFunctions.showError((XVar)(this.pageObject.getDataSource().lastError()));
				}
				while(XVar.Pack(data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(qResult.fetchAssoc())))))
				{
					if(XVar.Pack(this.repGroupFieldsCount))
					{
						this._groups.setGroup((XVar)(data));
					}
					if(XVar.Pack(this._fullRequest))
					{
						this._groups.setGlobalSummary(new XVar(true), (XVar)(data));
					}
					if(XVar.Pack(this.repGroupFieldsCount))
					{
						visible = XVar.Clone((XVar)(this._groups.isVisibleGroup())  || (XVar)(this._groupsTotal == 0));
					}
					else
					{
						visible = XVar.Clone(this.recordVisible((XVar)(nRow)));
					}
					if(XVar.Pack(visible))
					{
						nRowVisible++;
						this.pageObject.recId = XVar.Clone(nRow);
						this.setSummary(new XVar(true), (XVar)(data), (XVar)(this.getFormattedRow((XVar)(data))));
					}
					else
					{
						if((XVar)(!(XVar)(this._fullRequest))  && (XVar)(0 < MVCFunctions.count(this._list)))
						{
							break;
						}
					}
					nRow++;
					if((XVar)((XVar)(!(XVar)(this.repGroupFieldsCount))  && (XVar)(this.pSet.getRecordsLimit()))  && (XVar)(this.pSet.getRecordsLimit() <= from + nRowVisible))
					{
						break;
					}
				}
				this._sql.setOldAlgorithm(new XVar(false));
			}
			this.setFinish();
			this.makeSummary();
			global_totals = XVar.Clone(this.getTotals());
			this.writePageSummary();
			globals = XVar.Clone(this.writeGlobalSummary((XVar)(global_totals)));
			if(XVar.Pack(this.repGroupFieldsCount))
			{
				countrows = XVar.Clone(this._groups.getCountGroups((XVar)(this._fullRequest)));
				countGroups = XVar.Clone(countrows);
			}
			else
			{
				countrows = XVar.Clone(global_totals["count"]);
				countGroups = new XVar(1);
			}
			maxpages = new XVar(1);
			if(0 < this._groupsTotal)
			{
				maxpages = XVar.Clone((XVar)Math.Ceiling((double)(countrows / this._groupsTotal)));
			}
			returnthis = XVar.Clone(new XVar("list", this._list, "global", globals, "page", this._summary, "maxpages", maxpages, "countRows", countrows, "countGroups", countGroups));
			return returnthis;
		}
	}
	public partial class Report : ReportLogic
	{
		public dynamic forExport = XVar.Pack(false);
		public dynamic mode = XVar.Pack(Constants.MODE_LIST);
		protected static bool skipReportCtor = false;
		public Report(dynamic _param_order, dynamic _param_connection, dynamic _param_groupsTotal, dynamic _param_groupsPerPage, dynamic var_params, dynamic _param_pageObject = null)
			:base((XVar)_param_order, (XVar)_param_connection, (XVar)_param_groupsTotal, (XVar)_param_groupsPerPage, (XVar)var_params, (XVar)_param_pageObject)
		{
			if(skipReportCtor)
			{
				skipReportCtor = false;
				return;
			}
			#region default values
			if(_param_pageObject as Object == null) _param_pageObject = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic order = XVar.Clone(_param_order);
			dynamic connection = XVar.Clone(_param_connection);
			dynamic groupsTotal = XVar.Clone(_param_groupsTotal);
			dynamic groupsPerPage = XVar.Clone(_param_groupsPerPage);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

		}
		public virtual XVar groupOrderDirection(dynamic _param_grIdx)
		{
			#region pass-by-value parameters
			dynamic grIdx = XVar.Clone(_param_grIdx);
			#endregion

			dynamic gField = null, grFieldIdx = null, orderIndices = XVar.Array();
			orderIndices = this.pSet.getOrderIndexes();
			gField = XVar.Clone(this.repGroupFields[grIdx]["strGroupField"]);
			grFieldIdx = XVar.Clone(this.pSet.getFieldIndex((XVar)(gField)));
			foreach (KeyValuePair<XVar, dynamic> o in orderIndices.GetEnumerator())
			{
				if(o.Value[0] == grFieldIdx)
				{
					return o.Value[1];
				}
			}
			return "ASC";
		}
		public override XVar getFormattedRow(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic i = null, keylink = null, row = XVar.Array();
			row = XVar.Clone(new XVar("row_data", true));
			keylink = new XVar("");
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.tKeyFields); i++)
			{
				keylink = MVCFunctions.Concat(keylink, "&key", i + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(value[this.tKeyFields[i]])))));
			}
			if(XVar.Pack(this.forExport))
			{
				this.pageObject.setForExportVar((XVar)(this.forExport));
			}
			if(XVar.Pack(this.pageObject.pdfJsonMode()))
			{
				dynamic groupField = XVar.Array(), j = null;
				j = new XVar(0);
				for(;j < MVCFunctions.count(this.repGroupFields); j++)
				{
					groupField = this.repGroupFields[j];
					row.InitAndSetArrayItem("''", MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(groupField["strGroupField"])), "_grval"));
				}
			}
			if(XVar.Pack(this.repShowDet))
			{
				dynamic fieldData = XVar.Array();
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.fieldsArr); i++)
				{
					fieldData = this.fieldsArr[i];
					if(XVar.Pack(!(XVar)(fieldData["repPage"])))
					{
						continue;
					}
					row.InitAndSetArrayItem(this.pageObject.formatReportFieldValue((XVar)(fieldData["name"]), (XVar)(value), (XVar)(keylink)), MVCFunctions.Concat(fieldData["goodName"], "_value"));
					row.InitAndSetArrayItem(value[fieldData["name"]], MVCFunctions.Concat(fieldData["goodName"], "_dbvalue"));
				}
			}
			if(this.repLayout == Constants.REPORT_BLOCK)
			{
				row.InitAndSetArrayItem(true, MVCFunctions.GoodFieldName(new XVar("nonewgroup")));
			}
			return row;
		}
		public override XVar writeGroup(dynamic begin, dynamic var_end, dynamic _param_gkey, dynamic _param_grp, dynamic _param_nField, dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic gkey = XVar.Clone(_param_gkey);
			dynamic grp = XVar.Clone(_param_grp);
			dynamic nField = XVar.Clone(_param_nField);
			dynamic values = XVar.Clone(_param_values);
			#endregion

			dynamic field = null, gname = null, i = null;
			field = XVar.Clone(this._sql.field((XVar)(nField)));
			gname = XVar.Clone(field.name());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.repGroupFields); i++)
			{
				if(gname == this.repGroupFields[i]["strGroupField"])
				{
					dynamic j = null;
					if(this.repLayout == Constants.REPORT_BLOCK)
					{
						dynamic bFound = null, gname2 = null, nG = null;
						bFound = new XVar(false);
						nG = new XVar(0);
						for(;nG < this.repGroupFieldsCount; nG++)
						{
							field = XVar.Clone(this._sql.field((XVar)(nG)));
							gname2 = XVar.Clone(field.name());
							if(nG < nField)
							{
								if(XVar.Pack(begin.KeyExists(MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(gname2, "_firstnewgroup"))))))
								{
									bFound = new XVar(true);
								}
							}
							else
							{
								begin.Remove(MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(gname2, "_firstnewgroup"))));
							}
						}
						if(XVar.Pack(!(XVar)(bFound)))
						{
							begin.InitAndSetArrayItem(true, MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(gname, "_firstnewgroup"))));
						}
						begin.Remove(MVCFunctions.GoodFieldName(new XVar("nonewgroup")));
					}
					begin.InitAndSetArrayItem(true, MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(gname, "_newgroup"))));
					var_end.InitAndSetArrayItem(true, MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(gname, "_endgroup"))));
					if(XVar.Pack(this.repGroupFields[i]["showGroupSummary"]))
					{
						var_end.InitAndSetArrayItem(CommonFunctions.str_format_number((XVar)(grp["count"]), new XVar(0)), MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat("group", gname, "_total_cnt"))));
					}
					j = new XVar(0);
					for(;j < MVCFunctions.count(this.fieldsArr); j++)
					{
						if(XVar.Pack(MVCFunctions.is_array((XVar)(grp["summary"]))))
						{
							if(XVar.Pack(MVCFunctions.is_array((XVar)(grp["summary"][this.fieldsArr[j]["name"]]))))
							{
								if(XVar.Pack(this.fieldsArr[j]["totalMax"]))
								{
									var_end.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(grp["summary"][this.fieldsArr[j]["name"]]["MAX"]), (XVar)(this.fieldsArr[j]["name"]), (XVar)(this.fieldsArr[j]["viewFormat"]), new XVar("MAX"), (XVar)(values)), MVCFunctions.Concat("group", MVCFunctions.GoodFieldName((XVar)(gname)), "_total", this.fieldsArr[j]["goodName"], "_max"));
								}
								if(XVar.Pack(this.fieldsArr[j]["totalMin"]))
								{
									var_end.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(grp["summary"][this.fieldsArr[j]["name"]]["MIN"]), (XVar)(this.fieldsArr[j]["name"]), (XVar)(this.fieldsArr[j]["viewFormat"]), new XVar("MIN"), (XVar)(values)), MVCFunctions.Concat("group", MVCFunctions.GoodFieldName((XVar)(gname)), "_total", this.fieldsArr[j]["goodName"], "_min"));
								}
								if(XVar.Pack(this.fieldsArr[j]["totalAvg"]))
								{
									var_end.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(grp["summary"][this.fieldsArr[j]["name"]]["AVG"]), (XVar)(this.fieldsArr[j]["name"]), (XVar)(this.fieldsArr[j]["viewFormat"]), new XVar("AVG"), (XVar)(values)), MVCFunctions.Concat("group", MVCFunctions.GoodFieldName((XVar)(gname)), "_total", this.fieldsArr[j]["goodName"], "_avg"));
								}
								if(XVar.Pack(this.fieldsArr[j]["totalSum"]))
								{
									var_end.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(grp["summary"][this.fieldsArr[j]["name"]]["SUM"]), (XVar)(this.fieldsArr[j]["name"]), (XVar)(this.fieldsArr[j]["viewFormat"]), new XVar("SUM"), (XVar)(values)), MVCFunctions.Concat("group", MVCFunctions.GoodFieldName((XVar)(gname)), "_total", this.fieldsArr[j]["goodName"], "_sum"));
								}
							}
						}
						if(this.fieldsArr[j]["name"] == this.repGroupFields[i]["strGroupField"])
						{
							dynamic gvalue = null;
							field = XVar.Clone(this._sql.field((XVar)(nField)));
							gvalue = XVar.Clone(field.getFieldName((XVar)(gkey), (XVar)(grp["_first"]), (XVar)(this.pageObject)));
							if(XVar.Pack(field.overrideFormat()))
							{
								if(this.pageObject.pageType == Constants.PAGE_RPRINT)
								{
									if((XVar)((XVar)(this.pageObject.pdfJsonMode())  && (XVar)(CommonFunctions.IsDateFieldType((XVar)(this.pSet.getFieldType((XVar)(this.fieldsArr[j]["name"]))))))  && (XVar)((XVar)((XVar)(gvalue == "NULL")  || (XVar)(field._interval != 0))  || (XVar)(!(XVar)(field._viewFormat))))
									{
										gvalue = XVar.Clone(MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(gvalue)), "'"));
									}
								}
								begin.InitAndSetArrayItem((XVar.Pack(this.forExport == "excel") ? XVar.Pack(MVCFunctions.runner_htmlspecialchars((XVar)(gvalue))) : XVar.Pack(gvalue)), MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(gname)), "_grval"))));
								if(XVar.Pack(this.showGroupSummaryCount))
								{
									var_end.InitAndSetArrayItem((XVar.Pack(this.forExport == "excel") ? XVar.Pack(MVCFunctions.runner_htmlspecialchars((XVar)(gvalue))) : XVar.Pack(gvalue)), MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(gname)), "_grval"))));
								}
							}
							else
							{
								dynamic formattedValue = null;
								formattedValue = XVar.Clone(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(gvalue), (XVar)(this.fieldsArr[j]["name"]), (XVar)(this.fieldsArr[j]["viewFormat"]), new XVar(""), (XVar)(values)));
								begin.InitAndSetArrayItem((XVar.Pack(this.forExport == "excel") ? XVar.Pack(MVCFunctions.runner_htmlspecialchars((XVar)(formattedValue))) : XVar.Pack(formattedValue)), MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(gname, "_grval"))));
								if(XVar.Pack(this.showGroupSummaryCount))
								{
									var_end.InitAndSetArrayItem((XVar.Pack(this.forExport == "excel") ? XVar.Pack(MVCFunctions.runner_htmlspecialchars((XVar)(formattedValue))) : XVar.Pack(formattedValue)), MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(gname, "_grval"))));
								}
							}
						}
					}
				}
			}

			return null;
		}
		public override XVar _writePage(dynamic page, dynamic _param_src, dynamic _param_count)
		{
			#region pass-by-value parameters
			dynamic src = XVar.Clone(_param_src);
			dynamic count = XVar.Clone(_param_count);
			#endregion

			page.InitAndSetArrayItem(true, "page_summary");
			if(XVar.Pack(this.repPageSummary))
			{
				dynamic fGoodName = null, field = XVar.Array(), fieldName = null, i = null;
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.fieldsArr); i++)
				{
					field = this.fieldsArr[i];
					fieldName = XVar.Clone(field["name"]);
					fGoodName = XVar.Clone(field["goodName"]);
					if(XVar.Pack(MVCFunctions.is_array((XVar)(src[fieldName]))))
					{
						if(XVar.Pack(field["totalSum"]))
						{
							page.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(src[fieldName]["SUM"]), (XVar)(fieldName), (XVar)(field["viewFormat"]), new XVar("SUM")), MVCFunctions.Concat("page_total", fGoodName, "_sum"));
						}
						if(XVar.Pack(field["totalAvg"]))
						{
							page.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(src[fieldName]["AVG"]), (XVar)(fieldName), (XVar)(field["viewFormat"]), new XVar("AVG")), MVCFunctions.Concat("page_total", fGoodName, "_avg"));
						}
						if(XVar.Pack(field["totalMin"]))
						{
							page.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(src[fieldName]["MIN"]), (XVar)(fieldName), (XVar)(field["viewFormat"]), new XVar("MIN")), MVCFunctions.Concat("page_total", fGoodName, "_min"));
						}
						if(XVar.Pack(field["totalMax"]))
						{
							page.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(src[fieldName]["MAX"]), (XVar)(fieldName), (XVar)(field["viewFormat"]), new XVar("MAX")), MVCFunctions.Concat("page_total", fGoodName, "_max"));
						}
					}
				}
				page.InitAndSetArrayItem(CommonFunctions.str_format_number((XVar)(count), new XVar(0)), "page_total_cnt");
			}

			return null;
		}
		public override XVar writeGlobalSummary(dynamic _param_source)
		{
			#region pass-by-value parameters
			dynamic source = XVar.Clone(_param_source);
			#endregion

			dynamic result = XVar.Array();
			result = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(this.repGlobalSummary)))
			{
				return result;
			}
			if(XVar.Pack(MVCFunctions.is_array((XVar)(source["summary"]))))
			{
				dynamic fGoodName = null, field = XVar.Array(), fieldName = null, i = null;
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.fieldsArr); i++)
				{
					field = this.fieldsArr[i];
					fieldName = XVar.Clone(field["name"]);
					fGoodName = XVar.Clone(field["goodName"]);
					if(XVar.Pack(MVCFunctions.is_array((XVar)(source["summary"][fieldName]))))
					{
						if(XVar.Pack(field["totalMax"]))
						{
							result.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(source["summary"][fieldName]["MAX"]), (XVar)(fieldName), (XVar)(field["viewFormat"]), new XVar("MAX")), MVCFunctions.Concat("global_total", fGoodName, "_max"));
						}
						if(XVar.Pack(field["totalMin"]))
						{
							result.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(source["summary"][fieldName]["MIN"]), (XVar)(fieldName), (XVar)(field["viewFormat"]), new XVar("MIN")), MVCFunctions.Concat("global_total", fGoodName, "_min"));
						}
						if(XVar.Pack(field["totalAvg"]))
						{
							result.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(source["summary"][fieldName]["AVG"]), (XVar)(fieldName), (XVar)(field["viewFormat"]), new XVar("AVG")), MVCFunctions.Concat("global_total", fGoodName, "_avg"));
						}
						if(XVar.Pack(field["totalSum"]))
						{
							result.InitAndSetArrayItem(CommonFunctions.getFormattedValue((XVar)(this.pageObject), (XVar)(source["summary"][fieldName]["SUM"]), (XVar)(fieldName), (XVar)(field["viewFormat"]), new XVar("SUM")), MVCFunctions.Concat("global_total", fGoodName, "_sum"));
						}
					}
				}
			}
			result.InitAndSetArrayItem(CommonFunctions.str_format_number((XVar)(source["count"]), new XVar(0)), "global_total_cnt");
			return result;
		}
	}
	// Included file globals
	public partial class CommonFunctions
	{
		public static XVar create_reportfield(dynamic _param_name, dynamic _param_type, dynamic _param_interval, dynamic _param_alias, dynamic _param_table, dynamic _param_connection, dynamic _param_cipherer)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic var_type = XVar.Clone(_param_type);
			dynamic interval = XVar.Clone(_param_interval);
			dynamic alias = XVar.Clone(_param_alias);
			dynamic table = XVar.Clone(_param_table);
			dynamic connection = XVar.Clone(_param_connection);
			dynamic cipherer = XVar.Clone(_param_cipherer);
			#endregion

			if(XVar.Pack(!(XVar)(var_type)))
			{
				return null;
			}
			if(var_type == "char")
			{
				return new ReportCharField((XVar)(name), (XVar)(interval), (XVar)(alias), (XVar)(table), (XVar)(connection), (XVar)(cipherer));
			}
			if(var_type == "date")
			{
				return new ReportDateField((XVar)(name), (XVar)(interval), (XVar)(alias), (XVar)(table), (XVar)(connection), (XVar)(cipherer));
			}
			if(var_type == "numeric")
			{
				return new ReportNumericField((XVar)(name), (XVar)(interval), (XVar)(alias), (XVar)(table), (XVar)(connection), (XVar)(cipherer));
			}
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public static XVar getFormattedValue(dynamic _param_pageObject, dynamic _param_value, dynamic _param_fieldName, dynamic _param_strViewFormat, dynamic _param_total = null, dynamic _param_record = null)
		{
			#region default values
			if(_param_total as Object == null) _param_total = new XVar("");
			if(_param_record as Object == null) _param_record = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic value = XVar.Clone(_param_value);
			dynamic fieldName = XVar.Clone(_param_fieldName);
			dynamic strViewFormat = XVar.Clone(_param_strViewFormat);
			dynamic total = XVar.Clone(_param_total);
			dynamic record = XVar.Clone(_param_record);
			#endregion

			dynamic arrValues = XVar.Array();
			if((XVar)(strViewFormat == Constants.FORMAT_TIME)  && (XVar)(MVCFunctions.IsNumeric(value)))
			{
				if(total == "AVG")
				{
					value = XVar.Clone((XVar)Math.Round((double)(value), 0));
				}
				return ViewTimeField.getFormattedTotals((XVar)(fieldName), (XVar)(value), (XVar)(pageObject.pSet), (XVar)(pageObject.pdfJsonMode()), (XVar)(total == "SUM"));
			}
			if(XVar.Pack(CommonFunctions.basicViewFormat((XVar)(strViewFormat))))
			{
				arrValues = XVar.Clone(new XVar(fieldName, value));
			}
			else
			{
				arrValues = XVar.Clone(record);
				arrValues.InitAndSetArrayItem(value, fieldName);
			}
			return pageObject.formatReportFieldValue((XVar)(fieldName), (XVar)(arrValues));
		}
		public static XVar cached_db2time(dynamic _param_strtime)
		{
			#region pass-by-value parameters
			dynamic strtime = XVar.Clone(_param_strtime);
			#endregion

			if(XVar.Pack(!(XVar)(GlobalVars.cache_db2time.KeyExists(strtime))))
			{
				dynamic res = null;
				res = XVar.Clone(MVCFunctions.db2time((XVar)(strtime)));
				GlobalVars.cache_db2time.InitAndSetArrayItem(res, strtime);
				return res;
			}
			else
			{
				return GlobalVars.cache_db2time[strtime];
			}

			return null;
		}
		public static XVar cached_getdayofweek(dynamic _param_strtime)
		{
			#region pass-by-value parameters
			dynamic strtime = XVar.Clone(_param_strtime);
			#endregion

			if(XVar.Pack(!(XVar)(GlobalVars.cache_getdayofweek.KeyExists(strtime))))
			{
				dynamic date = null, res = null;
				date = XVar.Clone(CommonFunctions.cached_db2time((XVar)(strtime)));
				res = XVar.Clone(CommonFunctions.getdayofweek((XVar)(date)));
				GlobalVars.cache_getdayofweek.InitAndSetArrayItem(res, strtime);
				return res;
			}
			else
			{
				return GlobalVars.cache_getdayofweek[strtime];
			}

			return null;
		}
		public static XVar cached_getweekstart(dynamic _param_strtime)
		{
			#region pass-by-value parameters
			dynamic strtime = XVar.Clone(_param_strtime);
			#endregion

			if(XVar.Pack(!(XVar)(GlobalVars.cache_getweekstart.KeyExists(strtime))))
			{
				dynamic date = null, res = null;
				date = XVar.Clone(CommonFunctions.cached_db2time((XVar)(strtime)));
				res = XVar.Clone(CommonFunctions.getweekstart((XVar)(date)));
				GlobalVars.cache_getweekstart.InitAndSetArrayItem(res, strtime);
				return res;
			}
			else
			{
				return GlobalVars.cache_getweekstart[strtime];
			}

			return null;
		}
		public static XVar cached_formatweekstart(dynamic _param_strtime)
		{
			#region pass-by-value parameters
			dynamic strtime = XVar.Clone(_param_strtime);
			#endregion

			if(XVar.Pack(!(XVar)(GlobalVars.cache_formatweekstart.KeyExists(strtime))))
			{
				dynamic res = null, start = null, var_end = null;
				start = XVar.Clone(CommonFunctions.cached_getweekstart((XVar)(strtime)));
				var_end = XVar.Clone(CommonFunctions.adddays((XVar)(start), new XVar(6)));
				res = XVar.Clone(MVCFunctions.Concat(CommonFunctions.format_shortdate((XVar)(start)), " - ", CommonFunctions.format_shortdate((XVar)(var_end))));
				GlobalVars.cache_formatweekstart.InitAndSetArrayItem(res, strtime);
				return res;
			}
			else
			{
				return GlobalVars.cache_formatweekstart[strtime];
			}

			return null;
		}
	}
}
