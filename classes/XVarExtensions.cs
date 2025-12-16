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
	public partial class XVar
	{
		// Implicit casts to used types

		public static implicit operator XVar(ActionResult value)
		{
			return new XVar(value);
		}

		public static implicit operator ActionResult(XVar value)
		{
			var contentResult = new ContentResult();
			contentResult.Content = value.ToString();
			return contentResult;
		}

		public static implicit operator XVar(HttpPostedFile value)
		{
			return new XVar(value);
		}

		public static implicit operator HttpPostedFile(XVar value)
		{
			return (HttpPostedFile)(value.Value);
		}

	}
	public partial class XVar
	{
		// Unpackers to derived types

		public static Connection UnPackConnection(XVar value)
		{
			if(value == null) return null;
			return value is Connection ? (Connection)value : (Connection)value.Value;
		}

		public static ConnectionManager UnPackConnectionManager(XVar value)
		{
			if(value == null) return null;
			return value is ConnectionManager ? (ConnectionManager)value : (ConnectionManager)value.Value;
		}

		public static EditControl UnPackEditControl(XVar value)
		{
			if(value == null) return null;
			return value is EditControl ? (EditControl)value : (EditControl)value.Value;
		}

		public static ProjectSettings UnPackProjectSettings(XVar value)
		{
			if(value == null) return null;
			return value is ProjectSettings ? (ProjectSettings)value : (ProjectSettings)value.Value;
		}

		public static RunnerPage UnPackRunnerPage(XVar value)
		{
			if(value == null) return null;
			return value is RunnerPage ? (RunnerPage)value : (RunnerPage)value.Value;
		}

		public static SQLLogicalExpr UnPackSQLLogicalExpr(XVar value)
		{
			if(value == null) return null;
			return value is SQLLogicalExpr ? (SQLLogicalExpr)value : (SQLLogicalExpr)value.Value;
		}

		public static SQLQuery UnPackSQLQuery(XVar value)
		{
			if(value == null) return null;
			return value is SQLQuery ? (SQLQuery)value : (SQLQuery)value.Value;
		}

		public static ViewControl UnPackViewControl(XVar value)
		{
			if(value == null) return null;
			return value is ViewControl ? (ViewControl)value : (ViewControl)value.Value;
		}

		public static XTempl UnPackXTempl(XVar value)
		{
			if(value == null) return null;
			return value is XTempl ? (XTempl)value : (XTempl)value.Value;
		}

	}
}