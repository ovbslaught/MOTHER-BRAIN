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
	public partial class SearchPage : RunnerPage
	{
		public dynamic searchControllerId;
		public dynamic searchControlBuilder;
		public dynamic tableSettings = XVar.Array();
		public dynamic reportName;
		public dynamic chartName;
		public dynamic needSettings = XVar.Pack(false);
		public dynamic ctrlField;
		public dynamic extraPageParams;
		protected static bool skipSearchPageCtor = false;
		public SearchPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipSearchPageCtor)
			{
				skipSearchPageCtor = false;
				return;
			}
			this.headerForms = XVar.Clone(new XVar(0, "top"));
			this.footerForms = XVar.Clone(new XVar(0, "below-grid"));
			this.bodyForms = XVar.Clone(new XVar(0, "grid"));
		}
		public override XVar init()
		{
			base.init();
			this.searchControlBuilder = XVar.Clone(new AdvancedSearchControl((XVar)(this.searchControllerId), (XVar)(this.tName), (XVar)(this.searchClauseObj), this));

			return null;
		}
		protected override XVar assignSessionPrefix()
		{
			if(this.mode == Constants.SEARCH_DASHBOARD)
			{
				this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.dashTName, "_", this.tName));
			}
			else
			{
				this.sessionPrefix = XVar.Clone(this.tName);
			}

			return null;
		}
		public virtual XVar displaySearchControl()
		{
			dynamic ctrlBlockArr = XVar.Array(), defaultValue = null, resArr = XVar.Array();
			this.searchControlBuilder = XVar.Clone(new PanelSearchControl((XVar)(this.searchControllerId), (XVar)(this.tName), (XVar)(this.searchClauseObj), this));
			defaultValue = XVar.Clone(this.pSet.getSearchDefaultValue((XVar)(this.ctrlField)));
			ctrlBlockArr = XVar.Clone(this.searchControlBuilder.buildSearchCtrlBlockArr((XVar)(this.id), (XVar)(this.ctrlField), new XVar(0), new XVar(""), new XVar(false), new XVar(true), (XVar)(defaultValue), new XVar("")));
			resArr = XVar.Clone(XVar.Array());
			resArr.InitAndSetArrayItem(MVCFunctions.trim((XVar)(this.xt.call_func((XVar)(ctrlBlockArr["searchcontrol"])))), "control1");
			resArr.InitAndSetArrayItem(MVCFunctions.trim((XVar)(this.xt.call_func((XVar)(ctrlBlockArr["searchcontrol1"])))), "control2");
			resArr.InitAndSetArrayItem(MVCFunctions.trim((XVar)(ctrlBlockArr["searchtype"])), "comboHtml");
			resArr.InitAndSetArrayItem(MVCFunctions.trim((XVar)(ctrlBlockArr["delCtrlButt"])), "delButt");
			resArr.InitAndSetArrayItem(MVCFunctions.trim((XVar)(this.searchControlBuilder.getDelButtonId((XVar)(this.ctrlField), (XVar)(this.id)))), "delButtId");
			resArr.InitAndSetArrayItem(MVCFunctions.trim((XVar)(this.id)), "divInd");
			resArr.InitAndSetArrayItem(CommonFunctions.GetFieldLabel((XVar)(MVCFunctions.GoodFieldName((XVar)(this.tName))), (XVar)(MVCFunctions.GoodFieldName((XVar)(this.ctrlField)))), "fLabel");
			resArr.InitAndSetArrayItem(this.controlsMap["controls"], "ctrlMap");
			if(this.needSettings == "true")
			{
				this.fillSettings();
				resArr.InitAndSetArrayItem(this.jsSettings, "settings");
			}
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(resArr)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public virtual XVar process()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessSearch"))))
			{
				this.eventsObject.BeforeProcessSearch(this);
			}
			this.prepareSearchRadio();
			this.prepareFields();
			this.addButtonHandlers();
			this.addCommonJs();
			this.doCommonAssignments();
			this.prepareCollapseButton();
			this.fillSetCntrlMaps();
			this.displaySearchPage();

			return null;
		}
		protected virtual XVar prepareSearchRadio()
		{
			dynamic searchRadio = XVar.Array();
			searchRadio = XVar.Clone(this.searchControlBuilder.getSearchRadio());
			this.xt.assign_section(new XVar("all_checkbox_label"), (XVar)(searchRadio["all_checkbox_label"][0]), (XVar)(searchRadio["all_checkbox_label"][1]));
			this.xt.assign_section(new XVar("any_checkbox_label"), (XVar)(searchRadio["any_checkbox_label"][0]), (XVar)(searchRadio["any_checkbox_label"][1]));
			this.xt.assignbyref(new XVar("all_checkbox"), (XVar)(searchRadio["all_checkbox"]));
			this.xt.assignbyref(new XVar("any_checkbox"), (XVar)(searchRadio["any_checkbox"]));

			return null;
		}
		protected virtual XVar prepareFields()
		{
			foreach (KeyValuePair<XVar, dynamic> field in this.pSet.getAdvSearchFields().GetEnumerator())
			{
				dynamic ctrlBlockArr = XVar.Array(), ctrlInd = null, firstElementId = null, firstFieldParams = XVar.Array(), gfield = null, isFieldNeedSecCtrl = null, srchFields = XVar.Array(), srchTypeFull = null;
				gfield = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(field.Value)));
				srchFields = XVar.Clone(this.searchClauseObj.getSearchCtrlParams((XVar)(field.Value)));
				firstFieldParams = XVar.Clone(XVar.Array());
				if(XVar.Pack(MVCFunctions.count(srchFields)))
				{
					firstFieldParams = XVar.Clone(srchFields[0]);
				}
				else
				{
					firstFieldParams.InitAndSetArrayItem(field.Value, "fName");
					firstFieldParams.InitAndSetArrayItem("", "eType");
					firstFieldParams.InitAndSetArrayItem(this.pSet.getSearchDefaultValue((XVar)(field.Value)), "value1");
					firstFieldParams.InitAndSetArrayItem("", "value2");
					firstFieldParams.InitAndSetArrayItem(false, "not");
					firstFieldParams.InitAndSetArrayItem(this.pSet.getDefaultSearchOption((XVar)(field.Value)), "opt");
					firstFieldParams.InitAndSetArrayItem(false, "not");
					if(MVCFunctions.substr((XVar)(firstFieldParams["opt"]), new XVar(0), new XVar(4)) == "NOT ")
					{
						firstFieldParams.InitAndSetArrayItem(MVCFunctions.substr((XVar)(firstFieldParams["opt"]), new XVar(4)), "opt");
						firstFieldParams.InitAndSetArrayItem(true, "not");
					}
				}
				ctrlBlockArr = XVar.Clone(this.searchControlBuilder.buildSearchCtrlBlockArr((XVar)(this.id), (XVar)(firstFieldParams["fName"]), new XVar(0), (XVar)(firstFieldParams["opt"]), (XVar)(firstFieldParams["not"]), new XVar(false), (XVar)(firstFieldParams["value1"]), (XVar)(firstFieldParams["value2"])));
				if(firstFieldParams["opt"] == "")
				{
					firstFieldParams.InitAndSetArrayItem(this.pSet.getDefaultSearchOption((XVar)(firstFieldParams["fName"])), "opt");
				}
				srchTypeFull = XVar.Clone(this.searchControlBuilder.getCtrlSearchType((XVar)(firstFieldParams["fName"]), (XVar)(this.id), new XVar(0), (XVar)(firstFieldParams["opt"]), (XVar)(firstFieldParams["not"]), new XVar(true), new XVar(true)));
				if(XVar.Pack(this.is508))
				{
					this.xt.assign_section((XVar)(MVCFunctions.Concat(gfield, "_label")), (XVar)(MVCFunctions.Concat("<label for=\"", this.getInputElementId((XVar)(field.Value), (XVar)(this.pSet)), "\">")), new XVar("</label>"));
				}
				else
				{
					this.xt.assign((XVar)(MVCFunctions.Concat(gfield, "_label")), new XVar(true));
				}
				firstElementId = XVar.Clone(this.getControl((XVar)(field.Value), (XVar)(this.id)).getFirstElementId());
				if(XVar.Pack(firstElementId))
				{
					this.xt.assign((XVar)(MVCFunctions.Concat("labelfor_", gfield)), (XVar)(firstElementId));
				}
				this.xt.assign((XVar)(MVCFunctions.Concat(gfield, "_fieldblock")), new XVar(true));
				this.xt.assignbyref((XVar)(MVCFunctions.Concat(gfield, "_editcontrol")), (XVar)(ctrlBlockArr["searchcontrol"]));
				this.xt.assign((XVar)(MVCFunctions.Concat(gfield, "_notbox")), (XVar)(ctrlBlockArr["notbox"]));
				this.xt.assignbyref((XVar)(MVCFunctions.Concat(gfield, "_editcontrol1")), (XVar)(ctrlBlockArr["searchcontrol1"]));
				this.xt.assign((XVar)(MVCFunctions.Concat("searchtype_", gfield)), (XVar)(ctrlBlockArr["searchtype"]));
				this.xt.assign((XVar)(MVCFunctions.Concat("searchtypefull_", gfield)), (XVar)(srchTypeFull));
				isFieldNeedSecCtrl = XVar.Clone(this.searchControlBuilder.isNeedSecondCtrl((XVar)(field.Value)));
				ctrlInd = new XVar(0);
				if(XVar.Pack(isFieldNeedSecCtrl))
				{
					this.controlsMap.InitAndSetArrayItem(new XVar("fName", field.Value, "recId", this.id, "ctrlsMap", new XVar(0, ctrlInd, 1, ctrlInd + 1)), "search", "searchBlocks", null);
					ctrlInd += 2;
				}
				else
				{
					this.controlsMap.InitAndSetArrayItem(new XVar("fName", field.Value, "recId", this.id, "ctrlsMap", new XVar(0, ctrlInd)), "search", "searchBlocks", null);
					ctrlInd++;
				}
			}

			return null;
		}
		protected virtual XVar doCommonAssignments()
		{
			if(XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.SEARCH_SIMPLE)))
			{
				this.headerCommonAssign();
			}
			else
			{
				this.xt.assign(new XVar("menu_chiddenattr"), new XVar("data-hidden"));
			}
			this.setLangParams();
			if(!XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.SEARCH_DASHBOARD)))
			{
				this.assignBody();
			}
			this.xt.assign(new XVar("contents_block"), new XVar(true));
			this.xt.assign(new XVar("conditions_block"), new XVar(true));
			this.xt.assign(new XVar("search_button"), new XVar(true));
			this.xt.assign(new XVar("reset_button"), new XVar(true));
			this.xt.assign(new XVar("searchbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"searchButton", this.id, "\"")));
			this.xt.assign(new XVar("resetbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"resetButton", this.id, "\"")));
			this.xt.assign(new XVar("searchheader"), new XVar(true));
			this.xt.assign(new XVar("searchbuttons"), new XVar(true));
			if(!XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.SEARCH_DASHBOARD)))
			{
				this.xt.assign(new XVar("back_button"), new XVar(true));
				this.xt.assign(new XVar("backbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"backButton", this.id, "\"")));
			}
			if(XVar.Pack(this.reportName))
			{
				this.xt.assign(new XVar("dynamic"), new XVar("true"));
				this.xt.assign(new XVar("rname"), (XVar)(this.reportName));
			}
			if(XVar.Pack(this.chartName))
			{
				this.xt.assign(new XVar("dynamic"), new XVar("true"));
				this.xt.assign(new XVar("cname"), (XVar)(this.chartName));
			}
			this.xt.assign(new XVar("cancel_button"), (XVar)(this.mode == Constants.SEARCH_POPUP));
			this.xt.assign(new XVar("back_button"), (XVar)(this.mode != Constants.SEARCH_POPUP));

			return null;
		}
		protected virtual XVar displaySearchPage()
		{
			dynamic templatefile = null;
			templatefile = XVar.Clone(this.templatefile);
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowSearch"))))
			{
				this.eventsObject.BeforeShowSearch((XVar)(this.xt), ref templatefile, this);
			}
			if(this.mode == Constants.SEARCH_SIMPLE)
			{
				this.display((XVar)(templatefile));
				return null;
			}
			this.xt.assign(new XVar("header"), new XVar(false));
			this.xt.assign(new XVar("footer"), new XVar(false));
			this.xt.assign(new XVar("body"), (XVar)(this.body));
			this.displayAJAX((XVar)(templatefile), (XVar)(this.flyId + 1));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public static XVar getExtraPageParams()
		{
			dynamic prms = XVar.Array();
			prms = XVar.Clone(XVar.Array());
			prms.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("x")), "x");
			prms.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("y")), "y");
			prms.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("data")), "data");
			if((XVar)((XVar)(!(XVar)(MVCFunctions.strlen((XVar)(prms["x"]))))  && (XVar)(!(XVar)(MVCFunctions.strlen((XVar)(prms["y"])))))  && (XVar)(!(XVar)(MVCFunctions.strlen((XVar)(prms["data"])))))
			{
				return XVar.Array();
			}
			prms.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("op")), "op");
			prms.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("xtype")), "xtype");
			prms.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("ytype")), "ytype");
			return prms;
		}
		protected override XVar checkShowBreadcrumbs()
		{
			return this.mode == Constants.SEARCH_SIMPLE;
		}
		public static XVar readSearchModeFromRequest()
		{
			dynamic mode = null;
			mode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(mode == "dashsearch")
			{
				return Constants.SEARCH_DASHBOARD;
			}
			if(mode == "inlineLoadCtrl")
			{
				return Constants.SEARCH_LOAD_CONTROL;
			}
			if(XVar.Pack(MVCFunctions.postvalue(new XVar("onFly"))))
			{
				return Constants.SEARCH_POPUP;
			}
			return Constants.SEARCH_SIMPLE;
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
			if(XVar.Pack(this.extraPageParams))
			{
				settings.InitAndSetArrayItem(this.extraPageParams, "extraSearchPageParams");
			}
			settings.InitAndSetArrayItem(this.pSet.getAdvSearchFields(), "searchPageFields");
			return settings;
		}
		protected override XVar buildJsGlobalSettings()
		{
			dynamic settings = XVar.Array();
			settings = XVar.Clone(base.buildJsGlobalSettings());
			foreach (KeyValuePair<XVar, dynamic> field in this.pSet.getAdvSearchFields().GetEnumerator())
			{
				dynamic lookupTable = null;
				lookupTable = XVar.Clone(this.pSet.getLookupTable((XVar)(field.Value)));
				if(XVar.Pack(lookupTable))
				{
					settings.InitAndSetArrayItem(CommonFunctions.GetTableURL((XVar)(lookupTable)), "shortTNames", lookupTable);
				}
			}
			return settings;
		}
	}
}
