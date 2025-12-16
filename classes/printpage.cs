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
	public partial class PrintPage : RunnerPage
	{
		public dynamic allPagesMode = XVar.Pack(false);
		public dynamic recordset = XVar.Pack(null);
		public dynamic fetchedRecordCount = XVar.Pack(0);
		public dynamic splitByRecords = XVar.Pack(0);
		public dynamic detailTables;
		public dynamic pageBody = XVar.Array();
		protected dynamic recordsRenderData = XVar.Array();
		public dynamic totals = XVar.Array();
		public dynamic totalRowCount = XVar.Pack(false);
		public dynamic queryPageNo = XVar.Pack(1);
		public dynamic queryPageSize = XVar.Pack(0);
		public dynamic _eof = XVar.Pack(false);
		public dynamic nextRecord = XVar.Pack(null);
		public dynamic customFieldForSort = XVar.Array();
		public dynamic customHowFieldSort = XVar.Array();
		public dynamic pageNo = XVar.Pack(1);
		public dynamic hideColumns = XVar.Array();
		protected dynamic _notEmptyFieldColumns = XVar.Array();
		protected static bool skipPrintPageCtor = false;
		public PrintPage(dynamic var_params = null)
			:base((XVar)var_params)
		{
			if(skipPrintPageCtor)
			{
				skipPrintPageCtor = false;
				return;
			}
			#region default values
			if(var_params as Object == null) var_params = new XVar("");
			#endregion

			this.pSetEdit = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_SEARCH)));
			if(XVar.Pack(this.selection))
			{
				this.allPagesMode = new XVar(true);
			}
			if(XVar.Pack(!(XVar)(this.detailTables)))
			{
				this.detailTables = XVar.Clone(XVar.Array());
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(this.detailTables)))))
			{
				this.detailTables = XVar.Clone(new XVar(0, this.detailTables));
			}
			this.pageData.InitAndSetArrayItem(this.selection, "printSelection");
			this.pageData.InitAndSetArrayItem(this.detailTables, "printDetails");
			this.pageData.InitAndSetArrayItem(this.allPagesMode, "printAll");
			this.printGridLayout = XVar.Clone(this.pSet.getPrintGridLayout());
			this.recsPerRowPrint = XVar.Clone(this.pSet.getRecordsPerRowPrint());
			if(XVar.Pack(!(XVar)(this.recsPerRowPrint)))
			{
				this.recsPerRowPrint = new XVar(1);
			}
			this.totalsFields = XVar.Clone(this.pSet.getTotalsFields());
			if(XVar.Pack(!(XVar)(this.splitByRecords)))
			{
				this.splitByRecords = XVar.Clone(this.pSet.getPrinterSplitRecords());
			}
			this.pageData.InitAndSetArrayItem(this.splitByRecords, "printRecords");
			if(XVar.Pack(this.showHideFieldsFeatureEnabled()))
			{
				dynamic hideColumns = XVar.Array();
				hideColumns = XVar.Clone(this.getColumnsToHide());
				this.hideColumns = XVar.Clone(hideColumns[Constants.DESKTOP]);
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(this.hideColumns)))))
				{
					this.hideColumns = XVar.Clone(XVar.Array());
				}
				foreach (KeyValuePair<XVar, dynamic> f in this.hideColumns.GetEnumerator())
				{
					this.hideField((XVar)(this.pSet.getFieldByGoodFieldName((XVar)(f.Value))));
				}
			}
		}
		public static XVar readSelectedRecordsFromRequest(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic keyFields = XVar.Array(), selected_recs = XVar.Array();
			ProjectSettings pSet;
			if(XVar.Pack(!(XVar)(MVCFunctions.postvalue(new XVar("selection")))))
			{
				return XVar.Array();
			}
			pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(table)));
			keyFields = XVar.Clone(pSet.getTableKeys());
			selected_recs = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> keyblock in MVCFunctions.postvalue(new XVar("selection")).GetEnumerator())
			{
				dynamic arr = XVar.Array(), keys = XVar.Array();
				arr = XVar.Clone(MVCFunctions.explode(new XVar("&"), (XVar)(keyblock.Value)));
				if(MVCFunctions.count(arr) < MVCFunctions.count(keyFields))
				{
					continue;
				}
				keys = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> value in arr.GetEnumerator())
				{
					keys.InitAndSetArrayItem(MVCFunctions.urldecode((XVar)(value.Value)), keyFields[value.Key]);
				}
				selected_recs.InitAndSetArrayItem(keys, null);
			}
			return selected_recs;
		}
		protected virtual XVar prepareCustomListQueryLegacySorting()
		{
			dynamic arrFieldForSort = null, arrHowFieldSort = null, fieldList = XVar.Array(), i = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("ListQuery")))))
			{
				return null;
			}
			arrFieldForSort = XVar.Clone(XVar.Array());
			arrHowFieldSort = XVar.Clone(XVar.Array());
			fieldList = XVar.Clone(MVCFunctions.unserialize((XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_orderFieldsList")])));
			i = new XVar(0);
			for(;(XVar)(fieldList)  && (XVar)(i < MVCFunctions.count(fieldList)); i++)
			{
				this.customFieldForSort.InitAndSetArrayItem(fieldList[i].fieldIndex, null);
				this.customHowFieldSort.InitAndSetArrayItem(fieldList[i].orderDirection, null);
			}

			return null;
		}
		protected virtual XVar calcPageSizeAndNumber()
		{
			if(XVar.Pack(this.allPagesMode))
			{
				return null;
			}
			this.queryPageNo = XVar.Clone((int)XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagenumber")]);
			if(XVar.Pack(!(XVar)(this.queryPageNo)))
			{
				this.queryPageNo = new XVar(1);
			}
			this.queryPageSize = XVar.Clone((int)XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_pagesize")]);
			if(XVar.Pack(!(XVar)(this.queryPageSize)))
			{
				this.queryPageSize = XVar.Clone(this.pSet.getInitialPageSize());
			}
			if(this.queryPageSize < 0)
			{
				this.allPagesMode = new XVar(true);
			}

			return null;
		}
		protected virtual XVar setMapParams()
		{
			dynamic fieldsArr = XVar.Array();
			fieldsArr = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getPrinterFields().GetEnumerator())
			{
				fieldsArr.InitAndSetArrayItem(new XVar("fName", f.Value, "viewFormat", this.pSet.getViewFormat((XVar)(f.Value))), null);
			}
			this.setGoogleMapsParams((XVar)(fieldsArr));

			return null;
		}
		public virtual XVar process()
		{
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessPrint"))))
			{
				this.eventsObject.BeforeProcessPrint(this);
			}
			MVCFunctions.loadMaps((XVar)(this.pSet));
			this.processGridTabs();
			this.setMapParams();
			RunnerContext.pushSearchContext((XVar)(this.searchClauseObj));
			this.calcPageSizeAndNumber();
			this.calculateRecordCount();
			this.recordset = XVar.Clone(this.dataSource.getList((XVar)(this.queryCommand)));
			if(XVar.Pack(!(XVar)(this.recordset)))
			{
				MVCFunctions.showError((XVar)(this.dataSource.lastError()));
			}
			this.doFirstPageAssignments();
			if(XVar.Pack(!(XVar)(this.splitByRecords)))
			{
				this.fillGridPage();
				this.assignTotals();
				this.displayMasterTableInfo();
				this.addPage();
			}
			else
			{
				dynamic masterAdded = null;
				masterAdded = new XVar(false);
				while(XVar.Pack(true))
				{
					if(XVar.Pack(!(XVar)(masterAdded)))
					{
						this.displayMasterTableInfo();
						masterAdded = new XVar(true);
					}
					else
					{
						this.pageBody.InitAndSetArrayItem(false, "container_master");
						this.pageBody.InitAndSetArrayItem(false, "container_pdf");
					}
					this.fillGridPage();
					if(XVar.Pack(this.EOF()))
					{
						break;
					}
					this.wrapPageBody();
					this.addPage();
					++(this.pageNo);
					this.pageBody = XVar.Clone(XVar.Array());
				}
				this.assignTotals();
				this.wrapPageBody();
				this.addPage();
			}
			this.hideEmptyFields();
			this.addButtonHandlers();
			this.addCommonJs();
			this.commonAssign();
			this.fillAdvancedMapData();
			this.doCommonAssignments();
			this.addCustomCss();
			this.addDetailsCss();
			this.displayPrintPage();

			return null;
		}
		protected virtual XVar calcAllDataTotals()
		{
			dynamic currentPageSize = null;
			currentPageSize = XVar.Clone(this.queryPageSize);
			if((XVar)(!(XVar)(this.allPagesMode))  && (XVar)(this.pSet.getRecordsLimit()))
			{
				currentPageSize = XVar.Clone(this.pSet.getRecordsLimit() - this.queryPageSize * (this.queryPageNo - 1));
			}
			return (XVar)((XVar)(this.pSet.calcTotalsFor() == Constants.TOTALS_ALL_DATA)  && (XVar)(!(XVar)(this.allPagesMode)))  && (XVar)(this.queryPageSize < this.totalRowCount);
		}
		protected virtual XVar hideEmptyFields()
		{
			if(this.printGridLayout == Constants.gltHORIZONTAL)
			{
				foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getFieldsToHideIfEmpty().GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(this._notEmptyFieldColumns[f.Value])))
					{
						this.hideField((XVar)(f.Value));
					}
				}
			}

			return null;
		}
		public virtual XVar addPage()
		{
			dynamic pageIdx = null, pageRows = null;
			this.body.InitAndSetArrayItem(this.pageBody, "data", null);
			pageIdx = XVar.Clone(MVCFunctions.count(this.body["data"]) - 1);
			pageRows = this.body["data"][pageIdx]["grid_row"]["data"];
			this.fillRenderedData((XVar)(pageRows));

			return null;
		}
		protected virtual XVar fillRenderedData(dynamic pageRows)
		{
			dynamic rowIdx = null;
			rowIdx = new XVar(0);
			for(;rowIdx < MVCFunctions.count(pageRows); ++(rowIdx))
			{
				if(XVar.Pack(!(XVar)(this.manyRecordsInRow())))
				{
					this.recordsRenderData.InitAndSetArrayItem(pageRows[rowIdx], pageRows[rowIdx]["recId"]);
				}
				else
				{
					dynamic recordIdx = null, records = XVar.Array();
					records = pageRows[rowIdx]["grid_record"]["data"];
					recordIdx = new XVar(0);
					for(;recordIdx < MVCFunctions.count(records); ++(recordIdx))
					{
						this.recordsRenderData.InitAndSetArrayItem(records[recordIdx], records[recordIdx]["recId"]);
					}
				}
			}

			return null;
		}
		protected virtual XVar wrapPageBody()
		{
			this.pageBody.InitAndSetArrayItem("<div class=\"rp-presplitpage rp-page\">", "begin");
			this.pageBody.InitAndSetArrayItem("</div>", "end");

			return null;
		}
		protected virtual XVar assignTotals()
		{
			if(XVar.Pack(!(XVar)(this.totalsFields)))
			{
				return null;
			}
			if(XVar.Pack(this.calcAllDataTotals()))
			{
				this.buildAllDataTotals();
			}
			else
			{
				this.buildTotals((XVar)(this.totals));
			}

			return null;
		}
		public override XVar buildTotals(dynamic totals)
		{
			dynamic record = XVar.Array();
			record = XVar.Clone(XVar.Array());
			this.pageBody.InitAndSetArrayItem(true, "totals_record");
			foreach (KeyValuePair<XVar, dynamic> tf in this.totalsFields.GetEnumerator())
			{
				dynamic total = null;
				total = XVar.Clone(CommonFunctions.GetTotals((XVar)(tf.Value["fName"]), (XVar)(totals[tf.Value["fName"]]), (XVar)(tf.Value["totalsType"]), (XVar)(tf.Value["numRows"]), (XVar)(tf.Value["viewFormat"]), new XVar(Constants.PAGE_PRINT), (XVar)(this.pSet), new XVar(false), this));
				this.pageBody.InitAndSetArrayItem(total, MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(tf.Value["fName"])), "_total"));
				this.pageBody.InitAndSetArrayItem(true, MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(tf.Value["fName"])), "_showtotal"));
				record.InitAndSetArrayItem(true, MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(tf.Value["fName"])), "_showtotal"));
			}
			this.pageBody.InitAndSetArrayItem(new XVar("data", new XVar(0, record)), "totals_row");

			return null;
		}
		protected virtual XVar EOF()
		{
			dynamic currentPageSize = null;
			currentPageSize = XVar.Clone(this.queryPageSize);
			if((XVar)(!(XVar)(this.allPagesMode))  && (XVar)(this.pSet.getRecordsLimit()))
			{
				currentPageSize = XVar.Clone(this.pSet.getRecordsLimit() - this.queryPageSize * (this.queryPageNo - 1));
			}
			else
			{
				if(XVar.Pack(this.allPagesMode))
				{
					currentPageSize = XVar.Clone(this.limitRowCount((XVar)(this.totalRowCount)));
				}
			}
			if(currentPageSize <= this.fetchedRecordCount)
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
		protected virtual XVar readNextRecordInternal()
		{
			dynamic data = null;
			if(XVar.Pack(this._eof))
			{
				return null;
			}
			if(XVar.Pack(this.nextRecord))
			{
				return null;
			}
			while(XVar.Pack(true))
			{
				if(XVar.Pack(this.eventsObject.exists(new XVar("ListFetchArray"))))
				{
					data = XVar.Clone(this.eventsObject.ListFetchArray((XVar)(this.recordset), this));
				}
				else
				{
					data = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(this.recordset.fetchAssoc())));
				}
				if(XVar.Pack(!(XVar)(data)))
				{
					this._eof = new XVar(true);
					return null;
				}
				if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeProcessRowPrint"))))
				{
					if(XVar.Pack(!(XVar)(this.eventsObject.BeforeProcessRowPrint((XVar)(data), this))))
					{
						continue;
					}
				}
				this.nextRecord = XVar.Clone(data);
				return null;
			}

			return null;
		}
		protected virtual XVar readNextRecord()
		{
			dynamic data = null;
			if(XVar.Pack(this.EOF()))
			{
				return false;
			}
			++(this.fetchedRecordCount);
			data = XVar.Clone(this.nextRecord);
			this.nextRecord = new XVar(false);
			return data;
		}
		protected virtual XVar buildGridRecord(dynamic _param_data, dynamic row)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic dbValue = null, fieldsToHideIfEmpty = null, i = null, isEmptyValue = null, keyFields = XVar.Array(), keylink = null, keys = XVar.Array(), printFields = XVar.Array(), record = XVar.Array();
			this.genId();
			record = XVar.Clone(XVar.Array());
			record.InitAndSetArrayItem(MVCFunctions.Concat("data-record-id=\"", this.recId, "\""), "recordattrs");
			record.InitAndSetArrayItem(this.recId, "recId");
			if(XVar.Pack(!(XVar)(this.calcAllDataTotals())))
			{
				this.countTotals((XVar)(this.totals), (XVar)(data));
			}
			keyFields = XVar.Clone(this.pSet.getTableKeys());
			keylink = new XVar("");
			keys = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(keyFields); i++)
			{
				keylink = MVCFunctions.Concat(keylink, "&key", i + 1, "=", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.RawUrlEncode((XVar)(data[keyFields[i]])))));
				keys.InitAndSetArrayItem(data[keyFields[i]], i);
			}
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeMoveNextPrint"))))
			{
				this.eventsObject.BeforeMoveNextPrint((XVar)(data), (XVar)(row), (XVar)(record), (XVar)(record["recId"]), this);
			}
			fieldsToHideIfEmpty = XVar.Clone(this.pSet.getFieldsToHideIfEmpty());
			printFields = this.pSet.getPrinterFields();
			i = new XVar(0);
			for(;i < MVCFunctions.count(printFields); i++)
			{
				dbValue = XVar.Clone(this.showDBValue((XVar)(printFields[i]), (XVar)(data), (XVar)(keylink)));
				if(XVar.Pack(!(XVar)(this.pdfJsonMode())))
				{
					record.InitAndSetArrayItem(dbValue, MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(printFields[i])), "_value"));
				}
				else
				{
					record.InitAndSetArrayItem(dbValue, MVCFunctions.Concat(MVCFunctions.GoodFieldName((XVar)(printFields[i])), "_pdfvalue"));
				}
				isEmptyValue = XVar.Clone((XVar)((XVar)(this.pdfJsonMode())  && (XVar)(dbValue == "''"))  || (XVar)((XVar)(!(XVar)(this.pdfJsonMode()))  && (XVar)(dbValue == XVar.Pack(""))));
				if(XVar.Pack(MVCFunctions.in_array((XVar)(printFields[i]), (XVar)(fieldsToHideIfEmpty))))
				{
					if((XVar)(this.printGridLayout != Constants.gltHORIZONTAL)  && (XVar)(isEmptyValue))
					{
						this.hideField((XVar)(printFields[i]), (XVar)(this.recId));
					}
					else
					{
						if((XVar)(this.printGridLayout == Constants.gltHORIZONTAL)  && (XVar)(!(XVar)(isEmptyValue)))
						{
							this._notEmptyFieldColumns.InitAndSetArrayItem(true, printFields[i]);
						}
					}
				}
				this.setRowClassNames((XVar)(record), (XVar)(printFields[i]));
			}
			this.spreadRowStyles((XVar)(data), (XVar)(row), (XVar)(record));
			this.setRowCssRules((XVar)(record));
			record.InitAndSetArrayItem(true, "grid_recordheader");
			record.InitAndSetArrayItem(true, "grid_vrecord");
			if(XVar.Pack(this.pSet.hasMap()))
			{
				this.addBigGoogleMapMarkers((XVar)(data), (XVar)(keys));
			}
			return record;
		}
		protected virtual XVar showGridHeader(dynamic _param_columns)
		{
			#region pass-by-value parameters
			dynamic columns = XVar.Clone(_param_columns);
			#endregion

			dynamic i = null, rfooter = XVar.Array(), rheader = XVar.Array();
			this.pageBody.InitAndSetArrayItem(new XVar("data", XVar.Array()), "record_header");
			this.pageBody.InitAndSetArrayItem(new XVar("data", XVar.Array()), "record_footer");
			i = new XVar(0);
			for(;i < columns; i++)
			{
				rheader = XVar.Clone(XVar.Array());
				rfooter = XVar.Clone(XVar.Array());
				if(i < columns - 1)
				{
					rheader.InitAndSetArrayItem(true, "endrecordheader_block");
					rfooter.InitAndSetArrayItem(true, "endrecordheader_block");
				}
				this.pageBody.InitAndSetArrayItem(rheader, "record_header", "data", null);
				this.pageBody.InitAndSetArrayItem(rfooter, "record_footer", "data", null);
			}
			this.pageBody.InitAndSetArrayItem(true, "grid_header");
			this.pageBody.InitAndSetArrayItem(true, "grid_footer");

			return null;
		}
		protected virtual XVar manyRecordsInRow()
		{
			return (XVar)(this.printGridLayout == Constants.gltVERTICAL)  || (XVar)(this.recsPerRowPrint != 1);
		}
		protected virtual XVar fillGridPage()
		{
			dynamic assignmentMethod = null, builtrow = XVar.Array(), data = XVar.Array(), prevData = XVar.Array(), recno = null, recordsPrinted = null, row = XVar.Array();
			this.pageBody.InitAndSetArrayItem(XVar.Array(), "grid_row");
			this.pageBody.InitAndSetArrayItem(XVar.Array(), "grid_row", "data");
			recno = new XVar(0);
			recordsPrinted = new XVar(0);
			row = XVar.Clone(XVar.Array());
			while(XVar.Pack(data = XVar.Clone(this.readNextRecord())))
			{
				RunnerContext.pushRecordContext((XVar)(data), this);
				row.InitAndSetArrayItem(XVar.Array(), "details");
				row = XVar.Clone(XVar.Array());
				row.InitAndSetArrayItem(XVar.Array(), "grid_record");
				row.InitAndSetArrayItem(XVar.Array(), "grid_record", "data");
				row.InitAndSetArrayItem(XVar.Array(), "details_record");
				row.InitAndSetArrayItem(XVar.Array(), "details_record", "data");
				if(XVar.Pack(this.manyRecordsInRow()))
				{
					builtrow = XVar.Clone(this.buildGridRecord((XVar)(data), (XVar)(row)));
					foreach (KeyValuePair<XVar, dynamic> dt in this.detailTables.GetEnumerator())
					{
						assignmentMethod = XVar.Clone(this.buildDetailsXtMethod((XVar)(dt.Value), (XVar)(data)));
						if(XVar.Pack(assignmentMethod))
						{
							this.showItemType(new XVar("details_preview"));
							builtrow.InitAndSetArrayItem(true, MVCFunctions.Concat("details_", dt.Value));
							builtrow.InitAndSetArrayItem(assignmentMethod, MVCFunctions.Concat("displayDetailTable_", dt.Value));
						}
					}
					row.InitAndSetArrayItem(builtrow, "grid_record", "data", null);
				}
				else
				{
					builtrow = XVar.Clone(this.buildGridRecord((XVar)(data), (XVar)(row)));
					foreach (KeyValuePair<XVar, dynamic> value in builtrow.GetEnumerator())
					{
						row.InitAndSetArrayItem(value.Value, value.Key);
					}
					row.InitAndSetArrayItem(true, "grid_record");
					foreach (KeyValuePair<XVar, dynamic> dt in this.detailTables.GetEnumerator())
					{
						assignmentMethod = XVar.Clone(this.buildDetailsXtMethod((XVar)(dt.Value), (XVar)(data)));
						if(XVar.Pack(assignmentMethod))
						{
							this.showItemType(new XVar("details_preview"));
							row.InitAndSetArrayItem(true, MVCFunctions.Concat("details_", dt.Value));
							row.InitAndSetArrayItem(assignmentMethod, MVCFunctions.Concat("displayDetailTable_", dt.Value));
						}
					}
				}
				RunnerContext.pop();
				if(XVar.Pack(prevData))
				{
					dynamic grFields = XVar.Array();
					grFields = XVar.Clone(this.pSet.getGroupFields());
					foreach (KeyValuePair<XVar, dynamic> grF in grFields.GetEnumerator())
					{
						if(data[grF.Value] != prevData[grF.Value])
						{
							break;
						}
						foreach (KeyValuePair<XVar, dynamic> fItemId in this.pSet.getFieldItems((XVar)(grF.Value)).GetEnumerator())
						{
							this.hideItem((XVar)(fItemId.Value), (XVar)(builtrow["recId"]));
						}
					}
				}
				prevData = XVar.Clone(data);
				++(recno);
				this.pageBody.InitAndSetArrayItem(row, "grid_row", "data", null);
				if((XVar)(this.splitByRecords)  && (XVar)(this.splitByRecords <= recno))
				{
					break;
				}
			}
			this.showGridHeader((XVar)((XVar.Pack(this.recsPerRowPrint < recno) ? XVar.Pack(this.recsPerRowPrint) : XVar.Pack(recno))));
			this.pageBody.InitAndSetArrayItem(this.pageNo, "pageno");
			if(XVar.Pack(this.allPagesMode))
			{
				this.xt.assign(new XVar("print_pages"), new XVar(true));
				foreach (KeyValuePair<XVar, dynamic> mLString in this.pSet.printPagesLabelsData().GetEnumerator())
				{
					dynamic label = null;
					label = XVar.Clone(MVCFunctions.str_replace(new XVar("%current%"), (XVar)(this.pageNo), (XVar)(CommonFunctions.GetMLString((XVar)(mLString.Value)))));
					this.pageBody.InitAndSetArrayItem(label, MVCFunctions.Concat("print_pages_label", mLString.Key));
				}
			}

			return null;
		}
		public virtual XVar doCommonAssignments()
		{
			this.xt.assign(new XVar("pagecount"), (XVar)(this.pageNo));
			this.body["begin"] = MVCFunctions.Concat(this.body["begin"], CommonFunctions.GetBaseScriptsForPage(new XVar(false)));
			this.body.InitAndSetArrayItem(XTempl.create_method_assignment(new XVar("assignBodyEnd"), this), "end");
			if((XVar)(this.allPagesMode)  && (XVar)(!(XVar)(!(XVar)(this.body["data"]))))
			{
				dynamic total = null;
				total = XVar.Clone(MVCFunctions.count(this.body["data"]));
				foreach (KeyValuePair<XVar, dynamic> mLString in this.pSet.printPagesLabelsData().GetEnumerator())
				{
					foreach (KeyValuePair<XVar, dynamic> pageBody in this.body["data"].GetEnumerator())
					{
						this.body.InitAndSetArrayItem(MVCFunctions.str_replace(new XVar("%total%"), (XVar)(total), (XVar)(pageBody.Value[MVCFunctions.Concat("print_pages_label", mLString.Key)])), "data", pageBody.Key, MVCFunctions.Concat("print_pages_label", mLString.Key));
					}
				}
			}
			if(this.mode == Constants.PRINT_PDFJSON)
			{
				dynamic p = null, pdfBody = XVar.Array();
				pdfBody = this.body;
				pdfBody.Remove("begin");
				pdfBody.Remove("end");
				p = new XVar(0);
				for(;(XVar)(pdfBody["data"])  && (XVar)(p < MVCFunctions.count(pdfBody["data"])); ++(p))
				{
					pdfBody["data"][p].Remove("begin");
					pdfBody["data"][p].Remove("end");
				}
				this.xt.assignbyref(new XVar("body"), (XVar)(pdfBody));
				this.xt.assign(new XVar("pdfFonts"), (XVar)(MVCFunctions.runner_json_encode((XVar)(CommonFunctions.getPdfFonts()))));
			}
			else
			{
				this.xt.assignbyref(new XVar("body"), (XVar)(this.body));
			}
			this.xt.assign(new XVar("grid_block"), new XVar(true));
			this.xt.assign(new XVar("page_number"), new XVar(true));
			this.xt.assign(new XVar("printheader"), new XVar(true));
			if(1 < MVCFunctions.count(this.gridTabs))
			{
				dynamic curTabId = null;
				curTabId = XVar.Clone(this.getCurrentTabId());
				this.xt.assign(new XVar("printtabheader"), new XVar(true));
				this.xt.assign(new XVar("printtabheader_text"), (XVar)(this.getTabTitle((XVar)(curTabId))));
			}
			foreach (KeyValuePair<XVar, dynamic> f in this.pSet.getPrinterFields().GetEnumerator())
			{
				dynamic gf = null;
				gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(f.Value)));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_fieldheadercolumn")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_fieldheader")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_class")), (XVar)(this.fieldClass((XVar)(f.Value))));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_align")), (XVar)(this.fieldAlign((XVar)(f.Value))));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_fieldcolumn")), new XVar(true));
				this.xt.assign((XVar)(MVCFunctions.Concat(gf, "_fieldfootercolumn")), new XVar(true));
			}
			if(XVar.Pack(this.pSet.hasMap()))
			{
				foreach (KeyValuePair<XVar, dynamic> mapId in this.googleMapCfg["mainMapIds"].GetEnumerator())
				{
					this.xt.assign_event((XVar)(mapId.Value), this, new XVar("createMap"), (XVar)(new XVar("mapId", mapId.Value)));
				}
			}

			return null;
		}
		public virtual XVar createMap(dynamic var_params)
		{
			dynamic apiKey = null, height = null, locations = XVar.Array(), mapId = null, markers = XVar.Array(), masData = XVar.Array(), provider = null, src = null, width = null, zoom = null;
			provider = XVar.Clone(CommonFunctions.getMapProvider());
			mapId = XVar.Clone(var_params["mapId"]);
			apiKey = XVar.Clone(this.googleMapCfg["APIcode"]);
			zoom = XVar.Clone(this.googleMapCfg["mapsData"][mapId]["zoom"]);
			markers = XVar.Clone(this.googleMapCfg["mapsData"][mapId]["markers"]);
			masData = XVar.Clone(this.pSet.mapsData());
			width = XVar.Clone(masData[mapId]["width"]);
			if(XVar.Pack(!(XVar)(width)))
			{
				width = XVar.Clone((XVar.Pack(this.googleMapCfg["mapsData"][mapId]["width"]) ? XVar.Pack(this.googleMapCfg["mapsData"][mapId]["width"]) : XVar.Pack(400)));
			}
			height = XVar.Clone(masData[mapId]["height"]);
			if(XVar.Pack(!(XVar)(height)))
			{
				height = XVar.Clone((XVar.Pack(this.googleMapCfg["mapsData"][mapId]["height"]) ? XVar.Pack(this.googleMapCfg["mapsData"][mapId]["height"]) : XVar.Pack(300)));
			}
			locations = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> marker in markers.GetEnumerator())
			{
				if((XVar)(marker.Value["lat"] == "")  && (XVar)(marker.Value["lng"] == ""))
				{
					if(provider == Constants.GOOGLE_MAPS)
					{
						locations.InitAndSetArrayItem(marker.Value["address"], null);
					}
					else
					{
						dynamic locationByAddress = XVar.Array();
						locationByAddress = XVar.Clone(CommonFunctions.getLatLngByAddr((XVar)(marker.Value["address"])));
						locations.InitAndSetArrayItem(MVCFunctions.Concat(locationByAddress["lat"], ",", locationByAddress["lng"]), null);
					}
				}
				else
				{
					locations.InitAndSetArrayItem(MVCFunctions.Concat(marker.Value["lat"], ",", marker.Value["lng"]), null);
				}
			}
			switch(((XVar)provider).ToInt())
			{
				case Constants.GOOGLE_MAPS:
					src = XVar.Clone(MVCFunctions.Concat("https://maps.googleapis.com/maps/api/staticmap?size=", width, "x", height, "&key=", apiKey, "&"));
					if(XVar.Pack(!(XVar)(markers)))
					{
						src = MVCFunctions.Concat(src, "center=0,0&zoom=", (XVar.Pack(zoom) ? XVar.Pack(zoom) : XVar.Pack(5)));
					}
					else
					{
						src = MVCFunctions.Concat(src, (XVar.Pack(zoom) ? XVar.Pack(MVCFunctions.Concat("zoom=", zoom, "&")) : XVar.Pack("")), "markers=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.implode(new XVar("|"), (XVar)(locations)))));
					}
					break;
				case Constants.OPEN_STREET_MAPS:
					src = XVar.Clone(MVCFunctions.Concat("https://staticmap.openstreetmap.de/staticmap.php?size=", width, "x", height, "&"));
					if(XVar.Pack(!(XVar)(markers)))
					{
						src = MVCFunctions.Concat(src, "center=0,0&zoom=", (XVar.Pack(zoom) ? XVar.Pack(zoom) : XVar.Pack(3)));
					}
					else
					{
						src = MVCFunctions.Concat(src, "center=", locations[0], "&zoom=", (XVar.Pack(zoom) ? XVar.Pack(zoom) : XVar.Pack(3)), "&markers=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.implode(new XVar("|"), (XVar)(locations)))));
					}
					break;
				case Constants.BING_MAPS:
					if(XVar.Pack(!(XVar)(markers)))
					{
						src = XVar.Clone(MVCFunctions.Concat("https://dev.virtualearth.net/REST/v1/Imagery/Map/Road/0,0/", (XVar.Pack(zoom) ? XVar.Pack(zoom) : XVar.Pack(5)), "/?key=", apiKey, "&mapSize=", width, ",", height));
					}
					else
					{
						dynamic mParams = null;
						mParams = XVar.Clone(MVCFunctions.Concat("pp=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.implode(new XVar("&pp="), (XVar)(MVCFunctions.array_slice((XVar)(locations), new XVar(0), new XVar(17))))))));
						src = XVar.Clone(MVCFunctions.Concat("https://dev.virtualearth.net/REST/v1/Imagery/Map/Road?", mParams, "&key=", apiKey, "&mapSize=", width, ",", height));
						if(XVar.Pack(zoom))
						{
							src = MVCFunctions.Concat(src, "&zoomLevel=", zoom);
						}
					}
					break;
				case Constants.HERE_MAPS:
				case Constants.MAPQUEST_MAPS:
					if(provider == Constants.HERE_MAPS)
					{
						src = XVar.Clone(MVCFunctions.Concat("https://image.maps.ls.hereapi.com/mia/1.6/mapview?", "apiKey=", apiKey, "&w=", width, "&h=", height, "&poi=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.implode(new XVar(","), (XVar)(locations))))));
						if(XVar.Pack(zoom))
						{
							src = MVCFunctions.Concat(src, "&z=", zoom);
						}
					}
					src = XVar.Clone(MVCFunctions.Concat("https://www.mapquestapi.com/staticmap/v5/map?", "key=", apiKey, "&locations=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.implode(new XVar("||"), (XVar)(locations)))), "&size=", width, ",", height));
					if(XVar.Pack(zoom))
					{
						src = MVCFunctions.Concat(src, "&zoom=", zoom);
					}
					break;
				default:
					src = new XVar("");
					break;
			}
			if(XVar.Pack(this.pdfJsonMode()))
			{
				dynamic content = null, imageType = null;
				content = XVar.Clone(MVCFunctions.myurl_get_contents_binary((XVar)(src)));
				imageType = XVar.Clone(MVCFunctions.SupposeImageType((XVar)(content)));
				if((XVar)(imageType == "image/jpeg")  || (XVar)(imageType == "image/png"))
				{
					MVCFunctions.Echo(MVCFunctions.Concat("{\r\n\t\t\t\t\timage: \"", CommonFunctions.jsreplace((XVar)(MVCFunctions.Concat("data:", imageType, ";base64,", MVCFunctions.base64_bin2str((XVar)(content))))), "\",\r\n\t\t\t\t\twidth: ", width, ",\r\n\t\t\t\t\theight:", height, ",\r\n\t\t\t\t}"));
					return null;
				}
				MVCFunctions.Echo("\"\"");
				return null;
			}
			MVCFunctions.Echo(MVCFunctions.Concat("<img id=\"", var_params["mapId"], "\" src=\"", src, "\">"));

			return null;
		}
		protected override XVar reorderFieldsFeatureEnabled()
		{
			return (XVar)(base.reorderFieldsFeatureEnabled())  && (XVar)(this.pSet.listColumnsOrderOnPrint());
		}
		public virtual XVar displayPrintPage()
		{
			dynamic templatefile = null;
			templatefile = XVar.Clone(this.templatefile);
			if(XVar.Pack(this.eventsObject.exists(new XVar("BeforeShowPrint"))))
			{
				this.eventsObject.BeforeShowPrint((XVar)(this.xt), ref templatefile, this);
			}
			if(this.mode == Constants.PRINT_PDFJSON)
			{
				this.preparePDFBackground();
				this.xt.assign(new XVar("standalone_page"), new XVar(true));
				this.xt.displayJSON((XVar)(this.templatefile));
				return null;
			}
			this.display((XVar)(this.templatefile));

			return null;
		}
		public virtual XVar doFirstPageAssignments()
		{
			this.hideItemType(new XVar("details_preview"));
			foreach (KeyValuePair<XVar, dynamic> mapId in this.googleMapCfg["mainMapIds"].GetEnumerator())
			{
				this.pageBody.InitAndSetArrayItem(true, MVCFunctions.Concat("map_", mapId.Value));
			}
			if(XVar.Pack(this.pSet.isPrinterPagePDF()))
			{
				this.pageBody.InitAndSetArrayItem(true, "pdflink_block");
			}
			else
			{
				this.hideItemType(new XVar("print_pdf"));
			}

			return null;
		}
		public override XVar showField(dynamic _param_fieldName)
		{
			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			#endregion

			dynamic gf = null;
			gf = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(fieldName)));
			foreach (KeyValuePair<XVar, dynamic> value in this.body["data"].GetEnumerator())
			{
				this.body.InitAndSetArrayItem(true, "data", value.Key, MVCFunctions.Concat(gf, "_fieldheadercolumn"));
				this.body.InitAndSetArrayItem(true, "data", value.Key, MVCFunctions.Concat(gf, "_fieldcolumn"));
				this.body.InitAndSetArrayItem(true, "data", value.Key, MVCFunctions.Concat(gf, "_fieldfootercolumn"));
			}

			return null;
		}
		protected virtual XVar addDetailsCss()
		{
			foreach (KeyValuePair<XVar, dynamic> dt in this.detailTables.GetEnumerator())
			{
				dynamic cssFiles = null, dpSet = null, dtName = null, pageLayout = null, pageName = null, pageType = null, tSet = null, tType = null, templatefile = null, viewControls = null;
				dtName = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(dt.Value)));
				tSet = XVar.Clone(new ProjectSettings((XVar)(dtName)));
				tType = XVar.Clone(tSet.getDefaultPageType());
				pageType = XVar.Clone((XVar.Pack(tType == Constants.PAGE_REPORT) ? XVar.Pack(Constants.PAGE_RPRINT) : XVar.Pack(Constants.PAGE_PRINT)));
				pageName = XVar.Clone(this.pSet.detailsPageId((XVar)(dtName)));
				dpSet = XVar.Clone(new ProjectSettings((XVar)(dtName), (XVar)(pageType), (XVar)(pageName)));
				pageLayout = XVar.Clone(CommonFunctions.GetPageLayout((XVar)(dtName), (XVar)(dpSet.pageName())));
				templatefile = XVar.Clone(CommonFunctions.GetTemplateName((XVar)(CommonFunctions.GetTableURL((XVar)(dtName))), (XVar)(dpSet.pageName())));
				cssFiles = XVar.Clone(pageLayout.getCSSFiles((XVar)(CommonFunctions.isRTL())));
				this.AddCSSFile((XVar)(cssFiles));
				viewControls = XVar.Clone(new ViewControlsContainer((XVar)(new ProjectSettings((XVar)(dtName), (XVar)(pageType))), (XVar)(pageType)));
				viewControls.addControlsJSAndCSS();
				this.AddCSSFile((XVar)(viewControls.includes_css));
			}

			return null;
		}
		protected virtual XVar buildDetails(dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic details = XVar.Array();
			details = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> dt in this.detailTables.GetEnumerator())
			{
				dynamic assignmentMethod = null;
				assignmentMethod = XVar.Clone(this.buildDetailsXtMethod((XVar)(dt.Value), (XVar)(data)));
				if(XVar.Pack(assignmentMethod))
				{
					details.InitAndSetArrayItem(new XVar("details", assignmentMethod), null);
				}
			}
			return details;
		}
		protected virtual XVar buildDetailsXtMethod(dynamic _param_dt, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic dt = XVar.Clone(_param_dt);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic dTable = null, dtableArrParams = XVar.Array(), i = null, mkeys = XVar.Array(), tSet = null, tType = null;
			dTable = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(dt)));
			mkeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(dTable)));
			if(XVar.Pack(!(XVar)(mkeys)))
			{
				return false;
			}
			tSet = XVar.Clone(new ProjectSettings((XVar)(dTable)));
			tType = XVar.Clone(tSet.getDefaultPageType());
			dtableArrParams = XVar.Clone(XVar.Array());
			dtableArrParams = XVar.Clone(XVar.Array());
			dtableArrParams.InitAndSetArrayItem(this.genId() + 1, "id");
			dtableArrParams.InitAndSetArrayItem(new XTempl(), "xt");
			dtableArrParams.InitAndSetArrayItem(dTable, "tName");
			dtableArrParams.InitAndSetArrayItem(1 < MVCFunctions.count(this.detailTables), "multipleDetails");
			dtableArrParams.InitAndSetArrayItem(this.pSet.detailsPageId((XVar)(dTable)), "pageName");
			dtableArrParams.InitAndSetArrayItem(this.tName, "masterTable");
			dtableArrParams.InitAndSetArrayItem(XVar.Array(), "masterKeysReq");
			i = new XVar(0);
			foreach (KeyValuePair<XVar, dynamic> mkey in mkeys["masterKeys"].GetEnumerator())
			{
				i++;
				dtableArrParams.InitAndSetArrayItem(data[mkey.Value], "masterKeysReq", i);
			}
			if(tType == Constants.PAGE_REPORT)
			{
				dtableArrParams.InitAndSetArrayItem(Constants.PAGE_RPRINT, "pageType");
				dtableArrParams.InitAndSetArrayItem(true, "isDetail");
			}
			else
			{
				dtableArrParams.InitAndSetArrayItem(Constants.PAGE_PRINT, "pageType");
			}
			if(XVar.Pack(this.pdfJsonMode()))
			{
				dtableArrParams.InitAndSetArrayItem(Constants.PRINT_PDFJSON, "mode");
			}
			return XTempl.create_method_assignment(new XVar("showDetails"), this, (XVar)(dtableArrParams));
		}
		public virtual XVar showDetails(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			dynamic detailsObject = null;
			if(var_params["pageType"] == Constants.PAGE_RPRINT)
			{
				detailsObject = XVar.Clone(new ReportPrintPage((XVar)(var_params)));
				detailsObject.init();
				detailsObject.processDetailPrint();
			}
			else
			{
				detailsObject = XVar.Clone(new PrintPage_Details((XVar)(var_params)));
				detailsObject.init();
				detailsObject.process();
				this.includes_js = XVar.Clone(MVCFunctions.array_merge((XVar)(this.includes_js), (XVar)(detailsObject.includes_js)));
				this.viewControlsMap.InitAndSetArrayItem(detailsObject.viewControlsMap, "dViewControlsMap", var_params["tName"]);
				this.viewControlsMap.InitAndSetArrayItem(detailsObject.id, "dViewControlsMap", var_params["tName"], "id");
			}

			return null;
		}
		protected override XVar getColumnsToHide()
		{
			return this.getCombinedHiddenColumns();
		}
		public override XVar fieldClass(dynamic _param_f)
		{
			#region pass-by-value parameters
			dynamic f = XVar.Clone(_param_f);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(base.fieldClass((XVar)(f)));
			if((XVar)((XVar)(ret)  && (XVar)(this.printGridLayout == Constants.gltVERTICAL))  || (XVar)(this.printGridLayout == Constants.gltCOLUMNS))
			{
				ret = new XVar("");
			}
			return ret;
		}
		public override XVar getDataSourceFilterCriteria(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic filter = null, selectedRecords = XVar.Array();
			filter = XVar.Clone(base.getDataSourceFilterCriteria());
			selectedRecords = XVar.Clone(this.getSelectedRecords());
			if(!XVar.Equals(XVar.Pack(selectedRecords), XVar.Pack(null)))
			{
				dynamic keyFields = null, recConditions = XVar.Array();
				keyFields = XVar.Clone(this.pSet.getTableKeys());
				recConditions = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> keys in selectedRecords.GetEnumerator())
				{
					recConditions.InitAndSetArrayItem(DataCondition.FieldsEqual((XVar)(keyFields), (XVar)(keys.Value)), null);
				}
				filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, filter, 1, DataCondition._Or((XVar)(recConditions))))));
			}
			return filter;
		}
		public override XVar callBeforeQueryEvent(dynamic _param_dc)
		{
			#region pass-by-value parameters
			dynamic dc = XVar.Clone(_param_dc);
			#endregion

			dynamic order = null, prep = XVar.Array(), sql = null, where = null;
			if(XVar.Pack(!(XVar)(this.eventsObject.exists(new XVar("BeforeQueryPrint")))))
			{
				return null;
			}
			prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
			where = XVar.Clone(prep["where"]);
			order = XVar.Clone(prep["order"]);
			sql = XVar.Clone(prep["sql"]);
			this.eventsObject.BeforeQueryPrint((XVar)(sql), ref where, ref order, this);
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
				if(order != prep["order"])
				{
					this.dataSource.overrideOrder((XVar)(dc), (XVar)(order));
				}
			}

			return null;
		}
		public virtual XVar calculateRecordCount()
		{
			this.queryCommand = XVar.Clone(this.getSubsetDataCommand());
			this.callBeforeQueryEvent((XVar)(this.queryCommand));
			this.totalRowCount = XVar.Clone(this.dataSource.getCount((XVar)(this.queryCommand)));

			return null;
		}
		public override XVar getSubsetDataCommand(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(base.getSubsetDataCommand((XVar)(ignoreFilterField)));
			this.reoderCommandForReoderedRows((XVar)(this.getListPSet()), (XVar)(dc));
			if(XVar.Pack(!(XVar)(this.allPagesMode)))
			{
				dc.reccount = XVar.Clone(this.queryPageSize);
				dc.startRecord = XVar.Clone(this.queryPageSize * (this.queryPageNo - 1));
			}
			return dc;
		}
		public override XVar pdfJsonMode()
		{
			return this.mode == Constants.PRINT_PDFJSON;
		}
		public override XVar findRecordAssigns(dynamic _param_recordId)
		{
			#region pass-by-value parameters
			dynamic recordId = XVar.Clone(_param_recordId);
			#endregion

			return this.recordsRenderData[recordId];
		}
		public override XVar getSecurityCondition()
		{
			return Security.SelectCondition(new XVar("P"), (XVar)(this.pSet));
		}
		protected override XVar getTotalDataCommand()
		{
			return base.getSubsetDataCommand();
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
			settings.InitAndSetArrayItem(CommonFunctions.isRTL(), "isRTL");
			settings.InitAndSetArrayItem(this.pSet.isPrinterPagePDF(), "printerPagePDF");
			settings.InitAndSetArrayItem(this.pSet.getPrinterPageOrientation(), "printerPageOrientation");
			settings.InitAndSetArrayItem(this.pSet.getPrinterPageScale(), "printerPageScale");
			settings.InitAndSetArrayItem(this.pSet.isPrinterPageFitToPage(), "isPrinterPageFitToPage");
			settings.InitAndSetArrayItem(this.pSet.getPrinterSplitRecords(), "printerSplitRecords");
			if(XVar.Pack(this.printGridLayout))
			{
				settings.InitAndSetArrayItem(this.printGridLayout, "printGridLayout");
			}
			if(XVar.Pack(this.showHideFieldsFeatureEnabled()))
			{
				settings.InitAndSetArrayItem(true, "isAllowShowHideFields");
			}
			if((XVar)((XVar)(this.reorderFieldsFeatureEnabled())  && (XVar)(this.printGridLayout == Constants.gltHORIZONTAL))  && (XVar)(this.recsPerRowPrint == 1))
			{
				dynamic columnOrder = null, logger = null;
				settings.InitAndSetArrayItem(true, "isAllowFieldsReordering");
				logger = XVar.Clone(new paramsLogger((XVar)(this.tName), new XVar(Constants.FORDER_PARAMS_TYPE)));
				columnOrder = XVar.Clone(logger.getData());
				if(XVar.Pack(columnOrder))
				{
					settings.InitAndSetArrayItem(columnOrder, "columnOrder");
				}
			}
			return settings;
		}
	}
}
