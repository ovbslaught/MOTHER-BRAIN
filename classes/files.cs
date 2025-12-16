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
	public partial class MoveFile : XClass
	{
		public dynamic sourceFilename;
		public dynamic destFilename;
		public dynamic destPath;
		public dynamic destPathIsAbsolute;
		public MoveFile(dynamic _param_source, dynamic _param_name, dynamic _param_path, dynamic _param_abs)
		{
			#region pass-by-value parameters
			dynamic source = XVar.Clone(_param_source);
			dynamic name = XVar.Clone(_param_name);
			dynamic path = XVar.Clone(_param_path);
			dynamic abs = XVar.Clone(_param_abs);
			#endregion

			this.sourceFilename = XVar.Clone(source);
			this.destFilename = XVar.Clone(name);
			this.destPath = XVar.Clone(path);
			this.destPathIsAbsolute = XVar.Clone(abs);
		}
		public virtual XVar Move()
		{
			dynamic last = null, path = null;
			path = XVar.Clone(this.destPath);
			if(XVar.Pack(!(XVar)(this.destPathIsAbsolute)))
			{
				path = XVar.Clone(MVCFunctions.getabspath((XVar)(path)));
			}
			last = XVar.Clone(MVCFunctions.substr((XVar)(path), (XVar)(MVCFunctions.strlen((XVar)(path)) - 1)));
			if((XVar)(last != "/")  && (XVar)(last != "\\"))
			{
				path = MVCFunctions.Concat(path, "/");
			}
			MVCFunctions.runner_move_uploaded_file((XVar)(this.sourceFilename), (XVar)(MVCFunctions.Concat(path, this.destFilename)));

			return null;
		}
	}
	public partial class SaveFile : XClass
	{
		public dynamic fileContents;
		public dynamic destFilename;
		public dynamic destPath;
		public dynamic destPathIsAbsolute;
		public SaveFile(dynamic _param_contents, dynamic _param_name, dynamic _param_path, dynamic _param_abs)
		{
			#region pass-by-value parameters
			dynamic contents = XVar.Clone(_param_contents);
			dynamic name = XVar.Clone(_param_name);
			dynamic path = XVar.Clone(_param_path);
			dynamic abs = XVar.Clone(_param_abs);
			#endregion

			this.fileContents = XVar.Clone(contents);
			this.destFilename = XVar.Clone(name);
			this.destPath = XVar.Clone(path);
			this.destPathIsAbsolute = XVar.Clone(abs);
		}
		public virtual XVar Save()
		{
			dynamic last = null, path = null;
			path = XVar.Clone(this.destPath);
			if(XVar.Pack(!(XVar)(this.destPathIsAbsolute)))
			{
				path = XVar.Clone(MVCFunctions.getabspath((XVar)(path)));
			}
			last = XVar.Clone(MVCFunctions.substr((XVar)(path), (XVar)(MVCFunctions.strlen((XVar)(path)) - 1)));
			if((XVar)(last != "/")  && (XVar)(last != "\\"))
			{
				path = MVCFunctions.Concat(path, "/");
			}
			MVCFunctions.runner_save_file((XVar)(MVCFunctions.Concat(path, this.destFilename)), (XVar)(this.fileContents));

			return null;
		}
	}
	public partial class DeleteFile : XClass
	{
		public dynamic destFilename;
		public dynamic destPath;
		public dynamic destPathIsAbsolute;
		public DeleteFile(dynamic _param_name, dynamic _param_path, dynamic _param_abs)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic path = XVar.Clone(_param_path);
			dynamic abs = XVar.Clone(_param_abs);
			#endregion

			this.destFilename = XVar.Clone(name);
			this.destPath = XVar.Clone(path);
			this.destPathIsAbsolute = XVar.Clone(abs);
		}
		public virtual XVar Delete()
		{
			dynamic last = null, path = null;
			path = XVar.Clone(this.destPath);
			if(XVar.Pack(!(XVar)(this.destPathIsAbsolute)))
			{
				path = XVar.Clone(MVCFunctions.getabspath((XVar)(path)));
			}
			last = XVar.Clone(MVCFunctions.substr((XVar)(path), (XVar)(MVCFunctions.strlen((XVar)(path)) - 1)));
			if((XVar)(last != "/")  && (XVar)(last != "\\"))
			{
				path = MVCFunctions.Concat(path, "/");
			}
			MVCFunctions.runner_delete_file((XVar)(MVCFunctions.Concat(path, this.destFilename)));

			return null;
		}
	}
}
