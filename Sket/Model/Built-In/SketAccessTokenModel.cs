using MongoDB.Entities;
using System;
using System.Collections.Generic;

namespace Bracketcore.KetAPI.Model
{
    [Name("AccessToken")]
    public class SketAccessTokenModel : SketPersistedModel
    {
        public string Tk { get; set; }
        public DateTime Ttl { get; set; }
        public List<string> Scope { get; set; }

        public SketAccessTokenModel()
        {
            DB.Index<SketAccessTokenModel>()
                .Key(o => o.Tk, KeyType.Text)
                .Option(o => o.Unique = true)

                .Key(o => o.OwnerID, KeyType.Descending)
                .CreateAsync();
        }
    }
}