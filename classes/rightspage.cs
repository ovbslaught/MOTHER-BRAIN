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
	public partial class RightsPage : ListPage
	{
		public dynamic tables = XVar.Array();
		public dynamic pages = XVar.Array();
		public dynamic pageMasks = XVar.Array();
		public dynamic rights = XVar.Array();
		public dynamic pageRestrictions = XVar.Array();
		public dynamic groups = XVar.Array();
		public dynamic smartyGroups = XVar.Array();
		public dynamic cbxNames;
		public dynamic permissionNames = XVar.Array();
		public dynamic sortedTables;
		public dynamic menuOrderedTables;
		public dynamic alphaOrderedTables;
		protected static bool skipRightsPageCtor = false;
		private bool skipListPageCtorSurrogate = new Func<bool>(() => skipListPageCtor = true).Invoke();
		public RightsPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipRightsPageCtor)
			{
				skipRightsPageCtor = false;
				return;
			}
			this.connection = XVar.Clone(GlobalVars.cman.getForUserGroups());
			this.permissionNames.InitAndSetArrayItem(true, "A");
			this.permissionNames.InitAndSetArrayItem(true, "D");
			this.permissionNames.InitAndSetArrayItem(true, "E");
			this.permissionNames.InitAndSetArrayItem(true, "S");
			this.permissionNames.InitAndSetArrayItem(true, "P");
			this.permissionNames.InitAndSetArrayItem(true, "I");
			this.permissionNames.InitAndSetArrayItem(true, "M");
			this.cbxNames = XVar.Clone(new XVar("add", new XVar("mask", "A", "rightName", "add"), "edt", new XVar("mask", "E", "rightName", "edit"), "del", new XVar("mask", "D", "rightName", "delete"), "lst", new XVar("mask", "S", "rightName", "list"), "exp", new XVar("mask", "P", "rightName", "export"), "imp", new XVar("mask", "I", "rightName", "import"), "adm", new XVar("mask", "M")));
			this.initLogin();
			this.setLangParams();
			this.sortTables();
			this.fillGroupsArr();
			this.fillPagesArr();
		}
		public virtual XVar fillPagesArr()
		{
			dynamic pages = XVar.Array();
			pages = XVar.Clone(CommonFunctions.allTablePages());
			foreach (KeyValuePair<XVar, dynamic> _tablePages in pages.GetEnumerator())
			{
				this.pages.InitAndSetArrayItem(XVar.Array(), _tablePages.Key);
				foreach (KeyValuePair<XVar, dynamic> pageIds in pages[_tablePages.Key].GetEnumerator())
				{
					if((XVar)(_tablePages.Key == Constants.GLOBAL_PAGES)  && (XVar)(pageIds.Key != "menu"))
					{
						continue;
					}
					foreach (KeyValuePair<XVar, dynamic> p in pageIds.Value.GetEnumerator())
					{
						this.pages.InitAndSetArrayItem(Security.pageType2permission((XVar)(pageIds.Key)), _tablePages.Key, p.Value);
					}
				}
			}

			return null;
		}
		public virtual XVar fillGroupsArr()
		{
			dynamic dataSource = null, dc = null, grConnection = null, groupIdField = null, groupLabelField = null, groupProviderField = null, label = null, providerCode = null, qResult = null, renameable = null, tdata = XVar.Array();
			grConnection = XVar.Clone(GlobalVars.cman.getForUserGroups());
			this.groups.InitAndSetArrayItem(new XVar("label", MVCFunctions.Concat("<", CommonFunctions.mlang_message(new XVar("AA_GROUP_ADMIN")), ">")), -1);
			this.groups.InitAndSetArrayItem(new XVar("label", MVCFunctions.Concat("<", CommonFunctions.mlang_message(new XVar("AA_GROUP_DEFAULT")), ">")), -2);
			this.groups.InitAndSetArrayItem(new XVar("label", MVCFunctions.Concat("<", CommonFunctions.mlang_message(new XVar("AA_GROUP_GUEST")), ">")), -3);
			groupIdField = new XVar("GroupID");
			groupLabelField = new XVar("Label");
			groupProviderField = new XVar("Provider");
			dataSource = XVar.Clone(Security.getUgGroupsDatasource());
			dc = XVar.Clone(new DsCommand());
			if(XVar.Pack(CommonFunctions.storageGet(new XVar("groups_provider_field"))))
			{
				dc.order.InitAndSetArrayItem(new XVar("column", groupProviderField, "dir", "ASC"), null);
			}
			dc.order.InitAndSetArrayItem(new XVar("column", groupLabelField, "dir", "ASC"), null);
			qResult = XVar.Clone(dataSource.getList((XVar)(dc)));
			CommonFunctions.storageSet(new XVar("groups_provider_field"), (XVar)(qResult.fieldExists((XVar)(groupProviderField))));
			while(XVar.Pack(tdata = XVar.Clone(qResult.fetchAssoc())))
			{
				label = XVar.Clone(tdata[groupLabelField]);
				renameable = new XVar(true);
				providerCode = XVar.Clone(tdata[groupProviderField]);
				if(XVar.Pack(providerCode))
				{
					dynamic provider = XVar.Array(), providerLabel = null;
					provider = XVar.Clone(Security.findProvider((XVar)(providerCode)));
					renameable = XVar.Clone(provider["type"] == Constants.stDB);
					providerLabel = XVar.Clone(CommonFunctions.GetMLString((XVar)(provider["label"])));
					if(XVar.Pack(providerLabel))
					{
						label = XVar.Clone(MVCFunctions.Concat(providerLabel, ":", label));
					}
				}
				this.groups.InitAndSetArrayItem(new XVar("label", label, "renameable", renameable), tdata[groupIdField]);
			}

			return null;
		}
		public virtual XVar fillSmartyAndRights()
		{
			dynamic first = null;
			first = new XVar(true);
			foreach (KeyValuePair<XVar, dynamic> gr in this.groups.GetEnumerator())
			{
				dynamic name = null, sg = XVar.Array();
				name = XVar.Clone(gr.Value["label"]);
				sg = XVar.Clone(XVar.Array());
				sg.InitAndSetArrayItem(MVCFunctions.Concat("value=\"", gr.Key, "\""), "group_attrs");
				if(XVar.Pack(gr.Value["renameable"]))
				{
					sg["group_attrs"] = MVCFunctions.Concat(sg["group_attrs"], " data-renameable");
				}
				if(XVar.Pack(first))
				{
					sg.InitAndSetArrayItem("active", "group_class");
					first = new XVar(false);
				}
				sg.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(name)), "groupname");
				this.smartyGroups.InitAndSetArrayItem(sg, null);
			}

			return null;
		}
		public virtual XVar getRights()
		{
			dynamic group = null, mask = null, pages = null, qResult = null, sql = null, strPages = null, table = null, tdata = XVar.Array();
			sql = XVar.Clone(MVCFunctions.Concat("select ", this.connection.addFieldWrappers(new XVar("GroupID")), ", ", this.connection.addFieldWrappers(new XVar("TableName")), ", ", this.connection.addFieldWrappers(new XVar("AccessMask")), ", ", this.connection.addFieldWrappers(new XVar("Page")), " from ", this.connection.addTableWrappers((XVar)(MVCFunctions.Concat(ProjectSettings.getSecurityValue(new XVar("dpTablePrefix")), "ugrights"))), " order by ", this.connection.addFieldWrappers(new XVar("GroupID"))));
			qResult = XVar.Clone(this.connection.query((XVar)(sql)));
			while(XVar.Pack(tdata = XVar.Clone(qResult.fetchNumeric())))
			{
				group = XVar.Clone(tdata[0]);
				table = XVar.Clone(tdata[1]);
				mask = XVar.Clone(tdata[2]);
				strPages = XVar.Clone(tdata[3]);
				pages = XVar.Clone(XVar.Array());
				if(XVar.Pack(strPages))
				{
					pages = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(strPages)));
				}
				if(XVar.Pack(!(XVar)(this.tables.KeyExists(table))))
				{
					continue;
				}
				if(XVar.Pack(!(XVar)(this.groups.KeyExists(group))))
				{
					continue;
				}
				if(XVar.Pack(!(XVar)(this.rights.KeyExists(table))))
				{
					this.rights.InitAndSetArrayItem(XVar.Array(), table);
					this.pageRestrictions.InitAndSetArrayItem(XVar.Array(), table);
				}
				this.rights.InitAndSetArrayItem(this.fixMask((XVar)(mask), (XVar)(this.pageMasks[table])), table, group);
				if(XVar.Pack(pages))
				{
					this.pageRestrictions.InitAndSetArrayItem(pages, table, group);
				}
			}
			if(1 < MVCFunctions.count(MVCFunctions.array_keys((XVar)(this.pages[Constants.GLOBAL_PAGES]))))
			{
				if(XVar.Pack(!(XVar)(this.rights.KeyExists(Constants.GLOBAL_PAGES))))
				{
					this.rights.InitAndSetArrayItem(XVar.Array(), Constants.GLOBAL_PAGES);
					this.pageRestrictions.InitAndSetArrayItem(XVar.Array(), Constants.GLOBAL_PAGES);
				}
				foreach (KeyValuePair<XVar, dynamic> d in this.groups.GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(this.rights[Constants.GLOBAL_PAGES].KeyExists(d.Key))))
					{
						this.rights.InitAndSetArrayItem("S", Constants.GLOBAL_PAGES, d.Key);
					}
				}
			}

			return null;
		}
		public virtual XVar addJsGroupsAndRights()
		{

			return null;
		}
		public override XVar commonAssign()
		{
			this.xt.assign_loopsection(new XVar("groups"), (XVar)(this.smartyGroups));
			base.commonAssign();
			foreach (KeyValuePair<XVar, dynamic> t in this.permissionNames.GetEnumerator())
			{
				this.xt.assign((XVar)(MVCFunctions.Concat(t.Key, "_headcheckbox")), (XVar)(MVCFunctions.Concat(" id=\"colbox", t.Key, "\" data-perm=\"", t.Key, "\"")));
			}
			this.xt.assign(new XVar("delgroup_attrs"), new XVar("id=\"delGroupBtn\""));
			this.xt.assign(new XVar("rengroup_attrs"), new XVar("id=\"renGroupBtn\""));
			this.xt.assign(new XVar("savegroup_attrs"), new XVar("id=\"saveGroupBtn\""));
			this.xt.assign(new XVar("savebutton_attrs"), new XVar("id=\"saveBtn\""));
			this.xt.assign(new XVar("resetbutton_attrs"), new XVar("id=\"resetBtn\""));
			this.xt.assign(new XVar("cancelgroup_attrs"), new XVar("id=\"cancelBtn\""));
			this.xt.assign(new XVar("grid_block"), new XVar(true));
			this.xt.assign(new XVar("menu_block"), new XVar(true));
			this.xt.assign(new XVar("left_block"), new XVar(true));
			this.xt.assign(new XVar("rights_block"), new XVar(true));
			this.xt.assign(new XVar("message_block"), new XVar(true));
			this.xt.assign(new XVar("security_block"), new XVar(true));
			this.xt.assign(new XVar("savebuttons_block"), new XVar(true));
			this.xt.assign(new XVar("search_records_block"), new XVar(true));
			this.xt.assign(new XVar("recordcontrols_block"), new XVar(true));
			this.xt.assign(new XVar("username"), (XVar)(XSession.Session["UserName"]));
			this.xt.assign(new XVar("userid"), (XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(Security.getUserName()))));
			this.hideElement(new XVar("message"));

			return null;
		}
		public override XVar getBreadcrumbMenuId()
		{
			return "adminarea";
		}
		public virtual XVar sortTables()
		{
			dynamic addedTables = XVar.Array(), allTables = null, arr = XVar.Array(), groupsMap = XVar.Array(), menuNodes = XVar.Array(), menuObject = null;
			this.sortedTables = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> tbl in this.tables.GetEnumerator())
			{
				this.sortedTables.InitAndSetArrayItem(new XVar(0, tbl.Key, 1, tbl.Value[1]), null);
			}
			MVCFunctions.usort((XVar)(this.sortedTables), new XVar("rightsSortFunc"));
			this.alphaOrderedTables = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> t in this.sortedTables.GetEnumerator())
			{
				this.alphaOrderedTables.InitAndSetArrayItem(t.Value[0], null);
			}
			this.menuOrderedTables = XVar.Clone(XVar.Array());
			menuObject = XVar.Clone(RunnerMenu.getMenuObject(new XVar("main")));
			menuNodes = XVar.Clone(menuObject.collectNodes());
			addedTables = XVar.Clone(XVar.Array());
			groupsMap = XVar.Clone(XVar.Array());
			allTables = XVar.Clone(CommonFunctions.GetTablesListWithoutSecurity());
			addedTables.InitAndSetArrayItem(true, Constants.GLOBAL_PAGES);
			arr.InitAndSetArrayItem(Constants.GLOBAL_PAGES, "table");
			arr.InitAndSetArrayItem(XVar.Array(), "items");
			arr.InitAndSetArrayItem(true, "collapsed");
			this.menuOrderedTables.InitAndSetArrayItem(arr, null);
			foreach (KeyValuePair<XVar, dynamic> mNode in menuNodes.GetEnumerator())
			{
				dynamic nodeId = null, nodeType = null, pageType = null, parentNodeId = null, table = null, title = null;
				nodeType = XVar.Clone(mNode.Value.var_type);
				nodeId = XVar.Clone(mNode.Value.id);
				table = XVar.Clone(mNode.Value.table);
				title = XVar.Clone(mNode.Value.title);
				pageType = XVar.Clone(mNode.Value.pageType);
				parentNodeId = XVar.Clone((XVar.Pack(mNode.Value.parentItem) ? XVar.Pack(mNode.Value.parentItem.id) : XVar.Pack(0)));
				arr = XVar.Clone(XVar.Array());
				if((XVar)(pageType == "webreports")  || (XVar)(nodeType == Constants.menuItemTypeSeparator))
				{
					continue;
				}
				if((XVar)((XVar)(table)  && (XVar)(!(XVar)(addedTables[table])))  && (XVar)(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(table), (XVar)(allTables))), XVar.Pack(false))))
				{
					addedTables.InitAndSetArrayItem(true, table);
					arr.InitAndSetArrayItem(table, "table");
				}
				if(XVar.Pack(parentNodeId))
				{
					arr.InitAndSetArrayItem(groupsMap[parentNodeId], "parent");
					this.menuOrderedTables.InitAndSetArrayItem(MVCFunctions.count(this.menuOrderedTables), arr["parent"], "items", null);
				}
				if(nodeType == Constants.menuItemTypeGroup)
				{
					arr.InitAndSetArrayItem(MVCFunctions.count(this.menuOrderedTables), "groupId");
				}
				if(nodeType == Constants.menuItemTypeGroup)
				{
					groupsMap.InitAndSetArrayItem(MVCFunctions.count(this.menuOrderedTables), nodeId);
					arr.InitAndSetArrayItem(title, "title");
					arr.InitAndSetArrayItem(XVar.Array(), "items");
					arr.InitAndSetArrayItem(true, "collapsed");
				}
				this.menuOrderedTables.InitAndSetArrayItem(arr, null);
			}
			if(MVCFunctions.count(addedTables) < MVCFunctions.count(this.alphaOrderedTables))
			{
				dynamic unlistedId = null;
				unlistedId = XVar.Clone(MVCFunctions.count(this.menuOrderedTables));
				arr = XVar.Clone(XVar.Array());
				arr.InitAndSetArrayItem(true, "collapsed");
				arr.InitAndSetArrayItem(CommonFunctions.mlang_message(new XVar("UNLISTED")), "title");
				arr.InitAndSetArrayItem(XVar.Array(), "items");
				this.menuOrderedTables.InitAndSetArrayItem(arr, null);
				foreach (KeyValuePair<XVar, dynamic> _table in this.alphaOrderedTables.GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(addedTables[_table.Value])))
					{
						this.menuOrderedTables.InitAndSetArrayItem(MVCFunctions.count(this.menuOrderedTables), unlistedId, "items", null);
						this.menuOrderedTables.InitAndSetArrayItem(new XVar("table", _table.Value, "parent", unlistedId), null);
					}
				}
			}

			return null;
		}
		public virtual XVar getItemsCount(dynamic _param_itemIdx)
		{
			#region pass-by-value parameters
			dynamic itemIdx = XVar.Clone(_param_itemIdx);
			#endregion

			dynamic count = null;
			count = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> idx in this.menuOrderedTables[itemIdx]["items"].GetEnumerator())
			{
				if(XVar.Pack(this.menuOrderedTables[idx.Value].KeyExists("items")))
				{
					count += this.getItemsCount((XVar)(idx.Value));
				}
				if(XVar.Pack(this.menuOrderedTables[idx.Value].KeyExists("table")))
				{
					count++;
				}
			}
			return count;
		}
		public virtual XVar fillTablesGrid(dynamic rowInfoArr)
		{
			dynamic copylink = null, editlink = null, hasGroupsToExpand = null, parentStack = XVar.Array(), recNo = null, rowClass = null;
			rowClass = new XVar(false);
			recNo = new XVar(1);
			editlink = new XVar("");
			copylink = new XVar("");
			parentStack = XVar.Clone(XVar.Array());
			hasGroupsToExpand = new XVar(false);
			foreach (KeyValuePair<XVar, dynamic> tbl in this.menuOrderedTables.GetEnumerator())
			{
				dynamic childrenCount = null, parent = null, row = XVar.Array(), table = null;
				table = XVar.Clone(tbl.Value["table"]);
				parent = XVar.Clone(tbl.Value["parent"]);
				if((XVar)(table == Constants.GLOBAL_PAGES)  && (XVar)(MVCFunctions.count(MVCFunctions.array_keys((XVar)(this.pages[table]))) < 2))
				{
					continue;
				}
				if(XVar.Pack(!(XVar)(parent as object != null)))
				{
					parentStack = XVar.Clone(XVar.Array());
				}
				else
				{
					dynamic stackPos = null;
					stackPos = XVar.Clone(MVCFunctions.array_search((XVar)(parent), (XVar)(parentStack)));
					if(XVar.Equals(XVar.Pack(stackPos), XVar.Pack(false)))
					{
						parentStack.InitAndSetArrayItem(parent, null);
					}
					else
					{
						parentStack = XVar.Clone(MVCFunctions.array_slice((XVar)(parentStack), new XVar(0), (XVar)(stackPos + 1)));
					}
				}
				if(XVar.Pack(MVCFunctions.strlen((XVar)(table))))
				{
					dynamic caption = null, shortTable = null, tablename = null;
					caption = XVar.Clone(this.tables[table][1]);
					shortTable = XVar.Clone(this.tables[table][0]);
					row = XVar.Clone(XVar.Array());
					if(caption == table)
					{
						tablename = XVar.Clone(MVCFunctions.runner_htmlspecialchars((XVar)(table)));
					}
					else
					{
						if(table == Constants.GLOBAL_PAGES)
						{
							tablename = XVar.Clone(CommonFunctions.mlang_message(new XVar("MENU_PAGE")));
						}
						else
						{
							tablename = XVar.Clone(MVCFunctions.Concat("<span dir='LTR'>", MVCFunctions.runner_htmlspecialchars((XVar)(caption)), "&nbsp;(", MVCFunctions.runner_htmlspecialchars((XVar)(table)), ")</span>"));
						}
					}
					row.InitAndSetArrayItem(tablename, "tablename");
					row.InitAndSetArrayItem(MVCFunctions.Concat(" id=\"row_", shortTable, "\""), "table_row_attrs");
					row.InitAndSetArrayItem(MVCFunctions.Concat("id=\"rowbox", shortTable, "\" data-table=\"", shortTable, "\" data-checked=0"), "tablecheckbox_attrs");
					row.InitAndSetArrayItem(MVCFunctions.Concat(" id=\"tblcell", shortTable, "\""), "tbl_cell");
					row.InitAndSetArrayItem(table != Constants.GLOBAL_PAGES, "tablecheckbox");
					if(table != Constants.GLOBAL_PAGES)
					{
						dynamic mask = null;
						mask = XVar.Clone(this.pageMasks[table]);
						foreach (KeyValuePair<XVar, dynamic> x in this.permissionNames.GetEnumerator())
						{
							if(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(mask), (XVar)(x.Key))), XVar.Pack(false)))
							{
								continue;
							}
							row.InitAndSetArrayItem(true, MVCFunctions.Concat(x.Key, "_group"));
							row.InitAndSetArrayItem(MVCFunctions.Concat(" id=\"box", x.Key, shortTable, "\" data-checked=0"), MVCFunctions.Concat(x.Key, "_checkbox"));
							row.InitAndSetArrayItem(MVCFunctions.Concat(" id=\"cell", x.Key, shortTable, "\""), MVCFunctions.Concat(x.Key, "_cell"));
						}
					}
					row["hide_pages_attrs"] = MVCFunctions.Concat(row["hide_pages_attrs"], "data-hide-pages data-hidden data-table=\"", shortTable, "\"");
					row["show_pages_attrs"] = MVCFunctions.Concat(row["show_pages_attrs"], "data-show-pages data-table=\"", shortTable, "\"");
					this.fillPageRows((XVar)(table), (XVar)(shortTable), (XVar)(row), (XVar)(MVCFunctions.count(parentStack)));
				}
				else
				{
					dynamic title = null;
					title = XVar.Clone(tbl.Value["title"]);
					row = XVar.Clone(XVar.Array());
					row.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(title)), "tablename");
					row.InitAndSetArrayItem(" data-checked=-2", "tablecheckbox_attrs");
					row.InitAndSetArrayItem(MVCFunctions.Concat(" id=\"grouprow_", tbl.Key, "\""), "table_row_attrs");
					row["hide_pages_attrs"] = MVCFunctions.Concat(row["hide_pages_attrs"], "data-hidden");
					row["show_pages_attrs"] = MVCFunctions.Concat(row["show_pages_attrs"], "data-hidden");
				}
				if(XVar.Pack(parent as object != null))
				{
					row["table_row_attrs"] = MVCFunctions.Concat(row["table_row_attrs"], " data-level=\"", MVCFunctions.count(parentStack), "\"");
				}
				childrenCount = XVar.Clone(this.getItemsCount((XVar)(tbl.Key)));
				if((XVar)(tbl.Value.KeyExists("items"))  && (XVar)(childrenCount))
				{
					hasGroupsToExpand = new XVar(true);
					row["tablename"] = MVCFunctions.Concat(row["tablename"], "<span class='tablecount' dir='LTR'>&nbsp;(", this.getItemsCount((XVar)(tbl.Key)), ")</span>");
					row["table_row_attrs"] = MVCFunctions.Concat(row["table_row_attrs"], " data-groupid=\"", tbl.Key, "\"");
					row.InitAndSetArrayItem(true, "groupControl");
					row.InitAndSetArrayItem(" data-state='closed'", "groupControlState");
					row.InitAndSetArrayItem(" data-state='closed'", "groupControlClass");
					row["tblrowclass"] = MVCFunctions.Concat(row["tblrowclass"], " menugroup");
					if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(table)))))
					{
						row["tblrowclass"] = MVCFunctions.Concat(row["tblrowclass"], " menugrouponly");
					}
				}
				else
				{
					if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(table)))))
					{
						continue;
					}
				}
				if(XVar.Pack(parent))
				{
					row["table_row_attrs"] = MVCFunctions.Concat(row["table_row_attrs"], " style='display:none;' data-ingroup='true' ");
				}
				rowInfoArr.InitAndSetArrayItem(row, null);
			}
			if(XVar.Pack(!(XVar)(hasGroupsToExpand)))
			{
				this.hideItemType(new XVar("rights_expand_all"));
			}

			return null;
		}
		public virtual XVar fillPageRows(dynamic _param_table, dynamic _param_shortTable, dynamic row, dynamic _param_level)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic shortTable = XVar.Clone(_param_shortTable);
			dynamic level = XVar.Clone(_param_level);
			#endregion

			dynamic allPages = XVar.Array(), pageRows = XVar.Array(), pages = XVar.Array();
			allPages = XVar.Clone(CommonFunctions.tablePages((XVar)(table)));
			pages = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> pids in allPages.GetEnumerator())
			{
				if((XVar)(table == Constants.GLOBAL_PAGES)  && (XVar)(pids.Key != "menu"))
				{
					continue;
				}
				foreach (KeyValuePair<XVar, dynamic> p in pids.Value.GetEnumerator())
				{
					pages.InitAndSetArrayItem(pids.Key, p.Value);
				}
			}
			MVCFunctions.ksort(ref pages, new XVar(Constants.SORT_STRING));
			pageRows = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> pageType in pages.GetEnumerator())
			{
				dynamic pageRow = XVar.Array(), perm = null;
				pageRow = XVar.Clone(XVar.Array());
				perm = XVar.Clone(Security.pageType2permission((XVar)(pageType.Value)));
				pageRow.InitAndSetArrayItem(true, MVCFunctions.Concat(perm, "_pagebox"));
				pageRow.InitAndSetArrayItem(true, "pagebox");
				pageRow.InitAndSetArrayItem(MVCFunctions.Concat(" data-table=\"", shortTable, "\" data-page=\"", pageType.Key, "\" id=\"pagebox", perm, shortTable, "_", pageType.Key, "\" data-checked=0"), MVCFunctions.Concat(perm, "_pagecheckbox"));
				pageRow.InitAndSetArrayItem(MVCFunctions.Concat("data-permission=\"", perm, "\" data-table=\"", shortTable, "\" data-page=\"", pageType.Key, "\" id=\"wholepagebox_", shortTable, "_", pageType.Key, "\" data-checked=0"), "pagecheckbox");
				pageRow.InitAndSetArrayItem(MVCFunctions.Concat(" id=\"pagecell", perm, shortTable, "_", pageType.Key, "\""), MVCFunctions.Concat(perm, "_cell"));
				pageRow.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(pageType.Key)), "rights_page");
				pageRow.InitAndSetArrayItem(MVCFunctions.Concat("data-hidden data-table=\"", shortTable, "\" data-page=\"", pageType.Key, "\" data-level=\"", level, "\""), "page_row_attrs");
				pageRows.InitAndSetArrayItem(pageRow, null);
			}
			row.InitAndSetArrayItem(XVar.Array(), "page_row");
			row.InitAndSetArrayItem(pageRows, "page_row", "data");

			return null;
		}
		public override XVar fillGridData()
		{
			dynamic rowInfo = null;
			rowInfo = XVar.Clone(XVar.Array());
			this.fillTablesGrid((XVar)(rowInfo));
			this.xt.assign_loopsection(new XVar("grid_row"), (XVar)(rowInfo));

			return null;
		}
		public override XVar setSessionVariables()
		{

			return null;
		}
		public override XVar prepareForBuildPage()
		{
			this.fillSmartyAndRights();
			this.getRights();
			this.fillGridData();
			this.addCommonJs();
			this.addCommonHtml();
			this.commonAssign();

			return null;
		}
		public override XVar showPage()
		{
			this.display((XVar)(this.templatefile));

			return null;
		}
		public override XVar addCommonHtml()
		{
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage((XVar)(this.isDisplayLoading)));
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");

			return null;
		}
		public override XVar prepareForResizeColumns()
		{

			return null;
		}
		public override XVar addCommonJs()
		{
			RunnerPage.addCommonJs(this);
			this.addJsGroupsAndRights();

			return null;
		}
		public virtual XVar fixMask(dynamic _param_mask, dynamic _param_possibleMask)
		{
			#region pass-by-value parameters
			dynamic mask = XVar.Clone(_param_mask);
			dynamic possibleMask = XVar.Clone(_param_possibleMask);
			#endregion

			dynamic i = null, l = null, outMask = null;
			outMask = new XVar("");
			l = XVar.Clone(MVCFunctions.strlen((XVar)(possibleMask)));
			i = new XVar(0);
			for(;i < l; ++(i))
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(mask), (XVar)(possibleMask[i]))), XVar.Pack(false)))
				{
					outMask = MVCFunctions.Concat(outMask, possibleMask[i]);
				}
			}
			return outMask;
		}
		public virtual XVar saveRights(dynamic modifiedRights)
		{
			foreach (KeyValuePair<XVar, dynamic> rights in modifiedRights.GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> tableRights in modifiedRights[rights.Key].GetEnumerator())
				{
					this.updateTablePermissions((XVar)(tableRights.Key), (XVar)(rights.Key), (XVar)(tableRights.Value));
				}
			}
			MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(new XVar("success", true))));

			return null;
		}
		public virtual XVar updateTablePermissions(dynamic _param_table, dynamic _param_group, dynamic _param_tableRights)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic group = XVar.Clone(_param_group);
			dynamic tableRights = XVar.Clone(_param_tableRights);
			#endregion

			dynamic accessMaskWFieldName = null, data = XVar.Array(), groupWhere = null, groupisWFieldName = null, mask = null, pageWFieldName = null, pages = null, rightWTableName = null, sql = null, strPages = null, tableNameWFieldName = null;
			mask = XVar.Clone(tableRights["permissions"]);
			rightWTableName = XVar.Clone(this.connection.addTableWrappers((XVar)(MVCFunctions.Concat(ProjectSettings.getSecurityValue(new XVar("dpTablePrefix")), "ugrights"))));
			accessMaskWFieldName = XVar.Clone(this.connection.addFieldWrappers(new XVar("AccessMask")));
			groupisWFieldName = XVar.Clone(this.connection.addFieldWrappers(new XVar("GroupID")));
			pageWFieldName = XVar.Clone(this.connection.addFieldWrappers(new XVar("Page")));
			tableNameWFieldName = XVar.Clone(this.connection.addFieldWrappers(new XVar("TableName")));
			groupWhere = XVar.Clone(MVCFunctions.Concat(groupisWFieldName, "=", group, " and ", tableNameWFieldName, "=", this.connection.prepareString((XVar)(table))));
			strPages = new XVar("");
			pages = XVar.Clone(tableRights["pages"]);
			if(XVar.Pack(pages))
			{
				strPages = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(pages)));
			}
			sql = XVar.Clone(MVCFunctions.Concat("select ", accessMaskWFieldName, " from ", rightWTableName, " where ", groupWhere));
			data = XVar.Clone(this.connection.query((XVar)(sql)).fetchNumeric());
			if(XVar.Pack(data))
			{
				dynamic correctedMask = null, pageMask = null, savedMask = null;
				savedMask = XVar.Clone(data[0]);
				pageMask = XVar.Clone(this.pageMasks[table]);
				correctedMask = new XVar("");
				foreach (KeyValuePair<XVar, dynamic> t in this.permissionNames.GetEnumerator())
				{
					if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(pageMask), (XVar)(t.Key))), XVar.Pack(false)))
					{
						if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(mask), (XVar)(t.Key))), XVar.Pack(false)))
						{
							correctedMask = MVCFunctions.Concat(correctedMask, t.Key);
						}
					}
					else
					{
						if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(savedMask), (XVar)(t.Key))), XVar.Pack(false)))
						{
							correctedMask = MVCFunctions.Concat(correctedMask, t.Key);
						}
					}
				}
				mask = XVar.Clone(correctedMask);
				if((XVar)(MVCFunctions.strlen((XVar)(mask)))  && (XVar)(!(XVar)((XVar)(table == Constants.GLOBAL_PAGES)  && (XVar)((XVar)(strPages == XVar.Pack(""))  || (XVar)(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(mask), new XVar("S"))), XVar.Pack(false)))))))
				{
					sql = XVar.Clone(MVCFunctions.Concat("update ", rightWTableName, " set ", accessMaskWFieldName, "='", mask, "',", tableNameWFieldName, "=", this.connection.prepareString((XVar)(table)), ",", pageWFieldName, "=", this.connection.prepareString((XVar)(strPages)), " where ", groupWhere));
				}
				else
				{
					sql = XVar.Clone(MVCFunctions.Concat("delete from ", rightWTableName, " where ", groupWhere));
				}
			}
			else
			{
				if((XVar)(!(XVar)(MVCFunctions.strlen((XVar)(mask))))  || (XVar)((XVar)(table == Constants.GLOBAL_PAGES)  && (XVar)(strPages == XVar.Pack(""))))
				{
					return null;
				}
				sql = XVar.Clone(MVCFunctions.Concat("insert into ", rightWTableName, " (", groupisWFieldName, ", ", tableNameWFieldName, ", ", accessMaskWFieldName, ", ", pageWFieldName, ")", " values (", group, ", ", this.connection.prepareString((XVar)(table)), ", '", mask, "', ", this.connection.prepareString((XVar)(strPages)), ")"));
			}
			this.connection.exec((XVar)(sql));

			return null;
		}
		public override XVar buildSearchPanel()
		{

			return null;
		}
		public override XVar assignSimpleSearch()
		{

			return null;
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
			settings.InitAndSetArrayItem(true, "warnOnLeaving");
			settings.InitAndSetArrayItem(this.rights, "rights");
			settings.InitAndSetArrayItem(this.pageRestrictions, "pageRestrictions");
			settings.InitAndSetArrayItem(this.groups, "groups");
			settings.InitAndSetArrayItem(this.tables, "tables");
			settings.InitAndSetArrayItem(this.pages, "allPages");
			settings.InitAndSetArrayItem(this.pageMasks, "pageMasks");
			settings.InitAndSetArrayItem(this.menuOrderedTables, "menuOrderedTables");
			settings.InitAndSetArrayItem(this.alphaOrderedTables, "alphaOrderedTables");
			return settings;
		}
		public override XVar isAdminTable()
		{
			return true;
		}
	}
	// Included file globals
	public partial class CommonFunctions
	{
		public static XVar rightsSortFunc(dynamic _param_a, dynamic _param_b)
		{
			#region pass-by-value parameters
			dynamic a = XVar.Clone(_param_a);
			dynamic b = XVar.Clone(_param_b);
			#endregion

			if(a[1] == b[1])
			{
				return 0;
			}
			if(a[1] < b[1])
			{
				return -1;
			}
			return 1;
		}
	}
}
