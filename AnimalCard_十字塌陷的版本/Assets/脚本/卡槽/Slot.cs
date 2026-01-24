using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private bool _isOccupied;
    public Collider2D Collider;

    public bool IsOccupied
    {
        get => _isOccupied;
    }
    /// <summary>
    /// ––
    /// </summary>
    public int Row;
    /// <summary>
    /// ¡–
    /// </summary>
    public int Col;

    private void Awake()
    {
        if(Collider == null)
        {
            Collider=GetComponent<Collider2D>();
        }
    }

    public void Init(int row, int col)
    {
        Row = row;
        Col = col;
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
