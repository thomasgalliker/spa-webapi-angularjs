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

    app.controller('roleDetailsCtrl', roleDetailsCtrl);

    roleDetailsCtrl.$inject = ['$scope', '$routeParams', 'apiService', 'notificationService'];

    function roleDetailsCtrl($scope, $routeParams, apiService, notificationService) {
        $scope.vm = this;
        $scope.vm.Role = {};
        $scope.vm.AllClaims = [];
        $scope.pageClass = 'page-home';
        $scope.isLoading = true;
        $scope.isReadOnly = true;
        $scope.loadData = loadData;
        $scope.saveRole = saveRole;
        $scope.addRole = addRole;
        $scope.deleteRole = deleteRole;

        if ($routeParams.id === undefined) {
            $scope.IsAdding = true;
        } else {
            $scope.IsAdding = false;
        }

        $scope.toggleSelection = function toggleSelection(role) {
            var index = $scope.vm.Role.Claims.indexOfObjectWithProperty('Id', role.Id);
            if (index > -1) {
                $scope.vm.Role.Claims.splice(index, 1);
            } else {
                $scope.vm.Role.Claims.push(role);
            }
        };

        function loadData() {

            $scope.isLoading = true;

            if ($scope.IsAdding) {

            } else {
                apiService.get('/api/account/role/details/' + $routeParams.id, null,
                    loadCompleted);
            }

            apiService.get('/api/account/claims', null,
                           claimsLoadCompleted);
        }

        function loadCompleted(result) {
            $scope.vm.Role = result.data;
            $scope.vm.Role.Claims = result.data.Claims;

            $scope.isLoading = false;
        }

        function claimsLoadCompleted(result) {
            $scope.vm.AllClaims = result.data;

            $scope.isLoading = false;
        }


        function saveRole() {
            apiService.post('/api/account/role/update/', $scope.vm.Role,
                saveRoleSucceeded,
                saveRoleFailed);
        }

        function saveRoleSucceeded(result) {
            notificationService.displaySuccess("", "Successfully saved role");

            loadCompleted(result);
        }

        function saveRoleFailed(result) {
        }

        function addRole() {
            apiService.post('/api/account/role/add/', $scope.vm.Role,
                addRoleSucceeded,
                addRoleFailed);
        }

        function addRoleSucceeded(result) {
            notificationService.displaySuccess("", "Successfully added role");
        }

        function addRoleFailed(result) {
        }

        function deleteRole() {

        }

        loadData();
    }

})(angular.module('homeCinema'));