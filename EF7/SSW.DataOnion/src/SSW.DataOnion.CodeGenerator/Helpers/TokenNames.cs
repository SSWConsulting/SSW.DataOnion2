namespace SSW.DataOnion.CodeGenerator.Helpers
{
    public class TokenNames
    {
        public const string DbContextName = "#dbContextName#";
        public const string EntitiesProject = "#entitiesProject#";
        public const string DataProject = "#dataProject#";
        public const string DbSets = "#dbsets#";
        public const string DbSet = "#dbset#";
        public const string Entity = "#entity#";
        public const string DbSetTemplate = "public virtual DbSet<#entity#> #dbset# { get; set; }";
    }
}
