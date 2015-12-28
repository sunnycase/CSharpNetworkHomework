using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ParallelMatrixCompute
{
    class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> ComputeResult { get; } = new ObservableCollection<string>();

        private bool _isComputing;
        public bool IsComputing
        {
            get { return _isComputing; }
            private set
            {
                if (SetProperty(ref _isComputing, value))
                    ComputeCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand ComputeCommand { get; }
        private readonly Random _rand = new Random();

        public MainViewModel()
        {
            ComputeCommand = new RelayCommand(OnComputeCommand, () => !IsComputing);
        }

        private async void OnComputeCommand()
        {
            IsComputing = true;
            try
            {
                ComputeResult.Clear();
                await DoComputeMatrix(1, 200, 18, 27);
                await DoComputeMatrix(2, 2000, 180, 270);
                await DoComputeMatrix(3, 2000, 200, 300);
            }
            finally
            {
                IsComputing = false;
            }
        }

        private async Task DoComputeMatrix(int id, int m, int p, int n)
        {
            var message = await Task.Run(() =>
            {
                var mat1 = FillMatrix(m, p);
                var mat2 = FillMatrix(p, n);
                var mat3 = new float[m, n];
                Action<int> kernel = i =>
                {
                    for (int j = 0; j < n; j++)
                    {
                        float value = 0.0f;
                        for (int cntP = 0; cntP < p; cntP++)
                            value += mat1[i, cntP] * mat2[cntP, j];
                        mat3[i, j] = value;
                    }
                };
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Parallel.For(0, m, kernel);
                watch.Stop();
                return $"测试{id}（矩阵1：{m} x {p}，矩阵2：{p} x {n}），用时：{watch.ElapsedMilliseconds}毫秒。";
            });
            ComputeResult.Add(message);
        }

        private float[,] FillMatrix(int m, int n)
        {
            var mat = new float[m, n];
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    mat[i, j] = (float)_rand.NextDouble();
            return mat;
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
