angular.module("umbraco")
    .controller("seoChecker.autovalidationOptionsController",
function ($scope, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {

	if (!$scope.model.value) {
        $scope.model.value = "Always validate automatically";
	}
	
	//Initialize
	$scope.model.prevalues = [
					"Never validate automatically",
					"Always validate automatically",
					"Automatically validate after save"
	];
});
