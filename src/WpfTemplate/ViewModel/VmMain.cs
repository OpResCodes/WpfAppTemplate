using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using NLog;
using WpfTemplate.Dialogs;
using WpfTemplate.ViewModel.Base;

namespace WpfTemplate.ViewModel
{
    public class VmMain : VmBase
    {
        private readonly IDialogService _dialogService;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public VmMain(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ShowPopupCommand = new RelayCommand(OnShowPopup);
            ShowProgressPopup = new RelayCommand(async () => await OnTestProgress());
            MakeErrorCommand = new RelayCommand(async () => await OnMakeError());
            TestBusyOverlayCommand = new RelayCommand(async () => await OnBusyTest());
            TestBusyOverlayWithProgressCommand = new RelayCommand(async () => await OnBusyTestWithProgress());

            _title = "This is a title in the Main Viewmodel";

            SimpleChart = new VmSimpleChart()
            {
                ChartTitle = "Example Chart",
                XAxisTitle = "Some x value",
                YAxisTitle = "Some Y Values"
            };
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                SimpleChart.DataPoints.Add(new SimpleDataPoint($"Day {i + 1:N0}", r.NextDouble() * 100));
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private VmSimpleChart _simpleChart;
        public VmSimpleChart SimpleChart
        {
            get => _simpleChart;
            set => Set(ref _simpleChart, value);
        }

        public ICommand ShowPopupCommand { get; }

        public ICommand ShowProgressPopup { get; }

        public ICommand MakeErrorCommand { get; }

        public ICommand TestBusyOverlayCommand { get; }

        public ICommand TestBusyOverlayWithProgressCommand { get; }

        private async Task OnBusyTest()
        {
            _logger.Info("Running task with busy flag.");
            try
            {
                IsBusy = true;
                await Task.Run(async () =>
                {
                    int i = 10;
                    while (i > 0)
                    {
                        await Task.Delay(500);
                        i--;
                    }
                });
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            finally
            {
                IsBusy = false;
                _logger.Info("Task completed.");
            }
        }

        private async Task OnBusyTestWithProgress()
        {
            _logger.Info("Running task with busy flag.");
            Progress<double> progRep = new Progress<double>();
            this.HideProgressWhenBusy = false;
            this.CurrentProcessProgress = 0;
            progRep.ProgressChanged += (s, d) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => this.CurrentProcessProgress = d*100 );
                _logger.Trace("Progress: " + d);
            };

            try
            {
                IsBusy = true;
                await Task.Run(async () =>
                {
                    int i = 10;
                    while (i > 0)
                    {
                        await Task.Delay(500);
                        i--;
                        ((IProgress<double>)progRep).Report((double)(10 - i) / 10d);
                    }
                });
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            finally
            {
                IsBusy = false;
                _logger.Info("Task completed.");
                this.HideProgressWhenBusy = true;
            }
        }

        private void ProgRep_ProgressChanged(object sender, double e)
        {
            throw new NotImplementedException();
        }

        private async Task OnMakeError()
        {
            //default handling without trycatch:
            await Task.Delay(1000);
            throw new InvalidOperationException("Something went really wrong.");

            //with trycatch:

            //try
            //{
            //    IsBusy = true;
            //    await Task.Delay(1000);
            //    throw new InvalidOperationException("Something went really wrong.");
            //}
            //catch (Exception ex)
            //{
            //    HandleException(ex);
            //}
            //finally
            //{
            //    IsBusy = false;
            //}

        }

        private void OnShowPopup()
        {
            _dialogService.ShowUserPopup("This is my user popup.", "Hello, if you can read this, then you clicked the button before.");
        }

        private async Task OnTestProgress()
        {
            if (!DialogService.ShowConfirmationDialog("Should we really do this?", "Please confirm"))
                return;

            IsBusy = true;
            ProgressHandling progHandling = DialogService.ShowProgressPopup("Some Fake work", "Progress of the fake work");
            try
            {
                await FakeWork(progHandling.CancellationTokenSource.Token, progHandling.ProgressReport);
                DialogService.ShowUserPopup("Success", "Operation finished successfully.");
            }
            catch (OperationCanceledException)
            {
                DialogService.ShowUserPopup("Cancelled", "Operation was cancelled by the user.");
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task FakeWork(CancellationToken cancelToken, IProgress<double> progress)
        {
            double i = 0;
            double max = 100;
            while (i < max)
            {
                await Task.Delay(50);
                if (i % 5 == 0)
                    cancelToken.ThrowIfCancellationRequested();
                i++;
                progress.Report(i / max);
            }
        }

    }
}
