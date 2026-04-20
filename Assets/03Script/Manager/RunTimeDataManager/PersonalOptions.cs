public struct PersonalOptions
{
    /// <summary>
    /// 마스터 볼륨 조절용
    /// </summary>
    public float masterVolume; // 실제 표기는 0~100 정수 값. 실제 slider에 들어가는 값은 0~1의 실수 값.

    /// <summary>
    /// BGM 볼륨
    /// </summary>
    public float BGMVolume;

    /// <summary>
    /// SFX 볼륨
    /// </summary>
    public float SFXVolume;

    /// <summary>
    /// 투명도 조절 변수 0~100 값만 사용 예정
    /// 투명도가 높을 수록 게임이 투명해지며, 최대치 일때 게임은 알파 값 30 유지.
    /// </summary>
    public byte transparentLevel;

    public Language languageSetting { get; private set; } /// <summary> 현재 설정된 언어, 기본 값 : 영어 </summary>

    public PersonalOptions(float masterVolume, float BGMVolume, float SFXVolume, byte transparentLevel, Language lan)
    {
        this.masterVolume = masterVolume;
        this.BGMVolume = BGMVolume;
        this.SFXVolume = SFXVolume;
        this.transparentLevel = transparentLevel;
        languageSetting = lan;
    }
}
