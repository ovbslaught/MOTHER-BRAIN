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
	public partial class ListPage_Dashboard : ListPage_Embed
	{
		public dynamic showNoData = XVar.Pack(false);
		protected static bool skipListPage_DashboardCtor = false;
		public ListPage_Dashboard(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipListPage_DashboardCtor)
			{
				skipListPage_DashboardCtor = false;
				return;
			}
			this.showAddInPopup = new XVar(true);
			this.formBricks.InitAndSetArrayItem(new XVar(0, new XVar("name", "details_block", "align", "right"), 1, new XVar("name", "newrecord_controls_block", "align", "right"), 2, new XVar("name", "record_controls_block", "align", "right"), 3, new XVar("name", "details_found", "align", "right")), "header");
			this.formBricks.InitAndSetArrayItem(new XVar(0, "pagination_block"), "footer");
			if(XVar.Pack(this.mapRefresh))
			{
				this.pageSize = XVar.Clone(-1);
			}
			this.listAjax = new XVar(false);
		}
		protected override XVar assignSessionPrefix()
		{
			this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.dashTName, "_", this.dashElementName));

			return null;
		}
		protected override XVar buildJsTableSettings(dynamic _param_table, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic settings = XVar.Array();
			settings = XVar.Clone(base.buildJsTableSettings((XVar)(table), (XVar)(pSet)));
			if(XVar.Pack(this.addAvailable()))
			{
				settings.InitAndSetArrayItem(true, "showAddInPopup");
			}
			if((XVar)(this.editAvailable())  || (XVar)(this.updateSelectedAvailable()))
			{
				settings.InitAndSetArrayItem(true, "showEditInPopup");
			}
			if(XVar.Pack(this.viewAvailable()))
			{
				settings.InitAndSetArrayItem(true, "showViewInPopup");
			}
			if(XVar.Pack(this.inlineEditAvailable()))
			{
				settings.InitAndSetArrayItem(true, "inlineEditAvailable");
			}
			settings.InitAndSetArrayItem((XVar.Pack(this.numRowsFromSQL) ? XVar.Pack(true) : XVar.Pack(false)), "showRows");
			return settings;
		}
		public override XVar isDispGrid()
		{
			return this.permis[this.tName]["search"];
		}
		public override XVar addCommonJs()
		{
			this.addJsForGrid();
			this.addControlsJSAndCSS();
			this.addButtonHandlers();

			return null;
		}
		public override XVar addJsForGrid()
		{
			if(XVar.Pack(this.isResizeColumns))
			{
				this.prepareForResizeColumns();
			}

			return null;
		}
		public override XVar prepareForResizeColumns()
		{
			dynamic columnsData = null, logger = null;
			base.prepareForResizeColumns();
			logger = XVar.Clone(new paramsLogger((XVar)(MVCFunctions.Concat(this.dashTName, "_", this.dashElementName)), new XVar(Constants.CRESIZE_PARAMS_TYPE)));
			columnsData = XVar.Clone(logger.getData());
			if(XVar.Pack(columnsData))
			{
				this.pageData.InitAndSetArrayItem(columnsData, "resizableColumnsData");
			}

			return null;
		}
		public override XVar commonAssign()
		{
			base.commonAssign();
			this.xt.assign(new XVar("details_block"), new XVar(true));
			this.xt.assign(new XVar("withSelected"), (XVar)((XVar)(this.inlineEditAvailable())  || (XVar)(this.deleteAvailable())));
			this.hideElement(new XVar("printpanel"));

			return null;
		}
		protected override XVar prepareBreadcrumbs()
		{

			return null;
		}
		public override XVar getSubsetDataCommand(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(base.getSubsetDataCommand());
			if(XVar.Pack(this.showNoData))
			{
				dc.filter = XVar.Clone(DataCondition._False());
			}
			return dc;
		}
		public override XVar showPage()
		{
			this.BeforeShowList();
			this.prepareGridTabs();
			this.xt.prepare_template((XVar)(this.templatefile));
			this.showPageAjax();

			return null;
		}
		public virtual XVar showPageAjax()
		{
			dynamic icon = null, returnJSON = XVar.Array();
			this.addControlsJSAndCSS();
			this.fillSetCntrlMaps();
			returnJSON = XVar.Clone(XVar.Array());
			returnJSON.InitAndSetArrayItem(GlobalVars.pagesData, "pagesData");
			returnJSON.InitAndSetArrayItem(this.controlsHTMLMap, "controlsMap");
			returnJSON.InitAndSetArrayItem(this.viewControlsHTMLMap, "viewControlsMap");
			returnJSON.InitAndSetArrayItem(this.jsSettings, "settings");
			this.xt.assign(new XVar("header"), new XVar(false));
			this.xt.assign(new XVar("footer"), new XVar(false));
			this.hideForm(new XVar("above-grid"));
			returnJSON.InitAndSetArrayItem(this.fetchBlocksList((XVar)(new XVar(0, "above-grid_block", 1, "grid_tabs", 2, "grid_block"))), "html");
			if((XVar)((XVar)(this.mode == Constants.LIST_DASHDETAILS)  && (XVar)(this.dashElementData["item"]["customLabel"]))  && (XVar)(this.dashElementData["item"]["dashLabel"]))
			{
				returnJSON.InitAndSetArrayItem(MVCFunctions.Concat("<span class=\"rnr-dbebrick\">", CommonFunctions.GetMLString((XVar)(this.dashElementData["item"]["dashLabel"])), "</span>"), "headerCont");
			}
			else
			{
				returnJSON.InitAndSetArrayItem(MVCFunctions.Concat("<span class=\"rnr-dbebrick\">", this.getPageTitle((XVar)(this.pageName), (XVar)(this.tName)), "</span>"), "headerCont");
			}
			returnJSON.InitAndSetArrayItem(this.fetchForms((XVar)(new XVar(0, "below-grid"))), "footerCont");
			icon = XVar.Clone(CommonFunctions.getIconHTML((XVar)(this.dashElementData["item"]["icon"])));
			if(XVar.Pack(icon))
			{
				returnJSON.InitAndSetArrayItem(icon, "iconHtml");
			}
			returnJSON.InitAndSetArrayItem(this.flyId, "idStartFrom");
			returnJSON.InitAndSetArrayItem(true, "success");
			returnJSON.InitAndSetArrayItem(this.grabAllJsFiles(), "additionalJS");
			returnJSON.InitAndSetArrayItem(this.grabAllCSSFiles(), "CSSFiles");
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));

			return null;
		}
		public override XVar fillSetCntrlMaps()
		{
			base.fillSetCntrlMaps();
			this.controlsHTMLMap.InitAndSetArrayItem(this.myPage, this.tName, this.pageType, this.id, "pageNumber");
			this.controlsHTMLMap.InitAndSetArrayItem(this.maxPages, this.tName, this.pageType, this.id, "numberOfPages");

			return null;
		}
		public override XVar fillCheckAttr(dynamic record, dynamic _param_data, dynamic _param_keyblock)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic keyblock = XVar.Clone(_param_keyblock);
			#endregion

			if((XVar)((XVar)(this.deleteAvailable())  || (XVar)(this.inlineEditAvailable()))  || (XVar)(this.updateSelectedAvailable()))
			{
				record.InitAndSetArrayItem(true, "checkbox");
			}
			record.InitAndSetArrayItem(MVCFunctions.Concat("name=\"selection[]\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(keyblock)), "\" id=\"check", this.id, "_", this.recId, "\""), "checkbox_attrs");

			return null;
		}
		public override XVar deleteAvailable()
		{
			return (XVar)(base.deleteAvailable())  && (XVar)(this.dashElementData["deleteRecord"]);
		}
		public override XVar editAvailable()
		{
			return (XVar)(base.editAvailable())  && (XVar)(this.dashElementData["popupEdit"]);
		}
		public override XVar copyAvailable()
		{
			return (XVar)(base.copyAvailable())  && (XVar)(this.dashElementData["copy"]);
		}
		public override XVar addAvailable()
		{
			return (XVar)(base.addAvailable())  && (XVar)(this.dashElementData["popupAdd"]);
		}
		public override XVar inlineEditAvailable()
		{
			return (XVar)(base.inlineEditAvailable())  && (XVar)(this.dashElementData["inlineEdit"]);
		}
		public override XVar inlineAddAvailable()
		{
			return (XVar)(base.inlineAddAvailable())  && (XVar)(this.dashElementData["inlineAdd"]);
		}
		public override XVar updateSelectedAvailable()
		{
			return (XVar)(base.updateSelectedAvailable())  && (XVar)(this.dashElementData["updateSelected"]);
		}
		public override XVar viewAvailable()
		{
			return (XVar)(base.viewAvailable())  && (XVar)(this.dashElementData["popupView"]);
		}
		protected virtual XVar hasDependentDashMapElem()
		{
			foreach (KeyValuePair<XVar, dynamic> dElem in this.dashSet.getDashboardElements().GetEnumerator())
			{
				if((XVar)((XVar)(dElem.Value["table"] == this.tName)  && (XVar)(dElem.Value["type"] == Constants.DASHBOARD_MAP))  && (XVar)(!(XVar)(dElem.Value["updateMoved"])))
				{
					return true;
				}
			}
			return false;
		}
		protected override XVar hasMainDashMapElem()
		{
			foreach (KeyValuePair<XVar, dynamic> dElem in this.dashSet.getDashboardElements().GetEnumerator())
			{
				if((XVar)((XVar)(dElem.Value["table"] == this.tName)  && (XVar)(dElem.Value["type"] == Constants.DASHBOARD_MAP))  && (XVar)(dElem.Value["updateMoved"]))
				{
					return true;
				}
			}
			return false;
		}
		protected override XVar hasBigMap()
		{
			return (XVar)(base.hasBigMap())  || (XVar)(this.hasDependentDashMapElem());
		}
		public override XVar addBigGoogleMapMarkers(dynamic data, dynamic _param_keys, dynamic _param_editLink = null)
		{
			#region default values
			if(_param_editLink as Object == null) _param_editLink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			dynamic editLink = XVar.Clone(_param_editLink);
			#endregion

			base.addBigGoogleMapMarkers((XVar)(data), (XVar)(keys), (XVar)(editLink));
			foreach (KeyValuePair<XVar, dynamic> dElem in this.dashSet.getDashboardElements().GetEnumerator())
			{
				dynamic mapId = null, markerData = XVar.Array();
				if((XVar)((XVar)((XVar)(dElem.Value["elementName"] == this.dashElementName)  || (XVar)(dElem.Value["table"] != this.tName))  || (XVar)(dElem.Value["type"] != Constants.DASHBOARD_MAP))  || (XVar)(dElem.Value["updateMoved"]))
				{
					continue;
				}
				markerData = XVar.Clone(XVar.Array());
				markerData.InitAndSetArrayItem(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)((XVar.Pack(data[dElem.Value["latF"]]) ? XVar.Pack(data[dElem.Value["latF"]]) : XVar.Pack("")))), "lat");
				markerData.InitAndSetArrayItem(MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)((XVar.Pack(data[dElem.Value["lonF"]]) ? XVar.Pack(data[dElem.Value["lonF"]]) : XVar.Pack("")))), "lng");
				markerData.InitAndSetArrayItem((XVar.Pack(data[dElem.Value["addressF"]]) ? XVar.Pack(data[dElem.Value["addressF"]]) : XVar.Pack("")), "address");
				markerData.InitAndSetArrayItem((XVar.Pack(data[dElem.Value["descF"]]) ? XVar.Pack(data[dElem.Value["descF"]]) : XVar.Pack(markerData["address"])), "desc");
				markerData.InitAndSetArrayItem(this.dashSet.getDashMapIcon((XVar)(dElem.Value["elementName"]), (XVar)(data)), "mapIcon");
				markerData.InitAndSetArrayItem(this.recId, "recId");
				markerData.InitAndSetArrayItem(keys, "keys");
				markerData.InitAndSetArrayItem(this.getDetailTablesMasterKeys((XVar)(data)), "masterKeys");
				mapId = XVar.Clone(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(this.dashTName)), "_", dElem.Value["elementName"], "_dashMap"));
				if(XVar.Pack(!(XVar)(this.googleMapCfg["mapsData"].KeyExists(mapId))))
				{
					this.googleMapCfg.InitAndSetArrayItem(XVar.Array(), "mapsData", mapId);
					this.googleMapCfg.InitAndSetArrayItem(true, "mapsData", mapId, "skipped");
					this.googleMapCfg.InitAndSetArrayItem(true, "mapsData", mapId, "dashMap");
					this.googleMapCfg.InitAndSetArrayItem(dElem.Value["heatMap"], "mapsData", mapId, "heatMap");
				}
				if(XVar.Pack(!(XVar)(this.googleMapCfg["mapsData"][mapId].KeyExists("markers"))))
				{
					this.googleMapCfg.InitAndSetArrayItem(XVar.Array(), "mapsData", mapId, "markers");
				}
				if((XVar)(markerData["lat"] == "")  || (XVar)(markerData["lng"] == ""))
				{
					continue;
				}
				this.googleMapCfg.InitAndSetArrayItem(markerData, "mapsData", mapId, "markers", null);
			}

			return null;
		}
		protected override XVar isInlineAreaToSet()
		{
			if(this.mode == Constants.LIST_DASHBOARD)
			{
				return true;
			}
			return base.isInlineAreaToSet();
		}
		public override XVar rulePRG()
		{

			return null;
		}
		public override XVar buildSearchPanel()
		{

			return null;
		}
		public override XVar assignSimpleSearch()
		{

			return null;
		}
		public override XVar printAvailable()
		{
			return false;
		}
		public override XVar getTabDataCommand(dynamic _param_tab)
		{
			#region pass-by-value parameters
			dynamic tab = XVar.Clone(_param_tab);
			#endregion

			dynamic dc = null;
			this.skipMapFilter = new XVar(true);
			dc = XVar.Clone(base.getTabDataCommand((XVar)(tab)));
			this.skipMapFilter = new XVar(false);
			return dc;
		}
		public override XVar setGoogleMapsParams(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			base.setGoogleMapsParams((XVar)(var_params));
			this.googleMapCfg.InitAndSetArrayItem(false, "isUseMainMaps");

			return null;
		}
		public override XVar addCenterLink(ref dynamic value, dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			return value;
		}
		public override XVar inDashboardMode()
		{
			return true;
		}
		public override XVar dependsOnDashMap()
		{
			return (XVar)(this.hasMainDashMapElem())  && (XVar)(this.mapRefresh);
		}
		protected override XVar detailInGridAvailable(dynamic _param_detTable)
		{
			#region pass-by-value parameters
			dynamic detTable = XVar.Clone(_param_detTable);
			#endregion

			if(this.pSet.detailsPreview((XVar)(detTable)) == Constants.DP_NONE)
			{
				return false;
			}
			return true;
		}
		protected override XVar proceedLinkAvailable(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return false;
		}
		protected override XVar detailsHrefAvailable()
		{
			return false;
		}
		public override XVar displayTabsInPage()
		{
			return this.dashElementData["tabLocation"] == "body";
		}
	}
}
