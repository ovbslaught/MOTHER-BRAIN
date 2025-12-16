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
	public partial class DbCommon
	{
		public static void Apply() {

			//RunnerSettings.globalVars();
			GlobalVars.init_globalvars();
			RunnerSettings.Defaults();
			RunnerSettings.Apply();
			RunnerSettings.Add();

			RunnerSettings.Legacy();

			RunnerSettings.Databases();

			
			//	.NET-specific initialization
			GlobalVars.BufferStack = new Stack<StringBuilder>();
			MVCFunctions.ob_start();
			GlobalVars.IsOutputDone = false;

			GlobalVars.mlang_defaultlang = XVar.Clone(CommonFunctions.getDefaultLanguage());
			CommonFunctions.loadLanguage((XVar)(CommonFunctions.mlang_getcurrentlang()));
			RunnerSettings.Locale();

			GlobalVars.locale_info.InitAndSetArrayItem(CommonFunctions.GetLongDateFormat(), "LOCALE_ILONGDATE");
			GlobalVars.cCharset = XVar.Clone(GlobalVars.runnerProjectSettings["charset"]);
			GlobalVars.cCodePage = XVar.Clone(GlobalVars.runnerProjectSettings["codepage"]);
			MVCFunctions.Header((XVar)(MVCFunctions.Concat("Content-Type: text/html; charset=", GlobalVars.cCharset)));
			GlobalVars.useUTF8 = XVar.Clone(GlobalVars.cCharset == "utf-8");
			
			if( GlobalVars.globalEvents.exists( "BeforeConnect" ) ) {
				GlobalVars.globalEvents.BeforeConnect();
			}
			if( Security.hasLogin() ) {
				Security.autoLoginAsGuest();
				Security.updateCSRFCookie();
			}

			if( GlobalVars.globalEvents.exists( "AfterAppInit" ) ) {
				GlobalVars.globalEvents.AfterAppInit();
			}
		}
	}
}
