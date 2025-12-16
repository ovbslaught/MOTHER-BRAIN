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
	public partial class LoginPage : RunnerPage
	{
		public dynamic auditObj = XVar.Pack(null);
		public dynamic rememberPassword = XVar.Pack(0);
		public dynamic rememberMachine = XVar.Pack(0);
		public dynamic var_pUsername = XVar.Pack("");
		public dynamic var_pPassword = XVar.Pack("");
		public dynamic action = XVar.Pack("");
		public dynamic twoFactorMethod = XVar.Pack(-1);
		public dynamic redirectAfterLogin = XVar.Pack(false);
		protected dynamic myurl = XVar.Pack("");
		public dynamic loggedWithSP = XVar.Pack(false);
		protected dynamic loggedByCredentials = XVar.Pack(false);
		public dynamic messageType = XVar.Pack(Constants.MESSAGE_ERROR);
		public dynamic twoFactorCode;
		protected dynamic SMSCodeSent = XVar.Pack(false);
		protected dynamic pnoneNotInQuery = XVar.Pack(false);
		protected dynamic skipSecondStep = XVar.Pack(false);
		protected dynamic securityPlugins;
		protected dynamic controlsData = XVar.Array();
		public dynamic restore = XVar.Pack("");
		public dynamic providerCode = XVar.Pack("");
		protected static bool skipLoginPageCtor = false;
		public LoginPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipLoginPageCtor)
			{
				skipLoginPageCtor = false;
				return;
			}
			this.pSetEdit = XVar.UnPackProjectSettings(this.pSet);
			this.auditObj = XVar.Clone(CommonFunctions.GetAuditObject());
			this.headerForms = XVar.Clone(new XVar(0, "top"));
			this.footerForms = XVar.Clone(new XVar(0, "footer"));
			this.bodyForms = XVar.Clone(new XVar(0, "above-grid", 1, "grid", 2, "superbottom"));
			this.initMyURL();
			this.body = XVar.Clone(new XVar("begin", "", "end", ""));
			this.initSecurityPlugins();
		}
		protected virtual XVar initSecurityPlugins()
		{
			this.securityPlugins = XVar.Clone(Security.GetPlugins());
			this.addJSSettingsForSP();

			return null;
		}
		protected virtual XVar addJSSettingsForSP()
		{
			foreach (KeyValuePair<XVar, dynamic> sp in this.securityPlugins.GetEnumerator())
			{
				dynamic settings = XVar.Array();
				settings = XVar.Clone(sp.Value.getJSSettings());
				foreach (KeyValuePair<XVar, dynamic> s in settings.GetEnumerator())
				{
					this.pageData.InitAndSetArrayItem(s.Value, s.Key);
				}
			}

			return null;
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
		public virtual XVar process()
		{
			dynamic sessionLevel = null, twoFactorSettings = XVar.Array();
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeProcessLogin"))))
			{
				GlobalVars.globalEvents.BeforeProcessLogin(this);
			}
			twoFactorSettings = Security.twoFactorSettings();
			if((XVar)(!(XVar)(ProjectSettings.getProjectValue(new XVar("keepLoggedIn"))))  || (XVar)(twoFactorSettings["available"]))
			{
				this.hideItemType(new XVar("remember_password"));
			}
			sessionLevel = XVar.Clone(Security.userSessionLevel());
			if(this.action == "")
			{
				if((XVar)(!XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_FULL)))  && (XVar)(!XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_NONE))))
				{
					Security.clearSecuritySession();
					sessionLevel = XVar.Clone(Security.userSessionLevel());
				}
			}
			if((XVar)((XVar)(this.action == "change2f")  || (XVar)(this.action == "resendCode"))  && (XVar)(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_2F_PENDING))))
			{
				this.sendTwoFactorCode();
			}
			if((XVar)(this.action == "verifyCode")  && (XVar)(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_2F_PENDING))))
			{
				this.verifyTwoFactorCode((XVar)(this.twoFactorCode));
			}
			if((XVar)(this.action == "resendActivation")  && (XVar)(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_ACTIVATION_PENDING))))
			{
				this.resendActivation();
			}
			if(this.action == "logout")
			{
				this.Logout(new XVar(true));
				return null;
			}
			if(this.action == "providerlogin")
			{
				dynamic provider = null;
				provider = XVar.Clone(Security.getAuthPlugin((XVar)(this.providerCode)));
				if(XVar.Pack(provider))
				{
					provider.redirectToLogin();
					MVCFunctions.ob_flush();
					HttpContext.Current.Response.End();
					throw new RunnerInlineOutputException();
				}
				return null;
			}
			if(this.action == "pluginLogin")
			{
				dynamic plugin = null;
				plugin = XVar.Clone(this.securityPlugins[MVCFunctions.postvalue(new XVar("plugin"))]);
				if(XVar.Pack(plugin))
				{
					this.LoginWithSP((XVar)(plugin), (XVar)(MVCFunctions.postvalue(new XVar("plugin_token"))));
				}
			}
			this.refineMessage();
			this.readControls();
			if(XVar.Pack(this.isActionSubmit()))
			{
				this.doLoginRoutine();
				if(XVar.Equals(XVar.Pack(Security.userSessionLevel()), XVar.Pack(Constants.LOGGED_2F_PENDING)))
				{
					this.sendTwoFactorCode();
				}
			}
			this.prepareEditControls();
			sessionLevel = XVar.Clone(Security.userSessionLevel());
			if((XVar)(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_FULL)))  && (XVar)(!(XVar)(Security.isGuest())))
			{
				if(this.mode == Constants.LOGIN_POPUP)
				{
					if(XVar.Pack(!(XVar)(this.restore)))
					{
						this.pageData.InitAndSetArrayItem(this.getLoggedInRedirectUrl(), "redirectUrl");
					}
				}
				else
				{
					this.redirectAfterSuccessfulLogin();
					return null;
				}
			}
			else
			{
				if(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_2F_PENDING)))
				{
					this.hideMainLoginItems();
					this.prepareTwoFactorMessage();
					this.prepareTwoFactorAlternatives();
					this.xt.assign(new XVar("twofactor_code_message"), new XVar(true));
					this.pageData.InitAndSetArrayItem(this.get2FactorMethod(), "twoFactorMethod");
				}
				else
				{
					if(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_ACTIVATION_PENDING)))
					{
						this.hideTwoFactorItems();
						if(this.action != "resendActivation")
						{
							this.prepareActivationMessage();
						}
					}
					else
					{
						if(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_2FSETUP_PENDING)))
						{
							if(this.mode == Constants.LOGIN_POPUP)
							{
								this.pageData.InitAndSetArrayItem(MVCFunctions.GetTableLink(new XVar("userinfo")), "redirectUrl");
							}
							else
							{
								this.redirectToUserInfo();
								return null;
							}
						}
						else
						{
							this.hideTwoFactorItems();
							if(this.mode == Constants.LOGIN_SIMPLE)
							{
								if((XVar)(!(XVar)(CommonFunctions.isLogged()))  || (XVar)(Security.isGuest()))
								{
									Security.tryRelogin();
								}
								if((XVar)(CommonFunctions.isLogged())  && (XVar)(!(XVar)(Security.isGuest())))
								{
									MVCFunctions.HeaderRedirect(new XVar("menu"));
									return null;
								}
							}
						}
					}
				}
			}
			XSession.Session["MyURL"] = this.myurl;
			this.assignFieldBlocksAndLabels();
			if(XVar.Pack(this.captchaExists()))
			{
				this.displayCaptcha();
			}
			this.addCommonJs();
			this.addButtonHandlers();
			this.fillSetCntrlMaps();
			this.doCommonAssignments();
			this.showPage();

			return null;
		}
		protected virtual XVar hideMainLoginItems()
		{
			this.hideItemType(new XVar("login_button"));
			this.hideItemType(new XVar("username"));
			this.hideItemType(new XVar("password"));
			this.hideItemType(new XVar("username_label"));
			this.hideItemType(new XVar("password_label"));
			this.hideItemType(new XVar("guest_login"));
			this.hideItemType(new XVar("remember_password"));
			this.hideItemType(new XVar("login_external"));
			this.hideItemType(new XVar("login_provider"));
			this.hideElement(new XVar("bsloginregister"));

			return null;
		}
		protected virtual XVar hideTwoFactorItems()
		{
			this.hideItemType(new XVar("auth_code"));
			this.hideItemType(new XVar("verify_button"));
			this.hideItemType(new XVar("resend_button"));
			this.hideItemType(new XVar("remember_machine"));
			this.hideItemType(new XVar("twofactor_message"));

			return null;
		}
		protected virtual XVar isActionSubmit()
		{
			return this.action == "Login";
		}
		protected virtual XVar doLoginRoutine()
		{
			dynamic sessionLevel = null, userFound = null;
			if(XVar.Pack(!(XVar)(this.isLoginAccessAllowed())))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(this.checkCaptcha())))
			{
				return false;
			}
			this.readControls();
			if(XVar.Pack(!(XVar)(this.callBeforeLoginEvent())))
			{
				return false;
			}
			userFound = XVar.Clone(this.createSession((XVar)(this.var_pUsername), (XVar)(this.var_pPassword)));
			if(XVar.Pack(!(XVar)(userFound)))
			{
				Security.auditLoginFail((XVar)(this.var_pUsername));
				this.callAfterUnsuccessfulLoginEvent((XVar)(this.var_pUsername), (XVar)(this.var_pPassword));
				return false;
			}
			sessionLevel = XVar.Clone(Security.userSessionLevel());
			if(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_2F_PENDING)))
			{
				if(XVar.Pack(this.verifyRememberMachineCookie((XVar)(MVCFunctions.GetCookie("2ftoken")))))
				{
					Security.elevateSession();
				}
				sessionLevel = XVar.Clone(Security.userSessionLevel());
			}
			if((XVar)(XVar.Equals(XVar.Pack(sessionLevel), XVar.Pack(Constants.LOGGED_FULL)))  && (XVar)(!(XVar)(Security.isGuest())))
			{
				if(XVar.Pack(this.rememberPassword))
				{
					Security.setKeepLoggedCookie(new XVar(true));
				}
				else
				{
					Security.setKeepLoggedCookie(new XVar(false));
				}
				Security.auditLoginSuccess();
				this.callAfterSuccessfulLoginEvent((XVar)(this.var_pUsername), (XVar)(this.var_pPassword), (XVar)(Security.currentUserData()));
			}
			return true;
		}
		protected virtual XVar doAssignForSecondAuthStep(dynamic _param_codeSent)
		{
			#region pass-by-value parameters
			dynamic codeSent = XVar.Clone(_param_codeSent);
			#endregion

			this.xt.assign(new XVar("user_code"), new XVar(true));
			if(XVar.Pack(!(XVar)(codeSent)))
			{
				this.xt.assign(new XVar("userCodeFieldClass"), new XVar("rnr-hiddenblock"));
			}
			this.xt.assign(new XVar("user_code_buttons"), new XVar(true));
			if(XVar.Pack(!(XVar)(codeSent)))
			{
				this.xt.assign(new XVar("verifyButtonClass"), new XVar("rnr-invisible-button"));
			}
			if(XVar.Pack(this.pnoneNotInQuery))
			{
				this.xt.assign(new XVar("resendButtonClass"), new XVar("rnr-invisible-button"));
			}

			return null;
		}
		protected virtual XVar prepareTwoFactorAlternatives()
		{
			dynamic altMethods = XVar.Array(), method = null, twoValue = null;
			method = XVar.Clone(this.get2FactorMethod());
			twoValue = XVar.Clone(this.twoFactorValue());
			altMethods = XVar.Clone(Security.twoFactorEnabledMethods((XVar)(twoValue)));
			foreach (KeyValuePair<XVar, dynamic> dummy in altMethods.GetEnumerator())
			{
				dynamic tag = null;
				if(dummy.Key == method)
				{
					continue;
				}
				tag = new XVar("twofactor_altemail");
				if(dummy.Key == Constants.TWOFACTOR_APP)
				{
					tag = new XVar("twofactor_altapp");
				}
				else
				{
					if(dummy.Key == Constants.TWOFACTOR_PHONE)
					{
						tag = new XVar("twofactor_altphone");
					}
				}
				this.xt.assign((XVar)(tag), new XVar(true));
			}
			if(1 < MVCFunctions.count(altMethods))
			{
				this.xt.assign(new XVar("twofactor_alt_message"), new XVar(true));
			}

			return null;
		}
		protected virtual XVar twoFactorValue()
		{
			dynamic twofSettings = XVar.Array(), userdata = XVar.Array();
			userdata = XVar.Clone(Security.provisionalUserData());
			twofSettings = Security.twoFactorSettings();
			return userdata[twofSettings["twoFactorField"]];
		}
		protected virtual XVar prepareTwoFactorMessage()
		{
			dynamic destination = XVar.Array(), method = null, twofMessage = null;
			method = XVar.Clone(this.get2FactorMethod());
			destination = XVar.Clone(Security.twoFactorDeliveryInfo((XVar)(Security.provisionalUserData()), (XVar)(method)));
			twofMessage = new XVar("");
			if(destination["method"] == Constants.TWOFACTOR_PHONE)
			{
				twofMessage = XVar.Clone(MVCFunctions.str_replace(new XVar("%phone%"), (XVar)(destination["address"]), (XVar)(CommonFunctions.mlang_message(new XVar("MESSAGE_SENT_TO_PNONE")))));
			}
			else
			{
				if(destination["method"] == Constants.TWOFACTOR_EMAIL)
				{
					twofMessage = XVar.Clone(MVCFunctions.str_replace(new XVar("%email%"), (XVar)(destination["address"]), (XVar)(CommonFunctions.mlang_message(new XVar("MESSAGE_SENT_TO_EMAIL")))));
				}
				else
				{
					if(destination["method"] == Constants.TWOFACTOR_APP)
					{
						twofMessage = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "%username%", 1, "%site%")), (XVar)(new XVar(0, MVCFunctions.Concat("<br><b>", Security.provisionalUsername(), "</b>"), 1, MVCFunctions.Concat("<b>", destination["address"], "</b>"))), (XVar)(CommonFunctions.mlang_message(new XVar("ENTER_TOTP_CODE")))));
						this.hideItemType(new XVar("resend_button"));
					}
				}
			}
			this.xt.assign(new XVar("twofactor_message"), (XVar)(twofMessage));

			return null;
		}
		protected virtual XVar sendTwoFactorCode()
		{
			dynamic method = null, ret = XVar.Array();
			method = XVar.Clone(this.get2FactorMethod());
			ret = XVar.Clone(Security.generateAndSendTwoFactorCode((XVar)(method)));
			if(XVar.Pack(!(XVar)(ret)))
			{
				return true;
			}
			if(XVar.Pack(!(XVar)(ret["success"])))
			{
				this.message = XVar.Clone(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("ERR_SENDING_CODE")), " ", ret["message"]));
				this.messageType = new XVar(Constants.MESSAGE_ERROR);
				return false;
			}

			return null;
		}
		protected virtual XVar get2FactorMethod()
		{
			dynamic method = null, value = null;
			method = XVar.Clone(this.twoFactorMethod);
			value = XVar.Clone(this.twoFactorValue());
			if(XVar.Pack(Security.twoFactorMethodEnabled((XVar)(value), (XVar)(method))))
			{
				return method;
			}
			return Security.twoFactorPreferredMethod((XVar)(value));
		}
		protected virtual XVar verifyTwoFactorCode(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			if(XVar.Pack(Security.checkTwoFactorCode((XVar)(code))))
			{
				Security.elevateSession();
				this.setRememberMachineCookie(new XVar(true));
				this.saveTwoFactorValue();
				Security.auditLoginSuccess();
				this.callAfterSuccessfulLoginEvent((XVar)(Security.getUserName()), new XVar(""), (XVar)(Security.currentUserData()));
			}
			else
			{
				this.setRememberMachineCookie(new XVar(false));
				this.message = XVar.Clone(CommonFunctions.mlang_message(new XVar("WRONG_CODE")));
			}

			return null;
		}
		protected virtual XVar prepareActivationMessage()
		{
			this.xt.assign(new XVar("resend_activation_button"), new XVar(true));
			this.message = XVar.Clone(CommonFunctions.mlang_message(new XVar("LOGIN_USER_NOT_ACTIVATED")));
			this.messageType = new XVar(Constants.MESSAGE_ERROR);

			return null;
		}
		protected virtual XVar getLoggedInRedirectUrl()
		{
			if(XVar.Pack(this.myurl))
			{
				return MVCFunctions.Concat(this.myurl, (XVar.Pack(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(this.myurl), new XVar("?"))), XVar.Pack(false))) ? XVar.Pack("&a=login") : XVar.Pack("?a=login")));
			}
			return MVCFunctions.GetTableLink(new XVar("menu"));
		}
		protected virtual XVar refineMessage()
		{
			if(this.message == "expired")
			{
				this.message = XVar.Clone(MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("SESSION_EXPIRED1")), " ", CommonFunctions.mlang_message(new XVar("SESSION_EXPIRED2"))));
			}
			else
			{
				if(this.message == "invalidlogin")
				{
					this.message = XVar.Clone(CommonFunctions.mlang_message(new XVar("INVALID_LOGIN")));
				}
				else
				{
					if((XVar)(this.message == "loginblocked")  && (XVar)(MVCFunctions.strlen((XVar)(XSession.Session["loginBlockMessage"]))))
					{
						this.message = XVar.Clone(XSession.Session["loginBlockMessage"]);
					}
				}
			}
			if(XVar.Pack(this.message))
			{
				this.xt.assign(new XVar("message_class"), new XVar("alert-danger"));
			}
			XSession.Session.Remove("loginBlockMessage");

			return null;
		}
		protected virtual XVar initMyURL()
		{
			this.myurl = XVar.Clone(XSession.Session["MyURL"]);
			if((XVar)((XVar)((XVar)(this.redirectAfterLogin)  || (XVar)(this.mode == Constants.LOGIN_POPUP))  || (XVar)(this.action == "Login"))  || (XVar)(XVar.Equals(XVar.Pack(Security.userSessionLevel()), XVar.Pack(Constants.LOGGED_2F_PENDING))))
			{
			}
			else
			{
				this.myurl = new XVar("");
			}

			return null;
		}
		protected virtual XVar callAfterSuccessfulLoginEvent(dynamic _param_username, dynamic _param_password = null, dynamic _param_userData = null)
		{
			#region default values
			if(_param_password as Object == null) _param_password = new XVar("");
			if(_param_userData as Object == null) _param_userData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			dynamic userData = XVar.Clone(_param_userData);
			#endregion

			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterSuccessfulLogin"))))
			{
				GlobalVars.globalEvents.AfterSuccessfulLogin((XVar)(username), (XVar)(password), (XVar)(userData), this);
			}

			return null;
		}
		public virtual XVar redirectAfterSuccessfulLogin()
		{
			XSession.Session.Remove("MyURL");
			if(XVar.Pack(this.myurl))
			{
				MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", this.myurl)));
			}
			else
			{
				MVCFunctions.HeaderRedirect(new XVar("menu"));
			}

			return null;
		}
		protected virtual XVar redirectToUserInfo()
		{
			MVCFunctions.HeaderRedirect(new XVar("userinfo"));

			return null;
		}
		public virtual XVar callAfterUnsuccessfulLoginEvent(dynamic _param_username, dynamic _param_password = null)
		{
			#region default values
			if(_param_password as Object == null) _param_password = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			#endregion

			dynamic message = null;
			message = new XVar("");
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterUnsuccessfulLogin"))))
			{
				GlobalVars.globalEvents.AfterUnsuccessfulLogin((XVar)(username), (XVar)(password), ref message, this, (XVar)(this.controlsData));
			}
			if((XVar)(message == XVar.Pack(""))  && (XVar)(!(XVar)(this.message)))
			{
				this.message = XVar.Clone(CommonFunctions.mlang_message(new XVar("INVALID_LOGIN")));
			}
			else
			{
				if(XVar.Pack(message))
				{
					this.message = XVar.Clone(message);
				}
			}

			return null;
		}
		protected virtual XVar isLoginAccessAllowed()
		{
			if(XVar.Pack(!(XVar)(this.auditObj)))
			{
				return true;
			}
			if(XVar.Pack(!(XVar)(this.auditObj.LoginAccess())))
			{
				return true;
			}
			this.message = XVar.Clone(MVCFunctions.mysprintf((XVar)(CommonFunctions.mlang_message(new XVar("LOGIN_BLOCKED"))), (XVar)(new XVar(0, this.auditObj.LoginAccess()))));
			XSession.Session["loginBlockMessage"] = this.message;
			return false;
		}
		protected virtual XVar callBeforeLoginEvent(dynamic _param_data = null)
		{
			#region default values
			if(_param_data as Object == null) _param_data = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic message = null, ret = null;
			if(XVar.Pack(!(XVar)(GlobalVars.globalEvents.exists(new XVar("BeforeLogin")))))
			{
				return true;
			}
			message = new XVar("");
			if(XVar.Pack(!(XVar)(data)))
			{
				data = this.controlsData;
			}
			if(XVar.Pack(!(XVar)(data)))
			{
				data = XVar.Clone(XVar.Array());
			}
			ret = XVar.Clone(GlobalVars.globalEvents.BeforeLogin(ref this.var_pUsername, ref this.var_pPassword, ref message, this, (XVar)(data)));
			if(XVar.Pack(message))
			{
				this.message = XVar.Clone(message);
			}
			return ret;
		}
		protected virtual XVar logInHardcoded(dynamic _param_username, dynamic _param_password)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			#endregion

			if(XVar.Pack(Security.verifyHardcodedLogin((XVar)(username), (XVar)(password))))
			{
				Security.createHardcodedSession();
				return true;
			}
			return false;
		}
		public virtual XVar doAfterUnsuccessfulLog(dynamic _param_username)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			#endregion

			if(XVar.Pack(this.auditObj))
			{
				this.auditObj.LogLoginFailed((XVar)(username));
				this.auditObj.LoginUnsuccessful((XVar)(username));
			}

			return null;
		}
		protected virtual XVar createDBSesssion(dynamic _param_provider, dynamic _param_username, dynamic _param_password)
		{
			#region pass-by-value parameters
			dynamic provider = XVar.Clone(_param_provider);
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			#endregion

			dynamic data = XVar.Array(), displayName = null, twoFPendingLevel = null;
			data = XVar.Clone(Security.fetchUserData((XVar)(username), (XVar)(password), new XVar(false), (XVar)(this.controlsData)));
			if(XVar.Pack(!(XVar)(data)))
			{
				return false;
			}
			displayName = XVar.Clone(data[Security.fullnameField()]);
			username = XVar.Clone(data[Security.usernameField()]);
			if(displayName == XVar.Pack(""))
			{
				displayName = XVar.Clone(username);
			}
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				if(data[ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("activationField"))] != 1)
				{
					Security.createProvisionalSession((XVar)(provider), new XVar(Constants.LOGGED_ACTIVATION_PENDING), (XVar)(username), (XVar)(displayName), (XVar)(data));
					return true;
				}
			}
			twoFPendingLevel = XVar.Clone(Security.twoFactorPendingLevel((XVar)(data)));
			if(twoFPendingLevel != Constants.LOGGED_FULL)
			{
				Security.createProvisionalSession((XVar)(provider), (XVar)(twoFPendingLevel), (XVar)(username), (XVar)(displayName), (XVar)(data));
				return true;
			}
			Security.createUserSession((XVar)(provider), (XVar)(username), (XVar)(displayName), (XVar)(data));
			return true;
		}
		protected virtual XVar createADSesssion(dynamic _param_provider, dynamic _param_username, dynamic _param_password)
		{
			#region pass-by-value parameters
			dynamic provider = XVar.Clone(_param_provider);
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			#endregion

			dynamic plugin = null, sessionLevel = null, userInfo = XVar.Array();
			plugin = XVar.Clone(Security.getAuthPlugin((XVar)(provider["code"])));
			userInfo = XVar.Clone(plugin.login((XVar)(username), (XVar)(password)));
			if(XVar.Pack(!(XVar)(userInfo)))
			{
				this.message = XVar.Clone(plugin.getError());
				return false;
			}
			sessionLevel = new XVar(Constants.LOGGED_FULL);
			if(XVar.Pack(Security.providerInDb((XVar)(provider["code"]))))
			{
				dynamic fetchResult = XVar.Array();
				fetchResult = XVar.Clone(Security.fetchUpdateDatabaseUser((XVar)(userInfo["userdata"]), new XVar(true)));
				if(XVar.Pack(fetchResult["data"]))
				{
					sessionLevel = XVar.Clone(Security.twoFactorPendingLevel((XVar)(fetchResult["data"])));
				}
			}
			return plugin.createUserSession((XVar)(userInfo), new XVar(false), (XVar)(sessionLevel));
		}
		public virtual XVar createSession(dynamic _param_username, dynamic _param_password)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			#endregion

			dynamic provider = XVar.Array();
			provider = XVar.Clone(this.currentProvider());
			if(XVar.Pack(!(XVar)(provider)))
			{
				return false;
			}
			if(provider["type"] == Constants.stHARDCODED)
			{
				return this.logInHardcoded((XVar)(username), (XVar)(password));
			}
			if(provider["type"] == Constants.stDB)
			{
				return this.createDBSesssion((XVar)(provider), (XVar)(username), (XVar)(password));
			}
			if(provider["type"] == Constants.stAD)
			{
				return this.createADSesssion((XVar)(provider), (XVar)(username), (XVar)(password));
			}
			return true;
		}
		public virtual XVar Logout(dynamic _param_redirectToLogin = null)
		{
			#region default values
			if(_param_redirectToLogin as Object == null) _param_redirectToLogin = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic redirectToLogin = XVar.Clone(_param_redirectToLogin);
			#endregion

			dynamic authPlugin = null, logoutToken = null, username = null;
			if(XVar.Pack(this.auditObj))
			{
				this.auditObj.LogLogout();
			}
			username = XVar.Clone((XVar.Pack(Security.isGuest()) ? XVar.Pack("") : XVar.Pack(Security.getUserName())));
			logoutToken = XVar.Clone(CommonFunctions.storageGet(new XVar("logout_token_hint")));
			XSession.Session.Remove("MyURL");
			Security.clearSecuritySession();
			foreach (KeyValuePair<XVar, dynamic> sp in this.securityPlugins.GetEnumerator())
			{
				sp.Value.onLogout();
			}
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterLogout"))))
			{
				GlobalVars.globalEvents.AfterLogout((XVar)(username));
			}
			authPlugin = XVar.Clone(Security.getAuthPlugin((XVar)(this.providerCode)));
			if((XVar)(authPlugin != null)  && (XVar)(authPlugin.hasExternalLogout()))
			{
				dynamic extRedirectUri = null;
				extRedirectUri = XVar.Clone(MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("login"))));
				authPlugin.redirectToLogout((XVar)(logoutToken), (XVar)(extRedirectUri));
			}
			if(XVar.Pack(redirectToLogin))
			{
				MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", MVCFunctions.GetTableLink(new XVar("login")))));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}

			return null;
		}
		public virtual XVar LogoutAndRedirect(dynamic _param_url = null)
		{
			#region default values
			if(_param_url as Object == null) _param_url = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic url = XVar.Clone(_param_url);
			#endregion

			this.Logout();
			if(url == XVar.Pack(""))
			{
				url = XVar.Clone(MVCFunctions.GetTableLink(new XVar("menu")));
			}
			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", url)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar pluginUserCommand(dynamic _param_externalId)
		{
			#region pass-by-value parameters
			dynamic externalId = XVar.Clone(_param_externalId);
			#endregion

			dynamic externalIdField = null;
			externalIdField = XVar.Clone(Security.extIdField());
			if(XVar.Pack(externalIdField))
			{
				dynamic commands = XVar.Array(), dc = null;
				dc = XVar.Clone(new DsCommand());
				dc.filter = XVar.Clone(DataCondition.FieldEquals((XVar)(externalIdField), (XVar)(externalId)));
				commands.InitAndSetArrayItem(dc, null);
			}

			return null;
		}
		public virtual XVar LoginWithSP(dynamic _param_plugin, dynamic _param_token, dynamic _param_addNewUser = null)
		{
			#region default values
			if(_param_addNewUser as Object == null) _param_addNewUser = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic plugin = XVar.Clone(_param_plugin);
			dynamic token = XVar.Clone(_param_token);
			dynamic addNewUser = XVar.Clone(_param_addNewUser);
			#endregion

			dynamic data = XVar.Array(), info = XVar.Array(), provider = null;
			provider = XVar.Clone(Security.findProvider((XVar)(plugin.code)));
			info = XVar.Clone(plugin.getUserInfo((XVar)(token)));
			if(XVar.Pack(!(XVar)(info)))
			{
				this.message = XVar.Clone(plugin.getError());
				return false;
			}
			if(XVar.Pack(!(XVar)(this.callBeforeLoginEvent((XVar)(info)))))
			{
				return false;
			}
			if(XVar.Pack(Security.extIdField()))
			{
				dynamic dbUserData = XVar.Array();
				dbUserData = XVar.Clone(Security.fetchUpdateDatabaseUser((XVar)(info), (XVar)(addNewUser)));
				data = XVar.Clone(dbUserData["data"]);
				if(XVar.Pack(!(XVar)(data)))
				{
					this.message = XVar.Clone((XVar.Pack(dbUserData["errorMessage"]) ? XVar.Pack(dbUserData["errorMessage"]) : XVar.Pack("Database error")));
					return false;
				}
			}
			else
			{
				data = XVar.Clone(XVar.Array());
				data.InitAndSetArrayItem(info["email"], (XVar.Pack(Security.emailField()) ? XVar.Pack(Security.emailField()) : XVar.Pack("email")));
				data.InitAndSetArrayItem(info["name"], (XVar.Pack(Security.fullnameField()) ? XVar.Pack(Security.fullnameField()) : XVar.Pack("name")));
				data.InitAndSetArrayItem(info["picture"], (XVar.Pack(Security.userpicField()) ? XVar.Pack(Security.userpicField()) : XVar.Pack("picture")));
			}
			CommonFunctions.storageSet(new XVar("logout_token_hint"), (XVar)(token));
			CommonFunctions.storageSet(new XVar("rawUserData"), (XVar)(info["raw"]));
			plugin.saveStorageData();
			this.loggedWithSP = new XVar(true);
			Security.createUserSession((XVar)(provider), (XVar)(info["id"]), (XVar)(info["name"]), (XVar)(data));
			Security.auditLoginSuccess();
			this.callAfterSuccessfulLoginEvent((XVar)(info["id"]), (XVar)(data[Security.passwordField()]), (XVar)(data));
			return true;
		}
		public override XVar getCaptchaId()
		{
			return "login";
		}
		public virtual XVar setDatabaseError(dynamic _param_messageText)
		{
			#region pass-by-value parameters
			dynamic messageText = XVar.Clone(_param_messageText);
			#endregion

			this.message = XVar.Clone(messageText);

			return null;
		}
		protected virtual XVar remeberMachineChecksum()
		{
			dynamic data = XVar.Array(), elements = XVar.Array(), twofSettings = XVar.Array();
			twofSettings = Security.twoFactorSettings();
			elements = XVar.Clone(XVar.Array());
			data = Security.currentUserData();
			if(XVar.Pack(!(XVar)(data)))
			{
				return false;
			}
			elements.InitAndSetArrayItem(data[twofSettings["twoFactorField"]], null);
			elements.InitAndSetArrayItem(data[twofSettings["phoneField"]], null);
			elements.InitAndSetArrayItem(data[twofSettings["emailField"]], null);
			elements.InitAndSetArrayItem(data[twofSettings["codeField"]], null);
			return MVCFunctions.implode(new XVar(""), (XVar)(elements));
		}
		public virtual XVar setRememberMachineCookie(dynamic _param_success = null)
		{
			#region default values
			if(_param_success as Object == null) _param_success = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic success = XVar.Clone(_param_success);
			#endregion

			dynamic twofSettings = XVar.Array();
			twofSettings = Security.twoFactorSettings();
			if((XVar)((XVar)(success)  && (XVar)(this.rememberMachine))  && (XVar)(twofSettings["remember"]))
			{
				dynamic secondsIn30 = null;
				secondsIn30 = XVar.Clone((30 * 1440) * 60);
				CommonFunctions.setProjectCookie(new XVar("2ftoken"), (XVar)(CommonFunctions.jwt_encode((XVar)(new XVar("2fusername", Security.getUserName(), "host", CommonFunctions.projectHost(), "checksum", MVCFunctions.getPasswordHash((XVar)(this.remeberMachineChecksum())))), (XVar)(secondsIn30))), (XVar)(MVCFunctions.time() + secondsIn30), new XVar(true));
			}
			else
			{
				CommonFunctions.setProjectCookie(new XVar("2ftoken"), new XVar(""), (XVar)(MVCFunctions.time() - 1), new XVar(true));
			}

			return null;
		}
		public virtual XVar verifyRememberMachineCookie(dynamic _param_jwt)
		{
			#region pass-by-value parameters
			dynamic jwt = XVar.Clone(_param_jwt);
			#endregion

			dynamic payload = XVar.Array(), twofSettings = XVar.Array();
			twofSettings = Security.twoFactorSettings();
			if(XVar.Pack(!(XVar)(twofSettings["remember"])))
			{
				return false;
			}
			payload = XVar.Clone(CommonFunctions.jwt_verify_decode((XVar)(jwt)));
			if(XVar.Pack(!(XVar)(payload)))
			{
				return false;
			}
			return (XVar)(XVar.Equals(XVar.Pack(payload["2fusername"]), XVar.Pack(Security.provisionalUsername())))  && (XVar)(MVCFunctions.passwordVerify((XVar)(this.remeberMachineChecksum()), (XVar)(payload["checksum"])));
		}
		public override XVar setLangParams()
		{
			if(1 < MVCFunctions.count(ProjectSettings.getProjectValue(new XVar("languages"))))
			{
				if(this.mode == Constants.LOGIN_SIMPLE)
				{
					CommonFunctions.SetLangVars((XVar)(this.xt), new XVar("login"));
				}
			}

			return null;
		}
		protected override XVar assignBody()
		{
			dynamic projectBuildKey = null, wizardBuildKey = null;
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage(new XVar(false)));
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<form method=\"post\" action='", MVCFunctions.GetTableLink(new XVar("login"), new XVar(""), (XVar)(MVCFunctions.Concat("page=", MVCFunctions.RawUrlEncode((XVar)(this.pageName))))), "' ", "id=\"form", this.id, "\" name=\"form", this.id, "\">", "<input type=\"hidden\" name=\"btnSubmit\" value=\"Login\">");
			this.body["end"] = MVCFunctions.Concat(this.body["end"], "</form>");
			this.body["end"] = MVCFunctions.Concat(this.body["end"], "<script>");
			this.body["end"] = MVCFunctions.Concat(this.body["end"], "window.controlsMap = ", MVCFunctions.runner_json_encode((XVar)(this.controlsHTMLMap)), ";");
			this.body["end"] = MVCFunctions.Concat(this.body["end"], "window.viewControlsMap = ", MVCFunctions.runner_json_encode((XVar)(this.viewControlsHTMLMap)), ";");
			this.body["end"] = MVCFunctions.Concat(this.body["end"], "Runner.applyPagesData( ", MVCFunctions.runner_json_encode((XVar)(GlobalVars.pagesData)), " );");
			this.body["end"] = MVCFunctions.Concat(this.body["end"], "window.settings = ", MVCFunctions.runner_json_encode((XVar)(this.jsSettings)), ";</script>");
			projectBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("projectBuild")));
			wizardBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("wizardBuild")));
			this.body["end"] = MVCFunctions.Concat(this.body["end"], "<script language=\"JavaScript\" src=\"settings/project.js?", projectBuildKey, "\"></script>", "\r\n");
			this.body["end"] = MVCFunctions.Concat(this.body["end"], "<script language=\"JavaScript\" src=\"include/runnerJS/RunnerAll.js?", wizardBuildKey, "\"></script>", "\r\n");
			this.body["end"] = MVCFunctions.Concat(this.body["end"], "<script>", this.PrepareJS(), "</script>");
			this.xt.assignbyref(new XVar("body"), (XVar)(this.body));

			return null;
		}
		protected virtual XVar assignSPButtons()
		{
			this.xt.assign(new XVar("facebookbutton"), new XVar(true));
			this.xt.assign(new XVar("google_signin"), new XVar(true));
			if(XVar.Pack(CommonFunctions.GetEmailField()))
			{
				this.xt.assign(new XVar("fb_loginbutton_params"), new XVar(" data-scope=\"email\" "));
			}

			return null;
		}
		public virtual XVar doCommonAssignments()
		{
			dynamic guestUrl = null, rememberbox_checked = null, twofSettings = XVar.Array(), usernameValue = null;
			this.xt.assign(new XVar("loginlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"submitLogin", this.id, "\"")));
			this.assignSPButtons();
			this.setLangParams();
			rememberbox_checked = new XVar("");
			if(XVar.Pack(this.rememberPassword))
			{
				this.xt.assign(new XVar("rememberbox_attrs"), new XVar("checked"));
			}
			this.xt.assign(new XVar("guestlink_block"), (XVar)((XVar)(this.mode == Constants.LOGIN_SIMPLE)  && (XVar)(Security.guestLoginAvailable())));
			this.xt.assign(new XVar("username_label"), new XVar(true));
			this.xt.assign(new XVar("password_label"), new XVar(true));
			usernameValue = new XVar("");
			if(XVar.Pack(MVCFunctions.strlen((XVar)(this.var_pUsername))))
			{
				usernameValue = XVar.Clone(MVCFunctions.Concat("value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(this.var_pUsername)), "\""));
			}
			this.xt.assign(new XVar("username_attrs"), (XVar)(MVCFunctions.Concat("id=\"username\" ", usernameValue)));
			this.xt.assign(new XVar("password_attrs"), new XVar("id=\"password\" "));
			guestUrl = XVar.Clone((XVar.Pack((XVar)(this.myurl)  && (XVar)(XSession.Session["MyUrlAccess"])) ? XVar.Pack(this.myurl) : XVar.Pack(MVCFunctions.GetTableLink(new XVar("menu")))));
			this.xt.assign(new XVar("guestlink_attrs"), (XVar)(MVCFunctions.Concat("href=\"", MVCFunctions.runner_htmlspecialchars((XVar)(guestUrl)), "\"")));
			this.xt.assign(new XVar("main_loginfields"), new XVar(true));
			this.xt.assign(new XVar("signin_button"), new XVar(true));
			this.xt.assign(new XVar("login_logo"), new XVar(true));
			if(this.mode == Constants.LOGIN_POPUP)
			{
				if((XVar)((XVar)(this.restore)  && (XVar)(XVar.Equals(XVar.Pack(Security.userSessionLevel()), XVar.Pack(Constants.LOGGED_FULL))))  && (XVar)(!(XVar)(Security.isGuest())))
				{
					dynamic continuebutton_attrs = null;
					continuebutton_attrs = new XVar("href=\"#\" id=\"continueButton\"");
					this.xt.assign(new XVar("continuebutton_attrs"), (XVar)(continuebutton_attrs));
					this.xt.assign(new XVar("continue_button"), new XVar(true));
					this.hideMainLoginItems();
					this.hideTwoFactorItems();
					this.message = XVar.Clone(CommonFunctions.mlang_message(new XVar("SUCCES_LOGGED_IN")));
					this.messageType = new XVar(Constants.MESSAGE_INFO);
				}
				this.xt.assign(new XVar("footer"), new XVar(false));
				this.xt.assign(new XVar("header"), new XVar(false));
				this.xt.assign(new XVar("body"), (XVar)(this.body));
				this.xt.assign(new XVar("registerlink_attrs"), (XVar)(MVCFunctions.Concat("name=\"RegisterPage\" data-table=\"", MVCFunctions.runner_htmlspecialchars((XVar)(Security.loginTable())), "\"")));
				this.xt.assign(new XVar("forgotpasswordlink_attrs"), new XVar("name=\"ForgotPasswordPage\""));
				this.xt.assign(new XVar("login_logo"), new XVar(false));
			}
			if((XVar)((XVar)(this.message)  || (XVar)(this.mode == Constants.LOGIN_POPUP))  || (XVar)(this.securityPlugins))
			{
				this.xt.assign(new XVar("message_block"), new XVar(true));
				this.xt.assign(new XVar("message"), (XVar)(this.message));
				this.xt.assign(new XVar("message_class"), (XVar)((XVar.Pack(this.messageType == Constants.MESSAGE_INFO) ? XVar.Pack("alert-success") : XVar.Pack("alert-danger"))));
				if(XVar.Pack(!(XVar)(this.message)))
				{
					this.hideElement(new XVar("message"));
				}
			}
			twofSettings = Security.twoFactorSettings();
			if(XVar.Pack(!(XVar)(twofSettings["remember"])))
			{
				this.hideItemType(new XVar("remember_machine"));
			}

			return null;
		}
		public virtual XVar showPage()
		{
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeShowLogin"))))
			{
				GlobalVars.globalEvents.BeforeShowLogin((XVar)(this.xt), ref this.templatefile, this);
			}
			if(this.mode == Constants.LOGIN_POPUP)
			{
				this.displayAJAX((XVar)(this.templatefile), (XVar)(this.id + 1));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			if(this.mode == Constants.LOGIN_SIMPLE)
			{
				this.assignBody();
			}
			this.display((XVar)(this.templatefile));

			return null;
		}
		public static XVar readLoginModeFromRequest()
		{
			dynamic pageMode = null;
			pageMode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(pageMode == "popup")
			{
				return Constants.LOGIN_POPUP;
			}
			return Constants.LOGIN_SIMPLE;
		}
		public static XVar readActionFromRequest()
		{
			dynamic action = null;
			action = XVar.Clone(MVCFunctions.postvalue(new XVar("a")));
			if(XVar.Pack(action))
			{
				return action;
			}
			return MVCFunctions.postvalue("btnSubmit");
		}
		public static XVar readMethodFromRequest()
		{
			dynamic method = null;
			method = XVar.Clone(MVCFunctions.postvalue(new XVar("method")));
			if(method == XVar.Pack(""))
			{
				return -1;
			}
			return method;
		}
		public override XVar element2Item(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(name == "message")
			{
				return new XVar(0, "login_message");
			}
			else
			{
				if(name == "bsloginregister")
				{
					return new XVar(0, "login_remind", 1, "loginform_register_link");
				}
			}
			return base.element2Item((XVar)(name));
		}
		public override XVar createProjectSettings()
		{
			dynamic loginTable = null, table = null;
			loginTable = XVar.Clone(Security.loginTable());
			table = XVar.Clone(this.tName);
			if(XVar.Pack(CommonFunctions.GetTableURL((XVar)(loginTable))))
			{
				table = XVar.Clone(loginTable);
			}
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table), (XVar)(this.pageType), (XVar)(this.pageName), (XVar)(this.pageTable)));
			if(!XVar.Equals(XVar.Pack(this.pSet.getPageType()), XVar.Pack(this.pageType)))
			{
				this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table), (XVar)(this.pageType), new XVar(null), (XVar)(this.pageTable)));
			}

			return null;
		}
		protected virtual XVar prepareEditControls()
		{
			dynamic controlFields = XVar.Array(), defvalues = XVar.Array();
			if(XVar.Equals(XVar.Pack(Security.userSessionLevel()), XVar.Pack(Constants.LOGGED_2F_PENDING)))
			{
				return null;
			}
			defvalues = XVar.Clone(this.controlsData);
			controlFields = XVar.Clone(this.pSet.getPageFields());
			foreach (KeyValuePair<XVar, dynamic> fName in controlFields.GetEnumerator())
			{
				dynamic controls = XVar.Array(), firstElementId = null, gf = null, parameters = XVar.Array(), preload = XVar.Array();
				parameters = XVar.Clone(XVar.Array());
				parameters.InitAndSetArrayItem(this.id, "id");
				parameters.InitAndSetArrayItem("add", "mode");
				parameters.InitAndSetArrayItem(fName.Value, "field");
				parameters.InitAndSetArrayItem(defvalues[fName.Value], "value");
				parameters.InitAndSetArrayItem(this, "pageObj");
				gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(fName.Value)));
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
				if(XVar.Pack(this.pSet.isUseRTE((XVar)(fName.Value))))
				{
					XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_", fName.Value, "_rte")] = defvalues[fName.Value];
				}
				firstElementId = XVar.Clone(this.getControl((XVar)(fName.Value), (XVar)(this.id)).getFirstElementId());
				if(XVar.Pack(firstElementId))
				{
					this.xt.assign((XVar)(MVCFunctions.Concat("labelfor_", MVCFunctions.GoodFieldName((XVar)(fName.Value)))), (XVar)(firstElementId));
				}
				controls = XVar.Clone(XVar.Array());
				controls.InitAndSetArrayItem(XVar.Array(), "controls");
				controls.InitAndSetArrayItem(this.id, "controls", "id");
				controls.InitAndSetArrayItem("add", "controls", "mode");
				controls.InitAndSetArrayItem(0, "controls", "ctrlInd");
				controls.InitAndSetArrayItem(fName.Value, "controls", "fieldName");
				preload = XVar.Clone(this.fillPreload((XVar)(fName.Value), (XVar)(controlFields), (XVar)(defvalues)));
				if(!XVar.Equals(XVar.Pack(preload), XVar.Pack(false)))
				{
					controls.InitAndSetArrayItem(preload, "controls", "preloadData");
					if((XVar)(!(XVar)(defvalues[fName.Value]))  && (XVar)(0 < MVCFunctions.count(preload["vals"])))
					{
						defvalues.InitAndSetArrayItem(preload["vals"][0], fName.Value);
					}
				}
				this.fillControlsMap((XVar)(controls));
				this.fillControlFlags((XVar)(fName.Value));
			}

			return null;
		}
		protected virtual XVar readControls()
		{
			dynamic avalues = XVar.Array(), blobfields = null, filename_values = null;
			avalues = XVar.Clone(XVar.Array());
			blobfields = XVar.Clone(XVar.Array());
			filename_values = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getPageFields().GetEnumerator())
			{
				dynamic control = null;
				control = XVar.Clone(this.getControl((XVar)(f.Value), (XVar)(this.id)));
				control.readWebValue((XVar)(avalues), (XVar)(blobfields), new XVar(null), new XVar(null), (XVar)(filename_values));
			}
			foreach (KeyValuePair<XVar, dynamic> value in avalues.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(value.Key), (XVar)(blobfields)))))
				{
					this.controlsData.InitAndSetArrayItem(value.Value, value.Key);
				}
			}

			return null;
		}
		protected virtual XVar getControlsWhere(dynamic _param_cipherer)
		{
			#region pass-by-value parameters
			dynamic cipherer = XVar.Clone(_param_cipherer);
			#endregion

			dynamic controlsWhereParts = XVar.Array();
			if(XVar.Pack(!(XVar)(this.controlsData)))
			{
				return "";
			}
			controlsWhereParts = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in this.controlsData.GetEnumerator())
			{
				dynamic _value = null;
				_value = XVar.Clone(cipherer.MakeDBValue((XVar)(value.Key), (XVar)(value.Value), new XVar(""), new XVar(true)));
				controlsWhereParts.InitAndSetArrayItem(MVCFunctions.Concat(this.getFieldSQLDecrypt((XVar)(value.Key)), " = ", _value), null);
			}
			return MVCFunctions.Concat(" and ", MVCFunctions.implode(new XVar(" and "), (XVar)(controlsWhereParts)));
		}
		protected override XVar buildJsValidationData(dynamic _param_pSet_packed, dynamic _param_field)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic denyPos = null, settings = XVar.Array();
			settings = XVar.Clone(base.buildJsValidationData((XVar)(pSet), (XVar)(field)));
			denyPos = XVar.Clone(MVCFunctions.array_search(new XVar("DenyDuplicated"), (XVar)(settings["validationArr"])));
			if(!XVar.Equals(XVar.Pack(denyPos), XVar.Pack(false)))
			{
				MVCFunctions.array_splice((XVar)(settings["validationArr"]), (XVar)(denyPos), new XVar(1));
			}
			settings["customMessages"].Remove("DenyDuplicated");
			return settings;
		}
		protected virtual XVar hasProvisionalSession()
		{
			dynamic sessionLevel = null;
			sessionLevel = XVar.Clone(Security.userSessionLevel());
			return (XVar)(sessionLevel == Constants.LOGGED_2F_PENDING)  || (XVar)(sessionLevel == Constants.LOGGED_ACTIVATION_PENDING);
		}
		protected virtual XVar resendActivation()
		{
			dynamic activationCode = null, data = XVar.Array(), email = null, html = null, sent = XVar.Array(), uData = XVar.Array(), username = null;
			uData = XVar.Clone(Security.currentUserData());
			email = XVar.Clone(uData[Security.emailField()]);
			username = XVar.Clone(uData[Security.usernameField()]);
			activationCode = XVar.Clone(Security.getActivationCode((XVar)(username), (XVar)(uData[Security.passwordField()])));
			data = XVar.Clone(XVar.Array());
			data.InitAndSetArrayItem(this.getUserActivationUrl((XVar)(username), (XVar)(activationCode)), "activateurl");
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getPageFields().GetEnumerator())
			{
				data.InitAndSetArrayItem(uData[f.Value], MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(f.Value, "_value"))));
			}
			data.InitAndSetArrayItem(email, "email_value");
			data.InitAndSetArrayItem(username, MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(Security.usernameField(), "_value"))));
			data.InitAndSetArrayItem(email, MVCFunctions.GoodFieldName((XVar)(MVCFunctions.Concat(Security.emailField(), "_value"))));
			html = XVar.Clone(CommonFunctions.isEmailTemplateUseHTML(new XVar("userregister")));
			sent = XVar.Clone(RunnerPage.sendEmailByTemplate((XVar)(email), new XVar("userregister"), (XVar)(data), (XVar)(html)));
			if(XVar.Pack(!(XVar)(sent["mailed"])))
			{
				this.message = XVar.Clone(sent["message"]);
				this.messageType = new XVar(Constants.MESSAGE_ERROR);
			}
			else
			{
				this.message = XVar.Clone(MVCFunctions.str_replace(new XVar("%email%"), (XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(email))), (XVar)(CommonFunctions.mlang_message(new XVar("REGISTER_ACTIVATE")))));
				this.messageType = new XVar(Constants.MESSAGE_INFO);
			}

			return null;
		}
		protected virtual XVar currentProvider()
		{
			if(XVar.Pack(Security.hardcodedLogin()))
			{
				dynamic providers = XVar.Array();
				providers = Security.providersByType(new XVar(Constants.stHARDCODED));
				return providers[0];
			}
			if(XVar.Pack(this.providerCode))
			{
				dynamic provider = null;
				provider = XVar.Clone(Security.findProvider((XVar)(this.providerCode)));
				if(XVar.Pack(provider))
				{
					return provider;
				}
			}
			return Security.defaultProvider();
		}
		protected virtual XVar saveTwoFactorValue()
		{
			dynamic dataSource = null, dc = null, twoFactorSettings = XVar.Array(), userdata = XVar.Array();
			userdata = XVar.Clone(Security.currentUserData());
			twoFactorSettings = Security.twoFactorSettings();
			dc = XVar.Clone(new DsCommand());
			dc.values.InitAndSetArrayItem(userdata[twoFactorSettings["twoFactorField"]], twoFactorSettings["twoFactorField"]);
			dc.filter = XVar.Clone(Security.currentUserCondition());
			dataSource = XVar.Clone(CommonFunctions.getLoginDataSource());
			dataSource.updateSingle((XVar)(dc), new XVar(false));

			return null;
		}
	}
}
