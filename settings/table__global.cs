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
	public static partial class RunnerSettings
	{
		public static void table__global()
		{
			GlobalVars.runnerTableSettings.InitAndSetArrayItem(new XVar( "name", "<global>",
"type", 5,
"shortName", "_global",
"advancedSecurityType", 0,
"pagesByType", new XVar( "menu", new XVar( 0, "menu" ) ),
"pageTypes", new XVar( "menu", "menu" ),
"defaultPages", new XVar( "menu", "menu" ),
"originalPagesByType", new XVar( "menu", new XVar( 0, "menu" ) ),
"originalPageTypes", new XVar( "menu", "menu" ),
"originalDefaultPages", new XVar( "menu", "menu" ),
"hasJsEvents", false ), Constants.GLOBAL_PAGES);
			if(XVar.Equals(XVar.Pack(CommonFunctions.mlang_getcurrentlang()), XVar.Pack("English")))
			{
				GlobalVars.runnerTableLabels.InitAndSetArrayItem(new XVar( "pageTitles", new XVar(  ) ), Constants.GLOBAL_PAGES);
			}
		}
	}

}
