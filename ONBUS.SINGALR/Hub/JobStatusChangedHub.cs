using Microsoft.AspNetCore.SignalR;

namespace ONBUS.SINGALR.Hub
{
    public class JobStatusChangedHub: Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

         
            await Clients.Caller.SendAsync("GroupJoined", $"You have joined the group {groupName}.");
        }
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

         
            await Clients.Caller.SendAsync("GroupLeft", $"You have left the group {groupName}.");
        }
    }
}
