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
	public partial class RunnerFileSystem : XClass
	{
		protected dynamic errorMessage;
		protected dynamic uniqueFileAttempts = XVar.Pack(10);
		protected dynamic stubFileData = XVar.Pack("RunnerFileSystem");
		public RunnerFileSystem(dynamic var_params)
		{
		}
		public virtual XVar saveUploadedFile(dynamic _param_uploadFile, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic uploadFile = XVar.Clone(_param_uploadFile);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion


			return null;
		}
		public virtual XVar saveData(dynamic _param_data, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion


			return null;
		}
		public virtual XVar moveFile(dynamic _param_file, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion


			return null;
		}
		public virtual XVar copyFile(dynamic _param_fileId, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			return null;
		}
		public virtual XVar delete(dynamic _param_fileId)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			#endregion


			return null;
		}
		public virtual XVar printPartial(dynamic _param_fielId, dynamic _param_seek_start = null, dynamic _param_seek_end = null)
		{
			#region default values
			if(_param_seek_start as Object == null) _param_seek_start = new XVar(0);
			if(_param_seek_end as Object == null) _param_seek_end = new XVar(-1);
			#endregion

			#region pass-by-value parameters
			dynamic fielId = XVar.Clone(_param_fielId);
			dynamic seek_start = XVar.Clone(_param_seek_start);
			dynamic seek_end = XVar.Clone(_param_seek_end);
			#endregion


			return null;
		}
		public virtual XVar get(dynamic _param_userFilename, dynamic _param_path, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic userFilename = XVar.Clone(_param_userFilename);
			dynamic path = XVar.Clone(_param_path);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			return null;
		}
		public virtual XVar getFileInfo(dynamic _param_fileId)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			#endregion

			return null;
		}
		public virtual XVar lastError()
		{
			return this.errorMessage;
		}
		protected virtual XVar setLastError(dynamic _param_str)
		{
			#region pass-by-value parameters
			dynamic str = XVar.Clone(_param_str);
			#endregion

			this.errorMessage = XVar.Clone(str);

			return null;
		}
		public static XVar uploadedFileContent(dynamic _param_uploadFile)
		{
			#region pass-by-value parameters
			dynamic uploadFile = XVar.Clone(_param_uploadFile);
			#endregion

			if(ProjectSettings.ext() == "php")
			{
				return MVCFunctions.myfile_get_contents((XVar)(uploadFile["tmp_name"]));
			}
			return MVCFunctions.myfile_get_contents((XVar)(uploadFile["file"]));
		}
		public virtual XVar fast()
		{
			return false;
		}
		public virtual XVar directUpload()
		{
			return false;
		}
		public virtual XVar autoThumbnails()
		{
			return this.directUpload();
		}
		public virtual XVar redirectToFile(dynamic _param_fileId, dynamic _param_thumbnail)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			dynamic thumbnail = XVar.Clone(_param_thumbnail);
			#endregion


			return null;
		}
		protected virtual XVar tryCreateFile(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			return false;
		}
		public virtual XVar generateFilename(dynamic _param_filename, dynamic _param_obfuscate)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			dynamic obfuscate = XVar.Clone(_param_obfuscate);
			#endregion

			dynamic dotPos = null, extension = null, name = null;
			dotPos = XVar.Clone(MVCFunctions.strrpos((XVar)(filename), new XVar(".")));
			if(!XVar.Equals(XVar.Pack(dotPos), XVar.Pack(false)))
			{
				name = XVar.Clone(MVCFunctions.substr((XVar)(filename), new XVar(0), (XVar)(dotPos)));
				extension = XVar.Clone(MVCFunctions.substr((XVar)(filename), (XVar)(dotPos)));
			}
			else
			{
				name = XVar.Clone(filename);
				extension = new XVar("");
			}
			if((XVar)(!(XVar)(this.secureFilesystem()))  && (XVar)(MVCFunctions.strtolower((XVar)(extension)) == ".php"))
			{
				extension = new XVar(".phpfile");
			}
			return (XVar.Pack(obfuscate) ? XVar.Pack(MVCFunctions.Concat(name, "_", CommonFunctions.generatePassword(new XVar(8)), extension)) : XVar.Pack(MVCFunctions.Concat(name, extension)));
		}
		public virtual XVar tryCreateUniqueFile(dynamic _param_originalFilename, dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic originalFilename = XVar.Clone(_param_originalFilename);
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic attempt = null, filename = null, filepath = null;
			attempt = new XVar(0);
			do
			{
				filename = XVar.Clone(this.generateFilename((XVar)(originalFilename), (XVar)((XVar)(XVar.Pack(0) < attempt)  || (XVar)(!(XVar)(this.secureFilesystem())))));
				filepath = XVar.Clone(MVCFunctions.Concat(path, filename));
				if(XVar.Pack(this.tryCreateFile((XVar)(filepath))))
				{
					return filename;
				}
			}
			while(++(attempt) < this.uniqueFileAttempts);
			return false;
		}
		public virtual XVar secureFilesystem()
		{
			return true;
		}
	}
	public partial class FileResult : XClass
	{
		public dynamic var_type;
		public dynamic url;
		public dynamic data;
	}
}
