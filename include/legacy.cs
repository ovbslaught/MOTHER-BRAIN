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
		public static void Legacy()
		{
			GlobalVars.cCharset = XVar.Clone(GlobalVars.runnerProjectSettings["charset"]);
			GlobalVars.cCodePage = XVar.Clone(GlobalVars.runnerProjectSettings["codepage"]);
			GlobalVars.useUTF8 = XVar.Clone(GlobalVars.cCharset == "utf-8");
			GlobalVars.loginKeyFields = XVar.Clone(ProjectSettings.getProjectValue(new XVar("userTableKeys")));
			GlobalVars.cLoginTable = XVar.Clone(Security.loginTable());
			GlobalVars.cUserNameField = XVar.Clone(Security.usernameField());
			GlobalVars.cUserGroupField = XVar.Clone(Security.groupIdField());
			GlobalVars.cPasswordField = XVar.Clone(Security.passwordField());
			GlobalVars.cDisplayNameField = XVar.Clone(Security.fullnameField());
			GlobalVars.cUserpicField = XVar.Clone(Security.userpicField());
			GlobalVars.projectBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("projectBuild")));
			GlobalVars.wizardBuildKey = XVar.Clone(ProjectSettings.getProjectValue(new XVar("wizardBuild")));
			GlobalVars.projectLanguage = XVar.Clone(ProjectSettings.ext());
		}
	}

}
