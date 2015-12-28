using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Client.FileServiceReference;

namespace Client
{
    class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FileInfo> Files { get; } = new ObservableCollection<FileInfo>();
        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        private FileInfo _selectedFile;
        public FileInfo SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (SetProperty(ref _selectedFile, value))
                    DownloadCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand DownloadCommand { get; }

        private readonly FileServiceClient _fileServiceClient;

        public MainViewModel()
        {
            DownloadCommand = new RelayCommand(OnDownloadCommand, () => SelectedFile != null);
            _fileServiceClient = new FileServiceClient();
            LoadData();
        }

        private async void LoadData()
        {
            Files.Clear();
            foreach (var file in await _fileServiceClient.GetFilesAsync())
                Files.Add(file);
        }

        private async void OnDownloadCommand()
        {
            var file = SelectedFile;
            Logs.Add($"正在下载 {file.FileName}");
            var outputFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file.FileName);
            using (var ws = new System.IO.FileStream(outputFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var remoteFile = await _fileServiceClient.DownloadAsync(file.FileName);
                await remoteFile.CopyToAsync(ws);
            }
            Logs.Add($"下载完成，共下载{file.Length}字节，保存至{outputFileName}。");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T property, T value, [CallerMemberName]string propertyName = null)
        {
            if (!object.Equals(property, value))
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
    }
}
