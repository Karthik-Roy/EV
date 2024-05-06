using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SOPS.MitsuBase;
using SOPS.MTConnect;
namespace SOPS.Mitsu_Adapter
{

    internal class LeakTesting : MitsuBaseClass
    {
        //Message _mcAlarms = new Message("alarm");
        // Message mPartcount = new Message("part_count_2");
        //Sample mcycletime = new Sample("cycle_time_sec");

        Message mLeakTesting = new Message("LeakTestingData");

        public LeakTesting(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
        {


        }
        protected override void OnReadPLCData()
        {
            _mAdapter.Begin();
            GetPlcGroups();

            GetInternalVariables();
            _mAdapter.SendChanged();
        }

        public override void StartPLCTimer()
        {
            _mAdapter.Start();

            _mAvail.Value = "AVAILABLE";
            _mAdapter.AddDataItem(_mAvail);
            _mAdapter.AddDataItem(_power);
            //_mAdapter.AddDataItem(_mcAlarms);
            _mAdapter.AddDataItem(_machineLampStatus);
            _mAdapter.AddDataItem(_mMajorDownTime);
            _mAdapter.AddDataItem(_mMinorDownTime);
            _mAdapter.AddDataItem(mLeakTesting);
            //_mAdapter.AddDataItem(mcycletime);
            _machineLampStatus.Value = 3;
            _mMajorDownTime.Value = 0;
            _mMinorDownTime.Value = 0;

            Thread t = new Thread(new ThreadStart(ParamExchangeThread));
            t.Start();
            t.IsBackground = true;

        }



        private void GetInternalVariables()
        {
            //GetMachineLampStatus();
            if (((int)_machineLampStatus.Value == 2) || ((int)_machineLampStatus.Value == 4))
            {
                DownTimeManager(true);
            }
            else if ((int)_machineLampStatus.Value == 0)  //check machine lamp status for green auto , if auto stop timer
            {
                DownTimeManager(false);
            }
            // GetAlarms();
            if (!IsConnected) return;


            GetLeakTesting();

            //check machine lamp status for idle if idel start timer

        }

        #region LeakTesting
        private void GetLeakTesting()
        {
            const int userreg = 13544;
            const int opshift = 13561;
            const int batterybcode = 13578;
            string userdata = string.Empty;
            string shift = string.Empty;
            string barcode = string.Empty;


            /*int SI_No = 0;
            _mitsuPLC.GetDevice("D13540", out SI_No);*/

            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            for (int i = 0; i < 7; i++)
            {
                string user = "D" + (userreg + i);
                userdata = userdata + GetASCII(user);
            }
            userdata = userdata.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 3; i++)
            {
                string operation_shift = "D" + (opshift + i);
                shift = shift + GetASCII(operation_shift);
            }
            shift = shift.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string battery = "D" + (batterybcode + i);
                barcode = barcode + GetASCII(battery);
            }
            barcode = barcode.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();


            int pressureCurrent = 0;
            _mitsuPLC.GetDevice("D13597", out pressureCurrent);

            int leakSet = 0;
            _mitsuPLC.GetDevice("D13599", out leakSet);

            int leakCurrent = 0;
            _mitsuPLC.GetDevice("D13601", out leakCurrent);

            int leakResult = 0;
            _mitsuPLC.GetDevice("D13603", out leakResult);

            




            mLeakTesting.Value = "{" +
    "\"DateTime\": \"" + formattedDateTime + "\"," +
    "\"UserName\": \"" + userdata + "\"," +
    "\"OperationalShift\": \"" + shift + "\"," +
    "\"Battery_Pack_Barcode\": \"" + barcode + "\"," +
    "\"PressureCurrentValue\": \"" + pressureCurrent + "\"," +
    "\"LeakSetValue\": \"" + leakSet + "\"," +
    "\"LeakCurrentValue\": \"" + leakCurrent + "\"," +
    "\"LeakTestResult\": \"" + leakResult + "\"," +
    

    "}";



        }
        private string GetASCII(string register)
        {
            int outData = 0;
            if (_mitsuPLC.GetDevice(register, out outData) != 0) return null;
            byte lowByte = (byte)(outData & 0xff);
            byte highByte = (byte)((outData >> 8) & 0xff);

            return Convert.ToChar(lowByte).ToString() + Convert.ToChar(highByte).ToString();

        }
        #endregion





    }
}
