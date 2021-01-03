using System;
using System.Collections.Generic;
using MongoDB.Entities;

namespace Sket.Core.Models
{
    /// <summary>
    ///     Abstract model for the Access token model
    /// </summary>
    [Name("AccessToken")]
    public class SketAccessTokenModel : SketPersistedModel
    {
        public string Tk { get; set; }
        public DateTime Ttl { get; set; }
        public List<string> Scope { get; set; }
    }
}