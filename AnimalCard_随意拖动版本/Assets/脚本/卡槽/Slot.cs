using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private bool _isOccupied;
    public Collider2D Collider;

    public bool IsOccupied
    {
        get => _isOccupied;
    }

    private void Awake()
    {
        if(Collider == null)
        {
            Collider=GetComponent<Collider2D>();
        }
    }

    /// <summary>
    /// ÷ÿ÷√ø®≤€
    /// </summary>
    public void ResetSlot()
    {
        _isOccupied=false;
    }

    public void SetOccupied()
    {
        _isOccupied= true;
    }

    public void SetUnOccupied()
    {
        _isOccupied=false;
    }

}
