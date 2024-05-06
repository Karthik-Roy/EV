using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationBase
{

    #region GroupElement Collection    
    [ConfigurationCollection(typeof(GroupElement), AddItemName = "group")]
    public class GroupElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new GroupElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((GroupElement)element).Name;
        }

    }
    #endregion
}
