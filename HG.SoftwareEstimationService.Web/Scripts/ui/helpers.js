namespace('ui.helpers');

ui.helpers.getLabel = function(name) {
    return $("<label>" + name + ":</label>");
};

ui.helpers.getFormGroupContainer = function()
{
    return $("<div class='form-group' style='margin:0 auto; max-width:260px;'></div>");
}

ui.helpers.getTextInput = function (classes, id, name, defaultValue) {
    return $("<input class='" + classes +
        " form-control' id='" + id +
        "' name='" + name +
        "' type='text'" +
        (defaultValue != null ? "value='" + defaultValue +"'" : "") +
        " />");
}

ui.helpers.getSelectInput = function (id) {
    return $("<select id='" + id +"' class='form-control'></select>");
}

ui.helpers.getValidationSection = function(id)
{
    return $('<div id="' + id + '_validate" class="field-validation-error"></div></div>');
}

ui.helpers.getOptions = function(id, name) {
    return "<option value='" + id + "'>" + name + "</option>";
}

ui.helpers.wholeNumberValidation = function(validationRules, id)
{
    validationRules.rules[id].required = true;
    validationRules.rules[id].digits = true;
    validationRules.messages[id].digits = "Please enter whole numbers only.";
}

ui.helpers.getButton = function(additionalClasses, text, dataTarget) {
    return $('<a class="btn btn-primary btn-lg ' + additionalClasses + '"' +
        (dataTarget != null ? 'data-toggle="modal" data-target="#' + dataTarget + '"' : "") +
        '>' + text + '</a>');
}

ui.helpers.registerChangeCallback = function(element, callback)
{
    element.keyup(callback);
    element.change(callback);
}


