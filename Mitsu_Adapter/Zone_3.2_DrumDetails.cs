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

    internal class Z32_DrumDetails : MitsuBaseClass
    {
        //Message _mcAlarms = new Message("alarm");
        // Message mPartcount = new Message("part_count_2");
        //Sample mcycletime = new Sample("cycle_time_sec");

        Message mDrumData = new Message("DrumDetailsData");

        public Z32_DrumDetails(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
            /*_mAdapter.AddDataItem(_machineLampStatus);
			_mAdapter.AddDataItem(_mMajorDownTime);
			_mAdapter.AddDataItem(_mMinorDownTime);*/
            _mAdapter.AddDataItem(mDrumData);
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
            /*	//GetMachineLampStatus();
                if (((int)_machineLampStatus.Value == 2) || ((int)_machineLampStatus.Value == 4))
                {
                    DownTimeManager(true);
                }
                else if ((int)_machineLampStatus.Value == 0)  //check machine lamp status for green auto , if auto stop timer
                {
                    DownTimeManager(false);
                }
                // GetAlarms();
                if (!IsConnected) return;*/


            GetDrumData();

            //check machine lamp status for idle if idel start timer

        }

        #region DrumDetailsData
        private void GetDrumData()
        {
            const int userreg = 13111;
            const int opshift = 13128;
            const int drum1sernum = 13145;
            const int drum2sernum = 13179;
            const int thermaldrum1 = 13213;
            const int thermaldrum2 = 13247;
            const int insertiondrum1 = 13281;
            const int insertiondrum2 = 13315;
            string userdata = string.Empty;
            string shift = string.Empty;
            string barcodeData = string.Empty;
            string barcodeData1 = string.Empty;
            string barcodeData2 = string.Empty;
            string barcodeData3 = string.Empty;
            string barcodeData4 = string.Empty;
            string barcodeData5 = string.Empty;


            int SI_No = 0;
            _mitsuPLC.GetDevice("D13107", out SI_No);

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


            for (int i = 0; i < 13; i++)
            {
                string barcodee = "D" + (drum1sernum + i);
                barcodeData = barcodeData + GetASCII(barcodee);
            }
            barcodeData = barcodeData.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            // Drum Expiry Date needs to be monitored in live ladder 

            //Drum2SerialNumber
            for (int i = 0; i < 13; i++)
            {
                string battery = "D" + (drum2sernum + i);
                barcodeData1 = barcodeData1 + GetASCII(battery);
            }
            barcodeData1 = barcodeData1.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //Drum2SerialNumber

            //ThermalD1SerialNumber
            for (int i = 0; i < 13; i++)
            {
                string battery = "D" + (thermaldrum1 + i);
                barcodeData2 = barcodeData2 + GetASCII(battery);
            }
            barcodeData2 = barcodeData2.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //ThermalD1SerialNumber

            //ThermalD2SerialNumber
            for (int i = 0; i < 13; i++)
            {
                string battery = "D" + (thermaldrum2 + i);
                barcodeData3 = barcodeData3 + GetASCII(battery);
            }
            barcodeData3 = barcodeData3.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //ThermalD1SerialNumber


            //InsertionD1SerialNumber
            for (int i = 0; i < 13; i++)
            {
                string battery = "D" + (insertiondrum1 + i);
                barcodeData4 = barcodeData4 + GetASCII(battery);
            }
            barcodeData4 = barcodeData4.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //InsertionD1SerialNumber


            //InsertionD2SerialNumber
            for (int i = 0; i < 13; i++)
            {
                string battery = "D" + (insertiondrum2 + i);
                barcodeData5 = barcodeData5 + GetASCII(battery);
            }
            barcodeData5 = barcodeData5.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //InsertionD2SerialNumber


            int drum1level = 0;
            _mitsuPLC.GetDevice("D13349", out drum1level);

            int drum2level = 0;
            _mitsuPLC.GetDevice("D13351", out drum2level);

            int thermaldrum1level = 0;
            _mitsuPLC.GetDevice("D13353", out thermaldrum1level);

            int thermaldrum2level = 0;
            _mitsuPLC.GetDevice("D13355", out thermaldrum2level);

            int inserationdrum1level = 0;
            _mitsuPLC.GetDevice("D13357", out inserationdrum1level);

            int inserationdrum2level = 0;
            _mitsuPLC.GetDevice("D13359", out inserationdrum2level);

            mDrumData.Value = "{" +
	"\"SI_No\": \"" + SI_No + "\"," +
	"\"DateTime\": \"" + formattedDateTime + "\"," +
	"\"UserName\": \"" + userdata + "\"," +
	"\"FoamDrum01Serialnumber\": \"" + barcodeData + "\"," +
	"\"FoamDrum02Serialnumber\": \"" + barcodeData1 + "\"," +
	"\"ThermalDrum01Serialnumber\": \"" + barcodeData2 + "\"," +
	"\"ThermalDrum02Serialnumber\": \"" + barcodeData3 + "\"," +
	"\"InserationDrum01Serialnumber\": \"" + barcodeData4 + "\"," +
	"\"InserationDrum02Serialnumber\": \"" + barcodeData5 + "\"," +
	"\"FoamDrum01Level\": \"" + drum1level + "\"," +
	"\"FoamDrum02Level\": \"" + drum2level + "\"," +
	"\"ThermalDrum01Level\": \"" + thermaldrum1level + "\"," +
	"\"ThermalDrum02Level\": \"" + thermaldrum2level + "\"," +
	"\"InserationDrum01Level\": \"" + inserationdrum1level + "\"," +
	"\"InserationDrum02Level\": \"" + inserationdrum2level + "\"," +
	
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
