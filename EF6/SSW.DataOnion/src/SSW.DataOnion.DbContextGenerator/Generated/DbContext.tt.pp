<#@ template debug="true" hostSpecific="true" #>
<#@ output extension=".cs" #>
<#@ Assembly Name="System.Core" #>
<#@ import namespace="System" #>
<#@ import namespace="System.Reflection" #>
<#@ import namespace="System.IO" #>
<#@ import namespace="System.Security" #>
<#@ import namespace="System.Security.Permissions" #>
<#@ import namespace="System.Security.Policy" #>
<#@ import namespace="System.Diagnostics" #>
<#@ import namespace="System.Linq" #>
<#@ import namespace="System.Linq.Expressions" #>
<#@ import namespace="System.Collections" #>
<#@ import namespace="System.Collections.Generic" #>
<#@ include file="Configurations\DbContextConfigurations.ttinclude" #>

using System.Data.Entity;

namespace <#=Configurations.DataProjectNamespace#>
{
	<#
foreach (string entityNamespace in Configurations.DomainModelProjectNamespace.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries))	        
{
#>
using <#=entityNamespace#>;

<# 
}
#>
	public partial class <#=Configurations.DbContextName#>
	{
<#
	var entityTypes = Sandbox.Isolate<AssemblyHelper, ICollection<string>>(helper => helper.GetDomainTypes(Host.ResolvePath(@"..\bin\Debug"))).ToList();

	foreach (var entityType in entityTypes)	        
	{
#>
		public IDbSet<<#=entityType#>> <#=entityType#>s { get; set; }

<#
	}
#>
	}
}

<#+

public partial class AssemblyHelper : MarshalByRefObject 
{
	public ICollection<string> GetDomainTypes(string binFolderPath)
	{
		ICollection<string> entityTypes;
		var entityNamespaces = Configurations.DomainModelProjectNamespace.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList();

	
		try 
		{
			var assembly = Assembly.LoadFrom(Path.Combine(binFolderPath, Configurations.DomainModelProjectDll));

			var func = Configurations.DomainTypeFilter;

		    var queryable = assembly.GetTypes()
				.Where(t => string.IsNullOrEmpty(Configurations.DomainModelProjectNamespace) || entityNamespaces.Any(n => t.Namespace.StartsWith(n)))
				.Where(t => func(t));

		    if (!string.IsNullOrEmpty(Configurations.BaseEntityClass))
			{
				var baseEntityAssembly = Assembly.LoadFrom(Path.Combine(binFolderPath, Configurations.BaseEntityClassDll));
				Type baseType = baseEntityAssembly.GetTypes().First(t => t.Name == Configurations.BaseEntityClass);

				queryable  = queryable.Where(t => baseType == null || (baseType.IsAssignableFrom(t) && t != baseType));
			}

			entityTypes = queryable.Select(t => t.Name).ToList();
			
        }
		catch
		{
			throw;
        }

		return entityTypes;
    }
}
#>

<#+
    public static class Sandbox
    {
        private class DefaultSandboxContainer : MarshalByRefObject, ISandboxContainer
        {
            public T GetInstance<T>()
            {
                return Activator.CreateInstance<T>();
            }

            public T GetNamedInstance<T>(string name)
            {
                return GetInstance<T>();
            }
        }

        private static T WrapObject<T>(AppDomain domain)
        {
            var type = typeof(T);
            var obj = (T)domain.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);

            return obj;
        }

        public static TResult Isolate<T, TResult>(Func<T, TResult> callback)
        {
            return Isolate<DefaultSandboxContainer, T, TResult>(callback, null, null, null, null);
        }

        public static TResult Isolate<TIsolatedContainer, TClass, TResult>(
            Func<TClass, TResult> callback,
            string appDomainName,
            Evidence evidence,
            PermissionSet permissionSet,
            AppDomainSetup setup)
            where TIsolatedContainer : MarshalByRefObject, ISandboxContainer
        {
            if (String.IsNullOrEmpty(appDomainName))
            {
                appDomainName = string.Format("Sandbox_{0}", Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            }

            if (evidence == null)
            {
                evidence = new Evidence();
                evidence.AddHostEvidence(new Zone(SecurityZone.MyComputer));
            }

            var applicationBase = Path.GetDirectoryName(typeof(TClass).Assembly.Location);

            if (permissionSet == null)
            {
                permissionSet = SecurityManager.GetStandardSandbox(evidence) ?? new PermissionSet(PermissionState.None);
                permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, applicationBase));
                permissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));
            }

            if (setup == null)
            {
                setup = new AppDomainSetup { ApplicationBase = applicationBase };
            }

            var domain = AppDomain.CreateDomain(appDomainName, evidence, setup, permissionSet);

            try
            {
                var container = WrapObject<TIsolatedContainer>(domain);
                var instance = container.GetInstance<TClass>();
                return callback(instance);
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }
    }

    public interface ISandboxContainer
    {
        T GetInstance<T>();
        T GetNamedInstance<T>(string name);
    }
#>

