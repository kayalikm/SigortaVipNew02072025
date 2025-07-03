
using System;


namespace SigortaVip.Helpers
{
    internal class CreateLoginScript
    {
        public static string GenerateLoginScript(string userName, string password, LoginScriptInfo smsScriptInfo, int delay)
        {
         
            // Ankara SİGORTA Start
            if (smsScriptInfo.CompanyName == "AnkaraSigorta")
            {
                return $@"(function() {{
                    return new Promise(function(resolve) {{
                        function runLoginScript() {{
                            const usernameCandidates = Array.from(document.querySelectorAll('.input-group'))[0]?.querySelectorAll('.form-control') || [];
                            const passwordCandidates = Array.from(document.querySelectorAll('.input-group'))[1]?.querySelectorAll('.form-control') || [];

                            const usernameInput = Array.from(usernameCandidates).filter(x => !x.classList.contains('hidden') && x.offsetParent !== null).sort((a, b) => b.clientWidth - a.clientWidth)[0];
                            const passwordInput = Array.from(passwordCandidates).filter(x => !x.classList.contains('hidden') && x.offsetParent !== null).sort((a, b) => b.clientWidth - a.clientWidth)[0];

                            let response = {{
                                step: 'login-check',
                                usernameExists: !!usernameInput,
                                passwordExists: !!passwordInput,
                                success: !!usernameInput && !!passwordInput,
                                message: ''
                            }};

                            if (response.success) {{
                                response.message = 'Username ve password inputları bulundu.';

                                usernameInput.dispatchEvent(new Event('focus'));
                                usernameInput.value = '{userName}';
                                usernameInput.dispatchEvent(new Event('change'));
                                usernameInput.dispatchEvent(new Event('input'));
                                usernameInput.dispatchEvent(new Event('blur'));

                                passwordInput.dispatchEvent(new Event('focus'));
                                passwordInput.value = '{password}';
                                passwordInput.dispatchEvent(new Event('change'));
                                passwordInput.dispatchEvent(new Event('input'));
                                passwordInput.dispatchEvent(new Event('blur'));

                                const submitButton = document.querySelector('.login-action button[type=""submit""]');
                                if (submitButton) {{
                                    console.log('Giriş butonuna tıklanıyor...');
                                }} else {{
                                    response.success = false;
                                    response.message = 'Giriş butonu bulunamadı.';
                                }}
                            }} else {{
                                response.message = 'Inputlar bulunamadı.';
                            }}

                            resolve(response);
                        }}

                        if (document.readyState === 'complete' || document.readyState === 'interactive') {{
                            setTimeout(runLoginScript, {delay});
                        }} else {{
                            document.addEventListener('DOMContentLoaded', () => {{
                                setTimeout(runLoginScript, {delay});
                            }});
                        }}
                    }});
                }})();";
            }

            // Ankara SİGORTA End


            // QUICK SİGORTA Start
            if (smsScriptInfo.CompanyName == "QuickSigorta")
            {
                return $@"(function() {{
                    return new Promise(function(resolve) {{
                        document.addEventListener('fillInputs', function() {{
                            let input = document.getElementsByName('username')[0];
                            if (!input) return;

                            let nativeInputValueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                            nativeInputValueSetter.call(input, '{userName}');

                            input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                            input.dispatchEvent(new Event('change', {{ bubbles: true }}));

                            let inputPassword = document.getElementsByName('password')[0];
                            if (!inputPassword) return;

                            nativeInputValueSetter.call(inputPassword, '{password}');
                            inputPassword.dispatchEvent(new Event('input', {{ bubbles: true }}));
                            inputPassword.dispatchEvent(new Event('change', {{ bubbles: true }}));

                            let submitButton = document.querySelector('button[type=""submit""]');
                            if (submitButton) {{
                                submitButton.click();
                            }}

                            resolve({{ success: true, message: 'Kullanıcı adı ve şifre girildi, butona tıklandı.' }});
                        }});

                        setTimeout(function() {{
                            document.dispatchEvent(new Event('fillInputs'));
                        }}, 3000);

                        document.addEventListener('fillPhone', function () {{
                            let phoneInput = document.getElementsByName('phone')[0];
                            if (!phoneInput) return;

                            let openButton = document.querySelector('.MuiAutocomplete-popupIndicator');
                            if (openButton) openButton.click();

                            let tryCount = 0;
                            let interval = setInterval(function () {{
                                let listItems = document.querySelectorAll('li[role=""option""]');
                                if (listItems.length > 0) {{
                                    listItems[0].click();
                                    clearInterval(interval);

                                    let submitButton = document.querySelector('button[type=""submit""]');
                                    if (submitButton) submitButton.click();

                                    resolve({{ success: true, message: 'Telefon seçimi yapıldı ve submit edildi.' }});
                                }}
                                if (++tryCount > 10) {{
                                    clearInterval(interval);
                                    resolve({{ success: false, message: 'Telefon seçimi yapılamadı.' }});
                                }}
                            }}, 300);
                        }});

                        setTimeout(function () {{
                            document.dispatchEvent(new Event('fillPhone'));
                        }}, 5000);
                    }});
                }})();";
            }
            // SomeReactBasedCompany Start
            if (smsScriptInfo.CompanyName == "QuickSigorta2")
            {
                return $@"(function() {{
                    return new Promise(function(resolve) {{
                        document.addEventListener('fillInputs', function() {{
                            let input = document.getElementsByName('username')[0];
                            if (!input) return;

                            let nativeInputValueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                            nativeInputValueSetter.call(input, '{userName}');

                            input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                            input.dispatchEvent(new Event('change', {{ bubbles: true }}));

                            let inputPassword = document.getElementsByName('password')[0];
                            if (!inputPassword) return;

                            nativeInputValueSetter.call(inputPassword, '{password}');
                            inputPassword.dispatchEvent(new Event('input', {{ bubbles: true }}));
                            inputPassword.dispatchEvent(new Event('change', {{ bubbles: true }}));

                            let submitButton = document.querySelector('button[type=""submit""]');
                            if (submitButton) {{
                                submitButton.click();
                            }}

                            resolve({{ success: true, message: 'Kullanıcı adı ve şifre girildi, butona tıklandı.' }});
                        }});

                        setTimeout(function() {{
                            document.dispatchEvent(new Event('fillInputs'));
                        }}, 3000);

                        document.addEventListener('fillPhone', function () {{
                            let phoneInput = document.getElementsByName('phone')[0];
                            if (!phoneInput) return;

                            let openButton = document.querySelector('.MuiAutocomplete-popupIndicator');
                            if (openButton) openButton.click();

                            let tryCount = 0;
                            let interval = setInterval(function () {{
                                let listItems = document.querySelectorAll('li[role=""option""]');
                                if (listItems.length > 0) {{
                                    listItems[0].click();
                                    clearInterval(interval);

                                    let submitButton = document.querySelector('button[type=""submit""]');
                                    if (submitButton) submitButton.click();

                                    resolve({{ success: true, message: 'Telefon seçimi yapıldı ve submit edildi.' }});
                                }}
                                if (++tryCount > 10) {{
                                    clearInterval(interval);
                                    resolve({{ success: false, message: 'Telefon seçimi yapılamadı.' }});
                                }}
                            }}, 300);
                        }});

                        setTimeout(function () {{
                            document.dispatchEvent(new Event('fillPhone'));
                        }}, 5000);
                    }});
                }})();";
            }
            if (smsScriptInfo.CompanyName == "TurkiyeSigorta")
            {
                return $@"(function() {{
                    return new Promise(function(resolve) {{
                        setTimeout(function() {{
                            function setNativeValue(element, value) {{
                                const valueSetter = Object.getOwnPropertyDescriptor(element.__proto__, 'value').set;
                                valueSetter.call(element, value);
                                element.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                element.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                element.dispatchEvent(new Event('blur', {{ bubbles: true }}));
                            }}

                            let usernameInput = document.querySelector('input[name=""username""]');
                            let passwordInput = document.querySelector('input[name=""password""]');

                            let response = {{
                                step: 'login-check',
                                usernameExists: !!usernameInput,
                                passwordExists: !!passwordInput,
                                success: !!usernameInput && !!passwordInput,
                                message: ''
                            }};

                            if (response.success) {{
                                response.message = 'Inputlar bulundu.';

                                usernameInput.focus();
                                setNativeValue(usernameInput, '{userName}');

                                passwordInput.focus();
                                setNativeValue(passwordInput, '{password}');

                                let submitButton = document.querySelector('button[type=""submit""]');
                                if (submitButton) {{
                                    submitButton.click();
                                }} else {{
                                    response.success = false;
                                    response.message = 'Submit butonu bulunamadı.';
                                }}
                            }} else {{
                                response.message = 'Username veya password inputları bulunamadı.';
                            }}

                            resolve(response);
                        }}, 2000);
                    }});
                }})();";
            }




            if (smsScriptInfo.CompanyName == "RaySigorta")
            {
                return $@"(function() {{
                    return new Promise(function(resolve) {{
                        setTimeout(function() {{
                            function setNativeValue(el, value) {{
                                const setter = Object.getOwnPropertyDescriptor(el.__proto__, 'value').set;
                                setter.call(el, value);
                                el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                el.dispatchEvent(new Event('blur', {{ bubbles: true }}));
                                el.dispatchEvent(new KeyboardEvent('keyup', {{ bubbles: true }}));
                            }}

                            let username = document.getElementById('username');
                            let password = document.getElementById('password');
                            let button = document.getElementById('GirisYap');

                            let result = {{
                                step: 'login',
                                success: false,
                                message: ''
                            }};

                            if (username && password) {{
                                setNativeValue(username, '{userName}');
                                setNativeValue(password, '{password}');
                    
                                let retry = 0;
                                let interval = setInterval(function() {{
                                    if (!button.disabled) {{
                                        button.click();
                                        clearInterval(interval);
                                        result.success = true;
                                        result.message = 'Giriş yapıldı.';
                                        resolve(result);
                                    }} else if (++retry > 10) {{
                                        clearInterval(interval);
                                        result.message = 'Giriş butonu aktif olmadı.';
                                        resolve(result);
                                    }}
                                }}, 300);
                            }} else {{
                                result.message = 'Kullanıcı adı veya şifre alanı bulunamadı.';
                                resolve(result);
                            }}
                        }}, 2000);
                    }});
                }})();";
            }






            if (smsScriptInfo.CompanyName == "ZurichSigorta")
            {
                return $@"(function() {{
                    // Sayfa yüklenmeden önce alert'i engelle
                    window.alert = function() {{}}; // alert'i geçersiz kıl

                    return new Promise(function(resolve) {{
                        setTimeout(function() {{
                            function setNativeValue(el, value) {{
                                const valueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                                valueSetter.call(el, value);
                                el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                            }}

                            let result = {{
                                step: 'login',
                                success: false,
                                message: ''
                            }};

                            let usernameInput = document.querySelector('#txtUsername');
                            let passwordInput = document.querySelector('#txtPassword');
                            let loginButton = document.querySelector('#btnLogin');

                            if (usernameInput && passwordInput && loginButton) {{
                                setNativeValue(usernameInput, '{userName}');
                                setNativeValue(passwordInput, '{password}');

                                let tryCount = 0;
                                let interval = setInterval(function() {{
                                    if (!loginButton.disabled) {{
                                        loginButton.click();
                                        clearInterval(interval);
                                        result.success = true;
                                        result.message = 'Giriş yapıldı';
                                        resolve(result);
                                    }} else if (++tryCount > 10) {{
                                        clearInterval(interval);
                                        result.message = 'Giriş butonu aktif olmadı';
                                        resolve(result);
                                    }}
                                }}, 300);
                            }} else {{
                                result.message = 'Giriş alanları bulunamadı';
                                resolve(result);
                            }}
                        }}, 3000);
                    }});
                }})();";
            }
            if (smsScriptInfo.CompanyName == "SompoJapanSigorta")
            {
                return $@"(function() {{
                    function setNativeValue(element, value) {{
                        const valueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                        valueSetter.call(element, value);
                        element.dispatchEvent(new Event('input', {{ bubbles: true }}));
                        element.dispatchEvent(new Event('change', {{ bubbles: true }}));
                        element.dispatchEvent(new Event('blur', {{ bubbles: true }}));
                    }}

                    function runLoginScript() {{
                        let usernameInput = document.querySelector('input[placeholder=""Kullanıcı Adı""]');
                        let passwordInput = document.querySelector('input[placeholder=""Parola""]');

                        let response = {{
                            step: 'login-check',
                            usernameExists: !!usernameInput,
                            passwordExists: !!passwordInput,
                            success: !!usernameInput && !!passwordInput,
                            message: ''
                        }};

                        if (response.success) {{
                            setNativeValue(usernameInput, '{userName}');
                            setNativeValue(passwordInput, '{password}');

                            let submitButton = document.querySelector('button[type=""submit""]');
                            if (submitButton) {{
                                submitButton.click();
                                response.message = 'Form gönderildi.';
                                response.step = 'login-submit-clicked';
                            }} else {{
                                response.success = false;
                                response.message = 'Gönder butonu bulunamadı.';
                                response.step = 'submit-not-found';
                            }}
                        }} else {{
                            response.message = 'Inputlar bulunamadı.';
                        }}

                        return JSON.stringify(response);
                    }}

                    if (document.readyState === 'complete' || document.readyState === 'interactive') {{
                        return runLoginScript();
                    }} else {{
                        document.addEventListener('DOMContentLoaded', function() {{
                            runLoginScript();
                        }});
                        return JSON.stringify({{ success: false, message: 'Sayfa yüklenmesi bekleniyor.' }});
                    }}
                }})();";
            }
            if (smsScriptInfo.CompanyName == "NeovaSigorta")
            {
                return $@"
                    (function() {{
                        return new Promise(function(resolve) {{
                            setTimeout(function() {{
                                var response = {{
                                    step: 'login',
                                    success: false,
                                    message: ''
                                }};

                                try {{
                                    var focusEvent = new Event('focus', {{ bubbles: true }});
                                    var changeEvent = new Event('change', {{ bubbles: true }});
                                    var inputEvent = new Event('input', {{ bubbles: true }});
                                    var blurEvent = new Event('blur', {{ bubbles: true }});

                                    var usernameInput = document.querySelector('[id*=""txtUsername""]');
                                    var passwordInput = document.querySelector('[id*=""txtPassword""]');

                                    if (usernameInput && passwordInput) {{
                                        const nativeSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;

                                        // Username
                                        usernameInput.dispatchEvent(focusEvent);
                                        nativeSetter.call(usernameInput, '{userName}');
                                        usernameInput.dispatchEvent(changeEvent);
                                        usernameInput.dispatchEvent(inputEvent);
                                        usernameInput.dispatchEvent(blurEvent);

                                        // Password
                                        passwordInput.dispatchEvent(focusEvent);
                                        nativeSetter.call(passwordInput, '{password}');
                                        passwordInput.dispatchEvent(changeEvent);
                                        passwordInput.dispatchEvent(inputEvent);
                                        passwordInput.dispatchEvent(blurEvent);

                                        var submitButton = document.querySelector('button[aria-label=""Giriş Yap""]');
                                        if (submitButton) {{
                                            submitButton.click();
                                            response.success = true;
                                            response.message = 'Form gönderildi';
                                            response.step = 'login-submit-clicked';
                                        }} else {{
                                            response.message = 'Giriş butonu bulunamadı';
                                            response.step = 'submit-not-found';
                                        }}
                                    }} else {{
                                        response.message = 'Kullanıcı adı veya parola inputu bulunamadı';
                                        response.step = 'input-not-found';
                                    }}
                                }} catch (err) {{
                                    response.message = 'Hata oluştu: ' + err.message;
                                    response.step = 'exception';
                                }}

                                resolve(response);
                            }}, 4000); // 4 saniye bekleme
                        }});
                    }})();
                ";
            }

            if (smsScriptInfo.CompanyName == "UnicoSigorta")
            {
                return $@"(function() {{
                    function setNativeValue(element, value) {{
                        const valueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                        valueSetter.call(element, value);
                        element.dispatchEvent(new Event('input', {{ bubbles: true }}));
                        element.dispatchEvent(new Event('change', {{ bubbles: true }}));
                        element.dispatchEvent(new Event('blur', {{ bubbles: true }}));
                    }}

                    function runLoginScript() {{
                        let inputs = document.querySelectorAll('input[type=text], input[type=password]');
                        let usernameInput = inputs[0];
                        let passwordInput = inputs[1];
                        let submitButton = document.querySelector('button[aria-label=""Giriş Yap""]');

                        let response = {{
                            step: 'login-check',
                            usernameExists: !!usernameInput,
                            passwordExists: !!passwordInput,
                            success: !!usernameInput && !!passwordInput,
                            message: ''
                        }};

                        if (response.success) {{
                            setNativeValue(usernameInput, '{userName}');
                            setNativeValue(passwordInput, '{password}');

                            if (submitButton) {{
                                submitButton.click();
                                response.message = 'Form gönderildi.';
                                response.step = 'login-submit-clicked';
                            }} else {{
                                response.success = false;
                                response.message = 'Gönder butonu bulunamadı.';
                                response.step = 'submit-not-found';
                            }}
                        }} else {{
                            response.message = 'Inputlar bulunamadı.';
                        }}

                        return JSON.stringify(response);
                    }}

                    if (document.readyState === 'complete' || document.readyState === 'interactive') {{
                        return runLoginScript();
                    }} else {{
                        document.addEventListener('DOMContentLoaded', function() {{
                            runLoginScript();
                        }});
                        return JSON.stringify({{ success: false, message: 'Sayfa yüklenmesi bekleniyor.' }});
                    }}
                }})();";
            }

            if (smsScriptInfo.CompanyName == "AnadoluSigorta")
            {
                return $@"(function() {{
                    function getReactProps(el) {{
                        const keys = Object.keys(el);
                        const propKey = keys.find((key) => key.includes('reactProps'));
                        return el[propKey];
                    }}

                    function runLoginScript() {{
                        let response = {{
                            step: 'login-check',
                            success: false,
                            message: ''
                        }};

                        try {{
                            const usernameEl = document.getElementById('userName');
                            const passwordEl = document.getElementById('password');
                            const loginBtn = document.getElementById('btnLogin');

                            if (usernameEl && passwordEl) {{
                                const usernameProps = getReactProps(usernameEl);
                                const passwordProps = getReactProps(passwordEl);

                                usernameProps?.onChange({{ target: {{ value: '{userName}' }} }});
                                usernameProps?.onBlur();

                                passwordProps?.onChange({{ target: {{ value: '{password}' }} }});
                                passwordProps?.onBlur();

                                const tryClick = setInterval(() => {{
                                    if (loginBtn && loginBtn.disabled === false) {{
                                        loginBtn.click();
                                        clearInterval(tryClick);
                                    }}
                                }}, 1000);

                                response.success = true;
                                response.message = 'Bilgiler girildi, buton kontrol ediliyor.';
                                response.step = 'login-submit-waiting';
                            }} else {{
                                response.message = 'Input elemanları bulunamadı.';
                                response.step = 'input-not-found';
                            }}
                        }} catch (err) {{
                            response.message = 'Hata oluştu: ' + err.message;
                            response.step = 'exception';
                        }}

                        return JSON.stringify(response);
                    }}

                    if (document.readyState === 'complete' || document.readyState === 'interactive') {{
                        setTimeout(runLoginScript, {smsScriptInfo.Delay});
                    }} else {{
                        document.addEventListener('DOMContentLoaded', function() {{
                            setTimeout(runLoginScript, {smsScriptInfo.Delay});
                        }});
                    }}

                    return JSON.stringify({{ success: true, step: 'passed', message: 'Sayfa yüklenmesi bekleniyor.' }});
                }})();";
            }

            // ANADOLU SİGORTA End
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

                        function runLoginScript() {{
                            var response = {{
                                step: 'login',
                                success: false,
                                message: ''
                            }};

                            try {{
                                var usernameInput = document.querySelector('[id*=""txtUsername""]') || document.getElementById('fpf-username');
                                var passwordInput = document.querySelector('[id*=""txtPassword""]') || document.getElementById('fpf-password');
                                var submitButton = document.querySelector('button[aria-label=""Giriş Yap""]') || document.getElementById('lf-giris-yap');

                                if (usernameInput && passwordInput) {{
                                    setNativeValue(usernameInput, '{userName}');
                                    setNativeValue(passwordInput, '{password}');

                                    if (submitButton) {{
                                        submitButton.click();
                                        response.success = true;
                                        response.message = 'Form gönderildi';
                                        response.step = 'login-submit-clicked';
                                    }} else {{
                                        response.message = 'Giriş butonu bulunamadı';
                                        response.step = 'submit-not-found';
                                    }}
                                }} else {{
                                    response.message = 'Kullanıcı adı veya parola inputu bulunamadı';
                                    response.step = 'input-not-found';
                                }}
                            }} catch (err) {{
                                response.message = 'Hata oluştu: ' + err.message;
                                response.step = 'exception';
                            }}

                            return JSON.stringify(response);
                        }}

                        if (document.readyState === 'complete' || document.readyState === 'interactive') {{
                            return runLoginScript();
                        }} else {{
                            return JSON.stringify({{ success: false, message: 'Sayfa yüklenmesi bekleniyor.', step: 'waiting-dom' }});
                        }}
                    }})();
                ";
            }



            // GENERIC FORM
            // GENERIC FORM
            return $@"(function() {{
                    function runLoginScript() {{
                        const usernameInput = document.getElementById('{smsScriptInfo.UserNameHtml}');
                        const passwordInput = document.getElementById('{smsScriptInfo.PasswordHtml}');

                        let response = {{
                            step: 'login-check',
                            usernameExists: !!usernameInput,
                            passwordExists: !!passwordInput,
                            success: !!usernameInput && !!passwordInput,
                            message: ''
                        }};

                        if (response.success) {{
                            response.message = 'Username ve password inputları bulundu.';

                            usernameInput.value = '{userName}';
                            passwordInput.value = '{password}';

                            const submitButton = document.getElementsByClassName('{smsScriptInfo.SubmitButtonHtml}')[{smsScriptInfo.ButtonIndex}];

                            response.step = submitButton ? 'login-submit-clicked' : 'submit-not-found';
                            response.success = !!submitButton;
                            response.message = submitButton ? 'Buton bulundu, tıklanacak.' : 'Buton bulunamadı.';

                            if (submitButton) {{
                                setTimeout(() => submitButton.click(), 2000);
                            }}

                        }} else {{
                            response.success = false;
                            response.message = 'Inputlar bulunamadı.';
                        }}

                        return JSON.stringify(response);  // Burada sonucu hemen döndürüyoruz.
                    }}

                    if (document.readyState === 'complete' || document.readyState === 'interactive') {{
                        return runLoginScript();  // Sayfa hazır olduğunda sonucu döndürüyoruz.
                    }} else {{
                        document.addEventListener('DOMContentLoaded', () => {{
                            return runLoginScript();  // Sayfa yüklendiğinde sonucu döndürüyoruz.
                        }});
                    }}
                }})();";


        }
    }
}
