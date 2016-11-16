showdown.setOption('parseImgDimensions', true);
showdown.setOption('strikethrough', true);
showdown.setOption('tables', true);
showdown.setOption('tasklists', true);

var showdownConverter = new showdown.Converter({
    extensions: ['xssfilter']
});