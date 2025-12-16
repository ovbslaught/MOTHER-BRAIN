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
	public partial class ListPage : RunnerPage
	{
		public dynamic gPageSize = XVar.Pack(0);
		protected dynamic orderClause = XVar.Pack(null);
		public dynamic exportTo = XVar.Pack(false);
		public dynamic deleteRecs = XVar.Pack(false);
		public dynamic listFields = XVar.Array();
		protected dynamic recSet = XVar.Pack(null);
		public dynamic nSecOptions = XVar.Pack(0);
		protected dynamic recNo = XVar.Pack(1);
		protected dynamic rowId = XVar.Pack(0);
		protected dynamic selectedRecs = XVar.Array();
		protected dynamic recordsDeleted = XVar.Pack(0);
		public dynamic origTName = XVar.Pack("");
		public dynamic panelSearchFields = XVar.Array();
		public dynamic arrKeyFields = XVar.Array();
		public dynamic audit = XVar.Pack(null);
		public dynamic noRecordsFirstPage = XVar.Pack(false);
		public dynamic mainTableOwnerID = XVar.Pack("");
		public dynamic printFriendly = XVar.Pack(false);
		protected dynamic searchPanel = XVar.Pack(null);
		protected dynamic arrFieldSpanVal = XVar.Array();
		protected dynamic lockDelRec = XVar.Array();
		public dynamic firstTime = XVar.Pack(0);
		protected dynamic gMapFields = XVar.Array();
		protected dynamic recordFieldTypes = XVar.Array();
		protected dynamic hiddenColumnClasses = XVar.Array();
		protected dynamic showAddInPopup = XVar.Pack(false);
		protected dynamic showEditInPopup = XVar.Pack(false);
		protected dynamic showViewInPopup = XVar.Pack(false);
		protected dynamic fieldClasses = XVar.Array();
		public dynamic sortBy;
		protected dynamic addedDetailsCountSubqueries = XVar.Array();
		protected dynamic recordsRenderData = XVar.Array();
		protected dynamic fieldsWithRawValues = XVar.Array();
		protected dynamic editPage = XVar.Pack(null);
		public dynamic requestGoto = XVar.Pack(null);
		protected dynamic lockRecIds = XVar.Array();
		protected static bool skipListPageCtor = false;
		public ListPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipListPageCtor)
			{
				skipListPageCtor = false;
				return;
			}
			GlobalVars.init_runnerpage();
			dynamic allfields = XVar.Array(), i = null;
			this.listFields = XVar.Clone(XVar.Array());
			allfields = XVar.Clone(this.pSet.getPageFields());
			foreach (KeyValuePair<XVar, dynamic> f in allfields.GetEnumerator())
			{
				dynamic fieldTag = null;
				if(XVar.Pack(!(XVar)(this.pSet.appearOnPage((XVar)(f.Value)))))
				{
					continue;
				}
				fieldTag = XVar.Clone((XVar.Pack(this.pdfJsonMode()) ? XVar.Pack(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_pdfvalue")) : XVar.Pack(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_value"))));
				this.listFields.InitAndSetArrayItem(new XVar("fName", f.Value, "goodFieldName", MVCFunctions.GoodFieldName((XVar)(f.Value)), "valueFieldName", fieldTag, "viewFormat", this.pSet.getViewFormat((XVar)(f.Value)), "editFormat", this.pSet.getEditFormat((XVar)(f.Value))), null);
			}
			this.showAddInPopup = XVar.Clone(this.pSet.isShowAddInPopup());
			this.showEditInPopup = XVar.Clone(this.pSet.isShowEditInPopup());
			this.showViewInPopup = XVar.Clone(this.pSet.isShowViewInPopup());
			this.pSet.setPageType(new XVar(Constants.PAGE_SEARCH));
			this.beforeProcessEvent();
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.listFields); i++)
			{
				if(this.listFields[i]["viewFormat"] == Constants.FORMAT_MAP)
				{
					this.gMapFields.InitAndSetArrayItem(i, null);
				}
			}
			this.setLangParams();
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				this.permis.InitAndSetArrayItem(this.getPermissions((XVar)(details.Value)), details.Value);
			}
			this.genId();
			foreach (KeyValuePair<XVar, dynamic> tField in this.totalsFields.GetEnumerator())
			{
				if(tField.Value["totalsType"] == "COUNT")
				{
					this.outputFieldValue((XVar)(tField.Value["fName"]), new XVar(1));
				}
				else
				{
					this.outputFieldValue((XVar)(tField.Value["fName"]), new XVar(2));
				}
			}
			this.createOrderByObject();
			if((XVar)(this.listAjax)  && (XVar)(this.mode != Constants.LIST_DETAILS))
			{
				this.pageData.InitAndSetArrayItem(this.getUrlParams(), "urlParams");
			}
			if(XVar.Pack(this.spreadsheetGridApplicable()))
			{
				this.pageData.InitAndSetArrayItem(true, "spreadsheet");
				this.setupRelatedInlineEditPage();
			}
		}
		protected virtual XVar createOrderByObject()
		{
			this.orderClause = XVar.Clone(OrderClause.createFromPage(this, new XVar(true)));

			return null;
		}
		public virtual XVar buildClickActionSettings()
		{
			dynamic clickAction = XVar.Array();
			if((XVar)(MVCFunctions.strtoupper((XVar)(this.tName)) == "ADMIN_MEMBERS")  || (XVar)(MVCFunctions.strtoupper((XVar)(this.tName)) == "ADMIN_ADMEMBERS"))
			{
				return null;
			}
			clickAction = XVar.Clone(this.pSet.getClickActions());
			foreach (KeyValuePair<XVar, dynamic> fSetting in clickAction["fields"].GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.checkAllowClickAction((XVar)(fSetting.Value)))))
				{
					clickAction.InitAndSetArrayItem("noaction", "fields", fSetting.Key, "action");
				}
			}
			if(XVar.Pack(!(XVar)(this.checkAllowClickAction((XVar)(clickAction["row"])))))
			{
				clickAction.InitAndSetArrayItem("noaction", "row", "action");
			}
			return clickAction;
		}
		public virtual XVar checkAllowClickAction(dynamic _param_actionSet)
		{
			#region pass-by-value parameters
			dynamic actionSet = XVar.Clone(_param_actionSet);
			#endregion

			dynamic isActionAllowed = null;
			isActionAllowed = new XVar(true);
			if(actionSet["action"] == "open")
			{
				switch(((XVar)actionSet["openData"]["page"]).ToString())
				{
					case "add":
						isActionAllowed = XVar.Clone(this.addAvailable());
						break;
					case "edit":
						isActionAllowed = XVar.Clone(this.editAvailable());
						break;
					case "view":
						isActionAllowed = XVar.Clone(this.viewAvailable());
						break;
					case "print":
						isActionAllowed = XVar.Clone(this.printAvailable());
						break;
					default:
						isActionAllowed = new XVar(true);
						break;
				}
			}
			return isActionAllowed;
		}
		public virtual XVar addOrderUrlParam()
		{
			dynamic urlParams = null;
			urlParams = XVar.Clone(this.orderClause.getOrderUrlParams());
			if(0 < MVCFunctions.strlen((XVar)(urlParams)))
			{
				if(XVar.Pack(!(XVar)(this.pageData.KeyExists("urlParams"))))
				{
					this.pageData.InitAndSetArrayItem(XVar.Array(), "urlParams");
				}
				this.pageData.InitAndSetArrayItem(urlParams, "urlParams", "orderby");
			}

			return null;
		}
		public virtual XVar getUrlParams()
		{
			dynamic tabId = null, var_params = XVar.Array();
			var_params = XVar.Clone(XVar.Array());
			if(XVar.Pack(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_qs")]))
			{
				var_params.InitAndSetArrayItem(MVCFunctions.RawUrlEncode((XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_qs")])), "qs");
			}
			if(XVar.Pack(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_q")]))
			{
				var_params.InitAndSetArrayItem(MVCFunctions.RawUrlEncode((XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_q")])), "q");
			}
			if((XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_criteriaSearch")])  && (XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_criteriaSearch")] != "and"))
			{
				var_params.InitAndSetArrayItem(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_criteriaSearch")], "criteria");
			}
			if(XVar.Pack(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_filters")]))
			{
				var_params.InitAndSetArrayItem(MVCFunctions.RawUrlEncode((XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_filters")])), "f");
			}
			if((XVar)((XVar)((XVar)((XVar)(!(XVar)(MVCFunctions.postvalue(new XVar("qs"))))  && (XVar)(!(XVar)(MVCFunctions.postvalue(new XVar("q")))))  && (XVar)(!(XVar)(MVCFunctions.REQUESTKeyExists("f"))))  && (XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")]))  && (XVar)(1 < XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")]))
			{
				var_params.InitAndSetArrayItem(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")], "goto");
			}
			if((XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")])  && (XVar)(this.gPageSize != XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")]))
			{
				var_params.InitAndSetArrayItem(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")], "pagesize");
			}
			if(XVar.Pack(this.masterTable))
			{
				dynamic i = null;
				var_params.InitAndSetArrayItem(this.masterTable, "mastertable");
				i = new XVar(1);
				for(;i <= MVCFunctions.count(this.masterKeysReq); i++)
				{
					var_params.InitAndSetArrayItem(this.masterKeysReq[i], MVCFunctions.Concat("masterkey", i));
				}
			}
			if(XVar.Pack(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_masterpage")]))
			{
				var_params.InitAndSetArrayItem(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_masterpage")], "masterpage");
			}
			if(XVar.Pack(this.searchClauseObj))
			{
				if(XVar.Pack(this.searchClauseObj.savedSearchIsRun))
				{
					var_params.InitAndSetArrayItem(true, "savedSearch");
				}
			}
			tabId = XVar.Clone(this.getCurrentTabId());
			if(XVar.Pack(MVCFunctions.strlen((XVar)(tabId))))
			{
				var_params.InitAndSetArrayItem(MVCFunctions.RawUrlEncode((XVar)(tabId)), "tab");
			}
			return var_params;
		}
		public virtual XVar addCommonHtml()
		{
			if((XVar)(!(XVar)(this.isDashboardElement()))  && (XVar)(!(XVar)(this.pdfJsonMode())))
			{
				this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<div id=\"search_suggest\" class=\"search_suggest\"></div>");
			}
			if(XVar.Pack(this.is508))
			{
				this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<a href=\"#skipdata\" title=\"", CommonFunctions.mlang_message(new XVar("508_SKIP_DATA")), "\" class=\"", this.makeClassName(new XVar("s508")), "\">", CommonFunctions.mlang_message(new XVar("508_SKIP_DATA")), "</a>");
				this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<a href=\"#skipmenu\" title=\"", CommonFunctions.mlang_message(new XVar("508_SKIP_MENU")), "\" class=\"", this.makeClassName(new XVar("s508")), "\">", CommonFunctions.mlang_message(new XVar("508_SKIP_MENU")), "</a>");
				this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<a href=\"#skipsearch\" title=\"", CommonFunctions.mlang_message(new XVar("508_SKIP_SEARCH")), "\" class=\"", this.makeClassName(new XVar("s508")), "\">", CommonFunctions.mlang_message(new XVar("508_SKIP_SEARCH")), "</a>");
				this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<a href=\"templates/helpshortcut.htm\" title=\"", CommonFunctions.mlang_message(new XVar("508_HELP")), "\" class=\"", this.makeClassName(new XVar("s508")), "\">", CommonFunctions.mlang_message(new XVar("508_HELP")), "</a>");
			}
			this.displayMasterTableInfo();

			return null;
		}
		public override XVar addCommonJs()
		{
			base.addCommonJs();
			this.addJsForGrid();
			this.addButtonHandlers();
			if(XVar.Pack(this.spreadsheetGridApplicable()))
			{
				this.editPage.addControlsJSAndCSS();
				this.includes_js = XVar.Clone(MVCFunctions.array_merge((XVar)(this.includes_js), (XVar)(this.editPage.includes_js)));
			}

			return null;
		}
		protected override XVar useRichText()
		{
			dynamic addPSet = null, editPSet = null;
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(ProjectSettings.richTextEnabled())))
			{
				return false;
			}
			pSet = XVar.UnPackProjectSettings(this.pSet);
			addPSet = XVar.Clone(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_ADD)));
			editPSet = XVar.Clone(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_EDIT)));
			foreach (KeyValuePair<XVar, dynamic> f in pSet.getFieldsList().GetEnumerator())
			{
				if((XVar)((XVar)((XVar)(pSet.appearOnInlineAdd((XVar)(f.Value)))  && (XVar)(XVar.Equals(XVar.Pack(addPSet.getEditFormat((XVar)(f.Value))), XVar.Pack(Constants.EDIT_FORMAT_TEXT_AREA))))  && (XVar)(addPSet.isUseRTE((XVar)(f.Value))))  || (XVar)((XVar)((XVar)(pSet.appearOnInlineEdit((XVar)(f.Value)))  && (XVar)(XVar.Equals(XVar.Pack(editPSet.getEditFormat((XVar)(f.Value))), XVar.Pack(Constants.EDIT_FORMAT_TEXT_AREA))))  && (XVar)(editPSet.isUseRTE((XVar)(f.Value)))))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar addJsForGrid()
		{
			if(XVar.Pack(this.isResizeColumns))
			{
				this.prepareForResizeColumns();
			}
			if(XVar.Pack(this.reorderFieldsFeatureEnabled()))
			{
				this.AddJSFile(new XVar("include/jquery.dragtable.js"));
			}
			this.initGmaps();

			return null;
		}
		public virtual XVar prepareForResizeColumns()
		{
			this.AddJSFile(new XVar("include/colresizable.js"));

			return null;
		}
		public override XVar processMasterKeyValue()
		{
			base.processMasterKeyValue();

			return null;
		}
		public virtual XVar beforeProcessEvent()
		{
			if(XVar.Pack(this.eventExists(new XVar("BeforeProcessList"))))
			{
				this.eventsObject.BeforeProcessList(this);
			}

			return null;
		}
		public override XVar setSessionVariables()
		{
			base.setSessionVariables();
			if(XVar.Pack(this.searchClauseObj))
			{
				if(XVar.Pack(this.searchClauseObj.isSearchFunctionalityActivated()))
				{
					CommonFunctions.storageSet((XVar)(MVCFunctions.Concat(this.sessionPrefix, "_advsearch")), (XVar)(MVCFunctions.serialize((XVar)(this.searchClauseObj))));
				}
				else
				{
					CommonFunctions.storageDelete((XVar)(MVCFunctions.Concat(this.sessionPrefix, "_advsearch")));
				}
				if(XVar.Pack(!(XVar)(this.searchClauseObj.filtersActivated)))
				{
					CommonFunctions.storageDelete((XVar)(MVCFunctions.Concat(this.sessionPrefix, "_filters")));
				}
			}
			else
			{
				CommonFunctions.storageDelete((XVar)(MVCFunctions.Concat(this.sessionPrefix, "_advsearch")));
				CommonFunctions.storageDelete((XVar)(MVCFunctions.Concat(this.sessionPrefix, "_filters")));
			}
			if(XVar.Pack(this.requestGoto))
			{
				CommonFunctions.storageSet((XVar)(MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")), (XVar)(this.requestGoto));
			}
			this.myPage = XVar.Clone((int)CommonFunctions.storageGet((XVar)(MVCFunctions.Concat(this.sessionPrefix, "_pagenumber"))));
			if(XVar.Pack(!(XVar)(this.myPage)))
			{
				this.myPage = new XVar(1);
			}
			if(XVar.Pack(!(XVar)(this.pageSize)))
			{
				this.pageSize = XVar.Clone(this.gPageSize);
			}
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.pageSize = XVar.Clone(-1);
			}

			return null;
		}
		protected virtual XVar assignColumnHeaderClasses()
		{
			dynamic field = null, fieldClassStr = null, goodName = null, i = null;
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.listFields); i++)
			{
				field = XVar.Clone(this.listFields[i]["fName"]);
				goodName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(field)));
				fieldClassStr = XVar.Clone(this.fieldClass((XVar)(field)));
				if(XVar.Pack(this.hiddenColumnClasses.KeyExists(goodName)))
				{
					fieldClassStr = MVCFunctions.Concat(fieldClassStr, " ", this.hiddenColumnClasses[goodName]);
				}
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(field)), "_class")), (XVar)(fieldClassStr));
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(field)), "_align")), (XVar)(this.fieldAlign((XVar)(field))));
			}

			return null;
		}
		public virtual XVar isReoderByHeaderClickingEnabled()
		{
			return true;
		}
		public virtual XVar deleteRecords()
		{
			dynamic i = null, message_class = null, selectedKeys = XVar.Array();
			if(MVCFunctions.postvalue("a") != "delete")
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(CommonFunctions.isPostRequest())))
			{
				return null;
			}
			message_class = new XVar("alert-warning");
			this.deleteMessage = new XVar("");
			if(XVar.Pack(MVCFunctions.postvalue("mdelete")))
			{
				foreach (KeyValuePair<XVar, dynamic> ind in MVCFunctions.postvalue("mdelete").GetEnumerator())
				{
					selectedKeys = XVar.Clone(XVar.Array());
					i = new XVar(0);
					for(;i < MVCFunctions.count(this.arrKeyFields); i++)
					{
						selectedKeys.InitAndSetArrayItem(MVCFunctions.postvalue(MVCFunctions.Concat("mdelete", i + 1))[MVCFunctions.mdeleteIndex((XVar)(ind.Value))], this.arrKeyFields[i]);
					}
					this.selectedRecs.InitAndSetArrayItem(selectedKeys, null);
				}
			}
			else
			{
				if(XVar.Pack(MVCFunctions.postvalue(new XVar("selection"))))
				{
					foreach (KeyValuePair<XVar, dynamic> keyblock in MVCFunctions.postvalue(new XVar("selection")).GetEnumerator())
					{
						dynamic arr = XVar.Array();
						arr = XVar.Clone(MVCFunctions.explode(new XVar("&"), (XVar)(keyblock.Value)));
						if(MVCFunctions.count(arr) < MVCFunctions.count(this.arrKeyFields))
						{
							continue;
						}
						selectedKeys = XVar.Clone(XVar.Array());
						i = new XVar(0);
						for(;i < MVCFunctions.count(this.arrKeyFields); i++)
						{
							selectedKeys.InitAndSetArrayItem(MVCFunctions.urldecode((XVar)(arr[i])), this.arrKeyFields[i]);
						}
						this.selectedRecs.InitAndSetArrayItem(selectedKeys, null);
					}
				}
			}
			this.recordsDeleted = new XVar(0);
			this.lockDelRec = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> keys in this.selectedRecs.GetEnumerator())
			{
				dynamic dc = null, deletedResult = null, deleted_values = XVar.Array(), lockRecord = null, retval = null, tdeleteMessage = null, where = null;
				dc = XVar.Clone(this.getDeleteCommand((XVar)(keys.Value)));
				retval = new XVar(true);
				deletedResult = XVar.Clone(this.dataSource.getSingle((XVar)(dc)));
				deleted_values = XVar.Clone((XVar.Pack(deletedResult) ? XVar.Pack(this.cipherer.DecryptFetchedArray((XVar)(deletedResult.fetchAssoc()))) : XVar.Pack(XVar.Array())));
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("IsRecordEditable"), (XVar)(this.tName))))
				{
					if(XVar.Pack(!(XVar)(GlobalVars.globalEvents.IsRecordEditable((XVar)(deleted_values), new XVar(true), (XVar)(this.tName)))))
					{
						continue;
					}
				}
				if((XVar)(this.eventExists(new XVar("BeforeDelete")))  || (XVar)(this.eventExists(new XVar("AfterDelete"))))
				{
					dynamic prep = XVar.Array();
					prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
					where = XVar.Clone(prep["where"]);
				}
				if(XVar.Pack(this.eventExists(new XVar("BeforeDelete"))))
				{
					RunnerContext.pushRecordContext((XVar)(deleted_values), this);
					tdeleteMessage = XVar.Clone(this.deleteMessage);
					retval = XVar.Clone(this.eventsObject.BeforeDelete((XVar)(where), (XVar)(deleted_values), ref tdeleteMessage, this));
					this.deleteMessage = XVar.Clone(tdeleteMessage);
					RunnerContext.pop();
				}
				lockRecord = new XVar(false);
				if(XVar.Pack(this.lockingObj))
				{
					dynamic lockWhere = XVar.Array();
					lockWhere = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> keysvalue in keys.Value.GetEnumerator())
					{
						lockWhere.InitAndSetArrayItem(MVCFunctions.RawUrlEncode((XVar)(keysvalue.Value)), null);
					}
					if(XVar.Pack(this.lockingObj.isLocked((XVar)(this.origTName), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(lockWhere))), new XVar("1"))))
					{
						lockRecord = new XVar(true);
						this.lockDelRec.InitAndSetArrayItem(keys.Value, null);
					}
					if(this.mode == Constants.LIST_SIMPLE)
					{
						XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_lockDelRec")] = this.lockDelRec;
					}
				}
				if((XVar)((XVar)(!(XVar)(lockRecord))  && (XVar)(MVCFunctions.postvalue("a") == "delete"))  && (XVar)(retval))
				{
					this.recordsDeleted++;
					CommonFunctions.DeleteUploadedFiles((XVar)(this.pSet), (XVar)(deleted_values));
					if(XVar.Pack(!(XVar)(this.dataSource.deleteSingle((XVar)(dc)))))
					{
						this.deleteMessage = MVCFunctions.Concat(this.deleteMessage, (XVar.Pack(this.deleteMessage) ? XVar.Pack("<br>") : XVar.Pack("")), this.dataSource.lastError());
					}
					if((XVar)(this.audit)  && (XVar)(deleted_values))
					{
						dynamic deleted_audit_values = XVar.Array(), fieldsList = XVar.Array();
						fieldsList = XVar.Clone(this.pSet.getFieldsList());
						i = new XVar(0);
						foreach (KeyValuePair<XVar, dynamic> value in deleted_values.GetEnumerator())
						{
							if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(this.pSet.getFieldType((XVar)(fieldsList[i]))))))
							{
								deleted_audit_values.InitAndSetArrayItem(value.Value, fieldsList[i]);
							}
							else
							{
								deleted_audit_values.InitAndSetArrayItem(value.Value, value.Key);
							}
							i++;
						}
						this.audit.LogDelete((XVar)(this.tName), (XVar)(deleted_audit_values), (XVar)(keys.Value));
					}
					if((XVar)(this.tName == Constants.ADMIN_USERS)  && (XVar)(Security.dynamicPermissions()))
					{
						dynamic dataSource = null, user = null;
						dataSource = XVar.Clone(Security.getUgMembersDatasource());
						user = XVar.Clone(deleted_values[Security.usernameField()]);
						dc = XVar.Clone(new DsCommand());
						dc.filter = XVar.Clone(DataCondition.FieldEquals(new XVar("UserName"), (XVar)(user), new XVar(0), (XVar)((XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_STRICT)))));
						dataSource.deleteSingle((XVar)(dc), new XVar(false));
					}
					if(XVar.Pack(this.eventExists(new XVar("AfterDelete"))))
					{
						RunnerContext.pushRecordContext((XVar)(deleted_values), this);
						tdeleteMessage = XVar.Clone(this.deleteMessage);
						this.eventsObject.AfterDelete((XVar)(where), (XVar)(deleted_values), ref tdeleteMessage, this);
						this.deleteMessage = XVar.Clone(tdeleteMessage);
						RunnerContext.pop();
						message_class = new XVar("alert-info");
					}
				}
			}
			if((XVar)(MVCFunctions.count(this.selectedRecs))  && (XVar)(this.eventExists(new XVar("AfterMassDelete"))))
			{
				this.eventsObject.AfterMassDelete((XVar)(this.recordsDeleted), this);
			}
			if(XVar.Pack(this.deleteMessage))
			{
				this.xt.assignbyref(new XVar("message"), (XVar)(this.deleteMessage));
				this.xt.assignbyref(new XVar("message_class"), (XVar)(message_class));
				this.xt.assign(new XVar("message_block"), new XVar(true));
			}

			return null;
		}
		public virtual XVar rulePRG()
		{
			if(XVar.Pack(this.stopPRG))
			{
				return false;
			}
			if((XVar)((XVar)(MVCFunctions.no_output_done())  && (XVar)(MVCFunctions.count(this.selectedRecs)))  && (XVar)(!(XVar)(MVCFunctions.strlen((XVar)(this.deleteMessage)))))
			{
				dynamic getParams = null;
				getParams = XVar.Clone(MVCFunctions.implode(new XVar("&"), (XVar)(new XVar(0, "a=return", 1, this.getStateUrlParams()))));
				if(XVar.Pack(MVCFunctions.postvalue(new XVar("page"))))
				{
					getParams = MVCFunctions.Concat(getParams, "&page=", MVCFunctions.postvalue(new XVar("page")));
				}
				MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), (XVar)(this.getPageType()), (XVar)(getParams));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}

			return null;
		}
		public virtual XVar BeforeShowList()
		{
			if(XVar.Pack(this.eventExists(new XVar("BeforeShowList"))))
			{
				dynamic templatefile = null;
				templatefile = XVar.Clone(this.templatefile);
				this.eventsObject.BeforeShowList((XVar)(this.xt), ref templatefile, this);
				this.templatefile = XVar.Clone(templatefile);
			}

			return null;
		}
		public override XVar commonAssign()
		{
			base.commonAssign();
			this.xt.assignbyref(new XVar("body"), (XVar)(this.body));
			this.xt.assign(new XVar("newrecord_controls_block"), (XVar)((XVar)(this.inlineAddAvailable())  || (XVar)(this.addAvailable())));
			if((XVar)((XVar)(this.isDispGrid())  || (XVar)(this.listAjax))  && (XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(this.printAvailable())  || (XVar)(this.exportAvailable()))  || (XVar)(this.deleteAvailable()))  || (XVar)(this.updateSelectedAvailable()))  || (XVar)(this.inlineEditAvailable()))  || (XVar)(this.inlineAddAvailable()))  || (XVar)((XVar)(this.showAddInPopup)  && (XVar)(this.addAvailable()))))
			{
				this.xt.assign(new XVar("record_controls_block"), new XVar(true));
			}
			if(this.numRowsFromSQL == 0)
			{
				this.hideElement(new XVar("recordcontrol"));
			}
			if(XVar.Pack(this.addAvailable()))
			{
				dynamic add_attrs = null;
				this.xt.assign(new XVar("add_link"), new XVar(true));
				add_attrs = XVar.Clone(MVCFunctions.Concat("id=\"addButton", this.id, "\""));
				this.xt.assign(new XVar("addlink_attrs"), (XVar)(add_attrs));
				if(XVar.Pack(this.dashTName))
				{
					this.xt.assign(new XVar("addlink_getparams"), (XVar)(MVCFunctions.Concat("?fromDashboard=", this.dashTName)));
				}
			}
			this.inlineAddLinksAttrs();
			if((XVar)(this.isShowMenu())  || (XVar)(this.isAdminTable()))
			{
				this.xt.assign(new XVar("quickjump_attrs"), (XVar)(MVCFunctions.Concat("class=\"", this.makeClassName(new XVar("quickjump")), "\"")));
			}
			foreach (KeyValuePair<XVar, dynamic> mapId in this.googleMapCfg["mainMapIds"].GetEnumerator())
			{
				this.xt.assign_event((XVar)(mapId.Value), this, new XVar("createMapDiv"), (XVar)(new XVar("mapId", mapId.Value, "width", this.googleMapCfg["mapsData"][mapId.Value]["width"], "height", this.googleMapCfg["mapsData"][mapId.Value]["height"])));
			}
			this.assignSortByDropdown();
			this.addAssignForGrid();
			this.xt.assign(new XVar("grid_block"), new XVar(true));
			this.selectAllLinkAttrs();
			this.editSelectedLinkAttrs();
			this.updateSelectedLinkAttrs();
			this.saveAllLinkAttrs();
			this.cancelAllLinkAttrs();
			this.hideItemType(new XVar("details_preview"));
			this.hideItemType(new XVar("grid_inline_save"));
			this.hideItemType(new XVar("grid_inline_cancel"));

			return null;
		}
		public virtual XVar addAssignForGrid()
		{
			dynamic i = null, record_footer = XVar.Array(), record_header = XVar.Array(), rfooter = null, rheader = null;
			if((XVar)(!(XVar)(this.recordsOnPage))  && (XVar)(this.listAjax))
			{
				this.deleteSelectedLink();
				this.hideItemType(new XVar("delete"));
			}
			if(XVar.Pack(!(XVar)(this.isDispGrid())))
			{
				return null;
			}
			if(XVar.Pack(this.is508))
			{
				if(this.listGridLayout == Constants.gltVERTICAL)
				{
					this.xt.assign_section(new XVar("grid_header"), new XVar("<div style=\"display:none\">Table data</div>"), new XVar(""));
				}
				else
				{
					this.xt.assign_section(new XVar("grid_header"), new XVar("<caption style=\"display:none\">Table data</caption>"), new XVar(""));
				}
			}
			this.xt.assign(new XVar("endrecordblock_attrs"), new XVar("colid=\"endrecord\""));
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.listFields); i++)
			{
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(this.listFields[i]["fName"])), "_fieldheadercolumn")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(this.listFields[i]["fName"])), "_fieldcolumn")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(this.listFields[i]["fName"])), "_fieldfootercolumn")), new XVar(true));
			}
			record_header = XVar.Clone(new XVar("data", XVar.Array()));
			record_footer = XVar.Clone(new XVar("data", XVar.Array()));
			rheader = XVar.Clone(XVar.Array());
			rfooter = XVar.Clone(XVar.Array());
			record_header.InitAndSetArrayItem(rheader, "data", null);
			record_footer.InitAndSetArrayItem(rfooter, "data", null);
			this.xt.assignbyref(new XVar("record_header"), (XVar)(record_header));
			this.xt.assignbyref(new XVar("record_footer"), (XVar)(record_footer));
			this.xt.assign(new XVar("grid_header"), new XVar(true));
			this.xt.assign(new XVar("grid_footer"), new XVar(true));
			this.checkboxColumnAttrs();
			if(XVar.Pack(this.editAvailable()))
			{
				this.xt.assign(new XVar("edit_column"), new XVar(true));
				this.xt.assign(new XVar("edit_headercolumn"), new XVar(true));
				this.xt.assign(new XVar("edit_footercolumn"), new XVar(true));
			}
			if((XVar)(this.inlineEditAvailable())  && (XVar)(!(XVar)(this.spreadsheetGridApplicable())))
			{
				this.xt.assign(new XVar("inlineedit_column"), new XVar(true));
				this.xt.assign(new XVar("inlineedit_headercolumn"), new XVar(true));
				this.xt.assign(new XVar("inlineedit_footercolumn"), new XVar(true));
			}
			if((XVar)(this.inlineEditAvailable())  || (XVar)(this.inlineAddAvailable()))
			{
				this.xt.assign(new XVar("inline_cancel"), new XVar(true));
				this.xt.assign(new XVar("inline_save"), new XVar(true));
			}
			if((XVar)(this.inlineAddAvailable())  && (XVar)(this.spreadsheetGridApplicable()))
			{
				this.xt.assign(new XVar("grid_inline_add_link"), new XVar(true));
			}
			if(XVar.Pack(this.copyAvailable()))
			{
				this.xt.assign(new XVar("copy_column"), new XVar(true));
			}
			if(XVar.Pack(this.displayViewLink()))
			{
				this.xt.assign(new XVar("view_column"), new XVar(true));
			}
			this.assignListIconsColumn();
			if(XVar.Pack(this.detailsInGridAvailable()))
			{
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(this.tName)), "_dtable_column")), new XVar(true));
				this.xt.assign(new XVar("dtables_link"), new XVar(true));
			}
			this.deleteSelectedLink();

			return null;
		}
		public virtual XVar createMapDiv(dynamic var_params)
		{
			MVCFunctions.Echo(MVCFunctions.Concat("<div id=\"", var_params["mapId"], "\" style=\"width: 100%; height: 100%\"></div>"));

			return null;
		}
		public virtual XVar importLinksAttrs()
		{
			this.xt.assign(new XVar("import_link"), (XVar)(this.permis[this.tName]["import"]));
			this.xt.assign(new XVar("importlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"import_", this.id, "\" name=\"import_", this.id, "\"")));

			return null;
		}
		public virtual XVar getInlineAddLinksAttrs()
		{
			return MVCFunctions.Concat("name=\"inlineAdd_", this.id, "\" href='", MVCFunctions.GetTableLink((XVar)(this.shortTableName), new XVar("add"), (XVar)(this.getStateUrlParams())), "' id=\"inlineAdd", this.id, "\"");
		}
		public virtual XVar getAddLinksAttrs()
		{
			return MVCFunctions.Concat("href='", MVCFunctions.GetTableLink((XVar)(this.shortTableName), new XVar("add")), "' id=\"addButton", this.id, "\"");
		}
		public virtual XVar inlineAddLinksAttrs()
		{
			if(XVar.Pack(!(XVar)(this.inlineAddAvailable())))
			{
				return null;
			}
			this.xt.assign(new XVar("inlineadd_link"), new XVar(true));
			this.xt.assign(new XVar("inlineaddlink_attrs"), (XVar)(this.getInlineAddLinksAttrs()));

			return null;
		}
		public virtual XVar selectAllLinkAttrs()
		{
			this.xt.assign(new XVar("selectall_link"), (XVar)((XVar)((XVar)(this.deleteAvailable())  || (XVar)(this.permis[this.tName]["export"]))  || (XVar)(this.permis[this.tName]["edit"])));
			this.xt.assign(new XVar("selectalllink_span"), (XVar)(this.buttonShowHideStyle()));
			this.xt.assign(new XVar("selectalllink_attrs"), (XVar)(MVCFunctions.Concat("name=\"select_all", this.id, "\"\r\n\t\t\tid=\"select_all", this.id, "\"\r\n\t\t\thref=\"#\"")));

			return null;
		}
		public virtual XVar checkboxColumnAttrs()
		{
			dynamic showColumn = null;
			showColumn = XVar.Clone((XVar)((XVar)((XVar)((XVar)(this.deleteAvailable())  || (XVar)(this.exportAvailable()))  || (XVar)(this.inlineEditAvailable()))  || (XVar)(this.printAvailable()))  || (XVar)(this.updateSelectedAvailable()));
			this.xt.assign(new XVar("checkbox_column"), (XVar)(showColumn));
			this.xt.assign(new XVar("checkbox_header"), new XVar(true));
			this.xt.assign(new XVar("checkboxheader_attrs"), (XVar)(MVCFunctions.Concat("id=\"chooseAll_", this.id, "\" class=\"chooseAll", this.id, "\"")));

			return null;
		}
		public virtual XVar getPrintExportLinkAttrs(dynamic _param_page)
		{
			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			#endregion

			if(XVar.Pack(!(XVar)(page)))
			{
				return "";
			}
			return MVCFunctions.Concat("name=\"", page, "_selected", this.id, "\"\r\n\t\t\t\tid=\"", page, "_selected", this.id, "\"\r\n\t\t\t\thref = '", MVCFunctions.GetTableLink((XVar)(this.shortTableName), (XVar)(page)), "'");
		}
		public virtual XVar buttonShowHideStyle(dynamic _param_link = null)
		{
			#region default values
			if(_param_link as Object == null) _param_link = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic link = XVar.Clone(_param_link);
			#endregion

			if((XVar)(link == "saveall")  || (XVar)(link == "cancelall"))
			{
				return " style=\"display:none;\" ";
			}
			return (XVar.Pack(0 < this.numRowsFromSQL) ? XVar.Pack("") : XVar.Pack(" style=\"display:none;\" "));
		}
		public virtual XVar editSelectedLinkAttrs()
		{
			if(XVar.Pack(!(XVar)(this.inlineEditAvailable())))
			{
				return null;
			}
			this.xt.assign(new XVar("editselected_link"), new XVar(true));
			this.xt.assign(new XVar("editselectedlink_span"), (XVar)(this.buttonShowHideStyle()));
			this.xt.assign(new XVar("editselectedlink_attrs"), (XVar)(MVCFunctions.Concat("\r\n\t\t\t\t\thref='", MVCFunctions.GetTableLink((XVar)(this.shortTableName), new XVar("edit")), "'\r\n\t\t\t\t\tname=\"edit_selected", this.id, "\"\r\n\t\t\t\t\tid=\"edit_selected", this.id, "\"")));

			return null;
		}
		public virtual XVar updateSelectedLinkAttrs()
		{
			if(XVar.Pack(!(XVar)(this.updateSelectedAvailable())))
			{
				return null;
			}
			this.xt.assign(new XVar("updateselected_link"), new XVar(true));
			this.xt.assign(new XVar("updateselectedlink_attrs"), (XVar)(MVCFunctions.Concat(this.getUpdateSelectedAttrs(), this.buttonShowHideStyle())));

			return null;
		}
		protected virtual XVar getUpdateSelectedAttrs()
		{
			return MVCFunctions.Concat(" name=\"update_selected", this.id, "\" id=\"update_selected", this.id, "\"");
		}
		public virtual XVar saveAllLinkAttrs()
		{
			if(XVar.Pack(this.spreadsheetGridApplicable()))
			{
				this.xt.assign(new XVar("saveall_link"), new XVar(false));
				return null;
			}
			this.xt.assign(new XVar("saveall_link"), (XVar)((XVar)(this.permis[this.tName]["edit"])  || (XVar)(this.permis[this.tName]["add"])));
			this.xt.assign(new XVar("savealllink_span"), (XVar)(this.buttonShowHideStyle(new XVar("saveall"))));
			this.xt.assign(new XVar("savealllink_attrs"), (XVar)(MVCFunctions.Concat("name=\"saveall_edited", this.id, "\" id=\"saveall_edited", this.id, "\"")));

			return null;
		}
		public virtual XVar cancelAllLinkAttrs()
		{
			if(XVar.Pack(this.spreadsheetGridApplicable()))
			{
				this.xt.assign(new XVar("cancelall_link"), new XVar(false));
				return null;
			}
			this.xt.assign(new XVar("cancelall_link"), (XVar)((XVar)(this.permis[this.tName]["edit"])  || (XVar)(this.permis[this.tName]["add"])));
			this.xt.assign(new XVar("cancelalllink_span"), (XVar)(this.buttonShowHideStyle(new XVar("cancelall"))));
			this.xt.assign(new XVar("cancelalllink_attrs"), (XVar)(MVCFunctions.Concat("name=\"revertall_edited", this.id, "\" id=\"revertall_edited", this.id, "\"")));

			return null;
		}
		public virtual XVar deleteSelectedLink()
		{
			if(XVar.Pack(!(XVar)(this.deleteAvailable())))
			{
				return null;
			}
			this.xt.assign(new XVar("deleteselected_link"), new XVar(true));
			this.xt.assign(new XVar("deleteselectedlink_span"), (XVar)(this.buttonShowHideStyle()));
			this.xt.assign(new XVar("deleteselectedlink_attrs"), (XVar)(this.getDeleteLinksAttrs()));

			return null;
		}
		public virtual XVar getDeleteLinksAttrs()
		{
			return MVCFunctions.Concat("id=\"delete_selected", this.id, "\" name=\"delete_selected", this.id, "\" ");
		}
		public virtual XVar getEditLinksAttrs()
		{
			return MVCFunctions.Concat("id=\"edit_selected", this.id, "\" name=\"edit_selected", this.id, "\" ");
		}
		public virtual XVar getFormInputsHTML()
		{
			return "";
		}
		public virtual XVar getFormTargetHTML()
		{
			return "";
		}
		public override XVar getOrderByClause()
		{
			return this.orderClause.getOrderByExpression();
		}
		protected override XVar isDetailTableSubquerySupported(dynamic _param_details)
		{
			#region pass-by-value parameters
			dynamic details = XVar.Clone(_param_details);
			#endregion

			return (XVar)((XVar)((XVar)((XVar)(GlobalVars.bSubqueriesSupported)  && (XVar)(this.connection.checkDBSubqueriesSupport()))  && (XVar)(GlobalVars.cman.checkTablesSubqueriesSupport((XVar)(this.tName), (XVar)(details))))  && (XVar)(this.checkfDMLinkFieldsOfTheSameType((XVar)(details))))  && (XVar)(!(XVar)(this.gQuery.HasJoinInFromClause()));
		}
		protected virtual XVar useDetailsCountBySubquery()
		{
			dynamic entType = null, manyPages = null;
			entType = XVar.Clone(this.pSet.getEntityType());
			if((XVar)(!XVar.Equals(XVar.Pack(entType), XVar.Pack(Constants.titTABLE)))  && (XVar)(!XVar.Equals(XVar.Pack(entType), XVar.Pack(Constants.titVIEW))))
			{
				return false;
			}
			manyPages = new XVar(false);
			if(0 < this.numRowsFromSQL)
			{
				manyPages = XVar.Clone(this.pageSize / this.numRowsFromSQL < 0.100000);
			}
			if(this.pageSize < 0)
			{
				manyPages = new XVar(false);
			}
			return (XVar)((XVar)((XVar)(0 < MVCFunctions.count(this.pSet.getDetailsTables()))  && (XVar)((XVar)(MVCFunctions.max((XVar)(this.pageSize), (XVar)(this.numRowsFromSQL)) <= 20)  || (XVar)(manyPages)))  && (XVar)(!(XVar)(this.eventExists(new XVar("BeforeQueryList")))))  && (XVar)(!(XVar)(this.eventExists(new XVar("ListGetRowCount"))));
		}
		protected override XVar isDetailTableSubqueryApplied(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			return this.addedDetailsCountSubqueries[table];
		}
		protected virtual XVar checkfDMLinkFieldsOfTheSameType(dynamic _param_details)
		{
			#region pass-by-value parameters
			dynamic details = XVar.Clone(_param_details);
			#endregion

			dynamic detailsKeys = XVar.Array(), detailsPSet = null;
			if(this.connection.dbType == Constants.nDATABASE_MySQL)
			{
				return true;
			}
			detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(details)));
			detailsPSet = XVar.Clone(new ProjectSettings((XVar)(details)));
			foreach (KeyValuePair<XVar, dynamic> mKey in detailsKeys["masterKeys"].GetEnumerator())
			{
				dynamic dKey = null;
				dKey = XVar.Clone(detailsKeys["detailsKeys"][mKey.Key]);
				if(!XVar.Equals(XVar.Pack(this.pSet.getFieldType((XVar)(mKey.Value))), XVar.Pack(detailsPSet.getFieldType((XVar)(dKey)))))
				{
					return false;
				}
			}
			return true;
		}
		protected virtual XVar isInlineAreaToSet()
		{
			return (XVar)((XVar)(this.inlineAddAvailable())  || (XVar)((XVar)(this.addAvailable())  && (XVar)(this.showAddInPopup)))  || (XVar)((XVar)((XVar)(this.spreadsheetGridApplicable())  && (XVar)(this.pSet.addNewRecordAutomatically()))  && (XVar)(this.permis[this.tName]["add"]));
		}
		public virtual XVar prepareInlineAddArea(dynamic rowInfoArr)
		{
			dynamic copylink = null, editlink = null, field = null, gFieldName = null, hideDPLink = null, i = null, record = XVar.Array(), row = XVar.Array();
			rowInfoArr.InitAndSetArrayItem(XVar.Array(), "data");
			if(XVar.Pack(!(XVar)(this.isInlineAreaToSet())))
			{
				return null;
			}
			editlink = new XVar("");
			copylink = new XVar("");
			row = XVar.Clone(XVar.Array());
			row.InitAndSetArrayItem(MVCFunctions.Concat("data-pageid=\"", this.id, "\""), "rowattrs");
			row.InitAndSetArrayItem(MVCFunctions.Concat("gridRowAdd ", this.makeClassName(new XVar("hiddenelem"))), "rowclass");
			row.InitAndSetArrayItem(MVCFunctions.Concat("gridRowSepAdd ", this.makeClassName(new XVar("hiddenelem"))), "rsclass");
			if(this.listGridLayout == Constants.gltVERTICAL)
			{
				row["rowattrs"] = MVCFunctions.Concat(row["rowattrs"], "vertical=\"1\"");
			}
			record = XVar.Clone(XVar.Array());
			record.InitAndSetArrayItem("add", "recId");
			record.InitAndSetArrayItem(true, "edit_link");
			record.InitAndSetArrayItem(true, "inlineedit_link");
			record.InitAndSetArrayItem(true, "view_link");
			record.InitAndSetArrayItem(true, "copy_link");
			record.InitAndSetArrayItem(true, "checkbox");
			record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"editLink_add", this.id, "\" data-gridlink"), "editlink_attrs");
			record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"copyLink_add", this.id, "\" name=\"copyLink_add", this.id, "\" data-gridlink"), "copylink_attrs");
			record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"viewLink_add", this.id, "\" name=\"viewLink_add", this.id, "\" data-gridlink"), "viewlink_attrs");
			record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"check_add", this.id, "\" name=\"selection[]\""), "checkbox_attrs");
			if(XVar.Pack(this.inlineEditAvailable()))
			{
				record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"iEditLink_add", this.id, "\" "), "inlineeditlink_attrs");
			}
			record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"saveLink_add", this.id, "\" href=#"), "inlinesavelink_attrs");
			record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"revertLink_add", this.id, "\" href=#"), "inlinerevertlink_attrs");
			record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"ieditbuttonsholder", this.id, "\" "), "ieditbuttonsholder_attrs");
			if(XVar.Pack(this.detailsInGridAvailable()))
			{
				record.InitAndSetArrayItem(true, "dtables_link");
			}
			hideDPLink = new XVar(false);
			foreach (KeyValuePair<XVar, dynamic> details in this.pSet.getDetailsTables().GetEnumerator())
			{
				dynamic dShortTable = null, htmlAttributes = XVar.Array(), indIdAttr = null, indXTtag = null;
				dShortTable = XVar.Clone(CommonFunctions.GetTableURL((XVar)(details.Value)));
				if(XVar.Pack(this.detailInGridAvailable((XVar)(details.Value))))
				{
					record.InitAndSetArrayItem((XVar)(this.permis[details.Value]["add"])  || (XVar)(this.permis[details.Value]["search"]), MVCFunctions.Concat(dShortTable, "_dtable_link"));
				}
				if(XVar.Pack(this.pSet.detailsShowCount((XVar)(details.Value))))
				{
					record.InitAndSetArrayItem(MVCFunctions.Concat(" id='cntDet_", dShortTable, "_'"), MVCFunctions.Concat(dShortTable, "_childnumber_attr"));
					record.InitAndSetArrayItem(true, MVCFunctions.Concat(dShortTable, "_childcount"));
				}
				indXTtag = XVar.Clone(MVCFunctions.Concat(dShortTable, "_link_attrs"));
				indIdAttr = XVar.Clone(MVCFunctions.Concat("details_add", this.id, "_", dShortTable));
				htmlAttributes = XVar.Clone(XVar.Array());
				record.InitAndSetArrayItem("", indXTtag);
				htmlAttributes.InitAndSetArrayItem(indIdAttr, "id");
				htmlAttributes.InitAndSetArrayItem("display:none;", "style");
				if(XVar.Pack(this.detailsHrefAvailable()))
				{
					htmlAttributes.InitAndSetArrayItem(MVCFunctions.Concat(MVCFunctions.GetTableLink((XVar)(dShortTable), new XVar("list")), "?"), "href");
				}
				if(this.pSet.detailsPreview((XVar)(details.Value)) == Constants.DP_INLINE)
				{
					htmlAttributes.InitAndSetArrayItem(Labels.getTableCaption((XVar)(details.Value)), "caption");
					htmlAttributes.InitAndSetArrayItem(ProjectSettings.defaultPageType((XVar)(CommonFunctions.GetEntityType((XVar)(details.Value)))), "data-pagetype");
				}
				foreach (KeyValuePair<XVar, dynamic> value in htmlAttributes.GetEnumerator())
				{
					record[indXTtag] = MVCFunctions.Concat(record[indXTtag], value.Key, "=\"", value.Value, "\" ");
				}
				if((XVar)(this.pSet.detailsHideEmpty((XVar)(details.Value)))  && (XVar)(!(XVar)(hideDPLink)))
				{
					hideDPLink = new XVar(true);
				}
			}
			record.InitAndSetArrayItem(MVCFunctions.Concat(" href=\"#\" id=\"details_add", this.id, "\" "), "dtables_link_attrs");
			if(XVar.Pack(hideDPLink))
			{
				record["dtables_link_attrs"] = MVCFunctions.Concat(record["dtables_link_attrs"], " data-hidden");
			}
			this.addSpansForGridCells(new XVar("add"), (XVar)(record));
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.listFields); i++)
			{
				field = XVar.Clone(this.listFields[i]["fName"]);
				gFieldName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(field)));
				record[MVCFunctions.Concat(gFieldName, "_class")] = MVCFunctions.Concat(record[MVCFunctions.Concat(gFieldName, "_class")], this.fieldClass((XVar)(field)));
				this.addHiddenColumnClasses((XVar)(record), (XVar)(field));
			}
			record.InitAndSetArrayItem(true, "grid_recordheader");
			record.InitAndSetArrayItem(true, "grid_vrecord");
			row.InitAndSetArrayItem(new XVar("data", XVar.Array()), "grid_record");
			this.setRowsGridRecord((XVar)(row), (XVar)(record));
			rowInfoArr.InitAndSetArrayItem(row, "data", null);

			return null;
		}
		public virtual XVar beforeProccessRow()
		{
			dynamic data = null;
			if(XVar.Pack(this.eventExists(new XVar("ListFetchArray"))))
			{
				data = XVar.Clone(this.eventsObject.ListFetchArray((XVar)(this.recSet), this));
			}
			else
			{
				data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(this.recSet.fetchAssoc())));
			}
			while(XVar.Pack(data))
			{
				if(XVar.Pack(this.eventExists(new XVar("BeforeProcessRowList"))))
				{
					dynamic result = null;
					RunnerContext.pushRecordContext((XVar)(data), this);
					result = XVar.Clone(this.eventsObject.BeforeProcessRowList((XVar)(data), this));
					RunnerContext.pop();
					if(XVar.Pack(!(XVar)(result)))
					{
						if(XVar.Pack(this.eventExists(new XVar("ListFetchArray"))))
						{
							data = XVar.Clone(this.eventsObject.ListFetchArray((XVar)(this.recSet), this));
						}
						else
						{
							data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(this.recSet.fetchAssoc())));
						}
						continue;
					}
				}
				if(XVar.Pack(this.spreadsheetGridApplicable()))
				{
					if(XVar.Pack(this.eventsObject.exists(new XVar("ProcessValuesEdit"))))
					{
						this.eventsObject.ProcessValuesEdit((XVar)(data), this);
					}
				}
				return data;
			}

			return null;
		}
		public virtual XVar assignListIconsColumn()
		{
			if((XVar)((XVar)((XVar)(this.inlineEditAvailable())  || (XVar)(this.inlineAddAvailable()))  || (XVar)(this.displayViewLink()))  || (XVar)(this.editAvailable()))
			{
				this.xt.assign(new XVar("listIcons_column"), new XVar(true));
			}

			return null;
		}
		public virtual XVar fillGridData()
		{
			dynamic _notEmptyFieldColumns = XVar.Array(), copylink = null, currentPageSize = null, currentRow = XVar.Array(), data = XVar.Array(), defaultPages = XVar.Array(), displayRecCount = null, editlink = null, field = null, fieldsToHideIfEmpty = null, gridRowInd = null, i = null, isDeletable = null, isEditable = null, keyValue = null, keyValueHtml = null, keyblock = null, keylink = null, keys = XVar.Array(), pageName = null, prewData = XVar.Array(), record = XVar.Array(), row = XVar.Array(), rowinfo = XVar.Array(), state = null, tKeys = XVar.Array(), totals = null;
			totals = XVar.Clone(XVar.Array());
			rowinfo = XVar.Clone(XVar.Array());
			this.prepareInlineAddArea((XVar)(rowinfo));
			this.setDetailsBadgeStyles();
			data = XVar.Clone(this.beforeProccessRow());
			prewData = new XVar(false);
			this.lockRecIds = XVar.Clone(XVar.Array());
			tKeys = XVar.Clone(this.pSet.getTableKeys());
			this.controlsMap.InitAndSetArrayItem(XVar.Array(), "gridRows");
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.listFields); i++)
			{
				this.recordFieldTypes.InitAndSetArrayItem(this.pSet.getFieldType((XVar)(this.listFields[i]["fName"])), this.listFields[i]["fName"]);
			}
			currentPageSize = XVar.Clone(this.pageSize);
			if((XVar)(this.pSet.getRecordsLimit())  && (XVar)(this.maxPages == this.myPage))
			{
				currentPageSize = XVar.Clone(MVCFunctions.min((XVar)(this.pageSize), (XVar)(this.pSet.getRecordsLimit() - (this.myPage - 1) * this.pageSize)));
			}
			_notEmptyFieldColumns = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.calcAllDataTotals()))
			{
				this.pageData.InitAndSetArrayItem(true, "allDataTotals");
			}
			pageName = new XVar("");
			defaultPages = XVar.Clone(this.pSet.getDefaultPages());
			if(XVar.Pack(defaultPages))
			{
				pageName = XVar.Clone((XVar.Pack(!XVar.Equals(XVar.Pack(defaultPages[this.pageType]), XVar.Pack(this.pageName))) ? XVar.Pack(MVCFunctions.Concat("&listPage=", this.pageName)) : XVar.Pack("")));
			}
			displayRecCount = new XVar(0);
			while((XVar)(data)  && (XVar)((XVar)(this.recNo <= currentPageSize)  || (XVar)(currentPageSize == -1)))
			{
				++(displayRecCount);
				row = XVar.Clone(XVar.Array());
				RunnerContext.pushRecordContext((XVar)(data), this);
				row.InitAndSetArrayItem(XVar.Array(), "grid_record");
				row.InitAndSetArrayItem(XVar.Array(), "grid_record", "data");
				this.rowId++;
				if(XVar.Pack(!(XVar)(this.calcAllDataTotals())))
				{
					this.countTotals((XVar)(totals), (XVar)(data));
				}
				record = XVar.Clone(XVar.Array());
				this.genId();
				row.InitAndSetArrayItem(MVCFunctions.Concat(" id=\"gridRow", this.recId, "\""), "rowattrs");
				record.InitAndSetArrayItem(this.recId, "recId");
				gridRowInd = XVar.Clone(MVCFunctions.count(this.controlsMap["gridRows"]));
				this.controlsMap.InitAndSetArrayItem(XVar.Array(), "gridRows", gridRowInd);
				currentRow = this.controlsMap["gridRows"][gridRowInd];
				currentRow.InitAndSetArrayItem(this.recId, "id");
				currentRow.InitAndSetArrayItem(gridRowInd, "rowInd");
				currentRow.InitAndSetArrayItem(this.recId, "contextRowId");
				isEditable = XVar.Clone(Security.userCan(new XVar("E"), (XVar)(this.tName), new XVar(true), (XVar)(data[this.mainTableOwnerID])));
				isDeletable = XVar.Clone(Security.userCan(new XVar("D"), (XVar)(this.tName), new XVar(true), (XVar)(data[this.mainTableOwnerID])));
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("IsRecordEditable"), (XVar)(this.tName))))
				{
					isEditable = XVar.Clone(GlobalVars.globalEvents.IsRecordEditable((XVar)(data), (XVar)(isEditable), (XVar)(this.tName)));
					isDeletable = XVar.Clone((XVar)(isDeletable)  && (XVar)(isEditable));
				}
				currentRow.InitAndSetArrayItem(isEditable, "isEditOwnRow");
				currentRow.InitAndSetArrayItem(isEditable, "canEditRecord");
				currentRow.InitAndSetArrayItem(isDeletable, "canDeleteRecord");
				if((XVar)(this.tName == Constants.ADMIN_USERS)  && (XVar)(Security.dynamicPermissions()))
				{
					if(XVar.Pack(Security.currentUserRecord((XVar)(data))))
					{
						currentRow.InitAndSetArrayItem(false, "canDeleteRecord");
					}
				}
				currentRow.InitAndSetArrayItem(this.listGridLayout, "gridLayout");
				currentRow.InitAndSetArrayItem(XVar.Array(), "keyFields");
				currentRow.InitAndSetArrayItem(XVar.Array(), "keys");
				i = new XVar(0);
				for(;i < MVCFunctions.count(tKeys); i++)
				{
					currentRow.InitAndSetArrayItem(tKeys[i], "keyFields", i);
					currentRow.InitAndSetArrayItem(data[tKeys[i]], "keys", i);
				}
				if(XVar.Pack(this.pSet.reorderRows()))
				{
					currentRow.InitAndSetArrayItem(data[this.pSet.reorderRowsField()], "order");
				}
				record.InitAndSetArrayItem(isEditable, "edit_link");
				record.InitAndSetArrayItem(isEditable, "inlineedit_link");
				record.InitAndSetArrayItem(this.permis[this.tName]["search"], "view_link");
				record.InitAndSetArrayItem(this.permis[this.tName]["add"], "copy_link");
				if(XVar.Pack(this.lockingObj))
				{
					dynamic lockDelRec = null;
					if((XVar)((XVar)(this.mode == Constants.LIST_SIMPLE)  && (XVar)(!(XVar)(this.lockDelRec)))  && (XVar)(XSession.Session.KeyExists(MVCFunctions.Concat(this.sessionPrefix, "_lockDelRec"))))
					{
						this.lockDelRec = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_lockDelRec")]);
						XSession.Session.Remove(MVCFunctions.Concat(this.sessionPrefix, "_lockDelRec"));
					}
					i = new XVar(0);
					for(;(XVar)(!(XVar)(!(XVar)(this.lockDelRec)))  && (XVar)(i < MVCFunctions.count(this.lockDelRec)); i++)
					{
						lockDelRec = new XVar(true);
						foreach (KeyValuePair<XVar, dynamic> val in this.lockDelRec[i].GetEnumerator())
						{
							if(data[val.Key] != val.Value)
							{
								lockDelRec = new XVar(false);
								break;
							}
						}
						if(XVar.Pack(lockDelRec))
						{
							this.lockRecIds.InitAndSetArrayItem(this.recId, null);
							break;
						}
					}
				}
				this.proccessDetailGridInfo((XVar)(record), (XVar)(data), (XVar)(gridRowInd));
				keyblock = new XVar("");
				editlink = new XVar("");
				copylink = new XVar("");
				keylink = new XVar("");
				keys = XVar.Clone(XVar.Array());
				i = new XVar(0);
				for(;i < MVCFunctions.count(tKeys); i++)
				{
					if(i != XVar.Pack(0))
					{
						keyblock = MVCFunctions.Concat(keyblock, "&");
						editlink = MVCFunctions.Concat(editlink, "&");
						copylink = MVCFunctions.Concat(copylink, "&");
					}
					keyValue = XVar.Clone(MVCFunctions.RawUrlEncode((XVar)(data[tKeys[i]])));
					keyValueHtml = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(keyValue)));
					keyblock = MVCFunctions.Concat(keyblock, keyValue);
					editlink = MVCFunctions.Concat(editlink, "editid", i + 1, "=", keyValueHtml);
					copylink = MVCFunctions.Concat(copylink, "copyid", i + 1, "=", keyValueHtml);
					keylink = MVCFunctions.Concat(keylink, "&key", i + 1, "=", keyValueHtml);
					keys.InitAndSetArrayItem(data[tKeys[i]], i);
				}
				this.recIds.InitAndSetArrayItem(this.recId, null);
				record.InitAndSetArrayItem(MVCFunctions.Concat("data-record-id=\"", this.recId, "\""), "recordattrs");
				record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"editLink", this.recId, "\" name=\"editLink", this.recId, "\" data-gridlink"), "editlink_attrs");
				record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"copyLink", this.recId, "\" name=\"copyLink", this.recId, "\" data-gridlink"), "copylink_attrs");
				record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"viewLink", this.recId, "\" name=\"viewLink", this.recId, "\" data-gridlink"), "viewlink_attrs");
				state = XVar.Clone(this.getStateUrlParams());
				record.InitAndSetArrayItem(MVCFunctions.Concat(editlink, pageName, (XVar.Pack(state) ? XVar.Pack(MVCFunctions.Concat("&", state)) : XVar.Pack(""))), "editlink");
				record.InitAndSetArrayItem(MVCFunctions.Concat(copylink, (XVar.Pack(state) ? XVar.Pack(MVCFunctions.Concat("&", state)) : XVar.Pack(""))), "copylink");
				record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"iEditLink", this.recId, "\" name=\"iEditLink", this.recId, "\" href='", MVCFunctions.GetTableLink((XVar)(this.shortTableName), new XVar("edit"), (XVar)(editlink)), "' data-gridlink"), "inlineeditlink_attrs");
				record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"saveLink", this.recId, "\" href=#"), "inlinesavelink_attrs");
				record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"revertLink", this.recId, "\" href=#"), "inlinerevertlink_attrs");
				record.InitAndSetArrayItem(MVCFunctions.Concat("id=\"ieditbuttonsholder", this.recId, "\" "), "ieditbuttonsholder_attrs");
				this.fillCheckAttr((XVar)(record), (XVar)(data), (XVar)(keyblock));
				if(XVar.Pack(this.detailsInGridAvailable()))
				{
					record.InitAndSetArrayItem(true, "dtables_link");
				}
				if(XVar.Pack(this.hasBigMap()))
				{
					this.addBigGoogleMapMarkers((XVar)(data), (XVar)(keys), (XVar)(editlink));
				}
				fieldsToHideIfEmpty = XVar.Clone(this.pSet.getFieldsToHideIfEmpty());
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.listFields); i++)
				{
					if(XVar.Pack(this.checkFieldHasMapData((XVar)(i))))
					{
						this.addGoogleMapData((XVar)(this.listFields[i]["fName"]), (XVar)(data), (XVar)(keys), (XVar)(editlink));
					}
					record.InitAndSetArrayItem(this.proccessRecordValue((XVar)(data), (XVar)(keylink), (XVar)(this.listFields[i]), (XVar)(isEditable)), this.listFields[i]["valueFieldName"]);
					if(XVar.Pack(MVCFunctions.in_array((XVar)(this.listFields[i]["fName"]), (XVar)(fieldsToHideIfEmpty))))
					{
						if((XVar)(this.listGridLayout != Constants.gltHORIZONTAL)  && (XVar)(record[this.listFields[i]["valueFieldName"]] == ""))
						{
							this.hideField((XVar)(this.listFields[i]["fName"]), (XVar)(this.recId));
						}
						else
						{
							if((XVar)(this.listGridLayout == Constants.gltHORIZONTAL)  && (XVar)(record[this.listFields[i]["valueFieldName"]] != ""))
							{
								_notEmptyFieldColumns.InitAndSetArrayItem(true, this.listFields[i]["fName"]);
							}
						}
					}
				}
				this.addSpansForGridCells(new XVar("edit"), (XVar)(record), (XVar)(data));
				if(XVar.Pack(this.eventExists(new XVar("BeforeMoveNextList"))))
				{
					RunnerContext.pushRecordContext((XVar)(data), this);
					this.eventsObject.BeforeMoveNextList((XVar)(data), (XVar)(row), (XVar)(record), (XVar)(record["recId"]), this);
					RunnerContext.pop();
				}
				this.spreadRowStyles((XVar)(data), (XVar)(row), (XVar)(record));
				this.setRowCssRules((XVar)(record));
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.listFields); i++)
				{
					field = XVar.Clone(this.listFields[i]["fName"]);
					this.setRowClassNames((XVar)(record), (XVar)(field));
					this.addHiddenColumnClasses((XVar)(record), (XVar)(field));
				}
				record.InitAndSetArrayItem(true, "grid_recordheader");
				record.InitAndSetArrayItem(true, "grid_vrecord");
				this.setRowsGridRecord((XVar)(row), (XVar)(record));
				if(XVar.Pack(prewData))
				{
					dynamic grFields = XVar.Array();
					grFields = XVar.Clone(this.pSet.getGroupFields());
					foreach (KeyValuePair<XVar, dynamic> grF in grFields.GetEnumerator())
					{
						if(data[grF.Value] != prewData[grF.Value])
						{
							break;
						}
						foreach (KeyValuePair<XVar, dynamic> fItemId in this.pSet.getFieldItems((XVar)(grF.Value)).GetEnumerator())
						{
							this.hideItem((XVar)(fItemId.Value), (XVar)(this.recId));
						}
					}
				}
				prewData = XVar.Clone(data);
				RunnerContext.pop();
				data = XVar.Clone(this.beforeProccessRow());
				this.recNo++;
				rowinfo.InitAndSetArrayItem(row, "data", null);
				this.recordsRenderData.InitAndSetArrayItem(rowinfo["data"][MVCFunctions.count(rowinfo["data"]) - 1], record["recId"]);
			}
			if(XVar.Pack(this.pSet.hideNumberOfRecords()))
			{
				this.numRowsFromSQL = XVar.Clone(displayRecCount);
				if((XVar)((XVar)(this.pageSize)  && (XVar)(this.pageSize != -1))  && (XVar)(1 < this.myPage))
				{
					this.numRowsFromSQL += this.pageSize * (this.myPage - 1);
				}
			}
			if(XVar.Pack(MVCFunctions.count(rowinfo["data"])))
			{
				if((XVar)(this.listGridLayout == Constants.gltVERTICAL)  && (XVar)(this.is508))
				{
					rowinfo.InitAndSetArrayItem("<div style=\"display:none\">Table data</div>", "begin");
				}
				this.xt.assignbyref(new XVar("grid_row"), (XVar)(rowinfo));
			}
			this.assignTotals((XVar)(totals));
			if(this.listGridLayout == Constants.gltHORIZONTAL)
			{
				foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getFieldsToHideIfEmpty().GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(_notEmptyFieldColumns[f.Value])))
					{
						this.hideField((XVar)(f.Value));
					}
				}
			}

			return null;
		}
		protected virtual XVar hasBigMap()
		{
			return this.pSet.hasMap();
		}
		protected virtual XVar checkFieldHasMapData(dynamic _param_fIndex)
		{
			#region pass-by-value parameters
			dynamic fIndex = XVar.Clone(_param_fIndex);
			#endregion

			return MVCFunctions.in_array((XVar)(fIndex), (XVar)(this.gMapFields));
		}
		protected virtual XVar addHiddenColumnClasses(dynamic record, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic gFieldName = null;
			gFieldName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(field)));
			if(XVar.Pack(this.hiddenColumnClasses.KeyExists(gFieldName)))
			{
				record[MVCFunctions.Concat(gFieldName, "_class")] = MVCFunctions.Concat(record[MVCFunctions.Concat(gFieldName, "_class")], " ", this.hiddenColumnClasses[gFieldName]);
				if(this.listGridLayout != Constants.gltHORIZONTAL)
				{
					record.InitAndSetArrayItem(this.hiddenColumnClasses[gFieldName], MVCFunctions.Concat(gFieldName, "_label_class"));
				}
			}

			return null;
		}
		public override XVar fieldClass(dynamic _param_f)
		{
			#region pass-by-value parameters
			dynamic f = XVar.Clone(_param_f);
			#endregion

			if(XVar.Pack(!(XVar)(this.fieldClasses.KeyExists(f))))
			{
				this.fieldClasses.InitAndSetArrayItem(this.calcFieldClass((XVar)(f)), f);
			}
			return this.fieldClasses[f];
		}
		public virtual XVar calcFieldClass(dynamic _param_f)
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
			if((XVar)(this.listGridLayout == Constants.gltVERTICAL)  || (XVar)(this.listGridLayout == Constants.gltCOLUMNS))
			{
				return "";
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
		protected virtual XVar setRowHoverCssRule(dynamic _param_rowHoverCssRule, dynamic _param_fieldName = null)
		{
			#region default values
			if(_param_fieldName as Object == null) _param_fieldName = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic rowHoverCssRule = XVar.Clone(_param_rowHoverCssRule);
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			if((XVar)(this.listGridLayout != Constants.gltHORIZONTAL)  && (XVar)(this.listGridLayout != Constants.gltFLEXIBLE))
			{
				return null;
			}
			if(XVar.Pack(fieldName))
			{
				dynamic className = null;
				className = XVar.Clone(MVCFunctions.Concat("rnr-style", this.recId, "-", fieldName));
				this.row_css_rules = MVCFunctions.Concat(this.row_css_rules, " tr:hover > td.", className, ".", className, "{", this.getCustomCSSRule((XVar)(rowHoverCssRule)), "}\n");
				return className;
			}
			else
			{
				this.row_css_rules = XVar.Clone(MVCFunctions.Concat(" tr[id=\"gridRow", this.recId, "\"]:hover > td:not(.rnr-cs){", this.getCustomCSSRule((XVar)(rowHoverCssRule)), "}\n", this.row_css_rules));
				return "";
			}

			return null;
		}
		protected virtual XVar calcAllDataTotals()
		{
			dynamic currentPageSize = null;
			if(XVar.Pack(this.pSet.hideNumberOfRecords()))
			{
				return false;
			}
			currentPageSize = XVar.Clone(this.pageSize);
			if((XVar)(this.pSet.getRecordsLimit())  && (XVar)(this.maxPages == this.myPage))
			{
				currentPageSize = XVar.Clone(this.pSet.getRecordsLimit() - (this.myPage - 1) * this.pageSize);
			}
			return (XVar)((XVar)(this.pSet.calcTotalsFor() == Constants.TOTALS_ALL_DATA)  && (XVar)(currentPageSize != -1))  && (XVar)(currentPageSize < this.numRowsFromSQL);
		}
		protected override XVar getTotalDataCommand()
		{
			dynamic dc = null;
			dc = XVar.Clone(base.getSubsetDataCommand());
			if((XVar)((XVar)(this.mode != Constants.LIST_DETAILS)  && (XVar)(this.noRecordsFirstPage))  && (XVar)(!(XVar)(this.isSearchFunctionalityActivated())))
			{
				dc.filter = XVar.Clone(DataCondition._False());
			}
			return dc;
		}
		protected virtual XVar assignTotals(dynamic totals)
		{
			if(XVar.Pack(this.calcAllDataTotals()))
			{
				this.buildAllDataTotals();
			}
			else
			{
				this.buildTotals((XVar)(totals));
			}

			return null;
		}
		public override XVar buildTotals(dynamic totals)
		{
			if(XVar.Pack(MVCFunctions.count(this.totalsFields)))
			{
				dynamic goodName = null, i = null, record = null, total = null, totals_records = XVar.Array();
				this.xt.assign(new XVar("totals_row"), new XVar(true));
				totals_records = XVar.Clone(new XVar("data", XVar.Array()));
				record = XVar.Clone(XVar.Array());
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.totalsFields); i++)
				{
					goodName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(this.totalsFields[i]["fName"])));
					total = XVar.Clone(CommonFunctions.GetTotals((XVar)(this.totalsFields[i]["fName"]), (XVar)(totals[this.totalsFields[i]["fName"]]), (XVar)(this.totalsFields[i]["totalsType"]), (XVar)(this.totalsFields[i]["numRows"]), (XVar)(this.totalsFields[i]["viewFormat"]), new XVar(Constants.PAGE_LIST), (XVar)(this.pSet), new XVar(false), this));
					if(XVar.Pack(!(XVar)(this.pdfJsonMode())))
					{
						total = XVar.Clone(MVCFunctions.Concat("<span id=\"total", this.id, "_", goodName, "\">", total, "</span>"));
					}
					this.xt.assign((XVar)(MVCFunctions.Concat(goodName, "_total")), (XVar)(total));
					this.xt.assign((XVar)(MVCFunctions.Concat(goodName, "_showtotal")), new XVar(true));
				}
				totals_records.InitAndSetArrayItem(record, "data", null);
				this.xt.assignbyref(new XVar("totals_record"), (XVar)(totals_records));
				if(XVar.Pack(!(XVar)(this.recordsOnPage)))
				{
					this.xt.assign(new XVar("totals_attr"), new XVar("style='display:none;'"));
				}
			}

			return null;
		}
		public virtual XVar outputFieldValue(dynamic _param_field, dynamic _param_state)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic state = XVar.Clone(_param_state);
			#endregion

			this.arrFieldSpanVal.InitAndSetArrayItem(state, field);

			return null;
		}
		public virtual XVar addSpanVal(dynamic _param_fName, dynamic data)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic fieldValue = null;
			if(XVar.Pack(!(XVar)(this.printRawValue((XVar)(fName)))))
			{
				return null;
			}
			fieldValue = XVar.Clone(data[fName]);
			return MVCFunctions.Concat("val=\"", MVCFunctions.runner_htmlspecialchars((XVar)(fieldValue)), "\" ");
		}
		public virtual XVar addSpansForGridCells(dynamic _param_type, dynamic record, dynamic _param_data = null)
		{
			#region default values
			if(_param_data as Object == null) _param_data = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic i = null, span = null;
			if(XVar.Pack(this.pdfJsonMode()))
			{
				return null;
			}
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.listFields); i++)
			{
				span = XVar.Clone(MVCFunctions.Concat("<span id=\"", var_type, (XVar.Pack(var_type == "edit") ? XVar.Pack(this.recId) : XVar.Pack(this.id)), "_", this.listFields[i]["goodFieldName"], "\" "));
				if(var_type == "edit")
				{
					span = MVCFunctions.Concat(span, this.addSpanVal((XVar)(this.listFields[i]["fName"]), (XVar)(data)), ">");
					span = MVCFunctions.Concat(span, record[this.listFields[i]["valueFieldName"]]);
				}
				else
				{
					span = MVCFunctions.Concat(span, ">");
				}
				span = MVCFunctions.Concat(span, "</span>");
				record.InitAndSetArrayItem(span, this.listFields[i]["valueFieldName"]);
			}

			return null;
		}
		public virtual XVar proccessRecordValue(dynamic data, dynamic keylink, dynamic _param_listFieldInfo, dynamic _param_isEditable = null)
		{
			#region default values
			if(_param_isEditable as Object == null) _param_isEditable = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic listFieldInfo = XVar.Clone(_param_listFieldInfo);
			dynamic isEditable = XVar.Clone(_param_isEditable);
			#endregion

			dynamic dbVal = null, fName = null;
			fName = XVar.Clone(listFieldInfo["fName"]);
			if((XVar)(this.spreadsheetGridApplicable())  && (XVar)(isEditable))
			{
				if(XVar.Pack(MVCFunctions.in_array((XVar)(fName), (XVar)(this.pSet.getInlineEditFields()))))
				{
					return this.getInlineEditControl((XVar)(fName), (XVar)(this.recId), (XVar)(data));
				}
			}
			dbVal = XVar.Clone(this.showDBValue((XVar)(fName), (XVar)(data), (XVar)(keylink)));
			return this.addCenterLink(ref dbVal, (XVar)(fName));
		}
		protected virtual XVar getInlineEditControl(dynamic _param_fName, dynamic _param_recId, dynamic data)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			#endregion

			dynamic controls = null, ctrl = null, ctrlParams = XVar.Array();
			controls = XVar.Clone(this.editPage.getContolMapData((XVar)(fName), (XVar)(recId), (XVar)(data), (XVar)(this.editPage.editFields)));
			this.fillControlsMap((XVar)(controls));
			if(this.editPage.getEditFormat((XVar)(fName)) == Constants.EDIT_FORMAT_READONLY)
			{
				this.editPage.readOnlyFields.InitAndSetArrayItem(this.showDBValue((XVar)(fName), (XVar)(data)), fName);
			}
			ctrlParams = XVar.Clone(this.editPage.getEditContolParams((XVar)(fName), (XVar)(recId), (XVar)(data)));
			ctrlParams.InitAndSetArrayItem(Constants.MODE_INLINE_EDIT, "mode");
			ctrlParams.InitAndSetArrayItem(true, "extraParams", "spreadsheet");
			ctrl = XVar.Clone(this.editPage.getControl((XVar)(fName), (XVar)(recId), (XVar)(ctrlParams["extraParams"])));
			return ctrl.getControlMarkup((XVar)(ctrlParams), (XVar)(data));
		}
		public virtual XVar addCenterLink(ref dynamic value, dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			if(XVar.Pack(!(XVar)(this.googleMapCfg["isUseMainMaps"])))
			{
				return value;
			}
			foreach (KeyValuePair<XVar, dynamic> mapId in this.googleMapCfg["mainMapIds"].GetEnumerator())
			{
				if((XVar)(this.googleMapCfg["mapsData"][mapId.Value]["addressField"] != fName)  || (XVar)(!(XVar)(this.googleMapCfg["mapsData"][mapId.Value]["showCenterLink"])))
				{
					continue;
				}
				if(XVar.Equals(XVar.Pack(this.googleMapCfg["mapsData"][mapId.Value]["showCenterLink"]), XVar.Pack(1)))
				{
					value = XVar.Clone(this.googleMapCfg["mapsData"][mapId.Value]["centerLinkText"]);
				}
				return MVCFunctions.Concat("<a href=\"#\" type=\"centerOnMarker", this.id, "\" recId=\"", this.recId, "\">", value, "</a>");
			}
			return value;
		}
		public virtual XVar isDispGrid()
		{
			if((XVar)(this.permis[this.tName]["search"])  && (XVar)(this.recordsOnPage))
			{
				return true;
			}
			if((XVar)(this.inlineAddAvailable())  || (XVar)((XVar)(this.addAvailable())  && (XVar)(this.showAddInPopup)))
			{
				return true;
			}
			if(XVar.Pack(this.fieldFilterEnabled()))
			{
				return true;
			}
			return false;
		}
		public virtual XVar fillCheckAttr(dynamic record, dynamic _param_data, dynamic _param_keyblock)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic keyblock = XVar.Clone(_param_keyblock);
			#endregion

			record.InitAndSetArrayItem(true, "checkbox");
			record.InitAndSetArrayItem(MVCFunctions.Concat("name=\"selection[]\" ", "value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(keyblock)), "\" ", "id=\"check", this.id, "_", this.recId, "\" "), "checkbox_attrs");

			return null;
		}
		protected virtual XVar getRecordset()
		{
			dynamic keys = XVar.Array(), orderField = null, rs = null, updateCommand = null, vResult = XVar.Array();
			rs = XVar.Clone(this.dataSource.getList((XVar)(this.queryCommand)));
			if((XVar)(!(XVar)(rs))  || (XVar)(!(XVar)(this.pSet.reorderRows())))
			{
				return rs;
			}
			vResult = XVar.Clone(this.getVerifiedOrderRecordset((XVar)(rs)));
			rs = XVar.Clone(vResult["rs"]);
			if(XVar.Pack(!(XVar)(vResult["hasNulls"])))
			{
				return rs;
			}
			orderField = XVar.Clone(this.pSet.reorderRowsField());
			keys = XVar.Clone(this.pSet.getTableKeys());
			updateCommand = XVar.Clone(new DsCommand());
			updateCommand.advValues.InitAndSetArrayItem(new DsOperand(new XVar(Constants.dsotROWNO), new XVar("")), this.pSet.reorderRowsField());
			updateCommand.order = XVar.Clone(this.queryCommand.order);
			if(XVar.Pack(!(XVar)(this.pSet.inlineAddBottom())))
			{
				updateCommand.invertOrder();
			}
			if(XVar.Pack(this.dataSource.updateRowNumberAvailable((XVar)(updateCommand))))
			{
				updateCommand.filter = XVar.Clone(DataCondition.FieldIs((XVar)(this.pSet.reorderRowsField()), new XVar(Constants.dsopEMPTY), new XVar("")));
				this.dataSource.updateRowNumber((XVar)(updateCommand), (XVar)(this.getMaxOrderValue((XVar)(this.pSet))));
			}
			else
			{
				dynamic data = XVar.Array(), keyValues = XVar.Array(), maxOrder = null, orderRs = null, rKeys = XVar.Array(), selectCommand = null;
				selectCommand = XVar.Clone(new DsCommand());
				selectCommand.filter = XVar.Clone(DataCondition.FieldIs((XVar)(this.pSet.reorderRowsField()), new XVar(Constants.dsopEMPTY), new XVar("")));
				selectCommand.order = XVar.Clone(this.queryCommand.order);
				if(XVar.Pack(!(XVar)(this.pSet.inlineAddBottom())))
				{
					selectCommand.invertOrder();
				}
				rKeys = XVar.Clone(XVar.Array());
				orderRs = XVar.Clone(this.dataSource.getList((XVar)(selectCommand)));
				while(XVar.Pack(data = XVar.Clone(orderRs.fetchAssoc())))
				{
					keyValues = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> k in keys.GetEnumerator())
					{
						keyValues.InitAndSetArrayItem(data[k.Value], k.Value);
					}
					rKeys.InitAndSetArrayItem(keyValues, null);
				}
				maxOrder = XVar.Clone(this.getMaxOrderValue((XVar)(this.pSet)));
				foreach (KeyValuePair<XVar, dynamic> _keyValues in rKeys.GetEnumerator())
				{
					updateCommand = XVar.Clone(new DsCommand());
					updateCommand.keys = XVar.Clone(_keyValues.Value);
					updateCommand.values = XVar.Clone(XVar.Array());
					updateCommand.values.InitAndSetArrayItem(++(maxOrder), this.pSet.reorderRowsField());
					this.dataSource.updateSingle((XVar)(updateCommand));
				}
			}
			return this.dataSource.getList((XVar)(this.queryCommand));
		}
		protected virtual XVar getVerifiedOrderRecordset(dynamic _param_rs)
		{
			#region pass-by-value parameters
			dynamic rs = XVar.Clone(_param_rs);
			#endregion

			dynamic commands = XVar.Array(), data = XVar.Array(), hasNulls = null, keyFields = XVar.Array(), order = null, orderField = null, orders = XVar.Array(), records = XVar.Array();
			orderField = XVar.Clone(this.pSet.reorderRowsField());
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			hasNulls = new XVar(false);
			commands = XVar.Clone(XVar.Array());
			records = XVar.Clone(XVar.Array());
			orders = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(rs.fetchAssoc())))
			{
				order = XVar.Clone(data[orderField]);
				if(order == XVar.Pack(""))
				{
					hasNulls = new XVar(true);
					continue;
				}
				if(XVar.Pack(orders[order]))
				{
					dynamic updateCommand = null;
					updateCommand = XVar.Clone(new DsCommand());
					foreach (KeyValuePair<XVar, dynamic> k in keyFields.GetEnumerator())
					{
						updateCommand.keys.InitAndSetArrayItem(data[k.Value], k.Value);
					}
					while(XVar.Pack(orders[order]))
					{
						++(order);
					}
					updateCommand.values.InitAndSetArrayItem(order, orderField);
					commands.InitAndSetArrayItem(updateCommand, null);
					data.InitAndSetArrayItem(order, orderField);
				}
				orders.InitAndSetArrayItem(true, order);
				records.InitAndSetArrayItem(data, null);
			}
			foreach (KeyValuePair<XVar, dynamic> dc in commands.GetEnumerator())
			{
				this.dataSource.updateSingle((XVar)(dc.Value));
			}
			return new XVar("rs", new ArrayResult((XVar)(records)), "hasNulls", hasNulls);
		}
		public virtual XVar prepareForBuildPage()
		{
			if((XVar)(this.mode == Constants.LIST_DASHDETAILS)  || (XVar)((XVar)(this.mode == Constants.LIST_DETAILS)  && (XVar)((XVar)(this.masterPageType == Constants.PAGE_LIST)  || (XVar)(this.masterPageType == Constants.PAGE_REPORT))))
			{
				this.updateDetailsTabTitles();
			}
			MVCFunctions.loadMaps((XVar)(this.pSet));
			this.buildOrderParams();
			this.deleteRecords();
			this.rulePRG();
			this.processGridTabs();
			this.setGoogleMapsParams((XVar)(this.listFields));
			RunnerContext.pushSearchContext((XVar)(this.searchClauseObj));
			this.calculateRecordCount();
			this.assignPagingVariables();
			this.recSet = XVar.Clone(this.getRecordset());
			RunnerContext.pop();
			if(XVar.Pack(!(XVar)(this.recSet)))
			{
				MVCFunctions.showError((XVar)(this.dataSource.lastError()));
			}
			this.fillGridData();
			this.fillAdvancedMapData();
			this.buildPagination();
			this.setGridUserParams();
			this.assignColumnHeaderClasses();
			if(this.mode != Constants.LIST_MASTER)
			{
				this.buildSearchPanel();
				this.assignSimpleSearch();
			}
			this.addCommonJs();
			this.addCommonHtml();
			this.commonAssign();
			this.addCustomCss();

			return null;
		}
		public virtual XVar buildOrderParams()
		{
			this.assignColumnHeaders();
			if(XVar.Pack(!(XVar)(this.isPageSortable())))
			{
				return null;
			}
			this.addOrderUrlParam();

			return null;
		}
		public virtual XVar assignColumnHeaders()
		{
			dynamic orderFields = XVar.Array();
			foreach (KeyValuePair<XVar, dynamic> f in this.listFields.GetEnumerator())
			{
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value["fName"])), "_fieldheader")), new XVar(true));
			}
			if(XVar.Pack(!(XVar)(this.isReoderByHeaderClickingEnabled())))
			{
				return null;
			}
			orderFields = this.orderClause.getOrderFields();
			foreach (KeyValuePair<XVar, dynamic> of in orderFields.GetEnumerator())
			{
				dynamic directionIcon = null;
				if(XVar.Pack(of.Value["hidden"]))
				{
					continue;
				}
				directionIcon = XVar.Clone((XVar.Pack(of.Value["dir"] == "ASC") ? XVar.Pack("glyphicon-arrow-up") : XVar.Pack("glyphicon-arrow-down")));
				this.xt.assign((XVar)(MVCFunctions.Concat("arrow_icon_", MVCFunctions.GoodFieldName((XVar)(of.Value["column"])))), (XVar)(MVCFunctions.Concat("<span class=\"glyphicon ", directionIcon, "\"></span>")));
			}
			foreach (KeyValuePair<XVar, dynamic> f in this.listFields.GetEnumerator())
			{
				dynamic attrs = XVar.Array(), dir = null, gf = null, multisort = XVar.Array();
				gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(f.Value["fName"])));
				dir = new XVar("a");
				multisort = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> of in orderFields.GetEnumerator())
				{
					if(XVar.Pack(of.Value["hidden"]))
					{
						continue;
					}
					if(of.Value["column"] == f.Value["fName"])
					{
						dir = XVar.Clone((XVar.Pack(of.Value["dir"] == "ASC") ? XVar.Pack("d") : XVar.Pack("a")));
					}
					else
					{
						multisort.InitAndSetArrayItem(MVCFunctions.Concat((XVar.Pack(of.Value["dir"] == "ASC") ? XVar.Pack("a") : XVar.Pack("d")), MVCFunctions.GoodFieldName((XVar)(of.Value["column"]))), null);
					}
				}
				attrs = XVar.Clone(XVar.Array());
				attrs.InitAndSetArrayItem(MVCFunctions.Concat("data-href=\"", MVCFunctions.GetTableLink((XVar)(this.shortTableName), new XVar("list"), (XVar)(MVCFunctions.Concat("orderby=", dir, gf))), "\""), null);
				attrs.InitAndSetArrayItem(MVCFunctions.Concat("data-order=\"", dir, gf, "\""), null);
				attrs.InitAndSetArrayItem(MVCFunctions.Concat("id=\"order_", gf, "_", this.id, "\""), null);
				attrs.InitAndSetArrayItem(MVCFunctions.Concat("name=\"order_", gf, "_", this.id, "\""), null);
				attrs.InitAndSetArrayItem(MVCFunctions.Concat("data-multisort=\"", MVCFunctions.implode(new XVar(";"), (XVar)(multisort)), "\""), null);
				attrs.InitAndSetArrayItem("class=\"rnr-orderlink\"", null);
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_orderlinkattrs")), (XVar)(MVCFunctions.implode(new XVar(" "), (XVar)(attrs))));
				if(XVar.Pack(this.fieldFilterAllowed()))
				{
					dynamic btnClass = null, fieldFilterAttrs = XVar.Array();
					this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_fieldFilter")), new XVar(true));
					fieldFilterAttrs = XVar.Clone(XVar.Array());
					fieldFilterAttrs.InitAndSetArrayItem(MVCFunctions.Concat("data-fieldfilter-state=\"", (XVar.Pack(this.fieldFilterFieldEnabled((XVar)(f.Value["fName"]))) ? XVar.Pack("enabled") : XVar.Pack("disabled")), "\""), null);
					fieldFilterAttrs.InitAndSetArrayItem("data-fieldfilter", null);
					this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_fieldfilterattrs")), (XVar)(MVCFunctions.implode(new XVar(" "), (XVar)(fieldFilterAttrs))));
					btnClass = XVar.Clone((XVar.Pack(this.fieldFilterFieldEnabled((XVar)(f.Value["fName"]))) ? XVar.Pack("btn-success") : XVar.Pack("btn-default")));
					this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_fieldfilter_buttonclass")), (XVar)(btnClass));
				}
			}

			return null;
		}
		protected virtual XVar fieldFilterFieldEnabled(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			dynamic fieldFilterFields = null, settings = XVar.Array();
			fieldFilterFields = XVar.Clone(this.getFieldFilterFields());
			if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(fieldName), (XVar)(fieldFilterFields)))))
			{
				return false;
			}
			settings = XVar.Clone(this.getFieldFilterFieldSettings((XVar)(fieldName)));
			return settings["initialized"];
		}
		protected virtual XVar orderFieldLabelString(dynamic _param_fName, dynamic _param_desc, dynamic _param_showLabelOnly)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic desc = XVar.Clone(_param_desc);
			dynamic showLabelOnly = XVar.Clone(_param_showLabelOnly);
			#endregion

			fName = XVar.Clone(CommonFunctions.GetFieldLabel((XVar)(MVCFunctions.GoodFieldName((XVar)(this.tName))), (XVar)(MVCFunctions.GoodFieldName((XVar)(fName)))));
			if(XVar.Pack(showLabelOnly))
			{
				return fName;
			}
			return MVCFunctions.Concat(fName, " ", (XVar.Pack(desc) ? XVar.Pack(CommonFunctions.mlang_message(new XVar("SORT_HIGH_TO_LOW"))) : XVar.Pack(CommonFunctions.mlang_message(new XVar("SORT_LOW_TO_HIGH")))));
		}
		protected virtual XVar assignSortByDropdown()
		{
			dynamic markup = null, options = XVar.Array(), sortByIdx = null, sortSettings = XVar.Array();
			if(XVar.Pack(!(XVar)(this.pSet.hasSortByDropdown())))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(this.recordsOnPage)))
			{
				if(XVar.Pack(this.listAjax))
				{
					this.xt.assign(new XVar("reorder_records"), new XVar(true));
					this.hideElement(new XVar("reorder_records"));
				}
				else
				{
					this.hideItemType(new XVar("sort_dropdown"));
				}
				return null;
			}
			sortSettings = XVar.Clone(this.orderClause.getSortBySettings());
			sortByIdx = XVar.Clone(this.orderClause.getSortByControlIdx());
			options = XVar.Clone(XVar.Array());
			if(sortByIdx == -1)
			{
				options.InitAndSetArrayItem("<option selected> </option>", null);
			}
			foreach (KeyValuePair<XVar, dynamic> sData in sortSettings.GetEnumerator())
			{
				dynamic label = null, selected = null;
				label = XVar.Clone(Labels.multilangString((XVar)(sData.Value["label"])));
				if(XVar.Pack(!(XVar)(label)))
				{
					dynamic labelParts = XVar.Array();
					labelParts = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> fData in sData.Value["fields"].GetEnumerator())
					{
						labelParts.InitAndSetArrayItem(this.orderFieldLabelString((XVar)(fData.Value["field"]), (XVar)(fData.Value["desc"]), (XVar)(fData.Value["labelOnly"])), null);
					}
					label = XVar.Clone(MVCFunctions.implode(new XVar("; "), (XVar)(labelParts)));
				}
				selected = XVar.Clone((XVar.Pack(sortByIdx == sData.Key) ? XVar.Pack(" selected") : XVar.Pack("")));
				options.InitAndSetArrayItem(MVCFunctions.Concat("<option value=\"", sData.Key + 1, "\" ", selected, ">", label, "</option>"), null);
			}
			if(XVar.Pack(MVCFunctions.count(options)))
			{
				markup = XVar.Clone(MVCFunctions.Concat("<select id=\"sortBy", this.id, "\" class=\"form-control\">", MVCFunctions.implode(new XVar(""), (XVar)(options)), "</select>"));
			}
			this.xt.assign(new XVar("reorder_records"), new XVar(true));
			this.xt.assign(new XVar("sortByDropdown"), (XVar)(markup));

			return null;
		}
		public virtual XVar showPage()
		{
			this.BeforeShowList();
			this.display((XVar)(this.templatefile));

			return null;
		}
		public static XVar createListPage(dynamic _param_strTableName, dynamic _param_options)
		{
			#region pass-by-value parameters
			dynamic strTableName = XVar.Clone(_param_strTableName);
			dynamic options = XVar.Clone(_param_options);
			#endregion

			dynamic gSettings = null, pageObject = null, var_params = XVar.Array();
			SQLQuery gQuery;
			gSettings = XVar.Clone(new ProjectSettings((XVar)(strTableName), (XVar)(options["pageType"]), (XVar)(options["pageName"])));
			gQuery = XVar.UnPackSQLQuery(gSettings.getSQLQuery());
			var_params = XVar.Clone(options);
			var_params.InitAndSetArrayItem(strTableName, "tName");
			var_params.InitAndSetArrayItem(gSettings.getOriginalTableName(), "origTName");
			var_params.InitAndSetArrayItem(gSettings.getInitialPageSize(), "gPageSize");
			var_params.InitAndSetArrayItem(gSettings.getAdvancedSecurityType(), "nSecOptions");
			var_params.InitAndSetArrayItem(gSettings.getRecordsPerRowList(), "recsPerRowList");
			var_params.InitAndSetArrayItem(gSettings.getTableOwnerIdField(), "mainTableOwnerID");
			var_params.InitAndSetArrayItem(gSettings.hasExportPage(), "exportTo");
			var_params.InitAndSetArrayItem(gSettings.hasPrintPage(), "printFriendly");
			var_params.InitAndSetArrayItem(gSettings.hasDelete(), "deleteRecs");
			var_params.InitAndSetArrayItem(gSettings.getTableKeys(), "arrKeyFields");
			var_params.InitAndSetArrayItem(gSettings.getPanelSearchFields(), "panelSearchFields");
			var_params.InitAndSetArrayItem(gSettings.getListGridLayout(), "listGridLayout");
			var_params.InitAndSetArrayItem(gSettings.noRecordsOnFirstPage(), "noRecordsFirstPage");
			var_params.InitAndSetArrayItem(gSettings.getTotalsFields(), "totalsFields");
			var_params.InitAndSetArrayItem(gSettings.ajaxBasedListPage(), "listAjax");
			var_params.InitAndSetArrayItem(gSettings.getRecordsPerPageArray(), "arrRecsPerPage");
			var_params.InitAndSetArrayItem(gSettings.getScrollGridBody(), "isScrollGridBody");
			var_params.InitAndSetArrayItem(CommonFunctions.GetAuditObject((XVar)(strTableName)), "audit");
			if(var_params["mode"] == Constants.LIST_SIMPLE)
			{
				pageObject = XVar.Clone(new ListPage_Simple((XVar)(var_params)));
			}
			else
			{
				if(var_params["mode"] == Constants.LIST_MASTER)
				{
					pageObject = XVar.Clone(new ListPage_Master((XVar)(var_params)));
				}
				else
				{
					if(var_params["mode"] == Constants.LIST_AJAX)
					{
						pageObject = XVar.Clone(new ListPage_Ajax((XVar)(var_params)));
					}
					else
					{
						if(var_params["mode"] == Constants.LIST_LOOKUP)
						{
							pageObject = XVar.Clone(new ListPage_Lookup((XVar)(var_params)));
						}
						else
						{
							if((XVar)(var_params["mode"] == Constants.LIST_DETAILS)  && (XVar)(var_params["masterPageType"] == Constants.PAGE_LIST))
							{
								pageObject = XVar.Clone(new ListPage_DPList((XVar)(var_params)));
							}
							else
							{
								if((XVar)(var_params["mode"] == Constants.LIST_DETAILS)  || (XVar)(var_params["mode"] == Constants.LIST_PDFJSON))
								{
									pageObject = XVar.Clone(new ListPage_DPInline((XVar)(var_params)));
								}
								else
								{
									if(var_params["mode"] == Constants.LIST_POPUPDETAILS)
									{
										pageObject = XVar.Clone(new ListPage_DPPopup((XVar)(var_params)));
									}
									else
									{
										if(var_params["mode"] == Constants.LIST_DASHDETAILS)
										{
											pageObject = XVar.Clone(new ListPage_DPDash((XVar)(var_params)));
										}
										else
										{
											if(var_params["mode"] == Constants.RIGHTS_PAGE)
											{
												pageObject = XVar.Clone(new RightsPage((XVar)(var_params)));
											}
											else
											{
												if(var_params["mode"] == Constants.MEMBERS_PAGE)
												{
													if(var_params["providerType"] == Constants.stAD)
													{
														pageObject = XVar.Clone(new MembersPage_AD((XVar)(var_params)));
													}
													else
													{
														pageObject = XVar.Clone(new MembersPage((XVar)(var_params)));
													}
												}
												else
												{
													if(var_params["mode"] == Constants.LIST_DASHBOARD)
													{
														pageObject = XVar.Clone(new ListPage_Dashboard((XVar)(var_params)));
													}
													else
													{
														if(var_params["mode"] == Constants.MAP_DASHBOARD)
														{
															pageObject = XVar.Clone(new MapPage_Dashboard((XVar)(var_params)));
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			pageObject.init();
			return pageObject;
		}
		protected virtual XVar setRowsGridRecord(dynamic row, dynamic _param_record)
		{
			#region pass-by-value parameters
			dynamic record = XVar.Clone(_param_record);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> value in record.GetEnumerator())
			{
				row.InitAndSetArrayItem(value.Value, value.Key);
			}
			row.InitAndSetArrayItem(true, "grid_record");

			return null;
		}
		public virtual XVar getNotListBlobFieldsIndices()
		{
			dynamic allFields = XVar.Array(), blobIndices = XVar.Array(), indices = XVar.Array();
			allFields = XVar.Clone(this.pSet.getFieldsList());
			blobIndices = XVar.Clone(this.pSet.getBinaryFieldsIndices());
			indices = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> idx in blobIndices.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.pSet.appearOnListPage((XVar)(allFields[idx.Value - 1])))))
				{
					indices.InitAndSetArrayItem(idx.Value, null);
				}
			}
			return indices;
		}
		public static XVar readListModeFromRequest()
		{
			dynamic postedMode = null;
			postedMode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(postedMode == "ajax")
			{
				return Constants.LIST_AJAX;
			}
			else
			{
				if(postedMode == "lookup")
				{
					return Constants.LIST_LOOKUP;
				}
				else
				{
					if(postedMode == "listdetails")
					{
						return Constants.LIST_DETAILS;
					}
					else
					{
						if(postedMode == "listdetailspopup")
						{
							return Constants.LIST_POPUPDETAILS;
						}
						else
						{
							if(postedMode == "dashdetails")
							{
								return Constants.LIST_DASHDETAILS;
							}
							else
							{
								if(postedMode == "dashlist")
								{
									return Constants.LIST_DASHBOARD;
								}
								else
								{
									if(postedMode == "dashmap")
									{
										return Constants.MAP_DASHBOARD;
									}
								}
							}
						}
					}
				}
			}
			return Constants.LIST_SIMPLE;
		}
		protected static XVar readMainTableSettingsFromRequest(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic mainTableShortName = null;
			ProjectSettings pSet;
			if(XVar.Pack(Security.registerPage()))
			{
				if((XVar)(MVCFunctions.postvalue(new XVar("pageType")) == "register")  && (XVar)(MVCFunctions.postvalue(new XVar("table")) == Security.loginTable()))
				{
					return new ProjectSettings((XVar)(Security.loginTable()), new XVar(Constants.PAGE_REGISTER), new XVar(""), new XVar(Constants.GLOBAL_PAGES));
				}
			}
			mainTableShortName = XVar.Clone(CommonFunctions.GetTableURL((XVar)(MVCFunctions.postvalue(new XVar("table")))));
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table), new XVar("list")));
			return pSet.getLookupMainTableSettings((XVar)(mainTableShortName), (XVar)(MVCFunctions.postvalue(new XVar("field"))));
		}
		protected static XVar checkLookupPermissions(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic lookupMainSettings = null, mainTable = null;
			lookupMainSettings = XVar.Clone(ListPage.readMainTableSettingsFromRequest((XVar)(table)));
			if(XVar.Pack(!(XVar)(lookupMainSettings)))
			{
				return false;
			}
			mainTable = XVar.Clone(lookupMainSettings.table());
			if((XVar)((XVar)(CommonFunctions.CheckTablePermissions((XVar)(mainTable), new XVar("S")))  || (XVar)(CommonFunctions.CheckTablePermissions((XVar)(mainTable), new XVar("E"))))  || (XVar)(CommonFunctions.CheckTablePermissions((XVar)(mainTable), new XVar("A"))))
			{
				return true;
			}
			if(XVar.Pack(Security.registerPage()))
			{
				if(mainTable == Security.loginTable())
				{
					if(!XVar.Equals(XVar.Pack(lookupMainSettings.appearOnPage((XVar)(MVCFunctions.postvalue(new XVar("field"))))), XVar.Pack(false)))
					{
						return true;
					}
				}
			}
			return false;
		}
		public static XVar processListPageSecurity(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic mode = null;
			if(XVar.Pack(Security.checkPagePermissions((XVar)(table), new XVar("S"))))
			{
				return true;
			}
			mode = XVar.Clone(ListPage.readListModeFromRequest());
			if((XVar)(mode == Constants.LIST_LOOKUP)  && (XVar)(ListPage.checkLookupPermissions((XVar)(table))))
			{
				return true;
			}
			if(mode != Constants.LIST_SIMPLE)
			{
				Security.sendPermissionError();
				return false;
			}
			if((XVar)(CommonFunctions.isLogged())  && (XVar)(!(XVar)(Security.isGuest())))
			{
				MVCFunctions.HeaderRedirect(new XVar("menu"));
				return false;
			}
			CommonFunctions.redirectToLogin();
			return false;
		}
		public virtual XVar isPageSortable()
		{
			return true;
		}
		public static XVar processSaveParams(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			if(XVar.Pack(MVCFunctions.postvalue(new XVar("saveParam"))))
			{
				dynamic paramData = null, paramType = null, paramsLogger = null;
				paramType = XVar.Clone(MVCFunctions.intval((XVar)(MVCFunctions.postvalue(new XVar("saveParam")))));
				paramData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("data")))));
				if(XVar.Pack(MVCFunctions.postvalue(new XVar("onDashboard"))))
				{
					paramsLogger = XVar.Clone(new paramsLogger((XVar)(MVCFunctions.postvalue(new XVar("dashElementId"))), (XVar)(paramType)));
				}
				else
				{
					paramsLogger = XVar.Clone(new paramsLogger((XVar)(table), (XVar)(paramType)));
				}
				if(paramType == Constants.NEWSHFIELDS_PARAMS_TYPE)
				{
					dynamic macroDeviceClass = null, ps = null;
					macroDeviceClass = XVar.Clone(RunnerPage.deviceClassToMacro((XVar)(MVCFunctions.postvalue(new XVar("deviceClass")))));
					ps = XVar.Clone(new ProjectSettings((XVar)(table)));
					if(XVar.Pack(!(XVar)(ps.columnsByDeviceEnabled())))
					{
						macroDeviceClass = new XVar(0);
					}
					paramsLogger.saveShowHideData((XVar)(macroDeviceClass), (XVar)(paramData));
				}
				else
				{
					paramsLogger.save((XVar)(paramData));
				}
				return true;
			}
			return false;
		}
		protected virtual XVar setGridUserParams()
		{

			return null;
		}
		protected virtual XVar displayViewLink()
		{
			return this.viewAvailable();
		}
		public override XVar gridTabsAvailable()
		{
			if((XVar)(this.mode == Constants.LIST_DETAILS)  && (XVar)(this.masterPageType == Constants.PAGE_ADD))
			{
				return false;
			}
			return true;
		}
		protected virtual XVar hasMainDashMapElem()
		{
			return false;
		}
		public override XVar displayTabsInPage()
		{
			return (XVar)(this.simpleMode())  || (XVar)((XVar)(this.mode == Constants.LIST_DETAILS)  && (XVar)((XVar)(this.masterPageType == Constants.PAGE_VIEW)  || (XVar)(this.masterPageType == Constants.PAGE_EDIT)));
		}
		public override XVar element2Item(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(name == "message")
			{
				return new XVar(0, "grid_message");
			}
			if(name == "grid")
			{
				return new XVar(0, "grid");
			}
			return base.element2Item((XVar)(name));
		}
		public override XVar findRecordAssigns(dynamic _param_recordId)
		{
			#region pass-by-value parameters
			dynamic recordId = XVar.Clone(_param_recordId);
			#endregion

			return this.recordsRenderData[recordId];
		}
		public virtual XVar printRawValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(!(XVar)(this.fieldsWithRawValues.KeyExists(field))))
			{
				dynamic var_type = null;
				var_type = XVar.Clone(this.pSet.getFieldType((XVar)(field)));
				if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
				{
					this.fieldsWithRawValues.InitAndSetArrayItem(false, field);
				}
				else
				{
					this.fieldsWithRawValues.InitAndSetArrayItem((XVar)((XVar)((XVar)(this.addRawFieldValues)  || (XVar)(this.arrFieldSpanVal[field] == 2))  || (XVar)(this.arrFieldSpanVal[field] == 1))  || (XVar)(this.pSet.hasJsEvents()), field);
				}
			}
			return this.fieldsWithRawValues[field];
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
			if((XVar)((XVar)(this.mode != Constants.LIST_DETAILS)  && (XVar)(this.noRecordsFirstPage))  && (XVar)(!(XVar)(this.isSearchFunctionalityActivated())))
			{
				dc.filter = XVar.Clone(DataCondition._False());
			}
			dc.reccount = XVar.Clone(this.pageSize);
			if(0 <= this.pageSize)
			{
				dc.startRecord = XVar.Clone(this.pageSize * (this.myPage - 1));
			}
			else
			{
				dc.startRecord = new XVar(0);
			}
			if(XVar.Pack(this.pSet.getRecordsLimit()))
			{
				dc.reccount = XVar.Clone(this.pSet.getRecordsLimit());
			}
			this.reoderCommandForReoderedRows((XVar)(this.pSet), (XVar)(dc));
			return dc;
		}
		public virtual XVar calculateRecordCount()
		{
			dynamic prep = XVar.Array();
			this.queryCommand = XVar.Clone(this.getSubsetDataCommand());
			prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(this.queryCommand)));
			this.querySQL = XVar.Clone(prep["sql"]);
			this.callBeforeQueryEvent((XVar)(this.queryCommand));
			if(XVar.Pack(!(XVar)(this.pSet.hideNumberOfRecords())))
			{
				dynamic recordLimit = null;
				this.numRowsFromSQL = XVar.Clone(this.dataSource.getCount((XVar)(this.queryCommand)));
				recordLimit = XVar.Clone(this.pSet.getRecordsLimit());
				if((XVar)(recordLimit)  && (XVar)(recordLimit < this.numRowsFromSQL))
				{
					this.numRowsFromSQL = XVar.Clone(recordLimit);
				}
			}

			return null;
		}
		public override XVar callBeforeQueryEvent(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic order = null, prep = XVar.Array(), sql = null, where = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("BeforeQueryList")))))
			{
				return null;
			}
			prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
			where = XVar.Clone(prep["where"]);
			order = XVar.Clone(prep["order"]);
			sql = XVar.Clone(prep["sql"]);
			this.eventsObject.BeforeQueryList((XVar)(sql), ref where, ref order, this);
			if(sql != prep["sql"])
			{
				this.dataSource.overrideSQL((XVar)(dc), (XVar)(sql));
			}
			else
			{
				if(where != prep["where"])
				{
					this.dataSource.overrideWhere((XVar)(dc), (XVar)(where));
				}
				if(order != prep["order"])
				{
					this.dataSource.overrideOrder((XVar)(dc), (XVar)(order));
				}
			}
			prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
			this.querySQL = XVar.Clone(prep["sql"]);

			return null;
		}
		protected virtual XVar getDeleteCommand(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(Security.SelectCondition(new XVar("D"), (XVar)(this.pSet)));
			if((XVar)(this.tName == Constants.ADMIN_USERS)  && (XVar)(Security.dynamicPermissions()))
			{
				dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, DataCondition._Not((XVar)(Security.currentUserCondition()))))));
			}
			dc.keys = XVar.Clone(keys);
			return dc;
		}
		protected virtual XVar spreadsheetGridApplicable()
		{
			return (XVar)((XVar)((XVar)((XVar)((XVar)(this.mode == Constants.LIST_SIMPLE)  || (XVar)(this.mode == Constants.LIST_AJAX))  || (XVar)(this.mode == Constants.LIST_DASHBOARD))  || (XVar)(this.mode == Constants.LIST_DETAILS))  && (XVar)(this.pSet.spreadsheetGrid()))  && (XVar)(this.permis[this.tName]["edit"]);
		}
		protected virtual XVar setupRelatedInlineEditPage()
		{
			this.editPage = XVar.Clone(this.getRelatedInlineEditPage((XVar)(this.pageName)));

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

			dynamic i = null, settings = XVar.Array();
			settings = XVar.Clone(base.buildJsTableSettings((XVar)(table), (XVar)(pSet)));
			if((XVar)(this.spreadsheetGridApplicable())  && (XVar)(this.editPage))
			{
				foreach (KeyValuePair<XVar, dynamic> field in this.pSet.getInlineEditFields().GetEnumerator())
				{
					settings.InitAndSetArrayItem(this.buildJsFieldSettings((XVar)(this.editPage.pSet), (XVar)(field.Value), new XVar(Constants.PAGE_EDIT)), "fieldSettings", field.Value, Constants.PAGE_EDIT);
				}
			}
			settings.InitAndSetArrayItem(this.pSet.getPageOption(new XVar("list"), new XVar("inlineEdit")), "isInlineEdit");
			settings.InitAndSetArrayItem(this.pSet.getPageOption(new XVar("list"), new XVar("inlineAdd")), "isInlineAdd");
			settings.InitAndSetArrayItem(!(XVar)(!(XVar)(this.pSet.getDefaultPage(new XVar(Constants.PAGE_VIEW)))), "view");
			settings.InitAndSetArrayItem(this.pSet.getPageFields(), "listFields");
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.listFields); i++)
			{
				if(this.listFields[i]["viewFormat"] == Constants.FORMAT_MAP)
				{
					this.gMapFields.InitAndSetArrayItem(i, null);
				}
			}
			if(XVar.Pack(this.searchClauseObj))
			{
				settings.InitAndSetArrayItem(this.searchClauseObj.simpleSearchActive, "simpleSearchActive");
			}
			if((XVar)(this.pSet.reorderRows())  && (XVar)(Security.userCan(new XVar("E"), (XVar)(this.tName))))
			{
				settings.InitAndSetArrayItem(true, "reorderRows");
			}
			settings.InitAndSetArrayItem(this.pSet.inlineAddBottom(), "addToBottom");
			settings.InitAndSetArrayItem(XVar.Array(), "totalFields");
			foreach (KeyValuePair<XVar, dynamic> tField in this.totalsFields.GetEnumerator())
			{
				dynamic totalFieldData = XVar.Array();
				totalFieldData = XVar.Clone(new XVar("type", tField.Value["totalsType"], "fName", tField.Value["fName"], "format", tField.Value["viewFormat"]));
				totalFieldData.InitAndSetArrayItem(CommonFunctions.getFormatSettings((XVar)(tField.Value["viewFormat"]), (XVar)(this.pSet), (XVar)(tField.Value["fName"])), "formatSettings");
				settings.InitAndSetArrayItem(totalFieldData, "totalFields", null);
			}
			settings.InitAndSetArrayItem((XVar)((XVar)(this.listGridLayout == Constants.gltHORIZONTAL)  || (XVar)(this.listGridLayout == Constants.gltFLEXIBLE))  && (XVar)(this.isScrollGridBody), "scrollGridBody");
			settings.InitAndSetArrayItem(this.permis[this.tName], "permissions");
			if(this.pSet.getAdvancedSecurityType() == Constants.ADVSECURITY_EDIT_OWN)
			{
				settings.InitAndSetArrayItem(this.permis[this.tName], "isEditOwn");
			}
			settings.InitAndSetArrayItem(this.buildClickActionSettings(), "clickActions");
			settings.InitAndSetArrayItem(this.listGridLayout, "listGridLayout");
			if(XVar.Pack(this.spreadsheetGridApplicable()))
			{
				settings.InitAndSetArrayItem(this.pSet.addNewRecordAutomatically(), "autoAddNewRecord");
			}
			settings.InitAndSetArrayItem((XVar.Pack(this.numRowsFromSQL) ? XVar.Pack(true) : XVar.Pack(false)), "showRows");
			if(XVar.Pack(this.lockingObj))
			{
				settings.InitAndSetArrayItem(this.lockRecIds, "lockRecIds");
			}
			return settings;
		}
		public virtual XVar updateRowOrder()
		{
			dynamic action = null, allRecordKeys = XVar.Array(), conditions = XVar.Array(), data = XVar.Array(), forward = null, idKeys = XVar.Array(), keyFields = XVar.Array(), keyValues = XVar.Array(), keys = XVar.Array(), newOrder = null, oldOrder = null, orderField = null, orders = XVar.Array(), postData = null, recId = null, recordId = null, rs = null, selectDc = null, updateData = XVar.Array(), updateOrderDc = null;
			action = XVar.Clone(MVCFunctions.postvalue(new XVar("a")));
			postData = XVar.Clone(MVCFunctions.postvalue(new XVar("data")));
			if((XVar)(!(XVar)(this.pSet.reorderRows()))  || (XVar)(!(XVar)(Security.userCan(new XVar("E"), (XVar)(this.tName)))))
			{
				return false;
			}
			if(!XVar.Equals(XVar.Pack(action), XVar.Pack("saveRowOrder")))
			{
				return false;
			}
			updateData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(postData)));
			newOrder = XVar.Clone(updateData["newOrder"]);
			oldOrder = XVar.Clone(updateData["oldOrder"]);
			keys = XVar.Clone(updateData["keys"]);
			allRecordKeys = XVar.Clone(updateData["allRecordKeys"]);
			recordId = XVar.Clone(updateData["recordId"]);
			if((XVar)(XVar.Equals(XVar.Pack(newOrder), XVar.Pack(null)))  || (XVar)(XVar.Equals(XVar.Pack(oldOrder), XVar.Pack(null))))
			{
				MVCFunctions.Echo("wrong parameters");
				return true;
			}
			orderField = XVar.Clone(this.pSet.reorderRowsField());
			updateOrderDc = XVar.Clone(new DsCommand());
			updateOrderDc.advValues = XVar.Clone(XVar.Array());
			forward = XVar.Clone(oldOrder < newOrder);
			if(XVar.Pack(forward))
			{
				updateOrderDc.advValues.InitAndSetArrayItem(new DsOperand(new XVar(Constants.dsotSQL), (XVar)(MVCFunctions.Concat(this.dataSource.wrap((XVar)(orderField)), " - 1 "))), orderField);
				updateOrderDc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, DataCondition.FieldIs((XVar)(orderField), new XVar(Constants.dsopMORE), (XVar)(oldOrder)), 1, DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(orderField), new XVar(Constants.dsopMORE), (XVar)(newOrder))))))));
			}
			else
			{
				updateOrderDc.advValues.InitAndSetArrayItem(new DsOperand(new XVar(Constants.dsotSQL), (XVar)(MVCFunctions.Concat(this.dataSource.wrap((XVar)(orderField)), " + 1 "))), orderField);
				updateOrderDc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, DataCondition.FieldIs((XVar)(orderField), new XVar(Constants.dsopLESS), (XVar)(oldOrder)), 1, DataCondition._Not((XVar)(DataCondition.FieldIs((XVar)(orderField), new XVar(Constants.dsopLESS), (XVar)(newOrder))))))));
			}
			this.dataSource.updateSingle((XVar)(updateOrderDc), new XVar(false));
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			if(XVar.Pack(keys))
			{
				dynamic updateDc = null;
				updateDc = XVar.Clone(new DsCommand());
				foreach (KeyValuePair<XVar, dynamic> k in keyFields.GetEnumerator())
				{
					updateDc.keys.InitAndSetArrayItem(keys[k.Key], k.Value);
				}
				updateDc.values.InitAndSetArrayItem(newOrder, orderField);
				this.dataSource.updateSingle((XVar)(updateDc));
			}
			idKeys = XVar.Clone(XVar.Array());
			conditions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> rKeys in allRecordKeys.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(rKeys.Value["keys"])))
				{
					continue;
				}
				keyValues = XVar.Clone(CommonFunctions.numericToAssoc((XVar)(keyFields), (XVar)(rKeys.Value["keys"])));
				conditions.InitAndSetArrayItem(DataCondition.FieldsEqual((XVar)(keyFields), (XVar)(keyValues)), null);
				idKeys.InitAndSetArrayItem(keyValues, rKeys.Value["recordId"]);
			}
			selectDc = XVar.Clone(new DsCommand());
			selectDc.filter = XVar.Clone(DataCondition._Or((XVar)(conditions)));
			selectDc.order = XVar.Clone(new XVar(0, new XVar("column", orderField, "dir", "ASC")));
			rs = XVar.Clone(this.dataSource.getList((XVar)(selectDc)));
			orders = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(rs.fetchAssoc())))
			{
				keyValues = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> k in keyFields.GetEnumerator())
				{
					keyValues.InitAndSetArrayItem(data[k.Value], k.Value);
				}
				recId = XVar.Clone(CommonFunctions.findArrayInArray((XVar)(idKeys), (XVar)(keyValues)));
				if(XVar.Equals(XVar.Pack(recId), XVar.Pack(false)))
				{
					continue;
				}
				orders.InitAndSetArrayItem(data[orderField], recId);
			}
			orders.InitAndSetArrayItem(newOrder, recordId);
			MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(new XVar("orders", orders))));
			return true;
		}
		public override XVar getEditFormat(dynamic _param_field, dynamic _param_pSet_packed = null)
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

			if(XVar.Pack(this.spreadsheetGridApplicable()))
			{
				if(XVar.Pack(this.detailsKeyField((XVar)(field))))
				{
					return Constants.EDIT_FORMAT_READONLY;
				}
			}
			return base.getEditFormat((XVar)(field), (XVar)(pSet));
		}
		protected virtual XVar fieldFilterAllowed()
		{
			if((XVar)(this.mode == Constants.LIST_DETAILS)  && (XVar)(this.masterPageType == Constants.PAGE_ADD))
			{
				return false;
			}
			return true;
		}
		protected override XVar fieldFilterEnabled()
		{
			if(XVar.Pack(!(XVar)(this.fieldFilterAllowed())))
			{
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> f in this.listFields.GetEnumerator())
			{
				if(XVar.Pack(this.fieldFilterFieldEnabled((XVar)(f.Value["fName"]))))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar processTotals()
		{
			if(MVCFunctions.postvalue(new XVar("a")) == "calctotals")
			{
				dynamic dbTotals = XVar.Array(), totals = XVar.Array();
				dbTotals = XVar.Clone(this.getTotalsFromDB());
				totals = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> tf in this.totalsFields.GetEnumerator())
				{
					totals.InitAndSetArrayItem(CommonFunctions.GetTotals((XVar)(tf.Value["fName"]), (XVar)(dbTotals[tf.Value["fName"]]), (XVar)(tf.Value["totalsType"]), new XVar(1), (XVar)(tf.Value["viewFormat"]), new XVar(Constants.PAGE_LIST), (XVar)(this.pSet), new XVar(false), this), tf.Value["fName"]);
				}
				MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(totals)));
				return true;
			}
			return false;
		}
	}
}
