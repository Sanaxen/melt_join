using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.Core;

namespace tft
{

    public partial class Form1 : Form
    {
        [DllImport("shlwapi.dll",
            CharSet = CharSet.Auto)]
        private static extern IntPtr PathCombine(
            [Out] StringBuilder lpszDest,
            string lpszDir,
            string lpszFile);

        public string exePath = "";
        public string RlibPath = "";

        public string plot_time_unit = "";
        public string encoding = "sjis";

        public string cmd_all = "";
        public int output_idx = 0;
        public string with_current_df_cmd = "";
        public ListBox feature_cmd = new ListBox();

        public Form1()
        {
            InitializeComponent();
            exePath = AppDomain.CurrentDomain.BaseDirectory;

            if (File.Exists(exePath + "R_install_path.txt"))
            {
                using (StreamReader sr = new StreamReader(exePath + "R_install_path.txt"))
                {
                    textBox1.Text = sr.ReadToEnd().Replace("\n", "");
                }
            }

            StringBuilder sb = new StringBuilder(2048);
            IntPtr res = PathCombine(sb, exePath, "..\\..\\..\\lib");
            if (res == IntPtr.Zero)
            {
                MessageBox.Show("Failed to obtain absolute path of R library.");
            }
            else
            {
                RlibPath = sb.ToString().Replace("\\", "/");
            }
        }

        public ListBox removeDup(ListBox l1)
        {
            ListBox tmp = new ListBox();

            for (int i = 0; i < l1.Items.Count - 1; i++)
            {
                bool dup = false;
                for (int j = i + 1; j < l1.Items.Count; j++)
                {
                    if (l1.Items[i].Equals(l1.Items[j]) == true)
                    {
                        dup = true;
                    }
                }
                tmp.Items.Add(l1.Items[i]);
            }

            l1.Items.Clear();
            for (int i = 0; i < tmp.Items.Count; i++)
            {
                l1.Items.Add(tmp.Items[i]);
            }

            return l1;
        }

        public void cmd_save()
        {
            string file = "melt_join.r";
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(cmd_all);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
        }
        public static System.Drawing.Image CreateImage(string filename)
        {
            System.IO.FileStream fs = new System.IO.FileStream(
                filename,
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read);
            System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
            fs.Close();
            return img;
        }
        public string base_dir;
        public string work_dir;
        public string csv_file;
        public string csv_dir;
        public string base_name = "";
        public string base_name0 = "";

        public string base_dir2;
        public string work_dir2;
        public string csv_file2;
        public string csv_dir2;
        public string base_name2 = "";

        public string left_join_by = "";

        ListBox colname_list = new ListBox();
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();


        private void UpdateInvokeRequire()
        {
            TimeSpan ts = stopwatch.Elapsed;
        }

        void Proc_Exited(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(50);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            if (InvokeRequired)
            {
                Invoke(new Action(this.UpdateInvokeRequire));
                //Invoke(new Action(() => { label1.Text = "Stop!"; }));
            }
        }

        public void execute(string script_file, bool wait = true)
        {
            ProcessStartInfo pInfo = new ProcessStartInfo();
            //pInfo.FileName = textBox1.Text + "\\R.exe";
            //pInfo.Arguments = "CMD BATCH  --vanilla " + script_file;

            //pInfo.FileName = textBox1.Text + "\\Rscript.exe";
            //pInfo.Arguments = "" + script_file;

            pInfo.FileName = textBox1.Text + "\\x64\\Rscript.exe";
            pInfo.Arguments = "" + script_file;

            if (!File.Exists(pInfo.FileName))
            {
                MessageBox.Show(pInfo.FileName + " is not found.\nPlease confirm that " + textBox1.Text + " is specified as the file path, which is correct.");
                return;
            }
            //Process p = Process.Start(pInfo);
            Process p = new Process();
            p.StartInfo = pInfo;

            if (wait)
            {
                p.Start();
                p.WaitForExit();
            } else
            {
                stopwatch.Start();
                p.Exited += new EventHandler(Proc_Exited);
                p.EnableRaisingEvents = true;
                p.Start();
            }
        }

        public ListBox GetNames(bool gettypes = false)
        {
            if (File.Exists("names.txt"))
            {
                File.Delete("names.txt");
            }

            string cmd = "";
            cmd += "options(encoding=\"" + encoding + "\")\r\n";
            cmd += ".libPaths(c('" + RlibPath + "',.libPaths()))\r\n";
            cmd += "dir='" + work_dir.Replace("\\", "\\\\") + "'\r\n";
            cmd += "library(data.table)\r\n";
            cmd += "setwd(dir)\r\n";
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "#df <- read.csv(\"" + base_name + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "x_<-ncol(df)\r\n";
            cmd += "print(x_)\r\n";
            cmd += "for ( i in 1:x_) print(names(df)[i])\r\n";
            string file = "tmp_get_namse.R";

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write("sink(file = \"names.txt\")\r\n");
                    sw.Write(cmd);
                    sw.Write("sink()\r\n");
                    if ( gettypes)
                    {
                        sw.Write(GetTypes_cmd());
                    }
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return null;
            }

