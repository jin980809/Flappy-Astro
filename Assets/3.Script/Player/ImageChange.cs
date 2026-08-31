using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ImageChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    private Image buttonImage;

    private void Awake()
    {
        TryGetComponent(out buttonImage);
    }
    private void Start()
    {
        buttonImage.sprite = normalSprite;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.sprite = normalSprite;
    }
}
