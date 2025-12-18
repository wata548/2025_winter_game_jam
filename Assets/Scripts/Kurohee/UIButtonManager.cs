using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] float hoverScale = 1.2f;
    [SerializeField] float speed = 12f;
    
    Vector3 origin;
    Vector3 target;

    void Awake()
    {
        origin = transform.localScale;
        target = origin;
    }
            
    void Update()
    {
        transform.localScale =
            Vector3.Lerp(transform.localScale, target, Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        target = origin * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        target = origin;
    }
}
