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
	public partial class GlobalController : BaseController
	{
		public XVar lookupsuggest()
		{
			try
			{
				dynamic LookupType = null, contextParams = XVar.Array(), data = XVar.Array(), dataSource = null, dc = null, dcDisplay = XVar.Array(), displayFieldAlias = null, displayFieldName = null, displayedValue = null, field = null, isExistParent = null, keys = XVar.Array(), linkAndDisplaySame = null, linkField = null, linkFieldName = null, lookupField = null, lookupPSet = null, lookupTable = null, masterTable = null, multiselect = null, operation = null, pageName = null, pageType = null, parentCtrlsData = null, qResult = null, respObj = null, searchByLinkField = null, shortTable = null, value = null, values = null, var_response = XVar.Array();
				ProjectSettings pSet;
				MVCFunctions.Header("Expires", "Thu, 01 Jan 1970 00:00:01 GMT");
				shortTable = XVar.Clone(MVCFunctions.postvalue(new XVar("table")));
				GlobalVars.table = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(shortTable)));
				if(XVar.Pack(!(XVar)(GlobalVars.table)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				pageType = XVar.Clone(MVCFunctions.postvalue(new XVar("pageType")));
				pageName = XVar.Clone(MVCFunctions.postvalue(new XVar("page")));
				field = XVar.Clone(MVCFunctions.postvalue(new XVar("searchField")));
				if(XVar.Pack(!(XVar)(Security.userHasFieldPermissions((XVar)(GlobalVars.table), (XVar)(field), (XVar)(pageType), (XVar)(pageName), new XVar(true)))))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), (XVar)(pageType), (XVar)(pageName)));
				if(pSet.getEntityType() == Constants.titDASHBOARD)
				{
					dynamic dashFields = XVar.Array(), dashboard = null;
					dashboard = XVar.Clone(GlobalVars.table);
					dashFields = XVar.Clone(pSet.getDashboardSearchFields());
					GlobalVars.table = XVar.Clone(dashFields[field][0]["table"]);
					field = XVar.Clone(dashFields[field][0]["field"]);
					pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), (XVar)(pageType), (XVar)(pageName), (XVar)(dashboard)));
				}
				GlobalVars.cipherer = XVar.Clone(new RunnerCipherer((XVar)(GlobalVars.table)));
				masterTable = XVar.Clone(MVCFunctions.postvalue(new XVar("masterTable")));
				if((XVar)(masterTable != XVar.Pack(""))  && (XVar)(XSession.Session.KeyExists(MVCFunctions.Concat(masterTable, "_masterRecordData"))))
				{
					contextParams.InitAndSetArrayItem(XSession.Session[MVCFunctions.Concat(masterTable, "_masterRecordData")], "masterData");
				}
				contextParams.InitAndSetArrayItem(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("data")))), "data");
				RunnerContext.push((XVar)(new RunnerContextItem((XVar)(pageType), (XVar)(contextParams))));
				isExistParent = XVar.Clone(MVCFunctions.postvalue(new XVar("isExistParent")));
				searchByLinkField = XVar.Clone(MVCFunctions.postvalue(new XVar("searchByLinkField")));
				parentCtrlsData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("parentCtrlsData")))));
				value = XVar.Clone(MVCFunctions.postvalue(new XVar("searchFor")));
				values = XVar.Clone((XVar.Pack(MVCFunctions.postvalue(new XVar("multiselection"))) ? XVar.Pack(CommonFunctions.splitLookupValues((XVar)(value))) : XVar.Pack(new XVar(0, value))));
				lookupField = new XVar("");
				if(pSet.getEditFormat((XVar)(field)) == Constants.EDIT_FORMAT_LOOKUP_WIZARD)
				{
					LookupType = XVar.Clone(pSet.getLookupType((XVar)(field)));
					if((XVar)(LookupType == Constants.LT_LOOKUPTABLE)  || (XVar)(LookupType == Constants.LT_QUERY))
					{
						lookupField = XVar.Clone(field);
					}
				}
				if(XVar.Pack(!(XVar)(lookupField)))
				{
					respObj = XVar.Clone(new XVar("success", false, "data", XVar.Array()));
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(respObj)));
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				lookupTable = XVar.Clone(pSet.getLookupTable((XVar)(lookupField)));
				lookupPSet = new XVar(null);
				if(pSet.getLookupType((XVar)(field)) == Constants.LT_QUERY)
				{
					lookupPSet = XVar.Clone(new ProjectSettings((XVar)(lookupTable)));
				}
				linkFieldName = XVar.Clone(pSet.getLinkField((XVar)(lookupField)));
				displayFieldName = XVar.Clone(pSet.getDisplayField((XVar)(field)));
				linkAndDisplaySame = XVar.Clone(displayFieldName == linkFieldName);
				dcDisplay = XVar.Clone(LookupField.makeLookupDataCommand((XVar)(field), (XVar)(pSet), (XVar)(parentCtrlsData), (XVar)(value), new XVar(true), (XVar)(searchByLinkField), new XVar(true), (XVar)(searchByLinkField)));
				dc = XVar.Clone(dcDisplay["dc"]);
				displayFieldAlias = XVar.Clone(dcDisplay["displayField"]);
				operation = XVar.Clone((XVar.Pack(GlobalVars.ajaxSearchStartsWith) ? XVar.Pack(Constants.dsopSTART) : XVar.Pack(Constants.dsopCONTAIN)));
				if(XVar.Pack(!(XVar)(searchByLinkField)))
				{
					dynamic displayFieldCondition = null;
					displayFieldCondition = XVar.Clone((XVar.Pack(pSet.getCustomDisplay((XVar)(field))) ? XVar.Pack(DataCondition.SQLIs((XVar)(displayFieldName), (XVar)(operation), (XVar)(value))) : XVar.Pack(DataCondition.FieldIs((XVar)(displayFieldName), (XVar)(operation), (XVar)(value)))));
					dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, displayFieldCondition))));
				}
				dc.reccount = new XVar(200);
				dataSource = XVar.Clone(CommonFunctions.getLookupDataSource((XVar)(lookupField), (XVar)(pSet)));
				qResult = XVar.Clone(dataSource.getList((XVar)(dc)));
				if(XVar.Pack(!(XVar)(qResult)))
				{
					MVCFunctions.showError((XVar)(dataSource.lastError()));
				}
				multiselect = XVar.Clone(pSet.multiSelect((XVar)(lookupField)));
				linkField = XVar.Clone(pSet.getLinkField((XVar)(field)));
				var_response = XVar.Clone(XVar.Array());
				while(XVar.Pack(data = XVar.Clone(qResult.fetchAssoc())))
				{
					if((XVar)(LookupType == Constants.LT_QUERY)  && (XVar)(pSet.isLookupUnique((XVar)(lookupField))))
					{
						dynamic uniqueArray = XVar.Array();
						if(XVar.Pack(!(XVar)(uniqueArray as object != null)))
						{
							uniqueArray = XVar.Clone(XVar.Array());
						}
						if(XVar.Pack(MVCFunctions.in_array((XVar)(data[displayFieldAlias]), (XVar)(uniqueArray))))
						{
							continue;
						}
						uniqueArray.InitAndSetArrayItem(data[displayFieldAlias], null);
					}
					data.InitAndSetArrayItem(GlobalVars.cipherer.DecryptField((XVar)(lookupField), (XVar)(data[linkField])), linkField);
					if(LookupType == Constants.LT_QUERY)
					{
						data.InitAndSetArrayItem(GlobalVars.cipherer.DecryptField((XVar)(displayFieldName), (XVar)(data[displayFieldAlias])), displayFieldAlias);
					}
					displayedValue = XVar.Clone(data[displayFieldAlias]);
					if(XVar.Pack(MVCFunctions.in_array((XVar)(pSet.getViewFormat((XVar)(lookupField))), (XVar)(new XVar(0, Constants.FORMAT_DATE_SHORT, 1, Constants.FORMAT_DATE_LONG, 2, Constants.FORMAT_DATE_TIME)))))
					{
						dynamic ctrlData = XVar.Array(), viewContainer = null;
						ctrlData = XVar.Clone(XVar.Array());
						ctrlData.InitAndSetArrayItem(data[linkField], lookupField);
						viewContainer = XVar.Clone(new ViewControlsContainer((XVar)(pSet), new XVar(Constants.PAGE_LIST), new XVar(null)));
						displayedValue = XVar.Clone(viewContainer.getControl((XVar)(lookupField)).getTextValue((XVar)(ctrlData)));
					}
					keys = XVar.Clone(XVar.Array());
					if(XVar.Pack(lookupPSet))
					{
						foreach (KeyValuePair<XVar, dynamic> kf in lookupPSet.getTableKeys().GetEnumerator())
						{
							keys.InitAndSetArrayItem(data[kf.Value], null);
						}
					}
					var_response.InitAndSetArrayItem(new XVar("link", data[linkField], "display", displayedValue, "displayEscaped", MVCFunctions.runner_htmlspecialchars((XVar)(displayedValue)), "keys", keys), null);
				}
				respObj = XVar.Clone(new XVar("success", true, "data", MVCFunctions.array_slice((XVar)(var_response), new XVar(0), (XVar)(GlobalVars.runnerProjectSettings["lookupSuggestsNumber"]))));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(respObj)));
				MVCFunctions.Echo(new XVar(""));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
