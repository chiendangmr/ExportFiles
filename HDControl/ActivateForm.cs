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

namespace HDControl
{
    public partial class ActivateForm : HDForm
    {
        public ActivateForm()
        {
            InitializeComponent();
        }

        private void ActiveForm_Load(object sender, EventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess();
            string currentProcessName = currentProcess.ProcessName;

            string cpuId = CryptorEngine.GetCPUID() + "_" + currentProcessName;
            txtSoftwareId.Text = CryptorEngine.Encrypt(cpuId, "HdVietNam");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtLicense.Text == "")
            {
                XtraMessageBox.Show("Vui lòng nhập mã kích hoạt!", "Kích hoạt phần mềm",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string nStr = "";
            try
            {
                nStr = CryptorEngine.Decrypt(txtLicense.Text, "HdVietNam");
            }
            catch { }

            string licenseFile = Path.Combine(Application.StartupPath, "license.hd");
            StreamWriter writer = new StreamWriter(licenseFile, false);
            writer.WriteLine(nStr);
            writer.Flush();
            writer.Close();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}