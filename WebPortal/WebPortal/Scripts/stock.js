angular
    .module('stock', ['ngResource', 'ui.bootstrap'])
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
    .controller('StockCtrl', ['$scope', '$q', 'Stock',
        function ($scope, $q, Stock) {
            function refresh() {
                Stock.query(function (stocks) {
                    $scope.stocks = stocks.sort(function (x, y) {
                        return y.Profit - x.Profit; // sort desc
                    }).slice(0, 5); // and take the top five
                });
            }
            refresh();
            setInterval(refresh, 10000);
        }])
    .controller('StockPageCtrl', ['$scope', '$q', '$timeout', '$modal', 'Stock', 'Transaction',
        function ($scope, $q, $timeout, $modal, Stock, Transaction) {
            $scope.searchBuy = { amt: 1 };

            function refresh() {
                $scope.myStocks = Stock.query(function () {
                    for (var i = 0; i < $scope.myStocks.length; ++i) {
                        $scope.myStocks[i].tradeAmt = 1; // initialize input value
                        $scope.myStocks[i].Note = JSON.parse($scope.myStocks[i].Note);
                    }
                    $timeout(function () {
                        dataTableStocks();
                    }, 0, false);
                });
                $scope.transactions = Transaction.query(function () {
                    for (var i = 0; i < $scope.transactions.length; ++i) {
                        // parse some data points
                        $scope.transactions[i].date = new Date(parseInt($scope.transactions[i].date.substr(6)));
                        $scope.transactions[i].type = $scope.transactions[i].buy ? "Buy" : "Sell";
                        $scope.transactions[i].total = $scope.transactions[i].Amount * $scope.transactions[i].price;
                    }
                    $timeout(function () {
                        dataTableTransactions();
                    }, 0, false);
                });
                
            };

            refresh();

            $scope.search = function () {
                $scope.searchResult = Stock.get({ id: $scope.searchBuy.symbol });
            };

            $scope.buy = function (stock) {
                var symbol = stock != null ? stock.Symbol : $scope.searchResult.Symbol;
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

            $scope.note = function (stock) {
                $modal.open({
                    templateUrl: 'NoteModalContent.html',
                    backdrop: true,
                    windowClass: 'modal',
                    controller: function ($scope, $modalInstance, $log, stock) {
                        $scope.stock = stock;
                        $scope.ok = function () {
                            Stock.buy({ id: $scope.stock.Symbol }, JSON.stringify($scope.stock.Note));
                            $modalInstance.dismiss('cancel');
                        };
                        $scope.cancel = function () {
                            $modalInstance.dismiss('cancel');
                        };
                    },
                    resolve: {
                        stock: function () {
                            return stock;
                        }
                    }
                });
            };
        }]);


/* Custom filtering function */
if ($.fn.dataTable) {
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            if ($('#myStocks').is(settings.nTable)) {
                // symbol
                var symbol = $('#filter-stocks-symbol').val();
                if (symbol != null && symbol !== "" && data[0].toUpperCase().indexOf(symbol.toUpperCase()) == -1)
                    return false;

                // name
                var name = $('#filter-stocks-name').val();
                if (name != null && name !== "" && data[1].toUpperCase().indexOf(name.toUpperCase()) == -1)
                    return false;

                // amount
                var minAmt = parseInt($('#filter-stocks-amt-min').val(), 10);
                var maxAmt = parseInt($('#filter-stocks-amt-max').val(), 10);
                var amt = parseFloat(data[2]) || 0;
                if ((!isNaN(minAmt) && minAmt > amt) || (!isNaN(maxAmt) && amt > maxAmt))
                    return false;

                // last price
                var minPrice = parseFloat($('#filter-stocks-price-min').val());
                var maxPrice = parseFloat($('#filter-stocks-price-max').val());
                var price = parseFloat(data[3].slice(1)) || 0;
                if ((!isNaN(minPrice) && minPrice > price) || (!isNaN(maxPrice) && price > maxPrice))
                    return false;

                // change
                var minChange = parseFloat($('#filter-stocks-change-min').val());
                var maxChange = parseFloat($('#filter-stocks-change-max').val());
                var change = parseFloat(data[4].replace(/[()$]/g, '')) || 0;
                if ((!isNaN(minChange) && minChange > change) || (!isNaN(maxChange) && change > maxChange))
                    return false;

                // profit
                var minProfit = parseFloat($('#filter-stocks-profit-min').val());
                var maxProfit = parseFloat($('#filter-stocks-profit-max').val());
                var profit = parseFloat(data[5].replace(/[()$]/g, '')) || 0;
                if ((!isNaN(minProfit) && minProfit > profit) || (!isNaN(maxProfit) && profit > maxProfit))
                    return false;
            } else {
                // date
                var dateStart = new Date($('#filter-transactions-date-start').val());
                var dateEnd = new Date($('#filter-transactions-date-end').val());
                var date = new Date(data[0]);
                if ((dateStart != null && dateStart != "" && dateStart > date) || (dateEnd != null && dateEnd != "" && dateEnd < date))
                    return false;

                // symbol
                var symbol = $('#filter-transactions-symbol').val();
                if (symbol != null && symbol !== "" && data[1].toUpperCase().indexOf(symbol.toUpperCase()) == -1)
                    return false;

                // type
                var type = $('#filter-transactions-type').val();
                if (type != null && type !== "" && data[2] !== type)
                    return false;

                // amount
                var minAmt = parseInt($('#filter-transactions-amt-min').val(), 10);
                var maxAmt = parseInt($('#filter-transactions-amt-max').val(), 10);
                var amt = parseFloat(data[3]) || 0;
                if ((!isNaN(minAmt) && minAmt > amt) || (!isNaN(maxAmt) && amt > maxAmt))
                    return false;

                // price
                var minPrice = parseFloat($('#filter-transactions-price-min').val());
                var maxPrice = parseFloat($('#filter-transactions-price-max').val());
                var price = parseFloat(data[4].replace(/[$,]/g, '')) || 0;
                if ((!isNaN(minPrice) && minPrice > price) || (!isNaN(maxPrice) && price > maxPrice))
                    return false;

                // total
                var minTotal = parseFloat($('#filter-transactions-total-min').val());
                var maxTotal = parseFloat($('#filter-transactions-total-max').val());
                var total = parseFloat(data[5].replace(/[$,]/g, '')) || 0;
                if ((!isNaN(minTotal) && minTotal > total) || (!isNaN(maxTotal) && total > maxTotal))
                    return false;
            }
            return true;
        }
    );
}

function dataTableStocks() {
    var table = $('#myStocks').DataTable();
    $('#filter-stocks-symbol, #filter-stocks-name, ' +
        '#filter-stocks-amt-min, #filter-stocks-amt-max, ' +
        '#filter-stocks-price-min, #filter-stocks-price-max, ' +
        '#filter-stocks-change-min, #filter-stocks-change-max, ' +
        '#filter-stocks-profit-min, #filter-stocks-profit-max').on( 'keyup change', function () {
        table.draw();
    });
}

function dataTableTransactions() {
    var table = $('#transactions').DataTable();
    $('#filter-transactions-date-start, #filter-transactions-date-end, ' +
        '#filter-transactions-symbol, #filter-transactions-type, ' +
        '#filter-transactions-amt-min, #filter-transaction-amt-max, ' +
        '#filter-transactions-price-min, #filter-transactions-price-max, ' +
        '#filter-transactions-total-min, #filter-transactions-total-max').on( 'keyup change', function () {
        table.draw();
    });
}