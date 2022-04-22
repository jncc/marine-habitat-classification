angular.module("umbraco.resources")
    .factory("seocheckerBackofficeResources", function ($http) {
        return {
            //LicenseInfo
            getLicenseInfo: function () {
                return $http.get("backoffice/seochecker/seocheckerlicenseinfoapi/licenseinfo");
            },
            //validation
            initializeValidationForm: function (id) {
                return $http.get("backoffice/seochecker/seocheckervalidateapi/initialize", { params: { id: id } });
            },

            validate: function (validateOptions) {
                return $http.post("backoffice/seochecker/seocheckervalidateapi/validate", validateOptions);
            },
            saveValidation: function (validateOptions) {
                return $http.post("backoffice/seochecker/seocheckervalidateapi/saveValidation", validateOptions);
            },
            // validation queue
            validationqueue: function (model) {
                return $http.post("backoffice/seochecker/seocheckervalidationqueueapi/initialize",model);
            },
            clearValidationqueue: function (model) {
                return $http.post("backoffice/seochecker/seocheckervalidationqueueapi/clearvalidationqueue", model);
            },
            //validation issues
            validationIssues: function (model) {
                return $http.post("backoffice/seochecker/validationissuesapi/initialize",model);
            },
            //validation issues
            ignoredValidationIssues: function (model) {
                return $http.post("backoffice/seochecker/validationissuesapi/initializeignoredissues", model);
                },
            //validation issues
            removeIgnoredValidationIssues: function (model) {
                return $http.post("backoffice/seochecker/validationissuesapi/removefromignorelist", model);
            }
            ,
            //Ignored configuration issues
            ignoredConfigurationIssues: function (model) {
                return $http.post("backoffice/seochecker/configurationissuesapi/initializeignoredissues", model);
            },
            removeIgnoredConfigurationIssues: function (model) {
                return $http.post("backoffice/seochecker/configurationissuesapi/removefromignorelist", model);
            },
            
            //redirect mananer
            loadAllRedirects: function (model) {
                return $http.post("backoffice/seochecker/redirectmanagementapi/initialize", model);
            },
            //ignored inbond links mananer
            loadAllInBoundLinkErrors: function (model) {
                return $http.post("backoffice/seochecker/redirectmanagementapi/initializeinboundlinkerrors", model);
            },
            saveInBoundLinkErrors: function (model) {
                return $http.post("backoffice/seochecker/redirectmanagementapi/saveinboundlinkerrors", model);
            },
            //ignored inbond links mananer
            loadAllIgnoredInBoundLinkErrors: function (model) {
                return $http.post("backoffice/seochecker/redirectmanagementapi/initializeignoredinboundlinkerrors", model);
            },
            //validation issues
            removeIgnoredInboundLinkIssues: function (model) {
                return $http.post("backoffice/seochecker/redirectmanagementapi/removefromignorelist", model);
            },
            
            getContentTypeOfRedirectUrl: function (oldUrl) {
                return $http.get("backoffice/seochecker/redirectmanagementapi/getContentTypeOfRedirectUrl", { params: { redirectUrl: oldUrl }}   );
            },
            //configurationIssues
            configurationIssues: function (model) {
                return $http.post("backoffice/seochecker/configurationissuesapi/initialize", model);
            },
            // My Notifications
            initializeNotifications: function () {
                return $http.get("backoffice/seochecker/seocheckernotificationsapi/initialize");
            },
            saveNotifications: function (notificationsConfig) {
                return $http.post("backoffice/seochecker/seocheckernotificationsapi/save", notificationsConfig);
            },
            //Email settings
            initializeEmailSettings: function (emailTemplate) {
                return $http.get("backoffice/seochecker/emailsettingsapi/initialize", { params: { emailConfig: emailTemplate } });
            },
            saveEmailSettings: function (emailConfig) {
                return $http.post("backoffice/seochecker/emailsettingsapi/save",emailConfig);
            }
            ,
            //Domain settings
            initializeDomainSettings: function (nodeId) {
                return $http.get("backoffice/seochecker/domainsettingsapi/initialize", { params: { rootNodeId: nodeId } });
            },
            saveDomainSettings: function (domainConfig) {
                return $http.post("backoffice/seochecker/domainsettingsapi/save", domainConfig);
            }
              ,
            //Domain settings
            initializeDefinitionSettings: function (alias) {
                return $http.get("backoffice/seochecker/definitionsettingsapi/initialize", { params: { alias: alias } });
            },
            saveDefinitionSettings: function (domainConfig) {
                return $http.post("backoffice/seochecker/definitionsettingsapi/save", domainConfig);
            }
            ,
            //General settings
            initializeGeneralSettings: function (config) {
                return $http.post("backoffice/seochecker/generalsettingsapi/initialize", config);
            },
            saveGeneralSettings: function (config) {
                return $http.post("backoffice/seochecker/generalsettingsapi/save", config);
            },
            //Permission settings
            initializePermissionSettings: function () {
                return $http.get("backoffice/seochecker/permissionsettingsapi/initialize");
            },
            savePermissionSettings: function (config) {
                return $http.post("backoffice/seochecker/permissionsettingsapi/save", config);
            },
            //Property Selector field
            initializePropertySelectorField: function (alias) {
                return $http.get("backoffice/seochecker/propertyselectorfieldapi/initialize", { params: { alias: alias } });
            },

            initializePropertySelectorValue: function (propertyConfig) {
                return $http.post("backoffice/seochecker/propertyselectorfieldapi/InitializeValue", propertyConfig);
            },
            
            selectPropertySelectorFields: function (propertyConfig) {
                return $http.post("backoffice/seochecker/propertyselectorfieldapi/selectitems", propertyConfig);
            },
            deSelectPropertySelectorFields: function (propertyConfig) {
                return $http.post("backoffice/seochecker/propertyselectorfieldapi/deselectitems", propertyConfig);
            }
            ,
            moveUpPropertySelectorFields: function (propertyConfig) {
                return $http.post("backoffice/seochecker/propertyselectorfieldapi/moveup", propertyConfig);
            },
            moveDownPropertySelectorFields: function (propertyConfig) {
                return $http.post("backoffice/seochecker/propertyselectorfieldapi/movedown", propertyConfig);
            },
            //Dialogs
            //ValidationDialog
            initializeDeleteRedirects: function (selectedItems) {
                return $http.post("backoffice/seochecker/deleteredirectsdialogapi/initialize", selectedItems);
            },
            initializeEditRedirect: function (id) {
                return $http.get("backoffice/seochecker/editredirectsdialogapi/initialize", { params: { id: id } });
            },
            saveRedirect: function (model) {
                return $http.post("backoffice/seochecker/editredirectsdialogapi/save", model);
            },
            initializeExportRedirects: function () {
                return $http.get("backoffice/seochecker/exportredirectsdialogapi/initialize");
            },
            initializeImportRedirects: function (model) {
                return $http.post("backoffice/seochecker/importredirectsdialogapi/initialize",model);
            },
            redirectUpdateModel: function (model) {
                return $http.post("backoffice/seochecker/importredirectsdialogapi/update", model);
            },
            uploadFiles: function (formData) {
                return $http.post("backoffice/seochecker/fileuploadapi/upload",
            formData,
            {
                transformRequest: angular.identity,
                headers: {
                    "Content-Type": undefined
                }
            });
            },
           
            importRedirects: function (model) {
                return $http.post("backoffice/seochecker/importredirectsdialogapi/import",model);
            },
            getExportRedirectErrorsUrl: function (model) {
                return "backoffice/seochecker/importredirectsdialogapi/exporterrors?id="+ model.exportErrorId;
            },
            exportRedirects: function (model) {
                return $http.post("backoffice/seochecker/exportredirectsdialogapi/export", model);
            },
            getRedirectExportUrl: function (model) {
                return "backoffice/seochecker/exportredirectsdialogapi/doexport?contentTypeExportOptions=" + model.contentTypesToExport.selectedItem + "&dataExportOptions=" + model.dataToExport.selectedItem;
            },
            bulkDeleteRedirects: function (model) {
                return $http.post("backoffice/seochecker/deleteredirectsdialogapi/delete", model);
            },
            initializeDeleteValidationIssues: function (selectedItems) {
                return $http.post("backoffice/seochecker/deletevalidationissuesdialogapi/initialize", selectedItems);
            },
            bulkDeleteValidationIssues: function (model) {
                return $http.post("backoffice/seochecker/deletevalidationissuesdialogapi/delete", model);
            },
            initializeRevalidateIssues: function (model) {
                return $http.post("backoffice/seochecker/revalidateissuesdialogapi/initialize",model);
            },
            refreshRevalidateIssues: function (model) {
                return $http.post("backoffice/seochecker/revalidateissuesdialogapi/refresh", model,{ timeout :100});
            },
            initializeDeleteConfigurationIssues: function (selectedItems) {
                return $http.post("backoffice/seochecker/deleteconfigurationissuesdialogapi/initialize", selectedItems);
            },
            bulkDeleteConfigurationIssues: function (model) {
                return $http.post("backoffice/seochecker/deleteconfigurationissuesdialogapi/delete", model);
            },
            //Bulk actions on overviews
            clearValidationIssues: function () {
                return $http.post("backoffice/seochecker/validationissuesapi/clearallissues");
            }
            ,
            clearInboundLinkIssues: function () {
                return $http.post("backoffice/seochecker/redirectmanagementapi/clearallissues");
            }
            ,
            clearConfigurationIssues: function () {
                return $http.post("backoffice/seochecker/configurationissuesapi/clearallissues");
            },
            //PropertyEditor prevalues
            getAllProperties: function () {
                return $http.get("backoffice/seochecker/prevalueapi/getallproperties");
            },
            getAllImageProperties: function () {
                return $http.get("backoffice/seochecker/prevalueapi/getallimageproperties");
            },
            //dashboard
            initializeDashboard: function () {
                return $http.get("backoffice/seochecker/seocheckerdashboardapi/initialize");
            },
            processLicenseFile: function (fileInfo) {
                return $http.post("backoffice/seochecker/seocheckerdashboardapi/processlicense", fileInfo);
            } ,
            //General
            deleteTreeNodeById: function (id) {
                return $http.get("backoffice/seochecker/treeapi/deleteTreeNode", { params: { id: id } });
            }
        };
    });