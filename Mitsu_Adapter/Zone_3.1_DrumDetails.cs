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

	internal class Z31_DrumDetails : MitsuBaseClass
	{
		//Message _mcAlarms = new Message("alarm");
		// Message mPartcount = new Message("part_count_2");
		//Sample mcycletime = new Sample("cycle_time_sec");

		Message mDrumData = new Message("DrumDetailsData");

		public Z31_DrumDetails(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
			const int userreg = 13004;
			const int opshift = 13021;
			const int drum1sernum = 13038;
			const int drum1expdate = 13055;
            const int drum2expdate = 13089;
            const int thermaldrum1expdate = 13123;
            const int thermaldrum2expdate = 13157;
            const int inserdrum1expdate = 13191;
            const int inserdrum2expdate = 13225;
            const int drum2sernum = 13072;
			const int thermaldrum1 = 13106;
			const int thermaldrum2 = 13140;
			const int insertiondrum1 = 13174;
			const int insertiondrum2 = 13208;
			string userdata = string.Empty;
			string shift = string.Empty;
			string barcodeData = string.Empty;
			string drum1exp = string.Empty;
            string drum2exp = string.Empty;
            string thermaldrum1exp = string.Empty;
            string thermaldrum2exp = string.Empty;
            string inserdrum1exp = string.Empty;
            string inserdrum2exp = string.Empty;
            string barcodeData1 = string.Empty;
			string barcodeData2 = string.Empty;
			string barcodeData3 = string.Empty;
			string barcodeData4 = string.Empty;
			string barcodeData5 = string.Empty;


			int SI_No = 0;
			_mitsuPLC.GetDevice("D13000", out SI_No);

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

            for (int i = 0; i < 13; i++)
            {
                string barcodee = "D" + (drum1expdate + i);
                drum1exp = drum1exp + GetASCII(barcodee);
            }
            drum1exp = drum1exp.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            // Drum Expiry Date needs to be monitored in live ladder 

            //Drum2SerialNumber
            for (int i = 0; i < 13; i++)
			{
				string battery = "D" + (drum2sernum + i);
				barcodeData1 = barcodeData1 + GetASCII(battery);
			}
			barcodeData1 = barcodeData1.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 13; i++)
            {
                string barcodee = "D" + (drum2expdate + i);
                drum2exp = drum2exp + GetASCII(barcodee);
            }
            drum2exp = drum2exp.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //Drum2SerialNumber

            //ThermalD1SerialNumber
            for (int i = 0; i < 13; i++)
			{
				string battery = "D" + (thermaldrum1 + i);
				barcodeData2 = barcodeData2 + GetASCII(battery);
			}
			barcodeData2 = barcodeData2.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 13; i++)
            {
                string barcodee = "D" + (thermaldrum1expdate + i);
                thermaldrum1exp = thermaldrum1exp + GetASCII(barcodee);
            }
            thermaldrum1exp = thermaldrum1exp.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //ThermalD1SerialNumber

            //ThermalD2SerialNumber
            for (int i = 0; i < 13; i++)
			{
				string battery = "D" + (thermaldrum2 + i);
				barcodeData3 = barcodeData3 + GetASCII(battery);
			}
			barcodeData3 = barcodeData3.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 13; i++)
            {
                string barcodee = "D" + (thermaldrum2expdate + i);
                thermaldrum2exp = thermaldrum2exp + GetASCII(barcodee);
            }
            thermaldrum2exp = thermaldrum2exp.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //ThermalD2SerialNumber


            //InsertionD1SerialNumber
            for (int i = 0; i < 13; i++)
			{
				string battery = "D" + (insertiondrum1 + i);
				barcodeData4 = barcodeData4 + GetASCII(battery);
			}
			barcodeData4 = barcodeData4.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 13; i++)
            {
                string barcodee = "D" + (inserdrum1expdate + i);
                inserdrum1exp = inserdrum1exp + GetASCII(barcodee);
            }
            inserdrum1exp = inserdrum1exp.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //InsertionD1SerialNumber


            //InsertionD2SerialNumber
            for (int i = 0; i < 13; i++)
			{
				string battery = "D" + (insertiondrum2 + i);
				barcodeData5 = barcodeData5 + GetASCII(battery);
			}
			barcodeData5 = barcodeData5.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 13; i++)
            {
                string barcodee = "D" + (inserdrum2expdate + i);
                inserdrum2exp = inserdrum2exp + GetASCII(barcodee);
            }
            inserdrum2exp = inserdrum2exp.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();
            //InsertionD2SerialNumber


            int drum1level = 0;
			_mitsuPLC.GetDevice("D13242", out drum1level);

			int drum2level = 0;
			_mitsuPLC.GetDevice("D13244", out drum2level);

			int thermaldrum1level = 0;
			_mitsuPLC.GetDevice("D13246", out thermaldrum1level);

			int thermaldrum2level = 0;
			_mitsuPLC.GetDevice("D13248", out thermaldrum2level);

			int inserationdrum1level = 0;
			_mitsuPLC.GetDevice("D13250", out inserationdrum1level);

			int inserationdrum2level = 0;
			_mitsuPLC.GetDevice("D13252", out inserationdrum2level);
/*
			float number = 50 / 10;

			string objec = "DrumDetails";

			mDrumData.Value = "{" +
	 "\"Number\": \"" + number + "\"," +
	"\"String\": \"" + objec + "\"," +
	 

	"}";*/



            mDrumData.Value = "{" +
	"\"SI_No\": \"" + SI_No + "\"," +
	"\"DateTime\": \"" + formattedDateTime + "\"," +
	"\"UserName\": \"" + userdata + "\"," +
	"\"FoamDrum01Serialnumber\": \"" + barcodeData + "\"," +
	"\"FoamDrum01ExpiryDate\": \"" + drum1exp + "\"," +
	"\"FoamDrum02Serialnumber\": \"" + barcodeData1 + "\"," +
    "\"FoamDrum02ExpiryDate\": \"" + drum2exp + "\"," +
    "\"ThermalDrum01Serialnumber\": \"" + barcodeData2 + "\"," +
    "\"ThermalDrum01ExpiryDate\": \"" + thermaldrum1exp + "\"," +
     "\"ThermalDrum02Serialnumber\": \"" + barcodeData3 + "\"," +
     "\"ThermalDrum02ExpiryDate\": \"" + thermaldrum2exp + "\"," +
     "\"InserationDrum01Serialnumber\": \"" + barcodeData4 + "\"," +
     "\"InserationDrum01ExpiryDate\": \"" + inserdrum1exp + "\"," +
    "\"InserationDrum02Serialnumber\": \"" + barcodeData5 + "\"," +
     "\"InserationDrum02ExpiryDate\": \"" + inserdrum2exp + "\"," +
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
