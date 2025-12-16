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
	public partial class GlobalController : BaseController
	{
		public XVar dchartdata()
		{
			try
			{
				dynamic chartObj = null, chartSettings = XVar.Array(), param = XVar.Array(), webchart = null;
				MVCFunctions.Header("Expires", "Thu, 01 Jan 1970 00:00:01 GMT");
				if(XVar.Pack(ProjectSettings.webReports()))
				{
				}
				if(XVar.Pack(Security.hasLogin()))
				{
					if(XVar.Pack(!(XVar)(CommonFunctions.isLogged())))
					{
						Security.saveRedirectURL();
						MVCFunctions.HeaderRedirect(new XVar("login"), new XVar(""), new XVar("message=expired"));
						return MVCFunctions.GetBuferContentAndClearBufer();
					}
				}
				chartSettings = new XVar(null);
				if(XVar.Pack(CommonFunctions.GetTableByShort((XVar)(MVCFunctions.postvalue(new XVar("chartname"))))))
				{
					ProjectSettings pSet;
					pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(CommonFunctions.GetTableByShort((XVar)(MVCFunctions.postvalue(new XVar("chartname")))))));
					chartSettings = XVar.Clone(pSet.getChartSettings());
					if(XVar.Pack(!(XVar)(XSession.Session["webobject"])))
					{
						XSession.Session["webobject"] = XVar.Array();
					}
					XSession.Session.InitAndSetArrayItem("project", "webobject", "table_type");
					XSession.Session["object_sql"] = "";
				}
				webchart = new XVar(false);
				if(XVar.Pack(!(XVar)(chartSettings)))
				{
				}
				param = XVar.Clone(XVar.Array());
				param.InitAndSetArrayItem(webchart, "webchart");
				param.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("showDetails")), "showDetails");
				param.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("chartPreview")), "chartPreview");
				param.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("dashChart")), "dashChart");
				param.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("pageId")), "pageId");
				param.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("mastertable")), "masterTable");
				if(XVar.Pack(param["masterTable"]))
				{
					param.InitAndSetArrayItem(RunnerPage.readMasterKeysFromRequest(), "masterKeysReq");
				}
				if(XVar.Pack(param["dashChart"]))
				{
					dynamic var_params = XVar.Array();
					param.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("dashTName")), "dashTName");
					param.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("dashElName")), "dashElementName");
					var_params.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("dashPage")), "dashPage");
				}
				param.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("chartname")), "cname");
				param.InitAndSetArrayItem(CommonFunctions.GetTableByShort((XVar)(MVCFunctions.postvalue(new XVar("chartname")))), "tName");
				param.InitAndSetArrayItem("project", "tableType");
				if(XVar.Pack(webchart))
				{
					param.InitAndSetArrayItem(MVCFunctions.postvalue(new XVar("cname")), "cname");
					param.InitAndSetArrayItem(chartSettings["webTableType"], "tableType");
					param.InitAndSetArrayItem(chartSettings["webTableName"], "tName");
				}
				switch(((XVar)chartSettings["type"]).ToString())
				{
					case "2DColumn":
						param.InitAndSetArrayItem(!(XVar)(chartSettings["is3D"]), "2d");
						param.InitAndSetArrayItem(false, "bar");
						param.InitAndSetArrayItem(chartSettings["stacked"], "stacked");
						chartObj = XVar.Clone(new Chart_Bar((XVar)(param), (XVar)(chartSettings)));
						break;
					case "2DBar":
						param.InitAndSetArrayItem(!(XVar)(chartSettings["is3D"]), "2d");
						param.InitAndSetArrayItem(true, "bar");
						param.InitAndSetArrayItem(chartSettings["stacked"], "stacked");
						chartObj = XVar.Clone(new Chart_Bar((XVar)(param), (XVar)(chartSettings)));
						break;
					case "Line":
						chartObj = XVar.Clone(new Chart_Line((XVar)(param), (XVar)(chartSettings)));
						break;
					case "Area":
						param.InitAndSetArrayItem(chartSettings["stacked"], "stacked");
						chartObj = XVar.Clone(new Chart_Area((XVar)(param), (XVar)(chartSettings)));
						break;
					case "2DPie":
						param.InitAndSetArrayItem(!(XVar)(chartSettings["is3D"]), "2d");
						param.InitAndSetArrayItem(true, "pie");
						chartObj = XVar.Clone(new Chart_Pie((XVar)(param), (XVar)(chartSettings)));
						break;
					case "2DDoughnut":
						param.InitAndSetArrayItem(!(XVar)(chartSettings["is3D"]), "2d");
						param.InitAndSetArrayItem(false, "pie");
						chartObj = XVar.Clone(new Chart_Pie((XVar)(param), (XVar)(chartSettings)));
						break;
					case "Combined":
						chartObj = XVar.Clone(new Chart_Combined((XVar)(param), (XVar)(chartSettings)));
						break;
					case "Funnel":
						param.InitAndSetArrayItem(chartSettings["accumulationAppearance"], "funnel_type");
						param.InitAndSetArrayItem(chartSettings["accumInverted"], "funnel_inv");
						chartObj = XVar.Clone(new Chart_Funnel((XVar)(param), (XVar)(chartSettings)));
						break;
					case "Bubble":
						param.InitAndSetArrayItem(!(XVar)(chartSettings["is3D"]), "2d");
						param.InitAndSetArrayItem(1, "oppos");
						if(XVar.Pack(chartSettings["bubbleTransparent"]))
						{
							param.InitAndSetArrayItem(0.300000, "oppos");
						}
						chartObj = XVar.Clone(new Chart_Bubble((XVar)(param), (XVar)(chartSettings)));
						break;
					case "Gauge":
						if(chartSettings["gaugeAppearance"] == 0)
						{
							param.InitAndSetArrayItem("circular-gauge", "gaugeType");
						}
						else
						{
							param.InitAndSetArrayItem("linear-gauge", "gaugeType");
						}
						if(chartSettings["gaugeAppearance"] == 2)
						{
							param.InitAndSetArrayItem("horizontal", "layout");
						}
						else
						{
							param.InitAndSetArrayItem("", "layout");
						}
						chartObj = XVar.Clone(new Chart_Gauge((XVar)(param), (XVar)(chartSettings)));
						break;
					case "OHLC":
						param.InitAndSetArrayItem("ohcl", "ohcl_type");
						chartObj = XVar.Clone(new Chart_Ohlc((XVar)(param), (XVar)(chartSettings)));
						break;
					case "Candle":
						param.InitAndSetArrayItem("candlestick", "ohcl_type");
						chartObj = XVar.Clone(new Chart_Ohlc((XVar)(param), (XVar)(chartSettings)));
						break;
				}
				if(MVCFunctions.postvalue(new XVar("action")) == "refresh")
				{
					MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(chartObj.get_data())));
					MVCFunctions.Echo(new XVar(""));
					return MVCFunctions.GetBuferContentAndClearBufer();
				}
				MVCFunctions.Header("Content-Type", "application/json");
				chartObj.write();
				return MVCFunctions.GetBuferContentAndClearBufer();
			}
			catch(RunnerRedirectException ex)
			{ return Redirect(ex.Message); }
		}
	}
}
