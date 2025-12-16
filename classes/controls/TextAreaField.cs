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
	public partial class TextAreaField : TextControl
	{
		protected static bool skipTextAreaFieldCtor = false;
		private bool skipTextControlCtorSurrogate = new Func<bool>(() => skipTextControlCtor = true).Invoke();
		public TextAreaField(dynamic _param_field, dynamic _param_pageObject, dynamic _param_id, dynamic _param_connection)
			:base((XVar)_param_field, (XVar)_param_pageObject, (XVar)_param_id, (XVar)_param_connection)
		{
			if(skipTextAreaFieldCtor)
			{
				skipTextAreaFieldCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			dynamic id = XVar.Clone(_param_id);
			dynamic connection = XVar.Clone(_param_connection);
			#endregion

			this.format = new XVar(Constants.EDIT_FORMAT_TEXT_AREA);
		}
		public override XVar buildControl(dynamic _param_value, dynamic _param_mode, dynamic _param_fieldNum, dynamic _param_validate, dynamic _param_additionalCtrlParams, dynamic _param_data)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic mode = XVar.Clone(_param_mode);
			dynamic fieldNum = XVar.Clone(_param_fieldNum);
			dynamic validate = XVar.Clone(_param_validate);
			dynamic additionalCtrlParams = XVar.Clone(_param_additionalCtrlParams);
			dynamic data = XVar.Clone(_param_data);
			#endregion

			base.buildControl((XVar)(value), (XVar)(mode), (XVar)(fieldNum), (XVar)(validate), (XVar)(additionalCtrlParams), (XVar)(data));
			if(XVar.Pack(this.pageObject.pSetEdit.isUseRTE((XVar)(this.field))))
			{
				value = XVar.Clone(this.RTESafe((XVar)(value)));
				switch(((XVar)ProjectSettings.rteType()).ToString())
				{
					case Constants.rteBasic:
						this.buildTinyMCE((XVar)(value));
						break;
					case Constants.rteCKEditor:
						this.CreateCKeditor((XVar)(value));
						break;
					case Constants.rteInnovaEditor:
						this.buildInnova((XVar)(value));
						break;
				}
			}
			else
			{
				dynamic attrs = null, nHeight = null;
				nHeight = XVar.Clone(this.pageObject.pSetEdit.getNRows((XVar)(this.field)));
				attrs = XVar.Clone(this.getPlaceholderAttr());
				MVCFunctions.Echo(MVCFunctions.Concat("<textarea id=\"", this.cfield, "\" alt=\"", this.strLabel, "\" name=\"", this.cfield, "\" style=\"height:", nHeight, "px;\" ", attrs, " class=\"form-control\">", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "</textarea>"));
			}
			this.buildControlEnd((XVar)(validate), (XVar)(mode));

			return null;
		}
		public override XVar getFirstElementId()
		{
			return this.cfield;
		}
		protected virtual XVar RTESafe(dynamic _param_text)
		{
			#region pass-by-value parameters
			dynamic text = XVar.Clone(_param_text);
			#endregion

			dynamic tmpString = null;
			tmpString = XVar.Clone(MVCFunctions.trim((XVar)(text)));
			if(XVar.Pack(!(XVar)(tmpString)))
			{
				return "";
			}
			tmpString = XVar.Clone(MVCFunctions.str_replace(new XVar("'"), new XVar("&#39;"), (XVar)(tmpString)));
			tmpString = XVar.Clone(MVCFunctions.str_replace((XVar)(MVCFunctions.chr(new XVar(10))), new XVar(" "), (XVar)(tmpString)));
			tmpString = XVar.Clone(MVCFunctions.str_replace((XVar)(MVCFunctions.chr(new XVar(13))), new XVar(" "), (XVar)(tmpString)));
			return tmpString;
		}
		protected virtual XVar CreateCKeditor(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			MVCFunctions.Echo(MVCFunctions.Concat("<div id=\"disabledCKE_", this.cfield, "\">", "<textarea id=\"", this.cfield, "\" name=\"", this.cfield, "\" rows=\"8\" cols=\"60\">", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "</textarea>", "</div>"));

			return null;
		}
		public override XVar addJSFiles()
		{
			if((XVar)(this.pageObject.pSetEdit.isUseRTE((XVar)(this.field)))  && (XVar)(XVar.Equals(XVar.Pack(ProjectSettings.rteType()), XVar.Pack(Constants.rteBasic))))
			{
				this.pageObject.AddJSFile(new XVar("plugins/tinymce/tinymce.min.js"));
			}

			return null;
		}
		protected virtual XVar buildInnova(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic assetManagerUrl = null, nHeight = null;
			nHeight = XVar.Clone(this.pageObject.pSetEdit.getNRows((XVar)(this.field)));
			if(nHeight < 300)
			{
				nHeight = new XVar(300);
			}
			assetManagerUrl = XVar.Clone(MVCFunctions.Concat(CommonFunctions.projectURL(), "plugins/innovaeditor/assetmanager/", MVCFunctions.GetTableLink(new XVar("assetmanager"))));
			MVCFunctions.Echo(MVCFunctions.Concat("<div id=\"disabledInnova_", this.cfield, "\" style=\"width:100%; height:", nHeight, "px;\" data-am=\"", assetManagerUrl, "\">", "<textarea alt=\"", this.strLabel, "\" id=\"", this.cfield, "\" name=\"", this.cfield, "\">", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "</textarea>", "<div id=\"", this.cfield, "innova\" name=\"", this.cfield, "\" style=\"width:100%; height:", nHeight, "px;\">", "</div>", "</div>"));

			return null;
		}
		protected virtual XVar buildTinyMCE(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic nHeight = null;
			nHeight = XVar.Clone(this.pageObject.pSetEdit.getNRows((XVar)(this.field)) + 100);
			MVCFunctions.Echo(MVCFunctions.Concat("<div id=\"disabledTinyMCE_", this.cfield, "\">", "<textarea id=\"", this.cfield, "\" name=\"", this.cfield, "\" alt=\"", this.strLabel, "\" style=\"width:100%; height:", nHeight, "px;\">", MVCFunctions.runner_htmlspecialchars((XVar)(value)), "</textarea>", "</div>"));

			return null;
		}
	}
}
