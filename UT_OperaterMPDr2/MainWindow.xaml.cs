using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UT_OperaterMPDr2 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window {
        private clsAppCore _appCore;
        private const string SettingsFilePath = "settings.json";
        public MainWindow() {
            InitializeComponent();
            LoadSettingJson();
            _appCore = new clsAppCore();
            _ucStatusStrip.DataContext = _appCore;
        }
        /// <summary>
        /// アプリ設定をJSONから読み込み
        /// </summary>
        private void LoadSettingJson() {
            if(File.Exists(SettingsFilePath)) {
                try {
                    var json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<WindowSettings>(json);

                    if(settings != null) {
                        this.Left = settings.Left;
                        this.Top = settings.Top;
                        this.Width = settings.Width;
                        this.Height = settings.Height;
                    }
                } catch(Exception ex) {
                    // エラーハンドリング（例：ログ出力）
                    Console.WriteLine($"Failed to load window settings: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// アプリ設定をJSONへ書込み
        /// </summary>
        private void SaveSettingJson() {
            var settings = new WindowSettings {
                Left = this.Left,
                Top = this.Top,
                Width = this.Width,
                Height = this.Height
            };

            try {
                var json = JsonSerializer.Serialize(settings);
                File.WriteAllText(SettingsFilePath,json);
            } catch(Exception ex) {
                // エラーハンドリング（例：ログ出力）
                Console.WriteLine($"Failed to save window settings: {ex.Message}");
            }
        }
        /// <summary>
        /// メインウィンドウが閉じられるときの処理
        /// </summary>
        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            SaveSettingJson();
        }

    }//class

    /// <summary>
    /// ウインドウ設定を保持するクラス
    /// </summary>
    public class WindowSettings {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }//class

}//namespace
