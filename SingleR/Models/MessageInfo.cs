namespace SingleR.Models
{
    public class MessageInfo
    {
        public class MessageOutput
        {
            public string Message { get; set; }
            public DateTime? Datetime { get; set; }
            public string UID { get; set; }
            public string DocumentSEQ { get; set; }
            public List<string> AttachmentFile { get; set; }
            public List<string> FileExtension { get; set; }
            public List<string> FileName { get; set; }
        }
    }
}
