using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UT_OperaterMPDr2 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            LoadRootFolders();
        }

        private void LoadRootFolders() {
            // 全てのドライブを取得
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives) {
                if (drive.IsReady) {
                    TreeViewItem driveItem = CreateTreeViewItem(drive.RootDirectory);
                    FolderTreeView.Items.Add(driveItem);
                }
            }
        }

        private TreeViewItem CreateTreeViewItem(DirectoryInfo directoryInfo) {
            TreeViewItem item = new TreeViewItem {
                Header = CreateHeader(directoryInfo.Name, "FolderIcon"),
                Tag = directoryInfo
            };

            // ダミーアイテムを追加して、展開可能に見せる
            item.Items.Add(null);

            item.Expanded += Folder_Expanded;

            return item;
        }

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

                    // .isr と .isd ファイルを追加
                    foreach (var file in directoryInfo.GetFiles("*.isr")) {
                        item.Items.Add(CreateFileTreeViewItem(file));
                    }
                    foreach (var file in directoryInfo.GetFiles("*.isd")) {
                        item.Items.Add(CreateFileTreeViewItem(file));
                    }
                } catch { }
            }
        }

        private TreeViewItem CreateFileTreeViewItem(FileInfo fileInfo) {
            return new TreeViewItem {
                Header = CreateHeader(fileInfo.Name, "FileIcon"),
                Tag = fileInfo
            };
        }

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
    }
}