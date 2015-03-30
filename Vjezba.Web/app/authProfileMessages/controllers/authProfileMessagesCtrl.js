'use strict';

angular.module('mainApp').controller('authProfileMessagesCtrl',
		['$scope', '$state', '$rootScope', '$timeout', 'authDataService', 'authProfileMessagesService', 'authProfileSecondMessagesService', 'authUnreadedMessagesService',
function ($scope, $state, $timeout, $rootScope, authDataService, authProfileMessagesService, authProfileSecondMessagesService, authUnreadedMessagesService) {
    
    // INIT DATA
    var loadInitValues = function () {
        $scope.messageLoader = undefined;
        $scope.haveResult = undefined;
        $scope.noResult = undefined;
        $scope.search = {
            searchText : ''
        }
        $scope.newSeachText = '';

        $scope.listOfSelectedMessages = [];
        $scope.messageResult = [];
        $scope.totalMessages = 0;
        // this should match API page pageLength
        $scope.messagesPerPage = 10;

        $scope.pagination = {
            current: 1
        };
        $scope.currentPage = 1;
    }
    // AUTH DATA
    $scope.messagesAuthData = authDataService.getUser();
    // LOAD UNREADED MESSAGES
    var loadUnreadedMessages = function () {
        authUnreadedMessagesService.get().$promise.then(
            function (response) {
                // succcess
                setUnreadedMessages(response.unreadedMessages);
            }
            );
    }
    loadUnreadedMessages();
    // MAIN FUNCTION FOR GETTING MESSAGES
    var loadMessages = function (status, searchText, page) {
        $scope.messageLoader = true;
        $scope.haveResult = undefined;
        $scope.noResult = undefined;
        $scope.status = status;
        authProfileMessagesService.query({ status: status, searchText: searchText, page: page }).$promise.then(
            function (response) {
                // success              
                $scope.messageLoader = undefined;
                if (response[0] === undefined) {
                    $scope.noResult = true;
                } else {
                    $scope.messageResult = response;
                    $scope.totalMessages = response[0].totalMessages;
                    setUnreadedMessages(response[0].unreadedMessages);
                    $scope.haveResult = true;
                    $scope.noResult = undefined;
                }
            },
            function () {
                // error
                $scope.messageLoader = undefined;
                $scope.noResult = true;
            }
            );
    }
    // CHECK STATE AND SHOW INBOX
    if ($state.current.name === 'authProfileMessages') {
        loadInitValues();
        loadMessages('Inbox', '', 1);
        $state.go('authProfileMessages.authProfileListMsg');
    }
    // DELETE MESSAGES
    $scope.deleteMessages = function () {
        if ($scope.listOfSelectedMessages.length > 0) {
            $scope.disableUpdateButton = true;
            var updatedMessages = $scope.listOfSelectedMessages;
            authProfileMessagesService.update(updatedMessages).$promise.then(
                function () {
                    //success
                    var lenOfDelMsg = $scope.listOfSelectedMessages.length;
                    var lenOfmsgResult = $scope.messageResult.length;
                    for (var i = 0; i < lenOfDelMsg; i++) {
                        for (var x = 0; x < lenOfmsgResult; x++) {
                            if ($scope.status === 'Inbox' && $scope.messageResult[x].messageID === $scope.listOfSelectedMessages[i] && $scope.messageResult[x].readAt === null) {
                                if ($scope.unreadedMessages === 1) {
                                    $scope.unreadedMessages = undefined;
                                } else {
                                    $scope.unreadedMessages--;
                                }
                            }
                        }
                        $("#message-" + $scope.listOfSelectedMessages[i]).fadeOut();

                    }
                    $scope.disableUpdateButton = false;
                    $scope.listOfSelectedMessages = [];
                }
                );
        }
    }
    // UNDELETE MESSAGES
    $scope.unDeleteMessages = function () {
        if ($scope.listOfSelectedMessages.length > 0) {
            $scope.disableUpdateButton = true;
            var updatedMessages = $scope.listOfSelectedMessages;
            authProfileSecondMessagesService.save(updatedMessages).$promise.then(
                function () {
                    //success
                    var lenOfDelMsg = $scope.listOfSelectedMessages.length;
                    var lenOfmsgResult = $scope.messageResult.length;
                    for (var i = 0; i < lenOfDelMsg; i++) {
                        for (var x = 0; x < lenOfmsgResult; x++) {
                            if ($scope.messageResult[x].recipientID === $scope.messagesAuthData.profileID && $scope.messageResult[x].messageID === $scope.listOfSelectedMessages[i] && $scope.messageResult[x].readAt === null) {
                                if ($scope.unreadedMessages === undefined) {
                                    $scope.unreadedMessages = 1;
                                } else {
                                    $scope.unreadedMessages++;
                                }
                            }
                        }
                        $("#message-" + $scope.listOfSelectedMessages[i]).fadeOut();

                    }
                    $scope.disableUpdateButton = false;
                    $scope.listOfSelectedMessages = [];
                }
                );
        }
    }
    // UPDATE READ AT TO API
    var updateReadAtStatus = function (message) {
        var messageId = message.messageID;       
        authProfileSecondMessagesService.update({ messageId: messageId }).$promise.then();
        $scope.$emit('messageIsReaded', messageId);
    }
    // HELPER METHODS
    // =============================================================================
    // EVENT UPDATE
    $scope.$on('updateUnreadedMessages', function (e, num) {
        $scope.unreadedMessages = num;
    });
    // PAGINATOR
    $scope.pageChanged = function (status, search, newPage) {
        $scope.listOfSelectedMessages = [];
        loadMessages(status, search.searchText, newPage);
    };
    // SEARCH MESSAGES
    $scope.searchMessages = function (data) {
        loadMessages($scope.status, data, 1);
    }
    // UNREADED MESSAGES
    var setUnreadedMessages = function (num) {
        if (num > 0)
            $scope.unreadedMessages = num;
    }   
    // ON-CLICK MENU
    $scope.loadInboxMessages = function () {
        loadInitValues();
        loadMessages('Inbox', '', 1);
        $state.go('authProfileMessages.authProfileListMsg');
    }
    $scope.loadOutboxMessages = function () {
        loadInitValues();
        loadMessages('Outbox', '', 1);
        $state.go('authProfileMessages.authProfileListMsg');
    }
    $scope.loadDeletedMessages = function () {
        loadInitValues();
        loadMessages('Deleted', '', 1);
        $state.go('authProfileMessages.authProfileListMsg');
    }
    // SELECTING MESSAGES
    $scope.checkedValue = false;
    $scope.listOfSelectedMessages = [];
    $scope.addMessageToList = function (messageId, value) {
        console.log('addMessageToList', messageId, value);
        if (value === true) {
            $scope.listOfSelectedMessages.push(messageId);
        }
        else {
            var lenOfSelcMsg = $scope.listOfSelectedMessages.length;
            for (var i = 0; i < lenOfSelcMsg; i++) {
                if ($scope.listOfSelectedMessages[i] === messageId) {
                    $scope.listOfSelectedMessages.splice(i, 1);
                    lenOfSelcMsg--;
                }                  
            }
        }
        console.log('$scope.listOfSelectedMessages', $scope.listOfSelectedMessages);
    }
    // CHECK IS MESSAGE READED
    $scope.checkIsReaded = function (message) {
        if (message.recipientID === $scope.messagesAuthData.profileID) {
            if (message.readAt === null) {
                return false;
            }
            else {
                return true;
            }           
        } else {
            return true;
        }
    }
    // ON-CLICK PREVIEW
    $scope.previewMessage = function (message) {
        if (!$scope.checkIsReaded(message)) {           
            updateReadAtStatus(message);
            if ($scope.status === 'Inbox') {
                if ($scope.unreadedMessages === 1) {
                    $scope.unreadedMessages = undefined;
                } else {
                    $scope.unreadedMessages--;
                }
            }
        }
        $state.go('authProfileMessages.authProfileMsgPreview', { messageId: message.messageID });
    }   

}]);