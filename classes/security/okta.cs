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
	public partial class SecurityPluginOkta : SecurityPluginOpenId
	{
		public dynamic domain = XVar.Pack("");
		protected static bool skipSecurityPluginOktaCtor = false;
		public SecurityPluginOkta(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipSecurityPluginOktaCtor)
			{
				skipSecurityPluginOktaCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.domain = XVar.Clone(var_params["domain"]);
			this.configUrl = XVar.Clone(MVCFunctions.Concat("https://", this.domain, "/.well-known/openid-configuration"));
		}
	}
}
