using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;


[SerializationConfig(MemberSerialization.OptIn)]
public class DisinfectionConduitDispenser : KMonoBehaviour, ISaveLoadable
{
    private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);
    private int utilityCell = -1;
    [SerializeField]
    private ConduitType conduitType;
    [SerializeField]
    private SimHashes[] elementFilter;
    [SerializeField]
    private bool invertElementFilter;
    [SerializeField]
    private bool alwaysDispense;
    [MyCmpReq]
    private Operational operational;
    [MyCmpReq]
    private Storage storage;
    [SerializeField]
    private double germsDensityThreshold = 10.0;
    private HandleVector<int>.Handle partitionerEntry;
    private int elementOutputOffset;

    public ConduitType TypeOfConduit
    {
        get
        {
            return this.conduitType;
        }
        set
        {
            this.conduitType = value;
        }
    }

    public SimHashes[] ElementFilter
    {
        get
        {
            return this.elementFilter;
        }
        set
        {
            this.elementFilter = value;
        }
    }

    public bool InvertElementFilter
    {
        get
        {
            return this.invertElementFilter;
        }
        set
        {
            this.invertElementFilter = value;
        }
    }

    public ConduitFlow.ConduitContents ConduitContents
    {
        get
        {
            return this.GetConduitManager().GetContents(this.utilityCell);
        }
    }

    public double GermsDensityThreshold
    {
        get
        {
            return germsDensityThreshold;
        }
        set
        {
            germsDensityThreshold = value;
        }
    }


    public void SetConduitData(ConduitType type)
    {
        this.conduitType = type;
    }

    public ConduitFlow GetConduitManager()
    {
        switch (this.conduitType)
        {
            case ConduitType.Gas:
                return Game.Instance.gasConduitFlow;
            case ConduitType.Liquid:
                return Game.Instance.liquidConduitFlow;
            default:
                return (ConduitFlow)null;
        }
    }

    private void OnConduitConnectionChanged(object data)
    {
        this.Trigger(-2094018600, (object)this.IsConnected);
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        this.utilityCell = this.GetComponent<Building>().GetUtilityOutputCell();
        this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", (object)this.gameObject, this.utilityCell, GameScenePartitioner.Instance.objectLayers[this.conduitType != ConduitType.Gas ? 16 : 12], new Action<object>(this.OnConduitConnectionChanged));
        this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Last);
        this.OnConduitConnectionChanged((object)null);
    }

    protected override void OnCleanUp()
    {
        this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
        GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
        base.OnCleanUp();
    }

    private void ConduitUpdate(float dt)
    {
        this.operational.SetFlag(DisinfectionConduitDispenser.outputConduitFlag, this.IsConnected);
        if (!this.operational.IsOperational && !this.alwaysDispense)
        {
            return;
        }
        PrimaryElement suitableElement = this.FindSuitableElement();
        if ((UnityEngine.Object)suitableElement == (UnityEngine.Object)null)
        {
            return;
        }
        suitableElement.KeepZeroMassObject = true;
        float num1 = this.GetConduitManager().AddElement(this.utilityCell, suitableElement.ElementID, suitableElement.Mass, suitableElement.Temperature, byte.MaxValue, 0);
        if ((double)num1 <= 0.0)
        {
            return;
        }
        int num2 = (int)((double)(num1 / suitableElement.Mass) * (double)suitableElement.DiseaseCount);
        suitableElement.Mass -= num1;
        this.Trigger(-1697596308, (object)suitableElement.gameObject);
    }

    private PrimaryElement FindSuitableElement()
    {
        List<GameObject> items = this.storage.items;
        int count = items.Count;
        for (int index1 = 0; index1 < count; ++index1)
        {
            int index2 = (index1 + this.elementOutputOffset) % count;
            PrimaryElement component = items[index2].GetComponent<PrimaryElement>();
            if ((UnityEngine.Object)component != (UnityEngine.Object)null && 
                (double)component.Mass > 0.0 &&
                (double)component.DiseaseCount / component.Mass < germsDensityThreshold  &&
                ((this.conduitType != ConduitType.Liquid ? (component.Element.IsGas ? 1 : 0) : (component.Element.IsLiquid ? 1 : 0)) != 0 && 
                 (this.elementFilter == null || this.elementFilter.Length == 0 || !this.invertElementFilter && this.IsFilteredElement(component.ElementID) || this.invertElementFilter && !this.IsFilteredElement(component.ElementID))))
            {
                this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
                return component;
            }
        }
        return (PrimaryElement)null;
    }

    private bool IsFilteredElement(SimHashes element)
    {
        for (int index = 0; index != this.elementFilter.Length; ++index)
        {
            if (this.elementFilter[index] == element)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsConnected
    {
        get
        {
            GameObject gameObject = Grid.Objects[this.utilityCell, this.conduitType != ConduitType.Gas ? 16 : 12];
            if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
            {
                return (UnityEngine.Object)gameObject.GetComponent<BuildingComplete>() != (UnityEngine.Object)null;
            }
            return false;
        }
    }
}
