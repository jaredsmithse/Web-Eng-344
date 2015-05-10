angular
    .module('stock', ['ngResource'])
    .factory('Stock', ['$resource',
        function ($resource) {
            return $resource('/api/Stock/:id/:amt', { id: '@id', amt: '@amt' }, {
                'buy': { method: 'PUT' },
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
                $scope.myStocks = Stock.query(function () {
                    for (var i = 0; i < $scope.myStocks.length; ++i) {
                        $scope.myStocks[i].tradeAmt = 1; // initialize input value
                    }
                });
                $scope.transactions = Transaction.query(function () {
                    for (var i = 0; i < $scope.transactions.length; ++i) {
                        // parse some data points
                        $scope.transactions[i].date = new Date(parseInt($scope.transactions[i].date.substr(6)));
                        $scope.transactions[i].type = $scope.transactions[i].buy ? "Buy" : "Sell";
                        $scope.transactions[i].total = $scope.transactions[i].Amount * $scope.transactions[i].price;
                    }
                });
            };

            refresh();

            $scope.search = function () {
                $scope.searchResult = Stock.get({ id: $scope.searchBuy.symbol }, function (res) {
                    //$scope.searchResult = res;
                });
            };

            $scope.buy = function (stock) {
                var symbol = stock != null ? stock.Symbol : $scope.searchBuy.symbol;
                var amount = stock != null ? stock.tradeAmt : $scope.searchBuy.amt;
                Stock.buy({ id: symbol, amt: amount }, null, function () {
                    refresh();
                });
            };

            $scope.sell = function (stock) {
                var symbol = stock.Symbol;
                var amount = stock.tradeAmt;
                Stock.sell({ id: symbol, amt: amount }, null, function () {
                    refresh();
                });
            };
        }]);