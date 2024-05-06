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

	internal class Z31_InserationCalibration : MitsuBaseClass
	{
		//Message _mcAlarms = new Message("alarm");
		// Message mPartcount = new Message("part_count_2");
		//Sample mcycletime = new Sample("cycle_time_sec");

		Message mInserationCalibration = new Message("InserationCalibrationData");

		public Z31_InserationCalibration(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
			_mAdapter.AddDataItem(mInserationCalibration);
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


			GetInserationCalibration();

			//check machine lamp status for idle if idel start timer

		}

		#region InserationCalibration
		private void GetInserationCalibration()
		{
			const int userreg = 14694;
			const int opshift = 14711;
			string userdata = string.Empty;
			string shift = string.Empty;


			int SI_No = 0;
			_mitsuPLC.GetDevice("D14690", out SI_No);

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
			_mitsuPLC.GetDevice("D14728", out componentAweight);
			float caweight = BitConverter.ToSingle(BitConverter.GetBytes(componentAweight), 0);

			int componentBweight = 0;
			_mitsuPLC.GetDevice("D14730", out componentBweight);
			float cbweight = BitConverter.ToSingle(BitConverter.GetBytes(componentBweight), 0);

			int ratio = 0;
			_mitsuPLC.GetDevice("D14732", out ratio);

			int ratiostatus = 0;
			_mitsuPLC.GetDevice("D14734", out ratiostatus);

			int minratio = 0;
			_mitsuPLC.GetDevice("D14736", out minratio);
			float minimumRatio = BitConverter.ToSingle(BitConverter.GetBytes(minratio), 0);

			int maxratio = 0;
			_mitsuPLC.GetDevice("D14738", out maxratio);
			float maximumRatio = BitConverter.ToSingle(BitConverter.GetBytes(maxratio), 0);

			int cAservospeed = 0;
			_mitsuPLC.GetDevice("D14742", out cAservospeed);

			int ComponentADrumPressMotorSpeed = 0;
			_mitsuPLC.GetDevice("D14744", out ComponentADrumPressMotorSpeed);

			int cAtank = 0;
			_mitsuPLC.GetDevice("D14746", out cAtank);
			float ComponentADrumPressLinePressure = BitConverter.ToSingle(BitConverter.GetBytes(cAtank), 0);

			int cAop = 0;
			_mitsuPLC.GetDevice("D14748", out cAop);
			float cAservoinletpr = BitConverter.ToSingle(BitConverter.GetBytes(cAop), 0);

			int cBservospeed = 0;
			_mitsuPLC.GetDevice("D14750", out cBservospeed);
			float cAservoOutletpr = BitConverter.ToSingle(BitConverter.GetBytes(cBservospeed), 0);


			int cBmotorStatus = 0;
			_mitsuPLC.GetDevice("D14752", out cBmotorStatus);

			int cBtank = 0;
			_mitsuPLC.GetDevice("D14754", out cBtank);
			float cBservoOutletpr = BitConverter.ToSingle(BitConverter.GetBytes(cBtank), 0);

			




			mInserationCalibration.Value = "{" +
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
	"\"ComponentADrumPressMotorSpeed\": \"" + ComponentADrumPressMotorSpeed + "\"," +
	"\"ComponentADrumPressLinePressure\": \"" + ComponentADrumPressLinePressure + "\"," +
	"\"ComponentAServoInletPressure\": \"" + cAservoinletpr + "\"," +
	"\"ComponentAServoOutletPressure\": \"" + cAservoOutletpr + "\"," +
	"\"ComponentBMotorOnStatus\": \"" + cBmotorStatus + "\"," +
	"\"ComponentBServoOutletPressure\": \"" + cBservoOutletpr + "\"," +
	

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
