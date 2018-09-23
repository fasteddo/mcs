// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    partial class m6502_device : cpu_device
    {
        void adc_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read(TMP);
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void adc_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read(TMP);
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void adc_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = read(TMP);
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void adc_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            //}

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = read(TMP);
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void adc_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = read(TMP);
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void adc_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = read(TMP);
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void adc_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            do_adc(read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void adc_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            do_adc(read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void adc_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            do_adc(read((UInt16)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void adc_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
        //case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            do_adc(read((UInt16)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void adc_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void adc_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void adc_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP);
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void adc_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP);
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void adc_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((byte)(TMP+X));
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void adc_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((byte)(TMP+X));
            icount--;

            do_adc((byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void and_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void and_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void and_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void and_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            //}

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void and_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void and_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void and_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            A &= read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void and_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            A &= read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void and_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void and_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void and_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            A &= read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void and_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
        //case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A &= read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void and_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void and_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            A &= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void and_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            A &= read((byte)(TMP+X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void and_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A &= read((byte)(TMP+X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void asl_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void asl_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void asl_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void asl_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void asl_acc_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            A = do_asl(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void asl_acc_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            A = do_asl(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void asl_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void asl_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void asl_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void asl_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void bcc_rel_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_C) == 0) {

            if(icount == 0) { inst_substate = 2; return; }
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void bcc_rel_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_C) == 0) {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                //}

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void bcs_rel_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_C) != 0) {

            if(icount == 0) { inst_substate = 2; return; }
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void bcs_rel_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_C) != 0) {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                //}

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void beq_rel_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_Z) != 0) {

            if(icount == 0) { inst_substate = 2; return; }
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void beq_rel_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_Z) != 0) {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                //}

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void bit_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            do_bit(read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void bit_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            do_bit(read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void bit_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            do_bit(read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void bit_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            do_bit(read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void bmi_rel_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_N) != 0) {

            if(icount == 0) { inst_substate = 2; return; }
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void bmi_rel_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_N) != 0) {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                //}

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void bne_rel_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_Z) == 0) {

            if(icount == 0) { inst_substate = 2; return; }
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void bne_rel_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_Z) == 0) {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                //}

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void bpl_rel_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_N) == 0) {

            if(icount == 0) { inst_substate = 2; return; }
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void bpl_rel_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_N) == 0) {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                //}

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void brk_imp_full()
        {

            // The 6502 bug when a nmi occurs in a brk is reproduced (case !irq_taken && nmi_state)

            if(irq_taken) {

            if(icount == 0) { inst_substate = 1; return; }
                read_pc_noinc();
            icount--;

            } else {

            if(icount == 0) { inst_substate = 2; return; }
                read_pc();
            icount--;

            }

            if(icount == 0) { inst_substate = 3; return; }
            write(SP, (byte)(PC >> 8));
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 4; return; }
            write(SP, (byte)PC);
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 5; return; }
            write(SP, irq_taken ? (byte)(P & ~(byte)F.F_B) : P);
            icount--;

            dec_SP();

            if(nmi_state) {

            if(icount == 0) { inst_substate = 6; return; }
                PC = read_arg(0xfffa);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
                PC = set_h(PC, read_arg(0xfffb));
            icount--;

                nmi_state = false;

                m_diexec.standard_irq_callback((int)LINE.NMI_LINE);

            } else {

            if(icount == 0) { inst_substate = 8; return; }
                PC = read_arg(0xfffe);
            icount--;

            if(icount == 0) { inst_substate = 9; return; }
                PC = set_h(PC, read_arg(0xffff));
            icount--;

                if(irq_taken)

                    m_diexec.standard_irq_callback((int)LINE.IRQ_LINE);

            }

            irq_taken = false;

            P |= (byte)F.F_I; // Do *not* move after the prefetch

            if(icount == 0) { inst_substate = 10; return; }
            prefetch();
            icount--;

            inst_state = -1;

        }

        void brk_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            // The 6502 bug when a nmi occurs in a brk is reproduced (case !irq_taken && nmi_state)

            if(irq_taken) {

            if(icount == 0) { inst_substate = 1; return; }
        //case 1:
                read_pc_noinc();
            icount--;

            } else {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc();
            icount--;

            }

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 1:
                read_pc_noinc();
            icount--;

#if false
            } else {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc();
            icount--;

            }
#endif

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 2:
                read_pc();
            icount--;

            //}

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(SP, (byte)(PC >> 8));
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(SP, (byte)PC);
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(SP, irq_taken ? (byte)(P & ~(byte)F.F_B) : P);
            icount--;

            dec_SP();

            if(nmi_state) {

            if(icount == 0) { inst_substate = 6; return; }
        //case 6:
                PC = read_arg(0xfffa);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
        //case 7:
                PC = set_h(PC, read_arg(0xfffb));
            icount--;

                nmi_state = false;

                m_diexec.standard_irq_callback((int)LINE.NMI_LINE);

            } else {

            if(icount == 0) { inst_substate = 8; return; }
        //case 8:
                PC = read_arg(0xfffe);
            icount--;

            if(icount == 0) { inst_substate = 9; return; }
        //case 9:
                PC = set_h(PC, read_arg(0xffff));
            icount--;

                if(irq_taken)

                    m_diexec.standard_irq_callback((int)LINE.IRQ_LINE);

            }

            irq_taken = false;

            P |= (byte)F.F_I; // Do *not* move after the prefetch

            if(icount == 0) { inst_substate = 10; return; }
            goto case 10;
        case 6:
                PC = read_arg(0xfffa);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
        //case 7:
                PC = set_h(PC, read_arg(0xfffb));
            icount--;

                nmi_state = false;

                m_diexec.standard_irq_callback((int)LINE.NMI_LINE);

#if false
            } else {

            if(icount == 0) { inst_substate = 8; return; }
        //case 8:
                PC = read_arg(0xfffe);
            icount--;

            if(icount == 0) { inst_substate = 9; return; }
        //case 9:
                PC = set_h(PC, read_arg(0xffff));
            icount--;

                if(irq_taken)

                    standard_irq_callback(IRQ_LINE);

            }
#endif

            irq_taken = false;

            P |= (byte)F.F_I; // Do *not* move after the prefetch

            if(icount == 0) { inst_substate = 10; return; }
            goto case 10;
        case 7:
                PC = set_h(PC, read_arg(0xfffb));
            icount--;

                nmi_state = false;

                m_diexec.standard_irq_callback((int)LINE.NMI_LINE);

#if false
            } else {

            if(icount == 0) { inst_substate = 8; return; }
        //case 8:
                PC = read_arg(0xfffe);
            icount--;

            if(icount == 0) { inst_substate = 9; return; }
        //case 9:
                PC = set_h(PC, read_arg(0xffff));
            icount--;

                if(irq_taken)

                    standard_irq_callback(IRQ_LINE);

            }
