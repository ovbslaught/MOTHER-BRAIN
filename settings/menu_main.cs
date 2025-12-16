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
		public static void menu_main()
		{
			GlobalVars.runnerMenus.InitAndSetArrayItem(new XVar( "name", "main",
"id", "main",
"treeLike", true,
"root", new XVar( "id", "",
"parent", "",
"children", XVar.Array(),
"data", new XVar( "name", new XVar( "text", "",
"type", 0 ),
"comments", new XVar( "text", "",
"type", 0 ),
"style", "",
"href", "",
"params", "",
"pageId", "",
"itemType", 0,
"linkType", 2,
"openType", 0,
"iconType", 0,
"iconName", "",
"iconStyle", 0,
"showIconType", 1,
"linkToAnotherApp", false ) ) ), "main");
		}
	}

}
