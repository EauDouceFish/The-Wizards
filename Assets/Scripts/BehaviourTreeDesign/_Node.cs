using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    // 行为树所有节点一共抽象为三种状态
    // 运行中，运行失败，运行成功
    public enum State
    {
        Running,
        Failure,
        Success
    }

    // 初始化则运行结点
    public State state = State.Running;
    public bool started = false;

    // 结点更新逻辑
    // 首次更新标记为开始
    // 之后更新结点状态
    // 如果结点运行结束，回到结束状态
    public State Update()
    {
        if (!started)
        {
            OnStart();
            started = true;
        }

        state = OnUpdate();
        if(state == State.Failure || state == State.Success)
        {
            OnStop();
            // Clean up
            started = false;
        }
        return state;
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();

}
