using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MenuVibrationbsLogic : MonoBehaviour
{
    public TextMeshProUGUI Display;
    public Toggle Ref;
    [Space]
    public string on, off;
    private void OnEnable() => Change();

    public void Change() => Display.text = Ref.isOn ? on : off;
}