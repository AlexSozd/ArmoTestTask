using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTask1
{
    public partial class Form1 : Form
    {
        string dir, reg_ex;
        char[] syms;
        List<string> dirlist = new List<string>();
        List<TreeNode> trlist = new List<TreeNode>();
        TimeSpan ts = new TimeSpan();
        DateTime bt;
        public Form1()
        {
            InitializeComponent();
            string name = "userparameters.txt";
            if(File.Exists(name))
            {
                using (StreamReader sr = new StreamReader(name))
                {
                    dir = sr.ReadLine();
                    textBox1.Text = dir;
                    reg_ex = sr.ReadLine();
                    textBox2.Text = reg_ex;
                    textBox3.Text = sr.ReadLine();
                    syms = textBox3.Text.ToArray();
                }
            }
            label2.Text = "";
            label5.Text = "";
            label6.Text = ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds + "." + ts.Milliseconds;
        }

        //Two variants - timer with folders' list or recursion with threads
        //Два варианта - таймер со списком папок или, возможно, рекурсивная функция с созданием пула (асинхронных) потоков

        //Кнопка №1 - "Поиск"
        private void button1_Click(object sender, EventArgs e)
        {
            dir = textBox1.Text;
            reg_ex = "@" + textBox2.Text;
            syms = textBox3.Text.ToArray();
            if(Directory.Exists(dir))
            {
                if (treeView1.Nodes.Count > 0)
                {
                    treeView1.Nodes.Clear();
                    
                    ts = new TimeSpan();
                    label6.Text = ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds + "." + ts.Milliseconds;
                }
                //Begin search
                button1.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                label5.Text = "0";

                bt = DateTime.Now;
                //timer1.Start();
                TreeNode tr = new TreeNode(dir);
                tr.Name = dir;
                treeView1.Nodes.Add(tr);
                
                dirlist.Add(dir);
                trlist.Add(tr);
                /*string[] files = Directory.GetFiles(dir, reg_ex, SearchOption.TopDirectoryOnly);
                foreach (string s in files)
                {
                    tr.Nodes.Add(s);
                    label2.Text = s;
                    label5.Text = (int.Parse(label5.Text) + 1).ToString();
                }
                string[] subdirs = Directory.GetDirectories(dir);
                foreach(string s in subdirs)
                {
                    //Search in subdirs
                    //Rec_search(tr, s);
                    dirlist.Add(s);
                }*/
                timer1.Start();
            }
            else
            {
                MessageBox.Show("Директория с таким именем не найдена");
            }
        }

        //private void Rec_search(TreeNode tr, string dir)
        private void Dir_search()
        {
            //tr.Nodes.Add(dir);
            TreeNode tr = trlist[0], tr1;
            trlist.RemoveAt(0);
            string dir1 = dirlist[0];
            dirlist.Remove(dir1);

            label2.Text = dir1;
            //string[] files = Directory.GetFiles(dir1, reg_ex, SearchOption.TopDirectoryOnly);
            string[] files = Directory.GetFiles(dir1);
            foreach (string s in files)
            {
                if (Regex.IsMatch(s.Substring(s.LastIndexOf('\\') + 1), reg_ex, RegexOptions.IgnoreCase))
                {
                    //tr.Nodes.Add(s);
                    label2.Text = s;
                    if (SymFinded(s))
                    {
                        tr.Nodes.Add(s.Substring(s.LastIndexOf('\\') + 1));
                    }
                    label5.Text = (int.Parse(label5.Text) + 1).ToString();
                }
            }
            string[] subdirs = Directory.GetDirectories(dir1);
            foreach (string s in subdirs)
            {
                //Rec_search(tr, s);
                tr1 = new TreeNode(s.Substring(s.LastIndexOf('\\') + 1));
                tr1.Name = s;
                tr.Nodes.Add(tr1);
                trlist.Add(tr1);
                dirlist.Add(s);
            }
            tr.Expand();
        }
        //Кнопка №2 - "Остановить"
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (dirlist.Count > 0)
            {
                Dir_search();
                ts = ts + (DateTime.Now - bt);
                label6.Text = ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds + "." + ts.Milliseconds;
                bt = DateTime.Now;
            }
            else
            {
                timer1.Enabled = false;
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = false;
                label2.Text = "";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
            }
        }
        //Кнопка №3 - "Продолжить (поиск)"
        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            bt = DateTime.Now;
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
        }

        private bool SymFinded(string fname)
        {
            bool res = false;
            string buf;

            //using (StreamReader sr = new StreamReader(fname))
            using (FileStream sr = File.OpenRead(fname))
            {
                byte[] barr = new byte[sr.Length];
                while (((sr.Read(barr, 0, barr.Length)) > 0) && !res)
                {
                    buf = System.Text.Encoding.Default.GetString(barr);
                    for (int i = 0; i < syms.Length; i++)
                    {
                        if (buf.Contains(syms[i]))
                        {
                            res = true;
                        }
                    }
                }
                
                /*while (((buf = sr.ReadLine()) != null) && !res)
                {
                    for (int i = 0; i < syms.Length; i++)
                    {
                        if (buf.Contains(syms[i]))
                        {
                            res = true;
                        }
                    }
                }*/
            }
            
            return res;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("userparameters.txt"))
            {
                sw.WriteLine(dir);
                sw.WriteLine(reg_ex);
                sw.Write(syms);
            }
        }
    }
}
