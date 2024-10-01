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
using System.Security.Cryptography.X509Certificates;
using System.Security.RightsManagement;

namespace lr1_PaymentsBase
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int allcount { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            cbCategory.ItemsSource = PaymentsBaseLocalEntities.GetContext().Category.ToList();
            cbFIO.ItemsSource = PaymentsBaseLocalEntities.GetContext().User.ToList();
            DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.ToList();
            
            DG.SelectAll();  allcount = DG.SelectedItems.Count; DG.UnselectAll();
            
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

        private void CountItems()
        {
            DG.SelectAll(); int i = DG.SelectedItems.Count; DG.UnselectAll();
            mItem.Header = ($"Выбрано {i} из {allcount}");
        }
        private void btSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbCategory.SelectedItem != null && cbFIO.SelectedItem != null)
                {
                   DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.Where(x => x.CategoryId == cbCategory.SelectedIndex + 1 && x.UserId == cbFIO.SelectedIndex + 1).ToList();     
                   CountItems();
                }
                else if (cbCategory.SelectedItem != null && cbFIO.SelectedItem == null)
                {
                    DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.Where(x => x.CategoryId == cbCategory.SelectedIndex + 1).ToList();
                    CountItems();
                }
                else if (cbCategory.SelectedItem == null && cbFIO.SelectedItem != null)
                {
                    DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.Where(x => x.UserId == cbFIO.SelectedIndex + 1).ToList();
                    CountItems();
                }
                else
                {
                    MessageBox.Show("Вы не выбрали данные для отсортировки!");
                }
            }catch (Exception ex) {MessageBox.Show($"Error: {ex.Message.ToString()}"); }
        }

        private void btClear_Click (object sender, RoutedEventArgs e)
        {
            DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.ToList();
            cbCategory.SelectedIndex = -1;
            cbFIO.SelectedIndex = -1;
            mItem.Header = "";
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
