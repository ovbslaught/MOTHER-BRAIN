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
	public partial class ListPage_DPInline : ListPage_Embed
	{
		public dynamic dpParams = XVar.Pack("");
		public dynamic dpMasterKey = XVar.Array();
		public dynamic masterShortTable = XVar.Pack("");
		public dynamic masterFormName = XVar.Pack("");
		public dynamic masterId = XVar.Pack("");
		protected static bool skipListPage_DPInlineCtor = false;
		public ListPage_DPInline(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipListPage_DPInlineCtor)
			{
				skipListPage_DPInlineCtor = false;
				return;
			}
			this.showAddInPopup = new XVar(true);
			this.showEditInPopup = new XVar(true);
			this.showViewInPopup = new XVar(true);
			this.initDPInlineParams();
			if(XVar.Pack(this.firstTime))
			{
				this.searchClauseObj.resetSearch();
				XSession.Session.Remove(MVCFunctions.Concat(this.sessionPrefix, "_advsearch"));
			}
			this.addRawFieldValues = new XVar(true);
			if((XVar)(this.masterPageType == Constants.PAGE_ADD)  && (XVar)(base.spreadsheetGridApplicable()))
			{
				this.pageData.InitAndSetArrayItem(true, "spreadsheetOnList");
			}
		}
		public override XVar importLinksAttrs()
		{

			return null;
		}
		public override XVar displayMasterTableInfo()
		{

			return null;
		}
		public override XVar processMasterKeyValue()
		{
			dynamic i = null;
			base.processMasterKeyValue();
			i = new XVar(1);
			for(;i <= MVCFunctions.count(this.masterKeysReq); i++)
			{
				this.dpMasterKey.InitAndSetArrayItem(this.masterKeysReq[i], null);
			}

			return null;
		}
		public virtual XVar initDPInlineParams()
		{
			dynamic i = null, strkey = null;
			strkey = new XVar("");
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.dpMasterKey); i++)
			{
				strkey = MVCFunctions.Concat(strkey, "&masterkey", i + 1, "=", MVCFunctions.RawUrlEncode((XVar)(this.dpMasterKey[i])));
			}
			this.dpParams = XVar.Clone(MVCFunctions.Concat("mode=listdetails&id=", this.id, "&mastertable=", MVCFunctions.RawUrlEncode((XVar)(this.masterTable)), strkey, (XVar.Pack(this.masterId) ? XVar.Pack(MVCFunctions.Concat("&masterid=", this.masterId)) : XVar.Pack("")), (XVar.Pack((XVar)(this.masterPageType == Constants.PAGE_EDIT)  || (XVar)(this.masterPageType == Constants.PAGE_VIEW)) ? XVar.Pack(MVCFunctions.Concat("&masterpagetype=", this.masterPageType)) : XVar.Pack(""))));

			return null;
		}
		public virtual XVar getStrMasterKey()
		{
			dynamic i = null, strkey = XVar.Array();
			strkey = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.dpMasterKey); i++)
			{
				strkey.InitAndSetArrayItem(this.dpMasterKey[i], i);
			}
			return strkey;
		}
		public override XVar isReoderByHeaderClickingEnabled()
		{
			return this.masterPageType != Constants.PAGE_ADD;
		}
		public override XVar inlineAddLinksAttrs()
		{
			if(this.masterPageType != Constants.PAGE_VIEW)
			{
				base.inlineAddLinksAttrs();
			}

			return null;
		}
		public override XVar commonAssign()
		{
			base.commonAssign();
			this.xt.assign(new XVar("left_block"), new XVar(false));
			if((XVar)((XVar)(this.masterPageType == Constants.PAGE_ADD)  || (XVar)(this.masterPageType == Constants.PAGE_VIEW))  || (XVar)(this.mode == Constants.LIST_DASHDETAILS))
			{
				this.xt.assign(new XVar("selectall_link"), new XVar(false));
				this.xt.assign(new XVar("checkbox_column"), new XVar(false));
				this.xt.assign(new XVar("checkbox_header"), new XVar(false));
				this.xt.assign(new XVar("editselected_link"), new XVar(false));
				this.xt.assign(new XVar("delete_link"), new XVar(false));
				this.xt.assign(new XVar("saveall_link"), new XVar(false));
				this.xt.assign(new XVar("withSelectedClass"), new XVar("rnr-hiddenelem"));
				if(this.masterPageType == Constants.PAGE_VIEW)
				{
					this.xt.assign(new XVar("record_controls_block"), new XVar(false));
				}
			}
			else
			{
				this.selectAllLinkAttrs();
				this.checkboxColumnAttrs();
				this.editSelectedLinkAttrs();
				this.saveAllLinkAttrs();
				this.deleteSelectedLink();
				if(this.masterPageType != Constants.PAGE_EDIT)
				{
					dynamic searchPermis = null;
					searchPermis = XVar.Clone(this.permis[this.tName]["search"]);
					this.xt.assign(new XVar("record_controls_block"), (XVar)((XVar)((XVar)(this.permis[this.tName]["edit"])  && (XVar)(this.pSet.hasInlineEdit()))  || (XVar)((XVar)(this.permis[this.tName]["delete"])  && (XVar)(this.pSet.hasDelete()))));
					this.xt.assign(new XVar("details_block"), (XVar)((XVar)(searchPermis)  && (XVar)(this.recordsOnPage)));
					this.xt.assign(new XVar("details_attrs"), (XVar)(MVCFunctions.Concat("id=\"detFound", this.id, "\" name=\"detFound", this.id, "\"")));
					this.xt.assign(new XVar("pages_block"), (XVar)((XVar)(searchPermis)  && (XVar)(this.recordsOnPage)));
				}
			}
			this.xt.assign(new XVar("withSelected"), (XVar)((XVar)((XVar)(this.permis[this.tName]["export"])  || (XVar)(this.permis[this.tName]["edit"]))  || (XVar)(this.permis[this.tName]["delete"])));
			if(this.numRowsFromSQL == 0)
			{
				this.hideElement(new XVar("recordcontrol"));
			}
			if(this.masterPageType != Constants.PAGE_VIEW)
			{
				this.xt.assign(new XVar("inlineedit_column"), (XVar)((XVar)((XVar)(this.inlineEditAvailable())  && (XVar)(this.permis[this.tName]["edit"]))  && (XVar)(!(XVar)(this.spreadsheetGridApplicable()))));
				this.assignListIconsColumn();
				this.cancelAllLinkAttrs();
			}
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				dynamic permis = null;
				permis = XVar.Clone((XVar)((XVar)(Security.permissionsAvailable())  && (XVar)((XVar)(this.permis[details.Value]["add"])  || (XVar)(this.permis[details.Value]["search"])))  || (XVar)(!(XVar)(Security.permissionsAvailable())));
				if(XVar.Pack(permis))
				{
					this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(this.tName)), "_dtable_column")), (XVar)(permis));
					break;
				}
			}

			return null;
		}
		protected virtual XVar prepareTemplate()
		{
			this.xt.assign(new XVar("header"), new XVar(false));
			this.xt.assign(new XVar("footer"), new XVar(false));
			this.xt.prepare_template((XVar)(this.templatefile));

			return null;
		}
		protected virtual XVar getBSButtonsClass()
		{
			return "btn btn-xs btn-info";
		}
		protected virtual XVar getHeaderControlsBlocks()
		{
			dynamic bs_button_class = null, buttons = null, caption = null, controlsBlocks = null;
			controlsBlocks = new XVar("");
			buttons = new XVar("");
			bs_button_class = XVar.Clone(this.getBSButtonsClass());
			if((XVar)(this.inlineAddAvailable())  && (XVar)(this.xt.getVar(new XVar("inlineadd_link"))))
			{
				dynamic inlineaddlink_attrs = null;
				inlineaddlink_attrs = XVar.Clone(this.xt.getVar(new XVar("inlineaddlink_attrs")));
				if(XVar.Pack(this.addAvailable()))
				{
					caption = XVar.Clone(CommonFunctions.mlang_message(new XVar("INLINE_ADD")));
				}
				else
				{
					caption = XVar.Clone(CommonFunctions.mlang_message(new XVar("AA_P_ADD")));
				}
				buttons = MVCFunctions.Concat(buttons, "<a class=\"", bs_button_class, "\" href=\"#\" ", inlineaddlink_attrs, ">", caption, "</a> ");
			}
			if((XVar)(this.addAvailable())  && (XVar)(this.xt.getVar(new XVar("add_link"))))
			{
				dynamic addlink_attrs = null;
				addlink_attrs = XVar.Clone(this.xt.getVar(new XVar("addlink_attrs")));
				if(XVar.Pack(this.inlineAddAvailable()))
				{
					caption = XVar.Clone(CommonFunctions.mlang_message(new XVar("ADD_NEW")));
				}
				else
				{
					caption = XVar.Clone(CommonFunctions.mlang_message(new XVar("AA_P_ADD")));
				}
				buttons = MVCFunctions.Concat(buttons, "<a class=\"", bs_button_class, "\" href=\"#\" ", addlink_attrs, ">", caption, "</a> ");
			}
			if((XVar)(this.inlineEditAvailable())  && (XVar)(this.xt.getVar(new XVar("editselected_link"))))
			{
				dynamic editselectedlink_attrs = null, editselectedlink_span = null;
				editselectedlink_attrs = XVar.Clone(this.xt.getVar(new XVar("editselectedlink_attrs")));
				editselectedlink_span = XVar.Clone(this.xt.getVar(new XVar("editselectedlink_span")));
				buttons = MVCFunctions.Concat(buttons, "<a class=\"", bs_button_class, " \" disabled href=\"#\" ", editselectedlink_attrs, " ", editselectedlink_span, ">", CommonFunctions.mlang_message(new XVar("AA_P_EDIT")), "</a> ");
			}
			if((XVar)(this.updateSelectedAvailable())  && (XVar)(this.xt.getVar(new XVar("updateselected_link"))))
			{
				dynamic updateselectedlink_attrs = null;
				updateselectedlink_attrs = XVar.Clone(this.xt.getVar(new XVar("updateselectedlink_attrs")));
				buttons = MVCFunctions.Concat(buttons, "<a class=\"", bs_button_class, "\" disabled ", updateselectedlink_attrs, ">", CommonFunctions.mlang_message(new XVar("UPDATE_SELECTED")), "</a> ");
			}
			if(XVar.Pack(this.xt.getVar(new XVar("saveall_link"))))
			{
				dynamic savealllink_attrs = null, savealllink_span = null;
				savealllink_attrs = XVar.Clone(this.xt.getVar(new XVar("savealllink_attrs")));
				savealllink_span = XVar.Clone(this.xt.getVar(new XVar("savealllink_span")));
				buttons = MVCFunctions.Concat(buttons, "<a class=\"", bs_button_class, "\" href=\"#\" ", savealllink_attrs, " ", savealllink_span, ">", CommonFunctions.mlang_message(new XVar("SAVE_ALL")), "</a> ");
			}
			if(XVar.Pack(this.xt.getVar(new XVar("cancelall_link"))))
			{
				dynamic cancelalllink_attrs = null, cancelalllink_span = null;
				cancelalllink_attrs = XVar.Clone(this.xt.getVar(new XVar("cancelalllink_attrs")));
				cancelalllink_span = XVar.Clone(this.xt.getVar(new XVar("cancelalllink_span")));
				buttons = MVCFunctions.Concat(buttons, "<a class=\"", bs_button_class, "\" href=\"#\" ", cancelalllink_attrs, " ", cancelalllink_span, ">", CommonFunctions.mlang_message(new XVar("CANCEL_ALL")), "</a> ");
			}
			if((XVar)(this.deleteAvailable())  && (XVar)(this.xt.getVar(new XVar("deleteselected_link"))))
			{
				dynamic deleteselectedlink_attrs = null, deleteselectedlink_span = null;
				deleteselectedlink_attrs = XVar.Clone(this.xt.getVar(new XVar("deleteselectedlink_attrs")));
				deleteselectedlink_span = XVar.Clone(this.xt.getVar(new XVar("deleteselectedlink_span")));
				buttons = MVCFunctions.Concat(buttons, "<a class=\"", bs_button_class, "\" disabled href=\"#\" ", deleteselectedlink_attrs, " ", deleteselectedlink_span, ">", CommonFunctions.mlang_message(new XVar("AA_P_DELETE")), "</a> ");
			}
			if(XVar.Pack(buttons))
			{
				controlsBlocks = MVCFunctions.Concat(controlsBlocks, "<span class=\"rnr-dbebrick \">", buttons, "</span>");
			}
			return MVCFunctions.Concat(controlsBlocks, "<div class=\"rnr-dbefiller\"></div>");
		}
		public override XVar showPage()
		{
			dynamic contents = null, var_response = XVar.Array();
			this.BeforeShowList();
			var_response = XVar.Clone(XVar.Array());
			if((XVar)(!(XVar)(this.isDispGrid()))  && (XVar)(0 == this.getGridTabsCount()))
			{
				var_response.InitAndSetArrayItem(true, "noData");
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(var_response)));
				return null;
			}
			this.prepareTemplate();
			contents = XVar.Clone(this.fetchBlocksList((XVar)(new XVar(0, "grid_tabs", 1, "grid_block", 2, "below-grid_block"))));
			this.addControlsJSAndCSS();
			this.fillSetCntrlMaps();
			var_response.InitAndSetArrayItem(GlobalVars.pagesData, "pagesData");
			var_response.InitAndSetArrayItem(this.jsSettings, "settings");
			var_response.InitAndSetArrayItem(this.controlsHTMLMap, "controlsMap");
			var_response.InitAndSetArrayItem(this.viewControlsHTMLMap, "viewControlsMap");
			if((XVar)((XVar)(this.masterPageType == Constants.PAGE_EDIT)  && (XVar)(this.dashTName))  && (XVar)(this.dashElementName))
			{
				var_response.InitAndSetArrayItem(this.getHeaderControlsBlocks(), "headerButtonsBlock");
			}
			var_response.InitAndSetArrayItem(contents, "html");
			var_response.InitAndSetArrayItem(true, "success");
			var_response.InitAndSetArrayItem(this.id, "id");
			var_response.InitAndSetArrayItem(this.flyId, "idStartFrom");
			var_response.InitAndSetArrayItem(this.recordsDeleted, "delRecs");
			if(this.deleteMessage != "")
			{
				var_response.InitAndSetArrayItem(true, "delMess");
			}
			var_response.InitAndSetArrayItem(this.grabAllJsFiles(), "additionalJS");
			var_response.InitAndSetArrayItem(this.grabAllCSSFiles(), "additionalCSS");
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(var_response)));

			return null;
		}
		public override XVar showPageDp(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			return this.showGridOnly();
		}
		public override XVar prepareDisplayDetails()
		{
			this.prepareDisplayDetailsPD();

			return null;
		}
		public virtual XVar prepareDisplayDetailsPD()
		{
			dynamic bodyContents = null, forms = null;
			this.prepareTemplate();
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.xt.assign(new XVar("embedded_grid"), new XVar(true));
				this.xt.assign(new XVar("embedded_page_title"), new XVar(true));
				this.xt.load_templateJSON((XVar)(this.templatefile));
				this.renderedBody = XVar.Clone(this.xt.fetch_loadedJSON(new XVar("body")));
				return null;
			}
			forms = XVar.Clone(new XVar(0, "grid_block"));
			if(this.masterPageType != Constants.PAGE_ADD)
			{
				forms = XVar.Clone(new XVar(0, "grid_tabs", 1, "grid_block", 2, "below-grid_block"));
			}
			this.renderedButtons = XVar.Clone(this.fetchBlocksList((XVar)(new XVar(0, "firstAboveGridCell"))));
			this.renderedButtons = XVar.Clone(MVCFunctions.str_replace(new XVar("btn-primary"), new XVar("btn-xs btn-info"), (XVar)(this.renderedButtons)));
			this.renderedButtons = XVar.Clone(MVCFunctions.str_replace(new XVar("btn-default"), new XVar("btn-xs btn-info"), (XVar)(this.renderedButtons)));
			bodyContents = XVar.Clone(this.fetchBlocksList((XVar)(forms)));
			this.renderedBody = XVar.Clone(MVCFunctions.Concat("<div id=\"detailPreview", this.id, "\">", bodyContents, "</div>"));

			return null;
		}
		public override XVar showGridOnly()
		{
			MVCFunctions.Echo(this.renderedBody);

			return null;
		}
		public virtual XVar showButtonsDp(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			MVCFunctions.Echo(this.renderedButtons);

			return null;
		}
		public override XVar isPageSortable()
		{
			return this.masterPageType != Constants.PAGE_ADD;
		}
		public override XVar rulePRG()
		{

			return null;
		}
		public override XVar getMasterTableSQLClause()
		{
			if(this.masterPageType == Constants.PAGE_ADD)
			{
				return "1=0";
			}
			return base.getMasterTableSQLClause();
		}
		public override XVar assignButtonsOnMasterEdit(dynamic _param_masterXt)
		{
			#region pass-by-value parameters
			dynamic masterXt = XVar.Clone(_param_masterXt);
			#endregion

			if(XVar.Pack(this.inlineAddAvailable()))
			{
				masterXt.assign((XVar)(MVCFunctions.Concat("details_inlineadd_", this.shortTableName, "_link")), new XVar(true));
				masterXt.assign((XVar)(MVCFunctions.Concat("details_inlineadd_", this.shortTableName, "_attrs")), (XVar)(this.getInlineAddLinksAttrs()));
			}
			if(XVar.Pack(this.addAvailable()))
			{
				masterXt.assign((XVar)(MVCFunctions.Concat("details_add_", this.shortTableName, "_link")), new XVar(true));
				masterXt.assign((XVar)(MVCFunctions.Concat("details_add_", this.shortTableName, "_attrs")), (XVar)(this.getAddLinksAttrs()));
			}
			if(XVar.Pack(this.deleteAvailable()))
			{
				masterXt.assign((XVar)(MVCFunctions.Concat("details_delete_", this.shortTableName, "_link")), new XVar(true));
				masterXt.assign((XVar)(MVCFunctions.Concat("details_delete_", this.shortTableName, "_attrs")), (XVar)(this.getDeleteLinksAttrs()));
			}
			if(XVar.Pack(this.inlineEditAvailable()))
			{
				masterXt.assign((XVar)(MVCFunctions.Concat("details_edit_", this.shortTableName, "_link")), new XVar(true));
				masterXt.assign((XVar)(MVCFunctions.Concat("details_edit_", this.shortTableName, "_attrs")), (XVar)(this.getEditLinksAttrs()));
			}
			if(XVar.Pack(this.updateSelectedAvailable()))
			{
				masterXt.assign((XVar)(MVCFunctions.Concat("details_updateselected_", this.shortTableName, "_link")), new XVar(true));
				masterXt.assign((XVar)(MVCFunctions.Concat("details_updateselected_", this.shortTableName, "_attrs")), (XVar)(this.getUpdateSelectedAttrs()));
			}
			if((XVar)(this.inlineAddAvailable())  || (XVar)(this.inlineEditAvailable()))
			{
				masterXt.assign((XVar)(MVCFunctions.Concat("cancelall_", this.shortTableName, "_link")), new XVar(true));
				masterXt.assign((XVar)(MVCFunctions.Concat("cancelalllink_", this.shortTableName, "_span")), (XVar)(this.buttonShowHideStyle(new XVar("cancelall"))));
				masterXt.assign((XVar)(MVCFunctions.Concat("cancelalllink_", this.shortTableName, "_attrs")), (XVar)(MVCFunctions.Concat("name=\"revertall_edited", this.id, "\" id=\"revertall_edited", this.id, "\"")));
			}
			if((XVar)(this.masterPageType == Constants.PAGE_EDIT)  && (XVar)((XVar)(this.inlineAddAvailable())  || (XVar)(this.inlineEditAvailable())))
			{
				masterXt.assign((XVar)(MVCFunctions.Concat("saveall_", this.shortTableName, "_link")), new XVar(true));
				masterXt.assign((XVar)(MVCFunctions.Concat("savealllink_", this.shortTableName, "_span")), (XVar)(this.buttonShowHideStyle(new XVar("saveall"))));
				masterXt.assign((XVar)(MVCFunctions.Concat("savealllink_", this.shortTableName, "_attrs")), (XVar)(MVCFunctions.Concat("name=\"saveall_edited", this.id, "\" id=\"saveall_edited", this.id, "\"")));
			}

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
			settings.InitAndSetArrayItem(this.masterPageType, "masterPageType");
			settings.InitAndSetArrayItem(this.masterTable, "masterTable");
			settings.InitAndSetArrayItem(this.firstTime, "firstTime");
			settings.InitAndSetArrayItem(this.getStrMasterKey(), "strKey");
			if((XVar)(this.masterPageType == Constants.PAGE_ADD)  && (XVar)(base.spreadsheetGridApplicable()))
			{
				settings.InitAndSetArrayItem(this.pSet.addNewRecordAutomatically(), "autoAddNewRecord");
			}
			return settings;
		}
		public override XVar deleteAvailable()
		{
			return (XVar)((XVar)(this.masterPageType != Constants.PAGE_VIEW)  && (XVar)(this.masterPageType != Constants.PAGE_ADD))  && (XVar)(base.deleteAvailable());
		}
		public override XVar importAvailable()
		{
			return false;
		}
		public override XVar editAvailable()
		{
			return (XVar)((XVar)(this.masterPageType != Constants.PAGE_VIEW)  && (XVar)(this.masterPageType != Constants.PAGE_ADD))  && (XVar)(base.editAvailable());
		}
		public override XVar addAvailable()
		{
			return (XVar)((XVar)(this.masterPageType != Constants.PAGE_VIEW)  && (XVar)(this.masterPageType != Constants.PAGE_ADD))  && (XVar)(base.addAvailable());
		}
		public override XVar copyAvailable()
		{
			return (XVar)((XVar)(this.masterPageType != Constants.PAGE_VIEW)  && (XVar)(this.masterPageType != Constants.PAGE_ADD))  && (XVar)(base.copyAvailable());
		}
		public override XVar inlineEditAvailable()
		{
			return (XVar)((XVar)(this.masterPageType != Constants.PAGE_VIEW)  && (XVar)(this.masterPageType != Constants.PAGE_ADD))  && (XVar)(base.inlineEditAvailable());
		}
		public override XVar inlineAddAvailable()
		{
			return (XVar)((XVar)(this.masterPageType != Constants.PAGE_VIEW)  && (XVar)(base.inlineAddAvailable()))  || (XVar)((XVar)(this.masterPageType == Constants.PAGE_ADD)  && (XVar)(base.addAvailable()));
		}
		protected override XVar displayViewLink()
		{
			return (XVar)(this.masterPageType != Constants.PAGE_ADD)  && (XVar)(this.viewAvailable());
		}
		public override XVar updateSelectedAvailable()
		{
			return (XVar)((XVar)(this.masterPageType != Constants.PAGE_VIEW)  && (XVar)(this.masterPageType != Constants.PAGE_ADD))  && (XVar)(base.updateSelectedAvailable());
		}
		public override XVar isDispGrid()
		{
			if((XVar)(this.inlineAddAvailable())  || (XVar)(this.addAvailable()))
			{
				return true;
			}
			return base.isDispGrid();
		}
		public override XVar shouldDisplayDetailsPage()
		{
			return (XVar)(this.isDispGrid())  || (XVar)(0 != this.getGridTabsCount());
		}
		public override XVar pdfJsonMode()
		{
			return this.mode == Constants.LIST_PDFJSON;
		}
		protected override XVar spreadsheetGridApplicable()
		{
			return (XVar)((XVar)(this.masterPageType != Constants.PAGE_ADD)  && (XVar)(this.masterPageType != Constants.PAGE_VIEW))  && (XVar)(base.spreadsheetGridApplicable());
		}
		protected override XVar assignSessionPrefix()
		{
			dynamic masterKeys = null;
			masterKeys = XVar.Clone(MVCFunctions.md5((XVar)(MVCFunctions.implode(new XVar("-"), (XVar)(this.masterKeysReq)))));
			this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.tName, "_preview", masterKeys));

			return null;
		}
		public override XVar buildSearchPanel()
		{

			return null;
		}
	}
}
