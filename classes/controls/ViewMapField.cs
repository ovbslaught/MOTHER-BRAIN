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
	public partial class ViewMapField : ViewControl
	{
		protected static bool skipViewMapFieldCtor = false;
		public ViewMapField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject) // proxy constructor
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageObject) {}

		public override XVar showDBValue(dynamic data, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic mapData = null, mapId = null;
			if(XVar.Pack(!(XVar)(this.pageObject)))
			{
				return MVCFunctions.runner_htmlspecialchars((XVar)(data[this.field]));
			}
			if((XVar)(this.pageObject.pageType == Constants.PAGE_EXPORT)  || (XVar)((XVar)(this.pageObject.pageType == Constants.PAGE_RPRINT)  && (XVar)(this.container.forExport == "excel")))
			{
				return MVCFunctions.runner_htmlspecialchars((XVar)(data[this.field]));
			}
			mapId = XVar.Clone(MVCFunctions.Concat("littleMap_", MVCFunctions.GoodFieldName((XVar)(this.field)), "_", this.pageObject.recId));
			if(this.pageObject.pageType != Constants.PAGE_LIST)
			{
				mapData = XVar.Clone(this.pageObject.addGoogleMapData((XVar)(this.field), (XVar)(data)));
			}
			else
			{
				mapData = this.pageObject.googleMapCfg["mapsData"][mapId];
			}
			if((XVar)((XVar)((XVar)(this.pageObject.pageType != Constants.PAGE_PRINT)  && (XVar)(this.pageObject.pageType != Constants.PAGE_MASTER_INFO_PRINT))  && (XVar)(this.pageObject.pageType != Constants.PAGE_RPRINT))  && (XVar)(this.pageObject.pageType != Constants.PAGE_REPORT))
			{
				return this.getFieldMap((XVar)(mapData), (XVar)(mapId));
			}
			return this.getMapImage((XVar)(mapData));
		}
		protected virtual XVar getFieldMap(dynamic mapData, dynamic _param_mapId)
		{
			#region pass-by-value parameters
			dynamic mapId = XVar.Clone(_param_mapId);
			#endregion

			dynamic height = null, width = null;
			width = new XVar(300);
			if(XVar.Pack(this.pageObject.googleMapCfg.KeyExists("fieldsAsMap")))
			{
				width = XVar.Clone(this.pageObject.googleMapCfg["fieldsAsMap"][this.field]["width"]);
			}
			height = new XVar(225);
			if(XVar.Pack(this.pageObject.googleMapCfg.KeyExists("fieldsAsMap")))
			{
				height = XVar.Clone(this.pageObject.googleMapCfg["fieldsAsMap"][this.field]["height"]);
			}
			if((XVar)(ProjectSettings.getProjectValue(new XVar("mapSettings"), new XVar("embed")))  && (XVar)(CommonFunctions.getMapProvider() == Constants.GOOGLE_MAPS))
			{
				dynamic q = null, src = null;
				mapData.InitAndSetArrayItem(true, "skipped");
				q = XVar.Clone(this.getPlaceDefinition((XVar)(mapData)));
				if(XVar.Pack(!(XVar)(q)))
				{
					return "";
				}
				src = XVar.Clone(MVCFunctions.Concat("https://www.google.com/maps/embed/v1/place?q=", q, "&zoom=", mapData["zoom"], "&key=", this.pageObject.googleMapCfg["APIcode"]));
				return MVCFunctions.Concat("<iframe width=\"", width, "\" height=\"", height, "\" frameborder=\"0\" style=\"border:0\"\r\n\t\t\t\tsrc=\"", src, "\" allowfullscreen></iframe>");
			}
			return MVCFunctions.Concat("<div id=\"", mapId, "\" style=\"width:", width, "px; height:", height, "px;\" data-gridlink class=\"littleMap\">", "</div>");
		}
		public virtual XVar getPlaceDefinition(dynamic mapData)
		{
			dynamic markerData = XVar.Array();
			markerData = XVar.Clone(mapData["markers"][0]);
			if((XVar)(markerData["lat"] == "")  && (XVar)(markerData["lng"] == ""))
			{
				return markerData["address"];
			}
			return MVCFunctions.Concat(markerData["lat"], ",", markerData["lng"]);
		}
		public virtual XVar getLocation(dynamic _param_markerData)
		{
			#region pass-by-value parameters
			dynamic markerData = XVar.Clone(_param_markerData);
			#endregion

			if((XVar)(markerData["lat"] == "")  && (XVar)(markerData["lng"] == ""))
			{
				dynamic locationByAddress = XVar.Array();
				if(XVar.Pack(!(XVar)(markerData["address"])))
				{
					return "";
				}
				if(CommonFunctions.getMapProvider() == Constants.GOOGLE_MAPS)
				{
					return markerData["address"];
				}
				locationByAddress = XVar.Clone(CommonFunctions.getLatLngByAddr((XVar)(markerData["address"])));
				return MVCFunctions.Concat(locationByAddress["lat"], ",", locationByAddress["lng"]);
			}
			return MVCFunctions.Concat(markerData["lat"], ",", markerData["lng"]);
		}
		protected virtual XVar getMapImage(dynamic mapData)
		{
			dynamic markerData = XVar.Array();
			markerData = XVar.Clone(mapData["markers"][0]);
			return MVCFunctions.Concat("<img border=\"0\" alt=\"\" \r\n\t\t\tsrc=\"", this.getStaticMapURL((XVar)(this.getLocation((XVar)(markerData))), (XVar)(mapData["zoom"]), (XVar)(markerData["mapIcon"])), "\">");
		}
		public virtual XVar getStaticMapURL(dynamic _param_location, dynamic _param_zoom, dynamic _param_icon)
		{
			#region pass-by-value parameters
			dynamic location = XVar.Clone(_param_location);
			dynamic zoom = XVar.Clone(_param_zoom);
			dynamic icon = XVar.Clone(_param_icon);
			#endregion

			dynamic apiKey = null, height = null, markerLocation = null, width = null;
			markerLocation = XVar.Clone(location);
			apiKey = XVar.Clone(this.pageObject.googleMapCfg["APIcode"]);
			width = new XVar("300");
			height = new XVar("225");
			if(XVar.Pack(this.pageObject.googleMapCfg.KeyExists("fieldsAsMap")))
			{
				width = XVar.Clone(this.pageObject.googleMapCfg["fieldsAsMap"][this.field]["width"]);
				height = XVar.Clone(this.pageObject.googleMapCfg["fieldsAsMap"][this.field]["height"]);
			}
			switch(((XVar)CommonFunctions.getMapProvider()).ToInt())
			{
				case Constants.GOOGLE_MAPS:
					if((XVar)(icon)  && (XVar)(GlobalVars.showCustomMarkerOnPrint))
					{
						dynamic here = null, pos = null;
						here = XVar.Clone(MVCFunctions.Concat(CommonFunctions.request_protocol(), "://", MVCFunctions.GetServerVariable("HTTP_HOST"), MVCFunctions.GetServerVariable("REQUEST_URI")));
						pos = XVar.Clone(MVCFunctions.strrpos((XVar)(here), new XVar("/")));
						here = XVar.Clone(MVCFunctions.Concat(MVCFunctions.substr((XVar)(here), new XVar(0), (XVar)(pos)), "/images/menuicons/", icon));
						markerLocation = XVar.Clone(MVCFunctions.Concat("icon:", here, "|", location));
					}
					return MVCFunctions.Concat("https://maps.googleapis.com/maps/api/staticmap?center=", location, "&zoom=", zoom, "&size=", width, "x", height, "&maptype=mobile&markers=", markerLocation, "&key=", apiKey);
				case Constants.OPEN_STREET_MAPS:
					return MVCFunctions.Concat("https://staticmap.openstreetmap.de/staticmap.php?center=", location, "&zoom=", zoom, "&size=", width, "x", height, "&maptype=mobile&markers=", markerLocation, ",ol-marker");
				case Constants.BING_MAPS:
					return MVCFunctions.Concat("https://dev.virtualearth.net/REST/v1/Imagery/Map/Road/", location, "/", zoom, "?mapSize=", width, ",", height, "&pp=", markerLocation, ";63;&key=", apiKey);
				case Constants.HERE_MAPS:
					return MVCFunctions.Concat("https://image.maps.ls.hereapi.com/mia/1.6/mapview?", "apiKey=", apiKey, "&z=", zoom, "&c=", location, "&w=", width, "&h=", height);
				case Constants.MAPQUEST_MAPS:
					return MVCFunctions.Concat("https://www.mapquestapi.com/staticmap/v5/map?", "key=", apiKey, "&zoom=", zoom, "&locations=", location, "&size=", width, ",", height);
				default:
					return "";
			}

			return null;
		}
		public override XVar getPdfValue(dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			dynamic content = null, imageType = null, location = null, mapData = XVar.Array(), staticUrl = null;
			mapData = XVar.Clone(this.pageObject.addGoogleMapData((XVar)(this.field), (XVar)(data)));
			location = XVar.Clone(this.getLocation((XVar)(mapData["markers"][0])));
			staticUrl = XVar.Clone(this.getStaticMapURL((XVar)(location), (XVar)(mapData["zoom"]), (XVar)(mapData["markers"][0]["mapIcon"])));
			content = XVar.Clone(MVCFunctions.myurl_get_contents_binary((XVar)(staticUrl)));
			imageType = XVar.Clone(MVCFunctions.SupposeImageType((XVar)(content)));
			if((XVar)(imageType == "image/jpeg")  || (XVar)(imageType == "image/png"))
			{
				return MVCFunctions.Concat("{\r\n\t\t\t\timage: \"", CommonFunctions.jsreplace((XVar)(MVCFunctions.Concat("data:", imageType, ";base64,", MVCFunctions.base64_bin2str((XVar)(content))))), "\",\r\n\t\t\t}");
			}
			return "\"\"";
		}
	}
}
