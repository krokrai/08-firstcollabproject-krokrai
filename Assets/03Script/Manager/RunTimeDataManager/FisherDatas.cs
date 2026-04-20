using System;

[Serializable]
public struct FisherDatas
{
    /// <summary>
    /// 플레이어 등급 낚시 관련 레벨에 대해서 해당 부분에서 예외 처리가 없으므로, UI 작업자가 예외처리 열심히 해주셔야합니다.
    /// </summary>
    public int fishingGrade;
    /// <summary>
    /// 미끼 레벨
    /// </summary>
    public int baitLevel;
    /// <summary>
    /// 낚시대 레벨
    /// </summary>
    public int rodLevel;
    /// <summary>
    /// shipLevel;
    /// </summary>
    public int shipLevel;

    /// <summary>
    /// 최대로 저장할 수 있는 미끼 수
    /// </summary>
    public int fishingCount;

    /// <summary>
    /// 현재 갖고 있는 미끼 수.
    /// </summary>
    public int currentFishingCount;


    public FisherDatas(int fishingGrade, int baitLevel, int rodLevel, int shipLevel, int fishingCount, int currentFishingCount)
    {
        this.fishingGrade = fishingGrade;
        this.baitLevel = baitLevel;
        this.rodLevel = rodLevel;
        this.shipLevel = shipLevel;
        this.fishingCount = fishingCount;
        this.currentFishingCount = currentFishingCount;
    }
}