using UtilityTools;

namespace ScannerLink {
    public partial class Scanner {
        /// <summary>
        /// 接続確立時ハンドラ
        /// </summary>
        private void EvHconScan(object? sender,EventArgs e) {
            this.IsConnected= true;
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"接続成功");
        }
        /// <summary>
        /// 接続ロスト時ハンドラ
        /// </summary>
        private void EvHdisconScan(object? sender,EventArgs e) {
            this.IsConnected= false;
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"切断しました");
            this.Close();
        }
        /// <summary>
        /// 接続エラー時ハンドラ
        /// </summary>
        private void EvHerrScan(object? sender,EventArgs e) {
            this.IsConnected= false;
            this._logger?.Print(LogPrinter.eLogPriority.ERROR,"TCP接続エラーを検出しました");
            this.Close();
        }
        /// <summary>
        /// スキャン完了の後イベントハンドラ
        /// </summary>
        protected  virtual void handleScanFinish() {
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"Scan終了");
            this.Busy_Scanning = false;
        }
        /// <summary>
        /// センサ移動完了の後イベントハンドラ
        /// </summary>
        protected  virtual void handleMoveFinish() {
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"移動終了");
            this.Busy_ProveMooving= false;
        }
        /// <summary>
        /// データ保存完了の後イベントハンドラ
        /// </summary>
        protected virtual void handleDataSaved(string saveIsdPath) {
            this._logger?.Print(LogPrinter.eLogPriority.INFO,"データ保存完了:"+saveIsdPath);
            //継承先にでメタ情報の付加などの処理を行う
        }
    }//class
}//namespace
