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
		public static XVar RunnerApply(dynamic obj, dynamic argsArr)
		{
			foreach (KeyValuePair<XVar, dynamic> var_var in argsArr.GetEnumerator())
			{
				MVCFunctions.setObjectProperty((XVar)(obj), (XVar)(var_var.Key), (XVar)(argsArr[var_var.Key]));
			}

			return null;
		}
		public static XVar formatNumberForEdit(dynamic _param_numberStr)
		{
			#region pass-by-value parameters
			dynamic numberStr = XVar.Clone(_param_numberStr);
			#endregion

			return MVCFunctions.str_replace(new XVar("."), (XVar)(GlobalVars.locale_info["LOCALE_SDECIMAL"]), (XVar)(numberStr));
		}
		public static XVar GetImageFromDB(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic connection = null, data = XVar.Array(), dataSource = null, dc = null, field = null, itype = null, table = null, value = null;
			ProjectSettings pSet;
			table = XVar.Clone(var_params["table"]);
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table), (XVar)(var_params["pageType"]), (XVar)(var_params["page"])));
			field = XVar.Clone(var_params["field"]);
			connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(table)));
			dc = XVar.Clone(new DsCommand());
			dc.keys = XVar.Clone(var_params["keys"]);
			dc.filter = XVar.Clone(Security.SelectCondition(new XVar("S"), (XVar)(pSet)));
			dataSource = XVar.Clone(CommonFunctions.getDataSource((XVar)(table), (XVar)(pSet), (XVar)(connection)));
			data = XVar.Clone(dataSource.getSingle((XVar)(dc)).fetchAssoc());
			if(XVar.Pack(!(XVar)(data)))
			{
				return CommonFunctions.DisplayNoImage();
			}
			value = XVar.Clone(connection.stripSlashesBinary((XVar)(data[field])));
			if(XVar.Pack(!(XVar)(value)))
			{
				return CommonFunctions.DisplayNoImage();
			}
			itype = XVar.Clone(MVCFunctions.SupposeImageType((XVar)(value)));
			if(XVar.Pack(!(XVar)(itype)))
			{
				return CommonFunctions.DisplayFile();
			}
			MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Type: ", itype)));
			MVCFunctions.Header("Cache-Control", "private");
			MVCFunctions.SendContentLength((XVar)(MVCFunctions.strlen_bin((XVar)(value))));
			MVCFunctions.echoBinary((XVar)(value));
			return "";
		}
		public static XVar redirectToLogin()
		{
			dynamic expired = null, url = null;
			expired = new XVar("");
			url = new XVar("http://");
			if((XVar)(MVCFunctions.GetServerVariable("HTTPS"))  && (XVar)(MVCFunctions.GetServerVariable("HTTPS") != "off"))
			{
				url = new XVar("https://");
			}
			url = MVCFunctions.Concat(url, MVCFunctions.GetServerVariable("HTTP_HOST"), MVCFunctions.GetServerVariable("REQUEST_URI"));
			if((XVar)(!(XVar)(GlobalVars.logoutPerformed))  && (XVar)(MVCFunctions.SERVERKeyExists("HTTP_REFERER")))
			{
				if((XVar)((XVar)(CommonFunctions.getDirectoryFromURI((XVar)(MVCFunctions.GetServerVariable("HTTP_REFERER"))) == CommonFunctions.getDirectoryFromURI((XVar)(url)))  && (XVar)(CommonFunctions.getFilenameFromURI((XVar)(MVCFunctions.GetServerVariable("HTTP_REFERER"))) != "index.htm"))  && (XVar)(MVCFunctions.GetServerVariable("HTTP_REFERER") != CommonFunctions.getDirectoryFromURI((XVar)(url))))
				{
					expired = new XVar("message=expired");
				}
			}
			if(XVar.Pack(!(XVar)(GlobalVars.logoutPerformed)))
			{
				expired = XVar.Clone(MVCFunctions.Concat("return=true&", expired));
			}
			MVCFunctions.HeaderRedirect(new XVar("login"), new XVar(""), (XVar)(expired));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public static XVar getDirectoryFromURI(dynamic _param_uri)
		{
			#region pass-by-value parameters
			dynamic uri = XVar.Clone(_param_uri);
			#endregion

			dynamic lastSlash = null, qMark = null;
			qMark = XVar.Clone(MVCFunctions.strpos((XVar)(uri), new XVar("?")));
			if(!XVar.Equals(XVar.Pack(qMark), XVar.Pack(false)))
			{
				uri = XVar.Clone(MVCFunctions.substr((XVar)(uri), new XVar(0), (XVar)(qMark)));
			}
			lastSlash = XVar.Clone(MVCFunctions.strrpos((XVar)(uri), new XVar("/")));
			if(!XVar.Equals(XVar.Pack(lastSlash), XVar.Pack(false)))
			{
				return MVCFunctions.Concat(MVCFunctions.substr((XVar)(uri), new XVar(0), (XVar)(lastSlash)), "/");
			}
			return uri;
		}
		public static XVar getFilenameFromURI(dynamic _param_uri)
		{
			#region pass-by-value parameters
			dynamic uri = XVar.Clone(_param_uri);
			#endregion

			dynamic lastSlash = null, qMark = null;
			qMark = XVar.Clone(MVCFunctions.strpos((XVar)(uri), new XVar("?")));
			if(!XVar.Equals(XVar.Pack(qMark), XVar.Pack(false)))
			{
				uri = XVar.Clone(MVCFunctions.substr((XVar)(uri), (XVar)(qMark)));
			}
			lastSlash = XVar.Clone(MVCFunctions.strrpos((XVar)(uri), new XVar("/")));
			if(!XVar.Equals(XVar.Pack(lastSlash), XVar.Pack(false)))
			{
				return MVCFunctions.substr((XVar)(uri), (XVar)(lastSlash + 1));
			}
			return uri;
		}
		public static XVar getLangFileName(dynamic _param_langName)
		{
			#region pass-by-value parameters
			dynamic langName = XVar.Clone(_param_langName);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> lang in ProjectSettings.languageDescriptors().GetEnumerator())
			{
				if(lang.Value["name"] == langName)
				{
					return langName;
				}
			}
			return "";
		}
		public static XVar DisplayMap(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["addressField"]) ? XVar.Pack(var_params["addressField"]) : XVar.Pack("")), "mapsData", var_params["id"], "addressField");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["latField"]) ? XVar.Pack(var_params["latField"]) : XVar.Pack("")), "mapsData", var_params["id"], "latField");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["lngField"]) ? XVar.Pack(var_params["lngField"]) : XVar.Pack("")), "mapsData", var_params["id"], "lngField");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["weightField"]) ? XVar.Pack(var_params["weightField"]) : XVar.Pack("")), "mapsData", var_params["id"], "weightField");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem(var_params["clustering"], "mapsData", var_params["id"], "clustering");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem(var_params["heatMap"], "mapsData", var_params["id"], "heatMap");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar)((XVar)(var_params["showAllMarkers"])  || (XVar)(var_params["clustering"]))  || (XVar)(var_params["heatMap"]), "mapsData", var_params["id"], "showAllMarkers");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["width"]) ? XVar.Pack(var_params["width"]) : XVar.Pack(0)), "mapsData", var_params["id"], "width");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["height"]) ? XVar.Pack(var_params["height"]) : XVar.Pack(0)), "mapsData", var_params["id"], "height");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem("BIG_MAP", "mapsData", var_params["id"], "type");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["showCenterLink"]) ? XVar.Pack(var_params["showCenterLink"]) : XVar.Pack(0)), "mapsData", var_params["id"], "showCenterLink");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["descField"]) ? XVar.Pack(var_params["descField"]) : XVar.Pack(GlobalVars.pageObject.googleMapCfg["mapsData"][var_params["id"]]["addressField"])), "mapsData", var_params["id"], "descField");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["description"]) ? XVar.Pack(var_params["description"]) : XVar.Pack(GlobalVars.pageObject.googleMapCfg["mapsData"][var_params["id"]]["addressField"])), "mapsData", var_params["id"], "descField");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem(var_params["markerAsEditLink"], "mapsData", var_params["id"], "markerAsEditLink");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["markerIcon"]) ? XVar.Pack(var_params["markerIcon"]) : XVar.Pack("")), "mapsData", var_params["id"], "markerIcon");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["markerField"]) ? XVar.Pack(var_params["markerField"]) : XVar.Pack("")), "mapsData", var_params["id"], "markerField");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["showCurrentLocation"]) ? XVar.Pack(var_params["showCurrentLocation"]) : XVar.Pack(false)), "mapsData", var_params["id"], "showCurrentLocation");
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["currentLocationIcon"]) ? XVar.Pack(var_params["currentLocationIcon"]) : XVar.Pack("")), "mapsData", var_params["id"], "currentLocationIcon");
			if(XVar.Pack(var_params.KeyExists("zoom")))
			{
				GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem(var_params["zoom"], "mapsData", var_params["id"], "zoom");
			}
			if(XVar.Pack(GlobalVars.pageObject.googleMapCfg["mapsData"][var_params["id"]]["showCenterLink"]))
			{
				GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem((XVar.Pack(var_params["centerLinkText"]) ? XVar.Pack(var_params["centerLinkText"]) : XVar.Pack("")), "mapsData", var_params["id"], "centerLinkText");
			}
			GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem(var_params["id"], "mainMapIds", null);
			if(XVar.Pack(var_params.KeyExists("APIkey")))
			{
				GlobalVars.pageObject.googleMapCfg.InitAndSetArrayItem(var_params["APIkey"], "APIcode");
			}

			return null;
		}
		public static XVar GetPasswordField()
		{
			return Security.passwordField();
		}
		public static XVar GetUserNameField()
		{
			return Security.usernameField();
		}
		public static XVar GetDisplayNameField(dynamic _param_table = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			return Security.fullnameField();
		}
		public static XVar GetEmailField(dynamic _param_table = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			return Security.emailField();
		}
		public static XVar GetTablesListWithoutSecurity()
		{
			dynamic arr = XVar.Array();
			arr = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> _table in GlobalVars.runnerProjectSettings["allTables"].GetEnumerator())
			{
				arr.InitAndSetArrayItem(_table.Value["name"], null);
			}
			return arr;
		}
		public static XVar GetShorteningForLargeText(dynamic _param_strValue, dynamic _param_cNumberOfChars, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic strValue = XVar.Clone(_param_strValue);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(MVCFunctions.runner_substr((XVar)(strValue), new XVar(0), (XVar)(cNumberOfChars)));
			if(XVar.Pack(html))
			{
				return MVCFunctions.runner_htmlspecialchars((XVar)(ret));
			}
			return ret;
		}
		public static XVar AddLinkPrefix(dynamic _param_pSet_packed, dynamic _param_field, dynamic _param_link)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic field = XVar.Clone(_param_field);
			dynamic link = XVar.Clone(_param_link);
			#endregion

			if((XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(link), new XVar("://"))), XVar.Pack(false)))  && (XVar)(MVCFunctions.substr((XVar)(link), new XVar(0), new XVar(7)) != "mailto:"))
			{
				return MVCFunctions.Concat(pSet.getLinkPrefix((XVar)(field)), link);
			}
			return link;
		}
		public static XVar GetTotalsForTime(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic time = XVar.Array();
			time = XVar.Clone(CommonFunctions.parsenumbers((XVar)(value)));
			while(MVCFunctions.count(time) < 3)
			{
				time.InitAndSetArrayItem(0, null);
			}
			return time;
		}
		public static XVar GetTotals(dynamic _param_field, dynamic _param_value, dynamic _param_stype, dynamic _param_iNumberOfRows, dynamic _param_sFormat, dynamic _param_ptype, dynamic _param_pSet_packed, dynamic _param_useRawValue, dynamic _param_pageObject)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			dynamic stype = XVar.Clone(_param_stype);
			dynamic iNumberOfRows = XVar.Clone(_param_iNumberOfRows);
			dynamic sFormat = XVar.Clone(_param_sFormat);
			dynamic ptype = XVar.Clone(_param_ptype);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic useRawValue = XVar.Clone(_param_useRawValue);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			dynamic sValue = null;
			if(stype == "AVERAGE")
			{
				if(XVar.Pack(!(XVar)(iNumberOfRows)))
				{
					return "";
				}
				if(sFormat == Constants.FORMAT_TIME)
				{
					if(XVar.Pack(value))
					{
						value = XVar.Clone(ViewTimeField.getFormattedTotals((XVar)(field), (XVar)((XVar)Math.Round((double)(value / iNumberOfRows), 0)), (XVar)(pSet), (XVar)(pageObject.pdfJsonMode()), new XVar(false)));
					}
				}
				else
				{
					value = XVar.Clone((XVar)Math.Round((double)(value / iNumberOfRows), 2));
				}
			}
			if((XVar)(stype == "TOTAL")  || (XVar)(stype == "SUM"))
			{
				if(sFormat == Constants.FORMAT_TIME)
				{
					value = XVar.Clone(ViewTimeField.getFormattedTotals((XVar)(field), (XVar)(value), (XVar)(pSet), (XVar)(pageObject.pdfJsonMode()), new XVar(true)));
				}
			}
			if((XVar)(stype == "COUNT")  || (XVar)(useRawValue))
			{
				return value;
			}
			sValue = XVar.Clone(value);
			if((XVar)((XVar)((XVar)(sFormat == Constants.FORMAT_CURRENCY)  || (XVar)(sFormat == Constants.FORMAT_PERCENT))  || (XVar)(sFormat == Constants.FORMAT_NUMBER))  || (XVar)(sFormat == Constants.FORMAT_CUSTOM))
			{
				dynamic data = null, viewControls = null;
				data = XVar.Clone(new XVar(field, value));
				if(XVar.Pack(!(XVar)(pageObject)))
				{
					viewControls = XVar.Clone(new ViewControlsContainer((XVar)(pSet), (XVar)(ptype)));
				}
				else
				{
					viewControls = XVar.Clone(pageObject);
				}
				sValue = XVar.Clone(viewControls.showDBValue((XVar)(field), (XVar)(data)));
			}
			if((XVar)((XVar)(stype == "TOTAL")  || (XVar)(stype == "SUM"))  || (XVar)(stype == "AVERAGE"))
			{
				return sValue;
			}
			return "";
		}
		public static XVar DisplayNoImage()
		{
			dynamic path = null;
			path = XVar.Clone(MVCFunctions.getabspath(new XVar("images/no_image.gif")));
			MVCFunctions.Header("Content-Type", "image/gif");
			MVCFunctions.printfile((XVar)(path));

			return null;
		}
		public static XVar DisplayFile()
		{
			dynamic path = null;
			path = XVar.Clone(MVCFunctions.getabspath(new XVar("images/file.gif")));
			MVCFunctions.Header("Content-Type", "image/gif");
			MVCFunctions.printfile((XVar)(path));

			return null;
		}
		public static XVar my_strrpos(dynamic _param_haystack, dynamic _param_needle)
		{
			#region pass-by-value parameters
			dynamic haystack = XVar.Clone(_param_haystack);
			dynamic needle = XVar.Clone(_param_needle);
			#endregion

			dynamic index = null;
			index = XVar.Clone(MVCFunctions.strpos((XVar)(MVCFunctions.strrev((XVar)(haystack))), (XVar)(MVCFunctions.strrev((XVar)(needle)))));
			if(XVar.Equals(XVar.Pack(index), XVar.Pack(false)))
			{
				return false;
			}
			index = XVar.Clone((MVCFunctions.strlen((XVar)(haystack)) - MVCFunctions.strlen((XVar)(needle))) - index);
			return index;
		}
		public static XVar jsreplace(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\\", 1, "'", 2, "\"", 3, "\r", 4, "\n")), (XVar)(new XVar(0, "\\\\", 1, "\\'", 2, "\\\"", 3, "\\r", 4, "\\n")), (XVar)(str)));
			return CommonFunctions.my_str_ireplace(new XVar("</script>"), new XVar("</scr'+'ipt>"), (XVar)(ret));
		}
		public static XVar LogInfo(dynamic _param_SQL)
		{
			#region pass-by-value parameters
			dynamic SQL = XVar.Clone(_param_SQL);
			#endregion


			return null;
		}
		public static XVar CheckImageExtension(dynamic _param_filename)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			#endregion

			dynamic ext = null;
			if(MVCFunctions.strlen((XVar)(filename)) < 4)
			{
				return false;
			}
			ext = XVar.Clone(MVCFunctions.strtoupper((XVar)(MVCFunctions.substr((XVar)(filename), (XVar)(MVCFunctions.strlen((XVar)(filename)) - 4)))));
			if((XVar)((XVar)((XVar)((XVar)((XVar)(ext == ".GIF")  || (XVar)(ext == ".JPG"))  || (XVar)(ext == "JPEG"))  || (XVar)(ext == ".PNG"))  || (XVar)(ext == ".BMP"))  || (XVar)(ext == "WEBP"))
			{
				return ext;
			}
			return false;
		}
		public static XVar html_special_decode(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(str);
			ret = XVar.Clone(MVCFunctions.str_replace(new XVar("&gt;"), new XVar(">"), (XVar)(ret)));
			ret = XVar.Clone(MVCFunctions.str_replace(new XVar("&lt;"), new XVar("<"), (XVar)(ret)));
			ret = XVar.Clone(MVCFunctions.str_replace(new XVar("&quot;"), new XVar("\""), (XVar)(ret)));
			ret = XVar.Clone(MVCFunctions.str_replace(new XVar("&#039;"), new XVar("'"), (XVar)(ret)));
			ret = XVar.Clone(MVCFunctions.str_replace(new XVar("&#39;"), new XVar("'"), (XVar)(ret)));
			ret = XVar.Clone(MVCFunctions.str_replace(new XVar("&amp;"), new XVar("&"), (XVar)(ret)));
			return ret;
		}
		public static XVar whereAdd(dynamic _param_where, dynamic _param_clause)
		{
			#region pass-by-value parameters
			dynamic where = XVar.Clone(_param_where);
			dynamic clause = XVar.Clone(_param_clause);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(clause)))))
			{
				return where;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(where)))))
			{
				return clause;
			}
			return MVCFunctions.Concat("(", where, ") and (", clause, ")");
		}
		public static XVar combineSQLCriteria(dynamic _param_arrElements, dynamic _param_and = null)
		{
			#region default values
			if(_param_and as Object == null) _param_and = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic arrElements = XVar.Clone(_param_arrElements);
			dynamic var_and = XVar.Clone(_param_and);
			#endregion

			dynamic filteredCriteria = XVar.Array(), union = null;
			filteredCriteria = XVar.Clone(XVar.Array());
			union = XVar.Clone((XVar.Pack(var_and) ? XVar.Pack(" AND ") : XVar.Pack(" OR ")));
			foreach (KeyValuePair<XVar, dynamic> e in arrElements.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(e.Value))))
				{
					filteredCriteria.InitAndSetArrayItem(MVCFunctions.Concat("( ", e.Value, " )"), null);
				}
			}
			return MVCFunctions.implode((XVar)(union), (XVar)(filteredCriteria));
		}
		public static XVar sqlMoreThan(dynamic _param_field, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(value == "null")
			{
				return MVCFunctions.Concat(field, " is not null");
			}
			return MVCFunctions.Concat(field, " > ", value);
		}
		public static XVar sqlLessThan(dynamic _param_field, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(value == "null")
			{
				return "";
			}
			return MVCFunctions.Concat(field, " < ", value, " or ", field, " is null");
		}
		public static XVar sqlEqual(dynamic _param_field, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(value == "null")
			{
				return MVCFunctions.Concat(value, " is null");
			}
			return MVCFunctions.Concat(field, " = ", value);
		}
		public static XVar AddWhere(dynamic _param_sql, dynamic _param_where)
		{
			#region pass-by-value parameters
			dynamic sql = XVar.Clone(_param_sql);
			dynamic where = XVar.Clone(_param_where);
			#endregion

			dynamic n = null, n1 = null, n2 = null, tsql = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(where)))))
			{
				return sql;
			}
			sql = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(sql)));
			tsql = XVar.Clone(MVCFunctions.strtolower((XVar)(sql)));
			n = XVar.Clone(CommonFunctions.my_strrpos((XVar)(tsql), new XVar(" where ")));
			n1 = XVar.Clone(CommonFunctions.my_strrpos((XVar)(tsql), new XVar(" group by ")));
			n2 = XVar.Clone(CommonFunctions.my_strrpos((XVar)(tsql), new XVar(" order by ")));
			if(XVar.Equals(XVar.Pack(n1), XVar.Pack(false)))
			{
				n1 = XVar.Clone(MVCFunctions.strlen((XVar)(tsql)));
			}
			if(XVar.Equals(XVar.Pack(n2), XVar.Pack(false)))
			{
				n2 = XVar.Clone(MVCFunctions.strlen((XVar)(tsql)));
			}
			if(n2 < n1)
			{
				n1 = XVar.Clone(n2);
			}
			if(XVar.Equals(XVar.Pack(n), XVar.Pack(false)))
			{
				return MVCFunctions.Concat(MVCFunctions.substr((XVar)(sql), new XVar(0), (XVar)(n1)), " where ", where, MVCFunctions.substr((XVar)(sql), (XVar)(n1)));
			}
			else
			{
				return MVCFunctions.Concat(MVCFunctions.substr((XVar)(sql), new XVar(0), (XVar)(n + MVCFunctions.strlen(new XVar(" where ")))), "(", MVCFunctions.substr((XVar)(sql), (XVar)(n + MVCFunctions.strlen(new XVar(" where "))), (XVar)((n1 - n) - MVCFunctions.strlen(new XVar(" where ")))), ") and (", where, ")", MVCFunctions.substr((XVar)(sql), (XVar)(n1)));
			}

			return null;
		}
		public static XVar KeyWhere(dynamic keys, dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic cipherer = null, connection = null, keyFields = XVar.Array(), strWhere = null;
			ProjectSettings pSet;
			strWhere = new XVar("");
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			cipherer = XVar.Clone(new RunnerCipherer((XVar)(table)));
			connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(table)));
			if(XVar.Pack(!(XVar)(connection.dbBased())))
			{
				return "";
			}
			keyFields = XVar.Clone(pSet.getTableKeys());
			foreach (KeyValuePair<XVar, dynamic> kf in keyFields.GetEnumerator())
			{
				dynamic value = null, valueisnull = null;
				if(XVar.Pack(MVCFunctions.strlen((XVar)(strWhere))))
				{
					strWhere = MVCFunctions.Concat(strWhere, " and ");
				}
				value = XVar.Clone(cipherer.MakeDBValue((XVar)(kf.Value), (XVar)(keys[kf.Value]), new XVar(""), new XVar(true)));
				if(connection.dbType == Constants.nDATABASE_Oracle)
				{
					valueisnull = XVar.Clone((XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack("null")))  || (XVar)(value == "''"));
				}
				else
				{
					valueisnull = XVar.Clone(XVar.Equals(XVar.Pack(value), XVar.Pack("null")));
				}
				if(XVar.Pack(valueisnull))
				{
					strWhere = MVCFunctions.Concat(strWhere, RunnerPage._getFieldSQL((XVar)(kf.Value), (XVar)(connection), (XVar)(pSet)), " is null");
				}
				else
				{
					strWhere = MVCFunctions.Concat(strWhere, RunnerPage._getFieldSQLDecrypt((XVar)(kf.Value), (XVar)(connection), (XVar)(pSet), (XVar)(cipherer)), "=", cipherer.MakeDBValue((XVar)(kf.Value), (XVar)(keys[kf.Value]), new XVar(""), new XVar(true)));
				}
			}
			return strWhere;
		}
		public static XVar GetRowCount(dynamic _param_strSQL, dynamic _param_connection)
		{
			#region pass-by-value parameters
			dynamic strSQL = XVar.Clone(_param_strSQL);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			dynamic countdata = XVar.Array(), countstr = null, ind1 = null, ind2 = null, ind3 = null, tstr = null;
			strSQL = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(strSQL)));
			tstr = XVar.Clone(MVCFunctions.strtoupper((XVar)(strSQL)));
			ind1 = XVar.Clone(MVCFunctions.strpos((XVar)(tstr), new XVar("SELECT ")));
			ind2 = XVar.Clone(CommonFunctions.my_strrpos((XVar)(tstr), new XVar(" FROM ")));
			ind3 = XVar.Clone(CommonFunctions.my_strrpos((XVar)(tstr), new XVar(" GROUP BY ")));
			if(XVar.Equals(XVar.Pack(ind3), XVar.Pack(false)))
			{
				ind3 = XVar.Clone(MVCFunctions.strpos((XVar)(tstr), new XVar(" ORDER BY ")));
				if(XVar.Equals(XVar.Pack(ind3), XVar.Pack(false)))
				{
					ind3 = XVar.Clone(MVCFunctions.strlen((XVar)(strSQL)));
				}
			}
			countstr = XVar.Clone(MVCFunctions.Concat(MVCFunctions.substr((XVar)(strSQL), new XVar(0), (XVar)(ind1 + 6)), " count(*) ", MVCFunctions.substr((XVar)(strSQL), (XVar)(ind2 + 1), (XVar)(ind3 - ind2))));
			countdata = XVar.Clone(connection.query((XVar)(countstr)).fetchNumeric());
			return countdata[0];
		}
		public static XVar AddTop(dynamic _param_strSQL, dynamic _param_n)
		{
			#region pass-by-value parameters
			dynamic strSQL = XVar.Clone(_param_strSQL);
			dynamic n = XVar.Clone(_param_n);
			#endregion

			dynamic matches = XVar.Array(), pattern = null;
			pattern = new XVar("/(^\\s*select\\s+distinct\\s+)|(^\\s*select\\s+)/i");
			matches = XVar.Clone(XVar.Array());
			MVCFunctions.preg_match_all((XVar)(pattern), (XVar)(strSQL), (XVar)(matches));
			if(XVar.Pack(matches[0]))
			{
				return MVCFunctions.Concat(matches[0][0], "top ", n, " ", MVCFunctions.substr((XVar)(strSQL), (XVar)(MVCFunctions.strlen((XVar)(matches[0][0])))));
			}
			return strSQL;
		}
		public static XVar AddTopDB2(dynamic _param_strSQL, dynamic _param_n)
		{
			#region pass-by-value parameters
			dynamic strSQL = XVar.Clone(_param_strSQL);
			dynamic n = XVar.Clone(_param_n);
			#endregion

			return MVCFunctions.Concat(strSQL, " fetch first ", n, " rows only");
		}
		public static XVar AddTopIfx(dynamic _param_strSQL, dynamic _param_n)
		{
			#region pass-by-value parameters
			dynamic strSQL = XVar.Clone(_param_strSQL);
			dynamic n = XVar.Clone(_param_n);
			#endregion

			return MVCFunctions.Concat(MVCFunctions.substr((XVar)(strSQL), new XVar(0), new XVar(7)), "limit ", n, " ", MVCFunctions.substr((XVar)(strSQL), new XVar(7)));
		}
		public static XVar AddRowNumber(dynamic _param_strSQL, dynamic _param_n)
		{
			#region pass-by-value parameters
			dynamic strSQL = XVar.Clone(_param_strSQL);
			dynamic n = XVar.Clone(_param_n);
			#endregion

			return MVCFunctions.Concat("select * from (", strSQL, ") where rownum<", n + 1);
		}
		public static XVar applyDBrecordLimit(dynamic _param_sql, dynamic _param_N, dynamic _param_dbType)
		{
			#region pass-by-value parameters
			dynamic sql = XVar.Clone(_param_sql);
			dynamic N = XVar.Clone(_param_N);
			dynamic dbType = XVar.Clone(_param_dbType);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(dbType)))))
			{
				return sql;
			}
			if((XVar)((XVar)(dbType == Constants.nDATABASE_MySQL)  || (XVar)(dbType == Constants.nDATABASE_PostgreSQL))  || (XVar)(dbType == Constants.nDATABASE_SQLite3))
			{
				return MVCFunctions.Concat(sql, " LIMIT ", N);
			}
			if(dbType == Constants.nDATABASE_Oracle)
			{
				return CommonFunctions.AddRowNumber((XVar)(sql), (XVar)(N));
			}
			if((XVar)(dbType == Constants.nDATABASE_MSSQLServer)  || (XVar)(dbType == Constants.nDATABASE_Access))
			{
				return CommonFunctions.AddTop((XVar)(sql), (XVar)(N));
			}
			if(dbType == Constants.nDATABASE_Informix)
			{
				return CommonFunctions.AddTopIfx((XVar)(sql), (XVar)(N));
			}
			if(dbType == Constants.nDATABASE_DB2)
			{
				return CommonFunctions.AddTopDB2((XVar)(sql), (XVar)(N));
			}
			return sql;
		}
		public static XVar NeedQuotesNumeric(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(var_type == 203)  || (XVar)(var_type == 8))  || (XVar)(var_type == 129))  || (XVar)(var_type == 130))  || (XVar)(var_type == 7))  || (XVar)(var_type == 133))  || (XVar)(var_type == 134))  || (XVar)(var_type == 135))  || (XVar)(var_type == 201))  || (XVar)(var_type == 205))  || (XVar)(var_type == 200))  || (XVar)(var_type == 202))  || (XVar)(var_type == 72))  || (XVar)(var_type == 13))
			{
				return true;
			}
			else
			{
				return false;
			}

			return null;
		}
		public static XVar IsNumberType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(var_type == 20)  || (XVar)(var_type == 14))  || (XVar)(var_type == 5))  || (XVar)(var_type == 10))  || (XVar)(var_type == 6))  || (XVar)(var_type == 3))  || (XVar)(var_type == 131))  || (XVar)(var_type == 4))  || (XVar)(var_type == 2))  || (XVar)(var_type == 16))  || (XVar)(var_type == 21))  || (XVar)(var_type == 19))  || (XVar)(var_type == 18))  || (XVar)(var_type == 17))  || (XVar)(var_type == 139))  || (XVar)(var_type == 11))
			{
				return true;
			}
			return false;
		}
		public static XVar IsFloatType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if((XVar)((XVar)((XVar)((XVar)(var_type == 14)  || (XVar)(var_type == 5))  || (XVar)(var_type == 131))  || (XVar)(var_type == 4))  || (XVar)(var_type == 6))
			{
				return true;
			}
			return false;
		}
		public static XVar NeedQuotes(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			return !(XVar)(CommonFunctions.IsNumberType((XVar)(var_type)));
		}
		public static XVar IsBinaryType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if((XVar)((XVar)(var_type == 128)  || (XVar)(var_type == 205))  || (XVar)(var_type == 204))
			{
				return true;
			}
			return false;
		}
		public static XVar IsDateFieldType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if((XVar)((XVar)(var_type == 7)  || (XVar)(var_type == 133))  || (XVar)(var_type == 135))
			{
				return true;
			}
			return false;
		}
		public static XVar IsDateTimeFieldType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if(var_type == 135)
			{
				return true;
			}
			return false;
		}
		public static XVar IsTimeType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if((XVar)(var_type == 134)  || (XVar)(var_type == 145))
			{
				return true;
			}
			return false;
		}
		public static XVar IsCharType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if((XVar)((XVar)((XVar)((XVar)((XVar)(CommonFunctions.IsTextType((XVar)(var_type)))  || (XVar)(var_type == 8))  || (XVar)(var_type == 129))  || (XVar)(var_type == 200))  || (XVar)(var_type == 202))  || (XVar)(var_type == 130))
			{
				return true;
			}
			return false;
		}
		public static XVar IsTextType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if((XVar)(var_type == 201)  || (XVar)(var_type == 203))
			{
				return true;
			}
			return false;
		}
		public static XVar IsGuid(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if(var_type == 72)
			{
				return true;
			}
			return false;
		}
		public static XVar IsBigInt(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if(var_type == 20)
			{
				return true;
			}
			return false;
		}
		public static XVar GetUserPermissionsDynamic(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic userRights = XVar.Array();
			if(XVar.Pack(!(XVar)(CommonFunctions.isLogged())))
			{
				return "";
			}
			if((XVar)(Security.isAdmin())  && (XVar)(Security.isAdminTable((XVar)(table))))
			{
				return "ADESPIM";
			}
			userRights = Security.dynamicUserRights();
			return userRights[table]["mask"];
		}
		public static XVar IsAdmin()
		{
			return Security.isAdmin();
		}
		public static XVar GetUserPermissions(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic permissions = null;
			permissions = new XVar("");
			if(XVar.Pack(Security.hasLogin()))
			{
				if(XVar.Pack(!(XVar)(CommonFunctions.isLogged())))
				{
					return "";
				}
			}
			if(XVar.Pack(MVCFunctions.is_array((XVar)(XSession.Session["securityOverrides"]))))
			{
				if(XVar.Pack(XSession.Session["securityOverrides"].KeyExists(table)))
				{
					if(XVar.Pack(XSession.Session["securityOverrides"][table].KeyExists("mask")))
					{
						return XSession.Session["securityOverrides"][table]["mask"];
					}
				}
			}
			if(XVar.Pack(Security.permissionsAvailable()))
			{
				if(XVar.Pack(Security.dynamicPermissions()))
				{
					permissions = XVar.Clone(CommonFunctions.GetUserPermissionsDynamic((XVar)(table)));
				}
				else
				{
					permissions = XVar.Clone(Security.getUserPermissionsStatic((XVar)(table)));
				}
			}
			else
			{
				permissions = new XVar("ADESPI");
			}
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("GetTablePermissions"), (XVar)(table))))
			{
				permissions = XVar.Clone(GlobalVars.globalEvents.GetTablePermissions((XVar)(permissions), (XVar)(table)));
			}
			return permissions;
		}
		public static XVar menuLinkAvailable(dynamic _param_table, dynamic _param_pageType, dynamic _param_page = null)
		{
			#region default values
			if(_param_page as Object == null) _param_page = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic page = XVar.Clone(_param_page);
			#endregion

			if(table == Constants.WEBREPORTS_TABLE)
			{
				return true;
			}
			if(XVar.Pack(CommonFunctions.isAdminPage((XVar)(pageType))))
			{
				return Security.isAdmin();
			}
			if(XVar.Pack(!(XVar)(page)))
			{
				dynamic permission = null;
				permission = XVar.Clone(Security.pageType2permission((XVar)(pageType)));
				return Security.userCan((XVar)(permission), (XVar)(table));
			}
			return Security.userCanSeePage((XVar)(table), (XVar)(page));
		}
		public static XVar isLogged()
		{
			if(XVar.Pack(!(XVar)(Security.hasLogin())))
			{
				return true;
			}
			if(XVar.Pack(!(XVar)(Security.verifySafeCSRF())))
			{
				return false;
			}
			return !(XVar)(!(XVar)(Security.getUserName()));
		}
		public static XVar guestHasPermissions()
		{
			if(XVar.Equals(XVar.Pack(GlobalVars.gGuestHasPermissions), XVar.Pack(-1)))
			{
				if(XVar.Pack(Security.dynamicPermissions()))
				{
					GlobalVars.gGuestHasPermissions = XVar.Clone((XVar.Pack(Security.guestHasDynamicPermissions()) ? XVar.Pack(1) : XVar.Pack(0)));
				}
				else
				{
					GlobalVars.gGuestHasPermissions = XVar.Clone((XVar.Pack(Security.guestHasStaticPermissions()) ? XVar.Pack(1) : XVar.Pack(0)));
				}
			}
			return XVar.Equals(XVar.Pack(GlobalVars.gGuestHasPermissions), XVar.Pack(1));
		}
		public static XVar CheckTablePermissions(dynamic _param_strTableName, dynamic _param_permission)
		{
			#region pass-by-value parameters
			dynamic strTableName = XVar.Clone(_param_strTableName);
			dynamic permission = XVar.Clone(_param_permission);
			#endregion

			if(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(CommonFunctions.GetUserPermissions((XVar)(strTableName))), (XVar)(permission))), XVar.Pack(false)))
			{
				return false;
			}
			return true;
		}
		public static XVar pagetypeToPermissions(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			if(XVar.Pack(!(XVar)(GlobalVars._pagetypeToPermissions_dict)))
			{
				GlobalVars._pagetypeToPermissions_dict = XVar.Clone(XVar.Array());
				GlobalVars._pagetypeToPermissions_dict.InitAndSetArrayItem("S", "list");
				GlobalVars._pagetypeToPermissions_dict.InitAndSetArrayItem("S", "search");
				GlobalVars._pagetypeToPermissions_dict.InitAndSetArrayItem("S", "view");
				GlobalVars._pagetypeToPermissions_dict.InitAndSetArrayItem("A", "add");
				GlobalVars._pagetypeToPermissions_dict.InitAndSetArrayItem("E", "edit");
				GlobalVars._pagetypeToPermissions_dict.InitAndSetArrayItem("P", "print");
				GlobalVars._pagetypeToPermissions_dict.InitAndSetArrayItem("P", "export");
				GlobalVars._pagetypeToPermissions_dict.InitAndSetArrayItem("I", "import");
			}
			return GlobalVars._pagetypeToPermissions_dict[pageType];
		}
		public static XVar make_db_value(dynamic _param_field, dynamic _param_value, dynamic _param_controltype = null, dynamic _param_postfilename = null, dynamic _param_table = null)
		{
			#region default values
			if(_param_controltype as Object == null) _param_controltype = new XVar("");
			if(_param_postfilename as Object == null) _param_postfilename = new XVar("");
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			dynamic controltype = XVar.Clone(_param_controltype);
			dynamic postfilename = XVar.Clone(_param_postfilename);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(CommonFunctions.prepare_for_db((XVar)(field), (XVar)(value), (XVar)(controltype), (XVar)(postfilename), (XVar)(table)));
			if(XVar.Equals(XVar.Pack(ret), XVar.Pack(false)))
			{
				return ret;
			}
			return CommonFunctions.add_db_quotes((XVar)(field), (XVar)(ret), (XVar)(table));
		}
		public static XVar add_db_quotes(dynamic _param_field, dynamic _param_value, dynamic _param_table = null, dynamic _param_type = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			if(_param_type as Object == null) _param_type = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			dynamic table = XVar.Clone(_param_table);
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			dynamic connection = null;
			ProjectSettings pSet;
			if(table == XVar.Pack(""))
			{
				table = XVar.Clone(GlobalVars.strTableName);
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(table)));
			if(var_type == null)
			{
				var_type = XVar.Clone(pSet.getFieldType((XVar)(field)));
			}
			if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
			{
				return connection.addSlashesBinary((XVar)(value));
			}
			if((XVar)((XVar)((XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack("")))  || (XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack(false))))  || (XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack(null))))  && (XVar)(!(XVar)(CommonFunctions.IsCharType((XVar)(var_type)))))
			{
				return "null";
			}
			if(XVar.Pack(CommonFunctions.NeedQuotes((XVar)(var_type))))
			{
				if(XVar.Pack(!(XVar)(CommonFunctions.IsDateFieldType((XVar)(var_type)))))
				{
					value = XVar.Clone(connection.prepareString((XVar)(value)));
				}
				else
				{
					dynamic d = null, delim = null, m = null, matches = null, reg = null, y = null;
					y = new XVar("(\\d\\d\\d\\d)");
					m = new XVar("(0?[1-9]|1[0-2])");
					d = new XVar("(0?[1-9]|[1-2][0-9]|3[0-1])");
					delim = XVar.Clone(MVCFunctions.Concat("(-|", MVCFunctions.preg_quote((XVar)(GlobalVars.locale_info["LOCALE_SDATE"]), new XVar("/")), ")"));
					reg = XVar.Clone(MVCFunctions.Concat("/", d, delim, m, delim, y, "|", m, delim, d, delim, y, "|", y, delim, m, delim, d, "/"));
					if(XVar.Pack(!(XVar)(MVCFunctions.preg_match((XVar)(reg), (XVar)(value), (XVar)(matches)))))
					{
						return "null";
					}
					value = XVar.Clone(connection.addDateQuotes((XVar)(value)));
				}
			}
			else
			{
				if((XVar)(connection.dbType == Constants.nDATABASE_PostgreSQL)  && (XVar)(var_type == 11))
				{
					value = XVar.Clone(MVCFunctions.strtolower((XVar)(value)));
					if((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(!(XVar)(MVCFunctions.strlen((XVar)(value))))  || (XVar)(value == XVar.Pack(0)))  || (XVar)(value == "0"))  || (XVar)(value == "false"))  || (XVar)(value == "f"))  || (XVar)(value == "n"))  || (XVar)(value == "no"))  || (XVar)(value == "off"))
					{
						value = new XVar("f");
					}
					else
					{
						value = new XVar("t");
					}
					value = XVar.Clone(connection.prepareString((XVar)(value)));
				}
				else
				{
					value = XVar.Clone(DB.prepareNumberValue((XVar)(value)));
				}
			}
			return value;
		}
		public static XVar prepare_for_db(dynamic _param_field, dynamic _param_value, dynamic _param_controltype = null, dynamic _param_postfilename = null, dynamic _param_table = null)
		{
			#region default values
			if(_param_controltype as Object == null) _param_controltype = new XVar("");
			if(_param_postfilename as Object == null) _param_postfilename = new XVar("");
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			dynamic controltype = XVar.Clone(_param_controltype);
			dynamic postfilename = XVar.Clone(_param_postfilename);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic connection = null, filename = null, var_type = null;
			ProjectSettings pSet;
			if(controltype == "display")
			{
				return value;
			}
			if(table == XVar.Pack(""))
			{
				table = XVar.Clone(GlobalVars.strTableName);
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(table)));
			filename = new XVar("");
			var_type = XVar.Clone(pSet.getFieldType((XVar)(field)));
			if((XVar)((XVar)(!(XVar)(controltype))  || (XVar)(controltype == "multiselect"))  && (XVar)(!(XVar)(CommonFunctions.IsTimeType((XVar)(var_type)))))
			{
				if(XVar.Pack(MVCFunctions.is_array((XVar)(value))))
				{
					value = XVar.Clone(CommonFunctions.combineLookupValues((XVar)(value)));
				}
				if((XVar)((XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack("")))  || (XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack(false))))  && (XVar)(!(XVar)(CommonFunctions.IsCharType((XVar)(var_type)))))
				{
					return "";
				}
				if(XVar.Pack(CommonFunctions.IsGuid((XVar)(var_type))))
				{
					if(XVar.Pack(!(XVar)(CommonFunctions.IsGuidString(ref value))))
					{
						return "";
					}
				}
				if(XVar.Pack(CommonFunctions.IsFloatType((XVar)(var_type))))
				{
					return MVCFunctions.makeFloat((XVar)(value));
				}
				if((XVar)(CommonFunctions.IsNumberType((XVar)(var_type)))  && (XVar)(!(XVar)(MVCFunctions.IsNumeric(value))))
				{
					value = XVar.Clone(MVCFunctions.trim((XVar)(value)));
					if(XVar.Pack(!(XVar)(MVCFunctions.IsNumeric(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)(value))))))
					{
						value = new XVar("");
					}
				}
				return value;
			}
			else
			{
				dynamic time = null;
				if((XVar)(controltype == "time")  || (XVar)(CommonFunctions.IsTimeType((XVar)(var_type))))
				{
					if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(value)))))
					{
						return "";
					}
					time = XVar.Clone(CommonFunctions.localtime2db((XVar)(value)));
					if(connection.dbType == Constants.nDATABASE_PostgreSQL)
					{
						dynamic timeArr = XVar.Array();
						timeArr = XVar.Clone(MVCFunctions.explode(new XVar(":"), (XVar)(time)));
						if((XVar)((XVar)(24 < timeArr[0])  || (XVar)(59 < timeArr[1]))  || (XVar)(59 < timeArr[2]))
						{
							return "";
						}
					}
					if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(var_type))))
					{
						time = XVar.Clone(MVCFunctions.Concat("2000-01-01 ", time));
					}
					return time;
				}
				else
				{
					if(MVCFunctions.substr((XVar)(controltype), new XVar(0), new XVar(4)) == "date")
					{
						dynamic dformat = null;
						dformat = XVar.Clone(MVCFunctions.substr((XVar)(controltype), new XVar(4)));
						if((XVar)((XVar)(dformat == Constants.EDIT_DATE_SIMPLE)  || (XVar)(dformat == Constants.EDIT_DATE_SIMPLE_INLINE))  || (XVar)(dformat == Constants.EDIT_DATE_SIMPLE_DP))
						{
							time = XVar.Clone(CommonFunctions.localdatetime2db((XVar)(value)));
							if(time == "null")
							{
								return "";
							}
							return time;
						}
						else
						{
							if((XVar)((XVar)(dformat == Constants.EDIT_DATE_DD)  || (XVar)(dformat == Constants.EDIT_DATE_DD_INLINE))  || (XVar)(dformat == Constants.EDIT_DATE_DD_DP))
							{
								dynamic a = XVar.Array(), d = null, m = null, y = null;
								a = XVar.Clone(MVCFunctions.explode(new XVar("-"), (XVar)(value)));
								if(MVCFunctions.count(a) < 3)
								{
									return "";
								}
								else
								{
									y = XVar.Clone(a[0]);
									m = XVar.Clone(a[1]);
									d = XVar.Clone(a[2]);
								}
								if(y < 100)
								{
									if(y < 70)
									{
										y += 2000;
									}
									else
									{
										y += 1900;
									}
								}
								return MVCFunctions.mysprintf(new XVar("%04d-%02d-%02d"), (XVar)(new XVar(0, y, 1, m, 2, d)));
							}
							else
							{
								return "";
							}
						}
					}
					else
					{
						if(MVCFunctions.substr((XVar)(controltype), new XVar(0), new XVar(8)) == "checkbox")
						{
							dynamic ret = null;
							if(value == "on")
							{
								ret = new XVar(1);
							}
							else
							{
								if(value == "none")
								{
									return "";
								}
								else
								{
									ret = new XVar(0);
								}
							}
							return ret;
						}
						else
						{
							return false;
						}
					}
				}
			}

			return null;
		}
		public static XVar DeleteUploadedFiles(dynamic _param_pSet_packed, dynamic _param_deleted_values)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic deleted_values = XVar.Clone(_param_deleted_values);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> value in deleted_values.GetEnumerator())
			{
				dynamic filesArray = XVar.Array();
				if((XVar)(pSet.getEditFormat((XVar)(value.Key)) != Constants.EDIT_FORMAT_FILE)  && (XVar)(!(XVar)(pSet.getPageTypeByFieldEditFormat((XVar)(value.Key), new XVar(Constants.EDIT_FORMAT_FILE)))))
				{
					continue;
				}
				if(XVar.Pack(!(XVar)(pSet.isDeleteAssociatedFile((XVar)(value.Key)))))
				{
					continue;
				}
				filesArray = XVar.Clone(RunnerFileHandler.getFileArray((XVar)(value.Value), (XVar)(value.Key), (XVar)(pSet)));
				foreach (KeyValuePair<XVar, dynamic> delFile in filesArray.GetEnumerator())
				{
					dynamic fs = null;
					fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(pSet), (XVar)(value.Key)));
					fs.delete((XVar)(delFile.Value["name"]));
					if(XVar.Pack(delFile.Value["thumbnail"]))
					{
						fs.delete((XVar)(delFile.Value["thumbnail"]));
					}
				}
			}

			return null;
		}
		public static XVar combineLookupValues(dynamic _param_arr)
		{
			#region pass-by-value parameters
			dynamic arr = XVar.Clone(_param_arr);
			#endregion

			dynamic added = XVar.Array(), data = XVar.Array();
			added = XVar.Clone(XVar.Array());
			data = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in arr.GetEnumerator())
			{
				if((XVar)(v.Value != XVar.Pack(""))  && (XVar)(!(XVar)(added[v.Value])))
				{
					data.InitAndSetArrayItem(v.Value, null);
					added.InitAndSetArrayItem(true, v.Value);
				}
			}
			return CommonFunctions.combinevalues((XVar)(data));
		}
		public static XVar combinevalues(dynamic _param_arr)
		{
			#region pass-by-value parameters
			dynamic arr = XVar.Clone(_param_arr);
			#endregion

			dynamic ret = null;
			ret = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> item in arr.GetEnumerator())
			{
				dynamic val = null;
				val = XVar.Clone(item.Value);
				if(XVar.Pack(MVCFunctions.strlen((XVar)(ret))))
				{
					ret = MVCFunctions.Concat(ret, ",");
				}
				if((XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(val), new XVar(","))), XVar.Pack(false)))  && (XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(val), new XVar("\""))), XVar.Pack(false))))
				{
					ret = MVCFunctions.Concat(ret, val);
				}
				else
				{
					val = XVar.Clone(MVCFunctions.str_replace(new XVar("\""), new XVar("\"\""), (XVar)(val)));
					ret = MVCFunctions.Concat(ret, "\"", val, "\"");
				}
			}
			return ret;
		}
		public static XVar splitLookupValues(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic added = XVar.Array(), ret = XVar.Array(), values = XVar.Array();
			values = XVar.Clone(CommonFunctions.splitvalues((XVar)(str)));
			ret = XVar.Clone(XVar.Array());
			added = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				if((XVar)(added[v.Value])  || (XVar)(XVar.Equals(XVar.Pack(v.Value), XVar.Pack(""))))
				{
					continue;
				}
				added.InitAndSetArrayItem(true, v.Value);
				ret.InitAndSetArrayItem(v.Value, null);
			}
			return ret;
		}
		public static XVar splitvalues(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic arr = XVar.Array(), i = null, inquot = null, start = null;
			arr = XVar.Clone(XVar.Array());
			if(str == XVar.Pack(""))
			{
				arr.InitAndSetArrayItem("", null);
				return arr;
			}
			start = new XVar(0);
			i = new XVar(0);
			inquot = new XVar(false);
			while(i <= MVCFunctions.strlen((XVar)(str)))
			{
				if((XVar)(i < MVCFunctions.strlen((XVar)(str)))  && (XVar)(MVCFunctions.substr((XVar)(str), (XVar)(i), new XVar(1)) == "\""))
				{
					inquot = XVar.Clone(!(XVar)(inquot));
				}
				else
				{
					if((XVar)(i == MVCFunctions.strlen((XVar)(str)))  || (XVar)((XVar)(!(XVar)(inquot))  && (XVar)(MVCFunctions.substr((XVar)(str), (XVar)(i), new XVar(1)) == ",")))
					{
						dynamic val = null;
						val = XVar.Clone(MVCFunctions.substr((XVar)(str), (XVar)(start), (XVar)(i - start)));
						start = XVar.Clone(i + 1);
						if((XVar)(MVCFunctions.strlen((XVar)(val)))  && (XVar)(MVCFunctions.substr((XVar)(val), new XVar(0), new XVar(1)) == "\""))
						{
							val = XVar.Clone(MVCFunctions.substr((XVar)(val), new XVar(1), (XVar)(MVCFunctions.strlen((XVar)(val)) - 2)));
							val = XVar.Clone(MVCFunctions.str_replace(new XVar("\"\""), new XVar("\""), (XVar)(val)));
						}
						if(!XVar.Equals(XVar.Pack(val), XVar.Pack(false)))
						{
							arr.InitAndSetArrayItem(val, null);
						}
					}
				}
				i++;
			}
			return arr;
		}
		public static XVar getLacaleAmPmForTimePicker(dynamic _param_convention, dynamic _param_useTimePicker = null)
		{
			#region default values
			if(_param_useTimePicker as Object == null) _param_useTimePicker = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic convention = XVar.Clone(_param_convention);
			dynamic useTimePicker = XVar.Clone(_param_useTimePicker);
			#endregion

			dynamic am = null, locale = null, pm = null;
			am = new XVar("");
			pm = new XVar("");
			if(XVar.Pack(useTimePicker))
			{
				dynamic locale_convention = null;
				locale_convention = XVar.Clone((XVar.Pack(GlobalVars.locale_info["LOCALE_ITIME"]) ? XVar.Pack(24) : XVar.Pack(12)));
				if(convention == locale_convention)
				{
					am = XVar.Clone(GlobalVars.locale_info["LOCALE_S1159"]);
					pm = XVar.Clone(GlobalVars.locale_info["LOCALE_S2359"]);
					locale = XVar.Clone(GlobalVars.locale_info["LOCALE_STIMEFORMAT"]);
				}
				else
				{
					if(convention == 24)
					{
						am = new XVar("");
						pm = new XVar("");
						locale = new XVar("H:mm:ss");
					}
					else
					{
						am = new XVar("am");
						pm = new XVar("pm");
						locale = new XVar("h:mm:ss tt");
					}
				}
			}
			else
			{
				locale = XVar.Clone(GlobalVars.locale_info["LOCALE_STIMEFORMAT"]);
			}
			return new XVar("am", am, "pm", pm, "locale", locale);
		}
		public static XVar getValForTimePicker(dynamic _param_type, dynamic _param_value, dynamic _param_locale)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic value = XVar.Clone(_param_value);
			dynamic locale = XVar.Clone(_param_locale);
			#endregion

			dynamic dbtime = null, val = null;
			val = new XVar("");
			dbtime = XVar.Clone(XVar.Array());
			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(var_type))))
			{
				dbtime = XVar.Clone(MVCFunctions.db2time((XVar)(value)));
				if(XVar.Pack(MVCFunctions.count(dbtime)))
				{
					val = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(dbtime), (XVar)(locale)));
				}
			}
			else
			{
				dynamic arr = XVar.Array();
				arr = XVar.Clone(CommonFunctions.parsenumbers((XVar)(value)));
				if(XVar.Pack(MVCFunctions.count(arr)))
				{
					while(MVCFunctions.count(arr) < 3)
					{
						arr.InitAndSetArrayItem(0, null);
					}
					dbtime = XVar.Clone(new XVar(0, 0, 1, 0, 2, 0, 3, arr[0], 4, arr[1], 5, arr[2]));
					val = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(dbtime), (XVar)(locale)));
				}
			}
			return new XVar("val", val, "dbTime", dbtime);
		}
		public static XVar my_stripos(dynamic _param_str, dynamic _param_needle, dynamic _param_offest)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			dynamic needle = XVar.Clone(_param_needle);
			dynamic offest = XVar.Clone(_param_offest);
			#endregion

			if((XVar)(MVCFunctions.strlen((XVar)(needle)) == 0)  || (XVar)(MVCFunctions.strlen((XVar)(str)) == 0))
			{
				return false;
			}
			return MVCFunctions.strpos((XVar)(MVCFunctions.strtolower((XVar)(str))), (XVar)(MVCFunctions.strtolower((XVar)(needle))), (XVar)(offest));
		}
		public static XVar my_str_ireplace(dynamic _param_search, dynamic _param_replace, dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic search = XVar.Clone(_param_search);
			dynamic replace = XVar.Clone(_param_replace);
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic pos = null;
			pos = XVar.Clone(CommonFunctions.my_stripos((XVar)(str), (XVar)(search), new XVar(0)));
			if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
			{
				return str;
			}
			return MVCFunctions.Concat(MVCFunctions.substr((XVar)(str), new XVar(0), (XVar)(pos)), replace, MVCFunctions.substr((XVar)(str), (XVar)(pos + MVCFunctions.strlen((XVar)(search)))));
		}
		public static XVar in_assoc_array(dynamic _param_name, dynamic _param_arr)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic arr = XVar.Clone(_param_arr);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> value in arr.GetEnumerator())
			{
				if(value.Key == name)
				{
					return true;
				}
			}
			return false;
		}
		public static XVar xmlencode(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			str = XVar.Clone(MVCFunctions.str_replace(new XVar("&"), new XVar("&amp;"), (XVar)(str)));
			str = XVar.Clone(MVCFunctions.str_replace(new XVar("<"), new XVar("&lt;"), (XVar)(str)));
			str = XVar.Clone(MVCFunctions.str_replace(new XVar(">"), new XVar("&gt;"), (XVar)(str)));
			str = XVar.Clone(MVCFunctions.str_replace(new XVar("'"), new XVar("&apos;"), (XVar)(str)));
			return MVCFunctions.escapeEntities((XVar)(str));
		}
		public static XVar print_inline_array(dynamic arr, dynamic _param_printkey = null)
		{
			#region default values
			if(_param_printkey as Object == null) _param_printkey = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic printkey = XVar.Clone(_param_printkey);
			#endregion

			if(XVar.Pack(!(XVar)(printkey)))
			{
				foreach (KeyValuePair<XVar, dynamic> val in arr.GetEnumerator())
				{
					MVCFunctions.Echo(MVCFunctions.Concat(MVCFunctions.str_replace((XVar)(new XVar(0, "&", 1, "<", 2, "\\", 3, "\r", 4, "\n")), (XVar)(new XVar(0, "&amp;", 1, "&lt;", 2, "\\\\", 3, "\\r", 4, "\\n")), (XVar)(MVCFunctions.str_replace((XVar)(new XVar(0, "\\", 1, "\r", 2, "\n")), (XVar)(new XVar(0, "\\\\", 1, "\\r", 2, "\\n")), (XVar)(val.Value)))), "\\n"));
				}
			}
			else
			{
				foreach (KeyValuePair<XVar, dynamic> val in arr.GetEnumerator())
				{
					MVCFunctions.Echo(MVCFunctions.Concat(MVCFunctions.str_replace((XVar)(new XVar(0, "&", 1, "<", 2, "\\", 3, "\r", 4, "\n")), (XVar)(new XVar(0, "&amp;", 1, "&lt;", 2, "\\\\", 3, "\\r", 4, "\\n")), (XVar)(MVCFunctions.str_replace((XVar)(new XVar(0, "\\", 1, "\r", 2, "\n")), (XVar)(new XVar(0, "\\\\", 1, "\\r", 2, "\\n")), (XVar)(val.Key)))), "\\n"));
				}
			}

			return null;
		}
		public static XVar checkpassword(dynamic _param_pwd)
		{
			#region pass-by-value parameters
			dynamic pwd = XVar.Clone(_param_pwd);
			#endregion

			dynamic c = null, cDigit = null, cLower = null, cUnique = XVar.Array(), cUpper = null, i = null, len = null, pwdDigitsSymbols = null, pwdMinLength = null, pwdUniqueChars = null, pwdUpperLower = null;
			if(XVar.Pack(!(XVar)(ProjectSettings.passwordValidationValue(new XVar("strong")))))
			{
				return true;
			}
			pwdMinLength = XVar.Clone(ProjectSettings.passwordValidationValue(new XVar("minimumLength")));
			pwdUniqueChars = XVar.Clone(ProjectSettings.passwordValidationValue(new XVar("uniqueCharacters")));
			pwdDigitsSymbols = XVar.Clone(ProjectSettings.passwordValidationValue(new XVar("digitsAndSymbols")));
			pwdUpperLower = XVar.Clone(ProjectSettings.passwordValidationValue(new XVar("upperAndLowerCase")));
			len = XVar.Clone(MVCFunctions.strlen((XVar)(pwd)));
			if((XVar)(pwdMinLength)  && (XVar)(len < pwdMinLength))
			{
				return false;
			}
			cUnique = XVar.Clone(XVar.Array());
			cLower = XVar.Clone(cUpper = XVar.Clone(cDigit = new XVar(0)));
			i = new XVar(0);
			for(;i < len; i++)
			{
				c = XVar.Clone(MVCFunctions.substr((XVar)(pwd), (XVar)(i), new XVar(1)));
				if((XVar)("a" <= c)  && (XVar)(c <= "z"))
				{
					cLower++;
				}
				else
				{
					if((XVar)("A" <= c)  && (XVar)(c <= "Z"))
					{
						cUpper++;
					}
					else
					{
						cDigit++;
					}
				}
				cUnique.InitAndSetArrayItem(1, c);
			}
			if((XVar)(pwdUniqueChars)  && (XVar)(MVCFunctions.count(cUnique) < pwdUniqueChars))
			{
				return false;
			}
			if((XVar)(pwdDigitsSymbols)  && (XVar)(cDigit < pwdDigitsSymbols))
			{
				return false;
			}
			if((XVar)(pwdUpperLower)  && (XVar)((XVar)(!(XVar)(cLower))  || (XVar)(!(XVar)(cUpper))))
			{
				return false;
			}
			return true;
		}
		public static XVar isSecureProtocol()
		{
			return (XVar)((XVar)(!(XVar)(MVCFunctions.GetServerVariable("HTTPS").IsEmpty()))  && (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.GetServerVariable("HTTPS")), XVar.Pack("off"))))  || (XVar)(MVCFunctions.GetServerPort() == 443);
		}
		public static XVar GetSiteUrl()
		{
			dynamic proto = null;
			proto = new XVar("http://");
			if((XVar)(MVCFunctions.GetServerVariable("HTTPS"))  && (XVar)(MVCFunctions.GetServerVariable("HTTPS") != "off"))
			{
				proto = new XVar("https://");
			}
			return MVCFunctions.Concat(proto, MVCFunctions.GetServerVariable("HTTP_HOST"));
		}
		public static XVar GetFullSiteUrl()
		{
			return CommonFunctions.getDirectoryFromURI((XVar)(MVCFunctions.Concat(CommonFunctions.GetSiteUrl(), MVCFunctions.GetServerVariable("REQUEST_URI"))));
		}
		public static XVar GetAuditObject(dynamic _param_table = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic auditSettings = XVar.Array();
			auditSettings = ProjectSettings.getSecurityValue(new XVar("auditAndLocking"));
			if(auditSettings["loggingMode"] == 0)
			{
				return null;
			}
			if(XVar.Pack(table))
			{
				dynamic tableSettings = XVar.Array();
				tableSettings = XVar.Clone(auditSettings["tables"][table]);
				if((XVar)(!(XVar)(tableSettings))  || (XVar)(!(XVar)(tableSettings["logModifications"])))
				{
					return null;
				}
			}
			return (XVar.Pack(auditSettings["loggingMode"] == 1) ? XVar.Pack(new AuditTrailTable()) : XVar.Pack(new AuditTrailFile()));
		}
		public static XVar GetLockingObject(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic settings = null;
			if(XVar.Pack(!(XVar)(ProjectSettings.getSecurityValue(new XVar("auditAndLocking"), new XVar("enableLocking")))))
			{
				return null;
			}
			settings = XVar.Clone(new ProjectSettings((XVar)(table)));
			if(XVar.Pack(settings.lockingEnabled()))
			{
				return new oLocking();
			}
			else
			{
				return null;
			}

			return null;
		}
		public static XVar isEnableSection508()
		{
			return ProjectSettings.getProjectValue(new XVar("section508Compat"));
		}
		public static XVar getJsValidatorName(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			switch(((XVar)name).ToString())
			{
				case "Number":
					return "IsNumeric";
					break;
				case "Password":
					return "IsPassword";
					break;
				case "Email":
					return "IsEmail";
					break;
				case "Currency":
					return "IsMoney";
					break;
				case "US ZIP Code":
					return "IsZipCode";
					break;
				case "US Phone Number":
					return "IsPhoneNumber";
					break;
				case "US State":
					return "IsState";
					break;
				case "US SSN":
					return "IsSSN";
					break;
				case "Credit Card":
					return "IsCC";
					break;
				case "Time":
					return "IsTime";
					break;
				case "Regular expression":
					return "RegExp";
					break;
				default:
					return name;
					break;
			}

			return null;
		}
		public static XVar SetLangVars(dynamic _param_xt_packed, dynamic _param_prefix, dynamic _param_pageName = null, dynamic _param_extraparams = null)
		{
			#region packeted values
			XTempl _param_xt = XVar.UnPackXTempl(_param_xt_packed);
			#endregion

			#region default values
			if(_param_pageName as Object == null) _param_pageName = new XVar("");
			if(_param_extraparams as Object == null) _param_extraparams = new XVar("");
			#endregion

			#region pass-by-value parameters
			XTempl xt = XVar.Clone(_param_xt);
			dynamic prefix = XVar.Clone(_param_prefix);
			dynamic pageName = XVar.Clone(_param_pageName);
			dynamic extraparams = XVar.Clone(_param_extraparams);
			#endregion

			dynamic currentLang = null, dataAttr = null, var_var = null;
			xt.assign(new XVar("lang_label"), new XVar(true));
			currentLang = XVar.Clone(CommonFunctions.mlang_getcurrentlang());
			var_var = XVar.Clone(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(currentLang)), "_langattrs"));
			xt.assign((XVar)(var_var), new XVar("selected"));
			xt.assign((XVar)(MVCFunctions.Concat(currentLang, "LANGLINK_ACTIVE")), new XVar(true));
			foreach (KeyValuePair<XVar, dynamic> l in ProjectSettings.languageDescriptors().GetEnumerator())
			{
				xt.assign((XVar)(MVCFunctions.Concat(l.Value["name"], "LANGLINK")), (XVar)(l.Value["name"] != currentLang));
			}
			if(XVar.Pack(CommonFunctions.isEnableSection508()))
			{
				xt.assign_section(new XVar("lang_label"), new XVar("<label for=\"languageSelector\">"), new XVar("</label>"));
			}
			if(XVar.Pack(extraparams))
			{
				extraparams = XVar.Clone(MVCFunctions.Concat(extraparams, "&"));
			}
			dataAttr = XVar.Clone(MVCFunctions.Concat("data-params=\"", extraparams, "\" data-prefix=\"", prefix, "\""));
			xt.assign(new XVar("langselector_attrs"), (XVar)(MVCFunctions.Concat("id=\"languageSelector\" ", dataAttr)));
			xt.assign(new XVar("languages_block"), new XVar(true));

			return null;
		}
		public static XVar GetTableCaption(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			return Labels.getTableCaption((XVar)(table));
		}
		public static XVar GetFieldLabel(dynamic _param_goodTable, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic goodTable = XVar.Clone(_param_goodTable);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic table = null;
			if(XVar.Equals(XVar.Pack(goodTable), XVar.Pack(Constants.GLOBAL_PAGES_GOOD)))
			{
				table = XVar.Clone(Security.loginTable());
			}
			else
			{
				table = XVar.Clone(CommonFunctions.GetTableByGood((XVar)(goodTable)));
			}
			if(XVar.Pack(!(XVar)(GlobalVars.runnerTableLabels.KeyExists(table))))
			{
				return "";
			}
			return GlobalVars.runnerTableLabels[table]["fieldLabels"][field];
		}
		public static XVar GetFieldToolTip(dynamic _param_table, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			table = XVar.Clone(CommonFunctions.GetTableByGood((XVar)(table)));
			if(XVar.Pack(!(XVar)(GlobalVars.runnerTableLabels.KeyExists(table))))
			{
				return "";
			}
			return GlobalVars.runnerTableLabels[table]["fieldTooltips"][field];
		}
		public static XVar GetFieldPlaceHolder(dynamic _param_table, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			table = XVar.Clone(CommonFunctions.GetTableByGood((XVar)(table)));
			if(XVar.Pack(!(XVar)(GlobalVars.runnerTableLabels.KeyExists(table))))
			{
				return "";
			}
			return GlobalVars.runnerTableLabels[table]["fieldPlaceholders"][field];
		}
		public static XVar GetMLString(dynamic _param_mLString)
		{
			#region pass-by-value parameters
			dynamic mLString = XVar.Clone(_param_mLString);
			#endregion

			if(XVar.Pack(!(XVar)(mLString)))
			{
				return "";
			}
			if(XVar.Pack(MVCFunctions.is_string((XVar)(mLString))))
			{
				return mLString;
			}
			switch(((XVar)mLString["type"]).ToInt())
			{
				case Constants.mlTypeText:
					return mLString["text"];
				case Constants.mlTypeCustomLabel:
					return CommonFunctions.GetCustomLabel((XVar)(mLString["label"]));
				case Constants.mlTypeMessage:
					return CommonFunctions.mlang_message((XVar)(mLString["tag"]));
				case Constants.mlTypeTableCaption:
					return Labels.getTableCaption((XVar)(mLString["table"]));
			}
			return "";
		}
		public static XVar GetCustomLabel(dynamic _param_custom)
		{
			#region pass-by-value parameters
			dynamic custom = XVar.Clone(_param_custom);
			#endregion

			return GlobalVars.runnerLangMessages[CommonFunctions.mlang_getcurrentlang()]["customLabels"][custom];
		}
		public static XVar mlang_getcurrentlang()
		{
			if(XVar.Pack(MVCFunctions.postvalue("language")))
			{
				XSession.Session["language"] = MVCFunctions.postvalue("language");
			}
			if(XVar.Pack(MVCFunctions.postvalue("language")))
			{
				XSession.Session["language"] = MVCFunctions.postvalue("language");
			}
			if(XVar.Pack(XSession.Session["language"]))
			{
				dynamic lang = null;
				lang = XVar.Clone(XSession.Session["language"]);
				foreach (KeyValuePair<XVar, dynamic> l in ProjectSettings.languageDescriptors().GetEnumerator())
				{
					if(lang == l.Value["name"])
					{
						return lang;
					}
				}
				XSession.Session["language"] = "";
			}
			return GlobalVars.mlang_defaultlang;
		}
		public static XVar isRTL()
		{
			dynamic rtlLanguages = XVar.Array();
			rtlLanguages = XVar.Clone(ProjectSettings.getProjectValue(new XVar("rtlLanguages")));
			return !(XVar)(!(XVar)(rtlLanguages[CommonFunctions.mlang_getcurrentlang()]));
		}
		public static XVar mlang_getlanglist()
		{
			return MVCFunctions.array_keys((XVar)(GlobalVars.mlang_messages));
		}
		public static XVar getMountNames()
		{
			dynamic mounts = XVar.Array();
			mounts = XVar.Clone(XVar.Array());
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_JAN")), 1);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_FEB")), 2);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_MAR")), 3);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_APR")), 4);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_MAY")), 5);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_JUN")), 6);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_JUL")), 7);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_AUG")), 8);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_SEP")), 9);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_OCT")), 10);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_NOV")), 11);
			mounts.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MONTH_DEC")), 12);
			return mounts;
		}
		public static XVar showDetailTable(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic oldTableName = null;
			oldTableName = XVar.Clone(GlobalVars.strTableName);
			GlobalVars.strTableName = XVar.Clone(var_params["table"]);
			if(XVar.Pack(var_params["dpObject"].isDispGrid()))
			{
				var_params["dpObject"].showPage();
			}
			GlobalVars.strTableName = XVar.Clone(oldTableName);

			return null;
		}
		public static XVar DoInsertRecordSQL(dynamic _param_table, ref dynamic avalues, ref dynamic blobfields, dynamic pageObject)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic blobs = null, strFields = null, strSQL = null, strValues = null;
			strSQL = XVar.Clone(MVCFunctions.Concat("insert into ", pageObject.connection.addTableWrappers((XVar)(table)), " "));
			strFields = new XVar("(");
			strValues = new XVar("(");
			blobs = XVar.Clone(MVCFunctions.PrepareBlobs(ref avalues, ref blobfields, (XVar)(pageObject)));
			foreach (KeyValuePair<XVar, dynamic> value in avalues.GetEnumerator())
			{
				strFields = MVCFunctions.Concat(strFields, pageObject.getTableField((XVar)(value.Key)), ", ");
				if(XVar.Pack(MVCFunctions.in_array((XVar)(value.Key), (XVar)(blobfields))))
				{
					strValues = MVCFunctions.Concat(strValues, value.Value, ", ");
				}
				else
				{
					if(XVar.Pack(XVar.Equals(XVar.Pack(pageObject.cipherer), XVar.Pack(null))))
					{
						strValues = MVCFunctions.Concat(strValues, CommonFunctions.add_db_quotes((XVar)(value.Key), (XVar)(value.Value)), ", ");
					}
					else
					{
						strValues = MVCFunctions.Concat(strValues, pageObject.cipherer.AddDBQuotes((XVar)(value.Key), (XVar)(value.Value)), ", ");
					}
				}
			}
			if(MVCFunctions.substr((XVar)(strFields), new XVar(-2)) == ", ")
			{
				strFields = XVar.Clone(MVCFunctions.substr((XVar)(strFields), new XVar(0), (XVar)(MVCFunctions.strlen((XVar)(strFields)) - 2)));
			}
			if(MVCFunctions.substr((XVar)(strValues), new XVar(-2)) == ", ")
			{
				strValues = XVar.Clone(MVCFunctions.substr((XVar)(strValues), new XVar(0), (XVar)(MVCFunctions.strlen((XVar)(strValues)) - 2)));
			}
			strSQL = MVCFunctions.Concat(strSQL, strFields, ") values ", strValues, ")");
			if(XVar.Pack(!(XVar)(MVCFunctions.ExecuteUpdate((XVar)(pageObject), (XVar)(strSQL), (XVar)(blobs)))))
			{
				return false;
			}
			pageObject.ProcessFiles();
			return true;
		}
		public static XVar getIntervalLimitsExpressions(dynamic _param_pSet_packed, dynamic _param_field, dynamic _param_idx, dynamic _param_isLowerBound)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic field = XVar.Clone(_param_field);
			dynamic idx = XVar.Clone(_param_idx);
			dynamic isLowerBound = XVar.Clone(_param_isLowerBound);
			#endregion

			dynamic events = null;
			events = XVar.Clone(CommonFunctions.getEventObject((XVar)(pSet)));
			return events.filterIntervalValue((XVar)(field), (XVar)(idx), (XVar)(isLowerBound));
		}
		public static XVar getEventObject(dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic table = null;
			table = XVar.Clone(pSet.table());
			if(XVar.Pack(GlobalVars.tableEvents.KeyExists(table)))
			{
				return GlobalVars.tableEvents[table];
			}
			GlobalVars.tableEvents.InitAndSetArrayItem(MVCFunctions.createEventClass((XVar)(table)), table);
			return GlobalVars.tableEvents[table];
		}
		public static XVar add_nocache_headers()
		{
			MVCFunctions.Header("Cache-Control", "no-cache, no-store, max-age=0, must-revalidate");
			MVCFunctions.Header("Pragma", "no-cache");
			MVCFunctions.Header("Expires", "Fri, 01 Jan 1990 00:00:00 GMT");

			return null;
		}
		public static XVar IsGuidString(ref dynamic str)
		{
			dynamic c = null, i = null;
			if((XVar)((XVar)(MVCFunctions.strlen((XVar)(str)) == 36)  && (XVar)(MVCFunctions.substr((XVar)(str), new XVar(0), new XVar(1)) != "{"))  && (XVar)(MVCFunctions.substr((XVar)(str), new XVar(-1)) != "}"))
			{
				str = XVar.Clone(MVCFunctions.Concat("{", str, "}"));
			}
			else
			{
				if((XVar)((XVar)(MVCFunctions.strlen((XVar)(str)) == 37)  && (XVar)(MVCFunctions.substr((XVar)(str), new XVar(0), new XVar(1)) == "{"))  && (XVar)(MVCFunctions.substr((XVar)(str), new XVar(-1)) != "}"))
				{
					str = XVar.Clone(MVCFunctions.Concat(str, "}"));
				}
				else
				{
					if((XVar)((XVar)(MVCFunctions.strlen((XVar)(str)) == 37)  && (XVar)(MVCFunctions.substr((XVar)(str), new XVar(0), new XVar(1)) != "{"))  && (XVar)(MVCFunctions.substr((XVar)(str), new XVar(-1)) == "}"))
					{
						str = XVar.Clone(MVCFunctions.Concat("{", str));
					}
				}
			}
			if(MVCFunctions.strlen((XVar)(str)) != 38)
			{
				return false;
			}
			i = new XVar(0);
			for(;i < 38; i++)
			{
				c = XVar.Clone(MVCFunctions.substr((XVar)(str), (XVar)(i), new XVar(1)));
				if(i == XVar.Pack(0))
				{
					if(c != "{")
					{
						return false;
					}
				}
				else
				{
					if(i == 37)
					{
						if(c != "}")
						{
							return false;
						}
					}
					else
					{
						if((XVar)((XVar)((XVar)(i == 9)  || (XVar)(i == 14))  || (XVar)(i == 19))  || (XVar)(i == 24))
						{
							if(c != "-")
							{
								return false;
							}
						}
						else
						{
							if((XVar)((XVar)((XVar)(c < "0")  || (XVar)("9" < c))  && (XVar)((XVar)(c < "a")  || (XVar)("f" < c)))  && (XVar)((XVar)(c < "A")  || (XVar)("F" < c)))
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}
		public static XVar IsStoredProcedure(dynamic _param_strSQL)
		{
			#region pass-by-value parameters
			dynamic strSQL = XVar.Clone(_param_strSQL);
			#endregion

			if(6 < MVCFunctions.strlen((XVar)(strSQL)))
			{
				dynamic c = null;
				c = XVar.Clone(MVCFunctions.strtolower((XVar)(MVCFunctions.substr((XVar)(strSQL), new XVar(6), new XVar(1)))));
				if((XVar)((XVar)((XVar)(MVCFunctions.strtolower((XVar)(MVCFunctions.substr((XVar)(strSQL), new XVar(0), new XVar(6)))) == "select")  && (XVar)((XVar)(c < "0")  || (XVar)("9" < c)))  && (XVar)((XVar)(c < "a")  || (XVar)("z" < c)))  && (XVar)(c != "_"))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return true;
			}

			return null;
		}
		public static XVar MobileDetected()
		{
			dynamic accept = null, user_agent = null;
			user_agent = XVar.Clone(MVCFunctions.strtolower((XVar)(MVCFunctions.GetServerVariable("HTTP_USER_AGENT"))));
			accept = XVar.Clone(MVCFunctions.strtolower((XVar)(MVCFunctions.GetServerVariable("HTTP_ACCEPT"))));
			if((XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(accept), new XVar("text/vnd.wap.wml"))), XVar.Pack(false)))  || (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(accept), new XVar("application/vnd.wap.xhtml+xml"))), XVar.Pack(false))))
			{
				return 1;
			}
			if((XVar)(MVCFunctions.SERVERKeyExists("HTTP_X_WAP_PROFILE"))  || (XVar)(MVCFunctions.SERVERKeyExists("HTTP_PROFILE")))
			{
				return 2;
			}
			if(XVar.Pack(MVCFunctions.preg_match(new XVar(MVCFunctions.Concat("/(ipad|android|symbianos|opera mini|ipod|blackberry|", "palm os|palm|hiptop|avantgo|plucker|xiino|blazer|elaine|iris|3g_t|", "windows ce|opera mobi|windows ce; smartphone;|windows ce; iemobile|", "mini 9.5|vx1000|lge |m800|e860|u940|ux840|compal|", "wireless| mobi|ahong|lg380|lgku|lgu900|lg210|lg47|lg920|lg840|", "lg370|sam-r|mg50|s55|g83|t66|vx400|mk99|d615|d763|el370|sl900|", "mp500|samu3|samu4|vx10|xda_|samu5|samu6|samu7|samu9|a615|b832|", "m881|s920|n210|s700|c-810|_h797|mob-x|sk16d|848b|mowser|s580|", "r800|471x|v120|rim8|c500foma:|160x|x160|480x|x640|t503|w839|", "i250|sprint|w398samr810|m5252|c7100|mt126|x225|s5330|s820|", "htil-g1|fly v71|s302|-x113|novarra|k610i|-three|8325rc|8352rc|", "sanyo|vx54|c888|nx250|n120|mtk |c5588|s710|t880|c5005|i;458x|", "p404i|s210|c5100|teleca|s940|c500|s590|foma|samsu|vx8|vx9|a1000|", "_mms|myx|a700|gu1100|bc831|e300|ems100|me701|me702m-three|sd588|", "s800|8325rc|ac831|mw200|brew |d88|htc\\/|htc_touch|355x|m50|km100|", "d736|p-9521|telco|sl74|ktouch|m4u\\/|me702|8325rc|kddi|phone|lg |", "sonyericsson|samsung|240x|x320vx10|nokia|sony cmd|motorola|", "up.browser|up.link|mmp|symbian|smartphone|midp|wap|vodafone|o2|", "pocket|kindle|silk|hpwos|mobile|psp|treo)/")), (XVar)(user_agent))))
			{
				return 3;
			}
			if(XVar.Pack(MVCFunctions.in_array((XVar)(MVCFunctions.substr((XVar)(user_agent), new XVar(0), new XVar(4))), (XVar)(new XVar(0, "1207", 1, "3gso", 2, "4thp", 3, "501i", 4, "502i", 5, "503i", 6, "504i", 7, "505i", 8, "506i", 9, "6310", 10, "6590", 11, "770s", 12, "802s", 13, "a wa", 14, "abac", 15, "acer", 16, "acoo", 17, "acs-", 18, "aiko", 19, "airn", 20, "alav", 21, "alca", 22, "alco", 23, "amoi", 24, "anex", 25, "anny", 26, "anyw", 27, "aptu", 28, "arch", 29, "argo", 30, "aste", 31, "asus", 32, "attw", 33, "au-m", 34, "audi", 35, "aur ", 36, "aus ", 37, "avan", 38, "beck", 39, "bell", 40, "benq", 41, "bilb", 42, "bird", 43, "blac", 44, "blaz", 45, "brew", 46, "brvw", 47, "bumb", 48, "bw-n", 49, "bw-u", 50, "c55/", 51, "capi", 52, "ccwa", 53, "cdm-", 54, "cell", 55, "chtm", 56, "cldc", 57, "cmd-", 58, "cond", 59, "craw", 60, "dait", 61, "dall", 62, "dang", 63, "dbte", 64, "dc-s", 65, "devi", 66, "dica", 67, "dmob", 68, "doco", 69, "dopo", 70, "ds-d", 71, "ds12", 72, "el49", 73, "elai", 74, "eml2", 75, "emul", 76, "eric", 77, "erk0", 78, "esl8", 79, "ez40", 80, "ez60", 81, "ez70", 82, "ezos", 83, "ezwa", 84, "ezze", 85, "fake", 86, "fetc", 87, "fly-", 88, "fly_", 89, "g-mo", 90, "g1 u", 91, "g560", 92, "gene", 93, "gf-5", 94, "go.w", 95, "good", 96, "grad", 97, "grun", 98, "haie", 99, "hcit", 100, "hd-m", 101, "hd-p", 102, "hd-t", 103, "hei-", 104, "hiba", 105, "hipt", 106, "hita", 107, "hp i", 108, "hpip", 109, "hs-c", 110, "htc ", 111, "htc-", 112, "htc_", 113, "htca", 114, "htcg", 115, "htcp", 116, "htcs", 117, "htct", 118, "http", 119, "huaw", 120, "hutc", 121, "i-20", 122, "i-go", 123, "i-ma", 124, "i230", 125, "iac", 126, "iac-", 127, "iac/", 128, "ibro", 129, "idea", 130, "ig01", 131, "ikom", 132, "im1k", 133, "inno", 134, "ipaq", 135, "iris", 136, "jata", 137, "java", 138, "jbro", 139, "jemu", 140, "jigs", 141, "kddi", 142, "keji", 143, "kgt", 144, "kgt/", 145, "klon", 146, "kpt ", 147, "kwc-", 148, "kyoc", 149, "kyok", 150, "leno", 151, "lexi", 152, "lg g", 153, "lg-a", 154, "lg-b", 155, "lg-c", 156, "lg-d", 157, "lg-f", 158, "lg-g", 159, "lg-k", 160, "lg-l", 161, "lg-m", 162, "lg-o", 163, "lg-p", 164, "lg-s", 165, "lg-t", 166, "lg-u", 167, "lg-w", 168, "lg/k", 169, "lg/l", 170, "lg/u", 171, "lg50", 172, "lg54", 173, "lge-", 174, "lge/", 175, "libw", 176, "lynx", 177, "m-cr", 178, "m1-w", 179, "m3ga", 180, "m50/", 181, "mate", 182, "maui", 183, "maxo", 184, "mc01", 185, "mc21", 186, "mcca", 187, "medi", 188, "merc", 189, "meri", 190, "midp", 191, "mio8", 192, "mioa", 193, "mits", 194, "mmef", 195, "mo01", 196, "mo02", 197, "mobi", 198, "mode", 199, "modo", 200, "mot ", 201, "mot-", 202, "moto", 203, "motv", 204, "mozz", 205, "mt50", 206, "mtp1", 207, "mtv ", 208, "mwbp", 209, "mywa", 210, "n100", 211, "n101", 212, "n102", 213, "n202", 214, "n203", 215, "n300", 216, "n302", 217, "n500", 218, "n502", 219, "n505", 220, "n700", 221, "n701", 222, "n710", 223, "nec-", 224, "nem-", 225, "neon", 226, "netf", 227, "newg", 228, "newt", 229, "nok6", 230, "noki", 231, "nzph", 232, "o2 x", 233, "o2-x", 234, "o2im", 235, "opti", 236, "opwv", 237, "oran", 238, "owg1", 239, "p800", 240, "palm", 241, "pana", 242, "pand", 243, "pant", 244, "pdxg", 245, "pg-1", 246, "pg-2", 247, "pg-3", 248, "pg-6", 249, "pg-8", 250, "pg-c", 251, "pg13", 252, "phil", 253, "pire", 254, "play", 255, "pluc", 256, "pn-2", 257, "pock", 258, "port", 259, "pose", 260, "prox", 261, "psio", 262, "pt-g", 263, "qa-a", 264, "qc-2", 265, "qc-3", 266, "qc-5", 267, "qc-7", 268, "qc07", 269, "qc12", 270, "qc21", 271, "qc32", 272, "qc60", 273, "qci-", 274, "qtek", 275, "qwap", 276, "r380", 277, "r600", 278, "raks", 279, "rim9", 280, "rove", 281, "rozo", 282, "s55/", 283, "sage", 284, "sama", 285, "samm", 286, "sams", 287, "sany", 288, "sava", 289, "sc01", 290, "sch-", 291, "scoo", 292, "scp-", 293, "sdk/", 294, "se47", 295, "sec-", 296, "sec0", 297, "sec1", 298, "semc", 299, "send", 300, "seri", 301, "sgh-", 302, "shar", 303, "sie-", 304, "siem", 305, "sk-0", 306, "sl45", 307, "slid", 308, "smal", 309, "smar", 310, "smb3", 311, "smit", 312, "smt5", 313, "soft", 314, "sony", 315, "sp01", 316, "sph-", 317, "spv ", 318, "spv-", 319, "sy01", 320, "symb", 321, "t-mo", 322, "t218", 323, "t250", 324, "t600", 325, "t610", 326, "t618", 327, "tagt", 328, "talk", 329, "tcl-", 330, "tdg-", 331, "teli", 332, "telm", 333, "tim-", 334, "topl", 335, "tosh", 336, "treo", 337, "ts70", 338, "tsm-", 339, "tsm3", 340, "tsm5", 341, "tx-9", 342, "up.b", 343, "upg1", 344, "upsi", 345, "utst", 346, "v400", 347, "v750", 348, "veri", 349, "virg", 350, "vite", 351, "vk-v", 352, "vk40", 353, "vk50", 354, "vk52", 355, "vk53", 356, "vm40", 357, "voda", 358, "vulc", 359, "vx52", 360, "vx53", 361, "vx60", 362, "vx61", 363, "vx70", 364, "vx80", 365, "vx81", 366, "vx83", 367, "vx85", 368, "vx98", 369, "w3c ", 370, "w3c-", 371, "wap-", 372, "wapa", 373, "wapi", 374, "wapj", 375, "wapm", 376, "wapp", 377, "wapr", 378, "waps", 379, "wapt", 380, "wapu", 381, "wapv", 382, "wapy", 383, "webc", 384, "whit", 385, "wig ", 386, "winc", 387, "winw", 388, "wmlb", 389, "wonu", 390, "x700", 391, "xda-", 392, "xda2", 393, "xdag", 394, "yas-", 395, "your", 396, "zeto", 397, "zte-")))))
			{
				return 4;
			}
			return false;
		}
		public static XVar isIE8()
		{
			dynamic matches = XVar.Array();
			matches = new XVar("");
			MVCFunctions.preg_match(new XVar("/MSIE (.*?);/"), (XVar)(MVCFunctions.GetServerVariable("HTTP_USER_AGENT")), (XVar)(matches));
			return (XVar)(1 < MVCFunctions.count(matches))  && (XVar)(matches[1] <= 8);
		}
		public static dynamic GetPageLayout(dynamic _param_table, dynamic _param_page, dynamic _param_suffixName = null)
		{
			#region default values
			if(_param_suffixName as Object == null) _param_suffixName = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic page = XVar.Clone(_param_page);
			dynamic suffixName = XVar.Clone(_param_suffixName);
			#endregion

			dynamic layout = null, pdLayoutName = null, shortTableName = null, styleOverrides = null;
			shortTableName = XVar.Clone(CommonFunctions.GetTableURL((XVar)(table)));
			shortTableName = XVar.Clone((XVar.Pack(shortTableName == XVar.Pack("")) ? XVar.Pack(Constants.GLOBAL_PAGES_SHORT) : XVar.Pack(shortTableName)));
			pdLayoutName = XVar.Clone(MVCFunctions.Concat(shortTableName, "_", page));
			if(XVar.Pack(GlobalVars.all_page_layouts.KeyExists(pdLayoutName)))
			{
				return GlobalVars.all_page_layouts[pdLayoutName];
			}
			MVCFunctions.importPageOptions((XVar)(table), (XVar)(page));
			if((XVar)(!(XVar)(GlobalVars.pd_pages.KeyExists(table)))  || (XVar)(!(XVar)(GlobalVars.pd_pages[table].KeyExists(page))))
			{
				return null;
			}
			styleOverrides = ProjectSettings.getProjectValue(new XVar("styleOverrides"));
			layout = XVar.Clone(new PDLayout((XVar)(shortTableName), (XVar)(GlobalVars.pd_pages[table][page]), (XVar)(ProjectSettings.getProjectValue(new XVar("projectTheme"))), (XVar)(ProjectSettings.getProjectValue(new XVar("projectSize"))), (XVar)(pdLayoutName), (XVar)(styleOverrides.KeyExists(pdLayoutName))));
			GlobalVars.all_page_layouts.InitAndSetArrayItem(layout, MVCFunctions.Concat(shortTableName, "_", page));
			return layout;
		}
		public static XVar getLayoutByFilename(dynamic _param_filename)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			#endregion

			dynamic oldFileName = null;
			if(XVar.Pack(GlobalVars.all_page_layouts.KeyExists(filename)))
			{
				return GlobalVars.all_page_layouts[filename];
			}
			oldFileName = XVar.Clone(filename);
			if(MVCFunctions.substr((XVar)(filename), new XVar(0), new XVar(8)) == MVCFunctions.Concat(Constants.GLOBAL_PAGES_SHORT, "_"))
			{
				oldFileName = XVar.Clone(MVCFunctions.substr((XVar)(filename), new XVar(8)));
			}
			return GlobalVars.page_layouts[oldFileName];
		}
		public static XVar extractStyle(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic pos = null, pos1 = null, quot = null;
			pos = XVar.Clone(CommonFunctions.my_stripos((XVar)(str), new XVar("style=\""), new XVar(0)));
			quot = new XVar("\"");
			if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
			{
				pos = XVar.Clone(CommonFunctions.my_stripos((XVar)(str), new XVar("style='"), new XVar(0)));
				quot = new XVar("'");
			}
			if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
			{
				return null;
			}
			pos1 = XVar.Clone(MVCFunctions.strpos((XVar)(str), (XVar)(quot), (XVar)(pos + 7)));
			if(XVar.Equals(XVar.Pack(pos1), XVar.Pack(false)))
			{
				return "";
			}
			return MVCFunctions.substr((XVar)(str), (XVar)(pos + 7), (XVar)((pos1 - pos) - 7));
		}
		public static XVar injectStyle(dynamic _param_str, dynamic _param_style)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			dynamic style = XVar.Clone(_param_style);
			#endregion

			dynamic pos = null, quot = null;
			pos = XVar.Clone(CommonFunctions.my_stripos((XVar)(str), new XVar("style=\""), new XVar(0)));
			quot = new XVar("\"");
			if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
			{
				pos = XVar.Clone(CommonFunctions.my_stripos((XVar)(str), new XVar("style='"), new XVar(0)));
				quot = new XVar("'");
			}
			if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
			{
				return MVCFunctions.Concat(str, " style=\"", style, "\"");
			}
			return MVCFunctions.Concat(MVCFunctions.substr((XVar)(str), new XVar(0), (XVar)(pos + 7)), style, ";", MVCFunctions.substr((XVar)(str), (XVar)(pos + 7)));
		}
		public static XVar generateUserCode(dynamic _param_length)
		{
			#region pass-by-value parameters
			dynamic length = XVar.Clone(_param_length);
			#endregion

			dynamic code = null, i = null;
			code = new XVar("");
			i = new XVar(0);
			for(;i < length; i++)
			{
				code = MVCFunctions.Concat(code, MVCFunctions.rand(new XVar(0), new XVar(9)));
			}
			return code;
		}
		public static XVar generatePassword(dynamic _param_length)
		{
			#region pass-by-value parameters
			dynamic length = XVar.Clone(_param_length);
			#endregion

			dynamic i = null, j = null, password = null;
			password = new XVar("");
			i = new XVar(0);
			for(;i < length; i++)
			{
				j = XVar.Clone(MVCFunctions.rand(new XVar(0), new XVar(35)));
				if(j < 26)
				{
					password = MVCFunctions.Concat(password, MVCFunctions.chr((XVar)(MVCFunctions.ord(new XVar('a')) + j)));
				}
				else
				{
					password = MVCFunctions.Concat(password, MVCFunctions.chr((XVar)((MVCFunctions.ord(new XVar('0')) - 26) + j)));
				}
			}
			return password;
		}
		public static XVar generateHex(dynamic _param_length)
		{
			#region pass-by-value parameters
			dynamic length = XVar.Clone(_param_length);
			#endregion

			dynamic i = null, j = null, password = null;
			password = new XVar("");
			i = new XVar(0);
			for(;i < length; i++)
			{
				j = XVar.Clone(MVCFunctions.rand(new XVar(0), new XVar(15)));
				if(j < 10)
				{
					password = MVCFunctions.Concat(password, MVCFunctions.chr((XVar)(MVCFunctions.ord(new XVar('0')) + j)));
				}
				else
				{
					password = MVCFunctions.Concat(password, MVCFunctions.chr((XVar)((MVCFunctions.ord(new XVar('a')) - 10) + j)));
				}
			}
			return password;
		}
		public static XVar generateAlias()
		{
			return MVCFunctions.Concat("a", CommonFunctions.generatePassword(new XVar(9)));
		}
		public static XVar securityCheckFileName(dynamic _param_fileName)
		{
			#region pass-by-value parameters
			dynamic fileName = XVar.Clone(_param_fileName);
			#endregion

			dynamic i = null, maliciousStrings = XVar.Array();
			maliciousStrings = XVar.Clone(new XVar(0, "../", 1, "..\\"));
			i = new XVar(0);
			for(;i < MVCFunctions.count(maliciousStrings); i++)
			{
				while(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(fileName), (XVar)(maliciousStrings[i]))), XVar.Pack(false)))
				{
					fileName = XVar.Clone(MVCFunctions.str_replace((XVar)(maliciousStrings), new XVar(""), (XVar)(fileName)));
				}
			}
			return fileName;
		}
		public static XVar getOptionsForMultiUpload(dynamic _param_pSet_packed, dynamic _param_field)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic options = XVar.Array(), uploadDir = null;
			if(XVar.Pack(pSet.isAbsolute((XVar)(field))))
			{
				uploadDir = XVar.Clone(pSet.getUploadFolder((XVar)(field)));
			}
			else
			{
				uploadDir = XVar.Clone(MVCFunctions.getabspath((XVar)(pSet.getUploadFolder((XVar)(field)))));
			}
			options = XVar.Clone(new XVar("max_file_size", pSet.getMaxFileSize((XVar)(field)), "max_totalFile_size", pSet.getMaxTotalFilesSize((XVar)(field)), "max_number_of_files", pSet.getMaxNumberOfFiles((XVar)(field))));
			if(XVar.Pack(pSet.getResizeOnUpload((XVar)(field))))
			{
				options.InitAndSetArrayItem(true, "resizeOnUpload");
				options.InitAndSetArrayItem(pSet.getNewImageSize((XVar)(field)), "max_width");
				options.InitAndSetArrayItem(options["max_width"], "max_height");
			}
			if(XVar.Pack(pSet.getCreateThumbnail((XVar)(field))))
			{
				options.InitAndSetArrayItem(new XVar("thumbnail", new XVar("max_width", pSet.getThumbnailSize((XVar)(field)), "max_height", pSet.getThumbnailSize((XVar)(field)), "thumbnailPrefix", pSet.getStrThumbnail((XVar)(field)))), "image_versions");
			}
			return options;
		}
		public static XVar mimeTypeByExt(dynamic _param_ext)
		{
			#region pass-by-value parameters
			dynamic ext = XVar.Clone(_param_ext);
			#endregion

			dynamic mime = XVar.Array();
			mime = CommonFunctions.getMimeTypes();
			ext = XVar.Clone(MVCFunctions.strtolower((XVar)(ext)));
			if(XVar.Pack(mime[ext]))
			{
				return mime[ext];
			}
			return "application/octet-stream";
		}
		public static XVar getMimeTypes()
		{
			dynamic mime = XVar.Array();
			if(XVar.Pack(GlobalVars.onDemnadVariables["mimeTypes"]))
			{
				return GlobalVars.onDemnadVariables["mimeTypes"];
			}
			mime = XVar.Clone(XVar.Array());
			mime.InitAndSetArrayItem("audio/aac", "aac");
			mime.InitAndSetArrayItem("application/x-abiword", "abw");
			mime.InitAndSetArrayItem("application/x-freearc", "arc");
			mime.InitAndSetArrayItem("video/x-msvideo", "avi");
			mime.InitAndSetArrayItem("application/vnd.amazon.ebook", "azw");
			mime.InitAndSetArrayItem("application/octet-stream", "bin");
			mime.InitAndSetArrayItem("image/bmp", "bmp");
			mime.InitAndSetArrayItem("application/x-bzip", "bz");
			mime.InitAndSetArrayItem("application/x-bzip2", "bz2");
			mime.InitAndSetArrayItem("application/x-cdf", "cda");
			mime.InitAndSetArrayItem("application/x-csh", "csh");
			mime.InitAndSetArrayItem("text/css", "css");
			mime.InitAndSetArrayItem("text/csv", "csv");
			mime.InitAndSetArrayItem("application/msword", "doc");
			mime.InitAndSetArrayItem("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx");
			mime.InitAndSetArrayItem("application/vnd.ms-fontobject", "eot");
			mime.InitAndSetArrayItem("application/epub+zip", "epub");
			mime.InitAndSetArrayItem("application/gzip", "gz");
			mime.InitAndSetArrayItem("image/gif", "gif");
			mime.InitAndSetArrayItem("text/html", "htm");
			mime.InitAndSetArrayItem("text/html", "html");
			mime.InitAndSetArrayItem("image/vnd.microsoft.icon", "ico");
			mime.InitAndSetArrayItem("text/calendar", "ics");
			mime.InitAndSetArrayItem("application/java-archive", "jar");
			mime.InitAndSetArrayItem("image/jpeg", "jpeg");
			mime.InitAndSetArrayItem("image/jpeg", "jpg");
			mime.InitAndSetArrayItem("text/javascript", "js");
			mime.InitAndSetArrayItem("application/json", "json");
			mime.InitAndSetArrayItem("application/ld+json", "jsonld");
			mime.InitAndSetArrayItem("audio/midi audio/x-midi", "mid");
			mime.InitAndSetArrayItem("audio/midi audio/x-midi", "midi");
			mime.InitAndSetArrayItem("text/javascript", "mjs");
			mime.InitAndSetArrayItem("audio/mpeg", "mp3");
			mime.InitAndSetArrayItem("video/mp4", "mp4");
			mime.InitAndSetArrayItem("video/mp4", "m4v");
			mime.InitAndSetArrayItem("video/mpeg", "mpeg");
			mime.InitAndSetArrayItem("video/mpeg", "mpg");
			mime.InitAndSetArrayItem("video/x-matroska", "mkv");
			mime.InitAndSetArrayItem("application/vnd.apple.installer+xml", "mpkg");
			mime.InitAndSetArrayItem("application/vnd.oasis.opendocument.presentation", "odp");
			mime.InitAndSetArrayItem("application/vnd.oasis.opendocument.spreadsheet", "ods");
			mime.InitAndSetArrayItem("application/vnd.oasis.opendocument.text", "odt");
			mime.InitAndSetArrayItem("audio/ogg", "oga");
			mime.InitAndSetArrayItem("video/ogg", "ogv");
			mime.InitAndSetArrayItem("application/ogg", "ogx");
			mime.InitAndSetArrayItem("audio/opus", "opus");
			mime.InitAndSetArrayItem("font/otf", "otf");
			mime.InitAndSetArrayItem("image/png", "png");
			mime.InitAndSetArrayItem("application/pdf", "pdf");
			mime.InitAndSetArrayItem("application/x-httpd-php", "php");
			mime.InitAndSetArrayItem("application/vnd.ms-powerpoint", "ppt");
			mime.InitAndSetArrayItem("application/vnd.openxmlformats-officedocument.presentationml.presentation", "pptx");
			mime.InitAndSetArrayItem("application/vnd.rar", "rar");
			mime.InitAndSetArrayItem("application/rtf", "rtf");
			mime.InitAndSetArrayItem("application/x-sh", "sh");
			mime.InitAndSetArrayItem("image/svg+xml", "svg");
			mime.InitAndSetArrayItem("application/x-shockwave-flash", "swf");
			mime.InitAndSetArrayItem("application/x-tar", "tar");
			mime.InitAndSetArrayItem("image/tiff", "tif");
			mime.InitAndSetArrayItem("image/tiff", "tiff");
			mime.InitAndSetArrayItem("video/mp2t", "ts");
			mime.InitAndSetArrayItem("font/ttf", "ttf");
			mime.InitAndSetArrayItem("text/plain", "txt");
			mime.InitAndSetArrayItem("application/vnd.visio", "vsd");
			mime.InitAndSetArrayItem("audio/wav", "wav");
			mime.InitAndSetArrayItem("audio/webm", "wmv");
			mime.InitAndSetArrayItem("audio/webm", "weba");
			mime.InitAndSetArrayItem("video/webm", "webm");
			mime.InitAndSetArrayItem("image/webp", "webp");
			mime.InitAndSetArrayItem("font/woff", "woff");
			mime.InitAndSetArrayItem("font/woff2", "woff2");
			mime.InitAndSetArrayItem("application/xhtml+xml", "xhtml");
			mime.InitAndSetArrayItem("application/vnd.ms-excel", "xls");
			mime.InitAndSetArrayItem("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx");
			mime.InitAndSetArrayItem("application/xml", "xml");
			mime.InitAndSetArrayItem("application/vnd.mozilla.xul+xml", "xul");
			mime.InitAndSetArrayItem("application/zip", "zip");
			mime.InitAndSetArrayItem("video/3gpp", "3gp");
			mime.InitAndSetArrayItem("video/3gpp2", "3g2");
			mime.InitAndSetArrayItem("application/x-7z-compressed", "7z");
			mime.InitAndSetArrayItem("video/x-ms-asf", "asf");
			mime.InitAndSetArrayItem("audio/wav", "wav");
			mime.InitAndSetArrayItem("text/asp", "asp");
			mime.InitAndSetArrayItem("video/flv", "flv");
			GlobalVars.onDemnadVariables.InitAndSetArrayItem(mime, "mimeTypes");
			return mime;
		}
		public static XVar getContentTypeByExtension(dynamic _param_ext)
		{
			#region pass-by-value parameters
			dynamic ext = XVar.Clone(_param_ext);
			#endregion

			dynamic ctype = null;
			ext = XVar.Clone(MVCFunctions.strtolower((XVar)(ext)));
			if(MVCFunctions.substr((XVar)(ext), new XVar(0), new XVar(1)) != ".")
			{
				ext = XVar.Clone(MVCFunctions.Concat(".", ext));
			}
			if(ext == ".asf")
			{
				ctype = new XVar("video/x-ms-asf");
			}
			else
			{
				if(ext == ".avi")
				{
					ctype = new XVar("video/avi");
				}
				else
				{
					if(ext == ".doc")
					{
						ctype = new XVar("application/msword");
					}
					else
					{
						if(ext == ".zip")
						{
							ctype = new XVar("application/zip");
						}
						else
						{
							if(ext == ".xls")
							{
								ctype = new XVar("application/vnd.ms-excel");
							}
							else
							{
								if(ext == ".png")
								{
									ctype = new XVar("image/png");
								}
								else
								{
									if(ext == ".gif")
									{
										ctype = new XVar("image/gif");
									}
									else
									{
										if((XVar)(ext == ".jpg")  || (XVar)(ext == "jpeg"))
										{
											ctype = new XVar("image/jpeg");
										}
										else
										{
											if(ext == ".webp")
											{
												ctype = new XVar("image/webp");
											}
											else
											{
												if(ext == ".wav")
												{
													ctype = new XVar("audio/wav");
												}
												else
												{
													if(ext == ".mp3")
													{
														ctype = new XVar("audio/mpeg");
													}
													else
													{
														if((XVar)(ext == ".mpg")  || (XVar)(ext == "mpeg"))
														{
															ctype = new XVar("video/mpeg");
														}
														else
														{
															if(ext == ".rtf")
															{
																ctype = new XVar("application/rtf");
															}
															else
															{
																if((XVar)(ext == ".htm")  || (XVar)(ext == "html"))
																{
																	ctype = new XVar("text/html");
																}
																else
																{
																	if(ext == ".asp")
																	{
																		ctype = new XVar("text/asp");
																	}
																	else
																	{
																		if(ext == ".flv")
																		{
																			ctype = new XVar("video/flv");
																		}
																		else
																		{
																			if((XVar)(ext == ".mp4")  || (XVar)(ext == ".m4v"))
																			{
																				ctype = new XVar("video/mp4");
																			}
																			else
																			{
																				if(ext == ".webm")
																				{
																					ctype = new XVar("video/webm");
																				}
																				else
																				{
																					if(ext == ".wmv")
																					{
																						ctype = new XVar("video/webm");
																					}
																					else
																					{
																						if(ext == ".pdf")
																						{
																							ctype = new XVar("application/pdf");
																						}
																						else
																						{
																							ctype = new XVar("application/octet-stream");
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return ctype;
		}
		public static XVar getLatLngByAddr(dynamic _param_addr)
		{
			#region pass-by-value parameters
			dynamic addr = XVar.Clone(_param_addr);
			#endregion

			dynamic apiKey = null, data = XVar.Array(), lat = null, lng = null, result = XVar.Array(), ret = XVar.Array(), url = null, var_request = null;
			apiKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("mapSettings"), new XVar("apikey")));
			switch(((XVar)CommonFunctions.getMapProvider()).ToInt())
			{
				case Constants.GOOGLE_MAPS:
					url = XVar.Clone(MVCFunctions.Concat("https://maps.googleapis.com/maps/api/geocode/json?address=", MVCFunctions.RawUrlEncode((XVar)(addr)), "&sensor=false&key=", apiKey));
					result = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.myurl_get_contents((XVar)(url)))));
					if(result["status"] == "OK")
					{
						return result["results"][0]["geometry"]["location"];
					}
					break;
				case Constants.OPEN_STREET_MAPS:
					url = XVar.Clone(MVCFunctions.Concat("https://nominatim.openstreetmap.org/search/", MVCFunctions.RawUrlEncode((XVar)(addr)), "?format=json&addressdetails=1&limit=1"));
					result = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.myurl_get_contents((XVar)(url)))));
					if(XVar.Pack(result))
					{
						lat = XVar.Clone(result[0]["lat"]);
						if(XVar.Pack(!(XVar)(lat)))
						{
							lat = new XVar(0);
						}
						lng = XVar.Clone(result[0]["lon"]);
						if(XVar.Pack(!(XVar)(lng)))
						{
							lng = new XVar(0);
						}
						return new XVar("lat", lat, "lng", lng);
					}
					break;
				case Constants.BING_MAPS:
					if((XVar)(!(XVar)(apiKey))  || (XVar)(!(XVar)(addr)))
					{
						return false;
					}
					url = XVar.Clone(MVCFunctions.Concat("https://dev.virtualearth.net/REST/v1/Locations?query=", MVCFunctions.RawUrlEncode((XVar)(addr)), "&output=json&key=", apiKey));
					result = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.myurl_get_contents((XVar)(url)))));
					if(XVar.Pack(result))
					{
						lat = XVar.Clone(result["resourceSets"][0]["resources"][0]["geocodePoints"][0]["coordinates"][0]);
						if(XVar.Pack(!(XVar)(lat)))
						{
							lat = new XVar(0);
						}
						lng = XVar.Clone(result["resourceSets"][0]["resources"][0]["geocodePoints"][0]["coordinates"][1]);
						if(XVar.Pack(!(XVar)(lng)))
						{
							lng = new XVar(0);
						}
						return new XVar("lat", lat, "lng", lng);
					}
					break;
				case Constants.HERE_MAPS:
					var_request = XVar.Clone(new HttpRequest((XVar)(MVCFunctions.Concat("https://geocode.search.hereapi.com/v1/geocode?apiKey=", apiKey, "&q=", MVCFunctions.RawUrlEncode((XVar)(addr)))), new XVar("GET")));
					ret = XVar.Clone(var_request.run());
					if((XVar)(!(XVar)(ret["erorr"]))  && (XVar)(ret["content"]))
					{
						data = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(ret["content"])));
						return new XVar("lat", data["items"][0]["position"]["lat"], "lng", data["items"][0]["position"]["lng"]);
					}
					break;
				case Constants.MAPQUEST_MAPS:
					var_request = XVar.Clone(new HttpRequest((XVar)(MVCFunctions.Concat("http://www.mapquestapi.com/geocoding/v1/address?key=", apiKey, "&location=", MVCFunctions.RawUrlEncode((XVar)(addr)))), new XVar("GET")));
					ret = XVar.Clone(var_request.run());
					if((XVar)(!(XVar)(ret["erorr"]))  && (XVar)(ret["content"]))
					{
						data = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(ret["content"])));
						return new XVar("lat", data["results"][0]["locations"][0]["latLng"]["lat"], "lng", data["results"][0]["locations"][0]["latLng"]["lng"]);
					}
					break;
			}
			return false;
		}
		public static XVar isLoggedAsGuest()
		{
			return Security.isGuest();
		}
		public static XVar GetFieldType(dynamic _param_field, dynamic _param_table = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			if((XVar)(table != XVar.Pack(""))  || (XVar)(!(XVar)(GlobalVars.pageObject as object != null)))
			{
				dynamic newSet = null;
				if(table == XVar.Pack(""))
				{
					table = XVar.Clone(GlobalVars.strTableName);
				}
				newSet = XVar.Clone(new ProjectSettings((XVar)(table)));
				return newSet.getFieldType((XVar)(field));
			}
			else
			{
				return GlobalVars.pageObject.pSet.getFieldType((XVar)(field));
			}

			return null;
		}
		public static XVar Label(dynamic _param_field, dynamic _param_table = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic result = null;
			if((XVar)(table != XVar.Pack(""))  || (XVar)(!(XVar)(GlobalVars.pageObject as object != null)))
			{
				dynamic newSet = null;
				if(table == XVar.Pack(""))
				{
					table = XVar.Clone(GlobalVars.strTableName);
				}
				newSet = XVar.Clone(new ProjectSettings((XVar)(table)));
				result = XVar.Clone(newSet.label((XVar)(field)));
			}
			else
			{
				result = XVar.Clone(GlobalVars.pageObject.pSet.label((XVar)(field)));
			}
			return (XVar.Pack(result != XVar.Pack("")) ? XVar.Pack(result) : XVar.Pack(field));
		}
		public static XVar getIconByFileType(dynamic _param_fileType, dynamic _param_sourceFileName)
		{
			#region pass-by-value parameters
			dynamic fileType = XVar.Clone(_param_fileType);
			dynamic sourceFileName = XVar.Clone(_param_sourceFileName);
			#endregion

			dynamic dotPosition = null, fileName = null;
			switch(((XVar)fileType).ToString())
			{
				case "text/html":
					fileName = new XVar("html.png");
					break;
				case "text/asp":
					fileName = new XVar("code.png");
					break;
				case "application/msword":
				case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
					fileName = new XVar("doc.png");
					break;
				case "application/vnd.ms-excel":
				case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
					fileName = new XVar("xls.png");
					break;
				case "application/rtf":
					fileName = new XVar("rtf.png");
					break;
				case "image/png":
				case "image/x-png":
					fileName = new XVar("png.png");
					break;
				case "image/gif":
					fileName = new XVar("gif.png");
					break;
				case "image/jpeg":
				case "image/pjpeg":
					fileName = new XVar("jpg.png");
					break;
				case "audio/wav":
					fileName = new XVar("wma.png");
					break;
				case "audio/mp3":
				case "audio/mpeg3":
				case "audio/mpeg":
					fileName = new XVar("mp2.png");
					break;
				case "video/mpeg":
					fileName = new XVar("mpeg.png");
					break;
				case "video/flv":
					fileName = new XVar("flv.png");
					break;
				case "video/mp4":
					fileName = new XVar("mp4.png");
					break;
				case "video/x-ms-asf":
					fileName = new XVar("asf.png");
					break;
				case "video/webm":
				case "video/x-webm":
				case "video/avi":
					fileName = new XVar("mpg.png");
					break;
				case "application/zip":
				case "application/x-zip-compressed":
					fileName = new XVar("zip.png");
					break;
				default:
					fileName = new XVar("text.png");
					sourceFileName = XVar.Clone(MVCFunctions.strtolower((XVar)(sourceFileName)));
					dotPosition = XVar.Clone(MVCFunctions.strrpos((XVar)(sourceFileName), new XVar(".")));
					if((XVar)(!XVar.Equals(XVar.Pack(dotPosition), XVar.Pack(false)))  && (XVar)(dotPosition < MVCFunctions.strlen((XVar)(sourceFileName)) - 1))
					{
						dynamic ext = null, icons = XVar.Array();
						ext = XVar.Clone(MVCFunctions.substr((XVar)(sourceFileName), (XVar)(dotPosition + 1)));
						icons = XVar.Clone(XVar.Array());
						icons.InitAndSetArrayItem("7z", "7z");
						icons.InitAndSetArrayItem("asf", "asf");
						icons.InitAndSetArrayItem("code", "asp");
						icons.InitAndSetArrayItem("mpg", "avi");
						icons.InitAndSetArrayItem("chm", "chm");
						icons.InitAndSetArrayItem("doc", "doc");
						icons.InitAndSetArrayItem("doc", "docx");
						icons.InitAndSetArrayItem("flv", "flv");
						icons.InitAndSetArrayItem("gz", "gz");
						icons.InitAndSetArrayItem("html", "html");
						icons.InitAndSetArrayItem("mdb", "mdb");
						icons.InitAndSetArrayItem("mdb", "mdbx");
						icons.InitAndSetArrayItem("mp2", "mp3");
						icons.InitAndSetArrayItem("mp4", "mp4");
						icons.InitAndSetArrayItem("mpeg", "mpeg");
						icons.InitAndSetArrayItem("mpg", "mpg");
						icons.InitAndSetArrayItem("mov", "mov");
						icons.InitAndSetArrayItem("pdf", "pdf");
						icons.InitAndSetArrayItem("code", "php");
						icons.InitAndSetArrayItem("pps", "pps");
						icons.InitAndSetArrayItem("powerpoint", "ppt");
						icons.InitAndSetArrayItem("psd", "psd");
						icons.InitAndSetArrayItem("rar", "rar");
						icons.InitAndSetArrayItem("rtf", "rtf");
						icons.InitAndSetArrayItem("swf", "swf");
						icons.InitAndSetArrayItem("tif", "tif");
						icons.InitAndSetArrayItem("ttf", "ttf");
						icons.InitAndSetArrayItem("txt", "txt");
						icons.InitAndSetArrayItem("wav", "wav");
						icons.InitAndSetArrayItem("mpg", "webm");
						icons.InitAndSetArrayItem("wma", "wma");
						icons.InitAndSetArrayItem("emv", "wmv");
						icons.InitAndSetArrayItem("xls", "xls");
						icons.InitAndSetArrayItem("xls", "xlsx");
						icons.InitAndSetArrayItem("zip", "zip");
						if(XVar.Pack(icons.KeyExists(ext)))
						{
							fileName = XVar.Clone(MVCFunctions.Concat(icons[ext], ".png"));
						}
					}
					break;
			}
			return fileName;
		}
		public static XVar isImageType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			switch(((XVar)var_type).ToString())
			{
				case "image/png":
				case "image/x-png":
				case "image/gif":
				case "image/jpeg":
				case "image/pjpeg":
				case "image/webp":
					return true;
			}
			return false;
		}
		public static XVar initArray(dynamic array, dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			if(XVar.Pack(!(XVar)(array.KeyExists(key))))
			{
				array.InitAndSetArrayItem(XVar.Array(), key);
			}

			return null;
		}
		public static XVar GetKeysArray(dynamic _param_arr, dynamic _param_pageObject, dynamic _param_searchId = null)
		{
			#region default values
			if(_param_searchId as Object == null) _param_searchId = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic arr = XVar.Clone(_param_arr);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic searchId = XVar.Clone(_param_searchId);
			#endregion

			dynamic aKeys = XVar.Array(), keyfields = XVar.Array();
			keyfields = XVar.Clone(pageObject.pSet.getTableKeys());
			aKeys = XVar.Clone(XVar.Array());
			if(XVar.Pack(MVCFunctions.count(keyfields)))
			{
				foreach (KeyValuePair<XVar, dynamic> kfield in keyfields.GetEnumerator())
				{
					if(XVar.Pack(arr.KeyExists(kfield.Value)))
					{
						aKeys.InitAndSetArrayItem(arr[kfield.Value], kfield.Value);
					}
				}
				if((XVar)(MVCFunctions.count(aKeys) == 0)  && (XVar)(searchId))
				{
					dynamic lastId = null;
					lastId = XVar.Clone(pageObject.connection.getInsertedId());
					if(XVar.Pack(0) < lastId)
					{
						aKeys.InitAndSetArrayItem(lastId, keyfields[0]);
					}
				}
			}
			return aKeys;
		}
		public static XVar GetBaseScriptsForPage(dynamic _param_isDisplayLoading, dynamic _param_additionalScripts = null, dynamic _param_customText = null)
		{
			#region default values
			if(_param_additionalScripts as Object == null) _param_additionalScripts = new XVar("");
			if(_param_customText as Object == null) _param_customText = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic isDisplayLoading = XVar.Clone(_param_isDisplayLoading);
			dynamic additionalScripts = XVar.Clone(_param_additionalScripts);
			dynamic customText = XVar.Clone(_param_customText);
			#endregion

			dynamic projectBuildKey = null, result = null, wizardBuildKey = null;
			projectBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("projectBuild")));
			wizardBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("wizardBuild")));
			result = new XVar("");
			result = MVCFunctions.Concat(result, "<script type=\"text/javascript\" src=\"include/loadfirst.js?", wizardBuildKey, "\"></script>");
			if(XVar.Pack(ProjectSettings.getProjectValue(new XVar("hasCustomFunctionsJs"))))
			{
				result = MVCFunctions.Concat(result, "<script type=\"text/javascript\" src=\"usercode/custom_functions.js?", projectBuildKey, "\"></script>");
			}
			result = MVCFunctions.Concat(result, additionalScripts);
			result = MVCFunctions.Concat(result, "<script type=\"text/javascript\" src=\"include/lang/", CommonFunctions.getLangFileName((XVar)(CommonFunctions.mlang_getcurrentlang())), ".js?", wizardBuildKey, "\"></script>");
			if((XVar)(ProjectSettings.richTextEnabled())  && (XVar)(ProjectSettings.rteType() == Constants.rteInnovaEditor))
			{
				result = MVCFunctions.Concat(result, "<script type=\"text/javascript\" src=\"plugins/innovaeditor/scripts/innovaeditor.js\"></script>");
			}
			if(CommonFunctions.getMapProvider() == Constants.BING_MAPS)
			{
				result = MVCFunctions.Concat(result, "<script type=\"text/javascript\" src=\"https://www.bing.com/api/maps/mapcontrol?&setMkt=", CommonFunctions.getBingMapsLang(), "\"></script>");
			}
			if(CommonFunctions.getMapProvider() == Constants.HERE_MAPS)
			{
				result = MVCFunctions.Concat(result, "<link rel=\"stylesheet\" type=\"text/css\" href=\"https://js.api.here.com/v3/3.1/mapsjs-ui.css\" />");
				result = MVCFunctions.Concat(result, "<script src=\"https://js.api.here.com/v3/3.1/mapsjs-core.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
				result = MVCFunctions.Concat(result, "<script src=\"https://js.api.here.com/v3/3.1/mapsjs-core-legacy.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
				result = MVCFunctions.Concat(result, "<script src=\"https://js.api.here.com/v3/3.1/mapsjs-service.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
				result = MVCFunctions.Concat(result, "<script src=\"https://js.api.here.com/v3/3.1/mapsjs-service-legacy.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
				result = MVCFunctions.Concat(result, "<script src=\"https://js.api.here.com/v3/3.1/mapsjs-mapevents.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
				result = MVCFunctions.Concat(result, "<script type=\"text/javascript\" src=\"https://js.api.here.com/v3/3.1/mapsjs-ui.js\"></script>");
				result = MVCFunctions.Concat(result, "<script src=\"https://js.api.here.com/v3/3.1/mapsjs-clustering.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
				result = MVCFunctions.Concat(result, "<script src=\"https://js.api.here.com/v3/3.1/mapsjs-data.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
			}
			if(CommonFunctions.getMapProvider() == Constants.MAPQUEST_MAPS)
			{
				result = MVCFunctions.Concat(result, "<link type=\"text/css\" rel=\"stylesheet\" href=\"https://api.mqcdn.com/sdk/mapquest-js/v1.3.2/mapquest.css\"/>");
				result = MVCFunctions.Concat(result, "<script src=\"https://api.mqcdn.com/sdk/mapquest-js/v1.3.2/mapquest.js\"></script>");
				result = MVCFunctions.Concat(result, "<script src=\"https://unpkg.com/leaflet.markercluster@1.0.6/dist/leaflet.markercluster.js\"></script>");
				result = MVCFunctions.Concat(result, "<link type=\"text/css\" rel=\"stylesheet\" href=\"https://unpkg.com/leaflet.markercluster@1.0.6/dist/MarkerCluster.css\"/>");
				result = MVCFunctions.Concat(result, "<link type=\"text/css\" rel=\"stylesheet\" href=\"https://unpkg.com/leaflet.markercluster@1.0.6/dist/MarkerCluster.Default.css\"/>");
				result = MVCFunctions.Concat(result, "<script src=\"https://leaflet.github.io/Leaflet.heat/dist/leaflet-heat.js\"></script>");
			}
			if(XVar.Pack(isDisplayLoading))
			{
				result = MVCFunctions.Concat(result, "<script type=\"text/javascript\">Runner.runLoading('", customText, "');</script>");
			}
			return result;
		}
		public static XVar printJSON(dynamic _param_data, dynamic _param_returnPlainJSON = null)
		{
			#region default values
			if(_param_returnPlainJSON as Object == null) _param_returnPlainJSON = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic returnPlainJSON = XVar.Clone(_param_returnPlainJSON);
			#endregion

			dynamic rJSON = null;
			rJSON = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(data)));
			return (XVar.Pack(returnPlainJSON) ? XVar.Pack(rJSON) : XVar.Pack(MVCFunctions.runner_htmlspecialchars((XVar)(rJSON))));
		}
		public static XVar PrepareForExcel(dynamic _param_ret)
		{
			#region pass-by-value parameters
			dynamic ret = XVar.Clone(_param_ret);
			#endregion

			if(MVCFunctions.substr((XVar)(ret), new XVar(0), new XVar(1)) == "=")
			{
				ret = XVar.Clone(MVCFunctions.Concat("&#61;", MVCFunctions.substr((XVar)(ret), new XVar(1))));
			}
			return ret;
		}
		public static XVar countTotals(dynamic totals, dynamic _param_totalsFields, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic totalsFields = XVar.Clone(_param_totalsFields);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic i = null;
			if(XVar.Pack(!(XVar)(totalsFields)))
			{
				return null;
			}
			i = new XVar(0);
			for(;i < MVCFunctions.count(totalsFields); i++)
			{
				if(totalsFields[i]["totalsType"] == "COUNT")
				{
					if(data[totalsFields[i]["fName"]] != "")
					{
						totals[totalsFields[i]["fName"]]["value"]++;
					}
				}
				else
				{
					if(totalsFields[i]["viewFormat"] == "Time")
					{
						dynamic time = XVar.Array();
						time = XVar.Clone(CommonFunctions.GetTotalsForTime((XVar)(data[totalsFields[i]["fName"]])));
						totals[totalsFields[i]["fName"]]["value"] += (time[2] + time[1] * 60) + time[0] * 3600;
					}
					else
					{
						totals[totalsFields[i]["fName"]]["value"] += data[totalsFields[i]["fName"]] + 0;
					}
				}
				if(totalsFields[i]["totalsType"] == "AVERAGE")
				{
					if((XVar)(!(XVar)(XVar.Equals(XVar.Pack(data[totalsFields[i]["fName"]]), XVar.Pack(null))))  && (XVar)(!XVar.Equals(XVar.Pack(data[totalsFields[i]["fName"]]), XVar.Pack(""))))
					{
						totals[totalsFields[i]["fName"]]["numRows"]++;
					}
				}
			}

			return null;
		}
		public static XVar XMLNameEncode(dynamic _param_strValue)
		{
			#region pass-by-value parameters
			dynamic strValue = XVar.Clone(_param_strValue);
			#endregion

			dynamic ret = null, search = null;
			search = XVar.Clone(new XVar(0, " ", 1, "#", 2, "'", 3, "/", 4, "\\", 5, "(", 6, ")", 7, ",", 8, "["));
			ret = XVar.Clone(MVCFunctions.str_replace((XVar)(search), new XVar(""), (XVar)(strValue)));
			search = XVar.Clone(new XVar(0, "]", 1, "+", 2, "\"", 3, "-", 4, "_", 5, "|", 6, "}", 7, "{", 8, "="));
			ret = XVar.Clone(MVCFunctions.str_replace((XVar)(search), new XVar(""), (XVar)(ret)));
			return ret;
		}
		public static XVar getFileExtension(dynamic _param_fileName)
		{
			#region pass-by-value parameters
			dynamic fileName = XVar.Clone(_param_fileName);
			#endregion

			dynamic pos = null;
			pos = XVar.Clone(MVCFunctions.strrpos((XVar)(fileName), new XVar(".")));
			if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
			{
				return "";
			}
			return MVCFunctions.substr((XVar)(fileName), (XVar)(pos + 1));
		}
		public static XVar getFileWoExtension(dynamic _param_fileName)
		{
			#region pass-by-value parameters
			dynamic fileName = XVar.Clone(_param_fileName);
			#endregion

			dynamic pos = null;
			pos = XVar.Clone(MVCFunctions.strrpos((XVar)(fileName), new XVar(".")));
			if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
			{
				return fileName;
			}
			return MVCFunctions.substr((XVar)(fileName), new XVar(0), (XVar)(pos));
		}
		public static dynamic getDefaultConnection()
		{
			return GlobalVars.cman.getDefault();
		}
		public static XVar isIOS()
		{
			return (XVar)((XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.stripos((XVar)(MVCFunctions.GetServerVariable("HTTP_USER_AGENT")), new XVar("iPod"))), XVar.Pack(false)))  || (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.stripos((XVar)(MVCFunctions.GetServerVariable("HTTP_USER_AGENT")), new XVar("iPad"))), XVar.Pack(false))))  || (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.stripos((XVar)(MVCFunctions.GetServerVariable("HTTP_USER_AGENT")), new XVar("iPhone"))), XVar.Pack(false)));
		}
		public static XVar getMapProvider()
		{
			return ProjectSettings.getProjectValue(new XVar("mapSettings"), new XVar("provider"));
		}
		public static XVar getBingMapsLang()
		{
			dynamic arrBimgMapLang = XVar.Array();
			arrBimgMapLang = XVar.Clone(XVar.Array());
			arrBimgMapLang.InitAndSetArrayItem("cs-CZ", "Czech");
			arrBimgMapLang.InitAndSetArrayItem("da-DK", "Danish");
			arrBimgMapLang.InitAndSetArrayItem("nl-NL", "Dutch");
			arrBimgMapLang.InitAndSetArrayItem("en-US", "English");
			arrBimgMapLang.InitAndSetArrayItem("fr-FR", "French");
			arrBimgMapLang.InitAndSetArrayItem("de-DE", "German");
			arrBimgMapLang.InitAndSetArrayItem("it-IT", "Italian");
			arrBimgMapLang.InitAndSetArrayItem("ja-JP", "Japanese");
			arrBimgMapLang.InitAndSetArrayItem("nb-NO", "Norwegian");
			arrBimgMapLang.InitAndSetArrayItem("pl-PL", "Polish");
			arrBimgMapLang.InitAndSetArrayItem("pt-PT", "Portugal");
			arrBimgMapLang.InitAndSetArrayItem("pt-BR", "Portuguese");
			arrBimgMapLang.InitAndSetArrayItem("ru-RU", "Russian");
			arrBimgMapLang.InitAndSetArrayItem("es-ES", "Spanish");
			arrBimgMapLang.InitAndSetArrayItem("sw-SE", "Swedish");
			arrBimgMapLang.InitAndSetArrayItem("zh-TW", "Chinese");
			arrBimgMapLang.InitAndSetArrayItem("zh-HK", "Hongkong");
			if(XVar.Pack(arrBimgMapLang.KeyExists(CommonFunctions.mlang_getcurrentlang())))
			{
				return arrBimgMapLang[CommonFunctions.mlang_getcurrentlang()];
			}
			return arrBimgMapLang["English"];
		}
		public static XVar getDefaultLanguage()
		{
			dynamic acceptHeader = null, langCodes = XVar.Array(), projectLanguages = XVar.Array();
			if(XVar.Pack(!(XVar)(ProjectSettings.getProjectValue(new XVar("detectDefaultLanguage")))))
			{
				return ProjectSettings.getProjectValue(new XVar("defaultLanguage"));
			}
			acceptHeader = XVar.Clone(MVCFunctions.GetServerVariable("HTTP_ACCEPT_LANGUAGE"));
			if(XVar.Pack(!(XVar)(acceptHeader)))
			{
				return ProjectSettings.getProjectValue(new XVar("defaultLanguage"));
			}
			if(XVar.Pack(CommonFunctions.mlang_getcurrentlang()))
			{
				return ProjectSettings.getProjectValue(new XVar("defaultLanguage"));
			}
			projectLanguages = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> l in ProjectSettings.languageDescriptors().GetEnumerator())
			{
				projectLanguages.InitAndSetArrayItem(true, l.Value["name"]);
			}
			langCodes = XVar.Clone(XVar.Array());
			langCodes.InitAndSetArrayItem("Afrikaans", "af");
			langCodes.InitAndSetArrayItem("Arabic", "ar");
			langCodes.InitAndSetArrayItem("Bosnian", "bs");
			langCodes.InitAndSetArrayItem("Bulgarian", "bg");
			langCodes.InitAndSetArrayItem("Catalan", "ca");
			langCodes.InitAndSetArrayItem("Chinese", "zh");
			langCodes.InitAndSetArrayItem("Croatian", "hr");
			langCodes.InitAndSetArrayItem("Czech", "cs");
			langCodes.InitAndSetArrayItem("Danish", "da");
			langCodes.InitAndSetArrayItem("Dutch", "nl");
			langCodes.InitAndSetArrayItem("English", "en");
			langCodes.InitAndSetArrayItem("Farsi", "fa");
			langCodes.InitAndSetArrayItem("French", "fr");
			langCodes.InitAndSetArrayItem("Georgian", "ka");
			langCodes.InitAndSetArrayItem("German", "de");
			langCodes.InitAndSetArrayItem("Greek", "el");
			langCodes.InitAndSetArrayItem("Hebrew", "he");
			langCodes.InitAndSetArrayItem("Hongkong", "hk");
			langCodes.InitAndSetArrayItem("Hungarian", "hu");
			langCodes.InitAndSetArrayItem("Indonesian", "id");
			langCodes.InitAndSetArrayItem("Italian", "it");
			langCodes.InitAndSetArrayItem("Japanese", "ja");
			langCodes.InitAndSetArrayItem("Malay", "ms");
			langCodes.InitAndSetArrayItem("Norwegian", "no");
			langCodes.InitAndSetArrayItem("Phillipines", "fl");
			langCodes.InitAndSetArrayItem("Polish", "pl");
			langCodes.InitAndSetArrayItem("Portugal", "pt");
			langCodes.InitAndSetArrayItem("Portuguese", "br");
			langCodes.InitAndSetArrayItem("Romanian", "ro");
			langCodes.InitAndSetArrayItem("Russian", "ru");
			langCodes.InitAndSetArrayItem("Slovak", "sk");
			langCodes.InitAndSetArrayItem("Spanish", "es");
			langCodes.InitAndSetArrayItem("Swedish", "sv");
			langCodes.InitAndSetArrayItem("Taiwan", "tw");
			langCodes.InitAndSetArrayItem("Thai", "th");
			langCodes.InitAndSetArrayItem("Turkish", "tr");
			langCodes.InitAndSetArrayItem("Urdu", "ur");
			langCodes.InitAndSetArrayItem("Welsh", "cy");
			acceptHeader = XVar.Clone(MVCFunctions.strtolower((XVar)(acceptHeader)));
			acceptHeader = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, ";", 1, "-")), new XVar(","), (XVar)(acceptHeader)));
			foreach (KeyValuePair<XVar, dynamic> code in MVCFunctions.explode(new XVar(","), (XVar)(acceptHeader)).GetEnumerator())
			{
				dynamic language = null;
				language = XVar.Clone(langCodes[code.Value]);
				if((XVar)(language)  && (XVar)(projectLanguages[language]))
				{
					return language;
				}
			}
			return ProjectSettings.getProjectValue(new XVar("defaultLanguage"));
		}
		public static XVar xt_showpdchart(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			var_params.InitAndSetArrayItem(var_params["custom1"], "chartName");
			var_params.InitAndSetArrayItem(true, "pdMode");
			var_params.InitAndSetArrayItem(CommonFunctions.GetTableByShort((XVar)(var_params["chartName"])), "table");
			CommonFunctions.xt_showchart((XVar)(var_params));

			return null;
		}
		public static XVar xt_showchart(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic chartParams = XVar.Array();
			chartParams = XVar.Clone(CommonFunctions.getChartParams((XVar)(var_params)));
			MVCFunctions.Echo(MVCFunctions.Concat("<div class=\"bs-chart\" id=\"", chartParams["containerId"], "\"></div>"));
			if((XVar)(true)  || (XVar)(!(XVar)(var_params["singlePage"])))
			{
				MVCFunctions.Echo(MVCFunctions.Concat("<div data-runner-chart-params=\"", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.runner_json_encode((XVar)(chartParams)))), "\"></div>"));
			}

			return null;
		}
		public static XVar getFileUrl(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(var_params["custom1"]);

			return null;
		}
		public static XVar getPdfImageObject(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic height = null, imagePath = null, width = null;
			imagePath = XVar.Clone(var_params["custom1"]);
			width = XVar.Clone(var_params["custom2"]);
			height = XVar.Clone(var_params["custom3"]);
			MVCFunctions.Echo(MVCFunctions.Concat("{\r\n\t\timage: \"", CommonFunctions.jsreplace((XVar)(var_params["custom1"])), "\",\r\n\t\twidth: ", (XVar.Pack(width) ? XVar.Pack(width) : XVar.Pack("null")), ",\r\n\t\theight: ", (XVar.Pack(height) ? XVar.Pack(height) : XVar.Pack("null")), "\r\n\t}"));

			return null;
		}
		public static XVar setHomePage(dynamic _param_url)
		{
			#region pass-by-value parameters
			dynamic url = XVar.Clone(_param_url);
			#endregion

			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(2, "landingSettings", "type");
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(url, "url");

			return null;
		}
		public static XVar getHomePage()
		{
			dynamic page = null, pageId = null, table = null, var_type = null;
			var_type = XVar.Clone(ProjectSettings.getProjectValue(new XVar("landingSettings"), new XVar("type")));
			if(var_type == 2)
			{
				return ProjectSettings.getProjectValue(new XVar("landingSettings"), new XVar("url"));
			}
			if(var_type == XVar.Pack(0))
			{
				return MVCFunctions.GetTableLink(new XVar("menu"));
			}
			page = XVar.Clone(ProjectSettings.getProjectValue(new XVar("landingSettings"), new XVar("page")));
			pageId = XVar.Clone(ProjectSettings.getProjectValue(new XVar("landingSettings"), new XVar("pageId")));
			table = XVar.Clone(ProjectSettings.getProjectValue(new XVar("landingSettings"), new XVar("table")));
			if((XVar)((XVar)(page == XVar.Pack(""))  || (XVar)(page == "login"))  || (XVar)(page == "register"))
			{
				return MVCFunctions.GetTableLink(new XVar("menu"));
			}
			if(table != XVar.Pack(""))
			{
				if(pageId != XVar.Pack(""))
				{
					return MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(table))), (XVar)(page), (XVar)(MVCFunctions.Concat("page=", pageId)));
				}
				return MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(table))), (XVar)(page));
			}
			return MVCFunctions.GetTableLink((XVar)(page));
		}
		public static XVar printHomeLink(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(MVCFunctions.runner_htmlspecialchars((XVar)(CommonFunctions.getHomePage())));

			return null;
		}
		public static XVar setProjectLogo(dynamic _param_html, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic html = XVar.Clone(_param_html);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			return Labels.setProjectLogo((XVar)(html), (XVar)(lng));
		}
		public static XVar getProjectLogo(dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			return Labels.getProjectLogo((XVar)(lng));
		}
		public static XVar printProjectLogo(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(Labels.getProjectLogo());

			return null;
		}
		public static XVar xt_jspagetitlelabel(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(CommonFunctions.jsreplace((XVar)(CommonFunctions._pagetitlelabel((XVar)(var_params)))));

			return null;
		}
		public static XVar xt_pagetitlelabel(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(CommonFunctions._pagetitlelabel((XVar)(var_params)));

			return null;
		}
		public static XVar _pagetitlelabel(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic pageObject = null, record = null, settings = null;
			pageObject = XVar.Clone(var_params["pageObj"]);
			record = XVar.Clone((XVar.Pack(var_params.KeyExists("record")) ? XVar.Pack(var_params["record"]) : XVar.Pack(null)));
			settings = XVar.Clone((XVar.Pack(var_params.KeyExists("settings")) ? XVar.Pack(var_params["settings"]) : XVar.Pack(null)));
			if(XVar.Pack(var_params.KeyExists("custom2")))
			{
				return pageObject.getPageTitle((XVar)(var_params["custom2"]), (XVar)(CommonFunctions.GetTableByGood((XVar)(var_params["custom1"]))), (XVar)(record), (XVar)(settings));
			}
			else
			{
				return pageObject.getPageTitle((XVar)(var_params["custom1"]), new XVar(""), (XVar)(record), (XVar)(settings));
			}

			return null;
		}
		public static XVar xt_label(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(CommonFunctions.GetFieldLabel((XVar)(var_params["custom1"]), (XVar)(var_params["custom2"])));

			return null;
		}
		public static XVar xt_jslabel(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(CommonFunctions.jsreplace((XVar)(CommonFunctions.GetFieldLabel((XVar)(var_params["custom1"]), (XVar)(var_params["custom2"])))));

			return null;
		}
		public static XVar xt_tooltip(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(CommonFunctions.GetFieldToolTip((XVar)(var_params["custom1"]), (XVar)(var_params["custom2"])));

			return null;
		}
		public static XVar xt_tooltipAttr(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(CommonFunctions.GetFieldToolTip((XVar)(var_params["custom1"]), (XVar)(var_params["custom2"])))))))
			{
				MVCFunctions.Echo("data-hidden");
			}

			return null;
		}
		public static XVar xt_custom(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(CommonFunctions.GetCustomLabel((XVar)(var_params["custom1"])));

			return null;
		}
		public static XVar xt_htmlcustom(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(MVCFunctions.runner_htmlspecialchars((XVar)(CommonFunctions.GetCustomLabel((XVar)(var_params["custom1"])))));

			return null;
		}
		public static XVar xt_cl_length(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(MVCFunctions.strlen((XVar)(MVCFunctions.trim((XVar)(CommonFunctions.GetCustomLabel((XVar)(var_params["custom1"])))))));

			return null;
		}
		public static XVar xt_caption(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic table = null;
			table = XVar.Clone(CommonFunctions.GetTableByGood((XVar)(var_params["custom1"])));
			MVCFunctions.Echo(Labels.getTableCaption((XVar)(table)));

			return null;
		}
		public static XVar xt_jscaption(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic table = null;
			table = XVar.Clone(CommonFunctions.GetTableByGood((XVar)(var_params["custom1"])));
			MVCFunctions.Echo(CommonFunctions.jsreplace((XVar)(Labels.getTableCaption((XVar)(table)))));

			return null;
		}
		public static XVar xt_buildforwardcontrol(dynamic var_params)
		{
			dynamic clearVar = null;
			XTempl xt;
			clearVar = XVar.Clone(var_params["clearVar"]);
			xt = XVar.UnPackXTempl(var_params["xt"]);
			xt.assign((XVar)(clearVar), new XVar(false));
			if(XVar.Pack(!(XVar)(var_params["additionalCtrlParams"])))
			{
				var_params.InitAndSetArrayItem(XVar.Array(), "additionalCtrlParams");
			}
			var_params.InitAndSetArrayItem(true, "additionalCtrlParams", "inLabel");
			var_params.InitAndSetArrayItem("", "clearVar");
			return CommonFunctions.xt_buildeditcontrol((XVar)(var_params));
		}
		public static XVar xt_forwardViewControl(dynamic var_params)
		{
			dynamic clearVar = null;
			XTempl xt;
			clearVar = XVar.Clone(var_params["clearVar"]);
			xt = XVar.UnPackXTempl(var_params["xt"]);
			xt.assign((XVar)(clearVar), new XVar(false));
			MVCFunctions.Echo(var_params["value"]);

			return null;
		}
		public static XVar xt_buildviewcontrol(dynamic var_params)
		{
			dynamic clearVar = null;
			XTempl xt;
			clearVar = XVar.Clone(var_params["clearVar"]);
			xt = XVar.UnPackXTempl(var_params["xt"]);
			if(XVar.Pack(clearVar))
			{
				xt.assign((XVar)(clearVar), new XVar(false));
			}
			MVCFunctions.Echo(var_params["value"]);

			return null;
		}
		public static XVar xt_buildeditcontrol(dynamic var_params)
		{
			dynamic additionalCtrlParams = null, clearVar = null, data = null, extraParams = null, field = null, fieldNum = null, id = null, mode = null, pageObj = null, validate = null;
			XTempl xt;
			clearVar = XVar.Clone(var_params["clearVar"]);
			xt = XVar.UnPackXTempl(var_params["xt"]);
			if(XVar.Pack(clearVar))
			{
				xt.assign((XVar)(clearVar), new XVar(false));
			}
			pageObj = XVar.Clone(var_params["pageObj"]);
			data = XVar.Clone(pageObj.getFieldControlsData());
			field = XVar.Clone(var_params["field"]);
			if(var_params["mode"] == "edit")
			{
				mode = new XVar(Constants.MODE_EDIT);
			}
			else
			{
				if(var_params["mode"] == "add")
				{
					mode = new XVar(Constants.MODE_ADD);
				}
				else
				{
					if(var_params["mode"] == "inline_edit")
					{
						mode = new XVar(Constants.MODE_INLINE_EDIT);
					}
					else
					{
						if(var_params["mode"] == "inline_add")
						{
							mode = new XVar(Constants.MODE_INLINE_ADD);
						}
						else
						{
							mode = new XVar(Constants.MODE_SEARCH);
						}
					}
				}
			}
			fieldNum = new XVar(0);
			if(XVar.Pack(var_params["fieldNum"]))
			{
				fieldNum = XVar.Clone(var_params["fieldNum"]);
			}
			id = new XVar("");
			if(!XVar.Equals(XVar.Pack(var_params["id"]), XVar.Pack("")))
			{
				id = XVar.Clone(var_params["id"]);
			}
			validate = XVar.Clone(XVar.Array());
			if(XVar.Pack(var_params["validate"]))
			{
				validate = XVar.Clone(var_params["validate"]);
			}
			additionalCtrlParams = XVar.Clone(XVar.Array());
			if(XVar.Pack(var_params["additionalCtrlParams"]))
			{
				additionalCtrlParams = XVar.Clone(var_params["additionalCtrlParams"]);
			}
			extraParams = XVar.Clone(XVar.Array());
			if(XVar.Pack(var_params["extraParams"]))
			{
				extraParams = XVar.Clone(var_params["extraParams"]);
			}
			pageObj.getControl((XVar)(field), (XVar)(id), (XVar)(extraParams)).buildControl((XVar)(var_params["value"]), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));

			return null;
		}
		public static XVar getArrayElementNC(dynamic arr, dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			dynamic akey = null;
			akey = XVar.Clone(CommonFunctions.getArrayKeyNC((XVar)(arr), (XVar)(key)));
			if(XVar.Equals(XVar.Pack(akey), XVar.Pack(null)))
			{
				return null;
			}
			return arr[akey];
		}
		public static XVar getArrayKeyNC(dynamic _param_arr, dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic arr = XVar.Clone(_param_arr);
			dynamic key = XVar.Clone(_param_key);
			#endregion

			dynamic keys = XVar.Array(), uKey = null;
			if(XVar.Pack(!(XVar)(arr)))
			{
				return null;
			}
			if(XVar.Pack(arr.KeyExists(key)))
			{
				return key;
			}
			keys = XVar.Clone(MVCFunctions.array_keys((XVar)(arr)));
			uKey = XVar.Clone(MVCFunctions.strtoupper((XVar)(key)));
			foreach (KeyValuePair<XVar, dynamic> k in keys.GetEnumerator())
			{
				if(MVCFunctions.strtoupper((XVar)(k.Value)) == uKey)
				{
					return k.Value;
				}
			}
			return null;
		}
		public static XVar getSessionElementNC(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			dynamic ukey = null;
			if(XVar.Pack(XSession.Session.KeyExists(key)))
			{
				return XSession.Session[key];
			}
			ukey = XVar.Clone(MVCFunctions.strtoupper((XVar)(key)));
			foreach (KeyValuePair<XVar, dynamic> v in XSession.Session.GetEnumerator())
			{
				if(MVCFunctions.strtoupper((XVar)(v.Key)) == ukey)
				{
					return v.Value;
				}
			}
			return null;
		}
		public static XVar prepareLookupWhere(dynamic _param_field, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic where = null;
			where = XVar.Clone(pSet.getLookupWhere((XVar)(field)));
			if(XVar.Pack(pSet.isLookupWhereCode((XVar)(field))))
			{
				return where;
			}
			return DB.PrepareSQL((XVar)(where));
		}
		public static XVar verifyRecaptchaResponse(dynamic _param_response)
		{
			#region pass-by-value parameters
			dynamic var_response = XVar.Clone(_param_response);
			#endregion

			dynamic answers = XVar.Array(), data = XVar.Array(), errors = XVar.Array(), message = null, req = XVar.Array(), success = null, verifyUrl = null;
			verifyUrl = new XVar("https://www.recaptcha.net/recaptcha/api/siteverify?");
			errors = XVar.Clone(XVar.Array());
			errors.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SEC_INVALID_CAPTCHA_CODE")), "missing-input-response");
			errors.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SEC_INVALID_CAPTCHA_CODE")), "invalid-input-response");
			errors.InitAndSetArrayItem("The secret parameter is missing", "missing-input-secret");
			errors.InitAndSetArrayItem("The secret parameter is invalid or malformed", "invalid-input-secret");
			errors.InitAndSetArrayItem("The request is invalid or malformed", "bad-request");
			data = XVar.Clone(XVar.Array());
			data.InitAndSetArrayItem(ProjectSettings.captchaValue(new XVar("secretKey")), "secret");
			data.InitAndSetArrayItem(var_response, "response");
			data.InitAndSetArrayItem(MVCFunctions.remoteAddr(), "remoteIp");
			req = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in data.GetEnumerator())
			{
				req.InitAndSetArrayItem(MVCFunctions.Concat(value.Key, "=", MVCFunctions.RawUrlEncode((XVar)(value.Value))), null);
			}
			var_response = XVar.Clone(MVCFunctions.myurl_get_contents((XVar)(MVCFunctions.Concat(verifyUrl, MVCFunctions.implode(new XVar("&"), (XVar)(req))))));
			answers = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(var_response)));
			message = new XVar("");
			if(var_response == XVar.Pack(""))
			{
				success = new XVar(false);
				message = new XVar("Unable to contact reCaptcha server");
			}
			else
			{
				if(XVar.Pack(!(XVar)(answers.KeyExists("success"))))
				{
					success = new XVar(false);
					message = XVar.Clone(MVCFunctions.Concat("Unable to contact reCaptcha server<br>", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.substr((XVar)(var_response), new XVar(0), new XVar(100))))));
				}
				else
				{
					dynamic code = null, i = null;
					success = XVar.Clone(answers["success"]);
					i = new XVar(0);
					for(;(XVar)(answers["error-codes"])  && (XVar)(i < MVCFunctions.count(answers["error-codes"])); ++(i))
					{
						code = XVar.Clone(answers["error-codes"][i]);
						if(XVar.Pack(errors.KeyExists(code)))
						{
							answers.InitAndSetArrayItem(errors[code], "error-codes", i);
						}
						else
						{
							answers.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(code)), "error-codes", i);
						}
						message = XVar.Clone(MVCFunctions.implode(new XVar("<br>"), (XVar)(answers["error-codes"])));
					}
				}
			}
			return new XVar("success", success, "message", message);
		}
		public static XVar toSet(dynamic _param_arr)
		{
			#region pass-by-value parameters
			dynamic arr = XVar.Clone(_param_arr);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in arr.GetEnumerator())
			{
				ret.InitAndSetArrayItem(true, v.Value);
			}
			return ret;
		}
		public static XVar tablePages(dynamic _param_tablename)
		{
			#region pass-by-value parameters
			dynamic tablename = XVar.Clone(_param_tablename);
			#endregion

			MVCFunctions._loadTablePages();
			return GlobalVars.runnerPageInfo["allPages"][tablename];
		}
		public static XVar allTablePages()
		{
			MVCFunctions._loadTablePages();
			return GlobalVars.runnerPageInfo["allPages"];
		}
		public static XVar getMediaType()
		{
			return GlobalVars.mediaType;
		}
		public static XVar getListOfSuggests(dynamic _param_sfields, dynamic _param_table, dynamic _param_numberOfSuggests, dynamic _param_searchOpt, dynamic _param_searchFor, dynamic _param_searchField = null, dynamic _param_detailKeys = null, dynamic _param_conditions = null)
		{
			#region default values
			if(_param_searchField as Object == null) _param_searchField = new XVar("");
			if(_param_detailKeys as Object == null) _param_detailKeys = new XVar(XVar.Array());
			if(_param_conditions as Object == null) _param_conditions = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic sfields = XVar.Clone(_param_sfields);
			dynamic table = XVar.Clone(_param_table);
			dynamic numberOfSuggests = XVar.Clone(_param_numberOfSuggests);
			dynamic searchOpt = XVar.Clone(_param_searchOpt);
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic searchField = XVar.Clone(_param_searchField);
			dynamic detailKeys = XVar.Clone(_param_detailKeys);
			dynamic conditions = XVar.Clone(_param_conditions);
			#endregion

			dynamic cipherer = null, controls = null, dataSource = null, result = XVar.Array(), var_response = XVar.Array();
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table), new XVar(Constants.PAGE_SEARCH)));
			dataSource = XVar.Clone(CommonFunctions.getDataSource((XVar)(table), (XVar)(pSet)));
			cipherer = XVar.Clone(new RunnerCipherer((XVar)(table)));
			controls = XVar.Clone(new EditControlsContainer(new XVar(null), (XVar)(pSet), new XVar(Constants.PAGE_LIST), (XVar)(cipherer)));
			var_response = XVar.Clone(XVar.Array());
			result = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in sfields.GetEnumerator())
			{
				dynamic allConditions = XVar.Array(), dc = null, fType = null, fieldControl = null, qResult = null, row = XVar.Array(), val = null;
				fType = XVar.Clone(pSet.getFieldType((XVar)(f.Value)));
				if((XVar)((XVar)((XVar)(!(XVar)(CommonFunctions.IsCharType((XVar)(fType))))  && (XVar)(!(XVar)(CommonFunctions.IsNumberType((XVar)(fType)))))  && (XVar)(!(XVar)(CommonFunctions.IsGuid((XVar)(fType)))))  || (XVar)(MVCFunctions.in_array((XVar)(f.Value), (XVar)(detailKeys))))
				{
					continue;
				}
				if((XVar)(searchField != XVar.Pack(""))  && (XVar)(searchField != MVCFunctions.GoodFieldName((XVar)(f.Value))))
				{
					continue;
				}
				fieldControl = XVar.Clone(controls.getControl((XVar)(f.Value), new XVar(1)));
				dc = XVar.Clone(fieldControl.getSuggestCommand((XVar)(searchFor), (XVar)(searchOpt), (XVar)(numberOfSuggests)));
				allConditions = XVar.Clone(conditions);
				allConditions.InitAndSetArrayItem(dc.filter, null);
				dc.filter = XVar.Clone(DataCondition._And((XVar)(allConditions)));
				qResult = XVar.Clone(dataSource.getTotals((XVar)(dc)));
				while((XVar)(row = XVar.Clone(qResult.fetchNumeric()))  && (XVar)(MVCFunctions.count(var_response) < numberOfSuggests))
				{
					val = XVar.Clone(cipherer.DecryptField((XVar)(f.Value), (XVar)(row[0])));
					if(XVar.Pack(CommonFunctions.IsGuid((XVar)(fType))))
					{
						val = XVar.Clone(MVCFunctions.substr((XVar)(val), new XVar(1), new XVar(-1)));
					}
					fieldControl.suggestValue((XVar)(MVCFunctions.Concat("_", val)), (XVar)(searchFor), (XVar)(var_response), (XVar)(row));
				}
			}
			MVCFunctions.ksort(ref var_response, new XVar(Constants.SORT_STRING));
			foreach (KeyValuePair<XVar, dynamic> realValue in var_response.GetEnumerator())
			{
				dynamic pos = null, strRealValue = null, strValue = null;
				if(numberOfSuggests < MVCFunctions.count(result))
				{
					break;
				}
				strValue = XVar.Clone((XVar.Pack(realValue.Key[0] == "_") ? XVar.Pack(MVCFunctions.substr((XVar)(realValue.Key), new XVar(1))) : XVar.Pack(realValue.Key)));
				strRealValue = XVar.Clone((XVar.Pack(realValue.Value[0] == "_") ? XVar.Pack(MVCFunctions.substr((XVar)(realValue.Value), new XVar(1))) : XVar.Pack(realValue.Value)));
				pos = XVar.Clone(CommonFunctions.my_stripos((XVar)(strValue), (XVar)(searchFor), new XVar(0)));
				if(XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
				{
					result.InitAndSetArrayItem(new XVar("value", MVCFunctions.runner_htmlspecialchars((XVar)(strValue)), "realValue", strRealValue), null);
				}
				else
				{
					dynamic highlightedValue = null;
					highlightedValue = XVar.Clone(MVCFunctions.Concat(MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.substr((XVar)(strValue), new XVar(0), (XVar)(pos)))), "<b>", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.substr((XVar)(strValue), (XVar)(pos), (XVar)(MVCFunctions.strlen((XVar)(searchFor)))))), "</b>", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.substr((XVar)(strValue), (XVar)(pos + MVCFunctions.strlen((XVar)(searchFor))))))));
					result.InitAndSetArrayItem(new XVar("value", highlightedValue, "realValue", strRealValue), null);
				}
			}
			return result;
		}
		public static XVar IsEmptyRequest(dynamic _param_allowedKeys = null)
		{
			#region default values
			if(_param_allowedKeys as Object == null) _param_allowedKeys = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic allowedKeys = XVar.Clone(_param_allowedKeys);
			#endregion

			dynamic hasExtraKeys = null, mkeyLength = null;
			if(0 != MVCFunctions.POSTSize())
			{
				return false;
			}
			allowedKeys.InitAndSetArrayItem(true, "menuItemId");
			allowedKeys.InitAndSetArrayItem(true, "page");
			allowedKeys.InitAndSetArrayItem(true, "mastertable");
			hasExtraKeys = new XVar(false);
			mkeyLength = new XVar(9);
			foreach (KeyValuePair<XVar, dynamic> value in MVCFunctions.EnumerateGET())
			{
				if(XVar.Pack(allowedKeys[value.Key]))
				{
					continue;
				}
				if((XVar)(MVCFunctions.substr((XVar)(value.Key), new XVar(0), (XVar)(mkeyLength)) == "masterkey")  && (XVar)(0 < (int)MVCFunctions.substr((XVar)(value.Key), (XVar)(mkeyLength))))
				{
					continue;
				}
				hasExtraKeys = new XVar(true);
				break;
			}
			if(XVar.Pack(hasExtraKeys))
			{
				return false;
			}
			return true;
		}
		public static XVar IsSetKeyInRequest(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			return (XVar)(MVCFunctions.POSTKeyExists(key))  || (XVar)(MVCFunctions.GETKeyExists(key));
		}
		public static XVar addToAssocArray(dynamic _param_assocArray, dynamic _param_arr)
		{
			#region pass-by-value parameters
			dynamic assocArray = XVar.Clone(_param_assocArray);
			dynamic arr = XVar.Clone(_param_arr);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> e in arr.GetEnumerator())
			{
				assocArray.InitAndSetArrayItem(true, e.Value);
			}
			return assocArray;
		}
		public static XVar postvalue_number(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			return (int)MVCFunctions.postvalue((XVar)(key));
		}
		public static XVar basicViewFormat(dynamic _param_format)
		{
			#region pass-by-value parameters
			dynamic format = XVar.Clone(_param_format);
			#endregion

			return (XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_DATE_SHORT)))  || (XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_DATE_LONG))))  || (XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_DATE_TIME))))  || (XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_TIME))))  || (XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_CURRENCY))))  || (XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_PERCENT))))  || (XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_LOOKUP_WIZARD))))  || (XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_PHONE_NUMBER))))  || (XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_NUMBER))))  || (XVar)(XVar.Equals(XVar.Pack(format), XVar.Pack(Constants.FORMAT_CHECKBOX)));
		}
		public static XVar getCustomMessage(dynamic _param_message)
		{
			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			#endregion

			if((XVar)(!(XVar)(message))  || (XVar)(!(XVar)(MVCFunctions.is_array((XVar)(message)))))
			{
				return "";
			}
			if(message["messageType"] == "Text")
			{
				return message["message"];
			}
			else
			{
				return CommonFunctions.GetCustomLabel((XVar)(message["message"]));
			}

			return null;
		}
		public static XVar availablePage(dynamic _param_table, dynamic _param_pageType, dynamic _param_page = null)
		{
			#region default values
			if(_param_page as Object == null) _param_page = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic page = XVar.Clone(_param_page);
			#endregion

			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table), (XVar)(pageType), (XVar)(page)));
			return pSet.pageName();
		}
		public static XVar formatDateIntervalValue(dynamic _param_value, dynamic _param_intervalType)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic intervalType = XVar.Clone(_param_intervalType);
			#endregion

			dynamic dates = XVar.Array(), tm = XVar.Array();
			tm = XVar.Clone(XVar.Array());
			tm.InitAndSetArrayItem((int)MVCFunctions.substr((XVar)(value), new XVar(0), new XVar(4)), 0);
			tm.InitAndSetArrayItem((int)MVCFunctions.substr((XVar)(value), new XVar(4), new XVar(2)), 1);
			tm.InitAndSetArrayItem((int)MVCFunctions.substr((XVar)(value), new XVar(6), new XVar(2)), 2);
			tm.InitAndSetArrayItem((int)MVCFunctions.substr((XVar)(value), new XVar(8), new XVar(2)), 3);
			tm.InitAndSetArrayItem((int)MVCFunctions.substr((XVar)(value), new XVar(10), new XVar(2)), 4);
			tm.InitAndSetArrayItem(0, 5);
			if(XVar.Pack(!(XVar)(tm[0])))
			{
				return "";
			}
			switch(((XVar)intervalType).ToInt())
			{
				case 1:
					return MVCFunctions.substr((XVar)(value), new XVar(0), new XVar(4));
				case 2:
					return MVCFunctions.Concat(tm[0], "/Q", tm[1]);
				case 3:
					return MVCFunctions.Concat(GlobalVars.locale_info[MVCFunctions.Concat("LOCALE_SABBREVMONTHNAME", tm[1])], " ", tm[0]);
				case 4:
					dates = XVar.Clone(CommonFunctions.getDatesByWeek((XVar)(tm[1] + 1), (XVar)(tm[0])));
					return MVCFunctions.Concat(CommonFunctions.format_shortdate((XVar)(MVCFunctions.db2time((XVar)(dates[0])))), " - ", CommonFunctions.format_shortdate((XVar)(MVCFunctions.db2time((XVar)(dates[1])))));
				case 5:
					return CommonFunctions.format_shortdate((XVar)(tm));
				case 6:
					tm.InitAndSetArrayItem(0, 4);
					tm.InitAndSetArrayItem(0, 5);
					return CommonFunctions.str_format_datetime((XVar)(tm));
				case 7:
					tm.InitAndSetArrayItem(0, 5);
					return CommonFunctions.str_format_datetime((XVar)(tm));
				default:
					return CommonFunctions.str_format_datetime((XVar)(tm));
			}

			return null;
		}
		public static XVar pageTypeShowsData(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			return (XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(pageType == "list")  || (XVar)(pageType == "report"))  || (XVar)(pageType == "chart"))  || (XVar)(pageType == "view"))  || (XVar)(pageType == "export"))  || (XVar)(pageType == "edit"))  || (XVar)(pageType == "print"))  || (XVar)(pageType == "rprint"))  || (XVar)(pageType == "masterlist"))  || (XVar)(pageType == "masterprint"))  || (XVar)(pageType == "userinfo");
		}
		public static XVar pageTypeInputsData(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			return (XVar)((XVar)((XVar)((XVar)((XVar)(pageType == "add")  || (XVar)(pageType == "edit"))  || (XVar)(pageType == "search"))  || (XVar)(pageType == "register"))  || (XVar)(pageType == "login"))  || (XVar)(pageType == "userinfo");
		}
		public static XVar base64_encode_url(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.str_replace((XVar)(new XVar(0, "+", 1, "/", 2, "=")), (XVar)(new XVar(0, "-", 1, "_", 2, "")), (XVar)(MVCFunctions.base64_encode((XVar)(str))));
		}
		public static XVar base64_encode_url_binary(dynamic _param_bin)
		{
			#region pass-by-value parameters
			dynamic bin = XVar.Clone(_param_bin);
			#endregion

			return MVCFunctions.str_replace((XVar)(new XVar(0, "+", 1, "/", 2, "=")), (XVar)(new XVar(0, "-", 1, "_", 2, "")), (XVar)(MVCFunctions.base64_encode_binary((XVar)(bin))));
		}
		public static XVar base64_decode_url(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic ret = null;
			while(!XVar.Equals(XVar.Pack(MVCFunctions.strlen((XVar)(str))  %  4), XVar.Pack(0)))
			{
				str = MVCFunctions.Concat(str, "=");
			}
			ret = XVar.Clone(MVCFunctions.base64_decode((XVar)(MVCFunctions.str_replace((XVar)(new XVar(0, "-", 1, "_")), (XVar)(new XVar(0, "+", 1, "/")), (XVar)(str)))));
			return ret;
		}
		public static XVar base64_decode_url_binary(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic ret = null;
			while(!XVar.Equals(XVar.Pack(MVCFunctions.strlen((XVar)(str))  %  4), XVar.Pack(0)))
			{
				str = MVCFunctions.Concat(str, "=");
			}
			ret = XVar.Clone(MVCFunctions.base64_decode_binary((XVar)(MVCFunctions.str_replace((XVar)(new XVar(0, "-", 1, "_")), (XVar)(new XVar(0, "+", 1, "/")), (XVar)(str)))));
			return ret;
		}
		public static XVar jwt_encode(dynamic _param_payload, dynamic _param_duration = null)
		{
			#region default values
			if(_param_duration as Object == null) _param_duration = new XVar(10);
			#endregion

			#region pass-by-value parameters
			dynamic payload = XVar.Clone(_param_payload);
			dynamic duration = XVar.Clone(_param_duration);
			#endregion

			dynamic base64UrlSignature = null, header64 = null, jwtSecret = null, payload64 = null, signature = null;
			if(XVar.Pack(!(XVar)(payload["exp"])))
			{
				payload.InitAndSetArrayItem(MVCFunctions.time() + duration, "exp");
			}
			header64 = XVar.Clone(CommonFunctions.base64_encode_url((XVar)(MVCFunctions.runner_json_encode((XVar)(new XVar("typ", "JWT", "alg", "HS256"))))));
			payload64 = XVar.Clone(CommonFunctions.base64_encode_url((XVar)(MVCFunctions.runner_json_encode((XVar)(payload)))));
			jwtSecret = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("sessionControl"), new XVar("JWTSecret")));
			signature = XVar.Clone(MVCFunctions.hash_hmac_sha256((XVar)(MVCFunctions.Concat(header64, ".", payload64)), (XVar)(jwtSecret), new XVar(true)));
			base64UrlSignature = XVar.Clone(CommonFunctions.base64_encode_url_binary((XVar)(signature)));
			return MVCFunctions.Concat(header64, ".", payload64, ".", base64UrlSignature);
		}
		public static XVar jwt_verify_decode(dynamic _param_jwt)
		{
			#region pass-by-value parameters
			dynamic jwt = XVar.Clone(_param_jwt);
			#endregion

			dynamic jwtSecret = null, parts = XVar.Array(), ret = XVar.Array(), signature = null;
			parts = XVar.Clone(MVCFunctions.explode(new XVar("."), (XVar)(jwt)));
			if(MVCFunctions.count(parts) != 3)
			{
				return false;
			}
			signature = XVar.Clone(parts[2]);
			jwtSecret = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("sessionControl"), new XVar("JWTSecret")));
			if(!XVar.Equals(XVar.Pack(CommonFunctions.base64_encode_url_binary((XVar)(MVCFunctions.hash_hmac_sha256((XVar)(MVCFunctions.Concat(parts[0], ".", parts[1])), (XVar)(jwtSecret), new XVar(true))))), XVar.Pack(signature)))
			{
				return false;
			}
			ret = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(CommonFunctions.base64_decode_url((XVar)(parts[1])))));
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(ret)))))
			{
				return false;
			}
			if((XVar)(!(XVar)(ret["exp"]))  || (XVar)(ret["exp"] <= MVCFunctions.time()))
			{
				return false;
			}
			if((XVar)(!(XVar)(ret["host"]))  || (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.strtoupper((XVar)(ret["host"]))), XVar.Pack(MVCFunctions.strtoupper((XVar)(CommonFunctions.projectHost()))))))
			{
				return false;
			}
			return ret;
		}
		public static XVar request_protocol()
		{
			return (XVar.Pack((XVar)(MVCFunctions.GetServerVariable("HTTPS"))  && (XVar)(MVCFunctions.GetServerVariable("HTTPS") != "off")) ? XVar.Pack("https") : XVar.Pack("http"));
		}
		public static XVar projectURL()
		{
			return MVCFunctions.Concat(CommonFunctions.request_protocol(), "://", CommonFunctions.projectHost(), MVCFunctions.projectPath());
		}
		public static XVar projectOrigin()
		{
			return MVCFunctions.Concat(CommonFunctions.request_protocol(), "://", CommonFunctions.projectHost());
		}
		public static XVar projectHost()
		{
			dynamic host = null;
			host = XVar.Clone(MVCFunctions.GetServerVariable("HTTP_X_FORWARDED_HOST"));
			if(XVar.Pack(!(XVar)(host)))
			{
				host = XVar.Clone(MVCFunctions.GetServerVariable("HTTP_HOST"));
			}
			return host;
		}
		public static XVar hostFromUrl(dynamic _param_url)
		{
			#region pass-by-value parameters
			dynamic url = XVar.Clone(_param_url);
			#endregion

			dynamic hostEnd = null, hostStart = null;
			hostStart = XVar.Clone(MVCFunctions.strpos((XVar)(url), new XVar("://")));
			if(XVar.Equals(XVar.Pack(hostStart), XVar.Pack(false)))
			{
				return "";
			}
			hostStart += 3;
			hostEnd = XVar.Clone(MVCFunctions.strpos((XVar)(url), new XVar("/"), (XVar)(hostStart)));
			if(XVar.Equals(XVar.Pack(hostEnd), XVar.Pack(false)))
			{
				return MVCFunctions.substr((XVar)(url), (XVar)(hostStart));
			}
			return MVCFunctions.substr((XVar)(url), (XVar)(hostStart), (XVar)(hostEnd - hostStart));
		}
		public static XVar setProjectCookie(dynamic _param_name, dynamic _param_value, dynamic _param_expires = null, dynamic _param_httpOnly = null, dynamic _param_sameSiteStrict = null)
		{
			#region default values
			if(_param_expires as Object == null) _param_expires = new XVar(0);
			if(_param_httpOnly as Object == null) _param_httpOnly = new XVar(false);
			if(_param_sameSiteStrict as Object == null) _param_sameSiteStrict = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic value = XVar.Clone(_param_value);
			dynamic expires = XVar.Clone(_param_expires);
			dynamic httpOnly = XVar.Clone(_param_httpOnly);
			dynamic sameSiteStrict = XVar.Clone(_param_sameSiteStrict);
			#endregion

			CommonFunctions.runner_setcookie((XVar)(name), (XVar)(value), (XVar)(expires), (XVar)(MVCFunctions.projectPath()), new XVar(""), (XVar)(CommonFunctions.isSecureProtocol()), (XVar)(httpOnly), (XVar)(sameSiteStrict));

			return null;
		}
		public static XVar imageDataUrl(dynamic _param_image)
		{
			#region pass-by-value parameters
			dynamic image = XVar.Clone(_param_image);
			#endregion

			dynamic imageType = null;
			imageType = XVar.Clone(MVCFunctions.SupposeImageType((XVar)(image)));
			if((XVar)(imageType == "image/jpeg")  || (XVar)(imageType == "image/png"))
			{
				return MVCFunctions.Concat("data:", imageType, ";base64,", MVCFunctions.base64_bin2str((XVar)(image)));
			}
			return false;
		}
		public static XVar ldap_getUrl(dynamic _param_address)
		{
			#region pass-by-value parameters
			dynamic address = XVar.Clone(_param_address);
			#endregion

			if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(address), new XVar("://"))), XVar.Pack(false)))
			{
				return address;
			}
			return MVCFunctions.Concat("ldap://", address);
		}
		public static XVar ldap_getServer(dynamic _param_address)
		{
			#region pass-by-value parameters
			dynamic address = XVar.Clone(_param_address);
			#endregion

			dynamic pos = null;
			pos = XVar.Clone(MVCFunctions.strpos((XVar)(address), new XVar("://")));
			if(!XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
			{
				return MVCFunctions.substr((XVar)(address), (XVar)(pos + 3));
			}
			return address;
		}
		public static XVar ldap_getServerPort(dynamic _param_address)
		{
			#region pass-by-value parameters
			dynamic address = XVar.Clone(_param_address);
			#endregion

			dynamic portPos = null, server = null;
			server = XVar.Clone(CommonFunctions.ldap_getServer((XVar)(address)));
			portPos = XVar.Clone(MVCFunctions.strpos((XVar)(server), new XVar(":")));
			if((XVar)(XVar.Equals(XVar.Pack(portPos), XVar.Pack(false)))  && (XVar)(MVCFunctions.strtolower((XVar)(MVCFunctions.substr((XVar)(address), new XVar(0), new XVar(8)))) == "ldaps://"))
			{
				server = MVCFunctions.Concat(server, ":636");
			}
			return server;
		}
		public static XVar ldap_Domain2DN(dynamic _param_aDomain)
		{
			#region pass-by-value parameters
			dynamic aDomain = XVar.Clone(_param_aDomain);
			#endregion

			dynamic arrDomain = XVar.Array(), i = null;
			if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(aDomain), new XVar("="))), XVar.Pack(false)))
			{
				return aDomain;
			}
			arrDomain = XVar.Clone(MVCFunctions.explode(new XVar("."), (XVar)(aDomain)));
			i = new XVar(0);
			for(;i < MVCFunctions.count(arrDomain); i++)
			{
				arrDomain.InitAndSetArrayItem(MVCFunctions.Concat("dc=", arrDomain[i]), i);
			}
			return MVCFunctions.implode(new XVar(","), (XVar)(arrDomain));
		}
		public static XVar ldap_DN2Domain(dynamic _param_dn)
		{
			#region pass-by-value parameters
			dynamic dn = XVar.Clone(_param_dn);
			#endregion

			dynamic dom = XVar.Array();
			if(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(dn), new XVar("="))), XVar.Pack(false)))
			{
				return dn;
			}
			dom = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> d in MVCFunctions.explode(new XVar(","), (XVar)(dn)).GetEnumerator())
			{
				if(MVCFunctions.strtoupper((XVar)(MVCFunctions.substr((XVar)(d.Value), new XVar(0), new XVar(3)))) == "DC=")
				{
					dom.InitAndSetArrayItem(MVCFunctions.substr((XVar)(d.Value), new XVar(3)), null);
				}
			}
			return MVCFunctions.implode(new XVar("."), (XVar)(dom));
		}
		public static XVar getLookupDataSource(dynamic _param_field, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic connId = null, lookupTable = null, lookupType = null;
			lookupTable = XVar.Clone(pSet.getLookupTable((XVar)(field)));
			lookupType = XVar.Clone(pSet.getLookupType((XVar)(field)));
			if(XVar.Pack(!(XVar)(lookupTable)))
			{
				return null;
			}
			if(lookupType == Constants.LT_QUERY)
			{
				connId = XVar.Clone(GlobalVars.cman.byTable((XVar)(lookupTable)).connId);
			}
			else
			{
				connId = XVar.Clone(pSet.getNotProjectLookupTableConnId((XVar)(field)));
			}
			return CommonFunctions.getTableDataSource((XVar)(lookupTable), (XVar)(connId));
		}
		public static XVar getLoginDataSource()
		{
			return CommonFunctions.getTableDataSource((XVar)(Security.loginTable()), (XVar)(GlobalVars.cman.getLoginConnId()));
		}
		public static XVar getTableDataSource(dynamic _param_table, dynamic _param_connId)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic connId = XVar.Clone(_param_connId);
			#endregion

			dynamic connection = null;
			connection = XVar.Clone(GlobalVars.cman.byId((XVar)(connId)));
			if((XVar)(CommonFunctions.GetTableURL((XVar)(table)))  && (XVar)(GlobalVars.cman.getTableConnId((XVar)(table)) == connId))
			{
				ProjectSettings pSet;
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
				return CommonFunctions.getDataSource((XVar)(table), (XVar)(pSet), (XVar)(connection));
			}
			return CommonFunctions.getDbTableDataSource((XVar)(table), (XVar)(connId));
		}
		public static XVar getDbTableDataSource(dynamic _param_table, dynamic _param_connId)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic connId = XVar.Clone(_param_connId);
			#endregion

			dynamic dbTableInfo = null;
			dbTableInfo = XVar.Clone(DB._getTableInfo((XVar)(table), (XVar)(connId)));
			if(XVar.Pack(dbTableInfo))
			{
				return new DataSourceDbTable((XVar)(table), (XVar)(GlobalVars.cman.byId((XVar)(connId))), (XVar)(dbTableInfo));
			}
			return null;
		}
		public static XVar getWebDataSource(dynamic report, dynamic _param_tableType = null, dynamic _param_tName = null)
		{
			#region default values
			if(_param_tableType as Object == null) _param_tableType = new XVar("");
			if(_param_tName as Object == null) _param_tName = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic tableType = XVar.Clone(_param_tableType);
			dynamic tName = XVar.Clone(_param_tName);
			#endregion

			dynamic connId = null, connection = null, table = null, table_type = null;
			table_type = XVar.Clone(report["table_type"]);
			if(XVar.Pack(tableType))
			{
				table_type = XVar.Clone(tableType);
			}
			if(XVar.Pack(tName))
			{
				table = XVar.Clone(tName);
			}
			else
			{
				table = XVar.Clone(report["tables"][0]);
			}
			connId = XVar.Clone(GlobalVars.cman.getDefaultConnId());
			connection = XVar.Clone(GlobalVars.cman.getDefault());
			if(table_type == "db")
			{
				dynamic dbTableInfo = null;
				dbTableInfo = XVar.Clone(DB._getTableInfo((XVar)(table), (XVar)(connId)));
				if(XVar.Pack(dbTableInfo))
				{
					return new DataSourceWebTable((XVar)(table), (XVar)(connection), (XVar)(report), (XVar)(dbTableInfo));
				}
			}
			else
			{
				if(table_type == "custom")
				{
					return new DataSourceWebSQL((XVar)(connection), (XVar)(report));
				}
			}
			return null;
		}
		public static XVar getDataSource(dynamic _param_table, dynamic _param_pSet_packed = null, dynamic _param_connection = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_pSet as Object == null) _param_pSet = null;
			if(_param_connection as Object == null) _param_connection = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			dynamic var_type = null;
			if(XVar.Pack(!(XVar)(pSet)))
			{
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			}
			if(XVar.Pack(!(XVar)(connection)))
			{
				connection = XVar.Clone(GlobalVars.cman.byId((XVar)(pSet.getConnId())));
			}
			var_type = XVar.Clone(pSet.getEntityType());
			if((XVar)((XVar)((XVar)(var_type == Constants.titTABLE)  || (XVar)(var_type == Constants.titVIEW))  || (XVar)(var_type == Constants.titCHART))  || (XVar)(var_type == Constants.titREPORT))
			{
				return new DataSourceProjectTable((XVar)(table), (XVar)(pSet), (XVar)(connection));
			}
			if((XVar)((XVar)(var_type == Constants.titSQL)  || (XVar)(var_type == Constants.titSQL_CHART))  || (XVar)(var_type == Constants.titSQL_REPORT))
			{
				return new DataSourceSQL((XVar)(table), (XVar)(pSet), (XVar)(connection));
			}
			if((XVar)((XVar)(var_type == Constants.titREST)  || (XVar)(var_type == Constants.titREST_REPORT))  || (XVar)(var_type == Constants.titREST_CHART))
			{
				return new DataSourceREST((XVar)(table), (XVar)(pSet), (XVar)(GlobalVars.restApis.getConnection((XVar)(pSet.getConnId()))));
			}
			return null;
		}
		public static XVar getRestConnection(dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic var_type = null;
			var_type = XVar.Clone(pSet.getEntityType());
			if((XVar)((XVar)(var_type == Constants.titREST)  || (XVar)(var_type == Constants.titREST_REPORT))  || (XVar)(var_type == Constants.titREST_CHART))
			{
				return GlobalVars.restApis.getConnection((XVar)(pSet.getConnId()));
			}
			return null;
		}
		public static XVar inRestApi()
		{
			return GlobalVars.restApiCall as object != null;
		}
		public static XVar isReport(dynamic _param_entityType)
		{
			#region pass-by-value parameters
			dynamic entityType = XVar.Clone(_param_entityType);
			#endregion

			return (XVar)((XVar)(entityType == Constants.titREPORT)  || (XVar)(entityType == Constants.titSQL_REPORT))  || (XVar)(entityType == Constants.titREST_REPORT);
		}
		public static XVar isChart(dynamic _param_entityType)
		{
			#region pass-by-value parameters
			dynamic entityType = XVar.Clone(_param_entityType);
			#endregion

			return (XVar)((XVar)(entityType == Constants.titCHART)  || (XVar)(entityType == Constants.titSQL_CHART))  || (XVar)(entityType == Constants.titREST_CHART);
		}
		public static XVar prepareUrl(dynamic _param_url, dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic url = XVar.Clone(_param_url);
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic body = null, glue = null, lastSymbol = null;
			if(XVar.Pack(!(XVar)(url)))
			{
				return "";
			}
			if(XVar.Pack(!(XVar)(var_params)))
			{
				return url;
			}
			body = XVar.Clone(CommonFunctions.prepareUrlQuery((XVar)(var_params)));
			glue = XVar.Clone((XVar.Pack(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(url), new XVar("?"))), XVar.Pack(false))) ? XVar.Pack("&") : XVar.Pack("?")));
			lastSymbol = XVar.Clone(MVCFunctions.substr((XVar)(url), (XVar)(MVCFunctions.strlen((XVar)(url)) - 1), new XVar(1)));
			if((XVar)(lastSymbol == "&")  || (XVar)(lastSymbol == "?"))
			{
				glue = new XVar("");
			}
			return MVCFunctions.Concat(url, glue, body);
		}
		public static XVar prepareUrlQuery(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic data = XVar.Array();
			data = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in var_params.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.is_array((XVar)(value.Value))))
				{
					foreach (KeyValuePair<XVar, dynamic> item in value.Value.GetEnumerator())
					{
						data.InitAndSetArrayItem(MVCFunctions.Concat(MVCFunctions.RawUrlEncode((XVar)(value.Key)), "=", MVCFunctions.RawUrlEncode((XVar)(item.Value))), null);
					}
				}
				else
				{
					data.InitAndSetArrayItem(MVCFunctions.Concat(MVCFunctions.RawUrlEncode((XVar)(value.Key)), "=", MVCFunctions.RawUrlEncode((XVar)(value.Value))), null);
				}
			}
			return MVCFunctions.implode(new XVar("&"), (XVar)(data));
		}
		public static XVar getRESTConn(dynamic _param_name = null)
		{
			#region default values
			if(_param_name as Object == null) _param_name = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			dynamic id = null;
			id = XVar.Clone(GlobalVars.restApis.idByName((XVar)(name)));
			return GlobalVars.restApis.getConnection((XVar)(id));
		}
		public static XVar storageGet(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			if(XVar.Pack(!(XVar)(CommonFunctions.inRestApi())))
			{
				return XSession.Session[key];
			}
			else
			{
				return GlobalVars.restStorage[key];
			}

			return null;
		}
		public static XVar storageSet(dynamic _param_key, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(XVar.Pack(!(XVar)(CommonFunctions.inRestApi())))
			{
				XSession.Session[key] = value;
			}
			else
			{
				GlobalVars.restStorage.InitAndSetArrayItem(value, key);
			}

			return null;
		}
		public static XVar storageDelete(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			if(XVar.Pack(!(XVar)(CommonFunctions.inRestApi())))
			{
				XSession.Session.Remove(key);
			}
			else
			{
				GlobalVars.restStorage.Remove(key);
			}

			return null;
		}
		public static XVar storageKeys()
		{
			if(XVar.Pack(!(XVar)(CommonFunctions.inRestApi())))
			{
				dynamic ret = XVar.Array();
				ret = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> value in XSession.Session.GetEnumerator())
				{
					ret.InitAndSetArrayItem(value.Key, null);
				}
				return ret;
			}
			else
			{
				return MVCFunctions.array_keys((XVar)(GlobalVars.restStorage));
			}

			return null;
		}
		public static XVar storageExists(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			if(XVar.Pack(!(XVar)(CommonFunctions.inRestApi())))
			{
				return XSession.Session.KeyExists(key);
			}
			else
			{
				return GlobalVars.restStorage.KeyExists(key);
			}

			return null;
		}
		public static XVar storageFindOrCreate(dynamic _param_key, dynamic _param_defaultValue = null)
		{
			#region default values
			if(_param_defaultValue as Object == null) _param_defaultValue = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			dynamic defaultValue = XVar.Clone(_param_defaultValue);
			#endregion

			if(XVar.Pack(!(XVar)(CommonFunctions.inRestApi())))
			{
				if(XVar.Pack(!(XVar)(XSession.Session.KeyExists(key))))
				{
					XSession.Session[key] = defaultValue;
				}
				return XSession.Session[key];
			}
			else
			{
				if(XVar.Pack(!(XVar)(GlobalVars.restStorage.KeyExists(key))))
				{
					GlobalVars.restStorage.InitAndSetArrayItem(defaultValue, key);
				}
				return GlobalVars.restStorage[key];
			}

			return null;
		}
		public static XVar requestMethod()
		{
			return MVCFunctions.GetServerVariable("REQUEST_METHOD");
		}
		public static XVar isPostRequest()
		{
			return XVar.Equals(XVar.Pack(CommonFunctions.requestMethod()), XVar.Pack("POST"));
		}
		public static XVar runner_setcookie(dynamic _param_name, dynamic _param_value = null, dynamic _param_expires = null, dynamic _param_path = null, dynamic _param_domain = null, dynamic _param_secure = null, dynamic _param_httpOnly = null, dynamic _param_sameSiteStrict = null)
		{
			#region default values
			if(_param_value as Object == null) _param_value = new XVar("");
			if(_param_expires as Object == null) _param_expires = new XVar(0);
			if(_param_path as Object == null) _param_path = new XVar("");
			if(_param_domain as Object == null) _param_domain = new XVar("");
			if(_param_secure as Object == null) _param_secure = new XVar(false);
			if(_param_httpOnly as Object == null) _param_httpOnly = new XVar(false);
			if(_param_sameSiteStrict as Object == null) _param_sameSiteStrict = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic value = XVar.Clone(_param_value);
			dynamic expires = XVar.Clone(_param_expires);
			dynamic path = XVar.Clone(_param_path);
			dynamic domain = XVar.Clone(_param_domain);
			dynamic secure = XVar.Clone(_param_secure);
			dynamic httpOnly = XVar.Clone(_param_httpOnly);
			dynamic sameSiteStrict = XVar.Clone(_param_sameSiteStrict);
			#endregion

			dynamic options = XVar.Array();
			options = XVar.Clone(XVar.Array());
			options.InitAndSetArrayItem(MVCFunctions.Concat(name, "=", MVCFunctions.RawUrlEncode((XVar)(value)), ";"), null);
			options.InitAndSetArrayItem(MVCFunctions.Concat("Path=", path, ";"), null);
			if(XVar.Pack(expires))
			{
				options.InitAndSetArrayItem(MVCFunctions.Concat("Expires=", MVCFunctions.httpDateString((XVar)(expires)), ";"), null);
			}
			if(XVar.Pack(httpOnly))
			{
				options.InitAndSetArrayItem("HttpOnly;", null);
			}
			if(XVar.Pack(secure))
			{
				options.InitAndSetArrayItem("Secure;", null);
			}
			if(XVar.Pack(sameSiteStrict))
			{
				options.InitAndSetArrayItem("SameSite=Strict;", null);
			}
			MVCFunctions.addHeader((XVar)(MVCFunctions.Concat("Set-Cookie: ", MVCFunctions.implode(new XVar(" "), (XVar)(options)))));

			return null;
		}
		public static XVar sendEmailTemplate(dynamic _param_toEmail, dynamic _param_templateFile, dynamic _param_data, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic toEmail = XVar.Clone(_param_toEmail);
			dynamic templateFile = XVar.Clone(_param_templateFile);
			dynamic data = XVar.Clone(_param_data);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic body = null, i = null, j = null, key = null, m = XVar.Array(), matches = XVar.Array(), offsetDelta = null, subject = null, templParts = XVar.Array(), value = null, var_params = XVar.Array();
			data.InitAndSetArrayItem(CommonFunctions.projectURL(), "url");
			if(XVar.Pack(!(XVar)(data.KeyExists("loginUrl"))))
			{
				data.InitAndSetArrayItem(MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("login"))), "loginUrl");
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.file_exists((XVar)(MVCFunctions.getabspath((XVar)(templateFile)))))))
			{
				return false;
			}
			body = XVar.Clone(MVCFunctions.myfile_get_contents((XVar)(MVCFunctions.getabspath((XVar)(templateFile))), new XVar("r")));
			matches = XVar.Clone(MVCFunctions.findMatches(new XVar("/%[^%\\W]+%/i"), (XVar)(body)));
			i = new XVar(0);
			for(;i < MVCFunctions.count(matches); ++(i))
			{
				m = XVar.Clone(matches[i]);
				key = XVar.Clone(MVCFunctions.substr((XVar)(m["match"]), new XVar(1), (XVar)(MVCFunctions.strlen((XVar)(m["match"])) - 2)));
				value = XVar.Clone(MVCFunctions.Concat("", CommonFunctions.getArrayElementNC((XVar)(data), (XVar)(key))));
				if(XVar.Pack(html))
				{
					value = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(value)));
				}
				body = XVar.Clone(MVCFunctions.Concat(MVCFunctions.substr((XVar)(body), new XVar(0), (XVar)(m["offset"])), value, MVCFunctions.substr((XVar)(body), (XVar)(m["offset"] + MVCFunctions.strlen((XVar)(m["match"]))))));
				offsetDelta = XVar.Clone(MVCFunctions.strlen((XVar)(value)) - MVCFunctions.strlen((XVar)(m["match"])));
				j = XVar.Clone(i + 1);
				for(;j < MVCFunctions.count(matches); ++(j))
				{
					matches.InitAndSetArrayItem(matches[j]["offset"] + offsetDelta, j, "offset");
				}
			}
			subject = new XVar("");
			templParts = XVar.Clone(CommonFunctions.splitFirstLine((XVar)(body)));
			subject = XVar.Clone(templParts[0]);
			body = XVar.Clone(templParts[1]);
			var_params = XVar.Clone(new XVar("to", toEmail, "subject", subject, "charset", GlobalVars.cCharset));
			if(XVar.Pack(html))
			{
				var_params.InitAndSetArrayItem(body, "htmlbody");
			}
			else
			{
				var_params.InitAndSetArrayItem(body, "body");
			}
			return MVCFunctions.runner_mail((XVar)(var_params));
		}
		public static XVar base32symbols()
		{
			return new XVar(0, "A", 1, "B", 2, "C", 3, "D", 4, "E", 5, "F", 6, "G", 7, "H", 8, "I", 9, "J", 10, "K", 11, "L", 12, "M", 13, "N", 14, "O", 15, "P", 16, "Q", 17, "R", 18, "S", 19, "T", 20, "U", 21, "V", 22, "W", 23, "X", 24, "Y", 25, "Z", 26, "2", 27, "3", 28, "4", 29, "5", 30, "6", 31, "7");
		}
		public static XVar generateTotpSecret()
		{
			dynamic i = null, ret = null, table = XVar.Array();
			table = CommonFunctions.base32symbols();
			ret = new XVar("");
			i = new XVar(0);
			for(;i < 16; ++(i))
			{
				ret = MVCFunctions.Concat(ret, table[MVCFunctions.rand(new XVar(0), new XVar(31))]);
			}
			return ret;
		}
		public static XVar validateTotpSecret(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic i = null, table = null;
			if(MVCFunctions.strlen((XVar)(str)) != 16)
			{
				return false;
			}
			table = CommonFunctions.base32symbols();
			i = new XVar(0);
			for(;i < 16; ++(i))
			{
				if(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(str[i]), (XVar)(table))), XVar.Pack(false)))
				{
					return false;
				}
			}
			return true;
		}
		public static XVar validateEmail(dynamic _param_address)
		{
			#region pass-by-value parameters
			dynamic address = XVar.Clone(_param_address);
			#endregion

			return (XVar)(3 <= MVCFunctions.strlen((XVar)(address)))  && (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(address), new XVar("@"))), XVar.Pack(false)));
		}
		public static XVar maskPhoneNumber(dynamic _param_number)
		{
			#region pass-by-value parameters
			dynamic number = XVar.Clone(_param_number);
			#endregion

			dynamic astrixStringLength = null, smsMaskLength = null;
			smsMaskLength = XVar.Clone(ProjectSettings.getProjectValue(new XVar("smsMaskLength")));
			astrixStringLength = XVar.Clone(MVCFunctions.strlen((XVar)(number)) - smsMaskLength);
			number = XVar.Clone(MVCFunctions.Concat(MVCFunctions.preg_replace(new XVar("/[^+]/"), new XVar("*"), (XVar)(MVCFunctions.substr((XVar)(number), new XVar(0), (XVar)(astrixStringLength)))), MVCFunctions.substr((XVar)(number), (XVar)(astrixStringLength))));
			return number;
		}
		public static XVar maskEmailAddress(dynamic _param_address)
		{
			#region pass-by-value parameters
			dynamic address = XVar.Clone(_param_address);
			#endregion

			return address;
		}
		public static XVar runner_show_error(dynamic _param_errinfo)
		{
			#region pass-by-value parameters
			dynamic errinfo = XVar.Clone(_param_errinfo);
			#endregion

			dynamic debugRows = XVar.Array(), showArgs = null, sql = null;
			XTempl xt;
			errinfo.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(errinfo["errstr"])), "errstr");
			errinfo.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(errinfo["url"])), "url");
			sql = XVar.Clone(errinfo["sqlStr"]);
			errinfo.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)((XVar.Pack(MVCFunctions.strlen((XVar)(sql)) < 20480) ? XVar.Pack(sql) : XVar.Pack(MVCFunctions.Concat(MVCFunctions.substr((XVar)(sql), new XVar(0), new XVar(20480)), "..."))))), "sqlStr");
			showArgs = new XVar(false);
			debugRows = XVar.Clone(XVar.Array());
			if(XVar.Pack(errinfo.KeyExists("debugRows")))
			{
				foreach (KeyValuePair<XVar, dynamic> row in errinfo["debugRows"].GetEnumerator())
				{
					dynamic dRow = XVar.Array();
					dRow = XVar.Clone(XVar.Array());
					dRow.InitAndSetArrayItem(MVCFunctions.Concat("#", MVCFunctions.count(debugRows)), "num");
					dRow.InitAndSetArrayItem(MVCFunctions.Concat(row.Value["file"], ":", row.Value["line"]), "path");
					dRow.InitAndSetArrayItem(MVCFunctions.Concat(row.Value["class"], row.Value["type"], row.Value["function"]), "func");
					if(XVar.Pack(row.Value.KeyExists("params")))
					{
						dynamic args = XVar.Array();
						args = XVar.Clone(XVar.Array());
						showArgs = new XVar(true);
						foreach (KeyValuePair<XVar, dynamic> val in row.Value["params"].GetEnumerator())
						{
							dynamic prepVal = null;
							prepVal = XVar.Clone((XVar.Pack(MVCFunctions.strlen((XVar)(val.Value)) < 4096) ? XVar.Pack(val.Value) : XVar.Pack(MVCFunctions.Concat(MVCFunctions.substr((XVar)(val.Value), new XVar(0), new XVar(4096)), "..."))));
							args.InitAndSetArrayItem(MVCFunctions.Concat(MVCFunctions.count(args), ". ", prepVal), null);
						}
						dRow.InitAndSetArrayItem((XVar.Pack(MVCFunctions.count(args)) ? XVar.Pack(MVCFunctions.implode(new XVar("<br/> "), (XVar)(args))) : XVar.Pack("N/A")), "args");
					}
					debugRows.InitAndSetArrayItem(dRow, null);
				}
				errinfo.Remove("debugRows");
			}
			xt = XVar.UnPackXTempl(new XTempl());
			xt.assign(new XVar("showArgs"), (XVar)(showArgs));
			if(XVar.Pack(MVCFunctions.count(debugRows)))
			{
				xt.assign_loopsection(new XVar("debugRow"), (XVar)(debugRows));
			}
			xt.bulk_assign((XVar)(errinfo));
			xt.displayPartial((XVar)(CommonFunctions.GetTemplateName(new XVar(""), new XVar("error"))));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public static XVar numericToAssoc(dynamic _param_keys, dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			dynamic values = XVar.Clone(_param_values);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> k in keys.GetEnumerator())
			{
				ret.InitAndSetArrayItem(values[k.Key], k.Value);
			}
			return ret;
		}
		public static XVar findArrayInArray(dynamic _param_arr, dynamic _param_valueArr)
		{
			#region pass-by-value parameters
			dynamic arr = XVar.Clone(_param_arr);
			dynamic valueArr = XVar.Clone(_param_valueArr);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> value in arr.GetEnumerator())
			{
				dynamic equal = null;
				if(MVCFunctions.count(value.Value) != MVCFunctions.count(valueArr))
				{
					continue;
				}
				equal = new XVar(true);
				foreach (KeyValuePair<XVar, dynamic> v in value.Value.GetEnumerator())
				{
					if(valueArr[v.Key] != v.Value)
					{
						equal = new XVar(false);
						break;
					}
				}
				if(XVar.Pack(equal))
				{
					return value.Key;
				}
			}
			return false;
		}
		public static XVar originalTableField(dynamic _param_field, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic entityType = null, fieldTable = null;
			entityType = XVar.Clone(pSet.getEntityType());
			if((XVar)(entityType != Constants.titTABLE)  && (XVar)(entityType != Constants.titVIEW))
			{
				return true;
			}
			fieldTable = XVar.Clone(pSet.getOwnerTable((XVar)(field)));
			return fieldTable == pSet.getStrOriginalTableName();
		}
		public static XVar isEmailTemplateUseHTML(dynamic _param_template, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic template = XVar.Clone(_param_template);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			if(MVCFunctions.strlen((XVar)(lng)) == 0)
			{
				lng = XVar.Clone(CommonFunctions.mlang_getcurrentlang());
			}
			return !(XVar)(!(XVar)(ProjectSettings.getProjectValue(new XVar("emailTemplates"), (XVar)(lng), (XVar)(template), new XVar("useHTML"))));
		}
		public static XVar pre8count(dynamic arr)
		{
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(arr)))))
			{
				return 0;
			}
			return MVCFunctions.count(arr);
		}
		public static XVar ldapEscape(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic escapeChars = null, escapedChars = null;
			escapeChars = XVar.Clone(new XVar(0, ",", 1, "\\", 2, "#", 3, "+", 4, "<", 5, ">", 6, ";", 7, "\"", 8, "="));
			escapedChars = XVar.Clone(new XVar(0, "\\,", 1, "\\\\", 2, "\\#", 3, "\\+", 4, "\\<", 5, "\\>", 6, "\\;", 7, "\\\"", 8, "\\="));
			return MVCFunctions.str_replace((XVar)(escapeChars), (XVar)(escapedChars), (XVar)(str));
		}
		public static XVar getOneDriveConnection()
		{
			dynamic connData = XVar.Array(), tenant = null;
			connData = XVar.Clone(XVar.Array());
			connData.InitAndSetArrayItem(Constants.spidONEDRIVE, "id");
			connData.InitAndSetArrayItem("oauth", "authType");
			connData.InitAndSetArrayItem("https://graph.microsoft.com/v1.0", "url");
			tenant = new XVar("consumers");
			if(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudOneDriveAccountType")) == 1)
			{
				tenant = XVar.Clone(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudOneDriveDirectoryId")));
			}
			connData.InitAndSetArrayItem(MVCFunctions.Concat("https://login.microsoftonline.com/", tenant, "/oauth2/v2.0/authorize"), "authUrl");
			connData.InitAndSetArrayItem(MVCFunctions.Concat("https://login.microsoftonline.com/", tenant, "/oauth2/v2.0/token"), "tokenUrl");
			connData.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudOneDriveClientId")), "clientId");
			connData.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudOneDriveClientSecret")), "clientSecret");
			connData.InitAndSetArrayItem("files.readwrite offline_access", "scope");
			return new RestConnection((XVar)(connData));
		}
		public static XVar getGoogleDriveConnection()
		{
			dynamic connData = XVar.Array();
			connData = XVar.Clone(XVar.Array());
			connData.InitAndSetArrayItem(Constants.spidGOOGLEDRIVE, "id");
			connData.InitAndSetArrayItem("oauth", "authType");
			connData.InitAndSetArrayItem("https://www.googleapis.com/drive/v3/", "url");
			connData.InitAndSetArrayItem("https://accounts.google.com/o/oauth2/v2/auth", "authUrl");
			connData.InitAndSetArrayItem("https://oauth2.googleapis.com/token", "tokenUrl");
			connData.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudGDriveClientId")), "clientId");
			connData.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudGDriveClientSecret")), "clientSecret");
			connData.InitAndSetArrayItem("https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/drive.photos.readonly", "scope");
			return new RestConnection((XVar)(connData));
		}
		public static XVar getDropboxConnection()
		{
			dynamic connData = XVar.Array();
			connData = XVar.Clone(XVar.Array());
			connData.InitAndSetArrayItem(Constants.spidDROPBOX, "id");
			connData.InitAndSetArrayItem("oauth", "authType");
			connData.InitAndSetArrayItem(false, "sendOauthClientId");
			connData.InitAndSetArrayItem("https://www.dropbox.com/oauth2/authorize", "authUrl");
			connData.InitAndSetArrayItem("https://api.dropboxapi.com/oauth2/token", "tokenUrl");
			connData.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudDropboxClientId")), "clientId");
			connData.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudDropboxClientSecret")), "clientSecret");
			connData.InitAndSetArrayItem("files.content.read files.content.write files.metadata.read", "scope");
			return new RestConnection((XVar)(connData));
		}
		public static XVar getCloudProvider(dynamic _param_type, dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic path = XVar.Clone(_param_path);
			#endregion

			return CommonFunctions.createStorageObject((XVar)(var_type), (XVar)(new XVar("path", path)));
		}
		public static XVar createStorageObject(dynamic _param_providerType, dynamic _param_additionalParams)
		{
			#region pass-by-value parameters
			dynamic providerType = XVar.Clone(_param_providerType);
			dynamic additionalParams = XVar.Clone(_param_additionalParams);
			#endregion

			dynamic var_params = XVar.Array();
			var_params = XVar.Clone(XVar.Array());
			if(providerType == Constants.stpDISK)
			{
				var_params = XVar.Clone(MVCFunctions.array_merge((XVar)(var_params), (XVar)(additionalParams)));
				return new DiskFileSystem((XVar)(var_params));
			}
			if(providerType == Constants.stpAMAZON)
			{
				var_params.InitAndSetArrayItem(ProjectSettings.amazonAccessKey(), "accessKey");
				var_params.InitAndSetArrayItem(ProjectSettings.amazonSecretKey(), "secretKey");
				var_params.InitAndSetArrayItem(ProjectSettings.amazonBucket(), "bucket");
				var_params.InitAndSetArrayItem(ProjectSettings.amazonRegion(), "region");
				var_params = XVar.Clone(MVCFunctions.array_merge((XVar)(var_params), (XVar)(additionalParams)));
				return new S3FileSystem((XVar)(var_params));
			}
			if(providerType == Constants.stpGOOGLEDRIVE)
			{
				var_params = XVar.Clone(MVCFunctions.array_merge((XVar)(var_params), (XVar)(additionalParams)));
				return new GoogleDriveFileSystem((XVar)(var_params));
			}
			if(providerType == Constants.stpONEDRIVE)
			{
				var_params.InitAndSetArrayItem(ProjectSettings.oneDriveDrive(), "driveId");
				var_params = XVar.Clone(MVCFunctions.array_merge((XVar)(var_params), (XVar)(additionalParams)));
				return new OneDriveFileSystem((XVar)(var_params));
			}
			if(providerType == Constants.stpDROPBOX)
			{
				var_params = XVar.Clone(MVCFunctions.array_merge((XVar)(var_params), (XVar)(additionalParams)));
				return new DropboxFileSystem((XVar)(var_params));
			}
			if(providerType == Constants.stpWASABI)
			{
				var_params.InitAndSetArrayItem(ProjectSettings.wasabiAccessKey(), "accessKey");
				var_params.InitAndSetArrayItem(ProjectSettings.wasabiSecretKey(), "secretKey");
				var_params.InitAndSetArrayItem(ProjectSettings.wasabiBucket(), "bucket");
				var_params.InitAndSetArrayItem(ProjectSettings.wasabiRegion(), "region");
				var_params = XVar.Clone(MVCFunctions.array_merge((XVar)(var_params), (XVar)(additionalParams)));
				return new WasabiFileSystem((XVar)(var_params));
			}

			return null;
		}
		public static XVar getStorageProvider(dynamic _param_pSet_packed, dynamic _param_field)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic providerType = null, var_params = XVar.Array();
			providerType = XVar.Clone(pSet.fileStorageProvider((XVar)(field)));
			var_params = XVar.Clone(XVar.Array());
			if(providerType == Constants.stpDISK)
			{
				var_params.InitAndSetArrayItem(pSet.isAbsolute((XVar)(field)), "absolutePath");
				var_params.InitAndSetArrayItem(pSet.getUploadFolder((XVar)(field)), "path");
			}
			else
			{
				if(providerType == Constants.stpAMAZON)
				{
					var_params.InitAndSetArrayItem(pSet.amazonPath((XVar)(field)), "path");
				}
				else
				{
					if(providerType == Constants.stpGOOGLEDRIVE)
					{
						var_params.InitAndSetArrayItem(pSet.googleDriveFolder((XVar)(field)), "folder");
					}
					else
					{
						if(providerType == Constants.stpONEDRIVE)
						{
							var_params.InitAndSetArrayItem(pSet.oneDrivePath((XVar)(field)), "path");
						}
						else
						{
							if(providerType == Constants.stpDROPBOX)
							{
								var_params.InitAndSetArrayItem(pSet.dropboxPath((XVar)(field)), "path");
							}
							else
							{
								if(providerType == Constants.stpWASABI)
								{
									var_params.InitAndSetArrayItem(pSet.wasabiPath((XVar)(field)), "path");
								}
							}
						}
					}
				}
			}
			return CommonFunctions.createStorageObject((XVar)(providerType), (XVar)(var_params));
		}
		public static XVar fileAttrHash(dynamic _param_keyLink, dynamic _param_size, dynamic _param_lastModified = null)
		{
			#region default values
			if(_param_lastModified as Object == null) _param_lastModified = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic keyLink = XVar.Clone(_param_keyLink);
			dynamic size = XVar.Clone(_param_size);
			dynamic lastModified = XVar.Clone(_param_lastModified);
			#endregion

			if(XVar.Pack(!(XVar)(GlobalVars.cacheImages)))
			{
				return MVCFunctions.rand(new XVar(0), new XVar(32768));
			}
			return MVCFunctions.md5((XVar)(MVCFunctions.implode(new XVar("-"), (XVar)(new XVar(0, keyLink, 1, XVar.Pack(size).ToString(), 2, XVar.Pack(lastModified).ToString())))));
		}
		public static XVar getNotificationSettings()
		{
			return ProjectSettings.getProjectValue(new XVar("notifications"));
		}
		public static XVar createNotification(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic noti = null;
			noti = XVar.Clone(new RunnerNotifications((XVar)(CommonFunctions.getNotificationSettings())));
			return noti.create((XVar)(var_params));
		}
		public static XVar addNotification(dynamic _param_message, dynamic _param_title = null, dynamic _param_icon = null, dynamic _param_url = null, dynamic _param_expire = null, dynamic _param_user = null, dynamic _param_newWindow = null)
		{
			#region default values
			if(_param_title as Object == null) _param_title = new XVar();
			if(_param_icon as Object == null) _param_icon = new XVar();
			if(_param_url as Object == null) _param_url = new XVar();
			if(_param_expire as Object == null) _param_expire = new XVar();
			if(_param_user as Object == null) _param_user = new XVar();
			if(_param_newWindow as Object == null) _param_newWindow = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			dynamic title = XVar.Clone(_param_title);
			dynamic icon = XVar.Clone(_param_icon);
			dynamic url = XVar.Clone(_param_url);
			dynamic expire = XVar.Clone(_param_expire);
			dynamic user = XVar.Clone(_param_user);
			dynamic newWindow = XVar.Clone(_param_newWindow);
			#endregion

			dynamic var_params = XVar.Array();
			var_params = XVar.Clone(new XVar("message", message, "title", title, "icon", icon));
			var_params.InitAndSetArrayItem(url, "url");
			var_params.InitAndSetArrayItem(expire, "expire");
			var_params.InitAndSetArrayItem((XVar.Pack(MVCFunctions.is_string((XVar)(user))) ? XVar.Pack(new XVar("user", user)) : XVar.Pack(user)), "permissions");
			var_params.InitAndSetArrayItem(newWindow, "newWindow");
			return CommonFunctions.createNotification((XVar)(var_params));
		}
		public static XVar createPageLink(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic additionalParams = XVar.Array(), additionalStr = null, firstParam = null, keyLink = null, keyParamPrefix = null, pageType = null, queryParts = XVar.Array(), secondParam = null, shortTable = null, table = null;
			table = XVar.Clone(var_params["table"]);
			shortTable = XVar.Clone(CommonFunctions.GetTableURL((XVar)(table)));
			if((XVar)(table)  && (XVar)(!(XVar)(shortTable)))
			{
				shortTable = XVar.Clone(table);
				table = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(shortTable)));
			}
			pageType = XVar.Clone(var_params["pageType"]);
			if((XVar)(pageType == "edit")  || (XVar)(pageType == "view"))
			{
				keyParamPrefix = new XVar("editid");
			}
			else
			{
				if(pageType == "add")
				{
					keyParamPrefix = new XVar("copyid");
				}
			}
			if((XVar)((XVar)(keyParamPrefix)  && (XVar)(table))  && (XVar)(var_params["keys"]))
			{
				dynamic keyParams = XVar.Array(), keys = XVar.Array();
				ProjectSettings pSet;
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
				keys = XVar.Clone(var_params["keys"]);
				keyParams = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> k in pSet.getTableKeys().GetEnumerator())
				{
					if(XVar.Pack(keys.KeyExists(k.Key)))
					{
						keyParams.InitAndSetArrayItem(keys[k.Key], MVCFunctions.Concat(keyParamPrefix, k.Key + 1));
					}
					else
					{
						if(XVar.Pack(keys.KeyExists(k.Value)))
						{
							keyParams.InitAndSetArrayItem(keys[k.Value], MVCFunctions.Concat(keyParamPrefix, k.Key + 1));
						}
					}
				}
				keyLink = XVar.Clone(CommonFunctions.prepareUrlQuery((XVar)(keyParams)));
			}
			queryParts = XVar.Clone(XVar.Array());
			additionalParams = XVar.Clone(var_params["additionalParams"]);
			if(XVar.Pack(!(XVar)(additionalParams)))
			{
				additionalParams = XVar.Clone(XVar.Array());
			}
			if(XVar.Pack(var_params["page"]))
			{
				additionalParams.InitAndSetArrayItem(var_params["page"], "page");
			}
			additionalStr = XVar.Clone(CommonFunctions.prepareUrlQuery((XVar)(var_params["additionalParams"])));
			if(XVar.Pack(additionalStr))
			{
				queryParts.InitAndSetArrayItem(additionalStr, null);
			}
			if(XVar.Pack(keyLink))
			{
				queryParts.InitAndSetArrayItem(keyLink, null);
			}
			if(XVar.Pack(var_params["query"]))
			{
				queryParts.InitAndSetArrayItem(var_params["query"], null);
			}
			if(XVar.Pack(shortTable))
			{
				firstParam = XVar.Clone(shortTable);
				secondParam = XVar.Clone(pageType);
			}
			else
			{
				firstParam = XVar.Clone(pageType);
				secondParam = new XVar("");
			}
			return MVCFunctions.GetTableLink((XVar)(firstParam), (XVar)(secondParam), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(queryParts))));
		}
		public static XVar makePageLink(dynamic _param_table, dynamic _param_pageType, dynamic _param_keys = null, dynamic _param_additionalParams = null)
		{
			#region default values
			if(_param_keys as Object == null) _param_keys = new XVar();
			if(_param_additionalParams as Object == null) _param_additionalParams = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic keys = XVar.Clone(_param_keys);
			dynamic additionalParams = XVar.Clone(_param_additionalParams);
			#endregion

			dynamic var_params = XVar.Array();
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(table, "table");
			var_params.InitAndSetArrayItem(pageType, "pageType");
			var_params.InitAndSetArrayItem(keys, "keys");
			var_params.InitAndSetArrayItem(additionalParams, "additionalParams");
			return CommonFunctions.createPageLink((XVar)(var_params));
		}
		public static XVar getIconHTML(dynamic _param_iconDesc)
		{
			#region pass-by-value parameters
			dynamic iconDesc = XVar.Clone(_param_iconDesc);
			#endregion

			dynamic html = null, iconType = null;
			if(XVar.Pack(!(XVar)(iconDesc)))
			{
				return "";
			}
			if(XVar.Pack(iconDesc["file"]))
			{
				iconType = new XVar("file");
				html = XVar.Clone(MVCFunctions.Concat("<img src=\"images/menuicons/", MVCFunctions.RawUrlEncode((XVar)(iconDesc["file"])), "\">"));
			}
			else
			{
				if(XVar.Pack(iconDesc["glyph"]))
				{
					iconType = new XVar("text");
					html = XVar.Clone(MVCFunctions.Concat("<span class=\"glyphicon glyphicon-", iconDesc["glyph"], "\"></span>"));
				}
				else
				{
					if(XVar.Pack(iconDesc["fa"]))
					{
						iconType = new XVar("text");
						html = XVar.Clone(MVCFunctions.Concat("<span class=\"fa fa-", iconDesc["fa"], "\"></span>"));
					}
				}
			}
			if(XVar.Pack(html))
			{
				return MVCFunctions.Concat("<span data-icontype=\"", iconType, "\" class=\"r-panel-icon\">", html, "</span> ");
			}
			return "";
		}
		public static XVar getPdfChartObject(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic chart = XVar.Array(), chartParams = null;
			var_params.InitAndSetArrayItem(var_params["custom1"], "chartName");
			var_params.InitAndSetArrayItem(var_params["custom2"], "width");
			var_params.InitAndSetArrayItem(var_params["custom3"], "height");
			var_params.InitAndSetArrayItem(true, "pdMode");
			var_params.InitAndSetArrayItem(CommonFunctions.GetTableByShort((XVar)(var_params["chartName"])), "table");
			chartParams = XVar.Clone(CommonFunctions.getChartParams((XVar)(var_params)));
			chart = XVar.Clone(XVar.Array());
			chart.InitAndSetArrayItem(var_params["height"], "height");
			chart.InitAndSetArrayItem(var_params["width"], "width");
			chart.InitAndSetArrayItem(chartParams, "chartParams");
			MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(chart)));

			return null;
		}
		public static XVar getChartParams(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic chartParams = XVar.Array(), chartPreview = null, refresh = null, settings = null, showDetails = null;
			showDetails = XVar.Clone((XVar.Pack(var_params.KeyExists("showDetails")) ? XVar.Pack(var_params["showDetails"]) : XVar.Pack(true)));
			settings = XVar.Clone(new ProjectSettings((XVar)(CommonFunctions.GetTableByShort((XVar)(var_params["chartName"])))));
			refresh = XVar.Clone(settings.getChartRefreshTime());
			chartParams = XVar.Clone(XVar.Array());
			chartParams.InitAndSetArrayItem(showDetails, "showDetails");
			chartParams.InitAndSetArrayItem(var_params["chartName"], "chartName");
			chartParams.InitAndSetArrayItem(MVCFunctions.Concat("rnr", var_params["chartName"], var_params["id"]), "containerId");
			chartParams.InitAndSetArrayItem(var_params["ctype"], "chartType");
			chartParams.InitAndSetArrayItem(refresh, "refreshTime");
			chartParams.InitAndSetArrayItem(MVCFunctions.Concat(MVCFunctions.GetTableLink(new XVar("dchartdata")), "?chartname=", var_params["chartName"], chartPreview, "&ctype=", var_params["ctype"], "&showDetails=", showDetails, "&", var_params["stateLink"]), "xmlFile");
			chartParams.InitAndSetArrayItem(var_params["stateParams"], "stateParams");
			if((XVar)(var_params.KeyExists("dash"))  && (XVar)(var_params["dash"]))
			{
				chartParams["xmlFile"] = MVCFunctions.Concat(chartParams["xmlFile"], "&dashChart=", var_params["dash"]);
				chartParams.InitAndSetArrayItem(!(XVar)(!(XVar)(var_params["dash"])), "dashChart");
			}
			chartParams.InitAndSetArrayItem(var_params["id"], "pageId");
			if((XVar)(var_params.KeyExists("dashTName"))  && (XVar)(var_params["dashTName"]))
			{
				dynamic dashElement = XVar.Array(), dashSet = null;
				chartParams.InitAndSetArrayItem(var_params["dashTName"], "dashTName");
				chartParams.InitAndSetArrayItem(var_params["dashElementName"], "dashElementName");
				chartParams.InitAndSetArrayItem(var_params["id"], "pageId");
				chartParams["xmlFile"] = MVCFunctions.Concat(chartParams["xmlFile"], "&dashTName=", var_params["dashTName"]);
				chartParams["xmlFile"] = MVCFunctions.Concat(chartParams["xmlFile"], "&dashElName=", var_params["dashElementName"]);
				chartParams["xmlFile"] = MVCFunctions.Concat(chartParams["xmlFile"], "&pageId=", var_params["id"]);
				dashSet = XVar.Clone(new ProjectSettings((XVar)(var_params["dashTName"])));
				dashElement = XVar.Clone(dashSet.getDashboardElementData((XVar)(var_params["dashElementName"])));
				if(XVar.Pack(dashElement))
				{
					if(XVar.Pack(dashElement["reload"]))
					{
						chartParams.InitAndSetArrayItem(dashElement["reload"], "refreshTime");
					}
				}
			}
			if(XVar.Pack(var_params.KeyExists("refreshTime")))
			{
				chartParams.InitAndSetArrayItem(var_params["refreshTime"], "refreshTime");
			}
			if((XVar)(true)  || (XVar)(!(XVar)(var_params["singlePage"])))
			{
				chartParams.InitAndSetArrayItem(MVCFunctions.projectPath(), "webRootPath");
			}
			return chartParams;
		}
		public static XVar runner_basename(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic delimiters = XVar.Array();
			if(XVar.Pack(!(XVar)(path)))
			{
				return "";
			}
			delimiters = XVar.Clone(new XVar(0, "\\", 1, "/"));
			foreach (KeyValuePair<XVar, dynamic> d in delimiters.GetEnumerator())
			{
				dynamic idx = null, parts = XVar.Array();
				parts = XVar.Clone(MVCFunctions.explode((XVar)(d.Value), (XVar)(path)));
				idx = XVar.Clone(MVCFunctions.count(parts) - 1);
				path = XVar.Clone((XVar.Pack(XVar.Pack(0) <= idx) ? XVar.Pack(parts[idx]) : XVar.Pack(path)));
			}
			return path;
		}
		public static XVar getPdfFonts()
		{
			dynamic fonts = XVar.Array();
			if(XVar.Pack(!(XVar)(GlobalVars.runnerProjectSettings["fonts"])))
			{
				MVCFunctions.importFontSettings();
			}
			fonts = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> font in GlobalVars.runnerProjectSettings["fonts"].GetEnumerator())
			{
				if((XVar)(font.Value["pdf"])  && (XVar)(font.Value["type"] == 0))
				{
					fonts.InitAndSetArrayItem(font.Value, null);
				}
			}
			return fonts;
		}
		public static XVar splitFirstLine(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic crPos = null, crlfPos = null;
			crPos = XVar.Clone(MVCFunctions.strpos((XVar)(str), new XVar("\n")));
			crlfPos = XVar.Clone(MVCFunctions.strpos((XVar)(str), new XVar("\r\n")));
			if((XVar)(!XVar.Equals(XVar.Pack(crlfPos), XVar.Pack(false)))  && (XVar)(crlfPos <= crPos))
			{
				return new XVar(0, MVCFunctions.substr((XVar)(str), new XVar(0), (XVar)(crlfPos)), 1, MVCFunctions.substr((XVar)(str), (XVar)(crlfPos + 2)));
			}
			if(!XVar.Equals(XVar.Pack(crPos), XVar.Pack(false)))
			{
				return new XVar(0, MVCFunctions.substr((XVar)(str), new XVar(0), (XVar)(crPos)), 1, MVCFunctions.substr((XVar)(str), (XVar)(crPos + 1)));
			}
			return new XVar(0, "", 1, str);
		}
		public static XVar parseRestDate(dynamic _param_datestring, dynamic _param_format)
		{
			#region pass-by-value parameters
			dynamic datestring = XVar.Clone(_param_datestring);
			dynamic format = XVar.Clone(_param_format);
			#endregion

			dynamic dateParts = XVar.Array(), formatPositions = XVar.Array(), formatStrings = XVar.Array(), numbers = XVar.Array();
			if(XVar.Pack(!(XVar)(datestring)))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(format)))
			{
				return datestring;
			}
			numbers = XVar.Clone(CommonFunctions.parsenumbers((XVar)(datestring)));
			if(XVar.Pack(!(XVar)(numbers)))
			{
				return datestring;
			}
			formatStrings = XVar.Clone(new XVar(0, "YY", 1, "MM", 2, "DD", 3, "hh", 4, "mm", 5, "ss"));
			formatPositions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in formatStrings.GetEnumerator())
			{
				dynamic pos = null;
				pos = XVar.Clone(MVCFunctions.strpos((XVar)(format), (XVar)(f.Value)));
				if(!XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))
				{
					formatPositions.InitAndSetArrayItem(new XVar(0, f.Value, 1, pos), null);
				}
			}
			MVCFunctions.usort((XVar)(formatPositions), new XVar("sortDateFormatChunks"));
			dateParts = XVar.Clone(new XVar(0, 0, 1, 1, 2, 1, 3, 0, 4, 0, 5, 0));
			foreach (KeyValuePair<XVar, dynamic> n in numbers.GetEnumerator())
			{
				dynamic f = XVar.Array(), partIdx = null;
				if(MVCFunctions.count(formatPositions) <= n.Key)
				{
					break;
				}
				f = XVar.Clone(formatPositions[n.Key]);
				partIdx = XVar.Clone(MVCFunctions.array_search((XVar)(f[0]), (XVar)(formatStrings)));
				dateParts.InitAndSetArrayItem(n.Value, partIdx);
			}
			if(XVar.Pack(!(XVar)(dateParts[0])))
			{
				return CommonFunctions.format_datetime_custom((XVar)(dateParts), new XVar("HH:mm:ss"));
			}
			else
			{
				return CommonFunctions.format_datetime_custom((XVar)(dateParts), new XVar("yyyy-MM-dd HH:mm:ss"));
			}

			return null;
		}
		public static XVar sortDateFormatChunks(dynamic _param_v1, dynamic _param_v2)
		{
			#region pass-by-value parameters
			dynamic v1 = XVar.Clone(_param_v1);
			dynamic v2 = XVar.Clone(_param_v2);
			#endregion

			return v1[1] - v2[1];
		}
		public static XVar getFormatSettings(dynamic _param_viewFomat, dynamic pSet_packed, dynamic _param_fName)
		{
			#region packeted values
			ProjectSettings pSet = XVar.UnPackProjectSettings(pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic viewFomat = XVar.Clone(_param_viewFomat);
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic formatSettings = XVar.Array();
			formatSettings = XVar.Clone(XVar.Array());
			if(XVar.Equals(XVar.Pack(viewFomat), XVar.Pack(Constants.FORMAT_CURRENCY)))
			{
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_ICURRDIGITS"], "decimalDigits");
				formatSettings.InitAndSetArrayItem(MVCFunctions.explode(new XVar(";"), (XVar)(GlobalVars.locale_info["LOCALE_SMONGROUPING"])), "grouping");
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_SMONTHOUSANDSEP"], "thousandSep");
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_SMONDECIMALSEP"], "decimalSep");
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_ICURRENCY"], "LOCALE_ICURRENCY");
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_INEGCURR"], "LOCALE_INEGCURR");
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_SCURRENCY"], "LOCALE_SCURRENCY");
			}
			if(viewFomat == Constants.FORMAT_NUMBER)
			{
				formatSettings.InitAndSetArrayItem(pSet.isDecimalDigits((XVar)(fName)), "decimalDigits");
				formatSettings.InitAndSetArrayItem(MVCFunctions.explode(new XVar(";"), (XVar)(GlobalVars.locale_info["LOCALE_SGROUPING"])), "grouping");
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_STHOUSAND"], "thousandSep");
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_SDECIMAL"], "decimalSep");
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_SPOSITIVESIGN"], "LOCALE_SPOSITIVESIGN");
				formatSettings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_INEGNUMBER"], "LOCALE_INEGNUMBER");
			}
			return formatSettings;
		}
		public static XVar mlang_message(dynamic _param_tag)
		{
			#region pass-by-value parameters
			dynamic tag = XVar.Clone(_param_tag);
			#endregion

			return GlobalVars.runnerLangMessages[CommonFunctions.mlang_getcurrentlang()]["messages"][tag];
		}
		public static XVar loadLanguage(dynamic _param_lang)
		{
			#region pass-by-value parameters
			dynamic lang = XVar.Clone(_param_lang);
			#endregion

			MVCFunctions.loadLanguageMessages((XVar)(CommonFunctions.getLangFileName((XVar)(lang))));

			return null;
		}
		public static XVar GetTableLink(dynamic _param_shortTable, dynamic _param_pageType = null, dynamic _param_getParams = null)
		{
			#region default values
			if(_param_pageType as Object == null) _param_pageType = new XVar("");
			if(_param_getParams as Object == null) _param_getParams = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic shortTable = XVar.Clone(_param_shortTable);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic getParams = XVar.Clone(_param_getParams);
			#endregion

			dynamic ext = null, url = null;
			ext = XVar.Clone(ProjectSettings.getProjectValue(new XVar("ext")));
			if((XVar)(ext == "aspx")  && (XVar)(shortTable == "assetmanager"))
			{
				return "assetmanager.aspx";
			}
			if((XVar)(XVar.Equals(XVar.Pack(shortTable), XVar.Pack(Constants.GLOBAL_PAGES_SHORT)))  || (XVar)(shortTable == XVar.Pack("")))
			{
				url = XVar.Clone(pageType);
			}
			else
			{
				if(pageType == XVar.Pack(""))
				{
					url = XVar.Clone(shortTable);
				}
				else
				{
					if(ext == "php")
					{
						url = XVar.Clone(MVCFunctions.Concat(shortTable, "_", pageType));
					}
					else
					{
						url = XVar.Clone(MVCFunctions.Concat(shortTable, "/", pageType));
					}
				}
			}
			if(ext == "php")
			{
				url = MVCFunctions.Concat(url, ".php");
			}
			if(getParams != XVar.Pack(""))
			{
				url = MVCFunctions.Concat(url, "?", getParams);
			}
			return url;
		}
		public static XVar verifyAjaxSnippet(dynamic _param_eventId, dynamic _param_field, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic eventId = XVar.Clone(_param_eventId);
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic editEvent = null, fields = null, inlieAddFields = null, inlieEditFields = null, pageType = null, searchFields = null;
			pageType = XVar.Clone(pSet.getPageType());
			if(XVar.Equals(XVar.Pack(pageType), XVar.Pack("list")))
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(eventId), (XVar)(pSet.clickHandlerSnippets()))), XVar.Pack(false)))
				{
					return true;
				}
			}
			fields = XVar.Clone(pSet.getPageFields());
			searchFields = XVar.Clone((XVar.Pack(XVar.Equals(XVar.Pack(pageType), XVar.Pack("list"))) ? XVar.Pack(pSet.getSearchableFields()) : XVar.Pack(XVar.Array())));
			if((XVar)(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(fields))), XVar.Pack(false)))  && (XVar)(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(searchFields))), XVar.Pack(false))))
			{
				return false;
			}
			editEvent = XVar.Clone((XVar)((XVar)((XVar)((XVar)((XVar)(XVar.Equals(XVar.Pack(pageType), XVar.Pack("edit")))  || (XVar)(XVar.Equals(XVar.Pack(pageType), XVar.Pack("add"))))  || (XVar)(XVar.Equals(XVar.Pack(pageType), XVar.Pack("search"))))  || (XVar)(XVar.Equals(XVar.Pack(pageType), XVar.Pack("register"))))  || (XVar)(XVar.Equals(XVar.Pack(pageType), XVar.Pack("login"))))  || (XVar)(XVar.Equals(XVar.Pack(pageType), XVar.Pack("userinfo"))));
			if(XVar.Pack(pSet.fieldHasEvent((XVar)(eventId), (XVar)(field), (XVar)(editEvent))))
			{
				return true;
			}
			inlieAddFields = XVar.Clone(pSet.getInlineAddFields());
			inlieEditFields = XVar.Clone(pSet.getInlineEditFields());
			if((XVar)(pageType == "list")  && (XVar)((XVar)((XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(inlieEditFields))), XVar.Pack(false)))  || (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(inlieAddFields))), XVar.Pack(false))))  || (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(searchFields))), XVar.Pack(false)))))
			{
				if(XVar.Pack(pSet.fieldHasEvent((XVar)(eventId), (XVar)(field), new XVar(true))))
				{
					return true;
				}
			}
			return false;
		}
		public static XVar runnerConnectionIdByName(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> db in GlobalVars.runnerDatabases.GetEnumerator())
			{
				if(db.Value["connName"] == name)
				{
					return db.Value["connId"];
				}
			}
			return "";
		}
		public static XVar runnerGetRestConnectionInfo(dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			#endregion

			return GlobalVars.runnerRestConnections[id];
		}
		public static XVar runnerRestConnectionIdByName(dynamic _param_name = null)
		{
			#region default values
			if(_param_name as Object == null) _param_name = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> rest in GlobalVars.runnerRestConnections.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(name)))
				{
					return rest.Value["id"];
				}
				if(rest.Value["name"] == name)
				{
					return rest.Value["id"];
				}
			}
			return "";
		}
		public static XVar GetWebRootPath()
		{
			return "";
		}
		public static XVar my_json_decode(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.runner_json_decode((XVar)(str));
		}
		public static XVar my_json_encode(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.runner_json_encode((XVar)(str));
		}
		public static XVar menuExists(dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic availableMenus = null;
			availableMenus = XVar.Clone(ProjectSettings.getProjectValue(new XVar("menuIds")));
			return !XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(id), (XVar)(availableMenus))), XVar.Pack(false));
		}
		public static XVar GetTemplateName(dynamic _param_table, dynamic _param_templateName)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic templateName = XVar.Clone(_param_templateName);
			#endregion

			if(templateName == XVar.Pack(""))
			{
				return MVCFunctions.Concat(Constants.GLOBAL_PAGES_SHORT, "_", table, ".htm");
			}
			if(table == XVar.Pack(""))
			{
				return MVCFunctions.Concat(templateName, ".htm");
			}
			return MVCFunctions.Concat(table, "_", templateName, ".htm");
		}
		public static XVar isAdminPage(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			return (XVar)((XVar)(pageType == Constants.PAGE_ADMIN_RIGHTS)  || (XVar)(pageType == Constants.PAGE_ADMIN_MEMBERS))  || (XVar)(pageType == Constants.PAGE_ADMIN_ADMEMBERS);
		}
		public static XVar fgColorBlack(dynamic _param_bgColor)
		{
			#region pass-by-value parameters
			dynamic bgColor = XVar.Clone(_param_bgColor);
			#endregion

			dynamic blue = null, color = null, green = null, red = null;
			color = XVar.Clone(CommonFunctions.hexdec((XVar)(MVCFunctions.str_pad((XVar)(bgColor), new XVar(6), new XVar("0"), new XVar(Constants.STR_PAD_RIGHT)))));
			red = XVar.Clone((XVar.BitwiseAnd(XVar.Pack(color), XVar.Pack(16711680))) >> 16);
			green = XVar.Clone((XVar.BitwiseAnd(XVar.Pack(color), XVar.Pack(65280))) >> 8);
			blue = XVar.Clone(XVar.BitwiseAnd(XVar.Pack(color), XVar.Pack(255)));
			return 186 < (red * 0.299000 + green * 0.587000) + blue * 0.114000;
		}
		public static XVar currentLocale()
		{
			dynamic langData = XVar.Array();
			langData = XVar.Clone(GlobalVars.languages_data[CommonFunctions.mlang_getcurrentlang()]);
			if((XVar)(!(XVar)(langData))  || (XVar)(!(XVar)(langData["languageCode"])))
			{
				return GlobalVars.locale_info["LOCALE_SNAME"];
			}
			foreach (KeyValuePair<XVar, dynamic> lPart in MVCFunctions.explode(new XVar("-"), (XVar)(langData["languageCode"])).GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(MVCFunctions.strtoupper((XVar)(lPart.Value))), XVar.Pack(lPart.Value)))
				{
					return langData["languageCode"];
				}
			}
			return MVCFunctions.Concat(langData["languageCode"], "-", GlobalVars.locale_info["LOCALE_SISO3166CTRYNAME"]);
		}
	}
}
