// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int8_t = System.SByte;
using int16_t = System.Int16;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.diexec_global;
using static mame.emucore_global;
using static mame.m6809_global;


namespace mame
{
    public partial class m6809_base_device : cpu_device
    {
        static bool UNEXPECTED(bool value) { return value; }


        void execute_one_switch()
        {
            switch (pop_state())
            {
                case 0:     goto MAIN;
                case 1:     goto state_1;
                case 2:     goto NEG8;
                case 3:     goto COM8;
                case 4:     goto LSR8;
                case 5:     goto ROR8;
                case 6:     goto ASR8;
                case 7:     goto ASL8;
                case 8:     goto ROL8;
                case 9:     goto DEC8;
                case 10:    goto INC8;
                case 11:    goto TST8;
                case 12:    goto JMP;
                case 13:    goto CLR8;
                case 14:    goto LEA_xy;
                case 15:    goto LEA_us;
                case 16:    goto SUB8;
                case 17:    goto CMP8;
                case 18:    goto SBC8;
                case 19:    goto SUB16;
                case 20:    goto AND8;
                case 21:    goto BIT8;
                case 22:    goto LD8;
                case 23:    goto ST8;
                case 24:    goto EOR8;
                case 25:    goto ADC8;
                case 26:    goto OR8;
                case 27:    goto ADD8;
                case 28:    goto CMP16;
                case 29:    goto JSR;
                case 30:    goto LD16;
                case 31:    goto ST16;
                case 32:    goto ADD16;
                case 33:    goto state_33;
                case 34:    goto state_34;
                case 35:    goto state_35;
                case 36:    goto state_36;
                case 37:    goto state_37;
                case 38:    goto state_38;
                case 39:    goto state_39;
                case 40:    goto state_40;
                case 41:    goto state_41;
                case 42:    goto state_42;
                case 43:    goto state_43;
                case 44:    goto state_44;
                case 45:    goto state_45;
                case 46:    goto state_46;
                case 47:    goto state_47;
                case 48:    goto state_48;
                case 49:    goto state_49;
                case 50:    goto state_50;
                case 51:    goto state_51;
                case 52:    goto state_52;
                case 53:    goto state_53;
                case 54:    goto state_54;
                case 55:    goto state_55;
                case 56:    goto state_56;
                case 57:    goto state_57;
                case 58:    goto state_58;
                case 59:    goto state_59;
                case 60:    goto state_60;
                case 61:    goto state_61;
                case 62:    goto state_62;
                case 63:    goto state_63;
                case 64:    goto state_64;
                case 65:    goto state_65;
                case 66:    goto state_66;
                case 67:    goto state_67;
                case 68:    goto state_68;
                case 69:    goto state_69;
                case 70:    goto state_70;
                case 71:    goto state_71;
                case 72:    goto state_72;
                case 73:    goto state_73;
                case 74:    goto state_74;
                case 75:    goto state_75;
                case 76:    goto state_76;
                case 77:    goto state_77;
                case 78:    goto state_78;
                case 79:    goto state_79;
                case 80:    goto state_80;
                case 81:    goto state_81;
                case 82:    goto state_82;
                case 83:    goto state_83;
                case 84:    goto state_84;
                case 85:    goto state_85;
                case 86:    goto state_86;
                case 87:    goto state_87;
                case 88:    goto state_88;
                case 89:    goto state_89;
                case 90:    goto state_90;
                case 91:    goto state_91;
                case 92:    goto state_92;
                case 93:    goto state_93;
                case 94:    goto state_94;
                case 95:    goto state_95;
                case 96:    goto state_96;
                case 97:    goto state_97;
                case 98:    goto state_98;
                case 99:    goto state_99;
                case 100:   goto state_100;
                case 101:   goto state_101;
                case 102:   goto state_102;
                case 103:   goto state_103;
                case 104:   goto state_104;
                case 105:   goto state_105;
                case 106:   goto state_106;
                case 107:   goto state_107;
                case 108:   goto state_108;
                case 109:   goto state_109;
                case 110:   goto state_110;
                case 111:   goto state_111;
                case 112:   goto state_112;
                case 113:   goto state_113;
                case 114:   throw new emu_unimplemented();  //goto state_114;
                case 115:   goto state_115;
                case 116:   goto state_116;
                case 117:   goto state_117;
                case 118:   goto state_118;
                case 119:   goto state_119;
                case 120:   goto state_120;
                case 121:   goto state_121;
                case 122:   goto state_122;
                case 123:   goto state_123;
                case 124:   goto state_124;
                case 125:   goto state_125;
                case 126:   goto state_126;
                case 127:   goto state_127;
                case 128:   goto state_128;
                case 129:   goto state_129;
                case 130:   goto state_130;
                case 131:   goto state_131;
                case 132:   goto state_132;
                case 133:   goto state_133;
                case 134:   goto state_134;
                case 135:   throw new emu_unimplemented();  //goto state_135;
                case 136:   goto state_136;
                case 137:   goto state_137;
                case 138:   goto state_138;
                case 139:   goto state_139;
                case 140:   goto state_140;
                case 141:   goto state_141;
                case 142:   goto state_142;
                case 143:   goto state_143;
                case 144:   throw new emu_unimplemented();  //goto state_144;
                case 145:   goto state_145;
                case 146:   goto state_146;
                case 147:   goto state_147;
                case 148:   goto state_148;
                case 149:   goto state_149;
                case 150:   goto state_150;
                case 151:   goto state_151;
                case 152:   goto state_152;
                case 153:   goto state_153;
                case 154:   goto state_154;
                case 155:   goto state_155;
                case 156:   goto state_156;
                case 157:   goto state_157;
                case 158:   goto state_158;
                case 159:   goto state_159;
                case 160:   goto state_160;
                case 161:   goto state_161;
                case 162:   goto state_162;
                case 163:   goto state_163;
                case 164:   goto state_164;
                case 165:   goto state_165;
                case 166:   throw new emu_unimplemented();  //goto state_166;
                case 167:   throw new emu_unimplemented();  //goto state_167;
                case 168:   throw new emu_unimplemented();  //goto state_168;
                case 169:   throw new emu_unimplemented();  //goto state_169;
                case 170:   throw new emu_unimplemented();  //goto state_170;
                case 171:   throw new emu_unimplemented();  //goto state_171;
                case 172:   throw new emu_unimplemented();  //goto state_172;
                case 173:   throw new emu_unimplemented();  //goto state_173;
                case 174:   throw new emu_unimplemented();  //goto state_174;
                case 175:   throw new emu_unimplemented();  //goto state_175;
                case 176:   throw new emu_unimplemented();  //goto state_176;
                case 177:   throw new emu_unimplemented();  //goto state_177;
                case 178:   throw new emu_unimplemented();  //goto state_178;
                case 179:   throw new emu_unimplemented();  //goto state_179;
                case 180:   throw new emu_unimplemented();  //goto state_180;
                case 181:   throw new emu_unimplemented();  //goto state_181;
                case 182:   throw new emu_unimplemented();  //goto state_182;
                case 183:   throw new emu_unimplemented();  //goto state_183;
                case 184:   throw new emu_unimplemented();  //goto state_184;
                case 185:   throw new emu_unimplemented();  //goto state_185;
                case 186:   throw new emu_unimplemented();  //goto state_186;
                case 187:   throw new emu_unimplemented();  //goto state_187;
                case 188:   throw new emu_unimplemented();  //goto state_188;
                case 189:   throw new emu_unimplemented();  //goto state_189;
                case 190:   goto state_190;
                case 191:   throw new emu_unimplemented();  //goto state_191;
                case 192:   throw new emu_unimplemented();  //goto state_192;
                case 193:   throw new emu_unimplemented();  //goto state_193;
                case 194:   throw new emu_unimplemented();  //goto state_194;
                case 195:   throw new emu_unimplemented();  //goto state_195;
                case 196:   throw new emu_unimplemented();  //goto state_196;
                case 197:   throw new emu_unimplemented();  //goto state_197;
                case 198:   throw new emu_unimplemented();  //goto state_198;
                case 199:   throw new emu_unimplemented();  //goto state_199;
                case 200:   throw new emu_unimplemented();  //goto state_200;
                case 201:   throw new emu_unimplemented();  //goto state_201;
                case 202:   throw new emu_unimplemented();  //goto state_202;
                case 203:   throw new emu_unimplemented();  //goto state_203;
                case 204:   throw new emu_unimplemented();  //goto state_204;
                case 205:   throw new emu_unimplemented();  //goto state_205;
                case 206:   throw new emu_unimplemented();  //goto state_206;
                case 207:   throw new emu_unimplemented();  //goto state_207;
                case 208:   throw new emu_unimplemented();  //goto state_208;
                case 209:   throw new emu_unimplemented();  //goto state_209;
                case 210:   throw new emu_unimplemented();  //goto state_210;
                case 211:   throw new emu_unimplemented();  //goto state_211;
                case 212:   throw new emu_unimplemented();  //goto state_212;
                case 213:   throw new emu_unimplemented();  //goto state_213;
                case 214:   throw new emu_unimplemented();  //goto state_214;
                case 215:   throw new emu_unimplemented();  //goto state_215;
                case 216:   throw new emu_unimplemented();  //goto state_216;
                case 217:   throw new emu_unimplemented();  //goto state_217;
                case 218:   throw new emu_unimplemented();  //goto state_218;
                case 219:   throw new emu_unimplemented();  //goto state_219;
                case 220:   throw new emu_unimplemented();  //goto state_220;
                case 221:   throw new emu_unimplemented();  //goto state_221;
                case 222:   throw new emu_unimplemented();  //goto state_222;
                case 223:   throw new emu_unimplemented();  //goto state_223;
                case 224:   throw new emu_unimplemented();  //goto state_224;
                default:
                    fatalerror("Unexpected state");
                    break;
            }

            // license:BSD-3-Clause
            // copyright-holders:Nathan Woods
MAIN:
            // check interrupt lines
            switch (get_pending_interrupt())
            {
                case VECTOR_NMI:    goto NMI;
                case VECTOR_FIRQ:   goto FIRQ;
                case VECTOR_IRQ:    goto IRQ;
            }

            // debugger hook
            m_ppc = m_pc;
            debugger_instruction_hook(m_pc.w);

            // opcode fetch
            m_lic_func.op_s32(ASSERT_LINE);
            m_opcode = read_opcode();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(1); return; }

state_1:
            m_lic_func.op_s32(CLEAR_LINE);

