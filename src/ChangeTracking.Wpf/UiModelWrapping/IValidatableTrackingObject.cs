using System.ComponentModel;

namespace ChangeTracking.Wpf
{
    public interface IValidatableTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
    {
        bool IsValid { get; }

    }
}
