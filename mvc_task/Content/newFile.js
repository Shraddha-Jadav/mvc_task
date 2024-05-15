//add or update task by employee
$(document).ready(function() {
    $(document).on('click', '.AddTask', function() {
        var id = $(this).data('id');
        $('.modal-body').html('');
        if (id == undefined) {
            id = 0;
        }
        $('#myModal').modal('show');
        $.ajax({
            method: "GET",
            url: "/Task/AddTask/" + id,
            contentType: false,
            success: function(response) {
                $('.modal-body').append(response);
                $('#myModal').modal('show');
                $.validator.unobtrusive.parse($("#formData"));
            }
        });
    });

    //edit personal detail
    $(document).on('click', '.editDetail', function() {
        var id = $(this).data('id');
        $('.modal-body').html('');
        $('#myModal').modal('show');
        $.ajax({
            method: "GET",
            url: "/PersonalDetail/EditPerDetail/" + id,
            contentType: false,
            success: function(response) {
                $('.modal-body').append(response);
                $('#myModal').modal('show');
                $.validator.unobtrusive.parse($("#formData"));
            }
        });
    });

    //edit personal detail of employee/Manager by director
    $(document).on('click', '.editUserDetail', function() {
        var id = $(this).data('id');
        $('.modal-body').html('');
        $('#myModal').modal('show');
        $.ajax({
            method: "GET",
            url: "/Director/EditEmp/" + id,
            contentType: false,
            success: function(response) {
                $('.modal-body').append(response);
                $('#myModal').modal('show');
                $.validator.unobtrusive.parse($("#formData"));
                $('#ddlDepartment').change(function() {
                    $.ajax({
                        type: "post",
                        url: "/Director/GetReportingPer",
                        data: { departmentId: $('#ddlDepartment').val() },
                        datatype: "json",
                        traditional: true,
                        success: function(data) {
                            debugger;
                            var Employee = $("#ddlEmployeeName");
                            Employee.empty();
                            console.log(data);
                            if (data == null || Object.keys(data).length === 0) {
                                Employee.append('<option value="" disabled selected>No Reporting Person</option>');
                                Employee.prop('disabled', true);
                            }
                            else {
                                $('#ddlEmployeeName').prop('disabled', false);
                                for (var i = 0; i < data.length; i++) {
                                    Employee.append('<option value=' + data[i].Value + '>' + data[i].Text + '</option>');
                                }
                            }
                        }
                    });
                });
            }
        });
    });

    //approve or reject task by manager
    $(document).ready(function() {
        $(".approveBtn, .rejectBtn").click(function() {
            var taskId = $(this).data("taskid");
            var btn = $(this).data("btn");

            $.ajax({
                type: "POST",
                url: "@Url.Action(", AppOrRejByManager, ", ": Manager, ")": ,
                data: { id: taskId, btn: btn },
                success: function(response) {
                    $("#taskTable").html(response);
                }
            });
        });
    });
});
