/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

namespace GifPlayer.GifProtocol
{
    /// <summary>
    /// 文本扩展（解析文件时需处理,绘图时可忽略）
    /// </summary>
    public class ExtraText
    {
        /// <summary>
        /// 扩展引导标识 定值0x21
        /// </summary>
        public byte ExtraIntroducer;

        /// <summary>
        /// 文本扩展标识 0x01
        /// </summary>
        public byte TextFlag;

        // Block Size
        public byte BlockSize;

        public ExtraText(byte[] bytes, ref int offset)
        {
            // Extension Introducer(1 byte)
            // 0x21
            ExtraIntroducer = bytes[offset];
            offset++;

            // Plain Text Label(1 byte)
            // 0x01
            TextFlag = bytes[offset];
            offset++;

            // Block Size(1 byte)
            // 0x0c
            BlockSize = bytes[offset];
            offset++;

            // Text Grid Left Position(2 byte)
            // Not supported
            offset += 2;

            // Text Grid Top Position(2 byte)
            // Not supported
            offset += 2;

            // Text Grid Width(2 byte)
            // Not supported
            offset += 2;

            // Text Grid Height(2 byte)
            // Not supported
            offset += 2;

            // Character Cell Width(1 byte)
            // Not supported
            offset++;

            // Character Cell Height(1 byte)
            // Not supported
            offset++;

            // Text Foreground Color Index(1 byte)
            // Not supported
            offset++;

            // Text Background Color Index(1 byte)
            // Not supported
            offset++;

            // Block Size & Plain Text Data List
            while (bytes[offset] != 0x00)
            {
                // Block Size(1 byte)
                var blockSize = bytes[offset];
                offset += blockSize;
                offset++;
            }
            offset++;
        }
    }
}
