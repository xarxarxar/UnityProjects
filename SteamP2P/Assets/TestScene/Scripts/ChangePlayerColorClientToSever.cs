using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// ������߼����ǣ�
/// ������ͨ���ͻ���ͬ����������
/// </summary>
public class ChangePlayerColorClientToSever : NetworkBehaviour //������Ҫ�̳�NetworkBehaviour
{
    [SyncVar(hook = nameof(ColorChanged))]
    Color PlayerColor = Color.white;

    [SyncVar(hook = nameof(OnPositionChanged))]
    private Vector3 syncedPosition;

    public float moveSpeed = 5f;

    Rigidbody rb;

    //[Command(requiresAuthority = false)],���ʹ��������������ʾ�����ǲ��������Լ��Ľ�ɫ��ֻҪִ���˶���һ��ִ��
    [Command]
    void CmdChangeColor(Color color)
    {
        PlayerColor = color;
    }

    [Command]
    void CmdMove(float moveHorizontal, float moveVertical)
    {
        // Calculate movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * moveSpeed * Time.deltaTime;

        // Apply movement
        rb.MovePosition(rb.position + movement);
    }

    void OnPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        rb.MovePosition(newPosition);
    }

    MaterialPropertyBlock prop;
    CinemachineVirtualCamera cv;

    void ColorChanged(Color oldColor, Color newColor)
    {
        Debug.Log("Color Changed");
        prop.SetColor("_Color", newColor);
        GetComponent<Renderer>().SetPropertyBlock(prop);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        prop = new MaterialPropertyBlock();
        GetComponent<Renderer>().GetPropertyBlock(prop);
        if (isLocalPlayer)
        {
            cv = GameObject.FindGameObjectWithTag("VCAM").GetComponent<CinemachineVirtualCamera>();
            cv.Follow = this.transform;
            cv.LookAt = this.transform;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdChangeColor(PlayerColor == Color.white ? Color.black : Color.white);
        }

        if (isLocalPlayer)
        {
            // Capture player input
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            // Call CmdMove to send input to the server
            CmdMove(moveHorizontal, moveVertical);
        }
        else
        {
            // Update position for non-local players
            rb.MovePosition(syncedPosition);
        }
    }

}
