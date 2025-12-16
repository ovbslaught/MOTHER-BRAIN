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
	public partial class FilterIntervalList : FilterControl
	{
		protected static bool skipFilterIntervalListCtor = false;
		public FilterIntervalList(dynamic _param_fName, dynamic _param_pageObject, dynamic _param_id, dynamic _param_viewControls)
			:base((XVar)_param_fName, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_viewControls)
		{
			if(skipFilterIntervalListCtor)
			{
				skipFilterIntervalListCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic viewControls = XVar.Clone(_param_viewControls);
			#endregion

			this.separator = new XVar("~interval~");
			this.filterFormat = new XVar(Constants.FF_INTERVAL_LIST);
			if((XVar)(this.totals == Constants.FT_NONE)  || (XVar)(this.totals == Constants.FT_COUNT))
			{
				this.totalsfName = XVar.Clone(this.fName);
			}
			this.useApllyBtn = XVar.Clone(this.multiSelect == Constants.FM_ALWAYS);
		}
		protected virtual XVar getDataCommand()
		{
			dynamic dc = null, intervalsRowData = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.pageObject.getDataSourceFilterCriteria((XVar)(this.fName)));
			intervalsRowData = XVar.Clone(this.pSet.getFilterIntervals((XVar)(this.fName)));
			foreach (KeyValuePair<XVar, dynamic> intervalData in intervalsRowData.GetEnumerator())
			{
				dynamic caseExpr = null, idx = null, total = null;
				idx = XVar.Clone(intervalData.Value["index"]);
				total = XVar.Clone(this.dataTotalsName());
				if(XVar.Pack(!(XVar)(total)))
				{
					total = new XVar("count");
				}
				if(total == "count")
				{
					caseExpr = XVar.Clone(DataCondition.CaseConstOrNull((XVar)(FilterIntervalList.getFilterCondition((XVar)(this.fName), (XVar)(idx), (XVar)(this.pSet))), new XVar(1)));
				}
				else
				{
					caseExpr = XVar.Clone(DataCondition.CaseFieldOrNull((XVar)(FilterIntervalList.getFilterCondition((XVar)(this.fName), (XVar)(idx), (XVar)(this.pSet))), (XVar)(this.totalsfName)));
				}
				dc.totals.InitAndSetArrayItem(new XVar("total", total, "alias", MVCFunctions.Concat(this.fName, idx, "_", total), "caseStatement", caseExpr), null);
				if(total != "count")
				{
					dc.totals.InitAndSetArrayItem(new XVar("total", "count", "alias", MVCFunctions.Concat(this.fName, idx, "_count"), "caseStatement", DataCondition.CaseConstOrNull((XVar)(FilterIntervalList.getFilterCondition((XVar)(this.fName), (XVar)(idx), (XVar)(this.pSet))), new XVar(1))), null);
				}
			}
			return dc;
		}
		protected override XVar getValueToShow(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return this.getIntervalLabel((XVar)(value));
		}
		protected virtual XVar getIntervalLabel(dynamic _param_index)
		{
			#region pass-by-value parameters
			dynamic index = XVar.Clone(_param_index);
			#endregion

			dynamic iData = XVar.Array();
			iData = XVar.Clone(this.pSet.getFilterIntervalDatabyIndex((XVar)(this.fName), (XVar)(index)));
			return CommonFunctions.GetMLString((XVar)(iData["mutilangIntervalText"]));
		}
		protected override XVar addFilterBlocksFromDB(dynamic filterCtrlBlocks)
		{
			dynamic data = XVar.Array(), intervalsRowData = XVar.Array(), qResult = null, visibilityClass = null;
			visibilityClass = XVar.Clone((XVar.Pack((XVar)(this.filtered)  && (XVar)(this.multiSelect == Constants.FM_ON_DEMAND)) ? XVar.Pack(this.onDemandHiddenItemClassName) : XVar.Pack("")));
			qResult = XVar.Clone(this.dataSource.getTotals((XVar)(this.getDataCommand())));
			data = XVar.Clone(qResult.fetchAssoc());
			this.decryptDataRow((XVar)(data));
			intervalsRowData = XVar.Clone(this.pSet.getFilterIntervals((XVar)(this.fName)));
			foreach (KeyValuePair<XVar, dynamic> iData in intervalsRowData.GetEnumerator())
			{
				dynamic ctrlData = XVar.Array(), filterControl = null;
				if((XVar)(!(XVar)(this.pSet.showWithNoRecords((XVar)(this.fName))))  && (XVar)(data[MVCFunctions.Concat(this.fName, iData.Value["index"], "_count")] == 0))
				{
					continue;
				}
				this.valuesObtainedFromDB.InitAndSetArrayItem(iData.Value["index"], null);
				ctrlData = XVar.Clone(XVar.Array());
				ctrlData.InitAndSetArrayItem(iData.Value["index"], "index");
				if(XVar.Pack(this.dataTotalsName()))
				{
					ctrlData.InitAndSetArrayItem(data[MVCFunctions.Concat(this.fName, iData.Value["index"], "_", this.dataTotalsName())], "total");
				}
				filterControl = XVar.Clone(this.buildControl((XVar)(ctrlData)));
				filterCtrlBlocks.InitAndSetArrayItem(this.getFilterBlockStructure((XVar)(filterControl), (XVar)(visibilityClass), (XVar)(iData.Value["index"])), null);
			}

			return null;
		}
		protected override XVar getFilterBlockStructure(dynamic _param_filterControl, dynamic _param_visibilityClass = null, dynamic _param_value = null, dynamic _param_parentFiltersData = null)
		{
			#region default values
			if(_param_visibilityClass as Object == null) _param_visibilityClass = new XVar("");
			if(_param_value as Object == null) _param_value = new XVar("");
			if(_param_parentFiltersData as Object == null) _param_parentFiltersData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic filterControl = XVar.Clone(_param_filterControl);
			dynamic visibilityClass = XVar.Clone(_param_visibilityClass);
			dynamic value = XVar.Clone(_param_value);
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			if(this.multiSelect != Constants.FM_ALWAYS)
			{
				visibilityClass = MVCFunctions.Concat(visibilityClass, " filter-link");
			}
			return new XVar(MVCFunctions.Concat(this.gfName, "_filter"), filterControl, MVCFunctions.Concat("visibilityClass_", this.gfName), visibilityClass, "sortValue", value);
		}
		protected override XVar sortFilterBlocks(ref dynamic filterCtrlBlocks)
		{
			MVCFunctions.usort((XVar)(filterCtrlBlocks), (XVar)(new XVar(0, "FilterControl", 1, "compareBlocksByNumericValues")));

			return null;
		}
		protected override XVar addOutRangeValuesToFilter(dynamic filterCtrlBlocks)
		{
			dynamic visibilityClass = null;
			visibilityClass = XVar.Clone((XVar.Pack(this.multiSelect == Constants.FM_ON_DEMAND) ? XVar.Pack(this.onDemandHiddenItemClassName) : XVar.Pack("")));
			foreach (KeyValuePair<XVar, dynamic> index in this.filteredFields[this.fName]["values"].GetEnumerator())
			{
				dynamic filterControl = null;
				if(XVar.Pack(MVCFunctions.in_array((XVar)(index.Value), (XVar)(this.valuesObtainedFromDB))))
				{
					continue;
				}
				filterControl = XVar.Clone(this.buildControl((XVar)(new XVar("index", index.Value))));
				filterCtrlBlocks.InitAndSetArrayItem(this.getFilterBlockStructure((XVar)(filterControl), (XVar)(visibilityClass), (XVar)(index.Value)), null);
			}

			return null;
		}
		protected virtual XVar buildControl(dynamic _param_data, dynamic _param_parentFiltersData = null)
		{
			#region default values
			if(_param_parentFiltersData as Object == null) _param_parentFiltersData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			dynamic showValue = null, totalValue = null;
			showValue = XVar.Clone(this.getIntervalLabel((XVar)(data["index"])));
			totalValue = XVar.Clone(this.getTotalValueToShow((XVar)(data["total"])));
			return this.getControlHTML((XVar)(data["index"]), (XVar)(showValue), (XVar)(data["index"]), (XVar)(totalValue), (XVar)(this.separator));
		}
		public static XVar getOrdinaryIntervalCondition(dynamic _param_fName, dynamic _param_intervalData, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic intervalData = XVar.Clone(_param_intervalData);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic caseInsensitive = null, lowerCondition = null, lowerLimit = null, upperCondition = null, upperLimit = null;
			lowerCondition = new XVar(null);
			caseInsensitive = XVar.Clone((XVar.Pack(intervalData["caseInsensitiveFiltering"]) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_DEFAULT)));
			lowerLimit = XVar.Clone(intervalData["lowerLimit"]);
			if(XVar.Pack(intervalData["lowerUsesExpression"]))
			{
				lowerLimit = XVar.Clone(CommonFunctions.getIntervalLimitsExpressions((XVar)(pSet), (XVar)(fName), (XVar)(intervalData["index"]), new XVar(true)));
			}
			if(intervalData["lowerLimitType"] == Constants.FIL_MORE_THAN)
			{
				lowerCondition = XVar.Clone(DataCondition.FieldIs((XVar)(fName), new XVar(Constants.dsopMORE), (XVar)(lowerLimit), (XVar)(caseInsensitive)));
			}
			else
			{
				if(intervalData["lowerLimitType"] == Constants.FIL_MORE_THAN_OR_EQUAL)
				{
					lowerCondition = XVar.Clone(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(fName), new XVar(Constants.dsopLESS), (XVar)(lowerLimit), (XVar)(caseInsensitive)))));
				}
			}
			upperCondition = new XVar(null);
			upperLimit = XVar.Clone(intervalData["upperLimit"]);
			if(XVar.Pack(intervalData["upperUsesExpression"]))
			{
				upperLimit = XVar.Clone(CommonFunctions.getIntervalLimitsExpressions((XVar)(pSet), (XVar)(fName), (XVar)(intervalData["index"]), new XVar(false)));
			}
			if(intervalData["upperLimitType"] == Constants.FIL_LESS_THAN)
			{
				upperCondition = XVar.Clone(DataCondition.FieldIs((XVar)(fName), new XVar(Constants.dsopLESS), (XVar)(upperLimit), (XVar)(caseInsensitive)));
			}
			else
			{
				if(intervalData["upperLimitType"] == Constants.FIL_LESS_THAN_OR_EQUAL)
				{
					upperCondition = XVar.Clone(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(fName), new XVar(Constants.dsopMORE), (XVar)(upperLimit), (XVar)(caseInsensitive)))));
				}
			}
			if((XVar)(lowerCondition)  && (XVar)(upperCondition))
			{
				return DataCondition._And((XVar)(new XVar(0, lowerCondition, 1, upperCondition)));
			}
			if(XVar.Pack(lowerCondition))
			{
				return lowerCondition;
			}
			if(XVar.Pack(upperCondition))
			{
				return upperCondition;
			}
			return null;
		}
		public static XVar getFilterCondition(dynamic _param_fName, dynamic _param_index, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic index = XVar.Clone(_param_index);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic intervalData = XVar.Array();
			intervalData = XVar.Clone(pSet.getFilterIntervalDatabyIndex((XVar)(fName), (XVar)(index)));
			if(XVar.Pack(!(XVar)(intervalData)))
			{
				return null;
			}
			if(XVar.Pack(intervalData["remainder"]))
			{
				dynamic conditions = XVar.Array();
				conditions = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> _intervalData in pSet.getFilterIntervals((XVar)(fName)).GetEnumerator())
				{
					if(_intervalData.Value["index"] == index)
					{
						continue;
					}
					if(XVar.Pack(_intervalData.Value["noLimits"]))
					{
						return DataCondition._False();
					}
					conditions.InitAndSetArrayItem(DataCondition._Not((XVar)(FilterIntervalList.getOrdinaryIntervalCondition((XVar)(fName), (XVar)(_intervalData.Value), (XVar)(pSet)))), null);
				}
				return DataCondition._And((XVar)(conditions));
			}
			if(XVar.Pack(intervalData["noLimits"]))
			{
				return DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(fName), new XVar(Constants.dsopEMPTY), (XVar)(index))));
			}
			return FilterIntervalList.getOrdinaryIntervalCondition((XVar)(fName), (XVar)(intervalData), (XVar)(pSet));
		}
	}
}
