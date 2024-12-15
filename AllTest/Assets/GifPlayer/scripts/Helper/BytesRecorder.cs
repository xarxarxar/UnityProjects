/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

using System.IO;

namespace GifPlayer.GifProtocol
{
    public class BytesRecorder
    {
        public byte[] RefBytes { get; private set; }

        public int StartIndex { get; private set; }

        public int EndIndexPlus { get; private set; }

        public int EndIndex { get; private set; }

        public int Length { get; private set; }

        public BytesRecorder(byte[] bytes, int startIndex)
        {
            RefBytes = bytes;
            StartIndex = startIndex;
        }

        public void SetEndIndexPlus(int endIndexPlus)
        {
            EndIndexPlus = endIndexPlus;
            EndIndex = endIndexPlus - 1;
            Length = endIndexPlus - StartIndex;
        }

        public MemoryStream GetStream()
        {
            var stream = new MemoryStream();
            stream.Write(RefBytes, StartIndex, Length);
            stream.Position = 0;
            return stream;
        }
    }
}
