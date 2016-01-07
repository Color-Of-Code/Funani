angular.module('funani', [
    'ngRoute',
    'restangular',
    'home',
    'signin',
    'signup'
]).
config(['$routeProvider', function($routeProvider, RestangularProvider) {
    RestangularProvider.setBaseUrl('http://localhost:5000');
    $routeProvider.otherwise({redirectTo: '/home'});
}]);
