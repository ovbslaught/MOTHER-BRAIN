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
	public partial class TLayout : XClass
	{
		public dynamic containers = XVar.Array();
		public dynamic container_properties = XVar.Array();
		public dynamic blocks = XVar.Array();
		public dynamic name = XVar.Pack("");
		public dynamic version;
		public dynamic style = XVar.Pack("");
		public dynamic styleMobile = XVar.Pack("");
		public dynamic skins = XVar.Array();
		public dynamic skinsparams = XVar.Array();
		public dynamic bootstrapTheme = XVar.Pack("");
		public dynamic customCssPageName = XVar.Pack("");
		public TLayout(dynamic _param_name, dynamic _param_style, dynamic _param_styleMobile)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic style = XVar.Clone(_param_style);
			dynamic styleMobile = XVar.Clone(_param_styleMobile);
			#endregion

			this.name = XVar.Clone(name);
			this.style = XVar.Clone(style);
			this.styleMobile = XVar.Clone(styleMobile);
		}
		public virtual XVar pdfStyle()
		{
			return MVCFunctions.Concat("Pdf", MVCFunctions.substr((XVar)(this.styleMobile), new XVar(6)));
		}
		public virtual XVar isBrickSet(dynamic _param_brickName)
		{
			#region pass-by-value parameters
			dynamic brickName = XVar.Clone(_param_brickName);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> container in this.containers.GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> brick in container.Value.GetEnumerator())
				{
					if(brick.Value["name"] == brickName)
					{
						return true;
					}
				}
			}
			return false;
		}
		public virtual XVar getBrickTableName(dynamic _param_brickName)
		{
			#region pass-by-value parameters
			dynamic brickName = XVar.Clone(_param_brickName);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> container in this.containers.GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> brick in container.Value.GetEnumerator())
				{
					if(brick.Value["name"] == brickName)
					{
						return brick.Value["table"];
					}
				}
			}
			return "";
		}
		public virtual XVar getCSSFiles(dynamic _param_rtl = null, dynamic _param_pdf = null)
		{
			#region default values
			if(_param_rtl as Object == null) _param_rtl = new XVar(false);
			if(_param_pdf as Object == null) _param_pdf = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic rtl = XVar.Clone(_param_rtl);
			dynamic pdf = XVar.Clone(_param_pdf);
			#endregion

			dynamic files = XVar.Array(), suffix = null;
			files = XVar.Clone(XVar.Array());
			suffix = new XVar("");
			if(XVar.Pack(rtl))
			{
				suffix = new XVar("RTL");
			}
			files.InitAndSetArrayItem("include/bootstrap/css/bootstrap.min.css", null);
			if(XVar.Pack(MVCFunctions.strlen((XVar)(this.bootstrapTheme))))
			{
				files.InitAndSetArrayItem(MVCFunctions.Concat("styles/bootstrap/", this.bootstrapTheme, "/css/bootstrap-theme.min.css"), null);
			}
			else
			{
				files.InitAndSetArrayItem("include/bootstrap/css/bootstrap-theme.min.css", null);
			}
			if(XVar.Pack(pdf))
			{
				files.InitAndSetArrayItem("styles/pdf.css", null);
			}
			files.InitAndSetArrayItem(MVCFunctions.Concat("styles/bs", suffix, ".css"), null);
			if(XVar.Pack(MVCFunctions.strlen((XVar)(this.bootstrapTheme))))
			{
				if(XVar.Pack(MVCFunctions.file_exists((XVar)(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat("styles/bootstrap/", this.bootstrapTheme, "/css/tweaks", suffix, ".css")))))))
				{
					files.InitAndSetArrayItem(MVCFunctions.Concat("styles/bootstrap/", this.bootstrapTheme, "/css/tweaks", suffix, ".css"), null);
				}
			}
			if(XVar.Pack(MVCFunctions.file_exists((XVar)(MVCFunctions.getabspath(new XVar("styles/custom/custom.css"))))))
			{
				files.InitAndSetArrayItem(MVCFunctions.Concat("styles/custom/custom", suffix, ".css"), null);
			}
			if((XVar)(MVCFunctions.strlen((XVar)(this.customCssPageName)))  && (XVar)(MVCFunctions.file_exists((XVar)(MVCFunctions.getabspath((XVar)(MVCFunctions.Concat("styles/custom/", this.customCssPageName, ".css")))))))
			{
				files.InitAndSetArrayItem(MVCFunctions.Concat("styles/custom/", this.customCssPageName, suffix, ".css"), null);
			}
			return files;
		}
	}
}
