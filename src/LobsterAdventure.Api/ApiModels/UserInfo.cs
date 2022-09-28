namespace LobsterAdventure.Api.ApiModels
{
    public class UserInfo
    {
        public UserInfo()
        {
            UserId = string.Empty;
            UserName = string.Empty;
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
