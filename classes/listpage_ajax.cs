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
	public partial class ListPage_Ajax : ListPage_Simple
	{
		protected static bool skipListPage_AjaxCtor = false;
		private bool skipListPage_SimpleCtorSurrogate = new Func<bool>(() => skipListPage_SimpleCtor = true).Invoke();
		public ListPage_Ajax(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipListPage_AjaxCtor)
			{
				skipListPage_AjaxCtor = false;
				return;
			}
		}
		public override XVar commonAssign()
		{
			base.commonAssign();
			this.xt.assign(new XVar("filterPanelStateClass"), new XVar("filter-ajaxReloaded"));

			return null;
		}
		public override XVar addCommonHtml()
		{
			return true;
		}
		public override XVar addCommonJs()
		{
			this.addJsForGrid();

			return null;
		}
		public override XVar showPage()
		{
			dynamic returnJSON = XVar.Array();
			this.addControlsJSAndCSS();
			this.fillSetCntrlMaps();
			this.BeforeShowList();
			this.xt.load_template((XVar)(this.templatefile));
			returnJSON = XVar.Clone(XVar.Array());
			returnJSON.InitAndSetArrayItem(true, "success");
			returnJSON.InitAndSetArrayItem(this.flyId, "idStartFrom");
			returnJSON.InitAndSetArrayItem(GlobalVars.pagesData, "pagesData");
			returnJSON.InitAndSetArrayItem(this.controlsHTMLMap, "controlsMap");
			returnJSON.InitAndSetArrayItem(this.viewControlsMap, "viewControlsMap");
			returnJSON.InitAndSetArrayItem(this.jsSettings, "settings");
			returnJSON.InitAndSetArrayItem(MVCFunctions.Concat(this.row_css_rules, this.cell_css_rules, "\n", this.mobile_css_rules), "cellStyles");
			returnJSON.InitAndSetArrayItem(this.numRowsFromSQL, "numberOfRecs");
			returnJSON.InitAndSetArrayItem(this.pageSize, "recPerPage");
			if(this.deleteMessage != "")
			{
				returnJSON.InitAndSetArrayItem(true, "usermessage");
			}
			returnJSON.InitAndSetArrayItem(this.xt.fetch_loaded(new XVar("body")), "html");
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();
			return null;
		}
		public override XVar rulePRG()
		{

			return null;
		}
	}
}
