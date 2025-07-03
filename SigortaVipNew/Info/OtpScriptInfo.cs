public class OtpScriptInfo
{
    public string Script { get; set; }
    public int Delay { get; set; } = 1000;

    public string TokenHtml { get; set; }

    public string SubmitButtonHtml { get; set; }

    public string SubmitButtonIndex { get; set; } = "0";

    public string CompanyName { get; set; } = string.Empty;


}