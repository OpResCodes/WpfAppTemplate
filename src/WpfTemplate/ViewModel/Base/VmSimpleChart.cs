using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using WpfTemplate.Utilities;

namespace WpfTemplate.ViewModel.Base
{
    public class VmSimpleChart : ObservableObject
    {
        public ItemsSourceCollection<SimpleDataPoint> DataPoints { get; }

        public VmSimpleChart()
        {
            DataPoints = new ItemsSourceCollection<SimpleDataPoint>();
        }

        private string _title;

        public string ChartTitle
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _yAxisTitle;

        public string YAxisTitle
        {
            get => _yAxisTitle;
            set => Set(ref _yAxisTitle, value);
        }

        private string _xAxistitle;

        public string XAxisTitle
        {
            get => _xAxistitle;
            set => Set(ref _xAxistitle, value);
        }

    }
}
