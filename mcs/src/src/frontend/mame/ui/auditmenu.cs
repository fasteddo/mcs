// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using size_t = System.UInt64;


namespace mame.ui
{
    public static class auditmenu_global
    {
        public static bool sorted_game_list(game_driver x, game_driver y)
        {
            bool clonex = (x.parent[0] != '0') || string.IsNullOrEmpty(x.parent);
            int cx = -1;
            if (clonex)
            {
                cx = driver_list.find(x.parent);
                if ((0 > cx) || ((driver_list.driver((size_t)cx).flags & machine_flags.type.IS_BIOS_ROOT) != 0))
                    clonex = false;
            }

            bool cloney = (y.parent[0] != '0') || string.IsNullOrEmpty(y.parent);
            int cy = -1;
            if (cloney)
            {
                cy = driver_list.find(y.parent);
                if ((0 > cy) || ((driver_list.driver((size_t)cy).flags & machine_flags.type.IS_BIOS_ROOT) != 0))
                    cloney = false;
            }

            if (!clonex && !cloney)
            {
                return g.core_stricmp(x.type.fullname(), y.type.fullname()) < 0;
            }
            else if (clonex && cloney)
            {
                if (g.core_stricmp(x.parent, y.parent) == 0)
                    return g.core_stricmp(x.type.fullname(), y.type.fullname()) < 0;
                else
                    return g.core_stricmp(driver_list.driver((size_t)cx).type.fullname(), driver_list.driver((size_t)cy).type.fullname()) < 0;
            }
            else if (!clonex && cloney)
            {
                if (g.core_stricmp(x.name, y.parent) == 0)
                    return true;
                else
                    return g.core_stricmp(x.type.fullname(), driver_list.driver((size_t)cy).type.fullname()) < 0;
            }
            else
            {
                if (g.core_stricmp(x.parent, y.name) == 0)
                    return false;
                else
                    return (g.core_stricmp(driver_list.driver((size_t)cx).type.fullname(), y.type.fullname()) < 0);
            }
        }
    }
}
