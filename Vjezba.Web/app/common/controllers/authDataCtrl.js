'use strict';
angular.module('mainApp').controller('authDataCtrl',
		['$scope', '$rootScope', '$state', 'authDataService',
		 function ($scope, $rootScope, $state, authDataService) {
		     // Tracking if user data has changed 
		     $scope.$watch(function () { return authDataService.getUser() }, function (data) {
		         $scope.currentUser = data;
		         console.log('currentUser', $scope.currentUser)

		     });

		     // Tracking if user 'authenticated' has changed value
		     $scope.$watch(function () {
		         // this will return 'true' or 'false'
		         return authDataService.isAuthenticated()
		     }, function (data) {
		         $scope.isAuthenticated = data;
		     });
		     $rootScope.$state = $state;

		     $scope.headerSearchText = '';

		     $scope.goToMainSearch = function (data) {
		         $scope.headerSearchText = '';
		         $state.go('mainSearch', { nameToSearch: data })
		     }

		     $scope.$watch(function () { return $state.current.name }, function (state) {
		         if (state === 'home' || state === 'mainSearch') {
		             $scope.showHeaderSearch = undefined;
		         } else {
		             $scope.showHeaderSearch = true;
		         }
		     });
		   		     
		 }]);