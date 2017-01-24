(function (app) {
    'use strict';

    app.factory('notificationService', notificationService);

    function notificationService() {

        toastr.options = {
            "closeButton": true,
            "preventDuplicates": true,
            "debug": false,
            "positionClass": "toast-top-right",
            "onclick": null,
            "fadeIn": 300,
            "fadeOut": 1000,
            "timeOut": 3000,
            "extendedTimeOut": 1000
        };

        var service = {
            displaySuccess: displaySuccess,
            displayError: displayError,
            displayWarning: displayWarning,
            displayInfo: displayInfo
        };

        return service;

        function displaySuccess(message, title) {

            if (title === undefined) {
                toastr.success(message);
            } else {
                toastr.success(message, title);
            }
        }

        function displayError(title, message) {

            var optionsOverride = {
                "fadeIn": 60,
                "fadeOut": 60,
                "timeOut": 30000,
                "extendedTimeOut": 30000
            };

            if (Array.isArray(title)) {
                title.forEach(function (err) {
                    toastr.error(err, "", optionsOverride);
                });
            } else {
                toastr.error(message, title, optionsOverride);
            }
        }

        function displayWarning(message) {
            toastr.warning(message);
        }

        function displayInfo(message) {
            toastr.clear();
            toastr.info(message);
        }

    }

})(angular.module('common.core'));