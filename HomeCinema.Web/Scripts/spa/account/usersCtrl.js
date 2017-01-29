(function (app) {
    'use strict';

    app.controller('usersCtrl', usersCtrl);

    usersCtrl.$inject = ['$scope','apiService', 'notificationService'];

    function usersCtrl($scope, apiService, notificationService) {
        $scope.pageClass = 'page-home';
        $scope.isLoading = true;
        $scope.isReadOnly = true;
        $scope.loadData = loadData;
        $scope.deleteUser = deleteUser;

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

        function deleteUser(userId) {
            apiService.get('/api/account/delete/' + userId, null,
                        deleteUserCompleted,
                        deleteUserFailed);
        }

        function deleteUserCompleted(result) {
            notificationService.displaySuccess("", "Successfully deleted user");

            removeUserFromList(result.data);
        }

        function removeUserFromList(userId) {
            var index = -1;
            var usersArray = eval($scope.vm.Users);
            for (var i = 0; i < usersArray.length; i++) {
                var user = usersArray[i];
                if (user.Id === userId) {
                    index = i;
                    break;
                }
            }
            $scope.vm.Users.splice(index, 1);
        }

        function deleteUserFailed(response) {
            notificationService.displayError("", "Failed to deleted user");
        }
        loadData();
    }

})(angular.module('homeCinema'));