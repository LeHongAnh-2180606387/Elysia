using System;
using System.Collections.Generic;

namespace Systems.SaveLoad.Model
{
    public class TrackableObject
    {
        public Dictionary<string, bool> IsModified { get; set; }

        public TrackableObject()
        {
            IsModified = new Dictionary<string, bool>();
        }
    }
}
