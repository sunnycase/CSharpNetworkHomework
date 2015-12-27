using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPScanning
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _ipPrefixText = "192.168.0.";
        public string IPPrefixText
        {
            get { return _ipPrefixText; }
            set
            {
                if (SetProperty(ref _ipPrefixText, value))
                    OnIPChanged();
            }
        }

        private bool _isValid = true;
        public bool IsValid
        {
            get { return _isValid; }
            private set
            {
                if (SetProperty(ref _isValid, value))
                    ScanCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isScanning;
        public bool IsScanning
        {
            get { return _isScanning; }
            private set
            {
                if (SetProperty(ref _isScanning, value))
                    ScanCommand.RaiseCanExecuteChanged();
            }
        }

        private int _startValue;
        public int StartValue
        {
            get { return _startValue; }
            set
            {
                if (SetProperty(ref _startValue, value))
                    OnIPChanged();
            }
        }

        private int _endValue = 255;
        public int EndValue
        {
            get { return _endValue; }
            set
            {
                if (SetProperty(ref _endValue, value))
                    OnIPChanged();
            }
        }

        public RelayCommand ScanCommand { get; }
        public ObservableCollection<string> ScanResult { get; } = new ObservableCollection<string>();

        public MainViewModel()
        {
            ScanCommand = new RelayCommand(OnScanCommand, () => IsValid && !IsScanning);
        }

        private async void OnScanCommand()
        {
            var ips = Enumerable.Range(StartValue, EndValue - StartValue + 1)
                .Select(o => IPAddress.Parse(IPPrefixText + o)).ToList();
            IsScanning = true;
            try
            {
                var scanTasks = ips.Select(o => ScanIP(o));
                await Task.WhenAll(scanTasks);
            }
            finally
            {
                IsScanning = false;
            }
        }

        private async Task ScanIP(IPAddress ip)
        {
            var task = Task.Run(async () =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                var entry = await Dns.GetHostEntryAsync(ip);
                watch.Stop();
                return $"扫描地址：{ip}，扫描用时：{watch.ElapsedMilliseconds}毫秒，主机DNS名称：{entry?.HostName ?? "不在线"}";
            });
            ScanResult.Add(await task);
        }

        private void OnIPChanged()
        {
            IsValid = Regex.IsMatch(IPPrefixText, @"(2[0-4]\d|25[0-5]|[01]?\d\d?\.){3}") &&
                StartValue >= 0 && StartValue <= EndValue &&
                EndValue >= 0 && EndValue <= 255;
        }

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
