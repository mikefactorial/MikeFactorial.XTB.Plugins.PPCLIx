using Microsoft.VisualStudio.TestTools.UnitTesting;
using MikeFactorial.XTB.PPCLIx;
using Moq;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MikeFactorial.XTB.Plugins.PPCLIx.Tests
{
    [TestClass]
    public class PPCLIxPluginControlTests
    {
        private PPCLIxPluginControl control = null;
        private string json = @"
{
    ""@id"": ""https://api.nuget.org/v3/registration5-semver1/microsoft.powerapps.cli/index.json"",
    ""@type"": [
        ""catalog:CatalogRoot"",
        ""PackageRegistration"",
        ""catalog:Permalink""
    ],
    ""commitId"": ""597afe2c-4712-4b6c-9252-054fd660bedc"",
    ""commitTimeStamp"": ""2024-02-09T18:13:30.7483923+00:00"",
    ""count"": 2,
    ""items"": [
        {
            ""@id"": ""https://api.nuget.org/v3/registration5-semver1/microsoft.powerapps.cli/index.json#page/0.1.34/1.26.5"",
            ""@type"": ""catalog:CatalogPage"",
            ""commitId"": ""597afe2c-4712-4b6c-9252-054fd660bedc"",
            ""commitTimeStamp"": ""2024-02-09T18:13:30.7483923+00:00"",
            ""count"": 64,
            ""items"": [
                {
                    ""@id"": ""https://api.nuget.org/v3/registration5-semver1/microsoft.powerapps.cli/1.30.7.json"",
                    ""@type"": ""Package"",
                    ""commitId"": ""597afe2c-4712-4b6c-9252-054fd660bedc"",
                    ""commitTimeStamp"": ""2024-02-09T18:13:30.7483923+00:00"",
                    ""catalogEntry"": {
                        ""@id"": ""https://api.nuget.org/v3/catalog0/data/2024.02.09.18.12.37/microsoft.powerapps.cli.1.30.7.json"",
                        ""@type"": ""PackageDetails"",
                        ""authors"": ""Microsoft"",
                        ""description"": ""Microsoft PowerPlatform CLI is a simple, single-stop, developer command-line interface (CLI) for developing customizations and extensions for Microsoft PowerPlatform. See project site how to install."",
                        ""iconUrl"": ""https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.cli/1.30.7/icon"",
                        ""id"": ""Microsoft.PowerApps.CLI"",
                        ""language"": ""en-US"",
                        ""licenseExpression"": """",
                        ""licenseUrl"": ""https://www.nuget.org/packages/Microsoft.PowerApps.CLI/1.30.7/license"",
                        ""readmeUrl"": ""https://www.nuget.org/packages/Microsoft.PowerApps.CLI/1.30.7#show-readme-container"",
                        ""listed"": true,
                        ""minClientVersion"": """",
                        ""packageContent"": ""https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.cli/1.30.7/microsoft.powerapps.cli.1.30.7.nupkg"",
                        ""projectUrl"": ""https://learn.microsoft.com/en-us/power-platform/developer/cli/introduction"",
                        ""published"": ""2024-02-09T18:08:10.14+00:00"",
                        ""requireLicenseAcceptance"": true,
                        ""summary"": ""This package contains the Microsoft PowerPlatform CLI."",
                        ""tags"": [
                            ""Dynamics"",
                            ""CRM"",
                            ""PowerApps"",
                            ""PowerPlatform""
                        ],
                        ""title"": ""Microsoft PowerPlatform CLI"",
                        ""version"": ""1.20.7""
                    },
                    ""packageContent"": ""https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.cli/1.30.7/microsoft.powerapps.cli.1.30.7.nupkg"",
                    ""registration"": ""https://api.nuget.org/v3/registration5-semver1/microsoft.powerapps.cli/index.json""
                }
            ],
            ""parent"": ""https://api.nuget.org/v3/registration5-semver1/microsoft.powerapps.cli/index.json"",
            ""lower"": ""1.26.6"",
            ""upper"": ""1.30.7""
        },
        {
            ""@id"": ""https://api.nuget.org/v3/registration5-semver1/microsoft.powerapps.cli/index.json#page/0.1.34/1.26.5"",
            ""@type"": ""catalog:CatalogPage"",
            ""commitId"": ""597afe2c-4712-4b6c-9252-054fd660bedc"",
            ""commitTimeStamp"": ""2024-02-09T18:13:30.7483923+00:00"",
            ""count"": 64,
            ""items"": [
                {
                    ""@id"": ""https://api.nuget.org/v3/registration5-semver1/microsoft.powerapps.cli/1.30.7.json"",
                    ""@type"": ""Package"",
                    ""commitId"": ""597afe2c-4712-4b6c-9252-054fd660bedc"",
                    ""commitTimeStamp"": ""2024-02-09T18:13:30.7483923+00:00"",
                    ""catalogEntry"": {
                        ""@id"": ""https://api.nuget.org/v3/catalog0/data/2024.02.09.18.12.37/microsoft.powerapps.cli.1.30.7.json"",
                        ""@type"": ""PackageDetails"",
                        ""authors"": ""Microsoft"",
                        ""description"": ""Microsoft PowerPlatform CLI is a simple, single-stop, developer command-line interface (CLI) for developing customizations and extensions for Microsoft PowerPlatform. See project site how to install."",
                        ""iconUrl"": ""https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.cli/1.30.7/icon"",
                        ""id"": ""Microsoft.PowerApps.CLI"",
                        ""language"": ""en-US"",
                        ""licenseExpression"": """",
                        ""licenseUrl"": ""https://www.nuget.org/packages/Microsoft.PowerApps.CLI/1.30.7/license"",
                        ""readmeUrl"": ""https://www.nuget.org/packages/Microsoft.PowerApps.CLI/1.30.7#show-readme-container"",
                        ""listed"": true,
                        ""minClientVersion"": """",
                        ""packageContent"": ""https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.cli/1.30.7/microsoft.powerapps.cli.1.30.7.nupkg"",
                        ""projectUrl"": ""https://learn.microsoft.com/en-us/power-platform/developer/cli/introduction"",
                        ""published"": ""2024-02-09T18:08:10.14+00:00"",
                        ""requireLicenseAcceptance"": true,
                        ""summary"": ""This package contains the Microsoft PowerPlatform CLI."",
                        ""tags"": [
                            ""Dynamics"",
                            ""CRM"",
                            ""PowerApps"",
                            ""PowerPlatform""
                        ],
                        ""title"": ""Microsoft PowerPlatform CLI"",
                        ""version"": ""1.30.7""
                    },
                    ""packageContent"": ""https://api.nuget.org/v3-flatcontainer/microsoft.powerapps.cli/1.30.7/microsoft.powerapps.cli.1.30.7.nupkg"",
                    ""registration"": ""https://api.nuget.org/v3/registration5-semver1/microsoft.powerapps.cli/index.json""
                }
            ],
            ""parent"": ""https://api.nuget.org/v3/registration5-semver1/microsoft.powerapps.cli/index.json"",
            ""lower"": ""1.26.6"",
            ""upper"": ""1.30.7""
        }

    ],
    ""@context"": {
        ""@vocab"": ""http://schema.nuget.org/schema#"",
        ""catalog"": ""http://schema.nuget.org/catalog#"",
        ""xsd"": ""http://www.w3.org/2001/XMLSchema#"",
        ""items"": {
            ""@id"": ""catalog:item"",
            ""@container"": ""@set""
        },
        ""commitTimeStamp"": {
            ""@id"": ""catalog:commitTimeStamp"",
            ""@type"": ""xsd:dateTime""
        },
        ""commitId"": {
            ""@id"": ""catalog:commitId""
        },
        ""count"": {
            ""@id"": ""catalog:count""
        },
        ""parent"": {
            ""@id"": ""catalog:parent"",
            ""@type"": ""@id""
        },
        ""tags"": {
            ""@id"": ""tag"",
            ""@container"": ""@set""
        },
        ""reasons"": {
            ""@container"": ""@set""
        },
        ""packageTargetFrameworks"": {
            ""@id"": ""packageTargetFramework"",
            ""@container"": ""@set""
        },
        ""dependencyGroups"": {
            ""@id"": ""dependencyGroup"",
            ""@container"": ""@set""
        },
        ""dependencies"": {
            ""@id"": ""dependency"",
            ""@container"": ""@set""
        },
        ""packageContent"": {
            ""@type"": ""@id""
        },
        ""published"": {
            ""@type"": ""xsd:dateTime""
        },
        ""registration"": {
            ""@type"": ""@id""
        }
    }
}
";
        [TestInitialize]
        public void Init()
        {
            Form form = new Form();
            Mock<PacCLINugetFeed> nugetFeed = new Mock<PacCLINugetFeed>() { CallBase = true, };
            nugetFeed.Setup(c => c.DownloadPackageInfoStream())
                .Returns(() =>
                {
                    MemoryStream ms = new MemoryStream();
                    var sw = new StreamWriter(ms);
                    sw.Write(json);
                    sw.Flush();//otherwise you are risking empty stream
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                });

            nugetFeed.Setup(c => c.DownloadPackageStream(It.IsAny<string>()))
                .Returns((string version) =>
                {
                    using (FileStream file = new FileStream("../../Nuget/microsoft.powerapps.cli.1.30.7.nupkg", FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        MemoryStream stream = new MemoryStream();
                        stream.Write(bytes, 0, (int)file.Length);
                        return stream;
                    }
                });
            control = new PPCLIxPluginControl(nugetFeed.Object);
            form.Controls.Add(control);
        }
        [TestMethod]
        public void CheckRunEnabled()
        {
            Assert.IsFalse(control.ToolStripRunButton.Enabled);
            control.TextBoxCommand.Text = "Test";
            control.textBoxCommandText_TextChanged(control.TextBoxCommand, null);
            Assert.IsFalse(control.ToolStripRunButton.Enabled);
            control.TextBoxCommand.Text = "pac ";
            control.textBoxCommandText_TextChanged(control.TextBoxCommand, null);
            Assert.IsTrue(control.ToolStripRunButton.Enabled);

        }
        [TestMethod]
        public void LoadVersionsTest()
        {
            control.NugetFeed.Initialize();

            control.TreePacCommands.BeforeExpand -= control.TreePacCommands_BeforeExpand;
            control.TreePacCommands.AfterSelect -= control.TreePacCommands_AfterSelect;
            control.ToolStripCLIVersionsDropDown.SelectedIndexChanged -= control.ToolStripCLIVersionsDropDown_SelectedIndexChanged;

            control.UpdatePacVersions();
            NuGetPackageInfo packageInfo = JsonConvert.DeserializeObject<NuGetPackageInfo>(json);

            Assert.AreEqual(packageInfo.items.Count, control.ToolStripCLIVersionsDropDown.Items.Count);

            control.TreePacCommands.BeforeExpand += control.TreePacCommands_BeforeExpand;
            control.TreePacCommands.AfterSelect += control.TreePacCommands_AfterSelect;
            control.ToolStripCLIVersionsDropDown.SelectedIndexChanged += control.ToolStripCLIVersionsDropDown_SelectedIndexChanged;

        }

        [TestMethod]
        public void InstallSelectedPacVersionTest()
        {
            control.NugetFeed.Initialize();

            control.TreePacCommands.BeforeExpand -= control.TreePacCommands_BeforeExpand;
            control.TreePacCommands.AfterSelect -= control.TreePacCommands_AfterSelect;
            control.ToolStripCLIVersionsDropDown.SelectedIndexChanged -= control.ToolStripCLIVersionsDropDown_SelectedIndexChanged;

            if (Directory.Exists("./microsoft.powerapps.cli.1.30.7"))
            {
                Directory.Delete("./microsoft.powerapps.cli.1.30.7", true);
            }
            control.UpdatePacVersions();
            control.InstallSelectedPacVersion(control.NugetFeed.Versions.FirstOrDefault(v => v == "1.30.7"));
            Assert.IsTrue(Directory.Exists("./microsoft.powerapps.cli.1.30.7"));

            control.TreePacCommands.BeforeExpand += control.TreePacCommands_BeforeExpand;
            control.TreePacCommands.AfterSelect += control.TreePacCommands_AfterSelect;
            control.ToolStripCLIVersionsDropDown.SelectedIndexChanged += control.ToolStripCLIVersionsDropDown_SelectedIndexChanged;
        }
    }
}
