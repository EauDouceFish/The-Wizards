using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    // 对于SequencerNode如果一个子节点返回Failure
    // 则其作为父节点也会一并返回Failure
    // 其继承对象CompositeNode有children List

    int curChild;

    protected override void OnStart()
    {
        curChild = 0;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        Node child = children[curChild];
        switch (child.Update())
        {
            case State.Running: 
                return State.Running;
            case State.Failure:
                return State.Failure;   
            case State.Success:
                // 如果当前子节点执行完毕，执行下一个子节点
                curChild++;
                break;
        }
        return curChild == children.Count ? 
            State.Success : State.Running;   
    }
}
