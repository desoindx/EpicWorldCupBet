L.DrawToolbar = L.Toolbar.extend({

    statics: {
        TYPE: 'draw'
    },

    options: {
        polyline: {},
        polygon: {},
        rectangle: {},
        circle: {},
        marker: {}
    },

    initialize: function (options) {
        // Ensure that the options are merged correctly since L.extend is only shallow
        for (var type in this.options) {
            if (this.options.hasOwnProperty(type)) {
                if (options[type]) {
                    options[type] = L.extend({}, this.options[type], options[type]);
                }
            }
        }

        this._toolbarClass = 'leaflet-draw-draw';
        L.Toolbar.prototype.initialize.call(this, options);
    },

    getModeHandlers: function (map) {
        return [
			{
			    enabled: this.options.marker,
			    handler: new L.Draw.Marker(map, this.options.marker),
			    title: L.drawLocal.draw.toolbar.buttons.marker
			},
			{
			    enabled: this.options.custom,
			    handler: new L.Draw.Custom(map, this.options.custom),
			    title: L.drawLocal.draw.toolbar.buttons.custom
			}
        ];
    },

    // Get the actions part of the toolbar
    getActions: function (handler) {
        return [
			{
			    title: L.drawLocal.draw.toolbar.actions.title,
			    text: L.drawLocal.draw.toolbar.actions.text,
			    callback: this.disable,
			    context: this
			}
        ];
    },

    setOptions: function (options) {
        L.setOptions(this, options);

        for (var type in this._modes) {
            if (this._modes.hasOwnProperty(type) && options.hasOwnProperty(type)) {
                this._modes[type].handler.setOptions(options[type]);
            }
        }
    }
});