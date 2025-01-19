using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtilityTools;

namespace LogScreen {
    /// <summary>
    /// 
    /// </summary>
    public partial class clsLogScreen:UserControl {
        private LogPrinter logPrinter;
        private Timer timer;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public clsLogScreen() {
            InitializeComponent();
            logPrinter = LogPrinter.Instance;
            logPrinter.MessageReceived += OnMessageReceived; // イベントを購読
            this.Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender,RoutedEventArgs e) {
            logPrinter.MessageReceived -= OnMessageReceived; // イベントの購読を解除
        }

        private void OnMessageReceived(object? sender,string message) {
            // UIスレッドでTextBlockを更新
            Application.Current.Dispatcher.Invoke(() => {
                MessageTextBlock.Text += message + Environment.NewLine;
            });
        }
    }//class
}//namespace
