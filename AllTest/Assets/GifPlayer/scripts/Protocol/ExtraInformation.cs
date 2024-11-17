/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

using System.Text;

namespace GifPlayer.GifProtocol
{
    /// <summary>
    /// 应用扩展（解析文件时需处理,绘图时可忽略）
    /// </summary>
    public class ExtraInformation
    {
        /// <summary>
        /// 扩展引导标识 0x21
        /// </summary>
        public byte ExtraIntroducer;

        /// <summary>
        /// 应用扩展标识 0xFF
        /// </summary>
        public byte InformationFlag;

        // Block Size
        public byte BlockSize;

        public string ApplicationIdentifier;

        public string ApplicationAuthenticationCode;

        public ExtraInformation(byte[] bytes, ref int offset)
        {
            // Extension Introducer(1 byte)
            // 0x21
            ExtraIntroducer = bytes[offset];
            offset++;

            // Extension Label(1 byte)
            // 0xFF
            InformationFlag = bytes[offset];
            offset++;

            // Block Size(1 byte)
            // 0x0B
            BlockSize = bytes[offset];
            offset++;

            // Application Identifier(8 byte)
            ApplicationIdentifier = Encoding.ASCII.GetString(bytes, offset, 8);
            offset += 8;

            // Application Authentication Code(3 byte)
            ApplicationAuthenticationCode = Encoding.ASCII.GetString(bytes, offset, 3);
            offset += 3;

            // Block Size & Application Data List
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
