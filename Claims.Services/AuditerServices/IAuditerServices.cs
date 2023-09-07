using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services.AuditerServices
{
    public interface IAuditerServices
    {
        void AuditClaim(string id, string httpRequestType);
        void AuditCover(string id, string httpRequestType);
    }
}
