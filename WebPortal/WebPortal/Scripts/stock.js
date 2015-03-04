angular
    .module('stock', ['ngResource'])
    .factory('TopFive', ['$resource',
        function ($resource) {
            return $resource('http://dev.markitondemand.com/Api/v2/Quote/json');
        }])
    .controller('StockCtrl', ['$scope', '$q', 'TopFive',
        function ($scope, $q, TopFive) {
            console.log("here");
            //TODO refresh periodically
            $q.all([
                TopFive.query({ symbol: 'AAPL' }).$promise,
                TopFive.query({ symbol: 'MSFT' }).$promise,
                TopFive.query({ symbol: 'GOOG' }).$promise,
                TopFive.query({ symbol: 'AMZN' }).$promise,
                TopFive.query({ symbol: 'FB' }).$promise
            ]).then(function (results) {
                console.log(results);
                $scope.stocks = results;
            });
        }]);