/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GifPlayer.GifProtocol
{
    /// <summary>
    /// GIF文件结构 结构顺序是有意义的 根据 重要程度、处理先后 排序
    /// </summary>
    public class GraphicsInterchangeFormat
    {
        //全局属性
        public GlobalHeader Header;

        public int Width
        {
            get
            {
                return Header.Width;
            }
        }

        public int Height
        {
            get
            {
                return Header.Height;
            }
        }

        public int BgColorIndex
        {
            get
            {
                return Header.BgColorIndex;
            }
        }


        //附附属性（序列帧）集

        /// <summary>
        /// 帧描述器集
        /// </summary>
        public FrameImageDescriptor[] FrameImageDescriptors;

        public int FrameCount
        {
            get
            {
                return FrameImageDescriptors.Length;
            }
        }

        /// <summary>
        /// 绘图控制器集
        /// </summary>
        public FrameGraphicController[] FrameGraphicControllers;

        /// <summary>
        /// 应用扩展集
        /// </summary>
        public ExtraInformation[] ExtraInformations;

        /// <summary>
        /// 注释扩展集
        /// </summary>
        public ExtraComment[] ExtraComments;

        /// <summary>
        /// 文本扩展集
        /// </summary>
        public ExtraText[] ExtraTexts;

        /// <summary>
        /// 结束符 0x3B
        /// </summary>
        public byte Trailer;

        //方法顺序是有意义的 根据 重要程度、处理先后 排序
        public GraphicsInterchangeFormat(byte[] bytes)
        {
            //指针
            int offset = 0;

            //设置全局属性
            Header = new GlobalHeader(bytes, ref offset);

            //设置扩展属性和帧描述
            var frameGraphicControllers = new List<FrameGraphicController>();
            var frameImageDescriptors = new List<FrameImageDescriptor>();
            var extraComments = new List<ExtraComment>();
            var extraTexts = new List<ExtraText>();
            var extraInformations = new List<ExtraInformation>();

            //处理字节 序号相同就是没有字节能识别 循环结束
            while (true)
            {
                //处理下一字节
                switch (bytes[offset])
                {
                    //帧描述器 0x2C
                    case 0x2C:
                        frameImageDescriptors.Add(new FrameImageDescriptor(bytes, ref offset));
                        break;

                    //扩展 0x21
                    case 0x21:
                        switch (bytes[offset + 1])
                        {
                            //绘图控制扩展 0xF9
                            case 0xF9:
                                frameGraphicControllers.Add(new FrameGraphicController(bytes, ref offset));
                                break;

                            //应用扩展 0xFF
                            case 0xFF:
                                extraInformations.Add(new ExtraInformation(bytes, ref offset));
                                break;

                            //注释扩展 0xFE
                            case 0xFE:
                                extraComments.Add(new ExtraComment(bytes, ref offset));
                                break;

                            //文本扩展 0x01
                            case 0x01:
                                extraTexts.Add(new ExtraText(bytes, ref offset));
                                break;
                        }
                        break;

                    //结尾 0x3B
                    case 0x3B:
                        Trailer = bytes[offset];
                        offset++;
                        FrameGraphicControllers = frameGraphicControllers.ToArray();
                        FrameImageDescriptors = frameImageDescriptors.ToArray();
                        ExtraComments = extraComments.ToArray();
                        ExtraTexts = extraTexts.ToArray();
                        ExtraInformations = extraInformations.ToArray();
                        return;

                    default:
                        var info = string.Format("At bytes length {0}, offset {1}, bytes[offset] {2}, Gif Analyzes Error !",
                            bytes.Length, offset, Convert.ToString((int)bytes[offset], 16));
                        throw new Exception(info);
                }
            }
        }

        /// <summary>
        /// 全局色表中获取颜色
        /// </summary>
        public Color32 GetPixel(int globalColorIndex)
        {
            //异常
            if (!Header.GlobalColorTableFlag || globalColorIndex >= Header.GlobalColorTableSize)
            {
                Debug.LogError("全局色表不存在或者超出全局色表长度");
                return Color.clear;
            }

            //全局色
            return Header.GlobalColorTable[globalColorIndex];
        }
    }
}
