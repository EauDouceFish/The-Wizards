using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    // ��Ϊ�����нڵ�һ������Ϊ����״̬
    // �����У�����ʧ�ܣ����гɹ�
    public enum State
    {
        Running,
        Failure,
        Success
    }

    // ��ʼ�������н��
    public State state = State.Running;
    public bool started = false;

    // �������߼�
    // �״θ��±��Ϊ��ʼ
    // ֮����½��״̬
    // ���������н������ص�����״̬
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
