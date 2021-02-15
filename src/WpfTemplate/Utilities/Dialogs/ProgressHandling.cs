using System;
using System.Threading;

namespace WpfTemplate.Dialogs
{
    public class ProgressHandling
    {
        public ProgressHandling()
        {
            ProgressReport = new Progress<double>();
            CancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationTokenSource CancellationTokenSource { get; }

        public Progress<double> ProgressReport { get;  }

    }
}
