$(function () {

    function getFunction(code, argNames) {
        var fn = window, parts = (code || "").split(".");
        while (fn && parts.length) {
            fn = fn[parts.shift()];
        }
        if (typeof (fn) === "function") {
            return fn;
        }
        argNames.push(code);
        return Function.constructor.apply(null, argNames);
    }

    function isMethodProxySafe(method) {
        return method === "GET" || method === "POST";
    }

    function asyncOnSuccess(element, data, contentType) {
        var mode;

        if (contentType.indexOf("application/x-javascript") !== -1) {  // jQuery already executes JavaScript for us
            return;
        }

        mode = (element.getAttribute("data-ajax-mode") || "").toUpperCase();
        $(element.getAttribute("data-ajax-update")).each(function (i, update) {
            var top;

            switch (mode) {
                case "BEFORE":
                    top = update.firstChild;
                    $("<div />").html(data).contents().each(function () {
                        update.insertBefore(this, top);
                    });
                    break;
                case "AFTER":
                    $("<div />").html(data).contents().each(function () {
                        update.appendChild(this);
                    });
                    break;
                case "REPLACE-WITH":
                    $(update).replaceWith(data);
                    break;
                default:
                    $(update).html(data);
                    break;
            }
        });
    }

    function asyncRequest(element, options) {
        var loading, method, duration;

        loading = $(element.getAttribute("data-ajax-loading"));
        duration = parseInt(element.getAttribute("data-ajax-loading-duration"), 10) || 0;

        $.extend(options, {
            type: element.getAttribute("data-ajax-method") || undefined,
            url: element.getAttribute("data-ajax-url") || undefined,
            cache: !!element.getAttribute("data-ajax-cache"),
            beforeSend: function (xhr) {
                getFunction(element.getAttribute("data-ajax-begin"), ["xhr"]).apply(element, arguments);
            },
            complete: function () {
                getFunction(element.getAttribute("data-ajax-complete"), ["xhr", "status"]).apply(element, arguments);
            },
            success: function (data, status, xhr) {
                asyncOnSuccess(element, data, xhr.getResponseHeader("Content-Type") || "text/html");
                getFunction(element.getAttribute("data-ajax-success"), ["data", "status", "xhr"]).apply(element, arguments);
            },
            error: function () {
                getFunction(element.getAttribute("data-ajax-failure"), ["xhr", "status", "error"]).apply(element, arguments);
            }
        });

        options.data.push({ name: "X-Requested-With", value: "XMLHttpRequest" });

        method = options.type.toUpperCase();
        if (!isMethodProxySafe(method)) {
            options.type = "POST";
            options.data.push({ name: "X-HTTP-Method-Override", value: method });
        }

        $.ajax(options);
    }

    $(document).on("click", "div.pagination-wrapper .page-link[data-page]", function (evt) {
        var container = $(this).closest("div.pagination-wrapper");

        evt.preventDefault();
        if (container) {
            var page = $(this).attr("data-page")
            var controller = container.attr("asp-controller");
            var action = container.attr("asp-action")
            var url = controller + "/" + action;
            var method = container.attr("data-ajax-method");
            var data = container.find("input.pagination-data").val();


            var obj = JSON.parse(data);
            if (obj) {
                obj.Page = page;

                var dataArray = [];
                for (var prop in obj) {
                    if (obj.hasOwnProperty(prop)) {
                        var val = obj[prop];
                        dataArray.push({ name: prop, value: val })
                    }
                }

                if (url) {
                    asyncRequest(container[0], {
                        url: url,
                        type: method || "GET",
                        data: dataArray
                    });
                }
            }
        }
    });

    $(document).on("click", "a.sorting", function (evt) {
        var containerId = $(this).attr("paging-target");

        var column = $(this).attr("paging-sort-col");
        var sorting = $(this).attr("paging-sort-asc");
        var isAscending = sorting==="True";

        var container = $(containerId);
        evt.preventDefault();
        if (container) {
            var page = 0;
            var controller = container.attr("asp-controller");
            var action = container.attr("asp-action")
            var url = controller + "/" + action;
            var method = container.attr("data-ajax-method");
            var data = container.find("input.pagination-data").val();


            var obj = JSON.parse(data);
            if (obj) {
                obj.Page = page;

                if(obj.SortingItems){
                    obj.SortingItems.forEach(function(i, index){
                        if(i.Name == column){
                            obj.SortingItems.splice(index, 1)
                            return false;
                        }
                    });
                    obj.SortingItems.splice(0,0,{"Name":column, "IsAscending":!isAscending});
                    obj.SortItemString = JSON.stringify(obj.SortingItems);
                    delete obj.SortingItems;
                }

                var dataArray = [];
                for (var prop in obj) {
                    if (obj.hasOwnProperty(prop)) {
                        var val = obj[prop];
                        dataArray.push({ name: prop, value: val })
                    }
                }

                if (url) {
                    asyncRequest(container[0], {
                        url: url,
                        type: method || "GET",
                        data: dataArray
                    });
                }
            }
        }
    });
});