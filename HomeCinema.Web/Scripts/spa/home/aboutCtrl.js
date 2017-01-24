(function (app) {
    'use strict';

    app.controller('aboutCtrl', aboutCtrl);

    aboutCtrl.$inject = ['$scope','apiService', 'notificationService'];

    function aboutCtrl($scope, apiService, notificationService) {
        $scope.pageClass = 'page-home';
        $scope.isLoading = true;
        $scope.isReadOnly = true;
        $scope.loadData = loadData;
        $scope.simmulateCrash = simmulateCrash;

        function loadData() {
            apiService.get('/api/about', null,
                        aboutLoadCompleted,
                        aboutLoadFailed);
        }

        function aboutLoadCompleted(result) {
            $scope.vm = result.data;
            $scope.isLoading = false;
        }

        function aboutLoadFailed(response) {
            notificationService.displayError(response.data);
            $scope.isLoading = false;
        }

        function simmulateCrash() {
            apiService.get('/api/about/crash');
        }

        loadData();
    }

})(angular.module('homeCinema'));