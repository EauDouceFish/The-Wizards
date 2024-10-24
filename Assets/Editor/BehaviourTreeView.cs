using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

public class BehaviourTreeView : GraphView
{
    private const string STYLESHEET_PATH = "Assets/Editor/BehaviourTreeEditor.uss";
    // UxmlFactory提供UXML的UI布局支持，让该节点可以在UXML中实例化
    // UxmlTraits作用是在使用 UXML时，管理和应用从中读取的属性值到自定义控件。
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
    
    // 构造函数
    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());
        var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(STYLESHEET_PATH);
        styleSheets.Add(uss);
    }
}
