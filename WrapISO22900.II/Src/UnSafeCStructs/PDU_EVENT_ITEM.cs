﻿using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable IdentifierTypo

namespace ISO22900.II.UnSafeCStructs
{
    // Basic data types used in ISO22900-2
    // C# using alias directive     //C typedef...
    using UNUM8 = System.Byte;      //typedef unsigned char UNUM8;      // Unsigned numeric 8 bits
    //using SNUM8 = System.SByte;     //typedef signed char SNUM8;        // Signed numeric 8 bits
    //using UNUM16 = System.UInt16;   //typedef unsigned short UNUM16;    // Unsigned numeric 16 bits
    //using SNUM16 = System.Int16;    //typedef signed short SNUM16;      // Signed numeric 16 bits
    using UNUM32 = System.UInt32;   //typedef unsigned long UNUM32;     // Unsigned numeric 32 bits
    //using SNUM32 = System.Int32;    //typedef signed long SNUM32;       // Signed numeric 32 bits

    //typedef char CHAR8;               // ASCII-coded 8-bit character value (ISO8859-1 (Latin 1))


    /* Status code information. See section D.1.4 to D.1.6*/
    /* All native structures in this file are only used in the context of structure PDU_EVENT_ITEM. Therefore no separate file */

    // Item for event notification
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_EVENT_ITEM
    {
        internal PduIt ItemType;                  /* value= PDU_IT_RESULT or PDU_IT_STATUS or PDU_IT_ERROR or PDU_IT_INFO*/
        internal UNUM32 hCop;                        /* If item is from a ComPrimitiveLevel then the hCop contains the valid ComPrimitiveLevel handle, else it contains PDU_HANDLE_UNDEF */
        internal void* pCoPTag;                      /* ComPrimitiveLevel Tag. Should be ignored if hCop = PDU_HANDLE_UNDEF*/
        internal  UNUM32 Timestamp;                   /* Timestamp in microseconds*/
        internal void* pData;                        /* points to the data for the specified Item Type. See section 11.1.4.11.1 to 11.1.4.11.4 */
    }

    // Asynchronous result notification structure (received data) for the PDU_IT_RESULT Item
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_RESULT_DATA
    {
        internal readonly PDU_FLAG_DATA RxFlag;           /* Receive message status. See RxFlag definition.*/

        internal readonly UNUM32 UniqueRespIdentifier;    /* ECU response unique identifier */

        internal readonly UNUM32 AcceptanceId;            /* Acceptance Id value from ComPrimitiveLevel Expected Response Structure. */
        /* If multiple expected response entries match the response payload data, */
        /* then the first matching expected response id found in the array of expected */
        /* responses is used. (I.e. acceptance filtering is carried out in the sequence */
        /* of the expected responses as they appear in the array of expected responses. */
        /* Thus, an expected response with the lowest array index has the highest priority.)*/

        internal readonly PDU_FLAG_DATA TimestampFlags;   /* Bitoriented Timestamp Indicator flag (See sections Structure for flag data and TimestampFlag definition).  If the flag data is 0, then the following timestamp information is not valid.*/

        internal readonly UNUM32 TxMsgDoneTimestamp;      /* Transmit Message done Timestamp in microseconds*/

        internal readonly UNUM32 StartMsgTimestamp;       /* Start Message Timestamp in microseconds*/

        internal readonly PDU_EXTRA_INFO* pExtraInfo;     /* If NULL, no extra information is attached to the response structure. */
        /* This feature is enabled by setting the ENABLE_EXTRA_INFO bit in the TxFlag for */
        /* the ComPrimitiveLevel (See TxFlag definition section)*/

        internal readonly UNUM32 NumDataBytes;            /* Data size in bytes, if RawMode then the data includes header bytes, checksum, */
        /* message data bytes (pDataBytes), and extra data, if any.*/

        internal readonly UNUM8* pDataBytes;              /* Reference pointer to D-PDU API memory that contains PDU Payload data. */
        /* In non-Raw mode this data contains no header bytes, CAN Ids, or checksum information. */
        /* In RawMode, this data will contain the exact data received from the ECU. */
        /* For ISO 15765, ISO11898 and SAE J1939, the first 4 bytes are the */
        /* CAN ID (11 bit or 29 bit) followed by a possible extended address byte (See Table D.14 - TxFlag ) */
    }

    // Structure for extra result data information  (Part of PDU_RESULT_DATA)
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PDU_EXTRA_INFO
    {
        internal UNUM32 NumHeaderBytes;              /* Number of header bytes contained in pHeaderBytes array. */
        internal UNUM32 NumFooterBytes;              /* Number of footer bytes contained in pFooterBytes array. (SAE J1850 PWM) Start position of extra data in received message (for example, IFR) or ISO14230 checksum.) When no extra data bytes are present in the message, NumFooterBytes shall be set to zero. */
        internal UNUM8* pHeaderBytes;                /* Reference pointer to Response PDU Header bytes, NULL if NumHeaderBytes = 0 */
        internal UNUM8* pFooterBytes;                /* Reference pointer to Response PDU Footer bytes, NULL if NumFooterBytes = 0 */
    }


    // Asynchronous event information notification for PDU_IT_INFO Item.
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_INFO_DATA
    {
        internal PduInfo InfoCode;   /* Information code. See Table D.7 - Information event values */
        internal UNUM32 ExtraInfoData;  /* Optional additional information */
    }


    // Asynchronous error notification structure for the PDU_IT_ERROR Item.
    [StructLayout(LayoutKind.Sequential)]
    internal struct PDU_ERROR_DATA
    {
        internal PduErrEvt ErrorCodeId; /* error code, binary information. */

        internal UNUM32 ExtraErrorInfoId; /* Optional additional error information, text translation via MDF file. Binary Information, 0 indicates no additional error information See section Table D.18 - Event error and examples for additional error codes */
    }
}