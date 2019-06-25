/* ========================================================================= */
/*  Preloader
/* ========================================================================= */
//setTimeout(function () { $("#preloader").fadeOut("slow"); }, 5000);
//$(window).on('load', function () {
//    $("#preloader").fadeOut("slow");
//    registerControllers();
//});

// Write your JavaScript code.
(function (w) {
    w.app = {
        blockUI: function () {
            $.blockUI({
                css: {
                    border: 'none',
                    padding: '15px 0',
                    backgroundColor: 'transparent',
                    borderRadius: "15px",
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: 1,
                    top: '20%'
                },
                overlayCSS: { backgroundColor: '#c7c7c7' },
                message: $('#loader')
            });
        },
        unblockUI: function () {
            $.unblockUI();
        }
    };
})(window);

$.validator.setDefaults({ ignore: null });

function registerControllers() {
    var shouldClose = true;
    var autoCompleteHandler;
    var autoCompleteTimeup;
    var suggestList = $('<div style="position:absolute;z-index:5001;display:none;" class="suggest-box"><div class="list-group ac-container"><a class="list-group-item">加载数据...</a></div></div>');
    var suggestBox = $('<div style="position:absolute;z-index:5001;display:none;" class="suggest-box"><div class="box-group ac-container"><a class="box-group-item">加载数据...</a></div></div>');
    $("input[auto-complete='on'][auto-complete-type='list']").after(suggestList);
    $("input[auto-complete='on'][auto-complete-type='box']").after(suggestBox);
    $("input[auto-complete='on']").on("blur", function () {
        $('.suggest-box').hide().children("div.list-group").empty();
        shouldClose = true;
    }).on("keyup click", function () {
        shouldClose = false;

        var input = $(this);
        var p = input.position();
        var left = p.left;
        var top = p.top + input.innerHeight() + 2;
        url = input.attr("auto-complete-url");

        var nameTarget = input.attr("auto-complete-name");
        var idTarget = input.attr("auto-complete-id");
        var domainTarget = input.attr("auto-complete-domain");
        var type = input.attr("auto-complete-type");

        var sugBox = input.next();

        sugBox.css({
            top: top,
            left: left,
            minWidth: input.outerWidth()
        })

        var listBox = sugBox.children("div.ac-container");

        if (autoCompleteHandler) {
            autoCompleteHandler.abort();
        }
        if (autoCompleteTimeup) {
            clearTimeout(autoCompleteTimeup);
        }

        autoCompleteTimeup = setTimeout(function () {
            autoCompleteHandler = $.ajax({
                url: url,
                method: "GET",
                data: { name: input.val() }
            }).done(function (d) {
                listBox.empty()
                if (d.result && d.result.length > 0) {
                    d.result.forEach(function (e) {
                        var itemHtml;
                        if (type == "list") {
                            itemHtml = '<a class="list-group-item list-group-item-action ac-list-item" ';
                        }
                        else if (type == "box") {
                            itemHtml = '<a class="box-group-item ac-list-item" ';
                        }
                        else {
                            itemHtml = '<a class="ac-list-item" ';
                        }
                        if (e.id) {
                            itemHtml += 'data-id="' + e.id + '" data-id-target="' + idTarget + '" ';
                        }
                        if (e.name) {
                            itemHtml += 'data-name="' + e.name + '" data-name-target="' + nameTarget + '" ';
                        }
                        if (e.domain) {
                            itemHtml += 'data-domain="' + e.domain + '" data-domain-target="' + domainTarget + '" ';
                        }
                        itemHtml += '>' + e.name + '</a>';
                        listBox.append(itemHtml);
                    });
                }
                else {
                    if (type == "list") {
                        listBox.append('<a class="list-group-item" disabled>未找到' + input.val() + '。</a>');
                    }
                    else if (type == "box") {
                        listBox.append('<a class="box-group-item" disabled>未找到' + input.val() + '。</a>');
                    }

                }

                if (!shouldClose)
                    sugBox.show();
            });
        }, 500);
    });

    $('body').on("mousedown", ".ac-list-item", function (e) {
        var item = $(this);
        var nameTarget = item.attr("data-name-target");
        var idTarget = item.attr("data-id-target");
        var domainTarget = item.attr("data-domain-target");

        if ($("#" + nameTarget).is("input")) {
            $("#" + nameTarget).val(item.attr("data-name"));
        }
        else {
            $("#" + nameTarget).html(item.attr("data-name"))
        }

        if ($("#" + idTarget).is("input")) {
            $("#" + idTarget).val(item.attr("data-id"));
        }
        else {
            $("#" + idTarget).html(item.attr("data-id"))
        }

        if ($("#" + domainTarget).is("input")) {
            $("#" + domainTarget).val(item.attr("data-domain"));
        }
        else {
            $("#" + domainTarget).html(item.attr("data-domain"))
        }
    });
}