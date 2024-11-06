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
using melt_join;

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
        public int output_idx = 1;
        public string with_current_df_cmd = "";
        public ListBox feature_cmd = new ListBox();

        public string imagePictureBox2 = "";
        public string imagePictureBox4 = "";
        public string imagePictureBox5 = "";
        public string imagePictureBox6 = "";
        public string imagePictureBox7 = "";
        public string imagePictureBox8 = "";
        public int sampling_num_max = 4;
        public int sampling_count = 0;

        public int status = 0;
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

        public int update_output_idx(int idx = -1)
        {
            if (idx >= 0) output_idx = idx;
            output_idx++;
            numericUpDown6.Value = output_idx;

            return output_idx;
        }
        public int update_output_error()
        {
            output_idx--;
            numericUpDown6.Value = output_idx;

            return output_idx;
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
                status = -1;
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
        }
        public static System.Drawing.Image CreateImage(string filename)
        {
            System.Drawing.Image img = null;
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(
                    filename,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read);
                img = System.Drawing.Image.FromStream(fs);
                fs.Close();
            }catch
            {
                img = null;
            }
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
        public string IDCol = "";
        public string TimeCol = "";

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

        public string script_file_ = "";
        public void execute_()
        {
            bool wait = true;
            ProcessStartInfo pInfo = new ProcessStartInfo();
            //pInfo.FileName = textBox1.Text + "\\R.exe";
            //pInfo.Arguments = "CMD BATCH  --vanilla " + script_file;

            //pInfo.FileName = textBox1.Text + "\\Rscript.exe";
            //pInfo.Arguments = "" + script_file;

            pInfo.FileName = textBox1.Text + "\\x64\\Rscript.exe";
            pInfo.Arguments = "" + script_file_;

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
            }
            else
            {
                stopwatch.Start();
                p.Exited += new EventHandler(Proc_Exited);
                p.EnableRaisingEvents = true;
                p.Start();
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
            if (File.Exists("types.txt"))
            {
                File.Delete("types.txt");
            }
            if (File.Exists("TimeStart_End.txt"))
            {
                File.Delete("TimeStart_End.txt");
            }
            if (File.Exists("line.png"))
            {
                File.Delete("line.png");
            }
            if (File.Exists("hist.png"))
            {
                File.Delete("hist.png");
            }


            //string cmd1 = tft_header_ru();

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

            string plt = "";
            if (checkBox8.Checked)
            {
                plt += "plot = TRUE\r\n";
            }else
            {
                plt += "plot = FALSE\r\n";
            }

            plt += "if ( plot ){\r\n";
            plt += "    source(\"../../script/util.r\")\r\n";
            plt += "    plot_hist_df(df)\r\n";
            if (comboBox5.Text != "")
            {
                plt += "    plot_line_df(df,'"+ comboBox5.Text +"')\r\n";
            }
            plt += "}\r\n";

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
                    sw.Write(GetTimeStart_End_cmd());
                    sw.Write(plt);
                }
            }
            catch
            {
                status = -1;
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return null;
            }

            execute(file);
            ListBox list = new ListBox();

            if ( File.Exists("hist.png"))
            {
                imagePictureBox2 = "hist.png";
                pictureBox2.Image = CreateImage(imagePictureBox2);
            }
            else
            {

            }
            if (File.Exists("line.png"))
            {
                imagePictureBox4 = "line.png";
                pictureBox4.Image = CreateImage(imagePictureBox4);
            }
            else
            {

            }
            if (File.Exists("names.txt"))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader("names.txt", Encoding.GetEncoding("SHIFT_JIS"));
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
                            status = -1;
                            MessageBox.Show("Does the column name contain \", \" or \"spaces\"?\n" +
                                "ou may not be getting the column names correctly.", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    }
                    sr.Close();
                }
                catch { sr.Close(); status = -1; }
            }else
            {
                status = -1;
            }

            if(File.Exists("TimeStart_End.txt"))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader("TimeStart_End.txt", Encoding.GetEncoding("SHIFT_JIS"));
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        var txts = line.Split(',');
                        textBox10.Text = txts[0].Replace("\r", "").Replace("\n", "");

                        line = sr.ReadLine();
                        txts = line.Split(',');
                        textBox11.Text = txts[0].Replace("\r", "").Replace("\n", "");
                        if (textBox12.Text == "") textBox12.Text = textBox10.Text;
                        if (textBox17.Text == "") textBox17.Text = textBox11.Text;
                    }
                    sr.Close();
                }
                catch { sr.Close(); status = -1; }
            }else
            {
                status = -1;
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

                StreamReader sr = null;
                try
                {
                    sr = new StreamReader("types.txt", Encoding.GetEncoding("SHIFT_JIS"));
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
                catch { sr.Close(); }
            }
        }

        public string GetTimeStart_End_cmd()
        {
            if (comboBox5.Text == "") return "";

            string cmd = "";
            cmd += "df$" + comboBox5.Text + " <- as.POSIXct(df$" + comboBox5.Text + ", tz ='UTC')\r\n";
            cmd += "if( df$" + comboBox5.Text + "[1] > df$" + comboBox5.Text + "[nrow(df)])\r\n";
            cmd += "{\r\n";
            cmd += "    df <- df[order(df$" + comboBox5.Text + ",decreasing=F),]\r\n";
            cmd += "}\r\n";
            cmd += "\r\n";

            cmd += "sink(file = \"TimeStart_End.txt\")\r\n";
            cmd += "bg = which.min(df$" + comboBox5.Text + ")\r\n";
            cmd += "ed = which.max(df$" + comboBox5.Text + ")\r\n";
            cmd += "if ( bg > ed ){\r\n";
            cmd += "    df$index_number <- c(nrow(df):1)\r\n";
            cmd += "    df <- df[order(df$index_number), ]\r\n";
            cmd += "    bg = which.min(df$" + comboBox5.Text + ")\r\n";
            cmd += "    ed = which.max(df$" + comboBox5.Text + ")\r\n";
            cmd += "}\r\n";
            cmd += "\r\n";

            cmd += "s = sprintf(\"%s,%d\\n\", df$" + comboBox5.Text + "[bg], bg)\r\n";
            cmd += "cat(s)\r\n";
            cmd += "s = sprintf(\"%s,%d\\n\", df$" + comboBox5.Text + "[ed], ed)\r\n";
            cmd += "cat(s)\r\n";

            if (checkBox6.Checked)
            {
                if (numericUpDown3.Value + numericUpDown4.Value + numericUpDown5.Value != 100)
                {
                    MessageBox.Show("The sum of the data split ratios is not 100%", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                cmd += "ds <- unique(df$" + comboBox5.Text + ")\r\n";
                cmd += "train_n = " + numericUpDown3.Value.ToString() + "*0.01\r\n";
                cmd += "valid_n = " + numericUpDown4.Value.ToString() + "*0.01\r\n";
                cmd += "test_n = " + numericUpDown5.Value.ToString() + "*0.01\r\n";
                cmd += "train_st <- ds[1]\r\n";
                cmd += "train_ed <- ds[as.integer(length(ds)*train_n)]\r\n";
                cmd += "valid_st <- ds[(as.integer(length(ds)*train_n))]\r\n";
                cmd += "valid_ed <- ds[(as.integer(length(ds)*train_n)) + as.integer(length(ds)*valid_n)]\r\n";
                cmd += "test_st <-  ds[(as.integer(length(ds)*train_n)) + as.integer(length(ds)*valid_n)]\r\n";
                cmd += "test_ed <- ds[length(ds)]\r\n";
                cmd += "s = sprintf(\"%s,%d\\n\", train_st, -1)\r\n";
                cmd += "cat(s)\r\n";
                cmd += "s = sprintf(\"%s,%d\\n\", train_ed, -1)\r\n";
                cmd += "cat(s)\r\n";
                cmd += "s = sprintf(\"%s,%d\\n\", valid_st, -1)\r\n";
                cmd += "cat(s)\r\n";
                cmd += "s = sprintf(\"%s,%d\\n\", valid_ed, -1)\r\n";
                cmd += "cat(s)\r\n";
                cmd += "s = sprintf(\"%s,%d\\n\", test_st, -1)\r\n";
                cmd += "cat(s)\r\n";
                cmd += "s = sprintf(\"%s,%d\\n\", test_ed, -1)\r\n";
                cmd += "cat(s)\r\n";

                if (comboBox4.Text != "")
                {
                    cmd += "IDs = unique(df$" + comboBox4.Text + ")\r\n";
                    cmd += "tmp <- df %>% filter(" + comboBox4.Text + " == IDs[1])\r\n";
                }else
                {
                    cmd += "tmp <- df\r\n";
                }
                cmd += "dt = abs(difftime(tmp$" + comboBox5.Text + "[2],tmp$" + comboBox5.Text + "[1],  units='secs'))\r\n";
                cmd += "addnum = as.numeric(difftime(as.POSIXct(test_ed, tz='UTC'), as.POSIXct(test_st, tz='UTC'),  units='secs'))/as.numeric(dt)\r\n";
                cmd += "num_step = as.integer(addnum)\r\n";
                cmd += "s = sprintf(\"%s,%d\\n\", \"num_step\", num_step)\r\n";
                cmd += "cat(s)\r\n";

            }
            cmd += "sink()\r\n";

            return (cmd);
        }

        public string GetTimeStep_cmd()
        {
            if (comboBox5.Text == "") return "";

            string cmd = "";
            cmd += "sink(file = \"TimeStep.txt\")\r\n";
            cmd += "bg = which.min(df$"+ comboBox5.Text+")\r\n";
            cmd += "\r\n";

            cmd += "ds <- unique(df$"+ comboBox5.Text+")\r\n";
            cmd += "test_st <-  '" + textBox16.Text + "'\r\n";
            cmd += "test_ed <-  '" + textBox17.Text + "'\r\n";
            cmd += "\r\n";

            cmd += "IDs = unique(df$" + comboBox4.Text + ")\r\n";
            cmd += "tmp <- df %>% filter(" + comboBox4.Text + " == IDs[1])\r\n";
            cmd += "dt = abs(difftime(tmp$data[2],tmp$data[1],  units='secs'))\r\n";
            cmd += "addnum = as.numeric(difftime(as.POSIXct(test_ed, tz='UTC'), as.POSIXct(test_st, tz='UTC'),  units='secs'))/as.numeric(dt)\r\n";
            cmd += "num_step = as.integer(addnum)\r\n";
            cmd += "s = sprintf(\"%s,%d\\n\", \"num_step\", num_step)\r\n";
            cmd += "cat(s)\r\n";
            cmd += "sink()\r\n";

            return (cmd);
        }

        public string GetTypes_cmd()
        {
            string cmd = "";
            cmd += "sink(file = \"types.txt\")\r\n";
            cmd += "tmp <- data.frame(df)\r\n";
            cmd += "cols <- colnames(tmp)\r\n";
            cmd += "for ( i in 1:length(cols) )\r\n";
            cmd += "{\r\n";
            cmd += "	s = sprintf(\"%s %s %s %s %s\\n\", cols[i],\r\n";
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
            cmd += "    library(magrittr)\r\n";
            cmd += "    library(plotly)\r\n";
            cmd += "    library(patchwork)\r\n";
            cmd += "    library(htmlwidgets)\r\n";
            cmd += "})\r\n";
            cmd += "set.seed(1)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "freeram <- function(...) invisible(gc(...))\r\n";
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
                if (listBox4.SelectedItems[0].ToString().Substring(0, 1) == "'")
                {
                    cmd += "              " + listBox4.SelectedItems[0].ToString() + "";
                }
                else
                {
                    cmd += "              '" + listBox4.SelectedItems[0].ToString() + "'";
                }

                for (int i = 1; i < listBox4.SelectedItems.Count; i++)
                {
                    cmd += ", ";
                    if (listBox4.SelectedItems[i].ToString().Substring(0, 1) == "'")
                    {
                        cmd += "" + listBox4.SelectedItems[i].ToString() + "";
                    }
                    else
                    {
                        cmd += "'" + listBox4.SelectedItems[i].ToString() + "'";
                    }

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
                if (listBox1.SelectedItems[0].ToString().Substring(0, 1) == "'")
                {
                    cmd += "              " + listBox1.SelectedItems[0].ToString() + "";
                }else
                {
                    cmd += "              '" + listBox1.SelectedItems[0].ToString() + "'";
                }
                for (int i = 1; i < listBox1.SelectedItems.Count; i++)
                {
                    cmd += ", ";
                    if (listBox1.SelectedItems[i].ToString().Substring(0, 1) == "'")
                    {
                        cmd += "" + listBox1.SelectedItems[i].ToString() + "";
                    }
                    else
                    {
                        cmd += "'" + listBox1.SelectedItems[i].ToString() + "'";
                    }

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
            string file = "melt_join_setting_" + base_name0 + string.Format("{0}", output_idx)+".txt";
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(output_idx.ToString() + "\n");
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

                    sw.Write(listBox2.Items.Count.ToString() + "\n");
                    for (int i = 0; i < listBox2.Items.Count; i++)
                    {
                        sw.Write(listBox2.Items[i].ToString() + "\n");
                    }
                    sw.Write(listBox2.SelectedItems.Count.ToString() + "\n");
                    if (listBox2.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox2.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox2.SelectedIndices[i].ToString() + "\n");
                        }
                    }


                    sw.Write(listBox3.Items.Count.ToString() + "\n");
                    for (int i = 0; i < listBox3.Items.Count; i++)
                    {
                        sw.Write(listBox3.Items[i].ToString() + "\n");
                    }
                    sw.Write(listBox3.SelectedItems.Count.ToString() + "\n");
                    if (listBox3.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox3.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox3.SelectedIndices[i].ToString() + "\n");
                        }
                    }


                    sw.Write(listBox4.Items.Count.ToString() + "\n");
                    for (int i = 0; i < listBox4.Items.Count; i++)
                    {
                        sw.Write(listBox4.Items[i].ToString() + "\n");
                    }
                    sw.Write(listBox4.SelectedItems.Count.ToString() + "\n");
                    if (listBox4.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox4.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox4.SelectedIndices[i].ToString() + "\n");
                        }
                    }

                    sw.Write("textBox2," + textBox2.Text + "\n");
                    sw.Write("textBox3," + textBox3.Text + "\n");
                    sw.Write("textBox4," + textBox4.Text + "\n");
                    sw.Write("textBox5," + textBox5.Text + "\n");
                    sw.Write("textBox6," + textBox6.Text + "\n");
                    sw.Write("textBox7," + textBox7.Text + "\n");
                    sw.Write("textBox8," + textBox8.Text + "\n");
                    sw.Write("textBox9," + textBox9.Text + "\n");
                    sw.Write("textBox10," + textBox10.Text + "\n");
                    sw.Write("textBox11," + textBox11.Text + "\n");
                    sw.Write("textBox12," + textBox12.Text + "\n");
                    sw.Write("textBox13," + textBox13.Text + "\n");
                    sw.Write("textBox14," + textBox14.Text + "\n");
                    sw.Write("textBox15," + textBox15.Text + "\n");
                    sw.Write("textBox16," + textBox16.Text + "\n");
                    sw.Write("textBox17," + textBox17.Text + "\n");
                    sw.Write("textBox18," + textBox18.Text + "\n");
                    sw.Write("textBox19," + textBox19.Text + "\n");
                    sw.Write("textBox20," + textBox20.Text + "\n");
                    sw.Write("textBox21," + textBox21.Text + "\n");
                    sw.Write("textBox22," + textBox22.Text + "\n");
                    sw.Write("textBox23," + textBox23.Text + "\n");
                    sw.Write("textBox24," + textBox24.Text + "\n");
                    sw.Write("textBox25," + textBox25.Text + "\n");

                    sw.Write("comboBox1," + comboBox1.Text + "\n");
                    sw.Write("comboBox2," + comboBox2.Text + "\n");
                    sw.Write("comboBox3," + comboBox3.Text + "\n");
                    sw.Write("comboBox4," + comboBox4.Text + "\n");
                    sw.Write("comboBox5," + comboBox5.Text + "\n");
                    sw.Write("comboBox6," + comboBox6.Text + "\n");
                    sw.Write("comboBox7," + comboBox7.Text + "\n");
                    sw.Write("comboBox8," + comboBox8.Text + "\n");
                    sw.Write("comboBox9," + comboBox9.Text + "\n");
                    sw.Write("comboBox10," + comboBox10.Text + "\n");

                    sw.Write("numericUpDown1," + numericUpDown1.Value.ToString() + "\n");
                    sw.Write("numericUpDown2," + numericUpDown2.Value.ToString() + "\n");
                    sw.Write("numericUpDown3," + numericUpDown3.Value.ToString() + "\n");
                    sw.Write("numericUpDown4," + numericUpDown4.Value.ToString() + "\n");
                    sw.Write("numericUpDown5," + numericUpDown5.Value.ToString() + "\n");
                    sw.Write("numericUpDown6," + numericUpDown6.Value.ToString() + "\n");
                    sw.Write("numericUpDown7," + numericUpDown7.Value.ToString() + "\n");
                    sw.Write("numericUpDown8," + numericUpDown8.Value.ToString() + "\n");
                    sw.Write("numericUpDown9," + numericUpDown9.Value.ToString() + "\n");
                    //sw.Write("numericUpDown10," + numericUpDown10.Value.ToString() + "\n");
                    sw.Write("numericUpDown11," + numericUpDown11.Value.ToString() + "\n");
                    sw.Write("numericUpDown12," + numericUpDown12.Value.ToString() + "\n");
                    sw.Write("numericUpDown13," + numericUpDown13.Value.ToString() + "\n");
                    sw.Write("numericUpDown14," + numericUpDown14.Value.ToString() + "\n");
                    sw.Write("numericUpDown15," + numericUpDown15.Value.ToString() + "\n");
                    sw.Write("numericUpDown16," + numericUpDown16.Value.ToString() + "\n");
                    sw.Write("numericUpDown17," + numericUpDown17.Value.ToString() + "\n");

                    sw.Write("checkBox1," + (checkBox1.Checked?"TRUE":"FALSE") + "\n");
                    sw.Write("checkBox2," + (checkBox2.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("checkBox3," + (checkBox3.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("checkBox4," + (checkBox4.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("checkBox5," + (checkBox5.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("checkBox6," + (checkBox6.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("checkBox7," + (checkBox7.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("checkBox8," + (checkBox8.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("checkBox9," + (checkBox9.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("checkBox10," + (checkBox10.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("checkBox11," + (checkBox11.Checked ? "TRUE" : "FALSE") + "\n");

                    sw.Write("radioButton1," + (radioButton1.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("radioButton2," + (radioButton2.Checked ? "TRUE" : "FALSE") + "\n");
                    sw.Write("radioButton3," + (radioButton3.Checked ? "TRUE" : "FALSE") + "\n");

                    sw.Write("imagePictureBox2,"+ imagePictureBox2 + "\n");
                    sw.Write("imagePictureBox4,"+ imagePictureBox4 + "\n");
                    sw.Write("imagePictureBox5,"+ imagePictureBox5 + "\n");
                    sw.Write("imagePictureBox6,"+ imagePictureBox6 + "\n");
                    sw.Write("imagePictureBox7,"+ imagePictureBox7 + "\n");
                    sw.Write("imagePictureBox8,"+ imagePictureBox8 + "\n");

                    sw.Write("r_path," + textBox1.Text + "\n");
                }
            }
            catch
            {
                status = -1;
                if (MessageBox.Show("Cannot write in "+file , "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
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
            string file = "melt_join_setting_" + base_name0 + string.Format("{0}", output_idx) + ".txt";
            if (setting_file == "")
            {
                if (base_name0 == "")
                {
                    status = -1;
                    MessageBox.Show("input csv file !");
                    return;
                }
                if (!File.Exists(file)) save();

                if (!File.Exists(file))
                {
                    MessageBox.Show("file not found[" + file + ".txt]");
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
                    output_idx = int.Parse(s.Replace("\n", ""));

                    s = sr.ReadLine();
                    int n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        if (setting_file == "")
                        {
                            listBox1.Items.Add(s.Replace("\n", ""));
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
                        if (setting_file == "")
                        {
                            listBox2.Items.Add(s.Replace("\n", ""));
                        }
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
                        if (setting_file == "")
                        {
                            listBox3.Items.Add(s.Replace("\n", ""));
                        }
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
                        if (setting_file == "")
                        {
                            listBox4.Items.Add(s.Replace("\n", ""));
                        }
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


                        if (ss[0].IndexOf("textBox11") >= 0)
                        {
                            textBox11.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox12") >= 0)
                        {
                            textBox12.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox13") >= 0)
                        {
                            textBox13.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox14") >= 0)
                        {
                            textBox14.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox15") >= 0)
                        {
                            textBox15.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox16") >= 0)
                        {
                            textBox16.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox17") >= 0)
                        {
                            textBox17.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox18") >= 0)
                        {
                            textBox18.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox19") >= 0)
                        {
                            textBox19.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox20") >= 0)
                        {
                            textBox20.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox21") >= 0)
                        {
                            textBox21.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox22") >= 0)
                        {
                            textBox22.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox23") >= 0)
                        {
                            textBox23.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox24") >= 0)
                        {
                            textBox24.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox25") >= 0)
                        {
                            textBox25.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox2") >= 0)
                        {
                            textBox2.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox3") >= 0)
                        {
                            textBox3.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox4") >= 0)
                        {
                            textBox4.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox5") >= 0)
                        {
                            textBox5.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox6") >= 0)
                        {
                            textBox6.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox7") >= 0)
                        {
                            textBox7.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox8") >= 0)
                        {
                            textBox8.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox9") >= 0)
                        {
                            textBox9.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("textBox10") >= 0)
                        {
                            textBox10.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }

                        if (ss[0].IndexOf("comboBox10") >= 0)
                        {
                            comboBox10.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("comboBox1") >= 0)
                        {
                            comboBox1.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("comboBox2") >= 0)
                        {
                            comboBox2.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("comboBox3") >= 0)
                        {
                            comboBox3.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("comboBox4") >= 0)
                        {
                            comboBox4.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("comboBox5") >= 0)
                        {
                            comboBox5.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("comboBox6") >= 0)
                        {
                            comboBox6.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("comboBox7") >= 0)
                        {
                            comboBox7.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("comboBox8") >= 0)
                        {
                            comboBox8.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("comboBox9") >= 0)
                        {
                            comboBox9.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }

                        //if (ss[0].IndexOf("numericUpDown10") >= 0)
                        //{
                        //    numericUpDown10.Text = ss[1].Replace("\r\n", "");
                        //    continue;
                        //}
                        if (ss[0].IndexOf("numericUpDown11") >= 0)
                        {
                            numericUpDown11.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown12") >= 0)
                        {
                            numericUpDown12.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown13") >= 0)
                        {
                            numericUpDown13.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown14") >= 0)
                        {
                            numericUpDown14.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown15") >= 0)
                        {
                            numericUpDown15.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown16") >= 0)
                        {
                            numericUpDown16.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown17") >= 0)
                        {
                            numericUpDown17.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        //if (ss[0].IndexOf("numericUpDown18") >= 0)
                        //{
                        //    numericUpDown18.Text = ss[1].Replace("\r\n", "");
                        //    continue;
                        //}
                        //if (ss[0].IndexOf("numericUpDown19") >= 0)
                        //{
                        //    numericUpDown19.Text = ss[1].Replace("\r\n", "");
                        //    continue;
                        //}


                        if (ss[0].IndexOf("numericUpDown1") >= 0)
                        {
                            numericUpDown1.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown2") >= 0)
                        {
                            numericUpDown2.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown3") >= 0)
                        {
                            numericUpDown3.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown4") >= 0)
                        {
                            numericUpDown4.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown5") >= 0)
                        {
                            numericUpDown5.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown6") >= 0)
                        {
                            numericUpDown6.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown7") >= 0)
                        {
                            numericUpDown7.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown8") >= 0)
                        {
                            numericUpDown8.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("numericUpDown9") >= 0)
                        {
                            numericUpDown9.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }

                        if (ss[0].IndexOf("checkBox11") >= 0)
                        {
                            checkBox11.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox10") >= 0)
                        {
                            checkBox10.Checked = (ss[1].Replace("\r\n", "")=="TRUE")?true:false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox1") >= 0)
                        {
                            checkBox1.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox2") >= 0)
                        {
                            checkBox2.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox3") >= 0)
                        {
                            checkBox3.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox4") >= 0)
                        {
                            checkBox4.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox5") >= 0)
                        {
                            checkBox5.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox6") >= 0)
                        {
                            checkBox6.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox7") >= 0)
                        {
                            checkBox7.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox8") >= 0)
                        {
                            checkBox8.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("checkBox9") >= 0)
                        {
                            checkBox9.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }

                        if (ss[0].IndexOf("radioButton1") >= 0)
                        {
                            radioButton1.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("radioButton2") >= 0)
                        {
                            radioButton2.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }
                        if (ss[0].IndexOf("radioButton3") >= 0)
                        {
                            radioButton3.Checked = (ss[1].Replace("\r\n", "") == "TRUE") ? true : false;
                            continue;
                        }

                        if (ss[0].IndexOf("imagePictureBox2") >= 0)
                        {
                            imagePictureBox2 = ss[1].Replace("\r\n", "");
                            pictureBox2.Image = CreateImage(imagePictureBox2);
                            continue;
                        }
                        if (ss[0].IndexOf("imagePictureBox4") >= 0)
                        {
                            imagePictureBox4 = ss[1].Replace("\r\n", "");
                            pictureBox4.Image = CreateImage(imagePictureBox4);
                            continue;
                        }
                        if (ss[0].IndexOf("imagePictureBox5") >= 0)
                        {
                            imagePictureBox5 = ss[1].Replace("\r\n", "");
                            pictureBox5.Image = CreateImage(imagePictureBox5);
                            continue;
                        }
                        if (ss[0].IndexOf("imagePictureBox6") >= 0)
                        {
                            imagePictureBox6 = ss[1].Replace("\r\n", "");
                            pictureBox6.Image = CreateImage(imagePictureBox6);
                            continue;
                        }
                        if (ss[0].IndexOf("imagePictureBox7") >= 0)
                        {
                            imagePictureBox7 = ss[1].Replace("\r\n", "");
                            pictureBox7.Image = CreateImage(imagePictureBox7);
                            continue;
                        }
                        if (ss[0].IndexOf("imagePictureBox8") >= 0)
                        {
                            imagePictureBox8 = ss[1].Replace("\r\n", "");
                            pictureBox8.Image = CreateImage(imagePictureBox7);
                            continue;
                        }



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

            if (!join)
            {
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
                    if ( IDCol != "" && IDCol == listBox4.Items[i].ToString())
                    { 
                        comboBox4.Text = IDCol;
                    }
                    if (TimeCol != "" && TimeCol == listBox4.Items[i].ToString())
                    {
                        comboBox5.Text = TimeCol;
                    }
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

            string tmp = Path.GetDirectoryName(work_dir + "\\" + base_name + ".csv");
            if (csv_dir != Path.GetDirectoryName(work_dir + "\\"+base_name + ".csv"))
            {
                File.Copy(csv_file, base_name + ".csv", true);
            }
            base_name0 = base_name;

            if (id > 1)
            {
                if (!File.Exists(work_dir + string.Format("\\{0}{1}", base_name0, id) + ".csv"))
                {
                    return;
                }
                base_name = string.Format("{0}{1}", base_name0, id);
                output_idx = id;
            }
            numericUpDown6.Value = output_idx;

            this.Text = "[" + base_name + "]";
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;

            clear();

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
                if (IDCol != "" && IDCol == listBox4.Items[i].ToString())
                {
                    comboBox4.Text = IDCol;
                }
                if (TimeCol != "" && TimeCol == listBox4.Items[i].ToString())
                {
                    comboBox5.Text = TimeCol;
                }
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
            if (File.Exists(base_name0 + "_train.csv"))
            {
                File.Delete(base_name0 + "_train.csv");
            }
            if (File.Exists(base_name0 + "_valid.csv"))
            {
                File.Delete(base_name0 + "_valid.csv");
            }
            if (File.Exists(base_name0 + "_test.csv"))
            {
                File.Delete(base_name0 + "_test.csv");
            }
            if (File.Exists("names.txt"))
            {
                File.Delete("names.txt");
            }
            if (File.Exists("types.txt"))
            {
                File.Delete("types.txt");
            }
            if (File.Exists("TimeStart_End.txt"))
            {
                File.Delete("TimeStart_End.txt");
            }
            if (File.Exists("TimeStep.txt"))
            {
                File.Delete("TimeStep.txt");
            }
            if (File.Exists("line.png"))
            {
                File.Delete("line.png");
            }
            if (File.Exists("hist.png"))
            {
                File.Delete("hist.png");
            }

            if (File.Exists("importance.png"))
            {
                File.Delete("importance.png");
            }
            if (File.Exists("predict1.png"))
            {
                File.Delete("predict1.png");
            }
            if (File.Exists("predict2.png"))
            {
                File.Delete("predict2.png");
            }
            if (File.Exists("predict_measure.png"))
            {
                File.Delete("predict_measure.png");
            }
            if (File.Exists("hist.html"))
            {
                File.Delete("hist.html");
            }
            if (File.Exists("line.html"))
            {
                File.Delete("line.html");
            }
            if (File.Exists("importance.html"))
            {
                File.Delete("importance.html");
            }
            if (File.Exists("predict1.html"))
            {
                File.Delete("predict1.html");
            }
            if (File.Exists("predict2.html"))
            {
                File.Delete("predict2.html");
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
            status = 0;
            string cmd = "";

            string cmd1 = tft_header_ru();


            cmd = melt(base_name);

            if (numericUpDown8.Value > 0)
            {
                cmd += "tmp <- df\r\n";
                cmd += "for (n in 1:"+ numericUpDown8.Value.ToString()+") {\r\n";
                cmd += "    column_name = sprintf('" + textBox18.Text + "'," + numericUpDown7.Value.ToString() + " +n, sep = \"\")\r\n";
                cmd += "    tmp = tmp %>% mutate(!!column_name := 0)\r\n";
                cmd += "}\r\n";
                cmd += "df <- tmp\r\n";
                cmd += "rm(tmp)\r\n";
                cmd += "freeram()\r\n";
            }
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
                status = -1;
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + string.Format("{0}", output_idx);
            update_output_idx();
            
            listBox_remake(false, true);
            if (status != 0)
            {
                update_output_error();
                base_name = base_name0 + string.Format("{0}", output_idx);
                return;
            }
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                if (listBox4.GetSelected(i)) listBox1.SetSelected(i, false);
                else
                {
                    listBox1.SetSelected(i, true);

                    if (comboBox4.Text != "")
                    {
                        if (listBox1.Items[i].ToString() == comboBox4.Text)
                        { listBox1.SetSelected(i, false); }
                    }
                    if (comboBox5.Text != "")
                    {
                        if (listBox1.Items[i].ToString() == comboBox5.Text)
                        { listBox1.SetSelected(i, false); }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            status = 0;
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
                status = -1;
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
                status = -1;
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + string.Format("{0}", output_idx);
            update_output_idx();

            listBox_remake(false, true);
            if (status != 0)
            {
                update_output_error();
                base_name = base_name0 + string.Format("{0}", output_idx);
                return;
            }
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
                listBox2.Enabled = false;
                listBox3.Enabled = false;
                listBox4.SelectionMode = SelectionMode.MultiSimple;
                listBox2.SelectionMode = SelectionMode.One;
                listBox3.SelectionMode = SelectionMode.One;
                label16.Text = "select vars";
                label11.Text = "";
                label14.Text = "";
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
            if (tabControl1.SelectedIndex == 2)
            {
                listBox4.Enabled = true;
                listBox1.Enabled = true;
                listBox2.Enabled = false;
                listBox3.Enabled = false;

                listBox4.SelectionMode = SelectionMode.MultiSimple;
                listBox2.SelectionMode = SelectionMode.MultiSimple;
                label16.Text = "select vars";
                label11.Text = "";
                label14.Text = "";
                label3.Text = "";
            }
            if (tabControl1.SelectedIndex == 3)
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
            if (tabControl1.SelectedIndex == 4)
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
            if (tabControl1.SelectedIndex == 5)
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
            if (tabControl1.SelectedIndex == 6)
            {
                listBox4.Enabled = true;
                listBox1.Enabled = false;
                listBox2.Enabled = false;
                listBox3.Enabled = false;
                label16.Text = "select vars";
                label11.Text = "";
                label14.Text = "Feature value";
                label3.Text = "";
            }
            if(tabControl1.SelectedIndex == 7)
            {
                listBox4.Enabled = true;
                listBox1.Enabled = true;
                listBox2.Enabled = false;
                listBox3.Enabled = false;
                listBox4.SelectionMode = SelectionMode.One;
                listBox2.SelectionMode = SelectionMode.MultiSimple;
                listBox3.SelectionMode = SelectionMode.One;
                label16.Text = "target";
                label11.Text = "Feature";
                label14.Text = "";
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
            if (tabControl1.SelectedIndex == 8)
            {
                listBox4.Enabled = true;
                listBox1.Enabled = true;
                listBox2.Enabled = false;
                listBox3.Enabled = false;
                listBox4.SelectionMode = SelectionMode.One;
                listBox2.SelectionMode = SelectionMode.MultiSimple;
                listBox3.SelectionMode = SelectionMode.One;
                label16.Text = "target";
                label11.Text = "Feature";
                label14.Text = "";
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
                status = -1;
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + "_finalOutput.csv";
            update_output_idx();
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
            status = 0;
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
                status = -1;
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + string.Format("{0}", output_idx);
            update_output_idx();

            listBox_remake(false, true);
            if (status != 0)
            {
                update_output_error();
                base_name = base_name0 + string.Format("{0}", output_idx);
                return;
            }
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
            status = 0;
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
            const int LAG_MAX = 100000000;
            int lag_min = LAG_MAX;
            if (listBox3.SelectedItems.Count >= 1)
            {
                for (int i = 0; i < listBox3.SelectedItems.Count; i++)
                {
                    var args = listBox3.SelectedItems[i].ToString().Split(' ');

                    if (args[0] == "lag")
                    {
                        addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                        if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                        if (lag_min > int.Parse(args[2])) lag_min = int.Parse(args[2]);
                    }
                    if (args[0] == "mean")
                    {
                        if (args[3]=="1")
                        {
                            addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                            addfeature_cmd.Items.Add(string.Format("mean_{0}_{1} = roll_meanr(lag_{2}_{3}, {4})", args[1], args[2], args[1], args[2], args[2]));
                            if (skip_row_max < 2*int.Parse(args[2])-1) skip_row_max = 2 * int.Parse(args[2]) - 1;
                            if (lag_min > int.Parse(args[2])) lag_min = int.Parse(args[2]);
                        }
                        else
                        {
                            addfeature_cmd.Items.Add(string.Format("mean_{0}_{1} = roll_meanr({2}, {3})", args[1], args[2], args[1], args[2]));
                            if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                            if (lag_min > int.Parse(args[2])) lag_min = int.Parse(args[2]);
                        }
                    }
                    if (args[0] == "sd")
                    {
                        if (args[3] == "1")
                        {
                            addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                            addfeature_cmd.Items.Add(string.Format("sd_{0}_{1} = roll_sdr(lag_{2}_{3}, {4})", args[1], args[2], args[1], args[2], args[2]));
                            if (skip_row_max < 2 * int.Parse(args[2]) - 1) skip_row_max = 2 * int.Parse(args[2]) - 1;
                            if (lag_min > int.Parse(args[2])) lag_min = int.Parse(args[2]);
                        }
                        else
                        {
                            addfeature_cmd.Items.Add(string.Format("sd_{0}_{1} = roll_sdr({2}, {3})", args[1], args[2], args[1], args[2]));
                            if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                            if (lag_min > int.Parse(args[2])) lag_min = int.Parse(args[2]);
                        }
                    }
                    if (args[0] == "min")
                    {
                        if (args[3] == "1")
                        {
                            addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                            addfeature_cmd.Items.Add(string.Format("min_{0}_{1} = roll_minr(lag_{2}_{3}, {4})", args[1], args[2], args[1], args[2], args[2]));
                            if (skip_row_max < 2 * int.Parse(args[2]) - 1) skip_row_max = 2 * int.Parse(args[2]) - 1;
                            if (lag_min > int.Parse(args[2])) lag_min = int.Parse(args[2]);
                        }
                        else
                        {
                            addfeature_cmd.Items.Add(string.Format("min_{0}_{1} = roll_minr({2}, {3})", args[1], args[2], args[1], args[2]));
                            if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                            if (lag_min > int.Parse(args[2])) lag_min = int.Parse(args[2]);
                        }
                    }
                    if (args[0] == "max")
                    {
                        if (args[3] == "1")
                        {
                            addfeature_cmd.Items.Add(string.Format("lag_{0}_{1} = dplyr::lag({2}, n = {3})", args[1], args[2], args[1], args[2]));
                            addfeature_cmd.Items.Add(string.Format("max_{0}_{1} = roll_maxr(lag_{2}_{3}, {4})", args[1], args[2], args[1], args[2], args[2]));
                            if (skip_row_max < 2 * int.Parse(args[2]) - 1) skip_row_max = 2 * int.Parse(args[2]) - 1;
                            if (lag_min > int.Parse(args[2])) lag_min = int.Parse(args[2]);
                        }
                        else
                        {
                            addfeature_cmd.Items.Add(string.Format("mean_{0}_{1} = roll_maxr({2}, {3})", args[1], args[2], args[1], args[2]));
                            if (skip_row_max < int.Parse(args[2])) skip_row_max = int.Parse(args[2]);
                            if (lag_min > int.Parse(args[2])) lag_min = int.Parse(args[2]);
                        }
                    }

                    if (lag_min < LAG_MAX)
                    {
                        numericUpDown9.Value = lag_min;
                        numericUpDown9.Refresh();
                        label80.Text = lag_min.ToString();
                    }
                    if (comboBox5.Text != "")
                    {
                        args[1] = args[1].Replace(" ", "_");
                        args[1] = args[1].Replace("'", "");

                        if (args[0] == "year")
                        {
                            addfeature_cmd.Items.Add(string.Format("year_{0} = as.integer(lubridate::year(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "quarter")
                        {
                            addfeature_cmd.Items.Add(string.Format("quarter_{0} = as.integer(lubridate::quarter(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "month")
                        {
                            addfeature_cmd.Items.Add(string.Format("month_{0} = as.integer(lubridate::month(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "wday")
                        {
                            addfeature_cmd.Items.Add(string.Format("wday_{0} = as.integer(lubridate::wday(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "yday")
                        {
                            addfeature_cmd.Items.Add(string.Format("yday_{0} = as.integer(lubridate::yday(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "day")
                        {
                            addfeature_cmd.Items.Add(string.Format("day_{0} = as.integer(lubridate::day(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "hour")
                        {
                            addfeature_cmd.Items.Add(string.Format("hour_{0} = as.integer(lubridate::hour(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "am")
                        {
                            addfeature_cmd.Items.Add(string.Format("am_{0} = as.integer(lubridate::am(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "pm")
                        {
                            addfeature_cmd.Items.Add(string.Format("pm_{0} = as.integer(lubridate::pm(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "minute")
                        {
                            addfeature_cmd.Items.Add(string.Format("minute_{0} = as.integer(lubridate::minute(" + comboBox5.Text + "))", args[1]));
                        }
                        if (args[0] == "second")
                        {
                            addfeature_cmd.Items.Add(string.Format("second_{0} = as.integer(lubridate::second(" + comboBox5.Text + "))", args[1]));
                        }
                    }
                    textBox7.Text += addfeature_cmd.Items[addfeature_cmd.Items.Count - 1].ToString() + "\r\n";
                }
            }else
            {
                status = -1;
                if (MessageBox.Show("No features to add have been selected.", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            string feature_gen = "";

            feature_gen += "feature_gen <- function(df, clip=TRUE){\r\n";
            if (addfeature_cmd.Items.Count >= 1)
            {
                //addfeature_cmd = removeDup(addfeature_cmd);
                //cmd += "df <- df[unique(colnames(df))]\r\n";

                if (comboBox5.Text != "")
                {
                    if (comboBox5.Text.Length > 1 && comboBox5.Text.Substring(0, 1) == "'")
                    {
                        //feature_gen += "df$" + comboBox5.Text + " <- as.POSIXct(df$" + comboBox5.Text + ", tz='UTC')\r\n";
                        feature_gen += "df$" + comboBox5.Text + " <- lubridate::as_datetime(df$" + comboBox5.Text + ", tz='UTC')\r\n";
                        //
                    }
                    else
                    {
                        //feature_gen += "df$'" + comboBox5.Text + "' <- as.POSIXct(df$'" + comboBox5.Text + "', tz='UTC')\r\n";
                        feature_gen += "df$'" + comboBox5.Text + "' <- lubridate::as_datetime(df$'" + comboBox5.Text + "', tz='UTC')\r\n";

                    }

                    if (comboBox4.Text != "")
                    {
                        feature_gen += "x <- df %>% filter(" + comboBox4.Text + "== df$" + comboBox4.Text + "[1])\r\n";
                        feature_gen += "if ( x$" + comboBox5.Text + "[1] > x$" + comboBox5.Text + "[2]){\r\n";
                        feature_gen += "    df <- df %>% arrange(data)\r\n";
                        feature_gen += "}\r\n";
                        feature_gen += "rm(x)\r\n";
                        feature_gen += "freeram()\r\n";
                    }
                    else
                    {
                        feature_gen += "if (df$" + comboBox5.Text + "[1] > df$" + comboBox5.Text + "[2]){\r\n";
                        feature_gen += "    df <- df %>% arrange(data)\r\n";
                        feature_gen += "}\r\n";
                    }
                }

                feature_gen += "df <- df %>% \r\n";
                if (comboBox4.Text != "")
                {
                    feature_gen += "            group_by(" + comboBox4.Text + ") %>%\r\n";
                }
                feature_gen += "            mutate(\r\n";

                feature_gen += "                  " + addfeature_cmd.Items[0].ToString() + "\r\n";
                for (int i = 1; i < addfeature_cmd.Items.Count-1; i++)
                {
                    feature_gen += "                  ," + addfeature_cmd.Items[i].ToString() + "\r\n";
                }

                if (comboBox4.Text != "")
                {
                    feature_gen += "                  ," + addfeature_cmd.Items[addfeature_cmd.Items.Count - 1].ToString() + ") %>% \r\n";
                    feature_gen += "ungroup()\r\n";
                }
                else
                {
                    feature_gen += "                  ," + addfeature_cmd.Items[addfeature_cmd.Items.Count - 1].ToString() + ")\r\n";
                }
            }

            feature_gen += "df <- df %>% ";
            if (comboBox4.Text != "")
            {
                feature_gen += "group_by(" + comboBox4.Text + ") %>%\r\n";
            }
            feature_gen += "            mutate(sequence_index = row_number())\r\n";

            if (checkBox11.Checked && comboBox5.Text != "")
            {
                if (comboBox4.Text != "")
                {
                    feature_gen += "IDs = unique(df$" + comboBox4.Text + ")\r\n";
                    feature_gen += "#for ( k in 1:length(IDs))\r\n";
                    feature_gen += "for ( k in 1:1)\r\n";
                    feature_gen += "{\r\n";
                    feature_gen += "    tmp <- df %>% filter("+comboBox4.Text+" == IDs[k])\r\n";
                }
                else
                {
                    feature_gen += "    tmp <- df\r\n";

                }
                feature_gen += "    dt = as.numeric(abs(difftime(tmp$" + comboBox5.Text + "[2],tmp$" + comboBox5.Text + "[1],  units='secs')))\r\n";
                feature_gen += "    \r\n";
                feature_gen += "    if ( 31540000/dt > 1 )\r\n";
                feature_gen += "    {\r\n";
                feature_gen += "    	df$sin_Y = sin(2*pi*df$sequence_index/(31540000/dt))\r\n";
                feature_gen += "    	df$cos_Y = cos(2*pi*df$sequence_index/(31540000/dt))\r\n";
                feature_gen += "    }\r\n";
                feature_gen += "    if ( 2628000/dt  > 1 )\r\n";
                feature_gen += "    {\r\n";
                feature_gen += "	    df$sin_M = sin(2*pi*df$sequence_index/(2628000/dt))\r\n";
                feature_gen += "	    df$cos_M = cos(2*pi*df$sequence_index/(2628000/dt))\r\n";
                feature_gen += "	}\r\n";
                feature_gen += "	if ( 604876.71/dt > 1 )\r\n";
                feature_gen += "	{\r\n";
                feature_gen += "	    df$sin_W = sin(2*pi*df$sequence_index/(604876.71/dt))\r\n";
                feature_gen += "	    df$cos_W = cos(2*pi*df$sequence_index/(604876.71/dt))\r\n";
                feature_gen += "    }\r\n";
                feature_gen += "    \r\n";
                feature_gen += "    if ( 86410.96/dt > 1)\r\n";
                feature_gen += "    {\r\n";
                feature_gen += "    	df$sin_D = sin(2*pi*df$sequence_index/(86410.96/dt))\r\n";
                feature_gen += "    	df$cos_D = cos(2*pi*df$sequence_index/(86410.96/dt))\r\n";
                feature_gen += "    }\r\n";
                if (comboBox4.Text != "")
                {
                    feature_gen += "}\r\n";
                }
            }
            if (skip_row_max > 0)
            {
                feature_gen += "if ( clip ){\r\n";
                feature_gen += "    df <- df %>%  filter(sequence_index >= (sequence_index[1] + " + skip_row_max + "))\r\n";
                feature_gen += "}\r\n\r\n";
            }

            feature_gen += "\r\n";

            feature_gen += "\r\n";
            feature_gen += "if (";
            if (checkBox1.Checked) feature_gen += "TRUE){\r\n";
            else feature_gen += "FALSE){\r\n";
            feature_gen += "    df <- df %>% ";
            if (comboBox4.Text != "")
            {
                feature_gen += "group_by(" + comboBox4.Text + ") %>%\r\n";
            }
            feature_gen += "		    mutate(across(where(is.numeric), ~ replace_na(.x, " + textBox8.Text +"))) %>%\r\n";
            feature_gen += "		    mutate(across(where(is.character), ~  replace_na(.x, \"" + textBox9.Text + "\")))\r\n";

            feature_gen += "    df <- df %>% ";
            if (comboBox4.Text != "")
            {
                feature_gen += "group_by(" + comboBox4.Text + ") %>%\r\n";
            }
            feature_gen += "  		    mutate(across(where(is.character), ~ as.factor(.x)))\r\n";
            feature_gen += "}\r\n";
            feature_gen += "\r\n";
            feature_gen += "return(df);}\r\n";


            cmd += "source('feature_gen_fnc.r')\r\n";
            cmd += "df <- feature_gen(df)\r\n";
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
                status = -1;
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            string file2 = "feature_gen_fnc.r";
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file2, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(feature_gen);
                }
            }
            catch
            {
                status = -1;
                if (MessageBox.Show("Cannot write in " + file2, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            listBox3.Items.Clear();
            feature_cmd.Items.Clear();
            textBox7.Text = "";

            cmd_all += cmd;
            cmd_save();
            execute(file);

            base_name = base_name0 + string.Format("{0}", output_idx);
            update_output_idx();

            listBox_remake(false, true);
            if (status != 0)
            {
                update_output_error();
                base_name = base_name0 + string.Format("{0}", output_idx);
                return;
            }
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



        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void button33_Click_1(object sender, EventArgs e)
        {
            status = 0;
            if (comboBox5.Text == "") return;

            if (File.Exists(base_name0 + "_train.csv"))
            {
                File.Delete(base_name0 + "_train.csv");
            }
            if (File.Exists(base_name0 + "_valid.csv"))
            {
                File.Delete(base_name0 + "_valid.csv");
            }
            if (File.Exists(base_name0 + "_test.csv"))
            {
                File.Delete(base_name0 + "_test.csv");
            }
            string cmd = "";
            string cmd1 = tft_header_ru();

			string date_col = comboBox5.Text;
			if (date_col.Substring(0, 1) != "'")
			{
				date_col = "'"+date_col+"'";
			}
			
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";

            cmd += with_current_df_cmd;

            if (textBox12.Text == "") textBox12.Text = textBox10.Text;
            if (textBox17.Text == "") textBox17.Text = textBox11.Text;
            if (textBox14.Text == "") textBox14.Text = textBox13.Text;
            if (textBox16.Text == "") textBox16.Text = textBox15.Text;

            if (textBox12.Text == "" || textBox13.Text == "") return;
            if (textBox14.Text == "" || textBox15.Text == "") return;
            if (textBox16.Text == "" || textBox17.Text == "") return;

            if (radioButton3.Checked)
            {
                cmd += "use_KDE = T\r\n";
            }else
            {
                cmd += "use_KDE = F\r\n";
            }
            cmd += "df$" + comboBox5.Text + " <- as.POSIXct(df$" + comboBox5.Text + ", tz ='UTC')\r\n";
            cmd += "if( df$" + comboBox5.Text + "[1] > df$" + comboBox5.Text + "[nrow(df)])\r\n";
            cmd += "{\r\n";
            cmd += "    df <- df[order(df$" + comboBox5.Text + ",decreasing=F),]\r\n";
            cmd += "}\r\n";
            cmd += "\r\n";

            cmd += "if (as.POSIXct('" + textBox17.Text + "', tz ='UTC') > max(df$" + comboBox5.Text + "))\r\n";
            cmd += "{\r\n";
            cmd += "\r\n";
            cmd += "    n = 1\r\n";
            cmd += "    N = 0\r\n";
            if (radioButton1.Checked)
            {
                cmd += "    zero_padding = T\r\n";
                cmd += "    random_sampling = F\r\n";
            }
            if (radioButton2.Checked)
            {
                cmd += "    zero_padding = F\r\n";
                cmd += "    random_sampling = T\r\n";
            }
            if (radioButton3.Checked)
            {
                cmd += "    zero_padding = F\r\n";
                cmd += "    random_sampling = F\r\n";
            }

            if (comboBox4.Text == "")
            {
                cmd += "    for ( k in 1:1)\r\n";
            }
            else
            {
                cmd += "    IDs = unique(df$" + comboBox4.Text + ")\r\n";
                cmd += "    for ( k in 1:length(IDs))\r\n";
            }

            cmd += "    {\r\n";
            if (comboBox4.Text == "")
            {
                cmd += "	    tmp <- df\r\n";
            }
            else
            {
                cmd += "	    tmp <- df %>% filter(" + comboBox4.Text + " == IDs[k])\r\n";
            }
            cmd += "        dt = abs(difftime(tmp$" + comboBox5.Text + "[2],tmp$" + comboBox5.Text + "[1],  units='secs'))\r\n";
            cmd += "\r\n";
            cmd += "        endtime = max(tmp$" + comboBox5.Text + ")\r\n";
            cmd += "        addnum = as.numeric(difftime(as.POSIXct('" + textBox17.Text + "', tz='UTC'), endtime,  units='secs'))/as.numeric(dt)\r\n";
            cmd += "        tmp2 = NULL\r\n";
            cmd += "        if ( N == 0 )\r\n";
            cmd += "        {\r\n";
            if (comboBox4.Text == "")
            {
                cmd += "            N = addnum\r\n";
            }
            else
            {
                cmd += "            N = addnum*length(IDs)\r\n";
            }
            cmd += "        }\r\n";
            cmd += "	    for ( i in 1:addnum )\r\n";
            cmd += "	    {\r\n";
            cmd += "		    wrk <- tmp[nrow(tmp),]\r\n";
            cmd += "		    t = endtime + seconds(dt)\r\n";
            cmd += "		    wrk$"+ comboBox5.Text+" <- as.POSIXct(t, origin = '1970-01-01', tz='UTC')\r\n";
            cmd += "\r\n";
            cmd += "            if ( zero_padding ){\r\n";
            if (comboBox4.Text == "")
            {
                cmd += "		       wrk2 <- wrk[,-c(" + date_col + ")]*0\r\n";
            }
            else
            {
                cmd += "		       wrk2 <- wrk[,-c(" + date_col + ",'" + comboBox4.Text + "')]*0\r\n";
            }
            cmd += "            }\r\n";
            cmd += "\r\n";
            cmd += "			if ( random_sampling || use_KDE){\r\n";
            if (comboBox4.Text == "")
            {
                cmd += "			   wrk2 <- wrk[,-c(" + date_col + ")]\r\n";
            }
            else
            {
                cmd += "			   wrk2 <- wrk[,-c(" + date_col + ",'" + comboBox4.Text + "')]\r\n";
            }
            cmd += "			   names <- colnames(wrk2)\r\n";
            cmd += "               for ( j in 1:length(names)){\r\n";
            //cmd += "                   if( length(grep(\"year_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"quarter_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"month_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"wday_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"yday_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"day_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"hour_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"am_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"pm_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"minute_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"second_\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"sin_Y\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"cos_Y\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"sin_M\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"cos_M\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"sin_D\", names[j]))) next\r\n";
            //cmd += "                   if( length(grep(\"cos_D\", names[j]))) next\r\n";
            cmd += "                   d <- data.frame(tmp)[,names[j]]\r\n";
            cmd += "                   d <- d[(length(d)*0.8):length(d)]\r\n";
            cmd += "                   if ( use_KDE )\r\n";
            cmd += "                   {\r\n";
            cmd += "                       # Kernel Density Estimation\r\n";
            cmd += "                       kde <- density(d)\r\n";
            cmd += "                       #plot(kde, main = 'Kernel Density Estimation')\r\n";
            cmd += "                       #CDF\r\n";
            cmd += "                       kde_cdf <- cumsum(kde$y) / sum(kde$y)\r\n\r\n";
            cmd += "                       # Interpolate cumulative distribution for KDE evaluation points\r\n";
            cmd += "                       kde_cdf_func <- approxfun(kde$x, kde_cdf)\r\n";
            cmd += "                       while(T)\r\n";
            cmd += "                       {\r\n";
            cmd += "                           next_sample <- sample(kde$x, size = 1, prob = kde$y)\r\n";
            cmd += "                           p1 <- kde_cdf_func(next_sample)\r\n";
            cmd += "                           if ( p1 < 0.75 && p1 > 0.35 ) break\r\n";
            cmd += "                        }\r\n";
            cmd += "                   }else\r\n";
            cmd += "                   {\r\n";
            cmd += "                       mean_=mean(d,na.rm=T)\r\n";
            cmd += "                       sd_=sd(d,na.rm=T)\r\n";
            cmd += "                       while(T)\r\n";
            cmd += "                       {\r\n";
            cmd += "                           next_sample <- rnorm(1, mean=mean(d,na.rm=T), sd=sd(d,na.rm=T))\r\n";
            cmd += "                           p1 <- pnorm(next_sample, mean = mean_, sd = sd_)\r\n";
            cmd += "                           if ( p1 < 0.75 && p1 > 0.35 ) break\r\n";
            cmd += "                        }\r\n";
            cmd += "                   }\r\n";
            cmd += "                   wrk2[1,j] <- next_sample\r\n";
            cmd += "			   }\r\n";
            cmd += "			}\r\n";
            if (comboBox4.Text == "")
            {
                cmd += "		    wrk2$" + comboBox5.Text + " <- wrk$" + comboBox5.Text + "\r\n";
                cmd += "		    wrk <- wrk2 %>% dplyr::select('" + comboBox5.Text + "', , everything())\r\n";
            }
            else
            {
                cmd += "		    wrk2$" + comboBox5.Text + " <- wrk$" + comboBox5.Text + "\r\n";
                cmd += "		    wrk2$" + comboBox4.Text + " <- tmp$" + comboBox4.Text + "[1]\r\n";
                cmd += "		    wrk <- wrk2 %>% dplyr::select(c(" + date_col + ",'" + comboBox4.Text + "') , everything())\r\n";
            }
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "		    tmp2 <- bind_rows(tmp2, wrk)\r\n";
            cmd += "		    endtime = max(tmp2$"+ comboBox5.Text+")\r\n";
            cmd += "            if ( n %% 100 == 0)\r\n";
            cmd += "            {\r\n";
            cmd += "                 print( sprintf(\"%d/%d %.3f%%\", n, N, 100*as.integer(1000*n/N)/1000))\r\n";
            cmd += "                 flush.console()\r\n";
            cmd += "            }\r\n";
            cmd += "	    }\r\n";
            cmd += "        tmp2 <- tmp2 %>% dplyr::select(colnames(df))\r\n";
            cmd += "	    df <- bind_rows(df, tmp2)\r\n";
            cmd += "        rm(tmp2)\r\n";
            cmd += "        freeram()\r\n";
            cmd += "    }\r\n";
            cmd += "    fwrite(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "}\r\n";
            cmd += "\r\n";

            string split = "";
            split += "split_data <- function(df){\r\n";
            split += "df <- as.data.frame(df)\r\n";
            split += "df <- df[unique(colnames(df))]\r\n";

            split += "train <- df %>% filter(df$" + comboBox5.Text + ">= as.POSIXct('" + textBox12.Text + "', tz ='UTC') & df$" + comboBox5.Text + " <= as.POSIXct('" + textBox13.Text + "', tz ='UTC'))\r\n";
            split += "valid <- df %>% filter(df$" + comboBox5.Text + "> as.POSIXct('" + textBox14.Text + "', tz ='UTC') & df$" + comboBox5.Text + " <= as.POSIXct('" + textBox15.Text + "', tz ='UTC'))\r\n";
            split += "test  <- df %>% filter(df$" + comboBox5.Text + "> as.POSIXct('" + textBox16.Text + "', tz ='UTC') & df$" + comboBox5.Text + " <= as.POSIXct('" + textBox17.Text + "', tz ='UTC'))\r\n";

            split += "return( list(train, valid, test))\r\n}\r\n";

            cmd += "source('feature_gen_fnc.r')\r\n";
            cmd += "source('split_func.r')\r\n";
            cmd += "df <- feature_gen(df, clip = T)\r\n";
            cmd += "split <- split_data(df)\r\n";
            cmd += "train <- as.data.frame(split[[1]])\r\n";
            cmd += "valid <- as.data.frame(split[[2]])\r\n";
            cmd += "test  <- as.data.frame(split[[3]])\r\n";
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "fwrite(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "fwrite(train,'" + base_name0 + "_train.csv" + "', row.names = FALSE)\r\n";
            cmd += "fwrite(valid,'" + base_name0 + "_valid.csv" + "', row.names = FALSE)\r\n";
            cmd += "fwrite(test,'" + base_name0 + "_test.csv" + "', row.names = FALSE)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            string src = string.Format("split_func.r", output_idx);
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(src, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(split);
                }
            }
            catch
            {
                status = -1;
                if (MessageBox.Show("Cannot write in " + src, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }


            string file = string.Format("split{0}.r", output_idx);
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
                status = -1;
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            cmd_all += cmd;
            cmd_save();
            execute(file);

            if ( !File.Exists( base_name0 + "_train.csv"))
            {
                status = -1;
            }
            if (!File.Exists(base_name0 + "_valid.csv"))
            {
                status = -1;
            }
            if (!File.Exists(base_name0 + "_test.csv"))
            {
                status = -1;
            }
            if ( status < 0)
            {
                if (MessageBox.Show("Error in separating train, validate, and test data frames.", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            base_name = base_name0 + string.Format("{0}", output_idx);
            update_output_idx();

            listBox_remake(false, true);
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (comboBox5.Text == "")
            {
                if (MessageBox.Show("Please specify the time column", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            if (File.Exists("TimeStart_End.txt"))
            {
                File.Delete("TimeStart_End.txt");
            }
            string cmd1 = tft_header_ru();

            string cmd = "";
            cmd += "options(encoding=\"" + encoding + "\")\r\n";
            cmd += ".libPaths(c('" + RlibPath + "',.libPaths()))\r\n";
            cmd += "dir='" + work_dir.Replace("\\", "\\\\") + "'\r\n";
            cmd += "library(data.table)\r\n";
            cmd += "setwd(dir)\r\n";
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";

            string file = "tmp_get_splittime.R";

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write(cmd1);
                    sw.Write(cmd);
                    sw.Write(GetTimeStart_End_cmd());
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            execute(file);


            if (File.Exists("TimeStart_End.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TimeStart_End.txt", Encoding.GetEncoding("SHIFT_JIS"));
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        var txts = line.Split(',');
                        textBox10.Text = txts[0].Replace("\r", "").Replace("\n", "");

                        line = sr.ReadLine();
                        txts = line.Split(',');
                        textBox11.Text = txts[0].Replace("\r", "").Replace("\n", "");

                        if (checkBox6.Checked)
                        {
                            line = sr.ReadLine();
                            txts = line.Split(',');
                            textBox12.Text = txts[0].Replace("\r", "").Replace("\n", "");

                            line = sr.ReadLine();
                            txts = line.Split(',');
                            textBox13.Text = txts[0].Replace("\r", "").Replace("\n", "");

                            line = sr.ReadLine();
                            txts = line.Split(',');
                            textBox14.Text = txts[0].Replace("\r", "").Replace("\n", "");

                            line = sr.ReadLine();
                            txts = line.Split(',');
                            textBox15.Text = txts[0].Replace("\r", "").Replace("\n", "");

                            line = sr.ReadLine();
                            txts = line.Split(',');
                            textBox16.Text = txts[0].Replace("\r", "").Replace("\n", "");

                            line = sr.ReadLine();
                            txts = line.Split(',');
                            textBox17.Text = txts[0].Replace("\r", "").Replace("\n", "");

                            line = sr.ReadLine();
                            txts = line.Split(',');
                            label79.Text = txts[1].Replace("\r", "").Replace("\n", "");
                            if (numericUpDown9.Value.ToString() != "")
                            {
                                if (int.Parse(label79.Text) > numericUpDown9.Value)
                                {
                                    label79.ForeColor = Color.FromArgb(255, 0, 0);
                                }
                                else
                                {
                                    label79.ForeColor = Color.FromArgb(0, 255, 0);
                                }
                            }
                        }
                    }
                    sr.Close();
                    sr = null;
                }
                catch { }
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if ( checkBox6.Checked)
            {
                numericUpDown3.Enabled = true;
                numericUpDown4.Enabled = true;
                numericUpDown5.Enabled = true;
            }else
            {
                numericUpDown3.Enabled = false;
                numericUpDown4.Enabled = false;
                numericUpDown5.Enabled = false;
            }
        }

        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            if (comboBox4.Text != "") IDCol = comboBox4.Text;
        }

        private void comboBox5_TextChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text != "")
            {
                TimeCol = comboBox5.Text;

                string s = TimeCol;
                if (TimeCol.Length > 11)
                {
                    s = TimeCol.Substring(0,8);
                    s += "...";
                }
                label45.Text = s;
                label46.Text = s;
                label47.Text = s;
                label48.Text = s;
                label49.Text = s;
                label50.Text = s;
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            status = 0;
            if (listBox4.SelectedItems.Count == 0)
            {
                if (MessageBox.Show("No columns selected to delete.", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            string cmd = "";

            string cmd1 = tft_header_ru();

            cmd += "#df <- read.csv(\"" + base_name + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";


            if (listBox4.SelectedItems.Count >= 1)
            {
                for (int i = 0; i < listBox4.SelectedItems.Count; i++)
                {
                    cmd += "df$" + listBox4.SelectedItems[i].ToString() + " <- NULL\r\n";
                }
            }

            cmd += "#write.csv(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "fwrite(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            string file = string.Format("delete_columns{0}.r", output_idx);
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
            update_output_idx();

            listBox_remake(false, true);
            if (status != 0)
            {
                update_output_error();
                base_name = base_name0 + string.Format("{0}", output_idx);
                return;
            }
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;
        }

        private void button36_Click(object sender, EventArgs e)
        {
            textBox25.Text = "0.1";     //eta
            textBox19.Text = "1.0";     //min_child_weigth
            textBox20.Text = "1.0";     //subsample
            numericUpDown14.Value = 6;  //max_depth
            textBox24.Text = "0.0";     //gamma
            textBox23.Text = "0.0";     //alpha
            textBox22.Text = "0.0";     //lambda
            textBox21.Text = "0.0";     //colsample_bytree
            numericUpDown13.Value = 0;  //num_class
            numericUpDown12.Value = 3;  //num_thread
        }

        private void button37_Click(object sender, EventArgs e)
        {
            status = 0;
            if (!File.Exists(base_name0 + "_train.csv"))
            {
                status = -1;
            }
            if (!File.Exists(base_name0 + "_valid.csv"))
            {
                status = -1;
            }
            if (!File.Exists(base_name0 + "_test.csv"))
            {
                status = -1;
            }
            if (status < 0)
            {
                if (MessageBox.Show("Split the data frame into training, validation, and testing", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            if (listBox1.SelectedIndices.Count == 0 )
            {
                if (MessageBox.Show("No features selected", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            if (listBox4.SelectedIndices.Count == 0)
            {
                if (MessageBox.Show("No target selected", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            if (File.Exists("importance.png"))
            {
                File.Delete("importance.png");
            }
            if (File.Exists("predict1.png"))
            {
                File.Delete("predict1.png");
            }
            if (File.Exists("predict2.png"))
            {
                File.Delete("predict2.png");
            }
            if (File.Exists("importance.html"))
            {
                File.Delete("importance.html");
            }
            if (File.Exists("predict1.html"))
            {
                File.Delete("predict1.html");
            }
            if (File.Exists("predict2.html"))
            {
                File.Delete("predict2.html");
            }


            string cmd1 = tft_header_ru();
            string cmd = "";

            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "train <- fread(\"" + base_name0 + "_train.csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "valid <- fread(\"" + base_name0 + "_valid.csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "test <- fread(\"" + base_name0 + "_test.csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";

            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "use_features = c(\r\n";
            cmd += "    '" + listBox1.Items[listBox1.SelectedIndices[0]].ToString() + "'";
            for (int i = 1; i < listBox1.SelectedIndices.Count; i++)
            {
                cmd += ",\r\n";
                cmd += "    '" + listBox1.Items[listBox1.SelectedIndices[i]].ToString() + "'";
            }
            cmd += "\r\n)\r\n";

            string train = "";

            train += "training <- function(train, valid, test, use_features){\r\n";
            train += "train_data <- train %>% select(-" + listBox4.SelectedItem.ToString() + ")\r\n";
            train += "train_labels <- train %>% select(" + listBox4.SelectedItem.ToString() + ")\r\n";
            train += "valid_data <- valid %>% select(-" + listBox4.SelectedItem.ToString() + ")\r\n";
            train += "valid_labels <- valid %>% select(" + listBox4.SelectedItem.ToString() + ")\r\n";
            train += "test_data <- test %>% select(-" + listBox4.SelectedItem.ToString() + ")\r\n";
            train += "test_labels <- test %>% select(" + listBox4.SelectedItem.ToString() + ")\r\n";
            train += "\r\n\r\n";
            train += "train_data <- as.data.frame(train_data)\r\n";
            train += "train_labels <- as.data.frame(train_labels)\r\n";
            train += "valid_data <- as.data.frame(valid_data)\r\n";
            train += "valid_labels <- as.data.frame(valid_labels)\r\n";
            train += "test_data <- as.data.frame(test_data)\r\n";
            train += "test_labels <- as.data.frame(test_labels)\r\n";


            train += "train_set_xgb = xgb.DMatrix(data = data.matrix(train_data[, use_features]), label = data.matrix(train_labels))\r\n";
            train += "valid_set_xgb = xgb.DMatrix(data = data.matrix(valid_data[, use_features]), label = data.matrix(valid_labels))\r\n";
            train += "\r\n";
            train += "\r\n";
            train += "use_GPU = ";
            train += checkBox9.Checked ? "TRUE":"FALSE"+"\r\n";
            train += "eta ="+textBox25.Text +"\r\n";
            train += "min_child_weight = "+ textBox19.Text+"\r\n";
            train += "gamma = "+textBox24.Text +"\r\n";
            train += "max_depth= " + numericUpDown14.Value.ToString() + "\r\n";
            train += "if ( use_GPU )\r\n";
            train += "{\r\n";
            train += "	params <- list(booster =  " + comboBox10.Text + "\r\n";
            train += "                   ,min_child_weight = min_child_weight\r\n";
            train += "	               ,tree_method= " + comboBox6.Text + ", gpu_id=0,task_type = \"GPU\"\r\n";
            train += "	               ,objective = " + comboBox9.Text + "\r\n";
            train += "	               ,eta=eta\r\n";
            train += "                   ,gamma=gamma\r\n";
            train += "                   ,max_depth=max_depth)\r\n";
            train += "}else\r\n";
            train += "{\r\n";
            train += "	params <- list(booster = " + comboBox10.Text + "\r\n";
            train += "                   ,min_child_weight = min_child_weight\r\n";
            train += "	               ,tree_method=" + comboBox6.Text + "\r\n";
            train += "	               ,objective = " + comboBox9.Text + "\r\n";
            train += "	               ,eta=eta\r\n";
            train += "                   ,gamma=gamma\r\n";
            train += "                   ,max_depth=max_depth)\r\n";
            train += "}\r\n";

            train += "num_iterations = " + numericUpDown17.Value.ToString() + "\r\n";
            if ( checkBox10.Checked)
            {
                train += "\r\n";
                train += "nrounds = num_iterations\r\n";
                train += "xgb_cv <- xgb.cv(data = train_set_xgb\r\n";
                train += "                  , param = params\r\n";
                train += "                  , maximize = FALSE\r\n";
                train += "                  , evaluation = " + comboBox9.Text + "\r\n"; 
                train += "                  , nrounds = nrounds\r\n";
                train += "                  , nthreads = " + numericUpDown12.Value.ToString() + "\r\n";
                train += "                  , nfold = " + numericUpDown16.Value.ToString() + "\r\n";
                train += "                  , early_stopping_round =" + numericUpDown15.Value.ToString() + "\r\n";
                train += ")\r\n";
                train += "num_iterations = xgb_cv$best_iteration\r\n";
            }
            train += "model_xgb <- xgb.train(data = train_set_xgb,\r\n";
            train += "                              , param = params\r\n";
            train += "                              , maximize = FALSE\r\n";
            train += "                              , eval.metric = " + comboBox8.Text + "\r\n";
            train += "                              , nrounds = num_iterations\r\n";
            train += "                              , watchlist = list(train = train_set_xgb, eval = valid_set_xgb)\r\n";
            train += "                              , early_stopping_round = " + numericUpDown15.Value.ToString() + "\r\n";
            train += "                              , nthread=" + numericUpDown12.Value.ToString() + "\r\n";
            train += ")\r\n";
            train += "\r\n";
            train += "saveRDS(model_xgb, file = \"model_xgb\")\r\n";
            train += "importance <- xgb.importance(feature_names = colnames(train_set_xgb), model = model_xgb)\r\n";
            train += "importance_plt <- xgb.plot.importance(importance_matrix = importance)\r\n";
            train += "importance_plt <- xgb.ggplot.importance(importance, measure = NULL, rel_to_first = T, top_n = 25)\r\n";
            train += "importance_plt + ggplot2::ylab(\"Importance\")\r\n";
            train += "ggsave(file = \"importance.png\", plot = importance_plt, limitsize=F, width = 16, height = 9)\r\n";
            train += "plt_plotly <-ggplotly(importance_plt)\r\n";
            train += "print(plt_plotly)\r\n";
            train += "htmlwidgets::saveWidget(as_widget(plt_plotly), 'importance.html', selfcontained = F)\r\n";


            train += "return(model_xgb)}\r\n";

            cmd += "source('training_fnc.r')\r\n";
            cmd += "model_xgb <- training(train,valid, test, use_features)\r\n";

            string src = string.Format("training_fnc.r", output_idx);
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(src, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(train);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + src, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            string file = string.Format("train{0}.r", output_idx);
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

            if (File.Exists("importance.png"))
            {
                imagePictureBox5 = "importance.png";
                pictureBox5.Image = CreateImage(imagePictureBox5);
            }
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;
        }

        private void button38_Click(object sender, EventArgs e)
        {
            status = 0;
            if (!File.Exists(base_name0 + "_train.csv"))
            {
                status = -1;
            }
            if (!File.Exists(base_name0 + "_valid.csv"))
            {
                status = -1;
            }
            if (!File.Exists(base_name0 + "_test.csv"))
            {
                status = -1;
            }
            if (File.Exists("prdict1.png"))
            {
                File.Delete("prdict1.png");
            }
            if (File.Exists("prdict2.png"))
            {
                File.Delete("prdict2.png");
            }
            if (File.Exists("predict_measure.png"))
            {
                File.Delete("predict_measure.png");
            }

            if (File.Exists("predict1.html"))
            {
                File.Delete("predict1.html");
            }
            if (File.Exists("predict2.html"))
            {
                File.Delete("predict2.html");
            }
            if (File.Exists("progress.txt"))
            {
                File.Delete("progress.txt");
            }

            if (status < 0)
            {
                if (MessageBox.Show("No data frame used for predictive testing", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            string cmd1 = tft_header_ru();
            string cmd = "";

            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "train <- fread(\"" + base_name0 + "_train.csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "valid <- fread(\"" + base_name0 + "_valid.csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "test <- fread(\"" + base_name0 + "_test.csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "use_features = c(\r\n";
            cmd += "    '" + listBox1.Items[listBox1.SelectedIndices[0]].ToString() + "'";
            for (int i = 1; i < listBox1.SelectedIndices.Count; i++)
            {
                cmd += ",\r\n";
                cmd += "    '" + listBox1.Items[listBox1.SelectedIndices[i]].ToString() + "'";
            }
            cmd += "\r\n)\r\n";

            string prediction = "";

            prediction += "prediction <- function(test, model_xgb){\r\n";
            prediction += "test_data <- test %>% select(-" + listBox4.SelectedItem.ToString() + ")\r\n";
            prediction += "test_labels <- test %>% select(" + listBox4.SelectedItem.ToString() + ")\r\n";
            prediction += "\r\n";
            prediction += "test_data <- as.data.frame(test_data)\r\n";
            prediction += "test_labels <- as.data.frame(test_labels)\r\n";
            prediction += "\r\n";

            prediction += "test_set_xgb = xgb.DMatrix(data = data.matrix(test_data[, use_features]), label = data.matrix(test_labels))\r\n";
            prediction += "\r\n";
            prediction += "\r\n";
            prediction += "pred = predict(model_xgb, test_set_xgb)\r\n";
            prediction += "\r\n";
            prediction += "predict <- test\r\n";
            prediction += "predict$predict <- pred\r\n";
            prediction += "return(predict)}\r\n";


            string recursive_Feature = "";
            recursive_Feature += "recursive_Feature_predict <- function(df, train, valid, test, model_xgb, recursive_step, sampling_max=1, sampling_count=1){\r\n";
            recursive_Feature += "if ( file.exists(\"progress.txt\") && sampling_count==1) file.remove(\"progress.txt\")\r\n";

            recursive_Feature += "test  <- as.data.frame(test)\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "df  <- as.data.frame(df)\r\n";
            recursive_Feature += "test_n <- nrow(test)\r\n";
            recursive_Feature += "#lockback <- as.data.frame(df[1:(nrow(df)-test_n),])\r\n";
            recursive_Feature += "lockback <- bind_rows(train, valid)\r\n";
            recursive_Feature += "\r\n";

            recursive_Feature += "test_org <- test\r\n";
            recursive_Feature += "nn <- nrow(test)\r\n";
            recursive_Feature += "obs <- test$" + listBox4.SelectedItem.ToString() + "\r\n";
            if (comboBox4.Text != "")
            {
                recursive_Feature += "n = length(unique(test$" + comboBox4.Text + "))\r\n";
            }
            else
            {
                recursive_Feature += "n = 1\r\n";
            }
            recursive_Feature += "\r\n";
            recursive_Feature += "lag_min = recursive_step\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "s = 1\r\n";
            recursive_Feature += "e = lag_min*n\r\n";
            recursive_Feature += "\r\n";

            recursive_Feature += "exclude_patterns <- c(\r\n";
            if (comboBox4.Text != "")
            {
                recursive_Feature += "	                        \"" + comboBox4.Text + "\",\r\n";
            }
            recursive_Feature += "	                        \"year_\",\r\n";
            recursive_Feature += "	                        \"quarter_\",\r\n";
            recursive_Feature += "	                        \"month_\",\r\n";
            recursive_Feature += "	                        \"wday_\",\r\n";
            recursive_Feature += "	                        \"yday_\",\r\n";
            recursive_Feature += "	                        \"day_\",\r\n";
            recursive_Feature += "	                        \"hour_\",\r\n";
            recursive_Feature += "	                        \"am_\",\r\n";
            recursive_Feature += "	                        \"pm_\",\r\n";
            recursive_Feature += "	                        \"minute_\",\r\n";
            recursive_Feature += "	                        \"second_\",\r\n";
            recursive_Feature += "	                        \"sin_Y\",\r\n";
            recursive_Feature += "	                        \"cos_Y\",\r\n";
            recursive_Feature += "	                        \"sin_M\",\r\n";
            recursive_Feature += "	                        \"cos_M\",\r\n";
            recursive_Feature += "	                        \"sin_D\",\r\n";
            recursive_Feature += "	                        \"cos_D\")\r\n";
            recursive_Feature += "\r\n";


            recursive_Feature += "for ( k in 1:10000)\r\n";
            recursive_Feature += "{\r\n";
            recursive_Feature += "	if ( e >= nn) e = nn\r\n";
            recursive_Feature += "	d <- c(s:e)\r\n";
            recursive_Feature += "	x <- test[d,]\r\n";
            recursive_Feature += "	x[is.na(x)] <- 0\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "	#predict <- prediction(x,model_xgb)\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "	#test$" + listBox4.SelectedItem.ToString() + "[d] <- predict$predict\r\n";
            recursive_Feature += "	\r\n";
            recursive_Feature += "	xx <- bind_rows(lockback,test)\r\n";
            recursive_Feature += "	xx <- feature_gen(xx, clip = T)\r\n";
            //recursive_Feature += "  xx <- xx %>% \r\n";

            //if (comboBox4.Text != "")
            //{
            //    recursive_Feature += "   group_by('"+ comboBox4.Text+"') %>%\r\n";
            //}
            //recursive_Feature += "      mutate(sequence_index = row_number())\r\n";
            recursive_Feature += "	test <- xx[(nrow(xx)-test_n+1):nrow(xx),]\r\n";
            recursive_Feature += "\r\n";

            //sampline 
            recursive_Feature += "\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "### Sampling probable values ​​of explanatory variables from the past\r\n";

            if (radioButton1.Checked)
            {
                recursive_Feature += "  random_sampling = F\r\n";
                recursive_Feature += "  use_KDE = F\r\n";
            }
            if (radioButton2.Checked)
            {
                recursive_Feature += "  random_sampling = T\r\n";
                recursive_Feature += "  use_KDE = F\r\n";
            }
            if (radioButton3.Checked)
            {
                recursive_Feature += "  random_sampling = T\r\n";
                recursive_Feature += "  use_KDE = T\r\n";
            }
            recursive_Feature += "#  random_sampling = F\r\n";
            recursive_Feature += "#  use_KDE = F\r\n";

            recursive_Feature += "	if ( random_sampling ){\r\n";
            if (comboBox5.Text == "")
            {
                if (MessageBox.Show("Specify the column name indicating the time", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            if (listBox4.SelectedIndex < 0)
            {
                if (MessageBox.Show("Specify the objective variable", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            recursive_Feature += "   	   wrk2 <- test %>% as.data.frame() %>% dplyr::select(-'" + comboBox5.Text + "',-'" + listBox4.SelectedItem.ToString() + "' )\r\n";

            //if (comboBox4.Text != "" && comboBox5.Text != "")
            //{
            //    recursive_Feature += "   	   wrk2 <- test %>% as.data.frame() %>% dplyr::select(-'"+comboBox5.Text+ "',-'" + comboBox4.Text + "' )\r\n";
            //}
            //if (comboBox4.Text == "" && comboBox5.Text != "")
            //{
            //    recursive_Feature += "   	   wrk2 <- test %>% as.data.frame() %>% dplyr::select(-'" + comboBox5.Text + "')\r\n";
            //}
            //if (comboBox4.Text != "" && comboBox5.Text == "")
            //{
            //    recursive_Feature += "   	   wrk2 <- test %>% as.data.frame() %>% dplyr::select(-'" + comboBox4.Text + "')\r\n";
            //}
            //if (comboBox4.Text == "" && comboBox5.Text == "")
            //{
            //    recursive_Feature += "   	   wrk2 <- test %>% as.data.frame()\r\n";
            //}


            recursive_Feature += "	   wrk2 <- wrk2[d,]\r\n";
            recursive_Feature += "	   \r\n";
            recursive_Feature += "	   names <- colnames(wrk2)\r\n";
            recursive_Feature += "	   valid_names <- names[!sapply(names, function(name) any(grepl(paste(exclude_patterns, collapse = \"|\"), name)))]\r\n";

            if (comboBox4.Text != "")
            {
                recursive_Feature += "	   df_tmp <- data.frame(df)[,c('" + comboBox4.Text + "',valid_names), drop = FALSE]\r\n";
            }

            recursive_Feature += "	   for ( ii in 1:length(d))\r\n";
            recursive_Feature += "	   {\r\n";
            if (comboBox4.Text != "")
            {
                recursive_Feature += "	       IDs <- wrk2$" + comboBox4.Text + "[ii]\r\n";
                recursive_Feature += "	       x <- df_tmp %>% dplyr::filter(" + comboBox4.Text + " == IDs)\r\n";
            }
            recursive_Feature += "	       for ( name in valid_names){\r\n";
            if (comboBox4.Text == "")
            {
                recursive_Feature += "             df_tmp <- data.frame(name = df[,c(name)])\r\n";
                recursive_Feature += "             x <- df_tmp\r\n";
            }
            recursive_Feature += "             x <- x[(nrow(x)*0.2):nrow(x), , drop = FALSE]\r\n";

            recursive_Feature += "\r\n";
            recursive_Feature += "             if ( use_KDE )\r\n";
            recursive_Feature += "             {\r\n";
            recursive_Feature += "                 # Kernel Density Estimation\r\n";
            recursive_Feature += "                 kde <- density(x[[name]])\r\n";
            recursive_Feature += "                 #plot(kde, main = 'Kernel Density Estimation')\r\n";
            recursive_Feature += "                 #next_sample <- sample(kde$x, size = 1, prob = kde$y)\r\n";

            recursive_Feature += "                 #CDF\r\n";
            recursive_Feature += "                 kde_cdf <- cumsum(kde$y) / sum(kde$y)\r\n\r\n";
            recursive_Feature += "                 # Interpolate cumulative distribution for KDE evaluation points\r\n";
            recursive_Feature += "                 kde_cdf_func <- approxfun(kde$x, kde_cdf)\r\n";
            recursive_Feature += "                 #repeat\r\n";
            recursive_Feature += "                 #{\r\n";
            recursive_Feature += "                     next_sample <- sample(kde$x, size = 1, prob = kde$y)\r\n";
            recursive_Feature += "                     #p1 <- kde_cdf_func(next_sample)\r\n";
            recursive_Feature += "                     #if ( p1 < 0.75 && p1 > 0.35 ) break\r\n";
            recursive_Feature += "                 #}\r\n";
            
            recursive_Feature += "	           }else\r\n";
            recursive_Feature += "	           {\r\n";
            recursive_Feature += "                 mean_=mean(d,na.rm=T)\r\n";
            recursive_Feature += "                 sd_=sd(d,na.rm=T)\r\n";
            recursive_Feature += "                 #repeat\r\n";
            recursive_Feature += "                 #{\r\n";
            recursive_Feature += "                     next_sample <- rnorm(1, mean=mean(d,na.rm=T), sd=sd(d,na.rm=T))\r\n";
            recursive_Feature += "                     #p1 <- pnorm(next_sample, mean = mean_, sd = sd_)\r\n";
            recursive_Feature += "                     #if ( p1 < 0.75 && p1 > 0.35 ) break\r\n";
            recursive_Feature += "                 #}\r\n";

 
            recursive_Feature += "	           }\r\n";
            recursive_Feature += "	           wrk2[,name][ii] <- next_sample\r\n";
            recursive_Feature += "		   }\r\n";
            recursive_Feature += "	    }\r\n";
            recursive_Feature += "	    \r\n";
            recursive_Feature += "	    test=as.data.frame(test)\r\n";
            recursive_Feature += "	    wrk2=as.data.frame(wrk2)\r\n";
            recursive_Feature += "	    test[d, names] <- wrk2[,names]\r\n";
            recursive_Feature += "	}\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "\r\n";
            //


            recursive_Feature += "	test <- as.data.frame(test)\r\n";
            recursive_Feature += "	predict <- prediction(test,model_xgb)\r\n";
            recursive_Feature += "	test$" + listBox4.SelectedItem.ToString() + " <- predict$predict\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "	#print(sum(test$" + listBox4.SelectedItem.ToString() + " - obs))\r\n";
            recursive_Feature += "	s = e + 1\r\n";
            recursive_Feature += "	e = s + lag_min*n-1\r\n";
            recursive_Feature += "	\r\n";
            recursive_Feature += "	if ( s >= nn) break\r\n";
            recursive_Feature += "	if ( e >= nn) e = nn\r\n";
            recursive_Feature += "\r\n";

            //progress
            recursive_Feature += "## progress\r\n";
            recursive_Feature += "        print(sprintf(\"%d/%d %.3f%%\", as.integer(e*sampling_count/sampling_max), nn, 100*as.integer(1000*e*(sampling_count/sampling_max)/nn)/1000.0))\r\n";
            recursive_Feature += "        tryCatch({\r\n";
            recursive_Feature += "            sink(\"progress.txt\")\r\n";
            recursive_Feature += "            cat(as.integer(e*sampling_count/sampling_max))\r\n";
            recursive_Feature += "            cat (\"/\")\r\n";
            recursive_Feature += "            cat(nn)\r\n";
            recursive_Feature += "            cat(\"\\r\\n\")\r\n";
            recursive_Feature += "            flush.console()\r\n";
            recursive_Feature += "            sink()\r\n";
            recursive_Feature += "        },\r\n";
            recursive_Feature += "        error = function(e) {\r\n";
            recursive_Feature += "            sink()\r\n";
            recursive_Feature += "        },\r\n";
            recursive_Feature += "        finally   = {\r\n";
            recursive_Feature += "        },silent = TRUE )\r\n";
            recursive_Feature += "\r\n";

            recursive_Feature += "}\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "predict <- test_org\r\n";
            recursive_Feature += "#predict <- test\r\n";

            recursive_Feature += "predict$predict <- test$" + listBox4.SelectedItem.ToString() + "\r\n";
            recursive_Feature += "predict$" + listBox4.SelectedItem.ToString() + " <- obs\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "return(predict)}\r\n";
            recursive_Feature += "\r\n";
            recursive_Feature += "\r\n";

            cmd += "source('prediction_fnc.r')\r\n";
            cmd += "model_xgb <- readRDS(\"model_xgb\")\r\n";

            timer1.Stop();
            timer1.Enabled = false;
            progressBar1.Value = 0;

            if (checkBox7.Checked)
            {
                if (numericUpDown6.Value < 1)
                {
                    if (MessageBox.Show("recursive_step=0", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        return;
                }
                timer1.Enabled = true;
                timer1.Start();

                cmd += "source('feature_gen_fnc.r')\r\n";
                cmd += "sampling_num_max <- " + sampling_num_max.ToString() + "\r\n";
                cmd += "recursive_step = " + numericUpDown9.Value.ToString() + "\r\n";
                cmd += "source('recursive_Feature_prediction_fnc.r')\r\n";
                cmd += "predict <- recursive_Feature_predict(df, train, valid, test,model_xgb, recursive_step, sampling_num_max, 1)\r\n";
                cmd += "predict$upper <- predict$predict\r\n";
                cmd += "predict$lower <- predict$predict\r\n";
                cmd += "for ( i in 2:sampling_num_max ){\r\n";
                cmd += "    predict_ <- recursive_Feature_predict(df, train, valid, test,model_xgb, recursive_step, sampling_num_max, i)\r\n";
                cmd += "    predict$predict <-  predict$predict + predict_$predict\r\n";
                cmd += "    for ( j in 1:length(predict$predict)){\r\n";
                cmd += "        predict$upper[j] <- max(predict$upper[j], predict_$predict[j])\r\n";
                cmd += "        predict$lower[j] <- min(predict$lower[j], predict_$predict[j])\r\n";
                cmd += "    }\r\n";
                cmd += "}\r\n";
                cmd += "predict$predict <- predict$predict/(sampling_num_max)\r\n";
            }
            else
            {
                cmd += "predict <- prediction(test,model_xgb)\r\n";
                cmd += "predict$upper <- predict$predict\r\n";
                cmd += "predict$lower <- predict$predict\r\n";
            }

            cmd += "fwrite(predict,'" + base_name0 + "_predict.csv', row.names = FALSE)\r\n";

            cmd += "source(\"../../script/util.r\")\r\n";
            if (comboBox5.Text.Substring(0, 1) == "'")
            {
                cmd += "plot_predict1(" + comboBox5.Text + ",'" + listBox4.SelectedItem.ToString() + "','" + comboBox4.Text + "',train, valid, predict, timeUnit='" + comboBox7.Text + "')\r\n";
                cmd += "plot_predict2(" + comboBox5.Text + ",'" + listBox4.SelectedItem.ToString() + "','" + comboBox4.Text + "',train, valid, predict, timeUnit='" + comboBox7.Text + "')\r\n";

                cmd += "meas <- predict_measure(predict, x=" + comboBox5.Text + ", y='" + listBox4.SelectedItem.ToString() + "',id = '" + comboBox4.Text + "' )\r\n";
            }else
            {
                cmd += "plot_predict1('" + comboBox5.Text + "','" + listBox4.SelectedItem.ToString() + "','" + comboBox4.Text + "',train, valid, predict, timeUnit='" + comboBox7.Text + "')\r\n";
                cmd += "plot_predict2('" + comboBox5.Text + "','" + listBox4.SelectedItem.ToString() + "','" + comboBox4.Text + "',train, valid, predict, timeUnit='" + comboBox7.Text + "')\r\n";

                cmd += "meas <- predict_measure(predict, x='" + comboBox5.Text + "', y='" + listBox4.SelectedItem.ToString() + "',id = '" + comboBox4.Text + "' )\r\n";
            }
            //cmd += "meas_plt <- gridExtra::tableGrob(meas)\r\n";
            //cmd += "plot(meas_plt)\r\n";

            string src = string.Format("recursive_Feature_prediction_fnc.r", output_idx);
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(src, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(recursive_Feature);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + src, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            src = string.Format("prediction_fnc.r", output_idx);
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(src, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(prediction);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + src, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }


            string file = string.Format("predict{0}.r", output_idx);
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

            sampling_count = 0;
            if (checkBox7.Checked)
            {
                script_file_ = file;
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(execute_);
                System.Threading.Thread thread = new System.Threading.Thread(ts);
                thread.Start();

                while (thread.IsAlive)
                {
                    Application.DoEvents();
                }
            }else
            {
                execute(file);
            }

            if (!File.Exists(base_name0 + "_predict.csv"))
            {
                status = -1;
            }
            if (status < 0)
            {
                if (MessageBox.Show("Error in running forecast.", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            if (File.Exists("predict1.png"))
            {
                imagePictureBox6 = "predict1.png";
                pictureBox6.Image = CreateImage(imagePictureBox6);
            }
            else
            {

            }

            if (File.Exists("predict2.png"))
            {
                imagePictureBox7 = "predict2.png";
                pictureBox7.Image = CreateImage(imagePictureBox7);
            }
            else
            {

            }
            if (File.Exists("predict_measure.png"))
            {
                imagePictureBox8 = "predict_measure.png";
                pictureBox8.Image = CreateImage(imagePictureBox8);
            }
            else
            {

            }


            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;
        }

        private void button39_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0 || comboBox5.Text == "") return;
            listBox3.Items.Add(string.Format("quarter {0} 0", comboBox5.Text));
            listBox3.SetSelected(listBox3.Items.Count - 1, true);
            textBox7.Text += listBox3.Items[listBox3.Items.Count - 1] + "\r\n";
        }

        private void button40_Click(object sender, EventArgs e)
        {
            if (comboBox5.Text == "") return;
            if (File.Exists("TimeStep.txt"))
            {
                File.Delete("TimeStep.txt");
            }
            string cmd1 = tft_header_ru();

            string cmd = "";
            cmd += "options(encoding=\"" + encoding + "\")\r\n";
            cmd += ".libPaths(c('" + RlibPath + "',.libPaths()))\r\n";
            cmd += "dir='" + work_dir.Replace("\\", "\\\\") + "'\r\n";
            cmd += "library(data.table)\r\n";
            cmd += "setwd(dir)\r\n";
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";

            string file = "tmp_get_timestep.R";

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write(cmd1);
                    sw.Write(cmd);
                    sw.Write(GetTimeStep_cmd());
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            execute(file);


            if (File.Exists("TimeStep.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TimeStep.txt", Encoding.GetEncoding("SHIFT_JIS"));
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        var txts = line.Split(',');
                        label79.Text = txts[1].Replace("\r", "").Replace("\n", "");
                        if ( int.Parse(label80.Text) > int.Parse(label79.Text))
                        {
                            label79.ForeColor = Color.FromArgb(255, 0, 0);
                        }else
                        {
                            label79.ForeColor = Color.FromArgb(0, 0, 0);
                        }
                    }
                    sr.Close();
                    sr = null;
                }
                catch { }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (imagePictureBox2 == "") return;
            Form2 f = new Form2();

            f.SetFile(work_dir, imagePictureBox2);
            f.pictureBox1.Image = f.CreateImage(imagePictureBox2);

            f.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (imagePictureBox4 == "") return;
            Form2 f = new Form2();

            f.SetFile(work_dir, imagePictureBox4);
            f.pictureBox1.Image = f.CreateImage(imagePictureBox4);

            f.Show();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (imagePictureBox5 == "") return;
            Form2 f = new Form2();

            f.SetFile(work_dir, imagePictureBox5);
            f.pictureBox1.Image = f.CreateImage(imagePictureBox5);

            f.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (imagePictureBox6 == "") return;
            Form2 f = new Form2();

            f.SetFile(work_dir, imagePictureBox6);
            f.pictureBox1.Image = f.CreateImage(imagePictureBox6);

            f.Show();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (imagePictureBox7 == "") return;
            Form2 f = new Form2();

            f.SetFile(work_dir, imagePictureBox7);
            f.pictureBox1.Image = f.CreateImage(imagePictureBox7);

            f.Show();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (imagePictureBox8 == "") return;
            Form2 f = new Form2();

            f.SetFile(work_dir, imagePictureBox8);
            f.pictureBox1.Image = f.CreateImage(imagePictureBox8);

            f.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string line = "";
            System.IO.StreamReader sr = null;
            try
            {
                if (System.IO.File.Exists("progress.txt"))
                {
                    sr = new System.IO.StreamReader("progress.txt");
                    line = sr.ReadLine();
                }
            }
            catch { }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }

            if (line != "")
            {
                line = line.Replace("\r\n", "");
                var count = line.Split('/')[0];
                var tot = line.Split('/')[1];
                float x = float.Parse(count);
                float y = float.Parse(tot);
                float p = 1000.0f*(x / y);
                if (p > 1000.0f) p = 1000.0f;
                //progressBar1.Maximum = int.Parse(tot);
                //progressBar1.Value = int.Parse(count);
                progressBar1.Value = (int)p;

                if (progressBar1.Maximum == progressBar1.Value)
                {
                    timer1.Stop();
                    timer1.Enabled = false;
                }
                progressBar1.Refresh();
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            status = 0;
            if (listBox4.SelectedItems.Count == 0)
            {
                if (MessageBox.Show("No columns selected to ID.", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            string cmd = "";

            string cmd1 = tft_header_ru();

            cmd += "#df <- read.csv(\"" + base_name + ".csv\", header=T, stringsAsFactors = TRUE, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "df <- fread(\"" + base_name + ".csv\", na.strings=c(\"\", \"NULL\"), header = TRUE, stringsAsFactors = TRUE)\r\n";
            cmd += "df <- as.data.frame(df)\r\n";
            cmd += "names <- c(";
            if (listBox4.SelectedItems.Count >= 1)
            {
                cmd += "\"" + listBox4.SelectedItems[0].ToString() + "\"";
                for (int i = 1; i < listBox4.SelectedItems.Count; i++)
                {
                    cmd += ",\"" + listBox4.SelectedItems[i].ToString() + "\"";
                }
            }
            cmd += ")\r\n";

            cmd += "df$Key_Id <- df[,1]\r\n";
            cmd += "x <- df %>% select(names[1])\r\n";
            cmd += "Key_Id <- as.character(x[,1])\r\n";
            cmd += "for ( k in 1:length(names))\r\n";
            cmd += "{\r\n";
            cmd += "    x <- df %>% select(names[k])\r\n";
            cmd += "	y <- as.character(x[,1])\r\n";
            cmd += "	Key_Id <- paste(Key_Id, y,sep='_')\r\n";
            cmd += "}\r\n";
            cmd += "df$Key_Id <- Key_Id\r\n";

            cmd += "#write.csv(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "fwrite(df,'" + base_name0 + string.Format("{0}.csv", output_idx) + "', row.names = FALSE)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            string file = string.Format("make_keyID{0}.r", output_idx);
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
            update_output_idx();

            listBox_remake(false, true);
            if (status != 0)
            {
                update_output_error();
                base_name = base_name0 + string.Format("{0}", output_idx);
                return;
            }
            comboBox4.Text = "Key_Id";
            with_current_df_cmd = "";
            textBox6.Text = with_current_df_cmd;
        }
    }
}
