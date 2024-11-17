/* code by 372792797@qq.com https://assetstore.unity.com/packages/2d/environments/gif-play-plugin-116943 */

using System.Collections;
using System.IO;
using GifPlayer.GifProtocol;
using UnityEngine;

namespace GifPlayer
{
    public static class GifHelper
    {
        public static IEnumerator GetFramesEnumerator(GifData gifData)
        {
            //初始化GIF
            var gif = new GraphicsInterchangeFormat(gifData.Bytes);

            //借助组件单帧加载可以不考虑了，因为Gif需要借助上一帧

            //序列帧集初始化
            gifData.Frames = new SequenceFrame[gif.FrameImageDescriptors.Length];
            //初始化Texture
            var frameTexture = new Texture2D(gif.Width, gif.Height);

            //透明背景
            var transparentPixels = frameTexture.GetPixels32();
            for (var index = 0; index < transparentPixels.Length; index++)
                transparentPixels[index] = Color.clear;

            //背景色
            var backgroundColor = gif.GetPixel(gif.BgColorIndex);
            var backgroundPixels = frameTexture.GetPixels32();
            for (var index = 0; index < backgroundPixels.Length; index++)
                backgroundPixels[index] = backgroundColor;

            //记录下一帧的处理方法
            bool previousReserved = false;

            //处理每个图块
            for (var frameIndex = 0; frameIndex < gifData.Frames.Length; frameIndex++)
            {
                yield return 0;

                //命名
                frameTexture.name = "FrameOfIndex" + frameIndex;
                //图像描述器
                var frameImageDescriptor = gif.FrameImageDescriptors[frameIndex];
                //绘图控制扩展
                var frameGraphicController = gif.FrameGraphicControllers[frameIndex];

                //着色范围
                var blockWidth = frameImageDescriptor.Width;
                var blockHeight = frameImageDescriptor.Height;

                var leftIndex = frameImageDescriptor.MarginLeft;//含
                var rightBorder = leftIndex + blockWidth;//不含

                var topBorder = gif.Height - frameImageDescriptor.MarginTop;//不含
                var bottomIndex = topBorder - blockHeight;//含

                //色表
                var descriptorPixels = frameImageDescriptor.GetPixels(frameGraphicController, gif);
                //色表指针
                var colorIndex = -1;
                //gif的y是从上往下，texture的y是从下往上 
                for (var y = topBorder - 1; y >= bottomIndex; y--)
                {
                    for (var x = leftIndex; x < rightBorder; x++)
                    {
                        colorIndex++;
                        //判断是否保留像素
                        if (previousReserved && descriptorPixels[colorIndex].a == 0)
                            continue;
                        frameTexture.SetPixel(x, y, descriptorPixels[colorIndex]);
                    }
                }

                //保存
                frameTexture.wrapMode = TextureWrapMode.Clamp;
                frameTexture.Apply();

                //添加序列帧,并兵初始化Texture
                gifData.Frames[frameIndex] = new SequenceFrame(frameTexture, frameGraphicController.DelaySeconds);

                //预处理下一帧图像
                previousReserved = false;
                frameTexture = new Texture2D(gif.Width, gif.Height);
                switch (frameGraphicController.NextFrameDisposalMethod)
                {
                    case NextFrameDisposalMethod.Bg:
                        frameTexture.SetPixels32(backgroundPixels);
                        break;

                    case NextFrameDisposalMethod.Previous:
                        frameTexture.SetPixels32(gifData.Frames[frameIndex - 1].Texture.GetPixels32());
                        previousReserved = true;
                        break;

                    default:
                        frameTexture.SetPixels32(gifData.Frames[frameIndex].Texture.GetPixels32());
                        previousReserved = true;
                        break;
                }
            }
        }

