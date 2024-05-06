using SOPS.MitsuBase;
using SOPS.MTConnect;
using System;
using System.Threading;

namespace SOPS.Mitsu_Adapter
{
    internal class Adapter_15tonCrankPinPress_EA : MitsuBaseClass
    {

        Message _mcAlarms = new Message("alarm");
        Message mPartcount = new Message("part_count_2");
        Sample mcycletime = new Sample("cycle_time_sec");
        Message mStatusparam = new Message("status_param");
        Message mSetting = new Message("setting");

        public Adapter_15tonCrankPinPress_EA(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS) : base(pLCLogicalStation, adapterPortNumber, queryIntervalinMS)
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
            _mAdapter.AddDataItem(_mcAlarms);
            _mAdapter.AddDataItem(_machineLampStatus);
            _mAdapter.AddDataItem(_mMajorDownTime);
            _mAdapter.AddDataItem(_mMinorDownTime);
            _mAdapter.AddDataItem(mPartcount);
            _mAdapter.AddDataItem(mStatusparam);
            _mAdapter.AddDataItem(mSetting);
            _mAdapter.AddDataItem(mcycletime);
            _machineLampStatus.Value = 3;
            _mMajorDownTime.Value = 0;
            _mMinorDownTime.Value = 0;

            Thread t = new Thread(new ThreadStart(ParamExchangeThread));
            t.Start();
            t.IsBackground = true;

        }



        private void GetInternalVariables()
        {
            GetMachineLampStatus();
            if (((int)_machineLampStatus.Value == 2) || ((int)_machineLampStatus.Value == 4))
            {
                DownTimeManager(true);
            }
            else if ((int)_machineLampStatus.Value == 0)  //check machine lamp status for green auto , if auto stop timer
            {
                DownTimeManager(false);
            }
            GetAlarms();
            if (!IsConnected) return;

            GetPartCount();
            GetStatusParam();
            GetSetting();

            //check machine lamp status for idle if idel start timer

        }

        private void GetPartCount()
        {
            int partcount = 0;
            if (_mitsuPLC.GetDevice("D114", out partcount) != 0) return;
            mPartcount.Value = "{\"part_count\":" + partcount + ",\"program_name\":\"-1\"}";

            int cycletime = 0;
            if (_mitsuPLC.GetDevice("D120", out cycletime) != 0) return;
            float cycletimesec = Convert.ToSingle(cycletime) / 10;
            mcycletime.Value = cycletimesec;
        }

        private void GetStatusParam()
        {
            /*short[] load = new short[2];
            ReadDeviceBlock("D20", 2, out load);
            string curload = this.convertDataTo(load, "float");*/
            /*short[] ramps = new short[2];
            ReadDeviceBlock("D204", 2, out ramps);
            string rampos = this.convertDataTo(ramps, "float");

            short[] ramld = new short[2];
            ReadDeviceBlock("D30", 2, out ramld);
            string ramload = this.convertDataTo(ramld, "float");

            short[] lastload = new short[2];
            ReadDeviceBlock("D30", 2, out lastload);
            string lasload = this.convertDataTo(lastload, "float");*/
            try
            {
                int curload = 0;
                if (_mitsuPLC.GetDevice("D20", out curload) != 0) return;
                float currentload = Convert.ToSingle(curload) / 100;

                int curpressure = 0;
                if (_mitsuPLC.GetDevice("D40", out curpressure) != 0) return;
                float currentpressure = Convert.ToSingle(curpressure) / 10;

                int rampos = 0;
                if (_mitsuPLC.GetDevice("D204", out rampos) != 0) return;
                float ramposition = Convert.ToSingle(rampos) / 10;

                int rmload = 0;
                if (_mitsuPLC.GetDevice("D30", out rmload) != 0) return;
                float ramload = Convert.ToSingle(rmload) / 10;

                int lasload = 0;
                if (_mitsuPLC.GetDevice("D30", out lasload) != 0) return;
                float lastload = Convert.ToSingle(lasload) / 100;

                int lastpressure = 0;
                if (_mitsuPLC.GetDevice("D1500", out lastpressure) != 0) return;
                float laspressure = Convert.ToSingle(lastpressure) / 10;

                mStatusparam.Value = "{\"CurrentLoad\":" + currentload + ",\"CurrentPressure\":" + currentpressure + ",\"RamPosition\":" + ramposition + ",\"RamLoad\":"
                                        + ramload + ",\"LastLoad\":" + lastload + ",\"LastPressure\":" + laspressure + "}";
            }
            catch(Exception ex)
            {

                Console.WriteLine(ex);

            }
        }

