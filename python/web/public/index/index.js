angular.module('funani', [
    'ngRoute',
    'home',
    'signin',
    'signup'
]).
config(['$routeProvider', function($routeProvider) {
    $routeProvider.otherwise({redirectTo: '/home'});
}]);
