using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropertyVisualRule", menuName = "Scriptable Objects/PropertyVisualRule")]
public class PropertyVisualRules : ScriptableObject
{
    public List<PropertyVisualRule> PropertyVisualRulesList = new();
}

[Serializable]
public class PropertyVisualRule
{
    public string Keyword;
    public bool IsColorOnly;
    public Sprite Sprite;
    public Color Tint = Color.white;
}