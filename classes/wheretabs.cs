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
	public partial class WhereTabs : XClass
	{
		protected static XVar getGridTabs(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic tableName = null;
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(table)))
			{
				tableName = XVar.Clone(GlobalVars.strTableName);
			}
			else
			{
				tableName = XVar.Clone(table);
			}
			if(XVar.Equals(XVar.Pack(CommonFunctions.GetEntityType((XVar)(tableName))), XVar.Pack("")))
			{
				return false;
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(tableName)));
			return pSet._tableData["whereTabs"];
		}
		protected static XVar getGridTab(dynamic _param_table, dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic gridTabs = XVar.Array();
			gridTabs = WhereTabs.getGridTabs((XVar)(table));
			if(XVar.Equals(XVar.Pack(gridTabs), XVar.Pack(false)))
			{
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> tab in gridTabs.GetEnumerator())
			{
				if(tab.Value["id"] == id)
				{
					return tab.Value;
				}
			}
			return false;
		}
		public static XVar addTab(dynamic _param_table, dynamic _param_where, dynamic _param_title, dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic where = XVar.Clone(_param_where);
			dynamic title = XVar.Clone(_param_title);
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic gridTabs = XVar.Array();
			gridTabs = WhereTabs.getGridTabs((XVar)(table));
			if(XVar.Equals(XVar.Pack(gridTabs), XVar.Pack(false)))
			{
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> tab in gridTabs.GetEnumerator())
			{
				if(tab.Value["id"] == id)
				{
					return false;
				}
			}
			gridTabs.InitAndSetArrayItem(new XVar("id", id, "title", new XVar("type", Constants.mlTypeText, "text", title), "where", where, "showCount", false, "hideEmpty", false), null);
			return true;
		}
		public static XVar deleteTab(dynamic _param_table, dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic gridTabs = XVar.Array();
			gridTabs = WhereTabs.getGridTabs((XVar)(table));
			if(XVar.Equals(XVar.Pack(gridTabs), XVar.Pack(false)))
			{
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> tab in gridTabs.GetEnumerator())
			{
				if(tab.Value["id"] == id)
				{
					gridTabs.Remove(tab.Key);
					break;
				}
			}
			return true;
		}
		public static XVar setTabTitle(dynamic _param_table, dynamic _param_id, dynamic _param_title)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic id = XVar.Clone(_param_id);
			dynamic title = XVar.Clone(_param_title);
			#endregion

			dynamic tab = XVar.Array();
			tab = WhereTabs.getGridTab((XVar)(table), (XVar)(id));
			if(XVar.Pack(tab))
			{
				tab.InitAndSetArrayItem(new XVar("type", Constants.mlTypeText, "text", title), "title");
				return true;
			}
			return false;
		}
		public static XVar setTabWhere(dynamic _param_table, dynamic _param_id, dynamic _param_where)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic id = XVar.Clone(_param_id);
			dynamic where = XVar.Clone(_param_where);
			#endregion

			dynamic tab = XVar.Array();
			tab = WhereTabs.getGridTab((XVar)(table), (XVar)(id));
			if(XVar.Pack(tab))
			{
				tab.InitAndSetArrayItem(where, "where");
				return true;
			}
			return false;
		}
		public static XVar setTabShowCount(dynamic _param_table, dynamic _param_id, dynamic _param_showCount)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic id = XVar.Clone(_param_id);
			dynamic showCount = XVar.Clone(_param_showCount);
			#endregion

			dynamic tab = XVar.Array();
			tab = WhereTabs.getGridTab((XVar)(table), (XVar)(id));
			if(XVar.Pack(tab))
			{
				tab.InitAndSetArrayItem((XVar.Pack(showCount) ? XVar.Pack(1) : XVar.Pack(0)), "showCount");
				return true;
			}
			return false;
		}
		public static XVar setTabHideEmpty(dynamic _param_table, dynamic _param_id, dynamic _param_hideEmpty)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic id = XVar.Clone(_param_id);
			dynamic hideEmpty = XVar.Clone(_param_hideEmpty);
			#endregion

			dynamic tab = XVar.Array();
			tab = WhereTabs.getGridTab((XVar)(table), (XVar)(id));
			if(XVar.Pack(tab))
			{
				tab.InitAndSetArrayItem((XVar.Pack(hideEmpty) ? XVar.Pack(1) : XVar.Pack(0)), "hideEmpty");
				return true;
			}
			return false;
		}
	}
}
