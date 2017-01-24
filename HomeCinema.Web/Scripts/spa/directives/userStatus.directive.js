(function (app) {
	'use strict';

	app.directive('userStatus', userStatus);

	function userStatus() {
		return {
			restrict: 'E',
			templateUrl: "./Scripts/spa/directives/userStatus.html",
			link: function ($scope, $element, $attrs) {
				$scope.getUserStatusLabel = function () {
					if ($attrs.isLocked === 'true')
						return 'label label-danger'
					else
						return 'label label-success'
				};
				$scope.getUserStatus = function () {
				    if ($attrs.isLocked === 'true')
				        return 'Locked'
					else
						return 'Unlocked'
				};
			}
		}
	}

})(angular.module('common.ui'));