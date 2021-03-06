﻿using System;
using System.Collections.Generic;
using Harmony;
using STRINGS;

namespace DisinfectionStorageMod
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal static class DisinfectionStorageMod_GeneratedBuildings_LoadGeneratedBuildings
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            Debug.Log("Entry DisinfectionStorageMod_GeneratedBuildings_LoadGeneratedBuildings.Prefix ");

            Strings.Add("STRINGS.BUILDINGS.PREFABS." + GasDisinfectionStorageConfig.ID.ToUpper() + ".NAME", "Gas Disinfection Reservoir");
            Strings.Add("STRINGS.BUILDINGS.PREFABS." + GasDisinfectionStorageConfig.ID.ToUpper() + ".DESC", "Reservoirs cannot receive manually delivered resources.");
            Strings.Add("STRINGS.BUILDINGS.PREFABS." + GasDisinfectionStorageConfig.ID.ToUpper() + ".EFFECT", " Stores any Gas resources piped into it. Output only gas free from gems. Intended to be used inside a room filled with " + (string)ELEMENTS.CHLORINEGAS.NAME + ".");
            ModUtil.AddBuildingToPlanScreen(new HashedString("Medical"), GasDisinfectionStorageConfig.ID);

            Strings.Add("STRINGS.BUILDINGS.PREFABS." + LiquidDisinfectionStorageConfig.ID.ToUpper() + ".NAME", "Liquid Disinfection Reservoir");
            Strings.Add("STRINGS.BUILDINGS.PREFABS." + LiquidDisinfectionStorageConfig.ID.ToUpper() + ".DESC", "Reservoirs cannot receive manually delivered resources.");
            Strings.Add("STRINGS.BUILDINGS.PREFABS." + LiquidDisinfectionStorageConfig.ID.ToUpper() + ".EFFECT", " Stores any Liquid resources piped into it. Output only liquid free from gems. Intended to be used inside a room filled with " + (string)ELEMENTS.CHLORINEGAS.NAME + ".");
            ModUtil.AddBuildingToPlanScreen(new HashedString("Medical"), LiquidDisinfectionStorageConfig.ID);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    internal static class DisinfectionStorageMod_Db_Initialize
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            Debug.Log("Entry DisinfectionStorageMod_Db_Initialize.Prefix");
            List<string> ls = new List<string>(Database.Techs.TECH_GROUPING["MedicalResearch"]);
            ls.Add(GasDisinfectionStorageConfig.ID);
            ls.Add(LiquidDisinfectionStorageConfig.ID);
            Database.Techs.TECH_GROUPING["MedicalResearch"] = (string[])ls.ToArray();
            Debug.Log("Exit DisinfectionStorageMod_Db_Initialize.Prefix");
        }

    }
}
