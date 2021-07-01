## First Time Using Template
After creating a solution using this template, you will want to right click on the site project - GitInitTest.Site and select "Open Command Line" then select "Developer Command Prompt".

Next execute the postinstall.cmd file. This file will make sure all the EF packages are loaded and build the migrations for the database. The configuration assumes you have a local SQL Server and your LAN ID has Sysadmin rights on the database. If this is not the case, you will need to edit the appsetings.json and setup the correct database settings. 


Once the post install has completed, you will need to open the "task runner explorer" in visual studio and click the refresh symbol then right click on the "default" task and select run.

Next right click on the GitInitTest.Site and "set as the startup project".

Now you should be able to run the project with the base functionality preconfigured.



## Migrations in Separate Project
To have the data context in a separate project you need to have a DesignTimeDbContextFactory located ApplicationContext class. 
```
dotnet ef --context ApplicationDbContext migrations add <Migration Name>
dotnet ef --context ApplicationDbContext database update
```

You can also run the migrations from the package manager console. 
_Set the default project to GitInitTest.Data in the package manager drop down before running this command._
```
add-migration -Context ApplicationContext -StartupProject GitInitTest.Site CreateDb 

update-database -context ApplicationContext

```



