using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfTemplate.Dialogs;
using WpfTemplate.ViewModel;
using WpfTemplate.ViewModel.Base;

namespace WpfTemplate.Design
{
    public class DesignData : VmBase
    {

        public DesignData()
        {
            if(IsInDesignMode)
                Initialize();
        }

        private void Initialize()
        {
            SimpleChart = new VmSimpleChart()
            {
                ChartTitle = "Example Chart",
                XAxisTitle = "Some x value",
                YAxisTitle = "Some Y Values"
            };
            Random r = new Random();
            for (int i = 0; i < 12; i++)
            {
                SimpleChart.DataPoints.Add(new SimpleDataPoint($"Day {i + 1:N0}", r.NextDouble() * 100));
            }

            VmError = new VmErrorPopup("Design Error",
                @"Something went really wrong.
-------
   at WpfTemplate.ViewModel.VmMain.OnMakeError() in D:\Matthes\Source\Local\WpfTemplate\WpfTemplate\ViewModel\VmMain.cs:line 120
   at WpfTemplate.ViewModel.VmMain.<.ctor>b__2_1() in D:\Matthes\Source\Local\WpfTemplate\WpfTemplate\ViewModel\VmMain.cs:line 22
   at System.Threading.Tasks.Task.<>c.<ThrowAsync>b__140_0(Object state)
   at System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)
   at System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, Delegate catchHandler)
   at WpfTemplate.ViewModel.VmMain.OnMakeError() in D:\Matthes\Source\Local\WpfTemplate\WpfTemplate\ViewModel\VmMain.cs:line 120
   at WpfTemplate.ViewModel.VmMain.<.ctor>b__2_1() in D:\Matthes\Source\Local\WpfTemplate\WpfTemplate\ViewModel\VmMain.cs:line 22
   at System.Threading.Tasks.Task.<>c.<ThrowAsync>b__140_0(Object state)
   at System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)
   at System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, Delegate catchHandler)");
        }

        private VmSimpleChart _simpleChart;

        public VmSimpleChart SimpleChart
        {
            get => _simpleChart;
            set => Set(ref _simpleChart, value);
        }

        public VmErrorPopup VmError { get; private set; }

    }
}
