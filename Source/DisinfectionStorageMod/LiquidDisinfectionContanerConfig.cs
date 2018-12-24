using TUNING;
using UnityEngine;

namespace DisinfectionStorageMod
{
    class LiquidDisinfectionStorageConfig : IBuildingConfig
    {
        public const string ID = "LiquidDisinfectionReservoir";
        private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;
        private const int WIDTH = 2;
        private const int HEIGHT = 3;

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "liquidreservoir_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.UtilityInputOffset = new CellOffset(1, 2);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<Reservoir>();
            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go, false);
            defaultStorage.showDescriptor = true;
            defaultStorage.allowItemRemoval = false;
            defaultStorage.storageFilters = STORAGEFILTERS.LIQUIDS;
            defaultStorage.capacityKg = 5000f;
            defaultStorage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = defaultStorage.capacityKg;
            DisinfectionConduitDispenser conduitDispenser = go.AddOrGet<DisinfectionConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.elementFilter = (SimHashes[])null;
            conduitDispenser.germsDensityThreshold = 100.0d; // germs per kg of liquid
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}
