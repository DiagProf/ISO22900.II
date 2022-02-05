using System.Linq;
using ISO22900.II;
using NUnit.Framework;

namespace ISO22900.II.Test
{
    public partial class IntegrationTest_SetupAndTearDown
    {
        [Test]
        public void TestPduSetAndGetPduGetUniqueRespIdTableOneEntry()
        {
            var testValue = _dPduApi.PduGetUniqueRespIdTable(_moduleOne, _cll);
            _dPduApi.PduSetUniqueRespIdTable(_moduleOne, _cll, testValue);
            var result = _dPduApi.PduGetUniqueRespIdTable(_moduleOne, _cll);

            Assert.AreEqual(testValue.Count, result.Count);
            for (var index = 0; index < testValue.Count; index++)
            {
                Assert.AreEqual(testValue[index].UniqueRespIdentifier, result[index].UniqueRespIdentifier);
                Assert.AreEqual(testValue[index].ComParams.Count, result[index].ComParams.Count);
                for (var i = 0; i < testValue[index].ComParams.Count; i++)
                {
                    Assert.AreEqual(testValue[index].ComParams[i].ComParamId,
                        result[index].ComParams[i].ComParamId);
                    Assert.AreEqual(testValue[index].ComParams[i].ComParamDataType,
                        result[index].ComParams[i].ComParamDataType);
                    Assert.AreEqual(testValue[index].ComParams[i].ComParamClass,
                        result[index].ComParams[i].ComParamClass);
                    if (testValue[index].ComParams[i].ComParamDataType == PduPt.PDU_PT_UNUM32)
                        Assert.AreEqual((testValue[index].ComParams[i] as PduComParamOfTypeUint)?.ComParamData,
                            (result[index].ComParams[i] as PduComParamOfTypeUint)?.ComParamData);
                }
            }
        }

        [Test]
        public void TestPduSetAndGetPduGetUniqueRespIdTableTwoEntries()
        {
            var testValue = _dPduApi.PduGetUniqueRespIdTable(_moduleOne, _cll);

            testValue.First().UniqueRespIdentifier = 4711;
            var shallowCopyIdOnly = new PduEcuUniqueRespData(0815, testValue.First().ComParams);
            testValue.Add(shallowCopyIdOnly);

            _dPduApi.PduSetUniqueRespIdTable(_moduleOne, _cll, testValue);
            var result = _dPduApi.PduGetUniqueRespIdTable(_moduleOne, _cll);

            Assert.AreEqual(testValue.Count, result.Count);
            for (var index = 0; index < testValue.Count; index++)
            {
                Assert.AreEqual(testValue[index].UniqueRespIdentifier, result[index].UniqueRespIdentifier);
                Assert.AreEqual(testValue[index].ComParams.Count, result[index].ComParams.Count);
                for (var i = 0; i < testValue[index].ComParams.Count; i++)
                {
                    Assert.AreEqual(testValue[index].ComParams[i].ComParamId, result[index].ComParams[i].ComParamId);
                    Assert.AreEqual(testValue[index].ComParams[i].ComParamDataType,
                        result[index].ComParams[i].ComParamDataType);
                    Assert.AreEqual(testValue[index].ComParams[i].ComParamClass,
                        result[index].ComParams[i].ComParamClass);
                    if (testValue[index].ComParams[i].ComParamDataType == PduPt.PDU_PT_UNUM32)
                        Assert.AreEqual((testValue[index].ComParams[i] as PduComParamOfTypeUint)?.ComParamData,
                            (result[index].ComParams[i] as PduComParamOfTypeUint)?.ComParamData);
                }
            }
        }
    }
}