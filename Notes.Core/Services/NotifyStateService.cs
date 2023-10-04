using Common.DTOs;
using Common.Events;

namespace Notes.Core.Services
{
    public class NotifyStateService
    {
        public event FiltersEventDelegate? EventNoteAdd;
        public event FiltersEventDelegate? EventFilters;
      

        public void NotifyAddNoteEvent(object sender, FiltersRequest filtersRequest)
        {
            if (this.EventNoteAdd != null)
                this.EventNoteAdd(sender, filtersRequest);
        }

        public void NotifyFilters(object sender, FiltersRequest filtersRequest)
        {
            if (this.EventFilters != null)
                this.EventFilters(sender, filtersRequest);
        }


    }
}
