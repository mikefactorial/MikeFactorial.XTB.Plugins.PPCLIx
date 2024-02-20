﻿using McTools.Xrm.Connection;
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
        private PacCLINugetFeed nugetFeed = new PacCLINugetFeed();
        private Settings mySettings;
        private string pacPath;

        private TreeNode currentSelectedNode = null;
        private PacTag currentSelectedTag = null;
        private TreeNode mainNode = null;

        public PACUIPluginControl(PacCLINugetFeed feed) : this()
        {
            nugetFeed = feed;
        }
        public PACUIPluginControl()
        {
            InitializeComponent();
        }


        #region Properties
        public ToolStripComboBox ToolStripCLIVersionsDropDown => toolStripCLIVersionsDropDown;
        public TreeView TreePacCommands => treePacCommands;
        public string RepositoryName => "MikeFactorial.XTB.Plugins.PACUI";
        public string UserName => "mikefactorial";
        public string HelpUrl => "https://mikefactorial.com/?p=1366";
        public virtual PacCLINugetFeed NugetFeed => this.nugetFeed;
        public TextBox TextBoxCommand => textBoxCommandText;
        public TextBox TextBoxOutput => textBoxOutput;
        public ToolStripButton ToolStripRunButton => toolStripRunButton;
        public static bool ControlInitialized { get; set; }
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
        #endregion

        #region Methods
        /// <summary>
        /// Loads the PAC versions retrieved from NuGet into th edropdown menu
        /// </summary>
        public void UpdatePacVersions()
        {
            toolStripCLIVersionsDropDown.Items.Clear();
            toolStripCLIVersionsDropDown.Items.AddRange(NugetFeed.Versions.Reverse().ToArray());
            if (toolStripCLIVersionsDropDown.SelectedItem == null)
            {
                toolStripCLIVersionsDropDown.SelectedItem = NugetFeed.Versions.LastOrDefault();
            }
        }
        /// <summary>
        /// Executes a pac command using the command line as a background process and capture the output
        /// </summary>
        /// <param name="command">The command to run</param>
        /// <param name="streamOutputToLabel">True/False depending on if the results should be piped to the output textbox or returned to the caller.</param>
        /// <returns>The output of the command unless it is piped to the output textbox; otherwise empty string</returns>
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
        /// <summary>
        /// Streams the pac output to the output text box
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
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

        /// <summary>
        /// Set the pac command based on the current tree selections and properties
        /// </summary>
        private void UpdateCommandText()
        {
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
            if (ControlInitialized)
            {
                SyncPacAuthWithConnection(detail);
            }
        }
        /// <summary>
        /// Update PAC Auth based on the supplied XrmToolBox connection
        /// </summary>
        /// <param name="connection">The connection to try and sync with the PAC Auth</param>
        private void SyncPacAuthWithConnection(ConnectionDetail connection)
        {
            LoadPacAuthorizationProfiles();
            bool connectionUpdated = false;
            foreach (object auth in this.toolStripDropDownButton1.DropDownItems)
            {
                if (auth is ToolStripMenuItem && ((ToolStripMenuItem)auth).Name.ToLower().Contains(connection.WebApplicationUrl.ToLower()))
                {
                    connectionUpdated = SyncPacAuthWithSelectedWorker(((ToolStripMenuItem)auth).Name, $"An existing authentication profile was found for the current connection. The authentication profile has been selected to use for future commands.");
                }
            }

            if (!connectionUpdated && mainNode != null)
            {
                var authNode = mainNode.Nodes.OfType<TreeNode>().ToList().FirstOrDefault(n => n.Text == "auth");
                if (authNode != null)
                {
                    LoadChildNodesWorker(authNode, PacTag.PacTagType.Noun, false);
                    var createNode = authNode.Nodes.OfType<TreeNode>().ToList().FirstOrDefault(n => n.Text == "create");
                    if (createNode != null)
                    {
                        LoadChildNodesWorker(createNode, PacTag.PacTagType.Verb, false);
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

        /// <summary>
        /// Set the argument properties based on the argument nodes in the tree for the selected verb
        /// </summary>
        /// <param name="nodes"></param>
        private void UpdatePropertyGridProperties(TreeNodeCollection nodes)
        {
            var tags = new List<PacTag>();
            foreach (var node in nodes.OfType<TreeNode>().ToList())
            {
                tags.Add((PacTag)node.Tag);
            }
            this.propertyGrid1.SelectedObject = new PacTagListPropertyGridAdapter(tags);
        }
        /// <summary>
        /// Installs the pac version selected from the menu if it hasn't already been installed
        /// </summary>
        public async Task InstallSelectedPacVersion(NuGetVersion selectedVersion)
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
        /// <summary>
        /// Load the default and pac authorization profiles into the auth menu
        /// </summary>
        private void LoadPacAuthorizationProfiles()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    this.toolStripDropDownButton1.DropDownItems.Clear();
                    this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
                    {
                        this.syncCLIAuthWithCurrentConnectionToolStripMenuItem,
                        this.toolStripSeparator3
                    });
                }));
            }
            else
            {
                this.toolStripDropDownButton1.DropDownItems.Clear();
                this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
                {
                        this.syncCLIAuthWithCurrentConnectionToolStripMenuItem,
                        this.toolStripSeparator3
                });
            }
            string authlist = ExecutePacCommand("auth list", false);

            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
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
                }));
            }
            else
            {
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
        #endregion

        #region Worker Methods
        /// <summary>
        /// JIT loads child nodes when the user selects or expands a node
        /// </summary>
        /// <param name="node">The node that was selected or expanded</param>
        /// <param name="type">The type of node that was selected or expanded</param>
        /// <param name="expand">True/False whether to expand the node after it's loaded or not</param>
        private void LoadChildNodesWorker(TreeNode node, PacTag.PacTagType type, bool expand)
        {
            string command = (type == PacTag.PacTagType.Verb) ? $"{node.Parent.Text} {node.Text} help" : $"{node.Text} help";
            string results = string.Empty;
            WorkAsync(new WorkAsyncInfo
            {
                IsCancelable = true,
                Message = $"Loading Power Platform Commands...",
                Work = (worker, args) =>
                {
                    results = ExecutePacCommand(command, false);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorNotification("An error ocurred when trying to set the command line authorization. Try running 'Sync CLI Auth with Current Connection' or run 'pac auth select' from the command line to run commands against your environment.", null);
                        return;
                    }
                    else
                    {
                        ((PacTag)node.Tag).HelpText = results.Trim();
                        node.Nodes.Clear();
                        foreach (var newNodeText in PacCommands.RetrieveUsageDetails(results))
                        {
                            var actionNode = new TreeNode(newNodeText);
                            var helpText = PacCommands.RetrieveNodeHelpText(results, newNodeText);
                            actionNode.Tag = new PacTag(ref actionNode)
                            {
                                Type = PacTag.PacTagType.Verb,
                                Name = newNodeText,
                                Value = PacCommands.GetDefaultArgumentValue(helpText),
                                HelpText = helpText

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
            });
        }

        /// <summary>
        /// Updates the pac auth to the selected auth from the menu
        /// </summary>
        /// <param name="auth">The auth information returned by pac</param>
        /// <param name="message">The message to display in the async process</param>
        /// <returns></returns>
        private bool SyncPacAuthWithSelectedWorker(string auth, string message)
        {
            int authIndex = PacCommands.RetrieveAuthIndex(auth);
            bool success = false;
            if (authIndex > 0)
            {
                var authSelectResponse = string.Empty;

                WorkAsync(new WorkAsyncInfo
                {
                    IsCancelable = true,
                    Message = $"Authorizing Power Platform CLI...",
                    Work = (worker, args) =>
                    {
                        authSelectResponse = ExecutePacCommand($"auth select --index {authIndex}", false);
                        LoadPacAuthorizationProfiles();
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            ShowErrorNotification("An error ocurred when trying to set the command line authorization. Try running 'Sync CLI Auth with Current Connection' or run 'pac auth select' from the command line to run commands against your environment.", null);
                            return;
                        }
                        else
                        {
                            this.textBoxOutput.Text = authSelectResponse;
                            this.ShowInfoNotification(message, null);
                            success = true;
                        }
                    }
                });
            }
            return success;
        }

        /// <summary>
        /// Installs the pac version selected from the menu if it hasn't already been installed
        /// </summary>
        public virtual async Task InstallSelectedPacVersionWorker(bool syncConnection)
        {
            NuGetVersion selectedVersion = NugetFeed.Versions.LastOrDefault();

            if (toolStripCLIVersionsDropDown.SelectedItem != null)
            {
                selectedVersion = (NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem;
            }
            if (selectedVersion != null)
            {
                this.currentSelectedNode = null;
                this.currentSelectedTag = null;
                this.textBoxCommandText.Text = string.Empty;
                this.textBoxOutput.Text = string.Empty;
                this.textBoxParentNodeHelp.Text = string.Empty;
                this.propertyGrid1.SelectedObject = null;
                this.mainNode = new TreeNode("pac");
                this.treePacCommands.Nodes.Clear();
                this.treePacCommands.Nodes.Add(mainNode);
                this.pacPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Microsoft.PowerApps.CLI.{((NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem).OriginalVersion.ToString()}";

                this.Enabled = false;
                try
                {
                    await this.InstallSelectedPacVersion(selectedVersion);
                }
                finally
                {
                    this.Enabled = true;
                }
                string results = string.Empty;

                WorkAsync(new WorkAsyncInfo
                {
                    IsCancelable = true,
                    Message = $"Loading Power Platform Commands...",
                    Work = (worker, args) =>
                    {
                        results = ExecutePacCommand("help", false);
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            ShowErrorNotification("An error ocurred when trying to set the command line authorization. Try running 'Sync CLI Auth with Current Connection' or run 'pac auth select' from the command line to run commands against your environment.", null);
                            return;
                        }
                        else
                        {
                            this.NugetFeed.NugetVersionLoaded = ((NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem).OriginalVersion;
                            mainNode.Tag = new PacTag(ref mainNode)
                            {
                                Type = PacTag.PacTagType.Root,
                                Name = "pac",
                                HelpText = results,
                                Value = string.Empty
                            };
                            //Load the nouns
                            foreach (string noun in PacCommands.RetrieveUsageDetails(results))
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

                                    mainNode.Nodes.Add(nounNode);
                                }
                            }
                            mainNode.Expand();
                            if (this.ConnectionDetail != null && syncConnection)
                            {
                                SyncPacAuthWithConnection(this.ConnectionDetail);
                            }
                        }
                    }
                });
            }
        }
        #endregion

        #region Events Handlers
        /// <summary>
        /// Handles the initial loading of the control
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        protected async void MyPluginControl_Load(object sender, EventArgs e)
        {
            ControlInitialized = false;
            //Disable event handlers on the initial load to avoid thread contention issues
            this.treePacCommands.BeforeExpand -= this.TreePacCommands_BeforeExpand;
            this.treePacCommands.AfterSelect -= this.TreePacCommands_AfterSelect;
            this.toolStripCLIVersionsDropDown.SelectedIndexChanged -= ToolStripCLIVersionsDropDown_SelectedIndexChanged;
            try
            {
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

                await NugetFeed.Initialize();
                this.UpdatePacVersions();
                await this.InstallSelectedPacVersionWorker(true);
            }
            catch (Exception ex)
            {
                ShowErrorNotification($"An unexpected error occurred while loading the tool. Please try again or file an issue with the details of the error and the command you're trying to run.\n{ex.ToString()}", new Uri("https://github.com/mikefactorial/MikeFactorial.XTB.Plugins.PACUI/issues"), 32);
            }
            finally
            {
                this.treePacCommands.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreePacCommands_BeforeExpand);
                this.treePacCommands.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreePacCommands_AfterSelect);
                this.toolStripCLIVersionsDropDown.SelectedIndexChanged += ToolStripCLIVersionsDropDown_SelectedIndexChanged;
                ControlInitialized = true;
            }
        }
        /// <summary>
        /// Handles closing the control and saving settings
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }
        /// <summary>
        /// Handles updating the command text when a new node is selected on the tree
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        public async void ToolStripCLIVersionsDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            { 
                if (NugetFeed.NugetVersionLoaded != ((NuGetVersion)toolStripCLIVersionsDropDown.SelectedItem).OriginalVersion.ToString())
                {
                    await this.InstallSelectedPacVersionWorker(false);
                }
            }
            catch (Exception ex)
            {
                ShowErrorNotification($"An error occurred while loading the selected CLI version. Please try again or file an issue with the details of the error and the command you're trying to run.\n{ex.ToString()}", new Uri("https://github.com/mikefactorial/MikeFactorial.XTB.Plugins.PACUI/issues"), 32);
            }
        }
        /// <summary>
        /// Handles updating the command text when a new node is selected on the tree
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        public void TreePacCommands_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                ShowErrorNotification($"An error occurred while loading the commands. Please try again or file an issue with the details of the error and the command you're trying to run.\n{ex.ToString()}", new Uri("https://github.com/mikefactorial/MikeFactorial.XTB.Plugins.PACUI/issues"), 32);
            }
        }

        /// <summary>
        /// Loads the child nodes when the user clicks to expand a node if the child nodes haven't been loaded yet
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        public void TreePacCommands_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                if (e.Node.Nodes.Count == 1 && string.IsNullOrEmpty(e.Node.Nodes[0].Text) && e.Node.Parent != null && e.Node.Parent.Text == "pac")
                {
                    LoadChildNodesWorker(e.Node, PacTag.PacTagType.Noun, false);
                }
                else if (e.Node.Nodes.Count == 1 && string.IsNullOrEmpty(e.Node.Nodes[0].Text) && e.Node.Parent != null)
                {
                    LoadChildNodesWorker(e.Node, PacTag.PacTagType.Verb, false);
                }
            }
            catch (Exception ex)
            {
                ShowErrorNotification($"An error occurred while loading the commands. Please try again or file an issue with the details of the error and the command you're trying to run.\n{ex.ToString()}", new Uri("https://github.com/mikefactorial/MikeFactorial.XTB.Plugins.PACUI/issues"), 32);
            }
        }
        /// <summary>
        /// Updates the command text when an argument value changes to include the arugment value
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateCommandText();
        }
        /// <summary>
        /// Updates the help text section of the property grid
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            propertyGrid1.DocCommentHeight = 300;
        }
        /// <summary>
        /// Runs the command in the command text box
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        private void toolStripRunButton_Click(object sender, EventArgs e)
        {
            toolStripRunButton.Enabled = false;
            try
            {
                if (textBoxCommandText.Text.StartsWith("pac "))
                {
                    ExecutePacCommand(textBoxCommandText.Text.Replace("pac ", ""), true);
                }
            }
            catch (Exception ex)
            {
                ShowErrorNotification($"An error occurred while running the command. Please try again or file an issue with the details of the error and the command you're trying to run.\n{ex.ToString()}", new Uri("https://github.com/mikefactorial/MikeFactorial.XTB.Plugins.PACUI/issues"), 32);
            }
            finally
            {
                toolStripRunButton.Enabled = true;
            }
        }
        /// <summary>
        /// Syncs the pac auth profile with the XrmToolBox connection
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        private void syncCLIAuthWithCurrentConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Name == syncCLIAuthWithCurrentConnectionToolStripMenuItem.Name)
            {
                SyncPacAuthWithConnection(ConnectionDetail);
            }
            else
            {
                SyncPacAuthWithSelectedWorker(((ToolStripMenuItem)sender).Name, $"The authentication profile updated to use for future commands. NOTE: Your command line connection may no longer be in sync with the XrmToolBox connection.");
            }
        }
        /// <summary>
        /// Disable / Enable run button based on the command text
        /// </summary>
        /// <param name="sender">Sender Control</param>
        /// <param name="e">Event Arguments</param>
        public void textBoxCommandText_TextChanged(object sender, EventArgs e)
        {
            toolStripRunButton.Enabled = textBoxCommandText.Text.StartsWith("pac ");
        }
        #endregion
    }
}