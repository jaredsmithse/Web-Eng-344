angular
    .module('stock', ['ngResource'])
    .factory('TopFive', ['$resource',
        function ($resource) {
            return $resource('/Stock/Quote');
        }])
    .controller('StockCtrl', ['$scope', '$q', 'TopFive',
        function ($scope, $q, TopFive) {
            setInterval(function () {
                TopFive.query(function (results) {
                    $scope.stocks = results;
                });
            }, 5000);
        }]);