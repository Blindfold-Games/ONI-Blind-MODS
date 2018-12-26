﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Harmony;
using STRINGS;

namespace FluidPressureSensor
{
    public static class Logger {

        public static void Log(string message)
        {
            // Logs messages only in debug builds
#if DEBUG
            Debug.Log(message);
#endif
        }

        public static void LogFormat(string template, params object[] args)
        {
            // Logs messages only in debug builds
#if DEBUG
            Debug.LogFormat(template, args);
#endif
        }

    }

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal static class AdvFluidDistrMod_GeneratedBuildings_LoadGeneratedBuildings
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            Strings.Add("STRINGS.BUILDINGS.CONDUITPRESSURESENSOR.MASS", "Mass");
            Strings.Add("STRINGS.BUILDINGS.MASS_THRESHOLD", "Mass Threshold");
            Strings.Add("STRINGS.BUILDINGS.GRAMMS", "g.");

            Logger.Log("Entry AdvFluidDistrMod_GeneratedBuildings_LoadGeneratedBuildings.Prefix ");
            Strings.Add("STRINGS.BUILDINGS.PREFABS." + GasConduitPressureConfig.ID.ToUpper() + ".NAME", "Gas Pipe Pressure Sensor");
            Strings.Add("STRINGS.BUILDINGS.PREFABS." + GasConduitPressureConfig.ID.ToUpper() + ".DESC", "Gas Pipe Pressure Sensor can disable buildings when contents reach a certain mass.");
            Strings.Add("STRINGS.BUILDINGS.PREFABS." + GasConduitPressureConfig.ID.ToUpper() + ".EFFECT", string.Concat(new string[] { "Becomes ", UI.FormatAsLink("Active", "LOGIC"), " or on ", UI.FormatAsLink("Standby", "LOGIC"), " when the pipe contents mass enters the chosen range." }));
            ModUtil.AddBuildingToPlanScreen(new HashedString("HVAC"), GasConduitPressureConfig.ID);

            Strings.Add("STRINGS.BUILDINGS.PREFABS." + LiquidConduitPressureConfig.ID.ToUpper() + ".NAME", "Liquid Pipe Pressure Sensor");
            Strings.Add("STRINGS.BUILDINGS.PREFABS." + LiquidConduitPressureConfig.ID.ToUpper() + ".DESC", "Liquid Pipe Pressure Sensor can disable buildings when contents reach a certain mass.");
            Strings.Add("STRINGS.BUILDINGS.PREFABS." + LiquidConduitPressureConfig.ID.ToUpper() + ".EFFECT", string.Concat(new string[] { "Becomes ", UI.FormatAsLink("Active", "LOGIC"), " or on ", UI.FormatAsLink("Standby", "LOGIC"), " when the pipe contents mass enters the chosen range." }));
            ModUtil.AddBuildingToPlanScreen(new HashedString("Plumbing"), LiquidConduitPressureConfig.ID);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    internal static class AdvFluidDistrMod_Db_Initialize
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            Logger.Log("Entry AdvFluidDistrMod_Db_Initialize.Prefix");
            List<string> ls = new List<string>(Database.Techs.TECH_GROUPING["HVAC"]);
            ls.Add(GasConduitPressureConfig.ID);
            Database.Techs.TECH_GROUPING["HVAC"] = (string[])ls.ToArray();

            ls = new List<string>(Database.Techs.TECH_GROUPING["LiquidTemperature"]);
            ls.Add(LiquidConduitPressureConfig.ID);
            Database.Techs.TECH_GROUPING["LiquidTemperature"] = (string[])ls.ToArray();
            Logger.Log("Exit AdvFluidDistrMod_Db_Initialize.Prefix");
        }

    }

}
