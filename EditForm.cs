using System;
using System.Windows.Forms;

namespace RailwayApp
{
    public partial class EditForm : Form
    {
        private readonly Tariff _editedTariff;
        public Tariff CreatedTariff { get; private set; }

        public EditForm(Tariff tariffToEdit)
        {
            InitializeComponent();
            _editedTariff = tariffToEdit;

            if (_editedTariff == null)
            {
                Text = "Добавить новый тариф";
            }
            else
            {
                Text = "Редактировать тариф";
                txtDirection.Text = _editedTariff.Direction;
                txtBaseCost.Text = _editedTariff.BaseCost.ToString();

                if (_editedTariff.Strategy is PercentageDiscount discount)
                {
                    chkApplyDiscount.Checked = true;
                    numDiscountPercent.Value = discount.DiscountPercent;
                }
            }
        }

        private void chkApplyDiscount_CheckedChanged(object sender, EventArgs e)
        {
            numDiscountPercent.Enabled = chkApplyDiscount.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDirection.Text))
                    throw new Exception("Направление не может быть пустым");

                if (!double.TryParse(txtBaseCost.Text, out double baseCost) || baseCost < 1 || baseCost > 10000000)
                    throw new Exception("Введите корректную базовую стоимость");

                string direction = txtDirection.Text.Trim();

                DiscountStrategy strategy;
                if (chkApplyDiscount.Checked)
                {
                    int percent = (int)numDiscountPercent.Value;
                    if (percent == 0)
                    {
                        strategy = new NoDiscount();
                    }
                    else
                    {
                        strategy = new PercentageDiscount(percent);
                    }
                }
                else
                {
                    strategy = new NoDiscount();
                }
                CreatedTariff = new Tariff(direction, baseCost, strategy);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}