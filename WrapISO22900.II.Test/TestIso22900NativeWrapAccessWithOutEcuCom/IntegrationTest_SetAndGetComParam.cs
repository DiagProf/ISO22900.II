using ISO22900.II;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    public partial class IntegrationTest_SetupAndTearDown
    {
        [Test]
        public void TestPduSetAndGetComParamOfTypeUint()
        {
            const uint testValue = 0x010255AA;
            var cpP2MaxId = _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, "CP_P2Max");

            //Without MDF file you don't know the ComParamClass. The only chance is to read(get) the ComParam first
            //There are some VCIs so it is no problem to send any ComParamClass, but you cannot rely on it.
            var cpP2Max = (PduComParamOfTypeUint) _dPduApi.PduGetComParam(_moduleOne, _cll, cpP2MaxId);
            var cpClass = cpP2Max.ComParamClass;

            //Write the test value
            var cpP2MaxNew = new PduComParamOfTypeUint(cpP2MaxId, cpClass, testValue);
            _dPduApi.PduSetComParam(_moduleOne, _cll, cpP2MaxNew);

            //Read it back
            var cpP2MaxNewBack = (PduComParamOfTypeUint) _dPduApi.PduGetComParam(_moduleOne, _cll, cpP2MaxId);

            Assert.AreEqual(testValue, cpP2MaxNewBack.ComParamData);
        }

        [Test]
        public void TestPduSetAndGetComParamOfTypeByteField()
        {
            var testValue = new byte[] {0x3e, 0x47, 0x11};
            var cpTesterPresentMessageId =
                _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, "CP_TesterPresentMessage");

            //Without MDF file you don't know the ComParamClass. The only chance is to read(get) the ComParam first
            //There are some VCIs so it is no problem to send any ComParamClass, but you cannot rely on it.

            //For all ComParam's of the type field you actually cannot know the ParamMaxLen without an MDF file.
            //The only safe workaround is to read(get) the current field first and use this ParamMaxLen.
            var cpTesterPresentMessage =
                (PduComParamOfTypeByteField) _dPduApi.PduGetComParam(_moduleOne, _cll, cpTesterPresentMessageId);
            var cpClass = cpTesterPresentMessage.ComParamClass;
            var maxLen = cpTesterPresentMessage.ComParamData.ParamMaxLen;
            if (maxLen > testValue.Length)
            {
                var newField = new PduParamByteFieldData(testValue, maxLen);

                //Write the test value
                var cpTesterPresentMessageNew =
                    new PduComParamOfTypeByteField(cpTesterPresentMessageId, cpClass, newField);
                _dPduApi.PduSetComParam(_moduleOne, _cll, cpTesterPresentMessageNew);

                //Read it back
                var cpTesterPresentMessageNewBack =
                    (PduComParamOfTypeByteField) _dPduApi.PduGetComParam(_moduleOne, _cll, cpTesterPresentMessageId);

                Assert.AreEqual(testValue, cpTesterPresentMessageNewBack.ComParamData.DataArray);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TestPduSetAndGetComParamOfTypeLongField()
        {
            var testValue = new uint[] {1000000, 250000, 125000};
            var cpCanBaudrateRecordId = _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, "CP_CanBaudrateRecord");

            //Without MDF file you don't know the ComParamClass. The only chance is to read(get) the ComParam first
            //There are some VCIs so it is no problem to send any ComParamClass, but you cannot rely on it.

            //For all ComParam's of the type field you actually cannot know the ParamMaxLen without an MDF file.
            //The only safe workaround is to read(get) the current field first and use this ParamMaxLen.
            var cpCanBaudrateRecord =
                (PduComParamOfTypeUintField) _dPduApi.PduGetComParam(_moduleOne, _cll, cpCanBaudrateRecordId);
            var cpClass = cpCanBaudrateRecord.ComParamClass;
            var maxLen = cpCanBaudrateRecord.ComParamData.ParamMaxLen;
            if (maxLen > testValue.Length) //check that the number is not over the maximum allowed
            {

                //Write the test value
                var newField = new PduParamUintFieldData(testValue, maxLen);
                var cpCanBaudrateRecordNew =
                    new PduComParamOfTypeUintField(cpCanBaudrateRecordId, cpClass, newField);
                _dPduApi.PduSetComParam(_moduleOne, _cll, cpCanBaudrateRecordNew);

                //Read it back
                var cpCanBaudrateRecordNewBack =
                    (PduComParamOfTypeUintField) _dPduApi.PduGetComParam(_moduleOne, _cll, cpCanBaudrateRecordId);

                Assert.AreEqual(testValue, cpCanBaudrateRecordNewBack.ComParamData.DataArray);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TestPduSetAndGetComParamOfTypeStructField()
        {
            var testValue = new[]
            {
                new PduParamStructSessionTiming(0x03, 60, 25, 250, 50),
                new PduParamStructSessionTiming(0x02, 65, 20, 255, 55)
            };
            var cpSessionTimingOverrideId = _dPduApi.PduGetObjectId(PduObjt.PDU_OBJT_COMPARAM, "CP_SessionTimingOverride");

            //Without MDF file you don't know the ComParamClass. The only chance is to read(get) the ComParam first
            //There are some VCIs so it is no problem to send any ComParamClass, but you cannot rely on it.

            //For all ComParam's of the type field you actually cannot know the ParamMaxLen without an MDF file.
            //The only safe workaround is to read(get) the current field first and use this ParamMaxLen.
            var cpSessionTimingOverride =
                (PduComParamOfTypeStructField) _dPduApi.PduGetComParam(_moduleOne, _cll, cpSessionTimingOverrideId);
            var cpClass = cpSessionTimingOverride.ComParamClass;
            var maxLen = cpSessionTimingOverride.ComParamData.ParamMaxEntries;
            if (maxLen > testValue.Length)
            {
                //Write the test value
                var newField = new PduParamStructFieldData(PduCpSt.PDU_CPST_SESSION_TIMING, testValue, maxLen);
                var cpSessionTimingOverrideNew =
                    new PduComParamOfTypeStructField(cpSessionTimingOverrideId, cpClass, newField);
                _dPduApi.PduSetComParam(_moduleOne, _cll, cpSessionTimingOverrideNew);

                //Read it back
                var cpSessionTimingOverrideNewBack =
                    (PduComParamOfTypeStructField) _dPduApi.PduGetComParam(_moduleOne, _cll, cpSessionTimingOverrideId);

                var result = cpSessionTimingOverrideNewBack.ComParamData.StructArray as PduParamStructSessionTiming[];
                Assert.AreEqual(testValue.Length, result.Length);
                for (var index = 0; index < testValue.Length; index++)
                {
                    Assert.AreEqual(testValue[index].Session, result[index].Session);
                    Assert.AreEqual(testValue[index].P2MaxHigh, result[index].P2MaxHigh);
                    Assert.AreEqual(testValue[index].P2MaxLow, result[index].P2MaxLow);
                    Assert.AreEqual(testValue[index].P2StarHigh, result[index].P2StarHigh);
                    Assert.AreEqual(testValue[index].P2StarLow, result[index].P2StarLow);
                }
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}