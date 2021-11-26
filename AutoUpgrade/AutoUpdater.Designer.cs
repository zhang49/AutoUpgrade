
namespace AutoUpdater
{
    partial class AutoUpdater
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbAutoUpgradeFold = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbCheckIntravel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbHttpServerHost = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "AutoUpgradeFold:";
            // 
            // tbAutoUpgradeFold
            // 
            this.tbAutoUpgradeFold.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tbAutoUpgradeFold.Location = new System.Drawing.Point(161, 62);
            this.tbAutoUpgradeFold.Multiline = true;
            this.tbAutoUpgradeFold.Name = "tbAutoUpgradeFold";
            this.tbAutoUpgradeFold.ReadOnly = true;
            this.tbAutoUpgradeFold.Size = new System.Drawing.Size(237, 49);
            this.tbAutoUpgradeFold.TabIndex = 1;
            this.tbAutoUpgradeFold.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbAutoUpgradeFold_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "CheckIntravel(Seconds):";
            // 
            // tbCheckIntravel
            // 
            this.tbCheckIntravel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tbCheckIntravel.Location = new System.Drawing.Point(161, 35);
            this.tbCheckIntravel.Name = "tbCheckIntravel";
            this.tbCheckIntravel.ReadOnly = true;
            this.tbCheckIntravel.Size = new System.Drawing.Size(238, 21);
            this.tbCheckIntravel.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(84, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "HttpServer:";
            // 
            // tbHttpServerHost
            // 
            this.tbHttpServerHost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tbHttpServerHost.Location = new System.Drawing.Point(161, 8);
            this.tbHttpServerHost.Name = "tbHttpServerHost";
            this.tbHttpServerHost.ReadOnly = true;
            this.tbHttpServerHost.Size = new System.Drawing.Size(238, 21);
            this.tbHttpServerHost.TabIndex = 6;
            this.tbHttpServerHost.Text = "http://localhost:9000";
            // 
            // AutoUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 123);
            this.Controls.Add(this.tbHttpServerHost);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbCheckIntravel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbAutoUpgradeFold);
            this.Controls.Add(this.label1);
            this.Name = "AutoUpdater";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoUpdater_FormClosing);
            this.Load += new System.EventHandler(this.AutoUpdater_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAutoUpgradeFold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbCheckIntravel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbHttpServerHost;
    }
}