            execute(file);
            ListBox list = new ListBox();

            if (File.Exists("names.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("names.txt", Encoding.GetEncoding("SHIFT_JIS"));
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        var nums = line.Split(' ');
                        int num = int.Parse(nums[1]);

                        for (int i = 0; i < num; i++)
                        {
                            line = sr.ReadLine();
                            var names = line.Substring(4);

                            names = names.Replace("\n", "");
                            names = names.Replace("\r", "");
                            names = names.Replace("\"", "");
                            if (names.IndexOf(" ") >= 0)
                            {
                                names = "'" + names + "'";
                            }
                            list.Items.Add(names);
                        }
                        if (list.Items.Count != num)
                        {
                            MessageBox.Show("Does the column name contain \", \" or \"spaces\"?\n" +
                                "ou may not be getting the column names correctly.", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    }
                    sr.Close();
                }
                catch { }
            }
            return list;
        }

        public void SelectTypes()
        {

            if (File.Exists("types.txt"))
            {
                int id = -1;
                if (comboBox2.Text == "integer") id = 1;
                if (comboBox2.Text == "factor") id = 2;
                if (comboBox2.Text == "numeric") id = 3;
                if (comboBox2.Text == "character") id = 4;

                if (id == -1)
                {
                    return;
                }

                for (int i = 0; i < listBox4.Items.Count; i++)
                {
                   listBox4.SetSelected(i, false);
                }

                try
                {
                    StreamReader sr = new StreamReader("types.txt", Encoding.GetEncoding("SHIFT_JIS"));
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        var nums = line.Split(' ');

                        if (nums[id] == "1")
                        {
                            for (int i = 0; i < listBox4.Items.Count; i++)
                            {
                                if (nums[0] == listBox4.Items[i].ToString())
                                {
                                    listBox4.SetSelected(i, true);
                                }
                            }
                        }
                    }
                    sr.Close();
                }
                catch { }
            }
        }

        public string GetTypes_cmd()
        {
            string cmd = "";
            cmd += "sink(file = \"types.txt\")\r\n";
            cmd += "tmp <- data.frame(df)\r\n";
            cmd += "cols <- colnames(tmp)\r\n";
            cmd += "for ( i in 1:length(cols) )\r\n";
            cmd += "{\r\n";
            cmd += "	s = sprintf(\"%s %s %s %s %s\n\", cols[i],\r\n";
            cmd += "	ifelse(is.integer(tmp[i,i]),1,0),\r\n";
            cmd += "	ifelse(is.factor(tmp[i,i]),1,0),\r\n";
            cmd += "	ifelse(is.numeric(tmp[i,i]),1,0),\r\n";
            cmd += "	ifelse(is.character(tmp[i,i]),1,0))\r\n";
            cmd += "	cat(s)\r\n";
            cmd += "}\r\n";
            cmd += "sink()\r\n";
            cmd += "rm(tmp)\r\n";

            return (cmd);
        }
        public string tft_header_ru()
        {
            string cmd = "";

            cmd += "options(encoding=\"" + encoding + "\")\r\n";
            cmd += ".libPaths(c('" + RlibPath + "',.libPaths()))\r\n";
            cmd += "dir='" + work_dir.Replace("\\", "\\\\") + "'\r\n";
            cmd += "setwd(dir)\r\n";
            cmd += "suppressMessages({\r\n";
            cmd += "    library(data.table)\r\n";
            cmd += "    library(RcppRoll)\r\n";
            cmd += "#    library(lightgbm)\r\n";
            cmd += "    library(dplyr)\r\n";
            cmd += "    library(lubridate)\r\n";
            cmd += "    library(reshape2)\r\n";
            cmd += "    library(Matrix)\r\n";
            cmd += "    library(xgboost)\r\n";
            cmd += "    library(tidyverse)\r\n";
            cmd += "    library(scales)\r\n";
            cmd += "    library(skimr)\r\n";
            cmd += "    library(tibble)\r\n";
            cmd += "    library(tidyr) \r\n";
            cmd += "    library(stringr)  \r\n";
            cmd += "    library(ggthemes)\r\n";
            cmd += "    library(ggpubr)\r\n";
            cmd += "    library(timetk)\r\n";
            cmd += "})\r\n";
            cmd += "set.seed(1)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            if (cmd_all == "") cmd_all = cmd;
            return cmd;
        }
        string melt(string base_name)
        {
            string cmd = "";

            //cmd = tft_header_ru();

            cmd += "#df <- read.csv(\"" + base_name + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";

            if (listBox4.SelectedItems.Count >= 1)
            {
                cmd += "id.vars=c(\r\n";
                cmd += "              '" + listBox4.SelectedItems[0].ToString() + "'";
                for (int i = 1; i < listBox4.SelectedItems.Count; i++)
                {
                    cmd += ", ";
                    cmd += "'" + listBox4.SelectedItems[i].ToString() + "'";
                    if (i % 10 == 0)
                    {
                        cmd += "\r\n              ";
                    }
                }
                cmd += ")\r\n";
            } else
            {
                cmd += "id.vars=NULL\r\n";
            }

            if (listBox1.SelectedItems.Count >= 1)
            {
                cmd += "\r\n";
                cmd += "measure.vars=c(\r\n";
                cmd += "              '" + listBox1.SelectedItems[0].ToString() + "'";
                for (int i = 1; i < listBox1.SelectedItems.Count; i++)
                {
                    cmd += ", ";
                    cmd += "'" + listBox1.SelectedItems[i].ToString() + "'";
                    if (i % 10 == 0)
                    {
                        cmd += "\r\n              ";
                    }
                }
                cmd += ")\r\n";
            }
            else
            {
                //cmd += "measure.vars = NULL\r\n";
            }

            return cmd;
        }

