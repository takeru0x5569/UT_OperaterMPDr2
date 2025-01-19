using ScannerLink;
using UtilityTools;
namespace ScannerLinkTEST {
    [TestClass]
    public class TestScannerLink {
        [TestMethod]
        public void ê⁄ë±ÉeÉXÉg() {
            Scanner scn= new Scanner();
            scn.AtouchLogger(new LogPrinter("UnitTest"));
            
            scn.Open();
            Thread.Sleep(1000);
            scn.InitOrigin();

            Thread.Sleep(2000);
            scn.Close();
        }

    }//class
}//namespace