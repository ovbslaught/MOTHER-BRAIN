/**
 * The class containing specific functions using Google Maps API
 * https://developers.google.com/maps/documentation/javascript/reference
 * @class Runner.controls.gMap
 */
Runner.controls.gMap = Runner.extend( Runner.emptyFn, {
	/**
	 * The Google Maps Geocoder object
	 * @type {object}
	 */
	geoCoder: null,
	
	/**
	 * google.maps.visualization.HeatmapLayer
	 * @type {object}
	 */
	heatMapLayer: null,
	
	/**
	 * @constructor
	 */
	constructor: function() {
		this.geoCoder = new google.maps.Geocoder();
	},
	
	/**
	 * @param {jQuery object} $mapElem
	 * @param {number} zoom (required)
	 * @param {number} centerLat
	 * @param {number} centerLng
	 * @return {google.maps.Map object}
	 */
	createMap: function( $mapElem, zoom, centerLat, centerLng ) {	
		return new google.maps.Map( $mapElem.get(0), {
				zoom: zoom,
				center: new google.maps.LatLng( centerLat || 0, centerLng || 0 ),
				mapTypeId: google.maps.MapTypeId.ROADMAP 
			});
	},
	
	/**
	 * @param {string} address
	 * @param {object} map
	 * @param {number} zoom
	 */		
	setCenter: function( address, map, zoom ) {
		this.geoCoder.geocode( {address: address}, function( geoResult, status ) {
			if ( status == google.maps.GeocoderStatus.OK ) {
				map.setCenter( geoResult[0].geometry.location, zoom );									
			}
		});
	},
	
	/**
	 * @param {} lat
	 * @param {} lng
	 * @return {object}
	 */
	mapsLatLng: function( lat, lng ) {
		return new google.maps.LatLng( lat, lng );
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 */		
	setMarkerByCoords: function( markerData, mapData ) {
		var position = this.mapsLatLng( this.parseValue( markerData.lat ), this.parseValue( markerData.lng ) ),
			rstep = .0004,
			marker, icon;

		if ( mapData.clustering ) {
			mapData.addedCoordinates = mapData.addedCoordinates || {};
			mapData.clusterMarkers = mapData.clusterMarkers || [];
			
			while( mapData.addedCoordinates[ this.coordToString( position ) ] ) {
				position = new google.maps.LatLng( position.lat() + rstep * ( Math.random() - .5 ), position.lng() + rstep * ( Math.random() - .5 ) );
			}
		}
		
		if ( this.mapIsHeatmap( mapData ) ) {
			marker = {
				position: position
			};
		} else {
			icon = null;
			
			if ( markerData.mapIcon ) {
				icon = { url: settings.global["webRootPath"] + markerData.mapIcon };
			}
			
			marker = new google.maps.Marker({
				position: position,
				title: markerData.desc,		
				icon: icon 
			});
			
			marker.setMap( mapData.map );
		}
		
		markerData.marker = marker;
		
		if ( mapData.clustering && marker ) {
			mapData.clusterMarkers.push( marker );
			mapData.addedCoordinates[ this.coordToString( marker.getPosition() ) ] = true;
		}			
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 * @param {function} onAddressResolvedHandler
	 */			
	setMarkerByAddress: function( markerData, mapData, onAddressResolvedHandler ) {	
		this.geoCoder.geocode( { address: markerData.address }, function( geoResult, status ) {
			var marker, icon;
			
			if ( status != google.maps.GeocoderStatus.OK ) {
				return;
			}
			
			icon = null;
			
			if ( markerData.mapIcon ) {
				icon = { url: settings.global["webRootPath"] + markerData.mapIcon };
			}
			marker = new google.maps.Marker({
				position: geoResult[0].geometry.location,
				icon: icon,
				title: markerData.desc
			});

			marker.setMap( mapData.map );
			
			markerData.lat = geoResult[0].geometry.location.lat().toString();
			markerData.lng = geoResult[0].geometry.location.lng().toString();			
			markerData.marker = marker;
			
			onAddressResolvedHandler();
		});
	},
	
	/**
	 * @param {object} marker
	 * @param {function} handler
	 */
	addOnMarkerClickHandler: function( marker, handler ) {
		google.maps.event.addListener( marker, "click", handler );
	},
	
	/**
	 * @param {object} marker 
	 */
	triggerMarkerClickEvent: function( marker ) {
		google.maps.event.trigger( marker, "click" );
	},
	
	/**
	 * @param {object} map
	 * @param {object} marker
	 */		
	destroyMarker: function( map, marker ) {
		if ( marker && marker instanceof google.maps.Marker ) {
			marker.setMap( null );
		}
	},
	
	/**
	 * @param {object} map
	 * @param {array} markers
	 */
	setZoomAuto: function( map, markers, boundLatLng ) {
		var newMapBounds = new google.maps.LatLngBounds();
		
		if ( boundLatLng ) {
			newMapBounds.extend( markers[ boundLatLng.minLatID ].marker.position );
			newMapBounds.extend( markers[ boundLatLng.minLngID ].marker.position );
			newMapBounds.extend( markers[ boundLatLng.maxLatID ].marker.position );
			newMapBounds.extend( markers[ boundLatLng.maxLngID ].marker.position );
		}
		
		map.fitBounds( newMapBounds );
	},
	
	/**
	 * @param {object} map
	 */		
	triggerResizeEvent: function( map ) {
		google.maps.event.trigger( map, 'resize' );
	},
	
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
		google.maps.event.addDomListenerOnce( map, "tilesloaded", handler );
	},
	
	/**
	 * @param {object} map
	 * @param {function} handler
	 */
	addOnMapViewPortChangedHandler: function( map, handler ) {
		google.maps.event.addListener( map, "dragend", handler );
		google.maps.event.addListener( map, "zoom_changed", handler );
	},
	
	/**
	 * @param {object} map
	 * @return {object}
	 */
	getMapViewPortCoordinates: function( map ) {
		var mapBounds = map.getBounds(),
			ne, sw;
		
		if ( !mapBounds ) {
			return null;
		}
		
		ne = mapBounds.getNorthEast();
		sw = mapBounds.getSouthWest();	
		
		return {
			n: ne.lat(),
			s: sw.lat(),
			e: ne.lng(),
			w: sw.lng()
		};	
	},

	/**
	 * @param {object} marker
	 * @param {object} map
	 * @return {boolean}
	 */
	mapBoundsContains: function( marker, map ) {
		var mapBounds = map.getBounds();
		
		return mapBounds && mapBounds.contains( marker.getPosition() );
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
			marker.setIcon({ 
				//url: "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|FFFF00",
				url: settings.global["webRootPath"] + "images/icons/gYellowMarker.png",
				size: new google.maps.Size(26, 42),
				scaledSize: new google.maps.Size(26, 42)
			});
			//marker.setIcon( { url: "images/icons/gYellowMarker.png" } );
			return;
		}
			
		marker.setAnimation( google.maps.Animation.BOUNCE );
		setTimeout( function() { 
			marker.setAnimation( null );
		}, 500 );		
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
			marker.setIcon( null );
		} else {	
			marker.setAnimation( null );
		}
	},
	
	/**
	 * @param {object} mapData
	 * @return {boolean}
	 */
	mapIsHeatmap: function( mapData ) {
		return mapData && mapData.markers && mapData.markers.length && mapData.heatMap && !mapData.clustering;
	},
	
	/**
	 * Initialize a particular heat map
	 */
	initHeatMap: function( mapData ) {
		var map = mapData.map,
			data = [],
			i, weight, markerHeatData;
			
		if ( !map || !this.mapIsHeatmap( mapData ) ) {
			return;
		}
		
		for (i = 0; i < mapData.markers.length; i++) {
			weight = parseFloat( mapData.markers[ i ].weight );
			
			if ( mapData.weightField && !weight ) {
				continue;
			}
			
			markerHeatData = {
				location: new google.maps.LatLng( mapData.markers[ i ].lat, mapData.markers[ i ].lng )
			};
			
			if ( weight ) {
				markerHeatData.weight = weight;
			}
			
			data.push( markerHeatData );
		}
		
		if ( this.heatMapLayer ) {
			this.heatMapLayer.setMap( null );
		}
		
		this.heatMapLayer = new google.maps.visualization.HeatmapLayer({
			data: data,
			maxIntensity: 100,
			dissipating: true,
			radius: 40
		});
		
		this.heatMapLayer.setMap( map );		
	},
	
	/**
	 * @param {object} mapData
	 * @return {object}
	 */
	initializeClusterMarkers: function( mapData ) {
		return new MarkerClusterer( mapData.map, mapData.clusterMarkers, {'imagePath': settings.global["webRootPath"] + 'images/m' } );
	},
	
	/**
	 * @param {object} markerData
	 * @return {object}
	 */
	isClusterHandlerToAdd: function( markerData ) {
		return !!markerData.subMarkers;
	},
	
	/**
	 *
	 */
	addClusterClickHandler: function(  markerData, markerClusterer  ) {
		this.addOnMarkerClickHandler( markerData.marker, function() {
			this.clusterHandler( markerData );
		});		
	},
	
	/**
	 * @param {object} markerData
	 */
	clusterHandler: function( markerData ) {
		if ( !markerData.subMarkers ) {
			return;
		}
		
		var map = markerData.marker.getMap();
		
		map.setCenter( markerData.marker.position );
		map.setZoom( map.getZoom() + 1 );
	},
	
	/**
	 * @param {object} latlng
	 * @return {string}
	 */
	coordToString: function( latlng ) {
		return Math.round( latlng.lat() * 100000 ).toString() + ' ' + Math.round( latlng.lng() * 100000 ).toString();
	},

	destroyClusterMarker: function( markerClusterer, marker ) {
		markerClusterer.removeMarker( marker );	
	}		
});
