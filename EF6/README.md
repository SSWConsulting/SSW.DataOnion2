## Data Onion Configuration

### If using Autofac DI container read this section

1. Add **SSW.DataOnion.DependencyResolution.Autofac.EF6** nuget package to your UI project:
    
	```sh
    Install-Package SSW.DataOnion.DependencyResolution.Autofac.EF6 –Pre
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
    builder.AddDataOnion(new DbContextConfig(this.ConfigurationManager.ConnectionStrings["YourConnectionString"].ConnectionString, typeof(YourDbContextType), databaseInitializer));
    ```
    _Note: First parameter is just your connection string, adjust it if required._

4. Register repositories for each entity that you are going to use (only if you want to use repository pattern):
    
	```cs
    buider.Register<BaseRepository<YourEntityName, YourDbContextName>>().As<IRepository<YourEntityName>>();
    ```

Congratulations, you are now ready to use DataOnion2!!!

### If using any other DI container read this section

1. Add **SSW.DataOnion.Core.EF6** nuget package to your UI or DependencyResolution project:
    
	```sh
    Install-Package SSW.DataOnion.Core.EF6 –Pre
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

1. Add **SSW.DataOnion.DbContextGenerator.EF6** nuget package to project that will contain your DbContext (i.e YourNamespace.Data) :

    ```sh
    Install-Package SSW.DataOnion.DbContextGenerator.EF6 –Pre
    ```
2. Once package is installed, you will get readme.txt file with further instructions

Now you are ready to run EF migrations.

