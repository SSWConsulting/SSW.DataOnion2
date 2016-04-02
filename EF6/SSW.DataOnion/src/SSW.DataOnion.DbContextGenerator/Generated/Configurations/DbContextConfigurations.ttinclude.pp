<#+
public static partial class Configurations
{
	// OPTIONAL BASE TYPE FILTER - LEAVE BLANK IF NOT USED
	public const string BaseEntityClass = @""; // optional base entity to filter only entities that inherit from specific base class, i.e. "BaseEntity"
	public const string BaseEntityClassDll = @""; // optional base entity dll to filter only entities that inherit from specific base class, i.e. "SSW.Data.Entities.dll"
	///////////////////////////////////////////
	
	public const string DomainModelProjectDll = @"$rootnamespace$.DomainModel.dll"; // dll file name for domain models
	public const string DomainModelProjectNamespace = @"$rootnamespace$.DomainModel.Entities"; // custom domain model namespace, usualy the same as dll name. Accepts comma-separated list
	public const string DbContextName = @"YourDbContext"; // name of your db context
	public const string DataProjectNamespace = @"$rootnamespace$.Data"; // namespace to be used by generated db context

	/////////////////////////////////////////////////////////////////////
	// Use this only if you want to apply custom domain class filtering. 
	// Most of the time you will not need to change this
	/////////////////////////////////////////////////////////////////////
	public static Func<Type, bool> DomainTypeFilter 
	{ 
		get 
		{ 
			Func<Type, bool> func = 
				(t) => 
					!t.IsAbstract && !t.IsSealed; // <--- change this line to specify your own custom Domain Class filters, i.e. you may want to exclude all classes that start with "ABC"
			return func;
		} 
	}
}
#>

