using IkkForm;

namespace Insight.App {
    [TestClass]
    public class UnitTest1 {
        /// <summary>
        /// �e�X�g�F
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
                res = msgBx.ShowMessageDialog($"Type={type}","OK���Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Yes);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","���Ă��Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Cancel);

                //-----------------------------------
                type = MsgBox.eMsgBoxType.GoCancel;

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","���s�������Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Yes);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","�L�����Z���������Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.No);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","���Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Cancel);
                //-----------------------------------
                type = MsgBox.eMsgBoxType.YesNo;

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","Yes�������Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Yes);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","No�������Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.No);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","���Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Cancel);

                //-----------------------------------
                type = MsgBox.eMsgBoxType.Exclamation;

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","Yes�������Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.Yes);

                msgBx = new MsgBox(type);
                res = msgBx.ShowMessageDialog($"Type={type}","No�������Ă�������");
                Assert.AreEqual(res,MsgBox.eMsgBoxResult.No);

            });
            thread.SetApartmentState(ApartmentState.STA); // STA ���[�h���w�肵�Ȃ����t�H�[���N���ł��Ȃ�
            thread.Start();
            thread.Join();//�X���b�h���I������܂őҋ@

        }
    }//calss
}//namespace
