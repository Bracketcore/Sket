using System.Collections.Generic;
using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{
    [Name("AccessToken")]
    public sealed class SketAccessTokenModel: SketPersistedModel
    {
        public string Tk { get; set; }
        public int ttl { get; set; }
        public List<string> Scope { get; set; }

        public SketAccessTokenModel()
        {
            DB.Index<SketAccessTokenModel>()
                .Key(o => o.Tk, KeyType.Text)
                .Option(o=> o.Unique = true)

                .Key(o => o.OwnerID, KeyType.Descending)
                .CreateAsync();
        }
    }
}