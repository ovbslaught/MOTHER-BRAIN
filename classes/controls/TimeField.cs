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
	public partial class TimeField : DateTimeControl
	{
		protected dynamic timeAttrs;
		protected static bool skipTimeFieldCtor = false;
		private bool skipDateTimeControlCtorSurrogate = new Func<bool>(() => skipDateTimeControlCtor = true).Invoke();
		public TimeField(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection)
		{
			if(skipTimeFieldCtor)
			{
				skipTimeFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.format = new XVar(Constants.EDIT_FORMAT_TIME);
			this.timeAttrs = XVar.Clone(this.pageObject.pSetEdit.getFormatTimeAttrs((XVar)(this.field)));
		}
		public override XVar buildControl(dynamic _param_value, dynamic _param_mode, dynamic _param_fieldNum, dynamic _param_validate, dynamic _param_additionalCtrlParams, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic validate = XVar.Clone(_param_validate);
			dynamic additionalCtrlParams = XVar.Clone(_param_additionalCtrlParams);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if(this.container.pageType == Constants.PAGE_LIST)
			{
				value = XVar.Clone(CommonFunctions.prepare_for_db((XVar)(this.field), (XVar)(value), new XVar("time")));
			}
			base.buildControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.ctype, "\" ", this.inputStyle, " type=\"hidden\" name=\"", this.ctype, "\" value=\"time\">"));
			if(XVar.Pack(MVCFunctions.count(this.timeAttrs)))
			{
				dynamic classString = null, resultHtml = null, var_type = null;
				var_type = new XVar("text");
				classString = new XVar("class=\"form-control\"");
				resultHtml = XVar.Clone(MVCFunctions.Concat("<input ", this.getPlaceholderAttr(), " type=\"", var_type, "\" ", this.inputStyle, " name=\"", this.cfield, "\" ", classString, (XVar.Pack((XVar)((XVar)(mode == Constants.MODE_INLINE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  && (XVar)(this.is508 == true)) ? XVar.Pack(MVCFunctions.Concat("alt=\"", this.strLabel, "\" ")) : XVar.Pack("")), "id=\"", this.cfield, "\" ", this.maxLengthAttr()));
				if(XVar.Pack(this.timeAttrs["useTimePicker"]))
				{
					dynamic convention = null, loc = XVar.Array(), tpVal = XVar.Array();
					convention = XVar.Clone(this.timeAttrs["hours"]);
					loc = XVar.Clone(CommonFunctions.getLacaleAmPmForTimePicker((XVar)(convention), new XVar(true)));
					tpVal = XVar.Clone(CommonFunctions.getValForTimePicker((XVar)(this.var_type), (XVar)(value), (XVar)(loc["locale"])));
					resultHtml = MVCFunctions.Concat(resultHtml, " value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(tpVal["val"])), "\">");
					resultHtml = MVCFunctions.Concat(resultHtml, "<span class=\"input-group-addon\" id=\"trigger-test-", this.cfield, "\"><span class=\"glyphicon glyphicon-time\"></span></span>");
				}
				else
				{
					resultHtml = MVCFunctions.Concat(resultHtml, " value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(this.getOutputValue((XVar)(value)))), "\">");
				}
				if(XVar.Pack(CommonFunctions.isRTL()))
				{
					resultHtml = MVCFunctions.Concat(resultHtml, "<span></span>");
				}
				resultHtml = XVar.Clone(MVCFunctions.Concat("<div class=\"input-group\" ", this.inputStyle, " >", resultHtml, "</div>"));
				MVCFunctions.Echo(resultHtml);
			}
			this.buildControlEnd((XVar)(validate), (XVar)(mode));

			return null;
		}
		protected virtual XVar getOutputValue(dynamic _param_fieldValue)
		{
			#region pass-by-value parameters
			dynamic fieldValue = XVar.Clone(_param_fieldValue);
			#endregion

			dynamic numbers = XVar.Array();
			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(this.var_type))))
			{
				return CommonFunctions.str_format_time((XVar)(MVCFunctions.db2time((XVar)(fieldValue))));
			}
			numbers = XVar.Clone(CommonFunctions.parsenumbers((XVar)(fieldValue)));
			if(XVar.Pack(!(XVar)(numbers)))
			{
				return "";
			}
			while(MVCFunctions.count(numbers) < 3)
			{
				numbers.InitAndSetArrayItem(0, null);
			}
			if(MVCFunctions.count(numbers) == 6)
			{
				return CommonFunctions.str_format_time((XVar)(new XVar(0, 0, 1, 0, 2, 0, 3, numbers[3], 4, numbers[4], 5, numbers[5])));
			}
			return CommonFunctions.str_format_time((XVar)(new XVar(0, 0, 1, 0, 2, 0, 3, numbers[0], 4, numbers[1], 5, numbers[2])));
		}
		public override XVar getFirstElementId()
		{
			return this.cfield;
		}
		public override XVar addCSSFiles()
		{
			this.pageObject.AddCSSFile(new XVar("include/bootstrap/css/bootstrap-datetimepicker.min.css"));

			return null;
		}
	}
}
