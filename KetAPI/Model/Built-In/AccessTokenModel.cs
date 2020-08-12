using System.Collections.Generic;

namespace KetAPI.Model
{
    public abstract class AccessTokenModel: PersistedModel
    {
        public int ttl { get; set; }
        public List<string> Scope { get; set; }
    }
}