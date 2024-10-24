using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    BehaviourTree tree;

    private void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviourTree>();
        DebugLogNode log = ScriptableObject.CreateInstance<DebugLogNode>();
        log.message = "Debug Log Message";

        //RepeatNode loop = ScriptableObject.CreateInstance<RepeatNode>();    
        //loop.child = log;
        tree.rootNode = log;
    }

    private void Update()
    {
        tree.Update();
    }

}
