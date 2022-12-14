namespace blog_api_jwt.Contracts.v1;

public static class ApiRoutes
{
    public const string Root = "api";
    public const string Version = "v1";
    public const string Base = Root + "/" + Version;

    public static class Posts
    {
        public const string Create = Base + "/posts";
        public const string Delete = Base + "/posts/{id}";
        public const string Get = Base + "/posts/{id}";
        public const string GetAll = Base + "/posts";
        public const string Update = Base + "/posts/{id}";
    }

    public static class Tags
    {
        public const string Create = Base + "/tags";
        public const string Get = Base + "/tags/{name}";
        public const string GetAll = Base + "/tags";
    }

    public static class Identity
    {
        public const string Register = Base + "/identity/register";
        public const string Login = Base + "/identity/login";
        public const string Refresh = Base + "/identity/refresh";
    }
}