using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using System.Reflection;
using XrmToolBox.Extensibility.Interfaces;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using NuGet.Protocol.Plugins;

namespace MikeFactorial.XTB.PACUI
{
    public partial class PACUIPluginControl : PluginControlBase, IGitHubPlugin, IHelpPlugin
    {
        private string[] unsupportedActions = { "powerfx repl" };
        private Settings mySettings;
        private string pacPath;

        private PacCLINugetFeed nugetFeed = new PacCLINugetFeed();
        private TreeNode currentSelectedNode = null;
        private PacTag currentSelectedTag = null;
        private TreeNode mainNode = null;
        private List<string> existingPacConnections = new List<string>();

        public PACUIPluginControl(PacCLINugetFeed feed) : this()
        {
            nugetFeed = feed;
        }
        public PACUIPluginControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        #region Methods

        public void UpdatePacVersions()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    UpdatePacVersions();
                }));
            }
            else
            {
                toolStripCLIVersionsDropDown.Items.Clear();
                toolStripCLIVersionsDropDown.Items.AddRange(NugetFeed.Versions.Reverse().ToArray());
                if (toolStripCLIVersionsDropDown.SelectedItem == null)
                {
                    toolStripCLIVersionsDropDown.SelectedItem = NugetFeed.Versions.LastOrDefault();
                }
                pacPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Microsoft.PowerApps.CLI.{((NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem).OriginalVersion.ToString()}";
                NugetFeed.NugetVersionLoaded = ((NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem).OriginalVersion;
            }
        }

        private void LoadNouns()
        {
            mainNode = new TreeNode("pac");
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate 
                {
                    this.textBoxCommandText.Text = string.Empty;
                    this.textBoxOutput.Text = string.Empty;
                    this.textBoxParentNodeHelp.Text = string.Empty;
                    this.propertyGrid1.SelectedObject = null;
                    this.treePacCommands.Nodes.Clear();
                    this.treePacCommands.Nodes.Add(mainNode);
                }));
            }
            this.currentSelectedNode = null;
            this.currentSelectedTag = null;
            string results = ExecutePacCommand("help", false);
            mainNode.Tag = new PacTag(ref mainNode)
            {
                Type = PacTag.PacTagType.Root,
                Name = "pac",
                HelpText = results,
                Value = string.Empty
            };
            foreach (string noun in RetrieveUsageDetails(results))
            {
                if (noun != "help")
                {
                    var nounNode = new TreeNode(noun);
                    nounNode.Nodes.Add(new TreeNode());
                    nounNode.Tag = new PacTag(ref nounNode)
                    {
                        Type = PacTag.PacTagType.Noun,
                        Name = noun,
                        HelpText = results,
                        Value = string.Empty
                    };

                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            mainNode.Nodes.Add(nounNode);
                        }));
                    }
                }
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    mainNode.Expand();
                }));
            }
        }

        public virtual async Task InstallSelectedPacVersion()
        {
            NuGetVersion selectedVersion = NugetFeed.Versions.LastOrDefault();

            if (toolStripCLIVersionsDropDown.Control.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate 
                {
                    if (toolStripCLIVersionsDropDown.SelectedItem != null)
                    {
                        selectedVersion = (NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem;
                    }
                }));
            }
            else if(toolStripCLIVersionsDropDown.SelectedItem != null)
            {
                selectedVersion = (NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem;
            }
            if (selectedVersion != null && !Directory.Exists(pacPath))
            {
                using (var packageStream = new MemoryStream())
                {
                    await NugetFeed.Resource.CopyNupkgToStreamAsync(
                        NugetFeed.PackageId,
                        selectedVersion,
                        packageStream,
                        NugetFeed.Cache,
                        NugetFeed.Logger,
                        NugetFeed.CancellationToken);

                    packageStream.Position = 0; // Reset stream position

                    // Define the package and the folder to unpack to
                    PackageIdentity identity = new PackageIdentity(NugetFeed.PackageId, selectedVersion);
                    string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    PackagePathResolver pathResolver = new PackagePathResolver(currentDirectory);

                    var packageExtractionContext = new PackageExtractionContext(
                        PackageSaveMode.Defaultv3,
                        XmlDocFileSaveMode.None,
                        null,
                        NullLogger.Instance);

                    // Unpack the package
                    await PackageExtractor.ExtractPackageAsync(
                        NugetFeed.Repository.PackageSource.Source,
                        packageStream,
                        pathResolver,
                        packageExtractionContext,
                        NugetFeed.CancellationToken);
                }
            }
        }
        private string ExecutePacCommand(string command, bool streamOutputToLabel)
        {
            if (!string.IsNullOrEmpty(PacExecutable))
            {
                //Create process
                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                //strCommand is path and file name of command to run
                pProcess.StartInfo.FileName = PacExecutable;
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.CreateNoWindow = true;
                //Set output of program to be written to process output stream
                pProcess.StartInfo.RedirectStandardOutput = true;
                //Optional
                pProcess.StartInfo.WorkingDirectory = pacPath;
                if (streamOutputToLabel)
                {
                    textBoxOutput.Text = $"Executing command '{command}'";
                    pProcess.ErrorDataReceived += Process_OutputDataReceived;
                    pProcess.OutputDataReceived += Process_OutputDataReceived;
                    //strCommandParameters are parameters to pass to program
                    pProcess.StartInfo.Arguments = command;
                    pProcess.Start();
                    pProcess.BeginOutputReadLine();
                }

                if (streamOutputToLabel)
                {
                    return string.Empty;
                }
                else
                {
                    //strCommandParameters are parameters to pass to program
                    pProcess.StartInfo.Arguments = command;
                    //Start the process
                    pProcess.Start();
                    //Get program output
                    string strOutput = pProcess.StandardOutput.ReadToEnd();
                    //Wait for process to finish
                    pProcess.WaitForExit();
                    pProcess = null;
                    return strOutput;
                }
            }
            return string.Empty;
        }

        private void Process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        Process_OutputDataReceived(sender, e);
                    }));
                }
                else
                {
                    this.textBoxOutput.AppendText(Environment.NewLine + e.Data);
                }
            }
        }

        private string[] RetrieveUsageDetails(string results)
        {
            if (results.Contains("[") && !results.Contains("Error:"))
            {
                string usageDetails = results
                    .Split('\n')
                    .FirstOrDefault(s => s.StartsWith("Usage:"));

                if (!string.IsNullOrEmpty(usageDetails))
                {
                    return usageDetails
                        .Substring(usageDetails.IndexOf("["), (usageDetails.LastIndexOf("]") + 1) - usageDetails.IndexOf("[")).Replace("[", "").Replace("]", "")
                        .Split(' ');
                }
            }
            return new string[] { };
        }

        private string RetrieveNodeHelpText(string results, string nodeText)
        {
            var lines = results.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().StartsWith(nodeText))
                {
                    if (lines.Length > i && lines[i + 1].Trim().StartsWith("Values"))
                    {
                        return lines[i].Trim() + "\n" + lines[i + 1].Trim();
                    }
                    return lines[i];
                }
            }
            return string.Empty;
        }

        private void UpdateCommandText()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    UpdateCommandText();
                }));

            }
            this.textBoxCommandText.Text = $"{this.currentSelectedNode.FullPath}";
            foreach (TreeNode argumentNode in this.currentSelectedNode.Nodes)
            {
                if (argumentNode.Tag != null && ((PacTag)argumentNode.Tag).Value != null && !string.IsNullOrEmpty(((PacTag)argumentNode.Tag).Value.ToString()))
                {
                    this.textBoxCommandText.Text += $" {((PacTag)argumentNode.Tag).Name} \"{((PacTag)argumentNode.Tag).Value}\"";
                }
            }
        }
        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            WorkAsync(new WorkAsyncInfo
            {
                IsCancelable = true,
                Message = $"Authorizing Power Platform CLI...",
                Work = (worker, args) =>
                {
                    SyncPacAuthWithConnection(detail);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorNotification("An error ocurred when trying to set the command line authorization. Try running 'Sync CLI Auth with Current Connection' or run 'pac auth select' from the command line to run commands against your environment.", null);
                        return;
                    }
                }
            });
        }
        private void LoadPacAuthorizationProfiles()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    LoadPacAuthorizationProfiles();
                }));

            }
            else
            {
                string authlist = ExecutePacCommand("auth list", false);
                this.toolStripDropDownButton1.DropDownItems.Clear();
                this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
                {
                    this.syncCLIAuthWithCurrentConnectionToolStripMenuItem,
                    this.toolStripSeparator3
                });

                foreach (var auth in authlist.Split('\n'))
                {
                    if (auth.Trim().StartsWith("["))
                    {
                        //Create connection menu item for this auth
                        ToolStripMenuItem connectionItem = new ToolStripMenuItem();
                        connectionItem.Name = auth;
                        connectionItem.Text = auth;
                        connectionItem.Click += new System.EventHandler(this.syncCLIAuthWithCurrentConnectionToolStripMenuItem_Click);
                        this.toolStripDropDownButton1.DropDownItems.Add(connectionItem);
                    }
                }
            }
        }
        private void SyncPacAuthWithConnection(ConnectionDetail connection)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    SyncPacAuthWithConnection(connection);
                }));
            }
            else
            {
                LoadPacAuthorizationProfiles();
                bool connectionUpdated = false;
                foreach (object auth in this.toolStripDropDownButton1.DropDownItems)
                {
                    if (auth is ToolStripMenuItem && ((ToolStripMenuItem)auth).Name.ToLower().Contains(connection.WebApplicationUrl.ToLower()))
                    {
                        connectionUpdated = SyncPacAuthWithSelected(((ToolStripMenuItem)auth).Name, $"An existing authentication profile was found for the current connection. The authentication profile has been selected to use for future commands.");
                    }
                }

                if (!connectionUpdated && mainNode != null)
                {
                    var authNode = mainNode.Nodes.OfType<TreeNode>().ToList().FirstOrDefault(n => n.Text == "auth");
                    if (authNode != null)
                    {
                        LoadChildNodes(authNode, PacTag.PacTagType.Noun, false);
                        var createNode = authNode.Nodes.OfType<TreeNode>().ToList().FirstOrDefault(n => n.Text == "create");
                        if (createNode != null)
                        {
                            LoadChildNodes(createNode, PacTag.PacTagType.Verb, false);
                            treePacCommands.SelectedNode = createNode;
                            foreach (TreeNode argNode in createNode.Nodes)
                            {
                                if (((PacTag)argNode.Tag).Name == "--name")
                                {
                                    ((PacTag)argNode.Tag).Value = connection.ConnectionName;
                                }
                                else if (((PacTag)argNode.Tag).Name == "--username")
                                {
                                    ((PacTag)argNode.Tag).Value = connection.UserName;
                                }
                                else if (((PacTag)argNode.Tag).Name == "--environment")
                                {
                                    ((PacTag)argNode.Tag).Value = connection.WebApplicationUrl;
                                }
                                /*
                                else if (((PacTag)argNode.Tag).Name == "--applicationId")
                                {
                                    ((PacTag)argNode.Tag).Value = connection.AzureAdAppId;
                                }
                                */
                            }
                        }
                    }
                    ShowInfoNotification("No existing authentication profile could be found for the current connection. Use pac auth create to create a new authentication prfoile. We've prefilled some of the values based on the current connection.", null);
                }
                else
                {
                    LoadPacAuthorizationProfiles();
                }
            }
        }

        private bool SyncPacAuthWithSelected(string auth, string message)
        {
            int authIndex = RetrieveAuthIndex(auth);
            if (authIndex > 0)
            {
                var authSelectResponse = ExecutePacCommand($"auth select --index {authIndex}", false);
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        textBoxOutput.Text = authSelectResponse;
                        ShowInfoNotification(message, null);
                    }));
                }
                else
                {
                    textBoxOutput.Text = authSelectResponse;
                    ShowInfoNotification(message, null);
                }
                return true;
            }
            return false;
        }

        private int RetrieveAuthIndex(string auth)
        {
            int authIndex;
            if (Int32.TryParse(auth.Substring(0, auth.IndexOf("]")).Replace("[", "").Replace("]", ""), out authIndex))
            {
                return authIndex;
            }
            return -1;
        }

        private object GetDefaultArgumentValue(string helpText)
        {

            return string.Empty;
        }

        private void UpdatePropertyGridProperties(TreeNodeCollection nodes)
        {
            var tags = new List<PacTag>();
            foreach (var node in nodes.OfType<TreeNode>().ToList())
            {
                tags.Add((PacTag)node.Tag);
            }
            this.propertyGrid1.SelectedObject = new PacTagListPropertyGridAdapter(tags);

        }

        #endregion

        #region Properties
        public virtual PacCLINugetFeed NugetFeed => this.nugetFeed;
        public TextBox TextBoxCommand => textBoxCommandText;
        public TextBox TextBoxOutput => textBoxOutput;
        public System.Windows.Forms.ToolStripComboBox ToolStripCLIVersionsDropDown => toolStripCLIVersionsDropDown;
        public ToolStripButton ToolStripRunButton => toolStripRunButton;
        /// <summary>
        /// Expands environment variables and, if unqualified, locates the exe in the working directory
        /// or the evironment's path.
        /// </summary>
        /// <param name="exe">The name of the executable file</param>
        /// <returns>The fully-qualified path to the file</returns>
        /// <exception cref="System.IO.FileNotFoundException">Raised when the exe was not found</exception>
        private string PacExecutable
        {
            get
            {
                if (!string.IsNullOrEmpty(pacPath))
                {
                    string[] files = Directory.GetFiles(pacPath, "pac.exe", SearchOption.AllDirectories);
                    return files.FirstOrDefault();
                }
                return string.Empty;
            }
        }

        public string RepositoryName => "MikeFactorial.XTB.Plugins.PACUI";

        public string UserName => "mikefactorial";

        public string HelpUrl => "https://mikefactorial.com/?p=1366";
        #endregion

        #region Events
        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        private async void toolStripCLIVersionsDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NugetFeed.NugetVersionLoaded != ((NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem).OriginalVersion.ToString())
            {
                this.treePacCommands.Nodes.Clear();
                pacPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Microsoft.PowerApps.CLI.{((NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem).OriginalVersion.ToString()}";
                var loadingMessage = $"Loading Power Platform CLI version {((NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem).OriginalVersion.ToString()}...";
                try
                {
                    this.Enabled = false;
                    await InstallSelectedPacVersion();

                    WorkAsync(new WorkAsyncInfo
                    {

                        IsCancelable = false,
                        Message = loadingMessage,
                        Work = (worker, args) =>
                        {
                            LoadNouns();
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new MethodInvoker(delegate
                                {
                                    NugetFeed.NugetVersionLoaded = ((NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem).OriginalVersion;
                                }));
                            }
                        },
                        ProgressChanged = (args) =>
                        {
                            SetWorkingMessage(args.UserState?.ToString());
                        },
                        PostWorkCallBack = (args) =>
                        {
                            if (args.Error != null)
                            {
                                MessageBox.Show(args.Error.Message, "Uh oh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    });
                }
                finally
                {
                    this.Enabled = true;
                }
            }
        }
        protected void MyPluginControl_Load(object sender, EventArgs e)
        {
            WorkAsync(new WorkAsyncInfo
            {
                IsCancelable = true,
                Message = "Getting the latest version of Power Platform CLI...",
                Work = async (worker, args) =>
                {
                    await NugetFeed.Initialize();
                    UpdatePacVersions();
                    SyncPacAuthWithConnection(this.ConnectionDetail);
                },
                ProgressChanged = (args) =>
                {
                    SetWorkingMessage(args.UserState?.ToString());
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message, "Uh oh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            });
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        private void treePacCommands_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.currentSelectedNode = e.Node;
            this.currentSelectedTag = e.Node.Tag as PacTag;

            if(currentSelectedTag != null && currentSelectedTag.Type == PacTag.PacTagType.Verb)
            {
                UpdateCommandText();
            }
            else
            {
                this.textBoxCommandText.Text = "Select an action from the list to build your command";
            }
            if (!e.Node.IsExpanded)
            {
                e.Node.Expand();
            }
            else if (currentSelectedTag.Type == PacTag.PacTagType.Verb)
            {
                UpdatePropertyGridProperties(e.Node.Nodes);
            }
            this.textBoxParentNodeHelp.Text = currentSelectedTag.HelpText;
        }

        private void LoadChildNodes(TreeNode node, PacTag.PacTagType type, bool expand)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    LoadChildNodes(node, type, expand);
                }));
            }
            else
            {
                string command = (type == PacTag.PacTagType.Verb) ? $"{node.Parent.Text} {node.Text} help" : $"{node.Text} help";
                var results = ExecutePacCommand(command, false);
                ((PacTag)node.Tag).HelpText = results.Trim();
                node.Nodes.Clear();
                foreach (var newNode in RetrieveUsageDetails(results))
                {
                    var actionNode = new TreeNode(newNode);
                    actionNode.Tag = new PacTag(ref actionNode)
                    {
                        Type = PacTag.PacTagType.Verb,
                        Name = newNode,
                        Value = string.Empty,
                        HelpText = RetrieveNodeHelpText(results, newNode)

                    };
                    if (type == PacTag.PacTagType.Noun)
                    {
                        actionNode.Nodes.Add(new TreeNode());
                    }
                    node.Nodes.Add(actionNode);
                }
                if (expand)
                {
                    node.Expand();
                }
                if (type == PacTag.PacTagType.Verb)
                {
                    UpdatePropertyGridProperties(node.Nodes);
                }
                if (node.Tag != null)
                {
                    this.textBoxParentNodeHelp.Text = ((PacTag)node.Tag).HelpText;
                }
            }
        }
        private void TreePacCommands_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && string.IsNullOrEmpty(e.Node.Nodes[0].Text) && e.Node.Parent != null && e.Node.Parent.Text == "pac")
            {
                //Load Verbs for the selected Noun
                WorkAsync(new WorkAsyncInfo
                {
                    IsCancelable = true,
                    Message = $"Loading...",
                    Work = (worker, args) =>
                    {
                        LoadChildNodes(e.Node, PacTag.PacTagType.Noun, false);
                    },
                    ProgressChanged = (args) =>
                    {
                        SetWorkingMessage(args.UserState?.ToString());
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            MessageBox.Show(args.Error.Message, "Uh oh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                });
            }
            else if (e.Node.Nodes.Count == 1 && string.IsNullOrEmpty(e.Node.Nodes[0].Text) && e.Node.Parent != null)
            {
                //Load Arguments for the selected Verb
                WorkAsync(new WorkAsyncInfo
                {
                    IsCancelable = true,
                    Message = $"Loading...",
                    Work = (worker, args) =>
                    {
                        LoadChildNodes(e.Node, PacTag.PacTagType.Verb, false);
                    },
                    ProgressChanged = (args) =>
                    {
                        SetWorkingMessage(args.UserState?.ToString());
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            MessageBox.Show(args.Error.Message, "Uh oh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                });
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ((PacTagListPropertyDescriptor)e.ChangedItem.PropertyDescriptor).GetCurrentTreeNode().EndEdit(false);
            UpdateCommandText();
        }

        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            propertyGrid1.DocCommentHeight = 300;
        }

        private void toolStripRunButton_Click(object sender, EventArgs e)
        {
            if (currentSelectedTag.Type == PacTag.PacTagType.Verb && !string.IsNullOrEmpty(textBoxCommandText.Text))
            {
                ExecutePacCommand(textBoxCommandText.Text.Replace("pac ", ""), true);
                if (textBoxCommandText.Text.Contains("pac auth"))
                {
                    LoadPacAuthorizationProfiles();
                }
            }
        }
        private void syncCLIAuthWithCurrentConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WorkAsync(new WorkAsyncInfo
            {
                IsCancelable = true,
                Message = $"Authorizing Power Platform CLI...",
                Work = (worker, args) =>
                {
                    if (((ToolStripMenuItem)sender).Name == syncCLIAuthWithCurrentConnectionToolStripMenuItem.Name)
                    {
                        SyncPacAuthWithConnection(ConnectionDetail);
                    }
                    else
                    {
                        SyncPacAuthWithSelected(((ToolStripMenuItem)sender).Name, $"The authentication profile updated to use for future commands. NOTE: Your command line connection may no longer be in sync with the XrmToolBox connection.");
                    }
                    LoadPacAuthorizationProfiles();
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorNotification("An error ocurred when trying to set the command line authorization. Try running 'Sync CLI Auth with Current Connection' or run 'pac auth select' from the command line to run commands against your environment.", null);
                        return;
                    }
                }
            });
        }
        #endregion

        public void textBoxCommandText_TextChanged(object sender, EventArgs e)
        {
            toolStripRunButton.Enabled = textBoxCommandText.Text.StartsWith("pac ");
        }
    }
}