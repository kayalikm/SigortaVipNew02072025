using SigortaVip.Constant;
using System.Collections.Generic;

public static class SmsScriptMappings
{
    public static readonly Dictionary<string, SmsScriptInfo> ScriptMap = new Dictionary<string, SmsScriptInfo>
    {
        { InsuranceConstants.AkSigorta, new SmsScriptInfo {CompanyName = InsuranceConstants.AkSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "smsPassword",SubmitButtonHtml="btn btn-primary" , SubmitButtonIndex="1"} },
        { InsuranceConstants.HepIyiSigorta, new SmsScriptInfo {CompanyName = InsuranceConstants.HepIyiSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "authenticationCode",SubmitButtonHtml="btn btn-primary" , SubmitButtonIndex="0"} },
        { InsuranceConstants.AnadoluSigorta, new SmsScriptInfo {CompanyName = InsuranceConstants.AnadoluSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 4000,TokenHtml= "otpCode",SubmitButtonHtml="ant-btn ant-btn-primary small-radius" , SubmitButtonIndex="0"} },


    };
}
