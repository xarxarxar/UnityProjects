using UnityEngine;
using DG.Tweening;


public class SetTriggerPos : MonoBehaviour
{
    public enum TriggerType {
        Left,
        Right,
        Top,
        Bot
    }

    public TriggerType triggerType;

    private void Start () {
        float distance = Player.instance.balls[0].GetComponent<CircleCollider2D>().radius;

        switch (triggerType) {
            case TriggerType.Left:
                transform.DOMoveX(distance, 0f).SetRelative(true);
                break;

            case TriggerType.Right:
                transform.DOMoveX(-distance, 0f).SetRelative(true);
                break;

            case TriggerType.Top:
                transform.DOMoveY(-distance, 0f).SetRelative(true);
                break;

            case TriggerType.Bot:
                transform.DOMoveY(distance, 0f).SetRelative(true);
                break;
        }

    }

}
