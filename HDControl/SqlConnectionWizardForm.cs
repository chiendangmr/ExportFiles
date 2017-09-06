using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Diagnostics;
using HDCore;
using System.IO;
using DevExpress.LookAndFeel;

namespace HDControl
{
    public partial class SqlConnectionWizardForm : HDForm
    {
        public SqlConnectionWizardForm()
        {
            InitializeComponent();

            cboUserType.SelectedIndex = 0;
        }

        private void ckMirror_CheckedChanged(object sender, EventArgs e)
        {
            txtMirror.Enabled = ckMirror.Checked;
        }

        private void cboUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtUser.Enabled = txtPassWord.Enabled = cboUserType.SelectedIndex == 1;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtServer.Text.Trim() == "")
            {
                HDMessageBox.Show("Chưa nhập server chính", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ckMirror.Checked && txtMirror.Text.Trim() == "")
            {
                HDMessageBox.Show("Chưa nhập server mirror", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cboUserType.SelectedIndex == 1)
            {
                if (txtUser.Text.Trim() == "")
                {
                    HDMessageBox.Show("Chưa nhập tài khoản", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (txtPassWord.Text == "")
                {
                    HDMessageBox.Show("Chưa nhập mật khẩu", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (txtDatabase.Text.Trim() == "")
            {
                HDMessageBox.Show("Chưa nhập tên cơ sở dữ liệu", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        public string DBConnectionString()
        {
            return "Server=" + txtServer.Text.Trim() + ";" +
                (ckMirror.Checked ? "Failover Partner=" + txtMirror.Text.Trim() + ";" : "") +
                (cboUserType.SelectedIndex != 1 ? "Integrated Security = true;" : "uid=" + txtUser.Text.Trim() + ";pwd=" + txtPassWord.Text + ";") +
                "database=" + txtDatabase.Text;
        }
    }
}