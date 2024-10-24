using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTree = default;

    [MenuItem("Window/BehaviourTreeEditor/Editor")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // 如果没有手动分配visualTreeAsset就用面板内的
        if(visualTree == null || visualTree == default)
        {
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTreeEditor.uxml");
        }
        visualTree.CloneTree(root);
        

        var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(uss);
    }
}
