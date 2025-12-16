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
	public partial class RunnerPage : XClass
	{
		public dynamic id = XVar.Pack(1);
		public dynamic pageName = XVar.Pack("");
		protected dynamic isUseAjaxSuggest = XVar.Pack(true);
		public dynamic pageType = XVar.Pack("");
		public dynamic mode = XVar.Pack(0);
		public dynamic isDisplayLoading = XVar.Pack(false);
		public dynamic strOriginalTableName = XVar.Pack("");
		protected dynamic strCaption = XVar.Pack("");
		public dynamic shortTableName = XVar.Pack("");
		public dynamic sessionPrefix = XVar.Pack("");
		public dynamic tName = XVar.Pack("");
		public dynamic gstrOrderBy = XVar.Pack("");
		public XTempl xt = null;
		public dynamic searchClauseObj = XVar.Pack(null);
		public dynamic needSearchClauseObj = XVar.Pack(true);
		public dynamic flyId = XVar.Pack(1);
		public dynamic includes_js = XVar.Array();
		public dynamic includes_jsreq = XVar.Array();
		public dynamic includes_css = XVar.Array();
		public dynamic recId = XVar.Pack(0);
		public dynamic googleMapCfg = XVar.Array();
		public dynamic reCaptchaCfg = XVar.Array();
		public dynamic captchaValue = XVar.Pack("");
		public dynamic captchaName = XVar.Pack("captcha");
		public dynamic isCaptchaOk = XVar.Pack(true);
		public dynamic captchaPassesCount = XVar.Pack(5);
		public dynamic permis = XVar.Array();
		public dynamic isGroupSecurity = XVar.Pack(true);
		protected dynamic debugJSMode = XVar.Pack(false);
		protected dynamic recIds = XVar.Array();
		public dynamic listAjax = XVar.Pack(false);
		public dynamic body = XVar.Array();
		public dynamic masterTable = XVar.Pack("");
		public dynamic masterPage = XVar.Pack("");
		public dynamic masterRecordData = XVar.Array();
		public dynamic jsSettings = XVar.Array();
		public dynamic jsControlSettings = XVar.Array();
		public dynamic controlsHTMLMap = XVar.Array();
		public dynamic viewControlsHTMLMap = XVar.Array();
		public dynamic controlsMap = XVar.Array();
		public dynamic pageData = XVar.Array();
		public dynamic viewControlsMap = XVar.Array();
		public dynamic arrRecsPerPage = XVar.Array();
		public dynamic pageSize = XVar.Pack(0);
		protected dynamic returnPageType = XVar.Pack("");
		public dynamic returnPage = XVar.Pack("");
		public dynamic eventsObject;
		public dynamic masterKeysReq = XVar.Array();
		public dynamic lockingObj = XVar.Pack(null);
		protected dynamic isResizeColumns = XVar.Pack(false);
		public dynamic filesToSave = XVar.Array();
		public dynamic filesToMove = XVar.Array();
		public dynamic filesToDelete = XVar.Array();
		protected dynamic is508 = XVar.Pack(false);
		public dynamic cipherer = XVar.Pack(null);
		public ProjectSettings pSet = null;
		public dynamic pSetUsers = XVar.Pack(null);
		public ProjectSettings pSetEdit = null;
		public dynamic numRowsFromSQL = XVar.Pack(0);
		protected dynamic myPage = XVar.Pack(0);
		protected dynamic mapProvider = XVar.Pack(0);
		protected dynamic recordsOnPage = XVar.Pack(0);
		public dynamic recsPerRowList = XVar.Pack(0);
		public dynamic recsPerRowPrint = XVar.Pack(0);
		public dynamic listGridLayout = XVar.Pack(false);
		public dynamic printGridLayout = XVar.Pack(false);
		public dynamic gridTabs = XVar.Array();
		protected dynamic fieldCssRules = XVar.Array();
		protected dynamic cell_css_rules = XVar.Pack("");
		protected dynamic row_css_rules = XVar.Pack("");
		protected dynamic mobile_css_rules = XVar.Pack("");
		public dynamic totalsFields = XVar.Array();
		public dynamic rowsFound = XVar.Pack(false);
		protected dynamic deleteMessage = XVar.Pack("");
		protected dynamic maxPages = XVar.Pack(1);
		public dynamic templatefile = XVar.Pack("");
		public dynamic menuNodes = XVar.Array();
		protected SQLQuery gQuery = null;
		protected dynamic isControlsMapFilled = XVar.Pack(false);
		protected dynamic controls = XVar.Pack(null);
		public dynamic viewControls = XVar.Pack(null);
		public dynamic readOnlyFields = XVar.Array();
		public dynamic editFields = XVar.Array();
		protected dynamic searchPanelActivated = XVar.Pack(false);
		public dynamic pSetSearch = XVar.Pack(null);
		public dynamic searchTableName = XVar.Pack("");
		protected dynamic pageLayout = XVar.Pack(null);
		protected dynamic warnLeavingPages = XVar.Pack(null);
		public dynamic tableBasedSearchPanelAdded = XVar.Pack(false);
		public dynamic mainTable = XVar.Pack("");
		public dynamic mainField = XVar.Pack("");
		protected dynamic _cachedWhereComponents = XVar.Pack(null);
		protected dynamic timeRegexp;
		protected dynamic dispNoneStyle = XVar.Pack("style=\"display: none;\"");
		public dynamic searchLogger = XVar.Pack(null);
		public dynamic searchSavingEnabled = XVar.Pack(false);
		public dynamic pageHasSavedSearches = XVar.Pack(false);
		protected dynamic formBricks = XVar.Array();
		protected dynamic headerForms = XVar.Array();
		protected dynamic footerForms = XVar.Array();
		protected dynamic bodyForms = XVar.Array();
		public dynamic connection = XVar.Pack(null);
		public dynamic dashTName = XVar.Pack("");
		public dynamic dashPage = XVar.Pack("");
		public dynamic dashElementName = XVar.Pack("");
		protected dynamic dashSet;
		protected dynamic dashElementData = XVar.Array();
		public dynamic initialStep = XVar.Pack(0);
		public dynamic format = XVar.Pack("");
		public dynamic message = XVar.Pack("");
		public dynamic mapRefresh = XVar.Pack(false);
		public dynamic vpCoordinates = XVar.Array();
		public dynamic querySQL = XVar.Pack("");
		public dynamic masterPageType = XVar.Pack("");
		public dynamic masterPSet;
		protected dynamic data = XVar.Pack(null);
		public dynamic errorFields = XVar.Array();
		protected dynamic detailsTableObjects = XVar.Array();
		public dynamic isScrollGridBody = XVar.Pack(false);
		public dynamic stopPRG = XVar.Pack(false);
		public dynamic pageTitle = XVar.Pack(null);
		public dynamic pushContext = XVar.Pack(true);
		public dynamic standaloneContext = XVar.Pack(null);
		public dynamic keys = XVar.Array();
		public dynamic selection = XVar.Array();
		public dynamic tabChangeling = XVar.Pack(null);
		public dynamic skipMapFilter = XVar.Pack(false);
		public dynamic changeDetailsTabTitles = XVar.Pack(true);
		public dynamic pageTable = XVar.Pack("");
		public dynamic renderedBody = XVar.Pack("");
		public dynamic renderedButtons = XVar.Pack("");
		public dynamic pdfBackgroundImage = XVar.Pack("");
		public dynamic addRawFieldValues = XVar.Pack(false);
		protected dynamic dataSource = XVar.Pack(null);
		protected dynamic queryCommand = XVar.Pack(null);
		public dynamic listPagePSet = XVar.Pack(null);
		protected dynamic masterPageId;
		public RunnerPage(dynamic var_params)
		{
			dynamic detailsTables = XVar.Array();
			CommonFunctions.RunnerApply(this, (XVar)(var_params));
			if(XVar.Pack(this.pushContext))
			{
				RunnerContext.pushPageContext(this);
			}
			else
			{
				this.standaloneContext = XVar.Clone(new RunnerContextItem(new XVar(Constants.CONTEXT_PAGE), (XVar)(new XVar("pageObj", this))));
			}
			this.id = XVar.Clone((int)this.id);
			if(XVar.Pack(!(XVar)(this.id)))
			{
				this.id = new XVar(1);
			}
			GlobalVars.pagesData.InitAndSetArrayItem(this.pageData, this.id);
			this.pageData.InitAndSetArrayItem(XVar.Array(), "proxy");
			this.xt.pageId = XVar.Clone(this.id);
			this.xt.setPage(this);
			this.xt.assign(new XVar("pageid"), (XVar)(this.id));
			this.xt.assign(new XVar("id"), (XVar)(this.id));
			this.xt.assign_function(new XVar("pagetitlelabel"), new XVar("xt_pagetitlelabel"), (XVar)(new XVar("pageObj", this)));
			this.xt.assign_function(new XVar("jspagetitlelabel"), new XVar("xt_jspagetitlelabel"), (XVar)(new XVar("pageObj", this)));
			this.setTableConnection();
			if(this.pageTable == "")
			{
				this.pageTable = XVar.Clone(this.tName);
			}
			this.createProjectSettings();
			this.setDataSource();
			this.checkOauthLogin();
			this.pageName = XVar.Clone(this.pSet.pageName());
			this.pageData.InitAndSetArrayItem(this.pageName, "pageName");
			this.pageData.InitAndSetArrayItem(this.pSet.helperFormItems(), "helperFormItems");
			this.pageData.InitAndSetArrayItem(this.pSet.helperItemsByType(), "helperItemsByType");
			this.pageData.InitAndSetArrayItem(this.pSet.allFieldItems(), "helperFieldItems");
			this.pageData.InitAndSetArrayItem(this.pSet.buttons(), "buttons");
			this.pageData.InitAndSetArrayItem(this.pSet.allFieldItems(), "fieldItems");
			this.pageData.InitAndSetArrayItem(this.buildJsDetailsSettings((XVar)(this.pSet)), "detailTables");
			foreach (KeyValuePair<XVar, dynamic> b in this.pSet.buttons().GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(b.Value)))
				{
					continue;
				}
				this.AddJSFile((XVar)(MVCFunctions.Concat("usercode/button_", b.Value, ".js")));
			}
			this.pageData.InitAndSetArrayItem(CommonFunctions.getMediaType(), "renderedMediaType");
			this.pSetEdit = XVar.UnPackProjectSettings(this.pSet);
			this.pSetSearch = XVar.Clone(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_SEARCH), (XVar)(this.pageName), (XVar)(this.pageTable)));
			this.searchTableName = XVar.Clone(this.tName);
			if(XVar.Pack(this.dashTName))
			{
				this.dashSet = XVar.Clone(new ProjectSettings((XVar)(this.dashTName), new XVar("dashboard"), (XVar)(this.dashPage)));
				if(XVar.Pack(this.isDashboardElement()))
				{
					this.dashElementData = XVar.Clone(this.dashSet.getDashboardElementData((XVar)(this.dashElementName)));
				}
			}
			this.assignCipherer();
			this.prepareCharts();
			this.controls = XVar.Clone(new EditControlsContainer(this, (XVar)(this.pSetEdit), (XVar)(this.pageType)));
			this.viewControls = XVar.Clone(new ViewControlsContainer((XVar)(this.pSet), (XVar)(this.pageType), this));
			this.gQuery = XVar.UnPackSQLQuery(this.pSet.getSQLQuery());
			this.googleMapCfg = XVar.Clone(new XVar("id", this.id, "isUseMainMaps", false, "isUseFieldsMaps", false, "isUseGoogleMap", false, "APIcode", ProjectSettings.getProjectValue(new XVar("mapSettings"), new XVar("apikey")), "mainMapIds", XVar.Array(), "fieldMapsIds", XVar.Array(), "mapsData", XVar.Array(), "useEmbedMapsAPI", (XVar)(ProjectSettings.getProjectValue(new XVar("mapSettings"), new XVar("embed")))  && (XVar)(CommonFunctions.getMapProvider() == Constants.GOOGLE_MAPS)));
			this.captchaPassesCount = XVar.Clone(ProjectSettings.captchaValue(new XVar("passesCount")));
			if(ProjectSettings.captchaValue(new XVar("captchaType")) == Constants.RE_CAPTCHA)
			{
				this.reCaptchaCfg = XVar.Clone(new XVar("siteKey", ProjectSettings.captchaValue(new XVar("siteKey")), "inputCaptchaId", ""));
			}
			this.debugJSMode = new XVar(false);
			if(this.flyId < this.id + 1)
			{
				this.flyId = XVar.Clone(this.id + 1);
			}
			if(XVar.Pack(this.tName))
			{
				this.permis.InitAndSetArrayItem(this.getPermissions(), this.tName);
				this.eventsObject = XVar.Clone(CommonFunctions.getEventObject((XVar)(this.pSet)));
			}
			if(XVar.Pack(!(XVar)(this.sessionPrefix)))
			{
				this.assignSessionPrefix();
			}
			this.isDisplayLoading = XVar.Clone(this.pSet.displayLoading());
			this.shortTableName = XVar.Clone(CommonFunctions.GetTableURL((XVar)(this.tName)));
			this.pageLayout = XVar.Clone(CommonFunctions.GetPageLayout((XVar)(this.pageTable), (XVar)(this.pageName)));
			this.searchPanelActivated = XVar.Clone(this.checkIfSearchPanelActivated());
			this.searchSavingEnabled = XVar.Clone((XVar)(this.isSearchSavingEnabled())  && (XVar)(this.needSearchClauseObj));
			if((XVar)(this.masterTable)  && (XVar)(!(XVar)(this.pSet.verifyMasterTable((XVar)(this.masterTable)))))
			{
				this.masterTable = new XVar("");
			}
			if((XVar)(this.mode != Constants.LIST_MASTER)  && (XVar)(this.mode != Constants.PRINT_MASTER))
			{
				this.setSessionVariables();
			}
			this.lockingObj = XVar.Clone(this.getLockingObject());
			this.warnLeavingPages = XVar.Clone(this.pSet.warnLeavingPages());
			this.is508 = XVar.Clone((XVar)(CommonFunctions.isEnableSection508())  && (XVar)(!(XVar)(this.pdfJsonMode())));
			this.mapProvider = XVar.Clone(CommonFunctions.getMapProvider());
			this.strCaption = XVar.Clone(Labels.getTableCaption((XVar)(this.tName)));
			if(XVar.Pack(this.returnPage))
			{
				this.returnPageType = XVar.Clone(this.pSet.getPageType((XVar)(this.returnPage)));
			}
			if(XVar.Pack(!(XVar)(this.returnPageType)))
			{
				this.returnPageType = XVar.Clone(this.getDefaultPageType());
			}
			this.isResizeColumns = XVar.Clone(this.pSet.isResizeColumns());
			this.isUseAjaxSuggest = XVar.Clone(this.pSetSearch.isUseAjaxSuggest());
			this.setTemplateFile();
			if(XVar.Pack(this.pageLayout))
			{
				dynamic formItems = null, helper = XVar.Array();
				this.AddCSSFile((XVar)(this.pageLayout.getCSSFiles((XVar)(CommonFunctions.isRTL()))));
				helper = this.pSet.helperFormItems();
				formItems = helper["formItems"];
				foreach (KeyValuePair<XVar, dynamic> l in MVCFunctions.array_keys((XVar)(formItems)).GetEnumerator())
				{
					this.xt.assign((XVar)(MVCFunctions.Concat(l.Value, "_block")), new XVar(true));
					this.xt.assign((XVar)(MVCFunctions.Concat(l.Value, "_outerblock")), new XVar(true));
				}
			}
			this.jsSettings = XVar.Clone(XVar.Array());
			this.jsSettings.InitAndSetArrayItem(XVar.Array(), "tableSettings");
			this.controlsMap.InitAndSetArrayItem(false, "oldLayout");
			this.controlsMap.InitAndSetArrayItem(this.getLayoutName(), "layoutName");
			this.controlsMap.InitAndSetArrayItem(this.pSet.pageTable(), "pageTable");
			detailsTables = XVar.Clone(this.pSet.getDetailsTables());
			foreach (KeyValuePair<XVar, dynamic> details in detailsTables.GetEnumerator())
			{
				dynamic dPermission = null;
				dPermission = XVar.Clone(this.getPermissions((XVar)(details.Value)));
				this.permis.InitAndSetArrayItem(dPermission, details.Value);
				if((XVar)((XVar)(this.pageType == Constants.PAGE_LIST)  || (XVar)(this.pageType == Constants.PAGE_REPORT))  || (XVar)(this.pageType == Constants.PAGE_CHART))
				{
					XSession.Session.Remove(MVCFunctions.Concat(details.Value, "_advsearch"));
				}
			}
			if((XVar)(this.pageType == Constants.PAGE_ADD)  || (XVar)(this.pageType == Constants.PAGE_EDIT))
			{
				this.controlsMap.InitAndSetArrayItem(XVar.Array(), "dControlsMap");
			}
			this.controlsMap.InitAndSetArrayItem(XVar.Array(), "toolTips");
			this.controlsMap.InitAndSetArrayItem(this.searchPanelActivated, "searchPanelActivated");
			this.controlsMap.InitAndSetArrayItem(XVar.Array(), "controls");
			if((XVar)(!(XVar)((XVar)(this.pageType == Constants.PAGE_ADD)  && (XVar)(this.mode == Constants.ADD_INLINE)))  && (XVar)(!(XVar)((XVar)(this.pageType == Constants.PAGE_EDIT)  && (XVar)(this.mode == Constants.EDIT_INLINE))))
			{
				dynamic allSearchFields = null;
				allSearchFields = XVar.Clone(this.pSet.getAllSearchFields());
				this.controlsMap.InitAndSetArrayItem(XVar.Array(), "search");
				this.controlsMap.InitAndSetArrayItem(XVar.Array(), "search", "searchBlocks");
				this.controlsMap.InitAndSetArrayItem(allSearchFields, "search", "allSearchFields");
				this.controlsMap.InitAndSetArrayItem(this.getSearchFieldsLabels((XVar)(allSearchFields)), "search", "allSearchFieldsLabels");
				this.controlsMap.InitAndSetArrayItem(this.pSet.getPanelSearchFields(), "search", "panelSearchFields");
				this.controlsMap.InitAndSetArrayItem(this.pSet.getGoogleLikeFields(), "search", "googleLikeFields");
				this.controlsMap.InitAndSetArrayItem(!(XVar)(this.pSet.isFlexibleSearch()), "search", "inflexSearchPanel");
				this.controlsMap.InitAndSetArrayItem(this.pSet.getSearchRequiredFields(), "search", "requiredSearchFields");
				this.controlsMap.InitAndSetArrayItem(this.pSet.noRecordsOnFirstPage(), "search", "isSearchRequired");
				this.controlsMap.InitAndSetArrayItem(this.searchTableName, "search", "searchTableName");
				this.controlsMap.InitAndSetArrayItem(this.pSetSearch.getShortTableName(), "search", "shortSearchTableName");
				if(this.pageType != Constants.PAGE_SEARCH)
				{
					this.controlsMap.InitAndSetArrayItem(this.pageType, "search", "submitPageType");
				}
				else
				{
					this.controlsMap.InitAndSetArrayItem(this.returnPageType, "search", "submitPageType");
					this.controlsMap.InitAndSetArrayItem(this.returnPage, "search", "submitPageName");
				}
			}
			this.googleMapCfg.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("mapSettings"), new XVar("apikey")), "APIcode");
			this.processMasterKeyValue();
			if(XVar.Pack(this.masterTable))
			{
				this.pageData.InitAndSetArrayItem(this.masterTable, "masterTable");
				this.pageData.InitAndSetArrayItem(this.getMasterKeysParams(), "masterKeys");
				this.pageData.InitAndSetArrayItem(this.getMasterPageName(), "masterPageName");
			}
			this.gridTabs = XVar.Clone(this.pSet.getGridTabs());
			this.assignSearchLogger();
			this.checkFSProviders();
			this.pageData.InitAndSetArrayItem(this.getNotifications(), "notifications");
			this.pageData.InitAndSetArrayItem(this.pSet.getMobileSub(), "mobileSub");
			this.assignMenus();
		}
		public virtual XVar getDefaultPageType()
		{
			dynamic ret = null;
			ret = XVar.Clone(this.pSet.getDefaultPageType());
			if(ret == "list")
			{
				dynamic pageTypes = XVar.Array();
				pageTypes = XVar.Clone(new XVar(0, "list", 1, "calendar", 2, "report"));
				foreach (KeyValuePair<XVar, dynamic> pageType in pageTypes.GetEnumerator())
				{
					if(XVar.Pack(this.pSet.pageTypeAvailable((XVar)(pageType.Value))))
					{
						return pageType.Value;
					}
				}
			}
			return ret;
		}
		public virtual XVar limitRowCount(dynamic _param_rowCount, dynamic _param_pSet_packed = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_pSet as Object == null) _param_pSet = null;
			#endregion

			#region pass-by-value parameters
			dynamic rowCount = XVar.Clone(_param_rowCount);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			if(XVar.Pack(XVar.Equals(XVar.Pack(pSet), XVar.Pack(null))))
			{
				pSet = XVar.UnPackProjectSettings(this.pSet);
			}
			return (XVar.Pack((XVar)(pSet.getRecordsLimit())  && (XVar)(pSet.getRecordsLimit() < rowCount)) ? XVar.Pack(pSet.getRecordsLimit()) : XVar.Pack(rowCount));
		}
		public virtual XVar getGridTab(dynamic _param_tabId)
		{
			#region pass-by-value parameters
			dynamic tabId = XVar.Clone(_param_tabId);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> tab in this.gridTabs.GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(tab.Value["id"]), XVar.Pack(tabId)))
				{
					return tab.Value;
				}
			}
			return false;
		}
		public virtual XVar getCurrentTab()
		{
			dynamic i = null, tab = XVar.Array();
			if(XVar.Pack(!(XVar)(this.gridTabs)))
			{
				return false;
			}
			tab = XVar.Clone(this.getGridTab((XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_currentTab")])));
			if(XVar.Pack(tab))
			{
				if(XVar.Pack(!(XVar)(tab["hidden"])))
				{
					return tab;
				}
			}
			tab = XVar.Clone(this.getGridTab(new XVar("")));
			if(XVar.Pack(tab))
			{
				if(XVar.Pack(!(XVar)(tab["hidden"])))
				{
					return tab;
				}
			}
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.gridTabs); ++(i))
			{
				if(XVar.Pack(!(XVar)(this.gridTabs[i]["hidden"])))
				{
					return this.gridTabs[i];
				}
			}
			return this.gridTabs[0];
		}
		public virtual XVar getCurrentTabWhere()
		{
			dynamic currentTab = XVar.Array();
			if(XVar.Equals(XVar.Pack(this.tabChangeling), XVar.Pack(null)))
			{
				currentTab = XVar.Clone(this.getCurrentTab());
			}
			else
			{
				currentTab = XVar.Clone(this.getGridTab((XVar)(this.tabChangeling)));
			}
			if(XVar.Pack(currentTab))
			{
				return DB.PrepareSQL((XVar)(currentTab["where"]));
			}
			return "";
		}
		public virtual XVar getCurrentTabId()
		{
			dynamic currentTab = XVar.Array();
			currentTab = XVar.Clone(this.getCurrentTab());
			if(XVar.Pack(currentTab))
			{
				return currentTab["id"];
			}
			return "";
		}
		public virtual XVar setCurrentTabId(dynamic _param_tabId)
		{
			#region pass-by-value parameters
			dynamic tabId = XVar.Clone(_param_tabId);
			#endregion

			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_currentTab")] = tabId;

			return null;
		}
		public virtual XVar prepareGridTabs()
		{
			dynamic tab = XVar.Array();
			if(XVar.Pack(!(XVar)(this.masterTable)))
			{
				foreach (KeyValuePair<XVar, dynamic> key in MVCFunctions.array_keys((XVar)(this.gridTabs)).GetEnumerator())
				{
					dynamic masterTokent = null;
					tab = XVar.Clone(this.gridTabs[key.Value]);
					masterTokent = XVar.Clone(DB.readMasterTokens((XVar)(tab["where"])));
					if(0 < MVCFunctions.count(masterTokent))
					{
						this.gridTabs.Remove(key.Value);
					}
				}
			}
			if(XVar.Pack(this.gridTabsAvailable()))
			{
				dynamic tabIndices = XVar.Array();
				tabIndices = XVar.Clone(MVCFunctions.array_keys((XVar)(this.gridTabs)));
				foreach (KeyValuePair<XVar, dynamic> i in tabIndices.GetEnumerator())
				{
					tab = XVar.Clone(this.gridTabs[i.Value]);
					if((XVar)(tab["showCount"])  || (XVar)(tab["hideEmpty"]))
					{
						this.gridTabs.InitAndSetArrayItem(this.getRowCountByTab((XVar)(tab["id"])), i.Value, "count");
						if((XVar)(tab["hideEmpty"])  && (XVar)(!(XVar)(this.gridTabs[i.Value]["count"])))
						{
							this.gridTabs.InitAndSetArrayItem(true, i.Value, "hidden");
						}
					}
				}
			}

			return null;
		}
		public virtual XVar getTabsHtml()
		{
			dynamic currentTab = XVar.Array(), tabs = XVar.Array();
			currentTab = XVar.Clone(this.getCurrentTab());
			tabs = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> tab in this.gridTabs.GetEnumerator())
			{
				dynamic countRowHtml = null, defaultPage = null, getParams = XVar.Array(), linkAttrs = XVar.Array(), tabAttrs = XVar.Array();
				linkAttrs = XVar.Clone(XVar.Array());
				getParams = XVar.Clone(XVar.Array());
				getParams.InitAndSetArrayItem(MVCFunctions.Concat("tab=", tab.Value["id"]), null);
				getParams.InitAndSetArrayItem(this.getStateUrlParams(), null);
				if(this.pageName != defaultPage)
				{
					getParams.InitAndSetArrayItem(MVCFunctions.Concat("page=", this.pageName), null);
				}
				linkAttrs.InitAndSetArrayItem(MVCFunctions.Concat("href=\"", MVCFunctions.GetTableLink((XVar)(this.shortTableName), (XVar)(this.pageType), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(getParams)))), "\""), null);
				linkAttrs.InitAndSetArrayItem(MVCFunctions.Concat("data-pageid=\"", this.id, "\""), null);
				linkAttrs.InitAndSetArrayItem(MVCFunctions.Concat("data-tabid=\"", tab.Value["id"], "\""), null);
				tabAttrs = XVar.Clone(XVar.Array());
				if(XVar.Equals(XVar.Pack(currentTab["id"]), XVar.Pack(tab.Value["id"])))
				{
					tabAttrs.InitAndSetArrayItem("class=\"active\"", null);
				}
				if(XVar.Pack(tab.Value["hidden"]))
				{
					tabAttrs.InitAndSetArrayItem("data-hidden", null);
				}
				countRowHtml = XVar.Clone((XVar.Pack(tab.Value["showCount"]) ? XVar.Pack(MVCFunctions.Concat("&nbsp;(", tab.Value["count"], ")")) : XVar.Pack("")));
				tabs.InitAndSetArrayItem(MVCFunctions.Concat("<li ", MVCFunctions.implode(new XVar(" "), (XVar)(tabAttrs)), "><a ", MVCFunctions.implode(new XVar(" "), (XVar)(linkAttrs)), ">", this.getTabTitle((XVar)(tab.Value["id"])), countRowHtml, "</a></li>"), null);
			}
			return MVCFunctions.implode(new XVar(""), (XVar)(tabs));
		}
		public virtual XVar getGridTabsCount()
		{
			dynamic tcount = null;
			tcount = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> tab in this.gridTabs.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(tab.Value["hidden"])))
				{
					++(tcount);
				}
			}
			return tcount;
		}
		public virtual XVar processGridTabs()
		{
			dynamic html = null, tabId = null;
			this.pageData.InitAndSetArrayItem("", "gridTabs");
			tabId = XVar.Clone(this.getCurrentTabId());
			this.prepareGridTabs();
			if(MVCFunctions.count(this.gridTabs) <= 1)
			{
				return null;
			}
			html = XVar.Clone(this.getTabsHtml());
			this.pageData.InitAndSetArrayItem(html, "gridTabs");
			this.pageData.InitAndSetArrayItem(this.getCurrentTabId(), "tabId");
			if(XVar.Pack(this.displayTabsInPage()))
			{
				this.xt.assign(new XVar("grid_tabs"), new XVar(true));
				this.xt.assign(new XVar("grid_tabs_content"), (XVar)(html));
			}
			return tabId != this.pageData["tabId"];
		}
		public virtual XVar addTab(dynamic _param_where, dynamic _param_title, dynamic _param_tabId)
		{
			#region pass-by-value parameters
			dynamic where = XVar.Clone(_param_where);
			dynamic title = XVar.Clone(_param_title);
			dynamic tabId = XVar.Clone(_param_tabId);
			#endregion

			if(!XVar.Equals(XVar.Pack(this.getGridTab((XVar)(tabId))), XVar.Pack(false)))
			{
				return false;
			}
			this.gridTabs.InitAndSetArrayItem(new XVar("id", tabId, "title", new XVar("type", Constants.mlTypeText, "text", title), "where", where), null);
			return true;
		}
		public virtual XVar deleteTab(dynamic _param_tabId)
		{
			#region pass-by-value parameters
			dynamic tabId = XVar.Clone(_param_tabId);
			#endregion

			dynamic deleteKey = null;
			deleteKey = new XVar(false);
			foreach (KeyValuePair<XVar, dynamic> tab in this.gridTabs.GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(tab.Value["id"]), XVar.Pack(tabId)))
				{
					deleteKey = XVar.Clone(tab.Key);
					break;
				}
			}
			if(!XVar.Equals(XVar.Pack(deleteKey), XVar.Pack(false)))
			{
				this.gridTabs.Remove(deleteKey);
			}

			return null;
		}
		public virtual XVar setTabTitle(dynamic _param_tabId, dynamic _param_title)
		{
			#region pass-by-value parameters
			dynamic tabId = XVar.Clone(_param_tabId);
			dynamic title = XVar.Clone(_param_title);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> tab in this.gridTabs.GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(tab.Value["id"]), XVar.Pack(tabId)))
				{
					this.gridTabs.InitAndSetArrayItem(new XVar("type", Constants.mlTypeText, "text", title), tab.Key, "title");
					return true;
				}
			}
			return false;
		}
		public virtual XVar setTabWhere(dynamic _param_tabId, dynamic _param_where)
		{
			#region pass-by-value parameters
			dynamic tabId = XVar.Clone(_param_tabId);
			dynamic where = XVar.Clone(_param_where);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> tab in this.gridTabs.GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(tab.Value["id"]), XVar.Pack(tabId)))
				{
					this.gridTabs.InitAndSetArrayItem(where, tab.Key, "where");
					return true;
				}
			}
			return false;
		}
		public virtual XVar getTabTitle(dynamic _param_tabId)
		{
			#region pass-by-value parameters
			dynamic tabId = XVar.Clone(_param_tabId);
			#endregion

			dynamic tab = XVar.Array();
			tab = XVar.Clone(this.getTabInfo((XVar)(tabId)));
			if(XVar.Pack(!(XVar)(tab)))
			{
				return false;
			}
			return Labels.multilangString((XVar)(tab["title"]));
		}
		public virtual XVar getTabInfo(dynamic _param_tabId)
		{
			#region pass-by-value parameters
			dynamic tabId = XVar.Clone(_param_tabId);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> tab in this.gridTabs.GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(tab.Value["id"]), XVar.Pack(tabId)))
				{
					return tab.Value;
				}
			}
			return null;
		}
		public virtual XVar getTabFlags(dynamic _param_tabId)
		{
			#region pass-by-value parameters
			dynamic tabId = XVar.Clone(_param_tabId);
			#endregion

			dynamic flags = XVar.Array(), tab = XVar.Array();
			flags = XVar.Clone(XVar.Array());
			tab = XVar.Clone(this.getGridTab((XVar)(tabId)));
			if(XVar.Pack(!(XVar)(tab)))
			{
				return false;
			}
			flags.InitAndSetArrayItem(tab["showCount"], "showCount");
			flags.InitAndSetArrayItem(tab["hideEmpty"], "hideEmpty");
			return flags;
		}
		public virtual XVar setTabFlags(dynamic _param_tabId, dynamic _param_flags)
		{
			#region pass-by-value parameters
			dynamic tabId = XVar.Clone(_param_tabId);
			dynamic flags = XVar.Clone(_param_flags);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> tab in this.gridTabs.GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(tab.Value["id"]), XVar.Pack(tabId)))
				{
					if(XVar.Pack(flags.KeyExists("showCount")))
					{
						this.gridTabs.InitAndSetArrayItem(flags["showCount"], tab.Key, "showCount");
					}
					if(XVar.Pack(flags.KeyExists("showCount")))
					{
						this.gridTabs.InitAndSetArrayItem(flags["hideEmpty"], tab.Key, "hideEmpty");
					}
					return true;
				}
			}
			return false;
		}
		protected virtual XVar getLockingObject()
		{
			return CommonFunctions.GetLockingObject((XVar)(this.tName));
		}
		public virtual XVar isDashboardElement()
		{
			if(this.dashElementName == "")
			{
				return false;
			}
			return true;
		}
		public virtual XVar init()
		{
			if(XVar.Pack(this.xt))
			{
				this.xt.assign(new XVar("pagetitle"), (XVar)(this.getPageTitle((XVar)(this.pageName), (XVar)((XVar.Pack(this.tName == Constants.GLOBAL_PAGES) ? XVar.Pack("") : XVar.Pack(this.tName))), new XVar(null), new XVar(null), new XVar(false))));
			}
			this.buildAddedSearchPanel();
			if(XVar.Pack(Security.hasLogin()))
			{
				this.initLogin();
			}
			if((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_LIST)  && (XVar)((XVar)((XVar)(this.mode == Constants.LIST_AJAX)  || (XVar)(this.mode == Constants.LIST_SIMPLE))  || (XVar)(this.mode == Constants.LIST_LOOKUP)))  || (XVar)(this.pageType == Constants.PAGE_DASHBOARD))  || (XVar)((XVar)(this.pageType == Constants.PAGE_CALENDAR)  && (XVar)(this.mode == Constants.CALENDAR_SIMPLE)))  || (XVar)((XVar)(this.pageType == Constants.PAGE_GANTT)  && (XVar)(this.mode == Constants.GANTT_SIMPLE)))  || (XVar)(this.pageType == Constants.PAGE_DASHBOARD))  || (XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_REPORT)  && (XVar)(XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.REPORT_SIMPLE))))  || (XVar)((XVar)(this.pageType == Constants.PAGE_CHART)  && (XVar)(this.mode == Constants.CHART_SIMPLE))))
			{
				dynamic filterPanelVisible = null;
				if(XVar.Pack(this.pSet.getFilterFields()))
				{
					this.prepareGridTabs();
				}
				filterPanelVisible = XVar.Clone(this.buildFilterPanel());
				if(XVar.Pack(!(XVar)(filterPanelVisible)))
				{
					this.hideElement(new XVar("filterpanel"));
				}
			}

			return null;
		}
		protected virtual XVar setTableConnection()
		{
			if(this.tName != Constants.GLOBAL_PAGES)
			{
				this.connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(this.tName)));
			}

			return null;
		}
		protected virtual XVar setDataSource()
		{
			this.dataSource = XVar.Clone(CommonFunctions.getDataSource((XVar)(this.tName), (XVar)(this.pSet), (XVar)(this.connection)));

			return null;
		}
		public virtual XVar getDataSource()
		{
			return this.dataSource;
		}
		protected virtual XVar assignCipherer()
		{
			this.cipherer = XVar.Clone(new RunnerCipherer((XVar)(this.tName), (XVar)(this.pSet)));

			return null;
		}
		public virtual XVar initLogin()
		{
			dynamic loggedAsGuest = null;
			this.xt.assign(new XVar("security_block"), new XVar(true));
			this.xt.assign(new XVar("username"), (XVar)(XSession.Session["UserName"]));
			if(XVar.Pack(Security.showUserPic()))
			{
				this.xt.assign(new XVar("userbutton_image"), (XVar)(MVCFunctions.Concat("<span class=\"r-user-image\"><img src=\"", MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat("userpic=", MVCFunctions.RawUrlDecode((XVar)(Security.getUserName()))))), "\"></span>")));
			}
			else
			{
				this.xt.assign(new XVar("userbutton_icon"), new XVar(true));
			}
			this.xt.assign(new XVar("logoutlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"logoutButton", this.id, "\"")));
			loggedAsGuest = XVar.Clone(Security.isGuest());
			this.xt.assign(new XVar("loggedas_message"), (XVar)(!(XVar)(loggedAsGuest)));
			this.xt.assign(new XVar("guestloginbutton"), (XVar)(loggedAsGuest));
			this.xt.assign(new XVar("userinfo_link"), (XVar)(Security.currentUserInDatabase()));
			this.xt.assign(new XVar("logoutbutton"), (XVar)(Security.logoutAvailable()));
			this.xt.assign(new XVar("guest_register"), (XVar)(loggedAsGuest));
			this.xt.assign(new XVar("guestloginlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"loginButton", this.id, "\"")));
			if(this.pSet.getLoginFormType() == Constants.LOGIN_EMBEDED)
			{
				this.xt.assign(new XVar("embeded_log_fields"), new XVar(true));
				this.xt.assign(new XVar("embeded_log_ctrls"), new XVar(true));
				this.xt.assign(new XVar("embeded_username"), (XVar)(loggedAsGuest));
				this.xt.assign(new XVar("embeded_password"), (XVar)(loggedAsGuest));
				if(XVar.Pack(loggedAsGuest))
				{
					this.xt.assign(new XVar("username_attrs"), (XVar)(MVCFunctions.Concat("id=\"username", this.id, "\" placeholder=\"login\"")));
					this.xt.assign(new XVar("password_attrs"), (XVar)(MVCFunctions.Concat("id=\"password", this.id, "\" placeholder=\"password\"")));
				}
			}

			return null;
		}
		public virtual XVar assignAdmin()
		{
			if(XVar.Pack(this.isAdminTable()))
			{
				this.xt.assign(new XVar("exitadminarea_link"), new XVar(true));
				this.xt.assign(new XVar("exitaalink_attrs"), (XVar)(MVCFunctions.Concat("id=\"exitAdminArea", this.id, "\"")));
			}
			if(XVar.Pack(Security.isAdmin()))
			{
				this.xt.assign(new XVar("adminarea_link"), new XVar(true));
				this.xt.assign(new XVar("adminarealink_attrs"), (XVar)(MVCFunctions.Concat("id=\"adminArea", this.id, "\"")));
			}

			return null;
		}
		protected virtual XVar assignSessionPrefix()
		{
			this.sessionPrefix = XVar.Clone(this.tName);

			return null;
		}
		public virtual XVar isSearchSavingEnabled()
		{
			dynamic searchSavingEnabled = null;
			searchSavingEnabled = XVar.Clone(this.pSet.isSearchSavingEnabled());
			if(XVar.Pack(!(XVar)(searchSavingEnabled)))
			{
				return false;
			}
			return (XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_LIST)  && (XVar)((XVar)(this.mode == Constants.LIST_AJAX)  || (XVar)(this.mode == Constants.LIST_SIMPLE)))  || (XVar)((XVar)(this.pageType == Constants.PAGE_REPORT)  && (XVar)(this.mode == Constants.REPORT_SIMPLE)))  || (XVar)((XVar)(this.pageType == Constants.PAGE_CHART)  && (XVar)(this.mode == Constants.CHART_SIMPLE));
		}
		protected virtual XVar assignSearchLogger()
		{
			dynamic savedSearches = null;
			if((XVar)(!(XVar)(this.searchSavingEnabled))  || (XVar)(!(XVar)(this.searchClauseObj)))
			{
				return null;
			}
			this.searchLogger = XVar.Clone(new searchParamsLogger((XVar)(this.tName)));
			savedSearches = XVar.Clone(this.searchLogger.getSavedSeachesParams());
			if(XVar.Pack(MVCFunctions.count(savedSearches)))
			{
				this.pageHasSavedSearches = new XVar(true);
				this.controlsMap.InitAndSetArrayItem(savedSearches, "search", "savedSearches");
				this.controlsMap.InitAndSetArrayItem(this.searchClauseObj.savedSearchIsRun, "search", "savedSearchIsRun");
			}
			this.assignSearchSavingButtons();

			return null;
		}
		public virtual XVar processSaveSearch()
		{
			dynamic searchName = null;
			if((XVar)((XVar)(MVCFunctions.postvalue(new XVar("saveSearch")))  && (XVar)(MVCFunctions.postvalue(new XVar("searchName"))))  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.searchLogger), XVar.Pack(null)))))
			{
				dynamic searchParams = null;
				searchName = XVar.Clone(MVCFunctions.postvalue(new XVar("searchName")));
				searchParams = XVar.Clone(this.getSearchParamsForSaving());
				this.searchLogger.saveSearch((XVar)(searchName), (XVar)(searchParams));
				this.searchClauseObj.savedSearchIsRun = new XVar(true);
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_advsearch")] = MVCFunctions.serialize((XVar)(this.searchClauseObj));
				MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(searchParams)));
				return true;
			}
			if((XVar)((XVar)(MVCFunctions.postvalue(new XVar("deleteSearch")))  && (XVar)(MVCFunctions.postvalue(new XVar("searchName"))))  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.searchLogger), XVar.Pack(null)))))
			{
				searchName = XVar.Clone(MVCFunctions.postvalue(new XVar("searchName")));
				this.searchLogger.deleteSearch((XVar)(searchName));
				MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(XVar.Array())));
				return true;
			}
			return false;
		}
		protected virtual XVar assignSearchSavingButtons()
		{
			this.xt.assign(new XVar("searchsaving_block"), new XVar(true));
			if(XVar.Pack(this.listAjax))
			{
				this.xt.assign(new XVar("saveSeachButton"), new XVar(true));
			}
			if(XVar.Pack(this.searchClauseObj.isSearchFunctionalityActivated()))
			{
				this.xt.assign(new XVar("saveSeachButton"), new XVar(true));
				if(XVar.Pack(this.searchClauseObj.savedSearchIsRun))
				{
					this.hideItemType(new XVar("save_search"));
				}
			}
			else
			{
				if(XVar.Pack(this.listAjax))
				{
					this.hideItemType(new XVar("save_search"));
				}
			}
			this.xt.assign(new XVar("savedSeachesButton"), new XVar(true));
			if(XVar.Pack(!(XVar)(this.pageHasSavedSearches)))
			{
				this.hideItemType(new XVar("saved_searches"));
			}

			return null;
		}
		public virtual XVar getSearchParamsForSaving()
		{
			return this.searchClauseObj.getSearchParamsForSaving();
		}
		protected virtual XVar getSearchFieldsLabels(dynamic _param_searchFields)
		{
			#region pass-by-value parameters
			dynamic searchFields = XVar.Clone(_param_searchFields);
			#endregion

			dynamic sFieldLabels = XVar.Array();
			sFieldLabels = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> sField in searchFields.GetEnumerator())
			{
				sFieldLabels.InitAndSetArrayItem(this.pSetSearch.label((XVar)(sField.Value)), sField.Value);
			}
			return sFieldLabels;
		}
		public virtual XVar spreadRowStyles(dynamic data, dynamic row, dynamic record)
		{
			this.spreadRowStyle((XVar)(data), (XVar)(row), (XVar)(record));
			this.spreadRowCssStyle((XVar)(data), (XVar)(row), (XVar)(record));

			return null;
		}
		protected virtual XVar spreadRowStyle(dynamic data, dynamic row, dynamic record)
		{
			dynamic style = null;
			if(XVar.Pack(!(XVar)(row.KeyExists("rowstyle"))))
			{
				return null;
			}
			style = XVar.Clone(CommonFunctions.extractStyle((XVar)(row["rowstyle"])));
			if(style == XVar.Pack(""))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> field in MVCFunctions.array_keys((XVar)(data)).GetEnumerator())
			{
				record.InitAndSetArrayItem(CommonFunctions.injectStyle((XVar)(record[MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(field.Value)), "_style")]), (XVar)(style)), MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(field.Value)), "_style"));
			}

			return null;
		}
		protected virtual XVar spreadRowCssStyle(dynamic data, dynamic row, dynamic record)
		{
			dynamic style = null;
			if(XVar.Pack(!(XVar)(row.KeyExists("style"))))
			{
				return null;
			}
			style = XVar.Clone(row["style"]);
			if(MVCFunctions.trim((XVar)(style)) == "")
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> field in MVCFunctions.array_keys((XVar)(data)).GetEnumerator())
			{
				record.InitAndSetArrayItem(MVCFunctions.Concat(style, "; ", record[MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(field.Value)), "_css")]), MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(field.Value)), "_css"));
			}

			return null;
		}
		protected virtual XVar setRowCssRule(dynamic _param_rowCssRule)
		{
			#region pass-by-value parameters
			dynamic rowCssRule = XVar.Clone(_param_rowCssRule);
			#endregion

			dynamic selectors = null, tdSelector = null;
			tdSelector = XVar.Clone(MVCFunctions.Concat("[data-record-id=\"", this.recId, "\"][data-pageid=\"", this.id, "\"]"));
			selectors = XVar.Clone(MVCFunctions.Concat(" td", tdSelector, tdSelector));
			if(this.listGridLayout == Constants.gltVERTICAL)
			{
				selectors = MVCFunctions.Concat(selectors, " td");
			}
			this.row_css_rules = MVCFunctions.Concat(this.row_css_rules, selectors, "{", this.getCustomCSSRule((XVar)(rowCssRule)), "}");

			return null;
		}
		protected virtual XVar getCustomCSSRule(dynamic _param_unprocessedCss)
		{
			#region pass-by-value parameters
			dynamic unprocessedCss = XVar.Clone(_param_unprocessedCss);
			#endregion

			dynamic cssRules = XVar.Array(), i = null, rules = XVar.Array();
			cssRules = XVar.Clone(XVar.Array());
			rules = XVar.Clone(MVCFunctions.explode(new XVar(";"), (XVar)(unprocessedCss)));
			i = new XVar(0);
			for(;i < MVCFunctions.count(rules); i++)
			{
				if(MVCFunctions.trim((XVar)(rules[i])) != "")
				{
					cssRules.InitAndSetArrayItem(MVCFunctions.Concat(rules[i], " !important"), null);
				}
			}
			return MVCFunctions.implode(new XVar(";"), (XVar)(cssRules));
		}
		protected virtual XVar setFieldCssRule(dynamic _param_fieldCssRule, dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldCssRule = XVar.Clone(_param_fieldCssRule);
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			dynamic className = null, selectors = null;
			if(XVar.Pack(this.fieldCssRules.KeyExists(fieldCssRule)))
			{
				return this.fieldCssRules[fieldCssRule];
			}
			className = XVar.Clone(MVCFunctions.Concat("rnr-style", this.recId, "-", fieldName));
			this.fieldCssRules.InitAndSetArrayItem(className, fieldCssRule);
			if(this.listGridLayout == Constants.gltVERTICAL)
			{
				selectors = XVar.Clone(MVCFunctions.Concat(" td[data-record-id][data-record-id][data-record-id][data-record-id] td.", className, ", .", className, ":not([data-label-col])"));
			}
			else
			{
				selectors = XVar.Clone(MVCFunctions.Concat(" td[data-record-id][data-record-id][data-record-id][data-record-id].", className, ", .", className));
			}
			this.cell_css_rules = MVCFunctions.Concat(this.cell_css_rules, selectors, "{", this.getCustomCSSRule((XVar)(fieldCssRule)), "}");
			return className;
		}
		public virtual XVar addCustomCss()
		{
			dynamic gbl = XVar.Array();
			if((XVar)((XVar)(!(XVar)(this.cell_css_rules))  && (XVar)(!(XVar)(this.row_css_rules)))  && (XVar)(!(XVar)(this.mobile_css_rules)))
			{
				return null;
			}
			gbl = XVar.Clone(this.xt.getVar(new XVar("grid_block")));
			if(XVar.Pack(gbl))
			{
				dynamic rules = null;
				rules = XVar.Clone(MVCFunctions.Concat(this.row_css_rules, this.cell_css_rules, "\n", this.mobile_css_rules));
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(gbl)))))
				{
					gbl = XVar.Clone(new XVar("begin", MVCFunctions.Concat("<style class=\"rnr-cells-css\" type=\"text/css\"> ", rules, " </style>")));
				}
				else
				{
					gbl.InitAndSetArrayItem(MVCFunctions.Concat(gbl["begin"], "<style class=\"rnr-cells-css\" type=\"text/css\"> ", rules, " </style>"), "begin");
				}
				this.xt.assign(new XVar("grid_block"), (XVar)(gbl));
			}

			return null;
		}
		public virtual XVar setRowCssRules(dynamic record)
		{
			if(XVar.Pack(record["css"]))
			{
				this.setRowCssRule((XVar)(record["css"]));
			}
			if(XVar.Pack(record["hovercss"]))
			{
				this.Invoke("setRowHoverCssRule", (XVar)(record["hovercss"]));
			}

			return null;
		}
		public virtual XVar setRowClassNames(dynamic record, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic className = null, gFieldName = null;
			gFieldName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(field)));
			record[MVCFunctions.Concat(gFieldName, "_class")] = MVCFunctions.Concat(record[MVCFunctions.Concat(gFieldName, "_class")], this.fieldClass((XVar)(field)));
			if(XVar.Pack(record[MVCFunctions.Concat(gFieldName, "_css")]))
			{
				className = XVar.Clone(this.setFieldCssRule((XVar)(record[MVCFunctions.Concat(gFieldName, "_css")]), (XVar)(gFieldName)));
				record[MVCFunctions.Concat(gFieldName, "_class")] = MVCFunctions.Concat(record[MVCFunctions.Concat(gFieldName, "_class")], " ", className);
			}
			if(XVar.Pack(record[MVCFunctions.Concat(gFieldName, "_hovercss")]))
			{
				dynamic classNameHover = null;
				classNameHover = XVar.Clone(this.Invoke("setRowHoverCssRule", (XVar)(record[MVCFunctions.Concat(gFieldName, "_hovercss")]), (XVar)(gFieldName)));
				if(!XVar.Equals(XVar.Pack(classNameHover), XVar.Pack(className)))
				{
					record[MVCFunctions.Concat(gFieldName, "_class")] = MVCFunctions.Concat(record[MVCFunctions.Concat(gFieldName, "_class")], " ", classNameHover);
				}
			}

			return null;
		}
		public virtual XVar getLayoutName()
		{
			if(XVar.Pack(this.pageLayout))
			{
				return this.pageLayout.style;
			}
			else
			{
				return "";
			}

			return null;
		}
		public virtual XVar processMasterKeyValue()
		{
			dynamic i = null;
			if(XVar.Pack(MVCFunctions.count(this.masterKeysReq)))
			{
				i = new XVar(1);
				for(;i <= MVCFunctions.count(this.masterKeysReq); i++)
				{
					XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_masterkey", i)] = this.masterKeysReq[i];
				}
				if(XVar.Pack(XSession.Session.KeyExists(MVCFunctions.Concat(this.sessionPrefix, "_masterkey", i))))
				{
					XSession.Session.Remove(MVCFunctions.Concat(this.sessionPrefix, "_masterkey", i));
				}
			}
			else
			{
				this.masterKeysReq = XVar.Clone(XVar.Array());
				i = new XVar(1);
				while(XVar.Pack(XSession.Session.KeyExists(MVCFunctions.Concat(this.sessionPrefix, "_masterkey", i))))
				{
					this.masterKeysReq.InitAndSetArrayItem(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_masterkey", i)], i);
					++(i);
				}
			}
			if(XVar.Pack(this.masterTable))
			{
				XSession.Session[MVCFunctions.Concat(this.masterTable, "_masterRecordData")] = this.getMasterRecord();
			}

			return null;
		}
		public virtual XVar displayMasterTableInfo()
		{
			dynamic backButtonHref = null, detailsKeys = null, j = null, keys = XVar.Array(), mPSet = null, mParams = XVar.Array(), mTName = null, master = null, masterDefaultPageType = null, masterKeys = XVar.Array(), masterPage = null, masterShortTable = null, masterTableData = XVar.Array(), masterTableType = null, mrData = null, var_params = XVar.Array();
			XTempl xt;
			if(XVar.Pack(!(XVar)(this.pSet.verifyMasterTable((XVar)(this.masterTable)))))
			{
				return null;
			}
			masterTableData = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
			masterShortTable = XVar.Clone(CommonFunctions.GetTableURL((XVar)(this.masterTable)));
			masterTableType = XVar.Clone(CommonFunctions.GetEntityType((XVar)(this.masterTable)));
			masterDefaultPageType = XVar.Clone(ProjectSettings.defaultPageType((XVar)(masterTableType)));
			backButtonHref = XVar.Clone(MVCFunctions.GetTableLink((XVar)(masterShortTable), (XVar)(masterDefaultPageType), new XVar("a=return")));
			if(XVar.Pack(!(XVar)(this.pSet.masterPreview((XVar)(this.masterTable)))))
			{
				return null;
			}
			if((XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_PRINT)  || (XVar)(this.pageType == Constants.PAGE_RPRINT))  || (XVar)(this.pdfJsonMode()))  && (XVar)(masterDefaultPageType == Constants.PAGE_CHART))
			{
				return null;
			}
			this.pageData.InitAndSetArrayItem(true, "hasMasterList");
			detailsKeys = XVar.Clone(masterTableData["detailsKeys"]);
			masterKeys = XVar.Clone(XVar.Array());
			j = new XVar(0);
			for(;j < MVCFunctions.count(detailsKeys); j++)
			{
				masterKeys.InitAndSetArrayItem(this.masterKeysReq[j + 1], null);
			}
			this.addMasterInfoJSAndCSS((XVar)(masterDefaultPageType), (XVar)(this.masterTable), (XVar)(masterShortTable));
			master = XVar.Clone(XVar.Array());
			mrData = XVar.Clone(this.getListMasterRecordData((XVar)(this.masterTable), (XVar)(masterKeys)));
			this.genId();
			var_params = XVar.Clone(new XVar("detailtable", this.tName, "keys", masterKeys, "recId", this.recId, "masterRecordData", mrData));
			keys = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> kf in this.pSet.getTableKeys().GetEnumerator())
			{
				keys.InitAndSetArrayItem(this.masterRecordData[kf.Value], kf.Value);
			}
			this.pageData.InitAndSetArrayItem(keys, "masterPageKeys");
			xt = XVar.UnPackXTempl(new XTempl());
			mParams = XVar.Clone(XVar.Array());
			mParams.InitAndSetArrayItem(xt, "xt");
			mParams.InitAndSetArrayItem(var_params["recId"], "flyId");
			mParams.InitAndSetArrayItem(var_params["recId"], "id");
			mParams.InitAndSetArrayItem(mrData, "masterRecordData");
			mParams.InitAndSetArrayItem(false, "pushContext");
			mParams.InitAndSetArrayItem(masterDefaultPageType, "masterPageType");
			mPSet = XVar.Clone(new ProjectSettings((XVar)(mTName), new XVar(Constants.PAGE_LIST)));
			mParams.InitAndSetArrayItem(mPSet.getDefaultPage((XVar)(masterDefaultPageType)), "pageName");
			mParams.InitAndSetArrayItem(this.masterTable, "tName");
			if(XVar.Pack(this.pdfJsonMode()))
			{
				mParams.InitAndSetArrayItem(true, "pdfJson");
			}
			if((XVar)(this.pageType == Constants.PAGE_PRINT)  || (XVar)(this.pageType == Constants.PAGE_RPRINT))
			{
				mParams.InitAndSetArrayItem("masterprint", "pageType");
				if(mParams["masterPageType"] == Constants.PAGE_REPORT)
				{
					mParams.InitAndSetArrayItem("masterrprint", "pageType");
				}
				mParams.InitAndSetArrayItem(Constants.PRINT_MASTER, "mode");
				masterPage = XVar.Clone(new PrintPage_Master((XVar)(mParams)));
			}
			else
			{
				if(mParams["masterPageType"] == Constants.PAGE_CHART)
				{
					mParams.InitAndSetArrayItem("masterchart", "pageType");
					mParams.InitAndSetArrayItem(Constants.CHART_SIMPLE, "pageMode");
					masterPage = XVar.Clone(new ChartPage_Master((XVar)(mParams)));
				}
				else
				{
					mParams.InitAndSetArrayItem("masterlist", "pageType");
					if(mParams["masterPageType"] == Constants.PAGE_REPORT)
					{
						mParams.InitAndSetArrayItem("masterreport", "pageType");
					}
					mParams.InitAndSetArrayItem(Constants.LIST_MASTER, "mode");
					masterPage = XVar.Clone(ListPage.createListPage((XVar)(this.masterTable), (XVar)(mParams)));
				}
			}
			RunnerContext.push((XVar)(masterPage.standaloneContext));
			masterPage.init();
			if(XVar.Pack(!(XVar)(masterPage.pSet.pageName())))
			{
				return null;
			}
			masterPage.preparePage();
			foreach (KeyValuePair<XVar, dynamic> b in masterPage.pageData["buttons"].GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(b.Value)))
				{
					continue;
				}
				this.AddJSFile((XVar)(MVCFunctions.Concat("usercode/button_", b.Value, ".js")));
			}
			this.pageData.InitAndSetArrayItem(MVCFunctions.array_merge((XVar)(this.pageData["buttons"]), (XVar)(masterPage.pageData["buttons"])), "buttons");
			this.xt.assign(new XVar("mastertable_block"), new XVar(true));
			backButtonHref = XVar.Clone(MVCFunctions.GetTableLink((XVar)(masterShortTable), (XVar)(masterDefaultPageType), new XVar("a=return")));
			masterPage.xt.assign(new XVar("backtomasterlink_attrs"), (XVar)(MVCFunctions.Concat("href=\"", backButtonHref, "\"")));
			masterPage.xt.assign(new XVar("backtomasterlink_caption"), (XVar)(Labels.getTableCaption((XVar)(this.masterTable))));
			if((XVar)((XVar)(this.pageType == Constants.PAGE_VIEW)  || (XVar)(this.pageType == Constants.PAGE_ADD))  || (XVar)(this.pageType == Constants.PAGE_EDIT))
			{
				masterPage.hideItemType(new XVar("back_master"));
			}
			this.masterPageId = XVar.Clone(masterPage.id);
			this.xt.assign_method(new XVar("showmasterfile"), (XVar)(masterPage), new XVar("showMaster"), (XVar)(XVar.Array()));
			this.addMasterMapsSettings((XVar)(this.masterTable), (XVar)(masterPage.recId), ref mrData, (XVar)(masterDefaultPageType));
			this.genId();
			RunnerContext.pop();

			return null;
		}
		public virtual XVar getListMasterRecordData(dynamic _param_mTName, dynamic _param_masterKeyValues)
		{
			#region pass-by-value parameters
			dynamic mTName = XVar.Clone(_param_mTName);
			dynamic masterKeyValues = XVar.Clone(_param_masterKeyValues);
			#endregion

			dynamic dc = null, detailtable = null, filters = XVar.Array(), mCiph = null, mPSet = null, masterDs = null, masterKeys = XVar.Array();
			detailtable = XVar.Clone(this.tName);
			mCiph = XVar.Clone(new RunnerCipherer((XVar)(mTName)));
			mPSet = XVar.Clone(new ProjectSettings((XVar)(mTName), new XVar(Constants.PAGE_LIST)));
			masterDs = XVar.Clone(CommonFunctions.getDataSource((XVar)(this.masterTable), (XVar)(mPSet)));
			filters = XVar.Clone(XVar.Array());
			filters.InitAndSetArrayItem(Security.SelectCondition(new XVar("S"), (XVar)(mPSet)), null);
			masterKeys = XVar.Clone(this.pSet.getMasterKeys((XVar)(mTName)));
			foreach (KeyValuePair<XVar, dynamic> mk in masterKeys["masterKeys"].GetEnumerator())
			{
				filters.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(mk.Value), (XVar)(masterKeyValues[mk.Key])), null);
			}
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(filters)));
			dc.reccount = new XVar(1);
			return mCiph.DecryptFetchedArray((XVar)(masterDs.getList((XVar)(dc)).fetchAssoc()));
		}
		public virtual XVar addMasterMapsSettings(dynamic _param_mTName, dynamic _param_recId, ref dynamic data, dynamic _param_mPageType)
		{
			#region pass-by-value parameters
			dynamic mTName = XVar.Clone(_param_mTName);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic mPageType = XVar.Clone(_param_mPageType);
			#endregion

			dynamic haveMap = null, mPSet = null, masterType = null;
			if(mPageType == Constants.PAGE_CHART)
			{
				return null;
			}
			masterType = new XVar("masterlist");
			if(mPageType == Constants.PAGE_REPORT)
			{
				masterType = new XVar("masterreport");
			}
			else
			{
				if((XVar)(this.pageType == Constants.PAGE_PRINT)  || (XVar)(this.pageType == Constants.PAGE_RPRINT))
				{
					masterType = new XVar("masterprint");
				}
			}
			mPSet = XVar.Clone(new ProjectSettings((XVar)(mTName), (XVar)(masterType)));
			if(XVar.Pack(!(XVar)(data)))
			{
				return null;
			}
			haveMap = new XVar(false);
			foreach (KeyValuePair<XVar, dynamic> fName in mPSet.getMasterListFields().GetEnumerator())
			{
				dynamic address = null, desc = null, fieldMapData = XVar.Array(), keys = null, lat = null, lng = null, mapData = XVar.Array(), mapId = null;
				fieldMapData = XVar.Clone(mPSet.getMapData((XVar)(fName.Value)));
				if(XVar.Pack(!(XVar)(fieldMapData)))
				{
					continue;
				}
				mapData = XVar.Clone(XVar.Array());
				mapData.InitAndSetArrayItem(fName.Value, "fName");
				mapData.InitAndSetArrayItem((XVar.Pack(fieldMapData.KeyExists("zoom")) ? XVar.Pack(fieldMapData["zoom"]) : XVar.Pack("")), "zoom");
				mapData.InitAndSetArrayItem("FIELD_MAP", "type");
				mapData.InitAndSetArrayItem(data[fName.Value], "mapFieldValue");
				address = XVar.Clone((XVar.Pack(data[fieldMapData["address"]]) ? XVar.Pack(data[fieldMapData["address"]]) : XVar.Pack("")));
				lat = XVar.Clone(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)((XVar.Pack(data[fieldMapData["lat"]]) ? XVar.Pack(data[fieldMapData["lat"]]) : XVar.Pack("")))));
				lng = XVar.Clone(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)((XVar.Pack(data[fieldMapData["lng"]]) ? XVar.Pack(data[fieldMapData["lng"]]) : XVar.Pack("")))));
				desc = XVar.Clone((XVar.Pack(data[fieldMapData["desc"]]) ? XVar.Pack(data[fieldMapData["desc"]]) : XVar.Pack(address)));
				mapData.InitAndSetArrayItem(new XVar("address", address, "lat", lat, "lng", lng, "desc", desc, "keys", keys, "mapIcon", mPSet.getMapIcon((XVar)(fName.Value), (XVar)(data))), "markers", null);
				mapId = XVar.Clone(MVCFunctions.Concat("littleMap_", MVCFunctions.GoodFieldName((XVar)(fName.Value)), "_", recId));
				this.googleMapCfg.InitAndSetArrayItem(mapData, "mapsData", mapId);
				this.googleMapCfg.InitAndSetArrayItem(mapId, "fieldMapsIds", null);
				haveMap = new XVar(true);
			}
			if(XVar.Pack(haveMap))
			{
				this.googleMapCfg.InitAndSetArrayItem(true, "isUseGoogleMap");
				this.googleMapCfg.InitAndSetArrayItem(true, "isUseFieldsMaps");
			}

			return null;
		}
		protected virtual XVar addMasterInfoJSAndCSS(dynamic _param_mPageType, dynamic _param_mTableName, dynamic _param_mShortTableName)
		{
			#region pass-by-value parameters
			dynamic mPageType = XVar.Clone(_param_mPageType);
			dynamic mTableName = XVar.Clone(_param_mTableName);
			dynamic mShortTableName = XVar.Clone(_param_mShortTableName);
			#endregion

			dynamic layout = null, mastertype = null;
			if(mPageType == Constants.PAGE_CHART)
			{
				mastertype = new XVar("masterchart");
			}
			else
			{
				if(mPageType == Constants.PAGE_REPORT)
				{
					mastertype = new XVar("masterreport");
				}
				else
				{
					mastertype = new XVar("masterlist");
					if((XVar)(this.pageType == Constants.PAGE_PRINT)  || (XVar)(this.pageType == Constants.PAGE_RPRINT))
					{
						mastertype = new XVar("masterprint");
					}
				}
			}
			if(mPageType != Constants.PAGE_CHART)
			{
				dynamic viewControls = null;
				viewControls = XVar.Clone(new ViewControlsContainer((XVar)(new ProjectSettings((XVar)(mTableName), (XVar)(mastertype))), (XVar)(mastertype)));
				viewControls.addControlsJSAndCSS();
				this.includes_js = XVar.Clone(MVCFunctions.array_merge((XVar)(this.includes_js), (XVar)(viewControls.includes_js)));
				this.includes_jsreq = XVar.Clone(MVCFunctions.array_merge((XVar)(this.includes_jsreq), (XVar)(viewControls.includes_jsreq)));
				this.includes_css = XVar.Clone(MVCFunctions.array_merge((XVar)(this.includes_css), (XVar)(viewControls.includes_css)));
				this.viewControlsMap.InitAndSetArrayItem(viewControls.viewControlsMap, "mViewControlsMap");
			}
			layout = XVar.Clone(CommonFunctions.GetPageLayout((XVar)(mTableName), (XVar)(mastertype)));
			if(XVar.Pack(layout))
			{
				this.AddCSSFile((XVar)(layout.getCSSFiles((XVar)(CommonFunctions.isRTL()))));
			}

			return null;
		}
		public virtual XVar getMasterRecord()
		{
			dynamic dc = null, filters = XVar.Array(), masterDs = null, masterPSet = null;
			if(XVar.Pack(this.masterRecordData))
			{
				return this.masterRecordData;
			}
			if(XVar.Pack(!(XVar)(this.masterTable)))
			{
				return null;
			}
			masterPSet = XVar.Clone(new ProjectSettings((XVar)(this.masterTable), new XVar(Constants.PAGE_LIST)));
			masterDs = XVar.Clone(CommonFunctions.getDataSource((XVar)(this.masterTable), (XVar)(masterPSet)));
			filters = XVar.Clone(XVar.Array());
			filters.InitAndSetArrayItem(Security.SelectCondition(new XVar("S"), (XVar)(masterPSet)), null);
			foreach (KeyValuePair<XVar, dynamic> master in this.pSet.getMasterTables().GetEnumerator())
			{
				if(this.masterTable == master.Value["table"])
				{
					dynamic masterKeys = XVar.Array();
					masterKeys = XVar.Clone(this.getActiveMasterKeys());
					if((XVar)(!(XVar)(masterKeys))  || (XVar)(MVCFunctions.count(masterKeys) != MVCFunctions.count(master.Value["masterKeys"])))
					{
						return XVar.Array();
					}
					foreach (KeyValuePair<XVar, dynamic> key in master.Value["masterKeys"].GetEnumerator())
					{
						filters.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(key.Value), (XVar)(masterKeys[key.Key])), null);
					}
					break;
				}
			}
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(filters)));
			dc.reccount = new XVar(1);
			return masterDs.getList((XVar)(dc)).fetchAssoc();
		}
		public virtual XVar getActiveMasterKeys()
		{
			dynamic i = null, ret = XVar.Array();
			i = new XVar(1);
			ret = XVar.Clone(XVar.Array());
			while(XVar.Pack(true))
			{
				if(XVar.Pack(this.masterKeysReq.KeyExists(i)))
				{
					ret.InitAndSetArrayItem(this.masterKeysReq[i], null);
				}
				else
				{
					break;
				}
				++(i);
			}
			return ret;
		}
		public virtual XVar setProxyValue(dynamic _param_name, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(XVar.Pack(!(XVar)(name)))
			{
				return null;
			}
			this.pageData.InitAndSetArrayItem(value, "proxy", name);

			return null;
		}
		public virtual XVar getProxyValue(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(XVar.Pack(!(XVar)(name)))
			{
				return null;
			}
			return this.pageData["proxy"][name];
		}
		public static XVar setTemplateFile(dynamic instance)
		{
			if(XVar.Pack(!(XVar)(instance.templatefile)))
			{
				dynamic pageId = null;
				if(XVar.Pack(instance.pageName))
				{
					pageId = XVar.Clone(instance.pageName);
				}
				else
				{
					pageId = XVar.Clone(instance.pSet.getDefaultPage((XVar)(instance.pageType)));
				}
				instance.templatefile = XVar.Clone(CommonFunctions.GetTemplateName((XVar)(CommonFunctions.GetTableURL((XVar)(instance.pageTable))), (XVar)(pageId)));
			}
			instance.xt.set_template((XVar)(instance.templatefile));

			return null;
		}
		public virtual XVar setTemplateFile()
		{
			if(XVar.Pack(!(XVar)(this.templatefile)))
			{
				dynamic pageId = null;
				if(XVar.Pack(this.pageName))
				{
					pageId = XVar.Clone(this.pageName);
				}
				else
				{
					pageId = XVar.Clone(this.pSet.getDefaultPage((XVar)(this.pageType)));
				}
				this.templatefile = XVar.Clone(CommonFunctions.GetTemplateName((XVar)(CommonFunctions.GetTableURL((XVar)(this.pageTable))), (XVar)(pageId)));
			}
			this.xt.set_template((XVar)(this.templatefile));

			return null;
		}
		public virtual XVar menuAppearInLayout()
		{
			dynamic menuBricks = XVar.Array();
			if(XVar.Pack(!(XVar)(this.pageLayout)))
			{
				return false;
			}
			menuBricks = XVar.Clone(new XVar(0, "vmenu", 1, "vmenu_mobile", 2, "hmenu", 3, "bsmenu", 4, "quickjump"));
			foreach (KeyValuePair<XVar, dynamic> b in menuBricks.GetEnumerator())
			{
				if(XVar.Pack(this.isBrickSet((XVar)(b.Value))))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar isShowMenu()
		{
			if((XVar)((XVar)((XVar)((XVar)(!(XVar)(this.menuAppearInLayout()))  && (XVar)(this.pageType != Constants.PAGE_MENU))  && (XVar)(this.pageType != Constants.PAGE_ADD))  && (XVar)(this.pageType != Constants.PAGE_VIEW))  && (XVar)(this.pageType != Constants.PAGE_EDIT))
			{
				return false;
			}
			return this.multipleMenuItems();
		}
		public virtual XVar multipleMenuItems(dynamic _param_menuName = null)
		{
			#region default values
			if(_param_menuName as Object == null) _param_menuName = new XVar("main");
			#endregion

			#region pass-by-value parameters
			dynamic menuName = XVar.Clone(_param_menuName);
			#endregion

			dynamic allowedMenuItems = null, menuNodes = XVar.Array(), menuObject = null;
			menuObject = XVar.Clone(RunnerMenu.getMenuObject(new XVar("main")));
			menuNodes = XVar.Clone(menuObject.collectNodes());
			if(XVar.Pack(!(XVar)(menuNodes)))
			{
				return false;
			}
			allowedMenuItems = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> mNode in menuNodes.GetEnumerator())
			{
				dynamic linkType = null, pageId = null, pageType = null, table = null, var_type = null;
				if(1 < allowedMenuItems)
				{
					return true;
				}
				linkType = XVar.Clone(mNode.Value.linkType);
				table = XVar.Clone(mNode.Value.table);
				pageType = XVar.Clone(mNode.Value.pageType);
				pageId = XVar.Clone(mNode.Value.pageId);
				var_type = XVar.Clone(mNode.Value.var_type);
				if(linkType == "Internal")
				{
					if(XVar.Pack(CommonFunctions.menuLinkAvailable((XVar)(table), (XVar)(pageType), (XVar)(pageId))))
					{
						allowedMenuItems++;
					}
				}
				else
				{
					if((XVar)(linkType != "None")  || (XVar)(var_type != "Group"))
					{
						allowedMenuItems++;
					}
				}
			}
			if((XVar)(Security.isAdmin())  && (XVar)(this.pageType == Constants.PAGE_MENU))
			{
				allowedMenuItems++;
			}
			if(XVar.Pack(ProjectSettings.getProjectValue(new XVar("enableWebreports"))))
			{
				allowedMenuItems++;
			}
			return 1 < allowedMenuItems;
		}
		public virtual XVar isUserHaveTablePerm(dynamic _param_tName, dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic tName = XVar.Clone(_param_tName);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic strPerm = null, var_type = null;
			if(XVar.Equals(XVar.Pack(tName), XVar.Pack(Constants.WEBREPORTS_TABLE)))
			{
				return true;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(tName)))))
			{
				return false;
			}
			var_type = XVar.Clone(this.getPermisType((XVar)(pageType)));
			strPerm = XVar.Clone(CommonFunctions.GetUserPermissions((XVar)(tName)));
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(var_type)))))
			{
				return false;
			}
			if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), (XVar)(var_type))), XVar.Pack(false)))
			{
				return true;
			}
			return false;
		}
		public virtual XVar getPermisType(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic var_type = null;
			pageType = XVar.Clone(MVCFunctions.strtolower((XVar)(pageType)));
			var_type = new XVar("");
			if((XVar)((XVar)((XVar)((XVar)((XVar)(pageType == "list")  || (XVar)(pageType == "view"))  || (XVar)(pageType == "search"))  || (XVar)(pageType == "report"))  || (XVar)(pageType == "chart"))  || (XVar)(pageType == "dashboard"))
			{
				var_type = new XVar("S");
			}
			else
			{
				if(pageType == "add")
				{
					var_type = new XVar("A");
				}
				else
				{
					if(pageType == "edit")
					{
						var_type = new XVar("E");
					}
					else
					{
						if((XVar)(pageType == "print")  || (XVar)(pageType == "export"))
						{
							var_type = new XVar("P");
						}
						else
						{
							if(pageType == "import")
							{
								var_type = new XVar("I");
							}
						}
					}
				}
			}
			return var_type;
		}
		public virtual XVar clearSessionKeys()
		{
			if((XVar)((XVar)((XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_LIST)  || (XVar)(this.pageType == Constants.PAGE_CHART))  || (XVar)(this.pageType == Constants.PAGE_REPORT))  || (XVar)(this.pageType == Constants.PAGE_DASHBOARD))  && (XVar)(!(XVar)(MVCFunctions.POSTSize())))  && (XVar)((XVar)(CommonFunctions.IsEmptyRequest())  || (XVar)(MVCFunctions.postvalue("editType") == Constants.ADD_ONTHEFLY)))
			{
				this.unsetAllPageSessionKeys();
			}
			if((XVar)((XVar)(this.pageType == Constants.PAGE_LIST)  && (XVar)(XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.LIST_DETAILS))))  || (XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_REPORT)  && (XVar)(this.mode != Constants.REPORT_SIMPLE))  || (XVar)((XVar)(this.pageType == Constants.PAGE_CHART)  && (XVar)(this.mode != Constants.CHART_SIMPLE))))
			{
				CommonFunctions.storageDelete((XVar)(MVCFunctions.Concat(this.sessionPrefix, "_filters")));
			}

			return null;
		}
		protected virtual XVar unsetAllPageSessionKeys(dynamic _param_sessionPrefix = null)
		{
			#region default values
			if(_param_sessionPrefix as Object == null) _param_sessionPrefix = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic sessionPrefix = XVar.Clone(_param_sessionPrefix);
			#endregion

			dynamic prefixLength = null, sess_unset = null;
			if(XVar.Pack(!(XVar)(sessionPrefix)))
			{
				sessionPrefix = XVar.Clone(this.sessionPrefix);
			}
			prefixLength = XVar.Clone(MVCFunctions.strlen((XVar)(sessionPrefix)));
			sess_unset = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> key in CommonFunctions.storageKeys().GetEnumerator())
			{
				if((XVar)(MVCFunctions.substr((XVar)(key.Value), new XVar(0), (XVar)(prefixLength + 1)) == MVCFunctions.Concat(sessionPrefix, "_"))  && (XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(MVCFunctions.substr((XVar)(key.Value), (XVar)(prefixLength + 1))), new XVar("_"))), XVar.Pack(false))))
				{
					CommonFunctions.storageDelete((XVar)(key.Value));
				}
			}

			return null;
		}
		public virtual XVar setSessionVariables()
		{
			this.clearSessionKeys();
			if(this.masterTable != "")
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_mastertable")] = this.masterTable;
			}
			else
			{
				if((XVar)(this.masterTable == "")  && (XVar)(CommonFunctions.IsSetKeyInRequest(new XVar("mastertable"))))
				{
					XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_mastertable")] = "";
				}
				else
				{
					this.masterTable = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_mastertable")]);
				}
			}
			if((XVar)(this.needSearchClauseObj)  && (XVar)(!(XVar)(this.searchClauseObj)))
			{
				this.searchClauseObj = XVar.Clone(this.getSearchObject());
			}
			if((XVar)(this.searchSavingEnabled)  && (XVar)(this.searchClauseObj))
			{
				this.searchClauseObj.storeSearchParamsForLogging();
			}
			if(XVar.Pack(MVCFunctions.postvalue("pagesize")))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")] = MVCFunctions.postvalue("pagesize");
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")] = 1;
			}
			this.pageSize = XVar.Clone((int)XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")]);
			if(XVar.Pack(MVCFunctions.REQUESTKeyExists("tab")))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_currentTab")] = MVCFunctions.postvalue(new XVar("tab"));
				if((XVar)((XVar)(!XVar.Equals(XVar.Pack(this.pageType), XVar.Pack(Constants.PAGE_PRINT)))  && (XVar)(!XVar.Equals(XVar.Pack(this.pageType), XVar.Pack(Constants.PAGE_RPRINT))))  && (XVar)(!XVar.Equals(XVar.Pack(this.pageType), XVar.Pack(Constants.PAGE_EXPORT))))
				{
					XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")] = 1;
				}
			}

			return null;
		}
		public virtual XVar fillGlobalSettings()
		{
			this.jsSettings.InitAndSetArrayItem(this.buildJsGlobalSettings(), "global");

			return null;
		}
		protected virtual XVar fillTableSettings(dynamic _param_table = null, dynamic _param_pSet_packed = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			if(_param_pSet as Object == null) _param_pSet = null;
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			if(XVar.Pack(!(XVar)(table)))
			{
				table = XVar.Clone(this.tName);
				pSet = XVar.UnPackProjectSettings(this.pSet);
			}
			this.jsSettings.InitAndSetArrayItem(this.buildJsTableSettings((XVar)(table), (XVar)(pSet)), "tableSettings", table);

			return null;
		}
		protected virtual XVar detailsKeyField(dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic keyFields = XVar.Array();
			keyFields = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
			return MVCFunctions.in_array((XVar)(fName), (XVar)(keyFields["detailsKeys"]));
		}
		public virtual XVar fillPreload(dynamic _param_fName, dynamic _param_pageFields, dynamic _param_values, dynamic _param_controls = null)
		{
			#region default values
			if(_param_controls as Object == null) _param_controls = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic pageFields = XVar.Clone(_param_pageFields);
			dynamic values = XVar.Clone(_param_values);
			dynamic controls = XVar.Clone(_param_controls);
			#endregion

			dynamic vals = null;
			if((XVar)(this.detailsKeyField((XVar)(fName)))  || (XVar)(!(XVar)(this.pSet.useCategory((XVar)(fName)))))
			{
				return false;
			}
			vals = XVar.Clone(this.getRawPreloadData((XVar)(fName), (XVar)(values), (XVar)(pageFields)));
			if((XVar)((XVar)(this.pageType == Constants.PAGE_ADD)  || (XVar)(this.pageType == Constants.PAGE_EDIT))  || (XVar)(this.pageType == Constants.PAGE_REGISTER))
			{
				return this.getPreloadArr((XVar)(fName), (XVar)(vals));
			}
			return this.getSearchPreloadArr((XVar)(fName), (XVar)(vals), (XVar)(controls));
		}
		protected virtual XVar getRawPreloadData(dynamic _param_fName, dynamic _param_values, dynamic _param_pageFields)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic values = XVar.Clone(_param_values);
			dynamic pageFields = XVar.Clone(_param_pageFields);
			#endregion

			dynamic vals = XVar.Array();
			vals = XVar.Clone(XVar.Array());
			vals.InitAndSetArrayItem(values[fName], fName);
			if((XVar)((XVar)(this.pageType != Constants.PAGE_ADD)  && (XVar)(this.pageType != Constants.PAGE_EDIT))  && (XVar)(this.pageType != Constants.PAGE_REGISTER))
			{
				return vals;
			}
			foreach (KeyValuePair<XVar, dynamic> parentFName in this.getLookupParentFieldsNames((XVar)(fName)).GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.in_array((XVar)(parentFName.Value), (XVar)(pageFields))))
				{
					vals.InitAndSetArrayItem(values[parentFName.Value], parentFName.Value);
				}
			}
			return vals;
		}
		public virtual XVar getLookupParentFieldsNames(dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			if((XVar)((XVar)(this.pSet.getEditFormat((XVar)(fName)) != Constants.EDIT_FORMAT_LOOKUP_WIZARD)  || (XVar)(this.pSet.getEditFormat((XVar)(fName)) != Constants.EDIT_FORMAT_RADIO))  && (XVar)(!(XVar)(this.pSet.useCategory((XVar)(fName)))))
			{
				return XVar.Array();
			}
			return this.pSet.getLookupParentFNames((XVar)(fName));
		}
		public static XVar sqlFormattedDisplayField(dynamic _param_field, dynamic _param_connection, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic connection = XVar.Clone(_param_connection);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic displayField = null, lookupPSet = null, lookupType = null;
			displayField = XVar.Clone(pSet.getDisplayField((XVar)(field)));
			lookupType = XVar.Clone(pSet.getLookupType((XVar)(field)));
			if((XVar)(0 == MVCFunctions.strlen((XVar)(displayField)))  || (XVar)(pSet.getCustomDisplay((XVar)(field))))
			{
				return displayField;
			}
			if(lookupType != Constants.LT_QUERY)
			{
				return connection.addFieldWrappers((XVar)(displayField));
			}
			lookupPSet = XVar.Clone(new ProjectSettings((XVar)(pSet.getLookupTable((XVar)(field)))));
			return RunnerPage._getFieldSQL((XVar)(displayField), (XVar)(connection), (XVar)(lookupPSet));
		}
		public static XVar _getFieldSQL(dynamic _param_field, dynamic _param_connection, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic connection = XVar.Clone(_param_connection);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic fname = null;
			fname = new XVar("");
			if(XVar.Pack(pSet))
			{
				fname = XVar.Clone(DB.PrepareSQL((XVar)(pSet.getFullFieldName((XVar)(field)))));
			}
			if(XVar.Pack(!(XVar)(connection)))
			{
				connection = XVar.Clone(GlobalVars.cman.getDefault());
			}
			if(XVar.Pack(!(XVar)(connection.dbBased())))
			{
				return "";
			}
			if(fname == XVar.Pack(""))
			{
				return connection.addFieldWrappers((XVar)(field));
			}
			if(XVar.Pack(!(XVar)(pSet.isSQLExpression((XVar)(field)))))
			{
				return connection.addFieldWrappers((XVar)(fname));
			}
			return fname;
		}
		public static XVar _getFieldSQLDecrypt(dynamic _param_field, dynamic _param_connection, dynamic _param_pSet_packed, dynamic _param_cipherer)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic connection = XVar.Clone(_param_connection);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic cipherer = XVar.Clone(_param_cipherer);
			#endregion

			dynamic fname = null;
			fname = XVar.Clone(RunnerPage._getFieldSQL((XVar)(field), (XVar)(connection), (XVar)(pSet)));
			if((XVar)(cipherer)  && (XVar)(pSet))
			{
				if((XVar)(pSet.hasEncryptedFields())  && (XVar)(!(XVar)(cipherer.isEncryptionByPHPEnabled())))
				{
					return cipherer.GetFieldName((XVar)(fname), (XVar)(field));
				}
			}
			return fname;
		}
		public virtual XVar getFieldSQLDecrypt(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return RunnerPage._getFieldSQLDecrypt((XVar)(field), (XVar)(this.connection), (XVar)(this.pSet), (XVar)(this.cipherer));
		}
		public virtual XVar getFieldSQL(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return RunnerPage._getFieldSQL((XVar)(field), (XVar)(this.connection), (XVar)(this.pSet));
		}
		public virtual XVar getTableField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic strField = null;
			strField = XVar.Clone(this.pSet.getStrField((XVar)(field)));
			if(strField != XVar.Pack(""))
			{
				return this.connection.addFieldWrappers((XVar)(strField));
			}
			return this.getFieldSQL((XVar)(field));
		}
		public virtual XVar getPreloadArr(dynamic _param_fName, dynamic _param_vals)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic vals = XVar.Clone(_param_vals);
			#endregion

			dynamic categoryFieldAppear = null, fVal = null, output = XVar.Array(), parentFNames = XVar.Array();
			if((XVar)((XVar)(this.pageType != Constants.PAGE_ADD)  && (XVar)(this.pageType != Constants.PAGE_EDIT))  && (XVar)(this.pageType != Constants.PAGE_REGISTER))
			{
				return false;
			}
			parentFNames = XVar.Clone(this.getLookupParentFieldsNames((XVar)(fName)));
			if(XVar.Pack(!(XVar)(parentFNames)))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(this.checkFieldOnPage((XVar)(fName)))))
			{
				return false;
			}
			categoryFieldAppear = new XVar(true);
			if(this.pageType == Constants.PAGE_ADD)
			{
				foreach (KeyValuePair<XVar, dynamic> pFName in parentFNames.GetEnumerator())
				{
					categoryFieldAppear = XVar.Clone(this.checkFieldOnPage((XVar)(pFName.Value)));
					if(XVar.Pack(categoryFieldAppear))
					{
						break;
					}
				}
			}
			output = XVar.Clone(XVar.Array());
			if((XVar)(!(XVar)(this.pSet.isFreeInput((XVar)(fName))))  || (XVar)(this.pSet.isAllowToEdit((XVar)(fName))))
			{
				dynamic parentFiltersData = XVar.Array();
				parentFiltersData = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> pFName in parentFNames.GetEnumerator())
				{
					parentFiltersData.InitAndSetArrayItem(vals[pFName.Value], pFName.Value);
				}
				output = XVar.Clone(this.getControl((XVar)(fName)).loadLookupContent((XVar)(parentFiltersData), (XVar)(vals[fName]), (XVar)(categoryFieldAppear)));
			}
			else
			{
				if(XVar.Pack(vals.KeyExists(fName)))
				{
					output = XVar.Clone(XVar.Array());
					output.InitAndSetArrayItem(new XVar("displayValue", vals[fName], "linkValue", vals[fName], "keys", null), null);
				}
			}
			if(XVar.Pack(!(XVar)(output)))
			{
				return false;
			}
			fVal = new XVar("");
			if(XVar.Pack(MVCFunctions.strlen((XVar)(vals[fName]))))
			{
				fVal = XVar.Clone(vals[fName]);
			}
			if(XVar.Pack(this.pSet.multiSelect((XVar)(fName))))
			{
				fVal = XVar.Clone(CommonFunctions.splitLookupValues((XVar)(fVal)));
			}
			return new XVar("vals", output, "fVal", fVal);
		}
		protected virtual XVar checkFieldOnPage(dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			return true;
		}
		public virtual XVar headerCommonAssign()
		{
			this.xt.assign(new XVar("logo_block"), new XVar(true));
			this.xt.assign(new XVar("collapse_block"), new XVar(true));
			if(XVar.Pack(Security.hasLogin()))
			{
				dynamic provider = XVar.Array();
				this.assignAdmin();
				provider = XVar.Clone(Security.currentProvider());
				if(provider["type"] == Constants.stDB)
				{
					this.xt.assign(new XVar("changepwd_link"), new XVar(true));
				}
			}

			return null;
		}
		public virtual XVar commonAssign()
		{
			dynamic stateParams = null;
			this.headerCommonAssign();
			this.xt.assign(new XVar("quickjump_attrs"), (XVar)(MVCFunctions.Concat("class=\"", this.makeClassName(new XVar("quickjump")), "\"")));
			this.xt.assign(new XVar("more_list"), new XVar(true));
			this.hideElement(new XVar("searchpanel"));
			this.prepareCollapseButton();
			this.prepareBreadcrumbs();
			stateParams = XVar.Clone(this.getStateUrlParams());
			if(XVar.Pack(stateParams))
			{
				this.xt.assign(new XVar("stateLink"), (XVar)(MVCFunctions.Concat("&", this.getStateUrlParams())));
				this.xt.assign(new XVar("stateLink_full"), (XVar)(MVCFunctions.Concat("?", this.getStateUrlParams())));
			}

			return null;
		}
		public virtual XVar prepareCollapseButton()
		{
			if(XVar.Pack(!(XVar)(this.pSet.hasVerticalBar())))
			{
				return null;
			}
			if(XVar.Pack(MVCFunctions.GetCookie("collapse_leftbar")))
			{
				this.xt.assign(new XVar("leftbar_class"), new XVar("r-left-collapsed"));
				this.hideItemType(new XVar("collapse_button"));
				this.hideItemType(new XVar("logo"));
			}
			else
			{
				this.hideItemType(new XVar("expand_button"));
			}

			return null;
		}
		public virtual XVar getSearchPreloadArr(dynamic _param_fName, dynamic _param_vals, dynamic _param_controls)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic vals = XVar.Clone(_param_vals);
			dynamic controls = XVar.Clone(_param_controls);
			#endregion

			dynamic fVal = null, output = null, parentsFieldsData = XVar.Array(), searchApplied = null;
			if((XVar)((XVar)(XVar.Equals(XVar.Pack(controls), XVar.Pack(null)))  || (XVar)(this.pSet.getEditFormat((XVar)(fName)) != Constants.EDIT_FORMAT_LOOKUP_WIZARD))  || (XVar)(!(XVar)(this.pSet.useCategory((XVar)(fName)))))
			{
				return false;
			}
			parentsFieldsData = XVar.Clone(XVar.Array());
			searchApplied = XVar.Clone(this.searchClauseObj.searchStarted());
			foreach (KeyValuePair<XVar, dynamic> cData in this.pSet.getParentFieldsData((XVar)(fName)).GetEnumerator())
			{
				dynamic parentField = null;
				parentField = XVar.Clone(cData.Value["main"]);
				if(XVar.Pack(!(XVar)(this.searchFieldAppearsOnPage((XVar)(parentField)))))
				{
					continue;
				}
				parentsFieldsData.InitAndSetArrayItem("", parentField);
				if(XVar.Pack(searchApplied))
				{
					dynamic categoryFieldParams = XVar.Array();
					categoryFieldParams = XVar.Clone(this.searchClauseObj.getSearchCtrlParams((XVar)(parentField)));
					if(XVar.Pack(MVCFunctions.count(categoryFieldParams)))
					{
						parentsFieldsData.InitAndSetArrayItem(categoryFieldParams[0]["value1"], parentField);
					}
				}
				else
				{
					dynamic defaultValue = null, f = null;
					defaultValue = XVar.Clone(this.pSet.getDefaultValue((XVar)(f)));
					if(XVar.Pack(MVCFunctions.strlen((XVar)(defaultValue))))
					{
						parentsFieldsData.InitAndSetArrayItem(defaultValue, parentField);
					}
				}
			}
			output = XVar.Clone(controls.getControl((XVar)(fName)).loadLookupContent((XVar)(parentsFieldsData), (XVar)(vals[fName]), (XVar)(0 < MVCFunctions.count(parentsFieldsData))));
			if(XVar.Pack(!(XVar)(output)))
			{
				return false;
			}
			fVal = XVar.Clone(vals[fName]);
			if(XVar.Pack(this.pSet.multiSelect((XVar)(fName))))
			{
				fVal = XVar.Clone(CommonFunctions.splitLookupValues((XVar)(fVal)));
			}
			return new XVar("vals", output, "fVal", fVal);
		}
		public virtual XVar getTimeRegexp()
		{
			dynamic designators = null, is24hoursFormat = null, leadingZero = null, timeDelimiter = null, timeFormat = null, timeSep = null;
			timeDelimiter = XVar.Clone(GlobalVars.locale_info["LOCALE_STIME"]);
			timeFormat = XVar.Clone(GlobalVars.locale_info["LOCALE_STIMEFORMAT"]);
			is24hoursFormat = XVar.Clone(GlobalVars.locale_info["LOCALE_ITIME"] == "1");
			leadingZero = XVar.Clone(GlobalVars.locale_info["LOCALE_ITLZERO"] == "1");
			if(GlobalVars.locale_info["LOCALE_ITIME"] == "0")
			{
				designators = XVar.Clone(MVCFunctions.Concat(MVCFunctions.preg_quote((XVar)(GlobalVars.locale_info["LOCALE_S1159"]), new XVar("")), "|", MVCFunctions.preg_quote((XVar)(GlobalVars.locale_info["LOCALE_S2359"]), new XVar(""))));
			}
			if(XVar.Pack(is24hoursFormat))
			{
				if(XVar.Pack(leadingZero))
				{
					timeFormat = XVar.Clone(MVCFunctions.str_replace(new XVar("HH"), new XVar("(?:0[0-9]|1[0-9]|2[0-3])"), (XVar)(timeFormat)));
				}
				else
				{
					timeFormat = XVar.Clone(MVCFunctions.str_replace(new XVar("H"), new XVar("(?:[1-9]|1[0-9]|2[0-3])"), (XVar)(timeFormat)));
				}
			}
			else
			{
				if(XVar.Pack(leadingZero))
				{
					timeFormat = XVar.Clone(MVCFunctions.str_replace(new XVar("hh"), new XVar("(?:0[1-9]|1[0-2])"), (XVar)(timeFormat)));
				}
				else
				{
					timeFormat = XVar.Clone(MVCFunctions.str_replace(new XVar("h"), new XVar("(?:[1-9]|1[0-2])"), (XVar)(timeFormat)));
				}
				timeFormat = XVar.Clone(MVCFunctions.str_replace(new XVar("tt"), (XVar)(MVCFunctions.Concat("[\\s]{0,2}(?:", designators, "|am|pm)[\\s]{0,2}")), (XVar)(timeFormat)));
			}
			timeSep = XVar.Clone((XVar.Pack(timeDelimiter == ":") ? XVar.Pack(":") : XVar.Pack(MVCFunctions.Concat("(?:", timeDelimiter, "|:)"))));
			timeFormat = XVar.Clone(MVCFunctions.str_replace((XVar)(MVCFunctions.Concat(timeDelimiter, "mm", timeDelimiter, "ss")), (XVar)(MVCFunctions.Concat("(?:", timeSep, "[0-5][0-9](?:", timeSep, "[0-5][0-9])?)?")), (XVar)(timeFormat)));
			timeFormat = XVar.Clone(MVCFunctions.Concat("^", MVCFunctions.str_replace(new XVar(" "), new XVar("[\\s]{0,2}"), (XVar)(timeFormat)), "$"));
			return timeFormat;
		}
		public virtual XVar fillSettings()
		{
			this.fillGlobalSettings();
			this.fillTableSettings();

			return null;
		}
		public virtual XVar fillControlsMap(dynamic _param_arr, dynamic _param_addSet = null, dynamic _param_fName = null)
		{
			#region default values
			if(_param_addSet as Object == null) _param_addSet = new XVar(false);
			if(_param_fName as Object == null) _param_fName = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic arr = XVar.Clone(_param_arr);
			dynamic addSet = XVar.Clone(_param_addSet);
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			if(XVar.Pack(!(XVar)(addSet)))
			{
				foreach (KeyValuePair<XVar, dynamic> val in arr.GetEnumerator())
				{
					CommonFunctions.initArray((XVar)(this.controlsMap), (XVar)(val.Key));
					this.controlsMap.InitAndSetArrayItem(val.Value, val.Key, null);
				}
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> val in arr.GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> vval in val.Value.GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(fName)))
					{
						this.controlsMap.InitAndSetArrayItem(vval.Value, val.Key, MVCFunctions.count(this.controlsMap[val.Key]) - 1, vval.Key);
					}
					else
					{
						dynamic i = null;
						i = new XVar(0);
						for(;i < MVCFunctions.count(this.controlsMap[val.Key]); i++)
						{
							if(this.controlsMap[val.Key][i]["fieldName"] == fName)
							{
								this.controlsMap.InitAndSetArrayItem(vval.Value, val.Key, i, vval.Key);
								break;
							}
						}
					}
				}
			}

			return null;
		}
		public virtual XVar fillControlsHTMLMap()
		{
			this.controlsHTMLMap.InitAndSetArrayItem(XVar.Array(), this.tName);
			this.controlsHTMLMap.InitAndSetArrayItem(XVar.Array(), this.tName, this.pageType);
			this.controlsHTMLMap.InitAndSetArrayItem(XVar.Array(), this.tName, this.pageType, this.id);
			this.controlsMap.InitAndSetArrayItem(this.googleMapCfg, "gMaps");
			if(XVar.Pack(this.searchClauseObj))
			{
				if(XVar.Pack(!(XVar)(this.controlsMap.KeyExists("search"))))
				{
					this.controlsMap.InitAndSetArrayItem(XVar.Array(), "search");
				}
				this.controlsMap.InitAndSetArrayItem(this.searchClauseObj.searchStarted(), "search", "usedSrch");
			}
			foreach (KeyValuePair<XVar, dynamic> val in this.controlsMap.GetEnumerator())
			{
				this.controlsHTMLMap.InitAndSetArrayItem(val.Value, this.tName, this.pageType, this.id, val.Key);
			}
			this.viewControlsHTMLMap.InitAndSetArrayItem(XVar.Array(), this.tName);
			this.viewControlsHTMLMap.InitAndSetArrayItem(XVar.Array(), this.tName, this.pageType);
			this.viewControlsHTMLMap.InitAndSetArrayItem(XVar.Array(), this.tName, this.pageType, this.id);
			foreach (KeyValuePair<XVar, dynamic> val in this.viewControlsMap.GetEnumerator())
			{
				this.viewControlsHTMLMap.InitAndSetArrayItem(val.Value, this.tName, this.pageType, this.id, val.Key);
			}

			return null;
		}
		public virtual XVar fillSetCntrlMaps()
		{
			if(XVar.Pack(this.isControlsMapFilled))
			{
				return null;
			}
			this.fillSettings();
			this.fillControlsHTMLMap();
			this.isControlsMapFilled = new XVar(true);

			return null;
		}
		public virtual XVar assignBodyEnd(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic projectBuildKey = null, wizardBuildKey = null;
			this.fillSetCntrlMaps();
			MVCFunctions.Echo(MVCFunctions.Concat("<script>\r\n\t\t\twindow.controlsMap = ", MVCFunctions.runner_json_encode((XVar)(this.controlsHTMLMap)), ";\r\n\t\t\twindow.viewControlsMap = ", MVCFunctions.runner_json_encode((XVar)(this.viewControlsHTMLMap)), ";\r\n\t\t\twindow.settings = ", MVCFunctions.runner_json_encode((XVar)(this.jsSettings)), ";\r\n\t\t\tRunner.applyPagesData( ", MVCFunctions.runner_json_encode((XVar)(GlobalVars.pagesData)), " );\r\n\t\t\t</script>\r\n"));
			projectBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("projectBuild")));
			wizardBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("wizardBuild")));
			MVCFunctions.Echo(MVCFunctions.Concat("<script language=\"JavaScript\" src=\"settings/project.js?", projectBuildKey, "\"></script>", "\r\n"));
			MVCFunctions.Echo(MVCFunctions.Concat("<script language=\"JavaScript\" src=\"include/runnerJS/RunnerAll.js?", wizardBuildKey, "\"></script>", "\r\n"));
			MVCFunctions.Echo(MVCFunctions.Concat("<script>", this.PrepareJS(), "</script>"));

			return null;
		}
		public virtual XVar genId()
		{
			this.flyId++;
			this.recId = XVar.Clone(this.flyId);
			return this.flyId;
		}
		public virtual XVar getPageType()
		{
			return this.pageType;
		}
		public virtual XVar AddJSFileNoExt(dynamic _param_file)
		{
			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			#endregion

			this.includes_js.InitAndSetArrayItem(file, null);

			return null;
		}
		public virtual XVar AddJSFile(dynamic _param_file, dynamic _param_req1 = null, dynamic _param_req2 = null, dynamic _param_req3 = null)
		{
			#region default values
			if(_param_req1 as Object == null) _param_req1 = new XVar("");
			if(_param_req2 as Object == null) _param_req2 = new XVar("");
			if(_param_req3 as Object == null) _param_req3 = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			dynamic req1 = XVar.Clone(_param_req1);
			dynamic req2 = XVar.Clone(_param_req2);
			dynamic req3 = XVar.Clone(_param_req3);
			#endregion

			dynamic rootPath = null;
			rootPath = XVar.Clone(file);
			this.includes_js.InitAndSetArrayItem(rootPath, null);
			if(req1 != XVar.Pack(""))
			{
				this.includes_jsreq.InitAndSetArrayItem(new XVar(0, req1), rootPath);
			}
			if(req2 != XVar.Pack(""))
			{
				this.includes_jsreq.InitAndSetArrayItem(req2, rootPath, null);
			}
			if(req3 != XVar.Pack(""))
			{
				this.includes_jsreq.InitAndSetArrayItem(req3, rootPath, null);
			}

			return null;
		}
		public virtual XVar grabAllJsFiles()
		{
			dynamic jsFiles = XVar.Array();
			jsFiles = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> file in this.includes_js.GetEnumerator())
			{
				jsFiles.InitAndSetArrayItem(XVar.Array(), file.Value);
				if(XVar.Pack(this.includes_jsreq.KeyExists(file.Value)))
				{
					jsFiles.InitAndSetArrayItem(this.includes_jsreq[file.Value], file.Value);
				}
			}
			this.includes_js = XVar.Clone(XVar.Array());
			this.includes_jsreq = XVar.Clone(XVar.Array());
			return jsFiles;
		}
		public virtual XVar copyAllJsFiles(dynamic _param_jsFiles)
		{
			#region pass-by-value parameters
			dynamic jsFiles = XVar.Clone(_param_jsFiles);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> reqFiles in jsFiles.GetEnumerator())
			{
				this.includes_js.InitAndSetArrayItem(reqFiles.Key, null);
				if(XVar.Pack(this.includes_jsreq.KeyExists(reqFiles.Key)))
				{
					foreach (KeyValuePair<XVar, dynamic> rFile in reqFiles.Value.GetEnumerator())
					{
						if(XVar.Pack(this.includes_jsreq[reqFiles.Key].KeyExists(rFile.Value)))
						{
							continue;
						}
						this.includes_jsreq.InitAndSetArrayItem(rFile.Value, reqFiles.Key, null);
					}
				}
				else
				{
					this.includes_jsreq.InitAndSetArrayItem(reqFiles.Value, reqFiles.Key);
				}
			}

			return null;
		}
		public virtual XVar AddCSSFile(dynamic _param_file)
		{
			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			#endregion

			if(XVar.Pack(MVCFunctions.is_array((XVar)(file))))
			{
				foreach (KeyValuePair<XVar, dynamic> f in file.GetEnumerator())
				{
					this.includes_css.InitAndSetArrayItem(f.Value, null);
				}
			}
			else
			{
				this.includes_css.InitAndSetArrayItem(file, null);
			}

			return null;
		}
		public virtual XVar successPageType()
		{
			switch(((XVar)this.pageType).ToString())
			{
				case Constants.PAGE_CHANGEPASS:
					return "changepwd_success";
				case Constants.PAGE_REGISTER:
					return "register_success";
				case Constants.PAGE_REMIND:
					return "remind_success";
			}
			return this.pageType;
		}
		public virtual XVar switchToSuccessPage()
		{
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), (XVar)(this.pageName), new XVar(Constants.GLOBAL_PAGES)));
			this.pageName = XVar.Clone(this.pSet.getDefaultPage((XVar)(this.successPageType())));
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), (XVar)(this.pageName), new XVar(Constants.GLOBAL_PAGES)));
			this.templatefile = new XVar("");
			this.setTemplateFile();
			this.pageLayout = XVar.Clone(CommonFunctions.GetPageLayout(new XVar(Constants.GLOBAL_PAGES), (XVar)(this.pageName)));

			return null;
		}
		public virtual XVar grabAllCSSFiles()
		{
			dynamic cssFiles = null;
			cssFiles = XVar.Clone(this.includes_css);
			this.includes_css = XVar.Clone(XVar.Array());
			return cssFiles;
		}
		public virtual XVar copyAllCssFiles(dynamic _param_cssFiles)
		{
			#region pass-by-value parameters
			dynamic cssFiles = XVar.Clone(_param_cssFiles);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> file in cssFiles.GetEnumerator())
			{
				this.AddCSSFile((XVar)(file.Value));
			}

			return null;
		}
		public virtual XVar LoadJS_CSS()
		{
			dynamic var_out = null;
			this.includes_js = XVar.Clone(MVCFunctions.array_unique((XVar)(this.includes_js)));
			this.includes_css = XVar.Clone(MVCFunctions.array_unique((XVar)(this.includes_css)));
			var_out = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> file in this.includes_js.GetEnumerator())
			{
				var_out = MVCFunctions.Concat(var_out, "Runner.util.ScriptLoader.addJS(['", file.Value, "']");
				if(XVar.Pack(this.includes_jsreq.KeyExists(file.Value)))
				{
					foreach (KeyValuePair<XVar, dynamic> req in this.includes_jsreq[file.Value].GetEnumerator())
					{
						var_out = MVCFunctions.Concat(var_out, ",'", req.Value, "'");
					}
				}
				var_out = MVCFunctions.Concat(var_out, ");\r\n");
			}
			var_out = MVCFunctions.Concat(var_out, " Runner.util.ScriptLoader.load();");
			return var_out;
		}
		public virtual XVar setLangParams()
		{
			if(1 < MVCFunctions.count(ProjectSettings.languageDescriptors()))
			{
				CommonFunctions.SetLangVars((XVar)(this.xt), (XVar)(this.shortTableName), (XVar)(this.pageType));
			}

			return null;
		}
		public static XVar addCommonJs(dynamic instance)
		{
			if(XVar.Pack(instance.pSet.hasJsEvents()))
			{
				instance.AddJSFile((XVar)(MVCFunctions.Concat("usercode/pageevents_", CommonFunctions.GetTableURL((XVar)(instance.pageTable)), ".js")));
			}
			if((XVar)(instance.pSet.pageHasCharts())  || (XVar)(instance.pSet.getEntityType() == Constants.titDASHBOARD))
			{
				instance.AddJSFile(new XVar(MVCFunctions.Concat("usercode/pageevents_", Constants.GLOBAL_PAGES_SHORT, ".js")));
			}
			instance.loadRTE();
			instance.addControlsJSAndCSS();
			instance.AddCSSFile(new XVar("styles/bundle.css"));

			return null;
		}
		public virtual XVar addCommonJs()
		{
			if(XVar.Pack(this.pSet.hasJsEvents()))
			{
				this.AddJSFile((XVar)(MVCFunctions.Concat("usercode/pageevents_", CommonFunctions.GetTableURL((XVar)(this.pageTable)), ".js")));
			}
			if((XVar)(this.pSet.pageHasCharts())  || (XVar)(this.pSet.getEntityType() == Constants.titDASHBOARD))
			{
				this.AddJSFile(new XVar(MVCFunctions.Concat("usercode/pageevents_", Constants.GLOBAL_PAGES_SHORT, ".js")));
			}
			this.loadRTE();
			this.addControlsJSAndCSS();
			this.AddCSSFile(new XVar("styles/bundle.css"));

			return null;
		}
		protected virtual XVar loadRTE()
		{
			dynamic rteType = null;
			if(XVar.Pack(!(XVar)(this.useRichText())))
			{
				return null;
			}
			rteType = XVar.Clone(ProjectSettings.rteType());
			if(rteType == Constants.rteCKEditor)
			{
				this.AddJSFile(new XVar("plugins/ckeditor/ckeditor.js"));
			}
			else
			{
				if(rteType == Constants.rteCKEditor5)
				{
					this.AddJSFile(new XVar("plugins/ckeditor5/ckeditor5.js"));
				}
			}

			return null;
		}
		public virtual XVar addControlsJSAndCSS()
		{
			this.controls.addControlsJSAndCSS();
			this.viewControls.addControlsJSAndCSS();

			return null;
		}
		public virtual XVar PrepareJS()
		{
			return this.LoadJS_CSS();
		}
		public virtual XVar addButtonHandlers()
		{
			dynamic shortTableName = null;
			shortTableName = XVar.Clone(CommonFunctions.GetTableURL((XVar)(this.pageTable)));
			if((XVar)(!(XVar)(this.pSet.hasJsEvents()))  || (XVar)(shortTableName == XVar.Pack("")))
			{
				return false;
			}
			this.AddJSFile((XVar)(MVCFunctions.Concat("usercode/pageevents_", shortTableName, ".js")));
			return true;
		}
		public virtual XVar setGoogleMapsParams(dynamic _param_fieldsArr)
		{
			#region pass-by-value parameters
			dynamic fieldsArr = XVar.Clone(_param_fieldsArr);
			#endregion

			this.googleMapCfg.InitAndSetArrayItem(this.pSet.hasMap(), "isUseMainMaps");
			this.googleMapCfg.InitAndSetArrayItem(this.pSet.isUseFieldsMaps(), "isUseFieldsMaps");
			if(XVar.Pack(this.googleMapCfg["isUseFieldsMaps"]))
			{
				foreach (KeyValuePair<XVar, dynamic> f in fieldsArr.GetEnumerator())
				{
					if(f.Value["viewFormat"] == Constants.FORMAT_MAP)
					{
						dynamic fieldMap = XVar.Array();
						this.googleMapCfg.InitAndSetArrayItem(XVar.Array(), "fieldsAsMap", f.Value["fName"]);
						fieldMap = XVar.Clone(this.pSet.getMapData((XVar)(f.Value["fName"])));
						this.googleMapCfg.InitAndSetArrayItem((XVar.Pack(fieldMap["width"]) ? XVar.Pack(fieldMap["width"]) : XVar.Pack(0)), "fieldsAsMap", f.Value["fName"], "width");
						this.googleMapCfg.InitAndSetArrayItem((XVar.Pack(fieldMap["height"]) ? XVar.Pack(fieldMap["height"]) : XVar.Pack(0)), "fieldsAsMap", f.Value["fName"], "height");
						this.googleMapCfg.InitAndSetArrayItem(fieldMap["address"], "fieldsAsMap", f.Value["fName"], "addressField");
						this.googleMapCfg.InitAndSetArrayItem(fieldMap["lat"], "fieldsAsMap", f.Value["fName"], "latField");
						this.googleMapCfg.InitAndSetArrayItem(fieldMap["lng"], "fieldsAsMap", f.Value["fName"], "lngField");
						this.googleMapCfg.InitAndSetArrayItem(fieldMap["desc"], "fieldsAsMap", f.Value["fName"], "descField");
						this.googleMapCfg.InitAndSetArrayItem(this.pSet.getMapIcon((XVar)(f.Value["fName"]), (XVar)(this.data)), "fieldsAsMap", f.Value["fName"], "mapIcon");
						if(XVar.Pack(fieldMap.KeyExists("zoom")))
						{
							this.googleMapCfg.InitAndSetArrayItem(fieldMap["zoom"], "fieldsAsMap", f.Value["fName"], "zoom");
						}
					}
				}
			}
			this.googleMapCfg.InitAndSetArrayItem((XVar)(this.googleMapCfg["isUseMainMaps"])  || (XVar)(this.googleMapCfg["isUseFieldsMaps"]), "isUseGoogleMap");
			this.googleMapCfg.InitAndSetArrayItem(this.tName, "tName");

			return null;
		}
		public virtual XVar fillAdvancedMapData()
		{
			dynamic advMaps = XVar.Array(), clustering = null, data = XVar.Array(), dc = null, editlink = null, i = null, keys = XVar.Array(), recId = null, rs = null, tKeys = XVar.Array();
			advMaps = XVar.Clone(XVar.Array());
			clustering = new XVar(false);
			foreach (KeyValuePair<XVar, dynamic> mapData in this.googleMapCfg["mapsData"].GetEnumerator())
			{
				if(XVar.Pack(this.googleMapCfg["mapsData"][mapData.Key]["showAllMarkers"]))
				{
					advMaps.InitAndSetArrayItem(mapData.Key, null);
				}
				if((XVar)(this.googleMapCfg["mapsData"][mapData.Key]["clustering"])  && (XVar)(this.mapProvider == Constants.GOOGLE_MAPS))
				{
					clustering = new XVar(true);
				}
			}
			if(XVar.Pack(!(XVar)(advMaps)))
			{
				return null;
			}
			if(XVar.Pack(clustering))
			{
				this.AddJSFile(new XVar("include/markerclusterer.js"));
			}
			tKeys = XVar.Clone(this.pSet.getTableKeys());
			dc = XVar.Clone(this.queryCommand);
			if(XVar.Pack(!(XVar)(dc)))
			{
				dc = XVar.Clone(this.getSubsetDataCommand());
			}
			dc.reccount = XVar.Clone(-1);
			dc.startRecord = new XVar(0);
			rs = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			recId = XVar.Clone(this.recId);
			while(XVar.Pack(data = XVar.Clone(rs.fetchAssoc())))
			{
				editlink = new XVar("");
				keys = XVar.Clone(XVar.Array());
				i = new XVar(0);
				for(;i < MVCFunctions.count(tKeys); i++)
				{
					if(i != XVar.Pack(0))
					{
						editlink = MVCFunctions.Concat(editlink, "&");
					}
					editlink = MVCFunctions.Concat(editlink, "editid", i + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(data[tKeys[i]])))));
					keys.InitAndSetArrayItem(data[tKeys[i]], i);
				}
				foreach (KeyValuePair<XVar, dynamic> mapId in advMaps.GetEnumerator())
				{
					this.addBigGoogleMapMarker((XVar)(mapId.Value), (XVar)(data), (XVar)(keys), (XVar)(++(recId)), (XVar)(editlink));
				}
			}

			return null;
		}
		public virtual XVar addBigGoogleMapMarkers(dynamic data, dynamic _param_keys, dynamic _param_editLink = null)
		{
			#region default values
			if(_param_editLink as Object == null) _param_editLink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			dynamic editLink = XVar.Clone(_param_editLink);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> mapId in this.googleMapCfg["mainMapIds"].GetEnumerator())
			{
				if(XVar.Pack(this.fetchMapMarkersInSeparateQuery((XVar)(mapId.Value))))
				{
					continue;
				}
				this.addBigGoogleMapMarker((XVar)(mapId.Value), (XVar)(data), (XVar)(keys), (XVar)(this.recId), (XVar)(editLink));
			}

			return null;
		}
		public virtual XVar fetchMapMarkersInSeparateQuery(dynamic _param_mapId)
		{
			#region pass-by-value parameters
			dynamic mapId = XVar.Clone(_param_mapId);
			#endregion

			return (XVar)((XVar)(this.googleMapCfg["mapsData"][mapId]["heatMap"])  || (XVar)(this.googleMapCfg["mapsData"][mapId]["clustering"]))  && (XVar)((XVar)((XVar)(this.mapProvider == Constants.GOOGLE_MAPS)  || (XVar)(this.mapProvider == Constants.HERE_MAPS))  || (XVar)(this.mapProvider == Constants.MAPQUEST_MAPS));
		}
		public virtual XVar addBigGoogleMapMarker(dynamic _param_mapId, dynamic data, dynamic _param_keys, dynamic _param_recId, dynamic _param_editLink = null)
		{
			#region default values
			if(_param_editLink as Object == null) _param_editLink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic mapId = XVar.Clone(_param_mapId);
			dynamic keys = XVar.Clone(_param_keys);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic editLink = XVar.Clone(_param_editLink);
			#endregion

			dynamic addressF = null, descF = null, latF = null, lngF = null, markerArr = XVar.Array(), markerAsEditLink = null, weightF = null;
			latF = XVar.Clone(this.googleMapCfg["mapsData"][mapId]["latField"]);
			lngF = XVar.Clone(this.googleMapCfg["mapsData"][mapId]["lngField"]);
			addressF = XVar.Clone(this.googleMapCfg["mapsData"][mapId]["addressField"]);
			if((XVar)((XVar)(!(XVar)(MVCFunctions.strlen((XVar)(data[latF]))))  && (XVar)(!(XVar)(MVCFunctions.strlen((XVar)(data[lngF])))))  && (XVar)(!(XVar)(MVCFunctions.strlen((XVar)(data[addressF])))))
			{
				return null;
			}
			descF = XVar.Clone(this.googleMapCfg["mapsData"][mapId]["descField"]);
			markerAsEditLink = XVar.Clone(this.googleMapCfg["mapsData"][mapId]["markerAsEditLink"]);
			weightF = XVar.Clone(this.googleMapCfg["mapsData"][mapId]["weightField"]);
			markerArr = XVar.Clone(XVar.Array());
			markerArr.InitAndSetArrayItem(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)((XVar.Pack(data[latF]) ? XVar.Pack(data[latF]) : XVar.Pack("")))), "lat");
			markerArr.InitAndSetArrayItem(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)((XVar.Pack(data[lngF]) ? XVar.Pack(data[lngF]) : XVar.Pack("")))), "lng");
			markerArr.InitAndSetArrayItem((XVar.Pack(data[addressF]) ? XVar.Pack(data[addressF]) : XVar.Pack("")), "address");
			markerArr.InitAndSetArrayItem((XVar.Pack(data[descF]) ? XVar.Pack(data[descF]) : XVar.Pack(markerArr["address"])), "desc");
			if(XVar.Pack(weightF))
			{
				markerArr.InitAndSetArrayItem(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)((XVar.Pack(data[weightF]) ? XVar.Pack(data[weightF]) : XVar.Pack("")))), "weight");
			}
			if((XVar)(markerAsEditLink)  && (XVar)(this.editAvailable()))
			{
				markerArr.InitAndSetArrayItem(MVCFunctions.GetTableLink((XVar)(this.shortTableName), new XVar("edit"), (XVar)(MVCFunctions.Concat(editLink, "&", this.getStateUrlParams()))), "link");
			}
			else
			{
				if(XVar.Pack(this.viewAvailable()))
				{
					markerArr.InitAndSetArrayItem(MVCFunctions.GetTableLink((XVar)(this.shortTableName), new XVar("view"), (XVar)(MVCFunctions.Concat(editLink, "&", this.getStateUrlParams()))), "link");
				}
			}
			markerArr.InitAndSetArrayItem(recId, "recId");
			markerArr.InitAndSetArrayItem(keys, "keys");
			if(XVar.Pack(this.googleMapCfg["mapsData"][mapId]["dashMap"]))
			{
				markerArr.InitAndSetArrayItem(this.dashSet.getDashMapIcon((XVar)(this.dashElementName), (XVar)(data)), "mapIcon");
				markerArr.InitAndSetArrayItem(this.getDetailTablesMasterKeys((XVar)(data)), "masterKeys");
			}
			else
			{
				if(XVar.Pack(this.googleMapCfg["mapsData"][mapId]["markerField"]))
				{
					markerArr.InitAndSetArrayItem(data[this.googleMapCfg["mapsData"][mapId]["markerField"]], "mapIcon");
				}
				if((XVar)(!(XVar)(markerArr["mapIcon"]))  && (XVar)(this.googleMapCfg["mapsData"][mapId]["markerIcon"]))
				{
					markerArr.InitAndSetArrayItem(this.googleMapCfg["mapsData"][mapId]["markerIcon"], "mapIcon");
				}
			}
			this.googleMapCfg.InitAndSetArrayItem(markerArr, "mapsData", mapId, "markers", null);

			return null;
		}
		protected virtual XVar getDetailTablesMasterKeys(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic masterKeys = XVar.Array();
			masterKeys = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				dynamic detailsKeys = XVar.Array();
				if(XVar.Pack(!(XVar)(this.permis[details.Value]["search"])))
				{
					continue;
				}
				masterKeys.InitAndSetArrayItem(XVar.Array(), details.Value);
				detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(details.Value)));
				foreach (KeyValuePair<XVar, dynamic> m in detailsKeys["masterKeys"].GetEnumerator())
				{
					dynamic curM = null;
					curM = XVar.Clone(m.Value);
					if(this.pageType == Constants.PAGE_REPORT)
					{
						curM = XVar.Clone(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(curM)), "_dbvalue"));
					}
					masterKeys.InitAndSetArrayItem(data[curM], details.Value, MVCFunctions.Concat("masterkey", m.Key + 1));
				}
			}
			return masterKeys;
		}
		public virtual XVar addGoogleMapData(dynamic _param_fName, dynamic data, dynamic _param_keys = null, dynamic _param_editLink = null)
		{
			#region default values
			if(_param_keys as Object == null) _param_keys = new XVar(XVar.Array());
			if(_param_editLink as Object == null) _param_editLink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic keys = XVar.Clone(_param_keys);
			dynamic editLink = XVar.Clone(_param_editLink);
			#endregion

			dynamic address = null, desc = null, fieldMap = XVar.Array(), lat = null, lng = null, mapData = XVar.Array(), mapId = null, viewLink = null;
			fieldMap = XVar.Clone(this.pSet.getMapData((XVar)(fName)));
			mapData = XVar.Clone(XVar.Array());
			mapData.InitAndSetArrayItem(fName, "fName");
			mapData.InitAndSetArrayItem((XVar.Pack(fieldMap.KeyExists("zoom")) ? XVar.Pack(fieldMap["zoom"]) : XVar.Pack("")), "zoom");
			mapData.InitAndSetArrayItem("FIELD_MAP", "type");
			mapData.InitAndSetArrayItem(data[fName], "mapFieldValue");
			address = XVar.Clone((XVar.Pack(data[fieldMap["address"]]) ? XVar.Pack(data[fieldMap["address"]]) : XVar.Pack("")));
			lat = XVar.Clone(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)((XVar.Pack(data[fieldMap["lat"]]) ? XVar.Pack(data[fieldMap["lat"]]) : XVar.Pack("")))));
			lng = XVar.Clone(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)((XVar.Pack(data[fieldMap["lng"]]) ? XVar.Pack(data[fieldMap["lng"]]) : XVar.Pack("")))));
			desc = XVar.Clone((XVar.Pack(data[fieldMap["desc"]]) ? XVar.Pack(data[fieldMap["desc"]]) : XVar.Pack(address)));
			viewLink = new XVar("");
			if((XVar)(this.pageType != Constants.PAGE_VIEW)  && (XVar)(this.viewAvailable()))
			{
				viewLink = XVar.Clone(MVCFunctions.GetTableLink((XVar)(this.shortTableName), new XVar("view"), (XVar)(MVCFunctions.Concat(editLink, "&", this.getStateUrlParams()))));
			}
			mapData.InitAndSetArrayItem(new XVar("address", address, "lat", lat, "lng", lng, "link", viewLink, "desc", desc, "recId", this.recId, "keys", keys, "mapIcon", this.pSet.getMapIcon((XVar)(fName), (XVar)(data))), "markers", null);
			mapId = XVar.Clone(MVCFunctions.Concat("littleMap_", MVCFunctions.GoodFieldName((XVar)(fName)), "_", this.recId));
			this.googleMapCfg.InitAndSetArrayItem(mapData, "mapsData", mapId);
			this.googleMapCfg.InitAndSetArrayItem(mapId, "fieldMapsIds", null);
			return this.googleMapCfg["mapsData"][mapId];
		}
		protected virtual XVar getDashMapsIconsData(dynamic data)
		{
			dynamic mapIconsData = XVar.Array();
			mapIconsData = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(this.dashTName)))
			{
				return mapIconsData;
			}
			foreach (KeyValuePair<XVar, dynamic> dElem in this.dashSet.getDashboardElements().GetEnumerator())
			{
				if((XVar)(dElem.Value["table"] != this.tName)  || (XVar)(dElem.Value["type"] != Constants.DASHBOARD_MAP))
				{
					continue;
				}
				mapIconsData.InitAndSetArrayItem(this.dashSet.getDashMapIcon((XVar)(dElem.Value["elementName"]), (XVar)(data)), dElem.Value["elementName"]);
			}
			return mapIconsData;
		}
		protected virtual XVar getFieldMapIconsData(dynamic data)
		{
			dynamic iconsData = XVar.Array();
			iconsData = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.pSet.isUseFieldsMaps()))
			{
				foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getFieldsList().GetEnumerator())
				{
					if(this.pSet.getViewFormat((XVar)(f.Value)) == Constants.FORMAT_MAP)
					{
						iconsData.InitAndSetArrayItem(this.pSet.getMapIcon((XVar)(f.Value), (XVar)(data)), f.Value);
					}
				}
			}
			return iconsData;
		}
		public virtual XVar initGmaps()
		{
			if(XVar.Pack(!(XVar)(this.googleMapCfg["isUseGoogleMap"])))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> mapId in this.googleMapCfg["mainMapIds"].GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(this.googleMapCfg["mapsData"][mapId.Value]["showCenterLink"]), XVar.Pack(1)))
				{
					this.googleMapCfg.InitAndSetArrayItem(this.googleMapCfg["mapsData"][mapId.Value]["centerLinkText"], "centerLinkText");
					break;
				}
			}
			this.includeOSMfile();
			this.AddJSFile((XVar)(MVCFunctions.Concat(MVCFunctions.projectPath(), "include/runnerJS/MapManager.js")), new XVar("include/runnerJS/ControlConstants.js"));
			this.AddJSFile((XVar)(MVCFunctions.Concat("include/runnerJS/", this.getIncludeFileMapProvider())), (XVar)(MVCFunctions.Concat(MVCFunctions.projectPath(), "include/runnerJS/MapManager.js")));
			this.googleMapCfg.InitAndSetArrayItem(this.id, "id");
			if(XVar.Pack(!(XVar)(this.googleMapCfg["APIcode"])))
			{
				this.googleMapCfg.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("mapSettings"), new XVar("apikey")), "APIcode");
			}
			this.controlsMap.InitAndSetArrayItem(this.googleMapCfg, "gMaps");

			return null;
		}
		public virtual XVar getGeoCoordinates(dynamic _param_address)
		{
			#region pass-by-value parameters
			dynamic address = XVar.Clone(_param_address);
			#endregion

			return CommonFunctions.getLatLngByAddr((XVar)(address));
		}
		public virtual XVar glueAddressByAddressFields(dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			#endregion

			dynamic address = null, geoData = XVar.Array();
			address = new XVar("");
			geoData = XVar.Clone(this.pSet.getGeocodingData());
			foreach (KeyValuePair<XVar, dynamic> field in geoData["addressFields"].GetEnumerator())
			{
				dynamic addressField = null;
				addressField = XVar.Clone(MVCFunctions.trim((XVar)(values[field.Value])));
				if((XVar)(values.KeyExists(field.Value))  && (XVar)(MVCFunctions.strlen((XVar)(addressField))))
				{
					address = MVCFunctions.Concat(address, addressField, " ");
				}
			}
			return MVCFunctions.trim((XVar)(address));
		}
		public virtual XVar setUpdatedLatLng(dynamic values, dynamic _param_oldvalues = null)
		{
			#region default values
			if(_param_oldvalues as Object == null) _param_oldvalues = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic oldvalues = XVar.Clone(_param_oldvalues);
			#endregion

			dynamic address = null, location = XVar.Array(), mapData = XVar.Array(), oldaddress = null;
			if(XVar.Pack(!(XVar)(this.pSet.isUpdateLatLng())))
			{
				return null;
			}
			mapData = XVar.Clone(this.pSet.getGeocodingData());
			address = XVar.Clone(this.glueAddressByAddressFields((XVar)(values)));
			if(address == XVar.Pack(""))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(oldvalues), XVar.Pack(null)))))
			{
				oldaddress = XVar.Clone(this.glueAddressByAddressFields((XVar)(oldvalues)));
			}
			else
			{
				if((XVar)(MVCFunctions.trim((XVar)(values[mapData["latField"]])) != "")  && (XVar)(MVCFunctions.trim((XVar)(values[mapData["lngField"]])) != ""))
				{
					return null;
				}
			}
			if((XVar)((XVar)((XVar)(oldvalues)  && (XVar)(MVCFunctions.trim((XVar)(oldvalues[mapData["latField"]])) != ""))  && (XVar)(MVCFunctions.trim((XVar)(oldvalues[mapData["lngField"]])) != ""))  && (XVar)(address == oldaddress))
			{
				return null;
			}
			location = XVar.Clone(this.getGeoCoordinates((XVar)(address)));
			if(XVar.Pack(!(XVar)(location)))
			{
				return null;
			}
			values.InitAndSetArrayItem(location["lat"], mapData["latField"]);
			values.InitAndSetArrayItem(location["lng"], mapData["lngField"]);

			return null;
		}
		protected virtual XVar getMapCondition()
		{
			dynamic tGrid = null;
			if((XVar)(!(XVar)(this.mapRefresh))  || (XVar)(!(XVar)(this.vpCoordinates)))
			{
				return null;
			}
			tGrid = XVar.Clone(this.hasTableDashGridElement());
			foreach (KeyValuePair<XVar, dynamic> dElem in this.dashSet.getDashboardElements().GetEnumerator())
			{
				if((XVar)((XVar)(dElem.Value["table"] == this.tName)  && (XVar)(dElem.Value["type"] == Constants.DASHBOARD_MAP))  && (XVar)((XVar)(dElem.Value["updateMoved"])  || (XVar)(!(XVar)(tGrid))))
				{
					return this.getLatLngCondition((XVar)(dElem.Value["latF"]), (XVar)(dElem.Value["lonF"]));
				}
			}
			return null;
		}
		protected virtual XVar getLatLngCondition(dynamic _param_latFName, dynamic _param_lngFName)
		{
			#region pass-by-value parameters
			dynamic latFName = XVar.Clone(_param_latFName);
			dynamic lngFName = XVar.Clone(_param_lngFName);
			#endregion

			dynamic conditions = XVar.Array(), e = null, n = null, s = null, w = null;
			if(XVar.Pack(this.skipMapFilter))
			{
				return null;
			}
			if((XVar)(!(XVar)(this.mapRefresh))  || (XVar)(!(XVar)(this.vpCoordinates)))
			{
				return null;
			}
			s = XVar.Clone(this.vpCoordinates["s"]);
			n = XVar.Clone(this.vpCoordinates["n"]);
			w = XVar.Clone(this.vpCoordinates["w"]);
			e = XVar.Clone(this.vpCoordinates["e"]);
			conditions = XVar.Clone(XVar.Array());
			conditions.InitAndSetArrayItem(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(latFName), new XVar(Constants.dsopLESS), (XVar)(s)))), null);
			conditions.InitAndSetArrayItem(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(latFName), new XVar(Constants.dsopMORE), (XVar)(n)))), null);
			if(w <= e)
			{
				conditions.InitAndSetArrayItem(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(lngFName), new XVar(Constants.dsopLESS), (XVar)(w)))), null);
				conditions.InitAndSetArrayItem(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(lngFName), new XVar(Constants.dsopMORE), (XVar)(e)))), null);
			}
			else
			{
				conditions.InitAndSetArrayItem(DataCondition._Or((XVar)(new XVar(0, DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(lngFName), new XVar(Constants.dsopLESS), (XVar)(w)))), 1, DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(lngFName), new XVar(Constants.dsopMORE), (XVar)(e))))))), null);
			}
			return DataCondition._And((XVar)(conditions));
		}
		protected virtual XVar getLatLngWhere(dynamic _param_latFName, dynamic _param_lngFName)
		{
			#region pass-by-value parameters
			dynamic latFName = XVar.Clone(_param_latFName);
			dynamic lngFName = XVar.Clone(_param_lngFName);
			#endregion

			dynamic e = null, latSQLName = null, lngSQLName = null, n = null, s = null, w = null;
			if(XVar.Pack(this.skipMapFilter))
			{
				return "";
			}
			if((XVar)(!(XVar)(this.mapRefresh))  || (XVar)(!(XVar)(this.vpCoordinates)))
			{
				return "";
			}
			latSQLName = XVar.Clone(this.getFieldSQLDecrypt((XVar)(latFName)));
			lngSQLName = XVar.Clone(this.getFieldSQLDecrypt((XVar)(lngFName)));
			s = XVar.Clone(this.cipherer.MakeDBValue((XVar)(latFName), (XVar)(this.vpCoordinates["s"]), new XVar(""), new XVar(true)));
			n = XVar.Clone(this.cipherer.MakeDBValue((XVar)(latFName), (XVar)(this.vpCoordinates["n"]), new XVar(""), new XVar(true)));
			w = XVar.Clone(this.cipherer.MakeDBValue((XVar)(lngFName), (XVar)(this.vpCoordinates["w"]), new XVar(""), new XVar(true)));
			e = XVar.Clone(this.cipherer.MakeDBValue((XVar)(lngFName), (XVar)(this.vpCoordinates["e"]), new XVar(""), new XVar(true)));
			if(this.vpCoordinates["w"] <= this.vpCoordinates["e"])
			{
				return MVCFunctions.Concat(latSQLName, ">=", s, " AND ", latSQLName, "<=", n, " AND ", lngSQLName, "<=", e, " AND ", lngSQLName, ">=", w);
			}
			else
			{
				return MVCFunctions.Concat(latSQLName, ">=", s, " AND ", latSQLName, "<=", n, " AND (", lngSQLName, "<=", e, " OR ", lngSQLName, ">=", w, ")");
			}

			return null;
		}
		protected virtual XVar getPageFields()
		{
			return this.pSet.getFieldsList();
		}
		public virtual XVar getPermissions(dynamic _param_tName = null)
		{
			#region default values
			if(_param_tName as Object == null) _param_tName = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic tName = XVar.Clone(_param_tName);
			#endregion

			dynamic resArr = XVar.Array(), strPerm = null;
			resArr = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(tName)))
			{
				tName = XVar.Clone(this.tName);
			}
			if(XVar.Equals(XVar.Pack(tName), XVar.Pack(Constants.GLOBAL_PAGES)))
			{
				return new XVar("search", true);
			}
			strPerm = XVar.Clone(CommonFunctions.GetUserPermissions((XVar)(tName)));
			if(XVar.Pack(CommonFunctions.isLogged()))
			{
				resArr.InitAndSetArrayItem(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("A"))), XVar.Pack(false)), "add");
				resArr.InitAndSetArrayItem(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("D"))), XVar.Pack(false)), "delete");
				resArr.InitAndSetArrayItem(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("E"))), XVar.Pack(false)), "edit");
			}
			resArr.InitAndSetArrayItem(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("S"))), XVar.Pack(false)), "search");
			resArr.InitAndSetArrayItem(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("P"))), XVar.Pack(false)), "export");
			resArr.InitAndSetArrayItem(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("I"))), XVar.Pack(false)), "import");
			return resArr;
		}
		public virtual XVar eventExists(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(XVar.Pack(!(XVar)(this.eventsObject)))
			{
				return false;
			}
			return this.eventsObject.exists((XVar)(name));
		}
		public virtual XVar events()
		{
			return this.eventsObject;
		}
		public virtual XVar mapsExists()
		{
			return this.pSet.hasMap();
		}
		protected virtual XVar hasTableDashGridElement()
		{
			if(XVar.Pack(!(XVar)(this.dashSet)))
			{
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> dElem in this.dashSet.getDashboardElements().GetEnumerator())
			{
				if((XVar)(dElem.Value["table"] == this.tName)  && (XVar)(dElem.Value["type"] == Constants.DASHBOARD_LIST))
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar hasDashMapElement()
		{
			if(XVar.Pack(!(XVar)(this.dashSet)))
			{
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> dElem in this.dashSet.getDashboardElements().GetEnumerator())
			{
				if((XVar)(dElem.Value["table"] == this.tName)  && (XVar)(dElem.Value["type"] == Constants.DASHBOARD_MAP))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar getNextPrevQueryComponents(dynamic data)
		{
			dynamic equal = null, i = null, nextOrder = XVar.Array(), nextWhere = null, of = XVar.Array(), orderClause = null, orderFields = XVar.Array(), prev = null, prevOrder = XVar.Array(), prevWhere = null, query = null, sqlColumn = null, sqlValue = null, var_next = null;
			orderClause = XVar.Clone(OrderClause.createFromPage(this, new XVar(false)));
			orderFields = orderClause.getOrderFields();
			query = XVar.Clone(this.pSet.getSQLQuery());
			if((XVar)(!(XVar)(orderFields))  || (XVar)(!(XVar)(query)))
			{
				return XVar.Array();
			}
			nextWhere = new XVar("");
			prevWhere = new XVar("");
			nextOrder = XVar.Clone(XVar.Array());
			prevOrder = XVar.Clone(XVar.Array());
			i = XVar.Clone(MVCFunctions.count(orderFields) - 1);
			for(;XVar.Pack(0) <= i; --(i))
			{
				of = XVar.Clone(orderFields[i]);
				sqlColumn = XVar.Clone(this.connection.addFieldWrappers((XVar)(of["column"])));
				nextOrder.InitAndSetArrayItem(MVCFunctions.Concat(sqlColumn, " ", of["dir"]), null);
				prevOrder.InitAndSetArrayItem(MVCFunctions.Concat(sqlColumn, " ", (XVar.Pack(of["dir"] == "ASC") ? XVar.Pack("DESC") : XVar.Pack("ASC"))), null);
				sqlValue = XVar.Clone(this.cipherer.MakeDBValue((XVar)(of["column"]), (XVar)(data[of["column"]]), new XVar(""), new XVar(true)));
				equal = new XVar("");
				if(i < MVCFunctions.count(orderFields) - 1)
				{
					equal = XVar.Clone(CommonFunctions.sqlEqual((XVar)(sqlColumn), (XVar)(sqlValue)));
				}
				var_next = XVar.Clone(CommonFunctions.sqlMoreThan((XVar)(sqlColumn), (XVar)(sqlValue)));
				prev = XVar.Clone(CommonFunctions.sqlLessThan((XVar)(sqlColumn), (XVar)(sqlValue)));
				if(of["dir"] == "DESC")
				{
					prev = XVar.Clone(CommonFunctions.sqlMoreThan((XVar)(sqlColumn), (XVar)(sqlValue)));
					var_next = XVar.Clone(CommonFunctions.sqlLessThan((XVar)(sqlColumn), (XVar)(sqlValue)));
				}
				nextWhere = XVar.Clone(SQLQuery.combineCases((XVar)(new XVar(0, var_next, 1, SQLQuery.combineCases((XVar)(new XVar(0, equal, 1, nextWhere)), new XVar("and")))), new XVar("or")));
				prevWhere = XVar.Clone(SQLQuery.combineCases((XVar)(new XVar(0, prev, 1, SQLQuery.combineCases((XVar)(new XVar(0, equal, 1, prevWhere)), new XVar("and")))), new XVar("or")));
			}
			return new XVar("nextWhere", nextWhere, "prevWhere", prevWhere, "nextOrder", MVCFunctions.implode(new XVar(", "), (XVar)(MVCFunctions.array_reverse((XVar)(nextOrder)))), "prevOrder", MVCFunctions.implode(new XVar(", "), (XVar)(MVCFunctions.array_reverse((XVar)(prevOrder)))));
		}
		public virtual XVar getNextPrevRecordKeys(dynamic data, dynamic _param_what = null)
		{
			#region default values
			if(_param_what as Object == null) _param_what = new XVar(Constants.BOTH_RECORDS);
			#endregion

			#region pass-by-value parameters
			dynamic what = XVar.Clone(_param_what);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(this.getSubsetDataCommand());
			return this.dataSource.getNextPrevKeys((XVar)(dc), (XVar)(data), (XVar)(what));
		}
		public virtual XVar getOrderByClause()
		{
			dynamic orderClause = null;
			orderClause = XVar.Clone(OrderClause.createFromPage(this));
			return orderClause.getOrderByExpression();
		}
		protected virtual XVar getKeyParams(dynamic _param_keys = null)
		{
			#region default values
			if(_param_keys as Object == null) _param_keys = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic keyParams = XVar.Array();
			if(XVar.Pack(XVar.Equals(XVar.Pack(keys), XVar.Pack(null))))
			{
				keys = XVar.Clone(this.keys);
			}
			if(XVar.Pack(!(XVar)(keys)))
			{
				return "";
			}
			keyParams = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> k in this.pSet.getTableKeys().GetEnumerator())
			{
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("editid", k.Key + 1, "=", MVCFunctions.RawUrlEncode((XVar)((XVar.Pack(keys.KeyExists(k.Value)) ? XVar.Pack(keys[k.Value]) : XVar.Pack(keys[k.Key]))))), null);
			}
			return MVCFunctions.implode(new XVar("&"), (XVar)(keyParams));
		}
		public virtual XVar assignPrevNextButtons(dynamic _param_showNext, dynamic _param_showPrev, dynamic _param_dashGridBased = null)
		{
			#region default values
			if(_param_dashGridBased as Object == null) _param_dashGridBased = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic showNext = XVar.Clone(_param_showNext);
			dynamic showPrev = XVar.Clone(_param_showPrev);
			dynamic dashGridBased = XVar.Clone(_param_dashGridBased);
			#endregion

			if(XVar.Pack(!(XVar)(this.pSet.useMoveNext())))
			{
				return null;
			}
			if((XVar)(showNext)  || (XVar)(dashGridBased))
			{
				this.xt.assign(new XVar("next_button"), new XVar(true));
				this.xt.assign(new XVar("nextbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"nextButton", this.id, "\"")));
				if(XVar.Pack(dashGridBased))
				{
					this.xt.assign(new XVar("nextbutton_class"), new XVar("rnr-invisible-button"));
				}
			}
			else
			{
				if(XVar.Pack(showPrev))
				{
					this.xt.assign(new XVar("next_button"), new XVar(true));
					this.xt.assign(new XVar("nextbutton_class"), new XVar("rnr-invisible-button"));
				}
				else
				{
					this.xt.assign(new XVar("next_button"), new XVar(false));
				}
			}
			if((XVar)(showPrev)  || (XVar)(dashGridBased))
			{
				this.xt.assign(new XVar("prev_button"), new XVar(true));
				this.xt.assign(new XVar("prevbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"prevButton", this.id, "\"")));
				if(XVar.Pack(dashGridBased))
				{
					this.xt.assign(new XVar("prevbutton_class"), new XVar("rnr-invisible-button"));
				}
			}
			else
			{
				if(XVar.Pack(showNext))
				{
					this.xt.assign(new XVar("prev_button"), new XVar(true));
					this.xt.assign(new XVar("prevbutton_class"), new XVar("rnr-invisible-button"));
				}
				else
				{
					this.xt.assign(new XVar("prev_button"), new XVar(false));
				}
			}

			return null;
		}
		public virtual XVar captchaExists()
		{
			return this.pSet.hasCaptcha();
		}
		public virtual XVar checkCaptcha()
		{
			dynamic captchaType = null;
			this.isCaptchaOk = new XVar(true);
			if(XVar.Pack(!(XVar)(this.captchaExists())))
			{
				return true;
			}
			captchaType = XVar.Clone(ProjectSettings.captchaValue(new XVar("captchaType")));
			if(XVar.Pack(XSession.Session.KeyExists("count_passes_captcha")))
			{
				XSession.Session["count_passes_captcha"] = XSession.Session["count_passes_captcha"] + 1;
				return true;
			}
			if((XVar)(!(XVar)(XSession.Session.KeyExists(MVCFunctions.Concat("isCaptcha", this.getCaptchaId(), "Showed"))))  && (XVar)(this.captchaValue == ""))
			{
				return true;
			}
			if((XVar)(captchaType == Constants.FLASH_CAPTCHA)  && (XVar)(MVCFunctions.strtolower((XVar)(this.captchaValue)) != MVCFunctions.strtolower((XVar)(XSession.Session[MVCFunctions.Concat("captcha_", this.getCaptchaId())]))))
			{
				this.isCaptchaOk = new XVar(false);
				this.message = XVar.Clone(CommonFunctions.mlang_message(new XVar("SEC_INVALID_CAPTCHA_CODE")));
			}
			if(captchaType == Constants.RE_CAPTCHA)
			{
				dynamic verifyResponse = XVar.Array();
				verifyResponse = XVar.Clone(CommonFunctions.verifyRecaptchaResponse((XVar)(this.captchaValue)));
				if(XVar.Pack(!(XVar)(verifyResponse["success"])))
				{
					this.isCaptchaOk = new XVar(false);
					this.message = XVar.Clone(verifyResponse["message"]);
				}
			}
			if(XVar.Pack(this.isCaptchaOk))
			{
				if(captchaType == Constants.FLASH_CAPTCHA)
				{
					XSession.Session.Remove(MVCFunctions.Concat("captcha_", this.getCaptchaId()));
				}
				XSession.Session.Remove(MVCFunctions.Concat("isCaptcha", this.getCaptchaId(), "Showed"));
				XSession.Session["count_passes_captcha"] = 0;
			}
			return this.isCaptchaOk;
		}
		public virtual XVar displayCaptcha()
		{
			dynamic captchaFieldName = null, captchaType = null, controls = XVar.Array();
			captchaType = XVar.Clone(ProjectSettings.captchaValue(new XVar("captchaType")));
			captchaFieldName = XVar.Clone(this.getCaptchaFieldName());
			if((XVar)(!(XVar)(XSession.Session.KeyExists("count_passes_captcha")))  || (XVar)(this.captchaPassesCount <= XSession.Session["count_passes_captcha"]))
			{
				if(captchaType == Constants.FLASH_CAPTCHA)
				{
					this.xt.assign(new XVar("captcha_block"), new XVar(true));
					this.xt.assign(new XVar("captcha"), (XVar)(this.getCaptchaHtml((XVar)(captchaFieldName))));
				}
				else
				{
					if(captchaType == Constants.RE_CAPTCHA)
					{
						this.reCaptchaCfg.InitAndSetArrayItem(MVCFunctions.Concat("value_", captchaFieldName, "_", this.id), "inputCaptchaId");
						this.controlsMap.InitAndSetArrayItem(this.reCaptchaCfg, "reCaptcha");
						this.xt.assign(new XVar("captcha_block"), new XVar(true));
						this.xt.assign(new XVar("captcha"), (XVar)(this.getCaptchaHtml((XVar)(captchaFieldName))));
					}
				}
				this.xt.assign(new XVar("captcha_field_name"), (XVar)(captchaFieldName));
				if(XVar.Pack(XSession.Session.KeyExists("count_passes_captcha")))
				{
					XSession.Session.Remove("count_passes_captcha");
				}
				XSession.Session[MVCFunctions.Concat("isCaptcha", this.getCaptchaId(), "Showed")] = 1;
			}
			controls = XVar.Clone(new XVar("controls", XVar.Array()));
			controls.InitAndSetArrayItem(0, "controls", "ctrlInd");
			controls.InitAndSetArrayItem(this.id, "controls", "id");
			controls.InitAndSetArrayItem(captchaFieldName, "controls", "fieldName");
			controls.InitAndSetArrayItem(this.pageType, "controls", "mode");
			if(XVar.Pack(!(XVar)(this.isCaptchaOk)))
			{
				controls.InitAndSetArrayItem(true, "controls", "isInvalid");
			}
			this.fillControlsMap((XVar)(controls));

			return null;
		}
		public virtual XVar getCaptchaHtml(dynamic _param__captchaFieldName)
		{
			#region pass-by-value parameters
			dynamic _captchaFieldName = XVar.Clone(_param__captchaFieldName);
			#endregion

			dynamic captchaHTML = null, captchaType = null;
			captchaType = XVar.Clone(ProjectSettings.captchaValue(new XVar("captchaType")));
			captchaHTML = new XVar("<div class=\"captcha_block\"");
			if(captchaType == Constants.RE_CAPTCHA)
			{
				captchaHTML = MVCFunctions.Concat(captchaHTML, " style=\"min-height:78px;\"");
			}
			captchaHTML = MVCFunctions.Concat(captchaHTML, ">");
			if(captchaType == Constants.FLASH_CAPTCHA)
			{
				dynamic path = null, swfPath = null, typeCodeMessage = null;
				typeCodeMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("SEC_TYPETHECODE")));
				path = XVar.Clone(MVCFunctions.GetCaptchaPath());
				swfPath = XVar.Clone(MVCFunctions.GetCaptchaSwfPath());
				captchaHTML = MVCFunctions.Concat(captchaHTML, "\r\n\t\t\t\t<div style=\"height:65px;\">\r\n\t\t\t\t<object width=\"210\" height=\"65\" data=\"", swfPath, "?path=", path, "?id=", this.getCaptchaId(), "\" type=\"application/x-shockwave-flash\">\r\n\t\t\t\t\t<param value=\"", swfPath, "?path=", path, "?id=", this.getCaptchaId(), "\" name=\"movie\"/>\r\n\t\t\t\t\t<param value=\"opaque\" name=\"wmode\"/>\r\n\t\t\t\t\t<a href=\"http://www.macromedia.com/go/getflashplayer\"><img alt=\"Download Flash\" src=\"\"/></a>\r\n\t\t\t\t</object>\r\n\t\t\t\t</div>");
				captchaHTML = MVCFunctions.Concat(captchaHTML, "<div style=\"white-space: nowrap;\">", typeCodeMessage, ":</div>\r\n\t\t\t\t<span id=\"edit", this.id, "_", _captchaFieldName, "_0\">\r\n\t\t\t\t\t<input type=\"text\" value=\"\" class=\"captcha_value\" name=\"value_", _captchaFieldName, "_", this.id, "\" style=\"\" id=\"value_", _captchaFieldName, "_", this.id, "\"/>\r\n\t\t\t\t\t<font color=\"red\">*</font>\r\n\t\t\t\t</span>");
			}
			else
			{
				if(captchaType == Constants.RE_CAPTCHA)
				{
					captchaHTML = MVCFunctions.Concat(captchaHTML, "<span id=\"edit", this.id, "_", _captchaFieldName, "_0\">\r\n\t\t\t\t\t<input type=\"hidden\" value=\"\" class=\"captcha_value\" name=\"", this.reCaptchaCfg["inputCaptchaId"], "\" style=\"\" id=\"", this.reCaptchaCfg["inputCaptchaId"], "\"/>\r\n\t\t\t\t\t</span>");
				}
			}
			captchaHTML = MVCFunctions.Concat(captchaHTML, "</div>");
			return captchaHTML;
		}
		public virtual XVar getCaptchaId()
		{
			return this.id;
		}
		public virtual XVar getCaptchaFieldName()
		{
			return "captcha";
		}
		public virtual XVar createPerPage()
		{
			dynamic allMessage = null, classString = null, i = null, rpp = null;
			classString = new XVar("");
			allMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("SHOW_ALL")));
			classString = new XVar("class=\"form-control\"");
			allMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("ALL")));
			rpp = XVar.Clone(MVCFunctions.Concat("<select ", classString, " id=\"recordspp", this.id, "\">"));
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.arrRecsPerPage); i++)
			{
				if(this.arrRecsPerPage[i] != -1)
				{
					rpp = MVCFunctions.Concat(rpp, "<option value=\"", this.arrRecsPerPage[i], "\" ", (XVar.Pack(this.pageSize == this.arrRecsPerPage[i]) ? XVar.Pack("selected") : XVar.Pack("")), ">", this.arrRecsPerPage[i], "</option>");
				}
				else
				{
					rpp = MVCFunctions.Concat(rpp, "<option value=\"-1\" ", (XVar.Pack(this.pageSize == this.arrRecsPerPage[i]) ? XVar.Pack("selected") : XVar.Pack("")), ">", allMessage, "</option>");
				}
			}
			rpp = MVCFunctions.Concat(rpp, "</select>");
			this.xt.assign(new XVar("recsPerPage"), (XVar)(rpp));

			return null;
		}
		public virtual XVar bsCreatePerPage()
		{
			dynamic i = null, rpp = null, selectedAttr = null, txtVal = null, val = null;
			txtVal = XVar.Clone(this.pageSize);
			if(this.pageSize == -1)
			{
				txtVal = XVar.Clone(CommonFunctions.mlang_message(new XVar("SHOW_ALL")));
			}
			rpp = XVar.Clone(MVCFunctions.Concat("<div class=\"dropdown btn-group\">\r\n\t\t\t<button class=\"btn btn-default dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\"><span class=\"dropdown-text\">", txtVal, "</span> <span class=\"caret\"></span></button>\r\n\t\t\t<ul class=\"dropdown-menu pull-right\" role=\"menu\">"));
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.arrRecsPerPage); i++)
			{
				val = XVar.Clone(this.arrRecsPerPage[i]);
				txtVal = XVar.Clone(val);
				if(val == -1)
				{
					txtVal = XVar.Clone(CommonFunctions.mlang_message(new XVar("SHOW_ALL")));
				}
				selectedAttr = new XVar("");
				if(this.pageSize == val)
				{
					selectedAttr = new XVar("aria-selected=\"true\" class=\"active\"");
				}
				else
				{
					selectedAttr = new XVar("aria-selected=\"false\"");
				}
				rpp = MVCFunctions.Concat(rpp, "<li ", selectedAttr, "><a data-action=\"", val, "\" class=\"dropdown-item dropdown-item-button\">", txtVal, "</a></li>");
			}
			rpp = MVCFunctions.Concat(rpp, "</ul></div>");
			this.xt.assign(new XVar("recsPerPage"), (XVar)(rpp));

			return null;
		}
		public virtual XVar ProcessFiles()
		{
			foreach (KeyValuePair<XVar, dynamic> f in this.filesToDelete.GetEnumerator())
			{
				f.Value.Delete();
			}
			foreach (KeyValuePair<XVar, dynamic> f in this.filesToMove.GetEnumerator())
			{
				f.Value.Move();
			}
			foreach (KeyValuePair<XVar, dynamic> f in this.filesToSave.GetEnumerator())
			{
				f.Value.Save();
			}

			return null;
		}
		public virtual XVar countDetailsRecsNoSubQ(dynamic _param_details, dynamic detailid)
		{
			#region pass-by-value parameters
			dynamic details = XVar.Clone(_param_details);
			#endregion

			dynamic dc = null, detPSet = null, detailsDs = null, detailsKeyValues = XVar.Array(), detailsKeys = XVar.Array(), detailsKeysSettings = XVar.Array(), filters = XVar.Array(), ret = null;
			detPSet = XVar.Clone(new ProjectSettings((XVar)(details)));
			detailsDs = XVar.Clone(CommonFunctions.getDataSource((XVar)(details), (XVar)(detPSet)));
			detailsKeysSettings = XVar.Clone(detPSet.getMasterKeys((XVar)(this.tName)));
			detailsKeys = XVar.Clone(detailsKeysSettings["detailsKeys"]);
			filters = XVar.Clone(XVar.Array());
			filters.InitAndSetArrayItem(Security.SelectCondition(new XVar("S"), (XVar)(detPSet)), null);
			detailsKeyValues = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in detailsKeys.GetEnumerator())
			{
				filters.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(field.Value), (XVar)(detailid[field.Key])), null);
				detailsKeyValues.InitAndSetArrayItem(detailid[field.Key], field.Value);
			}
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(filters)));
			RunnerContext.pushMasterContext((XVar)(detailsKeyValues));
			ret = XVar.Clone(this.limitRowCount((XVar)(detailsDs.getCount((XVar)(dc))), (XVar)(detPSet)));
			RunnerContext.pop();
			return ret;
		}
		public virtual XVar noRecordsMessage()
		{
			dynamic isSearchRun = null;
			isSearchRun = XVar.Clone(this.isSearchFunctionalityActivated());
			if((XVar)(!(XVar)(isSearchRun))  && (XVar)(this.getCurrentTabWhere() != ""))
			{
				isSearchRun = new XVar(true);
			}
			if((XVar)(this.pSetSearch.noRecordsOnFirstPage())  && (XVar)(!(XVar)(isSearchRun)))
			{
				return CommonFunctions.mlang_message(new XVar("NOTHING_TO_SEE"));
			}
			if((XVar)(!(XVar)(this.recordsOnPage))  && (XVar)(!(XVar)(isSearchRun)))
			{
				return CommonFunctions.mlang_message(new XVar("NO_DATA_YET"));
			}
			if((XVar)(isSearchRun)  && (XVar)(!(XVar)(this.recordsOnPage)))
			{
				return CommonFunctions.mlang_message(new XVar("NO_RECORDS"));
			}

			return null;
		}
		public virtual XVar showNoRecordsMessage()
		{
			dynamic message = null;
			message = XVar.Clone(MVCFunctions.Concat((XVar.Pack(this.is508 == true) ? XVar.Pack("<a name=\"skipdata\"></a>") : XVar.Pack("")), this.noRecordsMessage()));
			message = XVar.Clone(MVCFunctions.Concat("<span name=\"notfound_message", this.id, "\">", message, "</span>"));
			this.xt.assign(new XVar("message"), (XVar)(message));
			this.xt.assign(new XVar("message_class"), new XVar("alert-warning"));
			this.xt.assign(new XVar("message_block"), new XVar(true));

			return null;
		}
		protected virtual XVar assignPagingVariables()
		{
			if((XVar)(this.pageSize)  && (XVar)(this.pageSize != -1))
			{
				this.maxPages = XVar.Clone((XVar)Math.Ceiling((double)(this.numRowsFromSQL / this.pageSize)));
			}
			if((XVar)(this.numRowsFromSQL == 0)  && (XVar)(this.pSet.hideNumberOfRecords()))
			{
				this.maxPages = XVar.Clone(this.myPage);
			}
			if(this.maxPages < 1)
			{
				this.maxPages = new XVar(1);
			}
			if((XVar)(this.maxPages < this.myPage)  && (XVar)(!(XVar)(this.pSet.hideNumberOfRecords())))
			{
				this.myPage = XVar.Clone(this.maxPages);
			}
			if(this.myPage < 1)
			{
				this.myPage = new XVar(1);
			}
			this.recordsOnPage = XVar.Clone(this.numRowsFromSQL - (this.myPage - 1) * this.pageSize);
			if((XVar)(this.pageSize < this.recordsOnPage)  && (XVar)(this.pageSize != -1))
			{
				this.recordsOnPage = XVar.Clone(this.pageSize);
			}

			return null;
		}
		public virtual XVar buildPagination()
		{
			this.assignPagingVariables();
			if(XVar.Pack(!(XVar)(this.recordsOnPage)))
			{
				this.hideItemType(new XVar("details_found"));
				this.hideItemType(new XVar("details_found_unknown"));
			}
			if(XVar.Pack(!(XVar)(this.numRowsFromSQL)))
			{
				this.rowsFound = new XVar(false);
				if(XVar.Pack(!(XVar)(this.fieldFilterEnabled())))
				{
					this.showNoRecordsMessage();
				}
				if((XVar)(this.listAjax)  || (XVar)(this.mode == Constants.LIST_LOOKUP))
				{
					this.xt.assign(new XVar("pagination_block"), new XVar(true));
					this.hideElement(new XVar("pagination"));
					if(XVar.Pack(this.listAjax))
					{
						this.xt.assign(new XVar("pagination"), new XVar("  "));
					}
				}
				this.hideItemType(new XVar("page_size"));
			}
			else
			{
				dynamic firstDisplayed = null, lastDisplayed = null;
				this.rowsFound = new XVar(true);
				this.xt.assign(new XVar("details_found"), new XVar(true));
				this.xt.assign(new XVar("message_block"), new XVar(false));
				if((XVar)(this.listAjax)  || (XVar)(this.mode == Constants.LIST_LOOKUP))
				{
					this.xt.assign(new XVar("message_block"), new XVar(true));
					this.xt.assign(new XVar("message_class"), new XVar("alert-warning"));
					this.hideElement(new XVar("message"));
				}
				else
				{
					if(this.deleteMessage != "")
					{
						this.xt.assign(new XVar("message_block"), new XVar(true));
					}
				}
				this.xt.assign(new XVar("records_found"), (XVar)(this.numRowsFromSQL));
				firstDisplayed = XVar.Clone((this.myPage - 1) * this.pageSize + 1);
				lastDisplayed = XVar.Clone(this.myPage * this.pageSize);
				if(this.pageSize < 0)
				{
					firstDisplayed = new XVar(1);
					lastDisplayed = XVar.Clone(this.numRowsFromSQL);
				}
				if(this.numRowsFromSQL < lastDisplayed)
				{
					lastDisplayed = XVar.Clone(this.numRowsFromSQL);
				}
				this.prepareRecordsIndicator((XVar)(firstDisplayed), (XVar)(lastDisplayed), (XVar)(this.numRowsFromSQL));
				this.xt.assign(new XVar("pagesize"), (XVar)(this.pageSize));
				this.xt.assign(new XVar("page"), (XVar)(this.myPage));
				this.xt.assign(new XVar("maxpages"), (XVar)(this.maxPages));
				this.xt.assign(new XVar("pagination_block"), new XVar(false));
				if((XVar)(1 < this.maxPages)  || (XVar)((XVar)(1 < this.myPage)  && (XVar)(this.pSet.hideNumberOfRecords())))
				{
					dynamic pagination = null;
					this.xt.assign(new XVar("pagination_block"), new XVar(true));
					pagination = XVar.Clone(this.buildPaginationControl((XVar)(this.myPage), (XVar)(this.maxPages), new XVar(10)));
					this.xt.assign(new XVar("pagination"), (XVar)(pagination));
				}
				else
				{
					if((XVar)((XVar)(this.listAjax)  || (XVar)(this.mode == Constants.LIST_LOOKUP))  || (XVar)(this.mode == Constants.MEMBERS_PAGE))
					{
						this.xt.assign(new XVar("pagination_block"), new XVar(true));
						this.hideElement(new XVar("pagination"));
						if(XVar.Pack(this.listAjax))
						{
							this.xt.assign(new XVar("pagination"), new XVar("  "));
						}
					}
				}
				if(XVar.Pack(this.pSet.hideNumberOfRecords()))
				{
					if((XVar)((XVar)(0 <= this.pageSize)  && (XVar)(this.pageSize <= this.recordsOnPage))  && (XVar)(this.recordsOnPage))
					{
						this.xt.assign(new XVar("next_records"), new XVar(true));
						this.xt.assign(new XVar("next_page"), (XVar)(this.myPage + 1));
						this.xt.assign(new XVar("next_page_link"), (XVar)(this.getPaginationUrl((XVar)(this.myPage + 1))));
					}
					if((XVar)((XVar)(0 <= this.pageSize)  && (XVar)(this.recordsOnPage < this.pageSize))  && (XVar)(this.listAjax))
					{
						this.xt.assign(new XVar("next_records"), new XVar(true));
						this.hideItem(new XVar("next_page"));
					}
					if(1 < this.myPage)
					{
						this.xt.assign(new XVar("prev_records"), new XVar(true));
						this.xt.assign(new XVar("prev_page"), (XVar)(this.myPage - 1));
						this.xt.assign(new XVar("prev_page_link"), (XVar)(this.getPaginationUrl((XVar)(this.myPage - 1))));
					}
				}
			}

			return null;
		}
		public virtual XVar buildPaginationControl(dynamic _param_currentPage, dynamic _param_maxPages, dynamic _param_limit)
		{
			#region pass-by-value parameters
			dynamic currentPage = XVar.Clone(_param_currentPage);
			dynamic maxPages = XVar.Clone(_param_maxPages);
			dynamic limit = XVar.Clone(_param_limit);
			#endregion

			dynamic counter = null, counterend = null, counterstart = null, pageLinks = null, pagination = null;
			pagination = new XVar("");
			counterstart = XVar.Clone(currentPage - (limit - 1));
			if(currentPage  %  limit != 0)
			{
				counterstart = XVar.Clone((currentPage - currentPage  %  limit) + 1);
			}
			counterend = XVar.Clone((counterstart + limit) - 1);
			if(maxPages < counterend)
			{
				counterend = XVar.Clone(maxPages);
			}
			if(counterstart != 1)
			{
				pagination = MVCFunctions.Concat(pagination, this.getPaginationLink(new XVar(1), (XVar)(CommonFunctions.mlang_message(new XVar("FIRST")))));
				pagination = MVCFunctions.Concat(pagination, this.getPaginationLink((XVar)(counterstart - 1), (XVar)(CommonFunctions.mlang_message(new XVar("PREVIOUS")))));
			}
			pageLinks = new XVar("");
			counter = XVar.Clone(counterstart);
			for(;counter <= counterend; counter++)
			{
				pageLinks = MVCFunctions.Concat(pageLinks, this.getPaginationLink((XVar)(counter), (XVar)(counter), (XVar)(counter == currentPage)));
			}
			pagination = MVCFunctions.Concat(pagination, pageLinks);
			if(counterend != maxPages)
			{
				pagination = MVCFunctions.Concat(pagination, this.getPaginationLink((XVar)(counterend + 1), (XVar)(CommonFunctions.mlang_message(new XVar("NEXT")))));
				pagination = MVCFunctions.Concat(pagination, this.getPaginationLink((XVar)(maxPages), (XVar)(CommonFunctions.mlang_message(new XVar("LAST")))));
			}
			return MVCFunctions.Concat("<nav class=\"text-center\"><ul class=\"pagination\" data-function=\"pagination", this.id, "\">", pagination, "</ul></nav>");
		}
		public virtual XVar prepareRecordsIndicator(dynamic _param_firstDisplayed, dynamic _param_lastDisplayed, dynamic _param_totalDisplayed)
		{
			#region pass-by-value parameters
			dynamic firstDisplayed = XVar.Clone(_param_firstDisplayed);
			dynamic lastDisplayed = XVar.Clone(_param_lastDisplayed);
			dynamic totalDisplayed = XVar.Clone(_param_totalDisplayed);
			#endregion

			if(XVar.Pack(!(XVar)(this.pSet.isCrossTabReport())))
			{
				this.xt.assign(new XVar("details_found"), new XVar(true));
			}
			this.xt.assign(new XVar("first_shown"), (XVar)(firstDisplayed));
			this.xt.assign(new XVar("last_shown"), (XVar)(lastDisplayed));

			return null;
		}
		public virtual XVar getPaginationLink(dynamic _param_pageNum, dynamic _param_linkText, dynamic _param_active = null)
		{
			#region default values
			if(_param_active as Object == null) _param_active = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic pageNum = XVar.Clone(_param_pageNum);
			dynamic linkText = XVar.Clone(_param_linkText);
			dynamic active = XVar.Clone(_param_active);
			#endregion

			dynamic href = null;
			href = XVar.Clone(this.getPaginationUrl((XVar)(pageNum)));
			return MVCFunctions.Concat("<li ", (XVar.Pack(CommonFunctions.isRTL()) ? XVar.Pack("dir=\"RTL\"") : XVar.Pack("")), " class=\"", (XVar.Pack(active) ? XVar.Pack("active") : XVar.Pack("")), "\"><a href=\"", href, "\" pageNum=\"", pageNum, "\" >", linkText, "</a></li>");
		}
		public virtual XVar getPaginationUrl(dynamic _param_pageNum)
		{
			#region pass-by-value parameters
			dynamic pageNum = XVar.Clone(_param_pageNum);
			#endregion

			dynamic var_params = XVar.Array();
			var_params = XVar.Clone(new XVar(0, MVCFunctions.Concat("goto=", pageNum), 1, this.getStateUrlParams()));
			if(this.pageName != this.pSet.getDefaultPage((XVar)(this.pageType)))
			{
				var_params.InitAndSetArrayItem(MVCFunctions.Concat("page=", this.pageName), null);
			}
			return MVCFunctions.Concat(MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(this.tName))), (XVar)(this.pageType)), "?", MVCFunctions.implode((XVar)(var_params)));
		}
		public virtual XVar isAdminTable()
		{
			if(XVar.Pack(Security.isAdminTable((XVar)(this.tName))))
			{
				return true;
			}
			return false;
		}
		public virtual XVar fieldAlign(dynamic _param_f)
		{
			#region pass-by-value parameters
			dynamic f = XVar.Clone(_param_f);
			#endregion

			dynamic format = null;
			if(this.pSet.getEditFormat((XVar)(f)) == Constants.FORMAT_LOOKUP_WIZARD)
			{
				return "left";
			}
			format = XVar.Clone(this.pSet.getViewFormat((XVar)(f)));
			if((XVar)((XVar)(format == Constants.FORMAT_FILE)  || (XVar)(format == Constants.FORMAT_AUDIO))  || (XVar)(format == Constants.FORMAT_CHECKBOX))
			{
				return "left";
			}
			if((XVar)(format == Constants.FORMAT_NUMBER)  || (XVar)(CommonFunctions.IsNumberType((XVar)(this.pSet.getFieldType((XVar)(f))))))
			{
				return "right";
			}
			return "left";
		}
		public virtual XVar fieldClass(dynamic _param_f)
		{
			#region pass-by-value parameters
			dynamic f = XVar.Clone(_param_f);
			#endregion

			dynamic format = null;
			if(this.pSet.getEditFormat((XVar)(f)) == Constants.FORMAT_LOOKUP_WIZARD)
			{
				return "";
			}
			format = XVar.Clone(this.pSet.getViewFormat((XVar)(f)));
			if(format == Constants.FORMAT_FILE)
			{
				return " rnr-field-file";
			}
			if(format == Constants.FORMAT_AUDIO)
			{
				return " rnr-field-audio";
			}
			if(format == Constants.FORMAT_CHECKBOX)
			{
				return " rnr-field-checkbox";
			}
			if((XVar)(format == Constants.FORMAT_NUMBER)  || (XVar)(CommonFunctions.IsNumberType((XVar)(this.pSet.getFieldType((XVar)(f))))))
			{
				return " r-field-number";
			}
			return "r-field-text";
		}
		public virtual XVar buildDetailGridLinks(dynamic data)
		{
			dynamic hrefs = XVar.Array();
			hrefs = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				dynamic dShortTable = null, idLink = null, idx = null, masterKeys = XVar.Array(), masterquery = null;
				dShortTable = XVar.Clone(CommonFunctions.GetTableURL((XVar)(details.Value)));
				masterquery = XVar.Clone(MVCFunctions.Concat("mastertable=", MVCFunctions.RawUrlEncode((XVar)(this.tName))));
				masterKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(details.Value)));
				idx = new XVar(1);
				for(;idx <= MVCFunctions.count(masterKeys["masterKeys"]); idx++)
				{
					masterquery = MVCFunctions.Concat(masterquery, "&masterkey", idx, "=", MVCFunctions.RawUrlEncode((XVar)(data[details.Value][MVCFunctions.Concat("masterkey", idx)])));
				}
				idLink = XVar.Clone(dShortTable);
				hrefs.InitAndSetArrayItem(new XVar("id", idLink, "href", MVCFunctions.GetTableLink((XVar)(dShortTable), (XVar)(ProjectSettings.defaultPageType((XVar)(CommonFunctions.GetEntityType((XVar)(details.Value))))), (XVar)(masterquery))), null);
			}
			return hrefs;
		}
		public virtual EditControl getControl(dynamic _param_field, dynamic _param_id = null, dynamic _param_extraParams = null)
		{
			#region default values
			if(_param_id as Object == null) _param_id = new XVar("");
			if(_param_extraParams as Object == null) _param_extraParams = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic id = XVar.Clone(_param_id);
			dynamic extraParams = XVar.Clone(_param_extraParams);
			#endregion

			return XVar.UnPackEditControl(this.controls.getControl((XVar)(field), (XVar)(id), (XVar)(extraParams)) ?? new XVar());
		}
		public virtual ViewControl getViewControl(dynamic _param_field, dynamic _param_format = null)
		{
			#region default values
			if(_param_format as Object == null) _param_format = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic format = XVar.Clone(_param_format);
			#endregion

			return XVar.UnPackViewControl(this.viewControls.getControl((XVar)(field), (XVar)(format)) ?? new XVar());
		}
		public virtual XVar setForExportVar(dynamic _param_forExport)
		{
			#region pass-by-value parameters
			dynamic forExport = XVar.Clone(_param_forExport);
			#endregion

			this.viewControls.setForExportVar((XVar)(forExport));

			return null;
		}
		public virtual XVar showDBValue(dynamic _param_field, dynamic data, dynamic _param_keylink = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			if((XVar)(!(XVar)(keylink))  && (XVar)(data))
			{
				dynamic i = null, tKeys = XVar.Array();
				tKeys = XVar.Clone(this.pSet.getTableKeys());
				keylink = new XVar("");
				i = new XVar(0);
				for(;i < MVCFunctions.count(tKeys); i++)
				{
					keylink = MVCFunctions.Concat(keylink, "&key", i + 1, "=", MVCFunctions.RawUrlEncode((XVar)(data[tKeys[i]])));
				}
			}
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return this.getViewControl((XVar)(field)).getPdfValue((XVar)(data), (XVar)(keylink));
			}
			return this.getViewControl((XVar)(field)).showDBValue((XVar)(data), (XVar)(keylink), (XVar)(html));
		}
		public virtual XVar showTextValue(dynamic _param_field, dynamic data)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getViewControl((XVar)(field)).getTextValue((XVar)(data));
		}
		public virtual XVar getTextValue(dynamic _param_field, dynamic data)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getViewControl((XVar)(field)).getTextValue((XVar)(data));
		}
		public virtual XVar getExportValue(dynamic _param_field, dynamic data, dynamic _param_keylink = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			if(_param_html as Object == null) _param_html = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			return this.getViewControl((XVar)(field)).getExportValue((XVar)(data), (XVar)(keylink), (XVar)(html));
		}
		public virtual XVar hideField(dynamic _param_fieldName, dynamic _param_recordId = null)
		{
			#region default values
			if(_param_recordId as Object == null) _param_recordId = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			dynamic recordId = XVar.Clone(_param_recordId);
			#endregion

			dynamic items = XVar.Array();
			items = XVar.Clone(this.pSet.getFieldItems((XVar)(fieldName)));
			if(XVar.Pack(MVCFunctions.is_array((XVar)(items))))
			{
				foreach (KeyValuePair<XVar, dynamic> i in items.GetEnumerator())
				{
					this.hideItem((XVar)(i.Value), (XVar)(recordId));
				}
			}

			return null;
		}
		public virtual XVar showField(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			dynamic items = XVar.Array();
			items = XVar.Clone(this.pSet.getFieldItems((XVar)(fieldName)));
			if(XVar.Pack(MVCFunctions.is_array((XVar)(items))))
			{
				foreach (KeyValuePair<XVar, dynamic> i in items.GetEnumerator())
				{
					this.showItem((XVar)(i.Value));
				}
			}

			return null;
		}
		public virtual dynamic getPageLayout(dynamic _param_tName = null, dynamic _param_pageType = null, dynamic _param_suffix = null)
		{
			#region default values
			if(_param_tName as Object == null) _param_tName = new XVar("");
			if(_param_pageType as Object == null) _param_pageType = new XVar("");
			if(_param_suffix as Object == null) _param_suffix = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic tName = XVar.Clone(_param_tName);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic suffix = XVar.Clone(_param_suffix);
			#endregion

			dynamic templateName = null;
			if(XVar.Pack(!(XVar)(tName)))
			{
				tName = XVar.Clone(this.tName);
			}
			if(XVar.Pack(!(XVar)(pageType)))
			{
				pageType = XVar.Clone(this.pageType);
			}
			templateName = XVar.Clone(MVCFunctions.Concat(CommonFunctions.GetTableURL((XVar)(tName)), "_", pageType));
			if(XVar.Pack(suffix))
			{
				templateName = XVar.Clone(MVCFunctions.Concat(templateName, "_", suffix));
			}
			if((XVar)(!(XVar)(this.isPageTableBased()))  || (XVar)(this.pageType == Constants.PAGE_REGISTER))
			{
				templateName = XVar.Clone(pageType);
			}
			return GlobalVars.page_layouts[templateName];
		}
		public virtual XVar isPageTableBased()
		{
			if((XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_MENU)  || (XVar)(this.pageType == Constants.PAGE_LOGIN))  || (XVar)(this.pageType == Constants.PAGE_REMIND))  || (XVar)(this.pageType == Constants.PAGE_CHANGEPASS))
			{
				return false;
			}
			return true;
		}
		public virtual XVar isBrickSet(dynamic _param_brickName)
		{
			#region pass-by-value parameters
			dynamic brickName = XVar.Clone(_param_brickName);
			#endregion

			dynamic layout = null;
			return true;
			layout = XVar.Clone(this.getPageLayout());
			if(XVar.Pack(layout))
			{
				return layout.isBrickSet((XVar)(brickName));
			}
			return false;
		}
		public virtual XVar getBrickTableName(dynamic _param_brickName)
		{
			#region pass-by-value parameters
			dynamic brickName = XVar.Clone(_param_brickName);
			#endregion

			dynamic layout = null;
			layout = XVar.Clone(this.getPageLayout());
			if(XVar.Pack(layout))
			{
				return layout.getBrickTableName((XVar)(brickName));
			}
			return "";
		}
		protected virtual XVar checkIfSearchPanelActivated()
		{
			return this.pSet.showSearchPanel();
		}
		protected virtual XVar buildAddedSearchPanel()
		{
			if((XVar)((XVar)((XVar)((XVar)(this.pageType != Constants.PAGE_REPORT)  && (XVar)(this.pageType != Constants.PAGE_CHART))  && (XVar)(this.pageType != Constants.PAGE_LIST))  && (XVar)(!(XVar)((XVar)(this.pageType == Constants.PAGE_ADD)  && (XVar)(this.mode == Constants.ADD_INLINE))))  && (XVar)(!(XVar)((XVar)(this.pageType == Constants.PAGE_EDIT)  && (XVar)(this.mode == Constants.EDIT_INLINE))))
			{
				this.buildSearchPanel();
			}

			return null;
		}
		public virtual XVar buildSearchPanel()
		{
			dynamic searchPanelObj = null, var_params = XVar.Array();
			if((XVar)(!(XVar)(this.searchPanelActivated))  || (XVar)(!(XVar)(this.permis[this.searchTableName]["search"])))
			{
				return null;
			}
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(this, "pageObj");
			searchPanelObj = XVar.Clone(new SearchPanelSimple((XVar)(var_params)));
			searchPanelObj.buildSearchPanel();

			return null;
		}
		public virtual XVar assignSimpleSearch()
		{
			dynamic showallbutton_attrs = null;
			if(XVar.Pack(!(XVar)(this.permis[this.searchTableName]["search"])))
			{
				return null;
			}
			this.xt.assign(new XVar("searchform_text"), new XVar(true));
			this.xt.assign(new XVar("searchform_search"), new XVar(true));
			this.xt.assign(new XVar("searchbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"searchButtTop", this.id, "\" title=\"", CommonFunctions.mlang_message(new XVar("SEARCH")), "\"")));
			if((XVar)((XVar)(!(XVar)(this.pSetSearch.noRecordsOnFirstPage()))  || (XVar)(this.searchClauseObj.isSearchFunctionalityActivated()))  || (XVar)(this.listAjax))
			{
				this.xt.assign(new XVar("searchform_showall"), new XVar(true));
			}
			if((XVar)(this.pSetSearch.noRecordsOnFirstPage())  && (XVar)(this.listAjax))
			{
				this.xt.displayItemHidden(new XVar("searchform_showall"));
			}
			showallbutton_attrs = MVCFunctions.Concat(showallbutton_attrs, "id=\"showAll", this.id, "\"");
			if(XVar.Pack(!(XVar)(this.searchClauseObj.isShowAll())))
			{
				showallbutton_attrs = MVCFunctions.Concat(showallbutton_attrs, " style=\"display: none;\"");
			}
			this.xt.assign(new XVar("showallbutton_attrs"), (XVar)(showallbutton_attrs));
			this.assignSearchControl();

			return null;
		}
		protected virtual XVar assignSearchControl()
		{
			dynamic searchforAttrs = null, var_params = XVar.Array();
			var_params = XVar.Clone(this.searchClauseObj.getSearchGlobalParams());
			searchforAttrs = XVar.Clone(MVCFunctions.Concat("name=\"ctlSearchFor", this.id, "\" id=\"ctlSearchFor", this.id, "\""));
			if(XVar.Pack(this.isUseAjaxSuggest))
			{
				searchforAttrs = MVCFunctions.Concat(searchforAttrs, " autocomplete=off ");
			}
			searchforAttrs = MVCFunctions.Concat(searchforAttrs, " placeholder=\"", CommonFunctions.mlang_message(new XVar("SEARCH_TIP")), "\"");
			if((XVar)(this.searchClauseObj.searchStarted())  || (XVar)(MVCFunctions.strlen((XVar)(var_params["simpleSrch"]))))
			{
				dynamic valSrchFor = null;
				valSrchFor = XVar.Clone(var_params["simpleSrch"]);
				searchforAttrs = MVCFunctions.Concat(searchforAttrs, " value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(valSrchFor)), "\"");
			}
			this.xt.assignbyref(new XVar("searchfor_attrs"), (XVar)(searchforAttrs));
			if(XVar.Pack(this.pSetSearch.showSimpleSearchOptions()))
			{
				dynamic fieldCtrl = null, typeCtrl = null;
				typeCtrl = XVar.Clone(SearchControl.getSimpleSearchTypeCombo((XVar)(var_params["simpleSrchTypeComboOpt"]), (XVar)(var_params["simpleSrchTypeComboNot"]), (XVar)(this.id)));
				this.xt.assign(new XVar("simpleSearchTypeCombo"), (XVar)(typeCtrl));
				fieldCtrl = XVar.Clone(SearchControl.simpleSearchFieldCombo((XVar)(this.pSet.getAllSearchFields()), (XVar)(this.pSet.getGoogleLikeFields()), (XVar)(var_params["simpleSrchFieldsComboOpt"]), (XVar)(this.searchTableName), (XVar)(this.id)));
				this.xt.assign(new XVar("simpleSearchFieldCombo"), (XVar)(fieldCtrl));
			}

			return null;
		}
		public virtual XVar buildFilterPanel()
		{
			dynamic filterPanel = null, var_params = XVar.Array();
			if((XVar)(!(XVar)(this.permis[this.tName]["search"]))  || (XVar)((XVar)(this.pSetEdit.isSearchRequiredForFiltering())  && (XVar)(!(XVar)(this.isRequiredSearchRunning()))))
			{
				this.prepareEmptyFPMarkup();
				return false;
			}
			if(XVar.Pack(!(XVar)(this.pSet.getFilterFields())))
			{
				return false;
			}
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(this, "pageObj");
			filterPanel = XVar.Clone(new FilterPanel((XVar)(var_params)));
			return filterPanel.buildFilterPanel();
		}
		protected virtual XVar prepareEmptyFPMarkup()
		{

			return null;
		}
		public virtual XVar isSearchFunctionalityActivated()
		{
			if(XVar.Pack(!(XVar)(this.searchClauseObj)))
			{
				return false;
			}
			return this.searchClauseObj.isSearchFunctionalityActivated();
		}
		public virtual XVar isRequiredSearchRunning()
		{
			if(XVar.Pack(!(XVar)(this.searchClauseObj)))
			{
				return false;
			}
			return this.searchClauseObj.isRequiredSearchRunning();
		}
		public virtual XVar makeClassName(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return MVCFunctions.Concat("rnr-", name);
		}
		protected virtual XVar checkDeniedDuplicatedForUpdate(dynamic oldRecordData, dynamic newRecordData)
		{
			foreach (KeyValuePair<XVar, dynamic> value in newRecordData.GetEnumerator())
			{
				dynamic ctrl = null, displayedValue = null;
				if(XVar.Pack(this.pSet.allowDuplicateValues((XVar)(value.Key))))
				{
					continue;
				}
				if(XVar.Pack(!(XVar)(this.hasDuplicateValue((XVar)(value.Key), (XVar)(value.Value), (XVar)(oldRecordData[value.Key])))))
				{
					continue;
				}
				this.errorFields.InitAndSetArrayItem(value.Key, null);
				displayedValue = XVar.Clone(value.Value);
				ctrl = XVar.Clone(this.getViewControl((XVar)(value.Key)));
				if(XVar.Pack(ctrl))
				{
					dynamic data = null;
					data = XVar.Clone(new XVar(value.Key, value.Value));
					displayedValue = XVar.Clone(ctrl.getTextValue((XVar)(data)));
				}
				this.setMessage((XVar)(this.getDenyDuplicatedMessage((XVar)(value.Key), (XVar)(displayedValue))));
				return false;
			}
			return true;
		}
		public virtual XVar hasDeniedDuplicateValues(dynamic _param_fieldsData, ref dynamic message)
		{
			#region pass-by-value parameters
			dynamic fieldsData = XVar.Clone(_param_fieldsData);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> value in fieldsData.GetEnumerator())
			{
				if(XVar.Pack(this.pSet.allowDuplicateValues((XVar)(value.Key))))
				{
					continue;
				}
				if(XVar.Pack(this.hasDuplicateValue((XVar)(value.Key), (XVar)(value.Value))))
				{
					this.errorFields.InitAndSetArrayItem(value.Key, null);
					if((XVar)(!(XVar)((XVar)(this.pageType == Constants.PAGE_EDIT)  && (XVar)(this.mode == Constants.EDIT_POPUP)))  && (XVar)(!(XVar)((XVar)(this.pageType == Constants.PAGE_ADD)  && (XVar)(this.mode == Constants.ADD_POPUP))))
					{
						dynamic ctrl = null, displayedValue = null;
						displayedValue = XVar.Clone(value.Value);
						ctrl = XVar.Clone(this.getViewControl((XVar)(value.Key)));
						if(XVar.Pack(ctrl))
						{
							dynamic data = null;
							data = XVar.Clone(new XVar(value.Key, value.Value));
							displayedValue = XVar.Clone(ctrl.getTextValue((XVar)(data)));
						}
						message = XVar.Clone(this.getDenyDuplicatedMessage((XVar)(value.Key), (XVar)(displayedValue)));
					}
					return true;
				}
			}
			return false;
		}
		protected virtual XVar getDenyDuplicatedMessage(dynamic _param_fName, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic message = null, messageData = null, validationData = XVar.Array();
			validationData = XVar.Clone(this.pSet.getValidationData((XVar)(fName)));
			messageData = XVar.Clone(validationData["customMessages"]["DenyDuplicated"]);
			message = XVar.Clone(CommonFunctions.GetMLString((XVar)(messageData)));
			return MVCFunctions.Concat(this.pSet.label((XVar)(fName)), ": ", MVCFunctions.str_replace(new XVar("%value%"), (XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.substr((XVar)(value), new XVar(0), new XVar(10))))), (XVar)(message)));
		}
		public virtual XVar hasDuplicateValue(dynamic _param_fieldName, dynamic _param_value, dynamic _param_oldValue = null)
		{
			#region default values
			if(_param_oldValue as Object == null) _param_oldValue = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			dynamic value = XVar.Clone(_param_value);
			dynamic oldValue = XVar.Clone(_param_oldValue);
			#endregion

			dynamic data = XVar.Array(), dc = null, qResult = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(value)))))
			{
				return false;
			}
			dc = XVar.Clone(new DsCommand());
			if(XVar.Pack(oldValue))
			{
				dynamic conditions = XVar.Array();
				conditions = XVar.Clone(new XVar(0, DataCondition.FieldEquals((XVar)(fieldName), (XVar)(value), new XVar(0), new XVar(Constants.dsCASE_DEFAULT))));
				conditions.InitAndSetArrayItem(DataCondition._Not((XVar)(DataCondition.FieldEquals((XVar)(fieldName), (XVar)(oldValue), new XVar(0), new XVar(Constants.dsCASE_DEFAULT)))), null);
				dc.filter = XVar.Clone(DataCondition._And((XVar)(conditions)));
			}
			else
			{
				dc.filter = XVar.Clone(DataCondition.FieldEquals((XVar)(fieldName), (XVar)(value), new XVar(0), new XVar(Constants.dsCASE_DEFAULT)));
			}
			dc.totals.InitAndSetArrayItem(new XVar("total", "count", "alias", MVCFunctions.Concat("count_", fieldName), "field", fieldName), null);
			qResult = XVar.Clone(this.dataSource.getTotals((XVar)(dc)));
			data = XVar.Clone(qResult.fetchAssoc());
			if(XVar.Pack(!(XVar)(data[MVCFunctions.Concat("count_", fieldName)])))
			{
				return false;
			}
			return true;
		}
		public virtual XVar fetchBlocksList(dynamic _param_blocks, dynamic _param_dash = null)
		{
			#region default values
			if(_param_dash as Object == null) _param_dash = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic blocks = XVar.Clone(_param_blocks);
			dynamic dash = XVar.Clone(_param_dash);
			#endregion

			dynamic brickCount = null, fetchedBlocks = null, firstRightAligned = null, hasRightAligned = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(blocks)))))
			{
				return this.xt.fetch_loaded((XVar)(blocks));
			}
			fetchedBlocks = new XVar("");
			firstRightAligned = new XVar(true);
			hasRightAligned = new XVar(false);
			brickCount = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> b in blocks.GetEnumerator())
			{
				dynamic align = null, fetched = null, name = null;
				++(brickCount);
				align = new XVar("");
				if(XVar.Pack(MVCFunctions.is_array((XVar)(b.Value))))
				{
					name = XVar.Clone(b.Value["name"]);
					align = XVar.Clone(b.Value["align"]);
				}
				else
				{
					name = XVar.Clone(b.Value);
				}
				fetched = XVar.Clone(this.xt.fetch_loaded((XVar)(name)));
				if(XVar.Pack(!(XVar)(fetched)))
				{
					continue;
				}
				if(XVar.Pack(dash))
				{
					dynamic alignClass = null;
					alignClass = new XVar("");
					if(align == "right")
					{
						alignClass = new XVar("rnr-dberight");
					}
					fetched = XVar.Clone(MVCFunctions.Concat("<span class=\"rnr-dbebrick ", alignClass, "\">", fetched, "</span>"));
					if((XVar)(align == "right")  && (XVar)(firstRightAligned))
					{
						fetched = XVar.Clone(MVCFunctions.Concat("<div class=\"rnr-dbefiller\"></div>", fetched));
						firstRightAligned = new XVar(false);
						hasRightAligned = new XVar(true);
					}
				}
				fetchedBlocks = MVCFunctions.Concat(fetchedBlocks, fetched);
			}
			if((XVar)((XVar)((XVar)(dash)  && (XVar)(fetchedBlocks != XVar.Pack("")))  && (XVar)(1 < brickCount))  && (XVar)(!(XVar)(hasRightAligned)))
			{
				fetchedBlocks = MVCFunctions.Concat(fetchedBlocks, "<div class=\"rnr-dbefiller\"></div>");
			}
			return fetchedBlocks;
		}
		protected virtual XVar needPopupSettings()
		{
			return true;
		}
		public virtual XVar displayAJAX(dynamic _param_templatefile, dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic templatefile = XVar.Clone(_param_templatefile);
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic extraParams = XVar.Array(), returnJSON = XVar.Array();
			if(XVar.Pack(this.gridTabsAvailable()))
			{
				this.pageData.InitAndSetArrayItem(this.getTabsHtml(), "tabs");
				this.pageData.InitAndSetArrayItem(this.getCurrentTabId(), "tabId");
			}
			returnJSON = XVar.Clone(XVar.Array());
			returnJSON.InitAndSetArrayItem(true, "success");
			returnJSON.InitAndSetArrayItem(GlobalVars.pagesData, "pagesData");
			if(XVar.Pack(MVCFunctions.count(this.controlsHTMLMap)))
			{
				returnJSON.InitAndSetArrayItem(this.controlsHTMLMap, "controlsMap");
			}
			if(XVar.Pack(MVCFunctions.count(this.viewControlsHTMLMap)))
			{
				returnJSON.InitAndSetArrayItem(this.viewControlsHTMLMap, "viewControlsMap");
			}
			if(XVar.Pack(MVCFunctions.count(this.includes_css)))
			{
				returnJSON.InitAndSetArrayItem(MVCFunctions.array_unique((XVar)(this.includes_css)), "CSSFiles");
			}
			returnJSON.InitAndSetArrayItem(this.grabAllJsFiles(), "additionalJS");
			returnJSON.InitAndSetArrayItem(id, "idStartFrom");
			this.xt.load_template((XVar)(templatefile));
			if(XVar.Pack(MVCFunctions.count(this.headerForms)))
			{
				returnJSON.InitAndSetArrayItem(this.fetchForms((XVar)(this.headerForms)), "headerCont");
			}
			if(XVar.Pack(MVCFunctions.count(this.footerForms)))
			{
				returnJSON.InitAndSetArrayItem(this.fetchForms((XVar)(this.footerForms)), "footerCont");
			}
			if(this.pageType == Constants.PAGE_CHART)
			{
				returnJSON.InitAndSetArrayItem(MVCFunctions.Concat("<span class=\"rnr-dbebrick\">", this.getPageTitle((XVar)(this.pageName), (XVar)(this.tName)), "</span>"), "headerCont");
			}
			if(XVar.Pack(this.dashElementData))
			{
				dynamic icon = null;
				icon = XVar.Clone(CommonFunctions.getIconHTML((XVar)(this.dashElementData["item"]["icon"])));
				if(XVar.Pack(icon))
				{
					returnJSON.InitAndSetArrayItem(icon, "iconHtml");
				}
			}
			this.assignFormFooterAndHeaderBricks(new XVar(false));
			returnJSON.InitAndSetArrayItem(this.getBodyMarkup((XVar)(templatefile)), "html");
			if(XVar.Pack(this.needPopupSettings()))
			{
				returnJSON.InitAndSetArrayItem(this.jsSettings, "settings");
			}
			extraParams = XVar.Clone(this.getExtraAjaxPageParams());
			if(XVar.Pack(MVCFunctions.count(extraParams)))
			{
				foreach (KeyValuePair<XVar, dynamic> paramValue in extraParams.GetEnumerator())
				{
					returnJSON.InitAndSetArrayItem(paramValue.Value, paramValue.Key);
				}
			}
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));

			return null;
		}
		protected virtual XVar getBodyMarkup(dynamic _param_templatefile)
		{
			#region pass-by-value parameters
			dynamic templatefile = XVar.Clone(_param_templatefile);
			#endregion

			return this.fetchForms((XVar)(this.bodyForms));
		}
		protected virtual XVar getExtraAjaxPageParams()
		{
			return XVar.Array();
		}
		public virtual XVar assignFormFooterAndHeaderBricks(dynamic _param_assignValue = null)
		{
			#region default values
			if(_param_assignValue as Object == null) _param_assignValue = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic assignValue = XVar.Clone(_param_assignValue);
			#endregion

			dynamic name = null;
			if(XVar.Pack(this.formBricks["header"]))
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(this.formBricks["header"])))))
				{
					this.formBricks.InitAndSetArrayItem(new XVar(0, this.formBricks["header"]), "header");
				}
				foreach (KeyValuePair<XVar, dynamic> b in this.formBricks["header"].GetEnumerator())
				{
					name = XVar.Clone(b.Value);
					if(XVar.Pack(MVCFunctions.is_array((XVar)(b.Value))))
					{
						name = XVar.Clone(b.Value["name"]);
					}
					this.xt.assign((XVar)(name), (XVar)(assignValue));
				}
			}
			if(XVar.Pack(this.formBricks["footer"]))
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(this.formBricks["footer"])))))
				{
					this.formBricks.InitAndSetArrayItem(new XVar(0, this.formBricks["footer"]), "footer");
				}
				foreach (KeyValuePair<XVar, dynamic> b in this.formBricks["footer"].GetEnumerator())
				{
					name = XVar.Clone(b.Value);
					if(XVar.Pack(MVCFunctions.is_array((XVar)(b.Value))))
					{
						name = XVar.Clone(b.Value["name"]);
					}
					this.xt.assign((XVar)(name), (XVar)(assignValue));
				}
			}

			return null;
		}
		public virtual XVar assignStyleFiles(dynamic _param_isPdfPage = null)
		{
			#region default values
			if(_param_isPdfPage as Object == null) _param_isPdfPage = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic isPdfPage = XVar.Clone(_param_isPdfPage);
			#endregion

			dynamic addKey = null, f = null, i = null, projectBuildKey = null, wizardBuildKey = null;
			projectBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("projectBuild")));
			wizardBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("wizardBuild")));
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.includes_css); i++)
			{
				f = XVar.Clone(this.includes_css[i]);
				if(XVar.Pack(this.isCustomCssFile((XVar)(f))))
				{
					addKey = XVar.Clone(MVCFunctions.Concat("?", projectBuildKey));
				}
				else
				{
					addKey = XVar.Clone(MVCFunctions.Concat("?", wizardBuildKey));
				}
				if(0 < MVCFunctions.strpos((XVar)(f), new XVar("style.css")))
				{
					addKey = MVCFunctions.Concat(addKey, "&", projectBuildKey);
				}
				this.includes_css.InitAndSetArrayItem(MVCFunctions.Concat(f, addKey), i);
			}
			this.xt.assign_array(new XVar("styleCSSFiles"), new XVar("stylepath"), (XVar)(MVCFunctions.array_unique((XVar)(this.includes_css))));
			this.includes_css = XVar.Clone(XVar.Array());
			this.xt.assign(new XVar("wizardBuildKey"), (XVar)(wizardBuildKey));

			return null;
		}
		public virtual XVar isCustomCssFile(dynamic _param_file)
		{
			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			#endregion

			return (XVar)(0 < MVCFunctions.strpos((XVar)(file), new XVar("/pages/")))  || (XVar)(0 < MVCFunctions.strpos((XVar)(file), new XVar("/custom/")));
		}
		public virtual XVar display(dynamic _param_templatefile)
		{
			#region pass-by-value parameters
			dynamic templatefile = XVar.Clone(_param_templatefile);
			#endregion

			this.assignStyleFiles();
			this.xt.display((XVar)(templatefile));

			return null;
		}
		public virtual XVar getMasterTableSQLClause()
		{
			dynamic detailsKeys = XVar.Array(), detailsKeysSettings = XVar.Array(), i = null, mKey = null, mValue = null, where = null;
			where = new XVar("");
			detailsKeysSettings = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
			detailsKeys = XVar.Clone(detailsKeysSettings["detailsKeys"]);
			if(XVar.Pack(!(XVar)(detailsKeys)))
			{
				return where;
			}
			i = new XVar(0);
			for(;i < MVCFunctions.count(detailsKeys); i++)
			{
				if(i != XVar.Pack(0))
				{
					where = MVCFunctions.Concat(where, " and ");
				}
				mKey = XVar.Clone(this.masterKeysReq[i + 1]);
				if((XVar)(this.cipherer)  && (XVar)(this.cipherer.isEncryptionByPHPEnabled()))
				{
					mValue = XVar.Clone(this.cipherer.MakeDBValue((XVar)(detailsKeys[i]), (XVar)(mKey)));
				}
				else
				{
					mValue = XVar.Clone(CommonFunctions.make_db_value((XVar)(detailsKeys[i]), (XVar)(mKey), new XVar(""), new XVar(""), (XVar)(this.tName)));
				}
				if(MVCFunctions.strlen((XVar)(mValue)) != 0)
				{
					where = MVCFunctions.Concat(where, this.getFieldSQLDecrypt((XVar)(detailsKeys[i])), "=", mValue);
				}
				else
				{
					where = MVCFunctions.Concat(where, "1=0");
				}
			}
			return where;
		}
		public virtual XVar showGridOnly()
		{
			return "";
		}
		public virtual XVar showPageDp(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			return this.showGridOnly();
		}
		public virtual XVar renderPageBody()
		{
			return this.xt.fetch_loaded(new XVar("body"));
		}
		public virtual XVar proccessDetailGridInfo(dynamic record, dynamic data, dynamic _param_gridRowInd)
		{
			#region pass-by-value parameters
			dynamic gridRowInd = XVar.Clone(_param_gridRowInd);
			#endregion

			dynamic detAddAvailabel = null, detEditAvailabel = null, detHref = null, detListAvailabel = null, hiddenPreviewTabs = XVar.Array(), hideDPLink = null, tabNamesToHide = null;
			hideDPLink = new XVar(true);
			hiddenPreviewTabs = XVar.Clone(XVar.Array());
			tabNamesToHide = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				dynamic dPset = null, dShortTable = null, detTableType = null, detailid = XVar.Array(), detailsKeysSettings = XVar.Array(), linkAttrs = null, masterquery = null;
				dPset = XVar.Clone(new ProjectSettings((XVar)(details.Value)));
				detTableType = XVar.Clone(dPset.getEntityType());
				detListAvailabel = XVar.Clone((XVar)((XVar)((XVar)(dPset.hasListPage())  || (XVar)(CommonFunctions.isChart((XVar)(detTableType))))  || (XVar)(CommonFunctions.isReport((XVar)(detTableType))))  && (XVar)(this.permis[details.Value]["search"]));
				detAddAvailabel = XVar.Clone((XVar)(dPset.hasAddPage())  && (XVar)(this.permis[details.Value]["add"]));
				detEditAvailabel = XVar.Clone((XVar)(dPset.hasEditPage())  && (XVar)(this.permis[details.Value]["edit"]));
				if((XVar)((XVar)(!(XVar)(detListAvailabel))  && (XVar)(!(XVar)(detAddAvailabel)))  && (XVar)(!(XVar)(detEditAvailabel)))
				{
					continue;
				}
				dShortTable = XVar.Clone(CommonFunctions.GetTableURL((XVar)(details.Value)));
				masterquery = XVar.Clone(MVCFunctions.Concat("mastertable=", MVCFunctions.RawUrlEncode((XVar)(this.tName))));
				CommonFunctions.initArray((XVar)(this.controlsMap), new XVar("gridRows"));
				CommonFunctions.initArray((XVar)(this.controlsMap["gridRows"]), (XVar)(gridRowInd));
				CommonFunctions.initArray((XVar)(this.controlsMap["gridRows"][gridRowInd]), new XVar("masterKeys"));
				this.controlsMap.InitAndSetArrayItem(XVar.Array(), "gridRows", gridRowInd, "masterKeys", details.Value);
				detailid = XVar.Clone(XVar.Array());
				detailsKeysSettings = XVar.Clone(this.pSet.getDetailsKeys((XVar)(details.Value)));
				foreach (KeyValuePair<XVar, dynamic> m in detailsKeysSettings["masterKeys"].GetEnumerator())
				{
					dynamic curM = null;
					curM = XVar.Clone(m.Value);
					if(this.pageType == Constants.PAGE_REPORT)
					{
						curM = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(curM)));
						curM = MVCFunctions.Concat(curM, "_dbvalue");
					}
					masterquery = MVCFunctions.Concat(masterquery, "&masterkey", m.Key + 1, "=", MVCFunctions.RawUrlEncode((XVar)(data[curM])));
					detailid.InitAndSetArrayItem(data[curM], null);
					this.controlsMap.InitAndSetArrayItem(data[curM], "gridRows", gridRowInd, "masterKeys", details.Value, MVCFunctions.Concat("masterkey", m.Key + 1));
				}
				if(XVar.Pack(!(XVar)(this.detailsInGridAvailable())))
				{
					continue;
				}
				if((XVar)((XVar)((XVar)(this.pSet.detailsShowCount((XVar)(details.Value)))  || (XVar)(this.pSet.detailsHideEmpty((XVar)(details.Value))))  || (XVar)(this.pSet.detailsHideEmptyPreview((XVar)(details.Value))))  && (XVar)(!(XVar)(this.isDetailTableSubqueryApplied((XVar)(details.Value)))))
				{
					data.InitAndSetArrayItem(this.countDetailsRecsNoSubQ((XVar)(details.Value), (XVar)(detailid)), MVCFunctions.Concat(details.Value, "_cnt"));
				}
				if(XVar.Pack(this.detailInGridAvailable((XVar)(details.Value))))
				{
					record.InitAndSetArrayItem((XVar)((XVar)(this.permis[details.Value]["add"])  || (XVar)(this.permis[details.Value]["edit"]))  || (XVar)(this.permis[details.Value]["search"]), MVCFunctions.Concat(dShortTable, "_dtable_link"));
				}
				if(XVar.Pack(this.pSet.detailsShowCount((XVar)(details.Value))))
				{
					if(data[MVCFunctions.Concat(details.Value, "_cnt")] + 0)
					{
						record.InitAndSetArrayItem(true, MVCFunctions.Concat(dShortTable, "_childcount"));
					}
					else
					{
						if(this.pSet.detailsLinks() != Constants.DL_INDIVIDUAL)
						{
							record.InitAndSetArrayItem(true, MVCFunctions.Concat(dShortTable, "_childcount"));
						}
						record.InitAndSetArrayItem("hidden-badge", MVCFunctions.Concat(dShortTable, "_dlink_class"));
						record.InitAndSetArrayItem("hidden-detcounter", MVCFunctions.Concat(dShortTable, "_cntspan_class"));
					}
					record.InitAndSetArrayItem(data[MVCFunctions.Concat(details.Value, "_cnt")], MVCFunctions.Concat(dShortTable, "_childnumber"));
					record.InitAndSetArrayItem(MVCFunctions.Concat(" id='cntDet_", dShortTable, "_", this.recId, "'"), MVCFunctions.Concat(dShortTable, "_childnumber_attr"));
					this.controlsMap.InitAndSetArrayItem(data[MVCFunctions.Concat(details.Value, "_cnt")], "gridRows", gridRowInd, "childNum");
				}
				if(XVar.Pack(detListAvailabel))
				{
					detHref = XVar.Clone(MVCFunctions.GetTableLink((XVar)(dShortTable), (XVar)(ProjectSettings.defaultPageType((XVar)(CommonFunctions.GetEntityType((XVar)(details.Value))))), (XVar)(masterquery)));
				}
				else
				{
					if(XVar.Pack(detAddAvailabel))
					{
						detHref = XVar.Clone(MVCFunctions.GetTableLink((XVar)(dShortTable), new XVar(Constants.PAGE_ADD), (XVar)(masterquery)));
					}
					else
					{
						if(XVar.Pack(detEditAvailabel))
						{
							detHref = XVar.Clone(MVCFunctions.GetTableLink((XVar)(dShortTable), new XVar(Constants.PAGE_EDIT), (XVar)(masterquery)));
						}
					}
				}
				if(XVar.Pack(this.detailsHrefAvailable()))
				{
					linkAttrs = XVar.Clone(MVCFunctions.Concat(" href=\"", detHref));
				}
				record.InitAndSetArrayItem(MVCFunctions.Concat(linkAttrs, "\" id=\"details_", this.recId, "_", dShortTable, "\" "), MVCFunctions.Concat(dShortTable, "_link_attrs"));
				if(XVar.Pack(this.pSet.detailsHideEmpty((XVar)(details.Value))))
				{
					if(XVar.Pack(!(XVar)(data[MVCFunctions.Concat(details.Value, "_cnt")] + 0)))
					{
						record[MVCFunctions.Concat(dShortTable, "_link_attrs")] = MVCFunctions.Concat(record[MVCFunctions.Concat(dShortTable, "_link_attrs")], "data-hidden");
					}
					else
					{
						if((XVar)(this.pSet.detailsPreview((XVar)(details.Value)))  && (XVar)(hideDPLink))
						{
							hideDPLink = new XVar(false);
						}
					}
				}
				else
				{
					if(XVar.Pack(hideDPLink))
					{
						hideDPLink = new XVar(false);
					}
				}
				if((XVar)(this.pSet.detailsHideEmptyPreview((XVar)(details.Value)))  && (XVar)(!(XVar)(data[MVCFunctions.Concat(details.Value, "_cnt")] + 0)))
				{
					hiddenPreviewTabs.InitAndSetArrayItem(details.Value, null);
				}
			}
			if(this.pSet.detailsLinks() == Constants.DL_SINGLE)
			{
				if((XVar)((XVar)(MVCFunctions.count(this.pSet.getDetailsTables()) == 1)  && (XVar)(!(XVar)(detListAvailabel)))  && (XVar)((XVar)(detAddAvailabel)  || (XVar)(detEditAvailabel)))
				{
					record.InitAndSetArrayItem(MVCFunctions.Concat(" href=\"", detHref, "\""), "dtables_link_attrs");
				}
				else
				{
					record.InitAndSetArrayItem(MVCFunctions.Concat(" href=\"#\" id=\"details_", this.recId, "\" "), "dtables_link_attrs");
				}
			}
			if(XVar.Pack(hideDPLink))
			{
				record["dtables_link_attrs"] = MVCFunctions.Concat(record["dtables_link_attrs"], " class=\"", this.makeClassName(new XVar("hiddenelem")), "\" data-hidden");
				record.InitAndSetArrayItem(this.makeClassName(new XVar("hiddenelem")), "dtables_link_class");
			}
			if((XVar)(this.pSet.detailsLinks() == Constants.DL_SINGLE)  && (XVar)(MVCFunctions.count(tabNamesToHide)))
			{
				record["dtables_link_attrs"] = MVCFunctions.Concat(record["dtables_link_attrs"], " data-hiddentabs=\"", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.runner_json_encode((XVar)(tabNamesToHide)))), "\"");
				this.controlsMap.InitAndSetArrayItem(tabNamesToHide, "gridRows", gridRowInd, "hiddentabs");
			}
			if(XVar.Pack(hiddenPreviewTabs))
			{
				this.controlsMap.InitAndSetArrayItem(hiddenPreviewTabs, "gridRows", gridRowInd, "hiddenPreviewTabs");
			}

			return null;
		}
		public virtual XVar getProceedUrl()
		{
			dynamic i = null, strkey = null;
			i = new XVar(1);
			for(;i <= MVCFunctions.count(this.masterKeysReq); i++)
			{
				strkey = MVCFunctions.Concat(strkey, "&masterkey", i, "=", MVCFunctions.RawUrlEncode((XVar)(this.masterKeysReq[i])));
			}
			return MVCFunctions.Concat(MVCFunctions.GetTableLink((XVar)(this.shortTableName), (XVar)(this.pageType)), "?mastertable=", MVCFunctions.RawUrlEncode((XVar)(this.masterTable)), strkey);
		}
		protected virtual XVar isDetailTableSubquerySupported(dynamic _param_details)
		{
			#region pass-by-value parameters
			dynamic details = XVar.Clone(_param_details);
			#endregion

			return false;
		}
		protected virtual XVar isDetailTableSubqueryApplied(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			return false;
		}
		public virtual XVar getDetailsParams(dynamic _param_ids)
		{
			#region pass-by-value parameters
			dynamic ids = XVar.Clone(_param_ids);
			#endregion

			dynamic dpParams = XVar.Array();
			dpParams = XVar.Clone(XVar.Array());
			if((XVar)((XVar)(this.pageType != Constants.PAGE_VIEW)  && (XVar)(this.pageType != Constants.PAGE_EDIT))  && (XVar)(this.pageType != Constants.PAGE_ADD))
			{
				return dpParams;
			}
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				dynamic dpPermis = XVar.Array();
				if(this.pSet.detailsPreview((XVar)(details.Value)) != Constants.DP_INLINE)
				{
					continue;
				}
				dpPermis = XVar.Clone(this.getPermissions((XVar)(details.Value)));
				if((XVar)((XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_VIEW)  || (XVar)(this.pageType == Constants.PAGE_EDIT))  && (XVar)(dpPermis["search"]))  || (XVar)((XVar)(this.pageType == Constants.PAGE_EDIT)  && (XVar)(dpPermis["edit"])))  || (XVar)((XVar)(this.pageType == Constants.PAGE_ADD)  && (XVar)(dpPermis["add"])))
				{
					dpParams.InitAndSetArrayItem(++(ids), "ids", null);
					dpParams.InitAndSetArrayItem(details.Value, "strTableNames", null);
					dpParams.InitAndSetArrayItem(ProjectSettings.defaultPageType((XVar)(CommonFunctions.GetEntityType((XVar)(details.Value)))), "type", null);
					dpParams.InitAndSetArrayItem(CommonFunctions.GetTableURL((XVar)(details.Value)), "shorTNames", null);
				}
			}
			return dpParams;
		}
		public virtual XVar setDetailPreview(dynamic _param_dpType, dynamic _param_dpTableName, dynamic _param_dpId, dynamic data)
		{
			#region pass-by-value parameters
			dynamic dpType = XVar.Clone(_param_dpType);
			dynamic dpTableName = XVar.Clone(_param_dpTableName);
			dynamic dpId = XVar.Clone(_param_dpId);
			#endregion

			if((XVar)((XVar)((XVar)(this.pageType != Constants.PAGE_EDIT)  && (XVar)(this.pageType != Constants.PAGE_VIEW))  && (XVar)(this.pageType != Constants.PAGE_ADD))  || (XVar)(!(XVar)(CommonFunctions.CheckTablePermissions((XVar)(dpTableName), new XVar("S")))))
			{
				return null;
			}
			if(dpType == Constants.PAGE_CHART)
			{
				this.setDetailChartOnEditView((XVar)(dpTableName), (XVar)(dpId), (XVar)(data));
			}
			else
			{
				if(dpType == Constants.PAGE_REPORT)
				{
					this.setDetailReportOnEditView((XVar)(dpTableName), (XVar)(dpId), (XVar)(data));
				}
				else
				{
					this.setDetailList((XVar)(dpTableName), (XVar)(dpId), (XVar)(data));
				}
			}

			return null;
		}
		protected virtual XVar getDetailsPageObject(dynamic _param_tName, dynamic _param_listId = null, dynamic _param_data = null)
		{
			#region default values
			if(_param_listId as Object == null) _param_listId = new XVar(0);
			if(_param_data as Object == null) _param_data = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic tName = XVar.Clone(_param_tName);
			dynamic listId = XVar.Clone(_param_listId);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic entityType = null, mKeys = XVar.Array(), masterKeys = XVar.Array(), mkr = null, options = XVar.Array(), pageObject = null;
			if(XVar.Pack(this.detailsTableObjects[tName]))
			{
				return this.detailsTableObjects[tName];
			}
			if(XVar.Pack(!(XVar)(listId)))
			{
				return null;
			}
			entityType = XVar.Clone(CommonFunctions.GetEntityType((XVar)(tName)));
			options = XVar.Clone(XVar.Array());
			options.InitAndSetArrayItem(listId, "id");
			options.InitAndSetArrayItem(1, "firstTime");
			options.InitAndSetArrayItem(this.tName, "masterTable");
			options.InitAndSetArrayItem(this.pageType, "masterPageType");
			options.InitAndSetArrayItem(new XTempl(), "xt");
			options.InitAndSetArrayItem(this.genId() + 1, "flyId");
			options.InitAndSetArrayItem(XVar.Array(), "masterKeysReq");
			options.InitAndSetArrayItem(false, "pushContext");
			if(XVar.Pack(this.pdfJsonMode()))
			{
				options.InitAndSetArrayItem(true, "pdfJson");
			}
			mkr = new XVar(1);
			mKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(tName)));
			masterKeys = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> mk in mKeys["masterKeys"].GetEnumerator())
			{
				options.InitAndSetArrayItem(data[mk.Value], "masterKeysReq", mkr);
				masterKeys.InitAndSetArrayItem(data[mk.Value], MVCFunctions.Concat("masterKey", mkr));
				mkr++;
			}
			options.InitAndSetArrayItem(this.pSet.detailsPageId((XVar)(tName)), "pageName");
			if((XVar)((XVar)((XVar)(Constants.titTABLE == entityType)  || (XVar)(Constants.titVIEW == entityType))  || (XVar)(Constants.titSQL == entityType))  || (XVar)(Constants.titREST == entityType))
			{
				options.InitAndSetArrayItem((XVar.Pack(this.pdfJsonMode()) ? XVar.Pack(Constants.LIST_PDFJSON) : XVar.Pack(Constants.LIST_DETAILS)), "mode");
				options.InitAndSetArrayItem(Constants.PAGE_LIST, "pageType");
				pageObject = XVar.Clone(ListPage.createListPage((XVar)(tName), (XVar)(options)));
			}
			else
			{
				if(XVar.Pack(CommonFunctions.isReport((XVar)(entityType))))
				{
					options.InitAndSetArrayItem(tName, "tName");
					options.InitAndSetArrayItem(Constants.REPORT_DETAILS, "mode");
					options.InitAndSetArrayItem(Constants.PAGE_REPORT, "pageType");
					pageObject = XVar.Clone(new ReportPage((XVar)(options)));
				}
				else
				{
					if(XVar.Pack(CommonFunctions.isChart((XVar)(entityType))))
					{
						options.InitAndSetArrayItem(tName, "tName");
						options.InitAndSetArrayItem(Constants.CHART_DETAILS, "mode");
						options.InitAndSetArrayItem(Constants.PAGE_CHART, "pageType");
						pageObject = XVar.Clone(new ChartPage((XVar)(options)));
					}
				}
			}
			this.detailsTableObjects.InitAndSetArrayItem(pageObject, tName);
			return pageObject;
		}
		public virtual XVar assignButtonsOnMasterEdit(dynamic _param_masterXt)
		{
			#region pass-by-value parameters
			dynamic masterXt = XVar.Clone(_param_masterXt);
			#endregion


			return null;
		}
		protected virtual XVar setDetailList(dynamic _param_listTName, dynamic _param_listId, dynamic data)
		{
			#region pass-by-value parameters
			dynamic listTName = XVar.Clone(_param_listTName);
			dynamic listId = XVar.Clone(_param_listId);
			#endregion

			dynamic listPageObject = null;
			listPageObject = XVar.Clone(this.getDetailsPageObject((XVar)(listTName), (XVar)(listId), (XVar)(data)));
			RunnerContext.push((XVar)(listPageObject.standaloneContext));
			listPageObject.prepareForBuildPage();
			if(XVar.Pack(listPageObject.shouldDisplayDetailsPage()))
			{
				foreach (KeyValuePair<XVar, dynamic> name in listPageObject.eventsObject.events.GetEnumerator())
				{
					listPageObject.xt.assign_event((XVar)(name.Key), (XVar)(listPageObject.eventsObject), (XVar)(name.Key), (XVar)(XVar.Array()));
				}
				listPageObject.addControlsJSAndCSS();
				listPageObject.fillSetCntrlMaps();
				listPageObject.BeforeShowList();
				this.assignDisplayDetailTableXtVariable((XVar)(listPageObject));
				this.copyDetailPreviewJSAndCSS((XVar)(listPageObject));
				this.updateSettingsWidthDPData((XVar)(listPageObject));
				this.viewControlsMap.InitAndSetArrayItem(listPageObject.viewControlsMap, "dViewControlsMap", listTName);
				this.controlsMap.InitAndSetArrayItem(listPageObject.controlsMap, "dControlsMap", listTName);
				if(this.pageType == Constants.PAGE_EDIT)
				{
					dynamic masterKeys = null;
					this.controlsMap.InitAndSetArrayItem(masterKeys, "dControlsMap", "masterKeys");
				}
				this.controlsMap.InitAndSetArrayItem(new XVar("tName", listTName, "id", listId, "pType", Constants.PAGE_LIST), "dpTablesParams", null);
			}
			this.flyId = XVar.Clone(listPageObject.recId + 1);
			RunnerContext.pop();

			return null;
		}
		protected virtual XVar setDetailReportOnEditView(dynamic _param_reportTName, dynamic _param_reportId, dynamic data)
		{
			#region pass-by-value parameters
			dynamic reportTName = XVar.Clone(_param_reportTName);
			dynamic reportId = XVar.Clone(_param_reportId);
			#endregion

			dynamic mKeys = XVar.Array(), mkr = null, options = XVar.Array(), reportPageObject = null;
			options = XVar.Clone(XVar.Array());
			options.InitAndSetArrayItem(reportId, "id");
			options.InitAndSetArrayItem(Constants.REPORT_DETAILS, "mode");
			options.InitAndSetArrayItem(reportTName, "tName");
			options.InitAndSetArrayItem(Constants.PAGE_REPORT, "pageType");
			options.InitAndSetArrayItem(this.pageType, "masterPageType");
			options.InitAndSetArrayItem(this.tName, "masterTable");
			options.InitAndSetArrayItem(new XTempl(), "xt");
			options.InitAndSetArrayItem(this.genId() + 1, "flyId");
			options.InitAndSetArrayItem(XVar.Array(), "masterKeysReq");
			options.InitAndSetArrayItem(false, "pushContext");
			if(XVar.Pack(this.pdfJsonMode()))
			{
				options.InitAndSetArrayItem(true, "pdfJson");
			}
			options.InitAndSetArrayItem(this.pSet.detailsPageId((XVar)(reportTName)), "pageName");
			mkr = new XVar(1);
			mKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(reportTName)));
			foreach (KeyValuePair<XVar, dynamic> mk in mKeys["masterKeys"].GetEnumerator())
			{
				options.InitAndSetArrayItem(data[mk.Value], "masterKeysReq", mkr++);
			}
			reportPageObject = XVar.Clone(new ReportPage((XVar)(options)));
			RunnerContext.push((XVar)(reportPageObject.standaloneContext));
			reportPageObject.init();
			reportPageObject.processGridTabs();
			reportPageObject.prepareDetailsForEditViewPage();
			if(XVar.Pack(!(XVar)(reportPageObject.shouldDisplayDetailsPage())))
			{
				return false;
			}
			reportPageObject.addControlsJSAndCSS();
			reportPageObject.fillSetCntrlMaps();
			reportPageObject.beforeShowReport();
			this.assignDisplayDetailTableXtVariable((XVar)(reportPageObject));
			this.copyDetailPreviewJSAndCSS((XVar)(reportPageObject));
			this.updateSettingsWidthDPData((XVar)(reportPageObject));
			this.viewControlsMap.InitAndSetArrayItem(reportPageObject.viewControlsMap, "dViewControlsMap", reportTName);
			this.controlsMap.InitAndSetArrayItem(reportPageObject.controlsMap, "dControlsMap", reportTName);
			this.controlsMap.InitAndSetArrayItem(new XVar("tName", reportTName, "id", options["id"], "pType", Constants.PAGE_REPORT), "dpTablesParams", null);
			RunnerContext.pop();

			return null;
		}
		protected virtual XVar setDetailChartOnEditView(dynamic _param_chartTName, dynamic _param_chartId, dynamic data)
		{
			#region pass-by-value parameters
			dynamic chartTName = XVar.Clone(_param_chartTName);
			dynamic chartId = XVar.Clone(_param_chartId);
			#endregion

			dynamic chartPageObject = null, chartXtParams = XVar.Array(), mKeys = XVar.Array(), masterKeysReq = XVar.Array(), mkr = null, options = XVar.Array();
			XTempl xt;
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return null;
			}
			xt = XVar.UnPackXTempl(new XTempl());
			options = XVar.Clone(XVar.Array());
			options.InitAndSetArrayItem(xt, "xt");
			options.InitAndSetArrayItem(chartId, "id");
			options.InitAndSetArrayItem(chartTName, "tName");
			options.InitAndSetArrayItem(Constants.CHART_DETAILS, "mode");
			options.InitAndSetArrayItem(Constants.PAGE_CHART, "pageType");
			options.InitAndSetArrayItem(this.pageType, "masterPageType");
			options.InitAndSetArrayItem(this.tName, "masterTable");
			options.InitAndSetArrayItem(this.genId() + 1, "flyId");
			options.InitAndSetArrayItem(false, "pushContext");
			options.InitAndSetArrayItem(this.pSet.detailsPageId((XVar)(chartTName)), "pageName");
			mkr = new XVar(1);
			mKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(chartTName)));
			foreach (KeyValuePair<XVar, dynamic> mk in mKeys["masterKeys"].GetEnumerator())
			{
				options.InitAndSetArrayItem(data[mk.Value], "masterKeysReq", mkr++);
			}
			masterKeysReq = XVar.Clone(options["masterKeysReq"]);
			if(XVar.Pack(MVCFunctions.count(masterKeysReq)))
			{
				dynamic i = null;
				i = new XVar(1);
				for(;i <= MVCFunctions.count(masterKeysReq); i++)
				{
					XSession.Session[MVCFunctions.Concat(chartTName, "_masterkey", i)] = masterKeysReq[i];
				}
				if(XVar.Pack(XSession.Session.KeyExists(MVCFunctions.Concat(chartTName, "_masterkey", i))))
				{
					XSession.Session.Remove(MVCFunctions.Concat(chartTName, "_masterkey", i));
				}
			}
			chartPageObject = XVar.Clone(new ChartPage((XVar)(options)));
			RunnerContext.push((XVar)(chartPageObject.standaloneContext));
			chartPageObject.init();
			chartXtParams.InitAndSetArrayItem(options["flyId"], "id");
			chartXtParams.InitAndSetArrayItem(chartTName, "table");
			chartXtParams.InitAndSetArrayItem(chartPageObject.pSet.getChartType(), "ctype");
			chartXtParams.InitAndSetArrayItem(chartPageObject.shortTableName, "chartName");
			chartXtParams.InitAndSetArrayItem(true, "singlePage");
			chartXtParams.InitAndSetArrayItem(MVCFunctions.Concat("rnr", chartXtParams["chartName"], chartXtParams["id"]), "containerId");
			xt.assign_function((XVar)(MVCFunctions.Concat(chartPageObject.shortTableName, "_chart")), new XVar("xt_showchart"), (XVar)(chartXtParams));
			chartPageObject.processGridTabs();
			chartPageObject.prepareDetailsForEditViewPage();
			chartPageObject.addControlsJSAndCSS();
			chartPageObject.fillSetCntrlMaps();
			this.AddJSFile(new XVar("libs/js/anychart.min.js"));
			this.AddCSSFile(new XVar("libs/js/anychart-ui.min.css"));
			this.AddCSSFile(new XVar("libs/js/anychart-font.min.css"));
			chartPageObject.beforeShowEvent();
			this.assignDisplayDetailTableXtVariable((XVar)(chartPageObject));
			this.copyDetailPreviewJSAndCSS((XVar)(chartPageObject));
			this.updateSettingsWidthDPData((XVar)(chartPageObject));
			this.viewControlsMap.InitAndSetArrayItem(chartPageObject.viewControlsMap, "dViewControlsMap", chartTName);
			this.controlsMap.InitAndSetArrayItem(chartPageObject.controlsMap, "dControlsMap", chartTName);
			this.controlsMap.InitAndSetArrayItem(new XVar("tName", chartTName, "id", options["id"], "pType", Constants.PAGE_CHART), "dpTablesParams", null);
			RunnerContext.pop();

			return null;
		}
		protected virtual XVar getKeysFromData(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic keyFields = XVar.Array(), keys = XVar.Array();
			keys = XVar.Clone(XVar.Array());
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			foreach (KeyValuePair<XVar, dynamic> keyField in keyFields.GetEnumerator())
			{
				keys.InitAndSetArrayItem(data[keyField.Value], keyField.Value);
			}
			return keys;
		}
		protected virtual XVar copyDetailPreviewJSAndCSS(dynamic dtPageObject)
		{
			dynamic layout = null;
			layout = XVar.Clone(CommonFunctions.GetPageLayout((XVar)(dtPageObject.tName), (XVar)(dtPageObject.pageType)));
			if(XVar.Pack(layout))
			{
				this.AddCSSFile((XVar)(layout.getCSSFiles((XVar)(CommonFunctions.isRTL()))));
			}
			this.copyAllJsFiles((XVar)(dtPageObject.grabAllJsFiles()));
			this.copyAllCssFiles((XVar)(dtPageObject.grabAllCSSFiles()));

			return null;
		}
		protected virtual XVar updateSettingsWidthDPData(dynamic dtPageObject)
		{
			dynamic tName = null;
			tName = XVar.Clone(dtPageObject.tName);
			this.jsSettings.InitAndSetArrayItem(dtPageObject.jsSettings["tableSettings"][tName], "tableSettings", tName);

			return null;
		}
		protected virtual XVar assignDisplayDetailTableXtVariable(dynamic dtPageObject)
		{
			dtPageObject.prepareDisplayDetails();
			this.xt.assign_method((XVar)(MVCFunctions.Concat("detailslButtons_", dtPageObject.shortTableName)), (XVar)(dtPageObject), new XVar("showButtonsDp"), new XVar(false));
			this.xt.assign((XVar)(MVCFunctions.Concat("proceedAttrs_", dtPageObject.shortTableName)), (XVar)(MVCFunctions.Concat("href=\"", dtPageObject.getProceedUrl(), "\"")));
			this.xt.assign((XVar)(MVCFunctions.Concat("details_pagetitle_", dtPageObject.shortTableName)), (XVar)(dtPageObject.getPageTitle((XVar)(dtPageObject.pageName), (XVar)(dtPageObject.tName))));
			this.xt.assign((XVar)(MVCFunctions.Concat("details_", dtPageObject.shortTableName)), new XVar(true));
			this.xt.assign_method((XVar)(MVCFunctions.Concat("displayDetailTable_", dtPageObject.shortTableName)), (XVar)(dtPageObject), new XVar("showPageDp"), new XVar(false));

			return null;
		}
		protected static XVar getKeysTitleTemplate(dynamic _param_table, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic keys = XVar.Array(), str = null;
			keys = XVar.Clone(pSet.getTableKeys());
			str = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> k in keys.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(str))))
				{
					str = MVCFunctions.Concat(str, ", ");
				}
				str = MVCFunctions.Concat(str, "{%", MVCFunctions.GoodFieldName((XVar)(k.Value)), "}");
			}
			return str;
		}
		public static XVar getDefaultPageTitle(dynamic _param_page, dynamic _param_table, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			dynamic table = XVar.Clone(_param_table);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic caption = null;
			caption = XVar.Clone(Labels.getTableCaption((XVar)(table)));
			if(page == "add")
			{
				return MVCFunctions.Concat(caption, ", ", CommonFunctions.mlang_message(new XVar("ADD_NEW")));
			}
			if(page == "edit")
			{
				return MVCFunctions.Concat(caption, ", ", CommonFunctions.mlang_message(new XVar("EDIT")), " [", RunnerPage.getKeysTitleTemplate((XVar)(table), (XVar)(pSet)), "]");
			}
			if(page == "view")
			{
				return MVCFunctions.Concat(caption, " [", RunnerPage.getKeysTitleTemplate((XVar)(table), (XVar)(pSet)), "]");
			}
			if(page == "export")
			{
				return CommonFunctions.mlang_message(new XVar("EXPORT"));
			}
			if(page == "import")
			{
				return MVCFunctions.Concat(caption, ", ", CommonFunctions.mlang_message(new XVar("IMPORT")));
			}
			if(page == "search")
			{
				return MVCFunctions.Concat(caption, " - ", CommonFunctions.mlang_message(new XVar("ADVANCED_SEARCH")));
			}
			if(page == "print")
			{
				return caption;
			}
			if(page == "rprint")
			{
				return caption;
			}
			if(page == "list")
			{
				return caption;
			}
			if(page == "masterlist")
			{
				return MVCFunctions.Concat(caption, " [", RunnerPage.getKeysTitleTemplate((XVar)(table), (XVar)(pSet)), "]");
			}
			if(page == "masterchart")
			{
				return caption;
			}
			if(page == "masterreport")
			{
				return MVCFunctions.Concat(caption, " [", RunnerPage.getKeysTitleTemplate((XVar)(table), (XVar)(pSet)), "]");
			}
			if(page == "masterprint")
			{
				return MVCFunctions.Concat(caption, " [", RunnerPage.getKeysTitleTemplate((XVar)(table), (XVar)(pSet)), "]");
			}
			if(page == "login")
			{
				return CommonFunctions.mlang_message(new XVar("LOGIN"));
			}
			if(page == "register")
			{
				return CommonFunctions.mlang_message(new XVar("REGISTER"));
			}
			if(page == "register_success")
			{
				return CommonFunctions.mlang_message(new XVar("REG_SUCCESS"));
			}
			if(page == "changepwd")
			{
				return CommonFunctions.mlang_message(new XVar("CHANGE_PASSWORD"));
			}
			if(page == "changepwd_success")
			{
				return CommonFunctions.mlang_message(new XVar("CHANGE_PASSWORD"));
			}
			if(page == "remind")
			{
				return CommonFunctions.mlang_message(new XVar("REMINDER"));
			}
			if(page == "remind_success")
			{
				return CommonFunctions.mlang_message(new XVar("REMINDER"));
			}
			if(page == "chart")
			{
				return caption;
			}
			if(page == "report")
			{
				return caption;
			}
			if(page == "dashboard")
			{
				return caption;
			}
			if(page == "menu")
			{
				return CommonFunctions.mlang_message(new XVar("MENU"));
			}
			if((XVar)((XVar)(page == "admin_rights_list")  || (XVar)(page == "admin_members_list"))  || (XVar)(page == "admin_admembers_list"))
			{
				return caption;
			}
			if(page == Constants.PAGE_USERINFO)
			{
				return CommonFunctions.mlang_message(new XVar("USERINFO"));
			}
			return caption;
		}
		protected virtual XVar getPageTitleTemplate(dynamic _param_page, dynamic _param_table, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			dynamic table = XVar.Clone(_param_table);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			if((XVar)((XVar)(!(XVar)(table))  || (XVar)(page == Constants.PAGE_REGISTER))  || (XVar)(page == Constants.PAGE_USERINFO))
			{
				table = new XVar(Constants.GLOBAL_PAGES);
			}
			if(XVar.Pack(GlobalVars.runnerTableLabels[table]["pageTitles"].KeyExists(page)))
			{
				return GlobalVars.runnerTableLabels[table]["pageTitles"][page];
			}
			return RunnerPage.getDefaultPageTitle((XVar)(pSet.getPageType()), (XVar)(table), (XVar)(pSet));
		}
		public virtual XVar getPageTitle(dynamic _param_page, dynamic _param_table = null, dynamic _param_record = null, dynamic _param_settings = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			if(_param_record as Object == null) _param_record = new XVar();
			if(_param_settings as Object == null) _param_settings = new XVar();
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			dynamic table = XVar.Clone(_param_table);
			dynamic record = XVar.Clone(_param_record);
			dynamic settings = XVar.Clone(_param_settings);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			if((XVar)((XVar)((XVar)(this.isDashboardElement())  && (XVar)(this.dashElementData["table"] == table))  && (XVar)(this.dashElementData["item"]["customLabel"]))  && (XVar)(this.dashElementData["item"]["dashLabel"]))
			{
				return CommonFunctions.GetMLString((XVar)(this.dashElementData["item"]["dashLabel"]));
			}
			return this._getPageTitle((XVar)(page), (XVar)(table), (XVar)(record), (XVar)(settings), (XVar)(html));
		}
		public virtual XVar _getPageTitle(dynamic _param_page, dynamic _param_table = null, dynamic _param_record = null, dynamic _param_settings = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			if(_param_record as Object == null) _param_record = new XVar();
			if(_param_settings as Object == null) _param_settings = new XVar();
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			dynamic table = XVar.Clone(_param_table);
			dynamic record = XVar.Clone(_param_record);
			dynamic settings = XVar.Clone(_param_settings);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic currentRecord = null, masterRecord = null, templ = null;
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings((XVar.Pack(XVar.Equals(XVar.Pack(settings), XVar.Pack(null))) ? XVar.Pack(this.pSet) : XVar.Pack(settings)));
			templ = XVar.Clone(this.getPageTitleTemplate((XVar)(page), (XVar)(table), (XVar)(pSet)));
			masterRecord = XVar.Clone(XVar.Array());
			if(!XVar.Equals(XVar.Pack(MVCFunctions.stripos((XVar)(templ), new XVar("{%master."))), XVar.Pack(false)))
			{
				masterRecord = XVar.Clone(this.getMasterRecord());
			}
			currentRecord = XVar.Clone(XVar.Array());
			if(XVar.Pack(record))
			{
				currentRecord = XVar.Clone(record);
			}
			else
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.preg_match(new XVar("/{\\%(?!master\\.)[\\w\\s\\-\\.]*}/"), (XVar)(templ))), XVar.Pack(false)))
				{
					currentRecord = XVar.Clone(this.getCurrentRecord());
				}
			}
			return this.calcPageTitle((XVar)(templ), (XVar)(currentRecord), (XVar)(this.masterTable), (XVar)(masterRecord), (XVar)(pSet), (XVar)(html));
		}
		public virtual XVar calcPageTitle(dynamic _param_templ, dynamic _param_currentRecord = null, dynamic _param_masterTable = null, dynamic _param_masterRecord = null, dynamic _param_pSet_packed = null, dynamic _param_html = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_currentRecord as Object == null) _param_currentRecord = new XVar(XVar.Array());
			if(_param_masterTable as Object == null) _param_masterTable = new XVar("");
			if(_param_masterRecord as Object == null) _param_masterRecord = new XVar(XVar.Array());
			if(_param_pSet as Object == null) _param_pSet = null;
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic templ = XVar.Clone(_param_templ);
			dynamic currentRecord = XVar.Clone(_param_currentRecord);
			dynamic masterTable = XVar.Clone(_param_masterTable);
			dynamic masterRecord = XVar.Clone(_param_masterRecord);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic matches = XVar.Array();
			if(XVar.Pack(!(XVar)(pSet)))
			{
				pSet = XVar.UnPackProjectSettings(this.pSet);
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(masterRecord)))))
			{
				masterRecord = XVar.Clone(XVar.Array());
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(currentRecord)))))
			{
				currentRecord = XVar.Clone(XVar.Array());
			}
			matches = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(MVCFunctions.preg_match_all(new XVar("/{\\%([\\w\\.\\s\\-]*)\\}/"), (XVar)(templ), (XVar)(matches)))))
			{
				return templ;
			}
			foreach (KeyValuePair<XVar, dynamic> m in matches[0].GetEnumerator())
			{
				dynamic field = null;
				if(XVar.Pack(!(XVar)(MVCFunctions.strcasecmp((XVar)(MVCFunctions.substr((XVar)(m.Value), new XVar(0), new XVar(9))), new XVar("{%master.")))))
				{
					dynamic mSettings = null, masterViewControl = null;
					mSettings = XVar.Clone(new ProjectSettings((XVar)(masterTable), new XVar(Constants.PAGE_LIST)));
					field = XVar.Clone(mSettings.getFieldByGoodFieldName((XVar)(MVCFunctions.trim((XVar)(MVCFunctions.substr((XVar)(m.Value), new XVar(9), (XVar)(MVCFunctions.strlen((XVar)(m.Value)) - 10)))))));
					masterViewControl = XVar.Clone(new ViewControlsContainer((XVar)(mSettings), new XVar(Constants.PAGE_LIST)));
					templ = XVar.Clone(MVCFunctions.str_replace((XVar)(m.Value), (XVar)((XVar.Pack(masterRecord) ? XVar.Pack(masterViewControl.showDBValue((XVar)(field), (XVar)(masterRecord), new XVar(""), new XVar(""), (XVar)(html))) : XVar.Pack(""))), (XVar)(templ)));
				}
				else
				{
					dynamic fieldValue = null;
					field = XVar.Clone(pSet.getFieldByGoodFieldName((XVar)(MVCFunctions.trim((XVar)(MVCFunctions.substr((XVar)(m.Value), new XVar(2), (XVar)(MVCFunctions.strlen((XVar)(m.Value)) - 3)))))));
					fieldValue = new XVar("");
					if(XVar.Pack(currentRecord))
					{
						fieldValue = XVar.Clone((XVar.Pack(this.pdfJsonMode()) ? XVar.Pack(CommonFunctions.jsreplace((XVar)(this.getTextValue((XVar)(field), (XVar)(currentRecord))))) : XVar.Pack(this.showDBValue((XVar)(field), (XVar)(currentRecord), new XVar(""), (XVar)(html)))));
					}
					templ = XVar.Clone(MVCFunctions.str_replace((XVar)(m.Value), (XVar)(fieldValue), (XVar)(templ)));
				}
			}
			return templ;
		}
		public virtual XVar setPageTitle(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			this.xt.assign(new XVar("pagetitlelabel"), (XVar)(str));

			return null;
		}
		public virtual XVar getCurrentRecord()
		{
			return XVar.Array();
		}
		protected virtual XVar assignBody()
		{
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return null;
			}
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage(new XVar(false)));
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<div id=\"search_suggest", this.id, "\"></div>\r\n");
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
			this.xt.assign(new XVar("body"), (XVar)(this.body));

			return null;
		}
		public virtual XVar getInputElementId(dynamic _param_field, dynamic _param_pSet_packed = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_pSet as Object == null) _param_pSet = null;
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic format = null;
			if(XVar.Pack(!(XVar)(pSet)))
			{
				pSet = XVar.UnPackProjectSettings(this.pSet);
			}
			format = XVar.Clone(pSet.getEditFormat((XVar)(field)));
			if(format == Constants.EDIT_FORMAT_DATE)
			{
				dynamic var_type = null;
				var_type = XVar.Clone(pSet.getDateEditType((XVar)(field)));
				if((XVar)(var_type == Constants.EDIT_DATE_DD)  || (XVar)(var_type == Constants.EDIT_DATE_DD_DP))
				{
					return MVCFunctions.Concat("dayvalue_", MVCFunctions.GoodFieldName((XVar)(field)), "_", this.id);
				}
				else
				{
					return MVCFunctions.Concat("value_", MVCFunctions.GoodFieldName((XVar)(field)), "_", this.id);
				}
			}
			else
			{
				if(format == Constants.EDIT_FORMAT_RADIO)
				{
					return MVCFunctions.Concat("radio_", MVCFunctions.GoodFieldName((XVar)(field)), "_", this.id, "_0");
				}
				else
				{
					if(format == Constants.EDIT_FORMAT_LOOKUP_WIZARD)
					{
						dynamic lookuptype = null;
						lookuptype = XVar.Clone(pSet.lookupControlType((XVar)(field)));
						if((XVar)(lookuptype == Constants.LCT_AJAX)  || (XVar)(lookuptype == Constants.LCT_LIST))
						{
							return MVCFunctions.Concat("display_value_", MVCFunctions.GoodFieldName((XVar)(field)), "_", this.id);
						}
						else
						{
							return MVCFunctions.Concat("value_", MVCFunctions.GoodFieldName((XVar)(field)), "_", this.id);
						}
					}
					else
					{
						return MVCFunctions.Concat("value_", MVCFunctions.GoodFieldName((XVar)(field)), "_", this.id);
					}
				}
			}

			return null;
		}
		public virtual XVar getFieldControlsData()
		{
			return XVar.Array();
		}
		public virtual XVar isSearchPanelActivated()
		{
			return this.searchPanelActivated;
		}
		public virtual XVar keysSQLExpression(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic chunks = XVar.Array(), keyFields = XVar.Array();
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			chunks = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> kf in keyFields.GetEnumerator())
			{
				dynamic value = null, valueisnull = null;
				value = XVar.Clone(this.cipherer.MakeDBValue((XVar)(kf.Value), (XVar)(keys[kf.Value]), new XVar(""), new XVar(true)));
				if(this.connection.dbType == Constants.nDATABASE_Oracle)
				{
					valueisnull = XVar.Clone((XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack("null")))  || (XVar)(value == "''"));
				}
				else
				{
					valueisnull = XVar.Clone(XVar.Equals(XVar.Pack(value), XVar.Pack("null")));
				}
				if(XVar.Pack(valueisnull))
				{
					chunks.InitAndSetArrayItem(MVCFunctions.Concat(this.getFieldSQL((XVar)(kf.Value)), " is null"), null);
				}
				else
				{
					chunks.InitAndSetArrayItem(MVCFunctions.Concat(this.getFieldSQLDecrypt((XVar)(kf.Value)), "=", this.cipherer.MakeDBValue((XVar)(kf.Value), (XVar)(keys[kf.Value]), new XVar(""), new XVar(true))), null);
				}
			}
			return MVCFunctions.implode(new XVar(" and "), (XVar)(chunks));
		}
		public virtual XVar countTotals(dynamic totals, dynamic data)
		{
			dynamic curTotalFieldValue = null, i = null;
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.totalsFields); i++)
			{
				curTotalFieldValue = XVar.Clone(data[this.totalsFields[i]["fName"]]);
				if(XVar.Pack(!(XVar)(totals.KeyExists(this.totalsFields[i]["fName"]))))
				{
					totals.InitAndSetArrayItem(0, this.totalsFields[i]["fName"]);
				}
				if(this.totalsFields[i]["totalsType"] == "COUNT")
				{
					if((XVar)(this.totalsFields[i]["viewFormat"] == Constants.FORMAT_CHECKBOX)  && (XVar)((XVar)(XVar.Equals(XVar.Pack(curTotalFieldValue), XVar.Pack(null)))  || (XVar)(!(XVar)(curTotalFieldValue))))
					{
						continue;
					}
					if(0 != MVCFunctions.strlen((XVar)(curTotalFieldValue)))
					{
						totals[this.totalsFields[i]["fName"]]++;
					}
				}
				else
				{
					if(this.totalsFields[i]["viewFormat"] == "Time")
					{
						dynamic time = XVar.Array();
						time = XVar.Clone(CommonFunctions.GetTotalsForTime((XVar)(curTotalFieldValue)));
						totals[this.totalsFields[i]["fName"]] += (time[2] + time[1] * 60) + time[0] * 3600;
					}
					else
					{
						totals[this.totalsFields[i]["fName"]] += (double)curTotalFieldValue;
					}
				}
				if(this.totalsFields[i]["totalsType"] == "AVERAGE")
				{
					if((XVar)(!(XVar)(XVar.Equals(XVar.Pack(curTotalFieldValue), XVar.Pack(null))))  && (XVar)(!XVar.Equals(XVar.Pack(curTotalFieldValue), XVar.Pack(""))))
					{
						this.totalsFields[i]["numRows"]++;
					}
				}
			}

			return null;
		}
		protected virtual XVar getTotalDataCommand()
		{
			return this.getSubsetDataCommand();
		}
		public virtual XVar buildTotals(dynamic totals)
		{

			return null;
		}
		public virtual XVar getTotalsFromDB()
		{
			dynamic dc = null, rs = null, totalTypes = XVar.Array(), totals = XVar.Array();
			if(XVar.Pack(!(XVar)(this.totalsFields)))
			{
				return XVar.Array();
			}
			dc = XVar.Clone(this.getTotalDataCommand());
			if(XVar.Pack(this.pSet.getRecordsLimit()))
			{
				dc.reccount = XVar.Clone(this.pSet.getRecordsLimit());
			}
			totalTypes = XVar.Clone(new XVar("TOTAL", "sum", "AVERAGE", "avg", "COUNT", "count"));
			dc.totals = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> tf in this.totalsFields.GetEnumerator())
			{
				dc.totals.InitAndSetArrayItem(new XVar("total", totalTypes[tf.Value["totalsType"]], "alias", MVCFunctions.Concat(tf.Value["totalsType"], "_", tf.Value["fName"]), "field", tf.Value["fName"], "timeToSec", tf.Value["viewFormat"] == "Time"), null);
			}
			totals = XVar.Clone(XVar.Array());
			rs = XVar.Clone(this.dataSource.getTotals((XVar)(dc)));
			if(XVar.Pack(rs))
			{
				dynamic data = XVar.Array();
				data = XVar.Clone(rs.fetchAssoc());
				if(XVar.Pack(data))
				{
					foreach (KeyValuePair<XVar, dynamic> tf in this.totalsFields.GetEnumerator())
					{
						totals.InitAndSetArrayItem(data[MVCFunctions.Concat(tf.Value["totalsType"], "_", tf.Value["fName"])], tf.Value["fName"]);
					}
				}
			}
			return totals;
		}
		public virtual XVar buildAllDataTotals()
		{
			dynamic dbTotals = null;
			dbTotals = XVar.Clone(this.getTotalsFromDB());
			foreach (KeyValuePair<XVar, dynamic> tf in this.totalsFields.GetEnumerator())
			{
				this.totalsFields.InitAndSetArrayItem(1, tf.Key, "numRows");
			}
			this.buildTotals((XVar)(dbTotals));

			return null;
		}
		public static XVar deleteAvailable(dynamic instance)
		{
			return (XVar)(instance.pSet.hasDelete())  && (XVar)(instance.permis[instance.tName]["delete"]);
		}
		public virtual XVar deleteAvailable()
		{
			return (XVar)(this.pSet.hasDelete())  && (XVar)(this.permis[this.tName]["delete"]);
		}
		public virtual XVar importAvailable()
		{
			return (XVar)(this.permis[this.tName]["import"])  && (XVar)(this.pSet.hasImportPage());
		}
		public static XVar editAvailable(dynamic instance)
		{
			return (XVar)(instance.pSet.hasEditPage())  && (XVar)(instance.permis[instance.tName]["edit"]);
		}
		public virtual XVar editAvailable()
		{
			return (XVar)(this.pSet.hasEditPage())  && (XVar)(this.permis[this.tName]["edit"]);
		}
		public static XVar addAvailable(dynamic instance)
		{
			return (XVar)(instance.pSet.hasAddPage())  && (XVar)(instance.permis[instance.tName]["add"]);
		}
		public virtual XVar addAvailable()
		{
			return (XVar)(this.pSet.hasAddPage())  && (XVar)(this.permis[this.tName]["add"]);
		}
		public virtual XVar copyAvailable()
		{
			return (XVar)(this.pSet.hasCopyPage())  && (XVar)(this.permis[this.tName]["add"]);
		}
		public static XVar inlineEditAvailable(dynamic instance)
		{
			return (XVar)(instance.permis[instance.tName]["edit"])  && (XVar)(instance.pSet.hasInlineEdit());
		}
		public virtual XVar inlineEditAvailable()
		{
			return (XVar)(this.permis[this.tName]["edit"])  && (XVar)(this.pSet.hasInlineEdit());
		}
		public virtual XVar updateSelectedAvailable()
		{
			return (XVar)(this.permis[this.tName]["edit"])  && (XVar)(this.pSet.hasUpdateSelected());
		}
		public static XVar inlineAddAvailable(dynamic instance)
		{
			return (XVar)(instance.permis[instance.tName]["add"])  && (XVar)(instance.pSet.hasInlineAdd());
		}
		public virtual XVar inlineAddAvailable()
		{
			return (XVar)(this.permis[this.tName]["add"])  && (XVar)(this.pSet.hasInlineAdd());
		}
		public static XVar viewAvailable(dynamic instance)
		{
			return (XVar)(instance.permis[instance.tName]["search"])  && (XVar)(instance.pSet.hasViewPage());
		}
		public virtual XVar viewAvailable()
		{
			return (XVar)(this.permis[this.tName]["search"])  && (XVar)(this.pSet.hasViewPage());
		}
		public virtual XVar exportAvailable()
		{
			return (XVar)(this.permis[this.tName]["export"])  && (XVar)(this.pSet.hasExportPage());
		}
		public virtual XVar printAvailable()
		{
			return (XVar)(this.permis[this.tName]["export"])  && (XVar)(this.pSet.hasPrintPage());
		}
		public virtual XVar advSearchAvailable()
		{
			return (XVar)(this.permis[this.tName]["search"])  && (XVar)(MVCFunctions.count(this.pSet.getAdvSearchFields()));
		}
		public virtual XVar gridTabsAvailable()
		{
			return false;
		}
		public virtual XVar getIncludeFileMapProvider()
		{
			switch(((XVar)CommonFunctions.getMapProvider()).ToInt())
			{
				case Constants.GOOGLE_MAPS:
					return "gmap.js";
					break;
				case Constants.OPEN_STREET_MAPS:
					return "osmap.js";
					break;
				case Constants.BING_MAPS:
					return "bingmap.js";
					break;
				case Constants.HERE_MAPS:
					return "heremap.js";
					break;
				case Constants.MAPQUEST_MAPS:
					return "mapquest.js";
					break;
			}

			return null;
		}
		public virtual XVar includeOSMfile()
		{
			if(CommonFunctions.getMapProvider() == Constants.OPEN_STREET_MAPS)
			{
				this.AddJSFile(new XVar("plugins/OpenLayers.js"));
			}

			return null;
		}
		public virtual XVar isMultistepped()
		{
			return this.pSet.isMultistepped();
		}
		public virtual XVar prepareSteps()
		{
			if(XVar.Pack(!(XVar)(this.isMultistepped())))
			{
				return null;
			}
			this.xt.assign(new XVar("steps_block"), new XVar(true));
			this.controlsMap.InitAndSetArrayItem(this.initialStep, "initialStep");
			this.controlsMap.InitAndSetArrayItem(true, "multistep");
			this.xt.assign(new XVar("nextStepButton"), new XVar(true));

			return null;
		}
		protected virtual XVar preparePdfControls()
		{
			this.controlsMap.InitAndSetArrayItem(XVar.Array(), "printPdf");
			this.controlsMap.InitAndSetArrayItem(this.pageType, "printPdf", "pageType");
			this.xt.assign(new XVar("pdflink_block"), new XVar(true));

			return null;
		}
		public virtual XVar formatReportFieldValue(dynamic _param_field, dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			if((XVar)(this.format == "excel")  || (XVar)(this.format == "word"))
			{
				return this.getExportValue((XVar)(field), (XVar)(data), (XVar)(keylink), new XVar(true));
			}
			return this.showDBValue((XVar)(field), (XVar)(data), (XVar)(keylink));
		}
		public virtual XVar getSearchObject()
		{
			dynamic sessionPrefix = null;
			sessionPrefix = XVar.Clone(this.sessionPrefix);
			if((XVar)(this.dashTName)  && (XVar)(this.pSet.getEntityType() != Constants.titDASHBOARD))
			{
				sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.dashTName, "_", this.tName));
			}
			return SearchClause.getSearchObject((XVar)(this.tName), (XVar)(this.dashTName), (XVar)(sessionPrefix), (XVar)(this.cipherer), (XVar)(this.searchSavingEnabled), (XVar)(this.pSet), new XVar(true));
		}
		protected virtual XVar assignMenus()
		{
			foreach (KeyValuePair<XVar, dynamic> menu in this.pSet.getPageMenus().GetEnumerator())
			{
				dynamic menuId = null;
				menuId = XVar.Clone(menu.Value["id"]);
				if(XVar.Pack(this.isAdminTable()))
				{
					menuId = new XVar("adminarea");
				}
				this.assignMenu((XVar)(menuId), (XVar)(menu.Value["horizontal"]));
			}

			return null;
		}
		protected virtual XVar assignMenu(dynamic _param_menuName, dynamic _param_horizontal)
		{
			#region pass-by-value parameters
			dynamic menuName = XVar.Clone(_param_menuName);
			dynamic horizontal = XVar.Clone(_param_horizontal);
			#endregion

			dynamic activeId = null, activeItem = null, availableMenus = null, menuData = null, menuObject = null, menuRoot = null;
			availableMenus = XVar.Clone(ProjectSettings.getProjectValue(new XVar("menuIds")));
			if(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(menuName), (XVar)(availableMenus))), XVar.Pack(false)))
			{
				menuName = new XVar("main");
			}
			if(XVar.Pack(this.isAdminTable()))
			{
				menuName = new XVar("adminarea");
			}
			menuObject = XVar.Clone(RunnerMenu.getMenuObject((XVar)(menuName)));
			menuRoot = XVar.Clone(menuObject.getRoot());
			MenuItem.setMenuSession();
			activeItem = XVar.Clone(menuObject.findActiveItem((XVar)(XSession.Session["menuItemId"]), (XVar)(this.tName), (XVar)(this.getPageType())));
			activeId = XVar.Clone((XVar.Pack(activeItem) ? XVar.Pack(activeItem.id) : XVar.Pack(null)));
			menuData = XVar.Clone(menuRoot.getMenuXtData((XVar)(activeId), (XVar)((XVar.Pack(horizontal) ? XVar.Pack(Constants.MENU_HORIZONTAL) : XVar.Pack(Constants.MENU_VERTICAL)))));
			this.xt.assign((XVar)(MVCFunctions.Concat("menuitems_", menuName)), (XVar)(new XVar("data", new XVar(0, menuData))));

			return null;
		}
		protected virtual XVar getMenuItemDetailPeersData(dynamic _param_mTName, dynamic _param_mType, dynamic _param_tName, dynamic _param_pType)
		{
			#region pass-by-value parameters
			dynamic mTName = XVar.Clone(_param_mTName);
			dynamic mType = XVar.Clone(_param_mType);
			dynamic tName = XVar.Clone(_param_tName);
			dynamic pType = XVar.Clone(_param_pType);
			#endregion

			dynamic mPSet = null, peers = XVar.Array();
			mPSet = XVar.Clone(new ProjectSettings((XVar)(mTName), (XVar)(mType)));
			peers = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> dt in mPSet.getAvailableDetailsTables().GetEnumerator())
			{
				dynamic caption = null, detailsData = XVar.Array(), detailsKeys = XVar.Array(), href = null, keyLabelTemplates = XVar.Array(), labelTemplate = null, mKeys = XVar.Array(), masterRecordData = XVar.Array();
				if(dt.Value == tName)
				{
					continue;
				}
				mKeys = XVar.Clone(XVar.Array());
				masterRecordData = XVar.Clone(XSession.Session[MVCFunctions.Concat(mTName, "_masterRecordData")]);
				detailsData = XVar.Clone(XVar.Array());
				if(XVar.Pack(!(XVar)(masterRecordData)))
				{
					masterRecordData = XVar.Clone(this.getMasterRecord());
				}
				keyLabelTemplates = XVar.Clone(XVar.Array());
				detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(dt.Value)));
				foreach (KeyValuePair<XVar, dynamic> dk in detailsKeys["masterKeys"].GetEnumerator())
				{
					mKeys.InitAndSetArrayItem(MVCFunctions.Concat("masterkey", dk.Key + 1, "=", MVCFunctions.RawUrlEncode((XVar)(masterRecordData[dk.Value]))), null);
					keyLabelTemplates.InitAndSetArrayItem(MVCFunctions.Concat("{%", MVCFunctions.GoodFieldName((XVar)(detailsKeys["detailsKeys"][dk.Key])), "}"), null);
					detailsData.InitAndSetArrayItem(masterRecordData[dk.Value], detailsKeys["detailsKeys"][dk.Key]);
				}
				href = XVar.Clone(MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(dt.Value))), (XVar)(ProjectSettings.defaultPageType((XVar)(CommonFunctions.GetEntityType((XVar)(dt.Value)))))));
				href = MVCFunctions.Concat(href, "?", MVCFunctions.implode(new XVar("&"), (XVar)(mKeys)), "&mastertable=", MVCFunctions.RawUrlEncode((XVar)(mTName)));
				labelTemplate = XVar.Clone(Labels.getBreadcrumbsLabelTempl((XVar)(dt.Value), (XVar)(mTName)));
				if(labelTemplate == XVar.Pack(""))
				{
					labelTemplate = XVar.Clone(MVCFunctions.Concat(Labels.getTableCaption((XVar)(dt.Value)), " [", MVCFunctions.implode(new XVar(", "), (XVar)(keyLabelTemplates)), "]"));
				}
				caption = XVar.Clone(this.calcPageTitle((XVar)(labelTemplate), (XVar)(detailsData), (XVar)(mTName), (XVar)(masterRecordData), (XVar)(new ProjectSettings((XVar)(dt.Value)))));
				peers.InitAndSetArrayItem(new XVar("title", caption, "href", href), null);
			}
			return peers;
		}
		protected virtual XVar getMasterDetailMenuItems(dynamic menuRoot, ref dynamic currentMenuItem)
		{
			dynamic caption = null, detailsData = XVar.Array(), href = null, itemData = XVar.Array(), items = XVar.Array(), keyLabelTemplates = XVar.Array(), labelTemplate = null, mTName = null, masterDefaultPageType = null, masterRecordData = XVar.Array(), masterShortTable = null, masterTableData = XVar.Array(), masterTableType = null, menuObject = null, pType = null, sessionPrefix = null, tName = null;
			ProjectSettings pSet;
			items = XVar.Clone(XVar.Array());
			pSet = XVar.UnPackProjectSettings(this.pSet);
			tName = XVar.Clone(this.tName);
			caption = XVar.Clone(Labels.getTableCaption((XVar)(tName)));
			pType = XVar.Clone(this.pageType);
			sessionPrefix = XVar.Clone(this.sessionPrefix);
			while(XVar.Pack(XSession.Session.KeyExists(MVCFunctions.Concat(sessionPrefix, "_mastertable"))))
			{
				mTName = XVar.Clone(XSession.Session[MVCFunctions.Concat(sessionPrefix, "_mastertable")]);
				if(XVar.Pack(!(XVar)(pSet.verifyMasterTable((XVar)(mTName)))))
				{
					break;
				}
				masterTableData = XVar.Clone(pSet.getMasterKeys((XVar)(mTName)));
				masterShortTable = XVar.Clone(CommonFunctions.GetTableURL((XVar)(mTName)));
				masterTableType = XVar.Clone(CommonFunctions.GetEntityType((XVar)(mTName)));
				masterDefaultPageType = XVar.Clone(ProjectSettings.defaultPageType((XVar)(masterTableType)));
				masterRecordData = XVar.Clone(XSession.Session[MVCFunctions.Concat(mTName, "_masterRecordData")]);
				detailsData = XVar.Clone(XVar.Array());
				keyLabelTemplates = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> dk in masterTableData["masterKeys"].GetEnumerator())
				{
					keyLabelTemplates.InitAndSetArrayItem(MVCFunctions.Concat("{%", MVCFunctions.GoodFieldName((XVar)(masterTableData["detailsKeys"][dk.Key])), "}"), null);
					detailsData.InitAndSetArrayItem(masterRecordData[dk.Value], masterTableData["detailsKeys"][dk.Key]);
				}
				if(XVar.Pack(currentMenuItem))
				{
					itemData = XVar.Clone(new XVar("isMenuItem", true, "menuItem", currentMenuItem, "title", currentMenuItem.title));
				}
				else
				{
					caption = XVar.Clone(Labels.getTableCaption((XVar)(tName)));
					href = XVar.Clone(MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(tName))), (XVar)(pType)));
					itemData = XVar.Clone(new XVar("isMenuItem", false, "menuItem", new XVar("href", href), "title", caption));
				}
				if(XVar.Pack(!(XVar)(items)))
				{
					dynamic otherDetailsData = null;
					otherDetailsData = XVar.Clone(this.getMenuItemDetailPeersData((XVar)(mTName), (XVar)(masterDefaultPageType), (XVar)(tName), (XVar)(pType)));
					if(0 < MVCFunctions.count(otherDetailsData))
					{
						itemData.InitAndSetArrayItem(otherDetailsData, "detailPeers");
					}
				}
				labelTemplate = XVar.Clone(Labels.getBreadcrumbsLabelTempl((XVar)(tName), (XVar)(mTName)));
				if(labelTemplate == XVar.Pack(""))
				{
					labelTemplate = XVar.Clone(MVCFunctions.Concat(itemData["title"], " [", MVCFunctions.implode(new XVar(", "), (XVar)(keyLabelTemplates)), "]"));
				}
				itemData.InitAndSetArrayItem(this.calcPageTitle((XVar)(labelTemplate), (XVar)(detailsData), (XVar)(mTName), (XVar)(masterRecordData), (XVar)(pSet)), "title");
				items.InitAndSetArrayItem(itemData, null);
				menuObject = XVar.Clone(RunnerMenu.getMenuObject(new XVar("main")));
				if(XVar.Pack(menuObject))
				{
					currentMenuItem = XVar.Clone(menuObject.findActiveItem(new XVar(null), (XVar)(mTName), (XVar)(masterDefaultPageType)));
				}
				tName = XVar.Clone(mTName);
				pType = XVar.Clone(masterDefaultPageType);
				sessionPrefix = XVar.Clone(mTName);
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(mTName), (XVar)(masterDefaultPageType)));
			}
			if(XVar.Pack(MVCFunctions.count(items)))
			{
				dynamic crumb = XVar.Array();
				crumb = XVar.Clone(XVar.Array());
				if(XVar.Pack(currentMenuItem))
				{
					crumb = XVar.Clone(new XVar("isMenuItem", true, "menuItem", currentMenuItem, "title", currentMenuItem.title));
				}
				else
				{
					href = XVar.Clone(MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(tName))), (XVar)(pType)));
					crumb = XVar.Clone(new XVar("isMenuItem", false, "menuItem", new XVar("href", href), "title", caption));
				}
				labelTemplate = XVar.Clone(Labels.getBreadcrumbsLabelTempl((XVar)(tName)));
				if(labelTemplate != XVar.Pack(""))
				{
					crumb.InitAndSetArrayItem(this.calcPageTitle((XVar)(labelTemplate)), "title");
				}
				items.InitAndSetArrayItem(crumb, null);
			}
			return items;
		}
		public virtual XVar getBreadcrumbMenuId()
		{
			return "main";
		}
		protected virtual XVar checkShowBreadcrumbs()
		{
			return true;
		}
		protected virtual XVar prepareBreadcrumbs()
		{
			dynamic attrs = XVar.Array(), breadChain = XVar.Array(), breadcrumbs = XVar.Array(), crumb = XVar.Array(), currentMenuItem = null, detailItem = null, detailPeers = null, dropItems = XVar.Array(), firstShowPeersIndex = null, href = null, i = null, item = null, itemData = XVar.Array(), menuId = null, menuObject = null, menuRoot = null, peers = XVar.Array(), title = null;
			if(XVar.Pack(!(XVar)(this.checkShowBreadcrumbs())))
			{
				return null;
			}
			menuId = XVar.Clone(this.getBreadcrumbMenuId());
			if(XVar.Pack(!(XVar)(this.pSet.hasBreadcrumb())))
			{
				return null;
			}
			detailItem = XVar.Clone(0 < MVCFunctions.strlen((XVar)(this.masterTable)));
			menuObject = XVar.Clone(RunnerMenu.getMenuObject((XVar)(menuId)));
			menuRoot = XVar.Clone(menuObject.getRoot());
			MenuItem.setMenuSession();
			if(XVar.Pack(!(XVar)(menuObject)))
			{
				return null;
			}
			currentMenuItem = XVar.Clone(menuObject.findActiveItem((XVar)(XSession.Session["menuItemId"]), (XVar)(this.tName), (XVar)(this.getPageType())));
			if((XVar)(!(XVar)(currentMenuItem))  && (XVar)(!(XVar)(detailItem)))
			{
				return null;
			}
			if((XVar)(currentMenuItem)  && (XVar)(!(XVar)(detailItem)))
			{
				if(XVar.Pack(!(XVar)(currentMenuItem.parentItem)))
				{
					return null;
				}
			}
			this.xt.assign(new XVar("breadcrumbs"), new XVar(true));
			this.xt.assign(new XVar("breadcrumb"), new XVar(true));
			this.xt.assign(new XVar("crumb_home_link"), (XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.GetTableLink(new XVar("menu"))))));
			breadChain = XVar.Clone(this.getMasterDetailMenuItems((XVar)(menuRoot), ref currentMenuItem));
			firstShowPeersIndex = XVar.Clone(MVCFunctions.count(breadChain));
			if((XVar)(XVar.Pack(0) < firstShowPeersIndex)  && (XVar)(breadChain[firstShowPeersIndex - 1]["isMenuItem"]))
			{
				currentMenuItem = XVar.Clone(breadChain[firstShowPeersIndex - 1]["menuItem"].parentItem);
			}
			if(XVar.Pack(currentMenuItem))
			{
				dynamic labelTemplate = null;
				while(XVar.Pack(currentMenuItem.parentItem))
				{
					crumb = XVar.Clone(new XVar("isMenuItem", true, "menuItem", currentMenuItem));
					labelTemplate = XVar.Clone(Labels.getBreadcrumbsLabelTempl((XVar)(currentMenuItem.getTable()), new XVar(""), (XVar)(currentMenuItem.getPageType())));
					if(labelTemplate != XVar.Pack(""))
					{
						crumb.InitAndSetArrayItem(this.calcPageTitle((XVar)(labelTemplate)), "title");
					}
					else
					{
						crumb.InitAndSetArrayItem(currentMenuItem.title, "title");
					}
					breadChain.InitAndSetArrayItem(crumb, null);
					currentMenuItem = XVar.Clone(currentMenuItem.parentItem);
				}
			}
			breadcrumbs = XVar.Clone(XVar.Array());
			i = XVar.Clone(MVCFunctions.count(breadChain) - 1);
			for(;XVar.Pack(0) <= i; --(i))
			{
				itemData = XVar.Clone(breadChain[i]);
				crumb = XVar.Clone(XVar.Array());
				item = new XVar(null);
				attrs = XVar.Clone(XVar.Array());
				if(XVar.Pack(itemData["isMenuItem"]))
				{
					item = XVar.Clone(itemData["menuItem"]);
					attrs = XVar.Clone(item.getMenuItemAttributes(new XVar(Constants.MENU_HORIZONTAL)));
					href = XVar.Clone(attrs["href"]);
				}
				else
				{
					href = XVar.Clone(itemData["menuItem"]["href"]);
				}
				title = XVar.Clone(itemData["title"]);
				if((XVar)(i < firstShowPeersIndex)  && (XVar)(i))
				{
					crumb.InitAndSetArrayItem(MVCFunctions.Concat("href=\"", href, (XVar.Pack(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(href), new XVar("?"))), XVar.Pack(false))) ? XVar.Pack("&a=return") : XVar.Pack("?a=return")), "\""), "crumb_attrs");
				}
				else
				{
					crumb.InitAndSetArrayItem(MVCFunctions.Concat("href=\"", href, "\""), "crumb_attrs");
				}
				if((XVar)(XVar.Pack(0) < firstShowPeersIndex)  && (XVar)(i == XVar.Pack(0)))
				{
					crumb.InitAndSetArrayItem(true, "crumb_title_span");
				}
				else
				{
					crumb.InitAndSetArrayItem(true, "crumb_title_link");
				}
				crumb.InitAndSetArrayItem(title, "crumb_title");
				if((XVar)((XVar)(i < firstShowPeersIndex)  && (XVar)((XVar)(!(XVar)(itemData["detailPeers"]))  || (XVar)(!(XVar)(itemData["isMenuItem"]))))  || (XVar)(this.isAdminTable()))
				{
					breadcrumbs.InitAndSetArrayItem(crumb, null);
					continue;
				}
				if(XVar.Pack(XVar.Equals(XVar.Pack(item), XVar.Pack(null))))
				{
					continue;
				}
				dropItems = XVar.Clone(XVar.Array());
				peers = XVar.Clone(XVar.Array());
				detailPeers = XVar.Clone(!(XVar)(!(XVar)(itemData["detailPeers"])));
				if(XVar.Pack(detailPeers))
				{
					peers = XVar.Clone(itemData["detailPeers"]);
				}
				else
				{
					item.parentItem.getItemDescendants((XVar)(peers));
				}
				if((XVar)(1 < MVCFunctions.count(peers))  || (XVar)(detailPeers))
				{
					foreach (KeyValuePair<XVar, dynamic> p in peers.GetEnumerator())
					{
						if(XVar.Pack(detailPeers))
						{
							dropItems.InitAndSetArrayItem(MVCFunctions.Concat("<li><a href=\"", p.Value["href"], "\">", p.Value["title"], "</a></li>"), null);
							continue;
						}
						if(p.Value.id == item.id)
						{
							continue;
						}
						attrs = XVar.Clone(XVar.Array());
						if((XVar)(!(XVar)(p.Value.showAsLink()))  && (XVar)(p.Value.showAsGroup()))
						{
							dynamic childWithLink = null;
							childWithLink = XVar.Clone(p.Value.getFirstChildWithLink());
							if(XVar.Pack(childWithLink))
							{
								attrs = XVar.Clone(childWithLink.getMenuItemAttributes(new XVar(Constants.MENU_HORIZONTAL)));
							}
						}
						else
						{
							if(XVar.Pack(p.Value.showAsLink()))
							{
								attrs = XVar.Clone(p.Value.getMenuItemAttributes(new XVar(Constants.MENU_HORIZONTAL)));
							}
						}
						if(XVar.Pack(MVCFunctions.count(attrs)))
						{
							dropItems.InitAndSetArrayItem(MVCFunctions.Concat("<li><a href=\"", attrs["href"], "\">", p.Value.title, "</a></li>"), null);
						}
					}
					if(0 < MVCFunctions.count(dropItems))
					{
						crumb["crumb_title"] = MVCFunctions.Concat(crumb["crumb_title"], "<span class=\"caret\"></span>");
						crumb["crumb_attrs"] = MVCFunctions.Concat(crumb["crumb_attrs"], " class=\"dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"");
						crumb.InitAndSetArrayItem("dropdown", "crumb_item_class");
						crumb.InitAndSetArrayItem(MVCFunctions.Concat("<ul class=\"dropdown-menu\">", MVCFunctions.implode(new XVar(""), (XVar)(dropItems)), "</ul>"), "crumb_dropdown");
					}
				}
				breadcrumbs.InitAndSetArrayItem(crumb, null);
			}
			this.xt.assign_loopsection(new XVar("crumb"), (XVar)(breadcrumbs));

			return null;
		}
		public virtual XVar fillControlFlags(dynamic _param_field, dynamic _param_required = null)
		{
			#region default values
			if(_param_required as Object == null) _param_required = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic required = XVar.Clone(_param_required);
			#endregion

			this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(field)), "_label")), new XVar(true));
			if((XVar)(this.pSet.isRequired((XVar)(field)))  || (XVar)(required))
			{
				this.xt.assign((XVar)(MVCFunctions.Concat("required_attr_", MVCFunctions.GoodFieldName((XVar)(field)))), new XVar("data-required=\"true\""));
			}

			return null;
		}
		protected virtual XVar setDetailsBadgeStyles()
		{
			if(XVar.Pack(!(XVar)(this.detailsInGridAvailable())))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				dynamic color = null, dSet = null;
				if(XVar.Pack(!(XVar)(this.pSet.detailsShowCount((XVar)(details.Value)))))
				{
					continue;
				}
				dSet = XVar.Clone(new ProjectSettings((XVar)(details.Value)));
				color = XVar.Clone(this.pSet.getDetailsBadgeColor((XVar)(details.Value)));
				if(XVar.Pack(!(XVar)(color)))
				{
					color = XVar.Clone(MVCFunctions.Concat("#", dSet.getDefaultBadgeColor()));
				}
				this.row_css_rules = XVar.Clone(MVCFunctions.Concat(".badge.badge.", dSet.getShortTableName(), "_badge { background-color:", color, " }\n", this.row_css_rules));
			}

			return null;
		}
		public virtual XVar detailsInGridAvailable()
		{
			dynamic detailsTables = XVar.Array();
			detailsTables = XVar.Clone(this.pSet.getDetailsTables());
			foreach (KeyValuePair<XVar, dynamic> details in detailsTables.GetEnumerator())
			{
				dynamic dPset = null, detAddAvailabel = null, detEditAvailabel = null, detListAvailabel = null, detTablePermis = XVar.Array(), detTableType = null;
				dPset = XVar.Clone(new ProjectSettings((XVar)(details.Value)));
				detTablePermis = XVar.Clone(this.permis[details.Value]);
				detTableType = XVar.Clone(dPset.getEntityType());
				detListAvailabel = XVar.Clone((XVar)((XVar)((XVar)(dPset.hasListPage())  || (XVar)(CommonFunctions.isReport((XVar)(detTableType))))  || (XVar)(CommonFunctions.isChart((XVar)(detTableType))))  && (XVar)(detTablePermis["search"]));
				detAddAvailabel = XVar.Clone((XVar)(dPset.hasAddPage())  && (XVar)(detTablePermis["add"]));
				detEditAvailabel = XVar.Clone((XVar)(dPset.hasEditPage())  && (XVar)(detTablePermis["edit"]));
				if((XVar)(detListAvailabel)  || (XVar)((XVar)((XVar)(this.pSet.detailsLinks() == Constants.DL_INDIVIDUAL)  || (XVar)(MVCFunctions.count(detailsTables) == 1))  && (XVar)((XVar)(detAddAvailabel)  || (XVar)(detEditAvailabel))))
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar getColumnsToHide()
		{
			return this.getHiddenColumnsByDevice();
		}
		protected static XVar deviceClassToMacro(dynamic _param_deviceClass)
		{
			#region pass-by-value parameters
			dynamic deviceClass = XVar.Clone(_param_deviceClass);
			#endregion

			if((XVar)(deviceClass == Constants.TABLET_10_IN)  || (XVar)(deviceClass == Constants.TABLET_7_IN))
			{
				return 1;
			}
			if((XVar)(deviceClass == Constants.SMARTPHONE_LANDSCAPE)  || (XVar)(deviceClass == Constants.SMARTPHONE_PORTRAIT))
			{
				return 2;
			}
			return 0;
		}
		protected virtual XVar getSpecificPageSettings(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pSet);
			if(!XVar.Equals(XVar.Pack(this.getPageType()), XVar.Pack(pageType)))
			{
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(pageType)));
			}
			return pSet;
		}
		protected virtual XVar showHideFieldsFeatureEnabled()
		{
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.getSpecificPageSettings(new XVar(Constants.PAGE_LIST)));
			return pSet.isAllowShowHideFields();
		}
		protected virtual XVar reorderFieldsFeatureEnabled()
		{
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.getSpecificPageSettings(new XVar(Constants.PAGE_LIST)));
			return pSet.isAllowFieldsReordering();
		}
		protected virtual XVar getCombinedHiddenColumns()
		{
			dynamic devices = XVar.Array(), logger = null, oldLogger = null, ret = XVar.Array(), savedColumnStates = XVar.Array(), savedHiddenColumns = XVar.Array();
			if(XVar.Pack(!(XVar)(this.showHideFieldsFeatureEnabled())))
			{
				return this.getHiddenColumnsByDevice();
			}
			logger = XVar.Clone(new paramsLogger((XVar)(this.tName), new XVar(Constants.NEWSHFIELDS_PARAMS_TYPE)));
			savedColumnStates = XVar.Clone(logger.getShowHideData());
			oldLogger = XVar.Clone(new paramsLogger((XVar)(this.tName), new XVar(Constants.SHFIELDS_PARAMS_TYPE)));
			savedHiddenColumns = XVar.Clone(oldLogger.getShowHideData());
			if(XVar.Pack(!(XVar)(this.pSet.columnsByDeviceEnabled())))
			{
				savedColumnStates.InitAndSetArrayItem(savedColumnStates[Constants.dmcDESKTOP], Constants.dmcSMARTPHONE);
				savedHiddenColumns.InitAndSetArrayItem(savedHiddenColumns[Constants.dmcDESKTOP], Constants.dmcSMARTPHONE);
			}
			ret = XVar.Clone(XVar.Array());
			devices = XVar.Clone(new XVar(0, Constants.DESKTOP, 1, Constants.SMARTPHONE_PORTRAIT));
			foreach (KeyValuePair<XVar, dynamic> d in devices.GetEnumerator())
			{
				dynamic dmc = null, hideFields = XVar.Array(), initialFields = null;
				dmc = XVar.Clone(RunnerPage.deviceClassToMacro((XVar)(d.Value)));
				hideFields = XVar.Clone(XVar.Array());
				initialFields = XVar.Clone(this.pSet.getHiddenGoodNameFields((XVar)(d.Value)));
				if(XVar.Pack(savedColumnStates[dmc]))
				{
					foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getPageFields().GetEnumerator())
					{
						dynamic gf = null;
						gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(f.Value)));
						if(XVar.Pack(savedColumnStates[dmc].KeyExists(gf)))
						{
							if(XVar.Equals(XVar.Pack(savedColumnStates[dmc][gf]), XVar.Pack(false)))
							{
								hideFields.InitAndSetArrayItem(gf, null);
							}
						}
						else
						{
							if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(gf), (XVar)(initialFields))), XVar.Pack(false)))
							{
								hideFields.InitAndSetArrayItem(gf, null);
							}
						}
					}
				}
				else
				{
					if(XVar.Pack(savedHiddenColumns[dmc]))
					{
						hideFields = XVar.Clone(savedHiddenColumns[dmc]);
					}
					else
					{
						hideFields = XVar.Clone(initialFields);
					}
				}
				ret.InitAndSetArrayItem(hideFields, d.Value);
			}
			return ret;
		}
		protected virtual XVar getHiddenColumnsByDevice()
		{
			dynamic columnsToHide = XVar.Array(), devices = XVar.Array();
			columnsToHide = XVar.Clone(XVar.Array());
			devices = XVar.Clone(new XVar(0, Constants.TABLET_7_IN, 1, Constants.SMARTPHONE_PORTRAIT, 2, Constants.SMARTPHONE_LANDSCAPE, 3, Constants.TABLET_10_IN, 4, Constants.DESKTOP));
			foreach (KeyValuePair<XVar, dynamic> d in devices.GetEnumerator())
			{
				columnsToHide.InitAndSetArrayItem(this.pSet.getHiddenGoodNameFields((XVar)(d.Value)), d.Value);
			}
			return columnsToHide;
		}
		public static XVar sendEmailByTemplate(dynamic _param_toEmail, dynamic _param_template, dynamic _param_data, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic toEmail = XVar.Clone(_param_toEmail);
			dynamic template = XVar.Clone(_param_template);
			dynamic data = XVar.Clone(_param_data);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic templatefile = null;
			templatefile = XVar.Clone(MVCFunctions.Concat("email/", CommonFunctions.mlang_getcurrentlang(), "/", template, ".txt"));
			return CommonFunctions.sendEmailTemplate((XVar)(toEmail), (XVar)(templatefile), (XVar)(data), (XVar)(html));
		}
		protected virtual XVar getSelectedRecords()
		{
			dynamic keyFields = XVar.Array(), keys = XVar.Array(), selected_recs = XVar.Array();
			selected_recs = XVar.Clone(XVar.Array());
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			if(XVar.Pack(MVCFunctions.postvalue("mdelete")))
			{
				foreach (KeyValuePair<XVar, dynamic> ind in MVCFunctions.postvalue("mdelete").GetEnumerator())
				{
					keys = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> f in keyFields.GetEnumerator())
					{
						keys.InitAndSetArrayItem(MVCFunctions.postvalue("mdelete" + (f.Key + 1))[MVCFunctions.mdeleteIndex((XVar)(ind.Value))], f.Value);
					}
					selected_recs.InitAndSetArrayItem(keys, null);
				}
			}
			else
			{
				if(XVar.Pack(this.selection))
				{
					foreach (KeyValuePair<XVar, dynamic> keyblock in this.selection.GetEnumerator())
					{
						dynamic arr = XVar.Array();
						arr = XVar.Clone(MVCFunctions.explode(new XVar("&"), (XVar)(keyblock.Value)));
						if(MVCFunctions.count(arr) < MVCFunctions.count(keyFields))
						{
							continue;
						}
						keys = XVar.Clone(XVar.Array());
						foreach (KeyValuePair<XVar, dynamic> f in keyFields.GetEnumerator())
						{
							keys.InitAndSetArrayItem(MVCFunctions.urldecode((XVar)(arr[f.Key])), f.Value);
						}
						selected_recs.InitAndSetArrayItem(keys, null);
					}
				}
				else
				{
					return null;
				}
			}
			return selected_recs;
		}
		protected virtual XVar getSelection()
		{
			return this.selection;
		}
		public virtual XVar getTabDataCommand(dynamic _param_tab)
		{
			#region pass-by-value parameters
			dynamic tab = XVar.Clone(_param_tab);
			#endregion

			dynamic dc = null;
			this.tabChangeling = XVar.Clone(tab);
			dc = XVar.Clone(this.getSubsetDataCommand());
			this.tabChangeling = new XVar(null);
			return dc;
		}
		public virtual XVar simpleMode()
		{
			return this.id == 1;
		}
		public virtual XVar displayTabsInPage()
		{
			return this.simpleMode();
		}
		public virtual XVar updateDetailsTabTitles()
		{
			if(XVar.Pack(!(XVar)(this.changeDetailsTabTitles)))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> i in MVCFunctions.array_keys((XVar)(this.gridTabs)).GetEnumerator())
			{
				dynamic id = null;
				id = XVar.Clone(this.gridTabs[i.Value]["id"]);
				this.setTabTitle((XVar)(id), (XVar)(MVCFunctions.Concat(Labels.getTableCaption((XVar)(this.tName)), " - ", this.getTabTitle((XVar)(id)))));
			}

			return null;
		}
		public virtual XVar shouldDisplayDetailsPage()
		{
			return true;
		}
		public virtual XVar hideItemType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			if(var_type == "grid_message")
			{
				this.hideGridMessage();
			}
			else
			{
				if(var_type == "grid")
				{
					this.hideForm(new XVar("grid"));
				}
				else
				{
					dynamic itemsByType = XVar.Array();
					itemsByType = this.pSet.helperItemsByType();
					this.xt.displayItemsHidden((XVar)(itemsByType[var_type]));
				}
			}

			return null;
		}
		public virtual XVar hideItem(dynamic _param_itemId, dynamic _param_recordId = null)
		{
			#region default values
			if(_param_recordId as Object == null) _param_recordId = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic itemId = XVar.Clone(_param_itemId);
			dynamic recordId = XVar.Clone(_param_recordId);
			#endregion

			this.xt.displayItemHidden((XVar)(itemId), (XVar)(recordId));

			return null;
		}
		public virtual XVar showItem(dynamic _param_itemId, dynamic _param_recordId = null)
		{
			#region default values
			if(_param_recordId as Object == null) _param_recordId = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic itemId = XVar.Clone(_param_itemId);
			dynamic recordId = XVar.Clone(_param_recordId);
			#endregion

			this.xt.showHiddenItem((XVar)(itemId), (XVar)(recordId));

			return null;
		}
		public virtual XVar hideAllFormItems(dynamic _param_location)
		{
			#region pass-by-value parameters
			dynamic location = XVar.Clone(_param_location);
			#endregion

			dynamic helper = XVar.Array();
			helper = XVar.Clone(this.pSet.helperFormItems());
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(helper["formItems"][location])))))
			{
				return null;
			}
			this.xt.displayItemsHidden((XVar)(helper["formItems"][location]));

			return null;
		}
		public virtual XVar showItemType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			dynamic itemsByType = XVar.Array();
			itemsByType = this.pSet.helperItemsByType();
			this.xt.showHiddenItems((XVar)(itemsByType[var_type]));

			return null;
		}
		public virtual XVar hideForm(dynamic _param_form)
		{
			#region pass-by-value parameters
			dynamic form = XVar.Clone(_param_form);
			#endregion

			this.xt.assign((XVar)(MVCFunctions.Concat("form_", form)), (XVar)(MVCFunctions.Concat(this.xt.getVar((XVar)(MVCFunctions.Concat("form_", form))), " data-hidden")));

			return null;
		}
		public virtual XVar setPageData(dynamic _param_key, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			this.pageData.InitAndSetArrayItem(value, key);

			return null;
		}
		public virtual XVar getMasterPSet()
		{
			if((XVar)(!(XVar)(this.masterPSet))  && (XVar)(this.masterTable))
			{
				this.masterPSet = XVar.Clone(new ProjectSettings((XVar)(this.masterTable), (XVar)(this.masterPageType), (XVar)(this.masterPage)));
			}
			return this.masterPSet;
		}
		public virtual XVar fetchForms(dynamic _param_forms)
		{
			#region pass-by-value parameters
			dynamic forms = XVar.Clone(_param_forms);
			#endregion

			dynamic formBlocks = XVar.Array();
			formBlocks = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in forms.GetEnumerator())
			{
				formBlocks.InitAndSetArrayItem(MVCFunctions.Concat(f.Value, "_block"), null);
			}
			return this.fetchBlocksList((XVar)(formBlocks));
		}
		public virtual XVar hideElement(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			dynamic itemTypes = XVar.Array();
			itemTypes = XVar.Clone(this.element2Item((XVar)(name)));
			foreach (KeyValuePair<XVar, dynamic> it in itemTypes.GetEnumerator())
			{
				this.hideItemType((XVar)(it.Value));
			}

			return null;
		}
		public virtual XVar element2Brick(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return name;
		}
		public virtual XVar element2Item(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(name == "recordcontrol")
			{
				return new XVar(0, "inline_save_all", 1, "inline_cancel_all", 2, "delete", 3, "update_selected", 4, "export_selected", 5, "delete_selected");
			}
			if(name == "reorder_records")
			{
				return new XVar(0, "sort_dropdown");
			}
			if(name == "printpanel")
			{
				return new XVar(0, "print_panel");
			}
			if(name == "message")
			{
				return new XVar(0, "message");
			}
			if(name == "searchpanel")
			{
				return new XVar(0, "search_panel", 1, "hide_search_panel");
			}
			if(name == "pagination")
			{
				return new XVar(0, "pagination");
			}
			if(name == "filterpanel")
			{
				return new XVar(0, "filter_panel");
			}
			return name;
		}
		public virtual XVar hideGridMessage()
		{
			this.xt.assign(new XVar("grid_message_attrs"), new XVar("data-hidden"));

			return null;
		}
		public virtual XVar prepareDisplayDetails()
		{

			return null;
		}
		public virtual XVar showButtonsDp()
		{

			return null;
		}
		public virtual XVar prepareCharts()
		{
			dynamic chartXtParams = null;
			chartXtParams = XVar.Clone(new XVar("id", this.id));
			this.xt.assign_function(new XVar("chart"), new XVar("xt_showpdchart"), (XVar)(chartXtParams));

			return null;
		}
		public virtual XVar hideRecordItem(dynamic _param_itemId, dynamic _param_recordid)
		{
			#region pass-by-value parameters
			dynamic itemId = XVar.Clone(_param_itemId);
			dynamic recordid = XVar.Clone(_param_recordid);
			#endregion

			if(XVar.Pack(!(XVar)(this.pdfJsonMode())))
			{
				this.recordAssign((XVar)(recordid), (XVar)(MVCFunctions.Concat("item_", itemId)), new XVar("data-hidden"));
			}
			else
			{
				this.recordAssign((XVar)(recordid), (XVar)(MVCFunctions.Concat("item_hide_", itemId)), new XVar("1"));
			}

			return null;
		}
		public virtual XVar recordAssign(dynamic _param_recordid, dynamic _param_key, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic recordid = XVar.Clone(_param_recordid);
			dynamic key = XVar.Clone(_param_key);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic data = XVar.Array();
			data = this.findRecordAssigns((XVar)(recordid));
			data.InitAndSetArrayItem(value, key);

			return null;
		}
		public virtual XVar findRecordAssigns(dynamic _param_recordid)
		{
			#region pass-by-value parameters
			dynamic recordid = XVar.Clone(_param_recordid);
			#endregion

			return XVar.Array();
		}
		public virtual XVar pdfJsonMode()
		{
			return false;
		}
		public virtual XVar searchFieldAppearsOnPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Equals(XVar.Pack(this.pageType), XVar.Pack(Constants.PAGE_SEARCH)))
			{
				return this.pSet.appearOnPage((XVar)(field));
			}
			return this.pSet.appearAlwaysOnSearchPanel((XVar)(field));
		}
		public virtual XVar createProjectSettings()
		{
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), (XVar)(this.pageName), (XVar)(this.pageTable)));
			if(!XVar.Equals(XVar.Pack(this.pSet.getPageType()), XVar.Pack(this.pageType)))
			{
				this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), new XVar(null), (XVar)(this.pageTable)));
			}

			return null;
		}
		public virtual XVar formatGroupValue(dynamic _param_fName, dynamic _param_intervalType, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic intervalType = XVar.Clone(_param_intervalType);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return RunnerPage.formatGroupValueStatic((XVar)(fName), (XVar)(intervalType), (XVar)(value), (XVar)(this.pSet), this, (XVar)(this.pdfJsonMode()));
		}
		public static XVar formatGroupValueStatic(dynamic _param_fName, dynamic _param_intervalType, dynamic _param_value, dynamic _param_pSet_packed, dynamic _param_fieldControls, dynamic _param_pdfJsonMode)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic intervalType = XVar.Clone(_param_intervalType);
			dynamic value = XVar.Clone(_param_value);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic fieldControls = XVar.Clone(_param_fieldControls);
			dynamic pdfJsonMode = XVar.Clone(_param_pdfJsonMode);
			#endregion

			dynamic fType = null;
			if(XVar.Pack(!(XVar)(intervalType)))
			{
				dynamic data = null;
				data = XVar.Clone(new XVar(fName, value));
				return fieldControls.showDBValue((XVar)(fName), (XVar)(data));
			}
			fType = XVar.Clone(pSet.getFieldType((XVar)(fName)));
			if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(fType))))
			{
				dynamic dataEnd = null, dataStart = null, start = null, strEnd = null, strStart = null, var_end = null;
				start = XVar.Clone(value - value  %  intervalType);
				if(XVar.Pack(!(XVar)(CommonFunctions.IsFloatType((XVar)(fType)))))
				{
					var_end = XVar.Clone((start + intervalType) - 1);
				}
				else
				{
					var_end = XVar.Clone((start + intervalType) - 0.010000);
				}
				dataStart = XVar.Clone(new XVar(fName, start));
				dataEnd = XVar.Clone(new XVar(fName, var_end));
				strStart = XVar.Clone(fieldControls.showDBValue((XVar)(fName), (XVar)(dataStart)));
				strEnd = XVar.Clone(fieldControls.showDBValue((XVar)(fName), (XVar)(dataEnd)));
				return (XVar.Pack(pdfJsonMode) ? XVar.Pack(MVCFunctions.Concat(MVCFunctions.substr((XVar)(strStart), new XVar(0), (XVar)(MVCFunctions.strlen((XVar)(strStart)) - 1)), " - ", MVCFunctions.substr((XVar)(strEnd), new XVar(1)))) : XVar.Pack(MVCFunctions.Concat(strStart, " - ", strEnd)));
			}
			else
			{
				dynamic result = null;
				if(XVar.Pack(CommonFunctions.IsCharType((XVar)(fType))))
				{
					result = XVar.Clone(CommonFunctions.xmlencode((XVar)(MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(intervalType)))));
					return (XVar.Pack(pdfJsonMode) ? XVar.Pack(MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(result)), "'")) : XVar.Pack(result));
				}
				else
				{
					if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(fType))))
					{
						result = XVar.Clone(CommonFunctions.formatDateIntervalValue((XVar)(value), (XVar)(intervalType)));
						return (XVar.Pack(pdfJsonMode) ? XVar.Pack(MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(result)), "'")) : XVar.Pack(result));
					}
				}
			}
			return value;
		}
		public virtual XVar preparePDFBackground()
		{
			dynamic filename = null, image = null, imgSize = XVar.Array(), url = null;
			if(XVar.Pack(!(XVar)(this.pdfBackgroundImage)))
			{
				return null;
			}
			filename = XVar.Clone(MVCFunctions.Concat("images/", MVCFunctions.str_replace(new XVar(".."), new XVar(""), (XVar)(this.pdfBackgroundImage))));
			image = XVar.Clone(MVCFunctions.myfile_get_contents_binary((XVar)(MVCFunctions.getabspath((XVar)(filename)))));
			imgSize = XVar.Clone(MVCFunctions.getImageDimensions((XVar)(image)));
			if(XVar.Pack(imgSize))
			{
				this.xt.assign(new XVar("bgWidth"), (XVar)(imgSize["width"]));
				this.xt.assign(new XVar("bgHeight"), (XVar)(imgSize["height"]));
			}
			url = XVar.Clone(CommonFunctions.imageDataUrl((XVar)(image)));
			if(XVar.Pack(!(XVar)(url)))
			{
				return null;
			}
			this.xt.assign(new XVar("backgroundImage"), (XVar)(MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(url)), "'")));

			return null;
		}
		public virtual XVar getMasterCondition()
		{
			dynamic conditions = XVar.Array(), detailsKeys = XVar.Array(), detailsKeysSettings = XVar.Array(), i = null;
			detailsKeysSettings = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
			detailsKeys = XVar.Clone(detailsKeysSettings["detailsKeys"]);
			if(XVar.Pack(!(XVar)(detailsKeys)))
			{
				return null;
			}
			conditions = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(detailsKeys); ++(i))
			{
				conditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(detailsKeys[i]), (XVar)(this.masterKeysReq[i + 1])), null);
			}
			return DataCondition._And((XVar)(conditions));
		}
		public virtual XVar getDetailsKeyValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic detailsKeys = XVar.Array(), masterKeysSettings = XVar.Array();
			masterKeysSettings = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
			detailsKeys = XVar.Clone(masterKeysSettings["detailsKeys"]);
			foreach (KeyValuePair<XVar, dynamic> f in detailsKeys.GetEnumerator())
			{
				if(field == f.Value)
				{
					return this.masterKeysReq[f.Key + 1];
				}
			}
			return false;
		}
		public virtual XVar getSecurityCondition()
		{
			return Security.SelectCondition(new XVar("S"), (XVar)(this.pSet));
		}
		public virtual XVar getDataSourceFilterCriteria(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic fieldFilter = null, filter = null, map = null, master = null, search = null, security = null, tab = null;
			security = XVar.Clone(this.getSecurityCondition());
			search = XVar.Clone(this.searchClauseObj.getSearchDataCondition());
			filter = XVar.Clone(this.searchClauseObj.getFilterCondition((XVar)(this.pSet), (XVar)(ignoreFilterField)));
			master = XVar.Clone(this.getMasterCondition());
			tab = XVar.Clone(DataCondition.SQLCondition((XVar)(this.getCurrentTabWhere())));
			map = XVar.Clone((XVar.Pack(this.dependsOnDashMap()) ? XVar.Pack(this.getMapCondition()) : XVar.Pack(null)));
			fieldFilter = XVar.Clone(this.getFieldFilterCondition());
			return DataCondition._And((XVar)(new XVar(0, security, 1, master, 2, filter, 3, search, 4, tab, 5, map, 6, fieldFilter)));
		}
		public virtual XVar getSubsetDataCommand(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic orderObject = null, ret = null;
			ret = XVar.Clone(new DsCommand());
			ret.filter = XVar.Clone(this.getDataSourceFilterCriteria((XVar)(ignoreFilterField)));
			orderObject = XVar.Clone(this.getOrderObject());
			ret.order = XVar.Clone(orderObject.getOrderFields());
			return ret;
		}
		public virtual XVar getMasterTableDataClause()
		{
			dynamic detailsKeys = XVar.Array(), i = null, mKey = null, masterKeysSettings = XVar.Array(), ret = XVar.Array();
			masterKeysSettings = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
			detailsKeys = XVar.Clone(masterKeysSettings["detailsKeys"]);
			if(XVar.Pack(!(XVar)(detailsKeys)))
			{
				return null;
			}
			ret = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(detailsKeys); i++)
			{
				mKey = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_masterkey", i + 1)]);
				ret.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(detailsKeys[i]), (XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_masterkey", i + 1)])), null);
			}
			return DataCondition._And((XVar)(ret));
		}
		public virtual XVar getOrderObject()
		{
			return OrderClause.createFromPage(this, new XVar(false));
		}
		public virtual XVar getEditLink(dynamic data)
		{
			dynamic i = null, keyLinkParts = XVar.Array(), keyValue = null, tKeys = XVar.Array();
			keyLinkParts = XVar.Clone(XVar.Array());
			tKeys = XVar.Clone(this.pSet.getTableKeys());
			i = new XVar(0);
			for(;i < MVCFunctions.count(tKeys); i++)
			{
				keyValue = XVar.Clone(MVCFunctions.RawUrlEncode((XVar)(data[tKeys[i]])));
				keyLinkParts.InitAndSetArrayItem(MVCFunctions.Concat("editid", i + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(keyValue))), null);
			}
			return MVCFunctions.implode(new XVar("&"), (XVar)(keyLinkParts));
		}
		public static XVar readMasterKeysFromRequest()
		{
			dynamic i = null, options = XVar.Array();
			i = new XVar(1);
			options = XVar.Clone(XVar.Array());
			while(XVar.Pack(MVCFunctions.REQUESTKeyExists(MVCFunctions.Concat("masterkey", i))))
			{
				options.InitAndSetArrayItem(MVCFunctions.postvalue(MVCFunctions.Concat("masterkey", i)), i);
				i++;
			}
			return options;
		}
		protected virtual XVar getMasterKeysParams()
		{
			dynamic i = null, var_params = XVar.Array();
			var_params = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(this.masterKeysReq)))
			{
				return var_params;
			}
			i = new XVar(1);
			for(;i <= MVCFunctions.count(this.masterKeysReq); i++)
			{
				var_params.InitAndSetArrayItem(this.masterKeysReq[i], MVCFunctions.Concat("masterkey", i));
			}
			return var_params;
		}
		protected virtual XVar getStateParams()
		{
			dynamic mainPageTypes = null, var_params = XVar.Array();
			var_params = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.masterTable))
			{
				var_params = XVar.Clone(this.getMasterKeysParams());
				var_params.InitAndSetArrayItem(this.masterTable, "mastertable");
			}
			mainPageTypes = XVar.Clone(new XVar(0, "list", 1, "calendar", 2, "gantt", 3, "chart", 4, "report"));
			if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(this.pageType), (XVar)(mainPageTypes))), XVar.Pack(false)))
			{
				dynamic defaultPages = XVar.Array();
				defaultPages = XVar.Clone(this.pSet.getDefaultPages());
				if(this.pageName != defaultPages[this.pSet.getDefaultPageType()])
				{
					var_params.InitAndSetArrayItem(this.pageName, "returnPage");
				}
			}
			return var_params;
		}
		public virtual XVar getStateUrlParams()
		{
			dynamic urlParams = XVar.Array();
			urlParams = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in this.getStateParams().GetEnumerator())
			{
				urlParams.InitAndSetArrayItem(MVCFunctions.Concat(value.Key, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(value.Value))))), null);
			}
			return MVCFunctions.implode(new XVar("&"), (XVar)(urlParams));
		}
		protected virtual XVar getMasterPageName()
		{
			dynamic mpSet = null;
			mpSet = XVar.Clone(new ProjectSettings((XVar)(this.masterTable)));
			return mpSet.pageName();
		}
		protected virtual XVar getRowCountByTab(dynamic _param_tab)
		{
			#region pass-by-value parameters
			dynamic tab = XVar.Clone(_param_tab);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(this.getTabDataCommand((XVar)(tab)));
			this.callBeforeQueryEvent((XVar)(dc));
			return this.dataSource.getCount((XVar)(dc));
		}
		public virtual XVar callBeforeQueryEvent(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion


			return null;
		}
		protected virtual XVar checkOauthLogin()
		{
			dynamic var_request = null;
			if(XVar.Pack(!(XVar)(this.dataSource)))
			{
				return true;
			}
			if(XVar.Pack(this.dataSource.checkAuthorization()))
			{
				return true;
			}
			var_request = XVar.Clone(this.dataSource.getAuthorizationInfo());
			Security.saveRedirectURL();
			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", var_request.getUrl())));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar getRelatedInlineEditPage(dynamic _param_pageName = null, dynamic _param_keys = null, dynamic _param_id = null)
		{
			#region default values
			if(_param_pageName as Object == null) _param_pageName = new XVar("");
			if(_param_keys as Object == null) _param_keys = new XVar();
			if(_param_id as Object == null) _param_id = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic pageName = XVar.Clone(_param_pageName);
			dynamic keys = XVar.Clone(_param_keys);
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic var_params = XVar.Array();
			XTempl xt;
			id = XVar.Clone((XVar.Pack(id) ? XVar.Pack(id) : XVar.Pack(this.genId())));
			xt = XVar.UnPackXTempl(new XTempl());
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(id, "id");
			var_params.InitAndSetArrayItem(xt, "xt");
			var_params.InitAndSetArrayItem(keys, "keys");
			var_params.InitAndSetArrayItem(Constants.EDIT_INLINE, "mode");
			var_params.InitAndSetArrayItem(Constants.PAGE_EDIT, "pageType");
			var_params.InitAndSetArrayItem(pageName, "pageName");
			var_params.InitAndSetArrayItem(this.tName, "tName");
			var_params.InitAndSetArrayItem(this.masterTable, "masterTable");
			if(XVar.Pack(var_params["masterTable"]))
			{
				var_params.InitAndSetArrayItem(this.masterKeysReq, "masterKeysReq");
			}
			return new EditPage((XVar)(var_params));
		}
		public virtual XVar getEditContolParams(dynamic _param_fName, dynamic _param_recId, dynamic data)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			#endregion

			dynamic parameters = XVar.Array();
			parameters = XVar.Clone(XVar.Array());
			parameters.InitAndSetArrayItem(recId, "id");
			parameters.InitAndSetArrayItem(Constants.PAGE_EDIT, "ptype");
			parameters.InitAndSetArrayItem(fName, "field");
			parameters.InitAndSetArrayItem(this, "pageObj");
			parameters.InitAndSetArrayItem(data[fName], "value");
			parameters.InitAndSetArrayItem(XVar.Array(), "extraParams");
			if(!XVar.Equals(XVar.Pack(this.getEditFormat((XVar)(fName))), XVar.Pack(Constants.EDIT_FORMAT_READONLY)))
			{
				parameters.InitAndSetArrayItem(this.getControl((XVar)(fName)).getDisplayValue((XVar)(data)), "value");
				parameters.InitAndSetArrayItem(this.pSet.getValidationData((XVar)(fName)), "validate");
			}
			parameters.InitAndSetArrayItem((XVar.Pack(this.mode == Constants.EDIT_INLINE) ? XVar.Pack("inline_edit") : XVar.Pack("edit")), "mode");
			if((XVar)(this.pSet.isUseRTE((XVar)(fName)))  && (XVar)(this.pSet.isAutoUpdatable((XVar)(fName))))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_", fName, "_rte")] = this.pSet.getAutoUpdateValue((XVar)(fName));
				parameters.InitAndSetArrayItem("add", "mode");
			}
			return parameters;
		}
		public virtual XVar getContolMapData(dynamic _param_fName, dynamic _param_recId, dynamic data, dynamic _param_pageFields)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic pageFields = XVar.Clone(_param_pageFields);
			#endregion

			dynamic controls = XVar.Array(), preload = null;
			if(XVar.Pack(!(XVar)(pageFields)))
			{
				pageFields = XVar.Clone(this.editFields);
			}
			controls = XVar.Clone(XVar.Array());
			controls.InitAndSetArrayItem(XVar.Array(), "controls");
			controls.InitAndSetArrayItem(0, "controls", "ctrlInd");
			controls.InitAndSetArrayItem(recId, "controls", "id");
			controls.InitAndSetArrayItem(fName, "controls", "fieldName");
			controls.InitAndSetArrayItem((XVar.Pack(this.mode == Constants.EDIT_INLINE) ? XVar.Pack("inline_edit") : XVar.Pack("edit")), "controls", "mode");
			if(XVar.Pack(this.detailsKeyField((XVar)(fName))))
			{
				controls.InitAndSetArrayItem(data[fName], "controls", "value");
			}
			preload = XVar.Clone(this.fillPreload((XVar)(fName), (XVar)(this.editFields), (XVar)(data)));
			if(!XVar.Equals(XVar.Pack(preload), XVar.Pack(false)))
			{
				controls.InitAndSetArrayItem(preload, "controls", "preloadData");
			}
			return controls;
		}
		public virtual XVar assignFieldBlocksAndLabels(dynamic _param_fields = null)
		{
			#region default values
			if(_param_fields as Object == null) _param_fields = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic fields = XVar.Clone(_param_fields);
			#endregion

			if(XVar.Pack(!(XVar)(fields)))
			{
				fields = XVar.Clone(this.getPageFields());
			}
			foreach (KeyValuePair<XVar, dynamic> fName in fields.GetEnumerator())
			{
				dynamic gfName = null;
				gfName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(fName.Value)));
				this.xt.assign((XVar)(MVCFunctions.Concat(gfName, "_fieldblock")), new XVar(true));
			}

			return null;
		}
		protected virtual XVar recheckUserPermissions()
		{
			if((XVar)(Security.isGuest())  || (XVar)(!(XVar)(CommonFunctions.isLogged())))
			{
				this.setMessage((XVar)(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("SESSION_EXPIRED1")), "<a href='#' id='loginButtonContinue", this.id, "'>", CommonFunctions.mlang_message(new XVar("SESSION_EXPIRED3")), "</a>", CommonFunctions.mlang_message(new XVar("SESSION_EXPIRED4")))));
			}
			else
			{
				this.setMessage(new XVar("You have no permissions to complete this action."));
			}
			return false;
		}
		public virtual XVar setMessage(dynamic _param_message)
		{
			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			#endregion

			this.message = XVar.Clone(message);

			return null;
		}
		public virtual XVar getEditFormat(dynamic _param_field, dynamic _param_pSet_packed = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_pSet as Object == null) _param_pSet = null;
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			if(XVar.Pack(pSet))
			{
				return pSet.getEditFormat((XVar)(field));
			}
			return this.pSetEdit.getEditFormat((XVar)(field));
		}
		public virtual XVar hasAdminPermissions()
		{
			return !XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(CommonFunctions.GetUserPermissions((XVar)(this.tName))), new XVar("M"))), XVar.Pack(false));
		}
		public virtual XVar lockingAdmin()
		{
			return (XVar)(this.hasAdminPermissions())  || (XVar)(Security.isAdmin());
		}
		public virtual XVar getMaxOrderValue(dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic alias = null, data = XVar.Array(), dc = null, rs = null;
			dc = XVar.Clone(new DsCommand());
			alias = XVar.Clone(CommonFunctions.generateAlias());
			dc.totals.InitAndSetArrayItem(new XVar("field", pSet.reorderRowsField(), "total", "max", "alias", alias), 0);
			rs = XVar.Clone(this.dataSource.getTotals((XVar)(dc)));
			data = XVar.Clone(rs.fetchAssoc());
			return (int)data[alias];
		}
		public virtual XVar getUniqueOrder(dynamic _param_pSet_packed, dynamic _param_order)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic order = XVar.Clone(_param_order);
			#endregion

			dynamic data = XVar.Array(), dc = null, orderField = null, orders = XVar.Array(), rs = null;
			orderField = XVar.Clone(pSet.reorderRowsField());
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(orderField), new XVar(Constants.dsopLESS), (XVar)(order)))));
			dc.order = XVar.Clone(new XVar(0, new XVar("column", orderField, "dir", "ASC")));
			rs = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			orders = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(rs.fetchAssoc())))
			{
				orders.InitAndSetArrayItem(true, data[orderField]);
			}
			while(XVar.Pack(orders[order]))
			{
				++(order);
			}
			return order;
		}
		protected virtual XVar getListPSet()
		{
			if(XVar.Pack(!(XVar)(this.listPagePSet)))
			{
				this.listPagePSet = XVar.Clone(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_LIST)));
			}
			return this.listPagePSet;
		}
		protected virtual XVar reoderCommandForReoderedRows(dynamic _param_listPSet, dynamic dc)
		{
			#region pass-by-value parameters
			dynamic listPSet = XVar.Clone(_param_listPSet);
			#endregion

			dynamic newOrder = XVar.Array();
			if(XVar.Pack(!(XVar)(listPSet.reorderRows())))
			{
				return null;
			}
			newOrder = XVar.Clone(new XVar(0, new XVar("column", listPSet.reorderRowsField(), "dir", (XVar.Pack(listPSet.inlineAddBottom()) ? XVar.Pack("ASC") : XVar.Pack("DESC")))));
			foreach (KeyValuePair<XVar, dynamic> o in dc.order.GetEnumerator())
			{
				newOrder.InitAndSetArrayItem(o.Value, null);
			}
			dc.order = XVar.Clone(newOrder);

			return null;
		}
		public virtual XVar inDashboardMode()
		{
			return false;
		}
		public virtual XVar dependsOnDashMap()
		{
			return (XVar)(this.inDashboardMode())  && (XVar)(this.mapRefresh);
		}
		protected virtual XVar getUserActivationUrl(dynamic _param_username, dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic code = XVar.Clone(_param_code);
			#endregion

			return MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("register")), "?a=activate&u=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.base64_encode((XVar)(username)))), "&code=", MVCFunctions.RawUrlEncode((XVar)(code)));
		}
		protected virtual XVar getSessionContolSettings()
		{
			dynamic settings = XVar.Array(), var_option = XVar.Array();
			var_option = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("sessionControl")));
			settings = XVar.Clone(XVar.Array());
			settings.InitAndSetArrayItem(var_option["keepAlive"], "keepAlive");
			settings.InitAndSetArrayItem(var_option["forceExpire"], "forceExpire");
			settings.InitAndSetArrayItem(var_option["lifeTime"], "lifeTime");
			settings.InitAndSetArrayItem((XVar.Pack(var_option["forceExpire"]) ? XVar.Pack(30) : XVar.Pack(60)), "requestDelay");
			settings.InitAndSetArrayItem(true, "clickTracking");
			settings.InitAndSetArrayItem(60, "warnBeforeSeconds");
			return settings;
		}
		protected virtual XVar hasStorageProviderFields(dynamic _param_sProvider)
		{
			#region pass-by-value parameters
			dynamic sProvider = XVar.Clone(_param_sProvider);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> field in this.pSet.getFieldsList().GetEnumerator())
			{
				if(this.pSet.fileStorageProvider((XVar)(field.Value)) == sProvider)
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar checkFSProviders()
		{
			dynamic cloudFields = XVar.Array(), providers = XVar.Array();
			cloudFields = XVar.Clone(this.pSet.getOAuthCloudFields());
			if(XVar.Pack(!(XVar)(cloudFields)))
			{
				return null;
			}
			providers = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in cloudFields.GetEnumerator())
			{
				dynamic fs = null, prov = null;
				prov = XVar.Clone(this.pSet.fileStorageProvider((XVar)(f.Value)));
				if(XVar.Pack(providers[prov]))
				{
					continue;
				}
				providers.InitAndSetArrayItem(true, prov);
				fs = XVar.Clone(CommonFunctions.getStorageProvider((XVar)(this.pSet), (XVar)(f.Value)));
				fs.loadAccessToken();
			}

			return null;
		}
		protected virtual XVar getNotifications()
		{
			dynamic noti = null, ret = XVar.Array();
			if(XVar.Pack(!(XVar)(this.pSet.hasNotifications())))
			{
				return false;
			}
			noti = XVar.Clone(new RunnerNotifications((XVar)(CommonFunctions.getNotificationSettings())));
			if(XVar.Pack(!(XVar)(noti.enabled)))
			{
				return false;
			}
			ret = XVar.Clone(XVar.Array());
			ret.InitAndSetArrayItem(noti.getMessages(), "messages");
			ret.InitAndSetArrayItem(noti.getLastRead(), "lastRead");
			ret.InitAndSetArrayItem(MVCFunctions.now(), "currentTime");
			return ret;
		}
		protected virtual XVar detailInGridAvailable(dynamic _param_details)
		{
			#region pass-by-value parameters
			dynamic details = XVar.Clone(_param_details);
			#endregion

			return true;
		}
		protected virtual XVar proceedLinkAvailable(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return this.pSet.detailsProceedLink((XVar)(dTable));
		}
		protected virtual XVar detailsHrefAvailable()
		{
			return true;
		}
		protected virtual XVar getFieldFilterFields()
		{
			if((XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_PRINT)  || (XVar)(this.pageType == Constants.PAGE_EXPORT))  || (XVar)(this.pageType == Constants.PAGE_EDIT))  || (XVar)(this.pageType == Constants.PAGE_VIEW))
			{
				dynamic settings = null;
				settings = XVar.Clone(this.getFieldFilterSettings());
				return MVCFunctions.array_keys((XVar)(settings));
			}
			return this.pSet.getFieldFilterFields();
		}
		protected virtual XVar getFieldFilterSettings()
		{
			dynamic settings = null;
			settings = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_fieldFilter")]);
			return (XVar.Pack(settings) ? XVar.Pack(settings) : XVar.Pack(XVar.Array()));
		}
		protected virtual XVar setFieldFilterSettings(dynamic _param_settings)
		{
			#region pass-by-value parameters
			dynamic settings = XVar.Clone(_param_settings);
			#endregion

			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_fieldFilter")] = settings;

			return null;
		}
		protected virtual XVar getFieldFilterFieldSettings(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			dynamic fieldSettings = null, settings = XVar.Array();
			settings = XVar.Clone(this.getFieldFilterSettings());
			fieldSettings = XVar.Clone(settings[fieldName]);
			if(XVar.Pack(!(XVar)(fieldSettings)))
			{
				return new XVar("values", XVar.Array(), "initialized", false);
			}
			return settings[fieldName];
		}
		protected virtual XVar getFieldFilterDistinctValues(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			dynamic data = XVar.Array(), dataSource = null, dc = null, fieldFilterSettings = null, fieldValues = XVar.Array(), newSettings = XVar.Array(), result = null, value = null;
			fieldFilterSettings = XVar.Clone(this.getFieldFilterSettings());
			newSettings = XVar.Clone(fieldFilterSettings);
			newSettings.Remove(fieldName);
			this.setFieldFilterSettings((XVar)(newSettings));
			dc = XVar.Clone(this.getSubsetDataCommand());
			dc.startRecord = new XVar(0);
			dc.reccount = XVar.Clone(-1);
			this.setFieldFilterSettings((XVar)(fieldFilterSettings));
			dc.totals.InitAndSetArrayItem(new XVar("field", fieldName, "total", "distinct", "caseStatement", DataCondition.CaseFieldOrNull((XVar)(DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(fieldName), new XVar(Constants.dsopEMPTY), new XVar(""))))), (XVar)(fieldName)), "alias", MVCFunctions.Concat("fieldfilter_", fieldName)), null);
			dc.reccount = XVar.Clone(GlobalVars.fieldFilterMaxValuesCount);
			dataSource = XVar.Clone(this.getDataSource());
			result = XVar.Clone(dataSource.getTotals((XVar)(dc)));
			fieldValues = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(result.fetchAssoc())))
			{
				value = XVar.Clone(data[MVCFunctions.Concat("fieldfilter_", fieldName)]);
				fieldValues.InitAndSetArrayItem(value, null);
			}
			if(this.fieldFilterSortMethod((XVar)(fieldName)) == 0)
			{
				MVCFunctions.sort(ref fieldValues, new XVar(Constants.SORT_REGULAR));
			}
			return fieldValues;
		}
		protected virtual XVar getFieldFilterState(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			dynamic allowedValues = XVar.Array(), distinctValues = XVar.Array(), filterInitialized = null, filterSettings = XVar.Array(), filterValues = null, filterValuesSet = XVar.Array(), stateEntries = XVar.Array();
			distinctValues = XVar.Clone(this.getFieldFilterDistinctValues((XVar)(fieldName)));
			filterSettings = XVar.Clone(this.getFieldFilterFieldSettings((XVar)(fieldName)));
			filterValues = XVar.Clone(filterSettings["values"]);
			filterInitialized = XVar.Clone(filterSettings["initialized"]);
			filterValuesSet = XVar.Clone(XVar.Array());
			allowedValues = XVar.Clone((XVar.Pack(filterInitialized) ? XVar.Pack(filterValues) : XVar.Pack(distinctValues)));
			foreach (KeyValuePair<XVar, dynamic> value in allowedValues.GetEnumerator())
			{
				filterValuesSet.InitAndSetArrayItem(true, value.Value);
			}
			stateEntries = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> rawValue in distinctValues.GetEnumerator())
			{
				stateEntries.InitAndSetArrayItem(this.getFieldFilterStateEntry((XVar)(fieldName), (XVar)(rawValue.Value), (XVar)(filterValuesSet)), null);
			}
			if(this.fieldFilterSortMethod((XVar)(fieldName)) == 1)
			{
				MVCFunctions.usort((XVar)(stateEntries), (XVar)(new XVar(0, this, 1, "fieldFilterSort")));
			}
			return stateEntries;
		}
		public virtual XVar fieldFilterSort(dynamic _param_e1, dynamic _param_e2)
		{
			#region pass-by-value parameters
			dynamic e1 = XVar.Clone(_param_e1);
			dynamic e2 = XVar.Clone(_param_e2);
			#endregion

			if(e1["display"] == e2["display"])
			{
				return 0;
			}
			return (XVar.Pack(e2["display"] < e1["display"]) ? XVar.Pack(1) : XVar.Pack(-1));
		}
		public virtual XVar fieldFilterSortMethod(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			dynamic var_type = null, viewFormat = null;
			if(XVar.Pack(this.pSet.lookupField((XVar)(fieldName))))
			{
				return 1;
			}
			viewFormat = XVar.Clone(this.pSet.getViewFormat((XVar)(fieldName)));
			var_type = XVar.Clone(this.pSet.getFieldType((XVar)(fieldName)));
			if((XVar)((XVar)(CommonFunctions.IsNumberType((XVar)(var_type)))  || (XVar)(CommonFunctions.IsTimeType((XVar)(var_type))))  || (XVar)(CommonFunctions.IsDateFieldType((XVar)(var_type))))
			{
				return 0;
			}
			return 1;
		}
		protected virtual XVar getFieldFilterStateEntry(dynamic _param_fieldName, dynamic _param_value, dynamic _param_selectedValuesSet)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			dynamic value = XVar.Clone(_param_value);
			dynamic selectedValuesSet = XVar.Clone(_param_selectedValuesSet);
			#endregion

			dynamic data = null, displayValue = null, returnDisplayValue = null, returnValue = null, searchValues = XVar.Array(), viewControl = null;
			returnValue = XVar.Clone((XVar.Pack(!XVar.Equals(XVar.Pack(XVar.Pack(value).ToString()), XVar.Pack(""))) ? XVar.Pack(value) : XVar.Pack(GlobalVars.fieldFilterDefaultValue)));
			data = XVar.Clone(new XVar(fieldName, returnValue));
			displayValue = XVar.Clone(XVar.Pack(this.getTextValue((XVar)(fieldName), (XVar)(data))).ToString());
			displayValue = XVar.Clone((XVar.Pack(!XVar.Equals(XVar.Pack(displayValue), XVar.Pack(""))) ? XVar.Pack(displayValue) : XVar.Pack(MVCFunctions.Concat("(", CommonFunctions.mlang_message(new XVar("EMPTY")), ")"))));
			returnDisplayValue = XVar.Clone((XVar.Pack(GlobalVars.fieldFilterMaxDisplayValueLength < MVCFunctions.runner_strlen((XVar)(displayValue))) ? XVar.Pack(MVCFunctions.Concat(MVCFunctions.runner_substr((XVar)(displayValue), new XVar(0), (XVar)(GlobalVars.fieldFilterMaxDisplayValueLength)), GlobalVars.fieldFilterValueShrinkPostfix)) : XVar.Pack(displayValue)));
			searchValues = XVar.Clone(new XVar(0, MVCFunctions.runner_substr((XVar)(displayValue), new XVar(0), (XVar)(GlobalVars.fieldFilterMaxSearchValueLength))));
			viewControl = XVar.Clone(this.getViewControl((XVar)(fieldName)));
			if((XVar)(viewControl.viewFormat == Constants.FORMAT_CURRENCY)  || (XVar)(viewControl.viewFormat == Constants.FORMAT_NUMBER))
			{
				dynamic numberDisplayValue = null;
				numberDisplayValue = XVar.Clone(CommonFunctions.formatNumberForEdit((XVar)(returnValue)));
				searchValues.InitAndSetArrayItem(MVCFunctions.runner_substr((XVar)(numberDisplayValue), new XVar(0), (XVar)(GlobalVars.fieldFilterMaxSearchValueLength)), null);
			}
			return new XVar("value", returnValue, "display", returnDisplayValue, "search", (XVar.Pack(returnValue) ? XVar.Pack(searchValues) : XVar.Pack(new XVar(0, XVar.Pack(returnValue).ToString()))), "selected", selectedValuesSet.KeyExists(returnValue));
		}
		protected virtual XVar getFieldFilterCondition()
		{
			dynamic fieldsConditions = XVar.Array(), filterFields = XVar.Array();
			filterFields = XVar.Clone(this.getFieldFilterFields());
			if(this.pageType == Constants.PAGE_LIST)
			{
				filterFields = XVar.Clone(MVCFunctions.array_intersect((XVar)(this.getPageFields()), (XVar)(filterFields)));
			}
			fieldsConditions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> fieldName in filterFields.GetEnumerator())
			{
				dynamic filterInitialized = null, filterSettings = XVar.Array(), filterValues = XVar.Array(), valuesConditions = XVar.Array();
				filterSettings = XVar.Clone(this.getFieldFilterFieldSettings((XVar)(fieldName.Value)));
				filterValues = XVar.Clone(filterSettings["values"]);
				filterInitialized = XVar.Clone(filterSettings["initialized"]);
				if(XVar.Pack(!(XVar)(filterInitialized)))
				{
					continue;
				}
				if(MVCFunctions.count(filterValues) == 0)
				{
					fieldsConditions.InitAndSetArrayItem(DataCondition._False(), null);
					continue;
				}
				valuesConditions = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> value in filterValues.GetEnumerator())
				{
					if(XVar.Equals(XVar.Pack(value.Value), XVar.Pack("")))
					{
						valuesConditions.InitAndSetArrayItem(DataCondition.FieldIs((XVar)(fieldName.Value), new XVar(Constants.dsopEMPTY), new XVar("")), null);
					}
					valuesConditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(fieldName.Value), (XVar)(value.Value)), null);
				}
				fieldsConditions.InitAndSetArrayItem(DataCondition._Or((XVar)(valuesConditions)), null);
			}
			return DataCondition._And((XVar)(fieldsConditions));
		}
		public virtual XVar processFieldFilter()
		{
			if(XVar.Pack(MVCFunctions.postvalue(new XVar("fieldFilter"))))
			{
				dynamic fieldFilterFields = null, fieldFilterParams = XVar.Array(), fieldName = null;
				fieldFilterParams = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("fieldFilter")))));
				fieldName = XVar.Clone(fieldFilterParams["field"]);
				fieldFilterFields = XVar.Clone(this.getFieldFilterFields());
				if(XVar.Pack(fieldFilterParams["clearFieldFilterValues"]))
				{
					this.updateFieldFilterSettings((XVar)(XVar.Array()));
					return false;
				}
				if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(fieldName), (XVar)(fieldFilterFields)))))
				{
					MVCFunctions.Echo(MVCFunctions.Concat("Filter is not enabled for ", fieldName, " field"));
					return true;
				}
				if(XVar.Pack(fieldFilterParams["filterValues"]))
				{
					dynamic var_response = null;
					var_response = XVar.Clone(new XVar("values", this.getFieldFilterState((XVar)(fieldName))));
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(var_response)));
					return true;
				}
				if(XVar.Pack(fieldFilterParams["clearFilter"]))
				{
					this.updateFieldFilterFieldSettings((XVar)(fieldName), (XVar)(XVar.Array()));
					return false;
				}
				if(XVar.Pack(fieldFilterParams["updateFieldFilterValues"]))
				{
					dynamic addToExisting = null, newSettings = null, newValues = null, values = null;
					values = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(fieldFilterParams["values"])));
					addToExisting = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(fieldFilterParams["addToExisting"])));
					newValues = XVar.Clone(XVar.Array());
					if(XVar.Pack(addToExisting))
					{
						dynamic fieldSettings = XVar.Array(), filterValues = null;
						fieldSettings = XVar.Clone(this.getFieldFilterFieldSettings((XVar)(fieldName)));
						filterValues = XVar.Clone(fieldSettings["values"]);
						newValues = XVar.Clone(MVCFunctions.array_unique((XVar)(MVCFunctions.array_merge((XVar)(filterValues), (XVar)(values)))));
					}
					else
					{
						newValues = XVar.Clone(values);
					}
					newSettings = XVar.Clone(new XVar("values", newValues, "initialized", true));
					this.updateFieldFilterFieldSettings((XVar)(fieldName), (XVar)(newSettings));
					return false;
				}
				MVCFunctions.Echo("Unrecognized FieldFilter request");
				return true;
			}
			return false;
		}
		protected virtual XVar fieldFilterEnabled()
		{
			return false;
		}
		protected virtual XVar updateFieldFilterFieldSettings(dynamic _param_fieldName, dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			dynamic values = XVar.Clone(_param_values);
			#endregion

			dynamic settings = XVar.Array();
			settings = XVar.Clone(this.getFieldFilterSettings());
			settings.InitAndSetArrayItem(values, fieldName);
			this.updateFieldFilterSettings((XVar)(settings));

			return null;
		}
		protected virtual XVar updateFieldFilterSettings(dynamic _param_settings)
		{
			#region pass-by-value parameters
			dynamic settings = XVar.Clone(_param_settings);
			#endregion

			this.setFieldFilterSettings((XVar)(settings));
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")] = 1;
			this.myPage = new XVar(1);

			return null;
		}
		protected virtual XVar getPwdStrongFailedMessage()
		{
			dynamic fmt = null, msg = null, pwdDigits = null, pwdLen = null, pwdUnique = null;
			msg = new XVar("");
			pwdLen = XVar.Clone(ProjectSettings.passwordValidationValue(new XVar("minimumLength")));
			if(XVar.Pack(pwdLen))
			{
				fmt = XVar.Clone(CommonFunctions.mlang_message(new XVar("SEC_PWD_LEN")));
				fmt = XVar.Clone(MVCFunctions.str_replace(new XVar("%%"), (XVar)(MVCFunctions.Concat("", pwdLen)), (XVar)(fmt)));
				msg = MVCFunctions.Concat(msg, "<br>", fmt);
			}
			pwdUnique = XVar.Clone(ProjectSettings.passwordValidationValue(new XVar("uniqueCharacters")));
			if(XVar.Pack(pwdUnique))
			{
				fmt = XVar.Clone(CommonFunctions.mlang_message(new XVar("SEC_PWD_UNIQUE")));
				fmt = XVar.Clone(MVCFunctions.str_replace(new XVar("%%"), (XVar)(MVCFunctions.Concat("", pwdUnique)), (XVar)(fmt)));
				msg = MVCFunctions.Concat(msg, "<br>", fmt);
			}
			pwdDigits = XVar.Clone(ProjectSettings.passwordValidationValue(new XVar("digitsAndSymbols")));
			if(XVar.Pack(pwdDigits))
			{
				fmt = XVar.Clone(CommonFunctions.mlang_message(new XVar("SEC_PWD_DIGIT")));
				fmt = XVar.Clone(MVCFunctions.str_replace(new XVar("%%"), (XVar)(MVCFunctions.Concat("", pwdDigits)), (XVar)(fmt)));
				msg = MVCFunctions.Concat(msg, "<br>", fmt);
			}
			if(XVar.Pack(ProjectSettings.passwordValidationValue(new XVar("upperAndLowerCase"))))
			{
				fmt = XVar.Clone(CommonFunctions.mlang_message(new XVar("SEC_PWD_CASE")));
				msg = MVCFunctions.Concat(msg, "<br>", fmt);
			}
			if(XVar.Pack(msg))
			{
				msg = XVar.Clone(MVCFunctions.substr((XVar)(msg), new XVar(4)));
			}
			return msg;
		}
		protected virtual XVar buildJsGlobalSettings()
		{
			dynamic cCharset = null, projectBuildKey = null, settings = XVar.Array(), wizardBuildKey = null;
			settings = XVar.Clone(XVar.Array());
			settings.InitAndSetArrayItem(XVar.Array(), "shortTNames");
			foreach (KeyValuePair<XVar, dynamic> _table in MVCFunctions.array_keys((XVar)(GlobalVars.runnerTableSettings)).GetEnumerator())
			{
				settings.InitAndSetArrayItem(CommonFunctions.GetTableURL((XVar)(_table.Value)), "shortTNames", _table.Value);
			}
			settings.InitAndSetArrayItem(MVCFunctions.projectPath(), "webRootPath");
			settings.InitAndSetArrayItem(MVCFunctions.projectPath(), "projectRoot");
			settings.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("ext")), "ext");
			settings.InitAndSetArrayItem(cCharset, "charSet");
			settings.InitAndSetArrayItem(CommonFunctions.mlang_getcurrentlang(), "curretLang");
			settings.InitAndSetArrayItem(this.debugJSMode, "debugMode");
			settings.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("mapSettings"), new XVar("apikey")), "mapsApiCode");
			settings.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("cookieBanner")), "useCookieBanner");
			settings.InitAndSetArrayItem(Labels.getCookieBanner(), "cookieBanner");
			projectBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("projectBuild")));
			wizardBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("wizardBuild")));
			settings.InitAndSetArrayItem(projectBuildKey, "projectBuildKey");
			settings.InitAndSetArrayItem(wizardBuildKey, "wizardBuildKey");
			settings.InitAndSetArrayItem(Security.currentProviderType() == Constants.stAD, "isAD");
			settings.InitAndSetArrayItem(this.is508, "s508");
			settings.InitAndSetArrayItem(this.mapProvider, "mapProvider");
			settings.InitAndSetArrayItem(this.pageType == Constants.PAGE_PRINT, "staticMapsOnly");
			settings.InitAndSetArrayItem(XVar.Array(), "locale");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_IDATE"], "locale", "dateFormat");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_LANGNAME"], "locale", "langName");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_CTRYNAME"], "locale", "ctryName");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_IFIRSTDAYOFWEEK"], "locale", "startWeekDay");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_SDATE"], "locale", "dateDelimiter");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_ITIME"], "locale", "is24hoursFormat");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_ITLZERO"], "locale", "leadingZero");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_STIME"], "locale", "timeDelimiter");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_S2359"], "locale", "timePmLetter");
			settings.InitAndSetArrayItem(GlobalVars.locale_info["LOCALE_S1159"], "locale", "timeAmLetter");
			settings.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("detailedError")), "showDetailedError");
			settings.InitAndSetArrayItem(ProjectSettings.getProjectValue(new XVar("customErrorMsg")), "customErrorMessage");
			settings.InitAndSetArrayItem(GlobalVars.resizeImagesOnClient, "resizeImagesOnClient");
			if((XVar)(Security.hasLogin())  && (XVar)((XVar)((XVar)(CommonFunctions.isLogged())  && (XVar)(!(XVar)(Security.isGuest())))  || (XVar)(this.pageType == Constants.PAGE_REGISTER)))
			{
				settings.InitAndSetArrayItem(this.getSessionContolSettings(), "sessionControl");
			}
			settings.InitAndSetArrayItem(Security.twoFactorSettings(), "twoFactorAuth");
			settings.InitAndSetArrayItem(this.pSet.getLoginFormType(), "loginFormType");
			settings.InitAndSetArrayItem(Security.loginTable(), "loginTName");
			settings.InitAndSetArrayItem(this.flyId, "idStartFrom");
			if(XVar.Pack(ProjectSettings.passwordValidationValue(new XVar("strong"))))
			{
				settings.InitAndSetArrayItem(ProjectSettings.passwordValidationValue(new XVar("minimumLength")), "pwdLen");
				settings.InitAndSetArrayItem(ProjectSettings.passwordValidationValue(new XVar("uniqueCharacters")), "pwdUnique");
				settings.InitAndSetArrayItem(ProjectSettings.passwordValidationValue(new XVar("digitsAndSymbols")), "pwdDigits");
				settings.InitAndSetArrayItem(true, "pwdStrong");
				settings.InitAndSetArrayItem(ProjectSettings.passwordValidationValue(new XVar("upperAndLowerCase")), "pwdUpperLower");
			}
			return settings;
		}
		protected virtual XVar buildJsTableSettings(dynamic _param_table, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic extraField = null, extraFieldSettings = XVar.Array(), pageType = null, settings = XVar.Array();
			settings = XVar.Clone(XVar.Array());
			settings.InitAndSetArrayItem(pSet.getEntityType(), "entityType");
			settings.InitAndSetArrayItem(pSet.getTableValue(new XVar("hasEvents")), "hasEvents");
			settings.InitAndSetArrayItem(pSet.getTableValue(new XVar("labels"), new XVar("tableCaption")), "strCaption");
			settings.InitAndSetArrayItem(this.listGridLayout, "listGridLayout");
			settings.InitAndSetArrayItem(pSet.getRecordsPerRowList(), "recsPerRowList");
			settings.InitAndSetArrayItem(pSet.isResizeColumns(), "isUseResize");
			settings.InitAndSetArrayItem(pSet.displayLoading(), "displayLoading");
			settings.InitAndSetArrayItem(pSet.isUseAjaxSuggest(), "ajaxSuggest");
			settings.InitAndSetArrayItem(pSet.getTableValue(new XVar("pageTypes")), "pages");
			settings.InitAndSetArrayItem(pSet.getTableKeys(), "keys");
			settings.InitAndSetArrayItem(pSet.getDefaultPages(), "defaultPages");
			settings.InitAndSetArrayItem(pSet.getDefaultPageType(), "defaultPageType");
			settings.InitAndSetArrayItem(this.mode, "pageMode");
			if(XVar.Pack(this.listAjax))
			{
				settings.InitAndSetArrayItem(Constants.LIST_AJAX, "pageMode");
			}
			settings.InitAndSetArrayItem(!(XVar)(!(XVar)(this.lockingObj)), "locking");
			if((XVar)(this.warnLeavingPages)  && (XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_REGISTER)  || (XVar)(this.pageType == Constants.PAGE_ADD))  || (XVar)(this.pageType == Constants.PAGE_EDIT)))
			{
				settings.InitAndSetArrayItem(true, "warnOnLeaving");
			}
			settings.InitAndSetArrayItem(pSet.getTableKeys(), "keys");
			settings.InitAndSetArrayItem(this.buildJsDetailsSettings((XVar)(pSet)), "detailTables");
			foreach (KeyValuePair<XVar, dynamic> details in pSet.getDetailsTables().GetEnumerator())
			{
				dynamic previewType = null;
				previewType = XVar.Clone(pSet.detailsPreview((XVar)(details.Value)));
				if(!XVar.Equals(XVar.Pack(previewType), XVar.Pack(Constants.DP_NONE)))
				{
					settings.InitAndSetArrayItem(true, "isShowDetails");
				}
				if(previewType == Constants.DP_POPUP)
				{
					settings.InitAndSetArrayItem(true, "isUsePopUp");
				}
			}
			settings.InitAndSetArrayItem(this.useCKEditor(), "isUseCK");
			pageType = XVar.Clone(pSet.getPageType());
			settings.InitAndSetArrayItem(XVar.Array(), "fieldSettings");
			foreach (KeyValuePair<XVar, dynamic> field in pSet.getFieldsList().GetEnumerator())
			{
				settings.InitAndSetArrayItem(new XVar(this.pageType, this.buildJsFieldSettings((XVar)(pSet), (XVar)(field.Value), (XVar)(this.pageType))), "fieldSettings", field.Value);
			}
			if((XVar)(this.googleMapCfg)  && (XVar)(this.googleMapCfg["isUseGoogleMap"]))
			{
				settings.InitAndSetArrayItem(true, "isUseGoogleMap");
				settings.InitAndSetArrayItem(this.googleMapCfg, "googleMapCfg");
			}
			if((XVar)(this.searchPanelActivated)  && (XVar)(this.permis[this.searchTableName]["search"]))
			{
				foreach (KeyValuePair<XVar, dynamic> field in this.pSetSearch.getAllSearchFields().GetEnumerator())
				{
					settings.InitAndSetArrayItem(this.buildJsFieldSettings((XVar)(this.pSetSearch), (XVar)(field.Value), new XVar(Constants.PAGE_SEARCH)), "fieldSettings", field.Value, Constants.PAGE_SEARCH);
				}
			}
			extraFieldSettings = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.captchaExists()))
			{
				extraField = XVar.Clone(this.getCaptchaFieldName());
				extraFieldSettings.InitAndSetArrayItem(this.buildJsExtraField((XVar)(extraField), new XVar(Constants.EDIT_FORMAT_TEXT_FIELD)), extraField);
			}
			if(this.pageType == Constants.PAGE_REGISTER)
			{
				extraField = new XVar("confirm");
				extraFieldSettings.InitAndSetArrayItem(this.buildJsExtraField((XVar)(extraField), new XVar(Constants.EDIT_FORMAT_PASSWORD)), extraField);
			}
			if(this.pageType == Constants.PAGE_CHANGEPASS)
			{
				foreach (KeyValuePair<XVar, dynamic> _extraField in new XVar(0, "oldpass", 1, "newpass", 2, "confirm").GetEnumerator())
				{
					extraFieldSettings.InitAndSetArrayItem(this.buildJsExtraField((XVar)(_extraField.Value), new XVar(Constants.EDIT_FORMAT_PASSWORD)), _extraField.Value);
				}
			}
			if(this.mode == Constants.MEMBERS_PAGE)
			{
				foreach (KeyValuePair<XVar, dynamic> _extraField in new XVar(0, "displayname", 1, "name", 2, "category").GetEnumerator())
				{
					extraFieldSettings.InitAndSetArrayItem(this.buildJsExtraField((XVar)(_extraField.Value), new XVar(Constants.EDIT_FORMAT_TEXT_FIELD)), _extraField.Value);
				}
			}
			foreach (KeyValuePair<XVar, dynamic> extraSettings in extraFieldSettings.GetEnumerator())
			{
				settings.InitAndSetArrayItem(extraSettings.Value, "fieldSettings", extraSettings.Key, pageType);
			}
			settings.InitAndSetArrayItem((XVar)(this.searchSavingEnabled)  && (XVar)(this.searchClauseObj), "searchSaving");
			settings.InitAndSetArrayItem(this.editAvailable(), "editAvailable");
			settings.InitAndSetArrayItem(this.viewAvailable(), "viewAvailable");
			settings.InitAndSetArrayItem(this.maxPages, "maxPages");
			settings.InitAndSetArrayItem(this.masterPageId, "masterPageId");
			return settings;
		}
		protected virtual XVar buildJsExtraField(dynamic _param_extraField, dynamic _param_editFormat)
		{
			#region pass-by-value parameters
			dynamic extraField = XVar.Clone(_param_extraField);
			dynamic editFormat = XVar.Clone(_param_editFormat);
			#endregion

			return new XVar("strName", extraField, "editFormat", editFormat, "validation", new XVar("validationArr", new XVar(0, "IsRequired")));
		}
		protected virtual XVar buildJsDetailsSettings(dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic settings = XVar.Array();
			if((XVar)((XVar)(this.pageType != Constants.PAGE_LIST)  && (XVar)(this.pageType != Constants.PAGE_REPORT))  && (XVar)(this.pageType != Constants.PAGE_CHART))
			{
				return XVar.Array();
			}
			settings = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> details in pSet.getDetailsTables().GetEnumerator())
			{
				dynamic dPageType = null, dPermission = XVar.Array(), dPset = null;
				dPermission = XVar.Clone(this.getPermissions((XVar)(details.Value)));
				if(XVar.Pack(!(XVar)(dPermission["search"])))
				{
					continue;
				}
				dPageType = XVar.Clone(ProjectSettings.defaultPageType((XVar)(CommonFunctions.GetEntityType((XVar)(details.Value)))));
				dPset = XVar.Clone(new ProjectSettings((XVar)(details.Value), (XVar)(dPageType), (XVar)(pSet.detailsPageId((XVar)(details.Value)))));
				settings.InitAndSetArrayItem(new XVar("pageType", dPageType, "dispChildCount", pSet.detailsShowCount((XVar)(details.Value)), "hideChild", pSet.detailsHideEmpty((XVar)(details.Value)), "listShowType", pSet.detailsPreview((XVar)(details.Value)), "addShowType", pSet.detailsPreview((XVar)(details.Value)), "editShowType", pSet.detailsPreview((XVar)(details.Value)), "viewShowType", pSet.detailsPreview((XVar)(details.Value)), "proceedLink", this.proceedLinkAvailable((XVar)(details.Value)), "label", Labels.getTableCaption((XVar)(details.Value)), "title", this.getPageTitle((XVar)(pSet.detailsPageId((XVar)(details.Value))), (XVar)(details.Value), new XVar(null), (XVar)(dPset)), "pageId", pSet.detailsPageId((XVar)(details.Value)), "hideEmptyPreview", pSet.detailsHideEmptyPreview((XVar)(details.Value)), "detailsHrefAvailable", this.detailsHrefAvailable()), details.Value);
			}
			return settings;
		}
		protected virtual XVar buildJsFieldSettings(dynamic _param_pSet_packed, dynamic _param_field, dynamic _param_targetPageType)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic field = XVar.Clone(_param_field);
			dynamic targetPageType = XVar.Clone(_param_targetPageType);
			#endregion

			dynamic editFormat = null, originalEditFormat = null, settings = XVar.Array(), validationPageTypes = null, viewFormat = null;
			settings = XVar.Clone(XVar.Array());
			settings.InitAndSetArrayItem(pSet.isUseTimestamp((XVar)(field)), "isUseTimeStamp");
			settings.InitAndSetArrayItem(field, "strName");
			viewFormat = XVar.Clone(pSet.getViewFormat((XVar)(field)));
			editFormat = XVar.Clone(pSet.getEditFormat((XVar)(field)));
			originalEditFormat = XVar.Clone(editFormat);
			if(this.pSet.table() == pSet.table())
			{
				editFormat = XVar.Clone(this.getEditFormat((XVar)(field)));
			}
			settings.InitAndSetArrayItem(viewFormat, "viewFormat");
			settings.InitAndSetArrayItem(editFormat, "editFormat");
			if(XVar.Equals(XVar.Pack(editFormat), XVar.Pack(Constants.EDIT_FORMAT_DATE)))
			{
				dynamic dateEditType = null, pageType = null;
				dateEditType = XVar.Clone(pSet.getDateEditType((XVar)(field)));
				pageType = XVar.Clone(pSet.getPageType());
				if((XVar)((XVar)((XVar)((XVar)(this.pageType == Constants.PAGE_LIST)  || (XVar)(this.pageType == Constants.PAGE_CHART))  || (XVar)(this.pageType == Constants.PAGE_REPORT))  || (XVar)(this.pageType == Constants.PAGE_DASHBOARD))  || (XVar)((XVar)(this.pageType == Constants.PAGE_SEARCH)  && (XVar)(this.mode == Constants.SEARCH_LOAD_CONTROL)))
				{
					if(dateEditType == Constants.EDIT_DATE_DD)
					{
						dateEditType = new XVar(Constants.EDIT_DATE_SIMPLE);
					}
					else
					{
						if(dateEditType == Constants.EDIT_DATE_DD_DP)
						{
							dateEditType = new XVar(Constants.EDIT_DATE_SIMPLE_DP);
						}
						else
						{
							if(dateEditType == Constants.EDIT_DATE_DD_INLINE)
							{
								dateEditType = new XVar(Constants.EDIT_DATE_SIMPLE_INLINE);
							}
						}
					}
				}
				settings.InitAndSetArrayItem(dateEditType, "dateEditType");
				settings.InitAndSetArrayItem(pSet.dateEditShowTime((XVar)(field)), "showTime");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("dateLastYearFactor")), "lastYear");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("dateFirstYearFactor")), "initialYear");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("dateAllowedDays")), "weekdays");
				settings.InitAndSetArrayItem(CommonFunctions.GetMLString((XVar)(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("dateAllowedDaysMessage")))), "weekdayMessage");
			}
			if(XVar.Equals(XVar.Pack(editFormat), XVar.Pack(Constants.EDIT_FORMAT_TEXT_AREA)))
			{
				if(XVar.Pack(pSet.isUseRTE((XVar)(field))))
				{
					settings.InitAndSetArrayItem(ProjectSettings.rteType(), "RTEType");
				}
				settings.InitAndSetArrayItem(pSet.getNRows((XVar)(field)), "nHeight");
			}
			settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("textboxMask")), "mask");
			settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageWidth")), "imageWidth");
			settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageHeight")), "imageHeight");
			settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileResize")), "resizeImage");
			settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileResizeSize")), "resizeImageSize");
			settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), (XVar)((XVar.Pack(this.pageType == Constants.PAGE_VIEW) ? XVar.Pack("view") : XVar.Pack("edit"))), new XVar("fieldEvents")), "events");
			if(XVar.Equals(XVar.Pack(editFormat), XVar.Pack(Constants.EDIT_FORMAT_FILE)))
			{
				dynamic acceptedTypes = null;
				acceptedTypes = XVar.Clone(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileTypes")));
				settings.InitAndSetArrayItem((XVar.Pack(0 < MVCFunctions.count(acceptedTypes)) ? XVar.Pack(MVCFunctions.Concat("(", MVCFunctions.implode(new XVar("|"), (XVar)(acceptedTypes)), ")$")) : XVar.Pack(".+$")), "acceptFileTypes");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("basicUpload")), "compatibilityMode");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileSizeLimit")), "maxFileSize");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileTotalSizeLimit")), "maxTotalFilesSize");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileMaxNumber")), "maxNumberOfFiles");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("storageProvider")), "fileStorageProvider");
			}
			if((XVar)(XVar.Equals(XVar.Pack(editFormat), XVar.Pack(Constants.EDIT_FORMAT_LOOKUP_WIZARD)))  || (XVar)(XVar.Equals(XVar.Pack(originalEditFormat), XVar.Pack(Constants.EDIT_FORMAT_LOOKUP_WIZARD))))
			{
				settings.InitAndSetArrayItem(pSet.getLookupParentFNames((XVar)(field)), "parentFields");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupControlType")), "lcType");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupTable")), "lookupTable");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupSize")), "selectSize");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupMultiselect")), "Multiselect");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupLinkField")), "linkField");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupDisplayField")), "dispField");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupOrderBy")), "lookupOrderBy");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupOrderByDesc")), "lookupDesc");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupFreeInput")), "freeInput");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupHorizontal")), "HorizontalLookup");
				settings.InitAndSetArrayItem(pSet.getAutoCompleteFields((XVar)(field)), "autoCompleteFields");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupListPage")), "listPageId");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupAddPage")), "addPageId");
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupEditPage")), "editPageId");
				this.addMainFieldsSettings((XVar)(pSet), (XVar)(field), (XVar)(settings));
			}
			if(XVar.Equals(XVar.Pack(editFormat), XVar.Pack(Constants.EDIT_FORMAT_TIME)))
			{
				dynamic timeAttrs = XVar.Array();
				timeAttrs = XVar.Clone(pSet.getFormatTimeAttrs((XVar)(field)));
				if((XVar)(!(XVar)(!(XVar)(timeAttrs)))  && (XVar)(timeAttrs["useTimePicker"]))
				{
					settings.InitAndSetArrayItem(new XVar("convention", timeAttrs["hours"], "showSec", timeAttrs["showSeconds"], "minutes", timeAttrs["minutes"]), "timePick");
				}
			}
			validationPageTypes = XVar.Clone(new XVar(0, Constants.PAGE_ADD, 1, Constants.PAGE_EDIT, 2, Constants.PAGE_REGISTER));
			if((XVar)((XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(pSet.getPageType()), (XVar)(validationPageTypes))), XVar.Pack(false)))  || (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(this.pageType), (XVar)(validationPageTypes))), XVar.Pack(false))))  || (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(targetPageType), (XVar)(validationPageTypes))), XVar.Pack(false))))
			{
				settings.InitAndSetArrayItem(this.buildJsValidationData((XVar)(pSet), (XVar)(field)), "validation");
			}
			if((XVar)(MVCFunctions.is_array((XVar)(this.jsControlSettings[field])))  && (XVar)(MVCFunctions.is_array((XVar)(this.jsControlSettings[field][targetPageType]))))
			{
				foreach (KeyValuePair<XVar, dynamic> value in this.jsControlSettings[field][targetPageType].GetEnumerator())
				{
					settings.InitAndSetArrayItem(value.Value, value.Key);
				}
			}
			return settings;
		}
		protected virtual XVar addMainFieldsSettings(dynamic _param_pSet_packed, dynamic _param_field, dynamic settings)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic mainFields = XVar.Array(), mainMasterFields = XVar.Array(), where = null;
			if(XVar.Pack(pSet.isLookupWhereCode((XVar)(field))))
			{
				return null;
			}
			mainMasterFields = XVar.Clone(XVar.Array());
			mainFields = XVar.Clone(XVar.Array());
			where = XVar.Clone(pSet.getLookupWhere((XVar)(field)));
			foreach (KeyValuePair<XVar, dynamic> token in DB.readSQLTokens((XVar)(where)).GetEnumerator())
			{
				dynamic dotPos = null, prefix = null;
				prefix = new XVar("");
				field = XVar.Clone(token.Value);
				dotPos = XVar.Clone(MVCFunctions.strpos((XVar)(token.Value), new XVar(".")));
				if(!XVar.Equals(XVar.Pack(dotPos), XVar.Pack(false)))
				{
					prefix = XVar.Clone(MVCFunctions.strtolower((XVar)(MVCFunctions.substr((XVar)(token.Value), new XVar(0), (XVar)(dotPos)))));
					field = XVar.Clone(MVCFunctions.substr((XVar)(token.Value), (XVar)(dotPos + 1)));
				}
				if(prefix == "master")
				{
					mainMasterFields.InitAndSetArrayItem(field, null);
				}
				else
				{
					if(XVar.Pack(!(XVar)(prefix)))
					{
						mainFields.InitAndSetArrayItem(field, null);
					}
				}
			}
			settings.InitAndSetArrayItem(mainFields, "mainFields");
			settings.InitAndSetArrayItem(mainMasterFields, "mainMasterFields");

			return null;
		}
		protected virtual XVar buildJsValidationData(dynamic _param_pSet_packed, dynamic _param_field)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic editFormat = null, settings = XVar.Array();
			editFormat = XVar.Clone(pSet.getEditFormat((XVar)(field)));
			settings = XVar.Clone(XVar.Array());
			settings.InitAndSetArrayItem(XVar.Array(), "validationArr");
			settings.InitAndSetArrayItem(XVar.Array(), "customMessages");
			if((XVar)((XVar)(editFormat == Constants.EDIT_FORMAT_TEXT_FIELD)  || (XVar)(editFormat == Constants.EDIT_FORMAT_PASSWORD))  || (XVar)(editFormat == Constants.EDIT_FORMAT_TIME))
			{
				dynamic validateAs = XVar.Array();
				if(XVar.Pack(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("validateRegex"))))
				{
					settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("validateRegex")), "regExp");
					settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("validateRegexMessage")), "customMessages", "RegExp");
				}
				validateAs = XVar.Clone(pSet.getValidationData((XVar)(field)));
				if(XVar.Pack(validateAs))
				{
					settings.InitAndSetArrayItem(validateAs["basicValidate"], "validationArr");
					if(XVar.Equals(XVar.Pack(validateAs), XVar.Pack("IsTime")))
					{
						settings.InitAndSetArrayItem(this.getTimeRegexp(), "regExp");
					}
				}
			}
			if((XVar)(!XVar.Equals(XVar.Pack(editFormat), XVar.Pack(Constants.EDIT_FORMAT_READONLY)))  && (XVar)(pSet.isRequired((XVar)(field))))
			{
				settings.InitAndSetArrayItem("IsRequired", "validationArr", null);
			}
			if(XVar.Pack(!(XVar)(pSet.allowDuplicateValues((XVar)(field)))))
			{
				settings.InitAndSetArrayItem("DenyDuplicated", "validationArr", null);
				settings.InitAndSetArrayItem(pSet.getFieldValue((XVar)(field), new XVar("edit"), new XVar("denyDuplicateMessage")), "customMessages", "DenyDuplicated");
			}
			return settings;
		}
		protected virtual XVar useRichText()
		{
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(ProjectSettings.richTextEnabled())))
			{
				return false;
			}
			pSet = XVar.UnPackProjectSettings(this.pSet);
			foreach (KeyValuePair<XVar, dynamic> f in pSet.getFieldsList().GetEnumerator())
			{
				if((XVar)(XVar.Equals(XVar.Pack(pSet.getEditFormat((XVar)(f.Value))), XVar.Pack(Constants.EDIT_FORMAT_TEXT_AREA)))  && (XVar)(pSet.isUseRTE((XVar)(f.Value))))
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar useCKEditor()
		{
			dynamic rteType = null;
			if(XVar.Pack(!(XVar)(ProjectSettings.richTextEnabled())))
			{
				return false;
			}
			rteType = XVar.Clone(ProjectSettings.rteType());
			return (XVar)(rteType == Constants.rteCKEditor)  || (XVar)(rteType == Constants.rteCKEditor5);
		}
		protected virtual XVar showDetailsPreview()
		{
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				if(!XVar.Equals(XVar.Pack(this.pSet.detailsPreview((XVar)(details.Value))), XVar.Pack(Constants.DP_NONE)))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar getLookupData(dynamic _param_lookupTable, dynamic _param_lookupField, dynamic _param_lookupPageType, dynamic _param_newRecordData)
		{
			#region pass-by-value parameters
			dynamic lookupTable = XVar.Clone(_param_lookupTable);
			dynamic lookupField = XVar.Clone(_param_lookupField);
			dynamic lookupPageType = XVar.Clone(_param_lookupPageType);
			dynamic newRecordData = XVar.Clone(_param_newRecordData);
			#endregion

			dynamic dispfield = null, linkField = null, lookupPSet = null, lvals = XVar.Array(), respData = XVar.Array();
			lookupPSet = XVar.Clone(this.pSet.getLookupMainTableSettings((XVar)(lookupTable), (XVar)(lookupField), (XVar)(lookupPageType)));
			if(XVar.Pack(!(XVar)(lookupPSet)))
			{
				return XVar.Array();
			}
			lvals = XVar.Clone(this.getNewLookupValues((XVar)(lookupPSet), (XVar)(lookupField), (XVar)(newRecordData)));
			if(XVar.Pack(!(XVar)(lvals)))
			{
				return XVar.Array();
			}
			linkField = XVar.Clone(lookupPSet.getLinkField((XVar)(lookupField)));
			dispfield = XVar.Clone(lookupPSet.getDisplayField((XVar)(lookupField)));
			respData = XVar.Clone(new XVar(linkField, lvals["link"], dispfield, lvals["display"]));
			if(XVar.Pack(MVCFunctions.in_array((XVar)(lookupPSet.getViewFormat((XVar)(lookupField))), (XVar)(new XVar(0, Constants.FORMAT_DATE_SHORT, 1, Constants.FORMAT_DATE_LONG, 2, Constants.FORMAT_DATE_TIME)))))
			{
				dynamic ctrlData = XVar.Array(), viewContainer = null;
				viewContainer = XVar.Clone(new ViewControlsContainer((XVar)(lookupPSet), new XVar(Constants.PAGE_LIST), new XVar(null)));
				ctrlData = XVar.Clone(XVar.Array());
				ctrlData.InitAndSetArrayItem(respData[linkField], lookupField);
				respData.InitAndSetArrayItem(viewContainer.getControl((XVar)(lookupField)).getTextValue((XVar)(ctrlData)), dispfield);
			}
			return new XVar("linkValue", respData[linkField], "displayValue", respData[dispfield], "vals", respData);
		}
		protected virtual XVar getNewLookupValues(dynamic _param_lookupPSet, dynamic _param_lookupField, dynamic newRecordData)
		{
			#region pass-by-value parameters
			dynamic lookupPSet = XVar.Clone(_param_lookupPSet);
			dynamic lookupField = XVar.Clone(_param_lookupField);
			#endregion

			dynamic data = XVar.Array(), dispFieldName = null, linkFieldName = null;
			linkFieldName = XVar.Clone(lookupPSet.getLinkField((XVar)(lookupField)));
			dispFieldName = XVar.Clone(lookupPSet.getDisplayField((XVar)(lookupField)));
			if(XVar.Pack(this.keys))
			{
				dynamic dc = null;
				dc = XVar.Clone(new DsCommand());
				dc.keys = XVar.Clone(this.keys);
				if(XVar.Pack(lookupPSet.getCustomDisplay((XVar)(lookupField))))
				{
					dynamic customField = null;
					customField = XVar.Clone(new DsFieldData((XVar)(dispFieldName), (XVar)(CommonFunctions.generateAlias()), new XVar("")));
					dispFieldName = XVar.Clone(customField.alias);
					dc.extraColumns.InitAndSetArrayItem(customField, null);
				}
				data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(this.dataSource.getSingle((XVar)(dc)).fetchAssoc())));
			}
			if(XVar.Pack(!(XVar)(data)))
			{
				data = XVar.Clone(newRecordData);
			}
			return new XVar("link", data[linkFieldName], "display", data[dispFieldName]);
		}
		public virtual XVar recordEditable(dynamic _param_data, dynamic _param_permission = null)
		{
			#region default values
			if(_param_permission as Object == null) _param_permission = new XVar("E");
			#endregion

			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic permission = XVar.Clone(_param_permission);
			#endregion

			dynamic editable = null;
			editable = XVar.Clone(Security.userCan((XVar)(permission), (XVar)(this.tName), new XVar(true), (XVar)(data[this.pSet.getTableOwnerIdField()])));
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("IsRecordEditable"), (XVar)(this.tName))))
			{
				editable = XVar.Clone(GlobalVars.globalEvents.IsRecordEditable((XVar)(data), new XVar(true), (XVar)(this.tName)));
			}
			return editable;
		}
	}
	public partial class DetailsPreview : RunnerPage
	{
		protected static bool skipDetailsPreviewCtor = false;
		public DetailsPreview(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipDetailsPreviewCtor)
			{
				skipDetailsPreviewCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

		}
		protected override XVar assignSessionPrefix()
		{
			this.sessionPrefix = new XVar("_detailsPreview");

			return null;
		}
	}
}
