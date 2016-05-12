$(function() {
	var interval = setInterval(function() {
		var elements = $('.nav.level1').find('a[href]');
		
		if (elements.length != 0) {
			clearInterval(interval);
		}
		else {
			return;
		}
		
		elements.each(function (i, el) {
		  $(el).text($(el).text().replace('MyTested.Mvc.Builders.Contracts.', ''));
		  $(el).text($(el).text().replace('MyTested.Mvc', 'Common'));
		});
	}, 200);
})