using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillText : MonoBehaviour
{
    public Image skillImg;
    public TMP_Text skillName;
    public List<TMP_Text> skillType;
    public Text skillText;
    public Text TextMP;
    public Text TextTired;
    public Text TextFail;

    public IEnumerator Reset()
    {
        skillName.text = "";
        foreach (var t in skillType)
            t.text = "";
        skillText.text = "";
        yield return null;
    }

}
