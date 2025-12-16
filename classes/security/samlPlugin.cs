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
	public partial class SecurityPluginSaml : SecurityPlugin
	{
		public dynamic issuer = XVar.Pack("");
		public dynamic SSOUrl = XVar.Pack("");
		public dynamic publicKey = XVar.Pack("");
		public dynamic privateKey = XVar.Pack("");
		public dynamic idClaim = XVar.Pack("");
		public dynamic nameClaim = XVar.Pack("");
		public dynamic emailClaim = XVar.Pack("");
		protected static bool skipSecurityPluginSamlCtor = false;
		public SecurityPluginSaml(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipSecurityPluginSamlCtor)
			{
				skipSecurityPluginSamlCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.publicKey = XVar.Clone(var_params["certificate"]);
			this.issuer = XVar.Clone(var_params["issuer"]);
			this.SSOUrl = XVar.Clone(var_params["url"]);
			this.code = XVar.Clone(var_params["code"]);
			this.privateKey = XVar.Clone(var_params["privateKey"]);
			this.idClaim = XVar.Clone(var_params["idClaim"]);
			this.nameClaim = XVar.Clone(var_params["nameClaim"]);
			this.emailClaim = XVar.Clone(var_params["emailClaim"]);
		}
		public override XVar getUserInfo(dynamic _param_samlResponce)
		{
			#region pass-by-value parameters
			dynamic samlResponce = XVar.Clone(_param_samlResponce);
			#endregion

			dynamic payload = XVar.Array(), ret = XVar.Array();
			payload = XVar.Clone(this.verifySAMLResponse((XVar)(samlResponce)));
			if((XVar)(!(XVar)(payload))  || (XVar)((XVar)(!(XVar)(this.idClaim))  && (XVar)(!(XVar)(payload["id"]))))
			{
				this.var_error = new XVar("SAML security plugin: no data to identify user");
				return XVar.Array();
			}
			ret = XVar.Clone(new XVar("id", MVCFunctions.Concat(this.code, payload["id"])));
			ret.InitAndSetArrayItem(payload, "raw");
			if(XVar.Pack(this.idClaim))
			{
				ret.InitAndSetArrayItem(MVCFunctions.Concat(this.code, payload[this.idClaim]), "id");
			}
			if(XVar.Pack(this.nameClaim))
			{
				ret.InitAndSetArrayItem(payload[this.nameClaim], "name");
			}
			if(XVar.Pack(this.emailClaim))
			{
				ret.InitAndSetArrayItem(payload[this.emailClaim], "email");
			}
			return ret;
		}
		public virtual XVar verifySAMLResponse(dynamic _param_samlResponce)
		{
			#region pass-by-value parameters
			dynamic samlResponce = XVar.Clone(_param_samlResponce);
			#endregion

			return MVCFunctions.verifySamlResponse((XVar)(samlResponce), (XVar)(this.publicKey), (XVar)(this.privateKey));
		}
		public override XVar redirectToLogin()
		{
			MVCFunctions.HeaderRedirect((XVar)(MVCFunctions.Concat("", this.SSOUrl)));

			return null;
		}
	}
}
