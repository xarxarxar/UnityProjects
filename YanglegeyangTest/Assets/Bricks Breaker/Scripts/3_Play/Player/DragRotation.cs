using UnityEngine;
using UnityEngine.EventSystems;

public class DragRotation : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    bool isInteractable = false;
    public Transform playerCenter;

    #region - DragPanel
    public void OnPointerDown (PointerEventData data) {
        if (playerCenter == null) return;
        if(CtrGame.instance.IsLock) return;

        //if (CtrGame.instance.isGameOver || !CtrGame.instance.isGameStart) return;
        //if (!CtrGame.instance.PlayerReady) return;
        isInteractable = true;


        Vector3 diff = Camera.main.ScreenToWorldPoint(data.position) - playerCenter.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        playerCenter.rotation = Quaternion.Euler(0f, 0f, Mathf.Clamp((rot_z - 90), -83, 83));
    }

    //Vector2 dragging;
    public void OnDrag (PointerEventData data) {
        if(CtrGame.instance.IsLock) return;
        if (playerCenter == null) return;
        //if (CtrGame.instance.isGameOver || !CtrGame.instance.isGameStart) return;
        if (!isInteractable) return;
        Vector3 diff = Camera.main.ScreenToWorldPoint(data.position) - playerCenter.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        playerCenter.rotation = Quaternion.Euler(0f, 0f, Mathf.Clamp((rot_z - 90), -83, 83));
    }

    public void OnPointerUp (PointerEventData data) {
        if(CtrGame.instance.IsLock) return;
        if (playerCenter == null) return;
        //if (CtrGame.instance.isGameOver || !CtrGame.instance.isGameStart) return;
        if (!isInteractable) return;
        isInteractable = false;

        Player.instance.ShotBall();
    }
    #endregion

}





