# ISO22900.II-Sharp

ISO22900.II-Sharp handles all the details of operating with unmanaged ISO 22900-2 spec library (also called D-PDU-API) and lets you deal with the important stuff.

The [ISO 22900-2](https://www.iso.org/standard/62490.html) friendly name is D-PDU-API both are synonymous for a software interface description. D-PDU-API can live alone but was normally designed in use with ISO 22900-X and ISO 22901-X in mind. All this spec libraries has the goal to make automotive diagnostic data interchangeable. In case of ISO 22900-2 this correctly means modular vehicle communication interface (MVCI or VCI) can be exchanged through MVCI from another manufacturer if both support ISO 22900-2.

Extract from 22900-2: *"The purpose of ISO 22900-2 is to ensure that diagnostic and reprogramming applications from any vehicle or tool manufacturer can operate on a common software interface and can easily exchange MVCI protocol module implementations."*

## A bit of history with philosophy

Some time ago every diagnostic tool used its own proprietary VCI (many small tools still do that today). That made it impossible to replace the VCI with another (other manufacturer, etc.). The end users then (typically the workshop) has the problem that they need 3 different VCIs for 3 different diagnostic tools. The OEMs had also a problem with proprietary VCI interfaces and the result worse with every new generation of diagnostic tool they deliver a new VCI. Of course in general every new VCI gets better (faster, wireless connections, new protocols etc.) but the old VCI was unsueable because the interface definition to the VCI was broken. 

The problem has already been recognized and interface specifications such as SAE J2534(PassThru) and RP1210a were created. PassThru capable VCIs are heavily used for ECU reprogramming therefore they are more widely used in America than e.g. in Europe. European vehicle manufacturers don't like to give out their ecu flash data :-(  especially for independent workshops (but for America they have to do it). RP1210a is used in the heavy-duty vehicle field. The RP1210 API is created for various flavors of Microsoft Windows as OS. Which limits the use very much.

From my point of view, the biggest advantages of the D-PDU-API are:

- asynchronous ECU communication

- can handle negative ECU responses internally

- the parameters for the vehicle protocols are also part of the specification (at least for the widely used protocols)

- the parameters and the vehicle protocols are named **completely**, **uniformly** and the **effectiveness** of the parameter is determined

## TODO's

- write more...

## Usage

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using ISO22900.II;

namespace ISO22900.II.Example
{
public class Program
{
public static async Task Main(string[] args)
{
    //The helper functions read registry and root xml
    var allInstalledPduApis = DiagPduApiHelper.InstalledMvciPduApiDetails().ToList();
    if ( allInstalledPduApis.Any() )
    {
        //take the first API
        var apiShortName = allInstalledPduApis.First().ShortName;
        using ( var api = DiagPduApiOneFactory.GetApi(DiagPduApiHelper
                   .FullyQualifiedLibraryFileNameFormShortName(apiShortName)) )
        {
            //without parameters means the first VCI that is found
            using ( var vci = api.ConnectVci() )
            {
                //Define the protocol behavior
                //These names (the strings) come from ODX or ISO 22900-2
                var dlcPinData = new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } };
                var busTypeName = "ISO_11898_2_DWCAN";
                var protocolName = "ISO_15765_3_on_ISO_15765_2";

                using ( var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList()) )
                {
                    //Set UniqueId ComParam's
                    uint pageOneId = 815; //give page one a ID  
                    link.SetUniqueRespIdTablePageOneUniqueRespIdentifier(pageOneId);
                    link.SetUniqueIdComParamValue(pageOneId, "CP_CanPhysReqId", 0x7E0);
                    link.SetUniqueIdComParamValue(pageOneId, "CP_CanRespUSDTId", 0x7E8);

                    //Set normal ComParam's
                    link.SetComParamValueViaGet("CP_P2Max", 500000);

                    link.Connect();

                    var request = new byte[] { 0x22, 0xF1, 0x90 };

                    using ( var cop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, 1, request) )
                    {
                        var result = await cop.WaitForCopResultAsync();

                        var responseString = string.Empty;
                        uint responseTime = 0;
                        if ( result.DataMsgQueue().Count > 0 )
                        {
                            responseString = string.Join(",", 
                                result.DataMsgQueue().ConvertAll(bytes => { return BitConverter.ToString(bytes); }));
                            responseTime = result.ResponseTime();
                        }
                        else if ( result.PduEventItemErrors().Count > 0 )
                        {
                            foreach ( var error in result.PduEventItemErrors() )
                            {
                                responseString += $"{error.ErrorCodeId}" + $" ({error.ExtraErrorInfoId})";
                            }

                            responseString = "Error: " + responseString;
                        }
                        Console.WriteLine($"{BitConverter.ToString(request)} | {responseString}  | {responseTime}µs");
                    }
                    link.Disconnect();
                }
            }
        }
    }
    Console.ReadKey();
}
}
}
```
