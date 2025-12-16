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
	public partial class RestManager : XClass
	{
		public virtual XVar getConnection(dynamic _param_id)
		{
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			#endregion

			dynamic connInfo = null;
			if(id == Constants.spidGOOGLEDRIVE)
			{
				return CommonFunctions.getGoogleDriveConnection();
			}
			if(id == Constants.spidONEDRIVE)
			{
				return CommonFunctions.getOneDriveConnection();
			}
			if(id == Constants.spidDROPBOX)
			{
				return CommonFunctions.getDropboxConnection();
			}
			connInfo = XVar.Clone(CommonFunctions.runnerGetRestConnectionInfo((XVar)(id)));
			if(XVar.Pack(!(XVar)(connInfo)))
			{
				return null;
			}
			return new RestConnection((XVar)(connInfo));
		}
		public virtual XVar idByName(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			return CommonFunctions.runnerRestConnectionIdByName((XVar)(name));
		}
	}
}
