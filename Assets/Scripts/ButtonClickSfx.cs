using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClickSfx : MonoBehaviour, IPointerDownHandler, ISubmitHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        GameSfx.PlayButtonClick();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        GameSfx.PlayButtonClick();
    }
}
