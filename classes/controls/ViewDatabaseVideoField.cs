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
	public partial class ViewDatabaseVideoField : ViewVideoFileField
	{
		protected static bool skipViewDatabaseVideoFieldCtor = false;
		public ViewDatabaseVideoField(dynamic field, dynamic container, dynamic pageobject) // proxy constructor
			:base((XVar)field, (XVar)container, (XVar)pageobject) {}

		public override XVar getTextValue(dynamic data)
		{
			dynamic fileNameField = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(data[this.field])))))
			{
				return "";
			}
			fileNameField = XVar.Clone(this.getContainer().pSet.getFilenameField((XVar)(this.field)));
			if((XVar)(fileNameField)  && (XVar)(data[fileNameField]))
			{
				return data[fileNameField];
			}
			return "<<Video>>";
		}
		public override XVar getExportValue(dynamic data, dynamic _param_keylink = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			if(_param_html as Object == null) _param_html = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			return CommonFunctions.mlang_message(new XVar("LONG_BINARY"));
		}
		protected override XVar getFileURLs(dynamic data, dynamic _param_keylink)
		{
			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic ext = null, fileNameField = null, fileURLs = null, url = null, var_params = XVar.Array(), var_type = null;
			ProjectSettings pSet;
			fileURLs = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(data[this.field])))
			{
				return XVar.Array();
			}
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			var_params = XVar.Clone(XVar.Array());
			fileNameField = XVar.Clone(pSet.getFilenameField((XVar)(this.field)));
			var_params.InitAndSetArrayItem((XVar.Pack((XVar)(fileNameField)  && (XVar)(data[fileNameField])) ? XVar.Pack(data[fileNameField]) : XVar.Pack("Track.flv")), "filename");
			var_params.InitAndSetArrayItem(pSet.table(), "table");
			var_params.InitAndSetArrayItem(this.field, "field");
			var_params.InitAndSetArrayItem(1, "nodisp");
			var_params.InitAndSetArrayItem(CommonFunctions.fileAttrHash((XVar)(keylink), (XVar)(MVCFunctions.strlen_bin((XVar)(data[this.field])))), "hash");
			url = XVar.Clone(MVCFunctions.Concat(CommonFunctions.projectURL(), MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat(CommonFunctions.prepareUrlQuery((XVar)(var_params)), keylink)))));
			ext = XVar.Clone(CommonFunctions.getFileExtension((XVar)(var_params["filename"])));
			var_type = XVar.Clone(CommonFunctions.mimeTypeByExt((XVar)(ext)));
			if(var_type == "application/octet-stream")
			{
				var_type = new XVar("video/flv");
			}
			if(!XVar.Equals(XVar.Pack(MVCFunctions.strpos((XVar)(var_type), new XVar("video"))), XVar.Pack(0)))
			{
				return XVar.Array();
			}
			return new XVar(0, new XVar("url", url, "type", var_type));
		}
	}
}
