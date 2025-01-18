namespace ScannerLink {
    public partial class Scanner {
        /// <summary>
        /// データ保存が完了したことを通知するイベント
        /// </summary>
        public event Action<string>? OnDataSaved = null;
        /// <summary>
        /// スキャン完了通知イベント
        /// </summary>
        public event Action? OnScanFinish = null;
        /// <summary>
        /// センサ完了通知イベント
        /// </summary>
        public event Action? OnMoveFinish = null;
    }//class
}//namespace
