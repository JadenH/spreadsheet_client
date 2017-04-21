namespace SpreadsheetGUI
{
    partial class LauncherWindow
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
            this.SpreadsheetTextBox = new System.Windows.Forms.TextBox();
            this.OpenButton = new System.Windows.Forms.Button();
            this.SpreadsheetName = new System.Windows.Forms.Label();
            this.IPAddressText = new System.Windows.Forms.Label();
            this.IPAddressTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SpreadsheetTextBox
            // 
            this.SpreadsheetTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpreadsheetTextBox.Location = new System.Drawing.Point(48, 135);
            this.SpreadsheetTextBox.Name = "SpreadsheetTextBox";
            this.SpreadsheetTextBox.Size = new System.Drawing.Size(189, 29);
            this.SpreadsheetTextBox.TabIndex = 0;
            this.SpreadsheetTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // OpenButton
            // 
            this.OpenButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenButton.Location = new System.Drawing.Point(48, 186);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(189, 47);
            this.OpenButton.TabIndex = 1;
            this.OpenButton.Text = "OPEN";
            this.OpenButton.UseVisualStyleBackColor = true;
            // 
            // SpreadsheetName
            // 
            this.SpreadsheetName.AutoSize = true;
            this.SpreadsheetName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpreadsheetName.Location = new System.Drawing.Point(37, 98);
            this.SpreadsheetName.Name = "SpreadsheetName";
            this.SpreadsheetName.Size = new System.Drawing.Size(212, 25);
            this.SpreadsheetName.TabIndex = 2;
            this.SpreadsheetName.Text = "Spreadsheet Name";
            this.SpreadsheetName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // IPAddressText
            // 
            this.IPAddressText.AutoSize = true;
            this.IPAddressText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPAddressText.Location = new System.Drawing.Point(67, 18);
            this.IPAddressText.Name = "IPAddressText";
            this.IPAddressText.Size = new System.Drawing.Size(148, 25);
            this.IPAddressText.TabIndex = 4;
            this.IPAddressText.Text = "IP ADDRESS";
            this.IPAddressText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // IPAddressTextBox
            // 
            this.IPAddressTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPAddressTextBox.Location = new System.Drawing.Point(48, 56);
            this.IPAddressTextBox.Name = "IPAddressTextBox";
            this.IPAddressTextBox.Size = new System.Drawing.Size(189, 29);
            this.IPAddressTextBox.TabIndex = 3;
            this.IPAddressTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LauncherWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.IPAddressText);
            this.Controls.Add(this.IPAddressTextBox);
            this.Controls.Add(this.SpreadsheetName);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.SpreadsheetTextBox);
            this.Name = "LauncherWindow";
            this.Text = "Launcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SpreadsheetTextBox;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Label SpreadsheetName;
        private System.Windows.Forms.Label IPAddressText;
        private System.Windows.Forms.TextBox IPAddressTextBox;
    }
}