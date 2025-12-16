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
	public partial class RegisterPage : RunnerPage
	{
		public dynamic action;
		protected dynamic regValues = XVar.Array();
		protected dynamic registerSuccess = XVar.Pack(false);
		protected dynamic strUsername;
		protected dynamic strPassword;
		protected dynamic strEmail;
		protected dynamic usernameFiled;
		protected dynamic emailFiled;
		protected dynamic prepActivationCode = XVar.Pack("");
		protected dynamic sendActivationLink = XVar.Pack(false);
		protected dynamic sendActivationLinkFailedMessage = XVar.Pack("");
		protected dynamic userErrorMessage = XVar.Pack("");
		protected dynamic emailErrorMessage = XVar.Pack("");
		protected dynamic pwdErrorMessage = XVar.Pack("");
		protected static bool skipRegisterPageCtor = false;
		public RegisterPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipRegisterPageCtor)
			{
				skipRegisterPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

			this.usernameFiled = XVar.Clone(Security.usernameField());
			this.emailFiled = XVar.Clone(CommonFunctions.GetEmailField());
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				this.sendActivationLink = new XVar(true);
			}
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
		}
		protected override XVar setTableConnection()
		{
			this.connection = XVar.Clone(GlobalVars.cman.getForLogin());

			return null;
		}
		protected override XVar assignCipherer()
		{
			this.cipherer = XVar.Clone(new RunnerCipherer((XVar)(this.tName)));

			return null;
		}
		protected override XVar setDataSource()
		{
			this.dataSource = XVar.Clone(CommonFunctions.getLoginDataSource());

			return null;
		}
		protected virtual XVar activateNewUser()
		{
			dynamic code = null, continueUrl = null, data = XVar.Array(), dbPassword = null, dc = null, dcUpdate = null, onClick = null, rs = null, sessionLevel = null, username = null, usernameCondition = null;
			username = XVar.Clone(MVCFunctions.base64_decode((XVar)(MVCFunctions.postvalue("u"))));
			code = XVar.Clone(MVCFunctions.postvalue("code"));
			usernameCondition = XVar.Clone(DataCondition.FieldEquals((XVar)(Security.usernameField()), (XVar)(username), new XVar(0), (XVar)((XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_STRICT)))));
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(usernameCondition);
			rs = XVar.Clone(this.dataSource.getSingle((XVar)(dc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				MVCFunctions.Echo(CommonFunctions.mlang_message(new XVar("SEC_INVALID_REG_CODE")));
				return null;
			}
			data = XVar.Clone(this.dataSource.decryptRecord((XVar)(rs.fetchAssoc())));
			if(XVar.Pack(!(XVar)(data)))
			{
				MVCFunctions.Echo(CommonFunctions.mlang_message(new XVar("SEC_INVALID_REG_CODE")));
				return null;
			}
			dbPassword = XVar.Clone(data[Security.passwordField()]);
			if(XVar.Pack(!(XVar)(Security.verifyActivationCode((XVar)(code), (XVar)(username), (XVar)(dbPassword)))))
			{
				MVCFunctions.Echo(CommonFunctions.mlang_message(new XVar("SEC_INVALID_REG_CODE")));
				return null;
			}
			dcUpdate = XVar.Clone(new DsCommand());
			dcUpdate.values.InitAndSetArrayItem(1, ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("activationField")));
			dcUpdate.filter = XVar.Clone(usernameCondition);
			this.dataSource.updateSingle((XVar)(dcUpdate), new XVar(false));
			sessionLevel = XVar.Clone(Security.userSessionLevel());
			if(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_ACTIVATION_PENDING)))
			{
				dynamic twofSettings = XVar.Array();
				twofSettings = Security.twoFactorSettings();
				if((XVar)(twofSettings["available"])  && (XVar)((XVar)(twofSettings["required"])  || (XVar)(twofSettings["enable"])))
				{
					Security.elevateSession(new XVar(Constants.LOGGED_2FSETUP_PENDING));
				}
				else
				{
					Security.elevateSession();
					Security.auditLoginSuccess();
					Security.callAfterLogin();
				}
				sessionLevel = XVar.Clone(Security.userSessionLevel());
			}
			this.switchToSuccessPage();
			this.hideItemType(new XVar("register_activate_message"));
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<form method=\"POST\" action=\"", MVCFunctions.GetTableLink(new XVar("login")), "\" name=\"loginform\">\r\n\t\t<input type=\"Hidden\" name=\"username\" value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(username)), "\">");
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "</form>");
			onClick = new XVar("");
			if(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_2FSETUP_PENDING)))
			{
				continueUrl = XVar.Clone(MVCFunctions.GetTableLink(new XVar("userinfo")));
			}
			else
			{
				if(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_FULL)))
				{
					continueUrl = XVar.Clone(MVCFunctions.GetTableLink(new XVar("menu")));
				}
				else
				{
					continueUrl = XVar.Clone(MVCFunctions.GetTableLink(new XVar("login")));
					onClick = new XVar("onclick=\"document.forms.loginform.submit();return false;\"");
				}
			}
			this.xt.assign(new XVar("body"), (XVar)(this.body));
			this.xt.assign(new XVar("loginlink_attrs"), (XVar)(MVCFunctions.Concat("href=\"", continueUrl, "\" ", onClick, " id=\"ProceedToLogin\"")));
			this.display((XVar)(this.templatefile));

			return null;
		}
		public virtual XVar process()
		{
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeProcessRegister"))))
			{
				GlobalVars.globalEvents.BeforeProcessRegister(this);
			}
			if((XVar)(this.action == "activate")  && (XVar)(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				return this.activateNewUser();
			}
			if(this.action == "Register")
			{
				this.registerSuccess = XVar.Clone(this.registerNewUser());
				this.doAfterRegistrationEvent();
				this.notifyUserAndAdmin();
				if((XVar)(!(XVar)(this.registerSuccess))  && (XVar)(this.mode == Constants.REGISTER_POPUP))
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
			if(XVar.Pack(this.captchaExists()))
			{
				this.displayCaptcha();
			}
			if(XVar.Pack(!(XVar)(this.registerSuccess)))
			{
				this.prepareEditControls();
				this.prepareSteps();
				this.prepareReadonlyFields();
			}
			if((XVar)((XVar)(this.registerSuccess)  && (XVar)(!(XVar)(this.sendActivationLink)))  || (XVar)(!(XVar)(this.registerSuccess)))
			{
				this.addCommonJs();
				this.fillSetCntrlMaps();
				this.addButtonHandlers();
			}
			if(XVar.Pack(this.registerSuccess))
			{
				this.tryLoginNewUser();
				this.pageName = XVar.Clone(this.pSet.getDefaultPage((XVar)(this.successPageType())));
				this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), (XVar)(this.pageName), new XVar(Constants.GLOBAL_PAGES)));
				this.xt.assign(new XVar("supertop_block"), new XVar(true));
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
			this.doCommonAssignments();
			this.showPage();

			return null;
		}
		public override XVar addCommonJs()
		{
			ProjectSettings pSet;
			base.addCommonJs();
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.pageType), (XVar)(this.pageName)));
			if(XVar.Pack(pSet.hasJsEvents()))
			{
				this.AddJSFile((XVar)(MVCFunctions.Concat("usercode/pageevents_", CommonFunctions.GetTableURL((XVar)(this.pageTable)), ".js")));
			}

			return null;
		}
		protected virtual XVar doAfterRegistrationEvent()
		{
			if((XVar)(this.registerSuccess)  && (XVar)(GlobalVars.globalEvents.exists(new XVar("AfterSuccessfulRegistration"))))
			{
				GlobalVars.globalEvents.AfterSuccessfulRegistration((XVar)(this.regValues), this);
			}
			if((XVar)(!(XVar)(this.registerSuccess))  && (XVar)(GlobalVars.globalEvents.exists(new XVar("AfterUnsuccessfulRegistration"))))
			{
				GlobalVars.globalEvents.AfterUnsuccessfulRegistration((XVar)(this.regValues), ref this.message, this);
			}

			return null;
		}
		protected virtual XVar notifyUserAndAdmin()
		{
			if(XVar.Pack(!(XVar)(this.registerSuccess)))
			{
				return null;
			}
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("notifyUser"))))
			{
				dynamic sentMailResults = XVar.Array();
				sentMailResults = XVar.Clone(this.sendUserRegisterMessage());
				if(XVar.Pack(!(XVar)(sentMailResults["mailed"])))
				{
					this.message = MVCFunctions.Concat(this.message, " ", sentMailResults["message"]);
					if(XVar.Pack(this.sendActivationLink))
					{
						this.sendActivationLinkFailedMessage = XVar.Clone(sentMailResults["message"]);
					}
				}
			}
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("notifyAdmin"))))
			{
				dynamic sentMailResults1 = XVar.Array();
				sentMailResults1 = XVar.Clone(this.sendAdminRegisterMessage());
				if(XVar.Pack(!(XVar)(sentMailResults1["mailed"])))
				{
					this.message = MVCFunctions.Concat(this.message, " ", sentMailResults1["message"]);
				}
			}

			return null;
		}
		protected virtual XVar sendUserRegisterMessage()
		{
			dynamic data = XVar.Array(), html = null, pos = null, strEmail = null;
			data = XVar.Clone(XVar.Array());
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				data.InitAndSetArrayItem(this.getUserActivationUrl((XVar)(this.regValues[Security.usernameField()]), (XVar)(this.prepActivationCode)), "activateurl");
			}
			foreach (KeyValuePair<XVar, dynamic> uf in this.pSet.getPageFields().GetEnumerator())
			{
				data.InitAndSetArrayItem(this.regValues[uf.Value], MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(uf.Value, "_value"))));
			}
			strEmail = XVar.Clone(this.strEmail);
			if((XVar)(!XVar.Equals(XVar.Pack(pos = XVar.Clone(MVCFunctions.strpos((XVar)(this.strEmail), new XVar("\r")))), XVar.Pack(false)))  || (XVar)(!XVar.Equals(XVar.Pack(pos = XVar.Clone(MVCFunctions.strpos((XVar)(this.strEmail), new XVar("\n")))), XVar.Pack(false))))
			{
				strEmail = XVar.Clone(MVCFunctions.substr((XVar)(this.strEmail), new XVar(0), (XVar)(pos)));
			}
			data.InitAndSetArrayItem(strEmail, "email_value");
			html = XVar.Clone(CommonFunctions.isEmailTemplateUseHTML(new XVar("userregister")));
			return RunnerPage.sendEmailByTemplate((XVar)(strEmail), new XVar("userregister"), (XVar)(data), (XVar)(html));
		}
		protected virtual XVar sendAdminRegisterMessage()
		{
			dynamic data = XVar.Array(), html = null, strEmail = null;
			data = XVar.Clone(XVar.Array());
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				data.InitAndSetArrayItem(this.getUserActivationUrl((XVar)(this.regValues[Security.usernameField()]), (XVar)(this.prepActivationCode)), "activateurl");
			}
			foreach (KeyValuePair<XVar, dynamic> uf in this.pSet.getPageFields().GetEnumerator())
			{
				data.InitAndSetArrayItem(this.regValues[uf.Value], MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(uf.Value, "_value"))));
			}
			strEmail = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("adminEmail")));
			html = XVar.Clone(CommonFunctions.isEmailTemplateUseHTML(new XVar("adminregister")));
			return RunnerPage.sendEmailByTemplate((XVar)(strEmail), new XVar("adminregister"), (XVar)(data), (XVar)(html));
		}
		protected virtual XVar registerNewUser()
		{
			dynamic allow_registration = null, blobfields = null, dc = null, filename_values = XVar.Array(), originalpassword = null, retval = null, sqlValues = XVar.Array(), values = XVar.Array();
			allow_registration = new XVar(true);
			if(XVar.Pack(!(XVar)(this.checkCaptcha())))
			{
				allow_registration = new XVar(false);
			}
			values = XVar.Clone(XVar.Array());
			blobfields = XVar.Clone(XVar.Array());
			filename_values = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> uf in this.pSet.getPageFields().GetEnumerator())
			{
				dynamic _control = null;
				_control = XVar.Clone(this.getControl((XVar)(uf.Value), (XVar)(this.id)));
				_control.readWebValue((XVar)(values), (XVar)(blobfields), new XVar(null), new XVar(null), (XVar)(filename_values));
			}
			foreach (KeyValuePair<XVar, dynamic> value in filename_values.GetEnumerator())
			{
				values.InitAndSetArrayItem(value.Value, value.Key);
			}
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				values.InitAndSetArrayItem(0, ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("activationField")));
			}
			this.strUsername = XVar.Clone(values[Security.usernameField()]);
			this.strPassword = XVar.Clone(values[Security.passwordField()]);
			if(XVar.Pack(Security.emailField()))
			{
				this.strEmail = XVar.Clone(values[Security.emailField()]);
			}
			this.regValues = XVar.Clone(values);
			if((XVar)(!(XVar)(this.checkRegisterData((XVar)(this.strUsername), (XVar)(this.strPassword), (XVar)(this.strEmail))))  || (XVar)(!(XVar)(this.checkDeniedDuplicated((XVar)(values)))))
			{
				allow_registration = new XVar(false);
			}
			retval = XVar.Clone(allow_registration);
			sqlValues = XVar.Clone(XVar.Array());
			if((XVar)(retval)  && (XVar)(GlobalVars.globalEvents.exists(new XVar("BeforeRegister"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.BeforeRegister((XVar)(values), (XVar)(sqlValues), ref this.message, this));
			}
			if(XVar.Pack(!(XVar)(retval)))
			{
				return false;
			}
			originalpassword = XVar.Clone(values[Security.passwordField()]);
			if((XVar)(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("hashPassword")))  && (XVar)(!(XVar)(this.cipherer.isFieldEncrypted((XVar)(Security.passwordField())))))
			{
				values.InitAndSetArrayItem(Security.hashPassword((XVar)(originalpassword)), Security.passwordField());
			}
			dc = XVar.Clone(new DsCommand());
			dc.values = values;
			dc.advValues = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> sqlValue in sqlValues.GetEnumerator())
			{
				dc.advValues.InitAndSetArrayItem(new DsOperand(new XVar(Constants.dsotSQL), (XVar)(sqlValue.Value)), sqlValue.Key);
			}
			retval = XVar.Clone(this.dataSource.insertSingle((XVar)(dc)));
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				this.prepActivationCode = XVar.Clone(Security.getActivationCode((XVar)(this.strUsername), (XVar)(values[Security.passwordField()])));
			}
			values.InitAndSetArrayItem(originalpassword, Security.passwordField());
			if(XVar.Pack(!(XVar)(retval)))
			{
				this.setDatabaseError((XVar)(this.dataSource.lastError()));
			}
			else
			{
				this.ProcessFiles();
			}
			return !(XVar)(!(XVar)(retval));
		}
		protected virtual XVar checkRegisterData(dynamic _param_strUsername, dynamic _param_strPassword, dynamic _param_strEmail)
		{
			#region pass-by-value parameters
			dynamic strUsername = XVar.Clone(_param_strUsername);
			dynamic strPassword = XVar.Clone(_param_strPassword);
			dynamic strEmail = XVar.Clone(_param_strEmail);
			#endregion

			dynamic ret = null;
			ret = new XVar(true);
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(strUsername)))))
			{
				this.userErrorMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("USER_NOEMPTY")));
				ret = new XVar(false);
			}
			else
			{
				if(XVar.Pack(!(XVar)(this.checkIfUsernameUnique((XVar)(strUsername)))))
				{
					this.userErrorMessage = XVar.Clone(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("USERNAME_EXISTS1")), " <i>", MVCFunctions.runner_htmlspecialchars((XVar)(strUsername)), "</i> ", CommonFunctions.mlang_message(new XVar("USERNAME_EXISTS2"))));
					ret = new XVar(false);
				}
			}
			if((XVar)(Security.emailField())  && (XVar)(this.pSet.appearOnPage((XVar)(Security.emailField()))))
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(strEmail)))))
				{
					this.emailErrorMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("VALID_EMAIL")));
					ret = new XVar(false);
				}
				else
				{
					if(XVar.Pack(!(XVar)(this.checkIfEmailUnique((XVar)(strEmail)))))
					{
						this.emailErrorMessage = XVar.Clone(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("EMAIL_ALREADY1")), " <i>", MVCFunctions.runner_htmlspecialchars((XVar)(strEmail)), "</i> ", CommonFunctions.mlang_message(new XVar("EMAIL_ALREADY2"))));
						ret = new XVar(false);
					}
				}
			}
			if(XVar.Pack(ProjectSettings.passwordValidationValue(new XVar("strong"))))
			{
				if(XVar.Pack(!(XVar)(CommonFunctions.checkpassword((XVar)(strPassword)))))
				{
					this.pwdErrorMessage = XVar.Clone(this.getPwdStrongFailedMessage());
					ret = new XVar(false);
				}
			}
			return ret;
		}
		protected virtual XVar checkIfUsernameUnique(dynamic _param_strUsername)
		{
			#region pass-by-value parameters
			dynamic strUsername = XVar.Clone(_param_strUsername);
			#endregion

			dynamic data = XVar.Array(), sUsername = null, strSQL = null;
			if(XVar.Pack(this.cipherer.isFieldEncrypted((XVar)(Security.usernameField()))))
			{
				sUsername = XVar.Clone(this.cipherer.MakeDBValue((XVar)(Security.usernameField()), (XVar)(strUsername), new XVar(""), new XVar(true)));
			}
			else
			{
				sUsername = XVar.Clone(CommonFunctions.add_db_quotes((XVar)(Security.usernameField()), (XVar)(strUsername)));
			}
			strSQL = XVar.Clone(MVCFunctions.Concat("select count(*) from ", this.connection.addTableWrappers((XVar)(Security.loginTable())), " where ", this.connection.comparisonSQL((XVar)(this.getFieldSQLDecrypt((XVar)(Security.usernameField()))), (XVar)(sUsername), (XVar)(Security.caseInsensitiveUsername()))));
			data = XVar.Clone(this.connection.query((XVar)(strSQL)).fetchNumeric());
			return data[0] == 0;
		}
		protected virtual XVar checkIfEmailUnique(dynamic _param_strEmail)
		{
			#region pass-by-value parameters
			dynamic strEmail = XVar.Clone(_param_strEmail);
			#endregion

			dynamic data = XVar.Array(), sEmail = null, strSQL = null;
			if(XVar.Pack(this.cipherer.isFieldEncrypted((XVar)(Security.emailField()))))
			{
				sEmail = XVar.Clone(this.cipherer.MakeDBValue((XVar)(Security.emailField()), (XVar)(strEmail), new XVar(""), new XVar(true)));
			}
			else
			{
				sEmail = XVar.Clone(CommonFunctions.add_db_quotes((XVar)(Security.emailField()), (XVar)(strEmail)));
			}
			strSQL = XVar.Clone(MVCFunctions.Concat("select count(*) from ", this.connection.addTableWrappers((XVar)(Security.loginTable())), " where ", this.connection.comparisonSQL((XVar)(this.getFieldSQLDecrypt((XVar)(Security.emailField()))), (XVar)(sEmail), new XVar(true))));
			data = XVar.Clone(this.connection.query((XVar)(strSQL)).fetchNumeric());
			return data[0] == 0;
		}
		protected virtual XVar prepareEditControls()
		{
			dynamic regFields = XVar.Array();
			regFields = XVar.Clone(this.pSet.getPageFields());
			if(XVar.Pack(!(XVar)(MVCFunctions.count(this.regValues))))
			{
				foreach (KeyValuePair<XVar, dynamic> f in regFields.GetEnumerator())
				{
					dynamic defaultValue = null;
					defaultValue = XVar.Clone(this.pSetSearch.getDefaultValue((XVar)(f.Value)));
					if(XVar.Pack(MVCFunctions.strlen((XVar)(defaultValue))))
					{
						this.regValues.InitAndSetArrayItem(defaultValue, f.Value);
					}
				}
			}
			foreach (KeyValuePair<XVar, dynamic> fName in regFields.GetEnumerator())
			{
				dynamic controls = XVar.Array(), firstElementId = null, gfName = null, parameters = XVar.Array(), preload = null;
				gfName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(fName.Value)));
				parameters = XVar.Clone(XVar.Array());
				parameters.InitAndSetArrayItem(this.id, "id");
				parameters.InitAndSetArrayItem("add", "mode");
				parameters.InitAndSetArrayItem(fName.Value, "field");
				parameters.InitAndSetArrayItem(this.regValues[fName.Value], "value");
				parameters.InitAndSetArrayItem(this, "pageObj");
				parameters.InitAndSetArrayItem((XVar)((XVar)(fName.Value == Security.passwordField())  || (XVar)(fName.Value == this.usernameFiled))  || (XVar)(fName.Value == this.emailFiled), "suggest");
				if(fName.Value == Security.passwordField())
				{
					parameters.InitAndSetArrayItem(XVar.Array(), "extraParams");
					parameters.InitAndSetArrayItem(true, "extraParams", "getConrirmFieldCtrl");
				}
				if((XVar)((XVar)(fName.Value == this.usernameFiled)  || (XVar)(fName.Value == Security.passwordField()))  || (XVar)(fName.Value == this.emailFiled))
				{
					parameters.InitAndSetArrayItem(new XVar("basicValidate", new XVar(0, "IsRequired")), "validate");
				}
				else
				{
					parameters.InitAndSetArrayItem(this.pSet.getValidationData((XVar)(fName.Value)), "validate");
				}
				controls = XVar.Clone(new XVar("controls", XVar.Array()));
				controls.InitAndSetArrayItem(this.id, "controls", "id");
				controls.InitAndSetArrayItem("add", "controls", "mode");
				controls.InitAndSetArrayItem(0, "controls", "ctrlInd");
				controls.InitAndSetArrayItem(parameters["suggest"], "controls", "suggest");
				controls.InitAndSetArrayItem(fName.Value, "controls", "fieldName");
				this.xt.assign((XVar)(MVCFunctions.Concat(gfName, "_fieldblock")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat(gfName, "_tabfieldblock")), new XVar(true));
				firstElementId = XVar.Clone(this.getControl((XVar)(fName.Value), (XVar)(this.id)).getFirstElementId());
				if(XVar.Pack(firstElementId))
				{
					this.xt.assign((XVar)(MVCFunctions.Concat("labelfor_", MVCFunctions.GoodFieldName((XVar)(fName.Value)))), (XVar)(firstElementId));
				}
				if(this.pSet.getEditFormat((XVar)(fName.Value)) == Constants.EDIT_FORMAT_CHECKBOX)
				{
					parameters.InitAndSetArrayItem(this.xt, "xt");
					parameters.InitAndSetArrayItem(MVCFunctions.Concat(gfName, "_forward_control"), "clearVar");
				}
				this.xt.assign_function((XVar)(MVCFunctions.Concat(gfName, "_editcontrol")), new XVar("xt_buildeditcontrol"), (XVar)(parameters));
				if(this.pSet.getEditFormat((XVar)(fName.Value)) == Constants.EDIT_FORMAT_CHECKBOX)
				{
					parameters.InitAndSetArrayItem(this.xt, "xt");
					parameters.InitAndSetArrayItem(MVCFunctions.Concat(gfName, "_editcontrol"), "clearVar");
					this.xt.assign_function((XVar)(MVCFunctions.Concat(gfName, "_forward_control")), new XVar("xt_buildforwardcontrol"), (XVar)(parameters));
					this.xt.assign((XVar)(MVCFunctions.Concat(gfName, "_label_class")), new XVar("r-checkbox-label"));
				}
				preload = XVar.Clone(this.fillPreload((XVar)(fName.Value), (XVar)(regFields), (XVar)(this.regValues)));
				if(!XVar.Equals(XVar.Pack(preload), XVar.Pack(false)))
				{
					controls.InitAndSetArrayItem(preload, "controls", "preloadData");
				}
				this.fillControlsMap((XVar)(controls));
				this.fillControlFlags((XVar)(fName.Value), (XVar)((XVar)((XVar)(fName.Value == this.usernameFiled)  || (XVar)(fName.Value == Security.passwordField()))  || (XVar)(fName.Value == this.emailFiled)));
				if((XVar)(fName.Value == Security.passwordField())  && (XVar)(Security.passwordField() != this.usernameFiled))
				{
					parameters = XVar.Clone(XVar.Array());
					parameters.InitAndSetArrayItem(this.id, "id");
					parameters.InitAndSetArrayItem("add", "mode");
					parameters.InitAndSetArrayItem("confirm", "field");
					parameters.InitAndSetArrayItem("Password", "format");
					parameters.InitAndSetArrayItem(true, "suggest");
					parameters.InitAndSetArrayItem(this, "pageObj");
					parameters.InitAndSetArrayItem(new XVar("basicValidate", new XVar(0, "IsRequired")), "validate");
					parameters.InitAndSetArrayItem(XVar.Array(), "extraParams");
					parameters.InitAndSetArrayItem(true, "extraParams", "isConfirm");
					parameters.InitAndSetArrayItem(true, "extraParams", "getConrirmFieldCtrl");
					controls = XVar.Clone(new XVar("controls", XVar.Array()));
					controls.InitAndSetArrayItem(this.id, "controls", "id");
					controls.InitAndSetArrayItem("add", "controls", "mode");
					controls.InitAndSetArrayItem(0, "controls", "ctrlInd");
					controls.InitAndSetArrayItem(true, "controls", "suggest");
					controls.InitAndSetArrayItem("confirm", "controls", "fieldName");
					this.xt.assign(new XVar("confirm_label"), new XVar(true));
					if(XVar.Pack(this.is508))
					{
						this.xt.assign_section(new XVar("confirm_label"), (XVar)(MVCFunctions.Concat("<label for=\"value_confirm_", this.id, "\">")), new XVar("</label>"));
					}
					this.xt.assign((XVar)(MVCFunctions.Concat("labelfor_", MVCFunctions.GoodFieldName((XVar)(fName.Value)), "_confirm")), (XVar)(MVCFunctions.Concat("value_confirm_", this.id)));
					this.xt.assign_function(new XVar("confirm_editcontrol1"), new XVar("xt_buildeditcontrol"), (XVar)(parameters));
					this.xt.assign(new XVar("confirm_block"), new XVar(true));
					this.xt.assign(new XVar("confirm_fieldblock"), new XVar(true));
					this.fillControlsMap((XVar)(controls));
					this.fillControlFlags(new XVar("confirm"), new XVar(true));
				}
			}

			return null;
		}
		protected virtual XVar prepareReadonlyFields()
		{
			foreach (KeyValuePair<XVar, dynamic> uf in this.pSet.getPageFields().GetEnumerator())
			{
				if(this.pSet.getEditFormat((XVar)(uf.Value)) == Constants.EDIT_FORMAT_READONLY)
				{
					this.readOnlyFields.InitAndSetArrayItem(this.showDBValue((XVar)(uf.Value), (XVar)(this.regValues)), uf.Value);
				}
			}

			return null;
		}
		public override XVar getCaptchaFieldName()
		{
			return "_register_captcha";
		}
		public override XVar getCaptchaId()
		{
			return "register";
		}
		public virtual XVar setDatabaseError(dynamic _param_messageText)
		{
			#region pass-by-value parameters
			dynamic messageText = XVar.Clone(_param_messageText);
			#endregion

			this.message = XVar.Clone(messageText);

			return null;
		}
		protected virtual XVar doCommonAssignments()
		{
			dynamic addStyle = null;
			this.xt.assign(new XVar("legend"), new XVar(true));
			this.xt.assign(new XVar("buttons_block"), new XVar(true));
			this.xt.assign(new XVar("message_block"), new XVar(true));
			if(XVar.Pack(MVCFunctions.strlen((XVar)(this.message))))
			{
				dynamic messageClass = null;
				messageClass = new XVar("alert-danger");
				if(XVar.Pack(this.registerSuccess))
				{
					messageClass = new XVar("alert-success");
				}
				this.xt.assign(new XVar("message_class"), (XVar)(messageClass));
				this.xt.assign(new XVar("message"), (XVar)(this.message));
			}
			else
			{
				this.hideElement(new XVar("message"));
			}
			addStyle = new XVar("");
			if(XVar.Pack(this.isMultistepped()))
			{
				addStyle = new XVar(" style=\"display: none;\"");
			}
			this.xt.assign(new XVar("submit_attrs"), (XVar)(MVCFunctions.Concat("id=\"saveButton", this.id, "\"", addStyle)));
			if((XVar)(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink")))  && (XVar)(this.registerSuccess))
			{
				this.xt.assign(new XVar("firstAboveGridCell"), new XVar(true));
				this.xt.assign(new XVar("email"), (XVar)(this.strEmail));
				this.xt.assign(new XVar("activation_block"), new XVar(true));
				this.xt.assign(new XVar("activate_message_class"), (XVar)((XVar.Pack(this.sendActivationLinkFailedMessage) ? XVar.Pack("alert-danger") : XVar.Pack("alert-success"))));
				foreach (KeyValuePair<XVar, dynamic> mLString in this.pSet.activatonMessages().GetEnumerator())
				{
					dynamic label = null;
					if(XVar.Pack(this.sendActivationLinkFailedMessage))
					{
						label = XVar.Clone(MVCFunctions.Concat("Error sending email.", this.sendActivationLinkFailedMessage));
					}
					else
					{
						label = XVar.Clone(MVCFunctions.str_replace(new XVar("%email%"), (XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(this.strEmail))), (XVar)(CommonFunctions.GetMLString((XVar)(mLString.Value)))));
					}
					this.xt.assign((XVar)(MVCFunctions.Concat("label_", mLString.Key)), (XVar)(label));
				}
			}
			if(XVar.Pack(this.registerSuccess))
			{
				dynamic continueUrl = null;
				this.xt.assign(new XVar("registered_block"), new XVar(true));
				continueUrl = XVar.Clone(MVCFunctions.GetTableLink(new XVar("menu")));
				if(XVar.Equals(XVar.Pack(Security.userSessionLevel()), XVar.Pack(Constants.LOGGED_2FSETUP_PENDING)))
				{
					continueUrl = XVar.Clone(MVCFunctions.GetTableLink(new XVar("userinfo")));
				}
				this.xt.assign(new XVar("loginlink_attrs"), (XVar)(MVCFunctions.Concat("href=\"", continueUrl, "\" id=\"ProceedToLogin\"")));
				if(this.mode == Constants.REGISTER_POPUP)
				{
					this.xt.assign(new XVar("close_win_btn"), new XVar(true));
					this.xt.assign(new XVar("closewindow_attrs"), new XVar("id=\"closeWindowRegister\""));
				}
			}
			if(this.mode == Constants.REGISTER_POPUP)
			{
				this.xt.assign(new XVar("backlink_attrs"), new XVar("style=\"display:none\""));
			}
			if(this.mode == Constants.REGISTER_SIMPLE)
			{
				this.assignBody();
			}

			return null;
		}
		protected override XVar assignBody()
		{
			if((XVar)(this.registerSuccess)  && (XVar)(!(XVar)(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink")))))
			{
				this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage(new XVar(false)), "<form method=\"POST\" action=\"", MVCFunctions.GetTableLink(new XVar("login")), "\" name=\"loginform\">\r\n\t\t\t\t\t<input type=\"Hidden\" name=username value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(this.strUsername)), "\">", "</form>");
				this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
				this.xt.assign(new XVar("body"), (XVar)(this.body));
				return null;
			}
			base.assignBody();

			return null;
		}
		protected virtual XVar showPage()
		{
			if(XVar.Pack(this.registerSuccess))
			{
				this.switchToSuccessPage();
				this.bodyForms = XVar.Clone(new XVar(0, "above-grid", 1, "grid"));
				if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
				{
					this.hideItemType(new XVar("register_proceed"));
					this.hideItemType(new XVar("register_activated_message"));
				}
			}
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeShowRegister"))))
			{
				GlobalVars.globalEvents.BeforeShowRegister((XVar)(this.xt), ref this.templatefile, this);
			}
			if(this.mode == Constants.REGISTER_POPUP)
			{
				this.xt.assign(new XVar("footer"), new XVar(false));
				this.xt.assign(new XVar("header"), new XVar(false));
				this.xt.assign(new XVar("body"), (XVar)(this.body));
				this.displayAJAX((XVar)(this.templatefile), (XVar)(this.id + 1));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			this.display((XVar)(this.templatefile));
			return null;
		}
		public static XVar readRegisterModeFromRequest()
		{
			if(MVCFunctions.postvalue(new XVar("onFly")) == 1)
			{
				return Constants.REGISTER_POPUP;
			}
			return Constants.REGISTER_SIMPLE;
		}
		public static XVar readActionFromRequest()
		{
			if(XVar.Pack(MVCFunctions.postvalue("btnSubmit")))
			{
				return MVCFunctions.postvalue("btnSubmit");
			}
			return MVCFunctions.postvalue(new XVar("a"));
		}
		public override XVar element2Item(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(name == "message")
			{
				return new XVar(0, "register_message");
			}
			return base.element2Item((XVar)(name));
		}
		public virtual XVar checkDeniedDuplicated(dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			#endregion

			dynamic ret = null, usermessage = null;
			usermessage = new XVar("");
			ret = XVar.Clone(this.hasDeniedDuplicateValues((XVar)(values), ref usermessage));
			if(XVar.Pack(ret))
			{
				this.message = XVar.Clone(usermessage);
			}
			return !(XVar)(ret);
		}
		protected virtual XVar tryLoginNewUser()
		{
			dynamic userData = XVar.Array(), username = null;
			if(XVar.Pack(!(XVar)(this.registerSuccess)))
			{
				return false;
			}
			userData = XVar.Clone(Security.fetchUserData((XVar)(this.strUsername), new XVar(""), new XVar(true)));
			if(XVar.Pack(!(XVar)(userData)))
			{
				return false;
			}
			username = XVar.Clone(userData[Security.usernameField()]);
			if((XVar)(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink")))  && (XVar)(userData[ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("activationField"))] != 1))
			{
				Security.createProvisionalSession((XVar)(Security.dbProvider()), new XVar(Constants.LOGGED_ACTIVATION_PENDING), (XVar)(username), (XVar)(userData[Security.fullnameField()]), (XVar)(userData));
			}
			else
			{
				dynamic twoSettings = XVar.Array();
				twoSettings = Security.twoFactorSettings();
				if((XVar)(twoSettings["enable"])  || (XVar)(twoSettings["required"]))
				{
					Security.createProvisionalSession((XVar)(Security.dbProvider()), new XVar(Constants.LOGGED_2FSETUP_PENDING), (XVar)(username), (XVar)(userData[Security.fullnameField()]), (XVar)(userData));
				}
				else
				{
					Security.createUserSession((XVar)(Security.dbProvider()), (XVar)(username), (XVar)(userData[Security.fullnameField()]), (XVar)(userData));
					Security.auditLoginSuccess();
					Security.callAfterLogin();
				}
			}
			return true;
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
			if(XVar.Pack(this.userErrorMessage))
			{
				settings.InitAndSetArrayItem(this.userErrorMessage, "msg_userError");
			}
			if(XVar.Pack(this.emailErrorMessage))
			{
				settings.InitAndSetArrayItem(this.emailErrorMessage, "msg_emailError");
			}
			if(XVar.Pack(this.pwdErrorMessage))
			{
				settings.InitAndSetArrayItem(this.pwdErrorMessage, "msg_passwordError");
			}
			foreach (KeyValuePair<XVar, dynamic> fName in this.pSet.getPageFields().GetEnumerator())
			{
				if(fName.Value == Security.passwordField())
				{
					settings.InitAndSetArrayItem(fName.Value, "passFieldName");
				}
				if(fName.Value == this.usernameFiled)
				{
					settings.InitAndSetArrayItem(fName.Value, "userFieldName");
				}
				if(fName.Value == this.emailFiled)
				{
					settings.InitAndSetArrayItem(fName.Value, "emailFieldName");
				}
			}
			return settings;
		}
	}
}
