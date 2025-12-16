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
	public partial class CrossTableReport : XClass
	{
		protected dynamic tableName;
		protected dynamic pageType;
		protected dynamic viewControls;
		protected ProjectSettings pSet = null;
		protected dynamic connection;
		protected dynamic dataSource;
		protected dynamic group_header = XVar.Array();
		protected dynamic col_summary = XVar.Array();
		protected dynamic rowinfo = XVar.Array();
		protected dynamic total_summary;
		protected dynamic showXSummary;
		protected dynamic showYSummary;
		protected dynamic showTotalSummary;
		protected dynamic groupFieldsData;
		protected dynamic fieldsTotalsData;
		protected dynamic xFName;
		protected dynamic yFName;
		protected dynamic xIntervalType;
		protected dynamic yIntervalType;
		protected dynamic dataField = XVar.Pack("");
		protected dynamic dataFieldSettings = XVar.Pack(null);
		protected dynamic dataGroupFunction = XVar.Pack("");
		protected dynamic pdfJSON = XVar.Pack(false);
		protected dynamic selectedAxis;
		protected dynamic pageObject;
		protected dynamic sort_groups;
		protected dynamic sortGroupOrder = XVar.Pack("ASC");
		public CrossTableReport(dynamic _param_params, dynamic _param_pageObject)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			this.pageObject = XVar.Clone(pageObject);
			this.dataSource = XVar.Clone(pageObject.getDataSource());
			this.pageType = XVar.Clone(var_params["pageType"]);
			this.tableName = XVar.Clone(var_params["tableName"]);
			this.setDbConnection();
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tableName), new XVar(Constants.PAGE_REPORT)));
			this.viewControls = XVar.Clone(new ViewControlsContainer((XVar)(this.pSet), new XVar(Constants.PAGE_REPORT)));
			this.pdfJSON = XVar.Clone(var_params["pdfJSON"]);
			this.groupFieldsData = XVar.Clone(var_params["groupFields"]);
			this.fieldsTotalsData = XVar.Clone(var_params["totals"]);
			this.selectedAxis = XVar.Clone(var_params["selectedAxis"]);
			this.showXSummary = XVar.Clone(var_params["xSummary"]);
			this.showYSummary = XVar.Clone(var_params["ySummary"]);
			this.showTotalSummary = XVar.Clone(var_params["totalSummary"]);
			this.xFName = XVar.Clone(this.getGroupFieldByParam(new XVar("x"), (XVar)(var_params["x"])));
			this.yFName = XVar.Clone(this.getGroupFieldByParam(new XVar("y"), (XVar)(var_params["y"]), (XVar)(this.xFName)));
			this.dataField = XVar.Clone(this.getDataFieldByParam((XVar)(var_params["data"])));
			this.dataFieldSettings = XVar.Clone(var_params["totals"][this.dataField]);
			this.dataGroupFunction = XVar.Clone(this.getDataGroupFunction((XVar)(var_params["operation"])));
			this.xIntervalType = XVar.Clone(this.getIntervalTypeByParam(new XVar("x"), (XVar)(this.xFName), (XVar)(var_params["xType"])));
			this.yIntervalType = XVar.Clone(this.getIntervalTypeByParam(new XVar("y"), (XVar)(this.yFName), (XVar)(var_params["yType"])));
			this.correctCrosstabParams();
			this.fillGridData((XVar)(var_params["headerClass"]), (XVar)(var_params["dataClass"]));
		}
		protected virtual XVar fillGridData(dynamic _param_headerClass, dynamic _param_dataClass)
		{
			#region pass-by-value parameters
			dynamic headerClass = XVar.Clone(_param_headerClass);
			dynamic dataClass = XVar.Clone(_param_dataClass);
			#endregion

			dynamic arravgcount = XVar.Array(), arravgsum = XVar.Array(), arrdata = XVar.Array(), avgcountx = XVar.Array(), avgsumx = XVar.Array(), data = XVar.Array(), group_x = XVar.Array(), group_y = XVar.Array(), key_x = null, key_y = null, newColSummary = XVar.Array(), qResult = null, sort_indices = XVar.Array(), space = null;
			qResult = XVar.Clone(this.dataSource.getTotals((XVar)(this.getDataCommand())));
			group_y = XVar.Clone(XVar.Array());
			group_x = XVar.Clone(XVar.Array());
			arrdata = XVar.Clone(XVar.Array());
			arravgsum = XVar.Clone(XVar.Array());
			arravgcount = XVar.Clone(XVar.Array());
			avgsumx = XVar.Clone(XVar.Array());
			avgcountx = XVar.Clone(XVar.Array());
			space = XVar.Clone((XVar.Pack(this.pdfJsonMode()) ? XVar.Pack("' '") : XVar.Pack("&nbsp;")));
			while(XVar.Pack(data = XVar.Clone(qResult.fetchNumeric())))
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(data[1]), (XVar)(group_y)))))
				{
					group_y.InitAndSetArrayItem(data[1], null);
				}
				if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(data[2]), (XVar)(group_x)))))
				{
					group_x.InitAndSetArrayItem(data[2], null);
					this.col_summary.InitAndSetArrayItem(space, "data", MVCFunctions.count(group_x) - 1, "col_summary");
					this.col_summary.InitAndSetArrayItem(MVCFunctions.Concat("total_x_", MVCFunctions.count(group_x) - 1), "data", MVCFunctions.count(group_x) - 1, "id_col_summary");
				}
				key_y = XVar.Clone(MVCFunctions.array_search((XVar)(data[1]), (XVar)(group_y)));
				key_x = XVar.Clone(MVCFunctions.array_search((XVar)(data[2]), (XVar)(group_x)));
				avgsumx.InitAndSetArrayItem(0, key_x);
				avgcountx.InitAndSetArrayItem(0, key_x);
				arrdata.InitAndSetArrayItem(data[0], key_y, key_x);
				arravgsum.InitAndSetArrayItem(data[3], key_y, key_x);
				arravgcount.InitAndSetArrayItem(data[4], key_y, key_x);
			}
			this.sort_groups = group_x;
			sort_indices = XVar.Clone(MVCFunctions.array_keys((XVar)(group_x)));
			this.sortGroupOrder = XVar.Clone(this.getGroupOrderDirection((XVar)(this.xFName)));
			MVCFunctions.usort((XVar)(sort_indices), (XVar)(new XVar(0, this, 1, "groupSort")));
			newColSummary = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> i in sort_indices.GetEnumerator())
			{
				newColSummary.InitAndSetArrayItem(this.col_summary["data"][i.Value], null);
			}
			this.col_summary.InitAndSetArrayItem(newColSummary, "data");
			this.rowinfo = XVar.Clone(this.getBasicRowsData((XVar)(group_y), (XVar)(group_x), (XVar)(sort_indices), (XVar)(arrdata), (XVar)(dataClass)));
			foreach (KeyValuePair<XVar, dynamic> _key_x in sort_indices.GetEnumerator())
			{
				dynamic value_x = null;
				value_x = XVar.Clone(group_x[_key_x.Value]);
				if(value_x != XVar.Pack(""))
				{
					this.group_header.InitAndSetArrayItem(this.pageObject.formatGroupValue((XVar)(this.xFName), (XVar)(this.xIntervalType), (XVar)(value_x)), "data", _key_x.Value, "gr_value");
				}
				else
				{
					this.group_header.InitAndSetArrayItem(space, "data", _key_x.Value, "gr_value");
				}
				this.group_header.InitAndSetArrayItem(headerClass, "data", _key_x.Value, "gr_x_class");
			}
			arravgsum = XVar.Clone(this.sortAvgTotalsData((XVar)(arravgsum), (XVar)(sort_indices)));
			arravgcount = XVar.Clone(this.sortAvgTotalsData((XVar)(arravgcount), (XVar)(sort_indices)));
			this.setSummariesData((XVar)(arravgsum), (XVar)(arravgcount), (XVar)(avgsumx), (XVar)(avgcountx));
			this.updateRecordsDisplayedFields();

			return null;
		}
		protected virtual XVar sortAvgTotalsData(dynamic _param_data, dynamic _param_sort_x)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic sort_x = XVar.Clone(_param_sort_x);
			#endregion

			dynamic sorted = XVar.Array();
			sorted = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value_y in data.GetEnumerator())
			{
				sorted.InitAndSetArrayItem(XVar.Array(), value_y.Key);
				foreach (KeyValuePair<XVar, dynamic> key_x in sort_x.GetEnumerator())
				{
					if(XVar.Pack(value_y.Value.KeyExists(key_x.Value)))
					{
						sorted.InitAndSetArrayItem(value_y.Value[key_x.Value], value_y.Key, key_x.Key);
					}
				}
			}
			return sorted;
		}
		public virtual XVar groupSort(dynamic _param_i1, dynamic _param_i2)
		{
			#region pass-by-value parameters
			dynamic i1 = XVar.Clone(_param_i1);
			dynamic i2 = XVar.Clone(_param_i2);
			#endregion

			dynamic a = null, b = null;
			a = XVar.Clone(this.sort_groups[i1]);
			b = XVar.Clone(this.sort_groups[i2]);
			if(b < a)
			{
				return (XVar.Pack(this.sortGroupOrder == "DESC") ? XVar.Pack(-1) : XVar.Pack(1));
			}
			if(a < b)
			{
				return (XVar.Pack(this.sortGroupOrder == "DESC") ? XVar.Pack(1) : XVar.Pack(-1));
			}
			return 0;
		}
		protected virtual XVar getBasicRowsData(dynamic _param_group_y, dynamic _param_group_x, dynamic _param_sort_x, dynamic _param_arrdata, dynamic _param_dataClass)
		{
			#region pass-by-value parameters
			dynamic group_y = XVar.Clone(_param_group_y);
			dynamic group_x = XVar.Clone(_param_group_x);
			dynamic sort_x = XVar.Clone(_param_sort_x);
			dynamic arrdata = XVar.Clone(_param_arrdata);
			dynamic dataClass = XVar.Clone(_param_dataClass);
			#endregion

			dynamic crossRowsData = XVar.Array(), ftype = null, space = null;
			crossRowsData = XVar.Clone(XVar.Array());
			ftype = XVar.Clone(this.pSet.getFieldType((XVar)(this.dataField)));
			space = XVar.Clone((XVar.Pack(this.pdfJsonMode()) ? XVar.Pack("' '") : XVar.Pack("&nbsp;")));
			foreach (KeyValuePair<XVar, dynamic> value_y in group_y.GetEnumerator())
			{
				dynamic row = XVar.Array();
				crossRowsData.InitAndSetArrayItem(space, value_y.Key, "row_summary");
				crossRowsData.InitAndSetArrayItem(this.pageObject.formatGroupValue((XVar)(this.yFName), (XVar)(this.yIntervalType), (XVar)(value_y.Value)), value_y.Key, "group_y");
				crossRowsData.InitAndSetArrayItem(MVCFunctions.Concat("total_y_", value_y.Key), value_y.Key, "id_row_summary");
				crossRowsData.InitAndSetArrayItem(dataClass, value_y.Key, "summary_class");
				if(XVar.Pack(!(XVar)(arrdata.KeyExists(value_y.Key))))
				{
					continue;
				}
				if(XVar.Pack(!(XVar)(crossRowsData[value_y.Key]["row_record"])))
				{
					crossRowsData.InitAndSetArrayItem(XVar.Array(), value_y.Key, "row_record");
				}
				crossRowsData.InitAndSetArrayItem(XVar.Array(), value_y.Key, "row_record", "data");
				row = crossRowsData[value_y.Key]["row_record"]["data"];
				foreach (KeyValuePair<XVar, dynamic> key_x in sort_x.GetEnumerator())
				{
					row.InitAndSetArrayItem(XVar.Array(), null);
				}
				foreach (KeyValuePair<XVar, dynamic> key_x in sort_x.GetEnumerator())
				{
					dynamic rowValue = null, value_x = null;
					value_x = XVar.Clone(group_x[key_x.Value]);
					rowValue = XVar.Clone(space);
					if((XVar)(arrdata[value_y.Key].KeyExists(key_x.Value))  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(arrdata[value_y.Key][key_x.Value]), XVar.Pack(null)))))
					{
						rowValue = XVar.Clone(arrdata[value_y.Key][key_x.Value]);
						if((XVar)(this.dataGroupFunction == "avg")  && (XVar)(!(XVar)(CommonFunctions.IsTimeType((XVar)(ftype)))))
						{
							rowValue = XVar.Clone(rowValue);
						}
					}
					row.InitAndSetArrayItem(rowValue, key_x.Key, "row_value");
					row.InitAndSetArrayItem(dataClass, key_x.Key, "row_value_class");
					row.InitAndSetArrayItem(MVCFunctions.Concat(value_y.Key, "_", key_x.Key), key_x.Key, "id_data");
				}
			}
			return crossRowsData;
		}
		protected virtual XVar setSummariesData(dynamic _param_arravgsum, dynamic _param_arravgcount, dynamic _param_avgsumx, dynamic _param_avgcountx)
		{
			#region pass-by-value parameters
			dynamic arravgsum = XVar.Clone(_param_arravgsum);
			dynamic arravgcount = XVar.Clone(_param_arravgcount);
			dynamic avgsumx = XVar.Clone(_param_avgsumx);
			dynamic avgcountx = XVar.Clone(_param_avgcountx);
			#endregion

			dynamic space = null, totaxSummary = null, xSummaty = null, ySummary = null;
			space = XVar.Clone((XVar.Pack(this.pdfJsonMode()) ? XVar.Pack("' '") : XVar.Pack("&nbsp;")));
			this.total_summary = XVar.Clone(space);
			xSummaty = XVar.Clone(XVar.Array());
			ySummary = XVar.Clone(XVar.Array());
			totaxSummary = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> yData in this.rowinfo.GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> value in yData.Value["row_record"]["data"].GetEnumerator())
				{
					if(value.Value["row_value"] == space)
					{
						continue;
					}
					switch(((XVar)this.dataGroupFunction).ToString())
					{
						case "sum":
							if(XVar.Equals(XVar.Pack(this.rowinfo[yData.Key]["row_summary"]), XVar.Pack(space)))
							{
								this.rowinfo.InitAndSetArrayItem(0, yData.Key, "row_summary");
							}
							if(XVar.Equals(XVar.Pack(this.col_summary["data"][value.Key]["col_summary"]), XVar.Pack(space)))
							{
								this.col_summary.InitAndSetArrayItem(0, "data", value.Key, "col_summary");
							}
							if(XVar.Equals(XVar.Pack(this.total_summary), XVar.Pack(space)))
							{
								this.total_summary = new XVar(0);
							}
							this.rowinfo[yData.Key]["row_summary"] += (double)value.Value["row_value"];
							this.col_summary["data"][value.Key]["col_summary"] += (double)value.Value["row_value"];
							this.total_summary += (double)value.Value["row_value"];
							break;
						case "min":
							if((XVar)(XVar.Equals(XVar.Pack(this.rowinfo[yData.Key]["row_summary"]), XVar.Pack(space)))  || (XVar)(value.Value["row_value"] < this.rowinfo[yData.Key]["row_summary"]))
							{
								this.rowinfo.InitAndSetArrayItem(value.Value["row_value"], yData.Key, "row_summary");
							}
							if((XVar)(XVar.Equals(XVar.Pack(this.col_summary["data"][value.Key]["col_summary"]), XVar.Pack(space)))  || (XVar)(value.Value["row_value"] < this.col_summary["data"][value.Key]["col_summary"]))
							{
								this.col_summary.InitAndSetArrayItem(value.Value["row_value"], "data", value.Key, "col_summary");
							}
							if((XVar)(XVar.Equals(XVar.Pack(this.total_summary), XVar.Pack(space)))  || (XVar)(value.Value["row_value"] < this.total_summary))
							{
								this.total_summary = XVar.Clone(value.Value["row_value"]);
							}
							break;
						case "max":
							if((XVar)(XVar.Equals(XVar.Pack(this.rowinfo[yData.Key]["row_summary"]), XVar.Pack(space)))  || (XVar)(this.rowinfo[yData.Key]["row_summary"] < value.Value["row_value"]))
							{
								this.rowinfo.InitAndSetArrayItem(value.Value["row_value"], yData.Key, "row_summary");
							}
							if((XVar)(XVar.Equals(XVar.Pack(this.col_summary["data"][value.Key]["col_summary"]), XVar.Pack(space)))  || (XVar)(this.col_summary["data"][value.Key]["col_summary"] < value.Value["row_value"]))
							{
								this.col_summary.InitAndSetArrayItem(value.Value["row_value"], "data", value.Key, "col_summary");
							}
							if((XVar)(XVar.Equals(XVar.Pack(this.total_summary), XVar.Pack(space)))  || (XVar)(this.total_summary < value.Value["row_value"]))
							{
								this.total_summary = XVar.Clone(value.Value["row_value"]);
							}
							break;
						case "avg":
							this.rowinfo[yData.Key]["avgsumy"] += arravgsum[yData.Key][value.Key];
							this.rowinfo[yData.Key]["avgcounty"] += arravgcount[yData.Key][value.Key];
							this.rowinfo[yData.Key]["row_record"]["data"][value.Key]["avgsumx"] += arravgsum[yData.Key][value.Key];
							this.rowinfo[yData.Key]["row_record"]["data"][value.Key]["avgcountx"] += arravgcount[yData.Key][value.Key];
							break;
					}
					if((XVar)(this.showXSummary)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.col_summary["data"][value.Key]["col_summary"]), XVar.Pack(null)))))
					{
						if(XVar.Pack(MVCFunctions.IsNumeric(this.col_summary["data"][value.Key]["col_summary"])))
						{
							this.col_summary.InitAndSetArrayItem(this.col_summary["data"][value.Key]["col_summary"], "data", value.Key, "col_summary");
						}
					}
					else
					{
						this.col_summary.InitAndSetArrayItem(space, "data", value.Key, "col_summary");
					}
				}
				if((XVar)(this.showYSummary)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.rowinfo[yData.Key]["row_summary"]), XVar.Pack(null)))))
				{
					if(XVar.Pack(MVCFunctions.IsNumeric(this.rowinfo[yData.Key]["row_summary"])))
					{
						this.rowinfo.InitAndSetArrayItem(this.rowinfo[yData.Key]["row_summary"], yData.Key, "row_summary");
					}
				}
				else
				{
					this.rowinfo.InitAndSetArrayItem(space, yData.Key, "row_summary");
				}
			}
			if(this.dataGroupFunction == "avg")
			{
				dynamic total_count = null, total_sum = null;
				total_sum = new XVar(0);
				total_count = new XVar(0);
				foreach (KeyValuePair<XVar, dynamic> valuey in this.rowinfo.GetEnumerator())
				{
					if(XVar.Pack(valuey.Value["avgcounty"]))
					{
						if(XVar.Pack(this.showYSummary))
						{
							this.rowinfo.InitAndSetArrayItem(valuey.Value["avgsumy"] / valuey.Value["avgcounty"], valuey.Key, "row_summary");
						}
						total_sum += valuey.Value["avgsumy"];
						total_count += valuey.Value["avgcounty"];
					}
					foreach (KeyValuePair<XVar, dynamic> valuex in valuey.Value["row_record"]["data"].GetEnumerator())
					{
						if(XVar.Pack(valuex.Value["avgcountx"]))
						{
							avgsumx[valuex.Key] += valuex.Value["avgsumx"];
							avgcountx[valuex.Key] += valuex.Value["avgcountx"];
							total_sum += valuex.Value["avgsumx"];
							total_count += valuex.Value["avgcountx"];
						}
					}
				}
				if(XVar.Pack(this.showXSummary))
				{
					foreach (KeyValuePair<XVar, dynamic> value in avgsumx.GetEnumerator())
					{
						if(XVar.Pack(avgcountx[value.Key]))
						{
							this.col_summary.InitAndSetArrayItem(value.Value / avgcountx[value.Key], "data", value.Key, "col_summary");
						}
					}
				}
				if(XVar.Pack(total_count))
				{
					this.total_summary = XVar.Clone(total_sum / total_count);
				}
			}
			if(XVar.Pack(!(XVar)(this.showTotalSummary)))
			{
				this.total_summary = XVar.Clone(space);
			}
			else
			{
				if(XVar.Pack(MVCFunctions.IsNumeric(this.total_summary)))
				{
					this.total_summary = XVar.Clone(this.total_summary);
				}
			}

			return null;
		}
		public virtual XVar getViewValue(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic controlData = null;
			if((XVar)(this.pSet.getViewFormat((XVar)(this.dataField)) == Constants.FORMAT_TIME)  && (XVar)(MVCFunctions.IsNumeric(value)))
			{
				if(this.dataGroupFunction == "avg")
				{
					value = XVar.Clone((XVar)Math.Round((double)(value), 0));
				}
				return ViewTimeField.getFormattedTotals((XVar)(this.dataField), (XVar)(value), (XVar)(this.pSet), (XVar)(this.pdfJsonMode()), (XVar)(this.dataGroupFunction == "sum"));
			}
			controlData = XVar.Clone(new XVar(this.dataField, value));
			return this.showDBValue((XVar)(this.dataField), (XVar)(controlData));
		}
		protected virtual XVar updateRecordsDisplayedFields()
		{
			dynamic space = null;
			if(XVar.Pack(!(XVar)(this.rowinfo)))
			{
				return null;
			}
			space = XVar.Clone((XVar.Pack(this.pdfJsonMode()) ? XVar.Pack("' '") : XVar.Pack("&nbsp;")));
			foreach (KeyValuePair<XVar, dynamic> data in this.rowinfo.GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> fieldData in data.Value["row_record"]["data"].GetEnumerator())
				{
					if(fieldData.Value["row_value"] == space)
					{
						continue;
					}
					this.rowinfo.InitAndSetArrayItem(this.getViewValue((XVar)(fieldData.Value["row_value"])), data.Key, "row_record", "data", fieldData.Key, "row_value");
				}
				if(data.Value["row_summary"] != space)
				{
					this.rowinfo.InitAndSetArrayItem(this.getViewValue((XVar)(data.Value["row_summary"])), data.Key, "row_summary");
				}
			}
			if(this.total_summary != space)
			{
				this.total_summary = XVar.Clone(this.getViewValue((XVar)(this.total_summary)));
			}
			foreach (KeyValuePair<XVar, dynamic> summaryData in this.col_summary["data"].GetEnumerator())
			{
				if(summaryData.Value["col_summary"] == space)
				{
					continue;
				}
				this.col_summary.InitAndSetArrayItem(this.getViewValue((XVar)(summaryData.Value["col_summary"])), "data", summaryData.Key, "col_summary");
			}

			return null;
		}
		public virtual XVar getCrossTableData()
		{
			return this.rowinfo;
		}
		public virtual XVar isEmpty()
		{
			return !(XVar)(this.rowinfo);
		}
		public virtual XVar getCrossTableHeader()
		{
			return this.group_header;
		}
		public virtual XVar getCrossTableSummary()
		{
			return this.col_summary;
		}
		public virtual XVar getTotalSummary()
		{
			return this.total_summary;
		}
		protected virtual XVar setDbConnection()
		{
			this.connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(this.tableName)));

			return null;
		}
		protected virtual XVar getIntervalTypeByParam(dynamic _param_axis, dynamic _param_crossName, dynamic _param_userIntType)
		{
			#region pass-by-value parameters
			dynamic axis = XVar.Clone(_param_axis);
			dynamic crossName = XVar.Clone(_param_crossName);
			dynamic userIntType = XVar.Clone(_param_userIntType);
			#endregion

			dynamic iType = null, intTypes = XVar.Array(), int_type = null;
			iType = XVar.Clone(this.getRefineIntervalType((XVar)(userIntType), (XVar)(crossName)));
			int_type = XVar.Clone(-1);
			intTypes = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> fData in this.groupFieldsData.GetEnumerator())
			{
				if((XVar)(fData.Value["name"] == crossName)  && (XVar)((XVar)(fData.Value["group_type"] == "all")  || (XVar)(fData.Value["group_type"] == axis)))
				{
					if((XVar)(!(XVar)(MVCFunctions.strlen((XVar)(userIntType))))  || (XVar)(iType == fData.Value["int_type"]))
					{
						int_type = XVar.Clone(fData.Value["int_type"]);
						break;
					}
					intTypes.InitAndSetArrayItem(fData.Value["int_type"], null);
				}
			}
			if(int_type != -1)
			{
				return int_type;
			}
			if(0 < MVCFunctions.count(intTypes))
			{
				return intTypes[0];
			}
			return 0;
		}
		protected virtual XVar getDataFieldByParam(dynamic _param_paramField)
		{
			#region pass-by-value parameters
			dynamic paramField = XVar.Clone(_param_paramField);
			#endregion

			dynamic dataFields = XVar.Array();
			if(XVar.Pack(this.fieldsTotalsData[paramField]))
			{
				return paramField;
			}
			dataFields = XVar.Clone(MVCFunctions.array_keys((XVar)(this.fieldsTotalsData)));
			if(XVar.Pack(MVCFunctions.count(dataFields)))
			{
				return dataFields[0];
			}
			return "";
		}
		protected virtual XVar getGroupFieldByParam(dynamic _param_axis, dynamic _param_paramField, dynamic _param_otherField = null)
		{
			#region default values
			if(_param_otherField as Object == null) _param_otherField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic axis = XVar.Clone(_param_axis);
			dynamic paramField = XVar.Clone(_param_paramField);
			dynamic otherField = XVar.Clone(_param_otherField);
			#endregion

			dynamic firstField = null;
			firstField = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> fData in this.groupFieldsData.GetEnumerator())
			{
				if((XVar)(fData.Value["group_type"] == "all")  || (XVar)(fData.Value["group_type"] == axis))
				{
					if(fData.Value["name"] == paramField)
					{
						return paramField;
					}
					if((XVar)(XVar.Equals(XVar.Pack(firstField), XVar.Pack("")))  && (XVar)((XVar)(!(XVar)(otherField))  || (XVar)(!XVar.Equals(XVar.Pack(otherField), XVar.Pack(firstField)))))
					{
						firstField = XVar.Clone(fData.Value["name"]);
					}
				}
			}
			return firstField;
		}
		protected virtual XVar getDataCommand()
		{
			dynamic BeforeQueryReport = null, BeforeQueryReportPrint = null, dc = null, eventObj = null, ftype = null;
			dc = XVar.Clone(this.pageObject.getSubsetDataCommand());
			ftype = XVar.Clone(this.pSet.getFieldType((XVar)(this.dataField)));
			dc.totals.InitAndSetArrayItem(new XVar("field", this.dataField, "total", this.dataGroupFunction, "timeToSec", (XVar)(this.pSet.getViewFormat((XVar)(this.dataField)) == Constants.FORMAT_TIME)  || (XVar)(CommonFunctions.IsTimeType((XVar)(ftype)))), null);
			dc.totals.InitAndSetArrayItem(new XVar("field", this.yFName, "modifier", this.yIntervalType, "direction", this.getGroupOrderDirection((XVar)(this.yFName))), null);
			dc.totals.InitAndSetArrayItem(new XVar("field", this.xFName, "modifier", this.xIntervalType), null);
			if((XVar)(this.dataGroupFunction == "avg")  && (XVar)(!(XVar)(CommonFunctions.IsDateFieldType((XVar)(ftype)))))
			{
				dc.totals.InitAndSetArrayItem(new XVar("field", this.dataField, "alias", "avg_sum", "total", "sum", "timeToSec", (XVar)(this.pSet.getViewFormat((XVar)(this.dataField)) == Constants.FORMAT_TIME)  || (XVar)(CommonFunctions.IsTimeType((XVar)(ftype)))), null);
				dc.totals.InitAndSetArrayItem(new XVar("field", this.dataField, "alias", "avg_count", "total", "count"), null);
			}
			else
			{
				dc.totals.InitAndSetArrayItem(new XVar("alias", "avg_sum", "total", "count"), null);
				dc.totals.InitAndSetArrayItem(new XVar("alias", "avg_count", "total", "count"), null);
			}
			eventObj = XVar.Clone(CommonFunctions.getEventObject((XVar)(this.pSet)));
			BeforeQueryReport = XVar.Clone((XVar)(this.pageType == Constants.PAGE_REPORT)  && (XVar)(eventObj.exists(new XVar("BeforeQueryReport"))));
			BeforeQueryReportPrint = XVar.Clone((XVar)(this.pageType == Constants.PAGE_RPRINT)  && (XVar)(eventObj.exists(new XVar("BeforeQueryReportPrint"))));
			if((XVar)(BeforeQueryReport)  || (XVar)(BeforeQueryReportPrint))
			{
				dynamic prep = XVar.Array(), where = null;
				prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
				where = XVar.Clone(prep["where"]);
				if(XVar.Pack(BeforeQueryReport))
				{
					eventObj.BeforeQueryReport((XVar)(where));
				}
				else
				{
					eventObj.BeforeQueryReportPrint((XVar)(where));
				}
				if(where != prep["where"])
				{
					this.dataSource.overrideWhere((XVar)(dc), (XVar)(where));
				}
			}
			return dc;
		}
		protected virtual XVar getGroupOrderDirection(dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic fieldIdx = null, orderIndices = XVar.Array();
			orderIndices = this.pSet.getOrderIndexes();
			fieldIdx = XVar.Clone(this.pSet.getFieldIndex((XVar)(fName)));
			foreach (KeyValuePair<XVar, dynamic> o in orderIndices.GetEnumerator())
			{
				if(o.Value[0] == fieldIdx)
				{
					return o.Value[1];
				}
			}
			return "ASC";
		}
		public virtual XVar getSelectedValue()
		{
			dynamic arr = XVar.Array(), firstarr = XVar.Array();
			arr = XVar.Clone(XVar.Array());
			firstarr = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in this.fieldsTotalsData.GetEnumerator())
			{
				if(MVCFunctions.count(firstarr) == 0)
				{
					firstarr.InitAndSetArrayItem(value.Value["name"], null);
				}
				if((XVar)((XVar)((XVar)(value.Value["min"] == true)  || (XVar)(value.Value["max"] == true))  || (XVar)(value.Value["sum"] == true))  || (XVar)(value.Value["avg"] == true))
				{
					arr.InitAndSetArrayItem(value.Value["name"], null);
				}
			}
			if(MVCFunctions.count(arr) == 0)
			{
				arr = XVar.Clone(firstarr);
			}
			return arr;
		}
		public virtual XVar getCurrentOperationList()
		{
			dynamic names = XVar.Array(), opData = XVar.Array();
			names = XVar.Clone(XVar.Array());
			names.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SUM")), "sum");
			names.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MIN")), "min");
			names.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MAX")), "max");
			names.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("AVG")), "avg");
			opData = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> n in names.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.dataFieldSettings[n.Key])))
				{
					continue;
				}
				opData.InitAndSetArrayItem(new XVar("value", n.Key, "selected", (XVar.Pack(this.dataGroupFunction == n.Key) ? XVar.Pack("selected") : XVar.Pack("")), "label", n.Value), null);
			}
			return opData;
		}
		public virtual XVar getCrossFieldsData(dynamic _param_axis)
		{
			#region pass-by-value parameters
			dynamic axis = XVar.Clone(_param_axis);
			#endregion

			dynamic dataList = XVar.Array();
			dataList = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> data in this.groupFieldsData.GetEnumerator())
			{
				dynamic intervalType = null, selected = null;
				if((XVar)((XVar)((XVar)(axis != "x")  || (XVar)(data.Value["group_type"] != "x"))  && (XVar)((XVar)(axis != "y")  || (XVar)(data.Value["group_type"] != "y")))  && (XVar)(data.Value["group_type"] != "all"))
				{
					continue;
				}
				selected = new XVar("");
				if((XVar)((XVar)((XVar)(axis == "x")  && (XVar)(data.Value["name"] == this.xFName))  && (XVar)(data.Value["int_type"] == this.xIntervalType))  || (XVar)((XVar)((XVar)(axis == "y")  && (XVar)(data.Value["name"] == this.yFName))  && (XVar)(data.Value["int_type"] == this.yIntervalType)))
				{
					selected = new XVar("selected");
				}
				intervalType = XVar.Clone((XVar.Pack(data.Value["uniqueName"]) ? XVar.Pack("") : XVar.Pack(this.getIntervalParam((XVar)(data.Value["int_type"]), (XVar)(data.Value["name"])))));
				dataList.InitAndSetArrayItem(new XVar("value", data.Value["name"], "selected", selected, "label", data.Value["label"], "intervalType", intervalType), null);
			}
			return dataList;
		}
		protected virtual XVar getRefineIntervalType(dynamic _param_intType, dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic intType = XVar.Clone(_param_intType);
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic ftype = null;
			if(XVar.Equals(XVar.Pack(intType), XVar.Pack(0)))
			{
				return "normal";
			}
			ftype = XVar.Clone(this.pSet.getFieldType((XVar)(fName)));
			if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(ftype))))
			{
				return MVCFunctions.substr((XVar)(intType), new XVar(1));
			}
			if(XVar.Pack(CommonFunctions.IsCharType((XVar)(ftype))))
			{
				return MVCFunctions.substr((XVar)(intType), (XVar)(MVCFunctions.strlen(new XVar("first"))));
			}
			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(ftype))))
			{
				switch(((XVar)intType).ToString())
				{
					case "year":
						return 1;
					case "quarter":
						return 2;
					case "month":
						return 3;
					case "week":
						return 4;
					case "day":
						return 5;
					case "hour":
						return 6;
					case "minute":
						return 7;
				}
			}
			return -1;
		}
		protected virtual XVar getIntervalParam(dynamic _param_intType, dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic intType = XVar.Clone(_param_intType);
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic ftype = null;
			if(intType == XVar.Pack(0))
			{
				return "normal";
			}
			ftype = XVar.Clone(this.pSet.getFieldType((XVar)(fName)));
			if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(ftype))))
			{
				return MVCFunctions.Concat("n", intType);
			}
			if(XVar.Pack(CommonFunctions.IsCharType((XVar)(ftype))))
			{
				return MVCFunctions.Concat("first", intType);
			}
			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(ftype))))
			{
				switch(((XVar)intType).ToInt())
				{
					case 1:
						return "year";
					case 2:
						return "quarter";
					case 3:
						return "month";
					case 4:
						return "week";
					case 5:
						return "day";
					case 6:
						return "hour";
					case 7:
						return "minute";
					default:
						return "";
				}
			}
			return "";
		}
		public virtual XVar getDataFieldsList()
		{
			dynamic listData = XVar.Array();
			listData = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in this.fieldsTotalsData.GetEnumerator())
			{
				if((XVar)((XVar)((XVar)(value.Value["min"] == true)  || (XVar)(value.Value["max"] == true))  || (XVar)(value.Value["sum"] == true))  || (XVar)(value.Value["avg"] == true))
				{
					dynamic selected = null;
					selected = XVar.Clone((XVar.Pack(value.Value["name"] == this.dataField) ? XVar.Pack("selected") : XVar.Pack("")));
					listData.InitAndSetArrayItem(new XVar("value", value.Value["name"], "selected", selected, "label", value.Value["label"]), null);
				}
			}
			return listData;
		}
		public virtual XVar getPrintCrossHeader()
		{
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return this.getPdfCrossHeader();
			}
			return MVCFunctions.Concat("<div>", CommonFunctions.mlang_message(new XVar("GROUP_X")), ":<b>", this.fieldsTotalsData[this.xFName]["label"], "</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", CommonFunctions.mlang_message(new XVar("GROUP_Y")), ":<b>", this.fieldsTotalsData[this.yFName]["label"], "</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", CommonFunctions.mlang_message(new XVar("FIELD")), ":<b>", this.fieldsTotalsData[this.dataField]["label"], "</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", CommonFunctions.mlang_message(new XVar("GROUP_FUNCTION")), ":<b>", this.dataGroupFunction, "</b></div>");
		}
		protected virtual XVar getPdfCrossHeader()
		{
			dynamic parts = XVar.Array();
			parts = XVar.Clone(XVar.Array());
			parts.InitAndSetArrayItem(MVCFunctions.Concat("{ text: '", CommonFunctions.mlang_message(new XVar("GROUP_X")), ":'}"), null);
			parts.InitAndSetArrayItem(MVCFunctions.Concat("{ text: '", this.getXGroupLabel(), "     ', bold: true }"), null);
			parts.InitAndSetArrayItem(MVCFunctions.Concat("{ text: '", CommonFunctions.mlang_message(new XVar("GROUP_Y")), ":'}"), null);
			parts.InitAndSetArrayItem(MVCFunctions.Concat("{ text: '", this.getYGroupLabel(), "     ', bold: true }"), null);
			parts.InitAndSetArrayItem(MVCFunctions.Concat("{ text: '", CommonFunctions.mlang_message(new XVar("FIELD")), ":'}"), null);
			parts.InitAndSetArrayItem(MVCFunctions.Concat("{ text: '", this.getDataFieldLabel(), "     ', bold: true }"), null);
			parts.InitAndSetArrayItem(MVCFunctions.Concat("{ text: '", CommonFunctions.mlang_message(new XVar("GROUP_FUNCTION")), ":'}"), null);
			parts.InitAndSetArrayItem(MVCFunctions.Concat("{ text: '", CommonFunctions.jsreplace((XVar)(this.dataGroupFunction)), "', bold: true }"), null);
			return MVCFunctions.implode((XVar)(parts), new XVar(","));
		}
		public virtual XVar getXGroupLabel()
		{
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(this.fieldsTotalsData[this.xFName]["label"])), "'");
			}
			return this.fieldsTotalsData[this.xFName]["label"];
		}
		public virtual XVar getYGroupLabel()
		{
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(this.fieldsTotalsData[this.yFName]["label"])), "'");
			}
			return this.fieldsTotalsData[this.yFName]["label"];
		}
		public virtual XVar getDataFieldLabel()
		{
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(this.fieldsTotalsData[this.dataField]["label"])), "'");
			}
			return this.fieldsTotalsData[this.dataField]["label"];
		}
		protected virtual XVar _getTotalsName()
		{
			switch(((XVar)this.dataGroupFunction).ToString())
			{
				case "sum":
					return CommonFunctions.mlang_message(new XVar("SUM"));
					break;
				case "min":
					return CommonFunctions.mlang_message(new XVar("MIN"));
					break;
				case "max":
					return CommonFunctions.mlang_message(new XVar("MAX"));
					break;
				case "avg":
					return CommonFunctions.mlang_message(new XVar("AVERAGE"));
					break;
				default:
					return "";
			}

			return null;
		}
		public virtual XVar getTotalsName()
		{
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(this._getTotalsName())), " '");
			}
			return this._getTotalsName();
		}
		protected virtual XVar getDataGroupFunction(dynamic _param_operation)
		{
			#region pass-by-value parameters
			dynamic operation = XVar.Clone(_param_operation);
			#endregion

			dynamic gfuncs = XVar.Array();
			if(this.dataFieldSettings[operation] == true)
			{
				return operation;
			}
			gfuncs = XVar.Clone(new XVar(0, "sum", 1, "max", 2, "min", 3, "avg"));
			foreach (KeyValuePair<XVar, dynamic> gf in gfuncs.GetEnumerator())
			{
				if(this.dataFieldSettings[gf.Value] == true)
				{
					return gf.Value;
				}
			}
			return "sum";
		}
		public virtual XVar getCurrentGroupFunction()
		{
			return this.dataGroupFunction;
		}
		public static XVar getCrossIntervalName(dynamic _param_ftype, dynamic _param_int_type)
		{
			#region pass-by-value parameters
			dynamic ftype = XVar.Clone(_param_ftype);
			dynamic int_type = XVar.Clone(_param_int_type);
			#endregion

			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(ftype))))
			{
				if(int_type == 1)
				{
					return CommonFunctions.mlang_message(new XVar("CCR_YEAR"));
				}
				if(int_type == 2)
				{
					return CommonFunctions.mlang_message(new XVar("CCR_QUARTER"));
				}
				if(int_type == 3)
				{
					return CommonFunctions.mlang_message(new XVar("CCR_MONTH"));
				}
				if(int_type == 4)
				{
					return CommonFunctions.mlang_message(new XVar("CCR_WEEK"));
				}
				if(int_type == 5)
				{
					return CommonFunctions.mlang_message(new XVar("CCR_DAY"));
				}
				if(int_type == 6)
				{
					return CommonFunctions.mlang_message(new XVar("CCR_HOUR"));
				}
				if(int_type == 7)
				{
					return CommonFunctions.mlang_message(new XVar("CCR_MINUTE"));
				}
			}
			return int_type;
		}
		protected virtual XVar pdfJsonMode()
		{
			return this.pdfJSON;
		}
		public virtual XVar showDBValue(dynamic _param_field, dynamic data)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic control = null;
			control = XVar.Clone(this.viewControls.getControl((XVar)(field)));
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return control.getPdfValue((XVar)(data), new XVar(""));
			}
			return control.showDBValue((XVar)(data), new XVar(""));
		}
		protected virtual XVar getFreeAxis()
		{
			dynamic xCount = null, yCount = null;
			if(XVar.Pack(this.selectedAxis))
			{
				return (XVar.Pack(this.selectedAxis == "y") ? XVar.Pack("x") : XVar.Pack("y"));
			}
			xCount = new XVar(0);
			yCount = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> fData in this.groupFieldsData.GetEnumerator())
			{
				if(fData.Value["group_type"] == "all")
				{
					++(xCount);
					++(yCount);
				}
				else
				{
					if(fData.Value["group_type"] == "x")
					{
						++(xCount);
					}
					else
					{
						++(yCount);
					}
				}
				if(1 < xCount)
				{
					return "x";
				}
				if(1 < yCount)
				{
					return "y";
				}
			}
			return "";
		}
		protected virtual XVar correctCrosstabParams()
		{
			dynamic freeAxis = null;
			if((XVar)(this.xFName != this.yFName)  || (XVar)(this.xIntervalType != this.yIntervalType))
			{
				return null;
			}
			freeAxis = XVar.Clone(this.getFreeAxis());
			if(XVar.Pack(!(XVar)(freeAxis)))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> fData in this.groupFieldsData.GetEnumerator())
			{
				if((XVar)(fData.Value["group_type"] == "all")  || (XVar)(fData.Value["group_type"] == freeAxis))
				{
					if((XVar)(!XVar.Equals(XVar.Pack(this.xFName), XVar.Pack(fData.Value["name"])))  || (XVar)(!XVar.Equals(XVar.Pack(this.xIntervalType), XVar.Pack(fData.Value["int_type"]))))
					{
						if(XVar.Equals(XVar.Pack(freeAxis), XVar.Pack("x")))
						{
							this.xFName = XVar.Clone(fData.Value["name"]);
							this.xIntervalType = XVar.Clone(fData.Value["int_type"]);
						}
						else
						{
							this.yFName = XVar.Clone(fData.Value["name"]);
							this.yIntervalType = XVar.Clone(fData.Value["int_type"]);
						}
						break;
					}
				}
			}

			return null;
		}
	}
}
