using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT_OperaterMPDr2 {
    public class clsAppCore:INotifyPropertyChanged {
        private bool _toggleFlag=false;
        private CancellationTokenSource _cancellationTokenSource;
        /// <summary>
        /// テスト用のフラグ
        /// </summary>
        public bool ToggleFlag {
            get { return _toggleFlag; }
            private set {
                if(_toggleFlag != value) {
                    _toggleFlag = value;
                    OnPropertyChanged(nameof(ToggleFlag));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public clsAppCore() {
            _toggleFlag = false;
            _cancellationTokenSource = new CancellationTokenSource();
            StartToggling();
        }
        /// <summary>
        /// フラグをトグルするテスト用
        /// </summary>
        private async void StartToggling() {
            while(!_cancellationTokenSource.Token.IsCancellationRequested) {
                await Task.Delay(1000);
                ToggleFlag = !ToggleFlag;
            }
        }

        public void StopToggling() {
            _cancellationTokenSource.Cancel();
        }
    }
}
