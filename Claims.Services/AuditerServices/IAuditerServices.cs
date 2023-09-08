using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services.AuditerServices
{
    public interface IAuditerServices
    {
        Task AuditClaim(string id, string httpRequestType);
        Task AuditCover(string id, string httpRequestType);
    }
}
