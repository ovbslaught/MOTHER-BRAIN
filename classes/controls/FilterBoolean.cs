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
	public partial class FilterBoolean : FilterControl
	{
		protected static bool skipFilterBooleanCtor = false;
		public FilterBoolean(dynamic _param_fName, dynamic _param_pageObject, dynamic _param_id, dynamic _param_viewControls)
			:base((XVar)_param_fName, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_viewControls)
		{
			if(skipFilterBooleanCtor)
			{
				skipFilterBooleanCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic viewControls = XVar.Clone(_param_viewControls);
			#endregion

			this.separator = new XVar("~checked~");
			this.filterFormat = new XVar(Constants.FF_BOOLEAN);
			if((XVar)(this.totals == Constants.FT_NONE)  || (XVar)(this.totals == Constants.FT_COUNT))
			{
				this.totalsfName = XVar.Clone(this.fName);
			}
		}
		protected override XVar getValueToShow(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return this.getShownValue((XVar)(value == "on"));
		}
		protected virtual XVar getShownValue(dynamic _param_checked)
		{
			#region pass-by-value parameters
			dynamic var_checked = XVar.Clone(_param_checked);
			#endregion

			dynamic mData = null;
			mData = XVar.Clone(this.pSet.getBooleanFilterMessageData((XVar)(this.fName), (XVar)(var_checked)));
			return CommonFunctions.GetMLString((XVar)(mData));
		}
		protected virtual XVar getDataCommand()
		{
			dynamic dc = null, total = null, values = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.pageObject.getDataSourceFilterCriteria((XVar)(this.fName)));
			total = XVar.Clone(this.dataTotalsName());
			if(XVar.Pack(!(XVar)(total)))
			{
				total = new XVar("count");
			}
			values = XVar.Clone(new XVar(0, "on", 1, "off"));
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				dynamic caseExpr = null;
				if(total == "count")
				{
					caseExpr = XVar.Clone(DataCondition.CaseConstOrNull((XVar)(FilterBoolean.getFilterCondition((XVar)(this.fName), (XVar)(v.Value), (XVar)(this.pSet))), new XVar(1)));
				}
				else
				{
					caseExpr = XVar.Clone(DataCondition.CaseFieldOrNull((XVar)(FilterBoolean.getFilterCondition((XVar)(this.fName), (XVar)(v.Value), (XVar)(this.pSet))), (XVar)(this.totalsfName)));
				}
				dc.totals.InitAndSetArrayItem(new XVar("total", total, "alias", MVCFunctions.Concat(v.Value, "_", total), "caseStatement", caseExpr), null);
				if(total != "count")
				{
					dc.totals.InitAndSetArrayItem(new XVar("total", "count", "alias", MVCFunctions.Concat(v.Value, "_count"), "caseStatement", DataCondition.CaseConstOrNull((XVar)(FilterBoolean.getFilterCondition((XVar)(this.fName), (XVar)(v.Value), (XVar)(this.pSet))), new XVar(1))), null);
				}
			}
			return dc;
		}
		protected override XVar addFilterBlocksFromDB(dynamic filterCtrlBlocks)
		{
			dynamic data = XVar.Array(), qResult = null, values = XVar.Array();
			qResult = XVar.Clone(this.dataSource.getTotals((XVar)(this.getDataCommand())));
			data = XVar.Clone(qResult.fetchAssoc());
			this.decryptDataRow((XVar)(data));
			if((XVar)(data["on_count"] == 0)  && (XVar)(data["off_count"] == 0))
			{
				return null;
			}
			values = XVar.Clone(new XVar(0, "on", 1, "off"));
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				dynamic ctrlData = XVar.Array(), filterControl = null;
				ctrlData = XVar.Clone(new XVar("value", v.Value));
				if(XVar.Pack(this.dataTotalsName()))
				{
					ctrlData.InitAndSetArrayItem(data[MVCFunctions.Concat(v.Value, "_", this.dataTotalsName())], "total");
				}
				filterControl = XVar.Clone(this.buildControl((XVar)(ctrlData)));
				filterCtrlBlocks.InitAndSetArrayItem(this.getFilterBlockStructure((XVar)(filterControl)), null);
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
			showValue = XVar.Clone(this.getShownValue((XVar)(data["value"] == "on")));
			totalValue = XVar.Clone(this.getTotalValueToShow((XVar)(data["total"])));
			return this.getControlHTML((XVar)(data["value"]), (XVar)(showValue), (XVar)(data["value"]), (XVar)(totalValue), (XVar)(this.separator));
		}
		public static XVar getFilterCondition(dynamic _param_fName, dynamic _param_value, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic value = XVar.Clone(_param_value);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			return CheckboxField.getFieldCondition((XVar)(fName), (XVar)(value));
		}
	}
}
