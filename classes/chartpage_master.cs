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
	public partial class ChartPage_Master : ChartPage
	{
		protected static bool skipChartPage_MasterCtor = false;
		public ChartPage_Master(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipChartPage_MasterCtor)
			{
				skipChartPage_MasterCtor = false;
				return;
			}
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
			this.xt.assign(new XVar("chart_block"), new XVar(true));
			this.assignChartElement();
			this.xt.assign(new XVar("pagetitlelabel"), (XVar)(this.getPageTitle((XVar)(this.pageType), (XVar)(this.tName), (XVar)(this.masterRecordData))));
			tKeys = XVar.Clone(this.pSet.getTableKeys());
			keylink = new XVar("");
			i = new XVar(0);
			for(;i < MVCFunctions.count(tKeys); i++)
			{
				keylink = MVCFunctions.Concat(keylink, "&key", i + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(this.masterRecordData[tKeys[i]])))));
			}
			fields = XVar.Clone(this.pSet.getMasterListFields());
			fields = XVar.Clone(MVCFunctions.array_merge((XVar)(fields), (XVar)(tKeys)));
			foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
			{
				dynamic fieldClassStr = null;
				fieldClassStr = XVar.Clone(this.fieldClass((XVar)(f.Value)));
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_mastervalue")), (XVar)(MVCFunctions.Concat("<span class='", fieldClassStr, "'>", this.showDBValue((XVar)(f.Value), (XVar)(this.masterRecordData), (XVar)(keylink)), "</span>")));
				this.xt.assign((XVar)(MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(f.Value)), "_class")), (XVar)(fieldClassStr));
			}
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
			this.xt.load_template((XVar)(this.templatefile));
			this.xt.assign(new XVar("masterlist_title"), new XVar(false));
			this.xt.display_loaded();

			return null;
		}
	}
}
