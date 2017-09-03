using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using LiveCharts.Wpf;
using Brushes = System.Windows.Media.Brushes;

namespace Oracle_Tablespace_Monitor
{
    public partial class Application : Form
    {
        public Application()
        {
            hwm = Decimal.Parse(ConfigurationManager.AppSettings["highWaterMark"])/100;
            InitializeComponent();
            FetchTablespaces();
            DisplayGauges();
            InitCheckedListBox();


            Thread.Sleep(1000);
        }
    }
}
