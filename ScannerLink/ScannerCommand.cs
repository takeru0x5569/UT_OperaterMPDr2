using RemoteControlLibrary;
using System.Diagnostics;
using UtilityTools;

namespace ScannerLink {
    public partial class Scanner {
        /// <summary>
        /// フラグの確認
        /// </summary>
        /// <returns></returns>
        private bool checkFlags(out string errorMessage) {
            bool _ret = true;
            string _msg = "";
            if(this.IsEmergency == true) {
                _msg += "非常停止が押されています\n";
                _ret = false;
            }
            if(this.IsInterlock == true) {
                _msg += "インターロック状態です\n";
                _ret = false;
            }
            errorMessage = _msg;
            return _ret;
        }
        /// <summary>
        /// 諸々のコマンド送る前に実施する状態確認
        /// </summary>
        /// <param name="checkErrorTitle"></param>
        private bool checkStatus(string checkErrorTitle){
            while(true) {
                string _msg = "";
                if(this.checkFlags(out _msg)==true) {
                    break;
                } else {
                    this._logger?.Print(LogPrinter.eLogPriority.WARNING,"リトライ待ち："+checkErrorTitle);
                //? var result = Fixer.GetInstance().GetDialog(_msg, checkErrorTitle, CustomMsgBox.Btn.RetryCancel, CustomMsgBox.Mark.Err).ShowDialog();
                //? if(result == DialogResult.Cancel) {
                //?     return false;
                //? }
                }
            }
            return true;//ここまで行ければ成功
        }
        /// <summary>
        /// フラグ類のクリア
        /// </summary>
        private void clearFlags() {
            this.IsEmergency = null;
            this.IsInterlock = null;
            this.Busy_Initialize = false;
            this.Busy_ProveMooving = false;
            this.Busy_Scanning = false;
        }
        /// <summary>
        /// TCP接続オープン
        /// </summary>
        public bool Open() {
            //接続済みなら成功だけ返す
            if(this.IsConnected == true) {
                return true;
            }
            //キャンセルされなければリトライさせる
            bool _ret = false;
            while(_ret == false) {
                _ret = _rScanClient.Open();
                if(_ret == false) {
                    LogPrinter.Instance.Print(LogPrinter.eLogPriority.WARNING,"InsightScan接続できません");
                    Thread.Sleep(1000);
                    //? var result = Fixer.GetInstance().GetDialog("InsightScanが起動していることを確認してください。\r\n接続パラメータを確認してください。",
                    //?     "InsightScan接続エラー", CustomMsgBox.Btn.RetryCancel, CustomMsgBox.Mark.Err).ShowDialog();
                    //YesNo選択ダイアログを表示してリトライかキャンセルかを選ばせる

                    //???????if(result == DialogResult.Cancel) {
                    //???????    return false;
                    //???????}
                }
            }
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.INFO,"InsightScan接続しました");
            //フラグクリア
            this.clearFlags();
            return _ret;
        }
        /// <summary>
        /// TCP切断
        /// </summary>
        public void Close() {
            _rScanClient?.Close();//リモートを閉じる
            this.clearFlags();//フラグをクリア
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.INFO,"InsightScan切断しました");
        }
        /// <summary>
        /// スキャナのステータス要求
        /// </summary> 
        public bool RequestScanState() {
            //接続状態に持っていく
            if(this.Open() == false) {
                return false;
            }
            //状態を問い合わせるコマンドを投げる（結果はTCP受信イベントで見れるのでここでは見れない）
            _rScanClient.SendCommandRequest(
                CommandTypes.GET,
                RemoteScanCommands.RequestScanState,
                ContentTypes.xml,
                null
            );
            return true;
        }
        /// <summary>
        /// スキャナ初期化原点移動
        /// </summary>
        /// <param name="recipi"></param>
        public bool InitOrigin() {
            //ステータス取得
            if(this.RequestScanState() == false) { return false; }
            //ステータス確認
            if(this.checkStatus("スキャナ初期化エラー") == false) {return false;}
            //初期化中フラグを立てる→初期化中フラグは受信応答で成功していたら立てるのでここでは立てない
            //xx this.Busy_Initialize = true;
            //初期化要求を投げる
            _rScanClient.SendCommandRequest(
                CommandTypes.PUT,
                RemoteScanCommands.InitSensor,
                ContentTypes.none,
                null
            );
            return true;
        }
        /// <summary>
        /// レシピファイル読み込み要求
        /// </summary>
        /// <param name="recipeFullPath">レシピファイルへのパス</param>
        public virtual bool LoadRecipi() {
            string _recipeFullPath = this.RecipePath;
            //ファイルチェック　→　ネットワーク越しだとInsightScanから見てるパスと一致しないので事前チェックはしない
            // if(!File.Exists(_recipeFullPath)) {
            //     MessageBox.Show("レシピファイルが見つかりません。\n" + _recipeFullPath,"レシピ読み込みエラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
            //     return false;
            // }
            //オープン
            if(this.Open() == false) {
                return false;
            }
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"レシピ読込要求："+_recipeFullPath);
            //レシピ読みフラグはいったんクリア
            this.IsRecipeLoaded=false;
            //レシピ読み込みコマンド
            this._respWaitFlag = false;
            _rScanClient.SendCommandRequest(
                CommandTypes.PUT,
                RemoteScanCommands.LoadScanRecipe,
                ContentTypes.xml,
                new LoadScanRecipeParams(_recipeFullPath)
            );
            //レシピ読み込み完了まで待つ
            while(this._respWaitFlag == false) {
                Thread.Sleep(100);
            }
            bool _ret =  this.IsRecipeLoaded;
            //? if(_ret==false) {
            //?     Fixer.GetInstance().GetDialog("スキャナが初期化されていない可能性があります\r\nレシピファイルが存在しない可能性があります\r\n"+_recipeFullPath,"レシピロード失敗", CustomMsgBox.Btn.Ok, CustomMsgBox.Mark.Err).ShowDialog();
            //? }
            return _ret;
        }
        /// <summary>
        /// スキャン開始要求
        /// </summary>
        public virtual bool StartScaning() {
            //ステータスを見つつ接続状態に持っていく
            //if(this.RequestScanState() == false) {
            //    return false;
            //}
            this.RequestScanState();
           //xx if(this.IsScanReady == false) {
           //xx     Fixer.GetInstance().GetDialog("スキャン準備ができていません。","スキャン開始エラー", CustomMsgBox.Btn.Ok, CustomMsgBox.Mark.Err).ShowDialog();
           //xx     return false;
           //xx }
           //xx //レシピ読み込み確認
           //xx if(this.IsRecipeLoaded==false) {
           //xx     this._logger?.AppendLog(Logger.eLogPriority.ERROR,"スキャン開始時にレシピ未ロードです");
           //xx     Fixer.GetInstance().GetDialog("レシピファイルが読み込まれていません。","スキャン開始エラー", CustomMsgBox.Btn.Ok, CustomMsgBox.Mark.Err).ShowDialog();
           //xx     return false;
           //xx }
            //スキャン中フラグを立てる
            this.Busy_Scanning = true;
            _rScanClient.SendCommandRequest(//スキャン開始
                CommandTypes.PUT,
                RemoteScanCommands.ControlScan,
                ContentTypes.xml,
                new ControlScanParams(ScanControl.Start)
            );
            return true;
        }
        /// <summary>
        /// スキャン結果保存要求(指定ディレクトリに時刻名で保存)
        /// </summary>
        public virtual bool SaveScanData(string saveDirctory,String prifix = "") {
            //ディレクトリが無ければ生成
            if(Directory.Exists(saveDirctory) == false) {
                try {
                    Directory.CreateDirectory(saveDirctory);
                } catch(Exception) {
                    return false;
                }
            }
            //時刻をもとに保存ファイルDirctoryパス生成(万が一被っても時刻が進んで名前が変わる)
            string saveFilePath = "";
            while(true) {
                saveFilePath = Path.Combine(saveDirctory,prifix + DateTime.Now.ToString("yyyy_MMdd_HHmm_ss") + ".isd");
                if(Directory.Exists(saveFilePath) == false) {
                    break;
                }
                Thread.Sleep(1000);
            }
            //接続状態に持っていく
            if(this.IsConnected != true) {
                if(this.Open() == false) {
                    return false;
                }
            }
            //保存ファイルパスを保持
            this.saveFilePath = saveFilePath;
            //保存コマンド投げる
            _rScanClient.SendCommandRequest(
                CommandTypes.PUT,
                RemoteScanCommands.SaveScanData,
                ContentTypes.xml,
                new SaveScanDataParams(saveFilePath)
            );
            return true;
        }
        /// <summary>
        /// スキャン停止要求
        /// </summary>
        public void StopScaning() {
            if((this.IsConnected != true) || (this.Busy_Scanning != true)){
                return;
            }
            _rScanClient.SendCommandRequest(//スキャン停止
                CommandTypes.PUT,
                RemoteScanCommands.ControlScan,
                ContentTypes.xml,
                new ControlScanParams(ScanControl.Stop)
            );
        }
        /// <summary>
        /// コーナー座標の設定
        /// </summary>
        /// <param name="cornerPositionY"></param>
        public bool RequestCornerPosition(double cornerPositionY) {
            //引数で受けっとたY軸コーナー座標を第二軸に適用
            string _startPosY = cornerPositionY.ToString("F3");
            string _val = $",{_startPosY},,,";
            //接続状態に持っていく
            if(this.IsConnected != true) {
                if(this.Open() == false) {
                    return false;
                }
            }
            // パラメータ設定
            var param = new SetRecipePositionParams() {
                MoveSensorMode = MoveSensorMode.Absolute,
                PositionType = PositionTypes.Corner,
                Value = _val, //第１軸から順に「,」区切りで指定する移動しない軸は空欄にする

            };
//            this._logger?.AppendLog(Logger.eLogPriority.INFO,"開始座標(Y軸)設定" + _startPosY + "[mm]");
            // プリセットポジション設定コマンド送信
            _rScanClient.SendCommandRequest(
                CommandTypes.PUT,
                RemoteScanCommands.SetRecipePosition,
                ContentTypes.xml,
                param
                );
            return true;
        }
        /// <summary>
        /// センサ移動
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public bool MoveSensor(double moveX,double moveY,double moveZ1,double moveZ2) {

            this._logger?.Print(LogPrinter.eLogPriority.DEBUG,$"相対移動:X={moveX},Y={moveY},Z1={moveZ1},Z2={moveZ2}");
            var param = new MoveSensorSequenceParams();
            // Z2
            param.Add(new MoveSequenceParams(
                    MoveSensorMode.Absolute,
                    3,//軸番号
                    moveZ2
                    ));
            // Z1
            param.Add(new MoveSequenceParams(
                    MoveSensorMode.Absolute,
                    2,//軸番号
                    moveZ1
                    ));
            // Y
            param.Add(new MoveSequenceParams(
                    MoveSensorMode.Absolute,
                    1,//軸番号
                    moveY
                    ));
//            // X
//            param.Add(new MoveSequenceParams(
//                    MoveSensorMode.Absolute,
//                    0,//軸番号
//                    moveX
//                    ));
            //プローブ移動
            this.moveProbe(param);
            return true;
        }
        /// <summary>
        /// プローブ移動
        /// </summary>
        /// <param name="_param"></param>
        public bool moveProbe(MoveSensorSequenceParams _param) {
            //ステータスを見つつ接続状態に持っていく
            if(this.RequestScanState() == false) {
                return false;
            }
            //ステータス確認
            if(this.checkStatus("プローブ移動エラー") == false) {return false;}

            //プローブ位置移動コマンドを投げる
            this.Busy_ProveMooving=true;
            _rScanClient.SendCommandRequest(
                CommandTypes.PUT,
                RemoteScanCommands.MoveSensorSequence,
                ContentTypes.xml,
                _param
            );
            return true;
        }
    }//class
}//namespace
