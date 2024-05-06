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

	internal class Z31_ProductionData : MitsuBaseClass
	{
		//Message _mcAlarms = new Message("alarm");
		// Message mPartcount = new Message("part_count_2");
		//Sample mcycletime = new Sample("cycle_time_sec");

		Message mProduction = new Message("ProductionData");

		public Z31_ProductionData(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
			_mAdapter.AddDataItem(mProduction);
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


			GetProductionData();

			//check machine lamp status for idle if idel start timer

		}

		#region ProductionData
		private void GetProductionData()
		{
			const int userreg = 12519;
			const int opshift = 12536;
			string userdata = string.Empty;
			string shift = string.Empty;
			
			int SI_No = 0;
			_mitsuPLC.GetDevice("D12500", out SI_No);

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

			int zfixoutcount = 0;
			_mitsuPLC.GetDevice("D12553", out zfixoutcount);

			int weldst01count = 0;
			_mitsuPLC.GetDevice("D12554", out weldst01count);

			int weldst02count = 0;
			_mitsuPLC.GetDevice("D12555", out weldst02count);

			int weldintoutcount = 0;
			_mitsuPLC.GetDevice("D12556", out weldintoutcount);

			int foamstationcount = 0;
			_mitsuPLC.GetDevice("D12557", out foamstationcount);

			int thermalstationoutcount = 0;
			_mitsuPLC.GetDevice("D12558", out thermalstationoutcount);

			int bmsactivationcount = 0;
			_mitsuPLC.GetDevice("D12559", out bmsactivationcount);

			int inserationcount = 0;
			_mitsuPLC.GetDevice("D12560", out inserationcount);

			int pulltestcount = 0;
			_mitsuPLC.GetDevice("D12561", out pulltestcount);

			int leaktestcount = 0;
			_mitsuPLC.GetDevice("D12562", out leaktestcount);

			int breathercount = 0;
			_mitsuPLC.GetDevice("D12563", out breathercount);


			mProduction.Value = "{" +
	"\"SINo\": \"" + SI_No + "\"," +
	"\"DateTime\": \"" + formattedDateTime + "\"," +
	"\"UserName\": \"" + userdata + "\"," +
	"\"OperationalShift\": \"" + shift + "\"," +
	"\"Z_FixationOutCounts\": \"" + zfixoutcount + "\"," +
	"\"Weldingstation01OutCounts\": \"" + weldst01count + "\"," +
	"\"Weldingstation02OutCounts\": \"" + weldst02count + "\"," +
	"\"WeldintegrityOutCounts\": \"" + weldintoutcount + "\"," +
	"\"FoamStationOutCounts\": \"" + foamstationcount + "\"," +
	"\"ThermalStationOutCounts\": \"" + thermalstationoutcount + "\"," +
	"\"BMSActivationStationOutCounts\": \"" + bmsactivationcount + "\"," +
	"\"InserationStationOutCounts\": \"" + inserationcount + "\"," +
	"\"PullTestStationOutCount\": \"" + pulltestcount + "\"," +
	"\"LeakTestingstationOutCounts\": \"" + leaktestcount + "\"," +
	"\"BreatherstationOutCounts\": \"" + breathercount + "\"," +
	

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
