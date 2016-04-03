var dataClusteringApp = angular.module('dataClusteringApp', ['smart-table']);


dataClusteringApp.controller('viewAllTweets', ['$scope', '$log', '$http', function ($scope, $log, $http) {

    $http({
        method:'GET',
        url: '/Home/Tweetsdata'
    }).then(function successCallback(response) {
        $scope.allTweets = response.data;
     }, function errorCallback(response) {
    });


}]);