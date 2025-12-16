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
	public partial class ReportPrintPage : ReportPage
	{
		public dynamic pageWidth = XVar.Pack(Constants.PDF_PAGE_WIDTH);
		public dynamic pageHeight = XVar.Pack(Constants.PDF_PAGE_HEIGHT);
		public dynamic splitAtServer = XVar.Pack(false);
		public dynamic splitByGroups = XVar.Pack(0);
		public dynamic pages = XVar.Array();
		public dynamic arrPages = XVar.Array();
		public dynamic pdfFitToPage = XVar.Pack(1);
		public dynamic landscape = XVar.Pack(0);
		public dynamic isDetail = XVar.Pack(false);
		public dynamic isReportEmpty = XVar.Pack(false);
		public dynamic multipleDetails = XVar.Pack(false);
		protected static bool skipReportPrintPageCtor = false;
		public ReportPrintPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipReportPrintPageCtor)
			{
				skipReportPrintPageCtor = false;
				return;
			}
			GlobalVars.init_runnerpage();
			if((XVar)(this.format == "excel")  || (XVar)(this.format == "word"))
			{
				this.splitAtServer = new XVar(false);
				this.splitByGroups = new XVar(0);
			}
			else
			{
				if(XVar.Pack(!(XVar)(this.pdfJsonMode())))
				{
					this.splitAtServer = new XVar(true);
					if(XVar.Pack(!(XVar)(this.splitByGroups)))
					{
						this.splitByGroups = XVar.Clone(this.pSet.getReportPrintGroupsPerPage());
					}
					this.pageData.InitAndSetArrayItem(this.splitByGroups, "printRecords");
				}
				else
				{
					if(XVar.Pack(this.pdfJsonMode()))
					{
						this.splitAtServer = new XVar(true);
						if(XVar.Pack(!(XVar)(this.splitByGroups)))
						{
							this.splitByGroups = new XVar(100000);
						}
					}
				}
			}
			if(XVar.Pack(this.isDetail))
			{
				this.splitAtServer = new XVar(false);
				this.splitByGroups = new XVar(0);
			}
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessReportPrint"))))
			{
				this.eventsObject.BeforeProcessReportPrint(this);
			}
			if(XVar.Pack(this.crossTable))
			{
				this.pageData.InitAndSetArrayItem(this.getCurrentCrossParams(), "crossParams");
			}
		}
		public override XVar getExtraReportParams()
		{
			dynamic extraParams = XVar.Array();
			extraParams = XVar.Clone(base.getExtraReportParams());
			if(this.format == "excel")
			{
				extraParams.InitAndSetArrayItem("excel", "forExport");
			}
			else
			{
				if(this.format == "word")
				{
					extraParams.InitAndSetArrayItem("word", "forExport");
				}
				else
				{
					extraParams.InitAndSetArrayItem(false, "forExport");
				}
			}
			if(XVar.Pack(!(XVar)(this.crossTable)))
			{
				extraParams.InitAndSetArrayItem(Constants.MODE_PRINT, "mode");
			}
			return extraParams;
		}
		public override XVar process()
		{
			dynamic extraParams = XVar.Array(), forExport = null;
			this.displayMasterTableInfo();
			forExport = new XVar(false);
			if(this.format == "excel")
			{
				forExport = new XVar("excel");
				MVCFunctions.Header("Content-Type", "application/vnd.ms-excel");
				MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Disposition: attachment;Filename=", this.shortTableName, ".xls")));
				MVCFunctions.Echo("<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns=\"http://www.w3.org/TR/REC-html40\">");
				MVCFunctions.Echo(MVCFunctions.Concat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=", GlobalVars.cCharset, "\">"));
			}
			else
			{
				if(this.format == "word")
				{
					forExport = new XVar("word");
					MVCFunctions.Header("Content-Type", "application/vnd.ms-word");
					MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Disposition: attachment;Filename=", this.shortTableName, ".doc")));
					MVCFunctions.Echo(MVCFunctions.Concat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=", GlobalVars.cCharset, "\">"));
				}
			}
			this.doCommonAssignments();
			extraParams = XVar.Clone(this.getExtraReportParams());
			this.setGoogleMapsParams((XVar)(extraParams["fieldsArr"]));
			this.fillAdvancedMapData();
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.assignTotalsDefaults();
			}
			RunnerContext.pushSearchContext((XVar)(this.searchClauseObj));
			this.setReportData((XVar)(extraParams));
			this.addButtonHandlers();
			this.addCommonJs();
			this.showPage();
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public override XVar doCommonAssignments()
		{
			this.assignBody();
			this.xt.assign(new XVar("stylesheetlink"), (XVar)(0 < MVCFunctions.strlen((XVar)(this.format))));
			foreach (KeyValuePair<XVar, dynamic> fName in this.pSet.getFieldsList().GetEnumerator())
			{
				this.xt.assign((XVar)(MVCFunctions.Concat(fName.Value, "_fieldheader")), new XVar(true));
			}
			if(1 < MVCFunctions.count(this.gridTabs))
			{
				dynamic curTabId = null;
				curTabId = XVar.Clone(this.getCurrentTabId());
				this.xt.assign(new XVar("printtabheader"), new XVar(true));
				this.xt.assign(new XVar("printtabheader_text"), (XVar)(this.getTabTitle((XVar)(curTabId))));
			}
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getPageFields().GetEnumerator())
			{
				dynamic gf = null;
				gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(f.Value)));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_class")), (XVar)(this.fieldClass((XVar)(f.Value))));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_align")), (XVar)(this.fieldAlign((XVar)(f.Value))));
			}
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.xt.assign(new XVar("pdfFonts"), (XVar)(MVCFunctions.runner_json_encode((XVar)(CommonFunctions.getPdfFonts()))));
			}

			return null;
		}
		protected override XVar setRecordsId()
		{
			dynamic i = null, j = null, page = XVar.Array(), pageCount = null, recCount = null;
			pageCount = XVar.Clone(MVCFunctions.count(this.arrReport["list"]));
			i = new XVar(0);
			for(;i < pageCount; ++(i))
			{
				page = this.arrReport["list"][i];
				recCount = XVar.Clone(MVCFunctions.count(page));
				j = new XVar(0);
				for(;j < recCount; ++(j))
				{
					this.genId();
					page.InitAndSetArrayItem(this.recId, j, "recId");
				}
			}

			return null;
		}
		protected override XVar assignBody()
		{
			if(XVar.Pack(!(XVar)(this.pdfJsonMode())))
			{
				this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage(new XVar(false)));
				this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
			}
			this.xt.assignbyref(new XVar("body"), (XVar)(this.body));
			this.xt.assign(new XVar("grid_block"), new XVar(true));
			this.xt.assign(new XVar("grid_header"), new XVar(true));
			if((XVar)(this.format)  && (XVar)(this.format != "pdf"))
			{
				this.body.InitAndSetArrayItem("", "begin");
				this.body.InitAndSetArrayItem("", "end");
				this.xt.assignbyref(new XVar("body"), (XVar)(this.body));
			}

			return null;
		}
		protected virtual XVar getnoRecOnFirstPageWhereCondition()
		{
			return "";
		}
		protected override XVar crossTableCommonAssign(dynamic _param_showSummary)
		{
			#region pass-by-value parameters
			dynamic showSummary = XVar.Clone(_param_showSummary);
			#endregion

			dynamic grid_row = XVar.Array(), pages = XVar.Array();
			this.xt.assign(new XVar("report_cross_header"), (XVar)(this.crossTableObj.getPrintCrossHeader()));
			this.xt.assign(new XVar("totals"), (XVar)(this.crossTableObj.getTotalsName()));
			grid_row.InitAndSetArrayItem(this.crossTableObj.getCrossTableData(), "data");
			if(0 < MVCFunctions.count(grid_row["data"]))
			{
				dynamic group_headerArr = XVar.Array();
				this.xt.assign(new XVar("grid_row"), (XVar)(grid_row));
				group_headerArr = XVar.Clone(this.crossTableObj.getCrossTableHeader());
				this.xt.assignbyref(new XVar("group_header"), (XVar)(group_headerArr));
				this.xt.assign(new XVar("group_x_count"), (XVar)(MVCFunctions.count(group_headerArr["data"])));
				this.xt.assign(new XVar("group_x_label"), (XVar)(this.crossTableObj.getXGroupLabel()));
				this.xt.assign(new XVar("group_y_label"), (XVar)(this.crossTableObj.getYGroupLabel()));
				this.xt.assign(new XVar("report_cross_field"), (XVar)(this.crossTableObj.getDataFieldLabel()));
				this.xt.assign(new XVar("report_cross_type"), (XVar)(this.crossTableObj.getTotalsName()));
				this.xt.assignbyref(new XVar("col_summary"), (XVar)(this.crossTableObj.getCrossTableSummary()));
				this.xt.assignbyref(new XVar("total_summary"), (XVar)(this.crossTableObj.getTotalSummary()));
				this.xt.assign(new XVar("cross_totals"), (XVar)(showSummary));
				if((XVar)(this.pdfJsonMode())  && (XVar)(1 < MVCFunctions.count(group_headerArr["data"])))
				{
					dynamic cells = XVar.Array(), i = null;
					cells = XVar.Clone(XVar.Array());
					i = new XVar(0);
					for(;i < MVCFunctions.count(group_headerArr["data"]) - 1; i++)
					{
						cells.InitAndSetArrayItem(true, null);
					}
					this.xt.assign(new XVar("fake_header_cells"), (XVar)(new XVar("data", cells)));
				}
			}
			else
			{
				if(XVar.Pack(this.isDetail))
				{
					this.isReportEmpty = new XVar(true);
					return null;
				}
			}
			pages = XVar.Clone(XVar.Array());
			pages.InitAndSetArrayItem(grid_row, 0, "grid_row");
			pages.InitAndSetArrayItem("<div name=page class=printpage>", 0, "begin");
			pages.InitAndSetArrayItem("</div>", 0, "end");
			this.xt.assign(new XVar("pageno"), new XVar(1));
			this.xt.assign(new XVar("maxpages"), new XVar(1));
			this.xt.assign(new XVar("printheader"), new XVar(true));
			this.xt.assign_loopsection(new XVar("pages"), (XVar)(pages));

			return null;
		}
		public override XVar setStandartData(dynamic _options)
		{
			dynamic pageSize = null, pagestart = null, rb = null;
			GlobalVars.init_reportlib();
			if(XVar.Pack(!(XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")])))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")] = -1;
			}
			if(XVar.Pack(!(XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")])))
			{
				XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")] = 1;
			}
			if((XVar)((XVar)(MVCFunctions.REQUESTKeyExists("all"))  && (XVar)(MVCFunctions.postvalue("all")))  || (XVar)(this.isDetail))
			{
				this.pageData.InitAndSetArrayItem(true, "printAll");
				pageSize = new XVar(0);
				pagestart = new XVar(0);
				this.controlsMap.InitAndSetArrayItem(1, "pdfSettings", "allPagesMode");
			}
			else
			{
				pageSize = XVar.Clone(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")]);
				pagestart = XVar.Clone((XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")] - 1) * pageSize);
			}
			rb = XVar.Clone(new Report((XVar)(this.pSet.getOrderIndexes()), (XVar)(this.connection), (XVar)(pageSize), (XVar)(this.splitByGroups), (XVar)(_options), this));
			this.arrReport = XVar.Clone(rb.getReport((XVar)(pagestart)));
			this.arrPages = XVar.Clone(rb.getPages());
			this.standardReportCommonAssign();
			this.assignColumnHeaderClasses();

			return null;
		}
		protected override XVar standardReportCommonAssign()
		{
			this.xt.assign(new XVar("printheader"), new XVar(true));
			if(XVar.Pack(this.splitAtServer))
			{
				this.standardReportCommonAssignSplit();
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> value in this.arrReport["page"].GetEnumerator())
			{
				this.xt.assign((XVar)(value.Key), (XVar)(value.Value));
			}
			if((XVar)(this.isDetail)  && (XVar)(!(XVar)(this.arrReport["list"])))
			{
				this.isReportEmpty = new XVar(true);
				return null;
			}
			this.xt.assign_loopsection(new XVar("grid_row"), (XVar)(this.arrReport["list"]));
			if(XVar.Pack(this.arrReport["global"]))
			{
				foreach (KeyValuePair<XVar, dynamic> value in this.arrReport["global"].GetEnumerator())
				{
					this.xt.assign((XVar)(value.Key), (XVar)(value.Value));
				}
			}
			this.xt.assign(new XVar("pageno"), new XVar(1));
			this.xt.assign(new XVar("maxpages"), new XVar(1));
			this.xt.assign(new XVar("global_summary"), new XVar(true));
			this.xt.assign(new XVar("pages"), new XVar(true));

			return null;
		}
		protected virtual XVar standardReportCommonAssignSplit()
		{
			dynamic page = XVar.Array(), pageno = null, total = null;
			page = XVar.Clone(new XVar("grid_row", new XVar("data", XVar.Array())));
			pageno = new XVar(1);
			this.setRecordsId();
			foreach (KeyValuePair<XVar, dynamic> pagerecords in this.arrReport["list"].GetEnumerator())
			{
				page.InitAndSetArrayItem(pagerecords.Value, "grid_row", "data");
				this.addPage((XVar)(page), (XVar)(pageno));
				++(pageno);
				page = XVar.Clone(new XVar("grid_row", new XVar("data", XVar.Array())));
			}
			if(XVar.Pack(this.arrReport["global"]))
			{
				dynamic lastPage = XVar.Array();
				lastPage = this.pages[MVCFunctions.count(this.pages) - 1];
				foreach (KeyValuePair<XVar, dynamic> value in this.arrReport["global"].GetEnumerator())
				{
					lastPage.InitAndSetArrayItem(value.Value, value.Key);
				}
				lastPage.InitAndSetArrayItem(true, "global_summary");
			}
			this.xt.assign(new XVar("maxpages"), (XVar)(pageno));
			total = XVar.Clone(MVCFunctions.count(this.pages));
			foreach (KeyValuePair<XVar, dynamic> mLString in this.pSet.printPagesLabelsData().GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> pageBody in this.pages.GetEnumerator())
				{
					this.pages.InitAndSetArrayItem(MVCFunctions.str_replace(new XVar("%total%"), (XVar)(total), (XVar)(pageBody.Value[MVCFunctions.Concat("print_pages_label", mLString.Key)])), pageBody.Key, MVCFunctions.Concat("print_pages_label", mLString.Key));
				}
			}
			this.body.InitAndSetArrayItem(this.pages, "data");
			this.xt.assign(new XVar("page_number"), new XVar(true));
			this.xt.assign(new XVar("pagecount"), (XVar)(pageno - 1));
			this.xt.assign(new XVar("pages"), new XVar(true));

			return null;
		}
		protected virtual XVar addPage(dynamic page, dynamic _param_pageno)
		{
			#region pass-by-value parameters
			dynamic pageno = XVar.Clone(_param_pageno);
			#endregion

			if(pageno == 1)
			{
				page.InitAndSetArrayItem(this.pSet.isPrinterPagePDF(), "pdflink_block");
			}
			if(XVar.Pack(!(XVar)(this.pdfJsonMode())))
			{
				page.InitAndSetArrayItem("<div class=\"rp-presplitpage rp-page\">", "begin");
				page.InitAndSetArrayItem("</div>", "end");
			}
			page.InitAndSetArrayItem(pageno, "pageno");
			if(XVar.Pack(MVCFunctions.is_array((XVar)(this.arrPages[pageno - 1]))))
			{
				foreach (KeyValuePair<XVar, dynamic> value in this.arrPages[pageno - 1].GetEnumerator())
				{
					page.InitAndSetArrayItem(value.Value, value.Key);
				}
			}
			page.InitAndSetArrayItem(pageno, "pageno");
			this.xt.assign(new XVar("print_pages"), new XVar(true));
			foreach (KeyValuePair<XVar, dynamic> mLString in this.pSet.printPagesLabelsData().GetEnumerator())
			{
				dynamic label = null;
				label = XVar.Clone(MVCFunctions.str_replace(new XVar("%current%"), (XVar)(pageno), (XVar)(CommonFunctions.GetMLString((XVar)(mLString.Value)))));
				page.InitAndSetArrayItem(label, MVCFunctions.Concat("print_pages_label", mLString.Key));
			}
			this.pages.InitAndSetArrayItem(page, null);

			return null;
		}
		public virtual XVar prepareWordOrExcelTemplate(dynamic _param_contents)
		{
			#region pass-by-value parameters
			dynamic contents = XVar.Clone(_param_contents);
			#endregion

			dynamic pos1 = null;
			pos1 = new XVar(0);
			while(!XVar.Equals(XVar.Pack(pos1), XVar.Pack(false)))
			{
				pos1 = XVar.Clone(MVCFunctions.stripos((XVar)(contents), new XVar("<link "), (XVar)(pos1)));
				if(!XVar.Equals(XVar.Pack(pos1), XVar.Pack(false)))
				{
					dynamic pos2 = null;
					pos2 = XVar.Clone(MVCFunctions.strpos((XVar)(contents), new XVar(">"), (XVar)(pos1)));
					if(!(XVar)(pos2) == false)
					{
						contents = XVar.Clone(MVCFunctions.Concat(MVCFunctions.substr((XVar)(contents), new XVar(0), (XVar)(pos1)), MVCFunctions.substr((XVar)(contents), (XVar)(pos2 + 1))));
					}
				}
			}
			contents = XVar.Clone(MVCFunctions.str_ireplace(new XVar("<img src=\"/images/spacer.gif\">"), new XVar(""), (XVar)(contents)));
			contents = XVar.Clone(MVCFunctions.str_ireplace(new XVar("<img src=\"/images/spacer.gif\"/>"), new XVar(""), (XVar)(contents)));
			contents = XVar.Clone(MVCFunctions.str_ireplace(new XVar("<img src=\"@webRootPath/images/spacer.gif\" />"), new XVar(""), (XVar)(contents)));
			return contents;
		}
		public virtual XVar processDetailPrint()
		{
			dynamic extraParams = null;
			if((XVar)(this.crossTable)  && (XVar)(!(XVar)(this.checkCrossParams())))
			{
				this.setDefaultParams();
			}
			extraParams = XVar.Clone(this.getExtraReportParams());
			this.setReportData((XVar)(extraParams));
			if(XVar.Pack(this.isReportEmpty))
			{
				return null;
			}
			this.xt.assign(new XVar("grid_header"), new XVar(true));
			this.showDetailPrint();

			return null;
		}
		public virtual XVar showDetailPrint()
		{
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
			this.xt.load_template((XVar)(this.templatefile));
			MVCFunctions.Echo(MVCFunctions.Concat("<div class=\"panel panel-info details-grid\">\r\n\t\t\t<div class=\"panel-heading\">\r\n\t\t\t\t<h4 class=\"panel-title\">", this.getPageTitle((XVar)(this.pageName), (XVar)(this.tName)), "</h4>\r\n\t\t\t</div>\r\n\t\t\t<div class=\"panel-body\">"));
			MVCFunctions.Echo(this.fetchForms((XVar)(new XVar(0, "grid"))));
			MVCFunctions.Echo("</div>\r\n\t\t</div>");

			return null;
		}
		public override XVar showPage()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowReportPrint"))))
			{
				this.eventsObject.BeforeShowReportPrint((XVar)(this.xt), ref this.templatefile, this);
			}
			if(XVar.Pack(this.pdfJsonMode()))
			{
				this.preparePDFBackground();
				this.xt.assign(new XVar("standalone_page"), new XVar(true));
				this.xt.displayJSON((XVar)(this.templatefile));
				return null;
			}
			if((XVar)(this.format == "excel")  || (XVar)(this.format == "word"))
			{
				dynamic contents = null;
				this.xt.load_template((XVar)(this.templatefile));
				contents = XVar.Clone(this.prepareWordOrExcelTemplate((XVar)(this.xt.template)));
				this.xt.template = XVar.Clone(contents);
				this.xt.display_loaded();
			}
			else
			{
				if(this.format == "pdf")
				{
					this.hideElement(new XVar("printpdf"));
					this.AddCSSFile(new XVar("styles/defaultPDF.css"));
					this.assignStyleFiles();
					this.xt.load_template((XVar)(this.templatefile));
					this.xt.display_loaded();
				}
				else
				{
					this.display((XVar)(this.templatefile));
				}
			}

			return null;
		}
		public override XVar element2Item(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(name == "printpdf")
			{
				return new XVar(0, "print_pdf");
			}
			return base.element2Item((XVar)(name));
		}
		public override XVar pdfJsonMode()
		{
			return this.mode == Constants.PRINT_PDFJSON;
		}
		public virtual XVar assignTotalsDefaults()
		{
			dynamic fields = XVar.Array(), summaries = XVar.Array(), totals = XVar.Array();
			totals = XVar.Clone(new XVar(0, "min", 1, "max", 2, "avg", 3, "sum"));
			summaries = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> group in this.pSet.getReportGroupFieldsData().GetEnumerator())
			{
				summaries.InitAndSetArrayItem(MVCFunctions.Concat("group", group.Value["strGroupField"]), null);
			}
			summaries.InitAndSetArrayItem("page", null);
			summaries.InitAndSetArrayItem("global", null);
			fields = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in this.pSet.getFieldsList().GetEnumerator())
			{
				fields.InitAndSetArrayItem(MVCFunctions.GoodFieldName((XVar)(field.Value)), null);
			}
			foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> s in summaries.GetEnumerator())
				{
					foreach (KeyValuePair<XVar, dynamic> t in totals.GetEnumerator())
					{
						this.xt.assign((XVar)(MVCFunctions.Concat(s.Value, "_total", f.Value, "_", t.Value)), new XVar("''"));
					}
				}
			}

			return null;
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
			if((XVar)((XVar)(MVCFunctions.REQUESTKeyExists("all"))  && (XVar)(MVCFunctions.postvalue("all")))  || (XVar)(this.isDetail))
			{
				settings.InitAndSetArrayItem(1, "reportPrintMode");
			}
			settings.InitAndSetArrayItem(CommonFunctions.isRTL(), "isRTL");
			settings.InitAndSetArrayItem((XVar.Pack(this.crossTable) ? XVar.Pack(1) : XVar.Pack(0)), "reportType");
			settings.InitAndSetArrayItem(1, "reportPrintPartitionType");
			settings.InitAndSetArrayItem(this.pSet.getReportPrintGroupsPerPage(), "reportPrintGroupsPerPage");
			settings.InitAndSetArrayItem(this.pSet.getReportLayout(), "reportPrintLayout");
			settings.InitAndSetArrayItem(this.pSet.isPrinterPagePDF(), "printerPagePDF");
			settings.InitAndSetArrayItem(this.pSet.getPrinterPageOrientation(), "printerPageOrientation");
			settings.InitAndSetArrayItem(this.pSet.getPrinterPageScale(), "printerPageScale");
			settings.InitAndSetArrayItem(this.pSet.isPrinterPageFitToPage(), "isPrinterPageFitToPage");
			settings.InitAndSetArrayItem(this.pSet.getReportPrintGroupsPerPage(), "printerSplitRecords");
			return settings;
		}
	}
}
