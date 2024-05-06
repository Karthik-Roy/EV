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

    internal class Z31_ThermalCalibration : MitsuBaseClass
    {
        //Message _mcAlarms = new Message("alarm");
        // Message mPartcount = new Message("part_count_2");
        //Sample mcycletime = new Sample("cycle_time_sec");

        Message mThermalCalibration = new Message("ThermalCalibrationData");

        public Z31_ThermalCalibration(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
            _mAdapter.AddDataItem(mThermalCalibration);
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


            GetThermalCalibration();

            //check machine lamp status for idle if idel start timer

        }

        #region ThermalCalibration
        private void GetThermalCalibration()
        {
            const int userreg = 14579;
            const int opshift = 14596;
            string userdata = string.Empty;
            string shift = string.Empty;


            int SI_No = 0;
            _mitsuPLC.GetDevice("D14575", out SI_No);

            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            for (int i = 0; i < 3; i++)
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

            int componentAweight = 0;
            _mitsuPLC.GetDevice("D14613", out componentAweight);
            float caweight = BitConverter.ToSingle(BitConverter.GetBytes(componentAweight), 0);

            int componentBweight = 0;
            _mitsuPLC.GetDevice("D14615", out componentBweight);
            float cbweight = BitConverter.ToSingle(BitConverter.GetBytes(componentBweight), 0);

            int ratio = 0;
            _mitsuPLC.GetDevice("D14617", out ratio);

            int ratiostatus = 0;
            _mitsuPLC.GetDevice("D14619", out ratiostatus);

            int minratio = 0;
            _mitsuPLC.GetDevice("D14621", out minratio);
            float minimumRatio = BitConverter.ToSingle(BitConverter.GetBytes(minratio), 0);

            int maxratio = 0;
            _mitsuPLC.GetDevice("D14623", out maxratio);
            float maximumRatio = BitConverter.ToSingle(BitConverter.GetBytes(maxratio), 0);

            int cAservospeed = 0;
            _mitsuPLC.GetDevice("D14625", out cAservospeed);

            int cAtankspeed = 0;
            _mitsuPLC.GetDevice("D14627", out cAtankspeed);

            int cAtank = 0;
            _mitsuPLC.GetDevice("D14629", out cAtank);
            float cAtanklevel = BitConverter.ToSingle(BitConverter.GetBytes(cAtank), 0);

            int cAop = 0;
            _mitsuPLC.GetDevice("D14631", out cAop);
            float cAoutletpr = BitConverter.ToSingle(BitConverter.GetBytes(cAop), 0);

            int cBservospeed = 0;
            _mitsuPLC.GetDevice("D14633", out cBservospeed);

            int cBtankspeed = 0;
            _mitsuPLC.GetDevice("D14635", out cBtankspeed);

            int cBtank = 0;
            _mitsuPLC.GetDevice("D14637", out cBtank);
            float cBtanklevel = BitConverter.ToSingle(BitConverter.GetBytes(cBtank), 0);

            int cBop = 0;
            _mitsuPLC.GetDevice("D14639", out cBop);
            float cBoutletpr = BitConverter.ToSingle(BitConverter.GetBytes(cBop), 0);

            int cBinpr = 0;
            _mitsuPLC.GetDevice("D14641", out cBinpr);
            float cBinlpr = BitConverter.ToSingle(BitConverter.GetBytes(cBinpr), 0);

            int cBoutpr = 0;
            _mitsuPLC.GetDevice("D14643", out cBoutpr);
            float cBouttpr = BitConverter.ToSingle(BitConverter.GetBytes(cBoutpr), 0);




            mThermalCalibration.Value = "{" +
    "\"SI_No\": \"" + SI_No + "\"," +
    "\"DateTime\": \"" + formattedDateTime + "\"," +
    "\"UserName\": \"" + userdata + "\"," +
    "\"OperationalShift\": \"" + shift + "\"," +
    "\"ComponentAWeight\": \"" + caweight + "\"," +
    "\"ComponentBWeight\": \"" + cbweight + "\"," +
    "\"Ratio\": \"" + ratio + "\"," +
    "\"RatioStatus\": \"" + ratiostatus + "\"," +
    "\"MinimumRatioValue\": \"" + minimumRatio + "\"," +
    "\"MaximumRatioValue\": \"" + maximumRatio + "\"," +
    "\"ComponentAServoSpeed\": \"" + cAservospeed + "\"," +
    "\"ComponentADrumPressMotorSpeed\": \"" + cAtankspeed + "\"," +
    "\"ComponentADrumPressLinePressure\": \"" + cAtanklevel + "\"," +
    "\"ComponentAServoInletPressure\": \"" + cAoutletpr + "\"," +
    "\"ComponentAServoOutletPressure\": \"" + cBservospeed + "\"," +
    "\"ComponentBServoSpeed\": \"" + cBtankspeed + "\"," +
    "\"ComponentBDrumPressMotorSpeed\": \"" + cBtanklevel + "\"," +
    "\"ComponentBDrumPressLinePressure\": \"" + cBoutletpr + "\"," +
    "\"ComponentBServoInletPressure\": \"" + cBinpr+ "\"," +
    "\"ComponentBServoOutletPressure\": \"" + cBouttpr + "\"," +

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
