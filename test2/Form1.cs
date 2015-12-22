using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test2
{
    public partial class Form1 : Form
    {
        public Bitmap foto2D, foto2D2;
        public Form1(Bitmap foto,Bitmap fotostart)
        {

            foto2D = foto;
            foto2D2 = fotostart;
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = foto2D;
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            pictureBox2.Image = foto2D2;
        }
    }
}