            // dispatch opcode
            switch (m_opcode)
            {
                case 0x00:  case 0x01:
                    push_state(2);  // NEG8
                    goto DIRECT;
                case 0x03:  case 0x02:
                    push_state(3);  // COM8
                    goto DIRECT;
                case 0x04:  case 0x05:
                    push_state(4);  // LSR8
                    goto DIRECT;
                case 0x06:
                    push_state(5);  // ROR8
                    goto DIRECT;
                case 0x07:
                    push_state(6);  // ASR8
                    goto DIRECT;
                case 0x08:
                    push_state(7);  // ASL8
                    goto DIRECT;
                case 0x09:
                    push_state(8);  // ROL8
                    goto DIRECT;
                case 0x0A:  case 0x0B:
                    push_state(9);  // DEC8
                    goto DIRECT;
                case 0x0C:
                    push_state(10); // INC8
                    goto DIRECT;
                case 0x0D:
                    push_state(11); // TST8
                    goto DIRECT;
                case 0x0E:
                    push_state(12); // JMP
                    goto DIRECT;
                case 0x0F:
                    push_state(13); // CLR8
                    goto DIRECT;

                case 0x10:
                    goto DISPATCH10;
                case 0x11:
                    goto DISPATCH11;
                case 0x12:
                    goto NOP;
                case 0x13:
                    goto SYNC;
                case 0x16:              set_cond(true); goto LBRANCH;
                case 0x17:
                    goto LBSR;
                case 0x19:
                    goto DAA;
                case 0x1A:              set_imm();      goto ORCC;
                case 0x1C:              set_imm();      goto ANDCC;
                case 0x1D:
                    goto SEX;
                case 0x1E:
                    goto EXG;
                case 0x1F:
                    goto TFR;

                case 0x20:              set_cond(true);         goto BRANCH;
                case 0x21:              set_cond(false);        goto BRANCH;
                case 0x22:              set_cond(cond_hi());    goto BRANCH;
                case 0x23:              set_cond(!cond_hi());   goto BRANCH;
                case 0x24:              set_cond(cond_cc());    goto BRANCH;
                case 0x25:              set_cond(!cond_cc());   goto BRANCH;
                case 0x26:              set_cond(cond_ne());    goto BRANCH;
                case 0x27:              set_cond(!cond_ne());   goto BRANCH;
                case 0x28:              set_cond(cond_vc());    goto BRANCH;
                case 0x29:              set_cond(!cond_vc());   goto BRANCH;
                case 0x2A:              set_cond(cond_pl());    goto BRANCH;
                case 0x2B:              set_cond(!cond_pl());   goto BRANCH;
                case 0x2C:              set_cond(cond_ge());    goto BRANCH;
                case 0x2D:              set_cond(!cond_ge());   goto BRANCH;
                case 0x2E:              set_cond(cond_gt());    goto BRANCH;
                case 0x2F:              set_cond(!cond_gt());   goto BRANCH;

                case 0x30:              set_regop16(m_x);       push_state(14); // LEA_xy
                goto INDEXED;
                case 0x31:              set_regop16(m_y);       push_state(14); // LEA_xy
                goto INDEXED;
                case 0x32:              set_regop16(m_s);       push_state(15); // LEA_us
                goto INDEXED;
                case 0x33:              set_regop16(m_u);       push_state(15); // LEA_us
                goto INDEXED;
                case 0x34:
                    goto PSHS;
                case 0x35:
                    goto PULS;
                case 0x36:
                    goto PSHU;
                case 0x37:
                    goto PULU;
                case 0x39:
                    goto RTS;
                case 0x3A:
                    goto ABX;
                case 0x3B:
                    goto RTI;
                case 0x3C:
                    goto CWAI;
                case 0x3D:
                    goto MUL;
                case 0x3F:
                    goto SWI;

                case 0x40:  case 0x41:                          set_a();    goto NEG8;
                case 0x43:  case 0x42:                          set_a();    goto COM8;
                case 0x44:  case 0x45:                          set_a();    goto LSR8;
                case 0x46:                                      set_a();    goto ROR8;
                case 0x47:                                      set_a();    goto ASR8;
                case 0x48:                                      set_a();    goto ASL8;
                case 0x49:                                      set_a();    goto ROL8;
                case 0x4A:  case 0x4B:                          set_a();    goto DEC8;
                case 0x4C:                                      set_a();    goto INC8;
                case 0x4D:                                      set_a();    goto TST8;
                case 0x4E:                                      set_a();    goto JMP;
                case 0x4F:                                      set_a();    goto CLR8;

                case 0x50:  case 0x51:                          set_b();    goto NEG8;
                case 0x53:  case 0x52:                          set_b();    goto COM8;
                case 0x54:  case 0x55:                          set_b();    goto LSR8;
                case 0x56:                                      set_b();    goto ROR8;
                case 0x57:                                      set_b();    goto ASR8;
                case 0x58:                                      set_b();    goto ASL8;
                case 0x59:                                      set_b();    goto ROL8;
                case 0x5A:  case 0x5B:                          set_b();    goto DEC8;
                case 0x5C:                                      set_b();    goto INC8;
                case 0x5D:                                      set_b();    goto TST8;
                case 0x5E:                                      set_b();    goto JMP;
                case 0x5F:                                      set_b();    goto CLR8;

                case 0x60:  case 0x61:
                    push_state(2);  // NEG8
                    goto INDEXED;
                case 0x63:  case 0x62:
                    push_state(3);  // COM8
                    goto INDEXED;
                case 0x64:  case 0x65:
                    push_state(4);  // LSR8
                    goto INDEXED;
                case 0x66:
                    push_state(5);  // ROR8
                    goto INDEXED;
                case 0x67:
                    push_state(6);  // ASR8
                    goto INDEXED;
                case 0x68:
                    push_state(7);  // ASL8
                    goto INDEXED;
                case 0x69:
                    push_state(8);  // ROL8
                    goto INDEXED;
                case 0x6A:  case 0x6B:
                    push_state(9);  // DEC8
                    goto INDEXED;
                case 0x6C:
                    push_state(10); // INC8
                    goto INDEXED;
                case 0x6D:
                    push_state(11); // TST8
                    goto INDEXED;
                case 0x6E:
                    push_state(12); // JMP
                    goto INDEXED;
                case 0x6F:
                    push_state(13); // CLR8
                    goto INDEXED;

                case 0x70:  case 0x71:
                    push_state(2);  // NEG8
                    goto EXTENDED;
                case 0x73:  case 0x72:
                    push_state(3);  // COM8
                    goto EXTENDED;
                case 0x74:  case 0x75:
                    push_state(4);  // LSR8
                    goto EXTENDED;
                case 0x76:
                    push_state(5);  // ROR8
                    goto EXTENDED;
                case 0x77:
                    push_state(6);  // ASR8
                    goto EXTENDED;
                case 0x78:
                    push_state(7);  // ASL8
                    goto EXTENDED;
                case 0x79:
                    push_state(8);  // ROL8
                    goto EXTENDED;
                case 0x7A:  case 0x7B:
                    push_state(9);  // DEC8
                    goto EXTENDED;
                case 0x7C:
                    push_state(10); // INC8
                    goto EXTENDED;
                case 0x7D:
                    push_state(11); // TST8
                    goto EXTENDED;
                case 0x7E:
                    push_state(12); // JMP
                    goto EXTENDED;
                case 0x7F:
                    push_state(13); // CLR8
                    goto EXTENDED;

                case 0x80:              set_regop8(m_q.r.a);    set_imm();  goto SUB8;
                case 0x81:              set_regop8(m_q.r.a);    set_imm();  goto CMP8;
                case 0x82:              set_regop8(m_q.r.a);    set_imm();  goto SBC8;
                case 0x83:              set_regop16(m_q.p.d);   set_imm();  goto SUB16;
                case 0x84:              set_regop8(m_q.r.a);    set_imm();  goto AND8;
                case 0x85:              set_regop8(m_q.r.a);    set_imm();  goto BIT8;
                case 0x86:              set_regop8(m_q.r.a);    set_imm();  goto LD8;
                case 0x87:              set_regop8(m_q.r.a);    set_imm();  goto ST8;
                case 0x88:              set_regop8(m_q.r.a);    set_imm();  goto EOR8;
                case 0x89:              set_regop8(m_q.r.a);    set_imm();  goto ADC8;
                case 0x8A:              set_regop8(m_q.r.a);    set_imm();  goto OR8;
                case 0x8B:              set_regop8(m_q.r.a);    set_imm();  goto ADD8;
                case 0x8C:              set_regop16(m_x);       set_imm();  goto CMP16;
                case 0x8D:
                    goto BSR;
                case 0x8E:              set_regop16(m_x);       set_imm();  goto LD16;
                case 0x8F:              set_regop16(m_x);       set_imm();  goto ST16;

                case 0x90:              set_regop8(m_q.r.a);    push_state(16); // SUB8
            goto DIRECT;
                case 0x91:              set_regop8(m_q.r.a);    push_state(17); // CMP8
            goto DIRECT;
                case 0x92:              set_regop8(m_q.r.a);    push_state(18); // SBC8
            goto DIRECT;
                case 0x93:              set_regop16(m_q.p.d);   push_state(19); // SUB16
            goto DIRECT;
                case 0x94:              set_regop8(m_q.r.a);    push_state(20); // AND8
            goto DIRECT;
                case 0x95:              set_regop8(m_q.r.a);    push_state(21); // BIT8
            goto DIRECT;
                case 0x96:              set_regop8(m_q.r.a);    push_state(22); // LD8
            goto DIRECT;
                case 0x97:              set_regop8(m_q.r.a);    push_state(23); // ST8
            goto DIRECT;
                case 0x98:              set_regop8(m_q.r.a);    push_state(24); // EOR8
            goto DIRECT;
                case 0x99:              set_regop8(m_q.r.a);    push_state(25); // ADC8
            goto DIRECT;
                case 0x9A:              set_regop8(m_q.r.a);    push_state(26); // OR8
            goto DIRECT;
                case 0x9B:              set_regop8(m_q.r.a);    push_state(27); // ADD8
            goto DIRECT;
                case 0x9C:              set_regop16(m_x);       push_state(28); // CMP16
                goto DIRECT;
                case 0x9D:
                    push_state(29); // JSR
                    goto DIRECT;
                case 0x9E:              set_regop16(m_x);       push_state(30); // LD16
                goto DIRECT;
                case 0x9F:              set_regop16(m_x);       push_state(31); // ST16
                goto DIRECT;

                case 0xA0:              set_regop8(m_q.r.a);    push_state(16); // SUB8
            goto INDEXED;
                case 0xA1:              set_regop8(m_q.r.a);    push_state(17); // CMP8
            goto INDEXED;
                case 0xA2:              set_regop8(m_q.r.a);    push_state(18); // SBC8
            goto INDEXED;
                case 0xA3:              set_regop16(m_q.p.d);   push_state(19); // SUB16
            goto INDEXED;
                case 0xA4:              set_regop8(m_q.r.a);    push_state(20); // AND8
            goto INDEXED;
                case 0xA5:              set_regop8(m_q.r.a);    push_state(21); // BIT8
            goto INDEXED;
                case 0xA6:              set_regop8(m_q.r.a);    push_state(22); // LD8
            goto INDEXED;
                case 0xA7:              set_regop8(m_q.r.a);    push_state(23); // ST8
            goto INDEXED;
                case 0xA8:              set_regop8(m_q.r.a);    push_state(24); // EOR8
            goto INDEXED;
                case 0xA9:              set_regop8(m_q.r.a);    push_state(25); // ADC8
            goto INDEXED;
                case 0xAA:              set_regop8(m_q.r.a);    push_state(26); // OR8
            goto INDEXED;
                case 0xAB:              set_regop8(m_q.r.a);    push_state(27); // ADD8
            goto INDEXED;
                case 0xAC:              set_regop16(m_x);       push_state(28); // CMP16
                goto INDEXED;
                case 0xAD:
                    push_state(29); // JSR
                    goto INDEXED;
                case 0xAE:              set_regop16(m_x);       push_state(30); // LD16
                goto INDEXED;
                case 0xAF:              set_regop16(m_x);       push_state(31); // ST16
                goto INDEXED;

                case 0xB0:              set_regop8(m_q.r.a);    push_state(16); // SUB8
            goto EXTENDED;
                case 0xB1:              set_regop8(m_q.r.a);    push_state(17); // CMP8
            goto EXTENDED;
                case 0xB2:              set_regop8(m_q.r.a);    push_state(18); // SBC8
            goto EXTENDED;
                case 0xB3:              set_regop16(m_q.p.d);   push_state(19); // SUB16
            goto EXTENDED;
                case 0xB4:              set_regop8(m_q.r.a);    push_state(20); // AND8
            goto EXTENDED;
                case 0xB5:              set_regop8(m_q.r.a);    push_state(21); // BIT8
            goto EXTENDED;
                case 0xB6:              set_regop8(m_q.r.a);    push_state(22); // LD8
            goto EXTENDED;
                case 0xB7:              set_regop8(m_q.r.a);    push_state(23); // ST8
            goto EXTENDED;
                case 0xB8:              set_regop8(m_q.r.a);    push_state(24); // EOR8
            goto EXTENDED;
                case 0xB9:              set_regop8(m_q.r.a);    push_state(25); // ADC8
            goto EXTENDED;
                case 0xBA:              set_regop8(m_q.r.a);    push_state(26); // OR8
            goto EXTENDED;
                case 0xBB:              set_regop8(m_q.r.a);    push_state(27); // ADD8
            goto EXTENDED;
                case 0xBC:              set_regop16(m_x);       push_state(28); // CMP16
                goto EXTENDED;
                case 0xBD:
                    push_state(29); // JSR
                    goto EXTENDED;
                case 0xBE:              set_regop16(m_x);       push_state(30); // LD16
                goto EXTENDED;
                case 0xBF:              set_regop16(m_x);       push_state(31); // ST16
                goto EXTENDED;

                case 0xC0:              set_regop8(m_q.r.b);    set_imm();  goto SUB8;
                case 0xC1:              set_regop8(m_q.r.b);    set_imm();  goto CMP8;
                case 0xC2:              set_regop8(m_q.r.b);    set_imm();  goto SBC8;
                case 0xC3:              set_regop16(m_q.p.d);   set_imm();  goto ADD16;
                case 0xC4:              set_regop8(m_q.r.b);    set_imm();  goto AND8;
                case 0xC5:              set_regop8(m_q.r.b);    set_imm();  goto BIT8;
                case 0xC6:              set_regop8(m_q.r.b);    set_imm();  goto LD8;
                case 0xC7:              set_regop8(m_q.r.b);    set_imm();  goto ST8;
                case 0xC8:              set_regop8(m_q.r.b);    set_imm();  goto EOR8;
                case 0xC9:              set_regop8(m_q.r.b);    set_imm();  goto ADC8;
                case 0xCA:              set_regop8(m_q.r.b);    set_imm();  goto OR8;
                case 0xCB:              set_regop8(m_q.r.b);    set_imm();  goto ADD8;
                case 0xCC:              set_regop16(m_q.p.d);   set_imm();  goto LD16;
                case 0xCD:              set_regop16(m_q.p.d);   set_imm();  goto ST16;
                case 0xCE:              set_regop16(m_u);       set_imm();  goto LD16;
                case 0xCF:              set_regop16(m_u);       set_imm();  goto ST16;

                case 0xD0:              set_regop8(m_q.r.b);    push_state(16); // SUB8
            goto DIRECT;
                case 0xD1:              set_regop8(m_q.r.b);    push_state(17); // CMP8
            goto DIRECT;
                case 0xD2:              set_regop8(m_q.r.b);    push_state(18); // SBC8
            goto DIRECT;
                case 0xD3:              set_regop16(m_q.p.d);   push_state(32); // ADD16
            goto DIRECT;
                case 0xD4:              set_regop8(m_q.r.b);    push_state(20); // AND8
            goto DIRECT;
                case 0xD5:              set_regop8(m_q.r.b);    push_state(21); // BIT8
            goto DIRECT;
                case 0xD6:              set_regop8(m_q.r.b);    push_state(22); // LD8
            goto DIRECT;
                case 0xD7:              set_regop8(m_q.r.b);    push_state(23); // ST8
            goto DIRECT;
                case 0xD8:              set_regop8(m_q.r.b);    push_state(24); // EOR8
            goto DIRECT;
                case 0xD9:              set_regop8(m_q.r.b);    push_state(25); // ADC8
            goto DIRECT;
                case 0xDA:              set_regop8(m_q.r.b);    push_state(26); // OR8
            goto DIRECT;
                case 0xDB:              set_regop8(m_q.r.b);    push_state(27); // ADD8
            goto DIRECT;
                case 0xDC:              set_regop16(m_q.p.d);   push_state(30); // LD16
            goto DIRECT;
                case 0xDD:              set_regop16(m_q.p.d);   push_state(31); // ST16
            goto DIRECT;
                case 0xDE:              set_regop16(m_u);       push_state(30); // LD16
                goto DIRECT;
                case 0xDF:              set_regop16(m_u);       push_state(31); // ST16
                goto DIRECT;

                case 0xE0:              set_regop8(m_q.r.b);    push_state(16); // SUB8
            goto INDEXED;
                case 0xE1:              set_regop8(m_q.r.b);    push_state(17); // CMP8
            goto INDEXED;
                case 0xE2:              set_regop8(m_q.r.b);    push_state(18); // SBC8
            goto INDEXED;
                case 0xE3:              set_regop16(m_q.p.d);   push_state(32); // ADD16
            goto INDEXED;
                case 0xE4:              set_regop8(m_q.r.b);    push_state(20); // AND8
            goto INDEXED;
                case 0xE5:              set_regop8(m_q.r.b);    push_state(21); // BIT8
            goto INDEXED;
                case 0xE6:              set_regop8(m_q.r.b);    push_state(22); // LD8
            goto INDEXED;
                case 0xE7:              set_regop8(m_q.r.b);    push_state(23); // ST8
            goto INDEXED;
                case 0xE8:              set_regop8(m_q.r.b);    push_state(24); // EOR8
            goto INDEXED;
                case 0xE9:              set_regop8(m_q.r.b);    push_state(25); // ADC8
            goto INDEXED;
                case 0xEA:              set_regop8(m_q.r.b);    push_state(26); // OR8
            goto INDEXED;
                case 0xEB:              set_regop8(m_q.r.b);    push_state(27); // ADD8
            goto INDEXED;
                case 0xEC:              set_regop16(m_q.p.d);   push_state(30); // LD16
            goto INDEXED;
                case 0xED:              set_regop16(m_q.p.d);   push_state(31); // ST16
            goto INDEXED;
                case 0xEE:              set_regop16(m_u);       push_state(30); // LD16
                goto INDEXED;
                case 0xEF:              set_regop16(m_u);       push_state(31); // ST16
                goto INDEXED;

                case 0xF0:              set_regop8(m_q.r.b);    push_state(16); // SUB8
            goto EXTENDED;
                case 0xF1:              set_regop8(m_q.r.b);    push_state(17); // CMP8
            goto EXTENDED;
                case 0xF2:              set_regop8(m_q.r.b);    push_state(18); // SBC8
            goto EXTENDED;
                case 0xF3:              set_regop16(m_q.p.d);   push_state(32); // ADD16
            goto EXTENDED;
                case 0xF4:              set_regop8(m_q.r.b);    push_state(20); // AND8
            goto EXTENDED;
                case 0xF5:              set_regop8(m_q.r.b);    push_state(21); // BIT8
            goto EXTENDED;
                case 0xF6:              set_regop8(m_q.r.b);    push_state(22); // LD8
            goto EXTENDED;
                case 0xF7:              set_regop8(m_q.r.b);    push_state(23); // ST8
            goto EXTENDED;
                case 0xF8:              set_regop8(m_q.r.b);    push_state(24); // EOR8
            goto EXTENDED;
                case 0xF9:              set_regop8(m_q.r.b);    push_state(25); // ADC8
            goto EXTENDED;
                case 0xFA:              set_regop8(m_q.r.b);    push_state(26); // OR8
            goto EXTENDED;
                case 0xFB:              set_regop8(m_q.r.b);    push_state(27); // ADD8
            goto EXTENDED;
                case 0xFC:              set_regop16(m_q.p.d);   push_state(30); // LD16
            goto EXTENDED;
                case 0xFD:              set_regop16(m_q.p.d);   push_state(31); // ST16
            goto EXTENDED;
                case 0xFE:              set_regop16(m_u);       push_state(30); // LD16
                goto EXTENDED;
                case 0xFF:              set_regop16(m_u);       push_state(31); // ST16
                goto EXTENDED;
                default:
                    goto ILLEGAL;
            
            }
            return;

DISPATCH10:
            m_opcode = read_opcode();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(33); return; }

state_33:
            switch(m_opcode)
            {
                case 0x20:              set_cond(true);                     goto LBRANCH;
                case 0x21:              set_cond(false);                    goto LBRANCH;
                case 0x22:              set_cond(cond_hi());                goto LBRANCH;
                case 0x23:              set_cond(!cond_hi());               goto LBRANCH;
                case 0x24:              set_cond(cond_cc());                goto LBRANCH;
                case 0x25:              set_cond(!cond_cc());               goto LBRANCH;
                case 0x26:              set_cond(cond_ne());                goto LBRANCH;
                case 0x27:              set_cond(!cond_ne());               goto LBRANCH;
                case 0x28:              set_cond(cond_vc());                goto LBRANCH;
                case 0x29:              set_cond(!cond_vc());               goto LBRANCH;
                case 0x2A:              set_cond(cond_pl());                goto LBRANCH;
                case 0x2B:              set_cond(!cond_pl());               goto LBRANCH;
                case 0x2C:              set_cond(cond_ge());                goto LBRANCH;
                case 0x2D:              set_cond(!cond_ge());               goto LBRANCH;
                case 0x2E:              set_cond(cond_gt());                goto LBRANCH;
                case 0x2F:              set_cond(!cond_gt());               goto LBRANCH;

                case 0x3F:
                    goto SWI2;

                case 0x83:              set_regop16(m_q.p.d);   set_imm();      goto CMP16;
                case 0x8C:              set_regop16(m_y);   set_imm();      goto CMP16;
                case 0x8E:              set_regop16(m_y);   set_imm();      goto LD16;
                case 0x8F:              set_regop16(m_y);   set_imm();      goto ST16;
                case 0x93:              set_regop16(m_q.p.d);   push_state(28); // CMP16
            goto DIRECT;
                case 0x9C:              set_regop16(m_y);   push_state(28); // CMP16
            goto DIRECT;
                case 0x9E:              set_regop16(m_y);   push_state(30); // LD16
            goto DIRECT;
                case 0x9F:              set_regop16(m_y);   push_state(31); // ST16
            goto DIRECT;
                case 0xA3:              set_regop16(m_q.p.d);   push_state(28); // CMP16
            goto INDEXED;
                case 0xAC:              set_regop16(m_y);   push_state(28); // CMP16
            goto INDEXED;
                case 0xAE:              set_regop16(m_y);   push_state(30); // LD16
            goto INDEXED;
                case 0xAF:              set_regop16(m_y);   push_state(31); // ST16
            goto INDEXED;
                case 0xB3:              set_regop16(m_q.p.d);   push_state(28); // CMP16
            goto EXTENDED;
                case 0xBC:              set_regop16(m_y);   push_state(28); // CMP16
            goto EXTENDED;
                case 0xBE:              set_regop16(m_y);   push_state(30); // LD16
            goto EXTENDED;
                case 0xBF:              set_regop16(m_y);   push_state(31); // ST16
            goto EXTENDED;

                case 0xCE:              set_regop16(m_s);   set_imm();      goto LD16;
                case 0xCF:              set_regop16(m_s);   set_imm();      goto ST16;
                case 0xDE:              set_regop16(m_s);   push_state(30); // LD16
            goto DIRECT;
                case 0xDF:              set_regop16(m_s);   push_state(31); // ST16
            goto DIRECT;
                case 0xEE:              set_regop16(m_s);   push_state(30); // LD16
            goto INDEXED;
                case 0xEF:              set_regop16(m_s);   push_state(31); // ST16
            goto INDEXED;
                case 0xFE:              set_regop16(m_s);   push_state(30); // LD16
            goto EXTENDED;
                case 0xFF:              set_regop16(m_s);   push_state(31); // ST16
            goto EXTENDED;

                default:
                    goto ILLEGAL;
            
            }
            return;

DISPATCH11:
            m_opcode = read_opcode();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(34); return; }

