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
	public partial class RunnerMenu : XClass
	{
		protected dynamic root;
		protected dynamic _name;
		protected dynamic _id;
		protected dynamic activeItem = XVar.Pack(null);
		public RunnerMenu(dynamic _param_id, dynamic _param_name, dynamic _param_root)
		{
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			dynamic name = XVar.Clone(_param_name);
			dynamic root = XVar.Clone(_param_root);
			#endregion

			this._id = XVar.Clone(id);
			this._name = XVar.Clone(name);
			this.root = XVar.Clone(root);
		}
		public virtual XVar name()
		{
			return this._name;
		}
		public virtual XVar id()
		{
			return this._id;
		}
		public static XVar getMenuObject(dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic menuInfo = XVar.Array(), menuMap = null, menuObj = null, nullParent = null, rootElement = null, rootInfoArr = null;
			if(XVar.Pack(GlobalVars.menuCache[id]))
			{
				return GlobalVars.menuCache[id];
			}
			menuInfo = XVar.Clone(MVCFunctions.loadMenu((XVar)(id)));
			nullParent = new XVar(null);
			rootInfoArr = XVar.Clone(new XVar("id", 0, "href", ""));
			menuMap = XVar.Clone(XVar.Array());
			rootElement = XVar.Clone(new MenuItem((XVar)(menuInfo["root"]), (XVar)(menuMap), (XVar)(id)));
			menuObj = XVar.Clone(new RunnerMenu((XVar)(menuInfo["id"]), (XVar)(menuInfo["name"]), (XVar)(rootElement)));
			GlobalVars.menuCache.InitAndSetArrayItem(menuObj, id);
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("ModifyMenu"))))
			{
				GlobalVars.globalEvents.ModifyMenu((XVar)(menuObj));
			}
			return menuObj;
		}
		public virtual XVar getRoot()
		{
			return this.root;
		}
		public virtual XVar findActiveItem(dynamic _param_savedActiveId, dynamic _param_hostTable, dynamic _param_hostPageType)
		{
			#region pass-by-value parameters
			dynamic savedActiveId = XVar.Clone(_param_savedActiveId);
			dynamic hostTable = XVar.Clone(_param_hostTable);
			dynamic hostPageType = XVar.Clone(_param_hostPageType);
			#endregion

			dynamic item = null;
			item = XVar.Clone(this.root.findActiveItem((XVar)(savedActiveId), (XVar)(hostTable), (XVar)(hostPageType)));
			if((XVar)(!(XVar)(item))  && (XVar)(savedActiveId))
			{
				item = XVar.Clone(this.root.findActiveItem(new XVar(null), (XVar)(hostTable), (XVar)(hostPageType)));
			}
			return item;
		}
		public virtual XVar addURL(dynamic _param_label, dynamic _param_url, dynamic _param_parentId = null)
		{
			#region default values
			if(_param_parentId as Object == null) _param_parentId = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic label = XVar.Clone(_param_label);
			dynamic url = XVar.Clone(_param_url);
			dynamic parentId = XVar.Clone(_param_parentId);
			#endregion

			dynamic item = null;
			item = XVar.Clone(this.makeURLItem((XVar)(label), (XVar)(url)));
			this.addItemToMenu((XVar)(item), (XVar)(parentId));
			return item;
		}
		public virtual XVar addPageLink(dynamic _param_label, dynamic _param_table, dynamic _param_pageType, dynamic _param_parentId = null)
		{
			#region default values
			if(_param_parentId as Object == null) _param_parentId = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic label = XVar.Clone(_param_label);
			dynamic table = XVar.Clone(_param_table);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic parentId = XVar.Clone(_param_parentId);
			#endregion

			dynamic item = null;
			item = XVar.Clone(this.makePageLinkItem((XVar)(label), (XVar)(table), (XVar)(pageType)));
			this.addItemToMenu((XVar)(item), (XVar)(parentId));
			return item;
		}
		public virtual XVar addGroup(dynamic _param_label, dynamic _param_parentId = null)
		{
			#region default values
			if(_param_parentId as Object == null) _param_parentId = new XVar(0);
			#endregion

			#region pass-by-value parameters
			dynamic label = XVar.Clone(_param_label);
			dynamic parentId = XVar.Clone(_param_parentId);
			#endregion

			dynamic item = null;
			item = XVar.Clone(this.makeGroupItem((XVar)(label)));
			this.addItemToMenu((XVar)(item), (XVar)(parentId));
			return item;
		}
		public virtual XVar makeGroupItem(dynamic _param_label)
		{
			#region pass-by-value parameters
			dynamic label = XVar.Clone(_param_label);
			#endregion

			dynamic menuNode = XVar.Array();
			menuNode = XVar.Clone(XVar.Array());
			menuNode.InitAndSetArrayItem(MenuItem.newId((XVar)(this.root)), "id");
			menuNode.InitAndSetArrayItem(XVar.Array(), "data");
			menuNode.InitAndSetArrayItem(Constants.menuItemTypeGroup, "data", "itemTtype");
			menuNode.InitAndSetArrayItem(label, "data", "name");
			return new MenuItem((XVar)(menuNode), (XVar)(this.root.menuTableMap), (XVar)(this.id()));
		}
		public virtual XVar makePageLinkItem(dynamic _param_label, dynamic _param_table, dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic label = XVar.Clone(_param_label);
			dynamic table = XVar.Clone(_param_table);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic menuNode = XVar.Array();
			menuNode = XVar.Clone(XVar.Array());
			menuNode.InitAndSetArrayItem(MenuItem.newId((XVar)(this.root)), "id");
			menuNode.InitAndSetArrayItem(XVar.Array(), "data");
			menuNode.InitAndSetArrayItem(Constants.menuItemTypeLeaf, "data", "itemType");
			menuNode.InitAndSetArrayItem(label, "data", "name");
			menuNode.InitAndSetArrayItem(Constants.menuLinkTypeInternal, "data", "linkType");
			menuNode.InitAndSetArrayItem(pageType, "data", "pageType");
			menuNode.InitAndSetArrayItem(table, "data", "tableName");
			return new MenuItem((XVar)(menuNode), (XVar)(this.root.menuTableMap), (XVar)(this.id()));
		}
		public virtual XVar makeURLItem(dynamic _param_label, dynamic _param_url)
		{
			#region pass-by-value parameters
			dynamic label = XVar.Clone(_param_label);
			dynamic url = XVar.Clone(_param_url);
			#endregion

			dynamic menuNode = XVar.Array();
			menuNode = XVar.Clone(XVar.Array());
			menuNode.InitAndSetArrayItem(MenuItem.newId((XVar)(this.root)), "id");
			menuNode.InitAndSetArrayItem(XVar.Array(), "data");
			menuNode.InitAndSetArrayItem(Constants.menuItemTypeLeaf, "data", "itemType");
			menuNode.InitAndSetArrayItem(label, "data", "name");
			menuNode.InitAndSetArrayItem(Constants.menuLinkTypeExternal, "data", "linkType");
			menuNode.InitAndSetArrayItem(url, "data", "href");
			return new MenuItem((XVar)(menuNode), (XVar)(this.root.menuTableMap), (XVar)(this.id()));
		}
		protected virtual XVar addItemToMenu(dynamic _param_menuItem, dynamic _param_parentId)
		{
			#region pass-by-value parameters
			dynamic menuItem = XVar.Clone(_param_menuItem);
			dynamic parentId = XVar.Clone(_param_parentId);
			#endregion

			dynamic parent = null;
			parent = XVar.Clone(MenuItem.findItemById((XVar)(this.root), (XVar)(parentId)));
			if(XVar.Pack(!(XVar)(parent)))
			{
				return false;
			}
			parent.AddChild((XVar)(menuItem));

			return null;
		}
		public virtual XVar removeItem(dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic filteredChildren = XVar.Array(), item = null, parent = null;
			item = XVar.Clone(MenuItem.findItemById((XVar)(this.root), (XVar)(id)));
			if(XVar.Pack(!(XVar)(item)))
			{
				return null;
			}
			if(XVar.Equals(XVar.Pack(item), XVar.Pack(this.root)))
			{
				return null;
			}
			parent = XVar.Clone(item.parentItem);
			filteredChildren = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> child in parent.children.GetEnumerator())
			{
				if(!XVar.Equals(XVar.Pack(child.Value), XVar.Pack(item)))
				{
					filteredChildren.InitAndSetArrayItem(child.Value, null);
				}
			}
			parent.children = XVar.Clone(filteredChildren);

			return null;
		}
		public virtual XVar copyItem(dynamic _param_id, dynamic _param_parentId = null)
		{
			#region default values
			if(_param_parentId as Object == null) _param_parentId = new XVar(-1);
			#endregion

			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			dynamic parentId = XVar.Clone(_param_parentId);
			#endregion

			dynamic clone = null, item = null, parent = null;
			item = XVar.Clone(MenuItem.findItemById((XVar)(this.root), (XVar)(id)));
			if(XVar.Pack(!(XVar)(item)))
			{
				return false;
			}
			clone = XVar.Clone(MenuItem.cloneNode((XVar)(item)));
			clone.id = XVar.Clone(MenuItem.newId((XVar)(this.root)));
			parent = XVar.Clone((XVar.Pack(parentId == -1) ? XVar.Pack(item.parentItem) : XVar.Pack(MenuItem.findItemById((XVar)(this.root), (XVar)(parentId)))));
			if(XVar.Pack(!(XVar)(parent)))
			{
				return false;
			}
			parent.AddChild((XVar)(clone));
			return clone;
		}
		public virtual XVar getItem(dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			#endregion

			return MenuItem.findItemById((XVar)(this.root), (XVar)(id));
		}
		public virtual XVar collectNodes()
		{
			return this.root.collectNodes();
		}
	}
}
