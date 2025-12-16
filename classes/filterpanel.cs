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
	public partial class FilterPanel : XClass
	{
		public dynamic pageObj;
		protected dynamic id;
		protected XTempl xt;
		protected dynamic tName;
		protected dynamic filterFileds;
		protected dynamic filterStates;
		protected dynamic viewControls;
		public FilterPanel(dynamic var_params)
		{
			CommonFunctions.RunnerApply(this, (XVar)(var_params));
			this.id = XVar.Clone(this.pageObj.id);
			this.xt = XVar.UnPackXTempl(this.pageObj.xt);
			this.tName = XVar.Clone(this.pageObj.tName);
			this.filterFileds = XVar.Clone(this.pageObj.pSet.getFilterFields());
			this.viewControls = XVar.Clone(new ViewControlsContainer((XVar)(this.pageObj.pSet), new XVar(Constants.PAGE_LIST)));
			this.xt.enable_section(new XVar("filterPanel"));
			if(XVar.Pack(!(XVar)(this.pageObj.controlsMap["filters"])))
			{
				this.pageObj.controlsMap.InitAndSetArrayItem(new XVar("controls", XVar.Array()), "filters");
			}
		}
		public virtual XVar buildFilterPanel()
		{
			dynamic panelVisible = null;
			panelVisible = new XVar(false);
			foreach (KeyValuePair<XVar, dynamic> fieldName in this.filterFileds.GetEnumerator())
			{
				dynamic filterButtonParams = null, filterControl = null, filterCtrlBlocks = null, filterExtraControls = null, filterFieldName = null, filterState = XVar.Array();
				filterFieldName = XVar.Clone(fieldName.Value);
				filterControl = XVar.Clone(FilterControl.getFilterControl((XVar)(filterFieldName), (XVar)(this.pageObj), (XVar)(this.id), (XVar)(this.viewControls)));
				if(XVar.Pack(!(XVar)(filterControl)))
				{
					continue;
				}
				if(XVar.Pack(filterControl.hasDependentFilter()))
				{
					continue;
				}
				filterCtrlBlocks = XVar.Clone(filterControl.buildFilterCtrlBlockArray((XVar)(this.pageObj)));
				filterButtonParams = XVar.Clone(filterControl.getFilterButtonParams());
				filterExtraControls = XVar.Clone(filterControl.getFilterExtraControls());
				while(XVar.Pack(filterControl.dependent))
				{
					filterFieldName = XVar.Clone(filterControl.parentFilterName);
					filterControl = XVar.Clone(FilterControl.getFilterControl((XVar)(filterFieldName), (XVar)(this.pageObj), (XVar)(this.id), (XVar)(this.viewControls)));
					if(XVar.Pack(!(XVar)(filterControl)))
					{
						continue;
					}
					filterCtrlBlocks = XVar.Clone(filterControl.buildFilterCtrlBlockArray((XVar)(this.pageObj), (XVar)(filterCtrlBlocks)));
					filterButtonParams = XVar.Clone(filterControl.getFilterButtonParams((XVar)(filterButtonParams)));
					filterExtraControls = XVar.Clone(filterControl.getFilterExtraControls((XVar)(filterExtraControls)));
				}
				filterState = XVar.Clone(filterControl.getFilterState());
				if(XVar.Pack(filterState["visible"]))
				{
					panelVisible = new XVar(true);
				}
				this.assignFilterPanelField((XVar)(filterFieldName), (XVar)(filterCtrlBlocks), (XVar)(filterState), (XVar)(filterButtonParams), (XVar)(filterExtraControls));
			}
			return panelVisible;
		}
		protected virtual XVar assignFilterPanelField(dynamic _param_fieldName, dynamic _param_filterCtrlBlocks, dynamic _param_filterState, dynamic _param_filterButtonParams, dynamic _param_filterExtraControls)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			dynamic filterCtrlBlocks = XVar.Clone(_param_filterCtrlBlocks);
			dynamic filterState = XVar.Clone(_param_filterState);
			dynamic filterButtonParams = XVar.Clone(_param_filterButtonParams);
			dynamic filterExtraControls = XVar.Clone(_param_filterExtraControls);
			#endregion

			dynamic postfix = null, visibility = null;
			postfix = XVar.Clone(MVCFunctions.Concat("_", MVCFunctions.GoodFieldName((XVar)(fieldName))));
			visibility = XVar.Clone(filterState["visible"]);
			this.xt.assign((XVar)(MVCFunctions.Concat("filter_control", postfix)), (XVar)(visibility));
			if(XVar.Pack(!(XVar)(visibility)))
			{
				return null;
			}
			this.xt.assign_loopsection_byValue((XVar)(MVCFunctions.Concat("filterCtrlBlock", postfix)), (XVar)(filterCtrlBlocks));
			this.xt.assign((XVar)(MVCFunctions.Concat("collapsedClass", postfix)), new XVar("filter-collapsed"));
			this.xt.assign((XVar)(MVCFunctions.Concat("filterbutton_attrs", postfix)), (XVar)(filterButtonParams["attrs"]));
			this.xt.assign((XVar)(MVCFunctions.Concat("filter_button_apply", postfix)), (XVar)(filterButtonParams["hasApplyBtn"]));
			this.xt.assign((XVar)(MVCFunctions.Concat("filter_button_multiselect", postfix)), (XVar)(filterButtonParams["hasMultiselectBtn"]));
			this.xt.assign((XVar)(MVCFunctions.Concat("clearLink", postfix)), (XVar)(filterExtraControls["filtered"]));
			this.xt.assign((XVar)(MVCFunctions.Concat("filter_selected", postfix)), (XVar)(filterExtraControls["filtered"]));
			this.xt.assign((XVar)(MVCFunctions.Concat("filtervalue", postfix)), (XVar)(filterExtraControls["showValue"]));
			this.xt.assign((XVar)(MVCFunctions.Concat("selectAll_attrs", postfix)), (XVar)(filterExtraControls["selectAllAttrs"]));
			this.xt.assign((XVar)(MVCFunctions.Concat("filter_button_showmore", postfix)), (XVar)(filterState["truncated"]));
			this.xt.assign((XVar)(MVCFunctions.Concat("show_n_more", postfix)), (XVar)(MVCFunctions.str_replace(new XVar("%n%"), (XVar)(filterExtraControls["numberOfExtraItemsToShow"]), (XVar)(CommonFunctions.mlang_message(new XVar("SHOW_N_MORE"))))));
			if(XVar.Pack(filterState["showMoreHidden"]))
			{
				this.xt.assign((XVar)(MVCFunctions.Concat("showMoreBtnClass", postfix)), new XVar("show-more-hidden"));
			}

			return null;
		}
	}
}
