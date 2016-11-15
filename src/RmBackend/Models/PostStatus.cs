using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RmBackend.Models
{
    public enum PostStatus
    {
        Draft,
        Posted,
        Deleted,
        NeedModification,
        NeedApproval
    }
}
