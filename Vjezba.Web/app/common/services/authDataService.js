'use strict';
angular.module('mainApp').factory('authDataService',
		['$q', 'localStorageService', function ($q, localStorageService) {
		    // We will try to save data to localstorage because if user refresh page
		    // or open page in new tab data must be saved
		    // Token data
		    var _token;
		    // Call this function to save token
		    this.setToken = function (token) {
		        if (localStorageService.isSupported) {
		            localStorageService.set('tokenData', { token: token });
		        }
		        _token = token;
		    };
		    // Call this function to get token
		    this.getToken = function () {
		        if (_token === undefined) {
		            var tokenData = localStorageService.get('tokenData');
		            if (tokenData === null) {
		                return _token;
		            } else {
		                this.setToken(tokenData.token);
		                return tokenData.token;
		            }
		        } else {
		            return _token;
		        }
		    };
		    // Call this function to delete token
		    this.destroyToken = function () {
		        if (localStorageService.isSupported) {
		            localStorageService.remove('tokenData');
		        }
		        _token = undefined;
		    };
		    // Authenticated
		    var _authenticated = false;
		    // Call this function to check is user authenticated
		    this.isAuthenticated = function () {
		        if (_authenticated === false) {
		            var authenticatedData = localStorageService.get('authenticatedData');
		            if (authenticatedData === null) {
		                return _authenticated;
		            } else {
		                this.setAuthenticated();
		                return authenticatedData.authenticated;
		            }
		        } else {
		            return _authenticated;
		        }
		    };
		    // Call this function to set 'authenticated' to  'true'
		    this.setAuthenticated = function () {
		        if (localStorageService.isSupported) {
		            localStorageService.set('authenticatedData', { authenticated: true });
		        }
		        _authenticated = true;
		    };

		    // Call this function to set 'authenticated' to 'false'
		    this.disableAuthenticated = function () {
		        if (localStorageService.isSupported) {
		            localStorageService.remove('authenticatedData');
		        }
		        _authenticated = false;
		    };
		    // User Data
		    var _user;
		    // Call this function to save user data
		    this.setUser = function (data) {
		        if (localStorageService.isSupported) {
		            localStorageService.set('userData', { data: data });
		        }
		        _user = data
		    };

            // Call this function to update user data
		    this.updateUserData = function (newData) {
		        if (localStorageService.isSupported) {
		            var oldData = localStorageService.get('userData');

		            oldData.data.name = newData.name;
		            oldData.data.category = newData.category;
		            oldData.data.phone = newData.phone;
		            oldData.data.country = newData.country;
		            oldData.data.city = newData.city;
		            oldData.data.address = newData.address;
		            oldData.data.description = newData.description;
		            localStorageService.remove('userData');
		            localStorageService.set('userData', { data: oldData.data });
		        }
		    };

		    // Call this function to get user data
		    this.getUser = function () {
		        if (_user === undefined) {
		            var userData = localStorageService.get('userData');
		            if (userData === null) {
		                return _user;
		            } else {
		                this.setUser(userData.data);
		                return userData.data;
		            }
		        } else {
		            return _user;
		        }
		    };

		    // Call this function to delete user data
		    this.destroyUser = function () {
		        if (localStorageService.isSupported) {
		            localStorageService.remove('userData');
		        }
		        _user = undefined;
		    };

            // Call this to check if user has confirm email
		    this.isValidated = function () {
		        if (_user === undefined) {
		            var userData = localStorageService.get('userData');
		            if (userData === null) {
		                return false;
		            } else {
		                return userData.data.isValidate;
		            }
		        } else {
		            return _user.isValidate;
		        }
		    }
		    return this;

		}]);