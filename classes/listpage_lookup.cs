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
	public partial class ListPage_Lookup : ListPage_Embed
	{
		public dynamic linkField = XVar.Pack("");
		public dynamic lookupSelectField = XVar.Pack("");
		public dynamic customField = XVar.Pack("");
		public dynamic dispField = XVar.Pack("");
		public dynamic dispFieldAlias = XVar.Pack("");
		public dynamic lookupValuesArr = XVar.Array();
		public dynamic parentCtrlsData;
		public dynamic mainPageType;
		protected dynamic mainPSet;
		public dynamic mainRecordData;
		public dynamic mainRecordMasterTable;
		public dynamic mainContext;
		public dynamic showSearchPanel;
		protected static bool skipListPage_LookupCtor = false;
		public ListPage_Lookup(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipListPage_LookupCtor)
			{
				skipListPage_LookupCtor = false;
				return;
			}
			this.initLookupParams();
			this.showAddInPopup = new XVar(true);
			this.permis.InitAndSetArrayItem(1, this.tName, "search");
			this.showSearchPanel = new XVar(false);
		}
		protected override XVar assignSessionPrefix()
		{
			this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.tName, "_lookup_", this.mainTable, "_", this.mainField));

			return null;
		}
		public virtual XVar initLookupParams()
		{
			if((XVar)((XVar)(this.mainPageType != Constants.PAGE_ADD)  && (XVar)(this.mainPageType != Constants.PAGE_EDIT))  && (XVar)(this.mainPageType != Constants.PAGE_REGISTER))
			{
				this.mainPageType = new XVar(Constants.PAGE_SEARCH);
			}
			this.mainPSet = XVar.Clone(new ProjectSettings((XVar)(this.mainTable), (XVar)(this.mainPageType)));
			this.linkField = XVar.Clone(this.mainPSet.getLinkField((XVar)(this.mainField)));
			this.dispField = XVar.Clone(this.mainPSet.getDisplayField((XVar)(this.mainField)));
			if(XVar.Pack(this.mainPSet.getCustomDisplay((XVar)(this.mainField))))
			{
				this.dispFieldAlias = XVar.Clone(CommonFunctions.generateAlias());
				this.customField = XVar.Clone(this.linkField);
			}
			this.outputFieldValue((XVar)(this.linkField), new XVar(2));
			this.outputFieldValue((XVar)(this.dispField), new XVar(2));
			if((XVar)(this.dispFieldAlias)  && (XVar)(this.pSet.appearOnListPage((XVar)(this.dispField))))
			{
				this.lookupSelectField = XVar.Clone(this.dispField);
			}
			else
			{
				if(XVar.Pack(this.pSet.appearOnListPage((XVar)(this.dispField))))
				{
					this.lookupSelectField = XVar.Clone(this.dispField);
				}
				else
				{
					this.lookupSelectField = XVar.Clone(this.listFields[0]["fName"]);
				}
			}
			this.mainContext = XVar.Clone(this.getMainContext());

			return null;
		}
		public override XVar displayMasterTableInfo()
		{

			return null;
		}
		public override XVar processMasterKeyValue()
		{

			return null;
		}
		protected virtual XVar getMainContext()
		{
			dynamic contextParams = XVar.Array();
			contextParams = XVar.Clone(XVar.Array());
			contextParams.InitAndSetArrayItem(this.mainRecordData, "data");
			if((XVar)(this.mainRecordMasterTable)  && (XVar)(XSession.Session.KeyExists(MVCFunctions.Concat(this.mainRecordMasterTable, "_masterRecordData"))))
			{
				contextParams.InitAndSetArrayItem(XSession.Session[MVCFunctions.Concat(this.mainRecordMasterTable, "_masterRecordData")], "masterData");
			}
			return contextParams;
		}
		public override XVar clearSessionKeys()
		{
			base.clearSessionKeys();
			if(XVar.Pack(this.firstTime))
			{
				this.unsetAllPageSessionKeys();
			}

			return null;
		}
		public override XVar addCommonJs()
		{
			this.controlsMap.InitAndSetArrayItem(this.dispFieldAlias, "dispFieldAlias");
			this.addControlsJSAndCSS();
			this.addButtonHandlers();

			return null;
		}
		public override XVar addSpanVal(dynamic _param_fName, dynamic data)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			if((XVar)(this.dispFieldAlias)  && (XVar)(this.arrFieldSpanVal[fName] == 2))
			{
				return MVCFunctions.Concat("val=\"", MVCFunctions.runner_htmlspecialchars((XVar)(data[this.dispFieldAlias])), "\" ");
			}
			else
			{
				return base.addSpanVal((XVar)(fName), (XVar)(data));
			}

			return null;
		}
		public override XVar getOrderByClause()
		{
			dynamic orderByField = null, strOrder = null, userStartingSort = null;
			userStartingSort = new XVar(false);
			if(XVar.Pack(XSession.Session.KeyExists(MVCFunctions.Concat(this.sessionPrefix, "_orderby"))))
			{
				userStartingSort = new XVar(true);
			}
			orderByField = XVar.Clone(this.mainPSet.getLookupOrderBy((XVar)(this.mainField)));
			if((XVar)(!(XVar)(userStartingSort))  && (XVar)(MVCFunctions.strlen((XVar)(orderByField))))
			{
				strOrder = XVar.Clone(MVCFunctions.Concat(" ORDER BY ", this.getFieldSQL((XVar)(orderByField))));
				if(XVar.Pack(this.mainPSet.isLookupDesc((XVar)(this.mainField))))
				{
					strOrder = MVCFunctions.Concat(strOrder, " DESC");
				}
			}
			else
			{
				strOrder = XVar.Clone(base.getOrderByClause());
			}
			return strOrder;
		}
		protected virtual XVar getCategoryFilter()
		{
			dynamic conditions = XVar.Array();
			if(XVar.Pack(!(XVar)(this.mainPSet.useCategory((XVar)(this.mainField)))))
			{
				return null;
			}
			if((XVar)(this.mainPageType != Constants.PAGE_SEARCH)  && (XVar)(!(XVar)(this.parentCtrlsData)))
			{
				return DataCondition._False();
			}
			conditions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> cData in this.mainPSet.getParentFieldsData((XVar)(this.mainField)).GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.parentCtrlsData.KeyExists(cData.Value["main"]))))
				{
					continue;
				}
				conditions.InitAndSetArrayItem(LookupField.categoryCondition((XVar)(this.mainPSet), (XVar)(cData.Value["main"]), (XVar)(cData.Value["lookup"]), (XVar)(this.parentCtrlsData[cData.Value["main"]])), null);
			}
			return DataCondition._And((XVar)(conditions));
		}
		public override XVar getSubsetDataCommand(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic dc = null, filters = XVar.Array();
			dc = XVar.Clone(base.getSubsetDataCommand());
			if(XVar.Pack(this.dispFieldAlias))
			{
				dc.extraColumns.InitAndSetArrayItem(new DsFieldData((XVar)(this.dispField), (XVar)(this.dispFieldAlias), new XVar("")), null);
			}
			filters = XVar.Clone(XVar.Array());
			filters.InitAndSetArrayItem(DataCondition.SQLCondition((XVar)(this.getLookupWizardWhere())), null);
			filters.InitAndSetArrayItem(this.getCategoryFilter(), null);
			filters.InitAndSetArrayItem(dc.filter, null);
			dc.filter = XVar.Clone(DataCondition._And((XVar)(filters)));
			return dc;
		}
		protected virtual XVar getLookupWizardWhere()
		{
			dynamic where = null;
			RunnerContext.push((XVar)(new RunnerContextItem(new XVar(Constants.CONTEXT_ROW), (XVar)(this.mainContext))));
			where = XVar.Clone(CommonFunctions.prepareLookupWhere((XVar)(this.mainField), (XVar)(this.mainPSet)));
			RunnerContext.pop();
			return where;
		}
		public override XVar buildSearchPanel()
		{
			if(XVar.Pack(this.showSearchPanel))
			{
				base.buildSearchPanel();
			}

			return null;
		}
		protected override XVar assignSearchControl()
		{
			dynamic searchforAttrs = null, var_params = XVar.Array();
			searchforAttrs = XVar.Clone(MVCFunctions.Concat("placeholder=\"", CommonFunctions.mlang_message(new XVar("SEARCH_TIP")), "\""));
			var_params = XVar.Clone(this.searchClauseObj.getSearchGlobalParams());
			if(XVar.Pack(this.searchClauseObj.searchStarted()))
			{
				dynamic valSrchFor = null;
				valSrchFor = XVar.Clone(var_params["simpleSrch"]);
				searchforAttrs = MVCFunctions.Concat(searchforAttrs, " value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(valSrchFor)), "\"");
			}
			searchforAttrs = MVCFunctions.Concat(searchforAttrs, " size=\"15\" name=\"ctlSearchFor", this.id, "\" id=\"ctlSearchFor", this.id, "\"");
			this.xt.assign(new XVar("searchfor_attrs"), (XVar)(searchforAttrs));

			return null;
		}
		public virtual XVar addLookupVals()
		{
			this.controlsMap.InitAndSetArrayItem(this.lookupValuesArr, "lookupVals");

			return null;
		}
		public override XVar fillGridData()
		{
			base.fillGridData();
			this.addLookupVals();

			return null;
		}
		public override XVar fillCheckAttr(dynamic record, dynamic _param_data, dynamic _param_keyblock)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic keyblock = XVar.Clone(_param_keyblock);
			#endregion

			dynamic checkbox_attrs = null;
			checkbox_attrs = XVar.Clone(MVCFunctions.Concat("name=\"selection[]\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(data[this.linkField])), "\" id=\"check", this.recId, "\""));
			record.InitAndSetArrayItem(new XVar("begin", MVCFunctions.Concat("<input type='checkbox' ", checkbox_attrs, ">"), "data", XVar.Array()), "checkbox");

			return null;
		}
		public override XVar addSpansForGridCells(dynamic _param_type, dynamic record, dynamic _param_data = null)
		{
			#region default values
			if(_param_data as Object == null) _param_data = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if(var_type == "add")
			{
				base.addSpansForGridCells((XVar)(var_type), (XVar)(record), (XVar)(data));
				return null;
			}
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(data), XVar.Pack(null)))))
			{
				dynamic dispVal = null;
				if(XVar.Pack(this.dispFieldAlias))
				{
					dispVal = XVar.Clone(data[this.dispFieldAlias]);
				}
				else
				{
					dispVal = XVar.Clone(data[this.dispField]);
				}
				if(XVar.Pack(MVCFunctions.in_array((XVar)(this.mainPSet.getViewFormat((XVar)(this.mainField))), (XVar)(new XVar(0, Constants.FORMAT_DATE_SHORT, 1, Constants.FORMAT_DATE_LONG, 2, Constants.FORMAT_DATE_TIME)))))
				{
					dynamic ctrlData = XVar.Array(), viewContainer = null;
					viewContainer = XVar.Clone(new ViewControlsContainer((XVar)(this.mainPSet), new XVar(Constants.PAGE_LIST), new XVar(null)));
					ctrlData = XVar.Clone(XVar.Array());
					ctrlData.InitAndSetArrayItem(data[this.linkField], this.mainField);
					dispVal = XVar.Clone(viewContainer.getControl((XVar)(this.mainField)).getTextValue((XVar)(ctrlData)));
				}
				if(XVar.Pack(this.mainPSet.isAllowToEdit((XVar)(this.mainField))))
				{
					dynamic keys = XVar.Array();
					keys = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> kf in this.pSetEdit.getTableKeys().GetEnumerator())
					{
						keys.InitAndSetArrayItem(data[kf.Value], null);
					}
					this.lookupValuesArr.InitAndSetArrayItem(new XVar("linkVal", data[this.linkField], "dispVal", dispVal, "keys", keys), null);
				}
				else
				{
					this.lookupValuesArr.InitAndSetArrayItem(new XVar("linkVal", data[this.linkField], "dispVal", dispVal), null);
				}
			}

			return null;
		}
		public override XVar proccessRecordValue(dynamic data, dynamic keylink, dynamic _param_listFieldInfo, dynamic _param_isEditable = null)
		{
			#region default values
			if(_param_isEditable as Object == null) _param_isEditable = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic listFieldInfo = XVar.Clone(_param_listFieldInfo);
			dynamic isEditable = XVar.Clone(_param_isEditable);
			#endregion

			dynamic value = null;
			value = XVar.Clone(base.proccessRecordValue((XVar)(data), (XVar)(keylink), (XVar)(listFieldInfo), (XVar)(isEditable)));
			if(this.lookupSelectField == listFieldInfo["fName"])
			{
				value = XVar.Clone(MVCFunctions.Concat("<a href=\"#\" data-ind=\"", MVCFunctions.count(this.lookupValuesArr), "\" type=\"lookupSelect", this.id, "\">", value, "</a>"));
			}
			return value;
		}
		public override XVar showPage()
		{
			this.BeforeShowList();
			this.hideAllFormItems(new XVar("supertop"));
			this.showItemType(new XVar("simple_search"));
			this.showItemType(new XVar("simple_search_field"));
			this.showItemType(new XVar("simple_search_option"));
			this.hideItemType(new XVar("columns_control"));
			if(XVar.Pack(this.showSearchPanel))
			{
				this.showItemType(new XVar("search_panel"));
				this.hideItemType(new XVar("logo"));
				this.hideItemType(new XVar("menu"));
				this.hideItemType(new XVar("breadcrumb"));
				this.hideItemType(new XVar("filter_panel"));
			}
			this.xt.prepare_template((XVar)(this.templatefile));
			this.showPageAjax();

			return null;
		}
		public virtual XVar showPageAjax()
		{
			dynamic bodyHtml = null, footerHtml = null, lookupSearchControls = null, returnJSON = XVar.Array(), superTopHtml = null;
			lookupSearchControls = XVar.Clone(MVCFunctions.Concat(this.xt.fetch_loaded(new XVar("searchform_text")), this.xt.fetch_loaded(new XVar("searchform_search")), this.xt.fetch_loaded(new XVar("searchform_showall"))));
			this.xt.assign(new XVar("lookupSearchControls"), (XVar)(lookupSearchControls));
			this.addControlsJSAndCSS();
			this.fillSetCntrlMaps();
			returnJSON = XVar.Clone(XVar.Array());
			returnJSON.InitAndSetArrayItem(GlobalVars.pagesData, "pagesData");
			returnJSON.InitAndSetArrayItem(this.controlsHTMLMap, "controlsMap");
			returnJSON.InitAndSetArrayItem(this.viewControlsHTMLMap, "viewControlsMap");
			returnJSON.InitAndSetArrayItem(this.jsSettings, "settings");
			this.xt.assign(new XVar("header"), new XVar(false));
			this.xt.assign(new XVar("footer"), new XVar(false));
			returnJSON.InitAndSetArrayItem(MVCFunctions.Concat("<h3 data-itemtype=\"lookupheader\" data-itemid=\"lookupheader\">", this.getPageTitle((XVar)(this.pageType), (XVar)(this.tName)), "</h3>"), "headerCont");
			superTopHtml = XVar.Clone(this.xt.fetch_loaded(new XVar("supertop_block")));
			footerHtml = XVar.Clone(this.xt.fetch_loaded(new XVar("below-grid_block")));
			if(XVar.Pack(this.showSearchPanel))
			{
				this.xt.assign(new XVar("supertop_block"), new XVar(false));
				this.xt.assign(new XVar("below-grid_block"), new XVar(false));
				bodyHtml = XVar.Clone(this.xt.fetch_loaded(new XVar("body")));
			}
			else
			{
				bodyHtml = XVar.Clone(this.fetchBlocksList((XVar)(new XVar(0, "above-grid_block", 1, "grid_tabs", 2, "grid_block"))));
			}
			returnJSON.InitAndSetArrayItem(MVCFunctions.Concat(superTopHtml, "<div class=\"r-popup-block\">", "<div class=\"r-popup-data\">", bodyHtml, "</div>", "</div>"), "html");
			returnJSON.InitAndSetArrayItem(footerHtml, "footerCont");
			returnJSON.InitAndSetArrayItem(this.flyId, "idStartFrom");
			returnJSON.InitAndSetArrayItem(true, "success");
			returnJSON.InitAndSetArrayItem(this.grabAllJsFiles(), "additionalJS");
			returnJSON.InitAndSetArrayItem(this.grabAllCSSFiles(), "CSSFiles");
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));

			return null;
		}
		public override XVar displayTabsInPage()
		{
			return true;
		}
		public override XVar buildTotals(dynamic totals)
		{

			return null;
		}
		public override XVar getMasterTableSQLClause()
		{
			return "";
		}
		public override XVar deleteAvailable()
		{
			return false;
		}
		public override XVar importAvailable()
		{
			return false;
		}
		public override XVar editAvailable()
		{
			return false;
		}
		public override XVar addAvailable()
		{
			return (XVar)(base.addAvailable())  && (XVar)(this.mainPSet.isAllowToAdd((XVar)(this.mainField)));
		}
		public override XVar copyAvailable()
		{
			return false;
		}
		public override XVar inlineAddAvailable()
		{
			return (XVar)(base.inlineAddAvailable())  && (XVar)(this.mainPSet.isAllowToAdd((XVar)(this.mainField)));
		}
		public override XVar inlineEditAvailable()
		{
			return false;
		}
		public override XVar viewAvailable()
		{
			return false;
		}
		public override XVar exportAvailable()
		{
			return false;
		}
		public override XVar printAvailable()
		{
			return false;
		}
		public override XVar advSearchAvailable()
		{
			return false;
		}
		public override XVar detailsInGridAvailable()
		{
			return false;
		}
		public override XVar updateSelectedAvailable()
		{
			return false;
		}
		public override XVar getSecurityCondition()
		{
			dynamic loginTable = null;
			loginTable = XVar.Clone(Security.loginTable());
			if((XVar)(this.mainPageType == Constants.PAGE_REGISTER)  && (XVar)(this.mainTable == loginTable))
			{
				dynamic registerPset = null;
				registerPset = XVar.Clone(new ProjectSettings((XVar)(loginTable), new XVar(Constants.PAGE_REGISTER), new XVar(""), new XVar(Constants.GLOBAL_PAGES)));
				if(XVar.Pack(registerPset.appearOnPage((XVar)(this.mainField))))
				{
					return null;
				}
			}
			return base.getSecurityCondition();
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
			settings.InitAndSetArrayItem(this.permis[this.tName], "permissions");
			return settings;
		}
	}
}
