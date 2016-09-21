namespace('service');

service.userConfirmation = function(positiveCallback, negativeCallback) {
    if (confirm("Are you sure?")) {
        positiveCallback();
    } else {
        if (negativeCallback != null) {
            negativeCallback();
        }
    }
};

service.callServer = function(promise) {
    return $.when(promise)
        .done(function(response) { return response; })
        .fail(function (response) {
            var responseText;
            try {
                responseText = JSON.parse(response.responseText);
            } catch (e) {
                alert(response.status + ' - ' +response.statusText);
                return;
            }

            if (responseText.ExceptionMessage != null) {
                alert(responseText.ExceptionMessage);
            } else {
                alert(responseText.Message);
            }
        });
}