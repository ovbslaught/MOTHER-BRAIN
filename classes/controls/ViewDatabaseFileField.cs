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
	public partial class ViewDatabaseFileField : ViewFileDownloadField
	{
		protected dynamic defaultFileName = XVar.Pack("file.bin");
		protected static bool skipViewDatabaseFileFieldCtor = false;
		public ViewDatabaseFileField(dynamic _param_field, dynamic _param_container, dynamic _param_pageobject) // proxy constructor
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageobject) {}

		public override XVar getPdfValue(dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			return MVCFunctions.runner_json_encode((XVar)(new XVar("text", this.getFileName((XVar)(data)))));
		}
		public virtual XVar getFileName(dynamic data)
		{
			dynamic fileName = null, fileNameF = null;
			fileNameF = XVar.Clone(this.container.pSet.getFilenameField((XVar)(this.field)));
			if(XVar.Pack(fileNameF))
			{
				fileName = XVar.Clone(data[fileNameF]);
				if(XVar.Pack(!(XVar)(fileName)))
				{
					fileName = XVar.Clone(this.defaultFileName);
				}
			}
			else
			{
				fileName = XVar.Clone(this.defaultFileName);
			}
			return fileName;
		}
		public override XVar getTextValue(dynamic data)
		{
			dynamic fileNameField = null;
			if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(data[this.field])))))
			{
				return "";
			}
			fileNameField = XVar.Clone(this.container.pSet.getFilenameField((XVar)(this.field)));
			if((XVar)(fileNameField)  && (XVar)(data[fileNameField]))
			{
				return data[fileNameField];
			}
			return "<<File>>";
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

			dynamic ext = null, fileURLs = null, filename = null, imageUrl = null, newTab = null, url = null, var_params = XVar.Array();
			ProjectSettings pSet;
			fileURLs = XVar.Clone(XVar.Array());
			if(XVar.Pack(!(XVar)(data[this.field])))
			{
				return XVar.Array();
			}
			pSet = XVar.UnPackProjectSettings(this.pSettings());
			filename = XVar.Clone(this.getFileName((XVar)(data)));
			var_params = XVar.Clone(XVar.Array());
			var_params.InitAndSetArrayItem(filename, "filename");
			var_params.InitAndSetArrayItem(pSet.table(), "table");
			var_params.InitAndSetArrayItem(this.field, "field");
			var_params.InitAndSetArrayItem(1, "nodisp");
			var_params.InitAndSetArrayItem(CommonFunctions.fileAttrHash((XVar)(keylink), (XVar)(MVCFunctions.strlen_bin((XVar)(data[this.field])))), "hash");
			url = XVar.Clone(MVCFunctions.GetTableLink(new XVar("file"), new XVar(""), (XVar)(MVCFunctions.Concat(CommonFunctions.prepareUrlQuery((XVar)(var_params)), keylink))));
			ext = XVar.Clone(CommonFunctions.getFileExtension((XVar)(var_params["filename"])));
			imageUrl = XVar.Clone(MVCFunctions.Concat("images/icons/", ViewFileDownloadField.getFileIconByType((XVar)(filename), new XVar(""))));
			newTab = new XVar(false);
			if((XVar)(this.displayPDF())  && (XVar)(MVCFunctions.strtolower((XVar)(CommonFunctions.getFileExtension((XVar)(var_params["filename"])))) == "pdf"))
			{
				url = XVar.Clone(MVCFunctions.Concat("include/pdfjs/web/viewer.html?file=", MVCFunctions.RawUrlEncode((XVar)(MVCFunctions.Concat(MVCFunctions.projectPath(), url)))));
				newTab = new XVar(true);
			}
			return new XVar(0, new XVar("url", url, "filename", filename, "imageUrl", imageUrl, "size", MVCFunctions.strlen_bin((XVar)(data[this.field])), "newTab", newTab));
		}
	}
}
