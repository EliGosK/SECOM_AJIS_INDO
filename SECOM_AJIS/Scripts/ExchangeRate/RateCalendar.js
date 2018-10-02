jQuery(function () {
    dayClickEvent = function (event) {
        $('#dlgBox').OpenInputRateDialog(event)
    };

    eventClickEvent = function (event) {
        $('#dlgBox').OpenInputRateDialog(event.start)
    };
    
    generateRequestURLForCalendar = function (url) {
        if (url.indexOf("k=") < 0) {
            var key = ajax_method.GetKeyURL();
            if (key != "") {
                if (url.indexOf("?") > 0) {
                    url = url + "&k=" + key;
                }
                else {
                    url = url + "?k=" + key;
                }
            }
        }
        if (url[url.length - 1] == "#") {
            url = url.substring(0, url.length - 1);
        }
        if (url.indexOf("?") > 0) {
            url = url + "&ajax=1";
        }
        else {
            url = url + "?ajax=1";
        }
        return url;
    };

    calendar = $('#calendar-area').fullCalendar({
        height: 650,
        defaultView: 'month',
        header: {
            right: 'agendaDay agendaWeek month today prev next'
        },
        selectHelper: true,
        ignoreTimezone: false,
        slotMinutes: 30,
        dayClick: dayClickEvent,
        //  events: [
        //{ 'id': '1', 'all_day': '1', 'color': 'rgba(#FFEEEE, 0)', 'start': '2016-08-29', 'end': '2016-08-29', 'target_date': '29/Aug/2016', 'title': '', 'bank_rate': '13090.000000000', 'tax_rate': '90.000000000' },
        //{ 'id': '2', 'all_day': '1', 'color': 'rgba(#FFEEEE, 0)', 'start': '2016-09-01', 'end': '2016-09-01', 'target_date': '1/Sep/2016', 'title': '', 'bank_rate': '13080.000000000', 'tax_rate': '91.000000000' },
        //{ 'id': '3', 'all_day': '1', 'color': 'rgba(#FFEEEE, 0)', 'start': '2016-08-31', 'end': '2016-08-31', 'target_date': '31/Aug/2016', 'title': '', 'bank_rate': '13081.123456789', 'tax_rate': '91.123456789' }
        //  ],
        events: generateRequestURLForCalendar("GetCalendarItems"),
        eventClick: eventClickEvent,
        eventRender: function (event, element, view) {
            element.find('.fc-event-title').append('<div class="calendar-item-bank-rate">' + event.bank_rate + '</div><br /><div class="calendar-item-tax-rate">' + event.tax_rate + '</div><br />');
        },
        viewDisplay: function (view) {
            remarks_html = '<div class="usr-label ui-helper-clearfix" style="float: right;margin: 7px;">'
            remarks_html += '  <div class="usr-object calendar-item-bank-rate" style="width: 10px;height: 10px;"></div>'
            remarks_html += '  <div class="usr-object">: BANK Rate </div>'
            remarks_html += '  <div class="usr-object calendar-item-tax-rate" style="margin-left: 4px;width: 10px;height: 10px;"></div>'
            remarks_html += '  <div class="usr-object">: TAX Rate </div>'
            remarks_html += '</div>'
            $('.fc-header-center').html(remarks_html);
        },
        eventMouseover: function (event, element, view) {
            document.body.style.cursor = "pointer";
        },
        eventMouseout: function (event, element, view) {
            document.body.style.cursor = "default";
        },
        editable: false,
        disableDragging: true,
        titleFormat: {
            month: 'MMMM/yyyy',
            week: "dd/MMM/yyyy{ - [dd/MMM/yyyy] }",
            day: 'dd/MMM/yyyy dddd'
        },
        columnFormat: {
            month: 'ddd',
            week: 'd',
            day: 'd'
        },
        timeFormat: {// for event elements
            agenda: 'HH:mm{-HH:mm}',
            '': 'HH:mm{-H:mm}' // default
        },
        monthNames: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        dayNames: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        buttonText: {
            prev: "<span class='fc-text-arrow'>&lsaquo;</span>",
            next: "<span class='fc-text-arrow'>&rsaquo;</span>",
            prevYear: "<span class='fc-text-arrow'>&laquo;</span>",
            nextYear: "<span class='fc-text-arrow'>&raquo;</span>",
            today: 'Today',
            month: 'Month',
            week: 'Week',
            day: 'Day'
        },
        firstHour: 0,
        minTime: '0:00',
        maxTime: '0:01', // For IE
        axisFormat: '',
        allDayText: '<div style="padding-top:20px;">BANK Rate<br /><br />TAX Rate</div>'
    });

    $("#btnCalendarReload").click(function () {
        calendar.fullCalendar('refetchEvents');
    });

});