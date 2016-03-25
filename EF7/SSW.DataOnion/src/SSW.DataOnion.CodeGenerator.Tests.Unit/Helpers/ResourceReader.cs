using System.IO;
using System.Reflection;
using System.Text;
using Serilog;

namespace SSW.DataOnion.CodeGenerator.Tests.Unit.Helpers
{
    public class ResourceReader
    {
        private static readonly ILogger Logger = Log.ForContext<ResourceReader>();

        public static string GetResourceContents(string resourceName)
        {
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
