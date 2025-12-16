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
	public partial class ViewControl : XClass
	{
		public dynamic field = XVar.Pack("");
		public dynamic displayField = XVar.Pack(null);
		public dynamic container = XVar.Pack(null);
		public dynamic pageObject = XVar.Pack(null);
		public dynamic is508 = XVar.Pack(false);
		public dynamic fieldType = XVar.Pack(null);
		public dynamic viewFormat = XVar.Pack(Constants.FORMAT_NONE);
		public dynamic editFormat = XVar.Pack(Constants.EDIT_FORMAT_NONE);
		public dynamic localControlsContainer = XVar.Pack(null);
		public dynamic linkAndDisplaySame = XVar.Pack(null);
		public dynamic searchClauseObj = XVar.Pack(null);
		public dynamic settings = XVar.Array();
		public dynamic viewControlsMap = XVar.Array();
		protected dynamic userControl = XVar.Pack(false);
		public dynamic searchHighlight = XVar.Pack(false);
		public dynamic isUsedForFilter = XVar.Pack(false);
		protected dynamic needLookupValueProcessing = XVar.Pack(true);
		protected dynamic isFieldLookup = XVar.Pack(false);
		protected dynamic useUTF8 = XVar.Pack(false);
		public virtual XVar addJSFiles()
		{

			return null;
		}
		public virtual XVar addCSSFiles()
		{

			return null;
		}
		public virtual XVar AddCSSFile(dynamic _param_fileName)
		{
			#region pass-by-value parameters
			dynamic fileName = XVar.Clone(_param_fileName);
			#endregion

			this.getContainer().AddCSSFile((XVar)(fileName));

			return null;
		}
		public virtual XVar AddJSFile(dynamic _param_fileName, dynamic _param_req1 = null, dynamic _param_req2 = null, dynamic _param_req3 = null)
		{
			#region default values
			if(_param_req1 as Object == null) _param_req1 = new XVar("");
			if(_param_req2 as Object == null) _param_req2 = new XVar("");
			if(_param_req3 as Object == null) _param_req3 = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic fileName = XVar.Clone(_param_fileName);
			dynamic req1 = XVar.Clone(_param_req1);
			dynamic req2 = XVar.Clone(_param_req2);
			dynamic req3 = XVar.Clone(_param_req3);
			#endregion

			this.getContainer().AddJSFile((XVar)(fileName), (XVar)(req1), (XVar)(req2), (XVar)(req3));

			return null;
		}
		public virtual dynamic pSettings()
		{
			return this.getContainer().pSet;
		}
		public virtual dynamic getContainer()
		{
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(this.pageObject), XVar.Pack(null)))))
			{
				return this.pageObject;
			}
			else
			{
				return this.container;
			}

			return null;
		}
		public ViewControl(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject = null)
		{
			#region default values
			if(_param_pageObject as Object == null) _param_pageObject = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic container = XVar.Clone(_param_container);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			this.useUTF8 = XVar.Clone(XVar.Equals(XVar.Pack(ProjectSettings.getProjectValue(new XVar("charset"))), XVar.Pack("utf-8")));
			this.field = XVar.Clone(field);
			this.container = XVar.Clone(container);
			this.pageObject = XVar.Clone(pageObject);
			this.is508 = XVar.Clone(CommonFunctions.isEnableSection508());
			this.fieldType = XVar.Clone(container.pSet.getFieldType((XVar)(this.field)));
			this.viewFormat = XVar.Clone(container.pSet.getViewFormat((XVar)(this.field)));
			this.editFormat = XVar.Clone(container.pSet.getEditFormat((XVar)(this.field)));
			if(XVar.Pack(this.pageObject))
			{
				this.searchClauseObj = XVar.Clone(this.pageObject.searchClauseObj);
				if(XVar.Pack(this.searchClauseObj))
				{
					this.searchHighlight = XVar.Clone((XVar)(container.searchHighlight)  && (XVar)(this.searchClauseObj.searchStarted()));
				}
			}
		}
		public virtual XVar getExportValue(dynamic data, dynamic _param_keylink = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			if(_param_html as Object == null) _param_html = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			return this.showDBValue((XVar)(data), (XVar)(keylink), (XVar)(html));
		}
		public virtual XVar getPdfValue(dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			return MVCFunctions.Concat("'", CommonFunctions.jsreplace((XVar)(this.showDBValue((XVar)(data), (XVar)(keylink), new XVar(false)))), "'");
		}
		public virtual XVar showDBValue(dynamic data, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic processedText = null, value = null;
			value = XVar.Clone(data[this.field]);
			if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(this.fieldType))))
			{
				value = XVar.Clone(CommonFunctions.mlang_message(new XVar("LONG_BINARY")));
				this.searchHighlight = new XVar(false);
			}
			if(XVar.Equals(XVar.Pack(value), XVar.Pack(false)))
			{
				value = new XVar("");
			}
			if((XVar)(this.editFormat == Constants.EDIT_FORMAT_CHECKBOX)  && (XVar)(this.viewFormat == Constants.FORMAT_NONE))
			{
				if((XVar)(value)  && (XVar)(value != XVar.Pack(0)))
				{
					value = XVar.Clone(CommonFunctions.mlang_message(new XVar("YES")));
				}
				else
				{
					value = XVar.Clone(CommonFunctions.mlang_message(new XVar("NO")));
				}
				this.searchHighlight = new XVar(false);
			}
			if((XVar)(this.container.forExport == "excel")  || (XVar)(this.container.forExport == "csv"))
			{
				return value;
			}
			processedText = XVar.Clone(this.processText((XVar)(value), (XVar)(keylink), (XVar)(html)));
			if(XVar.Pack(html))
			{
				return MVCFunctions.nl2br((XVar)(processedText));
			}
			return processedText;
		}
		public virtual XVar getTextValue(dynamic data)
		{
			return data[this.field];
		}
		public virtual XVar processText(dynamic _param_value, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic cNumberOfChars = null, inlineOrFlyMode = null, isDetailPreview = null, isListPage = null, isPagePrint = null, isReportPage = null, mode = null, needShortening = null, pageType = null;
			inlineOrFlyMode = new XVar(false);
			pageType = XVar.Clone(this.container.pageType);
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(this.pageObject), XVar.Pack(null)))))
			{
				mode = XVar.Clone(this.pageObject.mode);
				inlineOrFlyMode = XVar.Clone((XVar)((XVar)(pageType == Constants.PAGE_EDIT)  && (XVar)((XVar)(mode == Constants.EDIT_INLINE)  || (XVar)(mode == Constants.EDIT_POPUP)))  || (XVar)((XVar)(pageType == Constants.PAGE_ADD)  && (XVar)((XVar)(mode == Constants.ADD_INLINE)  || (XVar)(mode == Constants.ADD_POPUP))));
			}
			isDetailPreview = XVar.Clone(this.container.isDetailsPreview);
			if((XVar)(pageType == Constants.PAGE_ADD)  || (XVar)(pageType == Constants.PAGE_EDIT))
			{
				pageType = new XVar(Constants.PAGE_VIEW);
			}
			isPagePrint = XVar.Clone((XVar)((XVar)((XVar)(pageType == Constants.PAGE_RPRINT)  && (XVar)(this.container.forExport))  || (XVar)(pageType == Constants.PAGE_PRINT))  || (XVar)(pageType == Constants.PAGE_RPRINT));
			if(this.editFormat == Constants.EDIT_FORMAT_LOOKUP_WIZARD)
			{
				this.isFieldLookup = new XVar(true);
				this.needLookupValueProcessing = XVar.Clone(this.checkIfLookupValueIsToProcess());
				value = XVar.Clone(this.processMultiselectLWValue((XVar)(value)));
			}
			cNumberOfChars = XVar.Clone(this.container.pSet.getNumberOfChars((XVar)(this.field)));
			needShortening = XVar.Clone(this.textNeedsTruncating((XVar)(value), (XVar)(cNumberOfChars)));
			isReportPage = XVar.Clone((XVar)(pageType == Constants.PAGE_REPORT)  || (XVar)(pageType == Constants.PAGE_MASTER_INFO_REPORT));
			isListPage = XVar.Clone((XVar)(pageType == Constants.PAGE_LIST)  || (XVar)(pageType == Constants.PAGE_MASTER_INFO_LIST));
			if((XVar)((XVar)((XVar)((XVar)(html)  && (XVar)(needShortening))  && (XVar)((XVar)((XVar)(isListPage)  || (XVar)(isReportPage))  || (XVar)(inlineOrFlyMode)))  && (XVar)(!(XVar)(isDetailPreview)))  && (XVar)(keylink != XVar.Pack("")))
			{
				return this.getShorteningTextAndMoreLink((XVar)(value), (XVar)(cNumberOfChars), (XVar)(keylink), (XVar)(mode));
			}
			if((XVar)(needShortening)  && (XVar)((XVar)(isPagePrint)  || (XVar)(isDetailPreview)))
			{
				return this.getShorteningText((XVar)(value), (XVar)(cNumberOfChars), (XVar)(html));
			}
			return this.getText((XVar)(value), (XVar)(html));
		}
		protected virtual XVar textNeedsTruncating(dynamic _param_value, dynamic _param_cNumberOfChars)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			#endregion

			return (XVar)((XVar)((XVar)(!(XVar)(this.isUsedForFilter))  && (XVar)(!(XVar)(this.container.fullText)))  && (XVar)(XVar.Pack(0) < cNumberOfChars))  && (XVar)(cNumberOfChars < MVCFunctions.runner_strlen((XVar)(value)));
		}
		protected virtual XVar getShorteningText(dynamic _param_value, dynamic _param_cNumberOfChars, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic tail = null;
			tail = new XVar("...");
			if(XVar.Pack(html))
			{
				tail = new XVar("&nbsp;...");
			}
			return MVCFunctions.Concat(CommonFunctions.GetShorteningForLargeText((XVar)(value), (XVar)(cNumberOfChars), (XVar)(html)), tail);
		}
		protected virtual XVar getShorteningTextAndMoreLink(dynamic _param_value, dynamic _param_cNumberOfChars, dynamic _param_keylink, dynamic _param_mode)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic mode = XVar.Clone(_param_mode);
			#endregion

			dynamic dataField = null, label = null, tName = null, truncatedValue = null, var_params = XVar.Array();
			truncatedValue = XVar.Clone(CommonFunctions.GetShorteningForLargeText((XVar)(value), (XVar)(cNumberOfChars)));
			if(XVar.Pack(this.searchHighlight))
			{
				truncatedValue = XVar.Clone(this.highlightTruncatedBeforeMore((XVar)(value), (XVar)(truncatedValue), (XVar)(cNumberOfChars), (XVar)(cNumberOfChars)));
			}
			tName = XVar.Clone(this.getContainer().tName);
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(MVCFunctions.Concat("pagetype=", this.container.pSet._viewPage), null);
			var_params.InitAndSetArrayItem(MVCFunctions.Concat("table=", CommonFunctions.GetTableURL((XVar)(tName))), null);
			var_params.InitAndSetArrayItem(MVCFunctions.Concat("field=", MVCFunctions.RawUrlEncode((XVar)(this.field))), null);
			var_params.InitAndSetArrayItem(MVCFunctions.substr((XVar)(keylink), new XVar(1)), null);
			var_params.InitAndSetArrayItem(MVCFunctions.Concat("page=", this.container.pSet.pageName()), null);
			if(mode == Constants.LIST_DASHBOARD)
			{
				var_params.InitAndSetArrayItem(MVCFunctions.Concat("mode=", mode), null);
			}
			if(mode == Constants.LIST_LOOKUP)
			{
				var_params.InitAndSetArrayItem(MVCFunctions.Concat("maintable=", this.pageObject.mainTable), null);
				var_params.InitAndSetArrayItem(MVCFunctions.Concat("mainfield=", this.pageObject.mainField), null);
			}
			label = XVar.Clone(this.container.pSet.label((XVar)(this.field)));
			dataField = XVar.Clone(MVCFunctions.Concat("data-fieldlabel=\"", MVCFunctions.runner_htmlspecialchars((XVar)(label)), "\""));
			return MVCFunctions.Concat(truncatedValue, " <a href=\"javascript:void(0);\" data-gridlink data-query=\"", MVCFunctions.GetTableLink(new XVar("fulltext"), new XVar(""), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(var_params)))), "\" ", dataField, ">", CommonFunctions.mlang_message(new XVar("MORE")), "&nbsp;...</a>");
		}
		protected virtual XVar getText(dynamic _param_value, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			if(XVar.Pack(!(XVar)(html)))
			{
				return value;
			}
			value = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(value)));
			if(XVar.Pack(this.searchHighlight))
			{
				value = XVar.Clone(this.highlightSearchWord((XVar)(value), new XVar(true), new XVar("")));
			}
			return value;
		}
		protected virtual XVar checkIfLookupValueIsToProcess()
		{
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(this.container.pSet);
			if((XVar)((XVar)((XVar)(pSet.getLookupType((XVar)(this.field)) == Constants.LT_LOOKUPTABLE)  || (XVar)(pSet.getLookupType((XVar)(this.field)) == Constants.LT_QUERY))  && (XVar)(pSet.getLinkField((XVar)(this.field)) == pSet.getDisplayField((XVar)(this.field))))  && (XVar)(pSet.multiSelect((XVar)(this.field))))
			{
				return true;
			}
			return false;
		}
		protected virtual XVar processMultiselectLWValue(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(XVar.Pack(!(XVar)(this.needLookupValueProcessing)))
			{
				return value;
			}
			return MVCFunctions.implode(new XVar(","), (XVar)(CommonFunctions.splitLookupValues((XVar)(value))));
		}
		public virtual XVar highlightSearchWord(dynamic _param_value, dynamic _param_encoded, dynamic _param_dbValue = null)
		{
			#region default values
			if(_param_dbValue as Object == null) _param_dbValue = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic encoded = XVar.Clone(_param_encoded);
			dynamic dbValue = XVar.Clone(_param_dbValue);
			#endregion

			dynamic highlightData = null, lookupParams = null;
			if(dbValue == XVar.Pack(""))
			{
				dbValue = XVar.Clone(value);
			}
			lookupParams = XVar.Clone(this.getLookupParams());
			highlightData = XVar.Clone(this.searchClauseObj.getSearchHighlightingData((XVar)(this.field), (XVar)(dbValue), (XVar)(encoded), (XVar)(lookupParams)));
			if(XVar.Pack(!(XVar)(highlightData)))
			{
				return value;
			}
			return this.getValueHighlighted((XVar)(value), (XVar)(highlightData));
		}
		public virtual XVar highlightSearchWordForNumber(dynamic _param_value, dynamic _param_encoded, dynamic _param_dbValue)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic encoded = XVar.Clone(_param_encoded);
			dynamic dbValue = XVar.Clone(_param_dbValue);
			#endregion

			dynamic highlightData = null, lookupParams = null;
			lookupParams = XVar.Clone(this.getLookupParams());
			highlightData = XVar.Clone(this.searchClauseObj.getSearchHighlightingData((XVar)(this.field), (XVar)(dbValue), (XVar)(encoded), (XVar)(lookupParams), new XVar(true)));
			if(XVar.Pack(highlightData))
			{
				return this.getValueHighlighted((XVar)(value), (XVar)(highlightData));
			}
			return value;
		}
		public virtual XVar getValueHighlighted(dynamic _param_value, dynamic _param_highlightData)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic highlightData = XVar.Clone(_param_highlightData);
			#endregion

			dynamic pattern = null, searchOpt = null, searchWordsPattern = null;
			searchOpt = XVar.Clone(highlightData["searchOpt"]);
			searchWordsPattern = XVar.Clone(this.getSearchWordPattern((XVar)(highlightData["searchWords"]), new XVar(false)));
			switch(((XVar)searchOpt).ToString())
			{
				case "Equals":
					return this.addHighlightingSpan((XVar)(value));
				case "Starts with":
					return MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/(^", searchWordsPattern, ")/i")), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(value));
				case "Contains":
					pattern = XVar.Clone(MVCFunctions.Concat("/(", searchWordsPattern, ")/i"));
					if(XVar.Pack(!(XVar)(this.haveTheSameSpChReference((XVar)(pattern), (XVar)(value)))))
					{
						return MVCFunctions.preg_replace((XVar)(pattern), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(value));
					}
					return this.highlightValueWithSpecialChars((XVar)(value), (XVar)(pattern));
				default:
					return value;
			}

			return null;
		}
		public virtual XVar getNumberValueHighlighted(dynamic _param_value, dynamic _param_highlightData)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic highlightData = XVar.Clone(_param_highlightData);
			#endregion

			dynamic decimalPlaces = null, quantifier = null, searchOpt = null, searchWord = null, searchWordArr = XVar.Array();
			searchWordArr = XVar.Clone(XVar.Array());
			decimalPlaces = XVar.Clone(this.container.pSet.isDecimalDigits((XVar)(this.field)));
			quantifier = XVar.Clone((XVar.Pack(decimalPlaces <= 1) ? XVar.Pack("?") : XVar.Pack(MVCFunctions.Concat("{1,", decimalPlaces, "}"))));
			foreach (KeyValuePair<XVar, dynamic> _searchWord in highlightData["searchWords"].GetEnumerator())
			{
				dynamic currSearchWord = null, searchWordArray = null;
				currSearchWord = XVar.Clone(_searchWord.Value);
				if(XVar.Pack(!(XVar)(MVCFunctions.preg_match(new XVar("/^[\\d]+$/"), (XVar)(_searchWord.Value)))))
				{
					currSearchWord = XVar.Clone(this.formatSearchWord((XVar)(_searchWord.Value)));
					currSearchWord = XVar.Clone(MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/0", quantifier, "$/")), new XVar(""), (XVar)(currSearchWord)));
					currSearchWord = XVar.Clone(MVCFunctions.preg_replace(new XVar("/\\.$/"), new XVar(""), (XVar)(currSearchWord)));
				}
				searchWordArray = XVar.Clone(MVCFunctions.str_split((XVar)(currSearchWord)));
				searchWordArr.InitAndSetArrayItem(MVCFunctions.implode(new XVar("[^\\d]?"), (XVar)(searchWordArray)), null);
			}
			searchWord = XVar.Clone(MVCFunctions.implode(new XVar("|"), (XVar)(searchWordArr)));
			searchOpt = XVar.Clone(highlightData["searchOpt"]);
			switch(((XVar)searchOpt).ToString())
			{
				case "Equals":
					return this.addHighlightingSpan((XVar)(value));
				case "Starts with":
					return MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/^(", searchWord, ")/")), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(value));
				case "Contains":
					return MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/(", searchWord, ")/")), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(value));
				default:
					return value;
			}

			return null;
		}
		protected virtual XVar formatSearchWord(dynamic _param_searchWord)
		{
			#region pass-by-value parameters
			dynamic searchWord = XVar.Clone(_param_searchWord);
			#endregion

			return searchWord;
		}
		protected virtual XVar haveTheSameSpChReference(dynamic _param_pattern, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic pattern = XVar.Clone(_param_pattern);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic scContainedPattern = XVar.Array(), scFromValue = null;
			scContainedPattern = XVar.Clone(this.getSpecialCharsContainingPattern((XVar)(pattern)));
			scFromValue = XVar.Clone(this.getSpecialCharsFromString((XVar)(value)));
			foreach (KeyValuePair<XVar, dynamic> sc in scContainedPattern.GetEnumerator())
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(sc.Value), (XVar)(scFromValue))), XVar.Pack(false)))
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar getSpecialCharsContainingPattern(dynamic _param_pattern)
		{
			#region pass-by-value parameters
			dynamic pattern = XVar.Clone(_param_pattern);
			#endregion

			dynamic chars = XVar.Array(), csArray = XVar.Array();
			chars = XVar.Clone(new XVar(0, "&amp;", 1, "&quot;", 2, "&lt;", 3, "&gt;"));
			csArray = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> var_char in chars.GetEnumerator())
			{
				dynamic matches = XVar.Array();
				if(XVar.Pack(MVCFunctions.preg_match((XVar)(pattern), (XVar)(var_char.Value), (XVar)(matches))))
				{
					if(matches[0] != var_char.Value)
					{
						csArray.InitAndSetArrayItem(var_char.Value, null);
					}
				}
			}
			return csArray;
		}
		protected virtual XVar getSpecialCharsFromString(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic chars = XVar.Array(), csArray = XVar.Array();
			chars = XVar.Clone(new XVar(0, "&amp;", 1, "&quot;", 2, "&lt;", 3, "&gt;"));
			csArray = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> var_char in chars.GetEnumerator())
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(str), (XVar)(var_char.Value))), XVar.Pack(false)))
				{
					csArray.InitAndSetArrayItem(var_char.Value, null);
				}
			}
			return csArray;
		}
		protected virtual XVar getSplitStringWithCapturedDelimiters(dynamic _param_pattern, dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic pattern = XVar.Clone(_param_pattern);
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic delimiters = XVar.Array(), matches = XVar.Array(), resArray = XVar.Array(), strArray = XVar.Array();
			resArray = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(str)))))
			{
				return resArray;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.preg_match_all((XVar)(pattern), (XVar)(str), (XVar)(matches)))))
			{
				resArray.InitAndSetArrayItem(str, null);
				return resArray;
			}
			delimiters = XVar.Clone(matches[0]);
			strArray = XVar.Clone(MVCFunctions.preg_split((XVar)(pattern), (XVar)(str)));
			foreach (KeyValuePair<XVar, dynamic> item in strArray.GetEnumerator())
			{
				resArray.InitAndSetArrayItem(item.Value, null);
				if(XVar.Pack(delimiters.KeyExists(item.Key)))
				{
					resArray.InitAndSetArrayItem(delimiters[item.Key], null);
				}
			}
			return resArray;
		}
		protected virtual XVar highlightValueWithSpecialChars(dynamic _param_value, dynamic _param_pattern)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic pattern = XVar.Clone(_param_pattern);
			#endregion

			dynamic chars = XVar.Array();
			chars = XVar.Clone(new XVar(0, "&amp;", 1, "&quot;", 2, "&lt;", 3, "&gt;"));
			foreach (KeyValuePair<XVar, dynamic> var_char in chars.GetEnumerator())
			{
				dynamic matches = null, valueArr = XVar.Array(), valueArr2 = XVar.Array();
				valueArr = XVar.Clone(this.getSplitStringWithCapturedDelimiters((XVar)(MVCFunctions.Concat("/", var_char.Value, "/")), (XVar)(value)));
				if((XVar)(MVCFunctions.count(valueArr) == 1)  || (XVar)(!(XVar)(MVCFunctions.preg_match((XVar)(pattern), (XVar)(var_char.Value), (XVar)(matches)))))
				{
					continue;
				}
				valueArr2 = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> part in valueArr.GetEnumerator())
				{
					if(part.Value != var_char.Value)
					{
						valueArr2.InitAndSetArrayItem(MVCFunctions.preg_replace((XVar)(pattern), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(part.Value)), null);
					}
					else
					{
						valueArr2.InitAndSetArrayItem(var_char.Value, null);
					}
				}
				value = XVar.Clone(MVCFunctions.implode(new XVar(""), (XVar)(valueArr2)));
			}
			return value;
		}
		protected virtual XVar hasHTMLEntities(dynamic _param_item)
		{
			#region pass-by-value parameters
			dynamic item = XVar.Clone(_param_item);
			#endregion

			dynamic matches = XVar.Array();
			if(XVar.Pack(MVCFunctions.preg_match_all(new XVar("/&[^&;]{3,7};/"), (XVar)(item), (XVar)(matches))))
			{
				foreach (KeyValuePair<XVar, dynamic> entity in matches[0].GetEnumerator())
				{
					dynamic data = XVar.Array();
					data = XVar.Clone(MVCFunctions.getHTMLEntityData((XVar)(entity.Value)));
					if(XVar.Pack(data["isHTMLEntity"]))
					{
						return true;
					}
				}
			}
			return false;
		}
		protected virtual XVar highlightValueWithHTMLEntities(dynamic _param_item, dynamic _param_pattern)
		{
			#region pass-by-value parameters
			dynamic item = XVar.Clone(_param_item);
			dynamic pattern = XVar.Clone(_param_pattern);
			#endregion

			dynamic valueArr = XVar.Array(), valueArr2 = XVar.Array();
			valueArr = XVar.Clone(this.getSplitStringWithCapturedDelimiters(new XVar("/&[^&;]{3,7};/"), (XVar)(item)));
			valueArr2 = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> subitem in valueArr.GetEnumerator())
			{
				dynamic data = XVar.Array();
				data = XVar.Clone(MVCFunctions.getHTMLEntityData((XVar)(subitem.Value)));
				if(XVar.Pack(data["isHTMLEntity"]))
				{
					valueArr2.InitAndSetArrayItem(subitem.Value, null);
				}
				else
				{
					valueArr2.InitAndSetArrayItem(MVCFunctions.preg_replace((XVar)(pattern), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(subitem.Value)), null);
				}
			}
			return MVCFunctions.implode(new XVar(""), (XVar)(valueArr2));
		}
		protected virtual XVar getLookupParams()
		{
			dynamic lookupParams = XVar.Array();
			lookupParams = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.isFieldLookup))
			{
				ProjectSettings pSet;
				pSet = XVar.UnPackProjectSettings(this.container.pSet);
				lookupParams.InitAndSetArrayItem(pSet.multiSelect((XVar)(this.field)), "multiselect");
				lookupParams.InitAndSetArrayItem(this.needLookupValueProcessing, "needLookupProcessing");
				lookupParams.InitAndSetArrayItem(this.container.linkFieldValues[this.field], "linkFieldValue");
				lookupParams.InitAndSetArrayItem(this.container.originlinkValues[this.field], "originLinkValue");
			}
			return lookupParams;
		}
		protected virtual XVar getFirstSearchWordInLargeText(dynamic _param_searchWords, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic searchWords = XVar.Clone(_param_searchWords);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic matches = XVar.Array(), searchWordsPattern = null;
			searchWordsPattern = XVar.Clone(this.getSearchWordPattern((XVar)(searchWords), new XVar(false)));
			if(XVar.Pack(MVCFunctions.preg_match((XVar)(MVCFunctions.Concat("/", searchWordsPattern, "/i")), (XVar)(value), (XVar)(matches))))
			{
				return matches[0];
			}
			return searchWords[0];
		}
		protected virtual XVar highlightTruncatedBeforeMore(dynamic _param_value, dynamic _param_truncatedValue, dynamic _param_cNumberOfChars, dynamic _param_contenLength)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic truncatedValue = XVar.Clone(_param_truncatedValue);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			dynamic contenLength = XVar.Clone(_param_contenLength);
			#endregion

			dynamic firstPos = null, highlightData = XVar.Array(), highlighting = null, lastPos = null, lookupParams = null, pattern = null, regExpModifier = null, searchOpt = null, searchWord = null, searchWordEncoded = null, searchWordEncodedLen = null, truncLen = null;
			lookupParams = XVar.Clone(this.getLookupParams());
			highlightData = XVar.Clone(this.searchClauseObj.getSearchHighlightingData((XVar)(this.field), (XVar)(value), new XVar(false), (XVar)(lookupParams)));
			if(XVar.Pack(!(XVar)(highlightData)))
			{
				return truncatedValue;
			}
			searchWord = XVar.Clone(this.getFirstSearchWordInLargeText((XVar)(highlightData["searchWords"]), (XVar)(value)));
			searchWordEncoded = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(searchWord)));
			highlighting = XVar.Clone(this.addHighlightingSpan((XVar)(searchWordEncoded)));
			searchOpt = XVar.Clone(highlightData["searchOpt"]);
			switch(((XVar)searchOpt).ToString())
			{
				case "Equals":
					return this.addHighlightingSpan((XVar)(truncatedValue));
				case "Starts with":
					if(MVCFunctions.strlen((XVar)(truncatedValue)) < MVCFunctions.strlen((XVar)(searchWordEncoded)))
					{
						return this.addHighlightingSpan((XVar)(truncatedValue));
					}
					return MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/^", MVCFunctions.preg_quote((XVar)(searchWordEncoded), new XVar("/")), "/i")), (XVar)(highlighting), (XVar)(truncatedValue));
				case "Contains":
					regExpModifier = XVar.Clone((XVar.Pack(this.useUTF8) ? XVar.Pack("u") : XVar.Pack("")));
					firstPos = XVar.Clone(this.getFistOccurencePosition((XVar)(value), (XVar)(searchWord), (XVar)(searchWordEncoded)));
					lastPos = XVar.Clone(this.getLastOccurencePosition((XVar)(value), (XVar)(searchWord), (XVar)(searchWordEncoded)));
					searchWordEncodedLen = XVar.Clone(MVCFunctions.runner_strlen((XVar)(searchWordEncoded)));
					truncLen = XVar.Clone(MVCFunctions.runner_strlen((XVar)(truncatedValue)));
					pattern = XVar.Clone(MVCFunctions.Concat("/(", this.getSearchWordPattern((XVar)(highlightData["searchWords"]), new XVar(true)), ")/i"));
					if((XVar)(lastPos + searchWordEncodedLen <= truncLen)  || (XVar)(firstPos + searchWordEncodedLen <= truncLen))
					{
						if(XVar.Pack(!(XVar)(this.haveTheSameSpChReference((XVar)(pattern), (XVar)(truncatedValue)))))
						{
							return MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat(pattern, regExpModifier)), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(truncatedValue));
						}
						return this.highlightValueWithSpecialChars((XVar)(truncatedValue), (XVar)(MVCFunctions.Concat(pattern, regExpModifier)));
					}
					if(truncLen < firstPos)
					{
						dynamic firstPosDecoded = null, newNumberOfChars = null, qNumberOfChars = null, truncSubsr = null, valueSubstr = null;
						newNumberOfChars = XVar.Clone((XVar)Math.Ceiling((double)(cNumberOfChars / 2)));
						qNumberOfChars = XVar.Clone((XVar)Math.Ceiling((double)(cNumberOfChars / 4)));
						firstPosDecoded = XVar.Clone(MVCFunctions.runner_strpos((XVar)(value), (XVar)(searchWord)));
						truncSubsr = XVar.Clone(MVCFunctions.runner_substr((XVar)(value), new XVar(0), (XVar)(cNumberOfChars)));
						valueSubstr = XVar.Clone(MVCFunctions.runner_substr((XVar)(value), (XVar)(firstPosDecoded - qNumberOfChars), (XVar)(qNumberOfChars + MVCFunctions.runner_strlen((XVar)(searchWord)))));
						truncSubsr = XVar.Clone(MVCFunctions.runner_substr((XVar)(truncSubsr), new XVar(0), (XVar)(newNumberOfChars)));
						valueSubstr = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(valueSubstr)));
						pattern = XVar.Clone(MVCFunctions.Concat("/(", MVCFunctions.preg_quote((XVar)(searchWordEncoded), new XVar("/")), ")/i"));
						if(XVar.Pack(!(XVar)(this.haveTheSameSpChReference((XVar)(pattern), (XVar)(valueSubstr)))))
						{
							valueSubstr = XVar.Clone(MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat(pattern, regExpModifier)), (XVar)(highlighting), (XVar)(valueSubstr)));
						}
						else
						{
							valueSubstr = XVar.Clone(this.highlightValueWithSpecialChars((XVar)(valueSubstr), (XVar)(MVCFunctions.Concat(pattern, regExpModifier))));
						}
						return MVCFunctions.Concat(MVCFunctions.runner_htmlspecialchars((XVar)(truncSubsr)), "&nbsp;&lt;...&gt;&nbsp;", valueSubstr);
					}
					return MVCFunctions.Concat(MVCFunctions.runner_substr((XVar)(truncatedValue), new XVar(0), (XVar)(firstPos)), highlighting);
				default:
					return truncatedValue;
			}

			return null;
		}
		protected virtual XVar getFistOccurencePosition(dynamic _param_value, dynamic _param_searchWord, dynamic _param_searchWordEncoded)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic searchWord = XVar.Clone(_param_searchWord);
			dynamic searchWordEncoded = XVar.Clone(_param_searchWordEncoded);
			#endregion

			dynamic encodedPlaneSubstr = null, planeFirstPos = null, planeSubstr = null;
			planeFirstPos = XVar.Clone(MVCFunctions.strpos((XVar)(value), (XVar)(searchWord)));
			planeSubstr = XVar.Clone(MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(planeFirstPos)));
			encodedPlaneSubstr = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(planeSubstr)));
			return MVCFunctions.runner_strpos((XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(value))), (XVar)(searchWordEncoded), (XVar)(MVCFunctions.runner_strlen((XVar)(encodedPlaneSubstr))));
		}
		protected virtual XVar getLastOccurencePosition(dynamic _param_value, dynamic _param_searchWord, dynamic _param_searchWordEncoded)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic searchWord = XVar.Clone(_param_searchWord);
			dynamic searchWordEncoded = XVar.Clone(_param_searchWordEncoded);
			#endregion

			dynamic encodedPlaneSubstr = null, planeLastPos = null, planeSubstr = null;
			planeLastPos = XVar.Clone(MVCFunctions.strrpos((XVar)(value), (XVar)(searchWord)));
			planeSubstr = XVar.Clone(MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(planeLastPos)));
			encodedPlaneSubstr = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(planeSubstr)));
			return MVCFunctions.runner_strrpos((XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(value))), (XVar)(searchWordEncoded), (XVar)(MVCFunctions.runner_strlen((XVar)(encodedPlaneSubstr))));
		}
		protected virtual XVar getSearchWordPattern(dynamic _param_searchWords, dynamic _param_encoded = null)
		{
			#region default values
			if(_param_encoded as Object == null) _param_encoded = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic searchWords = XVar.Clone(_param_searchWords);
			dynamic encoded = XVar.Clone(_param_encoded);
			#endregion

			dynamic searchWordsPatterns = XVar.Array();
			searchWordsPatterns = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> searchWord in searchWords.GetEnumerator())
			{
				dynamic wordPattern = null;
				wordPattern = XVar.Clone(MVCFunctions.preg_quote((XVar)(searchWord.Value), new XVar("/")));
				if(XVar.Pack(encoded))
				{
					wordPattern = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(wordPattern)));
				}
				searchWordsPatterns.InitAndSetArrayItem(wordPattern, null);
			}
			return MVCFunctions.implode(new XVar("|"), (XVar)(searchWordsPatterns));
		}
		public virtual XVar addHighlightingSpan(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			return MVCFunctions.Concat("<span class=\"r-search-highlight\">", str, "</span>");
		}
		public virtual XVar getJSControl()
		{
			dynamic controlData = null, i = null;
			if(XVar.Pack(!(XVar)(this.getContainer().viewControlsMap.KeyExists("controls"))))
			{
				this.getContainer().viewControlsMap.InitAndSetArrayItem(XVar.Array(), "controls");
			}
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.getContainer().viewControlsMap["controls"]); i++)
			{
				if(this.getContainer().viewControlsMap["controls"][i]["fieldName"] == this.field)
				{
					return this.getContainer().viewControlsMap["controls"][i];
				}
			}
			controlData = XVar.Clone(new XVar("fieldName", this.field, "viewFormat", this.viewFormat));
			this.getContainer().viewControlsMap.InitAndSetArrayItem(controlData, "controls", null);
			return controlData;
		}
		public virtual XVar addJSControlSetting(dynamic _param_name, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic JScontrol = XVar.Array();
			JScontrol = this.getJSControl();
			JScontrol.InitAndSetArrayItem(value, name);

			return null;
		}
		public virtual XVar neededLoadJSFiles()
		{
			switch(((XVar)this.getContainer().pageType).ToString())
			{
				case Constants.PAGE_PRINT:
					return false;
				default:
					return true;
			}

			return null;
		}
		public virtual XVar isUserControl()
		{
			return (XVar)(this.userControl)  && (XVar)(XVar.Equals(XVar.Pack(this.userControl), XVar.Pack(true)));
		}
		protected virtual XVar getThumbnailSizeStyle(dynamic _param_returnStyleAttr = null)
		{
			#region default values
			if(_param_returnStyleAttr as Object == null) _param_returnStyleAttr = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic returnStyleAttr = XVar.Clone(_param_returnStyleAttr);
			#endregion

			dynamic style = null, width = null;
			width = XVar.Clone(this.container.pSet.getThumbnailSize((XVar)(this.field)));
			if(XVar.Pack(!(XVar)(width)))
			{
				return "";
			}
			style = XVar.Clone(MVCFunctions.Concat("max-width:", width, "px; max-height:", width, "px;"));
			if(XVar.Pack(returnStyleAttr))
			{
				return MVCFunctions.Concat(" style=\"", style, "\"");
			}
			return style;
		}
		protected virtual XVar getImageSizeStyle(dynamic _param_returnStyleAttr = null)
		{
			#region default values
			if(_param_returnStyleAttr as Object == null) _param_returnStyleAttr = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic returnStyleAttr = XVar.Clone(_param_returnStyleAttr);
			#endregion

			dynamic imgHeight = null, imgWidth = null, style = null;
			imgWidth = XVar.Clone(this.container.pSet.getImageWidth((XVar)(this.field)));
			imgHeight = XVar.Clone(this.container.pSet.getImageHeight((XVar)(this.field)));
			if((XVar)(!(XVar)(imgWidth))  && (XVar)(!(XVar)(imgHeight)))
			{
				return "";
			}
			style = new XVar("");
			if(XVar.Pack(imgWidth))
			{
				style = MVCFunctions.Concat(style, "max-width:", imgWidth, "px;");
			}
			if(XVar.Pack(imgHeight))
			{
				style = MVCFunctions.Concat(style, "max-height:", imgHeight, "px;");
			}
			if(XVar.Pack(returnStyleAttr))
			{
				return MVCFunctions.Concat(" style=\"", style, "\"");
			}
			return style;
		}
		public static XVar Format(dynamic _param_data, dynamic _param_field, dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic field = XVar.Clone(_param_field);
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic viewControls = null;
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(table)))
			{
				table = XVar.Clone(GlobalVars.strTableName);
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			viewControls = XVar.Clone(new ViewControlsContainer((XVar)(pSet), new XVar(Constants.PAGE_VIEW)));
			return viewControls.getControl((XVar)(field)).getTextValue((XVar)(data));
		}
	}
	public partial class ViewControlTypes : XClass
	{
		public dynamic viewTypes = XVar.Array();
		public ViewControlTypes()
		{
			this.viewTypes.InitAndSetArrayItem("", Constants.FORMAT_NONE);
			this.viewTypes.InitAndSetArrayItem("ViewShortDateField", Constants.FORMAT_DATE_SHORT);
			this.viewTypes.InitAndSetArrayItem("ViewLongDateField", Constants.FORMAT_DATE_LONG);
			this.viewTypes.InitAndSetArrayItem("ViewDatetimeField", Constants.FORMAT_DATE_TIME);
			this.viewTypes.InitAndSetArrayItem("ViewTimeField", Constants.FORMAT_TIME);
			this.viewTypes.InitAndSetArrayItem("ViewCurrencyField", Constants.FORMAT_CURRENCY);
			this.viewTypes.InitAndSetArrayItem("ViewPercentField", Constants.FORMAT_PERCENT);
			this.viewTypes.InitAndSetArrayItem("ViewHyperlinkField", Constants.FORMAT_HYPERLINK);
			this.viewTypes.InitAndSetArrayItem("ViewEmailHyperlinkField", Constants.FORMAT_EMAILHYPERLINK);
			this.viewTypes.InitAndSetArrayItem("ViewDatabaseImageField", Constants.FORMAT_DATABASE_IMAGE);
			this.viewTypes.InitAndSetArrayItem("ViewDatabaseFileField", Constants.FORMAT_DATABASE_FILE);
			this.viewTypes.InitAndSetArrayItem("ViewFileDownloadField", Constants.FORMAT_FILE);
			this.viewTypes.InitAndSetArrayItem("ViewImageDownloadField", Constants.FORMAT_FILE_IMAGE);
			this.viewTypes.InitAndSetArrayItem("ViewPhoneNumberField", Constants.FORMAT_PHONE_NUMBER);
			this.viewTypes.InitAndSetArrayItem("ViewNumberField", Constants.FORMAT_NUMBER);
			this.viewTypes.InitAndSetArrayItem("ViewCheckboxField", Constants.FORMAT_CHECKBOX);
			this.viewTypes.InitAndSetArrayItem("ViewMapField", Constants.FORMAT_MAP);
			this.viewTypes.InitAndSetArrayItem("ViewAudioFileField", Constants.FORMAT_AUDIO);
			this.viewTypes.InitAndSetArrayItem("ViewDatabaseAudioField", Constants.FORMAT_DATABASE_AUDIO);
			this.viewTypes.InitAndSetArrayItem("ViewVideoFileField", Constants.FORMAT_VIDEO);
			this.viewTypes.InitAndSetArrayItem("ViewDatabaseVideoField", Constants.FORMAT_DATABASE_VIDEO);
			this.viewTypes.InitAndSetArrayItem("ViewCustomField", Constants.FORMAT_CUSTOM);
			this.viewTypes.InitAndSetArrayItem("ViewLookupWizardField", Constants.FORMAT_LOOKUP_WIZARD);
			this.viewTypes.InitAndSetArrayItem("ViewHTMLField", Constants.FORMAT_HTML);
		}
	}
}
