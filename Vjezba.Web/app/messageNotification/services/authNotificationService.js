'use strict';

angular.module('mainApp').service('authNotificationService',
    ['$rootScope','localStorageService', 'API_SERVER',
    function ($rootScope,localStorageService, API_SERVER) {

        var parentProxy = null;
        var connection;

        var initialize = function () {
            //Getting the connection object
            var url = API_SERVER + 'notification'
            var tokenData = localStorageService.get('tokenData');
            // adding token header
            if (tokenData !== null && tokenData !== undefined) {
                $.signalR.ajaxDefaults.headers = { Authorization: 'Bearer ' + tokenData.token };
            }
                
            this.connection = $.hubConnection(url);

            //Creating proxy
            var initProxy = this.connection.createHubProxy('notification');
            this.parentProxy = initProxy;
          
            //Publishing an event when server push init messages
            initProxy.on('onMessageInit', function (message) {
                $rootScope.$broadcast('acceptInitMessageResult', message);
            });

            // Publishing an event when user send new message
            initProxy.on('onNewMessage', function (message) {
                $rootScope.$broadcast('acceptNewMessageResult', message);
            });
            
            //Starting connection
            this.connection.start(function () {
                initProxy.invoke('MessageInitNotification');
            });
                      
        };

        var sendNewMessageNotificationByUserEmail = function (userEmail) {
            // Invoking NewMessageNotificationByUserEmail method
            this.parentProxy.invoke('NewMessageNotificationByUserEmail', userEmail);
        };

        var sendNewMessageNotificationByUserId = function (userId) {
            // Invoking NewMessageNotificationByUserId method
            this.parentProxy.invoke('NewMessageNotificationByUserId', userId);
        };

        var disableNotification = function () {
            this.connection.stop();
        }

        return {
            initialize: initialize,
            sendNewMessageNotificationByUserEmail: sendNewMessageNotificationByUserEmail,
            sendNewMessageNotificationByUserId: sendNewMessageNotificationByUserId,
            disableNotification: disableNotification
        };
    }]);