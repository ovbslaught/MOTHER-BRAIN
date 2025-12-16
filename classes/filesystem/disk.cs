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
	public partial class DiskFileSystem : RunnerFileSystem
	{
		public dynamic absolutePath = XVar.Pack(false);
		public dynamic path = XVar.Pack("");
		protected static bool skipDiskFileSystemCtor = false;
		public DiskFileSystem(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipDiskFileSystemCtor)
			{
				skipDiskFileSystemCtor = false;
				return;
			}
			this.absolutePath = XVar.Clone(var_params["absolutePath"]);
			this.path = XVar.Clone(var_params["path"]);
		}
		public override XVar saveUploadedFile(dynamic _param_uploadFile, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic uploadFile = XVar.Clone(_param_uploadFile);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic filename = null, path = null;
			path = XVar.Clone(DiskFileSystem.normalizePath((XVar)(this.path)));
			if(XVar.Pack(!(XVar)(path)))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.makeSurePathExists((XVar)(path)))))
			{
				this.errorMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("ERROR_NO_UPLOAD_FOLDER")));
				return null;
			}
			filename = XVar.Clone(this.tryCreateUniqueFile((XVar)(MVCFunctions.simplify_file_name((XVar)(userFilename))), (XVar)(path)));
			if(XVar.Pack(!(XVar)(filename)))
			{
				this.errorMessage = XVar.Clone(MVCFunctions.Concat("Unable to create file in ", path));
				return null;
			}
			MVCFunctions.upload_File((XVar)(uploadFile), (XVar)(MVCFunctions.Concat(path, filename)));
			if(XVar.Pack(this.absolutePath))
			{
				return MVCFunctions.Concat(path, filename);
			}
			else
			{
				return MVCFunctions.Concat(DiskFileSystem.normalizeRelativePath((XVar)(this.path)), filename);
			}

			return null;
		}
		public override XVar moveFile(dynamic _param_file, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic filename = null, path = null;
			path = XVar.Clone(DiskFileSystem.normalizePath((XVar)(this.path)));
			if(XVar.Pack(!(XVar)(path)))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.makeSurePathExists((XVar)(path)))))
			{
				this.errorMessage = XVar.Clone(CommonFunctions.mlang_message(new XVar("ERROR_NO_UPLOAD_FOLDER")));
				return null;
			}
			filename = XVar.Clone(this.tryCreateUniqueFile((XVar)(MVCFunctions.simplify_file_name((XVar)(userFilename))), (XVar)(path)));
			if(XVar.Pack(!(XVar)(filename)))
			{
				this.errorMessage = XVar.Clone(MVCFunctions.Concat("Unable to create file in ", path));
				return null;
			}
			MVCFunctions.rename((XVar)(file), (XVar)(MVCFunctions.Concat(path, filename)));
			if(XVar.Pack(this.absolutePath))
			{
				return MVCFunctions.Concat(path, filename);
			}
			else
			{
				return MVCFunctions.Concat(DiskFileSystem.normalizeRelativePath((XVar)(this.path)), filename);
			}

			return null;
		}
		public override XVar saveData(dynamic _param_data, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic filename = null, path = null;
			path = XVar.Clone(DiskFileSystem.normalizePath((XVar)(this.path)));
			if(XVar.Pack(!(XVar)(path)))
			{
				return null;
			}
			filename = XVar.Clone(this.tryCreateUniqueFile((XVar)(userFilename), (XVar)(path)));
			MVCFunctions.runner_save_file((XVar)(MVCFunctions.Concat(path, filename)), (XVar)(data));
			if(XVar.Pack(this.absolutePath))
			{
				return MVCFunctions.Concat(path, filename);
			}
			else
			{
				return MVCFunctions.Concat(DiskFileSystem.normalizeRelativePath((XVar)(this.path)), filename);
			}

			return null;
		}
		public override XVar copyFile(dynamic _param_fileId, dynamic _param_userFilename)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			dynamic userFilename = XVar.Clone(_param_userFilename);
			#endregion

			dynamic filename = null, path = null;
			path = XVar.Clone(DiskFileSystem.normalizePath((XVar)(this.path)));
			if(XVar.Pack(!(XVar)(path)))
			{
				return null;
			}
			filename = XVar.Clone(this.tryCreateUniqueFile((XVar)(userFilename), (XVar)(path)));
			if(XVar.Pack(!(XVar)(filename)))
			{
				return null;
			}
			MVCFunctions.runner_copy_file((XVar)(DiskFileSystem.normalizeFilePath((XVar)(fileId))), (XVar)(MVCFunctions.Concat(path, filename)));
			if(XVar.Pack(this.absolutePath))
			{
				return MVCFunctions.Concat(path, filename);
			}
			else
			{
				return MVCFunctions.Concat(DiskFileSystem.normalizeRelativePath((XVar)(this.path)), filename);
			}

			return null;
		}
		public override XVar delete(dynamic _param_fileId)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			#endregion

			fileId = XVar.Clone(DiskFileSystem.normalizeFilePath((XVar)(fileId)));
			MVCFunctions.unlink((XVar)(fileId));

			return null;
		}
		public override XVar get(dynamic _param_userFilename, dynamic _param_path, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic userFilename = XVar.Clone(_param_userFilename);
			dynamic path = XVar.Clone(_param_path);
			dynamic data = XVar.Clone(_param_data);
			#endregion


			return null;
		}
		public override XVar printPartial(dynamic _param_fileId, dynamic _param_seek_start = null, dynamic _param_seek_end = null)
		{
			#region default values
			if(_param_seek_start as Object == null) _param_seek_start = new XVar(0);
			if(_param_seek_end as Object == null) _param_seek_end = new XVar(-1);
			#endregion

			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			dynamic seek_start = XVar.Clone(_param_seek_start);
			dynamic seek_end = XVar.Clone(_param_seek_end);
			#endregion

			if(XVar.Pack(!(XVar)(fileId)))
			{
				return null;
			}
			fileId = XVar.Clone(DiskFileSystem.normalizeFilePath((XVar)(fileId)));
			if((XVar)(!(XVar)(seek_start))  && (XVar)(seek_end < XVar.Pack(0)))
			{
				MVCFunctions.printfile((XVar)(fileId));
			}
			MVCFunctions.printfileByRange((XVar)(fileId), (XVar)(seek_start), (XVar)(seek_end));

			return null;
		}
		public static XVar normalizePath(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			path = XVar.Clone(DiskFileSystem.normalizeRelativePath((XVar)(path)));
			if(XVar.Pack(!(XVar)(MVCFunctions.isAbsolutePath((XVar)(path)))))
			{
				path = XVar.Clone(MVCFunctions.getabspath((XVar)(path)));
			}
			return path;
		}
		public static XVar normalizeFilePath(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.isAbsolutePath((XVar)(path)))))
			{
				path = XVar.Clone(MVCFunctions.getabspath((XVar)(path)));
			}
			return path;
		}
		public static XVar normalizeRelativePath(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic lastSymbol = null;
			if(XVar.Pack(!(XVar)(path)))
			{
				return "";
			}
			lastSymbol = XVar.Clone(MVCFunctions.substr((XVar)(path), (XVar)(MVCFunctions.strlen((XVar)(path)) - 1), new XVar(1)));
			if((XVar)(lastSymbol != "/")  && (XVar)(!XVar.Equals(XVar.Pack(lastSymbol), XVar.Pack("\\"))))
			{
				path = MVCFunctions.Concat(path, "/");
			}
			return path;
		}
		public override XVar getFileInfo(dynamic _param_fileId)
		{
			#region pass-by-value parameters
			dynamic fileId = XVar.Clone(_param_fileId);
			#endregion

			fileId = XVar.Clone(DiskFileSystem.normalizeFilePath((XVar)(fileId)));
			if(XVar.Pack(!(XVar)(MVCFunctions.myfile_exists((XVar)(fileId)))))
			{
				return null;
			}
			return new XVar("fullPath", fileId, "size", MVCFunctions.filesize((XVar)(fileId)), "returnContent", true, "lastModified", MVCFunctions.filemtime((XVar)(fileId)));
		}
		public override XVar fast()
		{
			return true;
		}
		protected override XVar tryCreateFile(dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic path = XVar.Clone(_param_path);
			#endregion

			return MVCFunctions.try_create_new_file((XVar)(path));
		}
		public static XVar uniqueFilename(dynamic _param_filename, dynamic _param_path)
		{
			#region pass-by-value parameters
			dynamic filename = XVar.Clone(_param_filename);
			dynamic path = XVar.Clone(_param_path);
			#endregion

			dynamic fs = null, var_params = null;
			var_params = XVar.Clone(XVar.Array());
			fs = XVar.Clone(new DiskFileSystem((XVar)(var_params)));
			return fs.tryCreateUniqueFile((XVar)(filename), (XVar)(path));
		}
		public override XVar secureFilesystem()
		{
			return false;
		}
	}
}
