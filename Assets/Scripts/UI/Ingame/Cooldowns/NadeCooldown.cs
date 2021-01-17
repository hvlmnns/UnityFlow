using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NadeCooldown : MonoBehaviour
{

  [Header("Settings")]
  public float lerpSpeed = 10f;
  public Vector3 activeOffset = new Vector3(0, -40f, 0);
  public GameObject overlayGO;
  public GameObject cooldownTextGO;
  public GameObject keybindTextGO;
  public GameObject positionGO;
  public GameObject iconGO;


  [Header("Dynamic settings")]
  public float maxValue = 1f;
  public float currentValue = 0f;
  public float maxRechargeTime = 0f;
  public float rechargeTime = 0f;
  public string key = "None";
  public float slot;
  public bool isCasting;
  public SO_NadeDefinition nadeDefinition;

  private RectTransform rect;
  private CanvasGroup transparencyGroup;
  private RectTransform overlay;
  private Text cooldownText;
  private Text keybindText;
  private Image icon;
  private Vector3 cooldownPosition;
  private Vector3 readyPosition;


  void Start()
  {
    rect = positionGO.GetComponent<RectTransform>() as RectTransform;
    cooldownText = cooldownTextGO.GetComponent<Text>() as Text;
    keybindText = keybindTextGO.GetComponent<Text>() as Text;
    overlay = overlayGO.GetComponent<RectTransform>() as RectTransform;
    icon = iconGO.GetComponent<Image>() as Image;
    transparencyGroup = overlayGO.GetComponent<CanvasGroup>() as CanvasGroup;

    readyPosition = rect.transform.localPosition;
    cooldownPosition = readyPosition + activeOffset;

    icon.sprite = nadeDefinition.icon;
  }

  void Update()
  {
    float percent = currentValue / maxValue;
    float value = maxValue * percent;
    bool isCoolingDown = percent > 0;

    keybindText.text = key;

    if (isCoolingDown)
    {
      cooldownText.enabled = true;
      rect.transform.localPosition = Vector3.Lerp(rect.transform.localPosition, cooldownPosition, lerpSpeed * Time.deltaTime);
      cooldownText.text = "" + (Mathf.Round(value * 10) / 10);
      transparencyGroup.alpha = Mathf.Lerp(transparencyGroup.alpha, 1, lerpSpeed * Time.deltaTime);
    }
    else
    {
      cooldownText.enabled = false;
      rect.transform.localPosition = Vector3.Lerp(rect.transform.localPosition, readyPosition, lerpSpeed * Time.deltaTime);
      cooldownText.text = "";
      transparencyGroup.alpha = Mathf.Lerp(transparencyGroup.alpha, 0, lerpSpeed * Time.deltaTime);
    }
  }
}
