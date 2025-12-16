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
	public partial class SessionExpiredPage : RunnerPage
	{
		public dynamic sessionControl;
		protected static bool skipSessionExpiredPageCtor = false;
		public SessionExpiredPage(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipSessionExpiredPageCtor)
			{
				skipSessionExpiredPageCtor = false;
				return;
			}
			this.bodyForms = XVar.Clone(new XVar(0, "above-grid", 1, "grid"));
		}
		public virtual XVar process()
		{
			this.addCommonJs();
			this.addButtonHandlers();
			this.fillSetCntrlMaps();
			this.doCommonAssignments();
			this.showPage();

			return null;
		}
		public virtual XVar doCommonAssignments()
		{
			this.setLangParams();

			return null;
		}
		public virtual XVar showPage()
		{
			if(this.mode == Constants.LOGIN_POPUP)
			{
				this.displayAJAX((XVar)(this.templatefile), (XVar)(this.id + 1));
				MVCFunctions.ob_flush();
				HttpContext.Current.Response.End();
				throw new RunnerInlineOutputException();
			}
			this.assignBody();
			this.display((XVar)(this.templatefile));

			return null;
		}
		public static XVar readExpiredModeFromRequest()
		{
			dynamic pageMode = null;
			pageMode = XVar.Clone(MVCFunctions.postvalue(new XVar("mode")));
			if(MVCFunctions.postvalue(new XVar("mode")) == "popup")
			{
				return Constants.SESSION_EXPIRED_POPUP;
			}
			return Constants.SESSION_EXPIRED_SIMPLE;
		}
	}
}
