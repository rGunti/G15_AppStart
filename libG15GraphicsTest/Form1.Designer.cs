namespace libG15GraphicsTest
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdNewDatabase = new System.Windows.Forms.Button();
            this.cmdGetAllApps = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.cmdAddApp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdNewDatabase
            // 
            this.cmdNewDatabase.Location = new System.Drawing.Point(12, 12);
            this.cmdNewDatabase.Name = "cmdNewDatabase";
            this.cmdNewDatabase.Size = new System.Drawing.Size(121, 45);
            this.cmdNewDatabase.TabIndex = 0;
            this.cmdNewDatabase.Text = "Create New Database";
            this.cmdNewDatabase.UseVisualStyleBackColor = true;
            this.cmdNewDatabase.Click += new System.EventHandler(this.cmdNewDatabase_Click);
            // 
            // cmdGetAllApps
            // 
            this.cmdGetAllApps.Location = new System.Drawing.Point(12, 63);
            this.cmdGetAllApps.Name = "cmdGetAllApps";
            this.cmdGetAllApps.Size = new System.Drawing.Size(121, 23);
            this.cmdGetAllApps.TabIndex = 1;
            this.cmdGetAllApps.Text = "Get All Apps";
            this.cmdGetAllApps.UseVisualStyleBackColor = true;
            this.cmdGetAllApps.Click += new System.EventHandler(this.cmdGetAllApps_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(139, 12);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(38, 13);
            this.lblResult.TabIndex = 2;
            this.lblResult.Text = "Ready";
            // 
            // cmdAddApp
            // 
            this.cmdAddApp.Location = new System.Drawing.Point(12, 92);
            this.cmdAddApp.Name = "cmdAddApp";
            this.cmdAddApp.Size = new System.Drawing.Size(121, 23);
            this.cmdAddApp.TabIndex = 3;
            this.cmdAddApp.Text = "Add App";
            this.cmdAddApp.UseVisualStyleBackColor = true;
            this.cmdAddApp.Click += new System.EventHandler(this.cmdAddApp_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 241);
            this.Controls.Add(this.cmdAddApp);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.cmdGetAllApps);
            this.Controls.Add(this.cmdNewDatabase);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdNewDatabase;
        private System.Windows.Forms.Button cmdGetAllApps;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button cmdAddApp;
    }
}

