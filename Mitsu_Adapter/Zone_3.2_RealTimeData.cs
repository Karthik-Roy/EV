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

	internal class Z32_RealTimeData : MitsuBaseClass
	{
		//Message _mcAlarms = new Message("alarm");
		// Message mPartcount = new Message("part_count_2");
		//Sample mcycletime = new Sample("cycle_time_sec");

		Message mRealTimeData = new Message("RealTimeData");

		public Z32_RealTimeData(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
			_mAdapter.AddDataItem(mRealTimeData);
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


			GetRealTimeData();

			//check machine lamp status for idle if idel start timer

		}

		#region RealTimeData
		private void GetRealTimeData()
		{
			const int userreg = 14109;
			const int opshift = 14142;
			string userdata = string.Empty;
			string shift = string.Empty;

			int SI_No = 0;
			_mitsuPLC.GetDevice("D14105", out SI_No);

			DateTime currentDateTime = DateTime.Now;
			string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

			for (int i = 0; i < 6; i++)
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

			int zfixstatus = 0;
			_mitsuPLC.GetDevice("D14174", out zfixstatus);

			int weldst01status = 0;
			_mitsuPLC.GetDevice("D14176", out weldst01status);
			
			int weldst02status = 0;
			_mitsuPLC.GetDevice("D14178", out weldst02status);

			int weldintstatus = 0;
			_mitsuPLC.GetDevice("D14180", out weldintstatus);

			int foamstationstatus = 0;
			_mitsuPLC.GetDevice("D14182", out foamstationstatus);

			int thermalstationstatus = 0;
			_mitsuPLC.GetDevice("D14184", out thermalstationstatus);

			int bmsactivationstatus = 0;
			_mitsuPLC.GetDevice("D14186", out bmsactivationstatus);

			int inserationstatus = 0;
			_mitsuPLC.GetDevice("D14188", out inserationstatus);

			int buffstatus = 0;
			_mitsuPLC.GetDevice("D14190", out buffstatus);

			int palletstatus = 0;
			_mitsuPLC.GetDevice("D14192", out palletstatus);

			mRealTimeData.Value = "{" +
	"\"SINo\": \"" + SI_No + "\"," +
	"\"DateTime\": \"" + formattedDateTime + "\"," +
	"\"UserName\": \"" + userdata + "\"," +
	"\"OperationalShift\": \"" + shift + "\"," +
	"\"Z_FixationStatus\": \"" + zfixstatus + "\"," +
	"\"Welding01StationStatus\": \"" + weldst01status + "\"," +
	"\"Welding02StationStatus\": \"" + weldst02status + "\"," +
	"\"WeldintegrityStationStatus\": \"" + weldintstatus + "\"," +
	"\"FoamStationOutCounts\": \"" + foamstationstatus + "\"," +
	"\"ThermalStationOutCounts\": \"" + thermalstationstatus + "\"," +
	"\"BMSActivationStationOutCounts\": \"" + bmsactivationstatus + "\"," +
	"\"InserationStationOutCounts\": \"" + inserationstatus + "\"," +
	"\"PullTestStationOutCount\": \"" + buffstatus + "\"," +
	"\"LeakTestingstationOutCounts\": \"" + palletstatus + "\"," +



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
