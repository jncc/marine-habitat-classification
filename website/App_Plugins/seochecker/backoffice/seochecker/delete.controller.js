angular.module("umbraco")
    .controller("seoChecker.deleteController",
    function ($scope, seocheckerBackofficeResources, treeService, navigationService) {
      
        $scope.performDelete = function () {

            //mark it for deletion (used in the UI)
            $scope.currentNode.loading = true;

            treeService.removeNode($scope.currentNode);

            seocheckerBackofficeResources.deleteTreeNodeById($scope.currentNode.id).then(function () {
                $scope.currentNode.loading = false;

                navigationService.hideMenu();
            });

        };

        $scope.cancel = function () {
            navigationService.hideDialog();
        };
    });

