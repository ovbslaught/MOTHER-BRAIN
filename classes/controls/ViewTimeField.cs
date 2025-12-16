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
	public partial class ViewTimeField : ViewControl
	{
		protected static bool skipViewTimeFieldCtor = false;
		public ViewTimeField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject) // proxy constructor
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageObject) {}

		public override XVar showDBValue(dynamic data, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic result = null;
			result = XVar.Clone(this.getTextValue((XVar)(data)));
			if((XVar)(!(XVar)(this.container.forExport))  || (XVar)((XVar)(this.container.forExport != "excel")  && (XVar)(this.container.forExport != "csv")))
			{
				result = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(result)));
			}
			return result;
		}
		public override XVar getTextValue(dynamic data)
		{
			dynamic formatData = XVar.Array(), h = null, m = null, s = null, time = XVar.Array();
			time = XVar.Clone(this.getTimeArray((XVar)(data)));
			if(XVar.Pack(!(XVar)(time)))
			{
				return "";
			}
			h = XVar.Clone(time[0]);
			m = XVar.Clone(time[1]);
			s = XVar.Clone(time[2]);
			formatData = XVar.Clone(this.container.pSet.getViewAsTimeFormatData((XVar)(this.field)));
			if(formatData["timeFormat"] == Constants.TIME_FORMAT_DURATION)
			{
				return ViewTimeField.getDuration((XVar)(h), (XVar)(m), (XVar)(s), (XVar)(formatData["showSeconds"]), (XVar)(formatData["showDaysInTotals"]));
			}
			return ViewTimeField.getDayTime((XVar)(h), (XVar)(m), (XVar)(s), (XVar)(formatData["showSeconds"]));
		}
		protected virtual XVar getTimeArray(dynamic data)
		{
			return ViewTimeField.splitStringValue((XVar)(this.fieldType), (XVar)(data[this.field]));
		}
		public static XVar splitStringValue(dynamic _param_fieldType, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic fieldType = XVar.Clone(_param_fieldType);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic time = XVar.Array();
			if(XVar.Pack(!(XVar)(value)))
			{
				return XVar.Array();
			}
			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(fieldType))))
			{
				dynamic date = XVar.Array();
				date = XVar.Clone(MVCFunctions.db2time((XVar)(value)));
				return new XVar(0, date[3], 1, date[4], 2, date[5]);
			}
			time = XVar.Clone(CommonFunctions.parsenumbers((XVar)(value)));
			if(XVar.Pack(!(XVar)(time)))
			{
				return XVar.Array();
			}
			while(MVCFunctions.count(time) < 3)
			{
				time.InitAndSetArrayItem(0, null);
			}
			if(MVCFunctions.count(time) == 6)
			{
				return new XVar(0, time[3], 1, time[4], 2, time[5]);
			}
			return time;
		}
		public static XVar getDuration(dynamic _param_h, dynamic _param_m, dynamic _param_s, dynamic _param_showSeconds, dynamic _param_showDays)
		{
			#region pass-by-value parameters
			dynamic h = XVar.Clone(_param_h);
			dynamic m = XVar.Clone(_param_m);
			dynamic s = XVar.Clone(_param_s);
			dynamic showSeconds = XVar.Clone(_param_showSeconds);
			dynamic showDays = XVar.Clone(_param_showDays);
			#endregion

			dynamic d = null, timeFormat = null;
			d = new XVar(0);
			if(XVar.Pack(showDays))
			{
				d = XVar.Clone((h - h  %  24) / 24);
				h = XVar.Clone(h  %  24);
			}
			timeFormat = XVar.Clone((XVar.Pack(showSeconds) ? XVar.Pack("H:mm:ss") : XVar.Pack("H:mm")));
			return MVCFunctions.Concat((XVar.Pack(d != XVar.Pack(0)) ? XVar.Pack(MVCFunctions.Concat(d, "d ")) : XVar.Pack("")), CommonFunctions.format_datetime_custom((XVar)(new XVar(0, 0, 1, 0, 2, 0, 3, h, 4, m, 5, s)), (XVar)(timeFormat)));
		}
		public static XVar getDayTime(dynamic _param_h, dynamic _param_m, dynamic _param_s, dynamic _param_showSeconds)
		{
			#region pass-by-value parameters
			dynamic h = XVar.Clone(_param_h);
			dynamic m = XVar.Clone(_param_m);
			dynamic s = XVar.Clone(_param_s);
			dynamic showSeconds = XVar.Clone(_param_showSeconds);
			#endregion

			dynamic timeFormat = null;
			timeFormat = XVar.Clone(GlobalVars.locale_info["LOCALE_STIMEFORMAT"]);
			if(XVar.Pack(!(XVar)(showSeconds)))
			{
				timeFormat = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, ":ss", 1, ":s")), (XVar)(new XVar(0, "", 1, "")), (XVar)(timeFormat)));
			}
			return CommonFunctions.format_datetime_custom((XVar)(new XVar(0, 0, 1, 0, 2, 0, 3, h, 4, m, 5, s)), (XVar)(timeFormat));
		}
		public static XVar getFormattedTotals(dynamic _param_field, dynamic _param_value, dynamic _param_pSet_packed, dynamic _param_pdfJsonMode, dynamic _param_forSumTotal)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic pdfJsonMode = XVar.Clone(_param_pdfJsonMode);
			dynamic forSumTotal = XVar.Clone(_param_forSumTotal);
			#endregion

			dynamic formatData = XVar.Array(), h = null, m = null, res = null, s = null;
			if(XVar.Pack(!(XVar)(value)))
			{
				return value;
			}
			s = XVar.Clone(value  %  60);
			value = XVar.Clone((value - s) / 60);
			m = XVar.Clone(value  %  60);
			value = XVar.Clone((value - m) / 60);
			h = XVar.Clone(value);
			formatData = XVar.Clone(pSet.getViewAsTimeFormatData((XVar)(field)));
			if(XVar.Pack(forSumTotal))
			{
				res = XVar.Clone(ViewTimeField.getDuration((XVar)(h), (XVar)(m), (XVar)(s), (XVar)(formatData["showSeconds"]), (XVar)(pSet.showDaysInTimeTotals((XVar)(field)))));
			}
			else
			{
				if(formatData["timeFormat"] == Constants.TIME_FORMAT_DURATION)
				{
					res = XVar.Clone(ViewTimeField.getDuration((XVar)(h), (XVar)(m), (XVar)(s), (XVar)(formatData["showSeconds"]), new XVar(false)));
				}
				else
				{
					res = XVar.Clone(ViewTimeField.getDayTime((XVar)(h), (XVar)(m), (XVar)(s), (XVar)(formatData["showSeconds"])));
				}
			}
			return (XVar.Pack(pdfJsonMode) ? XVar.Pack(MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(res)), "'")) : XVar.Pack(res));
		}
	}
}
