using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Dynamic;

namespace runnerDotNet
{
	public abstract class DynamicContentGeneratorBase : System.Web.Mvc.WebViewPage<dynamic>
	{
		private StringBuilder buffer;
		protected DynamicContentGeneratorBase()
		{
			Model = new ExpandoObject();
			webRootPath = ""; 
			/* 
			webRootPath = HttpContext.Current.Request.ApplicationPath != "/" ? HttpContext.Current.Request.ApplicationPath : "/";
			if(!webRootPath.EndsWith("/"))
				webRootPath += "/";
			*/
			
		}

		/// <summary>
		/// This is just a custom property
		/// </summary>
		public dynamic Model { get; set; }
		
		public dynamic ViewBag { get; set; }
		
		public dynamic Layout { get; set; }

		public string webRootPath { get; set; }

		/// <summary>
		/// This method is required and can be public but have to have exactly the same signature
		/// </summary>
		public void Write(object value)
		{
			WriteLiteral(value);
		}

		/// <summary>
		/// This method is required and can be public but have to have exactly the same signature
		/// </summary>
		protected void WriteLiteral(object value)
		{
			buffer.Append(value);
		}

		/// <summary>
		/// This method is just to have the rendered content without call Execute.
		/// </summary>
		/// <returns>The rendered content.</returns>
		public string GetContent()
		{
			buffer = new StringBuilder(1024); // check the buffer in the case of exception - it will contain last correctly processed data
			try
			{
				Execute();
			}
			catch(Exception ex)
			{
				throw new ApplicationException("MVC View runtime exception. " + ex.Message, ex);
			}
			return buffer.ToString();
		}
	}
}