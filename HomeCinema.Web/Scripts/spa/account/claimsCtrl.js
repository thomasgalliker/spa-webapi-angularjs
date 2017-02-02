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

    app.controller('claimsCtrl', claimsCtrl);

    claimsCtrl.$inject = ['$scope','apiService', 'notificationService'];

    function claimsCtrl($scope, apiService, notificationService) {
        $scope.vm = this;
        $scope.vm.IsEditing = false;
        $scope.pageClass = 'page-home';
        $scope.isLoading = true;
        $scope.isReadOnly = true;
        $scope.loadData = loadData;
        $scope.deleteUser = deleteUser;

        $scope.sortType = 'name'; // set the default sort type
        $scope.sortReverse = false;  // set the default sort order
        $scope.searchUser = '';     // set the default search/filter term

        function loadData() {
            apiService.get('/api/account/claims', null,
                        loadClaimsCompleted,
                        loadFailed);

            apiService.get('/api/account/roles', null,
                        loadRolesCompleted,
                        loadFailed);
        }

        function loadClaimsCompleted(result) {
            $scope.vm.Claims = result.data;
            
            $scope.isLoading = false;
        }

        function loadRolesCompleted(result) {
            $scope.vm.AllRoles = result.data;

            $scope.isLoading = false;
        }

        function loadFailed(response) {
            $scope.isLoading = false;
        }

        function deleteUser(claimId) {
            apiService.get('/api/account/delete/' + claimId, null,
                        deleteUserCompleted,
                        deleteUserFailed);
        }

        function deleteUserCompleted(result) {
            notificationService.displaySuccess("", "Successfully deleted claim");

            removeUserFromList(result.data);
        }

        function removeUserFromList(claimId) {
            var index = -1;
            var claimsArray = eval($scope.vm.Users);
            for (var i = 0; i < claimsArray.length; i++) {
                var claim = claimsArray[i];
                if (claim.Id === claimId) {
                    index = i;
                    break;
                }
            }
            $scope.vm.Users.splice(index, 1);
        }

        function deleteUserFailed(response) {
            notificationService.displayError("", "Failed to deleted claim");
        }

        $scope.toggleSelection = function toggleSelection(claim, role) {
            var claimIndex = $scope.vm.Claims.indexOfObjectWithProperty('Id', claim.Id);
            var index = $scope.vm.Claims[claimIndex].Roles.indexOfObjectWithProperty('Id', role.Id);
            if (index > -1) {
                $scope.vm.Claims[claimIndex].Roles.splice(index, 1);
            } else {
                $scope.vm.Claims[claimIndex].Roles.push(role);
            }
        };

        $scope.editClaims = function editClaims() {
            $scope.vm.IsEditing = !$scope.vm.IsEditing;
        }

        $scope.saveClaims = function saveClaims() {
            apiService.post('/api/account/claims/update', $scope.vm.Claims,
                          saveClaimsCompleted,
                          saveClaimsFailed);
        }

        function saveClaimsCompleted() {
            $scope.vm.IsEditing = false;
            notificationService.displaySuccess("", "Successfully saved claims");
        }

        function saveClaimsFailed() {
            notificationService.displayError("", "Failed to save claims");
        }


        loadData();
    }

})(angular.module('homeCinema'));