
		// <global>
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
			public static partial class Options____global____menu
			{
				static public XVar options()
				{
					return new XVar( "welcome", new XVar( "welcomePageSkip", false,
"welcomeItems", new XVar( "logo", new XVar( "menutItem", false ),
"menu", new XVar( "menutItem", false ) ) ),
"fields", new XVar( "gridFields", XVar.Array(),
"searchRequiredFields", XVar.Array(),
"searchPanelFields", XVar.Array(),
"fieldItems", new XVar(  ) ),
"layoutHelper", new XVar( "formItems", new XVar( "formItems", new XVar( "above-grid", XVar.Array(),
"supertop", new XVar( 0, "logo",
1, "menu" ),
"grid", XVar.Array() ),
"formXtTags", new XVar( "above-grid", XVar.Array(),
"grid", XVar.Array() ),
"itemForms", new XVar( "logo", "supertop",
"menu", "supertop" ),
"itemLocations", new XVar(  ),
"itemVisiblity", new XVar( "menu", 3 ) ),
"itemsByType", new XVar( "logo", new XVar( 0, "logo" ),
"menu", new XVar( 0, "menu" ) ),
"cellMaps", new XVar(  ) ),
"page", new XVar( "verticalBar", false,
"labeledButtons", new XVar( "update_records", new XVar(  ),
"print_pages", new XVar(  ),
"register_activate_message", new XVar(  ),
"details_found", new XVar(  ) ),
"hasCustomButtons", false,
"customButtons", XVar.Array(),
"codeSnippets", XVar.Array(),
"clickHandlerSnippets", XVar.Array(),
"hasNotifications", false,
"menus", new XVar( 0, new XVar( "id", "main",
"horizontal", true ) ),
"calcTotalsFor", 1,
"hasCharts", false ),
"events", new XVar( "maps", XVar.Array(),
"mapsData", new XVar(  ),
"buttons", XVar.Array() ) );
				}
				static public XVar page()
				{
					return new XVar( "id", "menu",
"type", "menu",
"layoutId", "topbar",
"disabled", false,
"default", 0,
"forms", new XVar( "above-grid", new XVar( "modelId", "empty-above-grid",
"grid", new XVar( 0, new XVar( "cells", new XVar( 0, new XVar( "cell", "c1" ) ),
"section", "" ) ),
"cells", new XVar( "c1", new XVar( "model", "c1",
"items", XVar.Array() ) ),
"deferredItems", XVar.Array(),
"recsPerRow", 1 ),
"supertop", new XVar( "modelId", "menu-topbar-menu",
"grid", new XVar( 0, new XVar( "cells", new XVar( 0, new XVar( "cell", "c1" ),
1, new XVar( "cell", "c2" ) ),
"section", "" ) ),
"cells", new XVar( "c1", new XVar( "model", "c1",
"items", new XVar( 0, "logo",
1, "menu" ) ),
"c2", new XVar( "model", "c2",
"items", XVar.Array() ) ),
"deferredItems", XVar.Array(),
"recsPerRow", 1 ),
"grid", new XVar( "modelId", "welcome",
"grid", XVar.Array(),
"cells", new XVar(  ),
"deferredItems", XVar.Array(),
"recsPerRow", 1 ) ),
"items", new XVar( "logo", new XVar( "type", "logo" ),
"menu", new XVar( "type", "menu" ) ),
"dbProps", new XVar(  ),
"version", 13,
"imageItem", new XVar( "type", "page_image" ),
"imageBgColor", "#f2f2f2",
"controlsBgColor", "white",
"imagePosition", "right",
"welcomePageStay", true,
"listTotals", 1,
"title", new XVar(  ) );
				}
			}
		}