        private void GetSetting()
        {

            /*short[] rammhom = new short[2];
            ReadDeviceBlock("D3002", 2, out rammhom);
            string rammhomepos = this.convertDataTo(rammhom, "float");

            short[] ramfeed = new short[2];
            ReadDeviceBlock("D3004", 2, out ramfeed);
            string ramfeedstrt = this.convertDataTo(ramfeed, "float");

            short[] ramstrk = new short[2];
            ReadDeviceBlock("D3006", 2, out ramstrk);
            string ramstrklimt = this.convertDataTo(ramstrk, "float");

            short[] lwload = new short[2];
            ReadDeviceBlock("D3018", 2, out lwload);
            string lowload = this.convertDataTo(lwload, "float");

            short[] hiload = new short[2];
            ReadDeviceBlock("D3020", 2, out hiload);
            string highload = this.convertDataTo(hiload, "float");

            short[] ramprs = new short[2];
            ReadDeviceBlock("D3008", 2, out ramprs);
            string rampress = this.convertDataTo(ramprs, "float");*/
            try
            {
                int rammhomepos = 0;
                if (_mitsuPLC.GetDevice("D3002", out rammhomepos) != 0) return;
                float ramhomepos = Convert.ToSingle(rammhomepos) / 10;

                int dwelltime = 0;
                if (_mitsuPLC.GetDevice("D3000", out dwelltime) != 0) return;
                float dweltime = Convert.ToSingle(dwelltime) / 10;

                int ramfeedstrt = 0;
                if (_mitsuPLC.GetDevice("D3004", out ramfeedstrt) != 0) return;
                float ramfedstart = Convert.ToSingle(ramfeedstrt) / 10;

                int ramstrklimt = 0;
                if (_mitsuPLC.GetDevice("D3006", out ramstrklimt) != 0) return;
                float ramstrokelimit = Convert.ToSingle(ramstrklimt) / 10;

                int lowload = 0;
                if (_mitsuPLC.GetDevice("D3018", out lowload) != 0) return;
                float lwload = Convert.ToSingle(lowload) / 100;

                int highload = 0;
                if (_mitsuPLC.GetDevice("D3020", out highload) != 0) return;
                float hiload = Convert.ToSingle(highload) / 100;

                int rampress = 0;
                if (_mitsuPLC.GetDevice("D3008", out rampress) != 0) return;

                mSetting.Value = "{\"RamHomePosition\":" + ramhomepos + ",\"RamFeedStart\":" + ramfedstart + ",\"RamStrokeLimit\":" + ramstrokelimit + ",\"DwellTime\":"
                                   + dweltime + ",\"LoadLowLimit\":" + lwload + ",\"LoadHighLimit\":" + hiload + ",\"RamPressure\":" + rampress + "}";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void GetAlarms()
        {
            string alarmText = string.Empty;
            short[] values = new short[2];
            if (ReadDeviceBlock("M288", 2, out values) != 0)
            {
                Console.WriteLine("Cannot read Parameter M80");
                IsConnected = false;
                return;

            }
            else
            {
                if ((values[0] & 0x1000) == 0x1000)
                {
                    alarmText = "OIL LEVEL LOW";
                }
                if ((values[0] & 0x2000) == 0x2000)
                {
                    alarmText = "MPCB TRIP";
                }
                if ((values[0] & 0x4000) == 0x4000)
                {
                    alarmText = "SAFETY LIGHT CURTAIN DISTURBED";
                }
                if ((values[0] & 0x8000) == 0x8000)
                {
                    alarmText = "DWELL PUSH BUTTON FAULT ";
                }
                if ((values[1] & 0x01) == 0x01)
                {
                    alarmText = "CHECK PRESSURE NOT REACHED AT LOW LIMIT";
                }
                if ((values[1] & 0x02) == 0x02)
                {
                    alarmText = "CHECK HOME PROXI. SW. ";
                }
                if ((values[1] & 0x04) == 0x04)
                {
                    alarmText = "WRONG MODE SELECTED";
                }
                if ((values[1] & 0x08) == 0x08)
                {
                    alarmText = "EMG.PRESSED";
                }
                if ((values[1] & 0x10) == 0x10)
                {
                    alarmText = "PUMP NOT RUNNING ";
                }
                if ((values[1] & 0x20) == 0x20)
                {
                    alarmText = "HYD. POWER SAVER TIME OVER ";
                }
                if ((values[1] & 0x40) == 0x40)
                {
                    alarmText = "CONTROL ON NOT OK";
                }
                if ((values[1] & 0x80) == 0x80)
                {
                    alarmText = "CHECK HOME POSITION";
                }
                if ((values[1] & 0x100) == 0x100)
                {
                    alarmText = "CHECK LUBRICANT IN LUB. TANK";
                }
                if ((values[1] & 0x200) == 0x200)
                {
                    alarmText = "LUB. MPCB TRIP";
                }
                if ((values[1] & 0x400) == 0x400)
                {
                    alarmText = "SAFETY LIGHT CURTAIN DISTURBED";
                }
                if ((values[1] & 0x800) == 0x800)
                {
                    alarmText = "CHECK LUB. FLOAT SW.";
                }
                if ((values[1] & 0x1000) == 0x1000)
                {
                    alarmText = "CHECK LUB. PR. SWITCH";
                }
                if ((values[1] & 0x2000) == 0x2000)
                {
                    alarmText = "HYD. OIL TEMP HIGH ";
                }
                if ((values[1] & 0x4000) == 0x4000)
                {
                    alarmText = "STROKE LIMIT DIST. FAULT";
                }
                if ((values[1] & 0x8000) == 0x8000)
                {
                    alarmText = "LOAD LIMIT LOW";
                }

            }
            short[] values1 = new short[2];
            if (ReadDeviceBlock("M320", 2, out values1) !=0)
            {
                Console.WriteLine("Cannot read Parameter M320");
                IsConnected = false;
                return;
            }
            if ((values1[0] & 0x01) == 0x01)
            {
                alarmText = "LOAD LIMIT HIGH";
            }
            if ((values1[0] & 0x01) == 0x01)
            {
                alarmText = "CHECK PRESSURE LIMIT HIGH";
            }
            if (!string.IsNullOrEmpty(alarmText))
            {
                _mcAlarms.Value = "[{\"code\":\"-1\",\"descr\":\"" + alarmText + "\" }]";
            }
        }

        private void GetMachineLampStatus()
        {
            
            int retValuesalues;
            if (_mitsuPLC.GetDevice("Y7", out retValuesalues) != 0)
            {
                IsConnected = false;
                return;
            }

            if (retValuesalues == 1)
            {
                _machineLampStatus.Value = 1; //ALARM

            }

            else if (_mitsuPLC.GetDevice("Y5", out retValuesalues) == 0 && retValuesalues == 1)
                _machineLampStatus.Value = 0; //AUTOMATIC

            else if (_mitsuPLC.GetDevice("Y10", out retValuesalues) == 0 && retValuesalues == 1)
                _machineLampStatus.Value = 2; //IDLE
            else
                _machineLampStatus.Value = 3;
        }
    }
}
