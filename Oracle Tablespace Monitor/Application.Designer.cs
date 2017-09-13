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
    using System.Xml;

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
            this.opcionesToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.opcionesToolStripMenuItem.Text = "Options";
            // 
            // cambiarHWMToolStripMenuItem
            // 
            this.cambiarHWMToolStripMenuItem.Name = "cambiarHWMToolStripMenuItem";
            this.cambiarHWMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.cambiarHWMToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.cambiarHWMToolStripMenuItem.Text = "Change HWM";
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
            this.menuStrip1.Size = new System.Drawing.Size(818, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // myLayout
            // 
            this.myLayout.AutoScroll = true;
            this.myLayout.Location = new System.Drawing.Point(9, 37);
            this.myLayout.Margin = new System.Windows.Forms.Padding(2);
            this.myLayout.Name = "myLayout";
            this.myLayout.Size = new System.Drawing.Size(688, 405);
            this.myLayout.TabIndex = 17;
            // 
            // tbspChecklist
            // 
            tbspChecklist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            tbspChecklist.BorderStyle = System.Windows.Forms.BorderStyle.None;
            tbspChecklist.ForeColor = System.Drawing.SystemColors.Control;
            tbspChecklist.FormattingEnabled = true;
            tbspChecklist.Location = new System.Drawing.Point(702, 40);
            tbspChecklist.Margin = new System.Windows.Forms.Padding(2);
            tbspChecklist.Name = "tbspChecklist";
            tbspChecklist.RightToLeft = System.Windows.Forms.RightToLeft.No;
            tbspChecklist.Size = new System.Drawing.Size(106, 390);
            tbspChecklist.TabIndex = 18;
            // 
            // Application
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.ClientSize = new System.Drawing.Size(818, 461);
            this.Controls.Add(tbspChecklist);
            this.Controls.Add(this.myLayout);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
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
            string name_read;
            DataSet ds = db_getTablespaces();

            bool flag = false;
            if (ds.Tables.Count == 0) flag = true;
            else if (!(ds.Tables[0].Rows.Count > 0)) flag = true;
            if (!flag)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    name_read = dt.Rows[i].Field<string>("TABLESPACE_NAME");

                    if (name_read != "UNDOTBS1")
                    {
                        bool isSystemTB = name_read == "SYSTEM" || name_read == "SYSAUX";

                        tbs.Add(new Tablespace
                        {
                            Visible = isSystemTB ? false : true,
                            Name = name_read,
                            Max = Convert.ToDouble(Math.Round(dt.Rows[i].Field<decimal>("BYTES_SIZE") / 1024 / 1024, 4)),
                            Free = Convert.ToDouble(Math.Round(dt.Rows[i].Field<decimal>("BYTES_FREE") / 1024 / 1024, 4)),
                            Used = Convert.ToDouble(Math.Round(dt.Rows[i].Field<decimal>("BYTES_USED") / 1024 / 1024, 4)),
                            //RateOfGrowthInMB = 50,
                            //DaysToHwm = 5,
                            //DaysToMax = 7
                        });
                    }

                }
            }
            return !flag;
        }

        private bool UpdateUsage()
        {
            var directory = new DirectoryInfo(ConfigurationManager.AppSettings["myDir"]);


            var last = directory.GetFiles()
                         .OrderByDescending(f => f.LastWriteTime)
                         .First();

            var seccond = directory.GetFiles()
                         .OrderByDescending(f => f.LastWriteTime)
                         .Skip(1)
                         .First();

            XmlDocument doc_last = new XmlDocument();
            XmlDocument doc_seccond = new XmlDocument();
            doc_last.Load(directory+"\\"+last.Name);
            doc_seccond.Load(directory + "\\" + seccond.Name);

            foreach (XmlNode node in doc_last.DocumentElement.SelectSingleNode("/ROWSET"))
            {
                string tb_name = Convert.ToString(node.FirstChild.InnerText);
                decimal tb_usage = Convert.ToDecimal(node.ChildNodes[2].InnerText);
                decimal tb_usage_sub = 0;

                foreach (XmlNode node_seccond in doc_seccond.DocumentElement.SelectSingleNode("/ROWSET"))
                {
                    if (Convert.ToString(node_seccond.FirstChild.InnerText) == tb_name)
                    {
                        tb_usage_sub = tb_usage - Convert.ToDecimal(node_seccond.ChildNodes[2].InnerText);
                        //System.Console.WriteLine(tb_name + " -> " + tb_usage + "->" + tb_usage_sub);
                    }                    
                }

                if (tb_name != "UNDOTBS1" && tb_name != "TEMP")
                {
                    tbs.Find(x => x.Name == tb_name).setRateOfGrothInBytes(tb_usage_sub, hwm);
                }
            }

            return true;
        }

        private DataSet db_getTablespaces()
        {
            /*
                --http://dbaforums.org/oracle/lofiversion/index.php?t13112.html
                CREATE OR REPLACE FUNCTION get_tablespace_info
                  RETURN SYS_REFCURSOR IS
                  cr SYS_REFCURSOR;
                  BEGIN
                    OPEN cr FOR
                    SELECT
                      ts.tablespace_name,
                      TRUNC("SIZE(B)", 2)                                  "BYTES_SIZE",
                      TRUNC(fr."FREE(B)", 2)                               "BYTES_FREE",
                      TRUNC("SIZE(B)" - "FREE(B)", 2)                      "BYTES_USED",
                      TRUNC((1 - (fr."FREE(B)" / df."SIZE(B)")) * 100, 10) "PCT_USED"
                    FROM
                      (SELECT
                         tablespace_name,
                         SUM(bytes) "FREE(B)"
                       FROM dba_free_space
                       GROUP BY tablespace_name) fr,
                      (SELECT
                         tablespace_name,
                         SUM(bytes)    "SIZE(B)",
                         SUM(maxbytes) "MAX_EXT"
                       FROM dba_data_files
                       GROUP BY tablespace_name) df,
                      (SELECT tablespace_name
                       FROM dba_tablespaces) ts
                    WHERE fr.tablespace_name = df.tablespace_name
                          AND fr.tablespace_name = ts.tablespace_name;
                    RETURN CR;
                  END;
                /
             */

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

        public void ChangeHWM(double pct, string target)
        {
            if(target == "GLOBAL")
                ChangeHwmGlobally(pct);
            else
            {
                foreach (Tablespace tb in tbs)
                {
                    if (tb.Name == target)
                    {
                        tb.updatesRatesWithNewHwm(hwm);
                    }
                }

                foreach (Tb_Panel pl in myLayout.Controls)
                {
                    if(pl.tb.Name == target)
                    {
                        LiveCharts.WinForms.AngularGauge ag = (LiveCharts.WinForms.AngularGauge)pl.Controls[0];
                        ag.Sections[0].FromValue = ag.ToValue * Convert.ToDouble(Convert.ToDecimal(pct));
                        pl.Controls[2].Text = pl.tb.DaysToHwm + " days to HWM";
                        pl.Controls[4].Text = "HWM: " + Math.Round(pl.tb.Max * Convert.ToDouble(Convert.ToDecimal(pct)), 2) + " MB";
                    }
                }
            }
        }

        private void ChangeHwmGlobally(double pct)
        {
            this.hwm = Convert.ToDecimal(pct);

            foreach (Tablespace tb in tbs)
            { tb.updatesRatesWithNewHwm(hwm); }

            foreach (Tb_Panel pl in myLayout.Controls)
            {
                LiveCharts.WinForms.AngularGauge ag = (LiveCharts.WinForms.AngularGauge)pl.Controls[0];
                ag.Sections[0].FromValue = ag.ToValue * Convert.ToDouble(hwm);
                pl.Controls[2].Text = pl.tb.DaysToHwm + " days to HWM";
                pl.Controls[4].Text = "HWM: " + Math.Round(pl.tb.Max * Convert.ToDouble(hwm), 2) + " MB";
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

        public List<Tablespace> tbs;
        private decimal hwm;
    }

}
