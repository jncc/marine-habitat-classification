angular.module("umbraco").controller("seoChecker.DeleteConfigurationIssuesController", function ($scope, localizationService, seocheckerBackofficeResources, seocheckerHelper) {
    $scope.initializeConfirm = function () {
        seocheckerBackofficeResources.initializeDeleteConfigurationIssues($scope.dialogData).then(function (res) {
            $scope.model = res.data;
            $scope.loaded = true;
        });
    };
    // <button ng-click="okClick()">OK</button>

    $scope.confirm = function () {
        seocheckerBackofficeResources.bulkDeleteConfigurationIssues($scope.model).then(function (res) {
            $scope.model = res.data;
            seocheckerHelper.showNotification($scope.model.notificationStatus);
            $scope.submit(true);
        });


    };

    $scope.cancel = function () {
        $scope.submit(false);
    };

    //Initialize
    $scope.initializeConfirm();
});