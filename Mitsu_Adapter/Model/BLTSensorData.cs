using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOPS.Mitsu_Adapter
{
    public class BLTSensorData
    {
        //public int sensor_hole_ok { get; set; }
        public int sensor_ng { get; set; }
        //public int stud_hole_cont_ok { get; set; }
        public int stud_hole_cont_ng { get; set; }
        //public int oil_hole_cont_ok { get; set; }
        public int drain_hole_cont_ng { get; set; }
        //public int leak_test_ok { get; set; }
        public int leak_test_ng { get; set; }
        public int shift_ng_counter { get; set; }
    }
}
