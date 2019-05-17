using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Timers;


namespace WindowsFormsApplication1
{
    struct Diagram
    {
        public float value;
        public Color color;
        public Diagram(string Str)
        {
            string[] str = Str.Split(' ');
            int[] rgb = new int[3] { 255, 255, 255 };
            for (int i = 1; i < str.Length; i++)
            {
                if (str[i] != "")
                    rgb[i - 1] = Int32.Parse(str[i]);
            }
            this.value = float.Parse(str[0]);
            this.color = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
        }
        public Diagram(float value, int r, int g, int b)
        {
            this.value = value;
            this.color = Color.FromArgb(r, g, b);
        }
    }
    public partial class Form1 : Form
    {
        List<Diagram> diag = new List<Diagram>();
        float k = 1;
        float sum = 0;
        bool type = true;
        float mx, mn;
        int tick = 0;
        public Form1()
        {
            InitializeComponent();
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
        }
        private void FromBoxToList()
        {
            List<Diagram> voiddiag = new List<Diagram>();
            diag = voiddiag;
            foreach (string Str in richTextBox1.Lines)
            {
                if (Str == "") continue;
                Diagram tmp = new Diagram(Str);
                diag.Add(tmp);
            }
        }
        private void FromListToBox()
        {
            richTextBox1.Clear();
            foreach (Diagram elem in diag)
            {
                richTextBox1.AppendText(elem.value.ToString() + " " + elem.color.R.ToString() + " " + elem.color.G.ToString() + " " + elem.color.B.ToString()+ "\n");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            clear();
            string path = openFileDialog1.FileName;
            using (StreamReader sr = File.OpenText(path))
            {
                richTextBox1.AppendText(sr.ReadToEnd());
            }
            FromBoxToList();
            paint();
        }
        private void paint()
        {
            analyze();
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.White);
            if (diag.Count == 0) return;
            if (type)
            {
                rectangle();
            }
            else
            {
                circle();
            }
        }
        private void rectangle()
        {
            Graphics g = panel1.CreateGraphics();
            g.TranslateTransform(30, 150);
            float width=700/ diag.Count;
            float probel = width * 0.1f;
            width = width-2*probel;
            float last = probel;
            foreach (Diagram elem in diag)
            {
                SolidBrush Brush = new SolidBrush(elem.color);
                if (elem.value > 0)
                {
                    g.FillRectangle(Brush, last, -elem.value * k, width, elem.value * k);
                    g.DrawRectangle(new Pen(Color.Black, 1), last, -elem.value * k, width, elem.value * k);
                }
                else
                {
                    g.FillRectangle(Brush, last, 0, width, -elem.value * k);
                    g.DrawRectangle(new Pen(Color.Black, 1), last, 0, width, -elem.value * k);
                }
                Brush.Color = Color.Black;
                Font font = new Font("Century Gothic", 10);
                int extra = elem.value > 0 ? 15 : 0;
                g.DrawString(elem.value.ToString(), font, Brush, last, -elem.value * k-extra); 
                last += 2 * probel + width;
                font.Dispose(); Brush.Dispose();
            }
            g.DrawLine(new Pen(Color.Black, 2.0f), 0, 120, 0, -120);
            g.DrawLine(new Pen(Color.Black, 2.0f), 0, 0, 700, 0);
        }
        private void circle()
        {
            Graphics g = panel1.CreateGraphics();
            float oneagnle = (float)360/sum;
            float startangle=0f;
            float height = 15f ;
            float probel = (float)panel1.Height / (diag.Count+1) - height;
            float last = probel+height/2;
            foreach (Diagram elem in diag)
            {
                g.FillPie(new SolidBrush(elem.color), 100, 25, 250, 250, startangle, (float)Math.Abs(elem.value * oneagnle));
                g.DrawPie(new Pen(Color.Black, 1.0f), 100, 25, 250, 250, startangle, (float)Math.Abs(elem.value * oneagnle));
                startangle += (float)Math.Abs(elem.value * oneagnle);
                SolidBrush Brush = new SolidBrush(elem.color);
                g.FillRectangle(Brush, 500, last, height,height);
                g.DrawRectangle(new Pen(Color.Black, 1), 500, last, height, height);
                Font font = new Font("Century Gothic", 10);
                Brush.Color = Color.Black;
                g.DrawString(elem.value.ToString(), font, Brush, 520, last);
                font.Dispose(); Brush.Dispose();
                last += probel + height;
            }
        }
        private void clear()
        {
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.White);
            richTextBox1.Clear();
            List<Diagram> voiddiag = new List<Diagram>();
            diag = voiddiag;
            k = 1; sum = 0;
        }
        private void analyze()
        {
            sum = 0;
            if (diag.Count > 0)
            {
                mn = diag[0].value;
                sum += Math.Abs(mn);
                mx = mn;
            }
            for (int i = 1; i < diag.Count; i++)
            {
                float elem = diag[i].value;
                sum += Math.Abs(elem);
                if (elem > mx) mx = elem;
                if (elem < mn) mn = elem;
            }
            double maxheight = Math.Max(Math.Abs(mx),Math.Abs(mn));
            k = (float)(120 / maxheight) ;
        }
        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            clear();
            generate();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            paint();
            FromListToBox();
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string path = saveFileDialog1.FileName;
            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i=0;i<richTextBox1.Lines.Length;i++)
                {
                    sw.WriteLine(richTextBox1.Lines[i]);
                }
            }
            MessageBox.Show("Data saved");
        }
        private void generate()
        {
            Random n = new Random();
            int maxn = n.Next(1, 20);
            clear();
            for (int i = 0; i < maxn; i++)
            {
                float val = (float)n.Next(-1000, 1000);
                if (!type) val = (float)n.Next(0, 1000);
                Diagram tmp = new Diagram(val, n.Next(1, 255), n.Next(1, 255), n.Next(1, 255));
                diag.Add(tmp);
            }
            FromListToBox();
            paint();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            type = true;
            paint();
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            type = false;
            paint();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {
            FromBoxToList();
            paint();
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (tick % trackBar1.Value == 0)
            {
                generate();
            }
            tick++;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button4.Enabled = false;
            trackBar1.Value = 0;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            tick = 0;
            timer1.Stop();
            if (trackBar1.Value != 0)
            {
                button4.Enabled = true;
                timer1.Start();
            }
            else
                button4.Enabled = false;
        }
    }
}
