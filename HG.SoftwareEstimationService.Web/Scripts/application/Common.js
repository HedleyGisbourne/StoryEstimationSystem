namespace('common');

common.generateGuid = function () {
    // http://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
};

common.getJqObject = function(element) {
    if (typeof element == 'string') {
        element = $('#' + element);
    };

    return element instanceof jQuery ? element : $(element);
}

common.addCustomButton = function (newButtonId, label, href, appendToElement) {
    var button = $('<button id="' + newButtonId + '" class="btn btn-primary btn-lg">'+ label + '</button>');

    button.click(function () {
        if (typeof href === 'function')
        {
            location.href = href();
        }
        else {
            location.href = href;
        }
        
    });
    common.getJqObject(appendToElement).append(button);
}

common.replaceComboOptions = function(element, options)
{
    var $el = common.getJqObject(element);

    $el.empty(); // remove old options
    $.each(options, function (item) {
        if (!options[item].LogicalDelete)
        {
            var option = $("<option></option>").attr("value", options[item].Id).text(options[item].Name);
            $el.append(option);
        }
    });
}