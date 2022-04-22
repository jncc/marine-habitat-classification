angular.module("umbraco").controller("seoChecker.ConfirmDialogController", function ($scope, localizationService) {

        $scope.initializeConfirm = function () {
            localizationService.localize($scope.dialogData.localizationKey).then(function (value) {
                $scope.confirmCaption = value;
            });
        };
        // <button ng-click="okClick()">OK</button>

        $scope.confirm = function() {
            $scope.submit(true);
        };

        $scope.cancel = function () {
            $scope.submit(false);
        };

        //Initialize
        $scope.initializeConfirm();
    });