// npm package: sweetalert2
// github link: https://github.com/sweetalert2/sweetalert2

$(function() {

  showSwal = function(type, msg) {
  'use strict';
    if (type === 'ops') {
      Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: msg
      })
    }

    else if (type === 'confirm')
    {
        Swal.fire({
              title: 'Are you sure?',
              text: "You won't be able to revert this!",
              icon: 'warning',
              showCancelButton: true,
              confirmButtonColor: 'swal-confirm-button',
              cancelButtonColor: 'swal-cancel-button',
              confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
              if (result.isConfirmed) {
                Swal.fire(
                  'Deleted!',
                  'Your file has been deleted.',
                  'success'
                )
              }
        })
    }
    else if(type==='success') {
      const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
          timerProgressBar: true,
          customClass: {
              popup: 'colored-toast',
              timerProgressBar: 'swal2-timer-progress-bar-success',
          },  
      }); 
      Toast.fire({
          icon: 'success',
        title: msg
      })
    }
    else if (type === 'error') {
        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            customClass: {
                popup: 'colored-toast',
                timerProgressBar: 'swal2-timer-progress-bar-danger'
            },

        });
        Toast.fire({
            icon: 'error',
            title: msg
        })
    }
  }
});