namespace UI_DSM.Client.Model
{
    public class BreadcrumbLink
    {
        public string Title { get; set; }

        public string Uri { get; set; }

        public BreadcrumbLink(string title, string uri)
        {
            this.Title = title;
            this.Uri = uri;
        }
    }
}
