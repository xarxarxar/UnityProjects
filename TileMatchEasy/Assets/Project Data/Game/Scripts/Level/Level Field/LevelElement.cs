namespace Watermelon
{
    public struct LevelElement
    {
        private TileBehavior tile;
        public TileBehavior Tile => tile;
        public bool State => tile != null;

        public static implicit operator LevelElement(TileBehavior tile) => new LevelElement { tile = tile };
        public static implicit operator TileBehavior(LevelElement element) => element.tile;

        public void LinkTile(TileBehavior tile)
        {
            this.tile = tile;
        }
    }
}