state_34:
            switch (m_opcode)
            {
                case 0x3F:
                    goto SWI3;
                case 0x83:              set_regop16(m_u);   set_imm();      goto CMP16;
                case 0x8C:              set_regop16(m_s);   set_imm();      goto CMP16;
                case 0x93:              set_regop16(m_u);   push_state(28); // CMP16
            goto DIRECT;
                case 0x9C:              set_regop16(m_s);   push_state(28); // CMP16
            goto DIRECT;
                case 0xA3:              set_regop16(m_u);   push_state(28); // CMP16
            goto INDEXED;
                case 0xAC:              set_regop16(m_s);   push_state(28); // CMP16
            goto INDEXED;
                case 0xB3:              set_regop16(m_u);   push_state(28); // CMP16
            goto EXTENDED;
                case 0xBC:              set_regop16(m_s);   push_state(28); // CMP16
            goto EXTENDED;
                default:
                    goto ILLEGAL;

            }
            return;

        // license:BSD-3-Clause
        // copyright-holders:Nathan Woods
NMI:
            m_nmi_asserted = false;
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(35); return; }

state_35:
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(36); return; }

state_36:
            dummy_vma(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(37); return; }

state_37:
            m_cc |= CC_E;
            set_regop16(m_s);
            m_temp.w = entire_state_registers();
            push_state(38);
            goto PUSH_REGISTERS;

