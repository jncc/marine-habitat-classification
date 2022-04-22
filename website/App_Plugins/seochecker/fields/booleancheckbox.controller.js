angular.module("umbraco")
    .controller("seoChecker.booleanFieldController",
    function ($scope, $rootScope) {
        $scope.itemChanged = function (item) {
            $rootScope.$broadcast("seobooleanCheckbox.changed", { alias: item.alias, value: item.value });
        };

    });
