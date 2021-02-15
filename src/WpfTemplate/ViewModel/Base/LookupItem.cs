using GalaSoft.MvvmLight;
using WpfExtras;

namespace WpfTemplate.ViewModel
{
    public class LookupItem : ObservableObject, ILookupItem
    {

        private int _id = 0;
        public int Id
        {
            get { return _id; }
            set { Set(() => Id, ref _id, value); }
        }
        
        private string _label = string.Empty;
        public string Label
        {
            get { return _label; }
            set { Set(() => Label, ref _label, value); }
        }
    }
}
