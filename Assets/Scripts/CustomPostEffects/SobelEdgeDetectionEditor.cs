/*using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.InputSystem.Controls.AxisControl;

namespace UnityEditor.Rendering.Universal
{
    [CustomEditor(typeof(SobelEdgeDetection))]
    sealed class SobelEdgeDetectionEditor : VolumeComponentEditor
    {

        SerializedDataParameter m_EdgesOnly;
        SerializedDataParameter m_EdgeColor;
        SerializedDataParameter m_BackgroundColor;
        public override void OnEnable()
        {
            var o = new PropertyFetcher<SobelEdgeDetection>(serializedObject);
            m_EdgesOnly = Unpack(o.Find(x => x.edgesOnly));
            m_EdgeColor = Unpack(o.Find(x => x.edgeColor));
            m_BackgroundColor = Unpack(o.Find(x => x.backgroundColor));
        }

        public override void OnInspectorGUI()
        {
            PropertyField(m_EdgesOnly);
            PropertyField(m_EdgeColor);
            PropertyField(m_BackgroundColor);

*//*            if (GUILayout.Button("Reset Edge Color"))
            {
                m_EdgeColor.value.colorValue = Color.black;
            }*//*
        }

    }
}*/
