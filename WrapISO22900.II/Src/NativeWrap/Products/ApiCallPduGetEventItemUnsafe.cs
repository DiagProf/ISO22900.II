using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ISO22900.II
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    //using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits
    //using CHAR8 = System.Byte;      //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))
    
    internal class ApiCallPduGetEventItemUnsafe : ApiCallPduGetEventItem
    {
        private readonly PduEventItemUnsafeFactory _eventItemFactory;
        
        internal override unsafe Queue<PduEventItem> PduGetEventItem(uint moduleHandle, uint comLogicalLinkHandle)
        {
            //An event callback is either generated when there are events in the queue when the
            //PDURegisterEventCallback function is called that registers a callback function or the ComLogicalLinkLayer’s Event Queue transitions
            //from empty to NOT empty. In other words, multiple events will not generate multiple callbacks even though each is a separate
            //event item in the Event Queue. The application is responsible for reading ALL events from the ComLogicalLinkLayer’s Event Queue
            //before another callback will be generated.

            var queue = new Queue<PduEventItem>();

            PduError result;
            do
            {
                PDU_EVENT_ITEM* pPduEventItem;
                CheckResultThrowException(result = PDUGetEventItem(moduleHandle, comLogicalLinkHandle, &pPduEventItem));
                if (result == PduError.PDU_STATUS_NOERROR)
                {
                    CheckResultThrowException(result);
                    queue.Enqueue(_eventItemFactory.GetPduEventItemFromPduEventItemPointer(pPduEventItem));
                    CheckResultThrowException(PDUDestroyItem((PDU_ITEM*) pPduEventItem));
                }
            } while (result != PduError.PDU_ERR_EVENT_QUEUE_EMPTY);

            return queue;
        }

        internal ApiCallPduGetEventItemUnsafe(IntPtr handleToLoadedNativeLibrary) : base(handleToLoadedNativeLibrary)
        {
            _eventItemFactory = new PduEventItemUnsafeFactory();
        }

        //should look like the C function as much as possible.
        // ReSharper disable InconsistentNaming
        private unsafe PduError PDUGetEventItem(UNUM32 hMod, UNUM32 hCLL, PDU_EVENT_ITEM** pEventItem) => ((delegate* unmanaged[Stdcall]<UNUM32, UNUM32, PDU_EVENT_ITEM**, PduError>)AddressOfNativeMethod)(hMod, hCLL, pEventItem);
        private unsafe PduError PDUDestroyItem(PDU_ITEM* pItem) => ((delegate* unmanaged[Stdcall]<PDU_ITEM*, PduError>)AddressOfNativeMethodDestroyItem)(pItem);
        // ReSharper restore InconsistentNaming

    }
}
