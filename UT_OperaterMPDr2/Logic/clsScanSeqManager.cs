using ScannerLink; //基底クラス
using UtilityTools; //ロガー
using System.Text.RegularExpressions;//正規表現

//xx  using RemoteControlLibrary;
//xx  using RecipeManager;

namespace UT_OperaterMPDr2 {
    /// <summary>
    /// スキャナのシーケンスを管理するクラス
    /// </summary>
    public class ScanSeqManager:Scanner {
        private readonly int axisIndexOfStage = 4;
//??        /// <summary>
//??        /// レシピ解析用のクラス（DLL参照
//??        /// </summary>
//??        private RecipeClass Recipe = new RecipeClass();
        /// <summary>
        /// パーティションポインタの最小値
        /// </summary>
        private const int PARTITION_MIN = 1;
        /// <summary>
        /// パーティションポインタの最大値
        /// </summary>
        private const int PARTITION_MAX = 3;
        /// <summary>
        /// スキャンデータ保存先ディレクトリのルート
        /// </summary>
        public string CaptureDirectory { get; set; } = "";
        /// <summary>
        /// シーケンス（連続スキャン）実行中フラグ
        /// </summary>
        public bool IsRunningSeqence { get; private set; } = false;
        /// <summary>
        /// レシピへのパス(オモテ面用)
        /// </summary>
        public string RecipePath_Fwd {
            get { return this._recipePath_Fwd; }
            set {
                if(this.IsRunningSeqence==false) {
                    this._recipePath_Fwd =value;
                }
            }//set
        }
        private string _recipePath_Fwd="";
        /// <summary>
        /// レシピへのパス(ウラ面用)
        /// </summary>
        public string RecipePath_Bak {
            get { return this._recipePath_Bak; }
            set {
                if(this.IsRunningSeqence==false) {
                    this._recipePath_Bak =value;
                }
            }//set
        }
        private string _recipePath_Bak="";
        /// <summary>
        /// 裏面検査フラグ
        /// </summary>
        public bool? FaceReverse {
            get { return this._faceReverse; }
            set {
                if(this.IsRunningSeqence==false) {
                    this._faceReverse=value;
                }
            }//set
        }
        private bool? _faceReverse=true;
        /// <summary>
        /// パーティション（Y軸）オフセット量
        /// <summary>
        public double OffsetPartition {
            get { return this._offsetPartition; }
            set {
                if(this.IsRunningSeqence==false) {
                    this._offsetPartition=value;
                }
            }//set
        }
        private double _offsetPartition = 0.0f;
        /// <summary>
        /// シーケンス終了時にステージ上昇させるフラグ
        /// </summary>
        public bool FnishSeq_StageUp {
            get { return this._fnishSeq_StageUp; }
            set {
                if(this.IsRunningSeqence==false) {
                    this._fnishSeq_StageUp=value;
                }
            }//set
        }
        private bool _fnishSeq_StageUp = false;
        /// <summary>
        /// 先頭のパーティション番号(基本的に変更しない)
        /// </summary>
        public int Partition_Head {
            get { return this._prtition_Head; }
            private set {
                if(this.IsRunningSeqence==false) {
                    this._prtition_Head=value;
                }
            }//set
        }
        private int _prtition_Head = PARTITION_MIN;
        /// <summary>
        /// 末尾のパーティション番号（２か３を選択可能）
        /// </summary>
        public int Partition_Tail {
            get { return this._partition_Tail; }
            set {
                if(this.IsRunningSeqence==false) {
                    if(value <= PARTITION_MAX) {
                        if(value > PARTITION_MIN) {
                            this._partition_Tail=value;
                        }
                    }
                }
            }//set
        }
        private int _partition_Tail = PARTITION_MAX;
        /// <summary>
        /// スキャン中のパーティション番号
        /// </summary>
        public int Partition_Pointer { get; set; } = PARTITION_MIN;
        /// <summary>
        /// X軸のコーナー座標(不明はNULLにする)
        /// </summary>
        public double? CornerPositionX{
            get { return this._cornerPositionX ; }
            private set {
                if(this.IsRunningSeqence==false) {
                    this._cornerPositionX =value;
                }
            }//set
        }
        private double? _cornerPositionX = null;
        /// <summary>
        /// Y軸のコーナー座標(不明はNULLにする)
        /// </summary>
        public double? CornerPositionY{get; set;}=null;
        /// <summary>
        /// ステージ上昇時の目標T軸座標
        /// </summary>
        public double PositionAbs_StageUp_AxisT {
            get {
                return this._positionAbs_StageUp_AxisT;
            }//get
            set {
                if(this.IsRunningSeqence==false) {
                    this._positionAbs_StageUp_AxisT=value;
                }
            }//set
        }
        private double _positionAbs_StageUp_AxisT = 0.0f;
        /// <summary>
        /// ステージ下降時の目標T軸座標
        /// </summary>
        public double PositionAbs_StageDown_AxisT {
            get {
                return this._positionAbs_StageDown_AxisT;
            }//get
            set {
                if(this.IsRunningSeqence==false) {
                    this._positionAbs_StageDown_AxisT=value;
                }
            }//set
        }
        private double _positionAbs_StageDown_AxisT = 0.0f;
        /// <summary>
        /// シーケンス終了時に退避実行
        /// </summary>
        public bool IsFinishSeqEscape { set; get; }= true;
        /// <summary>
        /// シーケンス終了時にステージ上昇実行
        /// </summary>
        public bool IsFinishSeqStageUp{ set; get; }= true;
        /// <summary>
        /// 製品個体情報（運用時はバーコード入力する情報識別番号）
        /// </summary>
        public typUniqueInfo[] UniqueInfo = new typUniqueInfo[3]{
            new typUniqueInfo { ChA = "", ChB = "" },
            new typUniqueInfo { ChA = "", ChB = "" },
            new typUniqueInfo { ChA = "", ChB = "" }
        };
        public struct typUniqueInfo{
            public string ChA;
            public string ChB;
        };
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static ScanSeqManager GetInstance() {
            if(_instance == null) {
                _instance = new ScanSeqManager();
            }
            return _instance;
        }
        private static ScanSeqManager? _instance = null;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public ScanSeqManager() {
            //イベントハンドラの登録
            this.OnScanFinish += this.handleScanFinish;
            this.OnDataSaved += this.handleDataSaved;
            this.OnMoveFinish += this.handleMoveFinish;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  bool Open() {
            base.Open();
            base.RequestScanState();
            return true;
        }
        /// <summary>
        ///プローブ退避
        /// </summary>
        public void EscapeProbe() {
            this._logger?.Print(LogPrinter.eLogPriority.INFO,$"プローブ退避");
            //相対移動で大きな移動慮を与えてリミットセンサに当てる
            base.MoveSensor(-175,250,25,25);
        }
        /// <summary>
        ///ステージ上昇 
        /// </summary>
        public void StageUp() {
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"ステージ上昇：目標"+this.PositionAbs_StageUp_AxisT +"[mm]");
            this.moveTableAbs(this.PositionAbs_StageUp_AxisT);
        }
        /// <summary>
        ///ステージ下降
        /// </summary>
        public void StageDown() {
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"ステージ下降：目標："+this.PositionAbs_StageDown_AxisT +"[mm]");
            this.moveTableAbs(this.PositionAbs_StageDown_AxisT);
        }
        /// <summary>
        /// T軸テーブル絶対値座標移動 
        /// </summary>
        private bool moveTableAbs(double absPositionT) {
//??            var param = new MoveSensorSequenceParams();
//??            //軸移動パラメータ生成：第５軸がテーブル
//??            param.Add(new MoveSequenceParams(
//??                    MoveSensorMode.Absolute,
//??                    this.axisIndexOfStage,//軸番号
//??                    absPositionT
//??                    ));
//??            //プローブ移動
//??            return base.moveProbe(param);
            return true;//仮実装
        }

