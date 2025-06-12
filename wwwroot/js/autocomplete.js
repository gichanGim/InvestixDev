$(document).ready(function () {
    let debounceTimer;

    $("#searchBox").on("keyup", function () {
        clearTimeout(debounceTimer);

        var query = $(this).val();
        if (query.length > 0) {
            debounceTimer = setTimeout(function () {
                fetchAutocompleteResults(query);
            }, 10);
        } else {
            $("#suggestionList").hide();
        }
    });

    function fetchAutocompleteResults(query) {
        $.ajax({
            url: "/Auth/GetCodeFromDB",
            type: "POST",
            data: { param: query },
            success: function (data) {
                var suggestionList = $("#suggestionList");
                suggestionList.empty();
                const searchForm = document.getElementById('searchForm');

                if (data.length > 0) {
                    searchForm.style.borderBottomLeftRadius = '0px';
                    searchForm.style.borderBottomRightRadius = '0px';

                    data.forEach(function (item) {
                        const maxNameLength = 30;
                        const displayName = item.name.length > maxNameLength
                            ? item.name.substring(0, maxNameLength) + "..."
                            : item.name;

                        const firstChar = item.name.charAt(0).toUpperCase();

                        const typeColorMap = {
                            'Stock': '#3b7bbf',
                            'ETF': '#c0d450',
                            'Index': '#79d964'
                        };
                        const bgColor = typeColorMap[item.type] || '#cccccc';

                        const currencyFlagMap = {
                            'USD': 'https://www.countryflags.com/wp-content/uploads/united-states-of-america-flag-png-large.png',
                            'KRW': 'https://www.countryflags.com/wp-content/uploads/south-korea-flag-png-large.png',
                            'JPY': 'https://www.countryflags.com/wp-content/uploads/japan-flag-png-large.png',
                            'CNY': 'https://www.countryflags.com/wp-content/uploads/china-flag-png-large.png',
                            'HKD': 'https://www.countryflags.com/wp-content/uploads/hongkong-flag-jpg-xl.jpg',
                            'VND': 'https://www.countryflags.com/wp-content/uploads/vietnam-flag-png-large.png'
                        };
                        const currencyImg = currencyFlagMap[item.currency] || 'img/default.png';

                        const imgUrl = `https://thumb.tossinvest.com/image/resized/96x0/https%3A%2F%2Fstatic.toss.im%2Fpng-icons%2Fsecurities%2Ficn-sec-fill-${item.symbol}.png`;

                        // ✅ SVG fallback 로고 생성
                        const fallbackSvg = encodeURIComponent(`
        <svg xmlns="http://www.w3.org/2000/svg" width="50" height="50">
            <rect width="100%" height="100%" fill="${bgColor}"/>
            <text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" font-size="20" font-weigh="bold" fill="white">${firstChar}</text>
        </svg>
    `);
                        const fallbackImage = `data:image/svg+xml;charset=UTF-8,${fallbackSvg}`;

                        const listItem = $(`
        <li class='suggestion-item' style='display:flex; align-items:center;justify-content:between;' data-symbol='${item.symbol}' data-name='${item.name}' data-exchange='${item.exchange}' data-currency='${item.currency}' data-type='${item.type}'>
            <img id="li-symbol-img" src="${imgUrl}" alt="${firstChar}"
                onerror="this.onerror=null; this.src='${fallbackImage}';" class="ms-2"/>
            <div id="li-container" class="ms-3">
                <span id="li-symbol">${item.symbol}</span>
                <span id="li-name">${item.name}</span>
                <span class="ms-3" style="font-size: 16px;font-weight:bold;">${item.exchange}/</span>
                <span style="font-size: 16px;font-weight:bold;">${item.type}</span>
                <span style="font-size:14px;font-weight:bold;color:gray;margin-top:5px;" class="ms-5">
                    <img src="${currencyImg}" alt="${item.currency}" style="width:30px; height:20px; vertical-align: middle;border-radius:2px;" />
                     &nbsp;${item.currency}
                </span>
            </div>
        </li>
    `);

                        suggestionList.append(listItem);
                    });

                    suggestionList.show();
                } else {
                    searchForm.style.borderBottomLeftRadius = '30px';
                    searchForm.style.borderBottomRightRadius = '30px';
                    suggestionList.hide();
                }
            },
            error: function () {
                console.error("자동완성 데이터를 가져오는 중 오류 발생");
            }
        });
    }

    $(document).on("click", ".suggestion-item", function () {
        var selectedSymbol = $(this).data("symbol");
        var selectedName = $(this).data("name");
        var selectedExchange = $(this).data("exchange");
        var selectedCurrency = $(this).data("currency");
        var selectedType = $(this).data("type");
        $("#searchBox").val(selectedSymbol);
        $("#suggestionList").hide();
        sendSelectedSymbolToServer(selectedSymbol, selectedName, selectedExchange, selectedCurrency, selectedType);
    });

    function sendSelectedSymbolToServer(symbol, name, exchange, currency, type) {
        $.ajax({
            url: "/Auth/AutocompleteClicked",
            type: "POST",
            data: {
                symbol: symbol,
                name: name,
                exchange: exchange,
                currency: currency,
                type: type
            },
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