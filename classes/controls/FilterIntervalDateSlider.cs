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
	public partial class FilterIntervalDateSlider : FilterIntervalSlider
	{
		protected dynamic stepType;
		protected dynamic maxKnobFormatValue;
		protected dynamic minKnobFormatValue;
		protected dynamic minDateArray;
		protected dynamic maxDateArray;
		protected dynamic adjMinDateMonth;
		protected dynamic adjMaxDateMonth;
		protected dynamic months = XVar.Array();
		protected static bool skipFilterIntervalDateSliderCtor = false;
		public FilterIntervalDateSlider(dynamic _param_fName, dynamic _param_pageObject, dynamic _param_id, dynamic _param_viewControls)
			:base((XVar)_param_fName, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_viewControls)
		{
			if(skipFilterIntervalDateSliderCtor)
			{
				skipFilterIntervalDateSliderCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic viewControls = XVar.Clone(_param_viewControls);
			#endregion

			this.stepType = XVar.Clone(this.pSet.getFilterStepType((XVar)(fName)));
			if(this.stepType == Constants.FSST_MONTHS)
			{
				this.months = XVar.Clone(new XVar(0, CommonFunctions.mlang_message(new XVar("MONTH_JAN")), 1, CommonFunctions.mlang_message(new XVar("MONTH_FEB")), 2, CommonFunctions.mlang_message(new XVar("MONTH_MAR")), 3, CommonFunctions.mlang_message(new XVar("MONTH_APR")), 4, CommonFunctions.mlang_message(new XVar("MONTH_MAY")), 5, CommonFunctions.mlang_message(new XVar("MONTH_JUN")), 6, CommonFunctions.mlang_message(new XVar("MONTH_JUL")), 7, CommonFunctions.mlang_message(new XVar("MONTH_AUG")), 8, CommonFunctions.mlang_message(new XVar("MONTH_SEP")), 9, CommonFunctions.mlang_message(new XVar("MONTH_OCT")), 10, CommonFunctions.mlang_message(new XVar("MONTH_NOV")), 11, CommonFunctions.mlang_message(new XVar("MONTH_DEC"))));
			}
		}
		protected override XVar assignKnobsValues()
		{
			dynamic filterData = XVar.Array(), filterValues = XVar.Array(), value1 = null;
			filterData = XVar.Clone(this.filteredFields[this.fName]);
			filterValues = XVar.Clone(XVar.Array());
			filterValues.InitAndSetArrayItem(filterData["values"][0], null);
			filterValues.InitAndSetArrayItem(filterData["sValues"][0], null);
			value1 = XVar.Clone(CommonFunctions.prepare_for_db((XVar)(this.fName), (XVar)(filterValues[0]), new XVar("")));
			if(this.knobsType == Constants.FS_MIN_ONLY)
			{
				this.minKnobValue = XVar.Clone(value1);
				this.minKnobFormatValue = XVar.Clone(filterValues[0]);
				return null;
			}
			if(this.knobsType == Constants.FS_MAX_ONLY)
			{
				this.maxKnobValue = XVar.Clone(value1);
				this.maxKnobFormatValue = XVar.Clone(filterValues[0]);
				return null;
			}
			this.minKnobValue = XVar.Clone(value1);
			this.maxKnobValue = XVar.Clone(CommonFunctions.prepare_for_db((XVar)(this.fName), (XVar)(filterValues[1]), new XVar("")));
			this.minKnobFormatValue = XVar.Clone(filterValues[0]);
			this.maxKnobFormatValue = XVar.Clone(filterValues[1]);

			return null;
		}
		protected virtual XVar getDateTimeArray(dynamic _param_value, dynamic _param_forCaption = null)
		{
			#region default values
			if(_param_forCaption as Object == null) _param_forCaption = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic forCaption = XVar.Clone(_param_forCaption);
			#endregion

			return MVCFunctions.db2time((XVar)(value));
		}
		protected override XVar getCaptionSpansHTML()
		{
			dynamic captionSpans = null, inlineStyelMax = null, inlineStyelMin = null, inlineStyelPostfix = null, inlineStyelPrefix = null, maxSpan = null, minSpan = null, postfix = null, postfixSpan = null, prefix = null, prefixSpan = null;
			inlineStyelPrefix = XVar.Clone(inlineStyelPostfix = new XVar(" style=\"display: none;\""));
			inlineStyelMin = XVar.Clone(inlineStyelMax = new XVar(""));
			if((XVar)(this.stepType != Constants.FSST_YEARS)  || (XVar)(this.stepType != Constants.FSST_DAYS))
			{
				dynamic maxValueArr = XVar.Array(), minValueArr = XVar.Array();
				minValueArr = XVar.Clone(this.minDateArray);
				if(this.getValueInSeconds((XVar)(this.minValue)) <= this.getValueInSeconds((XVar)(this.minKnobValue)))
				{
					minValueArr = XVar.Clone(this.getDateTimeArray((XVar)(this.minKnobValue)));
				}
				maxValueArr = XVar.Clone(this.maxDateArray);
				if(this.getValueInSeconds((XVar)(this.maxKnobValue)) <= this.getValueInSeconds((XVar)(this.maxValue)))
				{
					maxValueArr = XVar.Clone(this.getDateTimeArray((XVar)(this.maxKnobValue)));
				}
				if(this.stepType == Constants.FSST_MONTHS)
				{
					dynamic month = null;
					month = XVar.Clone(minValueArr[1]);
					if((XVar)(!(XVar)(this.filtered))  || (XVar)(this.isMonthNumberToAdjust(new XVar(true), (XVar)(minValueArr[0]), (XVar)(minValueArr[1]))))
					{
						month = XVar.Clone(this.getAdjustedMonthNumber((XVar)(minValueArr[1]), new XVar(true)));
					}
					prefix = XVar.Clone(this.getMonthName((XVar)(month)));
					if(minValueArr[0] == maxValueArr[0])
					{
						inlineStyelMin = new XVar(" style=\"display: none;\"");
						inlineStyelPrefix = new XVar("");
					}
				}
				else
				{
					if((XVar)((XVar)(this.stepType == Constants.FSST_HOURS)  || (XVar)(this.stepType == Constants.FSST_MINUTES))  || (XVar)(this.stepType == Constants.FSST_SECONDS))
					{
						dynamic timeFormatString = null;
						timeFormatString = XVar.Clone(GlobalVars.locale_info["LOCALE_STIMEFORMAT"]);
						if(this.stepType != Constants.FSST_SECONDS)
						{
							timeFormatString = XVar.Clone(MVCFunctions.str_replace((XVar)(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_STIME"], "ss")), new XVar(""), (XVar)(timeFormatString)));
						}
						postfix = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(maxValueArr), (XVar)(timeFormatString)));
						if((XVar)((XVar)(minValueArr[0] == maxValueArr[0])  && (XVar)(minValueArr[1] == maxValueArr[1]))  && (XVar)(minValueArr[2] == maxValueArr[2]))
						{
							inlineStyelMax = new XVar(" style=\"display: none;\"");
							inlineStyelPostfix = new XVar("");
						}
					}
				}
			}
			minSpan = XVar.Clone(MVCFunctions.Concat("<span class=\"slider-min\"", inlineStyelMin, ">", this.getMinSpanValue(), "</span>"));
			maxSpan = XVar.Clone(MVCFunctions.Concat("<span class=\"slider-max\"", inlineStyelMax, ">", this.getMaxSpanValue(), "</span>"));
			captionSpans = XVar.Clone(MVCFunctions.Concat(minSpan, "&nbsp;-&nbsp", maxSpan));
			prefixSpan = XVar.Clone(MVCFunctions.Concat("<span class=\"slider-caption-prefix\"", inlineStyelPrefix, ">", prefix, "</span>"));
			postfixSpan = XVar.Clone(MVCFunctions.Concat("<span class=\"slider-caption-postfix\"", inlineStyelPostfix, ">", postfix, "</span>"));
			captionSpans = XVar.Clone(MVCFunctions.Concat(prefixSpan, captionSpans, postfixSpan));
			return captionSpans;
		}
		protected virtual XVar getValueInSeconds(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return MVCFunctions.strtotime((XVar)(value));
		}
		protected override XVar getMinSpanValue()
		{
			dynamic minSpanValue = null;
			minSpanValue = XVar.Clone(this.minKnobValue);
			if(this.getValueInSeconds((XVar)(minSpanValue)) < this.getValueInSeconds((XVar)(this.minValue)))
			{
				minSpanValue = XVar.Clone(this.minValue);
			}
			minSpanValue = XVar.Clone(this.getRoundedDateCaption((XVar)(minSpanValue), new XVar(true)));
			return minSpanValue;
		}
		protected override XVar getMaxSpanValue()
		{
			dynamic maxSpanValue = null;
			maxSpanValue = XVar.Clone(this.maxKnobValue);
			if(this.getValueInSeconds((XVar)(this.maxValue)) < this.getValueInSeconds((XVar)(maxSpanValue)))
			{
				maxSpanValue = XVar.Clone(this.maxValue);
			}
			maxSpanValue = XVar.Clone(this.getRoundedDateCaption((XVar)(maxSpanValue), new XVar(false)));
			return maxSpanValue;
		}
		protected virtual XVar getRoundedDateCaption(dynamic _param_value, dynamic _param_isLower = null)
		{
			#region default values
			if(_param_isLower as Object == null) _param_isLower = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic isLower = XVar.Clone(_param_isLower);
			#endregion

			dynamic dateArray = XVar.Array();
			dateArray = XVar.Clone(this.getDateTimeArray((XVar)(value), new XVar(true)));
			switch(((XVar)this.stepType).ToInt())
			{
				case Constants.FSST_SECONDS:
					return this.getSecondsCaption((XVar)(dateArray));
				case Constants.FSST_MINUTES:
					return this.getMinutesCaption((XVar)(dateArray));
				case Constants.FSST_HOURS:
					return this.getHoursCaption((XVar)(dateArray), (XVar)(isLower));
				case Constants.FSST_DAYS:
					return CommonFunctions.format_normalized_shortdate((XVar)(dateArray));
				case Constants.FSST_MONTHS:
					return this.getMonthCaption((XVar)(dateArray), (XVar)(isLower));
				case Constants.FSST_YEARS:
					return dateArray[0];
				default:
					return value;
			}

			return null;
		}
		protected virtual XVar getSecondsCaption(dynamic _param_dateArray)
		{
			#region pass-by-value parameters
			dynamic dateArray = XVar.Clone(_param_dateArray);
			#endregion

			return CommonFunctions.str_format_datetime((XVar)(dateArray));
		}
		protected virtual XVar getMinutesCaption(dynamic _param_dateArray)
		{
			#region pass-by-value parameters
			dynamic dateArray = XVar.Clone(_param_dateArray);
			#endregion

			dynamic timeFormatString = null;
			dateArray.InitAndSetArrayItem(0, 5);
			timeFormatString = XVar.Clone(MVCFunctions.str_replace((XVar)(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_STIME"], "ss")), new XVar(""), (XVar)(GlobalVars.locale_info["LOCALE_STIMEFORMAT"])));
			return CommonFunctions.format_datetime_custom((XVar)(dateArray), (XVar)(MVCFunctions.Concat(CommonFunctions.normalized_date_format(), " ", timeFormatString)));
		}
		protected virtual XVar getHoursCaption(dynamic _param_dateArray, dynamic _param_isLower)
		{
			#region pass-by-value parameters
			dynamic dateArray = XVar.Clone(_param_dateArray);
			dynamic isLower = XVar.Clone(_param_isLower);
			#endregion

			dynamic hours = null, timeFormatString = null;
			hours = XVar.Clone(this.getAdjustedHoursValue((XVar)(dateArray[3]), (XVar)(dateArray[4]), (XVar)(isLower)));
			dateArray.InitAndSetArrayItem(dateArray.InitAndSetArrayItem(dateArray.InitAndSetArrayItem(0, 5), 4), 3);
			dateArray = XVar.Clone(CommonFunctions.addHours((XVar)(dateArray), (XVar)(hours)));
			timeFormatString = XVar.Clone(MVCFunctions.str_replace((XVar)(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_STIME"], "ss")), new XVar(""), (XVar)(GlobalVars.locale_info["LOCALE_STIMEFORMAT"])));
			return CommonFunctions.format_datetime_custom((XVar)(dateArray), (XVar)(MVCFunctions.Concat(CommonFunctions.normalized_date_format(), " ", timeFormatString)));
		}
		protected virtual XVar getAdjustedHoursValue(dynamic _param_hours, dynamic _param_minutes, dynamic _param_isLower)
		{
			#region pass-by-value parameters
			dynamic hours = XVar.Clone(_param_hours);
			dynamic minutes = XVar.Clone(_param_minutes);
			dynamic isLower = XVar.Clone(_param_isLower);
			#endregion

			dynamic step = null;
			step = XVar.Clone(this.stepValue * 60);
			minutes = XVar.Clone(hours * 60 + minutes);
			if(XVar.Pack(isLower))
			{
				return (XVar)Math.Floor((double)(minutes / step)) * this.stepValue;
			}
			return (XVar)Math.Ceiling((double)(minutes / step)) * this.stepValue;
		}
		protected virtual XVar getMonthCaption(dynamic _param_dateArray, dynamic _param_isLower)
		{
			#region pass-by-value parameters
			dynamic dateArray = XVar.Clone(_param_dateArray);
			dynamic isLower = XVar.Clone(_param_isLower);
			#endregion

			dynamic month = null, year = null;
			year = XVar.Clone(dateArray[0]);
			month = XVar.Clone(dateArray[1]);
			if((XVar)(!(XVar)(this.filtered))  || (XVar)(this.isMonthNumberToAdjust((XVar)(isLower), (XVar)(year), (XVar)(month))))
			{
				month = XVar.Clone(this.getAdjustedMonthNumber((XVar)(month), (XVar)(isLower)));
			}
			return MVCFunctions.Concat(this.getMonthName((XVar)(month)), " ", year);
		}
		protected virtual XVar isMonthNumberToAdjust(dynamic _param_isLower, dynamic _param_year, dynamic _param_month)
		{
			#region pass-by-value parameters
			dynamic isLower = XVar.Clone(_param_isLower);
			dynamic year = XVar.Clone(_param_year);
			dynamic month = XVar.Clone(_param_month);
			#endregion

			switch(((XVar)this.knobsType).ToInt())
			{
				case Constants.FS_MIN_ONLY:
					return !(XVar)(isLower);
				case Constants.FS_MAX_ONLY:
					return isLower;
				case Constants.FS_BOTH:
					if((XVar)((XVar)((XVar)(isLower)  && (XVar)(year == this.minDateArray[0]))  && (XVar)(MVCFunctions.abs((XVar)(month - this.adjMinDateMonth)) < this.stepValue))  || (XVar)((XVar)((XVar)(!(XVar)(isLower))  && (XVar)(year == this.maxDateArray[0]))  && (XVar)(MVCFunctions.abs((XVar)(month - this.adjMaxDateMonth)) < this.stepValue)))
					{
						return true;
					}
					return false;
				default:
					return false;
			}

			return null;
		}
		protected virtual XVar getMonthName(dynamic _param_month)
		{
			#region pass-by-value parameters
			dynamic month = XVar.Clone(_param_month);
			#endregion

			return this.months[month - 1];
		}
		protected virtual XVar getAdjustedMonthNumber(dynamic _param_month, dynamic _param_isLower)
		{
			#region pass-by-value parameters
			dynamic month = XVar.Clone(_param_month);
			dynamic isLower = XVar.Clone(_param_isLower);
			#endregion

			dynamic step = null;
			step = XVar.Clone(this.stepValue);
			if(step != 1)
			{
				if(XVar.Pack(isLower))
				{
					month = XVar.Clone((XVar)Math.Floor((double)((month - 1) / step)) * step + 1);
				}
				else
				{
					month = XVar.Clone((XVar)Math.Ceiling((double)(month / step)) * step);
				}
			}
			return month;
		}
		protected virtual XVar getDateTimeString(dynamic _param_dateArray)
		{
			#region pass-by-value parameters
			dynamic dateArray = XVar.Clone(_param_dateArray);
			#endregion

			dynamic formatString = null;
			formatString = new XVar("yyyy-MM-dd HH:mm:ss");
			return CommonFunctions.format_datetime_custom((XVar)(dateArray), (XVar)(formatString));
		}
		protected virtual XVar getRoundedDate(dynamic _param_value, dynamic _param_isLower, dynamic _param_isKnob = null)
		{
			#region default values
			if(_param_isKnob as Object == null) _param_isKnob = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic isLower = XVar.Clone(_param_isLower);
			dynamic isKnob = XVar.Clone(_param_isKnob);
			#endregion

			dynamic dateArray = XVar.Array(), diffSec = null, hours = null, minutes = null, prepDateArray = null;
			dateArray = XVar.Clone(this.getDateTimeArray((XVar)(value)));
			switch(((XVar)this.stepType).ToInt())
			{
				case Constants.FSST_SECONDS:
					if(XVar.Pack(isKnob))
					{
						return this.getDateTimeString((XVar)(dateArray));
					}
					prepDateArray = XVar.Clone(this.minDateArray);
					diffSec = XVar.Clone(this.getDifferenceInSecWithMin((XVar)(value), (XVar)(isLower)));
					prepDateArray = XVar.Clone(CommonFunctions.addSeconds((XVar)(prepDateArray), (XVar)(diffSec)));
					return this.getDateTimeString((XVar)(prepDateArray));
				case Constants.FSST_MINUTES:
					if(XVar.Pack(isKnob))
					{
						return this.getDateTimeString((XVar)(dateArray));
					}
					prepDateArray = XVar.Clone(this.minDateArray);
					diffSec = XVar.Clone(this.getDifferenceInSecWithMin((XVar)(value), (XVar)(isLower)));
					minutes = XVar.Clone((XVar)Math.Floor((double)(diffSec / 60)));
					prepDateArray = XVar.Clone(CommonFunctions.addMinutes((XVar)(prepDateArray), (XVar)(minutes)));
					return this.getDateTimeString((XVar)(prepDateArray));
				case Constants.FSST_HOURS:
					hours = XVar.Clone(this.getAdjustedHoursValue((XVar)(dateArray[3]), (XVar)(dateArray[4]), (XVar)(isLower)));
					dateArray.InitAndSetArrayItem(dateArray.InitAndSetArrayItem(dateArray.InitAndSetArrayItem(0, 5), 4), 3);
					dateArray = XVar.Clone(CommonFunctions.addHours((XVar)(dateArray), (XVar)(hours)));
					return this.getDateTimeString((XVar)(dateArray));
				case Constants.FSST_DAYS:
					return CommonFunctions.format_datetime_custom((XVar)(dateArray), new XVar("yyyy-MM-dd"));
				case Constants.FSST_MONTHS:
					if(XVar.Pack(isKnob))
					{
						return CommonFunctions.format_datetime_custom((XVar)(dateArray), new XVar("yyyy-MM-dd"));
					}
					dateArray.InitAndSetArrayItem(this.getAdjustedMonthNumber((XVar)(dateArray[1]), (XVar)(isLower)), 1);
					dateArray.InitAndSetArrayItem((XVar.Pack(isLower) ? XVar.Pack(1) : XVar.Pack(CommonFunctions.getLastMonthDayNumber((XVar)(dateArray[0]), (XVar)(dateArray[1])))), 2);
					dateArray.InitAndSetArrayItem(dateArray.InitAndSetArrayItem(dateArray.InitAndSetArrayItem(0, 5), 4), 3);
					return CommonFunctions.format_datetime_custom((XVar)(dateArray), new XVar("yyyy-MM-dd"));
				case Constants.FSST_YEARS:
					dateArray.InitAndSetArrayItem(dateArray.InitAndSetArrayItem(dateArray.InitAndSetArrayItem(0, 5), 4), 3);
					dateArray.InitAndSetArrayItem((XVar.Pack(isLower) ? XVar.Pack(1) : XVar.Pack(12)), 1);
					dateArray.InitAndSetArrayItem((XVar.Pack(isLower) ? XVar.Pack(1) : XVar.Pack(31)), 2);
					return CommonFunctions.format_datetime_custom((XVar)(dateArray), new XVar("yyyy-MM-dd"));
				default:
					return value;
			}

			return null;
		}
		protected virtual XVar getDifferenceInSecWithMin(dynamic _param_value, dynamic _param_isLower)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic isLower = XVar.Clone(_param_isLower);
			#endregion

			dynamic min = null, minSec = null, rvalue = null, step = null, valueSec = null;
			step = XVar.Clone(this.getStepValue());
			minSec = XVar.Clone(this.getValueInSeconds((XVar)(this.minValue)));
			valueSec = XVar.Clone(this.getValueInSeconds((XVar)(value)));
			min = XVar.Clone((XVar)Math.Floor((double)(minSec / step)) * step);
			if(XVar.Pack(isLower))
			{
				rvalue = XVar.Clone((XVar)Math.Floor((double)(valueSec / step)) * step);
			}
			else
			{
				rvalue = XVar.Clone((XVar)Math.Ceiling((double)(valueSec / step)) * step);
			}
			return rvalue - min;
		}
		protected override XVar getStepValue()
		{
			switch(((XVar)this.stepType).ToInt())
			{
				case Constants.FSST_MINUTES:
					return 60 * this.stepValue;
				case Constants.FSST_HOURS:
					return 3600 * this.stepValue;
				case Constants.FSST_DAYS:
					return 86400 * this.stepValue;
				default:
					return this.stepValue;
			}

			return null;
		}
		protected override XVar fieldHasNoRange(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic step = null;
			if((XVar)((XVar)(XVar.Equals(XVar.Pack(data["sliderMin"]), XVar.Pack(null)))  && (XVar)(XVar.Equals(XVar.Pack(data["sliderMax"]), XVar.Pack(null))))  || (XVar)(data["sliderMax"] == data["sliderMin"]))
			{
				return true;
			}
			step = XVar.Clone(this.getStepValue());
			if((XVar)(this.stepType == Constants.FSST_MONTHS)  || (XVar)(this.stepType == Constants.FSST_YEARS))
			{
				dynamic dateArrayMax = XVar.Array(), dateArrayMin = XVar.Array();
				dateArrayMin = XVar.Clone(MVCFunctions.db2time((XVar)(data["sliderMin"])));
				dateArrayMax = XVar.Clone(MVCFunctions.db2time((XVar)(data["sliderMax"])));
				if((XVar)((XVar)(this.stepType == Constants.FSST_MONTHS)  && (XVar)(dateArrayMax[0] == dateArrayMin[0]))  && (XVar)(dateArrayMax[1] - dateArrayMin[1] < step))
				{
					return true;
				}
				if((XVar)(this.stepType == Constants.FSST_YEARS)  && (XVar)(dateArrayMax[0] - dateArrayMin[0] < step))
				{
					return true;
				}
			}
			else
			{
				if(MVCFunctions.strtotime((XVar)(data["sliderMax"])) - MVCFunctions.strtotime((XVar)(data["sliderMin"])) < step)
				{
					return true;
				}
			}
			return false;
		}
		protected override XVar buildControl(dynamic _param_data, dynamic _param_parentFiltersData = null)
		{
			#region default values
			if(_param_parentFiltersData as Object == null) _param_parentFiltersData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			dynamic timeZone = null;
			this.minValue = XVar.Clone(data["sliderMin"]);
			this.maxValue = XVar.Clone(data["sliderMax"]);
			if(ProjectSettings.ext() == "php")
			{
				timeZone = XVar.Clone(CommonFunctions.date_default_timezone_get());
				CommonFunctions.date_default_timezone_set(new XVar("UTC"));
			}
			this.minDateArray = XVar.Clone(this.getDateTimeArray((XVar)(this.minValue)));
			this.maxDateArray = XVar.Clone(this.getDateTimeArray((XVar)(this.maxValue)));
			if(this.stepType == Constants.FSST_MINUTES)
			{
				this.minDateArray = XVar.Clone(this.getMinuteAdjustedMinValue());
			}
			else
			{
				if(this.stepType == Constants.FSST_SECONDS)
				{
					this.minDateArray = XVar.Clone(this.getSecAdjustedMinValue());
				}
			}
			if(this.stepType == Constants.FSST_MONTHS)
			{
				this.adjMinDateMonth = XVar.Clone(this.getAdjustedMonthNumber((XVar)(this.minDateArray[1]), new XVar(true)));
				this.adjMaxDateMonth = XVar.Clone(this.getAdjustedMonthNumber((XVar)(this.maxDateArray[1]), new XVar(false)));
			}
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
			if(ProjectSettings.ext() == "php")
			{
				CommonFunctions.date_default_timezone_set((XVar)(timeZone));
			}
			return this.getSliderHTML();
		}
		public virtual XVar getSecAdjustedMinValue()
		{
			dynamic diffSec = null, minRoundSec = null, minUpToSec = null, prepDateArray = XVar.Array(), step = null;
			prepDateArray = XVar.Clone(this.minDateArray);
			prepDateArray.InitAndSetArrayItem(0, 5);
			step = XVar.Clone(this.getStepValue());
			minRoundSec = XVar.Clone((XVar)Math.Floor((double)(MVCFunctions.strtotime((XVar)(this.minValue)) / step)) * step);
			minUpToSec = XVar.Clone((XVar)Math.Floor((double)(MVCFunctions.strtotime((XVar)(this.minValue)) / 60)) * 60);
			diffSec = XVar.Clone(minRoundSec - minUpToSec);
			prepDateArray = XVar.Clone(CommonFunctions.addSeconds((XVar)(prepDateArray), (XVar)(diffSec)));
			return prepDateArray;
		}
		public virtual XVar getMinuteAdjustedMinValue()
		{
			dynamic diffSec = null, minRoundSec = null, minUpToHour = null, minutes = null, prepDateArray = XVar.Array(), step = null;
			prepDateArray = XVar.Clone(this.minDateArray);
			prepDateArray.InitAndSetArrayItem(prepDateArray.InitAndSetArrayItem(0, 5), 4);
			step = XVar.Clone(this.getStepValue());
			minRoundSec = XVar.Clone((XVar)Math.Floor((double)(MVCFunctions.strtotime((XVar)(this.minValue)) / step)) * step);
			minUpToHour = XVar.Clone((XVar)Math.Floor((double)(MVCFunctions.strtotime((XVar)(this.minValue)) / 3600)) * 3600);
			diffSec = XVar.Clone(minRoundSec - minUpToHour);
			minutes = XVar.Clone((XVar)Math.Floor((double)(diffSec / 60)));
			prepDateArray = XVar.Clone(CommonFunctions.addMinutes((XVar)(prepDateArray), (XVar)(minutes)));
			return prepDateArray;
		}
		protected virtual XVar round(dynamic _param_value, dynamic _param_isLower, dynamic _param_isKnob = null)
		{
			#region default values
			if(_param_isKnob as Object == null) _param_isKnob = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic isLower = XVar.Clone(_param_isLower);
			dynamic isKnob = XVar.Clone(_param_isKnob);
			#endregion

			dynamic dateArray = XVar.Array(), minYear = null, month = null, monthsNumber = null, step = null, year = null;
			switch(((XVar)this.stepType).ToInt())
			{
				case Constants.FSST_SECONDS:
				case Constants.FSST_MINUTES:
				case Constants.FSST_HOURS:
					return this.getDifferenceInSecWithMin((XVar)(value), (XVar)(isLower));
				case Constants.FSST_DAYS:
					return MVCFunctions.strtotime((XVar)(value)) - MVCFunctions.strtotime((XVar)(this.minValue));
				case Constants.FSST_MONTHS:
					dateArray = XVar.Clone(MVCFunctions.db2time((XVar)(value)));
					year = XVar.Clone(dateArray[0]);
					month = XVar.Clone(dateArray[1]);
					if((XVar)(!(XVar)(isKnob))  || (XVar)(!(XVar)(this.filtered)))
					{
						month = XVar.Clone(this.getAdjustedMonthNumber((XVar)(dateArray[1]), (XVar)(isLower)));
					}
					minYear = XVar.Clone(this.minDateArray[0]);
					monthsNumber = XVar.Clone(((year - minYear) * 12 + month) - this.adjMinDateMonth);
					if(XVar.Pack(!(XVar)(isLower)))
					{
						monthsNumber = XVar.Clone(monthsNumber + 1);
					}
					return monthsNumber;
				case Constants.FSST_YEARS:
					step = XVar.Clone(this.getStepValue());
					dateArray = XVar.Clone(MVCFunctions.db2time((XVar)(value)));
					year = XVar.Clone(dateArray[0]);
					minYear = XVar.Clone(this.minDateArray[0]);
					if(XVar.Pack(isLower))
					{
						return (XVar)Math.Floor((double)((year - minYear) / step)) * step;
					}
					return (XVar)Math.Ceiling((double)((year - minYear) / step)) * step;
				default:
					return value;
			}

			return null;
		}
		protected virtual XVar getAdjustedRealDate(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic dateArray = XVar.Array();
			dateArray = XVar.Clone(this.getDateTimeArray((XVar)(value)));
			if(this.stepType == Constants.FSST_MINUTES)
			{
				dateArray.InitAndSetArrayItem(0, 5);
			}
			return this.getDateTimeString((XVar)(dateArray));
		}
		protected override XVar getBaseContolsMapParams()
		{
			dynamic ctrlsMap = XVar.Array(), timeZone = null;
			ctrlsMap = XVar.Clone(base.getBaseContolsMapParams());
			if(ProjectSettings.ext() == "php")
			{
				timeZone = XVar.Clone(CommonFunctions.date_default_timezone_get());
				CommonFunctions.date_default_timezone_set(new XVar("UTC"));
			}
			ctrlsMap.InitAndSetArrayItem(this.stepType, "stepType");
			if((XVar)(this.stepType == Constants.FSST_SECONDS)  || (XVar)(this.stepType == Constants.FSST_MINUTES))
			{
				ctrlsMap.InitAndSetArrayItem(this.getAdjustedRealDate((XVar)(this.minValue)), "realMinValue");
				ctrlsMap.InitAndSetArrayItem(this.getAdjustedRealDate((XVar)(this.maxValue)), "realMaxValue");
			}
			ctrlsMap.InitAndSetArrayItem(this.getRoundedDate((XVar)(this.minValue), new XVar(true)), "minValue");
			ctrlsMap.InitAndSetArrayItem(this.getRoundedDate((XVar)(this.maxValue), new XVar(false)), "maxValue");
			ctrlsMap.InitAndSetArrayItem(0, "roundedMin");
			ctrlsMap.InitAndSetArrayItem(this.round((XVar)(this.maxValue), new XVar(false)), "roundedMax");
			ctrlsMap.InitAndSetArrayItem(this.round((XVar)(this.minKnobValue), new XVar(true), new XVar(true)), "roundedMinKnobValue");
			ctrlsMap.InitAndSetArrayItem(this.round((XVar)(this.maxKnobValue), new XVar(false), new XVar(true)), "roundedMaxKnobValue");
			if(XVar.Pack(this.filtered))
			{
				ctrlsMap.InitAndSetArrayItem(ctrlsMap["minValue"], "minKnobValue");
				ctrlsMap.InitAndSetArrayItem(ctrlsMap["maxValue"], "maxKnobValue");
				if(this.knobsType != Constants.FS_MAX_ONLY)
				{
					ctrlsMap.InitAndSetArrayItem(this.getRoundedDate((XVar)(this.minKnobFormatValue), new XVar(true), new XVar(true)), "minKnobValue");
				}
				if(this.knobsType != Constants.FS_MIN_ONLY)
				{
					ctrlsMap.InitAndSetArrayItem(this.getRoundedDate((XVar)(this.maxKnobFormatValue), new XVar(false), new XVar(true)), "maxKnobValue");
				}
			}
			if(ProjectSettings.ext() == "php")
			{
				CommonFunctions.date_default_timezone_set((XVar)(timeZone));
			}
			if(this.stepType == Constants.FSST_SECONDS)
			{
				ctrlsMap.InitAndSetArrayItem(true, "showSeconds");
			}
			if((XVar)((XVar)(this.stepType == Constants.FSST_SECONDS)  || (XVar)(this.stepType == Constants.FSST_MINUTES))  || (XVar)(this.stepType == Constants.FSST_HOURS))
			{
				ctrlsMap.InitAndSetArrayItem(true, "showTime");
				ctrlsMap.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_STIME"], "timeDelimiter");
				ctrlsMap.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_STIMEFORMAT"], "timeFormat");
				ctrlsMap.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_ITIME"] == "1", "is24hoursFormat");
				ctrlsMap.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_ITLZERO"] == "1", "leadingZero");
				if(GlobalVars.locale_info["LOCALE_ITIME"] == "0")
				{
					ctrlsMap.InitAndSetArrayItem(new XVar("am", GlobalVars.locale_info["LOCALE_S1159"], "pm", GlobalVars.locale_info["LOCALE_S2359"]), "designators");
				}
			}
			return ctrlsMap;
		}
		public override XVar addFilterControlToControlsMap(dynamic _param_pageObj)
		{
			#region pass-by-value parameters
			dynamic pageObj = XVar.Clone(_param_pageObj);
			#endregion

			dynamic ctrlsMap = XVar.Array();
			ctrlsMap = XVar.Clone(this.getBaseContolsMapParams());
			ctrlsMap.InitAndSetArrayItem(true, "isFieldDateType");
			pageObj.controlsMap.InitAndSetArrayItem(ctrlsMap, "filters", "controls", null);

			return null;
		}
	}
}
