﻿using System;

namespace Bonobo.Git.Server.Data
{
    public class ServiceAccount
    {

        public string ServiceAccountName { get; set; }
        public bool InPassManager { get; set; }
        public DateTime? PassLastUpdated { get; set; }
        public Guid Id { get; set; }
        public Guid RepositoryId { get; set; }
        public virtual Repository Repository { get; set; }
    }
}