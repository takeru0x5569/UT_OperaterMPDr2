using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UT_OperaterMPDr2 {
    /// <summary>
    /// ucMainContents.xaml の相互作用ロジック
    /// </summary>
    public partial class ucMainContents:UserControl {
        public ObservableCollection<Person> People { get; set; }
        public ucMainContents() {
            InitializeComponent();
            People = new ObservableCollection<Person>
            {
                new Person { Name = "山田 太郎", Age = 30, Address = "東京都" },
                new Person { Name = "鈴木 次郎", Age = 25, Address = "大阪府" },
                new Person { Name = "佐藤 花子", Age = 28, Address = "福岡県" }
            };

            
        }


    }
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }

}
