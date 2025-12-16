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
	public partial class ListPage_Simple : ListPage
	{
		protected static bool skipListPage_SimpleCtor = false;
		public ListPage_Simple(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipListPage_SimpleCtor)
			{
				skipListPage_SimpleCtor = false;
				return;
			}
			this.pSetEdit = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_SEARCH)));
		}
		public override XVar commonAssign()
		{
			base.commonAssign();
			this.importLinksAttrs();
			this.xt.assign(new XVar("left_block"), new XVar(true));
			this.addAssignTopLinks();
			this.addAssignPageDetails();
			this.xt.assign(new XVar("moreButtons"), (XVar)((XVar)((XVar)((XVar)(this.exportAvailable())  || (XVar)(this.printAvailable()))  || (XVar)(this.importAvailable()))  || (XVar)(this.advSearchAvailable())));
			this.xt.assign(new XVar("withSelected"), (XVar)((XVar)((XVar)((XVar)(this.exportAvailable())  || (XVar)(this.printAvailable()))  || (XVar)(this.inlineEditAvailable()))  || (XVar)(this.deleteAvailable())));
			if(XVar.Pack(this.exportAvailable()))
			{
				this.xt.assign(new XVar("exportselected_link"), new XVar(true));
				this.xt.assign(new XVar("exportselectedlink_span"), (XVar)(this.buttonShowHideStyle()));
				this.xt.assign(new XVar("exportselectedlink_attrs"), (XVar)(this.getPrintExportLinkAttrs(new XVar("export"))));
			}
			if(XVar.Pack(this.pSet.isAllowShowHideFields()))
			{
				this.xt.assign(new XVar("field_hide_panel"), new XVar(true));
			}
			if(XVar.Pack(this.printAvailable()))
			{
				if(XVar.Pack(!(XVar)(this.recordsOnPage)))
				{
					this.hideItemType(new XVar("print_panel"));
				}
				this.xt.assign(new XVar("print_friendly"), new XVar(true));
				this.xt.assign(new XVar("print_friendly_all"), new XVar(true));
				this.xt.assign(new XVar("print_recspp"), (XVar)(this.pSet.getPrinterSplitRecords()));
				foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
				{
					dynamic dPset = null;
					dPset = XVar.Clone(new ProjectSettings((XVar)(details.Value)));
					if((XVar)(dPset.hasPrintPage())  && (XVar)(this.permis[details.Value]["export"]))
					{
						this.xt.assign(new XVar("print_details"), new XVar(true));
						this.xt.assign((XVar)(MVCFunctions.Concat("print_details_", CommonFunctions.GetTableURL((XVar)(details.Value)))), new XVar(true));
					}
				}
				this.xt.assign(new XVar("printselected_link"), new XVar(true));
				this.xt.assign(new XVar("printselectedlink_attrs"), (XVar)(this.getPrintExportLinkAttrs(new XVar("print"))));
				this.xt.assign(new XVar("printselectedlink_span"), (XVar)(this.buttonShowHideStyle()));
			}
			this.xt.assign(new XVar("advsearchlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"advButton", this.id, "\"")));
			this.xt.assign(new XVar("menu_block"), (XVar)((XVar)(this.isShowMenu())  || (XVar)(this.isAdminTable())));

			return null;
		}
		protected override XVar setGridUserParams()
		{
			if(XVar.Pack(this.pSet.isAllowShowHideFields()))
			{
				dynamic fieldsClasses = null, hideColumns = XVar.Array();
				hideColumns = XVar.Clone(this.getColumnsToHide());
				if(XVar.Pack(!(XVar)(this.recordsOnPage)))
				{
					this.hideItemType(new XVar("columns_control"));
				}
				fieldsClasses = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> fields in hideColumns.GetEnumerator())
				{
					dynamic dm = null;
					dm = XVar.Clone(RunnerPage.deviceClassToMacro((XVar)(fields.Key)));
					if((XVar)((XVar)(CommonFunctions.getMediaType() == 0)  && (XVar)(dm == XVar.Pack(0)))  || (XVar)((XVar)((XVar)(CommonFunctions.getMediaType() == 2)  || (XVar)(CommonFunctions.getMediaType() == 1))  && (XVar)(dm == 2)))
					{
						foreach (KeyValuePair<XVar, dynamic> gf in fields.Value.GetEnumerator())
						{
							dynamic fieldName = null;
							fieldName = XVar.Clone(this.pSet.getFieldByGoodFieldName((XVar)(gf.Value)));
							this.hideField((XVar)(fieldName));
						}
					}
				}
			}

			return null;
		}
		public override XVar getBreadcrumbMenuId()
		{
			if(XVar.Pack(this.isAdminTable()))
			{
				return "adminarea";
			}
			else
			{
				return "main";
			}

			return null;
		}
		public virtual XVar addAssignTopLinks()
		{
			if((XVar)(!(XVar)(this.isDispGrid()))  && (XVar)(!(XVar)(this.pSetEdit.ajaxBasedListPage())))
			{
				return null;
			}
			if(XVar.Pack(this.printAvailable()))
			{
				dynamic needShowLinkForAdd = null;
				this.xt.assign(new XVar("prints_block"), new XVar(true));
				this.xt.assign(new XVar("print_link"), (XVar)(this.recordsOnPage));
				this.xt.assign(new XVar("printlink_attrs"), (XVar)(MVCFunctions.Concat("id='print_", this.id, "' name='print_", this.id, "'", (XVar.Pack((XVar)(!(XVar)(this.recordsOnPage))  && (XVar)(needShowLinkForAdd)) ? XVar.Pack(" style='display:none;'") : XVar.Pack("")))));
				this.xt.assign(new XVar("printall_link"), new XVar(true));
				this.xt.assign(new XVar("printalllink_attrs"), (XVar)(MVCFunctions.Concat("id='printAll_", this.id, "' name='printAll_", this.id, "'", (XVar.Pack(!(XVar)(this.recordsOnPage)) ? XVar.Pack(" style='display:none;'") : XVar.Pack("")))));
			}
			if(XVar.Pack(this.exportAvailable()))
			{
				this.xt.assign(new XVar("export_link"), new XVar(true));
				this.xt.assign(new XVar("exportlink_attrs"), (XVar)(MVCFunctions.Concat("id='export_", this.id, "'", (XVar.Pack(!(XVar)(this.recordsOnPage)) ? XVar.Pack(" style='display:none;'") : XVar.Pack("")))));
			}

			return null;
		}
		public virtual XVar addAssignPageDetails()
		{
			dynamic searchPermis = null;
			searchPermis = XVar.Clone(this.permis[this.tName]["search"]);
			if((XVar)((XVar)(!(XVar)(this.recordsOnPage))  && (XVar)(!(XVar)(this.inlineAddAvailable())))  && (XVar)(!(XVar)(this.showAddInPopup)))
			{
				return null;
			}
			this.xt.assign(new XVar("details_block"), (XVar)(searchPermis));
			this.xt.assign(new XVar("pages_block"), (XVar)(searchPermis));
			this.xt.assign(new XVar("pages_attrs"), (XVar)(MVCFunctions.Concat("id=\"pageOf", this.id, "\" name=\"pageOf", this.id, "\"")));
			if((XVar)(searchPermis)  && (XVar)(MVCFunctions.count(this.arrRecsPerPage)))
			{
				this.xt.assign(new XVar("recordspp_block"), new XVar(true));
				this.createPerPage();
			}

			return null;
		}
		public override XVar addCommonHtml()
		{
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage((XVar)(this.isDisplayLoading)));
			base.addCommonHtml();
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");

			return null;
		}
		public override XVar buildSearchPanel()
		{
			dynamic masterKeys = XVar.Array(), var_params = XVar.Array();
			if(XVar.Pack(!(XVar)(this.permis[this.tName]["search"])))
			{
				return null;
			}
			this.xt.enable_section(new XVar("searchPanel"));
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(this, "pageObj");
			var_params.InitAndSetArrayItem(this.panelSearchFields, "panelSearchFields");
			masterKeys = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
			var_params.InitAndSetArrayItem(MVCFunctions.array_diff((XVar)(this.pSet.getAllSearchFields()), (XVar)(masterKeys["detailsKeys"])), "allSearchFields");
			this.searchPanel = XVar.Clone(new SearchPanelSimple((XVar)(var_params)));
			this.searchPanel.buildSearchPanel();

			return null;
		}
		public override XVar prepareForResizeColumns()
		{
			dynamic columnsData = null, logger = null;
			base.prepareForResizeColumns();
			logger = XVar.Clone(new paramsLogger((XVar)(this.tName), new XVar(Constants.CRESIZE_PARAMS_TYPE)));
			columnsData = XVar.Clone(logger.getData());
			if(XVar.Pack(columnsData))
			{
				this.pageData.InitAndSetArrayItem(columnsData, "resizableColumnsData");
			}

			return null;
		}
		protected override XVar getColumnsToHide()
		{
			return this.getCombinedHiddenColumns();
		}
		protected override XVar prepareEmptyFPMarkup()
		{
			if((XVar)((XVar)(this.listAjax)  && (XVar)(this.pSetEdit.isSearchRequiredForFiltering()))  && (XVar)(!(XVar)(this.isRequiredSearchRunning())))
			{
				this.xt.enable_section(new XVar("filterPanel"));
				this.hideElement(new XVar("filterpanel"));
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
			if(XVar.Pack(this.pSet.isAllowShowHideFields()))
			{
				settings.InitAndSetArrayItem(true, "isAllowShowHideFields");
				settings.InitAndSetArrayItem(this.getColumnsToHide(), "hideColumns");
			}
			if(XVar.Pack(this.reorderFieldsFeatureEnabled()))
			{
				dynamic columnOrder = null, logger = null;
				settings.InitAndSetArrayItem(true, "isAllowFieldsReordering");
				logger = XVar.Clone(new paramsLogger((XVar)(this.tName), new XVar(Constants.FORDER_PARAMS_TYPE)));
				columnOrder = XVar.Clone(logger.getData());
				if(XVar.Pack(columnOrder))
				{
					settings.InitAndSetArrayItem(columnOrder, "columnOrder");
				}
			}
			return settings;
		}
	}
}
