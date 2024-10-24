using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flex : MonoBehaviour
{
    [SerializeField]private RectTransform rectTransform;
    private Image image;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        if (rectTransform != null && image != null)
        {
            // ��ȡ��Ļ�߶ȵ�һ��
            float screenHeight = Screen.height;
            float targetHeight = screenHeight / 2;

            // ����ͼƬ�Ŀ�߱�
            float aspectRatio = rectTransform.rect.width / rectTransform.rect.height;

            // ����ͼƬ�ĸ߶�Ϊ��Ļ�߶ȵ�һ�룬�����������ſ��
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetHeight * aspectRatio);
        }
        else
        {
            Debug.LogError("Image �� RectTransform ���ȱʧ��");
        }
    }
}
