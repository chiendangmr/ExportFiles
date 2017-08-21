using HDControl;
using HDCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HDExportMetadataAndFile
{
    public partial class DBConfigForm : Form
    {
        public DBConfigForm()
        {
            InitializeComponent();
        }
        string dbCommandPath = "";
        private void btnDone_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSQLQuery.Text.Trim().Length > 10 && txtSQLQueryHighresFile.Text.Trim().Length > 10)
                {                    
                    bsDBConfig.Clear();
                    bsDBConfig.List.Add(new View.DBCommandObj
                    {
                        SQLQuery = txtSQLQuery.Text,
                        SQLQueryHighresFile = txtSQLQueryHighresFile.Text,
                        SQLQueryLowresFile = txtSQLQueryLowres.Text,
                        SQLQueryHighresOriginalFile=txtSQLQueryHighresOriginal.Text,
                        SQLQueryAudio = txtSQLQueryAudioFile.Text,
                        SQLQueryAudioOriginal = txtSQLAudioOriginal.Text,
                        SQLQueryMetadata = txtSQLQueryMetadata.Text,
                        SQLUpdateStatus = txtQueryUpdateStatus.Text,
                        SQLQueryPreview = txtSQLPreview.Text,
                        SQLQueryImage = txtSQLImage.Text,
                        SQLQueryMail = txtSQLMail.Text,
                        SQLQuerySubtitle = txtSQLQuerySubtitle.Text,
                        SQLQueryXmlOriginal = txtSQLQueryXmlOriginal.Text
                    });

                    (bsDBConfig.List as BindingList<View.DBCommandObj>).ToList().SaveObject(dbCommandPath);
                    HDMessageBox.Show("Cấu hình thành công! Hãy khởi động lại phần mềm để sử dụng cấu hình mới!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                    HDMessageBox.Show("Các trường (*) là bắt buộc!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                HDMessageBox.Show("Cấu hình thất bại! Mời cấu hình lại hoặc khởi động lại phần mềm!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DBConfigForm_Load(object sender, EventArgs e)
        {
            string configFolder = Path.Combine(Application.StartupPath, "Config");
            dbCommandPath = Path.Combine(configFolder, "DBCommand.xml");
            if (File.Exists(dbCommandPath))
                try
                {
                    var lstConfig = Utils.GetObject<List<View.DBCommandObj>>(dbCommandPath);
                    foreach (var temp in lstConfig)
                    {
                        txtSQLQuery.Text = temp.SQLQuery;
                        txtSQLQueryHighresFile.Text = temp.SQLQueryHighresFile;
                        txtSQLQueryLowres.Text = temp.SQLQueryLowresFile;
                        txtSQLQueryHighresOriginal.Text = temp.SQLQueryHighresOriginalFile;
                        txtSQLQueryAudioFile.Text = temp.SQLQueryAudio;
                        txtSQLAudioOriginal.Text = temp.SQLQueryAudioOriginal;
                        txtSQLQueryMetadata.Text = temp.SQLQueryMetadata;
                        txtQueryUpdateStatus.Text = temp.SQLUpdateStatus;
                        txtSQLPreview.Text = temp.SQLQueryPreview;
                        txtSQLImage.Text = temp.SQLQueryImage;
                        txtSQLMail.Text = temp.SQLQueryMail;
                        txtSQLQueryXmlOriginal.Text = temp.SQLQueryXmlOriginal;
                        txtSQLQuerySubtitle.Text = temp.SQLQuerySubtitle;
                    }
                }
                catch { }
            else
            {
                File.Create(dbCommandPath).Dispose();
            }           
        }
    }
}
