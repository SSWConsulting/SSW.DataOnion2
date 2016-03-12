using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SSW.DataOnion.CodeGenerator.Exceptions;

namespace SSW.DataOnion.CodeGenerator.Helpers
{
    public static class AssemblyHelper
    {
        public static ICollection<string> GetDomainTypes(
            string entitiesNamespaces,
            string entitiesDllPath,
            string baseEntityClassName = null)
        {
            ICollection<string> entityTypes;
            var entityNamespaces = entitiesNamespaces.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            try
            {
                var assembly = Assembly.LoadFrom(entitiesDllPath);

                var queryable = assembly.GetTypes()
                    .Where(
                        t =>
                            entityNamespaces.Any(n => t.Namespace?.StartsWith(n) ?? true) && 
                            !t.IsAbstract &&
                            !t.IsSealed);

                if (!string.IsNullOrEmpty(baseEntityClassName))
                {
                    var baseType = assembly.GetTypes().FirstOrDefault(t => t.Name == baseEntityClassName);
                    if (baseType == null)
                    {
                        return new List<string>();
                    }

                    queryable = queryable.Where(t => baseType.IsAssignableFrom(t) && t != baseType);
                }

                entityTypes = queryable.Select(t => t.Name).ToList();

            }
            catch
            {
                throw new GenerationException($"Failed to load assembly containing domain entities. Assembly path is '{entitiesDllPath}'");
            }

            return entityTypes;
        }
    }
}