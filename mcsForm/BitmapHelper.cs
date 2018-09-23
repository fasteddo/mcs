// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


public class BitmapHelper
{
    Bitmap m_bitmap;
    int m_width;
    int m_height;
    bool m_isAlpha;
    Byte [] m_rgbValues;
    BitmapData m_bitmapData;
    IntPtr m_bitmapPtr;
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

        Rectangle rect = new Rectangle(0, 0, Width, Height);
        m_bitmapData = BitmapRef.LockBits(rect, ImageLockMode.ReadWrite, BitmapRef.PixelFormat);
        m_bitmapPtr = m_bitmapData.Scan0;

        int rgbValueLength = IsAlphaBitmap ? 4 : 3;

        int bytes = (Width * Height) * rgbValueLength;
        m_rgbValues = new Byte [bytes - 1];
        Marshal.Copy(m_bitmapPtr, m_rgbValues, 0, m_rgbValues.Length);

        m_locked = true;
    }

    public void Unlock(bool writePixels)
    {
        if (!m_locked)
            throw new Exception("Bitmap not locked.");

        // Write the color values back to the bitmap
        if (writePixels)
            Marshal.Copy(m_rgbValues, 0, m_bitmapPtr, m_rgbValues.Length);

        // Unlock the bitmap
        BitmapRef.UnlockBits(m_bitmapData);

        m_rgbValues = null;
        m_bitmapData = null;
        m_bitmapPtr = (IntPtr)0;

        m_locked = false;
    }

    public void Clear(Color color)
    {
        if (!m_locked)
            throw new Exception("Bitmap not locked.");

        if (IsAlphaBitmap)
        {
            for (int index = 0; index < m_rgbValues.Length - 1; index += 4)
            {
                m_rgbValues[index]     = color.B;
                m_rgbValues[index + 1] = color.G;
                m_rgbValues[index + 2] = color.R;
                m_rgbValues[index + 3] = color.A;
            }
        }
        else
        {
            for (int index = 0; index < m_rgbValues.Length - 1; index += 3)
            {
                m_rgbValues[index]     = color.B;
                m_rgbValues[index + 1] = color.G;
                m_rgbValues[index + 2] = color.R;
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
            int index = ((y * Width + x) * 4);
            m_rgbValues[index]     = color.B;
            m_rgbValues[index + 1] = color.G;
            m_rgbValues[index + 2] = color.R;
            m_rgbValues[index + 3] = color.A;
        }
        else
        {
            int index = ((y * Width + x) * 3);
            m_rgbValues[index]     = color.B;
            m_rgbValues[index + 1] = color.G;
            m_rgbValues[index + 2] = color.R;
        }
    }

    public Color GetPixel(Point location) { return GetPixel(location.X, location.Y); }

    public Color GetPixel(int x, int y)
    {
        if (!m_locked)
            throw new Exception("Bitmap not locked.");

        if (IsAlphaBitmap)
        {
            int index = ((y * Width + x) * 4);
            int b = m_rgbValues[index];
            int g = m_rgbValues[index + 1];
            int r = m_rgbValues[index + 2];
            int a = m_rgbValues[index + 3];
            return Color.FromArgb(a, r, g, b);
        }
        else
        {
            int index = ((y * Width + x) * 3);
            int b = m_rgbValues[index];
            int g = m_rgbValues[index + 1];
            int r = m_rgbValues[index + 2];
            return Color.FromArgb(r, g, b);
        }
    }
}
