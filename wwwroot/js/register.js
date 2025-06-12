let isTimerExpired = false; 

// 아이디 중복 여부 체크
$('#checkIdbtn').on('click', function () {                  
    var userId = document.getElementById('UserIdInput').value;
    $.ajax({
        url: '/Account/CheckIdAvailable', // 컨트롤러 액션 주소
        type: 'POST',
        data: {
            id: userId // 필요한 데이터
        },
        success: function (response) {
            $('#isIdAvailableSave').val("true");

            let alertClass = response === true ? 'text-primary' : 'text-danger';
            let message = response === true ? '✔ 가입 가능한 아이디입니다.' : '✖ 이미 가입된 아이디입니다.';

            $('#checkIdResult').html(`
                    <div class="${alertClass}">
                        ${message}
                    </div>
                `);
        },
        error: function (xhr, status, error) {
            $('#isIdAvailableSave').val("false");
            window.alert("아이디를 입력해주세요." + error);
        }
    });
});
// 휴대번호 중복 여부 체크
$('#sendVerificationBtn').on('click', function () {
    var phone = document.getElementById('PhoneNumInput').value;
    var phonePattern = /^\d{10,11}$/;

    if (!phonePattern.test(phone)) {
        return; // 정규식 검사 실패 시 Ajax 요청 중단
    }

    $.ajax({
        url: '/Account/CheckPhoneAvailable', // 컨트롤러 액션 주소
        type: 'POST',
        data: {
            phone: phone // 필요한 데이터
        },
        success: function (response) {
            if (response === true) {        // 중복된 번호 없을 시 인증 문자 전송
                $.ajax({
                    url: '/Account/SendVerification',
                    type: 'POST',
                    data: { phone: phone },
                    success: function (res) {
                        if (res.success === true) {  // 인증 문자 전송 성공
                            $('#checkPhoneResult').html(`
                                <div class="text-primary">
                                    인증 문자가 전송되었습니다.
                                </div>
                `           );

                            $('#verifySaltSave').val(res.salt); // 인증 문자열 임시 저장

                            isTimerExpired = false;
                            let duration = 180; // 3분 = 180초
                            const input = document.getElementById('verifyTimer');

                            // 타이머 함수
                            const timer = setInterval(() => {
                                const minutes = Math.floor(duration / 60);
                                const seconds = duration % 60;

                                // mm:ss 형식으로 설정
                                input.textContent = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;

                                duration--;

                                // 타이머 종료
                                if (duration < 0) {
                                    isTimerExpired = true; // 타이머 끝난 후 메시지
                                }
                            }, 1000);
                        }
                        else {                                          // 인증 문자 전송 실패
                            $('#checkPhoneResult').html(`
                                <div class="text-danger">
                                    ✖ ${res.statusMessage}
                                </div>
                `           );
                        }
                    },
                    error: function () {
                        window.alert("서버 오류(인증 문자 전송): " + error);
                    }
                });
            }
            else {
                $('#checkPhoneResult').html(`
                    <div class="text-danger">
                        '✖이미 가입된 휴대번호입니다.'
                    </div>
                `);
            }
        },
        error: function (xhr, status, error) {
            window.alert("서버 오류(휴대번호 중복 체크): " + error);
        }
    });
});

// 인증 문자열 확인
$('#verifySaltbtn').on('click', function () {
    var salt = document.getElementById('verifySaltInput').value;
    var savedSalt = document.getElementById('verifySaltSave').value;

    if (salt.trim() !== '') {
        $.ajax({
            url: '/Account/VerifySalt', // 컨트롤러 액션 주소
            type: 'POST',
            data: {
                salt: salt, // 필요한 데이터
                savedSalt: savedSalt
            },
            success: function (response) {
                if (response.success === true) {
                    $('#IsPhoneVerifiedSave').val("true");

                    $('#checkSaltResult').html(`
                    <div class="text-primary">
                        ● ${response.message}
                    </div>
                `);
                }
                else {
                    $('#IsPhoneVerifiedSave').val("false");

                    $('#checkSaltResult').html(`
                    <div class="text-danger">
                        ● ${response.message}
                    </div>
                `);
                }
            },
            error: function (xhr, status, error) {
                window.alert("서버 오류(인증문자 체크): " + error);
            }
        });
    }
});