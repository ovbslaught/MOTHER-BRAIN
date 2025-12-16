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
	public partial class RunnerNotifications : XClass
	{
		public dynamic enabled;
		public dynamic messageLimit = XVar.Pack(30);
		public dynamic table;
		public dynamic dataSource;
		public RunnerNotifications(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.enabled = XVar.Clone(var_params["enabled"]);
			this.table = XVar.Clone(var_params["table"]);
			this.dataSource = XVar.Clone(CommonFunctions.getDbTableDataSource((XVar)(this.table["table"]), (XVar)(this.table["connId"])));
		}
		protected virtual XVar fetchRecords(dynamic _param_dc, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic data = XVar.Array(), note = XVar.Array(), ret = XVar.Array(), rs = null;
			rs = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				return XVar.Array();
			}
			ret = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(rs.fetchAssoc())))
			{
				note = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> k in keys.GetEnumerator())
				{
					note.InitAndSetArrayItem(data[k.Value], k.Value);
				}
				if(data["type"] == 0)
				{
					if(MVCFunctions.substr((XVar)(note["message"]), new XVar(0), new XVar(1)) == "{")
					{
						dynamic messageData = XVar.Array();
						messageData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(note["message"])));
						if(XVar.Pack(messageData))
						{
							note.InitAndSetArrayItem(messageData["message"], "message");
							note.InitAndSetArrayItem(messageData["newWindow"], "newWindow");
						}
					}
				}
				ret.InitAndSetArrayItem(note, null);
			}
			return ret;
		}
		protected virtual XVar fetchMessages(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic keys = null;
			keys = XVar.Clone(new XVar(0, "message", 1, "title", 2, "url", 3, "icon", 4, "created", 5, "expire", 6, "id"));
			return this.fetchRecords((XVar)(dc), (XVar)(keys));
		}
		public virtual XVar getUpdates(dynamic _param_lastId)
		{
			#region pass-by-value parameters
			dynamic lastId = XVar.Clone(_param_lastId);
			#endregion

			dynamic dc = null, messages = null;
			if(XVar.Pack(!(XVar)(this.enabled)))
			{
				return XVar.Array();
			}
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, this.userCondition(), 1, this.timeCondition(), 2, this.idCondition((XVar)(lastId)), 3, this.typeCondition(new XVar(Constants.ntMESSAGE))))));
			dc.order.InitAndSetArrayItem(new XVar("column", "created", "dir", "DESC"), null);
			dc.reccount = XVar.Clone(this.messageLimit);
			messages = XVar.Clone(this.fetchMessages((XVar)(dc)));
			return this.applyPermissionsFilter((XVar)(messages));
		}
		public virtual XVar getMessages()
		{
			dynamic dc = null, messages = null;
			if(XVar.Pack(!(XVar)(this.enabled)))
			{
				return XVar.Array();
			}
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, this.userCondition(), 1, this.timeCondition(), 2, this.typeCondition(new XVar(Constants.ntMESSAGE))))));
			dc.order.InitAndSetArrayItem(new XVar("column", "created", "dir", "DESC"), null);
			dc.reccount = XVar.Clone(this.messageLimit);
			messages = XVar.Clone(this.fetchMessages((XVar)(dc)));
			return this.applyPermissionsFilter((XVar)(messages));
		}
		public virtual XVar getUserData()
		{
			dynamic ds = null, rs = null;
			if(XVar.Pack(!(XVar)(this.enabled)))
			{
				return false;
			}
			ds = XVar.Clone(new DsCommand());
			ds.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, this.userCondition(), 1, this.typeCondition(new XVar(Constants.ntUSERDATA))))));
			rs = XVar.Clone(this.dataSource.getSingle((XVar)(ds)));
			if(XVar.Pack(rs))
			{
				dynamic data = XVar.Array();
				data = XVar.Clone(rs.fetchAssoc());
				if(XVar.Pack(data))
				{
					return MVCFunctions.runner_json_decode((XVar)(data["message"]));
				}
			}
			return false;
		}
		public virtual XVar saveUserData(dynamic _param_data, dynamic _param_update)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic update = XVar.Clone(_param_update);
			#endregion

			dynamic dc = null;
			if(XVar.Pack(!(XVar)(this.enabled)))
			{
				return null;
			}
			dc = XVar.Clone(new DsCommand());
			dc.values.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(data)), "message");
			if(XVar.Pack(update))
			{
				dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, this.userCondition(), 1, this.typeCondition(new XVar(Constants.ntUSERDATA))))));
				return !(XVar)(!(XVar)(this.dataSource.updateSingle((XVar)(dc), new XVar(false))));
			}
			else
			{
				if(XVar.Pack(Security.loggedInAsUser()))
				{
					dc.values.InitAndSetArrayItem(Security.getUserName(), "user");
					dc.values.InitAndSetArrayItem(Security.currentProviderCode(), "provider");
				}
				else
				{
					dc.values.InitAndSetArrayItem(MVCFunctions.session_id(), "user");
				}
				dc.values.InitAndSetArrayItem(Constants.ntUSERDATA, "type");
				return !(XVar)(!(XVar)(this.dataSource.insertSingle((XVar)(dc))));
			}

			return null;
		}
		public virtual XVar getLastRead()
		{
			dynamic userData = XVar.Array();
			if(XVar.Pack(!(XVar)(this.enabled)))
			{
				return false;
			}
			userData = XVar.Clone(this.getUserData());
			if(XVar.Pack(userData))
			{
				return userData["lastRead"];
			}
			return 0;
		}
		public virtual XVar updateLastRead(dynamic _param_newId)
		{
			#region pass-by-value parameters
			dynamic newId = XVar.Clone(_param_newId);
			#endregion

			dynamic update = null, userData = XVar.Array();
			update = new XVar(true);
			userData = XVar.Clone(this.getUserData());
			if(XVar.Equals(XVar.Pack(userData), XVar.Pack(false)))
			{
				update = new XVar(false);
			}
			if(XVar.Pack(!(XVar)(userData)))
			{
				userData = XVar.Clone(XVar.Array());
			}
			userData.InitAndSetArrayItem(newId, "lastRead");
			this.saveUserData((XVar)(userData), (XVar)(update));

			return null;
		}
		protected virtual XVar userCondition(dynamic _param_strict = null)
		{
			#region default values
			if(_param_strict as Object == null) _param_strict = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic strict = XVar.Clone(_param_strict);
			#endregion

			dynamic conditions = XVar.Array(), provider = null, providerCondition = null, userId = null;
			if(XVar.Pack(Security.loggedInAsUser()))
			{
				provider = XVar.Clone(Security.currentProviderCode());
				userId = XVar.Clone(Security.getUserName());
			}
			else
			{
				provider = new XVar("");
				userId = XVar.Clone(MVCFunctions.session_id());
			}
			providerCondition = XVar.Clone((XVar.Pack(provider) ? XVar.Pack(DataCondition.FieldEquals(new XVar("provider"), (XVar)(provider))) : XVar.Pack(DataCondition.FieldIs(new XVar("provider"), new XVar(Constants.dsopEMPTY), new XVar("")))));
			conditions = XVar.Clone(new XVar(0, DataCondition._And((XVar)(new XVar(0, DataCondition.FieldEquals(new XVar("user"), (XVar)(userId)), 1, providerCondition)))));
			if(XVar.Pack(!(XVar)(strict)))
			{
				conditions.InitAndSetArrayItem(DataCondition.FieldIs(new XVar("user"), new XVar(Constants.dsopEMPTY), new XVar("")), null);
			}
			return DataCondition._Or((XVar)(conditions));
		}
		protected virtual XVar timeCondition()
		{
			dynamic conditions = XVar.Array();
			conditions = XVar.Clone(XVar.Array());
			conditions.InitAndSetArrayItem(DataCondition._Or((XVar)(new XVar(0, DataCondition.FieldIs(new XVar("expire"), new XVar(Constants.dsopMORE), (XVar)(MVCFunctions.now())), 1, DataCondition.FieldIs(new XVar("expire"), new XVar(Constants.dsopEMPTY), new XVar(""))))), null);
			return DataCondition._And((XVar)(conditions));
		}
		protected virtual XVar typeCondition(dynamic _param_type)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			#endregion

			return DataCondition.FieldEquals(new XVar("type"), (XVar)(var_type));
		}
		protected virtual XVar idCondition(dynamic _param_lastId)
		{
			#region pass-by-value parameters
			dynamic lastId = XVar.Clone(_param_lastId);
			#endregion

			return DataCondition.FieldIs(new XVar("id"), new XVar(Constants.dsopMORE), (XVar)(lastId));
		}
		public virtual XVar create(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic messageId = null, permissions = null, permissionsType = null, ret = XVar.Array();
			ret = XVar.Clone(this.dataSource.insertSingle((XVar)(this.getAddMessageCommand((XVar)(var_params)))));
			if(XVar.Pack(!(XVar)(ret)))
			{
				return false;
			}
			messageId = XVar.Clone(ret["id"]);
			permissions = XVar.Clone(var_params["permissions"]);
			permissionsType = XVar.Clone(this.permissionsType((XVar)(permissions)));
			if((XVar)(permissionsType == Constants.ntPermTypeGroup)  || (XVar)(permissionsType == Constants.ntPermTypePage))
			{
				dynamic permissionsRet = null;
				permissionsRet = XVar.Clone(this.dataSource.insertSingle((XVar)(this.getAddPermissionsCommand((XVar)(messageId), (XVar)(permissions)))));
				if(XVar.Pack(!(XVar)(permissionsRet)))
				{
					return false;
				}
			}
			return messageId;
		}
		protected virtual XVar getAddMessageCommand(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic dc = null, keys = XVar.Array(), messageData = null, permissions = XVar.Array(), permissionsType = null, userKeys = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			keys = XVar.Clone(new XVar(0, "message", 1, "title", 2, "url", 3, "icon", 4, "expire"));
			foreach (KeyValuePair<XVar, dynamic> k in keys.GetEnumerator())
			{
				if(XVar.Pack(var_params.KeyExists(k.Value)))
				{
					dc.values.InitAndSetArrayItem(var_params[k.Value], k.Value);
				}
			}
			messageData = XVar.Clone(new XVar("message", var_params["message"], "newWindow", var_params["newWindow"]));
			dc.values.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(messageData)), "message");
			permissions = XVar.Clone(var_params["permissions"]);
			permissionsType = XVar.Clone(this.permissionsType((XVar)(permissions)));
			userKeys = XVar.Clone((XVar.Pack(permissionsType == Constants.ntPermTypeUser) ? XVar.Pack(new XVar(0, "user", 1, "provider")) : XVar.Pack(XVar.Array())));
			foreach (KeyValuePair<XVar, dynamic> k in userKeys.GetEnumerator())
			{
				if(XVar.Pack(permissions.KeyExists(k.Value)))
				{
					dc.values.InitAndSetArrayItem(permissions[k.Value], k.Value);
				}
			}
			if((XVar)(var_params["expire"])  && (XVar)(MVCFunctions.IsNumeric(var_params["expire"])))
			{
				dynamic expire = null;
				expire = XVar.Clone(CommonFunctions.dbFormatDateTime((XVar)(CommonFunctions.addMinutes((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), (XVar)(var_params["expire"])))));
				dc.values.InitAndSetArrayItem(expire, "expire");
			}
			dc.values.InitAndSetArrayItem(MVCFunctions.now(), "created");
			dc.values.InitAndSetArrayItem(Constants.ntMESSAGE, "type");
			return dc;
		}
		protected virtual XVar getAddPermissionsCommand(dynamic _param_messageId, dynamic _param_permissions)
		{
			#region pass-by-value parameters
			dynamic messageId = XVar.Clone(_param_messageId);
			dynamic permissions = XVar.Clone(_param_permissions);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(new DsCommand());
			dc.values.InitAndSetArrayItem(messageId, "title");
			dc.values.InitAndSetArrayItem(Constants.ntPERMISSIONS, "type");
			dc.values.InitAndSetArrayItem(MVCFunctions.runner_json_encode((XVar)(permissions)), "message");
			return dc;
		}
		protected virtual XVar getPermissions(dynamic _param_msgIdList)
		{
			#region pass-by-value parameters
			dynamic msgIdList = XVar.Clone(_param_msgIdList);
			#endregion

			dynamic dc = null, inListCondition = null, keys = null;
			dc = XVar.Clone(new DsCommand());
			inListCondition = XVar.Clone((XVar.Pack(0 < MVCFunctions.count(msgIdList)) ? XVar.Pack(DataCondition.FieldInList(new XVar("title"), (XVar)(msgIdList))) : XVar.Pack(DataCondition._False())));
			dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, inListCondition, 1, this.typeCondition(new XVar(Constants.ntPERMISSIONS))))));
			keys = XVar.Clone(new XVar(0, "message", 1, "title"));
			return this.fetchRecords((XVar)(dc), (XVar)(keys));
		}
		protected virtual XVar applyPermissionsFilter(dynamic _param_messages)
		{
			#region pass-by-value parameters
			dynamic messages = XVar.Clone(_param_messages);
			#endregion

			dynamic msgIdList = XVar.Array(), msgMap = XVar.Array(), msgPermissionsMap = XVar.Array(), permissions = null, permissionsRecordsList = XVar.Array(), returnMessages = XVar.Array();
			msgIdList = XVar.Clone(XVar.Array());
			msgMap = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> msg in messages.GetEnumerator())
			{
				dynamic msgId = null;
				msgId = XVar.Clone(msg.Value["id"]);
				msgIdList.InitAndSetArrayItem(msgId, null);
				msgMap.InitAndSetArrayItem(msg.Value, msgId);
			}
			permissionsRecordsList = XVar.Clone(this.getPermissions((XVar)(msgIdList)));
			msgPermissionsMap = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> permissionsRecord in permissionsRecordsList.GetEnumerator())
			{
				dynamic messageId = null;
				messageId = XVar.Clone(permissionsRecord.Value["title"]);
				permissions = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(permissionsRecord.Value["message"])));
				msgPermissionsMap.InitAndSetArrayItem(permissions, messageId);
			}
			returnMessages = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> message in msgMap.GetEnumerator())
			{
				permissions = XVar.Clone(msgPermissionsMap[message.Key]);
				if(XVar.Pack(this.userFitsPermissions((XVar)(permissions))))
				{
					returnMessages.InitAndSetArrayItem(message.Value, null);
				}
			}
			return returnMessages;
		}
		protected virtual XVar userFitsPermissions(dynamic _param_permissions)
		{
			#region pass-by-value parameters
			dynamic permissions = XVar.Clone(_param_permissions);
			#endregion

			dynamic permissionsType = null;
			permissionsType = XVar.Clone(this.permissionsType((XVar)(permissions)));
			if(permissionsType == Constants.ntPermTypeNone)
			{
				return true;
			}
			if(permissionsType == Constants.ntPermTypePage)
			{
				dynamic page = null, table = null;
				table = XVar.Clone(permissions["table"]);
				page = XVar.Clone(permissions["page"]);
				return Security.userCanSeePage((XVar)(table), (XVar)(page));
			}
			if(permissionsType == Constants.ntPermTypeGroup)
			{
				dynamic group = null, userGroups = XVar.Array();
				group = XVar.Clone(permissions["group"]);
				userGroups = XVar.Clone(Security.getUserGroups());
				return userGroups[group];
			}
			if(permissionsType == Constants.ntPermTypeUser)
			{
				return true;
			}
			return false;
		}
		protected virtual XVar permissionsType(dynamic _param_permissions)
		{
			#region pass-by-value parameters
			dynamic permissions = XVar.Clone(_param_permissions);
			#endregion

			if((XVar)(XVar.Equals(XVar.Pack(permissions), XVar.Pack(null)))  || (XVar)(!(XVar)(MVCFunctions.is_array((XVar)(permissions)))))
			{
				return Constants.ntPermTypeNone;
			}
			if(XVar.Pack(permissions["user"]))
			{
				return Constants.ntPermTypeUser;
			}
			if(XVar.Pack(permissions["group"]))
			{
				return Constants.ntPermTypeGroup;
			}
			if((XVar)(permissions["table"])  && (XVar)(permissions["page"]))
			{
				return Constants.ntPermTypePage;
			}
			return Constants.ntPermTypeNone;
		}
	}
}
