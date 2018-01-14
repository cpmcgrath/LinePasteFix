function Replace($file, $before, $after)
{
    $content = Get-Content $file | Foreach-Object {$_ -replace $before, $after }
    $content | Set-Content $file -Encoding UTF8
}

function UpdateVersion($project, $version)
{
    $file = "./$project/Properties/AssemblyInfo.cs"
    Replace $file "AssemblyVersion\s*\([^\)]+\)"     "AssemblyVersion    (`"$version`")"
    Replace $file "AssemblyFileVersion\s*\([^\)]+\)" "AssemblyFileVersion(`"$version`")"
}

function UpdateVsVersion($project, $version)
{
    UpdateVersion $project $version
        
    Replace "./$project/LinePasteFixPackage.cs" `
            "\[InstalledProductRegistration\(`"#110`", `"#112`", `"[^`"]+`"\)\]" `
            "[InstalledProductRegistration(`"#110`", `"#112`", `"$version`")]"
        
    Replace "./$project/source.extension.vsixmanifest" `
            "Id=`"LinePasteFix.CMcG.a2363b0b-ab89-427f-b36d-7bf3d6984f30`" Version=`"[^`"]+`"" `
            "Id=`"LinePasteFix.CMcG.a2363b0b-ab89-427f-b36d-7bf3d6984f30`" Version=`"$version`""
}