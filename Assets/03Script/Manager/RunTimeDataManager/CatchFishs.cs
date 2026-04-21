using System;

[Serializable]
public struct CatchFishs
{
    /// <summary>
    /// 현재까지 잡은 총 물고기 수
    /// </summary>
    public ulong catchFishs;

    /// <summary>
    /// 희귀도가 쓰레기인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishTrash;

    /// <summary>
    /// 희귀도가 일반인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishNormal;

    /// <summary>
    /// 희귀도가 우수인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishFine;

    /// <summary>
    /// 희귀도가 고급인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishSuperior;

    /// <summary>
    /// 희귀도가 희귀인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishRare;

    /// <summary>
    /// 희귀도가 명품인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishElite;

    /// <summary>
    /// 희귀도가 환상인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishFantastic;

    /// <summary>
    /// 희귀도가 전설인 물고기를 잡은 횟수
    /// </summary>
    public ulong catchFishLegendary;

    public CatchFishs(ulong catchFishs, ulong catchFishTrash, ulong catchFishNormal, ulong catchFishFine, ulong catchFishSuperior, ulong catchFishRare, ulong catchFishElite, ulong catchFishFantastic, ulong catchFishLegendary)
    {
        this.catchFishs = catchFishs;
        this.catchFishTrash = catchFishTrash;
        this.catchFishNormal = catchFishNormal;
        this.catchFishFine = catchFishFine;
        this.catchFishSuperior = catchFishSuperior;
        this.catchFishRare = catchFishRare;
        this.catchFishElite = catchFishElite;
        this.catchFishFantastic = catchFishFantastic;
        this.catchFishLegendary = catchFishLegendary;
    }
}
