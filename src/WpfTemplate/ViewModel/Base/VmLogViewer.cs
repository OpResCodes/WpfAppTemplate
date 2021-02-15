using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using NLog;
using NLog.Targets;

namespace WpfTemplate.ViewModel.Base
{
    public class VmLogViewer : VmBase
    {
        public MemoryEventTarget LoggingTarget;

        public ObservableCollection<LogEventInfo> LogCollection { get; } 

        public VmLogViewer()
        {
            LoggingTarget = new MemoryEventTarget();
            LogCollection = new ObservableCollection<LogEventInfo>();
            LoggingTarget.EventReceived += EventReceived;
            //add me to NLog targets
            var config = NLog.LogManager.Configuration;
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, LoggingTarget);
            LogManager.ReconfigExistingLoggers();
            LogManager.GetCurrentClassLogger().Debug("VmLogViewer ready.");
        }

        private void EventReceived(LogEventInfo obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                if (LogCollection.Count >= 150) LogCollection.RemoveAt(LogCollection.Count - 1);
                LogCollection.Insert(0,obj);
            });
        }


        [Target("MemoryEventTarget")]
        public class MemoryEventTarget : NLog.Targets.Target
        {
            public event Action<NLog.LogEventInfo> EventReceived;

            /// <summary>
            /// Notifies listeners about new event
            /// </summary>
            /// <param name="logEvent">The logging event.</param>
            protected override void Write(NLog.LogEventInfo logEvent)
            {
                EventReceived?.Invoke(logEvent);
            }
        }
    }
}
