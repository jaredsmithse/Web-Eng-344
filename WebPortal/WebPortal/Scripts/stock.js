angular
    .module('stock', ['ngResource'])
    .factory('Stock', ['$resource',
        function ($resource) {
            return $resource('/api/Stock/:id/:amt', { id: '@id', amt: '@amt' }, {
                'buy': { method: 'POST' },
                'sell': { method: 'DELETE' }
            });
        }])
    .factory('Transaction', ['$resource',
        function ($resource) {
            return $resource('/Stock/Transactions');
        }])
    .factory('TopFive', ['$resource',
        function ($resource) {
            return $resource('/Stock/Quote');
        }])
    .factory('MyStocks', ['$resource',
        function ($resource) {
            return $resource('/Stock/MyStocks');
        }])
    .controller('StockCtrl', ['$scope', '$q', 'TopFive',
        function ($scope, $q, TopFive) {
            setInterval(function () {
                TopFive.query(function (results) {
                    $scope.stocks = results;
                });
            }, 5000);
        }])
    .controller('StockPageCtrl', ['$scope', '$q', 'Stock', 'Transaction',
        function ($scope, $q, Stock, Transaction) {
            $scope.searchBuy = { amt: 1 };

            function refresh() {
                $scope.myStocks = Stock.query();
                $scope.transactions = Transaction.query(function () {
                    for (var i = 0; i < $scope.transactions.length; ++i) {
                        $scope.transactions[i].type = $scope.transactions[i].buy ? "Buy" : "Sell";
                        $scope.transactions[i].total = $scope.transactions[i].Amount * $scope.transactions[i].price;
                        console.log($scope.transactions[i]);
                    }
                });
            };

            refresh();

            $scope.search = function () {
                $scope.searchResult = Stock.get({ id: $scope.searchBuy.symbol }, function (res) {
                    //$scope.searchResult = res;
                });
            };

            $scope.buy = function () {
                Stock.buy({ id: $scope.searchBuy.symbol, amt: $scope.searchBuy.amt });
                refresh();
            };
        }]);