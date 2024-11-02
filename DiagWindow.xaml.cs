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
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;

namespace lr1_PaymentsBase
{
    /// <summary>
    /// Логика взаимодействия для DiagWindow.xaml
    /// </summary>
    public partial class DiagWindow : Window
    {
        public DiagWindow()
        {
            InitializeComponent();
            ChartPayments.ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("Main"));
            var currentSeries = new Series("Payments")
            {
                IsValueShownAsLabel = true,
            };
            ChartPayments.Series.Add(currentSeries);
            cmbUsers.ItemsSource = PaymentsBaseLocalEntities.GetContext().User.ToList();
            cmbChartTypes.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if(cmbUsers.SelectedItem is User currentUser && cmbChartTypes.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartPayments.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                var categoriesList = PaymentsBaseLocalEntities.GetContext().Category.ToList();
                foreach (var category in categoriesList)
                {
                    currentSeries.Points.AddXY(category.Name, PaymentsBaseLocalEntities.GetContext().Payment.ToList().Where(p => p.User == currentUser &&
                    p.Category == category).Sum(p => p.Price * p.Num));
                }
            }
        }
    }
}
