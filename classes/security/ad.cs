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
	public partial class SecurityPluginAd : SecurityPlugin
	{
		public dynamic useCustomLDAP = XVar.Pack(false);
		public dynamic ldapURL = XVar.Pack("");
		public dynamic ldapDomain = XVar.Pack("");
		public dynamic ldapBaseDN = XVar.Pack("");
		public dynamic ldapLoginFilter = XVar.Pack("(&(objectCategory=person)(|(sAMAccountName=%u)(userprincipalname=%u@%d)(userprincipalname=%u)))");
		public dynamic ldapLoginAttrs = XVar.Array();
		public dynamic ldapUsernames = XVar.Array();
		public dynamic ldapMemberAttrMap = XVar.Array();
		public dynamic ldapDisplayNameAttr = XVar.Pack("cn");
		public dynamic ldapGroupFilter = XVar.Pack("");
		public dynamic ldapGroupNameAttr = XVar.Pack("");
		public dynamic ldapMemberFilter = XVar.Pack("(&(|(objectCategory=group)(objectCategory=user))(|(cn=*%s*)(samaccountname=*%s*)))");
		public dynamic ldapObj = XVar.Pack(null);
		public dynamic followReferrals = XVar.Pack("");
		public dynamic loginAutomatically = XVar.Pack("");
		public dynamic username = XVar.Pack("");
		public dynamic password = XVar.Pack("");
		public dynamic useAdGroups = XVar.Pack(false);
		public dynamic useDbGroups = XVar.Pack(false);
		protected static bool skipSecurityPluginAdCtor = false;
		public SecurityPluginAd(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipSecurityPluginAdCtor)
			{
				skipSecurityPluginAdCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.ldapURL = XVar.Clone(var_params["url"]);
			this.ldapDomain = XVar.Clone(var_params["domain"]);
			this.ldapBaseDN = XVar.Clone(var_params["baseDN"]);
			this.followReferrals = XVar.Clone(var_params["followReferrals"]);
			this.loginAutomatically = XVar.Clone(var_params["loginAutomatically"]);
			this.username = XVar.Clone(var_params["username"]);
			this.password = XVar.Clone(var_params["password"]);
			this.useAdGroups = XVar.Clone(var_params["useAdGroups"]);
			this.useDbGroups = XVar.Clone((XVar)(!(XVar)(!(XVar)(var_params["useDbGroups"])))  && (XVar)(!(XVar)(!(XVar)(Security.extIdField()))));
			this.ldapUsernames = XVar.Clone(this.getDefaultUserNames());
			this.ldapMemberAttrMap = XVar.Clone(this.getDefaultMemberAttrMap());
			this.ldapObj = XVar.Clone(new RunnerLdap((XVar)(this.ldapURL)));
			this.useCustomLDAP = XVar.Clone(ProjectSettings.getProjectValue(new XVar("customLDAP")));
			if(XVar.Pack(this.useCustomLDAP))
			{
				this.ldapLoginFilter = XVar.Clone(GlobalVars.customLDAPSettings["loginFilter"]);
				this.ldapDisplayNameAttr = XVar.Clone(GlobalVars.customLDAPSettings["displayNameAttr"]);
				this.ldapGroupFilter = XVar.Clone(GlobalVars.customLDAPSettings["groupFilter"]);
				this.ldapGroupNameAttr = XVar.Clone(GlobalVars.customLDAPSettings["groupNameAttr"]);
				this.ldapUsernames = XVar.Clone(GlobalVars.customLDAPSettings["usernames"]);
				this.ldapMemberFilter = XVar.Clone(GlobalVars.customLDAPSettings["memberFilter"]);
				this.ldapMemberAttrMap = XVar.Clone(GlobalVars.customLDAPSettings["memberAttrMap"]);
				this.ldapLoginAttrs = XVar.Clone(XVar.Array());
			}
		}
		protected virtual XVar getDefaultUserNames()
		{
			dynamic usernames = XVar.Array();
			usernames = XVar.Clone(XVar.Array());
			usernames.InitAndSetArrayItem("%u@%d", null);
			usernames.InitAndSetArrayItem("%d\\%u", null);
			usernames.InitAndSetArrayItem("%u", null);
			usernames.InitAndSetArrayItem("uid=%u,%e", null);
			usernames.InitAndSetArrayItem("cname=%u,cname=Users,%e", null);
			usernames.InitAndSetArrayItem("cn=%u,%e", null);
			return usernames;
		}
		protected virtual XVar getDefaultMemberAttrMap()
		{
			return new XVar("name", "samaccountname", "displayname", new XVar(0, "displayname", 1, "cn", 2, "distinguishedname"), "category", "objectcategory", "email", "mail");
		}
		public virtual XVar getProcessedSearchPattern(dynamic _param_pattern, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic pattern = XVar.Clone(_param_pattern);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic queryStr = null;
			queryStr = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "%s")), (XVar)(new XVar(0, value)), (XVar)(pattern)));
			return MVCFunctions.str_replace(new XVar("**"), new XVar("*"), (XVar)(queryStr));
		}
		public virtual XVar getProcessedPattern(dynamic _param_pattern, dynamic _param_username)
		{
			#region pass-by-value parameters
			dynamic pattern = XVar.Clone(_param_pattern);
			dynamic username = XVar.Clone(_param_username);
			#endregion

			return MVCFunctions.str_replace((XVar)(new XVar(0, "%u", 1, "%d", 2, "%e")), (XVar)(new XVar(0, username, 1, this.ldapDomain, 2, this.ldapBaseDN)), (XVar)(pattern));
		}
		protected virtual XVar ldap_getCN(dynamic _param_dn)
		{
			#region pass-by-value parameters
			dynamic dn = XVar.Clone(_param_dn);
			#endregion

			dynamic firstRdn = null, rdnEqualPos = null, rdnList = XVar.Array();
			rdnList = XVar.Clone(MVCFunctions.explode(new XVar(","), (XVar)(dn)));
			firstRdn = XVar.Clone(rdnList[0]);
			rdnEqualPos = XVar.Clone(MVCFunctions.strpos((XVar)(firstRdn), new XVar("=")));
			if(XVar.Pack(rdnEqualPos))
			{
				return MVCFunctions.substr((XVar)(firstRdn), (XVar)(rdnEqualPos + 1));
			}
			return firstRdn;
		}
		protected virtual XVar ldap_getDN(dynamic _param_samaccountname)
		{
			#region pass-by-value parameters
			dynamic samaccountname = XVar.Clone(_param_samaccountname);
			#endregion

			dynamic attributes = null;
			attributes = XVar.Clone(new XVar(0, "dn"));
			return this.ldapObj.runner_ldap_getData((XVar)(MVCFunctions.Concat("", "(samaccountname=", samaccountname, ")")), (XVar)(this.ldapBaseDN), (XVar)(attributes));
		}
		protected virtual XVar getUserNames(dynamic _param_aUsername)
		{
			#region pass-by-value parameters
			dynamic aUsername = XVar.Clone(_param_aUsername);
			#endregion

			dynamic usernames = XVar.Array();
			usernames = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> usernamePattern in this.ldapUsernames.GetEnumerator())
			{
				usernames.InitAndSetArrayItem(this.getProcessedPattern((XVar)(usernamePattern.Value), (XVar)(aUsername)), null);
			}
			return usernames;
		}
		protected virtual XVar getLoginFilter(dynamic _param_username)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			#endregion

			return this.getProcessedPattern((XVar)(this.ldapLoginFilter), (XVar)(username));
		}
		public virtual XVar login(dynamic _param_username, dynamic _param_password, dynamic _param_loginUsername = null)
		{
			#region default values
			if(_param_loginUsername as Object == null) _param_loginUsername = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic password = XVar.Clone(_param_password);
			dynamic loginUsername = XVar.Clone(_param_loginUsername);
			#endregion

			dynamic displayName = null, entries = XVar.Array(), filter = null, ldapconn = null, rawData = XVar.Array(), userData = XVar.Array(), userGroups = null, userId = null, usernames = null;
			usernames = XVar.Clone(this.getUserNames((XVar)(username)));
			ldapconn = XVar.Clone(this.ldapObj.runner_ldap_connect((XVar)(usernames), (XVar)(password), (XVar)(this.followReferrals)));
			if(XVar.Pack(!(XVar)(ldapconn)))
			{
				this.var_error = XVar.Clone(this.var_error = XVar.Clone(this.ldapObj.ldap_error()));
				return false;
			}
			CommonFunctions.storageSet(new XVar("ldapPassword"), (XVar)(password));
			if(XVar.Pack(!(XVar)(loginUsername)))
			{
				loginUsername = XVar.Clone(username);
			}
			filter = XVar.Clone(this.getLoginFilter((XVar)(loginUsername)));
			entries = XVar.Clone(this.ldapObj.runner_ldap_getData((XVar)(filter), (XVar)(this.ldapBaseDN), (XVar)(this.ldapLoginAttrs)));
			if(XVar.Pack(!(XVar)(entries)))
			{
				this.var_error = XVar.Clone(CommonFunctions.mlang_message(new XVar("INVALID_LOGIN")));
				return false;
			}
			rawData = entries[0];
			displayName = XVar.Clone(this.getAttrValue((XVar)(rawData), (XVar)(this.ldapMemberAttrMap["displayname"])));
			userId = XVar.Clone(this.getAttrValue((XVar)(rawData), (XVar)(this.ldapMemberAttrMap["name"])));
			if(XVar.Pack(userId))
			{
				loginUsername = XVar.Clone(userId);
			}
			if(XVar.Pack(!(XVar)(displayName)))
			{
				displayName = XVar.Clone(loginUsername);
			}
			if(XVar.Pack(this.useDbGroups))
			{
				userData = XVar.Clone(XVar.Array());
				if(XVar.Pack(userId))
				{
					userData.InitAndSetArrayItem(MVCFunctions.Concat(this.code, userId), "id");
				}
				userData.InitAndSetArrayItem(loginUsername, "username");
				userData.InitAndSetArrayItem(displayName, "name");
				userData.InitAndSetArrayItem(this.getAttrValue((XVar)(rawData), (XVar)(this.ldapMemberAttrMap["email"])), "email");
				userData.InitAndSetArrayItem(rawData, "raw");
			}
			else
			{
				userData = XVar.Clone(rawData);
			}
			if(XVar.Pack(this.useAdGroups))
			{
				userGroups = XVar.Clone(this.getUserGroups((XVar)(loginUsername), (XVar)(rawData)));
				if((XVar)(Security.dynamicPermissions())  && (XVar)(rawData["distinguishedname"]))
				{
					this.addGroupsFromAD((XVar)(userGroups), (XVar)(rawData["distinguishedname"]));
				}
			}
			CommonFunctions.storageSet(new XVar("rawUserData"), (XVar)(rawData));
			return new XVar("userdata", userData, "groups", userGroups, "displayname", displayName, "username", loginUsername);
		}
		protected virtual XVar getUserGroups(dynamic _param_username, dynamic _param_loginEntry)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			dynamic loginEntry = XVar.Clone(_param_loginEntry);
			#endregion

			dynamic entries = XVar.Array(), filter = null, userGroups = XVar.Array();
			if(XVar.Pack(!(XVar)(this.useCustomLDAP)))
			{
				dynamic i = null;
				i = new XVar(0);
				userGroups = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> groupEntry in loginEntry["memberof"].GetEnumerator())
				{
					userGroups.InitAndSetArrayItem(this.ldap_getCN((XVar)(groupEntry.Value)), null);
					i++;
				}
				if(XVar.Pack(loginEntry.KeyExists("primarygroupid")))
				{
					dynamic attributes = null;
					filter = XVar.Clone(MVCFunctions.Concat("(&(objectSID=", this.ldapObj.getGroupSid((XVar)(loginEntry["objectsid"]), (XVar)(loginEntry["primarygroupid"])), "))"));
					attributes = XVar.Clone(new XVar(0, "cn"));
					entries = XVar.Clone(this.ldapObj.runner_ldap_getData((XVar)(filter), (XVar)(this.ldapBaseDN), (XVar)(attributes)));
					if(XVar.Pack(MVCFunctions.count(entries)))
					{
						userGroups.InitAndSetArrayItem(entries[0]["cn"], null);
					}
				}
				return userGroups;
			}
			userGroups = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(this.ldapGroupFilter)))
			{
				return userGroups;
			}
			filter = XVar.Clone(this.getProcessedPattern((XVar)(this.ldapGroupFilter), (XVar)(username)));
			entries = XVar.Clone(this.ldapObj.runner_ldap_getData((XVar)(filter), (XVar)(this.ldapBaseDN)));
			if(!XVar.Equals(XVar.Pack(entries), XVar.Pack(false)))
			{
				foreach (KeyValuePair<XVar, dynamic> group in entries.GetEnumerator())
				{
					userGroups.InitAndSetArrayItem(this.processAttrValue((XVar)(group.Value[this.ldapGroupNameAttr])), null);
				}
			}
			return userGroups;
		}
		protected virtual XVar addGroupsFromAD(dynamic userGroups, dynamic _param_distinguishedName)
		{
			#region pass-by-value parameters
			dynamic distinguishedName = XVar.Clone(_param_distinguishedName);
			#endregion

			dynamic dataSource = null, dbgroups = XVar.Array(), entries_dn = XVar.Array(), filter = null, qResult = null, tdata = XVar.Array();
			if(XVar.Pack(!(XVar)(GlobalVars.adNestedPermissions)))
			{
				return null;
			}
			dbgroups = XVar.Clone(XVar.Array());
			dataSource = XVar.Clone(Security.getUgGroupsDatasource());
			qResult = XVar.Clone(dataSource.getList((XVar)(new DsCommand())));
			while(XVar.Pack(tdata = XVar.Clone(qResult.fetchAssoc())))
			{
				dbgroups.InitAndSetArrayItem(true, tdata["Label"]);
			}
			filter = XVar.Clone(MVCFunctions.Concat("(member:1.2.840.113556.1.4.1941:=", distinguishedName, ")"));
			entries_dn = XVar.Clone(this.ldapObj.runner_ldap_getData((XVar)(filter), (XVar)(this.ldapBaseDN), (XVar)(new XVar(0, "cn"))));
			if(XVar.Pack(!(XVar)(entries_dn)))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> group in entries_dn.GetEnumerator())
			{
				dynamic adgroup = null;
				adgroup = XVar.Clone(group.Value["cn"]);
				if((XVar)(dbgroups[adgroup])  && (XVar)(!(XVar)(MVCFunctions.in_array((XVar)(adgroup), (XVar)(userGroups)))))
				{
					userGroups.InitAndSetArrayItem(adgroup, null);
				}
			}

			return null;
		}
		public virtual XVar loginAuto(dynamic _param_remoteUsername)
		{
			#region pass-by-value parameters
			dynamic remoteUsername = XVar.Clone(_param_remoteUsername);
			#endregion

			dynamic userInfo = null, username = null;
			if(XVar.Pack(!(XVar)(this.loginAutomatically)))
			{
				return false;
			}
			username = XVar.Clone(SecurityPluginAd.refineRemoteUsername((XVar)(remoteUsername)));
			if(XVar.Pack(!(XVar)(username)))
			{
				return false;
			}
			if((XVar)(this.username)  || (XVar)((XVar)(Security.dynamicPermissions())  && (XVar)(this.useAdGroups)))
			{
				userInfo = XVar.Clone(this.login((XVar)(this.username), (XVar)(this.password), (XVar)(username)));
				if(XVar.Pack(!(XVar)(userInfo)))
				{
					return false;
				}
			}
			else
			{
				userInfo = XVar.Clone(new XVar("userdata", new XVar("id", MVCFunctions.Concat(this.code, username)), "displayname", username, "username", username, "groups", XVar.Array()));
			}
			return this.createUserSession((XVar)(userInfo), new XVar(true));
		}
		public static XVar refineRemoteUsername(dynamic _param_remoteUsername)
		{
			#region pass-by-value parameters
			dynamic remoteUsername = XVar.Clone(_param_remoteUsername);
			#endregion

			dynamic parts = XVar.Array();
			parts = XVar.Clone(MVCFunctions.explode(new XVar("\\"), (XVar)(remoteUsername)));
			if(XVar.Pack(parts.KeyExists(1)))
			{
				return parts[1];
			}
			return remoteUsername;
		}
		public virtual XVar createUserSession(dynamic _param_userInfo, dynamic _param_autoLogin = null, dynamic _param_nextSessionLevel = null)
		{
			#region default values
			if(_param_autoLogin as Object == null) _param_autoLogin = new XVar(false);
			if(_param_nextSessionLevel as Object == null) _param_nextSessionLevel = new XVar(Constants.LOGGED_FULL);
			#endregion

			#region pass-by-value parameters
			dynamic userInfo = XVar.Clone(_param_userInfo);
			dynamic autoLogin = XVar.Clone(_param_autoLogin);
			dynamic nextSessionLevel = XVar.Clone(_param_nextSessionLevel);
			#endregion

			dynamic adGroups = null, data = null, displayName = null, userData = XVar.Array(), username = null;
			if(XVar.Pack(!(XVar)(userInfo)))
			{
				return false;
			}
			userData = XVar.Clone(userInfo["userdata"]);
			adGroups = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.useDbGroups))
			{
				dynamic dbUserData = XVar.Array();
				dbUserData = XVar.Clone(Security.fetchUpdateDatabaseUser((XVar)(userData), new XVar(true)));
				data = XVar.Clone(dbUserData["data"]);
				username = XVar.Clone(userData["username"]);
				displayName = XVar.Clone(userData["name"]);
			}
			else
			{
				data = XVar.Clone(userData);
				username = XVar.Clone(userInfo["username"]);
				displayName = XVar.Clone(userInfo["displayname"]);
			}
			if(XVar.Pack(this.useAdGroups))
			{
				adGroups = XVar.Clone(userInfo["groups"]);
			}
			if(nextSessionLevel != Constants.LOGGED_FULL)
			{
				Security.createProvisionalSession((XVar)(this.provider), (XVar)(nextSessionLevel), (XVar)(username), (XVar)(displayName), (XVar)(data));
				return true;
			}
			Security.createUserSession((XVar)(this.provider), (XVar)(username), (XVar)(displayName), (XVar)(data), (XVar)(adGroups), (XVar)(autoLogin));
			return true;
		}
		public virtual XVar getGroupList(dynamic _param_search, dynamic _param_hideUntilSearch = null)
		{
			#region default values
			if(_param_hideUntilSearch as Object == null) _param_hideUntilSearch = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic search = XVar.Clone(_param_search);
			dynamic hideUntilSearch = XVar.Clone(_param_hideUntilSearch);
			#endregion

			dynamic attrMapNames = XVar.Array(), attributes = XVar.Array(), data = null, filter = null, ldapconn = null;
			ldapconn = XVar.Clone(this.ldapObj.runner_ldap_connect((XVar)(this.getUserNames((XVar)(this.ldapUsername()))), (XVar)(this.ldapPassword()), (XVar)(this.followReferrals)));
			if(XVar.Pack(!(XVar)(ldapconn)))
			{
				return XVar.Array();
			}
			if((XVar)(!(XVar)(MVCFunctions.strlen((XVar)(search))))  && (XVar)(hideUntilSearch))
			{
				return XVar.Array();
			}
			filter = XVar.Clone(this.getMembersFilter((XVar)(search)));
			attributes = XVar.Clone(XVar.Array());
			attrMapNames = XVar.Clone(new XVar(0, "name", 1, "displayname", 2, "category", 3, "email"));
			foreach (KeyValuePair<XVar, dynamic> an in attrMapNames.GetEnumerator())
			{
				dynamic attr = XVar.Array();
				attr = XVar.Clone(this.ldapMemberAttrMap[an.Value]);
				if(XVar.Pack(!(XVar)(attr)))
				{
					continue;
				}
				if(XVar.Pack(MVCFunctions.is_array((XVar)(attr))))
				{
					foreach (KeyValuePair<XVar, dynamic> a in attr.GetEnumerator())
					{
						attributes.InitAndSetArrayItem(a.Value, null);
					}
				}
				else
				{
					attributes.InitAndSetArrayItem(attr, null);
				}
			}
			if(XVar.Pack(!(XVar)(attributes)))
			{
				return XVar.Array();
			}
			data = XVar.Clone(this.ldapObj.runner_ldap_getData((XVar)(filter), (XVar)(this.ldapBaseDN), (XVar)(attributes)));
			this.ldapObj.runner_ldap_unbind();
			if(XVar.Pack(data))
			{
				dynamic result = null;
				result = XVar.Clone(this.getMembersResult((XVar)(data)));
				return result;
			}
			return null;
		}
		protected virtual XVar ldapUsername()
		{
			if(XVar.Pack(this.username))
			{
				return this.username;
			}
			return Security.getUserName();
		}
		protected virtual XVar ldapPassword()
		{
			if(XVar.Pack(this.username))
			{
				return this.password;
			}
			return CommonFunctions.storageGet(new XVar("ldapPassword"));
		}
		protected virtual XVar getMembersFilter(dynamic _param_aGrName)
		{
			#region pass-by-value parameters
			dynamic aGrName = XVar.Clone(_param_aGrName);
			#endregion

			return this.getProcessedSearchPattern((XVar)(this.ldapMemberFilter), (XVar)(aGrName));
		}
		public virtual XVar processAttrValue(dynamic _param_attValue)
		{
			#region pass-by-value parameters
			dynamic attValue = XVar.Clone(_param_attValue);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(attValue)))))
			{
				return attValue;
			}
			foreach (KeyValuePair<XVar, dynamic> value in attValue.GetEnumerator())
			{
				return value.Value;
			}

			return null;
		}
		protected virtual XVar getMembersResult(dynamic data)
		{
			dynamic result = XVar.Array();
			result = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> row in data.GetEnumerator())
			{
				dynamic nameAttr = null;
				nameAttr = XVar.Clone(this.getAttrValue((XVar)(row.Value), (XVar)(this.ldapMemberAttrMap["name"])));
				if(XVar.Pack(!(XVar)(nameAttr)))
				{
					continue;
				}
				result.InitAndSetArrayItem(new XVar("name", this.processAttrValue((XVar)(nameAttr)), "displayname", this.ldap_getCN((XVar)(this.processAttrValue((XVar)(this.getAttrValue((XVar)(row.Value), (XVar)(this.ldapMemberAttrMap["displayname"])))))), "category", this.ldap_getCN((XVar)(this.processAttrValue((XVar)(this.getAttrValue((XVar)(row.Value), (XVar)(this.ldapMemberAttrMap["category"])))))), "email", this.processAttrValue((XVar)(this.getAttrValue((XVar)(row.Value), (XVar)(this.ldapMemberAttrMap["email"]))))), null);
			}
			return result;
		}
		protected virtual XVar getAttrValue(dynamic data, dynamic _param_attrNames)
		{
			#region pass-by-value parameters
			dynamic attrNames = XVar.Clone(_param_attrNames);
			#endregion

			dynamic attrs = XVar.Array();
			attrs = XVar.Clone(attrNames);
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(attrs)))))
			{
				attrs = XVar.Clone(new XVar(0, attrNames));
			}
			foreach (KeyValuePair<XVar, dynamic> attrName in attrs.GetEnumerator())
			{
				if(XVar.Pack(data[attrName.Value]))
				{
					return data[attrName.Value];
				}
			}
			return "";
		}
		public virtual XVar loginAsUser(dynamic _param_username)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			#endregion

			dynamic userInfo = null;
			if(XVar.Pack(this.username))
			{
				userInfo = XVar.Clone(this.login((XVar)(this.username), (XVar)(this.password), (XVar)(username)));
			}
			else
			{
				userInfo = XVar.Clone(new XVar("userdata", new XVar("id", MVCFunctions.Concat(this.code, username)), "displayname", username, "username", username, "groups", XVar.Array()));
			}
			return this.createUserSession((XVar)(userInfo));
		}
	}
}
