using System;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;


namespace CMcG.LinePasteFix
{
    [InstalledProductRegistration("#110", "#112", "8.0")]
    [Guid(GuidList.guidLinePasteFixPkgString)]
    public class LinePasteFixPackage : Package
    {
    }
}