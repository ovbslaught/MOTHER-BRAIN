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
	public partial class DateTimeControl : EditControl
	{
		protected static bool skipDateTimeControlCtor = false;
		public DateTimeControl(dynamic field, dynamic pageObject, dynamic id, dynamic connection) // proxy constructor
			:base((XVar)field, (XVar)pageObject, (XVar)id, (XVar)connection) {}

		public override XVar getSearchOptions(dynamic _param_selOpt, dynamic _param_not, dynamic _param_both)
		{
			#region pass-by-value parameters
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			dynamic optionsArray = XVar.Array();
			optionsArray = XVar.Clone(new XVar(0, Constants.EQUALS, 1, Constants.MORE_THAN, 2, Constants.LESS_THAN, 3, Constants.BETWEEN, 4, Constants.EMPTY_SEARCH));
			if(XVar.Pack(both))
			{
				optionsArray.InitAndSetArrayItem(Constants.NOT_EQUALS, null);
				optionsArray.InitAndSetArrayItem(Constants.NOT_MORE_THAN, null);
				optionsArray.InitAndSetArrayItem(Constants.NOT_LESS_THAN, null);
				optionsArray.InitAndSetArrayItem(Constants.NOT_BETWEEN, null);
				optionsArray.InitAndSetArrayItem(Constants.NOT_EMPTY, null);
			}
			return this.buildSearchOptions((XVar)(optionsArray), (XVar)(selOpt), (XVar)(var_not), (XVar)(both));
		}
	}
}
