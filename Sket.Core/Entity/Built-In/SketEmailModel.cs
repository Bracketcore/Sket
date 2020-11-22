namespace UnoRoute.Sket.Core.Entity
{
    /// <summary>
    /// Abstract model for the Email model
    /// </summary>
    public abstract class SketEmailModel : SketPersistedModel
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
    }
}