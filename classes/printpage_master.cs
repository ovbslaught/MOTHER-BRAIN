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
	public partial class PrintPage_Master : PrintPage
	{
		public dynamic pdfJson = XVar.Pack(false);
		protected static bool skipPrintPage_MasterCtor = false;
		public PrintPage_Master(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipPrintPage_MasterCtor)
			{
				skipPrintPage_MasterCtor = false;
				return;
			}
			this.pageType = new XVar("masterprint");
			this.masterPageType = XVar.Clone(var_params["masterPageType"]);
			if(this.masterPageType == "report")
			{
				this.pageType = new XVar("masterrprint");
			}
		}
		public override XVar commonAssign()
		{
			base.commonAssign();

			return null;
		}
		public virtual XVar getMasterHeading()
		{
			this.xt.assign(new XVar("masterlist_title"), new XVar(true));
			return this.xt.fetch_loaded(new XVar("masterlist_title"));
		}
		public virtual XVar preparePage()
		{
			dynamic fields = XVar.Array(), i = null, keylink = null, pageTypeTitle = null, tKeys = XVar.Array();
			if((XVar)(!(XVar)(this.masterRecordData))  || (XVar)(!(XVar)(this.masterRecordData)))
			{
				return null;
			}
			pageTypeTitle = XVar.Clone(this.pageType);
			if(this.masterPageType == "report")
			{
				pageTypeTitle = new XVar("masterprint");
			}
			this.xt.assign(new XVar("pagetitlelabel"), (XVar)(this.getPageTitle((XVar)(pageTypeTitle), (XVar)(this.tName), (XVar)(this.masterRecordData))));
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
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_fieldheader")), new XVar(true));
				fieldClassStr = XVar.Clone(this.fieldClass((XVar)(f.Value)));
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
			if(XVar.Pack(!(XVar)(this.pdfJsonMode())))
			{
				this.xt.load_template((XVar)(this.templatefile));
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
