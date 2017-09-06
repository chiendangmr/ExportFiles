using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using System.Linq;

namespace HDControl
{
    public partial class OpenFileInFolderDialog : HDForm
    {
        public string RootFolder = "";
        public string FilterString = "";

        public OpenFileInFolderDialog()
        {
            InitializeComponent();
        }

        private void treeFile_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            if (e.Node.GetDisplayText("Type") == "Folder")
                e.NodeImageIndex = 0;
            else e.NodeImageIndex = 1;
        }

        private void treeFile_MouseClick(object sender, MouseEventArgs e)
        {
            Point pt = treeFile.PointToClient(MousePosition);

            TreeListHitInfo info = treeFile.CalcHitInfo(pt);
            if (info.Node != null)
            {
                treeFile.SetFocusedNode(info.Node);
            }
        }

        private void treeFile_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point pt = treeFile.PointToClient(MousePosition);

            TreeListHitInfo info = treeFile.CalcHitInfo(pt);
            if (info.Node != null)
            {
                treeFile.SetFocusedNode(info.Node);

                if (info.Node.GetDisplayText("Type") == "File")
                    btnOK.PerformClick();
            }
        }

        private void OpenFileInFolderDialog_Load(object sender, EventArgs e)
        {
            if (RootFolder != "" && Directory.Exists(RootFolder))
            {
                treeFile.BeginUnboundLoad();
                string[] lstDir = Directory.GetDirectories(RootFolder);
                foreach (var dir in lstDir)
                {
                    string dirName = Path.GetFileName(dir);
                    var node = treeFile.AppendNode(new object[] { "Folder", dirName }, null);
                    node.HasChildren = true;
                }

                List<string> files = new List<string>();
                if (FilterString == "")
                    files = Directory.GetFiles(RootFolder).Select(f => Path.GetFileName(f)).ToList();
                else
                {
                    var lstExt = FilterString.Split(';');
                    foreach (var ex in lstExt)
                    {
                        var ff = Directory.GetFiles(RootFolder, ex).Select(f => Path.GetFileName(f)).ToList();
                        foreach (var f in ff)
                            files.Add(f);
                    }
                }
                foreach (var file in files.OrderBy(f => f).ToList())
                {
                    var node = treeFile.AppendNode(new object[] { "File", file }, null);
                    node.HasChildren = false;
                }

                treeFile.EndUnboundLoad();
            }
        }

        private void treeFile_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (e.Node.GetDisplayText("Type") == "File")
                btnOK.Enabled = true;
            else
                btnOK.Enabled = false;
        }

        private void treeFile_BeforeExpand(object sender, BeforeExpandEventArgs e)
        {
            if (e.Node.GetDisplayText("Type") == "Folder" && e.Node.HasChildren && e.Node.Nodes.Count == 0)
            {
                string dirPath = e.Node.GetDisplayText("Dir");
                var curNode = e.Node.ParentNode;
                while (curNode != null)
                {
                    dirPath = curNode.GetDisplayText("Dir") + "\\" + dirPath;
                    curNode = curNode.ParentNode;
                }
                dirPath = Path.Combine(RootFolder, dirPath);

                treeFile.BeginUnboundLoad();
                string[] lstDir = Directory.GetDirectories(dirPath);
                foreach (var dir in lstDir)
                {
                    string dirName = Path.GetFileName(dir);
                    var node = treeFile.AppendNode(new object[] { "Folder", dirName }, e.Node);
                    node.HasChildren = true;
                }

                List<string> files = new List<string>();
                if (FilterString == "")
                    files = Directory.GetFiles(dirPath).Select(f => Path.GetFileName(f)).ToList();
                else
                {
                    var lstExt = FilterString.Split(';');
                    foreach (var ex in lstExt)
                    {
                        var ff = Directory.GetFiles(dirPath, ex).Select(f => Path.GetFileName(f)).ToList();
                        foreach (var f in ff)
                            files.Add(f);
                    }
                }
                foreach (var file in files.OrderBy(f => f).ToList())
                {
                    var node = treeFile.AppendNode(new object[] { "File", file }, e.Node);
                    node.HasChildren = false;
                }

                treeFile.EndUnboundLoad();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (treeFile.FocusedNode == null || treeFile.FocusedNode.GetDisplayText("Type") != "File")
            {
                HDMessageBox.Show("Please choose a file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        public string FileName
        {
            get
            {
                if (treeFile.FocusedNode == null || treeFile.FocusedNode.GetDisplayText("Type") != "File")
                {
                    return "";
                }

                string dirPath = treeFile.FocusedNode.GetDisplayText("Dir");
                var curNode = treeFile.FocusedNode.ParentNode;
                while (curNode != null)
                {
                    dirPath = curNode.GetDisplayText("Dir") + "/" + dirPath;
                    curNode = curNode.ParentNode;
                }

                return dirPath;
            }
        }
    }
}