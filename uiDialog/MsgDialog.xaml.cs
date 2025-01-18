using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace IkkForm {
    /// <summary>
    /// ���b�Z�[�W�{�b�N�X������E�C���h�E
    /// </summary>
    public partial class MsgBox:Window {
        /// <summary>
        /// ���ʂ̒�`
        /// </summary>
        public enum eMsgBoxResult {
            Yes, No, Cancel
        }
        /// <summary>
        /// ���b�Z�[�W�{�b�N�X�̎�ނ��w�肷��񋓑�
        /// </summary>
        public enum eMsgBoxType{
            Information=0,
            GoCancel = 1,
            YesNo=2,
            Exclamation=3,
        }
        /// <summary>
        /// �^�C�g��
        /// </summary>
        public string TitleText {
            get => _titleText;
            set {
                _titleText = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }
        private string _titleText = "�^�C�g��";
        /// <summary>
        /// ������
        /// </summary>
        public string Explanation {
            get => _explanation;
            set {
                _explanation = value;
                OnPropertyChanged(nameof(Explanation));
            }
        }
        private string _explanation = "������";
        /// <summary>
        /// ���{�^���̃e�L�X�g
        /// </summary>
        public string ButtonTextL {
            get => _buttonTextL;
            set {
                _buttonTextL = value;
                OnPropertyChanged(nameof(ButtonTextL));
            }
        }
        private string _buttonTextL = "���{�^��";
        /// <summary>
        /// �E�{�^���̃e�L�X�g
        /// </summary>
        public string ButtonTextR {
            get => _buttonTextR;
            set {
                _buttonTextR = value;
                OnPropertyChanged(nameof(ButtonTextR));
            }
        }
        private string _buttonTextR = "�E�{�^��";
        /// <summary>
        /// �A�C�R���摜
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
        /// �_�C�A���O�̑I������
        /// </summary>
        public new eMsgBoxResult DialogResult {
            get { return _dialogResult; }
            private set { _dialogResult = value; }
        }
        private eMsgBoxResult _dialogResult = eMsgBoxResult.Cancel;

        /// <summary>
        /// ���b�Z�[�W�{�b�N�X��\������
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Icon"></param>
        /// <returns></returns>
        public eMsgBoxResult ShowMessageDialog(string title,string message) {
            this.TitleText = title;
            this.Explanation = message;
            // ShowDialog�𓯊��I�ɑҋ@
            bool? result = base.ShowDialog();
            return DialogResult;
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="formTitle"></param>
        /// <param name="labelText"></param>
        public MsgBox(eMsgBoxType type) {
            switch(type) {
                case eMsgBoxType.Information:
                    Title="���";
                    ButtonTextL = "OK";
                    ButtonTextR = "";
                    ImageSource = "Resources/Information.drawio.png";
                    break;
                case eMsgBoxType.GoCancel:
                    Title="���s or �L�����Z��?";
                    ButtonTextL = "���s";
                    ButtonTextR = "�L�����Z��";
                    ImageSource = "Resources/GoCancel.drawio.png";
                    break;
                case eMsgBoxType.YesNo:
                    Title="�͂� or ������";
                    ButtonTextL = "�͂�";
                    ButtonTextR = "������";
                    ImageSource = "Resources/YesNo.drawio.png";
                    break;
                case eMsgBoxType.Exclamation:
                    Title="����!";
                    ButtonTextL = "OK";
                    ButtonTextR = "";
                    ImageSource = "Resources/Exclamation.drawio.png";
                    break;
            }
            InitializeComponent();
            DataContext = this; // �f�[�^�o�C���h�̃R���e�L�X�g��ݒ�
            switch(type) {
                case eMsgBoxType.Information:
                case eMsgBoxType.Exclamation:
                    // �E�{�^�����\���ɂ���
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
        /// �v���p�e�B�ύX�ʒm�C�x���g
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