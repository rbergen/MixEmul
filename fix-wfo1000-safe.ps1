# Fix WFO1000 warnings - Add [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] to properties
# This script processes files and adds the required attribute

$ErrorActionPreference = "Stop"

# List of files and their properties that need fixing (from build errors)
$fixes = @(
    @{File="src\MixEmul\Components\MemoryEditor.cs"; Properties=@("IndexedAddressCalculatorCallback", "ReadOnly", "MarkedAddress", "FirstVisibleAddress", "Symbols", "Memory", "ResizeInProgress", "ToolTip")},
    @{File="src\MixEmul\Components\TextFileDeviceEditor.cs"; Properties=@("RecordName", "ReadOnly", "ResizeInProgress", "ShowReadOnly")},
    @{File="src\MixEmul\Components\WordValueEditor.cs"; Properties=@("ReadOnly", "ByteCount", "TextBoxWidth", "IncludeSign", "WordValue")},
    @{File="src\MixEmul\Components\DevicesControl.cs"; Properties=@("ToolTip", "Structure", "Devices")},
    @{File="src\MixEmul\Components\SymbolListView.cs"; Properties=@("Symbols", "MemoryMaxIndex", "MemoryMinIndex")},
    @{File="src\MixEmul\Components\TeletypeForm.cs"; Properties=@("StatusText", "TeletypeDevice", "EchoInput", "TopMost")},
    @{File="src\MixEmul\MixForm.cs"; Properties=@("ReadOnly")},
    @{File="src\MixEmul\Components\SourceCodeControl.cs"; Properties=@("MarkedFinding", "Findings", "Instructions")},
    @{File="src\MixEmul\Components\DeviceEditorForm.cs"; Properties=@("Devices")},
    @{File="src\MixEmul\Components\LogListView.cs"; Properties=@("SeverityImageList")},
    @{File="src\MixEmul\Components\MemoryWordEditor.cs"; Properties=@("ReadOnly", "MemoryMaxIndex", "WordValue", "GetMaxProfilingCount", "Marked", "MemoryMinIndex", "ToolTip", "BreakPointChecked", "MemoryWord", "IndexedAddressCalculatorCallback", "Symbols")},
    @{File="src\MixEmul\Components\FullWordEditor.cs"; Properties=@("WordValue", "ReadOnly", "FullWord")},
    @{File="src\MixEmul\Components\ToolStripCycleButton.cs"; Properties=@("Value")},
    @{File="src\MixEmul\Components\AssemblyFindingListView.cs"; Properties=@("Findings", "SeverityImageList")},
    @{File="src\MixEmul\Components\MixByteCollectionCharTextBox.cs"; Properties=@("UseEditMode", "MixByteCollectionValue")},
    @{File="src\MixEmul\Components\DeviceWordEditor.cs"; Properties=@("WordIndex")},
    @{File="src\MixEmul\Components\MemoryExportDialog.cs"; Properties=@("FromAddress", "MaxMemoryIndex", "ToAddress", "MinMemoryIndex", "ProgramCounter")},
    @{File="src\MixEmul\Components\LongValueToolStripTextBox.cs"; Properties=@("MinValue", "LongValue", "MaxValue", "ClearZero")},
    @{File="src\MixEmul\Components\BinaryFileDeviceEditor.cs"; Properties=@("ReadOnly", "ResizeInProgress", "RecordName", "ShowReadOnly")}
)

$totalFixed = 0

foreach ($fix in $fixes) {
    $filePath = $fix.File
    
    if (-not (Test-Path $filePath)) {
        Write-Warning "File not found: $filePath"
        continue
    }
    
    $content = Get-Content $filePath -Raw
    $originalContent = $content
    
    # Add using System.ComponentModel if not present
    if ($content -notmatch 'using System\.ComponentModel;') {
        # Try to add after 'using System;'
        if ($content -match '(?m)^(\s*using System;)') {
            $content = $content -replace '(?m)^(\s*using System;)', "`$1`r`n`$1.ComponentModel;"
            $content = $content -replace 'using System;.ComponentModel;', 'using System.ComponentModel;'
        }
        # Try namespace-scoped using
        elseif ($content -match '(?m)^(namespace .+\r?\n\{\r?\n\s*using System;)') {
            $content = $content -replace '(?m)^(namespace .+\r?\n\{\r?\n\s*using System;)', "`$1`r`n`tusing System.ComponentModel;"
        }
    }
    
    # Add attribute before each property
    foreach ($propName in $fix.Properties) {
        # Match: optional newlines/tabs, then "public" (with optional "new"), property declaration
        # Be very specific to avoid false matches
        $pattern = "(\r?\n\t\t)(public\s+(?:new\s+)?[\w\[\]<>,\.\s]+\s+$propName\s*(?:\{|$))"
        
        if ($content -match $pattern) {
            $replacement = "`$1[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]`r`n`$1`$2"
            $newContent = $content -replace $pattern, $replacement
            
            if ($newContent -ne $content) {
                $content = $newContent
                $totalFixed++
                Write-Host "  Fixed property: $propName"
            }
        }
    }
    
    # Write back if modified
    if ($content -ne $originalContent) {
        Set-Content -Path $filePath -Value $content -NoNewline
        Write-Host "Updated: $filePath"
    }
}

Write-Host "`nTotal properties fixed: $totalFixed"
Write-Host "Rebuild the solution to verify all fixes."
