angular.module("umbraco")
    .controller("seoChecker.domainSettingsController",
    function ($scope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {
        $scope.bindData = function () {

            seocheckerBackofficeResources.initializeDomainSettings($routeParams.id).then(function (res) {
                $scope.model = res.data;
                $scope.loaded = true;
                seocheckerHelper.syncPath($scope.model.path);
            },
            function (data) {
                seocheckerHelper.showServerError();
            });
        };

        $scope.save = function () {
            seocheckerBackofficeResources.saveDomainSettings($scope.model).then(function (res) {
                var model = res.data;
                if (!model.isInValid) {
                    $scope.frm.$setPristine();
                    seocheckerHelper.showNotification(model.notificationStatus);
                }
            },
            function (data) {
                seocheckerHelper.showServerError();
            });
        };

        //Initialize
        $scope.bindData();
    });