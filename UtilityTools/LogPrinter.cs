using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Text;
//using System.Diagnostics;

namespace UtilityTools {
    public class LogPrinter {
        public String OwnerName { get; set; } = "Unkown";

        private UdpClient? _udpClient = null;
        private IPEndPoint? _remoteEndPoint = null;
        /// <summary>
        /// ロギングスタート時に残す過去のログファイル上限数
        /// </summary>
        public int LimitLogFileNumber { private set; get; }
        /// <summary>
        /// 受け付けるログの優先度定義
        /// </summary>
        public eLogPriority LogPriority { private set; get; }
        public enum eLogPriority:uint {
            FATAL = 0x1   << 1,   //致命的エラー(ModbusLinkerの内部処理でエラー)
            ERROR = 0x1   << 2,   //運用エラー(運用が止まるエラー)
            WARNING = 0x1 << 3,   //警告(人為的に止めたなど、運用上想定しているが稼働が止まるような情報)
            INFO = 0x1    << 4,   //正常動作している情報をロギング
            NOTE = 0x1    << 5,   //運用ログに落とす
            DEBUG = 0x1   << 6,   //運用ではロギングしない細かい情報
        }
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        private static LogPrinter? _instance = null;
        //スレッド関連
        private object lockObj1 = new object();
        private object lockObj2 = new object();
        private Thread? _thread = null;
        private readonly int _cycleTimeMs = 2000;
        /// <summary>
        /// ログファイルの出力先ディレクトリ
        /// </summary>
        private string LogFilePath = "";
        /// <summary>
        /// メッセージを一時格納するためのキュー
        /// </summary>
        private ConcurrentQueue<string> _que = new ConcurrentQueue<string>();
        private StreamWriter? _sw = null;
        /// <summary>
        /// ロギング中フラグ
        /// </summary>
        private volatile static bool _threadEnable = false;
//        private UTOpMpd? _owner = null;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogPrinter() {
            _threadEnable = false;
            _udpClient = new UdpClient();
            _remoteEndPoint = new IPEndPoint(IPAddress.Loopback,5005);
        }
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static LogPrinter Instance {
            get {
                if(_instance == null) {
                    _instance = new LogPrinter();
                }
                return _instance;
            }
        }
        /// <summary>
        /// ログファイルへの書き込みスレッドスタート
        /// </summary>
        public void StartLogging(string logFileDir,eLogPriority priority,int limitFileNum) {
            lock(lockObj1) {
//                _owner = owner;
                //ファイル残す上限数を設定
                this.LimitLogFileNumber = limitFileNum;
                //ログ保存フォルダが無かったら作成
                if(Directory.Exists(logFileDir) == false) {
                    Directory.CreateDirectory(logFileDir);
                }
                //時刻をもとにログファイル名を作ってファイル生成
                this.LogFilePath = Path.Combine(logFileDir,DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".log");
                File.Create(this.LogFilePath).Close();
                _sw = File.AppendText(this.LogFilePath);
                // ディレクトリ内の全てのファイルを取得し、最終更新日時でソート
                var files = new DirectoryInfo(logFileDir).GetFiles().OrderBy(f => f.LastWriteTime).ToList();
                //ログファイルの数が規定数に収まるように古いファイルを削除
                if(files.Count > this.LimitLogFileNumber) {
                    int _removeNum = files.Count - this.LimitLogFileNumber;
                    _sw.WriteLine("Removed old Log files :");
                    for(int i = 0; i < _removeNum; i++) {
                        File.Delete(files[i].FullName);
                        _sw.WriteLine($" - {files[i].FullName}");
                    }
                }
                //ログプライオリティの設定
                LogPriority = priority;
                _sw.WriteLine($"Logging priority = {priority}");
                //スレッド開始
                _threadEnable = true;
                _thread = new Thread(() => ThreadProcess());
                _thread.Name = "LoggerThread";
                _thread.Priority = ThreadPriority.BelowNormal;
                // バックグラウンドスレッドに設定
                _thread.IsBackground = true;
                _thread.Start();
            }
        }
        /// <summary>
        /// スレッド停止処理
        /// </summary>
        /// <returns></returns>
        public void StopLogging() {
            lock(lockObj1) {
                if(_thread != null && _thread.IsAlive) {
                    _threadEnable = false;
                    _thread.Join();
                    _thread = null;
                }
            }
        }
        /// <summary>
        /// スレッド処理本体
        /// </summary>
        private void ThreadProcess() {
            if(_sw == null) { return; }
            //開始ログ
            WriteLog(DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + ":[START] ロギング開始しました");
            string? message = string.Empty;

            //キューに溜まっているログを書き出すループ
            while(_threadEnable) {
                //インターバル
                Thread.Sleep(_cycleTimeMs);
                try {
                    if(_que.Count <= 0) { continue; }
                    //メッセージが溜まっている限り引っこ抜いてログに書く
                    while(_que.TryDequeue(out message)) {
                        if(message == null) { break; }
                        WriteLog(message);
                    }
                } catch(Exception) {; }
            }
            _sw.Dispose();
            _sw = null;
        }
        private void WriteLog(string message) {
            if(_sw == null) { return; }
            _sw.WriteLine(message);
            _sw.Flush();
        }
        /// <summary>
        /// ログを受け入れてキューに追加
        /// </summary>
        /// <param name="msg"></param>
        public void Print(eLogPriority priority,string message) {
            //書式
            string timeStr = DateTime.Now.ToString("HH:mm:ss");
            
            // UDPでメッセージを送信
            this.sendUdp(priority,timeStr,message);

            lock(lockObj2) {
                // ロギングが停止されている場合はメッセージを追加しない
                if(!_threadEnable) { return; }
                //ログレベルが低い(数値の大きい)ものは無視
                if((uint)priority > (uint)LogPriority) { return; }
                //キューに格納
                string msg = $"{timeStr}[{priority}]{message}";
                _que.Enqueue(msg);
            }
        }
        /// <summary>
        /// UDPに色付きメッセージを流す
        /// </summary>
        private void sendUdp(eLogPriority priority,string timeStr,string message) {
            if(this._udpClient == null || this._remoteEndPoint == null){
                return;
            }
#pragma warning disable CS8524
            // LogPriorityに応じて色情報を追加
            string colorCode = priority switch {
                eLogPriority.FATAL => "\x1b[31m",   // 赤
                eLogPriority.ERROR => "\x1b[91m",   // 明るい赤
                eLogPriority.WARNING => "\x1b[33m", // 黄色
                eLogPriority.INFO => "\x1b[36m",    // 明るい青
                eLogPriority.NOTE => "\x1b[32m",    // 明るい緑
                eLogPriority.DEBUG => "\x1b[37;2m", // 薄いグレー
            };
#pragma warning restore CS8524
            string mag = timeStr + colorCode + $"[{OwnerName}]"+message + "\x1b[0m"; // メッセージの後にリセットコードを追加
            byte[] data = Encoding.UTF8.GetBytes(mag);
            _udpClient.Send(data,data.Length,_remoteEndPoint);
        }
    }//class
}//namespace