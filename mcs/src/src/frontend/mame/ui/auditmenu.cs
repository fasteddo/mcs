// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.ui
{
    public static class auditmenu_global
    {
        public static int cs_stricmp(string s1, string s2)
        {
            //for (;;)
            //{
            //    int c1 = tolower((UINT8)*s1++);
            //    int c2 = tolower((UINT8)*s2++);
            //    if (c1 == 0 || c1 != c2)
            //        return c1 - c2;
            //}
            return global.core_stricmp(s1, s2);
        }


        public static int sorted_game_list(game_driver x, game_driver y)
        {
            bool clonex = !string.IsNullOrEmpty(x.parent);
            bool cloney = !string.IsNullOrEmpty(y.parent);

            if (!clonex && !cloney)
                return cs_stricmp(x.type.fullname(), y.type.fullname()); // < 0);

            int cx = -1, cy = -1;
            if (clonex)
            {
                cx = driver_list.find(x.parent);
                if (cx == -1 || ((UInt64)driver_list.driver((UInt32)cx).flags & gamedrv_global.MACHINE_IS_BIOS_ROOT) != 0)
                    clonex = false;
            }

            if (cloney)
            {
                cy = driver_list.find(y.parent);
                if (cy == -1 || ((UInt64)driver_list.driver((UInt32)cy).flags & gamedrv_global.MACHINE_IS_BIOS_ROOT) != 0)
                    cloney = false;
            }

            if (!clonex && !cloney)
            {
                return cs_stricmp(x.type.fullname(), y.type.fullname()); // < 0);
            }
            else if (clonex && cloney)
            {
                if (cs_stricmp(x.parent, y.parent) == 0)
                    return cs_stricmp(x.type.fullname(), y.type.fullname()); // < 0);
                else
                    return cs_stricmp(driver_list.driver((UInt32)cx).type.fullname(), driver_list.driver((UInt32)cy).type.fullname()); // < 0);
            }
            else if (!clonex && cloney)
            {
                if (cs_stricmp(x.name, y.parent) == 0)
                    return cs_stricmp(x.name, y.parent);
                else
                    return cs_stricmp(x.type.fullname(), driver_list.driver((UInt32)cy).type.fullname()); // < 0);
            }
            else
            {
                if (cs_stricmp(x.parent, y.name) == 0)
                    return cs_stricmp(x.parent, y.name);
                else
                    return cs_stricmp(driver_list.driver((UInt32)cx).type.fullname(), y.type.fullname()); // < 0);
            }
        }
    }
}
