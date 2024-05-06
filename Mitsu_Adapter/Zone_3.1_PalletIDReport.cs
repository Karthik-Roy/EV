using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SOPS.MitsuBase;
using SOPS.MTConnect;
namespace SOPS.Mitsu_Adapter
{

	internal class Z31_PalletIDReport : MitsuBaseClass
	{
		//Message _mcAlarms = new Message("alarm");
		// Message mPartcount = new Message("part_count_2");
		//Sample mcycletime = new Sample("cycle_time_sec");

		Message mPalletID = new Message("PalletIDData");

		public Z31_PalletIDReport(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
			_mAdapter.AddDataItem(mPalletID);
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


			GetPalletID();

			//check machine lamp status for idle if idel start timer

		}

		#region PalletID
		private void GetPalletID()
		{
			const int userreg = 13259;
			const int opshift = 13276;
			const int palletbarcode = 6030;
			const int batteryid = 6050;
			const int zfixId = 6070;
			string userdata = string.Empty;
			string shift = string.Empty;
			string pbcode = string.Empty;
			string battery = string.Empty;
			string zfixation = string.Empty;

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

			for (int i = 0; i < 16; i++)
			{
				string pallet = "D" + (palletbarcode + i);
				pbcode = pbcode + GetASCII(pallet);
			}
			pbcode = pbcode.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

			for (int i = 0; i < 16; i++)
			{
				string btry = "D" + (batteryid + i);
				battery = battery + GetASCII(btry);
			}
			battery = battery.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

			for(int i = 0; i < 16; i++)
			{
				string zfix = "D" + (zfixId + i);
				zfixation = zfixation + GetASCII(zfix);
			}
			zfixation = zfixation.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();





			mPalletID.Value = "{" +
	"\"datetime\": \"" + formattedDateTime + "\"," +
	"\"UserName\": \"" + userdata + "\"," +
	"\"OperationalShift\": \"" + shift + "\"," +
	"\"PalletBarcodeData\": \"" + pbcode + "\"," +
	"\"BatteryID\": \"" + battery + "\"," +
	"\"Z_Fixation_ID\": \"" + zfixation + "\"," +

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
