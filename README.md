# marine-habitat-classification
Marine Habitat Classification subsite


Development
-----------

### Database set up ###
* Set up a local SQL server (we used SSDT for Visual Studio) and create a database named biotope-db
* Run the table creation SQL scripts found under the biotope-db project against the biotope-db database
* Create the umbraco-cms database by restoring it from the backup found under `website\App_Data\MSSQL Backup`

### Local configuration ###
* The file `microservices\microserviceConnectionStrings.config` contains an entity framework connection string
to your biotope-db database, e.g.

`<connectionStrings>
  <remove name="BiotopeDB"/>
  <add name="BiotopeDB" connectionString="metadata=res://*/Models.BiotopeDB.csdl|res://*/Models.BiotopeDB.ssdl|res://*/Models.BiotopeDB.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(localdb)\ProjectsV13;initial catalog=biotope-db;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
</connectionStrings>`

* The file `website\umbracoConnectionStrings.config` contains the connection string to your umbraco-cms database, e.g.

`<connectionStrings>
  <remove name="umbracoDbDSN" />
  <add name="umbracoDbDSN" connectionString="Server=(localdb)\ProjectsV13;Database=umbraco-cms;Integrated Security=true" providerName="System.Data.SqlClient" />
</connectionStrings>`

You should make sure that your local databases are named as such. These settings and others are altered by the deployment process. 

### Local run ###
* Right click the microservices project and set as start up project, then start it in Visual Studio (Ctrl+F5)
* Right click the website project and set as start up project, then start it in Visual Studio (Ctrl+F5)

### Local build (as on build/deployment server) ###
As well as Visual Studio 2017,  I installed https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2017

Open the Developer Command Prompt for VS2017.

    msbuild /p:Configuration=Release /p:RunOctoPack=true /p:OctoPackPackageVersion=0.0.0.0 /p:OctoPackPublishPackageToHttp=http://deployment-srv/nuget/packages /p:OctoPackPublishingApiKey=API-BLAH
