using ConfigurationBase;
using SOPS.MitsuBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOPS.Mitsu_Adapter
{
    class Program
    {
        static void Main(string[] args)
        {
            var adapterPort = Int32.Parse(ConfigurationManager.AppSettings["adapterPort"]);
            var queryIntervalinMS = Int32.Parse(ConfigurationManager.AppSettings["waitBeforeNextSendMS"]);
            var pLCLogicalStation = Int32.Parse(ConfigurationManager.AppSettings["PLCLogicalStation"]);

            MitsuBaseClass mPLC = CreateAdapterType(pLCLogicalStation, adapterPort, queryIntervalinMS);
            PLCGroups group = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).SectionGroups["mtcadapter"] as PLCGroups;
            if (group != null && group.Sections != null)
            {
                foreach (ConfigurationSection section in group.Sections)
                {
                    Console.WriteLine("\n\n===========================================");
                    Console.WriteLine(section.SectionInformation.Name.ToUpper());
                    Console.WriteLine("===========================================");
                    if (section.GetType() == typeof(GroupSection))
                    {
                        GroupSection c = (GroupSection)section;
                        GroupElementCollection coll = c.Groups;

                        mPLC.GroupElementList = coll;

                        foreach (GroupElement item in coll)
                        {
                            Console.WriteLine("|{0,25}|", item.Name);
                            Console.WriteLine("-------------------------------------------");
                            foreach (ParamElement item1 in item.Params)
                            {
                                Console.WriteLine("|{0,25} | {1,6}|{2,6}|", item1.Name, item1.ParamSyntax, item1.Convertion);
                                Console.WriteLine("-------------------------------------------");
                            }
                        }
                    }
                }
            }

            mPLC.StartPLCTimer();
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("-------Press a Key to Stop Adapter---------");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("-------------------------------------------");
            Console.ReadLine();
            mPLC.StopPLCTimer();

        }

        private static MitsuBaseClass CreateAdapterType(int pLCLogicalStation, int adapterPort, int queryIntervalinMS)
        {
            var machineType = ConfigurationManager.AppSettings["MachineType"];
            Console.WriteLine("================MachineType :: {0}================", machineType);
            switch (machineType)
            {
                case "Adapter_15tonCrankPinPress_EA":
                    return new Adapter_15tonCrankPinPress_EA(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "WeldIntegrity":
                    return new WeldIntegrity(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "WeldingStation":
                    return new WeldingStation(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_DrumDetails":
                    return new Z31_DrumDetails(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_FoamCalibration":
                    return new Z31_FoamCalibration(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_FoamDispensing":
                    return new Z31_FoamDispensing(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_InseratDispensing":
                    return new Z31_InseratDispensing(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_InserationCalibration":
                    return new Z31_InserationCalibration(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_PalletIDReport":
                    return new Z31_PalletIDReport(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_ProductionData":
                    return new Z31_ProductionData(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_RealTimeData":
                    return new Z31_RealTimeData(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_ThermalCalibration":
                    return new Z31_ThermalCalibration(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_ThermalDispensing":
                    return new Z31_ThermalDispensing(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z31_BMSActivation":
                    return new Z31_BMSActivation(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "LeakTesting":
                    return new LeakTesting(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "ZFixation":
                    return new ZFixation(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "FoamStation":
                    return new FoamStation(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "ThermalStation":
                    return new ThermalStation(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "InserationStation":
                    return new InserationStation(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_ProductionData":
                    return new Z32_ProductionData(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_RealTimeData":
                    return new Z32_RealTimeData(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_DrumDetails":
                    return new Z32_DrumDetails(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_FoamCalibration":
                    return new Z32_FoamCalibration(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_FoamDispensing":
                    return new Z32_FoamDispensing(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_InseratDispensing":
                    return new Z32_InseratDispensing(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_InserationCalibration":
                    return new Z32_InserationCalibration(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_PalletIDReport":
                    return new Z32_PalletIDReport(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_ThermalCalibration":
                    return new Z32_ThermalCalibration(pLCLogicalStation, adapterPort, queryIntervalinMS);
                case "Z32_ThermalDispensing":
                    return new Z32_ThermalDispensing(pLCLogicalStation, adapterPort, queryIntervalinMS);


                default:
                    {
                        Console.WriteLine("================ ERRORR !!! Creating MachineType :: {0}================", machineType);
                        return null;
                    }
            }

        }
    }
}