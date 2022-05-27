using System;
// ReSharper disable IdentifierTypo
// ReSharper disable BuiltInTypeReferenceStyle

namespace ISO22900.II
{
    public enum PduError : UInt32   //ENUM are also treated as a 32-bit value under 64-bit OS, this is more or less a reminder
    {
        PDU_STATUS_NOERROR = 0x00000000,   /* No error for the function call */
        PDU_ERR_FCT_FAILED = 0x00000001,   /* Function call failed (generic failure) */
        PDU_ERR_RESERVED_1 = 0x00000010,   /* Reserved by ISO22900-2 */
        PDU_ERR_COMM_PC_TO_VCI_FAILED = 0x00000011,   /* Communication between host and MVCI Protocol Module failed */
        PDU_ERR_PDUAPI_NOT_CONSTRUCTED = 0x00000020,   /* The D-PDU API has not yet been constructed */
        PDU_ERR_SHARING_VIOLATION = 0x00000021,   /* A PDUDestruct was not called before another PDUConstruct */
        PDU_ERR_RESOURCE_BUSY = 0x00000030,   /* The requested resource is already in use */
        PDU_ERR_RESOURCE_TABLE_CHANGED = 0x00000031,   /* Not used by the D-PDU API */
        PDU_ERR_RESOURCE_ERROR = 0x00000032,   /* Not used by the D-PDU API */
        PDU_ERR_CLL_NOT_CONNECTED = 0x00000040,   /* The ComLogicalLinkLayer cannot be in the PDU_CLLST_OFFLINE state to perform the requested operation */
        PDU_ERR_CLL_NOT_STARTED = 0x00000041,   /* The ComLogicalLinkLayer must be in the PDU_CLLST_COMM_STARTED state to perform the requested operation */
        PDU_ERR_INVALID_PARAMETERS = 0x00000050,   /* One or more of the parameters supplied in the function are invalid */
        PDU_ERR_INVALID_HANDLE = 0x00000060,   /* One or more of the handles supplied in the function are invalid */
        PDU_ERR_VALUE_NOT_SUPPORTED = 0x00000061,   /* One of the option values in PDUConstruct is invalid */
        PDU_ERR_ID_NOT_SUPPORTED = 0x00000062,   /* IOCTL command id not supported by the implementation of the D-PDU API */
        PDU_ERR_COMPARAM_NOT_SUPPORTED = 0x00000063,   /* ComParam id not supported by the implementation of the D-PDU API */
        PDU_ERR_COMPARAM_LOCKED = 0x00000064,   /* Physical ComParam cannot be changed because it is locked by another ComLogicalLinkLayer */
        PDU_ERR_TX_QUEUE_FULL = 0x00000070,   /* The ComLogicalLinkLayer’s transmit queue is full; the ComPrimitiveLevel could not be queued */
        PDU_ERR_EVENT_QUEUE_EMPTY = 0x00000071,   /* No more event items are available to be read from the requested queue */
        PDU_ERR_VOLTAGE_NOT_SUPPORTED = 0x00000080,   /* The voltage value supplied in the IOCTL call is not supported by the MVCI Protocol Module */
        PDU_ERR_MUX_RSC_NOT_SUPPORTED = 0x00000081,   /* The specified pin / resource are not supported by the MVCI Protocol Module for the IOCTL call */
        PDU_ERR_CABLE_UNKNOWN = 0x00000082,   /* The cable attached to the MVCI Protocol Module is of an unknown type */
        PDU_ERR_NO_CABLE_DETECTED = 0x00000083,   /* No cable is detected by the MVCI Protocol Module */
        PDU_ERR_CLL_CONNECTED = 0x00000084,   /* The ComLogicalLinkLayer is already in the PDU_CLLST_ONLINE state */
        PDU_ERR_TEMPPARAM_NOT_ALLOWED = 0x00000090,   /* Physical ComParams cannot be changed as a temporary ComParam */
        PDU_ERR_RSC_LOCKED = 0x000000A0,   /* The resource is already locked */
        PDU_ERR_RSC_LOCKED_BY_OTHER_CLL = 0x000000A1,   /* The ComLogicalLinkLayer's resource is currently locked by another ComLogicalLinkLayer */
        PDU_ERR_RSC_NOT_LOCKED = 0x000000A2,   /* The resource is already in the unlocked state */
        PDU_ERR_MODULE_NOT_CONNECTED = 0x000000A3,   /* The module is not in the PDU_MODST_READY state */
        PDU_ERR_API_SW_OUT_OF_DATE = 0x000000A4,   /* The API software is older than the MVCI Protocol Module Software */
        PDU_ERR_MODULE_FW_OUT_OF_DATE = 0x000000A5,   /* The MVCI Protocol Module software is older than the API software */
        PDU_ERR_PIN_NOT_CONNECTED = 0x000000A6,   /* The requested Pin is not routed by supported cable */
        PDU_ERR_IP_PROTOCOL_NOT_SUPPORTED = 0x000000B0,  /* IP protocol is not supported: e.g. IPv6 used as protocolVersion, but OS doesn't support IPv6 (or it is disabled).*/
        PDU_ERR_DOIP_ROUTING_ACTIVATION_FAILED = 0x000000B1,  /* DoIP Routing activation failed */
        PDU_ERR_DOIP_ROUTING_ACTIVATION_AUTHENTICATION_FAILED = 0x000000B2,  /* DoIP Routing activation denied due to missing authentication */
        PDU_ERR_DOIP_AMBIGUOUS_LOGICAL_ADDRESS = 0x000000B3,   /* Denied to connect a DoIP LogicalLink with a logical address which is identical for multiple DoIP entities inside a DoIP MVCI module representing a collection of DoIP entities */
        PDU_ERR_DOIP_ROUTING_ACTIVATION_INVALID_SOURCE_ADDRESS = 0x000000B4, /* Routing activation denied due to response code 0x00 indicating an unknown or invalid source address */
        PDU_ERR_DOIP_ROUTING_ACTIVATION_NO_DATA_SOCKET_AVAILABLE = 0x000000B5, /* Routing activation denied due to response code 0x01 indicating too many registered and active TCP clients */
        PDU_ERR_DOIP_ROUTING_ACTIVATION_SOURCE_ADDRESS_CHANGED = 0x000000B6, /* Routing activation denied due to response code 0x02 indicating that a different source address has already been activated before on this TCP connection */
        PDU_ERR_DOIP_ROUTING_ACTIVATION_SOURCE_ADDRESS_IN_USE = 0x000000B7, /* Routing activation denied due to response code 0x03 indicating that the same source address is already registered and active on a different TCP connection */
        PDU_ERR_DOIP_ROUTING_ACTIVATION_CONFIRMATION_REJECTED = 0x000000B8, /* Routing activation denied due to response code 0x05 indicating a rejected confirmation */
        PDU_ERR_DOIP_ROUTING_ACTIVATION_TYPE_UNSUPPORTED = 0x000000B9, /* Routing activation denied due to response code 0x06 indicating an unsupported routing activation type */
        PDU_ERR_DOIP_ROUTING_ACTIVATION_RESPONSE_CODE_UNKNOWN = 0x000000BA, /* Routing activation denied due to an unknown (i.e.  manufacturer specific) routing activation response code */
        PDU_ERR_DOIP_ROUTING_ACTIVATION_RESPONSE_TIMEOUT = 0x000000BB, /* Time out while waiting for routing activation response */
        PDU_ERR_DOIP_RESPONSE_TIMEOUT = 0x000000BC, /* DoIPCtrlTimeout has occurred */
        PDU_ERR_TLS_REQUIRED = 0x000000BD, /* The DoIP entity only accepts secured connections */
        PDU_ERR_TLS_CONF_INCOMPLETE = 0x000000BE, /* Missing mandatory TLS configuration, e.g. like TLS certificate */
        PDU_ERR_TLS_VERSION_AGREEMENT_FAILED = 0x000000BF, /* Specified TLS version could not be agreed upon */
        PDU_ERR_TLS_CIPHER_AGREEMENT_FAILED = 0x000000C0, /* Specified TLS cipher suite could not be agreed upon */
        PDU_ERR_TLS_CERT_VERIFY_FAILED = 0x000000C1, /* TLS certificate verification failed (e.g. may not be valid signed certificate, expired, …) */
        PDU_ERR_TLS_VERSION_NOT_SUPPORTED = 0x000000C2, /* Unsupported TLS version specified */
        PDU_ERR_TLS_CIPHER_NOT_SUPPORTED = 0x000000C3, /* Unsupported TLS cipher suite specified */
    }
}