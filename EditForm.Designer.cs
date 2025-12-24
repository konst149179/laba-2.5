using System.Windows.Forms;

namespace RailwayApp
{
    partial class EditForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtDirection;
        private TextBox txtBaseCost;
        private CheckBox chkApplyDiscount;
        private NumericUpDown numDiscountPercent;
        private Button btnSave;
        private Button btnCancel;
        private Label lblDirection;
        private Label lblBaseCost;
        private Label lblDiscount;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblDirection = new System.Windows.Forms.Label();
            this.txtDirection = new System.Windows.Forms.TextBox();
            this.lblBaseCost = new System.Windows.Forms.Label();
            this.txtBaseCost = new System.Windows.Forms.TextBox();
            this.chkApplyDiscount = new System.Windows.Forms.CheckBox();
            this.lblDiscount = new System.Windows.Forms.Label();
            this.numDiscountPercent = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numDiscountPercent)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDirection
            // 
            this.lblDirection.AutoSize = true;
            this.lblDirection.Location = new System.Drawing.Point(20, 20);
            this.lblDirection.Name = "lblDirection";
            this.lblDirection.Size = new System.Drawing.Size(100, 16);
            this.lblDirection.TabIndex = 8;
            this.lblDirection.Text = "Направление:";
            // 
            // txtDirection
            // 
            this.txtDirection.BackColor = System.Drawing.Color.White;
            this.txtDirection.Location = new System.Drawing.Point(120, 17);
            this.txtDirection.Name = "txtDirection";
            this.txtDirection.Size = new System.Drawing.Size(250, 22);
            this.txtDirection.TabIndex = 7;
            // 
            // lblBaseCost
            // 
            this.lblBaseCost.AutoSize = true;
            this.lblBaseCost.Location = new System.Drawing.Point(20, 50);
            this.lblBaseCost.Name = "lblBaseCost";
            this.lblBaseCost.Size = new System.Drawing.Size(80, 16);
            this.lblBaseCost.TabIndex = 6;
            this.lblBaseCost.Text = "Стоимость:";
            // 
            // txtBaseCost
            // 
            this.txtBaseCost.Location = new System.Drawing.Point(120, 47);
            this.txtBaseCost.Name = "txtBaseCost";
            this.txtBaseCost.Size = new System.Drawing.Size(100, 22);
            this.txtBaseCost.TabIndex = 5;
            // 
            // chkApplyDiscount
            // 
            this.chkApplyDiscount.AutoSize = true;
            this.chkApplyDiscount.Location = new System.Drawing.Point(23, 90);
            this.chkApplyDiscount.Name = "chkApplyDiscount";
            this.chkApplyDiscount.Size = new System.Drawing.Size(150, 20);
            this.chkApplyDiscount.TabIndex = 4;
            this.chkApplyDiscount.Text = "Применить скидку";
            this.chkApplyDiscount.CheckedChanged += new System.EventHandler(this.chkApplyDiscount_CheckedChanged);
            // 
            // lblDiscount
            // 
            this.lblDiscount.AutoSize = true;
            this.lblDiscount.Enabled = false;
            this.lblDiscount.Location = new System.Drawing.Point(23, 120);
            this.lblDiscount.Name = "lblDiscount";
            this.lblDiscount.Size = new System.Drawing.Size(90, 16);
            this.lblDiscount.TabIndex = 3;
            this.lblDiscount.Text = "Процент (%):";
            // 
            // numDiscountPercent
            // 
            this.numDiscountPercent.Enabled = false;
            this.numDiscountPercent.Location = new System.Drawing.Point(120, 118);
            this.numDiscountPercent.Name = "numDiscountPercent";
            this.numDiscountPercent.Size = new System.Drawing.Size(60, 22);
            this.numDiscountPercent.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(60, 160);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Сохранить";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(170, 160);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 210);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numDiscountPercent);
            this.Controls.Add(this.lblDiscount);
            this.Controls.Add(this.chkApplyDiscount);
            this.Controls.Add(this.txtBaseCost);
            this.Controls.Add(this.lblBaseCost);
            this.Controls.Add(this.txtDirection);
            this.Controls.Add(this.lblDirection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.numDiscountPercent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}