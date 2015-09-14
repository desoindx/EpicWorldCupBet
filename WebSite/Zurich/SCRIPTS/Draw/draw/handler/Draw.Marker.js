L.Map.TouchExtend = L.Handler.extend({

    initialize: function (map) {
        this._map = map;
        this._container = map._container;
        this._pane = map._panes.overlayPane;
    },

    addHooks: function () {
        L.DomEvent.on(this._container, 'touchstart', this._onTouchStart, this);
        L.DomEvent.on(this._container, 'touchend', this._onTouchEnd, this);
        L.DomEvent.on(this._container, 'touchcancel', this._onTouchCancel, this);
        L.DomEvent.on(this._container, 'touchleave', this._onTouchLeave, this);
        L.DomEvent.on(this._container, 'touchmove', this._onTouchMove, this);
    },

    removeHooks: function () {
        L.DomEvent.off(this._container, 'touchstart', this._onTouchStart);
        L.DomEvent.off(this._container, 'touchend', this._onTouchEnd);
        L.DomEvent.off(this._container, 'touchcancel', this._onTouchCancel);
        L.DomEvent.off(this._container, 'touchleave', this._onTouchLeave);
        L.DomEvent.off(this._container, 'touchmove', this._onTouchMove);
    },

    _touchEvent: function (e, type) {
        // #TODO: fix the pageX error that is do a bug in Android where a single touch triggers two click events
        // _filterClick is what leaflet uses as a workaround.
        // This is a problem with more things than just android. Another problem is touchEnd has no touches in
        // its touch list.
        if (!e.touches.length) { return; }
        var containerPoint = this._map.mouseEventToContainerPoint(e.touches[0]),
            layerPoint = this._map.mouseEventToLayerPoint(e.touches[0]),
            latlng = this._map.layerPointToLatLng(layerPoint);

        this._map.fire(type, {
            latlng: latlng,
            layerPoint: layerPoint,
            containerPoint: containerPoint,
            pageX: e.touches[0].pageX,
            pageY: e.touches[0].pageY,
            originalEvent: e
        });
    },

    _onTouchStart: function (e) {
        if (!this._map._loaded) { return; }

        var type = 'touchstart';
        this._touchEvent(e, type);

    },

    _onTouchEnd: function (e) {
        if (!this._map._loaded) { return; }

        var type = 'touchend';
        this._touchEvent(e, type);
    },

    _onTouchCancel: function (e) {
        if (!this._map._loaded) { return; }

        var type = 'touchcancel';
        this._touchEvent(e, type);
    },

    _onTouchLeave: function (e) {
        if (!this._map._loaded) { return; }

        var type = 'touchleave';
        this._touchEvent(e, type);
    },

    _onTouchMove: function (e) {
        if (!this._map._loaded) { return; }

        var type = 'touchmove';
        this._touchEvent(e, type);
    }
});

L.Map.addInitHook('addHandler', 'touchExtend', L.Map.TouchExtend);

// This isn't full Touch support. This is just to get makers to also support dom touch events after creation
// #TODO: find a better way of getting markers to support touch.
L.Marker.Touch = L.Marker.extend({

    // This is an exact copy of https://github.com/Leaflet/Leaflet/blob/v0.7/src/layer/marker/Marker.js
    // with the addition of the touch event son line 15.
    _initInteraction: function () {

        if (!this.options.clickable) { return; }

        // TODO refactor into something shared with Map/Path/etc. to DRY it up

        var icon = this._icon,
            events = ['dblclick', 'mousedown', 'mouseover', 'mouseout', 'contextmenu', 'touchstart', 'touchend', 'touchmove', 'touchcancel'];

        L.DomUtil.addClass(icon, 'leaflet-clickable');
        L.DomEvent.on(icon, 'click', this._onMouseClick, this);
        L.DomEvent.on(icon, 'keypress', this._onKeyPress, this);

        for (var i = 0; i < events.length; i) {
            L.DomEvent.on(icon, events[i], this._fireMouseEvent, this);
        }

        if (L.Handler.MarkerDrag) {
            this.dragging = new L.Handler.MarkerDrag(this);

            if (this.options.draggable) {
                this.dragging.enable();
            }
        }
    }
});


