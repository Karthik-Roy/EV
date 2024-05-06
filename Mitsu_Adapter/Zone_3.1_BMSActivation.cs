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

    internal class Z31_BMSActivation : MitsuBaseClass
    {
        //Message _mcAlarms = new Message("alarm");
        // Message mPartcount = new Message("part_count_2");
        //Sample mcycletime = new Sample("cycle_time_sec");

        Message mBMSActivation = new Message("BMSActivationData");

        public Z31_BMSActivation(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
            _mAdapter.AddDataItem(mBMSActivation);
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


            GetBMSActivation();

            //check machine lamp status for idle if idel start timer

        }

        #region BMSActivation
        private void GetBMSActivation()
        {
            const int bat_id = 15188;
            const int bms_id = 15207;
            
            const int err1 = 15283;
            const int err2 = 15300;
            const int err3 = 15317;
            const int err4 = 15283;
            const int err5 = 15300;
            const int err6 = 15317;
            const int err7 = 15317;
            string baterryID = string.Empty;
            string bmsID = string.Empty;
            string errcode1 = string.Empty;
            string errcode2 = string.Empty;
            string errcode3 = string.Empty;
            string errcode4 = string.Empty;
            string errcode5 = string.Empty;
            string errcode6 = string.Empty;
            string errcode7 = string.Empty;



            /* int SI_No = 0;
             _mitsuPLC.GetDevice("D14690", out SI_No);*/

            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            for (int i = 0; i < 15; i++)
            {
                string user = "D" + (bat_id + i);
                baterryID = baterryID + GetASCII(user);
            }
            baterryID = baterryID.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 30; i++)
            {
                string operation_shift = "D" + (bms_id + i);
                bmsID = bmsID + GetASCII(operation_shift);
            }
            bmsID = bmsID.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            int voltage = 0;
            _mitsuPLC.GetDevice("D15243", out voltage);
            float volt = voltage / 10.0f;
            

            int totalE = 0;
            _mitsuPLC.GetDevice("D15245", out totalE);
           

            int soH = 0;
            _mitsuPLC.GetDevice("D15247", out soH);

            int soC = 0;
            _mitsuPLC.GetDevice("D15249", out soC);
            float SoC = soC / 10.0f;

            int soE = 0;
            _mitsuPLC.GetDevice("D15251", out soE);
            float SoE = soE / 10.0f;

            int temp = 0;
            _mitsuPLC.GetDevice("D15253", out temp);
            float temperature = temp / 10.0f;

            int cell0 = 0;
            _mitsuPLC.GetDevice("D15255", out cell0);
            float Cell0 = cell0 / 10.0f;

            int cell1 = 0;
            _mitsuPLC.GetDevice("D15257", out cell1);
            float Cell1 = cell1 / 10.0f;

            int cell2 = 0;
            _mitsuPLC.GetDevice("D15259", out cell2);
            float Cell2 = cell2 / 10.0f;

            int cell3 = 0;
            _mitsuPLC.GetDevice("D15261", out cell3);
            float Cell3 = cell3 / 10.0f;

            int cell4 = 0;
            _mitsuPLC.GetDevice("D15263", out cell4);
            float Cell4 = cell4 / 10.0f;

            int cell5 = 0;
            _mitsuPLC.GetDevice("D15265", out cell5);
            float Cell5 = cell5 / 10.0f;

            int cell6 = 0;
            _mitsuPLC.GetDevice("D15267", out cell6);
            float Cell6 = cell6 / 10.0f;

            int cell7 = 0;
            _mitsuPLC.GetDevice("D15269", out cell7);
            float Cell7 = cell7 / 10.0f;

            int cell8 = 0;
            _mitsuPLC.GetDevice("D15271", out cell8);
            float Cell8 = cell8 / 10.0f;

            int cell9 = 0;
            _mitsuPLC.GetDevice("D15273", out cell9);
            float Cell9 = cell9 / 10.0f;

            int cell10 = 0;
            _mitsuPLC.GetDevice("D15275", out cell10);
            float Cell10 = cell10 / 10.0f;

            int cell11 = 0;
            _mitsuPLC.GetDevice("D15277", out cell11);
            float Cell11 = cell11 / 10.0f;

            int cell12 = 0;
            _mitsuPLC.GetDevice("D15279", out cell12);
            float Cell12 = cell12 / 10.0f;

            int cell13 = 0;
            _mitsuPLC.GetDevice("D15281", out cell13);
            float Cell13 = cell13 / 10.0f;

            for (int i = 0; i < 15; i++)
            {
                string errCode1 = "D" + (err1 + i);
                errcode1 = errcode1 + GetASCII(errCode1);
            }
            errcode1 = errcode1.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string errCode2 = "D" + (err2 + i);
                errcode2 = errcode2 + GetASCII(errCode2);
            }
            errcode2 = errcode2.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string errCode1 = "D" + (err1 + i);
                errcode1 = errcode1 + GetASCII(errCode1);
            }
            errcode1 = errcode1.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string errCode1 = "D" + (err1 + i);
                errcode1 = errcode1 + GetASCII(errCode1);
            }
            errcode1 = errcode1.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string errCode2 = "D" + (err2 + i);
                errcode2 = errcode2 + GetASCII(errCode2);
            }
            errcode2 = errcode2.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string errCode3 = "D" + (err3 + i);
                errcode3 = errcode3 + GetASCII(errCode3);
            }
            errcode3 = errcode3.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string errCode4 = "D" + (err4 + i);
                errcode4 = errcode4 + GetASCII(errCode4);
            }
            errcode4 = errcode4.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string errCode5 = "D" + (err5 + i);
                errcode5 = errcode5 + GetASCII(errCode5);
            }
            errcode5 = errcode5.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string errCode6 = "D" + (err6 + i);
                errcode6 = errcode6 + GetASCII(errCode6);
            }
            errcode4 = errcode4.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 15; i++)
            {
                string errCode7 = "D" + (err7 + i);
                errcode7 = errcode7 + GetASCII(errCode7);
            }
            errcode7 = errcode7.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            float zone = 3.1f;






            mBMSActivation.Value = "{" +
    "\"DateTime\": \"" + formattedDateTime + "\"," +
    "\"BatteryID\": \"" + baterryID + "\"," +
    "\"BMSID\": \"" + bmsID + "\"," +
    "\"Voltage\": \"" + volt + "\"," +
    "\"TotalEnergy\": \"" + totalE + "\"," +
    "\"SOH\": \"" + soH + "\"," +
    "\"SOC\": \"" + SoC + "\"," +
    "\"SOE\": \"" + SoE + "\"," +
    "\"Temperature\": \"" + temperature + "\"," +
    "\"Cell0\": \"" + Cell0 + "\"," +
    "\"Cell1\": \"" + Cell1 + "\"," +
    "\"Cell2\": \"" + Cell2 + "\"," +
    "\"Cell3\": \"" + Cell3 + "\"," +
    "\"Cell4\": \"" + Cell4 + "\"," +
    "\"Cell5\": \"" + Cell5 + "\"," +
    "\"Cell6\": \"" + Cell6 + "\"," +
    "\"Cell7\": \"" + Cell7 + "\"," +
    "\"Cell8\": \"" + Cell8 + "\"," +
    "\"Cell9\": \"" + Cell9 + "\"," +
    "\"Cell10\": \"" + Cell10 + "\"," +
    "\"Cell11\": \"" + Cell11 + "\"," +
    "\"Cell12\": \"" + Cell12 + "\"," +
    "\"Cell13\": \"" + Cell13 + "\"," +
    "\"ErrorCode01\": \"" + errcode1 + "\"," +
    "\"ErrorCode02\": \"" + errcode2 + "\"," +
    "\"ErrorCode03\": \"" + errcode3 + "\"," +
    "\"ErrorCode04\": \"" + errcode4 + "\"," +
    "\"ErrorCode05\": \"" + errcode5 + "\"," +
    "\"ErrorCode06\": \"" + errcode6 + "\"," +
    "\"ErrorCode07\": \"" + errcode7 + "\"," +



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
