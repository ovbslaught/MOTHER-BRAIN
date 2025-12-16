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
	public partial class ViewControlsContainer : XClass
	{
		public dynamic viewControls = XVar.Array();
		public ProjectSettings pSet = null;
		public dynamic pageType = XVar.Pack("");
		public dynamic isDetailsPreview = XVar.Pack(false);
		public dynamic pageObject = XVar.Pack(null);
		public dynamic forExport = XVar.Pack("");
		public dynamic isLocal = XVar.Pack(false);
		public dynamic recId = XVar.Pack(0);
		public dynamic id = XVar.Pack(0);
		public dynamic fullText = XVar.Pack(false);
		public dynamic includes_js = XVar.Array();
		public dynamic includes_jsreq = XVar.Array();
		public dynamic includes_css = XVar.Array();
		public dynamic viewControlsMap = XVar.Array();
		public dynamic linkFieldValues = XVar.Array();
		public dynamic originlinkValues = XVar.Array();
		public dynamic tName = XVar.Pack("");
		public dynamic searchHighlight = XVar.Pack(false);
		public ViewControlsContainer(dynamic _param_pSet_packed, dynamic _param_pageType, dynamic _param_pageObject = null)
		{
			#region packeted values
			ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
			#endregion

			#region default values
			if(_param_pageObject as Object == null) _param_pageObject = new XVar();
			#endregion

			#region pass-by-value parameters
			ProjectSettings pSet = XVar.Clone(_param_pSet);
			dynamic pageType = XVar.Clone(_param_pageType);
			dynamic pageObject = XVar.Clone(_param_pageObject);
			#endregion

			this.pSet = XVar.UnPackProjectSettings(pSet);
			this.pageType = XVar.Clone(pageType);
			this.pageObject = XVar.Clone(pageObject);
			this.tName = XVar.Clone(pSet.table());
			if(XVar.Pack(pageObject))
			{
				this.pSet.setPageMode((XVar)(pageObject.mode));
			}
			this.searchHighlight = XVar.Clone(this.highlightSearchResults());
		}
		protected virtual XVar highlightSearchResults()
		{
			dynamic curPageObject = null;
			curPageObject = XVar.Clone(this.pageObject);
			if((XVar)((XVar)(XVar.Equals(XVar.Pack(curPageObject), XVar.Pack(null)))  || (XVar)(!(XVar)(this.pSet.highlightSearchResults())))  || (XVar)(this.pageType != Constants.PAGE_LIST))
			{
				return false;
			}
			if((XVar)((XVar)((XVar)(curPageObject.mode != Constants.LIST_SIMPLE)  && (XVar)(curPageObject.mode != Constants.LIST_AJAX))  && (XVar)(curPageObject.mode != Constants.LIST_LOOKUP))  && (XVar)(curPageObject.mode != Constants.LIST_DASHBOARD))
			{
				return false;
			}
			return true;
		}
		public virtual XVar setForExportVar(dynamic _param_forExport)
		{
			#region pass-by-value parameters
			dynamic forExport = XVar.Clone(_param_forExport);
			#endregion

			this.forExport = XVar.Clone(forExport);

			return null;
		}
		public virtual XVar AddJSFile(dynamic _param_file, dynamic _param_req1 = null, dynamic _param_req2 = null, dynamic _param_req3 = null)
		{
			#region default values
			if(_param_req1 as Object == null) _param_req1 = new XVar("");
			if(_param_req2 as Object == null) _param_req2 = new XVar("");
			if(_param_req3 as Object == null) _param_req3 = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			dynamic req1 = XVar.Clone(_param_req1);
			dynamic req2 = XVar.Clone(_param_req2);
			dynamic req3 = XVar.Clone(_param_req3);
			#endregion

			dynamic rootPath = null;
			rootPath = XVar.Clone(file);
			this.includes_js.InitAndSetArrayItem(rootPath, null);
			if(req1 != XVar.Pack(""))
			{
				this.includes_jsreq.InitAndSetArrayItem(new XVar(0, req1), rootPath);
			}
			if(req2 != XVar.Pack(""))
			{
				this.includes_jsreq.InitAndSetArrayItem(req2, rootPath, null);
			}
			if(req3 != XVar.Pack(""))
			{
				this.includes_jsreq.InitAndSetArrayItem(req3, rootPath, null);
			}

			return null;
		}
		public virtual XVar AddCSSFile(dynamic _param_file)
		{
			#region pass-by-value parameters
			dynamic file = XVar.Clone(_param_file);
			#endregion

			this.includes_css.InitAndSetArrayItem(file, null);

			return null;
		}
		public virtual XVar addControlsJSAndCSS()
		{
			dynamic control = null, fields = XVar.Array(), i = null;
			fields = XVar.Clone(this.pSet.getPageFields());
			i = new XVar(0);
			for(;i < MVCFunctions.count(fields); i++)
			{
				control = XVar.Clone(this.getControl((XVar)(fields[i])));
				if(XVar.Pack(!(XVar)(control)))
				{
					continue;
				}
				if(XVar.Pack(control.neededLoadJSFiles()))
				{
					control.addJSFiles();
				}
				control.addCSSFiles();
			}

			return null;
		}
		public virtual ViewControl getControl(dynamic _param_field, dynamic _param_format = null)
		{
			#region default values
			if(_param_format as Object == null) _param_format = new XVar();
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic format = XVar.Clone(_param_format);
			#endregion

			if(XVar.Pack(!(XVar)(this.viewControls.KeyExists(field))))
			{
				dynamic className = null, editFormat = null, userControl = null, vcTypes = null, viewFormat = null;
				vcTypes = XVar.Clone(new ViewControlTypes());
				editFormat = XVar.Clone(this.pSet.getEditFormat((XVar)(field)));
				if(XVar.Pack(XVar.Equals(XVar.Pack(format), XVar.Pack(null))))
				{
					dynamic localPSet = null, lookupPageType = null;
					localPSet = XVar.Clone(this.pSet);
					lookupPageType = new XVar("");
					if(XVar.Pack(!(XVar)(this.isLocal)))
					{
						lookupPageType = XVar.Clone(this.pSet.getPageTypeByFieldEditFormat((XVar)(field), new XVar(Constants.EDIT_FORMAT_LOOKUP_WIZARD)));
						if(lookupPageType != XVar.Pack(""))
						{
							localPSet = XVar.Clone(new ProjectSettings((XVar)(this.pSet._table), (XVar)(lookupPageType)));
						}
					}
					if((XVar)((XVar)((XVar)(!(XVar)(this.isLocal))  && (XVar)((XVar)(editFormat == Constants.EDIT_FORMAT_LOOKUP_WIZARD)  || (XVar)(lookupPageType != XVar.Pack(""))))  && (XVar)((XVar)(localPSet.getLookupType((XVar)(field)) == Constants.LT_LOOKUPTABLE)  || (XVar)(localPSet.getLookupType((XVar)(field)) == Constants.LT_QUERY)))  && (XVar)(localPSet.getLinkField((XVar)(field)) != localPSet.getDisplayField((XVar)(field))))
					{
						viewFormat = new XVar(Constants.FORMAT_LOOKUP_WIZARD);
					}
					else
					{
						viewFormat = XVar.Clone(this.pSet.getViewFormat((XVar)(field)));
					}
				}
				else
				{
					viewFormat = XVar.Clone(format);
				}
				className = XVar.Clone(vcTypes.viewTypes[viewFormat]);
				if((XVar)(className == XVar.Pack(""))  && (XVar)(viewFormat != XVar.Pack("")))
				{
					className = XVar.Clone(MVCFunctions.Concat("View", viewFormat));
					userControl = new XVar(true);
				}
				if(className != XVar.Pack(""))
				{
					this.viewControls.InitAndSetArrayItem(MVCFunctions.createViewControlClass((XVar)(className), (XVar)(field), this, (XVar)(this.pageObject)), field);
				}
				else
				{
					this.viewControls.InitAndSetArrayItem(new ViewControl((XVar)(field), this, (XVar)(this.pageObject)), field);
				}
				if(XVar.Pack(userControl))
				{
					this.viewControls[field].viewFormat = XVar.Clone(className);
					this.viewControls[field].init();
					this.viewControls[field].initUserControl();
				}
			}
			return XVar.UnPackViewControl(this.viewControls[field] ?? new XVar());
		}
		public virtual XVar showDBValue(dynamic _param_field, dynamic data, dynamic _param_keylink = null, dynamic _param_value = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			if(_param_value as Object == null) _param_value = new XVar("");
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic field = XVar.Clone(_param_field);
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic value = XVar.Clone(_param_value);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic control = null;
			control = XVar.Clone(this.getControl((XVar)(field)));
			if(value != XVar.Pack(""))
			{
				control.displayField = XVar.Clone(value);
			}
			return control.showDBValue((XVar)(data), (XVar)(keylink), (XVar)(html));
		}
		public virtual XVar hasUserControls()
		{
			dynamic arFields = XVar.Array();
			arFields = XVar.Clone(this.pSet.getPrinterFields());
			foreach (KeyValuePair<XVar, dynamic> arField in arFields.GetEnumerator())
			{
				if(XVar.Pack(this.getControl((XVar)(arField.Value)).isUserControl()))
				{
					return true;
				}
			}
			return false;
		}
	}
}
