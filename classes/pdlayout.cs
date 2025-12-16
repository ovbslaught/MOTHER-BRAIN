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
	public partial class PDLayout : XClass
	{
		public dynamic bootstrapTheme = XVar.Pack("");
		public dynamic customCssPageName = XVar.Pack("");
		public dynamic page;
		public dynamic table;
		public dynamic version = XVar.Pack(4);
		public dynamic bootstrapSize;
		public dynamic name = XVar.Pack("");
		public dynamic style = XVar.Pack("");
		public dynamic customSettings = XVar.Pack(false);
		public PDLayout(dynamic _param_table, dynamic _param_page, dynamic _param_theme, dynamic _param_size, dynamic _param_name, dynamic _param_customSettings)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic page = XVar.Clone(_param_page);
			dynamic theme = XVar.Clone(_param_theme);
			dynamic size = XVar.Clone(_param_size);
			dynamic name = XVar.Clone(_param_name);
			dynamic customSettings = XVar.Clone(_param_customSettings);
			#endregion

			this.page = XVar.Clone(page);
			this.table = XVar.Clone(table);
			this.bootstrapTheme = XVar.Clone(theme);
			this.bootstrapSize = XVar.Clone(size);
			this.customSettings = XVar.Clone(customSettings);
			this.name = XVar.Clone(name);
		}
		public virtual XVar getCSSFiles(dynamic _param_rtl = null)
		{
			#region default values
			if(_param_rtl as Object == null) _param_rtl = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic rtl = XVar.Clone(_param_rtl);
			#endregion

			dynamic files = XVar.Array(), suffix = null;
			files = XVar.Clone(XVar.Array());
			suffix = new XVar("");
			if(XVar.Pack(rtl))
			{
				suffix = new XVar("RTL");
			}
			if(XVar.Pack(this.customSettings))
			{
				files.InitAndSetArrayItem(MVCFunctions.Concat("styles/bootstrap/custom/", this.name, "/style", suffix, ".css"), null);
			}
			else
			{
				files.InitAndSetArrayItem(MVCFunctions.Concat("styles/bootstrap/", this.bootstrapTheme, "/", this.bootstrapSize, "/style", suffix, ".css"), null);
			}
			files.InitAndSetArrayItem("styles/font-awesome/css/all.min.css", null);
			files.InitAndSetArrayItem("styles/font-awesome/css/v4-shims.min.css", null);
			if(XVar.Pack(!(XVar)(this.customSettings)))
			{
				if(XVar.Pack(ProjectSettings.getProjectValue(new XVar("hasCustomCss"))))
				{
					files.InitAndSetArrayItem("styles/custom/custom.css", null);
				}
			}
			files.InitAndSetArrayItem(MVCFunctions.Concat("styles/pages/", this.table, "_", this.page["id"], suffix, ".css"), null);
			files.InitAndSetArrayItem("fonts/fonts.css", null);
			return files;
		}
		public virtual XVar prepareGrid(dynamic _param_xt_packed, dynamic _param_itemsToHide, dynamic _param_recordItemsToHide, dynamic cellMap, dynamic _param_location, dynamic _param_pageObject)
		{
			#region packeted values
			XTempl _param_xt = XVar.UnPackXTempl(_param_xt_packed);
			#endregion

			#region pass-by-value parameters
			XTempl xt = XVar.Clone(_param_xt);
			dynamic itemsToHide = XVar.Clone(_param_itemsToHide);
			dynamic recordItemsToHide = XVar.Clone(_param_recordItemsToHide);
			dynamic location = XVar.Clone(_param_location);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			dynamic checkRecordItems = null, hiddenColsRows = XVar.Array(), hidingMap = null, recordsToCheck = XVar.Array(), removedColsRows = XVar.Array(), visibleWidth = null;
			checkRecordItems = XVar.Clone(!(XVar)(!(XVar)(recordItemsToHide)));
			recordsToCheck = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> dummy in cellMap.cells.GetEnumerator())
			{
				dynamic cMapRef = XVar.Array(), hidden = null, var_empty = null, visibleItems = XVar.Array();
				cMapRef = cellMap.cells[dummy.Key];
				if(XVar.Pack(cMapRef["fixedAtClient"]))
				{
					continue;
				}
				var_empty = new XVar(false);
				if(XVar.Pack(!(XVar)(cMapRef["fixedAtServer"])))
				{
					var_empty = new XVar(true);
					foreach (KeyValuePair<XVar, dynamic> item in cMapRef["tags"].GetEnumerator())
					{
						if(XVar.Pack(xt.getVar((XVar)(item.Value))))
						{
							var_empty = new XVar(false);
							break;
						}
					}
				}
				hidden = new XVar(true);
				visibleItems = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> item in cMapRef["items"].GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(itemsToHide[item.Value])))
					{
						hidden = new XVar(false);
						if(XVar.Pack(checkRecordItems))
						{
							visibleItems.InitAndSetArrayItem(true, item.Value);
						}
						else
						{
							break;
						}
					}
				}
				if((XVar)((XVar)((XVar)(checkRecordItems)  && (XVar)(!(XVar)(hidden)))  && (XVar)(!(XVar)(var_empty)))  && (XVar)(visibleItems))
				{
					cMapRef.InitAndSetArrayItem(this.findHiddenRecords((XVar)(visibleItems), (XVar)(recordItemsToHide)), "hiddenRecords");
					recordsToCheck = XVar.Clone(CommonFunctions.addToAssocArray((XVar)(recordsToCheck), (XVar)(cMapRef["hiddenRecords"])));
				}
				if(XVar.Pack(var_empty))
				{
					cMapRef.InitAndSetArrayItem(true, "removable");
				}
				if(XVar.Pack(hidden))
				{
					cMapRef.InitAndSetArrayItem(true, "hidable");
				}
			}
			removedColsRows = XVar.Clone(cellMap.removeRowsColumns(new XVar("removable")));
			hidingMap = XVar.Clone(cellMap.makeClone());
			hiddenColsRows = XVar.Clone(hidingMap.removeRowsColumns(new XVar("hidable")));
			visibleWidth = XVar.Clone(cellMap.width - MVCFunctions.count(hiddenColsRows["cols"]));
			xt.assign((XVar)(MVCFunctions.Concat("formwidth_", location)), (XVar)(visibleWidth));
			if(XVar.Pack(!(XVar)(pageObject.pdfJsonMode())))
			{
				foreach (KeyValuePair<XVar, dynamic> row in removedColsRows["rows"].GetEnumerator())
				{
					xt.assign((XVar)(MVCFunctions.Concat("row_", location, "_", row.Value)), new XVar("data-hidden"));
				}
				foreach (KeyValuePair<XVar, dynamic> row in hiddenColsRows["rows"].GetEnumerator())
				{
					xt.assign((XVar)(MVCFunctions.Concat("row_", location, "_", row.Value)), new XVar("data-hidden"));
				}
			}
			else
			{
				dynamic col = null, row = null;
				row = new XVar(0);
				for(;row < cellMap.height; ++(row))
				{
					xt.assign((XVar)(MVCFunctions.Concat("row_", location, "_", row)), new XVar(true));
				}
				foreach (KeyValuePair<XVar, dynamic> _row in removedColsRows["rows"].GetEnumerator())
				{
					xt.assign((XVar)(MVCFunctions.Concat("row_", location, "_", _row.Value)), new XVar(false));
				}
				foreach (KeyValuePair<XVar, dynamic> _row in hiddenColsRows["rows"].GetEnumerator())
				{
					xt.assign((XVar)(MVCFunctions.Concat("row_", location, "_", _row.Value)), new XVar(false));
				}
				col = new XVar(0);
				for(;col < cellMap.width; ++(col))
				{
					xt.assign((XVar)(MVCFunctions.Concat("col_", location, "_", col)), new XVar(true));
				}
				foreach (KeyValuePair<XVar, dynamic> _col in removedColsRows["cols"].GetEnumerator())
				{
					xt.assign((XVar)(MVCFunctions.Concat("col_", location, "_", _col.Value)), new XVar(false));
				}
				foreach (KeyValuePair<XVar, dynamic> _col in hiddenColsRows["cols"].GetEnumerator())
				{
					xt.assign((XVar)(MVCFunctions.Concat("col_", location, "_", _col.Value)), new XVar(false));
				}
			}
			foreach (KeyValuePair<XVar, dynamic> cMap in cellMap.cells.GetEnumerator())
			{
				dynamic dummyData = null;
				if((XVar)(0 == MVCFunctions.count(cMap.Value["rows"]))  || (XVar)(0 == MVCFunctions.count(cMap.Value["cols"])))
				{
					continue;
				}
				xt.assign((XVar)(MVCFunctions.Concat("cellblock_", location, "_", cMap.Key)), new XVar(true));
				dummyData = new XVar(null);
				this.assignCellAttrs((XVar)(hidingMap), (XVar)(cMap.Key), (XVar)(location), (XVar)(pageObject), (XVar)(xt), (XVar)(dummyData));
			}
			if(XVar.Pack(checkRecordItems))
			{
				foreach (KeyValuePair<XVar, dynamic> dummy in recordsToCheck.GetEnumerator())
				{
					dynamic hiddenRecordRows = XVar.Array(), recordData = XVar.Array(), recordHidingMap = null;
					recordHidingMap = XVar.Clone(hidingMap.makeClone());
					recordHidingMap.setRecordId((XVar)(dummy.Key));
					hiddenRecordRows = XVar.Clone(recordHidingMap.removeRowsColumns(new XVar("hidable"), new XVar(true)));
					recordData = pageObject.findRecordAssigns((XVar)(dummy.Key));
					if(XVar.Pack(!(XVar)(pageObject.pdfJsonMode())))
					{
						foreach (KeyValuePair<XVar, dynamic> row in hiddenRecordRows["rows"].GetEnumerator())
						{
							recordData.InitAndSetArrayItem("data-hidden", MVCFunctions.Concat("row_", location, "_", row.Value));
						}
					}
					else
					{
						foreach (KeyValuePair<XVar, dynamic> row in hiddenRecordRows["rows"].GetEnumerator())
						{
							recordData.InitAndSetArrayItem(false, MVCFunctions.Concat("row_", location, "_", row.Value));
						}
					}
					foreach (KeyValuePair<XVar, dynamic> cMap in cellMap.cells.GetEnumerator())
					{
						if((XVar)(0 == MVCFunctions.count(cMap.Value["rows"]))  || (XVar)(0 == MVCFunctions.count(cMap.Value["cols"])))
						{
							continue;
						}
						this.assignCellAttrs((XVar)(recordHidingMap), (XVar)(cMap.Key), (XVar)(location), (XVar)(pageObject), (XVar)(xt), (XVar)(recordData), new XVar(true));
					}
				}
			}
			return hidingMap;
		}
		public virtual XVar assignCellAttrs(dynamic cellMap, dynamic _param_cell, dynamic _param_location, dynamic _param_pageObject, dynamic _param_xt_packed, dynamic recordData, dynamic _param_forceCellSpans = null)
		{
			#region packeted values
			XTempl _param_xt = XVar.UnPackXTempl(_param_xt_packed);
			#endregion

			#region default values
			if(_param_forceCellSpans as Object == null) _param_forceCellSpans = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic cell = XVar.Clone(_param_cell);
			dynamic location = XVar.Clone(_param_location);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			XTempl xt = XVar.Clone(_param_xt);
			dynamic forceCellSpans = XVar.Clone(_param_forceCellSpans);
			#endregion

			dynamic cellAttrs = XVar.Array(), hCell = XVar.Array();
			cellAttrs = XVar.Clone(XVar.Array());
			hCell = cellMap.cells[cell];
			if(XVar.Pack(!(XVar)(pageObject.pdfJsonMode())))
			{
				if((XVar)(0 == MVCFunctions.count(hCell["rows"]))  || (XVar)(0 == MVCFunctions.count(hCell["cols"])))
				{
					cellAttrs.InitAndSetArrayItem("data-hidden", null);
				}
				if((XVar)(forceCellSpans)  || (XVar)(1 < MVCFunctions.count(hCell["cols"])))
				{
					cellAttrs.InitAndSetArrayItem(MVCFunctions.Concat("colspan=\"", MVCFunctions.count(hCell["cols"]), "\""), null);
				}
				if((XVar)(forceCellSpans)  || (XVar)(1 < MVCFunctions.count(hCell["rows"])))
				{
					cellAttrs.InitAndSetArrayItem(MVCFunctions.Concat("rowspan=\"", MVCFunctions.count(hCell["rows"]), "\""), null);
				}
				if(XVar.Pack(MVCFunctions.count(cellAttrs)))
				{
					this.assignPageVar((XVar)(recordData), (XVar)(xt), (XVar)(MVCFunctions.Concat("cell_", location, "_", cell)), (XVar)(MVCFunctions.implode(new XVar(" "), (XVar)(cellAttrs))));
				}
			}
			else
			{
				if((XVar)(0 == MVCFunctions.count(hCell["rows"]))  || (XVar)(0 == MVCFunctions.count(hCell["cols"])))
				{
					this.assignPageVar((XVar)(recordData), (XVar)(xt), (XVar)(MVCFunctions.Concat("cellblock_", location, "_", cell)), new XVar(false));
				}
				if((XVar)(forceCellSpans)  || (XVar)(1 < MVCFunctions.count(hCell["cols"])))
				{
					this.assignPageVar((XVar)(recordData), (XVar)(xt), (XVar)(MVCFunctions.Concat("colspan_", location, "_", cell)), (XVar)(MVCFunctions.count(hCell["cols"])));
				}
				if((XVar)(forceCellSpans)  || (XVar)(1 < MVCFunctions.count(hCell["rows"])))
				{
					this.assignPageVar((XVar)(recordData), (XVar)(xt), (XVar)(MVCFunctions.Concat("rowspan_", location, "_", cell)), (XVar)(MVCFunctions.count(hCell["rows"])));
				}
			}

			return null;
		}
		public virtual XVar assignPageVar(dynamic recordData, dynamic _param_xt_packed, dynamic _param_name, dynamic _param_value)
		{
			#region packeted values
			XTempl _param_xt = XVar.UnPackXTempl(_param_xt_packed);
			#endregion

			#region pass-by-value parameters
			XTempl xt = XVar.Clone(_param_xt);
			dynamic name = XVar.Clone(_param_name);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if(XVar.Equals(XVar.Pack(recordData), XVar.Pack(null)))
			{
				xt.assign((XVar)(name), (XVar)(value));
			}
			else
			{
				recordData.InitAndSetArrayItem(value, name);
			}

			return null;
		}
		public virtual XVar findHiddenRecords(dynamic _param_allItems, dynamic _param_hiddenItems)
		{
			#region pass-by-value parameters
			dynamic allItems = XVar.Clone(_param_allItems);
			dynamic hiddenItems = XVar.Clone(_param_hiddenItems);
			#endregion

			dynamic result = null;
			result = new XVar(null);
			foreach (KeyValuePair<XVar, dynamic> dummy in allItems.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(hiddenItems[dummy.Key])))
				{
					return XVar.Array();
				}
				if(XVar.Equals(XVar.Pack(result), XVar.Pack(null)))
				{
					result = XVar.Clone(hiddenItems[dummy.Key]);
				}
				else
				{
					result = XVar.Clone(MVCFunctions.array_intersect((XVar)(result), (XVar)(hiddenItems[dummy.Key])));
				}
				if(MVCFunctions.count(result) == 0)
				{
					break;
				}
			}
			return result;
		}
		public virtual XVar visibleOnMedia(dynamic _param_media, dynamic _param_visibilty)
		{
			#region pass-by-value parameters
			dynamic media = XVar.Clone(_param_media);
			dynamic visibilty = XVar.Clone(_param_visibilty);
			#endregion

			if(media == XVar.Pack(0))
			{
				return (XVar)((XVar)((XVar)(visibilty == XVar.Pack(0))  || (XVar)(visibilty == 3))  || (XVar)(visibilty == 4))  || (XVar)(visibilty == 5);
			}
			else
			{
				if(media == 1)
				{
					return (XVar)((XVar)(visibilty == XVar.Pack(0))  || (XVar)(visibilty == 2))  || (XVar)(visibilty == 4);
				}
			}

			return null;
		}
		public virtual XVar prepareForms(dynamic _param_xt_packed, dynamic _param_itemsToHide, dynamic _param_recordItemsToHide, dynamic _param_pageObject)
		{
			#region packeted values
			XTempl _param_xt = XVar.UnPackXTempl(_param_xt_packed);
			#endregion

			#region pass-by-value parameters
			XTempl xt = XVar.Clone(_param_xt);
			dynamic itemsToHide = XVar.Clone(_param_itemsToHide);
			dynamic recordItemsToHide = XVar.Clone(_param_recordItemsToHide);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			dynamic cellMaps = XVar.Array(), formItems = XVar.Array(), formTags = XVar.Array(), helper = XVar.Array(), invisibleItems = XVar.Array(), mediaHidenItems = XVar.Array(), mediaType = null, present = null, ps = null, visibleCellsMap = XVar.Array();
			if(XVar.Pack(pageObject))
			{
				ps = XVar.Clone(pageObject.pSet);
			}
			else
			{
				ps = XVar.Clone(new ProjectSettings((XVar)(CommonFunctions.GetTableByShort((XVar)(this.table))), (XVar)(this.page["type"]), (XVar)(this.page["id"])));
			}
			helper = ps.helperFormItems();
			invisibleItems = XVar.Clone(itemsToHide);
			mediaHidenItems = XVar.Clone(XVar.Array());
			mediaType = XVar.Clone((XVar.Pack(pageObject.pdfJsonMode()) ? XVar.Pack(Constants.MEDIA_DESKTOP) : XVar.Pack(CommonFunctions.getMediaType())));
			foreach (KeyValuePair<XVar, dynamic> visibility in helper["itemVisiblity"].GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(this.visibleOnMedia((XVar)(mediaType), (XVar)(visibility.Value)))))
				{
					invisibleItems.InitAndSetArrayItem(true, visibility.Key);
					mediaHidenItems.InitAndSetArrayItem(true, visibility.Key);
				}
			}
			visibleCellsMap = XVar.Clone(XVar.Array());
			cellMaps = ps.helperCellMaps();
			foreach (KeyValuePair<XVar, dynamic> loc in MVCFunctions.array_keys((XVar)(cellMaps)).GetEnumerator())
			{
				dynamic formRecordItemsToHide = XVar.Array(), hMap = null;
				formRecordItemsToHide = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> itemRecords in recordItemsToHide.GetEnumerator())
				{
					if(helper["itemForms"][itemRecords.Key] == loc.Value)
					{
						formRecordItemsToHide.InitAndSetArrayItem(MVCFunctions.array_keys((XVar)(itemRecords.Value)), itemRecords.Key);
					}
				}
				hMap = this.prepareGrid((XVar)(xt), (XVar)(invisibleItems), (XVar)(formRecordItemsToHide), (XVar)(new CellMapPD((XVar)(cellMaps[loc.Value]))), (XVar)(loc.Value), (XVar)(pageObject));
				if(XVar.Pack(pageObject))
				{
					visibleCellsMap.InitAndSetArrayItem(this.prepareClientCellMap((XVar)(cellMaps[loc.Value]), (XVar)(hMap)), loc.Value);
				}
			}
			if(XVar.Pack(pageObject))
			{
				pageObject.setPageData(new XVar("cellMaps"), (XVar)(visibleCellsMap));
			}
			if(XVar.Pack(!(XVar)(pageObject.pdfJsonMode())))
			{
				foreach (KeyValuePair<XVar, dynamic> item in MVCFunctions.array_keys((XVar)(invisibleItems)).GetEnumerator())
				{
					dynamic hideAttrs = XVar.Array();
					hideAttrs = XVar.Clone(XVar.Array());
					if(XVar.Pack(mediaHidenItems[item.Value]))
					{
						hideAttrs.InitAndSetArrayItem("data-media-hidden", null);
					}
					if(XVar.Pack(itemsToHide[item.Value]))
					{
						hideAttrs.InitAndSetArrayItem("data-hidden", null);
					}
					xt.assign((XVar)(MVCFunctions.Concat("item_", item.Value)), (XVar)(MVCFunctions.implode(new XVar(" "), (XVar)(hideAttrs))));
				}
			}
			else
			{
				foreach (KeyValuePair<XVar, dynamic> item in MVCFunctions.array_keys((XVar)(invisibleItems)).GetEnumerator())
				{
					xt.assign((XVar)(MVCFunctions.Concat("item_hide_", item.Value)), new XVar("1"));
				}
			}
			if(XVar.Pack(pageObject))
			{
				foreach (KeyValuePair<XVar, dynamic> itemRecords in recordItemsToHide.GetEnumerator())
				{
					foreach (KeyValuePair<XVar, dynamic> recordId in MVCFunctions.array_keys((XVar)(itemRecords.Value)).GetEnumerator())
					{
						pageObject.hideRecordItem((XVar)(itemRecords.Key), (XVar)(recordId.Value));
					}
				}
			}
			xt.assign(new XVar("firstAboveGridCell"), new XVar(true));
			formTags = helper["formXtTags"];
			foreach (KeyValuePair<XVar, dynamic> loc in MVCFunctions.array_keys((XVar)(formTags)).GetEnumerator())
			{
				present = new XVar(false);
				foreach (KeyValuePair<XVar, dynamic> tag in formTags[loc.Value].GetEnumerator())
				{
					if(XVar.Pack(xt.getVar((XVar)(tag.Value))))
					{
						present = new XVar(true);
						break;
					}
				}
				if(XVar.Pack(!(XVar)(present)))
				{
					xt.assign((XVar)(MVCFunctions.Concat(loc.Value, "_block")), new XVar(false));
					xt.assign((XVar)(MVCFunctions.Concat(loc.Value, "_outerblock")), new XVar(false));
				}
			}
			formItems = helper["formItems"];
			foreach (KeyValuePair<XVar, dynamic> loc in MVCFunctions.array_keys((XVar)(formItems)).GetEnumerator())
			{
				present = new XVar(false);
				foreach (KeyValuePair<XVar, dynamic> item in formItems[loc.Value].GetEnumerator())
				{
					if(XVar.Pack(!(XVar)(invisibleItems.KeyExists(item.Value))))
					{
						present = new XVar(true);
						break;
					}
				}
				if(XVar.Pack(!(XVar)(present)))
				{
					xt.assign((XVar)(MVCFunctions.Concat("form_", loc.Value)), new XVar("data-hidden"));
				}
			}

			return null;
		}
		protected virtual XVar prepareClientCellMap(dynamic allCells, dynamic visibleCells)
		{
			foreach (KeyValuePair<XVar, dynamic> cellId in MVCFunctions.array_keys((XVar)(visibleCells.cells)).GetEnumerator())
			{
				allCells.InitAndSetArrayItem(visibleCells.cells[cellId.Value]["cols"], "cells", cellId.Value, "visibleCols");
			}
			return allCells;
		}
	}
	public partial class CellMapPD : XClass
	{
		public dynamic cells;
		public dynamic height;
		public dynamic width;
		public CellMapPD(dynamic map)
		{
			this.cells = map["cells"];
			this.height = XVar.Clone(map["height"]);
			this.width = XVar.Clone(map["width"]);
		}
		public virtual XVar makeClone()
		{
			dynamic newMap = null;
			newMap = XVar.Clone(new XVar("cells", MVCFunctions.cloneArray((XVar)(this.cells)), "height", this.height, "width", this.width));
			return new CellMapPD((XVar)(newMap));
		}
		public virtual XVar setRecordId(dynamic _param_recId)
		{
			#region pass-by-value parameters
			dynamic recId = XVar.Clone(_param_recId);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> dummy in this.cells.GetEnumerator())
			{
				dynamic cMapRef = XVar.Array();
				cMapRef = this.cells[dummy.Key];
				if((XVar)(!(XVar)(cMapRef["hiddenRecords"]))  || (XVar)(cMapRef["hidable"]))
				{
					continue;
				}
				foreach (KeyValuePair<XVar, dynamic> rec in cMapRef["hiddenRecords"].GetEnumerator())
				{
					if(XVar.Equals(XVar.Pack(rec.Value), XVar.Pack(recId)))
					{
						cMapRef.InitAndSetArrayItem(true, "hidable");
						break;
					}
				}
			}

			return null;
		}
		public virtual XVar getColumnCells(dynamic _param_col)
		{
			#region pass-by-value parameters
			dynamic col = XVar.Clone(_param_col);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> cMap in this.cells.GetEnumerator())
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(col), (XVar)(cMap.Value["cols"]))), XVar.Pack(false)))
				{
					ret.InitAndSetArrayItem(cMap.Key, null);
				}
			}
			return ret;
		}
		public virtual XVar getRowCells(dynamic _param_row)
		{
			#region pass-by-value parameters
			dynamic row = XVar.Clone(_param_row);
			#endregion

			dynamic ret = XVar.Array();
			ret = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> cMap in this.cells.GetEnumerator())
			{
				if(!XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(row), (XVar)(cMap.Value["rows"]))), XVar.Pack(false)))
				{
					ret.InitAndSetArrayItem(cMap.Key, null);
				}
			}
			return ret;
		}
		public virtual XVar removeRowsColumns(dynamic _param_cellRemoveFlag, dynamic _param_rowsOnly = null)
		{
			#region default values
			if(_param_rowsOnly as Object == null) _param_rowsOnly = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic cellRemoveFlag = XVar.Clone(_param_cellRemoveFlag);
			dynamic rowsOnly = XVar.Clone(_param_rowsOnly);
			#endregion

			dynamic canDeleteRow = null, cell = null, i = null, ret = XVar.Array(), row = null, rowCells = XVar.Array(), rowIdx = null;
			ret = XVar.Clone(new XVar("cols", XVar.Array(), "rows", XVar.Array()));
			if(XVar.Pack(!(XVar)(rowsOnly)))
			{
				dynamic canDeleteCol = null, col = null, colCells = XVar.Array(), colIdx = null;
				col = new XVar(0);
				for(;col < this.width; ++(col))
				{
					colCells = XVar.Clone(this.getColumnCells((XVar)(col)));
					canDeleteCol = new XVar(true);
					foreach (KeyValuePair<XVar, dynamic> _cell in colCells.GetEnumerator())
					{
						if((XVar)(!(XVar)(this.cells[_cell.Value][cellRemoveFlag]))  && (XVar)(MVCFunctions.count(this.cells[_cell.Value]["cols"]) == 1))
						{
							canDeleteCol = new XVar(false);
							break;
						}
					}
					if(XVar.Pack(!(XVar)(canDeleteCol)))
					{
						continue;
					}
					i = new XVar(0);
					for(;i < MVCFunctions.count(colCells); ++(i))
					{
						cell = XVar.Clone(colCells[i]);
						colIdx = XVar.Clone(MVCFunctions.array_search((XVar)(col), (XVar)(this.cells[cell]["cols"])));
						if(ProjectSettings.ext() == "aspx ")
						{
							dynamic colsArr = null;
							colsArr = this.cells[cell]["cols"];
							MVCFunctions.array_splice((XVar)(colsArr), (XVar)(colIdx), new XVar(1));
							this.cells.InitAndSetArrayItem(colsArr, cell, "cols");
						}
						else
						{
							MVCFunctions.array_splice((XVar)(this.cells[cell]["cols"]), (XVar)(colIdx), new XVar(1));
						}
					}
					ret.InitAndSetArrayItem(col, "cols", null);
				}
			}
			row = XVar.Clone(this.height - 1);
			for(;XVar.Pack(0) <= row; --(row))
			{
				rowCells = XVar.Clone(this.getRowCells((XVar)(row)));
				canDeleteRow = new XVar(true);
				foreach (KeyValuePair<XVar, dynamic> _cell in rowCells.GetEnumerator())
				{
					if((XVar)(!(XVar)(this.cells[_cell.Value][cellRemoveFlag]))  && (XVar)((XVar)(MVCFunctions.count(this.cells[_cell.Value]["rows"]) == 1)  || (XVar)(XVar.Equals(XVar.Pack(this.cells[_cell.Value]["rows"][0]), XVar.Pack(row)))))
					{
						canDeleteRow = new XVar(false);
						break;
					}
				}
				if(XVar.Pack(!(XVar)(canDeleteRow)))
				{
					continue;
				}
				i = new XVar(0);
				for(;i < MVCFunctions.count(rowCells); ++(i))
				{
					cell = XVar.Clone(rowCells[i]);
					rowIdx = XVar.Clone(MVCFunctions.array_search((XVar)(row), (XVar)(this.cells[cell]["rows"])));
					if(ProjectSettings.ext() == "aspx ")
					{
						dynamic rowsArr = null;
						rowsArr = XVar.Clone(this.cells[cell]["rows"]);
						MVCFunctions.array_splice((XVar)(rowsArr), (XVar)(rowIdx), new XVar(1));
						this.cells.InitAndSetArrayItem(rowsArr, cell, "rows");
					}
					else
					{
						MVCFunctions.array_splice((XVar)(this.cells[cell]["rows"]), (XVar)(rowIdx), new XVar(1));
					}
				}
				ret.InitAndSetArrayItem(row, "rows", null);
			}
			return ret;
		}
	}
}
