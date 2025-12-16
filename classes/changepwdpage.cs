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
	public partial class ChangePasswordPage : RunnerPage
	{
		public dynamic token = XVar.Pack("");
		public dynamic action;
		protected dynamic passwordField;
		protected dynamic usernameField;
		protected dynamic auditObj = XVar.Pack(null);
		protected dynamic changePwdFields;
		protected dynamic changedSuccess = XVar.Pack(false);
		protected dynamic userPwdMessage = XVar.Pack("");
		protected static bool skipChangePasswordPageCtor = false;
		public ChangePasswordPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipChangePasswordPageCtor)
			{
				skipChangePasswordPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

			if((XVar)(!(XVar)(this.action))  && (XVar)(!(XVar)(this.checkToken())))
			{
				Security.saveRedirectURL();
				MVCFunctions.HeaderRedirect(new XVar("login"));
				return;
			}
			this.passwordField = XVar.Clone(Security.passwordField());
			this.usernameField = XVar.Clone(Security.usernameField());
			this.auditObj = XVar.Clone(CommonFunctions.GetAuditObject());
			if(XVar.Pack(this.token))
			{
				this.changePwdFields = XVar.Clone(new XVar(0, "newpass", 1, "confirm"));
				this.setProxyValue(new XVar("token"), (XVar)(this.token));
			}
			else
			{
				this.changePwdFields = XVar.Clone(new XVar(0, "oldpass", 1, "newpass", 2, "confirm"));
			}
			this.headerForms = XVar.Clone(new XVar(0, "top"));
			this.footerForms = XVar.Clone(new XVar(0, "footer"));
			this.bodyForms = XVar.Clone(new XVar(0, "above-grid", 1, "grid"));
		}
		protected override XVar setTableConnection()
		{
			this.connection = XVar.Clone(GlobalVars.cman.getForLogin());

			return null;
		}
		protected override XVar assignCipherer()
		{
			this.cipherer = XVar.Clone(RunnerCipherer.getForLogin());

			return null;
		}
		protected virtual XVar setReferer()
		{
			dynamic changePwdPage = null, home = null, referer = null;
			referer = XVar.Clone((XVar.Pack(MVCFunctions.GetServerVariable("HTTP_REFERER")) ? XVar.Pack(MVCFunctions.GetServerVariable("HTTP_REFERER")) : XVar.Pack("")));
			home = XVar.Clone(MVCFunctions.strtoupper((XVar)(CommonFunctions.projectURL())));
			changePwdPage = XVar.Clone(MVCFunctions.strtoupper((XVar)(MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("changepwd"))))));
			if((XVar)(MVCFunctions.substr((XVar)(MVCFunctions.strtoupper((XVar)(referer))), new XVar(0), (XVar)(MVCFunctions.strlen((XVar)(home)))) != home)  || (XVar)(MVCFunctions.substr((XVar)(MVCFunctions.strtoupper((XVar)(referer))), new XVar(0), (XVar)(MVCFunctions.strlen((XVar)(changePwdPage)))) == changePwdPage))
			{
				referer = new XVar("");
			}
			if(XVar.Pack(referer))
			{
				XSession.Session["changepwd_referer"] = referer;
			}
			if(XVar.Pack(!(XVar)(XSession.Session["changepwd_referer"])))
			{
				XSession.Session["changepwd_referer"] = CommonFunctions.projectURL();
			}

			return null;
		}
		protected virtual XVar getTokenCondition()
		{
			return DataCondition.FieldEquals((XVar)(ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("resetTokenField"))), (XVar)(this.token));
		}
		protected virtual XVar getUsernameCondition()
		{
			dynamic caseInsensitive = null;
			if(XVar.Pack(this.token))
			{
				return this.getTokenCondition();
			}
			caseInsensitive = XVar.Clone((XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_STRICT)));
			return DataCondition.FieldEquals((XVar)(this.usernameField), (XVar)(Security.getUserName()), new XVar(0), (XVar)(caseInsensitive));
		}
		protected virtual XVar getUpdateCommand(dynamic _param_newpass)
		{
			#region pass-by-value parameters
			dynamic newpass = XVar.Clone(_param_newpass);
			#endregion

			dynamic dc = null, values = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("hashPassword"))))
			{
				if(XVar.Pack(!(XVar)(this.cipherer.isFieldEncrypted((XVar)(this.passwordField)))))
				{
					newpass = XVar.Clone(Security.hashPassword((XVar)(newpass)));
				}
			}
			values = XVar.Clone(XVar.Array());
			values.InitAndSetArrayItem(newpass, this.passwordField);
			if(XVar.Pack(this.token))
			{
				values.InitAndSetArrayItem("", ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("resetTokenField")));
				values.InitAndSetArrayItem(null, ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("resetDateField")));
				if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
				{
					values.InitAndSetArrayItem("1", ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("activationField")));
				}
			}
			dc.values = XVar.Clone(values);
			dc.filter = XVar.Clone(this.getUsernameCondition());
			return dc;
		}
		protected virtual XVar getControlValues()
		{
			dynamic blobfields = null, filename_values = null, values = null;
			filename_values = XVar.Clone(XVar.Array());
			blobfields = XVar.Clone(XVar.Array());
			values = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> fName in this.changePwdFields.GetEnumerator())
			{
				dynamic fControl = null;
				fControl = XVar.Clone(this.getControl((XVar)(fName.Value), (XVar)(this.id)));
				fControl.readWebValue((XVar)(values), (XVar)(blobfields), new XVar(null), new XVar(null), (XVar)(filename_values));
			}
			return values;
		}
		protected virtual XVar changePassword()
		{
			dynamic data = null, dbOldPass = null, dc = null, newPass = null, oldPass = null, qResult = null, retval = null, row = XVar.Array(), username = null, values = XVar.Array();
			if(XVar.Pack(!(XVar)(CommonFunctions.isPostRequest())))
			{
				return null;
			}
			values = XVar.Clone(this.getControlValues());
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getUsernameCondition());
			qResult = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			data = XVar.Clone(qResult.fetchAssoc());
			row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(data)));
			dbOldPass = new XVar("");
			if(XVar.Pack(!(XVar)(row)))
			{
				this.message = XVar.Clone(CommonFunctions.mlang_message(new XVar("INVALID_PASSWORD")));
				return false;
			}
			dbOldPass = XVar.Clone(row[this.passwordField]);
			username = XVar.Clone(row[this.usernameField]);
			if((XVar)(!(XVar)(this.token))  && (XVar)(this.pSet.hasOldPassField()))
			{
				if(XVar.Pack(!(XVar)(Security.verifyPassword((XVar)(values["oldpass"]), (XVar)(dbOldPass)))))
				{
					this.message = XVar.Clone(CommonFunctions.mlang_message(new XVar("INVALID_PASSWORD")));
					return false;
				}
			}
			oldPass = XVar.Clone(dbOldPass);
			newPass = XVar.Clone(values["newpass"]);
			if((XVar)(ProjectSettings.passwordValidationValue(new XVar("strong")))  && (XVar)(!(XVar)(CommonFunctions.checkpassword((XVar)(newPass)))))
			{
				this.message = XVar.Clone(this.getPwdStrongFailedMessage());
				this.userPwdMessage = XVar.Clone(this.message);
				return false;
			}
			retval = new XVar(true);
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeChangePassword"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.BeforeChangePassword((XVar)(oldPass), ref newPass, this));
			}
			values.InitAndSetArrayItem(newPass, "newpass");
			if(XVar.Pack(retval))
			{
				dc = XVar.Clone(this.getUpdateCommand((XVar)(values["newpass"])));
				this.dataSource.updateSingle((XVar)(dc), new XVar(false));
				if(XVar.Pack(this.auditObj))
				{
					this.auditObj.LogChPassword((XVar)(username));
				}
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterChangePassword"))))
				{
					GlobalVars.globalEvents.AfterChangePassword((XVar)(oldPass), (XVar)(values["newpass"]), this);
				}
			}
			return retval;
		}
		public virtual XVar process()
		{
			this.setReferer();
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeProcessChangePwd"))))
			{
				GlobalVars.globalEvents.BeforeProcessChangePwd(this);
			}
			if(this.action == "Change")
			{
				this.changedSuccess = XVar.Clone(this.changePassword());
				if((XVar)(!(XVar)(this.changedSuccess))  && (XVar)(this.mode == Constants.CHANGEPASS_POPUP))
				{
					dynamic returnJSON = XVar.Array();
					returnJSON = XVar.Clone(XVar.Array());
					returnJSON.InitAndSetArrayItem(false, "success");
					if(XVar.Pack(MVCFunctions.strlen((XVar)(this.message))))
					{
						returnJSON.InitAndSetArrayItem(this.message, "message");
					}
					if(XVar.Pack(!(XVar)(this.isCaptchaOk)))
					{
						returnJSON.InitAndSetArrayItem(this.getCaptchaFieldName(), "wrongCaptchaFieldName");
					}
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
					MVCFunctions.ob_flush();
					HttpContext.Current.Response.End();
					throw new RunnerInlineOutputException();
				}
			}
			if(XVar.Pack(!(XVar)(this.changedSuccess)))
			{
				this.prepareEditControls();
			}
			else
			{
				this.pageName = XVar.Clone(this.pSet.getDefaultPage((XVar)(this.successPageType())));
				this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.successPageType()), (XVar)(this.pageName), (XVar)(this.pageTable)));
				this.pageData.InitAndSetArrayItem(MVCFunctions.array_merge((XVar)(this.pageData["buttons"]), (XVar)(this.pSet.buttons())), "buttons");
				foreach (KeyValuePair<XVar, dynamic> b in this.pSet.buttons().GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(b.Value)))
					{
						continue;
					}
					this.AddJSFile((XVar)(MVCFunctions.Concat("usercode/button_", b.Value, ".js")));
				}
			}
			this.addCommonJs();
			this.fillSetCntrlMaps();
			this.addButtonHandlers();
			this.doCommonAssignments();
			this.showPage();

			return null;
		}
		protected virtual XVar reportChangeStatus(dynamic _param_changed)
		{
			#region pass-by-value parameters
			dynamic changed = XVar.Clone(_param_changed);
			#endregion

			dynamic returnJSON = XVar.Array();
			returnJSON = XVar.Clone(XVar.Array());
			returnJSON.InitAndSetArrayItem(this.message, "message");
			returnJSON.InitAndSetArrayItem(true, "success");
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar prepareEditControls()
		{
			foreach (KeyValuePair<XVar, dynamic> fName in this.changePwdFields.GetEnumerator())
			{
				dynamic controls = XVar.Array(), parameters = XVar.Array();
				parameters = XVar.Clone(XVar.Array());
				parameters.InitAndSetArrayItem(this.id, "id");
				parameters.InitAndSetArrayItem("add", "mode");
				parameters.InitAndSetArrayItem(fName.Value, "field");
				parameters.InitAndSetArrayItem("Password", "format");
				parameters.InitAndSetArrayItem(this, "pageObj");
				parameters.InitAndSetArrayItem(true, "suggest");
				parameters.InitAndSetArrayItem(new XVar("basicValidate", new XVar(0, "IsRequired")), "validate");
				parameters.InitAndSetArrayItem(XVar.Array(), "extraParams");
				parameters.InitAndSetArrayItem(true, "extraParams", "getConrirmFieldCtrl");
				controls = XVar.Clone(new XVar("controls", XVar.Array()));
				controls.InitAndSetArrayItem(this.id, "controls", "id");
				controls.InitAndSetArrayItem("add", "controls", "mode");
				controls.InitAndSetArrayItem(0, "controls", "ctrlInd");
				controls.InitAndSetArrayItem(fName.Value, "controls", "fieldName");
				controls.InitAndSetArrayItem(parameters["suggest"], "controls", "suggest");
				this.xt.assign_function((XVar)(MVCFunctions.Concat(fName.Value, "_editcontrol")), new XVar("xt_buildeditcontrol"), (XVar)(parameters));
				this.xt.assign((XVar)(MVCFunctions.Concat(fName.Value, "_label")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat("labelfor_", MVCFunctions.GoodFieldName((XVar)(fName.Value)))), (XVar)(MVCFunctions.Concat("value_", fName.Value, "_", this.id)));
				if(XVar.Pack(this.is508))
				{
					this.xt.assign_section((XVar)(MVCFunctions.Concat(fName.Value, "_label")), (XVar)(MVCFunctions.Concat("<label for=\"value_", fName.Value, "_", this.id, "\">")), new XVar("</label>"));
				}
				this.xt.assign((XVar)(MVCFunctions.Concat(fName.Value, "_block")), new XVar(true));
				this.fillControlsMap((XVar)(controls));
			}

			return null;
		}
		protected override XVar assignBody()
		{
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage(new XVar(false)));
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
			this.xt.assignbyref(new XVar("body"), (XVar)(this.body));

			return null;
		}
		protected virtual XVar doCommonAssignments()
		{
			this.xt.assign(new XVar("submit_attrs"), (XVar)(MVCFunctions.Concat("id=\"saveButton", this.id, "\"")));
			if(this.mode == Constants.CHANGEPASS_POPUP)
			{
				this.hideItemType(new XVar("changepwd_back"));
				if(XVar.Pack(this.changedSuccess))
				{
					this.xt.assign(new XVar("backlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"backButton", this.id, "\"")));
				}
			}
			else
			{
				this.xt.assign(new XVar("backlink_attrs"), (XVar)(MVCFunctions.Concat("href=\"", MVCFunctions.runner_htmlspecialchars((XVar)(XSession.Session["changepwd_referer"])), "\"")));
			}
			this.xt.assign(new XVar("message_block"), new XVar(true));
			if(XVar.Pack(this.message))
			{
				this.xt.assign(new XVar("message_class"), new XVar("alert-danger"));
				this.xt.assign(new XVar("message"), (XVar)(this.message));
			}
			else
			{
				this.hideElement(new XVar("message"));
			}
			this.assignBody();

			return null;
		}
		protected virtual XVar showPage()
		{
			dynamic templatefile = null;
			if(XVar.Pack(this.changedSuccess))
			{
				this.switchToSuccessPage();
			}
			templatefile = XVar.Clone(this.templatefile);
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeShowChangePwd"))))
			{
				GlobalVars.globalEvents.BeforeShowChangePwd((XVar)(this.xt), ref templatefile, this);
			}
			if(this.mode == Constants.CHANGEPASS_POPUP)
			{
				this.xt.assign(new XVar("footer"), new XVar(false));
				this.xt.assign(new XVar("header"), new XVar(false));
				this.xt.assign(new XVar("body"), (XVar)(this.body));
				this.displayAJAX((XVar)(templatefile), (XVar)(this.id + 1));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			this.display((XVar)(templatefile));

			return null;
		}
		public static XVar readActionFromRequest()
		{
			if(XVar.Pack(MVCFunctions.postvalue("btnSubmit")))
			{
				return MVCFunctions.postvalue("btnSubmit");
			}
			return "";
		}
		protected virtual XVar checkToken()
		{
			dynamic data = XVar.Array(), dc = null, qResult = null;
			if(XVar.Pack(!(XVar)(this.token)))
			{
				return true;
			}
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getTokenCondition());
			qResult = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(qResult.fetchAssoc())));
			if(XVar.Pack(data))
			{
				return MVCFunctions.secondsPassedFrom((XVar)(data[ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("resetDateField"))])) < 86400;
			}
			return false;
		}
		public static XVar readModeFromRequest()
		{
			if(MVCFunctions.postvalue(new XVar("mode")) == "popup")
			{
				return Constants.CHANGEPASS_POPUP;
			}
			return Constants.CHANGEPASS_SIMPLE;
		}
		public override XVar element2Item(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(name == "message")
			{
				return new XVar(0, "changepwd_message");
			}
			return base.element2Item((XVar)(name));
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
			if(XVar.Pack(this.userPwdMessage))
			{
				settings.InitAndSetArrayItem(this.userPwdMessage, "msg_passwordError");
			}
			return settings;
		}
	}
}
