angular.module("umbraco")
    .controller("seoChecker.emailSettingsController",
    function ($scope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {
        $scope.bindData = function () {

            seocheckerBackofficeResources.initializeEmailSettings($routeParams.id).then(function (res) {
                $scope.model = res.data;
                $scope.loaded = true;
                seocheckerHelper.syncPath($scope.model.path);
            },
            function (data) {
                seocheckerHelper.showServerError();
            });
        };

        $scope.save = function () {
            seocheckerBackofficeResources.saveEmailSettings($scope.model).then(function (res) {
                $scope.model = res.data;
                if (!$scope.model.isInValid) {
                    seocheckerHelper.showNotification($scope.model.notificationStatus);
                }
            },
            function (data) {
                seocheckerHelper.showServerError();
            });
        };

        //Initialize
        $scope.bindData();
    });