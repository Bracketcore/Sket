using MongoDB.Entities;
using System;

namespace Bracketcore.KetAPI.Model
{

    public abstract class SketPersistedModel : Entity
    {
        public One<SketUserModel> OwnerID { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}