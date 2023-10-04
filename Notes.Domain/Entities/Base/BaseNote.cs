namespace Notes.Domain.Entities.Base
{
    public abstract class BaseNote
    {
        public int Id { get; set; }
        public DateTime AddedDateTime { get; set; }
    }
}
