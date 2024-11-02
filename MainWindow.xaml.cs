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
using Word = Microsoft.Office.Interop.Word;

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
            
            allcount = DG.Items.Count;
            
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

        private void btUsers_Click(object obj, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow();
            usersWindow.ShowDialog();
        }
        private void btDiag_Click(object obj, RoutedEventArgs e)
        {
            DiagWindow diagWindow = new DiagWindow();
            diagWindow.ShowDialog();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.ToList();
            cbCategory.SelectedIndex = -1;
            cbFIO.SelectedIndex = -1;
            mItem.Header = "";
        }

        private void CountItems()
        {
            int n = DG.Items.Count;
            var items = DG.ItemsSource;
            decimal sum = 0;
            foreach (Payment item in items) 
            {
                sum += item.Sum;
            }
            mItem.Header = ($"Выбрано {n} из {allcount}.  Сумма: {sum}");
        }
        private void Filter_DG()
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
                    
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message.ToString()}"); }
        }
        

        private void btClear_Click (object sender, RoutedEventArgs e)
        {
            DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().Payment.ToList();
            cbCategory.SelectedIndex = -1;
            cbFIO.SelectedIndex = -1;
            mItem.Header = "";
        }

        private void btExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allusers = PaymentsBaseLocalEntities.GetContext().User.ToList();
                var allCategory = PaymentsBaseLocalEntities.GetContext().Category.ToList();

                var app = new Word.Application();
                Word.Document doc = app.Documents.Add();

                foreach (var user in allusers)
                {
                    Word.Paragraph userParagrapth = doc.Paragraphs.Add();
                    Word.Range userRange = userParagrapth.Range;
                    userRange.Text = user.FIO;
                    
                    userRange.InsertParagraphAfter();

                    Word.Paragraph tableParagraph = doc.Paragraphs.Add();
                    Word.Range tableRange = tableParagraph.Range;
                    Word.Table paymentsTable = doc.Tables.Add(tableRange, allCategory.Count() + 1, 2);
                    paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    Word.Range cellRange;


                    cellRange = paymentsTable.Cell(1, 1).Range;
                    cellRange.Text = "Категория";
                    cellRange = paymentsTable.Cell(1, 2).Range;
                    cellRange.Text = "Сумма расходов";

                    paymentsTable.Rows[1].Range.Bold = 1;
                    paymentsTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    for (int i = 0; i < allCategory.Count; i++)
                    {
                        var curCategory = allCategory[i];

                        cellRange = paymentsTable.Cell(i + 2, 1).Range;
                        cellRange.Text = curCategory.Name;

                        cellRange = paymentsTable.Cell(i + 2, 2).Range;
                        cellRange.Text = user.Payment.ToList().Where(p => p.Category == curCategory).Sum(p => p.Num * p.Price).ToString("N2") + " руб.";
                    }

                    Payment maxPayment = user.Payment.OrderByDescending(p => p.Sum).FirstOrDefault();
                    if (maxPayment != null)
                    {
                        Word.Paragraph maxPaymentParagraph = doc.Paragraphs.Add();
                        Word.Range maxPaymentRange = maxPaymentParagraph.Range;
                        maxPaymentRange.Text = $"Самый дорогостоящий платеж - {maxPayment.Name} за {(maxPayment.Sum).ToString("N2")}" + $" руб. от {maxPayment.Date.ToString("dd.MM.yyyy")}";
                        
                        maxPaymentRange.Font.Color = Word.WdColor.wdColorDarkRed;
                        maxPaymentRange.InsertParagraphAfter();
                    }

                    Payment minPayment = user.Payment.OrderByDescending(p => p.Sum).FirstOrDefault();
                    if (minPayment != null)
                    {
                        Word.Paragraph minPaymentParagraph = doc.Paragraphs.Add();
                        Word.Range minPaymentRange = minPaymentParagraph.Range;
                        minPaymentRange.Text = $"Самый дешевый платеж - {minPayment.Name} за {(minPayment.Sum).ToString("N2")}" + $"  руб. от {minPayment.Date.ToString("dd.MM.yyyy")}";
                        
                        minPaymentRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                        minPaymentRange.InsertParagraphAfter();
                    }
                    if (user != allusers.LastOrDefault())
                        doc.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);


                }
                app.Visible = true;
                doc.SaveAs2(@"C:\test\test.docx");
                doc.SaveAs2(@"C:\test\testd.pdf", Word.WdExportFormat.wdExportFormatPDF);
            } catch (Exception ex) { MessageBox.Show($"Error: {ex.Message.ToString()}"); };
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

        private void cbFIO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter_DG();
        }

        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter_DG();
        }
    }
}
