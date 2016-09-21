// TODO still needs validaiton for length of strings
namespace('ui.observations');
var ui = ui;
var service = service;

ui.observations = function (storyId, grid) {

    var partTypeOptions;
    var descriptionInput;

    var observationDefintionClass = "observationDefintionClass";
    var mandatoryPartsSection = $('#MandatoryPartsSection');
    
    var addDescriptionControl = function () {
        var id = "observationInput-description";
        var container = ui.helpers.getFormGroupContainer();
        descriptionInput = ui.helpers.getTextInput(observationDefintionClass, id, id);
        container.append(ui.helpers.getLabel("Description"));
        container.append(descriptionInput);
        container.append(ui.helpers.getValidationSection(id));
        mandatoryPartsSection.append(container);
    };

    var addPartTypeControl = function () {
        var id = "PartType";
        partTypeOptions = ui.helpers.getSelectInput(id);
        var container = ui.helpers.getFormGroupContainer()
            .append(ui.helpers.getLabel("Type"))
            .append(partTypeOptions);

        mandatoryPartsSection.append(container);
    };

    addDescriptionControl();
    addPartTypeControl();

    var applicationConfig = {};
    var configLoaded = service.callServer($.get("/api/ObservationDefinition/GetConfiguration")).done(function(config) {
        applicationConfig = config;
        applicationConfig.PartTypes.forEach(function(part) {
            partTypeOptions.append(ui.helpers.getOptions(part.PartTypeId, part.Name));
        });
    });

    var form = $('#observationForm');

    var backButton = $('#backButton');
    var submitButton = $('#submitButton');
    var changesToInputs = false;
    var storyPartId;
    var partsSection = $('#PartsSection');
    var locked = false;

    var allInputs = [];

    var validationRules = {};

    var initialiseValidationRules = function() {
        validationRules.errorPlacement = function (error, element) {
            error.appendTo($("#" + element.attr('id') + "_validate"));
        };
        validationRules.rules = {};
        validationRules.messages = {};
    }

    var selectedPart = {};

    var getSelectDropdown = function (enumerations, enumerationId) {
        var element = $("<select class='" + observationDefintionClass + " form-control'></select>");

        var enumeration = Enumerable.From(enumerations).Single("$.EnumerationId == " + enumerationId);

        Enumerable.From(enumeration.EnumItems).ForEach(function (item) {
            var option = ui.helpers.getOptions(item.Id, item.Name);
            element.append(option);
        });

        return element;
    };
    
    var addPart = function (part) {
        var id = "observationInput-" + part.PropertyId;
        var container = ui.helpers.getFormGroupContainer();
        var label = ui.helpers.getLabel(part.Description);
        var input = (part.IsEnum)
            ? getSelectDropdown(applicationConfig.Enumerations, part.EnumerationId)
            : ui.helpers.getTextInput(observationDefintionClass, id, id, "0");
            
        var validation = (part.IsEnum)
            ? ""
            : ui.helpers.getValidationSection(id);

        container.append(label, input, validation);

        if (!part.IsEnum) {
            validationRules.rules[id] = {};
            validationRules.messages[id] = {};
            ui.helpers.wholeNumberValidation(validationRules, id);
        }

        partsSection.append(container);

        input.data('propertyId', part.PropertyId);
        allInputs.push(input);
    }

    var updateScreen = function (part) {
        partsSection.empty();

        initialiseValidationRules();
        allInputs = [];
        Enumerable.From(part.PropertyIds).ForEach(function (propertyId) {
            addPart(Enumerable.From(applicationConfig.Properties).Single("$.PropertyId == " + propertyId));
        });

        allInputs.push(descriptionInput);

        form.data('validator', null);
        form.validate(validationRules);
    };

    var partTypeOptionChange = function () {
        selectedPart = Enumerable.From(applicationConfig.PartTypes).Single("$.PartTypeId == " + partTypeOptions.val());
        updateScreen(selectedPart);
    }

    var inputHasChanged = function () {
        if (!changesToInputs) {
            changesToInputs = true;
            submitButton.removeAttr('disabled');
            backButton.html('Discard');
        }
    }

    var setValues = function (definitions) {
        partTypeOptions.val(definitions.PartTypeId);
        partTypeOptionChange();

        definitions.PropertyValues.forEach(function (propertyValue) {
            Enumerable.From(allInputs).ForEach(function (input) {
                if (propertyValue.PropertyId === input.data('propertyId')) {
                    input.val(propertyValue.Value);
                }
            });
        });
    };

    var intialised = function (definitions) {
        partTypeOptions.attr('disabled', 'disabled');
        

        ui.helpers.registerChangeCallback(
            partTypeOptions,
            function () {
                inputHasChanged();
                partTypeOptionChange();
        });
        
        if (definitions == null || definitions.PropertyValues.count === 0) {
            descriptionInput.val("");
            partTypeOptionChange();
        } else {
            descriptionInput.val(definitions.Description);
            setValues(definitions);
        };

        partTypeOptions.removeAttr('disabled');
        
        Enumerable.From(allInputs).ForEach(function (input) {
            ui.helpers.registerChangeCallback(input, function() { inputHasChanged(); });
        });
    };

    var init = function (partId) {
        changesToInputs = false;
        submitButton.attr('disabled', 'disabled');
        backButton.html('Back');
        storyPartId = partId;
        if (partId == null) {
            intialised();
            inputHasChanged();
        } else {
            service.callServer(
                $.when(
                    configLoaded, 
                    $.get("/api/ObservationDefinition/GetDefinitions/" + storyPartId)))
            .done(function(config, definitions) {
                intialised(definitions[0]);
            });
        }
    };

    var saveChanges = function () {
        if (locked)
            return;
        locked = true;
        var data = {};
        data.StoryPartId = storyPartId;
        data.PartTypeId = partTypeOptions.val();
        data.PropertyValues = [];
        data.Description = $('#observationInput-description').val();
        data.StoryId = storyId;

        Enumerable.From(allInputs).ForEach(function(input) {
            if (input.data('propertyId') != null) {
                data.PropertyValues.push({ PropertyId: input.data('propertyId'), Value: input.val() });
            }
        });

        service.callServer($.post("/api/ObservationDefinition/SaveDefinitions", data)).done(function (result) {
            storyPartId = result;
            locked = false;
            grid.refreshGrid();
        });

    };

    $('#submitButton').click(function() {
        if (form.valid()) {
            saveChanges();
            $('#ObservationsModal').modal('hide');
        };
    });

    $('#backButton').click(function() {
        $('#ObservationsModal').modal('hide');
    });

    return { init: init };
};