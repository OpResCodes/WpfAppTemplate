using GalaSoft.MvvmLight;

namespace WpfTemplate.ViewModel.Base
{
    public class SimpleDataPoint : ObservableObject
    {
        public SimpleDataPoint(string category, double value)
        {
            CategoryLabel = category;
            PointValue = value;
        }

        private string _categoryLabel = string.Empty;
        public string CategoryLabel
        {
            get => _categoryLabel;
            set { Set(() => CategoryLabel, ref _categoryLabel, value); }
        }

        private double _pointValue = 0;
        public double PointValue
        {
            get => _pointValue;
            set { Set(() => PointValue, ref _pointValue, value); }
        }
    }
}
