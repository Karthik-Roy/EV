using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationBase
{
    #region GroupSection Section  
    public class GroupSection : ConfigurationSection
    {
        [ConfigurationProperty("groups", IsDefaultCollection = true)]
        public GroupElementCollection Groups
        {
            get { return (GroupElementCollection)this["groups"]; }
            set { this["groups"] = value; }
        }
    }
    #endregion
}
