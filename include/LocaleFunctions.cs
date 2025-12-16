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
	public partial class CommonFunctions
	{
		public static XVar str_format_number(dynamic _param_val, dynamic _param_valDigits = null)
		{
			#region default values
			if(_param_valDigits as Object == null) _param_valDigits = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic val = XVar.Clone(_param_val);
			dynamic valDigits = XVar.Clone(_param_valDigits);
			#endregion

			dynamic frac = null, grouping = XVar.Array(), iDigits = null, sign = null, var_int = null, var_out = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.IsNumeric(val))))
			{
				return val;
			}
			iDigits = XVar.Clone(valDigits);
			if(XVar.Equals(XVar.Pack(iDigits), XVar.Pack(false)))
			{
				iDigits = XVar.Clone(GlobalVars.locale_info["LOCALE_IDIGITS"]);
			}
			val = XVar.Clone((XVar)Math.Round((double)(val), iDigits));
			if(XVar.Pack(0) <= val)
			{
				sign = new XVar(1);
				var_int = XVar.Clone((XVar)Math.Floor((double)(val)));
				frac = XVar.Clone(val - var_int);
			}
			else
			{
				sign = XVar.Clone(-1);
				var_int = XVar.Clone((XVar)Math.Floor((double)(-val)));
				frac = XVar.Clone((-val) - var_int);
			}
			var_out = XVar.Clone(MVCFunctions.number_format((XVar)(var_int), new XVar(0), new XVar(""), new XVar("")));
			grouping = XVar.Clone(MVCFunctions.explode(new XVar(";"), (XVar)(GlobalVars.locale_info["LOCALE_SGROUPING"])));
			if((XVar)(MVCFunctions.count(grouping))  && (XVar)(grouping[0]))
			{
				dynamic gi = null, ptr = null;
				ptr = XVar.Clone(MVCFunctions.strlen((XVar)(var_out)));
				gi = new XVar(0);
				for(;gi < MVCFunctions.count(grouping); gi++)
				{
					if(XVar.Pack(!(XVar)(grouping[gi])))
					{
						gi--;
					}
					if(ptr <= grouping[gi])
					{
						ptr = new XVar(0);
						break;
					}
					var_out = XVar.Clone(MVCFunctions.Concat(MVCFunctions.substr((XVar)(var_out), new XVar(0), (XVar)(ptr - grouping[gi])), GlobalVars.locale_info["LOCALE_STHOUSAND"], MVCFunctions.substr((XVar)(var_out), (XVar)(ptr - grouping[gi]))));
					ptr -= grouping[gi];
				}
			}
			if(XVar.Pack(0) < iDigits)
			{
				dynamic fmul = null, i = null, sfrac = null;
				fmul = new XVar(1);
				i = new XVar(0);
				for(;i < iDigits; i++)
				{
					fmul *= 10;
				}
				sfrac = XVar.Clone((XVar)Math.Round((double)(frac * fmul)));
				while(MVCFunctions.strlen((XVar)(sfrac)) < iDigits)
				{
					sfrac = XVar.Clone(MVCFunctions.Concat("0", sfrac));
				}
				var_out = MVCFunctions.Concat(var_out, GlobalVars.locale_info["LOCALE_SDECIMAL"], sfrac);
			}
			if(XVar.Pack(0) < sign)
			{
				return MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_SPOSITIVESIGN"], var_out);
			}
			else
			{
				switch(((XVar)GlobalVars.locale_info["LOCALE_INEGNUMBER"]).ToInt())
				{
					case 0:
						return MVCFunctions.Concat("(", var_out, ")");
					case 1:
						return MVCFunctions.Concat("-", var_out);
					case 2:
						return MVCFunctions.Concat("- ", var_out);
					case 3:
						return MVCFunctions.Concat(var_out, "-");
					case 4:
						return MVCFunctions.Concat(var_out, " -");
				}
			}
			return val;
		}
		public static XVar str_format_currency(dynamic _param_val)
		{
			#region pass-by-value parameters
			dynamic val = XVar.Clone(_param_val);
			#endregion

			dynamic frac = null, grouping = XVar.Array(), sign = null, var_int = null, var_out = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.IsNumeric(val))))
			{
				return val;
			}
			val = XVar.Clone((XVar)Math.Round((double)(val), GlobalVars.locale_info["LOCALE_ICURRDIGITS"]));
			if(XVar.Pack(0) <= val)
			{
				sign = new XVar(1);
				var_int = XVar.Clone((XVar)Math.Floor((double)(val)));
				frac = XVar.Clone(val - var_int);
			}
			else
			{
				sign = XVar.Clone(-1);
				var_int = XVar.Clone((XVar)Math.Floor((double)(-val)));
				frac = XVar.Clone((-val) - var_int);
			}
			var_out = XVar.Clone(MVCFunctions.number_format((XVar)(var_int), new XVar(0), new XVar(""), new XVar("")));
			grouping = XVar.Clone(MVCFunctions.explode(new XVar(";"), (XVar)(GlobalVars.locale_info["LOCALE_SMONGROUPING"])));
			if((XVar)(MVCFunctions.count(grouping))  && (XVar)(grouping[0]))
			{
				dynamic gi = null, ptr = null;
				ptr = XVar.Clone(MVCFunctions.strlen((XVar)(var_out)));
				gi = new XVar(0);
				for(;gi < MVCFunctions.count(grouping); gi++)
				{
					if(XVar.Pack(!(XVar)(grouping[gi])))
					{
						gi--;
					}
					if(ptr <= grouping[gi])
					{
						ptr = new XVar(0);
						break;
					}
					var_out = XVar.Clone(MVCFunctions.Concat(MVCFunctions.substr((XVar)(var_out), new XVar(0), (XVar)(ptr - grouping[gi])), GlobalVars.locale_info["LOCALE_SMONTHOUSANDSEP"], MVCFunctions.substr((XVar)(var_out), (XVar)(ptr - grouping[gi]))));
					ptr -= grouping[gi];
				}
			}
			if(0 < GlobalVars.locale_info["LOCALE_ICURRDIGITS"])
			{
				dynamic fmul = null, i = null, sfrac = null;
				fmul = new XVar(1);
				i = new XVar(0);
				for(;i < GlobalVars.locale_info["LOCALE_ICURRDIGITS"]; i++)
				{
					fmul *= 10;
				}
				frac = XVar.Clone((XVar)Math.Round((double)(frac * fmul)));
				sfrac = XVar.Clone(MVCFunctions.mysprintf(new XVar("%d"), (XVar)(new XVar(0, frac))));
				while(MVCFunctions.strlen((XVar)(sfrac)) < GlobalVars.locale_info["LOCALE_ICURRDIGITS"])
				{
					sfrac = XVar.Clone(MVCFunctions.Concat("0", sfrac));
				}
				var_out = MVCFunctions.Concat(var_out, GlobalVars.locale_info["LOCALE_SMONDECIMALSEP"], sfrac);
			}
			if(XVar.Pack(0) < sign)
			{
				switch(((XVar)GlobalVars.locale_info["LOCALE_ICURRENCY"]).ToInt())
				{
					case 0:
						return MVCFunctions.mysprintf(new XVar("%s%s"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 1:
						return MVCFunctions.mysprintf(new XVar("%s%s"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
					case 2:
						return MVCFunctions.mysprintf(new XVar("%s %s"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 3:
						return MVCFunctions.mysprintf(new XVar("%s %s"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
				}
			}
			else
			{
				switch(((XVar)GlobalVars.locale_info["LOCALE_INEGCURR"]).ToInt())
				{
					case 0:
						return MVCFunctions.mysprintf(new XVar("(%s%s)"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 1:
						return MVCFunctions.mysprintf(new XVar("-%s%s"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 2:
						return MVCFunctions.mysprintf(new XVar("%s-%s"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 3:
						return MVCFunctions.mysprintf(new XVar("%s%s-"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 4:
						return MVCFunctions.mysprintf(new XVar("(%s%s)"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
					case 5:
						return MVCFunctions.mysprintf(new XVar("-%s%s"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
					case 6:
						return MVCFunctions.mysprintf(new XVar("%s-%s"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
					case 7:
						return MVCFunctions.mysprintf(new XVar("%s%s-"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
					case 8:
						return MVCFunctions.mysprintf(new XVar("-%s %s"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
					case 9:
						return MVCFunctions.mysprintf(new XVar("-%s %s"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 10:
						return MVCFunctions.mysprintf(new XVar("%s %s-"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
					case 11:
						return MVCFunctions.mysprintf(new XVar("%s %s-"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 12:
						return MVCFunctions.mysprintf(new XVar("%s -%s"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 13:
						return MVCFunctions.mysprintf(new XVar("%s- %s"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
					case 14:
						return MVCFunctions.mysprintf(new XVar("(%s %s)"), (XVar)(new XVar(0, GlobalVars.locale_info["LOCALE_SCURRENCY"], 1, var_out)));
					case 15:
						return MVCFunctions.mysprintf(new XVar("(%s %s)"), (XVar)(new XVar(0, var_out, 1, GlobalVars.locale_info["LOCALE_SCURRENCY"])));
				}
			}
			return val;
		}
		public static XVar format_datetime_custom(dynamic _param_time, dynamic _param_format)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			dynamic format = XVar.Clone(_param_format);
			#endregion

			dynamic am = null, hour12 = null, i = null, inquot = null, subst = XVar.Array(), var_out = XVar.Array();
			if(MVCFunctions.count(time) < 3)
			{
				return "";
			}
			i = new XVar(0);
			subst = XVar.Clone(XVar.Array());
			if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(format), new XVar("ddd"))), XVar.Pack(false)))
			{
				dynamic weekday = null;
				weekday = XVar.Clone(CommonFunctions.getdayofweek((XVar)(time)));
				subst.InitAndSetArrayItem(GlobalVars.locale_info[MVCFunctions.Concat("LOCALE_SDAYNAME", weekday)], "dddd");
				subst.InitAndSetArrayItem(GlobalVars.locale_info[MVCFunctions.Concat("LOCALE_SABBREVDAYNAME", weekday)], "ddd");
			}
			subst.InitAndSetArrayItem(MVCFunctions.mysprintf(new XVar("%02d"), (XVar)(new XVar(0, time[2]))), "dd");
			subst.InitAndSetArrayItem(time[2], "d");
			if(XVar.Pack(GlobalVars.locale_info.KeyExists(MVCFunctions.Concat("LOCALE_SMONTHNAME", time[1]))))
			{
				subst.InitAndSetArrayItem(GlobalVars.locale_info[MVCFunctions.Concat("LOCALE_SMONTHNAME", time[1])], "MMMM");
				subst.InitAndSetArrayItem(GlobalVars.locale_info[MVCFunctions.Concat("LOCALE_SABBREVMONTHNAME", time[1])], "MMM");
				subst.InitAndSetArrayItem(MVCFunctions.mysprintf(new XVar("%02d"), (XVar)(new XVar(0, time[1]))), "MM");
			}
			else
			{
				subst.InitAndSetArrayItem("", "MMMM");
				subst.InitAndSetArrayItem("", "MMM");
				subst.InitAndSetArrayItem("00", "MM");
			}
			subst.InitAndSetArrayItem(time[1], "M");
			subst.InitAndSetArrayItem(MVCFunctions.mysprintf(new XVar("%04d"), (XVar)(new XVar(0, time[0]))), "yyyy");
			subst.InitAndSetArrayItem(MVCFunctions.mysprintf(new XVar("%02d"), (XVar)(new XVar(0, time[0]  %  100))), "yy");
			subst.InitAndSetArrayItem(time[0]  %  10, "y");
			subst.InitAndSetArrayItem("", "gg");
			subst.InitAndSetArrayItem(MVCFunctions.mysprintf(new XVar("%02d"), (XVar)(new XVar(0, time[3]))), "HH");
			subst.InitAndSetArrayItem(time[3], "H");
			subst.InitAndSetArrayItem(MVCFunctions.mysprintf(new XVar("%02d"), (XVar)(new XVar(0, time[4]))), "mm");
			subst.InitAndSetArrayItem(time[4], "m");
			subst.InitAndSetArrayItem(MVCFunctions.mysprintf(new XVar("%02d"), (XVar)(new XVar(0, time[5]))), "ss");
			subst.InitAndSetArrayItem(time[5], "s");
			hour12 = XVar.Clone(time[3]);
			am = new XVar(1);
			if(12 <= hour12)
			{
				am = new XVar(0);
				hour12 -= 12;
			}
			if(XVar.Pack(!(XVar)(hour12)))
			{
				hour12 = new XVar(12);
			}
			subst.InitAndSetArrayItem(MVCFunctions.mysprintf(new XVar("%02d"), (XVar)(new XVar(0, hour12))), "hh");
			subst.InitAndSetArrayItem(hour12, "h");
			if(XVar.Pack(am))
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strlen((XVar)(GlobalVars.locale_info["LOCALE_S1159"]))), XVar.Pack(0)))
				{
					subst.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_S1159"], "tt");
					subst.InitAndSetArrayItem(MVCFunctions.substr((XVar)(GlobalVars.locale_info["LOCALE_S1159"]), new XVar(0), new XVar(1)), "t");
				}
				else
				{
					subst.InitAndSetArrayItem("am", "tt");
					subst.InitAndSetArrayItem("a", "t");
				}
			}
			else
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strlen((XVar)(GlobalVars.locale_info["LOCALE_S2359"]))), XVar.Pack(0)))
				{
					subst.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_S2359"], "tt");
					subst.InitAndSetArrayItem(MVCFunctions.substr((XVar)(GlobalVars.locale_info["LOCALE_S2359"]), new XVar(0), new XVar(1)), "t");
				}
				else
				{
					subst.InitAndSetArrayItem("pm", "tt");
					subst.InitAndSetArrayItem("p", "t");
				}
			}
			var_out = XVar.Clone(format);
			inquot = new XVar(0);
			while(i < MVCFunctions.strlen((XVar)(var_out)))
			{
				if(var_out[i] == "'")
				{
					inquot = XVar.Clone(1 - inquot);
					var_out = XVar.Clone(MVCFunctions.Concat(MVCFunctions.substr((XVar)(var_out), new XVar(0), (XVar)(i)), MVCFunctions.substr((XVar)(var_out), (XVar)(i + 1))));
					continue;
				}
				else
				{
					if(XVar.Pack(!(XVar)(inquot)))
					{
						foreach (KeyValuePair<XVar, dynamic> value in subst.GetEnumerator())
						{
							if(MVCFunctions.substr((XVar)(var_out), (XVar)(i), (XVar)(MVCFunctions.strlen((XVar)(value.Key)))) == value.Key)
							{
								var_out = XVar.Clone(MVCFunctions.Concat(MVCFunctions.substr((XVar)(var_out), new XVar(0), (XVar)(i)), value.Value, MVCFunctions.substr((XVar)(var_out), (XVar)(MVCFunctions.strlen((XVar)(value.Key)) + i))));
								i += MVCFunctions.strlen((XVar)(value.Value)) - 1;
								break;
							}
						}
					}
				}
				i++;
			}
			return var_out;
		}
		public static XVar str_format_datetime(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			return CommonFunctions.format_datetime_custom((XVar)(time), (XVar)(MVCFunctions.Concat(GlobalVars.locale_info["LOCALE_SSHORTDATE"], " ", GlobalVars.locale_info["LOCALE_STIMEFORMAT"])));
		}
		public static XVar str_format_time(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			return CommonFunctions.format_datetime_custom((XVar)(time), (XVar)(GlobalVars.locale_info["LOCALE_STIMEFORMAT"]));
		}
		public static XVar format_shortdate(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			return CommonFunctions.format_datetime_custom((XVar)(time), (XVar)(GlobalVars.locale_info["LOCALE_SSHORTDATE"]));
		}
		public static XVar normalized_date_format()
		{
			dynamic elements = null;
			elements = XVar.Clone(new XVar(0, "dd", 1, "MM", 2, "yyyy"));
			if(GlobalVars.locale_info["LOCALE_IDATE"] == "0")
			{
				elements = XVar.Clone(new XVar(0, "MM", 1, "dd", 2, "yyyy"));
			}
			else
			{
				if(GlobalVars.locale_info["LOCALE_IDATE"] == "2")
				{
					elements = XVar.Clone(new XVar(0, "yyyy", 1, "MM", 2, "dd"));
				}
			}
			return MVCFunctions.implode((XVar)(GlobalVars.locale_info["LOCALE_SDATE"]), (XVar)(elements));
		}
		public static XVar format_normalized_shortdate(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			return CommonFunctions.format_datetime_custom((XVar)(time), (XVar)(CommonFunctions.normalized_date_format()));
		}
		public static XVar format_longdate(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			return CommonFunctions.format_datetime_custom((XVar)(time), (XVar)(GlobalVars.locale_info["LOCALE_SLONGDATE"]));
		}
		public static XVar simpledate2db(dynamic _param_strdate, dynamic _param_formatid)
		{
			#region pass-by-value parameters
			dynamic strdate = XVar.Clone(_param_strdate);
			dynamic formatid = XVar.Clone(_param_formatid);
			#endregion

			dynamic numbers = XVar.Array(), str = null, vDay = null, vMonth = null, vYear = null;
			str = XVar.Clone(strdate);
			numbers = XVar.Clone(CommonFunctions.parsenumbers((XVar)(str)));
			if(XVar.Pack(!(XVar)(numbers)))
			{
				return strdate;
			}
			while(MVCFunctions.count(numbers) < 3)
			{
				numbers.InitAndSetArrayItem(1, null);
			}
			if(XVar.Pack(!(XVar)(formatid)))
			{
				vMonth = XVar.Clone(numbers[0]);
				vDay = XVar.Clone(numbers[1]);
				vYear = XVar.Clone(numbers[2]);
			}
			else
			{
				if(formatid == 1)
				{
					vDay = XVar.Clone(numbers[0]);
					vMonth = XVar.Clone(numbers[1]);
					vYear = XVar.Clone(numbers[2]);
				}
				else
				{
					if(formatid == 2)
					{
						vYear = XVar.Clone(numbers[0]);
						vMonth = XVar.Clone(numbers[1]);
						vDay = XVar.Clone(numbers[2]);
					}
					else
					{
						return strdate;
					}
				}
			}
			if(vYear < 100)
			{
				if(vYear < 60)
				{
					vYear += 2000;
				}
				else
				{
					vYear += 1900;
				}
			}
			return MVCFunctions.mysprintf(new XVar("%04d-%02d-%02d"), (XVar)(new XVar(0, vYear, 1, vMonth, 2, vDay)));
		}
		public static XVar localdate2db(dynamic _param_strdate)
		{
			#region pass-by-value parameters
			dynamic strdate = XVar.Clone(_param_strdate);
			#endregion

			return CommonFunctions.simpledate2db((XVar)(strdate), (XVar)(GlobalVars.locale_info["LOCALE_IDATE"]));
		}
		public static XVar validTimeValue(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic timeArr = XVar.Array();
			timeArr = XVar.Clone(MVCFunctions.explode(new XVar(":"), (XVar)(value)));
			return (XVar)((XVar)((XVar)((XVar)((XVar)(0 <= timeArr[0])  && (XVar)(timeArr[0] < 24))  && (XVar)(0 <= timeArr[1]))  && (XVar)(timeArr[1] < 60))  && (XVar)(0 <= timeArr[2]))  && (XVar)(timeArr[2] < 60);
		}
		public static XVar localtime2db(dynamic _param_strtime)
		{
			#region pass-by-value parameters
			dynamic strtime = XVar.Clone(_param_strtime);
			#endregion

			dynamic amRegular = null, amstr = null, h = null, isAm = null, isPm = null, m = null, numbers = XVar.Array(), pm = null, pmRegular = null, pmstr = null, pos = null, s = null, str = null, use12 = null;
			use12 = new XVar(0);
			pos = XVar.Clone(MVCFunctions.strpos((XVar)(GlobalVars.locale_info["LOCALE_STIMEFORMAT"]), (XVar)(MVCFunctions.Concat("h", GlobalVars.locale_info["LOCALE_STIME"]))));
			amstr = XVar.Clone(GlobalVars.locale_info["LOCALE_S1159"]);
			pmstr = XVar.Clone(GlobalVars.locale_info["LOCALE_S2359"]);
			amRegular = new XVar("a\\.?m\\.?");
			pmRegular = new XVar("p\\.?m\\.?");
			if(XVar.Pack(MVCFunctions.strlen((XVar)(amstr))))
			{
				amRegular = MVCFunctions.Concat(amRegular, "|", amstr);
			}
			if(XVar.Pack(MVCFunctions.strlen((XVar)(pmstr))))
			{
				pmRegular = MVCFunctions.Concat(pmRegular, "|", pmstr);
			}
			isAm = XVar.Clone(MVCFunctions.preg_match((XVar)(MVCFunctions.Concat("/(", amRegular, ")/isU")), (XVar)(strtime)));
			isPm = XVar.Clone(MVCFunctions.preg_match((XVar)(MVCFunctions.Concat("/(", pmRegular, ")/isU")), (XVar)(strtime)));
			if((XVar)((XVar)(pos)  || (XVar)(isAm))  || (XVar)(isPm))
			{
				use12 = new XVar(1);
				pm = new XVar(0);
				if((XVar)(!(XVar)(isAm))  && (XVar)(isPm))
				{
					pm = new XVar(1);
				}
				else
				{
					if((XVar)(isAm)  && (XVar)(!(XVar)(isPm)))
					{
						pm = new XVar(0);
					}
					else
					{
						if((XVar)(!(XVar)(isAm))  && (XVar)(!(XVar)(isPm)))
						{
							use12 = new XVar(0);
						}
					}
				}
			}
			str = XVar.Clone(strtime);
			numbers = XVar.Clone(CommonFunctions.parsenumbers((XVar)(str)));
			while(MVCFunctions.count(numbers) < 3)
			{
				numbers.InitAndSetArrayItem(0, null);
			}
			h = XVar.Clone(numbers[0]);
			m = XVar.Clone(numbers[1]);
			s = XVar.Clone(numbers[2]);
			if((XVar)(use12)  && (XVar)(h))
			{
				if((XVar)(!(XVar)(pm))  && (XVar)(h == 12))
				{
					h = new XVar(0);
				}
				if((XVar)(pm)  && (XVar)(h < 12))
				{
					h += 12;
				}
			}
			return MVCFunctions.mysprintf(new XVar("%02d:%02d:%02d"), (XVar)(new XVar(0, h, 1, m, 2, s)));
		}
		public static XVar localdatetime2db(dynamic _param_strdatetime, dynamic _param_format = null)
		{
			#region default values
			if(_param_format as Object == null) _param_format = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic strdatetime = XVar.Clone(_param_strdatetime);
			dynamic format = XVar.Clone(_param_format);
			#endregion

			dynamic amRegular = null, amstr = null, h = null, isAm = null, isPm = null, locale_idate = null, m = null, numbers = XVar.Array(), pm = null, pmRegular = null, pmstr = null, pos = null, s = null, use12 = null, vDay = null, vMonth = null, vYear = null;
			locale_idate = XVar.Clone(GlobalVars.locale_info["LOCALE_IDATE"]);
			if(format == "dmy")
			{
				locale_idate = new XVar(1);
			}
			if(format == "mdy")
			{
				locale_idate = new XVar(0);
			}
			if(format == "ymd")
			{
				locale_idate = new XVar(2);
			}
			use12 = new XVar(0);
			pos = XVar.Clone(MVCFunctions.strpos((XVar)(GlobalVars.locale_info["LOCALE_STIMEFORMAT"]), (XVar)(MVCFunctions.Concat("h", GlobalVars.locale_info["LOCALE_STIME"]))));
			amstr = XVar.Clone(GlobalVars.locale_info["LOCALE_S1159"]);
			pmstr = XVar.Clone(GlobalVars.locale_info["LOCALE_S2359"]);
			amRegular = new XVar("a\\.?m\\.?");
			pmRegular = new XVar("p\\.?m\\.?");
			if(XVar.Pack(MVCFunctions.strlen((XVar)(amstr))))
			{
				amRegular = MVCFunctions.Concat(amRegular, "|", amstr);
			}
			if(XVar.Pack(MVCFunctions.strlen((XVar)(pmstr))))
			{
				pmRegular = MVCFunctions.Concat(pmRegular, "|", pmstr);
			}
			isAm = XVar.Clone(MVCFunctions.preg_match((XVar)(MVCFunctions.Concat("/(", amRegular, ")/isU")), (XVar)(strdatetime)));
			isPm = XVar.Clone(MVCFunctions.preg_match((XVar)(MVCFunctions.Concat("/(", pmRegular, ")/isU")), (XVar)(strdatetime)));
			if((XVar)((XVar)(pos)  || (XVar)(isAm))  || (XVar)(isPm))
			{
				use12 = new XVar(1);
				pm = new XVar(0);
				if((XVar)(!(XVar)(isAm))  && (XVar)(isPm))
				{
					pm = new XVar(1);
				}
				else
				{
					if((XVar)(isAm)  && (XVar)(!(XVar)(isPm)))
					{
						pm = new XVar(0);
					}
					else
					{
						if((XVar)(!(XVar)(isAm))  && (XVar)(!(XVar)(isPm)))
						{
							use12 = new XVar(0);
						}
					}
				}
			}
			numbers = XVar.Clone(CommonFunctions.parsenumbers((XVar)(strdatetime)));
			if((XVar)(!(XVar)(numbers))  || (XVar)(MVCFunctions.count(numbers) < 2))
			{
				return "null";
			}
			if(MVCFunctions.count(numbers) < 3)
			{
				if(locale_idate != 1)
				{
					vMonth = XVar.Clone(numbers[0]);
					vDay = XVar.Clone(numbers[1]);
				}
				else
				{
					vMonth = XVar.Clone(numbers[1]);
					vDay = XVar.Clone(numbers[0]);
				}
				vYear = XVar.Clone(MVCFunctions.GetCurrentYear());
			}
			else
			{
				if(XVar.Pack(!(XVar)(locale_idate)))
				{
					vMonth = XVar.Clone(numbers[0]);
					vDay = XVar.Clone(numbers[1]);
					vYear = XVar.Clone(numbers[2]);
				}
				else
				{
					if(locale_idate == 1)
					{
						vDay = XVar.Clone(numbers[0]);
						vMonth = XVar.Clone(numbers[1]);
						vYear = XVar.Clone(numbers[2]);
					}
					else
					{
						if(locale_idate == 2)
						{
							vYear = XVar.Clone(numbers[0]);
							vMonth = XVar.Clone(numbers[1]);
							vDay = XVar.Clone(numbers[2]);
						}
					}
				}
			}
			if((XVar)(!(XVar)(vMonth))  || (XVar)(!(XVar)(vDay)))
			{
				return "null";
			}
			while(MVCFunctions.count(numbers) < 6)
			{
				numbers.InitAndSetArrayItem(0, null);
			}
			h = XVar.Clone(numbers[3]);
			m = XVar.Clone(numbers[4]);
			s = XVar.Clone(numbers[5]);
			if((XVar)(use12)  && (XVar)(h))
			{
				if((XVar)(!(XVar)(pm))  && (XVar)(h == 12))
				{
					h = new XVar(0);
				}
				if((XVar)(pm)  && (XVar)(h < 12))
				{
					h += 12;
				}
			}
			if(vYear < 100)
			{
				if(vYear < 60)
				{
					vYear += 2000;
				}
				else
				{
					vYear += 1900;
				}
			}
			return MVCFunctions.Concat(MVCFunctions.mysprintf(new XVar("%04d-%02d-%02d"), (XVar)(new XVar(0, vYear, 1, vMonth, 2, vDay))), " ", MVCFunctions.mysprintf(new XVar("%02d:%02d:%02d"), (XVar)(new XVar(0, h, 1, m, 2, s))));
		}
		public static XVar parsenumbers(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic i = null, num = null, pos = null, ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			i = new XVar(0);
			num = new XVar(0);
			pos = new XVar(0);
			while(i < MVCFunctions.strlen((XVar)(str)))
			{
				if((XVar)(MVCFunctions.IsNumeric(MVCFunctions.substr((XVar)(str), (XVar)(i), new XVar(1))))  && (XVar)(!(XVar)(num)))
				{
					num = new XVar(1);
					pos = XVar.Clone(i);
				}
				else
				{
					if((XVar)(!(XVar)(MVCFunctions.IsNumeric(MVCFunctions.substr((XVar)(str), (XVar)(i), new XVar(1)))))  && (XVar)(num))
					{
						ret.InitAndSetArrayItem((int)MVCFunctions.substr((XVar)(str), (XVar)(pos), (XVar)(i - pos)), null);
						num = new XVar(0);
					}
				}
				i++;
			}
			if(XVar.Pack(num))
			{
				ret.InitAndSetArrayItem((int)MVCFunctions.substr((XVar)(str), (XVar)(pos), (XVar)(i - pos)), null);
			}
			return ret;
		}
		public static XVar getdayofweek(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			dynamic daydif = null, i = null, mdays = XVar.Array();
			daydif = new XVar(0);
			if(2004 <= time[0])
			{
				i = new XVar(2004);
				for(;i < time[0]; i++)
				{
					if(XVar.Pack(CommonFunctions.isleapyear((XVar)(i))))
					{
						daydif += 366;
					}
					else
					{
						daydif += 365;
					}
				}
			}
			else
			{
				i = new XVar(2003);
				for(;time[0] <= i; i--)
				{
					if(XVar.Pack(CommonFunctions.isleapyear((XVar)(i))))
					{
						daydif -= 366;
					}
					else
					{
						daydif -= 365;
					}
				}
			}
			mdays = XVar.Clone(new XVar(1, 31, 2, 28, 3, 31, 4, 30, 5, 31, 6, 30, 7, 31, 8, 31, 9, 30, 10, 31, 11, 30, 12, 31));
			if(XVar.Pack(CommonFunctions.isleapyear((XVar)(time[0]))))
			{
				mdays.InitAndSetArrayItem(29, 2);
			}
			i = new XVar(1);
			for(;(XVar)(i < time[1])  && (XVar)(i < 13); i++)
			{
				daydif += mdays[i];
			}
			daydif += time[2] - 1;
			if(XVar.Pack(0) < daydif)
			{
				return ((4 + daydif) - 1)  %  7 + 1;
			}
			return 7 - (3 - daydif)  %  7;
		}
		public static XVar getFirstJanuaryDay(dynamic _param_year)
		{
			#region pass-by-value parameters
			dynamic year = XVar.Clone(_param_year);
			#endregion

			dynamic daydif = null, i = null, ret = null;
			daydif = new XVar(0);
			if(2004 <= year)
			{
				i = new XVar(2004);
				for(;i < year; i++)
				{
					if(XVar.Pack(CommonFunctions.isleapyear((XVar)(i))))
					{
						daydif += 366;
					}
					else
					{
						daydif += 365;
					}
				}
			}
			else
			{
				i = new XVar(2003);
				for(;year <= i; i--)
				{
					if(XVar.Pack(CommonFunctions.isleapyear((XVar)(i))))
					{
						daydif -= 366;
					}
					else
					{
						daydif -= 365;
					}
				}
			}
			ret = XVar.Clone((3 + daydif)  %  7);
			if(ret < XVar.Pack(0))
			{
				ret += 7;
			}
			return ret;
		}
		public static XVar getDatesByWeek(dynamic _param_week, dynamic _param_year)
		{
			#region pass-by-value parameters
			dynamic week = XVar.Clone(_param_week);
			dynamic year = XVar.Clone(_param_year);
			#endregion

			dynamic firstDayFirstWeek = null, firstJan = null, firstJanDay = null, firstdayofweek = null, weekEndDate = null, weekStartDate = null;
			firstJanDay = XVar.Clone(CommonFunctions.getFirstJanuaryDay((XVar)(year)));
			firstdayofweek = XVar.Clone((int)GlobalVars.locale_info["LOCALE_IFIRSTDAYOFWEEK"]);
			firstJan = XVar.Clone(new XVar(0, year, 1, 1, 2, 1, 3, 0, 4, 0, 5, 0));
			if(firstdayofweek <= firstJanDay)
			{
				firstDayFirstWeek = XVar.Clone(CommonFunctions.adddays((XVar)(firstJan), (XVar)(firstdayofweek - firstJanDay)));
			}
			else
			{
				firstDayFirstWeek = XVar.Clone(CommonFunctions.adddays((XVar)(firstJan), (XVar)((firstdayofweek - firstJanDay) - 7)));
			}
			weekStartDate = XVar.Clone(CommonFunctions.adddays((XVar)(firstDayFirstWeek), (XVar)(7 * (week - 1))));
			weekEndDate = XVar.Clone(CommonFunctions.adddays((XVar)(weekStartDate), new XVar(6)));
			return new XVar(0, CommonFunctions.date2db((XVar)(weekStartDate)), 1, CommonFunctions.date2db((XVar)(weekEndDate)));
		}
		public static XVar getweeknumber(dynamic _param_time, dynamic _param_firstdayofweek = null)
		{
			#region default values
			if(_param_firstdayofweek as Object == null) _param_firstdayofweek = new XVar(-1);
			#endregion

			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			dynamic firstdayofweek = XVar.Clone(_param_firstdayofweek);
			#endregion

			dynamic daydif = null, firstWeekFirstDayDelta = null, firstYearDay = null, i = null, mdays = XVar.Array();
			if(firstdayofweek < XVar.Pack(0))
			{
				firstdayofweek = XVar.Clone((int)GlobalVars.locale_info["LOCALE_IFIRSTDAYOFWEEK"]);
			}
			mdays = XVar.Clone(new XVar(1, 31, 2, 28, 3, 31, 4, 30, 5, 31, 6, 30, 7, 31, 8, 31, 9, 30, 10, 31, 11, 30, 12, 31));
			if(XVar.Pack(CommonFunctions.isleapyear((XVar)(time[0]))))
			{
				mdays.InitAndSetArrayItem(29, 2);
			}
			i = new XVar(1);
			for(;i < time[1]; i++)
			{
				daydif += mdays[i];
			}
			daydif += time[2] - 1;
			firstYearDay = XVar.Clone(CommonFunctions.getFirstJanuaryDay((XVar)(time[0])));
			if(firstdayofweek <= firstYearDay)
			{
				firstWeekFirstDayDelta = XVar.Clone(firstYearDay - firstdayofweek);
			}
			else
			{
				firstWeekFirstDayDelta = XVar.Clone((7 + firstYearDay) - firstdayofweek);
			}
			daydif += firstWeekFirstDayDelta;
			return (daydif - daydif  %  7) / 7;
		}
		public static XVar getMonthDays(dynamic _param_vYear, dynamic _param_vMonth)
		{
			#region pass-by-value parameters
			dynamic vYear = XVar.Clone(_param_vYear);
			dynamic vMonth = XVar.Clone(_param_vMonth);
			#endregion

			if(vMonth != 2)
			{
				return GlobalVars._gmdays[vMonth];
			}
			return (XVar.Pack(CommonFunctions.isleapyear((XVar)(vYear))) ? XVar.Pack(29) : XVar.Pack(28));
		}
		public static XVar adddays(dynamic _param_tm, dynamic _param_days)
		{
			#region pass-by-value parameters
			dynamic tm = XVar.Clone(_param_tm);
			dynamic days = XVar.Clone(_param_days);
			#endregion

			dynamic time = XVar.Array();
			time = XVar.Clone(tm);
			if(XVar.Pack(0) < days)
			{
				time[2] += days;
				while(CommonFunctions.getMonthDays((XVar)(time[0]), (XVar)(time[1])) < time[2])
				{
					time[2] -= CommonFunctions.getMonthDays((XVar)(time[0]), (XVar)(time[1]));
					time[1]++;
					if(time[1] == 13)
					{
						time.InitAndSetArrayItem(1, 1);
						time[0]++;
					}
				}
			}
			else
			{
				time[2] += days;
				while(time[2] < 1)
				{
					time[2] += CommonFunctions.getMonthDays((XVar)(time[0]), (XVar)(time[1] - 1));
					time[1]--;
					if(time[1] == 0)
					{
						time.InitAndSetArrayItem(12, 1);
						time[0]--;
					}
				}
			}
			return time;
		}
		public static XVar addmonths(dynamic _param_tm, dynamic _param_months)
		{
			#region pass-by-value parameters
			dynamic tm = XVar.Clone(_param_tm);
			dynamic months = XVar.Clone(_param_months);
			#endregion

			dynamic mdays = XVar.Array(), time = XVar.Array();
			mdays = XVar.Clone(new XVar(1, 31, 2, 28, 3, 31, 4, 30, 5, 31, 6, 30, 7, 31, 8, 31, 9, 30, 10, 31, 11, 30, 12, 31));
			time = XVar.Clone(tm);
			time[0] += (int)months / 12;
			time[1] += months  %  12;
			if(12 < time[1])
			{
				time[1] -= 12;
				time[0]++;
			}
			else
			{
				if(time[1] < 1)
				{
					time[1] += 12;
					time[0]--;
				}
			}
			if(XVar.Pack(CommonFunctions.isleapyear((XVar)(time[0]))))
			{
				mdays.InitAndSetArrayItem(29, 2);
			}
			if(mdays[time[1]] < time[2])
			{
				time.InitAndSetArrayItem(mdays[time[1]], 2);
			}
			return time;
		}
		public static XVar addyears(dynamic _param_tm, dynamic _param_years)
		{
			#region pass-by-value parameters
			dynamic tm = XVar.Clone(_param_tm);
			dynamic years = XVar.Clone(_param_years);
			#endregion

			dynamic time = XVar.Array();
			time = XVar.Clone(tm);
			time[0] += years;
			if((XVar)((XVar)(time[2] == 29)  && (XVar)(time[1] == 2))  && (XVar)(!(XVar)(CommonFunctions.isleapyear((XVar)(time[0])))))
			{
				time.InitAndSetArrayItem(28, 2);
			}
			return time;
		}
		public static XVar comparedates(dynamic _param_time1, dynamic _param_time2)
		{
			#region pass-by-value parameters
			dynamic time1 = XVar.Clone(_param_time1);
			dynamic time2 = XVar.Clone(_param_time2);
			#endregion

			dynamic i = null;
			i = new XVar(0);
			for(;i < 6; ++(i))
			{
				if(time1[i] < time2[i])
				{
					return -1;
				}
				if(time2[i] < time1[i])
				{
					return 1;
				}
			}
			return 0;
		}
		public static XVar combinedates(dynamic _param_datePart, dynamic _param_timePart)
		{
			#region pass-by-value parameters
			dynamic datePart = XVar.Clone(_param_datePart);
			dynamic timePart = XVar.Clone(_param_timePart);
			#endregion

			return new XVar(0, datePart[0], 1, datePart[1], 2, datePart[2], 3, timePart[3], 4, timePart[4], 5, timePart[5]);
		}
		public static XVar addHours(dynamic _param_dateArray, dynamic _param_hours)
		{
			#region pass-by-value parameters
			dynamic dateArray = XVar.Clone(_param_dateArray);
			dynamic hours = XVar.Clone(_param_hours);
			#endregion

			dynamic days = null;
			days = XVar.Clone((XVar)Math.Floor((double)(hours / 24)));
			if(XVar.Pack(days))
			{
				dateArray = XVar.Clone(CommonFunctions.adddays((XVar)(dateArray), (XVar)(days)));
				hours = XVar.Clone(hours - days * 24);
			}
			if(dateArray[3] + hours < 24)
			{
				dateArray.InitAndSetArrayItem(dateArray[3] + hours, 3);
			}
			else
			{
				dateArray = XVar.Clone(CommonFunctions.adddays((XVar)(dateArray), new XVar(1)));
				dateArray.InitAndSetArrayItem((dateArray[3] + hours) - 24, 3);
			}
			return dateArray;
		}
		public static XVar addMinutes(dynamic _param_dateArray, dynamic _param_minutes)
		{
			#region pass-by-value parameters
			dynamic dateArray = XVar.Clone(_param_dateArray);
			dynamic minutes = XVar.Clone(_param_minutes);
			#endregion

			dynamic hours = null;
			hours = XVar.Clone((XVar)Math.Floor((double)(minutes / 60)));
			if(XVar.Pack(hours))
			{
				dateArray = XVar.Clone(CommonFunctions.addHours((XVar)(dateArray), (XVar)(hours)));
				minutes = XVar.Clone(minutes - hours * 60);
			}
			if(dateArray[4] + minutes < 60)
			{
				dateArray.InitAndSetArrayItem(dateArray[4] + minutes, 4);
			}
			else
			{
				dateArray = XVar.Clone(CommonFunctions.addHours((XVar)(dateArray), new XVar(1)));
				dateArray.InitAndSetArrayItem((dateArray[4] + minutes) - 60, 4);
			}
			return dateArray;
		}
		public static XVar addSeconds(dynamic _param_dateArray, dynamic _param_seconds)
		{
			#region pass-by-value parameters
			dynamic dateArray = XVar.Clone(_param_dateArray);
			dynamic seconds = XVar.Clone(_param_seconds);
			#endregion

			dynamic minutes = null;
			minutes = XVar.Clone((XVar)Math.Floor((double)(seconds / 60)));
			if(XVar.Pack(minutes))
			{
				dateArray = XVar.Clone(CommonFunctions.addMinutes((XVar)(dateArray), (XVar)(minutes)));
				seconds = XVar.Clone(seconds - minutes * 60);
			}
			if(dateArray[5] + seconds < 60)
			{
				dateArray.InitAndSetArrayItem(dateArray[5] + seconds, 5);
			}
			else
			{
				dateArray = XVar.Clone(CommonFunctions.addMinutes((XVar)(dateArray), new XVar(1)));
				dateArray.InitAndSetArrayItem((dateArray[5] + seconds) - 60, 5);
			}
			return dateArray;
		}
		public static XVar getweekstart(dynamic _param_time, dynamic _param_firstdayofweek = null)
		{
			#region default values
			if(_param_firstdayofweek as Object == null) _param_firstdayofweek = new XVar(-1);
			#endregion

			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			dynamic firstdayofweek = XVar.Clone(_param_firstdayofweek);
			#endregion

			dynamic diff = null, wday = null;
			if(firstdayofweek < XVar.Pack(0))
			{
				firstdayofweek = XVar.Clone(GlobalVars.locale_info["LOCALE_IFIRSTDAYOFWEEK"]);
			}
			wday = XVar.Clone(CommonFunctions.getdayofweek((XVar)(time)));
			if(firstdayofweek + 1 <= wday)
			{
				diff = XVar.Clone((wday - firstdayofweek) - 1);
			}
			else
			{
				diff = XVar.Clone(((wday + 7) - firstdayofweek) - 1);
			}
			return CommonFunctions.adddays((XVar)(time), (XVar)(-diff));
		}
		public static XVar isleapyear(dynamic _param_y)
		{
			#region pass-by-value parameters
			dynamic y = XVar.Clone(_param_y);
			#endregion

			return (XVar)(y  %  4 == 0)  && (XVar)((XVar)(y  %  400 == 0)  || (XVar)(y  %  100 != 0));
		}
		public static XVar GetLongDateFormat()
		{
			dynamic c = null, dindex = null, dstart = null, format = null, i = null, inquote = null, mindex = null, yindex = null;
			format = XVar.Clone(GlobalVars.locale_info["LOCALE_SLONGDATE"]);
			dstart = XVar.Clone(-1);
			inquote = new XVar(false);
			dindex = XVar.Clone(-1);
			mindex = XVar.Clone(-1);
			yindex = XVar.Clone(-1);
			i = new XVar(0);
			for(;true; i++)
			{
				c = new XVar("");
				if(i < MVCFunctions.strlen((XVar)(format)))
				{
					c = XVar.Clone(MVCFunctions.substr((XVar)(format), (XVar)(i), new XVar(1)));
				}
				if((XVar)(XVar.Pack(0) <= dstart)  && (XVar)(c != "d"))
				{
					if(i - dstart <= 2)
					{
						dindex = XVar.Clone(dstart);
					}
					dstart = XVar.Clone(-1);
				}
				if((XVar)(!(XVar)(inquote))  && (XVar)(c == "'"))
				{
					inquote = new XVar(true);
				}
				else
				{
					if(c == "'")
					{
						inquote = new XVar(false);
					}
					else
					{
						if(XVar.Pack(!(XVar)(inquote)))
						{
							if((XVar)(dindex < XVar.Pack(0))  && (XVar)(c == "d"))
							{
								if(dstart < XVar.Pack(0))
								{
									dstart = XVar.Clone(i);
								}
							}
							if((XVar)(yindex < XVar.Pack(0))  && (XVar)(c == "y"))
							{
								yindex = XVar.Clone(i);
							}
							if((XVar)(mindex < XVar.Pack(0))  && (XVar)(c == "M"))
							{
								mindex = XVar.Clone(i);
							}
						}
					}
				}
				if(MVCFunctions.strlen((XVar)(format)) <= i)
				{
					break;
				}
			}
			if((XVar)((XVar)(dindex < XVar.Pack(0))  || (XVar)(mindex < XVar.Pack(0)))  || (XVar)(yindex < XVar.Pack(0)))
			{
				return -1;
			}
			if((XVar)(dindex < mindex)  && (XVar)(mindex < yindex))
			{
				return 1;
			}
			if((XVar)(mindex < dindex)  && (XVar)(dindex < yindex))
			{
				return 0;
			}
			if((XVar)(yindex < mindex)  && (XVar)(mindex < dindex))
			{
				return 2;
			}
			if((XVar)(yindex < dindex)  && (XVar)(dindex < mindex))
			{
				return 1;
			}
			return -1;
		}
		public static XVar addSecondsToTime(dynamic _param_timeArr, dynamic _param_seconds)
		{
			#region pass-by-value parameters
			dynamic timeArr = XVar.Clone(_param_timeArr);
			dynamic seconds = XVar.Clone(_param_seconds);
			#endregion

			dynamic minutes = null;
			minutes = XVar.Clone((XVar)Math.Floor((double)(seconds / 60)));
			if(XVar.Pack(minutes))
			{
				timeArr = XVar.Clone(CommonFunctions.addMinutesToTime((XVar)(timeArr), (XVar)(minutes)));
				seconds = XVar.Clone(seconds - minutes * 60);
			}
			if(timeArr[3] + seconds < 60)
			{
				timeArr.InitAndSetArrayItem(timeArr[3] + seconds, 3);
			}
			else
			{
				timeArr = XVar.Clone(CommonFunctions.addMinutesToTime((XVar)(timeArr), new XVar(1)));
				timeArr.InitAndSetArrayItem((timeArr[3] + seconds) - 60, 3);
			}
			return timeArr;
		}
		public static XVar addMinutesToTime(dynamic _param_timeArr, dynamic _param_minutes)
		{
			#region pass-by-value parameters
			dynamic timeArr = XVar.Clone(_param_timeArr);
			dynamic minutes = XVar.Clone(_param_minutes);
			#endregion

			dynamic hours = null;
			hours = XVar.Clone((XVar)Math.Floor((double)(minutes / 60)));
			if(XVar.Pack(hours))
			{
				timeArr.InitAndSetArrayItem(timeArr[0] + hours, 0);
				minutes = XVar.Clone(minutes - hours * 60);
			}
			if(timeArr[1] + minutes < 60)
			{
				timeArr.InitAndSetArrayItem(timeArr[1] + minutes, 1);
			}
			else
			{
				timeArr.InitAndSetArrayItem(timeArr[0] + 1, 0);
				timeArr.InitAndSetArrayItem((timeArr[1] + minutes) - 60, 1);
			}
			return timeArr;
		}
		public static XVar getLastMonthDayNumber(dynamic _param_year, dynamic _param_vMonth)
		{
			#region pass-by-value parameters
			dynamic year = XVar.Clone(_param_year);
			dynamic vMonth = XVar.Clone(_param_vMonth);
			#endregion

			dynamic mdays = XVar.Array();
			if((XVar)(vMonth == 2)  && (XVar)(CommonFunctions.isleapyear((XVar)(year))))
			{
				return 29;
			}
			mdays = XVar.Clone(new XVar(0, 31, 1, 28, 2, 31, 3, 30, 4, 31, 5, 30, 6, 31, 7, 31, 8, 30, 9, 31, 10, 30, 11, 31));
			return mdays[vMonth - 1];
		}
		public static XVar date2db(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			return MVCFunctions.mysprintf(new XVar("%04d-%02d-%02d"), (XVar)(time));
		}
		public static XVar getDatePart(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			return new XVar(0, time[0], 1, time[1], 2, time[2], 3, 0, 4, 0, 5, 0);
		}
		public static XVar getTimePart(dynamic _param_time)
		{
			#region pass-by-value parameters
			dynamic time = XVar.Clone(_param_time);
			#endregion

			return new XVar(0, 0, 1, 0, 2, 0, 3, time[3], 4, time[4], 5, time[5]);
		}
		public static XVar getDayNameByNumber(dynamic _param_numDay, dynamic _param_format)
		{
			#region pass-by-value parameters
			dynamic numDay = XVar.Clone(_param_numDay);
			dynamic format = XVar.Clone(_param_format);
			#endregion

			if(format == "dddd")
			{
				return GlobalVars.locale_info[MVCFunctions.Concat("LOCALE_SDAYNAME", numDay)];
			}
			else
			{
				if(format == "ddd")
				{
					return GlobalVars.locale_info[MVCFunctions.Concat("LOCALE_SABBREVDAYNAME", numDay)];
				}
			}
			return numDay;
		}
		public static XVar getMonthNameByNumber(dynamic _param_numMon, dynamic _param_format)
		{
			#region pass-by-value parameters
			dynamic numMon = XVar.Clone(_param_numMon);
			dynamic format = XVar.Clone(_param_format);
			#endregion

			if(XVar.Pack(GlobalVars.locale_info.KeyExists(MVCFunctions.Concat("LOCALE_SMONTHNAME", numMon))))
			{
				if(format == "MMMM")
				{
					return GlobalVars.locale_info[MVCFunctions.Concat("LOCALE_SMONTHNAME", numMon)];
				}
				else
				{
					if(format == "MMM")
					{
						return GlobalVars.locale_info[MVCFunctions.Concat("LOCALE_SABBREVMONTHNAME", numMon)];
					}
				}
			}
			return numMon;
		}
		public static XVar dateInDbFormat(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic dbdate = XVar.Array();
			dbdate = XVar.Clone(MVCFunctions.db2time((XVar)(str)));
			if((XVar)(MVCFunctions.count(dbdate) < 3)  || (XVar)(6 < MVCFunctions.count(dbdate)))
			{
				return false;
			}
			if((XVar)((XVar)((XVar)(dbdate[1] < 1)  || (XVar)(12 < dbdate[1]))  || (XVar)(dbdate[2] < 1))  || (XVar)(31 < dbdate[2]))
			{
				return false;
			}
			if(4 <= MVCFunctions.count(dbdate))
			{
				if((XVar)(dbdate[3] < 0)  || (XVar)(23 < dbdate[3]))
				{
					return false;
				}
			}
			if(5 <= MVCFunctions.count(dbdate))
			{
				if((XVar)(dbdate[4] < 0)  || (XVar)(59 < dbdate[4]))
				{
					return false;
				}
			}
			if(MVCFunctions.count(dbdate) == 6)
			{
				if((XVar)(dbdate[5] < 0)  || (XVar)(59 < dbdate[5]))
				{
					return false;
				}
			}
			return true;
		}
		public static XVar dbFormatDateTime(dynamic _param_dt)
		{
			#region pass-by-value parameters
			dynamic dt = XVar.Clone(_param_dt);
			#endregion

			return CommonFunctions.format_datetime_custom((XVar)(dt), new XVar("yyyy-MM-dd HH:mm:ss"));
		}
		public static XVar dbFormatDate(dynamic _param_dt)
		{
			#region pass-by-value parameters
			dynamic dt = XVar.Clone(_param_dt);
			#endregion

			return CommonFunctions.format_datetime_custom((XVar)(dt), new XVar("yyyy-MM-dd"));
		}
		public static XVar dbFormatTime(dynamic _param_dt)
		{
			#region pass-by-value parameters
			dynamic dt = XVar.Clone(_param_dt);
			#endregion

			return CommonFunctions.format_datetime_custom((XVar)(dt), new XVar("HH:mm:ss"));
		}
	}
}
