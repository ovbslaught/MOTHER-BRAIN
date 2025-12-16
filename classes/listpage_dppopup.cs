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
	public partial class ListPage_DPPopup : ListPage_DPInline
	{
		protected static bool skipListPage_DPPopupCtor = false;
		public ListPage_DPPopup(dynamic var_params) // proxy constructor
			:base((XVar)var_params) {}

		public override XVar showPage()
		{
			dynamic detFoundMessage = null, returnJSON = XVar.Array();
			returnJSON = XVar.Clone(XVar.Array());
			this.xt.assign(new XVar("view_column"), new XVar(false));
			this.xt.assign(new XVar("edit_column"), new XVar(false));
			this.xt.assign(new XVar("inlineedit_column"), new XVar(false));
			this.xt.assign(new XVar("inlinesave_column"), new XVar(false));
			this.xt.assign(new XVar("inlinecancel_column"), new XVar(false));
			this.xt.assign(new XVar("copy_column"), new XVar(false));
			this.xt.assign(new XVar("checkbox_column"), new XVar(false));
			this.xt.assign(new XVar("dtable_column"), new XVar(false));
			this.xt.prepare_template((XVar)(this.templatefile));
			returnJSON.InitAndSetArrayItem(true, "success");
			returnJSON.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("counter")), "counter");
			detFoundMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("DETAILS_FOUND")));
			returnJSON.InitAndSetArrayItem(MVCFunctions.Concat("<span>", detFoundMessage, ": <strong>", this.numRowsFromSQL, "</strong></span>", this.xt.fetch_loaded(new XVar("grid_block"))), "body");
			MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		protected override XVar assignSessionPrefix()
		{
			this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.tName, "_preview"));

			return null;
		}
		public override XVar assignColumnHeaders()
		{
			foreach (KeyValuePair<XVar, dynamic> f in this.listFields.GetEnumerator())
			{
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value["fName"])), "_fieldheader")), new XVar(true));
			}

			return null;
		}
	}
}
