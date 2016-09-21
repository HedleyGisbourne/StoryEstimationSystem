namespace('ui.completeTask');

ui.completeTask = function(getStoryId, saveCallback) {
    var loaded = function() {

        $('#completeTask').click(function() {
            $('#CompleteTaskModel').modal('show');
        });

        $('#completeTask-submitButton').click(function() {
            var duration = {};
            duration.Years = $('#completeTask-years-input').val();
            duration.Months = $('#completeTask-months-input').val();
            duration.Weeks = $('#completeTask-weeks-input').val();
            duration.Days = $('#completeTask-days-input').val();
            duration.Hours = $('#completeTask-hours-input').val();
            duration.Minutes = $('#completeTask-minutes-input').val();
            if (duration.Years === "0"
                && duration.Months === "0"
                && duration.Weeks === "0"
                && duration.Days === "0"
                && duration.Hours === "0"
                && duration.Minutes === "0") {
                alert('Please input completed duration.');
                return;
            };

            service.callServer($.post("/api/Stories/CompleteTask/" + getStoryId(), duration))
                .done(function() {
                    if (typeof saveCallback === 'function') {
                        saveCallback();
                    }
                });

            $('#CompleteTaskModel').modal('hide');
        });

        $('#completeTask-cancelButton').click(function() { $('#CompleteTaskModel').modal('hide'); });
    };

    $('#CompleteTaskSection').load('/Content/HTML/CompleteTaskModal.html',
        function() {
            loaded();
        });
}


