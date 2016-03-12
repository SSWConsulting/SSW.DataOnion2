using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SSW.DataOnion.CodeGenerator.Exceptions;

namespace SSW.DataOnion.CodeGenerator.Helpers
{
    public class DbContextGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContextName">Name of the DbContext class</param>
        /// <param name="entitiesNamespaces">Namespace used for entities</param>
        /// <param name="dataProjectNamespace">Namespace used for project where DbContext resides</param>
        /// <param name="entitiesDllPath">Path to entities dll</param>
        /// <param name="baseEntityClassName">Optional base class used by all entities</param>
        public void Generate(
            string dbContextName, 
            string entitiesNamespaces, 
            string dataProjectNamespace,
            string entitiesDllPath,
            string baseEntityClassName = null)
        {
            Guard.AgainstNullOrEmptyString(dbContextName, nameof(dbContextName));
            Guard.AgainstNullOrEmptyString(entitiesNamespaces, nameof(entitiesNamespaces));
            Guard.AgainstNullOrEmptyString(dataProjectNamespace, nameof(dataProjectNamespace));
            Guard.AgainstNullOrEmptyString(entitiesDllPath, nameof(entitiesDllPath));

            var entityTypes = AssemblyHelper.GetDomainTypes(entitiesNamespaces, entitiesDllPath, baseEntityClassName);
            var template = ResourceReader.GetResourceContents("DbContext.template");

            var dbSets = GetDbSets(entityTypes);

            // replace all tokens
            template =
                template.Replace(TokenNames.DataProject, dataProjectNamespace)
                    .Replace(TokenNames.DbContextName, dbContextName)
                    .Replace(TokenNames.EntitiesProject,
                        string.Join(Environment.NewLine + "\t\t", entitiesNamespaces.Split(',').Select(v => $"using {v};")))
                    .Replace(TokenNames.DbSets,
                        string.Join(Environment.NewLine, dbSets));
            // create file
            File.WriteAllText($"{dbContextName}.cs", template);
        }

        private static IEnumerable<string> GetDbSets(ICollection<string> entityTypes)
        {
            var dbSets =
                entityTypes.Select(
                    e =>
                        $"{TokenNames.DbSetTemplate.Replace(TokenNames.Entity, e).Replace(TokenNames.DbSet, e.Pluralize())}");
            return dbSets;
        }
    }
}