        public static void GetFramesVoid(GifData gifData)
        {
            //初始化GIF
            var gif = new GraphicsInterchangeFormat(gifData.Bytes);

            //借助组件单帧加载可以不考虑了，因为Gif需要借助上一帧

            //序列帧集初始化
            gifData.Frames = new SequenceFrame[gif.FrameImageDescriptors.Length];
            //初始化Texture
            var frameTexture = new Texture2D(gif.Width, gif.Height);

            //透明背景
            var transparentPixels = frameTexture.GetPixels32();
            for (var index = 0; index < transparentPixels.Length; index++)
                transparentPixels[index] = Color.clear;

            //背景色
            var backgroundColor = gif.GetPixel(gif.BgColorIndex);
            var backgroundPixels = frameTexture.GetPixels32();
            for (var index = 0; index < backgroundPixels.Length; index++)
                backgroundPixels[index] = backgroundColor;

            //记录下一帧的处理方法
            bool previousReserved = false;

            //处理每个图块
            for (var frameIndex = 0; frameIndex < gifData.Frames.Length; frameIndex++)
            {
                //命名
                frameTexture.name = "FrameOfIndex" + frameIndex;
                //图像描述器
                var frameImageDescriptor = gif.FrameImageDescriptors[frameIndex];
                //绘图控制扩展
                var frameGraphicController = gif.FrameGraphicControllers[frameIndex];

                //着色范围
                var blockWidth = frameImageDescriptor.Width;
                var blockHeight = frameImageDescriptor.Height;

                var leftIndex = frameImageDescriptor.MarginLeft;//含
                var rightBorder = leftIndex + blockWidth;//不含

                var topBorder = gif.Height - frameImageDescriptor.MarginTop;//不含
                var bottomIndex = topBorder - blockHeight;//含

                //色表
                var descriptorPixels = frameImageDescriptor.GetPixels(frameGraphicController, gif);
                //色表指针
                var colorIndex = -1;
                //gif的y是从上往下，texture的y是从下往上 
                for (var y = topBorder - 1; y >= bottomIndex; y--)
                {
                    for (var x = leftIndex; x < rightBorder; x++)
                    {
                        colorIndex++;
                        //判断是否保留像素
                        if (previousReserved && descriptorPixels[colorIndex].a == 0)
                            continue;
                        frameTexture.SetPixel(x, y, descriptorPixels[colorIndex]);
                    }
                }

                //保存
                frameTexture.wrapMode = TextureWrapMode.Clamp;
                frameTexture.Apply();

                //添加序列帧,并兵初始化Texture
                gifData.Frames[frameIndex] = new SequenceFrame(frameTexture, frameGraphicController.DelaySeconds);

                //预处理下一帧图像
                previousReserved = false;
                frameTexture = new Texture2D(gif.Width, gif.Height);
                switch (frameGraphicController.NextFrameDisposalMethod)
                {
                    case NextFrameDisposalMethod.Bg:
                        frameTexture.SetPixels32(backgroundPixels);
                        break;

                    case NextFrameDisposalMethod.Previous:
                        frameTexture.SetPixels32(gifData.Frames[frameIndex - 1].Texture.GetPixels32());
                        previousReserved = true;
                        break;

                    default:
                        frameTexture.SetPixels32(gifData.Frames[frameIndex].Texture.GetPixels32());
                        previousReserved = true;
                        break;
                }
            }
        }

        public static SequenceFrame[] GetFrames(byte[] bytes)
        {
            var gifData = new GifData(bytes);
            GetFramesVoid(gifData);
            return gifData.Frames;
        }

        public static SequenceFrame[] GetFrames(TextAsset gifAsset)
        {
            return GetFrames(gifAsset.bytes);
        }

