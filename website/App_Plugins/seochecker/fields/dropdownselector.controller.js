angular.module("umbraco")
    .controller("seoChecker.dropdownSelectorController",
    function ($scope, $rootScope) {
        $scope.itemChanged = function (item) {
            $rootScope.$broadcast("seodropdown.changed", { alias: item.alias, value: item.value.selectedItem });
        };

    });