#endif

            irq_taken = false;

            P |= (byte)F.F_I; // Do *not* move after the prefetch

            if(icount == 0) { inst_substate = 10; return; }
            goto case 10;
        case 8:
                PC = read_arg(0xfffe);
            icount--;

            if(icount == 0) { inst_substate = 9; return; }
        //case 9:
                PC = set_h(PC, read_arg(0xffff));
            icount--;

                if(irq_taken)

                    m_diexec.standard_irq_callback((int)LINE.IRQ_LINE);

            //}

            irq_taken = false;

            P |= (byte)F.F_I; // Do *not* move after the prefetch

            if(icount == 0) { inst_substate = 10; return; }
            goto case 10;
        case 9:
                PC = set_h(PC, read_arg(0xffff));
            icount--;

                if(irq_taken)

                    m_diexec.standard_irq_callback((int)LINE.IRQ_LINE);

            //}

            irq_taken = false;

            P |= (byte)F.F_I; // Do *not* move after the prefetch

            if(icount == 0) { inst_substate = 10; return; }
            goto case 10;
        case 10:
            prefetch();
            icount--;

            inst_state = -1;
            break;
        }
            inst_substate = 0;
        }


        void bvc_rel_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_V) == 0) {

            if(icount == 0) { inst_substate = 2; return; }
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void bvc_rel_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_V) == 0) {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                //}

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void bvs_rel_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_V) != 0) {

            if(icount == 0) { inst_substate = 2; return; }
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void bvs_rel_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if((P & (byte)F.F_V) != 0) {

            if(icount == 0) { inst_substate = 2; return; }
        //case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 2:
                read_pc_noinc();
            icount--;

                if(page_changing(PC, (sbyte)(TMP))) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                }

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                    read_arg(set_l(PC, (byte)(PC+(sbyte)(TMP))));
            icount--;

                //}

                PC += (UInt16)(sbyte)(TMP);

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void clc_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            P &= unchecked((byte)~F.F_C);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void clc_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            P &= unchecked((byte)~F.F_C);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cld_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            P &= unchecked((byte)~F.F_D);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void cld_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            P &= unchecked((byte)~F.F_D);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cli_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

            P &= unchecked((byte)~F.F_I); // Do *not* move it before the prefetch

        }

        void cli_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;

            P &= unchecked((byte)~F.F_I); // Do *not* move it before the prefetch
            break;
        }
            inst_substate = 0;
        }


        void clv_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            P &= unchecked((byte)~F.F_V);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void clv_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            P &= unchecked((byte)~F.F_V);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cmp_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read(TMP);
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void cmp_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read(TMP);
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cmp_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = read(TMP);
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void cmp_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            //}

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = read(TMP);
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cmp_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = read(TMP);
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void cmp_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = read(TMP);
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cmp_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            do_cmp(A, read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void cmp_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            do_cmp(A, read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cmp_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            do_cmp(A, read((UInt16)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void cmp_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
        //case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            do_cmp(A, read((UInt16)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cmp_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void cmp_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cmp_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP);
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void cmp_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP);
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cmp_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((byte)(TMP+X));
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void cmp_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((byte)(TMP+X));
            icount--;

            do_cmp(A, (byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cpx_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read(TMP);
            icount--;

            do_cmp(X, (byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void cpx_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read(TMP);
            icount--;

            do_cmp(X, (byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cpx_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            do_cmp(X, (byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void cpx_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            do_cmp(X, (byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cpx_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP);
            icount--;

            do_cmp(X, (byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void cpx_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP);
            icount--;

            do_cmp(X, (byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cpy_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read(TMP);
            icount--;

            do_cmp(Y, (byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void cpy_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read(TMP);
            icount--;

            do_cmp(Y, (byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cpy_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            do_cmp(Y, (byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void cpy_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            do_cmp(Y, (byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void cpy_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP);
            icount--;

            do_cmp(Y, (byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void cpy_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP);
            icount--;

            do_cmp(Y, (byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dec_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void dec_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dec_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void dec_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dec_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void dec_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dec_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void dec_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dex_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            X--;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void dex_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            X--;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dey_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            Y--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void dey_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            Y--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void eor_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void eor_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void eor_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void eor_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            //}

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void eor_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void eor_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void eor_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void eor_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void eor_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            A ^= read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void eor_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
        //case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A ^= read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void eor_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            A ^= read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void eor_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            A ^= read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void eor_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void eor_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            A ^= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void eor_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            A ^= read((byte)(TMP+X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void eor_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A ^= read((byte)(TMP+X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void inc_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void inc_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void inc_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void inc_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void inc_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void inc_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void inc_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void inc_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void inx_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            X++;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void inx_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            X++;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void iny_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            Y++;

            set_nz(Y);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void iny_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            Y++;

            set_nz(Y);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void jmp_adr_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            PC = TMP;

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void jmp_adr_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            PC = TMP;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void jmp_ind_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            PC = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            PC = set_h(PC, read(set_l(TMP, (byte)(TMP+1))));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void jmp_ind_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            PC = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            PC = set_h(PC, read(set_l(TMP, (byte)(TMP+1))));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void jsr_adr_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(SP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(SP, (byte)(PC>>8));
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 4; return; }
            write(SP, (byte)PC);
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 5; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            PC = TMP;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void jsr_adr_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(SP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(SP, (byte)(PC>>8));
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(SP, (byte)PC);
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP = set_h(TMP, read_pc());
            icount--;

            PC = TMP;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lda_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            A = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void lda_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lda_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            A = read((UInt16)(TMP + X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void lda_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            A = read((UInt16)(TMP + X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lda_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            A = read((UInt16)(TMP + Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void lda_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            A = read((UInt16)(TMP + Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lda_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            A = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void lda_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lda_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            A = read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void lda_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
        //case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A = read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lda_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            A = read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void lda_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            A = read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lda_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            A = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void lda_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            A = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lda_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            A = read((byte)(TMP+X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void lda_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A = read((byte)(TMP+X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldx_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            X = read(TMP);
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void ldx_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            X = read(TMP);
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldx_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            X = read((UInt16)(TMP + Y));
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void ldx_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            X = read((UInt16)(TMP + Y));
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldx_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            X = read_pc();
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void ldx_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            X = read_pc();
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldx_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            X = read(TMP);
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void ldx_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            X = read(TMP);
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldx_zpy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            X = read((byte)(TMP+Y));
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void ldx_zpy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            X = read((byte)(TMP+Y));
            icount--;

            set_nz(X);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldy_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            Y = read(TMP);
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void ldy_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            Y = read(TMP);
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldy_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            Y = read(TMP);
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void ldy_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            //}

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            Y = read(TMP);
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldy_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            Y = read_pc();
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void ldy_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            Y = read_pc();
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldy_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            Y = read(TMP);
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void ldy_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            Y = read(TMP);
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ldy_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            Y = read((byte)(TMP+X));
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void ldy_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            Y = read((byte)(TMP+X));
            icount--;

            set_nz(Y);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lsr_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void lsr_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lsr_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void lsr_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lsr_acc_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            A = do_lsr(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void lsr_acc_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            A = do_lsr(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lsr_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void lsr_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lsr_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void lsr_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void nop_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void nop_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ora_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void ora_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ora_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void ora_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            //}

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ora_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void ora_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ora_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            A |= read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void ora_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            A |= read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ora_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void ora_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ora_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            A |= read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void ora_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
        //case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A |= read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ora_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void ora_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            A |= read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ora_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            A |= read((byte)(TMP+X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void ora_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A |= read((byte)(TMP+X));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void pha_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            write(SP, A);
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void pha_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            write(SP, A);
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void php_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            write(SP, P);
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void php_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            write(SP, P);
            icount--;

            dec_SP();

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void pla_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 3; return; }
            A = read(SP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void pla_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A = read(SP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void plp_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 3; return; }
            TMP = (UInt16)(read(SP) | (byte)(F.F_B|F.F_E));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

            P = (byte)TMP; // Do *not* move it before the prefetch

        }

        void plp_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = (UInt16)(read(SP) | (byte)(F.F_B|F.F_E));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;

            P = (byte)TMP; // Do *not* move it before the prefetch
            break;
        }
            inst_substate = 0;
        }


        void rol_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void rol_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rol_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void rol_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rol_acc_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            A = do_rol(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void rol_acc_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            A = do_rol(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rol_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void rol_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rol_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void rol_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ror_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void ror_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ror_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void ror_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ror_acc_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            A = do_ror(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void ror_acc_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            A = do_ror(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ror_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void ror_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ror_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void ror_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rti_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 3; return; }
            P = (byte)(read(SP) | (byte)(F.F_B|F.F_E));
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 4; return; }
            PC = read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 5; return; }
            PC = set_h(PC, read(SP));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void rti_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            P = (byte)(read(SP) | (byte)(F.F_B|F.F_E));
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            PC = read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            PC = set_h(PC, read(SP));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rts_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 3; return; }
            PC = read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 4; return; }
            PC = set_h(PC, read(SP));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            read_pc();
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void rts_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            PC = read(SP);
            icount--;

            inc_SP();

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            PC = set_h(PC, read(SP));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            read_pc();
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sbc_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read(TMP);
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void sbc_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read(TMP);
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sbc_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = read(TMP);
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void sbc_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            //}

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = read(TMP);
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sbc_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = read(TMP);
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void sbc_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = read(TMP);
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sbc_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            do_sbc(read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void sbc_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            do_sbc(read(TMP));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sbc_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            do_sbc(read((UInt16)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void sbc_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
        //case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            do_sbc(read((UInt16)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sbc_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void sbc_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sbc_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP);
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void sbc_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP);
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sbc_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((byte)(TMP+X));
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void sbc_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((byte)(TMP+X));
            icount--;

            do_sbc((byte)TMP);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sec_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            P |= (byte)F.F_C;

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void sec_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            P |= (byte)F.F_C;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sed_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            P |= (byte)F.F_D;

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void sed_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            P |= (byte)F.F_D;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sei_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

            P |= (byte)F.F_I; // Do *not* move it before the prefetch

        }

        void sei_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;

            P |= (byte)F.F_I; // Do *not* move it before the prefetch
            break;
        }
            inst_substate = 0;
        }


        void sta_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, A);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void sta_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, A);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sta_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write((UInt16)(TMP+X), A);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void sta_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write((UInt16)(TMP+X), A);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sta_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write((UInt16)(TMP+Y), A);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void sta_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write((UInt16)(TMP+Y), A);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sta_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, A);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void sta_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, A);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sta_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write((UInt16)(TMP+Y), A);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void sta_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write((UInt16)(TMP+Y), A);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sta_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            write(TMP, A);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void sta_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            write(TMP, A);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sta_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write((byte)(TMP+X), A);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void sta_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write((byte)(TMP+X), A);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void stx_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, X);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void stx_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, X);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void stx_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            write(TMP, X);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void stx_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            write(TMP, X);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void stx_zpy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write((byte)(TMP+Y), X);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void stx_zpy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write((byte)(TMP+Y), X);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sty_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, Y);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void sty_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, Y);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sty_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            write(TMP, Y);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void sty_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            write(TMP, Y);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sty_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write((byte)(TMP+X), Y);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void sty_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write((byte)(TMP+X), Y);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void tax_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            X = A;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void tax_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            X = A;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void tay_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            Y = A;

            set_nz(Y);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void tay_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            Y = A;

            set_nz(Y);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void tsx_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            X = (byte)SP;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void tsx_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            X = (byte)SP;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void txa_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            A = X;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void txa_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            A = X;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void txs_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            SP = set_l(SP, X);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void txs_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            SP = set_l(SP, X);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void tya_imp_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc_noinc();
            icount--;

            A = Y;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void tya_imp_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc_noinc();
            icount--;

            A = Y;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void reset_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            PC = read_arg(0xfffc);
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            PC = set_h(PC, read_arg(0xfffd));
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

            inst_state = -1;

        }

        void reset_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            PC = read_arg(0xfffc);
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            PC = set_h(PC, read_arg(0xfffd));
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;

            inst_state = -1;
            break;
        }
            inst_substate = 0;
        }


        void dcp_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void dcp_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dcp_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void dcp_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dcp_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void dcp_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dcp_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void dcp_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dcp_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void dcp_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dcp_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void dcp_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void dcp_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void dcp_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            do_cmp(A, TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void isb_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void isb_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void isb_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void isb_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void isb_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void isb_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void isb_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void isb_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void isb_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void isb_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void isb_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void isb_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void isb_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void isb_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2++;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            do_sbc(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lax_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            A = X = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void lax_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A = X = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lax_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            A = X = read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void lax_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            A = X = read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lax_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            A = X = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void lax_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A = X = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lax_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            A = X = read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void lax_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 4; return; }
        //case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 4:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            A = X = read((UInt16)(TMP+Y));
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lax_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            A = X = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void lax_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            A = X = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lax_zpy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+Y);

            if(icount == 0) { inst_substate = 3; return; }
            A = X = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void lax_zpy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+Y);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            A = X = read(TMP);
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rla_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void rla_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rla_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void rla_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rla_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void rla_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rla_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void rla_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rla_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void rla_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rla_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void rla_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rla_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void rla_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_rol(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            A &= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rra_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void rra_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rra_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void rra_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rra_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void rra_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rra_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void rra_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rra_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void rra_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rra_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void rra_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void rra_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void rra_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_ror(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            do_adc(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sax_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            TMP2 = (byte)(A & X);

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void sax_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            TMP2 = (byte)(A & X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sax_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            TMP2 = (byte)(A & X);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void sax_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            TMP2 = (byte)(A & X);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sax_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            TMP2 = (byte)(A & X);

            if(icount == 0) { inst_substate = 2; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void sax_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            TMP2 = (byte)(A & X);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sax_zpy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+Y);

            TMP2 = (byte)(A & X);

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void sax_zpy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+Y);

            TMP2 = (byte)(A & X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sbx_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            X &= A;

            if(X < TMP2)

                P &= unchecked((byte)~F.F_C);

            else

                P |= (byte)F.F_C;

            X -= TMP2;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void sbx_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            X &= A;

            if(X < TMP2)

                P &= unchecked((byte)~F.F_C);

            else

                P |= (byte)F.F_C;

            X -= TMP2;

            set_nz(X);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sha_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP2 = (byte)(A & X & ((TMP >> 8)+1));

            if(page_changing(TMP, Y))

                TMP = set_h((UInt16)(TMP+Y), TMP2);

            else

                TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void sha_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP2 = (byte)(A & X & ((TMP >> 8)+1));

            if(page_changing(TMP, Y))

                TMP = set_h((UInt16)(TMP+Y), TMP2);

            else

                TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sha_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP2 = (byte)(A & X & ((TMP >> 8)+1));

            if(page_changing(TMP, Y))

                TMP = set_h((UInt16)(TMP+Y), TMP2);

            else

                TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void sha_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP2 = (byte)(A & X & ((TMP >> 8)+1));

            if(page_changing(TMP, Y))

                TMP = set_h((UInt16)(TMP+Y), TMP2);

            else

                TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void shs_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            SP = set_l(SP, (byte)(A & X));

            TMP2 = (byte)(A & X & ((TMP >> 8)+1));

            if(page_changing(TMP, Y))

                TMP = set_h((UInt16)(TMP+Y), TMP2);

            else

                TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void shs_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            SP = set_l(SP, (byte)(A & X));

            TMP2 = (byte)(A & X & ((TMP >> 8)+1));

            if(page_changing(TMP, Y))

                TMP = set_h((UInt16)(TMP+Y), TMP2);

            else

                TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void shx_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP2 = (byte)(X & ((TMP >> 8)+1));

            if(page_changing(TMP, Y))

                TMP = set_h((UInt16)(TMP+Y), TMP2);

            else

                TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void shx_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP2 = (byte)(X & ((TMP >> 8)+1));

            if(page_changing(TMP, Y))

                TMP = set_h((UInt16)(TMP+Y), TMP2);

            else

                TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void shy_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP2 = (byte)(Y & ((TMP >> 8)+1));

            if(page_changing(TMP, X))

                TMP = set_h((UInt16)(TMP+X), TMP2);

            else

                TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void shy_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP2 = (byte)(Y & ((TMP >> 8)+1));

            if(page_changing(TMP, X))

                TMP = set_h((UInt16)(TMP+X), TMP2);

            else

                TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void slo_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void slo_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void slo_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void slo_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void slo_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void slo_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void slo_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void slo_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void slo_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void slo_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void slo_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void slo_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void slo_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void slo_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_asl(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            A |= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sre_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void sre_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sre_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void sre_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            TMP += X;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sre_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            prefetch();
            icount--;

        }

        void sre_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sre_idx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void sre_idx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP2);
            icount--;

            TMP2 += X;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = read((UInt16)(TMP2 & 0xff));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sre_idy_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            prefetch();
            icount--;

        }

        void sre_idy_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = read(TMP2);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP = set_h(TMP, read((UInt16)((TMP2+1) & 0xff)));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            TMP += Y;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 7; return; }
            goto case 7;
        case 7:
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 8; return; }
            goto case 8;
        case 8:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sre_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void sre_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void sre_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            prefetch();
            icount--;

        }

        void sre_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            TMP = (byte)(TMP+X);

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            TMP2 = read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            write(TMP, TMP2);
            icount--;

            TMP2 = do_lsr(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            write(TMP, TMP2);
            icount--;

            A ^= TMP2;

            set_nz(A);

            if(icount == 0) { inst_substate = 6; return; }
            goto case 6;
        case 6:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void anc_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            A &= read_pc();
            icount--;

            set_nz(A);

            if ((A & 0x80) != 0)

                P |= (byte)F.F_C;

            else

                P &= unchecked((byte)~F.F_C);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void anc_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            A &= read_pc();
            icount--;

            set_nz(A);

            if ((A & 0x80) != 0)

                P |= (byte)F.F_C;

            else

                P &= unchecked((byte)~F.F_C);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void ane_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP2 = read_pc();
            icount--;

            A &= (byte)(TMP2 & X);

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void ane_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP2 = read_pc();
            icount--;

            A &= (byte)(TMP2 & X);

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void asr_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            A &= read_pc();
            icount--;

            A = do_lsr(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void asr_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            A &= read_pc();
            icount--;

            A = do_lsr(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void arr_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            A &= read_pc();
            icount--;

            do_arr();

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void arr_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            A &= read_pc();
            icount--;

            do_arr();

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void las_aby_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            TMP2 = read((UInt16)(TMP+Y));
            icount--;

            A = (byte)(TMP2 | 0x51);

            X = 0xff;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void las_aby_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, Y)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+Y)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            TMP2 = read((UInt16)(TMP+Y));
            icount--;

            A = (byte)(TMP2 | 0x51);

            X = 0xff;

            set_nz(TMP2);

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void lxa_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            A = X = read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void lxa_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            A = X = read_pc();
            icount--;

            set_nz(A);

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void nop_aba_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void nop_aba_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void nop_abx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            read((UInt16)(TMP + X));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            prefetch();
            icount--;

        }

        void nop_abx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            TMP = set_h(TMP, read_pc());
            icount--;

            if(page_changing(TMP, X)) {

            if(icount == 0) { inst_substate = 3; return; }
        //case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            }

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 3:
                read(set_l(TMP, (byte)(TMP+X)));
            icount--;

            //}

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read((UInt16)(TMP + X));
            icount--;

            if(icount == 0) { inst_substate = 5; return; }
            goto case 5;
        case 5:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void nop_imm_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            prefetch();
            icount--;

        }

        void nop_imm_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void nop_zpg_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            prefetch();
            icount--;

        }

        void nop_zpg_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void nop_zpx_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read((byte)(TMP+X));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            prefetch();
            icount--;

        }

        void nop_zpx_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            TMP = read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(TMP);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read((byte)(TMP+X));
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            prefetch();
            icount--;
            break;
        }
            inst_substate = 0;
        }


        void kil_non_full()
        {

            if(icount == 0) { inst_substate = 1; return; }
            read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            read(0xffff);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            read(0xfffe);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            read(0xfffe);
            icount--;

            for(;;) {

            if(icount == 0) { inst_substate = 5; return; }
                read(0xffff);
            icount--;

            }

        }

        void kil_non_partial()
        {
        switch(inst_substate) {
        case 0:

            if(icount == 0) { inst_substate = 1; return; }
            goto case 1;
        case 1:
            read_pc();
            icount--;

            if(icount == 0) { inst_substate = 2; return; }
            goto case 2;
        case 2:
            read(0xffff);
            icount--;

            if(icount == 0) { inst_substate = 3; return; }
            goto case 3;
        case 3:
            read(0xfffe);
            icount--;

            if(icount == 0) { inst_substate = 4; return; }
            goto case 4;
        case 4:
            read(0xfffe);
            icount--;

            for(;;) {

            if(icount == 0) { inst_substate = 5; return; }
        //case 5:
                read(0xffff);
            icount--;

            }
        case 5:
                read(0xffff);
            icount--;

            //}
            break;
        }
            inst_substate = 0;
        }

        void do_exec_full()
        {
            switch(inst_state) {

            case 0x00: brk_imp_full(); break;
            case 0x01: ora_idx_full(); break;
            case 0x02: kil_non_full(); break;
            case 0x03: slo_idx_full(); break;
            case 0x04: nop_zpg_full(); break;
            case 0x05: ora_zpg_full(); break;
            case 0x06: asl_zpg_full(); break;
            case 0x07: slo_zpg_full(); break;
            case 0x08: php_imp_full(); break;
            case 0x09: ora_imm_full(); break;
            case 0x0a: asl_acc_full(); break;
            case 0x0b: anc_imm_full(); break;
            case 0x0c: nop_aba_full(); break;
            case 0x0d: ora_aba_full(); break;
            case 0x0e: asl_aba_full(); break;
            case 0x0f: slo_aba_full(); break;
            case 0x10: bpl_rel_full(); break;
            case 0x11: ora_idy_full(); break;
            case 0x12: kil_non_full(); break;
            case 0x13: slo_idy_full(); break;
            case 0x14: nop_zpx_full(); break;
            case 0x15: ora_zpx_full(); break;
            case 0x16: asl_zpx_full(); break;
            case 0x17: slo_zpx_full(); break;
            case 0x18: clc_imp_full(); break;
            case 0x19: ora_aby_full(); break;
            case 0x1a: nop_imp_full(); break;
            case 0x1b: slo_aby_full(); break;
            case 0x1c: nop_abx_full(); break;
            case 0x1d: ora_abx_full(); break;
            case 0x1e: asl_abx_full(); break;
            case 0x1f: slo_abx_full(); break;
            case 0x20: jsr_adr_full(); break;
            case 0x21: and_idx_full(); break;
            case 0x22: kil_non_full(); break;
            case 0x23: rla_idx_full(); break;
            case 0x24: bit_zpg_full(); break;
            case 0x25: and_zpg_full(); break;
            case 0x26: rol_zpg_full(); break;
            case 0x27: rla_zpg_full(); break;
            case 0x28: plp_imp_full(); break;
            case 0x29: and_imm_full(); break;
            case 0x2a: rol_acc_full(); break;
            case 0x2b: anc_imm_full(); break;
            case 0x2c: bit_aba_full(); break;
            case 0x2d: and_aba_full(); break;
            case 0x2e: rol_aba_full(); break;
            case 0x2f: rla_aba_full(); break;
            case 0x30: bmi_rel_full(); break;
            case 0x31: and_idy_full(); break;
            case 0x32: kil_non_full(); break;
            case 0x33: rla_idy_full(); break;
            case 0x34: nop_zpx_full(); break;
            case 0x35: and_zpx_full(); break;
            case 0x36: rol_zpx_full(); break;
            case 0x37: rla_zpx_full(); break;
            case 0x38: sec_imp_full(); break;
            case 0x39: and_aby_full(); break;
            case 0x3a: nop_imp_full(); break;
            case 0x3b: rla_aby_full(); break;
            case 0x3c: nop_abx_full(); break;
            case 0x3d: and_abx_full(); break;
            case 0x3e: rol_abx_full(); break;
            case 0x3f: rla_abx_full(); break;
            case 0x40: rti_imp_full(); break;
            case 0x41: eor_idx_full(); break;
            case 0x42: kil_non_full(); break;
            case 0x43: sre_idx_full(); break;
            case 0x44: nop_zpg_full(); break;
            case 0x45: eor_zpg_full(); break;
            case 0x46: lsr_zpg_full(); break;
            case 0x47: sre_zpg_full(); break;
            case 0x48: pha_imp_full(); break;
            case 0x49: eor_imm_full(); break;
            case 0x4a: lsr_acc_full(); break;
            case 0x4b: asr_imm_full(); break;
            case 0x4c: jmp_adr_full(); break;
            case 0x4d: eor_aba_full(); break;
            case 0x4e: lsr_aba_full(); break;
            case 0x4f: sre_aba_full(); break;
            case 0x50: bvc_rel_full(); break;
            case 0x51: eor_idy_full(); break;
            case 0x52: kil_non_full(); break;
            case 0x53: sre_idy_full(); break;
            case 0x54: nop_zpx_full(); break;
            case 0x55: eor_zpx_full(); break;
            case 0x56: lsr_zpx_full(); break;
            case 0x57: sre_zpx_full(); break;
            case 0x58: cli_imp_full(); break;
            case 0x59: eor_aby_full(); break;
            case 0x5a: nop_imp_full(); break;
            case 0x5b: sre_aby_full(); break;
            case 0x5c: nop_abx_full(); break;
            case 0x5d: eor_abx_full(); break;
            case 0x5e: lsr_abx_full(); break;
            case 0x5f: sre_abx_full(); break;
            case 0x60: rts_imp_full(); break;
            case 0x61: adc_idx_full(); break;
            case 0x62: kil_non_full(); break;
            case 0x63: rra_idx_full(); break;
            case 0x64: nop_zpg_full(); break;
            case 0x65: adc_zpg_full(); break;
            case 0x66: ror_zpg_full(); break;
            case 0x67: rra_zpg_full(); break;
            case 0x68: pla_imp_full(); break;
            case 0x69: adc_imm_full(); break;
            case 0x6a: ror_acc_full(); break;
            case 0x6b: arr_imm_full(); break;
            case 0x6c: jmp_ind_full(); break;
            case 0x6d: adc_aba_full(); break;
            case 0x6e: ror_aba_full(); break;
            case 0x6f: rra_aba_full(); break;
            case 0x70: bvs_rel_full(); break;
            case 0x71: adc_idy_full(); break;
            case 0x72: kil_non_full(); break;
            case 0x73: rra_idy_full(); break;
            case 0x74: nop_zpx_full(); break;
            case 0x75: adc_zpx_full(); break;
            case 0x76: ror_zpx_full(); break;
            case 0x77: rra_zpx_full(); break;
            case 0x78: sei_imp_full(); break;
            case 0x79: adc_aby_full(); break;
            case 0x7a: nop_imp_full(); break;
            case 0x7b: rra_aby_full(); break;
            case 0x7c: nop_abx_full(); break;
            case 0x7d: adc_abx_full(); break;
            case 0x7e: ror_abx_full(); break;
            case 0x7f: rra_abx_full(); break;
            case 0x80: nop_imm_full(); break;
            case 0x81: sta_idx_full(); break;
            case 0x82: nop_imm_full(); break;
            case 0x83: sax_idx_full(); break;
            case 0x84: sty_zpg_full(); break;
            case 0x85: sta_zpg_full(); break;
            case 0x86: stx_zpg_full(); break;
            case 0x87: sax_zpg_full(); break;
            case 0x88: dey_imp_full(); break;
            case 0x89: nop_imm_full(); break;
            case 0x8a: txa_imp_full(); break;
            case 0x8b: ane_imm_full(); break;
            case 0x8c: sty_aba_full(); break;
            case 0x8d: sta_aba_full(); break;
            case 0x8e: stx_aba_full(); break;
            case 0x8f: sax_aba_full(); break;
            case 0x90: bcc_rel_full(); break;
            case 0x91: sta_idy_full(); break;
            case 0x92: kil_non_full(); break;
            case 0x93: sha_idy_full(); break;
            case 0x94: sty_zpx_full(); break;
            case 0x95: sta_zpx_full(); break;
            case 0x96: stx_zpy_full(); break;
            case 0x97: sax_zpy_full(); break;
            case 0x98: tya_imp_full(); break;
            case 0x99: sta_aby_full(); break;
            case 0x9a: txs_imp_full(); break;
            case 0x9b: shs_aby_full(); break;
            case 0x9c: shy_abx_full(); break;
            case 0x9d: sta_abx_full(); break;
            case 0x9e: shx_aby_full(); break;
            case 0x9f: sha_aby_full(); break;
            case 0xa0: ldy_imm_full(); break;
            case 0xa1: lda_idx_full(); break;
            case 0xa2: ldx_imm_full(); break;
            case 0xa3: lax_idx_full(); break;
            case 0xa4: ldy_zpg_full(); break;
            case 0xa5: lda_zpg_full(); break;
            case 0xa6: ldx_zpg_full(); break;
            case 0xa7: lax_zpg_full(); break;
            case 0xa8: tay_imp_full(); break;
            case 0xa9: lda_imm_full(); break;
            case 0xaa: tax_imp_full(); break;
            case 0xab: lxa_imm_full(); break;
            case 0xac: ldy_aba_full(); break;
            case 0xad: lda_aba_full(); break;
            case 0xae: ldx_aba_full(); break;
            case 0xaf: lax_aba_full(); break;
            case 0xb0: bcs_rel_full(); break;
            case 0xb1: lda_idy_full(); break;
            case 0xb2: kil_non_full(); break;
            case 0xb3: lax_idy_full(); break;
            case 0xb4: ldy_zpx_full(); break;
            case 0xb5: lda_zpx_full(); break;
            case 0xb6: ldx_zpy_full(); break;
            case 0xb7: lax_zpy_full(); break;
            case 0xb8: clv_imp_full(); break;
            case 0xb9: lda_aby_full(); break;
            case 0xba: tsx_imp_full(); break;
            case 0xbb: las_aby_full(); break;
            case 0xbc: ldy_abx_full(); break;
            case 0xbd: lda_abx_full(); break;
            case 0xbe: ldx_aby_full(); break;
            case 0xbf: lax_aby_full(); break;
            case 0xc0: cpy_imm_full(); break;
            case 0xc1: cmp_idx_full(); break;
            case 0xc2: nop_imm_full(); break;
            case 0xc3: dcp_idx_full(); break;
            case 0xc4: cpy_zpg_full(); break;
            case 0xc5: cmp_zpg_full(); break;
            case 0xc6: dec_zpg_full(); break;
            case 0xc7: dcp_zpg_full(); break;
            case 0xc8: iny_imp_full(); break;
            case 0xc9: cmp_imm_full(); break;
            case 0xca: dex_imp_full(); break;
            case 0xcb: sbx_imm_full(); break;
            case 0xcc: cpy_aba_full(); break;
            case 0xcd: cmp_aba_full(); break;
            case 0xce: dec_aba_full(); break;
            case 0xcf: dcp_aba_full(); break;
            case 0xd0: bne_rel_full(); break;
            case 0xd1: cmp_idy_full(); break;
            case 0xd2: kil_non_full(); break;
            case 0xd3: dcp_idy_full(); break;
            case 0xd4: nop_zpx_full(); break;
            case 0xd5: cmp_zpx_full(); break;
            case 0xd6: dec_zpx_full(); break;
            case 0xd7: dcp_zpx_full(); break;
            case 0xd8: cld_imp_full(); break;
            case 0xd9: cmp_aby_full(); break;
            case 0xda: nop_imp_full(); break;
            case 0xdb: dcp_aby_full(); break;
            case 0xdc: nop_abx_full(); break;
            case 0xdd: cmp_abx_full(); break;
            case 0xde: dec_abx_full(); break;
            case 0xdf: dcp_abx_full(); break;
            case 0xe0: cpx_imm_full(); break;
            case 0xe1: sbc_idx_full(); break;
            case 0xe2: nop_imm_full(); break;
            case 0xe3: isb_idx_full(); break;
            case 0xe4: cpx_zpg_full(); break;
            case 0xe5: sbc_zpg_full(); break;
            case 0xe6: inc_zpg_full(); break;
            case 0xe7: isb_zpg_full(); break;
            case 0xe8: inx_imp_full(); break;
            case 0xe9: sbc_imm_full(); break;
            case 0xea: nop_imp_full(); break;
            case 0xeb: sbc_imm_full(); break;
            case 0xec: cpx_aba_full(); break;
            case 0xed: sbc_aba_full(); break;
            case 0xee: inc_aba_full(); break;
            case 0xef: isb_aba_full(); break;
            case 0xf0: beq_rel_full(); break;
            case 0xf1: sbc_idy_full(); break;
            case 0xf2: kil_non_full(); break;
            case 0xf3: isb_idy_full(); break;
            case 0xf4: nop_zpx_full(); break;
            case 0xf5: sbc_zpx_full(); break;
            case 0xf6: inc_zpx_full(); break;
            case 0xf7: isb_zpx_full(); break;
            case 0xf8: sed_imp_full(); break;
            case 0xf9: sbc_aby_full(); break;
            case 0xfa: nop_imp_full(); break;
            case 0xfb: isb_aby_full(); break;
            case 0xfc: nop_abx_full(); break;
            case 0xfd: sbc_abx_full(); break;
            case 0xfe: inc_abx_full(); break;
            case 0xff: isb_abx_full(); break;
            case (int)M6502_STATE.STATE_RESET: reset_full(); break;
            }
        }


        void do_exec_partial()
        {
            switch(inst_state) {

            case 0x00: brk_imp_partial(); break;
            case 0x01: ora_idx_partial(); break;
            case 0x02: kil_non_partial(); break;
            case 0x03: slo_idx_partial(); break;
            case 0x04: nop_zpg_partial(); break;
            case 0x05: ora_zpg_partial(); break;
            case 0x06: asl_zpg_partial(); break;
            case 0x07: slo_zpg_partial(); break;
            case 0x08: php_imp_partial(); break;
            case 0x09: ora_imm_partial(); break;
            case 0x0a: asl_acc_partial(); break;
            case 0x0b: anc_imm_partial(); break;
            case 0x0c: nop_aba_partial(); break;
            case 0x0d: ora_aba_partial(); break;
            case 0x0e: asl_aba_partial(); break;
            case 0x0f: slo_aba_partial(); break;
            case 0x10: bpl_rel_partial(); break;
            case 0x11: ora_idy_partial(); break;
            case 0x12: kil_non_partial(); break;
            case 0x13: slo_idy_partial(); break;
            case 0x14: nop_zpx_partial(); break;
            case 0x15: ora_zpx_partial(); break;
            case 0x16: asl_zpx_partial(); break;
            case 0x17: slo_zpx_partial(); break;
            case 0x18: clc_imp_partial(); break;
            case 0x19: ora_aby_partial(); break;
            case 0x1a: nop_imp_partial(); break;
            case 0x1b: slo_aby_partial(); break;
            case 0x1c: nop_abx_partial(); break;
            case 0x1d: ora_abx_partial(); break;
            case 0x1e: asl_abx_partial(); break;
            case 0x1f: slo_abx_partial(); break;
            case 0x20: jsr_adr_partial(); break;
            case 0x21: and_idx_partial(); break;
            case 0x22: kil_non_partial(); break;
            case 0x23: rla_idx_partial(); break;
            case 0x24: bit_zpg_partial(); break;
            case 0x25: and_zpg_partial(); break;
            case 0x26: rol_zpg_partial(); break;
            case 0x27: rla_zpg_partial(); break;
            case 0x28: plp_imp_partial(); break;
            case 0x29: and_imm_partial(); break;
            case 0x2a: rol_acc_partial(); break;
            case 0x2b: anc_imm_partial(); break;
            case 0x2c: bit_aba_partial(); break;
            case 0x2d: and_aba_partial(); break;
            case 0x2e: rol_aba_partial(); break;
            case 0x2f: rla_aba_partial(); break;
            case 0x30: bmi_rel_partial(); break;
            case 0x31: and_idy_partial(); break;
            case 0x32: kil_non_partial(); break;
            case 0x33: rla_idy_partial(); break;
            case 0x34: nop_zpx_partial(); break;
            case 0x35: and_zpx_partial(); break;
            case 0x36: rol_zpx_partial(); break;
            case 0x37: rla_zpx_partial(); break;
            case 0x38: sec_imp_partial(); break;
            case 0x39: and_aby_partial(); break;
            case 0x3a: nop_imp_partial(); break;
            case 0x3b: rla_aby_partial(); break;
            case 0x3c: nop_abx_partial(); break;
            case 0x3d: and_abx_partial(); break;
            case 0x3e: rol_abx_partial(); break;
            case 0x3f: rla_abx_partial(); break;
            case 0x40: rti_imp_partial(); break;
            case 0x41: eor_idx_partial(); break;
            case 0x42: kil_non_partial(); break;
            case 0x43: sre_idx_partial(); break;
            case 0x44: nop_zpg_partial(); break;
            case 0x45: eor_zpg_partial(); break;
            case 0x46: lsr_zpg_partial(); break;
            case 0x47: sre_zpg_partial(); break;
            case 0x48: pha_imp_partial(); break;
            case 0x49: eor_imm_partial(); break;
            case 0x4a: lsr_acc_partial(); break;
            case 0x4b: asr_imm_partial(); break;
            case 0x4c: jmp_adr_partial(); break;
            case 0x4d: eor_aba_partial(); break;
            case 0x4e: lsr_aba_partial(); break;
            case 0x4f: sre_aba_partial(); break;
            case 0x50: bvc_rel_partial(); break;
            case 0x51: eor_idy_partial(); break;
            case 0x52: kil_non_partial(); break;
            case 0x53: sre_idy_partial(); break;
            case 0x54: nop_zpx_partial(); break;
            case 0x55: eor_zpx_partial(); break;
            case 0x56: lsr_zpx_partial(); break;
            case 0x57: sre_zpx_partial(); break;
            case 0x58: cli_imp_partial(); break;
            case 0x59: eor_aby_partial(); break;
            case 0x5a: nop_imp_partial(); break;
            case 0x5b: sre_aby_partial(); break;
            case 0x5c: nop_abx_partial(); break;
            case 0x5d: eor_abx_partial(); break;
            case 0x5e: lsr_abx_partial(); break;
            case 0x5f: sre_abx_partial(); break;
            case 0x60: rts_imp_partial(); break;
            case 0x61: adc_idx_partial(); break;
            case 0x62: kil_non_partial(); break;
            case 0x63: rra_idx_partial(); break;
            case 0x64: nop_zpg_partial(); break;
            case 0x65: adc_zpg_partial(); break;
            case 0x66: ror_zpg_partial(); break;
            case 0x67: rra_zpg_partial(); break;
            case 0x68: pla_imp_partial(); break;
            case 0x69: adc_imm_partial(); break;
            case 0x6a: ror_acc_partial(); break;
            case 0x6b: arr_imm_partial(); break;
            case 0x6c: jmp_ind_partial(); break;
            case 0x6d: adc_aba_partial(); break;
            case 0x6e: ror_aba_partial(); break;
            case 0x6f: rra_aba_partial(); break;
            case 0x70: bvs_rel_partial(); break;
            case 0x71: adc_idy_partial(); break;
            case 0x72: kil_non_partial(); break;
            case 0x73: rra_idy_partial(); break;
            case 0x74: nop_zpx_partial(); break;
            case 0x75: adc_zpx_partial(); break;
            case 0x76: ror_zpx_partial(); break;
            case 0x77: rra_zpx_partial(); break;
            case 0x78: sei_imp_partial(); break;
            case 0x79: adc_aby_partial(); break;
            case 0x7a: nop_imp_partial(); break;
            case 0x7b: rra_aby_partial(); break;
            case 0x7c: nop_abx_partial(); break;
            case 0x7d: adc_abx_partial(); break;
            case 0x7e: ror_abx_partial(); break;
            case 0x7f: rra_abx_partial(); break;
            case 0x80: nop_imm_partial(); break;
            case 0x81: sta_idx_partial(); break;
            case 0x82: nop_imm_partial(); break;
            case 0x83: sax_idx_partial(); break;
            case 0x84: sty_zpg_partial(); break;
            case 0x85: sta_zpg_partial(); break;
            case 0x86: stx_zpg_partial(); break;
            case 0x87: sax_zpg_partial(); break;
            case 0x88: dey_imp_partial(); break;
            case 0x89: nop_imm_partial(); break;
            case 0x8a: txa_imp_partial(); break;
            case 0x8b: ane_imm_partial(); break;
            case 0x8c: sty_aba_partial(); break;
            case 0x8d: sta_aba_partial(); break;
            case 0x8e: stx_aba_partial(); break;
            case 0x8f: sax_aba_partial(); break;
            case 0x90: bcc_rel_partial(); break;
            case 0x91: sta_idy_partial(); break;
            case 0x92: kil_non_partial(); break;
            case 0x93: sha_idy_partial(); break;
            case 0x94: sty_zpx_partial(); break;
            case 0x95: sta_zpx_partial(); break;
            case 0x96: stx_zpy_partial(); break;
            case 0x97: sax_zpy_partial(); break;
            case 0x98: tya_imp_partial(); break;
            case 0x99: sta_aby_partial(); break;
            case 0x9a: txs_imp_partial(); break;
            case 0x9b: shs_aby_partial(); break;
            case 0x9c: shy_abx_partial(); break;
            case 0x9d: sta_abx_partial(); break;
            case 0x9e: shx_aby_partial(); break;
            case 0x9f: sha_aby_partial(); break;
            case 0xa0: ldy_imm_partial(); break;
            case 0xa1: lda_idx_partial(); break;
            case 0xa2: ldx_imm_partial(); break;
            case 0xa3: lax_idx_partial(); break;
            case 0xa4: ldy_zpg_partial(); break;
            case 0xa5: lda_zpg_partial(); break;
            case 0xa6: ldx_zpg_partial(); break;
            case 0xa7: lax_zpg_partial(); break;
            case 0xa8: tay_imp_partial(); break;
            case 0xa9: lda_imm_partial(); break;
            case 0xaa: tax_imp_partial(); break;
            case 0xab: lxa_imm_partial(); break;
            case 0xac: ldy_aba_partial(); break;
            case 0xad: lda_aba_partial(); break;
            case 0xae: ldx_aba_partial(); break;
            case 0xaf: lax_aba_partial(); break;
            case 0xb0: bcs_rel_partial(); break;
            case 0xb1: lda_idy_partial(); break;
            case 0xb2: kil_non_partial(); break;
            case 0xb3: lax_idy_partial(); break;
            case 0xb4: ldy_zpx_partial(); break;
            case 0xb5: lda_zpx_partial(); break;
            case 0xb6: ldx_zpy_partial(); break;
            case 0xb7: lax_zpy_partial(); break;
            case 0xb8: clv_imp_partial(); break;
            case 0xb9: lda_aby_partial(); break;
            case 0xba: tsx_imp_partial(); break;
            case 0xbb: las_aby_partial(); break;
            case 0xbc: ldy_abx_partial(); break;
            case 0xbd: lda_abx_partial(); break;
            case 0xbe: ldx_aby_partial(); break;
            case 0xbf: lax_aby_partial(); break;
            case 0xc0: cpy_imm_partial(); break;
            case 0xc1: cmp_idx_partial(); break;
            case 0xc2: nop_imm_partial(); break;
            case 0xc3: dcp_idx_partial(); break;
            case 0xc4: cpy_zpg_partial(); break;
            case 0xc5: cmp_zpg_partial(); break;
            case 0xc6: dec_zpg_partial(); break;
            case 0xc7: dcp_zpg_partial(); break;
            case 0xc8: iny_imp_partial(); break;
            case 0xc9: cmp_imm_partial(); break;
            case 0xca: dex_imp_partial(); break;
            case 0xcb: sbx_imm_partial(); break;
            case 0xcc: cpy_aba_partial(); break;
            case 0xcd: cmp_aba_partial(); break;
            case 0xce: dec_aba_partial(); break;
            case 0xcf: dcp_aba_partial(); break;
            case 0xd0: bne_rel_partial(); break;
            case 0xd1: cmp_idy_partial(); break;
            case 0xd2: kil_non_partial(); break;
            case 0xd3: dcp_idy_partial(); break;
            case 0xd4: nop_zpx_partial(); break;
            case 0xd5: cmp_zpx_partial(); break;
            case 0xd6: dec_zpx_partial(); break;
            case 0xd7: dcp_zpx_partial(); break;
            case 0xd8: cld_imp_partial(); break;
            case 0xd9: cmp_aby_partial(); break;
            case 0xda: nop_imp_partial(); break;
            case 0xdb: dcp_aby_partial(); break;
            case 0xdc: nop_abx_partial(); break;
            case 0xdd: cmp_abx_partial(); break;
            case 0xde: dec_abx_partial(); break;
            case 0xdf: dcp_abx_partial(); break;
            case 0xe0: cpx_imm_partial(); break;
            case 0xe1: sbc_idx_partial(); break;
            case 0xe2: nop_imm_partial(); break;
            case 0xe3: isb_idx_partial(); break;
            case 0xe4: cpx_zpg_partial(); break;
            case 0xe5: sbc_zpg_partial(); break;
            case 0xe6: inc_zpg_partial(); break;
            case 0xe7: isb_zpg_partial(); break;
            case 0xe8: inx_imp_partial(); break;
            case 0xe9: sbc_imm_partial(); break;
            case 0xea: nop_imp_partial(); break;
            case 0xeb: sbc_imm_partial(); break;
            case 0xec: cpx_aba_partial(); break;
            case 0xed: sbc_aba_partial(); break;
            case 0xee: inc_aba_partial(); break;
            case 0xef: isb_aba_partial(); break;
            case 0xf0: beq_rel_partial(); break;
            case 0xf1: sbc_idy_partial(); break;
            case 0xf2: kil_non_partial(); break;
            case 0xf3: isb_idy_partial(); break;
            case 0xf4: nop_zpx_partial(); break;
            case 0xf5: sbc_zpx_partial(); break;
            case 0xf6: inc_zpx_partial(); break;
            case 0xf7: isb_zpx_partial(); break;
            case 0xf8: sed_imp_partial(); break;
            case 0xf9: sbc_aby_partial(); break;
            case 0xfa: nop_imp_partial(); break;
            case 0xfb: isb_aby_partial(); break;
            case 0xfc: nop_abx_partial(); break;
            case 0xfd: sbc_abx_partial(); break;
            case 0xfe: inc_abx_partial(); break;
            case 0xff: isb_abx_partial(); break;
            case (int)M6502_STATE.STATE_RESET: reset_partial(); break;
            }
        }
    }
}
