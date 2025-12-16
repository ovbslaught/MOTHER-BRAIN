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
	public partial class EditPage : RunnerPage
	{
		protected dynamic cachedRecord = XVar.Pack(null);
		public dynamic oldKeys = XVar.Array();
		public dynamic newKeys = XVar.Array();
		protected dynamic keysChanged = XVar.Pack(false);
		public dynamic jsKeys = XVar.Array();
		public dynamic keyFields = XVar.Array();
		public dynamic readEditValues = XVar.Pack(false);
		public dynamic action = XVar.Pack("");
		public dynamic lockingAction = XVar.Pack("");
		public dynamic lockingSid = XVar.Pack(null);
		public dynamic lockingKeys = XVar.Pack(null);
		public dynamic lockingStart = XVar.Pack(null);
		protected dynamic lockingMessageAttr = XVar.Pack("data-locked");
		protected dynamic lockingMessageText = XVar.Pack("");
		protected dynamic lockingMessageBlock = XVar.Pack("");
		public dynamic messageType = XVar.Pack(Constants.MESSAGE_ERROR);
		protected dynamic auditObj = XVar.Pack(null);
		protected dynamic oldRecordData = XVar.Pack(null);
		protected dynamic newRecordData = XVar.Array();
		protected dynamic updatedSuccessfully = XVar.Pack(false);
		public dynamic screenWidth = XVar.Pack(0);
		public dynamic screenHeight = XVar.Pack(0);
		public dynamic orientation = XVar.Pack("");
		protected dynamic afterEditAction = XVar.Pack(null);
		protected dynamic prevKeys = XVar.Pack(null);
		protected dynamic nextKeys = XVar.Pack(null);
		protected dynamic recordValuesToEdit = XVar.Pack(null);
		public dynamic forSpreadsheetGrid = XVar.Pack(false);
		public dynamic hostPageName = XVar.Pack("");
		protected dynamic sqlValues = XVar.Array();
		public dynamic listPage = XVar.Pack("");
		public dynamic lookupTable = XVar.Pack("");
		public dynamic lookupField = XVar.Pack("");
		public dynamic lookupPageType = XVar.Pack("");
		public dynamic parentCtrlsData;
		protected static bool skipEditPageCtor = false;
		public EditPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipEditPageCtor)
			{
				skipEditPageCtor = false;
				return;
			}
			this.setKeysForJs();
			this.auditObj = XVar.Clone(CommonFunctions.GetAuditObject((XVar)(this.tName)));
			this.editFields = XVar.Clone(this.getPageFields());
			this.headerForms = XVar.Clone(new XVar(0, "top"));
			this.footerForms = XVar.Clone(new XVar(0, "below-grid"));
			if(XVar.Pack(this.isMultistepped()))
			{
				this.bodyForms = XVar.Clone(new XVar(0, "above-grid", 1, "steps"));
			}
			else
			{
				this.bodyForms = XVar.Clone(new XVar(0, "above-grid", 1, "grid"));
			}
			this.addPageSettings();
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
			this.buildAfterEditActionSettings((XVar)(settings));
			if((XVar)(true)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.prevKeys), XVar.Pack(null)))))
			{
				settings.InitAndSetArrayItem(this.prevKeys, "prevKeys");
			}
			if((XVar)(true)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.nextKeys), XVar.Pack(null)))))
			{
				settings.InitAndSetArrayItem(this.nextKeys, "nextKeys");
			}
			settings.InitAndSetArrayItem(this.jsKeys, "keys");
			settings.InitAndSetArrayItem(this.pSet.getTableKeys(), "keyFields");
			if(XVar.Pack(this.lockingObj))
			{
				dynamic escapedKeys = XVar.Array();
				escapedKeys = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> k in this.keys.GetEnumerator())
				{
					escapedKeys.InitAndSetArrayItem(MVCFunctions.RawUrlEncode((XVar)(k.Value)), null);
				}
				settings.InitAndSetArrayItem(MVCFunctions.implode(new XVar("&"), (XVar)(escapedKeys)), "sKeys");
				settings.InitAndSetArrayItem(this.lockingObj.ConfirmTime, "confirmTime");
			}
			return settings;
		}
		protected virtual XVar buildAfterEditActionSettings(dynamic settings)
		{
			dynamic afterEditAction = null;
			if((XVar)(!(XVar)(this.isPopupMode()))  && (XVar)(!(XVar)(this.isSimpleMode())))
			{
				return null;
			}
			afterEditAction = XVar.Clone(this.getAfterEditAction());
			settings.InitAndSetArrayItem(afterEditAction, "afterEditAction");
			if(afterEditAction == Constants.AE_TO_DETAIL_LIST)
			{
				settings.InitAndSetArrayItem(CommonFunctions.GetTableURL((XVar)(this.pSet.getAEDetailTable())), "afterEditActionDetTable");
			}
			if(this.mode == Constants.EDIT_POPUP)
			{
				if(afterEditAction == Constants.AE_TO_NEXT_EDIT)
				{
					settings.InitAndSetArrayItem(this.getNextKeys(), "nextKeys");
				}
				if(afterEditAction == Constants.AE_TO_PREV_EDIT)
				{
					settings.InitAndSetArrayItem(this.getPrevKeys(), "prevKeys");
				}
			}

			return null;
		}
		protected virtual XVar addPageSettings()
		{

			return null;
		}
		protected virtual XVar getAfterEditAction()
		{
			dynamic action = null;
			if((XVar)(true)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.afterEditAction), XVar.Pack(null)))))
			{
				return this.afterEditAction;
			}
			action = XVar.Clone(this.pSet.getAfterEditAction());
			if((XVar)((XVar)((XVar)((XVar)(this.isPopupMode())  && (XVar)(this.pSet.checkClosePopupAfterEdit()))  || (XVar)((XVar)(action == Constants.AE_TO_VIEW)  && (XVar)(!(XVar)(this.viewAvailable()))))  || (XVar)((XVar)(action == Constants.AE_TO_NEXT_EDIT)  && (XVar)(!(XVar)(this.getNextKeys()))))  || (XVar)((XVar)(action == Constants.AE_TO_PREV_EDIT)  && (XVar)(!(XVar)(this.getPrevKeys()))))
			{
				action = new XVar(Constants.AE_TO_LIST);
			}
			if(action == Constants.AE_TO_DETAIL_LIST)
			{
				dynamic dPermissions = XVar.Array(), dPset = null, dTName = null;
				dTName = XVar.Clone(this.pSet.getAEDetailTable());
				dPset = XVar.Clone(new ProjectSettings((XVar)(dTName)));
				dPermissions = XVar.Clone(this.getPermissions((XVar)(dTName)));
				if((XVar)(!(XVar)(dTName))  || (XVar)((XVar)(action == Constants.AE_TO_DETAIL_LIST)  && (XVar)((XVar)(!(XVar)(dPset.hasListPage()))  || (XVar)(!(XVar)(dPermissions["search"])))))
				{
					action = new XVar(Constants.AE_TO_LIST);
				}
			}
			this.afterEditAction = XVar.Clone(action);
			return this.afterEditAction;
		}
		protected override XVar assignSessionPrefix()
		{
			if((XVar)(this.mode == Constants.EDIT_DASHBOARD)  || (XVar)((XVar)((XVar)(this.isPopupMode())  || (XVar)(this.mode == Constants.EDIT_INLINE))  && (XVar)(this.dashTName)))
			{
				this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.dashTName, "_", this.tName));
				return null;
			}
			base.assignSessionPrefix();

			return null;
		}
		public override XVar setSessionVariables()
		{
			dynamic masterTable = null;
			masterTable = XVar.Clone(this.masterTable);
			base.setSessionVariables();
			this.masterTable = XVar.Clone(masterTable);
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_advsearch")] = MVCFunctions.serialize((XVar)(this.searchClauseObj));

			return null;
		}
		protected override XVar getPageFields()
		{
			if(this.mode == Constants.EDIT_INLINE)
			{
				return this.pSet.getInlineEditFields();
			}
			return this.pSet.getEditFields();
		}
		public virtual XVar setKeys(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			this.cachedRecord = new XVar(null);
			this.recordValuesToEdit = new XVar(null);
			this.keys = XVar.Clone(keys);
			this.setKeysForJs();

			return null;
		}
		public virtual XVar setKeysForJs()
		{
			dynamic i = null;
			i = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> value in this.keys.GetEnumerator())
			{
				this.jsKeys.InitAndSetArrayItem(value.Value, i++);
			}

			return null;
		}
		public virtual XVar isLockingRequest()
		{
			return (XVar)(this.lockingObj)  && (XVar)(this.lockingAction != "");
		}
		public virtual XVar doLockingAction()
		{
			dynamic arrkeys = XVar.Array();
			arrkeys = XVar.Clone(MVCFunctions.explode(new XVar("&"), (XVar)(MVCFunctions.urldecode((XVar)(this.lockingKeys)))));
			foreach (KeyValuePair<XVar, dynamic> ind in MVCFunctions.array_keys((XVar)(arrkeys)).GetEnumerator())
			{
				arrkeys.InitAndSetArrayItem(MVCFunctions.urldecode((XVar)(arrkeys[ind.Value])), ind.Value);
			}
			if(this.lockingAction == "unlock")
			{
				this.lockingObj.UnlockRecord((XVar)(this.tName), (XVar)(arrkeys), (XVar)(this.lockingSid));
			}
			else
			{
				if((XVar)(this.lockingAction == "lockadmin")  && (XVar)(this.lockingAdmin()))
				{
					this.lockingObj.UnlockAdmin((XVar)(this.tName), (XVar)(arrkeys), (XVar)(this.lockingStart == "yes"));
					if(this.lockingStart == "no")
					{
						MVCFunctions.Echo("unlock");
					}
					else
					{
						if(this.lockingStart == "yes")
						{
							MVCFunctions.Echo("lock");
						}
					}
				}
				else
				{
					if(this.lockingAction == "confirm")
					{
						dynamic lockMessage = null;
						lockMessage = new XVar("");
						if(XVar.Pack(!(XVar)(this.lockingObj.ConfirmLock((XVar)(this.tName), (XVar)(arrkeys), ref lockMessage))))
						{
							MVCFunctions.Echo(lockMessage);
						}
					}
				}
			}

			return null;
		}
		public override XVar setTemplateFile()
		{
			if(this.mode == Constants.EDIT_INLINE)
			{
				this.templatefile = XVar.Clone(CommonFunctions.GetTemplateName((XVar)(this.shortTableName), new XVar("inline_edit")));
			}
			base.setTemplateFile();

			return null;
		}
		public override XVar init()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessEdit"))))
			{
				this.eventsObject.BeforeProcessEdit(this);
			}
			base.init();

			return null;
		}
		public virtual XVar process()
		{
			if(this.action == "edited")
			{
				this.processDataInput();
				this.readEditValues = XVar.Clone(!(XVar)(this.updatedSuccessfully));
				if((XVar)(this.mode == Constants.EDIT_INLINE)  || (XVar)(this.isPopupMode()))
				{
					this.reportInlineSaveStatus();
					return null;
				}
				if(XVar.Pack(this.updatedSuccessfully))
				{
					if(XVar.Pack(this.afterEditActionRedirect()))
					{
						return null;
					}
				}
			}
			if(XVar.Pack(this.captchaExists()))
			{
				this.displayCaptcha();
			}
			this.prgReadMessage();
			if(XVar.Pack(!(XVar)(this.readRecord())))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(this.isRecordEditable(new XVar(false)))))
			{
				return this.SecurityRedirect();
			}
			if(XVar.Pack(!(XVar)(this.lockRecord())))
			{
				return null;
			}
			this.doCommonAssignments();
			this.prepareBreadcrumbs();
			this.prepareCollapseButton();
			this.prepareButtons();
			this.prepareSteps();
			this.prepareEditControls();
			this.prepareReadonlyFields();
			this.prepareJsSettings();
			this.prepareDetailsTables();
			if(this.mode != Constants.EDIT_INLINE)
			{
				this.addButtonHandlers();
			}
			this.addCommonJs();
			this.displayEditPage();

			return null;
		}
		public override XVar addCommonJs()
		{
			base.addCommonJs();

			return null;
		}
		protected virtual XVar prepareJsSettings()
		{
			this.pageData.InitAndSetArrayItem(this.getDetailTablesMasterKeys((XVar)(this.getCurrentRecordInternal())), "detailsMasterKeys");
			if(XVar.Pack(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_recordUpdated")]))
			{
				this.setProxyValue((XVar)(MVCFunctions.Concat(this.shortTableName, "_recordUpdated")), new XVar(true));
				XSession.Session.Remove(MVCFunctions.Concat(this.sessionPrefix, "_recordUpdated"));
			}
			else
			{
				this.setProxyValue((XVar)(MVCFunctions.Concat(this.shortTableName, "_recordUpdated")), new XVar(false));
			}
			if((XVar)(this.listPage)  && (XVar)(this.getAfterEditAction() == Constants.AE_TO_LIST))
			{
				this.pageData.InitAndSetArrayItem(this.listPage, "listPage");
			}

			return null;
		}
		protected virtual XVar doCommonAssignments()
		{
			dynamic data = null;
			if(XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.EDIT_SIMPLE)))
			{
				this.headerCommonAssign();
			}
			else
			{
				this.xt.assign(new XVar("menu_chiddenattr"), new XVar("data-hidden"));
			}
			this.setLangParams();
			this.xt.assign(new XVar("message_block"), new XVar(true));
			if(XVar.Pack(this.isMessageSet()))
			{
				this.xt.assign(new XVar("message"), (XVar)(this.message));
				this.xt.assign(new XVar("message_class"), (XVar)((XVar.Pack(this.messageType == Constants.MESSAGE_ERROR) ? XVar.Pack("alert alert-danger") : XVar.Pack("alert alert-success"))));
			}
			else
			{
				this.hideElement(new XVar("message"));
			}
			this.assignFieldBlocksAndLabels();
			if(XVar.Pack(this.isSimpleMode()))
			{
				this.assignBody();
				this.xt.assign(new XVar("flybody"), new XVar(true));
			}
			data = XVar.Clone(this.getCurrentRecordInternal());
			this.xt.assign(new XVar("editlink"), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(new XVar(0, this.getEditLink((XVar)(data)), 1, this.getStateUrlParams())))));

			return null;
		}
		protected virtual XVar displayEditPage()
		{
			dynamic templatefile = null;
			templatefile = XVar.Clone(this.templatefile);
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowEdit"))))
			{
				this.eventsObject.BeforeShowEdit((XVar)(this.xt), ref templatefile, (XVar)(this.getCurrentRecordInternal()), this);
			}
			if((XVar)(this.mode != Constants.EDIT_INLINE)  && (XVar)(this.mode != Constants.EDIT_ONTHEFLY))
			{
				this.displayMasterTableInfo();
			}
			this.fillSetCntrlMaps();
			if(XVar.Pack(this.isSimpleMode()))
			{
				this.display((XVar)(templatefile));
				return null;
			}
			if((XVar)((XVar)(this.isPopupMode())  || (XVar)(this.mode == Constants.EDIT_DASHBOARD))  || (XVar)(this.mode == Constants.EDIT_ONTHEFLY))
			{
				this.xt.assign(new XVar("footer"), new XVar(false));
				this.xt.assign(new XVar("header"), new XVar(false));
				this.xt.assign(new XVar("body"), (XVar)(this.body));
				this.displayAJAX((XVar)(templatefile), (XVar)(this.flyId + 1));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			if(this.mode == Constants.EDIT_INLINE)
			{
				dynamic returnJSON = XVar.Array();
				returnJSON = XVar.Clone(XVar.Array());
				this.xt.load_template((XVar)(templatefile));
				returnJSON.InitAndSetArrayItem(XVar.Array(), "htmlControls");
				foreach (KeyValuePair<XVar, dynamic> f in this.editFields.GetEnumerator())
				{
					returnJSON.InitAndSetArrayItem(this.xt.fetchVar((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_editcontrol"))), "htmlControls", f.Value);
				}
				returnJSON.InitAndSetArrayItem(GlobalVars.pagesData, "pagesData");
				returnJSON.InitAndSetArrayItem(this.jsSettings, "settings");
				returnJSON.InitAndSetArrayItem(this.controlsHTMLMap, "controlsMap");
				returnJSON.InitAndSetArrayItem(this.viewControlsHTMLMap, "viewControlsMap");
				returnJSON.InitAndSetArrayItem(this.grabAllJsFiles(), "additionalJS");
				returnJSON.InitAndSetArrayItem(this.grabAllCSSFiles(), "additionalCSS");
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}

			return null;
		}
		protected override XVar getBodyMarkup(dynamic _param_templatefile)
		{
			#region pass-by-value parameters
			dynamic templatefile = XVar.Clone(_param_templatefile);
			#endregion

			this.xt.assign(new XVar("locking"), new XVar(""));
			return MVCFunctions.Concat(this.lockingMessageBlock, this.fetchForms((XVar)(this.bodyForms)));
		}
		protected override XVar getExtraAjaxPageParams()
		{
			return this.getSaveStatusJSON();
		}
		protected virtual XVar prepareDetailsTables()
		{
			dynamic d = null, dpParams = XVar.Array(), dpTablesParams = XVar.Array();
			if(this.mode == Constants.EDIT_INLINE)
			{
				return null;
			}
			dpParams = XVar.Clone(this.getDetailsParams((XVar)(this.id)));
			if(XVar.Pack(!(XVar)(dpParams["ids"])))
			{
				return null;
			}
			if(this.mode == Constants.EDIT_DASHBOARD)
			{
				dpTablesParams = XVar.Clone(XVar.Array());
			}
			this.xt.assign(new XVar("detail_tables"), new XVar(true));
			this.flyId = XVar.Clone(dpParams["ids"][MVCFunctions.count(dpParams["ids"]) - 1] + 1);
			d = new XVar(0);
			for(;d < MVCFunctions.count(dpParams["ids"]); d++)
			{
				if(this.mode != Constants.EDIT_DASHBOARD)
				{
					this.setDetailPreview((XVar)(dpParams["type"][d]), (XVar)(dpParams["strTableNames"][d]), (XVar)(dpParams["ids"][d]), (XVar)(this.getCurrentRecordInternal()));
					this.displayDetailsButtons((XVar)(dpParams["type"][d]), (XVar)(dpParams["strTableNames"][d]), (XVar)(dpParams["ids"][d]));
				}
				else
				{
					this.xt.assign((XVar)(MVCFunctions.Concat("details_", dpParams["shorTNames"][d])), new XVar(true));
					dpTablesParams.InitAndSetArrayItem(new XVar("tName", dpParams["strTableNames"][d], "id", dpParams["ids"][d], "pType", dpParams["type"][d]), null);
					this.xt.assign((XVar)(MVCFunctions.Concat("displayDetailTable_", dpParams["shorTNames"][d])), (XVar)(MVCFunctions.Concat("<div id='dp_", MVCFunctions.GoodFieldName((XVar)(this.tName)), "_", this.pageType, "_", dpParams["ids"][d], "'></div>")));
				}
			}
			if(this.mode == Constants.EDIT_DASHBOARD)
			{
				this.controlsMap.InitAndSetArrayItem(dpTablesParams, "dpTablesParams");
			}

			return null;
		}
		protected virtual XVar displayDetailsButtons(dynamic _param_dpType, dynamic _param_dpTableName, dynamic _param_dpId)
		{
			#region pass-by-value parameters
			dynamic dpType = XVar.Clone(_param_dpType);
			dynamic dpTableName = XVar.Clone(_param_dpTableName);
			dynamic dpId = XVar.Clone(_param_dpId);
			#endregion

			dynamic listPageObject = null;
			if(XVar.Pack(!(XVar)(CommonFunctions.CheckTablePermissions((XVar)(dpTableName), new XVar("S")))))
			{
				return null;
			}
			if((XVar)(dpType == Constants.PAGE_CHART)  || (XVar)(dpType == Constants.PAGE_REPORT))
			{
				return null;
			}
			listPageObject = XVar.Clone(this.getDetailsPageObject((XVar)(dpTableName), (XVar)(dpId)));
			listPageObject.assignButtonsOnMasterEdit((XVar)(this.xt));

			return null;
		}
		protected virtual XVar prepareButtons()
		{
			dynamic addStyle = null;
			if(this.mode == Constants.EDIT_INLINE)
			{
				return null;
			}
			if(!XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.EDIT_ONTHEFLY)))
			{
				this.prepareNextPrevButtons();
			}
			if(XVar.Pack(this.isPopupMode()))
			{
				this.xt.assign(new XVar("close_button"), new XVar(true));
				this.xt.assign(new XVar("closebutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"closeButton", this.id, "\"")));
			}
			this.xt.assign(new XVar("save_button"), new XVar(true));
			if((XVar)(!XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.EDIT_SELECTED_SIMPLE)))  && (XVar)(!XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.EDIT_SELECTED_POPUP))))
			{
				this.xt.assign(new XVar("save_edit"), new XVar(true));
			}
			else
			{
				this.xt.assign(new XVar("save_update"), new XVar(true));
			}
			addStyle = new XVar("");
			if(XVar.Pack(this.isMultistepped()))
			{
				addStyle = new XVar(" style=\"display: none;\"");
			}
			this.xt.assign(new XVar("savebutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"saveButton", this.id, "\"", addStyle)));
			this.xt.assign(new XVar("resetbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"resetButton", this.id, "\"")));
			this.xt.assign(new XVar("reset_button"), new XVar(true));
			if(this.mode == Constants.EDIT_DASHBOARD)
			{
				return null;
			}
			if(XVar.Pack(this.isSimpleMode()))
			{
				if(XVar.Pack(XSession.Session.KeyExists("successfulEdit")))
				{
					this.xt.assign(new XVar("message_back_button"), new XVar(true));
				}
				if(XVar.Pack(this.pSet.hasListPage()))
				{
					this.xt.assign(new XVar("back_button"), new XVar(true));
					this.xt.assign(new XVar("backbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"backButton", this.id, "\"")));
					this.xt.assign(new XVar("mbackbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"extraBackButton", this.id, "\"")));
				}
				else
				{
					if(XVar.Pack(this.isShowMenu()))
					{
						this.xt.assign(new XVar("back_button"), new XVar(true));
						this.xt.assign(new XVar("backbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"backToMenuButton", this.id, "\"")));
					}
				}
			}
			if((XVar)(this.viewAvailable())  && (XVar)(!XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.EDIT_ONTHEFLY))))
			{
				this.xt.assign(new XVar("view_page_button"), new XVar(true));
				this.xt.assign(new XVar("view_page_button_attrs"), (XVar)(MVCFunctions.Concat("id=\"viewPageButton", this.id, "\"")));
				if(XVar.Pack(XSession.Session["successfulEdit"]))
				{
					this.xt.assign(new XVar("message_view_page_button"), new XVar(true));
				}
			}
			XSession.Session.Remove("successfulEdit");

			return null;
		}
		protected virtual XVar prepareNextPrevButtons()
		{
			dynamic nextPrev = XVar.Array();
			if(XVar.Pack(!(XVar)(this.pSet.useMoveNext())))
			{
				this.hideItemType(new XVar("prev"));
				this.hideItemType(new XVar("next"));
				return null;
			}
			nextPrev = XVar.Clone(this.getNextPrevRecordKeys((XVar)(this.getCurrentRecordInternal())));
			this.prevKeys = XVar.Clone(nextPrev["prev"]);
			this.nextKeys = XVar.Clone(nextPrev["next"]);
			this.assignPrevNextButtons((XVar)(!(XVar)(!(XVar)(nextPrev["next"]))), (XVar)(!(XVar)(!(XVar)(nextPrev["prev"]))), (XVar)((XVar)(this.mode == Constants.EDIT_DASHBOARD)  && (XVar)((XVar)(this.hasTableDashGridElement())  || (XVar)(this.hasDashMapElement()))));

			return null;
		}
		protected virtual XVar readRecord()
		{
			if(XVar.Pack(this.getCurrentRecordInternal()))
			{
				return true;
			}
			if(XVar.Pack(this.isSimpleMode()))
			{
				MVCFunctions.HeaderRedirect((XVar)(this.pSet.getShortTableName()), new XVar("list"), (XVar)(MVCFunctions.Concat("a=return&", this.getStateUrlParams())));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();
			return false;
		}
		protected virtual XVar prepareReadonlyFields()
		{
			dynamic data = null, fields = XVar.Array(), keyParams = XVar.Array(), keylink = null;
			fields = XVar.Clone(this.pSet.getFieldsList());
			data = XVar.Clone(this.getFieldControlValues());
			keyParams = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> k in this.pSet.getTableKeys().GetEnumerator())
			{
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("key", k.Key + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(this.keys[k.Value]))))), null);
			}
			keylink = XVar.Clone(MVCFunctions.Concat("&", MVCFunctions.implode(new XVar("&"), (XVar)(keyParams))));
			foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
			{
				if((XVar)(this.getEditFormat((XVar)(f.Value)) == Constants.EDIT_FORMAT_READONLY)  && (XVar)((XVar)(this.pSet.appearOnEditPage((XVar)(f.Value)))  || (XVar)(this.pSet.appearOnInlineEdit((XVar)(f.Value)))))
				{
					this.readOnlyFields.InitAndSetArrayItem(this.showDBValue((XVar)(f.Value), (XVar)(data), (XVar)(keylink)), f.Value);
				}
			}

			return null;
		}
		protected virtual XVar lockRecord()
		{
			if(XVar.Pack(!(XVar)(this.lockingObj)))
			{
				return true;
			}
			if(XVar.Pack(this.lockingObj.LockRecord((XVar)(this.tName), (XVar)(this.keys))))
			{
				this.lockingMessageBlock = XVar.Clone(MVCFunctions.Concat("<div class=\"rnr-locking\" style=\"display:none\" ", this.lockingMessageAttr, ">", this.lockingMessageText, "</div>"));
				this.xt.assign(new XVar("locking"), (XVar)(this.lockingMessageBlock));
				return true;
			}
			if(this.mode == Constants.EDIT_INLINE)
			{
				dynamic returnJSON = XVar.Array();
				returnJSON = XVar.Clone(XVar.Array());
				returnJSON.InitAndSetArrayItem(false, "success");
				if(XVar.Pack(this.lockingAdmin()))
				{
					returnJSON.InitAndSetArrayItem(this.lockingObj.GetLockInfo((XVar)(this.tName), (XVar)(this.keys), new XVar(false), (XVar)(this.id)), "message");
				}
				else
				{
					returnJSON.InitAndSetArrayItem(this.lockingObj.LockUser, "message");
				}
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			this.lockingMessageText = XVar.Clone(this.lockingObj.LockUser);
			this.pageData.InitAndSetArrayItem(true, "lockedByOther");
			if(XVar.Pack(this.lockingAdmin()))
			{
				dynamic ribbonMessage = null;
				ribbonMessage = XVar.Clone(this.lockingObj.GetLockInfo((XVar)(this.tName), (XVar)(this.keys), new XVar(true), (XVar)(this.id)));
				if(ribbonMessage != XVar.Pack(""))
				{
					this.lockingMessageText = XVar.Clone(ribbonMessage);
				}
			}
			this.lockingMessageBlock = XVar.Clone(MVCFunctions.Concat("<div class=\"rnr-locking\" style=\"display:none\">", this.lockingMessageText, "</div>"));
			this.xt.assign(new XVar("locking"), (XVar)(this.lockingMessageBlock));
			return true;
		}
		protected virtual XVar reportInlineSaveStatus()
		{
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(this.getSaveStatusJSON())));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar getSaveStatusJSON()
		{
			dynamic controlValues = XVar.Array(), data = XVar.Array(), dmapIconsData = null, fieldsIconsData = null, i = null, keyParams = XVar.Array(), keylink = null, listPSet = null, rawValues = XVar.Array(), returnJSON = XVar.Array(), values = XVar.Array();
			returnJSON = XVar.Clone(XVar.Array());
			if((XVar)(this.action != "edited")  || (XVar)(this.isSimpleMode()))
			{
				return returnJSON;
			}
			returnJSON.InitAndSetArrayItem(this.updatedSuccessfully, "success");
			returnJSON.InitAndSetArrayItem(this.message, "message");
			returnJSON.InitAndSetArrayItem(this.lockingMessageText, "lockMessage");
			if(XVar.Pack(!(XVar)(this.isCaptchaOk)))
			{
				returnJSON.InitAndSetArrayItem(this.getCaptchaFieldName(), "wrongCaptchaFieldName");
			}
			if(XVar.Pack(!(XVar)(this.updatedSuccessfully)))
			{
				return returnJSON;
			}
			data = XVar.Clone(this.getCurrentRecordInternal());
			if(XVar.Pack(!(XVar)(data)))
			{
				data = XVar.Clone(this.newRecordData);
			}
			returnJSON.InitAndSetArrayItem(XVar.Array(), "detKeys");
			foreach (KeyValuePair<XVar, dynamic> dt in this.pSet.getAvailableDetailsTables().GetEnumerator())
			{
				dynamic detailsKeys = XVar.Array(), dkeys = XVar.Array();
				dkeys = XVar.Clone(XVar.Array());
				detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(dt.Value)));
				foreach (KeyValuePair<XVar, dynamic> mk in detailsKeys["masterKeys"].GetEnumerator())
				{
					dkeys.InitAndSetArrayItem(data[mk.Value], MVCFunctions.Concat("masterkey", mk.Key + 1));
				}
				returnJSON.InitAndSetArrayItem(dkeys, "detKeys", dt.Value);
			}
			keyParams = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> k in this.pSet.getTableKeys().GetEnumerator())
			{
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("key", k.Key + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(this.keys[k.Value]))))), null);
			}
			keylink = XVar.Clone(MVCFunctions.Concat("&", MVCFunctions.implode(new XVar("&"), (XVar)(keyParams))));
			values = XVar.Clone(XVar.Array());
			rawValues = XVar.Clone(XVar.Array());
			controlValues = XVar.Clone(XVar.Array());
			listPSet = XVar.Clone(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_LIST), (XVar)(this.hostPageName), (XVar)(this.pageTable)));
			this.viewControls = XVar.Clone(new ViewControlsContainer((XVar)(listPSet), new XVar(Constants.PAGE_LIST), this));
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getFieldsList().GetEnumerator())
			{
				dynamic value = null;
				value = XVar.Clone(this.showDBValue((XVar)(f.Value), (XVar)(data), (XVar)(keylink)));
				values.InitAndSetArrayItem(value, f.Value);
				if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(this.pSet.getFieldType((XVar)(f.Value))))))
				{
					rawValues.InitAndSetArrayItem("", f.Value);
				}
				else
				{
					rawValues.InitAndSetArrayItem(MVCFunctions.runner_substr((XVar)(data[f.Value]), new XVar(0), new XVar(100)), f.Value);
					controlValues.InitAndSetArrayItem(data[f.Value], f.Value);
				}
			}
			returnJSON.InitAndSetArrayItem(this.jsKeys, "keys");
			returnJSON.InitAndSetArrayItem(this.getDetailTablesMasterKeys((XVar)(data)), "masterKeys");
			returnJSON.InitAndSetArrayItem(this.pSet.getTableKeys(), "keyFields");
			returnJSON.InitAndSetArrayItem(XVar.Array(), "oldKeys");
			i = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> value in this.oldKeys.GetEnumerator())
			{
				returnJSON.InitAndSetArrayItem(value.Value, "oldKeys", i++);
			}
			if(this.mode == Constants.EDIT_ONTHEFLY)
			{
				dynamic lokupData = XVar.Array();
				lokupData = XVar.Clone(this.getLookupData((XVar)(this.lookupTable), (XVar)(this.lookupField), (XVar)(this.lookupPageType), (XVar)(this.newRecordData)));
				returnJSON.InitAndSetArrayItem(lokupData["linkValue"], "linkValue");
				returnJSON.InitAndSetArrayItem(lokupData["displayValue"], "displayValue");
				returnJSON.InitAndSetArrayItem(lokupData["vals"], "vals");
				return returnJSON;
			}
			returnJSON.InitAndSetArrayItem(controlValues, "controlValues");
			returnJSON.InitAndSetArrayItem(values, "vals");
			returnJSON.InitAndSetArrayItem(this.pSet.getFieldsList(), "fields");
			returnJSON.InitAndSetArrayItem(rawValues, "rawVals");
			returnJSON.InitAndSetArrayItem(this.buildDetailGridLinks((XVar)(returnJSON["detKeys"])), "hrefs");
			if(XVar.Pack(!(XVar)(this.isRecordEditable(new XVar(false)))))
			{
				returnJSON.InitAndSetArrayItem(true, "nonEditable");
			}
			dmapIconsData = XVar.Clone(this.getDashMapsIconsData((XVar)(data)));
			if(XVar.Pack(!(XVar)(!(XVar)(dmapIconsData))))
			{
				returnJSON.InitAndSetArrayItem(dmapIconsData, "mapIconsData");
			}
			fieldsIconsData = XVar.Clone(this.getFieldMapIconsData((XVar)(data)));
			if(XVar.Pack(!(XVar)(!(XVar)(fieldsIconsData))))
			{
				returnJSON.InitAndSetArrayItem(fieldsIconsData, "fieldsMapIconsData");
			}
			returnJSON.InitAndSetArrayItem(this.editFields, "editFields");
			if(XVar.Pack(this.forSpreadsheetGrid))
			{
				returnJSON.InitAndSetArrayItem(listPSet.getInlineEditFields(), "editFields");
			}
			return returnJSON;
		}
		protected virtual XVar afterEditActionRedirect()
		{
			dynamic dTName = null, nextKeys = null, prevKeys = null, stateParams = null;
			if(XVar.Pack(!(XVar)(this.isSimpleMode())))
			{
				return false;
			}
			stateParams = XVar.Clone(this.getStateUrlParams());
			switch(((XVar)this.getAfterEditAction()).ToInt())
			{
				case Constants.AE_TO_EDIT:
					return this.prgRedirect();
				case Constants.AE_TO_LIST:
					if(XVar.Pack(this.pSet.hasListPage()))
					{
						MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), new XVar(Constants.PAGE_LIST), (XVar)(MVCFunctions.Concat("a=return&", (XVar.Pack(this.listPage) ? XVar.Pack(MVCFunctions.Concat("page=", this.listPage, "&")) : XVar.Pack("")), stateParams)));
					}
					else
					{
						MVCFunctions.HeaderRedirect(new XVar("menu"));
					}
					return true;
				case Constants.AE_TO_VIEW:
					MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), new XVar(Constants.PAGE_VIEW), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(new XVar(0, this.getKeyParams(), 1, stateParams)))));
					return true;
				case Constants.AE_TO_PREV_EDIT:
					XSession.Session["message_edit"] = MVCFunctions.Concat(this.message, "");
					prevKeys = XVar.Clone(this.getPrevKeys());
					MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), new XVar(Constants.PAGE_EDIT), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(new XVar(0, this.getKeyParams((XVar)(prevKeys)), 1, stateParams)))));
					return true;
				case Constants.AE_TO_NEXT_EDIT:
					XSession.Session["message_edit"] = MVCFunctions.Concat(this.message, "");
					nextKeys = XVar.Clone(this.getNextKeys());
					MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), new XVar(Constants.PAGE_EDIT), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(new XVar(0, this.getKeyParams((XVar)(nextKeys)), 1, stateParams)))));
					return true;
				case Constants.AE_TO_DETAIL_LIST:
					dTName = XVar.Clone(this.pSet.getAEDetailTable());
					MVCFunctions.HeaderRedirect((XVar)(CommonFunctions.GetTableURL((XVar)(dTName))), new XVar(Constants.PAGE_LIST), (XVar)(MVCFunctions.Concat(MVCFunctions.implode(new XVar("&"), (XVar)(this.getNewRecordMasterKeys((XVar)(dTName)))), "&mastertable=", this.tName)));
					return true;
				default:
					return false;
			}

			return null;
		}
		public virtual XVar getNewRecordMasterKeys(dynamic _param_dTName)
		{
			#region pass-by-value parameters
			dynamic dTName = XVar.Clone(_param_dTName);
			#endregion

			dynamic data = XVar.Array(), detailsKeys = XVar.Array(), mKeys = XVar.Array();
			data = XVar.Clone(this.getCurrentRecordInternal());
			mKeys = XVar.Clone(XVar.Array());
			detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(dTName)));
			foreach (KeyValuePair<XVar, dynamic> mk in detailsKeys["masterKeys"].GetEnumerator())
			{
				mKeys.InitAndSetArrayItem(MVCFunctions.Concat("masterkey", mk.Key + 1, "=", data[mk.Value]), null);
			}
			return mKeys;
		}
		protected virtual XVar getPrevKeys()
		{
			dynamic keys = XVar.Array();
			if((XVar)(true)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.prevKeys), XVar.Pack(null)))))
			{
				return this.prevKeys;
			}
			keys = XVar.Clone(this.getNextPrevRecordKeys((XVar)(this.getCurrentRecordInternal()), new XVar(Constants.PREV_RECORD)));
			this.prevKeys = XVar.Clone(keys["prev"]);
			return this.prevKeys;
		}
		protected virtual XVar getNextKeys()
		{
			dynamic keys = XVar.Array();
			if((XVar)(true)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.nextKeys), XVar.Pack(null)))))
			{
				return this.nextKeys;
			}
			keys = XVar.Clone(this.getNextPrevRecordKeys((XVar)(this.getCurrentRecordInternal()), new XVar(Constants.NEXT_RECORD)));
			this.nextKeys = XVar.Clone(keys["next"]);
			return this.nextKeys;
		}
		protected virtual XVar prgRedirect()
		{
			dynamic getParams = null;
			if(XVar.Pack(this.stopPRG))
			{
				return false;
			}
			if((XVar)((XVar)(!(XVar)(this.updatedSuccessfully))  || (XVar)(!(XVar)(this.isSimpleMode())))  || (XVar)(!(XVar)(MVCFunctions.no_output_done())))
			{
				return false;
			}
			XSession.Session["message_edit"] = MVCFunctions.Concat(this.message, "");
			XSession.Session["message_edit_type"] = this.messageType;
			getParams = XVar.Clone(MVCFunctions.implode(new XVar("&"), (XVar)(new XVar(0, this.getKeyParams(), 1, this.getStateUrlParams()))));
			if(XVar.Pack(this.pageName))
			{
				getParams = MVCFunctions.Concat(getParams, "&page=", this.pageName);
			}
			MVCFunctions.HeaderRedirect((XVar)(this.pSet.getShortTableName()), (XVar)(this.getPageType()), (XVar)(getParams));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();
			return true;
		}
		protected virtual XVar prgReadMessage()
		{
			if((XVar)(!(XVar)(this.isSimpleMode()))  || (XVar)(!(XVar)(XSession.Session.KeyExists("message_edit"))))
			{
				return null;
			}
			this.setMessage((XVar)(XSession.Session["message_edit"]));
			this.messageType = XVar.Clone(XSession.Session["message_edit_type"]);
			XSession.Session.Remove("message_edit");

			return null;
		}
		public override XVar getCurrentRecord()
		{
			dynamic data = XVar.Array(), newData = XVar.Array();
			data = XVar.Clone(this.getCurrentRecordInternal());
			newData = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> val in data.GetEnumerator())
			{
				dynamic editFormat = null;
				editFormat = XVar.Clone(this.getEditFormat((XVar)(val.Key)));
				if((XVar)(editFormat == Constants.EDIT_FORMAT_DATABASE_FILE)  || (XVar)(editFormat == Constants.EDIT_FORMAT_DATABASE_IMAGE))
				{
					if(XVar.Pack(data[val.Key]))
					{
						newData.InitAndSetArrayItem(true, val.Key);
					}
					else
					{
						newData.InitAndSetArrayItem(false, val.Key);
					}
				}
			}
			foreach (KeyValuePair<XVar, dynamic> val in newData.GetEnumerator())
			{
				data.InitAndSetArrayItem(val.Value, val.Key);
			}
			return data;
		}
		public virtual XVar getKeysWhereClause(dynamic _param_useOldKeys)
		{
			#region pass-by-value parameters
			dynamic useOldKeys = XVar.Clone(_param_useOldKeys);
			#endregion

			dynamic dc = null, sql = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			dc.keys = XVar.Clone((XVar.Pack(useOldKeys) ? XVar.Pack(this.oldKeys) : XVar.Pack(this.keys)));
			dc.filter = XVar.Clone(this.getSecurityCondition());
			sql = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
			return sql["where"];
		}
		public virtual XVar getCurrentRecordInternal()
		{
			dynamic dc = null, fetchedArray = null;
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(this.cachedRecord), XVar.Pack(null)))))
			{
				return this.cachedRecord;
			}
			dc = XVar.Clone(this.getSingleRecordCommand());
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeQueryEdit"))))
			{
				dynamic prep = XVar.Array(), sql = null, where = null;
				prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
				where = XVar.Clone(prep["where"]);
				sql = XVar.Clone(prep["sql"]);
				this.eventsObject.BeforeQueryEdit((XVar)(sql), ref where, this);
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
				}
			}
			fetchedArray = XVar.Clone(this.dataSource.getSingle((XVar)(dc)).fetchAssoc());
			this.cachedRecord = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
			if(XVar.Pack(!(XVar)(this.checkKeysSet())))
			{
				this.keys = XVar.Clone(this.getKeysFromData((XVar)(this.cachedRecord)));
				this.setKeysForJs();
			}
			if((XVar)(!(XVar)(this.cachedRecord))  && (XVar)(this.mode == Constants.EDIT_SIMPLE))
			{
				return this.cachedRecord;
			}
			foreach (KeyValuePair<XVar, dynamic> fName in this.getPageFields().GetEnumerator())
			{
				if((XVar)(MVCFunctions.postvalue("a") != "edited")  && (XVar)(!XVar.Equals(XVar.Pack(this.pSet.getAutoUpdateValue((XVar)(fName.Value))), XVar.Pack(""))))
				{
					this.cachedRecord.InitAndSetArrayItem(this.pSet.getAutoUpdateValue((XVar)(fName.Value)), fName.Value);
				}
			}
			if(XVar.Pack(this.readEditValues))
			{
				foreach (KeyValuePair<XVar, dynamic> fName in this.getPageFields().GetEnumerator())
				{
					dynamic editFormat = null;
					editFormat = XVar.Clone(this.getEditFormat((XVar)(fName.Value)));
					if((XVar)(!(XVar)(ProjectSettings.uploadEditType((XVar)(editFormat))))  && (XVar)(!XVar.Equals(XVar.Pack(editFormat), XVar.Pack(Constants.EDIT_FORMAT_READONLY))))
					{
						this.cachedRecord.InitAndSetArrayItem(this.newRecordData[fName.Value], fName.Value);
					}
				}
			}
			return this.cachedRecord;
		}
		protected virtual XVar checkKeysSet()
		{
			foreach (KeyValuePair<XVar, dynamic> kValue in this.keys.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(kValue.Value))))
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar getFieldControlValues()
		{
			dynamic data = XVar.Array();
			data = XVar.Clone(this.getFieldControlsData());
			if(XVar.Pack(this.readEditValues))
			{
				foreach (KeyValuePair<XVar, dynamic> f in this.editFields.GetEnumerator())
				{
					dynamic editFormat = null;
					if(XVar.Pack(!(XVar)(this.newRecordData.KeyExists(f.Value))))
					{
						continue;
					}
					editFormat = XVar.Clone(this.getEditFormat((XVar)(f.Value)));
					if((XVar)(!(XVar)(ProjectSettings.uploadEditType((XVar)(editFormat))))  && (XVar)(editFormat != Constants.EDIT_FORMAT_READONLY))
					{
						data.InitAndSetArrayItem(this.newRecordData[f.Value], f.Value);
					}
				}
			}
			return data;
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

			if(XVar.Pack(this.detailsKeyField((XVar)(field))))
			{
				return Constants.EDIT_FORMAT_READONLY;
			}
			return base.getEditFormat((XVar)(field), (XVar)(pSet));
		}
		protected virtual XVar prepareEditControl(dynamic _param_fName, dynamic data)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			dynamic controls = XVar.Array(), firstElementId = null, gf = null, parameters = XVar.Array();
			gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(fName)));
			firstElementId = XVar.Clone(this.getControl((XVar)(fName), (XVar)(this.id)).getFirstElementId());
			if(XVar.Pack(firstElementId))
			{
				this.xt.assign((XVar)(MVCFunctions.Concat("labelfor_", gf)), (XVar)(firstElementId));
			}
			parameters = XVar.Clone(this.getEditContolParams((XVar)(fName), (XVar)(this.id), (XVar)(data)));
			if(this.pSet.getEditFormat((XVar)(fName)) == Constants.EDIT_FORMAT_CHECKBOX)
			{
				parameters.InitAndSetArrayItem(this.xt, "xt");
				parameters.InitAndSetArrayItem(MVCFunctions.Concat(gf, "_forward_control"), "clearVar");
			}
			this.xt.assign_function((XVar)(MVCFunctions.Concat(gf, "_editcontrol")), new XVar("xt_buildeditcontrol"), (XVar)(parameters));
			controls = XVar.Clone(this.getContolMapData((XVar)(fName), (XVar)(this.id), (XVar)(data), (XVar)(this.editFields)));
			if(XVar.Pack(MVCFunctions.in_array((XVar)(fName), (XVar)(this.errorFields))))
			{
				controls.InitAndSetArrayItem(true, "controls", "isInvalid");
			}
			this.fillControlsMap((XVar)(controls));
			if(this.pSet.getEditFormat((XVar)(fName)) == Constants.EDIT_FORMAT_CHECKBOX)
			{
				parameters.InitAndSetArrayItem(this.xt, "xt");
				parameters.InitAndSetArrayItem(MVCFunctions.Concat(gf, "_editcontrol"), "clearVar");
				this.xt.assign_function((XVar)(MVCFunctions.Concat(gf, "_forward_control")), new XVar("xt_buildforwardcontrol"), (XVar)(parameters));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_label_class")), new XVar("r-checkbox-label"));
			}
			this.fillControlFlags((XVar)(fName));
			if(this.pSet.getViewFormat((XVar)(fName)) == Constants.FORMAT_MAP)
			{
				this.googleMapCfg.InitAndSetArrayItem(true, "isUseGoogleMap");
			}

			return null;
		}
		public virtual XVar prepareEditControls()
		{
			dynamic data = null;
			data = XVar.Clone(this.getFieldControlValues());
			foreach (KeyValuePair<XVar, dynamic> fName in this.editFields.GetEnumerator())
			{
				this.prepareEditControl((XVar)(fName.Value), (XVar)(data));
			}

			return null;
		}
		public static XVar readEditModeFromRequest()
		{
			dynamic editType = null, mode = null;
			editType = XVar.Clone(MVCFunctions.postvalue(new XVar("editType")));
			mode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(editType == "inline")
			{
				return Constants.EDIT_INLINE;
			}
			else
			{
				if(editType == Constants.EDIT_POPUP)
				{
					return Constants.EDIT_POPUP;
				}
				else
				{
					if(mode == "dashrecord")
					{
						return Constants.EDIT_DASHBOARD;
					}
					else
					{
						if(editType == Constants.EDIT_ONTHEFLY)
						{
							return Constants.EDIT_ONTHEFLY;
						}
						else
						{
							return Constants.EDIT_SIMPLE;
						}
					}
				}
			}

			return null;
		}
		public static XVar processEditPageSecurity(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic i = null, keyParams = XVar.Array(), pageMode = null;
			if(XVar.Pack(Security.checkPagePermissions((XVar)(table), new XVar("E"))))
			{
				return true;
			}
			if(MVCFunctions.postvalue(new XVar("a")) == "edited")
			{
				return true;
			}
			pageMode = XVar.Clone(EditPage.readEditModeFromRequest());
			if(pageMode != Constants.EDIT_SIMPLE)
			{
				dynamic messageLink = null;
				messageLink = new XVar("");
				if((XVar)(!(XVar)(CommonFunctions.isLogged()))  || (XVar)(Security.isGuest()))
				{
					messageLink = XVar.Clone(MVCFunctions.Concat(" <a href='#' id='loginButtonContinue'>", CommonFunctions.mlang_message(new XVar("SESSION_EXPIRED3")), "</a>"));
				}
				Security.sendPermissionError((XVar)(messageLink));
				return false;
			}
			if((XVar)(CommonFunctions.isLogged())  && (XVar)(!(XVar)(Security.isGuest())))
			{
				Security.redirectToList((XVar)(table));
				return false;
			}
			keyParams = XVar.Clone(XVar.Array());
			i = new XVar(1);
			while(XVar.Pack(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("editid", i)))))
			{
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("editid", i, "=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("editid", i)))))), null);
				i++;
			}
			XSession.Session["MyURL"] = MVCFunctions.Concat(MVCFunctions.GetScriptName(), "?", MVCFunctions.implode(new XVar("&"), (XVar)(keyParams)));
			CommonFunctions.redirectToLogin();
			return false;
		}
		public static XVar handleBrokenRequest()
		{
			if((XVar)(MVCFunctions.POSTSize() != 0)  || (XVar)(!(XVar)(MVCFunctions.postvalue(new XVar("submit")))))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.postvalue(new XVar("editid1")))))
			{
				dynamic returnJSON = XVar.Array();
				returnJSON = XVar.Clone(XVar.Array());
				returnJSON.InitAndSetArrayItem(false, "success");
				returnJSON.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("INLINE_ERROR")), "message");
				returnJSON.InitAndSetArrayItem(true, "fatalError");
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			else
			{
				if(XVar.Pack(MVCFunctions.postvalue(new XVar("fly"))))
				{
					MVCFunctions.Echo(-1);
					MVCFunctions.ob_flush();
					HttpContext.Current.Response.End();
					throw new RunnerInlineOutputException();
				}
				else
				{
					XSession.Session["message_edit"] = MVCFunctions.Concat("<< ", CommonFunctions.mlang_message(new XVar("INLINE_ERROR")), " >>");
				}
			}

			return null;
		}
		protected virtual XVar buildNewRecordData()
		{
			dynamic blobfields = null, efilename_values = XVar.Array(), evalues = XVar.Array(), keys = XVar.Array();
			evalues = XVar.Clone(XVar.Array());
			efilename_values = XVar.Clone(XVar.Array());
			blobfields = XVar.Clone(XVar.Array());
			keys = XVar.Clone(this.keys);
			foreach (KeyValuePair<XVar, dynamic> f in this.editFields.GetEnumerator())
			{
				dynamic control = null;
				control = XVar.Clone(this.getControl((XVar)(f.Value), (XVar)(this.id)));
				control.readWebValue((XVar)(evalues), (XVar)(blobfields), new XVar(null), new XVar(null), (XVar)(efilename_values));
				if((XVar)(keys.KeyExists(f.Value))  && (XVar)(evalues.KeyExists(f.Value)))
				{
					if(keys[f.Value] != control.getWebValue())
					{
						keys.InitAndSetArrayItem(control.getWebValue(), f.Value);
						this.keysChanged = new XVar(true);
					}
				}
			}
			if(XVar.Pack(this.keysChanged))
			{
				this.newKeys = XVar.Clone(keys);
			}
			foreach (KeyValuePair<XVar, dynamic> value in efilename_values.GetEnumerator())
			{
				evalues.InitAndSetArrayItem(value.Value, value.Key);
			}
			this.newRecordData = XVar.Clone(evalues);

			return null;
		}
		public virtual XVar processDataInput()
		{
			if(XVar.Pack(!(XVar)(CommonFunctions.isPostRequest())))
			{
				return false;
			}
			this.oldKeys = XVar.Clone(this.keys);
			this.buildNewRecordData();
			if(XVar.Pack(!(XVar)(this.recheckUserPermissions())))
			{
				this.oldRecordData = XVar.Clone(this.newRecordData);
				this.cachedRecord = XVar.Clone(this.newRecordData);
				this.recordValuesToEdit = new XVar(null);
				return false;
			}
			if(XVar.Pack(!(XVar)(this.checkCaptcha())))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(this.isRecordEditable(new XVar(true)))))
			{
				return this.SecurityRedirect();
			}
			if(XVar.Pack(!(XVar)(this.callBeforeEditEvent())))
			{
				return false;
			}
			this.setUpdatedLatLng((XVar)(this.getNewRecordData()), (XVar)(this.getOldRecordData()));
			if(XVar.Pack(!(XVar)(this.checkDeniedDuplicatedValues())))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(this.confirmLockingBeforeSaving())))
			{
				return false;
			}
			if(XVar.Pack(this.callCustomEditEvent()))
			{
				dynamic updateResult = null;
				updateResult = XVar.Clone(this.dataSource.updateSingle((XVar)(this.getUpdateDataCommand())));
				this.updatedSuccessfully = XVar.Clone(updateResult);
				if(XVar.Pack(!(XVar)(this.updatedSuccessfully)))
				{
					this.setDatabaseError((XVar)(this.dataSource.lastError()));
				}
			}
			if(XVar.Pack(!(XVar)(this.updatedSuccessfully)))
			{
				this.unlockNewRecord();
				return false;
			}
			if(XVar.Pack(this.keysChanged))
			{
				this.setKeys((XVar)(this.newKeys));
			}
			if(XVar.Pack(MVCFunctions.in_array((XVar)(this.getAfterEditAction()), (XVar)(new XVar(0, Constants.AE_TO_EDIT, 1, Constants.AE_TO_PREV_EDIT, 2, Constants.AE_TO_NEXT_EDIT)))))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_recordUpdated")] = true;
			}
			this.ProcessFiles();
			this.messageType = new XVar(Constants.MESSAGE_INFO);
			this.setSuccessfulEditMessage();
			this.callAfterSuccessfulSave();
			this.unlockOldRecord();
			this.mergeNewRecordData();
			this.auditLogEdit();
			this.callAfterEditEvent();
			this.setKeys((XVar)(this.keys));
			return true;
		}
		protected virtual XVar setSuccessfulEditMessage()
		{
			if(XVar.Pack(this.isMessageSet()))
			{
				return null;
			}
			if(XVar.Pack(this.isSimpleMode()))
			{
				XSession.Session["successfulEdit"] = this.updatedSuccessfully;
			}
			this.setMessage((XVar)(MVCFunctions.Concat("<strong>&lt;&lt;&lt; ", CommonFunctions.mlang_message(new XVar("RECORD_UPDATED")), " &gt;&gt;&gt;</strong>")));

			return null;
		}
		protected virtual XVar checkDeniedDuplicatedValues()
		{
			dynamic oldData = null;
			oldData = XVar.Clone(this.getOldRecordData());
			return this.checkDeniedDuplicatedForUpdate((XVar)(oldData), (XVar)(this.newRecordData));
		}
		protected virtual XVar auditLogEdit(dynamic _param_keys = null)
		{
			#region default values
			if(_param_keys as Object == null) _param_keys = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			if(XVar.Pack(!(XVar)(keys)))
			{
				keys = XVar.Clone(this.keys);
			}
			if(XVar.Pack(this.auditObj))
			{
				this.auditObj.LogEdit((XVar)(this.tName), (XVar)(this.newRecordData), (XVar)(this.getOldRecordData()), (XVar)(keys));
			}

			return null;
		}
		protected virtual XVar mergeNewRecordData()
		{
			if((XVar)(!(XVar)(this.auditObj))  && (XVar)(!(XVar)(this.eventsObject.exists(new XVar("AfterEdit")))))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> v in this.getOldRecordData().GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.newRecordData.KeyExists(v.Key))))
				{
					this.newRecordData.InitAndSetArrayItem(v.Value, v.Key);
				}
			}

			return null;
		}
		protected virtual XVar callAfterEditEvent()
		{
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("AfterEdit")))))
			{
				return null;
			}
			this.eventsObject.AfterEdit((XVar)(this.newRecordData), (XVar)(this.getKeysWhereClause(new XVar(false))), (XVar)(this.getOldRecordData()), (XVar)(this.keys), (XVar)(this.mode == Constants.EDIT_INLINE), this);

			return null;
		}
		protected virtual XVar unlockOldRecord()
		{
			if((XVar)(this.lockingObj)  && (XVar)(this.keysChanged))
			{
				this.lockingObj.UnlockRecord((XVar)(this.tName), (XVar)(this.oldKeys), new XVar(""));
			}

			return null;
		}
		protected virtual XVar unlockNewRecord()
		{
			if((XVar)(this.lockingObj)  && (XVar)(this.keysChanged))
			{
				this.lockingObj.UnlockRecord((XVar)(this.tName), (XVar)(this.newKeys), new XVar(""));
			}

			return null;
		}
		protected virtual XVar callAfterSuccessfulSave()
		{
			foreach (KeyValuePair<XVar, dynamic> f in this.editFields.GetEnumerator())
			{
				this.getControl((XVar)(f.Value), (XVar)(this.id)).afterSuccessfulSave();
			}

			return null;
		}
		protected virtual XVar callBeforeEditEvent()
		{
			dynamic ret = null, usermessage = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("BeforeEdit")))))
			{
				return true;
			}
			this.sqlValues = XVar.Clone(XVar.Array());
			usermessage = new XVar("");
			ret = XVar.Clone(this.eventsObject.BeforeEdit((XVar)(this.newRecordData), (XVar)(this.sqlValues), (XVar)(this.getKeysWhereClause(new XVar(true))), (XVar)(this.getOldRecordData()), (XVar)(this.oldKeys), ref usermessage, (XVar)(this.mode == Constants.EDIT_INLINE), this));
			if(usermessage != XVar.Pack(""))
			{
				this.setMessage((XVar)(usermessage));
			}
			return ret;
		}
		protected virtual XVar callCustomEditEvent()
		{
			dynamic ret = null, usermessage = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("CustomEdit")))))
			{
				return true;
			}
			usermessage = new XVar("");
			ret = XVar.Clone(this.eventsObject.CustomEdit((XVar)(this.newRecordData), (XVar)(this.getKeysWhereClause(new XVar(true))), (XVar)(this.getOldRecordData()), (XVar)(this.oldKeys), ref usermessage, (XVar)(this.mode == Constants.EDIT_INLINE), this));
			if(0 < MVCFunctions.strlen((XVar)(usermessage)))
			{
				this.updatedSuccessfully = new XVar(false);
				this.setMessage((XVar)(usermessage));
				return false;
			}
			if(XVar.Pack(ret))
			{
				return true;
			}
			this.updatedSuccessfully = new XVar(true);
			return false;
		}
		public override XVar captchaExists()
		{
			if((XVar)((XVar)((XVar)(this.mode == Constants.ADD_ONTHEFLY)  || (XVar)(this.mode == Constants.ADD_INLINE))  || (XVar)(this.mode == Constants.EDIT_INLINE))  || (XVar)(this.mode == Constants.EDIT_ONTHEFLY))
			{
				return false;
			}
			return this.pSet.hasCaptcha();
		}
		public override XVar getCaptchaFieldName()
		{
			return this.captchaName;
		}
		protected override XVar recheckUserPermissions()
		{
			if(XVar.Pack(CommonFunctions.CheckTablePermissions((XVar)(this.tName), new XVar("E"))))
			{
				return true;
			}
			return base.recheckUserPermissions();
		}
		protected virtual XVar confirmLockingBeforeSaving()
		{
			dynamic lockConfirmed = null, lockmessage = null;
			if(XVar.Pack(!(XVar)(this.lockingObj)))
			{
				return true;
			}
			lockmessage = new XVar("");
			if(XVar.Pack(this.keysChanged))
			{
				lockConfirmed = XVar.Clone(this.lockingObj.ConfirmLock((XVar)(this.tName), (XVar)(this.oldKeys), ref lockmessage));
				if(XVar.Pack(lockConfirmed))
				{
					lockConfirmed = XVar.Clone(this.lockingObj.LockRecord((XVar)(this.tName), (XVar)(this.newKeys)));
				}
			}
			else
			{
				lockConfirmed = XVar.Clone(this.lockingObj.ConfirmLock((XVar)(this.tName), (XVar)(this.oldKeys), ref lockmessage));
			}
			if(XVar.Pack(!(XVar)(lockConfirmed)))
			{
				this.lockingMessageAttr = new XVar("");
				if(this.mode == Constants.EDIT_INLINE)
				{
					dynamic returnJSON = XVar.Array();
					if(XVar.Pack(this.lockingAdmin()))
					{
						lockmessage = XVar.Clone(this.lockingObj.GetLockInfo((XVar)(this.tName), (XVar)(this.oldKeys), new XVar(false), (XVar)(this.id)));
					}
					returnJSON.InitAndSetArrayItem(false, "success");
					returnJSON.InitAndSetArrayItem(lockmessage, "message");
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
					MVCFunctions.ob_flush();
					HttpContext.Current.Response.End();
					throw new RunnerInlineOutputException();
				}
				else
				{
					if(XVar.Pack(this.lockingAdmin()))
					{
						this.lockingMessageText = XVar.Clone(this.lockingObj.GetLockInfo((XVar)(this.tName), (XVar)(this.oldKeys), new XVar(true), (XVar)(this.id)));
					}
					else
					{
						this.lockingMessageText = XVar.Clone(lockmessage);
					}
				}
				this.readEditValues = new XVar(true);
				return false;
			}
			return true;
		}
		protected virtual XVar SecurityRedirect()
		{
			if(this.mode == Constants.EDIT_INLINE)
			{
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(new XVar("success", false, "message", CommonFunctions.mlang_message(new XVar("RECORD_ISNOT_EDITABLE"))))));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			Security.redirectToList((XVar)(this.tName));
			return null;
		}
		protected virtual XVar isRecordEditable(dynamic _param_useOldData)
		{
			#region pass-by-value parameters
			dynamic useOldData = XVar.Clone(_param_useOldData);
			#endregion

			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("IsRecordEditable"), (XVar)(this.tName))))
			{
				if(XVar.Pack(!(XVar)(GlobalVars.globalEvents.IsRecordEditable((XVar)((XVar.Pack(useOldData) ? XVar.Pack(this.getOldRecordData()) : XVar.Pack(this.getCurrentRecordInternal()))), new XVar(true), (XVar)(this.tName)))))
				{
					return false;
				}
			}
			return true;
		}
		public virtual XVar getOldRecordData()
		{
			if(XVar.Equals(XVar.Pack(this.oldRecordData), XVar.Pack(null)))
			{
				dynamic dc = null, fetchedArray = null;
				dc = XVar.Clone(new DsCommand());
				dc.filter = XVar.Clone(this.getSecurityCondition());
				dc.keys = XVar.Clone(this.oldKeys);
				fetchedArray = XVar.Clone(this.dataSource.getSingle((XVar)(dc)).fetchAssoc());
				this.oldRecordData = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
			}
			return this.oldRecordData;
		}
		public virtual XVar getUpdateDataCommand()
		{
			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getSecurityCondition());
			dc.keys = XVar.Clone(this.oldKeys);
			dc.values = this.newRecordData;
			dc.advValues = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> sqlValue in this.sqlValues.GetEnumerator())
			{
				dc.advValues.InitAndSetArrayItem(new DsOperand(new XVar(Constants.dsotSQL), (XVar)(sqlValue.Value)), sqlValue.Key);
			}
			return dc;
		}
		public virtual XVar getNewRecordData()
		{
			return this.newRecordData;
		}
		protected virtual XVar isMessageSet()
		{
			return 0 < MVCFunctions.strlen((XVar)(this.message));
		}
		public virtual XVar setDatabaseError(dynamic _param_message)
		{
			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			#endregion

			if(this.mode != Constants.EDIT_INLINE)
			{
				this.message = XVar.Clone(MVCFunctions.Concat("<strong>&lt;&lt;&lt; ", CommonFunctions.mlang_message(new XVar("RECORD_NOT_UPDATED")), "</strong> &gt;&gt;&gt;<br><br>", message));
			}
			else
			{
				this.message = XVar.Clone(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("RECORD_NOT_UPDATED")), ". ", message));
			}
			this.messageType = new XVar(Constants.MESSAGE_ERROR);

			return null;
		}
		protected override XVar checkFieldOnPage(dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			if(this.mode == Constants.EDIT_INLINE)
			{
				return this.pSet.appearOnInlineEdit((XVar)(fName));
			}
			return this.pSet.appearOnEditPage((XVar)(fName));
		}
		public override XVar getFieldControlsData()
		{
			dynamic editValues = null;
			if(XVar.Pack(this.recordValuesToEdit))
			{
				return this.recordValuesToEdit;
			}
			editValues = XVar.Clone(this.getCurrentRecordInternal());
			if(XVar.Pack(this.eventsObject.exists(new XVar("ProcessValuesEdit"))))
			{
				this.eventsObject.ProcessValuesEdit((XVar)(editValues), this);
			}
			this.recordValuesToEdit = XVar.Clone(editValues);
			return this.recordValuesToEdit;
		}
		public override XVar viewAvailable()
		{
			if(XVar.Pack(this.dashElementData))
			{
				dynamic dashType = null;
				dashType = XVar.Clone(this.dashElementData["type"]);
				return (XVar)(base.viewAvailable())  && (XVar)((XVar)((XVar)(dashType == Constants.DASHBOARD_DETAILS)  && (XVar)(this.dashElementData["details"][this.tName]["view"]))  || (XVar)((XVar)(dashType == Constants.DASHBOARD_LIST)  && (XVar)(this.dashElementData["popupView"])));
			}
			return base.viewAvailable();
		}
		public virtual XVar setMessageType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			this.messageType = XVar.Clone(var_type);

			return null;
		}
		protected virtual XVar isPopupMode()
		{
			return this.mode == Constants.EDIT_POPUP;
		}
		protected virtual XVar isSimpleMode()
		{
			return this.mode == Constants.EDIT_SIMPLE;
		}
		public static XVar EditPageFactory(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			if((XVar)(!(XVar)(var_params["selection"]))  || (XVar)(!(XVar)(MVCFunctions.is_array((XVar)(var_params["selection"])))))
			{
				var_params.InitAndSetArrayItem(XSession.Session["edit_seletion"], "selection");
				XSession.Session.Remove("edit_seletion");
			}
			if((XVar)(var_params["selection"])  && (XVar)(MVCFunctions.is_array((XVar)(var_params["selection"]))))
			{
				if(XVar.Pack(!(XVar)(!(XVar)(var_params["selection"]))))
				{
					if(var_params["mode"] == Constants.EDIT_SIMPLE)
					{
						var_params.InitAndSetArrayItem(Constants.EDIT_SELECTED_SIMPLE, "mode");
					}
					if(var_params["mode"] == Constants.EDIT_POPUP)
					{
						var_params.InitAndSetArrayItem(Constants.EDIT_SELECTED_POPUP, "mode");
					}
					return new EditSelectedPage((XVar)(var_params));
				}
			}
			if(var_params["pageName"] == "edit_calendar")
			{
				return new EditCalendarPage((XVar)(var_params));
			}
			if(XVar.Pack(var_params["gantt"]))
			{
				return new EditGanttPage((XVar)(var_params));
			}
			return new EditPage((XVar)(var_params));
		}
		public override XVar getSecurityCondition()
		{
			return Security.SelectCondition(new XVar("E"), (XVar)(this.pSet));
		}
		public override XVar inDashboardMode()
		{
			return this.mode == Constants.EDIT_DASHBOARD;
		}
		public virtual XVar getSingleRecordCommand()
		{
			dynamic dc = null;
			if(XVar.Pack(this.checkKeysSet()))
			{
				dc = XVar.Clone(new DsCommand());
				dc.filter = XVar.Clone(this.getSecurityCondition());
				dc.keys = XVar.Clone(this.keys);
			}
			else
			{
				return this.getSubsetDataCommand();
			}
			return dc;
		}
		public override XVar element2Item(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(name == "message")
			{
				return new XVar(0, "edit_message");
			}
			return base.element2Item((XVar)(name));
		}
		protected override XVar checkShowBreadcrumbs()
		{
			return this.mode == Constants.EDIT_SIMPLE;
		}
		public override XVar createProjectSettings()
		{
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), (XVar)(this.pageName), (XVar)(this.pageTable)));
			if((XVar)(this.mode != Constants.EDIT_INLINE)  && (XVar)(!XVar.Equals(XVar.Pack(this.pSet.getPageType()), XVar.Pack(Constants.PAGE_EDIT))))
			{
				this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), new XVar(null), (XVar)(this.pageTable)));
			}
			if((XVar)(this.mode == Constants.EDIT_INLINE)  || (XVar)((XVar)(this.mode == Constants.EDIT_POPUP)  && (XVar)(this.action == "edited")))
			{
				this.pSet.setPageType(new XVar("list"));
			}

			return null;
		}
		public virtual XVar getSpreadsheetControlMarkup(dynamic _param_fName, dynamic _param_recId, dynamic data)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			#endregion

			dynamic ctrl = null, ctrlParams = XVar.Array();
			ctrlParams = XVar.Clone(this.getEditContolParams((XVar)(fName), (XVar)(recId), (XVar)(data)));
			ctrlParams.InitAndSetArrayItem(Constants.MODE_INLINE_EDIT, "mode");
			ctrlParams.InitAndSetArrayItem(true, "extraParams", "spreadsheet");
			ctrl = XVar.Clone(this.getControl((XVar)(fName), (XVar)(recId), (XVar)(ctrlParams["extraParams"])));
			return ctrl.getControlMarkup((XVar)(ctrlParams), (XVar)(data));
		}
	}
}
