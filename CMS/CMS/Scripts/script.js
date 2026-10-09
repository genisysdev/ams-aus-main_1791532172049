
//Document ready function
$(document).ready(function () {

    // Dropdown submenu code
    $(".nav-bottom-left .dropdown-toggle").click(function (e) {
        e.stopPropagation();
        $(".nav-bottom-left .dropdown-toggle").not(this).siblings().slideUp();
        $(this).siblings().slideToggle();
    });

    // User info dropdown menu
    $(".user-section .dropdown-toggle").click(function (e) {
        e.stopPropagation();
        $('.user-section .dropdown-menu').slideToggle();
    });

    // Burger menu code
    $('.main-nav .hmbrgr').click(function (e) {
        e.stopPropagation();
        $('.main-nav .hmbrgr').toggleClass('active');
        $('.sidebar-container-wrapper').toggleClass('active');
    });

    //On click body hide dropdown
    $('html, body').click(function (e) {
        // e.stopPropagation();
        $(".nav-bottom-left .dropdown-menu").slideUp();
        $(".user-section .dropdown-menu").slideUp();
        $('.main-nav .hmbrgr').removeClass('active');
        $('.sidebar-container-wrapper').removeClass('active');
    });

    // Dropdown submenu code for devices
    $(".sidebar-container .navbar-nav .dropdown-toggle").click(function (e) {
        e.stopPropagation();
        $(".sidebar-container .navbar-nav .dropdown-toggle").not(this).siblings().slideUp();
        $(this).siblings().slideToggle();
    });

    //
    $('#startDate, #endDate').datepicker({
        language: 'en',
        minDate: new Date(), // Now can select only dates, which goes after today
        dateFormat: 'dd-mm-yyyy'
    })
});