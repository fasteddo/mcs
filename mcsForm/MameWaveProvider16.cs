// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using NAudio;
using NAudio.Wave;


namespace mcsForm
{
    public class MameWaveProvider16 : WaveProvider16
    {
        int naudioUpdateCount = 0;


        public MameWaveProvider16() { }


        public override int Read(short[] buffer, int offset, int sampleCount)
        {
            int actualCount = 0;

            if (mame.mame_machine_manager.instance() == null || mame.mame_machine_manager.instance().osd() == null)
            {
                //while (actualCount < sampleCount)
                //{
                //    buffer[actualCount + offset] = 0;  // 20000;
                //    actualCount++;
                //}
                buffer[0] = 0;
                actualCount++;

                return actualCount;
            }

            osd_interface_WinForms osd = (osd_interface_WinForms)mame.mame_machine_manager.instance().osd();

            if (naudioUpdateCount++ % 10 == 0)
                System.Diagnostics.Trace.WriteLine(string.Format("MameWaveProvider16.Read() - {0} - buffer: {1} offset: {2} sampleCount: {3} osd.m_audiobuffer: {4}", naudioUpdateCount, buffer.Length, offset, sampleCount, osd.m_audiobuffer.Count));

            lock (this)
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    if (osd.m_audiobuffer.Count == 0)
                        break;

                    Int16 sample = osd.m_audiobuffer.Dequeue();
                    buffer[i + offset] = sample;
                    actualCount++;
                }

                osd.m_audiobuffer.Clear();

                //while (actualCount < sampleCount)
                //{
                //    buffer[actualCount + offset] = 0;  // 20000;
                //    actualCount++;
                //}

                if (actualCount == 0)
                {
                    buffer[0] = 0;
                    actualCount++;
                }
            }

            return actualCount;
        }
    }
}
