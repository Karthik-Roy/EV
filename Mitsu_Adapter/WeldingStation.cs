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

    internal class WeldingStation : MitsuBaseClass
    {
        //Message _mcAlarms = new Message("alarm");
        // Message mPartcount = new Message("part_count_2");
        //Sample mcycletime = new Sample("cycle_time_sec");
        

        Message mWSData = new Message("WS_Data");

        public WeldingStation(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
            _mAdapter.AddDataItem(mWSData);
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


            GetWSData();

            //check machine lamp status for idle if idel start timer

        }

        #region WeldStationData
        private void GetWSData()
        {
            const int userreg = 13624;
            const int opshift = 13641;
            const int barcode = 13658;
            const int zfixbcode = 13675;
            const int batteryweld = 13694;
            const int weldzon = 13711;
            string userdata = string.Empty;
            string shift = string.Empty;
            string barcodeData = string.Empty;
            string zfixationbarcode = string.Empty;
            string batteryweldType = string.Empty;
            string weldzoneName = string.Empty;

            int SI_No = 0;
            _mitsuPLC.GetDevice("D13620", out SI_No);

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


            for (int i = 0; i < 13; i++)
            {
                string barcodee = "D" + (barcode + i);
                barcodeData = barcodeData + GetASCII(barcodee);
            }
            barcodeData = barcodeData.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();


            for (int i = 0; i < 13; i++)
            {
                string zfixb = "D" + (zfixbcode + i);
                zfixationbarcode = zfixationbarcode + GetASCII(zfixb);
            }
            zfixationbarcode = zfixationbarcode.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty).Trim();

            int batterystack = 0;
            _mitsuPLC.GetDevice("D13692", out batterystack);

            for (int i = 0; i < 9; i++)
            {
                string bweldtype = "D" + (batteryweld + i);
                batteryweldType = batteryweldType + GetASCII(bweldtype);
            }
            batteryweldType = batteryweldType.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty);

            for (int i = 0; i < 9; i++)
            {
                string weldzone = "D" + (weldzon + i);
                weldzoneName = weldzoneName + GetASCII(weldzone);
            }
            weldzoneName = weldzoneName.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("NULL", string.Empty);

            int c1ipk = 0;
            _mitsuPLC.GetDevice("D13728", out c1ipk);
            float cell01weldipk = (float)c1ipk;

            int c1irms = 0;
            _mitsuPLC.GetDevice("D13730", out c1irms);
            float cell01weldirms = (float)c1irms;

            int c1upk = 0;
            _mitsuPLC.GetDevice("D13732", out c1upk);
            float cell01weldupk = (float)c1upk;

            int c1urms = 0;
            _mitsuPLC.GetDevice("D13734", out c1urms);
            float cell01weldurms = (float)c1urms;

            int c1s3 = 0;
            _mitsuPLC.GetDevice("D13736", out c1s3);
            float cell01welds3 = (float)c1s3;

            
            int c1x = 0;
            _mitsuPLC.GetDevice("D13738", out c1x);
            float c1xc = (float)c1x;

            int c1y = 0;
            _mitsuPLC.GetDevice("D13740", out c1y);
            float c1yc = (float)c1y;


            // Cell 2
            int c2ipk = 0;
            _mitsuPLC.GetDevice("D13742", out c2ipk);
            float cell02weldipk = (float)c2ipk;

            int c2irms = 0;
            _mitsuPLC.GetDevice("D13744", out c2irms);
            float cell02weldirms = (float)c2irms;

            int c2upk = 0;
            _mitsuPLC.GetDevice("D13746", out c2upk);
            float cell02weldupk = (float)c2upk;

            int c2urms = 0;
            _mitsuPLC.GetDevice("D13748", out c2urms);
            float cell02weldurms = (float)c2urms;

            int c2s3 = 0;
            _mitsuPLC.GetDevice("D13750", out c2s3);
            float cell02welds3 = (float)c2s3;

            int c2x = 0;
            _mitsuPLC.GetDevice("D13752", out c2x);
            float c2xc = (float)c2x;

            int c2y = 0;
            _mitsuPLC.GetDevice("D13754", out c2y);
            float c2yc = (float)c2y;
            

            // Cell 3
            int c3ipk = 0;
            _mitsuPLC.GetDevice("D13756", out c3ipk);
            float cell03weldipk = (float)c3ipk;

            int c3irms = 0;
            _mitsuPLC.GetDevice("D13758", out c3irms);
            float cell03weldirms = (float)c3irms;

            int c3upk = 0;
            _mitsuPLC.GetDevice("D13760", out c3upk);
            float cell03weldupk = (float)c3upk;

            int c3urms = 0;
            _mitsuPLC.GetDevice("D13762", out c3urms);
            float cell03weldurms = (float)c3urms;

            int c3s3 = 0;
            _mitsuPLC.GetDevice("D13764", out c3s3);
            float cell03welds3 = (float)c3s3;

            int c3x = 0;
            _mitsuPLC.GetDevice("D13766", out c3x);
            float c3xc = (float)c3x;

            int c3y = 0;
            _mitsuPLC.GetDevice("D13768", out c3y);
            float c3yc = (float)c3y;

            // Cell 4
            int c4ipk = 0;
            _mitsuPLC.GetDevice("D13770", out c4ipk);
            float cell04weldipk = (float)c4ipk;

            int c4irms = 0;
            _mitsuPLC.GetDevice("D13772", out c4irms);
            float cell04weldirms = (float)c4irms;

            int c4upk = 0;
            _mitsuPLC.GetDevice("D13774", out c4upk);
            float cell04weldupk = (float)c4upk;

            int c4urms = 0;
            _mitsuPLC.GetDevice("D13776", out c4urms);
            float cell04weldurms = (float)c4urms;

            int c4s3 = 0;
            _mitsuPLC.GetDevice("D13778", out c4s3);
            float cell04welds3 = (float)c4s3;

            int c4x = 0;
            _mitsuPLC.GetDevice("D13780", out c4x);
            float c4xc = (float)c4x;

            int c4y = 0;
            _mitsuPLC.GetDevice("D13782", out c4y);
            float c4yc = (float)c4y;

            // Cell 5
            int c5ipk = 0;
            _mitsuPLC.GetDevice("D13784", out c5ipk);
            float cell05weldipk = (float)c5ipk;

            int c5irms = 0;
            _mitsuPLC.GetDevice("D13786", out c5irms);
            float cell05weldirms = (float)c5irms;

            int c5upk = 0;
            _mitsuPLC.GetDevice("D13788", out c5upk);
            float cell05weldupk = (float)c5upk;

            int c5urms = 0;
            _mitsuPLC.GetDevice("D13790", out c5urms);
            float cell05weldurms = (float)c5urms;

            int c5s3 = 0;
            _mitsuPLC.GetDevice("D13792", out c5s3);
            float cell05welds3 = (float)c5s3;

            int c5x = 0;
            _mitsuPLC.GetDevice("D13794", out c5x);
            float c5xc = (float)c5x;

            int c5y = 0;
            _mitsuPLC.GetDevice("D13796", out c5y);
            float c5yc = (float)c5y;

            // Cell 6
            int c6ipk = 0;
            _mitsuPLC.GetDevice("D13798", out c6ipk);
            float cell06weldipk = (float)c6ipk;

            int c6irms = 0;
            _mitsuPLC.GetDevice("D13800", out c6irms);
            float cell06weldirms = (float)c6irms;

            int c6upk = 0;
            _mitsuPLC.GetDevice("D13802", out c6upk);
            float cell06weldupk = (float)c6upk;

            int c6urms = 0;
            _mitsuPLC.GetDevice("D13804", out c6urms);
            float cell06weldurms = (float)c6urms;

            int c6s3 = 0;
            _mitsuPLC.GetDevice("D13806", out c6s3);
            float cell06welds3 = (float)c6s3;

            int c6x = 0;
            _mitsuPLC.GetDevice("D13808", out c6x);
            float c6xc = (float)c6x;

            int c6y = 0;
            _mitsuPLC.GetDevice("D13810", out c6y);
            float c6yc = (float)c6y;

            // Cell 7
            int c7ipk = 0;
            _mitsuPLC.GetDevice("D13812", out c7ipk);
            float cell07weldipk = (float)c7ipk;

            int c7irms = 0;
            _mitsuPLC.GetDevice("D13814", out c7irms);
            float cell07weldirms = (float)c7irms;

            int c7upk = 0;
            _mitsuPLC.GetDevice("D13816", out c7upk);
            float cell07weldupk = (float)c7upk;

            int c7urms = 0;
            _mitsuPLC.GetDevice("D13818", out c7urms);
            float cell07weldurms = (float)c7urms;

            int c7s3 = 0;
            _mitsuPLC.GetDevice("D13820", out c7s3);
            float cell07welds3 = (float)c7s3;

            int c7x = 0;
            _mitsuPLC.GetDevice("D13822", out c7x);
            float c7xc = (float)c7x;

            int c7y = 0;
            _mitsuPLC.GetDevice("D13824", out c7y);
            float c7yc = (float)c7y;

            // Cell 8
            int c8ipk = 0;
            _mitsuPLC.GetDevice("D13826", out c8ipk);
            float cell08weldipk = (float)c8ipk;

            int c8irms = 0;
            _mitsuPLC.GetDevice("D13828", out c8irms);
            float cell08weldirms = (float)c8irms;

            int c8upk = 0;
            _mitsuPLC.GetDevice("D13830", out c8upk);
            float cell08weldupk = (float)c8upk;

            int c8urms = 0;
            _mitsuPLC.GetDevice("D13832", out c8urms);
            float cell08weldurms = (float)c8urms;

            int c8s3 = 0;
            _mitsuPLC.GetDevice("D13834", out c8s3);
            float cell08welds3 = (float)c8s3;

            int c8x = 0;
            _mitsuPLC.GetDevice("D13836", out c8x);
            float c8xc = (float)c8x;

            int c8y = 0;
            _mitsuPLC.GetDevice("D13838", out c8y);
            float c8yc = (float)c8y;

            // Cell 9
            int c9ipk = 0;
            _mitsuPLC.GetDevice("D13840", out c9ipk);
            float cell09weldipk = (float)c9ipk;

            int c9irms = 0;
            _mitsuPLC.GetDevice("D13842", out c9irms);
            float cell09weldirms = (float)c9irms;

            int c9upk = 0;
            _mitsuPLC.GetDevice("D13844", out c9upk);
            float cell09weldupk = (float)c9upk;

            int c9urms = 0;
            _mitsuPLC.GetDevice("D13846", out c9urms);
            float cell09weldurms = (float)c9urms;

            int c9s3 = 0;
            _mitsuPLC.GetDevice("D13848", out c9s3);
            float cell09welds3 = (float)c9s3;

            int c9x = 0;
            _mitsuPLC.GetDevice("D13850", out c9x);
            float c9xc = (float)c9x;

            int c9y = 0;
            _mitsuPLC.GetDevice("D13852", out c9y);
            float c9yc = (float)c9y;

            // Cell 10
            int c10ipk = 0;
            _mitsuPLC.GetDevice("D13854", out c10ipk);
            float cell10weldipk = (float)c10ipk;

            int c10irms = 0;
            _mitsuPLC.GetDevice("D13856", out c10irms);
            float cell10weldirms = (float)c10irms;

            int c10upk = 0;
            _mitsuPLC.GetDevice("D13858", out c10upk);
            float cell10weldupk = (float)c10upk;

            int c10urms = 0;
            _mitsuPLC.GetDevice("D13860", out c10urms);
            float cell10weldurms = (float)c10urms;

            int c10s3 = 0;
            _mitsuPLC.GetDevice("D13862", out c10s3);
            float cell10welds3 = (float)c10s3;

            int c10x = 0;
            _mitsuPLC.GetDevice("D13864", out c10x);
            float c10xc = (float)c10x;

            int c10y = 0;
            _mitsuPLC.GetDevice("D13866", out c10y);
            float c10yc = (float)c10y;

            // Cell 11
            int c11ipk = 0;
            _mitsuPLC.GetDevice("D13868", out c11ipk);
            float cell11weldipk = (float)c11ipk;

            int c11irms = 0;
            _mitsuPLC.GetDevice("D13870", out c11irms);
            float cell11weldirms = (float)c11irms;

            int c11upk = 0;
            _mitsuPLC.GetDevice("D13872", out c11upk);
            float cell11weldupk = (float)c11upk;

            int c11urms = 0;
            _mitsuPLC.GetDevice("D13874", out c11urms);
            float cell11weldurms = (float)c11urms;

            int c11s3 = 0;
            _mitsuPLC.GetDevice("D13876", out c11s3);
            float cell11welds3 = (float)c11s3;

            int c11x = 0;
            _mitsuPLC.GetDevice("D13878", out c11x);
            float c11xc = (float)c11x;

            int c11y = 0;
            _mitsuPLC.GetDevice("D13880", out c11y);
            float c11yc = (float)c11y;

            // Cell 12
            int c12ipk = 0;
            _mitsuPLC.GetDevice("D13882", out c12ipk);
            float cell12weldipk = (float)c12ipk;

            int c12irms = 0;
            _mitsuPLC.GetDevice("D13884", out c12irms);
            float cell12weldirms = (float)c12irms;

            int c12upk = 0;
            _mitsuPLC.GetDevice("D13886", out c12upk);
            float cell12weldupk = (float)c12upk;

            int c12urms = 0;
            _mitsuPLC.GetDevice("D13888", out c12urms);
            float cell12weldurms = (float)c12urms;

            int c12s3 = 0;
            _mitsuPLC.GetDevice("D13890", out c12s3);
            float cell12welds3 = (float)c12s3;

            int c12x = 0;
            _mitsuPLC.GetDevice("D13892", out c12x);
            float c12xc = (float)c12x;

            int c12y = 0;
            _mitsuPLC.GetDevice("D13894", out c12y);
            float c12yc = (float)c12y;

            // Cell 13
            int c13ipk = 0;
            _mitsuPLC.GetDevice("D13896", out c13ipk);
            float cell13weldipk = (float)c13ipk;

            int c13irms = 0;
            _mitsuPLC.GetDevice("D13898", out c13irms);
            float cell13weldirms = (float)c13irms;

            int c13upk = 0;
            _mitsuPLC.GetDevice("D13900", out c13upk);
            float cell13weldupk = (float)c13upk;

            int c13urms = 0;
            _mitsuPLC.GetDevice("D13902", out c13urms);
            float cell13weldurms = (float)c13urms;

            int c13s3 = 0;
            _mitsuPLC.GetDevice("D13904", out c13s3);
            float cell13welds3 = (float)c13s3;

            int c13x = 0;
            _mitsuPLC.GetDevice("D13906", out c13x);
            float c13xc = (float)c13x;

            int c13y = 0;
            _mitsuPLC.GetDevice("D13908", out c13y);
            float c13yc = (float)c13y;

            // Cell 14
            int c14ipk = 0;
            _mitsuPLC.GetDevice("D13910", out c14ipk);
            float cell14weldipk = (float)c14ipk;

            int c14irms = 0;
            _mitsuPLC.GetDevice("D13912", out c14irms);
            float cell14weldirms = (float)c14irms;

            int c14upk = 0;
            _mitsuPLC.GetDevice("D13914", out c14upk);
            float cell14weldupk = (float)c14upk;

            int c14urms = 0;
            _mitsuPLC.GetDevice("D13916", out c14urms);
            float cell14weldurms = (float)c14urms;

            int c14s3 = 0;
            _mitsuPLC.GetDevice("D13918", out c14s3);
            float cell14welds3 = (float)c14s3;

            int c14x = 0;
            _mitsuPLC.GetDevice("D13920", out c14x);
            float c14xc = (float)c14x;

            int c14y = 0;
            _mitsuPLC.GetDevice("D13922", out c14y);
            float c14yc = (float)c14y;

            // Cell 15
            int c15ipk = 0;
            _mitsuPLC.GetDevice("D13924", out c15ipk);
            float cell15weldipk = (float)c15ipk;

            int c15irms = 0;
            _mitsuPLC.GetDevice("D13926", out c15irms);
            float cell15weldirms = (float)c15irms;

            int c15upk = 0;
            _mitsuPLC.GetDevice("D13928", out c15upk);
            float cell15weldupk = (float)c15upk;

            int c15urms = 0;
            _mitsuPLC.GetDevice("D13930", out c15urms);
            float cell15weldurms = (float)c15urms;

            int c15s3 = 0;
            _mitsuPLC.GetDevice("D13932", out c15s3);
            float cell15welds3 = (float)c15s3;

            int c15x = 0;
            _mitsuPLC.GetDevice("D13934", out c15x);
            float c15xc = (float)c15x;

            int c15y = 0;
            _mitsuPLC.GetDevice("D13936", out c15y);
            float c15yc = (float)c15y;

            int electrodecurrentc = 0;
            _mitsuPLC.GetDevice("D13938", out electrodecurrentc);
            float ecc = (float)electrodecurrentc;

            int electrodesv = 0;
            _mitsuPLC.GetDevice("D13940", out electrodesv);
            float esv = (float)electrodesv;



            mWSData.Value = "{" +
    "\"SI_No\": \"" + SI_No + "\"," +
    "\"DateTime\": \"" + formattedDateTime + "\"," +
    "\"UserData\": \"" + userdata + "\"," +
    "\"OperationalShift\": \"" + shift + "\"," +
    "\"StackBarcodeData\": \"" + barcodeData + "\"," +
    "\"Z_FixationBarcode\": \"" + zfixationbarcode + "\"," +
    "\"BatteryStackLayerNumber\": \"" + batterystack + "\"," +
    "\"BatteryWeldType\": \"" + batteryweldType + "\"," +
    "\"WeldingZoneName\": \"" + weldzoneName + "\"," +
    "\"Cell01_Weld_Ipk\": \"" + c1ipk + "\"," +
    "\"Cell01_Weld_Irms\": \"" + c1irms + "\"," +
    "\"Cell01_Weld_Upk\": \"" + c1upk + "\"," +
    "\"Cell01_Weld_Urms\": \"" + c1urms + "\"," +
    "\"Cell01_Weld_S3\": \"" + c1s3 + "\"," +
    "\"Cell01_Weld_X_Coordinate\": \"" + c1xc + "\"," +
    "\"Cell01_Weld_Y_Coordinate\": \"" + c1yc + "\"," +
    "\"Cell02_Weld_Ipk\": \"" + c2ipk + "\"," +
    "\"Cell02_Weld_Irms\": \"" + c2irms + "\"," +
    "\"Cell02_Weld_Upk\": \"" + c2upk + "\"," +
    "\"Cell02_Weld_Urms\": \"" + c2urms + "\"," +
    "\"Cell02_Weld_S3\": \"" + c2s3 + "\"," +
    "\"Cell02_Weld_X_Coordinate\": \"" + c2xc + "\"," +
    "\"Cell02_Weld_Y_Coordinate\": \"" + c2yc + "\"," +
    "\"Cell03_Weld_Ipk\": \"" + c3ipk + "\"," +
    "\"Cell03_Weld_Irms\": \"" + c3irms + "\"," +
    "\"Cell03_Weld_Upk\": \"" + c3upk + "\"," +
    "\"Cell03_Weld_Urms\": \"" + c3urms + "\"," +
    "\"Cell03_Weld_S3\": \"" + c3s3 + "\"," +
    "\"Cell03_Weld_X_Coordinate\": \"" + c3xc + "\"," +
    "\"Cell03_Weld_Y_Coordinate\": \"" + c3yc + "\"," +
    "\"Cell04_Weld_Ipk\": \"" + c4ipk + "\"," +
    "\"Cell04_Weld_Irms\": \"" + c4irms + "\"," +
    "\"Cell04_Weld_Upk\": \"" + c4upk + "\"," +
    "\"Cell04_Weld_Urms\": \"" + c4urms + "\","+
    "\"Cell04_Weld_S3\": \"" + c4s3 + "\"," +
    "\"Cell04_Weld_X_Coordinate\": \"" + c4xc + "\"," +
    "\"Cell04_Weld_Y_Coordinate\": \"" + c4yc + "\"," +
    "\"Cell05_Weld_Ipk\": \"" + c5ipk + "\"," +
    "\"Cell05_Weld_Irms\": \"" + c5irms + "\"," +
    "\"Cell05_Weld_Upk\": \"" + c5upk + "\"," +
    "\"Cell05_Weld_Urms\": \"" + c5urms + "\"," +
    "\"Cell05_Weld_S3\": \"" + c5s3 + "\"," +
    "\"Cell05_Weld_X_Coordinate\": \"" + c5xc + "\"," +
    "\"Cell05_Weld_Y_Coordinate\": \"" + c5yc + "\"," +
    "\"Cell06_Weld_Ipk\": \"" + c6ipk + "\"," +
    "\"Cell06_Weld_Irms\": \"" + c6irms + "\"," +
    "\"Cell06_Weld_Upk\": \"" + c6upk + "\"," +
    "\"Cell06_Weld_Urms\": \"" + c6urms + "\"," +
    "\"Cell06_Weld_S3\": \"" + c6s3 + "\"," +
    "\"Cell06_Weld_X_Coordinate\": \"" + c6xc + "\"," +
    "\"Cell06_Weld_Y_Coordinate\": \"" + c6yc + "\"," +
    "\"Cell07_Weld_Ipk\": \"" + c7ipk + "\"," +
    "\"Cell07_Weld_Irms\": \"" + c7irms + "\"," +
    "\"Cell07_Weld_Upk\": \"" + c7upk + "\"," +
    "\"Cell07_Weld_Urms\": \"" + c7urms + "\"," +
    "\"Cell07_Weld_S3\": \"" + c7s3 + "\"," +
    "\"Cell07_Weld_X_Coordinate\": \"" + c7xc + "\"," +
    "\"Cell07_Weld_Y_Coordinate\": \"" + c7yc + "\"," +
    "\"Cell08_Weld_Ipk\": \"" + c8ipk + "\"," +
    "\"Cell08_Weld_Irms\": \"" + c8irms + "\"," +
    "\"Cell08_Weld_Upk\": \"" + c8upk + "\"," +
    "\"Cell08_Weld_Urms\": \"" + c8urms + "\"," +
    "\"Cell08_Weld_S3\": \"" + c8s3 + "\"," +
    "\"Cell08_Weld_X_Coordinate\": \"" + c8xc + "\"," +
    "\"Cell08_Weld_Y_Coordinate\": \"" + c8yc + "\"," +
    "\"Cell09_Weld_Ipk\": \"" + c9ipk + "\"," +
    "\"Cell09_Weld_Irms\": \"" + c9irms + "\"," +
    "\"Cell09_Weld_Upk\": \"" + c9upk + "\"," +
    "\"Cell09_Weld_Urms\": \"" + c9urms + "\"," +
    "\"Cell09_Weld_S3\": \"" + c9s3 + "\"," +
    "\"Cell09_Weld_X_Coordinate\": \"" + c9xc + "\"," +
    "\"Cell09_Weld_Y_Coordinate\": \"" + c9yc + "\"," +
    "\"Cell10_Weld_Ipk\": \"" + c10ipk + "\"," +
    "\"Cell10_Weld_Irms\": \"" + c10irms + "\"," +
    "\"Cell10_Weld_Upk\": \"" + c10upk + "\"," +
    "\"Cell10_Weld_Urms\": \"" + c10urms + "\"," +
    "\"Cell10_Weld_S3\": \"" + c10s3 + "\"," +
    "\"Cell10_Weld_X_Coordinate\": \"" + c10xc + "\"," +
    "\"Cell10_Weld_Y_Coordinate\": \"" + c10yc + "\"," +
    "\"Cell11_Weld_Ipk\": \"" + c11ipk + "\"," +
    "\"Cell11_Weld_Irms\": \"" + c11irms + "\"," +
    "\"Cell11_Weld_Upk\": \"" + c11upk + "\"," +
    "\"Cell11_Weld_Urms\": \"" + c11urms + "\"," +
    "\"Cell11_Weld_S3\": \"" + c11s3 + "\"," +
    "\"Cell11_Weld_X_Coordinate\": \"" + c11xc + "\"," +
    "\"Cell11_Weld_Y_Coordinate\": \"" + c11yc + "\"," +
    "\"Cell12_Weld_Ipk\": \"" + c12ipk + "\"," +
    "\"Cell12_Weld_Irms\": \"" + c12irms + "\"," +
    "\"Cell12_Weld_Upk\": \"" + c12upk + "\"," +
    "\"Cell12_Weld_Urms\": \"" + c12urms + "\"," +
    "\"Cell12_Weld_S3\": \"" + c12s3 + "\"," +
    "\"Cell12_Weld_X_Coordinate\": \"" + c12xc + "\"," +
    "\"Cell12_Weld_Y_Coordinate\": \"" + c12yc + "\"," +
    "\"Cell13_Weld_Ipk\": \"" + c13ipk + "\"," +
    "\"Cell13_Weld_Irms\": \"" + c13irms + "\"," +
    "\"Cell13_Weld_Upk\": \"" + c13upk + "\"," +
    "\"Cell13_Weld_Urms\": \"" + c13urms + "\"," +
    "\"Cell13_Weld_S3\": \"" + c13s3 + "\"," +
    "\"Cell13_Weld_X_Coordinate\": \"" + c13xc + "\"," +
    "\"Cell13_Weld_Y_Coordinate\": \"" + c13yc + "\"," +
    "\"Cell14_Weld_Ipk\": \"" + c14ipk + "\"," +
    "\"Cell14_Weld_Irms\": \"" + c14irms + "\"," +
    "\"Cell14_Weld_Upk\": \"" + c14upk + "\"," +
    "\"Cell14_Weld_Urms\": \"" + c14urms + "\"," +
    "\"Cell14_Weld_S3\": \"" + c14s3 + "\"," +
    "\"Cell14_Weld_X_Coordinate\": \"" + c14xc + "\"," +
    "\"Cell14_Weld_Y_Coordinate\": \"" + c14yc + "\"," +
    "\"Cell15_Weld_Ipk\": \"" + c15ipk + "\"," +
    "\"Cell15_Weld_Irms\": \"" + c15irms + "\"," +
    "\"Cell15_Weld_Upk\": \"" + c15upk + "\"," +
    "\"Cell15_Weld_Urms\": \"" + c15urms + "\"," +
    "\"Cell15_Weld_S3\": \"" + c15s3 + "\"," +
    "\"Cell15_Weld_X_Coordinate\": \"" + c15xc + "\"," +
    "\"Cell15_Weld_Y_Coordinate\": \"" + c15yc + "\"," +

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
