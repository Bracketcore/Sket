using MongoDB.Entities;
using System.Collections.Generic;

namespace Bracketcore.KetAPI.Model
{
    [Name("AccessToken")]
    public class AccessTokenModel : SketPersistedModel
    {
        public string Tk { get; set; }
        public int Ttl { get; set; }
        public List<string> Scope { get; set; }

        public AccessTokenModel()
        {
            DB.Index<AccessTokenModel>()
                .Key(o => o.Tk, KeyType.Text)
                .Option(o => o.Unique = true)

                .Key(o => o.OwnerID, KeyType.Descending)
                .CreateAsync();
        }
    }
}