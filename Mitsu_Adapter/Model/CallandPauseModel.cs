using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOPS.Mitsu_Adapter
{
    public class CallandPauseModel
    {
        public string RegisterInfo { get; set; }

        public int CurrentModelStatus { get; set; }

        public int ExistingModelStatus { get; set; }

        public long ExistingValue { get; set; }

        public int CurrentValue { get; set; }
        public Stopwatch PauseDataStopWatch { get; set; }
        public CallandPauseModel(string registerInfo, int previousValue, int existingModelStatus)
        {
            RegisterInfo = registerInfo;
            CurrentModelStatus = previousValue;
            ExistingModelStatus = existingModelStatus;
            CurrentValue = 1;
            PauseDataStopWatch = new Stopwatch();
        }
    }
}
