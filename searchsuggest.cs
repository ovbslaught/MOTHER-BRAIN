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
		public XVar searchsuggest()
		{
			try
			{
				dynamic allSearchFields = null, conditions = XVar.Array(), dashSearchFields = XVar.Array(), dashSettings = null, detailKeys = null, forDashboardSimpleSearch = null, forLookupPage = null, masterTable = null, numberOfSuggests = null, pSetList = null, page = null, pageType = null, parentCtrlsData = XVar.Array(), result = XVar.Array(), returnJSON = XVar.Array(), searchClauseObj = null, searchField = null, searchFor = null, searchOpt = null, shortTable = null, whereClauses = null;
				ProjectSettings pSet;
				CommonFunctions.add_nocache_headers();
				shortTable = XVar.Clone(MVCFunctions.postvalue(new XVar("table")));
				GlobalVars.table = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(shortTable)));
				if(XVar.Pack(!(XVar)(GlobalVars.table)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				page = XVar.Clone(MVCFunctions.postvalue(new XVar("page")));
				pageType = XVar.Clone(MVCFunctions.postvalue(new XVar("pageType")));
				if(XVar.Pack(!(XVar)(pageType)))
				{
					pageType = new XVar(Constants.PAGE_LIST);
				}
				searchField = XVar.Clone(MVCFunctions.postvalue(new XVar("searchField")));
				if(XVar.Pack(searchField))
				{
					if(XVar.Pack(!(XVar)(Security.userHasFieldPermissions((XVar)(GlobalVars.table), (XVar)(searchField), (XVar)(pageType), (XVar)(page), new XVar(true)))))
					{
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
				}
				searchFor = XVar.Clone(MVCFunctions.trim((XVar)(MVCFunctions.postvalue(new XVar("searchFor")))));
				if(searchFor == XVar.Pack(""))
				{
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(new XVar("success", true, "result", ""))));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				searchOpt = XVar.Clone((XVar.Pack(MVCFunctions.postvalue(new XVar("start"))) ? XVar.Pack("Starts with") : XVar.Pack("Contains")));
				searchField = XVar.Clone(MVCFunctions.GoodFieldName((XVar)(searchField)));
				numberOfSuggests = XVar.Clone(ProjectSettings.getProjectValue(new XVar("searchSuggestsNumber")));
				whereClauses = XVar.Clone(XVar.Array());
				forLookupPage = XVar.Clone(MVCFunctions.postvalue(new XVar("forLookup")));
				forDashboardSimpleSearch = XVar.Clone((XVar)(!(XVar)(searchField))  && (XVar)(pageType == Constants.PAGE_DASHBOARD));
				if(XVar.Pack(forDashboardSimpleSearch))
				{
					dynamic dashGoogleLikeFields = XVar.Array(), sfdata = XVar.Array();
					dashSettings = XVar.Clone(new ProjectSettings((XVar)(GlobalVars.table), new XVar(Constants.PAGE_DASHBOARD), (XVar)(page)));
					dashGoogleLikeFields = XVar.Clone(dashSettings.getGoogleLikeFields());
					dashSearchFields = XVar.Clone(dashSettings.getDashboardSearchFields());
					sfdata = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> g in dashGoogleLikeFields.GetEnumerator())
					{
						foreach (KeyValuePair<XVar, dynamic> data in dashSearchFields[g.Value].GetEnumerator())
						{
							sfdata.InitAndSetArrayItem(data.Value["field"], data.Value["table"], null);
						}
					}
					result = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> fields in sfdata.GetEnumerator())
					{
						dynamic _result = XVar.Array();
						if(numberOfSuggests <= MVCFunctions.count(result))
						{
							break;
						}
						if(XVar.Pack(!(XVar)(result)))
						{
							result = XVar.Clone(XVar.Array());
						}
						_result = XVar.Clone(CommonFunctions.getListOfSuggests((XVar)(fields.Value), (XVar)(fields.Key), (XVar)(numberOfSuggests - MVCFunctions.count(result)), (XVar)(searchOpt), (XVar)(searchFor)));
						foreach (KeyValuePair<XVar, dynamic> _data in _result.GetEnumerator())
						{
							dynamic found = null;
							found = new XVar(false);
							foreach (KeyValuePair<XVar, dynamic> data in result.GetEnumerator())
							{
								if(data.Value["realValue"] == _data.Value["realValue"])
								{
									found = new XVar(true);
									break;
								}
							}
							if(XVar.Pack(!(XVar)(found)))
							{
								result.InitAndSetArrayItem(_data.Value, null);
							}
						}
					}
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(new XVar("success", true, "result", result))));
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if(pageType == Constants.PAGE_DASHBOARD)
				{
					dynamic sfData = XVar.Array();
					dashSettings = XVar.Clone(new ProjectSettings((XVar)(GlobalVars.table), new XVar(Constants.PAGE_DASHBOARD), (XVar)(page)));
					dashSearchFields = XVar.Clone(dashSettings.getDashboardSearchFields());
					sfData = XVar.Clone(dashSearchFields[searchField][0]);
					searchField = XVar.Clone(sfData["field"]);
					GlobalVars.table = XVar.Clone(sfData["table"]);
					foreach (KeyValuePair<XVar, dynamic> elem in dashSettings.getDashboardElements().GetEnumerator())
					{
						if(elem.Value["table"] == GlobalVars.table)
						{
							pageType = new XVar(Constants.PAGE_LIST);
							if(elem.Value["type"] == Constants.DASHBOARD_CHART)
							{
								pageType = new XVar(Constants.PAGE_CHART);
							}
							else
							{
								if(elem.Value["type"] == Constants.DASHBOARD_REPORT)
								{
									pageType = new XVar(Constants.PAGE_REPORT);
								}
							}
							break;
						}
					}
				}
				pSetList = XVar.Clone(new ProjectSettings((XVar)(GlobalVars.table), (XVar)(pageType), (XVar)(page)));
				if(searchField == XVar.Pack(""))
				{
					allSearchFields = XVar.Clone(pSetList.getGoogleLikeFields());
				}
				else
				{
					allSearchFields = XVar.Clone(pSetList.getAllPageFields());
				}
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), new XVar(Constants.PAGE_SEARCH), (XVar)(page)));
				GlobalVars.cipherer = XVar.Clone(new RunnerCipherer((XVar)(GlobalVars.table)));
				conditions = XVar.Clone(XVar.Array());
				masterTable = XVar.Clone(MVCFunctions.postvalue(new XVar("mastertable")));
				detailKeys = XVar.Clone(XVar.Array());
				if(masterTable != XVar.Pack(""))
				{
					dynamic masterKetsReq = XVar.Array(), masterKeys = XVar.Array();
					masterKetsReq = XVar.Clone(RunnerPage.readMasterKeysFromRequest());
					masterKeys = XVar.Clone(pSet.getMasterKeys((XVar)(masterTable)));
					foreach (KeyValuePair<XVar, dynamic> field in masterKeys["detailsKeys"].GetEnumerator())
					{
						conditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(field.Value), (XVar)(masterKetsReq[field.Key + 1])), null);
					}
				}
				searchClauseObj = XVar.Clone(SearchClause.getSearchObject((XVar)(GlobalVars.table), new XVar(""), (XVar)(GlobalVars.table), (XVar)(GlobalVars.cipherer)));
				conditions.InitAndSetArrayItem(searchClauseObj.getFilterCondition((XVar)(pSet)), null);
				parentCtrlsData = XVar.Clone(MVCFunctions.runner_json_decode((XVar)(MVCFunctions.postvalue(new XVar("parentCtrlsData")))));
				if((XVar)(forLookupPage)  && (XVar)(parentCtrlsData))
				{
					dynamic mainField = null, mainPSet = null, mainPageType = null, mainTable = null, parentWhereParts = null;
					mainField = XVar.Clone(MVCFunctions.postvalue(new XVar("mainField")));
					mainTable = XVar.Clone(MVCFunctions.postvalue(new XVar("mainTable")));
					mainPageType = XVar.Clone(MVCFunctions.postvalue(new XVar("mainPageType")));
					mainPSet = XVar.Clone(new ProjectSettings((XVar)(mainTable), (XVar)(mainPageType)));
					parentWhereParts = XVar.Clone(XVar.Array());
					foreach (KeyValuePair<XVar, dynamic> cData in mainPSet.getParentFieldsData((XVar)(mainField)).GetEnumerator())
					{
						if(XVar.Pack(!(XVar)(parentCtrlsData.KeyExists(cData.Value["main"]))))
						{
							continue;
						}
						conditions.InitAndSetArrayItem(LookupField.categoryCondition((XVar)(mainPSet), (XVar)(cData.Value["main"]), (XVar)(cData.Value["lookup"]), (XVar)(parentCtrlsData[cData.Value["main"]])), null);
					}
				}
				result = XVar.Clone(CommonFunctions.getListOfSuggests((XVar)(allSearchFields), (XVar)(GlobalVars.table), (XVar)(numberOfSuggests), (XVar)(searchOpt), (XVar)(searchFor), (XVar)(searchField), (XVar)(detailKeys), (XVar)(conditions)));
				returnJSON = XVar.Clone(XVar.Array());
				returnJSON.InitAndSetArrayItem(true, "success");
				returnJSON.InitAndSetArrayItem(result, "result");
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				MVCFunctions.Echo(new XVar(""));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
