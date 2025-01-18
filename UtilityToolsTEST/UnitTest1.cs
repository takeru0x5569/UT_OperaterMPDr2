using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using UtilityTools;

namespace UtilityToolsTEST {
    [TestClass]

    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1(){
            LogPrinter.Instance.OwnerName = "UnitTest";
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.NOTE,"------------------");
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.FATAL,"Faital");
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.ERROR,"Error");
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.WARNING,"Warning");
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.INFO,"Info");
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.NOTE,"Note");
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.DEBUG,"Debug");
            LogPrinter.Instance.Print(LogPrinter.eLogPriority.NOTE,"------------------");
        }
    }
}