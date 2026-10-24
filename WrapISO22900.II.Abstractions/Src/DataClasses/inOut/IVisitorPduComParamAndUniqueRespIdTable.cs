namespace ISO22900.II
{
    internal interface IVisitorPduComParamAndUniqueRespIdTable
    {
        void VisitConcretePduComParamOfTypeByte(PduComParamOfTypeByte cp);
        
        void VisitConcretePduComParamOfTypeByteField(PduComParamOfTypeByteField cp);
        
        void VisitConcretePduComParamOfTypeInt(PduComParamOfTypeInt cp);
        
        void VisitConcretePduComParamOfTypeUintField(PduComParamOfTypeUintField cp);
        
        void VisitConcretePduComParamOfTypeSbyte(PduComParamOfTypeSbyte cp);
        
        void VisitConcretePduComParamOfTypeShort(PduComParamOfTypeShort cp);
        
        void VisitConcretePduComParamOfTypeStructField(PduComParamOfTypeStructField cp);
        
        void VisitConcretePduComParamOfTypeUint(PduComParamOfTypeUint cp);
        
        void VisitConcretePduComParamOfTypeUshort(PduComParamOfTypeUshort cp);

        //Param data types which have no equal native type
        void VisitConcretePduParamByteFieldData(PduParamByteFieldData pd);

        void VisitConcretePduParamUintFieldData(PduParamUintFieldData pd);
        
        void VisitConcretePduParamStructFieldData(PduParamStructFieldData pd);

        //Param data types which are a sub of PduParamStructFieldData
        void VisitConcretePduParamStructSessionTiming(PduParamStructSessionTiming pd);

        void VisitConcretePduParamStructAccessTiming(PduParamStructAccessTiming pd);

        //tis 
        void VisitConcretePduUniqueRespIdTable(PduUniqueRespIdTable pduUniqueRespIdTable);

        void VisitConcretePduEcuUniqueRespData(PduEcuUniqueRespData pduEcuUniqueRespData);
    }
}