// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_timer_id = System.UInt32;
using int8_t = System.SByte;
using int32_t = System.Int32;
using offs_t = System.UInt32;
using u8 = System.Byte;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> ym2151_device
    public class ym2151_device : device_t
                                 //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(YM2151, ym2151_device, "ym2151", "Yamaha YM2151 OPM")
        static device_t device_creator_ym2151_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new ym2151_device(mconfig, tag, owner, clock); }
        public static readonly device_type YM2151 = DEFINE_DEVICE_TYPE(device_creator_ym2151_device, "ym2151", "Yamaha YM2151 OPM");


        public class device_sound_interface_ym2151 : device_sound_interface
        {
            public device_sound_interface_ym2151(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((ym2151_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        //enum
        //{
        const uint32_t TIMER_IRQ_A_OFF = 0;
        const uint32_t TIMER_IRQ_B_OFF = 1;
        const uint32_t TIMER_A = 2;
        const uint32_t TIMER_B = 3;
        //};


        //enum
        //{
        const int RATE_STEPS = 8;
        const int TL_RES_LEN = 256; /* 8 bits addressing (real chip) */

        /*  TL_TAB_LEN is calculated as:
            *   13 - sinus amplitude bits     (Y axis)
            *   2  - sinus sign bit           (Y axis)
            *   TL_RES_LEN - sinus resolution (X axis)
            */
        const int TL_TAB_LEN = 13 * 2 * TL_RES_LEN;

        const int SIN_BITS = 10;
        const int SIN_LEN  = 1 << SIN_BITS;
        const int SIN_MASK = SIN_LEN - 1;
        //};


        const int FREQ_SH         = 16;  /* 16.16 fixed point (frequency calculations) */
        const int EG_SH           = 16;  /* 16.16 fixed point (envelope generator timing) */
        const int LFO_SH          = 10;  /* 22.10 fixed point (LFO calculations)       */
        //#define TIMER_SH        16  /* 16.16 fixed point (timers calculations)    */

        const int FREQ_MASK       = (1 << FREQ_SH) - 1;

        const int ENV_BITS        = 10;
        const int ENV_LEN         = 1 << ENV_BITS;
        const double ENV_STEP     = 128.0 / ENV_LEN;

        const int MAX_ATT_INDEX   = ENV_LEN - 1; /* 1023 */
        const int MIN_ATT_INDEX   = 0;         /* 0 */

        const int EG_ATT          = 4;
        const int EG_DEC          = 3;
        const int EG_SUS          = 2;
        const int EG_REL          = 1;
        const int EG_OFF          = 0;

        const int ENV_QUIET       = TL_TAB_LEN >> 3;


        device_sound_interface_ym2151 m_disound;


        int [] tl_tab = new int [TL_TAB_LEN];
        UInt32 [] sin_tab = new UInt32 [SIN_LEN];
        uint32_t [] d1l_tab = new uint32_t [16];


        static readonly uint8_t [] eg_inc = new uint8_t [19 * RATE_STEPS]
        {
            /*cycle:0 1  2 3  4 5  6 7*/

            /* 0 */ 0,1, 0,1, 0,1, 0,1, /* rates 00..11 0 (increment by 0 or 1) */
            /* 1 */ 0,1, 0,1, 1,1, 0,1, /* rates 00..11 1 */
            /* 2 */ 0,1, 1,1, 0,1, 1,1, /* rates 00..11 2 */
            /* 3 */ 0,1, 1,1, 1,1, 1,1, /* rates 00..11 3 */

            /* 4 */ 1,1, 1,1, 1,1, 1,1, /* rate 12 0 (increment by 1) */
            /* 5 */ 1,1, 1,2, 1,1, 1,2, /* rate 12 1 */
            /* 6 */ 1,2, 1,2, 1,2, 1,2, /* rate 12 2 */
            /* 7 */ 1,2, 2,2, 1,2, 2,2, /* rate 12 3 */

            /* 8 */ 2,2, 2,2, 2,2, 2,2, /* rate 13 0 (increment by 2) */
            /* 9 */ 2,2, 2,4, 2,2, 2,4, /* rate 13 1 */
            /*10 */ 2,4, 2,4, 2,4, 2,4, /* rate 13 2 */
            /*11 */ 2,4, 4,4, 2,4, 4,4, /* rate 13 3 */

            /*12 */ 4,4, 4,4, 4,4, 4,4, /* rate 14 0 (increment by 4) */
            /*13 */ 4,4, 4,8, 4,4, 4,8, /* rate 14 1 */
            /*14 */ 4,8, 4,8, 4,8, 4,8, /* rate 14 2 */
            /*15 */ 4,8, 8,8, 4,8, 8,8, /* rate 14 3 */

            /*16 */ 8,8, 8,8, 8,8, 8,8, /* rates 15 0, 15 1, 15 2, 15 3 (increment by 8) */
            /*17 */ 16,16,16,16,16,16,16,16, /* rates 15 2, 15 3 for attack */
            /*18 */ 0,0, 0,0, 0,0, 0,0, /* infinity rates for attack and decay(s) */
        };


        static uint8_t Osel(uint8_t a) { return (uint8_t)(a * RATE_STEPS); }  //#define O(a) (a*RATE_STEPS)

        /*note that there is no O(17) in this table - it's directly in the code */
        static readonly uint8_t [] eg_rate_select = new uint8_t [32 + 64 + 32]    /* Envelope Generator rates (32 + 64 rates + 32 RKS) */
        {
            /* 32 dummy (infinite time) rates */
            Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),
            Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),
            Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),
            Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),Osel(18),

            /* rates 00-11 */
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),
            Osel( 0),Osel( 1),Osel( 2),Osel( 3),

            /* rate 12 */
            Osel( 4),Osel( 5),Osel( 6),Osel( 7),

            /* rate 13 */
            Osel( 8),Osel( 9),Osel(10),Osel(11),

            /* rate 14 */
            Osel(12),Osel(13),Osel(14),Osel(15),

            /* rate 15 */
            Osel(16),Osel(16),Osel(16),Osel(16),

            /* 32 dummy rates (same as 15 3) */
            Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),
            Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),
            Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),
            Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16),Osel(16)
        };


        /*rate  0,    1,    2,   3,   4,   5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15*/
        /*shift 11,   10,   9,   8,   7,   6,  5,  4,  3,  2, 1,  0,  0,  0,  0,  0 */
        /*mask  2047, 1023, 511, 255, 127, 63, 31, 15, 7,  3, 1,  0,  0,  0,  0,  0 */

        static uint8_t Oshi(uint8_t a) { return (uint8_t)(a * 1); }  //#define O(a) (a*1)
        static readonly uint8_t [] eg_rate_shift = new uint8_t [32 + 64 + 32]      /* Envelope Generator counter shifts (32 + 64 rates + 32 RKS) */
        {
            /* 32 infinite time rates */
            Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),
            Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),
            Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),
            Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),Oshi(0),

            /* rates 00-11 */
            Oshi(11),Oshi(11),Oshi(11),Oshi(11),
            Oshi(10),Oshi(10),Oshi(10),Oshi(10),
            Oshi( 9),Oshi( 9),Oshi( 9),Oshi( 9),
            Oshi( 8),Oshi( 8),Oshi( 8),Oshi( 8),
            Oshi( 7),Oshi( 7),Oshi( 7),Oshi( 7),
            Oshi( 6),Oshi( 6),Oshi( 6),Oshi( 6),
            Oshi( 5),Oshi( 5),Oshi( 5),Oshi( 5),
            Oshi( 4),Oshi( 4),Oshi( 4),Oshi( 4),
            Oshi( 3),Oshi( 3),Oshi( 3),Oshi( 3),
            Oshi( 2),Oshi( 2),Oshi( 2),Oshi( 2),
            Oshi( 1),Oshi( 1),Oshi( 1),Oshi( 1),
            Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),

            /* rate 12 */
            Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),

            /* rate 13 */
            Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),

            /* rate 14 */
            Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),

            /* rate 15 */
            Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),

            /* 32 dummy rates (same as 15 3) */
            Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),
            Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),
            Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),
            Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0),Oshi( 0)
        };


        static readonly uint32_t [] dt2_tab = new uint32_t [4] { 0, 384, 500, 608 };


        static readonly uint8_t [] dt1_tab = new uint8_t [4 * 32]  /* 4*32 DT1 values */
        {
            /* DT1=0 */
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,

            /* DT1=1 */
                0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2,
                2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7, 8, 8, 8, 8,

            /* DT1=2 */
                1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5,
                5, 6, 6, 7, 8, 8, 9,10,11,12,13,14,16,16,16,16,

            /* DT1=3 */
                2, 2, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7,
                8, 8, 9,10,11,12,13,14,16,17,19,20,22,22,22,22
        };


        static readonly uint16_t [] phaseinc_rom = new uint16_t [768]
        {
            1299,1300,1301,1302,1303,1304,1305,1306,1308,1309,1310,1311,1313,1314,1315,1316,
            1318,1319,1320,1321,1322,1323,1324,1325,1327,1328,1329,1330,1332,1333,1334,1335,
            1337,1338,1339,1340,1341,1342,1343,1344,1346,1347,1348,1349,1351,1352,1353,1354,
            1356,1357,1358,1359,1361,1362,1363,1364,1366,1367,1368,1369,1371,1372,1373,1374,
            1376,1377,1378,1379,1381,1382,1383,1384,1386,1387,1388,1389,1391,1392,1393,1394,
            1396,1397,1398,1399,1401,1402,1403,1404,1406,1407,1408,1409,1411,1412,1413,1414,
            1416,1417,1418,1419,1421,1422,1423,1424,1426,1427,1429,1430,1431,1432,1434,1435,
            1437,1438,1439,1440,1442,1443,1444,1445,1447,1448,1449,1450,1452,1453,1454,1455,
            1458,1459,1460,1461,1463,1464,1465,1466,1468,1469,1471,1472,1473,1474,1476,1477,
            1479,1480,1481,1482,1484,1485,1486,1487,1489,1490,1492,1493,1494,1495,1497,1498,
            1501,1502,1503,1504,1506,1507,1509,1510,1512,1513,1514,1515,1517,1518,1520,1521,
            1523,1524,1525,1526,1528,1529,1531,1532,1534,1535,1536,1537,1539,1540,1542,1543,
            1545,1546,1547,1548,1550,1551,1553,1554,1556,1557,1558,1559,1561,1562,1564,1565,
            1567,1568,1569,1570,1572,1573,1575,1576,1578,1579,1580,1581,1583,1584,1586,1587,
            1590,1591,1592,1593,1595,1596,1598,1599,1601,1602,1604,1605,1607,1608,1609,1610,
            1613,1614,1615,1616,1618,1619,1621,1622,1624,1625,1627,1628,1630,1631,1632,1633,
            1637,1638,1639,1640,1642,1643,1645,1646,1648,1649,1651,1652,1654,1655,1656,1657,
            1660,1661,1663,1664,1666,1667,1669,1670,1672,1673,1675,1676,1678,1679,1681,1682,
            1685,1686,1688,1689,1691,1692,1694,1695,1697,1698,1700,1701,1703,1704,1706,1707,
            1709,1710,1712,1713,1715,1716,1718,1719,1721,1722,1724,1725,1727,1728,1730,1731,
            1734,1735,1737,1738,1740,1741,1743,1744,1746,1748,1749,1751,1752,1754,1755,1757,
            1759,1760,1762,1763,1765,1766,1768,1769,1771,1773,1774,1776,1777,1779,1780,1782,
            1785,1786,1788,1789,1791,1793,1794,1796,1798,1799,1801,1802,1804,1806,1807,1809,
            1811,1812,1814,1815,1817,1819,1820,1822,1824,1825,1827,1828,1830,1832,1833,1835,
            1837,1838,1840,1841,1843,1845,1846,1848,1850,1851,1853,1854,1856,1858,1859,1861,
            1864,1865,1867,1868,1870,1872,1873,1875,1877,1879,1880,1882,1884,1885,1887,1888,
            1891,1892,1894,1895,1897,1899,1900,1902,1904,1906,1907,1909,1911,1912,1914,1915,
            1918,1919,1921,1923,1925,1926,1928,1930,1932,1933,1935,1937,1939,1940,1942,1944,
            1946,1947,1949,1951,1953,1954,1956,1958,1960,1961,1963,1965,1967,1968,1970,1972,
            1975,1976,1978,1980,1982,1983,1985,1987,1989,1990,1992,1994,1996,1997,1999,2001,
            2003,2004,2006,2008,2010,2011,2013,2015,2017,2019,2021,2022,2024,2026,2028,2029,
            2032,2033,2035,2037,2039,2041,2043,2044,2047,2048,2050,2052,2054,2056,2058,2059,
            2062,2063,2065,2067,2069,2071,2073,2074,2077,2078,2080,2082,2084,2086,2088,2089,
            2092,2093,2095,2097,2099,2101,2103,2104,2107,2108,2110,2112,2114,2116,2118,2119,
            2122,2123,2125,2127,2129,2131,2133,2134,2137,2139,2141,2142,2145,2146,2148,2150,
            2153,2154,2156,2158,2160,2162,2164,2165,2168,2170,2172,2173,2176,2177,2179,2181,
            2185,2186,2188,2190,2192,2194,2196,2197,2200,2202,2204,2205,2208,2209,2211,2213,
            2216,2218,2220,2222,2223,2226,2227,2230,2232,2234,2236,2238,2239,2242,2243,2246,
            2249,2251,2253,2255,2256,2259,2260,2263,2265,2267,2269,2271,2272,2275,2276,2279,
            2281,2283,2285,2287,2288,2291,2292,2295,2297,2299,2301,2303,2304,2307,2308,2311,
            2315,2317,2319,2321,2322,2325,2326,2329,2331,2333,2335,2337,2338,2341,2342,2345,
            2348,2350,2352,2354,2355,2358,2359,2362,2364,2366,2368,2370,2371,2374,2375,2378,
            2382,2384,2386,2388,2389,2392,2393,2396,2398,2400,2402,2404,2407,2410,2411,2414,
            2417,2419,2421,2423,2424,2427,2428,2431,2433,2435,2437,2439,2442,2445,2446,2449,
            2452,2454,2456,2458,2459,2462,2463,2466,2468,2470,2472,2474,2477,2480,2481,2484,
            2488,2490,2492,2494,2495,2498,2499,2502,2504,2506,2508,2510,2513,2516,2517,2520,
            2524,2526,2528,2530,2531,2534,2535,2538,2540,2542,2544,2546,2549,2552,2553,2556,
            2561,2563,2565,2567,2568,2571,2572,2575,2577,2579,2581,2583,2586,2589,2590,2593
        };


        static readonly uint8_t [] lfo_noise_waveform = new uint8_t [256]
        {
            0xFF,0xEE,0xD3,0x80,0x58,0xDA,0x7F,0x94,0x9E,0xE3,0xFA,0x00,0x4D,0xFA,0xFF,0x6A,
            0x7A,0xDE,0x49,0xF6,0x00,0x33,0xBB,0x63,0x91,0x60,0x51,0xFF,0x00,0xD8,0x7F,0xDE,
            0xDC,0x73,0x21,0x85,0xB2,0x9C,0x5D,0x24,0xCD,0x91,0x9E,0x76,0x7F,0x20,0xFB,0xF3,
            0x00,0xA6,0x3E,0x42,0x27,0x69,0xAE,0x33,0x45,0x44,0x11,0x41,0x72,0x73,0xDF,0xA2,

            0x32,0xBD,0x7E,0xA8,0x13,0xEB,0xD3,0x15,0xDD,0xFB,0xC9,0x9D,0x61,0x2F,0xBE,0x9D,
            0x23,0x65,0x51,0x6A,0x84,0xF9,0xC9,0xD7,0x23,0xBF,0x65,0x19,0xDC,0x03,0xF3,0x24,
            0x33,0xB6,0x1E,0x57,0x5C,0xAC,0x25,0x89,0x4D,0xC5,0x9C,0x99,0x15,0x07,0xCF,0xBA,
            0xC5,0x9B,0x15,0x4D,0x8D,0x2A,0x1E,0x1F,0xEA,0x2B,0x2F,0x64,0xA9,0x50,0x3D,0xAB,

            0x50,0x77,0xE9,0xC0,0xAC,0x6D,0x3F,0xCA,0xCF,0x71,0x7D,0x80,0xA6,0xFD,0xFF,0xB5,
            0xBD,0x6F,0x24,0x7B,0x00,0x99,0x5D,0xB1,0x48,0xB0,0x28,0x7F,0x80,0xEC,0xBF,0x6F,
            0x6E,0x39,0x90,0x42,0xD9,0x4E,0x2E,0x12,0x66,0xC8,0xCF,0x3B,0x3F,0x10,0x7D,0x79,
            0x00,0xD3,0x1F,0x21,0x93,0x34,0xD7,0x19,0x22,0xA2,0x08,0x20,0xB9,0xB9,0xEF,0x51,

            0x99,0xDE,0xBF,0xD4,0x09,0x75,0xE9,0x8A,0xEE,0xFD,0xE4,0x4E,0x30,0x17,0xDF,0xCE,
            0x11,0xB2,0x28,0x35,0xC2,0x7C,0x64,0xEB,0x91,0x5F,0x32,0x0C,0x6E,0x00,0xF9,0x92,
            0x19,0xDB,0x8F,0xAB,0xAE,0xD6,0x12,0xC4,0x26,0x62,0xCE,0xCC,0x0A,0x03,0xE7,0xDD,
            0xE2,0x4D,0x8A,0xA6,0x46,0x95,0x0F,0x8F,0xF5,0x15,0x97,0x32,0xD4,0x28,0x1E,0x55
        };


        /* struct describing a single operator */
        class YM2151Operator
        {
            public uint32_t      phase;                  /* accumulated operator phase */
            public uint32_t      freq;                   /* operator frequency count */
            public int32_t       dt1;                    /* current DT1 (detune 1 phase inc/decrement) value */
            public uint32_t      mul;                    /* frequency count multiply */
            public uint32_t      dt1_i;                  /* DT1 index * 32 */
            public uint32_t      dt2;                    /* current DT2 (detune 2) value */

            public intref connect;  //signed int *connect;                /* operator output 'direction' */

            /* only M1 (operator 0) is filled with this data: */
            public intref mem_connect;  //signed int *mem_connect;            /* where to put the delayed sample (MEM) */
            public int32_t       mem_value;              /* delayed sample (MEM) value */

            /* channel specific data; note: each operator number 0 contains channel specific data */
            public uint32_t      fb_shift;               /* feedback shift value for operators 0 in each channel */
            public int32_t       fb_out_curr;            /* operator feedback value (used only by operators 0) */
            public int32_t       fb_out_prev;            /* previous feedback value (used only by operators 0) */
            public uint32_t      kc;                     /* channel KC (copied to all operators) */
            public uint32_t      kc_i;                   /* just for speedup */
            public uint32_t      pms;                    /* channel PMS */
            public uint32_t      ams;                    /* channel AMS */
            /* end of channel specific data */

            public uint32_t      AMmask;                 /* LFO Amplitude Modulation enable mask */
            public uint32_t      state;                  /* Envelope state: 4-attack(AR) 3-decay(D1R) 2-sustain(D2R) 1-release(RR) 0-off */
            public uint8_t       eg_sh_ar;               /*  (attack state) */
            public uint8_t       eg_sel_ar;              /*  (attack state) */
            public uint32_t      tl;                     /* Total attenuation Level */
            public int32_t       volume;                 /* current envelope attenuation level */
            public uint8_t       eg_sh_d1r;              /*  (decay state) */
            public uint8_t       eg_sel_d1r;             /*  (decay state) */
            public uint32_t      d1l;                    /* envelope switches to sustain state after reaching this level */
            public uint8_t       eg_sh_d2r;              /*  (sustain state) */
            public uint8_t       eg_sel_d2r;             /*  (sustain state) */
            public uint8_t       eg_sh_rr;               /*  (release state) */
            public uint8_t       eg_sel_rr;              /*  (release state) */

            public uint32_t      key;                    /* 0=last key was KEY OFF, 1=last key was KEY ON */

            public uint32_t      ks;                     /* key scale    */
            public uint32_t      ar;                     /* attack rate  */
            public uint32_t      d1r;                    /* decay rate   */
            public uint32_t      d2r;                    /* sustain rate */
            public uint32_t      rr;                     /* release rate */

            public uint32_t      reserved0;              /**/
            public uint32_t      reserved1;              /**/


            public void key_on(uint32_t key_set, uint32_t eg_cnt)
            {
                if (key == 0)
                {
                    phase = 0;            /* clear phase */
                    state = EG_ATT;       /* KEY ON = attack */
                    volume += (~volume * (eg_inc[eg_sel_ar + ((eg_cnt >> eg_sh_ar) & 7)])) >> 4;
                    if (volume <= MIN_ATT_INDEX)
                    {
                        volume = MIN_ATT_INDEX;
                        state = EG_DEC;
                    }
                }

                key |= key_set;
            }


            public void key_off(uint32_t key_set)
            {
                if (key != 0)
                {
                    key &= ~key_set;
                    if (key == 0)
                    {
                        if (state > EG_REL)
                            state = EG_REL; /* KEY OFF = release */
                    }
                }
            }
        }


        intref [] chanout = new intref [8];  //signed int chanout[8];
        intref m2 = new intref();  //signed int m2,c1,c2; /* Phase Modulation input for operators 2,3,4 */
        intref c1 = new intref();
        intref c2 = new intref();
        intref mem = new intref();  //signed int mem;     /* one sample delay memory */

        MemoryContainer<YM2151Operator> oper = new MemoryContainer<YM2151Operator>(32, true);           /* the 32 operators */

        uint32_t [] pan = new uint32_t[16];                /* channels output masks (0xffffffff = enable) */

        uint32_t eg_cnt;                 /* global envelope generator counter */
        uint32_t eg_timer;               /* global envelope generator counter works at frequency = chipclock/64/3 */
        uint32_t eg_timer_add;           /* step of eg_timer */
        uint32_t eg_timer_overflow;      /* envelope generator timer overflows every 3 samples (on real chip) */

        uint32_t lfo_phase;              /* accumulated LFO phase (0 to 255) */
        uint32_t lfo_timer;              /* LFO timer                        */
        uint32_t lfo_timer_add;          /* step of lfo_timer                */
        uint32_t lfo_overflow;           /* LFO generates new output when lfo_timer reaches this value */
        uint32_t lfo_counter;            /* LFO phase increment counter      */
        uint32_t lfo_counter_add;        /* step of lfo_counter              */
        uint8_t lfo_wsel;               /* LFO waveform (0-saw, 1-square, 2-triangle, 3-random noise) */
        uint8_t amd;                    /* LFO Amplitude Modulation Depth   */
        int8_t pmd;                    /* LFO Phase Modulation Depth       */
        uint32_t lfa;                    /* LFO current AM output            */
        int32_t lfp;                    /* LFO current PM output            */

        uint8_t test;                   /* TEST register */
        uint8_t ct;                     /* output control pins (bit1-CT2, bit0-CT1) */

        uint32_t noise;                  /* noise enable/period register (bit 7 - noise enable, bits 4-0 - noise period */
        uint32_t noise_rng;              /* 17 bit noise shift register */
        uint32_t noise_p;                /* current noise 'phase'*/
        uint32_t noise_f;                /* current noise period */

        uint32_t csm_req;                /* CSM  KEY ON / KEY OFF sequence request */

        uint32_t irq_enable;             /* IRQ enable for timer B (bit 3) and timer A (bit 2); bit 7 - CSM mode (keyon to all slots, everytime timer A overflows) */
        uint32_t status;                 /* chip status (BUSY, IRQ Flags) */
        uint8_t [] connect = new uint8_t[8];             /* channels connections */

        emu_timer timer_A;
        emu_timer timer_A_irq_off;
        emu_timer timer_B;
        emu_timer timer_B_irq_off;

        attotime [] timer_A_time = new attotime [1024];     /* timer A times for MAME */
        attotime [] timer_B_time = new attotime [256];      /* timer B times for MAME */

        int irqlinestate;

        uint32_t timer_A_index;          /* timer A index */
        uint32_t timer_B_index;          /* timer B index */
        uint32_t timer_A_index_old;      /* timer A previous index */
        uint32_t timer_B_index_old;      /* timer B previous index */

        /*  Frequency-deltas to get the closest frequency possible.
        *   There are 11 octaves because of DT2 (max 950 cents over base frequency)
        *   and LFO phase modulation (max 800 cents below AND over base frequency)
        *   Summary:   octave  explanation
        *              0       note code - LFO PM
        *              1       note code
        *              2       note code
        *              3       note code
        *              4       note code
        *              5       note code
        *              6       note code
        *              7       note code
        *              8       note code
        *              9       note code + DT2 + LFO PM
        *              10      note code + DT2 + LFO PM
        */
        uint32_t [] freq = new uint32_t [11 * 768];           /* 11 octaves, 768 'cents' per octave */

        /*  Frequency deltas for DT1. These deltas alter operator frequency
        *   after it has been taken from frequency-deltas table.
        */
        int32_t [] dt1_freq = new int32_t [8 * 32];         /* 8 DT1 levels, 32 KC values */

        // internal state
        sound_stream m_stream;
        uint8_t m_lastreg;
        devcb_write_line m_irqhandler;
        devcb_write8 m_portwritehandler;
        bool m_reset_active;


        // construction/destruction
        ym2151_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, YM2151, tag, owner, clock)
        {
        }


        ym2151_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_ym2151(mconfig, this));  //device_sound_interface(mconfig, *this),

            m_disound = GetClassInterface<device_sound_interface_ym2151>();


            chanout.Fill(() => { return new intref(); });
            oper.Fill(() => { return new YM2151Operator(); });


            m_stream = null;
            m_lastreg = 0;
            m_irqhandler = new devcb_write_line(this);
            m_portwritehandler = new devcb_write8(this);
            m_reset_active = false;
        }


        public device_sound_interface_ym2151 disound { get { return m_disound; } }


        // configuration helpers
        //auto irq_handler() { return m_irqhandler.bind(); }
        //auto port_write_handler() { return m_portwritehandler.bind(); }


        // read/write

        //-------------------------------------------------
        //  read - read from the device
        //-------------------------------------------------
        public u8 read(offs_t offset)
        {
            if ((offset & 1) != 0)
            {
                m_stream.update();
                return (u8)status;
            }
            else
            {
                return 0xff;    /* confirmed on a real YM2151 */
            }
        }


        //-------------------------------------------------
        //  write - write from the device
        //-------------------------------------------------
        public void write(offs_t offset, u8 data)
        {
            if ((offset & 1) != 0)
            {
                if (!m_reset_active)
                {
                    m_stream.update();
                    write_reg(m_lastreg, data);
                }
            }
            else
            {
                m_lastreg = data;
            }
        }


        //u8 status_r();
        //void register_w(u8 data);
        //void data_w(u8 data);


        //-------------------------------------------------
        //  reset_w - handle writes to the reset lines of
        //  the YM2151 and its associated DAC
        //-------------------------------------------------
        //WRITE_LINE_MEMBER(ym2151_device::reset_w)
        public void reset_w(int state)
        {
            // active low reset
            if (!m_reset_active && state == 0)
                reset();

            m_reset_active = state == 0;
        }


        // device-level overrides
        protected override void device_start()
        {
            init_tables();

            m_irqhandler.resolve_safe();
            m_portwritehandler.resolve_safe();

            m_stream = disound.stream_alloc(0, 2, clock() / 64);

            timer_A_irq_off = timer_alloc(TIMER_IRQ_A_OFF);
            timer_B_irq_off = timer_alloc(TIMER_IRQ_B_OFF);
            timer_A = timer_alloc(TIMER_A);
            timer_B = timer_alloc(TIMER_B);

            lfo_timer_add = 1 << LFO_SH;

            eg_timer_add  = 1 << EG_SH;
            eg_timer_overflow = 3 * eg_timer_add;

            irqlinestate = 0;

            //throw new emu_unimplemented();
#if false
            /* save all 32 operators */
            save_item(STRUCT_MEMBER(oper, phase));
            save_item(STRUCT_MEMBER(oper, freq));
            save_item(STRUCT_MEMBER(oper, dt1));
            save_item(STRUCT_MEMBER(oper, mul));
            save_item(STRUCT_MEMBER(oper, dt1_i));
            save_item(STRUCT_MEMBER(oper, dt2));
            /* operators connection is saved in chip data block */
            save_item(STRUCT_MEMBER(oper, mem_value));

            save_item(STRUCT_MEMBER(oper, fb_shift));
            save_item(STRUCT_MEMBER(oper, fb_out_curr));
            save_item(STRUCT_MEMBER(oper, fb_out_prev));
            save_item(STRUCT_MEMBER(oper, kc));
            save_item(STRUCT_MEMBER(oper, kc_i));
            save_item(STRUCT_MEMBER(oper, pms));
            save_item(STRUCT_MEMBER(oper, ams));
            save_item(STRUCT_MEMBER(oper, AMmask));

            save_item(STRUCT_MEMBER(oper, state));
            save_item(STRUCT_MEMBER(oper, eg_sh_ar));
            save_item(STRUCT_MEMBER(oper, eg_sel_ar));
            save_item(STRUCT_MEMBER(oper, tl));
            save_item(STRUCT_MEMBER(oper, volume));
            save_item(STRUCT_MEMBER(oper, eg_sh_d1r));
            save_item(STRUCT_MEMBER(oper, eg_sel_d1r));
            save_item(STRUCT_MEMBER(oper, d1l));
            save_item(STRUCT_MEMBER(oper, eg_sh_d2r));
            save_item(STRUCT_MEMBER(oper, eg_sel_d2r));
            save_item(STRUCT_MEMBER(oper, eg_sh_rr));
            save_item(STRUCT_MEMBER(oper, eg_sel_rr));

            save_item(STRUCT_MEMBER(oper, key));
            save_item(STRUCT_MEMBER(oper, ks));
            save_item(STRUCT_MEMBER(oper, ar));
            save_item(STRUCT_MEMBER(oper, d1r));
            save_item(STRUCT_MEMBER(oper, d2r));
            save_item(STRUCT_MEMBER(oper, rr));

            save_item(STRUCT_MEMBER(oper, reserved0));
            save_item(STRUCT_MEMBER(oper, reserved1));
#endif

            save_item(NAME(new { pan }));

            save_item(NAME(new { eg_cnt }));
            save_item(NAME(new { eg_timer }));
            save_item(NAME(new { eg_timer_add }));
            save_item(NAME(new { eg_timer_overflow }));

            save_item(NAME(new { lfo_phase }));
            save_item(NAME(new { lfo_timer }));
            save_item(NAME(new { lfo_timer_add }));
            save_item(NAME(new { lfo_overflow }));
            save_item(NAME(new { lfo_counter }));
            save_item(NAME(new { lfo_counter_add }));
            save_item(NAME(new { lfo_wsel }));
            save_item(NAME(new { amd }));
            save_item(NAME(new { pmd }));
            save_item(NAME(new { lfa }));
            save_item(NAME(new { lfp }));

            save_item(NAME(new { test }));
            save_item(NAME(new { ct }));

            save_item(NAME(new { noise }));
            save_item(NAME(new { noise_rng }));
            save_item(NAME(new { noise_p }));
            save_item(NAME(new { noise_f }));

            save_item(NAME(new { csm_req }));
            save_item(NAME(new { irq_enable }));
            save_item(NAME(new { status }));

            save_item(NAME(new { timer_A_index }));
            save_item(NAME(new { timer_B_index }));
            save_item(NAME(new { timer_A_index_old }));
            save_item(NAME(new { timer_B_index_old }));

            save_item(NAME(new { irqlinestate }));

            save_item(NAME(new { connect }));

            save_item(NAME(new { m_reset_active }));
        }


        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            int i;
            /* initialize hardware registers */
            for (i = 0; i < 32; i++)
            {
                oper[i] = new YM2151Operator();  //memset(&oper[i],'\0', sizeof(YM2151Operator));
                oper[i].volume = MAX_ATT_INDEX;
                    oper[i].kc_i = 768; /* min kc_i value */
            }

            eg_timer = 0;
            eg_cnt   = 0;

            lfo_timer  = 0;
            lfo_counter= 0;
            lfo_phase  = 0;
            lfo_wsel   = 0;
            pmd = 0;
            amd = 0;
            lfa = 0;
            lfp = 0;

            test = 0;

            irq_enable = 0;
            /* ASG 980324 -- reset the timers before writing to the registers */
            timer_A.enable(false);
            timer_B.enable(false);
            timer_A_index = 0;
            timer_B_index = 0;
            timer_A_index_old = 0;
            timer_B_index_old = 0;

            noise     = 0;
            noise_rng = 0;
            noise_p   = 0;
            noise_f   = 32;

            csm_req   = 0;
            status    = 0;

            write_reg(0x1b, 0);    /* only because of CT1, CT2 output pins */
            write_reg(0x18, 0);    /* set LFO frequency */

            for (i = 0x20; i < 0x100; i++)      /* set the operators */
            {
                write_reg(i, 0);
            }
        }


        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            throw new emu_unimplemented();
        }


        protected override void device_post_load()
        {
            throw new emu_unimplemented();
        }


        protected override void device_clock_changed()
        {
            m_stream.set_sample_rate(clock() / 64);
            calculate_timers();
        }


        // sound stream update overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            if (m_reset_active)
            {
                outputs[0].fill(0);
                outputs[1].fill(0);
                return;
            }

            for (int sampindex = 0; sampindex < outputs[0].samples(); sampindex++)
            {
                advance_eg();

                for (int ch = 0; ch < 8; ch++)
                    chanout[ch].i = 0;

                for (int ch = 0; ch < 7; ch++)
                    chan_calc((unsigned)ch);

                chan7_calc();

                int outl = 0;
                int outr = 0;
                for (int ch = 0; ch < 8; ch++)
                {
                    outl += chanout[ch].i & (int)pan[2 * ch];
                    outr += chanout[ch].i & (int)pan[2 * ch + 1];
                }

                outputs[0].put_int_clamp(sampindex, outl, 32768);
                outputs[1].put_int_clamp(sampindex, outr, 32768);

                advance();
            }
        }


        protected virtual void calculate_timers()
        {
            /* calculate timers' deltas */
            for (int i = 0; i < 1024; i++)
            {
                /* ASG 980324: changed to compute both tim_A_tab and timer_A_time */
                timer_A_time[i] = clocks_to_attotime(64 * (1024 - (uint64_t)i));
            }

            for (int i = 0; i < 256; i++)
            {
                /* ASG 980324: changed to compute both tim_B_tab and timer_B_time */
                timer_B_time[i] = clocks_to_attotime(1024 * (256 - (uint64_t)i));
            }
        }


        /* write a register on YM2151 chip number 'n' */
        protected virtual void write_reg(int r, int v)
        {
            Pointer<YM2151Operator> op = new Pointer<YM2151Operator>(oper, (r & 0x07) * 4 + ((r & 0x18) >> 3));  //YM2151Operator *op = &oper[ (r&0x07)*4+((r&0x18)>>3) ];

            /* adjust bus to 8 bits */
            r &= 0xff;
            v &= 0xff;

#if false
            /* There is no info on what YM2151 really does when busy flag is set */
            if ( status & 0x80 ) return;
            timer_set ( attotime::from_hz(clock()) * 64, chip, 0, timer_callback_chip_busy);
            status |= 0x80;   /* set busy flag for 64 chip clock cycles */
#endif

            switch (r & 0xe0)
            {
            case 0x00:
                switch (r)
                {
                case 0x01:  /* LFO reset(bit 1), Test Register (other bits) */
                    test = (uint8_t)v;
                    if ((v & 2) != 0) lfo_phase = 0;
                    break;

                case 0x08:
                    envelope_KONKOFF(new Pointer<YM2151Operator>(oper, (v & 7) * 4), v );
                    break;

                case 0x0f:  /* noise mode enable, noise period */
                    noise = (uint32_t)v;
                    noise_f = std.max(2, 32 - ((uint32_t)v & 0x1f)); /* rate 30 and 31 are the same */
                    break;

                case 0x10:  /* timer A hi */
                    timer_A_index = (timer_A_index & 0x003) | ((uint32_t)v << 2);
                    break;

                case 0x11:  /* timer A low */
                    timer_A_index = (timer_A_index & 0x3fc) | ((uint32_t)v & 3);
                    break;

                case 0x12:  /* timer B */
                    timer_B_index = (uint32_t)v;
                    break;

                case 0x14:  /* CSM, irq flag reset, irq enable, timer start/stop */

                    irq_enable = (uint32_t)v;   /* bit 3-timer B, bit 2-timer A, bit 7 - CSM */

                    if ((v & 0x10) != 0) /* reset timer A irq flag */
                    {
                        status &= ~1U;
                        timer_A_irq_off.adjust(attotime.zero);
                    }

                    if ((v & 0x20) != 0) /* reset timer B irq flag */
                    {
                        status &= ~2U;
                        timer_B_irq_off.adjust(attotime.zero);
                    }

                    if ((v & 0x02) != 0)
                    {
                        /* load and start timer B */
                        /* ASG 980324: added a real timer */
                        /* start timer _only_ if it wasn't already started (it will reload time value next round) */
                        if (!timer_B.enable(true))
                        {
                            timer_B.adjust(timer_B_time[timer_B_index]);
                            timer_B_index_old = timer_B_index;
                        }
                    }
                    else
                    {
                        /* stop timer B */
                        timer_B.enable(false);
                    }

                    if ((v & 0x01) != 0)
                    {   /* load and start timer A */
                        /* ASG 980324: added a real timer */
                        /* start timer _only_ if it wasn't already started (it will reload time value next round) */
                        if (!timer_A.enable(true))
                        {
                            timer_A.adjust(timer_A_time[timer_A_index]);
                            timer_A_index_old = timer_A_index;
                        }
                    }
                    else
                    {       /* stop timer A */
                        /* ASG 980324: added a real timer */
                        timer_A.enable(false);
                    }
                    break;

                case 0x18:  /* LFO frequency */
                    {
                        lfo_overflow    = (1U << ((15 - (v >> 4)) + 3)) * (1 << LFO_SH);
                        lfo_counter_add = 0x10 + ((uint32_t)v & 0x0f);
                    }
                    break;

                case 0x19:  /* PMD (bit 7==1) or AMD (bit 7==0) */
                    if ((v & 0x80) != 0)
                        pmd = (int8_t)(v & 0x7f);
                    else
                        amd = (uint8_t)(v & 0x7f);
                    break;

                case 0x1b:  /* CT2, CT1, LFO waveform */
                    ct = (uint8_t)(v >> 6);
                    lfo_wsel = (uint8_t)(v & 3);
                    m_portwritehandler.op(0, ct, 0xff);
                    break;

                default:
                    logerror("YM2151 Write {0} to undocumented register #{1}\n", v, r);
                    break;
                }
                break;

            case 0x20:
                op = new Pointer<YM2151Operator>(oper, (r & 7) * 4);
                switch (r & 0x18)
                {
                case 0x00:  /* RL enable, Feedback, Connection */
                    op[0].fb_shift = (((v >> 3) & 7) != 0) ? (((uint32_t)v >> 3) & 7) + 6 : 0;
                    pan[(r & 7) * 2    ] = ((v & 0x40) != 0) ? ~0U : 0;
                    pan[(r & 7) * 2 + 1] = ((v & 0x80) != 0) ? ~0U : 0;
                    connect[r & 7] = (uint8_t)(v & 7);
                    set_connect(op, r & 7, v & 7);
                    break;

                case 0x08:  /* Key Code */
                    v &= 0x7f;
                    if (v != op.op.kc)
                    {
                        uint32_t kc;
                        uint32_t kc_channel;

                        kc_channel = ((uint32_t)v - ((uint32_t)v >> 2)) * 64;
                        kc_channel += 768;
                        kc_channel |= (op.op.kc_i & 63);

                        (op + 0).op.kc = (uint32_t)v;
                        (op + 0).op.kc_i = kc_channel;
                        (op + 1).op.kc = (uint32_t)v;
                        (op + 1).op.kc_i = kc_channel;
                        (op + 2).op.kc = (uint32_t)v;
                        (op + 2).op.kc_i = kc_channel;
                        (op + 3).op.kc = (uint32_t)v;
                        (op + 3).op.kc_i = kc_channel;

                        kc = (uint32_t)v >> 2;

                        (op + 0).op.dt1 = dt1_freq[(op + 0).op.dt1_i + kc];
                        (op + 0).op.freq = ((freq[kc_channel + (op + 0).op.dt2] + (uint32_t)(op + 0).op.dt1) * (op + 0).op.mul) >> 1;

                        (op + 1).op.dt1 = dt1_freq[(op + 1).op.dt1_i + kc];
                        (op + 1).op.freq = ((freq[kc_channel + (op + 1).op.dt2] + (uint32_t)(op + 1).op.dt1) * (op + 1).op.mul) >> 1;

                        (op + 2).op.dt1 = dt1_freq[(op + 2).op.dt1_i + kc];
                        (op + 2).op.freq = ((freq[kc_channel + (op + 2).op.dt2] + (uint32_t)(op + 2).op.dt1) * (op + 2).op.mul) >> 1;

                        (op + 3).op.dt1 = dt1_freq[(op + 3).op.dt1_i + kc];
                        (op + 3).op.freq = ((freq[kc_channel + (op + 3).op.dt2] + (uint32_t)(op + 3).op.dt1) * (op + 3).op.mul) >> 1;

                        refresh_EG(op);
                    }
                    break;

                case 0x10:  /* Key Fraction */
                    v >>= 2;
                    if (v != (op.op.kc_i & 63))
                    {
                        uint32_t kc_channel;

                        kc_channel = (uint32_t)v;
                        kc_channel |= (op.op.kc_i & ~63U);

                        (op + 0).op.kc_i = kc_channel;
                        (op + 1).op.kc_i = kc_channel;
                        (op + 2).op.kc_i = kc_channel;
                        (op + 3).op.kc_i = kc_channel;

                        (op + 0).op.freq = ((freq[kc_channel + (op + 0).op.dt2] + (uint32_t)(op + 0).op.dt1) * (op + 0).op.mul) >> 1;
                        (op + 1).op.freq = ((freq[kc_channel + (op + 1).op.dt2] + (uint32_t)(op + 1).op.dt1) * (op + 1).op.mul) >> 1;
                        (op + 2).op.freq = ((freq[kc_channel + (op + 2).op.dt2] + (uint32_t)(op + 2).op.dt1) * (op + 2).op.mul) >> 1;
                        (op + 3).op.freq = ((freq[kc_channel + (op + 3).op.dt2] + (uint32_t)(op + 3).op.dt1) * (op + 3).op.mul) >> 1;
                    }
                    break;

                case 0x18:  /* PMS, AMS */
                    op.op.pms = ((uint32_t)v >> 4) & 7;
                    op.op.ams = ((uint32_t)v & 3);
                    break;
                }
                break;

            case 0x40:      /* DT1, MUL */
                {
                    uint32_t olddt1_i = op.op.dt1_i;
                    uint32_t oldmul = op.op.mul;

                    op.op.dt1_i = ((uint32_t)v & 0x70) << 1;
                    op.op.mul   = ((v & 0x0f) != 0) ? ((uint32_t)v & 0x0f) << 1 : 1;

                    if (olddt1_i != op.op.dt1_i)
                        op.op.dt1 = dt1_freq[op.op.dt1_i + (op.op.kc >> 2)];

                    if ((olddt1_i != op.op.dt1_i) || (oldmul != op.op.mul))
                        op.op.freq = ((freq[op.op.kc_i + op.op.dt2] + (uint32_t)op.op.dt1) * op.op.mul) >> 1;
                }
                break;

            case 0x60:      /* TL */
                op.op.tl = ((uint32_t)v & 0x7f) << (ENV_BITS - 7); /* 7bit TL */
                break;

            case 0x80:      /* KS, AR */
                {
                    uint32_t oldks = op.op.ks;
                    uint32_t oldar = op.op.ar;

                    op.op.ks = 5 - ((uint32_t)v >> 6);
                    op.op.ar = ((v & 0x1f) != 0) ? 32 + (((uint32_t)v & 0x1f) << 1) : 0;

                    if ((op.op.ar != oldar) || (op.op.ks != oldks))
                    {
                        if ((op.op.ar + (op.op.kc >> (int)op.op.ks)) < 32 + 62)
                        {
                            op.op.eg_sh_ar  = eg_rate_shift [op.op.ar  + (op.op.kc >> (int)op.op.ks)];
                            op.op.eg_sel_ar = eg_rate_select[op.op.ar  + (op.op.kc >> (int)op.op.ks)];
                        }
                        else
                        {
                            op.op.eg_sh_ar  = 0;
                            op.op.eg_sel_ar = 17 * RATE_STEPS;
                        }
                    }

                    if (op.op.ks != oldks)
                    {
                        op.op.eg_sh_d1r = eg_rate_shift [op.op.d1r + (op.op.kc >> (int)op.op.ks) ];
                        op.op.eg_sel_d1r= eg_rate_select[op.op.d1r + (op.op.kc >> (int)op.op.ks) ];
                        op.op.eg_sh_d2r = eg_rate_shift [op.op.d2r + (op.op.kc >> (int)op.op.ks) ];
                        op.op.eg_sel_d2r= eg_rate_select[op.op.d2r + (op.op.kc >> (int)op.op.ks) ];
                        op.op.eg_sh_rr  = eg_rate_shift [op.op.rr  + (op.op.kc >> (int)op.op.ks) ];
                        op.op.eg_sel_rr = eg_rate_select[op.op.rr  + (op.op.kc >> (int)op.op.ks) ];
                    }
                }
                break;

            case 0xa0:      /* LFO AM enable, D1R */
                op.op.AMmask = ((v & 0x80) != 0) ? ~0U : 0;
                op.op.d1r    = ((v & 0x1f) != 0) ? 32 + (((uint32_t)v & 0x1f) << 1) : 0;
                op.op.eg_sh_d1r = eg_rate_shift [op.op.d1r + (op.op.kc >> (int)op.op.ks)];
                op.op.eg_sel_d1r= eg_rate_select[op.op.d1r + (op.op.kc >> (int)op.op.ks)];
                break;

            case 0xc0:      /* DT2, D2R */
                {
                    uint32_t olddt2 = op.op.dt2;
                    op.op.dt2 = dt2_tab[v >> 6];
                    if (op.op.dt2 != olddt2)
                        op.op.freq = ((freq[op.op.kc_i + op.op.dt2] + (uint32_t)op.op.dt1) * op.op.mul) >> 1;
                }
                op.op.d2r = ((v & 0x1f) != 0) ? 32 + (((uint32_t)v & 0x1f) << 1) : 0;
                op.op.eg_sh_d2r = eg_rate_shift [op.op.d2r + (op.op.kc >> (int)op.op.ks)];
                op.op.eg_sel_d2r= eg_rate_select[op.op.d2r + (op.op.kc >> (int)op.op.ks)];
                break;

            case 0xe0:      /* D1L, RR */
                op.op.d1l = d1l_tab[v >> 4];
                op.op.rr  = 34 + (((uint32_t)v & 0x0f) << 2);
                op.op.eg_sh_rr  = eg_rate_shift [op.op.rr + (op.op.kc >> (int)op.op.ks)];
                op.op.eg_sel_rr = eg_rate_select[op.op.rr + (op.op.kc >> (int)op.op.ks)];
                break;
            }
        }



        void init_tables()
        {
            for (int x = 0; x < TL_RES_LEN; x++)
            {
                double m = floor(1 << 16) / pow(2, (x + 1) * (ENV_STEP / 4.0) / 8.0);

                /* we never reach (1<<16) here due to the (x+1) */
                /* result fits within 16 bits at maximum */

                int n = (int)m;     /* 16 bits here */
                n >>= 4;        /* 12 bits here */
                if ((n & 1) != 0)        /* round to closest */
                    n = (n >> 1) + 1;
                else
                    n = n >> 1;
                                /* 11 bits here (rounded) */
                n <<= 2;        /* 13 bits here (as in real chip) */
                tl_tab[ x * 2 + 0 ] = n;
                tl_tab[ x * 2 + 1 ] = -tl_tab[ x * 2 + 0 ];

                for (int i=1; i<13; i++)
                {
                    tl_tab[ x*2+0 + i*2*TL_RES_LEN ] =  tl_tab[ x*2+0 ]>>i;
                    tl_tab[ x*2+1 + i*2*TL_RES_LEN ] = -tl_tab[ x*2+0 + i*2*TL_RES_LEN ];
                }
            }

            for (int i = 0; i < SIN_LEN; i++)
            {
                /* non-standard sinus */
                double m = sin( ((i * 2) + 1) * M_PI / SIN_LEN ); /* verified on the real chip */

                /* we never reach zero here due to ((i*2)+1) */

                /* convert to 'decibels' */
                double o = 8 * log(1.0 / fabs(m)) / log(2.0);

                o = o / (ENV_STEP / 4);

                int n = (int)(2.0 * o);
                if ((n & 1) != 0)                        /* round to closest */
                    n = (n >> 1) + 1;
                else
                    n = n >> 1;

                sin_tab[i] = (UInt32)(n * 2 + (m >= 0.0 ? 0 : 1 ));
            }

            /* calculate d1l_tab table */
            for (int i = 0; i < 16; i++)
            {
                d1l_tab[i] = (uint32_t)((i != 15 ? i : i + 16) * (4.0 / ENV_STEP));   /* every 3 'dB' except for all bits = 1 = 45+48 'dB' */
            }

            /* this loop calculates Hertz values for notes from c-0 to b-7 */
            /* including 64 'cents' (100/64 that is 1.5625 of real cent) per note */
            /* i*100/64/1200 is equal to i/768 */

            /* real chip works with 10 bits fixed point values (10.10) */

            for (int i = 0; i < 768; i++)
            {
                /* octave 2 - reference octave */
                freq[ 768 + 2 * 768 + i ] = (uint32_t)(phaseinc_rom[i] << (FREQ_SH - 10)) & 0xffffffc0; /* adjust to X.10 fixed point */

                /* octave 0 and octave 1 */
                for (int j = 0; j < 2; j++)
                {
                    freq[768 + j * 768 + i] = (freq[ 768 + 2 * 768 + i ] >> (2 - j) ) & 0xffffffc0; /* adjust to X.10 fixed point */
                }

                /* octave 3 to 7 */
                for (int j = 3; j < 8; j++)
                {
                    freq[768 + j * 768 + i] = freq[ 768 + 2 * 768 + i ] << (j - 2);
                }
            }

            /* octave -1 (all equal to: oct 0, _KC_00_, _KF_00_) */
            for (int i = 0; i < 768; i++)
            {
                freq[ 0 * 768 + i ] = freq[1 * 768 + 0];
            }

            /* octave 8 and 9 (all equal to: oct 7, _KC_14_, _KF_63_) */
            for (int j = 8; j < 10; j++)
            {
                for (int i = 0; i < 768; i++)
                {
                    freq[768 + j * 768 + i ] = freq[768 + 8 * 768 -1];
                }
            }

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 32; i++)
                {
                    /*calculate phase increment, positive and negative values*/
                    dt1_freq[ (j + 0) * 32 + i ] = (dt1_tab[j * 32 + i] * SIN_LEN) >> (20 - FREQ_SH);
                    dt1_freq[ (j + 4) * 32 + i ] = -dt1_freq[ (j + 0) * 32 + i ];
                }
            }
        }


        // m1, m2, c1, c2
        static readonly uint8_t [] envelope_KONKOFF_masks = new uint8_t [4] { 0x08, 0x20, 0x10, 0x40 };

        void envelope_KONKOFF(Pointer<YM2151Operator> op, int v)  //void ym2151_device::envelope_KONKOFF(YM2151Operator * op, int v)
        {
            // m1, m2, c1, c2
            //static uint8_t masks[4] = { 0x08, 0x20, 0x10, 0x40 };

            for (int i = 0; i != 4; i++)
            {
                if ((v & envelope_KONKOFF_masks[i]) != 0) /* M1 */
                    op[i].key_on(1, eg_cnt);
                else
                    op[i].key_off(1);
            }
        }


        void set_connect(Pointer<YM2151Operator> om1, int cha, int v)  //void ym2151_device::set_connect(YM2151Operator *om1, int cha, int v)
        {
            Pointer<YM2151Operator> om2 = om1 + 1;  //YM2151Operator *om2 = om1+1;
            Pointer<YM2151Operator> oc1 = om1 + 2;  //YM2151Operator *oc1 = om1+2;

            /* set connect algorithm */

            /* MEM is simply one sample delay */

            switch (v & 7)
            {
            case 0:
                /* M1---C1---MEM---M2---C2---OUT */
                om1.op.connect = c1;
                oc1.op.connect = mem;
                om2.op.connect = c2;
                om1.op.mem_connect = m2;
                break;

            case 1:
                /* M1------+-MEM---M2---C2---OUT */
                /*      C1-+                     */
                om1.op.connect = mem;
                oc1.op.connect = mem;
                om2.op.connect = c2;
                om1.op.mem_connect = m2;
                break;

            case 2:
                /* M1-----------------+-C2---OUT */
                /*      C1---MEM---M2-+          */
                om1.op.connect = c2;
                oc1.op.connect = mem;
                om2.op.connect = c2;
                om1.op.mem_connect = m2;
                break;

            case 3:
                /* M1---C1---MEM------+-C2---OUT */
                /*                 M2-+          */
                om1.op.connect = c1;
                oc1.op.connect = mem;
                om2.op.connect = c2;
                om1.op.mem_connect = c2;
                break;

            case 4:
                /* M1---C1-+-OUT */
                /* M2---C2-+     */
                /* MEM: not used */
                om1.op.connect = c1;
                oc1.op.connect = chanout[cha];
                om2.op.connect = c2;
                om1.op.mem_connect = mem;   /* store it anywhere where it will not be used */
                break;

            case 5:
                /*    +----C1----+     */
                /* M1-+-MEM---M2-+-OUT */
                /*    +----C2----+     */
                om1.op.connect = null;   /* special mark */
                oc1.op.connect = chanout[cha];
                om2.op.connect = chanout[cha];
                om1.op.mem_connect = m2;
                break;

            case 6:
                /* M1---C1-+     */
                /*      M2-+-OUT */
                /*      C2-+     */
                /* MEM: not used */
                om1.op.connect = c1;
                oc1.op.connect = chanout[cha];
                om2.op.connect = chanout[cha];
                om1.op.mem_connect = mem;   /* store it anywhere where it will not be used */
                break;

            case 7:
                /* M1-+     */
                /* C1-+-OUT */
                /* M2-+     */
                /* C2-+     */
                /* MEM: not used*/
                om1.op.connect = chanout[cha];
                oc1.op.connect = chanout[cha];
                om2.op.connect = chanout[cha];
                om1.op.mem_connect = mem;   /* store it anywhere where it will not be used */
                break;
            }
        }


        void advance()
        {
            Pointer<YM2151Operator> op;  //YM2151Operator *op;
            UInt32 i;
            int a;
            int p;

            /* LFO */
            if ((test & 2) != 0)
            {
                lfo_phase = 0;
            }
            else
            {
                lfo_timer += lfo_timer_add;
                if (lfo_timer >= lfo_overflow)
                {
                    lfo_timer   -= lfo_overflow;
                    lfo_counter += lfo_counter_add;
                    lfo_phase   += (lfo_counter>>4);
                    lfo_phase   &= 255;
                    lfo_counter &= 15;
                }
            }

            i = lfo_phase;
            /* calculate LFO AM and PM waveform value (all verified on real chip, except for noise algorithm which is impossible to analyse)*/
            switch (lfo_wsel)
            {
            case 0:
                /* saw */
                /* AM: 255 down to 0 */
                /* PM: 0 to 127, -127 to 0 (at PMD=127: LFP = 0 to 126, -126 to 0) */
                a = 255 - (int)i;
                if (i < 128)
                    p = (int)i;
                else
                    p = (int)i - 255;
                break;
            case 1:
                /* square */
                /* AM: 255, 0 */
                /* PM: 128,-128 (LFP = exactly +PMD, -PMD) */
                if (i < 128)
                {
                    a = 255;
                    p = 128;
                }
                else
                {
                    a = 0;
                    p = -128;
                }
                break;
            case 2:
                /* triangle */
                /* AM: 255 down to 1 step -2; 0 up to 254 step +2 */
                /* PM: 0 to 126 step +2, 127 to 1 step -2, 0 to -126 step -2, -127 to -1 step +2*/
                if (i < 128)
                    a = 255 - ((int)i * 2);
                else
                    a = ((int)i * 2) - 256;

                if (i < 64)                       /* i = 0..63 */
                    p = (int)i * 2;                    /* 0 to 126 step +2 */
                else if (i < 128)                 /* i = 64..127 */
                        p = 255 - (int)i * 2;          /* 127 to 1 step -2 */
                    else if (i < 192)             /* i = 128..191 */
                            p = 256 - (int)i * 2;      /* 0 to -126 step -2*/
                        else                      /* i = 192..255 */
                            p = (int)i * 2 - 511;      /*-127 to -1 step +2*/
                break;
            case 3:
            default:    /*keep the compiler happy*/
                /* random */
                /* the real algorithm is unknown !!!
                    We just use a snapshot of data from real chip */

                /* AM: range 0 to 255    */
                /* PM: range -128 to 127 */

                a = lfo_noise_waveform[i];
                p = a - 128;
                break;
            }
            lfa = (uint32_t)(a * amd / 128);
            lfp = p * pmd / 128;


            /*  The Noise Generator of the YM2151 is 17-bit shift register.
            *   Input to the bit16 is negated (bit0 XOR bit3) (EXNOR).
            *   Output of the register is negated (bit0 XOR bit3).
            *   Simply use bit16 as the noise output.
            */
            noise_p += 2; // 32 clock per noise (2 * sample rate)
            while (noise_p >= noise_f)
            {
                uint32_t j;
                j = ( (noise_rng ^ (noise_rng >> 3) ) & 1) ^ 1;
                noise_rng = (j << 16) | (noise_rng >> 1);
                noise_p -= noise_f;
            }


            /* phase generator */
            op = new Pointer<YM2151Operator>(oper, 0);  //op = &oper[0]; /* CH 0 M1 */
            i = 8;
            do
            {
                if (op.op.pms != 0)    /* only when phase modulation from LFO is enabled for this channel */
                {
                    int32_t mod_ind = lfp;       /* -128..+127 (8bits signed) */
                    if (op.op.pms < 6)
                        mod_ind >>= (6 - (int)op.op.pms);
                    else
                        mod_ind <<= ((int)op.op.pms - 5);

                    if (mod_ind != 0)
                    {
                        uint32_t kc_channel = op.op.kc_i + (uint32_t)mod_ind;
                        (op + 0).op.phase += ((freq[kc_channel + (op + 0).op.dt2] + (uint32_t)(op + 0).op.dt1) * (op + 0).op.mul) >> 1;
                        (op + 1).op.phase += ((freq[kc_channel + (op + 1).op.dt2] + (uint32_t)(op + 1).op.dt1) * (op + 1).op.mul) >> 1;
                        (op + 2).op.phase += ((freq[kc_channel + (op + 2).op.dt2] + (uint32_t)(op + 2).op.dt1) * (op + 2).op.mul) >> 1;
                        (op + 3).op.phase += ((freq[kc_channel + (op + 3).op.dt2] + (uint32_t)(op + 3).op.dt1) * (op + 3).op.mul) >> 1;
                    }
                    else        /* phase modulation from LFO is equal to zero */
                    {
                        (op + 0).op.phase += (op + 0).op.freq;
                        (op + 1).op.phase += (op + 1).op.freq;
                        (op + 2).op.phase += (op + 2).op.freq;
                        (op + 3).op.phase += (op + 3).op.freq;
                    }
                }
                else            /* phase modulation from LFO is disabled */
                {
                    (op + 0).op.phase += (op + 0).op.freq;
                    (op + 1).op.phase += (op + 1).op.freq;
                    (op + 2).op.phase += (op + 2).op.freq;
                    (op + 3).op.phase += (op + 3).op.freq;
                }

                op += 4;
                i--;
            } while (i != 0);


            /* CSM is calculated *after* the phase generator calculations (verified on real chip)
            * CSM keyon line seems to be ORed with the KO line inside of the chip.
            * The result is that it only works when KO (register 0x08) is off, ie. 0
            *
            * Interesting effect is that when timer A is set to 1023, the KEY ON happens
            * on every sample, so there is no KEY OFF at all - the result is that
            * the sound played is the same as after normal KEY ON.
            */

            if (csm_req != 0)           /* CSM KEYON/KEYOFF seqeunce request */
            {
                if (csm_req == 2)    /* KEY ON */
                {
                    op = new Pointer<YM2151Operator>(oper, 0);  //op = &oper[0]; /* CH 0 M1 */
                    i = 32;
                    do
                    {
                        op.op.key_on(2, eg_cnt);
                        op++;
                        i--;
                    } while (i != 0);
                    csm_req = 1;
                }
                else                    /* KEY OFF */
                {
                    op = new Pointer<YM2151Operator>(oper, 0);  //op = &oper[0]; /* CH 0 M1 */
                    i = 32;
                    do
                    {
                        op.op.key_off(2);
                        op++;
                        i--;
                    } while (i != 0);
                    csm_req = 0;
                }
            }
        }


        void advance_eg()
        {
            Pointer<YM2151Operator> op;  //YM2151Operator *op;
            UInt32 i;

            eg_timer += eg_timer_add;

            while (eg_timer >= eg_timer_overflow)
            {
                eg_timer -= eg_timer_overflow;

                eg_cnt++;

                /* envelope generator */
                op = new Pointer<YM2151Operator>(oper, 0);  //op = &oper[0]; /* CH 0 M1 */
                i = 32;
                do
                {
                    switch (op.op.state)
                    {
                    case EG_ATT:    /* attack phase */
                        if ((eg_cnt & ((1U << op.op.eg_sh_ar) - 1)) == 0)
                        {
                            op.op.volume += (~op.op.volume *
                                            (eg_inc[op.op.eg_sel_ar + ((eg_cnt >> op.op.eg_sh_ar) & 7)])
                                            ) >> 4;

                            if (op.op.volume <= MIN_ATT_INDEX)
                            {
                                op.op.volume = MIN_ATT_INDEX;
                                op.op.state = EG_DEC;
                            }

                        }
                    break;

                    case EG_DEC:    /* decay phase */
                        if ((eg_cnt & ((1U << op.op.eg_sh_d1r) - 1)) == 0)
                        {
                            op.op.volume += eg_inc[op.op.eg_sel_d1r + ((eg_cnt >> op.op.eg_sh_d1r) & 7)];

                            if (op.op.volume >= op.op.d1l)
                                op.op.state = EG_SUS;

                        }
                    break;

                    case EG_SUS:    /* sustain phase */
                        if ((eg_cnt & ((1U << op.op.eg_sh_d2r) - 1)) == 0)
                        {
                            op.op.volume += eg_inc[op.op.eg_sel_d2r + ((eg_cnt >> op.op.eg_sh_d2r) & 7)];

                            if (op.op.volume >= MAX_ATT_INDEX)
                            {
                                op.op.volume = MAX_ATT_INDEX;
                                op.op.state = EG_OFF;
                            }

                        }
                    break;

                    case EG_REL:    /* release phase */
                        if ((eg_cnt & ((1U << op.op.eg_sh_rr) - 1)) == 0)
                        {
                            op.op.volume += eg_inc[op.op.eg_sel_rr + ((eg_cnt >> op.op.eg_sh_rr) & 7)];

                            if (op.op.volume >= MAX_ATT_INDEX)
                            {
                                op.op.volume = MAX_ATT_INDEX;
                                op.op.state = EG_OFF;
                            }

                        }
                    break;
                    }

                    op++;
                    i--;
                } while (i != 0);
            }
        }


        static UInt32 volume_calc(Pointer<YM2151Operator> OP, uint32_t AM) { return OP.op.tl + ((uint32_t)OP.op.volume) + (AM & OP.op.AMmask); }  //#define volume_calc(OP) ((OP)->tl + ((uint32_t)(OP)->volume) + (AM & (OP)->AMmask))

        void chan_calc(unsigned chan)
        {
            Pointer<YM2151Operator> op;  //YM2151Operator *op;
            unsigned env;
            uint32_t AM = 0;

            m2.i = c1.i = c2.i = mem.i = 0;
            op = new Pointer<YM2151Operator>(oper, (int)chan * 4);  //op = &oper[chan * 4];    /* M1 */

            op.op.mem_connect.i = op.op.mem_value;   /* restore delayed sample (MEM) value to m2 or c2 */

            if (op.op.ams != 0)
                AM = lfa << (int)(op.op.ams - 1);

            env = volume_calc(op, AM);

            {
                int32_t out_ = op.op.fb_out_prev + op.op.fb_out_curr;
                op.op.fb_out_prev = op.op.fb_out_curr;

                if (op.op.connect.i == 0)
                    /* algorithm 5 */
                    mem.i = c1.i = c2.i = op.op.fb_out_prev;
                else
                    /* other algorithms */
                    op.op.connect.i = op.op.fb_out_prev;

                op.op.fb_out_curr = 0;
                if (env < ENV_QUIET)
                {
                    if (op.op.fb_shift == 0)
                        out_ = 0;
                    op.op.fb_out_curr = op_calc1(op, env, (out_ << (int)op.op.fb_shift));
                }
            }

            env = volume_calc(op + 1, AM);    /* M2 */
            if (env < ENV_QUIET)
                (op + 1).op.connect.i += op_calc(op + 1, env, m2.i);

            env = volume_calc(op + 2, AM);    /* C1 */
            if (env < ENV_QUIET)
                (op + 2).op.connect.i += op_calc(op + 2, env, c1.i);

            env = volume_calc(op + 3, AM);    /* C2 */
            if (env < ENV_QUIET)
                chanout[chan].i += op_calc(op + 3, env, c2.i);
            //  if(chan==3) printf("%d\n", chanout[chan]);

            /* M1 */
            op.op.mem_value = mem.i;
        }


        void chan7_calc()
        {
            Pointer<YM2151Operator> op;  //YM2151Operator *op;
            UInt32 env;
            uint32_t AM = 0;

            m2.i = c1.i = c2.i = mem.i = 0;
            op = new Pointer<YM2151Operator>(oper, 7 * 4);  //op = &oper[7 * 4];       /* M1 */

            op.op.mem_connect.i = op.op.mem_value;   /* restore delayed sample (MEM) value to m2 or c2 */

            if (op.op.ams != 0)
                AM = lfa << (int)(op.op.ams - 1);

            env = volume_calc(op, AM);

            {
                int32_t out_ = op.op.fb_out_prev + op.op.fb_out_curr;
                op.op.fb_out_prev = op.op.fb_out_curr;

                if (op.op.connect.i == 0)
                    /* algorithm 5 */
                    mem.i = c1.i = c2.i = op.op.fb_out_prev;
                else
                    /* other algorithms */
                    op.op.connect.i = op.op.fb_out_prev;

                op.op.fb_out_curr = 0;
                if (env < ENV_QUIET)
                {
                    if (op.op.fb_shift == 0)
                        out_ = 0;

                    op.op.fb_out_curr = op_calc1(op, env, (out_ << (int)op.op.fb_shift) );
                }
            }

            env = volume_calc(op + 1, AM);    /* M2 */
            if (env < ENV_QUIET)
                (op + 1).op.connect.i += op_calc(op + 1, env, m2.i);

            env = volume_calc(op + 2, AM);    /* C1 */
            if (env < ENV_QUIET)
                (op + 2).op.connect.i += op_calc(op + 2, env, c1.i);

            env = volume_calc(op + 3, AM);    /* C2 */
            if ((noise & 0x80) != 0)
            {
                uint32_t noiseout;

                noiseout = 0;
                if (env < 0x3ff)
                    noiseout = (env ^ 0x3ff) * 2;   /* range of the YM2151 noise output is -2044 to 2040 */

                chanout[7].i += (((noise_rng & 0x10000) != 0) ? (int)noiseout: (int)-noiseout); /* bit 16 -> output */
            }
            else
            {
                if (env < ENV_QUIET)
                    chanout[7].i += op_calc(op + 3, env, c2.i);
            }

            /* M1 */
            op.op.mem_value = mem.i;
        }


        int op_calc(Pointer<YM2151Operator> OP, UInt32 env, int pm)  //int op_calc(YM2151Operator * OP, unsigned int env, signed int pm);
        {
            uint32_t p;

            p = (env << 3) + sin_tab[(((int)((OP.op.phase & ~FREQ_MASK) + (pm << 15))) >> FREQ_SH) & SIN_MASK];

            if (p >= TL_TAB_LEN)
                return 0;

            return tl_tab[p];
        }


        int op_calc1(Pointer<YM2151Operator> OP, UInt32 env, int pm)  //int op_calc1(YM2151Operator * OP, unsigned int env, signed int pm);
        {
            uint32_t p;
            int32_t  i;

            i = (int)(OP.op.phase & unchecked((uint32_t)~FREQ_MASK)) + pm;

            /*logerror("i=%08x (i>>16)&511=%8i phase=%i [pm=%08x] ",i, (i>>16)&511, OP->phase>>FREQ_SH, pm);*/

            p = (env<<3) + sin_tab[ (i>>FREQ_SH) & SIN_MASK];

            /*logerror("(p&255=%i p>>8=%i) out= %i\n", p&255,p>>8, tl_tab[p&255]>>(p>>8) );*/

            if (p >= TL_TAB_LEN)
                return 0;

            return tl_tab[p];
        }


        void refresh_EG(Pointer<YM2151Operator> opIn)  //void ym2151_device::refresh_EG(YM2151Operator * op)
        {
            var op = new Pointer<YM2151Operator>(opIn);

            uint32_t kc;
            uint32_t v;

            kc = op.op.kc;

            /* v = 32 + 2*RATE + RKS = max 126 */

            v = kc >> (int)op.op.ks;
            if ((op.op.ar + v) < 32 + 62)
            {
                op.op.eg_sh_ar  = eg_rate_shift [op.op.ar + v ];
                op.op.eg_sel_ar = eg_rate_select[op.op.ar + v ];
            }
            else
            {
                op.op.eg_sh_ar  = 0;
                op.op.eg_sel_ar = 17 * RATE_STEPS;
            }
            op.op.eg_sh_d1r = eg_rate_shift [op.op.d1r + v];
            op.op.eg_sel_d1r= eg_rate_select[op.op.d1r + v];
            op.op.eg_sh_d2r = eg_rate_shift [op.op.d2r + v];
            op.op.eg_sel_d2r= eg_rate_select[op.op.d2r + v];
            op.op.eg_sh_rr  = eg_rate_shift [op.op.rr  + v];
            op.op.eg_sel_rr = eg_rate_select[op.op.rr  + v];


            op += 1;

            v = kc >> (int)op.op.ks;
            if ((op.op.ar + v) < 32 + 62)
            {
                op.op.eg_sh_ar  = eg_rate_shift [op.op.ar + v ];
                op.op.eg_sel_ar = eg_rate_select[op.op.ar + v ];
            }
            else
            {
                op.op.eg_sh_ar  = 0;
                op.op.eg_sel_ar = 17 * RATE_STEPS;
            }
            op.op.eg_sh_d1r = eg_rate_shift [op.op.d1r + v];
            op.op.eg_sel_d1r= eg_rate_select[op.op.d1r + v];
            op.op.eg_sh_d2r = eg_rate_shift [op.op.d2r + v];
            op.op.eg_sel_d2r= eg_rate_select[op.op.d2r + v];
            op.op.eg_sh_rr  = eg_rate_shift [op.op.rr  + v];
            op.op.eg_sel_rr = eg_rate_select[op.op.rr  + v];

            op += 1;

            v = kc >> (int)op.op.ks;
            if ((op.op.ar + v) < 32 + 62)
            {
                op.op.eg_sh_ar  = eg_rate_shift [op.op.ar  + v ];
                op.op.eg_sel_ar = eg_rate_select[op.op.ar  + v ];
            }
            else
            {
                op.op.eg_sh_ar  = 0;
                op.op.eg_sel_ar = 17 * RATE_STEPS;
            }
            op.op.eg_sh_d1r = eg_rate_shift [op.op.d1r + v];
            op.op.eg_sel_d1r= eg_rate_select[op.op.d1r + v];
            op.op.eg_sh_d2r = eg_rate_shift [op.op.d2r + v];
            op.op.eg_sel_d2r= eg_rate_select[op.op.d2r + v];
            op.op.eg_sh_rr  = eg_rate_shift [op.op.rr  + v];
            op.op.eg_sel_rr = eg_rate_select[op.op.rr  + v];

            op += 1;

            v = kc >> (int)op.op.ks;
            if ((op.op.ar + v) < 32 + 62)
            {
                op.op.eg_sh_ar  = eg_rate_shift [op.op.ar  + v ];
                op.op.eg_sel_ar = eg_rate_select[op.op.ar  + v ];
            }
            else
            {
                op.op.eg_sh_ar  = 0;
                op.op.eg_sel_ar = 17 * RATE_STEPS;
            }
            op.op.eg_sh_d1r = eg_rate_shift [op.op.d1r + v];
            op.op.eg_sel_d1r= eg_rate_select[op.op.d1r + v];
            op.op.eg_sh_d2r = eg_rate_shift [op.op.d2r + v];
            op.op.eg_sel_d2r= eg_rate_select[op.op.d2r + v];
            op.op.eg_sh_rr  = eg_rate_shift [op.op.rr  + v];
            op.op.eg_sel_rr = eg_rate_select[op.op.rr  + v];
        }
    }


    // ======================> ym2164_device
    //class ym2164_device : public ym2151_device
}
