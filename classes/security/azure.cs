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
	public partial class SecurityPluginAzure : SecurityPluginOpenId
	{
		protected static bool skipSecurityPluginAzureCtor = false;
		public SecurityPluginAzure(dynamic _param_params) // proxy constructor
			:base((XVar)_param_params) {}

	}
}
