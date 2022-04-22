angular.module("umbraco").controller("seoChecker.ExportRedirectsController", function ($scope, localizationService, seocheckerBackofficeResources, seocheckerHelper) {
    $scope.initializeExport = function () {
        seocheckerBackofficeResources.initializeExportRedirects($scope.dialogData).then(function (res) {
            $scope.model = res.data;
            $scope.loaded = true;
        });
    };

    $scope.export = function () {
        
        seocheckerBackofficeResources.exportRedirects($scope.model).then(function (res) {
            $scope.model = res.data;
            if (!$scope.model.isInValid) {
                $scope.submit($scope.model);
            }
        });
    };

    $scope.onCancel = function () {
        $scope.submit(null);
    };

    //Initialize
    $scope.initializeExport();
});