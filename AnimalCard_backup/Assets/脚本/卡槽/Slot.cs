using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private int _index;
    [SerializeField] private bool _isOccupied;

    public int Index => _index;

    public bool IsOccupied
    {
        get => _isOccupied;
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
