using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace lr1_PaymentsBase
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private Payment _currentPaym = new Payment();
        public EditWindow(Payment selectedPayment)
        {
            InitializeComponent();
            if (selectedPayment != null)
            {
                _currentPaym = selectedPayment;
            }

            DataContext = _currentPaym;
            cbCategory.ItemsSource = PaymentsBaseLocalEntities.GetContext().Category.ToList();
            cbFIO.ItemsSource = PaymentsBaseLocalEntities.GetContext().User.ToList();
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int k = Convert.ToInt32(tbKol.Text);
                if (_currentPaym.Id == 0)
                    PaymentsBaseLocalEntities.GetContext().Payment.Add(_currentPaym);
                try
                {
                    PaymentsBaseLocalEntities.GetContext().SaveChanges();
                    MessageBox.Show("Data saved.");
                    Close();
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message.ToString()}"); }
            }catch (Exception exc) { MessageBox.Show($"Error: {exc.Message.ToString()}"); }
           
        }
    }
}
