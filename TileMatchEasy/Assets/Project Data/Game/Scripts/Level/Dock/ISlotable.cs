using UnityEngine;

namespace Watermelon
{
    public interface ISlotable
    {
        void Clear();
        void SubmitMove(Transform position, Vector3 scale, SimpleCallback onFinish);
        void SubmitMove(Vector3 position, Vector3 scale, SimpleCallback onFinish);
        void DockShuffleMove(Transform position, SimpleCallback onFinish);
        void SetSortingOrder(int order);
        bool IsSameType(ISlotable other);
        void MatchAnimation(float delay);

        int UniqueElementID { get; }
        Transform Transform { get; }
        Transform VisualsTransform { get; }
    }
}