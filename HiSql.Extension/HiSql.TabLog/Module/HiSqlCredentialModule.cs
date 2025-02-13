using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiSql.TabLog.Interface;

namespace HiSql.TabLog.Module
{
    public class HiSqlCredentialModule : ICredentialModule<HiSqlCredential, HiOperateLog>
    {
        public HiSqlCredentialModule(HiSqlCredentialStorage _storage)
        {
            storage = _storage;
        }

        private HiSqlCredentialStorage storage { get; set; }

        protected override Task<HiSqlCredential> GenerateCredential()
        {
            var hiTabManager = this.storage.hi_TabManager;
            var credentialId = SnroNumber.NewNumber(hiTabManager.SNRO, hiTabManager.SNUM);
            var credential = new HiSqlCredential
            {
                CredentialId = credentialId,
                TableName = hiTabManager.TabName
            };
            return Task.FromResult(credential);
        }

        protected override Task SaveCredential(HiSqlCredential credential)
        {
            return storage.SaveCredential(credential);
        }
    }
}
