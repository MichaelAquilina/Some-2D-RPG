using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Tiled
{
    public interface IPropertyBag
    {
        Dictionary<string, string> Properties { get; }
    }
}
