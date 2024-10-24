using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;

    // 更新根节点的状态
    public Node.State Update()
    {
        if(rootNode.state == Node.State.Running)
        {
            treeState = rootNode.Update();
        }
        return treeState;
    }
}
