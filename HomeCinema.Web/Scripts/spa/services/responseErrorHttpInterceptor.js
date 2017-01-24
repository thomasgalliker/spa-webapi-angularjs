(function () {
    'use strict';

    var app = angular.module('homeCinema');

    app.config(function ($provide, $httpProvider) {

        $provide.factory('responseErrorHttpInterceptor', function ($injector, notificationService, $q) {
            return {
                'responseError': function (httpResponse) {
                    var $rootScope;
                    var $location;
                    var title;
                    var message;

                    if (httpResponse.status === 401) {
                        title = "Authentication required (Error " + httpResponse.status + ")";
                        message = "<br/><b>CorrelationId: </b> " + httpResponse.data.CorrelationId +
                                  "<br/><b>Message: </b>Log in <a href=\"#\login\"\">here</a> to get authenticated." +
                                  "<br/><br/>";

                        notificationService.displayError(title, message);
                        $rootScope = $injector.get('$rootScope');
                        $location = $injector.get('$location');
                        $rootScope.previousState = $location.path();
                        $location.path('/login');
                    }
                    else if (httpResponse.status === 400 || httpResponse.status === 403 || httpResponse.status === 404) {
                        $rootScope = $injector.get('$rootScope');
                        $location = $injector.get('$location');
                        $rootScope.previousState = $location.path();
                        $location.path('/error/' + httpResponse.status);
                    }
                    else if (httpResponse.status === 500) {
                        title = httpResponse.statusText;
                        message = "<small>" +
                            "<br/>Error code: <b>" + httpResponse.status + "</b>" +
                            "<br/>CorrelationId: <b>" + httpResponse.data.CorrelationId + "</b>" +
                            "<br/>Message: <b> " + httpResponse.data.Message + "</b>" +
                            "<br/><br/>Check the <a href=\"#\logviewer\"\">error logs</a> for further information." +
                            "</small>";
                        notificationService.displayError(title, message);
                        $rootScope = $injector.get('$rootScope');
                        $location = $injector.get('$location');
                        $rootScope.previousState = $location.path();
                        $location.path('/error/500');
                    }
                    else {
                        var genericMessage = httpResponse.statusText + " (" + httpResponse.status + ")";
                        notificationService.displayError(genericMessage);
                    }

                    console.error(httpResponse);
                    return $q.reject(httpResponse);
                }
            };
        });

        $httpProvider.interceptors.push('responseErrorHttpInterceptor');
    });

})();