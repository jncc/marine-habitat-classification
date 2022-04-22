angular.module("umbraco")
    .controller("seoChecker.notificationsController",
    function ($scope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {

        $scope.bindData = function () {

            seocheckerBackofficeResources.initializeNotifications().then(function (res) {
                $scope.model = res.data;
                $scope.loaded = true;
                seocheckerHelper.syncPath($scope.model.path);
            },
            function (data) {
                seocheckerHelper.showServerError();
            });
        };

        $scope.save = function () {
            seocheckerBackofficeResources.saveNotifications($scope.model).then(function (res) {
                $scope.model = res.data;
                if (!res.data.isInValid) {
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