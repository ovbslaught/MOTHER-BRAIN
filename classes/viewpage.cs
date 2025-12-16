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
	public partial class ViewPage : RunnerPage
	{
		public dynamic jsKeys = XVar.Array();
		public dynamic keyFields = XVar.Array();
		public dynamic nextData;
		public dynamic prevData;
		protected static bool skipViewPageCtor = false;
		public ViewPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipViewPageCtor)
			{
				skipViewPageCtor = false;
				return;
			}
			this.setKeysForJs();
			this.headerForms = XVar.Clone(new XVar(0, "top"));
			this.footerForms = XVar.Clone(new XVar(0, "below-grid"));
			this.bodyForms = XVar.Clone(new XVar(0, "above-grid", 1, "grid"));
		}
		protected override XVar assignSessionPrefix()
		{
			if((XVar)(this.mode == Constants.VIEW_DASHBOARD)  || (XVar)((XVar)(this.mode == Constants.VIEW_POPUP)  && (XVar)(this.dashTName)))
			{
				this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.dashTName, "_", this.tName));
				return null;
			}
			base.assignSessionPrefix();

			return null;
		}
		public override XVar setSessionVariables()
		{
			base.setSessionVariables();
			XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_advsearch")] = MVCFunctions.serialize((XVar)(this.searchClauseObj));

			return null;
		}
		public static XVar processEditPageSecurity(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic messageLink = null, pageMode = null;
			pageMode = XVar.Clone(ViewPage.readViewModeFromRequest());
			messageLink = new XVar("");
			if((XVar)(!(XVar)(CommonFunctions.isLogged()))  || (XVar)(Security.isGuest()))
			{
				messageLink = XVar.Clone(MVCFunctions.Concat(" <a href='#' id='loginButtonContinue'>", CommonFunctions.mlang_message(new XVar("SESSION_EXPIRED3")), "</a>"));
			}
			if(XVar.Pack(!(XVar)(Security.processPageSecurity((XVar)(table), new XVar("S"), (XVar)(pageMode != Constants.VIEW_SIMPLE), (XVar)(messageLink)))))
			{
				return false;
			}
			return true;
		}
		public virtual XVar setKeys(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			this.data = new XVar(null);
			this.keys = XVar.Clone(keys);
			this.setKeysForJs();

			return null;
		}
		protected virtual XVar prepareJsSettings()
		{
			if(this.mode == Constants.VIEW_DASHBOARD)
			{
				this.pageData.InitAndSetArrayItem(this.getDetailTablesMasterKeys((XVar)(this.getCurrentRecordInternal())), "detailsMasterKeys");
			}

			return null;
		}
		public virtual XVar setKeysForJs()
		{
			dynamic i = null;
			i = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> value in this.keys.GetEnumerator())
			{
				this.jsKeys.InitAndSetArrayItem(value.Value, i++);
			}

			return null;
		}
		public override XVar getCurrentRecord()
		{
			dynamic data = XVar.Array(), oldData = XVar.Array(), tdata = null;
			tdata = XVar.Clone(this.getCurrentRecordInternal());
			data = XVar.Clone(tdata);
			oldData = XVar.Clone(data);
			foreach (KeyValuePair<XVar, dynamic> val in oldData.GetEnumerator())
			{
				dynamic viewFormat = null;
				viewFormat = XVar.Clone(this.pSet.getViewFormat((XVar)(val.Key)));
				if((XVar)((XVar)(viewFormat == Constants.FORMAT_DATABASE_FILE)  || (XVar)(viewFormat == Constants.FORMAT_DATABASE_IMAGE))  || (XVar)(viewFormat == Constants.FORMAT_FILE_IMAGE))
				{
					data.InitAndSetArrayItem((XVar.Pack(data[val.Key]) ? XVar.Pack(true) : XVar.Pack(false)), val.Key);
				}
			}
			return data;
		}
		public virtual XVar getCurrentRecordInternal()
		{
			dynamic dc = null, fetchedArray = null;
			if(XVar.Pack(!(XVar)(XVar.Equals(XVar.Pack(this.data), XVar.Pack(null)))))
			{
				return this.data;
			}
			dc = XVar.Clone(this.getSingleRecordCommand());
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeQueryView"))))
			{
				dynamic prep = XVar.Array(), sql = null, where = null;
				prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
				where = XVar.Clone(prep["where"]);
				sql = XVar.Clone(prep["sql"]);
				this.eventsObject.BeforeQueryView((XVar)(sql), ref where, this);
				if(sql != prep["sql"])
				{
					this.dataSource.overrideSQL((XVar)(dc), (XVar)(sql));
				}
				else
				{
					if(where != prep["where"])
					{
						this.dataSource.overrideWhere((XVar)(dc), (XVar)(where));
					}
				}
			}
			fetchedArray = XVar.Clone(this.dataSource.getSingle((XVar)(dc)).fetchAssoc());
			this.data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(fetchedArray)));
			if(XVar.Pack(!(XVar)(this.checkKeysSet())))
			{
				this.keys = XVar.Clone(this.getKeysFromData((XVar)(this.data)));
				this.setKeysForJs();
			}
			if((XVar)(MVCFunctions.count(this.data))  && (XVar)(this.eventsObject.exists(new XVar("ProcessValuesView"))))
			{
				this.eventsObject.ProcessValuesView((XVar)(this.data), this);
			}
			return this.data;
		}
		protected virtual XVar checkKeysSet()
		{
			foreach (KeyValuePair<XVar, dynamic> kValue in this.keys.GetEnumerator())
			{
				if(XVar.Pack(MVCFunctions.strlen((XVar)(kValue.Value))))
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar readRecord()
		{
			if(XVar.Pack(this.getCurrentRecordInternal()))
			{
				return true;
			}
			if(this.mode == Constants.VIEW_SIMPLE)
			{
				MVCFunctions.HeaderRedirect((XVar)(this.pSet.getShortTableName()), new XVar("list"), new XVar("a=return"));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();
			return false;
		}
		public virtual XVar process()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessView"))))
			{
				this.eventsObject.BeforeProcessView(this);
			}
			if(XVar.Pack(!(XVar)(this.readRecord())))
			{
				return null;
			}
			this.prepareMaps();
			if(this.mode == Constants.VIEW_SIMPLE)
			{
				this.preparePdfControls();
			}
			this.doCommonAssignments();
			this.prepareBreadcrumbs();
			this.prepareCollapseButton();
			this.prepareMockControls();
			this.prepareButtons();
			this.prepareSteps();
			this.prepareFields();
			this.prepareJsSettings();
			this.prepareDetailsTables();
			this.addButtonHandlers();
			this.addCommonJs();
			this.fillSetCntrlMaps();
			this.displayViewPage();

			return null;
		}
		public override XVar addCommonJs()
		{
			base.addCommonJs();

			return null;
		}
		protected virtual XVar doCommonAssignments()
		{
			dynamic data = XVar.Array();
			if(XVar.Equals(XVar.Pack(this.mode), XVar.Pack(Constants.VIEW_SIMPLE)))
			{
				this.headerCommonAssign();
			}
			else
			{
				this.xt.assign(new XVar("menu_chiddenattr"), new XVar("data-hidden"));
			}
			this.setLangParams();
			data = XVar.Clone(this.getCurrentRecordInternal());
			foreach (KeyValuePair<XVar, dynamic> k in this.pSet.getTableKeys().GetEnumerator())
			{
				dynamic viewFormat = null;
				viewFormat = XVar.Clone(this.pSet.getViewFormat((XVar)(k.Value)));
				if((XVar)((XVar)((XVar)((XVar)((XVar)((XVar)(viewFormat == Constants.FORMAT_HTML)  || (XVar)(viewFormat == Constants.FORMAT_FILE_IMAGE))  || (XVar)(viewFormat == Constants.FORMAT_FILE))  || (XVar)(viewFormat == Constants.FORMAT_HYPERLINK))  || (XVar)(viewFormat == Constants.FORMAT_HYPERLINK))  || (XVar)(viewFormat == Constants.FORMAT_EMAILHYPERLINK))  || (XVar)(viewFormat == Constants.FORMAT_CHECKBOX))
				{
					this.xt.assign((XVar)(MVCFunctions.Concat("show_key", k.Key + 1)), (XVar)(MVCFunctions.runner_htmlspecialchars((XVar)(data[k.Value]))));
				}
				else
				{
					this.xt.assign((XVar)(MVCFunctions.Concat("show_key", k.Key + 1)), (XVar)(this.showDBValue((XVar)(k.Value), (XVar)(data))));
				}
			}
			this.assignViewFieldsBlocksAndLabels();
			if(this.mode == Constants.VIEW_SIMPLE)
			{
				this.assignBody();
				this.xt.assign(new XVar("flybody"), new XVar(true));
			}
			this.displayMasterTableInfo();
			this.xt.assign(new XVar("editlink"), (XVar)(MVCFunctions.Concat(this.getEditLink((XVar)(data)), "&", this.getStateUrlParams())));
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.xt.assign(new XVar("pdfFonts"), (XVar)(MVCFunctions.runner_json_encode((XVar)(CommonFunctions.getPdfFonts()))));
			}

			return null;
		}
		protected virtual XVar displayViewPage()
		{
			dynamic templatefile = null;
			templatefile = XVar.Clone(this.templatefile);
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowView"))))
			{
				this.eventsObject.BeforeShowView((XVar)(this.xt), ref templatefile, (XVar)(this.getCurrentRecordInternal()), this);
			}
			if(this.mode == Constants.VIEW_SIMPLE)
			{
				this.display((XVar)(templatefile));
				return null;
			}
			if(this.mode == Constants.VIEW_PDFJSON)
			{
				this.preparePDFBackground();
				this.xt.displayJSON((XVar)(this.templatefile));
				return null;
			}
			this.xt.assign(new XVar("footer"), new XVar(false));
			this.xt.assign(new XVar("header"), new XVar(false));
			this.xt.assign(new XVar("body"), (XVar)(this.body));
			this.displayAJAX((XVar)(templatefile), (XVar)(this.flyId + 1));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected virtual XVar prepareDetailsTables()
		{
			dynamic d = null, dpParams = XVar.Array(), dpTablesParams = XVar.Array();
			if(XVar.Pack(!(XVar)(this.showDetailsPreview())))
			{
				return null;
			}
			dpParams = XVar.Clone(this.getDetailsParams((XVar)(this.id)));
			if(XVar.Pack(!(XVar)(dpParams["ids"])))
			{
				return null;
			}
			this.xt.assign(new XVar("detail_tables"), new XVar(true));
			if(this.mode == Constants.VIEW_DASHBOARD)
			{
				dpTablesParams = XVar.Clone(XVar.Array());
			}
			d = new XVar(0);
			for(;d < MVCFunctions.count(dpParams["ids"]); d++)
			{
				if(this.mode != Constants.VIEW_DASHBOARD)
				{
					this.setDetailPreview((XVar)(dpParams["type"][d]), (XVar)(dpParams["strTableNames"][d]), (XVar)(dpParams["ids"][d]), (XVar)(this.getCurrentRecordInternal()));
				}
				else
				{
					this.xt.assign((XVar)(MVCFunctions.Concat("details_", dpParams["shorTNames"][d])), new XVar(true));
					dpTablesParams.InitAndSetArrayItem(new XVar("tName", dpParams["strTableNames"][d], "id", dpParams["ids"][d], "pType", dpParams["type"][d]), null);
					this.xt.assign((XVar)(MVCFunctions.Concat("displayDetailTable_", dpParams["shorTNames"][d])), (XVar)(MVCFunctions.Concat("<div id='dp_", MVCFunctions.GoodFieldName((XVar)(this.tName)), "_", this.pageType, "_", dpParams["ids"][d], "'></div>")));
				}
			}
			if(this.mode == Constants.VIEW_DASHBOARD)
			{
				this.controlsMap.InitAndSetArrayItem(dpTablesParams, "dpTablesParams");
			}

			return null;
		}
		protected virtual XVar prepareFields()
		{
			dynamic data = XVar.Array(), keyParams = XVar.Array(), keylink = null, viewFields = XVar.Array();
			viewFields = XVar.Clone(this.pSet.getViewFields());
			data = XVar.Clone(this.getCurrentRecordInternal());
			RunnerContext.pushRecordContext((XVar)(data), this);
			foreach (KeyValuePair<XVar, dynamic> kf in this.pSet.getTableKeys().GetEnumerator())
			{
				keyParams.InitAndSetArrayItem(MVCFunctions.Concat("&key", kf.Key + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(data[kf.Value]))))), null);
			}
			keylink = XVar.Clone(MVCFunctions.implode(new XVar(""), (XVar)(keyParams)));
			foreach (KeyValuePair<XVar, dynamic> f in viewFields.GetEnumerator())
			{
				dynamic gname = null, parameters = XVar.Array(), value = null, varName = null;
				gname = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(f.Value)));
				if(XVar.Pack(!(XVar)(this.pdfJsonMode())))
				{
					value = XVar.Clone(MVCFunctions.Concat("<span id=\"view", this.id, "_", gname, "\" >", this.showDBValue((XVar)(f.Value), (XVar)(data), (XVar)(keylink)), "</span>"));
					varName = XVar.Clone(MVCFunctions.Concat(gname, "_value"));
				}
				else
				{
					value = XVar.Clone(this.showDBValue((XVar)(f.Value), (XVar)(data), (XVar)(keylink)));
					varName = XVar.Clone(MVCFunctions.Concat(gname, "_pdfvalue"));
				}
				parameters = XVar.Clone(new XVar("value", value));
				if(this.pSet.getViewFormat((XVar)(f.Value)) == Constants.FORMAT_CHECKBOX)
				{
					parameters.InitAndSetArrayItem(this.xt, "xt");
					parameters.InitAndSetArrayItem(MVCFunctions.Concat(gname, "_forward_control"), "clearVar");
				}
				this.xt.assign_function((XVar)(varName), new XVar("xt_buildviewcontrol"), (XVar)(parameters));
				if(this.pSet.getViewFormat((XVar)(f.Value)) == Constants.FORMAT_CHECKBOX)
				{
					parameters = XVar.Clone(XVar.Array());
					parameters.InitAndSetArrayItem(this.xt, "xt");
					parameters.InitAndSetArrayItem(varName, "clearVar");
					parameters.InitAndSetArrayItem(value, "value");
					this.xt.assign_function((XVar)(MVCFunctions.Concat(gname, "_forward_control")), new XVar("xt_forwardViewControl"), (XVar)(parameters));
				}
				this.xt.assign((XVar)(MVCFunctions.Concat(gname, "_fieldblock")), new XVar(true));
				if((XVar)(this.pSet.hideEmptyViewFields())  && (XVar)(data[f.Value] == ""))
				{
					this.hideField((XVar)(f.Value));
					this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_fieldblock")), new XVar(false));
				}
			}
			RunnerContext.pop();

			return null;
		}
		protected virtual XVar prepareMockControls()
		{
			dynamic controlFields = XVar.Array();
			controlFields = XVar.Clone(this.pSet.getViewFields());
			foreach (KeyValuePair<XVar, dynamic> fName in controlFields.GetEnumerator())
			{
				dynamic control = XVar.Array();
				control = XVar.Clone(XVar.Array());
				control.InitAndSetArrayItem(this.id, "id");
				control.InitAndSetArrayItem(0, "ctrlInd");
				control.InitAndSetArrayItem(fName.Value, "fieldName");
				control.InitAndSetArrayItem("view", "mode");
				this.controlsMap.InitAndSetArrayItem(control, "controls", null);
			}

			return null;
		}
		protected virtual XVar prepareMaps()
		{
			dynamic fieldsArr = XVar.Array(), viewFields = XVar.Array();
			fieldsArr = XVar.Clone(XVar.Array());
			viewFields = XVar.Clone(this.pSet.getViewFields());
			foreach (KeyValuePair<XVar, dynamic> f in viewFields.GetEnumerator())
			{
				fieldsArr.InitAndSetArrayItem(new XVar("fName", f.Value, "viewFormat", this.pSet.getViewFormat((XVar)(f.Value))), null);
			}
			this.setGoogleMapsParams((XVar)(fieldsArr));
			if(XVar.Pack(this.googleMapCfg["isUseGoogleMap"]))
			{
				this.initGmaps();
			}

			return null;
		}
		protected virtual XVar prepareNextPrevButtons()
		{
			dynamic nextPrev = XVar.Array(), prev = null, var_next = null;
			if((XVar)(!(XVar)(this.pSet.useMoveNext()))  || (XVar)(this.mode == Constants.VIEW_PDFJSON))
			{
				this.hideItemType(new XVar("prev"));
				this.hideItemType(new XVar("next"));
				return null;
			}
			var_next = XVar.Clone(XVar.Array());
			prev = XVar.Clone(XVar.Array());
			nextPrev = XVar.Clone(this.getNextPrevRecordKeys((XVar)(this.getCurrentRecordInternal())));
			this.assignPrevNextButtons((XVar)(!(XVar)(!(XVar)(nextPrev["next"]))), (XVar)(!(XVar)(!(XVar)(nextPrev["prev"]))), (XVar)((XVar)(this.mode == Constants.VIEW_DASHBOARD)  && (XVar)((XVar)(this.hasTableDashGridElement())  || (XVar)(this.hasDashMapElement()))));
			this.nextData = XVar.Clone(nextPrev["next"]);
			this.prevData = XVar.Clone(nextPrev["prev"]);

			return null;
		}
		protected virtual XVar prepareButtons()
		{
			dynamic editable = null;
			this.prepareNextPrevButtons();
			if(this.mode == Constants.VIEW_DASHBOARD)
			{
				this.xt.assign(new XVar("groupbutton_class"), new XVar("rnr-invisible-button"));
				return null;
			}
			if(this.mode == Constants.VIEW_SIMPLE)
			{
				if(XVar.Pack(this.pSet.hasListPage()))
				{
					this.xt.assign(new XVar("back_button"), new XVar(true));
					this.xt.assign(new XVar("backbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"backButton", this.id, "\"")));
					this.xt.assign(new XVar("mbackbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"extraBackButton", this.id, "\"")));
				}
				else
				{
					if(XVar.Pack(this.isShowMenu()))
					{
						this.xt.assign(new XVar("back_button"), new XVar(true));
						this.xt.assign(new XVar("backbutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"backToMenuButton", this.id, "\"")));
					}
				}
			}
			if(this.mode == Constants.VIEW_POPUP)
			{
				this.xt.assign(new XVar("close_button"), new XVar(true));
				this.xt.assign(new XVar("closebutton_attrs"), (XVar)(MVCFunctions.Concat("id=\"closeButton", this.id, "\"")));
			}
			editable = new XVar(false);
			if(XVar.Pack(this.editAvailable()))
			{
				dynamic data = null;
				data = XVar.Clone(this.getCurrentRecordInternal());
				if(XVar.Pack(this.recordEditable((XVar)(data))))
				{
					this.xt.assign(new XVar("edit_page_button"), new XVar(true));
					this.xt.assign(new XVar("edit_page_button_attrs"), (XVar)(MVCFunctions.Concat("id=\"editPageButton", this.id, "\"")));
				}
			}
			this.xt.assign(new XVar("view_menu_button"), (XVar)(editable));

			return null;
		}
		public static XVar readViewModeFromRequest()
		{
			if(MVCFunctions.postvalue(new XVar("mode")) == "dashrecord")
			{
				return Constants.VIEW_DASHBOARD;
			}
			else
			{
				if(XVar.Pack(MVCFunctions.postvalue(new XVar("onFly"))))
				{
					return Constants.VIEW_POPUP;
				}
				else
				{
					if(XVar.Pack(MVCFunctions.postvalue(new XVar("pdfjson"))))
					{
						return Constants.VIEW_PDFJSON;
					}
				}
			}
			return Constants.VIEW_SIMPLE;
		}
		public override XVar editAvailable()
		{
			if(XVar.Pack(this.dashElementData))
			{
				dynamic dashType = null;
				dashType = XVar.Clone(this.dashElementData["type"]);
				return (XVar)(base.editAvailable())  && (XVar)((XVar)((XVar)(dashType == Constants.DASHBOARD_DETAILS)  && (XVar)(this.dashElementData["details"][this.tName]["edit"]))  || (XVar)((XVar)(dashType == Constants.DASHBOARD_LIST)  && (XVar)(this.dashElementData["popupEdit"])));
			}
			return base.editAvailable();
		}
		public virtual XVar assignViewFieldsBlocksAndLabels()
		{
			dynamic viewFields = XVar.Array();
			viewFields = XVar.Clone(this.pSet.getViewFields());
			foreach (KeyValuePair<XVar, dynamic> fName in viewFields.GetEnumerator())
			{
				dynamic gfName = null;
				gfName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(fName.Value)));
				this.xt.assign((XVar)(MVCFunctions.Concat(gfName, "_fieldblock")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat(gfName, "_label")), new XVar(true));
			}

			return null;
		}
		protected override XVar checkShowBreadcrumbs()
		{
			return this.mode == Constants.VIEW_SIMPLE;
		}
		public override XVar pdfJsonMode()
		{
			return this.mode == Constants.VIEW_PDFJSON;
		}
		public virtual XVar getSingleRecordCommand()
		{
			dynamic dc = null;
			if(XVar.Pack(this.checkKeysSet()))
			{
				dc = XVar.Clone(new DsCommand());
				dc.filter = XVar.Clone(this.getSecurityCondition());
				dc.keys = XVar.Clone(this.keys);
			}
			else
			{
				return this.getSubsetDataCommand();
			}
			return dc;
		}
		public override XVar getSecurityCondition()
		{
			return Security.SelectCondition(new XVar("S"), (XVar)(this.pSet));
		}
		public override XVar inDashboardMode()
		{
			return this.mode == Constants.VIEW_DASHBOARD;
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
			settings.InitAndSetArrayItem(this.jsKeys, "keys");
			settings.InitAndSetArrayItem(this.pSet.getTableKeys(), "keyFields");
			settings.InitAndSetArrayItem(this.prevData, "prevKeys");
			settings.InitAndSetArrayItem(this.nextData, "nextKeys");
			return settings;
		}
	}
}
