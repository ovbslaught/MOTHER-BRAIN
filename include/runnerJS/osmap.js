/**
 * The class containing specific functions using Open Street Maps API
 * http://dev.openlayers.org/releases/OpenLayers-2.6/doc/apidocs/files/OpenLayers-js.html
 * @class Runner.controls.gMap
 */
Runner.controls.osMap = Runner.extend( Runner.emptyFn, {
	/**
	 * WGS 1984 Projection
	 */
	fromProjection: null,
	
	/**
	 * Spherical Mercator Projection
	 */
	toProjection: null,
	
	
	/**
	 * @constructor
	 */
	constructor: function() {
		Runner.controls.osMap.superclass.constructor.call( this );
		
		this.fromProjection = new OpenLayers.Projection("EPSG:4326");
		this.toProjection = new OpenLayers.Projection("EPSG:900913");		
	},
	
	/**
	 * 
	 */
	initMapBounds: function() {
		return new OpenLayers.Bounds();
	},
	
	/**
	 * @param {jQuery object} $mapElem
	 * @param {number} zoom
	 * @param {number} centerLat
	 * @param {number} centerLng	 
	 * @return {OpenLayers.Map object}
	 */
	createMap: function( $mapElem, zoom, centerLat, centerLng ) {
		var map, mapOSM, lonLat, position, markers;
		
		$mapElem.html("");
		
		lonLat = new OpenLayers.LonLat(0, 0);
		position = lonLat.transform( this.fromProjection, this.toProjection );
		
		map = new OpenLayers.Map( $mapElem.attr("id") );
		
		mapOSM = new OpenLayers.Layer.OSM(
				"OpenStreetMap", 
				// Official OSM tileset as protocol-independent URLs
				[
					'https://a.tile.openstreetmap.org/${z}/${x}/${y}.png',
					'https://b.tile.openstreetmap.org/${z}/${x}/${y}.png',
					'https://c.tile.openstreetmap.org/${z}/${x}/${y}.png'
				], 
				null);
		map.addLayer( mapOSM );
		
		markers = new OpenLayers.Layer.Markers("Markers");
		map.addLayer( markers );
		
		if ( centerLat !== undefined && centerLng !== undefined ) {
			this.setMapCenter( map, zoom, centerLat, centerLng );
		}
		
		return map;
	},
	
	/**
	 * @param {string} address
	 * @param {object} map
	 * @param {number} zoom
	 */	
	setCenter: function( address, map, zoom ) {
		var thisObj = this;
		
		$.getJSON('https://nominatim.openstreetmap.org/search/' + address + '?format=json', function( data ) {
			var addressData, geoResult;
			
			if ( data.length ) {
				addressData = data[0];
				geoResult = { 
					lng: parseFloat(addressData.lon), 
					lat: parseFloat(addressData.lat) 
				};
				
				map.setCenter( thisObj.mapsLatLng(geoResult.lat, geoResult.lng), zoom );
			}
		});
	},
	
	/**
	 * @param {} lat
	 * @param {} lng
	 * @return {}
	 */
	mapsLatLng: function( lat, lng ) {
		return new OpenLayers.LonLat(lng, lat).transform( this.fromProjection, this.toProjection );
	},
	
	/**
	 * Get the float number from string
	 * @param {string} aVal
	 * @return {number}
	 */
	parseValue: function( aVal ) {
		return typeof( aVal ) == 'string' ? parseFloat( aVal.replace(',', '.') ) : aVal;
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 */
	setMarkerByCoords: function( markerData, mapData ) {	
		var position = this.mapsLatLng( this.parseValue( markerData.lat ), this.parseValue( markerData.lng ) ),
			markersLayer = mapData.map.getLayersByName("Markers")[0],
			iconFile, icon, marker;

		if ( markerData.mapIcon ) {
			iconFile = settings.global["webRootPath"] + markerData.mapIcon;
		} else {
			iconFile = settings.global["webRootPath"] + "plugins/img/marker.png";
		}
		
		this.destroyMarker( mapData.map, markerData.marker );	
		
		icon = new OpenLayers.Icon( iconFile ), 
		marker = new OpenLayers.Marker( position, icon.clone() ), 	
		marker.icon.imageDiv.title = markerData.desc;
		markersLayer.addMarker ( marker );
		
		markerData.marker = marker;
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 * @param {function} onAddressResolvedHandler
	 */	
	setMarkerByAddress: function( markerData, mapData, onAddressResolvedHandler ) {
		var thisObj = this;
		
		$.getJSON('https://nominatim.openstreetmap.org/search/' + markerData.address + '?format=json', function( data ) {
			var addressData,	
				position, marker, markersLayer,
				icon;
			
			if ( !data.length ) {	
				thisObj.destroyMarker( mapData.map, markerData.marker );
				return;
			}
			
			addressData = data[ 0 ];	
			markerData.lat = parseFloat( addressData.lat ).toString();
			markerData.lng = parseFloat( addressData.lon ).toString();			
			
			position = thisObj.mapsLatLng( thisObj.parseValue(markerData.lat), thisObj.parseValue(markerData.lng) );
			icon = new OpenLayers.Icon( markerData.mapIcon ? settings.global["webRootPath"] + markerData.mapIcon : settings.global["webRootPath"] + "plugins/img/marker.png" );
			
			marker = new OpenLayers.Marker( position, icon.clone() );
			marker.icon.imageDiv.title = markerData.desc;
			
			markersLayer = mapData.map.getLayersByName("Markers")[0];			
			markersLayer.addMarker( marker );
			markerData.marker = marker;
			
			onAddressResolvedHandler();
		});
	},
	
	/**
	 * @param {object} marker
	 * @param {function} handler
	 */
	addOnMarkerClickHandler: function( marker, handler ) { 
		marker.events.register( "click", marker, handler );
	},
	
	/**
	 * @param {object} marker 
	 */
	triggerMarkerClickEvent: function( marker ) {
		marker.events.triggerEvent( "click" );
	},	
	
	/**
	 * @param {object} map
	 * @param {object} marker
	 */		
	destroyMarker: function( map, marker ) {
		var layers;
		
		if ( marker ) {
			layers = map.getLayersByName("Markers");
			
			layers[0].removeMarker( marker );
			delete marker;
		}
	},

	/**
	 * @param {object} map
	 * @param {array} markers
	 */
	setZoomAuto: function( map, markers, boundLatLng ) {
		var newMapBounds = new OpenLayers.Bounds();
		
		if ( boundLatLng ) {
			newMapBounds.extend( markers[ boundLatLng.minLatID ].marker.lonlat );
			newMapBounds.extend( markers[ boundLatLng.minLngID ].marker.lonlat );
			newMapBounds.extend( markers[ boundLatLng.maxLatID ].marker.lonlat );
			newMapBounds.extend( markers[ boundLatLng.maxLngID ].marker.lonlat );
		}
		
		map.zoomToExtent( newMapBounds );		
	},	
	
	/**
	 * A stub
	 * @param {object} map
	 */		
	triggerResizeEvent: function( map ) {},
	
	/**
	 * @param {objetc} map
	 * @param {number} zoom
	 * @param {number} lat
	 * @param {number} lng
	 */
	setMapCenter: function( map, zoom, lat, lng ) {
		map.setCenter( this.mapsLatLng(lat, lng), zoom );
	},
	
	/**
	 * @param {object} map
	 * @param {number} zoom
	 */	
	setZoom: function( map, zoom ) {
		map.setZoom( zoom ); 
	},
	
	/**
	 * @param {object} map
	 * @param {function} handler
	 */
	addOnMapLoadHandler: function( map, handler ) {
		handler();
	},
	
	/**
	 * @param {object} map
	 * @param {function} handler
	 */
	addOnMapViewPortChangedHandler: function( map, handler ) {		
		map.events.register( "moveend", map, handler );
		map.events.register( "zoomend", map, handler );		
	},
	
	/**
	 * @param {object} map
	 * @return {object}
	 */
	getMapViewPortCoordinates: function( map ) { 
		var mapBounds = map.getExtent().transform( this.toProjection, this.fromProjection );

		return {
			n: mapBounds.top,
			s: mapBounds.bottom,
			e: mapBounds.right,
			w: mapBounds.left		
		};
	},
	
	/**
	 * @param {object} marker
	 * @param {object} map
	 * @return {boolean}
	 */
	mapBoundsContains: function( marker, map ) {
		var mapBounds = map.getExtent();

		return mapBounds && mapBounds.containsLonLat( marker.lonlat );
	},	
	
	/**
	 * @param {object} marker
	 * @param {boolean} customIcon
	 */
	setMarkerActive: function( marker, customIcon ) {
		if ( !marker ) {
			return;
		}	
		
		if ( !customIcon ) {
			marker.setUrl( settings.global["webRootPath"] + "plugins/img/marker-green.png" );
		} else {	
			marker.inflate( 1.25 );
		}					
	},

	/**
	 * @param {object} marker
	 * @param {boolean} customIcon
	 */
	setMarkerInactive: function( marker, customIcon ) {
		if ( !marker ) {
			return;
		}
		
		if ( !customIcon ) {
			marker.setUrl( settings.global["webRootPath"] + "plugins/img/marker.png" );
		} else {	
			marker.inflate( 0.8 );
		}		
	},
	
	/**
	 * @param {object} markerData
	 * @return {object}
	 */
	isClusterHandlerToAdd: function( markerData ) {
		return false;
	},	
	
	/**
	 * A stub
	 */
	mapIsHeatmap: function( mapObj ) {
		return false;
	},
	
	/**
	 * A stub
	 */
	initializeClusterMarkers: function( mapData ) {
		return null;
	},
	
	/**
	 * A stub
	 */
	clusterHandler: function( markerData ) {}
});
