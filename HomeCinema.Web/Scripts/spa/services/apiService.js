(function (app) {
    'use strict';

    app.factory('apiService', apiService);

    apiService.$inject = ['$http'];

    function apiService($http) {
        var baseAddress = "/HomeCinema";

        var service = {
            get: get,
            post: post
        };

        function get(url, config, success, failure) {           
            return $http.get(baseAddress + url, config)
                    .then(function (result) {
                        success(result);
                    }, function (error) {
                        if (failure != null) {
                            failure(error);
                        }
                    });
        }

        function post(url, data, success, failure) {
            return $http.post(baseAddress + url, data)
                    .then(function (result) {
                        success(result);
                    }, function (error) {
                        if (failure != null) {
                            failure(error);
                        }
                    });
        }

        return service;
    }

})(angular.module('common.core'));