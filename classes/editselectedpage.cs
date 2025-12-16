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
	public partial class EditSelectedPage : EditPage
	{
		public dynamic rowIds = XVar.Array();
		public dynamic parsedSelection = XVar.Array();
		public dynamic updSelectedFields = XVar.Pack(null);
		public dynamic selectedFields = XVar.Pack(null);
		public dynamic nSelected = XVar.Pack(0);
		public dynamic nUpdated = XVar.Pack(0);
		public dynamic recordBeingUpdated;
		public dynamic currentWhereExpr;
		public dynamic recordCount = XVar.Pack(0);
		public dynamic messages = XVar.Array();
		protected dynamic inlineReportData = XVar.Array();
		protected static bool skipEditSelectedPageCtor = false;
		public EditSelectedPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipEditSelectedPageCtor)
			{
				skipEditSelectedPageCtor = false;
				return;
			}
			dynamic keyFields = XVar.Array();
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			foreach (KeyValuePair<XVar, dynamic> s in this.selection.GetEnumerator())
			{
				dynamic arr = XVar.Array(), parsed = XVar.Array();
				arr = XVar.Clone(MVCFunctions.explode(new XVar("&"), (XVar)(s.Value)));
				if(MVCFunctions.count(arr) != MVCFunctions.count(this.pSet.getTableKeys()))
				{
					continue;
				}
				foreach (KeyValuePair<XVar, dynamic> v in arr.GetEnumerator())
				{
					parsed.InitAndSetArrayItem(MVCFunctions.RawUrlDecode((XVar)(v.Value)), keyFields[v.Key]);
				}
				this.parsedSelection.InitAndSetArrayItem(parsed, null);
			}
		}
		protected override XVar getPageFields()
		{
			if(XVar.Equals(XVar.Pack(this.updSelectedFields), XVar.Pack(null)))
			{
				dynamic denyDuplicateFields = XVar.Array(), updateFields = XVar.Array();
				this.updSelectedFields = XVar.Clone(MVCFunctions.array_diff((XVar)(this.pSet.getUpdateSelectedFields()), (XVar)(this.pSet.getTableKeys())));
				denyDuplicateFields = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> f in this.updSelectedFields.GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(this.pSet.allowDuplicateValues((XVar)(f.Value)))))
					{
						denyDuplicateFields.InitAndSetArrayItem(f.Value, null);
					}
				}
				this.updSelectedFields = XVar.Clone(MVCFunctions.array_diff((XVar)(this.updSelectedFields), (XVar)(denyDuplicateFields)));
				updateFields = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> f in this.updSelectedFields.GetEnumerator())
				{
					dynamic editFormat = null;
					editFormat = XVar.Clone(this.pSet.getEditFormat((XVar)(f.Value)));
					if(editFormat != Constants.EDIT_FORMAT_FILE)
					{
						updateFields.InitAndSetArrayItem(f.Value, null);
					}
				}
				this.updSelectedFields = XVar.Clone(updateFields);
			}
			return this.updSelectedFields;
		}
		public override XVar setKeys(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			this.keys = XVar.Clone(keys);

			return null;
		}
		protected override XVar getAfterEditAction()
		{
			dynamic action = null;
			if((XVar)(true)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(this.afterEditAction), XVar.Pack(null)))))
			{
				return this.afterEditAction;
			}
			action = XVar.Clone(this.pSet.getAfterEditAction());
			if(action != Constants.AE_TO_LIST)
			{
				action = new XVar(Constants.AE_TO_EDIT);
			}
			if((XVar)(this.isPopupMode())  && (XVar)(this.pSet.checkClosePopupAfterEdit()))
			{
				action = new XVar(Constants.AE_TO_LIST);
			}
			this.afterEditAction = XVar.Clone(action);
			return this.afterEditAction;
		}
		public override XVar process()
		{
			if(this.action == "edited")
			{
				this.processDataInput();
				this.readEditValues = XVar.Clone(!(XVar)(this.updatedSuccessfully));
				if(XVar.Pack(this.isPopupMode()))
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
				this.cachedRecord = new XVar(null);
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
			this.setPageTitle((XVar)(Labels.getTableCaption((XVar)(this.tName))));
			this.prepareReadonlyFields();
			this.doCommonAssignments();
			this.prepareButtons();
			this.prepareSteps();
			this.prepareEditControls();
			this.prepareJsSettings();
			this.addButtonHandlers();
			this.addCommonJs();
			this.fillSetCntrlMaps();
			this.displayEditPage();

			return null;
		}
		protected override XVar prepareJsSettings()
		{
			this.pageData.InitAndSetArrayItem(this.getDetailTablesMasterKeys((XVar)(this.getCurrentRecordInternal())), "detailsMasterKeys");

			return null;
		}
		protected override XVar doCommonAssignments()
		{
			this.message = XVar.Clone(this.getMessages());
			base.doCommonAssignments();

			return null;
		}
		protected override XVar prepareDetailsTables()
		{

			return null;
		}
		protected override XVar prepareNextPrevButtons()
		{

			return null;
		}
		protected override XVar prepareButtons()
		{
			base.prepareButtons();
			this.hideItemType(new XVar("edit_view"));
			this.hideItemType(new XVar("prev"));
			this.hideItemType(new XVar("next"));
			this.xt.assign(new XVar("save_button"), new XVar(false));
			this.xt.assign(new XVar("view_page_button"), new XVar(false));
			this.xt.assign(new XVar("updsel_button"), new XVar(true));
			this.xt.assign(new XVar("updselbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"saveButton", this.id, "\"")));
			foreach (KeyValuePair<XVar, dynamic> mLString in this.pSet.updateSelectedButtons().GetEnumerator())
			{
				dynamic label = null;
				label = XVar.Clone(MVCFunctions.str_replace(new XVar("%n%"), (XVar)(this.nSelected), (XVar)(CommonFunctions.GetMLString((XVar)(mLString.Value)))));
				this.xt.assign((XVar)(MVCFunctions.Concat(mLString.Key, "_label")), (XVar)(label));
			}

			return null;
		}
		protected override XVar lockRecord()
		{
			return true;
		}
		protected override XVar reportInlineSaveStatus()
		{
			dynamic returnJSON = XVar.Array();
			returnJSON = XVar.Clone(this.inlineReportData);
			returnJSON.InitAndSetArrayItem(this.updatedSuccessfully, "success");
			if(XVar.Pack(!(XVar)(this.isCaptchaOk)))
			{
				returnJSON.InitAndSetArrayItem(this.getCaptchaFieldName(), "wrongCaptchaFieldName");
			}
			returnJSON.InitAndSetArrayItem(this.getMessages(), "message");
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar getRowSaveStatusJSON(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic controlValues = XVar.Array(), data = XVar.Array(), dmapIconsData = null, fieldsIconsData = null, i = null, keyParams = XVar.Array(), keylink = null, listPSet = null, rawValues = XVar.Array(), returnJSON = XVar.Array(), values = XVar.Array();
			returnJSON = XVar.Clone(XVar.Array());
			if((XVar)(this.action != "edited")  || (XVar)(this.isSimpleMode()))
			{
				return returnJSON;
			}
			returnJSON.InitAndSetArrayItem(MVCFunctions.array_keys((XVar)(this.newRecordData)), "fNamesSelected");
			returnJSON.InitAndSetArrayItem(this.getMessages(), "message");
			returnJSON.InitAndSetArrayItem(this.lockingMessageText, "lockMessage");
			if(XVar.Pack(!(XVar)(this.isCaptchaOk)))
			{
				returnJSON.InitAndSetArrayItem(this.getCaptchaFieldName(), "wrongCaptchaFieldName");
			}
			data = XVar.Clone(this.getRecordByKeys((XVar)(keys)));
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
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("key", k.Key + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(keys[k.Value]))))), null);
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
			returnJSON.InitAndSetArrayItem(controlValues, "controlValues");
			returnJSON.InitAndSetArrayItem(this.jsKeys, "keys");
			returnJSON.InitAndSetArrayItem(this.getDetailTablesMasterKeys((XVar)(data)), "masterKeys");
			returnJSON.InitAndSetArrayItem(this.pSet.getTableKeys(), "keyFields");
			returnJSON.InitAndSetArrayItem(XVar.Array(), "oldKeys");
			i = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> value in this.oldKeys.GetEnumerator())
			{
				returnJSON.InitAndSetArrayItem(value.Value, "oldKeys", i++);
			}
			returnJSON.InitAndSetArrayItem(values, "vals");
			returnJSON.InitAndSetArrayItem(this.pSet.getFieldsList(), "fields");
			returnJSON.InitAndSetArrayItem(rawValues, "rawVals");
			returnJSON.InitAndSetArrayItem(this.buildDetailGridLinks((XVar)(returnJSON["detKeys"])), "hrefs");
			if(XVar.Pack(!(XVar)(this.isRecordEditable(new XVar(false)))))
			{
				returnJSON.InitAndSetArrayItem(true, "nonEditable");
			}
			dmapIconsData = XVar.Clone(this.getDashMapsIconsData((XVar)(data)));
			if(XVar.Pack(MVCFunctions.count(dmapIconsData)))
			{
				returnJSON.InitAndSetArrayItem(dmapIconsData, "mapIconsData");
			}
			fieldsIconsData = XVar.Clone(this.getFieldMapIconsData((XVar)(data)));
			if(XVar.Pack(MVCFunctions.count(fieldsIconsData)))
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
		protected override XVar afterEditActionRedirect()
		{
			if(XVar.Pack(this.isPopupMode()))
			{
				return false;
			}
			switch(((XVar)this.getAfterEditAction()).ToInt())
			{
				case Constants.AE_TO_EDIT:
					return this.prgRedirect();
				case Constants.AE_TO_LIST:
					if(XVar.Pack(this.pSet.hasListPage()))
					{
						MVCFunctions.HeaderRedirect((XVar)(this.shortTableName), new XVar(Constants.PAGE_LIST), new XVar("a=return"));
					}
					else
					{
						MVCFunctions.HeaderRedirect(new XVar("menu"));
					}
					return true;
				default:
					return false;
			}

			return null;
		}
		protected override XVar getPrevKeys()
		{
			return XVar.Array();
		}
		protected override XVar getNextKeys()
		{
			return XVar.Array();
		}
		protected override XVar prgRedirect()
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
			XSession.Session["edit_seletion"] = this.selection;
			XSession.Session["message_edit"] = this.getMessages();
			XSession.Session["message_edit_type"] = this.messageType;
			getParams = XVar.Clone(this.getStateUrlParams());
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
		protected virtual XVar getSingleRecordWhereClause(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic dc = null, sql = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			dc.keys = XVar.Clone(keys);
			dc.filter = XVar.Clone(this.getSecurityCondition());
			sql = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
			return sql["where"];
		}
		public override XVar getKeysWhereClause(dynamic _param_useOldKeys)
		{
			#region pass-by-value parameters
			dynamic useOldKeys = XVar.Clone(_param_useOldKeys);
			#endregion

			return this.currentWhereExpr;
		}
		public override XVar getCurrentRecordInternal()
		{
			dynamic dc = null, diffValues = XVar.Array(), fetchedArray = XVar.Array(), fields = XVar.Array(), rs = null;
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(this.cachedRecord), XVar.Pack(null)))))
			{
				return this.cachedRecord;
			}
			dc = XVar.Clone(this.getSubsetDataCommand());
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
			this.nSelected = new XVar(0);
			fields = XVar.Clone(this.getPageFields());
			diffValues = XVar.Clone(XVar.Array());
			rs = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			while(XVar.Pack(fetchedArray = XVar.Clone(rs.fetchAssoc())))
			{
				fetchedArray = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
				if(XVar.Pack(!(XVar)(this.cachedRecord)))
				{
					this.cachedRecord = XVar.Clone(fetchedArray);
				}
				else
				{
					foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
					{
						if(this.cachedRecord[f.Value] != fetchedArray[f.Value])
						{
							diffValues.InitAndSetArrayItem(true, f.Value);
						}
					}
				}
				++(this.nSelected);
			}
			foreach (KeyValuePair<XVar, dynamic> v in diffValues.GetEnumerator())
			{
				this.cachedRecord.Remove(v.Key);
			}
			if(this.action != "edited")
			{
				foreach (KeyValuePair<XVar, dynamic> fName in this.getPageFields().GetEnumerator())
				{
					dynamic aValue = null;
					aValue = XVar.Clone(this.pSet.getAutoUpdateValue((XVar)(fName.Value)));
					if(!XVar.Equals(XVar.Pack(aValue), XVar.Pack("")))
					{
						this.cachedRecord.InitAndSetArrayItem(this.pSet.getAutoUpdateValue((XVar)(fName.Value)), fName.Value);
					}
				}
			}
			this.cachedRecord.InitAndSetArrayItem("", "...");
			return this.cachedRecord;
		}
		protected override XVar readRecord()
		{
			this.getCurrentRecordInternal();
			return true;
		}
		public override XVar fillControlFlags(dynamic _param_field, dynamic _param_required = null)
		{
			#region default values
			if(_param_required as Object == null) _param_required = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic required = XVar.Clone(_param_required);
			#endregion

			dynamic checkbox = null, data = null, gf = null, label = XVar.Array();
			gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(field)));
			data = XVar.Clone(this.getCurrentRecordInternal());
			checkbox = XVar.Clone(MVCFunctions.Concat("<input type=checkbox class=\"bs-updselbox\" id=updsel_", gf, this.id, " data-field=\"", MVCFunctions.runner_htmlspecialchars((XVar)(field)), "\">"));
			label = XVar.Clone(this.xt.getVar((XVar)(MVCFunctions.Concat(gf, "_label"))));
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(label)))))
			{
				label = XVar.Clone(new XVar("begin", ""));
			}
			label.InitAndSetArrayItem(MVCFunctions.Concat(checkbox, label["begin"]), "begin");
			this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_label")), (XVar)(label));
			if((XVar)(this.pSet.isRequired((XVar)(field)))  || (XVar)(required))
			{
				this.xt.assign((XVar)(MVCFunctions.Concat("required_attr_", MVCFunctions.GoodFieldName((XVar)(field)))), new XVar("data-required=\"true\""));
			}

			return null;
		}
		protected override XVar buildNewRecordData()
		{
			dynamic blobfields = null, efilename_values = null, evalues = null, keys = null, newFields = XVar.Array();
			evalues = XVar.Clone(XVar.Array());
			efilename_values = XVar.Clone(XVar.Array());
			blobfields = XVar.Clone(XVar.Array());
			keys = XVar.Clone(this.keys);
			if(XVar.Pack(!(XVar)(this.selectedFields)))
			{
				this.selectedFields = XVar.Clone(XVar.Array());
			}
			newFields = XVar.Clone(MVCFunctions.array_intersect((XVar)(this.getPageFields()), (XVar)(this.selectedFields)));
			foreach (KeyValuePair<XVar, dynamic> f in newFields.GetEnumerator())
			{
				dynamic control = null;
				control = XVar.Clone(this.getControl((XVar)(f.Value), (XVar)(this.id)));
				control.readWebValue((XVar)(evalues), (XVar)(blobfields), new XVar(null), new XVar(null), (XVar)(efilename_values));
			}
			this.newRecordData = XVar.Clone(evalues);

			return null;
		}
		protected virtual XVar getNewRecordCopy(dynamic _param_newRecordData)
		{
			#region pass-by-value parameters
			dynamic newRecordData = XVar.Clone(_param_newRecordData);
			#endregion

			if(ProjectSettings.ext() == "php")
			{
				return newRecordData;
			}
			else
			{
				dynamic newRecordCopy = XVar.Array();
				newRecordCopy = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> data in newRecordData.GetEnumerator())
				{
					newRecordCopy.InitAndSetArrayItem(data.Value, data.Key);
				}
				return newRecordCopy;
			}

			return null;
		}
		public override XVar processDataInput()
		{
			dynamic newRecordData = null, noLockedIdxs = XVar.Array();
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
				this.setMessage((XVar)(this.message));
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> value in this.newRecordData.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.pSet.allowDuplicateValues((XVar)(value.Key)))))
				{
					this.errorFields.InitAndSetArrayItem(value.Key, null);
					this.setMessage((XVar)(MVCFunctions.Concat(this.pSet.label((XVar)(value.Key)), " ", CommonFunctions.mlang_message(new XVar("INLINE_DENY_DUPLICATES")))));
					return false;
				}
			}
			newRecordData = XVar.Clone(this.getNewRecordCopy((XVar)(this.newRecordData)));
			noLockedIdxs = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.lockingObj))
			{
				foreach (KeyValuePair<XVar, dynamic> s in this.parsedSelection.GetEnumerator())
				{
					if(XVar.Pack(this.lockingObj.LockRecord((XVar)(this.tName), (XVar)(s.Value))))
					{
						noLockedIdxs.InitAndSetArrayItem(s.Key, null);
					}
				}
			}
			foreach (KeyValuePair<XVar, dynamic> s in this.parsedSelection.GetEnumerator())
			{
				dynamic dc = null, fetchedArray = null, newRecordDataTemp = null, rs = null;
				if(XVar.Pack(this.lockingObj))
				{
					if(XVar.Pack(MVCFunctions.in_array((XVar)(s.Key), (XVar)(noLockedIdxs))))
					{
						this.lockingObj.UnlockRecord((XVar)(this.tName), (XVar)(s.Value), new XVar(""));
					}
					else
					{
						continue;
					}
				}
				newRecordDataTemp = XVar.Clone(newRecordData);
				this.newRecordData = XVar.Clone(this.getNewRecordCopy((XVar)(newRecordDataTemp)));
				dc = XVar.Clone(new DsCommand());
				dc.keys = XVar.Clone(s.Value);
				this.currentWhereExpr = XVar.Clone(this.getSingleRecordWhereClause((XVar)(s.Value)));
				rs = XVar.Clone(this.dataSource.getSingle((XVar)(dc)));
				if(XVar.Pack(rs))
				{
					fetchedArray = XVar.Clone(rs.fetchAssoc());
				}
				fetchedArray = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
				if(XVar.Pack(!(XVar)(this.isRecordEditable((XVar)(fetchedArray)))))
				{
					continue;
				}
				this.setUpdatedLatLng((XVar)(this.getNewRecordData()), (XVar)(fetchedArray));
				this.oldKeys = XVar.Clone(s.Value);
				this.recordBeingUpdated = XVar.Clone(fetchedArray);
				if(XVar.Pack(!(XVar)(this.callBeforeEditEvent())))
				{
					continue;
				}
				if(XVar.Pack(this.callCustomEditEvent()))
				{
					dynamic updateResult = null;
					updateResult = XVar.Clone(this.dataSource.updateSingle((XVar)(this.getUpdateDataCommand())));
					if(XVar.Pack(!(XVar)(updateResult)))
					{
						this.setDatabaseError((XVar)(this.dataSource.lastError()));
						continue;
					}
				}
				++(this.nUpdated);
				this.mergeNewRecordData();
				this.auditLogEdit((XVar)(s.Value));
				this.callAfterEditEvent();
				if(XVar.Pack(this.isPopupMode()))
				{
					this.inlineReportData.InitAndSetArrayItem(this.getRowSaveStatusJSON((XVar)(s.Value)), this.rowIds[s.Key]);
				}
			}
			this.updatedSuccessfully = XVar.Clone(0 < this.nUpdated);
			if(XVar.Pack(!(XVar)(this.updatedSuccessfully)))
			{
				return false;
			}
			this.messageType = new XVar(Constants.MESSAGE_INFO);
			this.setSuccessfulEditMessage();
			this.callAfterSuccessfulSave();
			return true;
		}
		protected virtual XVar getRecordByKeys(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic dc = null, rs = null;
			dc = XVar.Clone(new DsCommand());
			dc.keys = XVar.Clone(keys);
			dc.filter = XVar.Clone(this.getSecurityCondition());
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
			rs = XVar.Clone(this.dataSource.getSingle((XVar)(dc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				return null;
			}
			return this.cipherer.DecryptFetchedArray((XVar)(rs.fetchAssoc()));
		}
		protected override XVar setSuccessfulEditMessage()
		{
			dynamic message = null;
			message = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "%succeed%", 1, "%total%")), (XVar)(new XVar(0, MVCFunctions.Concat("<strong>", this.nUpdated, "</strong>"), 1, MVCFunctions.Concat("<strong>", this.nSelected, "</strong>"))), (XVar)(CommonFunctions.mlang_message(new XVar("UPDATE_SELECTED_SUCCESS")))));
			this.setMessage((XVar)(message));
			if(this.nUpdated != this.nSelected)
			{
				message = XVar.Clone(MVCFunctions.str_replace(new XVar("%failed%"), (XVar)(MVCFunctions.Concat("<strong>", this.nSelected - this.nUpdated, "</strong>")), (XVar)(CommonFunctions.mlang_message(new XVar("UPDATE_SELECTED_FAIL")))));
				this.setMessage((XVar)(message));
			}

			return null;
		}
		protected override XVar mergeNewRecordData()
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
		protected override XVar callAfterEditEvent()
		{
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("AfterEdit")))))
			{
				return null;
			}
			this.eventsObject.AfterEdit((XVar)(this.newRecordData), (XVar)(this.getKeysWhereClause(new XVar(false))), (XVar)(this.getOldRecordData()), (XVar)(this.keys), (XVar)(this.mode == Constants.EDIT_INLINE), this);

			return null;
		}
		protected override XVar callAfterSuccessfulSave()
		{
			foreach (KeyValuePair<XVar, dynamic> f in this.editFields.GetEnumerator())
			{
				this.getControl((XVar)(f.Value), (XVar)(this.id)).afterSuccessfulSave();
			}

			return null;
		}
		protected override XVar callCustomEditEvent()
		{
			dynamic ret = null, usermessage = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("CustomEdit")))))
			{
				return true;
			}
			usermessage = new XVar("");
			ret = XVar.Clone(this.eventsObject.CustomEdit((XVar)(this.newRecordData), (XVar)(this.getKeysWhereClause(new XVar(true))), (XVar)(this.getOldRecordData()), (XVar)(this.oldKeys), ref usermessage, (XVar)(this.mode == Constants.EDIT_INLINE), this));
			if(XVar.Pack(!(XVar)(ret)))
			{
				if(0 == MVCFunctions.strlen((XVar)(usermessage)))
				{
					++(this.nUpdated);
				}
				else
				{
					this.setMessage((XVar)(usermessage));
				}
			}
			return ret;
		}
		protected override XVar isRecordEditable(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("IsRecordEditable"), (XVar)(this.tName))))
			{
				if(XVar.Pack(!(XVar)(GlobalVars.globalEvents.IsRecordEditable((XVar)(data), new XVar(true), (XVar)(this.tName)))))
				{
					return false;
				}
			}
			return true;
		}
		protected override XVar checkFieldOnPage(dynamic _param_fName)
		{
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			#endregion

			return this.pSet.appearOnUpdateSelected((XVar)(fName));
		}
		public override XVar getOldRecordData()
		{
			return this.recordBeingUpdated;
		}
		public override XVar setMessage(dynamic _param_message)
		{
			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			#endregion

			this.messages.InitAndSetArrayItem(message, null);

			return null;
		}
		public override XVar setDatabaseError(dynamic _param_message)
		{
			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			#endregion

			this.messages.InitAndSetArrayItem(MVCFunctions.Concat("<strong>&lt;&lt;&lt; ", CommonFunctions.mlang_message(new XVar("RECORD_NOT_UPDATED")), " &gt;&gt;&gt;</strong><br>", message), null);
			this.messageType = new XVar(Constants.MESSAGE_ERROR);

			return null;
		}
		public virtual XVar getMessages()
		{
			return MVCFunctions.implode(new XVar("<br>"), (XVar)(this.messages));
		}
		protected override XVar isPopupMode()
		{
			return this.mode == Constants.EDIT_SELECTED_POPUP;
		}
		protected override XVar isSimpleMode()
		{
			return this.mode == Constants.EDIT_SELECTED_SIMPLE;
		}
		public override XVar getSubsetDataCommand(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic conditions = XVar.Array(), dc = null;
			dc = XVar.Clone(new DsCommand());
			conditions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> s in this.parsedSelection.GetEnumerator())
			{
				conditions.InitAndSetArrayItem(DataCondition.FieldsEqual((XVar)(this.pSet.getTableKeys()), (XVar)(s.Value)), null);
			}
			dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, this.getSecurityCondition(), 1, DataCondition._Or((XVar)(conditions))))));
			return dc;
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
			settings.InitAndSetArrayItem(this.getSelection(), "selection");
			settings.InitAndSetArrayItem(this.pSet.getTableKeys(), "keyFields");
			settings.InitAndSetArrayItem(this.getCaptchaFieldName(), "captchaEditFieldName");
			return settings;
		}
	}
}
