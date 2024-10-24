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

    // RepeatNode运行只返回Running状态，以此重复
    protected override State OnUpdate()
    {
        child.Update();
        return State.Running;
    }
}
