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
	public partial class PrintPage_Details : PrintPage
	{
		public dynamic multipleDetails = XVar.Pack(false);
		protected static bool skipPrintPage_DetailsCtor = false;
		public PrintPage_Details(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipPrintPage_DetailsCtor)
			{
				skipPrintPage_DetailsCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

		}
		public override XVar process()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessPrint"))))
			{
				this.eventsObject.BeforeProcessPrint(this);
			}
			this.commonAssign();
			this.setMapParams();
			this.splitByRecords = new XVar(0);
			this.allPagesMode = new XVar(true);
			this.queryCommand = XVar.Clone(this.getSubsetDataCommand());
			this.callBeforeQueryEvent((XVar)(this.queryCommand));
			this.recordset = XVar.Clone(this.dataSource.getList((XVar)(this.queryCommand)));
			if(XVar.Pack(!(XVar)(this.recordset)))
			{
				MVCFunctions.showError((XVar)(this.dataSource.lastError()));
			}
			this.doFirstPageAssignments();
			this.fillGridPage();
			this.fillRenderedData((XVar)(this.pageBody["grid_row"]["data"]));
			this.assignTotals();
			this.hideEmptyFields();
			this.addCommonJs();
			this.doCommonAssignments();
			this.addCustomCss();
			this.displayPrintPage();

			return null;
		}
		public override XVar displayPrintPage()
		{
			if(XVar.Pack(!(XVar)(this.fetchedRecordCount)))
			{
				return null;
			}
			this.xt.bulk_assign((XVar)(this.pageBody));
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.xt.assign(new XVar("body"), new XVar(true));
				this.xt.assign(new XVar("embedded_grid"), new XVar(true));
				this.xt.assign(new XVar("embedded_page_title"), new XVar(true));
				this.xt.load_templateJSON((XVar)(this.templatefile));
				MVCFunctions.Echo(this.xt.fetch_loadedJSON(new XVar("body")));
				return null;
			}
			this.xt.assign(new XVar("grid_block"), new XVar(true));
			this.xt.assign(new XVar("printheader"), (XVar)(this.multipleDetails));
			this.xt.load_template((XVar)(this.templatefile));
			MVCFunctions.Echo(MVCFunctions.Concat("<div class=\"panel panel-info details-grid\">\r\n\t\t\t<div class=\"panel-heading\">\r\n\t\t\t\t<h4 class=\"panel-title\">", this.getPageTitle((XVar)(this.pageName), (XVar)(this.tName)), "</h4>\r\n\t\t\t</div>\r\n\t\t\t<div class=\"panel-body\">"));
			MVCFunctions.Echo(this.fetchForms((XVar)(new XVar(0, "grid"))));
			MVCFunctions.Echo("</div>\r\n\t\t</div>");

			return null;
		}
		public override XVar getMasterTableSQLClause()
		{
			dynamic dKeys = XVar.Array(), where = null;
			where = new XVar("");
			dKeys = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
			if(XVar.Pack(!(XVar)(dKeys["detailsKeys"])))
			{
				return "1=0";
			}
			foreach (KeyValuePair<XVar, dynamic> key in dKeys["detailsKeys"].GetEnumerator())
			{
				dynamic mValue = null;
				if(key.Key != XVar.Pack(0))
				{
					where = MVCFunctions.Concat(where, " and ");
				}
				if((XVar)(this.cipherer)  && (XVar)(this.cipherer.isEncryptionByPHPEnabled()))
				{
					mValue = XVar.Clone(this.cipherer.MakeDBValue((XVar)(key.Value), (XVar)(this.masterKeysReq[key.Key + 1])));
				}
				else
				{
					mValue = XVar.Clone(CommonFunctions.make_db_value((XVar)(key.Value), (XVar)(this.masterKeysReq[key.Key + 1]), new XVar(""), new XVar(""), (XVar)(this.tName)));
				}
				if(MVCFunctions.strlen((XVar)(mValue)) != 0)
				{
					where = MVCFunctions.Concat(where, this.getFieldSQLDecrypt((XVar)(key.Value)), "=", mValue);
				}
				else
				{
					where = MVCFunctions.Concat(where, "1=0");
				}
			}
			return where;
		}
		protected override XVar EOF()
		{
			dynamic recordLimit = null;
			recordLimit = XVar.Clone(this.pSet.getRecordsLimit());
			if((XVar)(recordLimit)  && (XVar)(recordLimit <= this.fetchedRecordCount))
			{
				return true;
			}
			this.readNextRecordInternal();
			if(XVar.Pack(this._eof))
			{
				return true;
			}
			return false;
		}
		protected virtual XVar prepareColumnOrderSettings()
		{

			return null;
		}
	}
}
