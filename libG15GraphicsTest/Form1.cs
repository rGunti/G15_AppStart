using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using libAppStart;

namespace libG15GraphicsTest
{
    public partial class Form1 : Form
    {
        Database db;

        public Form1()
        {
            InitializeComponent();
            db = new Database();
        }

        private void cmdNewDatabase_Click(object sender, EventArgs e)
        {
            db = new Database();
            Exception ex = db.InitializeDatabase(true);
            lblResult.Text = ex.GetType().ToString() + "\n\n" + ex.Message;
        }

        private void cmdGetAllApps_Click(object sender, EventArgs e)
        {
            if (db == null)
            {
                lblResult.Text = "No DB init";
                return;
            }

            Exception result = db.GetAllApplications();

            if (typeof(libAppStart.AppListRecieved) != result.GetType())
            {
                lblResult.Text = result.GetType().ToString() + "\n\n" + result.Message;
                return;
            }

            List<App> appList = ((AppListRecieved)result).RecievedData;

            string s = String.Format("Done! ({0} object(s))\n", appList.Count);
            foreach (libAppStart.App app in appList)
            {
                s += String.Format("{0} ; ({1})\n", app.Name, app.Path);
            }

            lblResult.Text = s;
        }

        private void cmdAddApp_Click(object sender, EventArgs e)
        {
            if (db == null)
                db = new Database();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "EXE File|*.exe";

            if (ofd.ShowDialog() != DialogResult.OK) return;

            App a = new App(ofd.FileName);

            Exception result = db.AddNewApplication(a);

            lblResult.Text = result.GetType().ToString() + "\n\n" + result.Message;
        }
    }
}
