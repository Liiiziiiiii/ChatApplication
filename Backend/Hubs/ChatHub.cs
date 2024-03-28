using Microsoft.AspNetCore.SignalR;



namespace Chat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, UserRoomConnection> _connections;

        public ChatHub(IDictionary<string, UserRoomConnection> connections)
        {
            _connections = connections;
        }
        public async Task JoinRoom(UserRoomConnection  userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room!);
            _connections[Context.ConnectionId] = userConnection;
            await Clients.Group(userConnection.Room!)
                .SendAsync("ReceiveMessage", "Let`s Program Bot", $"{userConnection.User} has Joined the Group", DateTime.Now);
            await SendConnectingUser(userConnection.Room!);
        }

        public async Task SendMessage(string message)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserRoomConnection userConnection))
            {
                await Clients.Groups(userConnection.Room!).SendAsync("ReceiveMessage", userConnection.User, message, DateTime.Now);
            }
        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if(!_connections.TryGetValue(Context.ConnectionId, out UserRoomConnection roomConnection))
            {
                return base.OnDisconnectedAsync(exception);
            }
            _connections.Remove(Context.ConnectionId);

            Clients.Group(roomConnection.Room!)
                .SendAsync("ReceiveMessage", "Let`s Program Bot", $"{roomConnection.User} has left the Group", DateTime.Now);

            SendConnectingUser(roomConnection.Room!);
            return base.OnDisconnectedAsync(exception) ;
        }

        public Task SendConnectingUser(string room)
        {
            var user = _connections.Values
                .Where(u => u.Room == room)
                .Select(u => u.User);

            return Clients.Group(room).SendAsync("ConnectedUser", user);
        }


    }
}
