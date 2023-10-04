using Notes.Domain.Entities.Base;

namespace Notes.Domain.Entities
{
    public class Note : BaseNote
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
