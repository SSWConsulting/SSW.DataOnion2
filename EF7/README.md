## Data Onion Configuration

### If using Microsoft out-of-the box DI container read this section

1. Add **SSW.DataOnion.DependencyResolution.Microsoft** nuget package to your UI project:
    
	```sh
    Install-Package SSW.DataOnion.DependencyResolution.Microsoft –Pre
    ```

2. Open your **Startup.cs** file and go to ConfigureServices method
3. Add the following code:
    
    To create database automatically if it doesn't exist
    
	```cs
    var databaseInitializer = new CreateDatabaseIfNotExists(new SampleDataSeeder());
    ```
    
	To always drop and re-create the database
    
	```cs
    var databaseInitializer = new DropCreateDatabaseAlways(new SampleDataSeeder());
    ```
    
	To use migrations
    
	```cs
    var databaseInitializer = new MigrateToLatestVersion(new SampleDataSeeder());
    ```
    _Note: data seeder parameter is optional. To create data seeder just implements_ **IDataSeeder** _interface._
    
    Now add this line to activate DataOnion:
    
	```cs
    services.AddDataOnion(new DbContextConfig(this.Configuration["Data:DefaultConnection:ConnectionString"], typeof(YourDbContextType), databaseInitializer));
    ```
    _Note: First parameter is just your connection string, adjust it if required._

4. Register repositories for each entity that you are going to use (only if you want to use repository pattern):
    
	```cs
    services.AddTransient<IRepository<YourEntityName>, BaseRepository<YourEntityName, YourDbContextName>>();
    ```

### If using Autofac DI container read this section

1. Add **SSW.DataOnion.DependencyResolution.Autofac** nuget package to your UI project:
    
	```sh
    Install-Package SSW.DataOnion.DependencyResolution.Autofac –Pre
    ```

2. Open your **Startup.cs** file or file used to configure your Autofac contrainer
3. Add the following code:

    To create database automatically if it doesn't exist

    ```cs
    var databaseInitializer = newCreateDatabaseIfNotExists(new SampleDataSeeder());
    ```

    To always drop and re-create the database

    ```cs
    var databaseInitializer = newDropCreateDatabaseAlways(new SampleDataSeeder());
    ```
    
	To use migrations
    
	```cs
    var databaseInitializer = newMigrateToLatestVersion(new SampleDataSeeder());
    ```
    _Note: data seeder parameter is optional. To create data seeder just implements_ **IDataSeeder** _interface._
    
    Now call AddDataOnion extension method on your Autofac container builder:
    
	```cs
    builder.AddDataOnion(new DbContextConfig(this.Configuration["Data:DefaultConnection:ConnectionString"], typeof(YourDbContextType), databaseInitializer));
    ```
    _Note: First parameter is just your connection string, adjust it if required._

4. Register repositories for each entity that you are going to use (only if you want to use repository pattern):
    
	```cs
    buider.Register<BaseRepository<YourEntityName, YourDbContextName>>().As<IRepository<YourEntityName>>();
    ```
### If using any other DI container read this section

1. Add **SSW.DataOnion.Core** nuget package to your UI or DependencyResolution project:
    
	```sh
    Install-Package SSW.DataOnion.Core –Pre
	```

2. Implement **IRepositoryResolver** interface (OPTIONAL - use it only if you are planning to use unit of work pattern)

3. Register the following classes with corresponding interfaces using DI Container of your choice:	

	* DbContextFactory with IDbContextFactory interface and parameter dbContextConfigs
	* DbContextScope with IDbContextScope interface
	* DbContextScopeFactory with IDbContextScopeFactory interface - instance per request is recommended
	* AmbientDbContextLocator with IAmbientDbContextLocator interface
	* DbContextReadOnlyScope with IDbContextReadOnlyScope interface
	
	if you are planning to use unit of work pattern, then the following dependencies must also be registered
	
	* RepositoryLocator with IRepositoryLocator interface
	* YourRepositoryResolver with IAutofacRepositoryResolver interface 
	* UnitOfWorkFactory with IUnitOfWorkFactory interface - instance per request is recommended
	* UnitOfWork with IUnitOfWork interface
	* ReadOnlyUnitOfWork with IReadOnlyUnitOfWork interface

Congratulations, you are now ready to use DataOnion2!!!

## Using Data Onion

### Interactions

Data onion supports two main DbContext interactions:

