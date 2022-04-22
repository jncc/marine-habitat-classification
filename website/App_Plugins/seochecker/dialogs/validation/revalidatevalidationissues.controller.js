angular.module("umbraco").controller("seoChecker.RevalidateValidationIssuesController", function ($scope, localizationService, seocheckerBackofficeResources, seocheckerHelper, $timeout) {
    $scope.initialize = function () {
        $scope.loaded = false;
        seocheckerBackofficeResources.initializeRevalidateIssues($scope.dialogData).then(function (res) {
            $scope.loaded = true;
            $scope.model = res.data;
            $scope.timer = $timeout($scope.refreshStatus, 1000);
        });
    };
    
    $scope.refreshStatus = function () {
        $scope.timer = $timeout($scope.refreshStatus, 1000);
        seocheckerBackofficeResources.refreshRevalidateIssues($scope.model).then(function (res) {
            $scope.model = res.data;
            if ($scope.model.finished === true) {
                $scope.submit(true);
            }
        });
    };

    $scope.onCancel = function () {
        $scope.submit(false);
    };

    $scope.$on('$destroy', function () {
        $scope.CancelTimer();
    });

    $scope.CancelTimer = function () {
        $timeout.cancel($scope.timer);
    };

    //Initialize
    $scope.initialize();
});