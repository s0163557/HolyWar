using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UnitIconElement : VisualElement
{
    private Image unitIconSprite => this.Q("unit-icon-sprite") as Image;
    //private Label unitIconName => this.Q<Label>("unit-icon-name");
    private Label unitIconCost => this.Q<Label>("unit-icon-cost");

    public void Init(Sprite icon, string name, int cost)
    {
        unitIconSprite.sprite = icon;
        //unitIconName.text = name;
        unitIconCost.text = cost.ToString();
    }

    public UnitIconElement() { }
}