state_38:
            m_cc |= CC_I | CC_F;
            set_ea(VECTOR_NMI);
            standard_irq_callback(INPUT_LINE_NMI);
            goto INTERRUPT_VECTOR;

FIRQ:
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(39); return; }

state_39:
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(40); return; }

state_40:
            dummy_vma(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(41); return; }

state_41:
            if (firq_saves_entire_state())
            {
                m_cc |= CC_E;
                m_temp.w = entire_state_registers();
            }
            else
            {
                m_cc &= unchecked((uint8_t)~CC_E);
                m_temp.w = partial_state_registers();
            }
            set_regop16(m_s);
            push_state(42);
            goto PUSH_REGISTERS;

state_42:
            m_cc |= CC_I | CC_F;
            set_ea(VECTOR_FIRQ);
            standard_irq_callback(M6809_FIRQ_LINE);
            goto INTERRUPT_VECTOR;

IRQ:
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(43); return; }

state_43:
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(44); return; }

state_44:
            dummy_vma(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(45); return; }

state_45:
            m_cc |= CC_E;
            set_regop16(m_s);
            m_temp.w = entire_state_registers();
            push_state(46);
            goto PUSH_REGISTERS;

state_46:
            m_cc |= CC_I;
            set_ea(VECTOR_IRQ);
            standard_irq_callback(M6809_IRQ_LINE);
            goto INTERRUPT_VECTOR;

