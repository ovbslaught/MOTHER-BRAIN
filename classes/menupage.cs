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
	public partial class MenuPage : RunnerPage
	{
		protected static bool skipMenuPageCtor = false;
		public MenuPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipMenuPageCtor)
			{
				skipMenuPageCtor = false;
				return;
			}
		}
		public virtual XVar process()
		{
			dynamic redirect = null;
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeProcessMenu"))))
			{
				GlobalVars.globalEvents.BeforeProcessMenu(this);
			}
			redirect = XVar.Clone(this.getRedirectForMenuPage());
			if(XVar.Pack(redirect))
			{
				MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", redirect)));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
				return null;
			}
			this.commonAssign();
			this.doCommonAssignments();
			this.hideWelcomeItemsIfEmpty((XVar)(this.pSet.welcomeItems()));
			this.addButtonHandlers();
			this.addCommonJs();
			this.displayMenuPage();

			return null;
		}
		public virtual XVar getAllowedWelcomeMenuItems(dynamic _param_itemsData)
		{
			#region pass-by-value parameters
			dynamic itemsData = XVar.Clone(_param_itemsData);
			#endregion

			dynamic allowedItemCnt = null;
			if(XVar.Pack(!(XVar)(itemsData)))
			{
				return 0;
			}
			allowedItemCnt = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> itemData in itemsData.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(itemData.Value["menutItem"])))
				{
					continue;
				}
				if(XVar.Pack(!(XVar)(itemData.Value["group"])))
				{
					if((XVar)((XVar)(itemData.Value["linkType"] == 0)  && (XVar)(itemData.Value["table"]))  && (XVar)(itemData.Value["page"]))
					{
						if(XVar.Pack(CommonFunctions.menuLinkAvailable((XVar)(itemData.Value["table"]), (XVar)(itemData.Value["page"]), (XVar)(itemData.Value["pageId"]))))
						{
							allowedItemCnt++;
						}
					}
					else
					{
						if(itemData.Value["linkType"] == 1)
						{
							allowedItemCnt += 2;
						}
					}
					continue;
				}
				else
				{
					allowedItemCnt += this.getAllowedWelcomeMenuItems((XVar)(itemData.Value["items"]));
				}
			}
			return allowedItemCnt;
		}
		public override XVar isShowMenu()
		{
			dynamic allowedMenuItems = null;
			if(XVar.Pack(!(XVar)(this.pSet.welcomePageSkip())))
			{
				return true;
			}
			if((XVar)((XVar)((XVar)((XVar)(!(XVar)(this.menuAppearInLayout()))  && (XVar)(this.pageType != Constants.PAGE_MENU))  && (XVar)(this.pageType != Constants.PAGE_ADD))  && (XVar)(this.pageType != Constants.PAGE_VIEW))  && (XVar)(this.pageType != Constants.PAGE_EDIT))
			{
				return false;
			}
			allowedMenuItems = XVar.Clone(this.getAllowedWelcomeMenuItems((XVar)(this.pSet.welcomeItems())));
			if(1 < allowedMenuItems)
			{
				return true;
			}
			return false;
		}
		public virtual XVar getRedirectForMenuPage()
		{
			dynamic menuNodes = XVar.Array(), menuObject = null, redirect = null;
			if(XVar.Pack(this.isShowMenu()))
			{
				return "";
			}
			redirect = new XVar("");
			menuObject = XVar.Clone(RunnerMenu.getMenuObject(new XVar("main")));
			menuNodes = XVar.Clone(menuObject.collectNodes());
			foreach (KeyValuePair<XVar, dynamic> mNode in menuNodes.GetEnumerator())
			{
				dynamic pageType = null, table = null;
				table = XVar.Clone(mNode.Value.table);
				pageType = XVar.Clone(mNode.Value.pageType);
				if(mNode.Value.linkType == "Internal")
				{
					if(XVar.Pack(this.isUserHaveTablePerm((XVar)(table), (XVar)(pageType))))
					{
						dynamic var_type = null;
						var_type = XVar.Clone(this.getPermisType((XVar)(pageType)));
						if(var_type == "A")
						{
							redirect = new XVar("add");
						}
						if(var_type == "E")
						{
							redirect = new XVar("edit");
						}
						else
						{
							if((XVar)(pageType == "list")  && (XVar)(var_type == "S"))
							{
								redirect = new XVar("list");
							}
							else
							{
								if((XVar)(pageType == "report")  && (XVar)(var_type == "S"))
								{
									redirect = new XVar("report");
								}
								else
								{
									if((XVar)(pageType == "chart")  && (XVar)(var_type == "S"))
									{
										redirect = new XVar("chart");
									}
									else
									{
										if((XVar)(pageType == "view")  && (XVar)(var_type == "S"))
										{
											redirect = new XVar("view");
										}
										else
										{
											if((XVar)(pageType == "dashboard")  && (XVar)(var_type == "S"))
											{
												redirect = new XVar("dashboard");
											}
										}
									}
								}
							}
						}
						redirect = XVar.Clone(MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(table))), (XVar)(redirect)));
					}
				}
			}
			if(XVar.Pack(!(XVar)(redirect)))
			{
				if(XVar.Pack(Security.isAdmin()))
				{
					redirect = XVar.Clone(MVCFunctions.GetTableLink(new XVar("admin_rights"), new XVar("list")));
				}
				else
				{
					if(XVar.Pack(ProjectSettings.getProjectValue(new XVar("enableWebreports"))))
					{
						redirect = XVar.Clone(MVCFunctions.GetTableLink(new XVar("webreport")));
					}
				}
			}
			return redirect;
		}
		protected virtual XVar doCommonAssignments()
		{
			this.setLangParams();
			this.xt.assign(new XVar("menu_block"), new XVar(true));
			this.assignBody();

			return null;
		}
		public virtual XVar hideWelcomeItemsIfEmpty(dynamic _param_itemsData)
		{
			#region pass-by-value parameters
			dynamic itemsData = XVar.Clone(_param_itemsData);
			#endregion

			dynamic hide = null;
			hide = new XVar(true);
			foreach (KeyValuePair<XVar, dynamic> itemData in itemsData.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(itemData.Value["menutItem"])))
				{
					hide = new XVar(false);
					continue;
				}
				if(XVar.Pack(!(XVar)(itemData.Value["group"])))
				{
					if((XVar)(itemData.Value["table"])  && (XVar)(itemData.Value["page"]))
					{
						if(XVar.Pack(CommonFunctions.menuLinkAvailable((XVar)(itemData.Value["table"]), (XVar)(itemData.Value["page"]), (XVar)(itemData.Value["pageId"]))))
						{
							hide = new XVar(false);
						}
						else
						{
							this.xt.displayItemHidden((XVar)(itemData.Key));
						}
					}
					else
					{
						hide = new XVar(false);
					}
					continue;
				}
				if(XVar.Pack(!(XVar)(this.hideWelcomeGroupIfEmpty((XVar)(itemData.Key), (XVar)(itemData.Value)))))
				{
					hide = new XVar(false);
				}
			}
			return hide;
		}
		public virtual XVar hideWelcomeGroupIfEmpty(dynamic _param_grId, dynamic _param_grData)
		{
			#region pass-by-value parameters
			dynamic grId = XVar.Clone(_param_grId);
			dynamic grData = XVar.Clone(_param_grData);
			#endregion

			dynamic hide = null;
			if((XVar)(!(XVar)(grData["items"]))  || (XVar)(MVCFunctions.count(grData["items"]) < 1))
			{
				this.xt.displayItemHidden((XVar)(grId));
				return true;
			}
			hide = XVar.Clone(this.hideWelcomeItemsIfEmpty((XVar)(grData["items"])));
			if(XVar.Pack(hide))
			{
				this.xt.displayItemHidden((XVar)(grId));
			}
			return hide;
		}
		public virtual XVar displayMenuPage()
		{
			dynamic templatefile = null;
			templatefile = XVar.Clone(this.templatefile);
			if(XVar.Pack(GlobalVars.globalEvents.exists(new XVar("BeforeShowMenu"))))
			{
				GlobalVars.globalEvents.BeforeShowMenu((XVar)(this.xt), ref templatefile, this);
			}
			this.display((XVar)(templatefile));

			return null;
		}
	}
}
