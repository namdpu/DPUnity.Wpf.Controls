namespace DPUnity.Wpf.Controls.Controls.FileDialogs
{
    public class FileDialog
    {
        public static string? PickFile(string filter = "All files (*.*)|*.*", string initialPath = "")
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a file",
                Filter = filter,
                InitialDirectory = initialPath
            };
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        public static List<string>? PickMultiFile(string filter = "All files (*.*)|*.*", string initialPath = "")
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select files",
                Filter = filter,
                InitialDirectory = initialPath,
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                return [.. openFileDialog.FileNames];
            }
            return null;
        }

        public static string? PickFolder(string initialPath = "")
        {
#if NET8_0_OR_GREATER
            // Sử dụng OpenFolderDialog cho .NET 8.0 trở lên
            var folderDialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Select a folder",
                InitialDirectory = initialPath
            };
            if (folderDialog.ShowDialog() == true)
            {
                return folderDialog.FolderName;
            }
#else
            // Sử dụng FolderPicker cho .NET Framework 4.7.2 và các phiên bản thấp hơn
            var folderPicker = new FolderPicker
            {
                InputPath = initialPath,
                Title = "Select a folder",
                ForceFileSystem = true
            };
            if (folderPicker.ShowDialog() == true)
            {
                return folderPicker.ResultPath;
            }
#endif
            return null;
        }

        public static string SaveFile(string filter = "All files (*.*)|*.*", string initialPath = "", string defaultFileName = "new_file")
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save file",
                Filter = filter,
                InitialDirectory = initialPath,
                FileName = defaultFileName
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            return string.Empty;
        }
    }
}
