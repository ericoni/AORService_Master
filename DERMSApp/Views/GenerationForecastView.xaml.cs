using DERMSApp.Model;
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
using System.Xml;

namespace DERMSApp.Views
{
    /// <summary>
    /// Interaction logic for GenerationForecastView.xaml
    /// </summary>
    public partial class GenerationForecastView : UserControl, IWeakEventListener
    {
        public GenerationForecastView()
        {
            InitializeComponent();
            EventSystem.Subscribe<XmlDocument>(displayTimeLineEvents);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void displayTimeLineEvents(XmlDocument doc)
        {
            timeline.ClearEvents();
            timeline.ResetEvents(doc.InnerXml);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return true;
        }
    }
}
