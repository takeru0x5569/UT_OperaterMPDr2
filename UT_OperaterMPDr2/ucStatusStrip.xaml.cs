using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace UT_OperaterMPDr2 {
    /// <summary>
    /// ucStatusStrip.xaml の相互作用ロジック
    /// </summary>
    public partial class ucStatusStrip:UserControl {
        public ucStatusStrip() {
            InitializeComponent();
            this.DataContext = new StatusBarViewModel(); // DataContextを設定
        }
    }
    public class StatusBarViewModel:INotifyPropertyChanged {
        private bool _isActive;
        public bool IsActive {
            get { return _isActive; }
            set {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        private string _statusText1="Starus";
        public string StatusText1 {
            get { return _statusText1; }
            set {
                _statusText1 = value;
                OnPropertyChanged(nameof(StatusText1));
            }
        }

        private string _statusText2="Starus2";
        public string StatusText2 {
            get { return _statusText2; }
            set {
                _statusText2 = value;
                OnPropertyChanged(nameof(StatusText2));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }
    }
}
