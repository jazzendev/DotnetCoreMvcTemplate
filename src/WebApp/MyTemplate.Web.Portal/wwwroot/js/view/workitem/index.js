_.templateSettings = {
    interpolate: /\{\{(.+?)\}\}/g
};

$("#workitemList").on("click", ".data-item", function () {
    app.blockUI();
    var id = $(this).attr("data-id");
    $.ajax({
        url: `/workitem/Progress/${id}`,
        method: "GET"
    }).done(function (data) {
        $("#workItemDetail").empty();

        if (data && data.isSuccess) {
            $("#progressTitle").html(data.result.name);
            var template = _.template($("#workitem-template").html());
            $("#workItemDetail").html(template(data.result));
            $('#workitemModel').modal('show');
        }
        else {
            alert("获取数据失败");
        }
    }).fail(function () {
        alert("网络不给力！");
    }).always(function () {
        app.unblockUI();
    });
});

$("#submitProgress").on('click', function () {
    //if (confirm("确认提交吗？")) { }
    var data = {
        id: $("#progressId").val(),
        ownerId: $("#progressOwner").val(),
        priority: $("#progressPriority").val(),
        status: $("#progressStatus").val()
    };

    $.ajax({
        url: `/workitem/UpdateProgress`,
        method: "POST",
        data: data
    }).done(function (data) {
        if (data && data.isSuccess) {
            alert("提交成功！");
            $('#workitemModel').modal('hide');
            $('#searchForm').submit();
        }
        else {
            alert("提交失败。");
        }
    }).fail(function () {
        alert("网络不给力！");
    }).always(function () {
        app.unblockUI();
    });
});