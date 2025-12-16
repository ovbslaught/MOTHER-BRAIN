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
	public static partial class GlobalVars
	{
		public static dynamic _cachedSeachClauses
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["_cachedSeachClauses"];
			}
			set
			{
				HttpContext.Current.Items["_cachedSeachClauses"] = value;
			}
		}
		public static dynamic _eventClasses
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["_eventClasses"];
			}
			set
			{
				HttpContext.Current.Items["_eventClasses"] = value;
			}
		}
		public static dynamic _gmdays
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["_gmdays"];
			}
			set
			{
				HttpContext.Current.Items["_gmdays"] = value;
			}
		}
		public static dynamic _pagetypeToPermissions_dict
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["_pagetypeToPermissions_dict"];
			}
			set
			{
				HttpContext.Current.Items["_pagetypeToPermissions_dict"] = value;
			}
		}
		public static dynamic adNestedPermissions
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["adNestedPermissions"];
			}
			set
			{
				HttpContext.Current.Items["adNestedPermissions"] = value;
			}
		}
		public static dynamic ajaxSearchStartsWith
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["ajaxSearchStartsWith"];
			}
			set
			{
				HttpContext.Current.Items["ajaxSearchStartsWith"] = value;
			}
		}
		public static dynamic all_page_layouts
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["all_page_layouts"];
			}
			set
			{
				HttpContext.Current.Items["all_page_layouts"] = value;
			}
		}
		public static dynamic arrCustomPages
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["arrCustomPages"];
			}
			set
			{
				HttpContext.Current.Items["arrCustomPages"] = value;
			}
		}
		public static dynamic auditMaxFieldLength
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["auditMaxFieldLength"];
			}
			set
			{
				HttpContext.Current.Items["auditMaxFieldLength"] = value;
			}
		}
		public static dynamic bSubqueriesSupported
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["bSubqueriesSupported"];
			}
			set
			{
				HttpContext.Current.Items["bSubqueriesSupported"] = value;
			}
		}
		public static dynamic breadcrumb_labels
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["breadcrumb_labels"];
			}
			set
			{
				HttpContext.Current.Items["breadcrumb_labels"] = value;
			}
		}
		public static dynamic cCharset
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cCharset"];
			}
			set
			{
				HttpContext.Current.Items["cCharset"] = value;
			}
		}
		public static dynamic cCodePage
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cCodePage"];
			}
			set
			{
				HttpContext.Current.Items["cCodePage"] = value;
			}
		}
		public static dynamic cDisplayNameField
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cDisplayNameField"];
			}
			set
			{
				HttpContext.Current.Items["cDisplayNameField"] = value;
			}
		}
		public static dynamic cLoginTable
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cLoginTable"];
			}
			set
			{
				HttpContext.Current.Items["cLoginTable"] = value;
			}
		}
		public static dynamic cPasswordField
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cPasswordField"];
			}
			set
			{
				HttpContext.Current.Items["cPasswordField"] = value;
			}
		}
		public static dynamic cUserGroupField
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cUserGroupField"];
			}
			set
			{
				HttpContext.Current.Items["cUserGroupField"] = value;
			}
		}
		public static dynamic cUserNameField
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cUserNameField"];
			}
			set
			{
				HttpContext.Current.Items["cUserNameField"] = value;
			}
		}
		public static dynamic cUserpicField
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cUserpicField"];
			}
			set
			{
				HttpContext.Current.Items["cUserpicField"] = value;
			}
		}
		public static dynamic cacheImages
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cacheImages"];
			}
			set
			{
				HttpContext.Current.Items["cacheImages"] = value;
			}
		}
		public static dynamic cache_db2time
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cache_db2time"];
			}
			set
			{
				HttpContext.Current.Items["cache_db2time"] = value;
			}
		}
		public static dynamic cache_formatweekstart
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cache_formatweekstart"];
			}
			set
			{
				HttpContext.Current.Items["cache_formatweekstart"] = value;
			}
		}
		public static dynamic cache_getdayofweek
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cache_getdayofweek"];
			}
			set
			{
				HttpContext.Current.Items["cache_getdayofweek"] = value;
			}
		}
		public static dynamic cache_getweekstart
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cache_getweekstart"];
			}
			set
			{
				HttpContext.Current.Items["cache_getweekstart"] = value;
			}
		}
		public static dynamic cipherer
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cipherer"];
			}
			set
			{
				HttpContext.Current.Items["cipherer"] = value;
			}
		}
		public static dynamic cman
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["cman"];
			}
			set
			{
				HttpContext.Current.Items["cman"] = value;
			}
		}
		public static dynamic conn
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["conn"];
			}
			set
			{
				HttpContext.Current.Items["conn"] = value;
			}
		}
		public static dynamic contextStack
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["contextStack"];
			}
			set
			{
				HttpContext.Current.Items["contextStack"] = value;
			}
		}
		public static dynamic csrfProtectionOff
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["csrfProtectionOff"];
			}
			set
			{
				HttpContext.Current.Items["csrfProtectionOff"] = value;
			}
		}
		public static dynamic currentConnection
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["currentConnection"];
			}
			set
			{
				HttpContext.Current.Items["currentConnection"] = value;
			}
		}
		public static dynamic customLDAPSettings
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["customLDAPSettings"];
			}
			set
			{
				HttpContext.Current.Items["customLDAPSettings"] = value;
			}
		}
		public static dynamic dDebug
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["dDebug"];
			}
			set
			{
				HttpContext.Current.Items["dDebug"] = value;
			}
		}
		public static dynamic dal_info
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["dal_info"];
			}
			set
			{
				HttpContext.Current.Items["dal_info"] = value;
			}
		}
		public static dynamic debug2Factor
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["debug2Factor"];
			}
			set
			{
				HttpContext.Current.Items["debug2Factor"] = value;
			}
		}
		public static dynamic doMySQLCountBugWorkaround
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["doMySQLCountBugWorkaround"];
			}
			set
			{
				HttpContext.Current.Items["doMySQLCountBugWorkaround"] = value;
			}
		}
		public static dynamic dummyEvents
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["dummyEvents"];
			}
			set
			{
				HttpContext.Current.Items["dummyEvents"] = value;
			}
		}
		public static dynamic fieldFilterDefaultValue
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["fieldFilterDefaultValue"];
			}
			set
			{
				HttpContext.Current.Items["fieldFilterDefaultValue"] = value;
			}
		}
		public static dynamic fieldFilterMaxDisplayValueLength
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["fieldFilterMaxDisplayValueLength"];
			}
			set
			{
				HttpContext.Current.Items["fieldFilterMaxDisplayValueLength"] = value;
			}
		}
		public static dynamic fieldFilterMaxSearchValueLength
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["fieldFilterMaxSearchValueLength"];
			}
			set
			{
				HttpContext.Current.Items["fieldFilterMaxSearchValueLength"] = value;
			}
		}
		public static dynamic fieldFilterMaxValuesCount
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["fieldFilterMaxValuesCount"];
			}
			set
			{
				HttpContext.Current.Items["fieldFilterMaxValuesCount"] = value;
			}
		}
		public static dynamic fieldFilterValueShrinkPostfix
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["fieldFilterValueShrinkPostfix"];
			}
			set
			{
				HttpContext.Current.Items["fieldFilterValueShrinkPostfix"] = value;
			}
		}
		public static dynamic gGuestHasPermissions
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["gGuestHasPermissions"];
			}
			set
			{
				HttpContext.Current.Items["gGuestHasPermissions"] = value;
			}
		}
		public static dynamic gLoadSearchControls
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["gLoadSearchControls"];
			}
			set
			{
				HttpContext.Current.Items["gLoadSearchControls"] = value;
			}
		}
		public static dynamic gPermissionsRead
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["gPermissionsRead"];
			}
			set
			{
				HttpContext.Current.Items["gPermissionsRead"] = value;
			}
		}
		public static dynamic gPermissionsRefreshTime
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["gPermissionsRefreshTime"];
			}
			set
			{
				HttpContext.Current.Items["gPermissionsRefreshTime"] = value;
			}
		}
		public static dynamic gReadPermissions
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["gReadPermissions"];
			}
			set
			{
				HttpContext.Current.Items["gReadPermissions"] = value;
			}
		}
		public static dynamic globalEvents
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["globalEvents"];
			}
			set
			{
				HttpContext.Current.Items["globalEvents"] = value;
			}
		}
		public static dynamic group_sort_y
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["group_sort_y"];
			}
			set
			{
				HttpContext.Current.Items["group_sort_y"] = value;
			}
		}
		public static dynamic jsonDataFromRequest
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["jsonDataFromRequest"];
			}
			set
			{
				HttpContext.Current.Items["jsonDataFromRequest"] = value;
			}
		}
		public static dynamic languages_data
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["languages_data"];
			}
			set
			{
				HttpContext.Current.Items["languages_data"] = value;
			}
		}
		public static dynamic locale_info
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["locale_info"];
			}
			set
			{
				HttpContext.Current.Items["locale_info"] = value;
			}
		}
		public static dynamic loginKeyFields
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["loginKeyFields"];
			}
			set
			{
				HttpContext.Current.Items["loginKeyFields"] = value;
			}
		}
		public static dynamic logoutPerformed
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["logoutPerformed"];
			}
			set
			{
				HttpContext.Current.Items["logoutPerformed"] = value;
			}
		}
		public static dynamic lookupTableLinks
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["lookupTableLinks"];
			}
			set
			{
				HttpContext.Current.Items["lookupTableLinks"] = value;
			}
		}
		public static dynamic mbEnabled
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["mbEnabled"];
			}
			set
			{
				HttpContext.Current.Items["mbEnabled"] = value;
			}
		}
		public static dynamic mediaType
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["mediaType"];
			}
			set
			{
				HttpContext.Current.Items["mediaType"] = value;
			}
		}
		public static dynamic menuCache
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["menuCache"];
			}
			set
			{
				HttpContext.Current.Items["menuCache"] = value;
			}
		}
		public static dynamic menuNodesObject
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["menuNodesObject"];
			}
			set
			{
				HttpContext.Current.Items["menuNodesObject"] = value;
			}
		}
		public static dynamic mlang_defaultlang
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["mlang_defaultlang"];
			}
			set
			{
				HttpContext.Current.Items["mlang_defaultlang"] = value;
			}
		}
		public static dynamic mlang_messages
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["mlang_messages"];
			}
			set
			{
				HttpContext.Current.Items["mlang_messages"] = value;
			}
		}
		public static dynamic mysqlSupportDates0000
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["mysqlSupportDates0000"];
			}
			set
			{
				HttpContext.Current.Items["mysqlSupportDates0000"] = value;
			}
		}
		public static dynamic onDemnadVariables
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["onDemnadVariables"];
			}
			set
			{
				HttpContext.Current.Items["onDemnadVariables"] = value;
			}
		}
		public static dynamic pageTypesForEdit
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["pageTypesForEdit"];
			}
			set
			{
				HttpContext.Current.Items["pageTypesForEdit"] = value;
			}
		}
		public static dynamic pageTypesForView
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["pageTypesForView"];
			}
			set
			{
				HttpContext.Current.Items["pageTypesForView"] = value;
			}
		}
		public static dynamic page_layouts
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["page_layouts"];
			}
			set
			{
				HttpContext.Current.Items["page_layouts"] = value;
			}
		}
		public static dynamic page_options
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["page_options"];
			}
			set
			{
				HttpContext.Current.Items["page_options"] = value;
			}
		}
		public static dynamic pagesData
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["pagesData"];
			}
			set
			{
				HttpContext.Current.Items["pagesData"] = value;
			}
		}
		public static dynamic pd_pages
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["pd_pages"];
			}
			set
			{
				HttpContext.Current.Items["pd_pages"] = value;
			}
		}
		public static dynamic projectBuildKey
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["projectBuildKey"];
			}
			set
			{
				HttpContext.Current.Items["projectBuildKey"] = value;
			}
		}
		public static dynamic projectLanguage
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["projectLanguage"];
			}
			set
			{
				HttpContext.Current.Items["projectLanguage"] = value;
			}
		}
		public static dynamic query
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["query"];
			}
			set
			{
				HttpContext.Current.Items["query"] = value;
			}
		}
		public static dynamic regenerateSessionOnLogin
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["regenerateSessionOnLogin"];
			}
			set
			{
				HttpContext.Current.Items["regenerateSessionOnLogin"] = value;
			}
		}
		public static dynamic reportCaseSensitiveGroupFields
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["reportCaseSensitiveGroupFields"];
			}
			set
			{
				HttpContext.Current.Items["reportCaseSensitiveGroupFields"] = value;
			}
		}
		public static dynamic requestPage
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["requestPage"];
			}
			set
			{
				HttpContext.Current.Items["requestPage"] = value;
			}
		}
		public static dynamic requestTable
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["requestTable"];
			}
			set
			{
				HttpContext.Current.Items["requestTable"] = value;
			}
		}
		public static dynamic resizeImagesOnClient
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["resizeImagesOnClient"];
			}
			set
			{
				HttpContext.Current.Items["resizeImagesOnClient"] = value;
			}
		}
		public static dynamic restApiCall
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["restApiCall"];
			}
			set
			{
				HttpContext.Current.Items["restApiCall"] = value;
			}
		}
		public static dynamic restApis
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["restApis"];
			}
			set
			{
				HttpContext.Current.Items["restApis"] = value;
			}
		}
		public static dynamic restResultCache
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["restResultCache"];
			}
			set
			{
				HttpContext.Current.Items["restResultCache"] = value;
			}
		}
		public static dynamic restStorage
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["restStorage"];
			}
			set
			{
				HttpContext.Current.Items["restStorage"] = value;
			}
		}
		public static dynamic runnerDatabases
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerDatabases"];
			}
			set
			{
				HttpContext.Current.Items["runnerDatabases"] = value;
			}
		}
		public static dynamic runnerDbTableInfo
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerDbTableInfo"];
			}
			set
			{
				HttpContext.Current.Items["runnerDbTableInfo"] = value;
			}
		}
		public static dynamic runnerDbTables
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerDbTables"];
			}
			set
			{
				HttpContext.Current.Items["runnerDbTables"] = value;
			}
		}
		public static dynamic runnerLangMessages
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerLangMessages"];
			}
			set
			{
				HttpContext.Current.Items["runnerLangMessages"] = value;
			}
		}
		public static dynamic runnerMenus
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerMenus"];
			}
			set
			{
				HttpContext.Current.Items["runnerMenus"] = value;
			}
		}
		public static dynamic runnerPageInfo
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerPageInfo"];
			}
			set
			{
				HttpContext.Current.Items["runnerPageInfo"] = value;
			}
		}
		public static dynamic runnerProjectDefaults
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerProjectDefaults"];
			}
			set
			{
				HttpContext.Current.Items["runnerProjectDefaults"] = value;
			}
		}
		public static dynamic runnerProjectSettings
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerProjectSettings"];
			}
			set
			{
				HttpContext.Current.Items["runnerProjectSettings"] = value;
			}
		}
		public static dynamic runnerRestConnections
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerRestConnections"];
			}
			set
			{
				HttpContext.Current.Items["runnerRestConnections"] = value;
			}
		}
		public static dynamic runnerTableDefaults
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerTableDefaults"];
			}
			set
			{
				HttpContext.Current.Items["runnerTableDefaults"] = value;
			}
		}
		public static dynamic runnerTableLabels
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerTableLabels"];
			}
			set
			{
				HttpContext.Current.Items["runnerTableLabels"] = value;
			}
		}
		public static dynamic runnerTableSettings
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["runnerTableSettings"];
			}
			set
			{
				HttpContext.Current.Items["runnerTableSettings"] = value;
			}
		}
		public static dynamic showCustomMarkerOnPrint
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["showCustomMarkerOnPrint"];
			}
			set
			{
				HttpContext.Current.Items["showCustomMarkerOnPrint"] = value;
			}
		}
		public static dynamic sortgroup
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["sortgroup"];
			}
			set
			{
				HttpContext.Current.Items["sortgroup"] = value;
			}
		}
		public static dynamic sortorder
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["sortorder"];
			}
			set
			{
				HttpContext.Current.Items["sortorder"] = value;
			}
		}
		public static dynamic strLastSQL
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["strLastSQL"];
			}
			set
			{
				HttpContext.Current.Items["strLastSQL"] = value;
			}
		}
		public static dynamic strTableName
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["strTableName"];
			}
			set
			{
				HttpContext.Current.Items["strTableName"] = value;
			}
		}
		public static dynamic strictSettings
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["strictSettings"];
			}
			set
			{
				HttpContext.Current.Items["strictSettings"] = value;
			}
		}
		public static dynamic suggestAllContent
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["suggestAllContent"];
			}
			set
			{
				HttpContext.Current.Items["suggestAllContent"] = value;
			}
		}
		public static dynamic table
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["table"];
			}
			set
			{
				HttpContext.Current.Items["table"] = value;
			}
		}
		public static dynamic tableEvents
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["tableEvents"];
			}
			set
			{
				HttpContext.Current.Items["tableEvents"] = value;
			}
		}
		public static dynamic tableinfo_cache
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["tableinfo_cache"];
			}
			set
			{
				HttpContext.Current.Items["tableinfo_cache"] = value;
			}
		}
		public static dynamic testingLinks
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["testingLinks"];
			}
			set
			{
				HttpContext.Current.Items["testingLinks"] = value;
			}
		}
		public static dynamic twilioAuth
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["twilioAuth"];
			}
			set
			{
				HttpContext.Current.Items["twilioAuth"] = value;
			}
		}
		public static dynamic twilioNumber
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["twilioNumber"];
			}
			set
			{
				HttpContext.Current.Items["twilioNumber"] = value;
			}
		}
		public static dynamic twilioSID
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["twilioSID"];
			}
			set
			{
				HttpContext.Current.Items["twilioSID"] = value;
			}
		}
		public static dynamic useUTF8
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["useUTF8"];
			}
			set
			{
				HttpContext.Current.Items["useUTF8"] = value;
			}
		}
		public static dynamic wizardBuildKey
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["wizardBuildKey"];
			}
			set
			{
				HttpContext.Current.Items["wizardBuildKey"] = value;
			}
		}
		public static Stack<StringBuilder> BufferStack
		{
			get
			{
				return (Stack<StringBuilder>)HttpContext.Current.Items["BufferStack"];
			}
			set
			{
				HttpContext.Current.Items["BufferStack"] = value;
			}
		}
		public static XVar ConnectionStrings
		{
			get
			{
				return (XVar)HttpContext.Current.Items["ConnectionStrings"];
			}
			set
			{
				HttpContext.Current.Items["ConnectionStrings"] = value;
			}
		}
		public static dynamic IsOutputDone
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["IsOutputDone"];
			}
			set
			{
				HttpContext.Current.Items["IsOutputDone"] = value;
			}
		}
		public static XVar LastDBError
		{
			get
			{
				return (XVar)HttpContext.Current.Items["LastDBError"];
			}
			set
			{
				HttpContext.Current.Items["LastDBError"] = value;
			}
		}
		public static dynamic pageObject
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["pageObject"];
			}
			set
			{
				HttpContext.Current.Items["pageObject"] = value;
			}
		}
		public static dynamic strErrorHandler
		{
			get
			{
				return (dynamic)HttpContext.Current.Items["strErrorHandler"];
			}
			set
			{
				HttpContext.Current.Items["strErrorHandler"] = value;
			}
		}
		private static dynamic appliedInitializers
		{
			get
			{
				if(HttpContext.Current.Items["appliedInitializers"] == null)
					HttpContext.Current.Items["appliedInitializers"] = new HashSet<string>();
				return (dynamic)HttpContext.Current.Items["appliedInitializers"];
			}
		}
		public static void init_crosstable_report()
		{
			if(GlobalVars.appliedInitializers.Contains("crosstable_report"))
				return;

			GlobalVars.group_sort_y = XVar.Clone(XVar.Array());

			GlobalVars.appliedInitializers.Add("crosstable_report");
		}
		public static void init_globalvars()
		{
			if(GlobalVars.appliedInitializers.Contains("globalvars"))
				return;

			GlobalVars._cachedSeachClauses = XVar.Clone(XVar.Array());
			GlobalVars._eventClasses = XVar.Clone(XVar.Array());
			GlobalVars._gmdays = XVar.Clone(new XVar(0, 31, 1, 31, 2, 28, 3, 31, 4, 30, 5, 31, 6, 30, 7, 31, 8, 31, 9, 30, 10, 31, 11, 30, 12, 31));
			GlobalVars.ajaxSearchStartsWith = new XVar(true);
			GlobalVars.all_page_layouts = XVar.Clone(XVar.Array());
			GlobalVars.auditMaxFieldLength = new XVar(300);
			GlobalVars.bSubqueriesSupported = new XVar(true);
			GlobalVars.breadcrumb_labels = XVar.Clone(XVar.Array());
			GlobalVars.cCharset = new XVar("utf-8");
			GlobalVars.cCodePage = new XVar(65001);
			GlobalVars.cLoginTable = new XVar("");
			GlobalVars.cUserGroupField = new XVar("");
			GlobalVars.cUserNameField = new XVar("");
			GlobalVars.cacheImages = new XVar(true);
			GlobalVars.cman = XVar.UnPackConnectionManager(new ConnectionManager());
			GlobalVars.conn = new XVar(null);
			GlobalVars.contextStack = XVar.Clone(new RunnerContext());
			GlobalVars.csrfProtectionOff = new XVar(false);
			GlobalVars.dDebug = new XVar(false);
			GlobalVars.debug2Factor = new XVar(false);
			GlobalVars.doMySQLCountBugWorkaround = new XVar(false);
			GlobalVars.dummyEvents = XVar.Clone(new eventsBase());
			GlobalVars.fieldFilterDefaultValue = new XVar("");
			GlobalVars.fieldFilterMaxDisplayValueLength = new XVar(50);
			GlobalVars.fieldFilterMaxSearchValueLength = new XVar(200);
			GlobalVars.fieldFilterMaxValuesCount = new XVar(3000);
			GlobalVars.fieldFilterValueShrinkPostfix = new XVar("...");
			GlobalVars.gGuestHasPermissions = XVar.Clone(-1);
			GlobalVars.gLoadSearchControls = new XVar(30);
			GlobalVars.gPermissionsRead = new XVar(false);
			GlobalVars.gPermissionsRefreshTime = new XVar(5);
			GlobalVars.gReadPermissions = new XVar(true);
			GlobalVars.globalEvents = XVar.Clone(new class_GlobalEvents());
			GlobalVars.jsonDataFromRequest = new XVar(null);
			GlobalVars.locale_info = XVar.Clone(XVar.Array());
			GlobalVars.loginKeyFields = XVar.Clone(XVar.Array());
			GlobalVars.logoutPerformed = new XVar(false);
			GlobalVars.lookupTableLinks = XVar.Clone(XVar.Array());
			GlobalVars.mediaType = XVar.Clone((XVar.Pack(MVCFunctions.COOKIEKeyExists("mediaType")) ? XVar.Pack(MVCFunctions.GetCookie("mediaType")) : XVar.Pack(0)));
			GlobalVars.menuCache = XVar.Clone(XVar.Array());
			GlobalVars.onDemnadVariables = XVar.Clone(XVar.Array());
			GlobalVars.pageTypesForEdit = XVar.Clone(XVar.Array());
			GlobalVars.pageTypesForEdit.InitAndSetArrayItem("add", null);
			GlobalVars.pageTypesForEdit.InitAndSetArrayItem("edit", null);
			GlobalVars.pageTypesForEdit.InitAndSetArrayItem("search", null);
			GlobalVars.pageTypesForEdit.InitAndSetArrayItem("register", null);
			GlobalVars.pageTypesForView = XVar.Clone(XVar.Array());
			GlobalVars.pageTypesForView.InitAndSetArrayItem("list", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("view", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("export", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("print", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("report", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("rprint", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("chart", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("masterlist", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("masterprint", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("gantt", null);
			GlobalVars.pageTypesForView.InitAndSetArrayItem("calendar", null);
			GlobalVars.page_layouts = XVar.Clone(XVar.Array());
			GlobalVars.page_options = XVar.Clone(XVar.Array());
			GlobalVars.pagesData = XVar.Clone(XVar.Array());
			GlobalVars.pd_pages = XVar.Clone(XVar.Array());
			GlobalVars.query = new XVar(null);
			GlobalVars.regenerateSessionOnLogin = new XVar(true);
			GlobalVars.resizeImagesOnClient = new XVar(true);
			GlobalVars.restApis = XVar.Clone(new RestManager());
			GlobalVars.restStorage = XVar.Clone(XVar.Array());
			GlobalVars.runnerDatabases = XVar.Clone(XVar.Array());
			GlobalVars.runnerDbTableInfo = XVar.Clone(XVar.Array());
			GlobalVars.runnerDbTables = XVar.Clone(XVar.Array());
			GlobalVars.runnerLangMessages = XVar.Clone(XVar.Array());
			GlobalVars.runnerMenus = XVar.Clone(XVar.Array());
			GlobalVars.runnerPageInfo = XVar.Clone(XVar.Array());
			GlobalVars.runnerProjectSettings = XVar.Clone(XVar.Array());
			GlobalVars.runnerRestConnections = XVar.Clone(XVar.Array());
			GlobalVars.runnerTableLabels = XVar.Clone(XVar.Array());
			GlobalVars.runnerTableSettings = XVar.Clone(XVar.Array());
			GlobalVars.strictSettings = new XVar(false);
			GlobalVars.suggestAllContent = new XVar(true);
			GlobalVars.table = new XVar("");
			GlobalVars.tableEvents = XVar.Clone(XVar.Array());
			GlobalVars.tableinfo_cache = XVar.Clone(XVar.Array());

			GlobalVars.appliedInitializers.Add("globalvars");
		}
		public static void init_phpfunctions()
		{
			if(GlobalVars.appliedInitializers.Contains("phpfunctions"))
				return;

			GlobalVars.mbEnabled = XVar.Clone(MVCFunctions.extension_loaded(new XVar("mbstring")));

			GlobalVars.appliedInitializers.Add("phpfunctions");
		}
		public static void init_reportlib()
		{
			if(GlobalVars.appliedInitializers.Contains("reportlib"))
				return;

			GlobalVars.cache_db2time = XVar.Clone(XVar.Array());
			GlobalVars.cache_formatweekstart = XVar.Clone(XVar.Array());
			GlobalVars.cache_getdayofweek = XVar.Clone(XVar.Array());
			GlobalVars.cache_getweekstart = XVar.Clone(XVar.Array());

			GlobalVars.appliedInitializers.Add("reportlib");
		}
		public static void init_runnerpage()
		{
			if(GlobalVars.appliedInitializers.Contains("runnerpage"))
				return;

			GlobalVars.menuNodesObject = new XVar(null);
			GlobalVars.menuNodesObject = new XVar(null);

			GlobalVars.appliedInitializers.Add("runnerpage");
		}
		public static void init_table_events()
		{
			if(GlobalVars.appliedInitializers.Contains("table_events"))
				return;

			GlobalVars.query = XVar.Clone(ProjectSettings.getTableSQLQuery((XVar)(table)));
			GlobalVars.table = new XVar("<%- @escape( @tableName() ) %>");

			GlobalVars.appliedInitializers.Add("table_events");
		}
	}
}
