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
	public partial class AuditTrailTable : XClass
	{
		public dynamic logTableName;
		public dynamic var_params;
		public dynamic strLogin = XVar.Pack("login");
		public dynamic strFailLogin = XVar.Pack("failed login");
		public dynamic strLogout = XVar.Pack("logout");
		public dynamic strChPass = XVar.Pack("change password");
		public dynamic strAdd = XVar.Pack("add");
		public dynamic strEdit = XVar.Pack("edit");
		public dynamic strDelete = XVar.Pack("delete");
		public dynamic strAccess = XVar.Pack("access");
		public dynamic strKeysHeader = XVar.Pack("---Keys");
		public dynamic strFieldsHeader = XVar.Pack("---Fields");
		public dynamic strUserinfo = XVar.Pack("change userinfo");
		public dynamic columnDate = XVar.Pack("Date");
		public dynamic columnTime = XVar.Pack("Time");
		public dynamic columnIP = XVar.Pack("IP");
		public dynamic columnUser = XVar.Pack("User");
		public dynamic columnTable = XVar.Pack("Table");
		public dynamic columnAction = XVar.Pack("Action");
		public dynamic columnKey = XVar.Pack("Key field");
		public dynamic columnField = XVar.Pack("Field");
		public dynamic columnOldValue = XVar.Pack("Old value");
		public dynamic columnNewValue = XVar.Pack("New value");
		public dynamic attLogin = XVar.Pack(0);
		public dynamic timeLogin = XVar.Pack(0);
		public dynamic maxFieldLength;
		protected dynamic connection;
		public AuditTrailTable()
		{
			dynamic userid = null;
			this.connection = XVar.Clone(GlobalVars.cman.getForAudit());
			this.logTableName = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("auditAndLocking"), new XVar("loggingTable"), new XVar("table")));
			if(XVar.Pack(ProjectSettings.getSecurityValue(new XVar("auditAndLocking"), new XVar("lockAfterUnsuccessfulLogin"))))
			{
				this.attLogin = new XVar(3);
				this.timeLogin = new XVar(5);
			}
			userid = new XVar("");
			if(XVar.Pack(Security.getUserName()))
			{
				userid = XVar.Clone(Security.getUserName());
			}
			this.var_params = XVar.Clone(new XVar(0, MVCFunctions.remoteAddr(), 1, userid));
			this.maxFieldLength = XVar.Clone(GlobalVars.auditMaxFieldLength);
		}
		public virtual XVar logSecurity()
		{
			return ProjectSettings.getSecurityValue(new XVar("auditAndLocking"), new XVar("logSecurityActions"));
		}
		public virtual XVar LogLogin(dynamic _param_pUsername)
		{
			#region pass-by-value parameters
			dynamic pUsername = XVar.Clone(_param_pUsername);
			#endregion

			dynamic arr = null, retval = null, table = null;
			if(XVar.Pack(!(XVar)(this.logSecurity())))
			{
				return null;
			}
			retval = new XVar(true);
			table = XVar.Clone(Security.loginTable());
			this.var_params.InitAndSetArrayItem(pUsername, 1);
			arr = XVar.Clone(XVar.Array());
			this.var_params = XVar.Clone(new XVar(0, MVCFunctions.remoteAddr(), 1, Security.getUserName()));
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strLogin), (XVar)(this.var_params), (XVar)(table), (XVar)(arr), (XVar)(arr), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				this.insert((XVar)(MVCFunctions.now()), (XVar)(this.var_params[0]), (XVar)(this.var_params[1]), (XVar)(table), (XVar)(this.strLogin), new XVar(""));
			}
			return retval;
		}
		public virtual XVar LogLoginFailed(dynamic _param_pUsername)
		{
			#region pass-by-value parameters
			dynamic pUsername = XVar.Clone(_param_pUsername);
			#endregion

			dynamic arr = null, retval = null, table = null;
			if(XVar.Pack(!(XVar)(this.logSecurity())))
			{
				return null;
			}
			retval = new XVar(true);
			table = XVar.Clone(Security.loginTable());
			this.var_params.InitAndSetArrayItem(pUsername, 1);
			arr = XVar.Clone(XVar.Array());
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strFailLogin), (XVar)(this.var_params), (XVar)(table), (XVar)(arr), (XVar)(arr), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				this.insert((XVar)(MVCFunctions.now()), (XVar)(this.var_params[0]), (XVar)(this.var_params[1]), (XVar)(table), (XVar)(this.strFailLogin), new XVar(""));
			}
			this.var_params = XVar.Clone(new XVar(0, MVCFunctions.remoteAddr(), 1, ""));
			return retval;
		}
		public virtual XVar LogLogout()
		{
			if(XVar.Pack(!(XVar)(this.logSecurity())))
			{
				return null;
			}
			if(Security.getUserName() != "")
			{
				dynamic arr = null, retval = null, table = null;
				retval = new XVar(true);
				table = XVar.Clone(Security.loginTable());
				arr = XVar.Clone(XVar.Array());
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
				{
					retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strLogout), (XVar)(this.var_params), (XVar)(table), (XVar)(arr), (XVar)(arr), (XVar)(arr)));
				}
				if(XVar.Pack(retval))
				{
					this.insert((XVar)(MVCFunctions.now()), (XVar)(this.var_params[0]), (XVar)(this.var_params[1]), (XVar)(table), (XVar)(this.strLogout), new XVar(""));
				}
				return retval;
			}

			return null;
		}
		public virtual XVar LogChPassword(dynamic _param_username)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			#endregion

			dynamic arr = null, retval = null, table = null;
			if(XVar.Pack(!(XVar)(this.logSecurity())))
			{
				return null;
			}
			retval = new XVar(true);
			table = XVar.Clone(Security.loginTable());
			arr = XVar.Clone(XVar.Array());
			this.var_params.InitAndSetArrayItem(username, 1);
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strChPass), (XVar)(this.var_params), (XVar)(table), (XVar)(arr), (XVar)(arr), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				this.insert((XVar)(MVCFunctions.now()), (XVar)(this.var_params[0]), (XVar)(this.var_params[1]), (XVar)(table), (XVar)(this.strChPass), new XVar(""));
			}
			return retval;
		}
		public virtual XVar LogAdd(dynamic _param_str_table, dynamic _param_values, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic str_table = XVar.Clone(_param_str_table);
			dynamic values = XVar.Clone(_param_values);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic arr = null, retval = null, table = null;
			ProjectSettings pSet;
			retval = new XVar(true);
			table = XVar.Clone(str_table);
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(str_table)));
			arr = XVar.Clone(XVar.Array());
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strAdd), (XVar)(this.var_params), (XVar)(table), (XVar)(keys), (XVar)(values), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				dynamic str = null, strFields = null;
				str = new XVar("");
				if(0 < MVCFunctions.count(keys))
				{
					str = MVCFunctions.Concat(str, this.strKeysHeader, "\r\n");
					foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
					{
						str = MVCFunctions.Concat(str, val.Key, " : ", val.Value, "\r\n");
					}
				}
				strFields = new XVar("");
				if(XVar.Pack(this.logValueEnable((XVar)(str_table))))
				{
					foreach (KeyValuePair<XVar, dynamic> val in values.GetEnumerator())
					{
						if((XVar)(val.Value != XVar.Pack(""))  && (XVar)(!(XVar)(keys.KeyExists(val.Key))))
						{
							dynamic v = null;
							strFields = MVCFunctions.Concat(strFields, val.Key, " [new]: ");
							if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(pSet.getFieldType((XVar)(val.Key))))))
							{
								v = new XVar("<binary value>");
							}
							else
							{
								v = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(val.Value)));
								v = XVar.Clone(this.getMaxLengthSubstr((XVar)(v)));
							}
							strFields = MVCFunctions.Concat(strFields, v, "\r\n");
						}
					}
				}
				if(strFields != XVar.Pack(""))
				{
					str = MVCFunctions.Concat(str, this.strFieldsHeader, "\r\n", strFields);
				}
				this.insert((XVar)(MVCFunctions.now()), (XVar)(this.var_params[0]), (XVar)(this.var_params[1]), (XVar)(str_table), (XVar)(this.strAdd), (XVar)(str));
			}
			return retval;
		}
		public virtual XVar formatChangedValues(dynamic _param_pSet_packed, dynamic _param_newValues, dynamic _param_oldValues, dynamic _param_include = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_include as Object == null) _param_include = new XVar();
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic newValues = XVar.Clone(_param_newValues);
			dynamic oldValues = XVar.Clone(_param_oldValues);
			dynamic include = XVar.Clone(_param_include);
			#endregion

			dynamic strings = null;
			strings = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> value in newValues.GetEnumerator())
			{
				dynamic field = null, newValue = null, oldValue = null, strNew = null, strOld = null, var_string = XVar.Array(), var_type = null;
				if(XVar.Pack(include))
				{
					if(XVar.Pack(!(XVar)(include.KeyExists(field))))
					{
						continue;
					}
				}
				var_type = XVar.Clone(pSet.getFieldType((XVar)(field)));
				if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
				{
					continue;
				}
				if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(var_type))))
				{
					newValue = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(newValues[field]))), new XVar("yyyy-MM-dd HH:mm:ss")));
					oldValue = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(oldValues[field]))), new XVar("yyyy-MM-dd HH:mm:ss")));
				}
				else
				{
					newValue = XVar.Clone(newValues[field]);
					oldValue = XVar.Clone(oldValues[field]);
				}
				if(newValue == oldValue)
				{
					continue;
				}
				strOld = XVar.Clone(MVCFunctions.Concat(field, " [old]: ", this.formatValue((XVar)(var_type), (XVar)(oldValue))));
				strNew = XVar.Clone(MVCFunctions.Concat(field, " [new]: ", this.formatValue((XVar)(var_type), (XVar)(newValue))));
				var_string.InitAndSetArrayItem(MVCFunctions.Concat(strOld, "\r\n", strNew), null);
			}
			return MVCFunctions.implode(new XVar("\r\n"), (XVar)(strings));
		}
		public virtual XVar formatValue(dynamic _param_type, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
			{
				return "<binary value>";
			}
			else
			{
				value = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(value)));
				return this.getMaxLengthSubstr((XVar)(value));
			}

			return null;
		}
		public virtual XVar LogEdit(dynamic _param_str_table, dynamic _param_newvalues, dynamic _param_oldvalues, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic str_table = XVar.Clone(_param_str_table);
			dynamic newvalues = XVar.Clone(_param_newvalues);
			dynamic oldvalues = XVar.Clone(_param_oldvalues);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic retval = null, table = null;
			ProjectSettings pSet;
			retval = new XVar(true);
			table = XVar.Clone(str_table);
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(str_table)));
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strEdit), (XVar)(this.var_params), (XVar)(table), (XVar)(keys), (XVar)(newvalues), (XVar)(oldvalues)));
			}
			if(XVar.Pack(retval))
			{
				dynamic str = null, strFields = null;
				str = new XVar("");
				if(0 < MVCFunctions.count(keys))
				{
					str = MVCFunctions.Concat(str, this.strKeysHeader, "\r\n");
					foreach (KeyValuePair<XVar, dynamic> val in newvalues.GetEnumerator())
					{
						if(XVar.Pack(keys.KeyExists(val.Key)))
						{
							if(val.Value != oldvalues[val.Key])
							{
								str = MVCFunctions.Concat(str, val.Key, " [old]: ", oldvalues[val.Key], "\r\n");
								str = MVCFunctions.Concat(str, val.Key, " [new]: ", val.Value, "\r\n");
							}
							else
							{
								str = MVCFunctions.Concat(str, val.Key, " : ", val.Value, "\r\n");
							}
						}
					}
				}
				strFields = new XVar("");
				if(XVar.Pack(this.logValueEnable((XVar)(str_table))))
				{
					dynamic v = null;
					v = new XVar("");
					foreach (KeyValuePair<XVar, dynamic> val in newvalues.GetEnumerator())
					{
						dynamic newvalue = null, oldvalue = null, var_type = null;
						var_type = XVar.Clone(pSet.getFieldType((XVar)(val.Key)));
						if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
						{
							continue;
						}
						if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(var_type))))
						{
							newvalue = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(newvalues[val.Key]))), new XVar("yyyy-MM-dd HH:mm:ss")));
							oldvalue = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(oldvalues[val.Key]))), new XVar("yyyy-MM-dd HH:mm:ss")));
						}
						else
						{
							newvalue = XVar.Clone(newvalues[val.Key]);
							oldvalue = XVar.Clone(oldvalues[val.Key]);
						}
						if((XVar)(newvalue != oldvalue)  && (XVar)(!(XVar)(keys.KeyExists(val.Key))))
						{
							strFields = MVCFunctions.Concat(strFields, val.Key, " [old]: ");
							if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
							{
								v = new XVar("<binary value>");
							}
							else
							{
								v = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(oldvalue)));
								v = XVar.Clone(this.getMaxLengthSubstr((XVar)(v)));
							}
							strFields = MVCFunctions.Concat(strFields, v, "\r\n");
							strFields = MVCFunctions.Concat(strFields, val.Key, " [new]: ");
							if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
							{
								v = new XVar("<binary value>");
							}
							else
							{
								v = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(newvalue)));
								v = XVar.Clone(this.getMaxLengthSubstr((XVar)(v)));
							}
							strFields = MVCFunctions.Concat(strFields, v, "\r\n");
						}
					}
					v = new XVar("");
				}
				if(strFields != XVar.Pack(""))
				{
					str = MVCFunctions.Concat(str, this.strFieldsHeader, "\r\n", strFields);
				}
				this.insert((XVar)(MVCFunctions.now()), (XVar)(this.var_params[0]), (XVar)(this.var_params[1]), (XVar)(str_table), (XVar)(this.strEdit), (XVar)(str));
			}
			return retval;
		}
		public virtual XVar LogDelete(dynamic _param_str_table, dynamic _param_values, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic str_table = XVar.Clone(_param_str_table);
			dynamic values = XVar.Clone(_param_values);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic arr = null, retval = null, table = null;
			ProjectSettings pSet;
			retval = new XVar(true);
			table = XVar.Clone(str_table);
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(str_table)));
			arr = XVar.Clone(XVar.Array());
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strDelete), (XVar)(this.var_params), (XVar)(table), (XVar)(keys), (XVar)(values), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				dynamic str = null, strFields = null;
				str = new XVar("");
				if(0 < MVCFunctions.count(keys))
				{
					str = MVCFunctions.Concat(str, this.strKeysHeader, "\r\n");
					foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
					{
						str = MVCFunctions.Concat(str, val.Key, " : ", val.Value, "\r\n");
					}
				}
				strFields = new XVar("");
				if(XVar.Pack(this.logValueEnable((XVar)(str_table))))
				{
					dynamic v = null;
					v = new XVar("");
					foreach (KeyValuePair<XVar, dynamic> val in values.GetEnumerator())
					{
						if((XVar)(val.Value != XVar.Pack(""))  && (XVar)(!(XVar)(keys.KeyExists(val.Key))))
						{
							strFields = MVCFunctions.Concat(strFields, val.Key, " [old]: ");
							if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(pSet.getFieldType((XVar)(val.Key))))))
							{
								v = new XVar("<binary value>");
							}
							else
							{
								v = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(val.Value)));
								v = XVar.Clone(this.getMaxLengthSubstr((XVar)(v)));
							}
							strFields = MVCFunctions.Concat(strFields, v, "\r\n");
						}
					}
				}
				if(strFields != XVar.Pack(""))
				{
					str = MVCFunctions.Concat(str, this.strFieldsHeader, "\r\n", strFields);
				}
				this.insert((XVar)(MVCFunctions.now()), (XVar)(this.var_params[0]), (XVar)(this.var_params[1]), (XVar)(str_table), (XVar)(this.strDelete), (XVar)(str));
			}
			return retval;
		}
		public virtual XVar LogAddEvent(dynamic _param_message, dynamic _param_description = null, dynamic _param_stable = null)
		{
			#region default values
			if(_param_description as Object == null) _param_description = new XVar("");
			if(_param_stable as Object == null) _param_stable = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			dynamic description = XVar.Clone(_param_description);
			dynamic stable = XVar.Clone(_param_stable);
			#endregion

			dynamic arr = null, retval = null, table = null;
			retval = new XVar(true);
			table = XVar.Clone(stable);
			arr = XVar.Clone(XVar.Array());
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				dynamic keys = null, values = null;
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(message), (XVar)(this.var_params), (XVar)(table), (XVar)(keys), (XVar)(values), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				this.insert((XVar)(MVCFunctions.now()), (XVar)(this.var_params[0]), (XVar)(this.var_params[1]), (XVar)(stable), (XVar)(message), (XVar)(description));
			}
			return retval;
		}
		public virtual XVar LoginSuccessful()
		{
			if((XVar)(0 < this.attLogin)  && (XVar)(0 < this.timeLogin))
			{
				dynamic sql = null, where = null;
				where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("ip")), "=", this.connection.prepareString((XVar)(MVCFunctions.remoteAddr())), " AND ", this.connection.addFieldWrappers(new XVar("action")), "=", this.connection.prepareString((XVar)(this.strAccess))));
				sql = XVar.Clone(MVCFunctions.Concat("DELETE FROM ", this.connection.addTableWrappers((XVar)(this.logTableName)), " WHERE ", where));
				this.connection.exec((XVar)(sql));
			}

			return null;
		}
		public virtual XVar LoginUnsuccessful(dynamic _param_pUsername)
		{
			#region pass-by-value parameters
			dynamic pUsername = XVar.Clone(_param_pUsername);
			#endregion

			if((XVar)(0 < this.attLogin)  && (XVar)(0 < this.timeLogin))
			{
				this.insert((XVar)(MVCFunctions.now()), (XVar)(MVCFunctions.remoteAddr()), (XVar)(pUsername), new XVar(""), (XVar)(this.strAccess), new XVar(""));
			}

			return null;
		}
		public virtual XVar LoginAccess()
		{
			if((XVar)(0 < this.attLogin)  && (XVar)(0 < this.timeLogin))
			{
				dynamic data = XVar.Array(), firstAccess = null, i = null, orderBy = null, qResult = null, sql = null, where = null;
				where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("ip")), "=", this.connection.prepareString((XVar)(MVCFunctions.remoteAddr())), " AND ", this.connection.addFieldWrappers(new XVar("action")), "=", this.connection.prepareString(new XVar("access"))));
				orderBy = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), " asc"));
				sql = XVar.Clone(MVCFunctions.Concat("SELECT * FROM ", this.connection.addTableWrappers((XVar)(this.logTableName)), " WHERE ", where, " ORDER BY ", orderBy));
				qResult = XVar.Clone(this.connection.query((XVar)(sql)));
				i = new XVar(0);
				while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
				{
					if(MVCFunctions.secondsPassedFrom((XVar)(data["datetime"])) / 60 <= this.timeLogin)
					{
						if(i == XVar.Pack(0))
						{
							firstAccess = XVar.Clone(data["datetime"]);
						}
						i += 1;
					}
				}
				if(this.attLogin <= i)
				{
					return (XVar)Math.Ceiling((double)(this.timeLogin - MVCFunctions.secondsPassedFrom((XVar)(firstAccess)) / 60));
				}
			}
			return false;
		}
		public virtual XVar logValueEnable(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic auditSettings = XVar.Array(), tableSettings = XVar.Array();
			auditSettings = ProjectSettings.getSecurityValue(new XVar("auditAndLocking"));
			if(auditSettings["loggingMode"] == 0)
			{
				return null;
			}
			tableSettings = XVar.Clone(auditSettings["tables"][table]);
			return (XVar)(tableSettings)  && (XVar)(tableSettings["logFieldValues"]);
		}
		protected virtual XVar insert(dynamic _param_datetime, dynamic _param_ip, dynamic _param_user, dynamic _param_table, dynamic _param_action, dynamic _param_description)
		{
			#region pass-by-value parameters
			dynamic datetime = XVar.Clone(_param_datetime);
			dynamic ip = XVar.Clone(_param_ip);
			dynamic user = XVar.Clone(_param_user);
			dynamic table = XVar.Clone(_param_table);
			dynamic action = XVar.Clone(_param_action);
			dynamic description = XVar.Clone(_param_description);
			#endregion

			dynamic sql = null;
			sql = XVar.Clone(MVCFunctions.Concat("INSERT INTO ", this.connection.addTableWrappers((XVar)(this.logTableName)), " (", this.connection.addFieldWrappers(new XVar("datetime")), ",", this.connection.addFieldWrappers(new XVar("ip")), ",", this.connection.addFieldWrappers(new XVar("user")), ",", this.connection.addFieldWrappers(new XVar("table")), ",", this.connection.addFieldWrappers(new XVar("action")), ",", this.connection.addFieldWrappers(new XVar("description")), ") VALUES (", this.connection.addDateQuotes((XVar)(datetime)), ",", this.connection.prepareString((XVar)(ip)), ",", this.connection.prepareString((XVar)(user)), ",", this.connection.prepareString((XVar)(table)), ",", this.connection.prepareString((XVar)(action)), ",", this.connection.prepareString((XVar)(description)), ")"));
			return this.connection.exec((XVar)(sql));
		}
		protected virtual XVar getMaxLengthSubstr(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if((XVar)(this.maxFieldLength)  && (XVar)(this.maxFieldLength < MVCFunctions.strlen((XVar)(value))))
			{
				return MVCFunctions.runner_substr((XVar)(value), new XVar(0), (XVar)(this.maxFieldLength));
			}
			return value;
		}
	}
	public partial class AuditTrailFile : XClass
	{
		public dynamic logfile;
		public dynamic strLogin = XVar.Pack("login");
		public dynamic strFailLogin = XVar.Pack("failed login");
		public dynamic strLogout = XVar.Pack("logout");
		public dynamic strChPass = XVar.Pack("change password");
		public dynamic strAdd = XVar.Pack("add");
		public dynamic strEdit = XVar.Pack("edit");
		public dynamic strDelete = XVar.Pack("delete");
		public dynamic strAccess = XVar.Pack("access");
		public dynamic strKeysHeader = XVar.Pack("---Keys");
		public dynamic strFieldsHeader = XVar.Pack("---Fields");
		public dynamic columnDate = XVar.Pack("Date");
		public dynamic columnTime = XVar.Pack("Time");
		public dynamic columnIP = XVar.Pack("IP");
		public dynamic columnUser = XVar.Pack("User");
		public dynamic columnTable = XVar.Pack("Table");
		public dynamic columnAction = XVar.Pack("Action");
		public dynamic columnKey = XVar.Pack("Key field");
		public dynamic columnField = XVar.Pack("Field");
		public dynamic columnOldValue = XVar.Pack("Old value");
		public dynamic columnNewValue = XVar.Pack("New value");
		public dynamic var_params;
		public dynamic maxFieldLength;
		public AuditTrailFile()
		{
			dynamic userid = null;
			this.logfile = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("auditAndLocking"), new XVar("loggingFile")));
			userid = new XVar("");
			if(XVar.Pack(Security.getUserName()))
			{
				userid = XVar.Clone(Security.getUserName());
			}
			this.var_params = XVar.Clone(new XVar(0, MVCFunctions.remoteAddr(), 1, userid));
			this.maxFieldLength = XVar.Clone(GlobalVars.auditMaxFieldLength);
		}
		public virtual XVar logSecurity()
		{
			return ProjectSettings.getSecurityValue(new XVar("auditAndLocking"), new XVar("logSecurityActions"));
		}
		public virtual XVar LogLogin(dynamic _param_pUsername)
		{
			#region pass-by-value parameters
			dynamic pUsername = XVar.Clone(_param_pUsername);
			#endregion

			dynamic arr = null, retval = null, table = null;
			if(XVar.Pack(!(XVar)(this.logSecurity())))
			{
				return null;
			}
			retval = new XVar(true);
			table = XVar.Clone(Security.loginTable());
			this.var_params.InitAndSetArrayItem(pUsername, 1);
			arr = XVar.Clone(XVar.Array());
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strLogin), (XVar)(this.var_params), (XVar)(table), (XVar)(arr), (XVar)(arr), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				dynamic str = null;
				str = XVar.Clone(MVCFunctions.Concat(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("MMM dd,yyyy")), MVCFunctions.chr(new XVar(9)), CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("HH:mm:ss")), MVCFunctions.chr(new XVar(9)), this.var_params[0], MVCFunctions.chr(new XVar(9)), this.var_params[1], MVCFunctions.chr(new XVar(9)), table, MVCFunctions.chr(new XVar(9)), this.strLogin, "\r\n"));
				this.writeToLogFile((XVar)(str));
			}
			return retval;
		}
		public virtual XVar LogLoginFailed(dynamic _param_pUsername)
		{
			#region pass-by-value parameters
			dynamic pUsername = XVar.Clone(_param_pUsername);
			#endregion

			dynamic arr = null, retval = null, table = null;
			if(XVar.Pack(!(XVar)(this.logSecurity())))
			{
				return null;
			}
			retval = new XVar(true);
			table = XVar.Clone(Security.loginTable());
			this.var_params.InitAndSetArrayItem(pUsername, 1);
			arr = XVar.Clone(XVar.Array());
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strFailLogin), (XVar)(this.var_params), (XVar)(table), (XVar)(arr), (XVar)(arr), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				dynamic str = null;
				str = XVar.Clone(MVCFunctions.Concat(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("MMM dd,yyyy")), MVCFunctions.chr(new XVar(9)), CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("HH:mm:ss")), MVCFunctions.chr(new XVar(9)), this.var_params[0], MVCFunctions.chr(new XVar(9)), this.var_params[1], MVCFunctions.chr(new XVar(9)), table, MVCFunctions.chr(new XVar(9)), this.strFailLogin, "\r\n"));
				this.writeToLogFile((XVar)(str));
			}
			return retval;
		}
		public virtual XVar LogLogout()
		{
			if(XVar.Pack(!(XVar)(this.logSecurity())))
			{
				return null;
			}
			if(Security.getUserName() != "")
			{
				dynamic arr = null, retval = null, table = null;
				retval = new XVar(true);
				table = XVar.Clone(Security.loginTable());
				arr = XVar.Clone(XVar.Array());
				if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
				{
					retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strLogout), (XVar)(this.var_params), (XVar)(table), (XVar)(arr), (XVar)(arr), (XVar)(arr)));
				}
				if(XVar.Pack(retval))
				{
					dynamic str = null;
					str = XVar.Clone(MVCFunctions.Concat(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("MMM dd,yyyy")), MVCFunctions.chr(new XVar(9)), CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("HH:mm:ss")), MVCFunctions.chr(new XVar(9)), this.var_params[0], MVCFunctions.chr(new XVar(9)), this.var_params[1], MVCFunctions.chr(new XVar(9)), table, MVCFunctions.chr(new XVar(9)), this.strLogout, "\r\n"));
					this.writeToLogFile((XVar)(str));
				}
				return retval;
			}

			return null;
		}
		public virtual XVar LogChPassword(dynamic _param_username)
		{
			#region pass-by-value parameters
			dynamic username = XVar.Clone(_param_username);
			#endregion

			dynamic arr = null, retval = null, table = null;
			if(XVar.Pack(!(XVar)(this.logSecurity())))
			{
				return null;
			}
			retval = new XVar(true);
			table = XVar.Clone(Security.loginTable());
			arr = XVar.Clone(XVar.Array());
			this.var_params.InitAndSetArrayItem(username, 1);
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strChPass), (XVar)(this.var_params), (XVar)(table), (XVar)(arr), (XVar)(arr), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				dynamic str = null;
				str = XVar.Clone(MVCFunctions.Concat(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("MMM dd,yyyy")), MVCFunctions.chr(new XVar(9)), CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("HH:mm:ss")), MVCFunctions.chr(new XVar(9)), this.var_params[0], MVCFunctions.chr(new XVar(9)), this.var_params[1], MVCFunctions.chr(new XVar(9)), table, MVCFunctions.chr(new XVar(9)), this.strChPass, "\r\n"));
				this.writeToLogFile((XVar)(str));
			}
			return retval;
		}
		public virtual XVar LogAdd(dynamic _param_str_table, dynamic _param_values, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic str_table = XVar.Clone(_param_str_table);
			dynamic values = XVar.Clone(_param_values);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic arr = null, retval = null, table = null;
			ProjectSettings pSet;
			retval = new XVar(true);
			table = XVar.Clone(str_table);
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(str_table)));
			arr = XVar.Clone(XVar.Array());
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strAdd), (XVar)(this.var_params), (XVar)(table), (XVar)(keys), (XVar)(values), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				dynamic key = null, str = null, str_add = null;
				if(0 < MVCFunctions.count(keys))
				{
					key = new XVar("");
					foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
					{
						if(key != XVar.Pack(""))
						{
							key = MVCFunctions.Concat(key, ",");
						}
						key = MVCFunctions.Concat(key, val.Value);
					}
				}
				str = XVar.Clone(MVCFunctions.Concat(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("MMM dd,yyyy")), MVCFunctions.chr(new XVar(9)), CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("HH:mm:ss")), MVCFunctions.chr(new XVar(9)), this.var_params[0], MVCFunctions.chr(new XVar(9)), this.var_params[1], MVCFunctions.chr(new XVar(9)), table, MVCFunctions.chr(new XVar(9)), this.strAdd, MVCFunctions.chr(new XVar(9)), key));
				str_add = new XVar("");
				if(XVar.Pack(this.logValueEnable((XVar)(str_table))))
				{
					foreach (KeyValuePair<XVar, dynamic> val in values.GetEnumerator())
					{
						if((XVar)(val.Value != XVar.Pack(""))  && (XVar)(!(XVar)(keys.KeyExists(val.Key))))
						{
							dynamic v = null;
							v = new XVar("");
							if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(pSet.getFieldType((XVar)(val.Key))))))
							{
								v = XVar.Clone(MVCFunctions.Concat("<binary value>", "\r\n"));
							}
							else
							{
								v = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(val.Value)));
								v = XVar.Clone(this.getMaxLengthSubstr((XVar)(v)));
							}
							str_add = MVCFunctions.Concat(str_add, str, MVCFunctions.chr(new XVar(9)), val.Key, MVCFunctions.chr(new XVar(9)), MVCFunctions.chr(new XVar(9)), v, "\r\n");
						}
					}
				}
				else
				{
					str_add = MVCFunctions.Concat(str_add, str, "\r\n");
				}
				this.writeToLogFile((XVar)(str_add));
			}
			return retval;
		}
		public virtual XVar LogEdit(dynamic _param_str_table, dynamic _param_newvalues, dynamic _param_oldvalues, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic str_table = XVar.Clone(_param_str_table);
			dynamic newvalues = XVar.Clone(_param_newvalues);
			dynamic oldvalues = XVar.Clone(_param_oldvalues);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic retval = null, table = null;
			ProjectSettings pSet;
			retval = new XVar(true);
			table = XVar.Clone(str_table);
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(str_table)));
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strEdit), (XVar)(this.var_params), (XVar)(table), (XVar)(keys), (XVar)(newvalues), (XVar)(oldvalues)));
			}
			if(XVar.Pack(retval))
			{
				dynamic key = null, putsValue = null, str = null, str_add = null;
				if(0 < MVCFunctions.count(keys))
				{
					key = new XVar("");
					foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
					{
						if(key != XVar.Pack(""))
						{
							key = MVCFunctions.Concat(key, ",");
						}
						key = MVCFunctions.Concat(key, val.Value);
					}
				}
				str = XVar.Clone(MVCFunctions.Concat(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("MMM dd,yyyy")), MVCFunctions.chr(new XVar(9)), CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("HH:mm:ss")), MVCFunctions.chr(new XVar(9)), this.var_params[0], MVCFunctions.chr(new XVar(9)), this.var_params[1], MVCFunctions.chr(new XVar(9)), table, MVCFunctions.chr(new XVar(9)), this.strEdit, MVCFunctions.chr(new XVar(9)), key));
				putsValue = new XVar(true);
				str_add = new XVar("");
				if(XVar.Pack(this.logValueEnable((XVar)(str_table))))
				{
					foreach (KeyValuePair<XVar, dynamic> val in newvalues.GetEnumerator())
					{
						dynamic newvalue = null, oldvalue = null, var_type = null;
						var_type = XVar.Clone(pSet.getFieldType((XVar)(val.Key)));
						if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
						{
							continue;
						}
						if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(var_type))))
						{
							newvalue = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(newvalues[val.Key]))), new XVar("yyyy-MM-dd HH:mm:ss")));
							oldvalue = XVar.Clone(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(oldvalues[val.Key]))), new XVar("yyyy-MM-dd HH:mm:ss")));
						}
						else
						{
							newvalue = XVar.Clone(newvalues[val.Key]);
							oldvalue = XVar.Clone(oldvalues[val.Key]);
						}
						if(newvalue != oldvalue)
						{
							dynamic v1 = null, v2 = null;
							v1 = new XVar("");
							if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
							{
								v1 = new XVar("<binary value>");
							}
							else
							{
								dynamic v = null;
								v1 = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(oldvalue)));
								v = XVar.Clone(this.getMaxLengthSubstr((XVar)(v)));
							}
							v2 = new XVar("");
							if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(var_type))))
							{
								v2 = new XVar("<binary value>");
							}
							else
							{
								v2 = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(newvalue)));
								v2 = XVar.Clone(this.getMaxLengthSubstr((XVar)(v2)));
							}
							str_add = MVCFunctions.Concat(str_add, str, MVCFunctions.chr(new XVar(9)), val.Key, MVCFunctions.chr(new XVar(9)), v1, MVCFunctions.chr(new XVar(9)), v2, "\r\n");
						}
					}
				}
				else
				{
					str_add = MVCFunctions.Concat(str_add, str, "\r\n");
				}
				this.writeToLogFile((XVar)(str_add));
			}
			return retval;
		}
		public virtual XVar LogDelete(dynamic _param_str_table, dynamic _param_values, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic str_table = XVar.Clone(_param_str_table);
			dynamic values = XVar.Clone(_param_values);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic arr = null, retval = null, table = null;
			ProjectSettings pSet;
			retval = new XVar(true);
			table = XVar.Clone(str_table);
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(str_table)));
			arr = XVar.Clone(XVar.Array());
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(this.strDelete), (XVar)(this.var_params), (XVar)(table), (XVar)(keys), (XVar)(values), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				dynamic key = null, str = null, str_add = null;
				if(0 < MVCFunctions.count(keys))
				{
					key = new XVar("");
					foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
					{
						if(key != XVar.Pack(""))
						{
							key = MVCFunctions.Concat(key, ",");
						}
						key = MVCFunctions.Concat(key, val.Value);
					}
				}
				str = XVar.Clone(MVCFunctions.Concat(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("MMM dd,yyyy")), MVCFunctions.chr(new XVar(9)), CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("HH:mm:ss")), MVCFunctions.chr(new XVar(9)), this.var_params[0], MVCFunctions.chr(new XVar(9)), this.var_params[1], MVCFunctions.chr(new XVar(9)), table, MVCFunctions.chr(new XVar(9)), this.strDelete, MVCFunctions.chr(new XVar(9)), key));
				str_add = new XVar("");
				if(XVar.Pack(this.logValueEnable((XVar)(str_table))))
				{
					foreach (KeyValuePair<XVar, dynamic> val in values.GetEnumerator())
					{
						dynamic v = null;
						v = new XVar("");
						if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(pSet.getFieldType((XVar)(val.Key))))))
						{
							v = new XVar("<binary value>");
						}
						else
						{
							v = XVar.Clone(MVCFunctions.str_replace((XVar)(new XVar(0, "\r\n", 1, "\n", 2, "\t")), new XVar(" "), (XVar)(val.Value)));
							v = XVar.Clone(this.getMaxLengthSubstr((XVar)(v)));
						}
						str_add = MVCFunctions.Concat(str_add, str, MVCFunctions.chr(new XVar(9)), val.Key, MVCFunctions.chr(new XVar(9)), v, "\r\n");
					}
				}
				else
				{
					str_add = XVar.Clone(MVCFunctions.Concat(str, "\r\n"));
				}
				this.writeToLogFile((XVar)(str_add));
			}
			return retval;
		}
		public virtual XVar writeToLogFile(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			dynamic fsize = null, fullname = null, logfileExt = null, logfileName = null, p = null, str_to_append = null, tn = null;
			p = XVar.Clone(MVCFunctions.strrpos((XVar)(this.logfile), new XVar(".")));
			logfileName = XVar.Clone(MVCFunctions.runner_substr((XVar)(this.logfile), new XVar(0), (XVar)(p)));
			logfileExt = XVar.Clone(MVCFunctions.runner_substr((XVar)(this.logfile), (XVar)(p + 1), (XVar)(MVCFunctions.strlen((XVar)(this.logfile)) - 1)));
			tn = XVar.Clone(MVCFunctions.Concat(logfileName, "_", CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("yyyyMMdd")), ".", logfileExt));
			fullname = XVar.Clone(MVCFunctions.getabspath((XVar)(tn)));
			fsize = new XVar(0);
			if(XVar.Pack(MVCFunctions.file_exists((XVar)(fullname))))
			{
				fsize = XVar.Clone(MVCFunctions.filesize((XVar)(fullname)));
			}
			str_to_append = new XVar("");
			if(XVar.Pack(!(XVar)(fsize)))
			{
				str_to_append = XVar.Clone(MVCFunctions.Concat(this.columnDate, MVCFunctions.chr(new XVar(9)), this.columnTime, MVCFunctions.chr(new XVar(9)), this.columnIP, MVCFunctions.chr(new XVar(9)), this.columnUser, MVCFunctions.chr(new XVar(9)), this.columnTable, MVCFunctions.chr(new XVar(9)), this.columnAction, MVCFunctions.chr(new XVar(9)), this.columnKey, MVCFunctions.chr(new XVar(9)), this.columnField, MVCFunctions.chr(new XVar(9)), this.columnOldValue, MVCFunctions.chr(new XVar(9)), this.columnNewValue, "\r\n"));
			}
			str_to_append = MVCFunctions.Concat(str_to_append, str);
			MVCFunctions.append_to_file((XVar)(fullname), (XVar)(str_to_append));

			return null;
		}
		public virtual XVar LogAddEvent(dynamic _param_message, dynamic _param_description = null, dynamic _param_str_table = null)
		{
			#region default values
			if(_param_description as Object == null) _param_description = new XVar("");
			if(_param_str_table as Object == null) _param_str_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic message = XVar.Clone(_param_message);
			dynamic description = XVar.Clone(_param_description);
			dynamic str_table = XVar.Clone(_param_str_table);
			#endregion

			dynamic arr = null, retval = null, table = null;
			retval = new XVar(true);
			table = XVar.Clone(str_table);
			arr = XVar.Clone(XVar.Array());
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("OnAuditLog"))))
			{
				retval = XVar.Clone(GlobalVars.globalEvents.OnAuditLog((XVar)(message), (XVar)(this.var_params), (XVar)(table), (XVar)(arr), (XVar)(arr), (XVar)(arr)));
			}
			if(XVar.Pack(retval))
			{
				dynamic str = null, var_params = XVar.Array();
				str = XVar.Clone(MVCFunctions.Concat(CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("MMM dd,yyyy")), MVCFunctions.chr(new XVar(9)), CommonFunctions.format_datetime_custom((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar("HH:mm:ss")), MVCFunctions.chr(new XVar(9)), var_params[0], MVCFunctions.chr(new XVar(9)), var_params[1], MVCFunctions.chr(new XVar(9)), table, MVCFunctions.chr(new XVar(9)), message, MVCFunctions.chr(new XVar(9)), description, "\r\n"));
				this.writeToLogFile((XVar)(str));
			}
			return retval;
		}
		public virtual XVar LoginAccess()
		{
			return false;
		}
		public virtual XVar LoginSuccessful()
		{
			return true;
		}
		public virtual XVar LoginUnsuccessful(dynamic _param_pUsername)
		{
			#region pass-by-value parameters
			dynamic pUsername = XVar.Clone(_param_pUsername);
			#endregion

			return true;
		}
		public virtual XVar logValueEnable(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic auditSettings = XVar.Array(), tableSettings = XVar.Array();
			auditSettings = ProjectSettings.getSecurityValue(new XVar("auditAndLocking"));
			if(auditSettings["loggingMode"] == 0)
			{
				return null;
			}
			tableSettings = XVar.Clone(auditSettings["tables"][table]);
			return (XVar)(tableSettings)  && (XVar)(tableSettings["logFieldValues"]);
		}
		protected virtual XVar getMaxLengthSubstr(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if((XVar)(this.maxFieldLength)  && (XVar)(this.maxFieldLength < MVCFunctions.strlen((XVar)(value))))
			{
				return MVCFunctions.runner_substr((XVar)(value), new XVar(0), (XVar)(this.maxFieldLength));
			}
			return value;
		}
	}
}
