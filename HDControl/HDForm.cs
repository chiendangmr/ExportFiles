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
    public partial class HDForm : XtraForm
    {
        private bool _needActive = false;

        public HDForm()
        {
            InitializeComponent();
        }

        [DefaultValue("Office 2010 Black")]
        public string Skin
        {
            get { return UserLookAndFeel.Default.SkinName; }
            set
            {
                UserLookAndFeel.Default.SetSkinStyle(value);
            }
        }

        public bool NeedActive
        {
            get { return _needActive; }
            set { _needActive = value; }
        }

        private void HDForm_Load(object sender, EventArgs e)
        {
            if (_needActive)
            {
                LogProcess.AddLog("Kích hoạt phần mềm");

                Process currentProcess = Process.GetCurrentProcess();
                string currentProcessName = currentProcess.ProcessName;

                string cpuId = CryptorEngine.GetCPUID() + "_" + currentProcessName;
                string keyId = CryptorEngine.GetMD5String(CryptorEngine.GetMD5String(cpuId));

                string licenseFile = Path.Combine(Application.StartupPath, "license.hd");

            CheckActive:
                string license = "";
                if (File.Exists(licenseFile))
                {
                    try
                    {
                        StreamReader read = new StreamReader(licenseFile);
                        license = read.ReadLine();
                        read.Close();
                    }
                    catch { }
                }

                if (keyId != license)
                {
                    LogProcess.AddLog("Chưa kích hoạt");

                    if (XtraMessageBox.Show("Phần mềm chưa được kích hoạt!\nBạn có muốn kích hoạt ngay bây giờ?",
                        "Kích hoạt phần mềm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    {
                        LogProcess.AddLog("Không kích hoạt");
                        Process.GetCurrentProcess().Kill();
                        Application.Exit();
                        return;
                    }
                    else
                    {
                        if (new ActivateForm().ShowDialog() == DialogResult.OK)
                        {
                            goto CheckActive;
                        }
                        else
                        {
                            LogProcess.AddLog("Không kích hoạt");
                            Process.GetCurrentProcess().Kill();
                            Application.Exit();
                            return;
                        }
                    }
                }
                else
                    LogProcess.AddLog("Đã kích hoạt");
            }
        }
    }
}