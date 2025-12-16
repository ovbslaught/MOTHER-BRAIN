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
	public partial class Labels : XClass
	{
		public static XVar getLanguages()
		{
			return ProjectSettings.languages();
		}
		private static XVar findLanguage(dynamic _param_lng)
		{
			#region pass-by-value parameters
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			dynamic languages = XVar.Array();
			languages = XVar.Clone(Labels.getLanguages());
			if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(lng), (XVar)(languages))), XVar.Pack(false)))
			{
				return lng;
			}
			lng = XVar.Clone(MVCFunctions.strtoupper((XVar)(lng)));
			foreach (KeyValuePair<XVar, dynamic> l in languages.GetEnumerator())
			{
				if(MVCFunctions.strtoupper((XVar)(l.Value)) == lng)
				{
					return l.Value;
				}
			}
			return CommonFunctions.mlang_getcurrentlang();
		}
		private static XVar findTable(dynamic _param_table, dynamic _param_strict = null)
		{
			#region default values
			if(_param_strict as Object == null) _param_strict = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic strict = XVar.Clone(_param_strict);
			#endregion

			table = XVar.Clone(CommonFunctions.findTable((XVar)(table), (XVar)(strict)));
			if(XVar.Pack(!(XVar)(table)))
			{
				return "";
			}
			return table;
		}
		protected static XVar setFieldString(dynamic _param_key, dynamic _param_table, dynamic _param_field, dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic ps = null;
			table = XVar.Clone(Labels.findTable((XVar)(table)));
			if(table == XVar.Pack(""))
			{
				return false;
			}
			ps = XVar.Clone(new ProjectSettings((XVar)(table)));
			field = XVar.Clone(ps.findField((XVar)(field)));
			if(field == XVar.Pack(""))
			{
				return false;
			}
			GlobalVars.runnerTableLabels.InitAndSetArrayItem(str, table, key, MVCFunctions.GoodFieldName((XVar)(field)));
			return true;
		}
		protected static XVar getFieldString(dynamic _param_key, dynamic _param_table, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic ps = null;
			table = XVar.Clone(Labels.findTable((XVar)(table)));
			if(table == XVar.Pack(""))
			{
				return "";
			}
			ps = XVar.Clone(new ProjectSettings((XVar)(table)));
			field = XVar.Clone(ps.findField((XVar)(field)));
			if(field == XVar.Pack(""))
			{
				return "";
			}
			return GlobalVars.runnerTableLabels[table][key][MVCFunctions.GoodFieldName((XVar)(field))];
		}
		public static XVar getFieldLabel(dynamic _param_table, dynamic _param_field, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			if((XVar)(lng)  && (XVar)(!XVar.Equals(XVar.Pack(lng), XVar.Pack(CommonFunctions.mlang_getcurrentlang()))))
			{
				return "";
			}
			return Labels.getFieldString(new XVar("fieldLabels"), (XVar)(table), (XVar)(field));
		}
		public static XVar setFieldLabel(dynamic _param_table, dynamic _param_field, dynamic _param_str, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic str = XVar.Clone(_param_str);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			if((XVar)(lng)  && (XVar)(!XVar.Equals(XVar.Pack(lng), XVar.Pack(CommonFunctions.mlang_getcurrentlang()))))
			{
				return false;
			}
			return Labels.setFieldString(new XVar("fieldLabels"), (XVar)(table), (XVar)(field), (XVar)(str));
		}
		public static XVar getTableCaption(dynamic _param_table, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			dynamic strings = null;
			table = XVar.Clone(Labels.findTable((XVar)(table), new XVar(true)));
			if(table == XVar.Pack(""))
			{
				return "";
			}
			lng = XVar.Clone(Labels.findLanguage((XVar)(lng)));
			strings = XVar.Clone(ProjectSettings.getProjectValue(new XVar("allTables"), (XVar)(table), new XVar("caption")));
			if(XVar.Pack(!(XVar)(strings)))
			{
				return table;
			}
			return Labels.getLanguageValue(ref strings, (XVar)(lng));
		}
		public static XVar setTableCaption(dynamic _param_table, dynamic _param_str, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic str = XVar.Clone(_param_str);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			table = XVar.Clone(Labels.findTable((XVar)(table)));
			if(table == XVar.Pack(""))
			{
				return false;
			}
			lng = XVar.Clone(Labels.findLanguage((XVar)(lng)));
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(str, "allTables", table, "caption", lng);
			return true;
		}
		public static XVar getProjectLogo(dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			lng = XVar.Clone(Labels.findLanguage((XVar)(lng)));
			return GlobalVars.runnerLangMessages[lng]["projectLogo"];
		}
		public static XVar setProjectLogo(dynamic _param_str, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			lng = XVar.Clone(Labels.findLanguage((XVar)(lng)));
			if(XVar.Pack(!(XVar)(GlobalVars.runnerLangMessages[lng])))
			{
				GlobalVars.runnerLangMessages.InitAndSetArrayItem(XVar.Array(), lng);
			}
			GlobalVars.runnerLangMessages.InitAndSetArrayItem(str, lng, "projectLogo");
			return true;
		}
		public static XVar getCookieBanner(dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			dynamic banner = null;
			lng = XVar.Clone(Labels.findLanguage((XVar)(lng)));
			banner = XVar.Clone(GlobalVars.runnerLangMessages[lng]["cookieBannerText"]);
			return (XVar.Pack(banner) ? XVar.Pack(banner) : XVar.Pack(GlobalVars.runnerLangMessages[lng]["messages"]["COOKIE_BANNER"]));
		}
		public static XVar setCookieBanner(dynamic _param_str, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			lng = XVar.Clone(Labels.findLanguage((XVar)(lng)));
			if(XVar.Pack(!(XVar)(GlobalVars.runnerLangMessages[lng])))
			{
				GlobalVars.runnerLangMessages.InitAndSetArrayItem(XVar.Array(), lng);
			}
			GlobalVars.runnerLangMessages.InitAndSetArrayItem(str, lng, "cookieBannerText");
			return true;
		}
		public static XVar setFieldTooltip(dynamic _param_table, dynamic _param_field, dynamic _param_str, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic str = XVar.Clone(_param_str);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			if((XVar)(lng)  && (XVar)(!XVar.Equals(XVar.Pack(lng), XVar.Pack(CommonFunctions.mlang_getcurrentlang()))))
			{
				return "";
			}
			return Labels.setFieldString(new XVar("fieldTooltips"), (XVar)(table), (XVar)(field), (XVar)(str));
		}
		public static XVar getFieldTooltip(dynamic _param_table, dynamic _param_field, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			if((XVar)(lng)  && (XVar)(!XVar.Equals(XVar.Pack(lng), XVar.Pack(CommonFunctions.mlang_getcurrentlang()))))
			{
				return "";
			}
			return Labels.getFieldString(new XVar("fieldTooltips"), (XVar)(table), (XVar)(field));
		}
		public static XVar setPageTitleTempl(dynamic _param_table, dynamic _param_page, dynamic _param_str, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic page = XVar.Clone(_param_page);
			dynamic str = XVar.Clone(_param_str);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			table = XVar.Clone(Labels.findTable((XVar)(table)));
			if(table == XVar.Pack(""))
			{
				return false;
			}
			GlobalVars.runnerTableLabels.InitAndSetArrayItem(page, table, "pageTitles", page);
			return true;
		}
		public static XVar getPageTitleTempl(dynamic _param_table, dynamic _param_page, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic page = XVar.Clone(_param_page);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			dynamic ps = null;
			table = XVar.Clone(Labels.findTable((XVar)(table)));
			if(table == XVar.Pack(""))
			{
				return "";
			}
			if(XVar.Pack(GlobalVars.runnerTableLabels[table]["pageTitles"].KeyExists(page)))
			{
				return GlobalVars.runnerTableLabels[table]["pageTitles"][page];
			}
			ps = XVar.Clone(new ProjectSettings((XVar)(table), new XVar(""), (XVar)(page)));
			return RunnerPage.getDefaultPageTitle((XVar)(ps.getPageType()), (XVar)(table), (XVar)(ps));
		}
		public static XVar setBreadcrumbsLabelTempl(dynamic _param_table, dynamic _param_str, dynamic _param_master = null, dynamic _param_page = null, dynamic _param_lng = null)
		{
			#region default values
			if(_param_master as Object == null) _param_master = new XVar("");
			if(_param_page as Object == null) _param_page = new XVar("");
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic str = XVar.Clone(_param_str);
			dynamic master = XVar.Clone(_param_master);
			dynamic page = XVar.Clone(_param_page);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			table = XVar.Clone(Labels.findTable((XVar)(table)));
			if(XVar.Pack(!(XVar)(table)))
			{
				table = new XVar(".");
			}
			master = XVar.Clone(CommonFunctions.findTable((XVar)(master)));
			if(XVar.Pack(!(XVar)(master)))
			{
				master = new XVar(".");
			}
			lng = XVar.Clone(Labels.findLanguage((XVar)(lng)));
			if(page == XVar.Pack(""))
			{
				page = new XVar(".");
			}
			if(XVar.Pack(!(XVar)(GlobalVars.breadcrumb_labels.KeyExists(lng))))
			{
				GlobalVars.breadcrumb_labels.InitAndSetArrayItem(XVar.Array(), lng);
			}
			if(XVar.Pack(!(XVar)(GlobalVars.breadcrumb_labels[lng].KeyExists(table))))
			{
				GlobalVars.breadcrumb_labels.InitAndSetArrayItem(XVar.Array(), lng, table);
			}
			if(XVar.Pack(!(XVar)(GlobalVars.breadcrumb_labels[lng][table].KeyExists(master))))
			{
				GlobalVars.breadcrumb_labels.InitAndSetArrayItem(XVar.Array(), lng, table, master);
			}
			GlobalVars.breadcrumb_labels.InitAndSetArrayItem(str, lng, table, master, page);

			return null;
		}
		public static XVar getBreadcrumbsLabelTempl(dynamic _param_table, dynamic _param_master = null, dynamic _param_page = null, dynamic _param_lng = null)
		{
			#region default values
			if(_param_master as Object == null) _param_master = new XVar("");
			if(_param_page as Object == null) _param_page = new XVar("");
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic master = XVar.Clone(_param_master);
			dynamic page = XVar.Clone(_param_page);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			table = XVar.Clone(Labels.findTable((XVar)(table)));
			if(XVar.Pack(!(XVar)(table)))
			{
				table = new XVar(".");
			}
			master = XVar.Clone(CommonFunctions.findTable((XVar)(master)));
			if(XVar.Pack(!(XVar)(master)))
			{
				master = new XVar(".");
			}
			lng = XVar.Clone(Labels.findLanguage((XVar)(lng)));
			if(page == XVar.Pack(""))
			{
				page = new XVar(".");
			}
			if(XVar.Pack(!(XVar)(GlobalVars.breadcrumb_labels.KeyExists(lng))))
			{
				return "";
			}
			if(XVar.Pack(!(XVar)(GlobalVars.breadcrumb_labels[lng].KeyExists(table))))
			{
				return "";
			}
			if(XVar.Pack(!(XVar)(GlobalVars.breadcrumb_labels[lng][table].KeyExists(master))))
			{
				return "";
			}
			return GlobalVars.breadcrumb_labels[lng][table][master][page];
		}
		public static XVar getPlaceholder(dynamic _param_table, dynamic _param_field, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			if((XVar)(lng)  && (XVar)(!XVar.Equals(XVar.Pack(lng), XVar.Pack(CommonFunctions.mlang_getcurrentlang()))))
			{
				return "";
			}
			return Labels.getFieldString(new XVar("fieldPlaceholders"), (XVar)(table), (XVar)(field));
		}
		public static XVar setPlaceholder(dynamic _param_table, dynamic _param_field, dynamic _param_placeHolder, dynamic _param_lng = null)
		{
			#region default values
			if(_param_lng as Object == null) _param_lng = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic placeHolder = XVar.Clone(_param_placeHolder);
			dynamic lng = XVar.Clone(_param_lng);
			#endregion

			dynamic str = null;
			if((XVar)(lng)  && (XVar)(!XVar.Equals(XVar.Pack(lng), XVar.Pack(CommonFunctions.mlang_getcurrentlang()))))
			{
				return "";
			}
			return Labels.setFieldString(new XVar("fieldPlaceholders"), (XVar)(table), (XVar)(field), (XVar)(str));
		}
		public static XVar multilangString(dynamic _param_mlString)
		{
			#region pass-by-value parameters
			dynamic mlString = XVar.Clone(_param_mlString);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(mlString)))))
			{
				return "";
			}
			switch(((XVar)mlString["type"]).ToInt())
			{
				case Constants.mlTypeText:
					return mlString["text"];
				case Constants.mlTypeCustomLabel:
					return CommonFunctions.GetCustomLabel((XVar)(mlString["label"]));
			}
			return "";
		}
		protected static XVar getLanguageValue(ref dynamic strings, dynamic _param_lang)
		{
			#region pass-by-value parameters
			dynamic lang = XVar.Clone(_param_lang);
			#endregion

			dynamic defaultLang = null;
			if(XVar.Pack(!(XVar)(strings)))
			{
				return "";
			}
			if(XVar.Pack(strings.KeyExists(lang)))
			{
				return strings[lang];
			}
			defaultLang = XVar.Clone(ProjectSettings.getProjectValue(new XVar("defaultLanguage")));
			if(XVar.Pack(strings.KeyExists(defaultLang)))
			{
				return strings[defaultLang];
			}
			foreach (KeyValuePair<XVar, dynamic> str in strings.GetEnumerator())
			{
				return str.Value;
			}
			return "";
		}
	}
}
