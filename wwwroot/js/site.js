// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    if ($('.table-datatable').length) {
        $('.table-datatable').DataTable({
            "pageLength": 10,
            "lengthChange": false,
            "info": true,
            "language": {
                "search": "",
                "searchPlaceholder": "Search..."
            }
        });
    }
});

function confirmDelete(form) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ec4899',
        cancelButtonColor: '#6366f1',
        confirmButtonText: 'Yes, delete it!',
        background: '#1e293b',
        color: '#f8fafc'
    }).then((result) => {
        if (result.isConfirmed) {
            form.submit();
        }
    });
}
