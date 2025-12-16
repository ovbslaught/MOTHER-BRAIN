/**
 * The class containing specific functions for MAPQUEST Maps API
 * @class Runner.controls.mapQuestMap
 */
Runner.controls.mapQuestMap = Runner.extend( Runner.emptyFn, {
	/**
	 * @type {object}
	 */
	geoCoder: null,
	
	/**
	 * @constructor
	 */
	constructor: function() {
		L.mapquest.key = window.settings['global'].mapsApiCode;
		
		this.geoCoder = L.mapquest.geocoding();
	},
	
	/**
	 * @param {jQuery object} $mapElem
	 * @param {number} zoom (required)
	 * @param {number} centerLat
	 * @param {number} centerLng
	 * @return {L.mapquest.map object} map
	 */
	createMap: function( $mapElem, zoom, centerLat, centerLng ) {
		// set up stacking context 
		// so that inner map elements with higher z-index do not overlap page's menu blocks 
		$mapElem.css( "z-index", 1 );

		return L.mapquest.map( $mapElem.get(0), {
			center: centerLat !== undefined && centerLng !== undefined ? 
				[ centerLat, centerLng ] :
				[ 0, 0 ],
			layers: L.mapquest.tileLayer('map'),
			zoom: zoom
		});
	},
	
	
	/**
	 * @param {string} address
	 * @param {L.mapquest.map object} map
	 * @param {number} zoom
	 */		
	setCenter: function( address, map, zoom ) {
		this.geoCoder.geocode( address, function( error, response ) {
			map.setView( response.results[0].locations[0].displayLatLng, zoom );
		});
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 */	
	setMarkerByCoords: function( markerData, mapData ) {
		var marker = L.marker( [ 
				this.parseValue( markerData.lat ), 
				this.parseValue( markerData.lng ) 
			], {
				title: markerData.desc
			});
			
		markerData.mapIcon && marker.setIcon( new L.icon({
				iconUrl: settings.global["webRootPath"] + markerData.mapIcon,
				iconSize: [ 20, 20 ]
			})
		);
		
		if ( !mapData.clustering && !this.mapIsHeatmap( mapData ) ) {
			marker.addTo( mapData.map );
		}
		
		markerData.marker = marker;
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 * @param {function} onAddressResolvedHandler
	 */	
	setMarkerByAddress: function( markerData, mapData, onAddressResolvedHandler ) {
		var self = this;
		
		this.geoCoder.geocode( markerData.address, function( error, response ) {
			var latLng = response.results[0].locations[0].displayLatLng;
			
			// Add a marker for a location found
			markerData.lat = latLng.lat;
			markerData.lng = latLng.lng;
			
			self.setMarkerByCoords( markerData, mapData );
			onAddressResolvedHandler();	
		});
	},
	
	/**
	 * @param {L.marker} marker
	 * @param {function} handler
	 */
	addOnMarkerClickHandler: function( marker, handler ) {
		marker.on('click', handler);
	},
	
	/**
	 * @param {L.marker} marker 
	 */
	triggerMarkerClickEvent: function( marker ) {
		marker.fire('click');
	},
	
	/**
	 * @param {L.mapquest.map} map
	 * @param {L.marker} marker
	 */	
	destroyMarker: function( map, marker ) {
		marker && marker.removeFrom( map );
	},
	
	/**
	 * @param {L.mapquest.map} map
	 * @param {array} markers
	 */
	setZoomAuto: function( map, markers, boundLatLng ) {
		boundLatLng && map.fitBounds( new L.latLngBounds( [
				[ boundLatLng.minLat, boundLatLng.minLng ],
				[ boundLatLng.maxLat, boundLatLng.maxLng ]
			]));	
	},
	
	/**
	 * A stub	
	 * @param {object} map
	 */		
	triggerResizeEvent: function( map ) {},
	
	/**
	 * @param {object} map L.map
	 * @param {number} zoom
	 * @param {number} lat
	 * @param {number} lng
	 */
	setMapCenter: function( map, zoom, lat, lng ) {
		map.setView( { lat: lat, lng: lng }, zoom );
	},
	
	/**
	 * @param {object} map L.map
	 * @param {number} zoom
	 */	
	setZoom: function( map, zoom ) {
		map.setZoom( zoom ); 
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
		map.on( 'zoomend', handler );
		map.on( 'moveend', handler );
	},
	
	/**
	 * @param {object} map
	 * @return {object}
	 */
	getMapViewPortCoordinates: function( map ) {
		var bounds = map.getBounds();
		return {
			n: bounds.getNorth(),
			s: bounds.getSouth(),
			e: bounds.getEast(),
			w: bounds.getWest()
		};
	},

	/**
	 * @param {object} marker
	 * @param {object} map
	 * @return {boolean}
	 */
	mapBoundsContains: function( marker, map ) {
		return map.getBounds().contains( marker.getLatLng() );
	},
	
	/**
	 * @param {object} marker
	 * @param {boolean} customIcon
	 */
	setMarkerActive: function( marker, customIcon ) {
		if ( customIcon ) {
			marker.setIcon( new L.icon({
				iconUrl: settings.global["webRootPath"] + customIcon,
				iconSize: [ 30, 30 ]
			}) );
			return;
		};		
		
		marker.setIcon( L.mapquest.icons.marker( { size: "md", primaryColor: "#ffa500" } ) );
	},

	/**
	 * @param {object} marker
	 * @param {boolean} customIcon
	 */
	setMarkerInactive: function( marker, customIcon ) {
		if ( customIcon ) {
			marker.setIcon( new L.icon({
				iconUrl: settings.global["webRootPath"] + customIcon,
				iconSize: [ 20, 20 ]
			}) );
			return;
		};
		
		marker.setIcon( L.mapquest.icons.marker() );
	},
	
	/**
	 * @param {object} mapData
	 * @return {boolean}
	 */
	mapIsHeatmap: function( mapData ) {
		return mapData && mapData.markers && mapData.markers.length && mapData.heatMap && !mapData.clustering;
	},
	
	/**
	 * Initialize a heat map
	 */
	initHeatMap: function( mapData ) {
		if ( !mapData.map ) {
			return;
		}		
		
		var weights = mapData.markers.map( function( markerData ) {
				return markerData.weight === undefined ? 1 : parseFloat( markerData.weight );
			}),
			// get max to normalize points intencity
			maxWeight = Math.max.apply( null, weights ) || 1;
		
		var dataPoints = mapData.markers
			.filter( function( markerData ) {
				return markerData.marker;
			})
			.map( function( markerData ) {
				var latLng = markerData.marker.getLatLng();
				if ( markerData.weight === undefined ) {
					return [ latLng.lat, latLng.lng ];
				}
				return [ latLng.lat, latLng.lng, parseFloat( markerData.weight ) / maxWeight ];
			});
		
		L.heatLayer( dataPoints, { 
				maxZoom: 4,
				minOpacity: 0.5,
				gradient: {
					0.4: 'green', 
					0.65: 'yellow', 
					1: 'red'
				}
			})
			.addTo( mapData.map );
	},
	
	/**
	 * @param {object} mapData
	 * @return {object}
	 */
	initializeClusterMarkers: function( mapData ) {
		if ( !mapData.markers || !mapData.markers.length ) {
			return;
		}
		
		var cluster = L.markerClusterGroup();

		mapData.markers.forEach( function( markerData ) {
			if ( markerData.marker ) {
				cluster.addLayer( markerData.marker );
			}
		});

		mapData.map.addLayer( cluster );
		return cluster;
	},
	
	/**
	 * @param {object} markerData
	 * @return {object}
	 */
	isClusterHandlerToAdd: function( markerData ) {
		return false;
	},
	
	/**
	 * @param {object} markerClusterer
	 * @param {object} marker
	 */
	destroyClusterMarker: function( markerClusterer, marker ) {
		marker && marker.removeFrom( markerClusterer );
	},
	
	/**
	 * @param {object} mapData
	 */	
	destroyMap: function( mapData ) {
		mapData.map && mapData.map.remove();
		delete mapData.map;
	}	
});
