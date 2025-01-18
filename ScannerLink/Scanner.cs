using RemoteControlLibrary;
using UtilityTools;

namespace ScannerLink {
    /// <summary>
    /// スキャナ通信クラス：
    /// </summary>
    public partial class Scanner {
        /// <summary>
        /// InsiteScanリモート接続TCPクライアント
        /// </summary>
        private RemoteScanClient _rScanClient = new RemoteScanClient();
        /// <summary>
        /// ログ収集クラスへの参照
        /// </summary>
        protected LogPrinter? _logger = null;
        /// <summary>
        /// TCP接続状態を示す(TCP切断して状態不明時はnull)
        /// </summary>
        public bool IsConnected { get; private set; } = false;
        /// <summary>
        /// インターロック状態(TCP切断して状態不明時はnull)
        /// </summary>
        public bool? IsInterlock { get; private set; } = null;
        /// <summary>
        /// 非常停止ボタン状態(TCP切断して状態不明時はnull)
        /// </summary>
        public bool? IsEmergency { get; private set; } = null;
        /// <summary>
        /// InsightScanから獲得できるフラグ(スキャン中だったりインタロックでFALSEになる様子)
        /// </summary>
        public bool? IsScanReady { get; private set; } = null;
        /// <summary>
        /// 初期化中フラグ
        /// </summary>
        public bool Busy_Initialize { get; private set; } = false;
        /// <summary>
        /// プローブ移動中フラグ
        /// </summary>
        public bool Busy_ProveMooving { get; private set; } = false;
        /// <summary>
        /// スキャン中フラグ
        /// </summary>
        public bool Busy_Scanning { get; private set; } = false;
        /// <summary>
        /// レシピファイルのパス
        /// </summary>
        public string RecipePath { get; internal set; } = "";
        /// <summary>
        /// レシピファイルのロード状態
        /// </summary>
        public bool IsRecipeLoaded { get; private set; } = false;
        /// <summary>
        /// データ保存ファイルパス
        /// </summary>
        public string saveFilePath { get; private set; } = "";
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="port"></param>
        public Scanner() {
            _rScanClient.HostName = "127.0.0.1";//デフォルト値
            _rScanClient.Port = 53301;
            //リモートコントロールクライアントのイベントハンドラの登録
            _rScanClient.OnConnected += EvHconScan;
            _rScanClient.OnDisconnected += EvHdisconScan;
            _rScanClient.OnError += EvHerrScan;
            _rScanClient.OnReceiveCommandRequest += EvHGetRx;
            //自身のイベントハンドラの登録
            //【Fix1027】継承先でもハンドラ追加しているのでこれがあると複数回同じハンドラが呼ばれる this.OnScanFinish += this.handleScanFinish;//スキャン終了イベント
            //【Fix1027】継承先でもハンドラ追加しているのでこれがあると複数回同じハンドラが呼ばれる this.OnDataSaved += this.handleDataSaved;//データ保存完了イベント
        }
        /// <summary>
        /// InsightScanとの接続IP設定変更
        /// </summary>
        public void SetHostPort(string hostName,int port) {
            if(IsConnected == true) {
                return;//接続中は変更不可
            }
            _rScanClient.HostName = hostName;
            _rScanClient.Port = port;
            _logger?.Print(LogPrinter.eLogPriority.DEBUG,$"IPアドレスセット:{hostName}:[{port}]");
        }
        /// <summary>
        /// ロガーへの接続(ユニットテストでは接続しない、接続していればロガーにメッセージが飛ぶ)
        /// </summary>
        /// <param name="logger"></param>
        public void AtouchLogger(LogPrinter refToLogger) {
            if(_logger == null) {
                _logger = refToLogger;
            }
        }
    }//class
}//namespace
