using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace MikeFactorial.XTB.PACUI
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "PAC UI"),
        ExportMetadata("Description", "A user interface for exploring and executing Power Platform CLI Commands"),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "/9j/4AAQSkZJRgABAQEAYABgAAD/4QBGRXhpZgAATU0AKgAAAAgABAESAAMAAAABAAEAAFEQAAEAAAABAQAAAFERAAQAAAABAAAAAFESAAQAAAABAAAAAAAAAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAAgACADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD88f2YP2cNa/aRv7fwpp9la2k4tftM1zdTpbosYDuHaQjgFY26ZzgADmvpbwl/wRI+J3xE8YWulaXdeC9Y1S4Eix2n9uxbplRckZYAYRUYjkdPoK85/wCCcuiw+Mvizp+nPbzXn2qbSrRLJAzNeOxmUWwaORWTzdxQuGUp5hPGMV+oPwr/AGOfD3xMs9Ke38H6LqWreMrSPWoNNtl1eXTPAejzSSBLzU7n7XG88oW3uEigjZWndM/Kis4+mr1ORX/T/gnzNGnztr+vyZ8OeO/+Dej46fDfw5Bea9B4a02zkvrazjnfXLeQNPNOkcKFUyfmdlXPRQckgAkfJv7TP7O2sfs4TR+GNQsre5kvdNi1KyubeZZllt5oorhJA+AceVLG2GCsNzAqCDX7BXnwI+Cvi/8AZ01LXNDstNkVrLUda8FfEHw9cauuieM30wTtd6RdWd1cs1pcH7PMpQSHzY4nlhcFWQfmr/wUn0K38K/G3UtPXTZLFrOfV4fsD72bS3Uwo1uXkkZn8riMSMzM+wHnOKmhUc73/K36sdamoWt+d/0RX/4JrXuhj4zeG59evtPtdDutb0S3u5dRu1itlCTMJFMpKjAXDNzlQw6cV+jvxM8eWPhy0vtG8N61cXnhe4+1aDFbP41jiul0mB/9HtZ57G/hS4tfMmuZIPMTzEWeVC8iGvyO/Zv/AGvPGH7M3h64fwprBs/tce6ddi7wSUVgrKwYKdqEqcr8g4zye1+Jf7fXjX4mfYbfxNqWk+ILTT2adRe6e8/2WUgISg83vkqT6Y9qupTc2uxNOahfufp+fineeIdL8O+F77xDodxoetWD2Cte68txZ+DvtYnsZp0t/t4hFxFbSSMHdTtWc4AJJP5zf8FT7/QoP2iPFj+Fb6xvPD9v4h16zsrrTbtLi1EbXCeWPNDMpDqp28ksCSM4Jrkvh7+3f4/+EthdSeHJtB0E6pKiXS6dbmIzOvyxs6iUkhQ/tzuHXmuT/aO/a98WftNeE7VvFmtSXkluhaCIRDdhSQgeRmZto3S4UYUEk4JIImlRcJX6DqVFNI//2Q=="),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class PACUIPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new PACUIPluginControl();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public PACUIPlugin()
        {
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
            // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}