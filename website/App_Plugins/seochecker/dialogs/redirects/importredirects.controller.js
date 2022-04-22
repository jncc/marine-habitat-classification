angular.module("umbraco").controller("seoChecker.ImportRedirectsController", function ($scope, localizationService, seocheckerBackofficeResources, seocheckerHelper, fileManager, $http, $rootScope) {
    $scope.initializeImport = function () {
        seocheckerBackofficeResources.initializeImportRedirects($scope.model).then(function (res) {
            $scope.model = res.data;
            $scope.loaded = true;
            
        });
    };

    $scope.updateModel = function () {
        seocheckerBackofficeResources.redirectUpdateModel($scope.model).then(function (res) {
            $scope.model = res.data;
            $scope.loaded = true;

        });
    };

    $scope.import = function () {
        seocheckerBackofficeResources.importRedirects($scope.model).then(function (res) {
            $scope.model = res.data;
            $scope.loaded = true;

        });
    };

    $scope.exportErrors = function () {
        var url = seocheckerBackofficeResources.getExportRedirectErrorsUrl($scope.model);
        seocheckerHelper.downloadFile(url);
    };

    $rootScope.$on("seodropdown.changed", function (event, data) {
        if (data.alias === 'worksheet') {
            $scope.model.resetWorksheet = true;
            $scope.updateModel();
        }
        if (data.alias === 'importProvider') {
            $scope.initializeImport();
        }
    });

    $scope.upload = function () {
        var fileInput = document.getElementById('uploadSeoFile');
         seocheckerHelper.uploadFiles('redirectimport', fileInput.files[0]).then(function(res) {
             $scope.model.importFileInfo = res;
             $scope.updateModel();
         });
    };

    $scope.showUploadButton = function () {
        return angular.isObject($scope.model) &&  $scope.model.fileSelected === false;
    };

    $scope.showImportButton = function () {
        return angular.isObject($scope.model) && $scope.model.fileSelected === true && $scope.model.fileImported === false;
    };

    $scope.showResult = function () {
        return angular.isObject($scope.model) && $scope.model.fileImported === true;
    };

    $scope.hasImportErrors = function () {
        return angular.isObject($scope.model) && $scope.model.importErrors > 0;
    };

    $scope.getTabByName = function(tabName)
    {
        if (angular.isUndefined($scope.model)) {
            return {};
        }
        return seocheckerHelper.getTabByName($scope.model.tabs, tabName);
    }

    $scope.onCancel = function () {
        $scope.submit(false);
    };

    $scope.close = function () {
        $scope.submit(true);
    };

    //Initialize
    $scope.initializeImport();
});