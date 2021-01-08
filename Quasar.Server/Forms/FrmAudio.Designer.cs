using Quasar.Server.Controls;

namespace Quasar.Server.Forms
{
    partial class FrmAudio
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAudio));
            this.cbDevices = new System.Windows.Forms.ComboBox();
            this.toolTipButtons = new System.Windows.Forms.ToolTip(this.components);
            this.pbStart = new System.Windows.Forms.PictureBox();
            this.pbStop = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblQuality = new System.Windows.Forms.Label();
            this.lblQualityShow = new System.Windows.Forms.Label();
            this.barQuality = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.pbStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // cbDevices
            // 
            this.cbDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDevices.FormattingEnabled = true;
            this.cbDevices.Location = new System.Drawing.Point(9, 110);
            this.cbDevices.Name = "cbDevices";
            this.cbDevices.Size = new System.Drawing.Size(263, 21);
            this.cbDevices.TabIndex = 8;
            this.cbDevices.TabStop = false;
            // 
            // pbStart
            // 
            this.pbStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbStart.Image = ((System.Drawing.Image)(resources.GetObject("pbStart.Image")));
            this.pbStart.Location = new System.Drawing.Point(12, 12);
            this.pbStart.Name = "pbStart";
            this.pbStart.Size = new System.Drawing.Size(65, 66);
            this.pbStart.TabIndex = 10;
            this.pbStart.TabStop = false;
            this.pbStart.Click += new System.EventHandler(this.pbStart_Click);
            // 
            // pbStop
            // 
            this.pbStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbStop.Enabled = false;
            this.pbStop.Image = global::Quasar.Server.Properties.Resources.audio_stop_disabled;
            this.pbStop.Location = new System.Drawing.Point(83, 12);
            this.pbStop.Name = "pbStop";
            this.pbStop.Size = new System.Drawing.Size(54, 54);
            this.pbStop.TabIndex = 11;
            this.pbStop.TabStop = false;
            this.pbStop.Click += new System.EventHandler(this.pbStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Ausgabegerät:";
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(141, 12);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(43, 13);
            this.lblQuality.TabIndex = 4;
            this.lblQuality.Text = "Bitrate:";
            // 
            // lblQualityShow
            // 
            this.lblQualityShow.AutoSize = true;
            this.lblQualityShow.Location = new System.Drawing.Point(175, 60);
            this.lblQualityShow.Name = "lblQualityShow";
            this.lblQualityShow.Size = new System.Drawing.Size(68, 13);
            this.lblQualityShow.TabIndex = 5;
            this.lblQualityShow.Text = "44100 (best)";
            // 
            // barQuality
            // 
            this.barQuality.Location = new System.Drawing.Point(144, 28);
            this.barQuality.Maximum = 12;
            this.barQuality.Minimum = 1;
            this.barQuality.Name = "barQuality";
            this.barQuality.Size = new System.Drawing.Size(128, 45);
            this.barQuality.TabIndex = 3;
            this.barQuality.TabStop = false;
            this.barQuality.Value = 8;
            this.barQuality.Scroll += new System.EventHandler(this.barQuality_Scroll);
            // 
            // FrmAudio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(285, 145);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbStop);
            this.Controls.Add(this.pbStart);
            this.Controls.Add(this.lblQuality);
            this.Controls.Add(this.lblQualityShow);
            this.Controls.Add(this.barQuality);
            this.Controls.Add(this.cbDevices);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FrmAudio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Audio";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAudio_FormClosing);
            this.Load += new System.EventHandler(this.FrmRemoteAudio_Load);
            this.Resize += new System.EventHandler(this.FrmAudio_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pbStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barQuality)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbDevices;
        private System.Windows.Forms.ToolTip toolTipButtons;
        private System.Windows.Forms.PictureBox pbStart;
        private System.Windows.Forms.PictureBox pbStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.Label lblQualityShow;
        private System.Windows.Forms.TrackBar barQuality;
    }
}