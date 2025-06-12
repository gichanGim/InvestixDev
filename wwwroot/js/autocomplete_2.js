$(document).ready(function () {
    const input = document.getElementById('searchBox');
    input.addEventListener('input', function () {
        if (input.value === ''){
            const searchForm = document.getElementById('searchForm');

            searchForm.style.borderBottomLeftRadius = '30px';
            searchForm.style.borderBottomRightRadius = '30px';
        }
    });
});

$(document).ready(function () {
    const $searchForm = $('#searchForm');
    const $overlay = $('#overlay');
    const $searchBoxPrev = $('#searchBoxPrev');
    const $searchBox = $('#searchBox');

    $searchBoxPrev.on('focus', function () {
        $searchForm.show();
        $overlay.show();
        $searchBox.focus(); // 포커스를 진짜 input으로 넘김
    });

    $searchBox.on('blur', function () {
        setTimeout(() => {
            $searchForm.hide();
            $overlay.hide();
            $searchBoxPrev.val(''); // 필요 시 초기화
        }, 150); // blur 이벤트 직후 클릭 막기 위해 약간의 지연
    });
});