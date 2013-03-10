using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Interfaces
{
    //Represents an Object that allows for meta-data to be stored in a dictionary
    public interface IPropertyBag
    {
        Dictionary<string, string> Properties { get; }
    }
}
