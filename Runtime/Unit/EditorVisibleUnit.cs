using System;
using System.Collections.Generic;
using Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class EditorVisibleUnit : MonoBehaviour
{
#if UNITY_EDITOR
    [ShowInInspector]
    public Unit unit { get; private set; }

    [ShowInInspector]
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, IsReadOnly = true)]
    private Dictionary<Type, Entity> components => unit?.Components;

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }
#endif
}