'use strict';

angular.module('mainApp').factory('authService',
		['$http', '$q', 'authDataService', 'API_SERVER',
		 function ($http, $q, authDataService, API_SERVER) {

		     // Login 
		     this.login = function (credentials) {
		         var defered = $q.defer();
		         var url = API_SERVER + 'auth-token';
		         var dataToSend = 'grant_type=password&username=' + credentials.username + '&password=' + credentials.password;

		         $http.post(url, dataToSend, {
		             headers: {
		                 'content-type': 'application/x-www-form-urlencoded'
		             }
		         }).then(
						 function (response) {
						     console.log('authService login', response);
						     var token = response.data.access_token;

						     if (token) {
						         // save token
						         authDataService.setToken(token);
						         defered.resolve(token);
						     } else {
						         defered.reject(response);
						     }
						 },
						 function (response) {
						     defered.reject(response);
						 }
				 );
		         return defered.promise;
		     };

		     // Signup 
		     this.signup = function (credentials) {
		         var defered = $q.defer();
		         var url = API_SERVER + 'signup-user';
		         var dataToSend = credentials;

		         $http.post(url, dataToSend).then(
						 function (response) {
						     defered.resolve(credentials);
						 },
						 function (response) {
						     defered.reject(response);
						 }
				 );
		         return defered.promise;
		     };

             // Change password
		     this.changePassword = function (credentials) {
		         var defered = $q.defer();
		         var url = API_SERVER + 'change-password';
		         var dataToSend = credentials;

		         $http.post(url, dataToSend).then(
						 function (response) {
						     defered.resolve();
						 },
						 function () {
						     defered.reject();
						 }
				 );
		         return defered.promise;
		     }
		    
		     // Loguot
		     this.logout = function () {
		         var defered = $q.defer();
		         // delete token
		         authDataService.destroyToken();
		         // delete user data
		         authDataService.destroyUser();
		         // set authenticated to false
		         authDataService.disableAuthenticated();

		         defered.resolve();

		         return defered.promise;
		     };

		     return this;
		 }]);