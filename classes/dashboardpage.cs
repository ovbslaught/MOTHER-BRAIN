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
	public partial class DashboardPage : RunnerPage
	{
		protected dynamic elementsPermissions = XVar.Array();
		public dynamic action = XVar.Pack("");
		public dynamic elementName = XVar.Pack("");
		protected static bool skipDashboardPageCtor = false;
		public DashboardPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipDashboardPageCtor)
			{
				skipDashboardPageCtor = false;
				return;
			}
			dynamic returnJSON = null;
			if(var_params["mode"] == "dashsearch")
			{
				dynamic showShowAllButton = null;
				showShowAllButton = XVar.Clone(this.searchClauseObj.searchStarted());
				foreach (KeyValuePair<XVar, dynamic> elem in this.pSet.getDashboardElements().GetEnumerator())
				{
					if((XVar)(elem.Value["type"] == Constants.DASHBOARD_LIST)  || (XVar)(elem.Value["type"] == Constants.DASHBOARD_REPORT))
					{
						dynamic elSessionPrefix = null;
						elSessionPrefix = XVar.Clone((XVar.Pack(elem.Value["type"] == Constants.DASHBOARD_LIST) ? XVar.Pack(MVCFunctions.Concat(this.tName, "_", elem.Value["elementName"])) : XVar.Pack(MVCFunctions.Concat(this.tName, "_", elem.Value["table"]))));
						XSession.Session[MVCFunctions.Concat(elSessionPrefix, "_pagenumber")] = 1;
					}
				}
				returnJSON = XVar.Clone(new XVar("success", true, "show_all", showShowAllButton));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			if(this.action == "reloadElement")
			{
				dynamic contentHtml = null, snippetId = null;
				snippetId = XVar.Clone(MVCFunctions.postvalue(new XVar("snippetId")));
				foreach (KeyValuePair<XVar, dynamic> elem in this.pSet.getDashboardElements().GetEnumerator())
				{
					if((XVar)(elem.Value["type"] == Constants.DASHBOARD_SNIPPET)  && (XVar)(elem.Value["elementName"] == this.elementName))
					{
						dynamic snippetData = null;
						snippetData = XVar.Clone(this.callSnippet((XVar)(elem.Value)));
						contentHtml = XVar.Clone(this.getSnippetHtml((XVar)(elem.Value), (XVar)(snippetData)));
						break;
					}
				}
				returnJSON = XVar.Clone(new XVar("success", true, "contentHtml", contentHtml));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			this.setLangParams();
		}
		public virtual XVar CheckPermissions(dynamic _param_table, dynamic _param_permis)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic permis = XVar.Clone(_param_permis);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> val in this.getPermissionType((XVar)(permis)).GetEnumerator())
			{
				if(XVar.Pack(CommonFunctions.CheckTablePermissions((XVar)(table), (XVar)(val.Value))))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar getPermissionType(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic result = null, var_type = null;
			result = XVar.Clone(XVar.Array());
			var_type = XVar.Clone(base.getPermisType((XVar)(pageType)));
			if((XVar)((XVar)((XVar)((XVar)((XVar)(pageType == "view")  || (XVar)(pageType == "chart"))  || (XVar)(pageType == "report"))  || (XVar)(pageType == "list"))  || (XVar)(pageType == "calendar"))  || (XVar)(pageType == "gantt"))
			{
				var_type = new XVar("S");
			}
			else
			{
				if(pageType == "add")
				{
					var_type = new XVar("A");
				}
				else
				{
					if(pageType == "edit")
					{
						var_type = new XVar("E");
					}
				}
			}
			return (XVar.Pack(var_type) ? XVar.Pack(new XVar(0, var_type)) : XVar.Pack(result));
		}
		public override XVar init()
		{
			base.init();
			this.fillElementPermissions();
			this.createElementContainers();

			return null;
		}
		public virtual XVar createElementContainers()
		{
			foreach (KeyValuePair<XVar, dynamic> elem in this.pSet.getDashboardElements().GetEnumerator())
			{
				dynamic contentHtml = null, dbElementAttrs = null, selectors = XVar.Array(), style = null;
				if(XVar.Pack(!(XVar)(this.elementsPermissions[elem.Key])))
				{
					continue;
				}
				style = new XVar("");
				if(XVar.Pack(elem.Value["width"]))
				{
					selectors = XVar.Clone(XVar.Array());
				}
				selectors.InitAndSetArrayItem(MVCFunctions.Concat("#dashelement_", elem.Value["elementName"], this.id, " > * > .tab-content > * > * > * > .panel > .panel-body"), null);
				selectors.InitAndSetArrayItem(MVCFunctions.Concat("#dashelement_", elem.Value["elementName"], this.id, " > * > .tab-content > * > * > .panel > .panel-body"), null);
				selectors.InitAndSetArrayItem(MVCFunctions.Concat("#dashelement_", elem.Value["elementName"], this.id, " > .panel > .panel-body"), null);
				selectors.InitAndSetArrayItem(MVCFunctions.Concat("#dashelement_", elem.Value["elementName"], this.id, " > .bs-containedpage > .panel > .panel-body"), null);
				style = MVCFunctions.Concat(style, MVCFunctions.join(new XVar(",\n"), (XVar)(selectors)), " {\r\n\t\t\t\t\t\twidth: ", elem.Value["width"], "px;\r\n\t\t\t\t\t\toverflow-x: auto;\r\n\t\t\t\t\t}");
				if(XVar.Pack(elem.Value["height"]))
				{
					style = MVCFunctions.Concat(style, "#dashelement_", elem.Value["elementName"], this.id, " > .panel > .panel-body,\r\n\t\t\t\t\t\t#dashelement_", elem.Value["elementName"], this.id, " > * > .tab-content > * > * > * > .panel > .panel-body,\r\n\t\t\t\t\t\t#dashelement_", elem.Value["elementName"], this.id, " > * > .tab-content > * > * > .panel > .panel-body,\r\n\t\t\t\t\t\t#dashelement_", elem.Value["elementName"], this.id, " > .bs-containedpage > .panel > .panel-body {\r\n\t\t\t\t\t\theight: ", elem.Value["height"], "px;\r\n\t\t\t\t\t\toverflow-y: auto;\r\n\t\t\t\t\t}");
				}
				if(style != XVar.Pack(""))
				{
					style = XVar.Clone(MVCFunctions.Concat("<style> @media (min-width: 768px){ ", style, " } </style>"));
				}
				contentHtml = new XVar("");
				if(elem.Value["type"] == Constants.DASHBOARD_SNIPPET)
				{
					dynamic snippetData = null;
					snippetData = XVar.Clone(this.callSnippet((XVar)(elem.Value)));
					contentHtml = XVar.Clone(this.getSnippetHtml((XVar)(elem.Value), (XVar)(snippetData)));
				}
				dbElementAttrs = new XVar("");
				if(XVar.Pack(elem.Value["width"]))
				{
					dbElementAttrs = MVCFunctions.Concat(dbElementAttrs, " data-fixed-width");
				}
				if(XVar.Pack(elem.Value["height"]))
				{
					dbElementAttrs = MVCFunctions.Concat(dbElementAttrs, " data-fixed-height");
				}
				dbElementAttrs = MVCFunctions.Concat(dbElementAttrs, " data-dashtype=\"", elem.Value["type"], "\"");
				this.xt.assign((XVar)(MVCFunctions.Concat("db_", elem.Value["elementName"])), (XVar)(contentHtml));
			}

			return null;
		}
		protected virtual XVar callSnippet(dynamic _param_dashElem)
		{
			#region pass-by-value parameters
			dynamic dashElem = XVar.Clone(_param_dashElem);
			#endregion

			dynamic header = null, icon = null, snippetData = XVar.Array();
			snippetData = XVar.Clone(new XVar("body", ""));
			header = new XVar("");
			icon = XVar.Clone(dashElem["item"]["icon"]);
			if(XVar.Pack(GlobalVars.globalEvents.dashSnippetExists((XVar)(dashElem["snippetId"]))))
			{
				dynamic funcName = null;
				funcName = XVar.Clone(MVCFunctions.Concat("event_", dashElem["snippetId"]));
				snippetData.InitAndSetArrayItem(MVCFunctions.callDashboardSnippet((XVar)(funcName), ref icon, ref header), "body");
			}
			snippetData.InitAndSetArrayItem(header, "header");
			snippetData.InitAndSetArrayItem(icon, "icon");
			return snippetData;
		}
		public virtual XVar getSnippetHtml(dynamic _param_elem, dynamic _param_snippetData)
		{
			#region pass-by-value parameters
			dynamic elem = XVar.Clone(_param_elem);
			dynamic snippetData = XVar.Clone(_param_snippetData);
			#endregion

			dynamic html = null, iconHtml = null, style = null;
			iconHtml = XVar.Clone(CommonFunctions.getIconHTML((XVar)(snippetData["icon"])));
			style = XVar.Clone(elem["item"]["snippetStyle"]);
			if(XVar.Pack(!(XVar)(style)))
			{
				style = new XVar("classic");
			}
			if(style == "classic")
			{
				dynamic bodyCont = null, headerCont = null;
				headerCont = XVar.Clone(MVCFunctions.Concat("<div class=\"panel-heading\">", iconHtml, snippetData["header"], "</div>"));
				bodyCont = XVar.Clone(MVCFunctions.Concat("<div class=\"panel-body\">", snippetData["body"], "</div>"));
				html = XVar.Clone(MVCFunctions.Concat("<div class=\"panel panel-", elem["panelStyle"], "\">", headerCont, bodyCont, "</div>"));
			}
			else
			{
				if(style == "left")
				{
					iconHtml = XVar.Clone(MVCFunctions.Concat("<div class=\"r-box-icon\">", iconHtml, "</div>"));
					html = XVar.Clone(MVCFunctions.Concat("<div class=\"r-box-left\">", iconHtml, "<div class=\"r-box-text\">", "<div class=\"r-box-header\">", snippetData["header"], "</div>", "<div class=\"r-box-body\">", snippetData["body"], "</div>", "</div>", "</div>"));
				}
				else
				{
					if(style == "right")
					{
						dynamic footer = null;
						iconHtml = XVar.Clone(MVCFunctions.Concat("<div class=\"r-box-icon\">", iconHtml, "</div>"));
						footer = XVar.Clone((XVar.Pack(snippetData["footer"]) ? XVar.Pack(MVCFunctions.Concat("<div class=\"r-box-footer\">", snippetData["footer"], "</div>")) : XVar.Pack("")));
						html = XVar.Clone(MVCFunctions.Concat("<div class=\"r-box-right\">", "<div class=\"r-box-main\">", "<div class=\"r-box-text\">", "<div class=\"r-box-header\">", snippetData["header"], "</div>", "<div class=\"r-box-body\">", snippetData["body"], "</div>", "</div>", iconHtml, "</div>", footer, "</div>"));
					}
				}
			}
			return html;
		}
		public override XVar clearSessionKeys()
		{
			if((XVar)(MVCFunctions.POSTSize())  && (XVar)(MVCFunctions.postvalue("mode") == "dashsearch"))
			{
				return null;
			}
			base.clearSessionKeys();
			if(XVar.Pack(CommonFunctions.IsEmptyRequest()))
			{
				this.unsetAllPageSessionKeys();
			}
			foreach (KeyValuePair<XVar, dynamic> elem in this.pSet.getDashboardElements().GetEnumerator())
			{
				if(elem.Value["type"] != Constants.DASHBOARD_SEARCH)
				{
					if(elem.Value["type"] == Constants.DASHBOARD_LIST)
					{
						this.unsetAllPageSessionKeys((XVar)(MVCFunctions.Concat(this.tName, "_", elem.Value["elementName"])));
					}
					else
					{
						this.unsetAllPageSessionKeys((XVar)(MVCFunctions.Concat(this.tName, "_", elem.Value["table"])));
					}
				}
			}

			return null;
		}
		public override XVar setSessionVariables()
		{
			base.setSessionVariables();
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_advsearch")] = MVCFunctions.serialize((XVar)(this.searchClauseObj));

			return null;
		}
		public virtual XVar addDashElementsJSAndCSS()
		{

			return null;
		}
		protected virtual XVar hasSingleRecordOrDetailsElement()
		{
			foreach (KeyValuePair<XVar, dynamic> elem in this.pSet.getDashboardElements().GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.elementsPermissions[elem.Key])))
				{
					continue;
				}
				if((XVar)(elem.Value["type"] == Constants.DASHBOARD_RECORD)  || (XVar)(elem.Value["type"] == Constants.DASHBOARD_DETAILS))
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar hasGridElement(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			if(XVar.Pack(this.getGridElement((XVar)(table))))
			{
				return true;
			}
			return false;
		}
		protected virtual XVar getGridElement(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> elem in this.pSet.getDashboardElements().GetEnumerator())
			{
				if((XVar)(elem.Value["table"] == table)  && (XVar)(elem.Value["type"] == Constants.DASHBOARD_LIST))
				{
					return elem.Value;
				}
			}
			return null;
		}
		public virtual XVar addCommonHtml()
		{
			this.body.InitAndSetArrayItem(CommonFunctions.GetBaseScriptsForPage(new XVar(false)), "begin");
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], "<div id=\"search_suggest\" class=\"search_suggest\"></div>");
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
			this.xt.assignbyref(new XVar("body"), (XVar)(this.body));

			return null;
		}
		protected virtual XVar doCommonAssignments()
		{
			this.xt.assign(new XVar("asearch_link"), new XVar(true));
			this.xt.assign(new XVar("advsearchlink_attrs"), (XVar)(MVCFunctions.Concat("id=\"advButton", this.id, "\"")));

			return null;
		}
		public override XVar commonAssign()
		{
			base.commonAssign();

			return null;
		}
		public virtual XVar process()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessDashboard"))))
			{
				this.eventsObject.BeforeProcessDashboard(this);
			}
			this.addButtonHandlers();
			this.addDashElementsJSAndCSS();
			this.addCommonJs();
			this.commonAssign();
			this.doCommonAssignments();
			this.addCommonHtml();
			this.assignSimpleSearch();
			this.updateJsSettings();
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowDashboard"))))
			{
				this.eventsObject.BeforeShowDashboard((XVar)(this.xt), ref this.templatefile, this);
			}
			this.display((XVar)(this.templatefile));

			return null;
		}
		public virtual XVar createElementLinks(dynamic elem)
		{
			elem.InitAndSetArrayItem(XVar.Array(), "parents");
			elem.InitAndSetArrayItem(XVar.Array(), "children");
			foreach (KeyValuePair<XVar, dynamic> e in this.pSet.getDashboardElements().GetEnumerator())
			{
				if(e.Value["elementName"] == elem["elementName"])
				{
					continue;
				}
				if((XVar)(e.Value["table"] == elem["masterTable"])  && (XVar)(DashboardPage.canUpdateDetails((XVar)(e.Value))))
				{
					elem.InitAndSetArrayItem(true, "parents", e.Value["elementName"]);
				}
				if((XVar)(elem["table"] == e.Value["masterTable"])  && (XVar)(DashboardPage.canUpdateDetails((XVar)(elem))))
				{
					elem.InitAndSetArrayItem(true, "children", e.Value["elementName"]);
				}
				if(elem["table"] == e.Value["table"])
				{
					if(XVar.Pack(DashboardPage.canUpdatePeers((XVar)(e.Value))))
					{
						elem.InitAndSetArrayItem(true, "parents", e.Value["elementName"]);
					}
					if(XVar.Pack(DashboardPage.canUpdatePeers((XVar)(elem))))
					{
						elem.InitAndSetArrayItem(true, "children", e.Value["elementName"]);
					}
				}
			}

			return null;
		}
		public virtual XVar getMajorElements()
		{
			dynamic dashboardTables = XVar.Array(), elements = XVar.Array(), majorElements = XVar.Array();
			majorElements = XVar.Clone(XVar.Array());
			dashboardTables = XVar.Clone(XVar.Array());
			elements = XVar.Clone(this.pSet.getDashboardElements());
			foreach (KeyValuePair<XVar, dynamic> e in elements.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(dashboardTables.KeyExists(e.Value["table"]))))
				{
					dashboardTables.InitAndSetArrayItem(XVar.Array(), e.Value["table"]);
				}
				dashboardTables.InitAndSetArrayItem(e.Key, e.Value["table"], null);
			}
			foreach (KeyValuePair<XVar, dynamic> t in dashboardTables.GetEnumerator())
			{
				dynamic major = null;
				major = new XVar("");
				foreach (KeyValuePair<XVar, dynamic> i in t.Value.GetEnumerator())
				{
					if((XVar)(elements[i.Value]["type"] == Constants.DASHBOARD_MAP)  && (XVar)(elements[i.Value]["updateMoved"]))
					{
						major = XVar.Clone(elements[i.Value]["elementName"]);
						break;
					}
				}
				if(major == XVar.Pack(""))
				{
					foreach (KeyValuePair<XVar, dynamic> i in t.Value.GetEnumerator())
					{
						dynamic elementType = null;
						elementType = XVar.Clone(elements[i.Value]["type"]);
						if((XVar)((XVar)((XVar)((XVar)(elementType == Constants.DASHBOARD_LIST)  || (XVar)(elementType == Constants.DASHBOARD_CHART))  || (XVar)(elementType == Constants.DASHBOARD_REPORT))  || (XVar)(elementType == Constants.DASHBOARD_CALENDAR))  || (XVar)(elementType == Constants.DASHBOARD_GANTT))
						{
							major = XVar.Clone(elements[i.Value]["elementName"]);
							break;
						}
					}
				}
				if(major == XVar.Pack(""))
				{
					foreach (KeyValuePair<XVar, dynamic> i in t.Value.GetEnumerator())
					{
						if(elements[i.Value]["type"] == Constants.DASHBOARD_MAP)
						{
							major = XVar.Clone(elements[i.Value]["elementName"]);
							break;
						}
					}
				}
				if(major == XVar.Pack(""))
				{
					foreach (KeyValuePair<XVar, dynamic> i in t.Value.GetEnumerator())
					{
						if(elements[i.Value]["type"] != Constants.DASHBOARD_RECORD)
						{
							continue;
						}
						if((XVar)(MVCFunctions.in_array(new XVar(Constants.PAGE_EDIT), (XVar)(elements[i.Value]["tabsPageTypes"])))  || (XVar)(MVCFunctions.in_array(new XVar(Constants.PAGE_VIEW), (XVar)(elements[i.Value]["tabsPageTypes"]))))
						{
							major = XVar.Clone(elements[i.Value]["elementName"]);
							break;
						}
					}
				}
				if(major != XVar.Pack(""))
				{
					majorElements.InitAndSetArrayItem(true, major);
				}
			}
			return majorElements;
		}
		public static XVar canUpdateDetails(dynamic elem)
		{
			return DashboardPage.canUpdatePeers((XVar)(elem));
		}
		public static XVar canUpdatePeers(dynamic elem)
		{
			return (XVar)((XVar)((XVar)((XVar)(elem["type"] == Constants.DASHBOARD_LIST)  || (XVar)(elem["type"] == Constants.DASHBOARD_CHART))  || (XVar)(elem["type"] == Constants.DASHBOARD_REPORT))  || (XVar)(elem["type"] == Constants.DASHBOARD_RECORD))  || (XVar)(elem["type"] == Constants.DASHBOARD_MAP);
		}
		protected virtual XVar fillElementPermissions()
		{
			foreach (KeyValuePair<XVar, dynamic> elem in this.pSet.getDashboardElements().GetEnumerator())
			{
				dynamic newElem = null, permissions = null;
				this.createElementLinks((XVar)(elem.Value));
				permissions = new XVar(false);
				newElem = XVar.Clone(elem.Value);
				if(elem.Value["type"] == Constants.DASHBOARD_RECORD)
				{
					foreach (KeyValuePair<XVar, dynamic> pageType in elem.Value["tabsPageTypes"].GetEnumerator())
					{
						if(XVar.Pack(this.CheckPermissions((XVar)(elem.Value["table"]), (XVar)(pageType.Value))))
						{
							permissions = new XVar(true);
							break;
						}
					}
				}
				else
				{
					if(elem.Value["type"] == Constants.DASHBOARD_DETAILS)
					{
						dynamic detailsTables = XVar.Array(), eset = null;
						eset = XVar.Clone(new ProjectSettings((XVar)(elem.Value["table"])));
						detailsTables = XVar.Clone(eset.getAvailableDetailsTables());
						foreach (KeyValuePair<XVar, dynamic> d in detailsTables.GetEnumerator())
						{
							if(XVar.Pack(MVCFunctions.in_array((XVar)(d.Value), (XVar)(elem.Value["notUsedDetailTables"]))))
							{
								continue;
							}
							permissions = new XVar(true);
							break;
						}
					}
					else
					{
						if((XVar)((XVar)(elem.Value["type"] == Constants.DASHBOARD_CHART)  || (XVar)(elem.Value["type"] == Constants.DASHBOARD_REPORT))  || (XVar)(elem.Value["type"] == Constants.DASHBOARD_SEARCH))
						{
							permissions = XVar.Clone(this.CheckPermissions((XVar)(elem.Value["table"]), new XVar("list")));
						}
						else
						{
							if(elem.Value["type"] == Constants.DASHBOARD_LIST)
							{
								permissions = XVar.Clone(this.CheckPermissions((XVar)(elem.Value["table"]), new XVar("list")));
							}
							else
							{
								if(elem.Value["type"] == Constants.DASHBOARD_CALENDAR)
								{
									permissions = XVar.Clone(this.CheckPermissions((XVar)(elem.Value["table"]), new XVar("list")));
								}
								else
								{
									if(elem.Value["type"] == Constants.DASHBOARD_GANTT)
									{
										permissions = XVar.Clone(this.CheckPermissions((XVar)(elem.Value["table"]), new XVar("list")));
									}
									else
									{
										if(elem.Value["type"] == Constants.DASHBOARD_MAP)
										{
											permissions = XVar.Clone(this.CheckPermissions((XVar)(elem.Value["table"]), new XVar("list")));
										}
										else
										{
											if(elem.Value["type"] == Constants.DASHBOARD_SNIPPET)
											{
												permissions = new XVar(true);
											}
										}
									}
								}
							}
						}
					}
				}
				this.elementsPermissions.InitAndSetArrayItem(permissions, elem.Key);
			}

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

			dynamic majorElements = XVar.Array(), settings = XVar.Array();
			settings = XVar.Clone(base.buildJsTableSettings((XVar)(table), (XVar)(pSet)));
			settings.InitAndSetArrayItem(XVar.Array(), "dashElements");
			majorElements = XVar.Clone(this.getMajorElements());
			foreach (KeyValuePair<XVar, dynamic> elem in this.pSet.getDashboardElements().GetEnumerator())
			{
				dynamic eset = null, newElem = XVar.Array();
				if(XVar.Pack(!(XVar)(this.elementsPermissions[elem.Key])))
				{
					continue;
				}
				this.createElementLinks((XVar)(elem.Value));
				newElem = XVar.Clone(elem.Value);
				if(elem.Value["type"] == Constants.DASHBOARD_RECORD)
				{
					dynamic gridElem = XVar.Array();
					newElem.InitAndSetArrayItem(XVar.Array(), "tabsPageTypes");
					foreach (KeyValuePair<XVar, dynamic> pageType in elem.Value["tabsPageTypes"].GetEnumerator())
					{
						if(XVar.Pack(this.CheckPermissions((XVar)(elem.Value["table"]), (XVar)(pageType.Value))))
						{
							newElem.InitAndSetArrayItem(pageType.Value, "tabsPageTypes", null);
						}
					}
					gridElem = XVar.Clone(this.getGridElement((XVar)(elem.Value["table"])));
					if(XVar.Pack(gridElem))
					{
						eset = XVar.Clone(new ProjectSettings((XVar)(elem.Value["table"]), new XVar(Constants.PAGE_LIST), (XVar)(gridElem["pageName"])));
						newElem.InitAndSetArrayItem(eset.spreadsheetGrid(), "spreadsheetGrid");
						newElem.InitAndSetArrayItem(gridElem["pageName"], "hostListPage");
					}
				}
				else
				{
					if(elem.Value["type"] == Constants.DASHBOARD_DETAILS)
					{
						dynamic detailsTables = XVar.Array();
						eset = XVar.Clone(new ProjectSettings((XVar)(elem.Value["table"])));
						detailsTables = XVar.Clone(eset.getAvailableDetailsTables());
						newElem.InitAndSetArrayItem(XVar.Array(), "details");
						foreach (KeyValuePair<XVar, dynamic> d in detailsTables.GetEnumerator())
						{
							dynamic detailsData = XVar.Array();
							if(XVar.Pack(MVCFunctions.in_array((XVar)(d.Value), (XVar)(elem.Value["notUsedDetailTables"]))))
							{
								continue;
							}
							detailsData = XVar.Clone(XVar.Array());
							detailsData.InitAndSetArrayItem(d.Value, "dDataSourceTable");
							detailsData.InitAndSetArrayItem(elem.Value["details"][d.Value]["pageName"], "pageName");
							detailsData.InitAndSetArrayItem(CommonFunctions.GetTableURL((XVar)(d.Value)), "dShortTable");
							detailsData.InitAndSetArrayItem(Labels.getTableCaption((XVar)(d.Value)), "dCaptionTable");
							detailsData.InitAndSetArrayItem(ProjectSettings.defaultPageType((XVar)(CommonFunctions.GetEntityType((XVar)(d.Value)))), "dType");
							newElem.InitAndSetArrayItem(detailsData, "details", null);
						}
					}
					else
					{
						if(elem.Value["type"] == Constants.DASHBOARD_MAP)
						{
							if(XVar.Pack(!(XVar)(elem.Value["updateMoved"])))
							{
								newElem.InitAndSetArrayItem(!(XVar)(this.hasGridElement((XVar)(elem.Value["table"]))), "updateMoved");
							}
						}
					}
				}
				if(XVar.Pack(majorElements.KeyExists(newElem["elementName"])))
				{
					newElem.InitAndSetArrayItem(true, "major");
				}
				settings.InitAndSetArrayItem(newElem, "dashElements", elem.Key);
			}
			return settings;
		}
		protected virtual XVar updateJsSettings()
		{
			foreach (KeyValuePair<XVar, dynamic> elem in this.pSet.getDashboardElements().GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(elem.Value["table"])))
				{
					continue;
				}
				this.jsSettings.InitAndSetArrayItem(CommonFunctions.GetTableURL((XVar)(elem.Value["table"])), "tableSettings", elem.Value["table"], "shortTName");
			}

			return null;
		}
	}
}
