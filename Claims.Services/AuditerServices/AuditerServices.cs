using Claims.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services.AuditerServices
{
    public class AuditerServices : IAuditerServices
    {
        private readonly AuditContext _auditContext;

        public AuditerServices(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        public async Task AuditClaim(string id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            await _auditContext.AddAsync(claimAudit);
            await _auditContext.SaveChangesAsync();
        }

        public async Task AuditCover(string id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            await _auditContext.AddAsync(coverAudit);
            await _auditContext.SaveChangesAsync();
        }
    }
}
