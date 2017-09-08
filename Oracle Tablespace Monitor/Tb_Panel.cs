using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using LiveCharts.Wpf;
using Brushes = System.Windows.Media.Brushes;
using LiveCharts;
using LiveCharts.Configurations;
using System.IO;
using System.Reflection;


namespace Oracle_Tablespace_Monitor
{
    class Tb_Panel : Panel
    {
        LiveCharts.WinForms.AngularGauge gauge;
        private Label tb_name;
        private Label tb_d_max;
        private Label tb_d_hwm;
        private Label tb_pct;
        public Tablespace tb;


        public Tb_Panel(Tablespace tb, decimal hwm)
        {
            this.tb = tb;
            this.gauge = new LiveCharts.WinForms.AngularGauge();
            this.tb_name = new Label();
            this.tb_pct = new Label();
            this.tb_d_hwm = new Label();
            this.tb_d_max = new Label();
            this.SuspendLayout();


            /* Configuramos cada elemento del panel */
            // 
            // panel
            // 
            this.Controls.Add(gauge);
            this.Controls.Add(tb_d_max);
            this.Controls.Add(tb_d_hwm);
            this.Controls.Add(tb_pct);
            this.Controls.Add(tb_name);
            this.Location = new System.Drawing.Point(3, 3);
            this.Name = "panel_" + tb.Name;
            this.Size = new System.Drawing.Size(300, 160);
            this.TabIndex = 2;
            // 
            // tb_name
            // 
            tb_name.AutoSize = true;
            tb_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_name.ForeColor = System.Drawing.SystemColors.ControlDark;
            tb_name.Location = new System.Drawing.Point(170, 5);
            tb_name.Name = "tb_name_" + tb.Name;
            tb_name.Size = new System.Drawing.Size(55, 20);
            tb_name.TabIndex = 2;
            tb_name.Text = tb.Name;
            // 
            // gauge
            // 
            gauge.Location = new System.Drawing.Point(3, 3);
            gauge.Name = "gauge_" + tb.Name;
            gauge.Size = new System.Drawing.Size(160, 160);
            gauge.TabIndex = 1;
            gauge.Text = "angularGauge_" + tb.Name;

            gauge.Value = tb.Used;
            gauge.FromValue = 0;
            gauge.ToValue = tb.Max;
            gauge.LabelsStep = Math.Round(tb.Max / 5, MidpointRounding.ToEven);
            gauge.TickStep = Math.Round(gauge.LabelsStep / 10, MidpointRounding.ToEven);
            gauge.TicksForeground = Brushes.White;
            gauge.Wedge = 190;
            gauge.NeedleFill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(253, 142, 0));
            gauge.Base.Foreground = Brushes.White;
            gauge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            gauge.Base.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(27, 27, 27));
            gauge.Base.FontWeight = FontWeights.Regular;
            gauge.Base.FontSize = 10;
            gauge.SectionsInnerRadius = 0.5;
            gauge.Sections.Add(new AngularSection
            {
                FromValue = tb.Max * Convert.ToDouble(hwm),
                ToValue = gauge.ToValue,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(142, 63, 63))
            });
            // 
            // tb_pct
            // 
            this.tb_pct.AutoSize = true;
            this.tb_pct.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_pct.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.tb_pct.Location = new System.Drawing.Point(170, 30);
            this.tb_pct.Name = "tb_pct_" + tb.Name;
            this.tb_pct.Size = new System.Drawing.Size(57, 18);
            this.tb_pct.TabIndex = 3;
            this.tb_pct.Text = Math.Round((tb.Used / tb.Max) * 100, 4) + "%";
            // 
            // tb_d_hwm
            // 
            this.tb_d_hwm.AutoSize = true;
            this.tb_d_hwm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_d_hwm.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.tb_d_hwm.Location = new System.Drawing.Point(170, 55);
            this.tb_d_hwm.Name = "tb_d_hwm_" + tb.Name;
            this.tb_d_hwm.Size = new System.Drawing.Size(84, 18);
            this.tb_d_hwm.TabIndex = 4;
            this.tb_d_hwm.Text = tb.DaysToHwm + "d to HWM";
            // 
            // tb_d_max
            // 
            this.tb_d_max.AutoSize = true;
            this.tb_d_max.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_d_max.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.tb_d_max.Location = new System.Drawing.Point(170, 75);
            this.tb_d_max.Name = "tb_d_max";
            this.tb_d_max.Size = new System.Drawing.Size(77, 18);
            this.tb_d_max.TabIndex = 5;
            this.tb_d_max.Text = tb.DaysToMax + "d to MAX";



            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public string getTablespaceName()
        {
            return this.tb.Name;
        }

    }
}
