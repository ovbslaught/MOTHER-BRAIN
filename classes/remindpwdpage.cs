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
	public partial class RemindPasswordPage : RunnerPage
	{
		public dynamic action = XVar.Pack("");
		public dynamic userEmail = XVar.Pack("");
		protected dynamic reminded = XVar.Pack(false);
		protected static bool skipRemindPasswordPageCtor = false;
		public RemindPasswordPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipRemindPasswordPageCtor)
			{
				skipRemindPasswordPageCtor = false;
				return;
			}
			this.headerForms = XVar.Clone(new XVar(0, "top"));
			this.footerForms = XVar.Clone(new XVar(0, "footer"));
			this.bodyForms = XVar.Clone(new XVar(0, "above-grid", 1, "grid", 2, "superbottom"));
		}
		protected override XVar setDataSource()
		{
			this.dataSource = XVar.Clone(CommonFunctions.getLoginDataSource());

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
		public override XVar getCaptchaId()
		{
			return "remind";
		}
		public virtual XVar process()
		{
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeProcessRemind"))))
			{
				GlobalVars.globalEvents.BeforeProcessRemind(this);
			}
			if(this.action == "Remind")
			{
				this.reminded = XVar.Clone(this.remindPassword());
				if((XVar)(!(XVar)(this.reminded))  && (XVar)(this.mode == Constants.REMIND_POPUP))
				{
					dynamic returnJSON = XVar.Array();
					returnJSON = XVar.Clone(XVar.Array());
					returnJSON.InitAndSetArrayItem(this.message, "message");
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
					MVCFunctions.ob_flush();
					HttpContext.Current.Response.End();
					throw new RunnerInlineOutputException();
				}
			}
			if(XVar.Pack(this.reminded))
			{
				this.pageName = XVar.Clone(this.pSet.getDefaultPage((XVar)(this.successPageType())));
				this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), (XVar)(this.successPageType()), (XVar)(this.pageName), (XVar)(this.pageTable)));
				this.pageData.InitAndSetArrayItem(MVCFunctions.array_merge((XVar)(this.pageData["buttons"]), (XVar)(this.pSet.buttons())), "buttons");
				foreach (KeyValuePair<XVar, dynamic> b in this.pSet.buttons().GetEnumerator())
				{
					this.AddJSFile((XVar)(MVCFunctions.Concat("include/button_", b.Value, ".js")));
				}
			}
			if(XVar.Pack(this.captchaExists()))
			{
				this.displayCaptcha();
			}
			this.addCommonJs();
			this.fillSetCntrlMaps();
			this.addButtonHandlers();
			this.doCommonAssignments();
			this.showPage();

			return null;
		}
		protected virtual XVar getNotRegisteredMessage()
		{
			return MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("USER_NOREG1")), " ", MVCFunctions.runner_htmlspecialchars((XVar)(this.userEmail)), " ", CommonFunctions.mlang_message(new XVar("USER_NOREG2")));
		}
		protected virtual XVar remindPassword()
		{
			dynamic data = XVar.Array(), dbPassword = null, dc = null, email = null, fetchedArray = null, password = null, reminded = null, token = null, username = null;
			if(XVar.Pack(!(XVar)(this.checkCaptcha())))
			{
				this.message = XVar.Clone(CommonFunctions.mlang_message(new XVar("SEC_INVALID_CAPTCHA_CODE")));
				return false;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(this.userEmail)))))
			{
				this.message = XVar.Clone(this.getNotRegisteredMessage());
				return false;
			}
			dc = XVar.Clone(this.getSubsetDataCommand());
			fetchedArray = XVar.Clone(this.dataSource.getSingle((XVar)(dc)).fetchAssoc());
			data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
			if(XVar.Pack(data))
			{
				username = XVar.Clone(data[Security.usernameField()]);
				email = XVar.Clone(data[Security.emailField()]);
				password = XVar.Clone(data[Security.passwordField()]);
			}
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeRemindPassword"))))
			{
				GlobalVars.globalEvents.BeforeRemindPassword((XVar)(username), (XVar)(email), this);
			}
			if(XVar.Pack(!(XVar)(data)))
			{
				this.message = XVar.Clone(this.getNotRegisteredMessage());
				return false;
			}
			token = new XVar("");
			dbPassword = XVar.Clone(password);
			if(this.remindMethod() == Constants.RPM_RESET)
			{
				token = XVar.Clone(CommonFunctions.generatePassword(new XVar(20)));
				this.dataSource.updateSingle((XVar)(this.getUpdateTokenCommand((XVar)(token))), new XVar(false));
			}
			else
			{
				if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("hashPassword"))))
				{
					if(XVar.Pack(!(XVar)(this.cipherer.isFieldEncrypted((XVar)(Security.passwordField())))))
					{
						password = XVar.Clone(CommonFunctions.generatePassword(new XVar(10)));
						dbPassword = XVar.Clone(Security.hashPassword((XVar)(password)));
						this.dataSource.updateSingle((XVar)(this.getUpdatePasswordCommand((XVar)(dbPassword))), new XVar(false));
					}
				}
			}
			reminded = XVar.Clone(this.sendRemindEmail((XVar)(username), (XVar)(password), (XVar)(email), (XVar)(token), (XVar)(dbPassword)));
			if(XVar.Pack(reminded))
			{
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterRemindPassword"))))
				{
					GlobalVars.globalEvents.AfterRemindPassword((XVar)(username), (XVar)(email), this);
				}
			}
			return reminded;
		}
		protected virtual XVar getUpdateTokenCommand(dynamic _param_token)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			#endregion

			dynamic dc = null, newData = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getUserCondition());
			newData = XVar.Clone(XVar.Array());
			newData.InitAndSetArrayItem(token, Security.resetTokenField());
			newData.InitAndSetArrayItem(MVCFunctions.now(), Security.resetDateField());
			dc.values = XVar.Clone(newData);
			return dc;
		}
		protected virtual XVar getUpdatePasswordCommand(dynamic _param_dbPassword)
		{
			#region pass-by-value parameters
			dynamic dbPassword = XVar.Clone(_param_dbPassword);
			#endregion

			dynamic dc = null, newData = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getUserCondition());
			newData = XVar.Clone(XVar.Array());
			newData.InitAndSetArrayItem(dbPassword, Security.passwordField());
			dc.values = XVar.Clone(newData);
			return dc;
		}
		protected virtual XVar remindMethod()
		{
			dynamic var_option = XVar.Array();
			var_option = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("registration")));
			return var_option["remindMethod"];
		}
		protected virtual XVar sendRemindEmail(dynamic _param_username, dynamic _param_password, dynamic _param_email, dynamic _param_token, dynamic _param_dbPassword)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			dynamic email = XVar.Clone(_param_email);
			dynamic token = XVar.Clone(_param_token);
			dynamic dbPassword = XVar.Clone(_param_dbPassword);
			#endregion

			dynamic data = XVar.Array(), html = null, sentMailResults = XVar.Array(), template = null, url = null, userData = XVar.Array();
			data = XVar.Clone(XVar.Array());
			url = XVar.Clone(MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("remind"))));
			userData = XVar.Clone(Security.getUserData((XVar)(username)));
			if(XVar.Pack(userData))
			{
				foreach (KeyValuePair<XVar, dynamic> value in userData.GetEnumerator())
				{
					data.InitAndSetArrayItem(value.Value, MVCFunctions.GoodFieldName((XVar)(value.Key)));
				}
			}
			if(this.remindMethod() == Constants.RPM_RESET)
			{
				template = new XVar("resetpassword");
				data.InitAndSetArrayItem(MVCFunctions.Concat(MVCFunctions.str_replace(new XVar("/remind"), new XVar("/changepwd"), (XVar)(url)), "?token=", token), "reseturl");
			}
			else
			{
				template = new XVar("remindpassword");
				if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
				{
					if(XVar.Pack(!(XVar)(userData[ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("activationField"))])))
					{
						if(XVar.Pack(!(XVar)(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("hashPassword")))))
						{
							dbPassword = XVar.Clone(MVCFunctions.md5((XVar)(dbPassword)));
						}
						data.InitAndSetArrayItem(MVCFunctions.str_replace(new XVar("/remind"), new XVar("/register"), (XVar)(url)), "activateurl");
						data["activateurl"] = MVCFunctions.Concat(data["activateurl"], "?a=activate&u=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.base64_encode((XVar)(username)))), "&code=");
						data["activateurl"] = MVCFunctions.Concat(data["activateurl"], MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.md5((XVar)(MVCFunctions.Concat(username, dbPassword))))));
					}
				}
				data.InitAndSetArrayItem(MVCFunctions.Concat(MVCFunctions.str_replace(new XVar("/remind"), new XVar("/login"), (XVar)(url)), "?username=", MVCFunctions.RawUrlEncode((XVar)(username))), "loginurl");
				data.InitAndSetArrayItem(username, "username");
				data.InitAndSetArrayItem(password, "password");
			}
			html = XVar.Clone(CommonFunctions.isEmailTemplateUseHTML((XVar)(template)));
			sentMailResults = XVar.Clone(RunnerPage.sendEmailByTemplate((XVar)(email), (XVar)(template), (XVar)(data), (XVar)(html)));
			if(XVar.Pack(!(XVar)(sentMailResults["mailed"])))
			{
				this.message = XVar.Clone(sentMailResults["message"]);
				if(XVar.Pack(!(XVar)(this.message)))
				{
					this.message = XVar.Clone(this.getNotRegisteredMessage());
				}
			}
			return sentMailResults["mailed"];
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
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.getUserCondition());
			return dc;
		}
		protected virtual XVar getUserCondition()
		{
			dynamic conditions = XVar.Array();
			conditions = XVar.Clone(XVar.Array());
			conditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(Security.usernameField()), (XVar)(this.userEmail), new XVar(0), (XVar)((XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_STRICT)))), null);
			conditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(Security.emailField()), (XVar)(this.userEmail), new XVar(0), new XVar(Constants.dsCASE_INSENSITIVE)), null);
			return DataCondition._Or((XVar)(conditions));
		}
		protected virtual XVar doCommonAssignments()
		{
			this.xt.assign(new XVar("username_email_attrs"), (XVar)(MVCFunctions.Concat((XVar.Pack(this.is508) ? XVar.Pack("id=\"username_email\" ") : XVar.Pack("")), "value=\"", MVCFunctions.runner_htmlspecialchars((XVar)(this.userEmail)), "\"")));
			this.xt.assign(new XVar("submit_attrs"), new XVar("onclick='document.forms.form1.submit();return false;'"));
			if((XVar)(this.mode == Constants.REMIND_POPUP)  && (XVar)(!(XVar)(this.reminded)))
			{
				this.xt.assign(new XVar("backlink_attrs"), new XVar("style=\"display:none\""));
				this.xt.assign(new XVar("submit_attrs"), new XVar("id='submitRemind'"));
			}
			this.xt.assign(new XVar("username_email_label"), new XVar(true));
			if(XVar.Pack(this.is508))
			{
				this.xt.assign_section(new XVar("username_email_label"), new XVar("<label for=\"username_email\">"), new XVar("</label>"));
			}
			if((XVar)(this.message)  || (XVar)((XVar)(this.mode == Constants.REMIND_POPUP)  && (XVar)(!(XVar)(this.reminded))))
			{
				this.xt.assign(new XVar("message_class"), new XVar("alert-danger"));
				this.xt.assign(new XVar("message"), (XVar)(this.message));
				this.xt.assign(new XVar("message_block"), new XVar(true));
				if((XVar)(this.mode == Constants.REMIND_POPUP)  && (XVar)(!(XVar)(this.reminded)))
				{
					this.hideElement(new XVar("message"));
				}
			}
			this.xt.assign(new XVar("closewindow_attrs"), new XVar("style=\"display:none\" id=\"closeWindowRemind\""));
			if(this.mode == Constants.REMIND_SIMPLE)
			{
				this.assignBody();
			}

			return null;
		}
		protected override XVar assignBody()
		{
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage(new XVar(false)));
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<form method=post action=\"", MVCFunctions.GetTableLink(new XVar("remind")), "\" id=form1 name=form1>\r\n\t\t<input type=hidden name=btnSubmit value=\"Remind\">");
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
			this.xt.assign(new XVar("body"), (XVar)(this.body));

			return null;
		}
		public override XVar assignBodyEnd(dynamic _param_params = null)
		{
			#region default values
			if(_param_params as Object == null) _param_params = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic projectBuildKey = null, wizardBuildKey = null;
			this.fillSetCntrlMaps();
			MVCFunctions.Echo("</form>");
			MVCFunctions.Echo(MVCFunctions.Concat("<script>\r\n\t\t\twindow.controlsMap = ", MVCFunctions.runner_json_encode((XVar)(this.controlsHTMLMap)), ";\r\n\t\t\twindow.viewControlsMap = ", MVCFunctions.runner_json_encode((XVar)(this.viewControlsHTMLMap)), ";\r\n\t\t\tRunner.applyPagesData( ", MVCFunctions.runner_json_encode((XVar)(GlobalVars.pagesData)), " );\r\n\t\t\twindow.settings = ", MVCFunctions.runner_json_encode((XVar)(this.jsSettings)), ";\r\n\t\t\t</script>"));
			projectBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("projectBuild")));
			wizardBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("wizardBuild")));
			MVCFunctions.Echo(MVCFunctions.Concat("<script language=\"JavaScript\" src=\"settings/project.js?", projectBuildKey, "\"></script>"));
			MVCFunctions.Echo(MVCFunctions.Concat("<script language=\"JavaScript\" src=\"include/runnerJS/RunnerAll.js?", wizardBuildKey, "\"></script>"));
			MVCFunctions.Echo(MVCFunctions.Concat("<script>", this.PrepareJS(), "</script>"));

			return null;
		}
		protected virtual XVar showPage()
		{
			if(XVar.Pack(this.reminded))
			{
				this.switchToSuccessPage();
			}
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeShowRemindPwd"))))
			{
				GlobalVars.globalEvents.BeforeShowRemindPwd((XVar)(this.xt), ref this.templatefile, this);
			}
			if(this.mode == Constants.REMIND_POPUP)
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
		public static XVar readActionFromRequest()
		{
			return MVCFunctions.postvalue(new XVar("btnSubmit"));
		}
		public static XVar readRemindModeFromRequest()
		{
			dynamic pageMode = null;
			pageMode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(pageMode == "popup")
			{
				return Constants.REMIND_POPUP;
			}
			return Constants.REMIND_SIMPLE;
		}
	}
}
