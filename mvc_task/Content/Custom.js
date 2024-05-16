
//open model and get data for add or update task
$(document).ready(function () {
    $(document).on('click', '.AddTask', function () {
        var id = $(this).data('id');
        $('.task-modal-body').html('');
        if (id == undefined) {
            id = 0;
        }
        $('#myModal').modal('show');
        $.ajax({
            method: "GET",
            url: "/Task/AddTask/" + id,
            contentType: false,
            success: function (response) {
                $('.task-modal-body').append(response);
                $('#myModal').modal('show');
                $.validator.unobtrusive.parse($("#taskForm"));
            }
        });
    });

    //save data in model 
    $(document).on('submit', '#taskForm', function (e) {
        e.preventDefault();
        $.ajax({
            method: "POST",
            url: "/Task/SubmitTask",
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    $('#myModal').modal('hide');
                    updateDataTable(response.task);
                } else {
                    alert('Error: ' + response.errors.join(', '));
                }
            }
        });
    });

    //update datatable without reload
    function updateDataTable(task) {
        debugger;
        var color = task.Status == "Pending" ? "warning" : row.Status == "Approved" ? "success" : "danger";
        if (task.TaskID === 0) {
            $('#taskTable').DataTable().row.add([
                task.TaskName,
                task.TaskDescription,
                task.TaskDate,
                '<p class="rounded-pill btn-sm btn border-' + color + ' text-' + color + '">' + task.Status + '</p>',
                task.ApprovedOrRejectedOn,
                task.CreatedOn,
                task.ModifiedOn,
                '<button type="button" data-id="' + task.TaskID + '" class="btn btn-sm btn-warning px-3 AddTask">Edit</button>&nbsp' +
                '<a href="/Task/DeleteTask/' + task.TaskID + '" class="btn btn-danger btn-sm" onclick="return confirm(\'Are you sure you want to delete this task?\')">Delete</a>'
            ]).draw();

        } else {
            var table = $('#taskTable').DataTable();
            var rowIndex = table.row('#row_' + task.TaskID).index();
            table.row(rowIndex).data([
                task.TaskName,
                task.TaskDescription,
                task.TaskDate,
                '<p class="rounded-pill btn-sm btn border-' + color + ' text-' + color + '">' + task.Status + '</p>',
                task.ApprovedOrRejectedOn,
                task.CreatedOn,
                task.ModifiedOn,
                '<button type="button" data-id="' + task.TaskID + '" class="btn btn-sm btn-warning px-3 AddTask">Edit</button>&nbsp' +
                '<a href="/Task/DeleteTask/' + task.TaskID + '" class="btn btn-danger btn-sm" onclick="return confirm(\'Are you sure you want to delete this task?\')">Delete</a>'
            ]).draw();
        }
    }

    //edit personal detail
    $(document).on('click', '.editDetail', function () {
        var id = $(this).data('id');
        $('.task-modal-body').html('');
        $.ajax({
            method: "GET",
            url: "/PersonalDetail/EditPerDetail/" + id,
            contentType: false,
            success: function (response) {
                $('.task-modal-body').append(response);
                $('#myModal').modal('show');
                $.validator.unobtrusive.parse($("#formData"));
            }
        });
    });

    //edit personal detail of employee/Manager by director
    $(document).on('click', '.editUserDetail', function () {
        var id = $(this).data('id');
        $('.user-modal-body').html('');
        $.ajax({
            method: "GET",
            url: "/Director/EditEmp/" + id,
            contentType: false,
            success: function (response) {
                $('.user-modal-body').append(response);
                $('#myModal').modal('show');
                $.validator.unobtrusive.parse($("#formData"));
                $('#ddlDepartment').change(function () {
                    $.ajax({
                        type: "post",
                        url: "/Director/GetReportingPer",
                        data: { departmentId: $('#ddlDepartment').val() },
                        datatype: "json",
                        traditional: true,
                        success: function (data) {
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
    $("#taskTable").on("click", ".approveBtn, .rejectBtn", function () {
        var taskId = $(this).data("taskid");
        var btn = $(this).data("btn");

        $.ajax({
            type: "POST",
            url: "/Manager/AppOrRejByManager",
            data: { id: taskId, btn: btn },
            success: function (response) {
                $("#taskTable").html(response);
            }
        });
    });
    
});