1. Direct interaction with **DbContext** via **DbContextScope**. To use this type of interaction, just inject **IDbContextScopeFactory** into the class that initiates business transaction.

    When you start business transaction use factory to create **DbContextScope** and resolve **DbContext** for you. At the end of business transaction, call **SaveChanges** or **SaveChangesAsync** on **DbContextScope** instance.
    
    Sample:
    ```cs
    public HomeController(IDbContextScopeFactory dbContextScopeFactory)
    {
        this.dbContextScopeFactory = dbContextScopeFactory;
    }
    
    public async Task<IActionResult> Schools()
    {
        using (var dbContextScope = this.dbContextScopeFactory.CreateReadOnly())
        {
            var schools =
                    await dbContextScope.DbContexts.Get<SchoolDbContext>()
                            .Schools
                            .Include(s => s.Address)
                            .Include(s => s.Students)
                            .ToListAsync();
    ```
    If you need to access the ambient  **DbContext** instances anywhere else (e.g. in a repository class), you can just take a dependency on **IAmbientDbContextLocator** and use it to locate your existing **DbContext**.

2. Indirect interaction with **DbContext** using **UnitOfWork** and **repository pattern**. To use this interaction, take a dependency on **IUnitOfWorkFactory**.

    When you start business transaction use factory to create **UnitOfWork**. At the end of business transaction, call **SaveChanges** or **SaveChangesAsync** on **Unitofwork** instance

    Sample:
    ```cs
    public HomeController(IUnitOfWorkFactory unitOfWorkFactory)
    {
        this.unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<IActionResult> Schools()
    {
        using (var unitOfWork = this.unitOfWorkFactory.CreateReadOnly())
        {
            var schools =
                    await unitOfWork.Repository<School>()
                        .Get()
                        .Include(s => s.Address)
                        .Include(s => s.Students)
                        .ToListAsync();
    ```

### Types of DbContext Scope or UnitOfWork

DataOnion also support two types of DbContextScope or UnitOfWork:

1. Full CRUD - with tracking and caching
2. ReadOnly - optimized for queries only with tracking disabled.

For more details see sample app: [https://github.com/SSWConsulting/SSW.DataOnion2/tree/master/EF7/SSW.DataOnion/sample](https://github.com/SSWConsulting/SSW.DataOnion2/tree/master/EF7/SSW.DataOnion/sample)


## Generating DbContext (Optional)


This step is optional. DbContext partial class will be automatically generated based on your **Domain Entities**. Generated db context will be properly configured for EF Migrations and will contain code to locate and map fluent configurations for entities (EF 7/Core1 is missing automatic assembly locator for entity configuration files).

1. Add **SSW.DataOnion.CodeGeneration** nuget package to project that will contain your DbContext (i.e YourNamespace.Data) :

    ```sh
    Install-Package SSW.DataOnion.CodeGenerator –Pre
    ```
2. Register custom data onion command in your  **project.son**  file:

    ```cs
    "commands": {
        "onion": "SSW.DataOnion.CodeGenerator"
    }
    ```
3. In console, navigate to your data project root folder and type:
    ```cs
    dnx onion —help
    ```
    List of available parameters will be shown.

4. Run the following command to generate your partial DbContext:
    ```cs
    dnx onion --entitiesNamespace "YourEntitiesProjectNamespace" --dataNamespace " YourDataProjectNamespace " --entitiesDll "path to your entities project dll" --name "NameOfYourDbContext" --baseClassName "OptionalEntityBaseClass"
    ```
    For sample project, the following command was used (https://github.com/SSWConsulting/SSW.DataOnion2/tree/master/EF7/SSW.DataOnion/sample):
    ```cs
    dnx onion --entitiesNamespace "SSW.DataOnion.Sample.Entities" --dataNamespace "SSW.DataOnion.Sample.Data" --entitiesDll "..\artifacts\bin\SSW.DataOnion.Sample.Entities\Debug\dnx451\SSW.DataOnion.Sample.Entities.dll" --name "SchoolDbContext" --baseClassName "AggregateRoot"
    ```
    Once this command is run, you should see **NameOfYourDbContext.gen.cs** partial class generated.

5. Create another partial class **NameOfYourDbContext.cs**, this will be where your custom code will go if necessary.
6. Optional. If using EF migrations, add config.json file (you can give it a different name, just make sure you update it in **NameOfYourDbContext.gen.cs file**), which contains connection string for your database. This will be used by EF Migrations to generate migrations.

    Sample config.json:
    ```cs
    {
        "Data": {
            "DefaultConnection": {
                "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=SSW.DataOnion.Sample;Trusted\_Connection=True;MultipleActiveResultSets=true"
            }
        }
    }
    ```
Now you are ready to run EF migrations.

