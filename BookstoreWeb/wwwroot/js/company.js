var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: 'name', "width": "16%" },
            { data: 'streetAddress', "width": "20%" },
            { data: 'city', "width": "9%" },
            { data: 'state', "width": "12%" },
            { data: 'postalCode', "width": "10%" },
            { data: 'phoneNumber', "width": "13%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div>
                                <a href="/admin/company/upsert?id=${data}" class="btn btn-success mx-2" style="width:90px">
		                        <i class="bi bi-pencil"></i> Edit
                                </a>

		                        <a onClick=Delete('/admin/company/delete/${data}') class="btn btn-danger mx-2" style="width:90px">
		                        <i class="bi bi-trash3"></i> Delete
		                        </a>
                            </div>`
                },
                "width": "20%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();
                }
            })
        }
    });
}



