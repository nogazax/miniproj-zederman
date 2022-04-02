using System;
using System.Collections.Generic;

namespace dotnetProj.Models
{
    public partial class Task
    {
        public string Id { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public long Status { get; set; }

        public virtual Person Owner { get; set; } = null!;
    }
}
