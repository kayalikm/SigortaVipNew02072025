using SigortaVip.Constant;
using System.Collections.Generic;


namespace SigortaVip.Models
{
    internal class LoginScriptMappings
    {
        public static readonly Dictionary<string, LoginScriptInfo> ScriptMap = new Dictionary<string, LoginScriptInfo>
        {
            // Genel Login Scripti Start
            { InsuranceConstants.AllianzSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AllianzSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="btn-submit", Delay = 2000 } },
            { InsuranceConstants.AkSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AkSigorta, Script ="",UserNameHtml ="fpf-username",PasswordHtml = "fpf-password",SubmitButtonHtml="btn btn-primary r", Delay = 4000 } },
            { InsuranceConstants.HepIyiSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.HepIyiSigorta, Script ="",UserNameHtml ="UserName",PasswordHtml = "Password",SubmitButtonHtml="btn btn-lg btn-primary w-100 mb-5", Delay = 2000 } },
            { InsuranceConstants.AcnTurkSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AcnTurkSigorta, Script ="",UserNameHtml ="userName",PasswordHtml = "password",SubmitButtonHtml="btn btn-primary btn-block btn-sm", Delay = 2000 } },
            { InsuranceConstants.AtlasSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AtlasSigorta, Script ="",UserNameHtml ="txtUsername",PasswordHtml = "txtPassword",SubmitButtonHtml="x-btn-wrap x-btn x-btn-text-icon",ButtonIndex="1", Delay = 2000 } },
            { InsuranceConstants.CorpusSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.CorpusSigorta, Script ="",UserNameHtml ="txtUsername",PasswordHtml = "txtPassword",SubmitButtonHtml="x-btn-text icon-accept",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.BereketSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.BereketSigorta, Script ="",UserNameHtml ="txtUsername",PasswordHtml = "txtPassword",SubmitButtonHtml="x-btn-text icon-accept",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.SekerSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.SekerSigorta, Script ="",UserNameHtml ="txtUsername",PasswordHtml = "txtPassword",SubmitButtonHtml="x-btn-text icon-accept",ButtonIndex="0", Delay = 500 } },
            { InsuranceConstants.AxaSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AxaSigorta, Script ="",UserNameHtml ="edtusername",PasswordHtml = "edtpassword",SubmitButtonHtml="x-btn-text icon-accept",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.AveonSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AveonSigorta, Script ="",UserNameHtml ="Username",PasswordHtml = "Password",SubmitButtonHtml="btn btn-primary",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.AnaSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AnaSigorta, Script ="",UserNameHtml ="Username",PasswordHtml = "Password",SubmitButtonHtml="btn btn-primary",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.DogaSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.DogaSigorta, Script ="",UserNameHtml ="Username",PasswordHtml = "Password",SubmitButtonHtml="btn btn-primary",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.KoruSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.KoruSigorta, Script ="",UserNameHtml ="Username",PasswordHtml = "Password",SubmitButtonHtml="btn btn-primary",ButtonIndex="0", Delay = 2000 } },
            //{ InsuranceConstants.AnaSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AnaSigorta, Script ="",UserNameHtml ="Username",PasswordHtml = "Password",SubmitButtonHtml="btn btn-primary",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.PriveSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.PriveSigorta, Script ="",UserNameHtml ="Username",PasswordHtml = "Password",SubmitButtonHtml="btn btn-primary",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.GriSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.GriSigorta, Script ="",UserNameHtml ="Username",PasswordHtml = "Password",SubmitButtonHtml="btn btn-primary",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.HdiSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.HdiSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="wr-btn grey-bg col-xs-12 col-md-12 col-lg-12 uppercase font-extra-large margin-bottom-double",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.GeneraliSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.GeneraliSigorta, Script ="",UserNameHtml ="login-username",PasswordHtml = "login-password",SubmitButtonHtml="btn newButton newButton btn-danger login-submit col-sm-6 ",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.TurkiyeKatilimSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.TurkiyeKatilimSigorta, Script ="",UserNameHtml ="Username",PasswordHtml = "Password",SubmitButtonHtml="btn btn-primary",ButtonIndex="0", Delay = 2000 } },



            { InsuranceConstants.OrientSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.OrientSigorta, Script ="",UserNameHtml ="txtUsername",PasswordHtml = "txtPassword",SubmitButtonHtml="x-btn-wrap x-btn x-btn-text-icon ",ButtonIndex="1", Delay = 2000 } },
            { InsuranceConstants.MagdeburgerSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.MagdeburgerSigorta, Script ="",UserNameHtml ="kullanici_adi",PasswordHtml = "kullanici_sifre",SubmitButtonHtml="btn btn-primary btn-block loginButton waves-effect waves-light",ButtonIndex="0", Delay =2000 } },
            { InsuranceConstants.NeovaSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.NeovaSigorta, Script ="",UserNameHtml ="ctl00_PlaceHolderMain_txtUsername",PasswordHtml = "ctl00_PlaceHolderMain_txtUsername",SubmitButtonHtml="btn newButton newButton btn-danger login-submit col-sm-6 ",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.TurkNipponSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.TurkNipponSigorta, Script ="",UserNameHtml ="Username",PasswordHtml = "Password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.TurkiyeSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.TurkiyeSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.AnkaraSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AnkaraSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.AnadoluSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.AnadoluSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.QuickSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.QuickSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.QuickSigorta2, new LoginScriptInfo {CompanyName = InsuranceConstants.QuickSigorta2, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.RaySigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.RaySigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.SompoJapanSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.SompoJapanSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.UnicoSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.UnicoSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 2000 } },
            { InsuranceConstants.ZurichSigorta, new LoginScriptInfo {CompanyName = InsuranceConstants.ZurichSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 3000 } },

            // Genel Login Scripti End

            { InsuranceConstants.Tramer, new LoginScriptInfo {CompanyName = InsuranceConstants.ZurichSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 3000 } },
            { InsuranceConstants.TramerSorgu, new LoginScriptInfo {CompanyName = InsuranceConstants.ZurichSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 3000 } },
            { InsuranceConstants.Ayarlar, new LoginScriptInfo {CompanyName = InsuranceConstants.ZurichSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 3000 } },
            { InsuranceConstants.QuickPortal, new LoginScriptInfo {CompanyName = InsuranceConstants.ZurichSigorta, Script ="",UserNameHtml ="username",PasswordHtml = "password",SubmitButtonHtml="loginSend",ButtonIndex="0", Delay = 3000 } },

        };
    };
}


