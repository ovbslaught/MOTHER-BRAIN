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
	public partial class FilterIntervalSlider : FilterControl
	{
		protected dynamic knobsType;
		protected dynamic stepValue;
		protected dynamic minValue;
		protected dynamic maxValue;
		protected dynamic minKnobValue;
		protected dynamic maxKnobValue;
		protected static bool skipFilterIntervalSliderCtor = false;
		public FilterIntervalSlider(dynamic _param_fName, dynamic _param_pageObject, dynamic _param_id, dynamic _param_viewControls)
			:base((XVar)_param_fName, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_viewControls)
		{
			if(skipFilterIntervalSliderCtor)
			{
				skipFilterIntervalSliderCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic viewControls = XVar.Clone(_param_viewControls);
			#endregion

			this.filterFormat = new XVar(Constants.FF_INTERVAL_SLIDER);
			this.useApllyBtn = XVar.Clone(this.pSet.isFilterApplyBtnSet((XVar)(fName)));
			this.knobsType = XVar.Clone(this.pSet.getFilterKnobsType((XVar)(fName)));
			this.stepValue = XVar.Clone(this.pSet.getFilterStepValue((XVar)(fName)));
			this.addJS_CSSfiles((XVar)(pageObject));
			if(XVar.Pack(this.filtered))
			{
				this.assignKnobsValues();
			}
			this.showCollapsed = XVar.Clone(this.pSet.showCollapsed((XVar)(fName)));
			this.separator = XVar.Clone(this.getSeparator());
		}
		protected virtual XVar assignKnobsValues()
		{
			dynamic filterData = XVar.Array(), filterValues = XVar.Array();
			filterData = XVar.Clone(this.filteredFields[this.fName]);
			filterValues = XVar.Clone(XVar.Array());
			filterValues.InitAndSetArrayItem(filterData["values"][0], null);
			filterValues.InitAndSetArrayItem(filterData["sValues"][0], null);
			if(this.knobsType == Constants.FS_MIN_ONLY)
			{
				this.minKnobValue = XVar.Clone(filterValues[0]);
				return null;
			}
			if(this.knobsType == Constants.FS_MAX_ONLY)
			{
				this.maxKnobValue = XVar.Clone(filterValues[0]);
				return null;
			}
			this.minKnobValue = XVar.Clone(filterValues[0]);
			this.maxKnobValue = XVar.Clone(filterValues[1]);

			return null;
		}
		protected virtual XVar getSeparator()
		{
			if(this.knobsType == Constants.FS_MIN_ONLY)
			{
				return "~moreequal~";
			}
			if(this.knobsType == Constants.FS_MAX_ONLY)
			{
				return "~lessequal~";
			}
			return "~slider~";
		}
		protected virtual XVar getDataCommand()
		{
			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.pageObject.getDataSourceFilterCriteria((XVar)(this.fName)));
			dc.totals.InitAndSetArrayItem(new XVar("field", this.fName, "alias", "sliderMin", "skipEmpty", true, "total", "min"), null);
			dc.totals.InitAndSetArrayItem(new XVar("field", this.fName, "alias", "sliderMax", "total", "max"), null);
			return dc;
		}
		protected override XVar addFilterBlocksFromDB(dynamic filterCtrlBlocks)
		{
			dynamic data = null, filterControl = null, qResult = null;
			qResult = XVar.Clone(this.dataSource.getTotals((XVar)(this.getDataCommand())));
			data = XVar.Clone(qResult.fetchAssoc());
			this.decryptDataRow((XVar)(data));
			if(XVar.Pack(this.fieldHasNoRange((XVar)(data))))
			{
				return filterCtrlBlocks;
			}
			filterControl = XVar.Clone(this.buildControl((XVar)(data)));
			filterCtrlBlocks.InitAndSetArrayItem(this.getFilterBlockStructure((XVar)(filterControl)), null);

			return null;
		}
		protected virtual XVar fieldHasNoRange(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if((XVar)((XVar)(XVar.Equals(XVar.Pack(data["sliderMin"]), XVar.Pack(null)))  && (XVar)(XVar.Equals(XVar.Pack(data["sliderMax"]), XVar.Pack(null))))  || (XVar)(data["sliderMax"] == data["sliderMin"]))
			{
				return true;
			}
			return false;
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

			if(XVar.Pack(!(XVar)(this.viewControl)))
			{
				return "";
			}
			this.minValue = XVar.Clone(data["sliderMin"]);
			this.maxValue = XVar.Clone(data["sliderMax"]);
			if(XVar.Pack(!(XVar)(this.filtered)))
			{
				this.minKnobValue = XVar.Clone(data["sliderMin"]);
				this.maxKnobValue = XVar.Clone(data["sliderMax"]);
			}
			else
			{
				if(this.knobsType == Constants.FS_MAX_ONLY)
				{
					this.minKnobValue = XVar.Clone(data["sliderMin"]);
				}
				if(this.knobsType == Constants.FS_MIN_ONLY)
				{
					this.maxKnobValue = XVar.Clone(data["sliderMax"]);
				}
			}
			return this.getSliderHTML();
		}
		protected virtual XVar getCaptionSpansHTML()
		{
			dynamic captionSpans = null, maxSpan = null, minSpan = null, postfixSpan = null, prefixSpan = null;
			minSpan = XVar.Clone(MVCFunctions.Concat("<span class=\"slider-min\">", this.getMinSpanValue(), "</span>"));
			maxSpan = XVar.Clone(MVCFunctions.Concat("<span class=\"slider-max\">", this.getMaxSpanValue(), "</span>"));
			captionSpans = XVar.Clone(MVCFunctions.Concat(minSpan, "&nbsp;-&nbsp", maxSpan));
			prefixSpan = new XVar("<span class=\"slider-caption-prefix\"></span>");
			postfixSpan = new XVar("<span class=\"slider-caption-postfix\"></span>");
			captionSpans = XVar.Clone(MVCFunctions.Concat(prefixSpan, captionSpans, postfixSpan));
			return captionSpans;
		}
		protected virtual XVar getSliderHTML()
		{
			dynamic captionSpans = null, filterControl = null;
			captionSpans = XVar.Clone(this.getCaptionSpansHTML());
			filterControl = XVar.Clone(MVCFunctions.Concat("<div id=\"slider_values_", this.gfName, "\" class=\"filter-slider-values\">", captionSpans, "</div>"));
			filterControl = MVCFunctions.Concat(filterControl, "<div id=\"slider_", this.gfName, "\" class=\"filter-slider\"></div>");
			return filterControl;
		}
		protected virtual XVar getMinSpanValue()
		{
			dynamic minSpanValue = null, viewFormat = null;
			minSpanValue = XVar.Clone(this.minKnobValue);
			if(minSpanValue < this.minValue)
			{
				minSpanValue = XVar.Clone(this.minValue);
			}
			viewFormat = XVar.Clone(this.viewControl.viewFormat);
			if((XVar)(viewFormat == Constants.FORMAT_CURRENCY)  || (XVar)(viewFormat == Constants.FORMAT_NUMBER))
			{
				dynamic data = null;
				data = XVar.Clone(new XVar(this.fName, minSpanValue));
				minSpanValue = XVar.Clone(this.viewControl.showDBValue((XVar)(data), new XVar("")));
			}
			return minSpanValue;
		}
		protected virtual XVar getMaxSpanValue()
		{
			dynamic maxSpanValue = null, viewFormat = null;
			maxSpanValue = XVar.Clone(this.maxKnobValue);
			if(this.maxValue < maxSpanValue)
			{
				maxSpanValue = XVar.Clone(this.maxValue);
			}
			viewFormat = XVar.Clone(this.viewControl.viewFormat);
			if((XVar)(viewFormat == Constants.FORMAT_CURRENCY)  || (XVar)(viewFormat == Constants.FORMAT_NUMBER))
			{
				dynamic data = null;
				data = XVar.Clone(new XVar(this.fName, maxSpanValue));
				maxSpanValue = XVar.Clone(this.viewControl.showDBValue((XVar)(data), new XVar("")));
			}
			return maxSpanValue;
		}
		public override XVar addFilterControlToControlsMap(dynamic _param_pageObj)
		{
			#region pass-by-value parameters
			dynamic pageObj = XVar.Clone(_param_pageObj);
			#endregion

			dynamic ctrlsMap = XVar.Array(), viewFomat = null;
			if(XVar.Pack(!(XVar)(this.viewControl)))
			{
				return null;
			}
			ctrlsMap = XVar.Clone(this.getBaseContolsMapParams());
			ctrlsMap.InitAndSetArrayItem(this.minValue, "minValue");
			ctrlsMap.InitAndSetArrayItem(this.maxValue, "maxValue");
			ctrlsMap.InitAndSetArrayItem(this.round((XVar)(this.minValue), new XVar(true)), "roundedMin");
			ctrlsMap.InitAndSetArrayItem(this.round((XVar)(this.maxValue), new XVar(false)), "roundedMax");
			ctrlsMap.InitAndSetArrayItem(this.round((XVar)(this.minKnobValue), new XVar(true)), "roundedMinKnobValue");
			ctrlsMap.InitAndSetArrayItem(this.round((XVar)(this.maxKnobValue), new XVar(false)), "roundedMaxKnobValue");
			if(XVar.Pack(this.filtered))
			{
				ctrlsMap.InitAndSetArrayItem(this.minKnobValue, "minKnobValue");
				ctrlsMap.InitAndSetArrayItem(this.maxKnobValue, "maxKnobValue");
			}
			viewFomat = XVar.Clone(this.viewControl.viewFormat);
			ctrlsMap.InitAndSetArrayItem(viewFomat == Constants.FORMAT_NUMBER, "viewAsNumber");
			ctrlsMap.InitAndSetArrayItem(viewFomat == Constants.FORMAT_CURRENCY, "viewAsCurrency");
			ctrlsMap.InitAndSetArrayItem(CommonFunctions.getFormatSettings((XVar)(viewFomat), (XVar)(this.pSet), (XVar)(this.fName)), "formatSettings");
			pageObj.controlsMap.InitAndSetArrayItem(ctrlsMap, "filters", "controls", null);

			return null;
		}
		protected override XVar getBaseContolsMapParams()
		{
			dynamic ctrlsMap = XVar.Array();
			ctrlsMap = XVar.Clone(XVar.Array());
			ctrlsMap.InitAndSetArrayItem(this.fName, "fieldName");
			ctrlsMap.InitAndSetArrayItem(this.gfName, "gfieldName");
			ctrlsMap.InitAndSetArrayItem(this.filterFormat, "filterFormat");
			ctrlsMap.InitAndSetArrayItem(this.filtered, "filtered");
			ctrlsMap.InitAndSetArrayItem(this.separator, "separator");
			ctrlsMap.InitAndSetArrayItem(this.knobsType, "knobsType");
			ctrlsMap.InitAndSetArrayItem(this.useApllyBtn, "useApllyBtn");
			ctrlsMap.InitAndSetArrayItem(this.getStepValue(), "step");
			ctrlsMap.InitAndSetArrayItem(this.showCollapsed, "collapsed");
			return ctrlsMap;
		}
		protected virtual XVar getStepValue()
		{
			return this.stepValue;
		}
		protected virtual XVar round(dynamic _param_value, dynamic _param_min)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic min = XVar.Clone(_param_min);
			#endregion

			dynamic step = null;
			step = XVar.Clone(this.stepValue);
			if(XVar.Pack(min))
			{
				return (XVar)Math.Floor((double)(value / step)) * step;
			}
			return (XVar)Math.Ceiling((double)(value / step)) * step;
		}
		protected virtual XVar addJS_CSSfiles(dynamic _param_pageObject)
		{
			#region pass-by-value parameters
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion


			return null;
		}
		public override XVar buildFilterCtrlBlockArray(dynamic _param_pageObj, dynamic _param_dFilterBlocks = null)
		{
			#region default values
			if(_param_dFilterBlocks as Object == null) _param_dFilterBlocks = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic pageObj = XVar.Clone(_param_pageObj);
			dynamic dFilterBlocks = XVar.Clone(_param_dFilterBlocks);
			#endregion

			dynamic filterCtrlBlocks = null;
			filterCtrlBlocks = XVar.Clone(XVar.Array());
			this.addFilterBlocksFromDB((XVar)(filterCtrlBlocks));
			if(XVar.Pack(!(XVar)(filterCtrlBlocks)))
			{
				this.visible = new XVar(false);
			}
			if(XVar.Pack(this.visible))
			{
				this.addFilterControlToControlsMap((XVar)(pageObj));
			}
			return filterCtrlBlocks;
		}
		public static XVar getFilterCondition(dynamic _param_fName, dynamic _param_value, dynamic _param_pSet_packed, dynamic _param_secondValue)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic value = XVar.Clone(_param_value);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic secondValue = XVar.Clone(_param_secondValue);
			#endregion

			dynamic conditionLess = null, conditionMore = null, knobsType = null;
			knobsType = XVar.Clone(pSet.getFilterKnobsType((XVar)(fName)));
			if(knobsType == Constants.FS_MAX_ONLY)
			{
				return DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(fName), new XVar(Constants.dsopMORE), (XVar)(value))));
			}
			conditionMore = XVar.Clone(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(fName), new XVar(Constants.dsopLESS), (XVar)(value)))));
			if(knobsType == Constants.FS_MIN_ONLY)
			{
				return conditionMore;
			}
			if((XVar)(3 <= pSet.getFilterStepType((XVar)(fName)))  && (XVar)(CommonFunctions.IsDateFieldType((XVar)(pSet.getFieldType((XVar)(fName))))))
			{
				dynamic tm = XVar.Array();
				tm = XVar.Clone(MVCFunctions.db2time((XVar)(secondValue)));
				if(XVar.Pack(!(XVar)(tm[0])))
				{
					conditionLess = new XVar(null);
				}
				else
				{
					conditionLess = XVar.Clone(DataCondition.FieldIs((XVar)(fName), new XVar(Constants.dsopLESS), (XVar)(CommonFunctions.date2db((XVar)(CommonFunctions.adddays((XVar)(tm), new XVar(1)))))));
				}
			}
			else
			{
				conditionLess = XVar.Clone(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(fName), new XVar(Constants.dsopMORE), (XVar)(secondValue)))));
			}
			return DataCondition._And((XVar)(new XVar(0, conditionLess, 1, conditionMore)));
		}
	}
}
