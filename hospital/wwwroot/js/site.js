// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


  function displayDoctorSchedule(doctorId) {
      var schedule = function (doctorId) {
          for (var i = 0; i < ViewBag.Doctors.length; i++) {
              var doctor = ViewBag.Doctors[i];
              if (doctor.Id === doctorId) {
                  return doctor.Schedule;
              }
          }
          return null; // Return null if doctor not found
      };
      if (schedule !== null) {
          var table = '<table><thead><tr><th>Date</th><th>Start Time</th><th>End Time</th></tr></thead><tbody>';
          schedule.forEach(function (event) {
              table += '<tr><td>' + event.date + '</td><td>' + event.startTime + '</td><td>' + event.endTime + '</td></tr>';
          });
          table += '</tbody></table>';

          // Display schedule table
          $('#schedule').html(table);
      }
    }

    // Event listener for doctor selection
    $('#doctorSelect').change(function () {
        var doctorId = $(this).val().Id;
        if (doctorId !== "") {
            displayDoctorSchedule(doctorId);
        } else {
            $('#schedule').empty(); // Clear schedule if no doctor selected
        }
    });