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
	public partial class Security : XClass
	{
		public static XVar processPageSecurity(dynamic _param_table, dynamic _param_permission, dynamic _param_ajaxMode = null, dynamic _param_message = null)
		{
			#region default values
			if(_param_ajaxMode as Object == null) _param_ajaxMode = new XVar(false);
			if(_param_message as Object == null) _param_message = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic permission = XVar.Clone(_param_permission);
			dynamic ajaxMode = XVar.Clone(_param_ajaxMode);
			dynamic message = XVar.Clone(_param_message);
			#endregion

			if(XVar.Pack(Security.checkPagePermissions((XVar)(table), (XVar)(permission))))
			{
				return true;
			}
			if(XVar.Pack(ajaxMode))
			{
				Security.sendPermissionError((XVar)(message));
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
		public static XVar processAdminPageSecurity(dynamic _param_ajaxMode = null)
		{
			#region default values
			if(_param_ajaxMode as Object == null) _param_ajaxMode = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic ajaxMode = XVar.Clone(_param_ajaxMode);
			#endregion

			Security.processLogoutRequest();
			if((XVar)(!(XVar)(CommonFunctions.isLogged()))  || (XVar)(Security.isGuest()))
			{
				Security.tryRelogin();
			}
			if(XVar.Pack(Security.isAdmin()))
			{
				return true;
			}
			if(XVar.Pack(ajaxMode))
			{
				Security.sendPermissionError();
				return false;
			}
			if((XVar)(CommonFunctions.isLogged())  && (XVar)(!(XVar)(Security.isGuest())))
			{
				MVCFunctions.HeaderRedirect(new XVar("menu"));
				return false;
			}
			Security.saveRedirectURL();
			CommonFunctions.redirectToLogin();
			return false;
		}
		public static XVar saveRedirectURL()
		{
			dynamic query = null, url = null;
			url = XVar.Clone(MVCFunctions.GetScriptName());
			query = new XVar("");
			if(XVar.Pack(MVCFunctions.postvalue(new XVar("dashelement"))))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> value in MVCFunctions.EnumerateGET())
			{
				if((XVar)(value.Key == "a")  && (XVar)(value.Value == "logout"))
				{
					continue;
				}
				if(query != XVar.Pack(""))
				{
					query = MVCFunctions.Concat(query, "&");
				}
				if(XVar.Pack(MVCFunctions.is_array((XVar)(value.Value))))
				{
					query = MVCFunctions.Concat(query, MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.Concat(value.Key, "[]"))), "=");
					query = MVCFunctions.Concat(query, MVCFunctions.implode((XVar)(MVCFunctions.Concat(MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.Concat(value.Key, "[]"))), "=")), (XVar)(value.Value)));
				}
				else
				{
					query = MVCFunctions.Concat(query, MVCFunctions.RawUrlEncode((XVar)(value.Key)));
					if(XVar.Pack(MVCFunctions.strlen((XVar)(value.Value))))
					{
						query = MVCFunctions.Concat(query, "=", MVCFunctions.RawUrlEncode((XVar)(value.Value)));
					}
				}
			}
			if(query != XVar.Pack(""))
			{
				url = MVCFunctions.Concat(url, "?", query);
			}
			XSession.Session["MyURL"] = url;

			return null;
		}
		public static XVar checkPagePermissions(dynamic _param_table, dynamic _param_permission)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic permission = XVar.Clone(_param_permission);
			#endregion

			dynamic ret = null;
			Security.processLogoutRequest();
			Security.saveRedirectURL();
			ret = XVar.Clone(Security.checkUserPermissions((XVar)(table), (XVar)(permission)));
			XSession.Session["MyUrlAccess"] = ret;
			return ret;
		}
		public static XVar createLoginPageObject()
		{
			dynamic loginPageObject = null, loginParams = XVar.Array(), loginXt = null;
			loginXt = XVar.Clone(new XTempl());
			loginParams = XVar.Clone(new XVar("pageType", Constants.PAGE_LOGIN));
			loginParams.InitAndSetArrayItem(-1, "id");
			loginParams.InitAndSetArrayItem(loginXt, "xt");
			loginParams.InitAndSetArrayItem(Constants.GLOBAL_PAGES, "tName");
			loginParams.InitAndSetArrayItem(false, "needSearchClauseObj");
			loginParams.InitAndSetArrayItem(Security.currentProviderCode(), "providerCode");
			loginPageObject = XVar.Clone(new LoginPage((XVar)(loginParams)));
			loginPageObject.init();
			return loginPageObject;
		}
		public static XVar tryRelogin()
		{
			dynamic loginPageObject = null, loginToken = null, securityPlugins = XVar.Array();
			if((XVar)(CommonFunctions.isLogged())  && (XVar)(!(XVar)(Security.isGuest())))
			{
				return null;
			}
			if(MVCFunctions.postvalue(new XVar("a")) == "logout")
			{
				return null;
			}
			if(XVar.Pack(CommonFunctions.isPostRequest()))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(Security.allowAutoLogin())))
			{
				return null;
			}
			loginPageObject = new XVar(null);
			if(XVar.Pack(Security.tryLoginAutoAd(new XVar(true))))
			{
				return true;
			}
			loginToken = XVar.Clone(MVCFunctions.postvalue(new XVar("token")));
			if(XVar.Pack(!(XVar)(loginToken)))
			{
				loginToken = XVar.Clone(MVCFunctions.GetCookie("token"));
			}
			if(XVar.Pack(loginToken))
			{
				dynamic tokenPayload = XVar.Array();
				tokenPayload = XVar.Clone(Security.verifyKeepLoggedToken((XVar)(loginToken)));
				if(XVar.Pack(tokenPayload))
				{
					Security.loginAs((XVar)(tokenPayload["username"]), new XVar(true));
					return true;
				}
				Security.setKeepLoggedCookie(new XVar(false));
			}
			securityPlugins = XVar.Clone(Security.GetPlugins());
			foreach (KeyValuePair<XVar, dynamic> sp in securityPlugins.GetEnumerator())
			{
				dynamic token = null;
				token = XVar.Clone(sp.Value.savedToken());
				if(XVar.Pack(token))
				{
					if(XVar.Pack(!(XVar)(loginPageObject)))
					{
						loginPageObject = XVar.Clone(Security.createLoginPageObject());
					}
					if(XVar.Pack(loginPageObject.LoginWithSP((XVar)(sp.Value), (XVar)(token), new XVar(false))))
					{
						return true;
					}
				}
			}
			return false;
		}
		public static XVar checkUserPermissions(dynamic _param_table, dynamic _param_permission)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic permission = XVar.Clone(_param_permission);
			#endregion

			if((XVar)(!(XVar)(CommonFunctions.isLogged()))  || (XVar)(Security.isGuest()))
			{
				Security.tryRelogin();
			}
			if(table == Constants.ADMIN_USERS)
			{
				return Security.isAdmin();
			}
			return CommonFunctions.CheckTablePermissions((XVar)(table), (XVar)(permission));
		}
		public static XVar processLogoutRequest()
		{
			dynamic loginPageObject = null;
			if(MVCFunctions.postvalue(new XVar("a")) != "logout")
			{
				return false;
			}
			if((XVar)(XVar.Equals(XVar.Pack(Security.userSessionLevel()), XVar.Pack(Constants.LOGGED_NONE)))  || (XVar)(Security.isGuest()))
			{
				return false;
			}
			loginPageObject = XVar.Clone(Security.createLoginPageObject());
			loginPageObject.Logout();
			Security.doGuestLogin();
			if(MVCFunctions.postvalue(new XVar("reason")) != "expired")
			{
				GlobalVars.logoutPerformed = new XVar(true);
			}
			return true;
		}
		public static XVar sendPermissionError(dynamic _param_message = null)
		{
			#region default values
			if(_param_message as Object == null) _param_message = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			#endregion

			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(new XVar("success", false, "message", MVCFunctions.Concat(CommonFunctions.mlang_message(new XVar("NO_PERMISSIONS")), " ", message)))));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public static XVar redirectToList(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic settings = null;
			settings = XVar.Clone(new ProjectSettings((XVar)(table)));
			if(XVar.Pack(settings.hasListPage()))
			{
				MVCFunctions.HeaderRedirect((XVar)(settings.getShortTableName()), new XVar("list"), new XVar("a=return"));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			MVCFunctions.HeaderRedirect(new XVar("menu"));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public static XVar clearSecuritySession()
		{
			dynamic toClear = XVar.Array();
			XSession.Session.Abandon();
			Security.setKeepLoggedCookie(new XVar(false));
			MVCFunctions.RemoveCookie("username");
			MVCFunctions.RemoveCookie("password");
			MVCFunctions.RemoveCookie("token");
			MVCFunctions.RemoveCookie("runnerSession");
			CommonFunctions.setProjectCookie(new XVar("runnerSession"), new XVar(""), (XVar)(MVCFunctions.time() - 1), new XVar(true));
			CommonFunctions.storageDelete(new XVar("UserID"));
			CommonFunctions.storageDelete(new XVar("UserName"));
			CommonFunctions.storageDelete(new XVar("AccessLevel"));
			CommonFunctions.storageDelete(new XVar("UserRights"));
			CommonFunctions.storageDelete(new XVar("LastReadRights"));
			CommonFunctions.storageDelete(new XVar("GroupID"));
			CommonFunctions.storageDelete(new XVar("OwnerID"));
			CommonFunctions.storageDelete(new XVar("securityOverrides"));
			CommonFunctions.storageDelete(new XVar("runnerSession"));
			CommonFunctions.storageDelete(new XVar("AutomaticLogin"));
			CommonFunctions.storageDelete(new XVar("logout_token_hint"));
			CommonFunctions.storageDelete(new XVar("rawUserData"));
			toClear = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in XSession.Session.GetEnumerator())
			{
				if(MVCFunctions.substr((XVar)(v.Key), new XVar(-8)) == "_OwnerID")
				{
					toClear.InitAndSetArrayItem(v.Key, null);
				}
				if(MVCFunctions.substr((XVar)(v.Key), new XVar(0), new XVar(11)) == "oauthToken_")
				{
					toClear.InitAndSetArrayItem(v.Key, null);
				}
			}
			foreach (KeyValuePair<XVar, dynamic> k in toClear.GetEnumerator())
			{
				CommonFunctions.storageDelete((XVar)(k.Value));
			}

			return null;
		}
		public static XVar doGuestLogin()
		{
			if(XVar.Pack(!(XVar)(Security.guestLoginAvailable())))
			{
				return null;
			}
			Security.createUserSession(new XVar(null), new XVar(""));

			return null;
		}
		public static XVar getUserGroup()
		{
			dynamic userGroups = XVar.Array();
			userGroups = XVar.Clone(Security.getUserGroups());
			foreach (KeyValuePair<XVar, dynamic> v in userGroups.GetEnumerator())
			{
				return v.Key;
			}
			return "";
		}
		public static XVar getUserGroupIds()
		{
			dynamic arrgroups = XVar.Array(), groups = XVar.Array(), userRights = XVar.Array();
			if(XVar.Pack(!(XVar)(Security.userGroupsAvailable())))
			{
				return XVar.Array();
			}
			if(XVar.Pack(!(XVar)(Security.dynamicPermissions())))
			{
				if(XVar.Pack(CommonFunctions.storageGet(new XVar("GroupID"))))
				{
					return new XVar(CommonFunctions.storageGet(new XVar("GroupID")), true);
				}
				return XVar.Array();
			}
			groups = XVar.Clone(XVar.Array());
			userRights = Security.dynamicUserRights();
			arrgroups = userRights[".Groups"];
			foreach (KeyValuePair<XVar, dynamic> g in arrgroups.GetEnumerator())
			{
				groups.InitAndSetArrayItem(true, g.Value);
			}
			return groups;
		}
		public static XVar getUserGroups()
		{
			dynamic data = XVar.Array(), dataSource = null, dc = null, grConnection = null, groupIds = XVar.Array(), groupNames = XVar.Array(), qResult = null;
			if(XVar.Pack(!(XVar)(Security.userGroupsAvailable())))
			{
				return XVar.Array();
			}
			if(XVar.Pack(!(XVar)(Security.dynamicPermissions())))
			{
				return Security.getUserGroupIds();
			}
			groupIds = XVar.Clone(Security.getUserGroupIds());
			if(XVar.Pack(!(XVar)(groupIds)))
			{
				return XVar.Array();
			}
			grConnection = XVar.Clone(GlobalVars.cman.getForUserGroups());
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition.FieldInList(new XVar("GroupID"), (XVar)(MVCFunctions.array_keys((XVar)(groupIds)))));
			dataSource = XVar.Clone(Security.getUgGroupsDatasource());
			qResult = XVar.Clone(dataSource.getList((XVar)(dc)));
			groupNames = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				groupNames.InitAndSetArrayItem(true, data["Label"]);
			}
			if(XVar.Pack(groupIds[-1]))
			{
				groupNames.InitAndSetArrayItem(true, "<Admin>");
			}
			return groupNames;
		}
		public static XVar getUserName()
		{
			dynamic ret = null;
			ret = XVar.Clone(CommonFunctions.storageGet(new XVar("UserID")));
			if(XVar.Pack(XVar.Equals(XVar.Pack(ret), XVar.Pack(null))))
			{
				return "";
			}
			return ret;
		}
		public static XVar getUserId()
		{
			dynamic provider = XVar.Array(), userId = null;
			userId = XVar.Clone((XVar.Pack(Security.userSessionLevel() == Constants.LOGGED_FULL) ? XVar.Pack(CommonFunctions.storageGet(new XVar("UserID"))) : XVar.Pack(Security.provisionalUsername())));
			provider = Security.currentProvider();
			if(provider["type"] != Constants.stAD)
			{
				return userId;
			}
			return MVCFunctions.Concat(provider["code"], userId);
		}
		public static XVar getDisplayName()
		{
			return CommonFunctions.storageGet(new XVar("UserName"));
		}
		public static XVar setDisplayName(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			CommonFunctions.storageSet(new XVar("UserName"), (XVar)(str));

			return null;
		}
		public static XVar refreshDisplayName()
		{
			dynamic fullName = null, fullnameField = null, userData = XVar.Array();
			userData = XVar.Clone(Security.getUserData((XVar)(Security.getUserName())));
			fullnameField = XVar.Clone(Security.fullnameField());
			if(XVar.Pack(!(XVar)(fullnameField)))
			{
				return null;
			}
			fullName = XVar.Clone(userData[fullnameField]);
			if(XVar.Pack(fullName))
			{
				Security.setDisplayName((XVar)(fullName));
			}

			return null;
		}
		public static XVar isGuest()
		{
			if((XVar)(Security.getUserName() == "Guest")  && (XVar)(CommonFunctions.storageGet(new XVar("AccessLevel")) == Constants.ACCESS_LEVEL_GUEST))
			{
				return true;
			}
			return false;
		}
		public static XVar isAdmin()
		{
			dynamic userRights = XVar.Array();
			if(XVar.Pack(!(XVar)(CommonFunctions.isLogged())))
			{
				return false;
			}
			if(XVar.Pack(!(XVar)(Security.dynamicPermissions())))
			{
				return false;
			}
			userRights = Security.dynamicUserRights();
			return userRights[".IsAdmin"];
		}
		public static XVar isLoggedIn()
		{
			return (XVar)(CommonFunctions.isLogged())  && (XVar)(!(XVar)(Security.isGuest()));
		}
		public static XVar loginAs(dynamic _param_username, dynamic _param_fireEvents = null, dynamic _param_displayName = null, dynamic _param_userData = null)
		{
			#region default values
			if(_param_fireEvents as Object == null) _param_fireEvents = new XVar(true);
			if(_param_displayName as Object == null) _param_displayName = new XVar("");
			if(_param_userData as Object == null) _param_userData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic fireEvents = XVar.Clone(_param_fireEvents);
			dynamic displayName = XVar.Clone(_param_displayName);
			dynamic userData = XVar.Clone(_param_userData);
			#endregion

			dynamic provider = XVar.Array();
			provider = XVar.Clone(Security.defaultProvider());
			if(XVar.Pack(!(XVar)(provider)))
			{
				return null;
			}
			if(XVar.Pack(Security.hardcodedLogin()))
			{
				Security.createUserSession((XVar)(provider), (XVar)(username), new XVar(""));
			}
			else
			{
				if(provider["type"] == Constants.stDB)
				{
					if(XVar.Pack(!(XVar)(userData)))
					{
						userData = XVar.Clone(Security.fetchUserData((XVar)(username), new XVar(""), new XVar(true)));
					}
					if(XVar.Pack(!(XVar)(userData)))
					{
						return false;
					}
					Security.createUserSession((XVar)(provider), (XVar)(userData[Security.usernameField()]), (XVar)(userData[Security.fullnameField()]), (XVar)(userData));
				}
				else
				{
					if(provider["type"] == Constants.stAD)
					{
						dynamic plugin = null;
						plugin = XVar.Clone(Security.getAuthPlugin((XVar)(provider["code"])));
						plugin.loginAsUser((XVar)(username));
					}
				}
			}
			if(XVar.Pack(fireEvents))
			{
				Security.auditLoginSuccess();
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterSuccessfulLogin"))))
				{
					GlobalVars.globalEvents.AfterSuccessfulLogin((XVar)(username), new XVar(""), (XVar)(userData), new XVar(null));
				}
			}
			return true;
		}
		public static XVar checkUsernamePassword(dynamic _param_username, dynamic _param_password, dynamic _param_fireEvents = null)
		{
			#region default values
			if(_param_fireEvents as Object == null) _param_fireEvents = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			dynamic fireEvents = XVar.Clone(_param_fireEvents);
			#endregion

			dynamic data = null;
			data = XVar.Clone(Security.fetchUserData((XVar)(username), (XVar)(password), new XVar(false)));
			if(XVar.Pack(data))
			{
				return true;
			}
			if(XVar.Pack(fireEvents))
			{
				dynamic message = null;
				message = new XVar("");
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterUnsuccessfulLogin"))))
				{
					GlobalVars.globalEvents.AfterUnsuccessfulLogin((XVar)(username), (XVar)(password), ref message, new XVar(null), new XVar(null));
				}
			}
			return false;
		}
		public static XVar login(dynamic _param_username, dynamic _param_password, dynamic _param_skipPasswordCheck = null, dynamic _param_fireEvents = null)
		{
			#region default values
			if(_param_skipPasswordCheck as Object == null) _param_skipPasswordCheck = new XVar(false);
			if(_param_fireEvents as Object == null) _param_fireEvents = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			dynamic skipPasswordCheck = XVar.Clone(_param_skipPasswordCheck);
			dynamic fireEvents = XVar.Clone(_param_fireEvents);
			#endregion

			if(XVar.Pack(Security.hardcodedLogin()))
			{
				if((XVar)(skipPasswordCheck)  || (XVar)(Security.verifyHardcodedLogin((XVar)(username), (XVar)(password))))
				{
					return Security.loginAs((XVar)(Security.hardcodedUsername()), (XVar)(fireEvents));
				}
			}
			else
			{
				dynamic userData = null;
				userData = XVar.Clone(Security.fetchUserData((XVar)(username), (XVar)(password), (XVar)(skipPasswordCheck)));
				if(XVar.Pack(userData))
				{
					return Security.loginAs((XVar)(username), (XVar)(fireEvents), new XVar(""), (XVar)(userData));
				}
			}
			if(XVar.Pack(fireEvents))
			{
				Security.auditLoginFail((XVar)(username));
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterUnsuccessfulLogin"))))
				{
					dynamic message = null;
					message = new XVar("");
					GlobalVars.globalEvents.AfterUnsuccessfulLogin((XVar)(username), (XVar)(password), ref message, new XVar(null), (XVar)(XVar.Array()));
				}
			}
			return false;
		}
		public static XVar getUserData(dynamic _param_username, dynamic _param_password = null)
		{
			#region default values
			if(_param_password as Object == null) _param_password = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			#endregion

			return Security.fetchUserData((XVar)(username), (XVar)(password), (XVar)(XVar.Equals(XVar.Pack(password), XVar.Pack(""))));
		}
		public static XVar currentUserData()
		{
			return CommonFunctions.storageGet(new XVar("UserData"));
		}
		public static XVar logout()
		{
			dynamic loginPageObject = null;
			loginPageObject = XVar.Clone(Security.createLoginPageObject());
			loginPageObject.Logout();

			return null;
		}
		public static XVar getPermissions(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			table = XVar.Clone(CommonFunctions.findTable((XVar)(table)));
			if(table == XVar.Pack(""))
			{
				return XVar.Array();
			}
			return Security.permMask2Array((XVar)(CommonFunctions.GetUserPermissions((XVar)(table))));
		}
		public static XVar setPermissions(dynamic _param_table, dynamic _param_rights)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic rights = XVar.Clone(_param_rights);
			#endregion

			dynamic oldPerm = XVar.Array(), overrides = XVar.Array();
			table = XVar.Clone(CommonFunctions.findTable((XVar)(table)));
			if(table == XVar.Pack(""))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(rights)))))
			{
				rights = XVar.Clone(Security.permMask2Array((XVar)(rights)));
			}
			oldPerm = XVar.Clone(Security.permMask2Array((XVar)(CommonFunctions.GetUserPermissions((XVar)(table)))));
			foreach (KeyValuePair<XVar, dynamic> v in rights.GetEnumerator())
			{
				if((XVar)(v.Value)  && (XVar)(!(XVar)(oldPerm[v.Key])))
				{
					Security.clearRestrictedPages((XVar)(table), (XVar)(v.Key));
				}
			}
			overrides = Security.createSecurityOverrides((XVar)(table));
			overrides.InitAndSetArrayItem(Security.permArray2Mask((XVar)(rights)), "mask");

			return null;
		}
		protected static XVar clearRestrictedPages(dynamic _param_table, dynamic _param_perm)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic perm = XVar.Clone(_param_perm);
			#endregion

			dynamic clearPageTypes = XVar.Array(), pages = XVar.Array();
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			pages = XVar.Clone(pSet.getOriginalPages());
			clearPageTypes = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> pageType in pages.GetEnumerator())
			{
				if(perm != Security.pageType2permission((XVar)(pageType.Value)))
				{
					continue;
				}
				clearPageTypes.InitAndSetArrayItem(true, pageType.Value);
			}
			foreach (KeyValuePair<XVar, dynamic> d in clearPageTypes.GetEnumerator())
			{
				Security._setRestrictedPages((XVar)(table), (XVar)(d.Key), (XVar)(XVar.Array()), (XVar)(pSet));
			}

			return null;
		}
		private static XVar _setRestrictedPages(dynamic _param_table, dynamic _param_pageType, dynamic _param_pages, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic pages = XVar.Clone(_param_pages);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic currentPages = XVar.Array(), newPages = XVar.Array(), overrides = XVar.Array();
			currentPages = XVar.Clone(Security.getRestrictedPages((XVar)(table), (XVar)(pSet)));
			newPages = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> p in currentPages.GetEnumerator())
			{
				if(!XVar.Equals(XVar.Pack(pSet.getOriginalPageType((XVar)(p.Value))), XVar.Pack(pageType)))
				{
					newPages.InitAndSetArrayItem(true, p.Value);
				}
			}
			if(XVar.Pack(pages))
			{
				foreach (KeyValuePair<XVar, dynamic> p in pages.GetEnumerator())
				{
					newPages.InitAndSetArrayItem(true, p.Value);
				}
			}
			overrides = Security.createSecurityOverrides((XVar)(table));
			overrides.InitAndSetArrayItem(newPages, "pages");
			pSet.resetPages();

			return null;
		}
		public static XVar setRestrictedPages(dynamic _param_table, dynamic _param_type, dynamic _param_pages)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic var_type = XVar.Clone(_param_type);
			dynamic pages = XVar.Clone(_param_pages);
			#endregion

			dynamic pageType = null;
			if(!XVar.Equals(XVar.Pack(table), XVar.Pack(Constants.GLOBAL_PAGES)))
			{
				table = XVar.Clone(CommonFunctions.findTable((XVar)(table)));
				if(table == XVar.Pack(""))
				{
					return null;
				}
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(pages)))))
			{
				pages = XVar.Clone(new XVar(0, pages));
			}
			Security._setRestrictedPages((XVar)(table), (XVar)(pageType), (XVar)(pages), (XVar)(new ProjectSettings((XVar)(table))));

			return null;
		}
		public static XVar setAllowedPages(dynamic _param_table, dynamic _param_type, dynamic _param_allowedPages)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic var_type = XVar.Clone(_param_type);
			dynamic allowedPages = XVar.Clone(_param_allowedPages);
			#endregion

			dynamic allPages = XVar.Array(), pageType = null, pages = XVar.Array();
			ProjectSettings pSet;
			if(!XVar.Equals(XVar.Pack(table), XVar.Pack(Constants.GLOBAL_PAGES)))
			{
				table = XVar.Clone(CommonFunctions.findTable((XVar)(table)));
				if(table == XVar.Pack(""))
				{
					return null;
				}
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(allowedPages)))))
			{
				allowedPages = XVar.Clone(new XVar(0, allowedPages));
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			allPages = XVar.Clone(pSet.getOriginalPagesByType((XVar)(var_type)));
			pages = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> p in allPages.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(p.Value), (XVar)(allowedPages)))))
				{
					pages.InitAndSetArrayItem(p.Value, null);
				}
			}
			Security._setRestrictedPages((XVar)(table), (XVar)(pageType), (XVar)(pages), (XVar)(pSet));

			return null;
		}
		private static XVar createSecurityOverrides(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			if(XVar.Pack(!(XVar)(XSession.Session.KeyExists("securityOverrides"))))
			{
				XSession.Session["securityOverrides"] = XVar.Array();
			}
			if(XVar.Pack(!(XVar)(XSession.Session["securityOverrides"].KeyExists(table))))
			{
				XSession.Session.InitAndSetArrayItem(XVar.Array(), "securityOverrides", table);
			}
			return XSession.Session["securityOverrides"][table];
		}
		private static XVar permMask2Array(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic c = null, i = null, ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.strlen((XVar)(str)); ++(i))
			{
				c = XVar.Clone(MVCFunctions.substr((XVar)(str), (XVar)(i), new XVar(1)));
				if((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(c == "A")  || (XVar)(c == "D"))  || (XVar)(c == "E"))  || (XVar)(c == "S"))  || (XVar)(c == "P"))  || (XVar)(c == "I"))  || (XVar)(c == "M"))
				{
					ret.InitAndSetArrayItem(true, c);
				}
			}
			return ret;
		}
		private static XVar permArray2Mask(dynamic _param_rights)
		{
			#region pass-by-value parameters
			dynamic rights = XVar.Clone(_param_rights);
			#endregion

			dynamic str = null;
			str = new XVar("");
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(rights)))))
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(rights))))
				{
					rights = XVar.Clone(Security.permMask2Array((XVar)(rights)));
				}
				else
				{
					return "";
				}
			}
			foreach (KeyValuePair<XVar, dynamic> v in rights.GetEnumerator())
			{
				if((XVar)(v.Value)  && (XVar)((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(v.Key == "A")  || (XVar)(v.Key == "D"))  || (XVar)(v.Key == "E"))  || (XVar)(v.Key == "S"))  || (XVar)(v.Key == "P"))  || (XVar)(v.Key == "I"))  || (XVar)(v.Key == "M")))
				{
					str = MVCFunctions.Concat(str, v.Key);
				}
			}
			return str;
		}
		public static XVar getOwnerId(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			table = XVar.Clone(CommonFunctions.findTable((XVar)(table)));
			if(table == XVar.Pack(""))
			{
				return null;
			}
			return CommonFunctions.storageGet((XVar)(MVCFunctions.Concat("_", table, "_OwnerID")));
		}
		public static XVar setOwnerId(dynamic _param_table, dynamic _param_ownerid)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic ownerid = XVar.Clone(_param_ownerid);
			#endregion

			table = XVar.Clone(CommonFunctions.findTable((XVar)(table)));
			if(table == XVar.Pack(""))
			{
				return null;
			}
			CommonFunctions.storageSet((XVar)(MVCFunctions.Concat("_", table, "_OwnerID")), (XVar)(ownerid));

			return null;
		}
		public static XVar hasLogin()
		{
			return ProjectSettings.getSecurityValue(new XVar("enabled"));
		}
		public static XVar loginMethod()
		{
			return Constants.SECURITY_TABLE;
		}
		public static XVar dynamicPermissions()
		{
			return ProjectSettings.getSecurityValue(new XVar("dynamicPermissions"));
		}
		public static XVar permissionsAvailable()
		{
			dynamic dbProv = XVar.Array();
			if(XVar.Pack(!(XVar)(Security.hasUsers())))
			{
				return false;
			}
			if(XVar.Pack(Security.dynamicPermissions()))
			{
				return true;
			}
			dbProv = XVar.Clone(Security.dbProvider());
			if((XVar)(dbProv)  && (XVar)(dbProv["active"]))
			{
				return 0 < MVCFunctions.count(ProjectSettings.staticPermissions());
			}
			return false;
		}
		public static XVar hasUsers()
		{
			return (XVar)(ProjectSettings.getSecurityValue(new XVar("enabled")))  && (XVar)(!(XVar)(ProjectSettings.getSecurityValue(new XVar("hardcodedLogin"))));
		}
		public static XVar userCan(dynamic _param_permission, dynamic _param_table, dynamic _param_checkOwnerId = null, dynamic _param_ownerId = null)
		{
			#region default values
			if(_param_checkOwnerId as Object == null) _param_checkOwnerId = new XVar(false);
			if(_param_ownerId as Object == null) _param_ownerId = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic permission = XVar.Clone(_param_permission);
			dynamic table = XVar.Clone(_param_table);
			dynamic checkOwnerId = XVar.Clone(_param_checkOwnerId);
			dynamic ownerId = XVar.Clone(_param_ownerId);
			#endregion

			dynamic advSecType = null, currentOwnerId = null, strPerm = null;
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(Security.hasLogin())))
			{
				return true;
			}
			strPerm = XVar.Clone(CommonFunctions.GetUserPermissions((XVar)(table)));
			if(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), (XVar)(permission))), XVar.Pack(false)))
			{
				return false;
			}
			if((XVar)(!(XVar)(checkOwnerId))  || (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("M"))), XVar.Pack(false))))
			{
				return true;
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			advSecType = XVar.Clone(pSet.getAdvancedSecurityType());
			if((XVar)(advSecType == Constants.ADVSECURITY_ALL)  || (XVar)(advSecType == Constants.ADVSECURITY_NONE))
			{
				return true;
			}
			if((XVar)((XVar)(advSecType == Constants.ADVSECURITY_EDIT_OWN)  && (XVar)(permission != "D"))  && (XVar)(permission != "E"))
			{
				return true;
			}
			currentOwnerId = XVar.Clone(XVar.Pack(CommonFunctions.storageGet((XVar)(MVCFunctions.Concat("_", table, "_OwnerID")))).ToString());
			if(XVar.Pack(Security.caseInsensitiveUsername()))
			{
				ownerId = XVar.Clone(MVCFunctions.strtoupper((XVar)(ownerId)));
				currentOwnerId = XVar.Clone(MVCFunctions.strtoupper((XVar)(currentOwnerId)));
			}
			return XVar.Equals(XVar.Pack(MVCFunctions.Concat("", ownerId)), XVar.Pack(MVCFunctions.Concat("", currentOwnerId)));
		}
		public static XVar userHasFieldPermissions(dynamic _param_table, dynamic _param_field, dynamic _param_pageType, dynamic _param_pageName, dynamic _param_edit)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic pageName = XVar.Clone(_param_pageName);
			dynamic edit = XVar.Clone(_param_edit);
			#endregion

			dynamic pageTable = null, permission = null;
			ProjectSettings pSet;
			pageTable = XVar.Clone(table);
			if((XVar)(XVar.Equals(XVar.Pack(table), XVar.Pack(Security.loginTable())))  && (XVar)((XVar)(XVar.Equals(XVar.Pack(pageType), XVar.Pack("register")))  || (XVar)(XVar.Equals(XVar.Pack(pageType), XVar.Pack("userinfo")))))
			{
				pageTable = new XVar(Constants.GLOBAL_PAGES);
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table), (XVar)(pageType), (XVar)(pageName), (XVar)(pageTable)));
			pageType = XVar.Clone(pSet.getPageType());
			permission = XVar.Clone(Security.pageType2permission((XVar)(pageType)));
			if((XVar)(pageTable != Constants.GLOBAL_PAGES)  && (XVar)(!(XVar)(Security.userCan((XVar)(permission), (XVar)(table)))))
			{
				return false;
			}
			if((XVar)(edit)  && (XVar)(!(XVar)(CommonFunctions.pageTypeInputsData((XVar)(pageType)))))
			{
				if(XVar.Pack(pSet.appearOnSearchPanel((XVar)(field))))
				{
					return true;
				}
				if(pageType == "list")
				{
					return (XVar)((XVar)(pSet.hasInlineEdit())  && (XVar)(pSet.appearOnInlineEdit((XVar)(field))))  || (XVar)((XVar)(pSet.hasInlineAdd())  && (XVar)(pSet.appearOnInlineAdd((XVar)(field))));
				}
			}
			if((XVar)(!(XVar)(edit))  && (XVar)(!(XVar)(CommonFunctions.pageTypeShowsData((XVar)(pageType)))))
			{
				return false;
			}
			return pSet.appearOnPage((XVar)(field));
		}
		public static XVar getRestrictedPages(dynamic _param_table, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic allPages = XVar.Array(), groupRights = XVar.Array(), groups = XVar.Array(), ret = XVar.Array(), userRights = XVar.Array();
			if(XVar.Pack(!(XVar)(Security.permissionsAvailable())))
			{
				return XVar.Array();
			}
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("GetTablePermissions"), (XVar)(table))))
			{
				return XVar.Array();
			}
			if(XVar.Pack(MVCFunctions.is_array((XVar)(XSession.Session["securityOverrides"]))))
			{
				if(XVar.Pack(XSession.Session["securityOverrides"].KeyExists(table)))
				{
					if(XVar.Pack(XSession.Session["securityOverrides"][table].KeyExists("pages")))
					{
						return XSession.Session["securityOverrides"][table]["pages"];
					}
				}
			}
			if(XVar.Pack(!(XVar)(Security.dynamicPermissions())))
			{
				return Security._staticRestrictedPages((XVar)(table));
			}
			userRights = Security.dynamicUserRights();
			groups = userRights[".Groups"];
			allPages = XVar.Clone(pSet.getOriginalPages());
			ret = XVar.Clone(XVar.Array());
			if(XVar.Pack(userRights[table]))
			{
				groupRights = XVar.Clone(userRights[table]["groupRights"]);
			}
			if(XVar.Pack(!(XVar)(groupRights)))
			{
				groupRights = XVar.Clone(XVar.Array());
			}
			foreach (KeyValuePair<XVar, dynamic> pageType in allPages.GetEnumerator())
			{
				dynamic allowed = null, pagePerm = null;
				pagePerm = XVar.Clone(Security.pageType2permission((XVar)(pageType.Value)));
				if(XVar.Pack(!(XVar)(Security.specialPermissionsTable((XVar)(table)))))
				{
					allowed = new XVar(false);
					foreach (KeyValuePair<XVar, dynamic> gr in groupRights.GetEnumerator())
					{
						if((XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(gr.Value["mask"]), (XVar)(pagePerm))), XVar.Pack(false)))  && (XVar)(!(XVar)(gr.Value["pages"][pageType.Key])))
						{
							allowed = new XVar(true);
							break;
						}
					}
				}
				else
				{
					dynamic restricted = null;
					restricted = new XVar(true);
					foreach (KeyValuePair<XVar, dynamic> g in groups.GetEnumerator())
					{
						if(XVar.Pack(!(XVar)(groupRights[g.Value])))
						{
							restricted = new XVar(false);
							break;
						}
						if(XVar.Pack(!(XVar)(groupRights[g.Value]["pages"][pageType.Key])))
						{
							restricted = new XVar(false);
							break;
						}
					}
					allowed = XVar.Clone(!(XVar)(restricted));
				}
				if(XVar.Pack(!(XVar)(allowed)))
				{
					ret.InitAndSetArrayItem(true, pageType.Key);
				}
			}
			return ret;
		}
		public static XVar pageType2permission(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			if(pageType == "add")
			{
				return "A";
			}
			else
			{
				if(pageType == "edit")
				{
					return "E";
				}
				else
				{
					if((XVar)((XVar)((XVar)((XVar)(pageType == "print")  || (XVar)(pageType == "export"))  || (XVar)(pageType == "rprint"))  || (XVar)(pageType == "masterprint"))  || (XVar)(pageType == "masterrprint"))
					{
						return "P";
					}
					else
					{
						if(pageType == "import")
						{
							return "I";
						}
						else
						{
							if((XVar)((XVar)(pageType == Constants.PAGE_REGISTER)  || (XVar)(pageType == Constants.PAGE_USERINFO))  || (XVar)(pageType == Constants.PAGE_LOGIN))
							{
								return "";
							}
						}
					}
				}
			}
			return "S";
		}
		public static XVar _staticRestrictedPages(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic group = null, groupPermissions = XVar.Array(), pages = null, permissions = XVar.Array(), tablePermissions = XVar.Array();
			permissions = ProjectSettings.staticPermissions();
			group = XVar.Clone(Security.getUserGroup());
			if(XVar.Pack(!(XVar)(group)))
			{
				group = new XVar("<Default>");
			}
			groupPermissions = permissions[group];
			if(XVar.Pack(!(XVar)(groupPermissions)))
			{
				return XVar.Array();
			}
			tablePermissions = groupPermissions["permissions"][table];
			if(XVar.Pack(!(XVar)(tablePermissions)))
			{
				return XVar.Array();
			}
			pages = XVar.Clone(tablePermissions["restrictedPages"]);
			if(XVar.Pack(!(XVar)(pages)))
			{
				return XVar.Array();
			}
			return pages;
		}
		public static XVar getAuthPlugin(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			dynamic provider = null;
			provider = XVar.Clone(Security.findProvider((XVar)(code)));
			if(XVar.Pack(!(XVar)(provider)))
			{
				return null;
			}
			return Security.PluginFactory((XVar)(provider));
		}
		protected static XVar PluginFactory(dynamic _param_providerParams)
		{
			#region pass-by-value parameters
			dynamic providerParams = XVar.Clone(_param_providerParams);
			#endregion

			dynamic providerType = null;
			providerType = XVar.Clone(providerParams["type"]);
			if(providerType == Constants.stFACEBOOK)
			{
				return new SecurityPluginFB((XVar)(providerParams));
			}
			if(providerType == Constants.stGOOGLE)
			{
				return new SecurityPluginGoogle((XVar)(providerParams));
			}
			if(providerType == Constants.stOPENID)
			{
				return new SecurityPluginOpenId((XVar)(providerParams));
			}
			if(providerType == Constants.stSAML)
			{
				return new SecurityPluginSaml((XVar)(providerParams));
			}
			if(providerType == Constants.stOKTA)
			{
				return new SecurityPluginOkta((XVar)(providerParams));
			}
			if(providerType == Constants.stAZURE)
			{
				return new SecurityPluginAzure((XVar)(providerParams));
			}
			if(providerType == Constants.stAD)
			{
				return new SecurityPluginAd((XVar)(providerParams));
			}
			return null;
		}
		public static XVar GetPlugins()
		{
			dynamic plugins = XVar.Array(), providersCfg = XVar.Array();
			plugins = XVar.Clone(XVar.Array());
			providersCfg = XVar.Clone(new XVar(Constants.stFACEBOOK, "fb", Constants.stGOOGLE, "go"));
			foreach (KeyValuePair<XVar, dynamic> code in providersCfg.GetEnumerator())
			{
				dynamic providers = XVar.Array();
				providers = XVar.Clone(Security.providersByType((XVar)(code.Key)));
				if((XVar)(plugins[code.Value] == null)  && (XVar)(MVCFunctions.count(providers) != 0))
				{
					plugins.InitAndSetArrayItem(Security.PluginFactory((XVar)(providers[0])), code.Value);
				}
			}
			return plugins;
		}
		public static XVar getLoginTable()
		{
			return Security.loginTable();
		}
		public static XVar userCanSeePage(dynamic _param_table, dynamic _param_page)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic page = XVar.Clone(_param_page);
			#endregion

			dynamic permission = null, strPerm = null;
			ProjectSettings pSet;
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table), new XVar(""), (XVar)(page)));
			if(pSet.pageName() != page)
			{
				return false;
			}
			if(table == Constants.GLOBAL_PAGES)
			{
				return true;
			}
			permission = XVar.Clone(Security.pageType2permission((XVar)(pSet.getPageType())));
			if(XVar.Pack(!(XVar)(permission)))
			{
				return true;
			}
			strPerm = XVar.Clone(CommonFunctions.GetUserPermissions((XVar)(table)));
			return !XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), (XVar)(permission))), XVar.Pack(false));
		}
		public static XVar SelectCondition(dynamic _param_strRequestedPremission, dynamic _param_pSet_packed, dynamic _param_skipTablePermissions = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_skipTablePermissions as Object == null) _param_skipTablePermissions = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic strRequestedPremission = XVar.Clone(_param_strRequestedPremission);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic skipTablePermissions = XVar.Clone(_param_skipTablePermissions);
			#endregion

			dynamic ownerid = null, strPerm = null, tableAdvSecurity = null;
			if(XVar.Pack(!(XVar)(pSet)))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(Security.hasUsers())))
			{
				return null;
			}
			strPerm = XVar.Clone(CommonFunctions.GetUserPermissions((XVar)(pSet.table())));
			if((XVar)(!(XVar)(skipTablePermissions))  && (XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), (XVar)(strRequestedPremission))), XVar.Pack(false))))
			{
				return DataCondition._False();
			}
			ownerid = XVar.Clone(CommonFunctions.storageGet((XVar)(MVCFunctions.Concat("_", pSet.table(), "_OwnerID"))));
			tableAdvSecurity = XVar.Clone(pSet.getAdvancedSecurityType());
			if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(strPerm), new XVar("M"))), XVar.Pack(false)))
			{
				return null;
			}
			if((XVar)(tableAdvSecurity == Constants.ADVSECURITY_VIEW_OWN)  || (XVar)((XVar)(tableAdvSecurity == Constants.ADVSECURITY_EDIT_OWN)  && (XVar)((XVar)(strRequestedPremission == "E")  || (XVar)(strRequestedPremission == "D"))))
			{
				return DataCondition.FieldEquals((XVar)(pSet.getTableOwnerIdField()), (XVar)(ownerid));
			}
			return null;
		}
		public static XVar getUgMembersDatasource()
		{
			if(XVar.Pack(!(XVar)(Security.dynamicPermissions())))
			{
				return null;
			}
			return CommonFunctions.getDbTableDataSource((XVar)(MVCFunctions.Concat(ProjectSettings.getSecurityValue(new XVar("dpTablePrefix")), "ugmembers")), (XVar)(GlobalVars.cman.getUserGroupsConnId()));
		}
		public static XVar getUgGroupsDatasource()
		{
			if(XVar.Pack(!(XVar)(Security.dynamicPermissions())))
			{
				return null;
			}
			return CommonFunctions.getDbTableDataSource((XVar)(MVCFunctions.Concat(ProjectSettings.getSecurityValue(new XVar("dpTablePrefix")), "uggroups")), (XVar)(GlobalVars.cman.getUserGroupsConnId()));
		}
		public static XVar getUgRightsDatasource()
		{
			if(XVar.Pack(!(XVar)(Security.dynamicPermissions())))
			{
				return null;
			}
			return CommonFunctions.getDbTableDataSource((XVar)(MVCFunctions.Concat(ProjectSettings.getSecurityValue(new XVar("dpTablePrefix")), "ugrights")), (XVar)(GlobalVars.cman.getUserGroupsConnId()));
		}
		public static XVar getOpenIdConfiguration(dynamic _param_configurationUrl)
		{
			#region pass-by-value parameters
			dynamic configurationUrl = XVar.Clone(_param_configurationUrl);
			#endregion

			dynamic var_response = XVar.Array();
			var_response = XVar.Clone(MVCFunctions.runner_http_request((XVar)(configurationUrl)));
			return MVCFunctions.runner_json_decode((XVar)(var_response["content"]));
		}
		public static XVar getOpenIdJWK(dynamic _param_jwt, dynamic _param_wellKnown)
		{
			#region pass-by-value parameters
			dynamic jwt = XVar.Clone(_param_jwt);
			dynamic wellKnown = XVar.Clone(_param_wellKnown);
			#endregion

			dynamic keyData = XVar.Array(), keys = XVar.Array(), parts = XVar.Array(), tokenHeader = XVar.Array(), var_response = XVar.Array();
			var_response = XVar.Clone(MVCFunctions.runner_http_request((XVar)(wellKnown["jwks_uri"])));
			keyData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(var_response["content"])));
			keys = XVar.Clone(keyData["keys"]);
			parts = XVar.Clone(MVCFunctions.explode(new XVar("."), (XVar)(jwt)));
			tokenHeader = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(CommonFunctions.base64_decode_url((XVar)(parts[0])))));
			if(XVar.Pack(tokenHeader.KeyExists("kid")))
			{
				foreach (KeyValuePair<XVar, dynamic> key in keys.GetEnumerator())
				{
					if(key.Value["kid"] == tokenHeader["kid"])
					{
						return key.Value;
					}
				}
				return null;
			}
			if((XVar)(MVCFunctions.count(keys) == 1)  && (XVar)(!(XVar)(MVCFunctions.strlen((XVar)(keys[0]["kid"])))))
			{
				return keys[0];
			}
			return null;
		}
		public static XVar openIdVerifyToken(dynamic _param_jwt, dynamic _param_jwk)
		{
			#region pass-by-value parameters
			dynamic jwt = XVar.Clone(_param_jwt);
			dynamic jwk = XVar.Clone(_param_jwk);
			#endregion

			if((XVar)(!(XVar)(jwt))  || (XVar)(!(XVar)(jwk)))
			{
				return false;
			}
			return MVCFunctions.verifyOpenIdToken((XVar)(jwt), (XVar)(jwk));
		}
		public static XVar parseJWT(dynamic _param_jwt)
		{
			#region pass-by-value parameters
			dynamic jwt = XVar.Clone(_param_jwt);
			#endregion

			dynamic parts = XVar.Array();
			parts = XVar.Clone(MVCFunctions.explode(new XVar("."), (XVar)(jwt)));
			if(MVCFunctions.count(parts) != 3)
			{
				return false;
			}
			return new XVar("header", MVCFunctions.runner_json_decode((XVar)(CommonFunctions.base64_decode_url((XVar)(parts[0])))), "payload", MVCFunctions.runner_json_decode((XVar)(CommonFunctions.base64_decode_url((XVar)(parts[1])))), "signature", CommonFunctions.base64_decode_url_binary((XVar)(parts[2])));
		}
		public static XVar twoFactorAvailable()
		{
			dynamic settings = XVar.Array();
			if(XVar.Pack(CommonFunctions.inRestApi()))
			{
				return false;
			}
			settings = Security.twoFactorSettings();
			return !(XVar)(!(XVar)(settings["available"]));
		}
		public static XVar twoFactorSettings()
		{
			dynamic db = XVar.Array(), emailField = null, ret = XVar.Array();
			ret = ProjectSettings.getSecurityValue(new XVar("twoFactorSettings"));
			if(XVar.Pack(!(XVar)(ret["available"])))
			{
				return XVar.Array();
			}
			db = Security.dbProvider();
			if((XVar)(!(XVar)(db))  || (XVar)(!(XVar)(ret)))
			{
				return XVar.Array();
			}
			emailField = XVar.Clone(ProjectSettings.twoFactorValue(new XVar("emailField")));
			if(XVar.Pack(!(XVar)(emailField)))
			{
				emailField = XVar.Clone(db["emailField"]);
			}
			ret.InitAndSetArrayItem(emailField, "emailField");
			return ret;
		}
		public static XVar twoFactorMethod(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(XVar.BitwiseAnd(XVar.Pack((int)value), XVar.Pack(2)))
			{
				return "email";
			}
			if(XVar.BitwiseAnd(XVar.Pack((int)value), XVar.Pack(4)))
			{
				return "phone";
			}
			if(XVar.BitwiseAnd(XVar.Pack((int)value), XVar.Pack(8)))
			{
				return "totp";
			}
			return "";
		}
		public static XVar twoFactorMethodEnabled(dynamic _param_value, dynamic _param_method)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic method = XVar.Clone(_param_method);
			#endregion

			dynamic mask = null, masks = XVar.Array();
			if(XVar.Pack(!(XVar)(Security.twoFactorEnabled((XVar)(value)))))
			{
				return false;
			}
			mask = new XVar(0);
			masks = XVar.Clone(Security.twoFactorMasks());
			mask = XVar.Clone(masks[method]);
			if(XVar.Pack(!(XVar)(mask)))
			{
				return false;
			}
			return !(XVar)(!(XVar)(XVar.BitwiseAnd(XVar.Pack((int)value), XVar.Pack(mask))));
		}
		public static XVar twoFactorMethodAvailable(dynamic _param_method)
		{
			#region pass-by-value parameters
			dynamic method = XVar.Clone(_param_method);
			#endregion

			dynamic name = null, twofSettings = XVar.Array();
			twofSettings = Security.twoFactorSettings();
			if(XVar.Pack(!(XVar)(twofSettings["available"])))
			{
				return false;
			}
			if(method == Constants.TWOFACTOR_EMAIL)
			{
				name = new XVar("email");
			}
			else
			{
				if(method == Constants.TWOFACTOR_PHONE)
				{
					name = new XVar("phone");
				}
				else
				{
					if(method == Constants.TWOFACTOR_APP)
					{
						name = new XVar("totp");
					}
					else
					{
						return false;
					}
				}
			}
			return !(XVar)(!(XVar)(twofSettings["types"][name]));
		}
		public static XVar twoFactorAllMethods()
		{
			return new XVar(0, Constants.TWOFACTOR_EMAIL, 1, Constants.TWOFACTOR_PHONE, 2, Constants.TWOFACTOR_APP);
		}
		public static XVar twoFactorMasks()
		{
			return new XVar(Constants.TWOFACTOR_EMAIL, 2, Constants.TWOFACTOR_PHONE, 4, Constants.TWOFACTOR_APP, 8);
		}
		public static XVar twoFactorAvailableMethods()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> m in Security.twoFactorAllMethods().GetEnumerator())
			{
				if(XVar.Pack(Security.twoFactorMethodAvailable((XVar)(m.Value))))
				{
					ret.InitAndSetArrayItem(m.Value, null);
				}
			}
			return ret;
		}
		public static XVar twoFactorEnabledMethods(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic methods = XVar.Array(), ret = XVar.Array();
			methods = XVar.Clone(Security.twoFactorAvailableMethods());
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> m in methods.GetEnumerator())
			{
				if(XVar.Pack(Security.twoFactorMethodEnabled((XVar)(value), (XVar)(m.Value))))
				{
					ret.InitAndSetArrayItem(true, m.Value);
				}
			}
			return ret;
		}
		public static XVar twoFactorPreferredMethod(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic method = null, methods = XVar.Array();
			method = XVar.Clone((XVar.BitwiseAnd(XVar.Pack((int)value), XVar.Pack(48))) / 16);
			if(XVar.Pack(Security.twoFactorMethodEnabled((XVar)(value), (XVar)(method))))
			{
				return method;
			}
			methods = XVar.Clone(Security.twoFactorAvailableMethods());
			foreach (KeyValuePair<XVar, dynamic> m in methods.GetEnumerator())
			{
				if(XVar.Pack(Security.twoFactorMethodEnabled((XVar)(value), (XVar)(m.Value))))
				{
					return m.Value;
				}
			}
			return 0;
		}
		public static XVar twoFactorEnabled(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			return XVar.BitwiseAnd(XVar.Pack(value), XVar.Pack(1));
		}
		public static XVar getTwoFactorValue(dynamic _param_methods, dynamic _param_preferred)
		{
			#region pass-by-value parameters
			dynamic methods = XVar.Clone(_param_methods);
			dynamic preferred = XVar.Clone(_param_preferred);
			#endregion

			dynamic masks = XVar.Array(), value = null;
			value = new XVar(0);
			masks = XVar.Clone(Security.twoFactorMasks());
			foreach (KeyValuePair<XVar, dynamic> m in Security.twoFactorAvailableMethods().GetEnumerator())
			{
				if(XVar.Pack(methods[m.Value]))
				{
					value = XVar.Clone(XVar.BitwiseOr(XVar.Pack(value), XVar.Pack(masks[m.Value])));
				}
			}
			if(XVar.Pack(value))
			{
				value = XVar.Clone(XVar.BitwiseOr(XVar.Pack(value), XVar.Pack(1)));
			}
			value = XVar.Clone(XVar.BitwiseOr(XVar.Pack(value), XVar.Pack(XVar.BitwiseAnd(XVar.Pack((int)preferred * 16), XVar.Pack(48)))));
			return value;
		}
		public static XVar prepareTwoFactorMessage(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			dynamic smsText = null;
			smsText = XVar.Clone(MVCFunctions.myfile_get_contents((XVar)(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat("email/", CommonFunctions.mlang_getcurrentlang(), "/twofactorauth.txt")))), new XVar("r")));
			return MVCFunctions.str_replace(new XVar("%code%"), (XVar)(code), (XVar)(smsText));
		}
		public static XVar allowAutoLogin()
		{
			dynamic twoFactorValue = null, twofSettings = XVar.Array(), userData = XVar.Array();
			userData = Security.provisionalUserData();
			twofSettings = Security.twoFactorSettings();
			twoFactorValue = XVar.Clone(userData[twofSettings["twoFactorField"]]);
			return !(XVar)(Security.twoFactorEnabled((XVar)(twoFactorValue)));
		}
		public static XVar guestLoginAvailable()
		{
			if((XVar)(ProjectSettings.getSecurityValue(new XVar("advancedSecurity"), new XVar("allowGuestLogin")))  || (XVar)(Security.dynamicPermissions()))
			{
				return CommonFunctions.guestHasPermissions();
			}
			return false;
		}
		public static XVar fillTablesOwnerId(dynamic _param_data, dynamic _param_provider)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic provider = XVar.Clone(_param_provider);
			#endregion

			dynamic adMode = null, ownerId = null, securityType = null, tablesAdvSecurity = XVar.Array(), userId = null;
			if(XVar.Pack(!(XVar)(Security.advancedSecurityAvailable())))
			{
				return null;
			}
			securityType = new XVar(Constants.stNONE);
			if(XVar.Pack(provider))
			{
				securityType = XVar.Clone(provider["type"]);
			}
			tablesAdvSecurity = ProjectSettings.getProjectValue(new XVar("tablesAdvSecurity"));
			adMode = XVar.Clone((XVar)(securityType == Constants.stAD)  && (XVar)(!(XVar)(provider["useDbGroups"])));
			userId = XVar.Clone(Security.getUserName());
			if(XVar.Pack(adMode))
			{
				CommonFunctions.storageSet(new XVar("OwnerID"), (XVar)(userId));
			}
			foreach (KeyValuePair<XVar, dynamic> advSec in tablesAdvSecurity.GetEnumerator())
			{
				if((XVar)(!XVar.Equals(XVar.Pack(advSec.Value["advSecurity"]), XVar.Pack(Constants.ADVSECURITY_VIEW_OWN)))  && (XVar)(!XVar.Equals(XVar.Pack(advSec.Value["advSecurity"]), XVar.Pack(Constants.ADVSECURITY_EDIT_OWN))))
				{
					continue;
				}
				ownerId = XVar.Clone((XVar.Pack(adMode) ? XVar.Pack(userId) : XVar.Pack(data[advSec.Value["usersOwnerIdField"]])));
				CommonFunctions.storageSet((XVar)(MVCFunctions.Concat("_", advSec.Key, "_OwnerID")), (XVar)(ownerId));
			}
			CommonFunctions.storageSet(new XVar("OwnerID"), (XVar)(ownerId));

			return null;
		}
		public static XVar createProvisionalSession(dynamic _param_provider, dynamic _param_sessionLevel, dynamic _param_userId, dynamic _param_displayName = null, dynamic _param_userData = null)
		{
			#region default values
			if(_param_displayName as Object == null) _param_displayName = new XVar("");
			if(_param_userData as Object == null) _param_userData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic provider = XVar.Clone(_param_provider);
			dynamic sessionLevel = XVar.Clone(_param_sessionLevel);
			dynamic userId = XVar.Clone(_param_userId);
			dynamic displayName = XVar.Clone(_param_displayName);
			dynamic userData = XVar.Clone(_param_userData);
			#endregion

			Security.createUserSession((XVar)(provider), (XVar)(userId), (XVar)(displayName), (XVar)(userData));
			CommonFunctions.storageDelete(new XVar("UserID"));
			CommonFunctions.storageSet(new XVar("SessionLevel"), (XVar)(sessionLevel));
			CommonFunctions.storageSet(new XVar("ProvisionalUserID"), (XVar)(userId));

			return null;
		}
		public static XVar elevateSession(dynamic _param_level = null)
		{
			#region default values
			if(_param_level as Object == null) _param_level = new XVar(Constants.LOGGED_FULL);
			#endregion

			#region pass-by-value parameters
			dynamic level = XVar.Clone(_param_level);
			#endregion

			dynamic userId = null;
			userId = XVar.Clone(CommonFunctions.storageGet(new XVar("ProvisionalUserID")));
			if(XVar.Pack(!(XVar)(userId)))
			{
				return false;
			}
			CommonFunctions.storageSet(new XVar("SessionLevel"), (XVar)(level));
			if(level == Constants.LOGGED_FULL)
			{
				CommonFunctions.storageSet(new XVar("UserID"), (XVar)(userId));
				CommonFunctions.storageDelete(new XVar("ProvisionalUserID"));
			}
			return true;
		}
		public static XVar createHardcodedSession()
		{
			return Security.createUserSession((XVar)(Security.hardcodedProvider()), (XVar)(Security.hardcodedUsername()));
		}
		public static XVar createUserSession(dynamic _param_provider, dynamic _param_userId, dynamic _param_displayName = null, dynamic _param_userData = null, dynamic _param_adGroups = null, dynamic _param_autoLogin = null)
		{
			#region default values
			if(_param_displayName as Object == null) _param_displayName = new XVar("");
			if(_param_userData as Object == null) _param_userData = new XVar(XVar.Array());
			if(_param_adGroups as Object == null) _param_adGroups = new XVar(XVar.Array());
			if(_param_autoLogin as Object == null) _param_autoLogin = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic provider = XVar.Clone(_param_provider);
			dynamic userId = XVar.Clone(_param_userId);
			dynamic displayName = XVar.Clone(_param_displayName);
			dynamic userData = XVar.Clone(_param_userData);
			dynamic adGroups = XVar.Clone(_param_adGroups);
			dynamic autoLogin = XVar.Clone(_param_autoLogin);
			#endregion

			dynamic accessLevel = null, providerCode = null, runnerSession = null, securityType = null;
			securityType = new XVar(Constants.stNONE);
			if(XVar.Pack(provider))
			{
				securityType = XVar.Clone(provider["type"]);
			}
			if((XVar)(userId)  && (XVar)(!XVar.Equals(XVar.Pack(Security.getUserName()), XVar.Pack(userId))))
			{
				MVCFunctions.regenerateSessionId();
				CommonFunctions.loadLanguage((XVar)(CommonFunctions.mlang_getcurrentlang()));
			}
			accessLevel = new XVar(Constants.ACCESS_LEVEL_USER);
			if(userId == XVar.Pack(""))
			{
				userId = new XVar("Guest");
				accessLevel = new XVar(Constants.ACCESS_LEVEL_GUEST);
				displayName = XVar.Clone(CommonFunctions.mlang_message(new XVar("AA_GROUP_GUEST")));
			}
			if(displayName == XVar.Pack(""))
			{
				displayName = XVar.Clone(userId);
			}
			providerCode = new XVar("");
			if((XVar)(!XVar.Equals(XVar.Pack(accessLevel), XVar.Pack(Constants.ACCESS_LEVEL_GUEST)))  && (XVar)(provider))
			{
				providerCode = XVar.Clone(provider["code"]);
			}
			CommonFunctions.storageSet(new XVar("SessionLevel"), new XVar(Constants.LOGGED_FULL));
			CommonFunctions.storageSet(new XVar("UserID"), (XVar)(userId));
			CommonFunctions.storageSet(new XVar("AccessLevel"), (XVar)(accessLevel));
			CommonFunctions.storageSet(new XVar("providerCode"), (XVar)(providerCode));
			CommonFunctions.storageSet(new XVar("UserName"), (XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(displayName))));
			if(XVar.Equals(XVar.Pack(securityType), XVar.Pack(Constants.stAD)))
			{
				CommonFunctions.storageSet(new XVar("GroupID"), (XVar)(MVCFunctions.implode(new XVar(","), (XVar)(adGroups))));
			}
			else
			{
				dynamic userGroupField = null;
				userGroupField = XVar.Clone(Security.groupIdField());
				CommonFunctions.storageSet(new XVar("GroupID"), (XVar)((XVar.Pack((XVar)(Security.permissionsAvailable())  && (XVar)(userGroupField)) ? XVar.Pack(userData[userGroupField]) : XVar.Pack(""))));
			}
			if(XVar.Equals(XVar.Pack(accessLevel), XVar.Pack(Constants.ACCESS_LEVEL_GUEST)))
			{
				CommonFunctions.storageSet(new XVar("GroupID"), new XVar("<Guest>"));
			}
			runnerSession = XVar.Clone(CommonFunctions.generatePassword(new XVar(20)));
			CommonFunctions.storageSet(new XVar("runnerSession"), (XVar)(runnerSession));
			CommonFunctions.setProjectCookie(new XVar("runnerSession"), (XVar)(runnerSession), new XVar(0), new XVar(true), new XVar(true));
			MVCFunctions.setCookieDirectly(new XVar("runnerSession"), (XVar)(runnerSession));
			Security.fillTablesOwnerId((XVar)(userData), (XVar)(provider));
			CommonFunctions.storageSet(new XVar("UserData"), (XVar)(userData));
			CommonFunctions.storageSet(new XVar("AutomaticLogin"), (XVar)(autoLogin));

			return null;
		}
		public static XVar readDatabaseUserGroups(dynamic _param_provider, dynamic _param_userId)
		{
			#region pass-by-value parameters
			dynamic provider = XVar.Clone(_param_provider);
			dynamic userId = XVar.Clone(_param_userId);
			#endregion

			dynamic data = XVar.Array(), dataSource = null, dc = null, groups = XVar.Array(), providerField = null, qResult = null;
			dataSource = XVar.Clone(Security.getUgMembersDatasource());
			dc = XVar.Clone(Security.userGroupsCommand((XVar)(provider), (XVar)(userId)));
			qResult = XVar.Clone(dataSource.getList((XVar)(dc)));
			groups = XVar.Clone(XVar.Array());
			providerField = new XVar("Provider");
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				groups.InitAndSetArrayItem(data["GroupID"], null);
			}
			CommonFunctions.storageSet(new XVar("members_provider_field"), (XVar)(qResult.fieldExists((XVar)(providerField))));
			return groups;
		}
		protected static XVar userGroupsCommand(dynamic _param_provider, dynamic _param_userId)
		{
			#region pass-by-value parameters
			dynamic provider = XVar.Clone(_param_provider);
			dynamic userId = XVar.Clone(_param_userId);
			#endregion

			dynamic dc = null, usernameFilter = null;
			if(provider["type"] == Constants.stAD)
			{
				userId = XVar.Clone(MVCFunctions.Concat(provider["code"], userId));
			}
			dc = XVar.Clone(new DsCommand());
			usernameFilter = XVar.Clone(DataCondition.FieldEquals(new XVar("UserName"), (XVar)(userId), new XVar(0), (XVar)((XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_STRICT)))));
			if(provider["type"] == Constants.stDB)
			{
				dc.filter = XVar.Clone(usernameFilter);
			}
			else
			{
				dynamic providerFilter = null;
				providerFilter = XVar.Clone(DataCondition.FieldEquals(new XVar("Provider"), (XVar)(provider["code"]), new XVar(0)));
				dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, usernameFilter, 1, providerFilter))));
			}
			return dc;
		}
		protected static XVar readADUserGroups(dynamic _param_provider)
		{
			#region pass-by-value parameters
			dynamic provider = XVar.Clone(_param_provider);
			#endregion

			dynamic atIdx = null, data = XVar.Array(), dataSource = null, dc = null, groups = XVar.Array(), providerField = null, qResult = null, userGroups = XVar.Array(), userId = null, verifyProvider = null;
			userId = XVar.Clone(Security.getUserName());
			userGroups = XVar.Clone(MVCFunctions.explode(new XVar(","), (XVar)(CommonFunctions.storageGet(new XVar("GroupID")))));
			userGroups.InitAndSetArrayItem(userId, null);
			atIdx = XVar.Clone(MVCFunctions.strpos((XVar)(userId), new XVar("@")));
			if(!XVar.Equals(XVar.Pack(atIdx), XVar.Pack(false)))
			{
				userGroups.InitAndSetArrayItem(MVCFunctions.substr((XVar)(userId), new XVar(0), (XVar)(atIdx)), null);
			}
			dataSource = XVar.Clone(Security.getUgGroupsDatasource());
			providerField = new XVar("Provider");
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition.FieldInList(new XVar("Label"), (XVar)(userGroups), (XVar)((XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_STRICT)))));
			qResult = XVar.Clone(dataSource.getList((XVar)(dc)));
			CommonFunctions.storageSet(new XVar("groups_provider_field"), (XVar)(qResult.fieldExists((XVar)(providerField))));
			verifyProvider = XVar.Clone((XVar)(qResult.fieldExists((XVar)(providerField)))  && (XVar)(!(XVar)(Security.ADonlyLogin())));
			groups = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				if((XVar)(verifyProvider)  && (XVar)(data[providerField] != provider["code"]))
				{
					continue;
				}
				groups.InitAndSetArrayItem(data["GroupID"], null);
			}
			return groups;
		}
		protected static XVar ADUserisAdmin()
		{
			dynamic adminUsers = XVar.Array(), provider = XVar.Array(), userGroups = XVar.Array(), userId = null;
			provider = XVar.Clone(Security.currentProvider());
			if(!XVar.Equals(XVar.Pack(provider["type"]), XVar.Pack(Constants.stAD)))
			{
				return false;
			}
			adminUsers = XVar.Clone(provider["admins"]);
			if(XVar.Pack(!(XVar)(adminUsers)))
			{
				return false;
			}
			userId = XVar.Clone(Security.getUserName());
			if(XVar.Pack(Security.caseInsensitiveUsername()))
			{
				foreach (KeyValuePair<XVar, dynamic> i in MVCFunctions.array_keys((XVar)(adminUsers)).GetEnumerator())
				{
					adminUsers.InitAndSetArrayItem(MVCFunctions.strtoupper((XVar)(adminUsers[i.Value])), i.Value);
				}
				userId = XVar.Clone(MVCFunctions.strtoupper((XVar)(userId)));
			}
			if(XVar.Pack(MVCFunctions.in_array((XVar)(userId), (XVar)(adminUsers))))
			{
				return true;
			}
			userGroups = XVar.Clone(MVCFunctions.explode(new XVar(","), (XVar)(CommonFunctions.storageGet(new XVar("GroupID")))));
			foreach (KeyValuePair<XVar, dynamic> g in userGroups.GetEnumerator())
			{
				dynamic gName = null;
				gName = XVar.Clone((XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack(MVCFunctions.strtoupper((XVar)(g.Value))) : XVar.Pack(g.Value)));
				if(XVar.Pack(MVCFunctions.in_array((XVar)(gName), (XVar)(adminUsers))))
				{
					return true;
				}
			}
			return false;
		}
		public static XVar dynamicUserRights()
		{
			Security.readUserPermissions();
			return Security.userRightsStorage();
		}
		public static XVar userRightsStorage(dynamic _param_userId = null)
		{
			#region default values
			if(_param_userId as Object == null) _param_userId = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic userId = XVar.Clone(_param_userId);
			#endregion

			dynamic userRights = XVar.Array();
			if(userId == XVar.Pack(""))
			{
				userId = XVar.Clone(Security.getUserName());
			}
			userRights = CommonFunctions.storageFindOrCreate(new XVar("UserRights"));
			if(XVar.Pack(!(XVar)(userRights.KeyExists(userId))))
			{
				userRights.InitAndSetArrayItem(XVar.Array(), userId);
			}
			return userRights[userId];
		}
		public static XVar fetchDynamicPermissions(dynamic _param_groups)
		{
			#region pass-by-value parameters
			dynamic groups = XVar.Clone(_param_groups);
			#endregion

			dynamic dataSource = null, dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, DataCondition.FieldInList(new XVar("GroupID"), (XVar)(groups)), 1, DataCondition._Not((XVar)(DataCondition.FieldIs(new XVar("AccessMask"), new XVar(Constants.dsopEMPTY), new XVar(""))))))));
			dataSource = XVar.Clone(Security.getUgRightsDatasource());
			return dataSource.getList((XVar)(dc));
		}
		protected static XVar readUserPermissions()
		{
			dynamic currentMask = null, data = XVar.Array(), group = null, groupRights = null, groups = XVar.Array(), i = null, mask = null, needreload = null, perm = null, qResult = null, readAdGroups = null, restrictedPages = null, sessionRights = XVar.Array(), table = null, userId = null, userRights = XVar.Array();
			if(XVar.Pack(!(XVar)(GlobalVars.gReadPermissions)))
			{
				return null;
			}
			userId = XVar.Clone(Security.getUserName());
			sessionRights = Security.userRightsStorage();
			needreload = XVar.Clone(!(XVar)(sessionRights));
			if((XVar)(!(XVar)(needreload))  && (XVar)((XVar)(GlobalVars.gPermissionsRead)  || (XVar)(MVCFunctions.time() - CommonFunctions.storageGet(new XVar("LastReadRights")) <= GlobalVars.gPermissionsRefreshTime)))
			{
				return null;
			}
			userRights = CommonFunctions.storageFindOrCreate(new XVar("UserRights"));
			userRights.InitAndSetArrayItem(XVar.Array(), Security.getUserName());
			sessionRights = userRights[Security.getUserName()];
			groups = XVar.Clone(XVar.Array());
			if((XVar)(Security.isGuest())  || (XVar)(userId == XVar.Pack("")))
			{
				groups.InitAndSetArrayItem(-3, null);
			}
			else
			{
				dynamic provider = XVar.Array(), readDbGroups = null;
				provider = XVar.Clone(Security.currentProvider());
				readAdGroups = new XVar(false);
				readDbGroups = new XVar(false);
				if(XVar.Pack(provider))
				{
					dynamic securityType = null;
					securityType = XVar.Clone(provider["type"]);
					if(provider["type"] == Constants.stAD)
					{
						readAdGroups = XVar.Clone(provider["useAdGroups"]);
						readDbGroups = XVar.Clone(provider["useDbGroups"]);
					}
					else
					{
						readDbGroups = new XVar(true);
					}
				}
				groups = XVar.Clone(XVar.Array());
				if(XVar.Pack(readDbGroups))
				{
					groups = XVar.Clone(Security.readDatabaseUserGroups((XVar)(provider), (XVar)(userId)));
				}
				if(XVar.Pack(readAdGroups))
				{
					foreach (KeyValuePair<XVar, dynamic> g in Security.readADUserGroups((XVar)(provider)).GetEnumerator())
					{
						groups.InitAndSetArrayItem(g.Value, null);
					}
				}
				if((XVar)((XVar)(readAdGroups)  && (XVar)(!(XVar)(readDbGroups)))  && (XVar)(Security.ADUserisAdmin()))
				{
					if(XVar.Pack(!(XVar)(MVCFunctions.in_array(new XVar(-1), (XVar)(groups)))))
					{
						groups.InitAndSetArrayItem(-1, null);
					}
				}
				if(XVar.Pack(!(XVar)(groups)))
				{
					groups.InitAndSetArrayItem(-2, null);
				}
			}
			qResult = XVar.Clone(Security.fetchDynamicPermissions((XVar)(groups)));
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				table = XVar.Clone(data["TableName"]);
				mask = XVar.Clone(data["AccessMask"]);
				group = XVar.Clone(data["GroupID"]);
				restrictedPages = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(data["Page"])));
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(restrictedPages)))))
				{
					restrictedPages = XVar.Clone(XVar.Array());
				}
				groupRights = XVar.Clone(new XVar("mask", mask, "pages", restrictedPages));
				if(XVar.Pack(!(XVar)(sessionRights.KeyExists(table))))
				{
					sessionRights.InitAndSetArrayItem(new XVar("mask", mask, "groupRights", new XVar(group, groupRights)), table);
					continue;
				}
				sessionRights.InitAndSetArrayItem(groupRights, table, "groupRights", group);
				currentMask = XVar.Clone(sessionRights[table]["mask"]);
				i = new XVar(0);
				for(;i < MVCFunctions.strlen((XVar)(mask)); i++)
				{
					perm = XVar.Clone(MVCFunctions.substr((XVar)(mask), (XVar)(i), new XVar(1)));
					if(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(currentMask), (XVar)(perm))), XVar.Pack(false)))
					{
						sessionRights[table]["mask"] = MVCFunctions.Concat(sessionRights[table]["mask"], perm);
					}
				}
			}
			sessionRights.InitAndSetArrayItem((XVar)(MVCFunctions.in_array(new XVar(-1), (XVar)(groups)))  || (XVar)((XVar)(readAdGroups)  && (XVar)(Security.ADUserisAdmin())), ".IsAdmin");
			sessionRights.InitAndSetArrayItem(groups, ".Groups");
			CommonFunctions.storageSet(new XVar("LastReadRights"), (XVar)(MVCFunctions.time()));
			GlobalVars.gPermissionsRead = new XVar(true);

			return null;
		}
		public static XVar guestHasStaticPermissions()
		{
			dynamic guestPermissions = XVar.Array(), permissions = XVar.Array(), tablesAdvSecurity = XVar.Array();
			permissions = ProjectSettings.staticPermissions();
			guestPermissions = permissions["<Guest>"];
			if((XVar)(!(XVar)(guestPermissions))  || (XVar)(!(XVar)(guestPermissions["permissions"])))
			{
				return false;
			}
			tablesAdvSecurity = ProjectSettings.getProjectValue(new XVar("tablesAdvSecurity"));
			foreach (KeyValuePair<XVar, dynamic> p in guestPermissions["permissions"].GetEnumerator())
			{
				dynamic advSec = XVar.Array();
				advSec = XVar.Clone(tablesAdvSecurity[p.Key]);
				if((XVar)((XVar)(p.Value["mask"])  && (XVar)(advSec))  && (XVar)(!XVar.Equals(XVar.Pack(advSec["advSecurity"]), XVar.Pack(Constants.ADVSECURITY_VIEW_OWN))))
				{
					return true;
				}
			}
			return false;
		}
		public static XVar guestHasDynamicPermissions()
		{
			dynamic data = XVar.Array(), result = null, tables = null;
			result = XVar.Clone(Security.fetchDynamicPermissions((XVar)(new XVar(0, -3))));
			tables = CommonFunctions.GetTablesListWithoutSecurity();
			while(XVar.Pack(data = XVar.Clone(result.fetchAssoc())))
			{
				if(XVar.Pack(MVCFunctions.in_array((XVar)(data["TableName"]), (XVar)(tables))))
				{
					return true;
				}
			}
			return false;
		}
		public static XVar auditLoginSuccess()
		{
			dynamic auditObj = null;
			auditObj = XVar.Clone(CommonFunctions.GetAuditObject());
			if(XVar.Pack(!(XVar)(auditObj)))
			{
				return null;
			}
			auditObj.LogLogin((XVar)(Security.getUserName()));
			auditObj.LoginSuccessful();

			return null;
		}
		public static XVar auditLoginFail(dynamic _param_username)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			#endregion

			dynamic auditObj = null;
			auditObj = XVar.Clone(CommonFunctions.GetAuditObject());
			if(XVar.Pack(!(XVar)(auditObj)))
			{
				return null;
			}
			auditObj.LogLoginFailed((XVar)(username));
			auditObj.LoginUnsuccessful((XVar)(username));

			return null;
		}
		protected static XVar createFetchUserCommand(dynamic _param_providerCode, dynamic _param_username, dynamic _param_loginControls = null, dynamic _param_ignoreActivation = null)
		{
			#region default values
			if(_param_loginControls as Object == null) _param_loginControls = new XVar(XVar.Array());
			if(_param_ignoreActivation as Object == null) _param_ignoreActivation = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic providerCode = XVar.Clone(_param_providerCode);
			dynamic username = XVar.Clone(_param_username);
			dynamic loginControls = XVar.Clone(_param_loginControls);
			dynamic ignoreActivation = XVar.Clone(_param_ignoreActivation);
			#endregion

			dynamic conditions = XVar.Array(), dc = null, fieldName = null, provider = XVar.Array();
			provider = XVar.Clone(Security.findProvider((XVar)(providerCode)));
			fieldName = XVar.Clone((XVar.Pack(provider["type"] == Constants.stDB) ? XVar.Pack(Security.usernameField()) : XVar.Pack(Security.extIdField())));
			if(provider["type"] == Constants.stAD)
			{
				username = XVar.Clone(MVCFunctions.Concat(provider["code"], username));
			}
			conditions = XVar.Clone(new XVar(0, DataCondition.FieldEquals((XVar)(fieldName), (XVar)(username), new XVar(0), (XVar)((XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_STRICT))))));
			if((XVar)(!(XVar)(ignoreActivation))  && (XVar)(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				conditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("activationField"))), new XVar(1)), null);
			}
			foreach (KeyValuePair<XVar, dynamic> value in loginControls.GetEnumerator())
			{
				conditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(value.Key), (XVar)(value.Value)), null);
			}
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(conditions)));
			return dc;
		}
		public static XVar fetchUserData(dynamic _param_username, dynamic _param_password, dynamic _param_skipPasswordCheck = null, dynamic _param_loginControls = null)
		{
			#region default values
			if(_param_skipPasswordCheck as Object == null) _param_skipPasswordCheck = new XVar(false);
			if(_param_loginControls as Object == null) _param_loginControls = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			dynamic skipPasswordCheck = XVar.Clone(_param_skipPasswordCheck);
			dynamic loginControls = XVar.Clone(_param_loginControls);
			#endregion

			dynamic data = XVar.Array(), dataSource = null, dc = null, dpProvider = XVar.Array(), providerCode = null, qResult = null;
			dpProvider = XVar.Clone(Security.dbProvider());
			if(XVar.Pack(!(XVar)(dpProvider)))
			{
				return null;
			}
			dataSource = XVar.Clone(CommonFunctions.getLoginDataSource());
			providerCode = XVar.Clone(CommonFunctions.storageGet(new XVar("providerCode")));
			if(XVar.Pack(!(XVar)(providerCode)))
			{
				providerCode = XVar.Clone(dpProvider["code"]);
			}
			dc = XVar.Clone(Security.createFetchUserCommand((XVar)(providerCode), (XVar)(username), (XVar)(loginControls), (XVar)(skipPasswordCheck)));
			qResult = XVar.Clone(dataSource.getSingle((XVar)(dc)));
			data = XVar.Clone(dataSource.decryptRecord((XVar)(qResult.fetchAssoc())));
			if((XVar)(!(XVar)(skipPasswordCheck))  && (XVar)(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				if(XVar.Pack(data))
				{
					data.InitAndSetArrayItem(1, ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("activationField")));
				}
				else
				{
					dc = XVar.Clone(Security.createFetchUserCommand((XVar)(providerCode), (XVar)(username), (XVar)(loginControls), new XVar(true)));
					qResult = XVar.Clone(dataSource.getSingle((XVar)(dc)));
					data = XVar.Clone(dataSource.decryptRecord((XVar)(qResult.fetchAssoc())));
				}
			}
			if(XVar.Pack(!(XVar)(data)))
			{
				return null;
			}
			if((XVar)(!(XVar)(skipPasswordCheck))  && (XVar)(!(XVar)(Security.verifyPassword((XVar)(password), (XVar)(data[Security.passwordField()])))))
			{
				return null;
			}
			return data;
		}
		public static XVar verifyPassword(dynamic _param_password, dynamic _param_storedPassword)
		{
			#region pass-by-value parameters
			dynamic password = XVar.Clone(_param_password);
			dynamic storedPassword = XVar.Clone(_param_storedPassword);
			#endregion

			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("hashPassword"))))
			{
				if(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("hashAlgorithm")) == 0)
				{
					return MVCFunctions.passwordVerify((XVar)(password), (XVar)(storedPassword));
				}
				else
				{
					return XVar.Equals(XVar.Pack(XVar.Pack(Security.hashPassword((XVar)(password))).ToString()), XVar.Pack(XVar.Pack(storedPassword).ToString()));
				}
			}
			return XVar.Equals(XVar.Pack(XVar.Pack(password).ToString()), XVar.Pack(XVar.Pack(storedPassword).ToString()));
		}
		public static XVar hashPassword(dynamic _param_password)
		{
			#region pass-by-value parameters
			dynamic password = XVar.Clone(_param_password);
			#endregion

			return (XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("hashAlgorithm")) == 0) ? XVar.Pack(MVCFunctions.getPasswordHash((XVar)(password))) : XVar.Pack(MVCFunctions.md5((XVar)(password))));
		}
		public static XVar sendTwoFactorCode(dynamic _param_method, dynamic _param_address, dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic method = XVar.Clone(_param_method);
			dynamic address = XVar.Clone(_param_address);
			dynamic code = XVar.Clone(_param_code);
			#endregion

			if(method == Constants.TWOFACTOR_EMAIL)
			{
				dynamic html = null;
				html = XVar.Clone(CommonFunctions.isEmailTemplateUseHTML(new XVar("twofactoremail")));
				return RunnerPage.sendEmailByTemplate((XVar)(address), new XVar("twofactoremail"), (XVar)(new XVar("code", code)), (XVar)(html));
			}
			else
			{
				if(method == Constants.TWOFACTOR_PHONE)
				{
					dynamic message = null, ret = XVar.Array();
					message = XVar.Clone(Security.prepareTwoFactorMessage((XVar)(code)));
					if(XVar.Pack(GlobalVars.debug2Factor))
					{
						return new XVar("success", true);
					}
					ret = XVar.Clone(MVCFunctions.runner_sms((XVar)(address), (XVar)(message)));
					ret.InitAndSetArrayItem(ret["error"], "message");
					return ret;
				}
			}
			return new XVar("success", false, "message", "Unknown two factor authentication method");
		}
		public static XVar userSessionLevel()
		{
			if(Security.getUserName() != "")
			{
				return Constants.LOGGED_FULL;
			}
			if(Security.provisionalUsername() == "")
			{
				return Constants.LOGGED_NONE;
			}
			return CommonFunctions.storageGet(new XVar("SessionLevel"));
		}
		public static XVar provisionalUsername()
		{
			if(Security.getUserName() != "")
			{
				return "";
			}
			return CommonFunctions.storageGet(new XVar("ProvisionalUserID"));
		}
		public static XVar provisionalUserData()
		{
			return CommonFunctions.storageGet(new XVar("UserData"));
		}
		public static XVar twoFactorDeliveryInfo(dynamic userData, dynamic _param_method)
		{
			#region pass-by-value parameters
			dynamic method = XVar.Clone(_param_method);
			#endregion

			dynamic ret = XVar.Array(), twoFactorValue = null, twofSettings = XVar.Array();
			twofSettings = Security.twoFactorSettings();
			twoFactorValue = XVar.Clone(userData[twofSettings["twoFactorField"]]);
			if(XVar.Pack(!(XVar)(Security.twoFactorEnabled((XVar)(twoFactorValue)))))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(Security.twoFactorMethodEnabled((XVar)(twoFactorValue), (XVar)(method)))))
			{
				return null;
			}
			ret = XVar.Clone(new XVar("method", method));
			if(method == Constants.TWOFACTOR_EMAIL)
			{
				ret.InitAndSetArrayItem(userData[twofSettings["emailField"]], "address");
			}
			else
			{
				if(method == Constants.TWOFACTOR_PHONE)
				{
					ret.InitAndSetArrayItem(userData[twofSettings["phoneField"]], "address");
				}
				else
				{
					if(method == Constants.TWOFACTOR_APP)
					{
						ret.InitAndSetArrayItem(twofSettings["projectName"], "address");
					}
				}
			}
			return ret;
		}
		public static XVar generateAndSendTwoFactorCode(dynamic _param_method)
		{
			#region pass-by-value parameters
			dynamic method = XVar.Clone(_param_method);
			#endregion

			dynamic code = null, destination = XVar.Array(), ret = XVar.Array(), twofSettings = null, userData = null;
			if(!XVar.Equals(XVar.Pack(Security.userSessionLevel()), XVar.Pack(Constants.LOGGED_2F_PENDING)))
			{
				return null;
			}
			userData = Security.provisionalUserData();
			twofSettings = Security.twoFactorSettings();
			destination = XVar.Clone(Security.twoFactorDeliveryInfo((XVar)(userData), (XVar)(method)));
			if(XVar.Pack(!(XVar)(destination)))
			{
				return null;
			}
			if(destination["method"] == Constants.TWOFACTOR_APP)
			{
				destination.InitAndSetArrayItem(true, "success");
				CommonFunctions.storageSet(new XVar("twoFactorCode"), (XVar)(new XVar("method", method)));
				return destination;
			}
			code = XVar.Clone(CommonFunctions.generateUserCode((XVar)(ProjectSettings.getProjectValue(new XVar("smsCodeLength")))));
			if(XVar.Pack(GlobalVars.debug2Factor))
			{
				code = new XVar("333");
			}
			CommonFunctions.storageSet(new XVar("twoFactorCode"), (XVar)(new XVar("method", method, "code", code)));
			ret = XVar.Clone(Security.sendTwoFactorCode((XVar)(destination["method"]), (XVar)(destination["address"]), (XVar)(code)));
			ret.InitAndSetArrayItem(destination["address"], "address");
			ret.InitAndSetArrayItem(destination["method"], "method");
			return ret;
		}
		public static XVar checkTwoFactorCode(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			dynamic method = null, result = null, savedCodeData = XVar.Array(), twoFactorValue = null, twofSettings = XVar.Array(), userData = XVar.Array();
			if(!XVar.Equals(XVar.Pack(Security.userSessionLevel()), XVar.Pack(Constants.LOGGED_2F_PENDING)))
			{
				return false;
			}
			userData = Security.provisionalUserData();
			twofSettings = Security.twoFactorSettings();
			twoFactorValue = XVar.Clone(userData[twofSettings["twoFactorField"]]);
			savedCodeData = XVar.Clone(CommonFunctions.storageGet(new XVar("twoFactorCode")));
			method = XVar.Clone(savedCodeData["method"]);
			if(XVar.Pack(!(XVar)(Security.twoFactorMethodEnabled((XVar)(twoFactorValue), (XVar)(method)))))
			{
				return false;
			}
			result = new XVar(false);
			if((XVar)(method == Constants.TWOFACTOR_EMAIL)  || (XVar)(method == Constants.TWOFACTOR_PHONE))
			{
				result = XVar.Clone((XVar)(code != XVar.Pack(""))  && (XVar)(savedCodeData["code"] == code));
			}
			else
			{
				if(method == Constants.TWOFACTOR_APP)
				{
					result = XVar.Clone(code == MVCFunctions.calculateTotpCode((XVar)(userData[twofSettings["codeField"]])));
				}
			}
			if(XVar.Pack(result))
			{
				userData.InitAndSetArrayItem(Security.getTwoFactorValue((XVar)(Security.twoFactorEnabledMethods((XVar)(twoFactorValue))), (XVar)(method)), twofSettings["twoFactorField"]);
				CommonFunctions.storageSet(new XVar("UserData"), (XVar)(userData));
			}
			return result;
		}
		public static XVar verifyTwoFactorEnabled(dynamic _param_userData)
		{
			#region pass-by-value parameters
			dynamic userData = XVar.Clone(_param_userData);
			#endregion

			dynamic methods = null, twoFactorValue = null, twofSettings = XVar.Array();
			twofSettings = Security.twoFactorSettings();
			if(XVar.Pack(!(XVar)(twofSettings)))
			{
				return false;
			}
			twoFactorValue = XVar.Clone(userData[twofSettings["twoFactorField"]]);
			methods = XVar.Clone(Security.twoFactorEnabledMethods((XVar)(twoFactorValue)));
			return 0 < MVCFunctions.count(methods);
		}
		public static XVar machineTwoFactorTrusted(dynamic _param_userData)
		{
			#region pass-by-value parameters
			dynamic userData = XVar.Clone(_param_userData);
			#endregion

			return false;
		}
		public static XVar verifyHardcodedLogin(dynamic _param_username, dynamic _param_password)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			#endregion

			dynamic cPassword = null, cUserName = null;
			if(XVar.Pack(!(XVar)(Security.hardcodedLogin())))
			{
				return false;
			}
			cUserName = XVar.Clone(Security.hardcodedUsername());
			cPassword = XVar.Clone(Security.hardcodedPassword());
			return (XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack((XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strtoupper((XVar)(username))), XVar.Pack(MVCFunctions.strtoupper((XVar)(cUserName)))))  && (XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strtoupper((XVar)(password))), XVar.Pack(MVCFunctions.strtoupper((XVar)(cPassword)))))) : XVar.Pack((XVar)(username == cUserName)  && (XVar)(password == cPassword)));
		}
		public static XVar setKeepLoggedCookie(dynamic _param_success)
		{
			#region pass-by-value parameters
			dynamic success = XVar.Clone(_param_success);
			#endregion

			dynamic prov = XVar.Array();
			prov = XVar.Clone(Security.currentProvider());
			if((XVar)((XVar)(success)  && (XVar)(ProjectSettings.getProjectValue(new XVar("keepLoggedIn"))))  && (XVar)(prov))
			{
				dynamic payload = XVar.Array(), secondsIn30 = null;
				payload = XVar.Clone(new XVar("username", Security.getUserName(), "host", CommonFunctions.projectHost(), "origin", CommonFunctions.projectURL()));
				payload.InitAndSetArrayItem(prov["code"], "provider");
				if(XVar.Pack(Security.hardcodedLogin()))
				{
					payload.InitAndSetArrayItem(MVCFunctions.getPasswordHash((XVar)(ProjectSettings.getSecurityValue(new XVar("hardcodedProvider"), new XVar("password")))), "checksum");
				}
				else
				{
					if(prov["type"] == Constants.stDB)
					{
						dynamic data = XVar.Array();
						data = Security.currentUserData();
						payload.InitAndSetArrayItem(MVCFunctions.getPasswordHash((XVar)(data[Security.passwordField()])), "checksum");
					}
				}
				secondsIn30 = XVar.Clone((30 * 1440) * 60);
				CommonFunctions.setProjectCookie(new XVar("token"), (XVar)(CommonFunctions.jwt_encode((XVar)(payload), (XVar)(secondsIn30))), (XVar)(MVCFunctions.time() + secondsIn30), new XVar(true));
			}
			else
			{
				CommonFunctions.setProjectCookie(new XVar("token"), new XVar(""), (XVar)(MVCFunctions.time() - 1), new XVar(true));
			}

			return null;
		}
		public static XVar verifyKeepLoggedToken(dynamic _param_jwt)
		{
			#region pass-by-value parameters
			dynamic jwt = XVar.Clone(_param_jwt);
			#endregion

			dynamic payload = XVar.Array(), provider = XVar.Array(), userData = XVar.Array();
			payload = XVar.Clone(CommonFunctions.jwt_verify_decode((XVar)(jwt)));
			if(XVar.Pack(!(XVar)(payload)))
			{
				return null;
			}
			if(payload["username"] == "")
			{
				return null;
			}
			if(XVar.Pack(payload["external"]))
			{
				return payload;
			}
			if(XVar.Pack(!(XVar)(ProjectSettings.getProjectValue(new XVar("keepLoggedIn")))))
			{
				return false;
			}
			userData = XVar.Clone(XVar.Array());
			provider = XVar.Clone(Security.findProvider((XVar)(payload["provider"])));
			if(XVar.Pack(!(XVar)(provider)))
			{
				return null;
			}
			if(XVar.Equals(XVar.Pack(provider["type"]), XVar.Pack(Constants.stDB)))
			{
				dynamic twofSettings = XVar.Array();
				userData = XVar.Clone(Security.fetchUserData((XVar)(payload["username"]), new XVar(""), new XVar(true)));
				if(XVar.Pack(!(XVar)(userData)))
				{
					return null;
				}
				if(XVar.Pack(!(XVar)(MVCFunctions.passwordVerify((XVar)(userData[Security.passwordField()]), (XVar)(payload["checksum"])))))
				{
					return null;
				}
				twofSettings = Security.twoFactorSettings();
				if(XVar.Pack(twofSettings["available"]))
				{
					return null;
				}
			}
			else
			{
				if(XVar.Equals(XVar.Pack(provider["type"]), XVar.Pack(Constants.stHARDCODED)))
				{
					if(XVar.Pack(!(XVar)(MVCFunctions.passwordVerify((XVar)(ProjectSettings.getSecurityValue(new XVar("hardcodedProvider"), new XVar("password"))), (XVar)(payload["checksum"])))))
					{
						return null;
					}
				}
			}
			return payload;
		}
		public static XVar autoLoginAsGuest()
		{
			dynamic scriptname = null;
			scriptname = XVar.Clone(CommonFunctions.getFileWoExtension((XVar)(MVCFunctions.getFileNameFromURL())));
			if((XVar)((XVar)((XVar)((XVar)((XVar)(!(XVar)(CommonFunctions.isLogged()))  && (XVar)(Security.provisionalUsername() == ""))  && (XVar)(scriptname != "login"))  && (XVar)(scriptname != "remind"))  && (XVar)(scriptname != "register"))  && (XVar)(scriptname != "checkduplicates"))
			{
				Security.doGuestLogin();
				return true;
			}
			return false;
		}
		public static XVar fieldIsUserpic(dynamic _param_table, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic provider = XVar.Array();
			provider = XVar.Clone(Security.dbProvider());
			if(XVar.Pack(!(XVar)(provider)))
			{
				return false;
			}
			return (XVar)(table == Security.loginTable())  && (XVar)(field == provider["userpicField"]);
		}
		public static XVar showUserPic()
		{
			dynamic prov = XVar.Array(), sessionLevel = null, upicField = null, userData = XVar.Array();
			if(XVar.Pack(Security.isGuest()))
			{
				return false;
			}
			sessionLevel = XVar.Clone(Security.userSessionLevel());
			if((XVar)(sessionLevel != Constants.LOGGED_FULL)  && (XVar)(sessionLevel != Constants.LOGGED_2FSETUP_PENDING))
			{
				return false;
			}
			userData = Security.currentUserData();
			upicField = XVar.Clone(Security.userpicField());
			prov = XVar.Clone(Security.currentProvider());
			if((XVar)((XVar)((XVar)(!(XVar)(upicField))  && (XVar)(prov))  && (XVar)(prov["type"] != Constants.stDB))  && (XVar)(!(XVar)(Security.extIdField())))
			{
				upicField = new XVar("picture");
			}
			if(XVar.Pack(!(XVar)(upicField)))
			{
				return false;
			}
			if((XVar)(!(XVar)(userData.KeyExists(upicField)))  || (XVar)(MVCFunctions.strlen_bin((XVar)(userData[upicField])) == 0))
			{
				return false;
			}
			return true;
		}
		public static XVar refreshUserdata()
		{
			dynamic userData = null;
			if(XVar.Pack(!(XVar)(Security.providerUsersInDb((XVar)(Security.currentProvider())))))
			{
				return null;
			}
			if((XVar)(!(XVar)(CommonFunctions.isLogged()))  || (XVar)(Security.isGuest()))
			{
				return null;
			}
			userData = XVar.Clone(Security.fetchUserData((XVar)(Security.getUserName()), new XVar(""), new XVar(true)));
			CommonFunctions.storageSet(new XVar("UserData"), (XVar)(userData));

			return null;
		}
		public static XVar verifySafeCSRF()
		{
			if((XVar)((XVar)(!(XVar)(CommonFunctions.inRestApi()))  && (XVar)(!(XVar)(GlobalVars.csrfProtectionOff)))  && (XVar)(CommonFunctions.isPostRequest()))
			{
				if((XVar)(!(XVar)(XSession.Session["runnerSession"]))  || (XVar)(!XVar.Equals(XVar.Pack(XSession.Session["runnerSession"]), XVar.Pack(MVCFunctions.GetCookie("runnerSession")))))
				{
					return false;
				}
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strtoupper((XVar)(CommonFunctions.hostFromUrl((XVar)(MVCFunctions.GetServerVariable("HTTP_REFERER")))))), XVar.Pack(MVCFunctions.strtoupper((XVar)(CommonFunctions.projectHost())))))
				{
					return false;
				}
			}
			return true;
		}
		public static XVar callAfterLogin()
		{
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterSuccessfulLogin"))))
			{
				GlobalVars.globalEvents.AfterSuccessfulLogin((XVar)(Security.getUserName()), new XVar(""), (XVar)(Security.currentUserData()), new XVar(null));
			}

			return null;
		}
		public static XVar updateCSRFCookie()
		{
			if(!XVar.Equals(XVar.Pack(CommonFunctions.requestMethod()), XVar.Pack("GET")))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(Security.isLoggedIn())))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(CommonFunctions.storageGet(new XVar("runnerSession")))))
			{
				CommonFunctions.storageSet(new XVar("runnerSession"), (XVar)(CommonFunctions.generatePassword(new XVar(20))));
			}
			if(!XVar.Equals(XVar.Pack(CommonFunctions.storageGet(new XVar("runnerSession"))), XVar.Pack(MVCFunctions.GetCookie("runnerSession"))))
			{
				CommonFunctions.setProjectCookie(new XVar("runnerSession"), (XVar)(CommonFunctions.storageGet(new XVar("runnerSession"))), new XVar(0), new XVar(true), new XVar(true));
				MVCFunctions.setCookieDirectly(new XVar("runnerSession"), (XVar)(CommonFunctions.storageGet(new XVar("runnerSession"))));
			}

			return null;
		}
		public static XVar getActivationCode(dynamic _param_username, dynamic _param_dbPassword)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic dbPassword = XVar.Clone(_param_dbPassword);
			#endregion

			dynamic passwordHash = null;
			passwordHash = XVar.Clone(MVCFunctions.md5((XVar)(dbPassword)));
			return MVCFunctions.getPasswordHash((XVar)(MVCFunctions.Concat(username, passwordHash)));
		}
		public static XVar verifyActivationCode(dynamic _param_code, dynamic _param_username, dynamic _param_dbPassword)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			dynamic username = XVar.Clone(_param_username);
			dynamic dbPassword = XVar.Clone(_param_dbPassword);
			#endregion

			dynamic passwordHash = null, usercode = null;
			passwordHash = XVar.Clone(MVCFunctions.md5((XVar)(dbPassword)));
			usercode = XVar.Clone(MVCFunctions.Concat(username, passwordHash));
			return MVCFunctions.passwordVerify((XVar)(usercode), (XVar)(code));
		}
		public static XVar dbProvider()
		{
			dynamic ret = null;
			ret = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("dbProvider")));
			if(XVar.Pack(!(XVar)(ret)))
			{
				return XVar.Array();
			}
			return ret;
		}
		public static XVar hardcodedProvider()
		{
			dynamic ret = null;
			ret = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("hardcodedProvider")));
			if(XVar.Pack(!(XVar)(ret)))
			{
				return XVar.Array();
			}
			return ret;
		}
		public static XVar findProvider(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			dynamic providers = XVar.Array();
			providers = ProjectSettings.getSecurityValue(new XVar("providers"));
			foreach (KeyValuePair<XVar, dynamic> p in providers.GetEnumerator())
			{
				if((XVar)(p.Value["code"] == code)  && (XVar)(p.Value["active"]))
				{
					return p.Value;
				}
			}
			return XVar.Array();
		}
		public static XVar providersByType(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			dynamic providers = XVar.Array(), ret = XVar.Array();
			providers = ProjectSettings.getSecurityValue(new XVar("providers"));
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> p in providers.GetEnumerator())
			{
				if((XVar)(p.Value["type"] == var_type)  && (XVar)(p.Value["active"]))
				{
					ret.InitAndSetArrayItem(p.Value, null);
				}
			}
			return ret;
		}
		public static XVar loginTable()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if((XVar)(db)  && (XVar)(db["table"]))
			{
				return db["table"]["table"];
			}
			return "";
		}
		public static XVar userpicField()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			return db["userpicField"];
		}
		public static XVar usernameField()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if(XVar.Pack(db))
			{
				return db["usernameField"];
			}
			return "";
		}
		public static XVar passwordField()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if(XVar.Pack(db))
			{
				return db["passwordField"];
			}
			return "";
		}
		public static XVar extIdField()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if(XVar.Pack(db))
			{
				return db["extUserIdField"];
			}
			return "";
		}
		public static XVar emailField()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if(XVar.Pack(db))
			{
				return db["emailField"];
			}
			return "";
		}
		public static XVar fullnameField()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if(XVar.Pack(db))
			{
				return db["fullnameField"];
			}
			return "";
		}
		public static XVar resetTokenField()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if(XVar.Pack(db))
			{
				return db["resetTokenField"];
			}
			return "";
		}
		public static XVar groupIdField()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if(XVar.Pack(db))
			{
				return db["userGroupField"];
			}
			return "";
		}
		public static XVar resetDateField()
		{
			dynamic db = XVar.Array();
			db = Security.dbProvider();
			if(XVar.Pack(db))
			{
				return db["resetDateField"];
			}
			return "";
		}
		public static XVar fetchPluginUser(dynamic _param_externalId)
		{
			#region pass-by-value parameters
			dynamic externalId = XVar.Clone(_param_externalId);
			#endregion

			dynamic commands = XVar.Array(), dataSource = null, dc = null;
			commands = XVar.Clone(XVar.Array());
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition.FieldEquals((XVar)(Security.extIdField()), (XVar)(externalId)));
			commands.InitAndSetArrayItem(dc, null);
			dataSource = XVar.Clone(CommonFunctions.getLoginDataSource());
			foreach (KeyValuePair<XVar, dynamic> _dc in commands.GetEnumerator())
			{
				dynamic qResult = null;
				qResult = XVar.Clone(dataSource.getSingle((XVar)(_dc.Value)));
				if(XVar.Pack(qResult))
				{
					dynamic data = null;
					data = XVar.Clone(dataSource.decryptRecord((XVar)(qResult.fetchAssoc())));
					if(XVar.Pack(data))
					{
						return new XVar("data", data, "dc", _dc.Value);
					}
				}
			}
			return null;
		}
		public static XVar updatePluginUserData(dynamic _param_info, dynamic _param_data, dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic info = XVar.Clone(_param_info);
			dynamic data = XVar.Clone(_param_data);
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic emailField = null, errMessage = null, externalIdField = null, fullnameField = null, newData = XVar.Array(), userpicField = null;
			newData = XVar.Clone(XVar.Array());
			externalIdField = XVar.Clone(Security.extIdField());
			if((XVar)(externalIdField)  && (XVar)(data[externalIdField] != info["id"]))
			{
				newData.InitAndSetArrayItem(info["id"], externalIdField);
				data.InitAndSetArrayItem(info["id"], externalIdField);
			}
			fullnameField = XVar.Clone(Security.fullnameField());
			if((XVar)((XVar)(fullnameField)  && (XVar)(info["name"]))  && (XVar)(info["name"] != data[fullnameField]))
			{
				newData.InitAndSetArrayItem(info["name"], fullnameField);
				data.InitAndSetArrayItem(info["name"], fullnameField);
			}
			emailField = XVar.Clone(Security.emailField());
			if((XVar)((XVar)(emailField)  && (XVar)(info["email"]))  && (XVar)(info["email"] != data[emailField]))
			{
				newData.InitAndSetArrayItem(info["email"], emailField);
				data.InitAndSetArrayItem(info["email"], emailField);
			}
			userpicField = XVar.Clone(Security.userpicField());
			if((XVar)((XVar)(userpicField)  && (XVar)(info["picture"]))  && (XVar)(info["picture"] != data[userpicField]))
			{
				newData.InitAndSetArrayItem(info["picture"], userpicField);
				data.InitAndSetArrayItem(info["picture"], userpicField);
			}
			errMessage = new XVar("");
			if(XVar.Pack(MVCFunctions.count(newData)))
			{
				dynamic dataSource = null, ret = null;
				dc.values = XVar.Clone(newData);
				dataSource = XVar.Clone(CommonFunctions.getLoginDataSource());
				ret = XVar.Clone(dataSource.updateSingle((XVar)(dc), new XVar(false)));
				if(XVar.Pack(!(XVar)(ret)))
				{
					errMessage = XVar.Clone(dataSource.lastError());
				}
			}
			return new XVar("data", data, "errMessage", errMessage);
		}
		public static XVar addNewPluginUser(dynamic _param_info)
		{
			#region pass-by-value parameters
			dynamic info = XVar.Clone(_param_info);
			#endregion

			dynamic cipherer = null, data = null, dataSource = null, dc = null, err = null, pPassword = null, userpicField = null;
			dataSource = XVar.Clone(CommonFunctions.getLoginDataSource());
			data = XVar.Clone(XVar.Array());
			pPassword = XVar.Clone(CommonFunctions.generatePassword(new XVar(20)));
			cipherer = XVar.Clone(new RunnerCipherer((XVar)(Security.loginTable())));
			if((XVar)(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("hashPassword")))  && (XVar)(!(XVar)(cipherer.isFieldEncrypted((XVar)(Security.passwordField())))))
			{
				pPassword = XVar.Clone(MVCFunctions.getPasswordHash((XVar)(pPassword)));
			}
			dc = XVar.Clone(new DsCommand());
			if(XVar.Pack(Security.passwordField()))
			{
				dc.values.InitAndSetArrayItem(pPassword, Security.passwordField());
			}
			dc.values.InitAndSetArrayItem(info["id"], Security.extIdField());
			if(XVar.Pack(Security.fullnameField()))
			{
				dc.values.InitAndSetArrayItem(info["name"], Security.fullnameField());
			}
			if(XVar.Pack(Security.emailField()))
			{
				dc.values.InitAndSetArrayItem(info["email"], Security.emailField());
			}
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("sendActivationLink"))))
			{
				dc.values.InitAndSetArrayItem(1, ProjectSettings.getSecurityValue(new XVar("dbProvider"), new XVar("activationField")));
			}
			userpicField = XVar.Clone(Security.userpicField());
			if((XVar)(userpicField)  && (XVar)(info["picture"] != ""))
			{
				dc.values.InitAndSetArrayItem(info["picture"], userpicField);
			}
			data = XVar.Clone(dataSource.insertSingle((XVar)(dc)));
			err = XVar.Clone((XVar.Pack(data) ? XVar.Pack("") : XVar.Pack(dataSource.lastError())));
			return new XVar("data", data, "errorMessage", err);
		}
		public static XVar advancedSecurityAvailable()
		{
			return ProjectSettings.getSecurityValue(new XVar("advancedSecurityAvailable"));
		}
		public static XVar userGroupsAvailable()
		{
			return ProjectSettings.getSecurityValue(new XVar("userGroupsAvailable"));
		}
		public static XVar hardcodedLogin()
		{
			return ProjectSettings.getSecurityValue(new XVar("hardcodedLogin"));
		}
		public static XVar defaultProvider()
		{
			return Security.findProvider((XVar)(ProjectSettings.getSecurityValue(new XVar("defaultProviderCode"))));
		}
		public static XVar ADonlyLogin()
		{
			return ProjectSettings.getSecurityValue(new XVar("adOnlyLogin"));
		}
		public static XVar currentProvider()
		{
			return Security.findProvider((XVar)(CommonFunctions.storageGet(new XVar("providerCode"))));
		}
		public static XVar currentProviderType()
		{
			dynamic prov = XVar.Array();
			prov = XVar.Clone(Security.currentProvider());
			if(XVar.Pack(!(XVar)(prov)))
			{
				return "";
			}
			return prov["type"];
		}
		public static XVar currentProviderCode()
		{
			dynamic prov = XVar.Array();
			prov = XVar.Clone(Security.currentProvider());
			if(XVar.Pack(!(XVar)(prov)))
			{
				return "";
			}
			if(prov["type"] == Constants.stDB)
			{
				return "";
			}
			return prov["code"];
		}
		public static XVar providerUsersInDb(dynamic _param_prov)
		{
			#region pass-by-value parameters
			dynamic prov = XVar.Clone(_param_prov);
			#endregion

			dynamic dbProvider = null;
			if(XVar.Pack(!(XVar)(prov)))
			{
				return false;
			}
			dbProvider = XVar.Clone(Security.dbProvider());
			if((XVar)(!(XVar)(prov))  || (XVar)(!(XVar)(dbProvider)))
			{
				return false;
			}
			if(XVar.Pack(Security.hardcodedLogin()))
			{
				return false;
			}
			if((XVar)(prov["type"] == Constants.stAD)  && (XVar)(!(XVar)(prov["useDbGroups"])))
			{
				return false;
			}
			return true;
		}
		public static XVar currentUserRecord(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			return XVar.Equals(XVar.Pack(Security.getUserId()), XVar.Pack(data[Security.currentUserIdField()]));
		}
		public static XVar currentUserCondition()
		{
			return DataCondition.FieldEquals((XVar)(Security.currentUserIdField()), (XVar)(Security.getUserId()));
		}
		public static XVar currentUserIdField()
		{
			dynamic provider = XVar.Array(), providerCode = null;
			providerCode = XVar.Clone(CommonFunctions.storageGet(new XVar("providerCode")));
			provider = XVar.Clone(Security.findProvider((XVar)(providerCode)));
			if((XVar)(provider["type"] == Constants.stDB)  || (XVar)(!(XVar)(providerCode)))
			{
				return Security.usernameField();
			}
			return Security.extIdField();
		}
		public static XVar hardcodedUsername()
		{
			dynamic prov = XVar.Array();
			prov = Security.hardcodedProvider();
			return prov["username"];
		}
		public static XVar hardcodedPassword()
		{
			dynamic prov = XVar.Array();
			prov = Security.hardcodedProvider();
			return prov["password"];
		}
		public static XVar fetchADUserData()
		{

			return null;
		}
		public static XVar tryLoginAutoAd(dynamic _param_fireEvents)
		{
			#region pass-by-value parameters
			dynamic fireEvents = XVar.Clone(_param_fireEvents);
			#endregion

			dynamic adProvider = XVar.Array(), plugin = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.GetRemoteUser())))
			{
				return false;
			}
			adProvider = XVar.Clone(Security.findAutoAdProvider());
			if(XVar.Pack(!(XVar)(adProvider)))
			{
				return false;
			}
			plugin = XVar.Clone(Security.getAuthPlugin((XVar)(adProvider["code"])));
			if(XVar.Pack(!(XVar)(plugin.loginAuto((XVar)(MVCFunctions.GetRemoteUser())))))
			{
				return false;
			}
			if(XVar.Pack(fireEvents))
			{
				Security.auditLoginSuccess();
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("AfterSuccessfulLogin"))))
				{
					GlobalVars.globalEvents.AfterSuccessfulLogin((XVar)(MVCFunctions.GetRemoteUser()), new XVar(""), (XVar)(Security.currentUserData()), new XVar(null));
				}
			}
			return true;
		}
		public static XVar findAutoAdProvider()
		{
			dynamic adProviders = XVar.Array();
			adProviders = XVar.Clone(Security.providersByType(new XVar(Constants.stAD)));
			foreach (KeyValuePair<XVar, dynamic> ad in adProviders.GetEnumerator())
			{
				if(XVar.Pack(ad.Value["loginAutomatically"]))
				{
					return ad.Value;
				}
			}
			return null;
		}
		public static XVar fetchUpdateDatabaseUser(dynamic _param_userInfo, dynamic _param_addNewUser)
		{
			#region pass-by-value parameters
			dynamic userInfo = XVar.Clone(_param_userInfo);
			dynamic addNewUser = XVar.Clone(_param_addNewUser);
			#endregion

			dynamic data = null, dc = null, errorMessage = null, pluginUserRecord = XVar.Array(), userData = XVar.Array();
			data = new XVar(null);
			pluginUserRecord = XVar.Clone(Security.fetchPluginUser((XVar)(userInfo["id"])));
			if(XVar.Pack(pluginUserRecord))
			{
				data = XVar.Clone(pluginUserRecord["data"]);
				dc = XVar.Clone(pluginUserRecord["dc"]);
			}
			errorMessage = new XVar("");
			if(XVar.Pack(data))
			{
				userData = XVar.Clone(Security.updatePluginUserData((XVar)(userInfo), (XVar)(data), (XVar)(dc)));
				data = userData["data"];
				errorMessage = XVar.Clone(userData["errorMessage"]);
			}
			else
			{
				if(XVar.Pack(addNewUser))
				{
					userData = XVar.Clone(Security.addNewPluginUser((XVar)(userInfo)));
					data = userData["data"];
					errorMessage = XVar.Clone(userData["errorMessage"]);
				}
				else
				{
					return false;
				}
			}
			return new XVar("data", data, "errorMessage", errorMessage);
		}
		public static XVar currentUserInDatabase()
		{
			dynamic prov = XVar.Array();
			prov = XVar.Clone(Security.currentProvider());
			if(XVar.Pack(!(XVar)(prov)))
			{
				return false;
			}
			if(XVar.Equals(XVar.Pack(prov["type"]), XVar.Pack(Constants.stHARDCODED)))
			{
				return false;
			}
			if(XVar.Equals(XVar.Pack(prov["type"]), XVar.Pack(Constants.stDB)))
			{
				return true;
			}
			if(XVar.Pack(!(XVar)(Security.extIdField())))
			{
				return false;
			}
			if(XVar.Equals(XVar.Pack(prov["type"]), XVar.Pack(Constants.stAD)))
			{
				return !(XVar)(!(XVar)(prov["useDbGroups"]));
			}
			return true;
		}
		public static XVar logoutAvailable()
		{
			dynamic prov = XVar.Array();
			prov = XVar.Clone(Security.currentProvider());
			if(XVar.Pack(!(XVar)(prov)))
			{
				return false;
			}
			if(XVar.Equals(XVar.Pack(prov["type"]), XVar.Pack(Constants.stAD)))
			{
				return !(XVar)(CommonFunctions.storageGet(new XVar("AutomaticLogin")));
			}
			return true;
		}
		public static XVar providersInDb()
		{
			return ProjectSettings.getSecurityValue(new XVar("dbProviderCodes"));
		}
		public static XVar caseInsensitiveUsername()
		{
			dynamic registration = XVar.Array();
			registration = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("registration")));
			return !(XVar)(!(XVar)(registration["caseInsensitiveLogin"]));
		}
		public static XVar registerPage()
		{
			dynamic registration = XVar.Array();
			registration = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("registration")));
			return !(XVar)(!(XVar)(registration["registerPage"]));
		}
		public static XVar loginDataSource()
		{
			return ProjectSettings.getSecurityValue(new XVar("loginDataSource"));
		}
		public static XVar twoFactorPendingLevel(dynamic _param_dbData)
		{
			#region pass-by-value parameters
			dynamic dbData = XVar.Clone(_param_dbData);
			#endregion

			if(XVar.Pack(Security.verifyTwoFactorEnabled((XVar)(dbData))))
			{
				if(XVar.Pack(!(XVar)(Security.machineTwoFactorTrusted((XVar)(dbData)))))
				{
					return Constants.LOGGED_2F_PENDING;
				}
			}
			else
			{
				dynamic twofSettings = XVar.Array();
				twofSettings = Security.twoFactorSettings();
				if(XVar.Pack(twofSettings["required"]))
				{
					return Constants.LOGGED_2FSETUP_PENDING;
				}
			}
			return Constants.LOGGED_FULL;
		}
		public static XVar providerInDb(dynamic _param_providerCode)
		{
			#region pass-by-value parameters
			dynamic providerCode = XVar.Clone(_param_providerCode);
			#endregion

			return MVCFunctions.in_array((XVar)(providerCode), (XVar)(Security.providersInDb()));
		}
		public static XVar checkFieldAccess(dynamic _param_table, dynamic _param_field, dynamic _param_edit)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic edit = XVar.Clone(_param_edit);
			#endregion

			dynamic pageTable = null, pages = null;
			ProjectSettings pSet;
			pageTable = XVar.Clone(table);
			if(XVar.Equals(XVar.Pack(table), XVar.Pack(Security.loginTable())))
			{
				if(XVar.Pack(Security.checkFieldAccess(new XVar(Constants.GLOBAL_PAGES), (XVar)(field), (XVar)(edit))))
				{
					return true;
				}
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table), new XVar(""), new XVar("")));
			pages = XVar.Clone(pSet.getPageIds());
			foreach (KeyValuePair<XVar, dynamic> p in MVCFunctions.array_keys((XVar)(pages)).GetEnumerator())
			{
				if(XVar.Pack(Security.userHasFieldPermissions((XVar)(table), (XVar)(field), (XVar)(pSet.getPageType((XVar)(p.Value))), (XVar)(p.Value), (XVar)(edit))))
				{
					return true;
				}
			}
			return false;
		}
		public static XVar loggedInAsUser()
		{
			if((XVar)(!(XVar)(Security.hasLogin()))  || (XVar)(Security.hardcodedLogin()))
			{
				return false;
			}
			return Security.isLoggedIn();
		}
		public static XVar specialPermissionsTable(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			return (XVar)(Security.isAdminTable((XVar)(table)))  || (XVar)(table == Constants.GLOBAL_PAGES);
		}
		public static XVar isAdminTable(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			return XVar.Equals(XVar.Pack(table), XVar.Pack("admin_users"));
		}
		public static XVar rawUserData()
		{
			return CommonFunctions.storageGet(new XVar("rawUserData"));
		}
		public static XVar getUserPermissionsStatic(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic adminPerm = null, groupRights = XVar.Array(), permissions = XVar.Array(), tableRights = XVar.Array(), userGroup = null;
			if(XVar.Pack(!(XVar)(CommonFunctions.isLogged())))
			{
				return "";
			}
			userGroup = XVar.Clone(CommonFunctions.storageGet(new XVar("GroupID")));
			permissions = ProjectSettings.staticPermissions();
			groupRights = permissions[userGroup];
			if((XVar)(!(XVar)(groupRights))  && (XVar)(!(XVar)(Security.isGuest())))
			{
				groupRights = permissions["<Default>"];
			}
			if(XVar.Pack(!(XVar)(groupRights)))
			{
				return "";
			}
			tableRights = groupRights["permissions"][table];
			if(XVar.Pack(!(XVar)(tableRights)))
			{
				return "";
			}
			adminPerm = new XVar("");
			if(XVar.Pack(Security.isGuest()))
			{
				dynamic advSec = XVar.Array(), tablesAdvSecurity = XVar.Array();
				tablesAdvSecurity = ProjectSettings.getProjectValue(new XVar("tablesAdvSecurity"));
				advSec = tablesAdvSecurity[table];
				if((XVar)(advSec)  && (XVar)(XVar.Equals(XVar.Pack(advSec["advSecurity"]), XVar.Pack(Constants.ADVSECURITY_VIEW_OWN))))
				{
					return "";
				}
			}
			else
			{
				adminPerm = XVar.Clone((XVar.Pack(groupRights["admin"]) ? XVar.Pack("M") : XVar.Pack("")));
			}
			return MVCFunctions.Concat(tableRights["mask"], adminPerm);
		}
	}
}
