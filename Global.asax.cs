using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace runnerDotNet
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode,
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.IgnoreRoute("favicon.ico");

			routes.IgnoreRoute("v1");

			string defController = "Global", defPage = "menu";


	//	LandingType.ltPAGE
			defController = "Landing";
			defPage = "Route";

			var landingUrl = CommonFunctions.GetIndexPage(); // get landing page from business template
			if (!new string[] {"\\", "/", ""}.Contains(landingUrl))
			{
				var urlParts = landingUrl.Split(new char [] {'\\', '/'}, StringSplitOptions.RemoveEmptyEntries);
				if(urlParts.Length == 1)
				{
					defPage = urlParts[0];
				}
				else
				{
					defController = urlParts[0];
					defPage = urlParts[1];
				}
			}

			routes.MapRoute(
				"Default", // Route name
				"", // URL with parameters
				new { controller = defController, action = defPage } // Parameter defaults
			);

			// api requests
			routes.MapRoute(
				"api", // Route name
				"api/{action}/", // URL with parameters
				new { controller = "Global" } // Parameter defaults
			);

			// enable multithreading requests
			/*
			routes.MapRoute(
				"mfhandlerSessionStatic", // Route name
				"mfhandler/", // URL with parameters
				new { controller = "SessionStaticGlobal", action = "mfhandler" } // Parameter defaults
			);
			*/

			routes.MapRoute(
				"notificationsSessionStatic", // Route name
				"notifications/", // URL with parameters
				new { controller = "SessionStaticGlobal", action = "notifications" } // Parameter defaults
			);

			routes.MapRoute(
				"fileSessionStatic", // Route name
				"file/", // URL with parameters
				new { controller = "SessionStaticGlobal", action = "file" } // Parameter defaults
			);

			routes.MapRoute(
				"comboSessionStatic", // Route name
				"combo/", // URL with parameters
				new { controller = "SessionStaticGlobal", action = "combo" } // Parameter defaults
			);
			//////////////////////////////////////

			routes.MapRoute(
				"Global", // Route name
				"{action}/", // URL with parameters
				new { controller = "Global" } // Parameter defaults
			);

			routes.MapRoute(
				"Controllers", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { id = UrlParameter.Optional } // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var exception = Server.GetLastError();

			// catch only RunnerRedirectException wrapped into two layers of System.Reflection.TargetInvocationException
			if (exception.InnerException != null && exception.InnerException.InnerException != null && exception.InnerException.InnerException is RunnerRedirectException)
			{
				Server.ClearError();
				Response.Redirect(exception.InnerException.InnerException.Message);
			}
			else
			{
				MVCFunctions.runner_error_handler(exception, sender);
			}
		}

		void Application_End(object sender, EventArgs e)
		{
			// HttpRuntime runtime = (HttpRuntime)typeof(System.Web.HttpRuntime).InvokeMember("_theRuntime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetField, null, null, null);
			// if (runtime == null)
			// 	return;
			// string shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField, null, runtime, null);
			// string shutDownStack = (string)runtime.GetType().InvokeMember("_shutDownStack", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField, null, runtime, null);
			// ApplicationShutdownReason shutdownReason = System.Web.Hosting.HostingEnvironment.ShutdownReason;
			// System.IO.File.WriteAllText("c:\\log.txt", String.Format("\r\n\r\nAPPLICATION END\r\n\r\n_shutDownReason = {2}\r\n\r\n _shutDownMessage = {0}\r\n\r\n_shutDownStack = {1}\r\n\r\n",
			// 	shutDownMessage, shutDownStack, shutdownReason));
		}
	}

	// internal-page routing
	public class LandingController : Controller
	{
		public ActionResult Route()
		{
			string tName = "", pType = "", prms = "";
			tName = "menu";
			pType = "";
			prms = "page=menu";
/*
##if @BUILDER.m_nLandingPageType == lanptPAGE##
	##if @BUILDER.m_strLandingPage == loginPAGE || @BUILDER.m_strLandingPage == menuPAGE || @BUILDER.m_strLandingPage == registerPAGE##
			tName = "##@BUILDER.m_strLandingPage##";
	##else##
		##foreach @BUILDER.Tables as @t filter @t.strDataSourceTable==@BUILDER.m_strLandingTable##
			tName = "##@t.strShortTableName##";
			pType = "##@BUILDER.m_strLandingPage##";
			##if @BUILDER.strLandingPageID ##
				prms = String.Concat("page=", "##@BUILDER.strLandingPageID##");
			##endif##
		##endfor##
	##endif##
##endif##
*/
			return Redirect( MVCFunctions.projectPath().ToString() + MVCFunctions.GetTableLink( tName, pType, prms ).ToString() );
		}
	}

	// for redirect-to-external-page routing
	public class SiteRouterController : Controller
	{
		public ActionResult Route()
		{
			return Redirect("");
		}
	}
}