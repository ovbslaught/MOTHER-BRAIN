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
	public partial class AdvancedSearchControl : SearchControl
	{
		protected static bool skipAdvancedSearchControlCtor = false;
		public AdvancedSearchControl(dynamic _param_id, dynamic _param_tName, dynamic searchClauseObj, dynamic pageObj)
			:base((XVar)_param_id, (XVar)_param_tName, (XVar)searchClauseObj, (XVar)pageObj)
		{
			if(skipAdvancedSearchControlCtor)
			{
				skipAdvancedSearchControlCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic id = XVar.Clone(_param_id);
			dynamic tName = XVar.Clone(_param_tName);
			#endregion

			this.getSrchPanelAttrs.InitAndSetArrayItem(true, "ctrlTypeComboStatus");
		}
		public override XVar getCtrlSearchTypeOptions(dynamic _param_fName, dynamic _param_selOpt, dynamic _param_not, dynamic _param_flexible = null, dynamic _param_both = null)
		{
			#region default values
			if(_param_flexible as Object == null) _param_flexible = new XVar(false);
			if(_param_both as Object == null) _param_both = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic fName = XVar.Clone(_param_fName);
			dynamic selOpt = XVar.Clone(_param_selOpt);
			dynamic var_not = XVar.Clone(_param_not);
			dynamic flexible = XVar.Clone(_param_flexible);
			dynamic both = XVar.Clone(_param_both);
			#endregion

			if((XVar)(!(XVar)(flexible))  && (XVar)((XVar)(selOpt == Constants.EMPTY_SEARCH)  || (XVar)(selOpt == Constants.NOT_EMPTY)))
			{
				return this.getControl((XVar)(fName)).buildSearchOptions((XVar)(new XVar(0, Constants.EMPTY_SEARCH, 1, Constants.NOT_EMPTY)), (XVar)(selOpt), (XVar)(var_not), new XVar(true));
			}
			return this.getControl((XVar)(fName)).getSearchOptions((XVar)(selOpt), (XVar)(var_not), new XVar(true));
		}
	}
}
