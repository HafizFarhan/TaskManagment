// npm package: jquery-tags-input
// github link: https://github.com/xoxco/jQuery-Tags-Input

$(function() {
  'use strict';

  $('#tags').tagsInput({
    'width': '100%',
    'height': '75%',
    'interactive': true,
      'defaultText': 'Add the tag name and press &quot;Enter&quot;',
    'removeWithBackspace': true,
    'minChars': 0,
    'maxChars': 20,
    'placeholderColor': '#666666'
  });
  $("#tags_tag").css("width", "300px");
});