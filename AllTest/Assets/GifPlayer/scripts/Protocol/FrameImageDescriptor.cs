using System;
using System.IO;
using UnityEngine;

namespace GifPlayer.GifProtocol
{
    /// <summary>
    /// GIF序列帧 描述器
    /// </summary>
    public class FrameImageDescriptor
    {
        /// <summary>
        /// 描述器 标志 分隔符 0x2C
        /// </summary>
        public byte Separator;

        /// <summary>
        /// 左边距离
        /// </summary>
        public ushort MarginLeft;

        /// <summary>
        /// 顶部距离
        /// </summary>
        public ushort MarginTop;

        /// <summary>
        /// 帧宽
        /// </summary>
        public ushort Width;

        /// <summary>
        /// 帧高
        /// </summary>
        public ushort Height;

        #region Packet Fields(1 byte)

        /// <summary>
        /// 是否包含局部色表（为真时使用局部色表,为假时使用全局色表）
        /// </summary>
        public bool LocalColorTableFlag;

        /// <summary>
        /// 是否包含交错
        /// </summary>
        public bool FlagInterlace;

        /// <summary>
        /// 重要颜色排序标志 一般为0 不作处理
        /// </summary>
        public bool FlagSort;

        /// <summary>
        /// 保留字段
        /// </summary>
        public int Reserved;

        /// <summary>
        /// 局部色表大小
        /// </summary>
        public int LocalColorTableSize;

        #endregion

        /// <summary>
        /// 局部色表
        /// </summary>
        public Color32[] LocalColorTable;

        public BytesRecorder LzwedColorNumberBytesRecorder;

        public FrameImageDescriptor(byte[] bytes, ref int offset)
        {
            //描述器标识符 0x2c (1 byte)
            Separator = bytes[offset];
            offset++;

            //图像起始点 (2 byte)
            MarginLeft = BitConverter.ToUInt16(bytes, offset);
            offset += 2;

            //图像起始点 (2 byte)
            MarginTop = BitConverter.ToUInt16(bytes, offset);
            offset += 2;

            //图像宽度 (2 byte)
            Width = BitConverter.ToUInt16(bytes, offset);
            offset += 2;

            //图像高度 (2 byte)
            Height = BitConverter.ToUInt16(bytes, offset);
            offset += 2;

            #region Packet Fields (1 byte) //此处算法参照全局配置

            //是否使用局部色表 (1 bit)
            LocalColorTableFlag = bytes[offset] >> 7 == 1;

            //是否包含交错 (1 bit)
            FlagInterlace = (bytes[offset] >> 6) % 2 == 1;
            //Debug.Log("FlagInterlace: " + FlagInterlace);

            //重要颜色靠前标识 (1 bit)
            FlagSort = (bytes[offset] >> 5) % 2 == 1;

            //保留字段 (2 bit)
            Reserved = (bytes[offset] >> 3) % 4;

            //局部色表长度 (3 bit)
            int power = bytes[offset] % 8 + 1;
            LocalColorTableSize = (int)Math.Pow(2, power);
            LocalColorTable = new Color32[LocalColorTableSize];
            #endregion

            //指针下移
            offset++;

            //是否包含局部色表,计算方法参照全局色表
            if (LocalColorTableFlag)
            {
                //Debug.Log("获取局部色表");
                for (int localColorIndex = 0; localColorIndex < LocalColorTableSize; localColorIndex++)
                {
                    LocalColorTable[localColorIndex] = new Color32(bytes[offset], bytes[offset + 1], bytes[offset + 2], 255);
                    offset += 3;
                }
            }

            //Lzw编码
            LzwedColorNumberBytesRecorder = new BytesRecorder(bytes, offset);
            offset++;

            //紧接着是表示色号的lzw压缩后的字节块 结构：长度+数据+长度+数据+...+结尾符
            while (bytes[offset] != 0x00)   //判断是否为结尾符
            {
                //字节块长度(1 byte)
                var blockSize = bytes[offset];
                offset += blockSize;
                offset++;
            }
            offset++;

            LzwedColorNumberBytesRecorder.SetEndIndexPlus(offset);
        }

        /// <summary>
        /// 获取像素的色号集
        /// </summary>
        public byte[] GetColorNumbers()
        {
            //目标数据长度
            var numbersLength = Height * Width;

            //Lzw解码
            var colorNumbers = GifHelper.LzwDecode(LzwedColorNumberBytesRecorder.GetStream(), numbersLength);

            //整理交错
            if (FlagInterlace)
            {
                //Debug.Log("整理交错");
                colorNumbers = GifHelper.InterlaceDecode(colorNumbers, Width);
            }

            return colorNumbers;
        }

        /// <summary>
        /// 局部色表中获取颜色
        /// </summary>
        public Color32[] GetPixels(FrameGraphicController controller, GraphicsInterchangeFormat gif)
        {
            var colorNumbers = GetColorNumbers();
            var pixels = new Color32[colorNumbers.Length];
            for (int index = 0; index < colorNumbers.Length; index++)
            {
                var colorNumber = colorNumbers[index];
                //透明色
                if (controller.FlagTransparentColor && colorNumber == controller.TransparentColorIndex)
                {
                    //Debug.Log("使用透明色");
                    pixels[index] = Color.clear;
                    continue;
                }

                //全局色
                if (!LocalColorTableFlag)
                {
                    //Debug.Log("使用全局色表");
                    pixels[index] = gif.GetPixel(colorNumber);
                    continue;
                }

                //异常
                if (colorNumber >= LocalColorTableSize)
                {
                    Debug.LogError("超出局部色表长度");
                    pixels[index] = Color.clear;
                    continue;
                }

                //局部色
                pixels[index] = LocalColorTable[colorNumber];
            }
            return pixels;
        }
    }
}
