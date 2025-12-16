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
	public partial class SecurityPlugin : XClass
	{
		public dynamic code;
		protected dynamic appId = XVar.Pack("");
		protected dynamic appSecret = XVar.Pack("");
		protected dynamic var_error = XVar.Pack("");
		protected dynamic provider;
		public SecurityPlugin(dynamic _param_params)
		{
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.code = XVar.Clone(var_params["code"]);
			this.provider = XVar.Clone(Security.findProvider((XVar)(this.code)));
		}
		public virtual XVar savedToken()
		{
			return false;
		}
		public virtual XVar getJSSettings()
		{
			return XVar.Array();
		}
		public virtual XVar getUserInfo(dynamic _param_token)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			#endregion

			return XVar.Array();
		}
		public virtual XVar onLogout()
		{

			return null;
		}
		public virtual XVar logout()
		{

			return null;
		}
		public virtual XVar getError()
		{
			return this.var_error;
		}
		public virtual XVar redirectToLogin()
		{

			return null;
		}
		public virtual XVar validateCallback(dynamic _param_code)
		{
			#region pass-by-value parameters
			dynamic code = XVar.Clone(_param_code);
			#endregion

			return false;
		}
		public virtual XVar hasExternalLogout()
		{
			return false;
		}
		public virtual XVar redirectToLogout(dynamic _param_token, dynamic _param_redirectUri)
		{
			#region pass-by-value parameters
			dynamic token = XVar.Clone(_param_token);
			dynamic redirectUri = XVar.Clone(_param_redirectUri);
			#endregion


			return null;
		}
		public virtual XVar saveStorageData()
		{

			return null;
		}
	}
}
