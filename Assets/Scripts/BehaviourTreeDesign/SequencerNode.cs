using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    // ����SequencerNode���һ���ӽڵ㷵��Failure
    // ������Ϊ���ڵ�Ҳ��һ������Failure
    // ��̳ж���CompositeNode��children List

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
                // �����ǰ�ӽڵ�ִ����ϣ�ִ����һ���ӽڵ�
                curChild++;
                break;
        }
        return curChild == children.Count ? 
            State.Success : State.Running;   
    }
}
