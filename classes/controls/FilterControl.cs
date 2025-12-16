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
	public partial class FilterControl : XClass
	{
		protected dynamic id;
		protected dynamic fName;
		protected dynamic gfName;
		protected dynamic tName;
		protected ProjectSettings pSet;
		protected dynamic totals;
		protected dynamic useTotals;
		protected dynamic multiSelect;
		protected dynamic cipherer;
		protected dynamic filteredFields;
		protected dynamic filtered = XVar.Pack(false);
		protected dynamic totalsfName;
		protected dynamic strSQL;
		protected dynamic viewControl;
		protected dynamic visible = XVar.Pack(true);
		protected dynamic filterFormat;
		protected dynamic useApllyBtn = XVar.Pack(false);
		protected dynamic separator = XVar.Pack("~~");
		protected dynamic totalViewControl;
		protected dynamic showCollapsed = XVar.Pack(false);
		protected dynamic whereComponents;
		protected dynamic fieldType;
		protected dynamic valuesObtainedFromDB = XVar.Array();
		protected dynamic onDemandHiddenItemClassName = XVar.Pack("filter-hidden");
		protected dynamic connection;
		protected dynamic dataSource;
		public dynamic dependent = XVar.Pack(false);
		public dynamic parentFilterName = XVar.Pack("");
		public dynamic pageObject;
		protected dynamic parentFiltersNames = XVar.Array();
		public FilterControl(dynamic _param_fName, dynamic _param_pageObj, dynamic _param_id, dynamic _param_viewControls)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic pageObj = XVar.Clone(_param_pageObj);
			dynamic id = XVar.Clone(_param_id);
			dynamic viewControls = XVar.Clone(_param_viewControls);
			#endregion

			this.pageObject = XVar.Clone(pageObj);
			this.id = XVar.Clone(id);
			this.fName = XVar.Clone(fName);
			this.gfName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(this.fName)));
			this.tName = XVar.Clone(pageObj.tName);
			this.connection = XVar.Clone(pageObj.connection);
			this.dataSource = XVar.Clone(pageObj.getDataSource());
			this.pSet = XVar.UnPackProjectSettings(pageObj.pSet);
			this.cipherer = XVar.Clone(pageObj.cipherer);
			this.totals = XVar.Clone(this.pSet.getFilterFieldTotal((XVar)(fName)));
			this.totalsfName = XVar.Clone(this.pSet.getFilterTotalsField((XVar)(fName)));
			if((XVar)(!(XVar)(this.totalsfName))  || (XVar)(this.totals == Constants.FT_COUNT))
			{
				this.totalsfName = XVar.Clone(this.fName);
			}
			this.useTotals = XVar.Clone(this.totals != Constants.FT_NONE);
			this.multiSelect = XVar.Clone(this.pSet.getFilterFiledMultiSelect((XVar)(fName)));
			this.filteredFields = XVar.Clone(pageObj.searchClauseObj.getFilteredFields());
			this.fieldType = XVar.Clone(this.pSet.getFieldType((XVar)(this.fName)));
			if(XVar.Pack(!(XVar)(!(XVar)(this.filteredFields[this.fName]))))
			{
				this.filtered = new XVar(true);
			}
			this.assignViewControls((XVar)(viewControls));
			this.showCollapsed = XVar.Clone(this.pSet.showCollapsed((XVar)(this.fName)));
		}
		protected virtual XVar assignViewControls(dynamic _param_viewControls)
		{
			#region pass-by-value parameters
			dynamic viewControls = XVar.Clone(_param_viewControls);
			#endregion

			if(XVar.Pack(!(XVar)(viewControls)))
			{
				return null;
			}
			this.viewControl = XVar.Clone(viewControls.getControl((XVar)(this.fName)));
			this.viewControl.searchHighlight = new XVar(false);
			this.viewControl.isUsedForFilter = new XVar(true);
			if((XVar)(this.totals == Constants.FT_MIN)  || (XVar)(this.totals == Constants.FT_MAX))
			{
				this.totalViewControl = XVar.Clone(viewControls.getControl((XVar)(this.totalsfName)));
				this.totalViewControl.searchHighlight = new XVar(false);
				this.totalViewControl.isUsedForFilter = new XVar(true);
			}

			return null;
		}
		public virtual XVar addFilterControlToControlsMap(dynamic _param_pageObj)
		{
			#region pass-by-value parameters
			dynamic pageObj = XVar.Clone(_param_pageObj);
			#endregion

			dynamic ctrlsMap = null;
			ctrlsMap = XVar.Clone(this.getBaseContolsMapParams());
			pageObj.controlsMap.InitAndSetArrayItem(ctrlsMap, "filters", "controls", null);

			return null;
		}
		protected virtual XVar getBaseContolsMapParams()
		{
			dynamic ctrlsMap = XVar.Array();
			ctrlsMap = XVar.Clone(XVar.Array());
			ctrlsMap.InitAndSetArrayItem(this.fName, "fieldName");
			ctrlsMap.InitAndSetArrayItem(this.gfName, "gfieldName");
			ctrlsMap.InitAndSetArrayItem(this.filterFormat, "filterFormat");
			ctrlsMap.InitAndSetArrayItem(this.multiSelect, "multiSelect");
			ctrlsMap.InitAndSetArrayItem(this.filtered, "filtered");
			ctrlsMap.InitAndSetArrayItem(this.separator, "separator");
			ctrlsMap.InitAndSetArrayItem(this.showCollapsed, "collapsed");
			if(XVar.Pack(this.filtered))
			{
				ctrlsMap.InitAndSetArrayItem(this.filteredFields[this.fName]["values"], "defaultValuesArray");
				ctrlsMap.InitAndSetArrayItem(XVar.Array(), "defaultShowValues");
				foreach (KeyValuePair<XVar, dynamic> dv in ctrlsMap["defaultValuesArray"].GetEnumerator())
				{
					ctrlsMap.InitAndSetArrayItem(this.getValueToShow((XVar)(dv.Value)), "defaultShowValues", null);
				}
			}
			return ctrlsMap;
		}
		protected virtual XVar getValueToShow(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion


			return null;
		}
		protected virtual XVar getTotalValueToShow(dynamic _param_totalValue)
		{
			#region pass-by-value parameters
			dynamic totalValue = XVar.Clone(_param_totalValue);
			#endregion

			if((XVar)(this.totals == Constants.FT_MIN)  || (XVar)(this.totals == Constants.FT_MAX))
			{
				dynamic totalData = null;
				totalData = XVar.Clone(new XVar(this.totalsfName, totalValue));
				totalValue = XVar.Clone(this.totalViewControl.showDBValue((XVar)(totalData), new XVar("")));
			}
			return totalValue;
		}
		public virtual XVar buildFilterCtrlBlockArray(dynamic _param_pageObj, dynamic _param_dFilterBlocks = null)
		{
			#region default values
			if(_param_dFilterBlocks as Object == null) _param_dFilterBlocks = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic pageObj = XVar.Clone(_param_pageObj);
			dynamic dFilterBlocks = XVar.Clone(_param_dFilterBlocks);
			#endregion

			dynamic filterCtrlBlocks = null;
			this.addFilterControlToControlsMap((XVar)(pageObj));
			filterCtrlBlocks = XVar.Clone(XVar.Array());
			if((XVar)(this.multiSelect != Constants.FM_ALWAYS)  && (XVar)(this.filtered))
			{
				filterCtrlBlocks = XVar.Clone(this.getFilteredFilterBlocks());
				if(this.multiSelect == Constants.FM_NONE)
				{
					return filterCtrlBlocks;
				}
			}
			this.addFilterBlocksFromDB((XVar)(filterCtrlBlocks));
			if((XVar)(this.multiSelect != Constants.FM_NONE)  && (XVar)(this.filtered))
			{
				this.addOutRangeValuesToFilter((XVar)(filterCtrlBlocks));
			}
			if(XVar.Pack(!(XVar)(filterCtrlBlocks)))
			{
				this.visible = new XVar(false);
			}
			this.extraBlocksProcessing(ref filterCtrlBlocks);
			return filterCtrlBlocks;
		}
		protected virtual XVar extraBlocksProcessing(ref dynamic filterCtrlBlocks)
		{
			this.sortFilterBlocks(ref filterCtrlBlocks);

			return null;
		}
		protected virtual XVar sortFilterBlocks(ref dynamic filterCtrlBlocks)
		{

			return null;
		}
		protected virtual XVar isTruncated()
		{
			return false;
		}
		public static XVar compareBlocksByNumericValues(dynamic _param_block1, dynamic _param_block2)
		{
			#region pass-by-value parameters
			dynamic block1 = XVar.Clone(_param_block1);
			dynamic block2 = XVar.Clone(_param_block2);
			#endregion

			if(block1["sortValue"] < block2["sortValue"])
			{
				return -1;
			}
			if(block2["sortValue"] < block1["sortValue"])
			{
				return 1;
			}
			return 0;
		}
		public static XVar compareBlocksByStringValues(dynamic _param_block1, dynamic _param_block2)
		{
			#region pass-by-value parameters
			dynamic block1 = XVar.Clone(_param_block1);
			dynamic block2 = XVar.Clone(_param_block2);
			#endregion

			dynamic caseCompareResult = null, sortValue1 = null, sortValue2 = null;
			sortValue1 = XVar.Clone(XVar.Pack(block1["sortValue"]).ToString());
			sortValue2 = XVar.Clone(XVar.Pack(block2["sortValue"]).ToString());
			caseCompareResult = XVar.Clone(MVCFunctions.strcasecmp((XVar)(sortValue1), (XVar)(sortValue2)));
			if(caseCompareResult == XVar.Pack(0))
			{
				return -MVCFunctions.strcmp((XVar)(sortValue1), (XVar)(sortValue2));
			}
			return caseCompareResult;
		}
		protected virtual XVar addOutRangeValuesToFilter(dynamic filterCtrlBlocks)
		{
			dynamic visibilityClass = null;
			visibilityClass = XVar.Clone((XVar.Pack(this.multiSelect == Constants.FM_ON_DEMAND) ? XVar.Pack(this.onDemandHiddenItemClassName) : XVar.Pack("")));
			foreach (KeyValuePair<XVar, dynamic> value in this.filteredFields[this.fName]["values"].GetEnumerator())
			{
				dynamic filterControl = null;
				if(XVar.Pack(MVCFunctions.in_array((XVar)(value.Value), (XVar)(this.valuesObtainedFromDB))))
				{
					continue;
				}
				filterControl = XVar.Clone(this.Invoke("buildControl", (XVar)(new XVar(this.fName, value.Value))));
				filterCtrlBlocks.InitAndSetArrayItem(this.getFilterBlockStructure((XVar)(filterControl), (XVar)(visibilityClass), (XVar)(value.Value)), null);
			}

			return null;
		}
		protected virtual XVar getFilterBlockStructure(dynamic _param_filterControl, dynamic _param_visibilityClass = null, dynamic _param_value = null, dynamic _param_parentFiltersData = null)
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

			return new XVar(MVCFunctions.Concat(this.gfName, "_filter"), filterControl, MVCFunctions.Concat("visibilityClass_", this.gfName), visibilityClass);
		}
		protected virtual XVar getFilteredFilterBlocks()
		{
			dynamic filterControl = null, filterCtrlBlocks = XVar.Array();
			filterControl = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in this.filteredFields[this.fName]["values"].GetEnumerator())
			{
				dynamic classes = null, delButtonHtml = null, parentFiltersData = null, showValue = null;
				showValue = XVar.Clone(this.getControlCaption((XVar)(value.Value)));
				delButtonHtml = XVar.Clone(this.getDelButtonHtml((XVar)(this.gfName), (XVar)(this.id), (XVar)(value.Value)));
				filterControl = XVar.Clone(MVCFunctions.Concat("<span>", delButtonHtml, showValue, "</span>"));
				parentFiltersData = XVar.Clone(this.getParentFiltersDataForFilteredBlock((XVar)(value.Value)));
				classes = XVar.Clone(MVCFunctions.Concat("filter-ready-value", (XVar.Pack(this.multiSelect == Constants.FM_ON_DEMAND) ? XVar.Pack(" ondemand") : XVar.Pack(""))));
				filterCtrlBlocks.InitAndSetArrayItem(this.getFilterBlockStructure((XVar)(filterControl), (XVar)(classes), (XVar)(value.Value), (XVar)(parentFiltersData)), null);
			}
			return filterCtrlBlocks;
		}
		protected virtual XVar getControlCaption(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return this.getValueToShow((XVar)(value));
		}
		protected virtual XVar getParentFiltersDataForFilteredBlock(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return XVar.Array();
		}
		protected virtual XVar addFilterBlocksFromDB(dynamic filterBlocks)
		{

			return null;
		}
		protected virtual XVar getControlHTML(dynamic _param_value, dynamic _param_showValue, dynamic _param_dataValue, dynamic _param_totalValue, dynamic _param_separator, dynamic _param_parentFiltersData = null)
		{
			#region default values
			if(_param_parentFiltersData as Object == null) _param_parentFiltersData = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic showValue = XVar.Clone(_param_showValue);
			dynamic dataValue = XVar.Clone(_param_dataValue);
			dynamic totalValue = XVar.Clone(_param_totalValue);
			dynamic separator = XVar.Clone(_param_separator);
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			dynamic checkBox = null, dataValueAttr = null, encodeDataValue = null, extraDataAttrs = null, filterControl = null, hrefAttr = null, label = null, labelAttrs = null, pageType = null;
			filterControl = new XVar("");
			encodeDataValue = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(dataValue)));
			dataValueAttr = XVar.Clone(MVCFunctions.Concat("data-filtervalue=\"", encodeDataValue, "\""));
			extraDataAttrs = XVar.Clone(this.getExtraDataAttrs((XVar)(parentFiltersData)));
			pageType = new XVar("list");
			if(XVar.Pack(CommonFunctions.isReport((XVar)(this.pSet.getEntityType()))))
			{
				pageType = new XVar("report");
			}
			else
			{
				if(XVar.Pack(CommonFunctions.isChart((XVar)(this.pSet.getEntityType()))))
				{
					pageType = new XVar("chart");
				}
			}
			if(this.multiSelect != Constants.FM_NONE)
			{
				dynamic checkedAttr = null, style = null;
				style = XVar.Clone((XVar.Pack((XVar)(this.filtered)  || (XVar)(this.multiSelect == Constants.FM_ALWAYS)) ? XVar.Pack("") : XVar.Pack("style=\"display: none;\"")));
				checkedAttr = XVar.Clone(this.getCheckedAttr((XVar)(value), (XVar)(parentFiltersData)));
				checkBox = XVar.Clone(MVCFunctions.Concat("<input type=\"checkbox\" ", checkedAttr, " name=\"f[]\" value=\"", encodeDataValue, "\" ", extraDataAttrs, " class=\"multifilter-checkbox filter_", this.gfName, "_", this.id, "\" ", style, ">"));
			}
			if(this.multiSelect != Constants.FM_ALWAYS)
			{
				dynamic href = null;
				href = XVar.Clone(MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(this.tName))), (XVar)(pageType), (XVar)(MVCFunctions.Concat("f=(", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(this.fName)))), separator, encodeDataValue, ")"))));
				hrefAttr = XVar.Clone(MVCFunctions.Concat("href=\"", href, "\""));
				label = XVar.Clone(MVCFunctions.Concat(checkBox, " ", showValue));
			}
			else
			{
				label = XVar.Clone(MVCFunctions.Concat(checkBox, " <span>", showValue, "</span>"));
			}
			if((XVar)(this.useTotals)  && (XVar)(totalValue != XVar.Pack("")))
			{
				label = MVCFunctions.Concat(label, " <span dir=\"LTR\">(", totalValue, ")</span>");
			}
			labelAttrs = XVar.Clone(MVCFunctions.implode(new XVar(" "), (XVar)(new XVar(0, hrefAttr, 1, dataValueAttr, 2, extraDataAttrs))));
			label = XVar.Clone(MVCFunctions.Concat("<a ", labelAttrs, " class=\"", this.gfName, "-filter-value\">", label, "</a>"));
			filterControl = MVCFunctions.Concat(filterControl, label);
			return filterControl;
		}
		protected virtual XVar getExtraDataAttrs(dynamic _param_parentFiltersData)
		{
			#region pass-by-value parameters
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			return "";
		}
		protected virtual XVar getCheckedAttr(dynamic _param_value, dynamic _param_parentFiltersData = null)
		{
			#region default values
			if(_param_parentFiltersData as Object == null) _param_parentFiltersData = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			if((XVar)(this.multiSelect == Constants.FM_NONE)  || (XVar)((XVar)(this.filtered)  && (XVar)(!(XVar)(MVCFunctions.in_array((XVar)(value), (XVar)(this.filteredFields[this.fName]["values"]))))))
			{
				return "";
			}
			return "checked=\"checked\"";
		}
		public virtual XVar getFilterButtonParams(dynamic _param_dBtnParams = null)
		{
			#region default values
			if(_param_dBtnParams as Object == null) _param_dBtnParams = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic dBtnParams = XVar.Clone(_param_dBtnParams);
			#endregion

			return new XVar("attrs", MVCFunctions.Concat("id=\"filter_", this.gfName, "_", this.id, "\""), "hasMultiselectBtn", this.multiSelect == Constants.FM_ON_DEMAND, "hasApplyBtn", this.useApllyBtn);
		}
		public virtual XVar getFilterState()
		{
			return new XVar("visible", this.visible, "filtered", this.filtered, "collapsed", this.showCollapsed, "truncated", this.isTruncated(), "showMoreHidden", this.isShowMoreHidden());
		}
		protected virtual XVar isShowMoreHidden()
		{
			return false;
		}
		public virtual XVar getFilterExtraControls(dynamic _param_dExtraCtrls = null)
		{
			#region default values
			if(_param_dExtraCtrls as Object == null) _param_dExtraCtrls = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic dExtraCtrls = XVar.Clone(_param_dExtraCtrls);
			#endregion

			dynamic selectAllAttrs = null;
			selectAllAttrs = new XVar("");
			if((XVar)(!(XVar)(this.filtered))  && (XVar)(!XVar.Equals(XVar.Pack(this.multiSelect), XVar.Pack(Constants.FM_NONE))))
			{
				selectAllAttrs = new XVar("checked=\"checked\"");
			}
			if(this.multiSelect == Constants.FM_ON_DEMAND)
			{
				selectAllAttrs = MVCFunctions.Concat(selectAllAttrs, " style=\"display: none;\"");
			}
			return new XVar("showValue", this.getShowValue(), "filtered", this.filtered, "selectAllAttrs", selectAllAttrs, "numberOfExtraItemsToShow", this.getNumberOfExtraItemsToShow());
		}
		protected virtual XVar getNumberOfExtraItemsToShow()
		{
			return 0;
		}
		public virtual XVar isVisible()
		{
			return this.visible;
		}
		public virtual XVar isCollapsed()
		{
			return this.showCollapsed;
		}
		public virtual XVar isFiltered()
		{
			return this.filtered;
		}
		protected virtual XVar getDelButtonHtml(dynamic _param_gfName, dynamic _param_id, dynamic _param_deleteValue)
		{
			#region pass-by-value parameters
			dynamic gfName = XVar.Clone(_param_gfName);
			dynamic id = XVar.Clone(_param_id);
			dynamic deleteValue = XVar.Clone(_param_deleteValue);
			#endregion

			dynamic html = null;
			deleteValue = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(deleteValue)));
			html = XVar.Clone(MVCFunctions.Concat("<a class=\"delFilterCtrlButt_", gfName, "_", id, " delete-button fa fa-remove\" data-delete=\"", deleteValue, "\" href=\"#\"></a>"));
			return html;
		}
		protected virtual XVar decryptDataRow(dynamic data)
		{
			if(XVar.Pack(this.cipherer.isFieldPHPEncrypted((XVar)(this.fName))))
			{
				data.InitAndSetArrayItem(this.cipherer.DecryptField((XVar)(this.fName), (XVar)(data[this.fName])), this.fName);
			}

			return null;
		}
		public virtual XVar getLabel(dynamic _param_type, dynamic _param_message)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic message = XVar.Clone(_param_message);
			#endregion

			if(XVar.Equals(XVar.Pack(var_type), XVar.Pack("Text")))
			{
				return message;
			}
			return CommonFunctions.GetCustomLabel((XVar)(message));
		}
		public static XVar getFilterControl(dynamic _param_fName, dynamic _param_pageObj, dynamic _param_id, dynamic _param_viewControls = null)
		{
			#region default values
			if(_param_viewControls as Object == null) _param_viewControls = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic pageObj = XVar.Clone(_param_pageObj);
			dynamic id = XVar.Clone(_param_id);
			dynamic viewControls = XVar.Clone(_param_viewControls);
			#endregion

			dynamic contorlType = null, fieldType = null, filterFields = null;
			filterFields = XVar.Clone(pageObj.pSet.getFilterFields());
			if(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(fName), (XVar)(filterFields))), XVar.Pack(false)))
			{
				return null;
			}
			contorlType = XVar.Clone(pageObj.pSet.getFilterFieldFormat((XVar)(fName)));
			switch(((XVar)contorlType).ToString())
			{
				case Constants.FF_VALUE_LIST:
					if(XVar.Pack(pageObj.pSet.multiSelectLookupEdit((XVar)(fName))))
					{
						return new FilterMultiselectLookup((XVar)(fName), (XVar)(pageObj), (XVar)(id), (XVar)(viewControls));
					}
					return new FilterValuesList((XVar)(fName), (XVar)(pageObj), (XVar)(id), (XVar)(viewControls));
				case Constants.FF_BOOLEAN:
					return new FilterBoolean((XVar)(fName), (XVar)(pageObj), (XVar)(id), (XVar)(viewControls));
				case Constants.FF_INTERVAL_LIST:
					return new FilterIntervalList((XVar)(fName), (XVar)(pageObj), (XVar)(id), (XVar)(viewControls));
				case Constants.FF_INTERVAL_SLIDER:
					fieldType = XVar.Clone(pageObj.pSet.getFieldType((XVar)(fName)));
					if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(fieldType))))
					{
						return new FilterIntervalDateSlider((XVar)(fName), (XVar)(pageObj), (XVar)(id), (XVar)(viewControls));
					}
					if(XVar.Pack(CommonFunctions.IsTimeType((XVar)(fieldType))))
					{
						return new FilterIntervalTimeSlider((XVar)(fName), (XVar)(pageObj), (XVar)(id), (XVar)(viewControls));
					}
					return new FilterIntervalSlider((XVar)(fName), (XVar)(pageObj), (XVar)(id), (XVar)(viewControls));
				default:
					return new FilterValuesList((XVar)(fName), (XVar)(pageObj), (XVar)(id), (XVar)(viewControls));
			}

			return null;
		}
		public virtual XVar hasDependentFilter()
		{
			return false;
		}
		protected virtual XVar dataTotalsName()
		{
			dynamic totalOption = null;
			totalOption = XVar.Clone(this.pSet.getFilterFieldTotal((XVar)(this.fName)));
			if(totalOption == Constants.FT_COUNT)
			{
				return "count";
			}
			else
			{
				if(totalOption == Constants.FT_MIN)
				{
					return "min";
				}
				else
				{
					if(totalOption == Constants.FT_MAX)
					{
						return "max";
					}
				}
			}
			return "";
		}
		protected virtual XVar getShowValue()
		{
			dynamic values = XVar.Array();
			if(XVar.Pack(!(XVar)(this.filtered)))
			{
				return "";
			}
			values = this.filteredFields[this.fName]["values"];
			if(XVar.Pack(!(XVar)(values)))
			{
				return "";
			}
			if(1 < MVCFunctions.count(values))
			{
				return MVCFunctions.Concat("(", MVCFunctions.count(values), ")");
			}
			return this.getControlCaption((XVar)(values[0]));
		}
	}
}
