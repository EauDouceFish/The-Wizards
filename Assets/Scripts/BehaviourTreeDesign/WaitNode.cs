using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class WaitNode : ActionNode
{
    public float duration = 1.0f;
    float startTime;

    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        // 倒计时结束就返回waitNode运行成功
        if(Time.time - startTime > duration)
        {
            return State.Success;
        }
        return State.Running;
    }
}
