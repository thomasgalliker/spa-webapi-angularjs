(function (app) {
    'use strict';

    Array.prototype.indexOfObjectWithProperty = function (propertyName, propertyValue) {
        for (var i = 0, len = this.length; i < len; i++) {
            if (this[i][propertyName] === propertyValue) return i;
        }

        return -1;
    };


    Array.prototype.containsObjectWithProperty = function (propertyName, propertyValue) {
        return this.indexOfObjectWithProperty(propertyName, propertyValue) != -1;
    };

    app.controller('accountDetailsCtrl', accountDetailsCtrl);

    accountDetailsCtrl.$inject = ['$scope', '$routeParams', 'apiService', 'notificationService'];

    function accountDetailsCtrl($scope, $routeParams, apiService, notificationService) {
        $scope.vm = this;
        $scope.vm.User = {};
        $scope.pageClass = 'page-home';
        $scope.isLoading = true;
        $scope.isReadOnly = true;
        $scope.loadData = loadData;
        $scope.saveAccount = saveAccount;
        $scope.addAccount = addAccount;
        $scope.deleteAccount = deleteAccount;

        if ($routeParams.id === undefined) {
            $scope.IsAdding = true;
        } else {
            $scope.IsAdding = false;
        }

        $scope.toggleSelection = function toggleSelection(role) {
            var index = $scope.vm.User.Roles.indexOfObjectWithProperty('Id', role.Id);
            if (index > -1) {
                $scope.vm.User.Roles.splice(index, 1);
            } else {
                $scope.vm.User.Roles.push(role);
            }
        };

        function loadData() {

            $scope.isLoading = true;

            if ($scope.IsAdding) {

            } else {
                apiService.get('/api/account/details/' + $routeParams.id, null,
                    loadCompleted);
            }

            apiService.get('/api/account/roles', null,
                           rolesLoadCompleted);
        }

        function loadCompleted(result) {
            $scope.vm.User = result.data;
            $scope.vm.User.Roles = result.data.Roles;
            $scope.vm.User.Roles = result.data.Roles;
            $scope.vm.User.Claims = result.data.Claims;

            $scope.isLoading = false;
        }

        function rolesLoadCompleted(result) {
            $scope.vm.AllRoles = result.data;

            $scope.isLoading = false;
        }


        function saveAccount() {
            apiService.post('/api/account/update/', $scope.vm.User,
                saveAccountSucceeded,
                saveAccountFailed);
        }

        function saveAccountSucceeded(result) {
            notificationService.displaySuccess("", "Successfully saved user");

            loadCompleted(result);
        }

        function saveAccountFailed(result) {
        }

        //TODO GATH: Is this really a valid workflow?
        function addAccount() {
            apiService.post('/api/account/add/', $scope.vm.User,
                saveAccountSucceeded,
                saveAccountFailed);
        }

        function addAccountSucceeded(result) {
            notificationService.displaySuccess("", "Successfully added user");
        }

        function addAccountFailed(result) {
        }

        function deleteAccount() {

        }

        loadData();
    }

})(angular.module('homeCinema'));