using System.Linq;
using System.Threading.Tasks;

namespace SigortaVip.Helpers
{
    internal class CreateOtpScript
    {
        public static async Task<string> GenerateOtpScriptAsync(string userName, string password, OtpScriptInfo otpScriptInfo)
        {
            var tokenHelper = new TokenHelper();
            string token = await tokenHelper.GetTokenAsync(userName, password);
            string script = string.Empty;
            if (string.IsNullOrEmpty(token))
            {
                return "// Token alınamadı.";
            }






            script = $@"(function () {{
                return new Promise(function (resolve) {{
                    const intervalTime = 2000;
                    const maxAttempts = 10;

                    let attempts = 0;
                    let interval = setInterval(() => {{
                        const tokenInput = document.getElementById('{otpScriptInfo.TokenHtml}');
                        let response = {{
                            step: 'otp-check',
                            tokenExists: !!tokenInput,
                            success: !!tokenInput,
                            message: ''
                        }};

                        if (tokenInput) {{
                            clearInterval(interval);

                            response.message = 'OTP input bulundu. Giriş yapılıyor...';

                            tokenInput.dispatchEvent(new Event('focus'));
                            tokenInput.value = '{token}';
                            tokenInput.dispatchEvent(new Event('input'));
                            tokenInput.dispatchEvent(new Event('change'));
                            tokenInput.dispatchEvent(new Event('blur'));

                            const submitButton = document.getElementsByClassName('{otpScriptInfo.SubmitButtonHtml}')[{otpScriptInfo.SubmitButtonIndex}];
                            if (submitButton) {{
                                setTimeout(() => {{
                                    submitButton.click();
                                    response.step = 'otp-submit-clicked';
                                    resolve(response);
                                }}, {otpScriptInfo.Delay});
                            }} else {{
                                response.success = false;
                                response.message = 'Submit butonu bulunamadı.';
                                resolve(response);
                            }}
                        }} else {{
                            attempts++;
                            if (attempts >= maxAttempts) {{
                                clearInterval(interval);
                                response.success = false;
                                response.message = 'OTP input belirlenen sürede bulunamadı.';
                                resolve(response);
                            }}
                        }}
                    }}, intervalTime);
                }});
            }})();";


            if (otpScriptInfo.CompanyName == "SompoJapanSigorta")
            {
                var chars = string.Join(",", token.Select(c => $"'{c}'")); // 'a','b','c','1','2','3' gibi
                script = $@"
                (function () {{
                    return new Promise(function (resolve) {{
                        document.getElementsByClassName(""login-type mb-3"")[0].click()
                        setTimeout(() => {{
                            let tokenChars = [{chars}];
                            let inputList = document.getElementsByTagName('input');

                            if (inputList.length === 0) {{
                                resolve({{ success: false, message: 'Input alanları bulunamadı.' }});
                                return;
                            }}

                            for (let i = 0; i < tokenChars.length && i < inputList.length; i++) {{
                                inputList[i].value = tokenChars[i];

                                let focusEvent = new Event('focus');
                                let changeEvent = new Event('change', {{ bubbles: true }});
                                let inputEvent = new Event('input');
                                let blurEvent = new Event('blur');

                                inputList[i].dispatchEvent(focusEvent);
                                inputList[i].dispatchEvent(changeEvent);
                                inputList[i].dispatchEvent(inputEvent);
                                inputList[i].dispatchEvent(blurEvent);
                            }}

                            resolve({{ success: true, message: 'OTP input\'ları başarıyla dolduruldu.' }});
                        }}, 15000); // 15 saniye sonra başlat
                    }});
                }})();";
            }
            if (otpScriptInfo.CompanyName == "UnicoSigorta")
            {
                script = $@"
                    (function() {{
                        return new Promise(function(resolve, reject) {{
                            let response = {{
                                step: 'otp-insert',
                                success: false,
                                message: ''
                            }};

                            try {{
                                let inputList = document.querySelectorAll('.outline-none');
                                if (inputList.length < 3) {{
                                    response.message = 'Beklenen input alanı bulunamadı.';
                                    reject(response); // Promise'yi reject et
                                    return;
                                }}

                                let inputElement = inputList[2];
                                let token = '{token}';  
                                console.log('Token: ', token); // Token'ı konsola yazdır

                                // Olaylar
                                const focusEvent = new Event('focus');
                                const changeEvent = new Event('change', {{ bubbles: true }});
                                const inputEvent = new Event('input', {{ bubbles: true }});
                                const blurEvent = new Event('blur', {{ bubbles: true }});

                                // Token'ı input alanına yerleştir
                                inputElement.value = token;
                                inputElement.dispatchEvent(focusEvent);
                                inputElement.dispatchEvent(changeEvent);
                                inputElement.dispatchEvent(inputEvent);
                                inputElement.dispatchEvent(blurEvent);

                                // Submit butonuna tıklanıyor
                                let submitButton = document.querySelectorAll('.flex.items-center.rounded-md.text-sm')[1];
                                if (submitButton) {{
                                    setTimeout(() => submitButton.click(), 100); // Butona tıkla
                                    response.success = true;
                                    response.message = 'OTP girildi ve butona tıklandı.';
                                    response.step = 'otp-submitted';
                                    resolve(response); // Promise'yi resolve et
                                }} else {{
                                    response.message = 'Submit butonu bulunamadı.';
                                    reject(response); // Promise'yi reject et
                                }}
                            }} catch (e) {{
                                response.message = 'Hata oluştu: ' + e.message;
                                reject(response); // Promise'yi reject et
                            }}
                        }});
                    }})();
                ";
            }

            if (otpScriptInfo.CompanyName == "HdiSigorta")
            {
                script = $@"
                    (function() {{
                        return new Promise(function(resolve, reject) {{
                            setTimeout(function() {{
                                var response = {{
                                    step: 'otp-insert',
                                    success: false,
                                    message: ''
                                }};

                                try {{
                                    var focusEvent = new Event('focus', {{ bubbles: true }});
                                    var changeEvent = new Event('change', {{ bubbles: true }});
                                    var inputEvent = new Event('input', {{ bubbles: true }});
                                    var blurEvent = new Event('blur', {{ bubbles: true }});

                                    var input = document.getElementsByName('token')[0];
                                    if (input) {{
                                        input.dispatchEvent(focusEvent);

                                        const nativeSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                                        nativeSetter.call(input, '{token}');

                                        input.dispatchEvent(inputEvent);
                                        input.dispatchEvent(changeEvent);
                                        input.dispatchEvent(blurEvent);
                                    }} else {{
                                        response.message = 'Token input bulunamadı.';
                                        response.step = 'input-not-found';
                                        reject(response);
                                        return;
                                    }}

                                    var containerList = document.getElementsByClassName('col-xs-2 col-sm-2 col-md-2 col-lg-2 form-group');
                                    if (containerList.length > 1 && containerList[1].children.length > 0) {{
                                        containerList[1].children[0].click();
                                        response.success = true;
                                        response.message = 'OTP girildi ve butona tıklandı.';
                                        response.step = 'otp-submitted';
                                        resolve(response);
                                    }} else {{
                                        response.message = 'Buton bulunamadı.';
                                        response.step = 'button-not-found';
                                        reject(response);
                                    }}
                                }} catch (err) {{
                                    response.message = 'Hata oluştu: ' + err.message;
                                    response.step = 'exception';
                                    reject(response);
                                }}
                            }}, 1000);
                        }});
                    }})();
                ";
            }

            if (otpScriptInfo.CompanyName == "RaySigorta")
            {
                script = $@"
                   (async () => {{
                      const script = document.createElement('script');
                      script.src = 'https://cdn.jsdelivr.net/npm/jsqr@1.4.0/dist/jsQR.min.js';
                      document.body.appendChild(script);

                      script.onload = async () => {{
                        const imgs = document.querySelectorAll('img');
                        if (!imgs.length) return console.error('Sayfada hiç <img> bulunamadı.');

                        for (const img of imgs) {{
                          if (!img.complete || img.naturalWidth === 0) continue;

                          const canvas = document.createElement('canvas');
                          canvas.width = img.naturalWidth;
                          canvas.height = img.naturalHeight;

                          const ctx = canvas.getContext('2d');
                          ctx.drawImage(img, 0, 0);

                          const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
                          const code = jsQR(imageData.data, imageData.width, imageData.height);

                          if (code) {{
                            console.log('✅ QR Kod bulundu!');
                            console.log('📦 İçerik:', code.data);
                            console.log('Kaynak Resim:', img.src);
                            return;
                          }}
                        }}

                        console.error('❌ QR kod hiçbir resimde bulunamadı.');
                      }};
                    }})();

                ";
            }

            if (otpScriptInfo.CompanyName == "NeovaSigorta")
            {
                script = $@"
                   // İlk setTimeout: Butonu bul ve tıkla
                   setTimeout(() => {{
                       var redirectButtonToOtp = document.getElementById('nv-login');
                       console.log('Button found:', redirectButtonToOtp);
                       if (redirectButtonToOtp && redirectButtonToOtp.href) {{
                           console.log('Redirecting to:', redirectButtonToOtp.href);
                           window.location.href = redirectButtonToOtp.href;
                       }} else {{
                           console.log('Button or href not found');
                       }}
                   }}, 1000);
   
                   // İkinci setTimeout: Token işlemlerini yap
                   setTimeout(() => {{
                       console.log('Starting token input after 4 seconds...');
                       const tokenInput = document.getElementById('{otpScriptInfo.TokenHtml}');
                       if (tokenInput) {{
                           console.log('Token input found:', tokenInput);
                           tokenInput.dispatchEvent(new Event('focus'));
                           tokenInput.value = '{token}';
                           tokenInput.dispatchEvent(new Event('input'));
                           tokenInput.dispatchEvent(new Event('change'));
                           tokenInput.dispatchEvent(new Event('blur'));
                           console.log('Token entered:', '{token}');
                       }} else {{
                           console.log('Token input not found');
                       }}
                   }}, 4000);
   
                   // Üçüncü setTimeout: Submit butonuna tıkla
                   setTimeout(() => {{
                       console.log('Clicking submit button after 5 seconds...');
                       const submitButton = document.getElementById('btnValidateTwoFactor');
                       if (submitButton) {{
                           console.log('Submit button found:', submitButton);
                           submitButton.click();
                           console.log('Submit button clicked');
                       }} else {{
                           console.log('Submit button not found');
                       }}
                   }}, 5000);
                ";
            }




            return script;
        }

    }
}
