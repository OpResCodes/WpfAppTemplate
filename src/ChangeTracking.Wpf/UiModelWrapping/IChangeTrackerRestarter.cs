using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracking.Wpf
{
    public interface IChangeTrackerRestarter
    {
        void StopChangeTracking();

        void RestartChangeTracking();
    }
}
