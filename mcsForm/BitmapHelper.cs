// license:BSD-3-Clause
// copyright-holders:Edward Fast

//#define USE_UNSAFE_FUNCTIONS

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace BitmapHelperNS
{
public class BitmapHelper
{
    Bitmap m_bitmap;
    int m_width;
    int m_height;
    bool m_isAlpha;
    BitmapData m_bitmapData;
    int m_bitmapSizeBytes;
#if !USE_UNSAFE_FUNCTIONS
    IntPtr m_bitmapPtr;
    byte [] m_bitmapDataCopy;
#endif

    bool m_locked = false;


    public Bitmap BitmapRef { get { return m_bitmap; } }
    public int Width { get { return m_width; } }
    public int Height { get { return m_height; } }
    public bool IsAlphaBitmap { get { return m_isAlpha; } }


    public BitmapHelper(Bitmap bitmap)
    {
        if (bitmap.PixelFormat == (bitmap.PixelFormat | PixelFormat.Indexed))
            throw new Exception("Cannot lock an Indexed image.");

        m_bitmap = bitmap;
        m_width = BitmapRef.Width;
        m_height = BitmapRef.Height;
        m_isAlpha = BitmapRef.PixelFormat == (BitmapRef.PixelFormat | PixelFormat.Alpha);
    }

    public void Lock()
    {
        if (m_locked)
            throw new Exception("Bitmap already locked.");

        // https://stackoverflow.com/questions/1563038/fast-work-with-bitmaps-in-c-sharp

        Rectangle rect = new(0, 0, Width, Height);
        m_bitmapData = BitmapRef.LockBits(rect, ImageLockMode.ReadWrite, BitmapRef.PixelFormat);

        //int rgbValueLength = IsAlphaBitmap ? 4 : 3;
        int rgbValueLength = 4;  // TODO fix this

        m_bitmapSizeBytes = (Width * Height) * rgbValueLength;

#if !USE_UNSAFE_FUNCTIONS
        m_bitmapPtr = m_bitmapData.Scan0;

        if (m_bitmapDataCopy == null || m_bitmapDataCopy.Length != m_bitmapSizeBytes)
            m_bitmapDataCopy = new byte [m_bitmapSizeBytes];

        Marshal.Copy(m_bitmapPtr, m_bitmapDataCopy, 0, m_bitmapDataCopy.Length);
#endif

        m_locked = true;
    }

    public void Unlock(bool writePixels = true)
    {
        if (!m_locked)
            throw new Exception("Bitmap not locked.");

#if !USE_UNSAFE_FUNCTIONS
        // Write the color values back to the bitmap
        if (writePixels)
            Marshal.Copy(m_bitmapDataCopy, 0, m_bitmapPtr, m_bitmapDataCopy.Length);
#endif

        // Unlock the bitmap
        BitmapRef.UnlockBits(m_bitmapData);

        m_bitmapData = null;
        m_bitmapSizeBytes = 0;
#if !USE_UNSAFE_FUNCTIONS
        //m_rgbValues = null;  // don't null this, keep this memory around for re-use
        m_bitmapPtr = (IntPtr)0;
#endif

        m_locked = false;
    }

#if USE_UNSAFE_FUNCTIONS
    public unsafe Span<byte> GetBitmapData()
    {
        // NOTE: no lock check!
        return new Span<byte>(m_bitmapData.Scan0.ToPointer(), m_bitmapSizeBytes);
    }

    public unsafe Span<byte> GetBitmapData(int start, int count)
    {
        // NOTE: no lock check!
        return new Span<byte>(m_bitmapData.Scan0.ToPointer(), m_bitmapSizeBytes).Slice(start, count);
    }
#else
    public Span<byte> GetBitmapData()
    {
        // NOTE: no lock check!
        return new Span<byte>(m_bitmapDataCopy);
    }

    public Span<byte> GetBitmapData(int start, int count)
    {
        // NOTE: no lock check!
        return new Span<byte>(m_bitmapDataCopy, start, count);
    }
#endif


    public void Clear(Color color)
    {
        if (!m_locked)
            throw new Exception("Bitmap not locked.");

        if (true)
        {
            var bitmapData = GetBitmapData();
            for (int index = 0; index < bitmapData.Length - 1; index += 4)
            {
                bitmapData[index]     = color.B;
                bitmapData[index + 1] = color.G;
                bitmapData[index + 2] = color.R;
                bitmapData[index + 3] = color.A;
            }
        }
        else
        {
            var bitmapData = GetBitmapData();
            for (int index = 0; index < bitmapData.Length - 1; index += 3)
            {
                bitmapData[index]     = color.B;
                bitmapData[index + 1] = color.G;
                bitmapData[index + 2] = color.R;
            }
        }
    }

    public void SetPixel(Point location, Color color) { SetPixel(location.X, location.Y, color); }

    public void SetPixel(int x, int y, Color color)
    {
        if (!m_locked)
            throw new Exception("Bitmap not locked.");

        if (IsAlphaBitmap)
        {
            var bitmapData = GetBitmapData();
            int index = ((y * Width + x) * 4);
            bitmapData[index]     = color.B;
            bitmapData[index + 1] = color.G;
            bitmapData[index + 2] = color.R;
            bitmapData[index + 3] = color.A;
        }
        else
        {
            var bitmapData = GetBitmapData();
            int index = ((y * Width + x) * 3);
            bitmapData[index]     = color.B;
            bitmapData[index + 1] = color.G;
            bitmapData[index + 2] = color.R;
        }
    }

    const int ARGBAlphaShift  = 24;
    const int ARGBRedShift    = 16;
    const int ARGBGreenShift  = 8;
    const int ARGBBlueShift   = 0;

    public delegate void SetPixelRawFunc(int arrayIndex, UInt32 color);

    public void SetPixelRawNoAlpha(int arrayIndex, UInt32 color)
    {
        // note: no lock check for perf
        //if (!m_locked)
        //    throw new Exception("Bitmap not locked.");

        var bitmapData = GetBitmapData();
        bitmapData[arrayIndex]     = (byte)((color >> ARGBBlueShift) & 0xFF);
        bitmapData[arrayIndex + 1] = (byte)((color >> ARGBGreenShift) & 0xFF);
        bitmapData[arrayIndex + 2] = (byte)((color >> ARGBRedShift) & 0xFF);
    }

    public void SetPixelRawAlpha(int arrayIndex, UInt32 color)
    {
        // note: no lock check for perf
        //if (!m_locked)
        //    throw new Exception("Bitmap not locked.");

        var bitmapData = GetBitmapData();
        bitmapData[arrayIndex]     = (byte)((color >> ARGBBlueShift) & 0xFF);
        bitmapData[arrayIndex + 1] = (byte)((color >> ARGBGreenShift) & 0xFF);
        bitmapData[arrayIndex + 2] = (byte)((color >> ARGBRedShift) & 0xFF);
        bitmapData[arrayIndex + 3] = (byte)((color >> ARGBAlphaShift) & 0xFF);
    }

    public int GetRawPixelSize()
    {
        return IsAlphaBitmap ? 4 : 3;
    }

    public int GetRawYIndex(int y)
    {
        return ((y * Width) * GetRawPixelSize());
    }

    public Color GetPixel(Point location) { return GetPixel(location.X, location.Y); }

    public Color GetPixel(int x, int y)
    {
        if (!m_locked)
            throw new Exception("Bitmap not locked.");

        if (IsAlphaBitmap)
        {
            var bitmapData = GetBitmapData();
            int index = ((y * Width + x) * 4);
            int b = bitmapData[index];
            int g = bitmapData[index + 1];
            int r = bitmapData[index + 2];
            int a = bitmapData[index + 3];
            return Color.FromArgb(a, r, g, b);
        }
        else
        {
            var bitmapData = GetBitmapData();
            int index = ((y * Width + x) * 3);
            int b = bitmapData[index];
            int g = bitmapData[index + 1];
            int r = bitmapData[index + 2];
            return Color.FromArgb(r, g, b);
        }
    }
}
}
