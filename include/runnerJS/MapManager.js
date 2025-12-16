/**
 * Maps manager constructor
 * @class Runner.controls.MapManager
 */
Runner.controls.MapManager = Runner.extend( Runner.emptyFn, {
	/**
	 * The host page's id
	 */
	id: -1,
	
	/**
	 * The maps host page's table name
	 */
	tName: '',
		
	/**
	 * Gmaps data map, contains objects with data about maps
	 */
	mapsData: {},
	
	/**
	 * "Big" maps id values
	 */
	mainMapIds: [],
	
	/**
	 * "Little" field maps' id values
	 */
	fieldMapsIds: [],

	/**
	 * The object having the names of the fields shown as maps as keys
	 * and containing fields maps settings data
	 */
	fieldsAsMap: {},

	/**
	 * The indicator showing if click on marker opens the edit page
	 * @type {boolean}
	 */
	markerAsEditLink: false,	

	showViewInPopup: false,
	
	showEditInPopup: false, 	
	
	centerLinkText: '',

	bigMapDefZoom : 5,

	littleMapDefZoom : 5,
	
	/**
	 *
	 */
	providerMap: null,
	
	/**
	 *
	 */
	hostPage: null,
	
	/**
	 *
	 */
	useEmbedMapsAPI: false,
	
	/**
	 * @constructor
	 * @param {object} cfg
	 */
	constructor: function( cfg ) {
		Runner.apply( this, cfg );
		Runner.controls.MapManager.superclass.constructor.call( this, cfg );
		
		this.hostPage = Runner.pages.PageManager.getAt( this.tName, this.id );
		this.showViewInPopup = this.hostPage && this.hostPage.showViewInPopup();
		this.showEditInPopup = this.hostPage && this.hostPage.showEditInPopup();

		this.providerMap = this.getMapProvider();	
	},

	/**
	 * Get an object providing map operations
	 * @return {object}
	 */
	getMapProvider: function() {
		switch ( Runner.pages.PageSettings.getGlobalData("mapProvider") ) {
			case Runner.controls.constants.GOOGLE_MAPS:
				return new Runner.controls.gMap();
				
			case Runner.controls.constants.OPEN_STREET_MAPS:
				return new Runner.controls.osMap();
				
			case Runner.controls.constants.BING_MAPS:
				return new Runner.controls.bingMap();
				
			case Runner.controls.constants.HERE_MAPS:
				return new Runner.controls.hereMap();
				
			case Runner.controls.constants.MAPQUEST_MAPS:
				return new Runner.controls.mapQuestMap();				
		}	
	},	
	
	/**
	 * Initialize the page's maps and grid links 
	 * @param {object} mapsData
	 * @public
	 */
	init: function( mapsData ) {
		if ( mapsData ) {
			this.disposeCurrentMaps();
		}
	
		this.mapsData = mapsData || this.mapsData;
		
		for ( var mapId in this.mapsData ) {
			if ( !this.mapsData[ mapId ].skipped ) {
				
				this.initMap( mapId );
				
				if ( this.isHeatMap( this.mapsData[ mapId ] ) ) {
					this.initHeatMap( mapId );
				}				
			}
		}

		this.initCenterLinks();
	},
	
	/**
	 * destroy old maps
	 */
	disposeCurrentMaps: function() {
		for ( var mapId in this.mapsData ) {
			if ( !this.mapsData[ mapId ].skipped ) {
				this.providerMap.destroyMap && this.providerMap.destroyMap( this.mapsData[ mapId ] );
			}
		}
	},
	
	/**
	 * @param {object} mapsData
	 * @param {array} fieldMapsIds
	 */
	reinit: function( mapsData, fieldMapsIds ) {
		this.fieldMapsIds = fieldMapsIds || this.fieldMapsIds;
		this.init( mapsData );
	},
	
	/**
	 * Initialize a particular map
	 * @param {string} mapId
	 * @param {function} onMapLoadHandler (optional)
	 */
	initMap: function( mapId, onMapLoadHandler ) {
		var $mapElem = $('#' + mapId),		
			mapData = this.mapsData[ mapId ],
			mapManager = this;
		
		if ( !$mapElem.length || !mapData ) {
			return false;
		}				
		
		this.providerMap.destroyMap && this.providerMap.destroyMap( mapData );		
		
		if ( (mapData.type == 'BIG_MAP' || mapData.type == 'DASH_MAP' ) 
			&& ( mapData.zoom === "auto" || typeof mapData.zoom === "undefined" ) ) {
			mapData.autoZoom = true; 	
		}
		
		if ( typeof mapData.zoom === "undefined" || mapData.autoZoom ) {
			mapData.zoom = (mapData.type == 'BIG_MAP' || mapData.type == 'DASH_MAP') 
				? this.bigMapDefZoom : this.littleMapDefZoom;
		}	
			
		mapData.map = this.providerMap.createMap( $mapElem, mapData.zoom, mapData.centerLat, mapData.centerLng );
		
		this.initMarkers( mapData );
		// wait a bit for the showCurrentLocation
		setTimeout(function() {
			mapManager.setMapCenteredByMarkers( mapData );
		}, 100);

		if ( typeof onMapLoadHandler === "function" ) {
			this.providerMap.addOnMapLoadHandler( mapData.map, onMapLoadHandler );
		}
		
		this.onMapViewPortChanged( mapId, Runner.trackUserActivity );
	},
	
	/**
	 * @param {string} mapId
	 * @return {boolean}
	 */
	isHeatMap: function( mapData ) {
		return this.providerMap.mapIsHeatmap( mapData );
	},
	
	/**
	 * @param {string} mapId
	 */
	initHeatMap: function( mapId ) {
		this.providerMap.initHeatMap( this.mapsData[ mapId ] );
	},
	
	/**
	 * @param {object} mapData
	 * @return {object}
	 */
	getBoundMarkers: function( mapData ) {
		var markersExists = false,
			markers = mapData.markers, 
			minLat, minLng, maxLat, maxLng, i,
			minLatID, minLngID, maxLatID, maxLngID;
		
		for (i = 0; i < markers.length; i++) {
			if ( markers[i].detached || markers[i].lat == '' ||	markers[i].lng == '' || 
				( this.isHeatMap( mapData ) && mapData.weightField && markers[i].weight <= 0 ) ) {
				continue;
			}
			
			var markerLat = this.parseFloatValue( markers[ i ].lat );
			var markerLng = this.parseFloatValue( markers[ i ].lng );
			
			if ( !markersExists ) {
				markersExists = true;
				minLatID = minLngID = maxLatID = maxLngID = i;

				minLat = markerLat;
				minLng = markerLng;
				maxLat = markerLat;
				maxLng = markerLng;
			} else {
				minLatID = minLat > markerLat ? i : minLatID; 
				minLngID = minLng > markerLng ? i : minLngID;
				maxLatID = maxLat < markerLat ? i : maxLatID;
				maxLngID = maxLng < markerLng ? i : maxLngID;			
			
				minLat = minLat > markerLat ? markerLat : minLat; 
				minLng = minLng > markerLng ? markers[ i ].lng : minLng;
				maxLat = maxLat < markerLat ? markerLat : maxLat;
				maxLng = maxLng < markerLng ? markers[ i ].lng : maxLng;			
			}
		}
		
		if ( !markersExists ) {
			return null;
		}
		
		return {
			minLatID: minLatID, 
			minLngID: minLngID, 
			maxLatID: maxLatID, 
			maxLngID: maxLngID,		
			minLat: minLat,
			minLng: minLng,
			maxLat: maxLat,
			maxLng: maxLng,			
		};
	},
	
	/**
	 * @param {object} mapData
	 */
	initCenter: function( mapData ) {
		var cenLat = 0, cenLng = 0,
			markersCount = 0,
			markers = mapData.markers || [],
			boundLatLng;
		
		if ( !mapData.map || !markers.length ) {
			return;
		}
		
		if ( !mapData.zoom && mapData.zoom !== 0 ) {
			mapData.zoom = (mapData.type == 'BIG_MAP' || mapData.type == 'DASH_MAP') ? this.bigMapDefZoom : this.littleMapDefZoom;
		}
		
		boundLatLng = this.getBoundMarkers( mapData );
		
		if ( boundLatLng ) {
			cenLat = this.parseFloatValue( boundLatLng.minLat ) + this.parseFloatValue( boundLatLng.maxLat );
			cenLng = this.parseFloatValue( boundLatLng.minLng ) + this.parseFloatValue( boundLatLng.maxLng );
			markersCount = 2;
		}
			
		if ( markersCount === 0 ) {
			if ( mapData.type == 'FIELD_MAP' && markers[0].address ) {
				this.providerMap.setCenter( markers[0].address, mapData.map, mapData.zoom );
			}
			markersCount = 1;
		} 

		this.providerMap.setMapCenter( mapData.map, mapData.zoom, cenLat / markersCount, cenLng / markersCount );
	},
	
	/**
	 * Set the map's zoom level automaticaly basing on its markers' coordinates
	 * @param {object} mapData
	 */
	setZoomAuto: function( mapData ) { 
		if ( !mapData.markers || !mapData.map ) {
			return false;
		}		
		
		this.providerMap.setZoomAuto( mapData.map, mapData.markers, this.getBoundMarkers( mapData ) );
		
		if ( mapData.map.getZoom() > 18 ) {
			// Zoom level between House and Street view
			this.providerMap.setZoom( mapData.map, 18 );
		}		
	},

	/**
	 * Set the new center coordinates for a map with a particular id
	 * (The users function #2145)
	 * @param {string} mapId
	 * @param {number} lat
	 * @param {number} lng
	 */
	setCenter: function( mapId, lat, lng ) {
		var mapData = this.mapsData[ mapId ];
		
		if ( !mapData || !mapData.map ) {
			return false;
		}

		if ( mapData.zoom === undefined ) {
			mapData.zoom = (mapData.type == 'BIG_MAP' || mapData.type == 'DASH_MAP') ? this.bigMapDefZoom : this.littleMapDefZoom;
		}
		this.providerMap.setMapCenter( mapData.map, mapData.zoom, lat, lng );
	},

	/**
	 * Set the map's centre according to all its markers coordinates.
	 * Set a zoom level so all map's marker could be visible if auto-zoom is turned on
	 * @param {object} mapData
	 */
	setMapCenteredByMarkers: function( mapData ) {
		if ( mapData.autoZoom ) {
			this.setZoomAuto( mapData );	
		} else {
			this.initCenter( mapData );
		}		
	},
	
	/**
	 * Add to the 'center' links a 'click' handler 
	 */
	initCenterLinks: function() {
		this.initCenterLink( $("[type='centerOnMarker" + this.id + "']") );
	},
	
	/**
	 * Add to the link the 'click' event handler 
	 * that center the map
	 * @param {jQuery object} link
	 */
	initCenterLink: function( $link ) {
		var mapManager = this;
		
		$link.on('click', function(e) {
			mapManager.centerOnMarker( $(this).attr('recId') );
			return false;
		});
	},

	/**
	 * Get the link that center the map 'on click'
	 * @param {number} id
	 * @param {string} val
	 * @param {string} type
	 * @return {jQuery object}
	 * Note: add not HTML5 valid markup
	 */ 
	getCenterLink: function( id, val, type ) {
		var $link = $('<a href="#" recId="' + id + '" type="centerOnMarker' + this.id + '"></a>');

		this.initCenterLink( $link );
		
		if ( type == 2 ) {
			return $link.html( val );
		}
		
		if ( type == 1 ) {
			return $link.html( this.centerLinkText );
		}

		return $link;
	},

	/**
	 * Request current position
	 * @param {function} successHn
	 */
	tryGetCurrentPosition: function( successHn ) {
		if (navigator.geolocation) {
			navigator.geolocation.getCurrentPosition(function( position ) {
				successHn( position.coords );
				//first call is an Geolocation API call, following calls - cached from closure
				Runner.controls.MapManager.prototype.tryGetCurrentPosition = function( handler ) {
					handler( position.coords );
				};
			});
		}
	},
	/**
	 * Add current location to map object,
	 * add marker for location
	 * @param {function} successHn
	 */
	addCurrentLocation: function( mapData ) {
		var mapManager = this;
		this.tryGetCurrentPosition(function(coords) {
			var currLocCoords = {
				address: "",
				desc: 'My location',
				lat: null,
				lng: null,
				currentLocationMarker: true,
				keys: [],
				masterKeys: [],
				mapIcon: mapData.currentLocationIcon,
			};
			currLocCoords.lat = coords.latitude;
			currLocCoords.lng = coords.longitude;
			mapManager.initMarker( mapData, currLocCoords );
			mapData.markers.push( currLocCoords );
		});
	},

	/**
	 * Initialize the map's markers
	 * @param {object} mapData
	 */
	initMarkers: function( mapData ) {		
		if ( !mapData.markers ) {
			mapData.markers = [];
		}
		for ( var i = 0; i < ( mapData.markers ).length; i++ ) {
			this.initMarker( mapData, mapData.markers[ i ] );
		}

		if ( mapData.showCurrentLocation && !this.isHeatMap( mapData ) ) {
			this.addCurrentLocation( mapData );
		}

		if ( mapData.clustering ) {
			this.markerClusterer = this.providerMap.initializeClusterMarkers( mapData );
		}
	},

	
	/**
	 * Initialize the map's marker
	 * @param {object} mapData
	 * @param {object} markerData
	 */
	initMarker: function( mapData, markerData ) {		
		if ( markerData.lat !== '' && markerData.lng !== '' ) {
			this.addMarkerByCoords( markerData, mapData );
			return;
		} 

		if ( markerData.address ) {
			this.addMarkerByAddress( markerData, mapData );
		}
	},

	/**
	 * A wrapper
	 * @param {object} marker
	 * @param {function} handler
	 */
	addOnMarkerClickHandler: function( marker, handler ) {
		this.providerMap.addOnMarkerClickHandler( marker, handler );
	},

	/**
	 * A wrapper
	 * @param {object} marker
	 */	 
	triggerMarkerClickEvent: function( marker ) {
		marker && this.providerMap.triggerMarkerClickEvent( marker );
	},
	
	/**
	 * Add to the map's marker a 'click' event handler
	 * @param {object} markerData
	 * @param {boolean} markerAsEditLink
	 */
	addMarkerClickEvent: function( marker, markerData, markerAsEditLink ) { 
		var mapManager = this;
			
		if ( !markerData.link ) { 
			return;
		}

		if ( this.providerMap.isClusterHandlerToAdd( markerData ) ) {			
			this.providerMap.addClusterClickHandler && 
				this.providerMap.addClusterClickHandler( markerData, this.markerClusterer );

			return;
		}
		
		if ( markerAsEditLink && this.showEditInPopup ) {
			//clicking on the maker opens the edit popup page	
			this.providerMap.addOnMarkerClickHandler( marker, function() {				
				$("#editLink" + markerData.recId).trigger("click");
			});
			
			return;
		}
		
		if ( this.showViewInPopup ) {
			//clicking on the maker opens the view page	
			this.providerMap.addOnMarkerClickHandler( marker, function() { 
				var hostPage = Runner.pages.PageManager.getAt( mapManager.tName, mapManager.id ),
					pageParams = Runner.pages.ViewPage.prototype.getOpenPageParams.call( hostPage, markerData.keys, 
						markerData.recId, hostPage.getInlineEditObj(), Runner.emptyFn );
				
				Runner.pages.PageManager.openPage( pageParams );
			});
			return;
		}
		
		this.providerMap.addOnMarkerClickHandler( marker, function() { 
			window.location.href = markerData.link;
		});
	},
	
	/**
	 * @param {string} mapId
	 * @return {object}
	 */
	getActiveMarkerData: function( mapId ) {
		var idx = this.getActiveMarkerIndex( mapId );	
		return idx === -1 ? null : this.mapsData[ mapId ].markers[ idx ];	
	},
	
	/**
	 * @param {boolean} goForward
	 * @param {string} mapId
	 * @return {number}
	 */
	getPrevNextActiveMarkerData: function( goForward, mapId ) {
		var mapData = this.mapsData[ mapId ],
			idx = this.getActiveMarkerIndex( mapId ),
			i;
		
		if ( idx === -1 ) {
			return null;
		}
		
		if ( goForward ) {
			for (i = idx + 1; i < mapData.markers.length; i++) {
				if ( !mapData.markers[i].detached && this.testMarkerInViewport( mapId, mapData.markers[i] ) ) {
					return mapData.markers[i];
				}
			}
		} else {
			for (i = idx; i--;) {
				if ( !mapData.markers[i].detached && this.testMarkerInViewport( mapId, mapData.markers[i] ) ) {
					return mapData.markers[i];
				}
			}
		}
		
		return null;
	},
	
	/**
	 * @param {string} mapId
	 * @return {number}
	 */
	getActiveMarkerIndex: function( mapId ) {
		var mapData = this.mapsData[ mapId ], i;
		
		for ( i = 0; i < ( mapData.markers || [] ).length; i++ ) {
			if ( mapData.markers[ i ].active ) {
				return i;
			}
		}
		
		return -1;	
	},
	
	/**
	 * @param {object} markerData
	 * @param {string} mapId
	 * @param {array} keys
	 */
	setMarkerActive: function( markerData, mapId, keys ) {
		var mapData = this.mapsData[ mapId ];

		markerData = markerData || this.getMarkerDataByKeys( mapId, keys );
		if ( markerData && markerData.active ) {
			return;
		}
		
		this.setAllMarkerInactive( mapId );

		if ( !markerData ) {
			return;
		}
		
		markerData.active = true;
		this.providerMap.setMarkerActive( markerData.marker, markerData.mapIcon );
	},
	
	/**
	 * @param {string} mapId
	 */
	setAllMarkerInactive: function( mapId ) {
		var activeMarkerData = this.getActiveMarkerData( mapId );
		
		if ( activeMarkerData ) {
			activeMarkerData.active = false;
			this.providerMap.setMarkerInactive( activeMarkerData.marker, activeMarkerData.mapIcon );		
		}	
	},
	
	/**
	 * @param {string} mapId
	 * @param {array} keys
	 * @param {boolean} useDetached
	 * @return {object}
	 */
	getMarkerDataByKeys: function( mapId, keys, useDetached ) {
		var mapData = this.mapsData[ mapId ], i;
		
		if ( !keys ) {
			return null;
		}
		
		for ( var i = 0; i < ( mapData.markers || [] ).length; i++ ) {
			if ( Runner.arraysEqual( keys, mapData.markers[ i ].keys, true ) && ( useDetached || !mapData.markers[ i ].detached ) ) {
				return mapData.markers[ i ];
			}
		}

		return null;	
	},	
	
	/**
	 * Add the marker to the map by its coordinates
	 * @param {object} markerData
	 * @param {object} mapData
	 */
	addMarkerByCoords: function( markerData, mapData ) {			
		if ( !mapData.map ) {
			return;
		}
		
		this.providerMap.setMarkerByCoords( markerData, mapData );
		if ( !this.isHeatMap( mapData ) && !markerData.currentLocationMarker ) {
			this.addMarkerClickEvent( markerData.marker, markerData, mapData.markerAsEditLink );
		}
	},
	
	/**
	 * Add the marker to the map using an addres string
	 * @param {object} markerData
	 * @param {object} mapData
	 */
	addMarkerByAddress: function( markerData, mapData ) {		
		if ( !mapData.map ) {
			return false;
		}
		
		var mapManager = this;
			
		this.providerMap.setMarkerByAddress( markerData, mapData, function() {
			if ( !mapManager.isHeatMap( mapData ) && !markerData.currentLocationMarker ) {
				mapManager.addMarkerClickEvent( markerData.marker, markerData, mapData.markerAsEditLink ); 
			}
			mapManager.setMapCenteredByMarkers( mapData );			
		});
	},

	/**
	 * Update the page's maps 
	 * @param {number} recId
	 * @param {object} recordData
	 * @param {array} oldKeys
	 * @param {string} iconData
	 * @param {object} fieldIconsData 
	 */
	updateAfterEdit: function( recId, recordData, oldKeys, iconData, fieldIconsData ) {
		var mapId, markers, i, fieldsData, mapData;
		
		for ( mapId in this.mapsData ) {
			mapData = this.mapsData[ mapId ];
			markers = mapData.markers;
			
			if ( mapData.skipped || !markers ) {
				continue;
			}
			
			for (i = 0; i < markers.length; i++) {
				if ( oldKeys && !Runner.arraysEqual( oldKeys, markers[i].keys, true ) || !oldKeys && markers[i].recId !== recId ) {
					continue;
				}
					
				if ( mapData.type == 'BIG_MAP' || mapData.type == 'DASH_MAP' ) {
					fieldsData = mapData;
				} else {
					fieldsData = this.fieldsAsMap[ mapData.fName ];
					iconData = fieldIconsData && fieldIconsData[ mapData.fName ] || "";
				}				
					
				markers[i].lat = recordData[ fieldsData.latField ] || '';
				markers[i].lng = recordData[ fieldsData.lngField ] || '';
				markers[i].desc = recordData[ fieldsData.descField ] || '';
				markers[i].address = recordData[ fieldsData.addressField ] || '';	
				markers[i].weight = recordData[ fieldsData.weightField ] || '';						
				markers[i].link = this.getMarkerLink( recordData.keys, mapData.markerAsEditLink );
				markers[i].keys = recordData.keys; 
				markers[i].mapIcon = iconData || mapData.markerField && recordData[ mapData.markerField ] || mapData.markerIcon || '';
			}
			
			if ( mapData.clustering ) {
				this.mapsData[ mapId ].clusterMarkers = [];
				this.mapsData[ mapId ].addedCoordinates = [];
			}
		}
		
		this.refreshMaps( recId, recordData.keys );
	},

	/**
	 * Update a particular marker
	 * @param {number} mapId
	 * @param {number} ind
	 */ 
	refreshMarker: function( mapId, ind ) {
		var mapData = this.mapsData[ mapId ],
			markerData = this.mapsData[ mapId ].markers[ ind ]; 
		
		if ( !mapData.map ) {
			return;
		}
		
		this.destroyMarker( mapData.map, markerData.marker );
		
		this.initMarker( mapData, mapData.markers[ ind ] );
		
		if ( mapData.dashMap && markerData.active ) {
			markerData.active = false;
			this.setMarkerActive( markerData, mapId );
		}
	},
	
	/**
	 * Wrapper
	 * @param {object} map
	 * @param {object} marker
	 */
	destroyMarker: function( map, marker ) {
		if ( this.markerClusterer ) {
			this.providerMap.destroyClusterMarker 
				&& this.providerMap.destroyClusterMarker( this.markerClusterer, marker );
		}
		
		this.providerMap.destroyMarker( map, marker );
		delete marker;
	},
	
	/**
	 * @param {string} mapId
	 * @return {boolean}
	 */
	newMarkerWasAdded: function( mapId ) {
		return this.mapsData[ mapId ] && this.mapsData[ mapId ].newMarkerWasAdded;
	},
	
	/**
	 * Update the page's maps data
	 * @param {number} recId
	 * @param {object} recordData
	 * @param {string} iconData
	 * @param {object} fieldIconsData
	 */
	updateAfterAdd: function( recId, recordData, iconData, fieldIconsData ) {
		var mapId, mapData, markerData, fName;
		
		for ( mapId in this.mapsData ) {
			mapData = this.mapsData[ mapId ];
			
			if ( mapData.type !== "BIG_MAP" && mapData.type !== "DASH_MAP" || mapData.skipped ) {
				continue;
			}
			
			// recreate marker, for avoidance memory mixing
			markerData = {
				recId: recId,
				lat: recordData[ mapData.latField ] || '',
				lng: recordData[ mapData.lngField ] || '',
				desc: recordData[ mapData.descField ] || '',
				address: recordData[ mapData.addressField ] || '',
				weight: recordData[ mapData.weightField ] || '',
				link: this.getMarkerLink( recordData.keys, mapData.markerAsEditLink ),
				keys: recordData.keys,
				mapIcon: iconData || mapData.markerField && recordData[ mapData.markerField ] || mapData.mapIcon || mapData.markerIcon
			};			
					
			mapData.markers = mapData.markers || [];
			mapData.markers.unshift( markerData );
			
			if ( mapData.clustering || this.isHeatMap( mapData ) ) {
				this.mapsData[ mapId ].clusterMarkers = [];
				this.mapsData[ mapId ].addedCoordinates = [];
				
				for ( var i = 0; i < mapData.markers.length; i++ ) {
					this.providerMap.destroyMarker( mapData.map, mapData.markers[i].marker );		
				}
				
				this.initMap( mapId );
				if ( this.isHeatMap( mapData ) ) {
					this.initHeatMap( mapId );
				}
			} else {
				this.initMarker( mapData, markerData );
			}
			
			if ( mapData.dashMap && !this.isHeatMap( mapData ) ) {
				this.setMarkerActive( markerData, mapId );
				if ( !this.mapBoundsContains( markerData, mapData.map ) ) {
					mapData.newMarkerWasAdded = true;
					this.setMapCenteredByMarkers( mapData );
					
					setTimeout( function() {
						delete mapData.newMarkerWasAdded;
					}, 1000 );
				}
			}
		}
		
		for ( fName in this.fieldsAsMap ) {
			mapId = this.getMapDivId("FIELD_MAP", recId, fName);
			
			this.mapsData[ mapId ] = {
				markers: [	{
					recId: recId,
					lat: recordData[ this.fieldsAsMap[ fName ].latField ] || '',
					lng: recordData[ this.fieldsAsMap[ fName ].lngField ] || '',
					desc: recordData[ this.fieldsAsMap[ fName ].descField ] || '',
					address: recordData[ this.fieldsAsMap[ fName ].addressField ] || '',
					link: this.getMarkerLink( recordData.keys ),
					keys: recordData.keys,
					mapIcon: fieldIconsData && fieldIconsData[fName] || this.fieldsAsMap[ fName ].mapIcon 
				} ],
				skipped: this.useEmbedMapsAPI,
				fName: fName, 
				map: null, 
				mapFieldValue: recordData[ fName ], 
				type: 'FIELD_MAP', 
				zoom: this.fieldsAsMap[ fName ].zoom
			};
		}
	},
	
	/**
	 * A wrapper
	 * @param {object} markerData
	 * @param {object} map
	 * @return {boolean}
	 */
	mapBoundsContains: function( markerData, map ) {
		return markerData.marker && map && this.providerMap.mapBoundsContains( markerData.marker, map );
	},
	
	/**
	 * Forms the URL basing on keys values and type of the page that opens by click ob a marker
	 * @param {array} keys
	 * @param {boolean} markerAsEditLink		Sets in the Big map's settings
	 * @return {string}
	 */
	getMarkerLink: function( keys, markerAsEditLink ) {
		var state = this.hostPage && this.hostPage.getStateParams();
		
		if ( markerAsEditLink && Runner.pages.PageSettings.getTableData(this.tName, "editAvailable") ) {
			return Runner.pages.getUrl( this.tName, Runner.pages.constants.PAGE_EDIT, keys, "editid", state );
		} else if ( Runner.pages.PageSettings.getTableData(this.tName, "viewAvailable") ) {
			return Runner.pages.getUrl( this.tName, Runner.pages.constants.PAGE_VIEW, keys, "editid", state );
		}
	},
	
	/**
	 * Update the map's markers and center the map
	 * @param {number} recId
	 * @param {array} keys
	 * @param {boolean} skipBigMap
	 */
	refreshMaps: function( recId, keys, skipBigMap ) {
		var mapId, mapData, markers, i;
		
		for ( mapId in this.mapsData ) {			
			mapData = this.mapsData[ mapId ];
			markers = mapData.markers;
			
			if ( skipBigMap && mapData.type == "BIG_MAP" ) {
				continue;
			}
			
			if ( !markers || mapData.skipped ) { 
				continue;
			}
			
			if ( mapData.clustering || this.isHeatMap( mapData ) ) {
				this.mapsData[ mapId ].clusterMarkers = [];
				this.mapsData[ mapId ].addedCoordinates = [];
				
				for ( var i = 0; i < mapData.markers.length; i++ ) {
					this.providerMap.destroyMarker( mapData.map, mapData.markers[i].marker );		
				}				
				
				this.initMap( mapId );
				
				if ( this.isHeatMap( mapData ) ) {
					this.initHeatMap( mapId );
				}
				
				continue;
			} 
			
			for (i = 0; i < markers.length; i++) {
				if ( recId === undefined || markers[i].recId == recId || keys && Runner.arraysEqual( keys, markers[i].keys, true ) ) {
					this.refreshMarker( mapId, i );
				}
			}
		
			this.setMapCenteredByMarkers( mapData );
		}
	},

	/**
	 * Checke if the field is set as 'View as map' 
	 * @param {string} fName
	 * @return {boolean}
	 */
	isFieldIsMap: function( fName ) {
		return this.fieldsAsMap[ fName ] !== undefined;
	},

	/**
	 * Get the map's div container id
	 * @param {string} type
	 * @param {number} recId
	 * @param {string} fName
	 * @return {string}
	 */
	getMapDivId: function( type, recId, fName ) {
		if (type == "FIELD_MAP") {
			return "littleMap_" + Runner.goodFieldName( fName ) + '_' + recId;
		}
		return recId;
	},

	/**
	 * Get the map's div container
	 * @param {string} type
	 * @param {number} id
	 * @param {string} fName 
	 * @return {jQuery} object
	 */
	getMapDiv: function( type, id, fName ) {
		var mapDivId = this.getMapDivId( type, id, fName ),
			mapDiv = $('<div id="' + mapDivId + '" class=littleMap></div>');

		if ( !this.mapsData[ mapDivId ] ) {
			return mapDiv;
		}		
			
		if ( this.mapsData[ mapDivId ].type !== 'FIELD_MAP' ) {
			return mapDiv.css({
					'width': this.mapsData[ mapDivId ].width,
					'height': this.mapsData[ mapDivId ].height
				});
		}

		return mapDiv.css({
				'width': this.fieldsAsMap[ fName ].width,
				'height': this.fieldsAsMap[ fName ].height
			});
	},

	/**
	 * Center big map by marker on little map.
	 * RecId is unique id or record, same as $this->recId property on server in runnerpage class
	 * @param {int} recId
	 */
	centerOnMarker: function( recId ) {
		var mapId, markers, i, mapData;
		
		for (mapId in this.mapsData) {
			if ( this.mapsData[ mapId ].type != 'BIG_MAP' && this.mapsData[ mapId ].type != 'DASH_MAP' || this.mapsData[ mapId ].skipped ) {
				continue;
			}
			
			markers = this.mapsData[ mapId ].markers;
			for (i = 0; i < markers.length; i++) {
				if ( markers[i].recId != recId || !this.mapsData[ mapId ].map ) {
					continue;
				}
				if ( this.mapsData[ mapId ].zoom === undefined ) {
					this.mapsData[ mapId ].zoom = this.bigMapDefZoom;
				}
				
				if ( markers[i].lat != '' && markers[i].lng != '' ) {
					this.providerMap.setMapCenter( this.mapsData[ mapId ].map, this.mapsData[ mapId ].zoom, 
						this.parseFloatValue( markers[i].lat ), this.parseFloatValue( markers[i].lng ) );
						
					break;
				} 
				
				if ( markers[i].address ) {
					mapData = this.mapsData[ mapId ];
					this.providerMap.setCenter( markers[i].address, mapData );
					break;
				}
			}
		}
	},

	/**
	 * @param {mixed} fName
	 */ 
	isFieldCenterLink: function( fName ) {
		var fieldsAsCenterLink = [],
			linkType = false,
			mapId, i;
		
		for (mapId in this.mapsData) {
			if ( this.mapsData[ mapId ].type != 'BIG_MAP' && this.mapsData[ mapId ].type != 'DASH_MAP' || !this.mapsData[mapId].showCenterLink || this.mapsData[ mapId ].skipped ) {
				continue;
			}
			fieldsAsCenterLink.push({
				addressField: this.mapsData[ mapId ].addressField, 
				linkType: this.mapsData[ mapId ].showCenterLink
			});
			if ( this.mapsData[ mapId ].addressField == fName ) {
				linkType = this.mapsData[mapId].showCenterLink;
			}
		}
		
		this.isFieldCenterLink = function( fName ) {
			for (i = 0; i < fieldsAsCenterLink.length; i++) {
				if ( fieldsAsCenterLink[i].addressField == fName ) {
					return fieldsAsCenterLink[i].linkType;
				}
			}
			return false;
		}

		return linkType;
	},
	
	/**
	 * Get the map object by its name.
	 * This method allows to get the map object trough the Java Script onLoad event #6924
	 * @param {string} mapName 
	 * @return {mixed} 
	 */
	getBigMap: function( mapName ) {
		var mapId;
		
		if ( typeof mapName !== "undefined" && this.mapsData[ mapName ] ) {
			return this.mapsData[ mapName ].map;
		}
		
		//getBigMap tries to return a big map
		if ( typeof mapName === "undefined" ) {
			for ( mapId in this.mapsData ) {
				if ( this.mapsData[ mapId ].type === "BIG_MAP" || this.mapsData[ mapId ].type === "DASH_MAP" ) {
					return this.mapsData[ mapId ].map;
				}
			}
		}			
	},

	/**
	 * Init the 'View as Map' field's map after Inline Add/Edit,
	 * update center links
	 * @param {string} fName	
	 * @param {number} recId	
	 * @param {string} caption	
	 * @param {boolean} refresh	
	 * @param {boolean} skipBigMap	
	 */
	initInlineMapOrCenterLink: function( fName, recId, caption, refresh, skipBigMap ) {
		var $span, $mapDiv, $mapCenterLink;
		
		if ( this.isFieldIsMap( fName ) ) {
			var mapId = this.getMapDivId("FIELD_MAP", recId, fName);
			if ( this.mapsData[ mapId ].skipped ) {
				return;
			}
			
			$span = Runner.getFieldSpan( fName, recId ).html('');
			$mapDiv = this.getMapDiv( "FIELD_MAP", recId, fName )
				.appendTo( $span );
		
			this.initMap( $mapDiv.attr("id") );
			
			if ( refresh ) {
				this.refreshMaps( recId, null, skipBigMap );
			}
			return;	
		}
		
		if ( this.isFieldCenterLink( fName ) ) {
			$span = Runner.getFieldSpan( fName, recId ).html('');
			$mapCenterLink = this.getCenterLink( recId, caption, this.isFieldCenterLink( fName ) )
				.appendTo( $span );
		}
	},

	/**
	 * Fire the 'resize' event for the exiting fields maps.
	 * Center these maps according to maps settings
	 */
	updateLittleMaps: function() {
		var mapData, i;
		
		for (i = 0; i < this.fieldMapsIds.length; i++) {
			mapData = this.mapsData[ this.fieldMapsIds[ i ] ];
			
			this.providerMap.triggerResizeEvent( mapData.map );
			this.setMapCenteredByMarkers( mapData );
		}
	},

	/**
	 *
	 */
	updateMapsOnEditCancel: function( row, fieldNames ) {
		for (var i = 0; i < fieldNames.length; i++) {
			if ( this.isFieldIsMap( fieldNames[i] ) ) {
				this.initMap( this.getMapDivId( "FIELD_MAP", row.id, fieldNames[i] ) );
			}
		}
	},
	
	/**
	 *
	 */
	updateMapsOnAfterEditSubmit: function( vals, fields, keys, recId, data ) {
		var recordVals = Runner.apply( { keys: keys }, data.rawVals ), i;
	
		for (i = 0; i < fields.length; i++) {
			this.initInlineMapOrCenterLink( fields[i], recId, vals[ fields[i] ] );
		}
		this.updateAfterEdit( recId, recordVals, null, "", data.fieldsMapIconsData );					
	},	
	
	/**
	 *
	 */
	updateMapsOnAfterAddSubmit: function( vals, fields, keys, recId, data ) {
		var recordVals = Runner.apply( { keys: keys }, data.rawVals ), 
			i;
		 
		this.updateAfterAdd( recId, recordVals, "", data.fieldsMapIconsData );
		
		for (i = 0; i < fields.length; i++) {
			this.initInlineMapOrCenterLink( fields[i], recId, vals[ fields[i] ], true, true );
		}
	},
	
	/**
	 * @param {object} mapsData
	 */
	reInitMapsData: function( mapsData ) {
		this.mapsData = mapsData;
	},
	
	/**
	 * @param {string} mapId
	 * @param {array} keyStrings
	 */	
	deleteSelectedMarkers: function( mapId, keyStrings ) {
		var mapData = this.mapsData[ mapId ], 
			markersData = [], i;
		
		if ( !mapData.map || !mapData.markers || !mapData.markers.length ) {
			return;
		}
		
		for ( i = 0; i < mapData.markers.length; i++ ) {
			if ( $.inArray( mapData.markers[ i ].keys.join("&"), keyStrings ) !== -1 ) { // fix it with Runner.arraysEqual
				this.destroyMarker( mapData.map, mapData.markers[i].marker );
			} else {
				markersData.push( mapData.markers[i] );
			}
		}
		
		mapData.markers = markersData;
	},

	/**
	 * @param {string} mapId
	 * @param {array} newMarkersDatad
	 * @param {boolean} updateCenter
	 * @param {boolean} notDestroyOldMarkers
	 */
	reinitMapMarkers: function( mapId, newMarkersData, updateCenter, notDestroyOldMarkers ) {
		var mapData = this.mapsData[ mapId ],
			activeMarkerData,
			mapManager = this;
		
		if ( !mapData.map ) {
			return;
		}
		
		newMarkersData = newMarkersData || [];
		
		if ( mapData.dashMap ) {
			activeMarkerData = this.getActiveMarkerData( mapId );
		}

		this.updateMapMarkersData( mapId, newMarkersData, notDestroyOldMarkers );	

		if ( this.isHeatMap( mapData ) ) {
			this.initHeatMap( mapId );
		}		
		
		if ( mapData.dashMap && !this.isHeatMap( mapData ) ) {
			activeMarkerData = activeMarkerData && this.getMarkerDataByKeys( mapId, activeMarkerData.keys ) || this.getFirtMarkerData( mapId );
			this.setMarkerActive( activeMarkerData, mapId );
		}		

		if ( updateCenter && !notDestroyOldMarkers ) {
			// wait a bit
			setTimeout(function() {
				mapManager.setMapCenteredByMarkers( mapData );
			}, 100);
		}			
	},

	/**
	 * @param {string} mapId
	 * @return {object}
	 */
	getFirtMarkerData: function( mapId ) {
		var mapData = this.mapsData[ mapId ], i;
		
		if ( !mapData.dashMap ) {
			return mapData.markers[0];
		}
		
		for ( var i = 0; i < mapData.markers.length; i++ ) {
			if ( !mapData.markers[i].detached ) {
				return mapData.markers[i]; 
			}
		}
		
		return null;
	},
	
	/**
	 * @param {string} mapId
	 * @param {array} newMarkersData
	 * @param {boolean} notDestroyOldMarkers
	 */
	updateMapMarkersData: function( mapId, newMarkersData, notDestroyOldMarkers ) {
		var mapData = this.mapsData[ mapId ],
			markersToInit = [], 
			markersData = [], 
			i;
		
		if ( !notDestroyOldMarkers ) {
			this.destroyMapMarkers( mapId );
			mapData.markers = newMarkersData;
			
			this.initMarkers( mapData );
			return;	
		}
		
		if ( !mapData.dashMap ) {
			return;
		}
		
		this.setAllMarkerInactive( mapId );
	
		mapData.markers = mapData.markers || [];
	
		for ( i = 0; i < newMarkersData.length; i++ ) {
			if ( !this.checkIfMarkerDataInByKeys( newMarkersData[i].keys, mapData.markers ) ) {
				markersToInit.push( newMarkersData[i] );
			} else {
				markersData.push( this.getMarkerDataByKeys( mapId, newMarkersData[i].keys, true ) );
			}
		}	

		for ( i = 0; i < markersToInit.length; i++ ) {
			this.initMarker( mapData, markersToInit[ i ] );
		}
		
		markersData = markersData.concat( markersToInit );
		
		for ( i = 0; i < mapData.markers.length; i++ ) {
			mapData.markers[i].detached = !this.checkIfMarkerDataInByKeys( mapData.markers[i].keys, newMarkersData );
			
			if ( mapData.markers[i].detached ) {
				markersData.push( mapData.markers[i] );
			}
		};

		mapData.markers = markersData;		
	},
	
	/**
	 * @param {string} mapId	
	 * @return {boolean}
	 */
	hasMarkers: function( mapId ) {
		var mapData = this.mapsData[ mapId ],
			i;
		
		for ( i = 0; i < mapData.markers.length; i++ ) {
			if ( !mapData.markers[i].detached ) {
				return true;
			}
		}
		
		return false;
	},
	
	/**
	 * @param {array} keys
	 * @param {array} markersData
	 * @return {boolean}
	 */
	checkIfMarkerDataInByKeys: function( keys, markersData ) {
		for ( var i = 0; i < markersData.length; i ++) {
			if ( Runner.arraysEqual( keys, markersData[i].keys, true ) ) {
				return true;
			}
		}
		
		return false;
	},
	
	/**
	 *
	 */	
	getMarker: function( mapId, keys ) {		
		var markerData = this.getMarkerDataByKeys( mapId, keys );
		
		return markerData ? markerData.marker : null;
	},
	
	/**
	 * @param {string} mapId
	 */
	destroyMapMarkers: function( mapId ) {
		var mapData = this.mapsData[ mapId ], i;
		
		if ( !mapData.markers ) {
			return;
		}
		
		if ( mapData.clustering ) {
			mapData.clusterMarkers = [];
			mapData.addedCoordinates = [];
		}
		
		for ( i = 0; i < mapData.markers.length; i++ ) {
			this.destroyMarker( mapData.map, mapData.markers[i].marker );		
		}
		
		mapData.markers = []; 
	},
	
	/**
	 * @param {string} mapId
	 * @return {object}
	 */
	getMapViewPortCoordinates: function( mapId ) {
		var mapData = this.mapsData[ mapId ];
		
		return this.providerMap.getMapViewPortCoordinates( mapData.map );	
	},
	
	testMarkerInViewport: function( mapId, marker ) {
		var viewport = this.getMapViewPortCoordinates( mapId ),
			lat = parseFloat( marker.lat ),
			lng = parseFloat( marker.lng );
			
		return lat >= viewport.s && lat <= viewport.n 
			&& Math.min( viewport.w, viewport.e ) <= lng && Math.max( viewport.w, viewport.e ) >= lng;
	},
	
	
	/**
	 * @param {string} mapId
	 * @param {function} handler
	 */
	onMapViewPortChanged: function( mapId, handler ) {		
		this.providerMap.addOnMapViewPortChangedHandler( this.mapsData[ mapId ].map, handler );
	},
	
	parseFloatValue: function( value ) {
		return typeof( value ) == 'string' ? parseFloat( value.replace(',', '.') ) : value;
	}
});

