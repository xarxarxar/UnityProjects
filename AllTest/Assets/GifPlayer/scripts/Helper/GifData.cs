namespace GifPlayer
{
    public class GifData
    {
        public byte[] Bytes;
        public SequenceFrame[] Frames;

        public GifData(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}
