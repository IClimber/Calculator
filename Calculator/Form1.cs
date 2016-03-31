using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Calculator
{
    public partial class Form1 : Form
    {
        private Calculator MC = new Calculator();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch S = new Stopwatch();
            S.Start();
            textBox1.Text = MC.Calculate(richTextBox1.Text).ToString();
            S.Stop();
            this.Text = "Ticks=" + (S.ElapsedTicks * 1000).ToString() + "  Milisec=" + (S.ElapsedMilliseconds).ToString();
            if ((MC.GraphPoints.Length > 0) && (MC.GraphPoints.Length < 50000))
            {
                richTextBox2.Clear();
                string Str = "";
                for (int i = 0; i < MC.GraphPoints.Length; i++)
                    Str += "X=" + MC.GraphPoints[i].X.ToString() + "  Y=" + MC.GraphPoints[i].Y.ToString() + "\n";
                richTextBox2.Text = Str;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Parser My = new Parser(richTextBox1.Text);
            while (My.NotEnd())
            {
                richTextBox2.AppendText(My.TokenStr() + "\n");
                My.NextToken();
            }
        }
    }
}
