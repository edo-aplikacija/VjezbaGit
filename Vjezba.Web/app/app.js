'use strict';
/* global app: true */

var mainApp = angular.module('mainApp', [
                                        'ngResource',
                                        'ui.router',
                                        'LocalStorageModule',
                                        'ui.bootstrap',
                                        'angularUtils.directives.dirPagination',
                                        'igTruncate',
                                        'angularFileUpload',
]);

// CONSTANTS
// In production change to api location http://www.example.com/
mainApp.constant('API_SERVER', 'http://localhost:8080/api/');
mainApp.constant('COMMON_TEMPLATES', 'app/common/views/');
mainApp.constant('HOME_TEMPLATES', 'app/home/views/');
mainApp.constant('HEADER_SEARCH_TEMPLATES', 'app/headerSearch/views/');
mainApp.constant('MAIN_SEARCH_TEMPLATES', 'app/mainSearch/views/');
mainApp.constant('AUTH_TEMPLATES', 'app/authentication/views/');
mainApp.constant('AUTH_PROFILE_TEMPLATES', 'app/authProfile/views/');
mainApp.constant('AUTH_PROFILE_INFO_TEMPLATES', 'app/authProfileInfo/views/');
mainApp.constant('AUTH_PROFILE_MESSAGES_TEMPLATES', 'app/authProfileMessages/views/');
mainApp.constant('PREVIEW_PROFILE_TEMPLATES', 'app/previewProfile/views/');
mainApp.constant('PREVIEW_PROFILE_MESSAGES_TEMPLATES', 'app/previewProfileMessages/views/');
mainApp.constant('PREVIEW_PROFILE_POSTS_TEMPLATES', 'app/previewProfilePosts/views/');

// CONFIG
mainApp.config(['$httpProvider', '$urlRouterProvider', '$stateProvider', 'HOME_TEMPLATES', 'AUTH_TEMPLATES', 'MAIN_SEARCH_TEMPLATES', 'AUTH_PROFILE_TEMPLATES', 'AUTH_PROFILE_INFO_TEMPLATES', 'AUTH_PROFILE_MESSAGES_TEMPLATES', 'PREVIEW_PROFILE_TEMPLATES', 'PREVIEW_PROFILE_MESSAGES_TEMPLATES', 'PREVIEW_PROFILE_POSTS_TEMPLATES',
                function ($httpProvider, $urlRouterProvider, $stateProvider, HOME_TEMPLATES, AUTH_TEMPLATES, MAIN_SEARCH_TEMPLATES, AUTH_PROFILE_TEMPLATES, AUTH_PROFILE_INFO_TEMPLATES, AUTH_PROFILE_MESSAGES_TEMPLATES, PREVIEW_PROFILE_TEMPLATES, PREVIEW_PROFILE_MESSAGES_TEMPLATES, PREVIEW_PROFILE_POSTS_TEMPLATES) {

                    $httpProvider.interceptors.push('authInterceptor');

                    $urlRouterProvider.otherwise('/');

                    $stateProvider
                    // home
                    .state('home', {
                        url: '/',
                        controller: 'homeSearchCtrl',
                        templateUrl: HOME_TEMPLATES + 'home.html',
                    })
                    // main search
                    .state('mainSearch', {
                        url: '/pretraga=:nameToSearch',
                        controller: 'mainSearchCtrl',
                        templateUrl: MAIN_SEARCH_TEMPLATES + 'mainSearch.html',                        
                    })
                    // authentication
                    .state('signup', {
                        controller: 'signupCtrl',
                        templateUrl: AUTH_TEMPLATES + 'signup.html',
                        url: '/registracija',
                    })
                    .state('login', {
                        controller: 'loginCtrl',
                        templateUrl: AUTH_TEMPLATES + 'login.html',
                        url: '/prijava',
                    })
                    .state('validationEmail', {
                        controller: 'validationEmailCtrl',
                        templateUrl: AUTH_TEMPLATES + 'validationEmail.html',
                        url: '/validacija-emaila/:email/:token',
                    })
                    // authProfile
                    .state('authProfile', {
                        controller: 'authProfileCtrl',
                        templateUrl: AUTH_PROFILE_TEMPLATES + 'authProfile.html',
                        url: '/profil',
                    })
                    // authProfileInfo
                    .state('authProfileInfo', {
                        controller: 'authProfileInfoCtrl',
                        url: '/postavke-profila',
                        templateUrl: AUTH_PROFILE_INFO_TEMPLATES + 'authProfileInfo.html',
                    })
                    // authProfileMessages
                    .state('authProfileMessages', {
                        url: '/poruke',
                        controller: 'authProfileMessagesCtrl',
                        templateUrl: AUTH_PROFILE_MESSAGES_TEMPLATES + 'authProfileMessages.html',
                    })
                    .state('authProfileMessages.authProfileListMsg', {
                        templateUrl: AUTH_PROFILE_MESSAGES_TEMPLATES + 'authProfileListMessages.html',
                    })
                    .state('authProfileMessages.authProfileMsgPreview', {
                        url: '/pregled-poruke/:messageId',
                        controller: 'authProfileMsgPreviewCtrl',
                        templateUrl: AUTH_PROFILE_MESSAGES_TEMPLATES + 'authProfileMessagePreview.html',
                    })
                    .state('authProfileMessages.authProfileNewMsg', {
                        controller: 'authProfileNewMsgCtrl',
                        templateUrl: AUTH_PROFILE_MESSAGES_TEMPLATES + 'authProfileNewMessage.html',
                    })
                    // previewProfile
                    .state('previewProfile', {
                        controller: 'previewProfileCtrl',
                        templateUrl: PREVIEW_PROFILE_TEMPLATES + 'previewProfile.html',
                        url: '/pregled-profila/:userId/:name',
                    })
                    // previewProfileFriends
                    .state('previewProfile.previewProfileFriends', {
                        url: '/saradnici',
                        templateUrl: PREVIEW_PROFILE_TEMPLATES + 'previewProfileFriends.html',
                    })
                    // previewProfileMessages
                    .state('previewProfile.previewProfileNewMsg', {
                        controller: 'previewProfileNewMsgCtrl',
                        url: '/posalji-poruku',
                        templateUrl: PREVIEW_PROFILE_MESSAGES_TEMPLATES + 'previewProfileNewMessage.html',
                    })
                    // previewProfilePosts
                    .state('previewProfile.previewProfilePosts', {
                        controller: 'previewProfilePostsCtrl',
                        url: '/novosti',
                        templateUrl: PREVIEW_PROFILE_POSTS_TEMPLATES + 'previewProfilePosts.html',
                    });
                }]);

mainApp.run(function ($rootScope, $state, authDataService) {
    $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
        var authStates = [
            'authProfile',
            'authProfileInfo',
            'authProfileMessages',
            'authProfileMessages.authProfileListMsg',
            'authProfileMessages.authProfileMsgPreview',
            'authProfileMessages.authProfileNewMsg',
            'previewProfile.previewProfileNewMsg'
        ];
        var notAuthStates = [
            'login',
            'signup',
            'validationEmail'
        ];

        if (authStates.indexOf(toState.name) > -1) {
            if (!authDataService.isAuthenticated()) {
                $state.transitionTo('login');
                event.preventDefault();
            }            
        } else if (notAuthStates.indexOf(toState.name) > -1) {
            if (authDataService.isAuthenticated()) {
                $state.transitionTo('authProfile');
                event.preventDefault();
            }
        }       
    });
});