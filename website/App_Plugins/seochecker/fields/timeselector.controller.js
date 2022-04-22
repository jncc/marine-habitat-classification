angular.module("umbraco")
    .controller("seoChecker.timeSelectorController",
    function ($scope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {

        $scope.bindData = function () {
            
            $scope.hours = [];
            for (var h = 0; h < 24; h++) {
                $scope.hours.push(h);
            }

            $scope.minutes = [];
            for (var m = 0; m < 60; m++) {
                $scope.minutes.push(m);
            }
        };

        //Initialize
        $scope.bindData();
        
    });