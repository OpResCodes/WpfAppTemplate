using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExtras
{
    public interface ILookupItem
    {
        int Id { get; set; }
        string Label { get; set; }
    }
}
