using System.Windows.Forms;

namespace MikeFactorial.XTB.PACUI
{
    partial class PACUIPluginControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        public void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PACUIPluginControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.toolStripPacVersionLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripCLIVersionsDropDown = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.syncCLIAuthWithCurrentConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripRunButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.groupCommandArgs = new System.Windows.Forms.GroupBox();
            this.groupTree = new System.Windows.Forms.GroupBox();
            this.treePacCommands = new System.Windows.Forms.TreeView();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupCommandArea = new System.Windows.Forms.GroupBox();
            this.textBoxCommandText = new System.Windows.Forms.TextBox();
            this.groupBoxHelp = new System.Windows.Forms.GroupBox();
            this.textBoxParentNodeHelp = new System.Windows.Forms.TextBox();
            this.groupBoxOutput = new System.Windows.Forms.GroupBox();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.propertyGrid1 = new MikeFactorial.XTB.PACUI.PropertyGridEx();
            this.toolStripMenu.SuspendLayout();
            this.groupCommandArgs.SuspendLayout();
            this.groupTree.SuspendLayout();
            this.groupCommandArea.SuspendLayout();
            this.groupBoxHelp.SuspendLayout();
            this.groupBoxOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPacVersionLabel,
            this.toolStripCLIVersionsDropDown,
            this.toolStripSeparator1,
            this.toolStripDropDownButton1,
            this.toolStripSeparator,
            this.toolStripRunButton,
            this.toolStripSeparator2});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(989, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // toolStripPacVersionLabel
            // 
            this.toolStripPacVersionLabel.Name = "toolStripPacVersionLabel";
            this.toolStripPacVersionLabel.Size = new System.Drawing.Size(68, 28);
            this.toolStripPacVersionLabel.Text = "CLI Version:";
            // 
            // toolStripCLIVersionsDropDown
            // 
            this.toolStripCLIVersionsDropDown.Name = "toolStripCLIVersionsDropDown";
            this.toolStripCLIVersionsDropDown.Size = new System.Drawing.Size(121, 31);
            this.toolStripCLIVersionsDropDown.SelectedIndexChanged += new System.EventHandler(this.toolStripCLIVersionsDropDown_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.syncCLIAuthWithCurrentConnectionToolStripMenuItem,
            this.toolStripSeparator3});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(37, 28);
            this.toolStripDropDownButton1.Text = "Update CLI Connection";
            // 
            // syncCLIAuthWithCurrentConnectionToolStripMenuItem
            // 
            this.syncCLIAuthWithCurrentConnectionToolStripMenuItem.Name = "syncCLIAuthWithCurrentConnectionToolStripMenuItem";
            this.syncCLIAuthWithCurrentConnectionToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.syncCLIAuthWithCurrentConnectionToolStripMenuItem.Text = "Sync CLI Auth with Current Connection";
            this.syncCLIAuthWithCurrentConnectionToolStripMenuItem.Click += new System.EventHandler(this.syncCLIAuthWithCurrentConnectionToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(279, 6);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripRunButton
            // 
            this.toolStripRunButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripRunButton.Enabled = false;
            this.toolStripRunButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripRunButton.Image")));
            this.toolStripRunButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRunButton.Name = "toolStripRunButton";
            this.toolStripRunButton.Size = new System.Drawing.Size(28, 28);
            this.toolStripRunButton.Text = "Run the Command in the Command Text Box";
            this.toolStripRunButton.Click += new System.EventHandler(this.toolStripRunButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // groupCommandArgs
            // 
            this.groupCommandArgs.Controls.Add(this.propertyGrid1);
            this.groupCommandArgs.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupCommandArgs.Location = new System.Drawing.Point(669, 31);
            this.groupCommandArgs.Name = "groupCommandArgs";
            this.groupCommandArgs.Size = new System.Drawing.Size(320, 478);
            this.groupCommandArgs.TabIndex = 14;
            this.groupCommandArgs.TabStop = false;
            // 
            // groupTree
            // 
            this.groupTree.Controls.Add(this.treePacCommands);
            this.groupTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupTree.Location = new System.Drawing.Point(0, 31);
            this.groupTree.Name = "groupTree";
            this.groupTree.Size = new System.Drawing.Size(320, 478);
            this.groupTree.TabIndex = 15;
            this.groupTree.TabStop = false;
            // 
            // treePacCommands
            // 
            this.treePacCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treePacCommands.FullRowSelect = true;
            this.treePacCommands.HideSelection = false;
            this.treePacCommands.Location = new System.Drawing.Point(3, 16);
            this.treePacCommands.Name = "treePacCommands";
            this.treePacCommands.PathSeparator = " ";
            this.treePacCommands.Size = new System.Drawing.Size(314, 459);
            this.treePacCommands.TabIndex = 7;
            this.treePacCommands.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreePacCommands_BeforeExpand);
            this.treePacCommands.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treePacCommands_AfterSelect);
            // 
            // groupCommandArea
            // 
            this.groupCommandArea.Controls.Add(this.textBoxCommandText);
            this.groupCommandArea.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupCommandArea.Location = new System.Drawing.Point(320, 31);
            this.groupCommandArea.Name = "groupCommandArea";
            this.groupCommandArea.Size = new System.Drawing.Size(349, 44);
            this.groupCommandArea.TabIndex = 16;
            this.groupCommandArea.TabStop = false;
            this.groupCommandArea.Text = "Command";
            // 
            // textBoxCommandText
            // 
            this.textBoxCommandText.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxCommandText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCommandText.Location = new System.Drawing.Point(3, 16);
            this.textBoxCommandText.Name = "textBoxCommandText";
            this.textBoxCommandText.Size = new System.Drawing.Size(343, 20);
            this.textBoxCommandText.TabIndex = 25;
            // 
            // groupBoxHelp
            // 
            this.groupBoxHelp.Controls.Add(this.textBoxParentNodeHelp);
            this.groupBoxHelp.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxHelp.Location = new System.Drawing.Point(320, 228);
            this.groupBoxHelp.Name = "groupBoxHelp";
            this.groupBoxHelp.Size = new System.Drawing.Size(349, 281);
            this.groupBoxHelp.TabIndex = 17;
            this.groupBoxHelp.TabStop = false;
            this.groupBoxHelp.Text = "Help";
            // 
            // textBoxParentNodeHelp
            // 
            this.textBoxParentNodeHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxParentNodeHelp.Location = new System.Drawing.Point(3, 16);
            this.textBoxParentNodeHelp.Multiline = true;
            this.textBoxParentNodeHelp.Name = "textBoxParentNodeHelp";
            this.textBoxParentNodeHelp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxParentNodeHelp.Size = new System.Drawing.Size(343, 262);
            this.textBoxParentNodeHelp.TabIndex = 24;
            // 
            // groupBoxOutput
            // 
            this.groupBoxOutput.Controls.Add(this.textBoxOutput);
            this.groupBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOutput.Location = new System.Drawing.Point(320, 75);
            this.groupBoxOutput.Name = "groupBoxOutput";
            this.groupBoxOutput.Size = new System.Drawing.Size(349, 153);
            this.groupBoxOutput.TabIndex = 18;
            this.groupBoxOutput.TabStop = false;
            this.groupBoxOutput.Text = "Output";
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxOutput.Location = new System.Drawing.Point(3, 16);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxOutput.Size = new System.Drawing.Size(343, 134);
            this.textBoxOutput.TabIndex = 25;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.DocCommentHeight = 59;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 16);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(314, 459);
            this.propertyGrid1.TabIndex = 18;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            this.propertyGrid1.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propertyGrid1_SelectedGridItemChanged);
            // 
            // PACUIPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxOutput);
            this.Controls.Add(this.groupBoxHelp);
            this.Controls.Add(this.groupCommandArea);
            this.Controls.Add(this.groupTree);
            this.Controls.Add(this.groupCommandArgs);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "PACUIPluginControl";
            this.PluginIcon = ((System.Drawing.Icon)(resources.GetObject("$this.PluginIcon")));
            this.Size = new System.Drawing.Size(989, 509);
            this.TabIcon = ((System.Drawing.Image)(resources.GetObject("$this.TabIcon")));
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.groupCommandArgs.ResumeLayout(false);
            this.groupTree.ResumeLayout(false);
            this.groupCommandArea.ResumeLayout(false);
            this.groupCommandArea.PerformLayout();
            this.groupBoxHelp.ResumeLayout(false);
            this.groupBoxHelp.PerformLayout();
            this.groupBoxOutput.ResumeLayout(false);
            this.groupBoxOutput.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripLabel toolStripPacVersionLabel;
        private System.Windows.Forms.ToolStripComboBox toolStripCLIVersionsDropDown;
        private GroupBox groupCommandArgs;
        private GroupBox groupTree;
        private TreeView treePacCommands;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private PropertyGridEx propertyGrid1;
        private GroupBox groupCommandArea;
        private ToolStripButton toolStripRunButton;
        private ToolStripSeparator toolStripSeparator1;
        private TextBox textBoxCommandText;
        private GroupBox groupBoxHelp;
        private TextBox textBoxParentNodeHelp;
        private GroupBox groupBoxOutput;
        private TextBox textBoxOutput;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem syncCLIAuthWithCurrentConnectionToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
    }
}
