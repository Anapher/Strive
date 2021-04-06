namespace Identity.API.Models.Response
{
    public class UserInfoDto
    {
        private UserInfoDto(string id, bool notFound, string? displayName)
        {
            Id = id;
            NotFound = notFound;
            DisplayName = displayName;
        }

        public string Id { get; set; }
        public bool NotFound { get; set; }
        public string? DisplayName { get; set; }

        public static UserInfoDto CreateNotFound(string id)
        {
            return new UserInfoDto(id, true, null);
        }

        public static UserInfoDto Create(string id, string displayName)
        {
            return new UserInfoDto(id, false, displayName);
        }
    }
}
