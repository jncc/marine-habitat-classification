angular.module("umbraco")
    .controller("seoChecker.redirectmanagerController",
    function ($scope, $timeout, $routeParams, notificationsService, dialogService, seocheckerBackofficeResources, seocheckerHelper) {
        $scope.loaded = false;
        $scope.bindData = function () {
            seocheckerBackofficeResources.loadAllRedirects($scope.model).then(function (res) {
                $scope.loaded = true;
                $scope.model = res.data;
                seocheckerHelper.syncPath($scope.model.path);
            });
        };
        $scope.doExport = function (model) {
            var url = seocheckerBackofficeResources.getRedirectExportUrl(model);
            seocheckerHelper.downloadFile(url);
        }

        $scope.doImport = function (model) {
            seocheckerBackofficeResources.importRedirects(model).then(function (res) {
                
            });
        }


        $scope.filter = function () {
            $scope.bindData();
        };

        $scope.sort = function (column) {
            $scope.model.setSortColumn = column;
            $scope.bindData();
        };

        $scope.setRecordCount = function () {
            $scope.model.resetPaging = true;
            $scope.bindData();
        };

        $scope.goToPage = function (pageNumber) {
            $scope.model.paging.currentPage = pageNumber;
            $scope.bindData();
        };

        $scope.showResult = function () {
            return $scope.model.hasRecords || $scope.model.filter.length > 0;
        };

        $scope.handleSelectAll = function () {
            seocheckerHelper.handleSelectAll($scope.model.selectAll, $scope.model.data);
        };

        $scope.anyItemSelected = function () {
            return seocheckerHelper.anyItemSelected($scope.model.data);
        }

        $scope.isSortDirection = function (columnName, sortDirection) {
            return seocheckerHelper.isSortDirection(columnName, sortDirection, $scope.model.orderByProperty, $scope.model.orderByDirection);
        }


        $scope.deleteSelected = function () {
            dialogService.open({
                // set the location of the view
                template: "/app_plugins/seochecker/dialogs/redirects/deleteredirects.html",
                // pass in data used in dialog
                dialogData: seocheckerHelper.getSelectedItems($scope.model.data),
                // function called when dialog is closed
                callback: function (value) {
                    if (value === true) {
                        $scope.model.resetPaging = true;
                        $scope.bindData();
                    }
                }
            });
        };

        $scope.editRedirect = function (id) {
            dialogService.open({
                // set the location of the view
                template: "/app_plugins/seochecker/dialogs/redirects/editredirects.html",
                // pass in data used in dialog
                dialogData: id,
                // function called when dialog is closed
                callback: function (value) {
                    if (value === true) {
                        $scope.model.resetPaging = true;
                        $scope.bindData();
                    }
                }
            });
        };

        $scope.createRedirect = function (id) {
            dialogService.open({
                // set the location of the view
                template: "/app_plugins/seochecker/dialogs/redirects/editredirects.html",
                // pass in data used in dialog
                dialogData: id,
                // function called when dialog is closed
                callback: function (value) {
                    if (value === true) {
                        $scope.model.resetPaging = true;
                        $scope.bindData();
                    }
                }
            });
        };

        $scope.buttonGroup = {
            defaultButton: {
                labelKey: "seoCheckerRedirectManager_createButton",
                hotKey: "ctrl+n",
                hotKeyWhenHidden: true,
                handler: function() {
                    $scope.createRedirect('0');
                }
            },
            subButtons: [
                {
                    labelKey: "seoCheckerExportRedirects_exportOptionButton",
                    hotKey: "ctrl+e",
                    hotKeyWhenHidden: true,
                    handler: function() {
                        dialogService.open({
                            // set the location of the view
                            template: "/app_plugins/seochecker/dialogs/redirects/exportredirects.html",
                            // function called when dialog is closed
                            callback: function (model) {
                                if (model != null) {
                                    $scope.doExport(model);
                                }

                            }
                        });
                    }
                },
                 {
                     labelKey: "seoCheckerImportRedirects_importOptionButton",
                     hotKey: "ctrl+i",
                     hotKeyWhenHidden: true,
                     handler: function () {
                         dialogService.open({
                             // set the location of the view
                             template: "/app_plugins/seochecker/dialogs/redirects/importredirects.html",
                             // function called when dialog is closed
                             callback: function (model) {
                                 $scope.bindData();
                             }
                         });
                     }
                 }
            ]
        };

        //Initialize
        $scope.bindData();
    });