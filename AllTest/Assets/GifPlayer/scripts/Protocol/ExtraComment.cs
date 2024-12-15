/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

namespace GifPlayer.GifProtocol
{
    /// <summary>
    /// 注释扩展（解析文件时需处理,绘图时可忽略）
    /// </summary>
    public class ExtraComment
    {
        /// <summary>
        /// 扩展引导标识 0x21
        /// </summary>
        public byte ExtraIntroducer;

        /// <summary>
        /// 注释扩展标识 0xFE
        /// </summary>
        public byte CommentFlag;

        public ExtraComment(byte[] bytes, ref int offset)
        {
            // Extension Introducer(1 byte)
            // 0x21
            ExtraIntroducer = bytes[offset];
            offset++;

            // Comment Label(1 byte)
            // 0xFE
            CommentFlag = bytes[offset];
            offset++;

            // Block Size & Comment Data List
            while (bytes[offset] != 0x00)
            {
                // Block Size (1 byte)
                var blockSize = bytes[offset];
                offset += blockSize;
                offset++;
            }
            offset++;
        }
    }
}
