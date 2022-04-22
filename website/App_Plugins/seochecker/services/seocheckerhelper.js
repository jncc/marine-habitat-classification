angular.module('umbraco.services')
.factory('seocheckerHelper', function (notificationsService, navigationService, seocheckerBackofficeResources, $timeout) {
    var service = {
        showNotification: function (notificationStatus) {
            if (notificationStatus.isError === false) {
                notificationsService.success(notificationStatus.header, notificationStatus.description);
            } else {
                notificationsService.error(notificationStatus.header, notificationStatus.description);
            }
        },
        showServerError: function () {
            notificationsService.error("Server error", "A server error occured");
        }
        ,
        applyValidationErrors: function (source, target) {
            target.isInValid = source.isInValid;
            target.validationMessages = source.validationMessages;
        }

        ,
        syncPath: function (path) {
            navigationService.syncTree({ tree: 'seochecker', path: path });
        },
        isSortDirection: function (columnName, sortDirection, currentSortColumn, currentSortDirection) {
            return columnName === currentSortColumn && sortDirection === currentSortDirection;
        }
        ,
        handleSelectAll: function (selected, items) {
            angular.forEach(items, function (item) {
                item.selected = selected;
            });
        }
          ,
        getSelectedItems: function (items) {
            var result = [];
            angular.forEach(items, function (item) {
                if (item.selected === true) {
                    result.push(item);
                }
            }
          );
            return result;
        },
        anyItemSelected: function (items) {
            var result = false;

            angular.forEach(items, function (item) {
                if (item.selected === true) {
                    result = true;
                    return;
                }
            }
           );
            return result === true;
        },
        downloadFile: function (url) {
            var redirectExport = document.createElement('a');
            redirectExport.id = "downloadframe";
            redirectExport.style.display = 'none';
            document.body.appendChild(redirectExport);
            redirectExport.href = url;
            redirectExport.click();
            //remove all traces
            $timeout(function () {
                document.body.removeChild(redirectExport);
            }, 1000);
        }
        ,
        uploadFiles: function (folder, fileCollection) {
            var formData = new FormData();
            formData.append("folderName", folder);
            formData.append("uploadfiles", fileCollection);
            
            return seocheckerBackofficeResources.uploadFiles(formData).then(function success(response) {
              return response.data;

          }, function error(response) {

              notificationsService.error(response.data);
          });
        }
          ,
        getTabByName: function (tabs, name) {
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].alias === name) {
                    return tabs[i];
                }
            }

            //just return an empty tab when nothing is found
            return {
                active: false,
                alias: 'emptyTab',
                id: -1,
                label: 'empty',
                properties:[]
            };
        }
         ,
        updateTab: function (oldModel, newModel, tabName) {

            var newtab = service.getTabByName(newModel.tabs, tabName);
            var oldTabs = service.getTabByName(oldModel.tabs, tabName);

            oldTabs.properties = newtab.properties;
        },
        isNullOrUndefined: function(val) {
            return  val === null || angular.isUndefined(val) ;
        }
    };
    return service;
});
