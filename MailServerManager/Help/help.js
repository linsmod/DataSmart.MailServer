function argItems (theArgName) {
	sArgs = location.search.slice(1).split('&');
    r = '';
    for (var i = 0; i < sArgs.length; i++) {
        if (sArgs[i].slice(0,sArgs[i].indexof('=')) == theArgName) {
            r = sArgs[i].slice(sArgs[i].indexOf('=')+1);
            break;
        }
    }
    return (r.length > 0 ? unescape(r).split(',') : '')
}
