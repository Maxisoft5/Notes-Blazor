using Infrastructure.EFCore;

namespace Notes.DataAccess.DAOs.Base
{
    public abstract class BaseNoteDao
    {
        protected DataContext DataContext { get; set; }

        public BaseNoteDao(DataContext dataContext)
        {
            DataContext = dataContext;
        }

    }
}
