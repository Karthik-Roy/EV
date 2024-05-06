using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationBase
{
    #region ParamElement Collection    
    [ConfigurationCollection(typeof(ParamElement), AddItemName = "param")]
    public class ParamElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ParamElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParamElement)element).Name;
        }

    }
    #endregion
}
