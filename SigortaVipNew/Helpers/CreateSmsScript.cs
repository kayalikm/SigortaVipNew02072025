using System;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaVip.Helpers
{
    internal class CreateSmsScript
    {
        public static async Task<string> GenerateSmsScriptAsync(string userName, string password, SmsScriptInfo smsScriptInfo)
        {
            string token = await SmsHelper.GetSmsAsync(userName, password);
            string script = string.Empty;


            //token = "123456";


            if (smsScriptInfo.CompanyName == "AkSigorta")
            {
                return $@"
                    (function() {{
                        function setNativeValue(element, value) {{
                            const valueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                            valueSetter.call(element, value);
                            element.dispatchEvent(new Event('input', {{ bubbles: true }}));
                            element.dispatchEvent(new Event('change', {{ bubbles: true }}));
                            element.dispatchEvent(new Event('blur', {{ bubbles: true }}));
                        }}

                        function runSmsScript() {{
                            var response = {{
                                step: 'sms',
                                success: false,
                                message: ''
                            }};

                            try {{
                                var tokenInput = document.getElementById('{smsScriptInfo.TokenHtml}');
                                if (tokenInput) {{
                                    setNativeValue(tokenInput, '{token}');

                                    var submitButtons = document.getElementsByClassName('{smsScriptInfo.SubmitButtonHtml}');
                                    var submitButton = submitButtons[{smsScriptInfo.SubmitButtonIndex}];

                                    if (submitButton) {{
                                        setTimeout(function() {{
                                            submitButton.click();
                                        }}, {smsScriptInfo.Delay});

                                        response.success = true;
                                        response.message = 'SMS token girildi ve gönderme butonuna tıklandı.';
                                        response.step = 'sms-submit-clicked';
                                    }} else {{
                                        response.message = 'Submit butonu bulunamadı.';
                                        response.step = 'submit-not-found';
                                    }}
                                }} else {{
                                    response.message = 'Token input bulunamadı.';
                                    response.step = 'token-not-found';
                                }}
                            }} catch (err) {{
                                response.message = 'Hata oluştu: ' + err.message;
                                response.step = 'exception';
                            }}

                            return JSON.stringify(response);
                        }}

                        if (document.readyState === 'complete' || document.readyState === 'interactive') {{
                            return runSmsScript();
                        }} else {{
                            return JSON.stringify({{ success: false, message: 'Sayfa yüklenmesi bekleniyor.', step: 'waiting-dom' }});
                        }}
                    }})();
                ";
            }




            if (smsScriptInfo.CompanyName == "HepIyiSigorta")

            {

                script = $@"setTimeout(function() {{
                var focusEvent = new Event('focus');
                var changeEvent = new Event('change');
                var inputEvent = new Event('input');
                var blurEvent = new Event('blur');

                var tokenInput = document.getElementById('{smsScriptInfo.TokenHtml}');
                if (tokenInput) {{
                    tokenInput.dispatchEvent(focusEvent);
                    tokenInput.value = '{token}';
                    tokenInput.dispatchEvent(changeEvent);
                    tokenInput.dispatchEvent(inputEvent);
                    tokenInput.dispatchEvent(blurEvent);
                    }}

                    var submitButtons = document.getElementsByClassName('{smsScriptInfo.SubmitButtonHtml}');
                    if (submitButtons.length > 0) {{
                        submitButtons[0].click();
                    }}
                }}, {smsScriptInfo.Delay});";

            }

            if (smsScriptInfo.CompanyName == "AnadoluSigorta")
            {
                script = $@"
                    (function() {{
                        function triggerEvents(el, value) {{
                            if (!el) return;

                            const nativeInputValueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                            nativeInputValueSetter.call(el, value);

                            el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                            el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                            el.dispatchEvent(new Event('blur', {{ bubbles: true }}));
                        }}

                        function runSmsScript() {{
                            let response = {{
                                step: 'start',
                                success: false,
                                message: ''
                            }};

                            const sendButton = document.getElementById('btnSendSms');
                            if (sendButton) {{
                                sendButton.click();
                                response.step = 'send-clicked';
                                response.message = 'Yeni Kod Gönder butonuna tıklandı.';

                                setTimeout(function() {{
                                    const tokenInput = document.getElementById('otpCode');
                                    const loginButton = document.getElementById('btnLogin');

                                    if (tokenInput) {{
                                        triggerEvents(tokenInput, '{token}');
                                        response.success = true;
                                        response.step = 'token-set';
                                        response.message = 'Token input yazıldı.';

                                        setTimeout(function() {{
                                            if (loginButton && !loginButton.disabled) {{
                                                loginButton.click();
                                                console.log('Login butonuna tıklandı');
                                            }} else {{
                                                console.log('Login butonu pasif veya yok');
                                            }}
                                        }}, 1000);

                                    }} else {{
                                        response.message = 'Token input bulunamadı.';
                                        response.step = 'token-not-found';
                                    }}

                                }}, 5000); // SMS geldikten sonra bekleme süresi

                            }} else {{
                                response.message = 'Gönder butonu (btnResendCode) bulunamadı.';
                                response.step = 'send-not-found';
                            }}

                            return JSON.stringify(response);
                        }}

                        if (document.readyState === 'complete' || document.readyState === 'interactive') {{
                            setTimeout(runSmsScript, {smsScriptInfo.Delay});
                        }} else {{
                            document.addEventListener('DOMContentLoaded', function() {{
                                setTimeout(runSmsScript, {smsScriptInfo.Delay});
                            }});
                        }}

                        return JSON.stringify({{ success: false, message: 'Sayfa yüklenmesi bekleniyor.' }});
                    }})();";
            }



            Console.WriteLine($"Sms Script: {script}");

            return script;
        }

    }
}
