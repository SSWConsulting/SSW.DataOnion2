using System.IO;
using System.Reflection;
using System.Text;
using Serilog;

namespace SSW.DataOnion.CodeGenerator.Helpers
{
    public class ResourceReader
    {
        private static readonly ILogger Logger = Log.ForContext<ResourceReader>();

        public static string GetResourceContents(string resourceName)
        {
            if (!resourceName.Contains("SSW.DataOnion.CodeGenerator.templates."))
            {
                resourceName = $"SSW.DataOnion.CodeGenerator.templates.{resourceName}";
            }

            try
            {
                var templateStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

                if (templateStream != null)
                {
                    using (var reader = new StreamReader(templateStream, Encoding.UTF8))
                    {
                        var text = reader.ReadToEnd();
                        return text;
                    }
                }
            }
            catch (IOException ex)
            {
                Logger.Error(ex, "Failed to read resource with kye '{key}'", resourceName);
                return null;
            }

            return null;
        }
    }
}
