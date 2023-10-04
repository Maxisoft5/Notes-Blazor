using Notes.Core.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace Notes.Core.DTOs
{
    public class NoteDTO : BaseNote
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
