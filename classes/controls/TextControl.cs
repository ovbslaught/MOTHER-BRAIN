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
	public partial class TextControl : EditControl
	{
		protected static bool skipTextControlCtor = false;
		public TextControl(dynamic field, dynamic pageObject, dynamic id, dynamic connection) // proxy constructor
			:base((XVar)field, (XVar)pageObject, (XVar)id, (XVar)connection) {}

		public override XVar getSearchOptions(dynamic _param_selOpt, dynamic _param_not, dynamic _param_both)
		{
			#region pass-by-value parameters
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			dynamic isPHPEncripted = null, optionsArray = XVar.Array();
			optionsArray = XVar.Clone(XVar.Array());
			isPHPEncripted = XVar.Clone(this.pageObject.cipherer.isFieldPHPEncrypted((XVar)(this.field)));
			if(XVar.Pack(!(XVar)(isPHPEncripted)))
			{
				optionsArray.InitAndSetArrayItem(Constants.CONTAINS, null);
			}
			optionsArray.InitAndSetArrayItem(Constants.EQUALS, null);
			if(XVar.Pack(!(XVar)(isPHPEncripted)))
			{
				optionsArray.InitAndSetArrayItem(Constants.STARTS_WITH, null);
				optionsArray.InitAndSetArrayItem(Constants.MORE_THAN, null);
				optionsArray.InitAndSetArrayItem(Constants.LESS_THAN, null);
				optionsArray.InitAndSetArrayItem(Constants.BETWEEN, null);
			}
			optionsArray.InitAndSetArrayItem(Constants.EMPTY_SEARCH, null);
			if(XVar.Pack(both))
			{
				if(XVar.Pack(!(XVar)(isPHPEncripted)))
				{
					optionsArray.InitAndSetArrayItem(Constants.NOT_CONTAINS, null);
				}
				optionsArray.InitAndSetArrayItem(Constants.NOT_EQUALS, null);
				if(XVar.Pack(!(XVar)(isPHPEncripted)))
				{
					optionsArray.InitAndSetArrayItem(Constants.NOT_STARTS_WITH, null);
					optionsArray.InitAndSetArrayItem(Constants.NOT_MORE_THAN, null);
					optionsArray.InitAndSetArrayItem(Constants.NOT_LESS_THAN, null);
					optionsArray.InitAndSetArrayItem(Constants.NOT_BETWEEN, null);
				}
				optionsArray.InitAndSetArrayItem(Constants.NOT_EMPTY, null);
			}
			return this.buildSearchOptions((XVar)(optionsArray), (XVar)(selOpt), (XVar)(var_not), (XVar)(both));
		}
	}
}
