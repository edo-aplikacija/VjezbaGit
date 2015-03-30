'use strict';

angular.module('mainApp').factory('authInterceptor',
		['$q', '$injector', 'authDataService', 'authNotificationService',
		 function ($q, $injector, authDataService, authNotificationService) {
		     return {
		         request: function (config) {
		             config.headers = config.headers || {};
		             var checkToken = authDataService.getToken();
		             // Setting up header for request to protected views
		             // we must send token that we recived on login
		             if (checkToken !== undefined) {
		                 config.headers.Authorization = 'Bearer ' + checkToken;
		             }
		             return config;
		         },
		         // if user is't authorized send him to login view
		         responseError: function (response) {
		             if (response.status === 401) {
                         // stop notification
		                 if (authDataService.isValidated === true) {
		                     authNotificationService.disableNotification();
		                 }
		                 // delete token
		                 authDataService.destroyToken();
		                 // delete user data
		                 authDataService.destroyUser();
		                 // set authenticated to false
		                 authDataService.disableAuthenticated();
		                 
		                 $injector.get('$state').transitionTo('login');
		                 return $q.reject();
		             }
		             return $q.reject(response);
		         }
		     };
		 }]);