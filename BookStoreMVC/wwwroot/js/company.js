﻿var dataTable;

$(document).ready(function () {

    loadDataTable();
});


function loadDataTable() {
    dataTable = $("#tblData").DataTable({

        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [ // Make the <th> for every column in the tbl first or this will error out
            { "data": "name", "width": "10%" },
            { "data": "streetAddress", "width": "10%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10%" },
           // { "data": "postalCode", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            {
                "data": "isAuthorizedCompany",
                "render": function (data) {
                    if (data) {
                        return `<input type="checkbox" disabled checked />`
                    } else {
                        return `<input type="checkbox" disabled />`
                    }
                },
                "width": "10%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                            <a href="/Admin/Company/Upsert/${data}" class="btn btn-success text-white"><i class="fas fa-edit"></i></a>
                            <a onclick="Delete('/Admin/Company/Delete/${data}')" class="btn btn-danger text-white"><i class="fas fa-trash-alt"></i></a>
                            </div>
                    `;
                }, "width": "25%"
            },
        ]
    });
}

function Delete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be sable to restore the data",
        icon: "warning",
        buttons: true,
        dangerMode: true

    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }

    });
}