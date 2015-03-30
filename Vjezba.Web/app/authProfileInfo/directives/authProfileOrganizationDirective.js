'use strict';

// <auth-profile-organization> 
angular.module('mainApp').directive('authProfileOrganization',
    ['AUTH_PROFILE_INFO_TEMPLATES', 'authProfileService', 'authDataService',
function (AUTH_PROFILE_INFO_TEMPLATES, authProfileService, authDataService) {
    return {
        restrict: 'E',
        scope: {
            userData: '=data',
        },
        transclude: false,
        replace: true,
        templateUrl: AUTH_PROFILE_INFO_TEMPLATES + 'authProfileOrganization.html',
        controller: function ($scope) {

            $scope.allowedCategories = [
                'Administracija',
                'Bankarstvo',
                'Ekonomija',
                'Farmaceutska industrija',
                'Informacione tehnologije',
                'Medicina',
                'Metalna industrija',
                'Novinarstvo',
                'Pravo',
                'Prehrambena industrija',
                'Transport i logistika'      
            ];

            $scope.organizationEditError = undefined;

            $scope.editOrganization = function (data) {
                $scope.editModeOrganization = true;
                $scope.editData = angular.copy(data);
            }

            $scope.returnName = function () {
                $scope.editModeOrganization = false;
                $scope.organizationEditError = undefined;
            }

            $scope.saveChanges = function () {
                if ($scope.editData.name && $scope.editData.category) {
                    if ($scope.editData.name.length > 2) {
                        var model = $scope.editData;
                        authProfileService.update(model).$promise.then(
                            function () {
                                // success
                                authDataService.updateUserData(model);
                                $scope.userData.name = $scope.editData.name;
                                $scope.userData.category = $scope.editData.category;

                                $scope.organizationEditError = undefined;
                                $scope.editModeOrganization = false;
                            },
                            function () {
                                // error
                                $scope.organizationEditError = "Ups! Trebate popuniti polja."
                            }
                            );
                    } else {
                        $scope.organizationEditError = "Ups! Ime treba imati minimalno 3 karaktera. Pokušajte ponovo!"
                    }
                                       
                } else {
                    $scope.organizationEditError = "Ups! Trebate popuniti polja."
                }
                
            }

            $scope.closeOrganizationEditError = function () {
                $scope.organizationEditError = undefined;
            }
            
        }
    };

}]);