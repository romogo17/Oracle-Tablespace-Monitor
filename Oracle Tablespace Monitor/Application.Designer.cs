namespace Oracle_Tablespace_Monitor
{
    using System;
    using System.Windows.Forms;
    using LiveCharts;
    using LiveCharts.Configurations;
    using LiveCharts.Wpf;
    //using System.Data.OracleClient;
    //using Oracle.DataAccess.Client;
    //using Oracle.DataAccess.Types;
    using Oracle.ManagedDataAccess.Client;
    using Oracle.ManagedDataAccess.Types;
    using System.Data;
    using System.Configuration;
    using System.Windows.Media;
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Linq;

    partial class Application
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Application));
            this.opcionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cambiarHWMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.myLayout = new System.Windows.Forms.FlowLayoutPanel();
            tbspChecklist = new System.Windows.Forms.CheckedListBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // opcionesToolStripMenuItem
            // 
            this.opcionesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cambiarHWMToolStripMenuItem});
            this.opcionesToolStripMenuItem.Name = "opcionesToolStripMenuItem";
            this.opcionesToolStripMenuItem.Size = new System.Drawing.Size(83, 24);
            this.opcionesToolStripMenuItem.Text = "Opciones";
            // 
            // cambiarHWMToolStripMenuItem
            // 
            this.cambiarHWMToolStripMenuItem.Name = "cambiarHWMToolStripMenuItem";
            this.cambiarHWMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.cambiarHWMToolStripMenuItem.Size = new System.Drawing.Size(235, 26);
            this.cambiarHWMToolStripMenuItem.Text = "Cambiar HWM";
            this.cambiarHWMToolStripMenuItem.Click += new System.EventHandler(this.cambiarHWMToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opcionesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1090, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // myLayout
            // 
            this.myLayout.AutoScroll = true;
            this.myLayout.Location = new System.Drawing.Point(12, 45);
            this.myLayout.Name = "myLayout";
            this.myLayout.Size = new System.Drawing.Size(918, 498);
            this.myLayout.TabIndex = 17;
            // 
            // tbspChecklist
            // 
            tbspChecklist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            tbspChecklist.BorderStyle = System.Windows.Forms.BorderStyle.None;
            tbspChecklist.ForeColor = System.Drawing.SystemColors.Control;
            tbspChecklist.FormattingEnabled = true;
            tbspChecklist.Location = new System.Drawing.Point(936, 49);
            tbspChecklist.Name = "tbspChecklist";
            tbspChecklist.RightToLeft = System.Windows.Forms.RightToLeft.No;
            tbspChecklist.Size = new System.Drawing.Size(142, 493);
            tbspChecklist.TabIndex = 18;
            // 
            // Application
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.ClientSize = new System.Drawing.Size(1090, 556);
            this.Controls.Add(tbspChecklist);
            this.Controls.Add(this.myLayout);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Application";
            this.Opacity = 0.8D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitor de Tablespaces";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void DisplayGauges()
        {
            if (this.tbs == null) return;

            myLayout.Controls.Clear();

            foreach(Tablespace tb in tbs)
            {
                if (tb.Visible)
                {
                    this.myLayout.Controls.Add(new Tb_Panel(tb, hwm));
                }
            }

        }

        private bool FetchTablespaces()
        {
            this.tbs = new List<Tablespace>();

            DataSet ds = db_getTablespaces();

            bool flag = false;
            if (ds.Tables.Count == 0) flag = true;
            else if (!(ds.Tables[0].Rows.Count > 0)) flag = true;
            if (!flag)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tbs.Add(new Tablespace
                    {
                        Visible = true,
                        Name = dt.Rows[i].Field<string>("TABLESPACE_NAME"),
                        Max = Convert.ToDouble(Math.Round(dt.Rows[i].Field<decimal>("BYTES_SIZE")/1024/1024,4)),
                        Free = Convert.ToDouble(Math.Round(dt.Rows[i].Field<decimal>("BYTES_FREE") / 1024 / 1024, 4)),
                        Used = Convert.ToDouble(Math.Round(dt.Rows[i].Field<decimal>("BYTES_USED") / 1024 / 1024, 4)),
                        RateOfGrowthInBytes = 50,
                        DaysToHwm = 5,
                        DaysToMax = 7
                    });
                }
            }
            return !flag;


            /*  
             *  
             *  Lineas de prueba, debe traer esto de la base de datos, del metodo
             *  private Dataset db_getTablespaces()
             *  
             *  Procesar el Dataset para ir formando la lista
             *  
             */

            /*tbs.Add(new Tablespace
            {
                Visible = true,
                Name = "USERS",
                Max = 100,
                Free = 30,
                Used = 70,
                RateOfGrowthInBytes = 50,
                DaysToHwm = 5,
                DaysToMax = 7
            });

            tbs.Add(new Tablespace
            {
                Visible = true,
                Name = "SYSTEM",
                Max = 200,
                Free = 100,
                Used = 100,
                RateOfGrowthInBytes = 50,
                DaysToHwm = 5,
                DaysToMax = 7
            });*/

            return true;
        }

        private DataSet db_getTablespaces()
        {
            using (OracleConnection objConn = new OracleConnection(ConfigurationManager.AppSettings["connectionString"]))
            {
                DataSet data = new DataSet("alerta");

                // Create and execute the command
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objConn;
                objCmd.CommandText = "get_tablespace_info";
                objCmd.CommandType = CommandType.StoredProcedure;

                // Set parameters
                OracleParameter retParam = objCmd.Parameters.Add("return_value", OracleDbType.RefCursor, ParameterDirection.ReturnValue);
                //objCmd.Parameters.Add("return_value", OracleDbType.Int32, sgaSize, System.Data.ParameterDirection.Input);

                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    OracleDataAdapter a = new OracleDataAdapter(objCmd);
                    a.TableMappings.Add("MyTable", "sample_table"); // possible need for this
                    a.Fill(data);

                    //return sqlInfo;
                    //System.Console.WriteLine("Memory Usage is {0}", retParam.Value);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                }

                objConn.Close();
                objConn.Dispose();
                return data;
            }
        }

        private void cambiarHWMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeHWM ch = new ChangeHWM(this, hwm);
            ch.ShowDialog(this);
        }

        public void ChangeHWM(double pct)
        {
            this.hwm = Convert.ToDecimal(pct);

            foreach(Tb_Panel pl in myLayout.Controls)
            {
                LiveCharts.WinForms.AngularGauge ag = (LiveCharts.WinForms.AngularGauge) pl.Controls[0];
                ag.Sections[0].FromValue = ag.ToValue * Convert.ToDouble(hwm);
            }
        }

        private void InitCheckedListBox()
        {
            tbspChecklist.CheckOnClick = true;
            ((ListBox)this.tbspChecklist).DataSource = tbs;
            ((ListBox)this.tbspChecklist).DisplayMember = "Name";
            ((ListBox)this.tbspChecklist).ValueMember = "Name";

            for (int i = 0; i < tbspChecklist.Items.Count; i++)
            {
                Tablespace obj = (Tablespace)tbspChecklist.Items[i];
                tbspChecklist.SetItemChecked(i, obj.Visible);
            }
            tbspChecklist.SelectedIndexChanged += new System.EventHandler(this.tbspChecklist_SelectedIndexChanged);
        }

        private void tbspChecklist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tbspChecklist.Enabled)
            {
                tbspChecklist.Enabled = false;

                /* Actualizar el valor Visible del tablespace segun se checkee o no */

                Tablespace selected = tbs.Find(x => x.Name == (tbspChecklist.SelectedItem as Tablespace).Name);
                CheckState aux = tbspChecklist.GetItemCheckState(tbspChecklist.SelectedIndex);
                selected.Visible = (aux == CheckState.Checked) ? true : false;

                /* Si el nuevo estado es false, es que antes si estaba visible. Basta con removerlo de la vista */
                if (!selected.Visible)
                {
                    foreach (Tb_Panel pl in myLayout.Controls)
                    {
                        if (pl.getTablespaceName() == selected.Name)
                        {
                            myLayout.Controls.Remove(pl);
                        }
                    }
                }

                /* Si el nuevo estado es true, es que antes no estaba visible. Basta con agregarlo a la vista */
                else
                {
                    this.myLayout.Controls.Add(new Tb_Panel(selected, hwm));
                }

                /* Bloquear el checklist para que no se pueda ejecutar una operacion hasta que termine todo el proceso */
                tbspChecklist.Enabled = true;
            }
        }


        private ToolStripMenuItem opcionesToolStripMenuItem;
        private ToolStripMenuItem cambiarHWMToolStripMenuItem;
        private MenuStrip menuStrip1;
        private FlowLayoutPanel myLayout;
        System.Windows.Forms.CheckedListBox tbspChecklist;

        List<Tablespace> tbs;
        private decimal hwm;
        //private Timer Timer { get; set; }
    }

}

