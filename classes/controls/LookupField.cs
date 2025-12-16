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
	public partial class LookupField : EditControl
	{
		public dynamic lookupTable = XVar.Pack("");
		public dynamic lookupType = XVar.Pack(0);
		public dynamic LCType = XVar.Pack(0);
		public dynamic ciphererLookup = XVar.Pack(null);
		public dynamic displayFieldName = XVar.Pack("");
		public dynamic linkFieldName = XVar.Pack("");
		public dynamic linkAndDisplaySame = XVar.Pack(false);
		public dynamic lookupSize = XVar.Pack(1);
		public dynamic multiple = XVar.Pack("");
		public dynamic postfix = XVar.Pack("");
		public dynamic alt = XVar.Pack("");
		public dynamic clookupfield = XVar.Pack("");
		public dynamic bUseCategory = XVar.Pack(false);
		public dynamic horizontalLookup = XVar.Pack(false);
		public dynamic addNewItem = XVar.Pack(false);
		public dynamic editLookupItem = XVar.Pack(false);
		public dynamic isLinkFieldEncrypted = XVar.Pack(false);
		public dynamic isDisplayFieldEncrypted = XVar.Pack(false);
		public dynamic lookupPageType = XVar.Pack("");
		public dynamic lookupPSet = XVar.Pack(null);
		public dynamic multiselect = XVar.Pack(false);
		public dynamic lwLinkField = XVar.Pack("");
		public dynamic lwDisplayFieldWrapped = XVar.Pack("");
		public dynamic customDisplay = XVar.Pack("");
		public dynamic tName = XVar.Pack("");
		private dynamic displayFieldAlias = XVar.Pack("");
		protected dynamic lookupTableAliases = XVar.Array();
		protected dynamic linkFieldAliases = XVar.Array();
		protected dynamic displayFieldAliases = XVar.Array();
		protected dynamic searchByDisplayedFieldIsAllowed = XVar.Pack(null);
		protected dynamic lookupDataSource;
		protected static bool skipLookupFieldCtor = false;
		public LookupField(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection)
		{
			if(skipLookupFieldCtor)
			{
				skipLookupFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.tName = XVar.Clone(this.pageObject.tName);
			if(XVar.Pack(this.pageObject.tableBasedSearchPanelAdded))
			{
				this.tName = XVar.Clone(this.pageObject.searchTableName);
			}
			this.format = new XVar(Constants.EDIT_FORMAT_TEXT_FIELD);
			if((XVar)(pageObject.pageType == Constants.PAGE_LIST)  || (XVar)(!(XVar)(pageObject.isPageTableBased())))
			{
				this.lookupPageType = new XVar(Constants.PAGE_SEARCH);
			}
			else
			{
				this.lookupPageType = XVar.Clone(pageObject.pageType);
			}
			this.lookupTable = XVar.Clone(this.pageObject.pSetEdit.getLookupTable((XVar)(this.field)));
			this.lookupType = XVar.Clone(this.pageObject.pSetEdit.getLookupType((XVar)(this.field)));
			this.lookupDataSource = XVar.Clone(CommonFunctions.getLookupDataSource((XVar)(this.field), (XVar)(this.pageObject.pSetEdit)));
			if(this.lookupType == Constants.LT_QUERY)
			{
				this.lookupPSet = XVar.Clone(new ProjectSettings((XVar)(this.lookupTable)));
			}
			this.displayFieldName = XVar.Clone(this.pageObject.pSetEdit.getDisplayField((XVar)(this.field)));
			this.linkFieldName = XVar.Clone(this.pageObject.pSetEdit.getLinkField((XVar)(this.field)));
			this.linkAndDisplaySame = XVar.Clone(this.displayFieldName == this.linkFieldName);
			if(this.lookupType == Constants.LT_QUERY)
			{
				this.ciphererLookup = XVar.Clone(new RunnerCipherer((XVar)(this.lookupTable)));
			}
			this.LCType = XVar.Clone(this.pageObject.pSetEdit.lookupControlType((XVar)(this.field)));
			this.multiselect = XVar.Clone(this.pageObject.pSetEdit.multiSelect((XVar)(this.field)));
			this.customDisplay = XVar.Clone(this.pageObject.pSetEdit.getCustomDisplay((XVar)(this.field)));
			this.lookupSize = XVar.Clone(this.pageObject.pSetEdit.selectSize((XVar)(this.field)));
			this.bUseCategory = XVar.Clone(this.pageObject.pSetEdit.useCategory((XVar)(this.field)));
		}
		public override XVar makeWidthStyle(dynamic _param_widthPx)
		{
			#region pass-by-value parameters
			dynamic widthPx = XVar.Clone(_param_widthPx);
			#endregion

			if(!XVar.Equals(XVar.Pack(this.LCType), XVar.Pack(Constants.LCT_DROPDOWN)))
			{
				return base.makeWidthStyle((XVar)(widthPx));
			}
			if(XVar.Pack(0) == widthPx)
			{
				return "";
			}
			return MVCFunctions.Concat("width: ", widthPx + 7, "px");
		}
		public override XVar addJSFiles()
		{
			if((XVar)(this.multiselect)  && (XVar)((XVar)((XVar)((XVar)(this.LCType == Constants.LCT_DROPDOWN)  && (XVar)(this.lookupSize == 1))  || (XVar)(this.LCType == Constants.LCT_AJAX))  || (XVar)(this.LCType == Constants.LCT_LIST)))
			{
				this.pageObject.AddJSFile(new XVar("include/chosen/chosen.jquery.js"));
			}

			return null;
		}
		public override XVar addCSSFiles()
		{
			if((XVar)(this.multiselect)  && (XVar)((XVar)((XVar)((XVar)(this.LCType == Constants.LCT_DROPDOWN)  && (XVar)(this.lookupSize == 1))  || (XVar)(this.LCType == Constants.LCT_AJAX))  || (XVar)(this.LCType == Constants.LCT_LIST)))
			{
				this.pageObject.AddCSSFile(new XVar("include/chosen/bootstrap-chosen.css"));
			}

			return null;
		}
		public virtual XVar parentBuildControl(dynamic _param_value, dynamic _param_mode, dynamic _param_fieldNum, dynamic _param_validate, dynamic _param_additionalCtrlParams, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic validate = XVar.Clone(_param_validate);
			dynamic additionalCtrlParams = XVar.Clone(_param_additionalCtrlParams);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			base.buildControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));

			return null;
		}
		public override XVar buildControl(dynamic _param_value, dynamic _param_mode, dynamic _param_fieldNum, dynamic _param_validate, dynamic _param_additionalCtrlParams, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic validate = XVar.Clone(_param_validate);
			dynamic additionalCtrlParams = XVar.Clone(_param_additionalCtrlParams);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic avalue = null, searchOption = null, suffix = null;
			base.buildControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			this.alt = XVar.Clone((XVar.Pack((XVar)((XVar)(mode == Constants.MODE_INLINE_EDIT)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  && (XVar)(this.is508)) ? XVar.Pack(MVCFunctions.Concat(" alt=\"", MVCFunctions.runner_htmlspecialchars((XVar)(this.strLabel)), "\" ")) : XVar.Pack("")));
			suffix = XVar.Clone(MVCFunctions.Concat("_", MVCFunctions.GoodFieldName((XVar)(this.field)), "_", this.id));
			this.clookupfield = XVar.Clone(MVCFunctions.Concat("display_value", (XVar.Pack(fieldNum) ? XVar.Pack(fieldNum) : XVar.Pack("")), suffix));
			this.cfield = XVar.Clone(MVCFunctions.Concat("value", suffix));
			this.ctype = XVar.Clone(MVCFunctions.Concat("type", suffix));
			if(XVar.Pack(fieldNum))
			{
				this.cfield = XVar.Clone(MVCFunctions.Concat("value", fieldNum, suffix));
				this.ctype = XVar.Clone(MVCFunctions.Concat("type", fieldNum, suffix));
			}
			if(XVar.Pack(this.ciphererLookup))
			{
				this.isLinkFieldEncrypted = XVar.Clone(this.ciphererLookup.isFieldPHPEncrypted((XVar)(this.linkFieldName)));
			}
			this.horizontalLookup = XVar.Clone(this.pageObject.pSetEdit.isHorizontalLookup((XVar)(this.field)));
			this.addNewItem = XVar.Clone(this.isAllowToAdd((XVar)(mode)));
			this.editLookupItem = XVar.Clone(this.isAllowToEdit((XVar)(mode)));
			this.multiple = XVar.Clone((XVar.Pack(this.multiselect) ? XVar.Pack(" multiple") : XVar.Pack("")));
			this.postfix = XVar.Clone((XVar.Pack(this.multiselect) ? XVar.Pack("[]") : XVar.Pack("")));
			if(XVar.Pack(this.multiselect))
			{
				avalue = XVar.Clone(CommonFunctions.splitLookupValues((XVar)(value)));
			}
			else
			{
				avalue = XVar.Clone(new XVar(0, XVar.Pack(value).ToString()));
			}
			searchOption = XVar.Clone(additionalCtrlParams["option"]);
			if(this.lookupType == Constants.LT_LISTOFVALUES)
			{
				this.buildListOfValues((XVar)(avalue), (XVar)(value), (XVar)(mode), (XVar)(searchOption));
			}
			else
			{
				if(XVar.Pack(!(XVar)(this.lookupDataSource)))
				{
					return null;
				}
				if(XVar.Pack(this.ciphererLookup))
				{
					this.isDisplayFieldEncrypted = XVar.Clone(this.ciphererLookup.isFieldPHPEncrypted((XVar)(this.displayFieldName)));
				}
				if((XVar)(this.LCType == Constants.LCT_AJAX)  || (XVar)(this.LCType == Constants.LCT_LIST))
				{
					this.buildAJAXLookup((XVar)(avalue), (XVar)(value), (XVar)(mode), (XVar)(searchOption));
				}
				else
				{
					this.buildClassicLookup((XVar)(avalue), (XVar)(value), (XVar)(mode), (XVar)(searchOption));
				}
			}
			this.buildControlEnd((XVar)(validate), (XVar)(mode));

			return null;
		}
		protected virtual XVar isAllowToAdd(dynamic _param_mode)
		{
			#region pass-by-value parameters
			dynamic mode = XVar.Clone(_param_mode);
			#endregion

			dynamic addNewItem = null, strPerm = null;
			addNewItem = new XVar(false);
			strPerm = XVar.Clone(CommonFunctions.GetUserPermissions((XVar)(this.lookupTable)));
			if((XVar)((XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("A"))), XVar.Pack(false)))  && (XVar)(this.LCType != Constants.LCT_LIST))  && (XVar)(mode != Constants.MODE_SEARCH))
			{
				dynamic advancedadd = null;
				addNewItem = XVar.Clone(this.pageObject.pSetEdit.isAllowToAdd((XVar)(this.field)));
				advancedadd = XVar.Clone(this.pageObject.pSetEdit.getLookupType((XVar)(this.field)) == Constants.LT_QUERY);
				if((XVar)(!(XVar)(advancedadd))  || (XVar)(this.pageObject.pageType == Constants.PAGE_REGISTER))
				{
					addNewItem = new XVar(false);
				}
			}
			return addNewItem;
		}
		protected virtual XVar isAllowToEdit(dynamic _param_mode)
		{
			#region pass-by-value parameters
			dynamic mode = XVar.Clone(_param_mode);
			#endregion

			dynamic allowEdit = null, strPerm = null;
			allowEdit = new XVar(false);
			strPerm = XVar.Clone(CommonFunctions.GetUserPermissions((XVar)(this.lookupTable)));
			if((XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("E"))), XVar.Pack(false)))  && (XVar)(mode != Constants.MODE_SEARCH))
			{
				allowEdit = XVar.Clone(this.pageObject.pSetEdit.isAllowToEdit((XVar)(this.field)));
			}
			return allowEdit;
		}
		public virtual XVar buildListOfValues(dynamic _param_avalue, dynamic _param_value, dynamic _param_mode, dynamic _param_searchOption)
		{
			#region pass-by-value parameters
			dynamic avalue = XVar.Clone(_param_avalue);
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic searchOption = XVar.Clone(_param_searchOption);
			#endregion

			dynamic arr = XVar.Array(), dataAttr = null, display_values = XVar.Array(), dropDownHasSimpleBox = null, i = null, optionContains = null, res = null, selectClass = null, spacer = null;
			arr = XVar.Clone(this.pageObject.pSetEdit.getLookupValues((XVar)(this.field)));
			display_values = XVar.Clone(arr);
			if(XVar.Pack(MVCFunctions.in_array((XVar)(this.pageObject.pSetEdit.getViewFormat((XVar)(this.field))), (XVar)(new XVar(0, Constants.FORMAT_DATE_SHORT, 1, Constants.FORMAT_DATE_LONG, 2, Constants.FORMAT_DATE_TIME)))))
			{
				dynamic container = null;
				container = XVar.Clone(new ViewControlsContainer((XVar)(this.pageObject.pSetEdit), new XVar(Constants.PAGE_LIST), new XVar(null)));
				foreach (KeyValuePair<XVar, dynamic> opt in arr.GetEnumerator())
				{
					dynamic data = XVar.Array();
					data = XVar.Clone(XVar.Array());
					data.InitAndSetArrayItem(opt.Value, this.field);
					display_values.InitAndSetArrayItem(container.getControl((XVar)(this.field)).getTextValue((XVar)(data)), opt.Key);
				}
			}
			dropDownHasSimpleBox = XVar.Clone((XVar)((XVar)(this.LCType == Constants.LCT_DROPDOWN)  && (XVar)(!(XVar)(this.multiselect)))  && (XVar)(mode == Constants.MODE_SEARCH));
			optionContains = XVar.Clone((XVar)(dropDownHasSimpleBox)  && (XVar)(this.isSearchOpitonForSimpleBox((XVar)(searchOption))));
			if(XVar.Pack(this.multiselect))
			{
				MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.ctype, "\" type=\"hidden\" name=\"", this.ctype, "\" value=\"multiselect\">"));
			}
			switch(((XVar)this.LCType).ToInt())
			{
				case Constants.LCT_DROPDOWN:
					dataAttr = XVar.Clone(selectClass = new XVar(""));
					if(XVar.Pack(dropDownHasSimpleBox))
					{
						dynamic simpleBoxClass = null, simpleBoxStyle = null;
						dataAttr = new XVar(" data-usesuggests=\"true\"");
						selectClass = XVar.Clone((XVar.Pack(optionContains) ? XVar.Pack(" class=\"rnr-hiddenControlSubelem\" ") : XVar.Pack("")));
						simpleBoxClass = XVar.Clone((XVar.Pack(optionContains) ? XVar.Pack("") : XVar.Pack(" class=\"rnr-hiddenControlSubelem\" ")));
						simpleBoxStyle = XVar.Clone(this.getWidthStyleForAdditionalControl());
						MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "_simpleSearchBox\" type=\"text\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\" autocomplete=\"off\"", simpleBoxClass, " ", simpleBoxStyle, ">"));
					}
					MVCFunctions.Echo(MVCFunctions.Concat("<select id=\"", this.cfield, "\" size=\"", this.lookupSize, "\" ", dataAttr, selectClass, " name=\"", this.cfield, this.postfix, "\" ", this.multiple, " ", this.inputStyle, ">"));
					if(XVar.Pack(!(XVar)(this.multiselect)))
					{
						MVCFunctions.Echo(MVCFunctions.Concat("<option value=\"\">", CommonFunctions.mlang_message(new XVar("PLEASE_SELECT")), "</option>"));
					}
					else
					{
						if(mode == Constants.MODE_SEARCH)
						{
							MVCFunctions.Echo("<option value=\"\"> </option>");
						}
					}
					foreach (KeyValuePair<XVar, dynamic> opt in arr.GetEnumerator())
					{
						res = XVar.Clone(MVCFunctions.array_search((XVar)(XVar.Pack(opt.Value).ToString()), (XVar)(avalue)));
						if(!XVar.Equals(XVar.Pack(res), XVar.Pack(false)))
						{
							MVCFunctions.Echo(MVCFunctions.Concat("<option value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(opt.Value)), "\" selected>", MVCFunctions.runner_htmlspecialchars((XVar)(display_values[opt.Key])), "</option>"));
						}
						else
						{
							MVCFunctions.Echo(MVCFunctions.Concat("<option value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(opt.Value)), "\">", MVCFunctions.runner_htmlspecialchars((XVar)(display_values[opt.Key])), "</option>"));
						}
					}
					MVCFunctions.Echo("</select>");
					break;
				case Constants.LCT_CBLIST:
					MVCFunctions.Echo(MVCFunctions.Concat("<div data-lookup-options class=\"", (XVar.Pack(this.horizontalLookup) ? XVar.Pack("rnr-horizontal-lookup") : XVar.Pack("rnr-vertical-lookup")), "\">"));
					spacer = new XVar("<br/>");
					if(XVar.Pack(this.horizontalLookup))
					{
						spacer = new XVar("&nbsp;&nbsp;");
					}
					i = new XVar(0);
					foreach (KeyValuePair<XVar, dynamic> opt in arr.GetEnumerator())
					{
						MVCFunctions.Echo("<span class=\"checkbox\"><label>");
						MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "_", i, "\" class=\"rnr-checkbox\" type=\"checkbox\" ", this.alt, " name=\"", this.cfield, this.postfix, "\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(opt.Value)), "\""));
						res = XVar.Clone(MVCFunctions.array_search((XVar)(XVar.Pack(opt.Value).ToString()), (XVar)(avalue)));
						if(!XVar.Equals(XVar.Pack(res), XVar.Pack(false)))
						{
							MVCFunctions.Echo(" checked=\"checked\" ");
						}
						MVCFunctions.Echo("/>");
						MVCFunctions.Echo(MVCFunctions.Concat("&nbsp;<span class=\"rnr-checkbox-label\" id=\"data_", this.cfield, "_", i, "\">", MVCFunctions.runner_htmlspecialchars((XVar)(display_values[opt.Key])), "</span>", spacer));
						MVCFunctions.Echo("</label></span>");
						i++;
					}
					MVCFunctions.Echo("</div>");
					break;
				case Constants.LCT_RADIO:
					MVCFunctions.Echo(MVCFunctions.Concat("<div data-lookup-options class=\"", (XVar.Pack(this.horizontalLookup) ? XVar.Pack("rnr-horizontal-lookup") : XVar.Pack("rnr-vertical-lookup")), "\">"));
					MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "\" type=\"hidden\" name=\"", this.cfield, "\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\">"));
					i = new XVar(0);
					foreach (KeyValuePair<XVar, dynamic> opt in arr.GetEnumerator())
					{
						dynamic var_checked = null;
						var_checked = new XVar("");
						if(opt.Value == value)
						{
							var_checked = new XVar(" checked=\"checked\" ");
						}
						MVCFunctions.Echo("<span class=\"radio\"><label>");
						MVCFunctions.Echo(MVCFunctions.Concat("<input type=\"Radio\" class=\"rnr-radio-button\" id=\"radio_", this.cfieldname, "_", i, "\" ", this.alt, " name=\"radio_", this.cfieldname, "\" ", var_checked, " value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(opt.Value)), "\">", " <span id=\"label_radio_", this.cfieldname, "_", i, "\" class=\"rnr-radio-label\">", MVCFunctions.runner_htmlspecialchars((XVar)(display_values[opt.Key])), "</span >"));
						MVCFunctions.Echo("</label></span>");
						i++;
					}
					MVCFunctions.Echo("</div>");
					break;
			}

			return null;
		}
		public virtual XVar buildAJAXLookup(dynamic _param_avalue, dynamic _param_value, dynamic _param_mode, dynamic _param_searchOption)
		{
			#region pass-by-value parameters
			dynamic avalue = XVar.Clone(_param_avalue);
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic searchOption = XVar.Clone(_param_searchOption);
			#endregion

			dynamic data = XVar.Array(), dataAttr = null, dataKeysAttr = null, inputParams = null, inputTag = null, listOptionContains = null, listSearchHasSimpleBox = null, lookupDc = null, lookup_value = null, optionContains = null, qResult = null;
			if(XVar.Pack(this.multiselect))
			{
				this.buildMultiselectAJAXLookup((XVar)(avalue), (XVar)(value), (XVar)(mode), (XVar)(searchOption));
				return null;
			}
			listSearchHasSimpleBox = XVar.Clone((XVar)(mode == Constants.MODE_SEARCH)  && (XVar)(this.isAdditionalControlRequired()));
			optionContains = XVar.Clone(this.isSearchOpitonForSimpleBox((XVar)(searchOption)));
			listOptionContains = XVar.Clone((XVar)(listSearchHasSimpleBox)  && (XVar)(optionContains));
			dataAttr = new XVar("");
			if(this.LCType == Constants.LCT_LIST)
			{
				dataAttr = XVar.Clone((XVar.Pack(listSearchHasSimpleBox) ? XVar.Pack(" data-usesuggests=\"true\"") : XVar.Pack("")));
			}
			else
			{
				if((XVar)(this.LCType == Constants.LCT_AJAX)  && (XVar)(optionContains))
				{
					dataAttr = new XVar(" data-simple-search-mode=\"true\" ");
				}
			}
			if(XVar.Pack(this.bUseCategory))
			{
				dynamic valueAttr = null;
				valueAttr = new XVar("");
				if((XVar)((XVar)(this.LCType == Constants.LCT_AJAX)  && (XVar)(optionContains))  || (XVar)((XVar)(this.LCType == Constants.LCT_LIST)  && (XVar)(listOptionContains)))
				{
					valueAttr = XVar.Clone(MVCFunctions.Concat(" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\""));
				}
				inputParams = XVar.Clone(MVCFunctions.Concat("\" ", this.getPlaceholderAttr(), " autocomplete=\"off\" id=\"", this.clookupfield, "\" ", valueAttr, " name=\"", this.clookupfield, "\" ", this.inputStyle));
				inputParams = MVCFunctions.Concat(inputParams, (XVar.Pack((XVar)(this.LCType == Constants.LCT_LIST)  && (XVar)(!(XVar)(listOptionContains))) ? XVar.Pack("readonly") : XVar.Pack("")));
				MVCFunctions.Echo(MVCFunctions.Concat("<input type=\"text\" ", inputParams, ">"));
				MVCFunctions.Echo(MVCFunctions.Concat("<input type=\"hidden\" id=\"", this.cfield, "\" ", valueAttr, " name=\"", this.cfield, "\"", dataAttr, ">"));
				MVCFunctions.Echo(this.getLookupLinks((XVar)(listOptionContains)));
				return null;
			}
			lookup_value = new XVar("");
			lookupDc = XVar.Clone(this.getLookupDataCommand((XVar)(XVar.Array()), (XVar)(value), new XVar(false), new XVar(true)));
			qResult = XVar.Clone(this.lookupDataSource.getList((XVar)(lookupDc)));
			if(XVar.Pack(!(XVar)(qResult)))
			{
				MVCFunctions.showError((XVar)(this.lookupDataSource.lastError()));
			}
			if(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				if(XVar.Pack(this.isDisplayFieldEncrypted))
				{
					lookup_value = XVar.Clone(this.ciphererLookup.DecryptField((XVar)(this.displayFieldName), (XVar)(data[this.displayFieldAlias])));
				}
				else
				{
					lookup_value = XVar.Clone(data[this.displayFieldAlias]);
				}
			}
			else
			{
				if(XVar.Pack(this.pageObject.pSetEdit.isLookupWhereSet((XVar)(this.field))))
				{
					lookupDc = XVar.Clone(this.getLookupDataCommand((XVar)(XVar.Array()), (XVar)(value), new XVar(false), new XVar(true), new XVar(false)));
					qResult = XVar.Clone(this.lookupDataSource.getList((XVar)(lookupDc)));
					if(XVar.Pack(!(XVar)(qResult)))
					{
						MVCFunctions.showError((XVar)(this.lookupDataSource.lastError()));
					}
					if(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
					{
						if(XVar.Pack(this.isDisplayFieldEncrypted))
						{
							lookup_value = XVar.Clone(this.ciphererLookup.DecryptField((XVar)(this.displayFieldName), (XVar)(data[this.displayFieldAlias])));
						}
						else
						{
							lookup_value = XVar.Clone(data[this.displayFieldAlias]);
						}
					}
				}
			}
			if((XVar)((XVar)((XVar)(this.LCType == Constants.LCT_AJAX)  && (XVar)(!(XVar)(MVCFunctions.strlen((XVar)(lookup_value)))))  && (XVar)((XVar)(this.pageObject.pSetEdit.isFreeInput((XVar)(this.field)))  || (XVar)(this.lookupPageType == Constants.PAGE_SEARCH)))  || (XVar)((XVar)(this.LCType == Constants.LCT_LIST)  && (XVar)(listOptionContains)))
			{
				lookup_value = XVar.Clone(value);
			}
			if(XVar.Pack(MVCFunctions.in_array((XVar)(this.pageObject.pSetEdit.getViewFormat((XVar)(this.field))), (XVar)(new XVar(0, Constants.FORMAT_DATE_SHORT, 1, Constants.FORMAT_DATE_LONG, 2, Constants.FORMAT_DATE_TIME)))))
			{
				dynamic container = null, ctrlData = XVar.Array();
				container = XVar.Clone(new ViewControlsContainer((XVar)(this.pageObject.pSetEdit), new XVar(Constants.PAGE_LIST), new XVar(null)));
				ctrlData = XVar.Clone(XVar.Array());
				ctrlData.InitAndSetArrayItem(lookup_value, this.field);
				lookup_value = XVar.Clone(container.getControl((XVar)(this.field)).getTextValue((XVar)(ctrlData)));
			}
			inputParams = XVar.Clone(MVCFunctions.Concat("autocomplete=\"off\" ", this.getPlaceholderAttr(), " id=\"", this.clookupfield, "\" name=\"", this.clookupfield, "\" ", this.inputStyle, this.alt));
			inputParams = MVCFunctions.Concat(inputParams, " value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(lookup_value)), "\"");
			if((XVar)(this.LCType == Constants.LCT_LIST)  && (XVar)(!(XVar)(listOptionContains)))
			{
				inputParams = MVCFunctions.Concat(inputParams, " readonly ");
			}
			if((XVar)(this.LCType == Constants.LCT_LIST)  && (XVar)(!(XVar)(this.pageObject.pSetEdit.isRequired((XVar)(this.field)))))
			{
				inputParams = MVCFunctions.Concat(inputParams, " class=\"clearable\" ");
			}
			dataKeysAttr = XVar.Clone((XVar.Pack(this.editLookupItem) ? XVar.Pack(this.getDataKeysAttr(ref data)) : XVar.Pack("")));
			inputTag = XVar.Clone(MVCFunctions.Concat("<input type=\"text\" ", inputParams, " ", dataKeysAttr, ">"));
			if(this.LCType == Constants.LCT_LIST)
			{
				MVCFunctions.Echo(MVCFunctions.Concat("<span class=\"bs-list-lookup\">", inputTag, "</span>"));
			}
			else
			{
				MVCFunctions.Echo(inputTag);
			}
			MVCFunctions.Echo(MVCFunctions.Concat("<input type=\"hidden\" id=\"", this.cfield, "\" name=\"", this.cfield, "\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\"", dataAttr, ">"));
			MVCFunctions.Echo(this.getLookupLinks((XVar)(listOptionContains)));

			return null;
		}
		protected virtual XVar buildMultiselectAJAXLookup(dynamic _param_avalue, dynamic _param_value, dynamic _param_mode, dynamic _param_searchOption)
		{
			#region pass-by-value parameters
			dynamic avalue = XVar.Clone(_param_avalue);
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic searchOption = XVar.Clone(_param_searchOption);
			#endregion

			MVCFunctions.Echo(MVCFunctions.Concat("<select ", this.getPlaceholderAttr(), " ", this.multiple, " id=\"", this.cfield, "\" name=\"", this.cfield, this.postfix, "\" ", this.inputStyle, this.alt, ">"));
			if((XVar)(!(XVar)(this.bUseCategory))  && (XVar)(MVCFunctions.strlen((XVar)(value))))
			{
				this.buildMultiselectAJAXLookupRows((XVar)(avalue), (XVar)(value), (XVar)(mode), (XVar)(searchOption));
			}
			MVCFunctions.Echo("</select>");
			MVCFunctions.Echo(this.getLookupLinks());

			return null;
		}
		protected virtual XVar buildMultiselectAJAXLookupRows(dynamic _param_avalue, dynamic _param_value, dynamic _param_mode, dynamic _param_searchOption)
		{
			#region pass-by-value parameters
			dynamic avalue = XVar.Clone(_param_avalue);
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic searchOption = XVar.Clone(_param_searchOption);
			#endregion

			dynamic data = XVar.Array(), lookupDc = null, options = null, qResult = null;
			if((XVar)((XVar)(this.linkAndDisplaySame)  && (XVar)(!(XVar)(this.editLookupItem)))  || (XVar)(this.lookupPageType == Constants.PAGE_SEARCH))
			{
				this.displayFieldAlias = XVar.Clone(this.displayFieldName);
				foreach (KeyValuePair<XVar, dynamic> mValue in avalue.GetEnumerator())
				{
					data = XVar.Clone(XVar.Array());
					data.InitAndSetArrayItem(mValue.Value, this.linkFieldName);
					data.InitAndSetArrayItem(mValue.Value, this.displayFieldAlias);
					this.buildLookupRow((XVar)(mode), (XVar)(data), new XVar(" selected"), (XVar)(mValue.Key));
				}
				return null;
			}
			lookupDc = XVar.Clone(this.getLookupDataCommand((XVar)(XVar.Array()), (XVar)(value), new XVar(false), new XVar(true)));
			qResult = XVar.Clone(this.lookupDataSource.getList((XVar)(lookupDc)));
			if(XVar.Pack(!(XVar)(qResult)))
			{
				MVCFunctions.showError((XVar)(this.lookupDataSource.lastError()));
			}
			options = new XVar(0);
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				this.decryptDataRow((XVar)(data));
				if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(data[this.linkFieldName]), (XVar)(avalue))), XVar.Pack(false)))
				{
					this.buildLookupRow((XVar)(mode), (XVar)(data), new XVar(" selected"), (XVar)(options));
					options++;
				}
			}
			if((XVar)((XVar)((XVar)(options == XVar.Pack(0))  && (XVar)(MVCFunctions.strlen((XVar)(value))))  && (XVar)(mode == Constants.MODE_EDIT))  && (XVar)(this.pageObject.pSetEdit.isLookupWhereSet((XVar)(this.field))))
			{
				lookupDc = XVar.Clone(this.getLookupDataCommand((XVar)(XVar.Array()), (XVar)(value), new XVar(false), new XVar(true), new XVar(false), new XVar(true)));
				qResult = XVar.Clone(this.lookupDataSource.getList((XVar)(lookupDc)));
				if(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
				{
					this.decryptDataRow((XVar)(data));
					this.buildLookupRow((XVar)(mode), (XVar)(data), new XVar(" selected"), (XVar)(options));
				}
			}

			return null;
		}
		public virtual XVar buildClassicLookup(dynamic _param_avalue, dynamic _param_value, dynamic _param_mode, dynamic _param_searchOption)
		{
			#region pass-by-value parameters
			dynamic avalue = XVar.Clone(_param_avalue);
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic searchOption = XVar.Clone(_param_searchOption);
			#endregion

			dynamic data = XVar.Array(), dataAttr = null, dropDownHasSimpleBox = null, footer = null, found = null, i = null, isLookupUnique = null, linkValue = null, lookupDc = null, optionContains = null, qResult = null, res = null, selectClass = null, simpleBoxClass = null, simpleBoxStyle = null, uniqueArray = XVar.Array(), var_checked = null;
			dropDownHasSimpleBox = XVar.Clone((XVar)((XVar)(this.LCType == Constants.LCT_DROPDOWN)  && (XVar)(mode == Constants.MODE_SEARCH))  && (XVar)(this.isAdditionalControlRequired()));
			optionContains = XVar.Clone((XVar)(dropDownHasSimpleBox)  && (XVar)(this.isSearchOpitonForSimpleBox((XVar)(searchOption))));
			if(XVar.Pack(this.multiselect))
			{
				MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.ctype, "\" type=\"hidden\" name=\"", this.ctype, "\" value=\"multiselect\">"));
			}
			if(XVar.Pack(this.bUseCategory))
			{
				switch(((XVar)this.LCType).ToInt())
				{
					case Constants.LCT_CBLIST:
						MVCFunctions.Echo("<div data-lookup-options>");
						MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "\" type=\"checkbox\" name=\"", this.cfield, "\" style=\"display:none;\">"));
						MVCFunctions.Echo("</div>");
						break;
					case Constants.LCT_RADIO:
						MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "\" type=\"hidden\" name=\"", this.cfield, "\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\">"));
						MVCFunctions.Echo("<div data-lookup-options>");
						MVCFunctions.Echo("</div>");
						break;
					case Constants.LCT_DROPDOWN:
						dataAttr = new XVar("");
						selectClass = new XVar("form-control");
						simpleBoxClass = new XVar("form-control");
						if(XVar.Pack(dropDownHasSimpleBox))
						{
							dataAttr = new XVar(" data-usesuggests=\"true\"");
							selectClass = MVCFunctions.Concat(selectClass, (XVar.Pack(optionContains) ? XVar.Pack(" rnr-hiddenControlSubelem") : XVar.Pack("")));
							simpleBoxClass = MVCFunctions.Concat(simpleBoxClass, (XVar.Pack(optionContains) ? XVar.Pack("") : XVar.Pack(" rnr-hiddenControlSubelem")));
							simpleBoxStyle = new XVar("");
							MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "_simpleSearchBox\" type=\"text\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\" autocomplete=\"off\" class=\"", simpleBoxClass, "\" ", simpleBoxStyle, ">"));
						}
						MVCFunctions.Echo(MVCFunctions.Concat("<select size=\"", this.lookupSize, "\" id=\"", this.cfield, "\" name=\"", this.cfield, this.postfix, "\" class=\"", selectClass, "\" ", dataAttr, this.multiple, " ", this.inputStyle, ">"));
						MVCFunctions.Echo(MVCFunctions.Concat("<option value=\"\">", CommonFunctions.mlang_message(new XVar("PLEASE_SELECT")), "</option>"));
						MVCFunctions.Echo("</select>");
						break;
				}
				MVCFunctions.Echo(this.getLookupLinks());
				return null;
			}
			lookupDc = XVar.Clone(this.getLookupDataCommand((XVar)(XVar.Array()), new XVar(""), new XVar(false)));
			qResult = XVar.Clone(this.lookupDataSource.getList((XVar)(lookupDc)));
			if(XVar.Pack(!(XVar)(qResult)))
			{
				MVCFunctions.showError((XVar)(this.lookupDataSource.lastError()));
			}
			if(this.LCType == Constants.LCT_DROPDOWN)
			{
				dataAttr = new XVar("");
				selectClass = new XVar("form-control");
				simpleBoxClass = new XVar("form-control");
				if(XVar.Pack(dropDownHasSimpleBox))
				{
					dataAttr = new XVar(" data-usesuggests=\"true\"");
					selectClass = MVCFunctions.Concat(selectClass, (XVar.Pack(optionContains) ? XVar.Pack(" rnr-hiddenControlSubelem") : XVar.Pack("")));
					simpleBoxClass = MVCFunctions.Concat(simpleBoxClass, (XVar.Pack(optionContains) ? XVar.Pack("") : XVar.Pack(" rnr-hiddenControlSubelem")));
					simpleBoxStyle = new XVar("");
					MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "_simpleSearchBox\" type=\"text\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\" autocomplete=\"off\" class=\"", simpleBoxClass, "\" ", simpleBoxStyle, ">"));
				}
				MVCFunctions.Echo(MVCFunctions.Concat("<select size=\"", this.lookupSize, "\" id=\"", this.cfield, "\" ", this.alt, " name=\"", this.cfield, this.postfix, "\"", dataAttr, " class=\"", selectClass, "\" ", this.multiple, " ", this.inputStyle, ">"));
				if(XVar.Pack(!(XVar)(this.multiselect)))
				{
					MVCFunctions.Echo(MVCFunctions.Concat("<option value=\"\">", CommonFunctions.mlang_message(new XVar("PLEASE_SELECT")), "</option>"));
				}
				else
				{
					if(mode == Constants.MODE_SEARCH)
					{
						MVCFunctions.Echo("<option value=\"\"> </option>");
					}
				}
			}
			else
			{
				if(this.LCType == Constants.LCT_RADIO)
				{
					MVCFunctions.Echo(MVCFunctions.Concat("<input id=\"", this.cfield, "\" type=\"hidden\" name=\"", this.cfield, "\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "\">"));
				}
				MVCFunctions.Echo(MVCFunctions.Concat("<div data-lookup-options class=\"", (XVar.Pack(this.horizontalLookup) ? XVar.Pack("rnr-horizontal-lookup") : XVar.Pack("rnr-vertical-lookup")), "\">"));
			}
			found = new XVar(false);
			i = new XVar(0);
			isLookupUnique = XVar.Clone((XVar)(this.lookupType == Constants.LT_QUERY)  && (XVar)(this.pageObject.pSetEdit.isLookupUnique((XVar)(this.field))));
			uniqueArray = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				this.decryptDataRow((XVar)(data));
				linkValue = XVar.Clone(data[this.linkFieldName]);
				if(XVar.Pack(isLookupUnique))
				{
					if(XVar.Pack(MVCFunctions.in_array((XVar)(linkValue), (XVar)(uniqueArray))))
					{
						continue;
					}
					uniqueArray.InitAndSetArrayItem(linkValue, null);
				}
				res = XVar.Clone(MVCFunctions.array_search((XVar)(XVar.Pack(linkValue).ToString()), (XVar)(avalue)));
				var_checked = new XVar("");
				if(!XVar.Equals(XVar.Pack(res), XVar.Pack(false)))
				{
					found = new XVar(true);
					var_checked = XVar.Clone((XVar.Pack((XVar)(this.LCType == Constants.LCT_CBLIST)  || (XVar)(this.LCType == Constants.LCT_RADIO)) ? XVar.Pack(" checked=\"checked\"") : XVar.Pack(" selected")));
				}
				this.buildLookupRow((XVar)(mode), (XVar)(data), (XVar)(var_checked), (XVar)(i));
				i++;
			}
			if((XVar)((XVar)((XVar)(!(XVar)(found))  && (XVar)(MVCFunctions.strlen((XVar)(value))))  && (XVar)(mode == Constants.MODE_EDIT))  && (XVar)(this.pageObject.pSetEdit.isLookupWhereSet((XVar)(this.field))))
			{
				lookupDc = XVar.Clone(this.getLookupDataCommand((XVar)(XVar.Array()), (XVar)(value), new XVar(false), new XVar(true), new XVar(false), new XVar(true)));
				qResult = XVar.Clone(this.lookupDataSource.getList((XVar)(lookupDc)));
				if(XVar.Pack(!(XVar)(qResult)))
				{
					MVCFunctions.showError((XVar)(this.lookupDataSource.lastError()));
				}
				if(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
				{
					this.decryptDataRow((XVar)(data));
					var_checked = XVar.Clone((XVar.Pack((XVar)(this.LCType == Constants.LCT_CBLIST)  || (XVar)(this.LCType == Constants.LCT_RADIO)) ? XVar.Pack(" checked=\"checked\"") : XVar.Pack(" selected")));
					this.buildLookupRow((XVar)(mode), (XVar)(data), (XVar)(var_checked), (XVar)(i));
				}
			}
			footer = XVar.Clone((XVar.Pack(this.LCType == Constants.LCT_DROPDOWN) ? XVar.Pack("</select>") : XVar.Pack("</div>")));
			MVCFunctions.Echo(footer);
			MVCFunctions.Echo(this.getLookupLinks());

			return null;
		}
		public virtual XVar decryptDataRow(dynamic data)
		{
			if(XVar.Pack(this.isLinkFieldEncrypted))
			{
				data.InitAndSetArrayItem(this.ciphererLookup.DecryptField((XVar)(this.linkFieldName), (XVar)(data[this.linkFieldName])), this.linkFieldName);
			}
			if(XVar.Pack(this.isDisplayFieldEncrypted))
			{
				data.InitAndSetArrayItem(this.ciphererLookup.DecryptField((XVar)(this.displayFieldName), (XVar)(data[this.displayFieldAlias])), this.displayFieldAlias);
			}

			return null;
		}
		public virtual XVar buildLookupRow(dynamic _param_mode, dynamic _param_data, dynamic _param_checked, dynamic _param_i)
		{
			#region pass-by-value parameters
			dynamic mode = XVar.Clone(_param_mode);
			dynamic data = XVar.Clone(_param_data);
			dynamic var_checked = XVar.Clone(_param_checked);
			dynamic i = XVar.Clone(_param_i);
			#endregion

			dynamic dataKeysAttr = null, display_value = null, link_value = null, render_value = null;
			display_value = XVar.Clone(data[this.displayFieldAlias]);
			link_value = XVar.Clone(data[this.linkFieldName]);
			if(XVar.Pack(MVCFunctions.in_array((XVar)(this.pageObject.pSetEdit.getViewFormat((XVar)(this.field))), (XVar)(new XVar(0, Constants.FORMAT_DATE_SHORT, 1, Constants.FORMAT_DATE_LONG, 2, Constants.FORMAT_DATE_TIME)))))
			{
				dynamic container = null, ctrlData = XVar.Array();
				container = XVar.Clone(new ViewControlsContainer((XVar)(this.pageObject.pSetEdit), new XVar(Constants.PAGE_LIST), new XVar(null)));
				ctrlData = XVar.Clone(XVar.Array());
				ctrlData.InitAndSetArrayItem(link_value, this.field);
				display_value = XVar.Clone(container.getControl((XVar)(this.field)).getTextValue((XVar)(ctrlData)));
			}
			render_value = XVar.Clone(this.getLookupTextValue((XVar)(display_value)));
			dataKeysAttr = XVar.Clone((XVar.Pack(this.editLookupItem) ? XVar.Pack(this.getDataKeysAttr(ref data)) : XVar.Pack("")));
			switch(((XVar)this.LCType).ToInt())
			{
				case Constants.LCT_DROPDOWN:
				case Constants.LCT_LIST:
				case Constants.LCT_AJAX:
					MVCFunctions.Echo(MVCFunctions.Concat("<option value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(data[this.linkFieldName])), "\"", var_checked, " ", dataKeysAttr, ">", render_value, "</option>"));
					break;
				case Constants.LCT_CBLIST:
					MVCFunctions.Echo(MVCFunctions.Concat("<span class=\"checkbox\"><label>", "<input id=\"", this.cfield, "_", i, "\" class=\"rnr-checkbox\" type=\"checkbox\" ", this.alt, " name=\"", this.cfield, this.postfix, "\" ", dataKeysAttr, " value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(link_value)), "\"", var_checked, "/>&nbsp;", "<span class=\"rnr-checkbox-label\" id=\"data_", this.cfield, "_", i, "\">", render_value, "</span>", "</label></span>"));
					break;
				case Constants.LCT_RADIO:
					MVCFunctions.Echo(MVCFunctions.Concat("<span class=\"radio\"><label>", "<input type=\"Radio\" class=\"rnr-radio-button\" id=\"radio_", this.cfieldname, "_", i, "\" ", this.alt, " ", dataKeysAttr, " name=\"radio_", this.cfieldname, "\" ", var_checked, " value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(link_value)), "\">", " <span id=\"label_radio_", this.cfieldname, "_", i, "\" class=\"rnr-radio-label\">", render_value, "</span>", "</label></span>"));
					break;
			}

			return null;
		}
		public override XVar getFirstElementId()
		{
			switch(((XVar)this.LCType).ToInt())
			{
				case Constants.LCT_AJAX:
					return MVCFunctions.Concat("display_value_", this.goodFieldName, "_", this.id);
					break;
				default:
					return this.cfield;
					break;
			}

			return null;
		}
		public virtual XVar isSearchOpitonForSimpleBox(dynamic _param_searchOption)
		{
			#region pass-by-value parameters
			dynamic searchOption = XVar.Clone(_param_searchOption);
			#endregion

			dynamic userSearchOptions = null;
			if((XVar)(searchOption == "Contains")  || (XVar)(searchOption == "Starts with"))
			{
				return true;
			}
			if(searchOption != XVar.Pack(""))
			{
				return false;
			}
			userSearchOptions = XVar.Clone(this.pageObject.pSetEdit.getSearchOptionsList((XVar)(this.field)));
			return (XVar)((XVar)(!(XVar)(userSearchOptions))  || (XVar)(MVCFunctions.in_array(new XVar("Contains"), (XVar)(userSearchOptions))))  || (XVar)(MVCFunctions.in_array(new XVar("Starts with"), (XVar)(userSearchOptions)));
		}
		protected virtual XVar isAdditionalControlRequired()
		{
			dynamic hostPageType = null, userSearchOptions = null;
			if(XVar.Pack(this.multiselect))
			{
				return false;
			}
			hostPageType = XVar.Clone(ProjectSettings.defaultPageType((XVar)(this.pageObject.pSetEdit.getEntityType())));
			if((XVar)(hostPageType == "report")  || (XVar)(hostPageType == "chart"))
			{
				return false;
			}
			userSearchOptions = XVar.Clone(this.pageObject.pSetEdit.getSearchOptionsList((XVar)(this.field)));
			if((XVar)((XVar)(!(XVar)(!(XVar)(userSearchOptions)))  && (XVar)(!(XVar)(MVCFunctions.in_array(new XVar("Contains"), (XVar)(userSearchOptions)))))  && (XVar)(!(XVar)(MVCFunctions.in_array(new XVar("Starts with"), (XVar)(userSearchOptions)))))
			{
				return false;
			}
			if((XVar)(this.lookupType == Constants.LT_LISTOFVALUES)  || (XVar)(this.linkAndDisplaySame))
			{
				return true;
			}
			if(this.connection.connId != this.lookupDataSource.getConnectionId())
			{
				return false;
			}
			if((XVar)(!(XVar)(this.connection.checkIfJoinSubqueriesOptimized()))  && (XVar)(this.LCType == Constants.LCT_LIST))
			{
				return false;
			}
			return (XVar)(this.isLookupSQLquerySimple())  && (XVar)(this.isMainTableSQLquerySimple());
		}
		protected virtual XVar getWidthStyleForAdditionalControl()
		{
			dynamic style = null, width = null;
			width = XVar.Clone((XVar.Pack(this.searchPanelControl) ? XVar.Pack(150) : XVar.Pack(this.pageObject.pSetEdit.getControlWidth((XVar)(this.field)))));
			return MVCFunctions.Concat("style=\"", style, "\"");
		}
		protected virtual XVar isLookupSQLquerySimple()
		{
			dynamic lookupConnection = null, lookupSqlQuery = null;
			lookupConnection = XVar.Clone(this.lookupDataSource.getConnection());
			if((XVar)((XVar)(lookupConnection.dbType == Constants.nDATABASE_DB2)  || (XVar)(lookupConnection.dbType == Constants.nDATABASE_Informix))  || (XVar)(lookupConnection.dbType == Constants.nDATABASE_SQLite3))
			{
				return false;
			}
			if((XVar)(this.lookupType == Constants.LT_LOOKUPTABLE)  || (XVar)(this.lookupType == Constants.LT_LISTOFVALUES))
			{
				return true;
			}
			if(XVar.Pack(this.lookupPSet.hasEncryptedFields()))
			{
				return false;
			}
			lookupSqlQuery = XVar.Clone(this.lookupPSet.getSQLQuery());
			if(XVar.Pack(!(XVar)(lookupSqlQuery)))
			{
				return false;
			}
			if((XVar)((XVar)(lookupSqlQuery.HasGroupBy())  || (XVar)(lookupSqlQuery.HavingToSql() != ""))  || (XVar)(lookupSqlQuery.HasSubQueryInFromClause()))
			{
				return false;
			}
			if(lookupConnection.dbType != Constants.nDATABASE_MySQL)
			{
				dynamic linkFieldType = null;
				linkFieldType = XVar.Clone(this.lookupPSet.getFieldType((XVar)(this.linkFieldName)));
				if(XVar.Pack(!(XVar)((XVar)((XVar)((XVar)(CommonFunctions.IsNumberType((XVar)(this.var_type)))  && (XVar)(CommonFunctions.IsNumberType((XVar)(linkFieldType))))  || (XVar)((XVar)(CommonFunctions.IsCharType((XVar)(this.var_type)))  && (XVar)(CommonFunctions.IsCharType((XVar)(linkFieldType)))))  || (XVar)((XVar)(CommonFunctions.IsDateFieldType((XVar)(this.var_type)))  && (XVar)(CommonFunctions.IsDateFieldType((XVar)(linkFieldType)))))))
				{
					return false;
				}
			}
			return true;
		}
		protected virtual XVar isMainTableSQLquerySimple()
		{
			dynamic sqlQuery = null;
			if((XVar)((XVar)((XVar)(this.connection.dbType != Constants.nDATABASE_MySQL)  && (XVar)(this.connection.dbType != Constants.nDATABASE_MSSQLServer))  && (XVar)(this.connection.dbType != Constants.nDATABASE_Oracle))  && (XVar)(this.connection.dbType != Constants.nDATABASE_PostgreSQL))
			{
				return false;
			}
			if(XVar.Pack(this.pageObject.pSetEdit.hasEncryptedFields()))
			{
				return false;
			}
			sqlQuery = XVar.Clone(this.pageObject.pSetEdit.getSQLQueryByField((XVar)(this.field)));
			if(XVar.Pack(!(XVar)(sqlQuery)))
			{
				return false;
			}
			if((XVar)((XVar)(sqlQuery.HasGroupBy())  || (XVar)(sqlQuery.HavingToSql() != ""))  || (XVar)(sqlQuery.HasSubQueryInFromClause()))
			{
				return false;
			}
			return true;
		}
		protected virtual XVar isSearchByDispalyedFieldAllowed()
		{
			dynamic hostPageType = null;
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(this.searchByDisplayedFieldIsAllowed), XVar.Pack(null)))))
			{
				return this.searchByDisplayedFieldIsAllowed;
			}
			if(XVar.Pack(!(XVar)(this.lookupDataSource)))
			{
				this.searchByDisplayedFieldIsAllowed = new XVar(false);
				return this.searchByDisplayedFieldIsAllowed;
			}
			if(this.connection.connId != this.lookupDataSource.getConnectionId())
			{
				this.searchByDisplayedFieldIsAllowed = new XVar(false);
				return this.searchByDisplayedFieldIsAllowed;
			}
			if((XVar)(!(XVar)(this.connection.checkIfJoinSubqueriesOptimized()))  && (XVar)((XVar)(this.LCType == Constants.LCT_LIST)  || (XVar)(this.LCType == Constants.LCT_AJAX)))
			{
				this.searchByDisplayedFieldIsAllowed = new XVar(false);
				return this.searchByDisplayedFieldIsAllowed;
			}
			hostPageType = XVar.Clone(ProjectSettings.defaultPageType((XVar)(this.pageObject.pSetEdit.getEntityType())));
			this.searchByDisplayedFieldIsAllowed = XVar.Clone((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(hostPageType != "report")  && (XVar)(hostPageType != "chart"))  && (XVar)(!(XVar)(this.linkAndDisplaySame)))  && (XVar)(!(XVar)(this.multiselect)))  && (XVar)((XVar)((XVar)(this.LCType == Constants.LCT_LIST)  || (XVar)(this.LCType == Constants.LCT_DROPDOWN))  || (XVar)(this.LCType == Constants.LCT_AJAX)))  && (XVar)(this.lookupType != Constants.LT_LISTOFVALUES))  && (XVar)(this.isLookupSQLquerySimple()))  && (XVar)(this.isMainTableSQLquerySimple()));
			return this.searchByDisplayedFieldIsAllowed;
		}
		public override XVar checkIfDisplayFieldSearch(dynamic _param_strSearchOption)
		{
			#region pass-by-value parameters
			dynamic strSearchOption = XVar.Clone(_param_strSearchOption);
			#endregion

			return (XVar)(this.isSearchByDispalyedFieldAllowed())  && (XVar)((XVar)(XVar.Equals(XVar.Pack(strSearchOption), XVar.Pack("Starts with")))  || (XVar)(XVar.Equals(XVar.Pack(strSearchOption), XVar.Pack("Contains"))));
		}
		public override XVar getSelectColumnsAndJoinFromPart(dynamic _param_searchFor, dynamic _param_searchOpt, dynamic _param_isSuggest)
		{
			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic searchOpt = XVar.Clone(_param_searchOpt);
			dynamic isSuggest = XVar.Clone(_param_isSuggest);
			#endregion

			if((XVar)(!(XVar)(isSuggest))  || (XVar)(!(XVar)(this.isSearchByDispalyedFieldAllowed())))
			{
				return base.getSelectColumnsAndJoinFromPart((XVar)(searchFor), (XVar)(searchOpt), (XVar)(isSuggest));
			}
			this.Invoke("initializeLookupTableAliases");
			return new XVar("selectColumns", this.Invoke("getSelectColumns", (XVar)(isSuggest)), "joinFromPart", this.Invoke("getFromClauseJoinPart", (XVar)(searchFor), (XVar)(searchOpt), (XVar)(isSuggest)));
		}
		public override XVar getSearchOptions(dynamic _param_selOpt, dynamic _param_not, dynamic _param_both)
		{
			#region pass-by-value parameters
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			dynamic optionsArray = XVar.Array();
			optionsArray = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.multiselect))
			{
				optionsArray.InitAndSetArrayItem(Constants.CONTAINS, null);
			}
			else
			{
				if(this.lookupType == Constants.LT_QUERY)
				{
					this.ciphererLookup = XVar.Clone(new RunnerCipherer((XVar)(this.lookupTable)));
				}
				if(XVar.Pack(this.ciphererLookup))
				{
					this.isDisplayFieldEncrypted = XVar.Clone(this.ciphererLookup.isFieldPHPEncrypted((XVar)(this.displayFieldName)));
				}
				if((XVar)(this.LCType == Constants.LCT_AJAX)  && (XVar)(!(XVar)(this.isDisplayFieldEncrypted)))
				{
					if((XVar)(this.isSearchByDispalyedFieldAllowed())  || (XVar)(this.linkAndDisplaySame))
					{
						optionsArray.InitAndSetArrayItem(Constants.CONTAINS, null);
						optionsArray.InitAndSetArrayItem(Constants.STARTS_WITH, null);
					}
					optionsArray.InitAndSetArrayItem(Constants.MORE_THAN, null);
					optionsArray.InitAndSetArrayItem(Constants.LESS_THAN, null);
					optionsArray.InitAndSetArrayItem(Constants.BETWEEN, null);
				}
				if((XVar)((XVar)(this.LCType == Constants.LCT_LIST)  || (XVar)(this.LCType == Constants.LCT_DROPDOWN))  && (XVar)(this.isAdditionalControlRequired()))
				{
					optionsArray.InitAndSetArrayItem(Constants.CONTAINS, null);
					optionsArray.InitAndSetArrayItem(Constants.STARTS_WITH, null);
				}
			}
			optionsArray.InitAndSetArrayItem(Constants.EQUALS, null);
			optionsArray.InitAndSetArrayItem(Constants.EMPTY_SEARCH, null);
			if(XVar.Pack(both))
			{
				if(XVar.Pack(this.multiselect))
				{
					optionsArray.InitAndSetArrayItem(Constants.NOT_CONTAINS, null);
				}
				else
				{
					if((XVar)(this.LCType == Constants.LCT_AJAX)  && (XVar)(!(XVar)(this.isDisplayFieldEncrypted)))
					{
						if((XVar)(this.isSearchByDispalyedFieldAllowed())  || (XVar)(this.linkAndDisplaySame))
						{
							optionsArray.InitAndSetArrayItem(Constants.NOT_CONTAINS, null);
							optionsArray.InitAndSetArrayItem(Constants.NOT_STARTS_WITH, null);
						}
						optionsArray.InitAndSetArrayItem(Constants.NOT_MORE_THAN, null);
						optionsArray.InitAndSetArrayItem(Constants.NOT_LESS_THAN, null);
						optionsArray.InitAndSetArrayItem(Constants.NOT_BETWEEN, null);
					}
					if((XVar)((XVar)(this.LCType == Constants.LCT_LIST)  || (XVar)(this.LCType == Constants.LCT_DROPDOWN))  && (XVar)(this.isAdditionalControlRequired()))
					{
						optionsArray.InitAndSetArrayItem(Constants.NOT_CONTAINS, null);
						optionsArray.InitAndSetArrayItem(Constants.NOT_STARTS_WITH, null);
					}
				}
				optionsArray.InitAndSetArrayItem(Constants.NOT_EQUALS, null);
				optionsArray.InitAndSetArrayItem(Constants.NOT_EMPTY, null);
			}
			return this.buildSearchOptions((XVar)(optionsArray), (XVar)(selOpt), (XVar)(var_not), (XVar)(both));
		}
		public override XVar suggestValue(dynamic _param_value, dynamic _param_searchFor, dynamic var_response, dynamic row)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic searchFor = XVar.Clone(_param_searchFor);
			#endregion

			dynamic data = XVar.Array(), lookupDc = null, qResult = null;
			base.suggestValue((XVar)(value), (XVar)(searchFor), (XVar)(var_response), (XVar)(row));
			return null;
			if((XVar)(this.lookupType == Constants.LT_LISTOFVALUES)  || (XVar)(this.isSearchByDispalyedFieldAllowed()))
			{
				base.suggestValue((XVar)(value), (XVar)(searchFor), (XVar)(var_response), (XVar)(row));
				return null;
			}
			lookupDc = XVar.Clone(this.getLookupDataCommand((XVar)(XVar.Array()), (XVar)(MVCFunctions.substr((XVar)(value), new XVar(1))), new XVar(false), new XVar(true), new XVar(true), new XVar(true)));
			qResult = XVar.Clone(this.lookupDataSource.getList((XVar)(lookupDc)));
			if(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				dynamic lookup_value = null;
				if(XVar.Pack(this.isDisplayFieldEncrypted))
				{
					lookup_value = XVar.Clone(MVCFunctions.Concat("_", this.ciphererLookup.DecryptField((XVar)(this.displayFieldName), (XVar)(data[this.displayFieldAlias]))));
				}
				else
				{
					lookup_value = XVar.Clone(MVCFunctions.Concat("_", data[this.displayFieldAlias]));
				}
				base.suggestValue((XVar)(lookup_value), (XVar)(searchFor), (XVar)(var_response), (XVar)(row));
			}

			return null;
		}
		protected virtual XVar needCategoryFiltering(dynamic _param_parentValuesData)
		{
			#region pass-by-value parameters
			dynamic parentValuesData = XVar.Clone(_param_parentValuesData);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> cData in this.pageObject.pSetEdit.getParentFieldsData((XVar)(this.field)).GetEnumerator())
			{
				dynamic parentVals = XVar.Array(), parentValue = null, strCategoryControl = null;
				strCategoryControl = XVar.Clone(cData.Value["main"]);
				if(XVar.Pack(!(XVar)(parentValuesData.KeyExists(cData.Value["main"]))))
				{
					continue;
				}
				parentValue = XVar.Clone(parentValuesData[cData.Value["main"]]);
				parentVals = XVar.Clone((XVar.Pack(this.pageObject.pSetEdit.multiSelect((XVar)(strCategoryControl))) ? XVar.Pack(CommonFunctions.splitLookupValues((XVar)(parentValue))) : XVar.Pack(new XVar(0, parentValue))));
				foreach (KeyValuePair<XVar, dynamic> parentVal in parentVals.GetEnumerator())
				{
					if(XVar.Pack(MVCFunctions.strlen((XVar)(MVCFunctions.trim((XVar)(parentVal.Value))))))
					{
						return true;
					}
				}
			}
			return false;
		}
		public override XVar loadLookupContent(dynamic _param_parentValuesData, dynamic _param_childVal = null, dynamic _param_doCategoryFilter = null, dynamic _param_initialLoad = null)
		{
			#region default values
			if(_param_childVal as Object == null) _param_childVal = new XVar("");
			if(_param_doCategoryFilter as Object == null) _param_doCategoryFilter = new XVar(true);
			if(_param_initialLoad as Object == null) _param_initialLoad = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic parentValuesData = XVar.Clone(_param_parentValuesData);
			dynamic childVal = XVar.Clone(_param_childVal);
			dynamic doCategoryFilter = XVar.Clone(_param_doCategoryFilter);
			dynamic initialLoad = XVar.Clone(_param_initialLoad);
			#endregion

			dynamic lookupDc = null;
			if((XVar)(this.bUseCategory)  && (XVar)(doCategoryFilter))
			{
				if(XVar.Pack(!(XVar)(this.needCategoryFiltering((XVar)(parentValuesData)))))
				{
					return XVar.Array();
				}
			}
			lookupDc = XVar.Clone(this.getLookupDataCommand((XVar)(parentValuesData), (XVar)(childVal), (XVar)(doCategoryFilter), (XVar)((XVar)(this.LCType == Constants.LCT_AJAX)  && (XVar)(initialLoad))));
			return this.getLookupContentData((XVar)(lookupDc), (XVar)(childVal != XVar.Pack("")));
		}
		protected virtual XVar getLookupContentData(dynamic _param_lookupDc, dynamic _param_selectValue)
		{
			#region pass-by-value parameters
			dynamic lookupDc = XVar.Clone(_param_lookupDc);
			dynamic selectValue = XVar.Clone(_param_selectValue);
			#endregion

			dynamic data = XVar.Array(), keys = XVar.Array(), qResult = null, var_response = XVar.Array();
			var_response = XVar.Clone(XVar.Array());
			qResult = XVar.Clone(this.lookupDataSource.getList((XVar)(lookupDc)));
			if(XVar.Pack(!(XVar)(qResult)))
			{
				MVCFunctions.showError((XVar)(this.lookupDataSource.lastError()));
			}
			if((XVar)(!XVar.Equals(XVar.Pack(this.LCType), XVar.Pack(Constants.LCT_AJAX)))  || (XVar)(this.multiselect))
			{
				dynamic dispValue = null, isUnique = null, uniqueArray = XVar.Array();
				isUnique = XVar.Clone(this.pageObject.pSetEdit.isLookupUnique((XVar)(this.field)));
				uniqueArray = XVar.Clone(XVar.Array());
				while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
				{
					dispValue = XVar.Clone(data[this.displayFieldAlias]);
					if(XVar.Pack(isUnique))
					{
						if(XVar.Pack(MVCFunctions.in_array((XVar)(dispValue), (XVar)(uniqueArray))))
						{
							continue;
						}
						uniqueArray.InitAndSetArrayItem(dispValue, null);
					}
					keys = XVar.Clone(XVar.Array());
					if(XVar.Pack(this.lookupPSet))
					{
						foreach (KeyValuePair<XVar, dynamic> kf in this.lookupPSet.getTableKeys().GetEnumerator())
						{
							keys.InitAndSetArrayItem(data[kf.Value], null);
						}
					}
					var_response.InitAndSetArrayItem(new XVar("linkValue", data[this.linkFieldName], "displayValue", this.getLookupTextValue((XVar)(dispValue)), "keys", keys), null);
				}
			}
			else
			{
				data = XVar.Clone(qResult.fetchAssoc());
				if((XVar)(data)  && (XVar)((XVar)(selectValue)  || (XVar)(!(XVar)(qResult.fetchAssoc()))))
				{
					keys = XVar.Clone(XVar.Array());
					if(XVar.Pack(this.lookupPSet))
					{
						foreach (KeyValuePair<XVar, dynamic> kf in this.lookupPSet.getTableKeys().GetEnumerator())
						{
							keys.InitAndSetArrayItem(data[kf.Value], null);
						}
					}
					var_response.InitAndSetArrayItem(new XVar("linkValue", data[this.linkFieldName], "displayValue", this.getLookupTextValue((XVar)(data[this.displayFieldAlias])), "keys", keys), null);
				}
			}
			return var_response;
		}
		public override XVar getLookupContentToReload(dynamic _param_isExistParent, dynamic _param_mode, dynamic _param_parentCtrlsData)
		{
			#region pass-by-value parameters
			dynamic isExistParent = XVar.Clone(_param_isExistParent);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic parentCtrlsData = XVar.Clone(_param_parentCtrlsData);
			#endregion

			if(XVar.Pack(isExistParent))
			{
				dynamic hasEmptyParent = null;
				hasEmptyParent = new XVar(false);
				foreach (KeyValuePair<XVar, dynamic> value in parentCtrlsData.GetEnumerator())
				{
					if(XVar.Equals(XVar.Pack(value.Value), XVar.Pack("")))
					{
						hasEmptyParent = new XVar(true);
						break;
					}
				}
				if(XVar.Pack(!(XVar)(hasEmptyParent)))
				{
					return this.loadLookupContent((XVar)(parentCtrlsData), new XVar(""), new XVar(true), new XVar(false));
				}
				if((XVar)((XVar)((XVar)((XVar)(mode == Constants.MODE_SEARCH)  || (XVar)(mode == Constants.MODE_EDIT))  || (XVar)(mode == Constants.MODE_INLINE_EDIT))  || (XVar)(mode == Constants.MODE_INLINE_ADD))  || (XVar)(mode == Constants.MODE_ADD))
				{
					return "";
				}
				return this.loadLookupContent((XVar)(parentCtrlsData), new XVar(""), new XVar(true), new XVar(false));
			}
			else
			{
				if((XVar)((XVar)((XVar)((XVar)(mode == Constants.MODE_SEARCH)  || (XVar)(mode == Constants.MODE_INLINE_ADD))  || (XVar)(mode == Constants.MODE_ADD))  || (XVar)(mode == Constants.MODE_EDIT))  || (XVar)(mode == Constants.MODE_INLINE_EDIT))
				{
					return this.loadLookupContent((XVar)(XVar.Array()), new XVar(""), new XVar(false), new XVar(false));
				}
				return this.loadLookupContent((XVar)(XVar.Array()), new XVar(""), new XVar(true), new XVar(false));
			}
			return "";
		}
		public virtual XVar getAutoFillData(dynamic _param_linkFieldVal)
		{
			#region pass-by-value parameters
			dynamic linkFieldVal = XVar.Clone(_param_linkFieldVal);
			#endregion

			dynamic autoCompleteFields = XVar.Array(), data = XVar.Array(), lookupDc = null, masterData = XVar.Array(), ret = XVar.Array(), row = XVar.Array(), rs = null;
			data = XVar.Clone(XVar.Array());
			lookupDc = XVar.Clone(this.getLookupDataCommand((XVar)(XVar.Array()), (XVar)(linkFieldVal), new XVar(false), new XVar(true), new XVar(true)));
			rs = XVar.Clone(this.lookupDataSource.getList((XVar)(lookupDc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				MVCFunctions.showError((XVar)(this.lookupDataSource.lastError()));
			}
			row = XVar.Clone(rs.fetchAssoc());
			autoCompleteFields = XVar.Clone(this.pageObject.pSetEdit.getAutoCompleteFields((XVar)(this.field)));
			if(this.lookupType == Constants.LT_QUERY)
			{
				data = XVar.Clone(this.ciphererLookup.DecryptFetchedArray((XVar)(row)));
			}
			else
			{
				foreach (KeyValuePair<XVar, dynamic> aData in autoCompleteFields.GetEnumerator())
				{
					data.InitAndSetArrayItem(row[aData.Value["lookupField"]], aData.Value["lookupField"]);
				}
			}
			ret = XVar.Clone(XVar.Array());
			masterData = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> aData in autoCompleteFields.GetEnumerator())
			{
				dynamic ctrl = null, dispValue = null, fieldData = XVar.Array(), val = null;
				masterData.InitAndSetArrayItem(data[aData.Value["lookupField"]], aData.Value["masterField"]);
				fieldData = XVar.Clone(XVar.Array());
				val = XVar.Clone(data[aData.Value["lookupField"]]);
				ctrl = XVar.Clone(this.pageObject.getControl((XVar)(aData.Value["masterField"])));
				dispValue = XVar.Clone(ctrl.getDisplayValue((XVar)(masterData)));
				if(ctrl.format == Constants.EDIT_FORMAT_READONLY)
				{
					fieldData.InitAndSetArrayItem(val, "value");
					fieldData.InitAndSetArrayItem(dispValue, "dispValue");
				}
				else
				{
					fieldData.InitAndSetArrayItem(dispValue, "value");
				}
				ret.InitAndSetArrayItem(fieldData, aData.Value["lookupField"]);
			}
			if(XVar.Pack(!(XVar)(ret)))
			{
				ret.InitAndSetArrayItem("", this.field);
			}
			return ret;
		}
		public override XVar getInputStyle(dynamic _param_mode)
		{
			#region pass-by-value parameters
			dynamic mode = XVar.Clone(_param_mode);
			#endregion

			return "class='form-control'";
		}
		protected virtual XVar getLookupLinks(dynamic _param_hiddenSelect = null)
		{
			#region default values
			if(_param_hiddenSelect as Object == null) _param_hiddenSelect = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic hiddenSelect = XVar.Clone(_param_hiddenSelect);
			#endregion

			dynamic links = XVar.Array();
			links = XVar.Clone(XVar.Array());
			if(this.LCType == Constants.LCT_LIST)
			{
				dynamic openId = null, visibility = null;
				visibility = XVar.Clone((XVar.Pack(hiddenSelect) ? XVar.Pack(" style=\"visibility: hidden;\"") : XVar.Pack("")));
				openId = XVar.Clone(MVCFunctions.Concat("open_lookup_", MVCFunctions.GoodFieldName((XVar)(this.field)), "_", this.id));
				links.InitAndSetArrayItem(MVCFunctions.Concat("<a href=\"#\" id=\"", openId, "\" ", visibility, ">", CommonFunctions.mlang_message(new XVar("SELECT_LIST")), "</a>"), null);
			}
			if(XVar.Pack(this.addNewItem))
			{
				links.InitAndSetArrayItem(MVCFunctions.Concat("<a href=\"#\" id=\"addnew_", this.cfield, "\">", CommonFunctions.mlang_message(new XVar("ADD_NEW")), "</a>"), null);
			}
			if(XVar.Pack(this.editLookupItem))
			{
				links.InitAndSetArrayItem(MVCFunctions.Concat("<a href=\"#\" class=\"hidden-link\" id=\"edit_lookup_item_", this.cfield, "\">", CommonFunctions.mlang_message(new XVar("MODIFY")), "</a>"), null);
			}
			if((XVar)(this.LCType == Constants.LCT_LIST)  && (XVar)(this.multiselect))
			{
				dynamic clearId = null;
				clearId = XVar.Clone(MVCFunctions.Concat("clearLookup_", MVCFunctions.GoodFieldName((XVar)(this.field)), "_", this.id));
				links.InitAndSetArrayItem(MVCFunctions.Concat("<a href=\"#\" id=\"", clearId, "\" style=\"visibility: hidden;\">", CommonFunctions.mlang_message(new XVar("FILTER_CLEAR")), "</a>"), null);
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.count(links))))
			{
				return "";
			}
			return MVCFunctions.Concat("<div class=\"bs-lookup-links\">", MVCFunctions.implode(new XVar(""), (XVar)(links)), "</div>");
		}
		public override XVar getBasicFieldCondition(dynamic _param_searchFor, dynamic _param_strSearchOption, dynamic _param_searchFor2 = null, dynamic _param_etype = null)
		{
			#region default values
			if(_param_searchFor2 as Object == null) _param_searchFor2 = new XVar("");
			if(_param_etype as Object == null) _param_etype = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic strSearchOption = XVar.Clone(_param_strSearchOption);
			dynamic searchFor2 = XVar.Clone(_param_searchFor2);
			dynamic etype = XVar.Clone(_param_etype);
			#endregion

			if(strSearchOption == Constants.EMPTY_SEARCH)
			{
				return base.getBasicFieldCondition((XVar)(searchFor), (XVar)(strSearchOption), (XVar)(searchFor2));
			}
			if(XVar.Pack(!(XVar)(this.multiselect)))
			{
				return this.singleValueCondition((XVar)(searchFor), (XVar)(strSearchOption), (XVar)(searchFor2));
			}
			else
			{
				return this.multiValueCondition((XVar)(searchFor), (XVar)(strSearchOption), (XVar)(searchFor2));
			}

			return null;
		}
		public virtual XVar multiValueCondition(dynamic _param_searchFor, dynamic _param_strSearchOption, dynamic _param_searchFor2 = null)
		{
			#region default values
			if(_param_searchFor2 as Object == null) _param_searchFor2 = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic strSearchOption = XVar.Clone(_param_strSearchOption);
			dynamic searchFor2 = XVar.Clone(_param_searchFor2);
			#endregion

			dynamic values = XVar.Array();
			values = XVar.Clone(CommonFunctions.splitLookupValues((XVar)(searchFor)));
			if(XVar.Pack(!(XVar)(this.pageObject.pSetEdit.multiSelectLookupEdit((XVar)(this.field)))))
			{
				dynamic conditions = XVar.Array();
				conditions = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
				{
					conditions.InitAndSetArrayItem(this.singleValueCondition((XVar)(v.Value), (XVar)(strSearchOption), (XVar)(searchFor2)), null);
				}
				return DataCondition._Or((XVar)(conditions));
			}
			if(strSearchOption == Constants.EQUALS)
			{
				return DataCondition.FieldHasList((XVar)(this.field), new XVar(Constants.dsopALL_IN_LIST), (XVar)(values));
			}
			if(strSearchOption == Constants.CONTAINS)
			{
				return DataCondition.FieldHasList((XVar)(this.field), new XVar(Constants.dsopSOME_IN_LIST), (XVar)(values));
			}
			return null;
		}
		public virtual XVar singleValueCondition(dynamic _param_searchFor, dynamic _param_strSearchOption, dynamic _param_searchFor2 = null)
		{
			#region default values
			if(_param_searchFor2 as Object == null) _param_searchFor2 = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic strSearchOption = XVar.Clone(_param_strSearchOption);
			dynamic searchFor2 = XVar.Clone(_param_searchFor2);
			#endregion

			dynamic cond = null;
			cond = XVar.Clone(base.getBasicFieldCondition((XVar)(searchFor), (XVar)(strSearchOption), (XVar)(searchFor2)));
			if(XVar.Pack(this.displayFieldSearch((XVar)(strSearchOption))))
			{
				cond.operands[0].joinData = XVar.Clone(this.createJoinData());
			}
			return cond;
		}
		protected virtual XVar displayFieldSearch(dynamic _param_searchOption)
		{
			#region pass-by-value parameters
			dynamic searchOption = XVar.Clone(_param_searchOption);
			#endregion

			if((XVar)(!XVar.Equals(XVar.Pack(searchOption), XVar.Pack(Constants.CONTAINS)))  && (XVar)(!XVar.Equals(XVar.Pack(searchOption), XVar.Pack(Constants.STARTS_WITH))))
			{
				return false;
			}
			if((XVar)(this.linkAndDisplaySame)  || (XVar)(this.lookupType == Constants.LT_LISTOFVALUES))
			{
				return false;
			}
			if((XVar)(this.multiselect)  && (XVar)(this.pageObject.pSetEdit.multiSelectLookupEdit((XVar)(this.field))))
			{
				return false;
			}
			return true;
		}
		protected virtual XVar createJoinData()
		{
			dynamic jd = null;
			jd = XVar.Clone(new DsJoinData());
			jd.dataSource = XVar.Clone(this.lookupDataSource);
			jd.linkField = XVar.Clone(this.linkFieldName);
			if(XVar.Pack(this.customDisplay))
			{
				jd.displayExpression = XVar.Clone(this.displayFieldName);
			}
			else
			{
				jd.displayField = XVar.Clone(this.displayFieldName);
			}
			jd.longList = XVar.Clone((XVar)(this.LCType == Constants.LCT_AJAX)  || (XVar)(this.LCType == Constants.LCT_LIST));
			jd.displayAlias = XVar.Clone(CommonFunctions.generateAlias());
			return jd;
		}
		protected virtual XVar getLookupDataCommand(dynamic _param_parentValuesData, dynamic _param_value = null, dynamic _param_doCategoryFilter = null, dynamic _param_doValueFilter = null, dynamic _param_doWhereFilter = null, dynamic _param_oneRecordMode = null)
		{
			#region default values
			if(_param_value as Object == null) _param_value = new XVar("");
			if(_param_doCategoryFilter as Object == null) _param_doCategoryFilter = new XVar(true);
			if(_param_doValueFilter as Object == null) _param_doValueFilter = new XVar(false);
			if(_param_doWhereFilter as Object == null) _param_doWhereFilter = new XVar(true);
			if(_param_oneRecordMode as Object == null) _param_oneRecordMode = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic parentValuesData = XVar.Clone(_param_parentValuesData);
			dynamic value = XVar.Clone(_param_value);
			dynamic doCategoryFilter = XVar.Clone(_param_doCategoryFilter);
			dynamic doValueFilter = XVar.Clone(_param_doValueFilter);
			dynamic doWhereFilter = XVar.Clone(_param_doWhereFilter);
			dynamic oneRecordMode = XVar.Clone(_param_oneRecordMode);
			#endregion

			dynamic ret = XVar.Array();
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.pageObject.pSetEdit);
			ret = XVar.Clone(LookupField.makeLookupDataCommand((XVar)(this.field), (XVar)(pSet), (XVar)(parentValuesData), (XVar)(value), (XVar)(doCategoryFilter), (XVar)(doValueFilter), (XVar)(doWhereFilter), (XVar)(oneRecordMode)));
			this.displayFieldAlias = XVar.Clone(ret["displayField"]);
			return ret["dc"];
		}
		public static XVar makeLookupDataCommand(dynamic _param_field, dynamic _param_pSet_packed, dynamic _param_parentValuesData, dynamic _param_value, dynamic _param_doCategoryFilter, dynamic _param_doValueFilter, dynamic _param_doWhereFilter, dynamic _param_oneRecordMode)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic parentValuesData = XVar.Clone(_param_parentValuesData);
			dynamic value = XVar.Clone(_param_value);
			dynamic doCategoryFilter = XVar.Clone(_param_doCategoryFilter);
			dynamic doValueFilter = XVar.Clone(_param_doValueFilter);
			dynamic doWhereFilter = XVar.Clone(_param_doWhereFilter);
			dynamic oneRecordMode = XVar.Clone(_param_oneRecordMode);
			#endregion

			dynamic dc = null, displayField = null, displayFieldAlias = null, filters = XVar.Array(), orderField = null, parents = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			orderField = XVar.Clone(pSet.getLookupOrderBy((XVar)(field)));
			if(XVar.Pack(orderField))
			{
				dynamic dir = null;
				dir = XVar.Clone((XVar.Pack(pSet.isLookupDesc((XVar)(field))) ? XVar.Pack("DESC") : XVar.Pack("ASC")));
				dc.order.InitAndSetArrayItem(new XVar("column", orderField, "dir", dir), null);
			}
			else
			{
				dynamic lookupType = null;
				lookupType = XVar.Clone(pSet.getLookupType((XVar)(field)));
				if(lookupType == Constants.LT_QUERY)
				{
					dynamic lookupTable = null;
					lookupTable = XVar.Clone(pSet.getLookupTable((XVar)(field)));
					dc.order = XVar.Clone(OrderClause.originalOrderFields((XVar)(new ProjectSettings((XVar)(lookupTable)))));
				}
			}
			displayField = XVar.Clone(pSet.getDisplayField((XVar)(field)));
			if(XVar.Pack(pSet.getCustomDisplay((XVar)(field))))
			{
				displayFieldAlias = XVar.Clone(CommonFunctions.generateAlias());
				dc.extraColumns.InitAndSetArrayItem(new DsFieldData((XVar)(displayField), (XVar)(displayFieldAlias), new XVar("")), null);
			}
			else
			{
				displayFieldAlias = XVar.Clone(displayField);
			}
			filters = XVar.Clone(XVar.Array());
			if(XVar.Pack(doValueFilter))
			{
				if((XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack("")))  || (XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack(null))))
				{
					filters.InitAndSetArrayItem(DataCondition._False(), null);
				}
				else
				{
					dynamic linkField = null, multiselect = null;
					linkField = XVar.Clone(pSet.getLinkField((XVar)(field)));
					multiselect = XVar.Clone(pSet.multiSelect((XVar)(field)));
					if(XVar.Pack(!(XVar)(multiselect)))
					{
						filters.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(linkField), (XVar)(value)), null);
					}
					else
					{
						dynamic valueConditions = XVar.Array(), values = XVar.Array();
						values = XVar.Clone(CommonFunctions.splitLookupValues((XVar)(value)));
						valueConditions = XVar.Clone(XVar.Array());
						foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
						{
							valueConditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(linkField), (XVar)(v.Value)), null);
						}
						filters.InitAndSetArrayItem(DataCondition._Or((XVar)(valueConditions)), null);
					}
				}
			}
			if(XVar.Pack(doWhereFilter))
			{
				filters.InitAndSetArrayItem(DataCondition.SQLCondition((XVar)(CommonFunctions.prepareLookupWhere((XVar)(field), (XVar)(pSet)))), null);
			}
			if(pSet.getLookupType((XVar)(field)) == Constants.LT_QUERY)
			{
				filters.InitAndSetArrayItem(Security.SelectCondition(new XVar("S"), (XVar)(new ProjectSettings((XVar)(pSet.getLookupTable((XVar)(field))))), new XVar(true)), null);
			}
			parents = XVar.Clone(pSet.getParentFieldsData((XVar)(field)));
			if((XVar)((XVar)(doCategoryFilter)  && (XVar)(parentValuesData))  && (XVar)(parents))
			{
				dynamic parentFilters = null;
				parentFilters = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> cdata in parents.GetEnumerator())
				{
					dynamic filterFieldName = null, mainControlName = null, mainControlVal = null;
					mainControlName = XVar.Clone(cdata.Value["main"]);
					filterFieldName = XVar.Clone(cdata.Value["lookup"]);
					mainControlVal = XVar.Clone(parentValuesData[mainControlName]);
					if((XVar)(pSet.multiSelect((XVar)(mainControlName)))  || (XVar)(MVCFunctions.strlen((XVar)(mainControlVal))))
					{
						filters.InitAndSetArrayItem(LookupField.categoryCondition((XVar)(pSet), (XVar)(mainControlName), (XVar)(filterFieldName), (XVar)(mainControlVal)), null);
					}
				}
			}
			if(XVar.Pack(pSet.isLookupUnique((XVar)(field))))
			{
				dc.totals.InitAndSetArrayItem(new XVar("field", pSet.getLinkField((XVar)(field)), "total", "distinct"), null);
				dc.totals.InitAndSetArrayItem(new XVar("field", displayFieldAlias, "total", "distinct"), null);
			}
			dc.filter = XVar.Clone(DataCondition._And((XVar)(filters)));
			if(XVar.Pack(oneRecordMode))
			{
				dc.reccount = new XVar(1);
			}
			return new XVar("dc", dc, "displayField", displayFieldAlias);
		}
		public static XVar categoryCondition(dynamic _param_pSet_packed, dynamic _param_parentControlName, dynamic _param_filterFieldName, dynamic _param_parentControlValue)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic parentControlName = XVar.Clone(_param_parentControlName);
			dynamic filterFieldName = XVar.Clone(_param_filterFieldName);
			dynamic parentControlValue = XVar.Clone(_param_parentControlValue);
			#endregion

			dynamic conditions = XVar.Array(), values = XVar.Array();
			if(XVar.Pack(!(XVar)(pSet.multiSelect((XVar)(parentControlName)))))
			{
				return DataCondition.FieldEquals((XVar)(filterFieldName), (XVar)(parentControlValue));
			}
			values = XVar.Clone(CommonFunctions.splitLookupValues((XVar)(parentControlValue)));
			conditions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in values.GetEnumerator())
			{
				conditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(filterFieldName), (XVar)(value.Value)), null);
			}
			return DataCondition._Or((XVar)(conditions));
		}
		public override XVar getSuggestCommand(dynamic _param_searchFor, dynamic _param_searchOpt, dynamic _param_numberOfSuggests)
		{
			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic searchOpt = XVar.Clone(_param_searchOpt);
			dynamic numberOfSuggests = XVar.Clone(_param_numberOfSuggests);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(base.getSuggestCommand((XVar)(searchFor), (XVar)(searchOpt), (XVar)(numberOfSuggests)));
			if(XVar.Pack(this.displayFieldSearch((XVar)(searchOpt))))
			{
				dynamic displayAlias = null;
				displayAlias = XVar.Clone(CommonFunctions.generateAlias());
				dc.extraColumns.InitAndSetArrayItem(new DsFieldData(new XVar(""), (XVar)(displayAlias), (XVar)(this.field), new XVar(0), (XVar)(this.createJoinData())), null);
				dc.totals = XVar.Clone(new XVar(0, new XVar("field", displayAlias, "total", "distinct")));
			}
			return dc;
		}
		protected virtual XVar getLookupTextValue(dynamic _param_displayValue)
		{
			#region pass-by-value parameters
			dynamic displayValue = XVar.Clone(_param_displayValue);
			#endregion

			if(this.pageObject.pSetEdit.getViewFormat((XVar)(this.field)) == Constants.FORMAT_HTML)
			{
				if((XVar)(this.LCType == Constants.LCT_CBLIST)  || (XVar)(this.LCType == Constants.LCT_RADIO))
				{
					return displayValue;
				}
			}
			return MVCFunctions.runner_htmlspecialchars((XVar)(displayValue));
		}
		protected virtual XVar getDataKeysAttr(ref dynamic data)
		{
			dynamic keys = XVar.Array();
			if(XVar.Pack(!(XVar)(data)))
			{
				return "";
			}
			keys = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.lookupPSet))
			{
				foreach (KeyValuePair<XVar, dynamic> kf in this.lookupPSet.getTableKeys().GetEnumerator())
				{
					keys.InitAndSetArrayItem(data[kf.Value], null);
				}
			}
			if(XVar.Pack(MVCFunctions.count(keys)))
			{
				return MVCFunctions.Concat("data-keys=\"", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.runner_json_encode((XVar)(keys)))), "\"");
			}
			return "";
		}
	}
}
