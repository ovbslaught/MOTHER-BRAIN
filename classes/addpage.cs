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
	public partial class AddPage : RunnerPage
	{
		public dynamic messageType = XVar.Pack(Constants.MESSAGE_ERROR);
		protected dynamic auditObj = XVar.Pack(null);
		protected dynamic addFields = XVar.Array();
		protected dynamic readAddValues = XVar.Pack(false);
		protected dynamic insertedSuccessfully = XVar.Pack(false);
		protected dynamic defvalues = XVar.Array();
		protected dynamic newRecordData = XVar.Array();
		public dynamic afterAdd_id = XVar.Pack("");
		public dynamic action = XVar.Pack("");
		public dynamic screenWidth = XVar.Pack(0);
		public dynamic screenHeight = XVar.Pack(0);
		public dynamic orientation = XVar.Pack("");
		public dynamic forListPageLookup = XVar.Pack(false);
		public dynamic lookupTable = XVar.Pack("");
		public dynamic lookupField = XVar.Pack("");
		public dynamic lookupPageType = XVar.Pack("");
		public dynamic parentCtrlsData;
		protected dynamic afterAddAction = XVar.Pack(null);
		public dynamic fromDashboard = XVar.Pack("");
		public dynamic forSpreadsheetGrid = XVar.Pack(false);
		public dynamic hostPageName = XVar.Pack("");
		public dynamic newRowId;
		public dynamic listPage = XVar.Pack("");
		protected dynamic sqlValues = XVar.Array();
		protected static bool skipAddPageCtor = false;
		public AddPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipAddPageCtor)
			{
				skipAddPageCtor = false;
				return;
			}
			this.addFields = XVar.Clone(this.getPageFields());
			this.auditObj = XVar.Clone(CommonFunctions.GetAuditObject((XVar)(this.tName)));
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
		public override XVar setSessionVariables()
		{
			base.setSessionVariables();
			if(XVar.Pack(!(XVar)(MVCFunctions.postvalue(new XVar("mastertable")))))
			{
				this.masterTable = new XVar("");
			}

			return null;
		}
		protected virtual XVar addPageSettings()
		{
			dynamic afterAddAction = null;
			if(XVar.Pack(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_recordAdded")]))
			{
				this.setProxyValue((XVar)(MVCFunctions.Concat(this.shortTableName, "_recordAdded")), new XVar(true));
				XSession.Session.Remove(MVCFunctions.Concat(this.sessionPrefix, "_recordAdded"));
			}
			else
			{
				this.setProxyValue((XVar)(MVCFunctions.Concat(this.shortTableName, "_recordAdded")), new XVar(false));
			}
			if((XVar)(this.mode != Constants.ADD_SIMPLE)  && (XVar)(this.mode != Constants.ADD_POPUP))
			{
				return null;
			}
			if((XVar)(this.listPage)  && (XVar)(afterAddAction == Constants.AA_TO_LIST))
			{
				this.pageData.InitAndSetArrayItem(this.listPage, "listPage");
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

			dynamic afterAddAction = null, settings = XVar.Array();
			settings = XVar.Clone(base.buildJsTableSettings((XVar)(table), (XVar)(pSet)));
			afterAddAction = XVar.Clone(this.getAfterAddAction());
			settings.InitAndSetArrayItem(afterAddAction, "afterAddAction");
			if((XVar)(afterAddAction == Constants.AA_TO_DETAIL_LIST)  || (XVar)(afterAddAction == Constants.AA_TO_DETAIL_ADD))
			{
				settings.InitAndSetArrayItem(CommonFunctions.GetTableURL((XVar)(this.pSet.getAADetailTable())), "afterAddActionDetTable");
			}
			settings.InitAndSetArrayItem(false, "isShowDetails");
			if((XVar)(this.showDetailsPreview())  && (XVar)((XVar)((XVar)((XVar)(this.mode == Constants.ADD_SIMPLE)  || (XVar)(this.mode == Constants.ADD_POPUP))  || (XVar)(this.mode == Constants.ADD_DASHBOARD))  || (XVar)(this.mode == Constants.ADD_MASTER_DASH)))
			{
				dynamic dpParams = XVar.Array();
				dpParams = XVar.Clone(this.getDetailsParams((XVar)(this.id)));
				settings.InitAndSetArrayItem(!(XVar)(!(XVar)(dpParams)), "isShowDetails");
				settings.InitAndSetArrayItem(new XVar("tableNames", dpParams["strTableNames"], "ids", dpParams["ids"]), "dpParams");
			}
			return settings;
		}
		protected virtual XVar getAfterAddAction()
		{
			dynamic action = null;
			if((XVar)(true)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.afterAddAction), XVar.Pack(null)))))
			{
				return this.afterAddAction;
			}
			action = XVar.Clone(this.pSet.getAfterAddAction());
			if((XVar)((XVar)((XVar)(this.mode == Constants.ADD_POPUP)  && (XVar)(this.pSet.checkClosePopupAfterAdd()))  || (XVar)((XVar)(action == Constants.AA_TO_VIEW)  && (XVar)(!(XVar)(this.viewAvailable()))))  || (XVar)((XVar)(action == Constants.AA_TO_EDIT)  && (XVar)(!(XVar)(this.editAvailable()))))
			{
				action = new XVar(Constants.AA_TO_LIST);
			}
			if((XVar)(action == Constants.AA_TO_DETAIL_LIST)  || (XVar)(action == Constants.AA_TO_DETAIL_ADD))
			{
				dynamic dPermissions = XVar.Array(), dPset = null, dTName = null, listPageAllowed = null;
				dTName = XVar.Clone(this.pSet.getAADetailTable());
				dPset = XVar.Clone(new ProjectSettings((XVar)(dTName)));
				dPermissions = XVar.Clone(this.getPermissions((XVar)(dTName)));
				listPageAllowed = XVar.Clone((XVar)(dPset.hasListPage())  && (XVar)(dPermissions["search"]));
				if((XVar)((XVar)(!(XVar)(dTName))  || (XVar)((XVar)(action == Constants.AA_TO_DETAIL_LIST)  && (XVar)(!(XVar)(listPageAllowed))))  || (XVar)((XVar)(action == Constants.AA_TO_DETAIL_ADD)  && (XVar)((XVar)(!(XVar)(dPset.hasAddPage()))  || (XVar)((XVar)(!(XVar)(dPermissions["add"]))  && (XVar)(!(XVar)(listPageAllowed))))))
				{
					action = new XVar(Constants.AA_TO_LIST);
				}
			}
			this.afterAddAction = XVar.Clone(action);
			return this.afterAddAction;
		}
		protected override XVar assignSessionPrefix()
		{
			if((XVar)((XVar)(this.mode == Constants.ADD_DASHBOARD)  || (XVar)(this.mode == Constants.ADD_MASTER_DASH))  || (XVar)((XVar)((XVar)((XVar)(this.mode == Constants.ADD_POPUP)  || (XVar)(this.mode == Constants.ADD_INLINE))  || (XVar)(this.fromDashboard != ""))  && (XVar)(this.dashTName)))
			{
				this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.dashTName, "_", this.tName));
				return null;
			}
			base.assignSessionPrefix();
			if(this.mode == Constants.ADD_ONTHEFLY)
			{
				this.sessionPrefix = MVCFunctions.Concat(this.sessionPrefix, "_add");
			}

			return null;
		}
		public override XVar setTemplateFile()
		{
			if(this.mode == Constants.ADD_INLINE)
			{
				this.templatefile = XVar.Clone(CommonFunctions.GetTemplateName((XVar)(this.shortTableName), new XVar("inline_add")));
			}
			base.setTemplateFile();

			return null;
		}
		protected override XVar getPageFields()
		{
			if(this.mode == Constants.ADD_INLINE)
			{
				if((XVar)((XVar)(this.masterTable)  && (XVar)(!(XVar)(this.inlineAddAvailable())))  && (XVar)(this.masterPageType == Constants.PAGE_ADD))
				{
					return this.pSet.getInlineAddFields();
				}
				return this.pSet.getInlineAddFields();
			}
			return this.pSet.getAddFields();
		}
		public static XVar handleBrokenRequest()
		{
			if((XVar)(MVCFunctions.POSTSize() != 0)  || (XVar)(!(XVar)(MVCFunctions.postvalue(new XVar("submit")))))
			{
				return null;
			}
			if(XVar.Pack(MVCFunctions.postvalue(new XVar("inline"))))
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
			if(XVar.Pack(MVCFunctions.postvalue(new XVar("fly"))))
			{
				MVCFunctions.Echo(-1);
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			XSession.Session["message_add"] = MVCFunctions.Concat("<< ", "Error occurred", " >>");

			return null;
		}
		public virtual XVar redirectAfterAdd()
		{
			dynamic aaData = XVar.Array();
			if(XVar.Pack(XSession.Session["after_add_data"]))
			{
				aaData = XSession.Session["after_add_data"];
			}
			else
			{
				aaData = XVar.Clone(XVar.Array());
			}
			if((XVar)(aaData.KeyExists(this.afterAdd_id))  && (XVar)(aaData[this.afterAdd_id]))
			{
				dynamic data = XVar.Array();
				data = XVar.Clone(aaData[this.afterAdd_id]);
				this.keys = XVar.Clone(data["keys"]);
				this.newRecordData = XVar.Clone(data["avalues"]);
				aaData.Remove(this.afterAdd_id);
				if(XVar.Pack(this.eventsObject.exists(new XVar("AfterAdd"))))
				{
					this.eventsObject.AfterAdd((XVar)(data["avalues"]), (XVar)(data["keys"]), (XVar)(data["inlineadd"]), this);
				}
			}
			foreach (KeyValuePair<XVar, dynamic> v in aaData.GetEnumerator())
			{
				if((XVar)(!(XVar)(MVCFunctions.is_array((XVar)(v.Value))))  || (XVar)(!(XVar)(v.Value.KeyExists("time"))))
				{
					aaData.Remove(v.Key);
					continue;
				}
				if(v.Value["time"] < MVCFunctions.time() - 3600)
				{
					aaData.Remove(v.Key);
				}
			}
			this.afterAddActionRedirect();

			return null;
		}
		public virtual XVar process()
		{
			if(XVar.Pack(MVCFunctions.strlen((XVar)(this.afterAdd_id))))
			{
				this.redirectAfterAdd();
				return null;
			}
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessAdd"))))
			{
				this.eventsObject.BeforeProcessAdd(this);
			}
			if(this.action == "added")
			{
				this.processDataInput();
				this.readAddValues = XVar.Clone(!(XVar)(this.insertedSuccessfully));
				if((XVar)((XVar)(this.mode != Constants.ADD_SIMPLE)  && (XVar)(this.mode != Constants.ADD_DASHBOARD))  && (XVar)(this.mode != Constants.ADD_MASTER_DASH))
				{
					this.reportSaveStatus();
					return null;
				}
				if(XVar.Pack(this.insertedSuccessfully))
				{
					if(XVar.Pack(this.afterAddActionRedirect()))
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
			this.prepareDefvalues();
			if(XVar.Pack(this.eventsObject.exists(new XVar("ProcessValuesAdd"))))
			{
				this.eventsObject.ProcessValuesAdd((XVar)(this.defvalues), this);
			}
			this.prepareReadonlyFields();
			this.prepareEditControls();
			this.prepareButtons();
			this.prepareSteps();
			if((XVar)(this.mode == Constants.ADD_SIMPLE)  || (XVar)(this.mode == Constants.ADD_ONTHEFLY))
			{
				this.addButtonHandlers();
			}
			this.addCommonJs();
			this.doCommonAssignments();
			this.prepareBreadcrumbs();
			this.prepareCollapseButton();
			this.prepareDetailsTables();
			this.displayAddPage();

			return null;
		}
		protected virtual XVar processDataInput()
		{
			if(this.action != "added")
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(CommonFunctions.isPostRequest())))
			{
				return null;
			}
			this.buildNewRecordData();
			if(XVar.Pack(!(XVar)(this.checkCaptcha())))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(this.recheckUserPermissions())))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(this.callBeforeAddEvent())))
			{
				return null;
			}
			this.setUpdatedLatLng((XVar)(this.newRecordData));
			if(XVar.Pack(!(XVar)(this.checkDeniedDuplicatedValues())))
			{
				return null;
			}
			if(XVar.Pack(this.callCustomAddEvent()))
			{
				dynamic insertResult = null;
				insertResult = XVar.Clone(this.dataSource.insertSingle((XVar)(this.getInsertDataCommand())));
				this.insertedSuccessfully = XVar.Clone(!XVar.Equals(XVar.Pack(insertResult), XVar.Pack(false)));
				if(XVar.Pack(!(XVar)(this.insertedSuccessfully)))
				{
					this.setDatabaseError((XVar)(this.dataSource.lastError()));
				}
				else
				{
					this.newRecordData = XVar.Clone(insertResult);
					foreach (KeyValuePair<XVar, dynamic> kf in this.pSet.getTableKeys().GetEnumerator())
					{
						if(XVar.Pack(this.newRecordData.KeyExists(kf.Value)))
						{
							this.keys.InitAndSetArrayItem(this.newRecordData[kf.Value], kf.Value);
						}
					}
					if(MVCFunctions.count(this.pSet.getTableKeys()) != MVCFunctions.count(this.keys))
					{
						this.keys = XVar.Clone(XVar.Array());
					}
				}
			}
			if(XVar.Pack(!(XVar)(this.insertedSuccessfully)))
			{
				return null;
			}
			if(this.getAfterAddAction() == Constants.AA_TO_ADD)
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_recordAdded")] = true;
			}
			this.ProcessFiles();
			if(XVar.Pack(this.auditObj))
			{
				this.auditObj.LogAdd((XVar)(this.tName), (XVar)(this.newRecordData), (XVar)(this.keys));
			}
			this.callAfterSuccessfulSave();
			this.callAfterAddEvent();
			this.messageType = new XVar(Constants.MESSAGE_INFO);
			this.setSuccessfulUpdateMessage();

			return null;
		}
		protected virtual XVar buildNewRecordData()
		{
			dynamic afilename_values = XVar.Array(), avalues = XVar.Array(), blobfields = null, listPSet = null;
			avalues = XVar.Clone(XVar.Array());
			blobfields = XVar.Clone(XVar.Array());
			afilename_values = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in this.addFields.GetEnumerator())
			{
				dynamic control = null;
				control = XVar.Clone(this.getControl((XVar)(f.Value), (XVar)(this.id)));
				control.readWebValue((XVar)(avalues), (XVar)(blobfields), new XVar(null), new XVar(null), (XVar)(afilename_values));
			}
			if(XVar.Pack(Security.advancedSecurityAvailable()))
			{
				dynamic securityType = null;
				securityType = XVar.Clone(this.pSet.getAdvancedSecurityType());
				if((XVar)(!(XVar)(this.isAdminTable()))  && (XVar)((XVar)(securityType == Constants.ADVSECURITY_EDIT_OWN)  || (XVar)(securityType == Constants.ADVSECURITY_VIEW_OWN)))
				{
					dynamic tableOwnerIdField = null;
					tableOwnerIdField = XVar.Clone(this.pSet.getTableOwnerIdField());
					if(XVar.Pack(this.checkIfToAddOwnerIdValue((XVar)(tableOwnerIdField), (XVar)(avalues[tableOwnerIdField]))))
					{
						avalues.InitAndSetArrayItem(CommonFunctions.prepare_for_db((XVar)(tableOwnerIdField), (XVar)(XSession.Session[MVCFunctions.Concat("_", this.tName, "_OwnerID")])), tableOwnerIdField);
					}
				}
			}
			foreach (KeyValuePair<XVar, dynamic> master in this.pSet.getMasterTables().GetEnumerator())
			{
				if(this.masterTable != master.Value["table"])
				{
					continue;
				}
				foreach (KeyValuePair<XVar, dynamic> dk in master.Value["detailsKeys"].GetEnumerator())
				{
					dynamic masterkeyIdx = null;
					masterkeyIdx = XVar.Clone(MVCFunctions.Concat("masterkey", dk.Key + 1));
					if(XVar.Pack(MVCFunctions.strlen((XVar)(MVCFunctions.postvalue((XVar)(masterkeyIdx))))))
					{
						XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_", masterkeyIdx)] = MVCFunctions.postvalue((XVar)(masterkeyIdx));
					}
					if((XVar)(!(XVar)(avalues.KeyExists(dk.Value)))  || (XVar)(avalues[dk.Value] == ""))
					{
						avalues.InitAndSetArrayItem(CommonFunctions.prepare_for_db((XVar)(dk.Value), (XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_", masterkeyIdx)])), dk.Value);
					}
				}
			}
			this.addLookupFilterFieldValue((XVar)(avalues), (XVar)(avalues));
			foreach (KeyValuePair<XVar, dynamic> value in afilename_values.GetEnumerator())
			{
				avalues.InitAndSetArrayItem(value.Value, value.Key);
			}
			listPSet = XVar.Clone(this.getListPSet());
			if(XVar.Pack(listPSet.reorderRows()))
			{
				dynamic order = null;
				order = XVar.Clone(MVCFunctions.postvalue(new XVar("order")));
				if(XVar.Pack(order))
				{
					order = XVar.Clone(this.getUniqueOrder((XVar)(listPSet), (XVar)(order)));
				}
				else
				{
					order = XVar.Clone(this.getMaxOrderValue((XVar)(listPSet)) + 1);
				}
				avalues.InitAndSetArrayItem(order, listPSet.reorderRowsField());
			}
			this.newRecordData = XVar.Clone(avalues);

			return null;
		}
		protected virtual XVar addLookupFilterFieldValue(dynamic _param_recordData, dynamic values)
		{
			#region pass-by-value parameters
			dynamic recordData = XVar.Clone(_param_recordData);
			#endregion

			dynamic lookupPSet = null;
			lookupPSet = XVar.Clone(this.pSet.getLookupMainTableSettings((XVar)(this.lookupTable), (XVar)(this.lookupField)));
			if(XVar.Pack(!(XVar)(lookupPSet)))
			{
				return null;
			}
			if(XVar.Pack(lookupPSet.useCategory((XVar)(this.lookupField))))
			{
				foreach (KeyValuePair<XVar, dynamic> cData in lookupPSet.getParentFieldsData((XVar)(this.lookupField)).GetEnumerator())
				{
					if((XVar)(this.parentCtrlsData.KeyExists(cData.Value["main"]))  && (XVar)(!(XVar)(recordData.KeyExists(cData.Value["lookup"]))))
					{
						values.InitAndSetArrayItem(this.parentCtrlsData[cData.Value["main"]], cData.Value["lookup"]);
					}
				}
			}

			return null;
		}
		public override XVar captchaExists()
		{
			if((XVar)(this.mode == Constants.ADD_ONTHEFLY)  || (XVar)(this.mode == Constants.ADD_INLINE))
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
			if(XVar.Pack(CommonFunctions.CheckTablePermissions((XVar)(this.tName), new XVar("A"))))
			{
				return true;
			}
			return base.recheckUserPermissions();
		}
		protected virtual XVar callBeforeAddEvent()
		{
			dynamic ret = null, usermessage = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("BeforeAdd")))))
			{
				return true;
			}
			this.sqlValues = XVar.Clone(XVar.Array());
			usermessage = new XVar("");
			ret = XVar.Clone(this.eventsObject.BeforeAdd((XVar)(this.newRecordData), (XVar)(this.sqlValues), ref usermessage, (XVar)(this.mode == Constants.ADD_INLINE), this));
			if(usermessage != XVar.Pack(""))
			{
				this.setMessage((XVar)(usermessage));
			}
			return ret;
		}
		public virtual XVar checkDeniedDuplicatedValues()
		{
			dynamic ret = null, usermessage = null;
			usermessage = new XVar("");
			ret = XVar.Clone(this.hasDeniedDuplicateValues((XVar)(this.newRecordData), ref usermessage));
			if(XVar.Pack(ret))
			{
				this.setMessage((XVar)(usermessage));
			}
			return !(XVar)(ret);
		}
		protected virtual XVar callCustomAddEvent()
		{
			dynamic customAddError = null, keyFields = XVar.Array(), keys = XVar.Array(), ret = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("CustomAdd")))))
			{
				return true;
			}
			keys = XVar.Clone(XVar.Array());
			customAddError = new XVar("");
			ret = XVar.Clone(this.eventsObject.CustomAdd((XVar)(this.newRecordData), (XVar)(keys), ref customAddError, (XVar)(this.mode == Constants.ADD_INLINE), this));
			if(0 < MVCFunctions.strlen((XVar)(customAddError)))
			{
				this.insertedSuccessfully = new XVar(false);
				this.setMessage((XVar)(customAddError));
				this.keys = XVar.Clone(XVar.Array());
				return false;
			}
			if(XVar.Pack(ret))
			{
				return true;
			}
			this.insertedSuccessfully = new XVar(true);
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			if((XVar)(!(XVar)(MVCFunctions.is_array((XVar)(keys))))  && (XVar)(MVCFunctions.count(keyFields) == 1))
			{
				keys = XVar.Clone(new XVar(keyFields[0], keys));
			}
			foreach (KeyValuePair<XVar, dynamic> kf in keyFields.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(keys[kf.Value]))))
				{
					this.keys.InitAndSetArrayItem(kf.Value, kf.Value);
				}
				else
				{
					if(XVar.Pack(this.newRecordData.KeyExists(kf.Value)))
					{
						this.keys.InitAndSetArrayItem(this.newRecordData[kf.Value], kf.Value);
					}
					else
					{
						this.keys.InitAndSetArrayItem(this.dataSource.lastAutoincValue((XVar)(kf.Value)), kf.Value);
					}
				}
				this.newRecordData.InitAndSetArrayItem(this.keys[kf.Value], kf.Value);
			}
			return false;
		}
		protected virtual XVar callAfterSuccessfulSave()
		{
			foreach (KeyValuePair<XVar, dynamic> f in this.addFields.GetEnumerator())
			{
				this.getControl((XVar)(f.Value), (XVar)(this.id)).afterSuccessfulSave();
			}

			return null;
		}
		protected virtual XVar callAfterAddEvent()
		{
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("AfterAdd")))))
			{
				return null;
			}
			if(this.mode != Constants.ADD_MASTER)
			{
				this.eventsObject.AfterAdd((XVar)(this.newRecordData), (XVar)(this.keys), (XVar)(this.mode == Constants.ADD_INLINE), this);
				return null;
			}
			this.afterAdd_id = XVar.Clone(CommonFunctions.generatePassword(new XVar(20)));
			if(XVar.Pack(!(XVar)(XSession.Session["after_add_data"])))
			{
				XSession.Session["after_add_data"] = XVar.Array();
			}
			XSession.Session.InitAndSetArrayItem(new XVar("avalues", this.newRecordData, "keys", this.keys, "inlineadd", this.mode == Constants.ADD_INLINE, "time", MVCFunctions.time()), "after_add_data", this.afterAdd_id);

			return null;
		}
		protected virtual XVar setSuccessfulUpdateMessage()
		{
			dynamic infoMessage = null, k = null, keyParams = XVar.Array(), keylink = null, keysArray = XVar.Array();
			if(XVar.Pack(this.isMessageSet()))
			{
				return null;
			}
			if(this.mode == Constants.ADD_INLINE)
			{
				infoMessage = XVar.Clone(MVCFunctions.Concat("", CommonFunctions.mlang_message(new XVar("RECORD_ADDED")), ""));
			}
			else
			{
				infoMessage = XVar.Clone(MVCFunctions.Concat("<strong><<< ", CommonFunctions.mlang_message(new XVar("RECORD_ADDED")), " >>></strong>"));
			}
			if((XVar)((XVar)(this.mode != Constants.ADD_SIMPLE)  && (XVar)(this.mode != Constants.ADD_MASTER))  || (XVar)(!(XVar)(this.keys)))
			{
				this.setMessage((XVar)(infoMessage));
				return null;
			}
			k = new XVar(0);
			keyParams = XVar.Clone(XVar.Array());
			keysArray = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> val in this.keys.GetEnumerator())
			{
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("editid", ++(k), "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(val.Value))))), null);
				keysArray.InitAndSetArrayItem(val.Value, null);
			}
			keylink = XVar.Clone(MVCFunctions.implode(new XVar("&"), (XVar)(keyParams)));
			if((XVar)(0 < MVCFunctions.count(keysArray))  && (XVar)(this.mode == Constants.ADD_SIMPLE))
			{
				XSession.Session["successKeys"] = keysArray;
			}
			else
			{
				infoMessage = MVCFunctions.Concat(infoMessage, "<br>");
				if(XVar.Pack(this.editAvailable()))
				{
					infoMessage = MVCFunctions.Concat(infoMessage, "&nbsp;<a href='", MVCFunctions.GetTableLink((XVar)(this.pSet.getShortTableName()), new XVar("edit"), (XVar)(keylink)), "'>", CommonFunctions.mlang_message(new XVar("EDIT")), "</a>&nbsp;");
				}
				if(XVar.Pack(this.viewAvailable()))
				{
					infoMessage = MVCFunctions.Concat(infoMessage, "&nbsp;<a href='", MVCFunctions.GetTableLink((XVar)(this.pSet.getShortTableName()), new XVar("view"), (XVar)(keylink)), "'>", CommonFunctions.mlang_message(new XVar("VIEW")), "</a>&nbsp;");
				}
			}
			this.setMessage((XVar)(infoMessage));

			return null;
		}
		protected virtual XVar reportSaveStatus()
		{
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(this.getSaveStatusJSON())));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar getSaveStatusJSON()
		{
			dynamic data = XVar.Array(), dmapIconsData = null, fieldsIconsData = null, haveData = null, isEditable = null, jsKeys = XVar.Array(), keyFields = XVar.Array(), keyParams = XVar.Array(), keylink = null, listPSet = null, listViewControls = null, returnJSON = XVar.Array(), showFields = XVar.Array(), showRawValues = XVar.Array(), showValues = XVar.Array();
			returnJSON = XVar.Clone(XVar.Array());
			if((XVar)(this.action != "added")  || (XVar)(this.mode == Constants.ADD_SIMPLE))
			{
				return returnJSON;
			}
			returnJSON.InitAndSetArrayItem(this.insertedSuccessfully, "success");
			returnJSON.InitAndSetArrayItem(this.message, "message");
			if(XVar.Pack(!(XVar)(this.isCaptchaOk)))
			{
				returnJSON.InitAndSetArrayItem(this.getCaptchaFieldName(), "wrongCaptchaFieldName");
			}
			else
			{
				if((XVar)((XVar)((XVar)(this.mode == Constants.ADD_POPUP)  || (XVar)(this.mode == Constants.ADD_MASTER))  || (XVar)(this.mode == Constants.ADD_MASTER_POPUP))  || (XVar)(this.mode == Constants.ADD_MASTER_DASH))
				{
					dynamic sessPrefix = null;
					sessPrefix = XVar.Clone(MVCFunctions.Concat(this.tName, "_", this.pageType));
					if((XVar)((XVar)(XSession.Session.KeyExists("count_passes_captcha"))  || (XVar)(0 < XSession.Session["count_passes_captcha"]))  || (XVar)(XSession.Session["count_passes_captcha"] < 5))
					{
						returnJSON.InitAndSetArrayItem(true, "hideCaptcha");
					}
				}
			}
			if(XVar.Pack(!(XVar)(this.insertedSuccessfully)))
			{
				return returnJSON;
			}
			jsKeys = XVar.Clone(XVar.Array());
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			if(XVar.Pack(this.keys))
			{
				foreach (KeyValuePair<XVar, dynamic> f in keyFields.GetEnumerator())
				{
					jsKeys.InitAndSetArrayItem(this.keys[f.Value], f.Key);
				}
			}
			if(this.mode == Constants.ADD_ONTHEFLY)
			{
				dynamic lokupData = XVar.Array();
				lokupData = XVar.Clone(this.getLookupData((XVar)(this.lookupTable), (XVar)(this.lookupField), (XVar)(this.lookupPageType), (XVar)(this.newRecordData)));
				returnJSON.InitAndSetArrayItem(lokupData["linkValue"], "linkValue");
				returnJSON.InitAndSetArrayItem(lokupData["displayValue"], "displayValue");
				returnJSON.InitAndSetArrayItem(lokupData["vals"], "vals");
				returnJSON.InitAndSetArrayItem(jsKeys, "keys");
				returnJSON.InitAndSetArrayItem(keyFields, "keyFields");
				return returnJSON;
			}
			data = XVar.Clone(XVar.Array());
			haveData = new XVar(true);
			if(XVar.Pack(this.keys))
			{
				data = XVar.Clone(this.getRecordData((XVar)(this.keys)));
			}
			if(XVar.Pack(!(XVar)(data)))
			{
				data = XVar.Clone(this.newRecordData);
				haveData = new XVar(false);
			}
			keyParams = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> kf in this.pSet.getTableKeys().GetEnumerator())
			{
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("key", kf.Key + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(data[kf.Value]))))), null);
			}
			keylink = XVar.Clone(MVCFunctions.Concat("&", MVCFunctions.implode(new XVar("&"), (XVar)(keyParams))));
			showValues = XVar.Clone(XVar.Array());
			showFields = XVar.Clone(XVar.Array());
			showRawValues = XVar.Clone(XVar.Array());
			listPSet = XVar.Clone(this.getListPSet());
			listViewControls = XVar.Clone(new ViewControlsContainer((XVar)(listPSet), (XVar)(this.pageType), this));
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getFieldsList().GetEnumerator())
			{
				dynamic control = null;
				control = XVar.Clone(listViewControls.getControl((XVar)(f.Value)));
				showValues.InitAndSetArrayItem(control.showDBValue((XVar)(data), (XVar)(keylink), new XVar(true)), f.Value);
				showFields.InitAndSetArrayItem(f.Value, null);
				if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(this.pSet.getFieldType((XVar)(f.Value))))))
				{
					showRawValues.InitAndSetArrayItem("", f.Value);
				}
				else
				{
					showRawValues.InitAndSetArrayItem(MVCFunctions.runner_substr((XVar)(data[f.Value]), new XVar(0), new XVar(100)), f.Value);
				}
			}
			if(XVar.Pack(listPSet.reorderRows()))
			{
				returnJSON.InitAndSetArrayItem(data[listPSet.reorderRowsField()], "order");
			}
			returnJSON.InitAndSetArrayItem(jsKeys, "keys");
			returnJSON.InitAndSetArrayItem(showValues, "vals");
			returnJSON.InitAndSetArrayItem(showFields, "fields");
			returnJSON.InitAndSetArrayItem(this.getShowDetailKeys((XVar)(data)), "detKeys");
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
			isEditable = new XVar(true);
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("IsRecordEditable"), (XVar)(this.tName))))
			{
				isEditable = XVar.Clone(GlobalVars.globalEvents.IsRecordEditable((XVar)(data), (XVar)(isEditable), (XVar)(this.tName)));
			}
			if(XVar.Pack(this.forSpreadsheetGrid))
			{
				if((XVar)(haveData)  && (XVar)(isEditable))
				{
					dynamic editPage = null, newRowId = null;
					newRowId = XVar.Clone((XVar.Pack(this.newRowId) ? XVar.Pack(this.newRowId) : XVar.Pack(this.id)));
					editPage = XVar.Clone(this.getRelatedInlineEditPage((XVar)(this.hostPageName), (XVar)(this.keys), (XVar)(newRowId)));
					returnJSON.InitAndSetArrayItem(listPSet.getInlineEditFields(), "editFields");
					returnJSON.InitAndSetArrayItem(XVar.Array(), "htmlControls");
					foreach (KeyValuePair<XVar, dynamic> fName in listPSet.getInlineEditFields().GetEnumerator())
					{
						dynamic controls = null;
						controls = XVar.Clone(editPage.getContolMapData((XVar)(fName.Value), (XVar)(newRowId), (XVar)(data), (XVar)(editPage.editFields)));
						editPage.fillControlsMap((XVar)(controls));
						if(editPage.getEditFormat((XVar)(fName.Value)) == Constants.EDIT_FORMAT_READONLY)
						{
							editPage.readOnlyFields.InitAndSetArrayItem(this.showDBValue((XVar)(fName.Value), (XVar)(data)), fName.Value);
						}
						returnJSON.InitAndSetArrayItem(editPage.getSpreadsheetControlMarkup((XVar)(fName.Value), (XVar)(newRowId), (XVar)(data)), "htmlControls", fName.Value);
					}
					returnJSON.InitAndSetArrayItem(newRowId, "pageId");
					editPage.fillSetCntrlMaps();
					returnJSON.InitAndSetArrayItem(editPage.controlsHTMLMap, "spreadControlsMap");
				}
				else
				{
					returnJSON.InitAndSetArrayItem(true, "nonEditable");
				}
			}
			if((XVar)((XVar)(this.mode == Constants.ADD_INLINE)  || (XVar)(this.mode == Constants.ADD_POPUP))  || (XVar)(this.mode == Constants.ADD_DASHBOARD))
			{
				returnJSON.InitAndSetArrayItem(!(XVar)(haveData), "noKeys");
				returnJSON.InitAndSetArrayItem(keyFields, "keyFields");
				returnJSON.InitAndSetArrayItem(showRawValues, "rawVals");
				returnJSON.InitAndSetArrayItem(this.buildDetailGridLinks((XVar)(returnJSON["detKeys"])), "hrefs");
				if(XVar.Pack(this.forListPageLookup))
				{
					dynamic linkAndDispVals = XVar.Array();
					linkAndDispVals = XVar.Clone(this.getLookupData((XVar)(this.lookupTable), (XVar)(this.lookupField), (XVar)(this.lookupPageType), (XVar)(this.newRecordData)));
					returnJSON.InitAndSetArrayItem(linkAndDispVals["linkValue"], "linkValue");
					returnJSON.InitAndSetArrayItem(linkAndDispVals["displayValue"], "displayValue");
				}
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("IsRecordEditable"), (XVar)(this.tName))))
				{
					if(XVar.Pack(!(XVar)(isEditable)))
					{
						returnJSON.InitAndSetArrayItem(true, "nonEditable");
					}
				}
				return returnJSON;
			}
			if((XVar)((XVar)(this.mode == Constants.ADD_MASTER)  || (XVar)(this.mode == Constants.ADD_MASTER_POPUP))  || (XVar)(this.mode == Constants.ADD_MASTER_DASH))
			{
				dynamic tData = null;
				XSession.Session["message_add"] = (XVar.Pack(this.message) ? XVar.Pack(this.message) : XVar.Pack(""));
				returnJSON.InitAndSetArrayItem(this.afterAdd_id, "afterAddId");
				tData = XVar.Clone(XVar.Array());
				returnJSON.InitAndSetArrayItem(this.getDetailTablesMasterKeys((XVar)(tData)), "mKeys");
				if((XVar)(this.mode == Constants.ADD_MASTER_POPUP)  || (XVar)(this.mode == Constants.ADD_MASTER_DASH))
				{
					returnJSON.InitAndSetArrayItem(!(XVar)(haveData), "noKeys");
					returnJSON.InitAndSetArrayItem(keyFields, "keyFields");
					returnJSON.InitAndSetArrayItem(showRawValues, "rawVals");
					returnJSON.InitAndSetArrayItem(this.buildDetailGridLinks((XVar)(returnJSON["detKeys"])), "hrefs");
					if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("IsRecordEditable"), (XVar)(this.tName))))
					{
						if(XVar.Pack(!(XVar)(isEditable)))
						{
							returnJSON.InitAndSetArrayItem(true, "nonEditable");
						}
					}
				}
				return returnJSON;
			}

			return null;
		}
		protected virtual XVar getShowDetailKeys(dynamic data)
		{
			dynamic showDetailKeys = XVar.Array();
			showDetailKeys = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> dt in this.pSet.getDetailsTables().GetEnumerator())
			{
				dynamic detailsKeys = XVar.Array();
				detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(dt.Value)));
				foreach (KeyValuePair<XVar, dynamic> dk in detailsKeys["masterKeys"].GetEnumerator())
				{
					showDetailKeys.InitAndSetArrayItem(data[dk.Value], dt.Value, MVCFunctions.Concat("masterkey", dk.Key + 1));
				}
			}
			if(this.getAfterAddAction() == Constants.AA_TO_DETAIL_ADD)
			{
				dynamic AAdTName = null, dTUrl = null;
				AAdTName = XVar.Clone(this.pSet.getAADetailTable());
				dTUrl = XVar.Clone(CommonFunctions.GetTableURL((XVar)(AAdTName)));
				if(XVar.Pack(!(XVar)(showDetailKeys.KeyExists(dTUrl))))
				{
					showDetailKeys.InitAndSetArrayItem(showDetailKeys[AAdTName], dTUrl);
				}
			}
			return showDetailKeys;
		}
		protected override XVar getDetailTablesMasterKeys(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic dpParams = XVar.Array(), i = null, mKeysData = XVar.Array();
			data = XVar.Clone(this.newRecordData);
			if(XVar.Pack(this.keys))
			{
				data = XVar.Clone(this.getRecordData((XVar)(this.keys)));
			}
			dpParams = XVar.Clone(this.getDetailsParams((XVar)(this.id)));
			if(XVar.Pack(!(XVar)(dpParams["ids"])))
			{
				return XVar.Array();
			}
			mKeysData = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(dpParams["ids"]); i++)
			{
				mKeysData.InitAndSetArrayItem(this.getMasterKeysData((XVar)(dpParams["strTableNames"][i]), (XVar)(data)), dpParams["strTableNames"][i]);
			}
			return mKeysData;
		}
		protected virtual XVar getMasterKeysData(dynamic _param_dTableName, dynamic data)
		{
			#region pass-by-value parameters
			dynamic dTableName = XVar.Clone(_param_dTableName);
			#endregion

			dynamic mKeyId = null, mKeys = XVar.Array(), mKeysData = XVar.Array();
			mKeyId = new XVar(1);
			mKeysData = XVar.Clone(XVar.Array());
			mKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(dTableName)));
			foreach (KeyValuePair<XVar, dynamic> mk in mKeys["masterKeys"].GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(data[mk.Value]))))
				{
					mKeysData.InitAndSetArrayItem(data[mk.Value], MVCFunctions.Concat("masterkey", mKeyId++));
				}
				else
				{
					mKeysData.InitAndSetArrayItem("", MVCFunctions.Concat("masterkey", mKeyId++));
				}
			}
			return mKeysData;
		}
		protected virtual XVar afterAddActionRedirect()
		{
			dynamic dTName = null, getParams = XVar.Array();
			if(this.mode != Constants.ADD_SIMPLE)
			{
				return false;
			}
			switch(((XVar)this.getAfterAddAction()).ToInt())
			{
				case Constants.AA_TO_ADD:
					if(XVar.Pack(this.insertedSuccessfully))
					{
						return this.prgRedirect();
					}
					getParams = XVar.Clone(XVar.Array());
					if(XVar.Pack(this.pageName))
					{
						getParams.InitAndSetArrayItem(MVCFunctions.Concat("page=", this.pageName), null);
					}
					getParams.InitAndSetArrayItem(this.getStateUrlParams(), null);
					MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), new XVar(Constants.PAGE_ADD), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(getParams))));
					return true;
				case Constants.AA_TO_LIST:
					if(XVar.Pack(this.pSet.hasListPage()))
					{
						MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), new XVar(Constants.PAGE_LIST), (XVar)(MVCFunctions.Concat("a=return&", (XVar.Pack(this.listPage) ? XVar.Pack(MVCFunctions.Concat("page=", this.listPage, "&")) : XVar.Pack("")), this.getStateUrlParams())));
					}
					else
					{
						MVCFunctions.HeaderRedirect(new XVar("menu"));
					}
					return true;
				case Constants.AA_TO_VIEW:
					MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), new XVar(Constants.PAGE_VIEW), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(new XVar(0, this.getKeyParams(), 1, this.getStateUrlParams())))));
					return true;
				case Constants.AA_TO_EDIT:
					MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), new XVar(Constants.PAGE_EDIT), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(new XVar(0, this.getKeyParams(), 1, this.getStateUrlParams())))));
					return true;
				case Constants.AA_TO_DETAIL_LIST:
					dTName = XVar.Clone(this.pSet.getAADetailTable());
					MVCFunctions.HeaderRedirect((XVar)(CommonFunctions.GetTableURL((XVar)(dTName))), new XVar(Constants.PAGE_LIST), (XVar)(MVCFunctions.Concat(MVCFunctions.implode(new XVar("&"), (XVar)(this.getNewRecordMasterKeys((XVar)(dTName)))), "&mastertable=", this.tName)));
					return true;
				case Constants.AA_TO_DETAIL_ADD:
					XSession.Session["message_add"] = (XVar.Pack(this.message) ? XVar.Pack(this.message) : XVar.Pack(""));
					dTName = XVar.Clone(this.pSet.getAADetailTable());
					MVCFunctions.HeaderRedirect((XVar)(CommonFunctions.GetTableURL((XVar)(dTName))), new XVar(Constants.PAGE_ADD), (XVar)(MVCFunctions.Concat(MVCFunctions.implode(new XVar("&"), (XVar)(this.getNewRecordMasterKeys((XVar)(dTName)))), "&mastertable=", this.tName)));
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
			data = XVar.Clone(this.getNewRecordData());
			mKeys = XVar.Clone(XVar.Array());
			detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(dTName)));
			foreach (KeyValuePair<XVar, dynamic> mk in detailsKeys["masterKeys"].GetEnumerator())
			{
				mKeys.InitAndSetArrayItem(MVCFunctions.Concat("masterkey", mk.Key + 1, "=", data[mk.Value]), null);
			}
			return mKeys;
		}
		protected virtual XVar prgRedirect()
		{
			dynamic getParams = XVar.Array();
			if(XVar.Pack(this.stopPRG))
			{
				return false;
			}
			if((XVar)((XVar)(!(XVar)(this.insertedSuccessfully))  || (XVar)(this.mode != Constants.ADD_SIMPLE))  || (XVar)(!(XVar)(MVCFunctions.no_output_done())))
			{
				return false;
			}
			XSession.Session["message_add"] = (XVar.Pack(this.message) ? XVar.Pack(this.message) : XVar.Pack(""));
			XSession.Session["message_add_type"] = this.messageType;
			getParams = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.pageName))
			{
				getParams.InitAndSetArrayItem(MVCFunctions.Concat("page=", this.pageName), null);
			}
			getParams.InitAndSetArrayItem(this.getStateUrlParams(), null);
			MVCFunctions.HeaderRedirect((XVar)(this.pSet.getShortTableName()), (XVar)(this.pageType), (XVar)(MVCFunctions.implode(new XVar("&"), (XVar)(getParams))));
			return true;
		}
		protected virtual XVar prgReadMessage()
		{
			if((XVar)(this.mode == Constants.ADD_SIMPLE)  && (XVar)(XSession.Session.KeyExists("message_add")))
			{
				this.message = XVar.Clone(XSession.Session["message_add"]);
				this.messageType = XVar.Clone(XSession.Session["message_add_type"]);
				XSession.Session.Remove("message_add");
			}

			return null;
		}
		public override XVar getCurrentRecord()
		{
			dynamic data = XVar.Array();
			data = XVar.Clone(XVar.Array());
			if((XVar)(this.masterTable)  && (XVar)(!(XVar)(!(XVar)(this.masterKeysReq))))
			{
				dynamic masterKeys = XVar.Array();
				masterKeys = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
				foreach (KeyValuePair<XVar, dynamic> detKey in masterKeys["detailsKeys"].GetEnumerator())
				{
					data.InitAndSetArrayItem(this.masterKeysReq[detKey.Key + 1], detKey.Value);
				}
			}
			return data;
		}
		protected virtual XVar replaceFileFieldsValuesWithCopies(dynamic defvalues)
		{
			foreach (KeyValuePair<XVar, dynamic> f in this.addFields.GetEnumerator())
			{
				if(this.pSet.getEditFormat((XVar)(f.Value)) == Constants.EDIT_FORMAT_FILE)
				{
					defvalues.InitAndSetArrayItem(this.getControl((XVar)(f.Value), (XVar)(this.id)).getFieldValueCopy((XVar)(defvalues[f.Value])), f.Value);
				}
			}

			return null;
		}
		protected virtual XVar getCopyKeys()
		{
			dynamic copyKeys = XVar.Array(), prefix = null;
			copyKeys = XVar.Clone(XVar.Array());
			if((XVar)(!(XVar)(MVCFunctions.REQUESTKeyExists("copyid1")))  && (XVar)(!(XVar)(MVCFunctions.REQUESTKeyExists("editid1"))))
			{
				return copyKeys;
			}
			prefix = XVar.Clone((XVar.Pack(MVCFunctions.REQUESTKeyExists("copyid1")) ? XVar.Pack("copyid") : XVar.Pack("editid")));
			foreach (KeyValuePair<XVar, dynamic> k in this.pSet.getTableKeys().GetEnumerator())
			{
				copyKeys.InitAndSetArrayItem(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat(prefix, k.Key + 1))), k.Value);
			}
			return copyKeys;
		}
		protected virtual XVar prepareDefvalues()
		{
			dynamic copyKeys = null;
			copyKeys = XVar.Clone(this.getCopyKeys());
			if((XVar)(copyKeys)  && (XVar)(this.mode != Constants.ADD_DASHBOARD))
			{
				dynamic dc = null, fetchedArray = null, keyWhere = null, prep = XVar.Array();
				dc = XVar.Clone(this.getDsCommand((XVar)(copyKeys)));
				prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
				keyWhere = XVar.Clone(prep["where"]);
				fetchedArray = XVar.Clone(this.dataSource.getSingle((XVar)(dc)).fetchAssoc());
				this.defvalues = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
				this.replaceFileFieldsValuesWithCopies((XVar)(this.defvalues));
				if(XVar.Pack(this.eventsObject.exists(new XVar("CopyOnLoad"))))
				{
					this.eventsObject.CopyOnLoad((XVar)(this.defvalues), (XVar)(keyWhere), this);
					if(keyWhere != prep["where"])
					{
						this.dataSource.overrideWhere((XVar)(dc), (XVar)(keyWhere));
						fetchedArray = XVar.Clone(this.dataSource.getSingle((XVar)(dc)).fetchAssoc());
						this.defvalues = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
						this.replaceFileFieldsValuesWithCopies((XVar)(this.defvalues));
					}
				}
			}
			else
			{
				foreach (KeyValuePair<XVar, dynamic> f in this.addFields.GetEnumerator())
				{
					dynamic defaultValue = null;
					defaultValue = XVar.Clone(this.pSet.getDefaultValue((XVar)(f.Value)));
					if(XVar.Pack(MVCFunctions.strlen((XVar)(defaultValue))))
					{
						this.defvalues.InitAndSetArrayItem(defaultValue, f.Value);
					}
				}
			}
			if(XVar.Pack(Security.advancedSecurityAvailable()))
			{
				dynamic securityType = null;
				securityType = XVar.Clone(this.pSet.getAdvancedSecurityType());
				if((XVar)(!(XVar)(this.isAdminTable()))  && (XVar)((XVar)(securityType == Constants.ADVSECURITY_EDIT_OWN)  || (XVar)(securityType == Constants.ADVSECURITY_VIEW_OWN)))
				{
					dynamic tableOwnerIdField = null;
					tableOwnerIdField = XVar.Clone(this.pSet.getTableOwnerIdField());
					if(XVar.Pack(this.checkIfToAddOwnerIdValue((XVar)(tableOwnerIdField), new XVar(""))))
					{
						this.defvalues.InitAndSetArrayItem(CommonFunctions.prepare_for_db((XVar)(tableOwnerIdField), (XVar)(XSession.Session[MVCFunctions.Concat("_", this.tName, "_OwnerID")])), tableOwnerIdField);
					}
				}
			}
			foreach (KeyValuePair<XVar, dynamic> master in this.pSet.getMasterTables().GetEnumerator())
			{
				if(this.masterTable != master.Value["table"])
				{
					continue;
				}
				foreach (KeyValuePair<XVar, dynamic> dk in master.Value["detailsKeys"].GetEnumerator())
				{
					dynamic masterkeyIdx = null;
					masterkeyIdx = XVar.Clone(MVCFunctions.Concat("masterkey", dk.Key + 1));
					if(XVar.Pack(MVCFunctions.strlen((XVar)(MVCFunctions.postvalue((XVar)(masterkeyIdx))))))
					{
						XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_", masterkeyIdx)] = MVCFunctions.postvalue((XVar)(masterkeyIdx));
					}
					if(this.masterPageType != Constants.PAGE_ADD)
					{
						this.defvalues.InitAndSetArrayItem(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_", masterkeyIdx)], dk.Value);
					}
				}
			}
			this.addLookupFilterFieldValue((XVar)(this.newRecordData), (XVar)(this.defvalues));
			if(XVar.Pack(this.readAddValues))
			{
				foreach (KeyValuePair<XVar, dynamic> fName in this.addFields.GetEnumerator())
				{
					dynamic editFormat = null;
					editFormat = XVar.Clone(this.pSet.getEditFormat((XVar)(fName.Value)));
					if((XVar)((XVar)(editFormat != Constants.EDIT_FORMAT_DATABASE_FILE)  && (XVar)(editFormat != Constants.EDIT_FORMAT_DATABASE_IMAGE))  && (XVar)(editFormat != Constants.EDIT_FORMAT_FILE))
					{
						this.defvalues.InitAndSetArrayItem(this.newRecordData[fName.Value], fName.Value);
					}
				}
			}

			return null;
		}
		protected virtual XVar prepareReadonlyFields()
		{
			foreach (KeyValuePair<XVar, dynamic> f in this.addFields.GetEnumerator())
			{
				if(this.pSet.getEditFormat((XVar)(f.Value)) == Constants.EDIT_FORMAT_READONLY)
				{
					this.readOnlyFields.InitAndSetArrayItem(this.showDBValue((XVar)(f.Value), (XVar)(this.defvalues)), f.Value);
				}
			}

			return null;
		}
		protected virtual XVar prepareButtons()
		{
			dynamic addStyle = null;
			if(this.mode == Constants.ADD_INLINE)
			{
				return null;
			}
			this.xt.assign(new XVar("save_button"), new XVar(true));
			addStyle = new XVar("");
			if(XVar.Pack(this.isMultistepped()))
			{
				addStyle = new XVar(" style=\"display: none;\"");
			}
			this.xt.assign(new XVar("savebutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"saveButton", this.id, "\"", addStyle)));
			if(this.mode == Constants.ADD_DASHBOARD)
			{
				this.xt.assign(new XVar("reset_button"), new XVar(true));
				return null;
			}
			if((XVar)((XVar)(this.mode != Constants.ADD_ONTHEFLY)  && (XVar)(this.mode != Constants.ADD_POPUP))  && (XVar)(this.mode != Constants.ADD_MASTER_DASH))
			{
				if(XVar.Pack(XSession.Session.KeyExists("successKeys")))
				{
					this.xt.assign(new XVar("message_back_button"), new XVar(true));
				}
				if(XVar.Pack(this.pSet.hasListPage()))
				{
					this.xt.assign(new XVar("back_button"), new XVar(true));
				}
				else
				{
					if(XVar.Pack(this.isShowMenu()))
					{
						this.xt.assign(new XVar("backToMenu_button"), new XVar(true));
					}
				}
			}
			else
			{
				this.xt.assign(new XVar("cancel_button"), new XVar(true));
			}
			if(this.mode == Constants.ADD_SIMPLE)
			{
				if(XVar.Pack(this.pSet.hasListPage()))
				{
					this.xt.assign(new XVar("backbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"backButton", this.id, "\"")));
				}
				else
				{
					if(XVar.Pack(this.isShowMenu()))
					{
						this.xt.assign(new XVar("backbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"backToMenuButton", this.id, "\"")));
					}
				}
			}
			if(XVar.Pack(XSession.Session.KeyExists("successKeys")))
			{
				dynamic dataKeysAttr = null, keysArray = null;
				keysArray = XVar.Clone(XSession.Session["successKeys"]);
				dataKeysAttr = XVar.Clone(MVCFunctions.Concat("data-keys=\"", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.runner_json_encode((XVar)(keysArray)))), "\""));
				XSession.Session.Remove("successKeys");
				if((XVar)(this.viewAvailable())  && (XVar)(keysArray))
				{
					this.xt.assign(new XVar("view_page_button"), new XVar(true));
					this.xt.assign(new XVar("view_page_button_attrs"), (XVar)(MVCFunctions.Concat("id=\"viewPageButton", this.id, "\" ", dataKeysAttr)));
				}
				if((XVar)(this.editAvailable())  && (XVar)(keysArray))
				{
					this.xt.assign(new XVar("edit_page_button"), new XVar(true));
					this.xt.assign(new XVar("edit_page_button_attrs"), (XVar)(MVCFunctions.Concat("id=\"editPageButton", this.id, "\" ", dataKeysAttr)));
				}
			}

			return null;
		}
		protected virtual XVar prepareEditControls()
		{
			dynamic controlFields = XVar.Array();
			controlFields = XVar.Clone(this.addFields);
			foreach (KeyValuePair<XVar, dynamic> fName in controlFields.GetEnumerator())
			{
				dynamic controls = XVar.Array(), firstElementId = null, gf = null, parameters = XVar.Array();
				gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(fName.Value)));
				if(XVar.Pack(this.detailsKeyField((XVar)(fName.Value))))
				{
					this.readOnlyFields.InitAndSetArrayItem(this.showDBValue((XVar)(fName.Value), (XVar)(this.defvalues)), fName.Value);
				}
				firstElementId = XVar.Clone(this.getControl((XVar)(fName.Value), (XVar)(this.id)).getFirstElementId());
				if(XVar.Pack(firstElementId))
				{
					this.xt.assign((XVar)(MVCFunctions.Concat("labelfor_", MVCFunctions.GoodFieldName((XVar)(fName.Value)))), (XVar)(firstElementId));
				}
				parameters = XVar.Clone(this.getEditContolParams((XVar)(fName.Value), (XVar)(this.id), (XVar)(this.defvalues)));
				if(this.pSet.getEditFormat((XVar)(fName.Value)) == Constants.EDIT_FORMAT_CHECKBOX)
				{
					parameters.InitAndSetArrayItem(this.xt, "xt");
					parameters.InitAndSetArrayItem(MVCFunctions.Concat(gf, "_forward_control"), "clearVar");
				}
				this.xt.assign_function((XVar)(MVCFunctions.Concat(gf, "_editcontrol")), new XVar("xt_buildeditcontrol"), (XVar)(parameters));
				if(this.pSet.getEditFormat((XVar)(fName.Value)) == Constants.EDIT_FORMAT_CHECKBOX)
				{
					parameters.InitAndSetArrayItem(this.xt, "xt");
					parameters.InitAndSetArrayItem(MVCFunctions.Concat(gf, "_editcontrol"), "clearVar");
					this.xt.assign_function((XVar)(MVCFunctions.Concat(gf, "_forward_control")), new XVar("xt_buildforwardcontrol"), (XVar)(parameters));
					this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_label_class")), new XVar("r-checkbox-label"));
				}
				controls = XVar.Clone(this.getContolMapData((XVar)(fName.Value), (XVar)(this.id), (XVar)(this.defvalues), (XVar)(controlFields)));
				if(XVar.Pack(MVCFunctions.in_array((XVar)(fName.Value), (XVar)(this.errorFields))))
				{
					controls.InitAndSetArrayItem(true, "controls", "isInvalid");
				}
				this.fillControlsMap((XVar)(controls));
				this.fillControlFlags((XVar)(fName.Value));
			}

			return null;
		}
		public override XVar getContolMapData(dynamic _param_fName, dynamic _param_recId, dynamic data, dynamic _param_pageFields)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			dynamic pageFields = XVar.Clone(_param_pageFields);
			#endregion

			dynamic controls = XVar.Array(), preload = XVar.Array();
			controls = XVar.Clone(XVar.Array());
			controls.InitAndSetArrayItem(XVar.Array(), "controls");
			controls.InitAndSetArrayItem(recId, "controls", "id");
			controls.InitAndSetArrayItem(0, "controls", "ctrlInd");
			controls.InitAndSetArrayItem(fName, "controls", "fieldName");
			if(XVar.Pack(this.pSet.isUseRTE((XVar)(fName))))
			{
				controls.InitAndSetArrayItem("add", "controls", "mode");
			}
			else
			{
				controls.InitAndSetArrayItem((XVar.Pack(this.mode == Constants.ADD_INLINE) ? XVar.Pack("inline_add") : XVar.Pack("add")), "controls", "mode");
			}
			if(XVar.Pack(this.detailsKeyField((XVar)(fName))))
			{
				controls.InitAndSetArrayItem(data[fName], "controls", "value");
			}
			preload = XVar.Clone(this.fillPreload((XVar)(fName), (XVar)(pageFields), (XVar)(data)));
			if(!XVar.Equals(XVar.Pack(preload), XVar.Pack(false)))
			{
				controls.InitAndSetArrayItem(preload, "controls", "preloadData");
				if((XVar)(!(XVar)(data[fName]))  && (XVar)(preload["vals"]))
				{
					data.InitAndSetArrayItem(preload["vals"][0], fName);
				}
			}
			return controls;
		}
		public override XVar getEditContolParams(dynamic _param_fName, dynamic _param_recId, dynamic data)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic recId = XVar.Clone(_param_recId);
			#endregion

			dynamic parameters = XVar.Array();
			parameters = XVar.Clone(XVar.Array());
			parameters.InitAndSetArrayItem(recId, "id");
			parameters.InitAndSetArrayItem(Constants.PAGE_ADD, "ptype");
			parameters.InitAndSetArrayItem(fName, "field");
			parameters.InitAndSetArrayItem(data[fName], "value");
			parameters.InitAndSetArrayItem(this, "pageObj");
			if(!XVar.Equals(XVar.Pack(this.getEditFormat((XVar)(fName))), XVar.Pack(Constants.EDIT_FORMAT_READONLY)))
			{
				parameters.InitAndSetArrayItem(this.pSet.getValidationData((XVar)(fName)), "validate");
				if(XVar.Pack(this.pSet.isUseRTE((XVar)(fName))))
				{
					XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_", fName, "_rte")] = data[fName];
				}
			}
			if(XVar.Pack(this.pSet.isUseRTE((XVar)(fName))))
			{
				parameters.InitAndSetArrayItem("add", "mode");
			}
			else
			{
				parameters.InitAndSetArrayItem((XVar.Pack(this.mode == Constants.ADD_INLINE) ? XVar.Pack("inline_add") : XVar.Pack("add")), "mode");
			}
			return parameters;
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
		protected virtual XVar prepareDetailsTables()
		{
			dynamic d = null, dpParams = XVar.Array();
			if((XVar)(!(XVar)(this.showDetailsPreview()))  || (XVar)((XVar)((XVar)((XVar)(this.mode != Constants.ADD_SIMPLE)  && (XVar)(this.mode != Constants.ADD_POPUP))  && (XVar)(this.mode != Constants.ADD_DASHBOARD))  && (XVar)(this.mode != Constants.ADD_MASTER_DASH)))
			{
				return null;
			}
			dpParams = XVar.Clone(this.getDetailsParams((XVar)(this.id)));
			if(XVar.Pack(!(XVar)(dpParams["ids"])))
			{
				return null;
			}
			this.xt.assign(new XVar("detail_tables"), new XVar(true));
			d = new XVar(0);
			for(;d < MVCFunctions.count(dpParams["ids"]); d++)
			{
				this.setDetailPreview(new XVar("list"), (XVar)(dpParams["strTableNames"][d]), (XVar)(dpParams["ids"][d]), (XVar)(this.defvalues));
				this.displayDetailsButtons((XVar)(dpParams["type"][d]), (XVar)(dpParams["strTableNames"][d]), (XVar)(dpParams["ids"][d]));
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
			listPageObject = XVar.Clone(this.getDetailsPageObject((XVar)(dpTableName), (XVar)(dpId)));
			listPageObject.assignButtonsOnMasterEdit((XVar)(this.xt));

			return null;
		}
		protected virtual XVar doCommonAssignments()
		{
			if(XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.ADD_SIMPLE)))
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
			if(this.mode != Constants.ADD_INLINE)
			{
				this.assignAddFieldsBlocksAndLabels();
			}
			if(this.mode == Constants.ADD_SIMPLE)
			{
				this.assignBody();
				this.xt.assign(new XVar("flybody"), new XVar(true));
			}
			if((XVar)((XVar)((XVar)(this.mode == Constants.ADD_ONTHEFLY)  || (XVar)(this.mode == Constants.ADD_POPUP))  || (XVar)(this.mode == Constants.ADD_DASHBOARD))  || (XVar)(this.mode == Constants.ADD_MASTER_DASH))
			{
				this.xt.assign(new XVar("body"), new XVar(true));
				this.xt.assign(new XVar("footer"), new XVar(false));
				this.xt.assign(new XVar("header"), new XVar(false));
				this.xt.assign(new XVar("flybody"), (XVar)(this.body));
			}

			return null;
		}
		public virtual XVar assignAddFieldsBlocksAndLabels()
		{
			foreach (KeyValuePair<XVar, dynamic> fName in this.addFields.GetEnumerator())
			{
				dynamic gfName = null;
				gfName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(fName.Value)));
				this.xt.assign((XVar)(MVCFunctions.Concat(gfName, "_fieldblock")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat(gfName, "_tabfieldblock")), new XVar(true));
			}

			return null;
		}
		protected virtual XVar displayAddPage()
		{
			dynamic templatefile = null;
			templatefile = XVar.Clone(this.templatefile);
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowAdd"))))
			{
				this.eventsObject.BeforeShowAdd((XVar)(this.xt), ref templatefile, this);
			}
			if((XVar)(this.mode != Constants.ADD_INLINE)  && (XVar)(this.mode != Constants.ADD_ONTHEFLY))
			{
				this.displayMasterTableInfo();
			}
			this.fillSetCntrlMaps();
			if(this.mode == Constants.ADD_SIMPLE)
			{
				this.display((XVar)(templatefile));
				return null;
			}
			if((XVar)((XVar)((XVar)(this.mode == Constants.ADD_ONTHEFLY)  || (XVar)(this.mode == Constants.ADD_POPUP))  || (XVar)(this.mode == Constants.ADD_DASHBOARD))  || (XVar)(this.mode == Constants.ADD_MASTER_DASH))
			{
				this.displayAJAX((XVar)(templatefile), (XVar)(this.flyId + 1));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			if(this.mode == Constants.ADD_INLINE)
			{
				dynamic listPSet = null, returnJSON = XVar.Array();
				returnJSON = XVar.Clone(XVar.Array());
				this.xt.load_template((XVar)(templatefile));
				returnJSON.InitAndSetArrayItem(XVar.Array(), "htmlControls");
				foreach (KeyValuePair<XVar, dynamic> fName in this.addFields.GetEnumerator())
				{
					returnJSON.InitAndSetArrayItem(this.xt.fetchVar((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(fName.Value)), "_editcontrol"))), "htmlControls", fName.Value);
				}
				listPSet = XVar.Clone(this.getListPSet());
				if(XVar.Pack(listPSet.reorderRows()))
				{
					returnJSON.InitAndSetArrayItem(this.getMaxOrderValue((XVar)(listPSet)) + 1, "order");
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
		protected override XVar getExtraAjaxPageParams()
		{
			return this.getSaveStatusJSON();
		}
		public virtual XVar checkIfToAddOwnerIdValue(dynamic _param_ownerField, dynamic _param_currentValue)
		{
			#region pass-by-value parameters
			dynamic ownerField = XVar.Clone(_param_ownerField);
			dynamic currentValue = XVar.Clone(_param_currentValue);
			#endregion

			return (XVar)((XVar)(CommonFunctions.originalTableField((XVar)(ownerField), (XVar)(this.pSet)))  && (XVar)(!(XVar)(this.isAutoincPrimaryKey((XVar)(ownerField)))))  && (XVar)((XVar)(!(XVar)(CommonFunctions.CheckTablePermissions((XVar)(this.tName), new XVar("M"))))  || (XVar)(!(XVar)(MVCFunctions.strlen((XVar)(currentValue)))));
		}
		protected virtual XVar isAutoincPrimaryKey(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic keyFields = null;
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			return (XVar)((XVar)(MVCFunctions.count(keyFields) == 1)  && (XVar)(MVCFunctions.in_array((XVar)(field), (XVar)(keyFields))))  && (XVar)(this.pSet.isAutoincField((XVar)(field)));
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

			if(this.mode != Constants.ADD_INLINE)
			{
				this.message = XVar.Clone(MVCFunctions.Concat("<strong>&lt;&lt;&lt; ", CommonFunctions.mlang_message(new XVar("RECORD_NOT_ADDED")), "</strong> &gt;&gt;&gt;<br><br>", message));
			}
			else
			{
				this.message = XVar.Clone(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("RECORD_NOT_ADDED")), ". ", message));
			}
			this.messageType = new XVar(Constants.MESSAGE_ERROR);

			return null;
		}
		public virtual XVar getNewRecordData()
		{
			return this.newRecordData;
		}
		protected override XVar checkFieldOnPage(dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			if(this.mode == Constants.ADD_INLINE)
			{
				return this.pSet.appearOnInlineAdd((XVar)(fName));
			}
			return this.pSet.appearOnAddPage((XVar)(fName));
		}
		public static XVar processAddPageSecurity(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic pageMode = null;
			if(XVar.Pack(Security.checkPagePermissions((XVar)(table), new XVar("A"))))
			{
				return true;
			}
			if(MVCFunctions.postvalue(new XVar("a")) == "added")
			{
				return true;
			}
			pageMode = XVar.Clone(AddPage.readAddModeFromRequest());
			if(pageMode != Constants.ADD_SIMPLE)
			{
				Security.sendPermissionError();
				return false;
			}
			if((XVar)(CommonFunctions.isLogged())  && (XVar)(!(XVar)(Security.isGuest())))
			{
				Security.redirectToList((XVar)(table));
				return false;
			}
			CommonFunctions.redirectToLogin();
			return false;
		}
		public static XVar readAddModeFromRequest()
		{
			dynamic editType = null;
			editType = XVar.Clone(MVCFunctions.postvalue(new XVar("editType")));
			if(editType == "inline")
			{
				return Constants.ADD_INLINE;
			}
			else
			{
				if(editType == Constants.ADD_POPUP)
				{
					return Constants.ADD_POPUP;
				}
				else
				{
					if(editType == Constants.ADD_MASTER)
					{
						return Constants.ADD_MASTER;
					}
					else
					{
						if(editType == Constants.ADD_MASTER_POPUP)
						{
							return Constants.ADD_MASTER_POPUP;
						}
						else
						{
							if(editType == Constants.ADD_MASTER_DASH)
							{
								return Constants.ADD_MASTER_DASH;
							}
							else
							{
								if(editType == Constants.ADD_ONTHEFLY)
								{
									return Constants.ADD_ONTHEFLY;
								}
								else
								{
									if(MVCFunctions.postvalue(new XVar("mode")) == "dashrecord")
									{
										return Constants.ADD_DASHBOARD;
									}
									else
									{
										return Constants.ADD_SIMPLE;
									}
								}
							}
						}
					}
				}
			}

			return null;
		}
		public override XVar editAvailable()
		{
			return (XVar)(!(XVar)(this.dashElementData))  && (XVar)(base.editAvailable());
		}
		public override XVar viewAvailable()
		{
			return (XVar)(!(XVar)(this.dashElementData))  && (XVar)(base.viewAvailable());
		}
		public virtual XVar setMessageType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			this.messageType = XVar.Clone(var_type);

			return null;
		}
		public override XVar element2Item(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(name == "message")
			{
				return new XVar(0, "add_message");
			}
			return base.element2Item((XVar)(name));
		}
		protected override XVar checkShowBreadcrumbs()
		{
			return this.mode == Constants.ADD_SIMPLE;
		}
		public override XVar createProjectSettings()
		{
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), (XVar)(this.pageName), (XVar)(this.pageTable)));
			if((XVar)(this.mode != Constants.ADD_INLINE)  && (XVar)(!XVar.Equals(XVar.Pack(this.pSet.getPageType()), XVar.Pack(Constants.PAGE_ADD))))
			{
				this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), new XVar(null), (XVar)(this.pageTable)));
			}
			if((XVar)(this.mode == Constants.ADD_POPUP)  && (XVar)(this.action == "added"))
			{
				this.pSet.setPageType(new XVar("list"));
			}

			return null;
		}
		public virtual XVar getInsertDataCommand()
		{
			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.values = this.newRecordData;
			dc.advValues = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> sqlValue in this.sqlValues.GetEnumerator())
			{
				dc.advValues.InitAndSetArrayItem(new DsOperand(new XVar(Constants.dsotSQL), (XVar)(sqlValue.Value)), sqlValue.Key);
			}
			return dc;
		}
		public override XVar getSecurityCondition()
		{
			return Security.SelectCondition(new XVar("S"), (XVar)(this.pSet));
		}
		protected virtual XVar getRecordData(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic dc = null, fetchedArray = null;
			dc = XVar.Clone(this.getDsCommand((XVar)(keys)));
			fetchedArray = XVar.Clone(this.dataSource.getSingle((XVar)(dc)).fetchAssoc());
			return this.cipherer.DecryptFetchedArray((XVar)(fetchedArray));
		}
		protected virtual XVar getDsCommand(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.keys = XVar.Clone(keys);
			dc.filter = XVar.Clone(this.getSecurityCondition());
			return dc;
		}
		protected override XVar getListPSet()
		{
			if(XVar.Pack(!(XVar)(this.listPagePSet)))
			{
				this.listPagePSet = XVar.Clone(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_LIST), (XVar)(this.hostPageName), (XVar)(this.pageTable)));
			}
			return this.listPagePSet;
		}
	}
}
