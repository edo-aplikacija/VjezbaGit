'use strict';

angular.module('mainApp').controller('signupCtrl',
		['$scope', '$state', 'authService', 'authDataService',
		 function ($scope, $state, authService, authDataService) {

		     if (authDataService.isAuthenticated() === true) {
		         $state.go('home');
		     } else {
		         // init data
		         $scope.signupError = undefined;
		         $scope.signupSuccess = undefined;
		         $scope.sendingValidation = undefined;
		         $scope.credentials = {
                     name: '',
                     email: '',
                     password: '',
                     confirmPassword:''
		         };

		         var passwordValidation = function (password, confirmPassword) {
		             if (password.length < 6) {
		                 $scope.signupError = 'Ups! Lozinka može imati minimalno 6 karaktera.';
		                 return false;
		             } else if (password.length > 100) {
		                 $scope.signupError = 'Ups! Lozinka može imati maksimalno 100 karaktera.';
		                 return false;
		             } else if (password !== confirmPassword) {
		                 $scope.signupError = 'Ups! Lozinke moraju biti jednake. Pokušajte ponovo!';
		                 return false;
		             } else {
		                 return true;
		             }
		         };

		         $scope.signup = function (credentials) {
		             $scope.signupError = undefined;
		             if (credentials.email && credentials.password && credentials.confirmPassword) {
		                 if (passwordValidation(credentials.password, credentials.confirmPassword)) {
		                     if (credentials.name.length > 2) {
		                         $scope.sendingValidation = true;
		                         authService.signup(credentials).then(
                                     function (credentials) {
                                         // success
                                         $scope.sendingValidation = undefined;
                                         $scope.signupError = undefined;
                                         $scope.signupSuccessTitle = 'Uspješno ste se registrovali!';
                                         $scope.signupSuccessBody = 'Poslali samo vam validacijski kod na vaš email koji ste naveli prilikom registracije. Provjerite vaše dolazne poruke i kliknite na validacijski link kako bi aktivirali vaš račun. Hvala.';
                                         $scope.signupSuccess = true;
                                     },
                                     function (response) {
                                         // error
                                         $scope.sendingValidation = undefined;
                                         $scope.signupError = response.data.message;
                                         $scope.signupSuccess = undefined;
                                         $scope.signupSuccessTitle = undefined;
                                         $scope.signupSuccessBody = undefined;
                                     }
                                     );
		                     } else {
		                         $scope.signupError = 'Ups! Ime treba imati minimalno 3 karaktera. Pokušajte ponovo!';
		                     }    
		                 }
		             } else {
		                 $scope.signupError = 'Ups! Trebali bi popuniti sva polja. Pokušajte ponovo!';
		             }
		         };

		         $scope.closeSignupError = function () {
		             $scope.signupError = undefined;
		         };

		         $scope.closeSignupSuccess = function () {
		             $scope.signupSuccess = undefined;
		             $scope.signupSuccessTitle = undefined;
		             $scope.signupSuccessBody = undefined;
		             $state.go('home');
		         };
		     }
		 }]);