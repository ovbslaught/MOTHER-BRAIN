using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;
using runnerDotNet;

namespace runnerDotNet
{
	public class SQLite3Info : DBInfo
	{
		public SQLite3Info(Connection connObj) : base(connObj)
		{ }
	}
}