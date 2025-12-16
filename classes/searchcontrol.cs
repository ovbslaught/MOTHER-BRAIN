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
	public partial class SearchControl : XClass
	{
		public dynamic tName = XVar.Pack("");
		public dynamic globSrchParams = XVar.Array();
		public dynamic getSrchPanelAttrs = XVar.Array();
		public dynamic dispNoneStyle = XVar.Pack("style=\"display: none;\"");
		public dynamic pageObj = XVar.Pack(null);
		public ProjectSettings pSet = null;
		public dynamic searchClauseObj = XVar.Pack(false);
		public dynamic id = XVar.Pack(1);
		public dynamic controlsContainer;
		public SearchControl(dynamic _param_id, dynamic _param_tName, dynamic searchClauseObj, dynamic pageObj)
		{
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			dynamic tName = XVar.Clone(_param_tName);
			#endregion

			this.tName = XVar.Clone(tName);
			this.searchClauseObj = XVar.Clone(searchClauseObj);
			this.getSrchPanelAttrs = XVar.Clone(this.searchClauseObj.getSrchPanelAttrs());
			this.globSrchParams = XVar.Clone(this.searchClauseObj.getSearchGlobalParams());
			this.id = XVar.Clone(id);
			this.pageObj = pageObj;
			this.pSet = XVar.UnPackProjectSettings((XVar.Pack(this.pageObj.pageType != Constants.PAGE_SEARCH) ? XVar.Pack(new ProjectSettings((XVar)(tName), new XVar(Constants.PAGE_SEARCH))) : XVar.Pack(this.pageObj.pSetSearch)));
			this.controlsContainer = XVar.Clone(new EditControlsContainer((XVar)(this.pageObj), (XVar)(this.pSet), new XVar(Constants.PAGE_SEARCH), (XVar)(this.pageObj.cipherer)));
		}
		public virtual XVar buildCtrlParamsArr(dynamic _param_fName, dynamic _param_recId, dynamic _param_fieldNum, dynamic _param_value, dynamic _param_opt, dynamic _param_renderHidden = null, dynamic _param_isCached = null)
		{
			#region default values
			if(_param_renderHidden as Object == null) _param_renderHidden = new XVar(false);
			if(_param_isCached as Object == null) _param_isCached = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic value = XVar.Clone(_param_value);
			dynamic opt = XVar.Clone(_param_opt);
			dynamic renderHidden = XVar.Clone(_param_renderHidden);
			dynamic isCached = XVar.Clone(_param_isCached);
			#endregion

			dynamic ctrlsMap = null, fType = null, format = null, hidden = null, parameters = XVar.Array();
			hidden = XVar.Clone(renderHidden);
			fType = XVar.Clone(this.pSet.getEditFormat((XVar)(fName)));
			format = XVar.Clone(fType);
			if((XVar)((XVar)((XVar)((XVar)(fType == Constants.EDIT_FORMAT_TEXT_AREA)  || (XVar)(fType == Constants.EDIT_FORMAT_PASSWORD))  || (XVar)(fType == Constants.EDIT_FORMAT_HIDDEN))  || (XVar)(fType == Constants.EDIT_FORMAT_READONLY))  || (XVar)(fType == Constants.EDIT_FORMAT_FILE))
			{
				format = new XVar(Constants.EDIT_FORMAT_TEXT_FIELD);
			}
			parameters = XVar.Clone(XVar.Array());
			parameters.InitAndSetArrayItem(fName, "field");
			parameters.InitAndSetArrayItem("search", "mode");
			parameters.InitAndSetArrayItem(Constants.PAGE_SEARCH, "ptype");
			parameters.InitAndSetArrayItem(recId, "id");
			parameters.InitAndSetArrayItem(fieldNum, "fieldNum");
			parameters.InitAndSetArrayItem(format, "format");
			parameters.InitAndSetArrayItem(this, "pageObj");
			parameters.InitAndSetArrayItem(value, "value");
			parameters.InitAndSetArrayItem(new XVar("hidden", hidden, "option", opt), "additionalCtrlParams");
			ctrlsMap = XVar.Clone(this.getControlMap((XVar)(fName), (XVar)(format), (XVar)(recId), (XVar)(fieldNum), (XVar)(hidden), (XVar)(value)));
			this.pageObj.fillControlsMap((XVar)(ctrlsMap));
			return parameters;
		}
		public virtual XVar getCtrlParamsArr(dynamic _param_fName, dynamic _param_recId, dynamic _param_fieldNum, dynamic _param_value, dynamic _param_opt, dynamic _param_renderHidden = null, dynamic _param_isCached = null)
		{
			#region default values
			if(_param_renderHidden as Object == null) _param_renderHidden = new XVar(false);
			if(_param_isCached as Object == null) _param_isCached = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic value = XVar.Clone(_param_value);
			dynamic opt = XVar.Clone(_param_opt);
			dynamic renderHidden = XVar.Clone(_param_renderHidden);
			dynamic isCached = XVar.Clone(_param_isCached);
			#endregion

			dynamic parameters = null;
			parameters = XVar.Clone(this.buildCtrlParamsArr((XVar)(fName), (XVar)(recId), (XVar)(fieldNum), (XVar)(value), (XVar)(opt), (XVar)(renderHidden), (XVar)(isCached)));
			return XTempl.create_function_assignment(new XVar("xt_buildeditcontrol"), (XVar)(parameters));
		}
		protected virtual XVar getControlMap(dynamic _param_fName, dynamic _param_format, dynamic _param_recId, dynamic _param_fieldNum, dynamic _param_hidden, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic format = XVar.Clone(_param_format);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic hidden = XVar.Clone(_param_hidden);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic ctrlsMap = XVar.Array(), preload = null;
			ctrlsMap = XVar.Clone(new XVar("controls", XVar.Array()));
			ctrlsMap.InitAndSetArrayItem(fName, "controls", "fieldName");
			ctrlsMap.InitAndSetArrayItem(Constants.MODE_SEARCH, "controls", "mode");
			ctrlsMap.InitAndSetArrayItem(format, "controls", "editFormat");
			ctrlsMap.InitAndSetArrayItem(recId, "controls", "id");
			ctrlsMap.InitAndSetArrayItem(fieldNum, "controls", "ctrlInd");
			ctrlsMap.InitAndSetArrayItem(hidden, "controls", "hidden");
			ctrlsMap.InitAndSetArrayItem(this.tName, "controls", "table");
			preload = XVar.Clone(this.pageObj.fillPreload((XVar)(fName), (XVar)(new XVar(0, fName)), (XVar)(new XVar(fName, value)), (XVar)(this.controlsContainer)));
			if(!XVar.Equals(XVar.Pack(preload), XVar.Pack(false)))
			{
				ctrlsMap.InitAndSetArrayItem(preload, "controls", "preloadData");
			}
			return ctrlsMap;
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

			return XVar.UnPackEditControl(this.controlsContainer.getControl((XVar)(field), (XVar)(id)) ?? new XVar());
		}
		public virtual XVar getFieldControlsData()
		{
			return XVar.Array();
		}
		public virtual XVar getSecCtrlParamsArr(dynamic _param_fName, dynamic _param_recId, dynamic _param_fieldNum, dynamic _param_value, dynamic _param_opt, dynamic _param_renderHidden = null, dynamic _param_isCached = null)
		{
			#region default values
			if(_param_renderHidden as Object == null) _param_renderHidden = new XVar(false);
			if(_param_isCached as Object == null) _param_isCached = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic value = XVar.Clone(_param_value);
			dynamic opt = XVar.Clone(_param_opt);
			dynamic renderHidden = XVar.Clone(_param_renderHidden);
			dynamic isCached = XVar.Clone(_param_isCached);
			#endregion

			dynamic fType = null;
			fType = XVar.Clone(this.pSet.getEditFormat((XVar)(fName)));
			if(XVar.Pack(this.isNeedSecondCtrl((XVar)(fName))))
			{
				return this.getCtrlParamsArr((XVar)(fName), (XVar)(recId), (XVar)(fieldNum + 1), (XVar)(value), (XVar)(opt), (XVar)(renderHidden), (XVar)(isCached));
			}
			return false;
		}
		public virtual XVar isNeedSecondCtrl(dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic fType = null, lookupType = null;
			fType = XVar.Clone(this.pSet.getEditFormat((XVar)(fName)));
			lookupType = XVar.Clone(this.pSet.lookupControlType((XVar)(fName)));
			if((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(fType == Constants.EDIT_FORMAT_DATE)  || (XVar)(fType == Constants.EDIT_FORMAT_TIME))  || (XVar)(fType == Constants.EDIT_FORMAT_TEXT_FIELD))  || (XVar)(fType == Constants.EDIT_FORMAT_TEXT_AREA))  || (XVar)(fType == Constants.EDIT_FORMAT_PASSWORD))  || (XVar)(fType == Constants.EDIT_FORMAT_HIDDEN))  || (XVar)(fType == Constants.EDIT_FORMAT_READONLY))  || (XVar)((XVar)(fType == Constants.EDIT_FORMAT_LOOKUP_WIZARD)  && (XVar)(lookupType == Constants.LCT_AJAX)))
			{
				return true;
			}
			return false;
		}
		public virtual XVar getCtrlSearchTypeOptions(dynamic _param_fName, dynamic _param_selOpt, dynamic _param_not, dynamic _param_flexible = null, dynamic _param_both = null)
		{
			#region default values
			if(_param_flexible as Object == null) _param_flexible = new XVar(false);
			if(_param_both as Object == null) _param_both = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic flexible = XVar.Clone(_param_flexible);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			return this.getControl((XVar)(fName)).getSearchOptions((XVar)(selOpt), (XVar)(var_not), (XVar)(both));
		}
		public virtual XVar getCtrlSearchType(dynamic _param_fName, dynamic _param_recId, dynamic _param_fieldNum, dynamic _param_selOpt, dynamic _param_not, dynamic _param_flexible, dynamic _param_both)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic flexible = XVar.Clone(_param_flexible);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			dynamic emptyOption = null, searchtype = null, var_class = null, visibility = null;
			emptyOption = XVar.Clone((XVar)(selOpt == Constants.EMPTY_SEARCH)  || (XVar)(selOpt == Constants.NOT_EMPTY));
			visibility = XVar.Clone((XVar.Pack((XVar)((XVar)(!(XVar)(flexible))  || (XVar)(this.getSrchPanelAttrs["ctrlTypeComboStatus"]))  || (XVar)(emptyOption)) ? XVar.Pack("") : XVar.Pack("style=\"display: none;\"")));
			searchtype = XVar.Clone(MVCFunctions.Concat("<span id=\"", this.getCtrlComboContId((XVar)(recId), (XVar)(fName)), "\" ", visibility, ">"));
			searchtype = MVCFunctions.Concat(searchtype, "<select class=\"form-control\" ", var_class, " id=\"", this.getSearchOptionId((XVar)(fName), (XVar)(recId)), "\" name=\"", this.getSearchOptionId((XVar)(fName), (XVar)(recId)), "\" size=1 ", visibility, ">");
			searchtype = MVCFunctions.Concat(searchtype, this.getCtrlSearchTypeOptions((XVar)(fName), (XVar)(selOpt), (XVar)(var_not), (XVar)(flexible), (XVar)(both)));
			searchtype = MVCFunctions.Concat(searchtype, "</select></span>");
			return searchtype;
		}
		public virtual XVar getSearchOptionId(dynamic _param_fName, dynamic _param_recId)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			#endregion

			return MVCFunctions.Concat("srchOpt_", recId, "_", MVCFunctions.GoodFieldName((XVar)(fName)));
		}
		public virtual XVar getNotBox(dynamic _param_fName, dynamic _param_recId, dynamic _param_not)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic var_not = XVar.Clone(_param_not);
			#endregion

			dynamic notbox = null;
			notbox = XVar.Clone(MVCFunctions.Concat("id=\"not_", recId, "_", MVCFunctions.GoodFieldName((XVar)(fName)), "\""));
			if(XVar.Pack(var_not))
			{
				notbox = MVCFunctions.Concat(notbox, " checked");
			}
			return notbox;
		}
		public virtual XVar getDelButtonHtml(dynamic _param_fName, dynamic _param_recId)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			#endregion

			dynamic html = null, text = null;
			text = new XVar("");
			text = new XVar("<span class=\"glyphicon glyphicon-remove\"></span>");
			html = XVar.Clone(MVCFunctions.Concat("<a id = \"", this.getDelButtonId((XVar)(fName), (XVar)(recId)), "\" ctrlId=\"", recId, "\" fName=\"", MVCFunctions.GoodFieldName((XVar)(fName)), "\" class=\"searchPanelButton\" href=\"#\" title=\"", CommonFunctions.mlang_message(new XVar("DELETE_CONTROL")), "\">", text, "</a>"));
			return html;
		}
		public virtual XVar getDelButtonId(dynamic _param_fName, dynamic _param_recId)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			#endregion

			return MVCFunctions.Concat("delCtrlButt_", recId, "_", MVCFunctions.GoodFieldName((XVar)(fName)));
		}
		public virtual XVar getSearchRadio()
		{
			dynamic id508l = null, id508n = null, resArr = XVar.Array();
			resArr = XVar.Clone(XVar.Array());
			resArr.InitAndSetArrayItem(new XVar(0, "", 1, ""), "all_checkbox_label");
			resArr.InitAndSetArrayItem(new XVar(0, "", 1, ""), "any_checkbox_label");
			if(XVar.Pack(CommonFunctions.isEnableSection508()))
			{
				resArr.InitAndSetArrayItem(new XVar(0, "<label for=\"all_checkbox\">", 1, "</label>"), "all_checkbox_label");
				resArr.InitAndSetArrayItem(new XVar(0, "<label for=\"any_checkbox\">", 1, "</label>"), "any_checkbox_label");
			}
			id508l = new XVar("id=\"all_checkbox\" ");
			id508n = new XVar("id=\"any_checkbox\" ");
			resArr.InitAndSetArrayItem(id508l, "all_checkbox");
			resArr.InitAndSetArrayItem(id508n, "any_checkbox");
			resArr["all_checkbox"] = MVCFunctions.Concat(resArr["all_checkbox"], "value=\"and\" ");
			resArr["any_checkbox"] = MVCFunctions.Concat(resArr["any_checkbox"], "value=\"or\" ");
			if((XVar)(this.globSrchParams.KeyExists("srchTypeRadio"))  && (XVar)(this.globSrchParams["srchTypeRadio"] == "or"))
			{
				resArr["any_checkbox"] = MVCFunctions.Concat(resArr["any_checkbox"], " checked");
			}
			else
			{
				resArr["all_checkbox"] = MVCFunctions.Concat(resArr["all_checkbox"], " checked");
			}
			return resArr;
		}
		public virtual XVar getFilterRowId(dynamic _param_recId, dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic recId = XVar.Clone(_param_recId);
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			return MVCFunctions.Concat("filter_", recId, "_", MVCFunctions.GoodFieldName((XVar)(fName)));
		}
		public virtual XVar getCtrlComboContId(dynamic _param_recId, dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic recId = XVar.Clone(_param_recId);
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			return MVCFunctions.Concat("searchType_", recId, "_", MVCFunctions.GoodFieldName((XVar)(fName)));
		}
		public virtual XVar buildSearchCtrlBlockArr(dynamic _param_recId, dynamic _param_fName, dynamic _param_ctrlInd, dynamic _param_opt, dynamic _param_not, dynamic _param_isChached, dynamic _param_val1, dynamic _param_val2, dynamic _param_panelField = null, dynamic _param_flexible = null, dynamic _param_immutable = null, dynamic _param_both = null)
		{
			#region default values
			if(_param_panelField as Object == null) _param_panelField = new XVar(false);
			if(_param_flexible as Object == null) _param_flexible = new XVar(true);
			if(_param_immutable as Object == null) _param_immutable = new XVar(false);
			if(_param_both as Object == null) _param_both = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic recId = XVar.Clone(_param_recId);
			dynamic fName = XVar.Clone(_param_fName);
			dynamic ctrlInd = XVar.Clone(_param_ctrlInd);
			dynamic opt = XVar.Clone(_param_opt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic isChached = XVar.Clone(_param_isChached);
			dynamic val1 = XVar.Clone(_param_val1);
			dynamic val2 = XVar.Clone(_param_val2);
			dynamic panelField = XVar.Clone(_param_panelField);
			dynamic flexible = XVar.Clone(_param_flexible);
			dynamic immutable = XVar.Clone(_param_immutable);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			dynamic filterRowAttrs = null, filterRowId = null, postfix = null, renderHidden = null, srchCtrlBlock = XVar.Array();
			srchCtrlBlock = XVar.Clone(XVar.Array());
			postfix = new XVar("");
			if(XVar.Pack(panelField))
			{
				postfix = XVar.Clone(MVCFunctions.Concat("_", MVCFunctions.GoodFieldName((XVar)(fName))));
			}
			if(opt == XVar.Pack(""))
			{
				opt = XVar.Clone(this.pSet.getDefaultSearchOption((XVar)(fName)));
			}
			renderHidden = XVar.Clone((XVar)(MVCFunctions.strtolower((XVar)(opt)) == "empty")  || (XVar)(MVCFunctions.strtolower((XVar)(opt)) == "not empty"));
			srchCtrlBlock.InitAndSetArrayItem(this.getCtrlParamsArr((XVar)(fName), (XVar)(recId), (XVar)(ctrlInd), (XVar)(val1), (XVar)(opt), (XVar)(renderHidden), (XVar)(isChached)), MVCFunctions.Concat("searchcontrol", postfix));
			renderHidden = XVar.Clone((XVar)(MVCFunctions.strtolower((XVar)(opt)) != "between")  && (XVar)(MVCFunctions.strtolower((XVar)(opt)) != "not between"));
			if((XVar)(flexible)  || (XVar)(!(XVar)(renderHidden)))
			{
				srchCtrlBlock.InitAndSetArrayItem(this.getSecCtrlParamsArr((XVar)(fName), (XVar)(recId), (XVar)(ctrlInd), (XVar)(val2), (XVar)(opt), (XVar)(renderHidden), (XVar)(isChached)), MVCFunctions.Concat("searchcontrol1", postfix));
			}
			if(XVar.Pack(!(XVar)(immutable)))
			{
				srchCtrlBlock.InitAndSetArrayItem(this.getDelButtonHtml((XVar)(fName), (XVar)(recId)), "delCtrlButt");
			}
			filterRowId = XVar.Clone(this.getFilterRowId((XVar)(recId), (XVar)(fName)));
			filterRowAttrs = XVar.Clone(MVCFunctions.Concat("id=\"", filterRowId, "\" "));
			if(XVar.Pack(isChached))
			{
				filterRowAttrs = MVCFunctions.Concat(filterRowAttrs, this.dispNoneStyle);
			}
			srchCtrlBlock.InitAndSetArrayItem(filterRowAttrs, MVCFunctions.Concat("filterRow_attrs", postfix));
			if(XVar.Pack(immutable))
			{
				srchCtrlBlock.InitAndSetArrayItem("rnr-basic-search-field", MVCFunctions.Concat("filterRow_class", postfix));
			}
			srchCtrlBlock.InitAndSetArrayItem(fName, "fName");
			srchCtrlBlock.InitAndSetArrayItem(this.getCtrlSearchType((XVar)(fName), (XVar)(recId), (XVar)(ctrlInd), (XVar)(opt), (XVar)(var_not), (XVar)(flexible), (XVar)(both)), MVCFunctions.Concat("searchtype", postfix));
			srchCtrlBlock.InitAndSetArrayItem(this.getNotBox((XVar)(fName), (XVar)(recId), (XVar)(var_not)), "notbox");
			srchCtrlBlock.InitAndSetArrayItem(CommonFunctions.GetFieldLabel((XVar)(MVCFunctions.GoodFieldName((XVar)(this.tName))), (XVar)(MVCFunctions.GoodFieldName((XVar)(fName)))), MVCFunctions.Concat("fLabel", postfix));
			return srchCtrlBlock;
		}
		public static XVar getSimpleSearchTypeCombo(dynamic _param_selOpt, dynamic _param_not, dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic options = null;
			options = XVar.Clone(MVCFunctions.Concat("<option value=\"Contains\" ", (XVar.Pack((XVar)(selOpt == "Contains")  && (XVar)(!(XVar)(var_not))) ? XVar.Pack("selected") : XVar.Pack("")), ">", CommonFunctions.mlang_message(new XVar("CONTAINS")), "</option>"));
			options = MVCFunctions.Concat(options, "<option value=\"Equals\" ", (XVar.Pack((XVar)(selOpt == "Equals")  && (XVar)(!(XVar)(var_not))) ? XVar.Pack("selected") : XVar.Pack("")), ">", CommonFunctions.mlang_message(new XVar("EQUALS")), "</option>");
			options = MVCFunctions.Concat(options, "<option value=\"Starts with\" ", (XVar.Pack((XVar)(selOpt == "Starts with")  && (XVar)(!(XVar)(var_not))) ? XVar.Pack("selected") : XVar.Pack("")), ">", CommonFunctions.mlang_message(new XVar("STARTS_WITH")), "</option>");
			options = MVCFunctions.Concat(options, "<option value=\"More than\" ", (XVar.Pack((XVar)(selOpt == "More than")  && (XVar)(!(XVar)(var_not))) ? XVar.Pack("selected") : XVar.Pack("")), ">", CommonFunctions.mlang_message(new XVar("MORE_THAN")), "</option>");
			options = MVCFunctions.Concat(options, "<option value=\"Less than\" ", (XVar.Pack((XVar)(selOpt == "Less than")  && (XVar)(!(XVar)(var_not))) ? XVar.Pack("selected") : XVar.Pack("")), ">", CommonFunctions.mlang_message(new XVar("LESS_THAN")), "</option>");
			options = MVCFunctions.Concat(options, "<option value=\"Empty\" ", (XVar.Pack((XVar)(selOpt == "Empty")  && (XVar)(!(XVar)(var_not))) ? XVar.Pack("selected") : XVar.Pack("")), ">", CommonFunctions.mlang_message(new XVar("EMPTY")), "</option>");
			return MVCFunctions.Concat("<select class=\"form-control\" id=\"simpleSrchTypeCombo", id, "\" name=\"simpleSrchTypeCombo", id, "\" size=\"1\">", options, "</select>");
		}
		public static XVar simpleSearchFieldCombo(dynamic _param_fNamesArr, dynamic _param_googleLikeFields, dynamic _param_selOpt, dynamic _param_tName, dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic fNamesArr = XVar.Clone(_param_fNamesArr);
			dynamic googleLikeFields = XVar.Clone(_param_googleLikeFields);
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic tName = XVar.Clone(_param_tName);
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic options = null;
			options = new XVar("");
			if(XVar.Pack(googleLikeFields))
			{
				options = XVar.Clone(MVCFunctions.Concat("<option value=\"\" >", CommonFunctions.mlang_message(new XVar("ANY_FIELD")), "</option>"));
			}
			foreach (KeyValuePair<XVar, dynamic> fName in fNamesArr.GetEnumerator())
			{
				dynamic fLabel = null;
				fLabel = XVar.Clone(CommonFunctions.GetFieldLabel((XVar)(MVCFunctions.GoodFieldName((XVar)(tName))), (XVar)(MVCFunctions.GoodFieldName((XVar)(fName.Value)))));
				options = MVCFunctions.Concat(options, "<option value=\"", fName.Value, "\" ", (XVar.Pack(selOpt == fName.Value) ? XVar.Pack("selected") : XVar.Pack("")), ">", fLabel, "</option>");
			}
			return MVCFunctions.Concat("<select class=\"form-control\" id=\"simpleSrchFieldsCombo", id, "\" name=\"simpleSrchFieldsCombo", id, "\" size=\"1\">", options, "</select>");
		}
	}
}
