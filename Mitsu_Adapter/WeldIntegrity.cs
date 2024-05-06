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
    
    internal class WeldIntegrity : MitsuBaseClass
    {
        //Message _mcAlarms = new Message("alarm");
        // Message mPartcount = new Message("part_count_2");
        //Sample mcycletime = new Sample("cycle_time_sec");
        
        Message mWIData = new Message("WI_Data"); 

        public WeldIntegrity(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
            _mAdapter.AddDataItem(mWIData);
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

           
            GetWIData();

            //check machine lamp status for idle if idel start timer

        }

        #region WeldIntegrityData
        private void GetWIData()
        {
            const int userreg = 13364;
            const int opshift = 13381;
            const int barcode = 13398;
            const int batterystacknumber = 13415;
            string userdata = string.Empty;
            string shift = string.Empty;
            string barcodeData = string.Empty;
            string batterystack = string.Empty;


            int SI_No = 0;
            _mitsuPLC.GetDevice("D13360", out SI_No);

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
                string barcodee = "D" + (barcode + i);
                barcodeData = barcodeData + GetASCII(barcodee);
            }
            barcodeData = barcodeData.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            for (int i = 0; i < 2; i++)
            {
                string battery = "D" + (batterystacknumber + i);
                batterystack = batterystack + GetASCII(battery);
            }
            batterystack = batterystack.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            int op190pr1 = 0;
            _mitsuPLC.GetDevice("D13432", out op190pr1);
            float pr1 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr1), 0);

            int op190pr2 = 0;
            _mitsuPLC.GetDevice("D13434", out op190pr2);
            float pr2 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr2), 0);

            int op190pr3 = 0;
            _mitsuPLC.GetDevice("D13436", out op190pr3);
            float pr3 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr3), 0);

            int op190pr4 = 0;
            _mitsuPLC.GetDevice("D13438", out op190pr4);
            float pr4 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr4), 0);

            int op190pr5 = 0;
            _mitsuPLC.GetDevice("D13440", out op190pr5);
            float pr5 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr5), 0);

            int op190pr6 = 0;
            _mitsuPLC.GetDevice("D13442", out op190pr6);
            float pr6 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr6), 0);

            int op190pr7 = 0;
            _mitsuPLC.GetDevice("D13444", out op190pr7);
            float pr7 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr7), 0);

            int op190pr8 = 0;
            _mitsuPLC.GetDevice("D13446", out op190pr8);
            float pr8 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr8), 0);

            int op190pr9 = 0;
            _mitsuPLC.GetDevice("D13448", out op190pr9);
            float pr9 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr9), 0);

            int op190pr10 = 0;
            _mitsuPLC.GetDevice("D13450", out op190pr10);
            float pr10 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr10), 0);

            int op190pr11 = 0;
            _mitsuPLC.GetDevice("D13452", out op190pr11);
            float pr11 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr11), 0);

            int op190pr12 = 0;
            _mitsuPLC.GetDevice("D13454", out op190pr12);
            float pr12 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr12), 0);

            int op190pr13 = 0;
            _mitsuPLC.GetDevice("D13456", out op190pr13);
            float pr13 = BitConverter.ToSingle(BitConverter.GetBytes(op190pr13), 0);

            int c1x = 0;
            _mitsuPLC.GetDevice("D13458", out c1x);
            float c1xc = BitConverter.ToSingle(BitConverter.GetBytes(c1x), 0);

            int c1y = 0;
            _mitsuPLC.GetDevice("D13460", out c1y);
            float c1yc = BitConverter.ToSingle(BitConverter.GetBytes(c1y), 0);

            int c2x = 0;
            _mitsuPLC.GetDevice("D13462", out c2x);
            float c2xc = BitConverter.ToSingle(BitConverter.GetBytes(c2x), 0);

            int c2y = 0;
            _mitsuPLC.GetDevice("D13464", out c2y);
            float c2yc = BitConverter.ToSingle(BitConverter.GetBytes(c2y), 0);

            int c3x = 0;
            _mitsuPLC.GetDevice("D13466", out c3x);
            float c3xc = BitConverter.ToSingle(BitConverter.GetBytes(c3x), 0);

            int c3y = 0;
            _mitsuPLC.GetDevice("D13468", out c3y);
            float c3yc = BitConverter.ToSingle(BitConverter.GetBytes(c3y), 0);

            int c4x = 0;
            _mitsuPLC.GetDevice("D13470", out c4x);
            float c4xc = BitConverter.ToSingle(BitConverter.GetBytes(c4x), 0);

            int c4y = 0;
            _mitsuPLC.GetDevice("D13472", out c4y);
            float c4yc = BitConverter.ToSingle(BitConverter.GetBytes(c4y), 0);

            int c5x = 0;
            _mitsuPLC.GetDevice("D13474", out c5x);
            float c5xc = BitConverter.ToSingle(BitConverter.GetBytes(c5x), 0);

            int c5y = 0;
            _mitsuPLC.GetDevice("D13476", out c5y);
            float c5yc = BitConverter.ToSingle(BitConverter.GetBytes(c5y), 0);

            int c6x = 0;
            _mitsuPLC.GetDevice("D13478", out c6x);
            float c6xc = BitConverter.ToSingle(BitConverter.GetBytes(c6x), 0);

            int c6y = 0;
            _mitsuPLC.GetDevice("D13480", out c6y);
            float c6yc = BitConverter.ToSingle(BitConverter.GetBytes(c6y), 0);

            int c7x = 0;
            _mitsuPLC.GetDevice("D13482", out c7x);
            float c7xc = BitConverter.ToSingle(BitConverter.GetBytes(c7x), 0);

            int c7y = 0;
            _mitsuPLC.GetDevice("D13484", out c7y);
            float c7yc = BitConverter.ToSingle(BitConverter.GetBytes(c7y), 0);

            int c8x = 0;
            _mitsuPLC.GetDevice("D13486", out c8x);
            float c8xc = BitConverter.ToSingle(BitConverter.GetBytes(c8x), 0);

            int c8y = 0;
            _mitsuPLC.GetDevice("D13488", out c8y);
            float c8yc = BitConverter.ToSingle(BitConverter.GetBytes(c8y), 0);

            int c9x = 0;
            _mitsuPLC.GetDevice("D13490", out c9x);
            float c9xc = BitConverter.ToSingle(BitConverter.GetBytes(c9x), 0);

            int c9y = 0;
            _mitsuPLC.GetDevice("D13492", out c9y);
            float c9yc = BitConverter.ToSingle(BitConverter.GetBytes(c9y), 0);

            int c10x = 0;
            _mitsuPLC.GetDevice("D13494", out c10x);
            float c10xc = BitConverter.ToSingle(BitConverter.GetBytes(c10x), 0);

            int c10y = 0;
            _mitsuPLC.GetDevice("D13496", out c10y);
            float c10yc = BitConverter.ToSingle(BitConverter.GetBytes(c10y), 0);

            int c11x = 0;
            _mitsuPLC.GetDevice("D13498", out c11x);
            float c11xc = BitConverter.ToSingle(BitConverter.GetBytes(c11x), 0);

            int c11y = 0;
            _mitsuPLC.GetDevice("D13500", out c11y);
            float c11yc = BitConverter.ToSingle(BitConverter.GetBytes(c11y), 0);

            int c12x = 0;
            _mitsuPLC.GetDevice("D13502", out c12x);
            float c12xc = BitConverter.ToSingle(BitConverter.GetBytes(c12x), 0);

            int c12y = 0;
            _mitsuPLC.GetDevice("D13504", out c12y);
            float c12yc = BitConverter.ToSingle(BitConverter.GetBytes(c12y), 0);

            int c13x = 0;
            _mitsuPLC.GetDevice("D13506", out c13x);
            float c13xc = BitConverter.ToSingle(BitConverter.GetBytes(c13x), 0);

            int c13y = 0;
            _mitsuPLC.GetDevice("D13508", out c13y);
            float c13yc = BitConverter.ToSingle(BitConverter.GetBytes(c13y), 0);

            int c14x = 0;
            _mitsuPLC.GetDevice("D13510", out c14x);
            float c14xc = BitConverter.ToSingle(BitConverter.GetBytes(c13x), 0);

            int c14y = 0;
            _mitsuPLC.GetDevice("D13512", out c14y);
            float c14yc = BitConverter.ToSingle(BitConverter.GetBytes(c14y), 0);



            mWIData.Value = "{" +
    "\"SI_No\": \"" + SI_No + "\"," +
    "\"DateTime\": \"" + formattedDateTime + "\"," +
    "\"UserData\": \"" + userdata + "\"," +
    "\"OperationalShift\": \"" + shift + "\"," +
    "\"StackBarcodeData\": \"" + barcodeData + "\"," +
    "\"BatteryStackLayerNumber\": \"" + batterystack + "\"," +
    "\"OP190_WIC_Paramerters_Report_1\": \"" + pr1 + "\"," +
    "\"OP190_WIC_Paramerters_Report_2\": \"" + pr2 + "\"," +
    "\"OP190_WIC_Paramerters_Report_3\": \"" + pr3 + "\"," +
    "\"OP190_WIC_Paramerters_Report_4\": \"" + pr4 + "\"," +
    "\"OP190_WIC_Paramerters_Report_5\": \"" + pr5 + "\"," +
    "\"OP190_WIC_Paramerters_Report_6\": \"" + pr6 + "\"," +
    "\"OP190_WIC_Paramerters_Report_7\": \"" + pr7 + "\"," +
    "\"OP190_WIC_Paramerters_Report_8\": \"" + pr8 + "\"," +
    "\"OP190_WIC_Paramerters_Report_9\": \"" + pr9 + "\"," +
    "\"OP190_WIC_Paramerters_Report_10\": \"" + pr10 + "\"," +
    "\"OP190_WIC_Paramerters_Report_11\": \"" + pr11 + "\"," +
    "\"OP190_WIC_Paramerters_Report_12\": \"" + pr12 + "\"," +
    "\"OP190_WIC_Paramerters_Report_13\": \"" + pr13 + "\"," +
    "\"Cell01_Weld_X_Coordinate\": \"" + c1xc + "\"," +
    "\"Cell01_Weld_Y_Coordinate\": \"" + c1yc + "\"," +
    "\"Cell02_Weld_X_Coordinate\": \"" + c2xc + "\"," +
    "\"Cell02_Weld_Y_Coordinate\": \"" + c2yc + "\"," +
    "\"Cell03_Weld_X_Coordinate\": \"" + c3xc + "\"," +
    "\"Cell03_Weld_Y_Coordinate\": \"" + c3yc + "\"," +
    "\"Cell04_Weld_X_Coordinate\": \"" + c4xc + "\"," +
    "\"Cell04_Weld_Y_Coordinate\": \"" + c4yc + "\"," +
    "\"Cell05_Weld_X_Coordinate\": \"" + c5xc + "\"," +
    "\"Cell05_Weld_Y_Coordinate\": \"" + c5yc + "\"," +
    "\"Cell06_Weld_X_Coordinate\": \"" + c6xc + "\"," +
    "\"Cell07_Weld_Y_Coordinate\": \"" + c7yc + "\"," +
    "\"Cell08_Weld_X_Coordinate\": \"" + c8xc + "\"," +
    "\"Cell08_Weld_Y_Coordinate\": \"" + c8yc + "\"," +
    "\"Cell09_Weld_X_Coordinate\": \"" + c9xc + "\"," +
    "\"Cell09_Weld_Y_Coordinate\": \"" + c9yc + "\"," +
    "\"Cell10_Weld_X_Coordinate\": \"" + c10xc + "\"," +
    "\"Cell10_Weld_Y_Coordinate\": \"" + c10yc + "\"," +
    "\"Cell11_Weld_X_Coordinate\": \"" + c11xc + "\"," +
    "\"Cell11_Weld_Y_Coordinate\": \"" + c11yc + "\"," +
    "\"Cell12_Weld_X_Coordinate\": \"" + c12xc + "\"," +
    "\"Cell12_Weld_Y_Coordinate\": \"" + c12yc + "\"," +
    "\"Cell13_Weld_X_Coordinate\": \"" + c13xc + "\"," +
    "\"Cell13_Weld_Y_Coordinate\": \"" + c13yc + "\"" +
     "\"Cell14_Weld_X_Coordinate\": \"" + c14xc + "\"," +
    "\"Cell14_Weld_Y_Coordinate\": \"" + c14yc + "\"" +
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
