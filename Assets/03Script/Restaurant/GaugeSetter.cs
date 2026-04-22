using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GaugeSetter : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] Image _fillImage;
    [SerializeField] TextMeshProUGUI _tmp;

    Color _gaugeBlueColor = new Color(153 / 255f, 172 / 255f, 1);
    Color _gaugeRedColor = new Color(1, 97 / 255f, 80 / 255f, 1);

    Color _normalCustomerTextColor = new Color(0.004f, 0.004f, 0.004f, 1f);
    Color _specialCustomerTextColor = new Color(1, 189 / 255f, 89 / 255f, 1);
    Color _vipCustomerTextColor = new Color(31 / 255f, 1, 147 / 255f, 1);

    /// <summary>
    /// 손님의 등급에 따라 표시되는 글자의 색을 바꿔줌.
    /// </summary>
    /// <param name="grade"></param>
    public void SetCustomerGrade(CustomerGrade grade)
    {
        // 99ACFF 게이지 파랑 : 153 172 255
        // FF6150 게이지 빨강 : 255 97 80
        switch (grade)
        {
            case CustomerGrade.NORMAL:
                _tmp.text = "Normal";
                _tmp.color = _normalCustomerTextColor;
                break;
            case CustomerGrade.SPECIAL:
                _tmp.text = "Special";
                _tmp.color = _specialCustomerTextColor;
                break;
            case CustomerGrade.VIP:
                _tmp.text = "VIP";
                _tmp.color = _vipCustomerTextColor;
                break;
        }
        Debug.Log(_tmp.color);
    }

    /// <summary>
    /// 손님이 주문 상태 또는 식사 상태에 따라 fill의 색을 바꿔줌
    /// 식사 중 = true
    /// </summary>
    /// <param name="isEatDuration">식사 중이면 true</param>
    public void SetState(bool isEatDuration, bool isVisible)
    {
        Debug.Log($"게이지 시작 : {isVisible}");
        gameObject.SetActive(isVisible);
        _fillImage.color = isEatDuration ? _gaugeRedColor : _gaugeBlueColor;
    }

    /// <summary>
    /// 슬라이더 value 교체용.
    /// 대입 방식임.
    /// </summary>
    /// <param name="value"> 0~1 사이 값</param>
    public void SliderValueUpdate(in float value)
    {
        _slider.value = value;
    }

    public void GaugeOff()
    {
        gameObject.SetActive(false);
    }
}