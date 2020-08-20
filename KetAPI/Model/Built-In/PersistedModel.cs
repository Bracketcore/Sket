using MongoDB.Entities;
using System;

namespace Bracketcore.KetAPI.Model
{

    public abstract class PersistedModel : Entity
    {
        public One<UserModel> OwnerID { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}