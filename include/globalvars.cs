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
	public static partial class RunnerSettings
	{
		public static void globalVars()
		{
			GlobalVars.dDebug = new XVar(false);
			GlobalVars.lookupTableLinks = XVar.Clone(XVar.Array());
			GlobalVars.runnerProjectSettings = XVar.Clone(XVar.Array());
			GlobalVars.runnerTableSettings = XVar.Clone(XVar.Array());
			GlobalVars.runnerMenus = XVar.Clone(XVar.Array());
			GlobalVars.runnerDatabases = XVar.Clone(XVar.Array());
			GlobalVars.runnerLangMessages = XVar.Clone(XVar.Array());
			GlobalVars.runnerPageInfo = XVar.Clone(XVar.Array());
			GlobalVars.locale_info = XVar.Clone(XVar.Array());
			GlobalVars.page_options = XVar.Clone(XVar.Array());
			GlobalVars.runnerTableLabels = XVar.Clone(XVar.Array());
			GlobalVars.pd_pages = XVar.Clone(XVar.Array());
			GlobalVars.runnerRestConnections = XVar.Clone(XVar.Array());
			GlobalVars.all_page_layouts = XVar.Clone(XVar.Array());
			GlobalVars.page_layouts = XVar.Clone(XVar.Array());
			GlobalVars.menuCache = XVar.Clone(XVar.Array());
			GlobalVars.cCharset = new XVar("utf-8");
			GlobalVars.cCodePage = new XVar(65001);
			GlobalVars._cachedSeachClauses = XVar.Clone(XVar.Array());
			GlobalVars.table = new XVar("");
			GlobalVars.query = new XVar(null);
			GlobalVars.fieldFilterMaxDisplayValueLength = new XVar(50);
			GlobalVars.fieldFilterMaxSearchValueLength = new XVar(200);
			GlobalVars.fieldFilterMaxValuesCount = new XVar(3000);
			GlobalVars.fieldFilterDefaultValue = new XVar("");
			GlobalVars.fieldFilterValueShrinkPostfix = new XVar("...");
			GlobalVars.gPermissionsRefreshTime = new XVar(5);
			GlobalVars.auditMaxFieldLength = new XVar(300);
			GlobalVars.csrfProtectionOff = new XVar(false);
			GlobalVars.cacheImages = new XVar(true);
			GlobalVars.resizeImagesOnClient = new XVar(true);
			GlobalVars.gLoadSearchControls = new XVar(30);
			GlobalVars.bSubqueriesSupported = new XVar(true);
			GlobalVars.regenerateSessionOnLogin = new XVar(true);
			GlobalVars.ajaxSearchStartsWith = new XVar(true);
			GlobalVars.suggestAllContent = new XVar(true);
			GlobalVars.doMySQLCountBugWorkaround = new XVar(false);
			GlobalVars.mediaType = XVar.Clone((XVar.Pack(MVCFunctions.COOKIEKeyExists("mediaType")) ? XVar.Pack(MVCFunctions.GetCookie("mediaType")) : XVar.Pack(0)));
			GlobalVars.gPermissionsRead = new XVar(false);
			GlobalVars.gReadPermissions = new XVar(true);
			GlobalVars.pagesData = XVar.Clone(XVar.Array());
			GlobalVars.logoutPerformed = new XVar(false);
			GlobalVars.gGuestHasPermissions = XVar.Clone(-1);
			GlobalVars.conn = new XVar(null);
			GlobalVars.loginKeyFields = XVar.Clone(XVar.Array());
			GlobalVars.cLoginTable = new XVar("");
			GlobalVars.cUserNameField = new XVar("");
			GlobalVars.cUserGroupField = new XVar("");
			GlobalVars.contextStack = XVar.Clone(new RunnerContext());
			GlobalVars.cman = XVar.Clone(new ConnectionManager());
			GlobalVars.restApis = XVar.Clone(new RestManager());
			GlobalVars.breadcrumb_labels = XVar.Clone(XVar.Array());
			GlobalVars.dummyEvents = XVar.Clone(new eventsBase());
			GlobalVars.tableEvents = XVar.Clone(XVar.Array());
			GlobalVars.globalEvents = XVar.Clone(new class_GlobalEvents());
			GlobalVars.onDemnadVariables = XVar.Clone(XVar.Array());
			GlobalVars.jsonDataFromRequest = new XVar(null);
			GlobalVars.runnerDbTableInfo = XVar.Clone(XVar.Array());
			GlobalVars.runnerDbTables = XVar.Clone(XVar.Array());
			GlobalVars.tableinfo_cache = XVar.Clone(XVar.Array());
			GlobalVars.restStorage = XVar.Clone(XVar.Array());
			GlobalVars._eventClasses = XVar.Clone(XVar.Array());
			GlobalVars.strictSettings = new XVar(false);
			GlobalVars.debug2Factor = new XVar(false);
			GlobalVars._gmdays = XVar.Clone(new XVar(0, 31, 1, 31, 2, 28, 3, 31, 4, 30, 5, 31, 6, 30, 7, 31, 8, 31, 9, 30, 10, 31, 11, 30, 12, 31));
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
			GlobalVars.pageTypesForEdit = XVar.Clone(XVar.Array());
			GlobalVars.pageTypesForEdit.InitAndSetArrayItem("add", null);
			GlobalVars.pageTypesForEdit.InitAndSetArrayItem("edit", null);
			GlobalVars.pageTypesForEdit.InitAndSetArrayItem("search", null);
			GlobalVars.pageTypesForEdit.InitAndSetArrayItem("register", null);
		}
	}

}
