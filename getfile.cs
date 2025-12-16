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
		public XVar getfile()
		{
			try
			{
				dynamic _connection = null, ctype = null, data = XVar.Array(), dataSource = null, dc = null, ext = null, field = null, keys = XVar.Array(), pageName = null, qResult = null, shortTableName = null, strFilename = null, value = null;
				ProjectSettings pSet;
				shortTableName = XVar.Clone(MVCFunctions.postvalue(new XVar("table")));
				GlobalVars.table = XVar.Clone(CommonFunctions.GetTableByShort((XVar)(shortTableName)));
				if(XVar.Pack(!(XVar)(GlobalVars.table)))
				{
					MVCFunctions.Echo(new XVar(0));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				pageName = XVar.Clone(MVCFunctions.postvalue(new XVar("pagename")));
				strFilename = XVar.Clone(MVCFunctions.postvalue(new XVar("filename")));
				ext = XVar.Clone(MVCFunctions.substr((XVar)(strFilename), (XVar)(MVCFunctions.strlen((XVar)(strFilename)) - 4)));
				ctype = XVar.Clone(CommonFunctions.getContentTypeByExtension((XVar)(ext)));
				field = XVar.Clone(MVCFunctions.postvalue(new XVar("field")));
				if(XVar.Pack(!(XVar)(Security.userHasFieldPermissions((XVar)(GlobalVars.table), (XVar)(field), new XVar(Constants.PAGE_LIST), (XVar)(pageName), new XVar(false)))))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(GlobalVars.table), new XVar(Constants.PAGE_LIST), (XVar)(pageName)));
				_connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(GlobalVars.table)));
				keys = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> k in pSet.getTableKeys().GetEnumerator())
				{
					keys.InitAndSetArrayItem(MVCFunctions.postvalue((XVar)(MVCFunctions.Concat("key", k.Key + 1))), k.Value);
				}
				dc = XVar.Clone(new DsCommand());
				dc.filter = XVar.Clone(Security.SelectCondition(new XVar("S"), (XVar)(pSet)));
				dc.keys = XVar.Clone(keys);
				dataSource = XVar.Clone(CommonFunctions.getDataSource((XVar)(GlobalVars.table), (XVar)(pSet), (XVar)(_connection)));
				qResult = XVar.Clone(dataSource.getSingle((XVar)(dc)));
				if((XVar)(!(XVar)(qResult))  || (XVar)(!(XVar)(data = XVar.Clone(qResult.fetchAssoc()))))
				{
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				value = XVar.Clone(_connection.stripSlashesBinary((XVar)(data[field])));
				MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Type: ", ctype)));
				MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Disposition: attachment;Filename=\"", strFilename, "\"")));
				MVCFunctions.Header("Cache-Control", "private");
				MVCFunctions.SendContentLength((XVar)(MVCFunctions.strlen_bin((XVar)(value))));
				MVCFunctions.echoBinary((XVar)(value));
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
