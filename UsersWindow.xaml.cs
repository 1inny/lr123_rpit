
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
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;

namespace lr1_PaymentsBase
{
    /// <summary>
    /// Логика взаимодействия для UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow : Window
    {
        public UsersWindow()
        {
            InitializeComponent();
            cbUsers.ItemsSource = PaymentsBaseLocalEntities.GetContext().User.ToList();
            DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().User.ToList();
        }
        private void RepWordStub(string sTr, string text, Word.Document wordDoc)
        {
            var range = wordDoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: sTr, ReplaceWith: text);
        }

        private void cbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbUsers.SelectedItem != null)
            {
                DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().User.Where(x => x.Id == cbUsers.SelectedIndex + 1).ToList();
            }
            vipiska(cbUsers.SelectedItem as User);
        }
        private void btClear_Click(object sender,  RoutedEventArgs e)
        {
            DG.ItemsSource = PaymentsBaseLocalEntities.GetContext().User.ToList();
            cbUsers.SelectedIndex = -1;
        }
        private void vipiska(User select)
        {
            try
            {
                User _curUser = new User();
                _curUser = select;

                var wordApp = new Word.Application();
                Word.Document doc = wordApp.Documents.Open(@"C:\Users\DimasCo\source\repos\lr1_rpit\VIPISKA.docx");

                RepWordStub("{FIO}", _curUser.FIO, doc); RepWordStub("{PIN}", _curUser.PIN.ToString(), doc); RepWordStub("{LOGIN}", _curUser.Login, doc); RepWordStub("{PASSWORD}", _curUser.Password, doc);

                wordApp.Visible = true;
            }
            catch(Exception e) { MessageBox.Show($"ErrorВыписка: {e.Message.ToString()}"); }
        }
        private void btVip_Click(object sender, RoutedEventArgs e)
        {
            
            vipiska((sender as Button).DataContext as User);
        }
    }
}
