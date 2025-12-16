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
	public partial class ProjectSettings : XClass
	{
		public dynamic _table;
		public dynamic _pageMode;
		public dynamic _pageType;
		public dynamic _page;
		public dynamic _viewPage = XVar.Pack(Constants.PAGE_VIEW);
		public dynamic _defaultViewPage = XVar.Pack(Constants.PAGE_VIEW);
		public dynamic _editPage = XVar.Pack(Constants.PAGE_EDIT);
		public dynamic _defaultEditPage = XVar.Pack(Constants.PAGE_EDIT);
		public dynamic _tableData = XVar.Array();
		public dynamic _auxTable = XVar.Pack("");
		public dynamic _auxTableData = XVar.Array();
		public dynamic _pageOptions = XVar.Array();
		public dynamic _dashboardElemPSet = XVar.Array();
		public ProjectSettings(dynamic _param_table = null, dynamic _param_pageType = null, dynamic _param_page = null, dynamic _param_pageTable = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			if(_param_pageType as Object == null) _param_pageType = new XVar("");
			if(_param_page as Object == null) _param_page = new XVar("");
			if(_param_pageTable as Object == null) _param_pageTable = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic page = XVar.Clone(_param_page);
			dynamic pageTable = XVar.Clone(_param_pageTable);
			#endregion

			dynamic mobileSub = null;
			if(XVar.Pack(!(XVar)(table)))
			{
				table = new XVar(Constants.GLOBAL_PAGES);
			}
			if(XVar.Pack(!(XVar)(pageTable)))
			{
				pageTable = XVar.Clone(table);
			}
			this.setTable((XVar)(table));
			if(table == pageTable)
			{
				this._auxTableData = this._tableData;
				this._auxTable = XVar.Clone(this._table);
			}
			else
			{
				this.loadAuxTable((XVar)(pageTable));
			}
			if(XVar.Pack(page))
			{
				this._pageType = XVar.Clone(this.getOriginalPageType((XVar)(page)));
			}
			if((XVar)(page)  && (XVar)(this.getPageIds().KeyExists(page)))
			{
				this.setPage((XVar)(page));
				this.setPageType((XVar)(this.getPageType()));
			}
			else
			{
				if(XVar.Pack(!(XVar)(pageType)))
				{
					pageType = XVar.Clone(this.getDefaultPageType());
				}
				if(XVar.Pack(pageType))
				{
					this._pageType = XVar.Clone(pageType);
					page = XVar.Clone(this.getDefaultPage((XVar)(pageType)));
					if(XVar.Pack(page))
					{
						this.setPage((XVar)(page));
					}
				}
			}
			if((XVar)(page)  && (XVar)(!(XVar)(pageType)))
			{
				pageType = XVar.Clone(this.getPageType());
			}
			if(XVar.Pack(pageType))
			{
				this.setPageType((XVar)(pageType));
			}
			mobileSub = XVar.Clone(this.getMobileSub());
			if((XVar)(GlobalVars.mediaType)  && (XVar)(mobileSub))
			{
				if(XVar.Pack(this.getPageIds().KeyExists(mobileSub)))
				{
					this.setPage((XVar)(mobileSub));
				}
			}
		}
		public virtual XVar table()
		{
			return this._table;
		}
		public virtual XVar getTableName()
		{
			return this.table();
		}
		public virtual XVar loadPageOptions(dynamic _param_option = null)
		{
			#region default values
			if(_param_option as Object == null) _param_option = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_option = XVar.Clone(_param_option);
			#endregion

			if(XVar.Pack(this._pageOptions))
			{
				return null;
			}
			MVCFunctions.importPageOptions((XVar)(this._auxTable), (XVar)(this._page));
			this._pageOptions = GlobalVars.page_options[this._auxTable][this._page];

			return null;
		}
		public virtual XVar setPage(dynamic _param_page)
		{
			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			#endregion

			if(page != this._page)
			{
				dynamic dummy = null;
				dummy = new XVar(null);
				this._pageOptions = dummy;
			}
			this._page = XVar.Clone(page);
			if(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(page), (XVar)(this.getPageIds()))), XVar.Pack(false)))
			{
				return null;
			}

			return null;
		}
		public virtual XVar setTable(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic pageType = null;
			this._table = XVar.Clone(table);
			if(CommonFunctions.GetTableURL((XVar)(table)) != "")
			{
				MVCFunctions.importTableSettings((XVar)(table));
			}
			if(XVar.Pack(GlobalVars.runnerTableSettings.KeyExists(this._table)))
			{
				this._tableData = GlobalVars.runnerTableSettings[this._table];
			}
			pageType = XVar.Clone(this.getDefaultPageType());
			this._editPage = XVar.Clone(this.getDefaultEditPageType((XVar)(pageType)));
			this._viewPage = XVar.Clone(this.getDefaultViewPageType((XVar)(pageType)));
			this._defaultEditPage = XVar.Clone(this._editPage);
			this._defaultViewPage = XVar.Clone(this._viewPage);

			return null;
		}
		public virtual XVar loadAuxTable(dynamic _param_auxTable)
		{
			#region pass-by-value parameters
			dynamic auxTable = XVar.Clone(_param_auxTable);
			#endregion

			this._auxTable = XVar.Clone(auxTable);
			if(CommonFunctions.GetTableURL((XVar)(auxTable)) != "")
			{
				MVCFunctions.importTableSettings((XVar)(auxTable));
			}
			if(XVar.Pack(GlobalVars.runnerTableSettings.KeyExists(this._auxTable)))
			{
				this._auxTableData = GlobalVars.runnerTableSettings[this._auxTable];
			}

			return null;
		}
		public virtual XVar pageName()
		{
			return this._page;
		}
		public virtual XVar pageType()
		{
			return this._pageType;
		}
		public virtual XVar pageTable()
		{
			return this._auxTable;
		}
		public virtual XVar getDefaultViewPageType(dynamic _param_tableType)
		{
			#region pass-by-value parameters
			dynamic tableType = XVar.Clone(_param_tableType);
			#endregion

			if((XVar)(tableType == Constants.PAGE_CHART)  || (XVar)(tableType == Constants.PAGE_REPORT))
			{
				return tableType;
			}
			return Constants.PAGE_VIEW;
		}
		public virtual XVar getDefaultEditPageType(dynamic _param_tableType)
		{
			#region pass-by-value parameters
			dynamic tableType = XVar.Clone(_param_tableType);
			#endregion

			if((XVar)(tableType == Constants.PAGE_CHART)  || (XVar)(tableType == Constants.PAGE_REPORT))
			{
				return Constants.PAGE_SEARCH;
			}
			return Constants.PAGE_EDIT;
		}
		public virtual XVar setPageType(dynamic _param_page)
		{
			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			#endregion

			if(XVar.Pack(this.isPageTypeForView((XVar)(page))))
			{
				dynamic tableType = null;
				tableType = XVar.Clone(this.getDefaultPageType());
				if((XVar)((XVar)(tableType != "report")  && (XVar)(tableType != "chart"))  && (XVar)((XVar)(page == Constants.PAGE_CHART)  || (XVar)(page == Constants.PAGE_REPORT)))
				{
					this._viewPage = new XVar(Constants.PAGE_LIST);
				}
				else
				{
					this._viewPage = XVar.Clone(page);
				}
				this._defaultViewPage = XVar.Clone(this.getDefaultViewPageType((XVar)(tableType)));
			}
			if(XVar.Pack(this.isPageTypeForEdit((XVar)(page))))
			{
				this._editPage = XVar.Clone(page);
				this._defaultEditPage = XVar.Clone(this.getDefaultEditPageType((XVar)(this.getDefaultPageType())));
			}

			return null;
		}
		public virtual XVar getDefaultPages()
		{
			this.updatePages();
			return this.getAuxTableValue(new XVar("defaultPages"));
		}
		public virtual XVar getDefaultPage(dynamic _param_type, dynamic _param_pageTable = null)
		{
			#region default values
			if(_param_pageTable as Object == null) _param_pageTable = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic var_type = XVar.Clone(_param_type);
			dynamic pageTable = XVar.Clone(_param_pageTable);
			#endregion

			dynamic defPage = null, defPages = XVar.Array();
			this.updatePages();
			defPages = this.getAuxTableValue(new XVar("defaultPages"));
			defPage = XVar.Clone(defPages[var_type]);
			if(XVar.Pack(defPage))
			{
				return defPage;
			}
			return null;
		}
		public virtual XVar getPageIds()
		{
			dynamic ret = null;
			this.updatePages();
			ret = XVar.Clone(this.getAuxTableValue(new XVar("pageTypes")));
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(ret)))))
			{
				return XVar.Array();
			}
			return ret;
		}
		public virtual XVar getEditPageType()
		{
			return this._editPage;
		}
		public virtual XVar isPageTypeForView(dynamic _param_ptype)
		{
			#region pass-by-value parameters
			dynamic ptype = XVar.Clone(_param_ptype);
			#endregion

			return MVCFunctions.in_array((XVar)(MVCFunctions.strtolower((XVar)(ptype))), (XVar)(GlobalVars.pageTypesForView));
		}
		public virtual XVar isPageTypeForEdit(dynamic _param_ptype)
		{
			#region pass-by-value parameters
			dynamic ptype = XVar.Clone(_param_ptype);
			#endregion

			return MVCFunctions.in_array((XVar)(MVCFunctions.strtolower((XVar)(ptype))), (XVar)(GlobalVars.pageTypesForEdit));
		}
		public virtual XVar getPageTypeByFieldEditFormat(dynamic _param_field, dynamic _param_editFormat)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic editFormat = XVar.Clone(_param_editFormat);
			#endregion

			dynamic editFormats = XVar.Array();
			editFormats = XVar.Clone(this.getTableValue(new XVar("fields"), (XVar)(field), new XVar("editFormats")));
			foreach (KeyValuePair<XVar, dynamic> ef in editFormats.GetEnumerator())
			{
				if(ef.Value["format"] == editFormat)
				{
					return ef.Key;
				}
			}
			return "";
		}
		public virtual XVar getPageOption(dynamic _param_key1, dynamic _param_key2 = null, dynamic _param_key3 = null, dynamic _param_key4 = null, dynamic _param_key5 = null)
		{
			#region default values
			if(_param_key2 as Object == null) _param_key2 = new XVar(false);
			if(_param_key3 as Object == null) _param_key3 = new XVar(false);
			if(_param_key4 as Object == null) _param_key4 = new XVar(false);
			if(_param_key5 as Object == null) _param_key5 = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic key1 = XVar.Clone(_param_key1);
			dynamic key2 = XVar.Clone(_param_key2);
			dynamic key3 = XVar.Clone(_param_key3);
			dynamic key4 = XVar.Clone(_param_key4);
			dynamic key5 = XVar.Clone(_param_key5);
			#endregion

			dynamic keys = XVar.Array(), opt = XVar.Array();
			this.loadPageOptions((XVar)(MVCFunctions.Concat(key1, key2)));
			if(XVar.Pack(!(XVar)(this._pageOptions.KeyExists(key1))))
			{
				return false;
			}
			keys = XVar.Clone(new XVar(0, key1));
			if(!XVar.Equals(XVar.Pack(key2), XVar.Pack(false)))
			{
				keys.InitAndSetArrayItem(key2, null);
			}
			if(!XVar.Equals(XVar.Pack(key3), XVar.Pack(false)))
			{
				keys.InitAndSetArrayItem(key3, null);
			}
			if(!XVar.Equals(XVar.Pack(key4), XVar.Pack(false)))
			{
				keys.InitAndSetArrayItem(key4, null);
			}
			if(!XVar.Equals(XVar.Pack(key5), XVar.Pack(false)))
			{
				keys.InitAndSetArrayItem(key5, null);
			}
			opt = this._pageOptions;
			foreach (KeyValuePair<XVar, dynamic> k in keys.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(opt)))))
				{
					return false;
				}
				if(XVar.Pack(!(XVar)(opt.KeyExists(k.Value))))
				{
					return false;
				}
				opt = opt[k.Value];
			}
			return opt;
		}
		public virtual XVar getPageOptionAsArray(dynamic _param_key1, dynamic _param_key2 = null, dynamic _param_key3 = null, dynamic _param_key4 = null, dynamic _param_key5 = null)
		{
			#region default values
			if(_param_key2 as Object == null) _param_key2 = new XVar(false);
			if(_param_key3 as Object == null) _param_key3 = new XVar(false);
			if(_param_key4 as Object == null) _param_key4 = new XVar(false);
			if(_param_key5 as Object == null) _param_key5 = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic key1 = XVar.Clone(_param_key1);
			dynamic key2 = XVar.Clone(_param_key2);
			dynamic key3 = XVar.Clone(_param_key3);
			dynamic key4 = XVar.Clone(_param_key4);
			dynamic key5 = XVar.Clone(_param_key5);
			#endregion

			dynamic ret = null;
			ret = this.getPageOption((XVar)(key1), (XVar)(key2), (XVar)(key3), (XVar)(key4), (XVar)(key5));
			if((XVar)(!(XVar)(ret))  || (XVar)(!(XVar)(MVCFunctions.is_array((XVar)(ret)))))
			{
				return XVar.Array();
			}
			return ret;
		}
		public virtual XVar getPageOptionArray(dynamic _param_keys)
		{
			#region pass-by-value parameters
			dynamic keys = XVar.Clone(_param_keys);
			#endregion

			dynamic opt = XVar.Array();
			this.loadPageOptions();
			opt = this._pageOptions;
			foreach (KeyValuePair<XVar, dynamic> k in keys.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(opt)))))
				{
					return false;
				}
				if(XVar.Pack(!(XVar)(opt.KeyExists(k.Value))))
				{
					return false;
				}
				opt = opt[k.Value];
			}
			return opt;
		}
		public virtual XVar getEffectiveViewFormat(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic effectiveView = null, formats = XVar.Array(), viewFormats = XVar.Array();
			viewFormats = this._tableData["fields"][field]["viewFormats"];
			if((XVar)(!(XVar)(MVCFunctions.is_array((XVar)(viewFormats))))  || (XVar)(!(XVar)(viewFormats)))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(this._tableData["fields"][field]["separateEditViewFormats"])))
			{
				if(XVar.Pack(viewFormats["view"]))
				{
					return "view";
				}
				if(this.getEntityType() == Constants.titREPORT)
				{
					return "report";
				}
			}
			effectiveView = XVar.Clone(this.getEffectiveViewPage((XVar)(field)));
			if(XVar.Pack(viewFormats.KeyExists(effectiveView)))
			{
				return effectiveView;
			}
			formats = XVar.Clone(MVCFunctions.array_keys((XVar)(viewFormats)));
			return formats[0];
		}
		public virtual XVar getEffectiveEditFormat(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic editFormats = null, effectiveEdit = null, formats = XVar.Array();
			editFormats = this._tableData["fields"][field]["editFormats"];
			if((XVar)(!(XVar)(MVCFunctions.is_array((XVar)(editFormats))))  || (XVar)(!(XVar)(editFormats)))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(this._tableData["fields"][field]["separateEditViewFormats"])))
			{
				if(XVar.Pack(editFormats.KeyExists("edit")))
				{
					return "edit";
				}
				formats = XVar.Clone(MVCFunctions.array_keys((XVar)(editFormats)));
				return formats[0];
			}
			effectiveEdit = XVar.Clone(this.getEffectiveEditPage((XVar)(field)));
			if(XVar.Pack(editFormats.KeyExists(effectiveEdit)))
			{
				return effectiveEdit;
			}
			formats = XVar.Clone(MVCFunctions.array_keys((XVar)(editFormats)));
			return formats[0];
		}
		public virtual XVar getEffectiveEditPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(this.isSeparate((XVar)(field))))
			{
				return this._editPage;
			}
			return this._defaultEditPage;
		}
		public virtual XVar getEffectiveViewPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(this.isSeparate((XVar)(field))))
			{
				if((XVar)(this._pageMode == Constants.EDIT_INLINE)  && (XVar)(this._viewPage != Constants.PAGE_VIEW))
				{
					return Constants.PAGE_LIST;
				}
				else
				{
					if((XVar)(this._pageMode == Constants.LIST_MASTER)  && (XVar)(this._viewPage == Constants.PAGE_LIST))
					{
						return Constants.PAGE_MASTER_INFO_LIST;
					}
					else
					{
						if((XVar)(this._pageMode == Constants.LIST_MASTER)  && (XVar)(this._viewPage == Constants.PAGE_REPORT))
						{
							return Constants.PAGE_MASTER_INFO_REPORT;
						}
						else
						{
							if((XVar)(this._pageMode == Constants.PRINT_MASTER)  && (XVar)(this._viewPage == Constants.PAGE_RPRINT))
							{
								return Constants.PAGE_MASTER_INFO_RPRINT;
							}
							else
							{
								if(this._pageMode == Constants.PRINT_MASTER)
								{
									return Constants.PAGE_MASTER_INFO_PRINT;
								}
							}
						}
					}
				}
				return this._viewPage;
			}
			return this._defaultViewPage;
		}
		public virtual XVar findField(dynamic _param_f)
		{
			#region pass-by-value parameters
			dynamic f = XVar.Clone(_param_f);
			#endregion

			dynamic fields = null;
			fields = XVar.Clone(this.getFieldsList());
			if(XVar.Pack(this._tableData["fields"][f]))
			{
				return f;
			}
			if(XVar.Pack(GlobalVars.runnerTableLabels[this._table]["fieldlabels"].KeyExists(f)))
			{
				return this.getFieldByGoodFieldName((XVar)(f));
			}
			f = XVar.Clone(MVCFunctions.strtoupper((XVar)(f)));
			foreach (KeyValuePair<XVar, dynamic> ff in this.getFieldsList().GetEnumerator())
			{
				if(MVCFunctions.strtoupper((XVar)(ff.Value)) == f)
				{
					return ff.Value;
				}
				if(MVCFunctions.strtoupper((XVar)(MVCFunctions.GoodFieldName((XVar)(ff.Value)))) == f)
				{
					return ff.Value;
				}
			}
			return "";
		}
		public virtual XVar addCustomExpressionIndex(dynamic _param_mainTable, dynamic _param_mainField, dynamic _param_index)
		{
			#region pass-by-value parameters
			dynamic mainTable = XVar.Clone(_param_mainTable);
			dynamic mainField = XVar.Clone(_param_mainField);
			dynamic index = XVar.Clone(_param_index);
			#endregion

			if(XVar.Pack(!(XVar)(this.Invoke("isExistsTableKey", new XVar(".customExpressionIndexes")))))
			{
				this._tableData.InitAndSetArrayItem(XVar.Array(), ".customExpressionIndexes");
			}
			if(XVar.Pack(!(XVar)(this._tableData[".customExpressionIndexes"].KeyExists(mainTable))))
			{
				this._tableData.InitAndSetArrayItem(XVar.Array(), ".customExpressionIndexes", mainTable);
			}
			this._tableData.InitAndSetArrayItem(index, ".customExpressionIndexes", mainTable, mainField);

			return null;
		}
		public virtual XVar getCustomExpressionIndex(dynamic _param_mainTable, dynamic _param_mainField)
		{
			#region pass-by-value parameters
			dynamic mainTable = XVar.Clone(_param_mainTable);
			dynamic mainField = XVar.Clone(_param_mainField);
			#endregion

			if(XVar.Pack(!(XVar)(this.Invoke("isExistsTableKey", new XVar(".customExpressionIndexes")))))
			{
				this._tableData.InitAndSetArrayItem(XVar.Array(), ".customExpressionIndexes");
			}
			if((XVar)(this._tableData[".customExpressionIndexes"].KeyExists(mainTable))  && (XVar)(this._tableData[".customExpressionIndexes"][mainTable].KeyExists(mainField)))
			{
				return this._tableData[".customExpressionIndexes"][mainTable][mainField];
			}
			return false;
		}
		public virtual XVar getDetailsTables()
		{
			return this.getTableValue(new XVar("detailsTables"));
		}
		public virtual XVar getAvailableDetailsTables()
		{
			if(XVar.Pack(!(XVar)(this._tableData.KeyExists("availableDetailsTables"))))
			{
				dynamic available = XVar.Array(), details = XVar.Array();
				available = XVar.Clone(XVar.Array());
				details = XVar.Clone(this.getTableValue(new XVar("detailsTables")));
				foreach (KeyValuePair<XVar, dynamic> d in details.GetEnumerator())
				{
					dynamic strPerm = null;
					strPerm = XVar.Clone(CommonFunctions.GetUserPermissions((XVar)(d.Value)));
					if(!XVar.Equals(XVar.Pack(strPerm), XVar.Pack("")))
					{
						available.InitAndSetArrayItem(d.Value, null);
					}
				}
				this._tableData.InitAndSetArrayItem(available, "availableDetailsTables");
			}
			return this._tableData["availableDetailsTables"];
		}
		public virtual XVar getDetailsKeys(dynamic _param_detailsTable)
		{
			#region pass-by-value parameters
			dynamic detailsTable = XVar.Clone(_param_detailsTable);
			#endregion

			dynamic detailsKeys = XVar.Array();
			if(XVar.Pack(!(XVar)(this._tableData.KeyExists("detailsKeys"))))
			{
				this._tableData.InitAndSetArrayItem(XVar.Array(), "detailsKeys");
			}
			detailsKeys = this._tableData["detailsKeys"];
			if(XVar.Pack(!(XVar)(detailsKeys.KeyExists(detailsTable))))
			{
				dynamic detailsPs = null;
				detailsPs = XVar.Clone(new ProjectSettings((XVar)(detailsTable)));
				detailsKeys.InitAndSetArrayItem(detailsPs.getMasterKeys((XVar)(this.table())), detailsTable);
			}
			return detailsKeys[detailsTable];
		}
		public virtual XVar getMasterTables()
		{
			return this.getTableValue(new XVar("masterTables"));
		}
		public virtual XVar getMasterKeys(dynamic _param_masterTable)
		{
			#region pass-by-value parameters
			dynamic masterTable = XVar.Clone(_param_masterTable);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> master in this.getMasterTables().GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(master.Value["table"]), XVar.Pack(masterTable)))
				{
					return new XVar("masterKeys", master.Value["masterKeys"], "detailsKeys", master.Value["detailsKeys"]);
				}
			}
			return new XVar("masterKeys", XVar.Array(), "detailsKeys", XVar.Array());
		}
		public virtual XVar verifyMasterTable(dynamic _param_masterTable)
		{
			#region pass-by-value parameters
			dynamic masterTable = XVar.Clone(_param_masterTable);
			#endregion

			if(XVar.Pack(!(XVar)(masterTable)))
			{
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> master in this.getMasterTables().GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(master.Value["table"]), XVar.Pack(masterTable)))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar GetFieldByIndex(dynamic _param_index)
		{
			#region pass-by-value parameters
			dynamic index = XVar.Clone(_param_index);
			#endregion

			dynamic fields = XVar.Array();
			fields = XVar.Clone(this.getFieldsList());
			foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
			{
				if(this.getFieldIndex((XVar)(f.Value)) == index)
				{
					return f.Value;
				}
			}
			return null;
		}
		public virtual XVar isSeparate(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("separateEditViewFormats"));
		}
		public virtual XVar label(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic result = null;
			result = XVar.Clone(CommonFunctions.GetFieldLabel((XVar)(MVCFunctions.GoodFieldName((XVar)(this._table))), (XVar)(MVCFunctions.GoodFieldName((XVar)(field)))));
			return (XVar.Pack(result != XVar.Pack("")) ? XVar.Pack(result) : XVar.Pack(field));
		}
		public virtual XVar getFilenameField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("fileFilenameField"));
		}
		public virtual XVar getLinkPrefix(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("linkPrefix"));
		}
		public virtual XVar getLinkType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("linkDisplay"));
		}
		public virtual XVar getLinkDisplayField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("linkDisplayField"));
		}
		public virtual XVar openLinkInNewWindow(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("linkNewWindow"));
		}
		public virtual XVar getLinkDisplayText(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("linkDisplayText"));
		}
		public virtual XVar getFieldType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("type"));
		}
		public virtual XVar getRestDateFormat(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("restDateFormat"));
		}
		public virtual XVar isAutoincField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("autoinc"));
		}
		public virtual XVar getCustomExpression(dynamic _param_field, dynamic _param_value, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic events = null, pageType = null;
			events = XVar.Clone(CommonFunctions.getEventObject(this));
			pageType = XVar.Clone(this.getEffectiveViewFormat((XVar)(field)));
			return events.customExpression((XVar)(field), (XVar)(pageType), (XVar)(value), (XVar)(data));
		}
		public virtual XVar getDefaultValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic events = null, pageType = null;
			events = XVar.Clone(CommonFunctions.getEventObject(this));
			pageType = XVar.Clone(this.getEffectiveEditFormat((XVar)(field)));
			if(XVar.Pack(!(XVar)(events.hasDefaultValue((XVar)(field), (XVar)(pageType)))))
			{
				return "";
			}
			return events.defaultValue((XVar)(field), (XVar)(pageType));
		}
		public virtual XVar getSearchDefaultValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(!(XVar)(this.isSeparate((XVar)(field)))))
			{
				return "";
			}
			return this.getDefaultValue((XVar)(field));
		}
		public virtual XVar isAutoUpdatable(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic events = null, pageType = null;
			events = XVar.Clone(CommonFunctions.getEventObject(this));
			pageType = XVar.Clone(this.getEffectiveEditFormat((XVar)(field)));
			return events.hasAutoUpdateValue((XVar)(field), (XVar)(pageType));
		}
		public virtual XVar getAutoUpdateValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic events = null, pageType = null;
			events = XVar.Clone(CommonFunctions.getEventObject(this));
			pageType = XVar.Clone(this.getEffectiveEditFormat((XVar)(field)));
			if(XVar.Pack(!(XVar)(events.hasAutoUpdateValue((XVar)(field), (XVar)(pageType)))))
			{
				return "";
			}
			return events.autoUpdateValue((XVar)(field), (XVar)(pageType));
		}
		public virtual XVar getEditFormat(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("format"));
		}
		public virtual XVar getViewFormat(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("format"));
		}
		public virtual XVar dateEditShowTime(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("dateShowTime"));
		}
		public virtual XVar lookupControlType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupControlType"));
		}
		public virtual XVar lookupListPageId(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupListPage"));
		}
		public virtual XVar lookupAddPageId(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupAddPage"));
		}
		public virtual XVar isDeleteAssociatedFile(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("deleteFile"));
		}
		public virtual XVar useCategory(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupDependent"));
		}
		public virtual XVar multiSelect(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupMultiselect"));
		}
		public virtual XVar getOAuthCloudFields()
		{
			dynamic fields = XVar.Array();
			if((XVar)((XVar)(!(XVar)(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudGDriveClientId"))))  && (XVar)(!(XVar)(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudOneDriveClientId")))))  && (XVar)(!(XVar)(ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudDropboxClientId")))))
			{
				return XVar.Array();
			}
			fields = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in this.getFieldsList().GetEnumerator())
			{
				dynamic stp = null;
				stp = XVar.Clone(this.fileStorageProvider((XVar)(field.Value)));
				if((XVar)((XVar)(stp == Constants.stpGOOGLEDRIVE)  || (XVar)(stp == Constants.stpDROPBOX))  || (XVar)(stp == Constants.stpONEDRIVE))
				{
					fields.InitAndSetArrayItem(field.Value, null);
				}
			}
			return fields;
		}
		public virtual XVar singleSelectLookupEdit(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic editFormats = XVar.Array(), hasLookup = null;
			hasLookup = new XVar(false);
			editFormats = XVar.Clone(this.getTableValue(new XVar("fields"), (XVar)(field), new XVar("editFormats")));
			foreach (KeyValuePair<XVar, dynamic> editFormat in editFormats.GetEnumerator())
			{
				if((XVar)(editFormat.Key != "edit")  && (XVar)(editFormat.Key != "add"))
				{
					continue;
				}
				if(editFormat.Value["format"] != Constants.EDIT_FORMAT_LOOKUP_WIZARD)
				{
					continue;
				}
				hasLookup = new XVar(true);
				if(XVar.Pack(editFormat.Value["lookupMultiselect"]))
				{
					return false;
				}
			}
			return hasLookup;
		}
		public virtual XVar getLookupMainTableSettings(dynamic _param_mainTableShortName, dynamic _param_mainField, dynamic _param_desiredPage = null)
		{
			#region default values
			if(_param_desiredPage as Object == null) _param_desiredPage = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic mainTableShortName = XVar.Clone(_param_mainTableShortName);
			dynamic mainField = XVar.Clone(_param_mainField);
			dynamic desiredPage = XVar.Clone(_param_desiredPage);
			#endregion

			dynamic editFormats = XVar.Array(), mainPSet = null;
			mainPSet = XVar.Clone(new ProjectSettings((XVar)(CommonFunctions.GetTableByShort((XVar)(mainTableShortName))), (XVar)(desiredPage)));
			editFormats = XVar.Clone(mainPSet.getTableValue(new XVar("fields"), (XVar)(mainField), new XVar("editFormats")));
			if(XVar.Pack(!(XVar)(editFormats)))
			{
				return null;
			}
			foreach (KeyValuePair<XVar, dynamic> ef in editFormats.GetEnumerator())
			{
				if((XVar)(ef.Value["format"] != Constants.EDIT_FORMAT_LOOKUP_WIZARD)  || (XVar)(ef.Value["lookupType"] == Constants.LT_LISTOFVALUES))
				{
					continue;
				}
				if(ef.Value["lookupTable"] == this.table())
				{
					return mainPSet;
				}
			}
			return null;
		}
		public virtual XVar multiSelectLookupEdit(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic editFormats = XVar.Array();
			editFormats = XVar.Clone(this.getTableValue(new XVar("fields"), (XVar)(field), new XVar("editFormats")));
			foreach (KeyValuePair<XVar, dynamic> editFormat in editFormats.GetEnumerator())
			{
				if((XVar)(editFormat.Key != "edit")  && (XVar)(editFormat.Key != "add"))
				{
					continue;
				}
				if(editFormat.Value["format"] != Constants.EDIT_FORMAT_LOOKUP_WIZARD)
				{
					continue;
				}
				if(XVar.Pack(editFormat.Value["lookupMultiselect"]))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar lookupField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic editFormats = XVar.Array();
			editFormats = XVar.Clone(this.getTableValue(new XVar("fields"), (XVar)(field), new XVar("editFormats")));
			foreach (KeyValuePair<XVar, dynamic> editFormat in editFormats.GetEnumerator())
			{
				if(editFormat.Value["format"] != Constants.EDIT_FORMAT_LOOKUP_WIZARD)
				{
					continue;
				}
				if(editFormat.Value["lookupLinkField"] != editFormat.Value["lookupDisplayField"])
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar selectSize(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupSize"));
		}
		public virtual XVar showThumbnail(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageShowThumbnail"));
		}
		public virtual XVar isImageURL(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageUrl"));
		}
		public virtual XVar showCustomExpr(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("fileShowCustom"));
		}
		public virtual XVar showFileSize(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("fileShowSize"));
		}
		public virtual XVar displayPDF(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("fileShowPdf"));
		}
		public virtual XVar showIcon(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("fileShowIcon"));
		}
		public virtual XVar getImageWidth(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageWidth"));
		}
		public virtual XVar getImageHeight(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageHeight"));
		}
		public virtual XVar getThumbnailWidth(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageThumbWidth"));
		}
		public virtual XVar getThumbnailHeight(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageThumbHeight"));
		}
		public virtual XVar getLookupType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic lookupType = null, projectTable = null;
			lookupType = XVar.Clone(this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupType")));
			if(lookupType == Constants.LT_LISTOFVALUES)
			{
				return lookupType;
			}
			projectTable = XVar.Clone(CommonFunctions.findTable((XVar)(this.getLookupTable((XVar)(field)))));
			return (XVar.Pack(projectTable) ? XVar.Pack(Constants.LT_QUERY) : XVar.Pack(Constants.LT_LOOKUPTABLE));
		}
		public virtual XVar getLookupTable(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupTable"));
		}
		public virtual XVar isLookupWhereCode(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupWhereCode"));
		}
		public virtual XVar isLookupWhereSet(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return 0 != MVCFunctions.strlen((XVar)(this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupWhere"))));
		}
		public virtual XVar getLookupWhere(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(this.getEntityType() == Constants.titDASHBOARD)
			{
				ProjectSettings pSet;
				pSet = XVar.UnPackProjectSettings(this.getDashSearchFieldSettings((XVar)(field)));
				if(XVar.Pack(!(XVar)(pSet)))
				{
					return "";
				}
				return pSet.getLookupWhere((XVar)(field));
			}
			if(XVar.Pack(this.isLookupWhereCode((XVar)(field))))
			{
				dynamic events = null, pageType = null;
				events = XVar.Clone(CommonFunctions.getEventObject(this));
				pageType = XVar.Clone(this.getEffectiveEditFormat((XVar)(field)));
				if(XVar.Pack(!(XVar)(events.hasLookupWhere((XVar)(field), (XVar)(pageType)))))
				{
					return "";
				}
				return events.lookupWhere((XVar)(field), (XVar)(pageType));
			}
			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupWhere"));
		}
		public virtual XVar getNotProjectLookupTableConnId(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupTableConnection"));
		}
		public virtual XVar getConnId()
		{
			return this.getTableValue(new XVar("connId"));
		}
		public virtual XVar getLinkField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupLinkField"));
		}
		public virtual XVar getDisplayField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupDisplayField"));
		}
		public virtual XVar getCustomDisplay(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupCustomDisplay"));
		}
		public virtual XVar NeedEncode(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic nonEncodeViewFormats = null;
			nonEncodeViewFormats = XVar.Clone(new XVar(0, Constants.FORMAT_CUSTOM, 1, Constants.FORMAT_HTML, 2, Constants.FORMAT_FILE, 3, Constants.FORMAT_FILE_IMAGE, 4, Constants.FORMAT_CHECKBOX, 5, Constants.FORMAT_EMAILHYPERLINK, 6, Constants.FORMAT_HYPERLINK));
			return XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(this.getViewFormat((XVar)(field))), (XVar)(nonEncodeViewFormats))), XVar.Pack(false));
		}
		public virtual XVar getValidationType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("validateAs"));
		}
		public virtual XVar getValidationData(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic editFormat = null, ret = XVar.Array(), validationType = null;
			ret = XVar.Clone(XVar.Array());
			ret.InitAndSetArrayItem(XVar.Array(), "basicValidate");
			ret.InitAndSetArrayItem(XVar.Array(), "customMessages");
			editFormat = XVar.Clone(this.getEditFormat((XVar)(field)));
			validationType = XVar.Clone(this.getValidationType((XVar)(field)));
			if((XVar)(validationType)  && (XVar)((XVar)((XVar)(editFormat == Constants.EDIT_FORMAT_TEXT_FIELD)  || (XVar)(editFormat == Constants.EDIT_FORMAT_TIME))  || (XVar)(editFormat == Constants.EDIT_FORMAT_PASSWORD)))
			{
				ret.InitAndSetArrayItem(CommonFunctions.getJsValidatorName((XVar)(validationType)), "basicValidate", null);
				if(XVar.Equals(XVar.Pack(validationType), XVar.Pack(Constants.VALIDATE_AS_REGEXP)))
				{
					ret.InitAndSetArrayItem(this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("validateRegex")), "regExp");
					ret.InitAndSetArrayItem(this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("validateRegexMessage")), "customMessages", "RegExp");
				}
			}
			if((XVar)(this.isRequired((XVar)(field)))  && (XVar)(editFormat != Constants.EDIT_FORMAT_READONLY))
			{
				ret.InitAndSetArrayItem("IsRequired", "basicValidate", null);
			}
			if(XVar.Pack(!(XVar)(this.allowDuplicateValues((XVar)(field)))))
			{
				ret.InitAndSetArrayItem("DenyDuplicated", "basicValidate", null);
				ret.InitAndSetArrayItem(this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("denyDuplicateMessage")), "customMessages", "DenyDuplicated");
			}
			return ret;
		}
		public virtual XVar getFieldItems(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getPageOption(new XVar("fields"), new XVar("fieldItems"), (XVar)(field));
		}
		public virtual XVar getGroupFields()
		{
			return this.getPageOption(new XVar("dataGrid"), new XVar("groupFields"));
		}
		public virtual XVar appearOnListPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(this.getPageOption(new XVar("fields"), new XVar("gridFields"))))), XVar.Pack(false)))
			{
				return true;
			}
			if(XVar.Pack(CommonFunctions.isReport((XVar)(this.getEntityType()))))
			{
				return !XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(this.getReportGroupFields()))), XVar.Pack(false));
			}
			return false;
		}
		public virtual XVar appearOnAddPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.appearOnPage((XVar)(field));
		}
		public virtual XVar appearOnInlineAdd(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic fields = null;
			fields = this.getInlineAddFields();
			if(XVar.Pack(!(XVar)(fields)))
			{
				return false;
			}
			return !XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(fields))), XVar.Pack(false));
		}
		public virtual XVar appearOnEditPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.appearOnPage((XVar)(field));
		}
		public virtual XVar appearOnInlineEdit(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic inlineFields = null;
			inlineFields = this.getInlineEditFields();
			if(XVar.Pack(!(XVar)(inlineFields)))
			{
				return false;
			}
			return !XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(inlineFields))), XVar.Pack(false));
		}
		public virtual XVar appearOnUpdateSelected(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic updateOnEditFields = null;
			updateOnEditFields = XVar.Clone(this.getPageOption(new XVar("fields"), new XVar("updateOnEditFields")));
			if(XVar.Pack(!(XVar)(updateOnEditFields)))
			{
				return false;
			}
			return !XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(this.getPageOption(new XVar("fields"), new XVar("updateOnEditFields"))))), XVar.Pack(false));
		}
		public virtual XVar appearOnPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic gridFields = null, ret = null;
			gridFields = this.getPageOption(new XVar("fields"), new XVar("gridFields"));
			if(XVar.Pack(!(XVar)(gridFields)))
			{
				ret = new XVar(false);
			}
			else
			{
				ret = XVar.Clone(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(gridFields))), XVar.Pack(false)));
			}
			if(XVar.Pack(!(XVar)(ret)))
			{
				if((XVar)(XVar.Equals(XVar.Pack(this.getPageType()), XVar.Pack("report")))  || (XVar)(XVar.Equals(XVar.Pack(this.getPageType()), XVar.Pack("rprint"))))
				{
					return !XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(this.getReportGroupFields()))), XVar.Pack(false));
				}
			}
			return ret;
		}
		public virtual XVar appearOnSearchPanel(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic fields = null;
			fields = this.getPageOption(new XVar("fields"), new XVar("searchPanelFields"));
			if(XVar.Pack(!(XVar)(fields)))
			{
				return false;
			}
			return !XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(fields))), XVar.Pack(false));
		}
		public virtual XVar appearAlwaysOnSearchPanel(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic fields = null;
			fields = this.getPageOption(new XVar("listSearch"), new XVar("alwaysOnPanelFields"));
			if(XVar.Pack(!(XVar)(fields)))
			{
				return false;
			}
			return !XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(field), (XVar)(fields))), XVar.Pack(false));
		}
		public virtual XVar getPageFields()
		{
			dynamic fields = null;
			fields = XVar.Clone(this.getPageOptionAsArray(new XVar("fields"), new XVar("gridFields")));
			if(XVar.Pack(CommonFunctions.isReport((XVar)(this.getEntityType()))))
			{
				return MVCFunctions.array_merge((XVar)(fields), (XVar)(this.getReportGroupFields()));
			}
			return fields;
		}
		public virtual XVar appearOnViewPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.appearOnPage((XVar)(field));
		}
		public virtual XVar appearOnPrinterPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.appearOnListPage((XVar)(field));
		}
		public virtual XVar isVideoUrlField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("videoFieldContainsFileURL"));
		}
		public virtual XVar isAbsolute(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("absolutePath"));
		}
		public virtual XVar getAudioTitleField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("videoTitleField"));
		}
		public virtual XVar getVideoWidth(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("videoWidth"));
		}
		public virtual XVar getVideoHeight(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("videoHeight"));
		}
		public virtual XVar isRewindEnabled(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("videoRewindEnabled"));
		}
		public virtual XVar getParentFieldsData(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic categoryFields = XVar.Array();
			if(XVar.Pack(!(XVar)(this.useCategory((XVar)(field)))))
			{
				return XVar.Array();
			}
			categoryFields = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> fields in this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupDependentFields")).GetEnumerator())
			{
				categoryFields.InitAndSetArrayItem(new XVar("main", fields.Value["masterField"], "lookup", fields.Value["lookupField"]), null);
			}
			return categoryFields;
		}
		public virtual XVar getLookupParentFNames(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic fNames = XVar.Array();
			fNames = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> data in this.getParentFieldsData((XVar)(field)).GetEnumerator())
			{
				fNames.InitAndSetArrayItem(data.Value["main"], null);
			}
			return fNames;
		}
		public virtual XVar isLookupUnique(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupUnique"));
		}
		public virtual XVar getLookupOrderBy(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupOrderBy"));
		}
		public virtual XVar isLookupDesc(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupOrderByDesc"));
		}
		public virtual XVar getOwnerTable(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("tableName"));
		}
		public virtual XVar isFieldEncrypted(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("encrypted"));
		}
		public virtual XVar isAllowToAdd(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupAllowAdd"));
		}
		public virtual XVar isAllowToEdit(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupAllowEdit"));
		}
		public static XVar encryptSettings(dynamic _param_connId)
		{
			#region pass-by-value parameters
			dynamic connId = XVar.Clone(_param_connId);
			#endregion

			dynamic ret = null;
			ret = XVar.Clone(ProjectSettings.getProjectValue(new XVar("connEncryptInfo"), (XVar)(connId)));
			if(XVar.Pack(!(XVar)(ret)))
			{
				ret = XVar.Clone(new XVar("encryptMethod", 0));
			}
			return ret;
		}
		public virtual XVar getAutoCompleteFields(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupAutofillFields"));
		}
		public virtual XVar isAutoCompleteFieldsOnEdit(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupAutofillEdit"));
		}
		public virtual XVar isFreeInput(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return (XVar)((XVar)(this.lookupControlType((XVar)(field)) == Constants.LCT_AJAX)  && (XVar)(this.getDisplayField((XVar)(field)) == this.getLinkField((XVar)(field))))  && (XVar)(this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupFreeInput")));
		}
		public virtual XVar getMapData(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic mapData = XVar.Array(), zoom = null;
			if(this.getViewFormat((XVar)(field)) != Constants.FORMAT_MAP)
			{
				return XVar.Array();
			}
			mapData = XVar.Clone(new XVar("width", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("mapWidth")), "height", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("mapHeight")), "address", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("mapAddressField")), "lat", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("mapLatField")), "lng", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("mapLonField")), "desc", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("mapDescriptionField")), "mapIcon", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("mapMarkerIcon")), "isMapIconCustom", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("mapMarkerCodeExpression"))));
			zoom = XVar.Clone(this.getFieldValue((XVar)(field), new XVar("view"), new XVar("mapZoom")));
			if(XVar.Pack(zoom))
			{
				mapData.InitAndSetArrayItem(zoom, "zoom");
			}
			return mapData;
		}
		public virtual XVar getFormatTimeAttrs(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic hours = null;
			hours = XVar.Clone((XVar.Pack(this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("timeConvention")) == 0) ? XVar.Pack(12) : XVar.Pack(24)));
			return new XVar("useTimePicker", this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("timeUseTimepicker")), "hours", hours, "minutes", this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("timeMinutesStep")), "showSeconds", this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("timeShowSeconds")));
		}
		public virtual XVar getViewAsTimeFormatData(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return new XVar("showSeconds", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("timeShowSeconds")), "showDaysInTotals", false, "timeFormat", this.getFieldValue((XVar)(field), new XVar("view"), new XVar("timeFormat")));
		}
		public virtual XVar showDaysInTimeTotals(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic formatData = XVar.Array();
			formatData = XVar.Clone(this.getViewAsTimeFormatData((XVar)(field)));
			return (XVar.Pack(formatData) ? XVar.Pack(formatData["showDaysInTotals"]) : XVar.Pack(false));
		}
		public virtual XVar appearOnExportPage(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.appearOnPage((XVar)(field));
		}
		public virtual XVar getStrOriginalTableName()
		{
			return this.getTableValue(new XVar("originalTable"));
		}
		public virtual XVar getSearchableFields()
		{
			if(this.getEntityType() == Constants.titDASHBOARD)
			{
				return this.getPageOptionAsArray(new XVar("dashSearch"), new XVar("allSearchFields"));
			}
			return this.getTableValue(new XVar("searchSettings"), new XVar("searchableFields"));
		}
		public virtual XVar getAllSearchFields()
		{
			return (XVar.Pack(this.getEntityType() == Constants.titDASHBOARD) ? XVar.Pack(this.getPageOptionAsArray(new XVar("dashSearch"), new XVar("allSearchFields"))) : XVar.Pack(this.getPageOptionAsArray(new XVar("fields"), new XVar("searchPanelFields"))));
		}
		public virtual XVar getAdvSearchFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("gridFields"));
		}
		public virtual XVar getDefaultPageType()
		{
			return ProjectSettings.defaultPageType((XVar)(this.getEntityType()));
		}
		public static XVar defaultPageType(dynamic _param_entityType)
		{
			#region pass-by-value parameters
			dynamic entityType = XVar.Clone(_param_entityType);
			#endregion

			switch(((XVar)entityType).ToInt())
			{
				case Constants.titTABLE:
				case Constants.titVIEW:
				case Constants.titSQL:
				case Constants.titREST:
					return "list";
				case Constants.titDASHBOARD:
					return "dashboard";
				case Constants.titGLOBAL:
					return "menu";
				case Constants.titREPORT:
				case Constants.titSQL_REPORT:
				case Constants.titREST_REPORT:
					return "report";
				case Constants.titCHART:
				case Constants.titSQL_CHART:
				case Constants.titREST_CHART:
					return "chart";
			}
			return "";
		}
		public virtual XVar getShortTableName()
		{
			return CommonFunctions.GetTableURL((XVar)(this._table));
		}
		public virtual XVar isShowAddInPopup()
		{
			return this.getPageOption(new XVar("list"), new XVar("addInPopup"));
		}
		public virtual XVar isShowEditInPopup()
		{
			return this.getPageOption(new XVar("list"), new XVar("editInPopup"));
		}
		public virtual XVar isShowViewInPopup()
		{
			return this.getPageOption(new XVar("list"), new XVar("viewInPopup"));
		}
		public virtual XVar isResizeColumns()
		{
			return this.getTableValue(new XVar("resizeColumns"));
		}
		public virtual XVar isUseAjaxSuggest()
		{
			return this.getTableValue(new XVar("searchSettings"), new XVar("searchSuggest"));
		}
		public virtual XVar getAllPageFields()
		{
			return MVCFunctions.array_merge((XVar)(this.getPageFields()), (XVar)(this.getAllSearchFields()));
		}
		public virtual XVar getPanelSearchFields()
		{
			return this.getPageOptionAsArray(new XVar("listSearch"), new XVar("alwaysOnPanelFields"));
		}
		public virtual XVar getGoogleLikeFields()
		{
			if(this.getEntityType() == Constants.titDASHBOARD)
			{
				return this.getPageOptionAsArray(new XVar("dashSearch"), new XVar("googleLikeFields"));
			}
			return this.getTableValue(new XVar("searchSettings"), new XVar("googleLikeSearchFields"));
		}
		public virtual XVar getInlineEditFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("inlineEditFields"));
		}
		public virtual XVar getUpdateSelectedFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("updateOnEditFields"));
		}
		public virtual XVar getExportFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("exportFields"));
		}
		public virtual XVar getImportFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("gridFields"));
		}
		public virtual XVar getEditFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("gridFields"));
		}
		public virtual XVar getInlineAddFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("inlineAddFields"));
		}
		public virtual XVar getAddFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("gridFields"));
		}
		public virtual XVar getMasterListFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("gridFields"));
		}
		public virtual XVar getViewFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("gridFields"));
		}
		public virtual XVar getFieldFilterFields()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in this.getPageOptionAsArray(new XVar("fields"), new XVar("fieldFilterFields")).GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(CommonFunctions.IsBinaryType((XVar)(this.getFieldType((XVar)(f.Value)))))))
				{
					ret.InitAndSetArrayItem(f.Value, null);
				}
			}
			return ret;
		}
		public virtual XVar getPrinterFields()
		{
			return this.getPageFields();
		}
		public virtual XVar getListFields()
		{
			dynamic fields = null;
			fields = XVar.Clone(this.getPageOptionAsArray(new XVar("fields"), new XVar("gridFields")));
			if(XVar.Pack(CommonFunctions.isReport((XVar)(this.getEntityType()))))
			{
				return MVCFunctions.array_merge((XVar)(fields), (XVar)(this.getReportGroupFields()));
			}
			return fields;
		}
		public virtual XVar hasJsEvents()
		{
			return this.getAuxTableValue(new XVar("hasJsEvents"));
		}
		public virtual XVar hasButtonsAdded()
		{
			return this.getPageOption(new XVar("page"), new XVar("hasCustomButtons"));
		}
		public virtual XVar customButtons()
		{
			return this.getPageOptionAsArray(new XVar("page"), new XVar("customButtons"));
		}
		public virtual XVar clickHandlerSnippets()
		{
			return this.getPageOptionAsArray(new XVar("page"), new XVar("clickHandlerSnippets"));
		}
		public virtual XVar isUseFieldsMaps()
		{
			return this.getTableValue(new XVar("hasFieldMaps"));
		}
		public virtual XVar getOrderInfo()
		{
			return this.getTableValue(new XVar("orderInfo"));
		}
		public virtual XVar getOrderIndexes()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> o in this.getOrderInfo().GetEnumerator())
			{
				ret.InitAndSetArrayItem(new XVar(0, o.Value["index"], 1, o.Value["dir"], 2, o.Value["field"]), null);
			}
			return ret;
		}
		public virtual XVar getStrOrderBy()
		{
			return this.getTableValue(new XVar("strOrderBy"));
		}
		public virtual SQLQuery getSQLQuery()
		{
			return XVar.UnPackSQLQuery(ProjectSettings.getTableSQLQuery((XVar)(this.table())) ?? new XVar());
		}
		public static XVar getTableSQLQuery(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic tableData = XVar.Array();
			tableData = GlobalVars.runnerTableSettings[table];
			if(XVar.Pack(!(XVar)(tableData)))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(tableData["sqlQuery"])))
			{
				tableData.InitAndSetArrayItem(CommonFunctions.sqlFromJson((XVar)(tableData["query"])), "sqlQuery");
			}
			return tableData["sqlQuery"];
		}
		public virtual XVar getSQLQueryByField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(this.getEntityType() == Constants.titDASHBOARD)
			{
				ProjectSettings pSet;
				pSet = XVar.UnPackProjectSettings(this.getDashSearchFieldSettings((XVar)(field)));
				return pSet.getSQLQuery();
			}
			else
			{
				return this.getSQLQuery();
			}

			return null;
		}
		public virtual XVar getDashSearchFieldSettings(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic dashSearchFields = XVar.Array();
			dashSearchFields = XVar.Clone(this.getDashboardSearchFields());
			return new ProjectSettings((XVar)(dashSearchFields[field][0]["table"]), (XVar)(this._editPage));
		}
		public virtual XVar getCreateThumbnail(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileCreateThumbnail"));
		}
		public virtual XVar getStrThumbnail(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileThumbnailPrefix"));
		}
		public virtual XVar getThumbnailField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileThumbnailField"));
		}
		public virtual XVar getThumbnailSize(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileThumbnailSize"));
		}
		public virtual XVar getResizeOnUpload(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileResize"));
		}
		public virtual XVar isBasicUploadUsed(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("basicUpload"));
		}
		public virtual XVar getNewImageSize(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileResizeSize"));
		}
		public virtual XVar getAcceptFileTypes(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> ext in this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileTypes")).GetEnumerator())
			{
				ret.InitAndSetArrayItem(MVCFunctions.strtoupper((XVar)(ext.Value)), null);
			}
			return ret;
		}
		public virtual XVar getAcceptFileTypesHtml(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> ext in this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileTypes")).GetEnumerator())
			{
				ret.InitAndSetArrayItem(MVCFunctions.Concat(".", ext.Value), null);
			}
			return MVCFunctions.implode(new XVar(","), (XVar)(ret));
		}
		public virtual XVar getMaxFileSize(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileSizeLimit"));
		}
		public virtual XVar getMaxTotalFilesSize(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileTotalSizeLimit"));
		}
		public virtual XVar getMaxNumberOfFiles(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileMaxNumber"));
		}
		public virtual XVar getNRows(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("textareaHeight"));
		}
		public virtual XVar getOriginalTableName()
		{
			dynamic result = null;
			result = XVar.Clone(this.getTableValue(new XVar("originalTable")));
			return (XVar.Pack(result != XVar.Pack("")) ? XVar.Pack(result) : XVar.Pack(this._table));
		}
		public virtual XVar getTableKeys()
		{
			return this.getTableValue(new XVar("keyFields"));
		}
		public virtual XVar truncateLargeText(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("textShowFirst"));
		}
		public virtual XVar getNumberOfChars(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return (XVar.Pack(this.truncateLargeText((XVar)(field))) ? XVar.Pack(this.getFieldValue((XVar)(field), new XVar("view"), new XVar("textShowFirstN"))) : XVar.Pack(0));
		}
		public virtual XVar isSQLExpression(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return !(XVar)(!(XVar)(this.getFieldValue((XVar)(field), new XVar("sqlExpression"))));
		}
		public virtual XVar getFullFieldName(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("sqlExpression"));
		}
		public virtual XVar setFullFieldName(dynamic _param_field, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			this._tableData.InitAndSetArrayItem(value, "fields", field, "sqlExpression");

			return null;
		}
		public virtual XVar isRequired(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("required"));
		}
		public virtual XVar insertNull(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("textInsertNull"));
		}
		public virtual XVar isUseRTE(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("textareaRTE"));
		}
		public virtual XVar isUseTimestamp(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fileAddTimestamp"));
		}
		public virtual XVar getFieldIndex(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("index"));
		}
		public virtual XVar getEntityType()
		{
			return this.getTableValue(new XVar("type"));
		}
		public virtual XVar getAuxEntityType()
		{
			return this.getAuxTableValue(new XVar("type"));
		}
		protected virtual XVar getDefaultFieldValue(dynamic _param_path1, dynamic _param_path2 = null, dynamic _param_path3 = null, dynamic _param_path4 = null)
		{
			#region default values
			if(_param_path2 as Object == null) _param_path2 = new XVar();
			if(_param_path3 as Object == null) _param_path3 = new XVar();
			if(_param_path4 as Object == null) _param_path4 = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic path1 = XVar.Clone(_param_path1);
			dynamic path2 = XVar.Clone(_param_path2);
			dynamic path3 = XVar.Clone(_param_path3);
			dynamic path4 = XVar.Clone(_param_path4);
			#endregion

			dynamic path = XVar.Array();
			path = XVar.Clone(new XVar(0, "fields", 1, ""));
			if(XVar.Equals(XVar.Pack(path1), XVar.Pack("filter")))
			{
				path.InitAndSetArrayItem("filterFormat", null);
			}
			else
			{
				if(XVar.Equals(XVar.Pack(path1), XVar.Pack("view")))
				{
					path.InitAndSetArrayItem("filterFormats", null);
					path.InitAndSetArrayItem("", null);
				}
				else
				{
					if(XVar.Equals(XVar.Pack(path1), XVar.Pack("edit")))
					{
						path.InitAndSetArrayItem("editFormats", null);
						path.InitAndSetArrayItem("", null);
					}
					else
					{
						path.InitAndSetArrayItem(path1, null);
					}
				}
			}
			if(!XVar.Equals(XVar.Pack(path2), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path2, null);
			}
			if(!XVar.Equals(XVar.Pack(path3), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path3, null);
			}
			if(!XVar.Equals(XVar.Pack(path4), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path4, null);
			}
			return ProjectSettings._getTableDefault((XVar)(path));
		}
		public virtual XVar getDashFieldValue(dynamic _param_field, dynamic _param_path1, dynamic _param_path2 = null, dynamic _param_path3 = null, dynamic _param_path4 = null)
		{
			#region default values
			if(_param_path2 as Object == null) _param_path2 = new XVar();
			if(_param_path3 as Object == null) _param_path3 = new XVar();
			if(_param_path4 as Object == null) _param_path4 = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic path1 = XVar.Clone(_param_path1);
			dynamic path2 = XVar.Clone(_param_path2);
			dynamic path3 = XVar.Clone(_param_path3);
			dynamic path4 = XVar.Clone(_param_path4);
			#endregion

			dynamic dashSearchFields = XVar.Array(), dfield = XVar.Array(), table = null;
			dashSearchFields = XVar.Clone(this.getDashboardSearchFields());
			dfield = XVar.Clone(dashSearchFields[field]);
			if(XVar.Pack(dfield))
			{
				table = XVar.Clone(dfield[0]["table"]);
			}
			if((XVar)(!(XVar)(dfield))  || (XVar)(!(XVar)(table)))
			{
				return this.getDefaultFieldValue((XVar)(path1), (XVar)(path2), (XVar)(path3), (XVar)(path4));
			}
			if(XVar.Pack(!(XVar)(this._dashboardElemPSet[table])))
			{
				this._dashboardElemPSet.InitAndSetArrayItem(new ProjectSettings((XVar)(table), (XVar)(this._editPage)), table);
			}
			return this._dashboardElemPSet[table].getFieldValue((XVar)(dfield[0]["field"]), (XVar)(path1), (XVar)(path2), (XVar)(path3), (XVar)(path4));
		}
		public static XVar getFieldObj(dynamic _param_table, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return GlobalVars.runnerTableSettings[table]["fields"][field];
		}
		public static XVar getFieldEditFormat(dynamic _param_table, dynamic _param_field, dynamic _param_pageType = null)
		{
			#region default values
			if(_param_pageType as Object == null) _param_pageType = new XVar("edit");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic fieldObj = XVar.Array();
			fieldObj = ProjectSettings.getFieldObj((XVar)(table), (XVar)(field));
			if(XVar.Pack(!(XVar)(fieldObj["editFormats"].KeyExists(pageType))))
			{
				dynamic pageTypes = XVar.Array();
				pageTypes = XVar.Clone(MVCFunctions.array_keys((XVar)(fieldObj["editFormats"])));
				pageType = XVar.Clone(pageTypes[0]);
			}
			return fieldObj["editFormats"][pageType];
		}
		public static XVar getFieldViewFormat(dynamic _param_table, dynamic _param_field, dynamic _param_pageType = null)
		{
			#region default values
			if(_param_pageType as Object == null) _param_pageType = new XVar("view");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic fieldObj = XVar.Array();
			fieldObj = ProjectSettings.getFieldObj((XVar)(table), (XVar)(field));
			if(XVar.Pack(!(XVar)(fieldObj["viewFormats"].KeyExists(pageType))))
			{
				dynamic pageTypes = XVar.Array();
				pageTypes = XVar.Clone(MVCFunctions.array_keys((XVar)(fieldObj["viewFormats"])));
				pageType = XVar.Clone(pageTypes[0]);
			}
			return fieldObj["viewFormats"][pageType];
		}
		public virtual XVar getFieldValue(dynamic _param_field, dynamic _param_path1, dynamic _param_path2 = null, dynamic _param_path3 = null, dynamic _param_path4 = null)
		{
			#region default values
			if(_param_path2 as Object == null) _param_path2 = new XVar();
			if(_param_path3 as Object == null) _param_path3 = new XVar();
			if(_param_path4 as Object == null) _param_path4 = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic path1 = XVar.Clone(_param_path1);
			dynamic path2 = XVar.Clone(_param_path2);
			dynamic path3 = XVar.Clone(_param_path3);
			dynamic path4 = XVar.Clone(_param_path4);
			#endregion

			dynamic path = XVar.Array(), value = null;
			if(this.getEntityType() == Constants.titDASHBOARD)
			{
				return this.getDashFieldValue((XVar)(field), (XVar)(path1), (XVar)(path2), (XVar)(path3), (XVar)(path4));
			}
			if((XVar)(!(XVar)(this._tableData["fields"]))  || (XVar)(!(XVar)(this._tableData["fields"].KeyExists(field))))
			{
				return this.getDefaultFieldValue((XVar)(path1), (XVar)(path2), (XVar)(path3), (XVar)(path4));
			}
			if(XVar.Equals(XVar.Pack(path1), XVar.Pack("filter")))
			{
				path1 = new XVar("filterFormat");
			}
			path = XVar.Clone(new XVar(0, "fields", 1, field));
			if(path1 == "view")
			{
				path.InitAndSetArrayItem("viewFormats", null);
				path.InitAndSetArrayItem(this.getEffectiveViewFormat((XVar)(field)), null);
			}
			else
			{
				if(path1 == "edit")
				{
					path.InitAndSetArrayItem("editFormats", null);
					path.InitAndSetArrayItem(this.getEffectiveEditFormat((XVar)(field)), null);
				}
				else
				{
					path.InitAndSetArrayItem(path1, null);
				}
			}
			if(!XVar.Equals(XVar.Pack(path2), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path2, null);
			}
			if(!XVar.Equals(XVar.Pack(path3), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path3, null);
			}
			if(!XVar.Equals(XVar.Pack(path4), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path4, null);
			}
			value = XVar.Clone(ProjectSettings._getSettingsValue((XVar)(this._tableData), (XVar)(path)));
			if(XVar.Equals(XVar.Pack(value), XVar.Pack(null)))
			{
				return ProjectSettings._getTableDefault((XVar)(path));
			}
			return value;
		}
		public virtual XVar getCalendarValue(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return this.getTableValue(new XVar("calendarSettings"), (XVar)(name));
		}
		public virtual XVar getGanttValue(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return this.getTableValue(new XVar("ganttSettings"), (XVar)(name));
		}
		public virtual XVar getTableValue(dynamic _param_path1, dynamic _param_path2 = null, dynamic _param_path3 = null, dynamic _param_path4 = null)
		{
			#region default values
			if(_param_path2 as Object == null) _param_path2 = new XVar();
			if(_param_path3 as Object == null) _param_path3 = new XVar();
			if(_param_path4 as Object == null) _param_path4 = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic path1 = XVar.Clone(_param_path1);
			dynamic path2 = XVar.Clone(_param_path2);
			dynamic path3 = XVar.Clone(_param_path3);
			dynamic path4 = XVar.Clone(_param_path4);
			#endregion

			dynamic path = XVar.Array(), value = null;
			path = XVar.Clone(new XVar(0, path1));
			if(!XVar.Equals(XVar.Pack(path2), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path2, null);
			}
			if(!XVar.Equals(XVar.Pack(path3), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path3, null);
			}
			if(!XVar.Equals(XVar.Pack(path4), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path4, null);
			}
			value = XVar.Clone(ProjectSettings._getSettingsValue((XVar)(this._tableData), (XVar)(path)));
			if(XVar.Equals(XVar.Pack(value), XVar.Pack(null)))
			{
				return ProjectSettings._getTableDefault((XVar)(path));
			}
			return value;
		}
		protected virtual XVar getAuxTableValue(dynamic _param_path1, dynamic _param_path2 = null, dynamic _param_path3 = null, dynamic _param_path4 = null)
		{
			#region default values
			if(_param_path2 as Object == null) _param_path2 = new XVar();
			if(_param_path3 as Object == null) _param_path3 = new XVar();
			if(_param_path4 as Object == null) _param_path4 = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic path1 = XVar.Clone(_param_path1);
			dynamic path2 = XVar.Clone(_param_path2);
			dynamic path3 = XVar.Clone(_param_path3);
			dynamic path4 = XVar.Clone(_param_path4);
			#endregion

			dynamic path = XVar.Array(), value = null;
			path = XVar.Clone(new XVar(0, path1));
			if(!XVar.Equals(XVar.Pack(path2), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path2, null);
			}
			if(!XVar.Equals(XVar.Pack(path3), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path3, null);
			}
			if(!XVar.Equals(XVar.Pack(path4), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path4, null);
			}
			value = XVar.Clone(ProjectSettings._getSettingsValue((XVar)(this._auxTableData), (XVar)(path)));
			if(XVar.Equals(XVar.Pack(value), XVar.Pack(null)))
			{
				return ProjectSettings._getTableDefault((XVar)(path));
			}
			return value;
		}
		public static XVar passwordValidationValue(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return ProjectSettings.getSecurityValue(new XVar("registration"), new XVar("passwordValidation"), (XVar)(name));
		}
		public static XVar staticPermissions()
		{
			return ProjectSettings.getSecurityValue(new XVar("staticPermissions"), new XVar("groups"));
		}
		public static XVar twoFactorValue(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return ProjectSettings.getSecurityValue(new XVar("twoFactorSettings"), (XVar)(name));
		}
		public static XVar captchaValue(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return ProjectSettings.getSecurityValue(new XVar("captchaSettings"), (XVar)(name));
		}
		public static XVar getSecurityValue(dynamic _param_path1, dynamic _param_path2 = null, dynamic _param_path3 = null, dynamic _param_path4 = null)
		{
			#region default values
			if(_param_path2 as Object == null) _param_path2 = new XVar();
			if(_param_path3 as Object == null) _param_path3 = new XVar();
			if(_param_path4 as Object == null) _param_path4 = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic path1 = XVar.Clone(_param_path1);
			dynamic path2 = XVar.Clone(_param_path2);
			dynamic path3 = XVar.Clone(_param_path3);
			dynamic path4 = XVar.Clone(_param_path4);
			#endregion

			dynamic path = XVar.Array(), value = null;
			path = XVar.Clone(new XVar(0, "security", 1, path1));
			if(!XVar.Equals(XVar.Pack(path2), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path2, null);
			}
			if(!XVar.Equals(XVar.Pack(path3), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path3, null);
			}
			if(!XVar.Equals(XVar.Pack(path4), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path4, null);
			}
			value = XVar.Clone(ProjectSettings._getSettingsValue((XVar)(GlobalVars.runnerProjectSettings), (XVar)(path)));
			if(XVar.Equals(XVar.Pack(value), XVar.Pack(null)))
			{
				return ProjectSettings._getDefaultSetting((XVar)(GlobalVars.runnerProjectDefaults), (XVar)(path));
			}
			return value;
		}
		public static XVar getProjectValue(dynamic _param_path1, dynamic _param_path2 = null, dynamic _param_path3 = null, dynamic _param_path4 = null)
		{
			#region default values
			if(_param_path2 as Object == null) _param_path2 = new XVar();
			if(_param_path3 as Object == null) _param_path3 = new XVar();
			if(_param_path4 as Object == null) _param_path4 = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic path1 = XVar.Clone(_param_path1);
			dynamic path2 = XVar.Clone(_param_path2);
			dynamic path3 = XVar.Clone(_param_path3);
			dynamic path4 = XVar.Clone(_param_path4);
			#endregion

			dynamic path = XVar.Array(), value = null;
			path = XVar.Clone(new XVar(0, path1));
			if(!XVar.Equals(XVar.Pack(path2), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path2, null);
			}
			if(!XVar.Equals(XVar.Pack(path3), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path3, null);
			}
			if(!XVar.Equals(XVar.Pack(path4), XVar.Pack(null)))
			{
				path.InitAndSetArrayItem(path4, null);
			}
			value = XVar.Clone(ProjectSettings._getSettingsValue((XVar)(GlobalVars.runnerProjectSettings), (XVar)(path)));
			if(XVar.Equals(XVar.Pack(value), XVar.Pack(null)))
			{
				return ProjectSettings._getDefaultSetting((XVar)(GlobalVars.runnerProjectDefaults), (XVar)(path));
			}
			return value;
		}
		public static XVar getProjectTables()
		{
			return GlobalVars.runnerProjectSettings["allTables"];
		}
		protected static XVar _getSettingsValue(dynamic root, dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic ptr = XVar.Array();
			ptr = root;
			foreach (KeyValuePair<XVar, dynamic> p in path.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(ptr)))))
				{
					return null;
				}
				if(XVar.Pack(!(XVar)(ptr.KeyExists(p.Value))))
				{
					return null;
				}
				ptr = ptr[p.Value];
			}
			return ptr;
		}
		protected static XVar _getTableDefault(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			return ProjectSettings._getDefaultSetting((XVar)(GlobalVars.runnerTableDefaults), (XVar)(path));
		}
		protected static XVar _getDefaultSetting(dynamic root, dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic ptr = XVar.Array();
			ptr = root;
			foreach (KeyValuePair<XVar, dynamic> p in path.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(ptr)))))
				{
					if(XVar.Pack(GlobalVars.strictSettings))
					{
						MVCFunctions.Echo("error in _getDefaultSetting");
						MVCFunctions.debugVar((XVar)(path));
						MVCFunctions.printStack();
						MVCFunctions.ob_flush();
						HttpContext.Current.Response.End();
						throw new RunnerInlineOutputException();
					}
					return null;
				}
				if(XVar.Pack(ptr.KeyExists("__meta__")))
				{
					dynamic metaType = null;
					metaType = XVar.Clone(ptr["__meta__"]);
					ptr = ptr["__object__"];
					if(XVar.Equals(XVar.Pack(metaType), XVar.Pack(Constants.metaObject)))
					{
						ptr = ptr[p.Value];
					}
				}
				else
				{
					ptr = ptr[p.Value];
				}
			}
			if((XVar)(MVCFunctions.is_array((XVar)(ptr)))  && (XVar)(ptr["__meta__"]))
			{
				return XVar.Array();
			}
			return ptr;
		}
		public virtual XVar getDateEditType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("dateEditType"));
		}
		public virtual XVar getHTML5InputType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("textHTML5Input"));
		}
		public virtual XVar getMaxLength(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("textboxMaxLenth"));
		}
		public virtual XVar getControlWidth(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("textboxSize"));
		}
		public virtual XVar checkFieldPermissions(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.appearOnPage((XVar)(field));
		}
		public virtual XVar getTableOwnerIdField()
		{
			return this.getTableValue(new XVar("tableOwnerIdField"));
		}
		public virtual XVar hasEvents()
		{
			return this.getTableValue(new XVar("hasEvents"));
		}
		public virtual XVar isHorizontalLookup(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupHorizontal"));
		}
		public virtual XVar isDecimalDigits(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("numberFractionalDigits"));
		}
		public virtual XVar getLookupValues(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("lookupValues"));
		}
		public virtual XVar hasEditPage()
		{
			return !(XVar)(!(XVar)(this.getDefaultPage(new XVar("edit"))));
		}
		public virtual XVar hasAddPage()
		{
			return !(XVar)(!(XVar)(this.getDefaultPage(new XVar("add"))));
		}
		public virtual XVar hasListPage()
		{
			return !(XVar)(!(XVar)(this.getDefaultPage(new XVar("list"))));
		}
		public virtual XVar hasImportPage()
		{
			return !(XVar)(!(XVar)(this.getDefaultPage(new XVar("import"))));
		}
		public virtual XVar hasInlineEdit()
		{
			return this.getPageOption(new XVar("list"), new XVar("inlineEdit"));
		}
		public virtual XVar hasUpdateSelected()
		{
			return this.getPageOption(new XVar("list"), new XVar("updateSelected"));
		}
		public virtual XVar updateSelectedButtons()
		{
			dynamic data = XVar.Array();
			data = XVar.Clone(this.labeledButtons());
			return data["update_records"];
		}
		public virtual XVar activatonMessages()
		{
			dynamic data = XVar.Array();
			data = XVar.Clone(this.labeledButtons());
			if(XVar.Pack(!(XVar)(MVCFunctions.is_array((XVar)(data["register_activate_message"])))))
			{
				return XVar.Array();
			}
			return data["register_activate_message"];
		}
		public virtual XVar labeledButtons()
		{
			return this.getPageOptionAsArray(new XVar("page"), new XVar("labeledButtons"));
		}
		public virtual XVar printPagesLabelsData()
		{
			dynamic data = XVar.Array();
			data = XVar.Clone(this.labeledButtons());
			return data["print_pages"];
		}
		public virtual XVar hasSortByDropdown()
		{
			return this.getPageOption(new XVar("list"), new XVar("sortDropdown"));
		}
		public virtual XVar getSortControlSettings()
		{
			return this.getTableValue(new XVar("sortByFields"), new XVar("sortOrder"));
		}
		public virtual XVar getClickActions()
		{
			return this.getTableValue(new XVar("clickActions"));
		}
		public virtual XVar hasCopyPage()
		{
			return true;
		}
		public virtual XVar hasViewPage()
		{
			return !(XVar)(!(XVar)(this.getDefaultPage(new XVar("view"))));
		}
		public virtual XVar hasExportPage()
		{
			return !(XVar)(!(XVar)(this.getDefaultPage(new XVar("export"))));
		}
		public virtual XVar hasPrintPage()
		{
			return (XVar)(!(XVar)(!(XVar)(this.getDefaultPage(new XVar("print")))))  || (XVar)(!(XVar)(!(XVar)(this.getDefaultPage(new XVar("rprint")))));
		}
		public virtual XVar hasDelete()
		{
			return this.getPageOption(new XVar("list"), new XVar("delete"));
		}
		public virtual XVar getTotalsFields()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> totals in this.getPageOptionAsArray(new XVar("totals")).GetEnumerator())
			{
				if((XVar)(totals.Value)  && (XVar)(totals.Value["totalsType"]))
				{
					ret.InitAndSetArrayItem(new XVar("fName", totals.Key, "numRows", 0, "totalsType", totals.Value["totalsType"], "viewFormat", this.getViewFormat((XVar)(totals.Key))), null);
				}
			}
			return ret;
		}
		public virtual XVar calcTotalsFor()
		{
			return this.getPageOption(new XVar("page"), new XVar("calcTotalsFor"));
		}
		public virtual XVar pageHasCharts()
		{
			return this.getPageOption(new XVar("page"), new XVar("hasCharts"));
		}
		public virtual XVar getExportTxtFormattingType()
		{
			return this.getPageOption(new XVar("export"), new XVar("format"));
		}
		public virtual XVar getExportDelimiter()
		{
			return this.getPageOption(new XVar("export"), new XVar("delimiter"));
		}
		public virtual XVar chekcExportDelimiterSelection()
		{
			return this.getPageOption(new XVar("export"), new XVar("selectDelimiter"));
		}
		public virtual XVar checkExportFieldsSelection()
		{
			return this.getPageOption(new XVar("export"), new XVar("selectFields"));
		}
		public virtual XVar exportFileTypes()
		{
			return this.getPageOption(new XVar("export"), new XVar("exportFileTypes"));
		}
		public virtual XVar getLoginFormType()
		{
			return this.getPageOption(new XVar("loginForm"), new XVar("loginForm"));
		}
		public virtual XVar getAdvancedSecurityType()
		{
			if(XVar.Pack(!(XVar)(Security.advancedSecurityAvailable())))
			{
				return Constants.ADVSECURITY_ALL;
			}
			return this.getTableValue(new XVar("advancedSecurityType"));
		}
		public virtual XVar displayLoading()
		{
			return this.getTableValue(new XVar("displayLoading"));
		}
		public virtual XVar getRecordsPerPageArray()
		{
			dynamic values = null;
			values = XVar.Clone(this.getTableValue(new XVar("pageSizeSelectorRecords")));
			return this.postProcessPerPageArr((XVar)((XVar.Pack(values) ? XVar.Pack(values) : XVar.Pack(new XVar(0, "10", 1, "20", 2, "30", 3, "50", 4, "100", 5, "500", 6, "all")))));
		}
		public virtual XVar getGroupsPerPageArray()
		{
			dynamic values = null;
			values = XVar.Clone(this.getTableValue(new XVar("pageSizeSelectorGroups")));
			return this.postProcessPerPageArr((XVar)((XVar.Pack(values) ? XVar.Pack(values) : XVar.Pack(new XVar(0, "1", 1, "3", 2, "5", 3, "10", 4, "50", 5, "100", 6, "all")))));
		}
		protected virtual XVar postProcessPerPageArr(dynamic _param_values)
		{
			#region pass-by-value parameters
			dynamic values = XVar.Clone(_param_values);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> v in values.GetEnumerator())
			{
				ret.InitAndSetArrayItem((XVar.Pack(v.Value == "all") ? XVar.Pack("-1") : XVar.Pack(v.Value)), null);
			}
			return ret;
		}
		public virtual XVar isReportWithGroups()
		{
			return !(XVar)(!(XVar)(this.getPageOption(new XVar("newreport"), new XVar("reportInfo"), new XVar("groupFields"))));
		}
		public virtual XVar isCrossTabReport()
		{
			return this.getPageOption(new XVar("newreport"), new XVar("reportInfo"), new XVar("crosstab"));
		}
		public virtual XVar getReportGroupFields()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> g in this.getPageOptionAsArray(new XVar("newreport"), new XVar("reportInfo"), new XVar("groupFields")).GetEnumerator())
			{
				ret.InitAndSetArrayItem(g.Value["field"], null);
			}
			return ret;
		}
		public virtual XVar getReportGroupFieldsData()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> g in this.getPageOptionAsArray(new XVar("newreport"), new XVar("reportInfo"), new XVar("groupFields")).GetEnumerator())
			{
				dynamic gdata = XVar.Array();
				gdata = XVar.Clone(XVar.Array());
				gdata.InitAndSetArrayItem(g.Value["field"], "strGroupField");
				gdata.InitAndSetArrayItem(g.Value["interval"], "groupInterval");
				gdata.InitAndSetArrayItem(g.Key + 1, "groupOrder");
				gdata.InitAndSetArrayItem(g.Value["summary"], "showGroupSummary");
				gdata.InitAndSetArrayItem(g.Value["axis"], "crossTabAxis");
				ret.InitAndSetArrayItem(gdata, null);
			}
			return ret;
		}
		public virtual XVar reportHasHorizontalSummary()
		{
			return this.getPageOption(new XVar("newreport"), new XVar("reportInfo"), new XVar("horizSummary"));
		}
		public virtual XVar reportHasVerticalSummary()
		{
			return this.getPageOption(new XVar("newreport"), new XVar("reportInfo"), new XVar("vertSummary"));
		}
		public virtual XVar reportHasPageSummary()
		{
			return this.getPageOption(new XVar("newreport"), new XVar("reportInfo"), new XVar("pageSummary"));
		}
		public virtual XVar reportHasGlobalSummary()
		{
			return this.getPageOption(new XVar("newreport"), new XVar("reportInfo"), new XVar("globalSummary"));
		}
		public virtual XVar getReportLayout()
		{
			dynamic rLayout = null;
			rLayout = XVar.Clone(this.getPageOption(new XVar("newreport"), new XVar("reportInfo"), new XVar("layout")));
			if(XVar.Equals(XVar.Pack(rLayout), XVar.Pack("stepped")))
			{
				return Constants.REPORT_STEPPED;
			}
			else
			{
				if(XVar.Equals(XVar.Pack(rLayout), XVar.Pack("align")))
				{
					return Constants.REPORT_ALIGN;
				}
				else
				{
					if(XVar.Equals(XVar.Pack(rLayout), XVar.Pack("outline")))
					{
						return Constants.REPORT_OUTLINE;
					}
					else
					{
						if(XVar.Equals(XVar.Pack(rLayout), XVar.Pack("block")))
						{
							return Constants.REPORT_BLOCK;
						}
						else
						{
							return Constants.REPORT_TABULAR;
						}
					}
				}
			}

			return null;
		}
		public virtual XVar isGroupSummaryCountShown()
		{
			foreach (KeyValuePair<XVar, dynamic> g in this.getPageOptionAsArray(new XVar("newreport"), new XVar("reportInfo"), new XVar("groupFields")).GetEnumerator())
			{
				if(XVar.Pack(g.Value["summary"]))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar reportDetailsShown()
		{
			return this.getPageOption(new XVar("newreport"), new XVar("reportInfo"), new XVar("showData"));
		}
		public virtual XVar reportTotalFieldsExist()
		{
			foreach (KeyValuePair<XVar, dynamic> f in this.getPageOptionAsArray(new XVar("newreport"), new XVar("reportInfo"), new XVar("fields")).GetEnumerator())
			{
				if((XVar)((XVar)((XVar)(f.Value["sum"])  || (XVar)(f.Value["min"]))  || (XVar)(f.Value["max"]))  || (XVar)(f.Value["avg"]))
				{
					return true;
				}
			}
			return false;
		}
		public virtual XVar reportFieldInfo(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> f in this.getPageOptionAsArray(new XVar("newreport"), new XVar("reportInfo"), new XVar("fields")).GetEnumerator())
			{
				if(XVar.Equals(XVar.Pack(f.Value["field"]), XVar.Pack(field)))
				{
					return f.Value;
				}
			}
			return null;
		}
		public virtual XVar noRecordsOnFirstPage()
		{
			return this.getTableValue(new XVar("searchSettings"), new XVar("hideDataUntilSearch"));
		}
		public virtual XVar isPrinterPageFitToPage()
		{
			return this.getTableValue(new XVar("printFitToPage"));
		}
		public virtual XVar getPrinterPageScale()
		{
			return this.getTableValue(new XVar("printScale"));
		}
		public virtual XVar getPrinterSplitRecords()
		{
			return this.getTableValue(new XVar("pageSizePrintRecords"));
		}
		public virtual XVar isPrinterPagePDF()
		{
			return this.getPageOption(new XVar("pdf"), new XVar("pdfView"));
		}
		public virtual XVar hasCaptcha()
		{
			return this.getPageOption(new XVar("captcha"), new XVar("captcha"));
		}
		public virtual XVar hasBreadcrumb()
		{
			return this.getPageOption(new XVar("misc"), new XVar("breadcrumb"));
		}
		public virtual XVar isSearchRequiredForFiltering()
		{
			return this.getTableValue(new XVar("searchSettings"), new XVar("hideFilterUntilSearch"));
		}
		public virtual XVar warnLeavingPages()
		{
			return this.getTableValue(new XVar("warnLeavingEdit"));
		}
		public virtual XVar hideEmptyViewFields()
		{
			return this.getTableValue(new XVar("hideEmptyFieldsOnView"));
		}
		public virtual XVar getInitialPageSize()
		{
			if(XVar.Pack(CommonFunctions.isReport((XVar)(this.getEntityType()))))
			{
				if(XVar.Pack(this.isReportWithGroups()))
				{
					return this.getTableValue(new XVar("pageSizeGroups"));
				}
			}
			return this.getTableValue(new XVar("pageSizeRecords"));
		}
		public virtual XVar getRecordsPerRowList()
		{
			return this.getPageOption(new XVar("page"), new XVar("recsPerRow"));
		}
		public virtual XVar getRecordsPerRowPrint()
		{
			return this.getPageOption(new XVar("page"), new XVar("recsPerRow"));
		}
		public virtual XVar getRecordsLimit()
		{
			return this.getTableValue(new XVar("limitRecords"));
		}
		public virtual XVar useMoveNext()
		{
			return this.getPageOption(new XVar("misc"), new XVar("nextPrev"));
		}
		public virtual XVar hasInlineAdd()
		{
			return this.getPageOption(new XVar("list"), new XVar("inlineAdd"));
		}
		public virtual XVar getListGridLayout()
		{
			return this.getPageOption(new XVar("page"), new XVar("gridType"));
		}
		public virtual XVar getPrintGridLayout()
		{
			return this.getPageOption(new XVar("page"), new XVar("gridType"));
		}
		public virtual XVar getPrinterPageOrientation()
		{
			return this.getTableValue(new XVar("pageOrientation"));
		}
		public virtual XVar getReportPrintGroupsPerPage()
		{
			if(XVar.Pack(this.isReportWithGroups()))
			{
				return this.getTableValue(new XVar("pageSizePrintGroups"));
			}
			else
			{
				return this.getTableValue(new XVar("pageSizePrintRecords"));
			}

			return null;
		}
		public virtual XVar ajaxBasedListPage()
		{
			return this.getTableValue(new XVar("listAjax"));
		}
		public virtual XVar isMultistepped()
		{
			return this.getPageOption(new XVar("page"), new XVar("multiStep"));
		}
		public virtual XVar hasVerticalBar()
		{
			return this.getPageOption(new XVar("page"), new XVar("verticalBar"));
		}
		public virtual XVar getGridTabs()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(this.getTableValue(new XVar("whereTabs")));
			if(XVar.Pack(!(XVar)(ret)))
			{
				return XVar.Array();
			}
			foreach (KeyValuePair<XVar, dynamic> tabIdx in MVCFunctions.array_keys((XVar)(ret)).GetEnumerator())
			{
				dynamic tab = XVar.Array();
				tab = ret[tabIdx.Value];
				if(XVar.Pack(!(XVar)(tab.KeyExists("title"))))
				{
					tab.InitAndSetArrayItem(ProjectSettings._getTableDefault((XVar)(new XVar(0, "whereTabs", 1, "", 2, "title"))), "title");
				}
			}
			return ret;
		}
		public virtual XVar highlightSearchResults()
		{
			return this.getTableValue(new XVar("searchSettings"), new XVar("highlightSearchResults"));
		}
		public virtual XVar getFieldsList()
		{
			if(XVar.Pack(!(XVar)(this._tableData["fields"])))
			{
				return XVar.Array();
			}
			return MVCFunctions.array_keys((XVar)(this._tableData["fields"]));
		}
		public virtual XVar getFieldCount()
		{
			return MVCFunctions.count(this.getFieldsList());
		}
		public virtual XVar getBinaryFieldsIndices()
		{
			dynamic fields = XVar.Array(), var_out = XVar.Array();
			fields = XVar.Clone(this.getFieldsList());
			var_out = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
			{
				if(XVar.Pack(CommonFunctions.IsBinaryType((XVar)(this.getFieldType((XVar)(f.Value))))))
				{
					var_out.InitAndSetArrayItem(f.Key + 1, null);
				}
			}
			return var_out;
		}
		public virtual XVar getNBFieldsList()
		{
			dynamic arr = XVar.Array(), t = XVar.Array();
			t = XVar.Clone(this.getFieldsList());
			arr = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in t.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(CommonFunctions.IsBinaryType((XVar)(this.getFieldType((XVar)(f.Value)))))))
				{
					arr.InitAndSetArrayItem(f.Value, null);
				}
			}
			return arr;
		}
		public virtual XVar getFieldByGoodFieldName(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> value in this._tableData["fields"].GetEnumerator())
			{
				if((XVar)(MVCFunctions.is_array((XVar)(value.Value)))  && (XVar)(1 < MVCFunctions.count(value.Value)))
				{
					if(value.Value["goodName"] == field)
					{
						return value.Key;
					}
				}
			}
			return "";
		}
		public virtual XVar getUploadFolder(dynamic _param_field, dynamic _param_fileData = null)
		{
			#region default values
			if(_param_fileData as Object == null) _param_fileData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic fileData = XVar.Clone(_param_fileData);
			#endregion

			dynamic eventObj = null, path = null;
			eventObj = XVar.Clone(CommonFunctions.getEventObject(this));
			if(XVar.Pack(eventObj.hasUploadFolder((XVar)(field))))
			{
				path = XVar.Clone(eventObj.uploadFolder((XVar)(field), (XVar)(fileData)));
			}
			else
			{
				path = XVar.Clone(this.getFieldValue((XVar)(field), new XVar("uploadFolder")));
			}
			if((XVar)(MVCFunctions.strlen((XVar)(path)))  && (XVar)(MVCFunctions.substr((XVar)(path), (XVar)(MVCFunctions.strlen((XVar)(path)) - 1)) != "/"))
			{
				path = MVCFunctions.Concat(path, "/");
			}
			return path;
		}
		public virtual XVar isMakeDirectoryNeeded(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return (XVar)(this.isUploadCodeExpression((XVar)(field)))  || (XVar)(!(XVar)(this.isAbsolute((XVar)(field))));
		}
		public virtual XVar getFinalUploadFolder(dynamic _param_field, dynamic _param_fileData = null)
		{
			#region default values
			if(_param_fileData as Object == null) _param_fileData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic fileData = XVar.Clone(_param_fileData);
			#endregion

			dynamic path = null;
			if(XVar.Pack(this.isAbsolute((XVar)(field))))
			{
				path = XVar.Clone(this.getUploadFolder((XVar)(field), (XVar)(fileData)));
			}
			else
			{
				path = XVar.Clone(MVCFunctions.getabspath((XVar)(this.getUploadFolder((XVar)(field), (XVar)(fileData)))));
			}
			if(ProjectSettings.ext() == "php")
			{
				if((XVar)(MVCFunctions.strlen((XVar)(path)))  && (XVar)(MVCFunctions.substr((XVar)(path), (XVar)(MVCFunctions.strlen((XVar)(path)) - 1)) != "/"))
				{
					path = MVCFunctions.Concat(path, "/");
				}
			}
			else
			{
				if((XVar)(MVCFunctions.strlen((XVar)(path)))  && (XVar)(MVCFunctions.substr((XVar)(path), (XVar)(MVCFunctions.strlen((XVar)(path)) - 1)) != "\\"))
				{
					path = MVCFunctions.Concat(path, "\\");
				}
			}
			return path;
		}
		public virtual XVar isUploadCodeExpression(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic eventObj = null;
			eventObj = XVar.Clone(CommonFunctions.getEventObject(this));
			return eventObj.hasUploadFolder((XVar)(field));
		}
		public virtual XVar getQueryObject()
		{
			dynamic queryObj = null;
			queryObj = XVar.Clone(this.getSQLQuery());
			return queryObj;
		}
		public virtual XVar getListOfFieldsByExprType(dynamic _param_needaggregate)
		{
			#region pass-by-value parameters
			dynamic needaggregate = XVar.Clone(_param_needaggregate);
			#endregion

			dynamic fields = XVar.Array(), query = null, var_out = XVar.Array();
			query = this.getSQLQuery();
			if(XVar.Pack(!(XVar)(query)))
			{
				return XVar.Array();
			}
			fields = XVar.Clone(this.getFieldsList());
			var_out = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> f in fields.GetEnumerator())
			{
				dynamic aggr = null, idx = null;
				idx = XVar.Clone(this.getFieldIndex((XVar)(f.Value)) - 1);
				aggr = XVar.Clone(query.IsAggrFuncField((XVar)(idx)));
				if((XVar)((XVar)(needaggregate)  && (XVar)(aggr))  || (XVar)((XVar)(!(XVar)(needaggregate))  && (XVar)(!(XVar)(aggr))))
				{
					var_out.InitAndSetArrayItem(f.Value, null);
				}
			}
			return var_out;
		}
		public virtual XVar isAggregateField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic idx = null, query = null;
			query = this.getSQLQuery();
			idx = XVar.Clone(this.getFieldIndex((XVar)(field)) - 1);
			return query.IsAggrFuncField((XVar)(idx));
		}
		public virtual XVar getNCSearch()
		{
			return !(XVar)(this.getTableValue(new XVar("searchSettings"), new XVar("caseSensitiveSearch")));
		}
		public virtual XVar getChartType()
		{
			return this.getTableValue(new XVar("chartSettings"), new XVar("shape"));
		}
		public virtual XVar getChartRefreshTime()
		{
			return (XVar.Pack(this.chartRefresh()) ? XVar.Pack(this.getTableValue(new XVar("chartSettings"), new XVar("chartRefreshTime"))) : XVar.Pack(0));
		}
		public virtual XVar chartRefresh()
		{
			return this.getTableValue(new XVar("chartSettings"), new XVar("chartRefresh"));
		}
		public virtual XVar getChartSettings()
		{
			return this.getTableValue(new XVar("chartSettings"));
		}
		public virtual XVar auditEnabled()
		{
			return this.getTableValue(new XVar("audit"));
		}
		public virtual XVar isSearchSavingEnabled()
		{
			return this.getPageOption(new XVar("listSearch"), new XVar("searchSaving"));
		}
		public virtual XVar isAllowShowHideFields()
		{
			if(XVar.Pack(this.getScrollGridBody()))
			{
				return false;
			}
			return this.getPageOption(new XVar("list"), new XVar("showHideFields"));
		}
		public virtual XVar isAllowFieldsReordering()
		{
			if((XVar)(this.getScrollGridBody())  || (XVar)(1 < this.getRecordsPerRowList()))
			{
				return false;
			}
			return this.getPageOption(new XVar("list"), new XVar("reorderFields"));
		}
		public virtual XVar lockingEnabled()
		{
			return this.getTableValue(new XVar("locking"));
		}
		public virtual XVar hasEncryptedFields()
		{
			return this.getTableValue(new XVar("hasEncryptedFields"));
		}
		public virtual XVar showSearchPanel()
		{
			return this.getPageOption(new XVar("listSearch"), new XVar("searchPanel"));
		}
		public virtual XVar isFlexibleSearch()
		{
			return !(XVar)(this.getPageOption(new XVar("listSearch"), new XVar("fixedSearchPanel")));
		}
		public virtual XVar getSearchRequiredFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("searchRequiredFields"));
		}
		public virtual XVar showSimpleSearchOptions()
		{
			return this.getPageOption(new XVar("listSearch"), new XVar("simpleSearchOptions"));
		}
		public virtual XVar getFieldsToHideIfEmpty()
		{
			return this.getPageOption(new XVar("fields"), new XVar("hideEmptyFields"));
		}
		public virtual XVar getFilterFields()
		{
			return this.getPageOptionAsArray(new XVar("fields"), new XVar("filterFields"));
		}
		public virtual XVar getFilterFieldFormat(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("format"));
		}
		public virtual XVar getFilterFieldTotal(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("filterTotals"));
		}
		public virtual XVar showWithNoRecords(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("showWithNoRecords"));
		}
		public virtual XVar getFilterSortValueType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("sortValueType"));
		}
		public virtual XVar isFilterSortOrderDescending(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("descendingOrder"));
		}
		public virtual XVar getNumberOfVisibleFilterItems(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("firstVisibleItems"));
		}
		public virtual XVar getFilterByInterval(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("filterBy"));
		}
		public virtual XVar getParentFilterName(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("parentFilter"));
		}
		public virtual XVar getFilterIntervals(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> original in this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("arrFilterIntervals")).GetEnumerator())
			{
				dynamic interval = XVar.Array();
				interval = XVar.Clone(original.Value);
				interval.InitAndSetArrayItem(original.Key + 1, "index");
				interval.InitAndSetArrayItem((XVar)(original.Value["lowerLimitType"] == Constants.FIL_REMAINDER)  && (XVar)(original.Value["upperLimitType"] == Constants.FIL_REMAINDER), "remainder");
				interval.InitAndSetArrayItem((XVar)(original.Value["lowerLimitType"] == Constants.FIL_NONE)  && (XVar)(original.Value["upperLimitType"] == Constants.FIL_NONE), "noLimits");
				ret.InitAndSetArrayItem(interval, null);
			}
			return ret;
		}
		public virtual XVar showCollapsed(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("hideControl"));
		}
		public virtual XVar getFilterIntervalDatabyIndex(dynamic _param_field, dynamic _param_idx)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic idx = XVar.Clone(_param_idx);
			#endregion

			dynamic filterIntervalsData = XVar.Array(), intervalData = null;
			intervalData = XVar.Clone(XVar.Array());
			filterIntervalsData = XVar.Clone(this.getFilterIntervals((XVar)(field)));
			if(XVar.Pack(filterIntervalsData[idx - 1]))
			{
				return filterIntervalsData[idx - 1];
			}
			if(XVar.Pack(filterIntervalsData[0]))
			{
				return filterIntervalsData[0];
			}
			return XVar.Array();
		}
		public virtual XVar getFilterTotalsField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("filterTotalsField"));
		}
		public virtual XVar getFilterFiledMultiSelect(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("filterMultiselect"));
		}
		public virtual XVar getBooleanFilterMessageData(dynamic _param_field, dynamic _param_checked)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic var_checked = XVar.Clone(_param_checked);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), (XVar)((XVar.Pack(var_checked) ? XVar.Pack("multilangCheckedMessage") : XVar.Pack("multilangUncheckedMessage"))));
		}
		public virtual XVar getFilterStepType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return (int)this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("sliderStepType"));
		}
		public virtual XVar getFilterStepValue(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return (double)this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("sliderStepValue"));
		}
		public virtual XVar getFilterKnobsType(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return (int)this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("sliderKnobs"));
		}
		public virtual XVar isFilterApplyBtnSet(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("filter"), new XVar("addApplyButton"));
		}
		public virtual XVar getStrField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("strField"));
		}
		public virtual XVar getSourceSingle(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("sourceSingle"));
		}
		public virtual XVar getFieldSource(dynamic _param_field, dynamic _param_listRequest)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic listRequest = XVar.Clone(_param_listRequest);
			#endregion

			return this.getFieldValue((XVar)(field), (XVar)((XVar.Pack(listRequest) ? XVar.Pack("strField") : XVar.Pack("sourceSingle"))));
		}
		public virtual XVar getScrollGridBody()
		{
			return this.getTableValue(new XVar("scrollGridBody"));
		}
		public virtual XVar isUpdateLatLng()
		{
			return this.getTableValue(new XVar("geoCoding"), new XVar("enabled"));
		}
		public virtual XVar getGeocodingData()
		{
			dynamic ret = XVar.Array();
			ret = XVar.Clone(this.getTableValue(new XVar("geoCoding")));
			ret.InitAndSetArrayItem(ret["lonField"], "lngField");
			return ret;
		}
		public virtual XVar allowDuplicateValues(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return !(XVar)(this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("denyDuplicate")));
		}
		public virtual XVar getSearchOptionsList(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("searchOptions"));
		}
		public virtual XVar getDefaultSearchOption(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic defaultOpt = null;
			defaultOpt = XVar.Clone(this.getFieldValue((XVar)(field), new XVar("defaultSearchOption")));
			if(XVar.Pack(!(XVar)(defaultOpt)))
			{
				dynamic searchOptionsList = XVar.Array();
				searchOptionsList = XVar.Clone(this.getSearchOptionsList((XVar)(field)));
				if(XVar.Pack(MVCFunctions.count(searchOptionsList)))
				{
					defaultOpt = XVar.Clone(searchOptionsList[0]);
				}
			}
			return defaultOpt;
		}
		public static XVar isMenuTreelike(dynamic _param_menuName)
		{
			#region pass-by-value parameters
			dynamic menuName = XVar.Clone(_param_menuName);
			#endregion

			return GlobalVars.runnerMenus[menuName]["treeLike"];
		}
		public virtual XVar setPageMode(dynamic _param_pageMode)
		{
			#region pass-by-value parameters
			dynamic pageMode = XVar.Clone(_param_pageMode);
			#endregion

			this._pageMode = XVar.Clone(pageMode);

			return null;
		}
		public virtual XVar editPageHasDenyDuplicatesFields()
		{
			foreach (KeyValuePair<XVar, dynamic> fieldName in this.getEditFields().GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.allowDuplicateValues((XVar)(fieldName.Value)))))
				{
					return true;
				}
			}
			return false;
		}
		public static XVar rteType()
		{
			return ProjectSettings.getProjectValue(new XVar("rteType"));
		}
		public static XVar richTextEnabled()
		{
			return ProjectSettings.getProjectValue(new XVar("richTextEnabled"));
		}
		public virtual XVar getHiddenFields(dynamic _param_device)
		{
			#region pass-by-value parameters
			dynamic device = XVar.Clone(_param_device);
			#endregion

			dynamic list = XVar.Array();
			list = XVar.Clone(this.getTableValue(new XVar("deviceHideFields")));
			if(XVar.Pack(list.KeyExists(device)))
			{
				return list[device];
			}
			return XVar.Array();
		}
		public virtual XVar getHiddenGoodNameFields(dynamic _param_device)
		{
			#region pass-by-value parameters
			dynamic device = XVar.Clone(_param_device);
			#endregion

			dynamic hFields = XVar.Array(), hGoodFields = XVar.Array();
			hGoodFields = XVar.Clone(XVar.Array());
			hFields = XVar.Clone(this.getHiddenFields((XVar)(device)));
			foreach (KeyValuePair<XVar, dynamic> field in hFields.GetEnumerator())
			{
				hGoodFields.InitAndSetArrayItem(MVCFunctions.GoodFieldName((XVar)(field.Value)), null);
			}
			return hGoodFields;
		}
		public virtual XVar columnsByDeviceEnabled()
		{
			dynamic list = XVar.Array();
			list = XVar.Clone(this.getTableValue(new XVar("deviceHideFields")));
			foreach (KeyValuePair<XVar, dynamic> v in list.GetEnumerator())
			{
				if(XVar.Pack(v.Value))
				{
					return true;
				}
			}
			return false;
		}
		public static XVar getForLogin()
		{
			return (XVar.Pack(!(XVar)(!(XVar)(Security.dbProvider()))) ? XVar.Pack(new ProjectSettings((XVar)(Security.loginTable()), new XVar(Constants.PAGE_LIST))) : XVar.Pack(null));
		}
		public virtual XVar getDashboardSearchFields()
		{
			return this.getPageOptionAsArray(new XVar("dashSearch"), new XVar("searchFields"));
		}
		public virtual XVar getDashboardElementData(dynamic _param_dashElementName)
		{
			#region pass-by-value parameters
			dynamic dashElementName = XVar.Clone(_param_dashElementName);
			#endregion

			dynamic dElements = XVar.Array();
			dElements = XVar.Clone(this.getDashboardElements());
			foreach (KeyValuePair<XVar, dynamic> dElemData in dElements.GetEnumerator())
			{
				if(dElemData.Value["elementName"] == dashElementName)
				{
					return dElemData.Value;
				}
			}
			return XVar.Array();
		}
		public virtual XVar getAfterAddAction()
		{
			return this.getTableValue(new XVar("afterAddAction"));
		}
		public virtual XVar getAADetailTable()
		{
			return this.getTableValue(new XVar("afterAddDetail"));
		}
		public virtual XVar checkClosePopupAfterAdd()
		{
			return this.getTableValue(new XVar("closePopupAfterAdd"));
		}
		public virtual XVar getAfterEditAction()
		{
			return this.getTableValue(new XVar("afterEditAction"));
		}
		public virtual XVar getAEDetailTable()
		{
			return this.getTableValue(new XVar("afterEditDetails"));
		}
		public virtual XVar checkClosePopupAfterEdit()
		{
			return this.getTableValue(new XVar("closePopupAfterEdit"));
		}
		public virtual XVar getMapIcon(dynamic _param_field, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			if(XVar.Pack(!(XVar)(this.isMapIconCustom((XVar)(field)))))
			{
				dynamic mapData = XVar.Array();
				mapData = XVar.Clone(this.getMapData((XVar)(field)));
				if(mapData["mapIcon"] != "")
				{
					return MVCFunctions.Concat("images/menuicons/", mapData["mapIcon"]);
				}
				return "";
			}
			else
			{
				dynamic eventObj = null, pageType = null;
				eventObj = XVar.Clone(CommonFunctions.getEventObject(this));
				pageType = XVar.Clone(this.getEffectiveViewFormat((XVar)(field)));
				return eventObj.mapMarker((XVar)(field), (XVar)(pageType), (XVar)(data));
			}

			return null;
		}
		public virtual XVar getFileText(dynamic _param_field, dynamic _param_fileData, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic fileData = XVar.Clone(_param_fileData);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic eventObj = null, pageType = null;
			eventObj = XVar.Clone(CommonFunctions.getEventObject(this));
			pageType = XVar.Clone(this.getEffectiveViewFormat((XVar)(field)));
			return eventObj.fileText((XVar)(field), (XVar)(pageType), (XVar)(fileData), (XVar)(data));
		}
		public virtual XVar getDashMapIcon(dynamic _param_dashElementName, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic dashElementName = XVar.Clone(_param_dashElementName);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			dynamic dashElementData = XVar.Array();
			dashElementData = XVar.Clone(this.getDashboardElementData((XVar)(dashElementName)));
			if(XVar.Pack(dashElementData["isMarkerIconCustom"]))
			{
				dynamic funcName = null;
				funcName = XVar.Clone(MVCFunctions.Concat("event_", dashElementData["iconSnippet"]));
				return GlobalVars.globalEvents.Invoke(funcName, (XVar)(data));
			}
			if(XVar.Pack(dashElementData["iconF"]))
			{
				return MVCFunctions.Concat("images/menuicons/", dashElementData["iconF"]);
			}
			return "";
		}
		public virtual XVar getDashMapLocationIcon(dynamic _param_dashElementName)
		{
			#region pass-by-value parameters
			dynamic dashElementName = XVar.Clone(_param_dashElementName);
			#endregion

			dynamic dashElementData = XVar.Array();
			dashElementData = XVar.Clone(this.getDashboardElementData((XVar)(dashElementName)));
			if(XVar.Pack(dashElementData["isLocationMarkerIconCustom"]))
			{
				dynamic funcName = null;
				funcName = XVar.Clone(MVCFunctions.Concat("event_", dashElementData["currentLocationIconSnippet"]));
				return GlobalVars.globalEvents.Invoke(funcName, (XVar)(XVar.Array()));
			}
			if(XVar.Pack(dashElementData["currentLocationIcon"]))
			{
				return MVCFunctions.Concat("images/menuicons/", dashElementData["currentLocationIcon"]);
			}
			return "";
		}
		public virtual XVar isMapIconCustom(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic mapData = XVar.Array();
			mapData = XVar.Clone(this.getMapData((XVar)(field)));
			return mapData["isMapIconCustom"];
		}
		public virtual XVar getDetailsBadgeColor(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return this.getPageOption(new XVar("details"), (XVar)(dTable), new XVar("badgeColor"));
		}
		public virtual XVar getPageMenus()
		{
			return this.getPageOptionAsArray(new XVar("page"), new XVar("menus"));
		}
		public virtual XVar getDefaultBadgeColor()
		{
			return this.getTableValue(new XVar("detailsBadgeColor"));
		}
		public virtual XVar helperFormItems()
		{
			return this.getPageOption(new XVar("layoutHelper"), new XVar("formItems"));
		}
		public virtual XVar helperItemsByType()
		{
			return this.getPageOption(new XVar("layoutHelper"), new XVar("itemsByType"));
		}
		public virtual XVar allFieldItems()
		{
			return this.getPageOption(new XVar("fields"), new XVar("fieldItems"));
		}
		public virtual XVar helperItemVisibility()
		{
			return this.getPageOption(new XVar("layoutHelper"), new XVar("itemVisibility"));
		}
		public virtual XVar helperCellMaps()
		{
			return this.getPageOption(new XVar("layoutHelper"), new XVar("cellMaps"));
		}
		public virtual XVar detailsShowCount(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return this.getPageOption(new XVar("details"), (XVar)(dTable), new XVar("showCount"));
		}
		public virtual XVar detailsHideEmpty(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return this.getPageOption(new XVar("details"), (XVar)(dTable), new XVar("hideEmptyChild"));
		}
		public virtual XVar detailsHideEmptyPreview(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return this.getPageOption(new XVar("details"), (XVar)(dTable), new XVar("hideEmptyPreview"));
		}
		public virtual XVar detailsPreview(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return this.getPageOption(new XVar("details"), (XVar)(dTable), new XVar("displayPreview"));
		}
		public virtual XVar detailsProceedLink(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return this.getPageOption(new XVar("details"), (XVar)(dTable), new XVar("showProceedLink"));
		}
		public virtual XVar detailsPrint(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return this.getPageOption(new XVar("details"), (XVar)(dTable), new XVar("printDetails"));
		}
		public virtual XVar detailsLinks()
		{
			return this.getPageOption(new XVar("allDetails"), new XVar("linkType"));
		}
		public virtual XVar detailsPageId(dynamic _param_dTable)
		{
			#region pass-by-value parameters
			dynamic dTable = XVar.Clone(_param_dTable);
			#endregion

			return this.getPageOption(new XVar("details"), (XVar)(dTable), new XVar("previewPageId"));
		}
		public virtual XVar masterPreview(dynamic _param_mTable)
		{
			#region pass-by-value parameters
			dynamic mTable = XVar.Clone(_param_mTable);
			#endregion

			return this.getPageOption(new XVar("master"), (XVar)(mTable), new XVar("preview"));
		}
		public virtual XVar hasMap()
		{
			return !(XVar)(!(XVar)(this.getPageOption(new XVar("events"), new XVar("maps"))));
		}
		public virtual XVar maps()
		{
			return this.getPageOption(new XVar("events"), new XVar("maps"));
		}
		public virtual XVar mapsData()
		{
			return this.getPageOption(new XVar("events"), new XVar("mapsData"));
		}
		public virtual XVar buttons()
		{
			return this.getPageOption(new XVar("events"), new XVar("buttons"));
		}
		public virtual XVar getPageType(dynamic _param_page = null)
		{
			#region default values
			if(_param_page as Object == null) _param_page = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			#endregion

			if(XVar.Pack(!(XVar)(page)))
			{
				page = XVar.Clone(this._page);
			}
			return this._auxTableData["pageTypes"][page];
		}
		public virtual XVar getOriginalPageType(dynamic _param_page = null)
		{
			#region default values
			if(_param_page as Object == null) _param_page = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic page = XVar.Clone(_param_page);
			#endregion

			if(XVar.Pack(!(XVar)(page)))
			{
				page = XVar.Clone(this._page);
			}
			return this._auxTableData["originalPageTypes"][page];
		}
		public virtual XVar getOriginalPages()
		{
			return this._auxTableData["originalPageTypes"];
		}
		public virtual XVar welcomeItems()
		{
			return this.getPageOption(new XVar("welcome"), new XVar("welcomeItems"));
		}
		public virtual XVar welcomePageSkip()
		{
			return this.getPageOption(new XVar("welcome"), new XVar("welcomePageSkip"));
		}
		public virtual XVar getMultipleImgMode(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic editFormat = null;
			editFormat = XVar.Clone(this.getEditFormat((XVar)(field)));
			if((XVar)(editFormat == Constants.EDIT_FORMAT_DATABASE_IMAGE)  || (XVar)(editFormat == Constants.EDIT_FORMAT_DATABASE_FILE))
			{
				return false;
			}
			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageMultipleMode"));
		}
		public virtual XVar getMaxImages(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			if(XVar.Pack(!(XVar)(this.getMultipleImgMode((XVar)(field)))))
			{
				return 1;
			}
			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageMaxCount"));
		}
		public virtual XVar isGalleryEnabled(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageGallery"));
		}
		public virtual XVar getGalleryMode()
		{
			return this.getTableValue(new XVar("galleryMode"));
		}
		public virtual XVar getCaptionMode(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageCaptions"));
		}
		public virtual XVar getCaptionField(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageCaptionField"));
		}
		public virtual XVar getImageBorder(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageBorder"));
		}
		public virtual XVar getImageFullWidth(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("imageMobileFullWidth"));
		}
		public virtual XVar pageTypeAvailable(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic pagesByType = XVar.Array();
			pagesByType = this._auxTableData["pagesByType"];
			return !(XVar)(!(XVar)(pagesByType[pageType]));
		}
		public virtual XVar updatePages()
		{
			dynamic defaultPages = XVar.Array(), newPages = XVar.Array(), pages = XVar.Array(), pagesByType = XVar.Array(), restrictedPages = XVar.Array();
			if(XVar.Pack(this._auxTableData["pagesUpdated"]))
			{
				return null;
			}
			if((XVar)((XVar)((XVar)(this._pageType == Constants.PAGE_LOGIN)  || (XVar)(this._pageType == Constants.PAGE_REGISTER))  || (XVar)(this._pageType == Constants.PAGE_REMIND))  || (XVar)(this._pageType == Constants.PAGE_REMIND_SUCCESS))
			{
				return null;
			}
			this._auxTableData.InitAndSetArrayItem(true, "pagesUpdated");
			restrictedPages = XVar.Clone(Security.getRestrictedPages((XVar)(this._auxTable), this));
			if(XVar.Pack(!(XVar)(restrictedPages)))
			{
				return null;
			}
			pages = this._auxTableData["pageTypes"];
			pagesByType = this._auxTableData["pagesByType"];
			newPages = XVar.Clone(XVar.Array());
			defaultPages = this._auxTableData["defaultPages"];
			foreach (KeyValuePair<XVar, dynamic> var_type in pages.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(restrictedPages[var_type.Key])))
				{
					newPages.InitAndSetArrayItem(var_type.Value, var_type.Key);
				}
				else
				{
					dynamic idx = null;
					idx = XVar.Clone(MVCFunctions.array_search((XVar)(var_type.Key), (XVar)(pagesByType[var_type.Value])));
					pagesByType[var_type.Value].Remove(idx);
					if(defaultPages[var_type.Value] == var_type.Key)
					{
						defaultPages.InitAndSetArrayItem("", var_type.Value);
						foreach (KeyValuePair<XVar, dynamic> d in pagesByType[var_type.Value].GetEnumerator())
						{
							defaultPages.InitAndSetArrayItem(d.Value, var_type.Value);
							break;
						}
					}
				}
			}
			this._auxTableData.InitAndSetArrayItem(newPages, "pageTypes");

			return null;
		}
		public virtual XVar resetPages()
		{
			this._auxTableData.Remove("pagesUpdated");
			this._auxTableData.InitAndSetArrayItem(this._auxTableData["originalDefaultPages"], "defaultPages");
			this._auxTableData.InitAndSetArrayItem(this._auxTableData["originalPageTypes"], "pageTypes");

			return null;
		}
		public virtual XVar getOriginalPagesByType(dynamic _param_pageType)
		{
			#region pass-by-value parameters
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			return this._auxTableData["originalPagesByType"][pageType];
		}
		public virtual XVar getDataSourceOps()
		{
			return this.getTableValue(new XVar("dataSourceOperations"));
		}
		public virtual XVar groupChart()
		{
			return this.getTableValue(new XVar("chartSettings"), new XVar("groupChart"));
		}
		public virtual XVar chartLabelInterval()
		{
			return this.getTableValue(new XVar("chartSettings"), new XVar("labelInterval"));
		}
		public virtual XVar chartSeries()
		{
			return this.getTableValue(new XVar("chartSettings"), new XVar("dataSeries"));
		}
		public virtual XVar chartLabelField()
		{
			return this.getTableValue(new XVar("chartSettings"), new XVar("labelField"));
		}
		public virtual XVar getViewPageType()
		{
			return this._viewPage;
		}
		public virtual XVar spreadsheetGrid()
		{
			return this.getPageOption(new XVar("list"), new XVar("spreadsheetMode"));
		}
		public static XVar uploadEditType(dynamic _param_editFormat)
		{
			#region pass-by-value parameters
			dynamic editFormat = XVar.Clone(_param_editFormat);
			#endregion

			return (XVar)((XVar)(editFormat == Constants.EDIT_FORMAT_DATABASE_FILE)  || (XVar)(editFormat == Constants.EDIT_FORMAT_DATABASE_IMAGE))  || (XVar)(editFormat == Constants.EDIT_FORMAT_FILE);
		}
		public virtual XVar addNewRecordAutomatically()
		{
			return this.getPageOption(new XVar("list"), new XVar("addNewRecordAutomatically"));
		}
		public virtual XVar reorderRows()
		{
			return (XVar)(this.getPageOption(new XVar("list"), new XVar("reorderRows")))  && (XVar)(this.reorderRowsField() != "");
		}
		public virtual XVar reorderRowsField()
		{
			return this.getPageOption(new XVar("list"), new XVar("reorderRowsField"));
		}
		public virtual XVar inlineAddBottom()
		{
			return !(XVar)(!(XVar)((XVar)(this.getPageOption(new XVar("list"), new XVar("addToBottom")))  || (XVar)((XVar)(this.spreadsheetGrid())  && (XVar)(this.addNewRecordAutomatically()))));
		}
		public virtual XVar listColumnsOrderOnPrint()
		{
			return this.getPageOption(new XVar("misc"), new XVar("listColumnsOrderOnPrint"));
		}
		public virtual XVar fileStorageProvider(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("storageProvider"));
		}
		public virtual XVar googleDriveFolder(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("googleDrivePath"));
		}
		public virtual XVar hideAdGroupsUntilSearch()
		{
			return this.getPageOption(new XVar("adGroups"), new XVar("hideUntilSearch"));
		}
		public virtual XVar hasNotifications()
		{
			return this.getPageOption(new XVar("page"), new XVar("hasNotifications"));
		}
		public static XVar amazonSecretKey()
		{
			return ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudAmazonSecretKey"));
		}
		public static XVar amazonAccessKey()
		{
			return ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudAmazonAccessKey"));
		}
		public virtual XVar amazonPath(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("amazonPath"));
		}
		public static XVar amazonBucket()
		{
			return ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudAmazonBucket"));
		}
		public static XVar amazonRegion()
		{
			return ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudAmazonRegion"));
		}
		public static XVar wasabiSecretKey()
		{
			return ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudWasabiSecretKey"));
		}
		public static XVar wasabiAccessKey()
		{
			return ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudWasabiAccessKey"));
		}
		public virtual XVar wasabiPath(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("wasabiPath"));
		}
		public static XVar wasabiBucket()
		{
			return ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudWasabiBucket"));
		}
		public static XVar wasabiRegion()
		{
			return ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudWasabiRegion"));
		}
		public virtual XVar oneDrivePath(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("oneDrivePath"));
		}
		public static XVar oneDriveDrive()
		{
			return ProjectSettings.getProjectValue(new XVar("cloudSettings"), new XVar("cloudOneDriveDrive"));
		}
		public virtual XVar dropboxPath(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("dropboxPath"));
		}
		public virtual XVar hasOldPassField()
		{
			return this.getPageOption(new XVar("changepwd"), new XVar("oldPassFieldOnPage"));
		}
		public virtual XVar getDashboardElements()
		{
			return this.getPageOption(new XVar("dashboard"), new XVar("elements"));
		}
		public virtual XVar getMobileSub()
		{
			return this.getPageOption(new XVar("page"), new XVar("mobileSub"));
		}
		public virtual XVar getChartCount()
		{
			return this.getPageOption(new XVar("chart"), new XVar("chartCount"));
		}
		public virtual XVar hideNumberOfRecords()
		{
			return this.getPageOption(new XVar("list"), new XVar("hideNumberOfRecords"));
		}
		public static XVar languageDescriptors()
		{
			return GlobalVars.runnerProjectSettings["languages"];
		}
		public static XVar languages()
		{
			return GlobalVars.runnerProjectSettings["languageNames"];
		}
		public virtual XVar fieldEditEvents(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("edit"), new XVar("fieldEvents"));
		}
		public virtual XVar fieldViewEvents(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return this.getFieldValue((XVar)(field), new XVar("view"), new XVar("fieldEvents"));
		}
		public virtual XVar fieldHasEvent(dynamic _param_eventId, dynamic _param_field, dynamic _param_editEvent)
		{
			#region pass-by-value parameters
			dynamic eventId = XVar.Clone(_param_eventId);
			dynamic field = XVar.Clone(_param_field);
			dynamic editEvent = XVar.Clone(_param_editEvent);
			#endregion

			dynamic var_false = null;
			if(XVar.Pack(editEvent))
			{
				dynamic editFormats = XVar.Array();
				editFormats = XVar.Clone(this.getTableValue(new XVar("fields"), (XVar)(field), new XVar("editFormats")));
				foreach (KeyValuePair<XVar, dynamic> editFormat in editFormats.GetEnumerator())
				{
					dynamic fieldEvents = null;
					fieldEvents = XVar.Clone(editFormat.Value["fieldEvents"]);
					if(XVar.Pack(this.eventInList((XVar)(editFormat.Value["fieldEvents"]), (XVar)(eventId))))
					{
						return true;
					}
				}
			}
			else
			{
				dynamic viewFormats = XVar.Array();
				viewFormats = XVar.Clone(this.getTableValue(new XVar("fields"), (XVar)(field), new XVar("viewFormats")));
				foreach (KeyValuePair<XVar, dynamic> viewFormat in viewFormats.GetEnumerator())
				{
					if(XVar.Pack(this.eventInList((XVar)(viewFormat.Value["fieldEvents"]), (XVar)(eventId))))
					{
						return true;
					}
				}
			}
			return var_false;
		}
		protected virtual XVar eventInList(dynamic _param_fieldEvents, dynamic _param_eventId)
		{
			#region pass-by-value parameters
			dynamic fieldEvents = XVar.Clone(_param_fieldEvents);
			dynamic eventId = XVar.Clone(_param_eventId);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> e in fieldEvents.GetEnumerator())
			{
				if(e.Value["handlerId"] == eventId)
				{
					return true;
				}
			}
			return false;
		}
		public static XVar ext()
		{
			return ProjectSettings.getProjectValue(new XVar("ext"));
		}
		public static XVar webReports()
		{
			return false;
		}
		public virtual XVar getEditParams(dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			return "";
		}
		public virtual XVar getTableOwnerID()
		{
			return this.getTableOwnerIdField();
		}
	}
	// Included file globals
	public partial class CommonFunctions
	{
		public static XVar findTable(dynamic _param_tableName, dynamic _param_strict = null)
		{
			#region default values
			if(_param_strict as Object == null) _param_strict = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic tableName = XVar.Clone(_param_tableName);
			dynamic strict = XVar.Clone(_param_strict);
			#endregion

			dynamic found = null, projectTables = XVar.Array(), tableObj = XVar.Array(), uTable = null;
			projectTables = ProjectSettings.getProjectTables();
			tableObj = XVar.Clone(projectTables[tableName]);
			if(XVar.Pack(tableObj))
			{
				return tableObj["name"];
			}
			if(XVar.Pack(strict))
			{
				return "";
			}
			uTable = XVar.Clone(MVCFunctions.strtoupper((XVar)(tableName)));
			foreach (KeyValuePair<XVar, dynamic> _table in ProjectSettings.getProjectTables().GetEnumerator())
			{
				dynamic gTable = null;
				if(MVCFunctions.strtoupper((XVar)(_table.Value["name"])) == uTable)
				{
					return _table.Value["name"];
				}
				gTable = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(_table.Value["name"])));
				if(uTable == MVCFunctions.strtoupper((XVar)(gTable)))
				{
					return _table.Value["name"];
				}
			}
			found = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(tableName)));
			if(XVar.Pack(found))
			{
				return found;
			}
			foreach (KeyValuePair<XVar, dynamic> _table in ProjectSettings.getProjectTables().GetEnumerator())
			{
				if(MVCFunctions.strtoupper((XVar)(_table.Value["shortName"])) == uTable)
				{
					return _table.Value["name"];
				}
			}
			return "";
		}
		public static XVar GetTableURL(dynamic _param_tableName)
		{
			#region pass-by-value parameters
			dynamic tableName = XVar.Clone(_param_tableName);
			#endregion

			dynamic projectTables = XVar.Array(), tableObj = XVar.Array();
			if(tableName == "<global>")
			{
				return Constants.GLOBAL_PAGES_SHORT;
			}
			projectTables = ProjectSettings.getProjectTables();
			tableObj = XVar.Clone(projectTables[tableName]);
			if(XVar.Pack(tableObj))
			{
				return tableObj["shortName"];
			}
			return "";
		}
		public static XVar GetTableByGID(dynamic _param_gid)
		{
			#region pass-by-value parameters
			dynamic gid = XVar.Clone(_param_gid);
			#endregion

			if(gid == -1)
			{
				return "<global>";
			}
			foreach (KeyValuePair<XVar, dynamic> _table in ProjectSettings.getProjectTables().GetEnumerator())
			{
				if(_table.Value["gid"] == gid)
				{
					return _table.Key;
				}
			}
			return "";
		}
		public static XVar GetEntityType(dynamic _param_table = null)
		{
			#region default values
			if(_param_table as Object == null) _param_table = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			dynamic projectTables = XVar.Array(), tableName = null, tableObj = XVar.Array();
			if(tableName == Constants.GLOBAL_PAGES)
			{
				return Constants.titGLOBAL;
			}
			projectTables = ProjectSettings.getProjectTables();
			tableObj = XVar.Clone(projectTables[table]);
			if(XVar.Pack(tableObj))
			{
				return tableObj["type"];
			}
			return null;
		}
		public static XVar GetTableByShort(dynamic _param_shortTName)
		{
			#region pass-by-value parameters
			dynamic shortTName = XVar.Clone(_param_shortTName);
			#endregion

			dynamic tableName = null;
			if(tableName == Constants.GLOBAL_PAGES_SHORT)
			{
				return Constants.GLOBAL_PAGES;
			}
			if(XVar.Pack(GlobalVars.runnerProjectSettings["tablesByShort"].KeyExists(shortTName)))
			{
				return GlobalVars.runnerProjectSettings["tablesByShort"][shortTName];
			}
			return "";
		}
		public static XVar GetTableByGood(dynamic _param_goodTableName)
		{
			#region pass-by-value parameters
			dynamic goodTableName = XVar.Clone(_param_goodTableName);
			#endregion

			dynamic tableName = null;
			if(tableName == Constants.GLOBAL_PAGES_SHORT)
			{
				return Constants.GLOBAL_PAGES;
			}
			if(XVar.Pack(GlobalVars.runnerProjectSettings["tablesByGood"].KeyExists(goodTableName)))
			{
				return GlobalVars.runnerProjectSettings["tablesByGood"][goodTableName];
			}
			return "";
		}
		public static XVar getSecurityOption(dynamic _param_option)
		{
			#region pass-by-value parameters
			dynamic var_option = XVar.Clone(_param_option);
			#endregion

			return ProjectSettings.getSecurityValue((XVar)(var_option));
		}
	}
}
