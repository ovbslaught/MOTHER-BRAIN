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
	public partial class SearchPageDash : SearchPage
	{
		protected static bool skipSearchPageDashCtor = false;
		public SearchPageDash(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipSearchPageDashCtor)
			{
				skipSearchPageDashCtor = false;
				return;
			}
			if(this.mode == Constants.SEARCH_DASHBOARD)
			{
				this.pageData.InitAndSetArrayItem(true, "isDashSearchPage");
			}
		}
		protected override XVar assignSessionPrefix()
		{
			this.sessionPrefix = XVar.Clone(this.tName);

			return null;
		}
		protected virtual XVar getTableSettings(dynamic _param_table)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			#endregion

			if(XVar.Pack(!(XVar)(this.tableSettings.KeyExists(table))))
			{
				dynamic tableSettings = XVar.Array();
				this.tableSettings.InitAndSetArrayItem(new ProjectSettings((XVar)(tableSettings[table]), new XVar(Constants.PAGE_SEARCH)), table);
			}
			return this.tableSettings[table];
		}
		protected override XVar buildJsFieldSettings(dynamic _param_pSet_packed, dynamic _param_field, dynamic _param_pageType)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic field = XVar.Clone(_param_field);
			dynamic pageType = XVar.Clone(_param_pageType);
			#endregion

			dynamic editFormat = null, settings = XVar.Array();
			settings = XVar.Clone(base.buildJsFieldSettings((XVar)(pSet), (XVar)(field), (XVar)(pageType)));
			editFormat = XVar.Clone(pSet.getEditFormat((XVar)(field)));
			if(XVar.Equals(XVar.Pack(editFormat), XVar.Pack(Constants.EDIT_FORMAT_LOOKUP_WIZARD)))
			{
				dynamic fData = XVar.Array(), parentData = XVar.Array(), tableFieldName = null;
				parentData = XVar.Clone(pSet.getLookupParentFNames((XVar)(tableFieldName)));
				foreach (KeyValuePair<XVar, dynamic> parentField in fData.GetEnumerator())
				{
					parentData.InitAndSetArrayItem(this.locateDashFieldByOriginal((XVar)(pSet.table()), (XVar)(parentField.Value)), parentField.Key);
				}
				settings.InitAndSetArrayItem(parentData, "parentFields");
			}
			return settings;
		}
		protected override XVar prepareFields()
		{
			dynamic pageFields = null;
			pageFields = XVar.Clone(this.pSet.getPageFields());
			foreach (KeyValuePair<XVar, dynamic> fdata in this.pSet.getDashboardSearchFields().GetEnumerator())
			{
				dynamic ctrlBlockArr = XVar.Array(), ctrlInd = null, fSet = null, field = null, firstFieldParams = XVar.Array(), isFieldNeedSecCtrl = null, srchFields = XVar.Array(), srchTypeFull = null, table = null;
				if(XVar.Equals(XVar.Pack(MVCFunctions.array_search((XVar)(fdata.Key), (XVar)(pageFields))), XVar.Pack(false)))
				{
					continue;
				}
				field = XVar.Clone(fdata.Value[0]["field"]);
				table = XVar.Clone(fdata.Value[0]["table"]);
				fSet = XVar.Clone(this.getTableSettings((XVar)(table)));
				srchFields = XVar.Clone(this.searchClauseObj.getSearchCtrlParams((XVar)(fdata.Key)));
				firstFieldParams = XVar.Clone(XVar.Array());
				if(XVar.Pack(MVCFunctions.count(srchFields)))
				{
					firstFieldParams = XVar.Clone(srchFields[0]);
				}
				else
				{
					firstFieldParams.InitAndSetArrayItem(fdata.Key, "fName");
					firstFieldParams.InitAndSetArrayItem("", "eType");
					firstFieldParams.InitAndSetArrayItem(this.pSet.getSearchDefaultValue((XVar)(field)), "value1");
					firstFieldParams.InitAndSetArrayItem("", "value2");
					firstFieldParams.InitAndSetArrayItem(false, "not");
					firstFieldParams.InitAndSetArrayItem(this.pSet.getDefaultSearchOption((XVar)(fdata.Key)), "opt");
					firstFieldParams.InitAndSetArrayItem(false, "not");
					if(MVCFunctions.substr((XVar)(firstFieldParams["opt"]), new XVar(0), new XVar(4)) == "NOT ")
					{
						firstFieldParams.InitAndSetArrayItem(MVCFunctions.substr((XVar)(firstFieldParams["opt"]), new XVar(4)), "opt");
						firstFieldParams.InitAndSetArrayItem(true, "not");
					}
				}
				ctrlBlockArr = XVar.Clone(this.searchControlBuilder.buildSearchCtrlBlockArr((XVar)(this.id), (XVar)(firstFieldParams["fName"]), new XVar(0), (XVar)(firstFieldParams["opt"]), (XVar)(firstFieldParams["not"]), new XVar(false), (XVar)(firstFieldParams["value1"]), (XVar)(firstFieldParams["value2"])));
				if(firstFieldParams["opt"] == "")
				{
					firstFieldParams.InitAndSetArrayItem(this.pSet.getDefaultSearchOption((XVar)(firstFieldParams["fName"])), "opt");
				}
				srchTypeFull = XVar.Clone(this.searchControlBuilder.getCtrlSearchType((XVar)(firstFieldParams["fName"]), (XVar)(this.id), new XVar(0), (XVar)(firstFieldParams["opt"]), (XVar)(firstFieldParams["not"]), new XVar(true), new XVar(true)));
				if(XVar.Pack(CommonFunctions.isEnableSection508()))
				{
					this.xt.assign_section((XVar)(MVCFunctions.Concat(fdata.Key, "_label")), (XVar)(MVCFunctions.Concat("<label for=\"", this.getInputElementId((XVar)(field), (XVar)(fSet)), "\">")), new XVar("</label>"));
				}
				else
				{
					this.xt.assign((XVar)(MVCFunctions.Concat(fdata.Key, "_label")), new XVar(true));
				}
				this.xt.assign((XVar)(MVCFunctions.Concat(fdata.Key, "_fieldblock")), new XVar(true));
				this.xt.assignbyref((XVar)(MVCFunctions.Concat(fdata.Key, "_editcontrol")), (XVar)(ctrlBlockArr["searchcontrol"]));
				this.xt.assign((XVar)(MVCFunctions.Concat(fdata.Key, "_notbox")), (XVar)(ctrlBlockArr["notbox"]));
				this.xt.assignbyref((XVar)(MVCFunctions.Concat(fdata.Key, "_editcontrol1")), (XVar)(ctrlBlockArr["searchcontrol1"]));
				this.xt.assign((XVar)(MVCFunctions.Concat("searchtype_", fdata.Key)), (XVar)(ctrlBlockArr["searchtype"]));
				this.xt.assign((XVar)(MVCFunctions.Concat("searchtypefull_", fdata.Key)), (XVar)(srchTypeFull));
				isFieldNeedSecCtrl = XVar.Clone(this.searchControlBuilder.isNeedSecondCtrl((XVar)(fdata.Key)));
				ctrlInd = new XVar(0);
				if(XVar.Pack(isFieldNeedSecCtrl))
				{
					this.controlsMap.InitAndSetArrayItem(new XVar("fName", fdata.Key, "recId", this.id, "ctrlsMap", new XVar(0, ctrlInd, 1, ctrlInd + 1)), "search", "searchBlocks", null);
					ctrlInd += 2;
				}
				else
				{
					this.controlsMap.InitAndSetArrayItem(new XVar("fName", fdata.Key, "recId", this.id, "ctrlsMap", new XVar(0, ctrlInd)), "search", "searchBlocks", null);
					ctrlInd++;
				}
			}

			return null;
		}
		public virtual XVar locateDashFieldByOriginal(dynamic _param_table, dynamic _param_field)
		{
			#region pass-by-value parameters
			dynamic table = XVar.Clone(_param_table);
			dynamic field = XVar.Clone(_param_field);
			#endregion

			dynamic fname = null;
			foreach (KeyValuePair<XVar, dynamic> data in this.pSet.getDashboardSearchFields().GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(data.Value)))
				{
					continue;
				}
				if((XVar)(data.Value[0]["table"] == table)  && (XVar)(data.Value[0]["field"] == field))
				{
					return data.Key;
				}
			}
			return fname;
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

			dynamic dashSearchFields = XVar.Array(), settings = XVar.Array(), tableSettingsFilled = XVar.Array();
			settings = XVar.Clone(base.buildJsTableSettings((XVar)(table), (XVar)(pSet)));
			if(pSet.table() != this.pSet.table())
			{
				return settings;
			}
			tableSettingsFilled = XVar.Clone(XVar.Array());
			tableSettingsFilled.InitAndSetArrayItem(true, this.tName);
			dashSearchFields = XVar.Clone(this.pSet.getDashboardSearchFields());
			settings.InitAndSetArrayItem(XVar.Array(), "fieldSettings");
			foreach (KeyValuePair<XVar, dynamic> fieldName in this.pSet.getAllSearchFields().GetEnumerator())
			{
				dynamic pageType = null, tableFieldName = null, tableName = null;
				tableName = XVar.Clone(dashSearchFields[fieldName.Value][0]["table"]);
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(tableName), (XVar)(pageType)));
				tableFieldName = XVar.Clone(dashSearchFields[fieldName.Value][0]["field"]);
				if(XVar.Pack(!(XVar)(tableSettingsFilled[tableName])))
				{
					tableSettingsFilled.InitAndSetArrayItem(true, tableName);
					this.fillTableSettings((XVar)(tableName), (XVar)(pSet));
				}
				settings.InitAndSetArrayItem(new XVar(this.pageType, this.buildJsFieldSettings((XVar)(pSet), (XVar)(tableFieldName), (XVar)(pageType))), "fieldSettings", fieldName.Value);
			}
			return settings;
		}
		protected override XVar buildJsGlobalSettings()
		{
			dynamic settings = XVar.Array();
			settings = XVar.Clone(base.buildJsGlobalSettings());
			foreach (KeyValuePair<XVar, dynamic> fdata in this.pSet.getDashboardSearchFields().GetEnumerator())
			{
				dynamic fSet = null, field = null, lookupTable = null, table = null;
				field = XVar.Clone(fdata.Value[0]["field"]);
				table = XVar.Clone(fdata.Value[0]["table"]);
				fSet = XVar.Clone(this.getTableSettings((XVar)(table)));
				lookupTable = XVar.Clone(fSet.getLookupTable((XVar)(field)));
				if(XVar.Pack(lookupTable))
				{
					settings.InitAndSetArrayItem(CommonFunctions.GetTableURL((XVar)(lookupTable)), "shortTNames", lookupTable);
				}
			}
			return settings;
		}
	}
}
