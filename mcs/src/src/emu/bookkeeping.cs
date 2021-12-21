// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u32 = System.UInt32;


namespace mame
{
    // ======================> bookkeeping_manager
    public class bookkeeping_manager
    {
        /* total # of coin counters */
        public const int COIN_COUNTERS = 8;


        // internal state
        running_machine m_machine;                  // reference to our machine

        u32 m_dispensed_tickets;
        u32 [] m_coin_count = new u32[COIN_COUNTERS];
        u32 [] m_coinlockedout = new u32[COIN_COUNTERS];
        u32 [] m_lastcoin = new u32[COIN_COUNTERS];


        // construction/destruction
        //-------------------------------------------------
        //  bookkeeping_manager - constructor
        //-------------------------------------------------
        public bookkeeping_manager(running_machine machine)
        {
            m_machine = machine;
            m_dispensed_tickets = 0;


            /* reset coin counters */
            for (int counternum = 0; counternum < COIN_COUNTERS; counternum++)
            {
                m_lastcoin[counternum] = 0;
                m_coinlockedout[counternum] = 0;
                m_coin_count[counternum] = 0;
            }

            // register coin save state
            machine.save().save_item(m_coin_count, "m_coin_count");
            machine.save().save_item(m_coinlockedout, "m_coinlockedout");
            machine.save().save_item(m_lastcoin, "m_lastcoin");
            machine.save().save_item(m_dispensed_tickets, "m_dispensed_tickets");

            // register for configuration
            machine.configuration().config_register("counters", config_load, config_save);
        }


        // ----- tickets -----
        // return the number of tickets dispensed
        //int get_dispensed_tickets() const;


        // increment the number of dispensed tickets
        //void increment_dispensed_tickets(int delta);


        // ----- coin counters -----
        // write to a particular coin counter (clocks on active high edge)
        /*-------------------------------------------------
            coin_counter_w - sets input for coin counter
        -------------------------------------------------*/
        public void coin_counter_w(int num, int on)
        {
            if (num >= (int)std.size(m_coin_count))
                return;

            /* Count it only if the data has changed from 0 to non-zero */
            if (on != 0 && (m_lastcoin[num] == 0))
                m_coin_count[num]++;
            m_lastcoin[num] = (u32)on;
        }


        // return the coin count for a given coin
        //int coin_counter_get_count(int num);


        // enable/disable coin lockout for a particular coin
        /*-------------------------------------------------
            coin_lockout_w - locks out one coin input
        -------------------------------------------------*/
        void coin_lockout_w(int num, int on)
        {
            if (num >= (int)std.size(m_coinlockedout))
                return;
            m_coinlockedout[num] = (u32)on;
        }

        // return current lockout state for a particular coin
        /*-------------------------------------------------
            coin_lockout_get_state - return current lockout
            state for a particular coin
        -------------------------------------------------*/
        public int coin_lockout_get_state(int num)
        {
            if (num >= (int)std.size(m_coinlockedout))
                return 0;
            return (int)m_coinlockedout[num];
        }


        // enable/disable global coin lockout
        /*-------------------------------------------------
            coin_lockout_global_w - locks out all the coin
            inputs
        -------------------------------------------------*/
        public void coin_lockout_global_w(int on)
        {
            for (int i = 0; i < (int)std.size(m_coinlockedout); i++)
                coin_lockout_w(i, on);
        }


        // getters
        //running_machine &machine() const { return m_machine; }


        /*-------------------------------------------------
            config_load - load the state of the counters
            and tickets
        -------------------------------------------------*/
        void config_load(config_type cfg_type, config_level cfg_level, util.xml.data_node parentnode)
        {
            //throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            config_save - save the state of the counters
            and tickets
        -------------------------------------------------*/
        void config_save(config_type cfg_type, util.xml.data_node parentnode)
        {
            //throw new emu_unimplemented();
        }
    }
}
