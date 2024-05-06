using ActUtlTypeLib;
using ConfigurationBase;
using SOPS.MTConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOPS.MitsuBase
{
    public abstract class MitsuBaseClass
    {

        Stopwatch downtimeStopwatch = new Stopwatch();

        #region Private 
        private readonly int _queryIntervalinMS = 1000;
        private int _PLCLogicalStation;

        private Dictionary<String, Sample> samplesList = new Dictionary<String, Sample>();
        private Dictionary<String, Event> eventsList = new Dictionary<String, Event>();

        ManualResetEvent _oSignalEndThreadEvent = new ManualResetEvent(false);
        bool _endThread = false;

        #endregion
        
        #region Protected Member
        
        protected int IgnoreDowntimeMS = 30 * 1000, MinorDowntimeLimitMS = 60 * 1000 * 3;
        protected Adapter _mAdapter;

        protected ActUtlType _mitsuPLC = new ActUtlType();
        protected Event _mAvail = new Event("avail");
        protected Event _machineLampStatus = new Event("MachineLampStatus");
        protected Event _power = new Event("power");

        protected Event _mMajorDownTime = new Event("DowntimeEndDuration");
        protected Event _mMinorDownTime = new Event("MinorDownTimeEndDuration");

        #endregion

        public GroupElementCollection GroupElementList { get; set; }

        public bool IsConnected { get; protected set; }

        public MitsuBaseClass(int pLCLogicalStation, int adapterPortNumber, int queryIntervalinMS)
        {
            _mAdapter = new Adapter(adapterPortNumber);
            _queryIntervalinMS = queryIntervalinMS;
            //Set the value of 'LogicalStationNumber' to the property.
            _mitsuPLC.ActLogicalStationNumber = pLCLogicalStation;
            _PLCLogicalStation = pLCLogicalStation;

            _endThread = false;
            ///To establish the connection
            Connect();
        }

        protected virtual void Connect()
        {
            IsConnected = (_mitsuPLC.Open() == 0);

            _endThread = false;
        }

        protected void ParamExchangeThread()
        {
            Stopwatch stopwatch = new Stopwatch();
            while (!_endThread)
            {
                if (!IsConnected )
                {
                    try
                    {
                        _mitsuPLC.Close();
                        _mitsuPLC = new ActUtlType();
                        _mitsuPLC.ActLogicalStationNumber = _PLCLogicalStation;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    Console.WriteLine("Attempting to connect to PLCLogicalStation @{0}", _PLCLogicalStation);
                    ///to reconnect
                    Connect();

                    if (!IsConnected)
                    {
                        _machineLampStatus.Value = 3;
                        _power.Value = "OFF";
                        _mAdapter.SendChanged();
                        Console.WriteLine("Cannot connect to PLCLogicalStation @{0}", _PLCLogicalStation);
                        Thread.Sleep(30000);

                        continue;
                    } 
                }

                _power.Value = "ON";
                stopwatch = Stopwatch.StartNew();

                ///Abstrat member to call and Get PLC parameter 
                OnReadPLCData();

                stopwatch.Stop();

                if (_queryIntervalinMS > stopwatch.ElapsedMilliseconds)
                {
                    Thread.Sleep((int)(_queryIntervalinMS - stopwatch.ElapsedMilliseconds));
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            _oSignalEndThreadEvent.Set();
            Close();
            Console.WriteLine("Ending PLC Exchange thread!");
        }

        protected abstract void OnReadPLCData();

        public abstract void StartPLCTimer();

        protected virtual void Disconnect()
        {
            IsConnected = false;
            
        }

        public void StopPLCTimer()
        {
            Console.WriteLine("Complete setup");
            _mAvail.Value = "UNAVAILABLE";
            _mAdapter.Unavailable();
            _mAdapter.SendChanged();

            Close();

            _mAdapter.Stop();
            Thread.Sleep(10);
            _endThread = true;
        }

        private int Close()
        {
            int iReturnCode;
            iReturnCode = _mitsuPLC.Close();
            IsConnected = false;
            return iReturnCode;
        }

        #region PLC Read Data 

        /// <summary>
        /// Get each group data, iterate through items and call get elements
        /// </summary>
        protected void GetPlcGroups()
        {
            if (GroupElementList == null) return;
            
            foreach (GroupElement item in GroupElementList)
            {
                Console.WriteLine("|{0,25}|", "Fetching for " + item.Name);
                GetGroupElements(item);
            }
        }

        /// <summary>
        /// Get each item from the group element, if the item is not present in the subscription list
        /// add it and get the item.
        /// </summary>
        /// <param name="CurrentGroupElement">Current group element.</param>
        private void GetGroupElements(GroupElement CurrentGroupElement)
        {
            string readValue;
            foreach (ParamElement item1 in CurrentGroupElement.Params)
            {
                if (CurrentGroupElement.Name == "SAMPLE")
                {
                    if (samplesList.ContainsKey(item1.Name) == false)
                    {
                        samplesList.Add(item1.Name, new Sample(item1.Name));
                        _mAdapter.AddDataItem(samplesList[item1.Name]);
                        Console.WriteLine("Added new adapter item " + item1.Name);
                    }
                    readValue = GetPLCITem(item1.ParamSyntax, item1.Convertion);
                    if (!IsConnected ) return; ///For disconnect time
                    if (!string.IsNullOrEmpty(readValue))
                    {
                        samplesList[item1.Name].Value = readValue;
                        Console.WriteLine("Read item " + item1.Name + " value " + readValue);
                    }
                }
                if (CurrentGroupElement.Name == "EVENT")
                {
                    if (eventsList.ContainsKey(item1.Name) == false)
                    {
                        eventsList.Add(item1.Name, new Event(item1.Name));
                        _mAdapter.AddDataItem(eventsList[item1.Name]);
                        Console.WriteLine("Added new adapter item " + item1.Name);
                    }
                    readValue = GetPLCITem(item1.ParamSyntax, item1.Convertion);
                    if (!IsConnected) return;
                    if (readValue == "0")
                    {
                        readValue = "OFF";
                        if (item1.ParamSyntax == "" || item1.ParamSyntax == "" || item1.ParamSyntax == "" || item1.ParamSyntax == "")
                        {
                            readValue = "Normal";
                            if (GetPLCITem("", item1.Convertion) == "1")
                            {
                                readValue = "Abnormal";
                            }
                        }
                    }
                    else if (readValue == "1")
                    {
                        readValue = "ON";
                        if (item1.ParamSyntax == "" || item1.ParamSyntax == "" || item1.ParamSyntax == "" || item1.ParamSyntax == "")
                        {
                            readValue = "Fault: Low";
                        }
                    }

                    if (!string.IsNullOrEmpty(readValue))
                    {
                        eventsList[item1.Name].Value = readValue;

                    }
                    Console.WriteLine("Read item " + item1.Name + " value " + readValue);
                }
            }
        }

        /// <summary>
        /// Iterate through each item and get the data, parse the syntax of the item and fetch it according to device
        /// </summary>
        /// <returns>The PLCIT em.</returns>
        /// <param name="currentPLCItem">Current PLCI tem.</param>
        protected string GetPLCITem(string currentPLCItem, string conversion)
        {
            string readValue = string.Empty;
            if (0 < currentPLCItem.IndexOf(','))//"D10,2"
            {
                string[] s = currentPLCItem.Split(',');
                short[] values = new short[Int32.Parse(s[1])];
                if (ReadDeviceBlock(s[0], Int32.Parse(s[1]), out values) == -1)
                {
                    Console.WriteLine("Error in getting parameters values {0}", s[0]);
                    IsConnected = false;
                    return string.Empty;
                }
                else
                {
                    //calling functions.Read values of parameter R18097 is -13107,16902
                    Console.WriteLine("Read values of parameter {0} is {1}", s[0], string.Join(",", values));
                     
                    switch (conversion)
                    {
                        case "short":
                            {
                                readValue = values[0].ToString();
                            }
                            break;
                        case "long":
                            {
                                readValue = this.convertDataTo(values, "long");
                            }
                            break;
                        case "int":
                            {
                                readValue = this.convertDataTo(values, "int");
                            }
                            break;
                        case "bool":
                            {
                                string hexString = short.Parse(values[1].ToString()).ToString("X") + short.Parse(values[0].ToString()).ToString("X");

                                //Console.WriteLine("hexString = {0}", hexString);

                                switch (hexString.Length)
                                {
                                    case 5:
                                        {
                                            hexString += "000";
                                        }
                                        break;
                                    case 6:
                                        {
                                            hexString += "00";
                                        }
                                        break;
                                    case 7:
                                        {
                                            hexString += "0";
                                        }
                                        break;

                                }

                                uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
                                byte[] floatVals = BitConverter.GetBytes(num);
                                float f = BitConverter.ToSingle(floatVals, 0);
                                //Console.WriteLine("Bool float convert = {0}", f);

                                readValue = f.ToString();

                                //Console.WriteLine("Bool readValue = {0}", readValue);

                            }
                            break;
                        case "real":
                            {
                                string hexString = short.Parse(values[1].ToString()).ToString("X") + short.Parse(values[0].ToString()).ToString("X");

                                //Console.WriteLine("hexString = {0}", hexString);

                                switch (hexString.Length)
                                {
                                    case 5:
                                        {
                                            hexString += "000";
                                        }
                                        break;
                                    case 6:
                                        {
                                            hexString += "00";
                                        }
                                        break;
                                    case 7:
                                        {
                                            hexString += "0";
                                        }
                                        break;

                                } 
                                uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
                                byte[] floatVals = BitConverter.GetBytes(num);
                                float f = BitConverter.ToSingle(floatVals, 0);
                                //Console.WriteLine("float convert = {0}", f);

                                readValue = f.ToString("#.####");

                                //Console.WriteLine("readValue = {0}", readValue);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (0 < currentPLCItem.IndexOf('=')) //// "D10=0" and "D10..12=0"
            {
                Console.WriteLine("Write not supported yet!");
            }
            else // "D10"
            {
                int value = 0;
                if (_mitsuPLC.GetDevice(currentPLCItem.ToUpper(), out value) != 0)
                {
                    Console.WriteLine("Error in getting parameters values {0}", currentPLCItem);
                }
                else
                {
                    //Console.WriteLine(currentPLCItem.ToUpper() + "=" + value.ToString(CultureInfo.InvariantCulture));
                    readValue = value.ToString(CultureInfo.InvariantCulture);
                }
            }

            return readValue;
        }

        public string convertDataTo(short[] values, string type2Convert)
        { 
            if (type2Convert == "float" || type2Convert == "real")
            {
                float retVal = SOConverters.ConvertRegistersToFloat(Array.ConvertAll(values, input => (int)input));
                return retVal.ToString("#0.###");
            }
            if (type2Convert == "short")
            {
                return values[0].ToString();
            }
            /*if (type2Convert == "short")
            {
                return values[0].ToString();
            }*/
            if (type2Convert == "int")
            {
                int retVal = SOConverters.ConvertRegistersToInt(Array.ConvertAll(values, input => (int)input));
                return retVal.ToString();
            }
            if (type2Convert == "long")
            {
                long retVal = SOConverters.ConvertRegistersToLong(Array.ConvertAll(values, input => (int)input));
                return retVal.ToString();
            }
            return string.Empty;
        }

        protected int ReadDeviceBlock(string m_Device, int m_Size, out short[] m_Values)
        {
            int iReturnCode = -1;			    //Return code

            ///Assign the array for 'DeviceValue'.
            m_Values = new short[m_Size];

            ///Processing of ReadDeviceBlock2 method
            try
            {
                //The ReadDeviceBlock2 method is executed.
                iReturnCode = _mitsuPLC.ReadDeviceBlock2(m_Device, m_Size,
                                                        out m_Values[0]/*arrDeviceValue*/);
            }
            ///Exception processing			
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return iReturnCode;
            }

            return iReturnCode;
        }

        #endregion

        #region DownTime

        protected void DownTimeManager(bool startTimer)
        {
            //start stopwatch if not started of startTimer is true
            if (startTimer)
            {
                if (!downtimeStopwatch.IsRunning)
                    downtimeStopwatch.Start();
            }
            else
            {
                //stop stopwatch
                downtimeStopwatch.Stop();
                if (downtimeStopwatch.ElapsedMilliseconds <= IgnoreDowntimeMS)
                {

                }
                else if (downtimeStopwatch.ElapsedMilliseconds <= MinorDowntimeLimitMS)
                {
                    //Report Minor Downtime in seconds
                    _mMinorDownTime.Value = downtimeStopwatch.ElapsedMilliseconds / 1000;    // Convert to Seconds
                }
                else
                {
                    // Report Major downtime in seconds
                    _mMajorDownTime.Value = downtimeStopwatch.ElapsedMilliseconds / 1000;
                }

                downtimeStopwatch.Reset();
                return;
            }
        }
        #endregion
    }
}
