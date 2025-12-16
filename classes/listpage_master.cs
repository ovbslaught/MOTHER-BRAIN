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
	public partial class ListPage_Master : ListPage
	{
		public dynamic pdfJson = XVar.Pack(false);
		protected static bool skipListPage_MasterCtor = false;
		public ListPage_Master(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipListPage_MasterCtor)
			{
				skipListPage_MasterCtor = false;
				return;
			}
			this.masterPageType = XVar.Clone(var_params["masterPageType"]);
		}
		public override XVar commonAssign()
		{
			base.commonAssign();

			return null;
		}
		protected override XVar createOrderByObject()
		{
			this.orderClause = XVar.Clone(OrderClause.createFromPage(this, new XVar(false)));

			return null;
		}
		public virtual XVar getMasterHeading()
		{
			this.xt.assign(new XVar("masterlist_title"), new XVar(true));
			return this.xt.fetch_loaded(new XVar("masterlist_title"));
		}
		public virtual XVar preparePage()
		{
			dynamic fields = XVar.Array(), i = null, keylink = null, tKeys = XVar.Array();
			if((XVar)(!(XVar)(this.masterRecordData))  || (XVar)(!(XVar)(this.masterRecordData)))
			{
				return null;
			}
			this.xt.assign(new XVar("pagetitlelabel"), (XVar)(this.getPageTitle((XVar)(this.pageType), (XVar)(this.tName), (XVar)(this.masterRecordData))));
			tKeys = XVar.Clone(this.pSet.getTableKeys());
			keylink = new XVar("");
			i = new XVar(0);
			for(;i < MVCFunctions.count(tKeys); i++)
			{
				keylink = MVCFunctions.Concat(keylink, "&key", i + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(this.masterRecordData[tKeys[i]])))));
			}
			fields = XVar.Clone(this.pSet.getPageFields());
			fields = XVar.Clone(MVCFunctions.array_merge((XVar)(fields), (XVar)(tKeys)));
			foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
			{
				dynamic fieldClassStr = null, value = null;
				fieldClassStr = XVar.Clone(this.fieldClass((XVar)(f.Value)));
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_fieldheader")), new XVar(true));
				value = XVar.Clone(this.showDBValue((XVar)(f.Value), (XVar)(this.masterRecordData), (XVar)(keylink)));
				if(XVar.Pack(this.pdfJsonMode()))
				{
					this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_pdfvalue")), (XVar)(value));
				}
				else
				{
					dynamic wrappedValue = null;
					wrappedValue = XVar.Clone(MVCFunctions.Concat("<span class='", fieldClassStr, "'>", value, "</span>"));
					this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_mastervalue")), (XVar)(wrappedValue));
					this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_value")), (XVar)(wrappedValue));
				}
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_class")), (XVar)(fieldClassStr));
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_fieldcolumn")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_fieldheadercolumn")), new XVar(true));
			}
			this.xt.assign(new XVar("grid_header"), new XVar(true));
			this.xt.assign(new XVar("grid_row"), new XVar(true));
			this.xt.assign(new XVar("grid_record"), new XVar(true));
			this.xt.assign(new XVar("grid_vrecord"), new XVar(true));
			if(XVar.Pack(this.pageLayout))
			{
				this.xt.assign(new XVar("pageattrs"), (XVar)(MVCFunctions.Concat("class=\"", this.pageLayout.style, " page-", this.pageLayout.name, "\"")));
			}

			return null;
		}
		public virtual XVar showMaster(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			if((XVar)(!(XVar)(this.masterRecordData))  || (XVar)(!(XVar)(this.masterRecordData)))
			{
				return null;
			}
			this.xt.assign(new XVar("masterlist_title"), new XVar(false));
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.xt.assign(new XVar("body"), new XVar(true));
				this.xt.assign(new XVar("embedded_grid"), new XVar(true));
				this.xt.assign(new XVar("embedded_grid_caption"), new XVar(true));
				this.xt.load_templateJSON((XVar)(this.templatefile));
				MVCFunctions.Echo(this.xt.fetch_loadedJSON(new XVar("body")));
			}
			else
			{
				this.xt.load_template((XVar)(this.templatefile));
				this.xt.display_loaded();
			}

			return null;
		}
		public override XVar pdfJsonMode()
		{
			return this.pdfJson;
		}
	}
}
