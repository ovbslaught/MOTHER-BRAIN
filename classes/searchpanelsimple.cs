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
	public partial class SearchPanelSimple : XClass
	{
		protected dynamic pageObj = XVar.Pack(null);
		protected dynamic searchClauseObj = XVar.Pack(null);
		protected dynamic tName = XVar.Pack("");
		protected ProjectSettings pSet = null;
		protected dynamic id = XVar.Pack(1);
		protected XTempl xt;
		protected dynamic controlBuilder = XVar.Pack(null);
		protected dynamic panelSearchFields = XVar.Array();
		protected dynamic allSearchFields = XVar.Array();
		public SearchPanelSimple(dynamic var_params)
		{
			this.pageObj = XVar.Clone(var_params["pageObj"]);
			this.searchClauseObj = this.pageObj.searchClauseObj;
			this.id = XVar.Clone(this.pageObj.id);
			this.pSet = XVar.UnPackProjectSettings(this.pageObj.pSetSearch);
			this.tName = XVar.Clone(this.pageObj.searchTableName);
			this.xt = XVar.UnPackXTempl(this.pageObj.xt);
			if(XVar.Pack(!(XVar)(var_params.KeyExists("panelSearchFields"))))
			{
				this.panelSearchFields = XVar.Clone(this.pSet.getPanelSearchFields());
			}
			else
			{
				this.panelSearchFields = XVar.Clone(var_params["panelSearchFields"]);
			}
			if(XVar.Pack(!(XVar)(var_params.KeyExists("allSearchFields"))))
			{
				this.allSearchFields = XVar.Clone(this.pSet.getAllSearchFields());
			}
			else
			{
				this.allSearchFields = XVar.Clone(var_params["allSearchFields"]);
			}
			this.controlBuilder = XVar.Clone(new PanelSearchControl((XVar)(this.id), (XVar)(this.tName), (XVar)(this.searchClauseObj), (XVar)(this.pageObj)));
		}
		public virtual XVar buildSearchPanel()
		{
			if(XVar.Pack(this.pSet.showSearchPanel()))
			{
				this.displaySearchPanel();
			}

			return null;
		}
		public virtual XVar displaySearchPanel()
		{
			dynamic radio = XVar.Array(), showHideOpt_mess = null, showallbutton_attrs = null, srchPanelAttrs = XVar.Array();
			this.xt.assign(new XVar("id"), (XVar)(this.id));
			this.xt.assign(new XVar("searchPanel"), new XVar(true));
			this.xt.assign(new XVar("searchPanelTopButtons"), new XVar(true));
			if(XVar.Pack(!(XVar)(this.pSet.isFlexibleSearch())))
			{
				this.xt.assign(new XVar("controls_block_class"), new XVar("flexibleSearchPanel"));
			}
			radio = XVar.Clone(this.controlBuilder.getSearchRadio());
			this.xt.assign_section(new XVar("all_checkbox_label"), (XVar)(radio["all_checkbox_label"][0]), (XVar)(radio["all_checkbox_label"][1]));
			this.xt.assign_section(new XVar("any_checkbox_label"), (XVar)(radio["any_checkbox_label"][0]), (XVar)(radio["any_checkbox_label"][1]));
			this.xt.assignbyref(new XVar("all_checkbox"), (XVar)(radio["all_checkbox"]));
			this.xt.assignbyref(new XVar("any_checkbox"), (XVar)(radio["any_checkbox"]));
			srchPanelAttrs = XVar.Clone(this.searchClauseObj.getSrchPanelAttrs());
			showHideOpt_mess = XVar.Clone((XVar.Pack(srchPanelAttrs["ctrlTypeComboStatus"]) ? XVar.Pack(CommonFunctions.mlang_message(new XVar("SEARCH_HIDE_OPTIONS_BUTTON"))) : XVar.Pack(CommonFunctions.mlang_message(new XVar("SEARCH_SHOW_OPTIONS_BUTTON")))));
			this.xt.assign(new XVar("showHideOpt_mess"), (XVar)(showHideOpt_mess));
			this.xt.assign(new XVar("showHideCtrlsOpt_attrs"), new XVar("style=\"display: none;\""));
			if(this.searchClauseObj.getUsedCtrlsCount() <= 0)
			{
				this.xt.assign(new XVar("bottomSearchButt_attrs"), new XVar("style=\"display: none;\""));
			}
			this.assignSearchBlocks();
			this.pageObj.controlsMap.InitAndSetArrayItem(this.searchClauseObj.isSearchPanelByUserApiRun(), "search", "searchPanelRunByUserApi");
			showallbutton_attrs = XVar.Clone(MVCFunctions.Concat("id=\"showAll", this.id, "\""));
			if(XVar.Pack(!(XVar)(this.searchClauseObj.isShowAll())))
			{
				dynamic dispNoneStyle = null;
				dispNoneStyle = new XVar("style=\"display: none;\"");
				showallbutton_attrs = MVCFunctions.Concat(showallbutton_attrs, " ", dispNoneStyle);
				this.xt.assign(new XVar("showAllCont_attrs"), (XVar)(dispNoneStyle));
			}
			this.xt.assign(new XVar("showallbutton_attrs"), (XVar)(showallbutton_attrs));

			return null;
		}
		public virtual XVar assignSearchBlocks()
		{
			dynamic defaultValue = null, notAddedFileds = XVar.Array(), otherFieldsBlocks = XVar.Array(), recId = null, searchPanelFieldsBlocks = XVar.Array(), srchCtrlBlocksNumber = null;
			searchPanelFieldsBlocks = XVar.Clone(XVar.Array());
			otherFieldsBlocks = XVar.Clone(XVar.Array());
			notAddedFileds = XVar.Clone(XVar.Array());
			srchCtrlBlocksNumber = new XVar(0);
			recId = XVar.Clone(this.pageObj.genId());
			foreach (KeyValuePair<XVar, dynamic> searchField in this.allSearchFields.GetEnumerator())
			{
				dynamic isSrchPanelField = null, srchFields = XVar.Array();
				srchFields = XVar.Clone(this.searchClauseObj.getSearchCtrlParams((XVar)(searchField.Value)));
				isSrchPanelField = XVar.Clone(MVCFunctions.in_array((XVar)(searchField.Value), (XVar)(this.panelSearchFields)));
				if(XVar.Pack(!(XVar)(srchFields)))
				{
					defaultValue = XVar.Clone(this.pSet.getSearchDefaultValue((XVar)(searchField.Value)));
					if(XVar.Pack(isSrchPanelField))
					{
						dynamic opt = null;
						opt = new XVar("");
						if(XVar.Pack(!(XVar)(this.pSet.isFlexibleSearch())))
						{
							opt = XVar.Clone(this.pSet.getDefaultSearchOption((XVar)(searchField.Value)));
						}
						srchFields.InitAndSetArrayItem(new XVar("opt", opt, "not", "", "value1", defaultValue, "value2", ""), null);
					}
				}
				if(XVar.Pack(srchFields))
				{
					if(XVar.Pack(isSrchPanelField))
					{
						srchFields.InitAndSetArrayItem(true, MVCFunctions.count(srchFields) - 1, "immutable");
					}
					foreach (KeyValuePair<XVar, dynamic> srchField in srchFields.GetEnumerator())
					{
						dynamic block = null;
						block = XVar.Clone(this.controlBuilder.buildSearchCtrlBlockArr((XVar)(recId), (XVar)(searchField.Value), new XVar(0), (XVar)(srchField.Value["opt"]), (XVar)(srchField.Value["not"]), new XVar(false), (XVar)(srchField.Value["value1"]), (XVar)(srchField.Value["value2"]), (XVar)(isSrchPanelField), (XVar)(this.pSet.isFlexibleSearch()), (XVar)(srchField.Value["immutable"])));
						if(XVar.Pack(isSrchPanelField))
						{
							searchPanelFieldsBlocks.InitAndSetArrayItem(block, searchField.Value, null);
						}
						else
						{
							otherFieldsBlocks.InitAndSetArrayItem(block, null);
						}
						srchCtrlBlocksNumber++;
						this.addSearchFieldToControlsMap((XVar)(searchField.Value), ref recId);
					}
				}
				else
				{
					notAddedFileds.InitAndSetArrayItem(searchField.Value, null);
				}
			}
			foreach (KeyValuePair<XVar, dynamic> namedBlocks in searchPanelFieldsBlocks.GetEnumerator())
			{
				this.xt.assign_loopsection_byValue((XVar)(MVCFunctions.Concat("searchCtrlBlock_", MVCFunctions.GoodFieldName((XVar)(namedBlocks.Key)))), (XVar)(namedBlocks.Value));
			}
			if(XVar.Pack(!(XVar)(this.pSet.isFlexibleSearch())))
			{
				return null;
			}
			if((XVar)(XVar.Pack(0) < srchCtrlBlocksNumber)  && (XVar)(srchCtrlBlocksNumber < GlobalVars.gLoadSearchControls))
			{
				dynamic otherSearchControlsMaxNumber = null;
				otherSearchControlsMaxNumber = XVar.Clone((GlobalVars.gLoadSearchControls - srchCtrlBlocksNumber) + MVCFunctions.count(otherFieldsBlocks));
				foreach (KeyValuePair<XVar, dynamic> searchField in notAddedFileds.GetEnumerator())
				{
					defaultValue = XVar.Clone(this.pSet.getSearchDefaultValue((XVar)(searchField.Value)));
					otherFieldsBlocks.InitAndSetArrayItem(this.controlBuilder.buildSearchCtrlBlockArr((XVar)(recId), (XVar)(searchField.Value), new XVar(0), new XVar(""), new XVar(false), new XVar(true), (XVar)(defaultValue), new XVar("")), null);
					this.addSearchFieldToControlsMap((XVar)(searchField.Value), ref recId);
					if(otherSearchControlsMaxNumber <= MVCFunctions.count(otherFieldsBlocks))
					{
						break;
					}
				}
			}
			this.xt.assign_loopsection(new XVar("searchCtrlBlock"), (XVar)(otherFieldsBlocks));

			return null;
		}
		public virtual XVar addSearchFieldToControlsMap(dynamic _param_fName, ref dynamic recId)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic ctrlInd = null, isFieldNeedSecCtrl = null, searchBlock = XVar.Array();
			isFieldNeedSecCtrl = XVar.Clone(this.controlBuilder.isNeedSecondCtrl((XVar)(fName)));
			searchBlock = XVar.Clone(new XVar("fName", fName, "recId", recId));
			ctrlInd = new XVar(0);
			searchBlock.InitAndSetArrayItem(ctrlInd, "ctrlsMap", 0);
			if(XVar.Pack(isFieldNeedSecCtrl))
			{
				searchBlock.InitAndSetArrayItem(ctrlInd + 1, "ctrlsMap", 1);
			}
			if(XVar.Pack(!(XVar)(this.pSet.isFlexibleSearch())))
			{
				searchBlock.InitAndSetArrayItem(this.pSet.getSearchOptionsList((XVar)(fName)), "inflexSearchOption");
			}
			this.pageObj.controlsMap.InitAndSetArrayItem(searchBlock, "search", "searchBlocks", null);
			recId = XVar.Clone(this.pageObj.genId());

			return null;
		}
		public virtual XVar refineOpenFilters(dynamic _param_openFilters)
		{
			#region pass-by-value parameters
			dynamic openFilters = XVar.Clone(_param_openFilters);
			#endregion

			dynamic openFiltersRefined = XVar.Array();
			openFiltersRefined = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> panelFiled in this.panelSearchFields.GetEnumerator())
			{
				dynamic key = null;
				key = XVar.Clone(MVCFunctions.array_search((XVar)(panelFiled.Value), (XVar)(openFilters)));
				if(!XVar.Equals(XVar.Pack(key), XVar.Pack(false)))
				{
					MVCFunctions.array_splice((XVar)(openFilters), (XVar)(key), new XVar(1));
				}
			}
			foreach (KeyValuePair<XVar, dynamic> field in openFilters.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.in_array((XVar)(field.Value), (XVar)(this.allSearchFields))))
				{
					openFiltersRefined.InitAndSetArrayItem(field.Value, null);
				}
			}
			return openFiltersRefined;
		}
	}
}