INTERRUPT_VECTOR:
            dummy_vma(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(47); return; }

state_47:
            m_pc.b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(48); return; }

state_48:
            m_pc.b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(49); return; }

state_49:
            dummy_vma(1);
            return;

NEG8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(50); return; }

state_50:
            m_temp.b.l = set_flags_u8(CC_NZVC, (uint8_t)0, m_temp.b.l, (uint32_t)(-m_temp.b.l));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(51); return; }

//state_51:
                ;
            }
state_51:
            write_operand(m_temp.b.l);
            return;

COM8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(52); return; }

state_52:
            m_cc &= unchecked((uint8_t)~CC_V);
            m_cc |= CC_C;
            m_temp.b.l = set_flags_u8(CC_NZ, (uint8_t) ~m_temp.b.l);
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(53); return; }

//state_53:
                ;
            }
state_53:
            write_operand(m_temp.b.l);
            return;

LSR8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(54); return; }

state_54:
            m_cc &= unchecked((uint8_t)~CC_C);
            m_cc |= (m_temp.b.l & 1) != 0 ? CC_C : (uint8_t)0;
            m_temp.b.l = set_flags_u8(CC_NZ, (uint8_t)(m_temp.b.l >> 1));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(55); return; }

//state_55:
                ;
            }
state_55:
            write_operand(m_temp.b.l);
            return;

ROR8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(56); return; }

state_56:
            m_temp.b.l = set_flags_u8(CC_NZ, rotate_right(m_temp.b.l));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(57); return; }

//state_57:
                ;
            }
state_57:
            write_operand(m_temp.b.l);
            return;

ASR8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(58); return; }

state_58:
            m_cc &= unchecked((uint8_t)~CC_C);
            m_cc |= (m_temp.b.l & 1) != 0 ? CC_C : (uint8_t)0;
            m_temp.b.l = set_flags_u8(CC_NZ, (uint8_t)(((int8_t) m_temp.b.l) >> 1));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(59); return; }

//state_59:
                ;
            }
state_59:
            write_operand(m_temp.b.l);
            return;

ASL8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(60); return; }
state_60:
            m_temp.b.l = set_flags_u8(CC_NZVC, m_temp.b.l, m_temp.b.l, (uint32_t)m_temp.b.l << 1);
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(61); return; }
//state_61:
                ;
            }
state_61:
            write_operand(m_temp.b.l);
            return;

ROL8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(62); return; }
state_62:
            m_temp.b.l = set_flags_u8(CC_NZV, m_temp.b.l, m_temp.b.l, rotate_left(m_temp.b.l));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(63); return; }
//state_63:
                ;
            }
state_63:
            write_operand(m_temp.b.l);
            return;

DEC8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(64); return; }
state_64:
            m_temp.b.l = set_flags_u8(CC_NZV, m_temp.b.l, 1, (uint32_t)(m_temp.b.l - 1));
            if(!hd6309_native_mode() || !is_register_addressing_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(65); return; }
//state_65:
                ;
            }
state_65:
            write_operand(m_temp.b.l);
            return;

INC8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(66); return; }
state_66:
            m_temp.b.l = set_flags_u8(CC_NZV, m_temp.b.l, 1, (uint32_t)(m_temp.b.l + 1));
            if(!hd6309_native_mode() || !is_register_addressing_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(67); return; }
//state_67:
                ;
            }
state_67:
            write_operand(m_temp.b.l);
            return;

TST8:
            m_temp.b.l = read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(68); return; }
state_68:
            set_flags_u8(CC_NZV, m_temp.b.l);
            if(!hd6309_native_mode()) {
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(69); return; }
//state_69:
                ;
            }
state_69:
            if(!is_register_addressing_mode()) {
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(70); return; }
//state_70:
                ;
            }
state_70:
            return;

JMP:
            m_pc.w = m_ea.w;
            return;

CLR8:
            read_operand();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(71); return; }
state_71:
            m_cc &= unchecked((uint8_t)~CC_NZVC);
            m_cc |= CC_Z;
            if(!hd6309_native_mode() || !is_register_addressing_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(72); return; }
//state_72:
                ;
            }
state_72:
            write_operand(0);
            return;

//NEG16:
            m_temp.b.h = read_operand(0);
            m_temp.b.l = read_operand(1);
            m_temp.w = set_flags_u16(CC_NZVC, (uint16_t)0, m_temp.w, (uint32_t)(-m_temp.w));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(73); return; }
//state_73:
                ;
            }
state_73:
            write_operand(0, m_temp.b.h);
            write_operand(1, m_temp.b.l);
            return;

//LSR16:
            m_temp.b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(74); return; }
        state_74:
            m_temp.b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(75); return; }
state_75:
            m_cc &= unchecked((uint8_t)~CC_C);
            m_cc |= (m_temp.w & 1) != 0 ? CC_C : (uint8_t)0;
            m_temp.w = set_flags_u16(CC_NZ, (uint16_t)(m_temp.w >> 1));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(76); return; }
//state_76:
                ;
            }
state_76:
            write_operand(0, m_temp.b.h);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(77); return; }
state_77:
            write_operand(1, m_temp.b.l);
            return;

//ROR16:
            m_temp.b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(78); return; }
state_78:
            m_temp.b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(79); return; }
state_79:
            m_temp.w = set_flags_u16(CC_NZ, rotate_right(m_temp.w));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(80); return; }
//state_80:
                ;
            }
state_80:
            write_operand(0, m_temp.b.h);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(81); return; }
state_81:
            write_operand(1, m_temp.b.l);
            return;

//ASR16:
            m_temp.b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(82); return; }
state_82:
            m_temp.b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(83); return; }
state_83:
            m_cc &= unchecked((uint8_t)~CC_C);
            m_cc |= (m_temp.w & 1) != 0 ? CC_C : (uint8_t)0;
            m_temp.w = set_flags_u16(CC_NZ, (uint16_t)(((int16_t) m_temp.w) >> 1));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(84); return; }
//state_84:
                ;
            }
