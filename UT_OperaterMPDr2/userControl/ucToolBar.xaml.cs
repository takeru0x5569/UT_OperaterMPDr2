﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UT_OperaterMPDr2 {
    /// <summary>
    /// ucToolBar.xaml の相互作用ロジック
    /// </summary>
    public partial class ucToolBar:UserControl {
        public ucToolBar() {
            InitializeComponent();
        }
        private void onClick_ConnectButton(object sender,RoutedEventArgs e) {
            bool ret = ScanSeqManager.GetInstance().Open();
        }
        private void onClick_DisConnectButton(object sender,RoutedEventArgs e) {
            ScanSeqManager.GetInstance().Close();
        }
    }//class
}//namespace
