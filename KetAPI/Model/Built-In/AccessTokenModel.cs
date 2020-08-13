using System.Collections.Generic;
using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{
    [Name("AccessToken")]
    public sealed class AccessTokenModel: PersistedModel
    {
        public string Tk { get; set; }
        public int ttl { get; set; }
        public List<string> Scope { get; set; }

        public AccessTokenModel()
        {
            DB.Index<AccessTokenModel>()
                .Key(o => o.Tk, KeyType.Text)
                .Option(o=> o.Unique = true)

                .Key(o => o.OwnerID, KeyType.Descending)
                .CreateAsync();
        }
    }
}