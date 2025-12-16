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
		public XVar fulltext()
		{
			try
			{
				dynamic _connection = null, cViewControl = null, container = null, data = XVar.Array(), dataSource = null, dc = null, field = null, fieldValue = null, htmlEncodedValue = null, keys = XVar.Array(), keysArr = XVar.Array(), lookup = null, mainField = null, mainTable = null, mode = null, pageName = null, pageType = null, qResult = null, returnJSON = null, searchClauseObj = null, sessionPrefix = null, shortTable = null;
				ProjectSettings pSet;
				mode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
				shortTable = XVar.Clone(MVCFunctions.postvalue(new XVar("table")));
				field = XVar.Clone(MVCFunctions.postvalue(new XVar("field")));
				pageType = XVar.Clone(MVCFunctions.postvalue(new XVar("pagetype")));
				pageName = XVar.Clone(MVCFunctions.postvalue(new XVar("page")));
				mainTable = XVar.Clone(MVCFunctions.postvalue(new XVar("maintable")));
				mainField = XVar.Clone(MVCFunctions.postvalue(new XVar("mainfield")));
				lookup = new XVar(false);
				if((XVar)(mainTable)  && (XVar)(mainField))
				{
					lookup = new XVar(true);
				}
				GlobalVars.table = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(shortTable)));
				if(XVar.Pack(!(XVar)(GlobalVars.table)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				if(XVar.Pack(!(XVar)(Security.userHasFieldPermissions((XVar)(GlobalVars.table), (XVar)(field), (XVar)(pageType), (XVar)(pageName), new XVar(false)))))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), (XVar)(pageType), (XVar)(pageName)));
				GlobalVars.cipherer = XVar.Clone(new RunnerCipherer((XVar)(GlobalVars.table), (XVar)(pSet)));
				_connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(GlobalVars.table)));
				keysArr = XVar.Clone(pSet.getTableKeys());
				keys = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> k in keysArr.GetEnumerator())
				{
					keys.InitAndSetArrayItem(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("key", k.Key + 1))), k.Value);
				}
				dc = XVar.Clone(new DsCommand());
				dc.filter = XVar.Clone(Security.SelectCondition(new XVar("S"), (XVar)(pSet)));
				dc.keys = XVar.Clone(keys);
				dataSource = XVar.Clone(CommonFunctions.getDataSource((XVar)(GlobalVars.table), (XVar)(pSet), (XVar)(_connection)));
				qResult = XVar.Clone(dataSource.getSingle((XVar)(dc)));
				if((XVar)(!(XVar)(qResult))  || (XVar)(!(XVar)(data = XVar.Clone(GlobalVars.cipherer.DecryptFetchedArray((XVar)(qResult.fetchAssoc()))))))
				{
					returnJSON = XVar.Clone(new XVar("success", false, "error", "Error: Wrong SQL query"));
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				fieldValue = XVar.Clone(data[field]);
				sessionPrefix = XVar.Clone(pSet.getOriginalTableName());
				if(mode == Constants.LIST_DASHBOARD)
				{
					sessionPrefix = XVar.Clone(MVCFunctions.Concat("Dashboard_", pSet.getOriginalTableName()));
				}
				if(XVar.Pack(lookup))
				{
					sessionPrefix = XVar.Clone(MVCFunctions.Concat(pSet.getOriginalTableName(), "_lookup_", mainTable, "_", mainField));
				}
				searchClauseObj = XVar.Clone(SearchClause.UnserializeObject((XVar)(XSession.Session[MVCFunctions.Concat(sessionPrefix, "_advsearch")])));
				container = XVar.Clone(new ViewControlsContainer((XVar)(pSet), new XVar(Constants.PAGE_LIST), new XVar(null)));
				cViewControl = XVar.Clone(container.getControl((XVar)(field)));
				if((XVar)(cViewControl.localControlsContainer)  && (XVar)(!(XVar)(cViewControl.linkAndDisplaySame)))
				{
					cViewControl.localControlsContainer.fullText = new XVar(true);
				}
				else
				{
					cViewControl.container.fullText = new XVar(true);
				}
				if(XVar.Pack(searchClauseObj))
				{
					dynamic useViewControl = null;
					if((XVar)(searchClauseObj.searchStarted())  || (XVar)(useViewControl))
					{
						cViewControl.searchClauseObj = XVar.Clone(searchClauseObj);
						cViewControl.searchHighlight = new XVar(true);
					}
				}
				htmlEncodedValue = XVar.Clone(cViewControl.showDBValue((XVar)(data), new XVar("")));
				returnJSON = XVar.Clone(new XVar("success", true, "textCont", htmlEncodedValue));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
