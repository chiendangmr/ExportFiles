namespace HDControl
{
    partial class SqlConnectionWizardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlConnectionWizardForm));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtServer = new DevExpress.XtraEditors.TextEdit();
            this.ckMirror = new DevExpress.XtraEditors.CheckEdit();
            this.txtMirror = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.cboUserType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtUser = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtPassWord = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtDatabase = new DevExpress.XtraEditors.TextEdit();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckMirror.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMirror.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUserType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(44, 17);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(56, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Tên server:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(109, 13);
            this.txtServer.Name = "txtServer";
            this.txtServer.Properties.NullText = "Tên/địa chỉ server chính";
            this.txtServer.Properties.NullValuePrompt = "Tên/địa chỉ server chính";
            this.txtServer.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtServer.Size = new System.Drawing.Size(329, 20);
            this.txtServer.TabIndex = 1;
            // 
            // ckMirror
            // 
            this.ckMirror.Location = new System.Drawing.Point(13, 42);
            this.ckMirror.Name = "ckMirror";
            this.ckMirror.Properties.Caption = "Mirror server:";
            this.ckMirror.Size = new System.Drawing.Size(90, 19);
            this.ckMirror.TabIndex = 2;
            this.ckMirror.CheckedChanged += new System.EventHandler(this.ckMirror_CheckedChanged);
            // 
            // txtMirror
            // 
            this.txtMirror.Enabled = false;
            this.txtMirror.Location = new System.Drawing.Point(109, 41);
            this.txtMirror.Name = "txtMirror";
            this.txtMirror.Properties.NullText = "Tên/địa chỉ mirror server";
            this.txtMirror.Properties.NullValuePrompt = "Tên/địa chỉ mirror server";
            this.txtMirror.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtMirror.Size = new System.Drawing.Size(329, 20);
            this.txtMirror.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(31, 77);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(69, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Kiểu xác thực:";
            // 
            // cboUserType
            // 
            this.cboUserType.Location = new System.Drawing.Point(109, 69);
            this.cboUserType.Name = "cboUserType";
            this.cboUserType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboUserType.Properties.Items.AddRange(new object[] {
            "Tài khoản windows",
            "Tài khoản SQL"});
            this.cboUserType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboUserType.Size = new System.Drawing.Size(329, 20);
            this.cboUserType.TabIndex = 5;
            this.cboUserType.SelectedIndexChanged += new System.EventHandler(this.cboUserType_SelectedIndexChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(50, 101);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(50, 13);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "Tài khoản:";
            // 
            // txtUser
            // 
            this.txtUser.Enabled = false;
            this.txtUser.Location = new System.Drawing.Point(109, 97);
            this.txtUser.Name = "txtUser";
            this.txtUser.Properties.NullText = "Tài khoản";
            this.txtUser.Properties.NullValuePrompt = "Tài khoản";
            this.txtUser.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtUser.Size = new System.Drawing.Size(329, 20);
            this.txtUser.TabIndex = 7;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(52, 129);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 13);
            this.labelControl4.TabIndex = 8;
            this.labelControl4.Text = "Mật khẩu:";
            // 
            // txtPassWord
            // 
            this.txtPassWord.Enabled = false;
            this.txtPassWord.Location = new System.Drawing.Point(109, 125);
            this.txtPassWord.Name = "txtPassWord";
            this.txtPassWord.Properties.NullText = "Mật khẩu";
            this.txtPassWord.Properties.NullValuePrompt = "Mật khẩu";
            this.txtPassWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPassWord.Properties.PasswordChar = '*';
            this.txtPassWord.Size = new System.Drawing.Size(329, 20);
            this.txtPassWord.TabIndex = 9;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(34, 157);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(66, 13);
            this.labelControl5.TabIndex = 10;
            this.labelControl5.Text = "Cơ sở dữ liệu:";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(109, 153);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Properties.NullText = "Tên cơ sở dữ liệu";
            this.txtDatabase.Properties.NullValuePrompt = "Tên cơ sở dữ liệu";
            this.txtDatabase.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtDatabase.Size = new System.Drawing.Size(329, 20);
            this.txtDatabase.TabIndex = 11;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(138, 185);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "Đồng ý";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(237, 185);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Thoát";
            // 
            // SqlConnectionWizardForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(450, 220);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtDatabase);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.txtPassWord);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.cboUserType);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txtMirror);
            this.Controls.Add(this.ckMirror);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SqlConnectionWizardForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cấu hình kết nối cơ sở dữ liệu";
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckMirror.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMirror.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUserType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtServer;
        private DevExpress.XtraEditors.CheckEdit ckMirror;
        private DevExpress.XtraEditors.TextEdit txtMirror;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.ComboBoxEdit cboUserType;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtUser;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtPassWord;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txtDatabase;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}