using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationBase
{

    public class GroupElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("elements", IsDefaultCollection = true)]
        public ParamElementCollection Params
        {
            get { return (ParamElementCollection)this["elements"]; }
            set { this["elements"] = value; }
        }
    }
}
