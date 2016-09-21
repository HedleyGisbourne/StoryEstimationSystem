namespace('ui.grids');

ui.grids.replaceLookup = function (uuid, options) {
    $('.' + uuid + '-lookup').each(function (id, element) {
        var $el = $(element);
        $el.html(options[$el.data('lookup')][$el.html()].Name);
    });
}

ui.grids.initGrid = function (elementId, addEnabled, editEnabled, deleteEnabled, fetchDataUrl, addRecord, editRecord, deleteRecordUrl) {
    var columnDefaults = {};
    var modalAdded = false;
    var gridData;
    var selectedRowId = null;
    var selectedRowNumber = null;
    var primaryKey;
    var addOrEditModal;
    var add = {};
    var edit = {};
    var columnDefinitions;

    var setDefaultValueForColumn = function (colName, val) {
        columnDefaults[colName] = val;
    }

    var getselectedRowId = function () {
        return selectedRowId;
    }

    var elementsToDisable = [];

    var disableEnableElement = function (element) {
        if (selectedRowId == null) {
            element.attr("disabled", "disabled");
        } else {
            element.removeAttr("disabled");
        }
    }

    var disableWhenNoRowSelected = function (element) {
        var $element = common.getJqObject(element);
        elementsToDisable.push($element);
        disableEnableElement($element);
    }

    var disabledEnableAllRegisteredElement = function () {
        elementsToDisable.forEach(function (element) {
            disableEnableElement(element);
        });
    }

    var uuid = common.generateGuid();

    var modalButton = 'btn-edit-or-add-modal';
    var addButton = ui.helpers.getButton(modalButton, 'Add', uuid + '-Modal');
    var editButton = ui.helpers.getButton(modalButton, 'Edit', uuid + '-Modal');
    var deleteButton = ui.helpers.getButton(modalButton, 'Delete');

    var gridContainer = $("#" + elementId);

    gridContainer.after('<div id="' + uuid + '-ModalContainer"></div>');

    var gridControls = $('<div class="application-button-bar"></div');
    var standardGridControls = $('<div class="pull-right"></div>');

    gridControls.append(standardGridControls);
    gridContainer.after(gridControls);

    if (addEnabled) {
        standardGridControls.append(addButton);
        addButton.click(function () {
            addOrEditModal = add;
            $('#' + uuid + '-ModalType').html('Add');
            // Clear the modal inputs.
            $('.' + uuid + '-input').each(function (index, element) {
                var $el = $(element);
                if ($el.hasClass('hidden')) {
                    $el.val(columnDefaults[$el.data('columnName')]);
                }
                else if (element.tagName === 'SELECT') {
                    $el.val('0');
                } else {
                    $el.val('');
                }
            });

            $('.' + uuid + '-input-primary-key').val(null);
        });
    }

    if (editEnabled) {
        standardGridControls.append(editButton);
        disableWhenNoRowSelected(editButton);
        editButton.click(function () {
            addOrEditModal = edit;
            $('#' + uuid + '-ModalType').html('Edit');

            // Update the modal inputs to match the grid row.
            columnDefinitions.forEach(function (column) {
                $('#' + uuid + '-' + column.Name).val(gridData.Data[selectedRowNumber][column.Name]);
            });
        });
    }

    var selectRow = function (row) {
        var $row = $(row);

        $row.parent().find('tr').removeClass('active');
        $row.addClass("active");
        selectedRowId = $row.data("rowId");
        selectedRowNumber = $row.data("rowNumber");
        disabledEnableAllRegisteredElement();
    };

    var lookups = [];

    var refreshGrid = function () {

        selectedRowId = null;
        selectedRowNumber = null;

        service.callServer($.get(fetchDataUrl)).done(function (data) {
            gridData = data;
            var visibleColumns = {};

            gridContainer.children().remove();

            var tableGrid = $('<div class="table-responsive" style="min-height: 400px; max-height:calc(100vh - 260px);overflow-y:auto; margin-bottom:40px;margin-top:40px;"></div>');
            var gridElement = $("<table class='table'></table>");

            tableGrid.append(gridElement);
            gridContainer.append(tableGrid);

            disabledEnableAllRegisteredElement();

            // Add header to the grid
            var row = $("<thead></thead>");
            columnDefinitions = data.ColumnDefinition;
            data.ColumnDefinition.forEach(function (column) {
                if (column.PrimaryKey) {
                    primaryKey = column.Name;
                } else if (!column.HideProperty) {
                    visibleColumns[column.Name] = column;
                    var td = column.Width === 0 ? "<th>" : '<th width="' + column.Width + '%">';
                    row.last().append(td + column.DisplayName + "</th>");
                }

                gridElement.last().append(row);
            });

            // Add rows to the grid
            data.Data.forEach(function (dataRow, i) {
                row = $('<tr></tr>');
                row.data("rowId", dataRow[primaryKey]);
                row.data("rowNumber", i);
                row.click(function () { selectRow(this) });

                for (var cell in visibleColumns) {
                    if (visibleColumns.hasOwnProperty(cell)) {
                        if (visibleColumns[cell].Lookup != null) {
                            var td = $("<td class='" + uuid + "-lookup'>" + dataRow[cell] + "</td>");
                            td.data('lookup', visibleColumns[cell].Lookup);
                            row.last().append(td);
                        } else {
                            row.last().append("<td>" + (dataRow[cell] != null ? dataRow[cell] : "-") + "</td>");
                        }
                    }
                }

                gridElement.last().append(row);
            });

            if (modalAdded) {
                ui.grids.replaceLookup(uuid, ui.grids.newOptions);
            }
            else {
                modalAdded = true;

                $('#' + uuid + '-ModalContainer').load('/Modal', { Id: uuid, RecordType: "System" }, function () {
                    // Add the modal popup fields
                    var form = $('#' + uuid + '-Form');

                    var validationRules = {};
                    validationRules.errorPlacement = function (error, element) {
                        var name = $(element).attr("name");
                        error.appendTo($("#" + name + "_validate"));
                    };
                    validationRules.rules = {};
                    validationRules.messages = {};

                    data.ColumnDefinition.forEach(function (cell) {
                        var unqiueId = uuid + "-" + cell.Name;
                        if (visibleColumns.hasOwnProperty(cell.Name) && cell.Editable) {
                            var input;
                            if (cell.Lookup !== null) {

                                lookups[cell.Lookup] = $('<select class="col-md-7 ' + uuid + '-input" id="' + unqiueId + '" name="' + cell.Name + '"></select>');
                                input = ui.helpers.getFormGroupContainer()
                                    .append(ui.helpers.getLabel(cell.DisplayName))
                                    .append(lookups[cell.Lookup]);

                                //var input = $('<div class="col-md-12 modal-input-block"><label class="col-md-5">' + cell.DisplayName + '</label>').append(lookups[cell.Lookup]);
                                form.prepend(input);
                                ;
                            } else {
                                input = ui.helpers.getFormGroupContainer()
                                    .append(ui.helpers.getLabel(cell.DisplayName))
                                    .append(ui.helpers.getTextInput(uuid + '-input', unqiueId, cell.Name))
                                    .append(ui.helpers.getValidationSection(unqiueId));

                                form.prepend(input);
                                // form.prepend($('<div class="col-md-12 modal-input-block"><label class="col-md-5">' + cell.DisplayName + '</label>'
                                //    + '<input class="col-md-7 ' + uuid + '-input" id="' + unqiueId + '" name="' + cell.Name + '" type="text" />'
                                //   + '<div class="col-md-5"></div><div id="' + unqiueId + '_validate" class="validation-message"></div></div>'));
                            }
                            validationRules.rules[unqiueId] = {};
                            validationRules.messages[unqiueId] = {};
                            validationRules.rules[unqiueId].required = true;

                            if (cell.GridType === 0) // 0 = Number
                            {
                                validationRules.rules[unqiueId].digits = true;
                                validationRules.messages[unqiueId].digits = "Please enter whole numbers only.";
                            }

                            if (cell.GridType === 1) {
                                if (cell.MaxLength > 0) {
                                    validationRules.rules[unqiueId].maxlength = cell.MaxLength;
                                    validationRules.messages[unqiueId].maxlength = "Maximum characters is " + cell.MaxLength;
                                }
                            }
                        } else {
                            var hiddenElement = $('<input name="' + cell.Name + '" class="hidden ' + uuid + '-input" id="' + unqiueId + '" type="text" />');
                            hiddenElement.data('columnName', cell.Name);
                            form.prepend(hiddenElement);
                        }
                    });

                    // populate the dropdowns
                    ui.grids.populate(uuid, lookups);

                    form.validate(validationRules);

                    var getArguments = function () {
                        var $inputs = $('.' + uuid + '-input');

                        var args = {};
                        $inputs.each(function (input) {
                            args[$inputs[input].name] = $($inputs[input]).val();
                        });

                        return args;
                    }

                    form.submit(function () {
                        // Check if the form is valid with JQuery Validate
                        if (!form.valid()) {
                            return false;
                        };

                        var callback = function() {};

                        $('.modal').modal('hide');

                        var url;
                        var args = getArguments();
                        if (addOrEditModal === add) {
                            if (typeof addRecord === 'object') {
                                url = addRecord.url;
                                callback = addRecord.callback;
                            } else {
                                url = addRecord;
                            }

                            args[primaryKey] = "";
                        }

                        else if (addOrEditModal === edit) {
                            if (typeof editRecord === 'object') {
                                url = editRecord.url;
                                callback = editRecord.callback;
                            } else {
                                url = editRecord;
                            }
                        }

                        service.callServer($.post(url, args)).done(function (id) {
                            if (typeof callback === 'function') {
callback(id);
                            } else {
                                refreshGrid();
                            }
                        });

                        return false;
                    });
                });
            }
        });
    }

    if (deleteEnabled) {
        disableWhenNoRowSelected(deleteButton);
        standardGridControls.append(deleteButton);
        deleteButton.click(function () {
            service.userConfirmation(function () {
                service.callServer($.get(deleteRecordUrl + '/' + selectedRowId)).done(function () {
                    refreshGrid();
                });
            });
        });
    }

    refreshGrid();

    var resetFetchDataUrl = function (newUrl) {
        fetchDataUrl = newUrl;
        refreshGrid();
    }

    var addCustomButton = function (elementId, label, href) {
        common.addCustomButton(elementId, label, href, gridControls);
    }

    var getSelectedRowId = function () {
        return selectedRowId;
    }

    return {
        refreshGrid: refreshGrid,
        disableWhenNoRowSelected: disableWhenNoRowSelected,
        getselectedRowId: getselectedRowId,
        resetFetchDataUrl: resetFetchDataUrl,
        addCustomButton: addCustomButton,
        getSelectedRowId: getSelectedRowId,
        setDefaultValueForColumn: setDefaultValueForColumn
    };
}