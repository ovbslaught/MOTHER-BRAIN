/**
 * The class containing specific functions using HERE Maps API
 * https://developer.here.com/documentation/maps/3.1.16.1/dev_guide/index.html
 * @class Runner.controls.hereMap
 */
Runner.controls.hereMap = Runner.extend( Runner.emptyFn, {
	/**
	 * @type {object}
	 */
	geoCoder: null,
	
	/**
	 * @type {object} H.service.Platform
	 */
	platform: null,
	
	/**
	 * H.map.DomIcon
	 */
	defaultMarkerIcon: null,
	activeMarkerIcon: null,
	
	/**
	 *
	 */
	defaultLayers: null,
	
	/**
	 * @constructor
	 */
	constructor: function() {
		this.platform = new H.service.Platform({
			'apikey': window.settings['global'].mapsApiCode
		});
		
		this.geoCoder = this.platform.getSearchService();
		this.defaultLayers = this.platform.createDefaultLayers();
			
		this.defaultMarkerIcon = this.getMarkerDomIcon("#01b6b2");
		this.activeMarkerIcon = this.getMarkerDomIcon("#ffa500");
	},
	
	/**
	 * @param {string} baseColor
	 * @return {string}
	 */
	getMarkerDomIcon: function( baseColor ) {
		return new H.map.DomIcon( '<svg xmlns="http://www.w3.org/2000/svg" width="25" height="31" viewBox="0 0 38 47">' +
			'<g fill="none">' + 
				'<path fill="#0F1621" fill-opacity=".4" d="M15 46c0 .317 1.79.574 4 .574s4-.257 4-.574c0-.317-1.79-.574-4-.574s-4 .257-4 .574z"></path>' +
				'<path fill="' + baseColor + '" d="M33.25 31.652A19.015 19.015 0 0 0 38 19.06C38 8.549 29.478 0 19 0S0 8.55 0 19.059c0 4.823 1.795 9.233 4.75 12.593L18.975 46 33.25 31.652z"></path>' + 
				'<path fill="#6A6D74" fill-opacity=".5" d="M26.862 37.5l4.714-4.77c3.822-3.576 5.924-8.411 5.924-13.62C37.5 8.847 29.2.5 19 .5S.5 8.848.5 19.11c0 5.209 2.102 10.044 5.919 13.614l4.719 4.776h15.724zM19 0c10.493 0 19 8.525 19 19.041 0 5.507-2.348 10.454-6.079 13.932L19 46 6.079 32.973C2.348 29.495 0 24.548 0 19.04 0 8.525 8.507 0 19 0z"></path>' + 
			'</g></svg>');	
	},	
	
	/**
	 * @param {jQuery object} $mapElem
	 * @param {number} zoom (required)
	 * @param {number} centerLat
	 * @param {number} centerLng
	 * @return {object} H.Map
	 */
	createMap: function( $mapElem, zoom, centerLat, centerLng ) {		
		var map = this._getNewMap( $mapElem.get(0), zoom );
			
		if ( centerLat !== undefined && centerLng !== undefined ) {
			map.setCenter( { lat: centerLat, lng: centerLng } );
		}
	
		// MapEvents enables the event system
		var behavior = new H.mapevents.Behavior( new H.mapevents.MapEvents( map ) );	
		return map;
	},
	
	/**
	 * Get vector or raster
	 * @param { object } mapElem
	 * @param {number} zoom (required)
	 * @return {object} H.Map
	 */
	_getNewMap: function( mapElem, zoom ) {
		var WebGL2support = !!document.createElement('canvas').getContext('webgl2');
		if ( WebGL2support ) {
			return new H.Map( mapElem, 
				this.defaultLayers.vector.normal.map, {
					zoom: zoom
				});
		}
		
		return new H.Map( mapElem, 
			this.defaultLayers.raster.normal.map, {
				zoom: zoom,
				engineType: H.map.render.RenderEngine.EngineType.P2D
			});
	},
	
	/**
	 * @param {string} address
	 * @param {object} map H.Map
	 * @param {number} zoom
	 */		
	setCenter: function( address, map, zoom ) {
		this.geoCoder.geocode({
				q: address
			}, function( result ) {
				map.setCenter( result.items[0].position );
				map.setZoom( zoom );
			}/*, error callback */);
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 */		
	setMarkerByCoords: function( markerData, mapData ) {		
		var marker = new H.map.DomMarker({ 
				lat: this.parseValue( markerData.lat ), 
				lng: this.parseValue( markerData.lng ) 
			});
			
		marker.setIcon( markerData.mapIcon ? 
			new H.map.DomIcon( '<img src="' + settings.global["webRootPath"] + markerData.mapIcon + '">' ) :
			this.defaultMarkerIcon 
		);
		
		markerData.marker = marker;
		if ( !mapData.clustering && !this.mapIsHeatmap( mapData ) ) {
			mapData.map.addObject( marker );
		}
		
		if ( markerData.desc ) {
			mapData.ui = mapData.ui || H.ui.UI.createDefault( mapData.map, this.defaultLayers );
			var bubble = new H.ui.InfoBubble( marker.getGeometry(), {
				content: '<span>' + markerData.desc + '</span>'
			});
			
			//bubble.addClass("rnr-map-description");
			bubble.close();
			mapData.ui.addBubble( bubble );
			
			// ХХХ: hack to hide an inner close button
			$( ".H_ib_close", bubble.getElement() ).hide();
			
			$( bubble.getContentElement() ).css({
				margin: "5px 10px",
				"font-size": "13px",
				"line-height": "20px",
			});
			
			marker.addEventListener('pointerenter', function( evt ) {
				bubble.open();
			});
			marker.addEventListener('pointerleave', function( evt ) {
				bubble.close();
			});
		}
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 * @param {function} onAddressResolvedHandler
	 */	
	setMarkerByAddress: function( markerData, mapData, onAddressResolvedHandler ) {
		var self = this;
		this.geoCoder.geocode({
				q: markerData.address
			}, function( result ) {
				// Add a marker for a location found
				markerData.lat = result.items[0].position.lat;
				markerData.lng = result.items[0].position.lng;
				
				self.setMarkerByCoords( markerData, mapData );
				
				onAddressResolvedHandler();
			}/*, error callback */ ); 
	},
	
	/**
	 * @param {object} marker
	 * @param {function} handler
	 */
	addOnMarkerClickHandler: function( marker, handler ) {
		marker.addEventListener('tap', handler);
	},
	
	/**
	 * @param {H.map.Marker} marker 
	 */
	triggerMarkerClickEvent: function( marker ) {
		marker.dispatchEvent('tap');
	},
	
	/**
	 * @param {object} map
	 * @param {object} marker
	 */	
	destroyMarker: function( map, marker ) {
		marker && marker.getProvider() && map.removeObject( marker );
	},
	
	/**
	 * @param {object} map
	 * @param {array} markers
	 */
	setZoomAuto: function( map, markers, boundLatLng ) {
		boundLatLng && map.getViewModel().setLookAtData({
			bounds: new H.geo.Rect(
				boundLatLng.maxLat,
				boundLatLng.minLng,
				boundLatLng.minLat,	
				boundLatLng.maxLng
			)
		});		
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
		map.setCenter( {lat: lat, lng: lng} ); 
		map.setZoom( zoom );
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
		handler();
	},
	
	/**
	 * @param {object} map
	 * @param {function} handler
	 */
	addOnMapViewPortChangedHandler: function( map, handler ) {
		map.addEventListener( 'mapviewchangeend', handler );
	},
	
	/**
	 * @param {object} map
	 * @return {object} H.geo.Rect
	 */
	_getMapBoundingBox: function( map ) {
		//H.geo.AbstractGeometry
		var bounds = map.getViewModel().getLookAtData().bounds;
		//H.geo.Rect
		return bounds.getBoundingBox();
	},
	
	/**
	 * @param {object} map
	 * @return {object}
	 */
	getMapViewPortCoordinates: function( map ) {
		//H.geo.Rect
		var bbox = this._getMapBoundingBox( map );
		
		if ( !bbox ) {
			return null;
		}
		
		return {
			n: bbox.getTop(),
			s: bbox.getBottom(),
			e: bbox.getRight(),
			w: bbox.getLeft(),
		};
	},

	/**
	 * @param {object} marker
	 * @param {object} map
	 * @return {boolean}
	 */
	mapBoundsContains: function( marker, map ) {
		//H.geo.Rect
		var bbox = this._getMapBoundingBox( map ),
			postion = marker.getGeometry();
		
		return bbox && bbox.containsLatLng( postion.lat, postion.lng );
	},
	
	/**
	 * @param {object} marker
	 * @param {boolean} customIcon
	 */
	setMarkerActive: function( marker, customIcon ) {
		marker && marker.setIcon( this.activeMarkerIcon );
	},

	/**
	 * @param {object} marker
	 * @param {boolean} customIcon
	 */
	setMarkerInactive: function( marker, customIcon ) {	
		marker && marker.setIcon( customIcon ?
			new H.map.DomIcon( '<img src="' + settings.global["webRootPath"] + customIcon + '">' ) :
			this.defaultMarkerIcon
		);	
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
		
		var colors = new H.data.heatmap.Colors({
				'0': 'rgba(0, 255, 0, 0.5)',   // half-transparent green
				'0.5': 'rgba(255, 255, 0, 0.5)', // half-transparent yellow
				'1': 'rgba(255, 0, 0, 0.5)'  // half-transparent red 
			},			
			true  // interpolate between the stops to create a smooth color gradient
		);		
		
		var heatmapProvider = new H.data.heatmap.Provider({
			type: 'value',
			colors: colors
		});
		
		var heatData = mapData.markers
			.map( function( markerData ) {
				if ( markerData.weight === undefined ) {
					return { lat: markerData.lat, lng: markerData.lng };
				}
				return { lat: markerData.lat, lng: markerData.lng, value: parseFloat( markerData.weight ) };
			})
			.filter( function( data ) {
				return !mapData.weightField || !!data.value;
			});
			
		
		heatmapProvider.addData( heatData );
		
		mapData.map.addLayer( new H.map.layer.TileLayer( heatmapProvider, {
			opacity: 0.6 
		}) );
		return heatmapProvider;
	},
	
	/**
	 * @param {object} mapData
	 * @return {object}
	 */
	initializeClusterMarkers: function( mapData ) {
		if ( !mapData.markers || !mapData.markers.length ) {
			return;
		}
		
		var dataPoints = mapData.markers.map( function( markerData ) {
			var data = markerData.marker.getData() || {};
			data.dataPoint = new H.clustering.DataPoint( markerData.lat, markerData.lng, null, markerData );
			markerData.marker.setData( data );
			return data.dataPoint;
		});

		var clusteredDataProvider = new H.clustering.Provider( dataPoints/*, {
			clusteringOptions: {},
		}*/);

		var self = this;	
		var theme = clusteredDataProvider.getTheme();
		theme.getNoisePresentation = function( noisePoint ) {
			var data = noisePoint.getData(),
				noiseMarker = new H.map.DomMarker( noisePoint.getPosition(), {
					min: noisePoint.getMinZoom(),
					icon: self.defaultMarkerIcon
				});
		
			noiseMarker.setData( data );
			return noiseMarker;
		};
		clusteredDataProvider.setTheme( theme );
		
		
		var layer = new H.map.layer.ObjectLayer( clusteredDataProvider );
		mapData.map.addLayer( layer );
		
		
		clusteredDataProvider.addEventListener('tap', function(event) {
			var result = event.target.getData();
			
			if ( result.marker ) {
				// noisePoint
				result.marker.dispatchEvent('tap');
			} else if ( result.isCluster && result.isCluster() ) {
				mapData.map.setCenter( result.getPosition() );
				mapData.map.setZoom( mapData.map.getZoom() + 1 );
			}
		});
		return clusteredDataProvider;
	},
	
	/**
	 * @param {object} markerData
	 * @return {object}
	 */
	isClusterHandlerToAdd: function( markerData ) {
		return false;
	},
	
	destroyClusterMarker: function( markerClusterer, marker ) {
		var data = marker.getData();
		data && data.dataPoint && markerClusterer.removeDataPoint( data.dataPoint );
	},
	
	destroyMap: function( mapData ) {
		mapData.map && mapData.map.dispose();
		delete mapData.map;
		delete mapData.ui;
	}
});
