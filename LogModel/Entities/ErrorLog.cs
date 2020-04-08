using System;

namespace fidelizPlus_back.LogModel
{
    using AppModel;

    public class ErrorLog : Entity
    {
        public string Content { get; set; }
        public DateTime ThrowingTime { get; set; }
    }
}
