using HDControl;
using HDCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using System.Net;
using System.Threading;
using System.Text;

namespace HDExportMetadataAndFile
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        string _connectionStr = "";
        string _sqlQuery = "";
        string _sqlQueryHighresFile = "";
        string _sqlQueryLowresFile = "";
        string _sqlQueryHighresOriginalFile = "";
        string _sqlQueryAudio = "";
        string _sqlQueryAudioOriginal = "";
        string _sqlQueryMetadata = "";
        string _sqlQueryPreview = "";
        string _sqlQueryImage = "";
        string _sqlQueryXmlOriginal = "";
        string _sqlQuerySubtitle = "";
        string _sqlUpdateQuery = "";
        string _logFilePath = Path.Combine(Application.StartupPath, "Logs");
        bool isRunning = true;
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (bsConfigList.List.Count > 0)
                {
                    HDMessageBox.Show("Lưu cấu hình thành công! Tiến trình xuất file bắt đầu!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.WindowState = FormWindowState.Minimized;
                    if (thrMain != null)
                    {
                        isRunning = false;
                        thrMain.Join();
                    }
                    isRunning = true;
                    thrMain = new Thread(MainThread);
                    thrMain.IsBackground = true;
                    thrMain.Start();
                }
                else { HDMessageBox.Show("Cấu hình trước!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            }
            catch (Exception ex)
            {
                HDMessageBox.Show(ex.ToString(), "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        string _dbCommandPath = "";
        string _userConfigPath = "";
        string _logFile = "";
        string _configFolder = Path.Combine(Application.StartupPath, "Config");
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(_configFolder))
            {
                Directory.CreateDirectory(_configFolder);
            }
            _dbCommandPath = Path.Combine(_configFolder, "DBCommand.xml");
            if (File.Exists(_dbCommandPath))
                try
                {
                    var lstConfig = Utils.GetObject<List<View.DBCommandObj>>(_dbCommandPath);
                    foreach (var temp in lstConfig)
                    {
                        _sqlQuery = temp.SQLQuery;
                        _sqlQueryHighresFile = temp.SQLQueryHighresFile;
                        _sqlQueryLowresFile = temp.SQLQueryLowresFile;
                        _sqlQueryHighresOriginalFile = temp.SQLQueryHighresOriginalFile;
                        _sqlQueryAudioOriginal = temp.SQLQueryAudioOriginal;
                        _sqlQueryAudio = temp.SQLQueryAudio;
                        _sqlQueryMetadata = temp.SQLQueryMetadata;
                        _sqlUpdateQuery = temp.SQLUpdateStatus;
                        _sqlQueryImage = temp.SQLQueryImage;
                        _sqlQueryPreview = temp.SQLQueryPreview;
                        _sqlQueryMail = temp.SQLQueryMail;
                        _sqlQuerySubtitle = temp.SQLQuerySubtitle;
                        _sqlQueryXmlOriginal = temp.SQLQueryXmlOriginal;
                    }
                }
                catch { }
            _connectionStr = AppSettings.Default.ConnectionStr;
            _userConfigPath = Path.Combine(_configFolder, "UserConfig.xml");

            if (!Directory.Exists(_logFilePath))
            {
                Directory.CreateDirectory(_logFilePath);
            }

            if (File.Exists(_userConfigPath))
            {
                try
                {
                    var lstConfig = Utils.GetObject<List<View.configObj>>(_userConfigPath);
                    foreach (var temp in lstConfig)
                    {
                        bsConfigList.List.Add(temp);
                    }
                }
                catch (Exception ex)
                {
                    addLog(_logFile, "Khong load config: " + ex.ToString());
                }
            }
            else
            {
                addLog(_logFile, "Khong co config");
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            DBConfigForm frmConfig = new DBConfigForm();
            frmConfig.Show();
            frmConfig.Activate();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutForm frmAbout = new AboutForm();
            frmAbout.Show();
            frmAbout.Activate();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false;
            if (thrMain != null)
                thrMain.Join();

            Application.Exit();
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog saveFolder = new FolderBrowserDialog();
            if (txtSaveFolder.Text.Trim().Length > 2)
                if (Directory.Exists(Path.GetFullPath(txtSaveFolder.Text)))
                {
                    saveFolder.SelectedPath = txtSaveFolder.Text;
                }
            if (saveFolder.ShowDialog() == DialogResult.OK)
            {
                txtSaveFolder.Text = saveFolder.SelectedPath;
            }
        }

        private void ckHeader_CheckedChanged(object sender, EventArgs e)
        {
            if (ckHeader.Checked)
                ckFooter.Checked = false;
            else ckFooter.Checked = true;
        }

        private void ckFooter_CheckedChanged(object sender, EventArgs e)
        {
            if (ckFooter.Checked)
                ckHeader.Checked = false;
            else ckHeader.Checked = true;
        }
        private bool isNasPath(string path)
        {
            if (path.Contains("ftp://")) return true;
            return false;
        }
        string strWaiting73 = "";
        string strProcessing73 = "";
        string strWaiting74 = "";
        string strProcessing74 = "";
        Thread thrMain = null;
        void MainThread()
        {
            var lstConfig = bsConfigList.List as BindingList<View.configObj>;
            while (isRunning)
            {
                try
                {
                    string[] files = Directory.GetFiles(_logFilePath, "*.txt", SearchOption.TopDirectoryOnly);
                    if (files.Count() > 80)
                    {
                        for (var i = 0; i < 50; i++)
                        {
                            System.IO.File.Delete(files[i]);
                        }
                    }

                    _logFile = Path.Combine(_logFilePath, DateTime.Now.Date.ToString("yyyy:MM:dd").Replace(":", "") + ".txt");
                    if (!File.Exists(_logFile))
                    {
                        File.Create(_logFile).Dispose();
                    }
                    using (var db = new SqlConnection(_connectionStr))
                    {
                        try
                        {
                            Task[] _task = new Task[2];
                            _task[0] = Task.Run(() =>
                            {
                                if (_sqlQuery != "" && _sqlQuery != null)
                                {
                                    var lstExportDB = db.Query<long>(_sqlQuery, new
                                    {
                                        nodeId = 73
                                    }).ToList();
                                    if (lstExportDB.Count > 0)
                                    {
                                        strWaiting73 = "";
                                        foreach (var i in lstExportDB)
                                        {
                                            strWaiting73 += i.ToString() + ", ";
                                        }
                                        foreach (var postId in lstExportDB)
                                        {
                                            strProcessing73 = postId.ToString();
                                            strWaiting73 = strWaiting73.Replace(strProcessing73, "");
                                            foreach (var tempConfig in lstConfig)
                                            {

                                                try
                                                {
                                                    var filesDB = db.Query<View.FileObj>(_sqlQueryHighresFile, new { id_clip = postId }).FirstOrDefault();
                                                    if (filesDB != null)
                                                    {
                                                        #region fileName
                                                        var ProgramName = filesDB.TEN_CHUONG_TRINH == null ? " " : filesDB.TEN_CHUONG_TRINH;
                                                        var MaBang = filesDB.MA_BANG == null ? " " : filesDB.MA_BANG;
                                                        var creatDate = DateTime.Now;//filesDB.CREATE_DATE == null ? DateTime.Now : filesDB.CREATE_DATE;
                                                        var broadcastDate = DateTime.Now;//filesDB.DATE_TO_BROADCAST == null ? DateTime.Now : filesDB.DATE_TO_BROADCAST;
                                                        var startRight = filesDB.START_RIGHTS == null ? DateTime.Now : filesDB.START_RIGHTS;
                                                        var endRight = filesDB.END_RIGHTS == null ? DateTime.Now.AddYears(1) : filesDB.END_RIGHTS;
                                                        var season = filesDB.Season == null ? " " : filesDB.Season;
                                                        var episode = filesDB.EPISODE_NUMBER == null ? 0 : filesDB.EPISODE_NUMBER;
                                                        string typeDescription = filesDB.TYPE_DESCRIPTION == null ? "Others" : Utils.ConvertToVietnameseNonSign(filesDB.TYPE_DESCRIPTION.Replace("(", "_").Replace(")", "_").Replace("/","_"));

                                                        string OriginalFileName = tempConfig.FileName.Contains("Mã băng") ? Utils.ConvertToVietnameseNonSign(MaBang.Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim()) : " ";
                                                        if (tempConfig.FileName.Contains("Tên chương trình"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(ProgramName).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim();
                                                        }
                                                        if (tempConfig.FileName.Contains("Ngày tháng phát sóng"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(broadcastDate.ToString("yyyy-MM-dd HH:mm:ss")).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim();
                                                        }
                                                        if (tempConfig.FileName.Contains("Ngày tháng tạo vỏ"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(creatDate.ToString("yyyy-MM-dd HH:mm:ss")).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim();
                                                        }
                                                        if (tempConfig.FileName.Contains("Phần"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(season).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim();
                                                        }
                                                        if (tempConfig.FileName.Contains("Tập"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(episode.ToString()).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace("\\", "_").Replace(":", "_").Replace("/", "_").Trim();
                                                        }
                                                        var tempHighres = filesDB.FILE_NAME == null ? " " : filesDB.FILE_NAME;
                                                        var tempThumbPicture = filesDB.THUMB_FILE_NAME == null ? " " : filesDB.THUMB_FILE_NAME;
                                                        var tempLowres = filesDB.HD_CLIP == null ? " " : filesDB.HD_CLIP;
                                                        var nasHighresPath = filesDB.DATA1_DIRECTORY == null ? " " : filesDB.DATA1_DIRECTORY;
                                                        var uncHighresPath = filesDB.UNC_BASE_PATH_DATA1 == null ? " " : filesDB.UNC_BASE_PATH_DATA1;
                                                        var nasLowresPath = filesDB.DATA3_DIRECTORY == null ? " " : filesDB.DATA3_DIRECTORY;
                                                        var uncLowresPath = filesDB.UNC_BASE_PATH_DATA3 == null ? " " : filesDB.UNC_BASE_PATH_DATA3;
                                                        string addSymbol = tempConfig.AddSymBol.Contains("Đầu tên file") ? tempConfig.AddSymBol.Replace("Đầu tên file:", "").Trim() : tempConfig.AddSymBol.Replace("Cuối tên file:", "").Trim();
                                                        string srcPath = "ftp://" + filesDB.NAS_IP + ":" + filesDB.PORT;
                                                        string ftpUploadFolder = tempConfig.NasPath + typeDescription + "/";
                                                        string localUploadFolder = Path.Combine(tempConfig.LocalPath, typeDescription);
                                                        if (tempConfig.AddSymBol.Contains("Đầu tên file"))
                                                        {
                                                            OriginalFileName = addSymbol + OriginalFileName;
                                                        }
                                                        if (tempConfig.AddSymBol.Contains("Cuối tên file"))
                                                        {
                                                            OriginalFileName += addSymbol;
                                                        }
                                                        if (tempConfig.NasPath.Length > 7)
                                                            if (!ftpDirectoryExists(typeDescription, tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            {
                                                                if (!createFTPDirectory(tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass, typeDescription))
                                                                {
                                                                    addLog(_logFile, "Node 73: Tao thu muc " + typeDescription + " tren nas khong thanh cong");
                                                                    ftpUploadFolder = tempConfig.NasPath;
                                                                }
                                                            }
                                                        #endregion
                                                        #region Export từ nas này sang nas khác
                                                        if (tempConfig.NasPath.Length > 15)
                                                        {

                                                            #region Export Media Highres
                                                            bool uncHighresSuccess = false;
                                                            if (tempConfig.FileType.Contains("Media Highres") && uncHighresPath != null && uncHighresPath.Length > 0)
                                                            {
                                                                try
                                                                {
                                                                    if (uploadFromUnc(uncHighresPath + "\\" + tempHighres, OriginalFileName + ".mxf", ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                    {
                                                                        uncHighresSuccess = true;
                                                                        //Ghi log
                                                                        addLog(_logFile, "Node 73: Xuat highres bang unc thanh cong");
                                                                    }
                                                                    else
                                                                    {
                                                                        //Ghi log
                                                                        addLog(_logFile, "Node 73: Ko xuat dc highres bang unc");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 73: Loi khi xuat highres bang unc: " + ex.ToString()); }
                                                            }
                                                            if (tempConfig.FileType.Contains("Media Highres") && nasHighresPath != null && nasHighresPath.Length > 0 && !uncHighresSuccess)
                                                            {
                                                                try
                                                                {
                                                                    var tempSourcePath = srcPath + nasHighresPath;
                                                                    if (copyFile(tempHighres, OriginalFileName + ".mxf", tempSourcePath, filesDB.USERNAME, filesDB.PASSWORD, ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                    {
                                                                        //Ghi log
                                                                        addLog(_logFile, "Node 73: Xuat highres bang ftp thanh cong");
                                                                    }
                                                                    else
                                                                    {
                                                                        //Ghi log
                                                                        addLog(_logFile, "Node 73: Ko xuat dc highres bang ftp");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 73: Loi khi xuat highres bang ftp: " + ex.ToString()); }
                                                            }
                                                            #endregion

                                                            #region Export Media Lowres
                                                            //bool uncLowresSucess = false;
                                                            //if (tempConfig.FileType.Contains("Lowres doc"))
                                                            //{
                                                            //    try
                                                            //    {
                                                            //        var lowresDb = db.Query<View.FileObj>(_sqlQueryLowresFile, new { id_clip = postId }).FirstOrDefault();
                                                            //        string srcLowresPath = "ftp://" + lowresDb.NAS_IP + ":" + lowresDb.PORT;
                                                            //        if (lowresDb != null)
                                                            //        {
                                                            //            //unc
                                                            //            try
                                                            //            {
                                                            //                if (uploadFromUnc(Path.Combine(lowresDb.UNC_BASE_PATH_DATA3, lowresDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(lowresDb.FILE_NAME), tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                {
                                                            //                    uncLowresSucess = true;
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 73: Xuat lowres bang unc thanh cong");
                                                            //                }
                                                            //                else
                                                            //                {
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 73: Xuat lowres bang unc ko thanh cong");
                                                            //                }
                                                            //            }
                                                            //            catch (Exception ex)
                                                            //            {
                                                            //                addLog(_logFile, "Node 73: Xuat lowres bang unc ko thanh cong: " + ex.ToString());
                                                            //            }
                                                            //            //nas
                                                            //            if (!uncLowresSucess)
                                                            //            {
                                                            //                try
                                                            //                {
                                                            //                    if (copyFile(lowresDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(lowresDb.FILE_NAME), srcLowresPath + lowresDb.DATA3_DIRECTORY, lowresDb.USERNAME, lowresDb.PASSWORD, tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 73: Xuat lowres bang ftp thanh cong");
                                                            //                    }
                                                            //                    else
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 73: Xuat lowres bang ftp ko thanh cong" + " Path: " + srcLowresPath);
                                                            //                    }
                                                            //                }
                                                            //                catch (Exception ex) { addLog(_logFile, "Node 73: Xuat lowres bang ftp ko thanh cong: " + ex.ToString()); }
                                                            //            }
                                                            //        }
                                                            //        else
                                                            //        {
                                                            //            addLog(_logFile, "Node 73: Ko có lowres de xuat bang ftp");
                                                            //            addToMail("Node 73: Ko có lowres trong he thong");
                                                            //        }
                                                            //    }
                                                            //    catch (Exception ex) { addLog(_logFile, "Node 73: Loi khi xuat lowres: " + ex.ToString()); }
                                                            //}
                                                            #endregion
                                                            #region Export Highres original
                                                            //bool uncHighresOriginalSucess = false;
                                                            //if (tempConfig.FileType.Contains("Highres Original"))
                                                            //{
                                                            //    try
                                                            //    {
                                                            //        var audioDb = db.Query<View.FileObj>(_sqlQueryHighresOriginalFile, new { id_clip = postId }).FirstOrDefault();
                                                            //        string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                            //        if (audioDb != null)
                                                            //        {
                                                            //            //unc
                                                            //            try
                                                            //            {
                                                            //                if (uploadFromUnc(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                {
                                                            //                    uncHighresOriginalSucess = true;
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 73: Xuat highres original bang unc thanh cong");
                                                            //                }
                                                            //                else
                                                            //                {
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 73: Xuat highres original bang unc ko thanh cong");
                                                            //                }
                                                            //            }
                                                            //            catch (Exception ex)
                                                            //            {
                                                            //                addLog(_logFile, "Node 73: Xuat highres original bang unc ko thanh cong: " + ex.ToString());
                                                            //            }
                                                            //            //nas
                                                            //            if (!uncHighresOriginalSucess)
                                                            //            {
                                                            //                try
                                                            //                {
                                                            //                    if (copyFile(audioDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.NAS_DATA_PATH), audioDb.USERNAME, audioDb.PASSWORD, tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 73: Xuat highres original bang ftp thanh cong");
                                                            //                    }
                                                            //                    else
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 73: Xuat highres original bang ftp ko thanh cong");
                                                            //                    }
                                                            //                }
                                                            //                catch (Exception ex) { addLog(_logFile, "Node 73: Xuat highres original bang ftp ko thanh cong: " + ex.ToString()); }
                                                            //            }
                                                            //        }
                                                            //        else
                                                            //        {
                                                            //            addLog(_logFile, "Node 73: Ko có highres original de xuat bang ftp");
                                                            //            addToMail("Node 73: Ko có highres original trong he thong");
                                                            //        }
                                                            //    }
                                                            //    catch (Exception ex) { addLog(_logFile, "Node 73: Loi khi xuat highres original: " + ex.ToString()); }
                                                            //}
                                                            #endregion

                                                            #region Export Image
                                                            bool uncImageSucess = false;
                                                            if (tempConfig.FileType.Contains("Image"))
                                                            {
                                                                try
                                                                {
                                                                    var audioDb = db.Query<View.FileObj>(_sqlQueryImage, new { id_clip = postId }).FirstOrDefault();
                                                                    string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                                    if (audioDb != null)
                                                                    {
                                                                        //unc
                                                                        try
                                                                        {
                                                                            if (uploadFromUnc(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                            {
                                                                                uncImageSucess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat image original bang unc thanh cong");
                                                                            }
                                                                            else
                                                                            {
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat image original bang unc ko thanh cong");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            addLog(_logFile, "Node 73: Xuat image original bang unc ko thanh cong: " + ex.ToString());
                                                                        }
                                                                        //nas
                                                                        if (!uncImageSucess)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (copyFile(audioDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.NAS_DATA_PATH), audioDb.USERNAME, audioDb.PASSWORD, ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat image original bang ftp thanh cong");
                                                                                }
                                                                                else
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat image original bang ftp ko thanh cong");
                                                                                }
                                                                            }
                                                                            catch (Exception ex) { addLog(_logFile, "Node 73: Xuat image original bang ftp ko thanh cong: " + ex.ToString()); }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        addLog(_logFile, "Node 73: Ko có image de xuat bang ftp");
                                                                        addToMail("Node 73: Ko có image trong he thong");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 73: Loi khi xuat lowres original: " + ex.ToString()); }
                                                            }
                                                            #endregion
                                                            #region Export Preview
                                                            bool uncPreviewSucess = false;
                                                            if (tempConfig.FileType.Contains("Preview"))
                                                            {
                                                                try
                                                                {
                                                                    var audioDb = db.Query<View.FileObj>(_sqlQueryPreview, new { id_clip = postId }).FirstOrDefault();
                                                                    string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                                    if (audioDb != null)
                                                                    {
                                                                        //unc
                                                                        try
                                                                        {
                                                                            if (uploadFromUnc(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.HD_CLIP), OriginalFileName + "." + Path.GetExtension(audioDb.HD_CLIP), ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                            {
                                                                                uncPreviewSucess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat Preview original bang unc thanh cong");
                                                                            }
                                                                            else
                                                                            {
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat Preview original bang unc ko thanh cong");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            addLog(_logFile, "Node 73: Xuat Preview original bang unc ko thanh cong: " + ex.ToString());
                                                                        }
                                                                        //nas
                                                                        if (!uncPreviewSucess)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (copyFile(audioDb.HD_CLIP, OriginalFileName + "." + Path.GetExtension(audioDb.HD_CLIP), Path.Combine(srcAudioPath, audioDb.NAS_DATA_PATH), audioDb.USERNAME, audioDb.PASSWORD, ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat Preview original bang ftp thanh cong");
                                                                                }
                                                                                else
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat Preview original bang ftp ko thanh cong");
                                                                                }
                                                                            }
                                                                            catch (Exception ex) { addLog(_logFile, "Node 73: Xuat Preview original bang ftp ko thanh cong: " + ex.ToString()); }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        addLog(_logFile, "Node 73: " + postId.ToString() + " - Ko có Preview original de xuat bang ftp");
                                                                        addToMail("Node 73: " + postId.ToString() + " - Ko có Preview trong he thong");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 73: " + postId.ToString() + " - Loi khi xuat Preview original: " + ex.ToString()); }
                                                            }
                                                            #endregion

                                                            #region Export audio
                                                            //bool uncPicSucess = false;
                                                            //if (tempConfig.FileType.Contains("Audio doc"))
                                                            //{
                                                            //    try
                                                            //    {
                                                            //        var audioDb = db.Query<View.AudioObj>(_sqlQueryAudio, new { id_clip = postId }).FirstOrDefault();
                                                            //        string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                            //        if (audioDb != null)
                                                            //        {
                                                            //            //unc
                                                            //            try
                                                            //            {
                                                            //                if (uploadFromUnc(Path.Combine(audioDb.UNC_BASE_PATH_DATA3, audioDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                {
                                                            //                    uncPicSucess = true;
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 73: Xuat audio bang unc thanh cong");
                                                            //                }
                                                            //                else
                                                            //                {
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 73: Xuat audio bang unc ko thanh cong");
                                                            //                }
                                                            //            }
                                                            //            catch (Exception ex)
                                                            //            {
                                                            //                addLog(_logFile, "Node 73: Xuat audio bang unc ko thanh cong: " + ex.ToString());
                                                            //            }
                                                            //            //nas
                                                            //            if (!uncPicSucess)
                                                            //            {
                                                            //                try
                                                            //                {
                                                            //                    if (copyFile(audioDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.DATA3_DIRECTORY), audioDb.USERNAME, audioDb.PASSWORD, tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 73: Xuat poster bang ftp thanh cong");
                                                            //                    }
                                                            //                    else
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 73: Xuat poster bang ftp ko thanh cong");
                                                            //                    }
                                                            //                }
                                                            //                catch (Exception ex) { addLog(_logFile, "Node 73: Xuat poster bang ftp ko thanh cong: " + ex.ToString()); }
                                                            //            }
                                                            //        }
                                                            //        else
                                                            //        {
                                                            //            addLog(_logFile, "Node 73: Ko có audio de xuat bang ftp");
                                                            //            addToMail("Node 73: Ko có audio trong he thong");
                                                            //        }
                                                            //    }
                                                            //    catch (Exception ex) { addLog(_logFile, "Node 73: Loi khi xuat poster: " + ex.ToString()); }
                                                            //}
                                                            #endregion
                                                            #region Export audio Upload
                                                            bool uncAuOriginalSucess = false;
                                                            if (tempConfig.FileType.Contains("Audio Upload"))
                                                            {
                                                                try
                                                                {
                                                                    var audioDb = db.Query<View.AudioObj>(_sqlQueryAudioOriginal, new { id_clip = postId }).FirstOrDefault();
                                                                    string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                                    if (audioDb != null)
                                                                    {
                                                                        //unc
                                                                        try
                                                                        {
                                                                            if (uploadFromUnc(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                            {
                                                                                uncAuOriginalSucess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat audio upload bang unc thanh cong");
                                                                            }
                                                                            else
                                                                            {
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat audio upload bang unc ko thanh cong");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            addLog(_logFile, "Node 73: Xuat audio upload bang unc ko thanh cong: " + ex.ToString());
                                                                        }
                                                                        //nas
                                                                        if (!uncAuOriginalSucess)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (copyFile(audioDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.NAS_DATA_PATH), audioDb.USERNAME, audioDb.PASSWORD, ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat audio upload bang ftp thanh cong");
                                                                                }
                                                                                else
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat audio upload bang ftp ko thanh cong");
                                                                                }
                                                                            }
                                                                            catch (Exception ex) { addLog(_logFile, "Node 73: Xuat audio upload bang ftp ko thanh cong: " + ex.ToString()); }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        addLog(_logFile, "Node 73: Ko có audio upload de xuat bang ftp");
                                                                        addToMail("Node 73: Ko có audio upload trong he thong");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 73: Loi khi xuat audio upload: " + ex.ToString()); }
                                                            }
                                                            #endregion

                                                            #region Export Subtitle
                                                            bool uncSubtitleSucess = false;
                                                            if (tempConfig.FileType.Contains("Subtitle"))
                                                            {
                                                                try
                                                                {
                                                                    var audioDb = db.Query<View.FileObj>(_sqlQuerySubtitle, new { id_clip = postId }).FirstOrDefault();
                                                                    string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                                    if (audioDb != null)
                                                                    {
                                                                        //unc
                                                                        try
                                                                        {
                                                                            if (uploadFromUnc(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                            {
                                                                                uncSubtitleSucess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat subtitle bang unc thanh cong");
                                                                            }
                                                                            else
                                                                            {
                                                                                uncSubtitleSucess = false;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat subtitle bang unc ko thanh cong");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            addLog(_logFile, "Node 73: Xuat subtitle bang unc ko thanh cong: " + ex.ToString());
                                                                        }
                                                                        //nas
                                                                        if (!uncSubtitleSucess)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (copyFile(audioDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.NAS_DATA_PATH), audioDb.USERNAME, audioDb.PASSWORD, ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat subtitle bang ftp thanh cong");
                                                                                }
                                                                                else
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat subtitle bang ftp ko thanh cong");
                                                                                }
                                                                            }
                                                                            catch (Exception ex) { addLog(_logFile, "Node 73: Xuat subtitle bang ftp ko thanh cong: " + ex.ToString()); }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        addLog(_logFile, "Node 73: Ko có subtitle de xuat bang ftp");
                                                                        addToMail("Node 73: Ko có subtitle trong he thong");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 73: Loi khi xuat subtitle: " + ex.ToString()); }
                                                            }
                                                            #endregion

                                                            #region Export XML
                                                            if (tempConfig.FileType.Contains("XML"))
                                                            {

                                                                try
                                                                {
                                                                    var metaDb = db.Query<View.metaObj>(_sqlQueryMetadata, new { post_id = postId }).FirstOrDefault();
                                                                    View.XMLChildObject xmlChild = new View.XMLChildObject()
                                                                    {
                                                                        fileID = postId,
                                                                        FileName = metaDb.FILE_NAME,
                                                                        FileOnNas = metaDb.NAS_ID,
                                                                        AFD = metaDb.AFD_TYPE,
                                                                        CensorMan = metaDb.CHECKER_USER_ID,
                                                                        CensorTime = metaDb.CHECK_DATE,
                                                                        MaBang = metaDb.MA_BANG,
                                                                        TransferStatus = metaDb.FILE_TRANS_STATUS,
                                                                        Resolution = metaDb.VIDEO_FORMAT,
                                                                        Duration = (TimeCode.TimeCodeFromString(metaDb.REAL_TC_OUT) - TimeCode.TimeCodeFromString(metaDb.REAL_TC_IN))
                                                                    };
                                                                    View.XMLObject xmlObject = new View.XMLObject();
                                                                    xmlObject.GenerateXml(xmlChild);
                                                                    var temp = Path.Combine(txtSaveFolder.Text, OriginalFileName + ".xml");
                                                                    xmlObject.SaveXmlFile(temp);
                                                                    try
                                                                    {
                                                                        if (uploadFromUnc(temp, OriginalFileName + ".xml", ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                        {
                                                                            addLog(_logFile, "Node 73: Xuat xml len ftp bang unc thanh cong");
                                                                            File.Delete(temp);
                                                                        }
                                                                        else
                                                                        {
                                                                            //Ghi log
                                                                            addLog(_logFile, "Node 73: Xuat xml len ftp bang unc ko thanh cong");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 73: Xuat xml len ftp bang unc ko thanh cong: " + ex.ToString());
                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    addLog(_logFile, "Node 73: Xuat xml sang ftp path tu nas ko thanh cong: " + ex.ToString());
                                                                }
                                                            }
                                                            #endregion

                                                        }
                                                        #endregion

                                                        #region Export từ nas sang unc path
                                                        if (tempConfig.LocalPath.Trim().Length > 0)
                                                        {
                                                            try
                                                            {
                                                                if (!Directory.Exists(localUploadFolder))
                                                                {
                                                                    Directory.CreateDirectory(localUploadFolder);
                                                                }

                                                                #region copy highres
                                                                if (tempConfig.FileType.Contains("Media Highres"))
                                                                {
                                                                    //unc
                                                                    bool uncSuccess = false;
                                                                    try
                                                                    {
                                                                        var t = Task.Run(() =>
                                                                        {
                                                                            uncSuccess = FCopy(uncHighresPath + "\\" + tempHighres, Path.Combine(localUploadFolder, OriginalFileName + ".mxf"));
                                                                        });
                                                                        t.Wait();
                                                                        if (uncSuccess)
                                                                        {
                                                                            //Ghi log
                                                                            addLog(_logFile, "Node 73: Xuat highres sang unc path tu unc path thanh cong");
                                                                        }
                                                                        else
                                                                        {
                                                                            addLog(_logFile, "Node 73: Xuat highres sang unc path tu unc path ko thanh cong");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        uncSuccess = false;
                                                                        //Ghi log
                                                                        addLog(_logFile, "Node 73: Xuat highres sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                    }
                                                                    //ftp
                                                                    if (!uncSuccess)
                                                                    {
                                                                        try
                                                                        {
                                                                            var tempSourcePath = srcPath + nasHighresPath;
                                                                            using (WebClient ftpClient = new WebClient())
                                                                            {
                                                                                ftpClient.Credentials = new NetworkCredential(filesDB.USERNAME, filesDB.PASSWORD);
                                                                                var t = Task.Run(() =>
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        ftpClient.DownloadFile(tempSourcePath, Path.Combine(localUploadFolder, OriginalFileName + ".mxf"));
                                                                                        //Ghi log
                                                                                        addLog(_logFile, "Node 73: Xuat highres sang unc path tu ftp path thanh cong");
                                                                                    }
                                                                                    catch (Exception ex) { addLog(_logFile, "Node 73: Loi xuat highres sang unc path tu ftp path: " + ex.ToString()); }
                                                                                });
                                                                                t.Wait();

                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            //Ghi log
                                                                            addLog(_logFile, "Node 73: Xuat highres sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                        }
                                                                    }
                                                                }
                                                                #endregion

                                                                #region copy lowres
                                                                //if (tempConfig.FileType.Contains("Lowres doc"))
                                                                //{
                                                                //    try
                                                                //    {
                                                                //        var audioDb = db.Query<View.FileObj>(_sqlQueryLowresFile, new { id_clip = postId }).FirstOrDefault();
                                                                //        if (audioDb != null)
                                                                //        {
                                                                //            //unc
                                                                //            bool uncSuccess = false;
                                                                //            try
                                                                //            {
                                                                //                File.Copy(Path.Combine(audioDb.UNC_BASE_PATH_DATA3, audioDb.FILE_NAME), Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                uncSuccess = true;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 73: Xuat lowres sang unc path tu unc path thanh cong");
                                                                //            }
                                                                //            catch (Exception ex)
                                                                //            {
                                                                //                uncSuccess = false;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 73: Xuat lowres sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                //            }
                                                                //            //ftp
                                                                //            if (!uncSuccess)
                                                                //            {
                                                                //                try
                                                                //                {
                                                                //                    using (WebClient ftpClient = new WebClient())
                                                                //                    {
                                                                //                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                //                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + audioDb.DATA3_DIRECTORY + "/" + audioDb.FILE_NAME, Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                        //Ghi log
                                                                //                        addLog(_logFile, "Node 73: Xuat lowres sang unc path tu ftp path thanh cong");
                                                                //                    }
                                                                //                }
                                                                //                catch (Exception ex)
                                                                //                {
                                                                //                    //Ghi log
                                                                //                    addLog(_logFile, "Node 73: Xuat lowres sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                //                }
                                                                //            }
                                                                //        }
                                                                //        else
                                                                //        {
                                                                //            addLog(_logFile, "Node 73: Ko co file lowes trong he thong khi xuat sang unc");
                                                                //            addToMail("Node 73: Ko co file lowes trong he thong");
                                                                //        }
                                                                //    }
                                                                //    catch (Exception ex)
                                                                //    {
                                                                //        addLog(_logFile, "Node 73: Xuat lowres sang unc path ko thanh cong: " + ex.ToString());
                                                                //    }
                                                                //}
                                                                #endregion
                                                                #region Export highres original
                                                                //if (tempConfig.FileType.Contains("Highres Original"))
                                                                //{
                                                                //    try
                                                                //    {
                                                                //        var audioDb = db.Query<View.FileObj>(_sqlQueryHighresOriginalFile, new { id_clip = postId }).FirstOrDefault();
                                                                //        if (audioDb != null)
                                                                //        {
                                                                //            //unc
                                                                //            bool uncSuccess = false;
                                                                //            try
                                                                //            {
                                                                //                File.Copy(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                uncSuccess = true;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 73: Xuat highres original sang unc path tu unc path thanh cong");
                                                                //            }
                                                                //            catch (Exception ex)
                                                                //            {
                                                                //                uncSuccess = false;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 73: Xuat highres original sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                //            }
                                                                //            //ftp
                                                                //            if (!uncSuccess)
                                                                //            {
                                                                //                try
                                                                //                {
                                                                //                    using (WebClient ftpClient = new WebClient())
                                                                //                    {
                                                                //                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                //                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + "/" + audioDb.NAS_DATA_PATH + "/" + audioDb.FILE_NAME, Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                        //Ghi log
                                                                //                        addLog(_logFile, "Node 73: Xuat highres original sang unc path tu ftp path thanh cong");
                                                                //                    }
                                                                //                }
                                                                //                catch (Exception ex)
                                                                //                {
                                                                //                    //Ghi log
                                                                //                    addLog(_logFile, "Node 73: Xuat highres original sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                //                }
                                                                //            }
                                                                //        }
                                                                //        else
                                                                //        {
                                                                //            addLog(_logFile, "Node 73: Ko tim thay highres original trong he thong.");
                                                                //            addToMail("Node 73: Ko tim thay highres original trong he thong.");
                                                                //        }
                                                                //    }
                                                                //    catch (Exception ex)
                                                                //    {
                                                                //        addLog(_logFile, "Node 73: Xuat highres original sang unc path ko thanh cong: " + ex.ToString());
                                                                //    }
                                                                //}
                                                                #endregion

                                                                #region Export audio
                                                                //if (tempConfig.FileType.Contains("Audio doc"))
                                                                //{
                                                                //    try
                                                                //    {
                                                                //        var audioDb = db.Query<View.AudioObj>(_sqlQueryAudio, new { id_clip = postId }).FirstOrDefault();
                                                                //        if (audioDb != null)
                                                                //        {
                                                                //            //unc
                                                                //            bool uncSuccess = false;
                                                                //            try
                                                                //            {
                                                                //                File.Copy(Path.Combine(audioDb.UNC_BASE_PATH_DATA3, audioDb.FILE_NAME), Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                uncSuccess = true;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 73: Xuat audio sang unc path tu unc path thanh cong");
                                                                //            }
                                                                //            catch (Exception ex)
                                                                //            {
                                                                //                uncSuccess = false;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 73: Xuat audio sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                //            }
                                                                //            //ftp
                                                                //            if (!uncSuccess)
                                                                //            {
                                                                //                try
                                                                //                {
                                                                //                    using (WebClient ftpClient = new WebClient())
                                                                //                    {
                                                                //                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                //                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + audioDb.DATA3_DIRECTORY + "/" + audioDb.FILE_NAME, Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                        //Ghi log
                                                                //                        addLog(_logFile, "Node 73: Xuat audio sang unc path tu ftp path thanh cong");
                                                                //                    }
                                                                //                }
                                                                //                catch (Exception ex)
                                                                //                {
                                                                //                    //Ghi log
                                                                //                    addLog(_logFile, "Node 73: Xuat audio sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                //                }
                                                                //            }
                                                                //        }
                                                                //        else
                                                                //        {
                                                                //            addLog(_logFile, "Node 73: Ko tim thay audio trong he thong.");
                                                                //            addToMail("Node 73: Ko tim thay audio trong he thong.");
                                                                //        }
                                                                //    }
                                                                //    catch (Exception ex)
                                                                //    {
                                                                //        addLog(_logFile, "Node 73: Xuat audio sang unc path ko thanh cong: " + ex.ToString());
                                                                //    }
                                                                //}
                                                                #endregion
                                                                #region Export audio original
                                                                if (tempConfig.FileType.Contains("Audio Upload"))
                                                                {
                                                                    try
                                                                    {
                                                                        var audioDb = db.Query<View.AudioObj>(_sqlQueryAudioOriginal, new { id_clip = postId }).FirstOrDefault();
                                                                        if (audioDb != null)
                                                                        {
                                                                            //unc
                                                                            bool uncSuccess = false;
                                                                            try
                                                                            {
                                                                                File.Copy(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                uncSuccess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat audio Upload sang unc path tu unc path thanh cong");
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                uncSuccess = false;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat audio Upload sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                            }
                                                                            //ftp
                                                                            if (!uncSuccess)
                                                                            {
                                                                                try
                                                                                {
                                                                                    using (WebClient ftpClient = new WebClient())
                                                                                    {
                                                                                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + "/" + audioDb.NAS_DATA_PATH + "/" + audioDb.FILE_NAME, Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                        //Ghi log
                                                                                        addLog(_logFile, "Node 73: Xuat audio Upload sang unc path tu ftp path thanh cong");
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat audio Upload sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            addLog(_logFile, "Node 73: Ko tim thay audio Upload trong he thong.");
                                                                            addToMail("Node 73: Ko tim thay audio Upload trong he thong.");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 73: Xuat audio Upload sang unc path ko thanh cong: " + ex.ToString());
                                                                    }
                                                                }
                                                                #endregion

                                                                #region Export Image
                                                                if (tempConfig.FileType.Contains("Image"))
                                                                {
                                                                    try
                                                                    {
                                                                        var audioDb = db.Query<View.AudioObj>(_sqlQueryImage, new { id_clip = postId }).FirstOrDefault();
                                                                        if (audioDb != null)
                                                                        {
                                                                            //unc
                                                                            bool uncSuccess = false;
                                                                            try
                                                                            {
                                                                                File.Copy(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                uncSuccess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat Image sang unc path tu unc path thanh cong");
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                uncSuccess = false;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat Image sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                            }
                                                                            //ftp
                                                                            if (!uncSuccess)
                                                                            {
                                                                                try
                                                                                {
                                                                                    using (WebClient ftpClient = new WebClient())
                                                                                    {
                                                                                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + "/" + audioDb.NAS_DATA_PATH + "/" + audioDb.FILE_NAME, Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                        //Ghi log
                                                                                        addLog(_logFile, "Node 73: Xuat Image sang unc path tu ftp path thanh cong");
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat Image sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            addLog(_logFile, "Node 73: Ko tim thay Image trong he thong.");
                                                                            addToMail("Node 73: Ko tim thay image trong he thong.");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 73: Xuat Image sang unc path ko thanh cong: " + ex.ToString());
                                                                    }
                                                                }
                                                                #endregion
                                                                #region Export Preview
                                                                if (tempConfig.FileType.Contains("Preview"))
                                                                {
                                                                    try
                                                                    {
                                                                        var audioDb = db.Query<View.FileObj>(_sqlQueryPreview, new { id_clip = postId }).FirstOrDefault();
                                                                        if (audioDb != null)
                                                                        {
                                                                            //unc
                                                                            bool uncSuccess = false;
                                                                            try
                                                                            {
                                                                                File.Copy(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.HD_CLIP), Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.HD_CLIP)));
                                                                                uncSuccess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat Preview sang unc path tu unc path thanh cong");
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                uncSuccess = false;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat Preview sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                            }
                                                                            //ftp
                                                                            if (!uncSuccess)
                                                                            {
                                                                                try
                                                                                {
                                                                                    using (WebClient ftpClient = new WebClient())
                                                                                    {
                                                                                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + "/" + audioDb.NAS_DATA_PATH + "/" + audioDb.HD_CLIP, Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.HD_CLIP)));
                                                                                        //Ghi log
                                                                                        addLog(_logFile, "Node 73: " + postId.ToString() + " - Xuat Preview sang unc path tu ftp path thanh cong");
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: " + postId.ToString() + " - Xuat Preview sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            addLog(_logFile, "Node 73: " + postId.ToString() + " - Ko tim thay Preview trong he thong.");
                                                                            addToMail("Node 73: " + postId.ToString() + " - Ko tim thay Preview trong he thong.");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 73: " + postId.ToString() + " - Xuat Preview sang unc path ko thanh cong: " + ex.ToString());
                                                                    }
                                                                }
                                                                #endregion

                                                                #region Export Subtitle
                                                                if (tempConfig.FileType.Contains("Subtitle"))
                                                                {
                                                                    try
                                                                    {
                                                                        var audioDb = db.Query<View.FileObj>(_sqlQuerySubtitle, new { id_clip = postId }).FirstOrDefault();
                                                                        if (audioDb != null)
                                                                        {
                                                                            //unc
                                                                            bool uncSuccess = false;
                                                                            try
                                                                            {
                                                                                File.Copy(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                uncSuccess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat Subtitle sang unc path tu unc path thanh cong");
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                uncSuccess = false;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 73: Xuat Subtitle sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                            }
                                                                            //ftp
                                                                            if (!uncSuccess)
                                                                            {
                                                                                try
                                                                                {
                                                                                    using (WebClient ftpClient = new WebClient())
                                                                                    {
                                                                                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + "/" + audioDb.NAS_DATA_PATH + "/" + audioDb.FILE_NAME, Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                        //Ghi log
                                                                                        addLog(_logFile, "Node 73: Xuat Subtitle sang unc path tu ftp path thanh cong");
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 73: Xuat Subtitle sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            addLog(_logFile, "Node 73: Ko tim thay Subtitle trong he thong.");
                                                                            addToMail("Node 73: Ko tim thay Subtitle trong he thong.");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 73: Xuat Subtitle sang unc path ko thanh cong: " + ex.ToString());
                                                                    }
                                                                }
                                                                #endregion
                                                                #region Export XML
                                                                if (tempConfig.FileType.Contains("XML"))
                                                                {
                                                                    try
                                                                    {
                                                                        var metaDb = db.Query<View.metaObj>(_sqlQueryMetadata, new { post_id = postId }).FirstOrDefault();

                                                                        View.XMLChildObject xmlChild = new View.XMLChildObject()
                                                                        {
                                                                            fileID = postId,
                                                                            FileName = metaDb.FILE_NAME,
                                                                            FileOnNas = metaDb.NAS_ID,
                                                                            AFD = metaDb.AFD_TYPE,
                                                                            CensorMan = metaDb.CHECKER_USER_ID,
                                                                            CensorTime = metaDb.CHECK_DATE,
                                                                            MaBang = metaDb.MA_BANG,
                                                                            TransferStatus = metaDb.FILE_TRANS_STATUS,
                                                                            Resolution = metaDb.VIDEO_FORMAT,
                                                                            Duration = (TimeCode.TimeCodeFromString(metaDb.REAL_TC_OUT) - TimeCode.TimeCodeFromString(metaDb.REAL_TC_IN))
                                                                        };
                                                                        View.XMLObject xmlObject = new View.XMLObject();
                                                                        xmlObject.GenerateXml(xmlChild);
                                                                        var temp = Path.Combine(localUploadFolder, OriginalFileName + ".xml");
                                                                        xmlObject.SaveXmlFile(temp);
                                                                        addLog(_logFile, "Node 73: Xuat xml sang unc path tu unc path thanh cong");
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 73: Xuat xml sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                    }

                                                                }
                                                                #endregion

                                                            }
                                                            catch (Exception ex) { addLog(_logFile, "Node 73: Loi khi export tu nas sang unc path: " + ex.ToString()); }
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        addLog(_logFile, "Node 73: Khong tim duoc file trong he thong");
                                                        addToMail("Node 73: Ko tim thay file can xuat trong he thong.");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    addLog(_logFile, "Node 73: Loi khi thuc hien task " + postId.ToString() + ": " + ex.ToString());
                                                }
                                            }
                                            db.Execute(_sqlUpdateQuery, new { nodeId = 73, id_clip = postId });
                                            if (errorStr.Length > 0)
                                            {
                                                try
                                                {
                                                    db.Execute(_sqlQueryMail, new
                                                    {
                                                        mail = txtEmail.Text,
                                                        content = errorStr,
                                                        date = DateTime.Now,
                                                        title = "Node 73: Thông tin xuất file"
                                                    });
                                                }
                                                catch (Exception ex)
                                                {
                                                    addLog(_logFile, "Node 73: Loi khi gui mail: " + ex.ToString());
                                                }
                                                errorStr = "";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        addLog(_logFile, "Node 73: Khong co file can xuat");
                                        strProcessing73 = "Không có file cần xuất";
                                        strWaiting73 = "Không có file trong hàng đợi";
                                    }
                                }
                                else
                                {
                                    addLog(_logFile, "Node 73: Chua co lenh Query EXPORT_JOB");
                                }
                            });
                            _task[0].Wait();

                            _task[1] = Task.Run(() =>
                            {
                                if (_sqlQuery != "" && _sqlQuery != null)
                                {
                                    var lstExportDB = db.Query<long>(_sqlQuery, new
                                    {
                                        nodeId = 74
                                    }).ToList();
                                    if (lstExportDB.Count > 0)
                                    {
                                        strWaiting74 = "";
                                        foreach (var i in lstExportDB)
                                        {
                                            strWaiting74 += i.ToString() + ", ";
                                        }
                                        foreach (var postId in lstExportDB)
                                        {
                                            strProcessing74 = postId.ToString();
                                            strWaiting74 = strWaiting74.Replace(strProcessing74, "");
                                            foreach (var tempConfig in lstConfig)
                                            {

                                                try
                                                {
                                                    var filesDB = db.Query<View.FileObj>(_sqlQueryHighresFile, new { id_clip = postId }).FirstOrDefault();
                                                    if (filesDB != null)
                                                    {
                                                        #region FileName
                                                        var ProgramName = filesDB.TEN_CHUONG_TRINH == null ? " " : filesDB.TEN_CHUONG_TRINH;
                                                        var MaBang = filesDB.MA_BANG == null ? " " : filesDB.MA_BANG;
                                                        var creatDate = DateTime.Now;//filesDB.CREATE_DATE == null ? DateTime.Now : filesDB.CREATE_DATE;
                                                        var broadcastDate = DateTime.Now;//filesDB.DATE_TO_BROADCAST == null ? DateTime.Now : filesDB.DATE_TO_BROADCAST;
                                                        var startRight = filesDB.START_RIGHTS == null ? DateTime.Now : filesDB.START_RIGHTS;
                                                        var endRight = filesDB.END_RIGHTS == null ? DateTime.Now.AddYears(1) : filesDB.END_RIGHTS;
                                                        var season = filesDB.Season == null ? " " : filesDB.Season;
                                                        var episode = filesDB.EPISODE_NUMBER == null ? 0 : filesDB.EPISODE_NUMBER;
                                                        string typeDescription = filesDB.TYPE_DESCRIPTION == null ? "Others" : Utils.ConvertToVietnameseNonSign(filesDB.TYPE_DESCRIPTION.Replace("(", "_").Replace(")", "_").Replace("/", "_"));

                                                        string OriginalFileName = tempConfig.FileName.Contains("Mã băng") ? Utils.ConvertToVietnameseNonSign(MaBang.Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim()) : " ";
                                                        if (tempConfig.FileName.Contains("Tên chương trình"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(ProgramName).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim();
                                                        }
                                                        if (tempConfig.FileName.Contains("Ngày tháng phát sóng"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(broadcastDate.ToString("yyyy-MM-dd HH:mm:ss")).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim();
                                                        }
                                                        if (tempConfig.FileName.Contains("Ngày tháng tạo vỏ"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(creatDate.ToString("yyyy-MM-dd HH:mm:ss")).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim();
                                                        }
                                                        if (tempConfig.FileName.Contains("Phần"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(season).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace(":", "_").Replace("\\", "_").Replace("/", "_").Trim();
                                                        }
                                                        if (tempConfig.FileName.Contains("Tập"))
                                                        {
                                                            OriginalFileName += "_" + Utils.ConvertToVietnameseNonSign(episode.ToString()).Replace(" ", "").Replace("-", "_").Replace("*", "_").Replace("\'", "_").Replace("\\", "_").Replace(":", "_").Replace("/", "_").Trim();
                                                        }
                                                        var tempHighres = filesDB.FILE_NAME == null ? " " : filesDB.FILE_NAME;
                                                        var tempThumbPicture = filesDB.THUMB_FILE_NAME == null ? " " : filesDB.THUMB_FILE_NAME;
                                                        var tempLowres = filesDB.HD_CLIP == null ? " " : filesDB.HD_CLIP;
                                                        var nasHighresPath = filesDB.DATA1_DIRECTORY == null ? " " : filesDB.DATA1_DIRECTORY;
                                                        var uncHighresPath = filesDB.UNC_BASE_PATH_DATA1 == null ? " " : filesDB.UNC_BASE_PATH_DATA1;
                                                        var nasLowresPath = filesDB.DATA3_DIRECTORY == null ? " " : filesDB.DATA3_DIRECTORY;
                                                        var uncLowresPath = filesDB.UNC_BASE_PATH_DATA3 == null ? " " : filesDB.UNC_BASE_PATH_DATA3;
                                                        string addSymbol = tempConfig.AddSymBol.Contains("Đầu tên file") ? tempConfig.AddSymBol.Replace("Đầu tên file:", "").Trim() : tempConfig.AddSymBol.Replace("Cuối tên file:", "").Trim();
                                                        string srcPath = "ftp://" + filesDB.NAS_IP + ":" + filesDB.PORT;
                                                        string ftpUploadFolder = tempConfig.NasPath + typeDescription + "/";
                                                        string localUploadFolder = Path.Combine(tempConfig.LocalPath, typeDescription);

                                                        if (tempConfig.AddSymBol.Contains("Đầu tên file"))
                                                        {
                                                            OriginalFileName = addSymbol + OriginalFileName;
                                                        }
                                                        if (tempConfig.AddSymBol.Contains("Cuối tên file"))
                                                        {
                                                            OriginalFileName += addSymbol;
                                                        }
                                                        if (tempConfig.NasPath.Length > 7)
                                                            if (!ftpDirectoryExists(typeDescription, tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            {
                                                                if (!createFTPDirectory(tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass, typeDescription))
                                                                {
                                                                    addLog(_logFile, "Node 74: Tao thu muc " + typeDescription + " tren nas khong thanh cong");
                                                                    ftpUploadFolder = tempConfig.NasPath;
                                                                }
                                                            }
                                                        #endregion
                                                        #region Export từ nas này sang nas khác
                                                        if (tempConfig.NasPath.Length > 7)
                                                        {

                                                            #region Export Media Highres
                                                            //bool uncHighresSuccess = false;
                                                            //if (tempConfig.FileType.Contains("Media Highres") && uncHighresPath != null && uncHighresPath.Length > 0)
                                                            //{
                                                            //    try
                                                            //    {
                                                            //        if (uploadFromUnc(uncHighresPath + "\\" + tempHighres, OriginalFileName + ".mxf", tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //        {
                                                            //            uncHighresSuccess = true;
                                                            //            //Ghi log
                                                            //            addLog(_logFile, "Node 74: Xuat highres bang unc thanh cong");
                                                            //        }
                                                            //        else
                                                            //        {
                                                            //            //Ghi log
                                                            //            addLog(_logFile, "Node 74: Ko xuat dc highres bang unc");
                                                            //        }
                                                            //    }
                                                            //    catch (Exception ex) { addLog(_logFile, "Node 74: Loi khi xuat highres bang unc: " + ex.ToString()); }
                                                            //}
                                                            //if (tempConfig.FileType.Contains("Media Highres") && nasHighresPath != null && nasHighresPath.Length > 0 && !uncHighresSuccess)
                                                            //{
                                                            //    try
                                                            //    {
                                                            //        var tempSourcePath = srcPath + nasHighresPath;
                                                            //        if (copyFile(tempHighres, OriginalFileName + ".mxf", tempSourcePath, filesDB.USERNAME, filesDB.PASSWORD, tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //        {
                                                            //            //Ghi log
                                                            //            addLog(_logFile, "Node 74: Xuat highres bang ftp thanh cong");
                                                            //        }
                                                            //        else
                                                            //        {
                                                            //            //Ghi log
                                                            //            addLog(_logFile, "Node 74: Ko xuat dc highres bang ftp");
                                                            //        }
                                                            //    }
                                                            //    catch (Exception ex) { addLog(_logFile, "Node 74: Loi khi xuat highres bang ftp: " + ex.ToString()); }
                                                            //}
                                                            #endregion

                                                            #region Export Media Lowres
                                                            bool uncLowresSucess = false;
                                                            if (tempConfig.FileType.Contains("Lowres doc"))
                                                            {
                                                                try
                                                                {
                                                                    var lowresDb = db.Query<View.FileObj>(_sqlQueryLowresFile, new { id_clip = postId }).FirstOrDefault();
                                                                    string srcLowresPath = "ftp://" + lowresDb.NAS_IP + ":" + lowresDb.PORT;
                                                                    if (lowresDb != null)
                                                                    {
                                                                        //unc
                                                                        try
                                                                        {
                                                                            if (uploadFromUnc(Path.Combine(lowresDb.UNC_BASE_PATH_DATA3, lowresDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(lowresDb.FILE_NAME), ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                            {
                                                                                uncLowresSucess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat lowres bang unc thanh cong");
                                                                            }
                                                                            else
                                                                            {
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat lowres bang unc ko thanh cong");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            addLog(_logFile, "Node 74: Xuat lowres bang unc ko thanh cong: " + ex.ToString());
                                                                        }
                                                                        //nas
                                                                        if (!uncLowresSucess)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (copyFile(lowresDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(lowresDb.FILE_NAME), srcLowresPath + lowresDb.DATA3_DIRECTORY, lowresDb.USERNAME, lowresDb.PASSWORD, ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat lowres bang ftp thanh cong");
                                                                                }
                                                                                else
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat lowres bang ftp ko thanh cong" + " Path: " + srcLowresPath);
                                                                                }
                                                                            }
                                                                            catch (Exception ex) { addLog(_logFile, "Node 74: Xuat lowres bang ftp ko thanh cong: " + ex.ToString()); }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        addLog(_logFile, "Node 74: Ko có lowres de xuat bang ftp");
                                                                        addToMail("Node 74: Ko có lowres trong he thong");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 74: Loi khi xuat lowres: " + ex.ToString()); }
                                                            }
                                                            #endregion
                                                            #region Export Highres original
                                                            bool uncHighresOriginalSucess = false;
                                                            if (tempConfig.FileType.Contains("Highres Original"))
                                                            {
                                                                try
                                                                {
                                                                    var audioDb = db.Query<View.FileObj>(_sqlQueryHighresOriginalFile, new { id_clip = postId }).FirstOrDefault();
                                                                    string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                                    if (audioDb != null)
                                                                    {
                                                                        //unc
                                                                        try
                                                                        {
                                                                            if (uploadFromUnc(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                            {
                                                                                uncHighresOriginalSucess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat highres original bang unc thanh cong");
                                                                            }
                                                                            else
                                                                            {
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat highres original bang unc ko thanh cong");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            addLog(_logFile, "Node 74: Xuat highres original bang unc ko thanh cong: " + ex.ToString());
                                                                        }
                                                                        //nas
                                                                        if (!uncHighresOriginalSucess)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (copyFile(audioDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.NAS_DATA_PATH), audioDb.USERNAME, audioDb.PASSWORD, ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat highres original bang ftp thanh cong");
                                                                                }
                                                                                else
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat highres original bang ftp ko thanh cong");
                                                                                }
                                                                            }
                                                                            catch (Exception ex) { addLog(_logFile, "Node 74: Xuat highres original bang ftp ko thanh cong: " + ex.ToString()); }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        addLog(_logFile, "Node 74: Ko có highres original de xuat bang ftp");
                                                                        addToMail("Node 74: Ko có highres original trong he thong");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 74: Loi khi xuat highres original: " + ex.ToString()); }
                                                            }
                                                            #endregion

                                                            #region Export Image
                                                            //bool uncImageSucess = false;
                                                            //if (tempConfig.FileType.Contains("Image"))
                                                            //{
                                                            //    try
                                                            //    {
                                                            //        var audioDb = db.Query<View.FileObj>(_sqlQueryImage, new { id_clip = postId }).FirstOrDefault();
                                                            //        string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                            //        if (audioDb != null)
                                                            //        {
                                                            //            //unc
                                                            //            try
                                                            //            {
                                                            //                if (uploadFromUnc(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                {
                                                            //                    uncImageSucess = true;
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 74: Xuat image original bang unc thanh cong");
                                                            //                }
                                                            //                else
                                                            //                {
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 74: Xuat image original bang unc ko thanh cong");
                                                            //                }
                                                            //            }
                                                            //            catch (Exception ex)
                                                            //            {
                                                            //                addLog(_logFile, "Node 74: Xuat image original bang unc ko thanh cong: " + ex.ToString());
                                                            //            }
                                                            //            //nas
                                                            //            if (!uncImageSucess)
                                                            //            {
                                                            //                try
                                                            //                {
                                                            //                    if (copyFile(audioDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.NAS_DATA_PATH), audioDb.USERNAME, audioDb.PASSWORD, tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 74: Xuat image original bang ftp thanh cong");
                                                            //                    }
                                                            //                    else
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 74: Xuat image original bang ftp ko thanh cong");
                                                            //                    }
                                                            //                }
                                                            //                catch (Exception ex) { addLog(_logFile, "Node 74: Xuat image original bang ftp ko thanh cong: " + ex.ToString()); }
                                                            //            }
                                                            //        }
                                                            //        else
                                                            //        {
                                                            //            addLog(_logFile, "Node 74: Ko có image de xuat bang ftp");
                                                            //            addToMail("Node 74: Ko có image trong he thong");
                                                            //        }
                                                            //    }
                                                            //    catch (Exception ex) { addLog(_logFile, "Node 74: Loi khi xuat lowres original: " + ex.ToString()); }
                                                            //}
                                                            #endregion
                                                            #region Export Preview
                                                            //bool uncPreviewSucess = false;
                                                            //if (tempConfig.FileType.Contains("Preview"))
                                                            //{
                                                            //    try
                                                            //    {
                                                            //        var audioDb = db.Query<View.FileObj>(_sqlQueryPreview, new { id_clip = postId }).FirstOrDefault();
                                                            //        string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                            //        if (audioDb != null)
                                                            //        {
                                                            //            //unc
                                                            //            try
                                                            //            {
                                                            //                if (uploadFromUnc(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.HD_CLIP), OriginalFileName + "." + Path.GetExtension(audioDb.HD_CLIP), tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                {
                                                            //                    uncPreviewSucess = true;
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 74: Xuat Preview original bang unc thanh cong");
                                                            //                }
                                                            //                else
                                                            //                {
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 74: Xuat Preview original bang unc ko thanh cong");
                                                            //                }
                                                            //            }
                                                            //            catch (Exception ex)
                                                            //            {
                                                            //                addLog(_logFile, "Node 74: Xuat Preview original bang unc ko thanh cong: " + ex.ToString());
                                                            //            }
                                                            //            //nas
                                                            //            if (!uncPreviewSucess)
                                                            //            {
                                                            //                try
                                                            //                {
                                                            //                    if (copyFile(audioDb.HD_CLIP, OriginalFileName + "." + Path.GetExtension(audioDb.HD_CLIP), Path.Combine(srcAudioPath, audioDb.NAS_DATA_PATH), audioDb.USERNAME, audioDb.PASSWORD, tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 74: Xuat Preview original bang ftp thanh cong");
                                                            //                    }
                                                            //                    else
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 74: Xuat Preview original bang ftp ko thanh cong");
                                                            //                    }
                                                            //                }
                                                            //                catch (Exception ex) { addLog(_logFile, "Node 74: Xuat Preview original bang ftp ko thanh cong: " + ex.ToString()); }
                                                            //            }
                                                            //        }
                                                            //        else
                                                            //        {
                                                            //            addLog(_logFile, "Node 74: " + postId.ToString() + " - Ko có Preview original de xuat bang ftp");
                                                            //            addToMail("Node 74: " + postId.ToString() + " - Ko có Preview trong he thong");
                                                            //        }
                                                            //    }
                                                            //    catch (Exception ex) { addLog(_logFile, "Node 74: " + postId.ToString() + " - Loi khi xuat Preview original: " + ex.ToString()); }
                                                            //}
                                                            #endregion

                                                            #region Export audio
                                                            bool uncPicSucess = false;
                                                            if (tempConfig.FileType.Contains("Audio Original"))
                                                            {
                                                                try
                                                                {
                                                                    var audioDb = db.Query<View.AudioObj>(_sqlQueryAudio, new { id_clip = postId }).FirstOrDefault();
                                                                    string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                                    if (audioDb != null)
                                                                    {
                                                                        //unc
                                                                        try
                                                                        {
                                                                            if (uploadFromUnc(Path.Combine(audioDb.UNC_BASE_PATH_DATA3, audioDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                            {
                                                                                uncPicSucess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat audio Original bang unc thanh cong");
                                                                            }
                                                                            else
                                                                            {
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat audio Original bang unc ko thanh cong");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            addLog(_logFile, "Node 74: Xuat audio Original bang unc ko thanh cong: " + ex.ToString());
                                                                        }
                                                                        //nas
                                                                        if (!uncPicSucess)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (copyFile(audioDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.DATA3_DIRECTORY), audioDb.USERNAME, audioDb.PASSWORD, ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat audio Original bang ftp thanh cong");
                                                                                }
                                                                                else
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat audio Original bang ftp ko thanh cong");
                                                                                }
                                                                            }
                                                                            catch (Exception ex) { addLog(_logFile, "Node 74: Xuat audio Original bang ftp ko thanh cong: " + ex.ToString()); }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        addLog(_logFile, "Node 74: Ko có audio Original de xuat bang ftp");
                                                                        addToMail("Node 74: Ko có audio Original trong he thong");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 74: Loi khi xuat audio Original: " + ex.ToString()); }
                                                            }
                                                            #endregion
                                                            #region Export audio original
                                                            //bool uncAuOriginalSucess = false;
                                                            //if (tempConfig.FileType.Contains("Audio Original"))
                                                            //{
                                                            //    try
                                                            //    {
                                                            //        var audioDb = db.Query<View.AudioObj>(_sqlQueryAudioOriginal, new { id_clip = postId }).FirstOrDefault();
                                                            //        string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                            //        if (audioDb != null)
                                                            //        {
                                                            //            //unc
                                                            //            try
                                                            //            {
                                                            //                if (uploadFromUnc(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                {
                                                            //                    uncAuOriginalSucess = true;
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 74: Xuat audio original bang unc thanh cong");
                                                            //                }
                                                            //                else
                                                            //                {
                                                            //                    //Ghi log
                                                            //                    addLog(_logFile, "Node 74: Xuat audio original bang unc ko thanh cong");
                                                            //                }
                                                            //            }
                                                            //            catch (Exception ex)
                                                            //            {
                                                            //                addLog(_logFile, "Node 74: Xuat audio original bang unc ko thanh cong: " + ex.ToString());
                                                            //            }
                                                            //            //nas
                                                            //            if (!uncAuOriginalSucess)
                                                            //            {
                                                            //                try
                                                            //                {
                                                            //                    if (copyFile(audioDb.FILE_NAME, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.NAS_DATA_PATH), audioDb.USERNAME, audioDb.PASSWORD, tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 74: Xuat audio original bang ftp thanh cong");
                                                            //                    }
                                                            //                    else
                                                            //                    {
                                                            //                        //Ghi log
                                                            //                        addLog(_logFile, "Node 74: Xuat audio original bang ftp ko thanh cong");
                                                            //                    }
                                                            //                }
                                                            //                catch (Exception ex) { addLog(_logFile, "Node 74: Xuat audio original bang ftp ko thanh cong: " + ex.ToString()); }
                                                            //            }
                                                            //        }
                                                            //        else
                                                            //        {
                                                            //            addLog(_logFile, "Node 74: Ko có audio original de xuat bang ftp");
                                                            //            addToMail("Node 74: Ko có audio original trong he thong");
                                                            //        }
                                                            //    }
                                                            //    catch (Exception ex) { addLog(_logFile, "Node 74: Loi khi xuat audio original: " + ex.ToString()); }
                                                            //}
                                                            #endregion

                                                            #region Export XML
                                                            //if (tempConfig.FileType.Contains("XML"))
                                                            //{

                                                            //    try
                                                            //    {
                                                            //        var metaDb = db.Query<View.metaObj>(_sqlQueryMetadata, new { id_clip = postId, post_id = postId }).ToList();
                                                            //        string metaStr = "";
                                                            //        if (metaDb != null)
                                                            //        {
                                                            //            foreach (var tempMeta in metaDb)
                                                            //            {
                                                            //                metaStr += "Node: " + tempMeta.NODE_WORK_ID + " - " + "Date: " + tempMeta.CHECK_DATE + " - " + "User: " + tempMeta.CHECKER_USER_ID + "; ";
                                                            //            }
                                                            //        }
                                                            //        else { metaStr = "Khong co thong tin ve WorkFlow cua file nay!"; }
                                                            //        View.XMLChildObject xmlChild = new View.XMLChildObject()
                                                            //        {
                                                            //            rootID = "Info",
                                                            //            rootScheduleDate = metaStr
                                                            //        };
                                                            //        View.XMLObject xmlObject = new View.XMLObject();
                                                            //        xmlObject.GenerateXml(xmlChild);
                                                            //        var temp = Path.Combine(txtSaveFolder.Text, OriginalFileName + ".xml");
                                                            //        xmlObject.SaveXmlFile(temp);
                                                            //        addLog(_logFile, "Node 74: Xuat xml sang unc path tu nas thanh cong");
                                                            //        try
                                                            //        {
                                                            //            if (uploadFromUnc(temp, OriginalFileName + ".xml", tempConfig.NasPath, tempConfig.NasUsername, tempConfig.NasPass))
                                                            //            {
                                                            //                addLog(_logFile, "Node 74: Xuat xml len ftp bang unc thanh cong");
                                                            //                File.Delete(temp);
                                                            //            }
                                                            //            else
                                                            //            {
                                                            //                //Ghi log
                                                            //                addLog(_logFile, "Node 74: Xuat xml len ftp bang unc ko thanh cong");
                                                            //            }
                                                            //        }
                                                            //        catch (Exception ex)
                                                            //        {
                                                            //            addLog(_logFile, "Node 74: Xuat xml len ftp bang unc ko thanh cong: " + ex.ToString());
                                                            //        }
                                                            //    }
                                                            //    catch (Exception ex)
                                                            //    {
                                                            //        addLog(_logFile, "Node 74: Xuat xml sang ftp path tu nas ko thanh cong: " + ex.ToString());
                                                            //    }
                                                            //}
                                                            #endregion
                                                            #region Export XML Original
                                                            bool uncXmlOriginalSucess = false;
                                                            if (tempConfig.FileType.Contains("XML Original"))
                                                            {
                                                                try
                                                                {
                                                                    var audioDb = db.Query<View.AudioObj>(_sqlQueryXmlOriginal, new { id_clip = postId }).FirstOrDefault();
                                                                    string srcAudioPath = "ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT;
                                                                    if (audioDb != null)
                                                                    {
                                                                        //unc
                                                                        try
                                                                        {
                                                                            if (uploadFromUnc(Path.Combine(audioDb.UNC_BASE_PATH_DATA3, audioDb.FILE_NAME), OriginalFileName + "_Original." + Path.GetExtension(audioDb.FILE_NAME), ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                            {
                                                                                uncXmlOriginalSucess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat XML Original bang unc thanh cong");
                                                                            }
                                                                            else
                                                                            {
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat XML Original bang unc ko thanh cong");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            addLog(_logFile, "Node 74: Xuat XML Original bang unc ko thanh cong: " + ex.ToString());
                                                                        }
                                                                        //nas
                                                                        if (!uncXmlOriginalSucess)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (copyFile(audioDb.FILE_NAME, OriginalFileName + "_Original." + Path.GetExtension(audioDb.FILE_NAME), Path.Combine(srcAudioPath, audioDb.DATA3_DIRECTORY), audioDb.USERNAME, audioDb.PASSWORD, ftpUploadFolder, tempConfig.NasUsername, tempConfig.NasPass))
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat XML Original bang ftp thanh cong");
                                                                                }
                                                                                else
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat XML Original bang ftp ko thanh cong");
                                                                                }
                                                                            }
                                                                            catch (Exception ex) { addLog(_logFile, "Node 74: Xuat XML Original bang ftp ko thanh cong: " + ex.ToString()); }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        addLog(_logFile, "Node 74: Ko có XML Original de xuat bang ftp");
                                                                        addToMail("Node 74: Ko có XML Original trong he thong");
                                                                    }
                                                                }
                                                                catch (Exception ex) { addLog(_logFile, "Node 74: Loi khi xuat XML Original: " + ex.ToString()); }
                                                            }
                                                            #endregion

                                                        }
                                                        #endregion

                                                        #region Export từ nas sang unc path
                                                        if (tempConfig.LocalPath.Length > 0)
                                                        {
                                                            try
                                                            {
                                                                if (!Directory.Exists(localUploadFolder))
                                                                {
                                                                    Directory.CreateDirectory(localUploadFolder);
                                                                }

                                                                #region copy highres
                                                                //if (tempConfig.FileType.Contains("Media Highres"))
                                                                //{
                                                                //    //unc
                                                                //    bool uncSuccess = false;
                                                                //    try
                                                                //    {
                                                                //        var t = Task.Run(() =>
                                                                //        {
                                                                //            uncSuccess = FCopy(uncHighresPath + "\\" + tempHighres, Path.Combine(tempConfig.LocalPath, OriginalFileName + ".mxf"));
                                                                //        });
                                                                //        t.Wait();
                                                                //        if (uncSuccess)
                                                                //        {
                                                                //            //Ghi log
                                                                //            addLog(_logFile, "Node 74: Xuat highres sang unc path tu unc path thanh cong");
                                                                //        }
                                                                //        else
                                                                //        {
                                                                //            addLog(_logFile, "Node 74: Xuat highres sang unc path tu unc path ko thanh cong");
                                                                //        }
                                                                //    }
                                                                //    catch (Exception ex)
                                                                //    {
                                                                //        uncSuccess = false;
                                                                //        //Ghi log
                                                                //        addLog(_logFile, "Node 74: Xuat highres sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                //    }
                                                                //    //ftp
                                                                //    if (!uncSuccess)
                                                                //    {
                                                                //        try
                                                                //        {
                                                                //            var tempSourcePath = srcPath + nasHighresPath;
                                                                //            using (WebClient ftpClient = new WebClient())
                                                                //            {
                                                                //                ftpClient.Credentials = new NetworkCredential(filesDB.USERNAME, filesDB.PASSWORD);
                                                                //                var t = Task.Run(() =>
                                                                //                {
                                                                //                    try
                                                                //                    {
                                                                //                        ftpClient.DownloadFile(tempSourcePath, Path.Combine(tempConfig.LocalPath, OriginalFileName + ".mxf"));
                                                                //                    //Ghi log
                                                                //                    addLog(_logFile, "Node 74: Xuat highres sang unc path tu ftp path thanh cong");
                                                                //                    }
                                                                //                    catch (Exception ex) { addLog(_logFile, "Node 74: Loi xuat highres sang unc path tu ftp path: " + ex.ToString()); }
                                                                //                });
                                                                //                t.Wait();

                                                                //            }
                                                                //        }
                                                                //        catch (Exception ex)
                                                                //        {
                                                                //            //Ghi log
                                                                //            addLog(_logFile, "Node 74: Xuat highres sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                //        }
                                                                //    }
                                                                //}
                                                                #endregion

                                                                #region copy lowres
                                                                if (tempConfig.FileType.Contains("Lowres doc"))
                                                                {
                                                                    try
                                                                    {
                                                                        var audioDb = db.Query<View.FileObj>(_sqlQueryLowresFile, new { id_clip = postId }).FirstOrDefault();
                                                                        if (audioDb != null)
                                                                        {
                                                                            //unc
                                                                            bool uncSuccess = false;
                                                                            try
                                                                            {
                                                                                File.Copy(Path.Combine(audioDb.UNC_BASE_PATH_DATA3, audioDb.FILE_NAME), Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                uncSuccess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat lowres sang unc path tu unc path thanh cong");
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                uncSuccess = false;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat lowres sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                            }
                                                                            //ftp
                                                                            if (!uncSuccess)
                                                                            {
                                                                                try
                                                                                {
                                                                                    using (WebClient ftpClient = new WebClient())
                                                                                    {
                                                                                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + audioDb.DATA3_DIRECTORY + "/" + audioDb.FILE_NAME, Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                        //Ghi log
                                                                                        addLog(_logFile, "Node 74: Xuat lowres sang unc path tu ftp path thanh cong");
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat lowres sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            addLog(_logFile, "Node 74: Ko co file lowes trong he thong khi xuat sang unc");
                                                                            addToMail("Node 74: Ko co file lowes trong he thong");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 74: Xuat lowres sang unc path ko thanh cong: " + ex.ToString());
                                                                    }
                                                                }
                                                                #endregion
                                                                #region Export highres original
                                                                if (tempConfig.FileType.Contains("Highres Original"))
                                                                {
                                                                    try
                                                                    {
                                                                        var audioDb = db.Query<View.FileObj>(_sqlQueryHighresOriginalFile, new { id_clip = postId }).FirstOrDefault();
                                                                        if (audioDb != null)
                                                                        {
                                                                            //unc
                                                                            bool uncSuccess = false;
                                                                            try
                                                                            {
                                                                                File.Copy(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                uncSuccess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat highres original sang unc path tu unc path thanh cong");
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                uncSuccess = false;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat highres original sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                            }
                                                                            //ftp
                                                                            if (!uncSuccess)
                                                                            {
                                                                                try
                                                                                {
                                                                                    using (WebClient ftpClient = new WebClient())
                                                                                    {
                                                                                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + "/" + audioDb.NAS_DATA_PATH + "/" + audioDb.FILE_NAME, Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                        //Ghi log
                                                                                        addLog(_logFile, "Node 74: Xuat highres original sang unc path tu ftp path thanh cong");
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat highres original sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            addLog(_logFile, "Node 74: Ko tim thay highres original trong he thong.");
                                                                            addToMail("Node 74: Ko tim thay highres original trong he thong.");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 74: Xuat highres original sang unc path ko thanh cong: " + ex.ToString());
                                                                    }
                                                                }
                                                                #endregion

                                                                #region Export audio
                                                                if (tempConfig.FileType.Contains("Audio Original"))
                                                                {
                                                                    try
                                                                    {
                                                                        var audioDb = db.Query<View.AudioObj>(_sqlQueryAudio, new { id_clip = postId }).FirstOrDefault();
                                                                        if (audioDb != null)
                                                                        {
                                                                            //unc
                                                                            bool uncSuccess = false;
                                                                            try
                                                                            {
                                                                                File.Copy(Path.Combine(audioDb.UNC_BASE_PATH_DATA3, audioDb.FILE_NAME), Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                uncSuccess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat audio Original sang unc path tu unc path thanh cong");
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                uncSuccess = false;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat audio Original sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                            }
                                                                            //ftp
                                                                            if (!uncSuccess)
                                                                            {
                                                                                try
                                                                                {
                                                                                    using (WebClient ftpClient = new WebClient())
                                                                                    {
                                                                                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + audioDb.DATA3_DIRECTORY + "/" + audioDb.FILE_NAME, Path.Combine(localUploadFolder, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                        //Ghi log
                                                                                        addLog(_logFile, "Node 74: Xuat audio Original sang unc path tu ftp path thanh cong");
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat audio Original sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            addLog(_logFile, "Node 74: Ko tim thay audio Original trong he thong.");
                                                                            addToMail("Node 74: Ko tim thay audio Original trong he thong.");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 74: Xuat audio Original sang unc path ko thanh cong: " + ex.ToString());
                                                                    }
                                                                }
                                                                #endregion
                                                                #region Export audio original
                                                                //if (tempConfig.FileType.Contains("Audio Original"))
                                                                //{
                                                                //    try
                                                                //    {
                                                                //        var audioDb = db.Query<View.AudioObj>(_sqlQueryAudioOriginal, new { id_clip = postId }).FirstOrDefault();
                                                                //        if (audioDb != null)
                                                                //        {
                                                                //            //unc
                                                                //            bool uncSuccess = false;
                                                                //            try
                                                                //            {
                                                                //                File.Copy(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                uncSuccess = true;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 74: Xuat audio original sang unc path tu unc path thanh cong");
                                                                //            }
                                                                //            catch (Exception ex)
                                                                //            {
                                                                //                uncSuccess = false;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 74: Xuat audio original sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                //            }
                                                                //            //ftp
                                                                //            if (!uncSuccess)
                                                                //            {
                                                                //                try
                                                                //                {
                                                                //                    using (WebClient ftpClient = new WebClient())
                                                                //                    {
                                                                //                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                //                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + "/" + audioDb.NAS_DATA_PATH + "/" + audioDb.FILE_NAME, Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                        //Ghi log
                                                                //                        addLog(_logFile, "Node 74: Xuat audio original sang unc path tu ftp path thanh cong");
                                                                //                    }
                                                                //                }
                                                                //                catch (Exception ex)
                                                                //                {
                                                                //                    //Ghi log
                                                                //                    addLog(_logFile, "Node 74: Xuat audio original sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                //                }
                                                                //            }
                                                                //        }
                                                                //        else
                                                                //        {
                                                                //            addLog(_logFile, "Node 74: Ko tim thay audio original trong he thong.");
                                                                //            addToMail("Node 74: Ko tim thay audio original trong he thong.");
                                                                //        }
                                                                //    }
                                                                //    catch (Exception ex)
                                                                //    {
                                                                //        addLog(_logFile, "Node 74: Xuat audio original sang unc path ko thanh cong: " + ex.ToString());
                                                                //    }
                                                                //}
                                                                #endregion

                                                                #region Export Image
                                                                //if (tempConfig.FileType.Contains("Image"))
                                                                //{
                                                                //    try
                                                                //    {
                                                                //        var audioDb = db.Query<View.AudioObj>(_sqlQueryImage, new { id_clip = postId }).FirstOrDefault();
                                                                //        if (audioDb != null)
                                                                //        {
                                                                //            //unc
                                                                //            bool uncSuccess = false;
                                                                //            try
                                                                //            {
                                                                //                File.Copy(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.FILE_NAME), Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                uncSuccess = true;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 74: Xuat Image sang unc path tu unc path thanh cong");
                                                                //            }
                                                                //            catch (Exception ex)
                                                                //            {
                                                                //                uncSuccess = false;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 74: Xuat Image sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                //            }
                                                                //            //ftp
                                                                //            if (!uncSuccess)
                                                                //            {
                                                                //                try
                                                                //                {
                                                                //                    using (WebClient ftpClient = new WebClient())
                                                                //                    {
                                                                //                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                //                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + "/" + audioDb.NAS_DATA_PATH + "/" + audioDb.FILE_NAME, Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                //                        //Ghi log
                                                                //                        addLog(_logFile, "Node 74: Xuat Image sang unc path tu ftp path thanh cong");
                                                                //                    }
                                                                //                }
                                                                //                catch (Exception ex)
                                                                //                {
                                                                //                    //Ghi log
                                                                //                    addLog(_logFile, "Node 74: Xuat Image sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                //                }
                                                                //            }
                                                                //        }
                                                                //        else
                                                                //        {
                                                                //            addLog(_logFile, "Node 74: Ko tim thay Image trong he thong.");
                                                                //            addToMail("Node 74: Ko tim thay image trong he thong.");
                                                                //        }
                                                                //    }
                                                                //    catch (Exception ex)
                                                                //    {
                                                                //        addLog(_logFile, "Node 74: Xuat Image sang unc path ko thanh cong: " + ex.ToString());
                                                                //    }
                                                                //}
                                                                #endregion
                                                                #region Export Preview
                                                                //if (tempConfig.FileType.Contains("Preview"))
                                                                //{
                                                                //    try
                                                                //    {
                                                                //        var audioDb = db.Query<View.FileObj>(_sqlQueryPreview, new { id_clip = postId }).FirstOrDefault();
                                                                //        if (audioDb != null)
                                                                //        {
                                                                //            //unc
                                                                //            bool uncSuccess = false;
                                                                //            try
                                                                //            {
                                                                //                File.Copy(Path.Combine(Path.Combine(audioDb.UNC_HOME, audioDb.NAS_DATA_PATH), audioDb.HD_CLIP), Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.HD_CLIP)));
                                                                //                uncSuccess = true;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 74: Xuat Preview sang unc path tu unc path thanh cong");
                                                                //            }
                                                                //            catch (Exception ex)
                                                                //            {
                                                                //                uncSuccess = false;
                                                                //                //Ghi log
                                                                //                addLog(_logFile, "Node 74: Xuat Preview sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                //            }
                                                                //            //ftp
                                                                //            if (!uncSuccess)
                                                                //            {
                                                                //                try
                                                                //                {
                                                                //                    using (WebClient ftpClient = new WebClient())
                                                                //                    {
                                                                //                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                //                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + "/" + audioDb.NAS_DATA_PATH + "/" + audioDb.HD_CLIP, Path.Combine(tempConfig.LocalPath, OriginalFileName + "." + Path.GetExtension(audioDb.HD_CLIP)));
                                                                //                        //Ghi log
                                                                //                        addLog(_logFile, "Node 74: " + postId.ToString() + " - Xuat Preview sang unc path tu ftp path thanh cong");
                                                                //                    }
                                                                //                }
                                                                //                catch (Exception ex)
                                                                //                {
                                                                //                    //Ghi log
                                                                //                    addLog(_logFile, "Node 74: " + postId.ToString() + " - Xuat Preview sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                //                }
                                                                //            }
                                                                //        }
                                                                //        else
                                                                //        {
                                                                //            addLog(_logFile, "Node 74: " + postId.ToString() + " - Ko tim thay Preview trong he thong.");
                                                                //            addToMail("Node 74: " + postId.ToString() + " - Ko tim thay Preview trong he thong.");
                                                                //        }
                                                                //    }
                                                                //    catch (Exception ex)
                                                                //    {
                                                                //        addLog(_logFile, "Node 74: " + postId.ToString() + " - Xuat Preview sang unc path ko thanh cong: " + ex.ToString());
                                                                //    }
                                                                //}
                                                                #endregion

                                                                #region Export XML
                                                                //if (tempConfig.FileType.Contains("XML"))
                                                                //{
                                                                //    try
                                                                //    {
                                                                //        var metaDb = db.Query<View.metaObj>(_sqlQueryMetadata, new { id_clip = postId, post_id = postId }).ToList();
                                                                //        string metaStr = "";
                                                                //        if (metaDb != null)
                                                                //        {
                                                                //            foreach (var tempMeta in metaDb)
                                                                //            {
                                                                //                metaStr += "Node: " + tempMeta.NODE_WORK_ID + " - " + "Date: " + tempMeta.CHECK_DATE + " - " + "User: " + tempMeta.CHECKER_USER_ID + "; ";
                                                                //            }
                                                                //        }
                                                                //        else { metaStr = "Khong co thong tin ve WorkFlow cua file nay!"; }
                                                                //        View.XMLChildObject xmlChild = new View.XMLChildObject()
                                                                //        {
                                                                //            rootID = "Info",
                                                                //            rootScheduleDate = metaStr
                                                                //        };
                                                                //        View.XMLObject xmlObject = new View.XMLObject();
                                                                //        xmlObject.GenerateXml(xmlChild);
                                                                //        var temp = Path.Combine(tempConfig.LocalPath, OriginalFileName + ".xml");
                                                                //        xmlObject.SaveXmlFile(temp);
                                                                //        addLog(_logFile, "Node 74: Xuat xml sang unc path tu unc path thanh cong");
                                                                //    }
                                                                //    catch (Exception ex)
                                                                //    {
                                                                //        addLog(_logFile, "Node 74: Xuat xml sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                //    }

                                                                //}
                                                                #endregion
                                                                #region Export XML Original
                                                                if (tempConfig.FileType.Contains("XML Original"))
                                                                {
                                                                    try
                                                                    {
                                                                        var audioDb = db.Query<View.AudioObj>(_sqlQueryXmlOriginal, new { id_clip = postId }).FirstOrDefault();
                                                                        if (audioDb != null)
                                                                        {
                                                                            //unc
                                                                            bool uncSuccess = false;
                                                                            try
                                                                            {
                                                                                File.Copy(Path.Combine(audioDb.UNC_BASE_PATH_DATA3, audioDb.FILE_NAME), Path.Combine(localUploadFolder, OriginalFileName + "_Original." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                uncSuccess = true;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat XML Original sang unc path tu unc path thanh cong");
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                uncSuccess = false;
                                                                                //Ghi log
                                                                                addLog(_logFile, "Node 74: Xuat XML Original sang unc path tu unc path ko thanh cong: " + ex.ToString());
                                                                            }
                                                                            //ftp
                                                                            if (!uncSuccess)
                                                                            {
                                                                                try
                                                                                {
                                                                                    using (WebClient ftpClient = new WebClient())
                                                                                    {
                                                                                        ftpClient.Credentials = new NetworkCredential(audioDb.USERNAME, audioDb.PASSWORD);
                                                                                        ftpClient.DownloadFile("ftp://" + audioDb.NAS_IP + ":" + audioDb.PORT + audioDb.DATA3_DIRECTORY + "/" + audioDb.FILE_NAME, Path.Combine(localUploadFolder, OriginalFileName + "_Original." + Path.GetExtension(audioDb.FILE_NAME)));
                                                                                        //Ghi log
                                                                                        addLog(_logFile, "Node 74: Xuat XML Original sang unc path tu ftp path thanh cong");
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    //Ghi log
                                                                                    addLog(_logFile, "Node 74: Xuat XML Original sang unc path tu ftp path ko thanh cong: " + ex.ToString());
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            addLog(_logFile, "Node 74: Ko tim thay XML Original trong he thong.");
                                                                            addToMail("Node 74: Ko tim thay XML Original trong he thong.");
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        addLog(_logFile, "Node 74: Xuat XML Original sang unc path ko thanh cong: " + ex.ToString());
                                                                    }
                                                                }
                                                                #endregion
                                                            }
                                                            catch (Exception ex) { addLog(_logFile, "Node 74: Loi khi export tu nas sang unc path: " + ex.ToString()); }
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        addLog(_logFile, "Node 74: Khong tim duoc file trong he thong");
                                                        addToMail("Node 74: Ko tim thay file can xuat trong he thong.");
                                                    }

                                                }
                                                catch (Exception ex)
                                                {
                                                    addLog(_logFile, "Node 74: Loi khi thuc hien task " + postId.ToString() + ": " + ex.ToString());
                                                }
                                            }
                                            db.Execute(_sqlUpdateQuery, new { nodeId = 74, id_clip = postId });
                                            if (errorStr.Length > 0)
                                            {
                                                try
                                                {
                                                    db.Execute(_sqlQueryMail, new
                                                    {
                                                        mail = txtEmail.Text,
                                                        content = errorStr,
                                                        date = DateTime.Now,
                                                        title = "Node 74: Thông tin xuất file"
                                                    });
                                                }
                                                catch (Exception ex)
                                                {
                                                    addLog(_logFile, "Node 74: Loi khi gui mail: " + ex.ToString());
                                                }
                                                errorStr = "";
                                            }

                                        }

                                    }
                                    else
                                    {
                                        addLog(_logFile, "Node 74: Khong co file can xuat");
                                        strProcessing74 = "Không có file cần xuất";
                                        strWaiting74 = "Không có file trong hàng đợi";
                                        Thread.Sleep(5000);
                                    }
                                }
                                else
                                {
                                    addLog(_logFile, "Node 74: Chua co lenh Query EXPORT_JOB");
                                }
                            });
                            _task[1].Wait();

                        }
                        catch (Exception ex)
                        {
                            addLog(_logFile, "Loi khi ket noi db: " + ex.ToString());
                        }
                    }

                }
                catch (Exception ex)
                {
                    addLog(_logFile, "Loi ko xac dinh: " + ex.ToString());
                }
            }
        }
        private string getNumber(string str)
        {
            var number = System.Text.RegularExpressions.Regex.Match(str, @"\d+").Value;

            return number;
        }
        private int getEpisodeNUmber(string programName)
        {
            return System.Text.RegularExpressions.Regex.Match(programName, @"\d+$").Value == "" ? 0 : int.Parse(System.Text.RegularExpressions.Regex.Match(programName, @"\d+$").Value);
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }
        /// <summary>
        /// Export file từ ftp server đến ftp server khác
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="targetFileName"></param>
        /// <param name="sourceURI"></param>
        /// <param name="sourceUser"></param>
        /// <param name="sourcePass"></param>
        /// <param name="targetURI"></param>
        /// <param name="targetUser"></param>
        /// <param name="targetPass"></param>
        /// <returns></returns>
        public bool copyFile(string fileName, string targetFileName, string sourceURI, string sourceUser, string sourcePass, string targetURI, string targetUser, string targetPass)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create((sourceURI + "/" + fileName).Replace("\\", "/"));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(sourceUser, sourcePass);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                bool temp = Upload(targetFileName.Replace(",", "_"), ToByteArray(responseStream), targetURI, targetUser, targetPass);
                responseStream.Close();
                if (temp)
                    return true;
                else return false;
            }
            catch (Exception ex)
            {
                addLog(_logFile, "Loi trong copyFile: " + ex.ToString() + " uri: " + sourceURI + "/" + fileName);
                return false;
            }
        }

        public Byte[] ToByteArray(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] chunk = new byte[4096 * 1024];
            int bytesRead;
            while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
            {
                ms.Write(chunk, 0, bytesRead);
            }

            return ms.ToArray();
        }

        public bool Upload(string FileName, byte[] Image, string targetURI, string targetUser, string targetPass)
        {
            FtpWebRequest clsRequest = (FtpWebRequest)WebRequest.Create(targetURI + FileName);
            try
            {
                clsRequest.Credentials = new NetworkCredential(targetUser, targetPass);
                clsRequest.Method = WebRequestMethods.Ftp.UploadFile;
                Stream clsStream = clsRequest.GetRequestStream();
                clsStream.Write(Image, 0, Image.Length);
                clsStream.Close();
                clsStream.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                addLog(_logFile, "Loi trong Upload: " + ex.ToString());
                return false;
            }
            finally
            {
                clsRequest = null;
            }
        }
        /// <summary>
        /// Upload file từ đường dẫn local  (unc) lên FTP Server
        /// </summary>
        /// <param name="inputFilePath">Đường dẫn file muốn upload</param>
        /// <param name="targetFileName">Tên file muốn đặt khi upload xong</param>
        /// <param name="nasIP"></param>
        /// <param name="nasPort"></param>
        /// <param name="nasPath"></param>
        /// <param name="nasUsername"></param>
        /// <param name="nasPassword"></param>
        public bool uploadFromUnc(string inputFilePath, string targetFileName, string ftpPath, string nasUsername, string nasPassword)
        {
            string ftpfullpath = ftpPath + targetFileName.Replace(",", "_");
            FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpfullpath);
            try
            {
                ftp.Credentials = new NetworkCredential(nasUsername, nasPassword);
                ftp.KeepAlive = true;
                ftp.UseBinary = true;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;

                int buffLength = 1024 * 1024 * 4;
                byte[] buffer = new byte[buffLength];
                int contentLen;

                FileStream fs = File.OpenRead(inputFilePath);

                contentLen = fs.Read(buffer, 0, buffLength);

                Stream ftpstream = ftp.GetRequestStream();

                while (contentLen != 0)
                {
                    ftpstream.Write(buffer, 0, contentLen);
                    contentLen = fs.Read(buffer, 0, buffLength);
                }
                ftpstream.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                addLog(_logFile, "Loi trong uploadFromUnc: " + ex.ToString() + "ftp full path: " + ftpPath + targetFileName.Replace(",", "_"));
                return false;
            }
            finally
            {
                ftp = null;
            }
        }
        private string getTimeNow()
        {
            return DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");
        }

        private void addLog(string filePath, string content)
        {
            try
            {
                using (StreamWriter file =
                new StreamWriter(filePath, true))
                {
                    file.WriteLine(" -\n - " + getTimeNow() + ":" + content + "\n");
                }
            }
            catch
            {
            }
        }
        string errorStr = "";
        private string _sqlQueryMail;

        private void addToMail(string error)
        {
            errorStr += " -\n - " + getTimeNow() + ":" + error + "\n";
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Clicks == 0)
            {
                popupMenu1.ShowPopup(MousePosition);
            }
        }

        private void barBtnShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }

        private void barBtnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// Copy file từ đường dẫn source sang đường dẫn destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
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
                                for (; ; )
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
            catch (Exception ex)
            {
                addLog(_logFile, "Loi trong FCopy: " + ex.ToString());
                return false;
            }
            return true;
        }

        private void btnAddConfig_Click(object sender, EventArgs e)
        {
            if ((txtSaveFolder.Text.Trim().Length < 3) && ((txtNasIP.Text == "") && (txtNasUsername.Text == "") && (txtNasPass.Text == "") && (txtNasPath.Text == "")))
            {
                HDMessageBox.Show("Chưa cấu hình thư mục lưu trữ!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (!ckAudioOriginal.Checked && !ckAudio.Checked && !ckMediaHighres.Checked && !ckMediaLowres.Checked && !ckXml.Checked && !ckHighresOriginal.Checked && !ckPreview.Checked && !ckImage.Checked && !ckXmlOriginal.Checked && !ckSubtitle.Checked)
            {
                HDMessageBox.Show("Chưa cấu hình file cần xuất!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                string temp = "";
                temp += (ckMediaLowres.Checked ? "Lowres doc; " : "") + (ckPreview.Checked ? "Preview; " : "") + (ckMediaHighres.Checked ? "Media Highres; " : "") + (ckHighresOriginal.Checked ? "Highres Original; " : "")
                    + (ckAudio.Checked ? "Audio Original; " : "") + (ckAudioOriginal.Checked ? "Audio Upload; " : "") + (ckXml.Checked ? "XML; " : "") + (ckXmlOriginal.Checked ? "XML Original; " : "") + (ckImage.Checked ? "Image; " : "") + (ckSubtitle.Checked ? "Subtitle; " : "");
                bsConfigList.List.Add(new View.configObj
                {
                    FileType = temp,
                    NasPath = txtNasIP.Text.Length < 7 ? "" : "ftp://" + txtNasIP.Text + ":" + nNasPort.Value.ToString() + txtNasPath.Text,
                    NasUsername = txtNasUsername.Text,
                    NasPass = txtNasPass.Text,
                    LocalPath = txtSaveFolder.Text,
                    AddSymBol = ckHeader.Checked ? "Đầu tên file: " + txtSymbol.Text : "Cuối tên file: " + txtSymbol.Text,
                    FileName = (ckMaBang.Checked ? "Mã băng; " : "") + (ckTenCT.Checked ? "Tên chương trình; " : "") + (ckDatePS.Checked ? "Ngày tháng phát sóng; " : "")
                                + (ckSeason.Checked ? "Phần; " : "") + (ckEpisode.Checked ? "Tập; " : "") + (ckDateCreate.Checked ? "Ngày tháng tạo vỏ; " : ""),
                    Emails = txtEmail.Text
                });
                (bsConfigList.List as BindingList<View.configObj>).ToList().SaveObject(_userConfigPath);
            }
            catch (Exception ex)
            {
                HDMessageBox.Show(ex.ToString());
            }
        }

        private void btnDelConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvConfig.SelectedRowsCount <= 0 && gvConfig.FocusedRowHandle < 0)
                {
                    HDMessageBox.Show("Không có tin nào để xóa!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    var lstSelected = gvConfig.GetSelectedRows();
                    if (lstSelected.Length == 0)
                        lstSelected = new int[1] { gvConfig.FocusedRowHandle };
                    if (HDMessageBox.Show("Chắc chắn xóa " + lstSelected.Length + " cấu hình?", "Chú ý", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        foreach (var handle in lstSelected.OrderByDescending(h => h).ToList())
                        {
                            bsConfigList.List.RemoveAt(handle);
                        }

                        (bsConfigList.List as BindingList<View.configObj>).ToList().SaveObject(_userConfigPath);
                    }
                }
            }
            catch (Exception ex)
            {
                HDMessageBox.Show(ex.ToString(), "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #region Check if ftp directory exists and create new one if not 
        private bool ftpDirectoryExists(string directory, string ftpHost, string ftpUsername, string ftpPassword)
        {
            try
            {
                var list = this.GetFileList(ftpHost, ftpUsername, ftpPassword);
                if (list.Contains(directory))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                addLog(_logFile, "Check ftpDirectoryExists Error: " + ex.Message);
                return false;
            }
        }
        private string[] GetFileList(string ftpHost, string ftpUsername, string ftpPassword)
        {
            var ftpPath = ftpHost;
            var ftpUser = ftpUsername;
            var ftpPass = ftpPassword;
            var result = new StringBuilder();
            var strLink = ftpPath;
            var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(strLink));
            try
            {
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(ftpUser, ftpPass);
                reqFtp.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFtp.Proxy = null;
                reqFtp.KeepAlive = false;
                reqFtp.UsePassive = true;
                using (var response = reqFtp.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var line = reader.ReadLine();
                        while (line != null)
                        {
                            result.Append(line);
                            result.Append("\n");
                            line = reader.ReadLine();
                        }
                        result.Remove(result.ToString().LastIndexOf('\n'), 1);
                    }
                }
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                addLog(_logFile, "Get FTP file list ERROR: " + ex.Message);
                return null;
            }

            finally
            {
                reqFtp = null;
            }
        }
        private bool createFTPDirectory(string ftpHost, string ftpUsername, string ftpPassword, string newDirectory)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpHost + newDirectory);

                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                request.Method = WebRequestMethods.Ftp.MakeDirectory;

                FtpWebResponse makeDirectoryResponse = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                addLog(_logFile, "Create FTP directory ERROR: " + ex.Message + ". " + ftpHost + newDirectory);
                return false;
            }
            return true;
        }
        #endregion

        private void MainForm_Shown(object sender, EventArgs e)
        {
            btnSave.PerformClick();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtProcessing73.Text = strProcessing73;
            txtWaiting73.Text = strWaiting73;
            txtWorking74.Text = strProcessing74;
            txtWaiting74.Text = strWaiting74;
        }
    }
}
