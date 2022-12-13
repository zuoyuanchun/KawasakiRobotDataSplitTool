using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Management;
using System.Windows.Forms;

namespace KhiRobotPgSplit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string FileLoadPath = string.Empty;
            OpenFileDialog LoadFileDialog = new OpenFileDialog();
            LoadFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            LoadFileDialog.Filter = "Robot File|*.au;*.as;*.bi;*.edl;*.el;*.fl;*.id;*.in;*.if;*.lc;*.mt;*.oi;*.ol;*.pg;*.rv;*.rb;*.sc;*.st;*.sy;*.txt|All File|*.*";
            LoadFileDialog.Title = "Please select the full data file exported by the robot";
            LoadFileDialog.ShowHelp = true;
            if (LoadFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileLoadPath = LoadFileDialog.FileName;
            }
            else
            {
                return;
            }
            /*
            string OSLangu = string.Empty;
            string encodingStr = "gb18030";
            //获取当前系统LCID编码 Get LCID
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * From Win32_OperatingSystem");
            ManagementObjectCollection moc = mos.Get();
            foreach (ManagementBaseObject mbo in moc)
            {
                ManagementObject mobj = (ManagementObject)mbo;
                OSLangu = mobj["OSLanguage"].ToString();
            }
            switch (OSLangu)
            {
                case "2052":
                    encodingStr = "gb18030";
                    break;//简体中文 Chinese
                case "1033":
                    encodingStr = "euc-jp";
                    break;//英文 English
                case "1041":
                    encodingStr = "shift-jis";
                    break;//日文 Japanese
                case "1031":
                    encodingStr = "iso-8859-1";
                    break;//德语 German
                case "1040":
                    encodingStr = "iso-8859-1";
                    break;//意大利语 Italian
                case "1049":
                    encodingStr = "iso-8859-5";
                    break;//俄文 Russian
                case "1042":
                    encodingStr = "euc-kr";
                    break;//韩语/朝鲜语 Korean
                case "1043":
                    encodingStr = "iso-8859-1";
                    break;//Dutch-荷兰语 Dutch;
                case "1029":
                    encodingStr = "iso-8859-1";
                    break;//Czech-捷克语 Czech
                default:
                    encodingStr = "iso-8859-5";
                    break;
            }
            StreamReader rs = new StreamReader(FileLoadPath, Encoding.GetEncoding(encodingStr), true);
             * */
            StreamReader streamReader = new StreamReader(FileLoadPath, Encoding.Default, true);
            List<string> TextData = new List<string>();
            while (!streamReader.EndOfStream)
            {
                TextData.Add(streamReader.ReadLine());
            }
            streamReader.Close();
            //Create Splited File Directory
            string BaseDir = string.Empty;
            string SaveDir = string.Empty;//Set the Save Directory
            if (SaveDir == string.Empty)
            {
                BaseDir = Application.StartupPath;
                SaveDir = Path.Combine(BaseDir, "SplitDir");
                if (!Directory.Exists(SaveDir)) //判断文件夹是否存在
                {
                    Directory.CreateDirectory(SaveDir);//创建文件夹
                }
            }
            string ts = string.Empty;
            string fullpath = string.Empty;
            bool savemode = false;
            int DataNum = 0;
            int ifpcount = 0;
            bool working = true;
            while (working) 
            {
                //判定是否为段落注释  Check Tips Name
                while (working) 
                {
                    if (DataNum >= TextData.Count)
                    {
                        working = false;
                        break;
                    }
                    if (TextData[DataNum].Length < 10)
                    {
                        DataNum++;
                        continue;
                    }
                    string tmpstr = TextData[DataNum].Substring(0, 8);
                    if (tmpstr == ".INTER_P")
                    {
                        fullpath = Path.Combine(SaveDir, "ajkmb.if");
                        if (ifpcount == 0)
                        {
                            savemode = false;
                        }
                        else
                        {
                            savemode = true;
                        }
                        ifpcount++;
                        break;
                    }
                    if (tmpstr == ".PROGRAM")
                    {
                        ts = TextData[DataNum];
                        ts = ts.Substring(ts.IndexOf(" "));
                        ts = ts.Substring(0, ts.IndexOf("("));
                        ts = ts.Trim();
                        fullpath = Path.Combine(SaveDir, ts + ".pg");
                        savemode = false;
                        break;
                    }
                    DataNum++;
                }
                //StreamWriter streamWrite = new StreamWriter(fullpath, savemode, Encoding.GetEncoding(encodingStr));
                StreamWriter streamWrite = new StreamWriter(fullpath, savemode, Encoding.Default);
                while (working) 
                {
                    streamWrite.WriteLine(TextData[DataNum]);
                    DataNum++;
                    if (DataNum > TextData.Count)
                    {
                        streamWrite.Close();
                        working = false;
                        break;
                    }
                    if (TextData[DataNum] == ".END")
                    {
                        streamWrite.WriteLine(TextData[DataNum]);
                        streamWrite.Flush();
                        streamWrite.Close();
                        DataNum++;
                        break;
                    }
                }
            }
            //string msgInfo = "数据已拆分并保存!\r\n是否打开文件夹?";
            //string msgTitle = "拆分完成";
            string msgInfo = "Split Complete!\r\nDo you want to open directory?";
            string msgTitle = "Done";
            DialogResult msgResult = MessageBox.Show(msgInfo, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (msgResult == DialogResult.Yes) 
            {
                System.Diagnostics.Process.Start(SaveDir);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string githubweb = "https://github.com/zuoyuanchun";
            System.Diagnostics.Process.Start(githubweb);
        }

    }
}
