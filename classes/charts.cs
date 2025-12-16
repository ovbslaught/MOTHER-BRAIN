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
	public partial class Chart : XClass
	{
		protected dynamic header;
		protected dynamic footer;
		protected dynamic y_axis_label;
		protected dynamic strLabel;
		protected dynamic arrDataLabels = XVar.Array();
		protected dynamic arrDataSeries = XVar.Array();
		public dynamic webchart;
		protected dynamic cname;
		protected dynamic tableType;
		protected dynamic cipherer = XVar.Pack(null);
		protected ProjectSettings pSet = null;
		protected dynamic searchClauseObj = XVar.Pack(null);
		protected dynamic sessionPrefix = XVar.Pack("");
		protected dynamic pageId;
		protected dynamic showDetails = XVar.Pack(true);
		protected dynamic chartPreview = XVar.Pack(false);
		protected dynamic dashChart = XVar.Pack(false);
		protected dynamic dashChartFirstPointSelected = XVar.Pack(false);
		protected dynamic detailMasterKeys = XVar.Pack("");
		protected dynamic dashTName = XVar.Pack("");
		protected dynamic dashElementName = XVar.Pack("");
		protected dynamic connection;
		protected dynamic _2d;
		protected dynamic noRecordsFound = XVar.Pack(false);
		protected dynamic singleSeries = XVar.Pack(false);
		protected dynamic masterKeysReq;
		protected dynamic masterTable;
		protected dynamic dataSource = XVar.Pack(null);
		protected dynamic tName = XVar.Pack("");
		protected dynamic chartSettings;
		protected dynamic webChartColors = XVar.Array();
		public Chart(dynamic _param_param, dynamic chartSettings)
		{
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			#endregion

			dynamic eventObj = null;
			this.chartSettings = XVar.Clone(chartSettings);
			this.webchart = XVar.Clone(param["webchart"]);
			if(XVar.Pack(this.webchart))
			{
				this.webChartColors = XVar.Clone(chartSettings["webColors"]);
			}
			this.tName = XVar.Clone(param["tName"]);
			this.tableType = XVar.Clone(param["tableType"]);
			this.sessionPrefix = XVar.Clone(param["tName"]);
			this.pSet = XVar.UnPackProjectSettings(new ProjectSettings((XVar)(this.tName), new XVar(Constants.PAGE_CHART)));
			this.setConnection();
			if(this.tableType == "project")
			{
				this.dataSource = XVar.Clone(CommonFunctions.getDataSource((XVar)(this.tName), (XVar)(this.pSet), (XVar)(this.connection)));
			}
			else
			{
				this.dataSource = XVar.Clone(CommonFunctions.getWebDataSource((XVar)(chartSettings["webSql"]), (XVar)(this.tableType), (XVar)(this.tName)));
			}
			this.showDetails = XVar.Clone(param["showDetails"]);
			this.pageId = XVar.Clone(param["pageId"]);
			this.cname = XVar.Clone(param["cname"]);
			this.masterTable = XVar.Clone(param["masterTable"]);
			this.masterKeysReq = XVar.Clone(param["masterKeysReq"]);
			this.chartPreview = XVar.Clone(param["chartPreview"]);
			this.dashChart = XVar.Clone(param["dashChart"]);
			if(XVar.Pack(this.dashChart))
			{
				this.dashTName = XVar.Clone(param["dashTName"]);
				this.dashElementName = XVar.Clone(param["dashElementName"]);
				this.sessionPrefix = XVar.Clone(MVCFunctions.Concat(this.dashTName, "_", this.sessionPrefix));
			}
			if((XVar)((XVar)(!(XVar)(this.webchart))  && (XVar)(!(XVar)(this.chartPreview)))  && (XVar)(XSession.Session.KeyExists(MVCFunctions.Concat(this.sessionPrefix, "_advsearch"))))
			{
				this.searchClauseObj = XVar.Clone(SearchClause.UnserializeObject((XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_advsearch")])));
			}
			if(XVar.Pack(this.searchClauseObj))
			{
				RunnerContext.pushSearchContext((XVar)(this.searchClauseObj));
			}
			if(XVar.Pack(this.isProjectDB()))
			{
				this.cipherer = XVar.Clone(new RunnerCipherer((XVar)(this.tName)));
			}
			this.setBasicChartProp();
			eventObj = XVar.Clone(CommonFunctions.getEventObject((XVar)(this.pSet)));
			if(XVar.Pack(eventObj.exists(new XVar("UpdateChartSettings"))))
			{
				eventObj.UpdateChartSettings(this);
			}
		}
		protected virtual XVar setBasicChartProp()
		{
			this.header = XVar.Clone(CommonFunctions.GetMLString((XVar)(this.chartSettings["header"])));
			this.footer = XVar.Clone(CommonFunctions.GetMLString((XVar)(this.chartSettings["footer"])));
			this.arrDataSeries = XVar.Clone(this.chartSettings["dataSeries"]);
			foreach (KeyValuePair<XVar, dynamic> series in this.arrDataSeries.GetEnumerator())
			{
				dynamic fieldName = null;
				if(XVar.Pack(!(XVar)(this.webchart)))
				{
					fieldName = XVar.Clone(series.Value["dataField"]);
					if((XVar)(this.chartSettings["type"] == "Candle")  || (XVar)(this.chartSettings["type"] == "OHLC"))
					{
						fieldName = XVar.Clone(series.Value["open"]);
					}
					this.arrDataLabels.InitAndSetArrayItem(CommonFunctions.GetFieldLabel((XVar)(MVCFunctions.GoodFieldName((XVar)(this.tName))), (XVar)(MVCFunctions.GoodFieldName((XVar)(fieldName)))), null);
				}
				else
				{
					if(XVar.Pack(series.Value["label"]))
					{
						this.arrDataLabels.InitAndSetArrayItem(series.Value["label"], null);
					}
					if((XVar)(this.chartSettings["type"] == "Candle")  || (XVar)(this.chartSettings["type"] == "OHLC"))
					{
						fieldName = XVar.Clone(series.Value["open"]);
					}
				}
			}
			this.strLabel = XVar.Clone(this.chartSettings["labelField"]);
			if(MVCFunctions.count(this.arrDataLabels) == 1)
			{
				this.y_axis_label = XVar.Clone(this.arrDataLabels[0]);
			}
			else
			{
				this.y_axis_label = XVar.Clone(CommonFunctions.GetMLString((XVar)(this.chartSettings["yAxisLabel"])));
			}

			return null;
		}
		protected virtual XVar getMasterCondition()
		{
			dynamic conditions = XVar.Array(), detailKeys = XVar.Array();
			if(XVar.Pack(this.dashChart))
			{
				return null;
			}
			detailKeys = XVar.Clone(this.pSet.getMasterKeys((XVar)(this.masterTable)));
			if(XVar.Pack(!(XVar)(detailKeys["detailsKeys"])))
			{
				return null;
			}
			conditions = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in detailKeys["detailsKeys"].GetEnumerator())
			{
				conditions.InitAndSetArrayItem(DataCondition.FieldEquals((XVar)(field.Value), (XVar)(this.masterKeysReq[field.Key + 1])), null);
			}
			return DataCondition._And((XVar)(conditions));
		}
		public virtual XVar getSubsetDataCommand(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic dc = null, orderObject = null;
			dc = XVar.Clone(new DsCommand());
			dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, Security.SelectCondition(new XVar("S"), (XVar)(this.pSet)), 1, this.getMasterCondition()))));
			if((XVar)(!(XVar)(this.chartPreview))  && (XVar)(this.searchClauseObj))
			{
				dynamic filter = null, search = null;
				search = XVar.Clone(this.searchClauseObj.getSearchDataCondition());
				filter = XVar.Clone(this.searchClauseObj.getFilterCondition((XVar)(this.pSet)));
				dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, search, 2, filter))));
			}
			if(XVar.Pack(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_chartTabWhere")]))
			{
				dc.filter = XVar.Clone(DataCondition._And((XVar)(new XVar(0, dc.filter, 1, DataCondition.SQLCondition((XVar)(XSession.Session[MVCFunctions.Concat(this.sessionPrefix, "_chartTabWhere")]))))));
			}
			orderObject = XVar.Clone(new OrderClause((XVar)(this.pSet), (XVar)(this.cipherer), (XVar)(this.sessionPrefix), (XVar)(this.connection)));
			dc.order = XVar.Clone(orderObject.getOrderFields());
			if(XVar.Pack(this.pSet.getRecordsLimit()))
			{
				dc.reccount = XVar.Clone(this.pSet.getRecordsLimit());
			}
			if(XVar.Pack(this.pSet.groupChart()))
			{
				dc.totals = XVar.Clone(this.getGroupChartCommandTotals());
			}
			return dc;
		}
		protected virtual XVar getGroupChartCommandTotals()
		{
			dynamic _totals = XVar.Array(), fields = XVar.Array(), orderInfo = XVar.Array(), series = XVar.Array(), totals = XVar.Array();
			totals = XVar.Clone(XVar.Array());
			totals.InitAndSetArrayItem(new XVar("alias", this.pSet.chartLabelField(), "field", this.pSet.chartLabelField(), "modifier", this.pSet.chartLabelInterval()), null);
			series = XVar.Clone(this.pSet.chartSeries());
			foreach (KeyValuePair<XVar, dynamic> s in series.GetEnumerator())
			{
				totals.InitAndSetArrayItem(new XVar("alias", s.Value["dataField"], "field", s.Value["dataField"], "total", MVCFunctions.strtolower((XVar)(s.Value["total"]))), null);
			}
			orderInfo = XVar.Clone(this.pSet.getOrderIndexes());
			if(XVar.Pack(!(XVar)(orderInfo)))
			{
				return totals;
			}
			fields = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> o in orderInfo.GetEnumerator())
			{
				fields.InitAndSetArrayItem(this.pSet.GetFieldByIndex((XVar)(o.Value[0])), null);
			}
			foreach (KeyValuePair<XVar, dynamic> t in totals.GetEnumerator())
			{
				if(XVar.Pack(!(XVar)(MVCFunctions.in_array((XVar)(t.Value["field"]), (XVar)(fields)))))
				{
					fields.InitAndSetArrayItem(t.Value["field"], null);
				}
				foreach (KeyValuePair<XVar, dynamic> o in orderInfo.GetEnumerator())
				{
					dynamic fieldIdx = null;
					fieldIdx = XVar.Clone(this.pSet.getFieldIndex((XVar)(t.Value["field"])));
					if(fieldIdx == o.Value[0])
					{
						totals.InitAndSetArrayItem(o.Value[1], t.Key, "direction");
						break;
					}
				}
			}
			_totals = XVar.Clone(XVar.Array());
			foreach (KeyValuePair<XVar, dynamic> field in fields.GetEnumerator())
			{
				foreach (KeyValuePair<XVar, dynamic> t in totals.GetEnumerator())
				{
					if(t.Value["field"] == field.Value)
					{
						_totals.InitAndSetArrayItem(t.Value, null);
					}
				}
			}
			return _totals;
		}
		protected virtual XVar isProjectDB()
		{
			if(XVar.Pack(!(XVar)(this.webchart)))
			{
				return true;
			}
			foreach (KeyValuePair<XVar, dynamic> t in ProjectSettings.getProjectTables().GetEnumerator())
			{
				if(t.Value["originalTable"] == this.tName)
				{
					return true;
				}
			}
			return false;
		}
		protected virtual XVar setConnection()
		{
			if(XVar.Pack(this.isProjectDB()))
			{
				this.connection = XVar.Clone(GlobalVars.cman.byTable((XVar)(this.tName)));
			}
			else
			{
				this.connection = XVar.Clone(GlobalVars.cman.getDefault());
			}

			return null;
		}
		public virtual XVar setFooter(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			this.footer = XVar.Clone(name);

			return null;
		}
		public virtual XVar getFooter()
		{
			return this.footer;
		}
		public virtual XVar setHeader(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			this.header = XVar.Clone(name);

			return null;
		}
		public virtual XVar getHeader()
		{
			return this.header;
		}
		public virtual XVar setLabelField(dynamic _param_name)
		{
			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			#endregion

			this.strLabel = XVar.Clone(name);

			return null;
		}
		public virtual XVar getLabelField()
		{
			return this.strLabel;
		}
		protected virtual XVar getDetailedTooltipMessage()
		{
			dynamic details = XVar.Array(), showClickHere = null;
			if(XVar.Pack(!(XVar)(this.showDetails)))
			{
				return "";
			}
			showClickHere = new XVar(true);
			if(XVar.Pack(this.dashChart))
			{
				dynamic arrDElem = XVar.Array(), pDSet = null;
				showClickHere = new XVar(false);
				pDSet = XVar.Clone(new ProjectSettings((XVar)(this.dashTName)));
				arrDElem = XVar.Clone(pDSet.getDashboardElements());
				foreach (KeyValuePair<XVar, dynamic> elem in arrDElem.GetEnumerator())
				{
					if((XVar)(elem.Value["table"] == this.tName)  && (XVar)(!(XVar)(!(XVar)(elem.Value["details"]))))
					{
						showClickHere = new XVar(true);
					}
				}
			}
			details = XVar.Clone(this.pSet.getAvailableDetailsTables());
			if((XVar)(showClickHere)  && (XVar)(MVCFunctions.count(details)))
			{
				dynamic tableCaption = null;
				tableCaption = XVar.Clone(Labels.getTableCaption((XVar)(details[0])));
				tableCaption = XVar.Clone((XVar.Pack(tableCaption) ? XVar.Pack(tableCaption) : XVar.Pack(details[0])));
				return MVCFunctions.Concat("\nClick here to see ", tableCaption, " details");
			}
			return "";
		}
		protected virtual XVar getNoDataMessage()
		{
			if(XVar.Pack(!(XVar)(this.noRecordsFound)))
			{
				return "";
			}
			if(XVar.Pack(!(XVar)(this.searchClauseObj)))
			{
				return CommonFunctions.mlang_message(new XVar("NO_DATA_YET"));
			}
			if(XVar.Pack(this.searchClauseObj.isSearchFunctionalityActivated()))
			{
				return CommonFunctions.mlang_message(new XVar("NO_RECORDS"));
			}
			return CommonFunctions.mlang_message(new XVar("NO_DATA_YET"));
		}
		public virtual XVar write()
		{
			dynamic chart = XVar.Array(), data = XVar.Array();
			data = XVar.Clone(XVar.Array());
			chart = XVar.Clone(XVar.Array());
			this.setTypeSpecChartSettings((XVar)(chart));
			if((XVar)(this.webChartColors["color71"] != "")  || (XVar)(this.webChartColors["color91"] != ""))
			{
				chart.InitAndSetArrayItem(XVar.Array(), "background");
			}
			if(this.webChartColors["color71"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color71"]), "background", "fill");
			}
			if(this.webChartColors["color91"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color91"]), "background", "stroke");
			}
			if(XVar.Pack(this.noRecordsFound))
			{
				data.InitAndSetArrayItem(this.getNoDataMessage(), "noDataMessage");
				MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(data)));
				return null;
			}
			if(XVar.Pack(this.chartSettings["animation"]))
			{
				chart.InitAndSetArrayItem(new XVar("enabled", "true", "duration", 1000), "animation");
			}
			if((XVar)(this.chartSettings["legend"] == "true")  && (XVar)(!(XVar)(this.chartPreview)))
			{
				chart.InitAndSetArrayItem(new XVar("enabled", "true"), "legend");
			}
			else
			{
				chart.InitAndSetArrayItem(new XVar("enabled", false), "legend");
			}
			chart.InitAndSetArrayItem(false, "credits");
			chart.InitAndSetArrayItem(new XVar("enabled", "true", "text", this.header), "title");
			if(this.webChartColors["color101"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color101"]), "title", "fontColor");
			}
			data.InitAndSetArrayItem(chart, "chart");
			MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(data)));

			return null;
		}
		protected virtual XVar setTypeSpecChartSettings(dynamic chart)
		{

			return null;
		}
		protected virtual XVar getGrids()
		{
			dynamic grids = XVar.Array();
			grids = XVar.Clone(XVar.Array());
			if(XVar.Pack(this.chartSettings["grid"]))
			{
				dynamic grid0 = XVar.Array(), stroke = null;
				stroke = XVar.Clone((XVar.Pack(this.webChartColors["color121"] != "") ? XVar.Pack(MVCFunctions.Concat("#", this.webChartColors["color121"])) : XVar.Pack("#ddd")));
				grid0 = XVar.Clone(new XVar("enabled", true, "drawLastLine", false, "stroke", stroke, "scale", 0, "axis", 0));
				if(this.webChartColors["color81"] != "")
				{
					grid0.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color81"]), "oddFill");
					grid0.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color81"]), "evenFill");
				}
				grids.InitAndSetArrayItem(grid0, null);
				grids.InitAndSetArrayItem(new XVar("enabled", true, "drawLastLine", false, "stroke", stroke, "axis", 1), null);
			}
			return grids;
		}
		protected virtual XVar labelFormat(dynamic _param_fieldName, dynamic _param_data, dynamic _param_truncated = null)
		{
			#region default values
			if(_param_truncated as Object == null) _param_truncated = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic fieldName = XVar.Clone(_param_fieldName);
			dynamic data = XVar.Clone(_param_data);
			dynamic truncated = XVar.Clone(_param_truncated);
			#endregion

			dynamic value = null, viewControls = null;
			if(XVar.Pack(!(XVar)(fieldName)))
			{
				return "";
			}
			if((XVar)(this.tableType == "db")  && (XVar)(!(XVar)(!(XVar)(this.chartSettings["webCustomLabels"]))))
			{
				fieldName = XVar.Clone(this.chartSettings["webCustomLabels"][fieldName]);
			}
			viewControls = XVar.Clone(new ViewControlsContainer((XVar)(this.pSet), new XVar(Constants.PAGE_CHART)));
			if(XVar.Pack(this.pSet.groupChart()))
			{
				dynamic interval = null;
				interval = XVar.Clone(this.pSet.chartLabelInterval());
				if(XVar.Pack(interval))
				{
					dynamic fType = null;
					fType = XVar.Clone(this.pSet.getFieldType((XVar)(fieldName)));
					return RunnerPage.formatGroupValueStatic((XVar)(fieldName), (XVar)(interval), (XVar)(data[fieldName]), (XVar)(this.pSet), (XVar)(viewControls), new XVar(false));
				}
			}
			value = XVar.Clone(viewControls.showDBValue((XVar)(fieldName), (XVar)(data), new XVar(""), new XVar(""), new XVar(false)));
			if((XVar)(truncated)  && (XVar)(50 < MVCFunctions.strlen((XVar)(value))))
			{
				value = XVar.Clone(MVCFunctions.Concat(MVCFunctions.runner_substr((XVar)(value), new XVar(0), new XVar(47)), "..."));
			}
			return value;
		}
		protected virtual XVar beforeQueryEvent(dynamic dc)
		{
			dynamic eventsObject = null, order = null, prep = XVar.Array(), sql = null, where = null;
			eventsObject = XVar.Clone(CommonFunctions.getEventObject((XVar)(this.pSet)));
			if(XVar.Pack(!(XVar)(eventsObject)))
			{
				return null;
			}
			if(XVar.Pack(!(XVar)(eventsObject.exists(new XVar("BeforeQueryChart")))))
			{
				return null;
			}
			prep = XVar.Clone(this.dataSource.prepareSQL((XVar)(dc)));
			where = XVar.Clone(prep["where"]);
			sql = XVar.Clone(prep["sql"]);
			order = XVar.Clone(prep["order"]);
			eventsObject.BeforeQueryChart((XVar)(sql), ref where, ref order);
			if(sql != prep["sql"])
			{
				this.dataSource.overrideSQL((XVar)(dc), (XVar)(sql));
			}
			else
			{
				if(where != prep["where"])
				{
					this.dataSource.overrideWhere((XVar)(dc), (XVar)(where));
				}
				if(order != prep["order"])
				{
					this.dataSource.overrideOrder((XVar)(dc), (XVar)(order));
				}
			}

			return null;
		}
		public virtual XVar get_data()
		{
			dynamic clickdata = XVar.Array(), data = XVar.Array(), dc = null, i = null, row = null, rs = null, series = XVar.Array(), strLabelFormat = null;
			data = XVar.Clone(XVar.Array());
			clickdata = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.arrDataSeries); i++)
			{
				data.InitAndSetArrayItem(XVar.Array(), i);
				clickdata.InitAndSetArrayItem(XVar.Array(), i);
			}
			dc = XVar.Clone(this.getSubsetDataCommand());
			this.beforeQueryEvent((XVar)(dc));
			if(XVar.Pack(this.pSet.groupChart()))
			{
				rs = XVar.Clone(this.dataSource.getTotals((XVar)(dc)));
			}
			else
			{
				rs = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			}
			if(XVar.Pack(!(XVar)(rs)))
			{
				MVCFunctions.showError((XVar)(this.dataSource.lastError()));
			}
			row = XVar.Clone(rs.fetchAssoc());
			if(XVar.Pack(this.cipherer))
			{
				row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(row)));
			}
			if(XVar.Pack(!(XVar)(row)))
			{
				this.noRecordsFound = new XVar(true);
			}
			while(XVar.Pack(row))
			{
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.arrDataSeries); i++)
				{
					data.InitAndSetArrayItem(this.getPoint((XVar)(i), (XVar)(row)), i, null);
					strLabelFormat = XVar.Clone(this.labelFormat((XVar)(this.strLabel), (XVar)(row)));
					clickdata.InitAndSetArrayItem(this.getActions((XVar)(row), (XVar)(this.arrDataSeries[i]), (XVar)(strLabelFormat)), i, null);
				}
				row = XVar.Clone(rs.fetchAssoc());
				if(XVar.Pack(this.cipherer))
				{
					row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(row)));
				}
			}
			series = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.arrDataSeries); i++)
			{
				series.InitAndSetArrayItem(this.getSeriesData((XVar)(this.arrDataLabels[i]), (XVar)(data[i]), (XVar)(clickdata[i]), (XVar)(i), (XVar)(1 < MVCFunctions.count(this.arrDataSeries))), null);
			}
			return series;
		}
		protected virtual XVar getPoint(dynamic _param_seriesNumber, dynamic _param_row)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			dynamic row = XVar.Clone(_param_row);
			#endregion

			dynamic fieldName = null, formattedValue = null, strDataSeries = null, strLabelFormat = null, viewControls = null;
			strLabelFormat = XVar.Clone(this.labelFormat((XVar)(this.strLabel), (XVar)(row)));
			viewControls = XVar.Clone(new ViewControlsContainer((XVar)(this.pSet), new XVar(Constants.PAGE_CHART)));
			strDataSeries = XVar.Clone(row[this.arrDataSeries[seriesNumber]["dataField"]]);
			fieldName = XVar.Clone(this.arrDataSeries[seriesNumber]["dataField"]);
			formattedValue = XVar.Clone(viewControls.showDBValue((XVar)(fieldName), (XVar)(row), new XVar(""), new XVar(""), new XVar(false)));
			if((XVar)(this.tableType != "db")  || (XVar)(!(XVar)(this.chartSettings["webCustomLabels"])))
			{
				strDataSeries = XVar.Clone(row[this.arrDataSeries[seriesNumber]["dataField"]]);
				fieldName = XVar.Clone(this.arrDataSeries[seriesNumber]["dataField"]);
				formattedValue = XVar.Clone(viewControls.showDBValue((XVar)(fieldName), (XVar)(row), new XVar(""), new XVar(""), new XVar(false)));
			}
			else
			{
				strDataSeries = XVar.Clone(row[this.chartSettings["webCustomLabels"][this.arrDataSeries[seriesNumber]["dataField"]]]);
				fieldName = XVar.Clone(this.chartSettings["webCustomLabels"][this.arrDataSeries[seriesNumber]["dataField"]]);
				formattedValue = XVar.Clone(viewControls.showDBValue((XVar)(fieldName), (XVar)(row), new XVar(""), new XVar(""), new XVar(false)));
			}
			return new XVar("x", strLabelFormat, "value", (double)MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)(strDataSeries)), "viewAsValue", formattedValue);
		}
		protected virtual XVar getSeriesData(dynamic _param_name, dynamic _param_pointsData, dynamic _param_clickData, dynamic _param_seriesNumber, dynamic _param_multiSeries = null)
		{
			#region default values
			if(_param_multiSeries as Object == null) _param_multiSeries = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic pointsData = XVar.Clone(_param_pointsData);
			dynamic clickData = XVar.Clone(_param_clickData);
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			dynamic multiSeries = XVar.Clone(_param_multiSeries);
			#endregion

			dynamic data = XVar.Array();
			data = XVar.Clone(new XVar("name", name, "data", pointsData, "xScale", "0", "yScale", "1", "seriesType", this.getSeriesType((XVar)(seriesNumber))));
			data.InitAndSetArrayItem(new XVar("enabled", this.chartSettings["values"], "format", "{%viewAsValue}"), "labels");
			if(this.webChartColors["color61"] != "")
			{
				data.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color61"]), "labels", "fontColor");
			}
			if((XVar)(clickData)  && (XVar)(this.pSet.getDetailsTables()))
			{
				data.InitAndSetArrayItem(clickData, "clickData");
			}
			data.InitAndSetArrayItem(this.getSeriesTooltip((XVar)(multiSeries)), "tooltip");
			return data;
		}
		protected virtual XVar getSeriesTooltip(dynamic _param_multiSeries)
		{
			#region pass-by-value parameters
			dynamic multiSeries = XVar.Clone(_param_multiSeries);
			#endregion

			return new XVar("enabled", true, "format", MVCFunctions.Concat("{%seriesName}: {%viewAsValue}", this.getDetailedTooltipMessage()));

			return null;
		}
		protected virtual XVar getSeriesType(dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			return "column";
		}
		protected virtual XVar getActions(dynamic _param_data, dynamic _param_seriesId, dynamic _param_pointId)
		{
			#region pass-by-value parameters
			dynamic data = XVar.Clone(_param_data);
			dynamic seriesId = XVar.Clone(_param_seriesId);
			dynamic pointId = XVar.Clone(_param_pointId);
			#endregion

			dynamic details = null, detailsKeys = XVar.Array(), detailsTables = XVar.Array(), masterquery = null;
			detailsTables = XVar.Clone(this.pSet.getAvailableDetailsTables());
			if(XVar.Pack(!(XVar)(MVCFunctions.count(detailsTables))))
			{
				return null;
			}
			if(XVar.Pack(this.dashChart))
			{
				dynamic masterKeysArr = XVar.Array();
				masterKeysArr = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> _details in detailsTables.GetEnumerator())
				{
					detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(_details.Value)));
					foreach (KeyValuePair<XVar, dynamic> mk in detailsKeys["masterKeys"].GetEnumerator())
					{
						masterKeysArr.InitAndSetArrayItem(new XVar(MVCFunctions.Concat("masterkey", mk.Key + 1), data[mk.Value]), _details.Value);
					}
				}
				if(XVar.Pack(!(XVar)(this.dashChartFirstPointSelected)))
				{
					this.dashChartFirstPointSelected = new XVar(true);
					this.detailMasterKeys = XVar.Clone(MVCFunctions.runner_json_encode((XVar)(masterKeysArr)));
				}
				return new XVar("masterKeys", masterKeysArr, "seriesId", seriesId, "pointId", pointId);
			}
			details = XVar.Clone(detailsTables[0]);
			detailsKeys = XVar.Clone(this.pSet.getDetailsKeys((XVar)(details)));
			masterquery = XVar.Clone(MVCFunctions.Concat("mastertable=", MVCFunctions.RawUrlEncode((XVar)(GlobalVars.strTableName))));
			foreach (KeyValuePair<XVar, dynamic> mk in detailsKeys["masterKeys"].GetEnumerator())
			{
				masterquery = MVCFunctions.Concat(masterquery, "&masterkey", mk.Key + 1, "=", MVCFunctions.RawUrlEncode((XVar)(data[mk.Value])));
			}
			return new XVar("url", MVCFunctions.GetTableLink((XVar)(CommonFunctions.GetTableURL((XVar)(details))), (XVar)(ProjectSettings.defaultPageType((XVar)(CommonFunctions.GetEntityType((XVar)(details))))), (XVar)(masterquery)));
		}
	}
	public partial class Chart_Bar : Chart
	{
		protected dynamic stacked;
		protected dynamic bar;
		protected static bool skipChart_BarCtor = false;
		public Chart_Bar(dynamic _param_param, dynamic _param_chartSettings)
			:base((XVar)_param_param, (XVar)_param_chartSettings)
		{
			if(skipChart_BarCtor)
			{
				skipChart_BarCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			dynamic chartSettings = XVar.Clone(_param_chartSettings);
			#endregion

			this.stacked = XVar.Clone(param["stacked"]);
			this._2d = XVar.Clone(param["2d"]);
			this.bar = XVar.Clone(param["bar"]);
		}
		protected override XVar getSeriesType(dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			if(XVar.Pack(this.bar))
			{
				return "bar";
			}
			else
			{
				return "column";
			}

			return null;
		}
		protected override XVar setTypeSpecChartSettings(dynamic chart)
		{
			chart.InitAndSetArrayItem(this.get_data(), "series");
			chart.InitAndSetArrayItem(this.getScales(), "scales");
			chart.InitAndSetArrayItem(this.chartSettings["logarithm"], "logarithm");
			if(XVar.Pack(this.bar))
			{
				chart.InitAndSetArrayItem("bar", "type");
			}
			else
			{
				chart.InitAndSetArrayItem("column", "type");
			}
			if(XVar.Pack(!(XVar)(this._2d)))
			{
				chart["type"] = MVCFunctions.Concat(chart["type"], "-3d");
			}
			chart.InitAndSetArrayItem(0, "xScale");
			chart.InitAndSetArrayItem(1, "yScale");
			chart.InitAndSetArrayItem(this.getGrids(), "grids");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", this.y_axis_label)), "yAxes");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", new XVar("text", this.footer), "labels", new XVar("enabled", this.chartSettings["names"]))), "xAxes");
			if(this.webChartColors["color51"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color51"]), "xAxes", 0, "labels", "fontColor");
			}
			if(this.webChartColors["color111"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color111"]), "xAxes", 0, "title", "fontColor");
			}
			if(this.webChartColors["color131"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color131"]), "xAxes", 0, "stroke");
			}
			if(this.webChartColors["color141"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color141"]), "yAxes", 0, "stroke");
			}

			return null;
		}
		protected virtual XVar getScales()
		{
			if((XVar)(this.stacked)  || (XVar)(this.chartSettings["logarithmic"]))
			{
				dynamic arr = XVar.Array();
				arr = XVar.Clone(XVar.Array());
				if(XVar.Pack(this.stacked))
				{
					arr.InitAndSetArrayItem("value", "stackMode");
				}
				if(XVar.Pack(this.chartSettings["logarithmic"]))
				{
					arr.InitAndSetArrayItem(10, "logBase");
					arr.InitAndSetArrayItem("log", "type");
				}
				return new XVar(0, new XVar("names", XVar.Array()), 1, arr);
			}
			return XVar.Array();
		}
	}
	public partial class Chart_Line : Chart
	{
		protected dynamic type_line;
		protected static bool skipChart_LineCtor = false;
		public Chart_Line(dynamic _param_param, dynamic _param_chartSettings)
			:base((XVar)_param_param, (XVar)_param_chartSettings)
		{
			if(skipChart_LineCtor)
			{
				skipChart_LineCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			dynamic chartSettings = XVar.Clone(_param_chartSettings);
			#endregion

			if(chartSettings["linestyle"] == 0)
			{
				this.type_line = new XVar("line");
			}
			else
			{
				if(chartSettings["linestyle"] == 2)
				{
					this.type_line = new XVar("step_line");
				}
				else
				{
					this.type_line = new XVar("spline");
				}
			}
			this.type_line = XVar.Clone(param["type_line"]);
		}
		protected override XVar setTypeSpecChartSettings(dynamic chart)
		{
			chart.InitAndSetArrayItem(this.get_data(), "series");
			chart.InitAndSetArrayItem("line", "type");
			chart.InitAndSetArrayItem(0, "xScale");
			chart.InitAndSetArrayItem(1, "yScale");
			chart.InitAndSetArrayItem(this.getGrids(), "grids");
			chart.InitAndSetArrayItem(this.chartSettings["logarithm"], "logarithm");
			chart.InitAndSetArrayItem(new XVar("displayMode", "single"), "tooltip");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", this.y_axis_label)), "yAxes");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", new XVar("text", this.footer), "labels", new XVar("enabled", this.chartSettings["names"]))), "xAxes");
			if(this.webChartColors["color51"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color51"]), "xAxes", 0, "labels", "fontColor");
			}
			if(this.webChartColors["color111"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color111"]), "xAxes", 0, "title", "fontColor");
			}
			if(this.webChartColors["color131"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color131"]), "xAxes", 0, "stroke");
			}
			if(this.webChartColors["color141"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color141"]), "yAxes", 0, "stroke");
			}

			return null;
		}
		protected override XVar getSeriesType(dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			switch(((XVar)this.type_line).ToString())
			{
				case "line":
					return "line";
				case "spline":
					return "spline";
				case "step_line":
					return "stepLine";
				default:
					return "line";
			}

			return null;
		}
	}
	public partial class Chart_Area : Chart
	{
		protected dynamic stacked;
		protected static bool skipChart_AreaCtor = false;
		public Chart_Area(dynamic _param_param, dynamic _param_chartSettings)
			:base((XVar)_param_param, (XVar)_param_chartSettings)
		{
			if(skipChart_AreaCtor)
			{
				skipChart_AreaCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			dynamic chartSettings = XVar.Clone(_param_chartSettings);
			#endregion

			this.stacked = XVar.Clone(param["stacked"]);
		}
		protected override XVar setTypeSpecChartSettings(dynamic chart)
		{
			chart.InitAndSetArrayItem(this.get_data(), "series");
			if(XVar.Pack(this.stacked))
			{
				chart.InitAndSetArrayItem(this.getScales(), "scales");
			}
			chart.InitAndSetArrayItem("area", "type");
			chart.InitAndSetArrayItem(0, "xScale");
			chart.InitAndSetArrayItem(1, "yScale");
			chart.InitAndSetArrayItem(this.chartSettings["logarithm"], "logarithm");
			chart.InitAndSetArrayItem(this.getGrids(), "grids");
			chart.InitAndSetArrayItem(new XVar("displayMode", "single"), "tooltip");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", this.y_axis_label)), "yAxes");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", new XVar("text", this.footer), "labels", new XVar("enabled", this.chartSettings["appearance"]["names"]))), "xAxes");
			if(this.webChartColors["color51"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color51"]), "xAxes", 0, "labels", "fontColor");
			}
			if(this.webChartColors["color111"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color111"]), "xAxes", 0, "title", "fontColor");
			}
			if(this.webChartColors["color131"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color131"]), "xAxes", 0, "stroke");
			}
			if(this.webChartColors["color141"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color141"]), "yAxes", 0, "stroke");
			}

			return null;
		}
		protected override XVar getSeriesType(dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			return "area";
		}
		protected virtual XVar getScales()
		{
			if(XVar.Pack(this.stacked))
			{
				dynamic arr = XVar.Array();
				arr = XVar.Clone(XVar.Array());
				arr.InitAndSetArrayItem("value", "stackMode");
				if(XVar.Pack(this.chartSettings["stacked"]))
				{
					arr.InitAndSetArrayItem("percent", "stackMode");
					arr.InitAndSetArrayItem("10", "maximumGap");
					arr.InitAndSetArrayItem("100", "maximum");
				}
				return new XVar(0, new XVar("names", XVar.Array()), 1, arr);
			}
			return XVar.Array();
		}
	}
	public partial class Chart_Pie : Chart
	{
		protected dynamic pie;
		protected static bool skipChart_PieCtor = false;
		public Chart_Pie(dynamic _param_param, dynamic _param_chartSettings)
			:base((XVar)_param_param, (XVar)_param_chartSettings)
		{
			if(skipChart_PieCtor)
			{
				skipChart_PieCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			dynamic chartSettings = XVar.Clone(_param_chartSettings);
			#endregion

			this.pie = XVar.Clone(param["pie"]);
			this._2d = XVar.Clone(param["2d"]);
			this.singleSeries = new XVar(true);
		}
		protected override XVar setTypeSpecChartSettings(dynamic chart)
		{
			dynamic series = XVar.Array();
			series = XVar.Clone(this.get_data());
			chart.InitAndSetArrayItem(series[0]["data"], "data");
			chart.InitAndSetArrayItem(series[0]["clickData"], "clickData");
			chart.InitAndSetArrayItem(true, "singleSeries");
			chart.InitAndSetArrayItem(series[0]["tooltip"], "tooltip");
			chart.InitAndSetArrayItem(false, "logarithm");
			if(XVar.Pack(this._2d))
			{
				chart.InitAndSetArrayItem("pie", "type");
			}
			else
			{
				chart.InitAndSetArrayItem("pie-3d", "type");
			}
			if(XVar.Pack(!(XVar)(this.pie)))
			{
				chart.InitAndSetArrayItem("30%", "innerRadius");
			}
			if((XVar)(this.chartSettings["legend"])  && (XVar)(!(XVar)(this.chartPreview)))
			{
				chart.InitAndSetArrayItem(new XVar("enabled", "true"), "legend");
			}
			chart.InitAndSetArrayItem(new XVar("enabled", (XVar)(this.chartSettings["values"])  || (XVar)(this.chartSettings["names"])), "labels");
			if(this.webChartColors["color51"] != "")
			{
				if(XVar.Pack(this.chartSettings["values"]))
				{
					chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color61"]), "labels", "fontColor");
				}
				else
				{
					if(XVar.Pack(this.chartSettings["names"]))
					{
						chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color51"]), "labels", "fontColor");
					}
				}
			}

			return null;
		}
	}
	public partial class Chart_Combined : Chart
	{
		protected static bool skipChart_CombinedCtor = false;
		public Chart_Combined(dynamic _param_param, dynamic _param_chartSettings)
			:base((XVar)_param_param, (XVar)_param_chartSettings)
		{
			if(skipChart_CombinedCtor)
			{
				skipChart_CombinedCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			dynamic chartSettings = XVar.Clone(_param_chartSettings);
			#endregion

		}
		protected override XVar setTypeSpecChartSettings(dynamic chart)
		{
			chart.InitAndSetArrayItem(this.get_data(), "series");
			chart.InitAndSetArrayItem("column", "type");
			chart.InitAndSetArrayItem(this.chartSettings["logarithm"], "logarithm");
			chart.InitAndSetArrayItem(0, "xScale");
			chart.InitAndSetArrayItem(1, "yScale");
			chart.InitAndSetArrayItem(this.getGrids(), "grids");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", this.y_axis_label)), "yAxes");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", new XVar("text", this.footer), "labels", new XVar("enabled", this.chartSettings["names"]))), "xAxes");
			if(this.webChartColors["color51"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color51"]), "xAxes", 0, "labels", "fontColor");
			}
			if(this.webChartColors["color111"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color111"]), "xAxes", 0, "title", "fontColor");
			}
			if(this.webChartColors["color131"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color131"]), "xAxes", 0, "stroke");
			}
			if(this.webChartColors["color141"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color141"]), "yAxes", 0, "stroke");
			}

			return null;
		}
		protected override XVar getSeriesType(dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			switch(((XVar)seriesNumber).ToInt())
			{
				case 0:
					return "spline";
					break;
				case 1:
					return "splineArea";
					break;
				default:
					return "column";
			}

			return null;
		}
	}
	public partial class Chart_Funnel : Chart
	{
		protected dynamic inver;
		protected static bool skipChart_FunnelCtor = false;
		public Chart_Funnel(dynamic _param_param, dynamic _param_chartSettings)
			:base((XVar)_param_param, (XVar)_param_chartSettings)
		{
			if(skipChart_FunnelCtor)
			{
				skipChart_FunnelCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			dynamic chartSettings = XVar.Clone(_param_chartSettings);
			#endregion

			this.inver = XVar.Clone(param["funnel_inv"]);
			this.singleSeries = new XVar(true);
		}
		protected override XVar setTypeSpecChartSettings(dynamic chart)
		{
			dynamic series = XVar.Array();
			series = XVar.Clone(this.get_data());
			chart.InitAndSetArrayItem("pyramid", "type");
			chart.InitAndSetArrayItem(series[0]["data"], "data");
			chart.InitAndSetArrayItem(series[0]["clickData"], "clickData");
			chart.InitAndSetArrayItem(true, "singleSeries");
			chart.InitAndSetArrayItem(series[0]["tooltip"], "tooltip");
			chart.InitAndSetArrayItem(false, "logarithm");
			if(XVar.Pack(this.inver))
			{
				chart.InitAndSetArrayItem(true, "reversed");
			}
			chart.InitAndSetArrayItem(new XVar("enabled", this.chartSettings["names"]), "labels");
			if(this.webChartColors["color51"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color51"]), "labels", "fontColor");
			}

			return null;
		}
	}
	public partial class Chart_Bubble : Chart
	{
		protected static bool skipChart_BubbleCtor = false;
		public Chart_Bubble(dynamic _param_param, dynamic _param_chartSettings)
			:base((XVar)_param_param, (XVar)_param_chartSettings)
		{
			if(skipChart_BubbleCtor)
			{
				skipChart_BubbleCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			dynamic chartSettings = XVar.Clone(_param_chartSettings);
			#endregion

			this._2d = XVar.Clone(param["2d"]);
		}
		protected override XVar setTypeSpecChartSettings(dynamic chart)
		{
			chart.InitAndSetArrayItem(this.get_data(), "series");
			chart.InitAndSetArrayItem("cartesian", "type");
			chart.InitAndSetArrayItem(this.getGrids(), "grids");
			chart.InitAndSetArrayItem(this.chartSettings["logarithm"], "logarithm");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", true, "title", this.y_axis_label, "labels", new XVar("enabled", this.chartSettings["values"]))), "yAxes");
			if(this.webChartColors["color61"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color61"]), "yAxes", 0, "labels", "fontColor");
			}
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", new XVar("text", this.footer), "labels", new XVar("enabled", this.chartSettings["names"]))), "xAxes");
			if(this.webChartColors["color51"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color51"]), "xAxes", 0, "labels", "fontColor");
			}
			if(this.webChartColors["color111"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color111"]), "xAxes", 0, "title", "fontColor");
			}
			if(this.webChartColors["color131"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color131"]), "xAxes", 0, "stroke");
			}
			if(this.webChartColors["color141"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color141"]), "yAxes", 0, "stroke");
			}

			return null;
		}
		protected override XVar getSeriesType(dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			return "bubble";
		}
		protected override XVar getPoint(dynamic _param_seriesNumber, dynamic _param_row)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			dynamic row = XVar.Clone(_param_row);
			#endregion

			dynamic pointData = XVar.Array();
			pointData = XVar.Clone(base.getPoint((XVar)(seriesNumber), (XVar)(row)));
			pointData.InitAndSetArrayItem((double)MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)(row[this.arrDataSeries[seriesNumber]["bubbleSize"]])), "size");
			return pointData;
		}
	}
	public partial class Chart_Gauge : Chart
	{
		protected dynamic gaugeType = XVar.Pack("");
		protected dynamic layout = XVar.Pack("");
		protected static bool skipChart_GaugeCtor = false;
		public Chart_Gauge(dynamic _param_param, dynamic _param_chartSettings)
			:base((XVar)_param_param, (XVar)_param_chartSettings)
		{
			if(skipChart_GaugeCtor)
			{
				skipChart_GaugeCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			dynamic chartSettings = XVar.Clone(_param_chartSettings);
			#endregion

			this.gaugeType = XVar.Clone(param["gaugeType"]);
			this.layout = XVar.Clone(param["layout"]);
		}
		public override XVar write()
		{
			dynamic chart = XVar.Array(), data = XVar.Array(), i = null;
			data = XVar.Clone(XVar.Array());
			i = XVar.Clone(MVCFunctions.count(this.arrDataSeries) - 1);
			for(;XVar.Pack(0) <= i; --(i))
			{
				chart = XVar.Clone(XVar.Array());
				if(XVar.Pack(this.chartSettings["animation"]))
				{
					chart.InitAndSetArrayItem(new XVar("enabled", "true", "duration", 1000), "animation");
				}
				this.setGaugeSpecChartSettings((XVar)(chart), (XVar)(i));
				if((XVar)(this.webChartColors["color71"] != "")  || (XVar)(this.webChartColors["color91"] != ""))
				{
					chart.InitAndSetArrayItem(XVar.Array(), "background");
				}
				if(this.webChartColors["color71"] != "")
				{
					chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color71"]), "background", "fill");
				}
				if(this.webChartColors["color91"] != "")
				{
					chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color91"]), "background", "stroke");
				}
				if(XVar.Pack(this.noRecordsFound))
				{
					data.InitAndSetArrayItem(this.getNoDataMessage(), "noDataMessage");
					MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(data)));
					return null;
				}
				data.InitAndSetArrayItem(new XVar("gauge", chart), null);
			}
			MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(new XVar("gauge", data, "header", this.header, "footer", this.footer))));

			return null;
		}
		protected virtual XVar setGaugeSpecChartSettings(dynamic chart, dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			dynamic series = XVar.Array();
			series = XVar.Clone(this.get_data());
			chart.InitAndSetArrayItem(series[seriesNumber]["data"], "data");
			chart.InitAndSetArrayItem(this.gaugeType, "type");
			chart.InitAndSetArrayItem(this.layout, "layout");
			chart.InitAndSetArrayItem(new XVar(0, this.getAxesSettings((XVar)(seriesNumber))), "axes");
			chart.InitAndSetArrayItem(false, "credits");
			chart.InitAndSetArrayItem(this.getCircularGaugeLabel((XVar)(seriesNumber), (XVar)(chart["data"][0])), "chartLabels");
			if(this.gaugeType == "circular-gauge")
			{
				chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", true)), "needles");
				chart.InitAndSetArrayItem(XVar.Array(), "ranges");
				foreach (KeyValuePair<XVar, dynamic> colorZone in this.arrDataSeries[seriesNumber]["gaugeColorZones"].GetEnumerator())
				{
					chart.InitAndSetArrayItem(new XVar("radius", 70, "from", colorZone.Value["begin"], "to", colorZone.Value["end"], "fill", MVCFunctions.Concat("#", colorZone.Value["color"]), "endSize", "10%", "startSize", "10%"), "ranges", null);
				}
			}
			else
			{
				dynamic hasColorZones = null, scalesData = XVar.Array();
				chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", true, "pointerType", "marker", "type", (XVar.Pack(this.layout == "horizontal") ? XVar.Pack("triangleUp") : XVar.Pack("triangleLeft")), "name", "", "offset", (XVar.Pack(hasColorZones) ? XVar.Pack("20%") : XVar.Pack("10%")), "dataIndex", 0)), "pointers");
				foreach (KeyValuePair<XVar, dynamic> colorZone in this.arrDataSeries[seriesNumber]["gaugeColorZones"].GetEnumerator())
				{
					chart.InitAndSetArrayItem(new XVar("enabled", true, "pointerType", "rangeBar", "name", "", "offset", "10%", "dataIndex", colorZone.Key + 1, "color", MVCFunctions.Concat("#", colorZone.Value["color"])), "pointers", null);
				}
				scalesData = XVar.Clone(this.getGaugeScales((XVar)(seriesNumber)));
				chart.InitAndSetArrayItem(0, "scale");
				chart.InitAndSetArrayItem(new XVar(0, new XVar("maximum", scalesData["max"], "minimum", scalesData["min"], "ticks", new XVar("interval", scalesData["interval"]), "minorTicks", new XVar("interval", scalesData["interval"] / 2))), "scales");
			}

			return null;
		}
		protected virtual XVar getCircularGaugeLabel(dynamic _param_seriesNumber, dynamic _param_pointData)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			dynamic pointData = XVar.Clone(_param_pointData);
			#endregion

			dynamic label = XVar.Array();
			label = XVar.Clone(new XVar("enabled", true, "vAlign", "center", "hAlign", "center", "text", this.getChartLabelText((XVar)(seriesNumber), (XVar)(pointData["value"]))));
			if(this.gaugeType == "circular-gauge")
			{
				label.InitAndSetArrayItem(-150, "offsetY");
				label.InitAndSetArrayItem("center", "anchor");
				label.InitAndSetArrayItem(new XVar("enabled", true, "fill", "#fff", "cornerType", "round", "corner", 0), "background");
				label.InitAndSetArrayItem(new XVar("top", 15, "right", 20, "bottom", 15, "left", 20), "padding");
			}
			return new XVar(0, label);
		}
		protected virtual XVar getAxesSettings(dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			dynamic axes = XVar.Array();
			axes = XVar.Clone(XVar.Array());
			if(this.gaugeType == "circular-gauge")
			{
				dynamic scalesData = XVar.Array();
				axes.InitAndSetArrayItem(-150, "startAngle");
				axes.InitAndSetArrayItem(300, "sweepAngle");
				scalesData = XVar.Clone(this.getGaugeScales((XVar)(seriesNumber)));
				axes.InitAndSetArrayItem(new XVar("maximum", scalesData["max"], "minimum", scalesData["min"], "ticks", new XVar("interval", scalesData["interval"]), "minorTicks", new XVar("interval", scalesData["interval"] / 2)), "scale");
				axes.InitAndSetArrayItem(new XVar("type", "trapezoid", "interval", scalesData["interval"]), "ticks");
				axes.InitAndSetArrayItem(new XVar("enabled", true, "length", 2), "minorTicks");
				if(this.webChartColors["color131"] != "")
				{
					axes.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color131"]), "fill");
				}
			}
			axes.InitAndSetArrayItem(true, "enabled");
			axes.InitAndSetArrayItem(new XVar("enabled", this.chartSettings["values"]), "labels");
			if(this.webChartColors["color61"] != "")
			{
				axes.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color61"]), "labels", "fontColor");
			}
			return axes;
		}
		protected virtual XVar getGaugeScales(dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			dynamic diff = null, interval = null, max = null, min = null, muls = XVar.Array(), slog = null;
			min = XVar.Clone(this.arrDataSeries[seriesNumber]["minValue"]);
			max = XVar.Clone(this.arrDataSeries[seriesNumber]["maxValue"]);
			if(XVar.Pack(!(XVar)(MVCFunctions.IsNumeric(min))))
			{
				min = new XVar(0);
			}
			if(XVar.Pack(!(XVar)(MVCFunctions.IsNumeric(max))))
			{
				max = new XVar(100);
			}
			diff = XVar.Clone(max - min);
			slog = XVar.Clone((XVar)Math.Floor((double)((XVar)Math.Log10(diff))));
			interval = XVar.Clone((XVar)Math.Pow(10, slog - 2));
			muls = XVar.Clone(new XVar(0, 1, 1, 2, 2, 3, 3, 5, 4, 10));
			while(XVar.Pack(true))
			{
				foreach (KeyValuePair<XVar, dynamic> m in muls.GetEnumerator())
				{
					if(diff / (interval * m.Value) <= 10)
					{
						interval *= m.Value;
						break;
					}
				}
				if(diff / interval <= 10)
				{
					break;
				}
				interval *= 10;
			}
			return new XVar("min", min, "max", max, "interval", interval);
		}
		public override XVar getSubsetDataCommand(dynamic _param_ignoreFilterField = null)
		{
			#region default values
			if(_param_ignoreFilterField as Object == null) _param_ignoreFilterField = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic ignoreFilterField = XVar.Clone(_param_ignoreFilterField);
			#endregion

			dynamic dc = null;
			dc = XVar.Clone(base.getSubsetDataCommand());
			if(this.tableType == "project")
			{
				dynamic order = XVar.Array(), orderObject = null, revertedOrder = XVar.Array();
				orderObject = XVar.Clone(new OrderClause((XVar)(this.pSet), (XVar)(this.cipherer), (XVar)(this.sessionPrefix), (XVar)(this.connection)));
				order = XVar.Clone(orderObject.getOrderFields());
				revertedOrder = XVar.Clone(XVar.Array());
				foreach (KeyValuePair<XVar, dynamic> o in order.GetEnumerator())
				{
					dynamic ro = XVar.Array();
					ro = XVar.Clone(o.Value);
					ro.InitAndSetArrayItem((XVar.Pack(ro["dir"] == "ASC") ? XVar.Pack("DESC") : XVar.Pack("ASC")), "dir");
					revertedOrder.InitAndSetArrayItem(ro, null);
				}
				dc.order = XVar.Clone(revertedOrder);
			}
			return dc;
		}
		public override XVar get_data()
		{
			dynamic clickdata = XVar.Array(), data = XVar.Array(), dc = null, i = null, row = null, rs = null, series = XVar.Array();
			data = XVar.Clone(XVar.Array());
			dc = XVar.Clone(this.getSubsetDataCommand());
			this.beforeQueryEvent((XVar)(dc));
			rs = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				MVCFunctions.showError((XVar)(this.dataSource.lastError()));
			}
			row = XVar.Clone(rs.fetchAssoc());
			if(XVar.Pack(this.cipherer))
			{
				row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(row)));
			}
			if(XVar.Pack(!(XVar)(row)))
			{
				this.noRecordsFound = new XVar(true);
			}
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.arrDataSeries); i++)
			{
				if(XVar.Pack(row))
				{
					data.InitAndSetArrayItem(XVar.Array(), i);
					data.InitAndSetArrayItem(this.getPoint((XVar)(i), (XVar)(row)), i, null);
				}
			}
			series = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.arrDataSeries); i++)
			{
				series.InitAndSetArrayItem(this.getSeriesData((XVar)(this.arrDataLabels[i]), (XVar)(data[i]), (XVar)(clickdata[i]), (XVar)(i), (XVar)(1 < MVCFunctions.count(this.arrDataSeries))), null);
			}
			return series;
		}
		protected override XVar getSeriesData(dynamic _param_name, dynamic _param_pointsData, dynamic _param_clickData, dynamic _param_seriesNumber, dynamic _param_multiSeries = null)
		{
			#region default values
			if(_param_multiSeries as Object == null) _param_multiSeries = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic name = XVar.Clone(_param_name);
			dynamic pointsData = XVar.Clone(_param_pointsData);
			dynamic clickData = XVar.Clone(_param_clickData);
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			dynamic multiSeries = XVar.Clone(_param_multiSeries);
			#endregion

			if(this.gaugeType == "linearGauge")
			{
				foreach (KeyValuePair<XVar, dynamic> colorZone in this.arrDataSeries[seriesNumber]["gaugeColorZones"].GetEnumerator())
				{
					pointsData.InitAndSetArrayItem(new XVar("low", colorZone.Value["begin"], "high", colorZone.Value["end"]), null);
				}
			}
			return new XVar("data", pointsData, "labelText", this.getChartLabelText((XVar)(seriesNumber), (XVar)(pointsData[0]["value"])));
		}
		protected virtual XVar getChartLabelText(dynamic _param_seriesNumber, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			if((XVar)(this.tableType == "project")  && (XVar)(!(XVar)(this.webchart)))
			{
				dynamic data = null, fieldName = null, viewControls = null, viewValue = null;
				fieldName = XVar.Clone(this.arrDataSeries[seriesNumber]["dataField"]);
				viewControls = XVar.Clone(new ViewControlsContainer((XVar)(this.pSet), new XVar(Constants.PAGE_CHART)));
				data = XVar.Clone(new XVar(fieldName, value));
				viewValue = XVar.Clone(viewControls.showDBValue((XVar)(fieldName), (XVar)(data), new XVar(""), new XVar(""), new XVar(false)));
				return MVCFunctions.Concat(this.arrDataLabels[seriesNumber], ": ", viewValue);
			}
			return MVCFunctions.Concat(this.arrDataLabels[seriesNumber], ": ", value);
		}
	}
	public partial class Chart_Ohlc : Chart
	{
		protected dynamic ohcl_type;
		protected static bool skipChart_OhlcCtor = false;
		public Chart_Ohlc(dynamic _param_param, dynamic _param_chartSettings)
			:base((XVar)_param_param, (XVar)_param_chartSettings)
		{
			if(skipChart_OhlcCtor)
			{
				skipChart_OhlcCtor = false;
				return;
			}
			#region pass-by-value parameters
			dynamic param = XVar.Clone(_param_param);
			dynamic chartSettings = XVar.Clone(_param_chartSettings);
			#endregion

			this.ohcl_type = XVar.Clone(param["ohcl_type"]);
		}
		public override XVar write()
		{
			dynamic chart = XVar.Array(), data = XVar.Array();
			data = XVar.Clone(XVar.Array());
			chart = XVar.Clone(XVar.Array());
			this.setTypeSpecChartSettings((XVar)(chart));
			if((XVar)(this.webChartColors["color71"] != "")  || (XVar)(this.webChartColors["color91"] != ""))
			{
				chart.InitAndSetArrayItem(XVar.Array(), "background");
			}
			if(this.webChartColors["color71"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color71"]), "background", "fill");
			}
			if(this.webChartColors["color91"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color91"]), "background", "stroke");
			}
			chart.InitAndSetArrayItem(false, "credits");
			chart.InitAndSetArrayItem(new XVar("enabled", "true", "text", this.header), "title");
			if(this.webChartColors["color101"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color101"]), "title", "fontColor");
			}
			if((XVar)(this.chartSettings["legend"])  && (XVar)(!(XVar)(this.chartPreview)))
			{
				chart.InitAndSetArrayItem(new XVar("enabled", "true"), "legend");
			}
			data.InitAndSetArrayItem(chart, "chart");
			MVCFunctions.Echo(MVCFunctions.runner_json_encode((XVar)(data)));

			return null;
		}
		protected override XVar setTypeSpecChartSettings(dynamic chart)
		{
			chart.InitAndSetArrayItem(this.get_data(), "series");
			if(XVar.Pack(this.webchart))
			{
				foreach (KeyValuePair<XVar, dynamic> series in this.arrDataSeries.GetEnumerator())
				{
					if(series.Value["ohlcColor"] != "")
					{
						chart.InitAndSetArrayItem(MVCFunctions.Concat("#", series.Value["ohlcColor"]), "series", series.Key, "fallingStroke");
						chart.InitAndSetArrayItem(MVCFunctions.Concat("#", series.Value["ohlcColor"]), "series", series.Key, "fallingFill");
						if(this.ohcl_type == "ohcl")
						{
							chart.InitAndSetArrayItem(MVCFunctions.Concat("#", series.Value["ohlcColor"]), "series", series.Key, "risingStroke");
							chart.InitAndSetArrayItem(MVCFunctions.Concat("#", series.Value["ohlcColor"]), "series", series.Key, "risingFill");
						}
					}
					if((XVar)(series.Value["ohlcCandleColor"] != "")  && (XVar)(this.ohcl_type == "candlestick"))
					{
						chart.InitAndSetArrayItem(MVCFunctions.Concat("#", series.Value["ohlcCandleColor"]), "series", series.Key, "risingStroke");
						chart.InitAndSetArrayItem(MVCFunctions.Concat("#", series.Value["ohlcCandleColor"]), "series", series.Key, "risingFill");
					}
				}
			}
			chart.InitAndSetArrayItem(this.getGrids(), "grids");
			chart.InitAndSetArrayItem(this.chartSettings["logarithm"], "logarithm");
			chart.InitAndSetArrayItem("financial", "type");
			chart.InitAndSetArrayItem(0, "xScale");
			chart.InitAndSetArrayItem(1, "yScale");
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", this.y_axis_label, "labels", new XVar("enabled", this.chartSettings["values"]))), "yAxes");
			if(this.webChartColors["color61"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color61"]), "yAxes", 0, "labels", "fontColor");
			}
			chart.InitAndSetArrayItem(new XVar(0, new XVar("enabled", "true", "title", new XVar("text", this.footer), "labels", new XVar("enabled", this.chartSettings["names"]))), "xAxes");
			if(this.webChartColors["color51"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color51"]), "xAxes", 0, "labels", "fontColor");
			}
			if(this.webChartColors["color111"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color111"]), "xAxes", 0, "title", "fontColor");
			}
			if(this.webChartColors["color131"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color131"]), "xAxes", 0, "stroke");
			}
			if(this.webChartColors["color141"] != "")
			{
				chart.InitAndSetArrayItem(MVCFunctions.Concat("#", this.webChartColors["color141"]), "yAxes", 0, "stroke");
			}
			if(XVar.Pack(this.chartSettings["logarithm"]))
			{
				chart.InitAndSetArrayItem(new XVar(0, new XVar("names", XVar.Array()), 1, new XVar("logBase", 10, "type", "log")), "scales");
			}

			return null;
		}
		public override XVar get_data()
		{
			dynamic clickdata = XVar.Array(), data = XVar.Array(), dc = null, i = null, row = null, rs = null, series = XVar.Array(), strLabelFormat = null;
			data = XVar.Clone(XVar.Array());
			clickdata = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.arrDataSeries); i++)
			{
				data.InitAndSetArrayItem(XVar.Array(), i);
				clickdata.InitAndSetArrayItem(XVar.Array(), i);
			}
			dc = XVar.Clone(this.getSubsetDataCommand());
			this.beforeQueryEvent((XVar)(dc));
			rs = XVar.Clone(this.dataSource.getList((XVar)(dc)));
			if(XVar.Pack(!(XVar)(rs)))
			{
				MVCFunctions.showError((XVar)(this.dataSource.lastError()));
			}
			row = XVar.Clone(rs.fetchAssoc());
			if(XVar.Pack(this.cipherer))
			{
				row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(row)));
			}
			if(XVar.Pack(!(XVar)(row)))
			{
				this.noRecordsFound = new XVar(true);
			}
			while(XVar.Pack(row))
			{
				i = new XVar(0);
				for(;i < MVCFunctions.count(this.arrDataSeries); i++)
				{
					data.InitAndSetArrayItem(this.getPoint((XVar)(i), (XVar)(row)), i, null);
					strLabelFormat = XVar.Clone(this.labelFormat((XVar)(this.strLabel), (XVar)(row)));
					clickdata.InitAndSetArrayItem(this.getActions((XVar)(row), (XVar)(this.arrDataSeries[i]), (XVar)(strLabelFormat)), i, null);
				}
				row = XVar.Clone(rs.fetchAssoc());
				if(XVar.Pack(this.cipherer))
				{
					row = XVar.Clone(this.cipherer.DecryptFetchedArray((XVar)(row)));
				}
			}
			series = XVar.Clone(XVar.Array());
			i = new XVar(0);
			for(;i < MVCFunctions.count(this.arrDataSeries); i++)
			{
				series.InitAndSetArrayItem(this.getSeriesData((XVar)(this.arrDataLabels[i]), (XVar)(data[i]), (XVar)(clickdata[i]), (XVar)(i)), null);
			}
			return series;
		}
		protected override XVar getSeriesType(dynamic _param_seriesNumber)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			#endregion

			if(this.ohcl_type == "ohcl")
			{
				return "ohlc";
			}
			return "candlestick";
		}
		protected override XVar getSeriesTooltip(dynamic _param_multiSeries)
		{
			#region pass-by-value parameters
			dynamic multiSeries = XVar.Clone(_param_multiSeries);
			#endregion

			dynamic tooltipSettings = null;
			tooltipSettings = XVar.Clone(new XVar("enabled", true));
			return tooltipSettings;
		}
		protected override XVar getPoint(dynamic _param_seriesNumber, dynamic _param_row)
		{
			#region pass-by-value parameters
			dynamic seriesNumber = XVar.Clone(_param_seriesNumber);
			dynamic row = XVar.Clone(_param_row);
			#endregion

			dynamic close = null, dataSeries = XVar.Array(), high = null, low = null, open = null;
			dataSeries = XVar.Clone(this.arrDataSeries[seriesNumber]);
			high = XVar.Clone(row[dataSeries["high"]]);
			low = XVar.Clone(row[dataSeries["low"]]);
			open = XVar.Clone(row[dataSeries["open"]]);
			close = XVar.Clone(row[dataSeries["close"]]);
			if((XVar)(this.tableType != "db")  || (XVar)(!(XVar)(this.chartSettings["webCustomLabels"])))
			{
				high = XVar.Clone(row[dataSeries["high"]]);
				low = XVar.Clone(row[dataSeries["low"]]);
				open = XVar.Clone(row[dataSeries["open"]]);
				close = XVar.Clone(row[dataSeries["close"]]);
			}
			else
			{
				high = XVar.Clone(row[this.chartSettings["webCustomLabels"][dataSeries["high"]]]);
				low = XVar.Clone(row[this.chartSettings["webCustomLabels"][dataSeries["low"]]]);
				open = XVar.Clone(row[this.chartSettings["webCustomLabels"][dataSeries["open"]]]);
				close = XVar.Clone(row[this.chartSettings["webCustomLabels"][dataSeries["close"]]]);
			}
			return new XVar("x", this.labelFormat((XVar)(this.strLabel), (XVar)(row)), "open", (double)open, "high", (double)high, "low", (double)low, "close", (double)MVCFunctions.str_replace(new XVar(","), new XVar("."), (XVar)(close)));
		}
	}
}
