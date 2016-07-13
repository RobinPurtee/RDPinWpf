using System;
using System.Drawing;

namespace RDPcontrol
{
    partial class RemoteDeskTopControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        public Size WindowSize { get; set; }


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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteDeskTopControl));
            this.axRdpClient = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
            ((System.ComponentModel.ISupportInitialize)(this.axRdpClient)).BeginInit();
            this.SuspendLayout();
            // 
            // axRdpClient
            // 
            this.axRdpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axRdpClient.Enabled = true;
            this.axRdpClient.Location = new System.Drawing.Point(0, 0);
            this.axRdpClient.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.axRdpClient.MaximumSize = new System.Drawing.Size(1280, 702);
            this.axRdpClient.MinimumSize = new System.Drawing.Size(683, 511);
            this.axRdpClient.Name = "axRdpClient";
            this.axRdpClient.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axRdpClient.OcxState")));
            this.axRdpClient.Size = new System.Drawing.Size(683, 511);
            this.axRdpClient.TabIndex = 0;
            // 
            // RemoteDeskTopControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.axRdpClient);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.Name = "RemoteDeskTopControl";
            this.Size = new System.Drawing.Size(20, 20);
            ((System.ComponentModel.ISupportInitialize)(this.axRdpClient)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private AxMSTSCLib.AxMsRdpClient9NotSafeForScripting axRdpClient;
    }
}
