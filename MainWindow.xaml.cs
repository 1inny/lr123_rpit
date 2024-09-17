using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace lr1_PaymentsBase
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.ToList();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            EditWindow editwin = new EditWindow(null);
            editwin.ShowDialog();
        }

        private void btEdit_Click(object sender, RoutedEventArgs e) 
        {
            EditWindow editwin = new EditWindow((sender as Button).DataContext as Payment);
            editwin.ShowDialog();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.ToList();
        }

        private void btDel_Click(object sender, RoutedEventArgs e)
        {
            var PaymForDel = DG.SelectedItems.Cast<Payment>().ToList();
            if (PaymForDel.Count == 0)
            {
                MessageBox.Show("Вы не выбрали элемент!"); 
            }
            else if (PaymForDel.Count > 0)
            {
                if (MessageBox.Show($"Вы точно хотите удалить данные ({PaymForDel.Count()})?", "Внимание!", MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
                {
                    try
                    {
                        PaymentsBaseLocalEntities.GetContext().Payment.RemoveRange(PaymForDel);
                        PaymentsBaseLocalEntities.GetContext().SaveChanges();
                        MessageBox.Show("Data deleted!");

                        DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.ToList();
                    }catch(Exception ex) { MessageBox.Show($"Error: {ex.ToString()}"); }
                } 
            }
        }
    }
}
