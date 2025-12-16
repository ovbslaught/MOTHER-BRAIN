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
	public partial class oLocking : XClass
	{
		public dynamic lockTableName;
		public dynamic ConfirmTime = XVar.Pack(15);
		public dynamic UnlockTime = XVar.Pack(30);
		public dynamic ConfirmAdmin;
		public dynamic ConfirmUser;
		public dynamic LockAdmin;
		public dynamic LockUser;
		public dynamic UserID;
		protected dynamic connection;
		public oLocking()
		{
			this.lockTableName = XVar.Clone(ProjectSettings.getSecurityValue(new XVar("auditAndLocking"), new XVar("lockingTable"), new XVar("table")));
			this.ConfirmAdmin = XVar.Clone(CommonFunctions.mlang_message(new XVar("LOCK_ADMIN_ABORTED")));
			this.ConfirmUser = XVar.Clone(CommonFunctions.mlang_message(new XVar("LOCK_TIMED_OUT")));
			this.LockAdmin = XVar.Clone(CommonFunctions.mlang_message(new XVar("LOCK_RECORD_EDITED_BY")));
			this.LockUser = XVar.Clone(CommonFunctions.mlang_message(new XVar("LOCK_RECORD_EDITED")));
			this.connection = XVar.Clone(GlobalVars.cman.getForLocking());
			this.UserID = XVar.Clone(Security.getUserName());
		}
		public virtual XVar LockRecord(dynamic _param_strtable, dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic strtable = XVar.Clone(_param_strtable);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic arrDelete = XVar.Array(), data = XVar.Array(), qResult = null, sdate = null, skeys = null, where = null;
			skeys = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(skeys))))
				{
					skeys = MVCFunctions.Concat(skeys, "&");
				}
				skeys = MVCFunctions.Concat(skeys, MVCFunctions.RawUrlEncode((XVar)(val.Value)));
			}
			sdate = XVar.Clone(MVCFunctions.now());
			this.insert((XVar)(strtable), (XVar)(sdate), (XVar)(sdate), (XVar)(skeys), (XVar)(MVCFunctions.session_id()), (XVar)(this.UserID), new XVar(1));
			arrDelete = XVar.Clone(XVar.Array());
			where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("table")), "=", this.connection.prepareString((XVar)(strtable)), " AND ", this.connection.addFieldWrappers(new XVar("keys")), "=", this.connection.prepareString((XVar)(skeys)), " AND ", this.connection.addFieldWrappers(new XVar("action")), "=1"));
			qResult = XVar.Clone(this.query((XVar)(where), (XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), " asc"))));
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				if(this.UnlockTime < MVCFunctions.secondsPassedFrom((XVar)(data["confirmdatetime"])))
				{
					arrDelete.InitAndSetArrayItem(data["id"], null);
				}
				else
				{
					foreach (KeyValuePair<XVar, dynamic> val in arrDelete.GetEnumerator())
					{
						this.delete((XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), "=", val.Value)));
					}
					if(data["sessionid"] == MVCFunctions.session_id())
					{
						return true;
					}
					else
					{
						where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("sessionid")), "=", this.connection.prepareString((XVar)(MVCFunctions.session_id())), " AND ", this.connection.addFieldWrappers(new XVar("action")), "=1", " AND ", this.connection.addFieldWrappers(new XVar("table")), "=", this.connection.prepareString((XVar)(strtable)), " AND ", this.connection.addFieldWrappers(new XVar("keys")), "=", this.connection.prepareString((XVar)(skeys))));
						this.delete((XVar)(where));
						return false;
					}
				}
			}
			return false;
		}
		public virtual XVar UnlockRecord(dynamic _param_strtable, dynamic _param_keys, dynamic _param_sid)
		{
			#region pass-by-value parameters
			dynamic strtable = XVar.Clone(_param_strtable);
			dynamic keys = XVar.Clone(_param_keys);
			dynamic sid = XVar.Clone(_param_sid);
			#endregion

			dynamic skeys = null, where = null;
			if(sid == XVar.Pack(""))
			{
				sid = XVar.Clone(MVCFunctions.session_id());
			}
			skeys = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(skeys))))
				{
					skeys = MVCFunctions.Concat(skeys, "&");
				}
				skeys = MVCFunctions.Concat(skeys, MVCFunctions.RawUrlEncode((XVar)(val.Value)));
			}
			where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("sessionid")), "=", this.connection.prepareString((XVar)(sid)), " AND ", this.connection.addFieldWrappers(new XVar("action")), "=1 AND ", this.connection.addFieldWrappers(new XVar("table")), "=", this.connection.prepareString((XVar)(strtable)), " AND ", this.connection.addFieldWrappers(new XVar("keys")), "=", this.connection.prepareString((XVar)(skeys))));
			this.delete((XVar)(where));

			return null;
		}
		public virtual XVar ConfirmLock(dynamic _param_strtable, dynamic _param_keys, ref dynamic message)
		{
			#region pass-by-value parameters
			dynamic strtable = XVar.Clone(_param_strtable);
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic data = XVar.Array(), myfound = null, newdate = null, newid = null, olddate = null, oldid = null, otherfound = null, qResult = null, sdate = null, skeys = null, tempfound = null, where = null;
			skeys = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(skeys))))
				{
					skeys = MVCFunctions.Concat(skeys, "&");
				}
				skeys = MVCFunctions.Concat(skeys, MVCFunctions.RawUrlEncode((XVar)(val.Value)));
			}
			sdate = XVar.Clone(MVCFunctions.now());
			this.insert((XVar)(strtable), (XVar)(sdate), (XVar)(sdate), (XVar)(skeys), (XVar)(MVCFunctions.session_id()), (XVar)(this.UserID), new XVar(1));
			where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("table")), "=", this.connection.prepareString((XVar)(strtable)), " AND ", this.connection.addFieldWrappers(new XVar("keys")), "=", this.connection.prepareString((XVar)(skeys)), " AND ", this.connection.addFieldWrappers(new XVar("action")), "=1"));
			qResult = XVar.Clone(this.query((XVar)(where), (XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), " asc"))));
			myfound = new XVar(0);
			newid = new XVar(0);
			oldid = new XVar(0);
			newdate = new XVar("");
			olddate = new XVar("");
			otherfound = new XVar(0);
			tempfound = new XVar(0);
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				if(data["sessionid"] == MVCFunctions.session_id())
				{
					oldid = XVar.Clone(newid);
					newid = XVar.Clone(data["id"]);
					newdate = XVar.Clone(data["confirmdatetime"]);
					olddate = XVar.Clone(newdate);
					myfound++;
					otherfound = XVar.Clone(tempfound);
					tempfound = new XVar(0);
					continue;
				}
				tempfound++;
			}
			if((XVar)(1 < myfound)  && (XVar)(!(XVar)(otherfound)))
			{
				this.update((XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("confirmdatetime")), "=", this.connection.addDateQuotes((XVar)(MVCFunctions.now())))), (XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), "=", oldid)));
				this.delete((XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), "=", newid)));
				return true;
			}
			else
			{
				if((XVar)(1 < myfound)  && (XVar)(otherfound))
				{
					if(this.UnlockTime - 5 < MVCFunctions.secondsPassedFrom((XVar)(olddate)))
					{
						this.UnlockRecord((XVar)(strtable), (XVar)(keys), (XVar)(MVCFunctions.session_id()));
						message = XVar.Clone(this.ConfirmUser);
						return false;
					}
					else
					{
						this.update((XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("confirmdatetime")), "=", this.connection.addDateQuotes((XVar)(MVCFunctions.now())))), (XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), "=", oldid)));
						this.delete((XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), "=", newid)));
						return true;
					}
				}
				else
				{
					this.UnlockRecord((XVar)(strtable), (XVar)(keys), (XVar)(MVCFunctions.session_id()));
					where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("table")), "=", this.connection.prepareString((XVar)(strtable)), " AND ", this.connection.addFieldWrappers(new XVar("keys")), "=", this.connection.prepareString((XVar)(skeys)), " AND ", this.connection.addFieldWrappers(new XVar("sessionid")), "<>'", MVCFunctions.session_id(), "' AND ", this.connection.addFieldWrappers(new XVar("action")), "=2"));
					if(XVar.Pack(data = XVar.Clone(this.query((XVar)(where), (XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), " asc"))).fetchAssoc())))
					{
						message = XVar.Clone(MVCFunctions.mysprintf((XVar)(this.ConfirmAdmin), (XVar)(new XVar(0, data["userid"]))));
					}
					else
					{
						message = XVar.Clone(this.ConfirmUser);
					}
					where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("action")), "=2 AND ", this.connection.addFieldWrappers(new XVar("table")), "=", this.connection.prepareString((XVar)(strtable)), " AND ", this.connection.addFieldWrappers(new XVar("keys")), "=", this.connection.prepareString((XVar)(skeys))));
					this.delete((XVar)(where));
					return false;
				}
			}

			return null;
		}
		public virtual XVar GetLockInfo(dynamic _param_strtable, dynamic _param_keys, dynamic _param_links, dynamic _param_pageid)
		{
			#region pass-by-value parameters
			dynamic strtable = XVar.Clone(_param_strtable);
			dynamic keys = XVar.Clone(_param_keys);
			dynamic links = XVar.Clone(_param_links);
			dynamic pageid = XVar.Clone(_param_pageid);
			#endregion

			dynamic data = XVar.Array(), page = null, qResult = null, skeys = null, where = null;
			page = XVar.Clone(MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(strtable))), new XVar("edit")));
			skeys = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(skeys))))
				{
					skeys = MVCFunctions.Concat(skeys, "&");
				}
				skeys = MVCFunctions.Concat(skeys, MVCFunctions.RawUrlEncode((XVar)(val.Value)));
			}
			where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("table")), "=", this.connection.prepareString((XVar)(strtable)), " AND ", this.connection.addFieldWrappers(new XVar("keys")), "=", this.connection.prepareString((XVar)(skeys)), " AND ", this.connection.addFieldWrappers(new XVar("sessionid")), "<>'", MVCFunctions.session_id(), "' AND ", this.connection.addFieldWrappers(new XVar("action")), "=1"));
			qResult = XVar.Clone(this.query((XVar)(where), (XVar)(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("id")), " asc"))));
			if(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				dynamic str = null;
				str = XVar.Clone(MVCFunctions.mysprintf((XVar)(this.LockAdmin), (XVar)(new XVar(0, data["userid"], 1, (XVar)Math.Round((double)(MVCFunctions.secondsPassedFrom((XVar)(data["startdatetime"])) / 60), 2)))));
				if(XVar.Pack(links))
				{
					str = MVCFunctions.Concat(str, "<a class=\"unlock\" href=\"#\">", CommonFunctions.mlang_message(new XVar("LOCK_UNLOCK")), "</a>");
					str = MVCFunctions.Concat(str, "<a class=\"edit\" href=\"#\">", CommonFunctions.mlang_message(new XVar("LOCK_EDIT")), "</a>");
				}
				return str;
			}
			return "";
		}
		public virtual XVar UnlockAdmin(dynamic _param_strtable, dynamic _param_keys, dynamic _param_startEdit)
		{
			#region pass-by-value parameters
			dynamic strtable = XVar.Clone(_param_strtable);
			dynamic keys = XVar.Clone(_param_keys);
			dynamic startEdit = XVar.Clone(_param_startEdit);
			#endregion

			dynamic sdate = null, skeys = null, where = null;
			skeys = new XVar("");
			foreach (KeyValuePair<XVar, dynamic> val in keys.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(skeys))))
				{
					skeys = MVCFunctions.Concat(skeys, "&");
				}
				skeys = MVCFunctions.Concat(skeys, MVCFunctions.RawUrlEncode((XVar)(val.Value)));
			}
			sdate = XVar.Clone(MVCFunctions.now());
			if(XVar.Pack(startEdit))
			{
				this.insert((XVar)(strtable), (XVar)(sdate), (XVar)(sdate), (XVar)(skeys), (XVar)(MVCFunctions.session_id()), (XVar)(this.UserID), new XVar(1));
			}
			where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("table")), "=", this.connection.prepareString((XVar)(strtable)), " AND ", this.connection.addFieldWrappers(new XVar("keys")), "=", this.connection.prepareString((XVar)(skeys)), " AND ", this.connection.addFieldWrappers(new XVar("action")), "=1 AND ", this.connection.addFieldWrappers(new XVar("sessionid")), "<>", this.connection.prepareString((XVar)(MVCFunctions.session_id()))));
			this.delete((XVar)(where));
			where = XVar.Clone(MVCFunctions.Concat(this.connection.addFieldWrappers(new XVar("startdatetime")), "<", this.connection.addDateQuotes((XVar)(CommonFunctions.format_datetime_custom((XVar)(CommonFunctions.adddays((XVar)(MVCFunctions.db2time((XVar)(MVCFunctions.now()))), new XVar(-2))), new XVar("yyyy-MM-dd HH:mm:ss")))), " AND ", this.connection.addFieldWrappers(new XVar("action")), "=2"));
			this.delete((XVar)(where));
			this.insert((XVar)(strtable), (XVar)(sdate), (XVar)(sdate), (XVar)(skeys), (XVar)(MVCFunctions.session_id()), (XVar)(this.UserID), new XVar(2));

			return null;
		}
		public virtual XVar isLocked(dynamic _param_table, dynamic _param_keys, dynamic _param_action)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic keys = XVar.Clone(_param_keys);
			dynamic action = XVar.Clone(_param_action);
			#endregion

			dynamic lockSQL = null, lockSet = XVar.Array(), lockWhere = null;
			lockSQL = XVar.Clone(MVCFunctions.Concat("select count(*) from ", this.connection.addTableWrappers((XVar)(this.lockTableName)), " where ", this.connection.addFieldWrappers(new XVar("keys")), "=", this.connection.prepareString((XVar)(lockWhere)), " AND ", this.connection.addFieldWrappers(new XVar("table")), "=", this.connection.prepareString((XVar)(table)), " AND ", this.connection.addFieldWrappers(new XVar("action")), "=", action));
			lockSet = XVar.Clone(this.connection.query((XVar)(lockSQL)).fetchNumeric());
			return 0 < lockSet[0];
		}
		protected virtual XVar insert(dynamic _param_table, dynamic _param_startdatetime, dynamic _param_confirmdatetime, dynamic _param_keys, dynamic _param_sessionid, dynamic _param_userid, dynamic _param_action)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic startdatetime = XVar.Clone(_param_startdatetime);
			dynamic confirmdatetime = XVar.Clone(_param_confirmdatetime);
			dynamic keys = XVar.Clone(_param_keys);
			dynamic sessionid = XVar.Clone(_param_sessionid);
			dynamic userid = XVar.Clone(_param_userid);
			dynamic action = XVar.Clone(_param_action);
			#endregion

			dynamic sql = null;
			sql = XVar.Clone(MVCFunctions.Concat("INSERT INTO ", this.connection.addTableWrappers((XVar)(this.lockTableName)), " (", this.connection.addFieldWrappers(new XVar("table")), ",", this.connection.addFieldWrappers(new XVar("startdatetime")), ",", this.connection.addFieldWrappers(new XVar("confirmdatetime")), ",", this.connection.addFieldWrappers(new XVar("keys")), ",", this.connection.addFieldWrappers(new XVar("sessionid")), ",", this.connection.addFieldWrappers(new XVar("userid")), ",", this.connection.addFieldWrappers(new XVar("action")), ") VALUES (", this.connection.prepareString((XVar)(table)), ",", this.connection.addDateQuotes((XVar)(startdatetime)), ",", this.connection.addDateQuotes((XVar)(confirmdatetime)), ",", this.connection.prepareString((XVar)(keys)), ",", this.connection.prepareString((XVar)(sessionid)), ",", this.connection.prepareString((XVar)(this.UserID)), ",", action, ")"));
			return this.connection.exec((XVar)(sql));
		}
		protected virtual dynamic query(dynamic _param_where, dynamic _param_orderBy)
		{
			#region pass-by-value parameters
			dynamic where = XVar.Clone(_param_where);
			dynamic orderBy = XVar.Clone(_param_orderBy);
			#endregion

			dynamic sql = null;
			if(XVar.Pack(!(XVar)(where)))
			{
				return null;
			}
			sql = XVar.Clone(MVCFunctions.Concat("SELECT * FROM ", this.connection.addTableWrappers((XVar)(this.lockTableName)), " WHERE ", where, " ORDER BY ", orderBy));
			return this.connection.query((XVar)(sql));
		}
		protected virtual XVar delete(dynamic _param_where)
		{
			#region pass-by-value parameters
			dynamic where = XVar.Clone(_param_where);
			#endregion

			dynamic sql = null;
			if(XVar.Pack(!(XVar)(where)))
			{
				return null;
			}
			sql = XVar.Clone(MVCFunctions.Concat("DELETE FROM ", this.connection.addTableWrappers((XVar)(this.lockTableName)), " WHERE ", where));
			this.connection.exec((XVar)(sql));

			return null;
		}
		protected virtual XVar update(dynamic _param_values, dynamic _param_where)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			dynamic where = XVar.Clone(_param_where);
			#endregion

			dynamic sql = null;
			if((XVar)(!(XVar)(where))  || (XVar)(!(XVar)(values)))
			{
				return null;
			}
			sql = XVar.Clone(MVCFunctions.Concat("UPDATE ", this.connection.addTableWrappers((XVar)(this.lockTableName)), " SET ", values, " WHERE ", where));
			this.connection.exec((XVar)(sql));

			return null;
		}
	}
}