state_84:
            write_operand(0, m_temp.b.h);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(85); return; }
state_85:
            write_operand(1, m_temp.b.l);
            return;

//ASL16:
            m_temp.b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(86); return; }
state_86:
            m_temp.b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(87); return; }
state_87:
            m_temp.w = set_flags_u16(CC_NZVC, m_temp.w, m_temp.w, (uint32_t)m_temp.w << 1);
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(88); return; }
//state_88:
                ;
            }
state_88:
            write_operand(0, m_temp.b.h);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(89); return; }
state_89:
            write_operand(1, m_temp.b.l);
            return;

//ROL16:
            m_temp.b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(90); return; }
        state_90:
            m_temp.b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(91); return; }
        state_91:
            m_temp.w = set_flags_u16(CC_NZV, (uint16_t)rotate_left(m_temp.w));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(92); return; }
//state_92:
                ;
            }
state_92:
            write_operand(0, m_temp.b.h);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(93); return; }
state_93:
            write_operand(1, m_temp.b.l);
            return;

//DEC16:
            m_temp.b.h = read_operand(0);
            m_temp.b.l = read_operand(1);
            m_temp.w = set_flags_u16(CC_NZVC, m_temp.w, 1, (uint32_t)m_temp.w - 1);
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(94); return; }
//state_94:
                ;
            }
state_94:
            write_operand(0, m_temp.b.h);
            write_operand(1, m_temp.b.l);
            return;

//INC16:
            m_temp.b.h = read_operand(0);
            m_temp.b.l = read_operand(1);
            m_temp.w = set_flags_u16(CC_NZVC, m_temp.w, 1, (uint32_t)m_temp.w + 1);
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(95); return; }
//state_95:
                ;
            }
state_95:
            write_operand(0, m_temp.b.h);
            write_operand(1, m_temp.b.l);
            return;

//TST16:
            m_temp.b.h = read_operand(0);
            m_temp.b.l = read_operand(1);
            set_flags_u16(CC_NZV, m_temp.w);
            if(!hd6309_native_mode()) {
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(96); return; }
//state_96:
                ;
            }
state_96:
            if(!is_register_addressing_mode()) {
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(97); return; }
//state_97:
                ;
            }
state_97:
            return;

//CLR16:
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(98); return; }
//state_98:
                ;
            }
state_98:
            m_cc &= unchecked((uint8_t)~CC_NZVC);
            m_cc |= CC_Z;
            write_operand(0, 0x00);
            write_operand(1, 0x00);
            return;

SUB8:
            m_temp.b.l = read_operand();
            regop8() = set_flags_u8(CC_NZVC, regop8(), m_temp.b.l, (uint32_t)regop8() - m_temp.b.l);
            return;

CMP8:
            m_temp.b.l = read_operand();
            set_flags_u8(CC_NZVC, regop8(), m_temp.b.l, (uint32_t)regop8() - m_temp.b.l);
            return;

SBC8:
            m_temp.b.l = read_operand();
            regop8() = set_flags_u8(CC_NZVC, regop8(), m_temp.b.l, (uint32_t)regop8() - m_temp.b.l - ((m_cc & CC_C) != 0 ? 1U : 0U));
            return;

AND8:
            m_cc &= unchecked((uint8_t)~CC_V);
            regop8() = set_flags_u8(CC_NZ, (uint8_t)0, regop8(), (uint32_t)regop8() & read_operand());
            return;

BIT8:
            m_cc &= unchecked((uint8_t)~CC_V);
            set_flags_u8(CC_NZ, (uint8_t)0, regop8(), (uint32_t)regop8() & read_operand());
            return;

EOR8:
            m_cc &= unchecked((uint8_t)~CC_V);
            regop8() = set_flags_u8(CC_NZ, (uint8_t)0, regop8(), (uint32_t)regop8() ^ read_operand());
            return;

ADC8:
            m_temp.b.l = read_operand();
            regop8() = set_flags_u8(add8_sets_h() ? CC_HNZVC : CC_NZVC, regop8(), m_temp.b.l, (uint32_t)regop8() + m_temp.b.l + ((m_cc & CC_C) != 0 ? 1U : 0U));
            return;

OR8:
            m_cc &= unchecked((uint8_t)~CC_V);
            regop8() = set_flags_u8(CC_NZ, (uint8_t)0, regop8(), (uint32_t)regop8() | read_operand());
            return;

ADD8:
            m_temp.b.l = read_operand();
            regop8() = set_flags_u8(add8_sets_h() ? CC_HNZVC : CC_NZVC, regop8(), m_temp.b.l, (uint32_t)regop8() + m_temp.b.l);
            return;

ADD16:
            m_temp.b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(99); return; }
state_99:
            m_temp.b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(100); return; }
state_100:
            regop16().w = set_flags_u16(CC_NZVC, regop16().w, m_temp.w, (uint32_t)regop16().w + m_temp.w);
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(101); return; }
//state_101:
                ;
            }
state_101:
            return;

SUB16:
            m_temp.b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(102); return; }
state_102:
            m_temp.b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(103); return; }
state_103:
            regop16().w = set_flags_u16(CC_NZVC, regop16().w, m_temp.w, (uint32_t)regop16().w - m_temp.w);
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(104); return; }
//state_104:
                ;
            }
state_104:
            return;

CMP16:
            m_temp.b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(105); return; }
state_105:
            m_temp.b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(106); return; }
state_106:
            set_flags_u16(CC_NZVC, regop16().w, m_temp.w, (uint32_t)(regop16().w - m_temp.w));
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(107); return; }
//state_107:
                ;
            }
state_107:
            return;

LD8:
            regop8() = read_operand();
            set_flags_u8(CC_NZV, regop8());
            return;

LD16:
            regop16().b.h = read_operand(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(108); return; }
state_108:
            regop16().b.l = read_operand(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(109); return; }
state_109:
            set_flags_u16(CC_NZV, regop16().w);
            if (regop16_is_m_s())  //if (&regop16() == &m_s)
                m_lds_encountered = true;
            return;

ST8:
            write_ea(set_flags_u8(CC_NZV, regop8()));
            return;

ST16:
            write_operand(0, regop16().b.h);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(110); return; }
state_110:
            write_operand(1, regop16().b.l);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(111); return; }
state_111:
            set_flags_u16(CC_NZV, regop16().w);
            return;

NOP:
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(112); return; }
//state_112:
                ;
            }
state_112:
            return;

SYNC:
            // SYNC stops processing instructions until an interrupt request happens.
            // This doesn't require the corresponding interrupt to be enabled: if it
            // is disabled, execution continues with the next instruction.
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(113); return; }
state_113:

            while(!m_nmi_asserted && !m_firq_line && !m_irq_line)
            {
                // massaging the PC this way makes the debugger's behavior more
                // intuitive
                m_pc.w--;

                eat_remaining();
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(114); return; }
//state_114:

                // unmassage...
                m_pc.w++;
            }
            eat(1);
            return;

DAA:
            daa();
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(115); return; }
//state_115:
                ;
            }
state_115:
            return;

ORCC:
            m_cc |= read_operand();
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(116); return; }
//state_116:
                ;
            }
state_116:
            return;

ANDCC:
            m_cc &= read_operand();
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(117); return; }
//state_117:
                ;
            }
state_117:
            return;

SEX:
            m_q.r.d = set_flags_u16(CC_NZ, (uint16_t)(int8_t) m_q.r.b);
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(118); return; }
//state_118:
                ;
            }
state_118:
            return;

BRANCH:
            m_temp.b.l = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(119); return; }
state_119:
            if(!hd6309_native_mode()) {
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(120); return; }
//state_120:
                ;
            }
state_120:
            if (branch_taken())
            {
                m_pc.w = (uint16_t)(m_pc.w + (int8_t) m_temp.b.l);
            }
            return;

LBRANCH:
            m_temp.b.h = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(121); return; }
state_121:
            m_temp.b.l = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(122); return; }
state_122:
            if(!hd6309_native_mode()) {
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(123); return; }
//state_123:
                ;
            }
state_123:
            if (branch_taken())
            {
                m_pc.w += m_temp.w;
                if(!hd6309_native_mode()) {
                    dummy_vma(1);
                    if (UNEXPECTED(m_icount.i <= 0)) { push_state(124); return; }
//state_124:
                    ;
                }
            }
state_124:
            return;

BSR:
            m_temp.b.l = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(125); return; }
state_125:
            m_ea.w = (uint16_t)(m_pc.w + (int8_t) m_temp.b.l);
            dummy_vma(hd6309_native_mode() ? 2 : 3);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(126); return; }
state_126:
            goto GOTO_SUBROUTINE;

LBSR:
            m_temp.b.h = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(127); return; }
state_127:
            m_temp.b.l = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(128); return; }
state_128:
            m_ea.w = (uint16_t)(m_pc.w + (int16_t) m_temp.w);
            dummy_vma(hd6309_native_mode() ? 2 : 4);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(129); return; }
