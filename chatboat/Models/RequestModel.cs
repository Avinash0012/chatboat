namespace chatboat.Models
{
    
    public class RequestPart
    {
        public string text { get; set; }
    }

    public class RequestContent
    {
        public string Role { get; set; }
        public List<RequestPart> Parts { get; set; }
    }

    public class RequestModel
    {
        public List<RequestContent> Contents { get; set; }
    }

}
