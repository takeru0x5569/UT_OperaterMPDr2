using IkkForm;

namespace Insight.App {
    [TestClass]
    public class UnitTest1 {
        /// <summary>
        /// テスト：
        /// </summary>
        [TestMethod]
        public void Test_MsgBox() {
            MsgBox msgBx;
            MsgBox.eMsgBoxResult res;
            var thread = new Thread(() => {
                MsgBox.eMsgBoxType type;
                //-----------------------------------
                type = MsgBox.eMsgBoxType.Information;
                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","OKしてください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Yes);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","閉じてしてください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Cancel);

                //-----------------------------------
                type = MsgBox.eMsgBoxType.GoCancel;

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","続行を押してください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Yes);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","キャンセルを押してください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.No);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","閉じてください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Cancel);
                //-----------------------------------
                type = MsgBox.eMsgBoxType.YesNo;

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","Yesを押してください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Yes);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","Noを押してください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.No);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","閉じてください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Cancel);

                //-----------------------------------
                type = MsgBox.eMsgBoxType.Exclamation;

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","Yesを押してください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Yes);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","Noを押してください");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.No);

            });
            thread.SetApartmentState(ApartmentState.STA); // STA モードを指定しないをフォーム起動できない
            thread.Start();
            thread.Join();//スレッドが終了するまで待機

        }
    }//calss
}//namespace
