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
		public XVar checkduplicates()
		{
			try
			{
				dynamic _connection = null, data = XVar.Array(), dataSource = null, dc = null, denyChecking = null, fieldControlType = null, fieldName = null, hasDuplicates = null, pageName = null, pageType = null, qResult = null, regEmailMode = null, regUsernameMode = null, returnJSON = null, shortTableName = null, userNameField = null, value = null;
				ProjectSettings pSet;
				MVCFunctions.Header("Expires", "Thu, 01 Jan 1970 00:00:01 GMT");
				shortTableName = XVar.Clone(MVCFunctions.postvalue(new XVar("tableName")));
				GlobalVars.table = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(shortTableName)));
				if(XVar.Pack(!(XVar)(GlobalVars.table)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				pageType = XVar.Clone(MVCFunctions.postvalue(new XVar("pageType")));
				pageName = XVar.Clone(MVCFunctions.postvalue(new XVar("page")));
				fieldName = XVar.Clone(MVCFunctions.postvalue(new XVar("fieldName")));
				fieldControlType = XVar.Clone(MVCFunctions.postvalue(new XVar("fieldControlType")));
				value = XVar.Clone(MVCFunctions.postvalue(new XVar("value")));
				if(XVar.Pack(!(XVar)(Security.userHasFieldPermissions((XVar)(GlobalVars.table), (XVar)(fieldName), (XVar)(pageType), (XVar)(pageName), new XVar(true)))))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), (XVar)(pageType)));
				denyChecking = XVar.Clone(pSet.allowDuplicateValues((XVar)(fieldName)));
				regEmailMode = new XVar(false);
				regUsernameMode = new XVar(false);
				userNameField = XVar.Clone(Security.usernameField());
				if((XVar)(Security.registerPage())  && (XVar)(GlobalVars.table == Security.loginDataSource()))
				{
					regEmailMode = XVar.Clone(fieldName == Security.emailField());
					regUsernameMode = XVar.Clone(fieldName == userNameField);
					denyChecking = XVar.Clone((XVar)((XVar)(denyChecking)  && (XVar)(fieldName != userNameField))  && (XVar)(fieldName != Security.emailField()));
				}
				if(XVar.Pack(denyChecking))
				{
					returnJSON = XVar.Clone(new XVar("success", false, "error", "Duplicated values are allowed"));
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				_connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(GlobalVars.table)));
				dataSource = XVar.Clone(CommonFunctions.getDataSource((XVar)(GlobalVars.table), (XVar)(pSet), (XVar)(_connection)));
				dc = XVar.Clone(new DsCommand());
				dc.totals = XVar.Clone(XVar.Array());
				dc.totals.InitAndSetArrayItem(new XVar("total", "count", "alias", MVCFunctions.Concat("count_", fieldName), "field", fieldName), null);
				dc.filter = XVar.Clone(DataCondition.FieldEquals((XVar)(fieldName), (XVar)(value), new XVar(0), new XVar(Constants.dsCASE_DEFAULT)));
				if(XVar.Pack(regEmailMode))
				{
					dc.filter = XVar.Clone(DataCondition.FieldEquals((XVar)(fieldName), (XVar)(value), new XVar(0), new XVar(Constants.dsCASE_INSENSITIVE)));
				}
				if(XVar.Pack(regUsernameMode))
				{
					dynamic fieldSQL = null, where = null;
					where = XVar.Clone(_connection.comparisonSQL((XVar)(fieldSQL), (XVar)(value), (XVar)(Security.caseInsensitiveUsername())));
					dc.filter = XVar.Clone(DataCondition.FieldEquals((XVar)(fieldName), (XVar)(value), new XVar(0), (XVar)((XVar.Pack(Security.caseInsensitiveUsername()) ? XVar.Pack(Constants.dsCASE_INSENSITIVE) : XVar.Pack(Constants.dsCASE_STRICT)))));
				}
				qResult = XVar.Clone(dataSource.getTotals((XVar)(dc)));
				if(XVar.Pack(!(XVar)(qResult)))
				{
					returnJSON = XVar.Clone(new XVar("success", false, "error", "Error: Wrong SQL query"));
					MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				hasDuplicates = new XVar(false);
				data = XVar.Clone(qResult.fetchAssoc());
				if(XVar.Pack(data))
				{
					hasDuplicates = XVar.Clone((XVar.Pack(data[MVCFunctions.Concat("count_", fieldName)]) ? XVar.Pack(true) : XVar.Pack(false)));
				}
				returnJSON = XVar.Clone(new XVar("success", true, "hasDuplicates", hasDuplicates, "error", ""));
				MVCFunctions.Echo(CommonFunctions.printJSON((XVar)(returnJSON)));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
