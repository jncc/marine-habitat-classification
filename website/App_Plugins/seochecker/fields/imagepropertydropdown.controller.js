angular.module("umbraco")
    .controller("seoChecker.imagePropertyDropdownController",
    function ($scope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {
        $scope.bindData = function () {

            seocheckerBackofficeResources.getAllImageProperties().then(function (res) {
                $scope.model.items = res.data;
            },
            function (data) {
                seocheckerHelper.showServerError();
            });
        };
       
        //Initialize
        $scope.bindData();
    });