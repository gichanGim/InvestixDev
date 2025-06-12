$(document).ready(function () {
    const $modifyKeySection = $('#modify-hidden-key');
    const $modifySecretSection = $('#modify-hidden-secret');
    const $overlay = $('#overlay');
    const $modifyKeyBtn = $('#modify-key-btn');
    const $modifySecretBtn = $('#modify-secret-btn');
    const $inputModifyKey = $('#input-modifykey');
    const $inputModifySecret = $('#input-modifysecret');
    const $ModifyKeyBtnReal = $('#modify-key-btn-real');
    const $ModifySecretBtnReal = $('#modify-secret-btn-real');

    $modifyKeyBtn.on('click', function () {
        $modifyKeySection.show();
        $overlay.show();
        $inputModifyKey.focus(); 
    });

    $inputModifyKey.on('blur', function () {
        setTimeout(() => {
            $modifyKeySection.hide();
            $overlay.hide();
            $inputModifyKey.val(''); // 필요 시 초기화
        }, 150);
    });

    $ModifyKeyBtnReal.on('click', function () {
        var appkey = $inputModifyKey.val();
        ModfiyAppKey(appkey);
        window.alert('APPKEY 변경완료.');

        $modifyKeySection.hide();
        $overlay.hide();
        $inputModifyKey.val('');
    });

    $modifySecretBtn.on('click', function () {
        $modifySecretSection.show();
        $overlay.show();
        $inputModifySecret.focus(); 
    });

    $inputModifySecret.on('blur', function () {
        setTimeout(() => {
            $modifySecretSection.hide();
            $overlay.hide();
            $inputModifySecret.val(''); // 필요 시 초기화
        }, 150);
    });

    $ModifySecretBtnReal.on('click', function () {
        var appsecret = $inputModifySecret.val();
        ModfiyAppSecret(appsecret);
        window.alert('APPSECRET 변경완료.');

        $modifySecretSection.hide();
        $overlay.hide();
        $inputModifySecret.val('');
    });

    function ModfiyAppKey(appkey) {
        $.ajax({
            url: "/Auth/ModifyAppKey",
            type: "POST",
            data: { appkey: appkey },
            success: function (response) {
                if (response.success) {
                    window.location.href = response.redirectUrl;
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("서버 요청 중 오류가 발생했습니다.");
            }
        });
    }

    function ModfiyAppSecret(appsecret) {
        $.ajax({
            url: "/Auth/ModifyAppSecret",
            type: "POST",
            data: { appsecret: appsecret },
            success: function (response) {
                if (response.success) {
                    window.location.href = response.redirectUrl;
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("서버 요청 중 오류가 발생했습니다.");
            }
        });
    }
});
