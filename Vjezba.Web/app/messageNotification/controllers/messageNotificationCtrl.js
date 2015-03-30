'use strict';

angular.module('mainApp').controller('messageNotificationCtrl',
		['$scope', '$state', '$rootScope', 'authDataService', 'authNotificationService', 'authProfileSecondMessagesService',
		 function ($scope, $state, $rootScope, authDataService, authNotificationService, authProfileSecondMessagesService) {

		     // INIT DATA
		     var initData = function () {
		         $scope.messageNotificationData = undefined;
		         $scope.totalNewNotifMessages = undefined;
		         $scope.noNotifMessages = undefined;
		     }
		     // REMOVE ALERT NOTIFICATION
		     $scope.removeAlertNotification = function () {
		         $scope.totalNewNotifMessages = undefined;
		     }
             // SET INIT MESSAGE NOTIFICATTION DATA
		     var setInitMessageNotificationData = function (data) {
		         var lenOfData = data.length;
		         if (data.length > 0) {
		             $scope.totalNewNotifMessages = lenOfData;
		             $scope.messageNotificationData = data;
		             $scope.noNotifMessages = undefined
		         } else {
		             $scope.noNotifMessages = true;
		         }
		     }
             // UPDATE MESSAGE NOTIFICATION DATA
		     var updateMessageNotificationData = function (data) {
		         var lenOfData = data.length;
		         if (data.length > 0) {
		             if ($scope.totalNewNotifMessages === undefined) {
		                 $scope.totalNewNotifMessages = lenOfData;
		             } else {
		                 $scope.totalNewNotifMessages += lenOfData;
		             }
		             $rootScope.$broadcast('updateUnreadedMessages', $scope.totalNewNotifMessages);
		             
		             if ($scope.messageNotificationData === undefined) {
		                 $scope.messageNotificationData = data;
		             } else {
		                 var oldData = $scope.messageNotificationData;
		                 var newData = data;
		                 var lenOfOldData = oldData.length;
		                 for (var i = 0; i < lenOfOldData; i++) {
		                     newData.push(oldData[i]);
		                 }
		                 $scope.messageNotificationData = newData;
		             }           
		             $scope.noNotifMessages = undefined
		         } else {
		             $scope.noNotifMessages = true;
		         }
		     }
             // REMOVE MESSAGE 
		     var removeMessage = function (messageId) {
		         var lenOfMsg = $scope.messageNotificationData.length;
		         for (var i = 0; i < lenOfMsg; i++) {
		             if ($scope.messageNotificationData[i].MessageID === messageId) {
		                 $scope.messageNotificationData.splice(i, 1);
		                 lenOfMsg--;
		             }		                		             
		         }
		         if ($scope.messageNotificationData.length < 1) {
		             $scope.noNotifMessages = true;
		         }
		         if ($scope.totalNewNotifMessages !== undefined && $scope.totalNewNotifMessages > 1) {
		             $scope.totalNewNotifMessages--;
		         } else if ($scope.totalNewNotifMessages === 1) {
		             $scope.totalNewNotifMessages = undefined;
		         }
		         if ($scope.messageNotificationData.length > 0) {
		             $rootScope.$broadcast('updateUnreadedMessages', $scope.messageNotificationData.length);
		         } else {
		             $rootScope.$broadcast('updateUnreadedMessages', undefined);
		         }
		         
		     }
             // GO TO PREVIEW MESSAGE
		     $scope.goToPreviewMessage = function (message) {
		         var messageId = message.MessageID;
		         authProfileSecondMessagesService.update({ messageId: messageId }).$promise.then(
                     function () {
                         // success
                         removeMessage(messageId);
                         
                     }
                     );
		         $state.go('authProfileMessages.authProfileMsgPreview', { messageId: messageId });
		     }
             // START CONNECTION
		     $scope.$watch('isAuthenticated', function (data) {
		         if (data === true) {
		             initData();
		             authNotificationService.initialize();
		         } else {
		             initData();
		         }
		     }, true);		     
		     // Updating notification messages after receiving a init messages through the event
		     $scope.$on('acceptInitMessageResult', function (e, message) {
		         $scope.$apply(function () {
		             setInitMessageNotificationData(message);
		         });
		     });
		     // Updating notification messages after receiving a new message through the event
		     $scope.$on('acceptNewMessageResult', function (e, message) {
		         $scope.$apply(function () {
		             updateMessageNotificationData(message);
		         });
		     });
             // Removing message after receiving message through the event
		     $rootScope.$on('messageIsReaded', function (e, messageId) {
		         console.log('messageIsReaded', messageId);
		         removeMessage(messageId);
		     });
            
		       
		 }]);