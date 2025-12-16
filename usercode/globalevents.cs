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
	public partial class class_GlobalEvents : GlobalEventsBase
	{
		protected static bool skipclass_GlobalEventsCtor = false;
		public override XVar init()
		{
			this.events = new XVar(new XVar( "pageEvents", new XVar(  ),
"onScreenEvents", new XVar(  ),
"dashboardEvents", new XVar(  ),
"buttons", new XVar(  ),
"maps", new XVar(  ),
"tablePermissions", new XVar(  ),
"recordEditable", new XVar(  ) ));

			return null;
		}
	}
}
