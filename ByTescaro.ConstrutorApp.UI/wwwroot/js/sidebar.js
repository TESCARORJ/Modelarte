window.sidebarHelper = {
    enableAutoClose: function (dotnetHelper, sidebarSelector) {
        document.addEventListener('click', function (event) {
            // Só ativa o autoclose se for uma tela responsiva
            if (window.innerWidth >= 992) return; // Evita execução no desktop

            const sidebar = document.querySelector(sidebarSelector);
            const toggle = document.querySelector('.rz-sidebar-toggle');

            if (!sidebar || !sidebar.classList.contains('rz-sidebar-expanded')) return;

            if (!sidebar.contains(event.target) && !toggle.contains(event.target)) {
                dotnetHelper.invokeMethodAsync('CloseSidebar');
            }
        });
    }
};



//window.sidebarHelper = {
//    enableAutoClose: function (dotnetHelper, sidebarSelector) {
//        document.addEventListener('click', function (event) {
//            const sidebar = document.querySelector(sidebarSelector);
//            const toggle = document.querySelector('.rz-sidebar-toggle');

//            if (!sidebar || !sidebar.classList.contains('rz-sidebar-expanded')) return;

//            if (!sidebar.contains(event.target) && !toggle.contains(event.target)) {
//                dotnetHelper.invokeMethodAsync('CloseSidebar');
//            }
//        });
//    }
//};
