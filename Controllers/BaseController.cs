using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Security.Principal;

namespace runnerDotNet
{
	public class ThreadContext
	{
		public System.Web.HttpContext context;
		public Exception exception;
		public WindowsIdentity identity;
	}

	public class BaseController : Controller
	{
		protected override void ExecuteCore()
		{
			var arg = new ThreadContext();
			arg.context = System.Web.HttpContext.Current;
			arg.identity = WindowsIdentity.GetCurrent();

			var thread = new Thread(ExecuteThread, 5000000); // 5mb stack

			thread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			thread.Start(arg);
			thread.Join();

			if (arg.exception != null)
				throw new AggregateException(arg.exception);
		}

		void ExecuteThread(object arg)
		{
			var context = arg as ThreadContext;

			context.identity.Impersonate();

			System.Web.HttpContext.Current = context.context;

			try
			{
				base.ExecuteCore();
			}
			catch (Exception ex)
			{
				context.exception = ex;
				Thread.Sleep(1); // force context switching (iis 5 issue #8174)
			}
			finally
			{
				if (GlobalVars.cman != null)
					GlobalVars.cman.CloseConnections();
			}
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			try
			{
				//appsettings.Apply();
				DbCommon.Apply();
			}
			catch (RunnerRedirectException ex)
			{
				filterContext.Result = Redirect(ex.Message);
			}
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			// Bail if we can't do anything; app will crash.
			if (filterContext == null)
				return;

			// get real exception
			var ex = filterContext.Exception;
			while (ex.InnerException != null && ex.GetType() == typeof(System.Reflection.TargetInvocationException))
			    ex = ex.InnerException;

			if (ex.GetType() == typeof(RunnerRedirectException) || ex.GetType() == typeof(RunnerInlineOutputException))
			{
				filterContext.ExceptionHandled = true;
				return;
			}
		}
	}
}
