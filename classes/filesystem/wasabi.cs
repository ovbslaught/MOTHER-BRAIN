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
	public partial class WasabiFileSystem : S3FileSystem
	{
		protected static bool skipWasabiFileSystemCtor = false;
		public WasabiFileSystem(dynamic _param_params) // proxy constructor
			:base((XVar)_param_params) {}

		protected override XVar urlPostfix()
		{
			if(XVar.Pack(this.region))
			{
				return MVCFunctions.Concat(".s3.", this.region, ".wasabisys.com");
			}
			return ".s3.wasabisys.com";
		}
		public override XVar directUpload()
		{
			return false;
		}
	}
}
