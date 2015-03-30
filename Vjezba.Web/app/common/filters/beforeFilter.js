angular.module('mainApp').filter('before', function () {
    return function (date) {
        moment.locale('bs');
        return moment(date).fromNow();
    }
});