namespace ms43x_util
{
    partial class MainForm
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
            narrowbandErrorBtn = new Button();
            loadXdfBtn = new Button();
            loadMapBtn = new Button();
            voVeConversionBtn = new Button();
            reloadLastCombo = new Button();
            SuspendLayout();
            // 
            // narrowbandErrorBtn
            // 
            narrowbandErrorBtn.Enabled = false;
            narrowbandErrorBtn.Location = new Point(12, 173);
            narrowbandErrorBtn.Name = "narrowbandErrorBtn";
            narrowbandErrorBtn.Size = new Size(361, 76);
            narrowbandErrorBtn.TabIndex = 0;
            narrowbandErrorBtn.Text = "Narrowband Error% VE";
            narrowbandErrorBtn.UseVisualStyleBackColor = true;
            narrowbandErrorBtn.Click += button1_Click;
            // 
            // loadXdfBtn
            // 
            loadXdfBtn.Location = new Point(12, 12);
            loadXdfBtn.Name = "loadXdfBtn";
            loadXdfBtn.Size = new Size(180, 80);
            loadXdfBtn.TabIndex = 1;
            loadXdfBtn.Text = "Select MS43x XDF";
            loadXdfBtn.UseVisualStyleBackColor = true;
            loadXdfBtn.Click += loadXdfBtn_Click;
            // 
            // loadMapBtn
            // 
            loadMapBtn.Enabled = false;
            loadMapBtn.Location = new Point(193, 12);
            loadMapBtn.Name = "loadMapBtn";
            loadMapBtn.Size = new Size(180, 80);
            loadMapBtn.TabIndex = 2;
            loadMapBtn.Text = "Select MS43x Bin";
            loadMapBtn.UseVisualStyleBackColor = true;
            loadMapBtn.Click += loadMapBtn_Click;
            // 
            // voVeConversionBtn
            // 
            voVeConversionBtn.Enabled = false;
            voVeConversionBtn.Location = new Point(12, 255);
            voVeConversionBtn.Name = "voVeConversionBtn";
            voVeConversionBtn.Size = new Size(361, 76);
            voVeConversionBtn.TabIndex = 3;
            voVeConversionBtn.Text = "VO -> VE Conversion";
            voVeConversionBtn.UseVisualStyleBackColor = true;
            voVeConversionBtn.Click += voVeConversionBtn_Click;
            // 
            // reloadLastCombo
            // 
            reloadLastCombo.Location = new Point(12, 98);
            reloadLastCombo.Name = "reloadLastCombo";
            reloadLastCombo.Size = new Size(361, 34);
            reloadLastCombo.TabIndex = 4;
            reloadLastCombo.Text = "Reload last xdf & map";
            reloadLastCombo.UseVisualStyleBackColor = true;
            reloadLastCombo.Click += reloadLastCombo_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(385, 450);
            Controls.Add(reloadLastCombo);
            Controls.Add(voVeConversionBtn);
            Controls.Add(loadMapBtn);
            Controls.Add(loadXdfBtn);
            Controls.Add(narrowbandErrorBtn);
            Name = "MainForm";
            ShowIcon = false;
            Text = "avail's MS43x Utility";
            ResumeLayout(false);
        }

        #endregion

        private Button narrowbandErrorBtn;
        private Button loadXdfBtn;
        private Button loadMapBtn;
        private Button voVeConversionBtn;
        private Button reloadLastCombo;
    }
}