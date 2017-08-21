using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HDExportMetadataAndFile.View;
using System.IO;
using HDCore;
using System.Threading.Tasks;

namespace zzz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            #region test xml
            //string temp = Path.Combine(Application.StartupPath, "test2.xml");


            //HDExportMetadataAndFile.View.XMLChildObject xmlChild = new HDExportMetadataAndFile.View.XMLChildObject()
            //{
            //    seriesTitle = "Chien Dang",
            //    seriesAction = "override"
            //};
            //HDExportMetadataAndFile.View.XMLObject xmlObject = new HDExportMetadataAndFile.View.XMLObject();
            //xmlObject.GenerateXml(xmlChild);
            //xmlObject.SaveXmlFile(temp);
            //System.Diagnostics.Process.Start(temp);
            #endregion
            #region test Move File Local
            bool uncSuccess = false;
            try
            {
                var t = Task.Run(() =>
                {
                    uncSuccess = FCopy(Path.GetFullPath(txtFilePath.Text), Path.Combine(txtPath.Text, "test.mp4"));
                });
                t.Wait();               

            }
            catch
            {
                uncSuccess = false;
            }
            if (uncSuccess)
            {
                MessageBox.Show("Done");
            }
            else
            {
                MessageBox.Show("Fail");
            }
            #endregion
            #region test Move File FTP
            //bool uncSuccess = false;
            //try
            //{
            //    var tempSourcePath = "ftp://10.10.10.39/ftpTest.mp4";
            //    using (WebClient ftpClient = new WebClient())
            //    {
            //        ftpClient.Credentials = new NetworkCredential("ftpuser", "12345678");
            //        ftpClient.DownloadFile(tempSourcePath, Path.Combine(txtPath.Text, "test.mp4"));
            //        uncSuccess = true;
            //    }
            //}
            //catch
            //{
            //    uncSuccess = false;
            //}
            //if (uncSuccess)
            //{
            //    MessageBox.Show("Done");
            //}
            //else
            //{
            //    MessageBox.Show("Fail");
            //}
            #endregion
            #region test Generate Excel
            //try
            //{
            //    string ProgramName = "0707 Nối Lại Tình Xưa Phần 2";
            //    string tempProgramName = ProgramName.Replace(" ", "").Trim(); ;
            //    var index = Utils.ConvertToVietnameseNonSign(tempProgramName).LastIndexOf("Tap");
            //    var tapStr = index >= 0 ? tempProgramName.Substring(index).Substring(3).Trim() : " ";
            //    if (tapStr.IndexOf(' ') > 0)
            //        tapStr = tapStr.Substring(0, tapStr.IndexOf(' '));
            //    tapStr = index >= 0 ? "Tap" + tapStr : " ";
            //    string temp = Path.Combine(Application.StartupPath, "test.xlsx");
            //    List<exObj> lstEx = new List<exObj>();
            //    exObj ex = new exObj()
            //    {
            //        auto_play_succesor = Utils.ConvertToVietnameseNonSign(tempProgramName),
            //        externalReference = Utils.ConvertToVietnameseNonSign(tempProgramName).ToLower(),
            //        parent = Utils.ConvertToVietnameseNonSign(tempProgramName).ToLower(),
            //        thumbnail = Utils.ConvertToVietnameseNonSign(tempProgramName).Replace(tapStr, "").ToLower() + ".jpg",
            //        media_file = Utils.ConvertToVietnameseNonSign(tempProgramName) + ".mp4",
            //        image_file_name1 = Utils.ConvertToVietnameseNonSign(tempProgramName).Replace(tapStr, "") + "_Poster.jpg",
            //        image_file_name2 = Utils.ConvertToVietnameseNonSign(tempProgramName).Replace(tapStr, "") + ".jpg",
            //        movideo_params_required1 = Utils.ConvertToVietnameseNonSign(tempProgramName) + ".mp4",
            //        movideo_params_required2 = Utils.ConvertToVietnameseNonSign(tempProgramName).Replace(tapStr, "") + ".jpg",
            //        movideo_params_required3 = Utils.ConvertToVietnameseNonSign(tempProgramName).Replace(tapStr, "") + "_Poster.jpg",
            //        movideo_params_required4 = Utils.ConvertToVietnameseNonSign(tempProgramName).Replace(tapStr, "") + ".jpg",
            //        movideo_params_required5 = Utils.ConvertToVietnameseNonSign(tempProgramName) + ".srt",
            //        media_duration = "60",
            //        linkTranscode = "2016_11",
            //        tagProfileId = "1",                    
            //        media_description = "Vô tình bị tráo đổi khi còn sơ sinh, Eun-suh trở thành tiểu thư quyền quý trong một gia đình đáng mơ ước.",
            //        product_description = " Vô tình bị tráo đổi khi còn sơ sinh, Eun-suh trở thành tiểu thư quyền quý trong một gia đình đáng mơ ước."
            //    };
            //    lstEx.Add(ex);
            //    infoXlsx InfoObj = new infoXlsx();
            //    InfoObj.DataSource = lstEx;
            //    InfoObj.CreateDocument();
            //    InfoObj.ExportToXlsx(temp);
            //    MessageBox.Show("Ok");
            //    System.Diagnostics.Process.Start(Path.GetDirectoryName(temp));
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show("Fail: " + ex.ToString());
            //}
            #endregion
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog open = new FolderBrowserDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = open.SelectedPath;
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = open.FileName;
            }
        }
        private bool FCopy(string source, string destination)
        {
            int array_length = (int)Math.Pow(2, 19);
            byte[] dataArray = new byte[array_length];
            try
            {
                using (FileStream fsread = new FileStream
                (source, FileMode.Open, FileAccess.Read, FileShare.None, array_length))
                {
                    using (BinaryReader bwread = new BinaryReader(fsread))
                    {
                        using (FileStream fswrite = new FileStream
                        (destination, FileMode.Create, FileAccess.Write, FileShare.None, array_length))
                        {
                            using (BinaryWriter bwwrite = new BinaryWriter(fswrite))
                            {
                                for (;;)
                                {
                                    int read = bwread.Read(dataArray, 0, array_length);
                                    if (0 == read)
                                        break;
                                    bwwrite.Write(dataArray, 0, read);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
