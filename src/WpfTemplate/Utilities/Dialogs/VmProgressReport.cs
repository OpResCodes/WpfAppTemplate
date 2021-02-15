using GalaSoft.MvvmLight.Command;
using System;
using System.Threading;
using GalaSoft.MvvmLight;
using WpfTemplate.ViewModel;

namespace WpfTemplate.Dialogs
{
    internal class VmProgressReport : ObservableObject, IChildWindowHelper
    {
        private readonly CancellationTokenSource _cts;
        private readonly Progress<double> _progress;

        public VmProgressReport(Progress<double> progressReport, CancellationTokenSource cancellationTokenSource = null)
        {
            _cts = cancellationTokenSource;
            _progress = progressReport;

            ShowCancelButton = (_cts != null);

            progressReport.ProgressChanged += ProgressReport_ProgressChanged;

            CancelCommand = new RelayCommand(() =>
            {
                HeadlineText = "Cancelling...";
                if (_cts != null)
                {
                    _cts.Cancel();
                    CloseDialog(false);
                }
            }, () => !(_cts == null));
        }
        
        private bool _ShowCancelButton = false;
        public bool ShowCancelButton
        {
            get { return _ShowCancelButton; }
            set { Set(() => ShowCancelButton, ref _ShowCancelButton, value); }
        }

        private void ProgressReport_ProgressChanged(object sender, double e)
        {
            this.CurrentProgress = e;
        }

        public RelayCommand CancelCommand { get; set; }

        private string _HeadlineText = "Work in Progress...";
        public string HeadlineText
        {
            get { return _HeadlineText; }
            set { Set(() => HeadlineText, ref _HeadlineText, value); }
        }

        private double _CurrentProgress = 0.0;
        public double CurrentProgress
        {
            get { return _CurrentProgress; }
            set
            {
                Set(() => CurrentProgress, ref _CurrentProgress, value);
                if (_CurrentProgress >= 1)
                    CloseDialog(true);
            }
        }

        public event EventHandler<RequestCloseEventArgs> RequestCloseDialog;

        public void CloseDialog(bool withResult = false)
        {
            _progress.ProgressChanged -= ProgressReport_ProgressChanged;
            RequestCloseDialog?.Invoke(this, new RequestCloseEventArgs(withResult));
        }

        public void WindowRequestsClose(object sender, RequestCloseEventArgs e)
        {
            CloseDialog(e.DialogResult);
        }
    }
}
