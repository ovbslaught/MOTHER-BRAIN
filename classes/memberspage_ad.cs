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
	public partial class MembersPage_AD : ListPage_Lookup
	{
		public dynamic resultData = XVar.Array();
		public dynamic strFilterClause = XVar.Pack("");
		public dynamic issetRecords = XVar.Array();
		public dynamic recNumber = XVar.Pack(1);
		public dynamic plugin = XVar.Pack(null);
		public dynamic providerCode = XVar.Pack("");
		protected static bool skipMembersPage_ADCtor = false;
		private bool skipListPage_LookupCtorSurrogate = new Func<bool>(() => skipListPage_LookupCtor = true).Invoke();
		public MembersPage_AD(dynamic _param_params)
			:base((XVar)_param_params)
		{
			if(skipMembersPage_ADCtor)
			{
				skipMembersPage_ADCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic var_params = XVar.Clone(_param_params);
			#endregion

			this.issetRecords = XVar.Clone(this.getDBMembers());
			this.headerForms = XVar.Clone(new XVar(0, "top"));
			this.footerForms = XVar.Clone(new XVar(0, "below-grid"));
			this.pageData.InitAndSetArrayItem(var_params["providerCode"], "provider");
			this.plugin = XVar.Clone(Security.getAuthPlugin((XVar)(var_params["providerCode"])));
			this.pageSize = new XVar(10);
		}
		public virtual XVar getDBMembers()
		{
			dynamic dataSource = null, dc = null, members = XVar.Array(), qResult = null, tdata = XVar.Array();
			dc = XVar.Clone(new DsCommand());
			dataSource = XVar.Clone(Security.getUgGroupsDatasource());
			if((XVar)(!(XVar)(Security.ADonlyLogin()))  || (XVar)(CommonFunctions.storageGet(new XVar("groups_provider_field"))))
			{
				dc.filter = XVar.Clone(DataCondition.FieldEquals(new XVar("Provider"), (XVar)(this.providerCode)));
			}
			qResult = XVar.Clone(dataSource.getList((XVar)(dc)));
			members = XVar.Clone(XVar.Array());
			while(XVar.Pack(tdata = XVar.Clone(qResult.fetchAssoc())))
			{
				members.InitAndSetArrayItem(tdata["Label"], null);
			}
			return members;
		}
		public virtual XVar isIssetRecord(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(name), (XVar)(this.issetRecords)))))
			{
				return true;
			}
			return false;
		}
		public override XVar commonAssign()
		{
			base.commonAssign();
			this.xt.assign(new XVar("add_link"), new XVar(true));
			this.xt.assign(new XVar("addselectlink_attrs"), new XVar("id = \"addSelBtn\" "));
			this.xt.assign(new XVar("recordcontrolsad_block"), new XVar(true));
			this.xt.assign(new XVar("checkbox_header"), new XVar(true));
			this.xt.assign(new XVar("checkboxheader_attrs"), (XVar)(MVCFunctions.Concat("id=\"chooseAll_", this.id, "\" data-checked=\"0\"")));

			return null;
		}
		public override XVar isDispGrid()
		{
			return true;
		}
		public override XVar buildPagination()
		{
			base.buildPagination();
			this.recNumber = XVar.Clone(this.pageSize * (this.myPage - 1));

			return null;
		}
		public virtual XVar seekPageInRecSet()
		{
			this.resultData = XVar.Clone(this.plugin.getGroupList((XVar)(this.searchClauseObj.getAllFieldsSearchValue()), (XVar)(this.pSet.hideAdGroupsUntilSearch())));
			this.numRowsFromSQL = XVar.Clone((XVar.Pack(this.resultData) ? XVar.Pack(MVCFunctions.count(this.resultData)) : XVar.Pack(0)));
			this.recSet = XVar.Clone(this.numRowsFromSQL);

			return null;
		}
		public override XVar beforeProccessRow()
		{
			if((XVar)(this.recNumber <= this.pageSize * this.myPage - 1)  && (XVar)(this.recNumber <= this.numRowsFromSQL - 1))
			{
				return this.resultData[this.recNumber];
			}
			return false;
		}
		public override XVar fillGridData()
		{
			dynamic data = XVar.Array(), gridRowInd = null, row = XVar.Array(), rowInfo = XVar.Array();
			rowInfo = XVar.Clone(XVar.Array());
			data = XVar.Clone(this.beforeProccessRow());
			this.controlsMap.InitAndSetArrayItem(XVar.Array(), "gridRows");
			while(XVar.Pack(data))
			{
				row = XVar.Clone(XVar.Array());
				row.InitAndSetArrayItem(MVCFunctions.Concat(" id=\"gridRow", this.recNumber, "\""), "rowattrs");
				gridRowInd = XVar.Clone(MVCFunctions.count(this.controlsMap["gridRows"]));
				this.controlsMap.InitAndSetArrayItem(this.recNumber, "gridRows", gridRowInd, "id");
				this.controlsMap.InitAndSetArrayItem(gridRowInd, "gridRows", gridRowInd, "rowInd");
				this.controlsMap.InitAndSetArrayItem(new XVar("name", data["name"]), "gridRows", gridRowInd, "keys");
				row.InitAndSetArrayItem(XVar.Array(), "grid_record");
				row.InitAndSetArrayItem(XVar.Array(), "grid_record", "data");
				row.InitAndSetArrayItem(this.isIssetRecord((XVar)(data["name"])), "add_link");
				row.InitAndSetArrayItem(this.isIssetRecord((XVar)(data["name"])), "checkbox");
				row.InitAndSetArrayItem(MVCFunctions.Concat("href='#' id='iAddLink", this.recNumber, "' "), "addlink_attrs");
				row.InitAndSetArrayItem(MVCFunctions.Concat("name=\"selection[]\" value=\"", this.recNumber, "\" \r\n\t\t\t\tid=\"check", this.id, "_", this.recNumber, "\" data-checked=\"0\""), "checkbox_attrs");
				row.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(data["name"])), "username");
				row.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(data["displayname"])), "displayusername");
				row.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(data["category"])), "categoryuser");
				row.InitAndSetArrayItem(MVCFunctions.runner_htmlspecialchars((XVar)(data["email"])), "emailuser");
				row.InitAndSetArrayItem(this.recNumber, "recNo");
				this.recNumber++;
				if(XVar.Pack(this.eventExists(new XVar("BeforeMoveNextList"))))
				{
					dynamic record = XVar.Array();
					this.eventsObject.BeforeMoveNextList((XVar)(data), (XVar)(row), (XVar)(record), (XVar)(record["recId"]), this);
				}
				rowInfo.InitAndSetArrayItem(row, null);
				data = XVar.Clone(this.beforeProccessRow());
			}
			this.xt.assign_loopsection(new XVar("grid_row"), (XVar)(rowInfo));

			return null;
		}
		public override XVar prepareForBuildPage()
		{
			this.buildOrderParams();
			this.strFilterClause = XVar.Clone(this.searchClauseObj._where["_simpleSrch"]);
			this.seekPageInRecSet();
			this.buildPagination();
			this.fillGridData();
			this.assignSimpleSearch();
			this.addCommonJs();
			this.addCommonHtml();
			this.commonAssign();
			this.assignColumnHeaderClasses();

			return null;
		}
		public override XVar showPage()
		{
			this.BeforeShowList();
			this.xt.prepare_template((XVar)(this.templatefile));
			this.showPageAjax();
			MVCFunctions.ob_flush();
			HttpContext.Current.Response.End();
			throw new RunnerInlineOutputException();

			return null;
		}
		public override XVar inlineAddAvailable()
		{
			return false;
		}
		public override XVar noRecordsMessage()
		{
			if((XVar)(this.pSet.hideAdGroupsUntilSearch())  && (XVar)(!(XVar)(this.isSearchFunctionalityActivated())))
			{
				return CommonFunctions.mlang_message(new XVar("NOTHING_TO_SEE"));
			}
			return base.noRecordsMessage();
		}
		public override XVar isAdminTable()
		{
			return true;
		}
	}
}