/* Lineas usadas para un timer
 * 

    //The next code simulates data changes every 500 ms
    Timer = new Timer
    {
        Interval = 1000
    };
    Timer.Tick += TimerOnTick;
    Timer.Start();

private void SetAxisLimits(System.DateTime now)
{
    myChart.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(0).Ticks; // lets force the axis to be 100ms ahead
    myChart.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(8).Ticks; //we only care about the last 8 seconds
}

private void TimerOnTick(object sender, EventArgs eventArgs)
{
    var now = System.DateTime.Now;
    double usedMemory = Convert.ToDouble(GetUsedMemory());

    ChartValues.Add(new MeasureModel
    {
        DateTime = now,
        Value = usedMemory
    });

    SetAxisLimits(now);

    //  definir si hay un pico de memoria
    if(usedMemory > Double.Parse(label2.Text))
    {
        label2.Text = Decimal.Round((decimal)usedMemory, 10).ToString();
        //label3.Text = now.ToString("hh:mm");
        label3.Text = now.ToShortTimeString();

        DataSet ds = GetUsersAtAlert();

        bool flag = false;
        if (ds.Tables.Count == 0) flag = true;
        else if (!(ds.Tables[0].Rows.Count > 0)) flag = true;
        if (!flag)
        {
            DataRow row = ds.Tables[0].Rows[0];
            label6.Text = (string)row["USERNAME"];
            label8.Text = (string)row["SQL_ID"];
            label10.Text = Convert.ToString(row["HASH_VALUE"]);
            label13.Text = Convert.ToString(Convert.ToDouble(row["MEMORY"]) / 1024);
        }



    }

    // verificar si hay una nueva alerta
    if(usedMemory > Convert.ToDouble(sgaSize * hwm))
    {
        if (!enAlerta)
        {
            enAlerta = true;

            string exeRuntimeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string subDirectory = System.IO.Path.Combine(exeRuntimeDirectory, "alertas");
            if (!System.IO.Directory.Exists(subDirectory))
            {
                System.IO.Directory.CreateDirectory(subDirectory);
            }
            GetUsersAtAlert().WriteXml(subDirectory + "\\" + now.ToFileTime() + ".xml");

        }
    } else { enAlerta = false; }

    //lets only use the last 30 values
    if (ChartValues.Count > 30) ChartValues.RemoveAt(0);
}
*/
