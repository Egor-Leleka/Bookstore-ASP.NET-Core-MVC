var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'title', "width": "35%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "5%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div>
                                <a href="/admin/product/upsert?id=${data}" class="btn btn-success mx-2" style="width:90px">
		                        <i class="bi bi-pencil"></i> Edit
                                </a>

		                        <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2" style="width:90px">
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
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}



