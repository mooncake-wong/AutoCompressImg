namespace AutoCompressImg
{
    partial class FormCompressPicture
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
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.lvSourceFolderList = new System.Windows.Forms.ListView();
            this.btnSelectSourceFolder = new System.Windows.Forms.Button();
            this.tbSourceFolderPath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbInfomation = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblFinal = new System.Windows.Forms.Label();
            this.lblChoose = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvSourceFolderList
            // 
            this.lvSourceFolderList.FullRowSelect = true;
            this.lvSourceFolderList.Location = new System.Drawing.Point(6, 42);
            this.lvSourceFolderList.MultiSelect = false;
            this.lvSourceFolderList.Name = "lvSourceFolderList";
            this.lvSourceFolderList.Size = new System.Drawing.Size(748, 398);
            this.lvSourceFolderList.TabIndex = 3;
            this.lvSourceFolderList.UseCompatibleStateImageBehavior = false;
            this.lvSourceFolderList.SelectedIndexChanged += new System.EventHandler(this.lvSourceFolderList_SelectedIndexChanged);
            // 
            // btnSelectSourceFolder
            // 
            this.btnSelectSourceFolder.Location = new System.Drawing.Point(679, 18);
            this.btnSelectSourceFolder.Name = "btnSelectSourceFolder";
            this.btnSelectSourceFolder.Size = new System.Drawing.Size(75, 21);
            this.btnSelectSourceFolder.TabIndex = 2;
            this.btnSelectSourceFolder.Text = "浏览";
            this.btnSelectSourceFolder.UseVisualStyleBackColor = true;
            this.btnSelectSourceFolder.Click += new System.EventHandler(this.btnSelectSourceFolder_Click);
            // 
            // tbSourceFolderPath
            // 
            this.tbSourceFolderPath.Location = new System.Drawing.Point(79, 18);
            this.tbSourceFolderPath.Name = "tbSourceFolderPath";
            this.tbSourceFolderPath.Size = new System.Drawing.Size(594, 21);
            this.tbSourceFolderPath.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvSourceFolderList);
            this.groupBox1.Controls.Add(this.btnSelectSourceFolder);
            this.groupBox1.Controls.Add(this.tbSourceFolderPath);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(0, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(760, 446);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "文件来源";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择文件夹";
            // 
            // lbInfomation
            // 
            this.lbInfomation.AutoSize = true;
            this.lbInfomation.Location = new System.Drawing.Point(6, 453);
            this.lbInfomation.Name = "lbInfomation";
            this.lbInfomation.Size = new System.Drawing.Size(53, 12);
            this.lbInfomation.TabIndex = 21;
            this.lbInfomation.Text = "已经停止";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(685, 466);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 21);
            this.btnClose.TabIndex = 20;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStart
            // 
            this.btnStart.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnStart.Location = new System.Drawing.Point(588, 466);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 21);
            this.btnStart.TabIndex = 19;
            this.btnStart.Text = "开始压缩";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(5, 495);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(107, 12);
            this.lblResult.TabIndex = 22;
            this.lblResult.Text = "已成功压缩0个文件";
            // 
            // lblFinal
            // 
            this.lblFinal.AutoSize = true;
            this.lblFinal.Location = new System.Drawing.Point(7, 474);
            this.lblFinal.Name = "lblFinal";
            this.lblFinal.Size = new System.Drawing.Size(41, 12);
            this.lblFinal.TabIndex = 23;
            this.lblFinal.Text = "label2";
            // 
            // lblChoose
            // 
            this.lblChoose.AutoSize = true;
            this.lblChoose.Location = new System.Drawing.Point(6, 519);
            this.lblChoose.Name = "lblChoose";
            this.lblChoose.Size = new System.Drawing.Size(107, 12);
            this.lblChoose.TabIndex = 24;
            this.lblChoose.Text = "获取选中行的内容:";
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(6, 536);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(748, 48);
            this.txtContent.TabIndex = 25;
            // 
            // FormCompressPicture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 587);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.lblChoose);
            this.Controls.Add(this.lblFinal);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbInfomation);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormCompressPicture";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "批量自动压缩图片";
            this.Load += new System.EventHandler(this.FormCompressPicture_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ListView lvSourceFolderList;
        private System.Windows.Forms.Button btnSelectSourceFolder;
        private System.Windows.Forms.TextBox tbSourceFolderPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbInfomation;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblFinal;
        private System.Windows.Forms.Label lblChoose;
        private System.Windows.Forms.TextBox txtContent;
    }
}