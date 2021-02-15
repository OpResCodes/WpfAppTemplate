using NLog;
using System.Windows.Controls;

namespace WpfTemplate.Views.Base
{

    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
            LogManager.GetCurrentClassLogger().Debug("LogView ready.");
        }
    }



}
