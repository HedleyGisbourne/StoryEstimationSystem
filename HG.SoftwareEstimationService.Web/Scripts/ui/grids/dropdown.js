namespace('ui.grids');

ui.grids.newOptions = {};

ui.grids.populate = function(uuid, dropdownDictionary) {
    if (dropdownDictionary.length > 0) {

        var args = [];
        Object.keys(dropdownDictionary).forEach(function(key) {
            args.push(key);
        });

        service.callServer($.ajax({
            url: '/api/lookupprovider/GetLookups',
            type: 'POST',
            data: {
                '': args
            }
        })).done(function(newOptions) {
            ui.grids.newOptions = newOptions;
            ui.grids.replaceLookup(uuid, newOptions);
            Object.keys(newOptions).forEach(function(newOption) {
                common.replaceComboOptions(dropdownDictionary[newOption], newOptions[newOption]);

            });
        });
    }
}