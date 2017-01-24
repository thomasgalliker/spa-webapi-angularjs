(function (app) {
    'use strict';

    app.controller('accountDetailsCtrl', accountDetailsCtrl);

    accountDetailsCtrl.$inject = ['$scope', '$routeParams', 'apiService'];

    function accountDetailsCtrl($scope, $routeParams, apiService) {
        $scope.pageClass = 'page-home';
        $scope.isLoading = true;
        $scope.isReadOnly = true;
        $scope.loadData = loadData;

        function loadData() {

            $scope.isLoading = true;

            apiService.get('/api/account/details/' + $routeParams.id, null,
                loadCompleted);
        }

        function loadCompleted(result) {
            $scope.vm = result.data;
            $scope.vm.Roles = result.data.Roles;
            $scope.vm.Claims = result.data.Claims;

            $scope.isLoading = false;
        }

        loadData();
    }

})(angular.module('homeCinema'));