namespace HDControl
{
    partial class OpenFileInFolderDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenFileInFolderDialog));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.treeFile = new DevExpress.XtraTreeList.TreeList();
            this.colType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colDir = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeImage = new DevExpress.Utils.ImageCollection();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeImage)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOK);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 430);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(536, 41);
            this.panelControl1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(449, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(358, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "Ok";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // treeFile
            // 
            this.treeFile.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colType,
            this.colDir});
            this.treeFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFile.Location = new System.Drawing.Point(0, 0);
            this.treeFile.Name = "treeFile";
            this.treeFile.OptionsBehavior.AutoNodeHeight = false;
            this.treeFile.OptionsBehavior.Editable = false;
            this.treeFile.OptionsMenu.EnableColumnMenu = false;
            this.treeFile.OptionsMenu.EnableFooterMenu = false;
            this.treeFile.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treeFile.OptionsView.EnableAppearanceEvenRow = true;
            this.treeFile.OptionsView.EnableAppearanceOddRow = true;
            this.treeFile.OptionsView.ShowColumns = false;
            this.treeFile.OptionsView.ShowHorzLines = false;
            this.treeFile.OptionsView.ShowIndicator = false;
            this.treeFile.OptionsView.ShowVertLines = false;
            this.treeFile.RowHeight = 25;
            this.treeFile.Size = new System.Drawing.Size(536, 430);
            this.treeFile.StateImageList = this.treeImage;
            this.treeFile.TabIndex = 1;
            this.treeFile.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.treeFile_GetStateImage);
            this.treeFile.BeforeExpand += new DevExpress.XtraTreeList.BeforeExpandEventHandler(this.treeFile_BeforeExpand);
            this.treeFile.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeFile_FocusedNodeChanged);
            this.treeFile.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeFile_MouseClick);
            this.treeFile.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeFile_MouseDoubleClick);
            // 
            // colType
            // 
            this.colType.Caption = "Type";
            this.colType.FieldName = "Type";
            this.colType.Name = "colType";
            this.colType.OptionsColumn.AllowEdit = false;
            this.colType.Width = 92;
            // 
            // colDir
            // 
            this.colDir.Caption = "Dir";
            this.colDir.FieldName = "Dir";
            this.colDir.MinWidth = 49;
            this.colDir.Name = "colDir";
            this.colDir.OptionsColumn.AllowEdit = false;
            this.colDir.Visible = true;
            this.colDir.VisibleIndex = 0;
            // 
            // treeImage
            // 
            this.treeImage.ImageSize = new System.Drawing.Size(24, 24);
            this.treeImage.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("treeImage.ImageStream")));
            this.treeImage.Images.SetKeyName(0, "Folder.png");
            this.treeImage.Images.SetKeyName(1, "File.png");
            // 
            // OpenFileInFolderDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(536, 471);
            this.Controls.Add(this.treeFile);
            this.Controls.Add(this.panelControl1);
            this.MinimizeBox = false;
            this.Name = "OpenFileInFolderDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose a file";
            this.Load += new System.EventHandler(this.OpenFileInFolderDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraTreeList.TreeList treeFile;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.Utils.ImageCollection treeImage;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colType;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDir;
    }
}