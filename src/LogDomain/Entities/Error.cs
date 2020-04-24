using System;

namespace fidelizPlus_back.LogDomain
{
    using AppDomain;

    public class Error : Entity
    {
        public string Content { get; set; }
        public DateTime ThrowingTime { get; set; }
    }
}
