angular.module("umbraco")
    .controller("seoChecker.propertySelectorController",
    function ($scope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources) {

        $scope.bindData = function () {
            seocheckerBackofficeResources.initializePropertySelectorField($routeParams.id).then(function (res) {
                $scope.properties = res.data;
                $scope.properties.value = $scope.model.value;
                $scope.properties.filterType = $scope.model.config.propertyTypeFilter;
                $scope.initializeValue();
                $scope.loaded = true;
            });
        };

        $scope.initializeValue = function () {
            seocheckerBackofficeResources.initializePropertySelectorValue($scope.properties).then(function (res) {
                $scope.properties = res.data;
                $scope.model.value = $scope.properties.value;
            });
        };
        
        $scope.selectItems = function () {
            seocheckerBackofficeResources.selectPropertySelectorFields($scope.properties).then(function (res) {
                $scope.properties = res.data;
                $scope.model.value = $scope.properties.value;
            });
        };

        $scope.deSelectItems = function () {
            seocheckerBackofficeResources.deSelectPropertySelectorFields($scope.properties).then(function (res) {
                $scope.properties = res.data;
                $scope.model.value = $scope.properties.value;
            });
        };

        $scope.moveUp = function () {
            seocheckerBackofficeResources.moveUpPropertySelectorFields($scope.properties).then(function (res) {
                $scope.properties = res.data;
                $scope.model.value = $scope.properties.value;
            });
        };

        $scope.moveDown = function () {
            seocheckerBackofficeResources.moveDownPropertySelectorFields($scope.properties).then(function (res) {
                $scope.properties = res.data;
                $scope.model.value = $scope.properties.value;
            });
        };

        //Initialize
        $scope.bindData();

    });