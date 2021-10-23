using System;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;


namespace CMcG.LinePasteFix
{
    [InstalledProductRegistration("#110", "#112", "1.0")]
    [Guid(GuidList.guidLinePasteFixPkgString)]
    public class LinePasteFixPackage : AsyncPackage
    {
    }
}