state_129:
            goto GOTO_SUBROUTINE;

JSR:
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(130); return; }
state_130:
            dummy_vma(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(131); return; }
state_131:
            goto GOTO_SUBROUTINE;

GOTO_SUBROUTINE:
            write_memory(--m_s.w, m_pc.b.l);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(132); return; }
state_132:
            write_memory(--m_s.w, m_pc.b.h);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(133); return; }
state_133:
            m_pc.w = m_ea.w;
            return;

RTS:
            m_temp.w = 0x80;    // RTS is equivalent to "PULS PC"
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(134); return; }
//state_134:
                ;
            }
state_134:
            set_regop16(m_s);
            goto PULL_REGISTERS;

ABX:
            m_x.w += m_q.r.b;
            if(!hd6309_native_mode()) {
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(135); return; }
//state_135:
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(136); return; }
//state_136:
                ;
            }
state_136:
            return;

MUL:
            mul();
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(137); return; }
state_137:
            dummy_vma(hd6309_native_mode() ? 8 : 9);
            return;

RTI:
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(138); return; }
state_138:
            set_regop16(m_s);
            m_cc = read_memory(regop16().w++);  if (UNEXPECTED(m_icount.i <= 0)) { push_state(139); return; }
state_139:
            // PULS CC
            m_temp.w = (uint16_t)(((m_cc & CC_E) != 0 ? entire_state_registers() : partial_state_registers()) & ~0x01);
            goto PULL_REGISTERS;

CWAI:
            m_cc &= read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(140); return; }
state_140:
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(141); return; }
state_141:
            dummy_vma(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(142); return; }
state_142:

            m_cc |= CC_E;
            set_regop16(m_s);
            m_temp.w = entire_state_registers();
            push_state(143);
            goto PUSH_REGISTERS;
state_143:

            while((m_ea.w = get_pending_interrupt()) == 0)
            {
                // massaging the PC this way makes the debugger's behavior more
                // intuitive
                m_pc.w -= 2;

                eat_remaining();
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(144); return; }
//state_144:

                // unmassage...
                m_pc.w += 2;
            }

            if (m_nmi_asserted)
                m_nmi_asserted = false;

            set_ea(m_ea.w); // need to do this to set the addressing mode
            m_cc |= (uint8_t)(CC_I | (m_ea.w != VECTOR_IRQ ? CC_F : 0));

            // invoke standard interrupt callback for MAME core
            switch (m_ea.w)
            {
                case VECTOR_NMI:    standard_irq_callback(INPUT_LINE_NMI); break;
                case VECTOR_FIRQ:   standard_irq_callback(M6809_FIRQ_LINE); break;
                case VECTOR_IRQ:    standard_irq_callback(M6809_IRQ_LINE); break;
                default:            break;
            }

            goto INTERRUPT_VECTOR;

LEA_xy:
            regop16().w = set_flags_u16(CC_Z, m_ea.w);
            dummy_vma(1);
            return;

LEA_us:
            if (regop16_is_m_s())  //if (&regop16() == &m_s)
                m_lds_encountered = true;
            regop16().w = m_ea.w;
            dummy_vma(1);
            return;

PSHS:
            m_temp.w = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(145); return; }
state_145:
            dummy_vma(2);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(146); return; }
state_146:
            set_regop16(m_s);
            if(!hd6309_native_mode()) {
                read_memory(regop16().w);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(147); return; }
//state_147:
                ;
            }
state_147:
            goto PUSH_REGISTERS;

PULS:
            m_temp.w = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(148); return; }
state_148:
            dummy_vma(hd6309_native_mode() ? 1 : 2);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(149); return; }
state_149:
            set_regop16(m_s);
            goto PULL_REGISTERS;

PSHU:
            m_temp.w = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(150); return; }
state_150:
            dummy_vma(2);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(151); return; }
state_151:
            set_regop16(m_u);
            if(!hd6309_native_mode()) {
                read_memory(regop16().w);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(152); return; }
//state_152:
                ;
            }
state_152:
            goto PUSH_REGISTERS;

PULU:
            m_temp.w = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(153); return; }
state_153:
            dummy_vma(hd6309_native_mode() ? 1 : 2);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(154); return; }
state_154:
            set_regop16(m_u);
            goto PULL_REGISTERS;

SWI:
            // doesn't use SOFTWARE_INTERRUPT label because SWI will
            // inhibit IRQ/FIRQ
            set_ea(VECTOR_SWI);
            standard_irq_callback(M6809_SWI);
            m_cc |= CC_E;
            set_regop16(m_s);
            m_temp.w = entire_state_registers();
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(155); return; }
state_155:
            dummy_vma(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(156); return; }
state_156:
            push_state(157);
            goto PUSH_REGISTERS;
state_157:
            m_cc |= CC_I | CC_F;
            goto INTERRUPT_VECTOR;

SWI2:
            set_ea(VECTOR_SWI2);
            standard_irq_callback(M6809_SWI);
            goto SOFTWARE_INTERRUPT;

SWI3:
            set_ea(VECTOR_SWI3);
            standard_irq_callback(M6809_SWI);
            goto SOFTWARE_INTERRUPT;

SOFTWARE_INTERRUPT:
            // used for SWI2/SWI3 and illegal/div0 on 6309
            m_cc |= CC_E;
            set_regop16(m_s);
            m_temp.w = entire_state_registers();
            dummy_read_opcode_arg(0);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(158); return; }
state_158:
            dummy_vma(1);
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(159); return; }
state_159:
            push_state(160);
            goto PUSH_REGISTERS;
state_160:
            goto INTERRUPT_VECTOR;

DIRECT:
            set_ea((uint16_t)(((uint16_t)m_dp << 8) | read_opcode_arg()));
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(161); return; }
state_161:
            if(!hd6309_native_mode()) {
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(162); return; }
//state_162:
                ;
            }
state_162:
            return;

EXTENDED:
            set_ea_h(read_opcode_arg());
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(163); return; }
state_163:
            set_ea_l(read_opcode_arg());
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(164); return; }
state_164:
            if(!hd6309_native_mode()) {
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(165); return; }
//state_165:
                ;
            }
state_165:
            return;

PUSH_REGISTERS:
            if ((m_temp.w & 0x80) != 0)
            {
                write_memory(--regop16().w, m_pc.b.l);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(166); return; }
//state_166:
                write_memory(--regop16().w, m_pc.b.h);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(167); return; }
//state_167:
                nop();
            }
            if ((m_temp.w & 0x40) != 0)
            {
                write_memory(--regop16().w, (regop16_is_m_s()) ? m_u.b.l : m_s.b.l);  //write_memory(--regop16().w, (&regop16() == &m_s) ? m_u.b.l : m_s.b.l);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(168); return; }
//state_168:
                write_memory(--regop16().w, (regop16_is_m_s()) ? m_u.b.h : m_s.b.h);  //write_memory(--regop16().w, (&regop16() == &m_s) ? m_u.b.h : m_s.b.h);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(169); return; }
//state_169:
                nop();
            }
            if ((m_temp.w & 0x20) != 0)
            {
                write_memory(--regop16().w, m_y.b.l);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(170); return; }
//state_170:
                write_memory(--regop16().w, m_y.b.h);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(171); return; }
//state_171:
                nop();
            }
            if ((m_temp.w & 0x10) != 0)
            {
                write_memory(--regop16().w, m_x.b.l);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(172); return; }
//state_172:
                write_memory(--regop16().w, m_x.b.h);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(173); return; }
//state_173:
                nop();
            }
            if ((m_temp.w & 0x08) != 0)
            {
                write_memory(--regop16().w, m_dp);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(174); return; }
//state_174:
                nop();
            }
            if ((m_temp.w & 0x04) != 0)
            {
                write_memory(--regop16().w, m_q.r.b);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(175); return; }
//state_175:
                nop();
            }
            if ((m_temp.w & 0x02) != 0)
            {
                write_memory(--regop16().w, m_q.r.a);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(176); return; }
//state_176:
                nop();
            }
            if ((m_temp.w & 0x01) != 0)
            {
                write_memory(--regop16().w, m_cc);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(177); return; }
//state_177:
                nop();
            }
            return;

PULL_REGISTERS:
            if ((m_temp.w & 0x01) != 0)
            {
                m_cc = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(178); return; }
//state_178:
                nop();
            }
            if ((m_temp.w & 0x02) != 0)
            {
                m_q.r.a = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(179); return; }
//state_179:
                nop();
            }
            if ((m_temp.w & 0x04) != 0)
            {
                m_q.r.b = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(180); return; }
//state_180:
                nop();
            }
            if ((m_temp.w & 0x08) != 0)
            {
                m_dp = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(181); return; }
