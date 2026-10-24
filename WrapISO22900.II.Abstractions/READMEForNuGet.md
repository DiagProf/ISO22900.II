# ISO22900.II-Sharp.Abstractions

Interfaces, data classes, enums and exceptions of [ISO22900.II-Sharp](https://www.nuget.org/packages/ISO22900.II-Sharp), **without** any native D-PDU API (ISO 22900-2) dependency.

The package exists for two kinds of users:

| You want to ... | Reference |
|---|---|
| talk to a real VCI through an installed D-PDU API | `ISO22900.II-Sharp` (pulls in this package automatically) |
| write your own implementation **below** the interfaces: a simulator, a test double, a bridge to a non ISO 22900-2 VCI, a remote/recording proxy, ... | `ISO22900.II-Sharp.Abstractions` only |

Applications and libraries that are programmed against the interfaces work with both, the native implementation and your own one.

## Interface hierarchy

```
IDiagPduApiOneSysLevel           native: DiagPduApiOneSysLevel  (DiagPduApiOneFactory.GetApi(...))
 └─ ConnectVci()      -> IModule              native: Module
     └─ OpenComLogicalLink() -> IComLogicalLink   native: ComLogicalLink
         └─ StartCop()        -> IComPrimitive    native: ComPrimitive
             └─ WaitForCopResult[Async]() -> ComPrimitiveResult
```

All four interfaces derive from `IDisposable`, so `using var ... = ...` works everywhere.
The namespaces are the same as in ISO22900.II-Sharp (`ISO22900.II` and `ISO22900.II.Interface`).

## Writing your own implementation

All data classes that the interfaces return have public constructors. Native handles are an implementation detail,
the handle free constructors are meant for your own implementations:

| Type | Constructor for own implementations |
|---|---|
| `ComPrimitiveResult` | `new ComPrimitiveResult(Queue<PduEventItem>)` |
| `PduEventItemResult` | `new PduEventItemResult(copTag, timestamp, pduResultData)` |
| `PduEventItemError` | `new PduEventItemError(copTag, timestamp, PduErrEvt, extraErrorInfoId)` |
| `PduEventItemInfo` | `new PduEventItemInfo(copTag, timestamp, PduInfo, extraInfoData)` |
| `PduEventItemStatus` | `new PduEventItemStatus(copTag, timestamp, PduStatus)` |
| `PduResultData` | `new PduResultData(byte[] dataBytes, uniqueRespIdentifier)` or the full constructors |
| `PduExStatusData` | `new PduExStatusData(PduStatus, timestamp, extraInfo)` |
| `PduExLastErrorData` | `new PduExLastErrorData(PduErrEvt, timestamp, extraInfo)` |
| `PduModuleData` | `new PduModuleData(moduleTypeId, vendorModuleName, vendorAdditionalInfo, PduStatus)` |
| `PduRscIdItemData` | `new PduRscIdItemData(vendorModuleName, uint[] resourceIds)` |
| `PduVersionData`, `PduIoCtlEntityStatusData` | full constructors |
| `PduComParamOfType*` | `new PduComParamOfTypeUint("CP_P2Max", PduPc.PDU_PC_TIMING, 500000)` (short name based) |
| `CallbackEventArgs` | `new CallbackEventArgs(PduEvtData.PDU_EVT_DATA_LOST)` |
| `Iso22900IIException` | `new Iso22900IIException(message, PduError)` |

### Example: ComPrimitive

A ComPrimitive that returns a fixed set of event items:

```csharp
using ISO22900.II;
using ISO22900.II.Interface;

public sealed class FakeComPrimitive(params PduEventItem[] items) : IComPrimitive
{
    public ComPrimitiveResult WaitForCopResult(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return CreateResult();
    }

    public Task<ComPrimitiveResult> WaitForCopResultAsync(CancellationToken ct = default)
        => ct.IsCancellationRequested
            ? Task.FromCanceled<ComPrimitiveResult>(ct)
            : Task.FromResult(CreateResult());

    public PduExStatusData Status() => new(PduStatus.PDU_COPST_FINISHED, 0, 0);
    public void Cancel() { }
    public bool TryToRecover(out string exMessage) { exMessage = string.Empty; return true; }
    public void Dispose() { }

    private ComPrimitiveResult CreateResult() => new(new Queue<PduEventItem>(items));
}
```

### Example: ComLogicalLink

A simulated ComLogicalLink. A delegate maps each request of a `PDU_COPT_SENDRECV` to the ECU responses,
an empty array simulates an ECU that does not answer (`PDU_ERR_EVT_RX_TIMEOUT`). The link tracks the
link state (`OFFLINE` → `ONLINE` → `COMM_STARTED`), keeps its ComParams and records all sent requests.

```csharp
using ISO22900.II;
using ISO22900.II.Interface;

public sealed class FakeComLogicalLink(Func<byte[], byte[][]> respond) : IComLogicalLink
{
    private readonly Dictionary<string, PduComParam> _comParams = new(StringComparer.Ordinal);
    private PduStatus _status = PduStatus.PDU_CLLST_OFFLINE;
    private uint _timestamp;

    public List<byte[]> SentRequests { get; } = [];
    public uint BatteryVoltageMv { get; set; } = 12_500;
    public bool IgnitionOn { get; set; } = true;

    // The fake never raises these events, so the accessors are intentionally empty.
    public event EventHandler<CallbackEventArgs> DataLost { add { } remove { } }
    public event EventHandler<PduEventItem> EventFired { add { } remove { } }

    public void AddComParam(PduComParam cp) => _comParams[cp.ComParamShortName] = cp;

    #region Communication

    public void Connect() => _status = PduStatus.PDU_CLLST_ONLINE;
    public void Disconnect() => _status = PduStatus.PDU_CLLST_OFFLINE;

    public IComPrimitive StartCop(PduCopt pduCopType, TimeSpan delayTimeMs = default, uint copTag = 0)
        => Execute(pduCopType, [], copTag);

    public IComPrimitive StartCop(PduCopt pduCopType, byte[] copData, uint copTag = 0)
        => Execute(pduCopType, copData, copTag);

    public IComPrimitive StartCop(PduCopt pduCopType, int sendCycles, int receiveCycles, byte[] copData, uint copTag = 0)
        => Execute(pduCopType, copData, copTag);

    public IComPrimitive StartCop(PduCopt pduCopType, byte[] copData, PduCopCtrlData copCtrlData, uint copTag = 0)
        => Execute(pduCopType, copData, copTag);

    private IComPrimitive Execute(PduCopt copType, byte[] request, uint copTag)
    {
        if (_status == PduStatus.PDU_CLLST_OFFLINE)
            throw new Iso22900IIException("ComLogicalLink is offline.", PduError.PDU_ERR_CLL_NOT_CONNECTED);

        switch (copType)
        {
            case PduCopt.PDU_COPT_STARTCOMM:
                _status = PduStatus.PDU_CLLST_COMM_STARTED;
                return new FakeComPrimitive();
            case PduCopt.PDU_COPT_STOPCOMM:
                _status = PduStatus.PDU_CLLST_ONLINE;
                return new FakeComPrimitive();
            case PduCopt.PDU_COPT_SENDRECV:
                SentRequests.Add(request);
                var responses = respond(request);
                if (responses.Length == 0)
                    return new FakeComPrimitive(new PduEventItemError(copTag, ++_timestamp, PduErrEvt.PDU_ERR_EVT_RX_TIMEOUT));
                return new FakeComPrimitive(
                    [.. responses.Select(r => (PduEventItem)new PduEventItemResult(copTag, ++_timestamp, new PduResultData(r)))]);
            default:
                return new FakeComPrimitive();
        }
    }

    #endregion

    #region ComParams

    public PduComParam GetComParam(string name)
        => _comParams.TryGetValue(name, out var cp)
            ? cp
            : throw new Iso22900IIException($"Unknown ComParam '{name}'.", PduError.PDU_ERR_INVALID_PARAMETERS);

    public PduComParam SetComParamValueViaGet(string name, long value) => GetComParam(name).UpdateData(value);
    public PduComParam SetComParamValueViaGet(string name, byte[] value) => GetComParam(name).UpdateData(value);
    public PduComParam SetComParamValueViaGet(string name, uint[] value) => GetComParam(name).UpdateData(value);
    public PduComParam SetComParamValueViaGet(string name, PduParamStructFieldData value) => GetComParam(name).UpdateData(value);
    public void SetComParam(PduComParam cp) => AddComParam(cp);
    public bool TrySetComParam(PduComParam cp) { AddComParam(cp); return true; }

    #endregion

    #region Unique response id table (single ECU: not needed, extend if your scenario uses it)

    public uint GetUniqueIdComParamValue(uint uniqueRespIdentifier, string name) => throw new NotSupportedException();
    public void SetUniqueIdComParamValue(uint uniqueRespIdentifier, string name, long value) => throw new NotSupportedException();
    public void SetUniqueIdComParamValues(uint uniqueRespIdentifier, List<KeyValuePair<string, uint>> listComParamNameToValuePairs) => throw new NotSupportedException();
    public void SetUniqueRespIdTable(List<PduEcuUniqueRespData> ecuUniqueRespDatas) => throw new NotSupportedException();
    public void SetUniqueRespIdTablePageOneUniqueRespIdentifier(uint uniqueRespIdentifier) => throw new NotSupportedException();
    public uint GetUniqueRespIdTableNumberOfPages() => 1;
    public uint GetPageOneUniqueRespIdentifier() => 0;
    public uint GetPageUniqueRespIdentifier(uint pageIndex) => 0;

    #endregion

    #region Status and IoCtl (Try* methods never throw)

    public uint MeasureBatteryVoltage() => BatteryVoltageMv;
    public bool IsIgnitionOn(byte dlcPinNumber = 0) => IgnitionOn;
    public PduExStatusData Status() => new(_status, _timestamp, 0);
    public PduExLastErrorData LastError() => new(PduErrEvt.PDU_ERR_EVT_NOERROR, _timestamp, 0);

    public bool TryIoCtlGeneral(string ioCtlShortName) => true;
    public bool TryIoCtlClearTxQueue() => true;
    public bool TryIoCtlSuspendTxQueue() => true;
    public bool TryIoCtlResumeTxQueue() => true;
    public bool TryIoCtlClearRxQueue() => true;
    public bool TryIoCtlClearMsgFilter() => true;
    public bool TryIoCtlSendBreak() => true;
    public bool TryIoCtlSetBufferSize(uint value) => true;
    public bool TryIoCtlStopMsgFilter(uint value) => true;
    public bool TryIoCtlStartMsgFilter(PduIoCtlOfTypeFilterList value) => true;
    public bool TryIoCtlTlsGetCurrentSessionMode(out uint value) { value = 0; return true; }
    public bool TryIoCtlGeneral(string ioCtlShortName, out uint value) { value = 0; return true; }
    public bool TryIoCtlIsoBusGetDetectedCfs(out byte[] value) { value = []; return true; }
    public bool TryIoCtlGeneral(string ioCtlShortName, out byte[] value) { value = []; return true; }
    public bool TryToRecover(out string exMessage) { exMessage = string.Empty; return true; }

    #endregion

    public void DestroyComLogicalLink() => _status = PduStatus.PDU_CLLST_OFFLINE;
    public void Dispose() => DestroyComLogicalLink();
}
```

Usage, exactly like a native `ComLogicalLink`:

```csharp
using var link = new FakeComLogicalLink(request => Convert.ToHexString(request) switch
{
    "1003"   => [Convert.FromHexString("5003003201F4")],
    "22F190" => [Convert.FromHexString("62F190575657")],
    _        => [] // no answer -> PDU_ERR_EVT_RX_TIMEOUT
});
link.AddComParam(new PduComParamOfTypeUint("CP_P2Max", PduPc.PDU_PC_TIMING, 50_000));

link.Connect();
link.SetComParamValueViaGet("CP_P2Max", 150_000);
using (var startComm = link.StartCop(PduCopt.PDU_COPT_STARTCOMM))
    await startComm.WaitForCopResultAsync();

using var cop = link.StartCop(PduCopt.PDU_COPT_SENDRECV, [0x22, 0xF1, 0x90], copTag: 7);
var result = await cop.WaitForCopResultAsync();
foreach (var item in result.RawQueue)
{
    if (item is PduEventItemResult response)
        Console.WriteLine($"tag {response.CopTag}: {Convert.ToHexString(response.ResultData.DataBytes)}");
}
```

The same pattern applies to `IModule` (returns your `IComLogicalLink` from `OpenComLogicalLink(...)`) and
`IDiagPduApiOneSysLevel` (returns your `IModule` from `ConnectVci(...)`).

## Contract your implementation should follow

The native implementation behaves like this, and consumers rely on it:

- `IComPrimitive.WaitForCopResult(ct)` and `WaitForCopResultAsync(ct)` throw `OperationCanceledException` when `ct` is cancelled.
  The native implementation can be switched back to the old "return the result collected so far" behavior with
  `ApiModifications.LEGACY_SWALLOW_COP_CANCELLATION`.
- The `ComPrimitiveResult` contains result, error and info items, but **no** status items.
- `Dispose()` on an `IComPrimitive` cancels a still running COP; `Dispose()` on link/module/api closes it.
- `Try*` methods never throw `Iso22900IIException`, they return `false`.
- Other methods signal D-PDU API errors with `Iso22900IIException` (`PduError` property).

## Versioning

`ISO22900.II-Sharp` and `ISO22900.II-Sharp.Abstractions` are released together with **the same version number**.
Always use the same version of both packages (the native package shares internal members with this package).

Up to 1.3.x all these types lived in `ISO22900.II-Sharp` itself. Since 1.4.0 they live here; the native assembly contains
type forwarders, so binaries that only use the concrete classes keep working. Code that uses the interfaces must be
recompiled, because `IModule.OpenComLogicalLink` now returns `IComLogicalLink` and `IComLogicalLink.StartCop` returns `IComPrimitive`.
