using SigortaVip.Constant;
using System.Collections.Generic;

public static class OtpScriptMappings
{
    public static readonly Dictionary<string, OtpScriptInfo> ScriptMap = new Dictionary<string, OtpScriptInfo>
    {
        { InsuranceConstants.AtlasSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.AtlasSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "txtGAKod",SubmitButtonHtml="x-btn-text icon-key"} },
        { InsuranceConstants.CorpusSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.CorpusSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "txtGAKod",SubmitButtonHtml="x-btn-text icon-key"} },
        { InsuranceConstants.SekerSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.SekerSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "txtGAKod",SubmitButtonHtml="x-btn-text icon-key"} },
        { InsuranceConstants.BereketSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.BereketSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "txtGAKod",SubmitButtonHtml="x-btn-text icon-key"} },
        { InsuranceConstants.OrientSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.OrientSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "txtGAKod",SubmitButtonHtml="x-btn-text icon-key"} },
        { InsuranceConstants.AveonSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.AveonSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn btn-primary"} },
        { InsuranceConstants.TurkiyeKatilimSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.TurkiyeKatilimSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn btn-primary"} },
        { InsuranceConstants.DogaSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.DogaSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn btn-primary"} },
        { InsuranceConstants.KoruSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.KoruSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn btn-primary"} },
        { InsuranceConstants.AnaSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.AnaSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn btn-primary"} },
        { InsuranceConstants.PriveSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.PriveSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn btn-primary"} },
        { InsuranceConstants.TurkNipponSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.TurkNipponSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Gauthcode",SubmitButtonHtml="loginSend"} },
        { InsuranceConstants.AllianzSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.AllianzSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "smsToken",SubmitButtonHtml="btn-submit js-redirect"} },
       // Bakılacak
        { InsuranceConstants.GeneraliSigorta, new OtpScriptInfo {CompanyName= InsuranceConstants.GeneraliSigorta,  Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "login-password",SubmitButtonHtml="btn newButton newButton btn-danger login-submit col-sm-6 "} },
        { InsuranceConstants.SompoJapanSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.SompoJapanSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn-submit js-redirect"} },
        { InsuranceConstants.UnicoSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.UnicoSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn-submit js-redirect"} },
        { InsuranceConstants.HdiSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.HdiSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn-submit js-redirect"} },
        { InsuranceConstants.NeovaSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.NeovaSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "txtGACode",SubmitButtonHtml="btn-submit js-redirect"} },
        { InsuranceConstants.AnkaraSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.AnkaraSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn-primary btn",SubmitButtonIndex="0"} },
        
      
        // Turkiye
        // Ray
        { InsuranceConstants.RaySigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.RaySigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn-submit js-redirect"} },
        { InsuranceConstants.HepIyiSigorta, new OtpScriptInfo {CompanyName = InsuranceConstants.HepIyiSigorta, Script = "document.querySelector(\"[class*='x-mask-loading']\").value" , Delay = 2000,TokenHtml= "Code",SubmitButtonHtml="btn-submit js-redirect"} },

        // Turkiye Katılım bizde yok
        

        
    };
}
