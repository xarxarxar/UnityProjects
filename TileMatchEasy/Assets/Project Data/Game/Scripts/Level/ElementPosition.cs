using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public struct ElementPosition
    {
        public static readonly ElementPosition Empty = new ElementPosition(-1, -1, -1);

        private static readonly Dictionary<Direction, ElementPosition> DIRECTION_OFFSET = new Dictionary<Direction, ElementPosition>()
        {
            { Direction.Up, new ElementPosition(0, -1) },
            { Direction.Down, new ElementPosition(0, 1) },
            { Direction.Left, new ElementPosition(-1, 0) },
            { Direction.Right, new ElementPosition(1, 0) },
        };

        [SerializeField] int x;
        [SerializeField] int y;
        [SerializeField] int layerId;

        public int X => x;
        public int Y => y;
        public int LayerId => layerId;

        public ElementPosition(int x, int y, int layerId = 0)
        {
            this.x = x;
            this.y = y;
            this.layerId = layerId;
        }

        public ElementPosition(ElementPosition position)
        {
            x = position.x;
            y = position.y;
            layerId = position.layerId;
        }

        public ElementPosition(ElementPosition position, int layerId)
        {
            x = position.x;
            y = position.y;
            this.layerId = layerId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ElementPosition operator +(ElementPosition a, ElementPosition b) => new ElementPosition(a.X + b.X, a.Y + b.Y, a.LayerId);

        public bool Equals(ElementPosition other)
        {
            return (x == other.x && y == other.y);
        }

        public bool Equals(int x, int y)
        {
            return (this.x == x && this.y == y);
        }

        public override int GetHashCode()
        {
            return ((x + y) * (x + y + 1) / 2) + y;
        }

        public override string ToString()
        {
            return string.Format("(x:{0}, y:{1}, layerId:{2})", x, y, layerId);
        }

        public bool IsEmpty()
        {
            return x == -1 || y == -1 || layerId == -1;
        }

        public ElementPosition RightNeighbourPos => this + DIRECTION_OFFSET[Direction.Right];
        public ElementPosition LeftNeighbourPos => this + DIRECTION_OFFSET[Direction.Left];
        public ElementPosition UpNeighbourPos => this + DIRECTION_OFFSET[Direction.Up];
        public ElementPosition BottomNeighbourPos => this + DIRECTION_OFFSET[Direction.Down];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ElementPosition operator +(ElementPosition first, int2 second) => new ElementPosition { x = first.x + second.x, y = first.y + second.y };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ElementPosition operator -(ElementPosition first, int2 second) => new ElementPosition { x = first.x - second.x, y = first.y - second.y };
    }
}
