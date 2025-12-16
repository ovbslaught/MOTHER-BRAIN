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
		public static void Add()
		{
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(true, "keepLoggedIn");
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(40, "suggestStringSize");
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(10, "searchSuggestsNumber");
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(50, "mapMarkerCount");
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(20, "lookupSuggestsNumber");
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(6, "smsCodeLength");
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(4, "smsMaskLength");
			GlobalVars.runnerProjectSettings.InitAndSetArrayItem(false, "customLDAP");
		}
	}

}
