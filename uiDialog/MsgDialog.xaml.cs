using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace IkkForm {
    /// <summary>
    /// メッセージボックス風自作ウインドウ
    /// </summary>
    public partial class MsgBox:Window {
        /// <summary>
        /// 結果の定義
        /// </summary>
        public enum eMsgBoxResult {
            Yes, No, Cancel
        }
        /// <summary>
        /// メッセージボックスの種類を指定する列挙体
        /// </summary>
        public enum eMsgBoxType{
            Information=0,
            GoCancel = 1,
            YesNo=2,
            Exclamation=3,
        }
        /// <summary>
        /// タイトル
        /// </summary>
        public string TitleText {
            get => _titleText;
            set {
                _titleText = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }
        private string _titleText = "タイトル";
        /// <summary>
        /// 説明分
        /// </summary>
        public string Explanation {
            get => _explanation;
            set {
                _explanation = value;
                OnPropertyChanged(nameof(Explanation));
            }
        }
        private string _explanation = "説明文";
        /// <summary>
        /// 左ボタンのテキスト
        /// </summary>
        public string ButtonTextL {
            get => _buttonTextL;
            set {
                _buttonTextL = value;
                OnPropertyChanged(nameof(ButtonTextL));
            }
        }
        private string _buttonTextL = "左ボタン";
        /// <summary>
        /// 右ボタンのテキスト
        /// </summary>
        public string ButtonTextR {
            get => _buttonTextR;
            set {
                _buttonTextR = value;
                OnPropertyChanged(nameof(ButtonTextR));
            }
        }
        private string _buttonTextR = "右ボタン";
        /// <summary>
        /// アイコン画像
        /// </summary>
        public string ImageSource {
            get => _imageSource;
            set {
                _imageSource  = value;
                OnPropertyChanged(nameof(_imageSource));
            }
        }
        private string _imageSource = "Resources/Information.drawio.png";
        /// <summary>
        /// ダイアログの選択結果
        /// </summary>
        public new eMsgBoxResult DialogResult {
            get { return _dialogResult; }
            private set { _dialogResult = value; }
        }
        private eMsgBoxResult _dialogResult = eMsgBoxResult.Cancel;

        /// <summary>
        /// メッセージボックスを表示する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Icon"></param>
        /// <returns></returns>
        public eMsgBoxResult ShowMessageDialog(string title,string message) {
            this.TitleText = title;
            this.Explanation = message;
            // ShowDialogを同期的に待機
            bool? result = base.ShowDialog();
            return DialogResult;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="formTitle"></param>
        /// <param name="labelText"></param>
        public MsgBox(eMsgBoxType type) {
            switch(type) {
                case eMsgBoxType.Information:
                    Title="情報";
                    ButtonTextL = "OK";
                    ButtonTextR = "";
                    ImageSource = "Resources/Information.drawio.png";
                    break;
                case eMsgBoxType.GoCancel:
                    Title="続行 or キャンセル?";
                    ButtonTextL = "続行";
                    ButtonTextR = "キャンセル";
                    ImageSource = "Resources/GoCancel.drawio.png";
                    break;
                case eMsgBoxType.YesNo:
                    Title="はい or いいえ";
                    ButtonTextL = "はい";
                    ButtonTextR = "いいえ";
                    ImageSource = "Resources/YesNo.drawio.png";
                    break;
                case eMsgBoxType.Exclamation:
                    Title="注意!";
                    ButtonTextL = "OK";
                    ButtonTextR = "";
                    ImageSource = "Resources/Exclamation.drawio.png";
                    break;
            }
            InitializeComponent();
            DataContext = this; // データバインドのコンテキストを設定
            switch(type) {
                case eMsgBoxType.Information:
                case eMsgBoxType.Exclamation:
                    // 右ボタンを非表示にする
                    ButtonR.IsEnabled = false;
                    ButtonR.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }
        public void Dispose() {
            this.Close();
        }
        /// <summary>
        /// プロパティ変更通知イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonL_Click(object sender,RoutedEventArgs e) {
            this.DialogResult = eMsgBoxResult.Yes;
            this.Close();
        }
        private void ButtonR_Click(object sender,RoutedEventArgs e) {
            this.DialogResult = eMsgBoxResult.No;
            this.Close();
        }
    }//calss
}//namespace