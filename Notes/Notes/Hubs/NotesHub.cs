using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class NotesHub : Hub
    {
        public async Task SendNotesUpdates()
        {
            await Clients.All.SendAsync("ReceiveNotesUpdates");
        }
    }
}