        /// <summary>
        /// lzwedStream from LzwCodeSize to BlockSize=0
        /// </summary>
        public static byte[] LzwDecode(Stream lzwedStream, int targetLength)
        {
            var targetBytes = new byte[targetLength]; // allocate new pixel array
            var targetIndex = 0;

            // Initialize GIF data stream decoding dictionary.
            var codeSize = lzwedStream.ReadByte();
            var clearCode = 1 << codeSize;
            var endCode = clearCode + 1;
            var dictionaryLength = clearCode + 2;
            var stackSize = targetLength > 4096 ? targetLength : 4096;
            var decodingDictionary = new short[stackSize];
            var decodingKeys = new byte[stackSize];
            int code;
            for (code = 0; code < clearCode; code++)
            {
                decodingDictionary[code] = 0;
                decodingKeys[code] = (byte)code;
            }

            //lzwed bytes block
            var lzwedBlockSize = 0;
            var lzwedBlockOffset = 0;
            byte[] lzwedBlock = new byte[256];

            //Decoded bytes block
            var decodedBlock = new byte[stackSize];
            var decodedBlockOffset = 0;

            //bits
            var bitLength = codeSize + 1;
            var codeMask = (1 << bitLength) - 1;
            var bitIndex = 0;

            var nullCode = -1;
            var oldCode = nullCode;
            int tempCode, first;
            tempCode = first = 0;
            int inCode;

            // Decode GIF pixel stream.
            while (targetIndex < targetLength)
            {
                if (decodedBlockOffset == 0)
                {
                    // Load bytes until there are enough bits for a code.
                    if (bitIndex < bitLength)
                    {
                        // Read a new data block.
                        if (lzwedBlockOffset == lzwedBlockSize)
                        {
                            lzwedBlockSize = lzwedStream.ReadByte();
                            lzwedStream.Read(lzwedBlock, 0, lzwedBlockSize);
                            lzwedBlockOffset = 0;
                        }

                        if (lzwedBlockSize == 0)
                            break;

                        tempCode += lzwedBlock[lzwedBlockOffset++] << bitIndex;
                        bitIndex += 8;
                        continue;
                    }

                    // Get the next code.
                    code = tempCode & codeMask;
                    tempCode >>= bitLength;
                    bitIndex -= bitLength;

                    if (code == clearCode)
                    {
                        // Reset decoding dictionary.
                        dictionaryLength = clearCode + 2;
                        oldCode = nullCode;
                        bitLength = codeSize + 1;
                        codeMask = (1 << bitLength) - 1;
                        continue;
                    }

                    // Interpret the code
                    if ((code == endCode))
                        break;

                    if (oldCode == nullCode)
                    {
                        decodedBlock[decodedBlockOffset++] = decodingKeys[code];
                        oldCode = code;
                        first = code;
                        continue;
                    }

                    inCode = code;

                    if (code == dictionaryLength)
                    {
                        decodedBlock[decodedBlockOffset++] = (byte)first;
                        code = oldCode;
                    }

                    while (code > clearCode)
                    {
                        decodedBlock[decodedBlockOffset++] = decodingKeys[code];
                        code = decodingDictionary[code];
                    }

                    first = decodingKeys[code];

                    // Add a new string to the string table,
                    decodedBlock[decodedBlockOffset++] = (byte)first;
                    decodingDictionary[dictionaryLength] = (short)oldCode;
                    decodingKeys[dictionaryLength] = (byte)first;
                    dictionaryLength++;

                    if (((dictionaryLength & codeMask) == 0))
                    {
                        bitLength++;
                        if (bitLength > 12)
                            bitLength = 12;
                        else
                            codeMask += dictionaryLength;
                    }

                    oldCode = inCode;
                }

                // Pop a pixel off the pixel stack.
                decodedBlockOffset--;
                targetBytes[targetIndex++] = decodedBlock[decodedBlockOffset];
            }

            lzwedStream.Close();
            return targetBytes;
        }

        /// <summary>
        /// 整理交错
        /// </summary>
        public static byte[] InterlaceDecode(byte[] numbers, int width)
        {
            var height = 0;
            var dataIndex = 0;
            var newIndexs = new byte[numbers.Length];
            // Every 8th. row, starting with row 0.
            for (var index = 0; index < newIndexs.Length; index++)
            {
                if (height % 8 == 0)
                {
                    newIndexs[index] = numbers[dataIndex];
                    dataIndex++;
                }
                if (index != 0 && index % width == 0)
                {
                    height++;
                }
            }
            height = 0;
            // Every 8th. row, starting with row 4.
            for (var index = 0; index < newIndexs.Length; index++)
            {
                if (height % 8 == 4)
                {
                    newIndexs[index] = numbers[dataIndex];
                    dataIndex++;
                }
                if (index != 0 && index % width == 0)
                {
                    height++;
                }
            }
            height = 0;
            // Every 4th. row, starting with row 2.
            for (var index = 0; index < newIndexs.Length; index++)
            {
                if (height % 4 == 2)
                {
                    newIndexs[index] = numbers[dataIndex];
                    dataIndex++;
                }
                if (index != 0 && index % width == 0)
                {
                    height++;
                }
            }
            height = 0;
            // Every 2nd. row, starting with row 1.
            for (var index = 0; index < newIndexs.Length; index++)
            {
                if (height % 8 != 0 && height % 8 != 4 && height % 4 != 2)
                {
                    newIndexs[index] = numbers[dataIndex];
                    dataIndex++;
                }
                if (index != 0 && index % width == 0)
                {
                    height++;
                }
            }

            return newIndexs;
        }
    }
}