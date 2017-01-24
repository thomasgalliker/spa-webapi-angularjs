(function (app) {
    'use strict';

    app.controller('usersCtrl', usersCtrl);

    usersCtrl.$inject = ['$scope','apiService', 'notificationService'];

    function usersCtrl($scope, apiService, notificationService) {
        $scope.pageClass = 'page-home';
        $scope.isLoading = true;
        $scope.isReadOnly = true;
        $scope.loadData = loadData;

        $scope.sortType = 'name'; // set the default sort type
        $scope.sortReverse = false;  // set the default sort order
        $scope.searchUser = '';     // set the default search/filter term

        function loadData() {
            apiService.get('/api/account/users', null,
                        loadCompleted,
                        loadFailed);
        }

        function loadCompleted(result) {
            $scope.vm = result.data;
            $scope.vm.Users = result.data;
            
            $scope.isLoading = false;
        }

        function loadFailed(response) {
            $scope.isLoading = false;
        }

        loadData();
    }

})(angular.module('homeCinema'));