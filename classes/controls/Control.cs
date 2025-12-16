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
	public partial class EditControl : XClass
	{
		public dynamic pageObject = XVar.Pack(null);
		public dynamic container = XVar.Pack(null);
		public dynamic id = XVar.Pack("");
		public dynamic field = XVar.Pack("");
		public dynamic goodFieldName = XVar.Pack("");
		public dynamic format = XVar.Pack("");
		public dynamic cfieldname = XVar.Pack("");
		public dynamic cfield = XVar.Pack("");
		public dynamic ctype = XVar.Pack("");
		public dynamic is508 = XVar.Pack(false);
		public dynamic strLabel = XVar.Pack("");
		public dynamic var_type = XVar.Pack("");
		public dynamic inputStyle = XVar.Pack("");
		public dynamic iquery = XVar.Pack("");
		public dynamic keylink = XVar.Pack("");
		public dynamic webValue = XVar.Pack(null);
		public dynamic webType = XVar.Pack(null);
		public dynamic settings = XVar.Array();
		public dynamic isOracle = XVar.Pack(false);
		public dynamic ismssql = XVar.Pack(false);
		public dynamic isdb2 = XVar.Pack(false);
		public dynamic btexttype = XVar.Pack(false);
		public dynamic isMysql = XVar.Pack(false);
		public dynamic var_like = XVar.Pack("like");
		public dynamic searchOptions = XVar.Array();
		public dynamic searchPanelControl = XVar.Pack(false);
		public dynamic data = XVar.Array();
		protected dynamic connection;
		public dynamic forSpreadsheetGrid;
		public EditControl(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.field = XVar.Clone(field);
			this.goodFieldName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(field)));
			this.setID((XVar)(id));
			this.connection = XVar.Clone(connection);
			this.pageObject = XVar.Clone(pageObject);
			this.is508 = XVar.Clone(CommonFunctions.isEnableSection508());
			this.strLabel = XVar.Clone(pageObject.pSetEdit.label((XVar)(field)));
			this.var_type = XVar.Clone(pageObject.pSetEdit.getFieldType((XVar)(this.field)));
			if(this.connection.dbType == Constants.nDATABASE_Oracle)
			{
				this.isOracle = new XVar(true);
			}
			if(this.connection.dbType == Constants.nDATABASE_MSSQLServer)
			{
				this.ismssql = new XVar(true);
			}
			if(this.connection.dbType == Constants.nDATABASE_DB2)
			{
				this.isdb2 = new XVar(true);
			}
			if(this.connection.dbType == Constants.nDATABASE_MySQL)
			{
				this.isMysql = new XVar(true);
			}
			if(this.connection.dbType == Constants.nDATABASE_PostgreSQL)
			{
				this.var_like = new XVar("ilike");
			}
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("CONTAINS")), Constants.CONTAINS);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("EQUALS")), Constants.EQUALS);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("STARTS_WITH")), Constants.STARTS_WITH);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("MORE_THAN")), Constants.MORE_THAN);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("LESS_THAN")), Constants.LESS_THAN);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("BETWEEN")), Constants.BETWEEN);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("EMPTY")), Constants.EMPTY_SEARCH);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SEARCH_NOT_CONTAINS")), Constants.NOT_CONTAINS);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SEARCH_NOT_EQUALS")), Constants.NOT_EQUALS);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SEARCH_NOT_STARTS_WITH")), Constants.NOT_STARTS_WITH);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SEARCH_NOT_MORE_THAN")), Constants.NOT_MORE_THAN);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SEARCH_NOT_LESS_THAN")), Constants.NOT_LESS_THAN);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SEARCH_NOT_BETWEEN")), Constants.NOT_BETWEEN);
			this.searchOptions.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("SEARCH_NOT_EMPTY")), Constants.NOT_EMPTY);
			this.init();
		}
		public virtual XVar setID(dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			#endregion

			this.id = XVar.Clone(id);
			this.cfieldname = XVar.Clone(MVCFunctions.Concat(this.goodFieldName, "_", id));
			this.cfield = XVar.Clone(MVCFunctions.Concat("value_", this.goodFieldName, "_", id));
			this.ctype = XVar.Clone(MVCFunctions.Concat("type_", this.goodFieldName, "_", id));

			return null;
		}
		public virtual XVar addJSFiles()
		{

			return null;
		}
		public virtual XVar addCSSFiles()
		{

			return null;
		}
		public virtual XVar getSetting(dynamic _param_key)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			#endregion

			if(XVar.Equals(XVar.Pack(key), XVar.Pack("Label")))
			{
				return this.pageObject.pSetEdit.label((XVar)(this.field));
			}
			return "";
		}
		public virtual XVar addJSSetting(dynamic _param_key, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic key = XVar.Clone(_param_key);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			this.pageObject.jsControlSettings.InitAndSetArrayItem(value, this.field, this.container.pageType, key);

			return null;
		}
		public virtual XVar buildControl(dynamic _param_value, dynamic _param_mode, dynamic _param_fieldNum, dynamic _param_validate, dynamic _param_additionalCtrlParams, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic validate = XVar.Clone(_param_validate);
			dynamic additionalCtrlParams = XVar.Clone(_param_additionalCtrlParams);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic additionalClass = null, arrKeys = XVar.Array(), isHidden = null, j = null;
			this.searchPanelControl = XVar.Clone(this.isSearchPanelControl((XVar)(mode), (XVar)(additionalCtrlParams)));
			this.inputStyle = XVar.Clone(this.getInputStyle((XVar)(mode)));
			if(XVar.Pack(fieldNum))
			{
				this.cfield = XVar.Clone(MVCFunctions.Concat("value", fieldNum, "_", this.goodFieldName, "_", this.id));
				this.ctype = XVar.Clone(MVCFunctions.Concat("type", fieldNum, "_", this.goodFieldName, "_", this.id));
			}
			this.iquery = XVar.Clone(MVCFunctions.Concat("field=", MVCFunctions.RawUrlEncode((XVar)(this.field))));
			arrKeys = XVar.Clone(this.pageObject.pSetEdit.getTableKeys());
			j = new XVar(0);
			for(;j < MVCFunctions.count(arrKeys); j++)
			{
				this.keylink = MVCFunctions.Concat(this.keylink, "&key", j + 1, "=", MVCFunctions.RawUrlEncode((XVar)(data[arrKeys[j]])));
			}
			this.iquery = MVCFunctions.Concat(this.iquery, this.keylink);
			isHidden = XVar.Clone((XVar)(additionalCtrlParams.KeyExists("hidden"))  && (XVar)(additionalCtrlParams["hidden"]));
			additionalClass = new XVar("");
			additionalClass = MVCFunctions.Concat(additionalClass, "bs-ctrlspan ");
			if(this.format == Constants.EDIT_FORMAT_READONLY)
			{
				additionalClass = MVCFunctions.Concat(additionalClass, "form-control-static ");
			}
			if((XVar)(validate["basicValidate"])  && (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.array_search(new XVar("IsRequired"), (XVar)(validate["basicValidate"]))), XVar.Pack(false))))
			{
				additionalClass = MVCFunctions.Concat(additionalClass, "bs-inlinerequired");
			}
			MVCFunctions.Echo(MVCFunctions.Concat("<span id=\"edit", this.id, "_", this.goodFieldName, "_", fieldNum, "\" class=\"", additionalClass, "\"", (XVar.Pack(isHidden) ? XVar.Pack(" style=\"display:none\"") : XVar.Pack("")), ">"));

			return null;
		}
		public virtual XVar getFirstElementId()
		{
			return false;
		}
		public virtual XVar isSearchPanelControl(dynamic _param_mode, dynamic _param_additionalCtrlParams)
		{
			#region pass-by-value parameters
			dynamic mode = XVar.Clone(_param_mode);
			dynamic additionalCtrlParams = XVar.Clone(_param_additionalCtrlParams);
			#endregion

			return (XVar)((XVar)(mode == Constants.MODE_SEARCH)  && (XVar)(additionalCtrlParams.KeyExists("searchPanelControl")))  && (XVar)(additionalCtrlParams["searchPanelControl"]);
		}
		public virtual XVar buildControlEnd(dynamic _param_validate, dynamic _param_mode)
		{
			#region pass-by-value parameters
			dynamic validate = XVar.Clone(_param_validate);
			dynamic mode = XVar.Clone(_param_mode);
			#endregion

			MVCFunctions.Echo("</span>");

			return null;
		}
		public virtual XVar getPostValueAndType()
		{
			this.webValue = XVar.Clone(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("value_", this.goodFieldName, "_", this.id))));
			this.webType = XVar.Clone(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("type_", this.goodFieldName, "_", this.id))));

			return null;
		}
		public virtual XVar getWebValue()
		{
			return this.webValue;
		}
		public virtual XVar readWebValue(dynamic avalues, dynamic blobfields, dynamic _param_legacy1, dynamic _param_legacy2, dynamic filename_values)
		{
			#region pass-by-value parameters
			dynamic legacy1 = XVar.Clone(_param_legacy1);
			dynamic legacy2 = XVar.Clone(_param_legacy2);
			#endregion

			this.getPostValueAndType();
			if(XVar.Pack(MVCFunctions.FieldSubmitted((XVar)(MVCFunctions.Concat(this.goodFieldName, "_", this.id)))))
			{
				this.webValue = XVar.Clone(CommonFunctions.prepare_for_db((XVar)(this.field), (XVar)(this.webValue), (XVar)(this.webType)));
			}
			else
			{
				this.webValue = new XVar(false);
			}
			if((XVar)(this.pageObject.pageType == Constants.PAGE_EDIT)  && (XVar)(XVar.Equals(XVar.Pack(this.pageObject.getEditFormat((XVar)(this.field))), XVar.Pack(Constants.EDIT_FORMAT_READONLY))))
			{
				if(XVar.Pack(this.pageObject.pSetEdit.getAutoUpdateValue((XVar)(this.field))))
				{
					this.webValue = XVar.Clone(this.pageObject.pSetEdit.getAutoUpdateValue((XVar)(this.field)));
				}
				else
				{
					if(XVar.Pack(!(XVar)(CommonFunctions.originalTableField((XVar)(this.field), (XVar)(this.pageObject.pSetEdit)))))
					{
						this.webValue = new XVar(false);
					}
				}
			}
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(this.webValue), XVar.Pack(false)))))
			{
				if(this.connection.dbType == Constants.nDATABASE_Informix)
				{
					if(XVar.Pack(CommonFunctions.IsTextType((XVar)(this.pageObject.pSetEdit.getFieldType((XVar)(this.field))))))
					{
						blobfields.InitAndSetArrayItem(this.field, null);
					}
				}
				if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("hashPassword"))))
				{
					if((XVar)(this.field == Security.passwordField())  && (XVar)(this.pageObject.tName == "admin_users"))
					{
						dynamic needHashPass = null;
						needHashPass = new XVar(true);
						if(this.pageObject.pageType == Constants.PAGE_EDIT)
						{
							dynamic oldData = XVar.Array();
							oldData = XVar.Clone(this.pageObject.getOldRecordData());
							needHashPass = XVar.Clone(oldData[this.field] != this.webValue);
						}
						if(XVar.Pack(needHashPass))
						{
							this.webValue = XVar.Clone(Security.hashPassword((XVar)(this.webValue)));
						}
					}
				}
				avalues.InitAndSetArrayItem(this.webValue, this.field);
			}

			return null;
		}
		public virtual XVar getSelectColumnsAndJoinFromPart(dynamic _param_searchFor, dynamic _param_searchOpt, dynamic _param_isSuggest)
		{
			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic searchOpt = XVar.Clone(_param_searchOpt);
			dynamic isSuggest = XVar.Clone(_param_isSuggest);
			#endregion

			return new XVar("selectColumns", this.getFieldSQLDecrypt(), "joinFromPart", "");
		}
		public virtual XVar checkIfDisplayFieldSearch(dynamic _param_strSearchOption)
		{
			#region pass-by-value parameters
			dynamic strSearchOption = XVar.Clone(_param_strSearchOption);
			#endregion

			return false;
		}
		public virtual XVar buildSearchOptions(dynamic _param_optionsArray, dynamic _param_selOpt, dynamic _param_not, dynamic _param_both)
		{
			#region pass-by-value parameters
			dynamic optionsArray = XVar.Clone(_param_optionsArray);
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			dynamic currentOption = null, defaultOption = null, result = null, userSearchOptions = XVar.Array();
			userSearchOptions = XVar.Clone(this.pageObject.pSetEdit.getSearchOptionsList((XVar)(this.field)));
			currentOption = XVar.Clone((XVar.Pack(var_not) ? XVar.Pack(MVCFunctions.Concat("NOT ", selOpt)) : XVar.Pack(selOpt)));
			if((XVar)(userSearchOptions)  && (XVar)(this.searchOptions.KeyExists(currentOption)))
			{
				userSearchOptions.InitAndSetArrayItem(currentOption, null);
			}
			if(XVar.Pack(!(XVar)(!(XVar)(userSearchOptions))))
			{
				optionsArray = XVar.Clone(MVCFunctions.array_intersect((XVar)(optionsArray), (XVar)(userSearchOptions)));
			}
			defaultOption = XVar.Clone(this.pageObject.pSetEdit.getDefaultSearchOption((XVar)(this.field)));
			if(XVar.Pack(!(XVar)(defaultOption)))
			{
				defaultOption = XVar.Clone(optionsArray[0]);
			}
			result = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> var_option in optionsArray.GetEnumerator())
			{
				dynamic dataAttr = null, selected = null;
				if((XVar)(!(XVar)(this.searchOptions.KeyExists(var_option.Value)))  || (XVar)((XVar)(!(XVar)(both))  && (XVar)(MVCFunctions.substr((XVar)(var_option.Value), new XVar(0), new XVar(4)) == "NOT ")))
				{
					continue;
				}
				selected = XVar.Clone((XVar.Pack(currentOption == var_option.Value) ? XVar.Pack("selected") : XVar.Pack("")));
				dataAttr = XVar.Clone((XVar.Pack(defaultOption == var_option.Value) ? XVar.Pack(" data-default-option=\"true\"") : XVar.Pack("")));
				result = MVCFunctions.Concat(result, "<option value=\"", var_option.Value, "\" ", selected, dataAttr, ">", this.searchOptions[var_option.Value], "</option>");
			}
			return result;
		}
		public virtual XVar getSearchOptions(dynamic _param_selOpt, dynamic _param_not, dynamic _param_both)
		{
			#region pass-by-value parameters
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			return this.buildSearchOptions((XVar)(new XVar(0, Constants.EQUALS, 1, Constants.NOT_EQUALS)), (XVar)(selOpt), (XVar)(var_not), (XVar)(both));
		}
		public virtual XVar suggestValue(dynamic _param_value, dynamic _param_searchFor, dynamic var_response, dynamic row)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic searchFor = XVar.Clone(_param_searchFor);
			#endregion

			dynamic realValue = null, suggestStringSize = null, viewFormat = null;
			suggestStringSize = XVar.Clone(ProjectSettings.getProjectValue(new XVar("suggestStringSize")));
			if(suggestStringSize <= MVCFunctions.runner_strlen((XVar)(searchFor)))
			{
				var_response.InitAndSetArrayItem(searchFor, MVCFunctions.Concat("_", searchFor));
				return null;
			}
			viewFormat = XVar.Clone(this.pageObject.pSetEdit.getViewFormat((XVar)(this.field)));
			if(viewFormat == Constants.FORMAT_NUMBER)
			{
				dynamic dotPosition = null;
				dotPosition = XVar.Clone(MVCFunctions.strpos((XVar)(value), new XVar(".")));
				if(!XVar.Equals(XVar.Pack(dotPosition), XVar.Pack(false)))
				{
					dynamic i = null;
					i = XVar.Clone(MVCFunctions.strlen((XVar)(value)) - 1);
					for(;dotPosition < i; i--)
					{
						if(MVCFunctions.substr((XVar)(value), (XVar)(i), new XVar(1)) != "0")
						{
							if(i < MVCFunctions.strlen((XVar)(value)) - 1)
							{
								value = XVar.Clone(MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(i + 1)));
							}
							break;
						}
						if((XVar)(i == dotPosition + 1)  && (XVar)(XVar.Pack(0) < dotPosition))
						{
							value = XVar.Clone(MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(dotPosition)));
							break;
						}
					}
				}
			}
			realValue = XVar.Clone(value);
			if(viewFormat == Constants.FORMAT_HTML)
			{
				dynamic get_text = null, html_tags = null, match = XVar.Array(), useUTF8 = null;
				html_tags = XVar.Clone(MVCFunctions.Concat("/<.*?>/i", (XVar.Pack(useUTF8) ? XVar.Pack("u") : XVar.Pack(""))));
				get_text = XVar.Clone(MVCFunctions.Concat("/(.*<.*>|^.*?)([.]*", MVCFunctions.preg_quote((XVar)(searchFor), new XVar("/")), ".*?)(<.*>|$)/i", (XVar.Pack(useUTF8) ? XVar.Pack("u") : XVar.Pack(""))));
				value = XVar.Clone(MVCFunctions.preg_replace((XVar)(html_tags), new XVar(""), (XVar)(MVCFunctions.runner_html_entity_decode((XVar)(value)))));
				if(XVar.Equals(XVar.Pack(MVCFunctions.stristr((XVar)(value), (XVar)(searchFor))), XVar.Pack(false)))
				{
					return null;
				}
				if(XVar.Pack(MVCFunctions.preg_match((XVar)(get_text), (XVar)(realValue), (XVar)(match))))
				{
					realValue = XVar.Clone(match[2]);
				}
				else
				{
					realValue = XVar.Clone(value);
				}
			}
			if(suggestStringSize < MVCFunctions.runner_strlen((XVar)(value)))
			{
				dynamic startPos = null, suggestValues = XVar.Array(), valueLength = null;
				startPos = new XVar(0);
				valueLength = new XVar(0);
				suggestValues = XVar.Clone(this.cutSuggestString((XVar)(value), (XVar)(searchFor)));
				if(XVar.Pack(suggestValues))
				{
					if(viewFormat == Constants.FORMAT_HTML)
					{
						suggestValues.InitAndSetArrayItem(realValue, "search");
					}
					var_response.InitAndSetArrayItem(suggestValues["search"], suggestValues["display"]);
				}
			}
			else
			{
				var_response.InitAndSetArrayItem(realValue, value);
			}

			return null;
		}
		public virtual XVar cutSuggestString(dynamic _param__value, dynamic _param_searchFor)
		{
			#region pass-by-value parameters
			dynamic _value = XVar.Clone(_param__value);
			dynamic searchFor = XVar.Clone(_param_searchFor);
			#endregion

			dynamic caseIns = null, grayZoneLength = null, leftEllipsis = null, leftGrayZone = null, leftGrayZoneLength = null, leftPadding = null, leftPaddingLength = null, line = null, lineIdx = null, lines = XVar.Array(), rightEllipsis = null, rightGrayZone = null, rightGrayZoneLength = null, rightPadding = null, rightPaddingLength = null, searchValue = null, startPos = null, stopPos = null, suggestStringSize = null, value = null;
			suggestStringSize = XVar.Clone(ProjectSettings.getProjectValue(new XVar("suggestStringSize")));
			caseIns = XVar.Clone(this.pageObject.pSetEdit.getNCSearch());
			lines = XVar.Clone(MVCFunctions.explode(new XVar("\n"), (XVar)(_value)));
			value = new XVar("");
			lineIdx = new XVar(0);
			for(;lineIdx < MVCFunctions.count(lines); ++(lineIdx))
			{
				line = XVar.Clone(lines[lineIdx]);
				if(XVar.Pack(caseIns))
				{
					startPos = XVar.Clone(MVCFunctions.stripos((XVar)(line), (XVar)(searchFor)));
					if(XVar.Pack(startPos))
					{
						startPos = XVar.Clone(MVCFunctions.runner_strlen((XVar)(MVCFunctions.substr((XVar)(line), new XVar(0), (XVar)(startPos)))));
					}
				}
				else
				{
					startPos = XVar.Clone(MVCFunctions.runner_strpos((XVar)(line), (XVar)(searchFor)));
				}
				if(!XVar.Equals(XVar.Pack(startPos), XVar.Pack(false)))
				{
					value = XVar.Clone(line);
					break;
				}
			}
			if(XVar.Equals(XVar.Pack(startPos), XVar.Pack(false)))
			{
				return false;
			}
			grayZoneLength = new XVar(5);
			leftPaddingLength = XVar.Clone(MVCFunctions.min((XVar)(suggestStringSize / 2), (XVar)(startPos)));
			leftPadding = XVar.Clone(MVCFunctions.runner_substr((XVar)(value), (XVar)(startPos - leftPaddingLength), (XVar)(leftPaddingLength)));
			leftGrayZoneLength = XVar.Clone((XVar.Pack(leftPaddingLength < suggestStringSize / 2) ? XVar.Pack(0) : XVar.Pack(grayZoneLength)));
			rightPaddingLength = XVar.Clone(MVCFunctions.min((XVar)(suggestStringSize - leftPaddingLength), (XVar)((MVCFunctions.runner_strlen((XVar)(value)) - startPos) - MVCFunctions.runner_strlen((XVar)(searchFor)))));
			rightPadding = XVar.Clone(MVCFunctions.runner_substr((XVar)(value), (XVar)(startPos + MVCFunctions.runner_strlen((XVar)(searchFor))), (XVar)(rightPaddingLength)));
			rightGrayZoneLength = XVar.Clone((XVar.Pack(rightPaddingLength < suggestStringSize / 2) ? XVar.Pack(0) : XVar.Pack(grayZoneLength)));
			leftGrayZone = XVar.Clone(MVCFunctions.runner_substr((XVar)(leftPadding), new XVar(0), (XVar)(leftGrayZoneLength)));
			stopPos = XVar.Clone(this.findFirstStop((XVar)(leftGrayZone), new XVar(true)));
			if(!XVar.Equals(XVar.Pack(stopPos), XVar.Pack(false)))
			{
				leftPadding = XVar.Clone(MVCFunctions.runner_substr((XVar)(leftPadding), (XVar)(stopPos)));
			}
			rightGrayZone = XVar.Clone(MVCFunctions.runner_substr((XVar)(rightPadding), (XVar)(rightPaddingLength - rightGrayZoneLength)));
			stopPos = XVar.Clone(this.findFirstStop((XVar)(rightGrayZone)));
			if(!XVar.Equals(XVar.Pack(stopPos), XVar.Pack(false)))
			{
				rightPadding = XVar.Clone(MVCFunctions.runner_substr((XVar)(rightPadding), new XVar(0), (XVar)((MVCFunctions.runner_strlen((XVar)(rightPadding)) - rightGrayZoneLength) + stopPos)));
			}
			leftEllipsis = XVar.Clone((XVar.Pack((XVar)(XVar.Pack(0) < lineIdx)  || (XVar)(MVCFunctions.runner_strlen((XVar)(leftPadding)) < startPos)) ? XVar.Pack("... ") : XVar.Pack("")));
			rightEllipsis = XVar.Clone((XVar.Pack((XVar)(lineIdx < MVCFunctions.count(lines) - 1)  || (XVar)(MVCFunctions.runner_strlen((XVar)(rightPadding)) < (MVCFunctions.runner_strlen((XVar)(value)) - startPos) - MVCFunctions.runner_strlen((XVar)(searchFor)))) ? XVar.Pack(" ...") : XVar.Pack("")));
			searchValue = XVar.Clone(MVCFunctions.Concat(leftPadding, MVCFunctions.runner_substr((XVar)(value), (XVar)(startPos), (XVar)(MVCFunctions.runner_strlen((XVar)(searchFor)))), rightPadding));
			return new XVar("search", searchValue, "display", MVCFunctions.Concat(leftEllipsis, searchValue, rightEllipsis));
		}
		public virtual XVar findFirstStop(dynamic _param_str, dynamic _param_reverse = null)
		{
			#region default values
			if(_param_reverse as Object == null) _param_reverse = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			dynamic reverse = XVar.Clone(_param_reverse);
			#endregion

			dynamic c = null, i = null, idx = null, length = null, stopSymbols = null;
			stopSymbols = new XVar(" .,;:\"'?!|\\/=(){}[]*-+\n\r");
			length = XVar.Clone(MVCFunctions.runner_strlen((XVar)(str)));
			i = new XVar(0);
			for(;i < length; ++(i))
			{
				idx = XVar.Clone((XVar.Pack(reverse) ? XVar.Pack((length - i) - 1) : XVar.Pack(i)));
				c = XVar.Clone(MVCFunctions.runner_substr((XVar)(str), (XVar)(idx), new XVar(1)));
				if(!XVar.Equals(XVar.Pack(MVCFunctions.runner_strpos((XVar)(stopSymbols), (XVar)(c))), XVar.Pack(false)))
				{
					return idx;
				}
			}
			return false;
		}
		public virtual XVar afterSuccessfulSave()
		{

			return null;
		}
		public virtual XVar init()
		{

			return null;
		}
		public virtual XVar isStringValidForLike(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			if((XVar)(!(XVar)(CommonFunctions.IsCharType((XVar)(this.var_type))))  && (XVar)(MVCFunctions.hasNonAsciiSymbols((XVar)(str))))
			{
				return false;
			}
			return true;
		}
		public virtual XVar getInputStyle(dynamic _param_mode)
		{
			#region pass-by-value parameters
			dynamic mode = XVar.Clone(_param_mode);
			#endregion

			return "";
		}
		public virtual XVar makeWidthStyle(dynamic _param_widthPx)
		{
			#region pass-by-value parameters
			dynamic widthPx = XVar.Clone(_param_widthPx);
			#endregion

			return "";
		}
		public virtual XVar loadLookupContent(dynamic _param_parentValuesData, dynamic _param_childVal = null, dynamic _param_doCategoryFilter = null, dynamic _param_initialLoad = null)
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

			return "";
		}
		public virtual XVar getLookupContentToReload(dynamic _param_isExistParent, dynamic _param_mode, dynamic _param_parentCtrlsData)
		{
			#region pass-by-value parameters
			dynamic isExistParent = XVar.Clone(_param_isExistParent);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic parentCtrlsData = XVar.Clone(_param_parentCtrlsData);
			#endregion

			return "";
		}
		public virtual XVar getFieldValueCopy(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return value;
		}
		public virtual XVar getFieldSQLDecrypt()
		{
			return RunnerPage._getFieldSQLDecrypt((XVar)(this.field), (XVar)(this.connection), (XVar)(this.pageObject.pSetEdit), (XVar)(this.pageObject.cipherer));
		}
		protected virtual XVar getPlaceholderAttr()
		{
			if((XVar)(!(XVar)(this.searchPanelControl))  && (XVar)(this.container.pageType != Constants.PAGE_SEARCH))
			{
				return MVCFunctions.Concat(" placeholder=\"", MVCFunctions.runner_htmlspecialchars((XVar)(CommonFunctions.GetFieldPlaceHolder((XVar)(MVCFunctions.GoodFieldName((XVar)(this.pageObject.tName))), (XVar)(MVCFunctions.GoodFieldName((XVar)(this.field)))))), "\"");
			}
			return "";
		}
		public virtual XVar getConnection()
		{
			return this.Invoke("connection");
		}
		public virtual XVar getBasicFieldCondition(dynamic _param_svalue, dynamic _param_strSearchOption, dynamic _param_svalue2 = null, dynamic _param_etype = null)
		{
			#region default values
			if(_param_svalue2 as Object == null) _param_svalue2 = new XVar("");
			if(_param_etype as Object == null) _param_etype = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic svalue = XVar.Clone(_param_svalue);
			dynamic strSearchOption = XVar.Clone(_param_strSearchOption);
			dynamic svalue2 = XVar.Clone(_param_svalue2);
			dynamic etype = XVar.Clone(_param_etype);
			#endregion

			dynamic caseInsensitive = null, searchFor = null, searchFor2 = null;
			searchFor = XVar.Clone(this.processControlValue((XVar)(svalue), (XVar)(etype)));
			searchFor2 = XVar.Clone(this.processControlValue((XVar)(svalue2), (XVar)(etype)));
			caseInsensitive = XVar.Clone((XVar.Pack(this.pageObject.pSetEdit.getNCSearch()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_DEFAULT)));
			if(strSearchOption == Constants.EQUALS)
			{
				return DataCondition.FieldEquals((XVar)(this.field), (XVar)(searchFor), new XVar(0), (XVar)(caseInsensitive));
			}
			else
			{
				if(strSearchOption == Constants.STARTS_WITH)
				{
					return DataCondition.FieldIs((XVar)(this.field), new XVar(Constants.dsopSTART), (XVar)(searchFor), (XVar)(caseInsensitive));
				}
				else
				{
					if(strSearchOption == Constants.CONTAINS)
					{
						return DataCondition.FieldIs((XVar)(this.field), new XVar(Constants.dsopCONTAIN), (XVar)(searchFor), (XVar)(caseInsensitive));
					}
					else
					{
						if(strSearchOption == Constants.MORE_THAN)
						{
							return DataCondition.FieldIs((XVar)(this.field), new XVar(Constants.dsopMORE), (XVar)(searchFor), (XVar)(caseInsensitive));
						}
						else
						{
							if(strSearchOption == Constants.LESS_THAN)
							{
								return DataCondition.FieldIs((XVar)(this.field), new XVar(Constants.dsopLESS), (XVar)(searchFor), (XVar)(caseInsensitive));
							}
							else
							{
								if((XVar)(strSearchOption == Constants.BETWEEN)  && (XVar)((XVar)(searchFor != XVar.Pack(""))  || (XVar)(searchFor2 != XVar.Pack(""))))
								{
									if(searchFor == XVar.Pack(""))
									{
										return this.getSearchCondition((XVar)(svalue2), new XVar(Constants.NOT_MORE_THAN), new XVar(""), (XVar)(etype));
									}
									if(searchFor2 == XVar.Pack(""))
									{
										return this.getSearchCondition((XVar)(svalue), new XVar(Constants.NOT_LESS_THAN), new XVar(""), (XVar)(etype));
									}
									return DataCondition.FieldBetween((XVar)(this.field), (XVar)(searchFor), (XVar)(searchFor2), (XVar)(caseInsensitive));
								}
								else
								{
									if(strSearchOption == Constants.EMPTY_SEARCH)
									{
										return DataCondition.FieldIs((XVar)(this.field), new XVar(Constants.dsopEMPTY), (XVar)(searchFor));
									}
								}
							}
						}
					}
				}
			}
			return null;
		}
		public virtual XVar getSearchCondition(dynamic _param_searchFor, dynamic _param_strSearchOption, dynamic _param_searchFor2 = null, dynamic _param_not = null, dynamic _param_etype = null)
		{
			#region default values
			if(_param_searchFor2 as Object == null) _param_searchFor2 = new XVar("");
			if(_param_not as Object == null) _param_not = new XVar(false);
			if(_param_etype as Object == null) _param_etype = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic strSearchOption = XVar.Clone(_param_strSearchOption);
			dynamic searchFor2 = XVar.Clone(_param_searchFor2);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic etype = XVar.Clone(_param_etype);
			#endregion

			dynamic cond = null;
			if(MVCFunctions.substr((XVar)(strSearchOption), new XVar(0), new XVar(4)) == "NOT ")
			{
				strSearchOption = XVar.Clone(MVCFunctions.substr((XVar)(strSearchOption), new XVar(4)));
				var_not = new XVar(true);
			}
			cond = XVar.Clone(this.getBasicFieldCondition((XVar)(searchFor), (XVar)(strSearchOption), (XVar)(searchFor2), (XVar)(etype)));
			if(XVar.Pack(var_not))
			{
				cond = XVar.Clone(DataCondition._Not((XVar)(cond)));
			}
			return cond;
		}
		public virtual XVar processControlValue(dynamic _param_value, dynamic _param_controlType)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic controlType = XVar.Clone(_param_controlType);
			#endregion

			if(MVCFunctions.substr((XVar)(controlType), new XVar(0), new XVar(4)) == "date")
			{
				dynamic dformat = null;
				dformat = XVar.Clone(MVCFunctions.substr((XVar)(controlType), new XVar(4)));
				if((XVar)((XVar)(dformat == Constants.EDIT_DATE_SIMPLE)  || (XVar)(dformat == Constants.EDIT_DATE_SIMPLE_INLINE))  || (XVar)(dformat == Constants.EDIT_DATE_SIMPLE_DP))
				{
					dynamic time = null;
					time = XVar.Clone(CommonFunctions.localdatetime2db((XVar)(value)));
					if(time == "null")
					{
						return "";
					}
					return time;
				}
				else
				{
					if((XVar)((XVar)(dformat == Constants.EDIT_DATE_DD)  || (XVar)(dformat == Constants.EDIT_DATE_DD_INLINE))  || (XVar)(dformat == Constants.EDIT_DATE_DD_DP))
					{
						dynamic a = XVar.Array(), d = null, m = null, y = null;
						a = XVar.Clone(MVCFunctions.explode(new XVar("-"), (XVar)(value)));
						if(MVCFunctions.count(a) < 3)
						{
							return "";
						}
						else
						{
							y = XVar.Clone(a[0]);
							m = XVar.Clone(a[1]);
							d = XVar.Clone(a[2]);
						}
						if(y < 100)
						{
							if(y < 70)
							{
								y += 2000;
							}
							else
							{
								y += 1900;
							}
						}
						return MVCFunctions.mysprintf(new XVar("%04d-%02d-%02d"), (XVar)(new XVar(0, y, 1, m, 2, d)));
					}
					else
					{
						return "";
					}
				}
			}
			else
			{
				if(controlType == "time")
				{
					dynamic ret = null;
					if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(value)))))
					{
						return "";
					}
					ret = XVar.Clone(CommonFunctions.localtime2db((XVar)(value)));
					if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(this.var_type))))
					{
						ret = XVar.Clone(MVCFunctions.Concat("2000-01-01 ", ret));
					}
					return ret;
				}
			}
			return value;
		}
		public virtual XVar getControlMarkup(dynamic var_params, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic additionalCtrlParams = null, fieldNum = null, markup = null, validate = null;
			fieldNum = new XVar(0);
			if(XVar.Pack(var_params["fieldNum"]))
			{
				fieldNum = XVar.Clone(var_params["fieldNum"]);
			}
			validate = XVar.Clone(XVar.Array());
			if(XVar.Pack(var_params["validate"]))
			{
				validate = XVar.Clone(var_params["validate"]);
			}
			additionalCtrlParams = XVar.Clone(XVar.Array());
			if(XVar.Pack(var_params["additionalCtrlParams"]))
			{
				additionalCtrlParams = XVar.Clone(var_params["additionalCtrlParams"]);
			}
			MVCFunctions.ob_start();
			this.buildControl((XVar)(data[this.field]), (XVar)(var_params["mode"]), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			markup = XVar.Clone(MVCFunctions.ob_get_contents());
			MVCFunctions.ob_end_clean();
			return markup;
		}
		public virtual XVar getSuggestCommand(dynamic _param_searchFor, dynamic _param_searchOpt, dynamic _param_numberOfSuggests)
		{
			#region pass-by-value parameters
			dynamic searchFor = XVar.Clone(_param_searchFor);
			dynamic searchOpt = XVar.Clone(_param_searchOpt);
			dynamic numberOfSuggests = XVar.Clone(_param_numberOfSuggests);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, this.getSearchCondition((XVar)(searchFor), (XVar)(searchOpt)), 1, Security.SelectCondition(new XVar("S"), (XVar)(this.pageObject.pSetEdit))))));
			dc.totals.InitAndSetArrayItem(new XVar("field", this.field, "total", "distinct"), null);
			dc.skipAggregated = new XVar(true);
			dc.reccount = XVar.Clone(numberOfSuggests);
			return dc;
		}
		public virtual XVar getDisplayValue(dynamic data)
		{
			dynamic fName = null, htmlType = null, value = null;
			fName = XVar.Clone(this.field);
			htmlType = XVar.Clone(this.pageObject.pSetEdit.getHTML5InputType((XVar)(fName)));
			value = XVar.Clone(data[fName]);
			if(!XVar.Equals(XVar.Pack(this.format), XVar.Pack(Constants.EDIT_FORMAT_READONLY)))
			{
				if((XVar)(CommonFunctions.IsFloatType((XVar)(this.var_type)))  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(value), XVar.Pack(null)))))
				{
					if(MVCFunctions.strtolower((XVar)(htmlType)) == "number")
					{
						value = XVar.Clone(MVCFunctions.formatNumberForHTML5((XVar)(value)));
					}
					else
					{
						value = XVar.Clone(CommonFunctions.formatNumberForEdit((XVar)(value)));
					}
				}
			}
			return value;
		}
		protected virtual XVar maxLengthAttr()
		{
			dynamic maxLength = null;
			maxLength = XVar.Clone(this.pageObject.pSetEdit.getMaxLength((XVar)(this.field)));
			if(XVar.Pack(maxLength))
			{
				return MVCFunctions.Concat(" maxlength=\"", maxLength, "\"");
			}

			return null;
		}
	}
}
