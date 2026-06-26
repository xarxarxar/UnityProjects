namespace EKStudio
{
    using DG.Tweening;
    using UnityEngine;
    
    public enum RotationDirection { X, Y, Z }
    
    public class RotationAnimation : MonoBehaviour
    {
    	public RotationDirection RotationDirection = RotationDirection.Y;
    	public RotateMode RotateMode = RotateMode.FastBeyond360;
    	public Ease Ease = Ease.Linear;
    	public LoopType LoopType = LoopType.Restart;
    
    	public float Distance = 360f;
    	public float Duration = 1f;
    	public float Delay = 0f;
    	void Start()
    	{
    		switch (RotationDirection)
    		{
    			case RotationDirection.X:
    				Rotate(new Vector3(transform.localEulerAngles.x + Distance, transform.localEulerAngles.y, transform.localEulerAngles.z));
    				break;
    			case RotationDirection.Y:
    				Rotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + Distance, transform.localEulerAngles.z));
    				break;
    			case RotationDirection.Z:
    				Rotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + Distance));
    				break;
    			default:
    				break;
    		}
    	}
    	private void Rotate(Vector3 target)
    	{
    		transform.DOLocalRotate(target, Duration, RotateMode)
    			.SetDelay(Delay)
    			.SetEase(Ease)
    			.SetLoops(-1, LoopType);
    	}
    }
    
}
