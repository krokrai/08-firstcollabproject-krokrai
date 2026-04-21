public struct UpgradeDatas
{
    public int MasterLevel;

    public int MaxCustomerLimitLevel;

    public int MaxMenuLimitLevel;

    public int MaxSpawnLimit01Level;

    public int MaxSpawnLimit02Level;

    public int WeightLevel;

    public int BonusTipsMultiLevel;

    public int BonusDishPrice01Level;

    public int BonusDishPrice02Level;

    public int BonusFood01Level;

    public int BonusFood02Level;

    public int UnlockCatObjectLevel;

    public int UnlockMenuLevel;

    public UpgradeDatas(int masterlvl, int maxCusLimitlvl, int mxMenuLimitlvl, int mxSpawnLimit01lvl, int mxSpawnLimit02lvl, int weightlvl, int bnsTipMulti, int bnsDishPrice01lvl, int bnsDishprice02lvl, int bnsFood01lvl, int bnsFood02lvl, int cat, int unMenulvl)
    {
        MasterLevel = masterlvl;
        MaxCustomerLimitLevel = maxCusLimitlvl;
        MaxMenuLimitLevel = mxMenuLimitlvl;
        MaxSpawnLimit01Level = mxSpawnLimit01lvl;
        MaxSpawnLimit02Level = mxSpawnLimit02lvl;
        WeightLevel = weightlvl;
        BonusTipsMultiLevel = bnsTipMulti;
        BonusDishPrice01Level = bnsDishPrice01lvl;
        BonusDishPrice02Level = bnsDishprice02lvl;
        BonusFood01Level = bnsFood01lvl;
        BonusFood02Level = bnsFood02lvl;
        UnlockCatObjectLevel = cat;
        UnlockMenuLevel = unMenulvl;
    }
}