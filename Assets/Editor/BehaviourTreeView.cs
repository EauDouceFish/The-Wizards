using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

public class BehaviourTreeView : GraphView
{
    private const string STYLESHEET_PATH = "Assets/Editor/BehaviourTreeEditor.uss";
    // UxmlFactory�ṩUXML��UI����֧�֣��øýڵ������UXML��ʵ����
    // UxmlTraits��������ʹ�� UXMLʱ�������Ӧ�ô��ж�ȡ������ֵ���Զ���ؼ���
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
    
    // ���캯��
    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());
        var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(STYLESHEET_PATH);
        styleSheets.Add(uss);
    }
}
