/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

namespace GifPlayer.GifProtocol
{
    /// <summary>
    /// 下一帧图像预处理方法
    /// </summary>
    public enum NextFrameDisposalMethod
    {
        /// <summary>
        /// 正常处理
        /// <para>0 - No disposal specified. The decoder is not required to take any action. </para>
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 保留当前帧
        /// <para>1 - Do not dispose. The graphic is to be left in place. </para>
        /// </summary>
        Last = 1,

        /// <summary>
        /// 还原背景色
        /// <para>2 - Restore to background color. The area used by the graphic must be restored to the background color. </para>
        /// </summary>
        Bg = 2,

        /// <summary>
        /// 还原上一帧
        /// <para>3 - Restore to previous. The decoder is required to restore the area overwritten by the graphic with what was there prior to rendering the graphic.</para>
        /// </summary>
        Previous = 3,
    }
}