/**
 * Global GmapLoader
 * @constructor
 */
Runner.GmapLoader = function() {
	var stack = [],
		stackContext = [],
		isLoaded = false;
	
	/**
	 * Load Google maps API
	 * @param {string} APIkey
	 */
	this.loadGMapScript = function( APIkey ) {			
		var script = document.createElement("script"),
			lang = Runner.getGoogleLanguage(),
			langStr = lang? "&language=" + lang : "",
			sensor = "false";

		if ( APIkey != '' ) {
			script.src = location.protocol + "//maps.googleapis.com/maps/api/js?key=" + APIkey + "&sensor=" + sensor + langStr + "&callback=Runner.globalGmapLoader.mapsLoaded&libraries=visualization,geometry";
		} else {
			script.src = location.protocol + "//maps.googleapis.com/maps/api/js?sensor=" + sensor + langStr + "&callback=Runner.globalGmapLoader.mapsLoaded&libraries=visualization,geometry";
		}
			
		script.type = "text/javascript";
		document.getElementsByTagName("head")[0].appendChild( script );
	}
		
	this.onLoad = function( f, context ) {
		stack.push( f );
		stackContext.push( context );
		if ( this.isLoaded ) {
			this.fireLoad();
		}
	}

	this.fireLoad = function() {
		for (var i = 0; i < stack.length; i++) {
			stack[i].apply( stackContext[i] || this );
		}
		stack = [];
		stackContext = [];
	}

	this.mapsLoaded = function() {
		this.isLoaded = true;
		this.fireLoad();
	}
};

/**
 * Load Maps API and map initializing handlers
 */
(function() {
	Runner.globalGmapLoader = new Runner.GmapLoader();
	
	if ( window.settings['global'].mapProvider === Runner.controls.constants.GOOGLE_MAPS && !window.settings['global'].staticMapsOnly ) {
		Runner.globalGmapLoader.loadGMapScript( window.settings['global'].mapsApiCode );
		return;	
	} 
	
	// onLoad handlers actually haven't been added yet
	Runner.globalGmapLoader.mapsLoaded();
}());