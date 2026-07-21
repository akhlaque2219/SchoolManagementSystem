// Sidebar toggle
document.addEventListener('DOMContentLoaded', function () {
    const toggle = document.getElementById('sidebarToggle');
    if (toggle) {
        toggle.addEventListener('click', function () {
            document.body.classList.toggle('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', document.body.classList.contains('sb-sidenav-toggled'));
        });
    }

    // Restore sidebar state
    if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
        document.body.classList.add('sb-sidenav-toggled');
    }

    // Auto-dismiss alerts after 4s
    document.querySelectorAll('.alert-dismissible').forEach(function (alert) {
        setTimeout(function () {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) bsAlert.close();
        }, 4000);
    });

    // Confirm deletes
    document.querySelectorAll('form[asp-action="Delete"] button[type=submit]').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            if (!confirm('Are you sure? This action cannot be undone.')) {
                e.preventDefault();
            }
        });
    });

    // Show validation summary if errors present
    var summary = document.querySelector('[data-valmsg-summary]');
    if (summary) {
        var items = summary.querySelectorAll('li');
        if (items.length > 0) {
            var alert = summary.closest('.alert');
            if (alert) alert.style.display = 'block';
        }
    }
});
