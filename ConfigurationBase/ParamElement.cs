using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationBase
{
    #region Param Element  
    public class ParamElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("paramSyntax", DefaultValue = "", IsRequired = true)]
        public string ParamSyntax
        {
            get { return (string)this["paramSyntax"]; }
            set { this["paramSyntax"] = value; }
        }

        [ConfigurationProperty("convertion", DefaultValue = "", IsRequired = false)]
        public string Convertion
        {
            get { return (string)this["convertion"]; }
            set { this["convertion"] = value; }
        }
    }
    #endregion

}
