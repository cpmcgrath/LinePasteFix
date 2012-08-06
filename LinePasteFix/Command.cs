using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMcG.LinePasteFix
{
    class Command
    {
        public Command(string guid, uint id)
        {
            Guid     = new Guid(guid);
            Id       = id;
        }

        public Guid Guid     { get; private set; }
        public uint Id       { get; private set; }

        public bool Is(Guid otherGuid, uint otherId)
        {
            return Guid == otherGuid && Id == otherId;
        }
    }
}
