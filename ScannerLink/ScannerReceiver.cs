using RemoteControlLibrary;
using System;
using System.Diagnostics;
using UtilityTools;

namespace ScannerLink {
    /// <summary>
    /// スキャナ通信クラス：
    /// </summary>
    public partial class Scanner {
        private bool _respWaitFlag=false;//応答待ちフラグ

        /// <summary>
        /// 電文受信時ハンドラ
        /// </summary>
        private void EvHGetRx(object? sender,RemoteCommandRequest comReq) {
            try {
                switch(RemoteScanHelper.CommandType(comReq)) {
                    //▼レスポンス電文受信
                    case CommandTypes.RES:
                        ProcRes(comReq);
                        break;
                    //▼電文受信：非同期応答
                    case CommandTypes.NTF:
                        ProcAsyncRes(comReq);
                        break;
                    default:
                        break;
                }
            } catch(Exception e) {
                Debug.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// レスポンス電文受信時処理
        /// </summary>
        /// <param name="comReq"></param>
        private void ProcRes(RemoteCommandRequest comReq) {
            //レスポンスコード
            int _responseCode = RemoteScanHelper.ResponseCode(comReq);

            switch(RemoteScanHelper.ScanCommand(comReq)) {
                //▼センサ初期化
                case RemoteScanCommands.InitSensor:
                    switch(_responseCode) {
                        case RemoteUtility.RC_SUCCESS:
                            _logger?.Print(LogPrinter.eLogPriority.INFO,"初期化要求が受理されました");
                            this.Busy_Initialize = true;
                            break;
                        case RemoteUtility.RC_ERROR:
                        default:
                            _logger?.Print(LogPrinter.eLogPriority.ERROR,"初期化要求が受理されませんでした");
                            this.Busy_Initialize = false;
                            break;
                    }
                    break;
                //▼レシピロード
                case RemoteScanCommands.LoadScanRecipe:
                    if(_responseCode != RemoteUtility.RC_SUCCESS) {
                        _logger?.Print(LogPrinter.eLogPriority.ERROR,"レシピロード失敗");
                        this.IsRecipeLoaded=false;
                    } else {
                        _logger?.Print(LogPrinter.eLogPriority.INFO,"レシピロード成功");
                        this.IsRecipeLoaded=true;
                    }
                    break;
                case RemoteScanCommands.SetRecipePosition:
                    break;
                //▼スキャンコントロール
                case RemoteScanCommands.ControlScan:
                    break;
                //▼スキャンデータ保存
                case RemoteScanCommands.SaveScanData:
                    if(_responseCode != RemoteUtility.RC_SUCCESS) {
                        _logger?.Print(LogPrinter.eLogPriority.ERROR,"データ保存失敗");
//xx                        Fixer.GetInstance().GetDialog("スキャンデータの保存に失敗しました","データ保存失敗", CustomMsgBox.Btn.Ok, CustomMsgBox.Mark.Err).ShowDialog();
                    } else {
                        _logger?.Print(LogPrinter.eLogPriority.INFO,"保存成功:" + this.saveFilePath);
                        //データ保存完了イベントを投げる
                        this.OnDataSaved?.Invoke(this.saveFilePath);
                    }
                    break;
                case RemoteScanCommands.MoveSensor:
                    break;
                //▼スキャナの状態取得
                case RemoteScanCommands.RequestScanState:
                    var param = (RequestScanStateParams)
                                 RCParamsSerializer.LoadFromStream(
                                     ContentTypes.xml,
                                     comReq.Content,
                                     typeof(RequestScanStateParams)
                                 );
                    this.IsEmergency = param.ReportState.Emergency;
                    this.IsInterlock = param.ReportState.Interlock;
                    this.IsScanReady = param.ScanReady;
                    if(param.ScanReady == false) {
                        _logger?.Print(LogPrinter.eLogPriority.WARNING,"インサイトスキャンNotReady");
//                        MessageBox.Show("InsightScanが準備できていません","InsightScan not ready",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    break;
                case RemoteScanCommands.SetScanArea:
                    break;
                case RemoteScanCommands.SetScanSpeed:
                    break;
                case RemoteScanCommands.SetAScopeArea:
                    break;
                case RemoteScanCommands.SetGateArea:
                    break;
                case RemoteScanCommands.SetPulsarReceiver:
                    break;
                //▼センサ移動
                case RemoteScanCommands.MoveSensorSequence:
                    if(_responseCode != RemoteUtility.RC_SUCCESS) {
                        _logger?.Print(LogPrinter.eLogPriority.ERROR,"センサ移動開始できませんでした");
                    } else {
                        _logger?.Print(LogPrinter.eLogPriority.INFO,"センサ移動開始");
                        this.Busy_ProveMooving = true;//移動中フラグを立てる
                    }
                    break;
                case RemoteScanCommands.GetGateData:
                    GetGateDataResult? dResult = null;
                    if(RemoteScanHelper.ContentType(comReq)
                                        == ContentTypes.xml) {
                        dResult = (GetGateDataResult)
                                  RCParamsSerializer.LoadFromStream(
                                      ContentTypes.xml,
                                      comReq.Content,
                                      typeof(GetGateDataResult)
                                  );
                    }
                    break;
                case RemoteScanCommands.LoadPalette:
                    break;
                case RemoteScanCommands.Lock:
                    break;
                case RemoteScanCommands.SavePalette:
                    break;
                case RemoteScanCommands.GetGateArea:
                    GetGateAreaResult? aResult = null;
                    if(RemoteScanHelper.ContentType(comReq)
                                        == ContentTypes.xml) {
                        aResult = (GetGateAreaResult)
                                  RCParamsSerializer.LoadFromStream(
                                      ContentTypes.xml,
                                      comReq.Content,
                                      typeof(GetGateAreaResult)
                                  );
                    }
                    break;
                case RemoteScanCommands.GetAscopeImage:
                    GetAScopeImageResult? iResult = null;
                    if(RemoteScanHelper.ContentType(comReq)
                                        == ContentTypes.xml) {
                        iResult = (GetAScopeImageResult)
                                  RCParamsSerializer.LoadFromStream(
                                      ContentTypes.xml,
                                      comReq.Content,
                                      typeof(GetAScopeImageResult)
                                  );
                    }
                    break;
                default:
                    break;
            }
            this._respWaitFlag = true;
        }
        /// <summary>
        /// 電文受信：非同期応答時処理
        /// </summary>
        /// <param name="comReq"></param>
        private void ProcAsyncRes(RemoteCommandRequest comReq) {
            var notify = RemoteScanHelper.NotifyCommand(comReq);
            switch(notify) {
                //▽非同期応答：完了
                case RemoteScanNotifyCommands.CompleteAsync:
                    if(RemoteScanHelper.ContentType(comReq) == ContentTypes.xml) {
                        //パラメータ取得
                        var notifyParams =
                            (NotifyCompleteAsyncParams)
                            RCParamsSerializer.LoadFromStream(
                                ContentTypes.xml,
                                comReq.Content,
                                typeof(NotifyCompleteAsyncParams)
                            );
                        //非同期応答の種類による処理分岐
                        switch(notifyParams.AsyncState) {
                            //▼センサ初期化完了
                            case AsyncStates.InitSensor:
                                this.Busy_Initialize = false;
                                _logger?.Print(LogPrinter.eLogPriority.INFO,"初期化完了しました");
                                break;
                            //▼スキャン完了
                            case AsyncStates.Scan:
                                this._logger?.Print(LogPrinter.eLogPriority.INFO,"スキャン完了しました");
                                this.OnScanFinish?.Invoke();//完了イベントを投げる
                                break;
                            //▼センサ移動完了
                            case AsyncStates.MoveSensor:
                                _logger?.Print(LogPrinter.eLogPriority.INFO,"軸移動完了しました");
                                this.OnMoveFinish?.Invoke();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                //▽非同期応答：通知
                case RemoteScanNotifyCommands.Report:
                    if(RemoteScanHelper.ContentType(comReq) == ContentTypes.xml) {
                        var reportParams =
                            (NotifyReportParams)
                            RCParamsSerializer.LoadFromStream(
                                ContentTypes.xml,
                                comReq.Content,
                                typeof(NotifyReportParams)
                            );
                        switch(reportParams.ReportState) {
                            //▼非常停止検出
                            case ReportStates.Emergemcy:
                                this.IsEmergency = true;
                                _logger?.Print(LogPrinter.eLogPriority.ERROR,"非常停止を検出しました");
                                break;
                            //▼非常停止解除
                            case ReportStates.ReleaseEmergemcy:
                                this.IsEmergency = false;
                                _logger?.Print(LogPrinter.eLogPriority.INFO,"非常停止を解除されました");
                                break;
                            //▼インターロック検出
                            case ReportStates.Interlock:
                                _logger?.Print(LogPrinter.eLogPriority.ERROR,"インターロック検出しました");
                                this.IsInterlock = true;
                                break;
                            //▼インターロック解除
                            case ReportStates.ReleaseInterlock:
                                this.IsInterlock = false;
                                _logger?.Print(LogPrinter.eLogPriority.INFO,"インターロックが解除されました");
                                break;
                            //▼モーターエラー
                            case ReportStates.MotorError:
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case RemoteScanNotifyCommands.Logger:
                    // No process
                    break;
                default:
                    break;
            }//switch
        }//function
    }//class
}//namespace
