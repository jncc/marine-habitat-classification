angular.module("umbraco")
    .controller("seoChecker.propertyDropdownController",
    function ($scope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {
        $scope.bindData = function () {

            seocheckerBackofficeResources.getAllProperties().then(function (res) {
                $scope.model.items = res.data;
            },
            function (data) {
                seocheckerHelper.showServerError();
            });
        };
       
        //Initialize
        $scope.bindData();
    });