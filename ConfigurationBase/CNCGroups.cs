using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationBase
{
    #region MySection Group  
    public class CNCGroups : ConfigurationSectionGroup
    {
        [ConfigurationProperty("mtsection", IsRequired = false)]
        public GroupSection ContextSettings
        {
            get { return (GroupSection)base.Sections["mtsection"]; }
        }
    }
    #endregion
}
