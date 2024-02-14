$(function() {

    // sample calendar events data

    var curYear = moment().format('YYYY');
    var curMonth = moment().format('MM');

    // Calendar Event Source



    $.ajax({
        url: '/PublicHoliday/getPublicHolidays',
        type: "GET",
        dataType: "JSON",

        success: function(result) {
            var events = [];

            $.each(result, function(i, data) {
                events.push({
                    id: '10',
                    start: data.Date_public_holidays,
                    end: data.Date_public_holidays,
                    title: data.Date_description
                });
            });

            callback(events);
        }
    });

    // Birthday Events Source
    var birthdayEvents = {
        id: 2,
        backgroundColor: 'rgba(16,183,89, .25)',
        borderColor: '#10b759',
        events: [{
                id: '7',
                start: curYear + '-' + curMonth + '-01T18:00:00',
                end: curYear + '-' + curMonth + '-01T23:30:00',
                title: 'Jensen Birthday',
                description: 'In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis az pede mollis...'
            },
            {
                id: '8',
                start: curYear + '-' + curMonth + '-21T15:00:00',
                end: curYear + '-' + curMonth + '-21T21:00:00',
                title: 'Carl\'s Birthday',
                description: 'In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis az pede mollis...'
            },
            {
                id: '9',
                start: curYear + '-' + curMonth + '-23T15:00:00',
                end: curYear + '-' + curMonth + '-23T21:00:00',
                title: 'Yaretzi\'s Birthday',
                description: 'In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis az pede mollis...'
            }
        ]
    };


    var holidayEvents = {
        id: 3,
        backgroundColor: 'rgba(241,0,117,.25)',
        borderColor: '#f10075',
        events: [{
                id: '10',
                start: curYear + '-' + curMonth + '-04',
                end: curYear + '-' + curMonth + '-06',
                title: 'Feast Day'
            },
            {
                id: '11',
                start: curYear + '-' + curMonth + '-26',
                end: curYear + '-' + curMonth + '-27',
                title: 'Memorial Day'
            },
            {
                id: '12',
                start: curYear + '-' + curMonth + '-28',
                end: curYear + '-' + curMonth + '-29',
                title: 'Veteran\'s Day'
            }
        ]
    };




    // initialize the calendar
    $('#fullcalendar').fullCalendar({
        header: {
            left: 'prev,today,next',
            center: 'title',
            right: 'month,agendaWeek,agendaDay,listMonth'
        },
        editable: true,
        droppable: true, // this allows things to be dropped onto the calendar
        dragRevertDuration: 0,
        defaultView: 'month',
        eventLimit: true, // allow "more" link when too many events
        eventSources: [birthdayEvents, holidayEvents],
        eventClick: function(event, jsEvent, view) {
            $('#modalTitle1').html(event.title);
            $('#modalBody1').html(event.description);
            $('#eventUrl').attr('href', event.url);
            $('#fullCalModal').modal();
        },
        dayClick: function(date, jsEvent, view) {
            $("#createEventModal").modal("show");
        },
        // defaultDate: '2020-07-12',
        // events: [{
        //     title: 'All Day Event',
        //     start: '2020-07-08'
        //   },
        //   {
        //     title: 'Long Event',
        //     start: '2020-07-01',
        //     end: '2020-07-07',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'
        //   },
        //   {
        //     id: 999,
        //     title: 'Repeating Event',
        //     start: '2020-07-09T16:00:00',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'

        //   },
        //   {
        //     id: 999,
        //     title: 'Repeating Event',
        //     start: '2020-07-16T16:00:00',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'
        //   },
        //   {
        //     title: 'Conference',
        //     start: '2020-07-11',
        //     end: '2020-07-13',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'        
        //   },
        //   {
        //     title: 'Meeting',
        //     start: '2020-07-12T10:30:00',
        //     end: '2020-07-12T12:30:00',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'
        //   },
        //   {
        //     title: 'Lunch',
        //     start: '2020-07-12T12:00:00',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'
        //   },
        //   {
        //     title: 'Meeting',
        //     start: '2020-07-12T14:30:00',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'
        //   },
        //   {
        //     title: 'Happy Hour',
        //     start: '2020-07-12T17:30:00',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'
        //   },
        //   {
        //     title: 'Dinner',
        //     start: '2020-07-12T20:00:00',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'
        //   },
        //   {
        //     title: 'Birthday Party',
        //     start: '2020-07-13T07:00:00',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'
        //   },
        //   {
        //     title: 'Team Lunch',
        //     start: '2020-07-28',
        //     description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Repellendus adipisci explicabo magnam molestiae libero.'
        //   }
        // ],

        // drop: function() {
        //   // is the "remove after drop" checkbox checked?
        //   if ($('#drop-remove').is(':checked')) {
        //     // if so, remove the element from the "Draggable Events" list
        //     $(this).remove();
        //   }
        // },
        eventDragStop: function(event, jsEvent, ui, view) {
            if (isEventOverDiv(jsEvent.clientX, jsEvent.clientY)) {
                // $('#calendar').fullCalendar('removeEvents', event._id);
                var el = $("<div class='fc-event'>").appendTo('#external-events-listing').text(event.title);
                el.draggable({
                    zIndex: 999,
                    revert: true,
                    revertDuration: 0
                });
                el.data('event', { title: event.title, id: event.id, stick: true });
            }
        }
    });


    var isEventOverDiv = function(x, y) {
        var external_events = $('#external-events');
        var offset = external_events.offset();
        offset.right = external_events.width() + offset.left;
        offset.bottom = external_events.height() + offset.top;

        // Compare
        if (x >= offset.left &&
            y >= offset.top &&
            x <= offset.right &&
            y <= offset.bottom) { return true; }
        return false;
    }

});