//state_181:
                nop();
            }
            if ((m_temp.w & 0x10) != 0)
            {
                m_x.b.h = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(182); return; }
//state_182:
                m_x.b.l = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(183); return; }
//state_183:
                nop();
            }
            if ((m_temp.w & 0x20) != 0)
            {
                m_y.b.h = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(184); return; }
//state_184:
                m_y.b.l = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(185); return; }
//state_185:
                nop();
            }
            if ((m_temp.w & 0x40) != 0)
            {
                //(&regop16() == &m_s ? m_u : m_s).b.h = read_memory(regop16().w++);
                if (regop16_is_m_s())
                    m_u.b.h = read_memory(regop16().w++);
                else
                    m_s.b.h = read_memory(regop16().w++);

                if (UNEXPECTED(m_icount.i <= 0)) { push_state(186); return; }
//state_186:
                //(&regop16() == &m_s ? m_u : m_s).b.l = read_memory(regop16().w++);
                if (regop16_is_m_s())
                    m_u.b.l = read_memory(regop16().w++);
                else
                    m_s.b.l = read_memory(regop16().w++);

                if (UNEXPECTED(m_icount.i <= 0)) { push_state(187); return; }
//state_187:
                nop();
            }
            if ((m_temp.w & 0x80) != 0)
            {
                m_pc.b.h = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(188); return; }
//state_188:
                m_pc.b.l = read_memory(regop16().w++);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(189); return; }
//state_189:
                nop();
            }
            read_memory(regop16().w);
            return;

INDEXED:
            m_opcode = read_opcode_arg();
            if (UNEXPECTED(m_icount.i <= 0)) { push_state(190); return; }
state_190:
            if ((m_opcode & 0x80) != 0)
            {
                switch(m_opcode & 0x7F)
                {
                    case 0x00: case 0x20: case 0x40: case 0x60:
                    case 0x10: case 0x30: case 0x50: case 0x70:
                        m_temp.w = ireg();
                        ireg()++;
                        dummy_read_opcode_arg(0);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(191); return; }
//state_191:
                        dummy_vma(2);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(192); return; }
//state_192:
                        break;

                    case 0x01: case 0x21: case 0x41: case 0x61:
                    case 0x11: case 0x31: case 0x51: case 0x71:
                        m_temp.w = ireg();
                        ireg() += 2;
                        dummy_read_opcode_arg(0);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(193); return; }
//state_193:
                        dummy_vma(3);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(194); return; }
//state_194:
                        break;

                    case 0x02: case 0x22: case 0x42: case 0x62:
                    case 0x12: case 0x32: case 0x52: case 0x72:
                        ireg()--;
                        m_temp.w = ireg();
                        dummy_read_opcode_arg(0);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(195); return; }
//state_195:
                        dummy_vma(2);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(196); return; }
//state_196:
                        break;

                    case 0x03: case 0x23: case 0x43: case 0x63:
                    case 0x13: case 0x33: case 0x53: case 0x73:
                        ireg() -= 2;
                        m_temp.w = ireg();
                        dummy_read_opcode_arg(0);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(197); return; }
//state_197:
                        dummy_vma(3);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(198); return; }
//state_198:
                        break;

                    case 0x04: case 0x24: case 0x44: case 0x64:
                    case 0x14: case 0x34: case 0x54: case 0x74:
                        m_temp.w = ireg();
                        dummy_read_opcode_arg(0);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(199); return; }
//state_199:
                        break;

                    case 0x05: case 0x25: case 0x45: case 0x65:
                    case 0x15: case 0x35: case 0x55: case 0x75:
                        m_temp.w = (uint16_t)(ireg() + (int8_t) m_q.r.b);
                        dummy_read_opcode_arg(0);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(200); return; }
//state_200:
                        dummy_vma(1);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(201); return; }
//state_201:
                        break;

                    case 0x06: case 0x26: case 0x46: case 0x66:
                    case 0x16: case 0x36: case 0x56: case 0x76:
                        m_temp.w = (uint16_t)(ireg() + (int8_t) m_q.r.a);
                        dummy_read_opcode_arg(0);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(202); return; }
//state_202:
                        dummy_vma(1);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(203); return; }
//state_203:
                        break;

                    case 0x08: case 0x28: case 0x48: case 0x68:
                    case 0x18: case 0x38: case 0x58: case 0x78:
                        m_temp.w = (uint16_t)(ireg() + (int8_t) read_opcode_arg());
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(204); return; }
//state_204:
                        dummy_read_opcode_arg(0);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(205); return; }
//state_205:
                        break;

                    case 0x09: case 0x29: case 0x49: case 0x69:
                    case 0x19: case 0x39: case 0x59: case 0x79:
                        m_temp.b.h = read_opcode_arg();
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(206); return; }
//state_206:
                        m_temp.b.l = read_opcode_arg();
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(207); return; }
//state_207:
                        m_temp.w = (uint16_t)(ireg() + m_temp.w);
                        dummy_vma(3);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(208); return; }
//state_208:
                        break;

                    case 0x0B: case 0x2B: case 0x4B: case 0x6B:
                    case 0x1B: case 0x3B: case 0x5B: case 0x7B:
                        m_temp.w = (uint16_t)(ireg() + m_q.r.d);
                        dummy_read_opcode_arg(0);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(209); return; }
//state_209:
                        dummy_read_opcode_arg(1);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(210); return; }
//state_210:
                        dummy_vma(3);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(211); return; }
//state_211:
                        break;

                    case 0x0C: case 0x2C: case 0x4C: case 0x6C:
                    case 0x1C: case 0x3C: case 0x5C: case 0x7C:
                        m_temp.b.l = read_opcode_arg();
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(212); return; }
//state_212:
                        m_temp.w = (uint16_t)(m_pc.w + (int8_t) m_temp.b.l);
                        dummy_vma(1);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(213); return; }
//state_213:
                        break;

                    case 0x0D: case 0x2D: case 0x4D: case 0x6D:
                    case 0x1D: case 0x3D: case 0x5D: case 0x7D:
                        m_temp.b.h = read_opcode_arg();
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(214); return; }
//state_214:
                        m_temp.b.l = read_opcode_arg();
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(215); return; }
//state_215:
                        m_temp.w = (uint16_t)(m_pc.w + (int16_t) m_temp.w);
                        dummy_vma(4);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(216); return; }
//state_216:
                        break;

                    case 0x0F: case 0x2F: case 0x4F: case 0x6F:
                    case 0x1F: case 0x3F: case 0x5F: case 0x7F:
                        m_temp.b.h = read_opcode_arg();
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(217); return; }
//state_217:
                        m_temp.b.l = read_opcode_arg();
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(218); return; }
//state_218:
                        dummy_vma(1);
                        if (UNEXPECTED(m_icount.i <= 0)) { push_state(219); return; }
//state_219:
                        break;

                    default:
                        m_temp.w = 0x0000;
                        break;
                }

                // indirect mode
                if ((m_opcode & 0x10) != 0)
                {
                    set_ea(m_temp.w);
                    m_temp.b.h = read_operand(0);
                    if (UNEXPECTED(m_icount.i <= 0)) { push_state(220); return; }
//state_220:
                    m_temp.b.l = read_operand(1);
                    if (UNEXPECTED(m_icount.i <= 0)) { push_state(221); return; }
//state_221:
                    dummy_vma(1);
                    if (UNEXPECTED(m_icount.i <= 0)) { push_state(222); return; }
//state_222:
                    ;
                }
            }
            else
            {
                // 5-bit offset
                m_temp.w = (uint16_t)(ireg() + (int8_t) ((m_opcode & 0x0F) | ((m_opcode & 0x10) != 0 ? 0xF0 : 0x00)));
                dummy_read_opcode_arg(0);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(223); return; }
//state_223:
                dummy_vma(1);
                if (UNEXPECTED(m_icount.i <= 0)) { push_state(224); return; }
//state_224:
                ;
            }
            set_ea(m_temp.w);
            return;

EXG:
            {
                uint8_t param = read_opcode_arg();
                exgtfr_register reg1 = read_exgtfr_register((uint8_t)(param >> 4));
                exgtfr_register reg2 = read_exgtfr_register((uint8_t)(param >> 0));
                write_exgtfr_register((uint8_t)(param >> 4), reg2);
                write_exgtfr_register((uint8_t)(param >> 0), reg1);
            }
            dummy_vma(hd6309_native_mode() ? 3 : 6);
            return;

TFR:
            {
                uint8_t param = read_opcode_arg();
                exgtfr_register reg = read_exgtfr_register((uint8_t)(param >> 4));
                write_exgtfr_register((uint8_t)(param >> 0), reg);
                if ((param & 0x0F) == 4) {
                    m_lds_encountered = true;
                }
            }
            dummy_vma(hd6309_native_mode() ? 2 : 4);
            return;

ILLEGAL:
            log_illegal();
            return;
        }
    }
}
