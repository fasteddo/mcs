// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class corealloc_global
    {
        //**************************************************************************
        //  MACROS
        //**************************************************************************

        // global allocation helpers -- use these instead of new and delete
        //#define global_alloc(Type)                          new Type
        //#define global_alloc_array(Type, Num)               new Type[Num]
        public static MemoryContainer<T> global_alloc_array<T>(UInt32 Num) where T : new()  //#define global_alloc_array(Type, Num)               new Type[Num]
        {
            MemoryContainer<T> list = new MemoryContainer<T>((int)Num);
            for (int i = 0; i < Num; i++)
                list.Add(new T());

            return list;
        }
        //#define global_free(Ptr)                            do { delete Ptr; } while (0)
        //#define global_free_array(Ptr)                      do { delete[] Ptr; } while (0)



        //template<typename T, typename... Params>
        //inline T* global_alloc_clear(Params &&... args)
        //{
        //    void *const ptr = ::operator new(sizeof(T)); // allocate memory
        //    std::memset(ptr, 0, sizeof(T));
        //    return new(ptr) T(std::forward<Params>(args)...);
        //}

        //template<typename T>
        public static MemoryContainer<T> global_alloc_array_clear<T>(UInt32 num) where T : new()  //inline T* global_alloc_array_clear(std::size_t num)
        {
            //auto const size = sizeof(T) * num;
            //void *const ptr = new unsigned char[size]; // allocate memory
            //std::memset(ptr, 0, size);
            //return new(ptr) T[num]();
            return global_alloc_array<T>(num);
        }
    }
}
