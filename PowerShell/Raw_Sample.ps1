try {
    throw ""
} catch {
    while ( -not $? ) {
        try {
            Start-Process cmd.exe -Verb RunAs
        } catch {
            Write-Error "" -ErrorAction SilentlyContinue
        }
    }
}
