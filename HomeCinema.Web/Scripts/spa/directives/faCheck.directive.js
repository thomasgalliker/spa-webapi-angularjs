(function (app) {
	'use strict';

	app.directive('faCheck', faCheck);

	function faCheck() {
		return {
			restrict: 'E',
			templateUrl: "./Scripts/spa/directives/faCheck.html",
			link: function ($scope, $element, $attrs) {
			    $scope.isChecked = function () {
			        var checked = $scope.$eval($attrs.isChecked);
			        if (checked === true || $attrs.isChecked === 'true')
				        return true;
				    else
				        return false;
				};
			}
		}
	}

})(angular.module('common.ui'));