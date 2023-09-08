﻿using Claims.Auditing;
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

        public void AuditClaim(string id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            _auditContext.Add(claimAudit);
            _auditContext.SaveChanges();
        }

        public void AuditCover(string id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            _auditContext.Add(coverAudit);
            _auditContext.SaveChanges();
        }
    }
}