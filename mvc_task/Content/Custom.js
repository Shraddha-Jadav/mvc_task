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

    //save data in model after add or update task
    $(document).on('submit', '#taskForm', function (e) {
        e.preventDefault();
        $.ajax({
            method: "POST",
            url: "/Task/SubmitTask",
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    $('#myModal').modal('hide');
                    $('#taskTable').DataTable().ajax.reload();
                } else {
                    alert('Error: ' + response.errors.join(', '));
                }
            }
        });
    });

    //edit personal detail
    $(document).on('click', '.editDetail', function () {
        var id = $(this).data('id');
        $('.editPerDetail-modal-body').html('');
        $.ajax({
            method: "GET",
            url: "/PersonalDetail/EditPerDetail/" + id,
            contentType: false,
            success: function (response) {
                $('.editPerDetail-modal-body').append(response);
                $('#myModal').modal('show');
                $.validator.unobtrusive.parse($("#formData"));
            }
        });
    });

    $(document).on('submit', '#editDetailForm', function (e) {
        e.preventDefault();
        $.ajax({
            method: "POST",
            url: "/PersonalDetail/UpdateDetails",
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    $('#myModal').modal('hide');
                    console.log(response.employee.FirstName)
                    console.log($("#firstName"))
                    $('#firstName').text(response.employee.FirstName);
                    $('#lastName').text(response.employee.LastName);

                    var dateString = response.employee.DOB;
                    var milliseconds = dateString.match(/\d+/)[0];
                    var date = new Date(parseInt(milliseconds));
                    var formattedDate = date.toLocaleDateString();
                    $('#dob').text(formattedDate);

                    if (response.employee.Gender == "F")
                        $('#gender').text("Female");
                    else
                        $('#gender').text("Male");
                } else {
                    alert('Error: ' + response.error);
                }
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

    //nested datatable to show tasks of employee
    $("#empListTable").on('click', '.showTask', function () {
        var empId = $(this).data('empid');
        var employeeRow = $(this).closest("tr");
        var nestedTableRow = employeeRow.next(".nested-table-row");

        $(".nested-table-row").not(nestedTableRow).hide();

        if (nestedTableRow.length) {
            nestedTableRow.toggle();
        } else {
            $.ajax({
                type: "POST",
                url: "/Director/GetEmployeeTasks",
                data: { empId: empId },
                success: function (response) {
                    if (response.success) {
                        console.log("success");

                        var nestedTable = `
                        <table id="nestedTable" class="bg-white">
                            <thead>
                                <tr>
                                    <th>Task Id</th>
                                    <th>Task Name</th>
                                    <th>Task Description</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${response.tasks.map(task => `
                                    <tr>
                                        <td>${task.TaskId}</td>
                                        <td>${task.TaskName}</td>
                                        <td>${task.TaskDescription}</td>
                                    </tr>
                                `).join('')}
                            </tbody>
                        </table>`;

                        employeeRow.after('<tr class="nested-table-row"><td colspan="7">' + nestedTable + '</td></tr>');
                        $('.nested-table-row table').DataTable();
                        console.log("success");
                    } else {
                        console.log("Error: " + response.error);
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    console.log("Errorrrrrrrr");
                    console.log("AJAX error: ", textStatus, errorThrown);
                    console.log("Response: ", xhr.responseText);
                }
            });
        }
    });

});
