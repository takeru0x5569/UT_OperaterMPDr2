using System;
using System.Collections.Generic;
using System.IO;
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
    /// ucSidebar.xaml の相互作用ロジック
    /// </summary>
    public partial class ucSidebar:UserControl {
        public ucSidebar() {
            InitializeComponent();
            LoadRootFolders();
        }
       /// <summary>
        /// 全てのドライブを取得
        /// </summary>
        private void LoadRootFolders() {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives) {
                if (drive.IsReady) {
                    TreeViewItem driveItem = CreateTreeViewItem(drive.RootDirectory);
                    FolderTreeView.Items.Add(driveItem);
                }
            }
        }
        /// <summary>
        /// フォルダのTreeViewItemを作成
        /// </summary>
        private TreeViewItem CreateTreeViewItem(DirectoryInfo directoryInfo) {
            TreeViewItem item = new TreeViewItem {
                Header = CreateHeader(directoryInfo.Name, "IconFolder"),
                Tag = directoryInfo
            };
            // ダミーアイテムを追加して、展開可能に見せる
            item.Items.Add(null);
            item.Expanded += Folder_Expanded;
            return item;
        }

        /// <summary>
        /// フォルダが展開されたときの処理
        /// </summary>
        private void Folder_Expanded(object sender, RoutedEventArgs e) {
            TreeViewItem item = (TreeViewItem)sender;
            // ダミーアイテムを削除
            if (item.Items.Count == 1 && item.Items[0] == null) {
                item.Items.Clear();
                DirectoryInfo directoryInfo = (DirectoryInfo)item.Tag;
                try {
                    // サブディレクトリを追加
                    foreach (var dir in directoryInfo.GetDirectories()) {
                        item.Items.Add(CreateTreeViewItem(dir));
                    }
                    //.isrファイルを追加
                    foreach (var file in directoryInfo.GetFiles("*.isr")) {
                        item.Items.Add(CreateFileTreeViewItem(file));
                    }
                    //isd ファイルを追加
                    foreach (var file in directoryInfo.GetFiles("*.isd")) {
                        item.Items.Add(CreateFileTreeViewItem(file));
                    }
                } catch { }
            }
        }
        /// <summary>
        /// ファイルのTreeViewItemを作成
        /// </summary>
        private TreeViewItem CreateFileTreeViewItem(FileInfo fileInfo) {
            return new TreeViewItem {
                Header = CreateHeader(fileInfo.Name, "IconFolder"),
                Tag = fileInfo
            };
        }
        /// <summary>
        /// ヘッダーを作成
        /// </summary>
        private StackPanel CreateHeader(string text, string iconKey) {
            StackPanel stack = new StackPanel {
                Orientation = Orientation.Horizontal
            };
            Image image = new Image {
                Source = (BitmapImage)FindResource(iconKey),
                Width = 16,
                Height = 16,
                Margin = new Thickness(0, 0, 5, 0)
            };
            TextBlock textBlock = new TextBlock {
                Text = text
            };
            stack.Children.Add(image);
            stack.Children.Add(textBlock);
            return stack;
        }

        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            // 選択されたフォルダまたはファイルに対する処理をここに追加
        }
    }//class
}//namespace

