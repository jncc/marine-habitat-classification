angular.module("umbraco")
    .controller("seoChecker.seocontentPickerController",
    function ($scope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {

        $scope.showDialog = function () {
            var dialogOptions = {
                multiPicker: false,
                startNodeId: null,
                callback: function (data) {
                    $scope.model.value.id = data.id;
                    $scope.model.value.name = data.name;
                },
                idType: "int"
            };
            if ($scope.model.value.contentType === 'media') {
                dialogService.mediaPicker(dialogOptions);

            } else {

                dialogService.contentPicker(dialogOptions);
            }

        };

        $scope.clear = function () {
            $scope.model.value.id = null;
            $scope.model.value.name = '';
        };
      
    });
