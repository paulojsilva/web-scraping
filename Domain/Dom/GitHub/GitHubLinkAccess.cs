namespace Domain.Dom.GitHub
{
    public class GitHubLinkAccess
    {
        public enum GitHubLinkAccessType
        {
            Undefined = 0,
            Folder,
            File
        }

        public string Endpoint { get; set; }
        public GitHubLinkAccessType Type { get; set; } = GitHubLinkAccessType.Undefined;
        public bool Loaded { get; set; } = false;
    }
}
