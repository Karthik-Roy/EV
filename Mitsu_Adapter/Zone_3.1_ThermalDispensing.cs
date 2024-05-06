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

    internal class Z31_ThermalDispensing : MitsuBaseClass
    {
        //Message _mcAlarms = new Message("alarm");
        // Message mPartcount = new Message("part_count_2");
        //Sample mcycletime = new Sample("cycle_time_sec");

        Message mThermalDispensing = new Message("ThermalDispensingData");

        public Z31_ThermalDispensing(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
            _mAdapter.AddDataItem(mThermalDispensing);
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


            GetThermalDispensing();

            //check machine lamp status for idle if idel start timer

        }

        #region ThermalDispensing
        private void GetThermalDispensing()
        {
            const int userreg = 14882;
            const int opshift = 14899;
            string userdata = string.Empty;
            string shift = string.Empty;


            int SI_No = 0;
            _mitsuPLC.GetDevice("D14878", out SI_No);

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


            int cAservospeed = 0;
            _mitsuPLC.GetDevice("D14916", out cAservospeed);

            int cAdrumMotorSpeed = 0;
            _mitsuPLC.GetDevice("D14918", out cAdrumMotorSpeed);

            int cAdrumpr = 0;
            _mitsuPLC.GetDevice("D14920", out cAdrumpr);

            int cAServoInPressure = 0;
            _mitsuPLC.GetDevice("D14922", out cAServoInPressure);
            float cAtanklevel = BitConverter.ToSingle(BitConverter.GetBytes(cAServoInPressure), 0);

            int cAServoOutPressure = 0;
            _mitsuPLC.GetDevice("D14924", out cAServoOutPressure);
            float cAoutletpr = BitConverter.ToSingle(BitConverter.GetBytes(cAServoOutPressure), 0);

            int cBservospeed = 0;
            _mitsuPLC.GetDevice("D14926", out cBservospeed);

            int cBDrumMotorSpeed = 0;
            _mitsuPLC.GetDevice("D14928", out cBDrumMotorSpeed);

            int cBdrumpr = 0;
            _mitsuPLC.GetDevice("D14930", out cBdrumpr);

            int cBServoInPressure = 0;
            _mitsuPLC.GetDevice("D14932", out cBServoInPressure);
            float cBtanklevel = BitConverter.ToSingle(BitConverter.GetBytes(cBServoInPressure), 0);

            int cBServoOutPressure = 0;
            _mitsuPLC.GetDevice("D14934", out cBServoOutPressure);
            float cBoutletpr = BitConverter.ToSingle(BitConverter.GetBytes(cBServoOutPressure), 0);




            mThermalDispensing.Value = "{" +
    "\"SI_No\": \"" + SI_No + "\"," +
    "\"DateTime\": \"" + formattedDateTime + "\"," +
    "\"UserName\": \"" + userdata + "\"," +
    "\"OperationalShift\": \"" + shift + "\"," +
    "\"ComponentAServoSpeed\": \"" + cAservospeed + "\"," +
    "\"ComponentADrumPressMotorSpeed\": \"" + cAdrumMotorSpeed + "\"," +
    "\"ComponentADrumPressLinePressure\": \"" + cAdrumpr + "\"," +
    "\"ComponentAServoInletPressure\": \"" + cAServoInPressure + "\"," +
    "\"ComponentAServoOutletPressure\": \"" + cAServoOutPressure + "\"," +
    "\"ComponentBServoSpeed\": \"" + cBservospeed + "\"," +
    "\"ComponentBDrumPressMotorSpeed\": \"" + cBDrumMotorSpeed + "\"," +
    "\"ComponentBDrumPressLinePressure\": \"" + cBdrumpr + "\"," +
    "\"ComponentBServoInletPressure\": \"" + cBServoInPressure + "\"," +
    "\"ComponentBServoOutletPressure\": \"" + cBServoOutPressure + "\"," +

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
