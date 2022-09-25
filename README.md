# `ISO22900.II-Sharp`

_[![ISO22900.II-Sharp NuGet Version](https://img.shields.io/nuget/v/ISO22900.II-Sharp.svg?style=flat&label=NuGet%3A%20ISO22900.II-Sharp)](https://www.nuget.org/packages/ISO22900.II-Sharp)_

ISO22900.II-Sharp handles all the details of operating with unmanaged ISO 22900-2 spec library (also called D-PDU-API) and lets you deal with the important stuff.

## Table of Contents

1. [Introduction](#introduction)
2. [A bit of history with philosophy](#a bit of history with philosophy)
3. [TODO's](#tODO's)
4. [Usage](#usage)
5. [FAQ](#faq)

## Introduction

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

If you look at the around 29 functions from ISO22900-2 it is initially difficult to see how a user-friendly API can be built from them. I think the idea of one API fits all and is easy to use is not so straightforward. For example it would be possible to build an API with the 29 functions that looks and works like the J2534 API or to build an API that fits well under a diagnostic-server but can also be used on its own. The latter is what I'm doing here. What becomes particularly visible in the project when the name "ApiOne" appears in the names of files and folders. If I wanted to build an API out of the 29 functions that looks like a J2534 API, I might name it Api2534. In order to understand this even more clearly, I would like to briefly explain how I split ISO22900-2 in order to transfer it from a C API to the object world.

1. If you look at the C functions, almost all of them only have one return value, which is passed to the function as a call by reference. The real return value is used for error handling. In C# the return value now throws an exception and the old call by reference value is now the return value of the function. (For a few functions that have more than one return value, the values would be packed together and returned in a class. At the moment, this is only the case with PduExStatusData. The Ex stands for Extension and in this context means that it is a small extension and this element is not exactly found in this way in ISO22900-2.)
2. For each function from ISO22900-2 there is a factory which is derived from ApiCallFactory. The reason for the factory is I want to porb some things later. Currently, I'm a big fan of marshaling across the stack and in unsafe code. That seems very fast. Later, however, more safe code is to be added and perhaps marshaling via the heap.
3. All products are derived from ApiCall and all those that have to release memory with PDUDestroyItem from ApiCallPduDestroyItem.
4. Iso22900NativeWrapAccess as the name says is now a wrapper over the function of ISO22900-2 and serves as a facade for the further steps
5. Now i built the functionality with this wrapper and called it ApiOne. And that's what I meant above, you could now use the wrapper and build another API. But the next thoughts relate to ApiOne.
6. The ApiOne is divided into 4 levels. System-Level, Module-Level, ComLogicalLink-Level and ComPrimitive-Level. These different levels encapsulate the respective functionality and of course also hide the handles that exist in the original C API.

For the next things we need a bit more ‘why’ first: For the mechanic in the workshop with hands like a bear and muscles like a lion, a VCI is just one tool of many like a hammer. And that's why they expect the diagnostic tester, VCI and the connection between both is robust like a hammer. Some developers do not understand this when they touch the VCI at their desks with velvet gloves. And that's why some software developers also believe that a loss of connection between the diagnostic tester and the VCI is an edge case. But in the real world, it's pretty much a common use case. ISO22900-2 also describes what should happen during VCI lost. But there are big differences between the API vendors. You could also say at this point "That separates the wheat from the chaff". But back how the ApiOne takes this point into account. 

In a more sophisticated application it looks something like this:

- The API from a vendor and a VCI behind it are set somewhere in the setting.
- Somewhere later in the application elsewhere in the code you establish a connection to the API and the VCI.
- One or more instances of com logical links are then opened on this VCI instance.  And these com logical links are then passed around in the application.
- At the point in the application where you use the com logical link, you usually no longer have direct access to the VCI instance. Unless you passed the VCI instance around which is usually ugly.

The use case is now you are doing something with the com logical link(s)  e.g. read live data or  read out the vehicle ignition status or vehicle battery voltage from the VCI directly. And now it comes to a VCI lost. Because, for example, the VCI was disconnected from OBDII connector or the USB, LAN, Wifi or Bluetooth connection to the VCI was interrupted. Now the ISO22900-II says in this case the com logical links and the VCI are no longer valid (in a nutshell). Which also means instances of VCI and com logical link are also no longer valid. But the biggest problem at this point is. If I want to make a new connection attempt to the VCI. I need to go back to the point in the code where I have an instance of the API. To avoid these twists the ApiOne have the internal classes ModuleLevel, ComLogicalLinkLevel, ComPrimitiveLevel represents real instances but the user of the ApiOne only has access to instances of Module, ComLogicalLink, ComPrimitive which are like wrappers. The trick is now… if there is a VCI lost and an exception is thrown somewhere you can catch the exception (evaluate it a bit more if you like) and then use TryToRecover to let the ApiOne try to establish a new connection. Under the hood, the ApiOne destroys/dispose the old instances and when the connection is back, new instances are created. However, the ApiOne user does not notice this because he is working on the wrapper instances and these are retained.

The [SophisticatedExample](#usage sophisticated example) below show that. Note ApiOne remembers when ComPrimitive was sent with type PduCopt.PDU_COPT_STARTCOM. During the TryToReover run on the ComLogicalLink, this stored ComPrimitive is also sent.

7. Following point number 6 and the explanation, there is an internal representation for Module, ComLogicalLink and ComPrimitive and one that is passed to the outside. 
   
   | internal            | outside        |
   | ------------------- | -------------- |
   | ModuleLevel         | Module         |
   | ComLogicalLinkLevel | ComLogicalLink |
   | ComPrimitiveLevel   | ComPrimitive   |
   
   Of course, this does not exist for the System-Level, since the connection to the dll (or so) is not lost. Therefore, there is for the System-Level only the class DiagPduApiOneSysLevel.

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
                   .FullLibraryPathFormApiShortName(apiShortName)) )
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

## Usage sophisticated example

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using ISO22900.II;

namespace ISO22900.II.SophisticatedExample
{
    public static class UserPreferencesStore
    {
        public static string ApiShortName { get; set; } = "";
        public static string VciName { get; set; } = "";
    }

    public class Program
    {

        public static async Task Main(string[] args)
        {
            using (var factory = new DiagPduApiOneFactory())
            {
                await Run();
            }// here, Dispose runs and cleans everything up

            Console.WriteLine("Finish press a key.");
            Console.ReadKey();
        }


        static async Task Run()
        {
            Settings();
            var vci = ConnectVci();
            var cts = new CancellationTokenSource();
            var vBatTask = ReadBatteryVoltage(vci, cts.Token);
            vBatTask.Start();
            var linkEcuOne = OpenComLogicalLinkForEcuOne(vci);
            await DoSomethingWithEcu(linkEcuOne);
            linkEcuOne.DestroyComLogicalLink(); //or Dispose() how you like;
            cts.Cancel();
            await vBatTask;
            vci.Disconnect(); //or Dispose() how you like;
        }


        static void Settings()
        {
            //Discover Api and VCI e.g. inside the application settings
            var apiGroupedVciList = DiscoverApiVci();
            //this is the list for the user
            foreach (var eachApi in apiGroupedVciList)
            {
                Console.WriteLine($"API: {eachApi.Key}");
                foreach (var vci in eachApi.Items)
                {
                    Console.WriteLine($"\tVCI: {vci.VciName}");
                }
            }

            //here we simulate user selection and saving the settings
            //select first vci from first api
            UserPreferencesStore.ApiShortName = apiGroupedVciList[0].Items[0].ApiShortName;
            UserPreferencesStore.VciName = apiGroupedVciList[0].Items[0].VciName;
        }

        static Module ConnectVci()
        {
            //Use this if you have a VCI that cannot read voltage or ignition status. 
            //var api = DiagPduApiOneFactory.GetApi(DiagPduApiHelper.FullLibraryPathFormApiShortName(UserPreferencesStore.ApiShortName),ApiModifications.VOLTAGE_FIX|ApiModifications.IGNITION_FIX);
            var api = DiagPduApiOneFactory.GetApi(DiagPduApiHelper.FullLibraryPathFormApiShortName(UserPreferencesStore.ApiShortName));
            return api.ConnectVci(UserPreferencesStore.VciName);
        }

        static Task ReadBatteryVoltage(Module vci, CancellationToken ct)
        {
            return new Task(() =>
            {
                while (!ct.IsCancellationRequested)
                {
                    string ignitionState;
                    string vBat; //VBATT means Vehicle Battery Voltage
                    try
                    {
                        vBat = $"VBATT: {(float)(vci.MeasureBatteryVoltage() / 1000.0):00.00}";
                        var temp = vci.IsIgnitionOn() ? "Yes" : "No";
                        ignitionState = $"Ignition on: {temp}";
                    }
                    catch (Iso22900IIException)
                    {
                        // eat all Exceptions
                        vBat = "VBATT: ---";
                        ignitionState = "Ignition on: ---";
                        continue; //comment "continue" out if you want to see it more or less parallel to what's happening around TryTpRcover
                    }

                    Console.WriteLine($"{vBat}");
                    Console.WriteLine($"{ignitionState}");
                    Thread.Sleep(1000); //Reading these values faster usually makes no sense
                }

            }, ct, TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously);
        }

        static ComLogicalLink OpenComLogicalLinkForEcuOne(Module vci)
        {
            var dlcPinData = new Dictionary<uint, string> { { 6, "HI" }, { 14, "LOW" } };
            var busTypeName = "ISO_11898_2_DWCAN";
            var protocolName = "ISO_15765_3_on_ISO_15765_2";

            var link = vci.OpenComLogicalLink(busTypeName, protocolName, dlcPinData.ToList());

            //Set UniqueId ComParam's
            uint pageOneId = 815; //give page one a ID  
            link.SetUniqueRespIdTablePageOneUniqueRespIdentifier(pageOneId);
            link.SetUniqueIdComParamValue(pageOneId, "CP_CanPhysReqId", 0x7E0);
            link.SetUniqueIdComParamValue(pageOneId, "CP_CanRespUSDTId", 0x7E8);

            //Set normal ComParam's
            link.SetComParamValueViaGet("CP_P2Max", 500000);
            return link;
        }

        static async Task DoSomethingWithEcu(ComLogicalLink link)
        {
            link.Connect();

            //Use StartComm to start tester present behavior
            using (var copStartComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM))
            {
                await copStartComm.WaitForCopResultAsync();
            }

            //while this loop is running... disconnect e.g. USB connection to see what happens
            for (byte i = 0x80; i < 0xB0; i++)
            {
                try
                {
                    var request = new byte[] { 0x22, 0xF1, i };

                    using (var cop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, 1, 1, request))
                    {
                        var result = await cop.WaitForCopResultAsync();

                        var responseString = string.Empty;
                        uint responseTime = 0;
                        if (result.DataMsgQueue().Count > 0)
                        {
                            responseString = string.Join(",",
                                result.DataMsgQueue().ConvertAll(bytes => { return BitConverter.ToString(bytes); }));
                            responseTime = result.ResponseTime();
                        }
                        else if (result.PduEventItemErrors().Count > 0)
                        {
                            foreach (var error in result.PduEventItemErrors())
                            {
                               responseString += $"{error.ErrorCodeId}";
                               //+ $"(InfoId: {error.ExtraErrorInfoId} (to decode this code supplier spezifische MDF-File needed))";
                            }

                            responseString = "Error: " + responseString;
                        }
                        Console.WriteLine($"{BitConverter.ToString(request)} | {responseString}  | {responseTime}µs");
                    }
                }
                catch (Iso22900IIException e)
                {
                    //These are the typical errors at VCI lost (you can trigger it with e.g. disconnecting the USB connection)
                    //Only Actia unfortunately does the very general "PDU_ERR_FCT_FAILED" with some VCI
                    //I hope Actia improves this
                    if (e.PduError == PduError.PDU_ERR_MODULE_NOT_CONNECTED ||
                         e.PduError == PduError.PDU_ERR_COMM_PC_TO_VCI_FAILED ||
                         e.PduError == PduError.PDU_ERR_FCT_FAILED)
                    {
                        Console.WriteLine("Error VCI lost. Check the connection to the VCI.");
                        Console.WriteLine("Press [Enter] to start TryToRecover function or [any] other key to exit");
                        if (Console.ReadKey().Key != ConsoleKey.Enter)
                        {
                            Console.WriteLine("Exit");
                            return;
                        }
                        else
                        {
                            //this sleep only makes sense if the user presses enter very quickly (after he has repaired the connection, of course)
                            //Windows also needs some time to recognize a reconnected device
                            //The time between "the mechanical connection is okay again" and "the operating system noticed it too" is difficult to determine
                            Thread.Sleep(1000); //gives some time to e.g. reconnect the USB plug
                            if (!link.TryToRecover(out var msg))
                            {
                                Console.WriteLine($"Recovering failed: {msg}");
                                return;
                            }
                        }
                    }
                }
            }
            link.Disconnect();
        }

        public static List<Grouping<string, ApiVci>> DiscoverApiVci()
        {
            List<ApiVci> ApiVciList = new List<ApiVci>();

            foreach (var rootFileItem in DiagPduApiHelper.InstalledMvciPduApiDetails())
            {
                using (var api = DiagPduApiOneFactory.GetApi(DiagPduApiHelper.FullLibraryPathFormApiShortName(rootFileItem.ShortName)))
                {
                    foreach (var vciBehindApi in api.PduModuleDataSets)
                    {
                        ApiVciList.Add(new ApiVci { ApiShortName = rootFileItem.ShortName, VciName = vciBehindApi.VendorModuleName });
                    }
                }
            }
            return (from apiVci in ApiVciList
                    group apiVci by apiVci.ApiShortName into apiGroup
                    select new Grouping<string, ApiVci>(apiGroup.Key, apiGroup)).ToList<Grouping<string, ApiVci>>();
        }

    }

    public class ApiVci
    {
        public string ApiShortName { get; set; }
        public string VciName { get; set; }
    }

    // Grouping of items by key 
    public class Grouping<TKey, TItem> : List<TItem>
    {
        public TKey Key { get; }

        public IList<TItem> Items => base.AsReadOnly();

        public Grouping(TKey key, IEnumerable<TItem> items)
        {
            Key = key;
            AddRange(items);
        }
    }
}
```

## FAQ

**Question:** DoIP and D-PDU API love each other?

**Answer:** DoIP is like an elephant in a china shop. No kidding you can really see how the 1st generation of developers on the ISO22900-2 standard tried to organize the various steps of initialization in such a way that the connection establishment always follows the same pattern to the outside independent of the protocol. The 2nd generation only tried to bring DoIP quickly into ISO 22900-2 without the ambition to adapt it to the known behavior as far as possible. The fact that DoIP is the elephant is always justified with the bus topology that is needed for DoIP. But that is only half the stroy.



**Q:** What is the right pin setup for DoIP (Diagnostic over internet protocol)?

**A:** Short answer this is a nightmare. And I think that comes from a lot of factors (very long answer). 

One is that the connection is not established when the ComLogicalLink connect() is executed as it's the case with other protocols but with DoIP much earlier, e.g. the Address Resolution Protocol (ARP) is running. This makes the transfer of the pin description when building the ComLogicalLink almost pointless.

**Let's look at the hardware side first.**

 The standards have always reserved pins for manufacturer-specific tasks.

Extract from 15031-3 or SAEJ1962:  *"6.3 Vehicle connector contact allocation
6.3.1 Vehicle connector contacts 1, 3, 8, 9, 11, 12 and 13
Allocation of Contacts 1, 3, 8, 9, 11, 12 and 13 of the vehicle connector is left at the discretion of the vehicle manufacturer. "*

With DoIP, the vehicle manufacturers recognized that these were the only pins that were not yet standardized. And that's why we were still free for DoIP. Of course, many manufacturers, especially in older vehicles, had already given some pins a different task than DoIP. But in the course of the task "getting a standard for DoIP". The manufacturers have agreed on "2 options for pinout" and the "Activation line" for DoIP. The Battery + pin is connected to the Activation line via a resistor. The control unit (e.g. gateway) in the vehicle recognizes via the "Activation line" that it should switch ethernet on the diagnostic plug (everything in the vehicle, mind you). This behavior is intended to protect the ethernet electronics in the vehicle, e.g. if old VCIs are plugged into the vehicle that have already done something else on these pins and also from short circuits that could occur with everything that can be plugged into the OBD plug these days.

There are 2 options how DoIP can be routed to the 16-pin OBD connector. 

| OBD pinout | Option 1        | Option 2        |
| ---------- | --------------- | --------------- |
| 1          |                 | RX+             |
| 3          | RX+             |                 |
| 8          | Activation line | Activation line |
| 9          |                 | RX-             |
| 11         | RX-             |                 |
| 12         | TX+             | TX+             |
| 13         | TX-             | TX-             |
| 16         | Battery +       | Battery +       |

*(only the pins required for DoIP are listed)*

Activation line is not a line over which data is transmitted, but only for protection as described above. Therefore it is not considered when opening the ComLoicalLogiclLink.

You can see in the table aboveTX- and TX+ are fixed. Only RX- and RX+ make the difference. 



**Now how the pinout should be described on the software side.**

The D-PDU API needs a description for each pin. The ISO22900-2 was only expanded to include these underlined DoIP entries when some were already done with their development.

| Short name     | Pin type description (example protocol usage)                                        |
| -------------- | ------------------------------------------------------------------------------------ |
| HI             | Differential Line - High (e.g. DW_CAN High)                                          |
| LOW            | Differential Line - Low (e.g. DW_CAN Low), *<u>DoIP TX_Minus_Line*</u>               |
| K              | UART K-Line (e.g. KWP2000)                                                           |
| L              | UART L-Line (e.g. ISO 9141-2)                                                        |
| TX             | UART uni-directional transmit (e.g. SAE J2190), *<u>DoIP TX_Plus_Line</u>*           |
| RX             | UART uni-directional receive (e.g. SAE J2190), *<u>DoIP RX_Plus_Line</u>*            |
| PLUS           | SAE J1850 Plus (e.g. SAE J1850 VPW and SAE J1850 PWM)                                |
| MINUS          | SAE J1850 Minus (e.g. SAE J1850 PWM), *<u>DoIP RX_Minus_Line</u>*                    |
| SINGLE         | Single wire [e.g. SW_CAN, and UART bi-directional transmit/receive (e.g. SAE J2740)] |
| PROGV          | Pin to set the programmable voltage on DLC                                           |
| IGNITION_CLAMP | Pin to read the ignition sense state from DLC                                        |

And it was probably not so easy to find out without this list whether e.g. Ethernet RX+ or RX- should get pin type description RX. Other developers have thought that since the DoIP options only differ by 2 pins, we should only specify 2 pins. Some manufacturers only describe the plus lines as TX+ and RX+. And apparently some do not describe the DLC but the RJ45 connection so that they do not have to distinguish between DoIP option 1 and 2.

To clarify the chaos, this phrase was added in the last 2022 edition of ISO 22900-2.

*NOTE The DoIP pins are used for documentation but are not considered for the D-PDU API*

Embarrassing when you declare chaos to be the norm.



I think that would be the correct pin to pin description for DoIP with option 1.

{ 3, "RX" }, { 11, "MINUS" }, { 12, "TX" }, { 13, "LOW" }

I've seen that too

-  { 3, "TX" }, { 11, "LOW" }, { 12, "RX" }, { 13, "MINUS" }

- { 1, "TX" }, { 3, "RX" }

- { 3, "RX" }, { 12, "TX" }

- 