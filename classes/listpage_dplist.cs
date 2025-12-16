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
	public partial class ListPage_DPList : ListPage_DPInline
	{
		protected static bool skipListPage_DPListCtor = false;
		public ListPage_DPList(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipListPage_DPListCtor)
			{
				skipListPage_DPListCtor = false;
				return;
			}
			this.formBricks.InitAndSetArrayItem(new XVar(0, "pagination_block"), "footer");
		}
		public override XVar showPage()
		{
			this.BeforeShowList();
			this.xt.prepare_template((XVar)(this.templatefile));
			this.showPageAjax();

			return null;
		}
		public virtual XVar showPageAjax()
		{
			dynamic returnJSON = XVar.Array();
			returnJSON = XVar.Clone(XVar.Array());
			if((XVar)((XVar)((XVar)((XVar)(!(XVar)(this.numRowsFromSQL))  && (XVar)(!(XVar)(this.addAvailable())))  && (XVar)(!(XVar)(this.inlineAddAvailable())))  && (XVar)(!(XVar)(this.recordsDeleted)))  && (XVar)(this.getGridTabsCount() == 0))
			{
				returnJSON.InitAndSetArrayItem(false, "success");
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				return null;
			}
			this.addControlsJSAndCSS();
			this.fillSetCntrlMaps();
			returnJSON.InitAndSetArrayItem(GlobalVars.pagesData, "pagesData");
			returnJSON.InitAndSetArrayItem(this.controlsHTMLMap, "controlsMap");
			returnJSON.InitAndSetArrayItem(this.viewControlsHTMLMap, "viewControlsMap");
			returnJSON.InitAndSetArrayItem(this.jsSettings, "settings");
			this.xt.assign(new XVar("header"), new XVar(false));
			this.xt.assign(new XVar("footer"), new XVar(false));
			returnJSON.InitAndSetArrayItem(this.xt.fetch_loaded(new XVar("above-grid_block")), "headerCont");
			returnJSON.InitAndSetArrayItem(this.xt.fetch_loaded(new XVar("below-grid_block")), "footerCont");
			this.xt.prepareContainers();
			returnJSON.InitAndSetArrayItem(this.xt.fetch_loaded(new XVar("grid_block")), "html");
			if(this.listGridLayout == Constants.gltVERTICAL)
			{
				returnJSON.InitAndSetArrayItem(MVCFunctions.Concat(this.xt.fetch_loaded(new XVar("message_block")), returnJSON["html"]), "html");
			}
			returnJSON.InitAndSetArrayItem(this.flyId, "idStartFrom");
			returnJSON.InitAndSetArrayItem(true, "success");
			returnJSON.InitAndSetArrayItem(this.grabAllJsFiles(), "additionalJS");
			returnJSON.InitAndSetArrayItem(this.grabAllCSSFiles(), "CSSFiles");
			if(this.deleteMessage != "")
			{
				returnJSON.InitAndSetArrayItem(true, "delMess");
			}
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));

			return null;
		}
		protected override XVar getBSButtonsClass()
		{
			return "btn btn-sm";
		}
		public override XVar showNoRecordsMessage()
		{

			return null;
		}
	}
}
