/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

using System;
using System.Text;
using UnityEngine;

namespace GifPlayer.GifProtocol
{
    public class GlobalHeader
    {
        /// <summary>
        /// GIF声明
        /// </summary>
        public string Signature;

        /// <summary>
        /// 版本声明
        /// </summary>
        public string Version;

        /// <summary>
        /// 画布宽
        /// </summary>
        public ushort Width;

        /// <summary>
        /// 画布高
        /// </summary>
        public ushort Height;

        #region Packet Fields(1 byte)
        /// <summary>
        /// 是否包含全局色表
        /// </summary>
        public bool GlobalColorTableFlag;

        /// <summary>
        /// 颜色分辨率 （不处理）
        /// </summary>
        public int ColorResolution;

        /// <summary>
        /// 重要颜色前排标识 一般为false （不处理）
        /// </summary>
        public bool Flagsort;

        /// <summary>
        /// 全局色表的数量 范围 0-256 所占字节再*3
        /// </summary>
        public int GlobalColorTableSize;
        #endregion

        /// <summary>
        /// 背景色色号
        /// </summary>
        public byte BgColorIndex;

        /// <summary>
        /// 像素宽高比
        /// </summary>
        public byte PixelAspectRatio;

        /// <summary>
        /// 全局色表 3个byte一组
        /// </summary>
        public Color32[] GlobalColorTable;

        public GlobalHeader(byte[] bytes, ref int offset)
        {
            // GIF声明 (GIF) (3 byte)
            Signature = Encoding.ASCII.GetString(bytes, 0, 3);

            // 版本声明 (87a) (3 byte)
            Version = Encoding.ASCII.GetString(bytes, 3, 3);

            // 画布宽度(2 byte)
            Width = BitConverter.ToUInt16(bytes, 6);

            // 画布高度(2 byte)
            Height = BitConverter.ToUInt16(bytes, 8);

            #region Packet Fields (1 byte)
            // 是否包含全局色表(1 bit)
            // 为1 时表明Logical Screen Descriptor 后面跟的是全局颜色表。
            GlobalColorTableFlag = bytes[10] >> 7 == 1;

            // 颜色分辨率(3 bit)
            //值加1 代表颜色表中每种基色用多少位表示,如为“111”时表示每种基色用8 位表示,则颜色表中每项为3Byte。由于该值有时可为0,一般在解码程序中,该3 位不作处理,而直接由Global Color Table Size 算出颜色表大小。
            ColorResolution = (bytes[10] >> 4) % 8 + 1;

            // 重要颜色前排标识(1 bit)
            //表示重要颜色排序标志,标志为1 时,表示颜色表中重要的颜色排在前面,有利于颜色数较少的解码器选择最好的颜色。一般该标志为0,不作处理。
            Flagsort = (bytes[10] >> 3) % 2 == 1;

            //色表长度（一个色表有3字节）(3 bit)
            //值加1 作为2 的幂,算得的数即为颜色表的项数,实际上颜色表每项由RGB 三基色构成,每种基色占一个字节,则颜色表占字节数为项数的3 倍。由于最大值为“111”,故颜色表的项数最多为256项,即256 种颜色,8 位每基色则颜色表大小为768 Bytes。
            var power = bytes[10] % 8 + 1;
            GlobalColorTableSize = (int)Math.Pow(2, power);
            #endregion

            // 背景色色号(1 byte)
            //表示背景颜色索引值*。可以这样理解：在指定大小显示区,GIF 图像的大小可能小于显示区域大小,显示区中剩余的区域则一律用背景颜色索引值在全局颜色表中对应的颜色填充。在实际解码过程中,在显示图像之前可将显示区域全部用该颜色填充。
            BgColorIndex = bytes[11];

            // 像素宽高比(1 byte)
            //表示像素宽高比,一般为0,不作处理,直接以Logical Screen 宽和高作处理。如该项不为0,则参照GIF89a 标准【1】计算。
            PixelAspectRatio = bytes[12];

            offset = 13;

            //获取全局色表
            GlobalColorTable = new Color32[GlobalColorTableSize];
            //判断是否包含全局色表
            if (GlobalColorTableFlag)
            {
                //色表长度*3（字节数）
                for (var globalColorIndex = 0; globalColorIndex < GlobalColorTableSize; globalColorIndex++)
                {
                    GlobalColorTable[globalColorIndex] = new Color32(bytes[offset], bytes[offset + 1], bytes[offset + 2], 255);
                    offset += 3;
                }
            }
        }
    }
}
