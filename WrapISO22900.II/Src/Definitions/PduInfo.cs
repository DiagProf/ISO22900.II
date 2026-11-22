namespace ISO22900.II
{
    // Information event values
    public enum PduInfo
    {
        PDU_INFO_MODULE_LIST_CHG = 0x8070, /* New MVCI Protocol Module list is available.  Client application should call PDUGetModuleIds  to get a list of the new set of modules and status.   Related to the System Callback */
       
        PDU_INFO_RSC_LOCK_CHG = 0x8071, /* There has been a change in the lock status on a shared physical resource. */

        /* Call PDUGetResourceStatus to get a description of the new lock status. */
        /* Only applicable to a resource shared by multiple ComLogicalLinks. */
        /* Related to the ComLogicalLinkLayer Callback. */
        PDU_INFO_PHYS_COMPARAM_CHG = 0x8072 /* There has been a change to the physical ComParams by another */
        /* ComLogicalLinkLayer sharing the resource.                         */
        /* Related to the ComLogicalLinkLayer Callback.                      */
    }
    
}