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
            // 获取屏幕高度的一半
            float screenHeight = Screen.height;
            float targetHeight = screenHeight / 2;

            // 计算图片的宽高比
            float aspectRatio = rectTransform.rect.width / rectTransform.rect.height;

            // 设置图片的高度为屏幕高度的一半，并按比例缩放宽度
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetHeight * aspectRatio);
        }
        else
        {
            Debug.LogError("Image 或 RectTransform 组件缺失。");
        }
    }
}
