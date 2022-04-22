angular.module("umbraco").controller("seoChecker.EditRedirectsController", function ($scope, localizationService, seocheckerBackofficeResources, seocheckerHelper) {
    $scope.initializeConfirm = function () {
        seocheckerBackofficeResources.initializeEditRedirect($scope.dialogData).then(function (res) {
            $scope.model = res.data;
            $scope.loaded = true;

            $scope.$watch("model.tabs['0'].properties[0].value", function (newVal, oldVal) {
                if (newVal != oldVal) {
                    if (!seocheckerHelper.isNullOrUndefined(newVal)) {
                        seocheckerBackofficeResources.getContentTypeOfRedirectUrl(newVal).then(function (res) {
                            $scope.model.tabs[0].properties[1].value.contentType = res.data;
                        });
                    }
                }
            }, true);
        });
    };

    $scope.save = function () {
        seocheckerBackofficeResources.saveRedirect($scope.model).then(function (res) {
            $scope.model = res.data;
            if (!$scope.model.isInValid) {
                seocheckerHelper.showNotification($scope.model.notificationStatus);
                $scope.submit(true);
            }
        });


    };

    $scope.onCancel = function () {
        $scope.submit(false);
    };

    //Initialize
    $scope.initializeConfirm();
});