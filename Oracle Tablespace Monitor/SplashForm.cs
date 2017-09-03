using Oracle_Tablespace_Monitor.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oracle_Tablespace_Monitor
{
    public partial class SplashForm : Form
    {
        private PictureBox spashPictureBox;

        public SplashForm()
        {
            InitializeComponent();

            spashPictureBox = new PictureBox
            {
                Image = Resources.OraTbspMonitorLoading,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(spashPictureBox);
        }
    }
}
