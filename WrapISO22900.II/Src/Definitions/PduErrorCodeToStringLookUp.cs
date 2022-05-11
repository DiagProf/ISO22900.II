using System.Collections.Generic;

namespace ISO22900.II
{
    public static class PduErrorCodeToStringLookUp
    {
        public static Dictionary<PduError, string> PduErrorToString { get; } = new Dictionary<PduError, string> 
        {
            [PduError.PDU_STATUS_NOERROR] = "No error for the function call",
            [PduError.PDU_ERR_FCT_FAILED] = "Function call failed (generic failure)",
            [PduError.PDU_ERR_RESERVED_1] = "Reserved by ISO22900-2",
            [PduError.PDU_ERR_COMM_PC_TO_VCI_FAILED] = "Communication between host and MVCI Protocol Module failed",
            [PduError.PDU_ERR_PDUAPI_NOT_CONSTRUCTED] = "The D-PDU API has not yet been constructed",
            [PduError.PDU_ERR_SHARING_VIOLATION] = "A PDUestruct was not called before another PDUConstruct",
            [PduError.PDU_ERR_RESOURCE_BUSY] = "The requested resource is already in use",
            [PduError.PDU_ERR_RESOURCE_TABLE_CHANGED] = "Not used by the D-PDU API",
            [PduError.PDU_ERR_RESOURCE_ERROR] = "Not used by the D-PDU API",
            [PduError.PDU_ERR_CLL_NOT_CONNECTED] = "The ComLogicalLinkLayer cannot be in the PDU_CLLST_OFFLINE state to perform the requested operation",
            [PduError.PDU_ERR_CLL_NOT_STARTED] = "The ComLogicalLinkLayer must be in the PDU_CLLST_COMM_STARTED state to perform the requested operation",
            [PduError.PDU_ERR_INVALID_PARAMETERS] = "One or more of the parameters supplied in the function are invalid",
            [PduError.PDU_ERR_INVALID_HANDLE] = "One or more of the handles supplied in the function are invalid",
            [PduError.PDU_ERR_VALUE_NOT_SUPPORTED] = "One of the option values in PDUConstruct is invalid",
            [PduError.PDU_ERR_ID_NOT_SUPPORTED] = "IOCTL command id not supported by the implementation of the D-PDU API",
            [PduError.PDU_ERR_COMPARAM_NOT_SUPPORTED] = "ComParam id not supported by the implementation of the D-PDU API",
            [PduError.PDU_ERR_COMPARAM_LOCKED] = "Physical ComParam cannot be changed because it is locked by another ComLogicalLinkLayer",
            [PduError.PDU_ERR_TX_QUEUE_FULL] = "The ComLogicalLinkLayer’s transmit queue is full; the ComPrimitiveLevel could not be queued",
            [PduError.PDU_ERR_EVENT_QUEUE_EMPTY] = "No more event items are available to be read from the requested queue",
            [PduError.PDU_ERR_VOLTAGE_NOT_SUPPORTED] = "The voltage value supplied in the IOCTL call is not supported by the MVCI Protocol Module",
            [PduError.PDU_ERR_MUX_RSC_NOT_SUPPORTED] = "The specified pin / resource are not supported by the MVCI Protocol Module for the IOCTL call",
            [PduError.PDU_ERR_CABLE_UNKNOWN] = "The cable attached to the MVCI Protocol Module is of an unknown type",
            [PduError.PDU_ERR_NO_CABLE_DETECTED] = "No cable is detected by the MVCI Protocol Module",
            [PduError.PDU_ERR_CLL_CONNECTED] = "The ComLogicalLinkLayer is already in the PDU_CLLST_ONLINE state",
            [PduError.PDU_ERR_TEMPPARAM_NOT_ALLOWED] = "Physical ComParams cannot be changed as a temporary ComParam",
            [PduError.PDU_ERR_RSC_LOCKED] = "The resource is already locked",
            [PduError.PDU_ERR_RSC_LOCKED_BY_OTHER_CLL] = "The ComLogicalLinkLayer's resource is currently locked by another ComLogicalLinkLayer",
            [PduError.PDU_ERR_RSC_NOT_LOCKED] = "The resource is already in the unlocked state",
            [PduError.PDU_ERR_MODULE_NOT_CONNECTED] = "The module is not in the PDU_MODST_READY state",
            [PduError.PDU_ERR_API_SW_OUT_OF_DATE] = "The API software is older than the MVCI Protocol Module Software",
            [PduError.PDU_ERR_MODULE_FW_OUT_OF_DATE] = "The MVCI Protocol Module software is older than the API software",
            [PduError.PDU_ERR_PIN_NOT_CONNECTED] = "The requested Pin is not routed by supported cable",
            [PduError.PDU_ERR_IP_PROTOCOL_NOT_SUPPORTED] = "IP protocol is not supported: e.g. IPv6 used as protocolVersion, but OS doesn't support IPv6 (or it is disabled)",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_FAILED] = "DoIP Routing activation failed",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_AUTHENTICATION_FAILED] = "DoIP Routing activation denied due to missing authentication",
            [PduError.PDU_ERR_DOIP_AMBIGUOUS_LOGICAL_ADDRESS] = "Denied to connect a DoIP LogicalLink with a logical address which is identical for multiple DoIP entities inside a DoIP MVCI module representing a collection of DoIP entities",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_INVALID_SOURCE_ADDRESS] = "Routing activation denied due to response code 0x00 indicating an unknown or invalid source address",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_NO_DATA_SOCKET_AVAILABLE] = "Routing activation denied due to response code 0x01 indicating too many registered and active TCP clients",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_SOURCE_ADDRESS_CHANGED] = " Routing activation denied due to response code 0x02 indicating that a different source address has already been activated before on this TCP connection",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_SOURCE_ADDRESS_IN_USE] = "Routing activation denied due to response code 0x03 indicating that the same source address is already registered and active on a different TCP connection",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_CONFIRMATION_REJECTED] = "Routing activation denied due to response code 0x05 indicating a rejected confirmation",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_TYPE_UNSUPPORTED] = "Routing activation denied due to response code 0x06 indicating an unsupported routing activation type",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_RESPONSE_CODE_UNKNOWN] = "Routing activation denied due to an unknown (i.e.  manufacturer specific) routing activation response code",
            [PduError.PDU_ERR_DOIP_ROUTING_ACTIVATION_RESPONSE_TIMEOUT] = "Time out while waiting for routing activation response",
            [PduError.PDU_ERR_DOIP_RESPONSE_TIMEOUT] = "DoIPCtrlTimeout has occurred",
            [PduError.PDU_ERR_TLS_REQUIRED] = "The DoIP entity only accepts secured connections",
            [PduError.PDU_ERR_TLS_CONF_INCOMPLETE] = "Missing mandatory TLS configuration, e.g. like TLS certificate",
            [PduError.PDU_ERR_TLS_VERSION_AGREEMENT_FAILED] = "Specified TLS version could not be agreed upon",
            [PduError.PDU_ERR_TLS_CIPHER_AGREEMENT_FAILED] = "Specified TLS cipher suite could not be agreed upon",
            [PduError.PDU_ERR_TLS_CERT_VERIFY_FAILED] = "TLS certificate verification failed (e.g. may not be valid signed certificate, expired, …)",
            [PduError.PDU_ERR_TLS_VERSION_NOT_SUPPORTED] = "Unsupported TLS version specified",
            [PduError.PDU_ERR_TLS_CIPHER_NOT_SUPPORTED] = "Unsupported TLS cipher suite specified",

        };
    }
}