        void save()
        {
            string file = "melt_join_setting_" + base_name0 + ".txt";
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(listBox1.Items.Count.ToString() + "\n");
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        sw.Write(listBox1.Items[i].ToString() + "\n");
                    }

                    sw.Write(listBox1.SelectedItems.Count.ToString() + "\n");
                    if (listBox1.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox1.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox1.SelectedIndices[i].ToString() + "\n");
                        }
                    }
                    sw.Write(listBox2.SelectedItems.Count.ToString() + "\n");
                    if (listBox2.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox2.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox2.SelectedIndices[i].ToString() + "\n");
                        }
                    }
                    sw.Write(listBox3.SelectedItems.Count.ToString() + "\n");
                    if (listBox3.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox3.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox3.SelectedIndices[i].ToString() + "\n");
                        }
                    }
                    sw.Write(listBox4.SelectedItems.Count.ToString() + "\n");
                    if (listBox4.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox4.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox4.SelectedIndices[i].ToString() + "\n");
                        }
                    }

                    sw.Write("r_path," + textBox1.Text + "\n");
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in melt_join_setting_.txt", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            try
            {
                File.Copy("melt_join_setting_" + base_name0 + ".txt", "pre_setting.txt", true);
            }
            catch { }
        }
        void load(string setting_file)
        {
            string file = "melt_join_setting_" + base_name0 + ".txt";
            if (setting_file == "")
            {
                if (base_name0 == "")
                {
                    MessageBox.Show("input csv file !");
                    return;
                }
                if (!File.Exists(file)) save();

                if (!File.Exists(file))
                {
                    MessageBox.Show("file not found[" + "melt_join_setting_" + base_name0 + ".txt]");
                }
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
            }
            else
            {
                file = setting_file;
            }

            string rexe1 = textBox1.Text + "\\x64\\Rscript.exe";
            string rexe = rexe1;

            bool rpath_chg = false;
            System.IO.StreamReader sr = new System.IO.StreamReader(file, Encoding.GetEncoding("SHIFT_JIS"));
            if (sr != null)
            {
                while (sr.EndOfStream == false)
                {
                    string s = sr.ReadLine();
                    int n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        if (setting_file == "")
                        {
                            listBox1.Items.Add(s.Replace("\n", ""));
                            listBox2.Items.Add(s.Replace("\n", ""));
                            listBox3.Items.Add(s.Replace("\n", ""));
                            listBox4.Items.Add(s.Replace("\n", ""));
                        }
                    }
                    if (setting_file == "")
                    {
                        for (int i = 0; i < n; i++)
                        {
                            listBox1.SetSelected(i, false);
                            listBox2.SetSelected(i, false);
                            listBox3.SetSelected(i, false);
                            listBox4.SetSelected(i, false);
                        }
                    }
                    s = sr.ReadLine();
                    n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        int k = int.Parse(s.Replace("\n", ""));
                        listBox1.SetSelected(k, true);
                    }
                    s = sr.ReadLine();
                    n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        int k = int.Parse(s.Replace("\n", ""));
                        listBox2.SetSelected(k, true);
                    }
                    s = sr.ReadLine();
                    n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        int k = int.Parse(s.Replace("\n", ""));
                        listBox3.SetSelected(k, true);
                    }
                    s = sr.ReadLine();
                    n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        int k = int.Parse(s.Replace("\n", ""));
                        listBox4.SetSelected(k, true);
                    }
                    while (sr.EndOfStream == false)
                    {
                        s = sr.ReadLine();
                        var ss = s.Split(',');

                        if (ss[0].IndexOf("r_path") >= 0)
                        {
                            string path = ss[1].Replace("\r\n", "");
                            rexe = path + "\\x64\\Rscript.exe";

                            if (!File.Exists(rexe))
                            {
                                continue;
                            } else
                            {
                                rpath_chg = true;
                            }
                            textBox1.Text = path;
                            continue;
                        }
                    }
                }
            }
            if (sr != null) sr.Close();
            if (!rpath_chg)
            {
                MessageBox.Show(rexe + " is not found.\nThe path of the loaded setting was ignored.");
                if (!File.Exists(rexe))
                {
                    MessageBox.Show("Please reconfigure the path where 'Rscript.exe' exists.");
                    return;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            System.IO.Directory.SetCurrentDirectory(exePath + "\\..\\..\\..\\");
            Directory.CreateDirectory("work");
            System.IO.Directory.SetCurrentDirectory(exePath + "\\..\\..\\..\\");

            base_dir2 = System.IO.Directory.GetCurrentDirectory();
            work_dir2 = base_dir + "\\work";
            System.IO.Directory.SetCurrentDirectory(work_dir);


            csv_file2 = openFileDialog1.FileName;
            csv_dir2 = Path.GetDirectoryName(csv_file2);
            base_name2 = Path.GetFileNameWithoutExtension(csv_file2);

            work_dir2 = base_dir2 + "\\work\\" + base_name2;
            System.IO.Directory.SetCurrentDirectory(work_dir);

            if (csv_dir != Path.GetDirectoryName(work_dir + base_name2 + ".csv"))
            {
                File.Copy(csv_file2, base_name2 + ".csv", true);
            }

            this.Text = "[" + base_name + "]";

            var tmp = base_name;
            base_name = base_name2;
            listBox_remake(true, false);
            base_name = tmp;

            try
            {
                //load("");
            }
            catch { }

            string file = exePath + "R_install_path.txt";

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(textBox1.Text + "\n");
                }
            }
            catch
            {
                if (MessageBox.Show("R_install_path", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
        }

        public void listBox_remake(bool join = false, bool gettypes=false)
        {
            colname_list = GetNames(gettypes);

            if (!join)
            {
                listBox4.Items.Clear();
                listBox1.Items.Clear();
            }
            else
            {
                listBox2.Items.Clear();
                listBox3.Items.Clear();
            }

            for (int i = 0; i < colname_list.Items.Count; i++)
            {
                if (!join)
                {
                    listBox4.Items.Add(colname_list.Items[i]);
                    listBox1.Items.Add(colname_list.Items[i]);
                }
                else
                {
                    listBox2.Items.Add(colname_list.Items[i]);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            System.IO.Directory.SetCurrentDirectory(exePath + "\\..\\..\\..\\");
            Directory.CreateDirectory("work");
            System.IO.Directory.SetCurrentDirectory(exePath + "\\..\\..\\..\\");

            base_dir = System.IO.Directory.GetCurrentDirectory();
            work_dir = base_dir + "\\work";
            System.IO.Directory.SetCurrentDirectory(work_dir);


            csv_file = openFileDialog1.FileName;
            csv_dir = Path.GetDirectoryName(csv_file);
            base_name = Path.GetFileNameWithoutExtension(csv_file);

            Directory.CreateDirectory(base_name);
            work_dir = base_dir + "\\work\\" + base_name;
            System.IO.Directory.SetCurrentDirectory(work_dir);

            int id = (int)numericUpDown6.Value;
            id = id - 1;

            string tmp = Path.GetDirectoryName(work_dir + "\\" + base_name + ".csv");
            if (csv_dir != Path.GetDirectoryName(work_dir + "\\"+base_name + ".csv"))
            {
                File.Copy(csv_file, base_name + ".csv", true);
            }
            base_name0 = base_name;

            if (id >= 0)
            {
                if (!File.Exists(work_dir + string.Format("\\{0}{1}", base_name0, id) + ".csv"))
                {
                    return;
                }
                base_name = string.Format("{0}{1}", base_name0, id);
                output_idx += id + 1;
            }

            this.Text = "[" + base_name + "]";
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;

            listBox_remake(false, true);

            comboBox4.Items.Clear();
            comboBox5.Items.Clear();

            comboBox4.Items.Add("");
            comboBox4.Text = "";
            comboBox5.Items.Add("");
            comboBox5.Text = "";
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                comboBox4.Items.Add(listBox4.Items[i].ToString());
                comboBox5.Items.Add(listBox4.Items[i].ToString());
            }
            try
            {
                //load("");
            }
            catch { }

            string file = exePath + "R_install_path.txt";

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(textBox1.Text + "\n");
                }
            }
            catch
            {
                if (MessageBox.Show("R_install_path", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
        }


        void clear()
        {
            if (File.Exists("tft_train_errorLog_" + base_name + ".txt"))
            {
                File.Delete("tft_train_errorLog_" + base_name + ".txt");
            }
            if (File.Exists("tft_predict_errorLog_" + base_name + ".txt"))
            {
                File.Delete("tft_predict_errorLog_" + base_name + ".txt");
            }
            if (File.Exists("tft_predict_measure_" + base_name + ".png"))
            {
                File.Delete("tft_predict_measure_" + base_name + ".png");
            }
            if (File.Exists(base_name + "_predict_measure.txt"))
            {
                File.Delete(base_name + "_predict_measure.txt");
            }
            if (File.Exists("tft_" + base_name + ".RData"))
            {
                File.Delete("tft_" + base_name + ".RData");
            }
            if (File.Exists(base_name + "_fitted.rd"))
            {
                File.Delete(base_name + "_fitted.rd");
            }
            if (File.Exists(base_name + "_predict.png"))
            {
                File.Delete(base_name + "_predict.png");
            }
            if (File.Exists(base_name + "_predict_real.png"))
            {
                File.Delete(base_name + "_predict_real.png");
            }
            if (File.Exists(base_name + "_p_learn_rate_plot.png"))
            {
                File.Delete(base_name + "_p_learn_rate_plot.png");
            }
            if (File.Exists(base_name + "_p_input_plot.png"))
            {
                File.Delete(base_name + "_p_input_plot.png");
            }
            if (File.Exists(base_name + "_fitted_plot.png"))
            {
                File.Delete(base_name + "_fitted_plot.png");
            }
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p.html"))
            {
                File.Delete(work_dir + "\\tft_" + base_name + "_p.html");
            }
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_learn_rate_plot.html"))
            {
                File.Delete(work_dir + "\\tft_" + base_name + "_p_learn_rate_plot.html");
            }
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_plt0.html"))
            {
                File.Delete(work_dir + "\\tft_" + base_name + "_plt0.html");
            }
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p.html"))
            {
                File.Delete(work_dir + "\\tft_" + base_name + "_p.html");
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            load("");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string cmd = "";

            string cmd1 = tft_header_ru();


            cmd = melt(base_name);

            cmd += with_current_df_cmd;

            if (listBox1.SelectedItems.Count == 0)
            {
                cmd += "df <- reshape2::melt(df, id.vars = id.vars, variable.name = \"" + textBox2.Text + "\", value.name = \"" + textBox3.Text + "\")\r\n";
            }
            else
            {
                cmd += "df <- reshape2::melt(df, id.vars = id.vars, measure.vars = measure.vars,  variable.name = \"" + textBox2.Text + "\", value.name = \"" + textBox3.Text + "\")\r\n";
            }
            cmd += "\r\n";
            cmd += "#write.csv(df,'" + base_name0 + string.Format( "{0}.csv", output_idx) +"', row.names = FALSE)\r\n";
            cmd += "fwrite(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            string file = string.Format("reshape2_melt{0}.r", output_idx);
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write(cmd1);
                    sw.Write(cmd);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + string.Format("{0}", output_idx);
            output_idx += 1;
            listBox_remake(false, true);
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                if (listBox4.GetSelected(i)) listBox1.SetSelected(i, false);
                else listBox1.SetSelected(i, true);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string cmd = "";

            string cmd1 = tft_header_ru();

            cmd += "#df <- read.csv(\"" + base_name + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "#df2 <- read.csv(\"" + base_name2 + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "df2 <- fread(\"" + base_name2 + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";

            cmd += with_current_df_cmd;

            string args = "";
            if (listBox3.SelectedItems.Count >= 1)
            {
                args += "by=c(\r\n";
                args += "                 "+listBox3.SelectedItems[0].ToString()+"\r\n";
                for (int i = 1; i < listBox3.SelectedItems.Count; i++)
                {
                    args += "                ," + listBox3.SelectedItems[i].ToString() + "\r\n";
                }
                args += "                )";
            }

            if (listBox3.SelectedItems.Count == 0)
            {
                string tmp = "";

                for (int i = 0; i < listBox4.Items.Count; i++)
                {
                    for (int j = 0; j < listBox2.Items.Count; j++)
                    {
                        if (listBox2.Items[j].ToString() == listBox4.Items[i].ToString())
                        {
                            tmp += "                 '" + listBox4.Items[i].ToString() + "' = '"+ listBox4.Items[i].ToString()+"',\r\n";
                        }
                    }
                }
                if (tmp != "")
                {
                    if (tmp[tmp.Length - 3] == ',' && tmp[tmp.Length-2] == '\r' && tmp[tmp.Length - 1] == '\n')
                    {
                        tmp = tmp.TrimEnd('\r', '\n', ',');
                    }
                    args += "by=c(\r\n";
                    args += tmp;
                    args += "                )";
                }
            }
            if ( args == "")
            {
                if (MessageBox.Show("The keysets to merge are not letting me choose.", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd += "df <- df %>% left_join(df2,\r\n";
            cmd += "                " + args;
            cmd += ")\r\n";
            cmd += "\r\n";
            cmd += "#write.csv(df,'" + base_name0 + string.Format("{0}.csv", output_idx) +"', row.names = FALSE)\r\n";
            cmd += "fwrite(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            string file = string.Format("left_join{0}.r", output_idx);
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write(cmd1);
                    sw.Write(cmd);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + string.Format("{0}", output_idx);
            output_idx += 1;
            listBox_remake(false, true);
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string cmd = "";
            if (listBox4.SelectedItems.Count == 1 && listBox2.SelectedItems.Count == 1)
            {
                cmd += " '" + listBox4.SelectedItems[0].ToString() + "'";
                cmd += "= '" + listBox2.SelectedItems[0].ToString() + "'";
                listBox3.Items.Add(cmd);
                listBox3.SetSelected(listBox3.Items.Count-1, true);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                listBox4.Enabled = true;
                listBox1.Enabled = true;
                listBox2.Enabled = false;
                listBox3.Enabled = false;

                listBox4.SelectionMode = SelectionMode.MultiSimple;
                listBox2.SelectionMode = SelectionMode.MultiSimple;
                label16.Text = "id.vars";
                label11.Text = "measure.vars";
                label14.Text = "key";
                label3.Text = "";
            }
            if (tabControl1.SelectedIndex == 1)
            {
                listBox4.Enabled = true;
                listBox1.Enabled = false;
                listBox2.Enabled = true;
                listBox3.Enabled = true;

                listBox4.SelectionMode = SelectionMode.One;
                listBox2.SelectionMode = SelectionMode.One;
                label16.Text = "left column";
                label11.Text = "";
                label14.Text = "right column";
                label3.Text = "left column = right  column";
            }
            if (tabControl1.SelectedIndex == 2)
            {
                listBox4.Enabled = true;
                listBox1.Enabled = false;
                listBox2.Enabled = false;
                listBox3.Enabled = false;
                listBox4.SelectionMode = SelectionMode.MultiSimple;
                listBox2.SelectionMode = SelectionMode.One;
                label16.Text = "select vars";
                label11.Text = "";
                label14.Text = "";
                label3.Text = "";
            }
            if (tabControl1.SelectedIndex == 3)
            {
                listBox4.Enabled = true;
                listBox1.Enabled = false;
                listBox2.Enabled = false;
                listBox3.Enabled = true;
                listBox4.SelectionMode = SelectionMode.One;
                listBox2.SelectionMode = SelectionMode.One;
                listBox3.SelectionMode = SelectionMode.MultiSimple;
                label16.Text = "select vars";
                label11.Text = "";
                label14.Text = "Feature value";
                label3.Text = "";

                listBox3.Items.Clear();
                if (feature_cmd.Items.Count > 0)
                {
                    for (int i = 0; i < feature_cmd.Items.Count; i++)
                    {
                        listBox3.Items.Add(feature_cmd.Items[i]);
                    }
                }
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            string cmd = "";

            string cmd1 = tft_header_ru();

            cmd += "#df <- read.csv(\"" + base_name + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";


            cmd += "\r\n";
            cmd += "#write.csv(df,'" + base_name0 + "_finalOutput.csv" + "', row.names = FALSE)\r\n";
            cmd += "fwrite(df,'" + base_name0 + "_finalOutput.csv" + "', row.names = FALSE)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            string file = string.Format("finalOutput{0}.r", output_idx);
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write(cmd1);
                    sw.Write(cmd);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + "_finalOutput.csv";
            output_idx += 1;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SelectTypes();
        }

        public void TypeChange_cmd()
        {
            string cmd = "";

            if (listBox4.SelectedItems.Count >= 1)
            {
                for (int i = 0; i < listBox4.SelectedItems.Count; i++)
                {
                    cmd += "df$" + listBox4.SelectedItems[i].ToString() + "<- as." + comboBox3.Text + "(" +
                        "df$" + listBox4.SelectedItems[i].ToString() + ")\r\n";
                }
                cmd += "\r\n";
            }
            with_current_df_cmd += cmd;
            textBox6.Text = with_current_df_cmd;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            TypeChange_cmd();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            comboBox2.Text = "factor";
            SelectTypes();

            string cmd = "";

            if (listBox4.SelectedItems.Count >= 1)
            {
                for (int i = 0; i < listBox4.SelectedItems.Count; i++)
                {
                    cmd += "df$" + listBox4.SelectedItems[i].ToString() + "<- as.character(df$" + listBox4.SelectedItems[i].ToString() + ")\r\n";
                    cmd += "df$" + listBox4.SelectedItems[i].ToString() + "<- replace(df$" + listBox4.SelectedItems[i].ToString() +
                        ", is.na(df$" + listBox4.SelectedItems[i].ToString() + "), \"" + textBox4.Text + "\")\r\n";
                    cmd += "df$" + listBox4.SelectedItems[i].ToString() + "<- as.factor(df$" + listBox4.SelectedItems[i].ToString() + ")\r\n\r\n";
                }
                cmd += "\r\n";
            }
            with_current_df_cmd += cmd;
            textBox6.Text = with_current_df_cmd;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            comboBox2.Text = "numeric";
            SelectTypes();

            string cmd = "";

            if (listBox4.SelectedItems.Count >= 1)
            {
                for (int i = 0; i < listBox4.SelectedItems.Count; i++)
                {
                    cmd += "df$" + listBox4.SelectedItems[i].ToString() + "<- replace(df$" + listBox4.SelectedItems[i].ToString() +
                        ", is.na(df$" + listBox4.SelectedItems[i].ToString() + "), " + textBox5.Text + ")\r\n";
                }
                cmd += "\r\n";
            }
            with_current_df_cmd += cmd;
            textBox6.Text = with_current_df_cmd;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string cmd = "";

            string cmd1 = tft_header_ru();


            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += with_current_df_cmd;

            cmd += "\r\n";
            cmd += "#write.csv(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "fwrite(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            string file = string.Format("reshape2_melt{0}.r", output_idx);
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write(cmd1);
                    sw.Write(cmd);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + string.Format("{0}", output_idx);
            output_idx += 1;
            listBox_remake(false, true);
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;

        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0) return;
            listBox3.Items.Add(string.Format("lag {0} {1}", listBox4.SelectedItem.ToString(), numericUpDown1.Value.ToString()));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1]+"\r\n";
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0) return;
            listBox3.Items.Add(string.Format("mean {0} {1} {2}", listBox4.SelectedItem.ToString(), numericUpDown2.Value.ToString(),checkBox2.Checked?1:0));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0) return;
            listBox3.Items.Add(string.Format("sd {0} {1} {2}", listBox4.SelectedItem.ToString(), numericUpDown2.Value.ToString(), checkBox3.Checked ? 1 : 0));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0) return;
            listBox3.Items.Add(string.Format("min {0} {1} {2}", listBox4.SelectedItem.ToString(), numericUpDown2.Value.ToString(), checkBox4.Checked ? 1 : 0));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0) return;
            listBox3.Items.Add(string.Format("max {0} {1} {2}", listBox4.SelectedItem.ToString(), numericUpDown2.Value.ToString(), checkBox5.Checked ? 1 : 0));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button20_Click(object sender, EventArgs e)
        {
            string cmd = "";

            string cmd1 = tft_header_ru();

            cmd += "#df <- read.csv(\"" + base_name + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "#df2 <- read.csv(\"" + base_name2 + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";

            cmd += with_current_df_cmd;

            feature_cmd.Items.Clear();
            for (int i = 0; i < listBox3.Items.Count; i++)
            {
                feature_cmd.Items.Add(listBox3.Items[i]);
            }

            ListBox addfeature_cmd = new ListBox();

            textBox7.Text = "";
            int skip_row_max = 0;
            if (listBox3.SelectedItems.Count >= 1)
            {
                for (int i = 0; i < listBox3.SelectedItems.Count; i++)
                {
                    var args = listBox3.SelectedItems[i].ToString().Split(' ');

                    if (args[0] == "lag")
                    {
                        addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                        if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                    }
                    if (args[0] == "mean")
                    {
                        if (args[3]=="1")
                        {
                            addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                            addfeature_cmd.Items.Add(string.Format("mean_{0}_{1} = roll_meanr(lag_{2}_{3}, {4})", args[1], args[2], args[1], args[2], args[2]));
                            if (skip_row_max < 2*int.Parse(args[2])-1) skip_row_max = 2 * int.Parse(args[2]) - 1;
                        }
                        else
                        {
                            addfeature_cmd.Items.Add(string.Format("mean_{0}_{1} = roll_meanr({2}, {3})", args[1], args[2], args[1], args[2]));
                            if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                        }
                    }
                    if (args[0] == "sd")
                    {
                        if (args[3] == "1")
                        {
                            addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                            addfeature_cmd.Items.Add(string.Format("sd_{0}_{1} = roll_sdr(lag_{2}_{3}, {4})", args[1], args[2], args[1], args[2], args[2]));
                            if (skip_row_max < 2 * int.Parse(args[2]) - 1) skip_row_max = 2 * int.Parse(args[2]) - 1;
                        }
                        else
                        {
                            addfeature_cmd.Items.Add(string.Format("sd_{0}_{1} = roll_sdr({2}, {3})", args[1], args[2], args[1], args[2]));
                            if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                        }
                    }
                    if (args[0] == "min")
                    {
                        if (args[3] == "1")
                        {
                            addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                            addfeature_cmd.Items.Add(string.Format("min_{0}_{1} = roll_minr(lag_{2}_{3}, {4})", args[1], args[2], args[1], args[2], args[2]));
                            if (skip_row_max < 2 * int.Parse(args[2]) - 1) skip_row_max = 2 * int.Parse(args[2]) - 1;
                        }
                        else
                        {
                            addfeature_cmd.Items.Add(string.Format("min_{0}_{1} = roll_minr({2}, {3})", args[1], args[2], args[1], args[2]));
                            if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                        }
                    }
                    if (args[0] == "max")
                    {
                        if (args[3] == "1")
                        {
                            addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                            addfeature_cmd.Items.Add(string.Format("max_{0}_{1} = roll_maxr(lag_{2}_{3}, {4})", args[1], args[2], args[1], args[2], args[2]));
                            if (skip_row_max < 2 * int.Parse(args[2]) - 1) skip_row_max = 2 * int.Parse(args[2]) - 1;
                        }
                        else
                        {
                            addfeature_cmd.Items.Add(string.Format("mean_{0}_{1} = roll_maxr({2}, {3})", args[1], args[2], args[1], args[2]));
                            if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                        }
                    }

                    if (comboBox5.Text != "")
                    {
                        if (args[0] == "year")
                        {
                            addfeature_cmd.Items.Add(string.Format("year_{0} = as.factor(lubridate::year(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "month")
                        {
                            addfeature_cmd.Items.Add(string.Format("month_{0} = as.factor(lubridate::month(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "wday")
                        {
                            addfeature_cmd.Items.Add(string.Format("wday_{0} = as.factor(lubridate::wday(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "yday")
                        {
                            addfeature_cmd.Items.Add(string.Format("yday_{0} = as.factor(lubridate::yday(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "day")
                        {
                            addfeature_cmd.Items.Add(string.Format("day_{0} = as.factor(lubridate::day(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "hour")
                        {
                            addfeature_cmd.Items.Add(string.Format("hour_{0} = as.factor(lubridate::hour(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "am")
                        {
                            addfeature_cmd.Items.Add(string.Format("am_{0} = as.factor(lubridate::am(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "pm")
                        {
                            addfeature_cmd.Items.Add(string.Format("pm_{0} = as.factor(lubridate::pm(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "minute")
                        {
                            addfeature_cmd.Items.Add(string.Format("minute_{0} = as.factor(lubridate::minute(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "second")
                        {
                            addfeature_cmd.Items.Add(string.Format("second_{0} = as.factor(lubridate::second(" + comboBox5.Text + "))", args[1]));
                        }
                    }
                    textBox7.Text += addfeature_cmd.Items[addfeature_cmd.Items.Count - 1].ToString() + "\r\n";
                }
            }else
            {
                if (MessageBox.Show("No features to add have been selected.", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            if (addfeature_cmd.Items.Count >= 1)
            {
                //addfeature_cmd = removeDup(addfeature_cmd);

                cmd += "df <- df %>% \r\n";
                if (comboBox4.Text != "")
                {
                    cmd += "            group_by(" + comboBox4.Text + ") %>%\r\n";
                }
                cmd += "            mutate(\r\n";

                cmd += "                  " + addfeature_cmd.Items[0].ToString() + "\r\n";
                for (int i = 1; i < addfeature_cmd.Items.Count-1; i++)
                {
                    cmd += "                  ," + addfeature_cmd.Items[i].ToString() + "\r\n";
                }

                if (comboBox4.Text != "")
                {
                    cmd += "                  ," + addfeature_cmd.Items[addfeature_cmd.Items.Count - 1].ToString() + ") %>% \r\n";
                    cmd += "ungroup()\r\n";
                }
                else
                {
                    cmd += "                  ," + addfeature_cmd.Items[addfeature_cmd.Items.Count - 1].ToString() + ")\r\n";
                }
            }

            cmd += "df <- df %>% ";
            if (comboBox4.Text != "")
            {
                cmd += "group_by(" + comboBox4.Text + ") %>%\r\n";
            }
            cmd += "            mutate(sequence_index = row_number())\r\n";
            if (skip_row_max > 0)
            {
                cmd += "df <- df %>%  filter(sequence_index >= (sequence_index[1] + " + skip_row_max + "))\r\n";
            }

            cmd += "\r\n";

            cmd += "\r\n";
            cmd += "if (";
            if (checkBox1.Checked) cmd += "TRUE){\r\n";
            else cmd += "FALSE){\r\n";
            cmd += "    df <- df %>% ";
            if (comboBox4.Text != "")
            {
                cmd += "group_by(" + comboBox4.Text + ") %>%\r\n";
            }
            cmd += "		    mutate(across(where(is.numeric), ~ replace_na(.x, " + textBox8.Text +"))) %>%\r\n";
            cmd += "		    mutate(across(where(is.character), ~  replace_na(.x, \"" + textBox9.Text + "\")))\r\n";
            
            cmd += "    df <- df %>% ";
            if (comboBox4.Text != "")
            {
                cmd += "group_by(" + comboBox4.Text + ") %>%\r\n";
            }
            cmd += "  		    mutate(across(where(is.character), ~ as.factor(.x)))\r\n";
            cmd += "}\r\n";
            cmd += "\r\n";

            cmd += "#write.csv(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "fwrite(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            string file = string.Format("feature{0}.r", output_idx);
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write(cmd1);
                    sw.Write(cmd);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            listBox3.Items.Clear();
            feature_cmd.Items.Clear();
            textBox7.Text = "";

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + string.Format("{0}", output_idx);
            output_idx += 1;
            listBox_remake(false, true);
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                if (listBox4.GetSelected(i)) listBox4.SetSelected(i, false);
                else listBox4.SetSelected(i, true);
            }
        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("day {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("year {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("month {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("week {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("wday {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button27_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("yday {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button28_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("hour {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button29_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("am {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button30_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("pm {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("minute {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button32_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("second {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }
    }
}
