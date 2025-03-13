using DG.Tweening;
using UnityEngine;

public class BackgroundChange : MonoBehaviour
{
    [SerializeField] private Transform cloudOne;
    [SerializeField] private Transform cloudTwo;
    [SerializeField] private Transform cloudThree;
    [SerializeField] private Transform cloudFour;
    // Start is called before the first frame update
    void Start()
    {
        MoveCloud(cloudOne,15,25);
        MoveCloud(cloudTwo, -15, 20);
        MoveCloud(cloudThree, 15, 15);
        MoveCloud(cloudFour, -15, 10);
    }

    /// <summary>
    /// 让云朵左右飘动，到头后瞬间回到起点
    /// </summary>
    /// <param name="cloud">要飘动的云朵对象</param>
    /// <param name="moveDistanceX">左右飘动距离</param>
    /// <param name="durationX">左右移动所需时间</param>
    public static void MoveCloud(Transform cloud, float moveDistanceX, float durationX)
    {
        if (cloud == null) return;

        Vector3 startPos = cloud.position; // 记录起始位置
        Vector3 targetPosX = startPos + new Vector3(moveDistanceX, 0, 0);

        void MoveHorizontally()
        {
            cloud.DOMoveX(targetPosX.x, durationX)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    cloud.position = startPos; // 直接回到起点
                    MoveHorizontally(); // 重新开始移动
                });
        }

        MoveHorizontally(); // 启动左右移动
    }
}
