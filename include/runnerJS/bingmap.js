/**
 * The class containing specific functions using Bing Maps API
 * https://msdn.microsoft.com/en-us/library/gg427610.aspx
 * @class Runner.controls.gMap
 */
Runner.controls.bingMap = Runner.extend( Runner.emptyFn, {
	/**
	 *
	 */
	APIkey: window.settings['global'].mapsApiCode,
	
	/**
	 *
	 */
	defaultPPIconUrl: "",
	
	/**
	 *
	 */
	defaultMarkerColor: "darkmagenta",
	activeMarkerColor: "red",
	decorationPadding: 10,
	decorationOffsetPoint: null,
	
	/**
	 * @param {jQuery object} $mapElem
	 * @param {number} zoom
	 * @param {number} centerLat
	 * @param {number} centerLng	 
	 * @return {Microsoft.Maps.Map object}
	 */
	createMap: function( $mapElem, zoom, centerLat, centerLng ) {
		var mapSettings = {
				credentials: this.APIkey, 
				height: $mapElem.get(0).clientHeight, 
				width: $mapElem.get(0).clientWidth,
				showDashboard: false,
				enableClickableLogo: false 		
			};
			
		if ( centerLat !== undefined && centerLng !== undefined ) {
			mapSettings.center = this.mapsLatLng( centerLat, centerLng )		
		}
		
		$mapElem.html(""); 
		return new Microsoft.Maps.Map( $mapElem.get(0), mapSettings );
	},
	
	/**
	 * @param {object} data
	 * @return {object}
	 */
	getLocationByAddress: function( data ) {
		var lat, lng;

		if ( data["statusCode"] != 200 ) {
			return this.mapsLatLng( 0, 0 );
		}
		
		lat = this.getLatLngFromAjax( data, "lat" );
		lng = this.getLatLngFromAjax( data, "lng" );
		
		return this.mapsLatLng( lat, lng );
	},
	
	/**
	 * @param {string} address
	 * @param {object} map
	 * @param {number} zoom
	 */	
	setCenter: function( address, map, zoom ) {
		var that = this;
		
		$.getJSON( this.urlLocationFromAddress( address ), function( data ) {
			map.setView( {
				center: that.getLocationByAddress( data ), 
				zoom: zoom
			} );
		});
	},
	
	/**
	 * @param {} lat
	 * @param {} lng
	 * @return {object}
	 */
	mapsLatLng: function( lat, lng ) {
		return new Microsoft.Maps.Location( lat, lng );
	},
	
	/**
	 * @param {object} marker
	 * @param {object} mapLocation
	 * @param {string} desc
	 * @return {object}
	 */
	getMapInfoBox: function( marker, mapLocation, desc ) {
		var infoboxOptions = {
				title: desc, 
				offset: new Microsoft.Maps.Point( 6, 6 ),
				showCloseButton: false,
				showPointer: false,
				visible: false
			}, 
			infobox = new Microsoft.Maps.Infobox( mapLocation, infoboxOptions );

		Microsoft.Maps.Events.addHandler( marker, "mouseover", function(e) { 
			infobox.setOptions( { visible: true } );
		});
		
		Microsoft.Maps.Events.addHandler( marker, "mouseout", function(e) { 
			infobox.setOptions( { visible: false } ); 
		});
		
		return infobox;
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 */		
	setMarkerByCoords: function( markerData, mapData ) {
		var mapLocation = this.mapsLatLng( this.parseValue( markerData.lat ), this.parseValue( markerData.lng ) ),
			mapIcon, pinInfobox;
		
		if ( markerData.mapIcon ) {
			mapIcon = settings.global["webRootPath"] + markerData.mapIcon;
		}
		
		markerData.marker = new Microsoft.Maps.Pushpin( mapLocation, { icon: mapIcon, color: this.defaultMarkerColor } );
		pinInfobox = this.getMapInfoBox( markerData.marker, mapLocation, markerData.desc );	
		
		mapData.map.entities.push( markerData.marker );
		pinInfobox.setMap( mapData.map );
	},
	
	/**
	 * @param {object} markerData
	 * @param {object} mapData
	 * @param {function} onAddressResolvedHandler
	 */	
	setMarkerByAddress: function( markerData, mapData, onAddressResolvedHandler ) {
		var that = this;
		
		$.getJSON( this.urlLocationFromAddress( markerData.address ), function( data ) {
			var mapLocation = that.getLocationByAddress( data ), 
				mapIcon, marker, pinInfobox;
			
			if ( markerData.mapIcon ) {
				mapIcon = settings.global["webRootPath"] + markerData.mapIcon;
			}
			
			marker = new Microsoft.Maps.Pushpin( mapLocation, { icon: mapIcon, color: this.defaultMarkerColor } );
			pinInfobox = that.getMapInfoBox( marker, mapLocation, markerData.desc );
			
			mapData.map.entities.push( marker );
			pinInfobox.setMap( mapData.map );
			
			markerData.lat = that.getLatLngFromAjax( data, "lat" );
			markerData.lng = that.getLatLngFromAjax( data, "lng" );
			markerData.marker = marker;
			
			onAddressResolvedHandler();
		});
	},
	
	/**
	 * @param {object} marker
	 * @param {function} handler
	 */
	addOnMarkerClickHandler: function( marker, handler ) { 
		Microsoft.Maps.Events.addHandler( marker, "click", handler );
	},
	
	/**
	 * @param {object} marker 
	 */
	triggerMarkerClickEvent: function( marker ) {
		Microsoft.Maps.Events.invoke( marker, "click" );
	},	
	
	/**
	 * @param {object} mapData
	 * @param {} markerData
	 */		
	destroyMarkers: function( mapData, markerData ) {
		mapData.map.entities.clear();
	},
	
	/**
	 * @param {object} map
	 * @param {object} marker
	 */	
	destroyMarker: function( map, marker ) {
		map.entities.remove( marker );
	},

	/**
	 * @param {object} map
	 * @param {array} markers
	 */
	setZoomAuto: function( map, markers, boundLatLng ) {
		var arrLocation = [], 
			viewRect;
		
		if ( boundLatLng ) {
			arrLocation.push( this.mapsLatLng( markers[ boundLatLng.minLatID ].lat, markers[ boundLatLng.minLatID ].lng ) );
			arrLocation.push( this.mapsLatLng( markers[ boundLatLng.minLngID ].lat, markers[ boundLatLng.minLngID ].lng ) );
			arrLocation.push( this.mapsLatLng( markers[ boundLatLng.maxLatID ].lat, markers[ boundLatLng.maxLatID ].lng ) );
			arrLocation.push( this.mapsLatLng( markers[ boundLatLng.maxLngID ].lat, markers[ boundLatLng.maxLngID ].lng ) );
			
			viewRect = Microsoft.Maps.LocationRect.fromLocations( arrLocation );		
			map.setView( {bounds: viewRect} );			
		}	
	},
	
	/**
	 * @param {object markerPos
	 * @return {object}
	 */		
	markerPosition: function( markerPos ) {
		return markerPos.position;
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
		map.setView({
			center: this.mapsLatLng( lat, lng ), 
			zoom: zoom
		});
	},
	
	/**
	 * @param {object} map
	 * @param {number} zoom
	 */
	setZoom: function( map, zoom ) { 
		map.setView( {zoom: zoom} );
	},
	
	/**
	 * @param {object} data
	 * @param {string} param
	 * @return {}
	 */
	getLatLngFromAjax: function( data, param ) {
		return data["resourceSets"][0] && data["resourceSets"][0]["resources"][0] ? data["resourceSets"][0]["resources"][0]["geocodePoints"][0]["coordinates"][ param == "lat" ? 0 : 1 ] : 0;
	},
	
	/**
	 * @param {string} address
	 * @return {string}
	 */
	urlLocationFromAddress: function( address ) {
		return 'http://dev.virtualearth.net/REST/v1/Locations?query=' + encodeURI( address ) + '&output=json&jsonp=?&key=' + this.APIkey;
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
		Microsoft.Maps.Events.addHandler( map, "viewchangeend", handler );		
	},
	
	/**
	 * @param {object} map
	 * @return {object}
	 */
	getMapViewPortCoordinates: function( map ) { 
		var mapBounds = map.getBounds();
		
		return {
			n: mapBounds.getNorth(),
			s: mapBounds.getSouth(),
			e: mapBounds.getEast(),
			w: mapBounds.getWest()		
		};
	},

	/**
	 * @param {object} marker
	 * @param {object} map
	 * @return {boolean}
	 */
	mapBoundsContains: function( marker, map ) {
		var mapBounds = map.getBounds();
		
		return mapBounds && mapBounds.contains( marker.getLocation() );
	},
	
	/**
	 * @param {Microsoft.Maps.Pushpin object} marker
	 * @param {ыекштп} customIcon	 
	 */
	setMarkerActive: function( marker, customIcon ) {
		if ( !marker ) {
			return;
		}

		if ( customIcon ) {
			this.decorateMarkerIcon( marker, settings.global["webRootPath"] + customIcon );
		} else {
			marker.setOptions( { color: this.activeMarkerColor } );
		}			
	},
	
	/**
	 * @return {object Microsoft.Maps.Point}
	 */
	getDecorationOffsetPoint: function() {
		if ( !this.decorationOffsetPoint ) { 
			this.decorationOffsetPoint = new Microsoft.Maps.Point( this.decorationPadding / 2, this.decorationPadding / 2 );
		}
		
		return this.decorationOffsetPoint;
	},
	
	/**
	 * @param {object} marker
	 * @param {string} icon
	 */
	decorateMarkerIcon: function( marker, icon ) {
		var img = new Image(),
			padding = this.decorationPadding,
			offsetPoint = this.getDecorationOffsetPoint();
		
		img.onload = function () {
			var c = document.createElement('canvas'),
				ctx, r;

			c.width = img.width + padding;
			c.height = img.height + padding;

			ctx = c.getContext('2d');
			ctx.strokeStyle = 'red';
			ctx.lineWidth = 2;

			r = Math.max( c.width, c.height ) / 2 - ctx.lineWidth;

			//Draw a circle around the pin
			ctx.beginPath();
			ctx.arc( c.width / 2, c.width / 2, r , 0, 2 * Math.PI );
			ctx.stroke();

			//Draw the pushpin icon
			ctx.drawImage( img, padding / 2, padding / 2 );

			marker.setOptions({ 
				icon: c.toDataURL(), 
				anchor: ( marker.getAnchor() || new Microsoft.Maps.Point( 0, 0 ) ).add( offsetPoint ) 
			});
		};
		
		img.src = icon;		
	},
	
	/**
	 * @param {Microsoft.Maps.Pushpin object} marker
	 * @param {mixed} customIcon
	 */
	setMarkerInactive: function( marker, customIcon ) {
		if ( !marker ) {
			return;
		}

		if ( customIcon ) {	
			marker.setOptions({ 
				icon: settings.global["webRootPath"] + customIcon, 
				anchor: marker.getAnchor().subtract( this.getDecorationOffsetPoint() ) 
			});
		} else {
			marker.setOptions( { color: this.defaultMarkerColor } );
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
