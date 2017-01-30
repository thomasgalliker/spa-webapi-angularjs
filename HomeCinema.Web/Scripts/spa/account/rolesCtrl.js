(function (app) {
    'use strict';

    app.controller('rolesCtrl', rolesCtrl);

    rolesCtrl.$inject = ['$scope','apiService', 'notificationService'];

    function rolesCtrl($scope, apiService, notificationService) {
        $scope.pageClass = 'page-home';
        $scope.isLoading = true;
        $scope.isReadOnly = true;
        $scope.loadData = loadData;
        $scope.deleteUser = deleteUser;

        $scope.sortType = 'name'; // set the default sort type
        $scope.sortReverse = false;  // set the default sort order
        $scope.searchUser = '';     // set the default search/filter term

        function loadData() {
            apiService.get('/api/account/roles', null,
                        loadCompleted,
                        loadFailed);
        }

        function loadCompleted(result) {
            $scope.vm = result.data;
            $scope.vm.Roles = result.data;
            
            $scope.isLoading = false;
        }

        function loadFailed(response) {
            $scope.isLoading = false;
        }

        function deleteUser(roleId) {
            apiService.get('/api/account/delete/' + roleId, null,
                        deleteUserCompleted,
                        deleteUserFailed);
        }

        function deleteUserCompleted(result) {
            notificationService.displaySuccess("", "Successfully deleted role");

            removeUserFromList(result.data);
        }

        function removeUserFromList(roleId) {
            var index = -1;
            var rolesArray = eval($scope.vm.Users);
            for (var i = 0; i < rolesArray.length; i++) {
                var role = rolesArray[i];
                if (role.Id === roleId) {
                    index = i;
                    break;
                }
            }
            $scope.vm.Users.splice(index, 1);
        }

        function deleteUserFailed(response) {
            notificationService.displayError("", "Failed to deleted role");
        }
        loadData();
    }

})(angular.module('homeCinema'));