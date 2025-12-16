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
	public partial class ListPage_Embed : ListPage
	{
		protected static bool skipListPage_EmbedCtor = false;
		public ListPage_Embed(dynamic var_params)
			:base((XVar)var_params)
		{
			if(skipListPage_EmbedCtor)
			{
				skipListPage_EmbedCtor = false;
				return;
			}
		}
		public override XVar addCommonHtml()
		{
			base.addCommonHtml();
			this.xt.assign(new XVar("footer"), new XVar(""));

			return null;
		}
		public override XVar commonAssign()
		{
			base.commonAssign();
			if(XVar.Pack(this.isDispGrid()))
			{
				this.xt.assign_section(new XVar("grid_block"), new XVar(""), new XVar(""));
				if((XVar)(!(XVar)(this.recordsOnPage))  && (XVar)(!(XVar)(this.fieldFilterEnabled())))
				{
					this.hideElement(new XVar("grid"));
				}
			}

			return null;
		}
		public override XVar importAvailable()
		{
			return false;
		}
		public override XVar printAvailable()
		{
			return false;
		}
		public override XVar advSearchAvailable()
		{
			return false;
		}
		public override XVar exportAvailable()
		{
			return false;
		}
	}
}