        /// <summary>
        /// レシピファイル読み込み要求
        /// </summary>
        /// <param name="recipeFullPath">レシピファイルへのパス</param>
        public override bool LoadRecipi() {
//?            //オモテ用かウラ用のレシピをフラグに応じて選択してから基底クラスでコマンドを投げる
//?            if(this.FaceReverse== true) {
//?                this.RecipePath = this.RecipePath_Bak;
//?            } else {
//?                this.RecipePath = this.RecipePath_Fwd;
//?            }
//?            //インサイトスキャンにコーナー座標問い合わせはできないのでMPDアプリ側でもレシピを読んでコーナー座標を特定する
//?            Recipe.Import(this.RecipePath);
//?            this.CornerPositionX = Recipe.PresetPositions[0].Corner;
//?            this.CornerPositionY = Recipe.PresetPositions[1].Corner;
            return base.LoadRecipi();
        }
        /// <summary>
        /// シーケンス中フラグを強制でおとす
        /// </summary>
        public void ClearScanBusy() {
//??            DialogResult result = MessageBox.Show("「検査中」を強制クリアします。続行しますか？","確認",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
//??            if(result == DialogResult.Yes) {
//??                this.IsRunningSeqence = false;
//??            }
        }
        /// <summary>
        /// シーケンススキャン開始
        /// </summary>
        /// <returns></returns>
        public bool StartSequence() {
            if(this.FaceReverse == null) {
                //??  MessageBox.Show("検査面を指定してください");
                return false;
            }
            //スキャン中は受け付けない
            if(this.IsRunningSeqence == true) {
                return false;
            }
            //バーコード情報のセットを確認
            bool _ok = true;
            for(int i = this.Partition_Pointer; i <= this.Partition_Tail; i++) {
                string _wornMessage = $"パーティション{i}";
                bool _lostInfo = false;
                if(this.UniqueInfo[i - 1].ChA == "") { _wornMessage += ",ChA"; _lostInfo = true; _ok = false; }
                if(this.UniqueInfo[i - 1].ChB == "") { _wornMessage += ",ChB"; _lostInfo = true; _ok = false; }
                if(_lostInfo){ this._logger?.Print(LogPrinter.eLogPriority.WARNING,"バーコード情報不足" + _wornMessage); }
            }
//??            // メッセージボックスを表示して続行するかどうか確認する
//??            if(_ok == false) {
//??                DialogResult result = Fixer.GetInstance().GetDialog("バーコード情報が不足しています。\r\n続行しますか？","確認", CustomMsgBox.Btn.YesNo, CustomMsgBox.Mark.Warn).ShowDialog();
//??                if((result == DialogResult.No) || (result == DialogResult.Cancel)) {
//??                    return false;
//??                }
//??            }
            //オープンして非常停止などの状態を取得
            //this.Open();
            //レシピを食わす
            if(this.LoadRecipi()==false) {
                return false;
            }
//xx        Application.DoEvents();
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"連続スキャン開始");
            //スキャン中フラグを立てる
            this.IsRunningSeqence = true;
            //
            return this.StartScaning();
        }
        /// <summary>
        /// シーケンススキャン停止
        /// </summary>
        public void StopSequence() {
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"スキャン停止操作を受け付けました");
            this.IsRunningSeqence=false;
            this.StopScaning();
        }
        /// <summary>
        /// スキャン開始要求(コーナー座標をオフセットさせてBaseクラスのスキャン開始を呼ぶ)
        /// </summary>
        public override bool StartScaning() {
//??            if(this.IsEmergency == true) {
//??                MessageBox.Show("緊急停止中です。","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
//??                return false;
//??            }
//??            if(this.IsInterlock == true) {
//??                MessageBox.Show("インターロック中です。","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
//??                return false;
//??            }
            //コーナー座標Yをパーティションオフセット分だけずらす
            double _partitionOffset = (this.CornerPositionY ?? 0.0F) + (this.OffsetPartition*(this.Partition_Pointer-1));
            this._logger?.Print(LogPrinter.eLogPriority.INFO,$"コーナーOffset設定：{_partitionOffset}[mm]");
            base.RequestCornerPosition(_partitionOffset);
            //スキャン開始
            return base.StartScaning();
        }
        /// <summary>
        /// スキャン結果保存要求(指定ディレクトリに時刻名で保存)
        /// </summary>
        public override bool SaveScanData(string saveDirctory,string prifix="") {
            //表裏面を識別する文字を付加
            string _face = (this.FaceReverse==true) ? "B" :"F";
            //パーティションを識別する文字を付加
            string _partition = "P" + this.Partition_Pointer.ToString("D");
            string _prifix = "("+_partition + "_" + _face + ")";
            //スキャンデータ保存(ベースクラスで時刻を元に自動で命名)
            return base.SaveScanData(saveDirctory,_prifix);
        }
        /// <summary>
        /// スキャン完了の後イベントハンドラ
        /// </summary>
        protected override void handleScanFinish() {
            //スキャン中フラグを下ろす
            base.handleScanFinish();
            //【Fix1027】ここがガードが無かったので、例えパーティションポインタがオーバーしていても保存しにいってしまう
            if (this.Partition_Pointer <= this.Partition_Tail) {
                //スキャンデータ保存
                this.SaveScanData(this.CaptureDirectory);
            }
            if(this.IsRunningSeqence == false) {
                return;//中断されているとみなして次へ行かない
            }
            //次の処理（パーティションポインタが末尾だったら終了）
            if (this.Partition_Pointer >= this.Partition_Tail) {
                //プローブ退避したい場合退避投げる
                if(this.IsFinishSeqEscape == true) {
                    this.EscapeProbe();
                } else {
                    //▼シーケンス終了
                    this.IsRunningSeqence = false;
                }
            } else {
                //▼次のシーケンスへ
                //パーティションポインタを進める
                this.Partition_Pointer++;
                this.StartScaning();
            }
        }
        /// <summary>
        /// センサ移動完了後のイベントハンドラ
        /// </summary>
        protected override void handleMoveFinish() {
            base.handleMoveFinish();
            if(this.IsRunningSeqence == true) {
                if(this.IsFinishSeqStageUp == true) {
                    this.StageUp();
                }
            }
            this.IsRunningSeqence = false;
        }
        /// <summary>
        /// データ保存完了の後イベントハンドラ
        /// </summary>
        protected override void handleDataSaved(string saveIsdPath) {
            //基底クラスの処理を先に実行
            base.handleDataSaved(saveIsdPath);
//??            // フォルダのボディを取得する
//??            string _folderBody = Path.GetFileName(saveIsdPath);
//??            //カッコで括られた中の文字を抽出
//??            string _keyWord = Regex.Match(_folderBody,@"\((.*?)\)").Groups[1].Value;
//??            
//??            // バーコード情報.txtのパスを作成
//??            string _barcodeFilePath = Path.Combine(saveIsdPath,"バーコード情報.txt");
//??            // ユニークInfoのインデックスがパーティションポインタと一致する要素のAchBchの値をファイルに書き込む
//??            using(StreamWriter writer = new StreamWriter(_barcodeFilePath)) {
//??                typUniqueInfo _uniqueInfo = this.UniqueInfo[Partition_Pointer - 1];
//??                writer.WriteLine($"ChA: {_uniqueInfo.ChA}");
//??                writer.WriteLine($"ChB: {_uniqueInfo.ChB}");
//??            }
        }
    }//class
}//namespace