L.Draw.Marker = L.Draw.Feature.extend({
    statics: {
        TYPE: 'marker'
    },

    options: {
        icon: new L.Icon.Default(),
        repeatMode: false,
        zIndexOffset: 2000 // This should be > than the highest z-index any markers
    },

    initialize: function (map, options) {
        // Save the type so super can fire, need to do this as cannot do this.TYPE :(
        this.type = L.Draw.Marker.TYPE;

        L.Draw.Feature.prototype.initialize.call(this, map, options);
    },

    addHooks: function () {
        L.Draw.Feature.prototype.addHooks.call(this);

        if (this._map) {
            this._tooltip.updateContent({ text: L.drawLocal.draw.handlers.marker.tooltip.start });

            // Same mouseMarker as in Draw.Polyline
            if (!this._mouseMarker) {
                this._mouseMarker = L.marker(this._map.getCenter(), {
                    icon: L.divIcon({
                        className: 'leaflet-mouse-marker',
                        iconAnchor: [20, 20],
                        iconSize: [40, 40]
                    }),
                    opacity: 0,
                    zIndexOffset: this.options.zIndexOffset
                });
            }

            this._mouseMarker
				.on('click', this._onClick, this)
				.addTo(this._map);

            this._map.on('mousemove', this._onMouseMove, this);
            this._map.on('click', this._onTouch, this);
        }
    },

    removeHooks: function () {
        L.Draw.Feature.prototype.removeHooks.call(this);

        if (this._map) {
            if (this._marker) {
                this._marker.off('click', this._onClick, this);
                this._map
					.off('click', this._onClick, this)
					.off('click', this._onTouch, this)
					.removeLayer(this._marker);
                delete this._marker;
            }

            this._mouseMarker.off('click', this._onClick, this);
            this._map.removeLayer(this._mouseMarker);
            delete this._mouseMarker;

            this._map.off('mousemove', this._onMouseMove, this);
        }
    },

    _onMouseMove: function (e) {
        var latlng = e.latlng;

        this._tooltip.updatePosition(latlng);
        this._mouseMarker.setLatLng(latlng);

        if (!this._marker) {
            this._marker = new L.Marker(latlng, {
                icon: this.options.icon,
                zIndexOffset: this.options.zIndexOffset
            });
            // Bind to both marker and map to make sure we get the click event.
            this._marker.on('click', this._onClick, this);
            this._map
				.on('click', this._onClick, this)
				.addLayer(this._marker);
        }
        else {
            latlng = this._mouseMarker.getLatLng();
            this._marker.setLatLng(latlng);
        }
    },

    _onClick: function () {
        this._fireCreatedEvent();

        this.disable();
        if (this.options.repeatMode) {
            this.enable();
        }
    },

    _onTouch: function (e) {
        // called on click & tap, only really does any thing on tap
        this._onMouseMove(e); // creates & places marker
        this._onClick(); // permenantly places marker & ends interaction
    },

    _fireCreatedEvent: function () {
        var pos = this._marker.getLatLng();
        $("#Longitude").val(pos.lng);
        $("#Lattitude").val(pos.lat);
        $("#newFountainDiv").bPopup({
            onOpen: function () {
            },
            follow: [false, false],
            speed: 250,
            transition: 'slideIn',
            transitionClose: 'slideBack'
        });
    }
});

L.Draw.Custom = L.Draw.Feature.extend({
    statics: {
        TYPE: 'marker'
    },

    options: {
        icon: new L.Icon.Default(),
        repeatMode: false,
        zIndexOffset: 2000 // This should be > than the highest z-index any markers
    },

    initialize: function (map, options) {
        // Save the type so super can fire, need to do this as cannot do this.TYPE :(
        this.type = L.Draw.Marker.TYPE;
    },

    addHooks: function () {
        navigator.geolocation.getCurrentPosition(createFountainHere);
    },

    removeHooks: function () {
    },

    _onMouseMove: function (e) {
    },

    _onClick: function () {
    },

    _fireCreatedEvent: function () {
    }
});

function createFountainHere(position) {
    $("#Longitude").val(position.coords.longitude);
    $("#Lattitude").val(position.coords.latitude);
    $("#newFountainDiv").bPopup({
        onOpen: function () {
        },
        follow: [false, false],
        speed: 250,
        transition: 'slideIn',
        transitionClose: 'slideBack'
    });
}