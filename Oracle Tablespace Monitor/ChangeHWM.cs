using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oracle_Tablespace_Monitor
{
    public partial class ChangeHWM : Form
    {
        public ChangeHWM(Application application, decimal c)
        {
            var current = c;
            while(current > 100)
            {
                current = current / 10;
            }

            InitializeComponent();
            app = application;
            curr = current*100;
            this.numericUpDown1.Value = current*100;

            selector.Items.Add("GLOBAL");
            selector.SelectedIndex = 0;
            foreach (Tablespace tb in application.tbs)
            {
                if(tb.Visible) selector.Items.Add(tb.Name);
            }
        }
    }
}
