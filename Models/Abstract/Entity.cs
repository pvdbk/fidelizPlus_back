using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public abstract class Entity
    {
        public abstract IEnumerable<string> Fields { get; }
        public int Id { get; set; }
    }
}
