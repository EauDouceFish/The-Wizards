using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatNode : DecorateNode
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {

    }

    // RepeatNode����ֻ����Running״̬���Դ��ظ�
    protected override State OnUpdate()
    {
        child.Update();
        return State.Running;
    }
}
