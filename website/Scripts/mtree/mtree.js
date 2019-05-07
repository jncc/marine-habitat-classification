

// Modified mtree.js
// Requires jquery.js
; (function ($, window, document, undefined) {
    
    // Only apply if mtree list exists
    if ($('ul.mtree').length) {

        // Settings
        var start_level = 2; // Start with collapsed menu (only level 1 items visible)
        var duration = 400; // Animation duration should be tweaked according to easing.
        

        // Set initial styles 
        $('.mtree ul').css({ 'overflow': 'hidden', 'height': 0, 'display': 'none' });

        // Get node elements, and add classes for styling
        var node = $('.mtree li:has(ul)');
        node.each(function (index, val) {
            var nodeLevel = $(this).parentsUntil($('ul.mtree'), 'ul').length + 1;
            $(this).children(':first-child').css('cursor', 'pointer');
            $(this).addClass('mtree-node mtree-' + ((start_level <= nodeLevel) ? 'closed' : 'open'));
            $(this).children('a').first().children('span').first().addClass(((start_level <= nodeLevel) ? 'closed' : 'open'));
            $(this).children('ul').addClass('mtree-level-' + nodeLevel);
            $(this).children('ul').css({ 'overflow': 'hidden', 'height': (start_level < nodeLevel + 1) ? 0 : 'auto', 'display': (start_level < nodeLevel + 1) ? 'none' : 'block' });
        });

        // Set mtree-active class on list items for last opened element
        $('.mtree li > *:first-child').on('click.mtree-active', function (e) {
            if ($(this).parent().hasClass('mtree-closed')) {
                $('.mtree-active').not($(this).parent()).removeClass('mtree-active');
                $(this).parent().addClass('mtree-active');
            } else if ($(this).parent().hasClass('mtree-open')) {
                $(this).parent().removeClass('mtree-active');
            } else {
                $('.mtree-active').not($(this).parent()).removeClass('mtree-active');
                $(this).parent().toggleClass('mtree-active');
            }
        });

        // Set node click elements, preferably <a> but node links can be <span> also
        node.children(':first-child').on('click.mtree', function (e) {

            // element vars
            var el = $(this).parent().children('ul').first();
            var isOpen = $(this).parent().hasClass('mtree-open');

            // force auto height of element so actual height can be extracted
            el.css({ 'height': 'auto' });

            setNodeClass($(this).parent(), isOpen);
            el.slideToggle(duration);
   

            // We can't have nodes as links unfortunately
            e.preventDefault();
        });
    }

    // Function for updating node class
    function setNodeClass(el, isOpen) {
        if (isOpen) {
            el.removeClass('mtree-open').addClass('mtree-closed');
            el.children('a').first().children('span').first().removeClass('open').addClass('closed');
        } else {
            el.removeClass('mtree-closed').addClass('mtree-open');
            el.children('a').first().children('span').first().removeClass('closed').addClass('open');
        }
    }

}(jQuery, this, this.document));
