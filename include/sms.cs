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
	public partial class CommonFunctions
	{
		public static XVar runner_sms(dynamic _param_number, dynamic _param_message, dynamic _param_parameters = null)
		{
			#region default values
			if(_param_parameters as Object == null) _param_parameters = new XVar(XVar.Array());
			#endregion

			#region pass-by-value parameters
			dynamic number = XVar.Clone(_param_number);
			dynamic message = XVar.Clone(_param_message);
			dynamic parameters = XVar.Clone(_param_parameters);
			#endregion

			dynamic certPath = null, headers = XVar.Array(), result = XVar.Array(), twilioAuth = null, twilioNumber = null, twilioSID = null, url = null, var_response = XVar.Array();
			twilioSID = XVar.Clone(ProjectSettings.getProjectValue(new XVar("smsSettings"), new XVar("twilioSID")));
			twilioAuth = XVar.Clone(ProjectSettings.getProjectValue(new XVar("smsSettings"), new XVar("twilioAuth")));
			twilioNumber = XVar.Clone(ProjectSettings.getProjectValue(new XVar("smsSettings"), new XVar("twilioNumber")));
			if(XVar.Pack(!(XVar)(parameters.KeyExists("To"))))
			{
				parameters.InitAndSetArrayItem(number, "To");
			}
			if(XVar.Pack(!(XVar)(parameters.KeyExists("Body"))))
			{
				parameters.InitAndSetArrayItem(message, "Body");
			}
			parameters.InitAndSetArrayItem(twilioNumber, "From");
			url = XVar.Clone(MVCFunctions.Concat("https://api.twilio.com/2010-04-01/Accounts/", twilioSID, "/Messages.json"));
			headers = XVar.Clone(XVar.Array());
			headers.InitAndSetArrayItem("twilio-php/5.7.3 (PHP 5.6.12)", "User-Agent");
			headers.InitAndSetArrayItem("utf-8", "Accept-Charset");
			headers.InitAndSetArrayItem("application/x-www-form-urlencoded", "Content-Type");
			headers.InitAndSetArrayItem("application/json", "Accept");
			headers.InitAndSetArrayItem(MVCFunctions.Concat("Basic ", MVCFunctions.base64_encode((XVar)(MVCFunctions.Concat(twilioSID, ":", twilioAuth)))), "Authorization");
			certPath = XVar.Clone(MVCFunctions.getabspath(new XVar("include/cacert.pem")));
			result = XVar.Clone(XVar.Array());
			result.InitAndSetArrayItem(false, "success");
			var_response = XVar.Clone(MVCFunctions.runner_post_request((XVar)(url), (XVar)(parameters), (XVar)(headers), (XVar)(certPath)));
			if(XVar.Pack(!(XVar)(var_response["error"])))
			{
				result.InitAndSetArrayItem(MVCFunctions.runner_json_decode((XVar)(var_response["content"])), "response");
				if(result["response"]["status"] == "queued")
				{
					result.InitAndSetArrayItem(true, "success");
				}
				else
				{
					result.InitAndSetArrayItem(MVCFunctions.Concat("Twilio error: ", result["response"]["message"]), "error");
				}
			}
			else
			{
				result.InitAndSetArrayItem(var_response["error"], "error");
			}
			return result;
		}
	}
}
