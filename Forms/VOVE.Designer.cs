namespace ms43x_util.Forms
{
    partial class VOVE
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
            veTable = new DataGridView();
            voTable = new DataGridView();
            ms43XdfBtn = new Button();
            ms43MapBtn = new Button();
            lastBtn = new Button();
            buttonPrevMap = new Button();
            buttonNextMap = new Button();
            currentMapLabel = new Label();
            label2 = new Label();
            label3 = new Label();
            iatTextBox = new TextBox();
            displacementTextBox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)veTable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)voTable).BeginInit();
            SuspendLayout();
            // 
            // veTable
            // 
            veTable.Anchor = AnchorStyles.None;
            veTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            veTable.Location = new Point(21, 258);
            veTable.Name = "veTable";
            veTable.RowHeadersWidth = 62;
            veTable.Size = new Size(1222, 439);
            veTable.TabIndex = 1;
            // 
            // voTable
            // 
            voTable.Anchor = AnchorStyles.None;
            voTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            voTable.Location = new Point(301, 14);
            voTable.Name = "voTable";
            voTable.RowHeadersWidth = 62;
            voTable.Size = new Size(942, 238);
            voTable.TabIndex = 2;
            // 
            // ms43XdfBtn
            // 
            ms43XdfBtn.Location = new Point(12, 14);
            ms43XdfBtn.Name = "ms43XdfBtn";
            ms43XdfBtn.Size = new Size(107, 34);
            ms43XdfBtn.TabIndex = 3;
            ms43XdfBtn.Text = "MS43 XDF";
            ms43XdfBtn.UseVisualStyleBackColor = true;
            ms43XdfBtn.Click += ms43XdfBtn_Click;
            // 
            // ms43MapBtn
            // 
            ms43MapBtn.Enabled = false;
            ms43MapBtn.Location = new Point(122, 14);
            ms43MapBtn.Name = "ms43MapBtn";
            ms43MapBtn.Size = new Size(107, 34);
            ms43MapBtn.TabIndex = 4;
            ms43MapBtn.Text = "MS43 BIN";
            ms43MapBtn.UseVisualStyleBackColor = true;
            ms43MapBtn.Click += ms43MapBtn_Click;
            // 
            // lastBtn
            // 
            lastBtn.Location = new Point(235, 14);
            lastBtn.Name = "lastBtn";
            lastBtn.Size = new Size(60, 34);
            lastBtn.TabIndex = 5;
            lastBtn.Text = "Last";
            lastBtn.UseVisualStyleBackColor = true;
            lastBtn.Click += lastBtn_Click;
            // 
            // buttonPrevMap
            // 
            buttonPrevMap.Enabled = false;
            buttonPrevMap.Location = new Point(12, 78);
            buttonPrevMap.Name = "buttonPrevMap";
            buttonPrevMap.Size = new Size(34, 34);
            buttonPrevMap.TabIndex = 6;
            buttonPrevMap.Text = "<";
            buttonPrevMap.UseVisualStyleBackColor = true;
            buttonPrevMap.Click += buttonPrevMap_Click;
            // 
            // buttonNextMap
            // 
            buttonNextMap.Enabled = false;
            buttonNextMap.Location = new Point(261, 78);
            buttonNextMap.Name = "buttonNextMap";
            buttonNextMap.Size = new Size(34, 34);
            buttonNextMap.TabIndex = 7;
            buttonNextMap.Text = ">";
            buttonNextMap.UseVisualStyleBackColor = true;
            buttonNextMap.Click += buttonNextMap_Click;
            // 
            // currentMapLabel
            // 
            currentMapLabel.AutoSize = true;
            currentMapLabel.Location = new Point(62, 83);
            currentMapLabel.Name = "currentMapLabel";
            currentMapLabel.Size = new Size(183, 25);
            currentMapLabel.TabIndex = 8;
            currentMapLabel.Text = "ip_maf_vo_1__map__n";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(94, 151);
            label2.Name = "label2";
            label2.Size = new Size(41, 25);
            label2.TabIndex = 9;
            label2.Text = "IAT:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 191);
            label3.Name = "label3";
            label3.Size = new Size(123, 25);
            label3.TabIndex = 10;
            label3.Text = "Displacement:";
            // 
            // iatTextBox
            // 
            iatTextBox.Enabled = false;
            iatTextBox.Location = new Point(141, 148);
            iatTextBox.Name = "iatTextBox";
            iatTextBox.Size = new Size(150, 31);
            iatTextBox.TabIndex = 11;
            iatTextBox.Text = "30.0";
            iatTextBox.TextChanged += iatTextBox_TextChanged;
            // 
            // displacementTextBox
            // 
            displacementTextBox.Enabled = false;
            displacementTextBox.Location = new Point(141, 191);
            displacementTextBox.Name = "displacementTextBox";
            displacementTextBox.Size = new Size(150, 31);
            displacementTextBox.TabIndex = 12;
            displacementTextBox.Text = "2.793";
            displacementTextBox.TextChanged += displacementTextBox_TextChanged;
            // 
            // VOVE
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1255, 709);
            Controls.Add(displacementTextBox);
            Controls.Add(iatTextBox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(currentMapLabel);
            Controls.Add(buttonNextMap);
            Controls.Add(buttonPrevMap);
            Controls.Add(lastBtn);
            Controls.Add(ms43MapBtn);
            Controls.Add(ms43XdfBtn);
            Controls.Add(voTable);
            Controls.Add(veTable);
            Name = "VOVE";
            ShowIcon = false;
            Text = "avail's MS4x Utility - VO to VE conversion";
            ((System.ComponentModel.ISupportInitialize)veTable).EndInit();
            ((System.ComponentModel.ISupportInitialize)voTable).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView veTable;
        private DataGridView voTable;
        private Button ms43XdfBtn;
        private Button ms43MapBtn;
        private Button lastBtn;
        private Button buttonPrevMap;
        private Button buttonNextMap;
        private Label currentMapLabel;
        private Label label2;
        private Label label3;
        private TextBox iatTextBox;
        private TextBox displacementTextBox;
    }
}