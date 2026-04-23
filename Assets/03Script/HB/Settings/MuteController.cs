using UnityEngine;
using UnityEngine.Rendering;

public class MuteController : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

    // 환경설정 Toggle의 OnValueChanged(bool)에 이 함수를 연결
    public void SetMuteFromToggle(bool isMuted)
    {
        if (audioManager == null) return;

        // 체크됨(Mute) -> 볼륨 0
        // 체크해제(Sound On) -> 볼륨 100
        //float targetVolume = isMuted ? 0f : 100;

        audioManager.SetMute(isMuted);

        //audioManager.SetMainVolume(targetVolume);
    }
}
