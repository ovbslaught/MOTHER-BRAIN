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
		public static void pages()
		{
			GlobalVars.runnerPageInfo = new XVar(new XVar( "allPages", new XVar( "<global>", new XVar( "menu", new XVar( 0, "menu" ) ) ),
"tableMasks", new XVar( "<global>", "S" ) ));
		}
	}

}
