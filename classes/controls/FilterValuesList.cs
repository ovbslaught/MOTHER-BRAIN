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
	public partial class FilterValuesList : FilterControl
	{
		protected dynamic useFormatedValueInSorting;
		protected dynamic isDescendingSortOrder;
		protected dynamic sortingType;
		protected dynamic dependentFilterNames = XVar.Array();
		protected dynamic dependentFilterName;
		protected dynamic hasDependent = XVar.Pack(false);
		protected dynamic filteredFieldParentFiltersKeysToIgnore = XVar.Pack(null);
		protected dynamic hideShowMore = XVar.Pack(false);
		protected dynamic numberOfVisibleItems = XVar.Pack(0);
		protected dynamic truncated = XVar.Pack(false);
		protected dynamic numberOfExtraItemsToShow = XVar.Pack(0);
		protected dynamic hiddenExtraItemClassName = XVar.Pack("filter-item-hidden");
		protected dynamic aliases = XVar.Array();
		protected static bool skipFilterValuesListCtor = false;
		public FilterValuesList(dynamic _param_fName, dynamic _param_pageObject, dynamic _param_id, dynamic _param_viewControls)
			:base((XVar)_param_fName, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_viewControls)
		{
			if(skipFilterValuesListCtor)
			{
				skipFilterValuesListCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic viewControls = XVar.Clone(_param_viewControls);
			#endregion

			this.filterFormat = new XVar(Constants.FF_VALUE_LIST);
			this.separator = new XVar("~equals~");
			this.useApllyBtn = XVar.Clone(this.multiSelect == Constants.FM_ALWAYS);
			this.numberOfVisibleItems = XVar.Clone(this.pSet.getNumberOfVisibleFilterItems((XVar)(fName)));
			this.parentFilterName = XVar.Clone(this.pSet.getParentFilterName((XVar)(fName)));
			this.dependent = XVar.Clone(!(XVar)(!(XVar)(this.parentFilterName)));
			this.assignDependentFiltersData();
			this.hasDependent = XVar.Clone(!(XVar)(!(XVar)(this.dependentFilterName)));
			this.assignParentFiltersData();
			this.setSortingParams();
		}
		protected virtual XVar assignParentFiltersData()
		{
			if(XVar.Pack(!(XVar)(this.dependent)))
			{
				return null;
			}
			this.parentFiltersNames = XVar.Clone(FilterValuesList.getParentFilterFields((XVar)(this.fName), (XVar)(this.pSet)));

			return null;
		}
		public static XVar getParentFilterFields(dynamic _param_field, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic parents = null;
			parents = XVar.Clone(XVar.Array());
			FilterValuesList.findParentFilters((XVar)(field), (XVar)(parents), (XVar)(pSet));
			return MVCFunctions.array_keys((XVar)(parents));
		}
		protected virtual XVar assignDependentFiltersData()
		{
			dynamic dependents = null;
			dependents = XVar.Clone(XVar.Array());
			this.findDependentFilters((XVar)(this.fName), (XVar)(this.pSet.getFilterFields()), (XVar)(dependents));
			this.dependentFilterNames = XVar.Clone(MVCFunctions.array_keys((XVar)(dependents)));

			return null;
		}
		public override XVar hasDependentFilter()
		{
			return !(XVar)(!(XVar)(this.dependentFilterName));
		}
		protected virtual XVar findDependentFilters(dynamic _param_field, dynamic filterFields, dynamic dependents)
		{
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> f in filterFields.GetEnumerator())
			{
				if((XVar)(!(XVar)(dependents.KeyExists(f.Value)))  && (XVar)(XVar.Equals(XVar.Pack(this.pSet.getParentFilterName((XVar)(f.Value))), XVar.Pack(field))))
				{
					dependents.InitAndSetArrayItem(true, f.Value);
					if(field == this.fName)
					{
						this.dependentFilterName = XVar.Clone(f.Value);
					}
					findDependentFilters((XVar)(f.Value), (XVar)(filterFields), (XVar)(dependents));
					break;
				}
			}

			return null;
		}
		protected static XVar findParentFilters(dynamic _param_field, dynamic parents, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			dynamic f = null;
			f = XVar.Clone(pSet.getParentFilterName((XVar)(field)));
			if(XVar.Pack(!(XVar)(f)))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(parents.KeyExists(f))))
			{
				parents.InitAndSetArrayItem(true, f);
				FilterValuesList.findParentFilters((XVar)(f), (XVar)(parents), (XVar)(pSet));
			}

			return null;
		}
		protected virtual XVar getDataCommand()
		{
			dynamic alias = null, dc = null, sortingType = null, totalsType = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(this.pageObject.getDataSourceFilterCriteria((XVar)(this.fName)));
			alias = XVar.Clone(CommonFunctions.generateAlias());
			this.aliases.InitAndSetArrayItem(alias, this.fName);
			dc.totals.InitAndSetArrayItem(new XVar("field", this.fName, "alias", alias, "modifier", this.pSet.getFilterByInterval((XVar)(this.fName)), "skipEmpty", true), null);
			totalsType = XVar.Clone(this.dataTotalsName());
			if(XVar.Pack(totalsType))
			{
				dc.totals.InitAndSetArrayItem(new XVar("field", this.totalsfName, "alias", MVCFunctions.Concat(this.fName, "TOTAL"), "total", totalsType), null);
			}
			if(XVar.Pack(this.dependent))
			{
				foreach (KeyValuePair<XVar, dynamic> p in this.parentFiltersNames.GetEnumerator())
				{
					dynamic pAlias = null;
					pAlias = XVar.Clone(CommonFunctions.generateAlias());
					this.aliases.InitAndSetArrayItem(pAlias, p.Value);
					dc.totals.InitAndSetArrayItem(new XVar("field", p.Value, "alias", pAlias, "modifier", this.pSet.getFilterByInterval((XVar)(p.Value)), "skipEmpty", true), null);
				}
			}
			sortingType = XVar.Clone(this.pSet.getFilterSortValueType((XVar)(this.fName)));
			if(sortingType != Constants.SORT_BY_DISP_VALUE)
			{
				dynamic direction = null;
				direction = XVar.Clone((XVar.Pack(this.pSet.isFilterSortOrderDescending((XVar)(this.fName))) ? XVar.Pack("DESC") : XVar.Pack("ASC")));
				if((XVar)(sortingType == Constants.SORT_BY_GR_VALUE)  && (XVar)(totalsType))
				{
					dc.totals.InitAndSetArrayItem(direction, 1, "direction");
				}
				else
				{
					dc.totals.InitAndSetArrayItem(direction, 0, "direction");
				}
			}
			return dc;
		}
		protected virtual XVar getFilterSQLExpr(dynamic _param_expr)
		{
			#region pass-by-value parameters
			dynamic expr = XVar.Clone(_param_expr);
			#endregion

			return FilterValuesList._getFilterSQLExpr((XVar)(this.fName), (XVar)(expr), (XVar)(this.pSet), (XVar)(this.connection));
		}
		protected static XVar _getFilterSQLExpr(dynamic _param_fName, dynamic _param_expr, dynamic _param_pSet_packed, dynamic _param_connection)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic expr = XVar.Clone(_param_expr);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			dynamic filterInterval = null, ftype = null;
			filterInterval = XVar.Clone(pSet.getFilterByInterval((XVar)(fName)));
			if(XVar.Pack(!(XVar)(filterInterval)))
			{
				return expr;
			}
			ftype = XVar.Clone(pSet.getFieldType((XVar)(fName)));
			if(XVar.Pack(CommonFunctions.IsNumberType((XVar)(ftype))))
			{
				return connection.intervalExpressionNumber((XVar)(expr), (XVar)(filterInterval));
			}
			if(XVar.Pack(CommonFunctions.IsCharType((XVar)(ftype))))
			{
				return connection.intervalExpressionString((XVar)(expr), (XVar)(filterInterval));
			}
			if(XVar.Pack(CommonFunctions.IsDateFieldType((XVar)(ftype))))
			{
				return connection.intervalExpressionDate((XVar)(expr), (XVar)(filterInterval));
			}
			return expr;
		}
		protected virtual XVar setSortingParams()
		{
			this.sortingType = XVar.Clone(this.pSet.getFilterSortValueType((XVar)(this.fName)));
			this.isDescendingSortOrder = XVar.Clone(this.pSet.isFilterSortOrderDescending((XVar)(this.fName)));
			this.useFormatedValueInSorting = XVar.Clone((XVar)((XVar)(this.sortingType == Constants.SORT_BY_DISP_VALUE)  || (XVar)(CommonFunctions.IsCharType((XVar)(this.fieldType))))  || (XVar)(this.pSet.getEditFormat((XVar)(this.fName)) == Constants.EDIT_FORMAT_LOOKUP_WIZARD));

			return null;
		}
		protected override XVar getValueToShow(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic data = null, showValue = null;
			if(XVar.Pack(!(XVar)(this.viewControl)))
			{
				return "";
			}
			data = XVar.Clone(new XVar(this.fName, value));
			showValue = XVar.Clone(this.viewControl.showDBValue((XVar)(data), new XVar("")));
			return showValue;
		}
		protected override XVar addFilterBlocksFromDB(dynamic filterCtrlBlocks)
		{
			dynamic alias = null, containsFilteredDependentOnDemandFilter = null, data = XVar.Array(), filterControl = null, parentFiltersData = XVar.Array(), qResult = null, rawValue = null, visibilityClass = null;
			containsFilteredDependentOnDemandFilter = XVar.Clone((XVar)((XVar)(!(XVar)(this.dependent))  && (XVar)(!(XVar)(this.filtered)))  && (XVar)(this.hasFilteredDependentOnDemandFilter()));
			visibilityClass = XVar.Clone((XVar.Pack((XVar)(this.filtered)  && (XVar)(this.multiSelect == Constants.FM_ON_DEMAND)) ? XVar.Pack(this.onDemandHiddenItemClassName) : XVar.Pack("")));
			qResult = XVar.Clone(this.dataSource.getTotals((XVar)(this.getDataCommand())));
			alias = XVar.Clone(this.aliases[this.fName]);
			while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
			{
				this.decryptDataRow((XVar)(data));
				rawValue = XVar.Clone(data[alias]);
				parentFiltersData = XVar.Clone(XVar.Array());
				if(XVar.Pack(this.dependent))
				{
					foreach (KeyValuePair<XVar, dynamic> pName in this.parentFiltersNames.GetEnumerator())
					{
						parentFiltersData.InitAndSetArrayItem(data[this.aliases[pName.Value]], pName.Value);
					}
				}
				this.valuesObtainedFromDB.InitAndSetArrayItem(rawValue, null);
				filterControl = XVar.Clone(this.buildControl((XVar)(data), (XVar)(parentFiltersData)));
				if(XVar.Pack(containsFilteredDependentOnDemandFilter))
				{
					filterControl = XVar.Clone(MVCFunctions.Concat(this.getDelButtonHtml((XVar)(this.gfName), (XVar)(this.id), (XVar)(rawValue)), filterControl));
				}
				filterCtrlBlocks.InitAndSetArrayItem(this.getFilterBlockStructure((XVar)(filterControl), (XVar)(visibilityClass), (XVar)(rawValue), (XVar)(parentFiltersData)), null);
			}

			return null;
		}
		protected virtual XVar hasFilteredDependentOnDemandFilter()
		{
			if(XVar.Pack(!(XVar)(this.hasDependent)))
			{
				return false;
			}
			foreach (KeyValuePair<XVar, dynamic> dName in this.dependentFilterNames.GetEnumerator())
			{
				if((XVar)(!(XVar)(!(XVar)(this.filteredFields[dName.Value])))  && (XVar)(this.pSet.getFilterFiledMultiSelect((XVar)(dName.Value)) == Constants.FM_ON_DEMAND))
				{
					return true;
				}
			}
			return false;
		}
		protected override XVar getParentFiltersDataForFilteredBlock(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic pValue = null, parentFiltersData = XVar.Array(), parentValuesData = XVar.Array();
			parentFiltersData = XVar.Clone(XVar.Array());
			if((XVar)(!(XVar)(this.filtered))  || (XVar)(!(XVar)(this.dependent)))
			{
				return parentFiltersData;
			}
			parentValuesData = XVar.Clone(this.filteredFields[this.fName]["parentValues"]);
			if(MVCFunctions.count(parentValuesData) <= 1)
			{
				foreach (KeyValuePair<XVar, dynamic> pName in this.parentFiltersNames.GetEnumerator())
				{
					pValue = XVar.Clone(this.filteredFields[pName.Value]["values"][0]);
					if((XVar)(!(XVar)(this.filteredFields.KeyExists(pName.Value)))  && (XVar)(MVCFunctions.count(parentValuesData[0])))
					{
						pValue = XVar.Clone(parentValuesData[0][0]);
					}
					parentFiltersData.InitAndSetArrayItem(pValue, pName.Value);
				}
				return parentFiltersData;
			}
			if(XVar.Pack(!(XVar)(this.filteredFieldParentFiltersKeysToIgnore)))
			{
				this.filteredFieldParentFiltersKeysToIgnore = XVar.Clone(XVar.Array());
			}
			foreach (KeyValuePair<XVar, dynamic> parentValues in parentValuesData.GetEnumerator())
			{
				if((XVar)(value != this.filteredFields[this.fName]["values"][parentValues.Key])  || (XVar)(MVCFunctions.in_array((XVar)(parentValues.Key), (XVar)(this.filteredFieldParentFiltersKeysToIgnore))))
				{
					continue;
				}
				this.filteredFieldParentFiltersKeysToIgnore.InitAndSetArrayItem(parentValues.Key, null);
				foreach (KeyValuePair<XVar, dynamic> pName in this.parentFiltersNames.GetEnumerator())
				{
					pValue = XVar.Clone(parentValues.Value[pName.Key]);
					parentFiltersData.InitAndSetArrayItem(pValue, pName.Value);
				}
				return parentFiltersData;
			}
			return parentFiltersData;
		}
		protected override XVar getFilterBlockStructure(dynamic _param_filterControl, dynamic _param_visibilityClass = null, dynamic _param_value = null, dynamic _param_parentFiltersData = null)
		{
			#region default values
			if(_param_visibilityClass as Object == null) _param_visibilityClass = new XVar("");
			if(_param_value as Object == null) _param_value = new XVar("");
			if(_param_parentFiltersData as Object == null) _param_parentFiltersData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic filterControl = XVar.Clone(_param_filterControl);
			dynamic visibilityClass = XVar.Clone(_param_visibilityClass);
			dynamic value = XVar.Clone(_param_value);
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			dynamic sortValue = null;
			if(XVar.Pack(!(XVar)(this.viewControl)))
			{
				return XVar.Array();
			}
			sortValue = XVar.Clone(value);
			if(XVar.Pack(this.useFormatedValueInSorting))
			{
				dynamic valueData = null;
				valueData = XVar.Clone(new XVar(this.fName, sortValue));
				sortValue = XVar.Clone(this.viewControl.showDBValue((XVar)(valueData), new XVar("")));
			}
			if(this.multiSelect != Constants.FM_ALWAYS)
			{
				visibilityClass = MVCFunctions.Concat(visibilityClass, " filter-link");
			}
			return new XVar(MVCFunctions.Concat(this.gfName, "_filter"), filterControl, MVCFunctions.Concat("visibilityClass_", this.gfName), visibilityClass, "sortValue", sortValue, "dbValue", value, "mainValues", parentFiltersData);
		}
		protected virtual XVar buildControl(dynamic _param_data, dynamic _param_parentFiltersData = null)
		{
			#region default values
			if(_param_parentFiltersData as Object == null) _param_parentFiltersData = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			dynamic dataValue = null, filterInterval = null, showValue = null, totalValue = null, value = null;
			filterInterval = XVar.Clone(this.pSet.getFilterByInterval((XVar)(this.fName)));
			value = XVar.Clone(data[this.aliases[this.fName]]);
			dataValue = XVar.Clone(value);
			showValue = XVar.Clone(this.getControlCaption((XVar)(value)));
			totalValue = XVar.Clone(this.getTotalValueToShow((XVar)(data[MVCFunctions.Concat(this.fName, "TOTAL")])));
			return this.getControlHTML((XVar)(value), (XVar)(showValue), (XVar)(dataValue), (XVar)(totalValue), (XVar)(this.separator), (XVar)(parentFiltersData));
		}
		protected override XVar getControlCaption(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic intervalType = null;
			intervalType = XVar.Clone(this.pSet.getFilterByInterval((XVar)(this.fName)));
			if(XVar.Pack(!(XVar)(intervalType)))
			{
				return this.getValueToShow((XVar)(value));
			}
			return this.pageObject.formatGroupValue((XVar)(this.fName), (XVar)(intervalType), (XVar)(value));
		}
		protected override XVar isTruncated()
		{
			return (XVar)(!(XVar)(this.dependent))  && (XVar)(this.truncated);
		}
		protected override XVar getExtraDataAttrs(dynamic _param_parentFiltersData)
		{
			#region pass-by-value parameters
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			if((XVar)(!(XVar)(this.dependent))  || (XVar)(XVar.Equals(XVar.Pack(parentFiltersData), XVar.Pack(null))))
			{
				return "";
			}
			return MVCFunctions.Concat(" data-parent-filters-values=\"", MVCFunctions.runner_htmlspecialchars((XVar)(MVCFunctions.runner_json_encode((XVar)(parentFiltersData)))), "\" ");
		}
		protected override XVar getCheckedAttr(dynamic _param_value, dynamic _param_parentFiltersData = null)
		{
			#region default values
			if(_param_parentFiltersData as Object == null) _param_parentFiltersData = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			#endregion

			dynamic filteredValues = null;
			filteredValues = XVar.Clone(this.filteredFields[this.fName]["values"]);
			if((XVar)(this.multiSelect == Constants.FM_NONE)  || (XVar)((XVar)(this.filtered)  && (XVar)(!(XVar)(MVCFunctions.in_array((XVar)(value), (XVar)(filteredValues))))))
			{
				return "";
			}
			if((XVar)((XVar)((XVar)(this.filtered)  && (XVar)(this.dependent))  && (XVar)(this.multiSelect == Constants.FM_ON_DEMAND))  && (XVar)(MVCFunctions.count(filteredValues) == 1))
			{
				return "checked=\"checked\"";
			}
			if((XVar)((XVar)(!(XVar)(this.filtered))  || (XVar)(!(XVar)(this.dependent)))  || (XVar)(XVar.Equals(XVar.Pack(parentFiltersData), XVar.Pack(null))))
			{
				return "checked=\"checked\"";
			}
			foreach (KeyValuePair<XVar, dynamic> parentValues in this.filteredFields[this.fName]["parentValues"].GetEnumerator())
			{
				if((XVar)(value == this.filteredFields[this.fName]["values"][parentValues.Key])  && (XVar)(this.isParentsValuesDataTheSame((XVar)(parentFiltersData), (XVar)(parentValues.Value))))
				{
					return "checked=\"checked\"";
				}
			}
			return "";
		}
		protected virtual XVar isParentsValuesDataTheSame(dynamic _param_parentFiltersData, dynamic _param_parentValues)
		{
			#region pass-by-value parameters
			dynamic parentFiltersData = XVar.Clone(_param_parentFiltersData);
			dynamic parentValues = XVar.Clone(_param_parentValues);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> pName in this.parentFiltersNames.GetEnumerator())
			{
				if(parentFiltersData[pName.Value] != parentValues[pName.Key])
				{
					return false;
				}
			}
			return true;
		}
		protected override XVar getBaseContolsMapParams()
		{
			dynamic ctrlsMap = XVar.Array();
			ctrlsMap = XVar.Clone(base.getBaseContolsMapParams());
			if(XVar.Pack(this.dependent))
			{
				ctrlsMap.InitAndSetArrayItem(true, "dependent");
				ctrlsMap.InitAndSetArrayItem(this.parentFiltersNames, "parentFilterNames");
				ctrlsMap.InitAndSetArrayItem(MVCFunctions.GoodFieldName((XVar)(this.parentFilterName)), "goodParentName");
				ctrlsMap.InitAndSetArrayItem(MVCFunctions.GoodFieldName((XVar)(this.parentFiltersNames[MVCFunctions.count(this.parentFiltersNames) - 1])), "goodOutermostParentName");
			}
			if(XVar.Pack(this.hasDependent))
			{
				ctrlsMap.InitAndSetArrayItem(true, "hasDependent");
				ctrlsMap.InitAndSetArrayItem(this.dependentFilterNames, "dependentFilterNames");
			}
			return ctrlsMap;
		}
		protected override XVar getDelButtonHtml(dynamic _param_gfName, dynamic _param_id, dynamic _param_deleteValue)
		{
			#region pass-by-value parameters
			dynamic gfName = XVar.Clone(_param_gfName);
			dynamic id = XVar.Clone(_param_id);
			dynamic deleteValue = XVar.Clone(_param_deleteValue);
			#endregion

			if((XVar)(this.multiSelect == Constants.FM_ALWAYS)  || (XVar)(this.dependent))
			{
				return "";
			}
			return base.getDelButtonHtml((XVar)(gfName), (XVar)(id), (XVar)(deleteValue));
		}
		protected override XVar getNumberOfExtraItemsToShow()
		{
			return this.numberOfExtraItemsToShow;
		}
		protected override XVar extraBlocksProcessing(ref dynamic filterCtrlBlocks)
		{
			dynamic hiddenItemsCounter = null, visbleItemsCounter = null;
			base.extraBlocksProcessing(ref filterCtrlBlocks);
			if((XVar)(!(XVar)(this.numberOfVisibleItems))  || (XVar)(this.dependent))
			{
				return null;
			}
			visbleItemsCounter = XVar.Clone(hiddenItemsCounter = new XVar(0));
			foreach (KeyValuePair<XVar, dynamic> block in filterCtrlBlocks.GetEnumerator())
			{
				dynamic visible = null;
				visible = XVar.Clone(XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(block.Value[MVCFunctions.Concat("visibilityClass_", this.gfName)]), (XVar)(this.onDemandHiddenItemClassName))), XVar.Pack(false)));
				if(XVar.Pack(visible))
				{
					visbleItemsCounter = XVar.Clone(visbleItemsCounter + 1);
				}
				else
				{
					hiddenItemsCounter = XVar.Clone(hiddenItemsCounter + 1);
				}
				if((XVar)((XVar)(visible)  && (XVar)(this.numberOfVisibleItems < visbleItemsCounter))  || (XVar)((XVar)(!(XVar)(visible))  && (XVar)(this.numberOfVisibleItems < hiddenItemsCounter)))
				{
					filterCtrlBlocks[block.Key][MVCFunctions.Concat("visibilityClass_", this.gfName)] = MVCFunctions.Concat(filterCtrlBlocks[block.Key][MVCFunctions.Concat("visibilityClass_", this.gfName)], " ", this.hiddenExtraItemClassName);
				}
			}
			if((XVar)(this.filtered)  && (XVar)(this.multiSelect == Constants.FM_ON_DEMAND))
			{
				if((XVar)(MVCFunctions.count(this.filteredFields[this.fName]["values"]) < this.numberOfVisibleItems)  && (XVar)(this.numberOfVisibleItems < hiddenItemsCounter))
				{
					this.truncated = new XVar(true);
					this.hideShowMore = new XVar(true);
					this.numberOfExtraItemsToShow = XVar.Clone(hiddenItemsCounter - this.numberOfVisibleItems);
				}
			}
			else
			{
				if(this.numberOfVisibleItems < visbleItemsCounter)
				{
					this.truncated = new XVar(true);
					this.numberOfExtraItemsToShow = XVar.Clone(visbleItemsCounter - this.numberOfVisibleItems);
				}
			}

			return null;
		}
		protected override XVar isShowMoreHidden()
		{
			return this.hideShowMore;
		}
		protected override XVar sortFilterBlocks(ref dynamic filterCtrlBlocks)
		{
			dynamic compareFunction = null;
			if(this.sortingType != Constants.SORT_BY_DISP_VALUE)
			{
				return null;
			}
			compareFunction = new XVar("compareBlocksByStringValues");
			MVCFunctions.usort((XVar)(filterCtrlBlocks), (XVar)(new XVar(0, "FilterControl", 1, compareFunction)));
			if(XVar.Pack(this.isDescendingSortOrder))
			{
				filterCtrlBlocks = XVar.Clone(MVCFunctions.array_reverse((XVar)(filterCtrlBlocks)));
			}

			return null;
		}
		public override XVar buildFilterCtrlBlockArray(dynamic _param_pageObj, dynamic _param_dFilterBlocks = null)
		{
			#region default values
			if(_param_dFilterBlocks as Object == null) _param_dFilterBlocks = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic pageObj = XVar.Clone(_param_pageObj);
			dynamic dFilterBlocks = XVar.Clone(_param_dFilterBlocks);
			#endregion

			dynamic filterBlocks = null;
			filterBlocks = XVar.Clone(base.buildFilterCtrlBlockArray((XVar)(pageObj)));
			if((XVar)(!(XVar)(this.hasDependent))  || (XVar)(XVar.Equals(XVar.Pack(dFilterBlocks), XVar.Pack(null))))
			{
				return filterBlocks;
			}
			return this.getCtrlBlocksMergeWithDependentFilterBlocks((XVar)(filterBlocks), (XVar)(dFilterBlocks));
		}
		protected virtual XVar getCtrlBlocksMergeWithDependentFilterBlocks(dynamic _param_mFilterBlocks, dynamic _param_dFilterBlocks)
		{
			#region pass-by-value parameters
			dynamic mFilterBlocks = XVar.Clone(_param_mFilterBlocks);
			dynamic dFilterBlocks = XVar.Clone(_param_dFilterBlocks);
			#endregion

			dynamic dBlockName = null, dgName = null, numberOfdItemsToShow = null, showMoreBlockName = null;
			dgName = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(this.dependentFilterName)));
			dBlockName = XVar.Clone(MVCFunctions.Concat("filterCtrlBlock_", dgName));
			showMoreBlockName = XVar.Clone(MVCFunctions.Concat("filter_button_showmore_", dgName));
			numberOfdItemsToShow = XVar.Clone(this.pSet.getNumberOfVisibleFilterItems((XVar)(this.dependentFilterName)));
			foreach (KeyValuePair<XVar, dynamic> block in mFilterBlocks.GetEnumerator())
			{
				dynamic invisibleItemsCounter = null, mMainValues = XVar.Array(), visibleItemsCounter = null;
				mMainValues = XVar.Clone(block.Value["mainValues"]);
				visibleItemsCounter = new XVar(0);
				invisibleItemsCounter = new XVar(0);
				foreach (KeyValuePair<XVar, dynamic> dBlock in dFilterBlocks.GetEnumerator())
				{
					dynamic addDependentBlock = null, dMainValues = XVar.Array();
					dMainValues = XVar.Clone(dBlock.Value["mainValues"]);
					if(dMainValues[this.fName] != block.Value["dbValue"])
					{
						continue;
					}
					addDependentBlock = new XVar(true);
					foreach (KeyValuePair<XVar, dynamic> value in mMainValues.GetEnumerator())
					{
						if(mMainValues[value.Key] != dMainValues[value.Key])
						{
							addDependentBlock = new XVar(false);
							break;
						}
					}
					if(XVar.Pack(addDependentBlock))
					{
						if(XVar.Pack(numberOfdItemsToShow))
						{
							dynamic visible = null;
							visible = XVar.Clone(dBlock.Value[MVCFunctions.Concat("visibilityClass_", dgName)] != this.onDemandHiddenItemClassName);
							if(XVar.Pack(visible))
							{
								visibleItemsCounter = XVar.Clone(visibleItemsCounter + 1);
							}
							else
							{
								invisibleItemsCounter = XVar.Clone(invisibleItemsCounter + 1);
							}
							if((XVar)((XVar)(visible)  && (XVar)(numberOfdItemsToShow < visibleItemsCounter))  || (XVar)((XVar)(!(XVar)(visible))  && (XVar)(numberOfdItemsToShow < invisibleItemsCounter)))
							{
								dBlock.Value.InitAndSetArrayItem(this.hiddenExtraItemClassName, MVCFunctions.Concat("visibilityClass_", dgName));
							}
						}
						mFilterBlocks.InitAndSetArrayItem(dBlock.Value, block.Key, dBlockName, "data", null);
					}
					mFilterBlocks.InitAndSetArrayItem(MVCFunctions.str_replace(new XVar("%n%"), (XVar)(visibleItemsCounter - numberOfdItemsToShow), (XVar)(CommonFunctions.mlang_message(new XVar("SHOW_N_MORE")))), block.Key, MVCFunctions.Concat("show_n_more_", dgName));
					mFilterBlocks.InitAndSetArrayItem((XVar)(numberOfdItemsToShow)  && (XVar)(numberOfdItemsToShow < visibleItemsCounter), block.Key, showMoreBlockName);
				}
			}
			return mFilterBlocks;
		}
		public override XVar getFilterButtonParams(dynamic _param_dBtnParams = null)
		{
			#region default values
			if(_param_dBtnParams as Object == null) _param_dBtnParams = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic dBtnParams = XVar.Clone(_param_dBtnParams);
			#endregion

			dynamic mBtnParams = XVar.Array();
			mBtnParams = XVar.Clone(base.getFilterButtonParams());
			if((XVar)(this.hasDependent)  && (XVar)(!(XVar)(XVar.Equals(XVar.Pack(dBtnParams), XVar.Pack(null)))))
			{
				mBtnParams.InitAndSetArrayItem((XVar)(mBtnParams["hasMultiselectBtn"])  || (XVar)(dBtnParams["hasMultiselectBtn"]), "hasMultiselectBtn");
				mBtnParams.InitAndSetArrayItem((XVar)(mBtnParams["hasApplyBtn"])  || (XVar)(dBtnParams["hasApplyBtn"]), "hasApplyBtn");
			}
			return mBtnParams;
		}
		public override XVar getFilterExtraControls(dynamic _param_dExtraCtrls = null)
		{
			#region default values
			if(_param_dExtraCtrls as Object == null) _param_dExtraCtrls = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic dExtraCtrls = XVar.Clone(_param_dExtraCtrls);
			#endregion

			dynamic mExtraCtrls = XVar.Array();
			mExtraCtrls = XVar.Clone(base.getFilterExtraControls());
			if((XVar)(!(XVar)(this.hasDependent))  || (XVar)(XVar.Equals(XVar.Pack(dExtraCtrls), XVar.Pack(null))))
			{
				return mExtraCtrls;
			}
			if((XVar)(!(XVar)(mExtraCtrls["selectAllAttrs"]))  || (XVar)((XVar)(this.multiSelect == Constants.FM_ON_DEMAND)  && (XVar)(dExtraCtrls["selectAllAttrs"])))
			{
				mExtraCtrls.InitAndSetArrayItem(dExtraCtrls["selectAllAttrs"], "selectAllAttrs");
			}
			if(XVar.Pack(!(XVar)(mExtraCtrls["filtered"])))
			{
				mExtraCtrls.InitAndSetArrayItem(dExtraCtrls["filtered"], "filtered");
			}
			return mExtraCtrls;
		}
		public static XVar getFilterCondition(dynamic _param_fName, dynamic _param_value, dynamic _param_pSet_packed)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic value = XVar.Clone(_param_value);
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			#endregion

			if(XVar.Pack(pSet.multiSelectLookupEdit((XVar)(fName))))
			{
				return FilterMultiselectLookup.getFilterCondition((XVar)(fName), (XVar)(value), (XVar)(pSet));
			}
			return DataCondition.FieldEquals((XVar)(fName), (XVar)(value), (XVar)(pSet.getFilterByInterval((XVar)(fName))));
		}
	}
}
